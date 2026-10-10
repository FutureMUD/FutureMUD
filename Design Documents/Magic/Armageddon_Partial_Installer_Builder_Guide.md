# Development-only Armageddon installer: disabled in Release

Authority: `Armageddon_Magic_Completion_Implementation_Brief.md`, Library ID
`libfile_a4606cd0097081918d6c9d9e220e6da0`, particularly N22/N23 and the installer/builder-guide requirements.
This guide describes the reviewed partial modules, not a complete Armageddon preset.
The central progress ledger and repertoire tables remain integration-owner documents.

## Scope and readiness

The Armageddon package is **disabled in Release until completion**. It is absent from the enabled seeder/menu catalogue,
exposes no installation questions and refuses direct readiness/installation calls before database access.
There is no runtime or environment switch to enable the incomplete package in a shipped build.
This gate preserves existing authored world data and generic engine capabilities; it does not remove installed content.

Debug development builds retain **Armageddon Magic (partial, prepared world)** for internal/disposable qualification.
Its package opt-in defaults to **No on every visit**, even after a historical Yes.
Confirming the ordinary package screen opens two questions: explicit opt-in and an ID-only JSON binding document.
No declines or standard replay profiles install package content. Generic answer-memory housekeeping still runs.
Binding documents use normal `SeederChoice` memory, are revalidated, and require confirmation on each opt-in.

Core, SkillPackage, Useful and Item seeders are ordering dependencies. Equivalent builder-authored native records
can supply the actual bindings. These dependencies do not create the magic preparation below automatically.
The menu's readiness means configuration/decline is available; it does not mean a complete magic world is ready.

Prepare these native records before opting in:

- An existing magic school and character source/reserve resources. The selected simple reserve must already have
  an authored ordinary body-attribute capacity and deterministic native expression. Select the attribute, expression
  and raw/effective basis explicitly. The installer checks the saved mapping and native expression; it never guesses
  an attribute, replaces the resource, evaluates a fixture cap or refills players.
- A numeric trait decorator; ordinary compiled no-argument Boolean false/true progs; a compiled Boolean Mend policy
  with **(target character, caster character)** parameters. The Mend policy is a builder decision.
- Five distinct existing character-owned utility skills. Source traditions create their own 82 spell skills;
  the five supplied skills are native utility-definition bindings, not grants of source entitlement.
- A real character-owned Gather skill with cap and use improver, and a skilllevel gathering template with native
  concentration expressions, paid methods and the selected reserve destination. Modules validate native method
  structure, prices, source selectors and compiled hooks before committing traditions.
- An existing water liquid, material and builder account; approved Holdable and light revisions. Light must contain
  exactly approved Holdable, Wearable and `Prog Light` components, native wear profiles, positive illumination,
  no wear/load scripts, morphing, read-only state or default GameItem hooks. Optional Water bonus plane is an existing ID.

**Self-only is the conservative suggestion supported by this checkpoint's native scenario.** It does not change the
approved matrix: Sorcerer Self/Gentle/Land; Preserver Self/Gentle; Defiler Self/Land. Each binding selects a nonempty
subset for each variant. The selected template must contain real valid methods; no placeholder Gentle or Land methods
are synthesized. Optional `arm.support.component_crafting` needs a real approved native craft using that skill;
`arm.support.vloran` needs a real native language linked to that skill. Omit unsupported mappings.

Optional new provisions are enabled after independently reviewed installed Active Sense eating/reload qualification.
Leave `Provisions` null to preserve an existing seven-record owned provision module without reconciling its content.
Incomplete, retired, missing, competing or changed stock identities still block dependent reconciliation explicitly.
To select new provisions, first install the ordinary four-admission package and inspect its owned Sustain Meal and
Draw Wine skill IDs. Then opt in again with those actual IDs and approved existing food/liquid selections below.
The installer creates no recipes, food prototypes, nutrition values, classes, character skills or item instances.
The existing owned spell-skill definitions are configuration records, not player grants.

## Binding document

### Optional Fury/Calm mappings

The bounded [Fury/Calm runtime](Armageddon_Fury_Calm_Runtime.md) adds optional `Emotions` to the same binding JSON. Leave it omitted/null to preserve an existing six-record owned module without running it. To select it, provide:

| Field | Required selection |
| --- | --- |
| `FuryAttribute` | Positive ordinary body-owned attribute ID, or null when exactly one Constitution/Physique/Endurance/Body attribute exists. Missing, ambiguous and derived-only inference refuses. |
| `UnitsPerSourcePoint` | Authored finite positive attribute units per source endurance point; no inferred numerical scale. |
| `FuryIntensity`, `CalmIntensity` | Authored finite nonnegative native intensity values. |
| `FuryEligibilityProg`, `CalmEligibilityProg` | Existing compiled Boolean progs with `(target character, caster character)` parameters. These remain builder-owned. |
| `CalmSaveTrait` | Existing native character skill or body attribute used for the opposed save. |
| `CalmSaves` | Exactly seven named native `Difficulty` values, in source grades 1–7 order. Numeric enums are rejected. |
| `Terrains` | Object containing `Air`, `City`, `Inside`, `Hills`, `Mountain`, `Thornlands`, `Earth`, each a distinct positive existing terrain ID. |

These selections extend the source Unravel → Fury → Calm → Mend path using owned tradition skills. The six new owned rows are two spells and their duration/cost expressions. With provisions and Water/See also selected, managed composition is 217 records, 12 payload definitions and 12 stored source admissions per variant, leaving 70 of 82 unavailable. Native installed-world/restart acceptance remains unrun; this does not enable the full Release package. No player state is granted/refreshed. Explicit stock/profile, expression and capability edits remain preserved on selected and null reruns.

### Existing binding schema

Use one JSON line at the prompt. This formatted schema is deliberately **not runnable**: replace every zero ID with
the selected actual ID, use the approved revision numbers (zero is a valid revision), and keep `Install` true.
No examples are silently used as defaults. Unknown/duplicate properties, numeric enum values, invalid IDs and oversized
documents are rejected. The normal API `ArmageddonMagicSeeder.SerializeBindings` produces accepted JSON for typed callers.

```json
{
  "Utilities": {
    "Install": true,
    "School": 0, "Resource": 0,
    "SpellSkills": {
      "arm.spell.sense_enchantment": 0,
      "arm.spell.unravel_enchantment": 0,
      "arm.spell.mend_flesh": 0,
      "arm.spell.draw_water": 0,
      "arm.spell.hovering_light": 0
    },
    "AlwaysFalseProg": 0, "MendEligibilityProg": 0, "Water": 0,
    "LightPrototype": 0, "LightRevision": 0,
    "HoldableComponent": 0, "HoldableRevision": 0,
    "Material": 0, "BuilderAccount": 0, "WaterBonusPlane": null
  },
  "ReserveResource": 0, "Decorator": 0, "AlwaysTrueProg": 0,
  "GatheringTemplate": 0, "SupportSkills": { "arm.support.gather": 0 },
  "CapacityAttribute": 0, "CapacityExpression": 0, "CapacityBasis": "raw",
  "AllowedMethods": { "sorcerer": ["Self"], "preserver": ["Self"], "defiler": ["Self"] },
  "Provisions": null
}
```

For an explicit provision selection, replace `"Provisions": null` with this object, substituting actual existing IDs
and revisions. The final fallback food profile contains exactly three distinct approved native food prototypes, each
with Holdable and Food components, finite positive bites, finite nonnegative nutrition and no eat/load hooks. Wine
recipes must select real liquids with finite nonnegative native nutrition. Ordered category profiles/recipes may precede the ordinary fallback; predicates use the existing
module's compiled caster-Boolean contract. No guessed category or liquid is installed.

```json
"Provisions": {
  "Install": true, "School": 0, "Resource": 0, "AlwaysFalseProg": 0,
  "MealSkill": 0, "WineSkill": 0,
  "FoodProfiles": [ { "Order": 32, "Predicate": 0,
    "Foods": [ { "Id": 0, "Revision": 0 }, { "Id": 0, "Revision": 0 }, { "Id": 0, "Revision": 0 } ] } ],
  "Wine": 0, "WineRecipes": [ { "Order": 32, "Predicate": 0, "Liquid": 0 } ],
  "WineBonusPlane": null
}
```

School, source resource and false prog must match Utilities. Optional WineBonusPlane is an existing positive ID.
Explicit selection runs the provision module between the skill-only bootstrap and Pierce, then applies capability
policy once from the complete plan. A pre-commit failure rolls back that module; earlier modules stay committed.
Loss of post-commit confirmation stops before capability policy changes. Inspect owned records from a fresh context
and rerun the same selections to recover the same IDs. It never replays player casting or eating actions.

The complete plan respects exact admission removals, disabled casting and builder spell/cost overrides. Reruns report
the saved admissions and enabled flag, preserving capability/merit and provision stock baselines. Removing Sustain Meal
or Draw Wine from a variant leaves six saved admissions; it does not grant a replacement or alter other saved entries.

## Inspect and adapt through native builder commands

Use the IDs in the module receipts; quote multiword names. The following command forms come from the actual builder
and player APIs. `<schoolverb>` is the selected school's verb, not a hardcoded command.

```text
magic school show <school>
magic resource show <reserve>
magic resource edit <reserve>
magic resource set capattribute <attribute> <expression> raw
magic capability show <capability>
magic capability edit <capability>
magic capability set casting show
magic capability set casting validate
magic capability set gather list
magic spell show <spell>
magic spell edit <spell>
magic spell set grades show
```

`casting show/validate` exposes the stored source/reserve, admission traits, source prerequisites, openings and raw caps.
`grades show` exposes controlled potency, practice and any authored incantation language/words/methods. No native spoken
language or historical vocabulary is fabricated by this package. Inspect an existing language before authoring an
incantation policy; `magic spell set grades incantation language <language>` edits an already-authored policy and does
not create one. Whole capability XML is a single reconciled field: any builder edit preserves that field on rerun,
so a requested new admission may remain absent. The final report reads the stored policy and says so.

Clone with `magic capability clone <old> <new>` or `magic spell clone <old> <new>` before custom adaptations.
Clones are unowned; capability/admission identities are regenerated by the native clone API. Inspect and validate the
clone, then attach it intentionally through the world's builder workflow. Changes to supplied external templates are
not seeder-owned. For device revisions, use the native `ChargedMagicDevice` component's builder settings:
`role charged|focus|dual`, `eligibility anyone|caster|magictype|acquiredspell`, `capability <capability|none>`,
`spell add|remove <spell>`, `usable <prog|none>`, `mingrade <0-7>` and `checkconfig`.
Approve revisions through the normal item/component workflow; the installer never manufactures live instances.

## Legitimate attachment, progression and use

No existing character is touched by installation. An authorised staff workflow may give the relevant permanent
capability merit with `givemerit <visible target> <merit>`, then explicitly enrol with
`magic casting enrol <character> <capability> <reason>`. The runtime also supports the authored
`enrolchannelcasting(character, magiccapability, reason)` FutureProg. Attaching a capability alone is not acquisition.
Normal acquisition checks the actual source graph; administrative `magic casting grant` is a separate explicit
exception, not a repair for an unavailable source path or a claim that this package is complete.

```text
<schoolverb> gather <capability> methods
<schoolverb> gather <capability> preview <method> <amount>
<schoolverb> gather <capability> <method> <amount>
<schoolverb> gather cancel
<schoolverb> spell "Sense Enchantment"
<schoolverb> cast "Sense Enchantment" grade 1 on self via <capability>
<schoolverb> practice "Sense Enchantment" grade 1 on self via <capability>
magicdevice show <held item>
magicdevice focus <held item> <capability> <spell> <grade> <targets>
magicdevice charge <held item> <capability> <spell> <grade> <missing-charge-count>
magicdevice charged <held item> <complete target selector>
```

Gathering methods and preview remain subject to current entitlement, environmental policy, native payment and capacity.
Practice requires an explicitly authored practice policy; these utilities do not acquire one automatically.
Inspect `<schoolverb> spell` help before using a route. Source raw proficiency and controlled grade are distinct:
roots open at raw 60/grade 1; real parent proficiency 80 opens the appropriate child at raw 30/grade 1, capped at raw 90.
The runtime enforces the entire current source path and per-spell grade policy.

Blank wand/staff templates have capacity 5/10, explicit dual modes, current-caster activation eligibility, and a Mend-only
carrier whitelist. The partial graph has no attainable Mend path, so stock device production/recharge is **unattainable**
through this package. A builder-authored compatible payload/route can use the production service; that adaptation is
not implicit approval or source completion. No stock scrolls, free charges or fabricated Vancian enrolment are installed.
Legitimate production requires currently acquired/admitted reproducible control, pays per charge and per-item supplies,
and creates a homogeneous bank. Interruption after payment gives no refund. Charged use consumes a charge and refuses
depletion; it never falls back to focus or teaches/improves the spell. Stored production potency is frozen while current
activation eligibility and live target resistance/wards are rechecked. Copying a device produces an empty bank.

## Reports, ownership and reruns

### Separately authored N19 ward and bane stocks

Create these definitions explicitly and add/grant them through the intended
configured capability:

```text
magic spell edit new stock severing-refuge <school> <casting skill> <resource> <indoor template room> <source terrain> <fallback room> <seconds per grade> <capacity> school|tag <selector> Incoming|Outgoing|Both
magic spell edit new stock apex-bane <school> <casting skill> <resource> <Boolean eligibility prog (target,caster)> <resistance trait> <difficulty> <damage per grade> <maximum damage> <damage type>
magic spell set effect 1 ward school <school>
magic spell set effect 1 ward tag <tag name>
magic spell set effect 1 ward coverage Incoming|Outgoing|Both
magic spell set effect 1 ward subschools true|false
```

Quote names containing spaces. The refuge uses ordinary `enter refuge` / `leave
outside` movement, shelter capacity and safe evacuation. Its ward matches selected
schools (optionally descendants) or invocation tag names through native magic
interdiction. School/tag editor commands toggle selectors, retaining at least one.
Ordinary physical hazards remain subject to native room mechanics.
Apex Bane accepts the builder's compiled eligibility predicate, native resistance
trait and damage type; its grade damage is capped by the selected maximum. An
ineligible target refuses before payment. Native paid ward/resistance failures
retain payment. The [lifecycle contract](Spell_Owned_Lifecycle.md) records exact
ward authority and occupied teardown. These builder definitions do not extend
the partial installer or the fixed Sorcerer roster.

### Separately authored N18 projection stocks

These stocks use the shared paid casting route and are installed explicitly by a builder:

```text
magic spell edit new stock sand-effigy <school> <casting skill> <resource> <plane> <plain holdable prototype> <seconds per grade> <backlash damage> <energy per grade>
magic spell edit new stock walking-shadow <school> <casting skill> <resource> <nonmaterial plane> <seconds per grade> <range in room edges> <backlash damage> <energy per grade> <cross closed doors true|false>
magic spell set effect 1 lifetime <seconds per grade>
magic spell set effect 1 range <0-32>
magic spell set effect 1 backlash <0-1000>
```

Quote names containing spaces. Add the new spell to the intended configured capability and
grant/acquire it through that route; creating its definition gives no automatic entitlement.
Cast normally on `self`, use `instances` and the numbered `focus` selection, then `focus primary`
to return. Both are observation presences with shared canonical skills/resources and empty
inventory. Sand Effigy is immobile and its figurine is the exact destructible anchor. Walking
Shadow travels through native exits within range while the primary is vulnerable in a trance.
Expiry, damage, severance and logout/reboot collapse their owned temporary graph.
The [lifecycle contract](Spell_Owned_Lifecycle.md) describes custody holds and authored-policy
limits. These definitions do not change the partial installer's inventory or qualify historical parity.

Successful base composition owns 196 records: 21 utility/device, 171 tradition, four Pierce. It creates six payload
definitions but only four prerequisite-closed source admissions per unedited variant: Sense, Unravel, Water and Pierce;
78 source spells lack admission. Mend and Hovering Light definitions do not create entitlement. With explicitly selected
or preserved valid owned provisions, there are 203 records and eight payload definitions; seven admissions/75 unavailable, adding
Sustain Meal, Draw Wine and Hovering Light. Edited variants can retain fewer/different stored admissions.
The report gives each stored variant separately, including its enabled flag. Stored admissions are definitions, not
character acquisition or a gameplay certificate. No classes, role-picker changes, NPC placement, player grants,
passive regeneration, refills, item instances or charge banks are installed.

Ownership is `SeederManagedRecords` with package `ArmageddonMagicSeeder`, module names
`reviewed-five-utilities-and-blank-devices`, `partial-source-traditions`, `reviewed-pierce`, and optional `reviewed-provisions`.
Examples of stable keys are `arm.spell.sense_enchantment`, its `.skill`/`.skill.cap` records,
`arm.capability.sorcerer`, `arm.merit.sorcerer`, and `arm.spell.pierce_concealment`.
Each module receives a fresh independent context and owns a serializable transaction. Utilities bootstrap real root
definitions, then traditions bootstrap 165 skill/cap/improver records without creating or reconciling any capability or
merit policy. Pierce uses the owned source skill. Only after all selected payload definitions commit do traditions
reconcile capabilities and merits against the complete stock admission baseline. Existing capability/merit XML and
baselines remain untouched during bootstrap, including interruption before Pierce commits. Removing Pierce by its
exact spell ID remains a builder override across reruns; the stored admission report reflects that removal.
A failed/blocked/uncertain module stops the sequence. Earlier completed modules stay committed.

Do not claim a cross-module rollback. On interruption or lost acknowledgement, inspect each module's durable ownership
and receipt from a fresh connection; rerun explicitly with the same valid bindings. No automatic retry or replay of
player actions occurs. The same stable IDs are reused. Missing, retired, competing, later-revision and unowned-name
collisions block rather than resurrect/adopt. Preserve historical ownership records; resolve conflicts explicitly.
For paid gameplay uncertainty use the existing staff reconciliation command
`magic casting resolve <character> <operation-guid> <reconciliation reason>`; seeder reruns never resolve player receipts.

## Builder-installed Folded Pocket

Create and approve an item prototype with exactly Holdable and FoldedPocket
components. The folded-pocket component type uses ordinary container open/size
editing; the spell freezes its actual instance capacity and size policy.

```
magic spell edit new stock folded-pocket <school> <casting skill> <resource> <pocket prototype> "<mass per grade>" <maximum size> <real seconds per grade> Bearer|Creator <permanent fallback room>
```

For example, `"2 kilograms" Normal 300 Bearer` is an authored mapping, not
verified historical balance. Configure native capability membership and
acquisition separately, then cast on an ordinary held item. The focus survives;
one new carrier is created. `effect 1` edits prototype, fallback, capacity, size,
lifetime or access for future invocations. Existing pockets retain their original
binding and deadline. Full weight, finite capacity, no pocket nesting or living
occupants, conservation on collapse and withdrawal from held retirements are
described in [the lifecycle contract](Spell_Owned_Lifecycle.md#folded-pocket).
This stock does not change the partial installer's roster or admission counts.

## Verification boundary

The dedicated `tests/ArmageddonPreparedSeederNativeHarness` runs on a UUID-named owned disposable database/server with
source and assembly fingerprints. It exercises actual menu/question/shared-executor paths, positive prepared-world
composition, genuine module rollback, lost acknowledgement, fresh-process reruns, builder overrides, explicit new provision
selection and unselected provision preservation, native configuration and controlled acquisition/grade, gathering and casting scenarios. Its replay mode uses the actual complete
Debug Medieval profile and verifies second-run refusal with an all-table content hash. A blocked first profile must be
reported as blocked; it does not qualify full replay. All five profile inventories/default declines have focused tests.
Read exact checkpoint receipts for outcomes. Native controlled-world scenarios do not certify Telnet/login or a full
running server. Read the separately retained installed Active Sense eating/reload qualification for the consumption repair.
The unattainable stock Mend/device prerequisite path remains an explicit limit.

The separate installed Sense smoke uses legitimate native enrolment, timed paid Self gathering and ordinary paid
grade-one command/speech casting in a running MUD. Normal random failures remain paid failed operations; bounded retries
must obtain their energy through gathering and may exhaust without a success certificate. Read its exact receipt for
achieved outcomes, acquisition/effect persistence and cold-repeat results. This qualification does not establish natural
child unlocking, overreach mastery or whole-repertoire progression.

The full replay uses production-equivalent lazy-loading contexts with the same owned-connection enforcement; controlled
native/module contexts remain unchanged. Before Human, it verifies the saved Core Colour values load through the replay
context and records the corresponding unloaded navigation in a plain context, without inserting values or editing seeders.

An explicitly selected Windows-only lane smoke can keep the replay database owner alive for native boot, seeded Admin
avatar login/LOOK, native save flush, graceful shutdown, cold restart and login. The executor-local child must exactly match
the fingerprinted dedicated Python source. Unique runtime directories and loopback endpoints isolate both MUD processes;
email is disabled in the owned database and the Discord bridge is redirected to a reserved non-listening loopback endpoint.
Parent and child process jobs enforce bounded cleanup and verify zero active owned processes before database disposal.
The Python child waits for a unique matching parent-ready receipt after job attachment before SQL or MUD startup,
so process scheduling cannot put MUD children outside the database owner's job.
Read the exact boot receipt for outcomes or the first substantive failure. Default replay runs no boot child. This basic
server smoke does not certify the whole Armageddon repertoire or remove the stock Mend/device prerequisite limit.
