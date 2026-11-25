---
slug: skills-checks-and-progression
category: Characters and bodies
summary: Build skills and choose how characters acquire, use and improve them. Compare difficulty curves, opposed rolls, training models and skill displays, then apply them to contests, apprenticeships and mastery.
---

# Skills, checks and character progression

When a character tries something difficult, the game answers three separate questions. **May they attempt it?** The activity can require a skill, a minimum rating or a particular knowledge. **How well did they do?** The activity asks for a check and interprets its outcome. **What did they learn?** The check can offer practice, and the skill’s improver decides whether that practice earns progress. A character can succeed without learning, learn from a failure, or be turned away before any roll takes place.

For a new skill, start with [traits and acquisition](#traits-acquisition-and-caps). To tune an action, start with [difficulties and outcomes](#difficulties-targets-and-outcomes). To change how players advance, compare the [four seeded learning models](#four-progression-models-used-by-the-skill-seeders). The [worked examples](#worked-examples) show these choices in use.

## Traits, acquisition and caps

An **attribute** is a characteristic of the person, such as Dexterity. An **ordinary skill** records training in a particular ability. A **derived trait** calculates its value from other traits. A **two-component skill** records practical experience and theory separately and uses an expression to combine them. A **knowledge** records whether a character knows something. Require a knowledge when the character must know a secret technique. Use a skill check when the quality of their execution matters. An activity can require both.

| Value | What it tells the builder |
| --- | --- |
| Stored or raw skill | How far the character has trained. Earned points can outlast a temporary increase to the cap. |
| Effective maximum | How far this character can currently train. The skill’s cap expression establishes the baseline and effects such as project labour impacts can change it. |
| Effective trait value | What the caller can use after applicable limits and adjustments. A bonus that helps with one activity can leave the stored rating untouched. |
| Decorated value | How the sheet presents the rating. A word such as *Master* conveys only the meaning assigned to that range. |

Start with `skill list` and `skill show <skill>`. Look up the cap expression, improver and decorator listed on the definition. New ordinary skills start with a constant cap of 70 and the world’s default improver and decorator. Skills that share one of these objects also share later edits to it.

Decide how someone acquires the first point, separately from how an experienced character earns the next one.

| Route | Who or what controls it |
| --- | --- |
| Character creation | `skill set chargen <prog>` accepts a Boolean prog taking a toon, optionally followed by the trait. A chargen skill group can grant a limited number of picks from an eligible list. The skill’s own `group` field simply organises the sheet. |
| Lessons | `skill set teachable <prog>` tests the teacher and `skill set learnable <prog>` tests the pupil. Both take a character, optionally followed by the trait. `skill set teach <difficulty>` and `skill set learn <difficulty>` govern the two lesson checks. The teacher must know the skill and have something to teach this pupil. The combined result and lesson fatigue matter. A lesson can open a skill or increase an existing one. |
| Discovery through use | A check that permits branching can try to open a missing skill. Formula participation, learnability and the separate branch check govern this route. |
| A curriculum or direct award | A branching improver can grant a named skill once a prerequisite reaches a threshold. Staff can also assign a value with `skill level <character> <skill> <level>`. |

## Trait expressions

A trait expression produces a number. Its **consumer** gives that number meaning. A cap expression produces a ceiling, a check expression produces a base target, and an improver’s amount expression produces a number of points. Inspect an existing expression with `te show <id>`. A shared expression can serve several consumers, so check its assignments before changing its formula.

| Fragment | Meaning |
| --- | --- |
| `dex:17` | Bind the actor’s effective trait with ID 17 to the name `dex`. Substitute a local ID. Later references in the same formula can use the name. |
| `dex:17{raw,noimprove,nobranch}` | Read the unmodified baseline. The independent `noimprove` and `nobranch` flags exclude this parameter from check-side learning and acquisition. |
| `reading:17 - disguise:!42` | Subtract the target’s trait 42 from the actor’s trait 17. The target binding is read-only and contributes zero without a trait-bearing target. |
| `variable` | The trait passed in by the caller. A shared craft check can use the skill selected on each recipe. Include `variable` if that supplied skill should participate in normal check-side improvement. |
| `value` | In a classic improver’s amount formula this is the skill’s current raw value. Other consumers supply their own named inputs. |

Use `te test <id> <character>` to evaluate an expression for a single subject. A preview can establish the arithmetic of a cap. It supplies neither an opposing character nor an opportunity for practice. Keep the effective trait out of its own cap formula to avoid a circular dependency.

## What calls a check?

An action selects an engine **check type**. Each installed type has a target expression, a maximum difficulty credited for improvement and an assigned **template**. The template supplies a resolution method, its difficulty modifiers and policies such as whether the check can improve or open skills. The caller supplies context such as actor, target, selected trait, difficulty and how good an outcome the activity needs.

| Player activity | Engine check | Builder decision |
| --- | --- | --- |
| Make something | `CraftOutcomeCheck` and `CraftQualityCheck` | The recipe sets the skill, difficulty and failure threshold. Product quality has its own check and weights. |
| Work on a project | `ProjectLabourCheck` | The role can gate entry on skill and a positive target. Working ticks use the check. Base hourly progress uses its target number divided by 100. |
| Finish a project phase | `ProjectSkillUseAction` | A `skilluse` completion action gives its configured number of uses to the project owner. Labourers receive their own role-based opportunities. |
| Give or take a lesson | `SkillTeachCheck` and `SkillLearnCheck` | Teaching and learning have separate difficulties and participants. |
| Forage | `ForageCheck` or `ForageSpecificCheck` | General searching and seeking a named foragable can use different settings. |
| Hide | `HideCheck` | An action may draw on contextual modifiers. Check its classifications when building a merit. |
| Use `test` or roll from a prog | `GenericSkillCheck` or `ProgSkillUseCheck` | These have their own assignments. To predict a recipe’s behaviour, inspect the recipe and its actual craft checks. |

Use `show checks` for the installed type, expression, template, missing-trait policy and `Max Improve`. Use `show checktypes` for classifications such as perception, healing or offensive combat. Use `checktemplate show <id>` for the template’s curve, policies and assignments.

### Choose a resolution method

| Template method | Behaviour to design around |
| --- | --- |
| Standard | A percentage target and up to three consecutive rolls give degrees of success or failure. |
| OGL | A d20 is used to meet or exceed the required number. Natural 1 gives Major Fail and natural 20 gives Major Pass. Use this method’s own difficulty values. |
| Static | A deterministic comparison suits a fixed capability threshold. It gives no practice or branching. |
| Timebound | Retains the result for a configured time in the relevant check context. Repeating the action can reuse the earlier answer. |
| Limb | Adds requirements for usable limbs of configured types to a standard check. |
| PassivePerception | Compares core perception with the opposed value using configurable fuzziness before using the underlying timebound check. |
| Branch | Resolves attempts to acquire a missing trait using applicable branching modifiers. It supports single checks. |
| BonusAbsent | On the ordinary check path, uses the expression and row modifier without the usual staged situational bonuses. |

Inspect the `improve`, `branch` and `missing` settings together. `DoNotFail`, `FailIfAnyMissing` and `FailIfAllMissing` govern missing owner-side bound traits. A missing required trait supplied separately by the caller can also force Major Fail. An action may permit practice with an existing skill while preventing discovery of a missing one. [Check template and decorator commands](Check_and_Decorator_Editors.md)

## Difficulties, targets and outcomes

A **difficulty** selects a row. A **target number** is the value the roll is assessed against. An **outcome** is the resulting grade. The following modifiers come from three stock curves. A builder can change them in the installed template. Use `show difficulties` and `show outcomes` for the names recognised in game.

| Index | Difficulty | Skill modifier | Language modifier | Perception modifier |
| ---: | --- | ---: | ---: | ---: |
| 0 | Automatic | +100 | +100 | +150 |
| 1 | Trivial | +75 | +55 | +100 |
| 2 | Extremely Easy | +50 | +35 | +75 |
| 3 | Very Easy | +33 | +22 | +50 |
| 4 | Easy | +15 | +11 | +30 |
| 5 | Normal | 0 | 0 | +10 |
| 6 | Hard | −10 | −10 | −10 |
| 7 | Very Hard | −20 | −20 | −20 |
| 8 | Extremely Hard | −33 | −40 | −30 |
| 9 | Insane | −50 | −60 | −50 |
| 10 | Impossible | −100 | −100 | −100 |

For a Standard check, start with the caller’s difficulty. Applicable bonuses move it towards Automatic, penalties towards Impossible. `CheckBonusPerDifficultyLevel` gives the bonus units per stage. Only whole stages count. Combat burden and contextual constraints can further affect which row is used. Add the final row’s modifier to the evaluated expression. With a base of 50 on the stock skill curve, Hard gives 40. Two stages of help move Hard through Normal to Easy, giving 65. A stage is a move between rows, so its numerical effect depends on where you start.

The first percentile roll determines pass or fail. Count how many consecutive rolls agree with the first, stopping at the first change or after three. The ordering below matters both for an activity’s acceptance threshold and for opposed checks.

| First rolls against the same target | Outcome |
| --- | --- |
| Fail, fail, fail | Major Fail |
| Fail, fail, pass | Fail |
| Fail, pass | Minor Fail |
| Pass, fail | Minor Pass |
| Pass, pass, fail | Pass |
| Pass, pass, pass | Major Pass |

With target 65, roughly 65% of ordinary attempts produce **some** pass. An activity demanding Major Pass succeeds only when all three pass, about 27.5% of attempts. Before adjusting difficulty, identify the threshold you mean to change. A minimum skill can bar entry before a roll. An outcome threshold decides what happens afterwards. The minimum difficulty for practice and maximum difficulty credited for practice govern learning independently.

### Repeated, multi-difficulty and opposed checks

| What the action needs | How a Standard check supplies it |
| --- | --- |
| Another independent attempt | A fresh check rolls again. A craft can request extra skill uses while keeping the first roll as the result for the batch. |
| One performance under two conditions | A two-difficulty check rolls one sequence of three percentiles and grades it twice using the two targets. Both evaluations can offer practice under the normal learning rules. |
| One performance across the difficulty scale | An all-difficulties check grades the same three rolls at all eleven rows. Only the designated **reference difficulty** can offer practice or branching. This supplies comparable results for resolving ties. |
| Two performances in opposition | Each participant supplies their own performance. The action chooses whether to compare two single grades or consult both participants’ all-difficulty results. |

These distinctions describe the Standard method. A static threshold needs no dice, and a Branch check has no multi-difficulty operation.

**Comparing two grades.** The proponent is the side attempting the action. The opponent is the side resisting it. Compare the grades in the order shown above. Equal grades give a stalemate. A gap of one grade gives a **Marginal** victory, two **Minor**, three **Moderate**, four **Major**, and five **Total**. Minor Pass beats Minor Fail by one grade. Minor Fail also beats Major Fail, so an action that requires an outright pass must apply that requirement separately. An action with several observers can compare one actor result against each observer’s own result. Different observers may see different outcomes.

**Resolving a contest that needs a winner.** Some actions compare the two all-difficulty results, starting at each side’s designated difficulty. If the grades match, the first tie sets the direction. Two failures send both sides one row easier. Two passes send both one row harder. Keep moving in that direction until the grades separate, holding a side at the edge if it gets there first. The first difference decides winner and margin. If both reach the end still tied, a coin flip awards a Marginal win. Looking up the next pair of rows reuses each participant’s original dice. It gives neither side extra attempts or extra practice. See [a contest resolved row by row](#resolve-a-tied-contest).

## How skill improvement works

Choose the behaviour you want to reward before setting a percentage. Failure-based learning gives a character a reason to try something uncertain. Success-based learning rewards competent work. If easy successes count and there is no rising difficulty requirement, routine work can train an expert. If only failures count, and an easy task always succeeds, that task provides no path forward.

The check must permit learning and the skill must participate in its expression. The **improver** then decides which uses count, how often they pay off and how much the character gains. Inspect it with `improver show <id>`. Since many skills may share it, create a dedicated model when changing one skill’s progression.

### Choose an improver

| Model | When to choose it |
| --- | --- |
| Classic | An ordinary skill grows by use. Configure qualifying outcomes, chance, amount, difficulty interval and the time between gains. |
| Branching | Use the classic controls and add explicit prerequisite-to-skill grants, with a threshold and opening value for each. Appropriate for a curriculum or skill tree. |
| Theoretical | The activity’s practical or theoretical use category matters to advancement. A two-component skill can apply the gain to the component exercised. |
| Non-improving | Routine checks can use the skill while direct awards or another progression system control its rating. Select an installed non-improving model. The new-model editor offers classic, branching and theoretical. |

### Tune the controls

| Classic improver control | What changes in play |
| --- | --- |
| `improver set success` and `improver set fail` | Independent toggles for qualifying outcomes. A fresh classic model enables both. Inspect the current flags before toggling. Enabling both lets success and failure offer practice. Disabling both removes ordinary gains on use. |
| `improver set chance 10%` | The base chance **given an eligible use**. Applicable learning merits can modify it. At a 60% pass rate, success-only learning with a 10% gain chance gives about 6 gains per 100 attempts before cooldowns, caps and other limits. |
| `improver set amount <formula>` | Points per gain. `1` gives flat steps. `max(1, 5 - value / 10)` gives 3 points at raw skill 20 and 1 point from 40 onwards. Changing the amount changes the size of progress a player sees. |
| `improver set prog <prog>` | For classic improvement, a number-returning prog taking a character, optionally followed by a trait, can supply the gain amount. Use this when the award depends on a character-specific condition beyond the formula. |
| `improver set interval <number>` | How many trained points raise the **minimum difficulty for practice** by one row. Smaller positive intervals require harder work sooner. Zero gives Automatic as the minimum throughout. |
| `improver set nogain <dice>` | A real-time waiting period on that character and trait before it can gain again. `1d60+60` means 61–120 seconds. The character can keep working and earning outcomes during the wait. |

For classic improvement, divide the raw skill by the positive interval, discard the fraction and look up that difficulty index, capped at Impossible. With interval 10, a character at 42 needs at least Easy (4), one at 50 needs Normal (5), and one at 60 needs Hard (6). Then check **Max Improve** on the activity’s check type. Standard improvement credits the bonus-adjusted difficulty up to that maximum. If the check credits at most Normal, increasing the activity to Very Hard still cannot train a character who needs Hard. Conversely, a bonus that makes a Hard job Easy can make the job too easy to teach.

### Four progression models used by the skill seeders

Both skill seeders offer the following choices. Each provides a general **Skill Improver** and a separate classic **Language Improver**. Read the installed definitions before tuning an existing game. The table gives the original settings. Chance is per qualifying use, amount is in trait points, and interval is in trained points per difficulty step.

| Seeder choice | General skills | Languages | What a player experiences |
| --- | --- | --- | --- |
| **RPI** | Classic, failures, 10%, amount 1, interval 100 | Failures, 2.5%, amount 1, interval 200 | Each gain is one point. At ordinary ratings the difficulty floor stays low. A task still has to produce a failure to qualify. |
| **LabMUD** | Classic, failures, 10%, `max(1,5-(variable/10))`, interval 10 | Failures, 2.5%, `max(1,5-(variable/30))`, interval 50 | Early gains are larger. As the character advances, gains shrink towards one point and familiar tasks stop teaching. Language practice follows a gentler difficulty curve. |
| **Armageddon** | Branching, failures, 5%, `max(1,5-(variable/12))`, interval 1000 | Failures, 2.5%, amount 1, interval 200 | A general skill can unlock specialisations through authored branches. Failures still matter even at high skill because its difficulty floor rises very slowly. |
| **SuccessTree** | Branching, successes, 5%, `max(1,5-(variable/12))`, interval 10 | Successes, 2.5%, `max(1,5-(variable/30))`, interval 50 | An expert must succeed at harder work to keep learning. This suits a curriculum in which reliable performance earns access to the next skill. |

All four use `1d500+4000` seconds between gain opportunities for the same trait once its cooldown starts, or 66 minutes 41 seconds to 75 minutes. The stock formulas above use `variable`, the evaluated trait. Use `value` in a custom classic amount when the curve should explicitly follow raw progress. With no modifiers and a rating of 60, all four general models award one point per gain. A failed Normal check at that rating can qualify under RPI and Armageddon. LabMUD needs a failure at Hard or above, and SuccessTree needs a success there. The threshold, outcome and chance work together to create four different experiences.

### Two ways to branch

**Discovery through use** starts when someone attempts a check involving a skill they lack. The template must permit branching, the formula parameter must be branchable, the character must be able to learn the skill, and a separate `TraitBranchCheck` resolves acquisition. `skill set branch <percentage>` tunes that skill’s multiplier on this route. The skill seeders’ question about enabling branching on use controls this check policy.

**An explicit prerequisite** belongs to a branching improver. `improver set branch <child> <base> <threshold> <opening>` grants the missing child at the opening value when the model evaluates a sufficiently trained base. The comparison uses the stored base. An eligible character who has already reached the threshold can receive the child on a use that gives no more points. The stock branching models supply a scaffold with zero trait IDs. For a working curriculum, configure real base and child skills. Use an amount formula on a branching improver because its current save path omits the optional gain prog.

## Practical and theoretical use

A use category belongs to the **activity**. A skill definition determines **what the character stores**. An ordinary skill has one trained value even if one activity calls it practical and another calls it theoretical. A two-component definition stores practical and theoretical values and combines them using its own expression. With `(practical + theory) / 2`, practical 50 and theory 30 produce a rating of 40. Two more points of theory raise the combined rating to 41. The theoretical improver considers the exercised component when assessing practice. Its no-gain effect belongs to the trait, so switching activities does not clear the wait.

A new craft’s outcome roll is practical. `craft set practical` toggles the selected recipe’s outcome roll and its extra uses between practical and theoretical. The craft’s separate quality roll uses its own ordinary practical call. Check both paths before promising that a study exercise offers only theory. A lesson that directly sets the general value of a two-component skill sets both components to that value.

**Current editor limits.** `skill edit new` and `skill clone` create ordinary skills. Selecting a theoretical improver alone cannot create the two stored components or their combining expression. Saving an existing two-component definition through the ordinary skill editor can persist its type as ordinary. The theoretical improver’s `fail` editor currently toggles the success flag, and `skillaudit` can omit two-component skill instances. For ordinary skills, `skillaudit` shows the minimum difficulty needed for practice.

## Decorators and what players see

| Decorator | When it helps |
| --- | --- |
| SimpleNumeric | Shows the effective value directly. Useful when players need to meet a known numerical prerequisite. |
| CurrentMax | Can show trained value, an adjustment in brackets and the effective maximum. Useful when temporary caps or bonuses are visible parts of play. |
| Percentage | Shows the effective value as a percentage of its maximum. A skill at 35 of 70 is halfway to its ceiling. Its chance of success still depends on the action. |
| Range | Displays a named band containing the effective value. Use this for in-world descriptions of proficiency. |

Use `skills` or `skills <group>` for the sheet and `skilllevels <skill>` to inspect the scale. A decorator can change the name or number the player sees without changing any check, prerequisite or award.

## Worked examples

### Read one attempt at two difficulties

A mind-connection power can tell the caster when a barrier blocked an attempt that would otherwise have worked. It grades the same attempt at the adjusted difficulty and at its ordinary difficulty. Suppose only the barrier affects the check, which uses the stock skill curve, with base target 50 and rolls **46, 32, 78**.

| Conditions | Target | Reading the same dice | Result |
| --- | ---: | --- | --- |
| With the barrier, Hard | 40 | 46 fails, then 32 passes | Minor Fail |
| Without the barrier, Normal | 50 | 46 and 32 pass, then 78 fails | Pass |

That combination triggers the barrier’s block message. Rolling twice independently could report a block simply because the second attempt got luckier. Sharing the rolls lets the power attribute the difference to the changed conditions.

### Use one search for several possible finds

Give a greenwood foraging profile two possible finds of the same yield type. Make an ordinary herb Easy and a scarce herb Hard, and allow each on Minor Pass or better. Suppose the forager’s base target is 50 on the stock skill curve and their rolls are **46, 32, 78**. On Easy the target is 65, giving Pass. On Hard the target is 40, giving Minor Fail. Only the ordinary herb qualifies on that search. The profile’s selection rules and the room’s remaining yield still govern what the forager takes home.

This uses all-difficulties resolution without an opponent. Foraging asks once with Normal as its reference difficulty, then reads the row belonging to each candidate. Increasing the rare herb’s difficulty can make it harder to find without increasing the difficulty credited for practice on the search.

### Resolve a tied contest

Two characters use `test <skill> vs <character> <skill> Normal`. Suppose both have a base target of 50, the generic skill check uses the stock Standard curve, and neither receives a bonus. The challenger rolls **35, 55, 82**. The opponent rolls **45, 60, 90**.

| Comparison | Challenger | Opponent | Decision |
| --- | --- | --- | --- |
| Normal, target 50 | 35 passes, 55 fails → Minor Pass | 45 passes, 60 fails → Minor Pass | The first tie is a pass, so read the next harder row. |
| Hard, target 40 | 35 passes, 55 fails → Minor Pass | All three fail → Major Fail | The challenger wins by three grades, a Moderate victory. |

The challenge was issued at Normal. Hard separated the performances. Each character had one reference row for learning. In a direct comparison of just the two Normal grades this same pair would have produced a stalemate.

For several observers, consider someone performing a covert action that calls `SpotStealthCheck` for witnesses. If the actor rolls Pass, one witness rolls Minor Pass and another rolls Major Pass, the first loses the comparison and the second notices. The engine keeps the actor’s one result across the observers. A tie can also count as detection in this action when the observer passed or the actor failed.

### Establish Pottery and give apprentices a way in

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

These commands open lessons to eligible teachers and pupils. The two difficulties apply to lessons. The recipe will set its own difficulty when the apprentice begins making mugs.

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

**During play.** The teacher uses `teach <apprentice> Pottery` and the apprentice accepts the lesson. `learn` reports the learner’s state. Repeated offers during the fatigue period make a poor curriculum. Staff can grant or correct a value with `skill level <character> Pottery <level>`. That is a direct award, so it tells you little about a player’s eligibility for lessons.

### Design an apprenticeship and its specialisation

The guild wants ordinary work to teach a novice, then send the apprentice to harder commissions as their skill grows. Create a dedicated classic improver while the guild has a single trade.

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

With both default outcome flags enabled, the guild offers a one-in-four base chance of one point on a qualifying use. A gain at 49 takes the apprentice to 50. An Easy commission that taught them at 49 is now below their Normal minimum. The short 61–120 second cooldown is a deliberate choice for this workshop. Use the stock seeder timings above for a slower progression.

When the guild adds Glazework as a separate skill, it wants anyone trained to 40 Pottery to open Glazework at 5. Create the child skill first, then replace the classic improver with a branching model. Set its learning controls explicitly so changing the curriculum preserves the workshop’s pace.

```text
improver edit new branching KilnCurriculum
improver set chance 25%
improver set amount 1
improver set interval 10
improver set nogain 1d60+60
improver set branch Glazework Pottery 40 5
skill edit Pottery
skill set improver KilnCurriculum
```

### Give an apprenticeship a character-specific cap

Suppose another guild grants a maximum of 60 plus half the character’s Dexterity. Inspect Dexterity’s local trait ID and substitute it for 17 in the formula.

```text
te new Guild Pottery Ceiling
te set formula 60 + dex:17 / 2
te edit
te test <new-expression-id> <visible-potter>
```

At effective Dexterity 20 the preview should give 70. To adopt it, use `skill edit Pottery` and `skill set expression <new-expression-id>`. The personal mastery example below uses the original constant 70 cap.

### Put the apprentice to work at the kiln

Select the Wayfarer cider mug recipe. Give it a failure phase in its sequence and set the check and failure threshold.

```text
craft set check 1 Pottery Normal
craft set threshold Failure
```

The **1** buys one additional skill-use roll at the failure phase. The first outcome roll decides the batch. The extra roll is a learning opportunity and cannot rescue cracked pottery. With **Failure** as the threshold, Failure and Major Failure take the failure path. Minor Failure is good enough to keep this forgiving recipe on its success path. Separately, the quality check and the recipe’s quality weights determine what comes out of the kiln. A successful craft with coarse clay and poor tools can still produce an ordinary mug. [Build and fire the glazed cider mug](Crafting_System_Builder_Workflows.md#fire-the-wayfarer-inns-glazed-cider-mug)

For a guild examination, ask for one roll when the apprentice submits the work. In a Boolean prog with a character parameter named `actor`, use the following statement.

```futureprog
return Check(@actor, ToTrait("Pottery"), 6) > 0
```

Here 6 means Hard. The function returns −3 to −1 on a failure and +1 to +3 on a pass. Zero can indicate an unresolved character or trait. For admission based on already having 40 Pottery, read the skill value in the eligibility prog. Rolling in a catalogue visibility prog would give a fresh examination each time the player opens the catalogue. [FutureProg for builders](FutureProg_Builder_Guide.md)

### Change a difficulty curve for a whole activity

For **one harder porcelain recipe**, change that recipe’s `craft set check` difficulty. For a **world-wide change to all craft outcome checks**, copy the template from the `CraftOutcomeCheck` row in `show checks`. For example, suppose the guild wants Hard crafts to use −15 on the otherwise familiar Standard curve.

```text
checktemplate clone <existing-standard-template-id> GuildCraftOutcomes
checktemplate set difficulty Hard -15
checktemplate show GuildCraftOutcomes
checktemplate assign GuildCraftOutcomes CraftOutcomeCheck
```

Record the old template ID first. Cloning preserves its other difficulty rows and policies. A new template starts with zero modifiers at every difficulty. Assignment changes **every** recipe using `CraftOutcomeCheck`. The quality check keeps its own assignment. Template edits and assignments save immediately and take effect after a server restart. To restore the prior behaviour, reassign the recorded template and restart. The editor supports the engine’s existing check types. A newly named template on its own has no caller.

### Give the guild named ranks

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

### Mastery as a choice the potter can give up

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
| A skill shows up on a character’s sheet but a new applicant cannot pick it | Check the skill’s availability prog, chargen group eligibility, existing-skill policy, mandatory grants and remaining picks. |
| Someone can use a skill but cannot teach it | Check teachability on the teacher, learnability on the pupil, their relative ratings and `learn` for fatigue. New skills default to teaching disabled. |
| Challenging work never gives a gain | Trace the **actual** check’s expression and improvement policy. Compare its credited difficulty with the improver’s minimum, then check qualifying outcomes, chance, cooldown and the character’s effective cap. |
| Two rolls described as Hard behave differently | Inspect each type’s target expression, template method and Hard modifier, then check the participants’ bonuses and what each caller counts as success. |
| A changed template affects every trade, or a saved edit has no effect yet | Compare the template’s assignments with the live `show checks` after restart. Use a recipe or role’s own difficulty for a local exception. |
| A specialisation appears for unexpected characters | Check which skills share the branching model and which base trait the rule names. An explicit threshold rule grants the child directly. |
| A character trained to 78 currently works at 70 | Inspect their effective ceiling, project assignment and cap impact’s minimum hours before changing the stored skill. |
