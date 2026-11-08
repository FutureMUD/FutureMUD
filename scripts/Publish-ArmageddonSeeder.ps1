[CmdletBinding()]
param(
	[ValidateSet('win-x64', 'linux-x64', 'linux-arm64')]
	[string]$Runtime = 'win-x64',
	[string]$OutputDirectory = '.artifacts/armageddon-packages'
)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$project = Join-Path $repoRoot 'ArmageddonSeeder/ArmageddonSeeder.csproj'
[xml]$projectXml = Get-Content -LiteralPath $project -Raw
$version = [string]$projectXml.Project.PropertyGroup.Version | Where-Object { $_ }
if ($version -notmatch '^\d+\.\d+\.\d+$') { throw 'Armageddon Seeder requires an explicit three-part version.' }
$destination = [System.IO.Path]::GetFullPath($OutputDirectory, $repoRoot)
New-Item -ItemType Directory -Force -Path $destination | Out-Null
$archive = Join-Path $destination "ArmageddonSeeder-$version-$Runtime.zip"
if (Test-Path -LiteralPath $archive) { throw "Archive already exists: $archive" }
# A unique staging directory avoids deleting or overwriting previous packages.
$staging = Join-Path $destination ("staging-" + [guid]::NewGuid().ToString('N'))
& dotnet publish $project -c Release -r $Runtime --self-contained false -m:1 -o $staging `
	-p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false `
	-p:PublishSelfContained=false -p:RestoreBuildInParallel=false -p:BuildInParallel=false
if ($LASTEXITCODE -ne 0) { throw "Armageddon Seeder publish failed with exit code $LASTEXITCODE." }
$forbidden = Get-ChildItem -LiteralPath $staging -Recurse -File | Where-Object {
	$_.Name -in @('ArmageddonWorldfiles.json.gz', 'ArmageddonWorldfiles.manifest.json', 'ArmageddonWorldfileSpecialText.json')
}
if ($forbidden) { throw 'Original worldfile packet must not be included in the executable archive.' }
Compress-Archive -LiteralPath (Get-ChildItem -LiteralPath $staging).FullName -DestinationPath $archive
$hash = (Get-FileHash -LiteralPath $archive -Algorithm SHA256).Hash.ToLowerInvariant()
[System.IO.File]::WriteAllText("$archive.sha256", "$hash  $([System.IO.Path]::GetFileName($archive))`n")
Write-Output "Created $archive"
Write-Output "SHA-256: $hash"
# No website promotion, public release upload, announcement or private-packet copying.
