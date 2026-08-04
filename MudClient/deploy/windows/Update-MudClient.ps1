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
		Remove-Item -LiteralPath $currentPath -Force
		New-Item -ItemType SymbolicLink -Path $currentPath -Target $previous.FullName | Out-Null
		& (Join-Path $currentPath 'deploy\windows\install-mudclient-proxy.ps1') -InstallRoot $InstallRoot -SettingsPath (Join-Path $ConfigRoot 'proxy\appsettings.json')
		Invoke-WebRequest http://127.0.0.1:5000/health | Out-Null
	}
	catch {
		Remove-Item -LiteralPath $currentPath -Force -ErrorAction SilentlyContinue
		New-Item -ItemType SymbolicLink -Path $currentPath -Target $originalTarget | Out-Null
		& (Join-Path $currentPath 'deploy\windows\install-mudclient-proxy.ps1') -InstallRoot $InstallRoot -SettingsPath (Join-Path $ConfigRoot 'proxy\appsettings.json')
		throw
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
