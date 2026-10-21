# Armageddon installer and player integration

Armageddon v1 is an optional Release and Debug installer package: ten capability/merit pairs, 152 required spells and 210 blueprints. Two optional spells remain deferred. The [v1 readiness policy](Armageddon_V1_Readiness_20261018.md) governs the bounded ordinary-use recommendation; historical receipts retain their original closed-gate scopes.

## Availability policy

Luke explicitly approved offering Defiler immediately alongside the other Classes on fresh classes-on installs. Defiler funds from builder-configured Land; no region or room is bound automatically. PR803 is merged and its reviewed-head checks passed. The [packaging receipt](Armageddon_V1_Release_Packaging_20261018.md) retains its dated execution scope; the [regional qualification](Armageddon_Regional_Land_Qualification_20261018.md) records the new follow-up and its own final-head CI boundary.

| Surface | Fresh explicit full install | Existing installation / builder ownership |
| --- | --- | --- |
| Release package menu | Available; installation defaults to **No on every visit** | Downloading/upgrading binaries performs no installation or activation |
| Complete casting and Classes | Enabled only after complete canonical admissions exist; ten Class roles only if earlier Chargen chose classes | Prior activation defaults retained; edited capability XML and Class availability preserved |
| Magic skills and permanent merits | Skills excluded from generic chargen/teaching; Class selection or explicit staff merit plus enrolment owns starting grants | Existing characters, acquired spells, grades, skill opportunities and reserves untouched |
| Source gathering | Independent permission open; Sorcerer Self/Land/Wildland, Preserver Self, Defiler Land/Wildland | Edited permission bodies preserved; builder supplies eligible real Land/profile/terrain/life bindings |
| Focus/device use and crafts | Editable permission open; native entitlement, knowledge, skill, materials, time and payment checks still apply | Edited policy bodies preserved; incompatible signatures/ownership refuse explicitly |
| Carrier scope and creature foundations | Mend Flesh charged and Restorative Mud topical only; conjured race/culture/ethnicity chargen stays closed | Other candidate carriers and optional variants stay unchanged |

`StockAvailabilityByDefault` is stored atomically with new fixed settings. The full user-facing install sets it only when no earlier Armageddon package ownership exists. Partial module APIs retain conservative defaults. Missing/false on historical settings means retain historical defaults; rerunning never upgrades this flag. The false property is omitted during serialization, preserving historical settings/answer mirrors exactly. A new installation interrupted after settings commit resumes the same intent; incomplete casting/Classes stay closed. Three-way reconciliation preserves owner changes to native rows, including an owner's subsequent decision to close a fresh install's gates.

## Install a fresh world


Back up any world that matters. Use a newly migrated MySQL world for a fresh installation. Run the Release DatabaseSeeder and complete its normal dependency-ordered setup. Its maintained replay profiles are Debug-only development tools with disposable credentials; they refuse an already seeded database without resetting it. Restore a known compatible backup when resetting a test world rather than rerunning foundational seeders over an existing world.

Choose whether to use classes during the earlier Chargen setup. Armageddon consumes that saved choice and checks the actual Class RolePicker configuration; there is no second class-mode question. With classes off, it creates zero package Class roles. With classes on, it creates ten matching Class roles with the corresponding permanent merit and the mode-specific starting skills/supports. Both choices install the same required content and complete spell admission/support graphs.

Select **Armageddon Setting**; this legacy menu/answer-memory identity is retained for compatibility, and its tagline/description identify the complete v1 package. Choose the complete installation question `install-armageddon`, then Classic or Simple at `armageddon-progression-mode`. Answer the capacity-attribute question if automatic selection is ambiguous: select an actual raw mental/spiritual Body attribute. Native binding overrides are requested only when needed; retain the validated automatic bindings when they resolve unambiguously. The complete route does not require builders to finish spell IDs, recipes or prototypes using the legacy prepared-bindings JSON.

Classic uses the locked per-spell proficiency graph. Simple uses one main proficiency per capability, opening at 10/capping at 100, plus the authored component-crafting support where applicable, opening at 10/capping at 90. Installation saves the game-wide direction; it does not convert an existing installation between modes or contradict the earlier class choice.

Inspect the installation report and resolve any typed preflight refusal before continuing. A failed module does not imply rollback of already committed earlier modules. Supported ownership-aware reconciliation preserves builder edits; a second fresh-world Debug replay is a different operation and must refuse. Never repair a mismatch by deleting ownership records or assigning arbitrary numeric IDs.

## Attach a character

For a fresh v1 install, the complete policies and matching Class route are ready. Start with an Earth Elementalist to use native passive rest funding without Land setup. Defiler is offered immediately by the approved fresh Class policy; builders should configure and preview usable Land before players need its Land-only funding. Installation creates no vegetation or region/room bindings.

With classes enabled, the matching selected Class supplies its permanent capability merit during ordinary character construction. Character approval flushes the durable character, then invokes selected-Class casting enrolment. The fresh complete stock policies explicitly opt in to that hook. Enrolment owns the authored starting spell knowledge and skill/support openings, once, at controlled grade 1. Package Class roles have no duplicate magic trait adjustments: those would enter the mandatory skill picker, receive a culture starting value, then add the role amount before enrolment. Supported reconciliation removes untouched legacy stock trait links, preserves and reports a builder-edited link array as a whole, and never changes existing characters. A retry uses the same durable enrolment instead of creating a second character or repeating starting grants.

With classes disabled, attach the permanent capability merit through normal builder/chargen tools. For an existing visible character, staff can use:

```text
givemerit <visible character> <permanent capability merit>
magic casting enrol <character> <capability> <reason>
```

Enrolment is explicit and idempotent. Owning a skill alone does not teach a spell. A temporary capability effect does not establish the permanent entitlement needed for ordinary acquisition. `magic casting grant` is a staff exception, not a substitute for a complete natural progression graph. Installing the package never enrols existing players, fills reserves, resets practice clocks or spawns live NPCs.

## Ordinary player commands

Use the capability's actual school verb and identity. For Earth, the stock verb is `armearth`; the examples use placeholders because IDs belong to the installed world.

```text
armearth practice "Earthen Ward" grade 1 via <Earth capability>
armearth cast "Earthen Ward" grade 1 on self via <Earth capability>
```

Practice is target-free. When authored, it pays the full quote and completes its timed work without manifesting the spell. It can advance raw proficiency through normal skill opportunities; knowledge, raw proficiency and controlled grade remain separate. Normal acquisition opens only eligible branches. Practice does not grant grade seven automatically. An empty reserve refuses before payment or work creation.

Elemental reserves regenerate only through their configured native rules: the stock base is 2 per minute, with the authored resting multiplier. Sorcerer-family gathering follows the [source-fidelity decision](Armageddon_Gathering_Source_Translation_20261016.md): Sorcerer Self/Land/Wildland, Preserver Self and Defiler Land/Wildland, with bodily sacrifice and real Land destruction. Generic Gentle remains available to other settings. The six methods and three normal Gather support grants use an independent editable availability prog, open for a fresh v1 installation. Existing player skills/resources are preserved. Land requires an explicit regional or local life-owner/profile binding. Stock installation creates private terrain/profile presets but never rebinds existing rooms or native terrains or invents live vegetation. Its zero-output marker is accounting only. See the source translation record and regional guide for body, terrain, life-unit and clock adaptations, and the dated qualification receipts for their executed scopes. To inspect and use a capability's gathering methods:

```text
<school verb> gather <capability> methods
<school verb> gather <capability> preview <method> <amount>
<school verb> gather <capability> <method> <amount>
<school verb> gather cancel
```

The regional Land follow-up adds Area/Zone shared life and the `wildland` local forage fallback, taking the package to six methods. See [Regional Land gathering](Regional_Land_Gathering.md) for the nine terrain families, regional-first activation, recovery, upgrade and rollback. Earlier dated qualification statements below retain their executed local-source scope.

Only methods actually configured on a capability held by the character are available. Preview does not pay or begin gathering. Raw attribute capacity and proficiency do not imply free energy or free casting.

For the initial v1.0 balance, the native capacity expression is `max(2*A, min(12*A, 100 + 3*(A-13)))`, where `A` is the selected effective body attribute. The ordinary Simple samples had `A=12` and capacity 97. The tested starting-root grade-one practices cost 50 and take 30 seconds. Earth at rest earned 52 through thirteen native four-unit minute ticks in about 12 minutes 47 seconds, then paid 50 and retained 2 across restart. A failed practice check may award no raw gain; one paid attempt does not guarantee improvement.

The first control-1/request-2 overreach quote is 112.5, so it will not fit capacity 97 even with a full reserve. Under the installed expression, effective attribute 18 is the first integer that can hold it. Improve the relevant attribute or use legitimate configured effects before that attempt; proficiency and a filled pool alone do not increase capacity. This threshold is arithmetic, not a new native overreach test or a change to the locked values.

Sorcerer-family previews also apply the native damage planner to the chosen body part. Self50 was safely refused in the ordinary sample even though total HP was 136; that does not prove all smaller Self batches are unaffordable. In a builder-bound fertile field, Land50 paid seven HP, proportional stun/stamina, one pasture unit and one scar, then funded the first practice. Other terrain, life availability and existing damage can change eligibility and price. Use `methods` and `preview` before committing; builders must supply the explicit eligible land bindings.

## Foci and the approved carriers

The ten tradition foci and empty wand/staff blueprints have native production recipes. Production takes the authored real work time. The normal device commands are:

```text
magicdevice show <item>
magicdevice focus <held item> <capability> <spell> <grade> <targets>
magicdevice charge <held item> <capability> <spell> <grade> <missing-charge-count>
magicdevice charged <held item> <complete target selector>
```

Focus casting is an explicit, fully paid personal cast. Blank wands hold five charges and blank staffs ten; both start empty and are caster-only. Mend Flesh is the sole approved stock charged payload, with 60 seconds of legitimate paid production per charge. Spending a charge does not teach a spell or award casting progress. There is no automatic free fallback or promotion of other candidate payloads.

Restorative Mud is the approved topical substance route. Cast its configured Water spell on a valid local item anchor, then use the native application command:

```text
<Water school verb> cast "Restorative Mud" grade 1 on <anchor> via <Water capability>
apply <source> <target> <bodypart> <amount>
```

The bound dose requires the complete one-gram application once and has a maximum healing budget of `10 * grade`. Partial or repeated use does not create another budget. The native S5 sample qualifies grade 1; the seven-grade budget/provenance contract has managed coverage. Other charged/substance candidate flags remain disabled metadata; no stock scroll route is authorised.

## Verification boundaries

Four fresh-install cells cover Classic/classes off, Classic/classes on, Simple/classes off and Simple/classes on. Their receipts must identify the earlier real Chargen choice, complete installed stock, closed availability, player-state conservation, native loading and owned cleanup. A restored clean pre-package base is a fresh Armageddon install; it does not mean foundational seeders were executed again in that cell.

Keep separate evidence for selected-Class enrolment tests, a native ordinary-Player fixture, and complete chargen approval/UI. A copied-body test player or a call to the downstream Class service does not certify the whole approval flow. S5's accepted real rest funding and paid practice samples remain evidence within their recorded scope; the install matrix does not repeat or broaden them. Completed N24 and representative N25 receipts retain their dated scopes. The [v1.0 readiness policy](Armageddon_V1_Readiness_20261018.md) records the exceptional Borrow database-rejection failure as a deferred follow-up; it is not an extra mandatory gameplay gate. Required builds/CI still apply. This guide does not authorise activating an existing game, merging, deploying or publishing a release.

## Upgrade and activate an existing installation

Keep the server stopped while using the seeder. Take a consistent database backup and retain the matching binaries/configuration first. Apply supported schema migrations through the normal upgrade coordinator; choose Armageddon explicitly to reconcile stock. Keep its original Classic/Simple mode, earlier Class direction and native IDs. Read every preservation/refusal message; do not delete ownership records, replace IDs or rerun foundational seeders over an established world. Package batches commit independently; a later failure retains earlier commits. Reconnect and resume uncertain acknowledgements through the same owned identities.

Upgrading binaries or rerunning a historical install leaves its previous activation defaults intact. Activation of that game is an owner decision outside this task. After reviewing the reconciled content, staff can edit each intended capability with `magic capability edit <capability>` then `magic capability set casting enable on` (use `off` to close it). Use `magic capability set casting classenrol on|off` only for the intended permanent Class routes. Set only the matching Class role availability through its native role editor, and separately edit the exact installed `ArmageddonGatheringAvailable`, `ArmageddonStockFocusDeviceUse` and `ArmageddonStockFocusDeviceCraft` progs, preserving their signatures. Do not blanket-enable all false progs: skill/merit picking, resource starting amounts, conjured-creature chargen and unapproved carriers have independent restrictions. Use the native editor help to verify the selected object and available setters; do not patch installation JSON/fingerprints or shared answer mirrors.

Explicitly enrol each authorised existing character after attaching its permanent capability merit. Enrolment is idempotent; it does not refill energy or reset practice. Check starting knowledge, empty reserve and an actual paid action. For Land, preview the chosen method in each eligible builder-bound area before opening access.

## Rollback

To suspend new access, close the affected Class roles, casting policies, gathering and device/craft permission progs using native editors. Closing casting alone does **not** close gathering or device permissions. Stop further admission and let already-paid work/effects settle under the matching runtime; do not delete player knowledge, reserves, operation receipts, bodies or created items as a rollback technique.

For a full rollback, stop the server and restore the consistent pre-change database backup together with its matching binaries/configuration, then check one login and restart before reopening. Downgrading only the installer is unsupported: older installers reject the new settings field, and data/runtime changes cannot be undone by copying an older executable. Keep the new backup if any post-upgrade player activity needs deliberate reconciliation.
