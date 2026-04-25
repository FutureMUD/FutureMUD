param(
	[string]$Project = 'MudSharp.sln',
	[string]$Configuration = 'Debug',
	[switch]$Restore,
	[switch]$NoRebuild,
	[string]$Code,
	[int]$SampleCount = 80
)

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

function ConvertTo-RelativePath {
	param([string]$Path)

	if ([string]::IsNullOrWhiteSpace($Path)) {
		return $Path
	}

	$prefix = $repoRoot.TrimEnd('\') + '\'
	if ($Path.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase)) {
		return $Path.Substring($prefix.Length)
	}

	return $Path
}

Initialize-Dotnet
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_NOLOGO = '1'

$buildArguments = @('build', $Project, '-c', $Configuration, '-m:1', '-clp:NoSummary',
	'-p:RestoreBuildInParallel=false', '-p:NuGetAudit=false', '-p:NoWarn=NU1902%3BNU1510')

if (!$Restore) {
	$buildArguments += '--no-restore'
}

if (!$NoRebuild) {
	$buildArguments += '-t:Rebuild'
}

Push-Location $repoRoot
try {
	$output = & dotnet @buildArguments 2>&1
	$exitCode = $LASTEXITCODE
}
finally {
	Pop-Location
}

$warnings = foreach ($line in $output) {
	$text = $line.ToString()
	if ($text -match '^(?<file>.*?\.cs)\((?<line>\d+),(?<column>\d+)\): warning (?<code>(CS|NU|MSB|CA|SYSLIB|NETSDK)\d+): (?<message>.*?) \[(?<project>[^\]]+\.csproj)\]') {
		[pscustomobject]@{
			Key = "$($Matches.file):$($Matches.line):$($Matches.column):$($Matches.code)"
			File = ConvertTo-RelativePath $Matches.file
			Line = [int]$Matches.line
			Column = [int]$Matches.column
			Code = $Matches.code
			Message = $Matches.message
			Project = Split-Path $Matches.project -Leaf
		}
	}
	elseif ($text -match 'warning (?<code>(CS|NU|MSB|CA|SYSLIB|NETSDK)\d+)') {
		[pscustomobject]@{
			Key = $text
			File = ''
			Line = 0
			Column = 0
			Code = $Matches.code
			Message = $text
			Project = '(unparsed)'
		}
	}
}

$uniqueWarnings = $warnings | Sort-Object Key -Unique
if (![string]::IsNullOrWhiteSpace($Code)) {
	$uniqueWarnings = $uniqueWarnings | Where-Object { $_.Code -eq $Code }
}
$errors = $output | Where-Object { $_.ToString() -match 'error (CS|MSB|NETSDK|NU)\d+' }

Write-Host "Exit code: $exitCode"
Write-Host "Raw warning lines: $($warnings.Count)"
if ([string]::IsNullOrWhiteSpace($Code)) {
	Write-Host "Unique warnings: $($uniqueWarnings.Count)"
}
else {
	Write-Host "Unique warnings for $Code`: $($uniqueWarnings.Count)"
}
Write-Host "Error lines: $($errors.Count)"

Write-Host ''
Write-Host 'Warnings by code'
$uniqueWarnings |
	Group-Object Code |
	Sort-Object Count -Descending |
	ForEach-Object { Write-Host ("{0}`t{1}" -f $_.Count, $_.Name) }

Write-Host ''
Write-Host 'Warnings by project'
$uniqueWarnings |
	Group-Object Project |
	Sort-Object Count -Descending |
	ForEach-Object { Write-Host ("{0}`t{1}" -f $_.Count, $_.Name) }

Write-Host ''
Write-Host 'Top warning files'
$uniqueWarnings |
	Where-Object File |
	Group-Object File |
	Sort-Object Count -Descending |
	Select-Object -First 40 |
	ForEach-Object { Write-Host ("{0}`t{1}" -f $_.Count, $_.Name) }

Write-Host ''
Write-Host 'Sample warnings'
$uniqueWarnings |
	Sort-Object Code, File, Line |
	Select-Object -First $SampleCount |
	ForEach-Object { Write-Host ("{0}:{1}:{2} {3} {4}" -f $_.File, $_.Line, $_.Column, $_.Code, $_.Message) }

if ($errors.Count -gt 0) {
	Write-Host ''
	Write-Host 'Errors'
	$errors | Select-Object -First 20 | ForEach-Object { Write-Host $_.ToString() }
}

exit $exitCode
