param(
	[ValidateRange(3, 20)][int]$Iterations = 5,
	[string]$OutputDirectory = (Join-Path '.artifacts' ('compiled-stock-' + [guid]::NewGuid().ToString('N'))),
	[string]$Runtime = [System.Runtime.InteropServices.RuntimeInformation]::RuntimeIdentifier,
	[switch]$UseExistingDependencies
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$outputRoot = [IO.Path]::GetFullPath($OutputDirectory)
if ((Test-Path -LiteralPath $outputRoot) -and (Get-ChildItem -LiteralPath $outputRoot -Force | Select-Object -First 1)) {
	throw 'Use a new or empty evidence directory.'
}
if ($Runtime -ne [System.Runtime.InteropServices.RuntimeInformation]::RuntimeIdentifier) {
	throw 'This measurement executes published binaries and requires the local host runtime identifier.'
}
[IO.Directory]::CreateDirectory($outputRoot) | Out-Null
$project = Join-Path $repoRoot 'DatabaseSeeder/DatabaseSeeder.csproj'
$oldCache = $env:NUGET_HTTP_CACHE_PATH
$oldExtractionRoot = $env:DOTNET_BUNDLE_EXTRACT_BASE_DIR
$env:NUGET_HTTP_CACHE_PATH = Join-Path $outputRoot 'nuget-http-cache'
$runRoot = Join-Path ([IO.Path]::GetTempPath()) ('FutureMudCompiledStock-' + [guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($runRoot) | Out-Null
# The Windows sandbox's virtualised temp cache cannot commit this application's native files.
# Use a short, owned cache. Its files are extracted from the executable, never supplied dependencies.
$env:DOTNET_BUNDLE_EXTRACT_BASE_DIR = Join-Path $repoRoot ('.artifacts/stock-cache-' + [guid]::NewGuid().ToString('N').Substring(0, 8))
[IO.Directory]::CreateDirectory($env:DOTNET_BUNDLE_EXTRACT_BASE_DIR) | Out-Null
$artifacts = @()
$samples = @()
try {
	foreach ($native in @($false, $true)) {
		$mode = if ($native) { 'r2r' } else { 'il' }
		$publish = Join-Path $outputRoot $mode
		$timer = [Diagnostics.Stopwatch]::StartNew()
		[string[]]$dependencyArguments = if ($UseExistingDependencies) { @('-p:BuildProjectReferences=false') } else { @() }
		& dotnet publish $project -c Release -r $Runtime --self-contained false -m:1 -p:RestoreBuildInParallel=false -p:BuildInParallel=false -p:UseSharedCompilation=false -p:NuGetAudit=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false -p:DebugType=embedded -p:PublishSelfContained=false "-p:PublishReadyToRun=$($native.ToString().ToLowerInvariant())" @dependencyArguments -o $publish *> (Join-Path $outputRoot "$mode-publish.log")
		$timer.Stop()
		if ($LASTEXITCODE) { throw "Publish failed: $mode" }
		$publishedEntry = Join-Path $publish $(if ($Runtime.StartsWith('win-')) { 'DatabaseSeeder.exe' } else { 'DatabaseSeeder' })
		$runDirectory = Join-Path $runRoot $mode
		[IO.Directory]::CreateDirectory($runDirectory) | Out-Null
		$entry = Join-Path $runDirectory (Split-Path -Leaf $publishedEntry)
		Copy-Item -LiteralPath $publishedEntry -Destination $entry
		if (!$Runtime.StartsWith('win-')) { & chmod u+x $entry; if ($LASTEXITCODE) { throw 'chmod failed' } }
		$artifacts += [pscustomobject]@{ Mode=$mode; Entry=$entry; Directory=$runDirectory; Bytes=(Get-Item -LiteralPath $entry).Length; Sha256=(Get-FileHash -LiteralPath $entry -Algorithm SHA256).Hash; PublishSeconds=$timer.Elapsed.TotalSeconds; PublishFiles=@(Get-ChildItem -LiteralPath $publish -File | Select-Object -ExpandProperty Name) }
	}
	for ($iteration = 0; $iteration -lt $Iterations; $iteration++) {
		$ordered = if ($iteration % 2 -eq 0) { $artifacts } else { @($artifacts[1], $artifacts[0]) }
		foreach ($artifact in $ordered) {
			Push-Location $artifact.Directory
			try {
				$arguments = @('--benchmark-compiled-item-stock')
				$timer = [Diagnostics.Stopwatch]::StartNew()
				$raw = & $artifact.Entry @arguments
				$timer.Stop()
				if ($LASTEXITCODE) { throw 'Published stock measurement failed' }
				$row = ($raw -join "`n") | ConvertFrom-Json
				if ($row.Items -ne 6450 -or $row.Medical -ne 178 -or $row.Prices -ne 17) { throw 'Incomplete stock measurement' }
				$samples += [pscustomobject]@{ Iteration=$iteration; Publication=$artifact.Mode; Mode=$row.Mode; ConstructionMilliseconds=$row.ConstructionMilliseconds; AllocatedBytes=$row.AllocatedBytes; ProcessSeconds=$timer.Elapsed.TotalSeconds; Digest=$row.Digest }
				$samples[-1] | ConvertTo-Json -Compress | Add-Content -LiteralPath (Join-Path $outputRoot 'samples.ndjson')
			} finally { Pop-Location }
		}
	}
	if (@($samples.Digest | Select-Object -Unique).Count -ne 1) { throw 'Published formats produced different values' }
	foreach ($artifact in $artifacts) {
		if ((Get-FileHash -LiteralPath $artifact.Entry -Algorithm SHA256).Hash -ne $artifact.Sha256) { throw 'Published binary changed during measurement' }
		if (@(Get-ChildItem -LiteralPath $artifact.Directory -Force).Count -ne 1) { throw 'Executable-only run directory gained a dependency' }
	}
	$inputReceipt = Get-ChildItem (Join-Path $repoRoot 'DatabaseSeeder/obj/Release') -Recurse -File -Filter 'CatalogueReadyToRunInputs.txt' | Where-Object { $_.FullName.Contains($Runtime) } | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
	if (!$inputReceipt) { throw 'Missing SDK-selected assembly receipt' }
	$headers = foreach ($path in Get-Content -LiteralPath $inputReceipt.FullName) {
		$stream = [IO.File]::OpenRead($path)
		$reader = [System.Reflection.PortableExecutable.PEReader]::new($stream)
		try { [pscustomobject]@{ Path=$path; NativeHeaderSize=$reader.PEHeaders.CorHeader.ManagedNativeHeaderDirectory.Size; Bytes=(Get-Item -LiteralPath $path).Length; Sha256=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash } }
		finally { $reader.Dispose(); $stream.Dispose() }
	}
	foreach ($name in @('Contracts', 'Medical', 'Industrial01', 'Industrial02', 'Industrial03', 'Industrial04')) {
		if (!@($headers | Where-Object { $_.NativeHeaderSize -gt 0 -and (Split-Path -Leaf $_.Path) -eq "DatabaseSeeder.Catalogue.$name.dll" -and $_.Path.Contains($Runtime) }).Count) {
			throw "Missing native header for $Runtime catalogue assembly: $name"
		}
	}
	foreach ($name in @('DatabaseSeeder.dll', 'MudSharp.dll', 'FutureMUDLibrary.dll', 'MudsharpDatabaseLibrary.dll')) {
		$header = @($headers | Where-Object { (Split-Path -Leaf $_.Path) -eq $name })
		if ($header.Count -ne 1 -or $header[0].NativeHeaderSize -ne 0) { throw "Unexpected precompilation of existing assembly: $name" }
	}
	[pscustomobject]@{ Runtime=$Runtime; SDK=(& dotnet --version); Iterations=$Iterations; UseExistingDependencies=[bool]$UseExistingDependencies; ExtractionDirectory=$env:DOTNET_BUNDLE_EXTRACT_BASE_DIR; Artifacts=$artifacts; NativeHeaders=@($headers); Samples=$samples; Scope='Stock loading and legacy compatibility projections only; no database writes, graph installation or full seeder bootstrap. Process time includes digest serialization and may include first-run native-library extraction. Publish timings have sequential cache states.' } | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $outputRoot 'summary.json')
	Write-Output "Verified $($samples.Count) matching samples; evidence: $outputRoot"
} finally { $env:NUGET_HTTP_CACHE_PATH = $oldCache; $env:DOTNET_BUNDLE_EXTRACT_BASE_DIR = $oldExtractionRoot }
