# Armageddon Magic Completion Implementation Brief

Edition 1, 2 October 2026. Repository: FutureMUD/FutureMUD. Planning baseline: `b0e4339d57bf15bc6e6a6f21fbdd5edc73006e38` on `master`, verified during preparation.

## Purpose and execution boundary

Complete the remaining configurable Armageddon-inspired magic runtime, usable stock catalogue and safe optional installer. The result must be a coherent package a builder can adopt, customise and connect to a world, rather than an engine API demonstration or a collection of decorative substitutes.

This is an implementation-ready design document. The current request authorises preparation of this document only. It does not authorise starting a Codex implementation task, changing repository code, applying migrations, writing to a game database, opening a pull request or deploying. The phases below describe the later implementation task after it is explicitly started.

This brief covers the entire remaining release. It incorporates the installer concerns previously separated as A-D10, but is not limited to that installer. It replaces conflicting September 27 proposals with the decisions recorded here. Read the repository's current `AGENTS.md` and applicable module instructions when implementation begins; rebase the baseline before changing anything.

The deliverable is self-contained for the approved Sorcerer roster, its numeric skill tree, the principal historical corrections and the remaining feature contracts. The existing repository repertoire remains the full 154-entry work inventory. Appendix B reproduces every candidate's stable key, intended behaviour and entry-specific work so no item disappears behind a generic “finish spells” instruction.

## Authority and scope

Use these distinctions throughout code, stock data, tests and documentation:

- **Approved requirement** is a product decision from the completed questionnaire. Implement it; do not reopen it because an older document says “pending”.
- **Historical observation** describes the supplied historical C snapshot or the selected published skill tree. It is evidence about those sources, not a claim of parity with the current live Armageddon game.
- **Engineering proposal** is a concrete default or implementation approach in this brief. It is tunable and may be adjusted with tests while preserving approved behaviour. Numerical proposals are not retrospectively labelled user-approved.
- **Existing implementation** is narrower than a finished stock spell. A registered effect, unit test, or old acceptance transcript does not prove the new complete package is usable.

The 154-entry register is the completion inventory, not evidence that every old proposed adaptation or number received individual approval. Implement its recognisable gameplay using the approved engine-native freedom; update superseded entries, and keep explicit dispositions for any unresolved semantic conflict. Do not silently remove required functionality, defer any of the eight required larger features, or expand Sorcerer membership beyond the selected 82. Wardcraft and Renew Earth remain separately identified FutureMUD additions; they are not historical Sorcerer spells. Preserve their existing runtime and optional builder availability without using them to inflate the definitive Sorcerer list.

Excluded from this task: new cyberpunk/spirit-service systems, a second astral/identity framework, production Vancian-fantasy content, a universal account or character-control mechanism, automatic player conversion, live database cleanup, automatic NPC placement, global chargen redesign, and stock spell scrolls. Existing unrelated magic routes must continue to work.

## Approved decision register

| Question | Binding result | Important limit |
| --- | --- | --- |
| Q01 | Recognisable, standalone usable magic adapted to FutureMUD, with builder customisation | Do not preserve an unsuitable historical mechanism merely for literal parity; do not replace defining gameplay with cosmetic output |
| Q02 | Include Spring Haven, Burrow Refuge, Sand Shelter, Sand Effigy, Walking Shadow, Severing Refuge, Apex Bane and Folded Pocket | All eight are release requirements |
| Q03 | Replace guessed Sorcerer membership with the selected historical 82-spell roster | Do not union Armor, Planeshift, Wardcraft, Renew Earth or other guessed entries into it |
| Q04 | Support general free-choice Sorcerer, restricted Preserver and restricted Defiler | Exact gathering-method matrices remain configurable engineering policy, not separately approved lore |
| Q05 | Individual spell skills, source openings, source raw caps and source parent branching thresholds | Keep raw proficiency distinct from controlled casting grade; extend native improvement compatibly if necessary |
| Q06 | Attribute-led mana capacity and mastery-driven casting efficiency | No approved fixed coefficients, race mapping or practice-driven pool growth |
| Q07 | Historical automatic branching; teacher-only routes are optional builder policy | Do not impose new mandatory lesson gates on the stock historical tree |
| Q08 | Stock practice can reach the highest normal grade | No practice-specific stock cap; ordinary maximum grades and raw caps still exist |
| Q09 | Actual historical POWER words; command casting and genuine speech-triggered casting; language-aware echo | Both enter the same guarded pipeline; generated output never triggers a second cast |
| Q10 | Quiet costs more and is harder; volume/method difficulty configurable; area defined per spell from source | No universal ally immunity or caster exclusion; visible effects and magical detection remain |
| Q11 | Stock charm can affect PCs; each spell can disable PC targets; IC command whitelist for PCs and NPCs | Reject account/settings/OOC/builder commands and all alias/indirect dispatch bypasses |
| Q12 | Source-informed per-spell/per-tier conjuration and summon lifetime; permanent, temporary cleanup and death-on-expiry modes | No heavy temporary-NPC database accumulation; handle early death, remains, possessions, references and restart |
| Q13 | Per-item activation eligibility: caster, magic type, spell knowledge or anyone; stock wands/staves caster-only | Historical unrestricted activation does not override this explicit stock choice |
| Q14 | Recharging caster must legitimately reproduce stored power | Capacities, timings, prices and failure rates remain tunable proposals |
| Q15 | No spell scrolls in the Armageddon stock installer | Optional separate scroll-learning support must enforce prerequisites and never copy maker mastery |
| Q16 | Complete usable capabilities, merits, templates, foci and supplies; optional matching classes if classes are enabled; short integration guide | Builder connects chargen and places NPCs; safe ownership-aware reruns do not update players |

## Current baseline and work that must not be redone

[PR 771](https://github.com/FutureMUD/FutureMUD/pull/771) is merged. It already supplies opt-in `skilllevel` casting policy, explicit cross-school admissions, canonical identity acquisition and grades, route-local skill/resource binding, seven-grade fixture, explicit next-grade overreach, guarded normal casting, typed FutureProg functions, durable operation receipts, quarantine and persistence. Its four casting tables are already introduced by `20260927050300_ConfigurableCasting`.

[PR 778](https://github.com/FutureMUD/FutureMUD/pull/778) is merged at the planning baseline. It fixes acquisition cascade ordering by allowing a previously blocked dependent admission to be revisited after a successful prerequisite grant. It does not add a seeder, schema, scheduler or paid-casting features. Preserve its reverse-order chain and all diamond permutations; do not reintroduce the call-wide evaluated-set bug while extending prerequisite types.

The PRs report substantial prior verification. Those receipts remain historical evidence. This document did not rerun runtime tests. PR 771 explicitly did not establish a full installed-world Telnet session or whole-preset balance; PR 778 did not add native database acceptance. Neither means this release is finished.

Existing foundations to extend rather than recreate:

- `IMagicCastingCapability`, `IControlledMagicSpell`, `IMagicCastingService`, `MagicCastingService.*`, `MagicCastingStateStore` and the casting partials of `SkillLevelBasedMagicCapability` and `MagicSpell`
- Native skill definitions, `ClassicImprovement`, `BranchingImprover`, lessons, trait change notifications and `CheckImprovementScope`
- Native language/emote/hearing, communication methods and volumes, command resolution and permission trees
- Existing effect factory and implementations, spell operation outcomes, personal/room/tag wards, native target resistance, planar checks and portal topology
- Canonical identity, body instances, focus, projection/clone/dead-speak infrastructure and existing death/corpse semantics
- Environmental magic coordinator, Self/Gentle/Land gathering, native organic accounting, scars and land rejuvenation
- Existing prepaid spell snapshot/compatibility and magical substance machinery; do not enable scroll stock as a side effect of reuse
- Seeder dependency/replay/answer-memory conventions and ownership helpers

Current missing common paths are practice, formula/speech adapters, quiet and area policy, production/recharge and charged wand/staff integration, finished stock authoring and complete installation acceptance. The repertoire also has entry-specific missing mechanisms. September 27 counts were 38 primitive/configuration, 65 primitive plus policy/content, 43 small missing primitives and 8 larger systems. These are classifications, not completion percentages; refresh implementation evidence as work lands.

## Historical baseline and corrected defaults

### Sorcerer roster

Use [the 2013 fan-published full-guild tree](http://armageddonmud.freetzi.com/sorcerer.html), published September 3, 2013 and verified entry-for-entry against [the January 6, 2016 archive](http://web.archive.org/web/20160106231707/http://armageddonmud.freetzi.com:80/sorcerer.html). It contains 94 skill rows: 82 spells and 12 support skills. All 82 match existing repertoire entries by historical name. Of these, 41 already had Sorcerer membership and 41 did not.

This is a fan publication, not an authenticated official configuration export. The user selected it as the definitive roster for this implementation despite that provenance limit. Later restricted/subguild eras do not change this choice. Other positive historical evidence for Armor and Planeshift remains documented, but is not included in this selected roster.

The four spell roots are Sense Enchantment, Unravel Enchantment, Gust Hands and Unyielding Veil, each opening at raw 60 and capped at 90. The other 78 spells open at 30. All spell caps are 90 except Mend Flesh/Heal, capped at 60. A parent's `go on` field is the raw threshold for its children; do not use the child's field or the preceding flattened row. The full tree appears in Appendix A.

### POWER words and indices

The historical ascending POWER words are `wek`, `yuqa`, `kral`, `een`, `pav`, `sul`, `mon`. Bind them to configured grades 1–7. Source constants use 0–6, while many spell handlers and item commands use 1–7; FutureMUD `SpellPower` enum integers are a third coordinate system. Never cast among these integers implicitly.

The stock native map remains grade 1 ExtremelyWeak, 2 VeryWeak, 3 Weak, 4 Standard, 5 Strong, 6 VeryStrong, 7 ExtremelyStrong. A dedicated mapping test must cover each word, external grade, source reference index and native enum. The older proposed `first` through `seventh` words are not stock defaults. Builders may add aliases.

### Area and lifetime corrections

Source-following was the selected policy; recollections during discussion were not absolute overrides. Therefore:

- Earthquake can damage its caster at one-third ordinary damage. Chain Lightning can select its caster at quarter damage. Neither supports a blanket ally filter. Some room/staff fireball and lightning-bolt wrappers do exclude the caster. Store those distinctions per spell and invocation mode.
- Historical conjured food schedules expiry and is marked NOSAVE; it is not universally permanent. Create Water fills an existing container with no magical expiry scheduled by that function. Implement useful consumption while respecting each authored lifetime.
- Fire Jambiya has timed lower tiers and a highest-tier component-consuming permanent-staff path. Do not apply a universal “all conjured gear disappears” rule across a source-defined permanent output. Temporary outputs still must not yield permanent salvage.
- Wands/staves exist in the historical snapshot. Its inspected activation branch lacks a guild/known-spell/skill gate. The explicit approved stock caster-only rule is an intentional FutureMUD change.

Appendix C records verified source functions and further concrete distinctions. Never infer all remaining summon limits, loot or spell behaviour from one example. Where source detail is absent or unsuitable, implement the bounded contract in the repertoire, identify the adaptation and test it.

## Shared architecture and persistence contracts

### One spell and one authorised pipeline

Keep one native spell definition per stable repertoire key, shared across every admission. Native school remains the authority for resistance, wards and school effects. Capability home school controls command organisation, not implicit access. Earth and Sorcerer casting the same spell must not mutate its global trait, cost, power, target definition or effect template.

Every ordinary input resolves to an immutable casting intent: canonical identity, acting instance, explicit capability/admission, spell, external grade, overreach consent, practice/manifest mode, communication method/volume, target adapter and origin token. Then quote and execute through the same service. Commands, real speech triggers and FutureProg cannot bypass one another's gates. A pure query performs no acquisition, skill opening, improvement, payment or persistence.

Retain preflight/commit separation, explicit ambiguity refusal, acting-body physical requirements, route entitlement, current target/material validation, existing quarantine and at-most-once payment/progression. Revalidate live state at commitment. Failed checks after commitment remain paid. No uncertain operation automatically refunds, replays effects, rerolls progression or clears its quarantine on restart.

A target group, multi-effect spell or multi-target chain is one invocation, one applicable cost commitment, one shared skill opportunity and at most one mastery opportunity. `Applied`, `NoChange`, `Rejected` and `Unknown` remain distinct. A factory load or a returned persistent effect is not sufficient evidence of application; instantaneous effects must report meaningful application without relying on a child effect.

### Identity and route boundaries

Acquisition, controlled grades, native proficiency, opportunities and configured reserves remain canonical-identity state. Acting body/instance governs location, speech, manipulation, materials, focus and target access. Switching bodies or capabilities must not clone a reserve, refresh deadlines or grant another root set. Temporary item/drug grants supply current permission only; they do not silently enrol or auto-branch.

Enrolment requires an explicit authorised workflow. Provide a typed authored enrolment hook suitable for a builder's selected chargen-finalisation or NPC-creation workflow if current staff-only `Enrol` cannot serve it. It must reuse the same idempotent service, permanent merit and stable capability identity checks, and provenance. Never use a query callback or a capability-list enumeration to enrol.

New persistent concepts must have explicit ownership, versioning, load validation and retirement semantics. Extend existing records/XML when sufficient; generate EF migrations, designer and snapshots using the pinned tools when schema changes are needed. Do not hand-write migrations. Do not bump existing spell profile versions for cosmetic edits or silently strand acquired progress. Changes requiring migration get an explicit migration/reconciliation policy; installer reruns are not player migrations.

## Progression and learning

### Source tree authoring

Author all 82 Sorcerer spell admissions and the four exact roots. Apply source raw openings/caps and parent thresholds from Appendix A. Preserver and Defiler use the same spell repertoire; restrictions live in gathering policy unless separately configured by a builder.

Represent prerequisites as validated typed edges. Existing `MagicCastingPrerequisite` can express only a spell prerequisite. Read Enchantment/Identify requires Component Crafting, which itself branches from Shadow Passage/Ethereal. Add support for an explicitly scoped native-trait prerequisite and support-trait grant, without forging a dummy spell. Keep full provenance and stable keys. Vloran and the remaining non-spell rows remain support data, not additional spells.

The stock package must provide a real attainable Component Crafting skill/progression path so Identify is reachable. Bind applicable native crafting/gathering/skill-use hooks instead of inventing a second unrelated spell-crafting system. Language and mundane combat support should map to suitable existing native concepts; where the world lacks a literal source language or psionic support package, document a configurable mapping or optional support template. Do not install a second psionics system or make unrelated mundane skills a mandatory dependency of every magic capability. Optional matching class templates may grant the relevant support rows; the core spell path must work without classes.

Native `BranchingImprover` already supports `(base trait, threshold, opening, branch trait)` rows and `ClassicImprovement` already supports a minimum difficulty interval. However, the existing improver directly opens a trait and cannot alone authorise canonical spell acquisition or scope it to an enrolled capability. Do not wire it to spell traits in a way that makes trait possession equal knowledge or branches unrelated traditions. Prefer an adapter/compatible improver that reports eligible typed edges to the existing casting acquisition service. A new global polling scheduler is unnecessary.

Preserve event-driven reverse indexing and PR 778's successful-grant-driven convergence. Extend tests for typed trait edges, missing supporting traits, cycles, diamonds, interrupted grants and a second body. Adding a native skill must remain repairable after a failed persistence boundary, without double-opening or raising an existing value on rerun.

### Raw caps and mastery are different

A raw skill of 60 does not mean controlled grade 6. A newly acquired spell begins at controlled grade 1 even when its historical raw opening is 60. Maintain this separation in player help, training, logs and data.

The old fixture's grade-7 raw threshold 95 is incompatible with the approved raw caps of 90 and Heal's 60. Leaving it unchanged would make maximum mastery unreachable and violate Q08. Resolve this explicitly rather than inflating the source caps.

**Engineering proposal:** keep the shared seven-grade meaning and use cap-relative proficiency gates for mastery eligibility. Reuse the old relative gate fractions `0, .20, .40, .55, .70, .85, .95` against the admission's raw cap, without normalising the actual spell check. Thus grade 7 eligibility is raw 85.5 on a cap-90 binding and 57 on Heal's cap-60 binding. The actual spell check still uses the real native trait, and grade is still earned through the configured mastery opportunity. These fractions are tunable proposals, not historical facts or verbal approval of the old numerical profile.

Opening values and route caps need explicit admission configuration because `ControlledSpellProfile.OpeningSkill` alone is shared across routes. A source opening applies only when first opening a missing native skill; never lower existing skill, grant it repeatedly or convert a previously learned spell into new knowledge. If multiple permanent enrolled routes bind the same skill with different caps, document one canonical raw-skill cap policy. Proposed default: the highest legitimately applicable permanent-route cap, while each route's mastery gate uses its own declared scale; temporary attachments do not raise it. Preserve existing skill values on route loss and inhibit further improvement above entitlement rather than destructively trimming history.

Shared-tradition proficiency remains supported as an engine option. Validate the relation between a shared trait's cap, per-spell grade gates and source-specific stock definitions. Individual-spell skills are the installed default.

### Practice and teaching

Practice is target-free training. It must not invoke the real spell and discard output. It pays the configured full practice energy, uses an explicit practice physical/material plan, performs the appropriate check and may improve both raw proficiency and controlled mastery under shared limits. It cannot damage, heal, summon, generate materials, provoke target resistance, consume a bystander's ward or run caster effects.

Stock practice has no separate maximum-grade restriction and no hidden native difficulty cap that prevents reaching its normal cap. `ClassicImprovement` supports `DifficultyThresholdInterval`; configure a stock profile that permits the intended practice path. Builders can set practice difficulty, required difficulty intervals, maximum practice grade or disabled practice where desired. A UI setting is insufficient: test the effect on real improvement and the highest grade.

**Engineering proposal:** a 30-second action with full energy committed at start, required retained focus throughout, no advancement on interruption and no automatic refund after commitment. Preserve per-identity/per-trait and per-identity/per-spell opportunity sharing with manifestation. Reuse fixture mastery chance 25%, mastery interval 600 seconds and skill interval 60 seconds only as initial testable balance defaults, clearly labelled proposals. Tune them after actual progression runs; do not force incompatible fixture opening values or caps into stock.

Interruption includes movement where the authored action forbids it, loss of consciousness/speech/manipulation/focus, capability loss, death, logout, invalidation and restart. Persist enough operation/deadline state to prevent cancellation/restart farming. Do not resume an uncertain practice action and award progress twice.

Historical stock acquisition branches automatically at the specified parent threshold. Teaching is an optional alternate route and builder-configurable requirement, not a new mandatory historical bottleneck. A lesson verifies the student's admission and prerequisites, teacher's legitimate knowledge/controlled grade, physical communication and normal lesson limits. Cross-tradition teaching of a shared spell is permitted when both routes authorise it. It grants grade 1 and the student's opening, never copies teacher proficiency or mastery. Any proposed teacher thresholds remain configurable.

## Mana capacity and costs

### Capacity and regeneration

Extend the existing `SimpleMagicResource` cap prog and route resource configuration. Attribute-led capacity is the stock model; practice and spell use do not directly increase the maximum pool. Use a builder-selected native mental/spiritual attribute or expression instead of adding a hard-coded historical race system. Keep maximum capacity, current balance, regeneration entitlement and gathering as separate concepts.

Historical `mana_limit` begins with `100 + 3 × (Wisdom − 13)`, clamps to `2 × Wisdom` through `12 × Wisdom`, then applies race and other modifiers, including a magicker minimum. Age can affect Wisdom indirectly; the function is not a direct skill/grade/age growth formula and does not contain a separate random mana-capacity roll.

**Engineering proposal:** expose that expression shape through a named cap prog, bind its attribute explicitly to the world's appropriate seeded attribute, and do not import half-giant/mul/spice constants into unrelated FutureMUD races. Installer preflight must find a compatible attribute mapping or request one actionable choice. No name-guessing fallback silently picks an unrelated attribute. Show the selected mapping in the install summary.

Retain canonical balances, initial zero and no refill on enrolment, attachment or rerun. Recalculate cap safely when relevant attributes change; lowering cap must not create a refill exploit. Passive elemental regeneration and Sorcerer gathering remain distinct. Regeneration rates/posture multipliers from the old fixture are provisional balance values, not source-backed approvals. The historical regeneration function excludes gather users; stock Sorcerer/Preserver/Defiler must not accidentally gain passive elemental regeneration through another template.

Proposed initial restriction matrix: general Sorcerer allows configured Self, Gentle and Land methods; Preserver allows Self/Gentle and forbids Land; Defiler allows Self/Land and forbids Gentle. Keep each method independently builder-configurable. This is a concrete starting policy, not a claimed approved historical matrix. No irreversible alignment, automatic class conversion, universal criminality or extra metaplot follows merely from choosing a method. Existing ecological debit/scars/consequences remain real.

### Mastery efficiency

The resource quote needs both requested grade and controlled mastery. Existing `base × grade` alone does not implement Q06. Extend the typed numerical context; do not smuggle mastery into a mutable global variable or replace raw skill with a grade.

A useful source-shaped translation is:

1. Let `m` be controlled grade 1–7 and `p` requested grade 1–7 after explicit index conversion.
2. If `p == m`, source base cost is 50.
3. If `p > m`, source base cost is `max(100 − floor(50 / (p − m + 1)), spell minimum)`.
4. If `p < m`, source base cost is `max(floor(50 / (m − p + 1)), spell minimum)`.
5. Apply explicitly configured route/volume/area/overreach policy once and aggregate other legitimate secondary costs as before.

This reproduces the observed source formula shape for ordinary indexed inputs; source immortal/negative-level branches are not player stock mechanics. The equal-grade source branch returns 50 directly, rather than applying the minimum clamp. Define that deliberately if exposing a literal-source profile, and reject nonsensical minima/configuration. Do not accidentally apply both the old linear grade multiplier and this replacement curve. The overreach multiplier may compound the curve only as an explicitly labelled FutureMUD balance choice; default testing should compare source-shaped cost alone against that extra penalty rather than silently double-penalising.

**Engineering proposal:** use the selected roster's printed minimum-mana values as the stock lower-cost bounds for its 82 spells. The source printing function computes these values at maximum mastery and lowest power; they are not necessarily the raw `skill[].min_mana` values, so a difference from C metadata is not by itself a version conflict. Preserve that distinction and author an explicit lower bound for other spells. Expose an energy scale so the translated numbers can fit native attribute ranges. Every spell/grade/allowed mode must have an attainable documented caster configuration. Generate an affordability matrix across lower, ordinary and advanced cap values and prove the high-grade maxima advertised to players are reachable. Do not advertise permanently unaffordable quiet/area options as usable stock.

Example reference curve before other multipliers: controlled 1/requested 1 costs 50; controlled 2/requested 1 costs 25; controlled 7/requested 1 costs `max(7, minimum)`; controlled 1/requested 2 costs at least 75. These are curve tests, not a claim that all effect balance is settled.

## Incantations communication and area targeting

### Command and real speech adapters

Support ordinary named commands and real spoken formulas. A formula is a configured vocabulary mapping to one intent; it is not proof of knowledge. The inspected historical parser resolves five categories: power, reach, element, sphere and mood, finding each category among the five supplied words rather than requiring a single fixed order. Provide a builder-authored grammar capable of that full form as well as concise spell-token aliases; stock must use the approved POWER vocabulary. Seed the remaining words from verified repertoire metadata where available, otherwise author explicitly labelled FutureMUD aliases instead of inventing historical words. Retain builder-authored aliases and language binding. Actual POWER words are stock; do not claim a generic three-token alias reconstructs the full historical element/sphere/mood grammar unless that grammar is also authored and tested.

Hook the existing structured speech event/`ISpeakPhraseTrigger` surface after physical speech eligibility is known and before any duplicate legacy direct trigger can execute. Only an explicit complete player-origin incantation in an eligible speech method can request casting. Do not scan arbitrary output, descriptions, overheard text, quotations, language-garbled listener output, emote narration or chat transcripts for executable strings. Unknown, ambiguous and inadmissible formulas refuse without casting costs; ordinary speech still follows its native communication rules.

Command casting generates one language-aware incantation echo through native emotes/hearing. Speech-triggered casting reuses its original speech rather than adding a second utterance. Mark generated cast speech with non-triggering origin/correlation metadata. A listener repeating a formula is a new attempt subject to their own admission; an echo, translation, replay or relay is not. Test two eligible listeners, self-hearing, room relay, dead-speak/recite, alias invocation, network duplicate and effect-generated speech. One original invocation yields one debit/check/operation regardless of callbacks.

Observers receive what their language, hearing, distance and perception permit. Hearing does not grant acquisition, proficiency, mastery, grade identification or perfect spell identification. Do not leak the authoritative formula through an unconditional plain-text echo to someone who cannot understand it. A special recognition feature is optional and is not a dependency for release.

### Quiet and volume policy

Stock normal/whisper modes are sufficient. Use the existing communication volume/method infrastructure, with configurable casting difficulty and energy modifiers, not a new parallel volume enum. Quiet is whispered rather than silent; a mute/incapable caster cannot bypass speech by selecting it. Visible effects, gestures, magical senses and other physical requirements remain.

Proposed initial quiet modifier is ×2 energy and +1 difficulty step, reflecting the earlier proposal and source cost evidence; those numbers are tuning choices. Builders may author shouting as easier, or a different volume policy. Any supported combination such as quiet plus area must be explicitly enabled and quoted; it must not emerge accidentally from multiplying arbitrary flags. Disable unmapped combinations with a useful refusal.

### Per-spell area policies

Author a target policy for each spell/mode, including physical scope, maximum targets or bounces, inclusion of caster, ally handling, plane compatibility, immortality/staff handling, grounded/airborne checks, target ordering and per-target attenuation. An area marker cannot automatically turn a single-target spell into a group spell. Room/exit effects operate on their native cell/exit, not a synthetic list of characters.

Use one bounded snapshot of candidate targets at commitment, deduplicate real actor identities/instances according to the spell contract and revalidate each target's live reach, ward and resistance before effect application. Preserve legitimate separate bodies as targets when the physical spell affects them, but do not double-credit progression or drain a single canonical reserve twice through shared identity. Changes during resolution cannot create unbounded iteration.

Do not silently identify enemies or exclude allies. Encode source differences: Earthquake's caster attenuation and grounding, Chain Lightning's bounded bounces/caster possibility, and caster-excluding room Fireball/Lightning Bolt. Distinguish their source room/staff wrappers from normal single-target casting. Define the result for an empty target set before payment, and for all targets rejecting after payment. Reflection and redirection must have a strict lineage/depth guard.

## PC charm and command authority

Implement persistent, resistible influence, not a one-shot unrestricted `forcecommand`. Configure PC eligibility per spell, resistance and duration, controller identity, permissible order range and action set, break conditions, conflicts with other control, and command timing. Stock Compelling Regard permits PCs as requested; other control effects retain their authored contracts.

Author an explicit stock whitelist of safe in-character gameplay command IDs/actions. Start with useful bounded actions such as permitted movement, follow, posture, drop/give within ordinary item permissions, and supported combat orders only where the spell contract allows them. This list is an engineering content decision that must be reviewed against the native command inventory. Neither “any command except these strings” nor simply using the NPC command tree is sufficient.

Resolve aliases, abbreviations, command wrappers and any queued expansion to final executable commands before checking the whitelist. Preserve input origin and controller provenance through nested dispatch. Reject multi-command separators, aliases/macros and script/force/order wrappers unless every final operation is individually authorised and bounded. Never execute raw text through a more privileged command manager or another actor's administrative authority.

Always exclude account, password/login, settings, OOC, builder/admin, external communication/bridge, script editing/execution, and other out-of-game controls. A PC charm can affect gameplay choices; it cannot capture the account. Recheck controller validity, effect duration, target body/focus, ordinary permissions and whitelist at execution time, not only when the order is queued. Failure or expiry cancels queued control safely.

Test a charmed PC and NPC with identical allowed and forbidden commands; renamed aliases; abbreviation collisions; nested alias; alias to `force`/script; queue surviving expiry; resisted charm; controller death/logout; target body switch; overlapping control; and reload. Preserve permanent learned skills and player-authored data; generic charm does not erase them.

## Conjuration summons and durable cleanup

### Reusable lifecycle record

Create one reusable spell-owned lifecycle contract for created items, autonomous summons, temporary cells/links and identity projections. Store a stable operation/lifecycle key, source spell and grade, creator canonical identity, exact owned entity/instance IDs, creation time, absolute deadline if any, lifecycle mode, ownership of generated possessions, cleanup state and optional bounded provenance. Never rely on one in-memory timer for durable cleanup.

Modes must include:

- Permanent creation: ordinary durable entity after successful creation, with no automatic spell expiry
- Temporary cleanup: retire/dissipate without creating ordinary valuable corpse/loot by default
- Death on expiry: use the authored native death/remains path once, then clean up the temporary identity when its dependent remains/references permit

The expiry behaviour is separate from control behaviour and from current life/death status. A permanent summoned NPC may have temporary control. A temporary autonomous NPC is not automatically another player-controlled body. Player projections/copies use the existing canonical identity/instance framework; do not create a fake independent player identity or delete its owner on collapse.

### Safe state transitions

Use explicit idempotent states such as Active, Retiring, RemainsPending and Completed, with a durably recorded transition before destructive cleanup where needed. Exact type names are illustrative. The invariant is at most one terminal transition and no duplicate corpse, ejection, payout or object release.

Handle, at minimum:

1. Normal expiry, dispel, dismissal, owner capability loss and configured logout behaviour
2. Kill before expiry, including a corpse that survives longer than the original summon timer
3. Corpse decay/deletion before or after the deadline; corpse moving into another cell/container
4. Restart before expiry, after overdue expiry, during retirement and between death and corpse cleanup
5. Summon or source item missing, deleted source prototype, unloaded caster and invalid destination
6. Riders, passengers, combat targets, followers, orders, AI state, held/implanted/carried goods, containers, room occupants and event subscriptions
7. Historical references such as wounds/combat provenance, crime/witness records, corpse `OriginalBody`/`OriginalCharacter`, hooks, effects and other foreign keys

Early death cancels or transforms the active lifetime registration. The later timer cannot resurrect or kill again. If a corpse requires original body data, retain only the necessary record until ordinary corpse cleanup releases that dependency. A cleanup queue must retry safe incomplete retirement, but never repeat a paid effect, create new loot or blindly issue cascading SQL deletes.

Preserving foreign possessions is an explicit FutureMUD safety adaptation: the historical extraction path can destroy retained inventory on ordinary summon expiry. Do not reproduce that loss. Foreign possessions are not spell-owned merely because a summon carries them. Separate generated temporary equipment from existing world items. On expiry, evacuate riders and occupants to a validated location, release foreign possessions exactly once and preserve contents if a container collapses. If the nominal return cell no longer exists, use a configured safe fallback or a recoverable holding location with clear diagnostics; never strand or silently delete actors/items.

### Database bloat acceptance

Do not solve temporary summons by leaving permanent full `Character`/body/inventory/AI graphs behind forever. Inspect existing instance retirement/deletion and FK semantics, then implement a proven bounded retirement path. Heavy ephemeral records should return to a steady state after their lifetime and any valid corpse/reference retention interval. When immutable historical references cannot be removed, use a deliberately lightweight archival/tombstone representation or detach nullable references while preserving meaningful attribution, with bounded retention policy and no live actor load.

Never delete a player identity, a builder-authored permanent NPC or a borrowed body because it is reachable from a spell effect. Ownership checks must prove the entity was created for that lifecycle and its policy permits removal. Shared native identity projections retire only their created instance/effect, not the canonical identity. A dead-speak proxy restores the same real corpse and releases only its temporary proxy.

On a disposable database, repeatedly create, kill/expire, decay and restart at scale. After reconciliation, no eligible heavy temporary graphs, active timers, dangling event subscriptions or orphan owned items may accumulate; legitimate permanent creations and independently retained world possessions remain. Report row counts by table and reason for every retained record. “Hidden from the world” alone does not satisfy this requirement.

### Stock controls and economy

Summon count is configurable per spell/family and canonical identity, with source-informed stock choices. “One combat summon” was a proposal, not an approved universal limit. Count live/retiring entities consistently so simultaneous casts, alternate bodies and restart cannot evade the cap. Replacement requires explicit authored behaviour; do not delete a summon to make room silently.

Source-derived starting relationships must remain distinct: Gate creates one actor with escalating hostility risk from existing Gate followers rather than a hard cap; Guardian creates one creature per requested grade in the inspected function, with guard/liking links; Mount creates one rideable actor per cast. Preserve the configurable per-cast counts and risk/relationship policies rather than imposing one total-active summon universally. Exact historical delay units require a single declared conversion to native durations; do not label raw source event values as seconds without proof. Give summons functional native AI/control, guarding/riding/tracking/courier behaviour as appropriate. Commands remain creator-authorised and bounded. Generated temporary loot/salvage must not turn into permanent value by killing, disassembling, selling, skinning, butchering, stacking, splitting, crafting or transferring it. Legitimately permanent source outputs are explicitly costed and treated as permanent, not hidden behind a cleanup exemption. Conjured food is consumable while it exists; consumed quantities do not reappear on cleanup. Water follows container capacity/mixing and conservation rules.

## Eight required larger features

These are minimum usable contracts. They deliberately reuse a small number of native mechanisms and the lifecycle service above.

| Feature | Required implementation | Native acceptance |
| --- | --- | --- |
| Spring Haven | Temporary physically usable sheltered watering place, bounded shelter/occupancy and finite water supply, source/terrain admission and linked cleanup | Enter/use shelter, draw finite water, reject overdraw, expire while occupied, preserve visitors and foreign goods |
| Burrow Refuge | Enterable temporary underground cell/topology with terrain gate, safe entrance/return and no permanent mutation of a builder's cell | Enter and leave, reject incompatible terrain, expire with occupants/offline occupants, restart and invalidate anchor |
| Sand Shelter | Functional temporary shelter through suitable native shelter/item/interior mechanics, not a decorative object | Demonstrate protection/occupancy, controlled admission, destruction and expiry with riders/contents/occupants safe |
| Sand Effigy | Bounded item-to-instance link using existing canonical identity/body infrastructure; immobile projected copy where defined; meaningful focus/anchor/destruction semantics | Focus/observe as supported, destroy effigy, damage/collapse projection, reload and prove owner/skills/resources are not cloned or deleted |
| Walking Shadow | Controllable shadow instance while the original body remains, explicit planar/body limits, range/anchor/focus/recognition and damage consequences | Switch focus, attempt forbidden manipulation/travel, sever anchor, expire/restart and restore valid focus without duplicate identity state |
| Severing Refuge | Functional refuge with an explicitly authored ward against selected elemental interactions; defined topology and permitted entry/exit | Prove representative blocked and allowed elemental operations at the boundary, no universal invulnerability, safe occupied teardown |
| Apex Bane | Resisted attack against a builder-defined magical-creature category/tag/prog, with bounded grade-scaled effect | Eligible and ineligible creatures, resistance/ward result, no name/race-ID hard-coding, native delivered damage and payment |
| Folded Pocket | Finite items-only storage with capacity/weight policy, ownership/access, no living occupants and no nested pocket spaces or cyclic containers | Fill to boundary, refuse nesting/body entry, save/restart, transfer source item, destroy/expire while full and conserve every foreign item exactly once |

Severing Refuge's ward vocabulary and Apex Bane's creature category are builder-configurable stock abstractions, not imported universal Armageddon metaphysics. No second planar framework or new global creature taxonomy is required. Temporary topology must integrate pathfinding, route coordinates, exit/portal invalidation and occupants, not merely create a cell row.

## Remaining catalogue mechanisms

Appendix B is the full per-entry inventory. Implement shared mechanisms once and bind each affected entry to them, then demonstrate each spell independently. The following grouping owns the known small gaps:

- Temporary items, lights and embedded payloads: Hovering Light, Flame Knife, Ember Seed, Sand Knife, Shadow Blade, Champion Blade, Spark Lash and Storm Spear. Add ownership, placement/equipment policy, trigger/activation and per-tier permanence, not only `CreateItemEffect`
- Inventory/ingestion interception: Custodian Glyph enforces taking permissions; Burning Draught Curse changes later ingestion; neither is equivalent to a tag or immediate damage
- Liquid creation: Draw Water and Draw Wine handle empty mixtures, capacity, compatible mixing and refusal without null-path failures; no uncontrolled replacement of existing liquids
- Attunement/information/path: Read Attunement has a bounded builder-authored vocabulary/prog; Trace the Path uses permission-aware bounded path information without revealing hidden/private/inaccessible destinations
- Delivered-value composites: Vital Siphon, Devouring Touch and Leeching Edge heal only from actual delivered eligible harm, with native clamps, absorption/resistance and per-hit attribution; independent damage-plus-heal lists do not satisfy conservation
- Traversal hazards: Flame Barrier, Thorn Barrier and Blade Barrier bind injury/entanglement to legitimate crossing attempts, show both-side output and avoid repeat damage from a refused command loop beyond configured cadence
- Native organic damage: Wither Growth respects plant eligibility and real crop/land accounting; it cannot bypass the environmental resource ledger
- Paid substance production: Restorative Mud and other explicitly compatible substance templates use real carrier/dose rules and captured-grade production; topical contact alone is not automatically ingestion/exposure
- Ordered movement/combat: Ground Tremor adds grounded admission and a fall check; Wind Hammer pushes only after qualifying hit; Fleet Step, Leadfoot and Lightning Pace modify actual movement timing with sensible stacking
- Delayed actions/hazards: Return Tether owns a persistent cancellable return record; Storm Locus has a bounded spell-owned tick/deadline lifecycle; neither uses a broad environmental scan as a substitute
- Summon behaviours: Earth Mount, Clay Sentinel, Air Guardian, Seeking Wind, Wind Courier, Haunting Shape and Call Outsider have real ride/guard/track/message/control and retirement contracts. Historical Gate/Call Outsider summons an actor; a portal is not a substitute
- Multi-target lightning: Leaping Lightning implements source-informed bounded repeated random selection and its caster attenuation; targets can repeat unless a builder explicitly changes that policy. A distinct-target diminishing chain from the old register is an adaptation, not the recovered source default; a flat undifferentiated area list is insufficient
- Feign Death integrates appearance, physiological observation and action restrictions while keeping the living identity intact; it does not create a fake permanent death/corpse
- Silence the Mind covers selected psionic entry points and active-link suspension/teardown as authored; it is not proof that every unrelated psionic system is disabled
- Compelling Regard uses the approved PC/NPC charm contract above
- Turn the Elements performs bounded eligible redirection with lineage protection; a rejecting ward is not reflection
- Restore Object repairs eligible real item damage while preserving material, quality, identity, contents and revision; `MendEffect` for character wounds is not item repair
- Drowning Grip uses native breathing/hypoxia admission and a removable resistible effect, rather than arbitrary damage. The historical entry lacks a bound function, so identify this explicitly as a FutureMUD adaptation

For existing primitive/configuration entries, author actual effect parameters, components, targeting, scaling, cancellation, stacking, counter-effects, descriptions and builder help. Purge Toxin/Purge Disease must accurately describe whether they remove spell-owned conditions or broader native medical state. Do not claim universal cures from narrow removal adapters. Spatial Join Traveller must use teleport/movement semantics, not the wound-treatment `relocate` primitive. Correct these distinctions in the register and tests.

Every included spell needs an actual operation-reporting path sufficient for its intended mastery behaviour, native resistance/ward handling, meaningful low/high-grade tests and an ordinary-player use case. A JSON row, registered token, successful XML roundtrip or placeholder FutureProg is not completion.

## Wands staves foci and substances

### Roles and eligibility

Keep focus and charged-payload roles explicit. Focus use requires ordinary acquired spell knowledge, current route and all ordinary reserve/material/body costs; it never supplies free knowledge or grade. Charged use consumes the item's stored payload under its own eligibility rules. A depleted charged item must refuse; it must not silently fall through to a personal-reserve cast. A dual-role item offers explicit mode selection.

Provide per-item eligibility profiles for any user, any legitimate caster, matching capability/magic type, and acquired stored spell, with optional grade/skill check policy. Define “caster” from legitimate current runtime capability entitlement, not a class-name substring or possession of any magic item. Stock wand/staff templates use caster-only eligibility. Non-caster activation remains a supported builder option rather than a stock guarantee.

Use captured spell/grade/numeric context from a legitimate production route, plus current target resistance/wards at activation. No activation grants spell acquisition, spell proficiency or controlled mastery. Optional item-use skill progress is separate and explicitly configured. Higher-power eligibility/difficulty is configurable; do not silently carry forward the old universal Normal-difficulty skill gate as approved.

Preflight refusal does not consume a charge. A committed activation consumes exactly one charge before effects; a check failure or all-resisted outcome follows the authored committed-failure policy. Proposed default is charge loss on committed failure, with durable receipt/tombstone semantics and no restart replay. Snapshot compatibility is carrier-specific and fail-closed for unsupported effects, identity ownership, concentration, extra targets or unresolved numerical bindings. A listed payload must pass real capture/release tests.

### Production and recharge

Recharging must establish that the producer currently has an admissible route, acquired spell and controlled grade at least as high as the stored payload and can legitimately reproduce its defining stored power. Mere display grade, a temporary trait bonus, another person's item or a stale acquisition record is insufficient. If captured potency includes additional attributes/bonuses, define a reproducibility comparison that cannot upgrade an old powerful item using an unqualified weaker producer.

Production/recharge uses a target-free production plan with payment/materials, physical foci, duration, interruption and durable commitment. It never manifests the spell in the room. Do not allow overreach to manufacture a grade the caster cannot control. Keep stored payloads homogeneous per charge bank, cap capacity, fill only missing capacity, and reject a payload/grade change while incompatible charges remain. Copies of an item cannot clone spendable charges.

Proposed starting values are wand capacity 5, staff capacity 10, 60 seconds per charge and one appropriate casting-energy charge plus production components per charge. They are not approved historical numbers. Author and tune them with recharge economics and source mana-storage semantics in view. Prefer existing item-resource storage if it can express the contract without making a second unexplained resource system. Failed/uncertain production cannot refund and also leave usable charges.

Only seed compatible substances with actual dose/carrier/trigger definitions and recipes/supplies. Preserve native drug-vector/exposure and dose-lifetime rules; calibrate magnitude from reference dose rather than assuming “one gram” is universally correct. Do not create a second learnable spell for each carrier. Keep source spell identity and independent carrier compatibility/attribution.

### Scroll exclusion

The Armageddon installer creates no spell scroll templates, scroll charges, scroll recipes or default learning route. Existing scroll/Vancian engine support and other packages remain untouched. If optional generic scroll learning is implemented as a reusable feature, it is separately disabled for this preset, respects admission and prerequisites, consumes according to its authored contract and grants only normal initial knowledge/grade. Reading or possessing a scroll never copies maker mastery or grants a free ordinary casting route.

## Optional installer and content ownership

### Package contents and dependency order

Use one discoverable optional Armageddon magic installer with explicit dependency metadata. Reuse existing seeder framework rather than registering a special case in `Program` or building a second installer. All concrete seeders implement `IDatabaseSeeder`; keep partials under the seeder's owning folder and data/manifests under the established assets layout.

Install in validated dependency order:

1. Resolve native attributes, required skills/improvers/checks, language bindings, units/materials and supporting world primitives
2. Create/reuse stable schools, resources, cap/regeneration progs and environment/gathering policy references
3. Create builder-editable tags, materials, foci, reagent/supply prototypes, substance carriers, NPC templates/AI and temporary-topology templates
4. Author shared spell definitions, grade/efficiency/formula/variant/lifecycle profiles and complete effect configuration
5. Install exact admissions, source skill openings/caps and scoped prerequisite edges for each capability
6. Install capability merits and the Sorcerer/Preserver/Defiler policy variants
7. Install compatible charged wand/staff and substance production templates; exclude spell scrolls
8. Optionally install matching native `ChargenRoleType.Class` roles only when classes are enabled and this option is selected
9. Emit a concise installed/conflicted/skipped report and builder integration guide

The stock traditions are Earth, Fire, Water, Wind, Shadow, Lightning and Void plus the three Sorcerer policy configurations. Elemental memberships use the existing explicit register, with uncertain historical guild-tree claims labelled as designed FutureMUD stock rather than invented historical facts. There is no requirement to duplicate spells per tradition or per Sorcerer variant.

Capabilities and supplies must be usable after explicit builder attachment, without hand-editing code or inventing missing prototypes. “Builder integrates” means deciding who gets access and where content appears, not finishing the mechanics. Classless worlds must receive the complete magic capability package. Do not enable classes, overwrite role-picker settings, attach capabilities to existing players, enrol every character or place NPCs while installing.

Use persisted shared seeder answers and inspect the actual class configuration; do not infer that classes are enabled merely because a role row exists. If saved configuration and the current role picker disagree, surface a specific non-destructive choice/report. Optional classes remain builder-controlled and carry the correct capability merit, skill/support links and documented enrolment hook.

### Ownership manifest

Maintain stable `arm.*` keys independently of display names and numeric IDs. Record package/version, entity type, ID plus revision where applicable, owned fields, last-installed canonical hash, dependency keys and adoption/tombstone status. Reuse a generic manifest facility if already delivered by another task; otherwise extend the shared seeder framework with tests rather than inventing incompatible ownership rules. Names alone are never permission to take over a builder's entity.

A rerun must:

- Create genuinely missing stock only when ownership/tombstone policy permits; distinguish an intentional builder deletion/retirement from a failed partial install
- Update only unchanged stock-owned fields and links by comparing the last installed value, current value and proposed value
- Preserve edited fields, names, custom clones and unrelated links, and report conflicts with stable keys and actionable explanations
- Preserve IDs and revisions where supported; allocate revisions/IDs through native patterns for composite-key entities
- Avoid silently resurrecting retired stock, reclaiming a clone's key or reverting a user's deliberate configuration
- Be retry-safe after failure, including partial committed work; report completed, failed and unstarted steps honestly
- Never grant/refill/recharge/reset/trim player acquisition, raw skills, mastery, opportunity clocks, enrolment, balances, charges or inventory

Do not let a “missing stock repair” act as an unauthorised player migration. Content repair and player-state migration are separate operations. Cloning releases stock ownership for the clone and gives any internal policy/admission IDs the correct independent identity without copying player state.

### Workflow and replay

Follow the repository's seeder repeatability strategy and `futuremud-database-seeder` skill. If adding an enabled seeder or changing dependencies/questions/defaults, update every Debug replay profile deliberately: concrete seeder types, exact strict inventory, dependency order and all question IDs including conditionally inactive ones. Keep the shared execution/answer path, Debug-only replay, warning and fresh-local-database refusal. Never make an invalid profile silently fall back to interactive defaults.

Build the seeder in Debug and Release; run focused replay tests plus the whole seeder suite. On a disposable newly migrated database, run a supported fresh profile and prove the second Debug replay attempt is refused without mutation. Separately test the installer's supported ownership-aware rerun on an installed world; these two behaviours are intentionally different.

## Proposed implementation phases

Phases are dependency boundaries, not permission to omit later work or advertise completion early. One coordinator owns integration and release acceptance. Keep source writing isolated; follow repository delegation/test policies instead of launching a large speculative task tree.

| Phase | Work and principal files | Exit evidence |
| --- | --- | --- |
| 0 Rebase and reconcile | Current `master`, `Design Documents/Magic/Armageddon_Casting_*.md`, `Armageddon_Repertoire.json/.md`, existing A-D10 work if landed | Current commit and delivery matrix; no redo of PR771/778; complete 154 disposition plus exact 82 membership |
| 1 Extend progression and energy | `FutureMUDLibrary/Magic/IMagicCastingCapability.cs`, `IMagicCastingService.cs`, `ISpellNumericalContext.cs`; `MagicCastingService.Acquisition.cs`, `.Resources.cs`, `.Execution.cs`; capability casting builder; `MagicSpell.Grades.cs`, `.Casting.cs`; native improvement classes | Typed support edges, source openings/caps, cap-aware maximum mastery, source-shaped efficiency, backwards compatibility and PR778 permutations pass |
| 2 Finish invocation adapters | `MudSharpCore/Commands/Modules/MagicModule.Casting.cs`, communication module/body strategy integration, `CharacterCommunication.cs`, language/emote hooks, magic casting service | Genuine speech/name/formula parity, no recursion, quiet policy, explicit area modes and side-effect-free practice |
| 3 Implement reusable effect and lifecycle gaps | Shared lifecycle interfaces; `MudSharpCore/Magic/SpellEffects/`, `Effects/Concrete/SpellEffects/`, character instance service, items, native movement/AI/topology seams | Eight larger features and shared small mechanisms with native ownership/restart/cleanup/command-authority tests |
| 4 Finish portable production | `MagicSpell.Production.cs`, existing snapshot compatibility and numerical context, game-item component/prototype families, substance machinery | Focus/charged separation, caster-only stock, configurable eligibility, reproduce-power recharge, no stock scroll dependency |
| 5 Author full stock and installer | New owning `DatabaseSeeder/Seeders/ArmageddonMagicSeeder/` or equivalent agreed name; shared ownership framework; manifests; replay metadata and profiles | Complete usable stock, exact 82, classless and optional-class installs, safe reruns and conflict receipts |
| 6 Native release qualification | Core/library/seeder/persistence test owners, disposable MySQL fixtures and ordinary non-admin installed-world scenarios; update guides/receipts | All production-readiness gates below, complete per-spell evidence, balance report and no hidden unrun blockers |

New filenames are suggestions; actual implementations should fit current owning modules. Do not create duplicate services solely to match this table. Review and update owning design documents in the same implementation change as runtime contracts. Consult the item-system, native communication, character-instance, environmental/gathering and seeding documents only for affected surfaces.

If schema changes are needed, `MudsharpDatabaseLibrary` owns models/mappings/migrations. Generate using the repo's SDK/EF versions, align designer/model/blank snapshots, verify clean import and upgrade from the baseline, and leave production connections untouched. Preserve existing casting operation data and profile compatibility.

## Verification plan

### Automated tests

Choose the owning suite. Core behaviour belongs in `MudSharpCore Unit Tests`; shared value objects/parsers in `FutureMUDLibrary Unit Tests`; seeder contents/reruns/replay in `DatabaseSeeder Unit Tests`; mappings/upgrades in `MudsharpDatabaseLibrary Unit Tests`; formula changes in `ExpressionEngine Unit Tests` when applicable. Use deterministic clocks/randomness for assertions, plus separate real progression/balance exercises. Native test results must retain complete counts, no unexplained skips and source fingerprints.

Required groups include:

1. Existing casting acquisition, integration, ownership and all PR778 cascade permutations
2. Exact82 membership, four roots, source openings/caps/parent thresholds, all 94 row schema, no accidental Armor/Planeshift/new-spell Sorcerer union
3. Component Crafting to Identify, support-trait scope, enrolment permanence, interrupted skill opening and second-body acquisition
4. Raw-cap-aware grade eligibility including Heal 60, stock practice reaching grade 7, builder-imposed restrictions actually taking effect
5. Mana cap attribute changes, current balance versus cap, source efficiency boundary/index cases, overreach and modifier combination, source minima and affordability
6. Name/formula/speech equivalent quote/outcome, true language/hearing output, malformed/ambiguous input, output-recursion prevention and relay/replay cases
7. Quiet speech physical eligibility and actual volume difficulty; spell-specific area caster/ally/grounded/plane/bounce policies; one invocation's payment/progression
8. Pure practice failure/interruption/restart with no target/caster effects, ward checks or material creation
9. PC/NPC charm whitelist and alias/nested dispatch escapes, effect expiry/controller loss/queued commands
10. Lifecycle, generated/foreign item conservation, early death, corpse dependencies, overdue restart recovery, bounded row growth and no owner deletion
11. Every larger feature's acceptance case and every small-gap composite's meaningful operation result
12. Charged/focus distinction, all eligibility modes, homogeneous snapshots, compatibility refusal, copying/stacking, recharge reproducibility and interrupted/uncertain payment
13. Installer class-on/off, no player mutation, stock edits/deletions/clones, version upgrades, partial failure, deterministic rerun and strict Debug replay
14. Regressions for legacy, Vancian, spell-backed powers, environmental gathering, land rejuvenation, substances, identity focus/death, native command permissions and ordinary communication

Use repository test wrappers where appropriate, inspect supported flags, run direct .NET commands with `-m:1`, and use `--no-restore` after a successful restore. Target projects rather than defaulting to a full solution build with unavailable VSIX targets. Report audit/network/tooling workarounds separately; they are not passing security or gameplay evidence.

### Native acceptance and worked cases

Execute against a uniquely named disposable local MySQL database and a real seeded-world non-admin session. Do not use an unknown/shared/production database. Record revision, fixture construction, commands, expected/actual observations, actual state assertions, restart boundaries and receipt IDs. A controlled harness plus a real player session are complementary; neither factory tests nor admin-only direct effect calls replace them.

| Case | Scenario | Required result |
| --- | --- | --- |
| N01 Fresh classless install | Install prerequisites and package with classes off; builder attaches a capability/merit to a test character and enrols through the supported path | All dependencies usable; no required class; no production/player-wide grant; zero initial reserve until earned |
| N02 Optional classes | Classes enabled, opt into matching roles, choose one on a disposable new character | Correct merit/support/enrolment wiring; existing roles/settings/players untouched |
| N03 Shared spell routes | Elementalist and Sorcerer invoke one native shared spell with explicit route | Same spell/native school; correct independent reserve/trait; no auto-route based on richest pool |
| N04 Exact source tree | Sorcerer roots 60; advance Sense Enchantment from below 80 to 80 | Correct child opens 30, capped 90, grade1; all sibling/diamond/order rules preserved; no guessed roots |
| N05 Support branch | Acquire Shadow Passage, meet its threshold, improve Component Crafting to 80 | Read Enchantment becomes acquired through the legitimate native support edge, without a fake spell or manual database edit |
| N06 Low cap maximum | Practice Mend Flesh from legitimate acquisition through normal progression to maximum controlled grade | Raw skill never exceeds60; grade 7 is attainable without a separate practice cap or manifest requirement |
| N07 Source efficiency | Controlled grades1,2,7 request grade1, with a source minimum7 | Reference pre-modifier curve 50,25,7; actual configured costs match displayed quote; no double grade multiplier |
| N08 Capacity and gathering | Change cap-driving attribute, remove/re-add capability, switch bodies, gather with Sorcerer/Preserver/Defiler | No free refill/duplicated reserve; correct method restrictions and environmental accounting; no Sorcerer passive leakage |
| N09 Formula parity | Named grade3 cast, command formula using `kral`, actual spoken equivalent | Same admission/grade/cost pipeline, one invocation each, correct native language echo; listener receives only perceptible speech |
| N10 Echo recursion | Cast near another listener/relay/proxy with matching formula hooks | Generated output never recasts; overhearing never grants knowledge; repeated callback cannot debit twice |
| N11 Quiet and area | Whisper an eligible spell; execute Earthquake, Chain Lightning and room Fireball policies | Quoted modifiers and physical volume apply; visible effects remain; source-specific caster policies differ correctly |
| N12 Practice purity | Practice hostile magic beside warded, injured and hostile actors; interrupt and restart | No effects/target resistance/ward use; full configured payment once; no interrupted gain/refund/replay |
| N13 PC charm | Charm a consenting test PC fixture, order allowed IC action then alias to forbidden settings/builder/OOC command | Allowed gameplay executes under ordinary gates; all forbidden final commands refuse on PC and NPC; no privilege crossover |
| N14 Summon early death | In a fixture explicitly configured to leave remains, kill a temporary guardian, retain corpse and foreign carried item, pass original expiry, restart, then decay corpse; separately exercise default dissipation | No resurrection/second death; foreign item conserved; temporary heavy graph eventually retired when references clear |
| N15 Lifecycle modes | Same template exercised as permanent, temporary cleanup and death-on-expiry | Correct persistence/death/remains behaviour and exactly one terminal transition; permanent creation survives its no-expiry contract |
| N16 High-volume retirement | Repeated summon/use/kill/expire/restart with deterministic terminal checks | Stable eligible heavy-row/timer/subscription counts after cleanup; every retained record explained |
| N17 Occupied shelters | Use Spring Haven, Burrow Refuge and Sand Shelter; expire/destroy while occupied | Real shelter utility/finite water; no stranded/deleted actors or foreign possessions; safe fallback when anchor missing |
| N18 Effigy and shadow | Create each, switch focus, damage/sever source and restart | Correct instance identity, anchor and focus; canonical skills/resources not duplicated; owner survives temporary collapse as configured |
| N19 Ward and bane | Use Severing Refuge against allowed/blocked element interactions; Apex Bane against eligible/ineligible beings | Functional bounded ward and resisted category attack, not cosmetic effects or name checks |
| N20 Pocket conservation | Fill Folded Pocket, attempt pocket nesting/body entry, transfer and collapse after restart | Capacity enforced, nesting/living targets rejected, all foreign contents released exactly once |
| N21 Permanent tier exception | Exercise timed Flame Knife tier and configured highest-tier permanent output with real component | Correct source-informed per-tier output; timed gear cannot yield permanent salvage; permanent output properly paid |
| N22 Item activation | Caster and non-caster use stock charged wand; builder changes one test item to anyone; try depleted charge then explicit focus | Stock caster gate, configurable exception, preserved captured potency; no free fallback or acquired/mastery gains |
| N23 Recharge | Qualified producer, wrong magic type, unacquired spell, insufficient grade and insufficient reproducible potency | Only legitimate producer succeeds; bounded charge/payment commit, no mixed payload or duplicate charge after restart |
| N24 Rerun safety | Rename/edit stock; intentionally delete/retire one entry; clone another; interrupt install; rerun | Preserve edits/clones/players, report conflicts, honour retirement, repair only owned partial work and preserve stable IDs |
| N25 Cross-feature uncertain failure | Inject provider failure after debit/effect/progression or lifecycle transition; retry via another body/route and restart | Conservative quarantine/reconciliation, no replay/refund/double grade/second corpse; unrelated records remain usable |

Additionally execute at least one ordinary non-admin manifestation for every included repertoire spell using its real seeded configuration. Use representative parameter sweeps for reusable mechanisms, but do not claim a group-level test exercises a spell never invoked. Each entry must link to its own evidence for defining behaviour, target refusal, low/high grade, lifetime and any applicable counter-effect.

### Production readiness gates

A release candidate is ready only when all of the following hold:

- Approved Q01–Q16 behaviour is implemented, including all eight larger features; no required spell is a placeholder, disabled shell or “future work” entry
- Sorcerer membership is exactly82 with exact four roots and source tree/openings/caps; all branches including support-skill bridges are attainable
- Every stock capability is fully usable with builder attachment; classless and optional-class paths work; supplies and summoned templates are functional
- Maximum normal grade is attainable through stock practice, every advertised spell/grade/mode is affordable by a documented legitimate configuration, and progression/energy tests do not rely only on admin overrides
- Speech and command parity, per-spell area, PC command safety, legitimate recharging and no-stock-scroll rules are proven
- Temporary entity cleanup is restart-safe, possession-safe, reference-safe and demonstrably bounded; permanent outputs remain permanent by explicit policy
- Complete catalogue dispositions and evidence cover all 154 candidates, with any non-installed optional addition or evidence-limited adaptation honestly labelled; no omitted required behaviour is disguised by a changed count
- Ownership reruns preserve customisations and every player-state table; strict replay and schema/blank-snapshot upgrade/import checks pass
- Native installed-world acceptance, per-spell receipts, relevant automated suites and the retained environmental/04C deadline/checkpoint gate pass at the final revision
- Documentation, actual command help, example templates, stock manifests and configuration validators agree; a builder can customise the package without coding
- No unresolved data-loss, privilege/control bypass, charge duplication, infinite retry/timer, affordability or unreachable-progression defect remains; unavailable checks are listed as blockers rather than passes

The completion report must distinguish implemented, verified, blocked and intentionally optional work. Report actual tests/counts and native receipts, unresolved limitations and the final revision. Do not present old PR receipts as fresh full-preset acceptance.

## Builder integration guide to ship

Ship a short practical guide alongside detailed runtime documentation. It should let a builder:

1. Install dependencies and the optional package, read the readiness/conflict report and locate stock keys
2. Inspect a capability, its spell list, source openings/caps/branches, resource cap attribute, gathering methods, formula language and item eligibility
3. Clone a stock definition safely, change values through ordinary builders and validate before enabling it
4. Attach an appropriate capability merit to a test character/NPC template, explicitly enrol it and confirm root skills/knowledge and starting reserve
5. Choose optional matching class roles only in a class-enabled world, then wire the documented chargen-finalisation hook without changing existing players
6. Place chosen NPCs/supplies through ordinary world-building tools and test speech, cast, practice, gather, summon and item-use commands as a non-admin
7. Configure PC charm eligibility/whitelist, practice restrictions, volume difficulty, area policies, summoned lifetime/corpse behaviour and permanent output tiers
8. Re-run safely, resolve an ownership conflict explicitly, understand stock deletion/retirement and inspect operation/lifecycle diagnostics

Show real validated commands from the implemented builders; examples in this document describe intended features and are not proof that their exact final syntax already exists. Update `Configurable_Casting.md`, the repertoire and decision/integration documents to remove superseded claims such as guessed Sorcerer membership, generic POWER defaults, NPC-only charm, globally temporary conjuration, universal caster exclusion, stock scrolls and fixed-pool linear cost. Keep clearly marked historical design records where useful.

## Source and evidence index

- [Current baseline commit](https://github.com/FutureMUD/FutureMUD/commit/b0e4339d57bf15bc6e6a6f21fbdd5edc73006e38)
- [Merged configurable casting foundation PR771](https://github.com/FutureMUD/FutureMUD/pull/771) and [merged cascade fix PR778](https://github.com/FutureMUD/FutureMUD/pull/778)
- [Configurable casting guide at baseline](https://github.com/FutureMUD/FutureMUD/blob/b0e4339d57bf15bc6e6a6f21fbdd5edc73006e38/Design%20Documents/Magic/Configurable_Casting.md) and [handover](https://github.com/FutureMUD/FutureMUD/blob/b0e4339d57bf15bc6e6a6f21fbdd5edc73006e38/Design%20Documents/Magic/Configurable_Casting_Handover.md)
- [Original candidate register](https://github.com/FutureMUD/FutureMUD/blob/b0e4339d57bf15bc6e6a6f21fbdd5edc73006e38/Design%20Documents/Magic/Armageddon_Repertoire.json) and [original integration evidence](https://github.com/FutureMUD/FutureMUD/blob/b0e4339d57bf15bc6e6a6f21fbdd5edc73006e38/Design%20Documents/Magic/Armageddon_Casting_Integration.md), read subject to this brief's explicit supersessions
- [Selected historical Sorcerer tree](http://armageddonmud.freetzi.com/sorcerer.html) and [verified 2016 archive](http://web.archive.org/web/20160106231707/http://armageddonmud.freetzi.com:80/sorcerer.html)
- Supplied historical `codedump.c`: 8,354,060 bytes, 242,965 lines; recorded SHA-256 `e44778f6a5fb9fd02db5c3ff78d4c468b93ae6c854a058d5af769dc0a95cd59f`. Relevant function/line observations are summarised in Appendix C. The source snapshot has not been authenticated as the exact server version used by the selected 2013 tree
- [Repository instructions](https://github.com/FutureMUD/FutureMUD/blob/b0e4339d57bf15bc6e6a6f21fbdd5edc73006e38/AGENTS.md), applicable project/module instructions, and [seeder repeatability strategy](https://github.com/FutureMUD/FutureMUD/blob/b0e4339d57bf15bc6e6a6f21fbdd5edc73006e38/Design%20Documents/Seeding/DatabaseSeeder_Repeatability_Strategy.md)

The full historical C source is not reproduced or required to interpret this document's approved contracts. Any implementation audit should use these precise observations and the current repository before broadening the source investigation.

## Appendix A Exact selected Sorcerer tree

This is the complete selected source tree, in source order. `Parent threshold` is the parent's printed go-on value. `Branches at` is this row's go-on value for its children. These are raw proficiency values. The four spell roots have starting grants; support roots are separate from automatic magic knowledge. Source spell IDs are provenance, never FutureMUD database IDs. Stable keys for support rows are proposed bindings; their source names and numbers are exact.

The values below make this Markdown independently usable; no companion file or private source access is required for the selected tree. The published `min_mana` is a computed lowest-power cost in the source print routine; do not mistake it for a proof of the raw `skill[].min_mana` field.

| # | Kind and historical name | FutureMUD stable key | Parent | Parent threshold | Opens | Raw cap | Branches at | Printed min mana |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | spell: detect magick | `arm.spell.sense_enchantment` | root | — | 60 | 90 | 80 | 7 |
| 2 | spell: detect invisible | `arm.spell.pierce_concealment` | detect magick | 80 | 30 | 90 | 80 | 7 |
| 3 | spell: infravision | `arm.spell.night_eyes` | detect invisible | 80 | 30 | 90 | 80 | 7 |
| 4 | spell: cure blindness | `arm.spell.clear_eyes` | infravision | 80 | 30 | 90 | 80 | 7 |
| 5 | spell: remove curse | `arm.spell.lift_hex` | infravision | 80 | 30 | 90 | 80 | 7 |
| 6 | spell: detect ethereal | `arm.spell.see_the_unbodied` | detect invisible | 80 | 30 | 90 | 80 | 7 |
| 7 | spell: detect poison | `arm.spell.sense_toxin` | detect ethereal | 80 | 30 | 90 | 80 | 7 |
| 8 | spell: tongues | `arm.spell.borrowed_tongues` | detect ethereal | 80 | 30 | 90 | 80 | 7 |
| 9 | spell: invisibility | `arm.spell.veil_from_sight` | detect magick | 80 | 30 | 90 | 80 | 7 |
| 10 | spell: send shadow | `arm.spell.walking_shadow` | invisibility | 80 | 30 | 90 | 80 | 7 |
| 11 | spell: glyph | `arm.spell.custodian_glyph` | send shadow | 80 | 30 | 90 | 80 | 10 |
| 12 | spell: curse | `arm.spell.clinging_hex` | send shadow | 80 | 30 | 90 | 80 | 7 |
| 13 | spell: ethereal | `arm.spell.shadow_passage` | invisibility | 80 | 30 | 90 | 80 | 20 |
| 14 | spell: determine relationship | `arm.spell.read_attunement` | ethereal | 80 | 30 | 90 | 80 | 20 |
| 15 | support: component crafting | `arm.support.component_crafting` | ethereal | 80 | 30 | 90 | 80 | — |
| 16 | spell: identify | `arm.spell.read_enchantment` | component crafting | 80 | 30 | 90 | 80 | 7 |
| 17 | spell: dispel ethereal | `arm.spell.draw_into_flesh` | ethereal | 80 | 30 | 90 | 80 | 7 |
| 18 | spell: dispel invisibility | `arm.spell.reveal_the_hidden` | invisibility | 80 | 30 | 90 | 80 | 7 |
| 19 | spell: dispel magick | `arm.spell.unravel_enchantment` | root | — | 60 | 90 | 80 | 7 |
| 20 | spell: create water | `arm.spell.draw_water` | dispel magick | 80 | 30 | 90 | 80 | 7 |
| 21 | spell: create food | `arm.spell.sustain_meal` | create water | 80 | 30 | 90 | 80 | 7 |
| 22 | spell: ball of light | `arm.spell.hovering_light` | create food | 80 | 30 | 90 | 80 | 7 |
| 23 | spell: alarm | `arm.spell.threshold_alarm` | ball of light | 80 | 30 | 90 | 80 | 33 |
| 24 | spell: empower | `arm.spell.kindle_enchantment` | create food | 80 | 30 | 90 | 80 | 33 |
| 25 | spell: repair item | `arm.spell.restore_object` | create food | 80 | 30 | 90 | 80 | 7 |
| 26 | spell: mount | `arm.spell.earth_mount` | create water | 80 | 30 | 90 | 80 | 20 |
| 27 | spell: gate | `arm.spell.call_outsider` | mount | 80 | 30 | 90 | 80 | 50 |
| 28 | spell: guardian | `arm.spell.air_guardian` | mount | 80 | 30 | 90 | 80 | 33 |
| 29 | spell: sand statue | `arm.spell.sand_effigy` | guardian | 80 | 30 | 90 | 80 | 20 |
| 30 | spell: messenger | `arm.spell.wind_courier` | guardian | 80 | 30 | 90 | 80 | 7 |
| 31 | spell: create wine | `arm.spell.draw_wine` | create water | 80 | 30 | 90 | 80 | 7 |
| 32 | spell: breathe water | `arm.spell.water_breathing` | create wine | 80 | 30 | 90 | 80 | 20 |
| 33 | spell: fury | `arm.spell.roused_fury` | dispel magick | 80 | 30 | 90 | 80 | 20 |
| 34 | spell: blind | `arm.spell.shroud_eyes` | fury | 80 | 30 | 90 | 80 | 7 |
| 35 | spell: poison | `arm.spell.venom_touch` | blind | 80 | 30 | 90 | 80 | 7 |
| 36 | spell: feeblemind | `arm.spell.clouded_will` | poison | 80 | 30 | 90 | 80 | 20 |
| 37 | spell: create darkness | `arm.spell.deep_darkness` | poison | 80 | 30 | 90 | 80 | 7 |
| 38 | spell: pseudo death | `arm.spell.feign_death` | blind | 80 | 30 | 90 | 80 | 20 |
| 39 | spell: sleep | `arm.spell.heavy_slumber` | pseudo death | 80 | 30 | 90 | 80 | 20 |
| 40 | spell: paralyze | `arm.spell.still_limbs` | sleep | 80 | 30 | 90 | 80 | 20 |
| 41 | spell: animate dead | `arm.spell.raise_servitor` | pseudo death | 80 | 30 | 90 | 80 | 7 |
| 42 | support: vloran | `arm.support.vloran` | animate dead | 80 | 40 | 100 | 90 | — |
| 43 | spell: dead speak | `arm.spell.voice_of_remains` | animate dead | 80 | 30 | 90 | 80 | 33 |
| 44 | spell: recite | `arm.spell.echo_servant` | dead speak | 80 | 30 | 90 | 80 | 7 |
| 45 | spell: calm | `arm.spell.still_anger` | fury | 80 | 30 | 90 | 80 | 7 |
| 46 | spell: heal | `arm.spell.mend_flesh` | calm | 80 | 30 | 60 | 50 | 20 |
| 47 | spell: cure poison | `arm.spell.purge_toxin` | calm | 80 | 30 | 90 | 80 | 7 |
| 48 | spell: hands of wind | `arm.spell.gust_hands` | root | — | 60 | 90 | 80 | 7 |
| 49 | spell: teleport | `arm.spell.waystep` | hands of wind | 80 | 30 | 90 | 80 | 7 |
| 50 | spell: banishment | `arm.spell.banish_traveller` | teleport | 80 | 30 | 90 | 80 | 7 |
| 51 | spell: summon | `arm.spell.draw_traveller` | banishment | 80 | 30 | 90 | 80 | 50 |
| 52 | spell: slow | `arm.spell.leadfoot` | banishment | 80 | 30 | 90 | 80 | 33 |
| 53 | spell: repel | `arm.spell.driving_gust` | teleport | 80 | 30 | 90 | 80 | 7 |
| 54 | spell: portal | `arm.spell.open_threshold` | repel | 80 | 30 | 90 | 80 | 33 |
| 55 | spell: wall of wind | `arm.spell.wind_barrier` | repel | 80 | 30 | 90 | 80 | 10 |
| 56 | spell: levitate | `arm.spell.buoyant_lift` | hands of wind | 80 | 30 | 90 | 80 | 7 |
| 57 | spell: mark | `arm.spell.arcane_mark` | levitate | 80 | 30 | 90 | 80 | 25 |
| 58 | spell: godspeed | `arm.spell.fleet_step` | mark | 80 | 30 | 90 | 80 | 10 |
| 59 | spell: quickening | `arm.spell.lightning_pace` | godspeed | 80 | 30 | 90 | 80 | 33 |
| 60 | spell: travel gate | `arm.spell.linked_threshold` | mark | 80 | 30 | 90 | 80 | 33 |
| 61 | spell: fly | `arm.spell.windborne_flight` | levitate | 80 | 30 | 90 | 80 | 25 |
| 62 | spell: relocate | `arm.spell.join_traveller` | fly | 80 | 30 | 90 | 80 | 7 |
| 63 | spell: create rune | `arm.spell.anchor_rune` | relocate | 80 | 30 | 90 | 80 | 50 |
| 64 | spell: burrow | `arm.spell.burrow_refuge` | fly | 80 | 30 | 90 | 80 | 20 |
| 65 | spell: invulnerability | `arm.spell.unyielding_veil` | root | — | 60 | 90 | 80 | 7 |
| 66 | spell: lightning bolt | `arm.spell.lightning_lance` | invulnerability | 80 | 30 | 90 | 80 | 20 |
| 67 | spell: fireball | `arm.spell.ember_lance` | lightning bolt | 80 | 30 | 90 | 80 | 20 |
| 68 | spell: earthquake | `arm.spell.ground_tremor` | fireball | 80 | 30 | 90 | 80 | 7 |
| 69 | spell: chain lightning | `arm.spell.leaping_lightning` | fireball | 80 | 30 | 90 | 80 | 7 |
| 70 | spell: lightning spear | `arm.spell.storm_spear` | chain lightning | 80 | 30 | 90 | 80 | 20 |
| 71 | spell: fire jambiya | `arm.spell.flame_knife` | lightning bolt | 80 | 30 | 90 | 80 | 7 |
| 72 | spell: fear | `arm.spell.dread_presence` | fire jambiya | 80 | 30 | 90 | 80 | 7 |
| 73 | spell: aura drain | `arm.spell.empty_aura` | fear | 80 | 30 | 90 | 80 | 7 |
| 74 | spell: stamina drain | `arm.spell.steal_breath` | fear | 80 | 30 | 90 | 80 | 7 |
| 75 | spell: immolate | `arm.spell.immolate` | fire jambiya | 80 | 30 | 90 | 80 | 20 |
| 76 | spell: demonfire | `arm.spell.consuming_flame` | immolate | 80 | 30 | 90 | 80 | 33 |
| 77 | spell: energy shield | `arm.spell.charged_aegis` | invulnerability | 80 | 30 | 90 | 80 | 7 |
| 78 | spell: wall of sand | `arm.spell.sand_barrier` | energy shield | 80 | 30 | 90 | 80 | 10 |
| 79 | spell: sanctuary | `arm.spell.sheltering_veil` | wall of sand | 80 | 30 | 90 | 80 | 7 |
| 80 | spell: stone skin | `arm.spell.stone_skin` | wall of sand | 80 | 30 | 90 | 80 | 7 |
| 81 | spell: strength | `arm.spell.earthen_strength` | stone skin | 80 | 30 | 90 | 80 | 20 |
| 82 | spell: blade barrier | `arm.spell.blade_barrier` | energy shield | 80 | 30 | 90 | 80 | 10 |
| 83 | spell: wall of fire | `arm.spell.flame_barrier` | blade barrier | 80 | 30 | 90 | 80 | 10 |
| 84 | spell: wind armor | `arm.spell.wind_mantle` | energy shield | 80 | 30 | 90 | 80 | 7 |
| 85 | support: contact | `arm.support.contact` | root | — | 20 | 80 | 70 | — |
| 86 | support: barrier | `arm.support.barrier` | root | — | 5 | 80 | 70 | — |
| 87 | support: expel | `arm.support.expel` | barrier | 70 | 5 | 80 | 70 | — |
| 88 | support: dual wield | `arm.support.dual_wield` | root | — | 5 | 20 | 10 | — |
| 89 | support: shield use | `arm.support.shield_use` | root | — | 5 | 10 | 0 | — |
| 90 | support: forage | `arm.support.forage` | root | — | 5 | 50 | 40 | — |
| 91 | support: cooking | `arm.support.cooking` | root | — | 5 | 80 | 70 | — |
| 92 | support: analyze | `arm.support.analyze` | root | — | 10 | 90 | 80 | — |
| 93 | support: two handed | `arm.support.two_handed` | root | — | 5 | 20 | 10 | — |
| 94 | support: gather | `arm.support.gather` | root | — | 10 | 90 | 80 | — |

Spell admissions must equal the 82 spell keys in this table, with no other Sorcerer spell memberships. Remove Sorcerer admission from the old register's Armor, Weaken, Feather Fall, Refresh, Insomnia, Regenerate, Psionic Suppression, Planeshift, Possess Corpse, Phantasm, Disembody, Wardcraft and Renew Earth proposals. Do not delete their shared spell definitions or another tradition's admissions. All three Sorcerer policy variants use this same selected list.

## Appendix B Complete candidate implementation inventory

All154 existing stable keys are listed below. “Primitive” and “Policy” mean existing ingredients plus full configuration, integration and native verification; neither means the finished spell already exists. “Gap” and “Larger” point to additional reusable mechanisms required in this brief. All entries also require the applicable common casting, practice, item, lifecycle, seeder and acceptance work.

Source-confirmed corrections in this brief supersede older per-entry descriptions, in particular PC charm, food lifetime, area caster rules and permanent highest-tier staff outputs. Preserve the original register's metadata/evidence links while updating its contracts, status and tests. Wardcraft/Renew Earth are labelled additions and never part of the selected historical82.

| Stable key | Spell | Completion group | Defining result or remaining entry work |
| --- | --- | --- | --- |
| `arm.spell.sense_enchantment` | Sense Enchantment | Primitive | Reveal perceptible magical effects to the recipient. |
| `arm.spell.ember_lance` | Ember Lance | Policy | Strike one visible target with fire damage. |
| `arm.spell.cinder_rain` | Cinder Rain | Policy | Burn eligible others in the current cell with an initial fire strike and timed burning. |
| `arm.spell.hovering_light` | Hovering Light | Gap | Create one temporary light around the recipient. Generic creation does not prove auto-equip or linked cleanup; use a worn-light lifecycle adapter or approve a glow-only substitute. |
| `arm.spell.unravel_enchantment` | Unravel Enchantment | Policy | Contest and remove spell-owned effects matching configured dispel keys. |
| `arm.spell.pillar_of_flame` | Pillar of Flame | Policy | Deliver a stronger single-target fire strike. |
| `arm.spell.consuming_flame` | Consuming Flame | Policy | Deliver an intense fire strike requiring a consumed reagent. |
| `arm.spell.kindle_enchantment` | Kindle Enchantment | Policy | Temporarily improve a selected weapon's damage through native enchantment hooks. |
| `arm.spell.custodian_glyph` | Custodian Glyph | Gap | Mark an item so unauthorised taking is refused while the glyph lasts. Metadata alone does not intercept taking; requires a bounded inventory permission effect. |
| `arm.spell.fireworks` | Fireworks | Policy | Display an audience-visible burst of coloured flame and light without physical damage. |
| `arm.spell.borrowed_tongues` | Borrowed Tongues | Primitive | Allow comprehension of speech without granting fluent speech. |
| `arm.spell.burning_draught_curse` | Burning Draught Curse | Gap | Make later drinking inflict configured burning harm instead of ordinary refreshment. Needs an ingestion event interceptor; an immediate damage or need delta is not equivalent. |
| `arm.spell.desiccate` | Desiccate | Policy | Reduce hydration and apply a small fire injury. |
| `arm.spell.flame_knife` | Flame Knife | Gap | Timed low-tier fire weapon; mon component-consuming permanent staff output; source-informed prototypes and explicit per-tier ownership/cleanup |
| `arm.spell.flame_barrier` | Flame Barrier | Gap | Block an exit with a timed barrier and burn actors who attempt to cross. Blocking exists; traversal fire damage and both-side output require a bounded adapter. |
| `arm.spell.flame_mantle` | Flame Mantle | Policy | Apply a finite protective armour layer with a visible warm glow. |
| `arm.spell.ember_seed` | Ember Seed | Gap | Create one temporary ember seed carrying a configured fire payload. Creation alone is insufficient for delayed activation, paid payload and cleanup. |
| `arm.spell.daylight` | Daylight | Primitive | Increase room illumination for a bounded duration. |
| `arm.spell.immolate` | Immolate | Policy | Apply timed fire injury ticks to the target. |
| `arm.spell.draw_water` | Draw Water | Gap | Add clean water to an accessible drink container up to capacity. Current CreateLiquidEffect empty-container capacity/null path needs a focused correction before claiming this complete. |
| `arm.spell.sense_toxin` | Sense Toxin | Primitive | Let the recipient inspect detectable active and latent drug poison state. |
| `arm.spell.mend_flesh` | Mend Flesh | Primitive | Heal existing eligible wounds, worst first, within the numeric healing budget. |
| `arm.spell.venom_touch` | Venom Touch | Policy | Introduce a configured stock toxin through an explicit vector. |
| `arm.spell.sheltering_veil` | Sheltering Veil | Policy | Give the target a finite protective envelope against configured damage. |
| `arm.spell.purge_toxin` | Purge Toxin | Policy | Remove matching spell-owned poison effects and their originator-linked drug doses. |
| `arm.spell.still_anger` | Still Anger | Policy | Apply native pacifism to suppress aggressive behaviour for a duration. |
| `arm.spell.spring_haven` | Spring Haven | Larger | Real temporary sheltered watering place, finite usable water, terrain admission and occupancy-safe lifecycle |
| `arm.spell.unyielding_veil` | Unyielding Veil | Policy | Provide a stronger but finite armour pool. |
| `arm.spell.read_attunement` | Read Attunement | Gap | Report an explicitly configured relationship between a target and elemental traditions. Requires an agreed attunement vocabulary and bounded query; historical setting relationships are unresolved. |
| `arm.spell.thunderclap` | Thunderclap | Policy | Strike local eligible others with sonic harm and temporary deafness. |
| `arm.spell.hush_hearing` | Hush Hearing | Primitive | Temporarily suppress the recipient's hearing. |
| `arm.spell.seal_voice` | Seal Voice | Primitive | Prevent normal speech through the native magical silence status. |
| `arm.spell.vital_siphon` | Vital Siphon | Gap | Harm a target and heal the caster by no more than the harm actually delivered. Requires linked measured transfer; independent damage and caster-heal effects would over-credit resisted harm. |
| `arm.spell.thorn_barrier` | Thorn Barrier | Gap | Block an exit with temporary thorns that injure crossing attempts. Traversal injury/entanglement and reciprocal presentation need explicit support. |
| `arm.spell.draw_wine` | Draw Wine | Gap | Add configured wine to an accessible drink container without exceeding capacity. Same empty-container path concern as Draw Water; never silently convert incompatible mixtures. |
| `arm.spell.headlong_revel` | Headlong Revel | Policy | Increase native drunkenness without conjuring permanent drink objects. |
| `arm.spell.clear_head` | Clear Head | Primitive | Reduce native drunkenness toward zero. |
| `arm.spell.wither_growth` | Wither Growth | Gap | Damage a targeted plant-like item through an explicit material/tag gate. Source sap transfer and living land/crop accounting are not ordinary item damage; no ecological debit bypass. |
| `arm.spell.distant_mirage` | Distant Mirage | Policy | Show selected viewers a non-interactive misleading scene. |
| `arm.spell.mist_shield` | Mist Shield | Policy | Surround a character with a finite mist-themed protective layer. |
| `arm.spell.restorative_mud` | Restorative Mud | Gap | Produce one paid topical healing dose in a mud carrier. Exposure payload is feasible, but preset manufacturing and frozen-grade binding are not implemented. |
| `arm.spell.purge_disease` | Purge Disease | Policy | Remove a matching spell-owned disease effect and its tracked native infection. |
| `arm.spell.water_breathing` | Water Breathing | Primitive | Allow compatible breathing underwater while the spell lasts. |
| `arm.spell.earthen_ward` | Earthen Ward | Primitive | Apply a finite earthen protective armour layer. |
| `arm.spell.sustain_meal` | Sustain Meal | Policy | Create finite usable food with source-informed magical expiry plus ordinary consumption; no automatic permanence or recreation of consumed units |
| `arm.spell.ground_tremor` | Ground Tremor | Gap | Grounded-target earthquake and fall policy; local caster receives one-third source damage with authored protections; nearby-cell messages/falls are separate |
| `arm.spell.heavy_slumber` | Heavy Slumber | Primitive | Put a susceptible target into native magical sleep. |
| `arm.spell.earthen_strength` | Earthen Strength | Primitive | Increase the configured strength trait for a duration. |
| `arm.spell.sand_knife` | Sand Knife | Gap | Timed low-tier sand weapon; mon component-consuming permanent staff output; source-informed prototypes and explicit per-tier ownership/cleanup |
| `arm.spell.stone_skin` | Stone Skin | Policy | Apply stone-like protection with an agility penalty. |
| `arm.spell.sapping_weight` | Sapping Weight | Primitive | Apply a negative modifier to the configured strength trait. |
| `arm.spell.trace_the_path` | Trace the Path | Gap | Reveal a permitted bounded route toward a known target. Needs a permission-aware path information effect; teleport and track tokens are not proof of this behavior. |
| `arm.spell.burrow_refuge` | Burrow Refuge | Larger | Enterable temporary underground topology, terrain gate, safe return, occupied expiry and restart cleanup |
| `arm.spell.clouded_will` | Clouded Will | Policy | Reduce the target's explicitly configured magical reserve. |
| `arm.spell.roused_fury` | Roused Fury | Primitive | Apply native rage intensity for a duration. |
| `arm.spell.earth_mount` | Earth Mount | Gap | Create a rideable temporary earth creature belonging to its summoner. NPC creation is present; mount control and expiry with riders need bounded lifecycle work. |
| `arm.spell.sand_barrier` | Sand Barrier | Policy | Block a chosen exit temporarily with packed sand. |
| `arm.spell.return_tether` | Return Tether | Gap | Record the caster's position and return them after a bounded delay if still eligible. Needs durable delayed-return identity and cancellation semantics; not time reversal of health or inventory. |
| `arm.spell.threshold_alarm` | Threshold Alarm | Policy | Notify the configured owner of eligible entry through a room alarm. |
| `arm.spell.fleet_step` | Fleet Step | Gap | Increase movement speed and reduce locomotion effort for a duration. Trait bonuses and stamina multipliers alone do not establish movement-delay modification. |
| `arm.spell.sand_shelter` | Sand Shelter | Larger | Physically usable shelter with protection/occupancy and foreign-content-preserving teardown |
| `arm.spell.clay_sentinel` | Clay Sentinel | Gap | Animate a controlled temporary clay guardian. Needs creator command eligibility and teardown; historical golem summary is insufficient to infer those details. |
| `arm.spell.sand_effigy` | Sand Effigy | Larger | Bounded source-item-to-projection link using canonical identity, immobile copy/focus/anchor restrictions and safe collapse |
| `arm.spell.shatter_stone` | Shatter Stone | Policy | Damage a stone-tagged item with a finite damage budget. |
| `arm.spell.restore_object` | Restore Object | Gap | Repair eligible item damage without duplicating material or resetting quality. MendEffect treats wounds; an item-repair adapter is still required. |
| `arm.spell.waystep` | Waystep | Policy | Move the caster to an explicitly resolved permitted cell. |
| `arm.spell.pierce_concealment` | Pierce Concealment | Primitive | Let the recipient perceive eligible invisible entities. |
| `arm.spell.veil_from_sight` | Veil from Sight | Primitive | Apply native invisibility to the recipient. |
| `arm.spell.draw_traveller` | Draw Traveller | Policy | Move a permitted remote character to the caster's current cell. |
| `arm.spell.join_traveller` | Join Traveller | Policy | Move the caster to a permitted cell resolved from another character. |
| `arm.spell.buoyant_lift` | Buoyant Lift | Primitive | Apply native persistent levitation. |
| `arm.spell.scouring_gust` | Scouring Gust | Policy | Inflict wind-themed injury and stamina loss on eligible local targets. |
| `arm.spell.gust_hands` | Gust Hands | Primitive | Move the target along an explicitly selected traversable path. |
| `arm.spell.banish_traveller` | Banish Traveller | Policy | Send a target to a configured permitted destination after resistance. |
| `arm.spell.air_guardian` | Air Guardian | Gap | Create a temporary protective air creature. NPC creation does not establish guarding commands, summon ownership or safe expiry. |
| `arm.spell.exchange_places` | Exchange Places | Primitive | Swap caster and target spatial locations when native validation allows. |
| `arm.spell.windborne_flight` | Windborne Flight | Primitive | Grant the native magical flight status. |
| `arm.spell.seeking_wind` | Seeking Wind | Gap | Summon a temporary tracker with a specifically authorised quarry. Tracking AI and target authority/lifecycle are required; creating an NPC is not sufficient. |
| `arm.spell.gentle_descent` | Gentle Descent | Primitive | Reduce falling harm through the native featherfall effect. |
| `arm.spell.driving_gust` | Driving Gust | Policy | Force a target one eligible step through a chosen exit. |
| `arm.spell.wind_barrier` | Wind Barrier | Policy | Block passage through one exit with a wind-themed barrier. |
| `arm.spell.wind_mantle` | Wind Mantle | Primitive | Apply a finite wind-themed armour layer. |
| `arm.spell.wind_hammer` | Wind Hammer | Gap | Strike and then move a target through a selected exit only if the hit resolves. Requires ordered linked outcome handling; two independent effects could push a rejected target. |
| `arm.spell.false_presence` | False Presence | Policy | Hide the target's visible presence for a duration. |
| `arm.spell.wind_shield` | Wind Shield | Policy | Protect a character with a wind-themed armour envelope. |
| `arm.spell.wind_courier` | Wind Courier | Gap | Create a temporary courier that delivers a bounded message to an authorised recipient. Needs delivery/recipient permissions and summon expiry; no arbitrary command proxy. |
| `arm.spell.reveal_the_hidden` | Reveal the Hidden | Primitive | Remove eligible spell invisibility from the target. |
| `arm.spell.anchor_rune` | Anchor Rune | Policy | Mark an owned item as an explicitly registered travel anchor. |
| `arm.spell.shroud_eyes` | Shroud Eyes | Primitive | Apply native magical blindness. |
| `arm.spell.clear_eyes` | Clear Eyes | Primitive | Remove spell-induced blindness through the native removal effect. |
| `arm.spell.lift_hex` | Lift Hex | Primitive | Remove the eligible magical curse status. |
| `arm.spell.dread_presence` | Dread Presence | Primitive | Apply native magical fear to a susceptible target. |
| `arm.spell.night_eyes` | Night Eyes | Primitive | Grant native infravision for a duration. |
| `arm.spell.walking_shadow` | Walking Shadow | Larger | Controllable shadow instance with original body, canonical progress/resources, focus/anchor/plane limits and safe collapse |
| `arm.spell.shadow_passage` | Shadow Passage | Policy | Apply an approved ethereal planar-presence overlay. |
| `arm.spell.see_the_unbodied` | See the Unbodied | Primitive | Expose eligible ethereal entities to the recipient's perception. |
| `arm.spell.cooling_shade` | Cooling Shade | Policy | Reduce local light and temperature for a bounded duration. |
| `arm.spell.deep_darkness` | Deep Darkness | Primitive | Reduce room illumination through the native light effect. |
| `arm.spell.clinging_hex` | Clinging Hex | Primitive | Apply the configured native curse modifier. |
| `arm.spell.haunting_shape` | Haunting Shape | Gap | Summon a temporary unsettling shadow creature. Source summons an actor; phantom room text is only an unapproved alternative, not complete coverage. |
| `arm.spell.shadow_blade` | Shadow Blade | Gap | Timed low-tier shadow sword; mon component-consuming permanent staff output; source-informed prototypes and explicit per-tier ownership/cleanup |
| `arm.spell.shadowplay` | Shadowplay | Policy | Display a non-interactive shadow scene to the selected audience. |
| `arm.spell.shadow_mantle` | Shadow Mantle | Policy | Apply a finite shadow-themed protective layer. |
| `arm.spell.draw_into_flesh` | Draw into Flesh | Policy | Remove an eligible spell-owned ethereal overlay. |
| `arm.spell.champion_blade` | Champion Blade | Gap | Create a temporary enchanted sword with an explicitly selected combat profile. No summoned-hero linkage without a separate lifecycle adapter. |
| `arm.spell.lightning_lance` | Lightning Lance | Policy | Strike a target with electrical damage. |
| `arm.spell.second_breath` | Second Breath | Primitive | Restore a bounded amount of stamina. |
| `arm.spell.wakeful_mind` | Wakeful Mind | Primitive | Apply native magical wakefulness and its sleep interaction. |
| `arm.spell.leadfoot` | Leadfoot | Gap | Slow locomotion while applying an agility penalty. A bounded movement-delay modifier is missing; trait penalties alone are not the whole behavior. |
| `arm.spell.steal_breath` | Steal Breath | Primitive | Remove a bounded amount of the target's stamina. |
| `arm.spell.still_limbs` | Still Limbs | Primitive | Apply native magical paralysis after resistance. |
| `arm.spell.leaping_lightning` | Leaping Lightning | Gap | Source-informed bounded repeated random target selection; caster possible at quarter damage and Energy Shield protection; do not silently impose distinct targets or diminishing falloff |
| `arm.spell.storm_locus` | Storm Locus | Gap | Maintain a local electrical hazard that strikes eligible occupants on bounded ticks. Needs a spell-owned hazard lifecycle, not an environmental resource heartbeat. |
| `arm.spell.charged_aegis` | Charged Aegis | Policy | Provide finite protection against configured electrical harm. |
| `arm.spell.luminous_trail` | Luminous Trail | Policy | Make native tracks left by the target easier to observe. |
| `arm.spell.quickened_healing` | Quickened Healing | Primitive | Increase native natural healing rates for a duration. |
| `arm.spell.lightning_pace` | Lightning Pace | Gap | Improve agility and actual movement speed for a duration. Requires the same bounded speed modifier as Fleet Step; no implicit cancellation of all movement effects. |
| `arm.spell.spark_lash` | Spark Lash | Gap | Timed low-tier lightning whip; mon component-consuming permanent staff output; use executable source over stale object comment |
| `arm.spell.sky_lantern` | Sky Lantern | Policy | Show a luminous sky display where location policy permits. |
| `arm.spell.storm_spear` | Storm Spear | Gap | Create a temporary lightning-themed spear. Placement and expiry need explicit handling; questionable source rune references are not treated as spear semantics. |
| `arm.spell.raise_servitor` | Raise Servitor | Policy | Animate an eligible corpse with selected AI and restore its corpse lifecycle on expiry. |
| `arm.spell.call_outsider` | Call Outsider | Gap | Summon a temporary otherworldly creature under explicit control policy. Historical Gate summons a creature; a portal effect is not equivalent coverage. |
| `arm.spell.arcane_mark` | Arcane Mark | Primitive | Attach bounded spell-owned magical metadata to an item. |
| `arm.spell.feign_death` | Feign Death | Gap | Simulate death convincingly to configured observations while preserving the living character. Needs coordinated appearance, physiological observation and action restrictions; a corpse description alone is insufficient. |
| `arm.spell.silence_the_mind` | Silence the Mind | Gap | Suppress selected psionic actions and suspend relevant active links. Interception exists; active-link teardown and all relevant psionic entry points need explicit coverage. |
| `arm.spell.devouring_touch` | Devouring Touch | Gap | Drain delivered vitality into bounded healing for the caster. Requires measured transfer and configured target restrictions; no dragon cosmology imported. |
| `arm.spell.compelling_regard` | Compelling Regard | Gap | Persistent resisted PC/NPC charm with per-spell PC toggle and final-command IC whitelist; block privileged/OOC/account/alias bypasses |
| `arm.spell.unmaking_ward` | Unmaking Ward | Policy | Intercept explicitly tagged elemental spell effects against the wearer. |
| `arm.spell.severing_refuge` | Severing Refuge | Larger | Usable refuge with defined builder-configurable elemental ward scope and safe temporary topology |
| `arm.spell.apex_bane` | Apex Bane | Larger | Resisted bounded attack against builder-tagged exceptional magical beings, with explicit eligible/ineligible tests |
| `arm.spell.empty_aura` | Empty Aura | Primitive | Reduce a specifically configured target reserve without crediting the caster. |
| `arm.spell.linked_threshold` | Linked Threshold | Policy | Create a bounded linked travel connection between authorised anchors. |
| `arm.spell.bar_the_elements` | Bar the Elements | Policy | Refuse explicitly tagged elemental interactions in the cell. |
| `arm.spell.turn_the_elements` | Turn the Elements | Gap | Redirect an eligible elemental attack to its source once under configured rules. A rejecting ward is not reflection; needs bounded redirect and recursion prevention. |
| `arm.spell.folded_pocket` | Folded Pocket | Larger | Finite items-only pocket storage; no nesting/living occupants; persist access/ownership and conserve contents on collapse |
| `arm.spell.veil_of_elements` | Veil of Elements | Policy | Obscure a cell with a configured non-physical elemental mist. |
| `arm.spell.cross_the_veil` | Cross the Veil | Policy | Apply an approved native planar-presence transition. |
| `arm.spell.borrow_the_dead` | Borrow the Dead | Policy | Control an eligible corpse through the native shared-identity possession mechanism. |
| `arm.spell.open_threshold` | Open Threshold | Policy | Create a transient permitted exit between resolved cells. |
| `arm.spell.read_enchantment` | Read Enchantment | Policy | Let the recipient see authored information about permitted targets through LOOK. |
| `arm.spell.blade_barrier` | Blade Barrier | Gap | Block passage with a timed barrier that cuts crossing attempts. Traversal damage needs an explicit adapter; bare blocking is an unapproved reduction. |
| `arm.spell.mind_scour` | Mind Scour | Policy | Apply configured psychic injury through normal spell resistance. |
| `arm.spell.phantasm` | Phantasm | Policy | Display an audience-filtered, non-interactive apparition in the cell. |
| `arm.spell.unbodied_journey` | Unbodied Journey | Policy | Create a focusable native projection while the anchor body remains. |
| `arm.spell.erode_object` | Erode Object | Policy | Inflict finite material-gated damage on an item. |
| `arm.spell.leeching_edge` | Leeching Edge | Gap | Enchant a weapon to heal its wielder by a bounded share of actual damage dealt. Existing flat enchantment bonuses are not a per-hit measured lifesteal hook. |
| `arm.spell.voice_of_remains` | Voice of Remains | Policy | Raise a temporary corpse speech proxy using native corpse restoration rules. |
| `arm.spell.echo_servant` | Echo Servant | Policy | Relay authorised speech through a selected proxy using native language-aware output. |
| `arm.spell.drowning_grip` | Drowning Grip | Gap | Interfere with breathing through a bounded, resistible temporary effect. Source entry has no bound function; hypoxia rather than generic damage needs a specific adapter and acceptance. |
| `arm.spell.sow_sickness` | Sow Sickness | Policy | Apply one configured native disease to a susceptible target. |
| `arm.spell.caustic_spray` | Caustic Spray | Policy | Strike explicitly selected local targets with acid-type damage. |
| `arm.spell.gather_puddle` | Gather Puddle | Policy | Create or top up a native clean-water puddle in the current cell. |
| `arm.spell.wardcraft` | Wardcraft | Policy addition | Apply a basic tag-scoped personal ward; explicit FutureMUD addition, optional builder stock, never selected historical Sorcerer membership. |
| `arm.spell.renew_earth` | Renew Earth | Policy addition | Apply bounded scar repair to the eligible cell; explicit FutureMUD addition, optional builder stock, never selected historical Sorcerer membership. |

## Appendix C Historical implementation evidence

Verified 2 October 2026. Read-only source review; this appendix is evidence for implementation planning, not an implementation or a claim that one historical release contained every cited feature.

### Source and citation conventions

- **C**: user-supplied **codedump.c**, 8,354,060 bytes, 242,965 rendered lines. All C line ranges below refer to the supplied rendering of that file and were inspected directly. Function and constant names are included so an implementer can relocate evidence if their local concatenation has different line offsets.
- **F**: [jcarter's full-guild Sorcerer tree](http://armageddonmud.freetzi.com/sorcerer.html); [homepage dated addition](http://armageddonmud.freetzi.com/); [6 January 2016 archive](http://web.archive.org/web/20160106231707/http://armageddonmud.freetzi.com:80/sorcerer.html). Prior verified research established a 3 September 2013 publication, 94 skill rows including 82 spells, and an entry-for-entry match of all 94 rows against the 2016 capture. The tree is a fan publication, not a canonical official export.
- F establishes a concrete historical membership/progression baseline. C establishes implementation behavior in the supplied code. Neither proves that all features belong to the identical release. Missing fan-tree entries are not proof of historical exclusion; the explicitly selected stock Sorcerer roster nevertheless remains exactly 82 spells.
- **Stock requirement supplied for this brief**: source-match where evidenced, retain per-spell configuration, and do not ship stock spell scrolls. The scroll exclusion is a deliberate adaptation despite scroll support in C.

### 1. Seven power words, indices, parser, and mana

**Direct facts**

C **24200–24208**, `Power[]`, is exactly:
1. wek
2. yuqa
3. kral
4. een
5. pav
6. sul
7. mon

C **166948–166956** defines `POWER_WEK=0`, `POWER_YUQA=1`, `POWER_KRAL=2`, `POWER_EEN=3`, `POWER_PAV=4`, `POWER_SUL=5`, `POWER_MON=6`, `LAST_POWER=6`. These are zero-based symbolic indices.

C **173358–173365**, `get_power`, compares each word case-insensitively and returns **i+1**, or -1. Thus parsed spell power is **1–7**. `find_spell` (**173408–173483**) gets power/reach/element/sphere/mood from five words, identifies a magick-class skill matching element/sphere/mood, and returns the parsed power unchanged. It searches the five positions for each category; do not infer a mandatory position order merely from one conventional incantation.

C **136680–136706**, `old_search_block`, increments its match index before returning; the matching word is likewise 1-based. C **133063–133069**, `cmd_use`, parses quoted device power through this helper and rejects power greater than the device's stored level. C **126773–126779** stores device power using this 1-based helper.

C **173601–173618**, `mana_amount_level(vdiff,power,spl)`, has this exact integer-arithmetic behavior:
- First set d = vdiff + 1
- If power = d: return 50, with no min-mana clamp on this branch
- If power > d: return max(100 - floor(50 / (power - d + 1)), skill[spl].min_mana)
- If power < d: return max(floor(50 / (d - power + 1)), skill[spl].min_mana)

C **173622–173631**, `mana_amount`: immortals cost 0; negative `ch->skills[spl]->rel_lev` costs the caster's maximum mana; otherwise pass `rel_lev` as vdiff.

C **57764–57784**, `cat_skills_prev_to_string`, prints the distinctive tree format `st/max/min_mana/go on`: min_mana is computed with `mana_amount_level(POWER_MON, POWER_WEK+1, spell)`, and go-on is guild cap minus 10. This is provenance support for the tree's configuration-shaped format, not proof of its origin or proof that printed min_mana is every cast's actual cost.

**Implementation consequence / proposed adaptation**

Represent power identity separately from a zero-based proficiency index and a 1–7 effect magnitude. Write explicit conversion tests; copying POWER_* into formulas mechanically risks off-by-one behavior. Preserve the exact mana formula as source evidence; if substituting FutureMUD resources/checks, document that numeric translation separately. In particular, the staff caller below supplies its stored 1-based device level to a function that increments vdiff; preserve or deliberately adapt that behavior, do not silently “correct” it.

### 2. Area effects and caster/allied exclusions

**Direct facts**

- **Earthquake**: C **86103–86172**, `spell_earthquake`. Local-room other targets take full damage; caster takes one-third damage. Both branches skip immortals, flying, ethereal, stone-skin targets and shadow race. There is no ally/follower/group exclusion in this loop. C **86046–86092**, `earthquake_helper`, gives nearby-room messages and falls, not the same damage loop; exclusions include agility success and the listed protections. C **86133–86135** bounds the search with level and no-magick flags.
- **Chain lightning**: C **83253–83371**, `spell_chain_lightning`. Repeatedly selects a random non-immortal in the room; caster can be selected and takes quarter damage, other selected targets take full damage. Energy Shield blocks that damage. C **236312–236340**, `get_random_non_immortal_in_room`, filters immortality only, not allies/caster. The implemented loop, rather than the storm-cloud duration comment, controls behavior.
- **Rain of fire**: C **93062–93176**, `spell_rain_of_fire`. Local other targets receive full damage; caster receives one-third, with local ethereal/fire-armor/shadow-race exclusions. It can also affect the room below, whose separate loop checks immortality only. There is no general ally exclusion.
- **Fireball via STAFF or ROOM**: C **219104–219114**, `cast_fireball`, excludes caster and immortals and requires `ethereal_ch_compare`; no ally filter. Potion dispatch instead targets the user; wand dispatch takes its selected target (**219075–219122**).
- **Lightning bolt via STAFF or ROOM**: C **220330–220342**, `cast_lightning_bolt`, excludes caster and immortals and requires ethereal compatibility. Direct spell use in the water plane instead fans out to others with a distinct loop (**220299–220312**).

**Implementation consequence**

Neither “all area spells spare their caster” nor “all area spells spare allies” is source-backed. Store explicit per-spell and per-delivery-mode selectors, caster scaling, protection filters, collateral-room rules and relationship exclusions. Relationship protection remains absent for the verified examples unless a separate approved adaptation adds it.

### 3. Wands/staves: existence, activation, charge and type rules

**Direct facts**

- C **25114–25115** defines ITEM_WAND and ITEM_STAFF. C **22083–22135** labels their slots max_mana, power, mana, spell.
- The complete `cmd_use` body, C **132875–133104**, requires the item to be equipped/held; staff/wand power word must have single quotes. Silence or PSI_BABBLE substitutes an invalid word. Wands must resolve a character or object target in room/inventory/equipment; staves dispatch without a selected target.
- The wand/staff branch does **not** check caster guild, possession of the embedded spell skill, a use-magic-item skill, or a learned percentage. The language-skill gate in the same function is in the **scroll** branch only. C **137798** registers use at sitting position for mortals. This is evidence for this generic path, not proof that every scripted artifact lacks additional restrictions.
- C **133059–133088** validates embedded skill ID/function pointer, exact valid power word, requested power within stored level, and sufficient device mana. It deducts device mana before dispatch.
- Staff cost is `10 * mana_amount_level(stored_level, requested_power, embedded_spell)`.
- Wand cost overrides this: `600 + 200*(requested_power-1)`. Non-[ARTIFACT] wands have maximum capacity clamped to `1100 + 100*stored_level` and lose 100 maximum capacity per use; [ARTIFACT] exempts these two degradation steps. Wand applies WAIT_STATE 1. C **133072–133102**.
- C **132759–132795**, `cmd_donate`, transfers a positive requested mana amount from character to held wand/staff, rejects insufficient character mana or exceeding device capacity, then adds/subtracts the amount. No guild/known-spell check occurs in this function.
- C **216723–216739**, `get_spell_magick_type`, classifies potion/scroll/wand/staff casting as **MAGICK_OBJECT**, not the activator's elemental/sorcerous type.
- Delivery eligibility is **per spell**. `cast_guardian`, C **219677–219697**, summons for STAFF/ROOM/direct spell/scroll, but WAND/POTION only show a ripple message. `cast_gate`, C **219543–219554**, performs the summon for direct spell/scroll; staff/wand/room/potion only cause vertigo. These devices are not universally effective spell containers.
- C **172323–172359**, `imbue_item`, requires the matching wand/staff reach and object type; wand maximum mana becomes `1100+100*level`, staff maximum becomes `500*level`; stores level and spell. [ARTIFACT] wands reject alteration.

**Implementation consequence**

The inspected historical generic activation path does not impose a caster-type or spell-knowledge restriction. The approved FutureMUD design deliberately adds configurable per-item eligibility and makes STOCK wand/staff templates caster-only; this explicit user choice takes precedence over the historical default. Device construction, recharge, activation, actual spell effect and scripted-artifact restrictions are separate gates. Keep the source's per-spell delivery matrix. The requested stock package excludes spell scrolls even though C defines and executes them.

### 4. Conjured item persistence and timed removal

**Direct facts**

- `spell_create_food`, C **83526–83621**: creates **level** food objects (prototype selection depends on setting/race/guild/tribe), places them in room, marks NOSAVE, and schedules EVNT_REMOVE_OBJECT after **level * 30 * 60 event units**.
- `spell_create_water`, C **83644–83700**: adds liquid to an existing drink container (normally 5*level, water plane 10*level, capacity-limited), with slime rules. This function schedules **no magical expiry** of the created liquid or container. “All conjured food and water expire” is unsupported.
- `spell_fire_jambiya`, C **86963–87057**: numeric levels 1–6 create temporary NOSAVE weapons, delay `(level+1)*3000/2`, or half that in shadow plane. Numeric **7 / mon** consumes a conjuration component, creates random object **1386–1393**, stamps creator, gives it to caster, and returns before NOSAVE/expiry. Comment explicitly calls the high-tier output a permanent staff.
- `spell_sand_jambiya`, C **93876–93992**: temporary 1–6 weapons use the same base delay; mon consumes creation component and creates **1378–1385**, returning before NOSAVE/expiry.
- `spell_lightning_whip`, C **90300–90383**: temporary 1–6 weapons use EVNT_LIGHTNING_REMOVE and the same base delay; mon consumes creation component and creates **1476–1483**, returning before NOSAVE/expiry. Its preceding comment says object 1028 and a single low-tier whip, but executable code uses the ranges above; do not treat the stale comment as an override.
- `spell_shadow_sword`, C **94761–94852**: temporary 1–6 weapons use the same base delay, doubled on shadow plane; mon consumes creation component and creates **1468–1475**, returning before NOSAVE/expiry.
- `spell_lightning_spear`, C **90158–90219**: requires a creation component at level-2, produces object 488, marks NOSAVE, stores [LIGHTNINGSPEAR_POWERLEVEL], equips a primary hand subject to occupancy, and schedules removal `(24+random(1,6))*90*level`. Uses EVNT_SANDJAMBIYA_REMOVE despite the lightning flavor.
- C **44543–44659**: generic/fire-jambiya/sand-jambiya removal handlers ultimately `extract_obj`; they handle room, carried and equipped locations. Picking up or equipping is not an exemption.
- C **45703–45760** and **45997–46037** use event-heap time; do not casually label these raw event-delay values as seconds or game hours. Affect durations are another mechanism; RT_ZAL_HOUR is 600 in C **24979**.

**Implementation consequence**

Configure persistence by spell **and tier/output**, not a global conjured-item flag. Model magical expiry separately from ordinary food/liquid decay and save persistence. Actual permanent staff prototype stats/types, food prototypes, and spear-attached script behavior require the missing object/script data or clearly labeled new content. A permanent branch means no expiry scheduled by this function; it is not a claim that its object is indestructible.

### 5. Summon counts, control, expiry, death and loot

**Direct facts**

- `spell_gate`, C **87603–87804**: summons one creature; source selection varies by guild or gather/land relationship. Applies GATE summoned affect for **level+random(2,4)**. Counts existing followers affected by GATE, computes **3+40*count**, rolls 1–100, and on **roll < threshold** creates hostility/aggression; otherwise follower+charm for the same duration. This is escalating control risk, **not a hard cap**. Do not simplify the strict-less-than comparison into an exact percentage claim.
- `spell_guardian`, C **88146–88360**: **level creatures per cast**, component above wek; duration **5+random(0,level)**; adds liking and guard relationship. No total-active cap is checked in this function.
- `spell_mount`, C **90862–91020**: one mount, component except stone elementalists, duration **level*9**, summoned flag. No total-active cap is checked in this function.
- `raise_corpse_helper`, C **81934–82078**: transfers corpse contents to the raised creature and attempts wear-all; charm duration **level+6** (not applied in silt/shallows), summon duration **level+10**, adds follower. `spell_animate_dead`, C **82091–82118**, consumes the corpse. No total-active cap is checked in these functions.
- `affect_update`, C **172710–172715**, sends expired MOUNT, SANDSTATUE, GATE, ANIMATE_DEAD, GUARDIAN and SEND_SHADOW to `remove_summoned`. That helper, C **172447–172513**, emits type-specific disappearance, removes all affects and extracts the character.
- `raw_kill`, C **10387–10433**: summoned or possessed-corpse characters bypass the ordinary corpse-creation path. Within the generic dissipating-body branch, undead first drop carried items except OFL_DISINTIGRATE, drop equipment and emit money; carried/equipped items gain bloody/sewer state. This exception does not apply to the earlier mount/sandstatue branches.
- `extract_char`, C **51848–51851**, calls `extract_char_mnt_and_objs` then follower cleanup. That helper, C **51655–51664**, destroys retained equipment/inventory. Thus ordinary summon expiry destroys items still retained by the summon; undead **death** has an explicit pre-extraction loot-drop exception, while expiry does not invoke that raw_kill exception.
- C **15271–15284**, `die_follower`, breaks master/follower links; C **113735–113775**, `stop_follower`, removes charm/control and the link. These functions do not impose blanket “all summons disappear when caster dies.” Mounted-character extraction has additional special handling (**51667–51695**); do not generalize it to every summon.

**Implementation consequence**

Separate count per cast, total-active limits, control risk, relationship type, duration, corpse creation, original/introduced equipment, and exit reason. Do not invent “one summon,” “all summons disappear on caster death,” or “summons never leave loot” as universal historical defaults. The approved FutureMUD design preserves ordinary foreign carried contents on expiry; this is a required safety adaptation to the destructive historical extraction path. Distinguish introduced world assets from generated temporary loot. If balancing adds a total-active cap, label it as an adaptation rather than recovered behaviour.

### 6. Implementation-facing unresolved evidence, not brief-wide blockers

1. Object/mobile prototypes and attached scripts are not supplied by these function bodies. Preserve stable source IDs as provenance; design/verify FutureMUD replacement prototypes separately.
2. Map event-heap timing and affect timing to the chosen FutureMUD time model, with one conversion contract and tests. Keep raw source expressions in catalogue evidence.
3. Reconcile only actual source contradictions: executable behavior vs stale comments (lightning whip; chain lightning), zero/one-based power uses, and per-mode target filters. Do not “normalize” heterogeneous behavior silently.
4. Validate each spell's full delivery-mode wrapper and effect function before assigning area/device defaults; the verified examples demonstrate why a single blanket default is unsafe.
5. Source-era roster additions remain evidence unions/version questions. The full-guild fan baseline is 82 spells; the separate source-comment evidence for Armor and Planeshift is additive evidence, not proof that 84 coexisted in F.
6. Keep requested no-stock-scrolls as a named adaptation. Existing source scroll code does not override that choice.

### Minimum source-match tests

- All seven words round-trip: index 0–6 ↔ parsed/effect level 1–7; mana equality/above/below branches, min clamp exceptions and negative relative level
- Historical device-path characterization and FutureMUD per-item eligibility tests, including caster-only stock and configurable non-caster activation; silenced/babbling use, invalid/out-of-range power, insufficient charge, wand degradation/artifact exemption, and spell-specific STAFF/WAND differences
- Earthquake/chain-lightning/rain-of-fire caster self-harm versus STAFF fireball caster exclusion; allies remain eligible in the verified loops
- Food timer; water has no automatic conjuration-expiry; mon permanent outputs versus lower-tier expiry; expiry after transfer/equip
- Gate risk with 0/1/2/3 current gate followers; guardian count per cast; summon expiry versus death; historical undead death/expiry differences with required FutureMUD foreign-item preservation; caster/follower separation without unconditional despawn

