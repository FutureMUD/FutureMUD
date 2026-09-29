# Monster AI v1 implementation and verification

Implementation date: 2026-09-29. Checkout baseline: `960155faa385d7d24dd34190ac5f2d707adc5db3`; local SDK `10.0.401`. This record describes the working-tree implementation, not a published or deployed revision.

## Delivered scope

- `AnimalAI` and `MonsterAI` are sibling controllers on `CreatureAIBase`. Shared mechanics include movement strategies, observation, assessment, home/refuge, pursuit, ambush, extraction and receipt-based trap/venom tactics. Animal ecology, group policy, existing XML and `AnimalHunt` persistence remain supported.
- Monster owns configurable motives, time/season/calendar/lunar/condition restrictions, allies/targets, warnings, provocation, cooldowns, bounded pursuit, home return, and optional Needs or AfterKill feeding. Off is the default feeding mode; attaching Monster does not change needs.
- Saved `MonsterIntent` and `MonsterState` use the existing effect store. Loading validates intent without executing attacks or payloads. No schema migration is required.
- Builders have `monster` registration, configuration/help, readiness checks, conflict guards and `impdebug monster`. Weapon and authored combat-power behavior uses existing combat. The previously empty `combat config psychic` handler now validates and saves its weight; weight rebalancing handles increases as well as decreases.
- The packs seed twelve additional stock Monster definitions. The source-generated recommendation manifest covers all 43 mythical and 46 supernatural races, with required setup and explicit limitations. Existing NPC assignments and Wildlife recommendations are retained.

See the [builder/runtime guide](../../Design%20Documents/AI/Monster_AI.md), [approved proposal](../../Design%20Documents/AI/Monster_AI_V1_Proposal.md), and [recommendation manifest](../../DatabaseSeeder/Assets/Manifests/Monster_AI_Recommendations.json).

## Automated results

All passing rows below have zero failures and zero skipped tests. The final Core and Library scripts both report stable source fingerprints.

| Check | Result | Evidence under repository root |
| --- | --- | --- |
| Full Core suite after final runtime changes | **4,056 passed** | `.artifacts/test-runs/20260929T132517Z-dd9c41ddb80d/summary.json` |
| Full Library suite after the shared movement interface change | **532 passed** | `.artifacts/test-runs/20260929T132502Z-b349cdd470f8/summary.json` |
| Full DatabaseSeeder suite | **1,441 passed** | `.artifacts/monster-implementation/full-tests/DatabaseSeeder/results.trx` and `console.log` |
| Focused movement, pathing, Monster, predator, scan and psychic configuration regressions | **186 passed** | `.artifacts/test-runs/20260929T132319Z-60418f1f4a9e/summary.json` |
| Targeted Monster catalogue and existing pack/wildlife regressions | **117 passed** | `.artifacts/test-runs/20260929T122853Z-f9e7301b62ee/summary.json` |

The full Seeder run used the already-built tested catalogue. Seeder source did not change afterward. The focused counts overlap their full suites and are not added to them.

Coverage includes Animal compatibility and needs opt-in; observed-target gates; no-needs Monster hunting; delayed/window-close cancellation, including door callbacks; missing references; actual custom-calendar dates; lunar/season combinations; per-NPC state; bounded after-kill native eating; detach ownership; XML round trips; saved intent without replay; and stock identity/rerun/custom-clone preservation in both pack orders.

The native investigation exposed three issues that were fixed and covered before the final passing Core run:

1. Monster's local candidate enumeration omitted visible targets on another layer. It now uses the shared hunt visibility gate over local characters and genuine scan observations.
2. An owned layer-preparation path could remain unfinished with ambient wandering disabled. Monster now completes its owned path while the activity window permits it.
3. AI path planning called execution-only `CanMove` for later graph edges, which rejected their remote origins. `CanMoveForPathPlanning` shares the physical checks while permitting inspection of a later edge. Actual execution retains the ordinary origin/access checks. Tests use real Character movement methods, a two-exit route, changed physical conditions and safe-water restrictions. See [Pathfinding System](../../Design%20Documents/World/Pathfinding_System.md) for the current-state estimation limitation.

## Native evidence and persistence

The owned, isolated MySQL world is retained at `.artifacts/authored-celestials/run-95a7199239bc`. Its `native-replay.json` records a successful fresh Debug replay, including both creature packs, and a second replay refusing the nonblank database without mutation. The before/after digest is identical. All twelve stock Monster definitions were independently queried from MySQL. No existing user database or service was used.

The latest evidence directory is `monster-6ea34a24` under that root. `requests.jsonl`, `responses.jsonl`, `receipt.json`, the redacted transcript and `server-output.txt` retain the commands, assertions and native actions. The prior directory `monster-158a3827` supplies the pre-restart persistence read. `cleanup.json` confirms that the owned MySQL instance was stopped and its data preserved; the game received a normal flush and `shutdown stop`.

**Native status: all six scenarios have successful observations and follow-up checks, but the diagnostic operator's aggregate receipt remains FAIL.** It deliberately retains two earlier failed probes and the resulting aggregate failure. Those probes are not relabelled as passes. The table identifies the later evidence resolving them; this is not a claim of an entirely green unattended native run.

| Scenario | Successful evidence in `monster-6ea34a24/responses.jsonl` |
| --- | --- |
| No-needs night hunter | Request 21: inactive Morning restriction remains idle with `NoNeeds`; switching the configured band to local Night starts Scheduled native combat; closing it abandons pursuit. |
| Guardian warning, leash and home return | Request 18: native warning, Territory engagement, then idle/no combat after moving the guardian beyond its leash. Request 25: after flush and ordinary travel time, independent SQL confirms NPC 6 at home cell 4 following the two-exit route from cell 2. |
| Aquatic ambush | Requests 15–17: approved Crocodile fixture with its own Monster clone, underwater staging, hiding, native ambush strike, and successful opposed seizure. The transcript reports the fresh crocodile seizing its opponent following its ambush; diagnostics show Fighting and a real grapple. No synthetic capture receipt or granted attack was used. |
| Armed sentinel | Requests 4–7: real sword wielded, native sword attack, disarming, then native unarmed attack under the configured fallback. |
| Authored power and resource fallback | Requests 9–13: native Concussive Pulse, independent Focus balance `96` after starting at `100`, and zero currently usable powers after setting Focus to zero. Requests 22–23: with mixed 50% psychic/50% natural weights and target permissions explicitly configured, the resource-exhausted Monster executes a physical attack. |
| Saved relentless pursuit | Prior directory `monster-158a3827`, requests 36–37: NPC 25 saves AI 114, target 3, origin 2, Approach and deadline `2026-09-29T13:53:05.6583411Z`. Latest requests 1–2 retain those exact fields after restart, with no combat at the first diagnostic. Request 9 shows Opening/native combat on ordinary runtime events, with the same target and deadline. |

The two retained failed probes in the latest session were:

- Request 3 inspected the guardian before a warning appeared. The later request 18 allowed settling time and recorded the native warning, subsequent engagement and leash cleanup. Request 25 verified the completed home route.
- Request 14 expected a physical attack while the target was critically injured and the combat settings still vetoed attacking critically injured opponents. Requests 22–23 explicitly configured that native permission and recorded the physical attack. Monster does not override combat target permissions.

The source-linked follow-up receipt is `.artifacts/monster-implementation/native-acceptance-review.json`. It verifies the successful assertions and persistence comparison above while retaining the raw session's failed status and failed probe names.

## Earlier failures retained

- The first extraction test run exposed two source-location fixtures that needed to follow the extracted ownership. A new bounded-feeding mock also required correction. Subsequent focused/full tests passed with their behavioral assertions retained.
- Two new movement-test builds failed before execution because imports were missing (`20260929T131612Z-6119914613c1`, `20260929T131712Z-6659a724cbd3`). These are not passing tests.
- Focused run `20260929T132151Z-e7318c37a62c` had 185 passes and one predator-fixture failure: the test depended on another test initializing the static combat stamina expression. The fixture now sets and restores that expression; the same filter then passed 186/186.
- Full Core run `20260929T132338Z-66ea3047ae6c` executed 4,056 passing tests but remains **INCONCLUSIVE** because documentation changed during its source-fingerprint interval. The stable repeat listed above is the accepted result.
- Native directories `monster-12d51882` and `monster-4093423e` retain incomplete runs, including loading an older approved Crocodile fixture without a valid ethnicity. The valid edited revision was subsequently approved before loading. This was fixture setup, not a Monster runtime change.
- Native directory `monster-50d4aaaa` retains the investigation that found the three runtime issues listed above, plus a short power-fallback observation failure. It is not an accepted aggregate pass.
- Native directory `monster-158a3827` retains premature home-location observation, old dead sparring targets, and an observer on the wrong room layer. Corrected observations showed the actual two-exit return, sword/unarmed attacks and resource-exhausted physical combat. Its aggregate failure is preserved.

## Limits

- The native schedule scenario changed configured bands against frozen local Night. It did not demonstrate a naturally advancing celestial transition. Calendar, season and moon combinations have automated coverage.
- Native acceptance demonstrated the aquatic variant. Full trap/web construction, aerial carrying/dropping, and venom withdrawal were not each replayed in this Monster smoke; their shared mechanics and prerequisites retain existing regression coverage.
- The native pack replay exercised its supported install order. Both stock-profile reconciliation orders, ID stability and clone preservation were checked in automated tests; no second full native world was provisioned solely to reverse pack order.
- This adds integration with authored combat powers, not a general autonomous spellbook planner, lore-specific resurrection, blood feeding or Monster group AI.
- No full-solution/VSIX, climate, release, deployment or hosted CI run is claimed.
