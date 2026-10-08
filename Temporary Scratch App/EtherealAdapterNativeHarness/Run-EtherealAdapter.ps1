[CmdletBinding()]
param([ValidateSet('ethereal-adapter','water-stock','water-adapter','pierce-stock','pierce-shared-runtime','provision-stock','five-stock','regressions')][string]$Mode = 'ethereal-adapter')

$ErrorActionPreference = 'Stop'

$workspaceRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$harnessDll = Join-Path $PSScriptRoot 'bin\Debug\net10.0\GatheringNativePersistenceHarness.dll'
$mysqld = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqld.exe'
$mysql = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe'
$mysqladmin = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqladmin.exe'
$taskRoot = Join-Path $env:TEMP ('futuremud-ethereal-adapter-mysql_' + [guid]::NewGuid().ToString('N'))
$taskData = Join-Path $taskRoot 'data'
$taskLog = Join-Path $taskRoot 'mysql.err'
$taskPidFile = Join-Path $taskRoot 'mysql.pid'
$temporaryBase = ([IO.Path]::GetFullPath($env:TEMP)).TrimEnd('\') + '\futuremud-ethereal-adapter-mysql_'
$serverLaunched = $false
$reachable = $false
$cleanupSucceeded = $false
$runExit = 1
$previousSnapshotConnection = $env:FUTUREMUD_SNAPSHOT_CONNECTION_STRING
$ownedEnvironmentNames = @('FUTUREMUD_GATHERING_TEST_CONNECTION', 'FUTUREMUD_OWNED_MYSQL_UUID', 'FUTUREMUD_OWNED_MYSQL_PORT', 'FUTUREMUD_OWNED_MYSQL_DATADIR', 'FUTUREMUD_CAPACITY_ACCEPTANCE_OUTPUT')
$ownedPreviousEnvironment = @{}
foreach ($ownedName in $ownedEnvironmentNames) { $ownedPreviousEnvironment[$ownedName] = [Environment]::GetEnvironmentVariable($ownedName) }

function Require-OwnedServer {
	param([string]$Boundary)
	$ownedDescriptor = @(& $mysql '--no-defaults' '--protocol=tcp' '--host=127.0.0.1' "--port=$taskPort" '--user=root' '--skip-password' '--batch' '--skip-column-names' '--execute=SELECT @@server_uuid, @@datadir, @@port')
	if ($LASTEXITCODE -ne 0 -or $ownedDescriptor.Count -ne 1) { throw 'Unable to verify the owned MySQL process boundary.' }
	$ownedFields = $ownedDescriptor[0].Split([char]9)
	$ownedUuidLine = @(Get-Content -LiteralPath (Join-Path $taskData 'auto.cnf') | Where-Object { $_.StartsWith('server-uuid=') })
	if ($ownedFields.Count -ne 3 -or $ownedUuidLine.Count -ne 1 -or $ownedFields[0] -ne $ownedUuidLine[0].Substring(12) -or
		[int]$ownedFields[2] -ne $taskPort -or
		-not $ownedFields[1].Replace('/', '\').Replace('\\', '\').TrimEnd('\').Equals(([IO.Path]::GetFullPath($taskData)).TrimEnd('\'), [StringComparison]::OrdinalIgnoreCase)) {
		throw 'The MySQL endpoint does not match this run''s exact owned instance.'
	}
	$env:FUTUREMUD_OWNED_MYSQL_UUID = $ownedFields[0]
	$env:FUTUREMUD_OWNED_MYSQL_PORT = [string]$taskPort
	$env:FUTUREMUD_OWNED_MYSQL_DATADIR = [IO.Path]::GetFullPath($taskData)
	Write-Output "ownedServer=verified boundary=$Boundary endpoint=127.0.0.1:$taskPort exact-instance=true"
}

function Invoke-OwnedHarness {
	param([string]$Mode)
	Require-OwnedServer -Boundary ('process-' + $Mode)
	& dotnet $harnessDll $Mode
}

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

	# A full snapshot refresh must preserve the maintained dump's database default.
	$serverCollation = 'utf8mb4_general_ci'
	$serverArguments = "--no-defaults --basedir=`"C:\Program Files\MySQL\MySQL Server 8.0`" --datadir=`"$taskData`" --port=$taskPort --bind-address=127.0.0.1 --pid-file=`"$taskPidFile`" --log-error=`"$taskLog`" --character-set-server=utf8mb4 --collation-server=$serverCollation --mysqlx=0"
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
	Require-OwnedServer -Boundary 'runner-ready'
	$env:FUTUREMUD_CAPACITY_ACCEPTANCE_OUTPUT = Join-Path $workspaceRoot ('.artifacts/test-runs/ethereal-adapter-native-' + [guid]::NewGuid().ToString('N'))
	Invoke-OwnedHarness '--probe'
	$runExit=$LASTEXITCODE
	$modes=switch($Mode) {
		'ethereal-adapter' { @('--ethereal-adapter-run') }
		'water-stock' { @('--water-breathing-stock-run') }
		'water-adapter' { @('--water-breathing-adapter-run') }
		'pierce-stock' { @('--pierce-stock-run') }
		'pierce-shared-runtime' { @('--pierce-shared-runtime-run') }
		'provision-stock' { @('--provision-stock-run') }
		'five-stock' { @('--five-stock-run') }
		'regressions' { @('--raise-servitor-stock-run','--storm-spear-stock-run','--flame-knife-stock-run','--sand-knife-stock-run') }
	}
	foreach($nativeMode in $modes) {
		if($runExit -ne 0) { break }
		Invoke-OwnedHarness $nativeMode
		$runExit=$LASTEXITCODE
	}
	Write-Output "nativeHarnessExit=$runExit"
}
catch {
	$runExit = 1
	Write-Output "runnerFailure=$($_.Exception.Message)"
}
finally {
	$env:FUTUREMUD_SNAPSHOT_CONNECTION_STRING = $previousSnapshotConnection
	try {
		if ($reachable) {
			Require-OwnedServer -Boundary 'cleanup-before-shutdown'
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
	finally {
		foreach ($ownedName in $ownedEnvironmentNames) { [Environment]::SetEnvironmentVariable($ownedName, $ownedPreviousEnvironment[$ownedName]) }
	}
}

if (-not $cleanupSucceeded) {
	exit 2
}

exit $runExit
