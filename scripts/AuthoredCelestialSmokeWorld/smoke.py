"""Bounded native assertions. Run only against the owned smoke-instance receipt."""
import argparse, importlib.util, json, pathlib, re, socket, subprocess, sys, time, uuid
sys.dont_write_bytecode = True
repo = pathlib.Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--run-dir', type=pathlib.Path, required=True)
arguments = parser.parse_args()
root = arguments.run_dir.resolve()
assert root.is_relative_to((repo / '.artifacts/authored-celestials').resolve()), 'Run directory must be inside owned authored-celestial artifacts.'

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
def new(preset, label):
    name = f'QA{tag}{label}'
    output = command(f'celestial new {preset} 1 1 {name}', 'Created.*Attach it')
    identifier = int(re.search(r' #(\d+)', output).group(1))
    report['objects'][label] = {'id': identifier, 'name': name}
    return identifier, name
def set_(setting): command('celestial set ' + setting, 'Draft updated')
def edit(identifier): command(f'celestial edit {identifier}', 'private draft')
def save(): command('celestial validate', 'Valid:'); command('celestial save', 'Saved and silently activated')
def prog(label, expression, parameters, values, return_type='MudDateTime'):
    name = 'qa' + tag + label
    command('prog edit new ' + name)
    command('prog set return ' + return_type, 'return type')
    for pname, ptype in parameters: command(f'prog set parameter add {pname} {ptype}')
    command('prog set text'); command('return ' + expression)
    command('@', 'compiled successfully')
    answer = command('prog execute ' + name + ' ' + values, 'It returned')
    check('FutureProg ' + label, 'never' not in answer.lower() and re.search('error|no such|could not|cannot', answer, re.I) is None, answer.strip())
    return answer
try:
    start()
    command('celestial types', 'RailSun,RailMoon,ScriptedSun,ScriptedMoon')
    package = command(f'cell package new CelestialQA{tag}', 'open overlay package')
    package_id = int(re.search(r'Package #(\d+)', package).group(1))
    command('cell set type outdoors', 'now Outdoors')
    command('cell package submit', 'submit the Cell Overlay Package')
    command(f'cell package review {package_id}', 'To approve')
    command('accept edit Authored celestial smoke', 'approve')
    command(f'cell package swap {package_id}', 'Swapped Cell Overlay Package')
    sun, sunname = new('RailSun', 'Sun')
    set_('longitude 0'); save()
    command('celestial preview 360 3 360', r'Minute 360:.*elevation 0\.000000.*Minute 720:.*elevation 90\.000000.*Minute 1080:.*elevation 0\.000000')
    command('celestial event sunrise 360 1000000', r'anchor minute 1,440,000,360')
    moon, moonname = new('RailMoon', 'Moon')
    set_('milestone custom:crescent 40320 24180'); set_(f'crescent {sun} custom:crescent'); save()
    command('celestial preview 0 2 10080', 'phase Full.*phase LastQuarter')
    command(f'celestial event visiblecrescent 0 1 {sun}', 'anchor minute 24,180')
    scripted, scriptedname = new('ScriptedSun', 'Scripted')
    set_('milestone custom:stir 60 10'); save()
    scriptedmoon, scriptedmoonname = new('ScriptedMoon', 'FixedMoon')
    set_('phase fixed 0'); save()
    command('celestial event fullmoon 0', 'NoFutureOccurrence')
    # moonphase(location) deliberately selects the first attached moon. Use the fixed
    # Full fixture so this public lookup assertion is independent of the world's date.
    command(f'shard set 1 celestials 1 {sun} {scriptedmoon} {moon} {scripted}', 'first.*eligible|time.of.day')
    command(f'shard set 1 celestials {sun} {scriptedmoon} {moon} {scripted}', 'celestial')
    command('clock edit 1')
    command('clock set time 09:00:00', 'now set')
    command('look sky', re.escape(sunname) + r'.*rising.*' + re.escape(scriptedmoonname) + r'.*stationary.*full')
    command('time', 'currently Morning')
    command('look', 'lighting is bright')
    command('look ' + sunname, 'appointed course|horizon|degrees above')
    common = [('loc', 'Location'), ('cal', 'Calendar'), ('sun', 'Number')]
    for function in ['nextsunrise', 'nextsunset', 'nextsolarlongitude']:
        extra = ', 90' if function == 'nextsolarlongitude' else ''
        prog(function, f'{function}(@loc, @sun, @cal{extra})', common, f'here 1 {sun}')
    lunar = [('loc', 'Location'), ('cal', 'Calendar'), ('moon', 'Number')]
    for function in ['nextnewmoon', 'nextfullmoon']:
        prog(function, f'{function}(@loc, @moon, @cal)', lunar, f'here 1 {moon}')
    prog('crescent', 'nextvisiblecrescent(@loc, @sun, @moon, @cal)', common + [('moon', 'Number')], f'here 1 {sun} {moon}')
    prog('named', 'nextcelestialevent(@loc, @sun, @cal, "custom:stir")', common, f'here 1 {scripted}')
    phase_output = prog('phase', 'moonphase(@loc)', [('loc', 'Location')], 'here', 'Text')
    check('public moonphase value', 'It returned Full' in phase_output, phase_output)
    elevation_output = prog('elevation', 'celestialelevation(@loc, @sun)', [('loc', 'Location'), ('sun', 'Number')], f'here {sun}', 'Number')
    check('public elevation radians', 'It returned 0.785' in elevation_output, elevation_output)
    edit(sun)
    exported = command('celestial export', '"Kind": "RailSun"')
    source = json.JSONDecoder().raw_decode(exported[exported.index('{'):])[0]
    command('celestial import', 'Paste a version 1')
    command(json.dumps(source, separators=(',', ':'))); command('@', 'imported|Imported')
    save()
    set_('rail 1440 360 360 90 90 180')
    command('celestial save', 'distinct|different|rise.*set')
    command(f'celestial show {sun}', '"Set": 1080')
    edit(sun)
    cloneout = command(f'celestial clone {sun} QA{tag}Clone', 'Created.*RailSun')
    report['clone'] = int(re.search(r' #(\d+)', cloneout).group(1))
    # A wholly below-horizon object separates SkyVisible and BodyVisible.
    edit(scripted)
    set_('path sparse 2'); set_('key 0 90 -5 Hold'); set_('key 2 90 -5 Hold')
    set_('scheduled sky 2 1 SkyVisible 0 The amber smoke marker crosses the sky')
    set_('scheduled body 2 1 BodyVisible 1 The hidden smoke marker should not appear')
    save()
    command('clock set time 00:00:59', 'now set', 'amber smoke marker|hidden smoke marker')
    command('celestial preview 0 3', 'Minute 0:', 'amber smoke marker|hidden smoke marker')
    command('celestial event custom:stir 0 1000000', 'Found', 'amber smoke marker|hidden smoke marker')
    live = command('impdebug unfreezetime', 'unfreeze', seconds=3)
    live += command('impdebug freezetime', 'freeze')
    check('normal-minute SkyVisible exactly once', live.count('The amber smoke marker crosses the sky.') == 1, live)
    check('below-horizon BodyVisible suppressed', 'hidden smoke marker' not in live, live)
    command('look sky', forbidden='amber smoke marker|hidden smoke marker')
    window_package = command(f'cell package new WindowQA{tag}', 'open overlay package')
    window_id = int(re.search(r'Package #(\d+)', window_package).group(1))
    command('cell set type windows', 'now Indoors')
    command(f'cell overlay {window_id}', 'overlay|package')
    command('clock set time 00:00:59', 'now set', 'amber smoke marker|hidden smoke marker')
    window_live = command('impdebug unfreezetime', 'unfreeze', seconds=3)
    window_live += command('impdebug freezetime', 'freeze')
    check('windows receive the outside prefix', '[Outside] The amber smoke marker crosses the sky.' in window_live, window_live)
    command('cell set type indoors', 'now Indoors')
    command('clock set time 00:00:59', 'now set', 'amber smoke marker|hidden smoke marker')
    sheltered = command('impdebug unfreezetime', 'unfreeze', seconds=3)
    sheltered += command('impdebug freezetime', 'freeze')
    check('shelter suppresses sky and body echoes', 'smoke marker' not in sheltered, sheltered)
    command('look sky', 'cannot see the sky')
    command('cell overlay clear', 'only see the current cell overlay')
    command('celestial preview 0 2', forbidden='amber smoke marker|hidden smoke marker')
    command('clock set time 00:00:59', 'now set', 'amber smoke marker|hidden smoke marker')
    live = command('impdebug unfreezetime', 'unfreeze', seconds=3)
    live += command('impdebug freezetime', 'freeze')
    check('rewind then ordinary crossing emits once', live.count('The amber smoke marker crosses the sky.') == 1, live)
    command('clock set time 09:00:00', 'now set', 'amber smoke marker|hidden smoke marker')
    stop(graceful=True)
    rows = sql('SELECT Id, CelestialType FROM Celestials WHERE Id IN (' + ','.join(str(x['id']) for x in report['objects'].values()) + ')')
    check('four created types persisted in MySQL', all(t in rows for t in ['RailSun', 'RailMoon', 'ScriptedSun', 'ScriptedMoon']), rows)
    start()
    command('celestial list', re.escape(sunname) + '.*' + re.escape(scriptedmoonname))
    command('look sky', re.escape(sunname) + r'.*' + re.escape(scriptedmoonname), 'amber smoke marker|hidden smoke marker')
    edit(scripted)
    command('celestial show', '"Id": "sky".*amber smoke marker')
    command('celestial event custom:stir 0 1000000', 'Found')
    stop(graceful=True)
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
