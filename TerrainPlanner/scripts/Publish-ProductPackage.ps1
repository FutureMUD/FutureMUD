[CmdletBinding()]
param(
	[Parameter(Mandatory = $true)]
	[ValidateSet('win-x64', 'linux-x64', 'linux-arm64')]
	[string]$RuntimeIdentifier,
	[Parameter(Mandatory = $true)]
	[ValidatePattern('^\d+\.\d+\.\d+$')]
	[string]$Version,
	[string]$Configuration = 'Release',
	[string]$OutputRoot = 'artifacts\release',
	[switch]$SkipTests
)

$ErrorActionPreference = 'Stop'

function Invoke-DotNet {
	param([Parameter(Mandatory = $true)][string[]]$Arguments)
	& dotnet @Arguments
	if ($LASTEXITCODE -ne 0) { throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE." }
}

$OutputRoot = [IO.Path]::GetFullPath($OutputRoot)
$plannerRoot = Split-Path -Parent $PSScriptRoot
Set-Location $plannerRoot
$packageName = "terrainplanner-$Version-$RuntimeIdentifier"
$packageRoot = Join-Path $OutputRoot $packageName
$clientPublish = Join-Path $OutputRoot '_client'
$serverPublish = Join-Path $OutputRoot "_server-$RuntimeIdentifier"
$deploymentPublish = Join-Path $OutputRoot "_deployment-$RuntimeIdentifier"
$zipPath = Join-Path $OutputRoot "$packageName.zip"

foreach ($path in @($packageRoot, $clientPublish, $serverPublish, $deploymentPublish)) {
	if (Test-Path -LiteralPath $path) { Remove-Item -LiteralPath $path -Recurse -Force }
}
New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null

Invoke-DotNet @('restore', 'TerrainPlanner.slnx', '-m:1', '-p:RestoreBuildInParallel=false', '-p:NuGetAudit=false')
Invoke-DotNet @('restore', 'TerrainPlanner.Server/TerrainPlanner.Server.csproj', '-r', $RuntimeIdentifier, '-m:1', '-p:RestoreBuildInParallel=false', '-p:NuGetAudit=false')
Invoke-DotNet @('restore', 'TerrainPlanner.Deployment/TerrainPlanner.Deployment.csproj', '-r', $RuntimeIdentifier, '-m:1', '-p:RestoreBuildInParallel=false', '-p:NuGetAudit=false')
if (-not $SkipTests) {
	Invoke-DotNet @('test', 'TerrainPlanner.Tests/TerrainPlanner.Tests.csproj', '-c', $Configuration, '--no-restore', '-m:1', '-p:NuGetAudit=false')
}

Invoke-DotNet @(
	'publish', 'TerrainPlanner.Client/TerrainPlanner.Client.csproj', '-c', $Configuration, '--no-restore',
	"-p:Version=$Version", '-p:ContinuousIntegrationBuild=true', '-o', $clientPublish)
Invoke-DotNet @(
	'publish', 'TerrainPlanner.Server/TerrainPlanner.Server.csproj', '-c', $Configuration, '--no-restore',
	'-r', $RuntimeIdentifier, '--self-contained', 'true', '-p:PublishSingleFile=true',
	'-p:IncludeNativeLibrariesForSelfExtract=true', '-p:DebugType=embedded', "-p:Version=$Version",
	'-p:ContinuousIntegrationBuild=true', '-o', $serverPublish)
Invoke-DotNet @(
	'publish', 'TerrainPlanner.Deployment/TerrainPlanner.Deployment.csproj', '-c', $Configuration, '--no-restore',
	'-r', $RuntimeIdentifier, '--self-contained', 'true', '-p:PublishSingleFile=true',
	'-p:IncludeNativeLibrariesForSelfExtract=true', '-p:DebugType=embedded', "-p:Version=$Version",
	'-p:ContinuousIntegrationBuild=true', '-o', $deploymentPublish)

New-Item -ItemType Directory -Force -Path (Join-Path $serverPublish 'wwwroot') | Out-Null
Copy-Item -Path (Join-Path $clientPublish 'wwwroot\*') -Destination (Join-Path $serverPublish 'wwwroot') -Recurse -Force

New-Item -ItemType Directory -Force -Path (Join-Path $packageRoot 'app'), (Join-Path $packageRoot 'tools') | Out-Null
Copy-Item -Path (Join-Path $serverPublish '*') -Destination (Join-Path $packageRoot 'app') -Recurse -Force
Copy-Item -Path (Join-Path $deploymentPublish '*') -Destination (Join-Path $packageRoot 'tools') -Recurse -Force
Copy-Item -Path 'deploy' -Destination $packageRoot -Recurse -Force
Copy-Item -Path 'DEPLOYMENT.md' -Destination $packageRoot -Force
[IO.File]::WriteAllText((Join-Path $packageRoot 'version.txt'), $Version, [Text.UTF8Encoding]::new($false))
[IO.File]::WriteAllText((Join-Path $packageRoot 'README.txt'), 'Start with DEPLOYMENT.md. The planner and Engine API are installed and upgraded together.', [Text.UTF8Encoding]::new($false))

$requiredPaths = @(
	'app\wwwroot\index.html',
	'app\wwwroot\_framework',
	'deploy\appsettings.Production.template.json',
	'deploy\Caddyfile.fragment',
	'DEPLOYMENT.md',
	'version.txt'
)
if ($RuntimeIdentifier -eq 'win-x64') {
	$requiredPaths += @('app\TerrainPlanner.exe', 'tools\TerrainPlanner.Deployment.exe', 'deploy\windows\Install-TerrainPlanner.ps1')
} else {
	$requiredPaths += @('app\TerrainPlanner', 'tools\TerrainPlanner.Deployment', 'deploy\linux\install-terrainplanner.sh')
}
foreach ($relativePath in $requiredPaths) {
	if (-not (Test-Path -LiteralPath (Join-Path $packageRoot $relativePath))) {
		throw "Release package is missing required path '$relativePath'."
	}
}

if (Test-Path -LiteralPath $zipPath) { Remove-Item -LiteralPath $zipPath -Force }
Compress-Archive -Path $packageRoot -DestinationPath $zipPath -Force
Write-Host "Release package created: $zipPath"
