"""Read-only audit of a failed N16 batch command boundary; never qualifies gameplay."""
import hashlib
import importlib.util
import json
import os
import pathlib
import uuid

spec = importlib.util.spec_from_file_location('fury_calm_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.report.update(scope='Read-only N16 batch command acknowledgement audit; no MUD startup',
                qualificationMarker='guardian-batch-audit-passed.json', n16Qualified=False)
s.report['inputs'][str(pathlib.Path(__file__).relative_to(s.repo))] = hashlib.sha256(pathlib.Path(__file__).read_bytes()).hexdigest()
path = (s.root / os.environ['N16_BATCH_FAILED_RECEIPT']).resolve()
assert path.is_relative_to(s.root)
failed = json.loads(path.read_text(encoding='utf-8'))


def positive_id(value):
    assert type(value) is int and 0 < value <= 9223372036854775807, 'Invalid positive entity ID'
    return value


try:
    assert failed['status'] == failed['runnerStatus'] == 'FAIL' and failed['cleanup']['mysqlStopped']
    cycle = failed['activeCycle']
    assert cycle['batch'] and len(cycle['claims']) == 128
    for identifier, claim in cycle['claims'].items():
        assert isinstance(identifier, str) and str(uuid.UUID(identifier)) == identifier, 'Invalid lifecycle UUID'
        assert set(claim) == {'1', '3'}, 'Unexpected typed ownership claims'
        positive_id(claim['1'])
        positive_id(claim['3'])
    item = positive_id(cycle['foreignItem'])
    caster = positive_id(failed['objects']['caster'])
    reserve = positive_id(failed['objects']['reserve'])
    previous = json.loads((s.root / 'guardian-churn-15-passed.json').read_text(encoding='utf-8'))
    assert previous['status'] == previous['runnerStatus'] == 'PASS' and previous['objects']['caster'] == caster
    # Historic failures predate immediate room capture. The unchanged owned fixture
    # is anchored to the preceding qualified room, never to its current location alone.
    room = positive_id(cycle.get('room', previous['cycles'][-1]['room']))
    expected_deaths = int(os.environ.get('N16_BATCH_EXPECTED_EARLY_DEATHS', '64'))
    assert 0 <= expected_deaths <= 64
    identifiers = ','.join("'" + identifier + "'" for identifier in cycle['claims'])
    rows = [json.loads(line) for line in s.sql(
        f'SELECT JSON_ARRAY(Id,State,Reason,DeathObservedUtc,DeadlineUtc,DeathObservedUtc<DeadlineUtc) '
        f'FROM MagicSpellLifecycles WHERE Id IN ({identifiers}) ORDER BY Id').splitlines()]
    early = sum(row[5] == 1 for row in rows)
    s.report.update(failedReceipt=dict(path=str(path), sha256=hashlib.sha256(path.read_bytes()).hexdigest()),
                    journals=rows, earlyDeaths=early,
                    terminal=sum(row[1] == 3 for row in rows),
                    retainedForeignItem=item, expectedEarlyDeaths=expected_deaths,
                    expectedEarlyDeathSource='operator-specified submitted count; corroborate with retained transcript',
                    originalRoomAnchor=dict(room=room, source='failed activeCycle' if 'room' in cycle else
                        'preceding qualified stage15; unchanged fixture required'))
    s.check('all exact failed-batch journals remain present', len(rows) == 128)
    s.check('operator-specified submitted kill count matches durable early deaths', early == expected_deaths, str(early))
    if os.environ.get('N16_BATCH_REQUIRE_RECOVERY') == '1':
        s.report['recoveryVerified'] = True
        s.check('all exact failed-batch outputs are terminal after native recovery', all(row[1] == 3 for row in rows))
        for identifier, claim in cycle['claims'].items():
            character, body = claim['1'], claim['3']
            s.check('exact interrupted output has no heavy physical records ' + identifier,
                    s.sql(f'SELECT (SELECT COUNT(*) FROM Bodies WHERE Id={body}),'
                          f'(SELECT COUNT(*) FROM Npcs WHERE CharacterId={character}),'
                          f'(SELECT COUNT(*) FROM CharacterInstances WHERE CharacterId={character} OR BodyId={body})') == '0\t0\t0')
            s.check('exact interrupted canonical archive retains original body and lifecycle ' + identifier,
                    s.sql(f'SELECT c.IsArchived,c.BodyId,a.OriginalBodyId,a.LifecycleId FROM Characters c '
                          f'JOIN CharacterArchives a ON a.CharacterId=c.Id WHERE c.Id={character}') ==
                    f'1\tNULL\t{body}\t{identifier}')
            s.check('both original typed claims survive unchanged ' + identifier,
                    s.sql(f"SELECT Kind,EntityId,Role FROM MagicSpellOwnedEntities WHERE LifecycleId='{identifier}' ORDER BY Kind") ==
                    f'1\t{character}\t0\n3\t{body}\t0')
        s.check('foreign goods survive in the pinned original room with no body or claim custody',
                s.sql(f'SELECT LocationId FROM CharacterInstances WHERE CharacterId={caster} AND IsPrimary=1') == str(room) and
                s.sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={item}') == str(room) and
                s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={item}') == '1' and
                not s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={item}') and
                s.sql(f'SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={item}') == '0')
        previous_audit_path = os.environ.get('N16_BATCH_PREVIOUS_RECOVERY_AUDIT')
        if previous_audit_path:
            previous_audit_path = (s.root / previous_audit_path).resolve()
            assert previous_audit_path.is_relative_to(s.root)
            previous_audit = json.loads(previous_audit_path.read_text(encoding='utf-8'))
            assert previous_audit['status'] == previous_audit['runnerStatus'] == 'PASS' and previous_audit['recoveryVerified']
            immediately_preceding = json.loads((path.parent / 'prior-receipt.json').read_text(encoding='utf-8'))
            assert immediately_preceding == previous_audit, 'Recovery audit must be the failed invocation\'s immediate predecessor'
            earlier_failed_path = pathlib.Path(previous_audit['failedReceipt']['path']).resolve()
            assert earlier_failed_path.is_relative_to(s.root)
            assert hashlib.sha256(earlier_failed_path.read_bytes()).hexdigest() == previous_audit['failedReceipt']['sha256']
            earlier_failed = json.loads(earlier_failed_path.read_text(encoding='utf-8'))
            assert earlier_failed['objects']['caster'] == caster and earlier_failed['objects']['reserve'] == reserve
            assert set(cycle['before']['lifecycles']) == (set(earlier_failed['activeCycle']['before']['lifecycles']) |
                                                        set(earlier_failed['activeCycle']['claims']))
            expected = previous_audit['resourceAfterRecovery'] - 9
        else:
            expected = previous['cycles'][-1]['energyAfter'] - 9
        actual = float(s.sql(f'SELECT Amount FROM Characters_MagicResources WHERE CharacterId={caster} '
                             f'AND MagicResourceId={reserve}'))
        s.check('the exact failed paid cast remains charged after native recovery', actual == expected, str(actual))
        s.report.update(retainedForeignRoom=int(room), resourceAfterRecovery=actual,
                        recoveryReceipt=json.loads((s.root / 'guardian-churn-recovery-passed.json').read_text(encoding='utf-8'))['invocationReceipt'])
    s.report['status'] = 'PASS'
except BaseException as error:
    s.report.update(status='FAIL', error=s.helper.redact(str(error), [s.connection, s.password]))
finally:
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
