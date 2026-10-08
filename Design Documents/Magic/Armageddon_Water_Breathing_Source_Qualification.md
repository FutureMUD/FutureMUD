# Water Breathing source qualification and adapter boundary

**Outcome:** the exact source contract is recoverable, but the current adapter does not faithfully implement it through stock configuration alone. No stock row or runtime change is authored. This checkpoint adds only this note and a [source-bound receipt](Armageddon_Water_Breathing_Source_Qualification_Receipt.json), based on inspected revision `d936c929626936f49aa7a9bfd913efb778487ab9`. Emotional live hooks, reviewed evidence, installer-owned Pierce extraction and main-owned authority/lifecycle methods remain untouched.

## Identity and authoritative acquisition

The authoritative completion brief is available through supported Library reads: `libfile_a4606cd0097081918d6c9d9e220e6da0`, file `file_00000000273481fab2e542daf8801228`,131071 reported bytes,905 lines. Row32 at548 and the committed [source tree](Armageddon_Sorcerer_Source_Tree.json) agree:

| Field | Source-qualified value |
|---|---|
| Canonical key/name | `arm.spell.water_breathing` / Water Breathing |
| Historical identity | Breathe Water, skill443; the source number is not a native database ID |
| Native elemental association | Water; Abjuration; Beneficial |
| Selected Sorcerer roster | Yes, row32; exact elemental full-guild learnlist is not established by the coded association |
| Immediate prerequisite | Draw Wine (`arm.spell.draw_wine`) at raw proficiency80 |
| Full source path | Unravel Enchantment root -> Draw Water raw80 -> Draw Wine raw80 -> Water Breathing raw80 |
| Opening/raw cap/branches | 30 /90 /80 |
| Printed minimum mana | 20 |
| Historical target/position | Character in room; Standing; source wrapper permits self and defaults an omitted spell target to self |
| Historical component | None; the table also has component0 |
| Brief completion contract | Allow compatible breathing underwater while the spell lasts (brief664) |

The old [catalogue proposal](Armageddon_Repertoire.md) suggests Draw Water grade2/raw20, duration60*g seconds and energy base5. Those are superseded proposals, not the exact Sorcerer source prerequisite, historical timing or printed minimum. No catalogue/disposition table is rewritten here. Existing acquisition and energy-envelope APIs should use the canonical Draw Wine/raw80 edge and the approved source-efficiency mapping with minimum20; the table's base-power15 field is not a native trait/database ID or an additional resource formula inferred here.

## Recovered executable historical rules

Supported Library reads of `codedump.c` (`libfile_befb4ae78d1c8191aaa2640c49912d9d`, file `file_00000000438871faac40c5042679ce3a`,8354060 reported bytes,242965 lines) establish:

| Evidence window | Recovered rule |
|---|---|
| `175892-175894`, `166711` | Skill443 metadata: Water/Abjuration/Beneficial, character in room, Standing, minimum20, base power15, no component |
| `82927-82991` | Character/self binary grant. Requires non-null caster/victim; records criminal casting; no target save, race/undead exclusion, numeric attribute boost or consumed component in the effect function |
| `82956-82960` | Nilaz-plane rejection is commented out. It is not an active refusal. No active Silt, Fire, Water-plane or underwater-only casting restriction occurs in this function |
| `82962-82990` | Duration is `max(1, number(floor(g/2), 3*g))`; selected source grade becomes affect power; first and repeat casts have different visible mouth/nose/lung messages; stack ceiling36 |
| `81574-81592` | `stack_spell_affect` adds durations of existing effects of this source type, retains maximum source power, clamps the sum, removes those old affects, then attaches one replacement. It is not a same-caster-only or same-native-spell-ID refresh |
| `236270-236278` | `number` uses inclusive integer endpoints. Water grade1 draws0..3 and then clamps0 to1; unlike Fury grade1, this is not an inverted range and does require a draw |
| `24979` | A source hour is600 real seconds |
| `217138-217177` | Ordinary spells reject object targets and default missing character targets to self. Potion/scroll/wand/room/staff delivery has distinct wrapper rules; those carriers/area routes are excluded from this stock checkpoint |
| `172556-172584` | Water-plane drowning update exempts the spell, natural-water-breather description, immortals and Water elementals. The effect is a water-plane protection, not evidence for breathing every gas/liquid or repairing failed organs |
| `85626-85640` | Historical dispel subtracts grade source hours; if expiration is past, removes this spell. Water-plane breathing-loss messages are conditional on location |

The full-dump `SPELL_BREATHE_WATER` search returned all17 matches without pagination. This includes status/perception descriptions, effect application, dispel, water-plane update, two separate hostile-spell protection checks and delivery declarations. Specifically, `spell_firebreather` refuses an affected victim at87159, and `spell_parch` refuses one at91312 (also refusing when the caster is in Water Plane). Those are cross-entry rules, not general attack immunity: record them for the Firebreather/Parch owners rather than silently adding new hostile-spell implementations here. Source-specific guild acquisition and the categorical Water-plane model come from different evidence than native fluid identities; no original native water-liquid ID can be recovered from that sector constant.

Source grade1..7 maps to these bounds:

| Grade | Inclusive draw | New duration units after clamp | New real seconds |
|---:|---:|---:|---:|
| 1 | 0..3 | 1..3 (raw0 and1 both become1) | 600..1800 |
| 2 | 1..6 | 1..6 | 600..3600 |
| 3 | 1..9 | 1..9 | 600..5400 |
| 4 | 2..12 | 2..12 | 1200..7200 |
| 5 | 2..15 | 2..15 | 1200..9000 |
| 6 | 3..18 | 3..18 | 1800..10800 |
| 7 | 3..21 | 3..21 | 1800..12600 |

For a future native mapping, add the selected increment to normalized remaining source units, cap36 units/21600 seconds and retain the strongest source grade independently of native power. Using `ceil(native remaining seconds /600)` with exact native parent scheduling follows the already reviewed endpoint-normalization approach, rather than claiming byte-exact historical pulse/inclusive-endpoint behavior. Existing saved remaining-time restoration is an engine adaptation; historical offline timing parity is not asserted. Sample once into the existing prepared-selection token and reuse it through confirmation/payment; do not reroll a generic duration expression after payment.

## Proposed native mapping and verified gaps

The [template](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs) currently constructs `SpellWaterBreathingEffect` for an `ICharacter` and exposes no builder options. Its [native child](../../MudSharpCore/Effects/Concrete/SpellEffects/StandaloneSpellStatusEffects.cs) returns true from `AppliesToFluid` for **every non-null fluid**, including gases. It has no persisted authored fluid scope, selected source grade or recast policy. This is broader than the brief's compatible-underwater grant and its catalogue boundary excluding a survival promise in poisonous atmospheres.

Proposed bounded fluid mapping: require an explicitly selected native water `ILiquid` or an authored list of compatible water liquids; use the engine's `CountsAs` relationships in their declared direction and persist the IDs. Do not guess a fluid by English name, historical skill number or global material ID. Reject gases/null and unselected liquids as additional grants. Normal racial breathing continues independently. That narrowing needs an editable scoped child/template; adding ignored XML under the present no-options template would not implement it.

The [lung strategy](../../MudSharpCore/Health/Breathing/LungBreather.cs) reads applicable `IAdditionalBreathableFluidEffect` from the body's combined character/body effects. It resolves underwater fluid from the current terrain/layer, and otherwise atmosphere or a real breathing-gas supply. Heart/lung/trachea function, airway bleeding, breathing-stop effects and anaesthesia still control respiration. This spell must not heal those failures, refill oxygen/held-breath state or remove poison/exposure. The native [body combination](../../MudSharpCore/Body/Implementations/Body.cs) includes character-owned children, so a correctly scoped native child has an existing consumer.

Gill, blowhole and partless strategies currently query racial fluid compatibility without this additional-effect grant. A lung-only target contract would therefore be a deliberate engine adaptation, not universal source parity. Broader support would touch shared breathing consumers and needs allocation/review; it is not authorised by adapter registration alone. Native tests must prove character/body applicability, valid water, toxic gas, other liquid, absent atmosphere, failed airway/anaesthesia, supply precedence, expiry/dispel and strategy behavior before any survival claim.

The current template is not `IMagicSpellEffectOperation`, so configured paid dispatch cannot observe intended application/mastery through that adapter. A narrow concrete operation partial could use the existing interface, but attaching a fresh child is insufficient proof if breathing capability is unchanged. Reporting must distinguish a newly granted capability, observed deadline/strength extension, unchanged effective state and uncertainty. The shared base and other statuses should remain untouched.

The current [lifetime resolver](../../MudSharpCore/Magic/MagicSpell.Lifetime.cs) explicitly admits one `DetectInvisibleEffect` and one `SpellDetectInvisibleEffect` only. Ordinary exclusive Water Breathing removes/replaces same-ID parents; nonexclusive behavior creates overlapping parents. Neither supplies random remaining-time accumulation, cap36 or a shared source-type/strongest-grade cohort. Copying the detection XML policy onto this adapter would fail validation. A separately owned Water-specific policy/admission/resolution partial would need agreed caller allocation and a persisted grade/fluid cohort; do not generalise the cleared detection resolver or silently edit main-owned lifecycle paths.

Normal stock construction can reuse current spell/skill/resource builders, native character/self trigger, acquisition service, source energy envelope and practice. Keep native Water school under a Sorcerer route, opening30/raw cap90 and Draw Wine raw80. Map historical Standing explicitly rather than assuming common body gates reproduce it. A source-faithful stock has no new terrain rejection, no consumed component and no opposed save. Quiet/focus requirements stay explicit current casting configuration; source carriers and area wrappers remain separate. Parent persistence and scheduler APIs exist, but they do not prove this missing per-entry policy.

## Stopping boundary and later acceptance packet

The conditional stock-only implementation authorisation is not met. Concrete blockers are scoped fluid persistence/consumption, selected random accumulated lifetime/retained grade, and truthful operation reporting. Strategy coverage is an additional explicit adaptation/allocation decision. No new stock, adapter, broad infrastructure, test database, process or native harness is created in this checkpoint.

After scope selection, the dedicated entry packet should cover editable construction/save/reload; canonical acquisition and grade7 progression under raw cap90; paid grade1/7 with the actual source-efficiency cost; refusal before debit for object/unreachable/unsupported targets and malformed policy; no copied Silt/Fire/Nilaz prohibition; grade1 raw0 clamp and inclusive endpoints; sealed random reuse/callback drift; weak/strong recast and cap36 across managed casters/copies; scoped water versus toxic gas/other liquids; applicable native respiration without organ repair; no-change/application receipts and mastery; native parent persistence/reload, expiry and dispel. Any run must use its own disposable database and process/evidence paths. Existing7511-test Fury evidence remains historical proof for `da9af18a`, not acceptance of unimplemented Water Breathing stock.

This documentation-only checkpoint validates referenced paths, exact source-tree row, source/record hashes, diff and clean live-source boundaries. It does not rerun code tests or claim paid/native acceptance. The independent emotional review correction is separately recorded in [its integration note](Armageddon_Emotional_Cessation_Review_Note.md).
