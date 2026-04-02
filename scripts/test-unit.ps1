$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$localDotnetRoot = Join-Path $repoRoot '.dotnet-cli\.dotnet'

function Initialize-Dotnet {
	$localDotnet = Join-Path $localDotnetRoot 'dotnet.exe'
	if (Get-Command dotnet -ErrorAction SilentlyContinue) {
		return
	}

	if (Test-Path $localDotnet) {
		$env:DOTNET_ROOT = $localDotnetRoot
		$env:PATH = "$localDotnetRoot;$env:PATH"
		return
	}

	throw 'dotnet was not found. Run scripts/setup.ps1 to bootstrap a local SDK.'
}

Initialize-Dotnet
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_NOLOGO = '1'

Push-Location $repoRoot
try {
	dotnet build DatabaseSeeder\DatabaseSeeder.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510
	dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -p:NoWarn=NU1902%3BNU1510
	dotnet test 'ExpressionEngine Unit Tests\ExpressionEngine Unit Tests.csproj' -c Debug --no-restore -p:NoWarn=NU1902%3BNU1510
	dotnet test 'DatabaseSeeder Unit Tests\DatabaseSeeder Unit Tests.csproj' -c Debug --no-restore -p:NoWarn=NU1902%3BNU1510
	dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510
}
finally {
	Pop-Location
}
