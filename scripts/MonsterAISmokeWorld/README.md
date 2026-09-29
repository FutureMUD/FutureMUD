# Monster AI native acceptance

Run from the repository root on Windows with MySQL 8 installed:

```powershell
python -B scripts/AuthoredCelestialSmokeWorld/native.py --smoke-script scripts/MonsterAISmokeWorld/smoke.py
```

The existing provisioner creates a new loopback MySQL instance, imports the blank snapshot, applies migrations and runs the full Debug seeder replay. Its second replay must refuse the nonblank database without mutation. The Monster operator then boots the current engine through the `futuremud-mud-tester` session helper. It uses the disposable replay account, redacts credentials, and never connects to a user's existing database.

This is an adaptive acceptance operator, not an unattended passing test. It requires the scenario commands and assertions below. The owned run's `monster-runtime.txt` identifies its evidence directory. Append one complete JSON object per line to that directory's `requests.jsonl`; read `responses.jsonl` for the resulting command output. Commands are retained in the redacted transcript. SQL requests allow only independent `SELECT`/`SHOW` reads and verify the instance data directory before each request.

```json
{"commands":[{"text":"impdebug monster here","expect":"Monster","label":"controller diagnostics available"}]}
{"sql":"SELECT COUNT(*) FROM ArtificialIntelligences WHERE Type='Monster' AND Name LIKE 'Monster - %'","expect":"^12$","label":"all stock profiles persisted"}
{"restart":true}
```

Commands can specify `seconds` (maximum 30) for their bounded response wait. A request can use `read` for a later output observation with an `expect` regex and `label`. For probabilistic combat, use `{"observe":{"seconds":45,"expect":"<native action regex>","label":"<assertion>"}}`; it accumulates new output until the assertion matches or the bounded interval (maximum 60 seconds) ends. An unsuccessful assertion is preserved in the receipt; continuing investigation never converts it into a pass. Finish a clean acceptance run with `{"finish":true}`. The operator has an 880-second deadline including setup, and the provisioner has its own bounded startup/replay/smoke limits. It flushes and shuts down its own game process; the provisioner stops its own MySQL process and retains data/evidence.

To resume only a stopped database previously created by this provisioner, use `python -B scripts/MonsterAISmokeWorld/resume.py --run-dir <owned-run-directory>`. It validates the owned directory, replay receipts, free port and MySQL data-directory identity before operating. It creates a separate Monster evidence directory and preserves earlier failures.

## Required scenarios

Use owned QA templates, cloned AIs and an invisible administrator. `npc make me <name>` creates a current simple humanoid template; `npc edit <id>` creates a draft revision. Submit and approve that revision before `npc load <id>`, which otherwise loads the previous approved version. After changing race, bind its valid ethnicity and culture, randomise its body, and then override test descriptions/attributes (use aliases such as `str`, `dex`, `con`). Give prey no primary AI. Publish the cells' overlay package before testing routes. Do not attach Animal and Monster controllers together.

Use `goto <unique NPC keyword>` to put the observer on that NPC's layer. Numeric `goto <cell>` preserves the observer's current layer, including when leaving an underwater test. Stage equipment with `peace` followed by `peace permanent`; loading prey can otherwise start combat before the weapon is wielded. Remove the restriction with `peace off`. Use fresh, living targets and inspect normal combat permissions (`attack_helpless`, `attack_critical`) when observing fallback. Allow actual movement and combat recovery time: an immediate diagnostic snapshot or a short quiet interval does not establish failure. Numeric target selectors in ordinary staff commands are not necessarily character IDs; `impdebug monster` explicitly accepts IDs.

1. **No-needs night hunter:** clone Night Stalker, configure allies/eligibility for the fixture, and test a nonmatching activity band followed by the actual local band. Verify NoNeeds remains selected and a scheduled intent engages through normal combat. A frozen-clock test may switch the configured bands against the actual local Night; it does not prove a natural celestial transition. Calendar/time/season combinations have separate unit coverage.
2. **Guardian:** bind a stable home, set a warning and short leash, introduce eligible prey, observe the warning before an intent, then move outside the leash and verify bounded abandonment/return. No teleportation is an AI action.
3. **Trap or aquatic hunter:** configure a real owned capture or a race with authored aquatic ambush/extraction attacks. Verify the relevant receipt/native combat action. Mere proximity or configuration is insufficient.
4. **Armed sentinel:** use weapon-capable anatomy, a real wielded weapon and compatible combat settings. Observe a normal armed attack; remove the weapon and inspect/use the configured native fallback.
5. **Authored power user:** grant an actual existing combat power/capability, appropriate skill, combat weights and resources. Observe native power selection/resolution and resource debit, then unavailability/fallback when resources are exhausted. A nonzero power count alone is insufficient.
6. **Relentless pursuer and restart:** create a bounded active intent, flush it, read its saved XML independently, restart, and compare identities/deadline/phase. No attack, capture payload or resource debit should replay on load. Resume only on an ordinary event; show stale/expired intent cleanup as appropriate.

After the required assertions for a scenario, append `{"completedScenario":"<scenario name>"}`. This marker only groups the human-reviewed evidence; it does not replace the assertions. PASS requires six distinct scenario markers, every recorded assertion passing and graceful shutdown within the deadline. A failed fixture setup should be repaired in a fresh owned run or retained as a failure with a separate follow-up receipt.

The durable evidence is `receipt.json`, `requests.jsonl`, `responses.jsonl`, `transcript.txt`, the MySQL data and provisioner's replay/cleanup receipts. See `Verification.md` for the implementation run's actual outcomes and limitations.
