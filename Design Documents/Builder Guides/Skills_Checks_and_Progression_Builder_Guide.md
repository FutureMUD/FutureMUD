---
slug: skills-checks-and-progression
category: Characters and bodies
summary: Build Pottery from an apprentice’s first skill choice to a master’s commission. Author trait expressions, checks, difficulty curves, improvers, branching, practical and theoretical use, and skill displays.
---

# Skills, checks and character progression

The Wayfarer Inn buys glazed cider mugs from a local pottery. An apprentice can throw a serviceable mug, a journeyman can teach the craft, and a master takes on commissions beyond the ordinary training ceiling. Follow one firing through three decisions. Did the mug survive? How good is it? Did the potter learn anything?

## Follow a skill from definition to use

| Builder decision | Object that owns it | Where to look |
| --- | --- | --- |
| Who can pick or learn Pottery? | Skill definition, chargen choices and acquisition progs | `skill show Pottery`, `chargenskillgroup list` |
| How far can this potter train? | Skill cap expression and applicable cap effects | Cap expression ID in `skill show Pottery`, then `te show <id>` |
| Which ability does the kiln use, and how hard is this batch? | The craft supplies the trait and difficulty to an engine check type | `craft show` on the selected recipe |
| How does the engine resolve that action? | The check type has a target-number expression, template and maximum difficulty credited for improvement | `show checks`, `show checktypes`, `checktemplate show <id>` |
| How does a qualifying use turn into practice? | The check’s improvement policy, followed by the skill’s improver | `improver show <id>` |
| What does the potter see on their sheet? | The skill’s trait decorator | `skills`, `skilllevels Pottery`, `traitdecorator show <id>` |

An **attribute** such as Dexterity can contribute to many activities. An **ordinary skill** such as Pottery belongs to the character and stores earned progress. A **derived trait** calculates a value from other traits. A **knowledge** is a separate possession that can gate a recipe, such as knowing the guild’s glaze formula. Give the recipe a knowledge requirement if its secret matters and a Pottery check if execution can go wrong. Someone may know the formula and still crack the mug.

## Establish Pottery and give apprentices a way in

Begin with `skill list` and `skill show Pottery`. Reuse the skill already used by your pottery recipes. In a new world a Senior Administrator can create it with the skill editor.

```text
skill edit new Pottery
skill set group Artisan
skill set teachable AlwaysTrue
skill set learnable AlwaysTrue
skill set teach Normal
skill set learn Easy
skill edit
```

A new ordinary skill starts with its own constant cap expression of 70, the world’s default improver and decorator, open chargen availability, and teaching disabled. The commands above allow a willing, more skilled potter to teach it. `teach Normal` is the **teacher’s lesson difficulty** and `learn Easy` the **student’s**. Neither sets how hard it is to fire a pot. This example uses the stock always-true prog. For a guild-only skill, `skill set chargen <prog>` takes a Boolean prog accepting a toon, optionally followed by the trait. The teachability and learnability progs take a character, optionally followed by the trait. Check the teacher and the learner in their respective progs.

**At character creation.** If every character may buy Pottery, the ordinary skill picker can offer it through its availability prog. For a background that grants a choice of trades, prepare Pottery and another real skill such as Carpentry. Define `HasWorkshopBackground` as a Boolean chargen prog keyed to your setting’s earlier background choice. Replace the skill IDs with those from `skill list`.

```text
chargenskillgroup create "Workshop upbringing"
chargenskillgroup set member add <pottery-id>
chargenskillgroup set member add <carpentry-id>
chargenskillgroup set picks 1
chargenskillgroup set existing newonly
chargenskillgroup set eligibility HasWorkshopBackground
chargenskillgroup validate "Workshop upbringing"
chargenskillgroup set enabled yes
```

In the skill group the applicant uses `pick Pottery`, then `done`. Their group allocation pays for that base choice. The skill’s `Artisan` group on the sheet is a display category. The chargen group is an entitlement with its own eligibility, members and pick count. If another background already grants Pottery, `newonly` makes the applicant choose an eligible **new** skill. Make sure enough choices remain for applicants who receive mandatory skills. [Character creation](Character_Creation_Builder_Workflows.md)

**During play.** The teacher uses `teach <apprentice> Pottery` and the apprentice accepts the lesson. The teacher must know the skill. The student needs permission to learn and room to learn from this teacher. Teaching and learning checks, their combined outcome and lesson fatigue govern the result. A successful lesson can open a missing skill or add to an existing one. `learn` reports the learner’s state. Repeated offers during the fatigue period make a poor curriculum. Staff can grant or correct a value with `skill level <character> Pottery <level>`. That is a direct award, so it tells you little about a player’s eligibility for lessons.

## Give Pottery a learning curve

The inn needs a steady supply of ordinary mugs, but years of making that one shape should eventually send an ambitious potter looking for harder work. Give Pottery a dedicated model so changes to the guild curriculum do not also affect every skill sharing the world default.

```text
improver edit new classic KilnPractice
improver set chance 25%
improver set amount 1
improver set interval 10
improver set nogain 1d60+60
improver show
skill edit Pottery
skill set improver KilnPractice
```

These are example tuning values. A new classic model permits gains on successes and failures. `improver set success` and `improver set fail` **toggle** the selected option. Inspect it before changing either. At a qualifying use the example gives a base one-in-four chance of a one-point gain, subject to caps and any learning modifiers. Its no-gain period keeps a rapid sequence of uses from awarding points back to back. The dice expression is measured in real seconds. `1d60+60` yields 61–120 seconds. The potter can keep making pots during that time.

**Minimum difficulty for improvement.** With interval 10 the classic model divides the stored skill by 10, takes the whole part and uses that index on the difficulty scale. At 42 Pottery, Easy (4) is sufficient. At 50 it takes Normal (5), at 60 Hard (6), and at 70 Very Hard (7). An interval of 0 removes this rising minimum. Each check also has a **maximum difficulty credited for learning**, visible under `Max Improve` in `show checks`. If that maximum is Normal, even a Very Hard commission using that check cannot satisfy a Hard training minimum. Inspect the installed check before promising a progression route. For ordinary skills, `skillaudit` reports the improver’s minimum and `skills` shows the character’s standing.

| Improver | When to use it |
| --- | --- |
| Classic | One ordinary skill improves through eligible use. `amount` can be a formula using `value`, the current trained value. For example, `max(0.25, 2 - value / 50)` slows gains as the potter advances. A gain prog can replace the amount formula. |
| Branching | Classic improvement plus explicit rules that grant another trait once a base trait reaches a stored value. Useful for a guild curriculum with named specialisations. |
| Theoretical | Evaluates use by its practical or theoretical category. A skill definition built to store two values can advance the corresponding component. An ordinary skill still has one trained value. |
| Non-improving | Keeps a rating fixed through routine checks. Useful when awards or a separate progression system control it. Select an installed non-improving model. The in-game new-model choices are classic, branching and theoretical. |

**Give a journeyman a specialisation.** Suppose `Glazework` already exists and should open at 5 once a potter has trained Pottery to 40. Create a branching improver, configure its chance, amount, interval and cooldown as above, then attach it to Pottery.

```text
improver edit new branching KilnCurriculum
improver set branch Glazework Pottery 40 5
skill edit Pottery
skill set improver KilnCurriculum
```

A skill has one improver. Configure the replacement’s learning settings as well as its branch rule. Use an amount formula in this curriculum. The branching model’s saved definition currently omits the optional gain prog, so a prog selected for this model does not survive reload. The rule inspects the stored base when the model evaluates branches on use. It can grant Glazework once the threshold has already been reached, even on a use that awards no further points. A temporary bonus to Pottery cannot qualify the apprentice for this grant. A different route, **branching on a check**, lets someone attempt an activity involving a missing skill. That route depends on the template’s branching flag, branchable formula parameters, learnability and a separate `TraitBranchCheck`. `skill set branch <percentage>` tunes a skill’s branching multiplier for that route. Choose deliberately whether a player should discover Glazework by trying it or receive it as a curriculum award.

## Write a trait expression for the right consumer

A trait expression returns a number. The caller decides whether that number is a cap, a check target, a gain amount or another quantity. To inspect the inn’s baseline, take the cap ID from `skill show Pottery` and use `te show <cap-id>`. Leave it at 70 for the mastery example below.

For a different campaign, suppose the guild wants a cap of 60 plus half the potter’s Dexterity. Create a separate expression. In this example **17 stands for Dexterity’s trait ID**. Substitute the ID from your own world before entering the formula.

```text
te new Guild Pottery Ceiling
te set formula 60 + dex:17 / 2
te edit
te test <new-expression-id> <visible-potter>
```

At effective Dexterity 20 this gives 70. To adopt that policy use `skill edit Pottery` followed by `skill set expression <new-expression-id>`. The latter selects an existing expression by name or ID. Use `te set formula` to change the mathematics. Editing a shared expression changes every skill or check that uses it. Avoid making the cap depend on Pottery’s own effective value, which already depends on that cap.

| Expression fragment | What it reads |
| --- | --- |
| `dex:17` | The acting character’s effective trait 17 in the evaluation context supplied by the caller. The name before the colon is your label. Reuse `dex` for later references in the same formula. |
| `dex:17{raw,noimprove,nobranch}` | The actor’s unmodified baseline. The two additional flags exclude this bound parameter from check-side improvement and branching. They are independent of `raw`. |
| `variable` | The trait supplied by the caller, such as the selected craft’s Pottery or another craft’s Smithing. In a shared craft check this keeps the formula usable across trades. |
| `reading:17 - disguise:!42` | An actor’s trait minus the target’s trait. Target bindings are read-only and evaluate to zero without a trait-bearing target. Give each binding its own label. A solo craft does not supply an opposing artisan. |
| `value` | In an improver’s amount formula, the current value supplied by that model. Other expression consumers have their own named parameters. |

An expression **can** use a skill without giving the character practice. Evaluating a cap or previewing arithmetic with `te test` does not run an action check. When editing a real check, decide which owner parameters should improve or branch. For the craft-supplied skill to take part in normal check-side improvement its target expression must actually use `variable`. For target-aware formulas, `te test` evaluates for a single subject. It cannot demonstrate the opposing character’s contribution.

## Put the apprentice to work at the kiln

Select the Wayfarer cider mug recipe. Give it a failure phase in its sequence and set the check and failure threshold.

```text
craft set check 1 Pottery Normal
craft set threshold Failure
```

The **1** buys one additional skill-use roll at the failure phase. The first outcome roll decides the batch. The extra roll is a learning opportunity and cannot rescue cracked pottery. With **Failure** as the threshold, Failure and Major Failure take the failure path. Minor Failure is good enough to keep this forgiving recipe on its success path. Separately, the quality check and the recipe’s quality weights determine what comes out of the kiln. A successful craft with coarse clay and poor tools can still produce an ordinary mug. [Build and fire the glazed cider mug](Crafting_System_Builder_Workflows.md#fire-the-wayfarer-inns-glazed-cider-mug)

### Where checks come from

The engine chooses a **check type** at the point where an action needs an answer. A builder usually chooses the skill and difficulty in the editor for that action. Use `show checks` to trace the named check to its actual formula, modifiers policy and template. Use `show checktypes` for classifications such as healing, perception, or offensive and defensive combat. Those classifications matter to effects and contextual bonuses.

| Activity | Check source | What the result controls |
| --- | --- | --- |
| Fire a mug | `CraftOutcomeCheck` and `CraftQualityCheck` | Crossing the recipe’s failure threshold and contributing to product quality are separate decisions. The quality result is retained for the craft. |
| Work a pottery project | `ProjectLabourCheck` | Ordinary skill-based labour can require a minimum trait and a positive target number for entry. A working tick uses the check. Base hourly progress uses its target number divided by 100. A preview estimates work without awarding practice. |
| Finish an apprenticeship phase | `ProjectSkillUseAction` | A `skilluse` completion action makes its configured number of checks for the **project owner**. Choose the action’s trait and difficulty explicitly. Crew members get their own labour opportunities. |
| Teach a pottery lesson | `SkillTeachCheck` and `SkillLearnCheck` | The instructor and pupil use different checks. The instructor’s success alone cannot guarantee a gain. |
| Gather clay by foraging | `ForageCheck` or `ForageSpecificCheck` | Foraging generally and asking for a specific foragable dispatch different check types. Follow the one the player actually used. |
| Conceal a character in the storeroom | `HideCheck` | The hide action supplies its own check. A merit for crafting checks cannot help simply because the actor is a potter. For a stealth or perception merit, inspect the classification of the relevant check with `show checktypes`. |
| Make a one-off guild assessment | `GenericSkillCheck` for a player’s `test Pottery Hard` and `ProgSkillUseCheck` for FutureProg’s `Check` | These use the generic or scripted check’s own settings. Neither reproduces a kiln’s recipe, materials or quality calculation. |

For a guild entrance requirement of “already has 40 Pottery”, read the character’s trait in the eligibility prog. For an actual examination with an uncertain outcome, roll **once** when the apprentice submits the work. For example, in a Boolean prog with a character parameter named `actor`, use this return statement.

```futureprog
return Check(@actor, ToTrait("Pottery"), 6) > 0
```

Here 6 is Hard. The function returns signed degrees, from −3 to −1 for failure and +1 to +3 for success. Zero can also mean the character or trait could not be resolved. A scripted check can award practice under its own template rules. Rolling inside a catalogue visibility prog would let repeatedly opening the catalogue reroll the examination. [FutureProg for builders](FutureProg_Builder_Guide.md)

### Read difficulty, target number and outcome separately

Use `show difficulties` and `show outcomes` for your world’s command vocabulary. Difficulty runs from Automatic (0) through Trivial, Extremely Easy, Very Easy, Easy, Normal (5), Hard, Very Hard, Extremely Hard and Insane to Impossible (10). These names select rows in a template. The outcome is a degree of success or failure. **Minimum skill to enrol**, **difficulty of the task**, **minimum difficulty to learn** and **worst acceptable outcome** are four independent choices.

For example, consider a **Standard** check whose expression gives 50, with the seeded Skill Check modifiers of −10 for Hard, 0 for Normal and +15 for Easy. A Hard commission with no modifiers gives a target of 40. If the total applicable bonus is two difficulty stages, Hard moves through Normal to Easy, for a target of 65. The world setting `CheckBonusPerDifficultyLevel` determines how many bonus units make a stage. Effects, merits and the circumstances of the check can supply them. Inspect your installed template before borrowing these numbers.

On the standard percentage roll, 31 followed by 82 against 65 gives **Minor Pass**. The first roll succeeds and the second ends the run. Rolls of 31, 54, 82 give **Pass**. Three consecutive successes give **Major Pass**. The same rule on the failing side gives Minor Fail, Fail or Major Fail. A template’s missing-trait policy can still impose Major Fail. Thus increasing the target changes both the chance of success and how often exceptional results occur.

### Choose the check method and the scope of an edit

| Template method | What a builder is choosing |
| --- | --- |
| Standard | A percentage target with degrees of success, staged difficulty and configurable learning. A useful starting point for a trade. |
| OGL | Roll a d20 and meet or exceed the required number. A natural 1 is a Major Fail and a natural 20 a Major Pass. Use this method’s own difficulty values. |
| Static | A deterministic comparison for a predictable threshold. It supplies no skill practice or branching. |
| Timebound | Reuses a recent result for the relevant check context within a configured time window. Repeating an action immediately can return the retained result. |
| Limb | Extends a standard check with requirements for usable limbs of particular types. Inspect the limb list when anatomy affects eligibility. |
| PassivePerception | Compares core perception with the opposed value using configurable fuzziness before proceeding to the underlying timebound check. |
| Branch | Resolves a chance to acquire a missing trait, including applicable branching modifiers. This method is for acquisition. |
| BonusAbsent | Uses the formula and difficulty modifier without the usual staged situational bonuses on its ordinary check path. |

For **one harder porcelain recipe**, change that recipe’s `craft set check` difficulty. For a **world-wide change to all craft outcome checks**, copy the template from the `CraftOutcomeCheck` row in `show checks`. For example, suppose the guild wants Hard crafts to use −15 on the otherwise familiar Standard curve.

```text
checktemplate clone <existing-standard-template-id> GuildCraftOutcomes
checktemplate set difficulty Hard -15
checktemplate show GuildCraftOutcomes
checktemplate assign GuildCraftOutcomes CraftOutcomeCheck
```

Record the old template ID first. Cloning preserves its other difficulty rows and policies. A new template starts with zero modifiers at every difficulty. Assignment changes **every** recipe using `CraftOutcomeCheck`. The quality check keeps its own assignment. Template edits and assignments save immediately and take effect after a server restart. To restore the prior behaviour, reassign the recorded template and restart. The editor supports the engine’s existing check types. A newly named template on its own has no caller.

Inspect `improve`, `branch` and `missing` together. The missing-trait options are `DoNotFail`, `FailIfAnyMissing` and `FailIfAllMissing`. The last two apply to owner-side bound traits. A separately supplied required trait may also force failure when missing. A template can allow practice for existing skills while refusing attempts to acquire missing ones. [Check template and decorator commands](Check_and_Decorator_Editors.md)

## Practical work and theoretical study

A potter can learn at the wheel or by calculating a kiln schedule. Those are **use categories** supplied by an activity. On a new recipe the craft outcome check is practical. To make a separate kiln-calculation recipe theoretical, select that recipe, inspect `craft show`, and use `craft set practical` once if it currently says practical. The switch affects the outcome roll and its extra skill-use rolls. The separate quality check uses its own ordinary practical call. If the design calls for theory alone, inspect whether the quality check also permits learning. Its template is shared with other recipes.

An **ordinary Pottery skill** records one number under either use category. A **theoretical skill definition** can store practical and theory separately. Its value expression combines the components into the trait value that ordinary checks and decorators see. For example, a definition using `(practical + theory) / 2` gives a value of 40 from practical 50 and theory 30. A two-point theory gain then changes theory to 32 and the combined value to 41. With a theoretical improver, the minimum difficulty can depend on the selected component, so a modest exercise may still teach the pupil about kiln schedules after it has ceased to challenge their hands.

There is an authoring limit here. `skill edit new` and `skill clone` create ordinary skill definitions. Selecting a theoretical **improver** does not create two stored components, and the ordinary skill editor has no field for their combining expression. On current engine code, saving an existing two-component definition through the ordinary skill editor can persist its type as ordinary. Leave such definitions out of ordinary skill edits. The theoretical improver has an editor defect. Its `fail` command toggles improvement on success. The workshop’s classic model has independent switches. A loaded two-component skill can also be absent from `skillaudit`, which lists ordinary skill instances. Treat a direct lesson award separately from theoretical craft use. Updating a two-component skill through its general value setter sets both components to the assigned value.

## Show players what their standing means

Use a numeric decorator when exact ratings matter. **SimpleNumeric** presents the effective number. **CurrentMax** can show the trained value, an adjustment in brackets, and the effective maximum. **Percentage** presents the effective value as a percentage of that maximum. **Range** translates the effective number into named bands. A potter at 35 of 70 is halfway to the current ceiling. That percentage says nothing by itself about the chance of firing a particular mug.

For the guild, use the words *Apprentice*, *Journeyman* and *Master* on the skill sheet. Create a dedicated range decorator for that vocabulary.

```text
traitdecorator new PotterStanding Range
traitdecorator set ranges set 1 0 35 Apprentice
traitdecorator set ranges add 35 70 Journeyman
traitdecorator set ranges add 70 100 Master
traitdecorator set ranges list
traitdecorator close
```

Ranges meet at the **same** numeric endpoint. In the order above the earlier band includes the shared endpoint, so 35 reads Apprentice, 70 reads Journeyman and 70.5 reads Master. Gapped bands such as 0–35 followed by 36–70 leave fractional skill values unaccounted for and the editor rejects them. Restart to load the new decorator, then use `skill edit Pottery` and `skill set describer PotterStanding`. Players can use `skills Artisan` and `skilllevels Pottery`. Administrator skill listings can use their own numeric CurrentMax view. A guild appointment or recipe prerequisite should refer to the actual skill or membership. Changing the word printed on the sheet grants neither.

## Mastery as a choice the potter can give up

With the ordinary cap still at 70, build a personal project called **Focus on Mastery: Pottery**. Give it an endless role called Focus and require 70 Pottery to join. On that role add a cap impact for Pottery.

```text
project set phase 1 labour add endless Focus
project set phase 1 labour Focus trait Pottery
project set phase 1 labour Focus minskill 70
project set phase 1 labour Focus impact add cap PotteryMastery
project set phase 1 labour Focus impact PotteryMastery trait Pottery
project set phase 1 labour Focus impact PotteryMastery bonus 10
project set phase 1 labour Focus impact PotteryMastery description Focusing on masterwork pottery raises your training ceiling by ten points.
```

Complete the template’s catalogue and initiation settings, submit it for approval, then have the potter start the approved project and join Focus. The project takes their labour assignment. They can keep crafting. Give them a demanding commission commensurate with the improver’s minimum difficulty and the check’s maximum learning credit. A wider ceiling alone awards no points. [Personal project setup and eligibility](Projects_System_Builder_Workflows.md#reserve-a-personal-project-for-skill-mastery)

| The potter’s choice | Stored Pottery | Usable before other bonuses | Effective maximum |
| --- | ---: | ---: | ---: |
| Ordinary training reaches its ceiling | 70 | 70 | 70 |
| Joins Focus | 70 | 70 | 80 |
| Earns eight more points on qualifying work | 78 | 78 | 80 |
| Leaves Focus to take a paid shift | 78 | 70 | 70 |
| Returns to Focus | 78 | 78 | 80 |

While the stored 78 exceeds the ordinary ceiling, practice cannot advance it. Returning to Focus restores access to the earned eight points. With the guild decorator above, the character can move between Journeyman and Master as their effective rating changes. Use a permanent guild award if the title is meant to be held for life.

## When a build behaves unexpectedly

| What the player notices | Where to look first |
| --- | --- |
| Pottery appears under Artisan on one character’s sheet but is absent from a new apprentice’s choices | Check the skill availability prog, chargen group eligibility, existing-skill policy, mandatory grants and the remaining picks. |
| The potter can make a mug but cannot teach the apprentice | Inspect teachability on the teacher, learnability on the pupil, their relative effective ratings and `learn` for fatigue. New skills default to teaching disabled. |
| A successful mug earns no practice | Check `improve` on the outcome template, the use of `variable` in its expression, the credited difficulty, the improver’s pass/fail flags, chance, cooldown and the character’s cap. One success promises no gain. |
| Harder orders change failure rates but ordinary mugs are still the same quality | Inspect `CraftQualityCheck`, the recipe’s quality formula and the material/tool weights. |
| A guild specialisation appears for the wrong people | Inspect everyone using the branching improver and its base trait. An explicit threshold rule grants its child directly. Use separate models or the role progression system for separate curricula. |
| A template edit has no effect, or changes every trade | Compare the saved template’s assigned check types with the live `show checks` after restart. Change a recipe or labour definition for a local exception. |
| The potter has trained to 78 but currently works at 70 | Inspect their current project assignment, the cap impact’s scope and minimum hours, and their effective ceiling before changing the saved skill. |
