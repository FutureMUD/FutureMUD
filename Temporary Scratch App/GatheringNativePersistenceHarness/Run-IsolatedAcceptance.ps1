[CmdletBinding()]
param([switch]$LandOnly)

$ErrorActionPreference = 'Stop'

$workspaceRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$harnessDll = Join-Path $PSScriptRoot 'bin\Debug\net10.0\GatheringNativePersistenceHarness.dll'
$mysqld = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqld.exe'
$mysql = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe'
$mysqladmin = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqladmin.exe'
$taskRoot = Join-Path $env:TEMP ('futuremud-gather-mysql_' + [guid]::NewGuid().ToString('N'))
$taskData = Join-Path $taskRoot 'data'
$taskLog = Join-Path $taskRoot 'mysql.err'
$taskPidFile = Join-Path $taskRoot 'mysql.pid'
$temporaryBase = ([IO.Path]::GetFullPath($env:TEMP)).TrimEnd('\') + '\futuremud-gather-mysql_'
$serverLaunched = $false
$reachable = $false
$cleanupSucceeded = $false
$runExit = 1

function Require-TemporaryRoot {
	param([string]$Path)

	$resolved = [IO.Path]::GetFullPath($Path)
	if (-not $resolved.StartsWith($temporaryBase, [StringComparison]::OrdinalIgnoreCase)) {
		throw 'Refusing an unsafe temporary MySQL target.'
	}
}

function Stop-OwnedMySql {
	param([int]$Port)

	$actualData = (& $mysql '--no-defaults' '--protocol=tcp' '--host=127.0.0.1' "--port=$Port" '--user=root' '--skip-password' '--batch' '--skip-column-names' '--execute=SELECT @@datadir').Trim()
	if ($LASTEXITCODE -ne 0) {
		throw 'Unable to verify the temporary MySQL instance for cleanup.'
	}

	$normalActual = $actualData.Replace('/', '\').Replace('\\', '\').TrimEnd('\')
	$normalExpected = ([IO.Path]::GetFullPath($taskData)).TrimEnd('\')
	if (-not $normalActual.Equals($normalExpected, [StringComparison]::OrdinalIgnoreCase)) {
		throw 'The MySQL data directory did not match the owned temporary instance; refusing shutdown.'
	}

	& $mysqladmin '--no-defaults' '--protocol=tcp' '--host=127.0.0.1' "--port=$Port" '--user=root' '--skip-password' '--connect-timeout=5' shutdown
	if ($LASTEXITCODE -ne 0) {
		throw 'The verified temporary MySQL instance did not accept graceful shutdown.'
	}

	for ($attempt = 0; $attempt -lt 40; $attempt++) {
		if ((Get-Content -LiteralPath $taskLog -Tail 20) -match 'Shutdown complete') {
			return
		}

		Start-Sleep -Milliseconds 250
	}

	throw 'The verified temporary MySQL instance did not finish graceful shutdown.'
}

Require-TemporaryRoot $taskRoot
$listener = [Net.Sockets.TcpListener]::new([Net.IPAddress]::Loopback, 0)
$listener.Start()
$taskPort = ($listener.LocalEndpoint).Port
$listener.Stop()
New-Item -ItemType Directory -Path $taskRoot | Out-Null

try {
	$revision = (& git -C $workspaceRoot rev-parse HEAD).Trim()
	if ($LASTEXITCODE -ne 0) {
		throw 'Unable to identify the checked-out revision.'
	}
	$workingTreeStatus = @(& git -C $workspaceRoot status --porcelain=v1 --untracked-files=all)
	if ($LASTEXITCODE -ne 0) {
		throw 'Unable to identify the working-tree state.'
	}
	$workingTreeState = if ($workingTreeStatus.Count -eq 0) { 'clean' } else { 'dirty' }

	Write-Output "workingTreeRevision=$revision"
	Write-Output "workingTreeState=$workingTreeState"
	Write-Output 'server=isolated-loopback-mysql'
	& $mysqld '--no-defaults' '--initialize-insecure' "--basedir=C:\Program Files\MySQL\MySQL Server 8.0" "--datadir=$taskData" "--log-error=$taskLog"
	if ($LASTEXITCODE -ne 0) {
		throw 'The owned MySQL instance could not be initialized.'
	}

	$serverArguments = "--no-defaults --basedir=`"C:\Program Files\MySQL\MySQL Server 8.0`" --datadir=`"$taskData`" --port=$taskPort --bind-address=127.0.0.1 --pid-file=`"$taskPidFile`" --log-error=`"$taskLog`" --character-set-server=utf8mb4 --collation-server=utf8mb4_general_ci --mysqlx=0"
	Start-Process $mysqld -ArgumentList $serverArguments -WindowStyle Hidden | Out-Null
	$serverLaunched = $true
	$env:FUTUREMUD_GATHERING_TEST_CONNECTION = "Server=127.0.0.1;Port=$taskPort;User ID=root;Database=;SslMode=None;AllowPublicKeyRetrieval=True"

	for ($attempt = 0; $attempt -lt 60; $attempt++) {
		& $mysql '--no-defaults' '--protocol=tcp' '--host=127.0.0.1' "--port=$taskPort" '--user=root' '--skip-password' '--connect-timeout=1' '--batch' '--skip-column-names' '--execute=SELECT 1' *> $null
		if ($LASTEXITCODE -eq 0) {
			$reachable = $true
			break
		}

		Start-Sleep -Milliseconds 500
	}

	if (-not $reachable) {
		throw 'The owned MySQL instance did not become reachable.'
	}

	& dotnet $harnessDll --probe
	$runExit = $LASTEXITCODE
	if (-not $LandOnly -and $runExit -eq 0) {
		& dotnet $harnessDll --run
		$runExit = $LASTEXITCODE
	}
	if ($runExit -eq 0) {
		& dotnet $harnessDll --land-run
		$runExit = $LASTEXITCODE
	}
	Write-Output "nativeHarnessExit=$runExit"
}
catch {
	$runExit = 1
	Write-Output "runnerFailure=$($_.Exception.Message)"
}
finally {
	try {
		if ($reachable) {
			Stop-OwnedMySql $taskPort
		}
		elseif ($serverLaunched) {
			throw 'The temporary server was launched but could not be verified for safe cleanup.'
		}

		if (Test-Path -LiteralPath $taskRoot) {
			Require-TemporaryRoot $taskRoot
			Remove-Item -LiteralPath $taskRoot -Recurse -Force
		}

		$cleanupSucceeded = $true
		Write-Output 'temporaryMySqlCleanup=gracefully-shut-down-and-deleted-owned-instance'
	}
	catch {
		Write-Output "cleanupFailure=$($_.Exception.Message)"
	}
}

if (-not $cleanupSucceeded) {
	exit 2
}

exit $runExit
