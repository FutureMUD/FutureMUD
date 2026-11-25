---
slug: npcs-and-ai
category: NPCs and AI
summary: Build named characters and variable populations, equip and approve them, and choose who directs their actions. Staff a town gate, establish a grazing herd, employ a buyer, train a riding pony and place an observatory guardian.
---

# Building NPCs: characters, populations and behaviour

Start with what a player should encounter. Is the ferryman the same person each morning? Does the town need four watch members regardless of which four are on duty? Should the deer in a valley graze independently or flee together? Those questions lead to different builder objects.

Read about [defining a character](#define-a-character) or use the [behaviour index](#behaviour-index) to find an AI for a particular job. The [east gate watch](#worked-example-staff-the-east-gate) and [water meadow herd](#worked-example-deer-on-the-water-meadows) take those systems through a complete build.

## Which editor to use

| Question | Object | Where to work |
| --- | --- | --- |
| Who is created? | An **NPC template** defines a starting character or the rules for generating one. | `npc list`, `npc show <template>`, `npc edit <template>` |
| Which person am I dealing with? | A **live NPC** has a character ID, body, belongings, relationships and location. Two copies of one template are two people. | `npc instances <template>`, then inspect the character |
| What can that NPC do of their own accord? | An **AI definition** supplies reusable behaviour. Both a template and a particular live character can have AI attachments. | `ai types`, `ai typehelp <type>`, `ai show <definition>`, `ai npclist <definition>` |
| How many should live in this area? | A **spawner** monitors a population and creates replacements. | `npcspawner list`, `npcspawner show <name>` |
| Who decides where a herd or pack goes? | A **group AI template** defines the policy. A **live group** holds its members, roles, alertness and current activity. | `gait list`, `gait show <template>`, `groups`, `group show <group>` |
| Who gives this person a job or powers of office? | A shop, employment host, clan, legal authority or other institution sets the terms. | Inspect that institution as well as the NPC. |

Junior Admins can use `npc` and manage live groups. Editing individual AIs, skill packages and spawners requires Admin. Editing group templates with `gait` requires Senior Admin. In the examples, substitute your world's names or IDs for text in angle brackets. Use quotes for a multiword name where a command expects a single argument.

**Who gets the new uniform?** Change an outfit template linked to the watch and the next recruit loads with the revised equipment. The guard at the gate still owns the coat they were issued. To give that guard a new one, issue the item in play. Change the gate-opening policy on the shared AI, on the other hand, and the gatekeeper already using it can apply that rule to the next request. This distinction matters whenever a builder edits a shop employee, a mount or an entire population.

## Define a character

Use `npc edit new simple` for a named individual whose starting features you choose yourself. Use `npc edit new variable` for a population with authored variation. A simple template can produce any number of copies. Naming it after one official does nothing to enforce uniqueness. Use `npc clone <approved-template> [new name]` to start from an existing template of the right kind. `npc make <character>` can take a PC as the starting point for a simple template.

| Decision | Simple template | Variable template |
| --- | --- | --- |
| Heritage and identity | Select race, culture, ethnicity, gender, a personal name, birthday, height, weight and handedness. | Set race and culture, choose an ethnicity or allow a compatible one to be selected. Weight the allowed genders and give each a ready random-name profile and height/weight model. Set an age range in years or racial age categories. |
| What players see | Write the short and full descriptions, select characteristic values, or apply a description pattern. `npc set randomise` can generate features to keep on this template. | Choose description patterns or let the generator choose eligible ones. Match patterns and characteristic vocabulary to the population. |
| Ability and background | Set individual attributes and skills. Choose merits, knowledges and any class, subclass or other roles your game uses. | Set an attribute budget and priority order. Give skills distributions, knowledges chances, and merits/flaws a pool and counts. Reserve certainty for what every member needs. |
| Speech | Give the character the language's linked skill and an accent. An optional `native` override selects a known language. | Give a required language skill a 100% chance. Configure permitted accents with `npc set accent <language> <accent>` or `npc set accent <language> all`. |
| Health and combat | Both editors can override the race's health strategy and the default combat setting. Use `npc set healthstrategy none` or `npc set combatsetting none` to return to the default. | Anatomy, equipment, training and the actions the character can physically perform still govern the result. |

A template's **unique name** is a stable builder and script lookup key. Set it with `npc set unique <key>`. The template name is what builders browse, the character has a personal name, and the short description is what most strangers see. Add intent or dependencies with `npc set comment <text>`. `npc list +<word>` searches the catalogue, `npc list &<ai>` finds templates using an AI, and `npc list all` includes historical revisions. Bulk `npc rename` and `npc renameunique` affect different names. Check their proposed match sets before using them.

While editing, `npc edit` displays the template and outstanding submission requirements. Complete the fields, use `npc edit submit`, then have a reviewer use `npc review <id>` and `accept edit <comment>`. Load the **Current** revision with `npc load <id or unique name>`. The new character appears at your exact location. Review later revisions the same way. To retire a design, obsolete its template and revisit its population sources. Existing people, their accounts and contracts need separate decisions. [Editable content and revision review](Generic_Editable_Item_Builder_Workflows.md)

### Teach a profession to many NPCs

A fixed `npc set skill <skill> 45` assigns an ordinary simple-template rating. On a variable template `npc set skill <skill> 80% 45 5` means four in five generated characters should have the skill, centred on 45 with a standard deviation of 5. Optional skewness changes the shape of the spread. A missing skill and a low rating can lead to different check outcomes.

Use `npcskillpackage` for skills shared by several professions or races. Each entry takes a chance, mean, standard deviation and optional skewness. A 100% chance, fixed mean and zero deviation give a dependable starting ability. Apply the package with `npc set skillpackage <package>`.

- A **simple template** rolls each entry when you apply the package. It adds missing skills or raises existing ones. Future copies receive the resulting stored ratings.
- A **variable template** copies the distributions. A competing entry replaces an existing one only when its chance multiplied by its mean is higher. The skill is rolled when the character is generated.
- **Racial packages** can be inherited. Setting the template's race applies the relevant packages additively. Changing race later can leave earlier skills in place. Editing a package later leaves earlier recipients as they were.
- **Chargen skill groups** are a separate option for variable templates. `npc set skillgroups 17 decline` makes repeatable selections from eligible groups using seed 17, taking required picks. `fill` takes optional picks up to the maximum. `npc set skillgroups off` disables the feature and keeps hand-authored skills. Use packages for a trade's common training and groups when the NPC should follow the game's chargen choices.

Packages grant skills. Supply required accents, knowledge of a procedure, equipment and authority through their respective systems. [Skills, checks and progression](Skills_Checks_and_Progression_Builder_Guide.md) · [NPC skill package rules](../Characters/NPC_Skill_Packages.md)

### What each new character receives

| Template setting | What it creates or attaches |
| --- | --- |
| `npc set outfit add <outfit-template> [name <name>]` | Materialises the outfit and its item prototypes. The outfit can specify worn items, wear profiles, inventory, containers and other placements. Use clothing that fits the race and equipment the body can use. |
| `npc set clan add <clan> <rank> [paygrade <paygrade>]` | Grants membership at the specified rank. `npc set clan appointment add <clan> <appointment>` also assigns an office, subject to its requirements. Choose appointments individually when the office has limited seats. |
| `npc set bank add <account-type> balance <amount>` | Creates an account for this character with the configured opening balance. That is new money and a new account on each eligible load. Use the institution's funds for institutional work. |
| `npc set implant add <key> <prototype>` and `npc set prosthetic add <key> <prototype>` | Add body items. A bodypart can be specified. Implant power and neural links refer to the other entries by key. Inspect the actual body and the load warnings when a part or connection is unavailable. |
| `npc set hook add <hook>` and `npc set onload <prog>` | Install a compatible event hook and run template-specific initialisation on new characters. An on-load prog is useful for an NPC-only duty command. Preserve existing initialisation when changing it. |

These additions can succeed independently. A loaded NPC wearing a coat is no proof their appointment, implant or bank account succeeded. Read the warnings from a manual load and inspect the person's memberships, gear and accounts before making the population repeatable.

## Load one NPC or maintain a population

For a unique official, `npc load <template>` once, record the character ID and decide succession in play. For a scripted introduction, FutureProg's `loadnpc("EastGateKeeper", @room)` returns the created character. Put the load on a one-time event or guard it with stored state if it should happen only once. A stable purchase or magical summon has its own creation and ownership lifecycle.

A spawner checks every five real minutes. It counts characters of the selected template in its **monitored zones**, including matching manual loads and qualifying dormant workers. A Boolean `counts` prog taking one character can narrow that count. The spawner acts when the count is **strictly below minimum**, aiming for **target**. With minimum 3 and target 5, three residents cause no refill. Two trigger an attempt to create three. Someone who walks beyond the monitored zones stops counting even while alive.

| Strategy | What to configure |
| --- | --- |
| `Simple` | One template and one or more spawn rooms. The spawner chooses an authored room for each arrival. A batch uses one generated character specification, so variable recruits in that refill can resemble one another. |
| `Multi` | Authored rooms and weighted template entries, for example `npcspawner set npc <porter-template> 3` and `npcspawner set npc <clerk-template> 1`. Weights govern draws across one combined population. They do not guarantee one clerk per three porters. Each draw generates its character specification. |
| `OpenTerritory` | A template with `TerritorialWanderer` and suitable unclaimed territory in the monitored zones. The current implementation stops that refill pass after placing one NPC. Allow subsequent population checks to fill more vacancies. |

`npcspawner set zone <zone>` toggles where the spawner **counts**. `npcspawner set cell <room-id>` toggles an arrival room for Simple or Multi. An arrival room outside all monitored zones can cause repeated overpopulation. Inspect both lists before enabling the spawner. `active` takes a Boolean prog with no arguments. Set it last. A false result prevents future fills and leaves existing residents in place. Use `npcspawner set onspawn <prog>` for an optional Void prog taking the new character. On a spawner load the template additions apply first, then the spawner prog, then the template on-load prog. The load-finished event follows entry into the room.

## Give NPCs actions and decisions

An **AI type** is a kind of behaviour offered by the engine. An **AI definition** is your named configuration of that type. `ai types` lists the available types and `ai typehelp <type>` explains their settings. `ai edit new <type> <name>` creates a definition. `ai clone <definition> <new-name>` gives you a separate copy to tune. Use `ai set ?` and `ai show` while editing. Individual AI edits take effect on the shared definition.

Give future recruits a definition with `npc set ai add <definition>` on their template, then submit and approve it. Give just one character that behaviour with `ai add <definition> <nearby-npc>`. `ai remove <definition> <nearby-npc>` removes the live attachment. `ai npclist <definition>` answers “who runs this now?” A specialised behaviour such as a mount or shopkeeper can coexist with other actions. Give responsibility for choosing movement, starting a fight or spending a host's money to a clearly identified behaviour.

### Events, decisions and custom actions

- **Use an existing AI** when the engine already knows the sequence: hear a knock, admit a visitor, close the gate after passage. A policy prog can supply one decision inside it, such as whether this visitor belongs to the watch.
- **Use a hook** for an action triggered by a particular event. `show events` and `show event <event>` give the recipient and ordered parameters. Create it with `hook create <name> <prog> <event>`. Use `hook install <target> <hook>` for a live recipient or `npc set hook add <hook>` on a template. Command-input hooks also require the command token. World default hooks have their own filtered installation rules.
- **Use a heartbeat** when the behaviour needs to reconsider periodically, such as looking for a destination. A heartbeat is an opportunity to act. A sleeping, paused, occupied or otherwise ineligible character can stay still. Observing an event, handling it and preventing another routine from handling it have different effects. Observer AIs receive their notification, ordinary AIs get their turn in attachment order until one handles the event, and regular hooks still have their own dispatch path.
- **Keep state with its owner.** The AI definition holds everyone's admission policy. A guard's current duty mode belongs to that guard. The live herd maintains its own alertness and roles. For short-lived personal dispositions, `remember`, `remembers` and `forget` can express like, fear or hate towards another character. This session memory follows those live individuals and expires across reloads. Use persistent world records for citizenship, pay or criminal status.

[FutureProg for builders](FutureProg_Builder_Guide.md) · [Event and hook contracts](../AI/Event_System_for_AI_and_Hooks.md)

### Movement, perception and relationships

Give a walker both a **destination** and a **way to reach it**. `PathToLocation` uses a Boolean pathing-enabled prog and a location-returning target prog. Returning the NPC's own room or a null location gives it nowhere new to go. A valid destination can still be unreachable because of an overlay, a one-way exit, water, an inaccessible layer, a closed door or the search range. The ordinary destination search extends up to twelve exits. Inspect the AI's door, key and guard settings before changing the map. Use separate posts for a stationary gatekeeper and a mobile watch member. Both can use the same uniform.

A path to a room does not establish that the NPC has **perceived** a character in it. Animal attention depends on senses, visibility, distance, layers and line of sight. A visible animal across a stream can be outside a ground predator's usable movement path. Pick the right relationship for a player-directed NPC. Following moves a willing character with a leader. A `Commandable` AI decides whose orders to accept and which commands are allowed. A `Mount` AI decides who may ride and who may direct movement. Companion assignment can link a character's logout and reunion to a handler. None of these grants a shop contract or a legal office.

### Animals, monsters and groups

Use **Animal** for a creature whose behaviour follows its ecology. Choose how it moves (ground, tree, air, water or amphibious), where it lives, how it feeds and drinks, what it notices, how it responds to threats and where it takes refuge. A ground forager needs a usable food source, an active needs model, reachable drinking water and terrain it can traverse. A predator also needs suitable prey, the ability to perceive and reach it, and the anatomy and combat options to act. A preferred habitat attracts movement, tolerated habitat permits transit. An impassable stretch between two excellent feeding sites can isolate both. Activity windows and dormancy change when the animal normally acts. Urgent needs or immediate threats can interrupt rest.

Use **Monster** for a creature driven by a motive such as guarding territory, responding to provocation or hunting on a schedule. Give it a stable home if the motive requires one. New definitions need an explicit proactive motive. Its willingness to act, observation of a target and ability to carry out the action are separate decisions. An NPC can have one primary Animal or Monster controller. A Monster cannot join a Wildlife group. An auxiliary behaviour, such as maintaining a natural trap, can complement the primary controller. [Creature behaviour and hunting](../AI/Predator_Hunting.md) · [Monster motives and activity windows](../AI/Monster_AI.md)

Use **group AI** for decisions made together. `gait` edits the policy. `group new <policy> <name>` creates an empty live group and `group addmember <group> <nearby-npc>` enrols a character. One NPC can be in one live group. A production Wildlife policy can describe herds, packs, flocks, schools and other social arrangements. Its control scopes can include movement, feeding, threats, activity, shelter and senses. Give the group the scopes it should coordinate and let the individual handle unclaimed decisions, direct combat and urgent survival. The live group holds its leader, sentries and other roles, alertness, action and remembered places.

Use `gait show <id>` to distinguish the configurable `wildlife` type from the older `wimpyherdgrazer`, `neutralherdgrazer`, `territorialherdgrazer` and `familypredator` policies. `WildAnimalHerd` is an older *individual AI* with its own herd logic. When building a new meadow herd, start with individual Animal behaviour and one Wildlife group policy so there is a single owner for coordinated movement. `group show <group>` displays live status. `gait set emote` adds behaviour descriptions that can depend on role, age, gender, target or alertness. Changes to a shared group template take effect on its live groups.

## Behaviour index

Use the names below with `ai typehelp <type>`. For each job, start with the smallest behaviour that supplies the decision you need. A staff member who greets visitors need not also choose the shop's purchases or control its door.

| Type | What it contributes and what to supply |
| --- | --- |
| `PathToLocation` | Goes to a prog-selected destination when pathing is enabled. Supply a reachable destination and door policy. |
| `Wanderer` | Roams among eligible rooms. A second routine assigning a fixed post can send the same person back and forth. |
| `ArborealWanderer` | Prefers tree layers. Supply connected tree habitat and decide when descent is allowed. |
| `FlyingWanderer` | Roams in flight. The body must be able to fly and the route must offer usable layers. |
| `TerritorialWanderer` | Seeks and maintains territory. Also supplies the free-territory search for an OpenTerritory spawner. |
| `IdleEmoter` | Supplies periodic ambient emotes. Give it actions a bystander can plausibly see the NPC doing. |
| `React` | Binds reactions such as greetings, gifts or weather to progs and emotes. Choose the event recipient and conditions. |
| `Commandable` | Accepts authorised player orders. Its policy receives the NPC and issuer. Limit who can issue which commands. |
| `SelfCare` | Responds to health needs such as bleeding. Check whether the character can carry out the action with available supplies. |
| `Scavenge` | Evaluates items and calls a scavenging prog. For an animal collecting possessions in a den, see `LairScavenger`. |
| `Animal` | Composes movement, diet, water, habitat, perception, threat response and activity. Choose the individual and group responsibilities together. |
| `Monster` | Chooses actions from authored motives, territory and activity windows. Supply target eligibility, a usable body and the required home or schedule. |
| `DenBuilder` | Establishes a den or nest using a craft. Supply a valid craft and a suitable site. |
| `LairScavenger` | Collects eligible items and returns them to a remembered lair. Supply a home and items it can actually collect. |
| `NaturalTrap` | Maintains a natural trap at an eligible site. Supply a current trap template and site policy. It can observe events alongside the primary controller. |
| `WildAnimalHerd` | Legacy individual herd behaviour with its own roles and reactions. For an existing herd, inspect its configuration before adding group movement. |
| `Aggressor` | Engages eligible targets. Its target prog determines who qualifies, and aggression classification affects how other systems perceive the NPC. |
| `SemiAggressive` | Combines selective aggression and movement. Inspect the engagement and pathing rules before assigning a second route. |
| `AggressivePather` | Combines a prog-selected route with target selection and engagement. Use when the same behaviour should own both decisions. |
| `TrackingAggressor` | Pursues targets using tracking behaviour. Decide when pursuit may begin, continue or end. |
| `Stealth` | Hides or sneaks under the configured activity and habitat conditions. Consider what a group's sentries or senses already control. |
| `Rescuer` | Assists eligible allies in trouble. Define who merits assistance. |
| `CombatEnd` | Handles truces and what happens when an opponent is incapacitated. Pair with the intended combat policy. |
| `SparPartner` | Responds to invitations to spar. Set willingness and response delay for a practice partner. |
| `ArenaParticipant` | Acts within an arena event. Set up the arena, entry and opponents before relying on arena behaviour. |
| `ArtilleryCrew` | Provides crew behaviour for in-game artillery. The crew member needs a nearby piece with a usable crew role and an existing combat target. |
| `Doorguard` | Handles door requests. Supply an admission prog, open/close actions, a real door and an NPC on duty. |
| `Tollkeeper` | Collects an authored toll for passage through the exit the NPC is guarding. The toll position and passage permit belong to the individual arrangement. |
| `ClanSentry` | Reports observed departures privately to a chosen clan. Configure the directions and the recipient clan. |
| `Shopkeeper` | Responds to shop events and helps restock displays as an employee. Give it employment, stock and a reachable display. |
| `EmploymentWorker` | Performs host tasks through its enabled capabilities and the worker's contract and delegated authority. |
| `ProjectWorker` | Participates in project labour. Supply an eligible role and the skills it requires. |
| `Enforcer` | Acts for a legal authority. Supply the recognised enforcer role, on-duty status and patrol setup. |
| `Judge` | Directs court proceedings within the justice system. The jurisdiction, courtroom and trial arrangements give it a case to hear. |
| `Lawyer` | Handles legal hiring, travel and representation. Supply eligibility, advocacy skills and the authority's legal service. |
| `Mount` | Sets riding and control permissions, rider count and difficulties. Companion following is optional. |
| `SpellCompanion` | Supplies behaviour for a magical companion. The spell supplies the bond. The AI selects companion or outsider mode, permitted orders and any loss-of-control policy. |

### When another system supplies the job

| Job | Configure on the person | Configure at the institution |
| --- | --- | --- |
| Shop counter or buyer | Free hands, carrying capacity and the AI appropriate to the task | Employment, merchandise, funds, spending authority and delivery access. [Shopkeeping](Shop_and_Banking_Builder_Guide.md) · [Employment hosts](Employment_Hosts_Builder_Guide.md) |
| Inn cook, server or hospital practitioner | Worker capabilities, the relevant skills, tools and any required surgical knowledge | Contract, service role, rooms, supplies and work orders. [Hospitality](Hospitality_Builder_Guide.md) · [Hospitals](Hospital_Builder_Guide.md) |
| Project labourer | Relevant traits and behaviour | Approved project, available labour role, requirements, assignment and any pay. [Projects](Projects_System_Builder_Workflows.md) |
| Clan officer or justice official | Membership, rank, skills and appropriate AI | Office, legal authority's eligibility rules, patrol or court, jurisdiction and duty status. A watch uniform grants none of these powers. [Clans](Clan_Builder_Guide.md) · [Justice](Justice_System_Builder_Guide.md) |
| Arena entrant | Eligible body, combat settings, skills and arena behaviour | Combatant class, venue and event registration. [Combat and arenas](Combat_and_Arenas_Builder_Guide.md) |
| Stable animal or pasture stock | A compatible live template, species and any Mount or Animal behaviour | Boarding or sales at a stable, or a herd definition with a matching template on a field. A sale, a boarding contract and group membership are separate. [Hospitality](Hospitality_Builder_Guide.md) · [Agriculture](Agriculture_Builder_Workflows.md) |

## Worked example: staff the east gate

The eastern gatehouse has one outward-facing door to the road and an open passage behind it to the watch yard. **Mara**, the named keeper, stays inside the gatehouse and admits fellow watch members who knock. **Watch recruits** wear the same uniform and walk between the yard and the gatehouse. Only the recruits are replenished. The walk is an ordinary posting. A legal authority organises formal arrests and dispatches patrols.

### Prepare the uniform and training

Have a Current coat prototype with a wearable component that fits the watch, a town watch clan and a recruit rank. Use your local IDs for the coat, clan, ranks, skills and language. Supply any referenced skill or outfit once, then reuse it on both templates.

```text
outfittemplate new EastWatchUniform
outfittemplate set item add coat <coat-prototype> worn
npcskillpackage new EastWatchTraining
npcskillpackage set skill <observation-skill> 100% 45 5
npcskillpackage set skill <local-language-skill> 100% 55 0
npcskillpackage show EastWatchTraining
```

Give Mara her own fixed identity. Start with `npc edit new simple`, set her `race`, `culture`, `ethnicity` and `gender`, then `npc set name <valid-local-personal-name>` and `npc set birthday random 46`. Give her appropriate `height`, `weight`, `hand`, attributes and characteristic values. Set `npc set sdesc a grey-haired watch officer`. Use `npc set desc` to write her full description, such as “A ring of pale stitching marks the shoulder where she once wore a sergeant's patch.” If your game already has the right foundation, clone a Current simple template and replace its identity and inherited duties.

```text
npc set unique EastGateKeeper
npc set clan add <watch-clan> <keeper-rank>
npc set outfit add EastWatchUniform
npc set skillpackage EastWatchTraining
npc set accent <local-language> <local-accent>
npc set comment Keeper at the east gate. Appoint a successor individually.
```

### Give Mara a door and an admission rule

Install a door on the **east** exit from Mara's post. Begin with it closed, unlocked and operable by her body. In this example the watch is her only clan and fellow members are admitted. Create the four progs below in `prog edit new <name>`. Set the return type, add the named parameters **in order**, then enter the body with `prog set text`. The last parameter on the first three progs is an exit. The action commands use the direction on Mara's side of this particular gate. For a guard serving several doors, resolve the appropriate exit keyword from the guard's side instead. [In-game FutureProg editor](FutureProg_Builder_Guide.md)

| Prog | Return and parameters | Body |
| --- | --- | --- |
| `EastWatchAdmits` | Boolean, `guard character`, `visitor character`, `gate exit` | `return isclanbrother(@guard, @visitor)` |
| `EastWatchOpen` | Void, same three parameters | `force @guard "open east"` |
| `EastWatchClose` | Void, same three parameters | `force @guard "close east"` |
| `EastWatchDuty` | Void, `guard character` | `force @guard "doorguard"` |

For example, set up the admission prog as follows. Submit the source with `@` alone on a line. Use the same editor for the other three. If the template already has an on-load prog, add the duty command to its existing initialisation.

```text
prog edit new EastWatchAdmits
prog set return boolean
prog set parameter add guard character
prog set parameter add visitor character
prog set parameter add gate exit
prog set text
return isclanbrother(@guard, @visitor)
@
```

```text
ai edit new doorguard EastGateAdmission
ai set will EastWatchAdmits
ai set open EastWatchOpen
ai set close EastWatchClose
ai show
ai close
npc set ai add EastGateAdmission
npc set onload EastWatchDuty
npc edit submit
```

The `will` prog decides whether Mara agrees. The `open` and `close` progs take the actual actions. Leave **Own Side Only** off to respond to knocks from the road. Leave **Respect Game Rules** on for this unlocked door. A physically blocked or locked gate can fail the capability check before the open action runs. A `cant` response can explain that to the visitor, while `wont` can explain a refusal. For a locked-door design, explicitly plan key handling and the order of that capability check. If admission must also apply while the gate stands open, put the restriction on the exit itself. A keeper's decision about opening a door alone gives no control over someone following a friend through it.

Have a reviewer approve Mara's submitted revision. Stand **inside** the gatehouse, run `npc load EastGateKeeper` once and note her character ID from `npc instances EastGateKeeper`. `doorguard` is a toggle on the individual. It is useful to possess an existing off-duty keeper, switch it on and `return`. Reissuing it on an on-duty keeper takes her off duty. From the road a watch member using `knock west` on the closed gate should see it open after the delay and close after passage. A stranger should get a different result while it remains closed.

### Make a guard walk between two rooms

Create a location-returning prog `EastWatchRound`, taking one character parameter `guard`. In the following body substitute the IDs from `room show` for the gatehouse (101) and yard (102). The final null leaves someone temporarily posted elsewhere without a new destination.

```text
if (@guard.location.id == 101)
  return tolocation(102)
end if
if (@guard.location.id == 102)
  return tolocation(101)
end if
return null("location")
```

```text
ai edit new pathtolocation EastWatchRoundAI
ai set target EastWatchRound
ai set pathingenabled AlwaysTrue
ai show
ai close
```

`AlwaysTrue` here is the stock permissive prog. A Boolean prog taking the guard can instead restrict the round to duty hours. A null destination, a closed connecting door or a competing assignment can each leave a perfectly able guard at the post for different reasons.

Create the second template using `npc edit new variable`. Set `npc set name East Gate Watch Recruit`, `npc set unique EastWatchRecruit` and the race, culture and ethnicity of the recruiting district. For each eligible gender use `npc set gender <gender> <weight>`, `npc set hwmodel <gender> <model>` and `npc set randomname <profile> <gender>`. A pair of weights 1 and 1 gives equal odds. `show hwmodels` helps locate the correct models. Select valid descriptions for the population and use `npc set age 20 48`. If a suitable variable townsfolk template already exists, cloning it preserves the name and build rules.

```text
npc set clan add <watch-clan> <recruit-rank>
npc set outfit add EastWatchUniform
npc set skillpackage EastWatchTraining
npc set accent <local-language> <local-accent>
npc set ai add EastWatchRoundAI
npc edit
npc edit submit
```

Approve the recruit revision. Load one recruit in the yard before opening the post to more staff. The recruit should be wearing the watch coat, have a personal name, speak the local language and start the round. Mara keeps her own post and office.

### Add a population spawner

Use a `Multi` spawner with a single weighted template here so each opening in a batch draws its own variable recruit. A second template and weight can later introduce another kind of watch member. Set the **gatehouse zone**, which must include both ends of the route, and use the **yard** for arrival.

```text
npcspawner edit new EastGateWatch
npcspawner set strategy Multi
npcspawner set npc <recruit-template-id> 1
npcspawner set zone <watch-zone-id>
npcspawner set cell <yard-room-id>
npcspawner set target 4
npcspawner set minimum 3
npcspawner show EastGateWatch
npcspawner set active AlwaysTrue
npcspawner close
```

With one matching recruit in the zone, the next active check can bring the count up to four. Three remaining watch members can handle the gate without a replacement. If one of those three leaves the district, the remaining two trigger a refill. Mara has a different template and contributes nothing to the count. `npcspawner show EastGateWatch` displays the count. Use `npc instances EastWatchRecruit` to identify the actual people. To suspend recruitment, edit EastGateWatch and set `active AlwaysFalse`.

## Worked example: deer on the water meadows

Build two connected meadow rooms near accessible drinking water. Players cut greens on the drier ground, while deer can feed on either bank. A **room yield** feeds the animals, a **deer template** supplies bodies, an **Animal AI** supplies individual needs and responses, and a **live herd** coordinates its members. Establish each in that order.

### Give the deer something to eat

Use an approved foragable such as sorrel with yield type `greens`. Create a foragable profile for the meadows. If you already have a suitable profile, use it directly.

```text
fp edit new
fp set name Water Meadow Greens
fp set yield greens 4 1
fp set foragable <approved-sorrel-id>
fp edit submit
```

After review and approval, stand in **each** feeding room and use `room set forage <profile-id>`. Each room gets a capacity of four yield points and recovers one per hour. The rooms share configuration and keep independent balances. Select the deer race in the race editor and inspect its inherited diet first. If greens are new to it, use the following settings.

```text
race set yield add greens 2 0 $0 crops the meadow grass.
race set yield greens bite 0.25
```

Two and zero are the hours of hunger and thirst satisfied per yield point. A quarter-point bite feeds the animal but supplies no water in this example. Place a water-filled trough or another accessible liquid container at ground level for a land forager with `water drink`. Blue terrain or a stream mentioned in the room description alone does not supply a drinkable container. The individual and group habitat policies must allow the route to food and water. A flower prototype sitting on the ground is not a supply of the room's `greens` yield. [Foraging, yield recovery and racial diets](Primary_Production_and_Agriculture_Builder_Guide.md)

### Give each deer its own behaviour

Use `ai list +Forager` to find `Wildlife - Timid Ground Forager` if the wildlife catalogue is installed. Clone the definition for local habitat changes. This profile prefers grassland and shrubland and is normally active at dawn and dusk. `ai show` should report a ready ground-moving forager, active feeding needs and a usable water strategy. In a world without the catalogue, start from `ai edit new animal MeadowDeerForager`. Configure `movement ground`, `feeding forager`, `feeding needs active`, `water drink` and the animal's threat, awareness and activity settings through `ai set`. Inspect readiness and supply the preferred and tolerated habitat progs for your terrain before proceeding.

```text
ai clone "Wildlife - Timid Ground Forager" MeadowDeerForager
ai show MeadowDeerForager
npc clone <current-deer-template>
npc set unique MeadowDeer
npc set ai add MeadowDeerForager
npc edit submit
```

Use either the catalogue clone or the new definition. Between the clone and AI-add steps, inspect the new NPC template. If it already carries an Animal or Monster AI, remove that attachment with `npc set ai remove <old-ai>` before adding MeadowDeerForager. A variable deer template can vary age, sex, size and coat. Inspect the clone for inherited duties, loot-bearing outfits or other behaviour that would be out of place. Submit and approve, then load several deer at the near meadow with `npc load MeadowDeer`. Keep their character IDs from `npc instances MeadowDeer`.

### Put the animals in a social group

Look up `Wildlife Group - Timid Grazing Herd` with `gait list`. Clone it when tuning a local herd. Inspect its activity window, preferred habitat, threat rule, movement range and control scopes. In particular, give the group **Feeding** and **Movement** so it can coordinate grazing and travel. The following scope also hands it threat, activity and sensory decisions. For a world without the stock group, create the policy with `gait edit new MeadowHerdPolicy` and set `gait set type wildlife Female Herd Timid Crepuscular`. For this herd, create `MeadowIntruder` as a Boolean prog taking one character named `target`, with body `return not(isanimal(@target))`. Set `gait set threat MeadowIntruder`. Continue below from `gait edit MeadowHerdPolicy` once the policy exists.

```text
gait clone "Wildlife Group - Timid Grazing Herd" MeadowHerdPolicy
gait edit MeadowHerdPolicy
gait set activity Crepuscular
gait set scope Movement,Feeding,Threats,Activity,Senses
gait show
gait close
group new MeadowHerdPolicy "Water Meadow Herd"
group addmember "Water Meadow Herd" <first-live-deer-id>
group addmember "Water Meadow Herd" <second-live-deer-id>
group show "Water Meadow Herd"
```

This aligns the group with the stock individual forager’s dawn and dusk activity. A live group can maintain roles such as leader and sentry, coordinate responses to threats and choose to graze when its members need food. Hungry deer consuming greens reduce the same room balance available to a player who forages there. As one meadow empties, the other offers a feeding opportunity. Their next action also depends on thirst, alertness, activity window and accessible exits. To make an approaching traveller matter, inspect the group's `threat` prog and choose a timid response policy that recognises the intended visitor. A threat the animals cannot perceive does not demonstrate how they flee. Once safe, the herd can return to meeting its needs.

For a continuing population, configure a monitored zone, meadow arrival rooms and a spawner for the deer template as in the watch example. **New deer start outside the live group.** Compare `npc instances MeadowDeer` with `group show "Water Meadow Herd"` and use `group addmember` for a replacement. A spawner's on-spawn prog has no native FutureProg operation to enrol the new animal in a live Wildlife group. For unattended replenishment, give each deer an individual profile that also works when solitary.

A farmer who wants headcount, condition, wool or milk should create an **agricultural pasture herd**. Its field operations manage abstract livestock. `field herd draw` can create a live animal when the herd definition has a matching NPC template. `field herd absorb` takes an eligible matching animal back into field stock, subject to handling and relationship checks. Adding deer to a Wildlife group neither creates a field herd nor assigns the farm its products. [Pasture and livestock handling](Agriculture_Builder_Workflows.md)

## Three smaller builds

### A shopkeeper at the counter and a buyer at the market

A provisions shop needs two people. The counter clerk puts available goods on display. The buyer leaves the building, spends shop funds at a supplier and brings stock home. Give each a Current NPC template with the physical capacity for the job. Place the clerk at the counter and provide a reachable stockroom and display. Create and attach the two live behaviours:

```text
ai edit new shopkeeper ProvisionsClerk
ai close
ai add ProvisionsClerk <clerk>
ai edit new employmentworker ProvisionsBuyer
ai set host shop
ai set currency <local-currency>
ai set capability CanPurchaseCommodities
ai show
ai close
ai add ProvisionsBuyer <buyer>
```

A new EmploymentWorker already has tasking and delivery enabled. The capability command toggles purchasing, which covers discrete items as well as commodities. Inspect it before toggling an existing worker's configuration. Inside the shop, use `shop contracts assign #<clerk-character-id> employee` and `shop contracts assign #<buyer-character-id> employee`. Check `shop contracts`. Give the **buyer contract** the delegated rights required by the purchasing goal. For the restaurant ingredient workflow these are `ManageStockRules`, `ApprovePurchases` and `ManageDeliveryRoutes`, assigned with `shop contracts delegate <contract-id> grant <right>`.

Create the manager's replenishment goal, identify the supplying shop and its stock, set a spending cap and fund the employer. The buyer needs a usable route in both directions. The clerk can tend a display while the buyer is away. For future employees, attach each AI to the corresponding template and arrange a contract for each new hire. A market `shopper` is a separate, scheduled virtual customer. It simulates demand and does not bring a physical employee back with supplies. [Worked purchase and delivery goal](Hospitality_Builder_Guide.md#send-an-npc-to-the-market-for-supplies) · [Merchandise and shop staff](Shop_and_Banking_Builder_Guide.md)

### A stable pony that follows its rider

Start with a sound pony whose body and size suit a rider. This example is a riding-school pony that permits members of the school to mount and direct it. Give the pony membership of the school's clan, and create `SchoolRider` as Boolean with **rider character first, mount character second**. For a pony belonging only to that clan, `return isclanbrother(@rider, @mount)` admits fellow members. If passengers and handlers have different privileges, use separate `permit` and `control` progs.

```text
ai edit new mount SchoolPony
ai set permit SchoolRider
ai set control SchoolRider
ai set riders 1
ai set difficultycontrol Easy
ai set companion
ai set nearbycompanion
ai show
ai close
ai add SchoolPony <pony>
```

On a new definition the two companion toggles above turn those behaviours on. A permitted pupil can `mount <pony>`, direct travel as the primary rider and `dismount`. With rider-companion behaviour enabled, riding establishes the handler association and the pony can follow after dismount or reunion. Nearby-only reunion requires the same room and layer. Fit appropriate tack for the intended riding difficulty. Bareback riding, missing reins or other missing riding gear can affect control and falls. Booking the pony into a stable uses that stable's custody and fees; authorising a pupil to ride does not settle that account. [Stable services](Hospitality_Builder_Guide.md) · [Riding and equipment](../Combat/Mounted_Combat.md)

### A guardian of the old observatory

The stone observer guards the telescope chamber. Its instruction is to warn intruders and return to that chamber after a pursuit. Record the chamber ID with `room show`. Write `ObservatoryHome` returning Location for one Character parameter, with `return tolocation(<chamber-id>)` using the actual number. Using the guardian's *current* room would move the defended site every time it left home.

```text
ai edit new monster ObservatoryGuardian
ai set motive Territory
ai set home location ObservatoryHome
ai set guardrange 0
ai set warning 20
ai set warningemote $0 turns its stone face towards $1 and raises one hand.
ai set returnhome on
ai show
ai close
npc set ai add ObservatoryGuardian
```

The final command assumes you have the guardian's NPC template open and have removed any inherited Animal or Monster controller. Give the guardian appropriate target and ally rules for your setting, an alert body and a usable movement path. Submit and approve the template, then load it in the chamber. An observed, eligible intruder in the home room should receive a warning. Leaving during the warning and remaining in the defended area have different consequences. The guardian's pursuit uses its configured range and timeout. Ordinary combat settings and anatomy still determine what it can actually do.

For a **night-only ward**, reopen the definition, set `ai set active times Night` and `ai set defencewindow on`. Territory and provocation ordinarily operate outside proactive activity windows, so the second setting matters. Keep the Territory motive for a guardian that only challenges intrusion. Adding the separate Scheduled motive also permits proactive hunting during its window. Immediate self-defence remains available. [Monster activity, warning and home settings](../AI/Monster_AI.md)

## Troubleshooting

Begin with the character players are seeing. Use `npc instances <template>` to find the person, `ai npclist <definition>` for live users of an individual AI, `npcspawner show <name>` for counts and `group show <name>` for roster and current action. Staff with possession authority can `possess <npc>`, use an NPC-only command and `return` to their staff character. `pause` toggles individual AI behaviour while you inspect it. Check the state before leaving an NPC on duty.

| What players see | What to inspect |
| --- | --- |
| A new template will not load | Is it Current? Does the simple NPC have its identity, birthday, descriptions and required accents? Does the variable template have a usable name profile and height/weight model for each eligible gender? Read submission feedback, then confirm the review was accepted. |
| A recruit has the wrong coat, rank or skills | Check the character's template and revision, load warnings, outfit placement and package application date. Decide whether to update future recruits, issue items to existing staff or do both. |
| The keeper ignores knocks | Check the side of the closed door, `doorguard` duty mode, `will` prog, open and close actions, and the NPC's ability to operate it. Use the visitor the policy is supposed to admit. |
| A watch member stands still | Inspect enabled and target progs, the destination from that particular room, the active overlay, both exit directions, posture, doors and keys. Check whether the NPC is paused, already moving or assigned competing work. |
| An extra captain appears or the watch keeps overfilling | Verify which template the spawner counts, zone boundaries, dormant workers, the optional counting prog and the arrival rooms. An eligible manual load can satisfy the minimum. A small duty area inside a large zone may count staff posted elsewhere. |
| A replacement deer never follows the other deer | Confirm individual AI with `ai npclist` and group membership with `group show`. Add the live replacement to the herd. |
| A herd ignores a lush meadow | Inspect actual room yield, the race's diet and bite size, hunger and thirst, reachable water, allowed habitats, group control scopes, activity and threats. Green room text alone supplies no food. Founder diagnostics `impdebug wildlife <npc-id>` and `impdebug wildlife group <group-id>` expose the current decisions. |
| The buyer never leaves the shop | Check the worker's host, capabilities and contract grants, the approved goal, reserved funds and supplier availability. An idle counter clerk offers little evidence about the purchasing workflow. |
| The pony accepts a rider but will not move | Inspect the separate control prog with rider and mount in the correct order, primary rider, anatomy, tack, encumbrance and the exit. |
| The guardian warns nobody | Check the home prog, territory motive, guard radius, allies, target visibility, activity and defence-window settings. Founder diagnostics `impdebug monster <npc-id> <target-id>` explain how this individual assessed this intruder. |
