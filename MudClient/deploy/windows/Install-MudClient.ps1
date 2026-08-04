[CmdletBinding()]
param(
	[Parameter(Mandatory = $true)][string]$ArchivePath,
	[string]$InstallRoot = "C:\MudClient",
	[switch]$Migrate,
	[string]$ConfigRoot = "$env:ProgramData\FutureMUD\MudClient",
	[string]$CaddyExecutable,
	[string]$CaddyTaskName = 'FutureMUD Web Client HTTPS'
)

$ErrorActionPreference = 'Stop'
$principal = [Security.Principal.WindowsPrincipal]::new([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) { throw 'Run from an elevated PowerShell prompt.' }
if (-not (Test-Path -LiteralPath $ArchivePath)) { throw "Archive '$ArchivePath' was not found." }

$packageRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$packageName = Split-Path -Leaf $packageRoot
if ($packageName -notmatch '^mudclient-(?<version>\d+\.\d+\.\d+)-(?<runtime>win-x64)$') { throw 'The extracted package folder has an invalid name.' }
$version = $Matches.version
$runtime = $Matches.runtime
$manifestPath = Join-Path $env:TEMP "mudclient-update-manifest-$PID.json"
$signaturePath = Join-Path $env:TEMP "mudclient-update-manifest-$PID.sig"
try {
	Invoke-WebRequest https://futuremud.com/downloads/mudclient/latest/update-manifest.json -OutFile $manifestPath
	Invoke-WebRequest https://futuremud.com/downloads/mudclient/latest/update-manifest.sig -OutFile $signaturePath
	& (Join-Path $packageRoot 'tools\MudClientDeployment.exe') verify --manifest $manifestPath --signature $signaturePath --archive $ArchivePath --runtime $runtime
	if ($LASTEXITCODE -ne 0) { throw 'The archive did not pass signed release verification.' }
}
finally {
	Remove-Item -LiteralPath $manifestPath,$signaturePath -Force -ErrorAction SilentlyContinue
}

$releaseRoot = Join-Path $InstallRoot 'releases'
$releasePath = Join-Path $releaseRoot $version
$legacyProxy = Join-Path $InstallRoot 'proxy'
$legacyWeb = Join-Path $InstallRoot 'web'
$legacyReleasePath = $null
New-Item -ItemType Directory -Force -Path $releaseRoot, (Join-Path $ConfigRoot 'proxy'), (Join-Path $ConfigRoot 'web') | Out-Null
if ((Test-Path -LiteralPath $legacyProxy) -and -not (Get-Item -LiteralPath $legacyProxy).LinkType) {
	if (-not $Migrate) { throw 'A flat MudClient installation was found. Re-run with -Migrate.' }
	$legacyPath = Join-Path $releaseRoot ("legacy-" + (Get-Date -Format 'yyyyMMddHHmmss'))
	$legacyReleasePath = $legacyPath
	New-Item -ItemType Directory -Force -Path $legacyPath | Out-Null
	Move-Item -LiteralPath $legacyProxy -Destination (Join-Path $legacyPath 'proxy')
	Move-Item -LiteralPath $legacyWeb -Destination (Join-Path $legacyPath 'web')
	Copy-Item -LiteralPath (Join-Path $legacyPath 'proxy\appsettings.json') -Destination (Join-Path $ConfigRoot 'proxy\appsettings.json') -Force
	$legacyWebSettings = Join-Path $legacyPath 'web\wwwroot\appsettings.json'
	if (Test-Path -LiteralPath $legacyWebSettings) { Copy-Item -LiteralPath $legacyWebSettings -Destination (Join-Path $ConfigRoot 'web\appsettings.json') -Force }
}
if (Test-Path -LiteralPath $releasePath) { throw "Release $version is already staged." }
Copy-Item -LiteralPath $packageRoot -Destination $releasePath -Recurse
New-Item -ItemType Directory -Force -Path (Join-Path $InstallRoot 'deploy\windows') | Out-Null
$persistentCaddyfile = Join-Path $InstallRoot 'deploy\windows\Caddyfile'
if (-not (Test-Path -LiteralPath $persistentCaddyfile)) {
	Copy-Item -LiteralPath (Join-Path $releasePath 'deploy\windows\Caddyfile') -Destination $persistentCaddyfile
}
if ($CaddyExecutable) {
	if (-not (Test-Path -LiteralPath $CaddyExecutable)) { throw "Caddy executable '$CaddyExecutable' was not found." }
	& $CaddyExecutable validate --config $persistentCaddyfile --adapter caddyfile
	if ($LASTEXITCODE -ne 0) { throw 'Caddy configuration validation failed.' }
	$existingCaddyTask = Get-ScheduledTask -TaskName $CaddyTaskName -ErrorAction SilentlyContinue
	if ($existingCaddyTask) { Unregister-ScheduledTask -TaskName $CaddyTaskName -Confirm:$false }
	$action = New-ScheduledTaskAction -Execute $CaddyExecutable -Argument "run --config `"$persistentCaddyfile`" --adapter caddyfile" -WorkingDirectory (Split-Path -Parent $CaddyExecutable)
	$trigger = New-ScheduledTaskTrigger -AtStartup
	$principal = New-ScheduledTaskPrincipal -UserId 'SYSTEM' -LogonType ServiceAccount -RunLevel Highest
	$settings = New-ScheduledTaskSettingsSet -RestartCount 3 -RestartInterval (New-TimeSpan -Minutes 1) -ExecutionTimeLimit (New-TimeSpan -Days 0)
	Register-ScheduledTask -TaskName $CaddyTaskName -Action $action -Trigger $trigger -Principal $principal -Settings $settings -Description 'FutureMUD Web Client HTTPS reverse proxy.' | Out-Null
}
$proxySettings = Join-Path $ConfigRoot 'proxy\appsettings.json'
$webSettings = Join-Path $ConfigRoot 'web\appsettings.json'
if (-not (Test-Path -LiteralPath $proxySettings)) { Copy-Item -LiteralPath (Join-Path $releasePath 'proxy\appsettings.json') -Destination $proxySettings }
if (-not (Test-Path -LiteralPath $webSettings)) { Copy-Item -LiteralPath (Join-Path $releasePath 'web\wwwroot\appsettings.json') -Destination $webSettings }
Copy-Item -LiteralPath $webSettings -Destination (Join-Path $releasePath 'web\wwwroot\appsettings.json') -Force

$existingServiceInstaller = Join-Path $InstallRoot 'deploy\windows\install-mudclient-proxy.ps1'
$legacyTaskName = 'FutureMUD Mud WebSocket Proxy'
$legacyTaskBackup = Join-Path $ConfigRoot 'legacy-proxy-task.xml'
$legacyTask = Get-ScheduledTask -TaskName $legacyTaskName -ErrorAction SilentlyContinue
if ($legacyTask) {
	Export-ScheduledTask -TaskName $legacyTaskName | Set-Content -LiteralPath $legacyTaskBackup -Encoding UTF8
	Stop-ScheduledTask -TaskName $legacyTaskName -ErrorAction SilentlyContinue
	Disable-ScheduledTask -TaskName $legacyTaskName | Out-Null
}
$previousTarget = if (Test-Path -LiteralPath (Join-Path $InstallRoot 'current')) { (Get-Item -LiteralPath (Join-Path $InstallRoot 'current')).Target } else { $legacyReleasePath }
if (Test-Path -LiteralPath $existingServiceInstaller) {
	& $existingServiceInstaller -Uninstall -ErrorAction SilentlyContinue
}
try {
	foreach ($link in @('current', 'web', 'proxy')) {
		$linkPath = Join-Path $InstallRoot $link
		if (Test-Path -LiteralPath $linkPath) { Remove-Item -LiteralPath $linkPath -Force }
	}
	New-Item -ItemType SymbolicLink -Path (Join-Path $InstallRoot 'current') -Target $releasePath | Out-Null
	New-Item -ItemType SymbolicLink -Path (Join-Path $InstallRoot 'web') -Target (Join-Path $InstallRoot 'current\web') | Out-Null
	New-Item -ItemType SymbolicLink -Path (Join-Path $InstallRoot 'proxy') -Target (Join-Path $InstallRoot 'current\proxy') | Out-Null
	& (Join-Path $releasePath 'deploy\windows\install-mudclient-proxy.ps1') -InstallRoot $InstallRoot -SettingsPath $proxySettings
	Invoke-WebRequest http://127.0.0.1:5000/health | Out-Null
}
catch {
	if ($previousTarget) {
		Remove-Item -LiteralPath (Join-Path $InstallRoot 'current') -Force -ErrorAction SilentlyContinue
		New-Item -ItemType SymbolicLink -Path (Join-Path $InstallRoot 'current') -Target $previousTarget | Out-Null
		if (Test-Path -LiteralPath $existingServiceInstaller) { & $existingServiceInstaller -InstallRoot $InstallRoot }
	}
	if ($legacyTask -and (Test-Path -LiteralPath $legacyTaskBackup)) { schtasks.exe /Create /TN $legacyTaskName /XML $legacyTaskBackup /F | Out-Null }
	throw
}
$activeReleases = Get-ChildItem -LiteralPath $releaseRoot -Directory | Where-Object Name -notlike 'legacy-*' | Sort-Object Name
while ($activeReleases.Count -gt 3) {
	Remove-Item -LiteralPath $activeReleases[0].FullName -Recurse -Force
	$activeReleases = $activeReleases[1..($activeReleases.Count - 1)]
}
Write-Host "MudClient $version is active. Future upgrades: & '$InstallRoot\current\deploy\windows\Update-MudClient.ps1'"
