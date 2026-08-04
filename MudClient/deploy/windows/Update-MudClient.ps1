[CmdletBinding()]
param(
	[string]$InstallRoot = 'C:\MudClient',
	[string]$ConfigRoot,
	[switch]$Check,
	[switch]$Rollback
)

$ErrorActionPreference = 'Stop'
$commonApplicationData = [Environment]::GetFolderPath([Environment+SpecialFolder]::CommonApplicationData)
if ([string]::IsNullOrWhiteSpace($ConfigRoot)) {
	if ([string]::IsNullOrWhiteSpace($commonApplicationData)) { throw 'Windows did not provide a common application-data directory.' }
	$ConfigRoot = Join-Path $commonApplicationData 'FutureMUD\MudClient'
}
$temporaryRoot = [System.IO.Path]::GetTempPath()
if ([string]::IsNullOrWhiteSpace($temporaryRoot)) { throw 'Windows did not provide a temporary directory.' }
$currentPath = Join-Path $InstallRoot 'current'
$deploymentTool = Join-Path $currentPath 'tools\MudClientDeployment.exe'
if (-not (Test-Path -LiteralPath $deploymentTool)) { throw 'The deployed MudClient update verifier is unavailable.' }

function Remove-MudClientPath {
	param([Parameter(Mandatory = $true)][string]$Path)

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

function Get-SignedLatestManifest {
	param([string]$Directory)
	$manifestPath = Join-Path $Directory 'update-manifest.json'
	$signaturePath = Join-Path $Directory 'update-manifest.sig'
	Invoke-WebRequest https://futuremud.com/downloads/mudclient/latest/update-manifest.json -OutFile $manifestPath
	Invoke-WebRequest https://futuremud.com/downloads/mudclient/latest/update-manifest.sig -OutFile $signaturePath
	& $deploymentTool verify-manifest --manifest $manifestPath --signature $signaturePath --runtime win-x64
	if ($LASTEXITCODE -ne 0) { throw 'The latest MudClient manifest failed signature validation.' }
	return $manifestPath
}

if ($Check) {
	$temp = Join-Path $temporaryRoot ("mudclient-update-check-" + [Guid]::NewGuid().ToString('N'))
	New-Item -ItemType Directory -Path $temp | Out-Null
	try {
		$manifestPath = Get-SignedLatestManifest -Directory $temp
		(Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json).version
	}
	finally {
		Remove-Item -LiteralPath $temp -Recurse -Force -ErrorAction SilentlyContinue
	}
	return
}

$principal = [Security.Principal.WindowsPrincipal]::new([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) { throw 'Run updates and rollback from an elevated PowerShell prompt.' }

if ($Rollback) {
	$originalTarget = [string](Get-Item -LiteralPath $currentPath).Target
	$currentVersion = Split-Path -Leaf $originalTarget
	$releases = @(Get-ChildItem -LiteralPath (Join-Path $InstallRoot 'releases') -Directory |
		Where-Object Name -notlike 'legacy-*' |
		Sort-Object { [version]$_.Name })
	$index = -1
	for ($candidateIndex = 0; $candidateIndex -lt $releases.Count; $candidateIndex++) {
		if ($releases[$candidateIndex].Name -eq $currentVersion) {
			$index = $candidateIndex
			break
		}
	}
	if ($index -lt 1) { throw 'No prior release is available.' }
	$previous = $releases[$index - 1]
	& (Join-Path $currentPath 'deploy\windows\install-mudclient-proxy.ps1') -Uninstall
	try {
		Remove-MudClientPath -Path $currentPath
		New-Item -ItemType SymbolicLink -Path $currentPath -Target $previous.FullName | Out-Null
		& (Join-Path $currentPath 'deploy\windows\install-mudclient-proxy.ps1') -InstallRoot $InstallRoot -SettingsPath (Join-Path $ConfigRoot 'proxy\appsettings.json')
		Wait-ForMudClientHealth
	}
	catch {
		$rollbackError = $_
		try { Wait-ForMudClientServiceRemoval } catch { Write-Warning $_.Exception.Message }
		try { Remove-MudClientPath -Path $currentPath } catch { Write-Warning "Could not restore the previous current release: $($_.Exception.Message)" }
		New-Item -ItemType SymbolicLink -Path $currentPath -Target $originalTarget | Out-Null
		& (Join-Path $currentPath 'deploy\windows\install-mudclient-proxy.ps1') -InstallRoot $InstallRoot -SettingsPath (Join-Path $ConfigRoot 'proxy\appsettings.json')
		throw $rollbackError
	}
	Write-Host "MudClient rolled back to $($previous.Name)."
	return
}

$temp = Join-Path $temporaryRoot ("mudclient-update-" + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temp | Out-Null
try {
	$manifestPath = Get-SignedLatestManifest -Directory $temp
	$version = (Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json).version
	$currentVersion = Split-Path -Leaf ([string](Get-Item -LiteralPath $currentPath).Target)
	if ([version]$version -eq [version]$currentVersion) {
		Write-Host "MudClient $version is already active."
		return
	}
	if ([version]$version -lt [version]$currentVersion) { throw "Refusing unsupported downgrade from $currentVersion to $version." }
	$archive = Join-Path $temp "mudclient-$version-win-x64.zip"
	Invoke-WebRequest https://futuremud.com/downloads/mudclient/latest/win-x64 -OutFile $archive
	& $deploymentTool verify --manifest $manifestPath --signature (Join-Path $temp 'update-manifest.sig') --archive $archive --runtime win-x64 --expected-version $version
	if ($LASTEXITCODE -ne 0) { throw 'The latest MudClient archive failed signature validation.' }
	Expand-Archive -LiteralPath $archive -DestinationPath $temp
	$package = Join-Path $temp "mudclient-$version-win-x64"
	if (-not (Test-Path -LiteralPath $package)) { throw 'The verified archive has an invalid package layout.' }
	& (Join-Path $package 'deploy\windows\Install-MudClient.ps1') -ArchivePath $archive -InstallRoot $InstallRoot -ConfigRoot $ConfigRoot
}
finally {
	Remove-Item -LiteralPath $temp -Recurse -Force -ErrorAction SilentlyContinue
}
