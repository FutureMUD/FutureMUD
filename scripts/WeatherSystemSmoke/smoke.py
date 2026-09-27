"""Bounded native assertions. Run only against the owned smoke-instance receipt."""
import argparse, importlib.util, json, pathlib, re, socket, subprocess, sys, time, uuid
import xml.etree.ElementTree as ET
sys.dont_write_bytecode = True
repo = pathlib.Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--run-dir', type=pathlib.Path, required=True)
arguments = parser.parse_args()
root = arguments.run_dir.resolve()
assert root.is_relative_to((repo / '.artifacts/weather').resolve()), 'Run directory must be inside owned weather artifacts.'

instance = json.loads((root / 'mysql-instance.json').read_text())
replay = json.loads((root / 'native-replay.json').read_text())
assert replay['firstRun'] == 'PASS' and replay['secondRun'] == 'REFUSED_WITHOUT_MUTATION'
assert instance['database'] == 'authored_celestial_smoke'
assert pathlib.Path(instance['data']).resolve().is_relative_to(root)
def sql(query):
    identity = subprocess.run(instance['client'] + ['-e', 'SELECT @@datadir'], capture_output=True, text=True, creationflags=subprocess.CREATE_NO_WINDOW, timeout=30)
    assert identity.returncode == 0 and pathlib.Path(identity.stdout.strip()).resolve() == pathlib.Path(instance['data']).resolve()
    answer = subprocess.run(instance['client'] + [instance['database'], '-e', query], capture_output=True, text=True, creationflags=subprocess.CREATE_NO_WINDOW, timeout=30)
    assert answer.returncode == 0, answer.stderr
    return answer.stdout.strip()
sql('SELECT 1')
connection = f"server=127.0.0.1;port={instance['port']};database=authored_celestial_smoke;uid=root;SslMode=None;AllowPublicKeyRetrieval=True;Default Command Timeout=600;"
password = re.search(r'const string DebugPassword\s*=\s*"([^"]+)"', (repo / 'DatabaseSeeder/DebugSeederReplay.cs').read_text(encoding='utf-8-sig')).group(1)
spec = importlib.util.spec_from_file_location('mud_session', repo / '.agents/skills/futuremud-mud-tester/scripts/mud_session.py')
helper = importlib.util.module_from_spec(spec); spec.loader.exec_module(helper)
tag = uuid.uuid4().hex[:8]
runtime = root / ('smoke-' + tag); runtime.mkdir()
report = {'status': 'RUNNING', 'run': tag, 'assertions': [], 'objects': {}, 'replay': 'native-replay.json'}
process = None; session = None; transcripts = []
deadline = time.monotonic() + 900
def check(label, condition, detail=''):
    report['assertions'].append({'name': label, 'passed': bool(condition), 'detail': detail})
    assert condition, label + ': ' + detail
def command(text, expected=None, forbidden=None, seconds=.5):
    assert time.monotonic() < deadline, '900 second scenario deadline exceeded'
    answer = session.send(text, read_seconds=seconds)
    response_deadline = time.monotonic() + 8
    while expected and not re.search(expected, answer, re.I | re.S) and time.monotonic() < response_deadline:
        answer += session.read_for(.3)
    if expected: check(text, re.search(expected, answer, re.I | re.S) is not None, expected)
    if forbidden: check(text + ' suppressed', re.search(forbidden, answer, re.I | re.S) is None, forbidden)
    return answer
def start():
    global process, session
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0)); port = listener.getsockname()[1]
    (runtime / 'Connection.config').write_text(f'127.0.0.1\n{port}\n\n', encoding='utf-8')
    process = helper.launch_mud(repo, runtime, argparse.Namespace(provider='MySql.Data.MySqlClient', startup_timeout=180), connection, [connection, password])
    session = helper.MudSocket('127.0.0.1', port, [connection, password])
    session.read_for(.5)
    command('l'); command('Admin'); session.send(password, secret=True, read_seconds=.4)
    command('c'); command('1', 'Guest Lounge', seconds=2)
    command('impdebug freezetime', 'freeze all')
def stop(graceful=False):
    global session, process
    if session:
        if graceful:
            command('impdebug flush', seconds=1)
            command('shutdown stop', seconds=2)
            try: process.wait(timeout=30)
            except subprocess.TimeoutExpired: check('graceful shutdown', False, 'did not stop within 30 seconds')
            check('graceful shutdown', process.returncode == 0, str(process.returncode))
        transcripts.append(session.full_transcript()); session.close(); session = None
    if process: helper.stop_process_tree(process); process = None

def outdoors(kind, suffix):
    package = command(f'cell package new WeatherQA{tag}{suffix}', 'open overlay package')
    package_id = int(re.search(r'Package #(\d+)', package).group(1))
    command(f'cell set type {kind}', 'now ' + kind)
    command('cell package submit', 'submit the Cell Overlay Package')
    command(f'cell package review {package_id}', 'To approve')
    command('accept edit Weather smoke', 'approve')
    command(f'cell package swap {package_id}', 'Swapped Cell Overlay Package')
    return package_id


def reading():
    data = sql(f'SELECT EffectData FROM Characters WHERE Id={character}')
    effect = ET.fromstring(data).find(".//Effect[@Controller='" + str(controller) + "']")
    check('saved daily reading exists in MySQL', effect is not None)
    return ET.tostring(effect, encoding='unicode')


try:
    character = int(sql("SELECT Id FROM Characters WHERE AccountId=(SELECT Id FROM Accounts WHERE Name='Admin') ORDER BY Id LIMIT 1"))
    zone = int(sql(f'SELECT r.ZoneId FROM Characters ch JOIN Cells c ON c.Id=ch.Location JOIN Rooms r ON r.Id=c.RoomId WHERE ch.Id={character}'))
    climate = int(sql('SELECT Id FROM RegionalClimates ORDER BY Id LIMIT 1'))
    start()
    outdoor_package = outdoors('outdoors', 'Outdoor')
    name = 'WeatherQA' + tag
    command(f'wc edit new {name} {climate} {zone}', 'weather controller|editing')
    controller = int(sql(f"SELECT Id FROM WeatherControllers WHERE Name='{name}'"))
    report['objects']['controller'] = controller
    command(f'zone set {zone} weather {name}', 'weather controller')
    command('wc set forecast 7', 'forecasts 7')
    model = int(sql(f'SELECT ClimateModelId FROM RegionalClimates WHERE Id={climate}'))
    command(f'clm edit {model}', 'editing')
    command('clm set ticks 1', 'minute')
    command('clock edit 1')
    command('clock set time 09:00:00', 'now set')
    skill = command(f'skill add {character} Meteorology 75', 'Meteorology')
    if 'already has' in skill:
        command(f'skill level {character} Meteorology 75', 'Meteorology')
    command('weather forecast', 'Weather outlook')
    command('impdebug flush', seconds=1)
    first_reading = reading()
    command('weather forecast table', 'Expected Conditions')
    command('impdebug flush', seconds=1)
    check('narrative and table share persisted reading', reading() == first_reading)
    event = int(sql(f'SELECT CurrentWeatherEventId FROM WeatherControllers WHERE Id={controller}'))
    report['objects']['event'] = event
    command(f'weatherevent edit {event}', 'editing')
    dust = int(sql("SELECT Id FROM Gases WHERE Name='dusty air'"))
    choking = int(sql("SELECT Id FROM Gases WHERE Name='choking dust'"))
    command(f'weatherevent set atmosphere {dust}', 'hazard settings updated')
    command('survey', 'atmosphere here consists of dusty air')
    command(f'weatherevent set atmosphere {choking}', 'hazard settings updated')
    command('survey', 'atmosphere here consists of choking dust')
    command('weatherevent set atmosphere none', 'hazard settings updated')
    command('survey', 'atmosphere here consists of', 'consists of dusty air|consists of choking dust')
    command('weatherevent set lightning chance 1', 'hazard settings updated')
    command('weatherevent set lightning atmospheric 0', 'hazard settings updated')
    command('weatherevent set lightning targets 0 1 0', 'hazard settings updated')
    command('weatherevent set lightning damage 12 5 5', 'hazard settings updated')
    command('weatherevent set lightning flash Weather smoke flash.', 'hazard settings updated')
    command('weatherevent set lightning thunder Weather smoke thunder.', 'hazard settings updated')
    command('clock set time 09:00:59', 'now set')
    live = command('impdebug unfreezetime', 'unfreeze', seconds=3)
    live += command('impdebug freezetime', 'freeze')
    check('live lightning flash', 'Weather smoke flash.' in live, live)
    check('live lightning thunder', 'Weather smoke thunder.' in live, live)
    check('live direct strike', 'Lightning strikes you!' in live, live)
    command('weatherevent set lightning chance 0', 'hazard settings updated')
    command('impdebug flush', seconds=1)
    wounds = sql(f'SELECT COUNT(*) FROM Wounds WHERE BodyId=(SELECT BodyId FROM Characters WHERE Id={character}) AND DamageType=11 AND OriginalDamage>0')
    check('electrical wound persisted in MySQL', int(wounds) > 0, wounds)
    check('hazard and clock changes do not reroll issued reading', reading() == first_reading)
    rain = int(sql("SELECT Id FROM WeatherEvents WHERE WeatherEventType='rain' AND Name LIKE 'WeatherHazard_Lightning_%' ORDER BY Id LIMIT 1"))
    clonename = 'WeatherRainClone' + tag
    command(f'weatherevent clone {rain} {clonename}', 'clone|editing')
    command('impdebug flush', seconds=1)
    clone = sql(f"SELECT WeatherEventType,AdditionalInfo FROM WeatherEvents WHERE Name='{clonename}'")
    check('rain clone retains rain type, liquid and hazards', clone.startswith('rain\t') and '<Liquid>' in clone and '<Lightning>' in clone, clone[:150])
    indoors_package = outdoors('indoors', 'Indoor')
    command('weather forecast', 'Weather outlook')
    command('clock set time 09:01:59', 'now set')
    command('impdebug unfreezetime', 'unfreeze', seconds=3)
    command('impdebug freezetime', 'freeze')
    command('wc show', 'Forecast')
    stop(graceful=True)
    before = json.loads(sql(f'SELECT ForecastState FROM WeatherControllers WHERE Id={controller}'))
    check('native schedule persisted', len(before['Points']) > 0)
    before_reading = reading()
    start()
    command('weather forecast table', 'Weather outlook.*Expected Conditions')
    check('restart preserves remembered reading', reading() == before_reading)
    command(f'weatherevent show {clonename}', 'Rain Weather Event.*Rain Liquid.*Lightning per minute')
    command(f'wc show {name}', 'Forecast')
    stop(graceful=True)
    after = json.loads(sql(f'SELECT ForecastState FROM WeatherControllers WHERE Id={controller}'))
    check('restart preserves future scheduled checkpoints', before['Points'] == after['Points'])
    check('restart preserves private RNG state', before['RandomState'] == after['RandomState'])
    report['status'] = 'PASS'
except BaseException as error:
    report['status'] = 'FAIL'; report['error'] = helper.redact(str(error), [connection, password])
    raise
finally:
    stop()
    (runtime / 'transcript.txt').write_text('\n\n=== NEXT SESSION ===\n\n'.join(transcripts), encoding='utf-8')
    report['transcript'] = str((runtime / 'transcript.txt').relative_to(root))
    (runtime / 'receipt.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    (root / 'smoke-latest.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
