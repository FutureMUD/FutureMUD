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
    parser.add_argument('--reuse-owned', type=pathlib.Path,
                        help='Resume a previously provisioned, still-running instance with a passing replay receipt.')
    parser.add_argument('--no-build', action='store_true', help='Use already built Debug engine and harness binaries.')
    args = parser.parse_args()
    if os.name != 'nt':
        parser.error('This provisioning wrapper requires Windows/MySQL 8. The numeric tests are portable.')
    base = (repo / '.artifacts/authored-celestials').resolve()
    root = args.reuse_owned.resolve() if args.reuse_owned else base / ('run-' + uuid.uuid4().hex[:12])
    if not root.is_relative_to(base):
        parser.error('Owned run directories must be inside .artifacts/authored-celestials.')
    root.mkdir(parents=True, exist_ok=bool(args.reuse_owned))
    flags = subprocess.CREATE_NO_WINDOW
    instance = None
    process = None
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

    try:
        if args.reuse_owned:
            instance = json.loads((root / 'mysql-instance.json').read_text())
            identity()
            receipt = json.loads((root / 'native-replay.json').read_text())
            if receipt['firstRun'] != 'PASS' or receipt['secondRun'] != 'REFUSED_WITHOUT_MUTATION':
                raise RuntimeError('Reuse requires a successful original replay and nonblank refusal.')
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
            with (mysql_dir / 'process.log').open('w', encoding='utf-8') as output:
                process = subprocess.Popen([str(binary / 'mysqld.exe'), '--no-defaults',
                    '--basedir=' + str(binary.parent), '--datadir=' + str(data), '--port=' + str(port),
                    '--bind-address=127.0.0.1', '--mysqlx=0', '--character-set-server=utf8mb4',
                    '--collation-server=utf8mb4_unicode_ci', '--max-allowed-packet=64M',
                    '--log-error=' + str(mysql_dir / 'mysql.err')], stdout=output,
                    stderr=subprocess.STDOUT, creationflags=flags)
            client = [str(binary / 'mysql.exe'), '--no-defaults', '--protocol=TCP',
                      '--host=127.0.0.1', '--port=' + str(port), '--user=root', '--batch', '--skip-column-names']
            instance = dict(port=port, pid=process.pid, data=str(data), database='authored_celestial_smoke',
                            run=str(mysql_dir), client=client)
            (root / 'mysql-instance.json').write_text(json.dumps(instance, indent=2), encoding='utf-8')
            for _ in range(120):
                if process.poll() is not None:
                    raise RuntimeError('Owned MySQL exited; inspect mysql/mysql.err.')
                probe = run(client + ['-e', 'SELECT 1'], capture_output=True, text=True, timeout=5)
                if probe.returncode == 0:
                    break
                time.sleep(.5)
            else:
                raise RuntimeError('Owned MySQL startup exceeded 60 seconds.')
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

        if not args.reuse_owned:
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
        with (root / 'smoke.log').open('w', encoding='utf-8') as output:
            smoke = run([sys.executable, '-B', '-u', str(pathlib.Path(__file__).with_name('smoke.py')),
                         '--run-dir', str(root)], timeout=960, env=smoke_env, stdout=output, stderr=subprocess.STDOUT)
        result_code = smoke.returncode
        print(('PASS' if result_code == 0 else 'FAIL') + ': ' + str(root / 'smoke-latest.json'))
    finally:
        if instance or process:
            stop_owned_mysql(instance, process, identity, run, root)
    return result_code


if __name__ == '__main__':
    raise SystemExit(main())
