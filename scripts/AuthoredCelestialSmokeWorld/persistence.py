"""Upgrade a stopped, owned native-test world, refresh the blank snapshot, and prove dense persistence."""
from __future__ import annotations

import argparse
import json
import os
import pathlib
import socket
import subprocess
import sys
import time
import uuid

sys.path.insert(0, str(pathlib.Path(__file__).resolve().parent))
from native import stop_owned_mysql


def main():
    repo = pathlib.Path(__file__).resolve().parents[2]
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--run-dir', required=True, type=pathlib.Path,
                        help='Stopped owned native.py world with the original TEXT schema.')
    parser.add_argument('--backup', type=pathlib.Path,
                        help='Restore an earlier owned-world backup into a NEW isolated instance before proving the upgrade.')
    args = parser.parse_args()
    base = (repo / '.artifacts/authored-celestials').resolve()
    source = args.run_dir.resolve()
    if not source.is_relative_to(base):
        parser.error('The source world must be inside this worktree\'s authored celestial artifacts.')
    previous = json.loads((source / 'mysql-instance.json').read_text())
    cleanup = json.loads((source / 'cleanup.json').read_text())
    if previous['database'] != 'authored_celestial_smoke' or not cleanup['mysqlStopped']:
        parser.error('Only a previously stopped owned test world may be upgraded.')
    data = pathlib.Path(previous['data']).resolve()
    if not data.is_relative_to(source) or not data.is_dir():
        parser.error('Data must be an existing directory within the owned run.')
    root = base / ('persistence-' + uuid.uuid4().hex[:12])
    root.mkdir()
    binary = pathlib.Path(previous['client'][0]).parent
    flags = subprocess.CREATE_NO_WINDOW
    backup = args.backup.resolve() if args.backup else None
    if backup and (not backup.is_relative_to(base) or not backup.is_file()):
        parser.error('A backup must be an existing file inside this worktree\'s authored celestial artifacts.')
    if backup:
        data = root / 'mysql/data'
        data.mkdir(parents=True)
    process = None
    instance = None
    result = {'status': 'FAIL', 'sourceWorld': source.name}

    def run(command, *, timeout=180, **kwargs):
        return subprocess.run(command, cwd=repo, creationflags=flags, timeout=timeout, **kwargs)

    def query(statement):
        reply = run(instance['client'] + ['-e', statement], capture_output=True, text=True, timeout=30)
        if reply.returncode:
            raise RuntimeError('Owned database query failed: ' + reply.stderr)
        return reply.stdout.strip()

    def identity():
        if pathlib.Path(query('SELECT @@datadir')).resolve() != data:
            raise RuntimeError('Owned MySQL data directory mismatch.')

    def start():
        nonlocal instance, process
        with socket.socket() as listener:
            listener.bind(('127.0.0.1', 0))
            port = listener.getsockname()[1]
        with (root / 'mysql-process.log').open('a', encoding='utf-8') as output:
            process = subprocess.Popen([str(binary / 'mysqld.exe'), '--no-defaults',
                '--basedir=' + str(binary.parent), '--datadir=' + str(data), '--port=' + str(port),
                '--bind-address=127.0.0.1', '--mysqlx=0', '--max-allowed-packet=64M',
                '--log-error=' + str(root / 'mysql.err')], stdout=output, stderr=subprocess.STDOUT, creationflags=flags)
        client = [str(binary / 'mysql.exe'), '--no-defaults', '--protocol=TCP', '--host=127.0.0.1',
                  '--port=' + str(port), '--user=root', '--batch', '--skip-column-names']
        instance = dict(port=port, pid=process.pid, data=str(data), database='authored_celestial_smoke', client=client)
        (root / 'mysql-instance.json').write_text(json.dumps(instance, indent=2))
        for _ in range(120):
            if process.poll() is not None:
                raise RuntimeError('Owned MySQL exited before readiness.')
            probe = run(client + ['-e', 'SELECT 1'], capture_output=True, text=True, timeout=5)
            if probe.returncode == 0:
                identity()
                return
            time.sleep(.5)
        raise RuntimeError('MySQL startup exceeded 60 seconds.')

    def connection(database):
        return f"server=127.0.0.1;port={instance['port']};database={database};uid=root;SslMode=None;AllowPublicKeyRetrieval=True;Default Command Timeout=600;"

    def execute(label, command, env):
        print(label, flush=True)
        with (root / (label + '.log')).open('w', encoding='utf-8') as output:
            completed = run(command, env=env, stdout=output, stderr=subprocess.STDOUT, timeout=900)
        if completed.returncode:
            raise RuntimeError(label + ' failed; inspect its log.')

    try:
        if backup:
            initialised = run([str(binary / 'mysqld.exe'), '--no-defaults', '--initialize-insecure',
                '--basedir=' + str(binary.parent), '--datadir=' + str(data),
                '--log-error=' + str(root / 'mysql.err')], capture_output=True, text=True)
            if initialised.returncode:
                raise RuntimeError('Fresh owned MySQL initialization failed; inspect mysql.err.')
        start()
        if backup:
            # MySqlBackup.NET includes an unconditional DROP DATABASE in its dump.
            # This instance is newly initialized, so create its empty target first.
            query('CREATE DATABASE authored_celestial_smoke')
            imported = run(instance['client'], input=backup.read_text(encoding='utf-8-sig'),
                           capture_output=True, text=True, encoding='utf-8', timeout=900)
            (root / 'backup-import.log').write_text(imported.stdout + imported.stderr, encoding='utf-8')
            if imported.returncode:
                raise RuntimeError('Backup import into fresh owned instance failed.')
            result['restoredBackup'] = str(backup.relative_to(base))
        harness = ['dotnet', str(repo / 'scripts/AuthoredCelestialSmokeWorld/bin/Debug/net10.0/AuthoredCelestialSmokeWorld.dll')]
        receipt = str(root / 'upgrade.json')
        env = os.environ.copy()
        env['AUTHORED_SMOKE_CONNECTION'] = connection('authored_celestial_smoke')
        execute('upgrade', harness + [str(data), receipt, '--persistence-upgrade'], env)
        stop_owned_mysql(instance, process, identity, run, root)
        process = None
        instance = None
        start()
        env['AUTHORED_SMOKE_CONNECTION'] = connection('authored_celestial_smoke')
        execute('reload', harness + [str(data), receipt, '--persistence-reload'], env)
        identity()
        snapshot_database = 'authored_celestial_snapshot_' + uuid.uuid4().hex[:10]
        env['FUTUREMUD_SNAPSHOT_CONNECTION_STRING'] = connection(snapshot_database)
        # Keep snapshot temporary operations isolated from stale shared TEMP directory ACLs.
        temp = root / 'temp'
        temp.mkdir()
        env['TEMP'] = env['TMP'] = str(temp)
        execute('snapshot-refresh', ['dotnet', 'run', '--project', 'DatabaseSeeder/DatabaseSeeder.csproj',
                                   '-c', 'Debug', '--no-build', '--', '--refresh-blank-snapshot'], env)
        identity()
        query('CREATE DATABASE authored_celestial_snapshot_import')
        env['AUTHORED_SMOKE_CONNECTION'] = connection('authored_celestial_snapshot_import')
        execute('snapshot-import', harness + [str(data), str(root / 'snapshot-import.json'),
                '--snapshot-import-verify', str(repo / 'DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.sql')], env)
        result.update(status='PASS', upgrade='upgrade.json', snapshotImport='snapshot-import.json')
    except Exception as error:
        result['error'] = str(error)
        raise
    finally:
        if instance or process:
            stop_owned_mysql(instance, process, identity, run, root)
        (root / 'verification.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
        print(result['status'] + ': ' + str(root / 'verification.json'), flush=True)
    return 0


if __name__ == '__main__':
    sys.exit(main())
