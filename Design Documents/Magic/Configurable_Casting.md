# Configurable casting (ARM-02)

This is the implementation guide for [ARM-02](ARM02_Configurable_Casting_Implementation_Brief.md).
The broader [ARM-01 contract](Armageddon_Casting_Design.md) includes later features; this slice
provides explicit acquisition, normal casting, controlled grades and prerequisite progression.
It does not install the candidate repertoire as stock content. See the
[verification handover](Configurable_Casting_Handover.md) for executed evidence and limits.

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

One paid cast may attempt native skill improvement per identity/trait per 60 seconds.
One next-grade attempt consumes a mastery opportunity per identity/spell per 600 seconds.
Only a successful cast with an applied intended operation can sample the profile's 25%
mastery chance. Empty groups, all-target rejection, no-op blindness removal, wholly
absorbed damage, unknown effect reports and caster effects alone do not qualify. A failed
native check can still improve its skill under the native improvement model.

`practice`, `formula`, `quiet` and `area` are unavailable. They do not select an ordinary
manifestation. Existing legacy casting and independent spell-backed powers/Vancian routes
retain their own syntax and payment rules.

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
casting prerequisite add <spell> <prerequisite spell> <minimum grade> <raw proficiency>
casting prerequisite remove <spell> <prerequisite spell>
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

Prerequisites belong to their particular capability and require every listed edge.
They use the prerequisite spell's trait binding in that route. Only explicit enrolment
with a currently applicable permanent capability merit enables automatic prerequisite
acquisition. Temporary capability attachment alone grants no roots or branches. Trait
changes and acquired-grade changes notify a bounded reverse index, rather than polling
all characters. Newly acquired prerequisites are followed through that affected cascade
in the same evaluation, independent of admission or queue order. Every edge must still
meet its minimum controlled grade and route-bound raw proficiency, and quarantined
prerequisites or grant inputs remain blocked. Only successful new acquisitions schedule
downstream work; blocked candidates stop when no further grant can make them eligible.
Cloning a capability generates new policy/admission/edge identities and
keeps shared spell references. It does not clone player progress or enrolment.

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
grant is idempotent and opens a missing native skill at the profile's opening value (10
in the fixture). If native skill persistence is interrupted after acquisition is recorded,
the route refuses until the same explicit grant is retried; an existing skill is never
used to infer acquisition. Removal of capability or admission retains acquired progress.

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

These functions use native typed capability/spell variables, not display-name inference.
Treat a grant Prog as an authored mutation, not an inspection hook. Pure queries never
enrol, open skills, perform checks or save. A successful quote/preflight reserves nothing;
execution revalidates the live physical body, route, target, materials, cost and balance.

## Persistence and extension seams

The generated `ConfigurableCasting` migration introduces empty acquired-spell,
trait-opportunity, enrolment and paid-operation tables. No historical knowledge, Vancian
book, scroll, skill or administrative power is converted automatically. Acquired rows
and trait deadlines use optimistic versions. Enrolments use stable capability GUIDs.

`IMagicCastingCapability`, `IControlledMagicSpell` and `IMagicCastingService` expose policy,
profile, acquisition, routes and immutable resolved quotes. No route state is placed on
the global spell. Typed numerical adapters bind the acting body's effective route trait,
grade and unchanged native power in a detached copy. Costs allow `self`; duration allows
`degrees` and `success`; supported effect expressions allow native `outcome`. Scalar
bindings are evaluated before the check and accept only `variable`, `grade`, `power` and
explicit trait references. Unsupported fields/parameters fail readiness with their location.

`IMagicSpellEffectOperation` is optional: `Applied`, `NoChange`, `Rejected` or `Unknown`,
plus an optional child effect. The five ARM-02 adapters use one application path. Damage
observes delivered native wound deltas; reportless legacy effects remain usable but cannot
prove mastery. This progression result is separate from the existing invocation outcome.
