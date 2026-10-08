"""Create an isolated Windows MySQL world, replay seeders, and run the Telnet smoke.

No existing service, database, login configuration, or user credential is used.
The new loopback-only instance is stopped on completion; its data and evidence remain.
"""
from __future__ import annotations

import argparse
import json
import os
import pathlib
import re
import socket
import subprocess
import sys
import time
import uuid


def begin_smoke_invocation(root, marker_name):
    """Retire earlier proof before any restart, build or replay can fail."""
    invocation = root / ('smoke-invocation-' + uuid.uuid4().hex[:12])
    invocation.mkdir()
    marker = root / marker_name if marker_name else None
    if marker and marker.exists():
        marker.replace(invocation / 'prior-phase-marker.json')
    latest = root / 'smoke-latest.json'
    if latest.exists():
        latest.replace(invocation / 'prior-receipt.json')
    return invocation, marker, latest


def stop_owned_mysql(instance, process, identity, run, root):
    """Only a newly spawned process handle permits fallback termination after a failed identity query."""
    receipt = {'mysqlStopped': False, 'dataPreserved': instance['data'] if instance else None}
    try:
        if instance:
            identity()
            admin = list(instance['client'])
            admin[0] = str(pathlib.Path(admin[0]).with_name('mysqladmin.exe'))
            admin = [x for x in admin if x not in ('--batch', '--skip-column-names')]
            stopped = run(admin + ['shutdown'], capture_output=True, text=True, timeout=30)
            if stopped.returncode:
                raise RuntimeError('Owned MySQL shutdown command failed.')
        elif process and process.poll() is None:
            process.terminate()
        if process:
            process.wait(timeout=30)
        receipt['mysqlStopped'] = True
    except Exception as error:
        receipt['error'] = str(error)
        if process is None:
            raise RuntimeError('Cannot verify the resumed instance; no process was terminated.') from error
        if process.poll() is None:
            process.terminate()
            try:
                process.wait(timeout=30)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait(timeout=30)
        receipt['mysqlStopped'] = True
        receipt['ownedProcessFallback'] = True
    finally:
        (root / 'cleanup.json').write_text(json.dumps(receipt, indent=2), encoding='utf-8')


def main() -> int:
    repo = pathlib.Path(__file__).resolve().parents[2]
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--mysql-bin', type=pathlib.Path,
                        default=pathlib.Path(r'C:\Program Files\MySQL\MySQL Server 8.0\bin'))
    resume = parser.add_mutually_exclusive_group()
    resume.add_argument('--reuse-owned', type=pathlib.Path,
                        help='Resume a previously provisioned, still-running instance with a passing replay receipt.')
    resume.add_argument('--restart-owned', type=pathlib.Path,
                        help='Restart a stopped owned instance with a passing replay and verified prior cleanup.')
    parser.add_argument('--no-build', action='store_true', help='Use already built Debug engine and harness binaries.')
    parser.add_argument('--smoke-script', type=pathlib.Path,
                        help='Repository-owned alternate scenario to run against this same disposable seeded world.')
    parser.add_argument('--qualification-marker',
                        help='Owned JSON phase marker, promoted only after fresh PASS and verified cleanup.')
    args = parser.parse_args()
    if args.qualification_marker and not re.fullmatch(r'[a-z0-9][a-z0-9._-]*\.json', args.qualification_marker):
        parser.error('Qualification marker must be a simple lowercase JSON filename.')
    smoke_script = args.smoke_script.resolve() if args.smoke_script else pathlib.Path(__file__).with_name('smoke.py')
    if not smoke_script.is_file() or not smoke_script.is_relative_to(repo / 'scripts'):
        parser.error('Smoke script must be an existing file beneath this checkout\'s scripts directory.')
    if os.name != 'nt':
        parser.error('This provisioning wrapper requires Windows/MySQL 8. The numeric tests are portable.')
    base = (repo / '.artifacts/authored-celestials').resolve()
    resume_root = args.reuse_owned or args.restart_owned
    root = resume_root.resolve() if resume_root else base / ('run-' + uuid.uuid4().hex[:12])
    if not root.is_relative_to(base):
        parser.error('Owned run directories must be inside .artifacts/authored-celestials.')
    root.mkdir(parents=True, exist_ok=bool(resume_root))
    invocation_root, marker, latest_receipt = begin_smoke_invocation(root, args.qualification_marker)
    flags = subprocess.CREATE_NO_WINDOW
    instance = None
    process = None
    cleanup_needed = False
    result_code = 1

    def run(command, *, timeout=180, **kwargs):
        return subprocess.run(command, cwd=repo, creationflags=flags, timeout=timeout, **kwargs)

    def query(text):
        result = run(instance['client'] + ['-e', text], capture_output=True, text=True)
        if result.returncode:
            raise RuntimeError('Owned MySQL query failed: ' + result.stderr)
        return result.stdout.strip()

    def identity():
        if instance['database'] != 'authored_celestial_smoke':
            raise RuntimeError('Unexpected database identity.')
        data = pathlib.Path(instance['data']).resolve()
        if not data.is_relative_to(root) or pathlib.Path(query('SELECT @@datadir')).resolve() != data:
            raise RuntimeError('Data directory mismatch; refusing mutation.')
        if query('SELECT @@server_uuid').strip().lower() != expected_uuid:
            raise RuntimeError('Server UUID mismatch; refusing mutation.')

    def wait_ready():
        startup_deadline = time.monotonic() + 60
        while time.monotonic() < startup_deadline:
            if process is not None and process.poll() is not None:
                raise RuntimeError('Owned MySQL exited; inspect mysql/mysql.err.')
            try:
                probe = run(instance['client'] + ['-e', 'SELECT 1'], capture_output=True, text=True,
                            timeout=max(.01, min(5, startup_deadline - time.monotonic())))
            except subprocess.TimeoutExpired:
                continue
            if probe.returncode == 0:
                return
            time.sleep(min(.5, max(0, startup_deadline - time.monotonic())))
        raise RuntimeError('Owned MySQL startup exceeded 60 seconds.')

    def read_uuid(data):
        match = re.search(r'^server-uuid\s*=\s*([0-9a-fA-F-]{36})\s*$',
                          (data / 'auto.cnf').read_text(encoding='utf-8-sig'), re.M)
        if not match:
            raise RuntimeError('Owned MySQL data directory has no server UUID.')
        return str(uuid.UUID(match.group(1)))

    def launch_owned(binary, data, port):
        with (root / 'mysql/process.log').open('a', encoding='utf-8') as output:
            return subprocess.Popen([str(binary / 'mysqld.exe'), '--no-defaults',
                '--basedir=' + str(binary.parent), '--datadir=' + str(data), '--port=' + str(port),
                '--bind-address=127.0.0.1', '--mysqlx=0', '--character-set-server=utf8mb4',
                '--collation-server=utf8mb4_unicode_ci', '--max-allowed-packet=64M',
                '--log-error=' + str(root / 'mysql/mysql.err')], stdout=output,
                stderr=subprocess.STDOUT, creationflags=flags)

    try:
        if resume_root:
            instance = json.loads((root / 'mysql-instance.json').read_text())
            data = pathlib.Path(instance['data']).resolve()
            if (not data.is_relative_to(root) or data != (root / 'mysql/data').resolve() or
                    instance['database'] != 'authored_celestial_smoke' or
                    type(instance['port']) is not int or not 1 <= instance['port'] <= 65535):
                raise RuntimeError('Unexpected owned data directory or database; refusing restart.')
            expected_uuid = read_uuid(data)
            if instance.get('server_uuid', expected_uuid) != expected_uuid:
                raise RuntimeError('Owned data directory UUID changed since provisioning.')
            binary = args.mysql_bin.resolve()
            # Persisted metadata identifies an endpoint, never executable command text.
            instance['client'] = [str(binary / 'mysql.exe'), '--no-defaults', '--protocol=TCP',
                '--host=127.0.0.1', '--port=' + str(instance['port']), '--user=root', '--ssl-mode=DISABLED',
                '--connect-timeout=2', '--batch', '--skip-column-names']
            receipt = json.loads((root / 'native-replay.json').read_text())
            if (receipt['firstRun'] != 'PASS' or receipt['secondRun'] != 'REFUSED_WITHOUT_MUTATION' or
                    receipt['digestBefore'] != receipt['digestAfter']):
                raise RuntimeError('Reuse requires a successful original replay and nonblank refusal.')
            if args.restart_owned:
                cleanup = json.loads((root / 'cleanup.json').read_text())
                if not cleanup.get('mysqlStopped') or pathlib.Path(cleanup['dataPreserved']).resolve() != data:
                    raise RuntimeError('Restart requires verified prior owned-instance cleanup.')
                with socket.socket() as listener:
                    listener.bind(('127.0.0.1', instance['port']))
                process = launch_owned(binary, data, instance['port'])
                cleanup_needed = True
                instance['pid'] = process.pid
                (root / 'mysql-instance.json').write_text(json.dumps(instance, indent=2), encoding='utf-8')
                wait_ready()
            identity()
            cleanup_needed = True
        else:
            mysql_dir = root / 'mysql'
            data = mysql_dir / 'data'
            data.mkdir(parents=True)
            binary = args.mysql_bin.resolve()
            with socket.socket() as listener:
                listener.bind(('127.0.0.1', 0))
                port = listener.getsockname()[1]
            init = run([str(binary / 'mysqld.exe'), '--no-defaults', '--initialize-insecure',
                        '--basedir=' + str(binary.parent), '--datadir=' + str(data),
                        '--log-error=' + str(mysql_dir / 'mysql.err')], capture_output=True, text=True)
            if init.returncode:
                raise RuntimeError('Isolated MySQL initialization failed: ' + init.stderr)
            expected_uuid = read_uuid(data)
            process = launch_owned(binary, data, port)
            cleanup_needed = True
            client = [str(binary / 'mysql.exe'), '--no-defaults', '--protocol=TCP',
                      '--host=127.0.0.1', '--port=' + str(port), '--user=root', '--ssl-mode=DISABLED',
                      '--connect-timeout=2', '--batch', '--skip-column-names']
            instance = dict(port=port, pid=process.pid, data=str(data), database='authored_celestial_smoke',
                            run=str(mysql_dir), client=client, server_uuid=expected_uuid)
            (root / 'mysql-instance.json').write_text(json.dumps(instance, indent=2), encoding='utf-8')
            wait_ready()
            identity()
            if query("SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME='authored_celestial_smoke'") != '0':
                raise RuntimeError('Refusing nonblank target; snapshot import is first-run only.')
            query('CREATE DATABASE authored_celestial_smoke')
            snapshot = (repo / 'DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.sql').read_text(encoding='utf-8-sig')
            snapshot = snapshot.replace('__FUTUREMUD_DATABASE__', instance['database'])
            imported = run(client, input=snapshot, capture_output=True, text=True, encoding='utf-8')
            (root / 'snapshot-import.log').write_text(imported.stdout + imported.stderr, encoding='utf-8')
            if imported.returncode:
                raise RuntimeError('Blank snapshot import failed; inspect snapshot-import.log.')

        if not args.no_build:
            for label, project in [('engine', 'MudSharpCore/MudSharpCore.csproj'),
                                   ('harness', 'scripts/AuthoredCelestialSmokeWorld/AuthoredCelestialSmokeWorld.csproj')]:
                with (root / (label + '-build.log')).open('w', encoding='utf-8') as output:
                    built = run(['dotnet', 'build', project, '-c', 'Debug', '-m:1',
                                 '-p:RestoreBuildInParallel=false', '-p:NuGetAudit=false'],
                                timeout=600, stdout=output, stderr=subprocess.STDOUT)
                if built.returncode:
                    raise RuntimeError(label + ' build failed; inspect its build log.')

        if not resume_root:
            identity()
            connection = f"server=127.0.0.1;port={instance['port']};database=authored_celestial_smoke;uid=root;SslMode=None;AllowPublicKeyRetrieval=True;Default Command Timeout=600;"
            source = (repo / 'DatabaseSeeder/DebugSeederReplay.cs').read_text(encoding='utf-8-sig')
            password = re.search(r'const string DebugPassword\s*=\s*"([^"]+)"', source).group(1)
            env = os.environ.copy()
            env['AUTHORED_SMOKE_CONNECTION'] = connection
            env['AUTHORED_SMOKE_PASSWORD'] = password
            with (root / 'native-replay.log').open('w', encoding='utf-8') as output:
                replay = run(['dotnet', str(repo / 'scripts/AuthoredCelestialSmokeWorld/bin/Debug/net10.0/AuthoredCelestialSmokeWorld.dll'),
                              instance['data'], str(root / 'native-replay.json')], env=env, timeout=1800,
                             stdout=output, stderr=subprocess.STDOUT)
            log = (root / 'native-replay.log').read_text(encoding='utf-8')
            (root / 'native-replay.log').write_text(log.replace(password, '[REDACTED]').replace(connection, '[REDACTED]'), encoding='utf-8')
            if replay.returncode:
                raise RuntimeError('Native replay failed; completed seeder commits remain. Inspect native-replay.log.')
        identity()
        smoke_env = os.environ.copy()
        smoke_env['PYTHONIOENCODING'] = 'utf-8'
        with (invocation_root / 'smoke.log').open('w', encoding='utf-8') as output:
            smoke = run([sys.executable, '-B', '-u', str(smoke_script),
                         '--run-dir', str(root)], timeout=960, env=smoke_env, stdout=output, stderr=subprocess.STDOUT)
        (root / 'smoke.log').write_bytes((invocation_root / 'smoke.log').read_bytes())
        result_code = smoke.returncode
        if not latest_receipt.exists():
            result_code = 1
            latest_receipt.write_text(json.dumps(dict(status='FAIL', milestoneQualified=False,
                error='Smoke process exited without a fresh receipt.', exitCode=smoke.returncode,
                log=str(invocation_root / 'smoke.log')), indent=2), encoding='utf-8')
        receipt = json.loads(latest_receipt.read_text(encoding='utf-8'))
        if receipt.get('status') != 'PASS':
            result_code = result_code or 1
        if args.qualification_marker and receipt.get('qualificationMarker') != args.qualification_marker:
            result_code = 1
            receipt['markerError'] = 'The fresh scenario receipt does not match the requested phase marker.'
            latest_receipt.write_text(json.dumps(receipt, indent=2), encoding='utf-8')
        (invocation_root / 'receipt.json').write_bytes(latest_receipt.read_bytes())
    finally:
        if cleanup_needed:
            stop_owned_mysql(instance, process, identity, run, root)
    cleanup = json.loads((root / 'cleanup.json').read_text(encoding='utf-8'))
    (invocation_root / 'cleanup.json').write_text(json.dumps(cleanup, indent=2), encoding='utf-8')
    if not cleanup.get('mysqlStopped') or cleanup.get('error'):
        result_code = 1
    receipt['cleanup'] = cleanup
    receipt['invocationReceipt'] = str(invocation_root / 'receipt.json')
    receipt['runnerStatus'] = 'PASS' if result_code == 0 else 'FAIL'
    latest_receipt.write_text(json.dumps(receipt, indent=2), encoding='utf-8')
    (invocation_root / 'receipt.json').write_bytes(latest_receipt.read_bytes())
    if marker and result_code == 0:
        marker.write_bytes(latest_receipt.read_bytes())
    print(('PASS' if result_code == 0 else 'FAIL') + ': ' + str(latest_receipt))
    return result_code


if __name__ == '__main__':
    raise SystemExit(main())
