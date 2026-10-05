# Optional Armageddon installer: prepared-world checkpoint

Authority: `Armageddon_Magic_Completion_Implementation_Brief.md`, Library ID
`libfile_a4606cd0097081918d6c9d9e220e6da0`, particularly N22/N23 and the installer/builder-guide requirements.
This guide describes the reviewed partial modules, not a complete Armageddon preset.
The central progress ledger and repertoire tables remain integration-owner documents.

## Scope and readiness

The reflected DatabaseSeeder menu includes **Armageddon Magic (partial, prepared world)**.
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
  exactly approved Holdable, Wearable and ProgLight components, native wear profiles, positive illumination,
  no wear/load scripts, morphing, read-only state or default GameItem hooks. Optional Water bonus plane is an existing ID.

**Self-only is the conservative suggestion supported by this checkpoint's native scenario.** It does not change the
approved matrix: Sorcerer Self/Gentle/Land; Preserver Self/Gentle; Defiler Self/Land. Each binding selects a nonempty
subset for each variant. The selected template must contain real valid methods; no placeholder Gentle or Land methods
are synthesized. Optional `arm.support.component_crafting` needs a real approved native craft using that skill;
`arm.support.vloran` needs a real native language linked to that skill. Omit unsupported mappings.

New provisions are temporarily gated by `ArmageddonPreparedWorldInstaller.NewProvisionsQualified`.
Prior native qualification found callback-free eating under Active Sense credited nutrition and left zero-bite food held
in `Retiring` state across reload. The exact cleared runtime repair is integrated without its main-branch ancestry;
independently reviewed combined installed-content eating/reload qualification must precede lifting this source gate.
Leave `Provisions` null. The composition point for the existing production module is retained for that later review.
Existing seven-record owned provision modules are preserved without reconciling their definitions or ownership;
incomplete, retired, missing, competing or changed stock identities block dependent reconciliation explicitly.
Their presence is not a certificate that consumption is repaired.

## Binding document

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

Successful base composition owns 196 records: 21 utility/device, 171 tradition, four Pierce. It creates six payload
definitions but only four prerequisite-closed source admissions per unedited variant: Sense, Unravel, Water and Pierce;
78 source spells lack admission. Mend and Hovering Light definitions do not create entitlement. With an existing valid
owned provision module, there are 203 records and eight payload definitions; seven admissions/75 unavailable, adding
Sustain Meal, Draw Wine and Hovering Light. Edited variants can retain fewer/different stored admissions.
The report gives each stored variant separately, including its enabled flag. Stored admissions are definitions, not
character acquisition or a gameplay certificate. No classes, role-picker changes, NPC placement, player grants,
passive regeneration, refills, item instances or charge banks are installed.

Ownership is `SeederManagedRecords` with package `ArmageddonMagicSeeder`, module names
`reviewed-five-utilities-and-blank-devices`, `partial-source-traditions`, `reviewed-pierce`, and retained `reviewed-provisions`.
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

## Verification boundary

The dedicated `tests/ArmageddonPreparedSeederNativeHarness` runs on a UUID-named owned disposable database/server with
source and assembly fingerprints. It exercises actual menu/question/shared-executor paths, positive prepared-world
composition, genuine module rollback, lost acknowledgement, fresh-process reruns, builder overrides, legacy provision
preservation, native configuration and ordinary progression/gathering/casting. Its replay mode uses the actual complete
Debug Medieval profile and verifies second-run refusal with an all-table content hash. A blocked first profile must be
reported as blocked; it does not qualify full replay. All five profile inventories/default declines have focused tests.
Read exact checkpoint receipts for outcomes. Native controlled-world scenarios do not certify Telnet/login or a full
running server. The combined consumption qualification/review gate and unattainable stock Mend/device path remain explicit limits.
