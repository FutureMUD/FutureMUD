"""Read-only audit of the retained N16 journal transport failure; no MUD startup."""
import hashlib
import importlib.util
import json
import os
import pathlib

repo = pathlib.Path(__file__).resolve().parents[2]
spec = importlib.util.spec_from_file_location('fury_calm_smoke', repo / 'scripts/FuryCalmSmokeWorld/smoke.py')
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.report.update(scope='Read-only N16 journal transport audit, no gameplay qualification',
                qualificationMarker='guardian-journal-transport-audit-passed.json', n16Qualified=False)
s.report['inputs'][str(pathlib.Path(__file__).relative_to(repo))] = hashlib.sha256(pathlib.Path(__file__).read_bytes()).hexdigest()
failed_path = (s.root / os.environ['N16_JOURNAL_AUDIT_FAILED_RECEIPT']).resolve()
assert failed_path.is_relative_to(s.root)
failed = json.loads(failed_path.read_text(encoding='utf-8'))
columns = ('Id,SpellId,Grade,CreatorId,HEX(Family),Mode,CreatedUtc,DeadlineUtc,'
           'HEX(Provenance),State,Reason,DeathObservedUtc,RemainsItemId,'
           'RemainsRemovalRequestedUtc,RemainsNotificationAttemptedUtc,'
           'RemainsNotificationCompletedUtc,UpdatedUtc,Version,HEX(Diagnostic)')
try:
    assert failed['status'] == failed['runnerStatus'] == 'FAIL' and failed['cleanup']['mysqlStopped']
    old = failed['cycles'][-1]['terminalJournals'].splitlines()
    old_ids = {line.split('\t', 1)[0] for line in old}
    current = s.sql(f'SELECT {columns} FROM MagicSpellLifecycles ORDER BY Id')
    actual = [line for line in current.splitlines() if line.split('\t', 1)[0] in old_ids]
    differences = [(before, after) for before, after in zip(old, actual) if before != after]
    s.check('retained transport failure has exactly one differing prior row',
            len(actual) == len(old) and len(differences) == 1)
    before, after = differences[0]
    identifier = before.split('\t', 1)[0]
    s.check('only the old final empty diagnostic delimiter differs',
            before == old[-1] and len(before.split('\t')) == 18 and
            len(after.split('\t')) == 19 and after == before + '\t')
    s.check('the exact native diagnostic field is empty and was never null',
            json.loads(s.sql(f"SELECT JSON_ARRAY(Diagnostic IS NULL,HEX(Diagnostic)) FROM MagicSpellLifecycles WHERE Id='{identifier}'")) == [0, ""])
    rows = [json.loads(line) for line in s.sql(f'SELECT JSON_ARRAY({columns}) FROM MagicSpellLifecycles ORDER BY Id').splitlines()]
    s.check('lossless transport preserves nineteen fields for every actual journal', all(len(row) == 19 for row in rows))
    claims = failed['activeCycle']['claims']
    characters = ','.join(str(value['1']) for value in claims.values())
    bodies = ','.join(str(value['3']) for value in claims.values())
    s.check('all interrupted outputs already completed physical retirement',
            s.sql(f'SELECT COUNT(*) FROM Bodies WHERE Id IN ({bodies})') == '0' and
            s.sql(f'SELECT COUNT(*) FROM Npcs WHERE CharacterId IN ({characters})') == '0' and
            s.sql(f'SELECT COUNT(*) FROM CharacterInstances WHERE CharacterId IN ({characters})') == '0' and
            s.sql('SELECT COUNT(*) FROM Characters c JOIN CharacterArchives a ON a.CharacterId=c.Id '
                  f'WHERE c.Id IN ({characters}) AND c.IsArchived=1 AND c.BodyId IS NULL') == str(len(claims)))
    s.report.update(status='PASS', auditedPriorRows=len(old), differingRow=identifier,
                    differingField='Empty HEX(Diagnostic) final TSV delimiter stripped from old last row',
                    baselineRow=before, currentRow=after,
                    journals=rows,
                    failedReceipt=dict(path=str(failed_path), sha256=hashlib.sha256(failed_path.read_bytes()).hexdigest()))
except BaseException as error:
    s.report.update(status='FAIL', error=s.helper.redact(str(error), [s.connection, s.password]))
finally:
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
