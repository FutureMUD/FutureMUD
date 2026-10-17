# Armageddon installer and player integration

This guide covers the fixed-install spell, prototype and inactive admission/support graph in Classic and Simple; the source gathering slice is qualified within its dated receipt, with whole-S6 readiness still open. The package contains ten capability/merit pairs, 152 required spells and 210 blueprints; two optional spells remain deferred. Current verification status is recorded in the [programme progress](Armageddon_Completion_Progress.md) and dated qualification checkpoints. Release and package, Class, use, craft and unqualified-carrier availability remain disabled pending the remaining programme gates. Installing the data does not make the stock available to players.

## Install a development world

Use a disposable newly migrated MySQL world for a fresh installation. Run the repository's Debug DatabaseSeeder and complete its normal dependency-ordered setup. Its maintained replay profiles are Debug-only development tools with disposable credentials; they refuse an already seeded database without resetting it. Restore a known compatible backup when resetting a test world rather than rerunning foundational seeders over an existing world.

Choose whether to use classes during the earlier Chargen setup. Armageddon consumes that saved choice and checks the actual Class RolePicker configuration; there is no second class-mode question. With classes off, it creates zero package Class roles. With classes on, it creates ten matching Class roles with the corresponding permanent merit and the mode-specific starting skills/supports. Both choices install the same required content and complete inactive spell admission/support graphs.

Select **Armageddon Magic (partial, prepared world)** from the Debug seeder. The menu name is retained for compatibility. Choose the complete installation question `install-armageddon`, then Classic or Simple at `armageddon-progression-mode`. Answer the capacity-attribute question if automatic selection is ambiguous: select an actual raw mental/spiritual Body attribute. Native binding overrides are requested only when needed; retain the validated automatic bindings when they resolve unambiguously. The complete route does not require builders to finish spell IDs, recipes or prototypes using the legacy prepared-bindings JSON.

Classic uses the locked per-spell proficiency graph. Simple uses one main proficiency per capability, opening at 10/capping at 100, plus the authored component-crafting support where applicable, opening at 10/capping at 90. Installation saves the game-wide direction; it does not convert an existing installation between modes or contradict the earlier class choice.

Inspect the installation report and resolve any typed preflight refusal before continuing. A failed module does not imply rollback of already committed earlier modules. Supported ownership-aware reconciliation preserves builder edits; a second fresh-world Debug replay is a different operation and must refuse. Never repair a mismatch by deleting ownership records or assigning arbitrary numeric IDs.

## Attach a character after availability is authorised

Current stock availability remains closed. The following describes the normal integration once the programme's availability gate is separately approved; it is not an instruction to enable the current false progs.

With classes enabled, the matching selected Class supplies its permanent capability merit and initial traits during ordinary character construction. Character approval flushes the durable character, then invokes selected-Class casting enrolment. An enabled policy must explicitly opt in to that hook. It grants the authored starting knowledge once, at controlled grade 1. A retry uses the same durable enrolment instead of creating a second character or repeating starting grants.

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

Elemental reserves regenerate only through their configured native rules: the stock base is 2 per minute, with the authored resting multiplier. Sorcerer-family gathering is installed under the [source-fidelity decision](Armageddon_Gathering_Source_Translation_20261016.md): Sorcerer Self/Land, Preserver Self and Defiler Land, with bodily sacrifice and real Land destruction. Generic Gentle remains available to other settings. The earlier magical ambient-pool proposal is superseded. The four methods and three normal Gather support grants are installed behind an independently false availability prog. Existing player skills/resources are preserved. Land requires an explicit real life-owner/profile binding; stock installation does not assign rooms, terrains or vegetation. Its zero-output marker is accounting only. See the source translation record for body, terrain, life-unit and clock adaptations and the qualification receipt for executed scope. To inspect and use a capability's gathering methods:

```text
<school verb> gather <capability> methods
<school verb> gather <capability> preview <method> <amount>
<school verb> gather <capability> <method> <amount>
<school verb> gather cancel
```

Only methods actually configured on a capability held by the character are available. Preview does not pay or begin gathering. Raw attribute capacity and proficiency do not imply free energy or free casting.

## Foci and the approved carriers

The ten tradition foci and empty wand/staff blueprints have native production recipes. Production takes the authored real work time. Once availability is authorised, the normal device commands are:

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

Keep separate evidence for selected-Class enrolment tests, a native ordinary-Player fixture, and complete chargen approval/UI. A copied-body test player or a call to the downstream Class service does not certify the whole approval flow. S5's accepted real rest funding and paid practice samples remain evidence within their recorded scope; the install matrix does not repeat or broaden them. N24 ownership reconciliation, N25 integration risks and final Release readiness remain separate gates.
