# Five stock spell integration checkpoint

This lane starts exactly at local `1ad24d636925da64e7a82ffa858b0af470e56b0a` in a new isolated branch/worktree. It supplies ordinary editable stock definitions for Sense Enchantment, Unravel Enchantment, Mend Flesh, Draw Water and Hovering Light. It does not install a repertoire or update the central completion ledger. The reporting dependency is the combined `48132987` and `3fe9968f` checkpoints, independently cleared by the parent within their bounded reporting scope.

## Authority and recovered rules

The authoritative brief is Library `libfile_a4606cd0097081918d6c9d9e220e6da0`, `Armageddon_Magic_Completion_Implementation_Brief.md`, underlying file `file_00000000273481fab2e542daf8801228`, reported version 0, 905 lines. Supported Library reads covered 1–300, 301–600 and 601–905 through EOF. The supported materialization helper downloaded but failed on Windows `os.setxattr`; no cloud path was assumed, helper bypassed, temporary bytes substituted or transfer retried around that failure. Content reads supplied authority with retained identity. Local transfer diagnostics remain outside the repo in the lane's authority directory.

Historical C is Library `libfile_befb4ae78d1c8191aaa2640c49912d9d`, `codedump.c`, underlying file `file_00000000438871faac40c5042679ce3a`. Source rules were recovered through supported reads rather than inferred from catalogue proposals. Source-tree opening/cap/minimum metadata comes from the committed Sorcerer source roster. The brief's completion contracts govern the bounded native adaptations below.

| Stock | Recovered source evidence | Native contract and deliberate adaptation |
| --- | --- | --- |
| Sense Enchantment | `spell_detect_magick` 84616–84650; direct wrapper around 217970. Silt refuses; character detection lasts five game hours per level; stacking caps at 48 hours. Opening 60, cap 90, printed minimum 7. | Character detection grants native magical perception. Lifetime `3000*grade` real seconds uses nominal 600-second game hours. An exclusive parent refreshes instead of accumulating historical duration. Editable terrain support prog maps Silt. |
| Unravel Enchantment | `spell_dispel_magick` full 84900–85900 region and wrapper 218147–218191. Many character effects lose `level*RT_ZAL_HOUR`; assorted object spells extract objects or strip flags, with other bespoke branches. Opening 60, cap 90, minimum 7. | Character-targeted native matching-key contest and removal, as required by the completion brief. Default any caster/key, hostile allowed, contest on. Builders can narrow keys, spells, schools, tags and caster policy or use native shorten mode. No broad historical flag/object/topology/undead-damage parity is claimed. |
| Mend Flesh | `spell_heal` 88877–88913, wrapper 219754–219785. Undead refuses; Nilaz excludes non-defilers; Fire reduces level, Water raises it; healing is `level²*random(1,3)` hit points. Opening 30, **cap 60**, minimum 20, source parent Still Anger raw 80. | Native eligible wounds worst first with overflow and deterministic mean budget `2*grade²` damage. Builder-selected boolean `(target, caster)` eligibility prog explicitly owns undead/Nilaz/defiler category mappings. No hardcoded invented native race/guild IDs. Terrain level shifts and HP parity are replaced by the declared native wound budget. Cap 60 belongs to the casting admission; practice has no special maximum and reaches grade 7. |
| Draw Water | `spell_create_water` 83644–83695 and wrapper 217482–217539. Fire Plane refuses; five units/level, doubled in Water Plane; capacity clamps. Silt/incompatible liquid produce slime. Separate ground and thirst routes exist. Opening 30, cap 90, minimum 7. | Accessible open owning drink container, clean selected liquid, `0.5*grade` litres and an optional selected plane ×2, capacity clamp. The 0.1-litre/unit mapping is authored native scale. Incompatible liquid and Silt refuse before payment instead of destructive slime conversion. No ground/thirst route. Ordinary liquid consumption/persistence; no magical expiry. |
| Hovering Light | `spell_ball_of_light` 82299–82445 and wrapper 216907–216944. One guild/tribe/environment-selected light, auto-worn about head or dropped at feet; consumes flag; timer three game hours/level. Opening 30, cap 90, minimum 7. | One selected approved native wearable prog-light, worn-light placement, immutable `1800*grade` cleanup deadline. Native slot admission refuses instead of ground fallback. Unwearing preserves the deadline rather than emulating historical consumes-on-removal. Historical object profiles/luminosities are unavailable; the native fixture's 40 lux is labeled authored content. |

The earlier generic Hovering Light fixture using `600*grade` and old catalogue proposals are superseded for this installed stock. No unavailable historical object stats, guild IDs, water/slime recipes or HP-to-wound conversion are presented as recovered facts.

## Builder construction

Use ordinary `magic spell edit new stock` commands, quoting names with spaces:

```text
sense-enchantment <school> <character casting skill> <resource>
unravel-enchantment <school> <character casting skill> <resource>
mend-flesh <school> <character casting skill> <resource> <boolean eligibility prog>
draw-water <school> <character casting skill> <resource> <clean water liquid> <bonus plane|none>
hovering-light <school> <character casting skill> <resource> <approved wearable prog-light prototype>
```

Factories validate selected world definitions and persist spell/expression/support-prog rows in an independent transaction. They refuse duplicate stock names before changing content. Definitions use ordinary triggers, effects, grade profiles, practice plans, costs and lifecycle APIs. No alternate cast implementation or acquisition installer is supplied. Builders edit native effects, trigger filters, source efficiency, duration, practice, compatible liquids and prototypes normally.

Add each spell to the intended casting capability with relative grades: Sense/Unravel opening 60 cap 90; Mend opening 30 cap 60; Draw/Hover opening 30 cap 90. Bind Mend to its own spell skill to retain cap60; the existing casting API intentionally uses the highest legitimate cap when multiple entries share one native trait. Supply legitimate acquisition separately. Native efficiency uses controlled/requested grade and the printed minimum, rather than treating the fallback cost expression as the source curve. Stock overreach ×1.5 needs 112.5 energy for the next grade. The native harness uses ordinary attribute binding `80+2*variable`, raw attribute 19, capacity 118, with full disposable fixture credits; it does not change stock cost policy to fit the older 100-point fixture.

## Verification and integration boundaries

Entry-specific JSON receipts bind the final source, built assemblies, tests, native output and explicit limits. Failed diagnostic attempts are retained under `.artifacts/test-runs/five-stock-reporting-history` and never relabeled as passing evidence. Native fixtures use actual spell builders, casting service, resource payment, skills/improvers, wounds, items/components, owned lifecycle rows and MySQL stores; unrelated world catalogues/check rolls are controlled. Real wall-clock gameplay, full boot/world installation, historical object statistics, PC/NPC category mappings and every outcome/resistance branch are outside the qualification. Practice clock acceleration is explicit.

The dedicated `Temporary Scratch App/FiveStockNativeHarness` project compiles existing harness partials plus its owned partial and startup class. It retains the existing friend assembly name inside a separate project/output directory. Its runner uses a unique `futuremud-five-stock-mysql_<guid>` temporary directory, random loopback port, server UUID/datadir checks and owned DB markers. Each fresh DB has the existing permitted `futuremud_land_<timestamp>_<random>` prefix inside that isolated server. Reader processes and output paths belong to this run; no user database or other lane process is used. Shutdown and deletion require exact ownership checks.

```powershell
dotnet build 'Temporary Scratch App/FiveStockNativeHarness/FiveStockNativeHarness.csproj' -c Debug -m:1 -p:UseSharedCompilation=false
& 'Temporary Scratch App/FiveStockNativeHarness/Run-FiveStockAcceptance.ps1' -Mode five-stock
& 'Temporary Scratch App/FiveStockNativeHarness/Run-FiveStockAcceptance.ps1' -Mode regressions
```

The ordinary builder dispatch has only an early utility-stock hook and expanded stock-name help. The shared native harness dispatch and runner are unchanged. No shared dispatch addition is required when retaining the dedicated project. If integration prefers the existing project, add the lane partial to that project's compile items and route `--five-stock-run`/`--five-stock-reader` to it in the coordinator-owned dispatch; do not duplicate the startup class. The source registry/repertoire/completion-ledger integration is coordinator-owned and remains outstanding for parent review.

## Final checkpoint evidence

The frozen final repository fast run passed all 7,285 tests across 10 projects, with zero failures/skips and source_stable true. The final dedicated five-stock native entrypoint passed all five low/high paid casts, actual reporting, refusal and persistence checks, Mend paid native practice through grade 7 at raw60, prepared-light mastery/quarantine, fresh-process refusal of a completed unproven mutation, and exact expiry/custody/water conservation. Final four-stock regressions are bound separately in the verification receipt. Assemblies are fingerprinted before and after the final native runs; raw source SHA256 and canonical Git blob hashes bind committed code.

The reporting-test-only factory is removed in TestCleanup, with DoNotParallelize, so the full-suite compatibility registry stays unchanged. Earlier failed fixture diagnostics, source-changed test windows, a busy test run and a DLL-copy lock failure remain retained and excluded from final qualification. No production API changed after the parent-cleared reporting dependency.
