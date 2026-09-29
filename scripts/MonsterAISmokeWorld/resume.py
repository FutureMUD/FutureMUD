"""Resume only a stopped, previously owned Monster smoke database, preserving failed receipts."""
import argparse
import importlib.util
import json
import pathlib
import socket
import subprocess
import sys
import time

repo = pathlib.Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--run-dir', type=pathlib.Path, required=True)
args = parser.parse_args()
root = args.run_dir.resolve()
assert root.is_relative_to((repo / '.artifacts/authored-celestials').resolve())
instance = json.loads((root / 'mysql-instance.json').read_text())
data = pathlib.Path(instance['data']).resolve()
assert data.is_relative_to(root) and instance['database'] == 'authored_celestial_smoke'
replay = json.loads((root / 'native-replay.json').read_text())
assert replay['firstRun'] == 'PASS' and replay['secondRun'] == 'REFUSED_WITHOUT_MUTATION'
with socket.socket() as probe:
    assert probe.connect_ex(('127.0.0.1', instance['port'])) != 0, 'Owned port is occupied; refusing a second operator'
binary = pathlib.Path(instance['client'][0]).parent
process = None
spec = importlib.util.spec_from_file_location('native', repo / 'scripts/AuthoredCelestialSmokeWorld/native.py')
native = importlib.util.module_from_spec(spec); spec.loader.exec_module(native)


def run(command, **kwargs):
    return subprocess.run(command, cwd=repo, creationflags=subprocess.CREATE_NO_WINDOW, **kwargs)


def identity():
    result = run(instance['client'] + ['-e', 'SELECT @@datadir'], capture_output=True, text=True, timeout=10)
    assert result.returncode == 0 and pathlib.Path(result.stdout.strip()).resolve() == data, 'Owned MySQL identity mismatch'


try:
    with (root / 'mysql/resume-process.log').open('a') as output:
        process = subprocess.Popen([str(binary / 'mysqld.exe'), '--no-defaults', '--basedir=' + str(binary.parent),
            '--datadir=' + str(data), '--port=' + str(instance['port']), '--bind-address=127.0.0.1', '--mysqlx=0',
            '--character-set-server=utf8mb4', '--collation-server=utf8mb4_unicode_ci', '--max-allowed-packet=64M',
            '--log-error=' + str(root / 'mysql/mysql.err')], stdout=output, stderr=subprocess.STDOUT,
            creationflags=subprocess.CREATE_NO_WINDOW)
    for _ in range(120):
        assert process.poll() is None, 'Owned MySQL exited'
        try:
            identity(); break
        except (AssertionError, subprocess.TimeoutExpired): time.sleep(.5)
    else: raise RuntimeError('Owned MySQL startup deadline exceeded')
    instance['pid'] = process.pid
    (root / 'mysql-instance.json').write_text(json.dumps(instance, indent=2))
    result = run([sys.executable, '-B', '-u', str(pathlib.Path(__file__).with_name('smoke.py')), '--run-dir', str(root)], timeout=960)
    sys.exit(result.returncode)
finally:
    native.stop_owned_mysql(instance, process, identity, run, root)
