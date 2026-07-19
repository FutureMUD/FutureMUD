$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$localDotnetRoot = Join-Path $repoRoot '.dotnet-cli\.dotnet'
$testProjectManifest = Join-Path $PSScriptRoot 'unit-test-projects.txt'

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

function Invoke-Dotnet {
	param([Parameter(Mandatory)] [scriptblock]$Command)

	& $Command
	if ($LASTEXITCODE -ne 0) {
		throw "dotnet command failed with exit code $LASTEXITCODE."
	}
}

Initialize-Dotnet
$dotnetCommand = (Get-Command dotnet).Source
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_NOLOGO = '1'

Push-Location $repoRoot
try {
	$testProjects = Get-Content -LiteralPath $testProjectManifest |
		Where-Object { -not [string]::IsNullOrWhiteSpace($_) -and -not $_.TrimStart().StartsWith('#') }

	# Build sequentially to avoid shared-output locks, then run the isolated test hosts in parallel.
	# Each targeted build restores its own graph, avoiding unrelated platform-specific solution projects.
	foreach ($testProject in $testProjects) {
		Invoke-Dotnet { & $dotnetCommand build $testProject -c Debug -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false -p:NoWarn=NU1902%3BNU1510 }
	}

	$jobs = foreach ($testProject in $testProjects) {
		Start-Job -ArgumentList $repoRoot, $dotnetCommand, $testProject -ScriptBlock {
			param($jobRepoRoot, $jobDotnetCommand, $jobTestProject)
			Set-Location $jobRepoRoot
			$commandOutput = @(
				& $jobDotnetCommand test $jobTestProject -c Debug --no-restore --no-build -m:1 -p:NoWarn=NU1902%3BNU1510 2>&1 |
					ForEach-Object { $_.ToString() }
			)
			[pscustomobject]@{
				Project = $jobTestProject
				ExitCode = $LASTEXITCODE
				Output = $commandOutput
			}
		}
	}

	try {
		$jobs | Wait-Job | Out-Null
		$failures = @()
		foreach ($job in $jobs) {
			$result = Receive-Job -Job $job
			$result.Output | ForEach-Object { Write-Host $_ }
			if ($result.ExitCode -ne 0) {
				$failures += "$($result.Project) (exit code $($result.ExitCode))"
			}
		}

		if ($failures.Count -gt 0) {
			throw "Unit-test project failures: $($failures -join '; ')"
		}
	}
	finally {
		$jobs | Remove-Job -Force -ErrorAction SilentlyContinue
	}
}
finally {
	Pop-Location
}