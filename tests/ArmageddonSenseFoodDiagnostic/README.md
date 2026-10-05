# Active-Sense food diagnostic

This capsule qualifies the root-owned created-item removal adapter against cleared
installer `37de143f202520a4a6f95d27889c2f521f833d04`. It imports no installer or stock
runtime into the repair branch. The dedicated installer harness supplies the real
paid Sense cast, installed Sustain Meal, native Character/Body/GameItem/needs and
disposable MySQL persistence; world/check catalogues remain controlled fixtures.

Create a fresh detached local worktree of that exact installer commit next to this
checkout. Read its applicable AGENTS instructions, then run:

```powershell
python -B tests/ArmageddonSenseFoodDiagnostic/prepare_fixture.py ../SenseFoodDiagnostic
dotnet build '../SenseFoodDiagnostic/tests/ArmageddonTraditionNativeHarness/ArmageddonTraditionNativeHarness.csproj' -m:1
$env:TEMP = 'C:\Users\luker\AppData\Local\Temp'
$env:TMP = $env:TEMP
```

Use a short writable system temp directory appropriate to the execution account.
The MySQL runner creates and verifies its own unique instance there. A long temp
path can exceed Windows' table-file limit and fail snapshot import before gameplay.
Do not substitute another server or alter the snapshot to get past that failure.

Configure Git `safe.directory` for the diagnostic checkout if its ownership requires
it, then invoke its existing runner with a new, immutable evidence destination:

```powershell
../SenseFoodDiagnostic/tests/ArmageddonTraditionNativeHarness/Run-IsolatedTraditions.ps1 -Mode custody -EvidenceRoot <owned-evidence-directory>
```

Restore the calling process's environment afterwards. Preserve the runner's source
start/end manifests, assemblies, native log and owned-instance cleanup receipts.
The preparation script replaces only the exact adapter, the custody scenario and
reader routing, and adds this native partial. It refuses an unexpected revision or
tracked work. The production source bytes and native capsule must match the repair
checkout; never edit or rebuild tested inputs during a run.

The unchanged installer custody mode prints `BLOCKED` and exits zero when it
successfully reproduces the historical bug. A prepared capsule must instead pass
its `ARMSENSE` assertions. Native evidence from this installer scaffold does not
certify a combined root/installer branch, Telnet login, all stock profiles or the
full Armageddon acceptance matrix.
