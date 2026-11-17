"""Run the opt-in employment persistence test in two independent test hosts."""
from __future__ import annotations

import argparse
import json
import os
import pathlib
import re
import shutil
import subprocess
import sys
import time
import uuid
import xml.etree.ElementTree as ET


def main() -> int:
	parser = argparse.ArgumentParser(description=__doc__)
	parser.add_argument('--run-dir', required=True, type=pathlib.Path)
	args = parser.parse_args()
	root = args.run_dir.resolve()
	repo = pathlib.Path(__file__).resolve().parents[2]
	attempt = root / ('employment-attempt-' + uuid.uuid4().hex[:12])
	attempt.mkdir()
	receipt_path = attempt / 'employment-restart.json'
	receipt = {'status': 'FAIL', 'scenario': 'employment native provider restart', 'phases': [], 'assertions': []}
	try:
		instance_path = root / 'mysql-instance.json'
		instance = json.loads(instance_path.read_text(encoding='utf-8'))
		data_dir = pathlib.Path(instance['data']).resolve()
		expected_data = (root / 'mysql' / 'data').resolve()
		port = instance['port']
		expected_uuid = str(uuid.UUID(instance['server_uuid'])).lower()
		client = [str(part) for part in instance['client']]
		if (data_dir != expected_data or not data_dir.is_relative_to(root) or
			type(port) is not int or not 1 <= port <= 65535 or
			instance.get('database') != 'authored_celestial_smoke' or
			'--host=127.0.0.1' not in client or '--port=' + str(port) not in client):
			raise RuntimeError('MySQL metadata is not the expected owned loopback instance; refusing database work.')
		uuid_match = re.search(r'^server-uuid\s*=\s*([0-9a-fA-F-]{36})\s*$',
			(data_dir / 'auto.cnf').read_text(encoding='utf-8-sig'), re.M)
		if not uuid_match or str(uuid.UUID(uuid_match.group(1))).lower() != expected_uuid:
			raise RuntimeError('Owned MySQL data UUID mismatch; refusing database work.')
		flags = subprocess.CREATE_NO_WINDOW if os.name == 'nt' else 0
		def sql(query: str, database: str | None = None) -> str:
			command = client + (['--database=' + database] if database else []) + ['-e', query]
			result = subprocess.run(command, capture_output=True, text=True, encoding='utf-8',
				errors='replace', timeout=30, creationflags=flags)
			if result.returncode:
				raise RuntimeError('Owned MySQL query failed; refusing to continue.')
			return result.stdout.strip()

		def verify_identity() -> None:
			fields = sql('SELECT @@server_uuid, @@datadir, @@port').split('\t')
			if (len(fields) != 3 or str(uuid.UUID(fields[0])).lower() != expected_uuid or
				pathlib.Path(fields[1]).resolve() != data_dir or int(fields[2]) != port):
				raise RuntimeError('Live MySQL identity mismatch; refusing database work.')

		def check(label: str, passed: bool, detail: str = '') -> None:
			receipt['assertions'].append({'name': label, 'passed': bool(passed), **({'detail': detail} if detail else {})})
			if not passed:
				raise RuntimeError('Assertion failed: ' + label)

		verify_identity()
		check('owned_loopback_mysql_identity', True)
		suffix = uuid.uuid4().hex[:12]
		refresh_schema = 'employment_snapshot_' + suffix
		import_schema = 'employment_snapshot_import_' + suffix
		for schema in (refresh_schema, import_schema):
			check('schema_unused_' + schema, not sql("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" + schema + "'"))

		assets = repo / 'DatabaseSeeder' / 'Assets' / 'Database'
		snapshot_path = assets / 'BlankDatabaseSnapshot.sql'
		manifest_path = assets / 'BlankDatabaseSnapshot.manifest.json'
		backup_dir = attempt / 'snapshot-before-refresh'
		backup_dir.mkdir(exist_ok=True)
		shutil.copy2(snapshot_path, backup_dir / snapshot_path.name)
		shutil.copy2(manifest_path, backup_dir / manifest_path.name)
		receipt['snapshotBaseline'] = {'snapshot': str(backup_dir / snapshot_path.name),
			'manifest': str(backup_dir / manifest_path.name)}
		verify_identity()
		refresh_connection = 'server=127.0.0.1;port={};database={};uid=root;SslMode=Disabled;AllowPublicKeyRetrieval=True;Default Command Timeout=600;'.format(port, refresh_schema)
		refresh_env = os.environ.copy()
		refresh_env['FUTUREMUD_SNAPSHOT_CONNECTION_STRING'] = refresh_connection
		refresh_started = time.monotonic()
		refresh_command = ['dotnet', str(repo / 'DatabaseSeeder' / 'bin' / 'Debug' / 'net11.0' / 'DatabaseSeeder.dll'), '--refresh-blank-snapshot']
		# The common harness uses unicode_ci; preserve the maintained blank snapshot's
		# 0900 default instead of changing hundreds of unrelated table defaults.
		original_collation = sql('SELECT @@GLOBAL.collation_server')
		if not re.fullmatch(r'[a-zA-Z0-9_]+', original_collation):
			raise RuntimeError('Unexpected owned server collation; refusing refresh.')
		sql("SET GLOBAL collation_server='utf8mb4_0900_ai_ci'")
		try:
			refresh = subprocess.run(refresh_command, cwd=repo, env=refresh_env, capture_output=True,
				text=True, encoding='utf-8', errors='replace', timeout=900, creationflags=flags)
		finally:
			verify_identity()
			sql("SET GLOBAL collation_server='" + original_collation + "'")
		refresh_log = (refresh.stdout + refresh.stderr).replace(refresh_connection, '[REDACTED CONNECTION]')
		(attempt / 'employment-snapshot-refresh.log').write_text(refresh_log, encoding='utf-8')
		receipt['snapshotRefresh'] = {'schema': refresh_schema, 'exitCode': refresh.returncode,
			'durationSeconds': round(time.monotonic() - refresh_started, 3), 'log': str(attempt / 'employment-snapshot-refresh.log')}
		check('blank_snapshot_refresh_succeeded', refresh.returncode == 0)
		check('blank_snapshot_default_collation_preserved', sql("SELECT DEFAULT_COLLATION_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" + refresh_schema + "'") == 'utf8mb4_0900_ai_ci')
		verify_identity()
		manifest = json.loads(manifest_path.read_text(encoding='utf-8'))
		latest_migration = manifest.get('LatestMigrationId')
		check('refreshed_manifest_has_latest_migration', isinstance(latest_migration, str) and bool(latest_migration), str(latest_migration))

		verify_identity()
		sql('CREATE DATABASE `' + import_schema + '`')
		snapshot_sql = snapshot_path.read_text(encoding='utf-8-sig')
		check('snapshot_contains_database_placeholder', snapshot_sql.count('__FUTUREMUD_DATABASE__') >= 1)
		snapshot_sql = snapshot_sql.replace('__FUTUREMUD_DATABASE__', import_schema)
		import_started = time.monotonic()
		imported = subprocess.run(client, input=snapshot_sql, capture_output=True, text=True,
			encoding='utf-8', errors='replace', timeout=600, creationflags=flags)
		import_connection_redacted = imported.stdout + imported.stderr
		(attempt / 'employment-snapshot-import.log').write_text(import_connection_redacted, encoding='utf-8')
		receipt['snapshotImport'] = {'schema': import_schema, 'exitCode': imported.returncode,
			'durationSeconds': round(time.monotonic() - import_started, 3), 'log': str(attempt / 'employment-snapshot-import.log')}
		check('refreshed_snapshot_import_succeeded', imported.returncode == 0)
		verify_identity()
		imported_migration = sql('SELECT MigrationId FROM `__efmigrationshistory` ORDER BY MigrationId DESC LIMIT 1', import_schema)
		check('snapshot_import_latest_migration_matches_manifest', imported_migration == latest_migration,
			'imported=' + imported_migration + '; manifest=' + str(latest_migration))
		column_count = sql("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='" + import_schema +
			"' AND ((TABLE_NAME='employmentactivetasks' AND COLUMN_NAME='ProvenanceJson') OR " +
			"(TABLE_NAME='employmentscheduledtaskrules' AND COLUMN_NAME='ApprovalPrincipalJson') OR " +
			"(TABLE_NAME='employmentmanagergoals' AND COLUMN_NAME='ApprovalConfigurationJson'))")
		check('employment_approval_provenance_columns_imported', column_count == '3', column_count)

		project = repo / 'MudSharpCore Unit Tests' / 'MudSharpCore Unit Tests.csproj'
		results = attempt / 'employment-test-results'
		results.mkdir(exist_ok=True)
		env = os.environ.copy()
		env['FUTUREMUD_EMPLOYMENT_NATIVE_CONNECTION'] = 'server=127.0.0.1;port={};database={};uid=root;SslMode=Disabled;AllowPublicKeyRetrieval=True;Default Command Timeout=600;'.format(port, instance['database'])
		env['FUTUREMUD_EMPLOYMENT_NATIVE_DATADIR'] = str(data_dir)
		env['FUTUREMUD_EMPLOYMENT_NATIVE_UUID'] = expected_uuid
		env['FUTUREMUD_EMPLOYMENT_NATIVE_PORT'] = str(port)
		env['FUTUREMUD_EMPLOYMENT_NATIVE_ARTIFACTS'] = str(attempt)
		for phase in ('write', 'read'):
			started = time.monotonic()
			trx = results / ('employment-' + phase + '.trx')
			phase_env = env.copy()
			phase_env['FUTUREMUD_EMPLOYMENT_NATIVE_PHASE'] = phase
			command = ['dotnet', 'test', str(project), '-c', 'Debug', '--no-build', '--no-restore', '-m:1',
				'-p:IncludeExtendedTests=true', '--settings', str(repo / 'test-extended.runsettings'),
				'--filter', 'FullyQualifiedName~EmploymentRestartNative_', '--logger', 'trx;LogFileName=' + trx.name,
				'--results-directory', str(results)]
			completed = subprocess.run(command, cwd=repo, env=phase_env, capture_output=True,
				text=True, encoding='utf-8', errors='replace', timeout=1200,
				creationflags=subprocess.CREATE_NO_WINDOW if os.name == 'nt' else 0)
			phase_result = {'phase': phase, 'exitCode': completed.returncode,
				'durationSeconds': round(time.monotonic() - started, 3), 'trx': str(trx)}
			receipt['phases'].append(phase_result)
			if completed.returncode or not trx.is_file():
				failure_log = (completed.stdout + completed.stderr).replace(env['FUTUREMUD_EMPLOYMENT_NATIVE_CONNECTION'], '[REDACTED CONNECTION]')
				(attempt / ('employment-' + phase + '.log')).write_text(failure_log, encoding='utf-8')
			if not trx.is_file():
				raise RuntimeError('Phase {} produced no TRX results (exit {}).'.format(phase, completed.returncode))
			ns = {'t': 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'}
			tree = ET.parse(trx)
			summary = tree.find('.//t:ResultSummary/t:Counters', ns)
			if summary is None:
				raise RuntimeError('Phase {} TRX has no result counters.'.format(phase))
			counts = {key: int(summary.get(key, '0')) for key in ('total', 'executed', 'passed', 'failed', 'error', 'timeout', 'aborted', 'inconclusive', 'notExecuted', 'warning')}
			phase_result['counts'] = counts
			if completed.returncode or counts['total'] == 0 or counts['passed'] == 0 or any(
				counts[key] for key in ('failed', 'error', 'timeout', 'aborted', 'inconclusive', 'notExecuted')):
				raise RuntimeError('Phase {} failed, skipped, or ran no employment test; see TRX and phase log.'.format(phase))
		receipt.update(status='PASS', mysqlUuid=expected_uuid, mysqlPort=port,
			database=instance['database'], totalDurationSeconds=round(sum(x['durationSeconds'] for x in receipt['phases']), 3))
		return_code = 0
	except BaseException as error:
		receipt['error'] = str(error)
		return_code = 1
	finally:
		receipt_path.write_text(json.dumps(receipt, indent=2), encoding='utf-8')
		(root / 'employment-restart.json').write_text(json.dumps(receipt, indent=2), encoding='utf-8')
		(root / 'smoke-latest.json').write_text(json.dumps(receipt, indent=2), encoding='utf-8')
	print('{}: {}'.format(receipt['status'], receipt_path))
	return return_code


if __name__ == '__main__':
	raise SystemExit(main())
