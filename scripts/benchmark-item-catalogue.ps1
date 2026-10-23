param(
	[ValidateRange(3, 20)][int]$Iterations = 5,
	[string]$OutputDirectory,
	[string]$Eras = 'antiquity medieval renaissance earlymodern industrial',
	[string]$LegacyBinary
)
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path -Parent $PSScriptRoot
if (-not $OutputDirectory) { $OutputDirectory = Join-Path $taskRoot ('.artifacts/item-catalogue-benchmarks/' + [guid]::NewGuid().ToString('N')) }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
$taskDll = Join-Path $taskRoot 'DatabaseSeeder/bin/Release/net10.0/DatabaseSeeder.dll'
if (-not (Test-Path -LiteralPath $taskDll)) { throw 'Build DatabaseSeeder in Release before benchmarking.' }
$binaryHash = (Get-FileHash -LiteralPath $taskDll -Algorithm SHA256).Hash
if ($LegacyBinary) {
	$LegacyBinary = (Resolve-Path -LiteralPath $LegacyBinary).Path
	if ($LegacyBinary -eq $taskDll) { throw 'LegacyBinary must be a separately frozen legacy executable.' }
	$legacyHash = (Get-FileHash -LiteralPath $LegacyBinary -Algorithm SHA256).Hash
}
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$samples = @()
Push-Location $taskRoot
try {
	for ($iteration = 0; $iteration -lt $Iterations; $iteration++) {
		$modes = if (-not $LegacyBinary) { @('unified') } elseif ($iteration % 2 -eq 0) { @('legacy', 'unified') } else { @('unified', 'legacy') }
		foreach ($mode in $modes) {
			$destination = Join-Path $OutputDirectory "$iteration-$mode"
			$binary = if ($mode -eq 'legacy') { $LegacyBinary } else { $taskDll }
			$arguments = @($binary, '--benchmark-item-catalogue', '--output', $destination, '--eras', $Eras)
			if ($mode -eq 'legacy') { $arguments += '--legacy' }
			& dotnet @arguments
			if ($LASTEXITCODE -ne 0) { throw "Benchmark failed: iteration $iteration, $mode" }
			$samples += Get-Content -Raw -LiteralPath (Join-Path $destination 'benchmark.json') | ConvertFrom-Json
		}
	}
	if ((Get-FileHash -LiteralPath $taskDll -Algorithm SHA256).Hash -ne $binaryHash) { throw 'Benchmark binary changed during execution.' }
	if ($LegacyBinary -and (Get-FileHash -LiteralPath $LegacyBinary -Algorithm SHA256).Hash -ne $legacyHash) { throw 'Legacy benchmark binary changed during execution.' }
	$definitionsMatch = @($samples.definitionDigest | Select-Object -Unique).Count -eq 1
	function Median($values) { $sorted = @($values | Sort-Object); if ($sorted.Count % 2) { $sorted[[int]($sorted.Count / 2)] } else { ($sorted[$sorted.Count / 2 - 1] + $sorted[$sorted.Count / 2]) / 2 } }
	$groups = foreach ($unified in @($false, $true)) {
		$group = @($samples | Where-Object unified -EQ $unified)
		if (-not $group.Count) { continue }
		[pscustomobject]@{
			Unified = $unified; Samples = $group.Count
			MedianMilliseconds = Median $group.elapsedMilliseconds
			MinimumMilliseconds = ($group.elapsedMilliseconds | Measure-Object -Minimum).Minimum
			MaximumMilliseconds = ($group.elapsedMilliseconds | Measure-Object -Maximum).Maximum
			MedianAllocatedBytes = Median $group.allocatedBytes
			MedianRepresentativeConstructionMilliseconds = Median $group.representativeConstruction.milliseconds
			MedianRepresentativeConstructionAllocatedBytes = Median $group.representativeConstruction.allocatedBytes
		}
	}
	[pscustomobject]@{
		BinarySha256 = $binaryHash; LegacyBinarySha256 = $legacyHash; DotnetSdk = (& dotnet --version); OS = [Environment]::OSVersion.ToString()
		SourceRevision = (& git rev-parse HEAD); Eras = $Eras; DefinitionDigest = $samples[0].definitionDigest
		DefinitionsMatch = $definitionsMatch
		Scope = 'Cold in-memory authoring workload; no MySQL writes. Different definition digests are not an equivalent-workload speed comparison.'
		ConstructionScope = '42,756 repetitions of one representative payload; allocation/parser comparison only.'
		Results = @($groups)
	} | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $OutputDirectory 'summary.json')
	Get-Content -LiteralPath (Join-Path $OutputDirectory 'summary.json')
} finally { Pop-Location }
