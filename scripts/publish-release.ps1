[CmdletBinding()]
param(
	[string]$RuntimeIdentifier,
	[string]$Configuration = "Release",
	[string]$OutputRoot = "artifacts\release",
	[switch]$SkipTests
)

$ErrorActionPreference = "Stop"

function Invoke-DotNet {
	param(
		[Parameter(Mandatory = $true)]
		[string[]]$Arguments
	)

	& dotnet @Arguments
	if ($LASTEXITCODE -ne 0) {
		throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
	}
}

function Get-DefaultRuntimeIdentifier {
	$osPlatform = [System.Runtime.InteropServices.OSPlatform]
	$runtimeInfo = [System.Runtime.InteropServices.RuntimeInformation]

	if ($runtimeInfo::IsOSPlatform($osPlatform::Windows)) {
		$os = "win"
	}
	elseif ($runtimeInfo::IsOSPlatform($osPlatform::Linux)) {
		$os = "linux"
	}
	else {
		throw "Automatic runtime detection only supports Windows and Linux. Pass -RuntimeIdentifier explicitly."
	}

	$architecture = $runtimeInfo::OSArchitecture.ToString().ToLowerInvariant()
	$arch = switch ($architecture) {
		"x64" { "x64" }
		"arm64" { "arm64" }
		default { throw "Unsupported architecture '$architecture'. Pass -RuntimeIdentifier explicitly." }
	}

	return "$os-$arch"
}

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

if ([string]::IsNullOrWhiteSpace($RuntimeIdentifier)) {
	$RuntimeIdentifier = Get-DefaultRuntimeIdentifier
}

$packageName = "mudclient-$RuntimeIdentifier"
$packageRoot = Join-Path $OutputRoot $packageName
$webPublish = Join-Path $OutputRoot "_web"
$proxyPublish = Join-Path $OutputRoot "_proxy-$RuntimeIdentifier"
$zipPath = Join-Path $OutputRoot "$packageName.zip"

foreach ($path in @($packageRoot, $webPublish, $proxyPublish)) {
	if (Test-Path $path) {
		Remove-Item -LiteralPath $path -Recurse -Force
	}
}

New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null

Invoke-DotNet @("restore", "MudClientSolution.sln")

if (-not $SkipTests) {
	Invoke-DotNet @("test", "MudClientSolution.sln", "-c", $Configuration, "--no-restore")
}

Invoke-DotNet @("publish", "MudClientBlazor/MudClientBlazor.csproj", "-c", $Configuration, "--no-restore", "-o", $webPublish)
Invoke-DotNet @("publish", "MudWebSocketProxy/MudWebSocketProxy.csproj", "-c", $Configuration, "-r", $RuntimeIdentifier, "--self-contained", "true", "-p:PublishSingleFile=true", "-o", $proxyPublish)

New-Item -ItemType Directory -Force -Path (Join-Path $packageRoot "web"), (Join-Path $packageRoot "proxy") | Out-Null
Copy-Item -Path (Join-Path $webPublish "*") -Destination (Join-Path $packageRoot "web") -Recurse -Force
Copy-Item -Path (Join-Path $proxyPublish "*") -Destination (Join-Path $packageRoot "proxy") -Recurse -Force
Copy-Item -Path "deploy" -Destination $packageRoot -Recurse -Force
Copy-Item -Path "DEPLOYMENT.md" -Destination $packageRoot -Force
Set-Content -Path (Join-Path $packageRoot "README.txt") -Value "Start with DEPLOYMENT.md. The Blazor static site is in web/wwwroot and the websocket proxy is in proxy/."

if (Test-Path $zipPath) {
	Remove-Item -LiteralPath $zipPath -Force
}

Compress-Archive -Path $packageRoot -DestinationPath $zipPath -Force

Write-Host "Release package created:"
Write-Host "  $packageRoot"
Write-Host "  $zipPath"
