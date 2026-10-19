# Configurable casting (ARM-02)

## Remaining S4 definitions, 15 October 2026

The fixed installer declares all 152 required definitions. Definition installation,
focused runtime checks and paid native qualification remain separate; the continuation
verification JSON records their actual receipts. The two optional entries remain deferred. The twelve focus/device blueprints and
recipes are installed with their separate availability gates disabled. Release, package admissions,
Class availability, quiet/area variants and unqualified carriers remain disabled.

Ground Tremor applies `3*grade` local crushing damage and one-third to its caster;
nearby permitted rooms receive diminishing messages and agility-based falls without
remote damage. Airborne, nonphysical, staff, active Stone Skin and Shadow protections
apply. Silt terrain, Water, Wind and Void interrupt propagation. Trace the Path uses
one verified unobscured identity dub and at most `10*grade` ordinary permitted edges,
including grade seven; hidden exits and closed doors do not qualify. Dubs now persist
nullable concealment provenance: legacy unknown provenance does not establish verified
personal recognition. Explicit native dubbing and introduction establish provenance.

Return Tether retains the approved native physical-return adaptation for `30*grade`
seconds. The archived source event reports a direction; it does not itself authorize
arbitrary teleporting. The native return binds the exact original identity, instance,
body, room, overlay, terrain and layer, and does not rewind health or inventory.

Join Traveller moves the caster to a recognised visible traveller, while Draw
Traveller resists and moves that traveller to the caster. Grade-one through six path
bounds are respectively strictly less than and at most `20*grade`; grade seven waives
only that distance bound. They keep distinct component plans. Draw uses the existing
sparse Conjuration ranks through an explicit numeric minimum selector; it does not
change the historical S3 rank hierarchy. Waystep selects either one legally owned
active Anchor Rune's live room or a mapped weighted pool. Banish Traveller uses its
own resisted local target and weighted pool; the archived rune/no-save exception
refuses because no native authority for waiving that save is bound. Urban/wilderness
weights are 10/90, then uniform zone and room selection, with a separate explicit
same-zone elemental pool. Pool tags do not automatically mark existing world rooms.
All relocation routes retain permanent endpoint, plane, projection, ward, body and
native membership checks, with exact compensation or NeedsReview after uncertainty.

Storm Locus is an outdoor Lightning room hazard for `30*grade` seconds. Each due
ten-second pulse commits its cursor before applying `2*grade` native electrical
damage to distinct eligible physical occupants, including the caster. There is no
missed-pulse catch-up or pulse-based training. Water refuses; Void requires the exact
owned Defiler merit. Its persistence and reboot rules are in Spell_Owned_Lifecycle.md.

Native powers have explicit activation tags, edited with `tag <identifier>` in the
power editor and saved under `PowerTags/Tag`. Identifiers are lowercase, bounded to
64 characters and 64 distinct tags. The existing explicit `IsPsionic` classification
contributes `psionic`; names and schools do not infer tags. `MagicPowerBase.CanInvokePower`
checks matching active `IMagicPowerTagSuppression` effects before the existing
invocation prog and rechecks after it. Existing invocation checks use that base
method; the three older activation paths without such a check now call it directly.
Cancellation remains available. There is no global effect-handler admission rule.

Silence the Mind uses this generic tag effect to suppress `psionic` activation for
`30*grade` seconds after normal resistance. It severs matching outbound and reciprocal
native mind contacts. Staff, undead and overlapping active suppression refuse. The
saved child binds physical identity, instance, body and absolute expiry. Riding does
not grant immunity. Ordinary character, account and OOC commands remain available.

Compelling Regard supplies resisted PC/NPC influence for `30*grade` seconds. Its
effect editor has a per-spell `pcs` toggle. `compel <local target> <command>` admits
only exact native `look`, `drop`, `give`, `stand`, `sit`, `kneel`, `prostrate`,
`lounge`, `rest`, `sprawl` and `prone` commands. The target retains normal permission,
state, combat, movement and delay checks. A final exact-command/actor/input admission
rejects aliases, abbreviations, overrides, nested commands and queued continuations;
staff/account/OOC commands are outside the whitelist. Controller/target physical
bindings, locality, visibility and wards are rechecked around callbacks. Logout,
death, body replacement or expiry ends the grant. This does not install a general
follower AI or authorize movement/combat command queues.

Open Threshold creates one native paired transient exit for `120*grade` seconds.
Its typed selector accepts `mark <carried owned Arcane Mark keywords>` or
`traveller <verified identity dub>`. It accepts no raw room IDs. Recognised-character
targeting refuses player characters with Immortal (`IAdminSightEffect`) or WIZINVIS
(`IAdminInvisEffect`) effects unless the caster is in Immortal mode. Staff rank alone
does not bypass this rule. The shared recognition gate also applies to Trace Path
and Remote Traveller, including their ordinary live rechecks. Both permanent
same-plane endpoints, current overlays, selected native layer, configured restricted
and lethal tags and live wards are checked. A marked route durably debits one exact
saved mark use before publishing its portal; uncertain commit enters NeedsReview
without retry. Native traversal checks the portal's current attachment, endpoints,
expiry and wards. Closure unregisters the exit and leaves occupants and goods in
their existing real rooms. Existing general builder portals retain their behavior.

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
readiness fails. The supported typed fields are `boost.Bonus`,
`roomlight.AddedLuxPerPower`, `healingrate.Multiplier` and `needdelta.DrunkDelta`.
The same writer supplies detached invocations and frozen stored-spell copies. It rejects
nonfinite results. Exact alcohol-removal bindings also reject positive or out-of-bounds
amounts before payment. Omitting a binding retains the authored fixed value. A detached invocation receives -1 at grade 1 and -3 at grade 3;
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

Paid practice records completion and releases its mutation fence before notifying the
typed prerequisite index, while retaining the character's owner guard. Raw proficiency
or controlled-grade progress can therefore acquire eligible branches immediately.
Interrupted, uncertain and replayed completions do not trigger this notification.

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
Additional spell invocations, including nested casts, check and pay their costs against
the lesser of actual funds and the current maximum. A successful payment subtracts from
the stored balance without prematurely clipping an enclosing spell's provisional excess;
the enclosing batch still reconciles once at completion. This payment boundary does not
change the same spell's ordinary resource children or opposing-modifier arithmetic.
Additional invocations share one reserve budget per canonical owner and outer mutation
batch, established from capped actual funds at its first successful nonzero payment.
Each additional payment reduces that budget. Later same-spell credits do not replenish
it before the outer batch finishes; they still participate in the spell's raw arithmetic.
Every payment remains bounded by the current capacity and actual funds, so a capacity
change cannot bypass the shared budget. Queries do not consume or establish a budget.
Native power admission and debit use the same configured-reserve budget; configured
casting quotes also respect the remaining budget without consuming it. This is not transactional native-power payment: some powers still apply effects
before their legacy debit and do not propagate a declined debit; that separate ordering
contract is unchanged.
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
School command visibility retains acquired knowledge when a capability is absent. It checks
the school's admitted spell IDs in one database query, rather than querying each admission;
an unknown school or a current matching capability needs no acquisition query.
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

`IMagicCastingService.LearnedAffiliations` is a pure query of exact completed canonical
capability enrolments. Completion follows durable starting acquisitions; incomplete or
misbound records are excluded. Shared canonical spell knowledge cannot infer another
teaching route. Learning history remains when permanent permission becomes inactive.
The native store fetches all relevant enrolments for that canonical owner in one query
per description read. It does not cache the result across reads, so enrolment changes
remain immediately visible.
The active flag checks the supplied physical body's currently attached capability and
enabled valid policy, without demanding current energy or casting posture. A temporary
attachment can reactivate an already learned route; it cannot create learning history.
The Read Attunement adapter uses this query separately from explicit affinity merits in
a description overlay on its actual spell target. Only its exact casting instance can
read that overlay while it can see the target; ordinary casting retains target wards and
resistance. See the [S4 checkpoint](Armageddon_S4_Checkpoint_20261012.md) for the bounded
verification and outstanding installed-stock bindings.

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

## Explicit native NPC mission inputs

The optional shared `IMagicSpellEffectInvocationInputs` capability binds typed parser inputs on detached casting copies before prepared selections are captured or reused, and before effect application/payment. The `roommission` cast trigger resolves the creator's room plus an exact `SpellNpcMissionInputs` payload. Seek/Haunt syntax is `<target character>`; Courier syntax is `<held parcel> to <recipient>`. Native character/item parsers supply references, and missing inputs refuse before casting. Generic stored carriers remain unsupported for `createnpc`; complete target parsing is not carrier qualification.

Input binding draws no randomness and changes no shared spell definition, acquisition or player state. Native missions retain the captured exact target identity, instance, body and spatial state, and exact parcel identity, quantity, registry and held custody. Fresh invocation copies must agree on these inputs. Raw fences run after live admission callbacks, immediately before native creation, after load callbacks, and after controller authority/visibility callbacks immediately before mission construction. A stale input cannot be silently resampled. Failure after committed NPC creation retains a lifecycle hold and refuses replay. Normal controller primary-creator, current acquisition and attached enabled capability guards remain required.

The [S4 checkpoint](Armageddon_S4_Checkpoint_20261012.md) distinguishes focused pipeline/fixture evidence from outstanding paid native creature acceptance.

## Exact native maintenance and linked vitality

S4 opt-in `DirectAlcoholRemoval`, `DirectStaminaAdjustment` and `ExactResourceDepletion`
use the captured native needs model, stamina balance or explicitly selected reserve.
They clamp to native limits, preserve unrelated needs/reserves, and do not call expenditure
multipliers, second winds, caster-credit transfers or refill/grant paths. A missing exact
reserve reports NoChange. Native setter/save-queue failures retain a hold rather than
replaying an uncertain write; successful writes dirty their actual owners.

Owned disease binds native SownSickness kind5, contact exposure, exact grade intensity,
virulence1 and family `arm.effect.disease`. Intensity/virulence must be fixed, finite and
exact before payment; this rule leaves ordinary unowned builder disease definitions intact.
The native infection begins health processing and retains original body/infection identity
across saved parents. Purge Disease removes only the matching owned family/infection.
Toxin-origin persistence and retained toxin detection remain separate unresolved adapters.

Vital Siphon and Devouring Touch heal at most both `5*grade` and new/aggravated native
wound damage delivered to the captured victim body. Armour/item wounds, pain, stun and
existing wounds earn no healing. Leeching Edge applies only to actual native melee-weapon
harm and selects the strongest single fraction, `min(0.1*grade,0.5)`. Healing uses native
Mend-eligible wounds worst first. Explicit resistance and target/caster progs supply world
policy; native Arcane is an editable authored choice. Callback drift before native mutation
refuses; uncertain postcommit work is held without refund or replay.

Canonical contribution keys come from the installer's same resolver list used for execution.
The MySQL lost-ack recovery case interrupts its current final contribution, then requires a
true no-op rerun; it does not assume an older eight-definition endpoint. Definition counts,
S4 gameplay qualification and S5/release admission remain distinct.

## Opt-in one-use reflection and native mobility mappings

Native school wards can set `OneUse` with Incoming/Reflect, editable with the existing ward `oneuse` command. Absence retains unlimited legacy behaviour. Queries do not consume the use. The paid successful reflection commits the exact admitted child/parent token, records spent state and marks owning effects dirty before later callbacks. The same token fences reflection text and downstream payload emote callbacks; stale authority holds the paid recipient, with no replay/refund. Used state and remaining lifetime survive native parent reload.

Stock Turn the Elements selects the explicit S2 Fire school, no inferred union, with floor7 and30*grade seconds. The school is builder editable. Stone Skin binds ordinary physical Agility where unique, otherwise the supported ordinary Dexterity mobility equivalent (the labmud preset); ambiguous candidates hold. The alias retains its exact native ID when a later attribute appears. It creates no attribute, and the spell description records the actual mapped attribute name.

## Gated S4 focus and device stock

The fixed installer follows the152 spell definitions with `ArmageddonStockFocusDeviceInstaller`.
It owns ten `arm.focus.<tradition>` prototypes and `arm.item.blank_wand` / `arm.item.blank_staff`,
plus each exact `.recipe`. Foci use native Role.Focus and real references from the locked
tradition rosters. Blanks use explicit Role.Dual, Caster eligibility, capacity5/10,
empty instances and60-second paid production; only the existing compatible Mend Flesh
payload is initially whitelisted. No item grants knowledge/mastery, and charged mode never
falls back to personal casting. These content references do not automatically qualify
all spell effects or broader S5 carrier production.

Both item-use and craft availability progs remain false. Reruns preserve ordinary builder
customisations and stable identities, while incompatible enabled availability progs refuse
without overwriting the edit. Recipes consume the locked blanks/seals through real native
SimpleItem inputs and SimpleProduct outputs. A timed work phase plus zero-duration final
release phase supplies the actual60/120-second duration under native crafting semantics.

The representative first/cold receipt in the continuation verification covers disposable
availability-only copies, real recipe timers/custody, personal focus, target-free practice,
a paid frozen Mend charge and exact cold bank/reserve retention. An empty explicit practice
plan does not enforce possession of a focus; the sample proves retention while held.
Actual per-entry material/focus requirements remain explicit qualification decisions.
All stock/package/Class/variant/Release gates stay disabled; S5 remains separate.

### Selected equipped-item bonuses

The builder effect `itemempower` uses the `itemchoice` trigger: one configured option
followed by an exact accessible carried item. Configure an option with
`option <name> <Trait|Health|Stun|Stamina|MagicResource> <target id or 0> <seven grade amounts>`.
Trait and MagicResource require an exact current attribute/resource ID; other kinds
require zero. Amounts must be finite and nonnegative. A zero amount at the selected
grade refuses before payment. Choice, grade and exact custody are frozen and rechecked
before payment and application; a changed paid target cannot silently receive another choice.

The resolved child is `SpellEquippedItemBonus`, with `Kind`, `TargetId` and `Bonus`.
The ordinary exclusive spell parent owns remaining duration, save/load, dispel and expiry.
Its contribution requires exact membership in the holder's native WornItems/WieldedItems;
a retained wearable owner or old wield-location marker cannot establish equipment.
Held-only, contained, dropped, transferred or destroyed items give no stale contribution.
Attribute choices flow through native effective trait expressions and combat consumers.
Health/stun raise native wound-tolerance thresholds without removing wounds. Stamina
raises maximum stamina; a MagicResource choice raises only the exact canonical holder's
reserve capacity. Raising either maximum never adds current points. Removal/equipment
changes use existing capacity reconciliation and clamps, preserving exertion and ticks.
These bonuses grant no enrolment, capability or casting permission.

The source-traced stock Kindle options, grade tables and bounded adaptation are recorded in
[the Kindle source decision](Armageddon_Kindle_Source_Trace_20261015.md). The installed
minimum remains33, with60*grade seconds and explicit native pool options. Its owned old
weapon-filter program and spell identities reconcile in place; builder edits remain protected.
