---
slug: building-health
category: Health and environment
summary: Choose a health model, build wound supplies, splints, repair kits and prosthetics, author drugs and poisons, and design surgery with timed phases, tools and effects.
---

# Building the health system

A medical item works when the patient's **body and health strategy** can produce the condition, the **wound** accepts the intended treatment, and the **item or procedure** provides it. Start with one test patient, one practitioner and one treatment room. Prove that the practitioner can use each item directly before putting it in a hospital supply room or selling it as medicine. [Building hospitals](Hospital_Builder_Guide.md)

## Choose the bodies, injuries and skills your game supports

The body prototype determines which external parts, bones and organs exist. The health strategy decides how damage to them becomes wounds, how serious the damage is and how recovery advances. A living character can have a visible wound, a fracture in the underlying bone and internal injuries at the same time. A mechanical body's leaking or damaged parts have different treatment rules. Choose the model before designing a universal-looking first-aid kit.

As a builder, inspect `bodyproto show <body>`, `healthstrategy types` and `healthstrategy list`. For a test race with organ-based health, start with a working character-owned `ComplexLiving` model. This example makes a named copy you can tune independently.

```text
healthstrategy typehelp ComplexLiving
healthstrategy clone <existing-complex-living-id> Clinic Test Living
healthstrategy set bleedcooldown 30
healthstrategy show
healthstrategy close
race edit <test-race>
race set health Clinic Test Living
```

The example spaces bleeding messages 30 seconds apart. The wound can keep bleeding between messages. Cloning copies the strategy's linked trait expressions. Inspect the hit-point, pain and stun expressions and confirm their traits exist on your test population. A race uses its default strategy, while an NPC template can specify an override. Check the race and any NPC override, then record the patient's blood and condition with the staff `health <patient>` diagnostic. Give the practitioner your world's wound-care skills, a suitable surgical skill and the knowledge required by each procedure. A surgical school groups procedures. The knowledge controls who knows them. [Body prototype guide](Body_Prototype_Builder_Guide.md) · [Checks and decorators](Check_and_Decorator_Editors.md)

## Stock a treatment room

Start with distinct items so a tester can see exactly which supply the game chose. The builder command `show treatmenttypes` lists the available treatments. The item name is for people. The treatment type makes the item eligible for a bedside command.

| What to stock | Component and setting | What to test in game |
| --- | --- | --- |
| Dressing or bandage | `Treatment` with `Trauma` | `bind <patient>` starts a timed action against visible, actively bleeding wounds. Compare the observed result and remaining uses. |
| Cleaning cloth and antiseptic dressing | `Treatment` with `Clean` or `Antiseptic` | `cleanwounds <patient>` can use matching supplies. Ordinary cleaning can be attempted without items. An antiseptic item provides additional protection after successful treatment. Cleaning helps prevent infection. An established infection needs its own response. |
| Suture packet | `Treatment` with `Close` | `suture <patient>` selects closing equipment. On an organic wound the bleeding must be trauma-controlled, with any lodged object removed. A previously closed wound is a poor test target. |
| Aftercare kit or poultice | `Treatment` with `Tend` or `AntiInflammatory` | `tend <patient>` improves the wound's healing prospects or treats eligible bodypart pain. Watch the patient over subsequent ticks. A successful tend need not make the wound vanish. |
| Arm splint or leg cast | `Immobilising` with a matching wear profile | Immobilisation follows the external parts the item actually covers while worn. Verify the intended limb on the intended body. |
| Workshop repair kit | `RepairKit` with repair points, skill and limits | `repair <target> why <kit>` explains which damage it can address. Use a damaged prop or a compatible mechanical patient. |
| Replacement hand or limb | `Prosthetic` with a body, sever location and functional setting | Fit it to a test patient with that part missing. Check appearance and what actions the patient can perform after fitting. |

### Build a dressing and matching supplies

Create a dedicated component for a two-use dressing. These commands operate on a fresh component. The `treatment` command toggles the named type, so inspect an existing component before reissuing it.

```text
comp edit new Treatment
comp set name ClinicTraumaDressing
comp set treatment Trauma
comp set uses 2
comp set stages 0
comp edit submit
```

Have a reviewer run `comp review <id>` and `accept edit <comment>`. Create an item with `item edit new`, `item set unique clinic_dressing`, `item set noun dressing` and `item set add <Current-treatment-component-id>`. Set its descriptions, material, size and weight. It needs a Current `Holdable` component if new items in your game do not get one automatically. Submit and review the item, then `item load <item-id>` for each test copy. The same item prototype can later be a shop product and a hospital supply. [Item authoring walkthrough](Item_Authoring_Guidelines.md)

For suture packets repeat the component recipe with `Close` and `uses 1`. For an aftercare roll choose `Tend` and several uses. Positive `stages` makes a treatment check easier and negative stages make it harder. Keep stages at zero while testing whether the item is being selected. A new treatment component begins with unlimited uses (`-1`). A finite, non-refillable item disappears when its last use is spent. `comp set refill` preserves the exhausted item, which stops satisfying treatment requests at zero. A shop or hospital that promises continuous supplies still needs replacements or a deliberate replenishment workflow.

**Try the bedside sequence.** Have a test patient with a visible bleeding organic wound stand with a practitioner who can use their hands. Record the patient's `wounds` and staff `health <patient>` results. Bring a dressing, cleaner, suture packet and aftercare roll within reach, with a spare packet for a second wound. Run `bind <patient>`, let it finish, remove a lodged object with `dislodge <patient> <lodged-item>` if the wound has one, then try cleaning, closing and tending. Obtain consent where prompted. Observe which supply was used and what the next command can now do. Equipment access is rechecked between timed steps. A bleeding wound, a lodged item, clothing hiding the site, a spent packet or a practitioner with no usable hands can each explain why the next step fails.

### Fit a splint and a prosthesis

Use `show wears` and `show wear <profile-id>` to find an arm profile for the patient's body. Make a dedicated immobilising component, then build and approve a holdable splint item using that component.

```text
comp edit new Immobilising
comp set name ClinicArmSplint
comp set add <arm-wear-profile-id>
comp set default <arm-wear-profile-id>
comp edit submit
```

On a test patient with a displaced fracture, try `relocate <patient> <bodypart>` and then have the patient wear the splint over the affected bone. Examine the fracture, remove the splint, and compare. A left-arm profile on a right-arm injury does nothing for that bone. A `Treatment` component marked `Set` on an unworn stick cannot supply immobilisation. Design a distinct cast or an additional valid profile for another limb or species.

For a replacement limb, `comp edit new Prosthetic`, then `comp set body <body-id>` and `comp set bodypart <sever-location>`. The location is the attachment point on the body's sever tree. `race` and `gender` refine the parts the prosthesis represents. Inspect its current settings before toggling `obvious` (appearance) or `functional` (restored use). Submit both component and item. Hold the resulting item and try `attach <prosthetic> <patient>` on a compatible patient. If your clinic offers skilled fitting, configure an `InstallProsthetic` procedure and run `surgery perform "<procedure>" <patient> <held-prosthetic>`. It checks the actual missing part and device. Test a cosmetic and a functional version on a hand that would normally hold an item.

### Calibrate a mechanical repair kit

On a fresh `RepairKit` component, use `comp set check <repair-skill>`, `comp set severity Moderate` and `comp set points 20` for a small workshop kit. At least one valid action echo is required for submission. For example, `comp set echo add $0 check|checks $1 with $2.` identifies the repairer, the target and the kit. Review `comp edit` before changing `type <damage-type>`, `material <material>` or `tag <tag>`. These toggle entries. The kit already accepts several damage types on creation. A material restriction checks the material of the damaged item or mechanical bodypart. Tags filter item targets.

Give the approved kit to a tester and hold it. Compare `repair <target> why <kit>` on an eligible damaged machine and on a target above the severity limit. Then use `repair <target> with <kit>`. The `worst` suffix starts with the hardest eligible damage. The charge budget is repair points, spent according to wound severity. An organic suture packet and a mechanical repair kit can share a workshop shelf, but they need different components and test patients.

## Make a herbal remedy, a poison and its antidote

A drug definition specifies the effects, delivery vectors, potency and metabolism. A wrapper delivers it through play. For example, the healer can sell a jar of salve that relieves simulated pain, a story poison that causes nausea, and a remedy specific to that poison. Use fictional definitions and tune the numbers against your game's health model.

**Moonleaf salve.** Create the analgesic below. Fresh drugs start with `Injected` enabled. These two vector commands add touch and remove the default injected route. On an existing record, inspect `drug show` before toggling either one.

```text
drug edit new Moonleaf Relief
drug set touch
drug set injected
drug set intensity 100%
drug set type intensity Analgesic 100%
drug show
drug close
comp edit new TopicalCream
comp set name MoonleafSalve
comp set quantity 10g
comp set drug add "Moonleaf Relief" 0.1 0.5
comp edit submit
```

The last two numbers are game drug grams per gram of cream and the absorbed fraction from zero to one. After component review, build a holdable item with the unique name `moonleaf_salve` and the noun `salve`. Hold the loaded salve and `apply salve <patient> <bodypart>`. Compare its remaining quantity and the test patient's pain response over time. The underlying wound should still be there. A salve described as a poultice uses the topical drug route. A poultice intended to do bedside tending also needs an appropriate treatment item design.

**A story poison and a matching cure.** Create two more definitions. On each fresh one add `Ingested` and toggle off the default `Injected`. The poison uses a symptom with an observable effect. The antidote names the exact drug it neutralises. Add its effect type before setting that type's payload.

```text
drug edit new Mothberry Blight
drug set ingested
drug set injected
drug set type intensity Nausea 100%
drug show
drug close
drug edit new Mothberry Remedy
drug set ingested
drug set injected
drug set type intensity NeutraliseSpecificDrug 100%
drug set type neutralisespecific Mothberry Blight
drug show
drug close
```

For separately identifiable test items, make one `Pill` component for each definition. Use `comp set drug <drug-id>` and `comp set dose <game-mass>`, then approve and attach each to its own holdable item. Give the antidote item the unique name `mothberry_remedy` and the keyword `remedy` so the cottage shop can stock it. Have a test character `swallow <pill>` and compare the named remedy with an unrelated remedy under the same conditions. Judge the result over absorption and clearance ticks. For the retail version, a bottled remedy can instead use an ingested drug on a liquid with `liquid set drug <drug-id>` and `liquid set drugvolume <percentage>`. Fill the bottles before stocking them. A medicine name, shop price or bottle description alone supplies no drug.

Change one thing per test. Whole-drug `intensity` scales potency, type intensity scales one effect and `metabolism` changes clearance. A route mismatch calls for fixing the wrapper or allowed vector before changing any of those numbers. The specialist guide covers the other families, mixtures, dosage controls and diagnostic examples. [Drug builder guide](Drug_Builder_Guide.md)

## Author surgery with tools and phase effects

A surgical procedure has a coded outcome, a target body prototype, a required knowledge and timed phases. Choose the procedure type for its outcome first. A `DetailedExamination` reveals information, `InstallProsthetic` fits a device, and `InvasiveProcedureFinalisation` closes the recorded site from invasive work. A phase emote supplies narration. Inventory actions find, hold or consume actual items. Special actions can change the patient or stop the phase. A phase prog adds optional scripted behaviour. A hospital service can then offer that same procedure to patients.

### Build an exploratory procedure and its closure

Use a test body with an external part that contains organs, an existing medical knowledge and a surgical skill trait. Pick the same permitted bodypart for both procedures. Make a holdable reusable instrument and a separate disposable pack. Tag their **item prototypes** with two distinct existing tags using `item set tag <tag>`. Below, replace `<instrument-tag>` and `<pack-tag>` with those tag IDs or unambiguous names. Load both items for the practitioner.

```text
surgery edit new ExploratorySurgery <body-id> <knowledge-id> "Clinic Medicine" "Clinic Exploration" "examining"
surgery set trait <surgical-skill-id>
surgery set part <organ-bearing-bodypart>
surgery set beginemote @ begin|begins an examination of $1.
surgery set phase add 10 @ prepare|prepares to examine $1.
surgery set phase 1 held <instrument-tag>
surgery set phase 1 special exposed
surgery set phase add 15 @ complete|completes the examination of $1.
surgery set phase 2 used <pack-tag> 1
surgery edit
surgery close
surgery edit new InvasiveProcedureFinalisation <body-id> <knowledge-id> "Clinic Medicine" "Clinic Closure" "closing"
surgery set trait <surgical-skill-id>
surgery set part <same-bodypart>
surgery set phase add 10 @ prepare|prepares the closure pack for $1.
surgery set phase 1 used <pack-tag> 1
surgery set phase 1 special exposed
surgery edit
surgery close
```

Fresh bodypart-specific procedures use a permitted-parts list. An empty list can prevent every target from qualifying. `part` toggles membership and `forbidden` switches between a permitted and an excluded list. Inspect the result on each procedure. For exploration, the chosen part must actually contain an organ. Closure additionally requires an outstanding surgical finalisation at that same part. Ordinary `suture` addresses a different wound state.

**Phase tools.** `held <tag>` and `wield <tag>` require an appropriate implement and a usable hand or wielder. `inroom <tag>` is useful for a fixed theatre fixture. `used <tag> 1` consumes one item at that phase. Use a disposable pack for this action, since it can consume a whole multi-use kit. Bring **two** packs for the example. The procedure checks phase inventory feasibility at the start and again as work advances. Players can lose access to a tool between phases.

**Phase narration and effects.** Use `surgery set phase 1 emote` without more text to see the supported substitutions. `$0` denotes the surgeon, `$1` the patient and `$i1` the first inventory-plan result in that phase. `special exposed` requires the target site to be uncovered. Test the same site clothed and exposed. For a design where the phase itself causes simulated blood loss, `surgery set phase <n> special bleeding <major-fail> <fail> <minor-fail> <minor-pass> <pass> <major-pass>` accepts six fluid volumes, each as one argument in your game's units. This changes the patient's blood state. Writing an emote about bleeding only changes what the room sees. Inspect `surgery set ?` for specials supported by the particular procedure. `special remove <n>` removes one and `special none` clears the phase's specials.

To add a scripted notification, define a Void FutureProg with two Character parameters, surgeon then patient. For example, `send(@surgeon, "The examination phase is complete.")` can confirm timing. Bind it with `surgery set phase 1 prog <prog>`. It runs after the phase's inventory actions and emote. A `useprog` with a corresponding `whyprog` belongs on a start permission check. Completion and abort progs have procedure-specific signatures. [FutureProg authoring](FutureProg_Builder_Guide.md)

**Commission the pair.** Grant the test practitioner the knowledge with `giveknowledge <practitioner> <knowledge-id>`. Log in as the practitioner and check `surgery list` and `surgery show "Clinic Exploration"`. Try it with the instrument absent first. Restore the instrument, provide a consenting, eligible test patient, and run `surgery perform "Clinic Exploration" <patient> <part>`. Observe the phase echoes, consumption of one pack and the result of the examination. Then use `surgery perform "Clinic Closure" <patient> <same-part>` on a patient who meets the game's helplessness requirement for closure. Confirm the finalisation state, recovery wound and remaining supplies. An interrupted invasive operation can also leave a site needing closure. Test interruption and keep a second practitioner with closure knowledge available before offering the operation as a hospital service.

## Diagnose a failed build

| Observation | Check first |
| --- | --- |
| Bandages sell correctly but the medic ignores them | Inspect the Current component on a loaded copy, its `Trauma` flag and remaining uses. Check the wound is visible and actively bleeding. |
| A cast fits but the fracture receives no support | Inspect the **active** wear profile and which external part covers the broken bone on this body. |
| A purchased remedy is inert | Inspect the loaded item, the linked drug, the selected vector and the amount actually delivered. Check the test patient's initial state and allow time for absorption. |
| A known operation is missing from a worker's list | Compare the procedure's required knowledge to the worker's learned knowledge. Test as that worker. |
| A procedure is listed but cannot start or stops mid-phase | Check the target body and part, consent or patient state, knowledge, access to the site, free hands, live tool tags and consumables. After an interruption check whether this patient now needs closure. |
| Direct care succeeds but a paid request remains queued | The health content is ready. Follow the hospital guide to check the advertised service, room roles, staffing and supply staging. [Building hospitals](Hospital_Builder_Guide.md) |
