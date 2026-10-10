"""N21 full-server Flame Knife: paid timed weapons, real components, permanent output and cold custody."""
from __future__ import annotations

import argparse
import hashlib
import importlib.util
import json
import pathlib
import re
import socket
import subprocess
import sys
import time
import uuid
import xml.etree.ElementTree as ET

sys.dont_write_bytecode = True
spec = importlib.util.spec_from_file_location('n21_base', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.deadline = time.monotonic() + 1500
s.report.update(scope='bounded N21 builder-installed Flame Knife timed and paid permanent tiers', milestoneQualified=False)
for path in [pathlib.Path(__file__), s.repo / 'scripts/FuryCalmSmokeWorld/FlameKnifeFixtures.cs',
             s.repo / 'MudSharpCore/bin/Debug/net10.0/FutureMUDLibrary.dll',
             s.repo / 'MudSharpCore/Magic/ArmageddonFlameKnifeStock.cs',
             s.repo / 'MudSharpCore/GameItems/SpellOwnedItemValuePolicy.cs',
             *sorted((s.repo / 'MudSharpCore/Magic/SpellEffects').glob('CreateItemEffect*.cs'))]:
    s.report['inputs'][str(path.relative_to(s.repo))] = hashlib.sha256(path.read_bytes()).hexdigest()
mysql = None
fixture = places = None
spell = name = None
original_definition = original_seconds = None
original_command = s.command


def command(text, expected=None, seconds=.25):
    if fixture:
        for key, keyword in fixture['Keywords'].items(): text = text.replace('qan21' + key, keyword)
    expected = {'get': r'\bYou (get|pick up|take)\b', 'put': r'\bYou put\b', 'drop': r'\bYou drop\b',
                'give': r'\bYou give\b', 'wield': r'\bYou wield\b', 'unwield': r'\bYou (unwield|stop wielding)\b'}.get(expected, expected)
    return original_command(text, expected, seconds)


s.command = command


def start():
    title = s.sql('SELECT o.RoomName FROM Characters c JOIN Rooms r ON r.Id=c.Location JOIN RoomOverlays o ON o.Id=r.CurrentOverlayId WHERE c.Id=1')
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0)); port = listener.getsockname()[1]
    (s.runtime / 'Connection.config').write_text(f'127.0.0.1\n{port}\n\n', encoding='utf-8')
    s.process = s.helper.launch_mud(s.repo, s.runtime, argparse.Namespace(provider='MySql.Data.MySqlClient', startup_timeout=180), s.connection, [s.connection, s.password])
    s.session = s.helper.MudSocket('127.0.0.1', port, [s.connection, s.password]); s.session.read_for(.5)
    s.command('l'); s.command('Admin'); s.session.send(s.password, secret=True, read_seconds=.5)
    s.command('c'); s.command('1', re.escape(title), seconds=2)
    s.command('immortal', 'become immortal|already immortal')
    s.command('impdebug freezetime', 'freeze all')


def flush(): s.command('impdebug flush', seconds=.6)


def reserve():
    flush(); return float(s.sql('SELECT Amount FROM Characters_MagicResources WHERE CharacterId=1 AND MagicResourceId=5') or '0')


def count_lives(): return int(s.sql(f'SELECT COUNT(*) FROM MagicSpellLifecycles WHERE SpellId={spell}'))


def count_operations(): return int(s.sql(f'SELECT COUNT(*) FROM MagicCastingOperations WHERE MagicSpellId={spell}'))


def edit(text):
    s.command(f'magic spell edit {spell}', 'edit'); s.command('magic spell set effect 1 ' + text, 'updated|now|set'); s.command('magic spell close'); flush()


def load(role):
    s.command(f"item load {fixture['Prototypes'][role]}", 'load|create', seconds=.5); flush()
    item = int(s.sql(f"SELECT Id FROM GameItems WHERE GameItemProtoId={fixture['Prototypes'][role]} ORDER BY Id DESC LIMIT 1"))
    s.report.setdefault('ordinaryItems', []).append(dict(id=item, role=role, prototype=fixture['Prototypes'][role]))
    return item


def cast(grade, cost, reason=''):
    before = reserve(); lives = count_lives(); operations = count_operations()
    s.command('mortal', 'no longer immortal')
    try: answer = s.command(f'qamagi cast "{name}" grade {grade} on me via 5', seconds=1)
    finally: s.command('immortal', 'become immortal')
    after = reserve()
    s.check('exact native casting payment ' + str(grade) + ' ' + reason, before - after == cost, answer)
    s.check('one completed paid operation or zero refused operations', count_operations() == operations + (1 if cost else 0))
    s.check('one created lifecycle or zero refused lifecycles', count_lives() == lives + (1 if cost else 0))
    s.report.setdefault('paidCasts', []).append(dict(grade=grade, before=before, after=after, cost=cost, reason=reason, answer=answer))
    if cost: s.check('positive paid success rather than command echo', 'paid casting succeeded' in answer.lower(), answer)
    return answer


def created(grade, cost):
    cast(grade, cost)
    life, mode, selected_grade, deadline, state = s.sql(f"SELECT Id,Mode,Grade,IFNULL(DeadlineUtc,''),State FROM MagicSpellLifecycles WHERE SpellId={spell} ORDER BY CreatedUtc DESC LIMIT 1").split('\t')
    claims = s.sql(f"SELECT Kind,EntityId FROM MagicSpellOwnedEntities WHERE LifecycleId='{life}'").splitlines()
    s.check('exactly one native item is claimed', len(claims) == 1 and claims[0].split('\t')[0] == str(fixture['ItemKind']), str(claims))
    item = int(claims[0].split('\t')[1]); prototype = int(s.sql(f'SELECT GameItemProtoId FROM GameItems WHERE Id={item}'))
    expected_mode = fixture['Modes']['Permanent' if grade == 7 else 'Temporary']
    s.check('source grade maps to explicit output mode and prototype', int(selected_grade) == grade and int(mode) == expected_mode and
            prototype in (fixture['Staffs'] if grade == 7 else [fixture['Knives'][grade - 1]]))
    s.check('only permanent tier has no deadline', bool(deadline) == (grade != 7))
    if grade != 7:
        s.check('timed grade has exact configured duration', s.sql(f"SELECT TIMESTAMPDIFF(MICROSECOND,CreatedUtc,DeadlineUtc) FROM MagicSpellLifecycles WHERE Id='{life}'") == str((240 + 5 * grade) * 1000000))
    s.check('permanent creation completes while timed creation is active', int(state) == fixture['States']['Completed' if grade == 7 else 'Active'])
    role = next(key for key, value in fixture['Prototypes'].items() if value == prototype)
    s.check('new weapon has exact ordinary hand custody', s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={item}') == s.sql('SELECT BodyId FROM Characters WHERE Id=1'))
    row = dict(id=life, item=item, prototype=prototype, role=role, grade=grade, mode=int(mode), deadline=deadline,
               original=s.sql(f"SELECT GameItemProtoId,GameItemProtoRevision,Quality,IFNULL(OwnerId,0),IFNULL(OwnerType,'') FROM GameItems WHERE Id={item}"))
    s.report.setdefault('lifecycles', []).append(row)
    return row


def temporary_refuses_salvage(row):
    was_wielded = s.sql(f"SELECT Wielded FROM Bodies_GameItems WHERE GameItemId={row['item']}") == '1'
    if was_wielded: s.command('unwield qan21' + row['role'], 'unwield')
    s.command('drop qan21' + row['role'], 'drop')
    baseline = s.sql(f"SELECT COUNT(*) FROM GameItems WHERE GameItemProtoId={fixture['Prototypes']['scrap']}")
    answer = s.command('salvage qan21' + row['role'], 'temporary spell-created material cannot become permanent value', seconds=.5)
    flush()
    s.check('temporary salvage produces no permanent goods and preserves source', s.sql(f"SELECT COUNT(*) FROM GameItems WHERE GameItemProtoId={fixture['Prototypes']['scrap']}") == baseline and
            s.sql(f"SELECT COUNT(*) FROM GameItems WHERE Id={row['item']}") == '1', answer)
    s.command('get qan21' + row['role'], 'get')
    if was_wielded: s.command('wield qan21' + row['role'], 'wield')
    flush()
    s.check('temporary salvage refusal restores exact hand or wield custody', s.sql(f"SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={row['item']}") == s.sql('SELECT BodyId FROM Characters WHERE Id=1') and
            (not was_wielded or s.sql(f"SELECT Wielded FROM Bodies_GameItems WHERE GameItemId={row['item']}") == '1'))


def complete(row, maximum=300):
    end = time.monotonic() + maximum; state = ''; seen_before_due = False
    while time.monotonic() < end:
        state = s.sql(f"SELECT State,Reason,Diagnostic FROM MagicSpellLifecycles WHERE Id='{row['id']}'")
        remaining = int(s.sql(f"SELECT TIMESTAMPDIFF(MICROSECOND,UTC_TIMESTAMP(6),DeadlineUtc) FROM MagicSpellLifecycles WHERE Id='{row['id']}'"))
        if remaining > 0:
            assert state.split('\t')[0] == str(fixture['States']['Active']), 'Temporary weapon retired before its original deadline: ' + state
            if remaining < 5000000:
                assert s.sql(f"SELECT COUNT(*) FROM GameItems WHERE Id={row['item']}") == '1', 'Temporary weapon disappeared before its original deadline'
                seen_before_due = True
        if state.split('\t')[0] == str(fixture['States']['Completed']): break
        s.session.read_for(.5)
    s.check('temporary weapon survives immediately before original deadline', seen_before_due)
    s.check('temporary weapon expires at original deadline without a hold', state.split('\t')[0] == str(fixture['States']['Completed']), state)
    s.check('terminal reason and timestamp prove actual expiry', state.split('\t')[1] == str(fixture['ExpiryReason']) and
            s.sql(f"SELECT UpdatedUtc>=DeadlineUtc FROM MagicSpellLifecycles WHERE Id='{row['id']}'") == '1')
    s.check('only the exact temporary weapon row is removed', s.sql(f"SELECT COUNT(*) FROM GameItems WHERE Id={row['item']}") == '0')
    row['terminal'] = state


def consumes_only(selected, create):
    ordinary = [x['id'] for x in s.report['ordinaryItems'] if s.sql(f"SELECT COUNT(*) FROM GameItems WHERE Id={x['id']}") == '1']
    result = create()
    s.check('component consumption removes only selected ordinary item', all(
        s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={item}') == ('0' if item == selected else '1') for item in ordinary))
    return result


def run():
    global mysql, fixture, places, spell, name, original_definition, original_seconds
    try:
        binary = pathlib.Path(r'C:\Program Files\MySQL\MySQL Server 8.0\bin'); data = pathlib.Path(s.instance['data']).resolve()
        assert data == (s.root / 'mysql/data').resolve()
        with socket.socket() as probe: assert probe.connect_ex(('127.0.0.1', s.instance['port'])) != 0, 'Owned port is occupied'
        log = (s.runtime / 'mysql-process.log').open('w', encoding='utf-8')
        mysql = subprocess.Popen([str(binary / 'mysqld.exe'), '--no-defaults', '--basedir=' + str(binary.parent), '--datadir=' + str(data),
                                  '--port=' + str(s.instance['port']), '--bind-address=127.0.0.1', '--mysqlx=0', '--character-set-server=utf8mb4',
                                  '--collation-server=utf8mb4_unicode_ci', '--max-allowed-packet=64M', '--log-error=' + str(s.root / 'mysql/mysql.err')],
                                 stdout=log, stderr=log, creationflags=subprocess.CREATE_NO_WINDOW)
        limit = time.monotonic() + 60
        while time.monotonic() < limit:
            try: s.sql('SELECT 1'); break
            except AssertionError: time.sleep(.25)
        s.fixture('projection-fixtures'); places = s.fixture('shelter-fixtures'); fixture = s.fixture('flame-knife-fixtures')
        start(); s.command('peace', 'end .* combats'); s.command('drop all')
        s.command(f"magic spell edit new stock flame-knife 5 297 4 {' '.join(str(x) for x in fixture['Knives'] + fixture['Staffs'])} {fixture['ComponentTag']}", 'Created.*Flame Knife')
        spell = int(s.sql("SELECT Id FROM MagicSpells WHERE Name='Flame Knife' ORDER BY Id DESC LIMIT 1")); name = 'QA_N21_Flame_' + uuid.uuid4().hex[:6]
        s.command('magic spell set name ' + name, 'name|renamed|called'); s.command('magic spell set difficulty automatic', 'Automatic to cast'); s.command('magic spell close'); flush()
        original_definition = s.sql(f'SELECT Definition FROM MagicSpells WHERE Id={spell}')
        original_seconds = ET.fromstring(original_definition).findtext('Effects/Effect/Lifecycle/Seconds')
        s.report['originalStockDefinition'] = original_definition
        edit('lifetime 240+5*grade')
        s.command('magic capability edit 5', 'edit'); s.command(f'magic capability set casting entry add {spell}', 'updated|already'); s.command('magic capability close')
        s.command(f'magic casting grant 1 5 {spell} N21 native qualification', 'grant|acquir|already')
        s.command(f"combat set {fixture['CombatSetting']}", 'setting')
        s.stop()
        s.sql('UPDATE CharacterTraits SET Value=100 WHERE CharacterId=1 AND TraitDefinitionId=297')
        s.sql('INSERT INTO Characters_MagicResources (CharacterId,MagicResourceId,Amount) VALUES (1,5,1000) ON DUPLICATE KEY UPDATE Amount=1000')
        s.sql(f'UPDATE CharacterAcquiredSpells SET ControlledGrade=7 WHERE CharacterId=1 AND MagicSpellId={spell}')
        s.report['objects'] = dict(player=1, recipient=3, combatTarget=2, spell=spell, fixtures=fixture, rooms=places)
        start(); s.command(f"goto {places['Anchor']}", 'already|QA'); s.command('transfer qatarget'); s.command('force qatarget drop all'); s.command('drop all')
        cast(7, 0, 'missing component')
        for role in ['wrong', 'low']:
            item = load(role); cast(7, 0, role + ' component')
            s.check('unsuitable real component is not consumed', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={item}') == '1'); s.command('drop qan21' + role, 'drop')
        component = load('six'); s.command('drop qan21six', 'drop'); cast(7, 0, 'ground component')
        bag = load('bag'); s.command('get qan21six', 'get'); s.command('put qan21six qan21bag', 'put'); cast(7, 0, 'contained component')
        s.command('get qan21six qan21bag', 'get'); s.command('drop qan21bag', 'drop')
        timed = created(1, 7)
        s.check('lower tier does not consume eligible carried component', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={component}') == '1')
        temporary_refuses_salvage(timed)
        s.command('drop qan21six', 'drop'); cast(7, 0, 'temporary weapon as otherwise tagged component')
        s.check('temporary casting-material refusal retains exact source', s.sql(f"SELECT COUNT(*) FROM GameItems WHERE Id={timed['item']}") == '1')
        s.command('drop qan21' + timed['role'], 'drop'); s.command('get qan21bag', 'get')
        foreign = load('foreign'); s.command('put qan21foreign qan21bag', 'put'); s.command('get qan21' + timed['role'], 'get'); s.command('put qan21' + timed['role'] + ' qan21bag', 'put')
        s.command('give qan21bag qatarget', 'give'); flush()
        s.report['borrowedCustody'] = dict(bag=bag, foreign=foreign, holder=3,
            bagOriginal=s.sql(f"SELECT GameItemProtoId,GameItemProtoRevision,Quality,IFNULL(OwnerId,0),IFNULL(OwnerType,'') FROM GameItems WHERE Id={bag}"),
            foreignOriginal=s.sql(f"SELECT GameItemProtoId,GameItemProtoRevision,Quality,IFNULL(OwnerId,0),IFNULL(OwnerType,'') FROM GameItems WHERE Id={foreign}"))
        s.check('timed weapon transfers inside borrowed bag without changing origin', s.sql(f"SELECT ContainerId FROM GameItems WHERE Id={timed['item']}") == str(bag) and
                s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={bag}') == s.sql('SELECT BodyId FROM Characters WHERE Id=3'))
        s.command('get qan21six', 'get'); wielded = created(6, 25)
        s.command('wield qan21' + wielded['role'], 'wield'); flush()
        s.command('transfer qacaster'); s.command('force qacaster drop all'); s.command('force qacaster sleep', seconds=.5)
        s.check('native caster wields only exact claimed burning weapon', s.sql(f"SELECT GameItemId FROM Bodies_GameItems WHERE BodyId=(SELECT BodyId FROM Characters WHERE Id=1) AND Wielded=1") == str(wielded['item']) and
                s.sql(f"SELECT Id,DamageType FROM WeaponAttacks WHERE WeaponTypeId={fixture['WeaponType']}") == f"{fixture['Attack']}\t{fixture['BurningDamageType']}" and
                s.sql('SELECT s.WeaponUsePercentage,s.NaturalWeaponPercentage,s.MagicUsePercentage,s.PsychicUsePercentage,s.AuxiliaryPercentage FROM Characters c JOIN CharacterCombatSettings s ON s.Id=c.CurrentCombatSettingId WHERE c.Id=1') == '1\t0\t0\t0\t0')
        before_wounds = int(s.sql('SELECT IFNULL(MAX(Id),0) FROM Wounds'))
        s.command('mortal', 'no longer immortal')
        try:
            combat_start = len(s.session.full_transcript())
            s.command('hit qacaster', seconds=1); limit = time.monotonic() + 30
            while time.monotonic() < limit:
                s.session.read_for(.5); flush()
                wounds = s.sql(f"SELECT Id,DamageType,CurrentDamage,CurrentPain,CurrentStun,ActorOriginId,ToolOriginId FROM Wounds WHERE Id>{before_wounds} AND BodyId=(SELECT BodyId FROM Characters WHERE Id=2) AND DamageType={fixture['BurningDamageType']} AND ActorOriginId=1 AND CurrentDamage>0")
                if wounds: break
            combat_output = s.session.full_transcript()[combat_start:]
            s.check('same native hit window records exact weapon striking combat target', re.search(r'You thrust a ' + re.escape(fixture['Keywords'][wielded['role']]) + r' fixture at a qacaster human, and .*?is struck', combat_output, re.S) is not None, combat_output[-2500:])
            s.check('native wielded weapon delivers persisted burning wounds', bool(wounds), wounds)
            s.check('native wound tool attribution is absent or exact, never another weapon', all(x.split('\t')[-1] in ['NULL', str(wielded['item'])] for x in wounds.splitlines()))
            s.report['burningWounds'] = wounds
            s.report['combatEvidence'] = dict(target=2, weapon=wielded['item'], attack=fixture['Attack'], afterWoundId=before_wounds,
                output=combat_output, limitation='The native GetDamagePlusSelfDamageForWeapon path sets ToolOrigin=null. Burning wound/caster and same-window weapon-specific strike prove behavior; Wounds does not persist the weapon ID.')
        finally:
            s.command('immortal', 'become immortal'); s.command('peace', 'end .* combats')
        s.command('unwield qan21' + wielded['role'], 'unwield')
        s.command('drop qan21' + wielded['role'], 'drop')
        permanent = consumes_only(component, lambda: created(7, 50))
        s.check('highest tier consumes exactly selected carried component', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={component}') == '0')
        s.command('give qan21' + permanent['role'] + ' qatarget', 'give')
        higher = load('seven'); convertible = consumes_only(higher, lambda: created(7, 50))
        s.check('higher descendant component is consumed exactly once', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={higher}') == '0')
        s.command('get qan21' + wielded['role'], 'get')
        s.command('wield qan21' + wielded['role'], 'wield'); flush()
        s.check('temporary weapon is wielded again after freeing a casting hand', s.sql(f"SELECT BodyId,Wielded FROM Bodies_GameItems WHERE GameItemId={wielded['item']}") == s.sql('SELECT BodyId FROM Characters WHERE Id=1') + '\t1')
        saved = s.sql(f"SELECT Id,Mode,Grade,IFNULL(DeadlineUtc,''),Provenance FROM MagicSpellLifecycles WHERE SpellId={spell} ORDER BY Id")
        paid = reserve(); s.check('normal paid tiers debit source efficiency exactly', paid == 868)
        s.stop(); start()
        s.check('cold restart preserves exact origins deadlines modes and payment', s.sql(f"SELECT Id,Mode,Grade,IFNULL(DeadlineUtc,''),Provenance FROM MagicSpellLifecycles WHERE SpellId={spell} ORDER BY Id") == saved and reserve() == paid)
        for row in [timed, wielded, permanent, convertible]:
            s.check('cold created item retains prototype quality and legal owner', s.sql(f"SELECT GameItemProtoId,GameItemProtoRevision,Quality,IFNULL(OwnerId,0),IFNULL(OwnerType,'') FROM GameItems WHERE Id={row['item']}") == row['original'])
        s.check('cold timed weapon remains in exact recipient-held bag', s.sql(f"SELECT ContainerId FROM GameItems WHERE Id={timed['item']}") == str(bag) and
                s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={bag}') == s.sql('SELECT BodyId FROM Characters WHERE Id=3'))
        s.check('cold grade-six weapon remains wielded on exact player body', s.sql(f"SELECT BodyId,Wielded FROM Bodies_GameItems WHERE GameItemId={wielded['item']}") == s.sql('SELECT BodyId FROM Characters WHERE Id=1') + '\t1' and fixture['Keywords'][wielded['role']] in s.command('inventory').lower())
        temporary_refuses_salvage(wielded)
        scrap_before = int(s.sql(f"SELECT COUNT(*) FROM GameItems WHERE GameItemProtoId={fixture['Prototypes']['scrap']}"))
        s.command('drop qan21' + convertible['role'], 'drop')
        s.command('salvage qan21' + convertible['role'], 'begin.*salvage', seconds=2); limit = time.monotonic() + 20
        while time.monotonic() < limit:
            flush()
            if s.sql(f"SELECT COUNT(*) FROM GameItems WHERE Id={convertible['item']}") == '0': break
            s.session.read_for(.5)
        s.check('paid permanent staff permits legitimate permanent salvage', s.sql(f"SELECT COUNT(*) FROM GameItems WHERE Id={convertible['item']}") == '0' and
                int(s.sql(f"SELECT COUNT(*) FROM GameItems WHERE GameItemProtoId={fixture['Prototypes']['scrap']}")) == scrap_before + 1)
        scrap = int(s.sql(f"SELECT Id FROM GameItems WHERE GameItemProtoId={fixture['Prototypes']['scrap']} ORDER BY Id DESC LIMIT 1"))
        s.report['legitimatePermanentSalvage'] = dict(staff=convertible['item'], product=scrap)
        s.check('legitimate salvage has ordinary permanent value', s.sql(f"SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind={fixture['ItemKind']} AND EntityId={scrap}") == '0')
        complete(timed); complete(wielded)
        s.check('wielded expiry clears live player equipment', fixture['Keywords'][wielded['role']] not in s.command('inventory', seconds=.5).lower())
        flush()
        s.check('foreign bag and contents survive expiry once with exact custody', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id IN ({bag},{foreign})') == '2' and
                s.sql(f'SELECT ContainerId FROM GameItems WHERE Id={foreign}') == str(bag) and
                s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={bag}') == s.sql('SELECT BodyId FROM Characters WHERE Id=3'))
        for label, item in [('bag', bag), ('foreign', foreign)]:
            s.check('foreign prototype quality and title retained', s.sql(f"SELECT GameItemProtoId,GameItemProtoRevision,Quality,IFNULL(OwnerId,0),IFNULL(OwnerType,'') FROM GameItems WHERE Id={item}") == s.report['borrowedCustody'][label + 'Original'])
        s.check('paid permanent output remains held after temporary expiry', s.sql(f"SELECT COUNT(*) FROM GameItems WHERE Id={permanent['item']}") == '1' and
                s.sql(f"SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={permanent['item']}") == s.sql('SELECT BodyId FROM Characters WHERE Id=3') and
                s.sql(f"SELECT Mode,State,DeadlineUtc IS NULL FROM MagicSpellLifecycles WHERE Id='{permanent['id']}'") == f"{fixture['Modes']['Permanent']}\t{fixture['States']['Completed']}\t1")
        s.check('no refunds or new creations during restart and expiry', reserve() == paid and count_lives() == 4 and count_operations() == 4)
        edit('lifetime ' + original_seconds)
        s.check('original authored stock definition is restored exactly', s.sql(f'SELECT Definition FROM MagicSpells WHERE Id={spell}') == original_definition)
        s.command(f"combat set {fixture['OriginalCombatSetting']}", 'setting'); flush()
        s.check('original combat setting is restored', s.sql('SELECT CurrentCombatSettingId FROM Characters WHERE Id=1') == str(fixture['OriginalCombatSetting']))
        s.stop(); start()
        s.check('second cold load retains permanent staff and legitimate salvage', s.sql(f"SELECT COUNT(*) FROM GameItems WHERE Id IN ({permanent['item']},{scrap})") == '2' and
                s.sql(f"SELECT COUNT(*) FROM GameItems WHERE Id IN ({timed['item']},{wielded['item']})") == '0' and reserve() == paid)
        s.stop(); s.check('all captured native inputs remain unchanged', all(hashlib.sha256((s.repo / path).read_bytes()).hexdigest() == digest for path, digest in s.report['inputs'].items()))
        s.report.update(status='PASS', milestoneQualified=True)
    except BaseException as error:
        s.report.update(status='FAIL', error=s.helper.redact(str(error), [s.connection, s.password]))
    finally:
        try: s.finish()
        finally:
            if mysql:
                try: s.sql('SHUTDOWN')
                except AssertionError: pass
                s.helper.stop_process_tree(mysql); s.report['mysqlStoppedByOwnedHandle'] = mysql.poll() is not None
                end = time.monotonic() + 30; closed = False
                while time.monotonic() < end:
                    with socket.socket() as probe: closed = probe.connect_ex(('127.0.0.1', s.instance['port'])) != 0
                    if closed: break
                    time.sleep(.25)
                s.report['mysqlPortClosed'] = closed
                if not closed: s.report.update(status='FAIL', milestoneQualified=False, error='Owned MySQL port remained open after cleanup')
            (s.runtime / 'receipt.json').write_text(json.dumps(s.report, indent=2), encoding='utf-8')
    return 0 if s.report['status'] == 'PASS' else 1


if __name__ == '__main__': raise SystemExit(run())
