"""Retry the unchanged physical-release gate for a retained failed NPC admission receipt."""
import hashlib
import importlib.util
import json
import os
import pathlib
import re
import uuid

spec = importlib.util.spec_from_file_location('fury_calm_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.report['scope'] = 'retained paid NPC expiry/archival retry and a separate-process completed retry'
s.report['qualificationMarker'] = 'npc-archival-passed.json'
for name in ['scripts/FuryCalmSmokeWorld/npc_archival.py', 'MudSharpCore/Character/CharacterArchiveService.cs',
             'scripts/FuryCalmSmokeWorld/OwnedBoneArmourRepair.cs',
             'DatabaseSeeder/Seeders/HumanSeeder/HumanSeeder.Bodyparts.cs',
             'DatabaseSeeder/Seeders/AnimalSeeder/AnimalSeeder.Races.cs',
             'MudSharpCore/Character/NpcArchiveReferencePolicy.cs', 'MudSharpCore/Character/NpcArchiveReferencePolicy.Agriculture.cs',
             'MudSharpCore/Character/NpcArchiveReferencePolicy.Combat.cs', 'MudSharpCore/Body/Implementations/Body.cs',
             'MudSharpCore/Body/Implementations/Body.Archival.cs']:
    s.report['inputs'][name] = hashlib.sha256((s.repo / name).read_bytes()).hexdigest()


def verify_retained_claims_and_absent_npc(life):
    identifier, character, body = life['id'], life['character'], life['body']
    s.check('captured NPC row is absent independently of ownership claims',
            s.sql(f'SELECT COUNT(*) FROM Npcs WHERE CharacterId={character}') == '0')
    s.check('exact ownership receipts survive physical archival',
            s.sql(f"SELECT Kind,EntityId,Role FROM MagicSpellOwnedEntities WHERE LifecycleId='{identifier}' ORDER BY Kind") ==
            f'1\t{character}\t0\n3\t{body}\t0')


try:
    receipt_path = pathlib.Path(os.environ['FUTUREMUD_NPC_ARCHIVE_SOURCE_RECEIPT']).resolve()
    s.check('retry receipt belongs to this exact owned world', receipt_path.is_relative_to(s.root) and receipt_path.name == 'receipt.json')
    prior = json.loads(receipt_path.read_text(encoding='utf-8'))
    s.check('retry consumes a stopped failed native receipt with passed payment cases',
            prior['status'] == prior['runnerStatus'] == 'FAIL' and prior['prepaymentCasesPassed'] and
            prior['cleanup']['mysqlStopped'] and len(prior['serverProcesses']) > 0 and
            all(x['returncode'] == 0 and x['collectorStopped'] for x in prior['serverProcesses']))
    s.report['sourceReceipt'] = dict(path=str(receipt_path), sha256=hashlib.sha256(receipt_path.read_bytes()).hexdigest())
    lives = prior['createdLifecycles']
    s.check('retry selects both exact paid lifecycle records', len(lives) == 2 and len({x['id'] for x in lives}) == 2)
    s.report['createdLifecycles'] = lives
    s.report['priorPrepaymentCasesPassed'] = True
    actors = json.loads((s.root / 'fury-calm-actors.json').read_text(encoding='utf-8'))
    s.caster = actors['caster']
    s.fury_id = actors['fury']
    original_hex = s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={s.fury_id}')
    s.check('original Fury definition matches the failed receipt restoration',
            hashlib.sha256(bytes.fromhex(original_hex)).hexdigest() == prior['originalDefinitionSha256'])
    if os.environ.get('FUTUREMUD_NPC_ARMOUR_SOURCE_RECEIPT'):
        repair = s.fixture('armour-repair')
        s.check('exact owned bone-armour repair passed with an idempotent rerun',
                repair['Status'] == 'PASS' and repair['ChangedRows'] in (0, 2) and repair['RerunChangedRows'] == 0)
        s.report['boneArmourRepair'] = {key: repair[key] for key in
                                      ['Status', 'SourceReceipt', 'SourceReceiptSha256', 'ChangedRows',
                                       'BuilderEditRefusals', 'RollbackProbe', 'RerunChangedRows']}
        s.report['boneArmourRepair']['receipt'] = str(s.runtime / 'fixture-armour-repair.json')
    s.report['archiveReferenceEvidence'] = {
        table: s.sql(f'SELECT Id,HEX(Definition) FROM {table} ORDER BY Id') for table in
        ['AgricultureOperations', 'AgricultureCropDefinitions', 'AgricultureFieldProfiles', 'ArmourTypes']}
    # Diagnostic census only: these digit collisions do not classify or exempt any column.
    # Keep values out of the receipt; the runtime's typed reference policy remains authoritative.
    columns = s.sql("SELECT TABLE_NAME,COLUMN_NAME FROM information_schema.COLUMNS "
                    "WHERE TABLE_SCHEMA=DATABASE() AND DATA_TYPE IN ('char','varchar','text','mediumtext','longtext') "
                    "AND (COLUMN_NAME LIKE '%Definition%' OR COLUMN_NAME IN "
                    "('EffectData','Data','Value','StateData','StateJson','ResultJson','WaitArgument','Tattoos',"
                    "'ExtraInformation','ProcedureParameters','OperationalPayload','CommandArguments','StrategyData','LandDetailJson')) "
                    "ORDER BY TABLE_NAME,COLUMN_NAME")
    token = re.compile(r'(?<![\d.])(?:' + '|'.join(str(x) for life in lives for x in [life['character'], life['body']]) + r')(?![\d.])')
    s.report['serializedDigitCollisionCensus'] = []
    for column in columns.splitlines():
        table, name = column.split('\t')
        if not all(re.fullmatch(r'[A-Za-z0-9_]+', x) for x in [table, name]):
            raise RuntimeError('Unsupported diagnostic metadata identifier')
        values = s.sql(f'SELECT HEX(`{name}`) FROM `{table}` LIMIT 10001')
        count = sum(bool(token.search(bytes.fromhex(value).decode('utf-8'))) for value in values.splitlines() if value != 'NULL')
        if count:
            s.report['serializedDigitCollisionCensus'].append(dict(table=table, column=name, rows=count))
    death_before = {}
    s.report['physicalEvidenceBeforeStart'] = {}
    s.report['initialJournalStates'] = {}
    for life in lives:
        identifier, character, body = life['id'], life['character'], life['body']
        s.check('receipt has an exact lifecycle UUID and positive physical IDs',
                str(uuid.UUID(identifier)) == identifier and type(character) is int and character > 0 and
                type(body) is int and body > 0)
        journal = s.sql(f"SELECT State,Mode,Reason FROM MagicSpellLifecycles WHERE Id='{identifier}'")
        s.report['initialJournalStates'][identifier] = journal
        s.check('exact temporary expiry journal remains pending or already completed', journal in ('2\t1\t0', '3\t1\t0'))
        s.check('exact native ownership claims are retained',
                s.sql(f"SELECT Kind,EntityId FROM MagicSpellOwnedEntities WHERE LifecycleId='{identifier}' AND Role=0 ORDER BY Kind") == f'1\t{character}\n3\t{body}')
        death_before[identifier] = s.sql(f"SELECT DeathObservedUtc FROM MagicSpellLifecycles WHERE Id='{identifier}'")
        s.check('previous native death is durably recorded', death_before[identifier] not in ('', 'NULL'))
        s.report['physicalEvidenceBeforeStart'][identifier] = s.sql(
            f'SELECT c.Id,c.BodyId,c.State,HEX(c.EffectData),HEX(b.EffectData),'
            f'(SELECT COUNT(*) FROM Bodies_GameItems WHERE BodyId={body}),'
            f'(SELECT COUNT(*) FROM CharacterBodies WHERE CharacterId={character} AND BodyId<>{body}),'
            f'(SELECT COUNT(*) FROM CharacterBodySources WHERE CharacterId={character} AND BodyId<>{body}),'
            f'(SELECT COUNT(*) FROM Characters WHERE Id<>{character} AND BodyId={body}) '
            f'FROM Characters c LEFT JOIN Bodies b ON b.Id=c.BodyId WHERE c.Id={character}')
    s.start()
    for life in lives:
        s.wait_npc_retired(life['id'], life['character'], life['body'])
        verify_retained_claims_and_absent_npc(life)
        s.check('archival retry preserves the original native death',
                s.sql(f"SELECT DeathObservedUtc FROM MagicSpellLifecycles WHERE Id='{life['id']}'") == death_before[life['id']])
    s.stop()
    completed = {life['id']: s.sql(f"SELECT * FROM MagicSpellLifecycles WHERE Id='{life['id']}'") for life in lives}
    archives = {life['id']: s.sql(f"SELECT * FROM CharacterArchives WHERE CharacterId={life['character']}") for life in lives}
    s.start()
    for life in lives:
        s.wait_npc_retired(life['id'], life['character'], life['body'])
        verify_retained_claims_and_absent_npc(life)
        s.check('cold completed retry preserves the exact lifecycle and archive rows',
                completed[life['id']] == s.sql(f"SELECT * FROM MagicSpellLifecycles WHERE Id='{life['id']}'") and
                archives[life['id']] == s.sql(f"SELECT * FROM CharacterArchives WHERE CharacterId={life['character']}"))
    s.stop()
    s.check('archival retry preserves exact original Fury definition',
            s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={s.fury_id}') == original_hex)
    s.report['fixtureRestoration'] = 'PASS: Fury definition unchanged; no payload or funding mutation'
    s.report['status'] = 'PASS'
except BaseException as error:
    s.report['status'] = 'FAIL'
    s.report['error'] = s.helper.redact(str(error), [s.connection, s.password])
    try:
        s.report['physicalEvidenceAfterFailure'] = {}
        for life in s.report.get('createdLifecycles', []):
            identifier, character, body = life['id'], life['character'], life['body']
            if (str(uuid.UUID(identifier)) != identifier or type(character) is not int or character <= 0 or
                    type(body) is not int or body <= 0):
                continue
            s.report['physicalEvidenceAfterFailure'][identifier] = s.sql(
                f"SELECT State,Mode,Reason,"
                f"(SELECT COUNT(*) FROM Npcs WHERE CharacterId={character}),"
                f"(SELECT COUNT(*) FROM Bodies WHERE Id={body}),"
                f"(SELECT COUNT(*) FROM CharacterInstances WHERE CharacterId={character} OR BodyId={body}),"
                f"(SELECT COUNT(*) FROM Characters WHERE Id={character} AND IsArchived=0 AND BodyId={body}),"
                f"(SELECT COUNT(*) FROM CharacterArchives WHERE CharacterId={character}) "
                f"FROM MagicSpellLifecycles WHERE Id='{identifier}'")
        if 'original_hex' in globals():
            s.report['fixturePreservedAfterFailure'] = s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={s.fury_id}') == original_hex
    except BaseException as evidence_error:
        s.report['failureEvidenceError'] = s.helper.redact(str(evidence_error), [s.connection, s.password])
finally:
    try:
        if s.process is not None:
            s.stop()
    except BaseException as cleanup_error:
        s.report['status'] = 'FAIL'
        s.report['cleanupError'] = s.helper.redact(str(cleanup_error), [s.connection, s.password])
    finally:
        s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
