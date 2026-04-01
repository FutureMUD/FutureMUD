$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$bootstrapRoot = Join-Path $repoRoot '.dotnet-cli'
$installRoot = Join-Path $bootstrapRoot '.dotnet'
$installScript = Join-Path $bootstrapRoot 'dotnet-install.ps1'

New-Item -ItemType Directory -Path $bootstrapRoot -Force | Out-Null
Invoke-WebRequest -UseBasicParsing -Uri 'https://dot.net/v1/dotnet-install.ps1' -OutFile $installScript
& powershell.exe -ExecutionPolicy Bypass -File $installScript -Channel 10.0 -InstallDir $installRoot
& (Join-Path $installRoot 'dotnet.exe') --info
