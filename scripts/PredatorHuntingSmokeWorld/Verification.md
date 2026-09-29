# Predator hunting verification record

Implementation and native checks were performed on 2026-09-28/29 before publication, in the working tree based on `cacbd2d26057cfdaaabf9532c6d4e210c50f9f10`. The [feature guide](../../Design%20Documents/AI/Predator_Hunting.md) describes the final builder contract; the [harness guide](README.md) separates automated and manually operated scenarios.

## Native evidence

All recorded native sessions used the disposable, loopback-only `authored_celestial_smoke` instance rooted at `.artifacts/authored-celestials/run-b2339e23dfa8`. The installed MySQL service and other game databases were not used. Full logs, transcripts, data and receipts are local artifacts, not repository fixtures.

| Check | Result and evidence within that run directory |
| --- | --- |
| Fresh installation | PASS: `native-replay.json` records a successful complete medieval replay and a second replay refused without table mutation. |
| Builder/XML/restart | PASS: `predator-e51d0861/receipt.json` and `transcript.txt` cover nine ambush race links, three giant venom anatomy pairs, cloned AI edits, independent saved XML and reload. This early receipt predates later combat/content fixes; it is evidence for its named builder/persistence assertions. |
| Saved hunt intent | PASS: `predator-operator-1790641759` verified the same target and deadline after restart without replaying an attack during loading. |
| Atomic ambush | PASS: `predator-operator-1790642311` exercised a tree-to-ground defended strike with an opposed initial grip in one combat action. |
| Tree extraction and follow-through | PASS: `predator-operator-1790674531/transcript-current.txt` records Leopard #2 against healthy Rabbit #16: ambush, grapple extension, haul, continued climbing attacks, death and feeding. After a flush, independent MySQL reads placed both at cell 2/layer 4; the leopard's posture was 15 (Climbing), and diagnostics showed an active StandardMelee fight with controlled grip. The preceding run `1790673948` exposed the obsolete Skirmisher default; that result alone was not accepted as complete follow-through. |
| Restored web and home path | PASS: `predator-operator-1790673948/transcript-current.txt` starts with the saved web armed with one charge. Spider #6 returned from cell 7 to its cell 6 web through normal movement, waited, and attacked healthy Rabbit #9 after it entered from the east. The web was spent; the log records a successful envenoming move, withdrawal and a second attack after incapacity. The effect inspection was after the 20-second restraint expired; direct inspection of a live restraint is covered by runtime tests, not claimed from that transcript. |
| Aerial ascent and release | PASS: `predator-operator-1790672973/Console Log 2026 September 29 09 09 48.txt` records repeated complete sequences of four pulls at five stamina each followed by a zero-stamina DropGrappledTargetMove. Independent MySQL inspection placed Rabbit #13 at layer 7 (HighInAir) after release; the eagle had begun descending. Ground-level output shows only the lower portion of the fall and is not used to infer maximum altitude. |
| Venom withdrawal and return | PASS: `predator-operator-1790670937/transcript-current.txt` records Viper #11 delivering venom to Rabbit #8, retreating from cell 4 to 5, entering Shadowing out of combat and returning to attack after observed incapacity. Independent body-dose reads found positive Viper Venom quantities (approximately 0.484245 g and 0.015625 g). Those administrative reads were evidence collection, not AI inputs. |
| Native content reconciliation | PASS: `content-reconciliation.json` in `predator-operator-1790674531` records two stable production reconciliation calls, canonical air, prone venom, stock approach settings and organic corpse maps. Independent reads also confirmed Leopard and Panther now default to Beast Brawler. |
| Cleanup | PASS: final operator `1790674531` flushed saves and shut down the MUD gracefully; the owned MySQL process stopped, with data preserved and `cleanup.json` written. The combat timing override was restored to 1 before shutdown. |

The live fixtures used high relevant skills, active hours and shortened combat/engagement delays. Some acquisition thresholds and pursuit deadlines were relaxed to isolate physical behavior; policy and default-threshold behavior are tested separately. Existing NPCs were explicitly assigned the repaired Beast Brawler/Clincher setting because changing a race default does not replace an existing NPC's selected setting. Old prey were administratively removed only between scenarios; Rabbit #16's final death and feeding were autonomous.

## Automated evidence

Final owning-suite results total **5,907 passed**, with no failures or skips in the final result for each project. Full receipts live beneath `.artifacts/predator-hunting/tests`, including source fingerprints, native logs and TRX results.

| Suite | Final result | Run |
| --- | --- | --- |
| MudSharpCore Unit Tests | 3,948 passed, 0 failed, 0 skipped | `20260929T095327Z-ed6d11244bca` |
| FutureMUDLibrary Unit Tests | 524 passed, 0 failed, 0 skipped | `20260929T094223Z-bcba63b41b8b` |
| DatabaseSeeder Unit Tests | 1,435 passed, 0 failed, 0 skipped | `20260929T094750Z-6bcb3095a36c` |

Each execution recorded stable source. The earlier combined run `20260929T094223Z-bcba63b41b8b` was an overall FAIL because the mythic-template test still expected Giant Spider's former Skirmisher default. Its library result passed independently. After updating that expectation and aligning the fresh Yacumama template with its reconciled Drowner default, the entire seeder suite passed. The core suite was rerun after adding three underwater-to-surface ingress cases for legal swimming, a non-swimmer and blocked ascent; all passed. No production runtime code changed after the live checks.

The final seeder gate also covers the post-live preservation guards for custom Dropper/Drowner defaults. The native harness build exited 0 after that seeder gate. Both Python harnesses passed syntax checks, new source files passed whitespace checks, documentation links resolve, and `git diff --check` passed. This verification record was updated after execution; no executable inputs were edited during a run.

Prior focused gates passed:

- `20260929T092055Z-b8cce0856a24`: 159 runtime tests, no failures or skips. This includes the return-home, teleport-layer, restored trap receiver, combat target retention and approach regressions.
- `20260929T093433Z-084cd1641d1a`: 35 seeder tests, no failures or skips, including the leopard/panther default correction. The subsequent custom Dropper/Drowner default-preservation cases are included in the final owning-suite run.
- The Debug native harness built successfully with `--no-restore -m:1`; `.artifacts/predator-hunting/harness-build.log` retains the output.

Earlier failed fixture assertions and the earlier timed-out run were investigated and retained in local artifacts. They are not counted as passing verification. Independent code review found issues in combat approach fallback and fresh-context default preservation that were repaired and retested; its final bounded return-home review found no actionable defect.

## Coverage boundaries

- The crocodile underwater variant has automated swimming/ascent eligibility, shared layer/permission, strategy and content coverage, but no completed native crocodile fight in this task. The live extraction proof is the leopard; flight, web and venom have separate live proofs above.
- No native performance/load run or every-species balance sweep was performed. The assessment is an observable heuristic, not a calibrated win probability.
- Spatial routes, group ownership, lost-target pursuit and negative policy cases have automated coverage; the live scenes use ordinary cells and individual NPCs.
- No schema change is required: hunting configuration and per-NPC intent use the existing XML/effect persistence. Old AnimalAI definitions without Hunting stay on their legacy path.
