param(
	[Parameter(Mandatory = $true)][string]$ArtifactDirectory,
	[Parameter(Mandatory = $true)][string]$Version,
	[Parameter(Mandatory = $true)][string]$SourceCommit,
	[Parameter(Mandatory = $true)][string]$OutputPath,
	[string]$KeyId = 'futuremud-mudclient-2026-08'
)

$ErrorActionPreference = 'Stop'
$artifacts = @()
foreach ($file in Get-ChildItem -LiteralPath $ArtifactDirectory -Filter "terrainplanner-$Version-*.zip" -File | Sort-Object Name) {
	if ($file.Name -notmatch "^terrainplanner-$([regex]::Escape($Version))-(win-x64|linux-x64|linux-arm64)\.zip$") {
		throw "Unexpected Terrain Planner archive '$($file.Name)'."
	}
	$artifacts += [ordered]@{
		runtime = $Matches[1]
		fileName = $file.Name
		size = $file.Length
		sha256 = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
	}
}
if ($artifacts.Count -ne 3) { throw 'The signed Terrain Planner manifest requires all three runtime archives.' }
[IO.File]::WriteAllText($OutputPath, ([ordered]@{
	schemaVersion = 1
	product = 'terrainplanner'
	version = $Version
	sourceCommit = $SourceCommit
	keyId = $KeyId
	artifacts = $artifacts
} | ConvertTo-Json -Depth 5 -Compress), [Text.UTF8Encoding]::new($false))
