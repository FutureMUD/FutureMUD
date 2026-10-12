# Armageddon S3 catalogue completion — 12 October 2026

For a new task, start with the [S4 continuation handoff](Armageddon_S4_Continuation_Handoff_20261012.md). Post-publication checks of commit `8d503f6e789e5d68e03fceb4ae64b0a66f1fabd3` exposed two missed Seeder export-baseline expectations (248 types versus the actual 249). Both Linux and Windows failed those tests; one duplicate Windows job was cancelled. The handoff records exact tests/paths and the required repair before S4. The local passes below retain their original dated scope; final hosted/all-project success is not claimed.

The approved S3 catalogue is implemented and qualified within the boundaries below. The composite installer connects S1 fixed settings and S2 progression to every one of the **198 S3 blueprints**. The ten foci, two device blanks and their twelve recipes remain in S4 with real spell dependencies. Full spell hydration, normal-player progression/cost qualification, the S6 four-cell fresh-install matrix and N24 remain separate gates. Release entry, staged spell admissions and Class availability remain disabled.

## Installed catalogue

| Native stock | Count |
| --- | ---: |
| Supply item prototypes and native recipes | 64 each |
| Component tags / finite component-blank forage | 61 / 1 |
| Shelter autobuilder templates | 4 |
| Melee item prototypes, weapon types, attacks and combat messages | 49 each |
| Weapon damage/pain/stun expressions | 147 |
| Food prototypes / empty supply bottle | 3 / 1 |
| Wine / retained native water binding | 1 / 1 |
| Independent body/race/ethnicity/affinity bundles | 7 |
| Conjured Creatures culture | 1 |
| Graded native NPC templates | 49 |
| Controller bundles / native AI rows | 8 / 10 |
| Special item prototypes / contact poison / infection-type alias | 6 / 1 / 1 |

The installer creates **123 item prototypes** and no live items, NPCs, characters, bodies or Rooms. The seven independently owned anatomies use known Human BaseBody/CountsAs links and scoped FK endpoints, retaining native organs, bone coverage and spinal links. Quadrupeds have four native leg limbs and separate grasping forepaws; winged creatures have working wings and native flight data. Attributes are 10 + 2×grade; combat skills are 20 + 5×grade. Culture, language, blood and health dependencies are native typed bindings. The [creature foundations](Armageddon_Catalogue_S3_Creature_Foundations_20261012.md) describe the scoped graph and explicit affinity merit. Affinity labels grant no magic; S4 Read Attunement reports them separately from learned affiliations.

Spell companion controllers provide saved follow/stop/guard/attack orders, recent exact-instance track seeking, conserved courier custody, bounded haunting and Outsider control loss. Earth Mount also uses the native Mount AI. Servitors retain the existing corpse-animation/commandable path. Controller authority requires the exact primary creator and NPC, active lifecycle claims, a real ready spell, current admitted grade and attached enabled capability. A manually loaded unbound template grants no command authority. Actual stock invocation, summon grade/lifetime hydration and riding admission through those casts remain S4 work.

The specialised stock contains Hover Light, Ember Seed, Sand Effigy, Folded Pocket, Spring Haven and Restorative Mud plus contact poison and native Sown Sickness. Ember and Mud require real lifecycle binding and confirmed durable one-use consumption before applying consequences; callback identity/body/location/custody fences protect those consequences. Sown Sickness uses the existing infection system with native fever and grade-scaled stamina penalties. Ordinary heat effects remain independent. [Native special mechanics](Armageddon_S3_Native_Special_Mechanics.md) records these contracts and the remaining S4/S5 boundaries. Food source energy translates into satiation; no calorie model is introduced.

## Rerun and persistence contracts

The normal development opt-in runs settings, progression and catalogue in that order. Catalogue batches commit independently: supplies, weapons, provisions, anatomy, foundations, controllers, templates and specials. A later failure preserves earlier successful batches; reconnect/rerun resolves uncertain acknowledgement using retained identities. One installation keeps its original Classic/Simple and classes choices. There is no conversion or parallel-edition path, no player refresh and no new NPC/item questions.

Explicit scalar whitelists, child schemas and typed aliases preserve native IDs and builder edits. Compatible craft repairs retain child IDs referenced by active crafts. Narrative numbers, arbitrary definitions and guessed ID fields are never identity links. `ITraitExpression.TraitReferenceIds` exposes the native parser's known parameter links for check/skill dependency closure.

Haunt cancellation edits only the exact parent in the known primary-character effect envelope. Cached memory changes occur after confirmed commit. Failed persistence leaves cached fear intact, retains earlier queued changes and holds the controller until reload. A clean cached target does not acquire a later broad character save that could overwrite unrelated offline fields. Visibility and exact target/body/location authority are rechecked after resistance callbacks. The [lifecycle contract](Spell_Owned_Lifecycle.md) owns the complete runtime rules.

Tracks now retain an optional exact `CharacterInstanceId`, with SetNull deletion semantics; legacy and vehicle tracks remain nullable. EF model, generated migration/designer, model snapshot and refreshed blank SQL/manifest align at `20261012024208_TrackCharacterInstanceIdentity`. Native refresh and fresh import prove snapshot parity. The private gameplay fixture helper restores only a hash-matched owned archive into a fresh owned database and reseals its marker; fixture restores are explicitly labelled separately from fresh seeder runs.

## Qualification

The [machine-readable receipt](Armageddon_Catalogue_S3_Completion_20261012.json) records exact runs, inputs, failures and limits. The full default ten-project Debug suite passed **9,495 tests**, with zero failures and three Windows platform skips. Later runtime changes passed the final full Core suite (**6,514**) and focused terminal/builder/Haunt gates (**25**, **22**, **22**). These overlap and are not added into one total. Final integrated Release Seeder and Debug native-harness builds passed with zero errors and 160/0 warnings respectively. Independent focused review findings were fixed and their closure reviewed.

Actual stock combat exposed missing weapon provenance in the shared melee damage helper: ordinary successful dodge/helpless-defense target packets had null tool origins, unlike block/parry resolution. Both weapon-wear branches now retain the exact weapon item. Natural, worn and spell armour then rebuilt those packets without the tool origin; all six reconstructions now preserve it. Damage, pain, stun and self-wear formulas are unchanged. Two melee cases and four armour cases pass in the full Core suite; the live attack independently saves a wound with the exact knife as its tool origin. This repairs the shared native combat path.

Disposable MySQL qualification runs the complete 32-seeder Medieval replay and unchanged second-run refusal, then proves composite late rollback, earlier-batch retention, lost-acknowledgement/resume, all 198 records, no-op all-table checksum conservation and no player writes. Cold native anatomy/race/culture/affinity readers pass with explicit external registries. The separate native controller fixture proves current authority, durable follow/stop, exact cold control restoration, wrong-creator/retiring/self-attack refusal and cached Haunt cleanup. Its injected database failure on a previously dirty/queued target preserves cached and durable fear, unrelated siblings, saved guard/mission and a later actual save-queue flush. The clean cached-success case preserves an unrelated offline scalar.

Live builder/Admin gameplay passes **21 receipt assertions** from a sealed, hash-matched catalogue fixture. It proves exact craft input/product, wielded weapon and persisted native wounds retaining the exact weapon, numeric food/liquid consumption, grade 1/7 native creature traits, mount movement, winged Flying posture and flight across an approved real exit. Native movement completion uses a bounded 45-second location poll. Both server phases stop gracefully; cold restart retains the partially eaten food (5 to 4 bites) and reduced bottle contents (1 to 0.97 litres), avatar identity/location and native login/logout persistence. The test uses the same 71 changed production files as the root worktree. This restored-fixture lane is separate from the earlier fresh 32-seeder replay.

This does not certify ordinary-player spell acquisition/casting, every controller mission end to end, riding through real seeded casts, all 49 creature/weapon variants, active-craft resume after repair, substance calibration or historical/game balance. Those retain the S4–S6 and later gates. Earlier failed attempts remain in the receipt, including corrected fixture assumptions, the shared combat provenance omissions and the premature movement query; none count as passing evidence.

Earlier [supply/weapon](Armageddon_Catalogue_S3_Checkpoint_20261012.md) and [provision](Armageddon_Catalogue_S3_Provisions_20261012.md) checkpoints retain their original bounded evidence. Their statements about incomplete composite wiring and unexercised gameplay are historical, superseded only for the surfaces explicitly qualified here.
