"""Check archived bytes and report source changes; this is not a database import test."""
import base64
import hashlib
import json
from pathlib import Path

root = Path(__file__).resolve().parents[2]
corpus = root / 'Design Documents/Seeding/CultureSeederOriginalCorpus'
inventory = json.loads((corpus / 'inventory.json').read_text(encoding='utf-8'))
source_bytes = (corpus / 'original_sources.json').read_bytes()
assert hashlib.sha256(source_bytes).hexdigest() == inventory['originalSourcesSha256']
original = json.loads(source_bytes)
changes = []
for entry in original['files']:
    preserved = base64.b64decode(entry['bytes_base64'], validate=True)
    assert hashlib.sha256(preserved).hexdigest() == entry['sha256'], entry['path']
    current = root / entry['path']
    changes.append({
        'path': entry['path'], 'original_bytes_recoverable': True,
        'original_sha256': entry['sha256'],
        'current_sha256': hashlib.sha256(current.read_bytes()).hexdigest() if current.exists() else None,
        'source_file_changed': not current.exists() or current.read_bytes() != preserved,
    })
totals = {}
for entry in inventory['generatedPacks']:
    data = (corpus / (entry['pack'] + '.json')).read_bytes()
    assert hashlib.sha256(data).hexdigest() == entry['sha256'], entry['pack']
    tables = json.loads(data)['Tables']
    for table, count in entry['tables'].items():
        assert len(tables[table]) == count, (entry['pack'], table)
        totals[table] = totals.get(table, 0) + count
report = {
    'scope': 'Archived source bytes and generated source-pack-local tables; shared defaults are counted in each source pack. No proof of active database reconciliation.',
    'source_commit': inventory['sourceCommit'],
    'original_source_files_recoverable': len(changes), 'generated_source_packs': len(inventory['generatedPacks']),
    'source_file_changes': changes, 'source_pack_local_totals': totals,
}
destination = root / 'Design Documents/Verification/CultureSeeder_Redesign_Source_Preservation.json'
destination.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
print(f"{len(changes)}/{len(changes)} original files recoverable; {len(inventory['generatedPacks'])} archived packs checksum-matched.")
print(f"{totals['RandomNameProfile']} source-pack-local profiles; {totals['RandomNameProfilesElements']} name elements; {totals['Accent']} accents.")
print(f"Changed original source files: {sum(x['source_file_changed'] for x in changes)}")
