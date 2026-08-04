[CmdletBinding()]
param(
	[Parameter(Mandatory = $true)][string]$ArchivePath,
	[string]$InstallRoot = "C:\MudClient",
	[switch]$Migrate,
	[string]$ConfigRoot,
	[string]$CaddyExecutable,
	[string]$CaddyDomain,
	[string]$CaddyTaskName = 'FutureMUD Web Client HTTPS'
)

$ErrorActionPreference = 'Stop'
$commonApplicationData = [Environment]::GetFolderPath([Environment+SpecialFolder]::CommonApplicationData)
if ([string]::IsNullOrWhiteSpace($ConfigRoot)) {
	if ([string]::IsNullOrWhiteSpace($commonApplicationData)) { throw 'Windows did not provide a common application-data directory.' }
	$ConfigRoot = Join-Path $commonApplicationData 'FutureMUD\MudClient'
}
$principal = [Security.Principal.WindowsPrincipal]::new([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) { throw 'Run from an elevated PowerShell prompt.' }
if (-not (Test-Path -LiteralPath $ArchivePath)) { throw "Archive '$ArchivePath' was not found." }

$packageRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$packageName = Split-Path -Leaf $packageRoot
if ($packageName -notmatch '^mudclient-(?<version>\d+\.\d+\.\d+)-(?<runtime>win-x64)$') { throw 'The extracted package folder has an invalid name.' }
$version = $Matches.version
$runtime = $Matches.runtime
$temporaryRoot = [System.IO.Path]::GetTempPath()
if ([string]::IsNullOrWhiteSpace($temporaryRoot)) { throw 'Windows did not provide a temporary directory.' }

function Remove-MudClientPath {
	param([AllowNull()][AllowEmptyString()][string]$Path)

	if ([string]::IsNullOrWhiteSpace($Path)) { return }

	$item = Get-Item -LiteralPath $Path -Force -ErrorAction SilentlyContinue
	if (-not $item) { return }
	if (($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -eq 0) {
		Remove-Item -LiteralPath $Path -Recurse -Force
		return
	}

	& cmd.exe /d /c "rmdir /q `"$Path`"" | Out-Null
	if ($LASTEXITCODE -ne 0) {
		throw "Windows could not remove the reparse point '$Path' (exit code $LASTEXITCODE)."
	}
}

function Remove-MudClientFile {
	param([AllowNull()][AllowEmptyString()][string]$Path)

	if ([string]::IsNullOrWhiteSpace($Path)) { return }
	if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { return }
	Remove-Item -LiteralPath $Path -Force -ErrorAction SilentlyContinue
}

function Wait-ForMudClientServiceRemoval {
	param([int]$TimeoutSeconds = 30)

	$deadline = (Get-Date).AddSeconds($TimeoutSeconds)
	do {
		$service = Get-Service -Name 'MudClientProxy' -ErrorAction SilentlyContinue
		if (-not $service) { return }
		if ($service.Status -ne 'Stopped') {
			Stop-Service -Name 'MudClientProxy' -Force -ErrorAction SilentlyContinue
		}
		Start-Sleep -Milliseconds 500
	} while ((Get-Date) -lt $deadline)

	throw "The MudClientProxy service did not finish stopping before the activation timeout."
}

function Wait-ForMudClientHealth {
	param([int]$TimeoutSeconds = 30)

	$deadline = (Get-Date).AddSeconds($TimeoutSeconds)
	do {
		try {
			$response = Invoke-WebRequest -UseBasicParsing -TimeoutSec 2 -Uri 'http://127.0.0.1:5000/health'
			if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 300) { return }
		}
		catch {
			# The service may still be starting. Retry until the bounded deadline.
		}
		Start-Sleep -Seconds 1
	} while ((Get-Date) -lt $deadline)

	throw "The MudClientProxy service did not become healthy within $TimeoutSeconds seconds."
}

$manifestPath = Join-Path $temporaryRoot "mudclient-update-manifest-$PID.json"
$signaturePath = Join-Path $temporaryRoot "mudclient-update-manifest-$PID.sig"
try {
	Invoke-WebRequest https://futuremud.com/downloads/mudclient/latest/update-manifest.json -OutFile $manifestPath
	Invoke-WebRequest https://futuremud.com/downloads/mudclient/latest/update-manifest.sig -OutFile $signaturePath
	& (Join-Path $packageRoot 'tools\MudClientDeployment.exe') verify --manifest $manifestPath --signature $signaturePath --archive $ArchivePath --runtime $runtime --expected-version $version
	if ($LASTEXITCODE -ne 0) { throw 'The archive did not pass signed release verification.' }
}
finally {
	foreach ($temporaryFile in @($manifestPath, $signaturePath)) {
		Remove-MudClientFile -Path ([string]$temporaryFile)
	}
}

$releaseRoot = Join-Path $InstallRoot 'releases'
$releasePath = Join-Path $releaseRoot $version
$currentPath = Join-Path $InstallRoot 'current'
if (Test-Path -LiteralPath $currentPath) {
	$currentVersion = Split-Path -Leaf ([string](Get-Item -LiteralPath $currentPath).Target)
	if ($currentVersion -match '^\d+\.\d+\.\d+$' -and [version]$version -lt [version]$currentVersion) {
		throw "Refusing unsupported downgrade from $currentVersion to $version."
	}
}
$legacyProxy = Join-Path $InstallRoot 'proxy'
$legacyWeb = Join-Path $InstallRoot 'web'
$legacyReleasePath = $null
$previousTarget = if (Test-Path -LiteralPath $currentPath) { [string](Get-Item -LiteralPath $currentPath).Target } else { $null }
$persistentCaddyfile = Join-Path $InstallRoot 'deploy\windows\Caddyfile'
$caddyFileCreated = $false
$createCaddyTask = $false
if ($CaddyExecutable) {
	if (-not (Test-Path -LiteralPath $CaddyExecutable)) { throw "Caddy executable '$CaddyExecutable' was not found." }
	if (Get-ScheduledTask -TaskName $CaddyTaskName -ErrorAction SilentlyContinue) { throw "Caddy task '$CaddyTaskName' already exists and will not be replaced." }
	if (-not $CaddyDomain -and -not (Test-Path -LiteralPath $persistentCaddyfile)) { throw '-CaddyDomain is required when creating the managed Caddyfile.' }
}
New-Item -ItemType Directory -Force -Path (Join-Path $InstallRoot 'deploy\windows') | Out-Null
if (-not (Test-Path -LiteralPath $persistentCaddyfile)) {
	$caddyTemplate = Get-Content -LiteralPath (Join-Path $packageRoot 'deploy\windows\Caddyfile') -Raw
	if ($CaddyDomain) { $caddyTemplate = $caddyTemplate.Replace('play.example.com', $CaddyDomain).Replace('C:\MudClient', $InstallRoot) }
	Set-Content -LiteralPath $persistentCaddyfile -Value $caddyTemplate -Encoding UTF8
	$caddyFileCreated = $true
}
if ($CaddyExecutable) {
	try {
		& $CaddyExecutable validate --config $persistentCaddyfile --adapter caddyfile
		if ($LASTEXITCODE -ne 0) { throw 'Caddy configuration validation failed.' }
		$createCaddyTask = $true
	}
	catch {
		if ($caddyFileCreated) { Remove-MudClientFile -Path $persistentCaddyfile }
		throw
	}
}
try {
New-Item -ItemType Directory -Force -Path $releaseRoot, (Join-Path $ConfigRoot 'proxy'), (Join-Path $ConfigRoot 'web') | Out-Null
if ((Test-Path -LiteralPath $legacyProxy) -and -not (Get-Item -LiteralPath $legacyProxy).LinkType) {
	if (-not $Migrate) { throw 'A flat MudClient installation was found. Re-run with -Migrate.' }
	$legacyPath = Join-Path $releaseRoot ("legacy-" + (Get-Date -Format 'yyyyMMddHHmmss'))
	$legacyReleasePath = $legacyPath
	if (-not $previousTarget) { $previousTarget = $legacyReleasePath }
	New-Item -ItemType Directory -Force -Path $legacyPath | Out-Null
	Move-Item -LiteralPath $legacyProxy -Destination (Join-Path $legacyPath 'proxy')
	Move-Item -LiteralPath $legacyWeb -Destination (Join-Path $legacyPath 'web')
	if (-not (Test-Path -LiteralPath (Join-Path $ConfigRoot 'proxy\appsettings.json'))) {
		Copy-Item -LiteralPath (Join-Path $legacyPath 'proxy\appsettings.json') -Destination (Join-Path $ConfigRoot 'proxy\appsettings.json')
	}
	$legacyWebSettings = Join-Path $legacyPath 'web\wwwroot\appsettings.json'
	if ((Test-Path -LiteralPath $legacyWebSettings) -and -not (Test-Path -LiteralPath (Join-Path $ConfigRoot 'web\appsettings.json'))) {
		Copy-Item -LiteralPath $legacyWebSettings -Destination (Join-Path $ConfigRoot 'web\appsettings.json')
	}
}
if (Test-Path -LiteralPath $releasePath) { throw "Release $version is already staged." }
Copy-Item -LiteralPath $packageRoot -Destination $releasePath -Recurse
$proxySettings = Join-Path $ConfigRoot 'proxy\appsettings.json'
$webSettings = Join-Path $ConfigRoot 'web\appsettings.json'
if (-not (Test-Path -LiteralPath $proxySettings)) { Copy-Item -LiteralPath (Join-Path $releasePath 'proxy\appsettings.json') -Destination $proxySettings }
if (-not (Test-Path -LiteralPath $webSettings)) { Copy-Item -LiteralPath (Join-Path $releasePath 'web\wwwroot\appsettings.json') -Destination $webSettings }
Copy-Item -LiteralPath $webSettings -Destination (Join-Path $releasePath 'web\wwwroot\appsettings.json') -Force

$existingServiceInstaller = if (Test-Path -LiteralPath $currentPath) {
	Join-Path $currentPath 'deploy\windows\install-mudclient-proxy.ps1'
}
else {
	Join-Path $InstallRoot 'deploy\windows\install-mudclient-proxy.ps1'
}
$releaseServiceInstaller = Join-Path $releasePath 'deploy\windows\install-mudclient-proxy.ps1'
$legacyTaskName = 'FutureMUD Mud WebSocket Proxy'
$legacyTaskBackup = Join-Path $ConfigRoot 'legacy-proxy-task.xml'
$serviceBackup = Join-Path $ConfigRoot 'mudclient-proxy-service.txt'
$existingService = Get-Service -Name MudClientProxy -ErrorAction SilentlyContinue
if ($existingService) { sc.exe qc MudClientProxy | Set-Content -LiteralPath $serviceBackup -Encoding UTF8 }
$legacyTask = Get-ScheduledTask -TaskName $legacyTaskName -ErrorAction SilentlyContinue
if ($legacyTask) {
	Export-ScheduledTask -TaskName $legacyTaskName | Set-Content -LiteralPath $legacyTaskBackup -Encoding UTF8
	Stop-ScheduledTask -TaskName $legacyTaskName -ErrorAction SilentlyContinue
	Disable-ScheduledTask -TaskName $legacyTaskName | Out-Null
}
if (Test-Path -LiteralPath $existingServiceInstaller) {
	& $existingServiceInstaller -Uninstall -ErrorAction SilentlyContinue
}
	Wait-ForMudClientServiceRemoval
	foreach ($link in @('current', 'web', 'proxy')) {
		$linkPath = Join-Path $InstallRoot $link
		Remove-MudClientPath -Path $linkPath
	}
		New-Item -ItemType SymbolicLink -Path $currentPath -Target $releasePath | Out-Null
	New-Item -ItemType SymbolicLink -Path (Join-Path $InstallRoot 'web') -Target (Join-Path $InstallRoot 'current\web') | Out-Null
	New-Item -ItemType SymbolicLink -Path (Join-Path $InstallRoot 'proxy') -Target (Join-Path $InstallRoot 'current\proxy') | Out-Null
		& $releaseServiceInstaller -InstallRoot $InstallRoot -SettingsPath $proxySettings
		Wait-ForMudClientHealth
		if ($createCaddyTask) {
			$action = New-ScheduledTaskAction -Execute $CaddyExecutable -Argument "run --config `"$persistentCaddyfile`" --adapter caddyfile" -WorkingDirectory (Split-Path -Parent $CaddyExecutable)
			$trigger = New-ScheduledTaskTrigger -AtStartup
			$caddyPrincipal = New-ScheduledTaskPrincipal -UserId 'SYSTEM' -LogonType ServiceAccount -RunLevel Highest
			$settings = New-ScheduledTaskSettingsSet -RestartCount 3 -RestartInterval (New-TimeSpan -Minutes 1) -ExecutionTimeLimit (New-TimeSpan -Days 0)
			Register-ScheduledTask -TaskName $CaddyTaskName -Action $action -Trigger $trigger -Principal $caddyPrincipal -Settings $settings -Description 'FutureMUD Web Client HTTPS reverse proxy.' | Out-Null
			Start-ScheduledTask -TaskName $CaddyTaskName
		}
}
catch {
	$activationError = $_
	if ($releaseServiceInstaller -and (Test-Path -LiteralPath $releaseServiceInstaller)) { & $releaseServiceInstaller -Uninstall -ErrorAction SilentlyContinue }
	try {
		Wait-ForMudClientServiceRemoval
	}
	catch {
		Write-Warning $_.Exception.Message
	}
	if ($previousTarget) {
		try {
			Remove-MudClientPath -Path $currentPath
			New-Item -ItemType SymbolicLink -Path $currentPath -Target $previousTarget | Out-Null
		}
		catch {
			Write-Warning "Could not restore the previous current release: $($_.Exception.Message)"
		}
		foreach ($link in @('web', 'proxy')) {
			$linkPath = Join-Path $InstallRoot $link
			if (-not (Get-Item -LiteralPath $linkPath -Force -ErrorAction SilentlyContinue)) { New-Item -ItemType SymbolicLink -Path $linkPath -Target (Join-Path $currentPath $link) | Out-Null }
		}
		if (Test-Path -LiteralPath $existingServiceInstaller) { & $existingServiceInstaller -InstallRoot $InstallRoot }
	}
	if ($legacyTask -and (Test-Path -LiteralPath $legacyTaskBackup)) { schtasks.exe /Create /TN $legacyTaskName /XML $legacyTaskBackup /F | Out-Null }
	if ($createCaddyTask) { Unregister-ScheduledTask -TaskName $CaddyTaskName -Confirm:$false -ErrorAction SilentlyContinue }
	if ($caddyFileCreated) { Remove-MudClientFile -Path $persistentCaddyfile }
	throw $activationError
}
$activeReleases = Get-ChildItem -LiteralPath $releaseRoot -Directory | Where-Object Name -notlike 'legacy-*' | Sort-Object { [version]$_.Name }
while ($activeReleases.Count -gt 3) {
	$oldestRelease = $activeReleases | Select-Object -First 1
	if ($oldestRelease -and -not [string]::IsNullOrWhiteSpace([string]$oldestRelease.FullName)) {
		Remove-MudClientPath -Path $oldestRelease.FullName
	}
	$activeReleases = $activeReleases[1..($activeReleases.Count - 1)]
}
Write-Host "MudClient $version is active. Future upgrades: & '$InstallRoot\current\deploy\windows\Update-MudClient.ps1'"
