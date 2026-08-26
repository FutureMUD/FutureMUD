[CmdletBinding()]
param(
	[Parameter(Mandatory = $true)][ValidateSet('win-x64')][string]$RuntimeIdentifier,
	[Parameter(Mandatory = $true)][string]$Hostname,
	[string]$InstallRoot = "$env:ProgramFiles\FutureMUD\TerrainPlanner",
	[string]$CaddyExecutable,
	[string]$Caddyfile
)

$ErrorActionPreference = 'Stop'
$work = Join-Path ([IO.Path]::GetTempPath()) ("terrainplanner-update-" + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $work | Out-Null
try {
	$base = 'https://futuremud.com/downloads/terrainplanner/latest'
	Invoke-WebRequest "$base/update-manifest.json" -OutFile (Join-Path $work 'update-manifest.json')
	Invoke-WebRequest "$base/update-manifest.sig" -OutFile (Join-Path $work 'update-manifest.sig')
	$manifest = Get-Content (Join-Path $work 'update-manifest.json') -Raw | ConvertFrom-Json
	$archiveName = "terrainplanner-$($manifest.version)-$RuntimeIdentifier.zip"
	$archive = Join-Path $work $archiveName
	Invoke-WebRequest "$base/$archiveName" -OutFile $archive
	$verifier = Join-Path $InstallRoot 'current\tools\TerrainPlanner.Deployment.exe'
	& $verifier verify --manifest (Join-Path $work 'update-manifest.json') --signature (Join-Path $work 'update-manifest.sig') --archive $archive --runtime $RuntimeIdentifier
	if ($LASTEXITCODE -ne 0) { throw 'Signed update verification failed.' }
	Expand-Archive -LiteralPath $archive -DestinationPath (Join-Path $work 'package')
	& (Join-Path $work "package\terrainplanner-$($manifest.version)-$RuntimeIdentifier\deploy\windows\Install-TerrainPlanner.ps1") -Hostname $Hostname -InstallRoot $InstallRoot -CaddyExecutable $CaddyExecutable -Caddyfile $Caddyfile
} finally {
	if (Test-Path -LiteralPath $work) { Remove-Item -LiteralPath $work -Recurse -Force }
}
