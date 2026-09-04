param(
	[switch]$Refresh,
	[switch]$Check,
	[switch]$CheckReview,
	[switch]$SelfTest
)

$ErrorActionPreference = 'Stop'
$selectedModeCount = @(@($Refresh.IsPresent, $Check.IsPresent, $CheckReview.IsPresent, $SelfTest.IsPresent) | Where-Object { $_ }).Count
if ($selectedModeCount -ne 1) {
	throw 'Choose exactly one of -Refresh, -Check, -CheckReview, or -SelfTest.'
}

$repoRoot = Split-Path $PSScriptRoot -Parent
$project = Join-Path $repoRoot 'DatabaseSeeder/DatabaseSeeder.csproj'

if ($SelfTest) {
	& dotnet test (Join-Path $repoRoot 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj') -c Debug --no-restore -m:1 --filter 'FullyQualifiedName~IndustrialisedFood'
	exit $LASTEXITCODE
}

$command = if ($Refresh) { '--export-industrialised-catalogue' } else { '--check-industrialised-catalogue' }
& dotnet run --project $project -c Debug --no-build -- $command
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

if ($CheckReview) {
	$design = Get-Content -Raw (Join-Path $repoRoot 'Design Documents/Seeding/FutureMUD_Industrialised_Food_Drink_Design_Reference.md')
	if ($design -notmatch 'Gate 2.*awaiting editorial acceptance') {
		throw 'The food design reference must state that Gate 2 is awaiting editorial acceptance.'
	}
}
