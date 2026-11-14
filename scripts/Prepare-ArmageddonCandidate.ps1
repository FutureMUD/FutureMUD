[CmdletBinding()]
param(
	[ValidatePattern('^\d+\.\d+\.\d+-rc\.[1-9]\d*$')]
	[string]$CandidateVersion = '1.0.0-rc.1',
	[ValidateSet('win-x64')]
	[string]$Runtime = 'win-x64',
	[string]$OutputDirectory = '.artifacts/armageddon-candidates'
)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
Push-Location $repoRoot
try {
	$revision = (& git rev-parse HEAD).Trim()
	if ($LASTEXITCODE -ne 0) { throw 'Cannot identify candidate source.' }
	if (@(& git status --porcelain).Count -ne 0) { throw 'Commit candidate inputs before packaging; the working tree must be clean.' }
	$destination = Join-Path ([IO.Path]::GetFullPath($OutputDirectory, $repoRoot)) "$CandidateVersion-$Runtime-$($revision.Substring(0, 12))"
	if (Test-Path -LiteralPath $destination) { throw "Candidate directory already exists: $destination" }
	New-Item -ItemType Directory -Path $destination | Out-Null
	$entries = @()
	foreach ($product in @(
		@{ Id = 'engine'; Project = 'MudSharpCore/MudSharpCore.csproj' },
		@{ Id = 'seeder'; Project = 'DatabaseSeeder/DatabaseSeeder.csproj' },
		@{ Id = 'ArmageddonSeeder'; Project = 'ArmageddonSeeder/ArmageddonSeeder.csproj' }
	)) {
		[xml]$project = Get-Content -LiteralPath $product.Project -Raw
		$version = [string]($project.Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1)
		if ($version -notmatch '^\d+\.\d+\.\d+$') { throw "Missing three-part version for $($product.Id)" }
		if ($product.Id -eq 'ArmageddonSeeder' -and $CandidateVersion.Split('-')[0] -ne $version) {
			throw 'Candidate version must match the Armageddon Seeder product version.'
		}
		$staging = Join-Path $destination "staging-$($product.Id)"
		& dotnet publish $product.Project -c Release -r $Runtime --self-contained false -m:1 -o $staging `
			-p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false `
			-p:PublishSelfContained=false -p:DebugType=embedded -p:RestoreBuildInParallel=false `
			-p:BuildInParallel=false -p:UseSharedCompilation=false
		if ($LASTEXITCODE -ne 0) { throw "$($product.Id) publish failed: $LASTEXITCODE" }
		$forbidden = Get-ChildItem -LiteralPath $staging -Recurse -File | Where-Object {
			$_.Name -in @('ArmageddonWorldfiles.json.gz', 'ArmageddonWorldfiles.manifest.json', 'ArmageddonWorldfileSpecialText.json', 'Connection.config', 'StartMUD.bat', 'StartMUD.sh')
		}
		if ($forbidden) { throw "Private data or instance configuration found in $($product.Id) staging." }
		$archiveName = "$($product.Id)-$version-armageddon-$CandidateVersion-$Runtime.zip"
		$archive = Join-Path $destination $archiveName
		Compress-Archive -LiteralPath (Get-ChildItem -LiteralPath $staging).FullName -DestinationPath $archive
		$hash = (Get-FileHash -LiteralPath $archive -Algorithm SHA256).Hash.ToLowerInvariant()
		[IO.File]::WriteAllText("$archive.sha256", "$hash  $archiveName`n")
		$entries += @{ product = $product.Id; version = $version; archive = $archiveName; sha256 = $hash }
	}
	$documentation = Join-Path $destination 'catalogue.json'
	& (Join-Path $destination 'staging-engine/MudSharp.exe') --export-documentation $documentation --source-revision $revision
	if ($LASTEXITCODE -ne 0) { throw "Packaged engine documentation export failed: $LASTEXITCODE" }
	$catalogue = Get-Content -LiteralPath $documentation -Raw | ConvertFrom-Json
	foreach ($family in @('commands', 'progFunctions', 'progTypes', 'collectionExtensions', 'itemComponents')) {
		if ($null -eq $catalogue.$family -or @($catalogue.$family).Count -eq 0) { throw "Documentation family is empty: $family" }
	}
	if ($catalogue.sourceRevision -ne $revision) { throw 'Documentation source identity differs from candidate.' }
	# The export is also a database-free launch check of the actual packaged engine.
	if ((& git rev-parse HEAD).Trim() -ne $revision -or @(& git status --porcelain).Count -ne 0) {
		throw 'Candidate source changed during packaging.'
	}
	$readmeSource = Join-Path $repoRoot 'Design Documents/Seeding/Armageddon_Release_Candidate_1.0.md'
	$readmeText = Get-Content -LiteralPath $readmeSource -Raw
	$readmeText = [regex]::Replace($readmeText, '\]\(([^)#]+\.md)(#[^)]*)?\)', {
		param($link)
		if ($link.Groups[1].Value -match '^https?://') { return $link.Value }
		$target = [IO.Path]::GetFullPath([Uri]::UnescapeDataString($link.Groups[1].Value), (Split-Path $readmeSource -Parent))
		if (-not $target.StartsWith($repoRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase) -or -not (Test-Path -LiteralPath $target)) {
			throw "Invalid candidate documentation link: $($link.Groups[1].Value)"
		}
		$relative = [IO.Path]::GetRelativePath($repoRoot, $target).Replace('\', '/')
		$encoded = (($relative -split '/') | ForEach-Object { [Uri]::EscapeDataString($_) }) -join '/'
		return "](https://github.com/FutureMUD/FutureMUD/blob/$revision/$encoded$($link.Groups[2].Value))"
	})
	[IO.File]::WriteAllText((Join-Path $destination 'README-UAT.md'), $readmeText)
	$manifest = [ordered]@{
		candidate = $CandidateVersion; sourceRevision = $revision; runtime = $Runtime
		configuration = 'Release'; framework = 'net11.0'; frameworkDependent = $true
		singleFile = $true; trimmed = $false; privatePacketIncluded = $false
		createdUtc = [DateTime]::UtcNow.ToString('O'); products = $entries
		documentation = @{ file = 'catalogue.json'; sha256 = (Get-FileHash -LiteralPath $documentation -Algorithm SHA256).Hash.ToLowerInvariant() }
	}
	$manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $destination 'candidate.json') -Encoding utf8NoBOM
	Write-Output "Prepared private candidate: $destination"
} finally { Pop-Location }
