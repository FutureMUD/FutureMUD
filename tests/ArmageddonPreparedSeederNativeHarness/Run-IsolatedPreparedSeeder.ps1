[CmdletBinding()]
param([string]$EvidenceRoot,
	[ValidateSet('prepared', 'replay', 'settings', 'stock-progressions', 'stock-supplies', 'stock-anatomy', 'stock-catalogue', 'stock-catalogue-boot', 'catalogue-fixture-boot', 'base-fixture-capture', 'base-fixture-restore', 'base-fixture-package', 'gate-fixture-restore', 'exchange-fixture-restore', 'focus-fixture-restore', 's3-runtime', 'snapshot-refresh', 'stock-progression-fixtures', 'installed', 'installed-sql', 'sense', 'sense-control')]
	[string]$Mode = 'prepared',
	[string]$BaseFixture,
	[string]$BaseFixtureSha256,
	[string]$GateFixture,
	[string]$GateFixtureSha256,
	[string]$ExchangeFixture,
	[string]$ExchangeFixtureSha256,
	[string]$FocusDeviceFixture,
	[string]$FocusDeviceFixtureSha256,
	[ValidateSet('Classic', 'Simple')][string]$PackageMode = 'Classic',
	[ValidateSet('catalogue', 's4-stock')][string]$QualificationScope = 'catalogue',
	[switch]$GateSmokeOnly,
	[switch]$ExchangeSmokeOnly,
	[switch]$RemainingSmokeOnly,
	[switch]$FocusDeviceSmokeOnly,
	[switch]$S5DoseSmokeOnly,
	[switch]$S5ProgressionSmokeOnly,
	[switch]$S6IntegrationSmokeOnly,
	[switch]$S6ReplayCapture,
	[ValidateSet('Off','On')][string]$EarlierClasses = 'Off',
	[switch]$FamilySmokeOnly,
	[ValidateSet('all', 'corpse', 'projection', 'voice', 'pocket', 'kindle')][string]$FamilySmokeCases = 'all',
	[ValidateSet('PsionicsSeeder', 'EnvironmentalExposureSeeder')][string[]]$LaterSteps = @(),
	[switch]$FreshInstallCheck)

$ErrorActionPreference = 'Stop'

$workspaceRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$harnessDll = Join-Path $PSScriptRoot 'bin\Debug\net10.0\GatheringNativePersistenceHarness.dll'
$mysqld = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqld.exe'
$mysql = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe'
$mysqladmin = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqladmin.exe'
$taskRoot = Join-Path $env:TEMP ('futuremud-prepared-seeder-mysql_' + [guid]::NewGuid().ToString('N'))
$taskData = Join-Path $taskRoot 'data'
$taskLog = Join-Path $taskRoot 'mysql.err'
$taskPidFile = Join-Path $taskRoot 'mysql.pid'
$temporaryBase = ([IO.Path]::GetFullPath($env:TEMP)).TrimEnd('\') + '\futuremud-prepared-seeder-mysql_'
$serverLaunched = $false
$reachable = $false
$cleanupSucceeded = $false
$runExit = 1
$previousSnapshotConnection = $env:FUTUREMUD_SNAPSHOT_CONNECTION_STRING
$ownedEnvironmentNames = @('FUTUREMUD_BASE_WORLD_FIXTURE', 'FUTUREMUD_BASE_WORLD_FIXTURE_SHA256', 'FUTUREMUD_BASE_PACKAGE_MODE', 'FUTUREMUD_BASE_LATER_STEPS', 'FUTUREMUD_BASE_WORLD_RECEIPT', 'FUTUREMUD_BASE_FRESH_INSTALL_CHECK', 'FUTUREMUD_GATHERING_TEST_CONNECTION', 'FUTUREMUD_OWNED_MYSQL_UUID', 'FUTUREMUD_OWNED_MYSQL_PORT', 'FUTUREMUD_OWNED_MYSQL_DATADIR', 'FUTUREMUD_CAPACITY_ACCEPTANCE_OUTPUT', 'FUTUREMUD_S4_ATTUNEMENT_STOCK_SMOKE', 'FUTUREMUD_S4_GATE_SMOKE_ONLY', 'FUTUREMUD_S4_GATE_FIXTURE', 'FUTUREMUD_S4_GATE_FIXTURE_SHA256', 'FUTUREMUD_CATALOGUE_BOOT_SCOPE', 'FUTUREMUD_PREPARED_REPLAY_BOOT_SCRIPT', 'FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT')
$ownedPreviousEnvironment = @{}
foreach ($ownedName in $ownedEnvironmentNames) { $ownedPreviousEnvironment[$ownedName] = [Environment]::GetEnvironmentVariable($ownedName) }
if ($S6IntegrationSmokeOnly -and ($S5ProgressionSmokeOnly -or $S5DoseSmokeOnly -or $FocusDeviceSmokeOnly -or $FamilySmokeOnly -or $RemainingSmokeOnly -or $GateSmokeOnly -or $ExchangeSmokeOnly -or $QualificationScope -ne 's4-stock' -or $Mode -ne 'base-fixture-package')) { throw 'S6 integration requires its own isolated pinned-base current-package lane.' }
if ($S6ReplayCapture -and ($Mode -ne 'base-fixture-capture' -or $EarlierClasses -ne 'On')) { throw 'S6 full replay capture requires a fresh classes-on base capture.' }
foreach ($entry in @{
	FUTUREMUD_S6_INTEGRATION_SMOKE_ONLY = $(if ($S6IntegrationSmokeOnly) { '1' } else { $null })
	FUTUREMUD_S6_REPLAY_CAPTURE = $(if ($S6ReplayCapture) { '1' } else { $null })
	FUTUREMUD_BASE_EARLIER_CLASSES = $EarlierClasses
}.GetEnumerator()) {
	$ownedEnvironmentNames += $entry.Key
	$ownedPreviousEnvironment[$entry.Key] = [Environment]::GetEnvironmentVariable($entry.Key)
	[Environment]::SetEnvironmentVariable($entry.Key, $entry.Value)
}
if ($GateSmokeOnly -and $QualificationScope -ne 's4-stock') { throw 'Gate-only native qualification requires -QualificationScope s4-stock.' }
if ($ExchangeSmokeOnly -and ($GateSmokeOnly -or $QualificationScope -ne 's4-stock' -or $Mode -notin @('base-fixture-package', 'exchange-fixture-restore'))) { throw 'Exchange-only qualification requires a separate s4-stock package or exact Exchange restore run.' }
if ($RemainingSmokeOnly -and ($GateSmokeOnly -or $ExchangeSmokeOnly -or $QualificationScope -ne 's4-stock' -or $Mode -ne 'base-fixture-package')) { throw 'Remaining-stock qualification requires its separate s4-stock package lane.' }
if ($FamilySmokeOnly -and ($FocusDeviceSmokeOnly -or $RemainingSmokeOnly -or $GateSmokeOnly -or $ExchangeSmokeOnly -or $QualificationScope -ne 's4-stock' -or $Mode -ne 'focus-fixture-restore')) { throw 'Family acceptance requires its separate hash-pinned prepared-fixture restore.' }
if ($FamilySmokeCases -ne 'all' -and -not $FamilySmokeOnly) { throw 'Family case selection requires -FamilySmokeOnly.' }
if ($S5ProgressionSmokeOnly -and ($S5DoseSmokeOnly -or $FocusDeviceSmokeOnly -or $FamilySmokeOnly -or $RemainingSmokeOnly -or $GateSmokeOnly -or $ExchangeSmokeOnly -or $QualificationScope -ne 's4-stock' -or $Mode -ne 'base-fixture-package')) { throw 'S5 progression requires its own current-package install on the pinned clean base.' }
$ownedEnvironmentNames += 'FUTUREMUD_S5_PROGRESSION_SMOKE_ONLY'
$ownedPreviousEnvironment['FUTUREMUD_S5_PROGRESSION_SMOKE_ONLY'] = [Environment]::GetEnvironmentVariable('FUTUREMUD_S5_PROGRESSION_SMOKE_ONLY')
$env:FUTUREMUD_S5_PROGRESSION_SMOKE_ONLY = if ($S5ProgressionSmokeOnly) { '1' } else { $null }
if ($S5DoseSmokeOnly -and (-not $FocusDeviceSmokeOnly -or $Mode -ne 'focus-fixture-restore')) { throw 'S5 dose acceptance requires the isolated focus fixture restore and focused smoke lane.' }
$ownedEnvironmentNames += 'FUTUREMUD_S5_DOSE_SMOKE_ONLY'
$ownedPreviousEnvironment['FUTUREMUD_S5_DOSE_SMOKE_ONLY'] = [Environment]::GetEnvironmentVariable('FUTUREMUD_S5_DOSE_SMOKE_ONLY')
$env:FUTUREMUD_S5_DOSE_SMOKE_ONLY = if ($S5DoseSmokeOnly) { '1' } else { $null }

$ownedEnvironmentNames += 'FUTUREMUD_S4_FAMILY_CASES'
$ownedPreviousEnvironment['FUTUREMUD_S4_FAMILY_CASES'] = [Environment]::GetEnvironmentVariable('FUTUREMUD_S4_FAMILY_CASES')
$env:FUTUREMUD_S4_FAMILY_CASES = if ($FamilySmokeOnly) { $FamilySmokeCases } else { $null }
$ownedEnvironmentNames += 'FUTUREMUD_S4_FAMILY_SMOKE_ONLY'
$ownedPreviousEnvironment['FUTUREMUD_S4_FAMILY_SMOKE_ONLY'] = [Environment]::GetEnvironmentVariable('FUTUREMUD_S4_FAMILY_SMOKE_ONLY')
$env:FUTUREMUD_S4_FAMILY_SMOKE_ONLY = if ($FamilySmokeOnly) { '1' } else { $null }
if ($FocusDeviceSmokeOnly -and ($RemainingSmokeOnly -or $GateSmokeOnly -or $ExchangeSmokeOnly -or $QualificationScope -ne 's4-stock' -or $Mode -notin @('base-fixture-package','focus-fixture-restore'))) { throw 'Focus/device qualification requires its separate s4-stock package lane.' }
$ownedEnvironmentNames += 'FUTUREMUD_S4_FOCUS_DEVICE_SMOKE_ONLY'
$ownedPreviousEnvironment['FUTUREMUD_S4_FOCUS_DEVICE_SMOKE_ONLY'] = [Environment]::GetEnvironmentVariable('FUTUREMUD_S4_FOCUS_DEVICE_SMOKE_ONLY')
$env:FUTUREMUD_S4_FOCUS_DEVICE_SMOKE_ONLY = if ($FocusDeviceSmokeOnly) { '1' } else { $null }
$ownedEnvironmentNames += 'FUTUREMUD_S4_REMAINING_SMOKE_ONLY'
$ownedPreviousEnvironment['FUTUREMUD_S4_REMAINING_SMOKE_ONLY'] = [Environment]::GetEnvironmentVariable('FUTUREMUD_S4_REMAINING_SMOKE_ONLY')
$env:FUTUREMUD_S4_REMAINING_SMOKE_ONLY = if ($RemainingSmokeOnly) { '1' } else { $null }
$ownedEnvironmentNames += 'FUTUREMUD_S4_EXCHANGE_SMOKE_ONLY'
$ownedPreviousEnvironment['FUTUREMUD_S4_EXCHANGE_SMOKE_ONLY'] = [Environment]::GetEnvironmentVariable('FUTUREMUD_S4_EXCHANGE_SMOKE_ONLY')
foreach ($exchangeEnvironment in @('FUTUREMUD_S4_EXCHANGE_FIXTURE', 'FUTUREMUD_S4_EXCHANGE_FIXTURE_SHA256', 'FUTUREMUD_S4_FOCUS_DEVICE_FIXTURE', 'FUTUREMUD_S4_FOCUS_DEVICE_FIXTURE_SHA256')) {
	$ownedEnvironmentNames += $exchangeEnvironment
	$ownedPreviousEnvironment[$exchangeEnvironment] = [Environment]::GetEnvironmentVariable($exchangeEnvironment)
}
if ($QualificationScope -eq 's4-stock') {
	if ($Mode -notin @('base-fixture-package', 'gate-fixture-restore', 'exchange-fixture-restore', 'focus-fixture-restore') -or ($Mode -eq 'gate-fixture-restore' -and -not $GateSmokeOnly) -or ($Mode -eq 'exchange-fixture-restore' -and -not $ExchangeSmokeOnly) -or ($Mode -eq 'focus-fixture-restore' -and -not ($FocusDeviceSmokeOnly -or $FamilySmokeOnly))) { throw 'S4 stock qualification requires package mode or an explicit focused fixture restore.' }
	$bootSource = Join-Path $PSScriptRoot 'PreparedCatalogueBootSmoke.py'
	$bootCopy = Join-Path (Split-Path $workspaceRoot -Parent) 'PreparedCatalogueBootSmoke.py'
	if (-not (Test-Path -LiteralPath $bootCopy) -or
		(Get-FileHash -LiteralPath $bootSource -Algorithm SHA256).Hash -ne (Get-FileHash -LiteralPath $bootCopy -Algorithm SHA256).Hash) {
		throw 'S4 stock qualification requires the exact parent-directory copy of PreparedCatalogueBootSmoke.py.'
	}
}
elseif ($Mode -eq 'base-fixture-package' -and $env:FUTUREMUD_S4_ATTUNEMENT_STOCK_SMOKE -eq '1') {
	throw 'S4 stock flag was supplied without -QualificationScope s4-stock; refusing ambiguous catalogue-only qualification.'
}
if ($Mode -in @('base-fixture-restore', 'base-fixture-package', 'gate-fixture-restore', 'exchange-fixture-restore', 'focus-fixture-restore')) {
	if (-not $BaseFixture -or $BaseFixtureSha256 -cnotmatch '^[A-F0-9]{64}$') { throw 'Restore requires an explicit base descriptor and its capture SHA256.' }
	$baseFixtureFull = [IO.Path]::GetFullPath($BaseFixture)
	$baseFixtureBoundary = (Join-Path $workspaceRoot '.artifacts\pre-armageddon-base-fixtures') + [IO.Path]::DirectorySeparatorChar
	if (-not $baseFixtureFull.StartsWith($baseFixtureBoundary, [StringComparison]::OrdinalIgnoreCase) -or
		-not (Test-Path -LiteralPath $baseFixtureFull) -or (Get-FileHash -LiteralPath $baseFixtureFull -Algorithm SHA256).Hash -ne $BaseFixtureSha256) {
		throw 'The base descriptor path or explicit digest is invalid.'
	}
}
if ($Mode -eq 'gate-fixture-restore') {
	$gateFixtureFull = [IO.Path]::GetFullPath($GateFixture)
	$gateFixtureBoundary = (Join-Path $workspaceRoot '.artifacts\s4-nilaz-gate-fixtures') + [IO.Path]::DirectorySeparatorChar
	if ($GateFixtureSha256 -cnotmatch '^[A-F0-9]{64}$' -or
		-not $gateFixtureFull.StartsWith($gateFixtureBoundary, [StringComparison]::OrdinalIgnoreCase) -or
		-not (Test-Path -LiteralPath $gateFixtureFull) -or
		(Get-FileHash -LiteralPath $gateFixtureFull -Algorithm SHA256).Hash -ne $GateFixtureSha256) {
		throw 'The focused Nilaz fixture path or explicit digest is invalid.'
	}
}
if ($Mode -eq 'exchange-fixture-restore') {
	$exchangeFixtureFull = [IO.Path]::GetFullPath($ExchangeFixture)
	$exchangeFixtureBoundary = (Join-Path $workspaceRoot '.artifacts\s4-exchange-fixtures') + [IO.Path]::DirectorySeparatorChar
	if ($ExchangeFixtureSha256 -cnotmatch '^[A-F0-9]{64}$' -or
		-not $exchangeFixtureFull.StartsWith($exchangeFixtureBoundary, [StringComparison]::OrdinalIgnoreCase) -or
		-not (Test-Path -LiteralPath $exchangeFixtureFull) -or (Get-FileHash -LiteralPath $exchangeFixtureFull -Algorithm SHA256).Hash -ne $ExchangeFixtureSha256) {
		throw 'The focused Exchange fixture path or explicit digest is invalid.'
	}
}
if ($Mode -eq 'focus-fixture-restore') {
	$focusFixtureFull = [IO.Path]::GetFullPath($FocusDeviceFixture)
	$focusFixtureBoundary = (Join-Path $workspaceRoot '.artifacts\s4-focus-device-fixtures') + [IO.Path]::DirectorySeparatorChar
	if ($FocusDeviceFixtureSha256 -cnotmatch '^[A-F0-9]{64}$' -or -not $focusFixtureFull.StartsWith($focusFixtureBoundary, [StringComparison]::OrdinalIgnoreCase) -or
		-not (Test-Path -LiteralPath $focusFixtureFull) -or (Get-FileHash -LiteralPath $focusFixtureFull -Algorithm SHA256).Hash -ne $FocusDeviceFixtureSha256) { throw 'Invalid explicit focus/device fixture descriptor or digest.' }
}
if ($FreshInstallCheck -and $Mode -ne 'base-fixture-capture') { throw 'FreshInstallCheck applies only to capture.' }
if ($LaterSteps.Count -gt 0 -and $Mode -ne 'base-fixture-package') { throw 'LaterSteps applies only to targeted package mode.' }
if ($QualificationScope -eq 's4-stock') {
	$env:FUTUREMUD_S4_ATTUNEMENT_STOCK_SMOKE = '1'
	$env:FUTUREMUD_S4_GATE_SMOKE_ONLY = if ($GateSmokeOnly) { '1' } else { '0' }
	$env:FUTUREMUD_S4_EXCHANGE_SMOKE_ONLY = if ($ExchangeSmokeOnly) { '1' } else { '0' }
	$env:FUTUREMUD_S4_EXCHANGE_FIXTURE = $ExchangeFixture
	$env:FUTUREMUD_S4_EXCHANGE_FIXTURE_SHA256 = $ExchangeFixtureSha256
	$env:FUTUREMUD_S4_FOCUS_DEVICE_FIXTURE = $FocusDeviceFixture
	$env:FUTUREMUD_S4_FOCUS_DEVICE_FIXTURE_SHA256 = $FocusDeviceFixtureSha256
	$env:FUTUREMUD_CATALOGUE_BOOT_SCOPE = 'catalogue'
	$env:FUTUREMUD_PREPARED_REPLAY_BOOT_SCRIPT = [IO.Path]::GetFullPath($bootCopy)
	$env:FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT = Join-Path (Split-Path $workspaceRoot -Parent) ('prepared-catalogue-boot-native-s4-stock-' + [guid]::NewGuid().ToString('N'))
}
$evidenceParent = if ($EvidenceRoot) { [IO.Path]::GetFullPath($EvidenceRoot) } else { Join-Path $workspaceRoot '.artifacts\armageddon-prepared-seeder' }
$evidenceDirectory = Join-Path $evidenceParent ([DateTime]::UtcNow.ToString('yyyyMMddTHHmmssZ') + '_' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $evidenceDirectory -Force | Out-Null

function Write-SourceEvidence {
	param([string]$Name)
	$sourcePaths = @(& git -c core.quotepath=false -C $workspaceRoot ls-files --cached --others --exclude-standard)
	if ($LASTEXITCODE -ne 0) { throw 'Source inventory failed.' }
	$sourcePaths = @($sourcePaths | Where-Object { $Mode -ne 'snapshot-refresh' -or $_ -notin @('DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.sql', 'DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.manifest.json') })
	$records = @($sourcePaths | Where-Object { $_ -match '\.(cs|csproj|props|targets|sln|json|ps1|md|sql|sql\.gz)$' -or $_ -like 'tests/ArmageddonPreparedSeederNativeHarness/*.py' -or
		$_ -in @('tests/ArmageddonPreparedSeederNativeHarness/PreparedReplayBootSmoke.py', 'tests/ArmageddonPreparedSeederNativeHarness/PreparedInstalledMagicSmoke.py', 'tests/ArmageddonPreparedSeederNativeHarness/PreparedCatalogueBootSmoke.py') } | Sort-Object -Unique | ForEach-Object {
		$sourcePath = Join-Path $workspaceRoot $_
		if (-not (Test-Path -LiteralPath $sourcePath)) { throw "Source missing: $_" }
		@{ path = $_; sha256 = (Get-FileHash -LiteralPath $sourcePath -Algorithm SHA256).Hash }
	})
	$manifest = $records | ConvertTo-Json -Depth 4 -Compress
	$manifestPath = Join-Path $evidenceDirectory ($Name + '.json')
	[IO.File]::WriteAllText($manifestPath, $manifest, [Text.UTF8Encoding]::new($false))
	return (Get-FileHash -LiteralPath $manifestPath -Algorithm SHA256).Hash
}

function Write-AssemblyEvidence {
	$assemblyDirectory = Split-Path -Parent $harnessDll
	$records = @('GatheringNativePersistenceHarness.dll', 'GatheringNativePersistenceHarness.deps.json', 'GatheringNativePersistenceHarness.runtimeconfig.json',
		'MudSharp.dll', 'FutureMUDLibrary.dll', 'MudsharpDatabaseLibrary.dll', 'ExpressionEngine.dll', 'DatabaseSeeder.dll') | ForEach-Object {
		$assemblyPath = Join-Path $assemblyDirectory $_
		@{ path = $assemblyPath; sha256 = (Get-FileHash -LiteralPath $assemblyPath -Algorithm SHA256).Hash }
	}
	$records | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $evidenceDirectory 'assemblies.json')
}

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
	$env:FUTUREMUD_BASE_WORLD_FIXTURE = $BaseFixture
	$env:FUTUREMUD_BASE_WORLD_FIXTURE_SHA256 = $BaseFixtureSha256
	$env:FUTUREMUD_S4_GATE_FIXTURE = $GateFixture
	$env:FUTUREMUD_S4_GATE_FIXTURE_SHA256 = $GateFixtureSha256
	$env:FUTUREMUD_BASE_PACKAGE_MODE = $PackageMode
	$env:FUTUREMUD_BASE_LATER_STEPS = $LaterSteps -join ','
	$env:FUTUREMUD_BASE_WORLD_RECEIPT = Join-Path $evidenceDirectory 'base-world-run.json'
	$env:FUTUREMUD_BASE_FRESH_INSTALL_CHECK = if ($FreshInstallCheck) { '1' } else { $null }
	$sourceStart = Write-SourceEvidence 'source-start'
	Write-AssemblyEvidence
	Write-Output "nativeEvidence=$evidenceDirectory sourceStart=$sourceStart"
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
	# One mode per fresh endpoint prevents accumulated Windows TCP TIME_WAIT exhaustion.
	$argument = "--$Mode-run"
	Invoke-OwnedHarness $argument
	$runExit = $LASTEXITCODE
	$stockFirstLoaded = 0
	$stockColdLoaded = 0
	$stockAssertions = 0
	$stockCampaignAssertions = 0
	$stockBootReceipt = $null
	if ($QualificationScope -eq 's4-stock' -and $runExit -eq 0) {
		try {
			$stockBootReceipt = Join-Path $env:FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT 'boot-latest.json'
			$boot = Get-Content -LiteralPath $stockBootReceipt -Raw | ConvertFrom-Json
			if (-not $GateSmokeOnly -and -not $ExchangeSmokeOnly -and -not $FocusDeviceSmokeOnly -and -not $FamilySmokeOnly) {
				$stockFirstLoaded = @($boot.stock_definition_loader.first.PSObject.Properties).Count
				$stockColdLoaded = @($boot.stock_definition_loader.cold.PSObject.Properties).Count
			}
			$stockAssertions = @($boot.assertions).Count
			$stockCampaignAssertions = @($boot.assertions | Where-Object { $_ -like '*S4 stock*' }).Count
			if ($S6IntegrationSmokeOnly) {
				if ($boot.status -ne 'PASS' -or $boot.s6_integration.status -ne 'PASS' -or $boot.s6_integration.cold_status -ne 'PASS' -or $stockFirstLoaded -ne 152 -or $stockColdLoaded -ne 152 -or -not $boot.owned_mud_processes_stopped) { throw 'S6 integration requires all152 definitions and ordinary-player first/cold assertions.' }
				Write-Output "s6IntegrationQualification=verified mode=$PackageMode classes=$EarlierClasses assertions=$stockAssertions"
			}
			elseif ($S5ProgressionSmokeOnly) {
				if ($boot.status -ne 'PASS' -or $boot.s5_progression.status -ne 'PASS' -or $boot.s5_progression.cold_status -ne 'PASS' -or -not $boot.owned_mud_processes_stopped) { throw 'S5 normal progression/funding first/cold qualification is incomplete.' }
				Write-Output "s5ProgressionQualification=verified assertions=$stockAssertions"
			}
			elseif ($ExchangeSmokeOnly) {
				if ($boot.status -ne 'PASS' -or $boot.exchange_places.status -ne 'PASS' -or
					$boot.exchange_places.cold_status -ne 'PASS' -or -not $boot.owned_mud_processes_stopped) {
					throw 'Paid Exchange Places first/cold or owned-process assertions are incomplete.'
				}
				Write-Output "s4ExchangeQualification=verified assertions=$stockAssertions"
			}
			elseif ($GateSmokeOnly) {
				if ($boot.status -ne 'PASS' -or $boot.scope -ne 'catalogue' -or
					$boot.nilaz_gate.status -ne 'PASS' -or $boot.nilaz_gate.cold_status -ne 'PASS' -or
					-not $boot.owned_mud_processes_stopped) {
					throw 'Paid Nilaz gate first/cold or owned-process assertions are incomplete.'
				}
				Write-Output "s4GateQualification=verified assertions=$stockAssertions"
			}
			elseif ($FamilySmokeOnly) {
				if ($boot.status -ne 'PASS' -or $boot.family_acceptance.status -ne 'PASS' -or $boot.family_acceptance.cold_status -ne 'PASS' -or -not $boot.owned_mud_processes_stopped) { throw 'Family acceptance first/cold evidence is incomplete.' }
				Write-Output "s4FamilyQualification=verified assertions=$stockAssertions"
			}
			elseif ($FocusDeviceSmokeOnly) {
				if ($S5DoseSmokeOnly -and ($boot.focus_devices.stage -ne 'S5' -or $boot.focus_devices.scenario -ne 'restorative-mud' -or @($boot.focus_devices.applications).Count -ne 2)) { throw 'S5 dose scenario and both native calibrated applications are required.' }
				if ($boot.status -ne 'PASS' -or $boot.focus_devices.status -ne 'PASS' -or $boot.focus_devices.cold_status -ne 'PASS' -or -not $boot.owned_mud_processes_stopped) { throw 'Focus/device first/cold acceptance is incomplete.' }
				Write-Output "s4FocusDeviceQualification=verified assertions=$stockAssertions"
			}
			elseif ($RemainingSmokeOnly) {
				if ($boot.status -ne 'PASS' -or $boot.remaining_stock.status -ne 'PASS' -or $boot.remaining_stock.cold_status -ne 'PASS' -or
					$stockFirstLoaded -lt 1 -or $stockFirstLoaded -ne $stockColdLoaded -or -not $boot.owned_mud_processes_stopped) { throw 'Remaining-stock first/cold journals, native movement/injury, or loader assertions are incomplete.' }
				Write-Output "s4RemainingQualification=verified first=$stockFirstLoaded cold=$stockColdLoaded assertions=$stockAssertions"
			}
			else {
				if ($boot.status -ne 'PASS' -or $boot.scope -ne 'catalogue' -or
					$boot.representative_campaign.status -ne 'PASS' -or $stockCampaignAssertions -lt 1 -or
					$stockFirstLoaded -lt 1 -or $stockColdLoaded -ne $stockFirstLoaded -or
					-not $boot.owned_mud_processes_stopped) {
					throw 'Stock first/cold definition, paid S4 campaign, or owned-process assertions are incomplete.'
				}
				Write-Output "s4StockQualification=verified first=$stockFirstLoaded cold=$stockColdLoaded assertions=$stockAssertions"
			}
		}
		catch {
			$runExit = 1
			Write-Output "s4StockQualification=failed reason=$($_.Exception.Message)"
		}
	}
	Write-Output "nativeHarnessExit=$runExit"
	$sourceEnd = Write-SourceEvidence 'source-end'
	$sourceStable = $sourceStart -eq $sourceEnd
	$qualificationLimits = if ($S6IntegrationSmokeOnly) { 'S6 inactive admission/support graph install cell and ordinary Player enrolment/cold sample; selected-Class service tests are separate from complete chargen approval/UI. Gathering inputs pending; no complete S6 or Release qualification.' } elseif ($S5ProgressionSmokeOnly) { 'S5 full-stock Earth ordinary Player acquisition and real funding/practice first-cold sample; separate shared-contract branching and approved carrier receipts complete S5 scope; no S6/Release qualification.' } elseif ($QualificationScope -eq 's4-stock') { 'Focused Telnet evidence is in stock_boot_receipt; no whole-package or S5 qualification.' } else { 'Native Character/Body/items/skills/resources/database with controlled world/check catalogue; no Telnet/login certificate.' }
	@{ revision = $revision; mode = $Mode; earlier_classes = $EarlierClasses; s6_integration_smoke_only = [bool]$S6IntegrationSmokeOnly; s6_replay_capture = [bool]$S6ReplayCapture; qualification_scope = $QualificationScope; gate_smoke_only = [bool]$GateSmokeOnly; exchange_smoke_only = [bool]$ExchangeSmokeOnly; remaining_smoke_only = [bool]$RemainingSmokeOnly; focus_device_smoke_only = [bool]$FocusDeviceSmokeOnly; s5_dose_smoke_only = [bool]$S5DoseSmokeOnly; s5_progression_smoke_only = [bool]$S5ProgressionSmokeOnly; family_smoke_only = [bool]$FamilySmokeOnly; family_smoke_cases = $FamilySmokeCases; focus_device_fixture = $FocusDeviceFixture; focus_device_fixture_sha256 = $FocusDeviceFixtureSha256; stock_first_loaded = $stockFirstLoaded; stock_cold_loaded = $stockColdLoaded; stock_assertions = $stockAssertions; stock_campaign_assertions = $stockCampaignAssertions; stock_boot_receipt = $stockBootReceipt; source_start = $sourceStart; source_end = $sourceEnd; source_stable = $sourceStable; native_exit = $runExit;
		base_fixture = $BaseFixture; base_fixture_sha256 = $BaseFixtureSha256; gate_fixture = $GateFixture; gate_fixture_sha256 = $GateFixtureSha256; exchange_fixture = $ExchangeFixture; exchange_fixture_sha256 = $ExchangeFixtureSha256; package_mode = $PackageMode; later_steps = $LaterSteps; fresh_install_check = [bool]$FreshInstallCheck; base_receipt = (Join-Path $evidenceDirectory 'base-world-run.json'); process_root = $taskRoot; owned_port = $taskPort; limits = $qualificationLimits } |
		ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $evidenceDirectory 'summary.json')
	Write-Output "nativeSourceStable=$sourceStable sourceEnd=$sourceEnd"
	if (-not $sourceStable) { $runExit = 1 }
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
			if (Test-Path -LiteralPath $taskLog) { Copy-Item -LiteralPath $taskLog -Destination (Join-Path $evidenceDirectory 'mysql.err') }
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
