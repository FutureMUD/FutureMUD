[CmdletBinding()]
param(
	[Parameter(Mandatory = $true)]
	[ValidatePattern('^(win|linux)-(x64|arm64)$')]
	[string]$RuntimeIdentifier,
	[Parameter(Mandatory = $true)]
	[ValidatePattern('^\d+\.\d+\.\d+$')]
	[string]$Version,
	[string]$Configuration = "Release",
	[string]$OutputRoot = "artifacts\release",
	[switch]$SkipTests
)

$ErrorActionPreference = "Stop"

function Invoke-DotNet {
	param([Parameter(Mandatory = $true)][string[]]$Arguments)

	& dotnet @Arguments
	if ($LASTEXITCODE -ne 0) {
		throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
	}
}

$clientRoot = Split-Path -Parent $PSScriptRoot
Set-Location $clientRoot

$packageName = "mudclient-$Version-$RuntimeIdentifier"
$packageRoot = Join-Path $OutputRoot $packageName
$webPublish = Join-Path $OutputRoot "_web"
$proxyPublish = Join-Path $OutputRoot "_proxy-$RuntimeIdentifier"
$deploymentPublish = Join-Path $OutputRoot "_deployment-$RuntimeIdentifier"
$zipPath = Join-Path $OutputRoot "$packageName.zip"

foreach ($path in @($packageRoot, $webPublish, $proxyPublish, $deploymentPublish)) {
	if (Test-Path -LiteralPath $path) {
		Remove-Item -LiteralPath $path -Recurse -Force
	}
}

New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null

Invoke-DotNet @("restore", "MudClientSolution.sln", "-p:NuGetAudit=false")
Invoke-DotNet @("restore", "MudWebSocketProxy/MudWebSocketProxy.csproj", "-r", $RuntimeIdentifier, "-p:NuGetAudit=false")
Invoke-DotNet @("restore", "MudClientDeployment/MudClientDeployment.csproj", "-r", $RuntimeIdentifier, "-p:NuGetAudit=false")
if (-not $SkipTests) {
	Invoke-DotNet @("test", "MudClientSolution.sln", "-c", $Configuration, "--no-restore", "-p:NuGetAudit=false")
}

Invoke-DotNet @(
	"publish", "MudClientBlazor/MudClientBlazor.csproj", "-c", $Configuration, "--no-restore",
	"-o", $webPublish, "-p:Version=$Version", "-p:ContinuousIntegrationBuild=true")
Invoke-DotNet @(
	"publish", "MudWebSocketProxy/MudWebSocketProxy.csproj", "-c", $Configuration, "--no-restore",
	"-r", $RuntimeIdentifier, "--self-contained", "true", "-p:PublishSingleFile=true",
	"-p:IncludeNativeLibrariesForSelfExtract=true", "-p:DebugType=embedded", "-p:Version=$Version",
	"-p:ContinuousIntegrationBuild=true", "-o", $proxyPublish)
Invoke-DotNet @(
	"publish", "MudClientDeployment/MudClientDeployment.csproj", "-c", $Configuration, "--no-restore",
	"-r", $RuntimeIdentifier, "--self-contained", "true", "-p:PublishSingleFile=true",
	"-p:IncludeNativeLibrariesForSelfExtract=true", "-p:DebugType=embedded", "-p:Version=$Version",
	"-p:ContinuousIntegrationBuild=true", "-o", $deploymentPublish)

New-Item -ItemType Directory -Force -Path (Join-Path $packageRoot "web"), (Join-Path $packageRoot "proxy"), (Join-Path $packageRoot "tools") | Out-Null
Copy-Item -Path (Join-Path $webPublish "*") -Destination (Join-Path $packageRoot "web") -Recurse -Force
Copy-Item -Path (Join-Path $proxyPublish "*") -Destination (Join-Path $packageRoot "proxy") -Recurse -Force
Copy-Item -Path (Join-Path $deploymentPublish "*") -Destination (Join-Path $packageRoot "tools") -Recurse -Force
Copy-Item -Path "deploy" -Destination $packageRoot -Recurse -Force
Copy-Item -Path "DEPLOYMENT.md" -Destination $packageRoot -Force
Set-Content -LiteralPath (Join-Path $packageRoot "README.txt") -Value "Start with DEPLOYMENT.md. Linux users can run deploy/linux/install-mudclient.sh after extracting the package."

if (Test-Path -LiteralPath $zipPath) {
	Remove-Item -LiteralPath $zipPath -Force
}

Compress-Archive -Path $packageRoot -DestinationPath $zipPath -Force
Write-Host "Release package created:"
Write-Host "  $zipPath"
