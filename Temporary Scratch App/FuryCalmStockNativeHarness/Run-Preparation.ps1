[CmdletBinding()]
param([ValidateSet('profile-preflight', 'paid-native')][string]$Mode = 'profile-preflight')

$ErrorActionPreference = 'Stop'
$workspaceRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$harnessDll = Join-Path $PSScriptRoot 'bin\Debug\net10.0\FuryCalmStockNativeHarness.dll'
if (-not (Test-Path -LiteralPath $harnessDll)) { throw 'Build this dedicated harness before running preparation.' }
$runRoot = Join-Path $workspaceRoot ('.artifacts\test-runs\fury-calm-stock-preparation_' + [guid]::NewGuid().ToString('N'))
$resolvedRun = [IO.Path]::GetFullPath($runRoot)
$ownedPrefix = [IO.Path]::GetFullPath((Join-Path $workspaceRoot '.artifacts\test-runs\fury-calm-stock-preparation_'))
if (-not $resolvedRun.StartsWith($ownedPrefix, [StringComparison]::OrdinalIgnoreCase)) { throw 'Unsafe lane output path.' }
New-Item -ItemType Directory -Path $resolvedRun | Out-Null

function Get-SourceManifest {
	$paths = @(& git -C $workspaceRoot ls-files --cached --others --exclude-standard)
	if ($LASTEXITCODE -ne 0) { throw 'Unable to enumerate complete source inputs.' }
	$inputs = $paths | Where-Object {
		($_ -match '^(MudSharpCore/|FutureMUDLibrary/|ExpressionEngine/|MudsharpDatabaseLibrary/|FutureMUD Analyzers/|Temporary Scratch App/FuryCalmStockNativeHarness/)' -and
		$_ -match '\.(cs|csproj|props|targets|json|xml|ps1)$') -or
		$_ -match '^(Directory\.Build\.(props|targets)|global\.json|NuGet\.Config)$'
	} | Sort-Object -Unique
	@($inputs | ForEach-Object { [ordered]@{ path = $_; sha256 = (Get-FileHash -LiteralPath (Join-Path $workspaceRoot $_) -Algorithm SHA256).Hash.ToLowerInvariant() } })
}

$sourceBefore = Get-SourceManifest | ConvertTo-Json -Depth 8
$sourceBefore | Set-Content -LiteralPath (Join-Path $resolvedRun 'source-start.json') -Encoding utf8
$assemblyBefore = @(Get-ChildItem -LiteralPath (Split-Path $harnessDll) -Filter '*.dll' | Sort-Object Name | ForEach-Object {
	[ordered]@{ name = $_.Name; sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
}) | ConvertTo-Json -Depth 8
$assemblyBefore | Set-Content -LiteralPath (Join-Path $resolvedRun 'assembly-start.json') -Encoding utf8
& dotnet $harnessDll $Mode (Join-Path $resolvedRun 'entry.json') *> (Join-Path $resolvedRun 'entry.log')
$entryExit = $LASTEXITCODE
$sourceAfter = Get-SourceManifest | ConvertTo-Json -Depth 8
$sourceAfter | Set-Content -LiteralPath (Join-Path $resolvedRun 'source-end.json') -Encoding utf8
$assemblyAfter = @(Get-ChildItem -LiteralPath (Split-Path $harnessDll) -Filter '*.dll' | Sort-Object Name | ForEach-Object {
	[ordered]@{ name = $_.Name; sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
}) | ConvertTo-Json -Depth 8
$assemblyAfter | Set-Content -LiteralPath (Join-Path $resolvedRun 'assembly-end.json') -Encoding utf8
$entry = Get-Content -LiteralPath (Join-Path $resolvedRun 'entry.json') -Raw | ConvertFrom-Json
$sourceStable = $sourceBefore -ceq $sourceAfter
$assembliesStable = $assemblyBefore -ceq $assemblyAfter
$revision = (& git -C $workspaceRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Unable to read source revision.' }
[ordered]@{ mode = $Mode; commit = $revision; entry_exit = $entryExit; status = $entry.Status;
	source_stable = $sourceStable; assemblies_stable = $assembliesStable;
	runtime_qualified = $false; database_started = $false; owned_database_process_cleanup_required = $false
} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $resolvedRun 'summary.json') -Encoding utf8
Write-Output "Fury/Calm preparation: status=$($entry.Status); sourceStable=$sourceStable; assembliesStable=$assembliesStable; runtimeQualified=false; evidence=$resolvedRun"
if (-not $sourceStable -or -not $assembliesStable) { exit 3 }
exit $entryExit
