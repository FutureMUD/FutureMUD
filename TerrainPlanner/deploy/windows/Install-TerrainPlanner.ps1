[CmdletBinding()]
param(
	[Parameter(Mandatory = $true)][ValidatePattern('^[A-Za-z0-9.-]+$')][string]$Hostname,
	[string]$InstallRoot = "$env:ProgramFiles\FutureMUD\TerrainPlanner",
	[string]$CaddyExecutable,
	[string]$Caddyfile
)

$ErrorActionPreference = 'Stop'
$packageRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$version = (Get-Content -LiteralPath (Join-Path $packageRoot 'version.txt') -Raw).Trim()
$releaseRoot = Join-Path $InstallRoot "releases\$version"
$sharedRoot = Join-Path $InstallRoot 'shared'
$configPath = Join-Path $sharedRoot 'appsettings.Production.json'
$currentPath = Join-Path $InstallRoot 'current'
$previousTarget = if (Test-Path -LiteralPath $currentPath) { (Get-Item -LiteralPath $currentPath).Target } else { $null }

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
if (Test-Path -LiteralPath $currentPath) { Remove-Item -LiteralPath $currentPath -Force }
New-Item -ItemType Junction -Path $currentPath -Target $releaseRoot | Out-Null

$exe = Join-Path $currentPath 'app\TerrainPlanner.exe'
$serviceCommand = "`"$exe`" --windows-service"
if (-not [Diagnostics.EventLog]::SourceExists('FutureMUD Terrain Planner')) {
	New-EventLog -LogName Application -Source 'FutureMUD Terrain Planner'
}

$existingService = Get-Service FutureMUDTerrainPlanner -ErrorAction SilentlyContinue
$serviceWasCreated = $false
if ($existingService) {
	Stop-Service FutureMUDTerrainPlanner -ErrorAction SilentlyContinue
	sc.exe config FutureMUDTerrainPlanner binPath= $serviceCommand start= auto | Out-Null
	if ($LASTEXITCODE -ne 0) { throw 'Could not update the Terrain Planner service command.' }
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
if (-not $healthy) {
	Stop-Service FutureMUDTerrainPlanner -ErrorAction SilentlyContinue
	Remove-Item -LiteralPath $currentPath -Force
	if ($previousTarget) { New-Item -ItemType Junction -Path $currentPath -Target $previousTarget | Out-Null; Start-Service FutureMUDTerrainPlanner }
	if ($serviceWasCreated) { sc.exe delete FutureMUDTerrainPlanner | Out-Null }
	Remove-Item -LiteralPath $releaseRoot -Recurse -Force
	throw 'Health check failed; the previous release was restored.'
}
Get-ChildItem (Join-Path $InstallRoot 'releases') -Directory | Sort-Object LastWriteTimeUtc -Descending | Select-Object -Skip 3 | Remove-Item -Recurse -Force
& (Join-Path $packageRoot 'deploy\windows\Install-CaddySite.ps1') -Hostname $Hostname -CaddyExecutable $CaddyExecutable -Caddyfile $Caddyfile
Write-Host "Terrain Planner $version is healthy at https://$Hostname."
