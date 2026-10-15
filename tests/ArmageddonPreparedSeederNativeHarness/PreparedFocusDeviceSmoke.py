"""One native S4 content sample. Stock availability stays disabled; S5 is not certified."""
import time
from copy import deepcopy
import xml.etree.ElementTree as ET
from pathlib import Path


def run(host, session, phase, data, mapped):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    config = data['focus_devices']
    source = config['source']
    require(Path(source['path']).resolve() == host['REPO'] / 'tests/ArmageddonPreparedSeederNativeHarness/Program.FocusDeviceFixture.cs'
            and host['sha'](source['path']) == source['sha256'], 'Focus/device preparation source pin differs.')
    samples = {x['key']: x for x in config['samples']}
    actor, body, resource = int(data['character']), int(data['body']), int(config['resource'])
    for gate in ('use_gate', 'craft_gate'):
        rows = sql(f'SELECT HEX(FunctionText) FROM FutureProgs WHERE Id={int(config[gate])}', (0,))
        require(rows == [['return false']], 'A stock focus/device availability gate changed.')
    for key in config['keys']:
        claim = host['catalogue_record'](key)
        require(claim[1] == 'GameItemProto', 'Focus/device claim is not a typed item prototype.')
    require(len(config['keys']) == 12, 'Focus/device installed inventory differs from twelve.')

    def flush():
        output = session.send('impdebug flush', read_seconds=.5)
        deadline = time.monotonic() + 6
        while 'All queued saves have been flushed' not in output and time.monotonic() < deadline:
            output += session.read_for(.5)
        require('All queued saves have been flushed' in output, 'Native focus/device save flush failed within its bounded acknowledgement window: ' + host['redact'](output))

    def balance():
        return float(sql(mapped(f'SELECT IFNULL((SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}),0)'))[0][0])

    def fund(cap):
        flush()
        amount = balance()
        if amount < 99:
            session.send(f'{config["invoked"]} gather {cap} draw {100-amount:.4f}', read_seconds=.5)
            deadline = time.monotonic() + 18
            while time.monotonic() < deadline and balance() < 99:
                session.read_for(.5)
            require(balance() >= 99, 'Ordinary paid gathering did not fund the content sample.')

    def items(sample):
        flush()
        return sql(f'SELECT Id FROM GameItems WHERE GameItemProtoId={int(sample["prototype"])} ORDER BY Id')

    def bank(sample, item):
        rows = sql(f'SELECT HEX(Definition) FROM GameItemComponents WHERE GameItemId={int(item)} AND GameItemComponentProtoId={int(sample["component"])}', (0,))
        require(len(rows) == 1, 'Expected exactly one persisted native device component.')
        return ET.fromstring(rows[0][0])

    def operations():
        return sql(f'SELECT HEX(Id),Stage,HEX(Definition) FROM MagicCastingOperations WHERE CharacterId={actor} ORDER BY CreatedUtc,Id', (0, 2))

    def frozen_xml(element):
        copied = deepcopy(element)
        copied.tail = None  # Bank indentation is outside the frozen payload.
        return ET.tostring(copied, encoding='unicode')

    wand, staff, focus = (samples[k] for k in ('arm.item.blank_wand', 'arm.item.blank_staff', 'arm.focus.water'))
    if phase == 'cold':
        report = receipt['focus_devices']
        current = bank(wand, report['wand'])
        report['cold_charge_ids'] = [x.text for x in current.findall('Charge')]
        report['cold_stored_spell'] = frozen_xml(current.find('StoredSpell'))
        require(report['cold_charge_ids'] == report['charge_ids'], 'Cold restart changed exact charge GUIDs.')
        require(report['cold_stored_spell'] == frozen_xml(ET.fromstring(report['stored_spell'])), 'Cold restart changed frozen payload content.')
        require(balance() == report['balance'], 'Cold restart changed the paid reserve.')
        require(len(bank(staff, report['staff']).findall('Charge')) == 0 and
                len(bank(focus, report['focus']).findall('Charge')) == 0, 'Cold restart filled an empty staff or focus.')
        require(items(focus) == [[str(report['focus'])]], 'Cold restart lost or copied the retained crafted focus.')
        report['cold_status'] = 'PASS'
        assertions.append('S4 focus/device: cold retained exact charge GUID/frozen potency, focus identity, empty staff and paid reserves; stock gates disabled')
        return

    report = receipt['focus_devices'] = {'status': 'FAIL', 'sample_scope': 'Two exact recipe copies and three component/item copies; only availability bindings differ from installed stock. No package gate or admission enabled.'}
    session.send('drop all', read_seconds=.3)
    for merit, cap in ((config['read_merit'], config['read_capability']), (config['mend_merit'], config['mend_capability'])):
        session.send(f'givemerit me {merit}', read_seconds=.3)
        session.send(f'magic casting enrol {actor} {cap} disposable focus device acceptance', read_seconds=.3)
    flush()
    for sample in (focus, wand):
        for attempt in range(3):
            session.send('drop all', read_seconds=.3)
            session.send(f'item load {host["item_command_reference"](int(config["blank"]))}', read_seconds=.3)
            session.send(f'item load {host["item_command_reference"](int(config["seal"]))}', read_seconds=.3)
            started = time.monotonic()
            output = session.send('craft begin ' + sample['craft'], read_seconds=.5)
            deadline = time.monotonic() + float(sample['seconds']) + 20
            while time.monotonic() < deadline and (time.monotonic() < started + float(sample['seconds']) + 6 or not items(sample)):
                session.read_for(2)
            report.setdefault('craft_attempts', []).append({'key': sample['key'], 'attempt': attempt+1, 'output': host['redact'](output), 'elapsed_seconds': time.monotonic()-started})
            if items(sample):
                break
        require(len(items(sample)) == 1, 'Three bounded native crafting attempts did not produce exactly one ' + sample['key'])
        session.send('get ' + sample['keyword'], read_seconds=.3)
        instance = int(items(sample)[0][0])
        require(sql(f'SELECT GameItemId FROM Bodies_GameItems WHERE BodyId={body} AND GameItemId={instance}') == [[str(instance)]], 'Craft did not release its exact product into accessible native custody.')
        require(not bank(sample, instance).findall('Charge'), 'A newly crafted focus or carrier had free charges.')
        report['focus' if sample is focus else 'wand'] = instance
    assertions.append('S4 focus/device: native60-second focus and120-second wand recipes produced exact retained/empty products from owned blanks and seals')
    session.send('drop all', read_seconds=.3)
    session.send(f'item load {host["item_command_reference"](int(staff["prototype"]))}', read_seconds=.3)
    report['staff'] = int(items(staff)[0][0])
    require(not bank(staff, report['staff']).findall('Charge'), 'Blank capacity-ten staff was prefilled.')
    before = operations(); funds = balance()
    session.send('magicdevice charged ' + staff['keyword'] + ' self', read_seconds=.3); flush()
    require(before == operations() and funds == balance(), 'Empty charged activation paid or created an operation.')
    session.send('drop all', read_seconds=.3); session.send('get ' + focus['keyword'], read_seconds=.3)
    fund(config['read_capability'])
    before = operations(); funds = balance()
    output = session.send(f'magicdevice focus {focus["keyword"]} {config["read_capability"]} {config["read_spell"]} 1 self', read_seconds=.5); flush()
    added = [x for x in operations() if x[0] not in {old[0] for old in before}]
    require(len(added) == 1 and added[0][1] == 'Completed' and balance() < funds, 'Explicit native personal focus invocation did not pay: ' + host['redact'](output))
    require(items(focus) == [[str(report['focus'])]] and not bank(focus, report['focus']).findall('Charge'), 'Personal focus was consumed, copied or filled.')
    assertions.append('S4 focus/device: explicit Water focus paid the ordinary acquired personal route and retained its exact empty item; empty charged staff refused without payment')
    fund(config['read_capability']); funds = balance(); before = operations()
    output = session.send(f'{config["invoked"]} practice "Read Attunement" grade 1 via {config["read_capability"]}', read_seconds=.5)
    deadline = time.monotonic() + 45
    added = []
    while time.monotonic() < deadline:
        session.read_for(1); flush()
        added = [x for x in operations() if x[0] not in {old[0] for old in before}]
        if len(added) == 1 and added[0][1] == 'Completed':
            break
    require(len(added) == 1 and added[0][1] == 'Completed' and ET.fromstring(added[0][2]).get('mode') == 'Practice'
            and balance() < funds and items(focus) == [[str(report['focus'])]], 'Thirty-second paid target-free practice did not complete with the same retained focus: ' + host['redact'](output))
    report['practice_operation'] = added[0][0]
    assertions.append('S4 focus/device: ordinary30-second paid target-free practice completed while retaining the exact focus; no claim that an empty plan mandates a focus')
    session.send('drop all', read_seconds=.3); session.send('get ' + wand['keyword'], read_seconds=.3)
    fund(config['mend_capability']); funds = balance()
    output = session.send(f'magicdevice charge {wand["keyword"]} {config["mend_capability"]} {config["mend_spell"]} 1 1', read_seconds=.5)
    deadline = time.monotonic() + 80
    while time.monotonic() < deadline and not bank(wand, report['wand']).findall('Charge'):
        session.read_for(2); flush()
    current = bank(wand, report['wand'])
    require(len(current.findall('Charge')) == 1 and current.find('StoredSpell') is not None and balance() < funds,
            'One60-second paid native production did not persist one frozen homogeneous charge: ' + host['redact'](output))
    report['charge_ids'] = [x.text for x in current.findall('Charge')]
    report['stored_spell'] = frozen_xml(current.find('StoredSpell'))
    report['balance'] = balance(); report['status'] = 'PASS'
    assertions.append('S4 focus/device: one60-second legitimate acquired Mend Flesh production paid reserve and saved one unique charge with frozen potency')
