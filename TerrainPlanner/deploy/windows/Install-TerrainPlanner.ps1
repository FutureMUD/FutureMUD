[CmdletBinding()]
param(
	[Parameter(Mandatory = $true)][ValidatePattern('^[A-Za-z0-9.-]+$')][string]$Hostname,
	[string]$InstallRoot = "$env:ProgramFiles\FutureMUD\TerrainPlanner",
	[string]$CaddyExecutable,
	[string]$Caddyfile
)

$ErrorActionPreference = 'Stop'

function Remove-CurrentReleaseJunction {
	param([Parameter(Mandatory = $true)][string]$Path)

	if (-not (Test-Path -LiteralPath $Path)) { return }
	$item = Get-Item -LiteralPath $Path -Force
	if (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne [IO.FileAttributes]::ReparsePoint) {
		throw "Refusing to remove '$Path' because it is not a release junction."
	}

	# Directory.Delete removes the junction itself; it does not recurse into or remove its target release.
	[IO.Directory]::Delete($Path)
}

function Invoke-ServiceControl {
	param(
		[Parameter(Mandatory = $true)][string[]]$Arguments,
		[Parameter(Mandatory = $true)][string]$FailureMessage
	)

	$startInfo = [Diagnostics.ProcessStartInfo]::new()
	$startInfo.FileName = Join-Path $env:SystemRoot 'System32\sc.exe'
	$startInfo.UseShellExecute = $false
	$startInfo.RedirectStandardOutput = $true
	$startInfo.RedirectStandardError = $true
	foreach ($argument in $Arguments) {
		[void]$startInfo.ArgumentList.Add($argument)
	}

	$process = [Diagnostics.Process]::new()
	$process.StartInfo = $startInfo
	[void]$process.Start()
	$standardOutput = $process.StandardOutput.ReadToEnd()
	$standardError = $process.StandardError.ReadToEnd()
	$process.WaitForExit()
	if ($process.ExitCode -ne 0) {
		$detail = ($standardOutput, $standardError | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }) -join [Environment]::NewLine
		throw "$FailureMessage sc.exe exited with code $($process.ExitCode). $detail"
	}
}

$packageRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$version = (Get-Content -LiteralPath (Join-Path $packageRoot 'version.txt') -Raw).Trim()
$releaseRoot = Join-Path $InstallRoot "releases\$version"
$sharedRoot = Join-Path $InstallRoot 'shared'
$configPath = Join-Path $sharedRoot 'appsettings.Production.json'
$currentPath = Join-Path $InstallRoot 'current'
$previousTarget = $null
if (Test-Path -LiteralPath $currentPath) {
	$currentItem = Get-Item -LiteralPath $currentPath -Force
	if (($currentItem.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne [IO.FileAttributes]::ReparsePoint) {
		throw "Refusing to replace '$currentPath' because it is not a release junction."
	}
	$previousTarget = $currentItem.Target
}

New-Item -ItemType Directory -Force -Path (Join-Path $InstallRoot 'releases'), $sharedRoot, (Join-Path $sharedRoot 'keys') | Out-Null
if (-not (Test-Path -LiteralPath $configPath)) {
	$template = Get-Content -LiteralPath (Join-Path $packageRoot 'deploy\appsettings.Production.template.json') -Raw
	$template = $template.Replace('REPLACE_WITH_DURABLE_KEY_PATH', (Join-Path $sharedRoot 'keys').Replace('\', '\\')).Replace('REPLACE_WITH_PLANNER_HOSTNAME', $Hostname)
	[IO.File]::WriteAllText($configPath, $template, [Text.UTF8Encoding]::new($false))
	throw "Created $configPath. Replace REPLACE_WITH_SECRET with the least-privilege MySQL password, protect the file ACL, then rerun."
}
if (Test-Path -LiteralPath $releaseRoot) { throw "Release $version is already installed." }
New-Item -ItemType Directory -Path $releaseRoot | Out-Null
Copy-Item -LiteralPath (Join-Path $packageRoot 'app'), (Join-Path $packageRoot 'tools'), (Join-Path $packageRoot 'deploy'), (Join-Path $packageRoot 'DEPLOYMENT.md') -Destination $releaseRoot -Recurse -Force
Copy-Item -LiteralPath $configPath -Destination (Join-Path $releaseRoot 'app\appsettings.Production.json') -Force
Remove-CurrentReleaseJunction -Path $currentPath
New-Item -ItemType Junction -Path $currentPath -Target $releaseRoot | Out-Null

$exe = Join-Path $currentPath 'app\TerrainPlanner.exe'
$serviceCommand = "`"$exe`" --windows-service"
$existingService = Get-Service FutureMUDTerrainPlanner -ErrorAction SilentlyContinue
$serviceWasCreated = $false
try {
	if (-not [Diagnostics.EventLog]::SourceExists('FutureMUD Terrain Planner')) {
		New-EventLog -LogName Application -Source 'FutureMUD Terrain Planner'
	}

	if ($existingService) {
		Stop-Service FutureMUDTerrainPlanner -ErrorAction SilentlyContinue
		Invoke-ServiceControl -Arguments @('config', 'FutureMUDTerrainPlanner', 'binPath=', $serviceCommand, 'start=', 'auto') -FailureMessage 'Could not update the Terrain Planner service command.'
	} else {
		New-Service -Name FutureMUDTerrainPlanner -BinaryPathName $serviceCommand -StartupType Automatic -DisplayName 'FutureMUD Terrain Planner and Engine API'
		$serviceWasCreated = $true
	}

	# LocalService is a low-privilege built-in service account. The unrestricted service SID below gives only this service
	# access to its durable configuration and data-protection keys.
	sc.exe --% config FutureMUDTerrainPlanner obj= "NT AUTHORITY\LocalService" password= ""
	if ($LASTEXITCODE -ne 0) { throw 'Could not configure the Terrain Planner service account.' }
	sc.exe --% sidtype FutureMUDTerrainPlanner unrestricted
	if ($LASTEXITCODE -ne 0) { throw 'Could not enable the Terrain Planner service SID.' }
	& icacls.exe $sharedRoot /inheritance:r /grant:r 'Administrators:(OI)(CI)F' 'SYSTEM:(OI)(CI)F' 'NT SERVICE\FutureMUDTerrainPlanner:(OI)(CI)M' | Out-Null
	if ($LASTEXITCODE -ne 0) { throw 'Could not protect the Terrain Planner shared data directory.' }
	sc.exe failure FutureMUDTerrainPlanner reset= 86400 actions= restart/5000/restart/15000/none/0 | Out-Null
	if ($LASTEXITCODE -ne 0) { throw 'Could not configure Terrain Planner service recovery.' }
	Start-Service FutureMUDTerrainPlanner

	$healthy = $false
	for ($attempt = 0; $attempt -lt 30; $attempt++) {
		try { Invoke-RestMethod http://127.0.0.1:5010/health/ready | Out-Null; $healthy = $true; break } catch { Start-Sleep -Seconds 1 }
	}
	if (-not $healthy) { throw 'The new release did not become ready within 30 seconds.' }
} catch {
	$activationError = $_
	Stop-Service FutureMUDTerrainPlanner -ErrorAction SilentlyContinue
	try {
		Remove-CurrentReleaseJunction -Path $currentPath
		if ($previousTarget) {
			New-Item -ItemType Junction -Path $currentPath -Target $previousTarget | Out-Null
		}
		if ($serviceWasCreated) {
			sc.exe delete FutureMUDTerrainPlanner | Out-Null
		} elseif ($previousTarget) {
			Start-Service FutureMUDTerrainPlanner
		}
		Remove-Item -LiteralPath $releaseRoot -Recurse -Force
	} catch {
		throw "Terrain Planner activation failed and rollback also failed. Activation error: $($activationError.Exception.Message) Rollback error: $($_.Exception.Message)"
	}

	throw "Terrain Planner activation failed; the previous release was restored. $($activationError.Exception.Message)"
}
Get-ChildItem (Join-Path $InstallRoot 'releases') -Directory | Sort-Object LastWriteTimeUtc -Descending | Select-Object -Skip 3 | Remove-Item -Recurse -Force
& (Join-Path $packageRoot 'deploy\windows\Install-CaddySite.ps1') -Hostname $Hostname -CaddyExecutable $CaddyExecutable -Caddyfile $Caddyfile
Write-Host "Terrain Planner $version is healthy at https://$Hostname."
