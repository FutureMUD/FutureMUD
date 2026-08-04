[CmdletBinding()]
param(
	[string]$InstallRoot = "C:\MudClient",
	[string]$SettingsPath = "C:\ProgramData\FutureMUD\MudClient\proxy\appsettings.json",
	[string]$ServiceName = "MudClientProxy",
	[int]$Port = 5000,
	[switch]$Uninstall
)

$ErrorActionPreference = "Stop"

$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
	throw "Run this script from an elevated PowerShell prompt."
}

if ($Uninstall) {
	$existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
	if ($existingService) {
		if ($existingService.Status -ne "Stopped") {
			Stop-Service -Name $ServiceName -Force
		}

		sc.exe delete $ServiceName | Out-Null
		Write-Host "Removed service '$ServiceName'."
	}
	else {
		Write-Host "Service '$ServiceName' was not installed."
	}

	return
}

$proxyExe = Join-Path $InstallRoot "proxy\MudWebSocketProxy.exe"
if (-not (Test-Path $proxyExe)) {
	throw "Could not find proxy executable at '$proxyExe'. Unzip the win-x64 release package to '$InstallRoot' first."
}

$existing = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($existing) {
	if ($existing.Status -ne "Stopped") {
		Stop-Service -Name $ServiceName -Force
	}

	sc.exe delete $ServiceName | Out-Null
	Start-Sleep -Seconds 2
}

$binaryPath = "`"$proxyExe`" --urls http://127.0.0.1:$Port --settings `"$SettingsPath`""
New-Service `
	-Name $ServiceName `
	-BinaryPathName $binaryPath `
	-DisplayName "MUD Client WebSocket Proxy" `
	-Description "Local websocket-to-telnet proxy for the Blazor MUD client." `
	-StartupType Automatic | Out-Null

Start-Service -Name $ServiceName
Write-Host "Installed and started '$ServiceName' on http://127.0.0.1:$Port."
