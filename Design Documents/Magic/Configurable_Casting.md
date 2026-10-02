# Configurable casting (ARM-02)

This is the implementation guide for [ARM-02](ARM02_Configurable_Casting_Implementation_Brief.md).
The broader [ARM-01 contract](Armageddon_Casting_Design.md) includes later features; this slice
provides explicit acquisition, normal casting, controlled grades and prerequisite progression.
It does not install the candidate repertoire as stock content. See the
[verification handover](Configurable_Casting_Handover.md) for executed evidence and limits.

The completion plan's first progression stage adds opt-in route openings/raw caps,
cap-relative mastery gates and source-shaped energy. Its full remaining phases and
acceptance cases are tracked in [Armageddon_Completion_Progress.json](Armageddon_Completion_Progress.json).
The selected 82-spell/12-support roster is recorded in
[Armageddon_Sorcerer_Source_Tree.json](Armageddon_Sorcerer_Source_Tree.json); this data is not an installed stock package.

## Players

A spell can be acquired even when its reserve is empty or its capability is temporarily
unavailable. Knowledge and controlled grade belong to the character identity and survive
body changes. A current capability must explicitly admit the spell to cast it. A child
school relationship does not supply admission.

```text
earth spells
earth spell "Stone Skin"
earth cast "Stone Skin" grade 2 on self
earth cast "Stone Skin" grade 3 overreach on self via "Earth Adept"
earth practice "Stone Skin" grade 3 overreach via "Earth Adept"
earth quiet "Stone Skin" grade 2 on self via "Earth Adept"
earth formula kral fm-near fm-earth fm-protect fm-shape on self via "Earth Adept"
earth quiet formula kral stone on self via "Earth Adept"
earth area "Earthquake" grade 3 overreach on here via "Earth Adept"
earth area formula kral quake overreach on here via "Earth Adept"
earth quiet area "Earthquake" grade 2 on here via "Earth Adept"
say kral fm-near fm-earth fm-protect fm-shape on self via "Earth Adept"
whisper kral stone on self via "Earth Adept"
say area kral quake overreach on here via "Earth Adept"
```

Replace `earth` with the admitting capability's school verb. Spell and capability names
containing spaces require quotes; IDs also work. `via` is optional only when one current
capability admits that spell. A richer reserve never selects a route automatically.
The native spell school still controls its ordinary wards and resistance.

The normal casting route requires the focused, conscious, speaking body and a functioning
free hand. Native target permissions, planar reach, component plans and lockouts still
apply. A refused preflight pays nothing and performs no check. Once payment begins, a
failed check or targets rejecting the spell retains payment and lockouts.

With controlled grade 2 and raw proficiency 42, grades 1 and 2 are permitted. Grade 3
requires the explicit `overreach` word; grade 4 or 7 refuses. For Stone Skin's authored
`5*grade` cost, grade 3 costs 22.5 units. Costs are not rounded before payment.

The supplied seven-grade profile maps grades 1–7 to native `SpellPower` values
ExtremelyWeak, VeryWeak, Weak, Standard, Strong, VeryStrong and ExtremelyStrong. The raw
skill thresholds are 0, 20, 40, 55, 70, 85 and 95; difficulty additions are 0, 0, 1, 1, 2,
2 and 3. Overreach multiplies designated energy by 1.5 and adds one difficulty step.
Existing fixed secondary costs stay unchanged and are combined if they address the same
holder and resource as the rebound cost.

An admission may instead declare cap-relative gates: those same seven threshold numbers
are percentages of that route's raw cap. Grade 7 therefore needs 85.5 on cap 90, or 57
on Mend Flesh's cap 60. A raw opening of 60 still starts at controlled grade 1. The
actual native casting check uses raw skill and its ordinary bonuses, without normalization.

An optional source efficiency profile replaces the designated source-resource expression:
equal requested/controlled grade costs 50; lower requested grades cost integer
`50/(controlled-requested+1)`, clamped to the authored minimum; higher grades cost
`100-50/(requested-controlled+1)`, similarly clamped. Its positive energy scale applies
once, followed by the explicit overreach multiplier. Controlled grades 1/2/7 requesting
grade 1 with minimum 7 and scale 1 cost 50/25/7 before other policy modifiers. Legacy
profiles retain their authored expressions. Fixed secondary costs still aggregate normally.

One paid cast may attempt native skill improvement per identity/trait per 60 seconds.
One next-grade attempt consumes a mastery opportunity per identity/spell per 600 seconds.
Only a successful cast with an applied intended operation can sample the profile's 25%
mastery chance. Empty groups, all-target rejection, no-op blindness removal, wholly
absorbed damage, unknown effect reports and caster effects alone do not qualify. A failed
native check can still improve its skill under the native improvement model.

An explicitly authored `practice` mode trains without targets or spell effects. It uses
the same admission, raw-cap gates, source efficiency, combined payment and shared skill/mastery
opportunities as manifestation. It pays the complete configured energy and its separate
material plan at the start, then requires retained focus and the configured physical
conditions for the whole action. It performs one target-free casting check at completion;
a successful check may sample mastery without requiring a manifested effect. Failed
checks can still improve the native skill under its own improvement model.

The provisional fixture takes 30 seconds and requires speech and a free hand; movement
is forbidden. Native stop, focus change, death, quit, body/state changes, loss of the
capability or required physical inputs interrupt it without progress or refund. Standard
invalidation, native inventory/wound/position signals and native effect changes recheck the
current inputs immediately. Added or removed silence, body-part/limb restrictions and forced
paralysis notify active practice after the effect list changes. Any required eligibility
loss latches interruption, even if the effect is removed or expires before the next heartbeat
or is changed re-entrantly during completion. Speech-free and hand-free profiles continue
when their other requirements remain satisfied. Forced paralysis is checked before a later
native health update changes character state. A five-second validity heartbeat and final
live rechecks remain fallbacks. Shared 60/600-second deadlines are consumed at commitment and survive
interruption. Pending operations persist their deadline and input/payment receipt, while
their timer is transient. Restart quarantines uncertain paid work for staff reconciliation;
it never resumes, refunds or rolls it automatically. Staff cannot reconcile live work.

Formula, genuine speech and quiet casting require an authored incantation profile. The
examples above use explicitly labelled FutureMUD category aliases and the spell alias
`stone`; they are not a recovered historical vocabulary. POWER is fixed: grades 1 through 7
are `wek`, `yuqa`, `kral`, `een`, `pav`, `sul`, `mon`, mapped explicitly to the seven native
SpellPower values above. A full formula has one POWER plus the spell's REACH, ELEMENT,
SPHERE and MOOD words in any order. A concise formula has POWER plus one authored spell
alias. Both require `[overreach] on <complete native target> [via <capability>]` and the
actor must know and select the profile's native spoken language. Unknown, duplicate,
ambiguous or incomplete words refuse. Route selection never uses available energy.

Named and formula commands speak one phrase through the native body, language and hearing
pipeline before paying. Actual player speech uses its original utterance without a second
echo. Only the original player's complete command can supply executable speech authority;
hearing, emotes, scripted speech, output callbacks and relays do not. A listener's own
explicit repetition is a separate paid invocation. Native quote syntax is retained for
formula and target resolution while the speech output keeps its ordinary presentation.
Operation origins are consumed once, including after restart and across practice/manifest
entry points. Pure quotes do not emit words, check, pay or add effects.

Quiet uses native `Whisper` and `AudioVolume.Quiet`. The provisional fixture multiplies
designated energy by 2 and adds one difficulty step, each once; fixed secondary costs are
unchanged. Native speech anatomy, gagging, mute merits, silence and volume requirements
still apply, as do the free hand and existing permissions. Visible casting emotes and
effects retain their normal output. Builders can author native method modifiers, but
unmapped methods and quiet/practice combinations refuse. Area and quiet-area require
their own explicitly authored native method. Existing legacy casting and independent spell-backed
powers/Vancian routes retain their own syntax and payment rules.

Area is an explicit per-spell character delivery variant. It requires exactly `on here`
and leaves ordinary selected-character, cell and exit effects on their existing routes.
Each policy declares caster/allies/others, scope, layer, grounding, staff and planar gates,
an optional native boolean target/caster Prog, physical-body or canonical-identity deduplication,
selection and application bounds, and separate caster/other damage multipliers. Physical-body
deduplication preserves separate bodies sharing one character identity. The native trigger's
self/filter permissions, live planar reach, target wards and resistance remain authoritative.
Area group reflection uses the existing group fail disposition, with reflection depth zero.

A paid invocation captures one bounded candidate list and one selected application order;
quotes consume no selection randomness. Raw inputs are bounded at 512, authored eligible
targets and applications at 1..256. Overflow or an empty pre-payment list refuses without
payment. Removed, moved or otherwise ineligible targets are skipped by live checks before
each application and child effect; new arrivals are never appended. Random selection with
replacement permits repeated hits on the same body. Payment, casting check, skill opportunity
and mastery are committed once for the whole invocation, including distinct sibling bodies.
All-target rejection retains payment and lockout and grants no mastery. Selected identity,
instance, body, multiplier and ordering persist in the immutable operation journal; restart
does not select again or replay damage. Partially attached persistent children retain their
scheduled parent lifetime on eligibility loss or exception; uncertain paid failures quarantine.

The three source examples are partial policies: Earthquake includes allies/others at full
damage and the caster at one-third; Chain Lightning permits repeated random hits with the
caster at one-quarter and other bodies at full damage; room Fireball excludes the caster
and includes allies/others. Native layer, grounding, staff/plane mappings and the Chain
fixture's three-hit bound are provisional. Specific historical race/protection predicates,
Energy Shield/Stone Skin bindings, nearby Earthquake falls and final stock damage/counts
still require source-informed authoring and native qualification. Attenuation applies only
to the matching invocation's native damage/pain/stun, leaving catalogue templates unchanged.

## Builders

Create a normal spell and a `skilllevel` capability using the existing builders. Edit the
spell with `magic spell edit <spell>` and issue the following through `magic spell set`:

```text
grades fixture
grades show
grades version <positive integer>
grades grade <number> <SpellPower> <raw skill threshold> <difficulty steps>
grades mastery <chance 0..1> <seconds>
grades skill <seconds> <opening skill>
grades overreach <cost multiplier> <difficulty steps>
grades efficiency source <minimum 0..50> <positive energy scale>
grades efficiency off
grades practice fixture
grades practice enabled <true|false>
grades practice duration <seconds>
grades practice difficulty <native difficulty>
grades practice energy <positive designated-energy multiplier>
grades practice max <1..7|none>
grades practice speech <true|false>
grades practice hand <true|false>
grades practice movement <true|false>
grades practice plan add <native inventory action>
grades practice plan remove <one-based index>
grades incantation fixture <native language> <reach> <element> <sphere> <mood> <alias>
grades incantation off
grades incantation language <native language>
grades incantation word <reach|element|sphere|mood> <single token>
grades incantation alias add|remove <single token>
grades incantation method <Say|Whisper|Talk|LoudSay|Yell|Shout|Sing> <positive energy multiplier> <difficulty steps -10..10>
grades incantation method <native method> off
grades incantation provenance <source label>
grades area fixture <earthquake|chainlightning|roomfireball>
grades area off
grades area include <caster|allies|others> <true|false>
grades area damage <caster|others> <non-negative multiplier>
grades area scope <RoomCharacters|ImmediateCharacters>
grades area selection <Ordered|RandomWithReplacement|RandomDistinct>
grades area identity <PhysicalBody|CanonicalCharacter>
grades area plane <MagicReach|PhysicalAndMagicReach>
grades area targets <1..256>
grades area applications <1..256>
grades area <layer|grounded|staff> <true|false>
grades area filter <native boolean target/caster Prog|none>
grades area method <Say|Whisper|Talk|LoudSay|Yell|Shout|Sing> <positive total energy multiplier> <difficulty steps -10..10>
grades area method <native method> off
grades area provenance <source label>
grades scalar add <target|caster> <zero-based effect index> boost Bonus <expression>
grades scalar remove <target|caster> <zero-based effect index>
```

For Stone Skin with spell armour first and agility boost second, use
`grades scalar add target 1 boost Bonus -grade`. The index is zero-based even though the
ordinary effect editor uses one-based effect numbers. Reordering/removing effects must
leave the binding's expected `boost` token, index, `Bonus` field and trait valid; otherwise
readiness fails. Only this typed scalar adapter is supported. Omitting a binding retains
the authored fixed bonus. A detached invocation receives -1 at grade 1 and -3 at grade 3;
the catalogue scalar and saved template remain unchanged.

The incantation fixture enables Say (energy x1, difficulty +0) and Whisper (x2, +1).
Its four category words are labelled `FutureMUD aliases` until the builder records a
source-backed provenance label. Words and aliases must be distinct single tokens and
cannot collide with POWER or command delimiters. POWER cannot be changed by this builder.
Language, vocabulary, methods and finite positive modifiers validate atomically; malformed
XML retains its unreadable definition for repair and refuses readiness. The native method
fixes its actual AudioVolume: Say/Decent, Whisper/Quiet, Talk/Quiet, LoudSay/Loud,
Yell/VeryLoud, Shout/ExtremelyLoud, Sing/Loud. Difficulty adjustments apply before the
native bounds. These coefficients are engineering tuning, not installed stock balance.

Area XML is optional and validates atomically with the grade profile. The examples enable
Say with total designated-energy x1 and difficulty +0. A native character trigger is required;
including the caster also requires its self permission. `staff true` excludes administrators.
Whole-room scope refuses spatial RouteCells; `ImmediateCharacters` must be authored explicitly
to use their native vicinity query. Area method values are complete modifiers, applied once:
quiet-area uses its authored Whisper entry instead of multiplying ordinary quiet and area
tables together. Add `grades area method Whisper 2 1` and an incantation language/vocabulary
to enable the provisional quiet-area example. For school formula syntax, omit the extra
`area` word after `area formula`; genuine spoken formulas start with `area`.

Complete-target checking is enabled only for authored incantations. Self, room and party
triggers require the exact selectors `self`, `here` and `party`; exit and character/exit
triggers must consume all target arguments. Native character, item, corpse, local item,
vicinity and character-vicinity parsers use their complete native target text. The
character-Prog-room trigger also uses complete character text. Other Prog target adapters
need an explicit complete-target mapping and fail readiness until it exists. Existing
legacy/Vancian target parsing is unchanged.

Grade profiles are shared by every route admitting the spell. A capability can narrow
the grade range, but cannot replace grade meanings. Existing acquired profile versions
must match the spell; changing the version deliberately disables old progress pending
an explicit administrative migration. Do not bump the version for cosmetic changes.

Edit a capability with `magic capability edit <capability>`. Use these arguments after
`magic capability set`:

```text
casting trait <character-owned native skill>
casting resources <designated source resource> <reserve resource> passive|gather
casting entry add <spell>
casting entry remove <spell>
casting entry trait <spell> <skill|default>
casting entry starting <spell> on|off
casting entry grades <spell> <minimum> <maximum>
casting entry skill <spell> <opening> <raw cap> absolute|relative
casting entry skill <spell> default
casting prerequisite add <spell> <prerequisite spell> <minimum grade> <raw proficiency>
casting prerequisite remove <spell> <prerequisite spell>
casting prerequisite trait <spell> <scoped support skill> <raw proficiency>
casting prerequisite traitremove <spell> <scoped support skill>
casting support add <native skill> <opening> <raw cap|native> on|off
casting support remove <native skill>
casting support prerequisite spell <support skill> <source spell> <minimum grade> <raw proficiency>
casting support prerequisite trait <support skill> <source support skill> <raw proficiency>
casting support prerequisite remove spell|trait <support skill> <source>
casting enable on|off
casting validate
casting show
```

For example, Earth and Sorcerer may both admit the native Earth Stone Skin while using
different native skills and 100-unit reserves. Earth chooses `passive`; Sorcerer chooses
`gather`. Author the reserve cap and generator using their existing builders. Configured
reserves are canonical identity resources. New bodies/reattachment do not copy or refill
them; new configured balances initialise to zero. Gathering still pays through the acting
body's existing gathering pipeline. Passive entitlement permits configured generators;
it does not create one or replenish a reserve immediately.

For a `SimpleMagicResource`, these resource-builder arguments opt into explicit
attribute capacity:

```text
capattribute <native body attribute> <trait expression> [raw|effective]
capattribute none
```

Practice is opt-in; old profiles do not enable it implicitly. `grades practice fixture`
authors an explicit empty practice material plan, separate from the manifestation plan,
and leaves its maximum unset so the ordinary admission/profile range applies. All fixture
numbers are provisional tuning. `energy` modifies the designated source cost; fixed
secondary costs still aggregate normally. The native skill's `ClassicImprovement` builder
`interval` setting and casting check improvement limits still govern actual raw gains.
Author and verify an appropriate profile for stock practice; a separate stock practice
maximum must not be introduced. Invalid policy/XML prevents readiness and preserves the
unreadable XML for repair. Practice plan edits reject trailing input without changing the
saved plan.

`variable` binds the selected attribute. Other expression parameters must explicitly
bind native body attributes. Skills, mastery, extended options, unknown parameters and
random functions are rejected. Omitted basis means `effective`; select `raw` explicitly
when bonuses should not affect capacity. The configured reserve uses its canonical
holder's current body, so focusing another instance does not substitute its attributes.
The optional versioned `AttributeCapacity` XML retains the IDs and basis. Invalid or
unreadable configuration fails closed and is preserved for repair. Existing resources
without this XML retain their cap Prog; `capattribute none` or `cap <prog>` restores that
path. Item/cell capacity retains the existing Prog/environment behavior.

Attribute and standard effective-modifier mutations reconcile existing maxima. A lower
valid cap removes excess balance; a higher cap grants no energy. Constructors, effect
restoration and body transitions defer this reconciliation until all contributors have
loaded. Quotes/preflight remain pure and require both a valid finite, non-negative cap
and sufficient actual balance, bounded by that maximum. Invalid caps/balances cannot
fund a configured cast or receive normal credits. No enrolment, attachment, route change
or reconciliation refills the reserve. Custom state-dependent Progs/modifiers must call
`NotifyCapacityChange` after their mutation; live accounting also validates the cap.

Compound spell application batches reconciliation across all target and caster children
for each canonical owner. Complete parent removal/expiration and aggregate effect removals
likewise reconcile after the selected children are removed. Opposing modifiers therefore
cannot discard energy at a transient intermediate maximum. A genuine completed decrease
still clamps once; a completed increase never refills. These mutation batches nest with
load/body restoration, release on exceptions and preserve ordinary resource-effect accounting.
During a live mutation, configured-reserve credits and debits validate finite amounts,
valid current capacity and actual funds, then defer their upper-cap clamp to completion.
A +7 resource child between opposing +5/-5 modifiers therefore credits balance 90 to 97
in either order. Genuine final decreases still remove excess; invalid capacities still
refuse accounting. Other resources keep their existing immediate maximum.
An interrupted operation reconciles the effects actually remaining; it does not roll back
effects or restore energy. Incomplete reconstruction retains its stricter accounting refusal.

See [capacity and affordability](Armageddon_Capacity_Affordability.md) for provisional
attribute scenarios, the complete 82-spell energy envelope and native verification limits.

Prerequisites belong to their particular capability and require every listed edge.
They use the prerequisite spell's trait binding in that route. Only explicit enrolment
with a currently applicable permanent capability merit enables automatic prerequisite
acquisition. Temporary capability attachment alone grants no roots or branches. Trait
changes and acquired-grade changes notify a bounded reverse index, rather than polling
all characters. Newly acquired prerequisites are followed through that affected cascade
in the same evaluation, independent of admission or queue order. Every edge must still
meet its minimum controlled grade and route-bound raw proficiency, and quarantined
prerequisites or grant inputs remain blocked. Only successful new grants or repaired
support openings schedule downstream work;
blocked candidates stop when no further grant can make them eligible.
Cloning a capability generates new policy/admission/support/edge identities and
keeps shared spell references. It does not clone player progress or enrolment.

Support skills are explicit capability-scoped grants, not spells. A typed trait edge
requires the support's durable authorisation under this policy identity and stable grant
key, its actual native skill and the declared raw threshold. Native trait possession alone
does not supply that authorisation. All edges are AND requirements; cycles and duplicate
typed sources fail validation across the combined spell/support graph. `on` marks an
explicit starting support; `off` requires its authored prerequisite path. Optional mundane,
language and psionic support mappings remain builder choices for the later stock installer.

For the Component Crafting bridge, configure the native skill with opening 30/cap 90,
add a spell prerequisite on Shadow Passage at raw 80, then a trait prerequisite on
Read Enchantment at Component Crafting raw 80. Configure Read Enchantment's own admission
with opening 30/cap 90. Bind the support skill to ordinary native crafting/gathering/skill-use
checks and a suitable native improver. The real `ClassicImprovement` hook notifies the
reverse index on gains; no extra polling or independent crafting system is introduced.

For the source roster, use `casting entry skill <spell> 60 90 relative` for the four
spell roots, `30 90 relative` for other spells, and `30 60 relative` for Mend Flesh.
Omitted/default skill settings keep the shared profile opening and native cap with
absolute proficiency gates. Opening applies only to a missing native skill; reruns and
repeated grants preserve existing proficiency. The source tree's parent thresholds remain
absolute raw proficiency, independently of these mastery gates.

Native skill use and positive skill writes (including ordinary lessons) use the highest
cap of the character's applicable permanent, enrolled routes that bind an acquired spell
or a durably authorised support to that skill. Temporary attachment does not raise it. A legitimate uncapped shared route
retains the native ceiling. Losing all eligible capped routes inhibits new gains while
retaining stored raw proficiency; it does not trim history or change the native check.
The native skill definition's cap remains an additional ceiling, so author it high enough
for every legitimate route. Pure cap queries do not enrol, acquire, open skills or save.
Native branching remains available when numerical growth is capped; it never substitutes
for canonical spell acquisition. Character-owned native maximum lookup follows canonical
trait storage, so ordinary improvers can reach the configured source thresholds. Capped
theoretical skills are rejected because their separate practical/theory values have no
supported single raw-cap basis. Uncapped theoretical bindings retain their existing behavior.

Validation rejects missing traits/resources/spells, non-character skills, duplicate
admissions/edges/keys, cycles, invalid grade ranges and unsupported numerical fields.
The designated cost resource must occur on every admitted spell. Enabling configured
casting on a Vancian capability is refused. Invalid configured definitions cannot be used
as an independent legacy grant through a true `SpellKnownProg`.

## Staff acquisition and recovery

```text
magic casting enrol <character> <capability> <reason>
magic casting grant <character> <capability> <spell> <reason>
magic casting resolve <character> <operation-guid> <reconciliation reason>
```

Use quotes for multiword names. Enrolment durably records starting grants once. Explicit
grant is idempotent and opens a missing native skill at the admission's explicit opening,
or the profile's opening value when omitted (10 in the legacy fixture). If native skill persistence is interrupted after acquisition is recorded,
the route refuses until the same explicit grant is retried; an existing skill is never
used to infer acquisition. Removal of capability or admission retains acquired progress.
Support authorisation commits before opening its missing native skill. Repeating enrolment
or reconciling a legitimate permanent route repairs an interrupted support write/opening
without repeating root grants, increasing existing proficiency or refilling reserves.

A `NeedsReview` response contains the operation GUID and failure stage. The receipt
quarantines the affected canonical spell, trait and all cost reserves, plus selected
physical component inputs, across alternative routes and bodies. Inspect the saved
`MagicCastingOperations` diagnostic/payment intent and audit the named resources/items
before acknowledging resolution. Resolution never recasts, refunds uncertain payment,
reapplies effects or rolls again. It can apply a durably recorded successful sample once;
an unrecorded sample leaves the previous controlled grade in place. Deadlines remain spent.
Unrelated records remain usable.

## FutureProg

| Function | Return | Contract |
| --- | --- | --- |
| `hasacquiredspell(character, magicspell)` | Boolean | Pure canonical knowledge query, including inactive routes and empty reserves. |
| `controlledspellgrade(character, magicspell)` | Number | Pure controlled grade; zero if unacquired. |
| `canchannelspell(character, magiccapability, magicspell, number, boolean)` | Boolean | Pure route/grade/body preflight only; excludes target, components and payment. |
| `channelspell(character, magiccapability, magicspell, number, boolean, text)` | Text | Same guarded service as the player command; grade, overreach and native target arguments. Returns status and diagnostic. |
| `grantchannelspell(character, magiccapability, magicspell, text)` | Boolean | Explicit authored grant with nonempty provenance, recorded as `FutureProg: <reason>`. |
| `enrolchannelcasting(character, magiccapability, text)` | Boolean | Explicit idempotent authored enrolment with an applicable permanent capability merit and nonempty provenance. Reuses the staff enrolment service; select it in a chargen-finalisation or NPC-creation workflow. |

These functions use native typed capability/spell variables, not display-name inference.
Treat a grant Prog as an authored mutation, not an inspection hook. Pure queries never
enrol, open skills, perform checks or save. A successful quote/preflight reserves nothing;
execution revalidates the live physical body, route, target, materials, cost and balance.
Authored enrolment records its reason in a terminal journal receipt and the initial spell
grant provenance. Capability enumeration, inspection and temporary attachments never call
this hook automatically. A failed authored enrolment can be retried directly.

## Persistence and extension seams

The generated `ConfigurableCasting` migration introduces empty acquired-spell,
trait-opportunity, enrolment and paid-operation tables. No historical knowledge, Vancian
book, scroll, skill or administrative power is converted automatically. Acquired rows
and trait deadlines use optimistic versions. Enrolments use stable capability GUIDs.

Acquiring a capped admission through an enrolled route also records one terminal
`SkillCapRecorded` journal marker per canonical character/trait. Its versioned XML captures
the capability identity, admission and first cap. This opt-in history survives admission
removal or trait rebinding: when no enrolled permanent route still applies, positive gains
are inhibited and stored proficiency is retained. A current, legitimately enrolled uncapped
shared route restores the ordinary native ceiling. Unrelated legacy/native characters are
unaffected by a capped definition elsewhere. Markers are excluded from unresolved payment
receipts and the staff acknowledgement command cannot alter them; they do not spend resources,
skill opportunities or mastery attempts. The first acquisition and its marker share the same
state-store transaction, so marker failure cannot leave a newly acquired spell without cap history. No schema
change is required. Cap queries load/cache this history without writing it.

Support grants use deterministic per-character/policy/grant operation identities and
versioned XML capturing the scoped key, trait, opening, cap and provenance. `SupportGranted`,
`CappedSupportGranted` and `EnrolmentRecorded` are terminal stages alongside the existing
cap marker. They are excluded from unresolved payment receipts; staff acknowledgement
returns an unchanged result and cannot rewrite them. A capped support's opt-in history
survives removal of its definition. Its existing native proficiency remains intact; no
eligible enrolled permanent route means no further numerical gain. Malformed typed XML
is refused while its unreadable definition remains safely preservable for builder repair.

When valid XML adds a cap to an already granted uncapped support under the same policy
identity and support key, reconciliation records a separate terminal `SkillCapRecorded`
marker. It captures the support key, original grant operation and adopted cap without
rewriting the immutable grant's opening, original cap or provenance. Existing native skill
presence does not skip this adoption. Marker failures leave the grant and proficiency
intact and can be retried; after successful adoption, route loss or support removal still
inhibits gains after restart. Cap inspection remains read-only.

`IMagicCastingCapability`, `IControlledMagicSpell` and `IMagicCastingService` expose policy,
profile, acquisition, routes and immutable resolved quotes. No route state is placed on
the global spell. Typed numerical adapters bind the acting body's effective route trait,
grade, canonical controlled mastery (`mastery`) and unchanged native power in a detached copy. Costs allow `self`; duration allows
`degrees` and `success`; supported effect expressions allow native `outcome`. Scalar
bindings are evaluated before the check and accept only `variable`, `grade`, `power`, `mastery` and
explicit trait references. Unsupported fields/parameters fail readiness with their location.

Incantation configuration is optional, versioned XML within the existing controlled-grade
definition; no new database migration is required. Paid operation XML captures the native
method, volume, language, modifiers, actual words and original formula syntax alongside
the existing canonical identity, grade, targets, materials and payment receipt. Native
SpokenLanguageInfo carries correlation metadata; copied/relayed language payloads retain
the origin but mark it as relay. That metadata grants no casting authority: executable
authority is a single-use scoped token from the original player command. Generated casting
speech and re-entrant callbacks cannot consume it, and existing terminal operation receipts
prevent the same origin from being paid again.

`IMagicSpellEffectOperation` is optional: `Applied`, `NoChange`, `Rejected` or `Unknown`,
plus an optional child effect. The five ARM-02 adapters use one application path. Damage
observes delivered native wound deltas; reportless legacy effects remain usable but cannot
prove mastery. This progression result is separate from the existing invocation outcome.
