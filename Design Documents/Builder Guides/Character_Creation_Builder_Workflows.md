---
slug: character-creation
category: Characters and bodies
summary: Build playable peoples, naming traditions, starting roles and the screens that bring them into play. Follow a river pilot from ancestry and upbringing to guild membership and first login.
---

# Playable peoples and character creation

Suppose everyone raised in a river village learns to swim. An estuary-born child raised there should get the same opportunity. A child of village parents raised in the city might never learn. Attach the opportunity to upbringing and the skill screen. If only ferry crews learn to swim, attach it to the role instead. One question settles many chargen decisions: **who should share this, and who should still get a choice?**

This guide starts with the records a builder authors, then the screens that use them. The [worked examples](#worked-examples) build two communities of one people, establish a ferry pilot's profession and take one applicant through review and first login. Commands in angle brackets need a value from your game. Use list and show to resolve installed records. Quoted multiword names and the order of arguments matter. A setting described as a *toggle* reverses its current state each time.

For a particular editor, jump to [race](#building-a-playable-race), [ethnicity](#ethnicity-and-population), [culture](#culture-and-upbringing), [name cultures](#name-cultures-and-name-entry), [random names](#random-name-profiles), [roles](#roles-entitlements-and-starting-places) or [storyboards](#build-the-application-journey).

## Where should I build this?

| What should the player experience? | Build it here | What that decision buys you |
| --- | --- | --- |
| A different arrangement of limbs, organs, wear locations or movement positions | Body prototype | Every race using that body shares its anatomy. A second population with the same anatomy can reuse it. |
| Every member of a people has a longer reach, different needs or greater endurance | Race and, for a reusable effect, an intrinsic body-scoped merit | PCs and NPCs of that race share the physical rule. A character who changes body can have different physical capabilities. |
| Members of the same race draw their eye colours from different distributions | Characteristic definition and values, then an ethnicity's profile | Two populations can differ in frequency while sharing the same colours and anatomy. |
| A community uses a family name, speaks a trade language and reckons birthdays by a harbour calendar | Culture, name culture, native-language default and language skill grants | An applicant of a different ancestry can share that upbringing. Accent, language skill and the name on the character sheet are distinct choices. |
| A pilot knows how to handle a boat and belongs to the ferry guild | Profession role, trait grants and clan membership | Another member of the same community can choose another occupation. |
| Apprentices choose two river trades, or receive a suggested skill they may replace | Chargen skill selection group, or the skill picker | The group expresses an entitlement. A suggestion spends an ordinary pick and remains editable. |
| One character has a scar, a personal flaw or an unusual description | Individual chargen selection, applicable merit or description pattern | Two siblings with identical heritage can look and play differently. |
| Applicants need experience on the **account** to take a senior post | Resource requirement, role availability and approval authority | You can recognise experienced players, charge for a choice, or reserve it for review. Each has a different effect. |
| The pilot starts at the landing with a coat belonging to them | Starting-location role and entry, room and commencement prog | The first-login action receives the actual character and can place and assign ownership of an item. |

**Authoring order and screen order serve different purposes.** Define the body, supporting models, names, traits, languages and characteristic profiles before referencing them. Create the heritage and role records, then wire the screens. Applicants can still choose a profession early if its advice should guide their later decisions. Treat the seeder-installed storyboards as a starting point. A builder can change this journey in game.

## Building a playable race

A race is the engine's species-like package for a character body. It selects anatomy and establishes mechanical defaults used by chargen and live play. Ethnicity and culture describe other parts of identity and upbringing. Use race for traits shared by the species or body. Use ethnicity for a population's appearance profiles and other heritage distinctions. Use culture for upbringing, naming conventions, calendars, and language defaults. Put a career's skills and benefits on its role. This placement determines what remains true when ancestry, upbringing, and occupation differ.

### Find, inspect, create, and clone

```text
race list
race show <race>
race edit <race>
race edit new "<race name>" "<body prototype>" [<parent race>]
race clone "<source race>" <new race name>
race show <race>
race close
```

The `race edit new` command needs a body prototype and can take an optional parent. Quote multiword race and body names, and the source name when cloning. Both creation and cloning create a same-named ethnicity as well as a race, then leave both editing contexts active simultaneously. Use `race set` for the race and the `ethnicity` command family for the ethnicity. New races start with chargen availability set to `AlwaysFalse`, so they are hidden from applicants until you assign an availability prog.

Cloning copies the source race configuration and retains its parent relationship. Review the copy and parent link before use. Cloning is useful when most race settings should be duplicated. It does not establish a new parent link to the source. Use `race set parent <race>` when an existing race should become a child of another race.

Parent relationships combine live inheritance with values copied into a child at creation. Attributes, characteristics, and intrinsic merits are read through the parent relationship. Many physiological and numeric settings are copied when the child is created and can then be independently edited. Bodypart additions are direct race values, not a live parent default. When removing a parent, the editor separately offers to copy its attributes, characteristics, and bodypart additions. Assigning a parent later does not copy every setting. Do not assume changing a parent updates every child default. Inspect the specific setting in the race editor and `race show` after changing the hierarchy.

Changing or clearing a parent is consequential. A new parent must not create a loop. If its body is incompatible, the engine asks for confirmation before replacing the child's body. Clearing a parent also asks whether to copy its attributes, characteristics, and additional bodyparts into the child before unlinking. Choose deliberately, then review both races and any descendants.

### Choose anatomy before the population

A body prototype defines anatomy used by movement, health, equipment, combat, and other body systems. Reuse one when it fits. Build or adapt a body prototype first when the anatomy differs, then use `race set body <body prototype>`. Setting a race's body requires high-administrator permission. Changing it after characters already use the race can remove organs or drop their inventory, even for offline characters. The body of a child race must count as the parent's body or a subtype.

Two communities with the same anatomy usually need separate cultures or ethnicities, not separate races. A second race creates another species-like set of physical defaults and can affect health, equipment, combat, and character generation.

### Race command surface

Use `race set` while editing. Multiword names may be quoted. Percentage and physical-unit values use the builder's configured unit/culture parsing. The race editor's `race show` output is the concise review of effective and direct values.

| Decision | Race settings | Builder implications |
|---|---|---|
| Shared description and NPC defaults | `name <name>`, `desc`, `skillpackage <NPC skill package>` | Edit the race's description shown to applicants. Skill packages apply to new NPC templates, not PC chargen skills. |
| Body scaling | `body <body prototype>`, `partsize <integer>`, `parthealth <percentage>` | Bodypart size is an integer number of size steps, positive for larger and negative for smaller. Bodypart health is a positive percentage multiplier for part hit points. These don't replace a suitable body prototype. |
| Attribute model | `attribute <attribute>`, `attribute bonus <attribute> <amount>`, `attribute dice <attribute> <dice\|default>`, `roll <dice>`, `cap <integer>` | Toggle which attributes the race uses. A bonus changes racial attribute lookup, a dice override changes chargen rolling for one attribute, `default` clears that override, `roll` sets the shared dice formula, and `cap` sets the sum cap. Changes to attribute rolls and caps affect the character-generation budget. |
| Attribute inheritance | `attribute promote <attribute>`, `attribute demote <attribute>` | Promote moves a setting to the parent race. Demote moves it to child races and removes it from this race. Review the affected family before using either. The old attribute-bonus-prog setting is obsolete and cannot be configured. |
| Characteristics and genders | `genders <gender> [<gender> ...]`; `variable all <characteristic>`; `variable male <characteristic>`; `variable female <characteristic>`; `variable remove <characteristic>`; `variable promote <characteristic>`; `variable demote <characteristic>` | Allowed gender values are Male, Female, NonBinary, and Neuter. Characteristics can be available to all genders or only the named gender. Add characteristic definitions on the race, then configure their valid values and distributions in ethnicity profiles. Removing a characteristic can immediately remove its values from existing characters and requires acceptance. Promote/demote affect the race family. |
| Age and apparent age | `age <category> <minimum age>`; `age description <prog\|none>` | Configure the Child, Youth, YoungAdult, Adult, Elder, and Venerable minimum ages in order. Baby has a fixed minimum. Each threshold must be at least one year and fit between its adjacent stages. The optional description prog changes display text only, not mechanical lifecycle category. |
| Height and weight | `hwmodel <gender> <height/weight model>` | Select the default model for a gender. The separate height and weight storyboard progs set the applicant's allowed measurements. |
| Health and remains | `health <health model>`; `corpse <corpse model>`; `butcher <butchery profile\|none>` | Select the health rules, corpse representation, and body-part yields used by butchery. Coordinate these with anatomy and the health and primary-production guides. |
| Senses, temperature, and pain | `perception <percentage>`; `tempfloor <temperature>`; `tempceiling <temperature>`; `paintolerance <percentage>` | Configure the light-perception multiplier, base tolerable temperatures, and pain-tolerance multiplier. Temperature values accept the game's physical units. Consider how merits and equipment combine with the race baseline. |
| Breathing and bodily fluids | `breathing nonbreather\|simple\|lung\|gill\|blowhole`; `breathingrate <volume per minute expression>`; `holdbreath <seconds expression>`; `breathable liquid\|gas <fluid> <percentage>`; `sweat <liquid\|none>`; `sweatrate <volume per minute>`; `blood <liquid\|none>`; `bloodmodel <model\|none>` | Match breathing strategy and fluids to anatomy. Expressions are trait expressions. A breathable value of 0% makes that fluid unbreathable and overrides an inherited parent breathable. Blood controls bleeding fluid; blood model controls antigen typing. |
| Hunger and thirst | `hunger <percentage>`; `thirst <percentage>`; `hungerlimit <hours>`; `thirstlimit <hours>` | Set baseline need rates and maximum stored satiation time. These values influence daily play and should fit the setting's clock and food availability. |
| Combat and natural armour | `armour <armour type\|none>`; `armourquality <quality>`; `armourmaterial <material>`; `canattack`; `candefend`; `canuseweapons`; `combatsetting <setting\|none>` | Configure natural armour and whether the race can attack, defend, and wield weapons if it has suitable bodyparts. Combat capability toggles invert the current setting. |
| Chargen availability and cost | `chargen <boolean prog>`; `advice <chargen advice>`; `cost <resource> <amount>`; `require <resource> <amount>`; `cost <resource> clear` | Availability progs take one Chargen parameter. Advice is toggled on the race. Cost is deducted when the application is approved. Requirement checks the balance without spending it. Use the documented `cost <resource> clear` form to clear the configured cost or requirement. Don't use hidden access gates where an applicant-facing explanation is needed. |
| Tracking and alert echoes | `trackvisual <percentage>`; `tracksmell <percentage>`; `trackingvisual <percentage>`; `trackingsmell <percentage>`; `alertemote <emote\|clear>`; `alertfar <emote\|clear>` | Configure visual and smell track intensity, tracking skill modifiers, and near/far alert text. The distant alert emote uses `{0}` for the direction. |

The race editor has an optional direct corpse-eating mode (`caneatcorpses`, `biteweight`) and a separate system for making forage yields edible. A forage yield must first exist on a foragable profile. Its syntax is `yield add <yield type> <hunger hours> <thirst hours> <emote>`; adjust an existing profile with `yield <yield type> hunger <hours>`, `water <volume>`, `thirst <hours>`, `alcohol <mass>`, `emote <text>`, or singular `bite <amount>`. This is accepted by the editor even though one race-help entry calls it `bites`. The edible-material commands are advertised in help but currently fail: the dispatcher tries to look up the entire remaining text, including `add`, `remove`, or the nutrition property, as the material name.

### Link a shared body merit

A race's intrinsic merits are body-scoped. Every body using this race receives those merits. Use them for a benefit belonging to the body, such as a species-wide capacity. A character-scoped merit applies across the person's bodies. A career benefit belongs on the chargen role. Race merits are toggled links, and parent-race merits remain effective when a child removes its own direct link.

Use `race show` to check direct and inherited merits. Configure a merit's scope before linking it to a race because changing it to character scope is blocked while any race directly assigns it.

For an established body and race, inspect the existing measurement model with `hwmodel list` and `hwmodel show <model>`. A model describes a distribution for generating bodies. `hwmodel clone <source> <new name>` lets a population vary without changing its neighbours. Set `meanheight` and `stddevheight` for the centre and spread. Use BMI for weight correlated with height, or a direct weight distribution when you have species-specific weights. The editor includes mean, standard deviation and skewness for those distributions. Bind the model on the race with `race set hwmodel <gender> <model>`. For PC height and weight limits also inspect the two picker progs. For generated characters see [building NPCs](NPC_Builder_Guide.md).

## Ethnicity and population

An ethnicity is attached to a parent race. It can select the characteristic profiles applicants use for that race's characteristics, as well as population blood, temperature modifiers, names, advice, costs and eligibility. Its group labels organise records and do not create shared traits.

Use `ethnicity list`, `ethnicity show <name>`, `ethnicity edit <name>`, `ethnicity set ...` and `ethnicity close`. Create a record with `ethnicity edit new <name> <race>`. The parent race must already have an applicable profile for every characteristic definition. Race creation also makes a starter ethnicity, which is useful to inspect before creating additional populations. Use `ethnicity clone <source> <new name>` to copy a population within the same parent race. These commands require Junior Admin permission.

| Setting | What it means and what to decide |
| --- | --- |
| `name <name>` | Renames the population record. Ethnicity names must be unique. |
| `characteristic <definition> <profile>` | Chooses a profile whose target definition matches the selected characteristic. This changes the chargen options and generated value distribution for that characteristic. It does not directly alter the person's value after creation. Check every characteristic the parent race exposes. |
| `native <language\|none>` | Sets this ethnicity's optional native-language identity default. It grants no language skill. Native identity resolves from an applicant's explicit choice, then ethnicity, then culture, and finally the highest raw known language skill. |
| `nameculture <name culture\|none> [all\|male\|female\|neuter\|nb\|indeterminate]` | Assigns or clears a name-culture override for all genders or the selected gender. With `none`, the name picker falls back to the selected culture's name culture. The picker uses the ethnicity override before the culture default. |
| `group <name\|clear>` | Adds or clears the optional ethnic-group label. The list of suggested group names comes from other ethnicities of this race. The label organises records and carries no shared gameplay effect. Clear the subgroup before clearing its group. Clearing the group itself currently leaves any subgroup text on the record. |
| `subgroup <name\|clear>` | Adds or clears a subgroup under the current group. A group must exist before the command can set or clear a subgroup. |
| `bloodmodel <model\|clear>` | Selects the population blood model. It must use the same blood model as its parent race. If that race has no blood model, this ethnicity cannot use one. |
| `tempfloor <amount>` and `tempceiling <amount>` | Add modifiers to the character's tolerable temperature bounds. Include temperature units such as `5C` or `-9F`. These are offsets, not absolute climate settings. |
| `advice <which>` | Toggles an existing chargen-advice entry for this ethnicity. Advice is shown at its configured stage. |
| `cost <resource> <amount>` | Charges the specified chargen resource for selecting the ethnicity. Use `cost <resource> clear` to remove it. |
| `require <resource> <amount>` | Requires the applicant to have the balance, without spending it. This is an eligibility requirement rather than a price. |
| `desc` (also `description`) | Opens the text editor for the ethnicity's chargen blurb. This is the text applicants read about the population. It is not a separate in-world long description. |
| `availability <prog>` | Restricts the option with a Boolean prog accepting either `(Chargen)` or `(Chargen, Ethnicity)`. An unset/null prog means available, so a new ethnicity does not need a prog just to appear. A configured false result hides it. |

## Culture and upbringing

Culture records upbringing-related defaults. Use `culture list`, `culture show <name>`, `culture edit <name>`, `culture set ...` and `culture close`. `culture edit new <name>` creates a culture. Use `culture clone <source> <new name>` to build on an existing upbringing. The culture command requires Junior Admin permission.

| Setting | What it means and what to decide |
| --- | --- |
| `name <name>` | Changes the culture's unique display name. |
| `native <language\|none>` | Sets the optional native-language identity default. It grants no skill. An explicit applicant selection takes precedence, then an ethnicity default, then this culture default, then the highest raw known language skill. |
| `calendar <which>` | Assigns an existing calendar used for culturally formatted dates, including birthday handling. This is not a choice to create a new calendar. |
| `nameculture <which> [all\|male\|female\|neuter\|nb\|indeterminate]` | Assigns a name culture to every gender or one gender. The culture needs a name culture in each gender slot. Ethnicity can selectively override these defaults. `culture` does not accept `none` to clear a slot. |
| `male <word>`, `female <word>`, `neuter <word>`, `indeterminate <word>` | Sets the corresponding culture-specific person word used in presentation. The command has no separate non-binary word setting. These words do not set a character's gender or pronouns. |
| `availability <prog>` | Assigns a Boolean prog with one `Chargen` parameter to control whether applicants can choose the culture. New cultures use AlwaysFalse until this is set. |
| `skill <prog>` | Assigns a Number-returning prog with `(Chargen, Trait, Number)` parameters to compute culture-based starting skill values. The number input is the selected boost count for that skill. A fresh culture returns one for every skill until you set a different policy. Use this only for a deliberate cultural baseline. Role grants, skill-selection groups and learned languages are separate decisions. |
| `tempfloor <amount>` and `tempceiling <amount>` | Add temperature tolerance modifiers with explicit temperature units. |
| `advice <which>` | Toggles an existing chargen-advice entry for this culture. |
| `cost <resource> <amount>` | Charges a chargen resource. Use `cost <resource> clear` to remove the cost. |
| `require <resource> <amount>` | Requires an available balance without spending it. |
| `desc` | Edits the player-facing culture description. |

A new culture also begins with the first available calendar and a generic default name-culture assignment in all gender slots. Inspect and replace those defaults. The initial calendar is merely an existing calendar choice, not evidence that it suits the community. Check availability before trying the selection in chargen.

## Name cultures and name entry

A name culture contains named components such as birth name, surname, nickname or patronym; the minimum and maximum number of each component; applicant-facing prompts; a regex used when parsing a complete name; and display patterns. A culture assigns one for each gender. An ethnicity may override one or more assignments. The name picker first selects the ethnicity's scheme for the chosen gender, then falls back to the culture's scheme.

Name-culture commands and random-name commands require Admin permission. Use `nameculture list`, `nameculture show <name>`, `nameculture edit <name>`, `nameculture set ...`, and `nameculture close`. Create with `nameculture edit new <name>` or use `nameculture clone <source> <new name>`. A new scheme supplies one required `BirthName` element, default pattern placeholders, and a default regex. Always replace the default regex before using the scheme. Its current default pattern only accepts the literal letter `w` plus spaces and punctuation, so it rejects ordinary names. Review elements, patterns, regex and case options when editing an existing scheme.

| Command | Effect and design decision |
| --- | --- |
| `nameculture set element add <type>` | Adds one unique `NameUsage`, such as `BirthName`, `MiddleName`, `Surname`, `Nickname` or `Patronym`. New elements start with a minimum of 0 and a maximum of 1, with a standard label and chargen prompt. |
| `nameculture set element remove <type>` | Removes an element from the scheme. It does not rewrite display patterns or random-name profile dice and value entries. Update those references before using the revised scheme. |
| `nameculture set element <type> name <label>` | Sets the name applicants see for that component. |
| `nameculture set element <type> min <count>` | Sets the minimum component count, zero or greater. The command does not check that the current maximum is at least the new minimum. When increasing the minimum, set the maximum first, then the minimum. |
| `nameculture set element <type> max <count\|unlimited>` | Sets a maximum of at least one, or removes the maximum. It rejects finite values below the current minimum. |
| `nameculture set element <type> blurb` | Edits the applicant-facing prompt for that element. |
| `nameculture set regex <regex>` | Controls whole-name parsing when the game is given a whole name, including named captures for usages present in the pattern. Use lowercase group names such as `birthname` and `surname`. The whole-name parser looks them up in lowercase. Every element needs a named group, including optional elements. This regex does not determine the separate, sequential prompts used by the chargen NamePicker. |
| `nameculture set pattern <style> <pattern>` | Sets rendering for `GivenOnly`, `SimpleFull`, `FullName`, `Affectionate`, `SurnameOnly` or `FullWithNickname`. Refer to components with `$BirthName`, `$Surname`, and similar tokens. Conditional text can use `?Nickname[true text][false text]`. The default patterns only reference the original birth-name component, so update relevant styles when adding a surname or other component. |
| `nameculture set preservecase` | Toggles preservation of entered/generated name case. When enabled, random-profile names are matched case-sensitively. |
| `nameculture set exactgender` | Toggles exact random-profile gender matching. When off, NonBinary and Indeterminate profiles can match any selected gender. When on, profile gender must match exactly. |
| `nameculture set typographic` | Toggles acceptance of curly apostrophes and Unicode hyphens, alongside the ordinary punctuation validation. |

Other available usages include `Dimunative` (the engine spelling), `AdultName`, `ChildName`, `Matronym`, `FamilyGroupName`, `GenerationName`, `RegnalName`, `SacredName`, `Toponym`, `OwnerName`, `Title`, `Affiliation`, `ClanName` and `EarnedName`. A `Title` or `Affiliation` is a piece of a name. Grant formal authority or membership through a role and clan.

The chargen NamePicker walks the element list and asks for each element's configured number of values. It enforces min/max, ordinary character validity, per-component duplicates, and any supplied-value restrictions present in the stored definition. It does not assemble a whole typed name and run the regex. The whole-name parser reads a complete supplied name through the regex and maps its named capture groups back to `NameUsage`. The two paths solve related but separate problems. Patterns determine how an accepted name is displayed in each context.

While editing `NamePicker`, `chargen set allowunicode` toggles whether applicants can enter Unicode letters. That setting governs manual entry independently of typographic punctuation and the whole-name regex. The NamePicker can also offer extra name cultures from its optional `stylesprog`, a FutureProg returning a name-culture collection from one Chargen. When more than one scheme is available, the applicant can select with `/style <number>`. Changing style restarts name entry. This is configured on the NamePicker storyboard, not the name-culture record.

The runtime also understands structured name-entry data stored in definitions, including allowed/suggested values, parent-dependent values, confirmations and distinctness constraints. The current `nameculture` editor does not expose commands for every such field or a toggle for structured entry. Treat them as existing-definition/runtime features unless a specific screen or builder command exposes them.

## Random-name profiles

Random-name profiles contain a name culture, a target gender, dice expressions that choose counts for each element, and weighted values for those elements. They supply NPC/storyteller name generation and, when the optional prog permits, chargen suggestions for the corresponding component.

Use `randomname list [<name culture>]`, `randomname show <profile>`, `randomname edit <profile>`, `randomname set ...` and `randomname close`. Create with `randomname edit new <name culture> <profile name> <gender>`. Alternatively, `randomname clone <source> <new name>` gives an existing pool a second record to edit. Each profile is tied to one naming culture, and its element names must exist there. In edit mode:

- `dice <element> <dice expression>` chooses how many values of the element a generated name receives. The expression must be a valid dice expression.
- `<element> add <name> [<weight>]` adds a source value. The weight must be a positive integer and defaults to 100.
- `<element> remove <name>` removes a source value.
- `<element> weight <name> <weight>` changes a value's positive integer weight.
- `chargen <prog>` sets a Boolean `(Chargen)` prog controlling use for chargen suggestions.
- `name <name>` and `gender <gender>` edit profile identity and gender.

Use `randomname show` to inspect readiness. Every element with a positive generated count needs suitable names. The generator does not reuse the same value twice within one generated full name. If exact gender matching is off, an Indeterminate profile acts as a wildcard. If it is on, create a profile for each gender that needs suggestions. When you add a name-culture element, edit each existing profile to set its dice count and values. Use the same naming scheme and compatible profiles on generated NPC templates so the harbourmaster and visiting pilots follow the same conventions as local PCs. [Building NPCs](NPC_Builder_Guide.md)

## Roles, entitlements and starting places

A **role** describes an opportunity at the start of play. The role picker offers at most one choice in each category: `Class`, `Subclass`, `Profession`, `Family`, `Childhood`, `Education`, `ImmediatePast` and `Story`. The separate starting-location screen supplies the `StartingLocation` role. Use Profession for someone trained to pilot ferries, Family for the household they belong to, and Story for a premise the player agrees to play. If they should receive a recurring wage or work shifts, build that employment separately. [Employment hosts](Employment_Hosts_Builder_Guide.md)

Use `role list Profession`, `role show <role>`, `role new <type> <name>`, `role edit <role>`, `role clone <source> <new name>` and `role close`. Creation opens the editor with an always-false availability prog. Compare `role show` for the source and its clone before opening the new post. A clone copies grants such as clan membership and trait adjustments, but omits merit grants, required resource balances and minimum approval and viewing authorities. Reapply the intended merits and authorities, and express a required balance in the eligibility prog.

| Role settings after `role set` | How to use them |
| --- | --- |
| `name`, `blurb`, `type` | Name the choice and explain what the player agrees to do and receives. Two professions compete for the same slot. A profession and a family can both contribute benefits. |
| `prog <prog>` or `prog clear` | A Boolean prog accepting a chargen controls availability. Clear it for an unrestricted role. Only read decisions made by earlier stages. |
| `view <level>`, `permission <level>`, `approvers <account> ...`, `approvers clear` | The first controls builder visibility. The second sets minimum review authority. A list of named accounts allows **one of them** to review that role. A reviewer below High Admin must satisfy the list for each selected role that has one. Set the authority above automatic approval if the named review matters. |
| `clan <clan> rank <rank>`, `paygrade <paygrade>`, `appointment <appointment>`, `remove` | Each subcommand follows `clan <clan>`. Set rank first to establish membership. Choose a paygrade available to the rank and toggle eligible appointments. Removing the membership here changes the role's future grant. [Clans and town government](Clan_Builder_Guide.md) |
| `trait <trait name> <amount> [boost]`, `merit <merit>` | A positive trait adjustment can supply a missing skill at character creation. Append `boost` to modify it only for someone who already has it. An amount of zero removes the adjustment. The merit command toggles a merit or flaw. |
| `alive <n>`, `total <n>`, `expired`, `cost <amount> <resource>`, `currency <currency> <amount>` | These controls have important limitations in the current engine. The picker checks availability without enforcing the stored population limits or expired flag. To withdraw a role, give it an always-false availability prog. The role cost editor takes amount first but edits a temporary cost collection, so a new price does not stick. The currency field saves, but character creation does not issue those coins. The ferry example uses an eligibility prog for the account threshold and an explicit first-login payment. |

The **RolePicker** storyboard controls the presentation of the categories. `chargen set Profession name <label>` changes the caption, `Profession blurb` edits its introduction and `Profession canskip` toggles whether an applicant may select none. `swap <type1> <type2>` changes their order within that screen. Categories with no eligible roles are skipped. This is separate from ordering the role screen among the other chargen stages.

Choose how training reaches play:

- **Grant a mandatory skill** through the skills screen's `freeskills` prog if later chargen screens need to see it. A role trait adjustment takes effect when the character is created. Use it for a starting adjustment or acquisition when those earlier screens do not need the skill.
- **Offer bounded choices** with `chargenskillgroup`. Set eligible skill members, minimum and maximum picks, and a compiled Boolean(chargen) eligibility prog. `existing newonly` requires new skills. Validate and enable the group. Enabled groups are resolved by the supported skill screens; there is no separate attachment to a profession or screen. The eligibility prog makes the link. Mandatory skills resolve first, group choices follow, and ordinary picks come last.
- **Suggest a skill** on `SkillPicker` with `chargen set suggestedskills <prog>` or clear it with `none`. The prog returns a trait collection for the application. Accepted suggestions count towards the normal pick limit and costs. A player can remove them while keeping the profession.

### Starting locations

A **starting place** joins three records: a StartingLocation role with a chargen eligibility prog, a room, and an entry on `StartingLocationPicker`. Create a distinct role per entry so first login can identify the correct commencement prog. The entry's optional prog takes the **live character** and returns void. Edit the storyboard and use `location add <name> <starting-role> <room> [commencement-prog]`, then enter the location blurb. Use `location rename`, `role`, `room`, `blurb`, `commence` or `remove` after `location` to maintain it. `skipone` controls automatic selection when exactly one place is available. Leave ordinary applicants at least one usable starting place.

## Build the application journey

A **stage** is a decision such as choosing characteristics. Its **storyboard** is the installed screen implementation and its settings. An **application** holds one applicant's answers. A FutureProg that runs on an application can read answers already given. At this point there is no live character to dress or move.

### Set order, dependencies and application modes

Start with `chargen overview`. It shows the order in *this game*. Use `chargen show <storyboard ID or name>` and `chargen types` to compare the installed screens with alternatives. For example:

```text
chargen typehelp RerollableCharacteristicPicker
chargen show NamePicker
chargen edit NamePicker
chargen show
chargen close
```

Use `chargen set ...` while editing. To replace a screen, use `chargen changetype SelectCharacteristics RerollableCharacteristicPicker`, inspect the proposal, then `accept`. You are now editing the replacement. Record the old settings first. A different implementation can drop limits, blurbs or progs that its predecessor understood. When rebuilding a live graph, arrange a quiet period and have an administrator use `wizlock chargen` while applicants are out of it.

Order determines **what comes next**. A dependency determines **which answers must already exist and which later decisions need to be made again**. In the dependent screen, `chargen set dependency <earlier-stage>` toggles the link. Use `chargen reorder <stage> <after-stage>` to move a stage directly after another. For example, put birthday after culture so the date can use the upbringing's calendar. If a profession supplies suggested skills, put role selection before skills and declare the dependency. When an applicant reopens an earlier choice from the menu, completed dependent stages can be reset recursively. A missing dependency can leave an old answer in place after the premise for it changes.

The special-application screen distinguishes **normal**, **simple** and **special** applications. Its `autofirst` toggle puts first applications into short form automatically. Give a simple applicant a complete starting character with fewer decisions. Use special status in an availability prog when an exceptional choice is meant to open only on that route. Role approval authority and the submit screen's automatic approval threshold govern review. Merely labelling an application special does not implement a particular staffing policy.

### Choose what the applicant decides

| Decision | Screen and useful controls | Design question |
| --- | --- | --- |
| Race, culture, ethnicity | Edit the corresponding picker. `blurb` introduces the choice and `skipone` can pass through a single eligible option. Race and culture also have `blanks` for showing gaps where choices are unavailable. | Would a player benefit from reading the only choice? If there are no choices, inspect the heritage records and availability rules. |
| Gender, handedness, age and measurements | Race supplies the possible genders and handedness. `AgePicker` uses `minage` and `maxage` progs and the culture's calendar for birthdays. Height and weight pickers use `min` and `max` progs, and accept measurements in the game's units. | A race's age categories, a chargen age limit and a date in a cultural calendar answer three different questions. A weight bound that uses height needs height first. [Building time and calendars](Time_Skies_and_Weather_Builder_Guide.md) |
| Attributes | `AttributeOrderer` lets applicants rank the race's attributes and rolls them using its dice and caps. `AttributePointBuy` gives the builder controls for base values (`attribute`), free boosts, minuses, maximum boosts, paid extras, a cost expression and an account resource. | Is being strong a priority the player expresses, or a value they buy deliberately? Configure the point-buy budget and caps together. |
| Skills and knowledges | `SkillPicker` offers editable choices. `SkillCostPicker` adds extra-pick and boost pricing. Skippers can supply a configured allocation. `KnowledgePickerBySkill` offers eligible knowledges for each selected skill. Its `free` prog returns a knowledge collection for the application. Its `picks` setting takes a **number-returning prog** accepting the application, optionally also the current skill. | Boat Handling measures ability. Knowledge of a particular ferry procedure can be a separate entitlement. For a selectable knowledge, set it learnable at chargen and make its chargen prog accept the intended skill and applicant. [Skills, checks and progression](Skills_Checks_and_Progression_Builder_Guide.md) |
| Accents | On `AccentPicker`, `native` toggles the native-language question, `picks` sets the allowance per language, and `extras <amount> <resource>` or `extras none` controls paid additions. | Grant the spoken language's skill first. Choose an accent among the languages the applicant actually knows. |
| Individual merits and flaws | `MeritPicker` can `balance` numbers of merits and flaws and set a total `max`. `QuirkPicker` presents a combined choice with its own maximum. On the merit itself, `cost <resource> <amount>` sets the cost, zero removes it and `cost <resource> spend` toggles spend versus required balance. | A pilot's individual aptitude belongs here. A physical advantage intrinsic to every Riverfolk body belongs on the race. For a merit that provisions another body, see [multiple body forms](Multiple_Body_Forms_and_Instances_Builder_Guide.md). |
| Notes, scars and prostheses | Use the notes and disfigurement pickers for applicant decisions or automatic alternatives to grant selected outcomes. On `NotePicker`, `note add <name> <prog>` creates a conditional prompt. | Ask for information staff will actually use. A senior pilot might need a previous command described; everyone entering town need not write a service history. |

### Characteristics and the descriptions other people see

An **attribute** such as Strength participates in checks. A **characteristic** such as eye colour has a value rendered in prose. A **profile** supplies the pool of possible values. An ethnicity assigns a profile to each applicable definition on its parent race. A description **pattern** decides how to put those values into a sentence.

1. Inspect the catalogue with `characteristic definition list`, `characteristic value list <definition>` and `characteristic profile list`. Reuse the installed definition when two peoples have the same feature. To author a distinct feature, `characteristic definition add <name> <type>` creates it. `addchild` specialises an existing definition, and `addpart` ties a feature to a bodypart shape. The definition's `pattern` setting is the expression matching its token in descriptions.
2. Add values with `characteristic value add <definition> <name> [type-specific arguments]`. Create a pool with `characteristic profile new <definition> <type> <name>`. Use **standard** for a curated list, **all** for every matching value, **compound** to combine existing profiles, or **weighted** to change the chances of individual values in a random roll. In a weighted pool, `characteristic profile set <profile> value <value> <weight>` sets relative weight. Weights of 3 and 1 mean a three-to-one ratio. A rare selectable value is still a valid manual choice.
3. Assign the profile with `ethnicity set characteristic <definition> <profile>`. Use `RerollableCharacteristicPicker` when players should start with a population-appropriate appearance and adjust it. They can `lock <characteristic>`, `basiclock <characteristic>` to keep a broad value while rolling variants, or `unlock <characteristic>`. The screen's `intro` toggles the separate introduction. A sequential picker suits a short list of important choices. `SimpleCharacteristicPicker` gives an overview with customisation. Races with no definitions can pass through this stage.
4. Use `description edit new sdesc <eligibility-prog>` for a short description or `fdesc` for a full look. `description set pattern` opens the editor with markup help, `prog` changes eligibility, and `weight` controls random selection. With a compatible eye-colour definition, the full-description fragment `$eyecolour[@ eyes][eyes hidden behind $]` gives a visible-value branch and a covered branch. Use the tokens from your own definitions. Each pattern needs enough information to read naturally for every eligible body, including covered or missing features.
5. On `DescriptionPicker`, `sdesc` and `desc` edit the applicant instructions, and `patterns` and `custom` control the offered methods. For every applicant to write their own short and full text, select `CustomDescriptionPicker`. With no characteristic definitions on the race, the ordinary picker also uses custom entry. A personal name, a short description other characters target, and the full paragraph they see on looking are three separate pieces of content.

### Skipping optional selection screens

An automatic screen still runs and can grant content. `chargen changetype SelectAccents AccentSkipper`, for example, replaces the accent questions with automatic selection. Configure these while editing the corresponding storyboard. Each optional prog below takes one `chargen` argument. Use `none` to clear it.

| Screen | Settings | Result |
| --- | --- | --- |
| `AccentSkipper` | `accents <prog>` returning an accent collection | Clears old accents, grants sole available accents and applicable scripted accents for known languages, then resolves outstanding defaults. A native-language selection survives only while the language is known. |
| `QuirkSkipper` | `quirks <prog>` returning a merit collection | Rebuilds the application's selected merits and flaws from the returned character merits. With no prog, this selection is empty. |
| `NoteSkipper` | Automatic | Preserves existing notes and permits completing the stage with none. Switching back to `NotePicker` restores its saved prompts. |
| `DisfigurementSkipper` | `missingbodyparts`, `prostheses`, `scars`, `tattoos`, each taking a void prog | Preserves prior selections and runs the hooks in that order. `specialpicker` toggles interactive choices for special applications. Configure those choices with the picker settings, including `allowmissing`, `allowscars` and `allowtattoos`. |

Automatic grants can bypass the manual choice limits, availability rules and chargen costs. Put the eligibility rule in the grant prog if it matters there. Use `AddChargenMissingBodypart`, `AddChargenProsthesis`, `AddChargenScar` and `AddChargenTattoo` on the application. A missing part must be severable and non-vital, a prosthesis must fit a missing root, and scars and tattoos require a present external part. Tattoo templates can also require custom text slots. The helpers report whether the addition succeeded. Keep other side effects repeatable because the applicant can return to a stage. [Health system builder guide](Health_System_Builder_Guide.md) · [FutureProg for builders](FutureProg_Builder_Guide.md)

For a character merit, `merit types` and `merit typehelp <type>` show the available behaviours. `merit edit new <type> <name>` creates a record. The merit's `chargenprog` controls who can choose it, `applies` controls when its effect operates, `scope body|character` sets which body or person carries it, and `blurb` and `description` explain it in chargen and play. Set `parent <combo merit>` to assign it to a combination, or `parent none` to detach. For a people with an intrinsic advantage, use a false selection prog if other applicants should never purchase it as a personal merit.

## Account resources, advice and approval

**Account resources** express an admission policy. **Currency** buys things in the game. A senior pilot might need five account points to qualify and still board the ferry carrying only their starting wages. When setting up a paid option, decide where to charge it once. The application aggregates costs from its screens, including heritage, roles and optional boosts. A required balance checks that the applicant has the resource. A spendable cost is included in the total and deducted on approval.

Use `chargenresource list` and `chargenresource show <name>` before adding a new resource. `chargenresource edit new <type> <name> <plural> <alias>` creates one. Use **simple** for a staff-awarded resource, **regenerating** for awards based on elapsed time while playing, **playtime** for increments tied to accumulated playtime, or **realtime** for a game-wide timed award. The simplest design for a scarce senior post is a staff-awarded resource or an existing account resource with the right policy. While editing, `name`, `plural`, `alias` and `score` control presentation. `permission`, `permissiontime`, `time` and `max` configure manual award authority, wait-time overrides, intervals and award size. The editor also exposes `control`, `resource` and `formula` for automatic-award eligibility and cap policies; accrual behavior depends on the resource type. Staff use `award <account> <resource> [amount]` or `deduct <account> <resource> [amount]` and enter the reason in the note editor.

**Advice** appears at a specified stage for applicants who have selected a linked race, ethnicity, culture or role. Create it with `chargenadvice edit new <stage> <title>`. Use `chargenadvice set text` to write it, `set stage` to move it, `set title` to rename it, and `set prog <boolean-chargen-prog>` for a further condition. `set race`, `set ethnicity`, `set culture` and `set role` toggle links to existing records. For a mixed-upbringing applicant, place the port's naming advice on the name screen after culture selection. A short explanation such as “Port families give a household name when they sign the harbour roll” helps the player make the next decision. A general history of the port belongs elsewhere.

**Submission** checks that the application is complete and affordable and the account has room for another active or submitted character. The player sees a review and types `yes` to submit. Read `chargen show Submit` before designing restricted roles. On that storyboard, `chargen set approve <level>` sets the highest minimum approval authority that qualifies for automatic approval. Put roles requiring human review above it. For manual review:

```text
applications
application view <application ID>
application review <application ID>
```

The queue shows the required authority. The highest minimum among the selected roles applies. Named role approvers can further restrict who may review it. The reviewer sees the application, then uses `accept` or `decline` and completes the comments editor. Declining returns the application to the applicant with the comment and prior rejection history. Give a concrete correction the player can act on. To inspect an application outside the review queue, senior builders can use `chargen list InProgress`, `chargen list Submitted`, `chargen list *<accountname>` and `chargen view <id>`.

Approval creates the character. First login is where the starting location's commencement prog receives that character. Keep eligibility and advice progs free of payouts or equipment grants: the engine can ask those questions multiple times before the character exists.

## Worked examples

### Establish one Riverfolk race

First choose a playable human-like parent whose anatomy, breathing, movement and health suit the setting. Use `race show <parent>` to find its body prototype, and check the parent's characteristics have profiles. Riverfolk use that same body. Both villages can inherit one set of physical rules. A builder can change their naming, languages or eligibility for a profession independently.

```text
race edit new Riverfolk <body-prototype-id> <parent-race-id>
race set desc
race show Riverfolk
ethnicity show Riverfolk
```

In the editor, describe the shared people and what a player can expect from the body. Reserve the villages' stories for their culture blurbs. The new race starts hidden. Create **OpenRiverHeritage(app chargen) → Boolean** with the single line `return true`. Keep it for the new race and open community options after the naming and skill choices are ready.

### Give Riverfolk their shared stamina

The example uses a fictional merit name. Use `merit types` and `merit typehelp "Stamina Capacity"` to confirm the type is installed and view its current commands. Create the merit and set its body scope, bonus, and multiplier:

```text
merit edit new "Stamina Capacity" "Riverfolk Stamina"
merit set scope body
merit set chargenprog AlwaysFalse
merit set bonus 5
merit set multiplier 100%
merit show
merit close
race edit "Riverfolk"
race set merit "Riverfolk Stamina"
race show "Riverfolk"
race close
```

The bonus is a finite addition to maximum stamina and can be negative. The multiplier is finite, nonnegative, and applied after additions. A multiplier of `100%` leaves that result unchanged. This merit changes maximum stamina only. It does not change recovery or refill current stamina.

### Build Reedbank and Estuary

This example uses an existing Riverfolk race whose body and race section already supply the appropriate characteristic definitions and eligible profiles. Confirm those profiles first, because an ethnicity cannot be created until all of its race's characteristics have applicable profiles. Use your installed calendars and languages. In this example the compiled helper OpenRiverHeritage takes one chargen argument and returns Boolean, with the body `return true`. The culture availability progs must return Boolean and accept one Chargen. The free-skills policy in the pilot example teaches the two languages.

Create a one-component Reedbank scheme. A new name culture starts with required BirthName, so correct its regex and all display patterns before use:

```text
nameculture edit new "Reedbank Names"
nameculture set regex "^(?<birthname>[\p{L}'-]+)$"
nameculture set pattern GivenOnly $BirthName
nameculture set pattern SimpleFull $BirthName
nameculture set pattern FullName $BirthName
nameculture set pattern Affectionate $BirthName
nameculture set pattern SurnameOnly $BirthName
nameculture set pattern FullWithNickname $BirthName
nameculture show
nameculture close
```

Create the Estuary scheme by adding one required surname. Set max before min because the minimum command does not guard against temporarily exceeding the maximum:

```text
nameculture edit new "Estuary Names"
nameculture set element add Surname
nameculture set element Surname name "Family name"
nameculture set element Surname max 1
nameculture set element Surname min 1
nameculture set regex "^(?<birthname>[\p{L}'-]+) (?<surname>[\p{L}'-]+)$"
nameculture set pattern GivenOnly $BirthName
nameculture set pattern SimpleFull "$BirthName $Surname"
nameculture set pattern FullName "$BirthName $Surname"
nameculture set pattern Affectionate $BirthName
nameculture set pattern SurnameOnly $Surname
nameculture set pattern FullWithNickname "$BirthName $Surname"
nameculture show
nameculture close
```

Create the upbringing cultures and assign their date, language-identity and naming defaults. Substitute existing chargen eligibility progs and calendars:

```text
culture edit new "Reedbank"
culture set desc
culture set availability OpenRiverHeritage
culture set calendar <River-calendar>
culture set native "River Tongue"
culture set nameculture "Reedbank Names" all
culture show
culture close

culture edit new "Estuary"
culture set desc
culture set availability OpenRiverHeritage
culture set calendar <Port-calendar>
culture set native "Harbour Speech"
culture set nameculture "Estuary Names" all
culture show
culture close
```

Use the ethnicity that the race editor created for you. Reedbank eyes are usually grey and occasionally brown. Find the installed eye-colour definition and two values belonging to it. Substitute their IDs below. The optional group label lets related populations be browsed together. Leave its native-language default unset so a selected upbringing supplies the default language identity:

```text
characteristic value list "Eye Colour"
characteristic profile new "Eye Colour" weighted Reedbank Eyes
characteristic profile set "Reedbank Eyes" value <grey-value-id> 3
characteristic profile set "Reedbank Eyes" value <brown-value-id> 1
ethnicity edit Riverfolk
ethnicity set name Reedbank Riverfolk
ethnicity set group "Riverfolk"
ethnicity set subgroup "Reedbank"
ethnicity set native none
ethnicity set characteristic "Eye Colour" "Reedbank Eyes"
ethnicity set nameculture none all
ethnicity show
```

On a random draw from Reedbank Eyes, grey is three times as likely as brown. Inspect the other definitions on the renamed ethnicity. Keep each existing profile that fits, and change any that need a local pool. `nameculture none all` clears overrides so the selected culture chooses the scheme. If a population needs its own naming exception, set only those gender slots to a specific name culture. Finish with `ethnicity set desc` and enter the applicant-facing chargen blurb in the editor. After saving the editor, use:

```text
ethnicity show
ethnicity close
```

Create one weighted source profile for each naming scheme. This representative Indeterminate profile is a wildcard for any gender while exact matching remains off. Add gender-specific profiles if the community needs distinct name pools, or enable exact matching and populate every gender used by characters:

```text
randomname edit new "Reedbank Names" "Reedbank Names" Indeterminate
randomname set dice BirthName 1
randomname set BirthName add Mara 100
randomname set BirthName add Teren 100
randomname set chargen OpenRiverHeritage
randomname show
randomname close

randomname edit new "Estuary Names" "Estuary Names" Indeterminate
randomname set dice BirthName 1
randomname set dice Surname 1
randomname set BirthName add Anja 100
randomname set BirthName add Nerin 100
randomname set Surname add Vale 100
randomname set Surname add Lockwater 50
randomname set chargen OpenRiverHeritage
randomname show
randomname close
```

Now a character can choose Reedbank ethnicity and Estuary culture. The character keeps Riverfolk anatomy and Reedbank characteristic options, uses the Estuary calendar and name scheme, and can have Harbour Speech as their native language once the skills policy grants it. The culture setting does not itself grant Harbour Speech. The relevant chargen skill group, role or another explicit learning path must grant language knowledge. With Reedbank upbringing instead, the same ethnicity resolves River Tongue identity and the one-component name scheme.

Once the population profiles, roles, screens and skills are in place, use `race edit Riverfolk` and `race set chargen OpenRiverHeritage` to open the race to applicants. Every visible choice needs a finished blurb and a valid set of profiles.

### Give ferry pilots training and a place in the guild

The landing has a Ferrymen's Guild with Recruit and Senior ranks. Its pilots use Boat Handling. An apprentice chooses two of Ropework, Mending and Navigation. Weather Lore is useful enough to suggest, but a pilot who learned something else can replace it. Install or substitute actual skills under those names. The skill linked to a language may also have a different name from the language.

Create the ordinary profession and write its blurb when the editor opens. For example: “You have crewed the estuary ferries through a season of tides. The guild admits you as a recruit. Choose two river trades, then collect your work coat and first allowance at the landing.”

```text
role new Profession Ferry Pilot
role set blurb
role set clan "Ferrymen's Guild" rank Recruit
role set prog clear
role show
```

For the senior profession, create **CanStartSeniorPilot(app chargen) → Boolean** with the body `return getresource(@app, "Role Points") >= 5`. Substitute the name or alias of your actual account resource. A balance of five qualifies the player and remains available after approval. Then clone the role, write the senior blurb, and select an actual review level above the submit storyboard's threshold:

```text
role clone "Ferry Pilot" Senior Ferry Pilot
role set blurb
role set clan "Ferrymen's Guild" rank Senior
role set prog CanStartSeniorPilot
role set permission <review level>
role set approvers <guild reviewer account>
role show
role close
```

For training, compile **IsFerryPilot(app chargen) → Boolean**. Both roles qualify. The same result can also control access to the guild landing.

```futureprog
return @app.roles.any(r, @r.name == "Ferry Pilot" or @r.name == "Senior Ferry Pilot")
```

On a world with an existing free-skills policy, extend its result just before its final return. The example below gives the complete body for a world whose free grants consist of these community languages and pilot training. **RiverStartingSkills(app chargen) → trait collection** expects culture and profession to have been selected already. Resolve all three skill names before attaching it.

```futureprog
var skills as trait collection
if (@app.culture.name == "Reedbank")
  additem skills totrait("River Tongue")
end if
if (@app.culture.name == "Estuary")
  additem skills totrait("Harbour Speech")
end if
if (@IsFerryPilot(@app))
  additem skills totrait("Boat Handling")
end if
return @skills
```

For **PilotSuggestions(app chargen) → trait collection**, use:

```futureprog
var skills as trait collection
if (@IsFerryPilot(@app))
  additem skills totrait("Weather Lore")
end if
return @skills
```

For a world using `SkillPicker`, the setup is:

```text
chargen edit SkillPicker
chargen set freeskills RiverStartingSkills
chargen set suggestedskills PilotSuggestions
chargen close
chargenskillgroup create "River Trades"
chargenskillgroup set description
chargenskillgroup set member add Ropework
chargenskillgroup set member add Mending
chargenskillgroup set member add Navigation
chargenskillgroup set picks 2
chargenskillgroup set eligibility IsFerryPilot
chargenskillgroup set existing newonly
chargenskillgroup validate "River Trades"
chargenskillgroup set enabled yes
```

Enter “Choose two trades you learned aboard the estuary ferries” in the group description editor. The ordinary skill picker still needs its own pick allowance. With at least one ordinary pick available, the pilot starts with Boat Handling, picks two river trades, and can keep Weather Lore or replace it. A villager who selects a different profession still knows their community language. If another existing grant already supplies two of the three trades, the `newonly` group needs more candidates or a different overlap policy.

### Bring the pilot through review to the ferry landing

Build a coat the pilot's body can wear and an outfit template with one entry. Create a numeric **character** register called `FerryIssue` with its normal zero default. The guild issues ten base units of an installed currency named Harbour Coin in denominations that can make that amount. Substitute your game's currency and the current wearable prototype.

```text
outfittemplate new FerryWorkKit
outfittemplate set item add coat <work-coat prototype> worn
outfittemplate show FerryWorkKit
register character FerryIssue number
```

Compile **IssueFerryKit(ch character) → void**. Run it on first login at the guild landing. The register records an issue begun before creating anything, so a partial issue stays visible to staff and a repeated call cannot pay twice.

```futureprog
if (isnull(@ch.location) or getregister(@ch, "FerryIssue") != 0)
  return
end if
if (not(@ch.roles.any(r, @r.name == "Ferry Pilot" or @r.name == "Senior Ferry Pilot")))
  return
end if
var kit = tooutfittemplate("FerryWorkKit")
var coin = tocurrency("Harbour Coin")
if (isnull(@kit) or isnull(@coin))
  send(@ch, "The clerk cannot complete your issue. Please ask staff at the landing.")
  return
end if
setregister @ch "FerryIssue" 1
var issued = loadoutfittemplate(@kit, @ch)
if (isnull(@issued))
  send(@ch, "The clerk has held your issue for staff to inspect.")
  return
end if
if (givecurrency(@coin, 10, @ch, true))
  setregister @ch "FerryIssue" 2
  send(@ch, "The clerk enters your coat and first allowance in the guild book.")
end if
```

The outfit loader assigns ownership and attempts the requested placement. A coat that cannot be worn can land in the room. The currency function can likewise place a new pile at the character's feet if they cannot take it. If the register stays at 1, inspect the character and the landing, supply only what is missing, then mark the issue complete. Keep the role's currency field empty when using this stipend. [Starting outfits, ownership and partial issues](FutureProg_Builder_Guide.md#issue-a-starting-outfit)

Create the location role and its entry. Use the landing room's real ID or unique name. The last argument of `location add` is the commencement prog, and the room argument is required. The location role's Boolean prog determines who sees the landing.

```text
role new StartingLocation Guild Landing Start
role set prog IsFerryPilot
role close
chargen edit StartingLocationPicker
chargen set location add "Ferry Landing" "Guild Landing Start" <landing-room-id> IssueFerryKit
chargen show
chargen close
```

Enter a blurb describing the gangway, the clerk and the first shift. For an existing landing use `chargen set location commence "Ferry Landing" IssueFerryKit` while editing the picker. If a commencement prog already welcomes entrants, call the issue prog from it to retain both actions. Keep a public starting place for non-pilots.

**The applicant's journey.** Anja chooses Riverfolk, Reedbank ancestry, Estuary upbringing and Ferry Pilot. She keeps an eye-colour roll from her ancestry, enters **Anja Vale** under the port naming rules, and reckons her birthday by its calendar. Harbour Speech comes from the free-skills policy. She chooses Ropework and Navigation in River Trades and exchanges Weather Lore for an eligible ordinary skill. She selects the ferry landing. At submission she can review her name, description, choices and the account cost. If she goes back to change her upbringing, the dependent name, birthday and skills choices should reopen so the new upbringing can take effect.

**The reviewer's decision.** For the senior variant, an account below the five-point threshold can still select the ordinary pilot. An eligible account sees Senior Ferry Pilot. An authorised reviewer uses `applications`, then `application review <id>` and reads the selected profession, skills, costs and notes. If the application describes a newly apprenticed recruit in the senior post, `decline` with a comment such as “Choose Ferry Pilot for a first season, or tell us which ferry you previously commanded.” The applicant can amend and resubmit. On approval the account retains those five qualifying points, assuming no other paid selections.

**First login.** Anja appears at the landing with the Recruit rank. Her work coat belongs to her, her allowance is physical coin, and she knows the language she can actually speak. The senior pilot starts with the senior rank. An ordinary Estuary resident shares the calendar and language, and can have the same name and eye colour, without inheriting a guild rank or coat.

## When the result surprises the builder

| What the builder sees | Where to look first |
| --- | --- |
| A race exists in the editor but no applicant can choose it | Inspect its availability prog, account requirements and the stage order. Then check there is an eligible ethnicity with complete characteristic profiles. |
| A port-raised applicant gets upriver names | Inspect the ethnicity's name-culture override for that gender, followed by the culture and the name picker's optional styles prog. Changing a random name pool changes suggestions and NPC output, not the selected convention. |
| The right native language is configured but missing in play | Inspect the skills actually granted at chargen. A heritage default resolves identity among languages already known. |
| The pilot has a navigation adjustment in play but cannot pick a navigation knowledge in chargen | A role adjustment can arrive after the knowledge screen. Grant the prerequisite skill before knowledge selection, then check the knowledge's chargen eligibility and pick allowance. |
| The group appears but the applicant cannot finish it | Check pick bounds, already-owned skills, `newonly`, any per-member prog and the candidates left for this particular applicant. Reopen via the group screen to change protected group picks. |
| A role vanishes when the player changes upbringing, or survives when it should disappear | Check what the role prog reads and whether those choices precede the role stage. Inspect dependencies before changing the prog. |
| The landing appears but the coat never arrives | Check the selected **location role**, the room and that entry's commencement prog. Read the issue register, inspect the named template and current prototype, then inspect worn items, inventory and the ground. Role membership alone does not call a commencement prog. |
