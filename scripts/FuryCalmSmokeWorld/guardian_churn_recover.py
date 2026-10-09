"""Recover an interrupted N16 fixture through native expiry, never count as N16 qualification."""
import importlib.util
import hashlib
import json
import pathlib
import time

spec = importlib.util.spec_from_file_location('fury_calm_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.report.update(scope='Interrupted N16 owned-fixture native retirement recovery only',
                qualificationMarker='guardian-churn-recovery-passed.json', n16Qualified=False)
s.report['inputs'][str(pathlib.Path(__file__).relative_to(s.repo))] = hashlib.sha256(pathlib.Path(__file__).read_bytes()).hexdigest()

try:
    prior = json.loads((s.root / 'guardian-lifecycle-modes-passed.json').read_text(encoding='utf-8'))
    s.check('N15 owned-world prerequisite is qualified', prior['status'] == prior['runnerStatus'] == 'PASS')
    spell = prior['objects']['guardianSpell']
    permanent = next(row for row in prior['createdLifecycles'] if row['mode'] == 'permanent')
    s.report['before'] = s.sql(f'SELECT Id,Mode,State,DeadlineUtc FROM MagicSpellLifecycles WHERE SpellId={spell} ORDER BY Id')
    operations = s.sql('SELECT COUNT(*) FROM MagicCastingOperations')
    claims = s.sql('SELECT LifecycleId,Kind,EntityId,Role FROM MagicSpellOwnedEntities ORDER BY LifecycleId,Kind,EntityId')
    identifiers = s.sql(f'SELECT Id FROM MagicSpellLifecycles WHERE SpellId={spell} ORDER BY Id')
    s.check('interrupted outputs are temporary with elapsed exact deadlines',
            s.sql(f'SELECT COUNT(*) FROM MagicSpellLifecycles WHERE SpellId={spell} AND State<>3 '
                  'AND (Mode=0 OR DeadlineUtc IS NULL OR DeadlineUtc>UTC_TIMESTAMP(6))') == '0')
    item_proto = int(s.sql("SELECT LogicalId FROM SeederManagedRecords WHERE StableKey='arm.item.charged_staff'"))
    admin_body = int(s.sql("SELECT c.BodyId FROM Characters c JOIN Accounts a ON a.Id=c.AccountId WHERE a.Name='Admin' AND c.IsArchived=0"))
    held = s.sql(f'SELECT i.Id FROM GameItems i JOIN Bodies_GameItems b ON b.GameItemId=i.Id '
                 f'WHERE b.BodyId={admin_body} AND i.GameItemProtoId={item_proto} ORDER BY i.Id').splitlines()
    s.report['retainedInterruptedGoods'] = held
    s.start()
    # Failed pre-give goods are ordinary items; move them to the room through public custody.
    for item in held:
        s.check('interrupted foreign item is unclaimed',
                s.sql(f'SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={int(item)}') == '0')
        s.command('drop staff', 'drop')
    limit = min(s.deadline, time.monotonic() + 180)
    answer = ''
    while time.monotonic() < limit:
        answer = s.sql(f'SELECT COUNT(*) FROM MagicSpellLifecycles WHERE SpellId={spell} AND State<>3')
        if answer == '0':
            break
        s.session.read_for(.5)
    s.check('native expiry completes every interrupted temporary lifecycle', answer == '0', answer)
    s.command('impdebug flush', seconds=.5)
    s.check('recovery never replays payment creation or ownership',
            s.sql('SELECT COUNT(*) FROM MagicCastingOperations') == operations and
            s.sql(f'SELECT Id FROM MagicSpellLifecycles WHERE SpellId={spell} ORDER BY Id') == identifiers and
            s.sql('SELECT LifecycleId,Kind,EntityId,Role FROM MagicSpellOwnedEntities ORDER BY LifecycleId,Kind,EntityId') == claims)
    s.check('ordinary permanent guardian remains the same living body',
            s.sql(f'SELECT BodyId,IsArchived,(State & 64)<>0 FROM Characters WHERE Id={permanent["character"]}') ==
            f'{permanent["body"]}\t0\t0')
    for item in held:
        s.check('interrupted foreign goods retained in ordinary room custody',
                s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={int(item)}') == '1' and
                not s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={int(item)}') and
                bool(s.sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={int(item)}')))
    s.report['after'] = s.sql(f'SELECT Id,Mode,State,DeadlineUtc FROM MagicSpellLifecycles WHERE SpellId={spell} ORDER BY Id')
    s.stop()
    s.report['status'] = 'PASS'
except BaseException as error:
    s.report.update(status='FAIL', error=s.helper.redact(str(error), [s.connection, s.password]))
finally:
    s.deadline = max(s.deadline, time.monotonic() + 40)
    if s.process is not None:
        try:
            s.stop()
        except BaseException as error:
            s.report.update(status='FAIL', cleanupError=s.helper.redact(str(error), [s.connection, s.password]))
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
