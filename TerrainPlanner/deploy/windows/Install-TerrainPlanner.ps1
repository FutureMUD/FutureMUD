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

function Get-ReleaseJunctionTarget {
	param([Parameter(Mandatory = $true)][string]$Path)

	if (-not (Test-Path -LiteralPath $Path)) { return $null }
	$item = Get-Item -LiteralPath $Path -Force
	if (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne [IO.FileAttributes]::ReparsePoint) {
		throw "Refusing to replace '$Path' because it is not a release junction."
	}

	# Windows PowerShell returns DirectoryInfo.Target as a one-element string array for a junction.
	$targets = @($item.Target)
	if ($targets.Count -ne 1 -or [string]::IsNullOrWhiteSpace([string]$targets[0])) {
		throw "Could not determine the target for release junction '$Path'."
	}

	return [string]$targets[0]
}

$packageRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$version = (Get-Content -LiteralPath (Join-Path $packageRoot 'version.txt') -Raw).Trim()
$releaseRoot = Join-Path $InstallRoot "releases\$version"
$sharedRoot = Join-Path $InstallRoot 'shared'
$configPath = Join-Path $sharedRoot 'appsettings.Production.json'
$currentPath = Join-Path $InstallRoot 'current'
$previousTarget = Get-ReleaseJunctionTarget -Path $currentPath

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
$existingService = Get-Service FutureMUDTerrainPlanner -ErrorAction SilentlyContinue
$serviceWasCreated = $false
try {
	Remove-CurrentReleaseJunction -Path $currentPath
	New-Item -ItemType Junction -Path $currentPath -Target $releaseRoot | Out-Null

	$exe = Join-Path $currentPath 'app\TerrainPlanner.exe'
	$serviceCommand = "`"$exe`" --windows-service"
	if (-not [Diagnostics.EventLog]::SourceExists('FutureMUD Terrain Planner')) {
		New-EventLog -LogName Application -Source 'FutureMUD Terrain Planner'
	}

	if ($existingService) {
		if ($existingService.Status -ne 'Stopped') {
			Stop-Service FutureMUDTerrainPlanner -ErrorAction Stop
		}
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
