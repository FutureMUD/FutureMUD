"""Apply the exact repair and native fixture to a disposable cleared-installer worktree."""
from pathlib import Path
import hashlib, json, subprocess, sys

source = Path(__file__).resolve().parents[2]
destination = Path(sys.argv[1]).resolve()
base = '37de143f202520a4a6f95d27889c2f521f833d04'
git = ['git', '-c', 'safe.directory=' + str(destination).replace('\\', '/'), '-C', str(destination)]
assert destination != source and destination.parent == source.parent
assert subprocess.check_output(git + ['rev-parse', 'HEAD'], text=True).strip() == base
assert not subprocess.check_output(git + ['status', '--porcelain', '--untracked-files=no'], text=True).strip()
def sha(p): return hashlib.sha256(p.read_bytes()).hexdigest()
def original(path): return subprocess.check_output(git + ['show', base + ':' + path])
runtime = 'MudSharpCore/GameItems/GameItem.SpellRemoval.cs'
assert (destination/runtime).read_text(encoding='utf-8-sig') == original(runtime).decode('utf-8-sig').replace('\r\n', '\n')
fixture = 'tests/ArmageddonTraditionNativeHarness/Program.Traditions.Provisions.cs'
text = (destination/fixture).read_text(encoding='utf-8-sig')
start = text.index('\t\tif (reproduceCustody)\n')
end = text.index('\t\t// Clear the earlier paid Sense effect', start)
text = text[:start] + '''\t\tif (reproduceCustody)
\t\t{
\t\t\tQualifySenseFood(host, database, fixture, utilities, utilityIds, ids, installed,
\t\t\t\t(spell, target) => Cast(spell, target), CreatedFood);
\t\t\treturn;
\t\t}
''' + text[end:]
entry = destination/'tests/ArmageddonTraditionNativeHarness/Program.Traditions.cs'
entry_text = entry.read_text(encoding='utf-8-sig')
anchor = '\t\t\t"--traditions-custody-reader" => ProvisionCustodyRestart(args[1]),'
assert entry_text.count(anchor) == 1
entry_text = entry_text.replace(anchor, anchor + '\n\t\t\t"--sense-food-reader" => SenseFoodRestart(args[1]),')
# Only this custody scenario and its added cold reader change. No installer/runtime stock integration.
(destination/runtime).write_bytes((source/runtime).read_bytes())
(destination/fixture).write_text(text, encoding='utf-8')
entry.write_text(entry_text, encoding='utf-8')
native = destination/'tests/ArmageddonTraditionNativeHarness/Program.SenseFood.cs'
assert not native.exists()
native.write_bytes((Path(__file__).parent/'Program.SenseFood.cs').read_bytes())
print(json.dumps({'base':base,'repair_sha256':sha(source/runtime),'diagnostic_repair_sha256':sha(destination/runtime),
 'native_fixture_sha256':sha(native),'diagnostic_root':str(destination),
 'changed':[runtime,fixture,str(entry.relative_to(destination)),str(native.relative_to(destination))]},indent=2))
