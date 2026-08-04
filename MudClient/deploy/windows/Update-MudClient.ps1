[CmdletBinding()]
param(
	[string]$InstallRoot = 'C:\MudClient',
	[switch]$Check,
	[switch]$Rollback
)

$ErrorActionPreference = 'Stop'
if ($Check) { Invoke-RestMethod https://futuremud.com/downloads/mudclient/latest/update-manifest.json | ConvertTo-Json -Depth 5; return }
if ($Rollback) {
	$releases = Get-ChildItem -LiteralPath (Join-Path $InstallRoot 'releases') -Directory | Where-Object Name -notlike 'legacy-*' | Sort-Object Name
	if ($releases.Count -lt 2) { throw 'No prior release is available.' }
	& (Join-Path $InstallRoot 'deploy\windows\install-mudclient-proxy.ps1') -Uninstall
	Remove-Item -LiteralPath (Join-Path $InstallRoot 'current') -Force
	New-Item -ItemType SymbolicLink -Path (Join-Path $InstallRoot 'current') -Target $releases[-2].FullName | Out-Null
	& (Join-Path $InstallRoot 'current\deploy\windows\install-mudclient-proxy.ps1') -InstallRoot $InstallRoot
	Invoke-WebRequest http://127.0.0.1:5000/health | Out-Null
	return
}

$temp = Join-Path $env:TEMP ("mudclient-update-" + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temp | Out-Null
try {
	$manifest = Invoke-RestMethod https://futuremud.com/downloads/mudclient/latest/update-manifest.json
	$version = $manifest.version
	$archive = Join-Path $temp "mudclient-$version-win-x64.zip"
	Invoke-WebRequest https://futuremud.com/downloads/mudclient/latest/win-x64 -OutFile $archive
	Expand-Archive -LiteralPath $archive -DestinationPath $temp
	$package = Join-Path $temp "mudclient-$version-win-x64"
	& (Join-Path $package 'deploy\windows\Install-MudClient.ps1') -ArchivePath $archive -InstallRoot $InstallRoot
}
finally {
	Remove-Item -LiteralPath $temp -Recurse -Force -ErrorAction SilentlyContinue
}
