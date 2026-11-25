---
slug: futureprog
category: Builder tools
summary: Write and attach FutureProgs for gate admission, custom craft products, starting outfits and water main construction. Learn signatures, collections, events, registers and item ownership.
---

# FutureProg for builders

A builder writes a **prog** when a game system needs a rule or action particular to their world. A gatekeeper needs to know whom to admit. A pottery recipe needs to turn a customer's order into the right vessels. The systems already know how to answer a knock and finish a firing. The prog supplies the part the builder decides.

Start with the caller and the language below, then follow the [east gate](#admit-a-visitor-at-the-east-gate), [custom pottery order](#fulfil-a-custom-pottery-order), [starting outfit](#issue-a-starting-outfit) or [water main extension](#extend-a-water-main). Each named source block gives its parameters in editor order and its return type. Substitute your world's catalogue names and IDs where indicated. The [original prog listings](#appendix-original-futureprogs) include the older scripts discussed in these builds. If you have already built the gate or kiln from its companion guide, inspect the existing named progs before creating replacements.

## Start with the caller

The **caller** is the game system, event hook or other prog that invokes your code. Its contract answers four questions. When does it run? Which values does it pass, in what order? What result does it expect? Who acts on that result?

| Caller | Signature it can use | What the caller does with it |
| --- | --- | --- |
| Doorguard admission | Boolean(Character, Character, Exit) | Asks about a particular guard, visitor and door. A positive answer permits the AI to proceed with its opening routine. |
| A craft's prog product | Item Collection(Item Collection Dictionary) | Supplies the items the recipe actually consumed and collects the finished items you return. |
| Starting-location commencement | Void(Character) | Supplies the character entering play. The prog performs the issue or other welcome action itself. |
| Local project | Boolean(Character), Text(Character), or Void(Project), according to the field | Asks the prospective worker about eligibility, requests a reason for refusal, or notifies an active project of a lifecycle event. |
| Installed event hook | The event's own parameter list | Delivers an event to the object on which the applicable hook is installed. Use `show event <event>` to find the recipient and arguments. |

A **signature** is the return type plus the ordered parameter types. The two Character arguments at the gate have different jobs. Swapping their names can leave a prog that compiles and answers the wrong question. A **policy** returns an answer and should be safe to call repeatedly. Menus and availability checks may ask the same question before anyone acts. An **action** changes the world. Put actions at the point where the caller has committed to doing the work.

## Values, variables and choices

- A **Boolean** is `true` or `false`. A **Number** can hold a count, an ID or a fraction. **Text** includes messages, names and dictionary keys. `"3"` is text and `3` is a number.
- A **Character**, **Item**, **Location** (room), **Exit**, **Grid** or **Project** refers to a particular game object. A prototype describes what can be created. An item is one existing copy. `@slip.id` identifies that piece of paper and `@slip.proto` identifies its prototype. Use `prog help type item` to see the properties the language exposes.
- Declare a local with `var site = @actor.location` when the expression supplies the type, or `var ware as item collection` to start a typed collection. Read a value as `@site`. Assign a variable by its bare name, for example `count = 4`. `==` compares two values. A local belongs to this invocation. A new call, including a delayed call, gets new locals.
- A lookup can return **null** when no object matches. Check `isnull(@slip)` before reading `@slip.proto`. A failed lookup and a function that raises an execution error are different outcomes. For example, `tooutfittemplate` lets you check for a missing template, while trying to load a missing template stops the prog.
- Use `if`, `else` and `end if` for branches. `return` ends this call immediately. In a Boolean prog, `return false` gives the caller an answer. In a Void prog, `return` ends the action. Use `switch` for named stages or alternatives, `foreach` to visit members, and `for (cup : 4)` to repeat a fixed number of times.

Call a built-in as `isclanbrother(@guard, @visitor)`. Call a builder-authored helper as `@WhyWaterMain(@actor)`. Give a reusable helper one explicit job so several callers can share it. `prog help function <name>` shows built-in overloads and result types. `prog show <name>` shows the signature and source of an authored prog.

### Collections and dictionaries

A **collection** holds values of one type. `@ch.roles.any(r, @r.name == "Watch Officer")` asks whether at least one role matches. Inside the expression, `r` names the current role. An empty collection answers false. `.where` collects matches, `.first` selects the first match and `.count` tells you how many there are. When exactly one exit must match a survey, count the matches first. Taking the first of two conceals an ambiguous route.

A **dictionary** maps text keys to values of one type. A **collection dictionary** maps each text key to a collection. A recipe can consume several pieces from one input slot, which is why its input dictionary holds collections even when this particular recipe asks for one slip. `collectiondictionarygetfirst(@inputs, "3")` retrieves a single consumed item, or a null item for a missing or empty slot. Use `prog help collections` for operations on collections and `prog help function dictionaryset` for keyed updates.

### State that survives a call

Choose the owner of persistent state by asking who needs to remember it. A starter kit belongs on the **character**. A survey belongs on its **room**. The route a crew accepted belongs on the **active project**, so changing tomorrow's survey cannot redirect today's crew. A station shared by a train service can belong on its **zone**.

Declare a register once with `register <owner-type> <name> <value-type>`. In a prog read it with `getregister(@owner, "Name")` and write it with the statement `setregister @owner "Name" value`. In the builder console `register show <owner-type>` inspects declarations and `setregister <owner-type> <target> <name> <value>` sets a particular object's value. Use `sniff here` or `sniff <character>` to inspect the values stored on a particular room or character. For a dictionary, save the result of `dictionaryset` or `dictionarywithoutkey` back to the register. A new dictionary returned inside a call has no effect on the saved one until you write it.

**Timing.** `delayprog` queues a later call in the running game. Its delay is in milliseconds. Pass the next stage explicitly, bound the number of stages and check the world again when the call runs. A saved “playing” flag can outlive a queued call after a restart. Give the scene an end condition and a way to release that flag.

**Caching.** Leave these examples dynamic. A fully static prog reuses its first successful result until its cache resets. Admission depends on the two people at the gate, a craft must create fresh items, and a reservation must read the current owner each time.

## Author, attach and inspect a prog

The `prog` command requires Admin authority. Look up the field in the caller's editor first. Some fields accept multiple signatures. Choose the one that provides the information the rule needs.

| In-game command | Builder use |
| --- | --- |
| `prog list +<word>`, `prog list uncompiled`, `prog show <name or id>` | Find existing work and inspect the signature, static setting, source and compile result. |
| `prog edit new <name>`, `prog set return <type>`, `prog set parameter add <name> <type>` | Create the definition. Add the parameters from left to right. |
| `prog set category <name>`, `prog set subcategory <name>`, `prog set comment` | Group related progs and record who calls them and which catalogue records they expect. |
| `prog set text`, `prog set append`, `prog edit close` | Replace or append source, then close the edit selection when finished. In the text editor submit with `@` alone on a line, or abandon with `*cancel`. |
| `prog execute <name> <arguments>`, `prog help arguments` | Supply values such as `self`, `here` or `null`. Execution really runs actions that create property, spend materials or change rooms. |
| `show events`, `show event <event>`, `prog edit newevent <event> <name>` | Inspect an event and initialise a Void prog with its argument names and types. |
| `builderrefs prog <id>` | Find registered references before changing a signature or replacing a shared rule. Also inspect authored calls in other progs. |

Edits are live. Submitting invalid source can leave a bound prog uncompiled. Prepare substantial replacements under a fresh name, compile, then bind them in the owning editor. A compile verifies types and syntax. The caller determines which objects arrive, whether an action can succeed and what players see.

### Events, messages and stages

A subsystem slot such as `ai set will` already supplies its trigger. An event hook is useful for an additional reaction, for example greeting a new arrival. `prog edit newevent CharacterEnterRoomFinish HallArrival` creates a prog with `mover`, `destination` and `exit`. In its body, check the destination before using `send(@mover, "The porter points you towards the issuing desk.")`. Create it with `hook create HallGreeting HallArrival CharacterEnterRoomFinish` and install it with `hook install <character-keyword> HallGreeting` as a Senior Admin. This event goes to the moving character. Choose the corresponding witness event if the observer should react. Remove the binding with `hook remove <character-keyword> HallGreeting`. [Events and hook recipients](../AI/Event_System_for_AI_and_Hooks.md)

The same Boolean can govern visibility of a paragraph with `hedit extra add <topic> <prog>`. A help note is a convenient home for an explanation. Bind an eligibility rule to the action's own permission field when it should control what players can actually do.

## Admit a visitor at the east gate

Mara stands inside the gatehouse. Her only clan is the town watch. A visitor on the road knocks on the closed door, and Mara admits people who share her watch membership. Start with one accessible, unlocked door on the **east** exit from her post. This gives the rule a visible consequence whatever answer it returns.

Create the admission rule in game. Each signature in this guide names the prog and its inputs. The type after the arrow goes in `prog set return`.

```text
prog edit new EastWatchAdmits
prog set return boolean
prog set parameter add guard character
prog set parameter add visitor character
prog set parameter add gate exit
prog set text
```

**`EastWatchAdmits(guard character, visitor character, gate exit) → boolean`**

```futureprog
if (isnull(@guard) or isnull(@visitor))
  return false
end if
return isclanbrother(@guard, @visitor)
```

Submit with `@`. The AI supplies all three arguments even though this rule only reads two. `isclanbrother` checks for **any** shared clan. If Mara also belongs to the local guild, a guild member could qualify. For a town with overlapping affiliations, test the visitor's membership of the particular watch clan instead. Supply an actual watch member and an outsider to `prog execute EastWatchAdmits <guard> <visitor> null` to see the two answers without opening the door.

Create three small Void progs using the same editor. Their complete bodies fit on one line each.

| Prog | Parameters in order | Body |
| --- | --- | --- |
| `EastWatchOpen` | `guard character, visitor character, gate exit` | `force @guard "open east"` |
| `EastWatchClose` | `guard character, visitor character, gate exit` | `force @guard "close east"` |
| `EastWatchDuty` | `guard character` | `force @guard "doorguard"` |

On a new Doorguard AI, use `ai set will EastWatchAdmits`, `ai set open EastWatchOpen` and `ai set close EastWatchClose`. Leave **Own Side Only** off for knocks from outside. Attach the AI to Mara's template with `npc set ai add <ai-name>` and run `EastWatchDuty` from her on-load initialisation. The duty command toggles her status. Call it once for a new off-duty guard. A watch member knocking west from the road should now see Mara open the gate. A stranger knocking at the closed gate should find it stays shut. Once it is open, the door itself allows passage according to the exit's rules. [Build Mara and the east gate](NPC_Builder_Guide.md#give-mara-a-door-and-an-admission-rule)

## Fulfil a custom pottery order

The Wayfarer Inn commissions either four cider mugs for its common room, or a pitcher and two mugs for table service. Both orders use the same wheel, kiln, clay and glaze. A normal item product would suffice for one fixed output. A prog product earns its place here because the *combination and count* depend on the slip the potter actually used.

Set up the recipe with clay in slot 1, four slate-blue glaze portions in slot 2 and one tagged **Pottery Order** slip in slot 3. Give the two slip prototypes distinct identities. Prepare Current, manually loadable mug, pitcher and paper kiln-query prototypes. For this version give the three output prototypes the unique names `WayfarerCiderMug`, `WayfarerTablePitcher` and `WayfarerKilnQuery`, or substitute your own. Both vessels need the `WayfarerGlaze` Characteristic. Replace `9101` with its slate-blue value ID, and `8101` and `8102` with the two **slip prototype** IDs. Keeping those local catalogue choices together makes an order change easy to review.

**`FillWayfarerPotteryOrder(inputs item collection dictionary) → item collection`**

```futureprog
var ware as item collection
var slip = collectiondictionarygetfirst(@inputs, "3")
var cups = 0
var pitcher = false
if (not(isnull(@slip)))
  if (@slip.proto == 8101)
    cups = 4
  end if
  if (@slip.proto == 8102)
    cups = 2
    pitcher = true
  end if
end if
if (@cups == 0)
  additem ware loaditem("WayfarerKilnQuery")
  return @ware
end if
var finish = "wayfarerglaze=9101"
if (@pitcher)
  additem ware loaditem("WayfarerTablePitcher", @finish)
end if
while (@cups > 0)
  additem ware loaditem("WayfarerCiderMug", @finish)
  cups = @cups - 1
end while
return @ware
```

Use `prog set parameter add inputs item collection dictionary` and `prog set return item collection`. Add a normal prog product with `craft set product add prog` and bind it with `craft set product <number> prog FillWayfarerPotteryOrder`. On a clone of the mug recipe, replace the inherited normal mug product and keep the failure potsherds. In the final success phase refer to the new product as `$p1` if it is product 1. Commit the slip before the product is due. [Complete recipe, phase echoes and input order](Crafting_System_Builder_Workflows.md#fulfil-a-pottery-order-with-a-prog-product)

**What the craft receives.** Each `loaditem` creates one real vessel. The craft takes the collection, assigns the normal output to the crafter and releases it at the craft workpiece when the craft finishes. Leave placement to the craft. Passing a quantity to a single load only increases quantity on a stackable prototype. The vessels retain their loaded quality. A recipe that should transfer craft reference quality needs to account for that when choosing its product.

A missing or unrecognised slip returns a kiln query for the steward. The committed clay and glaze have already been spent. A failed firing follows the recipe's failure branch. A broken output prototype is a third outcome: `loaditem` can raise an execution error partway through generation. The query only covers unrecognised orders. Check all three named outputs and both vessel finishes before opening the order book, especially when revising the batch.

## Issue a starting outfit

The [White Clan commencement prog](#whiteclanoncommence) packed a white bag one prototype at a time, varied the clothes according to chargen roles, then tried to put the bag in the character's hands. That is a useful distinction to preserve: **chargen decides who gets which kit; the equipment definition says what is in it**. An outfit template can now handle garment fit, load arguments, bag contents, placement and personal ownership in one issue.

Suppose recruits who start at the White Hall receive a satchel, work coat and towel. A character with the **Hall Officer** role gets a dress coat and gloves instead of the work coat. Use the role's actual name from `role list`. If your builders rename it, update the selector. Use a satchel with enough capacity for the largest permitted contents and Current clothing prototypes suited to the bodies that can start here. Give garments a white Characteristic value through the template's per-item `args` where their prototype supports it.

```text
outfittemplate new WhiteHallWorkKit
outfittemplate set item add bag <satchel-prototype> inventory
outfittemplate set item add coat <work-coat-prototype> container bag
outfittemplate set item add towel <towel-prototype> container bag
outfittemplate show WhiteHallWorkKit
outfittemplate new WhiteHallOfficerKit
outfittemplate set item add bag <satchel-prototype> inventory
outfittemplate set item add coat <dress-coat-prototype> container bag
outfittemplate set item add gloves <gloves-prototype> container bag
outfittemplate set item add towel <towel-prototype> container bag
outfittemplate show WhiteHallOfficerKit
register character WhiteHallKitIssue number
```

Use `worn` for a garment the recruit should arrive wearing. Here both coats are packed. `bag` is a key within each template, so a change of satchel prototype leaves all its content entries pointing to the right place. The register records `0` for unissued, `1` for an issue begun and `2` for an issue returned from the template loader. Give the Number register its normal zero default. The middle state matters because repeating a load can create another complete set.

**`IssueWhiteHallKit(ch character) → void`**

```futureprog
if (isnull(@ch))
  return
end if
if (isnull(@ch.location))
  return
end if
if (getregister(@ch, "WhiteHallKitIssue") != 0)
  return
end if
var kitname = "WhiteHallWorkKit"
if (@ch.roles.any(r, @r.name == "Hall Officer"))
  kitname = "WhiteHallOfficerKit"
end if
var kit = tooutfittemplate(@kitname)
if (isnull(@kit))
  send(@ch, "The issuing clerk cannot find your outfit on the roll. Please speak to staff.")
  return
end if
setregister @ch "WhiteHallKitIssue" 1
var issued = loadoutfittemplate(@kit, @ch)
setregister @ch "WhiteHallKitIssue" 2
send(@ch, "Your kit has been issued as " + @issued.name + ". Check the satchel and the floor by the issuing desk.")
```

In `chargen edit <starting-location-screen>`, bind `chargen set location commence "White Hall" IssueWhiteHallKit`. If the location already has a commencement prog, call `@IssueWhiteHallKit(@ch)` from it and keep the existing welcome actions. Use the actual parameter name of that prog. Bind the issue in one place. A second path that independently issues the same equipment can still create duplicates. [Character creation and starting locations](Character_Creation_Builder_Workflows.md#starting-locations)

**Ownership, placement and recovery.** The template assigns the recipient as owner of **each** new item, including the bag and its contents. It creates a saved outfit for the character as well. Owning the bag, holding the bag and having the coat inside it are three distinct facts. If the character cannot hold the bag it can remain at the issuing desk. An item that does not fit its intended container can remain there too, still owned by the recruit. A successful return records issue even in these cases. Move the existing piece and adjust the template for future recruits.

For a one-off hand-scripted gift, `loaditem` alone creates the item. Decide who owns it (`setownership(@item, @ch)` for a personal gift) and where it goes (`silentget(@ch, @item)` attempts pickup and returns a Boolean). If pickup fails, place it deliberately at the character's location and tell them where it is. `isowner(@item, @ch)` checks the recorded owner even when the item is on the floor. `setownership` changes one item. For a freshly assembled personal kit, `deepsetownership(@bag, @ch)` also assigns its nested contents, belt attachments and sheathed items. Use it only when all those pieces really are being transferred. A communal uniform on loan may instead belong to the clan.

If an issue stops in state `1`, inspect the character's saved outfits, held and worn items, the satchel and the issuing room before doing anything else. Complete a partial kit by supplying the missing pieces and marking the issue `2`. If nothing was created, fix the cause and reset just this character with `setregister character <character> WhiteHallKitIssue 0` before reissuing. When adding this prog to an existing world, mark previously outfitted characters as issued before a manual replay. The register guards ordinary repeats. It cannot make several item creations and a persistent save one atomic operation.

## Extend a water main

The pump house supplies drinking water. The adjacent washhouse needs a standpipe. The crew will bring pipe, work a plumbing shift and extend the **existing potable grid** across the surveyed exit. A sewer crossing the same rooms is also a Liquid grid. Checking only the grid type would let a crew connect the washhouse to the wrong service.

### Survey and store the route

Use `grid audit` to identify the potable main. This example uses ID `93`. Substitute its ID in the survey **and** in `WhyWaterMain` below. Stand in the pump house and identify the keyword for the exit to the washhouse, here `e`. The grid should already cover the pump house. A room can belong to several grids.

Use the shared progs from [Extend a utility through a player-built project](Electrical_and_Telecom_Grid_Builder_Guide.md#extend-a-utility-through-a-player-built-project). Create the six register declarations once per game. If the electrical works already use them, the water crew can use the same declarations and progs.

```text
register location GridExpansionDirections text dictionary
register location GridExpansionGrid number
register location GridConstructionOwners number dictionary
register project GridWorkGrid number
register project GridWorkTarget location
register project GridWorkDirection text
```

For a pump house with no other surveys, enter:

```text
setregister location here GridExpansionDirections (93 e)
setregister location here GridExpansionGrid 93
```

If the room already has a power survey `42 n`, set the dictionary to `(42 n 93 e)` to keep both entries. The console command replaces the dictionary. The key is the grid ID written as text. The value is the exit's keyword. A live Exit is usable during a call but cannot be saved in this register. The active project therefore remembers its own grid ID, direction and destination room.

### Give the water crew one answer and one refusal reason

Install the [shared route and reservation readers](Electrical_and_Telecom_Grid_Builder_Guide.md#write-the-permission-check-and-its-refusal-message) and the [start, release and installation progs](Electrical_and_Telecom_Grid_Builder_Guide.md#reserve-the-rooms-lay-the-line-and-release-the-reservation). Enter the named definitions once and compile them before the following water-specific progs. In particular, `GridLineProblem(site, gid, expected)` returns an empty string for a valid route. Its third argument is the reservation owner a caller expects. Zero means both rooms should be free.

**`WhyWaterMain(actor character) → text`**

```futureprog
if (isnull(@actor))
  return "A crew member must be at the worksite."
end if
var site = @actor.location
if (isnull(@site))
  return "Stand at the surveyed worksite."
end if
var gid = getregister(@site, "GridExpansionGrid")
if (@gid != 93)
  return "Select the potable water main for this plumbing job."
end if
return @GridLineProblem(@site, @gid, 0)
```

**`CanWaterMain(actor character) → boolean`**

```futureprog
return @WhyWaterMain(@actor) == ""
```

Asking about the job reads the survey. It neither changes the grid nor books a crew. The shared check verifies that the grid exists and reaches the source, exactly one marked exit leads onward, the far room lacks coverage by this grid, and the two reservation entries match the expected owner. Crew eligibility or a waterworks franchise rule belongs in `WhyWaterMain` so the displayed explanation and the Boolean stay in step.

### Reserve, complete or cancel

Create a **local** project for laying the washhouse main. Give it an appearance prog and a phase requiring pipe and plumbing labour. Bind the water-specific permission and the shared lifecycle helpers:

```text
project set can CanWaterMain
project set why WhyWaterMain
project set start StartGridLine
project set cancel ReleaseGridLine
project set finish FinishGridLine
```

| Moment | What the prog does | Why the distinction matters |
| --- | --- | --- |
| The crew starts | `StartGridLine(job)` rechecks the selected route, snapshots it on the active job, and records `job.id` against this grid on **both** rooms. | A second crew approaching the washhouse from elsewhere sees that the far room is reserved. A Boolean “busy” alone cannot tell which crew may release it. |
| The survey changes during work | The project keeps its original destination. At completion `InstallGridLine` checks the reservation owners, current survey, exit and destination against the saved job. | Editing the survey from east to south cannot silently connect the neighbouring laundry. |
| The crew finishes | `FinishGridLine(job)` calls the installer, which uses `extendgrid(@gid, @target)` after validation and releases the crew's reservations. | The built-in changes room coverage. The script supplies adjacency and service selection. Failure can leave a completed project without a new connection. A Void completion callback cannot refund its pipe or veto completion. |
| The crew cancels | `ReleaseGridLine(job)` removes an entry only if it still belongs to this project. It reads the saved far room even if the map has changed. | A departing crew can free its route without clearing another crew's reservation. |

Submit and approve the project. With the survey selected, `prog execute WhyWaterMain self` at the pump house should return empty text. The crew can then use `project start <project-name>`, supply its pipe and complete the shift. `grid audit` should show the washhouse on the **same** potable grid. Connect a compatible standpipe there and draw water. Coverage gives the room access to the network. The reservoir and standpipe provide the actual water service. [Water supply and disposal](Electrical_and_Telecom_Grid_Builder_Guide.md) · [Project labour and materials](Projects_System_Builder_Workflows.md)

If the worksite reports a reservation after a crew disappears, find the recorded active project and check both rooms before clearing anything. On ordinary cancellation the release prog handles this. Completion rechecks the route because a map edit or an independently connected washhouse can invalidate yesterday's plan.

## Patterns from existing worlds

Follow an example name to its [source listing in the appendix](#appendix-original-futureprogs).

| Authored example | Pattern worth borrowing |
| --- | --- |
| [White Clan commencement](#whiteclanoncommence) | A role selects the contents of a real starting kit. Keep the kit in an outfit template and have the commencement prog choose it. Handle any role-specific tattoos separately from repeatable equipment issue. |
| [Savannah introduction (`BlackClanIntro`)](#blackclanintro) | A shared room flag prevents two people starting the same presentation over one another. Send personal instructions to the entrant and room narration to witnesses. Decide who clears the flag if the scene is interrupted. |
| [Subway heartbeat](#subwayheartbeat) | A zone register records the station, while a Number argument records the current step of this arrival. A `switch` can announce, open and close in sequence. A delayed step should still verify that it is acting on the intended service. |
| [Dreadzone control progs](#dreadzone-control-progs) | Several small progs coordinate a longer-running area. Keep the condition for starting a phase, the change it makes and its reset separately named. For example, repeated button presses should see that activation is already underway. |
| [Redemption counter](#redeemexplosive) | A command can offer `list` and `preview` before committing a redemption. Validate the argument count before reading an option. Only the committing branch should spend the player's token or issue the selected reward. |

These scripts were written for particular worlds. Their room numbers, catalogue IDs, roles and timings are local decisions. Reuse the relationship between the caller, its state and its effects when adapting one.

### A delayed welcome at the issuing desk

The [Savannah scene](#blackclanintro) and the [subway](#subwayheartbeat) both pass a step to a later call. Here the hall porter gives a new arrival time to read the kit message before explaining where to change. The second stage checks whether the recruit is still in the hall. Leaving during the welcome ends it quietly.

**`HallWelcome(ch character, hall location, step number) → void`**

```futureprog
if (isnull(@ch) or isnull(@hall))
  return
end if
if (@ch.location != @hall)
  return
end if
switch (@step)
  case (1)
    send(@ch, "The porter points out the white satchel that goes with your posting.")
    delayprog HallWelcome 5000 @ch @hall 2
  case (2)
    send(@ch, "The changing room is through the curtained door.")
end switch
```

Call `@HallWelcome(@ch, @ch.location, 1)` from an existing welcome prog, after issue if the greeting refers to a satchel. Stage 2 queues nothing. This private message needs no shared room flag. A public performance playing to everyone in the hall would need a single start path and a room-level guard like the one in the [Savannah example](#blackclanintro).

## Troubleshooting

| What the builder sees | Where to look |
| --- | --- |
| A field refuses a compiled prog | Inspect the caller's help, return type and ordered parameter list. `Item Collection` and `Item Collection Dictionary` are distinct. |
| The editor rejects a dot property or collection expression | Inspect the type with `prog help type <type>`. Refer to the current member of an expression by its local name, such as `@r`, and check for an empty result before dereferencing. |
| A manual call works but nobody reacts in play | Inspect the binding and recipient. A Doorguard needs duty mode, an event hook needs the object that receives the event, and a craft product needs a success phase that produces it. |
| Results repeat for different people | Inspect the static setting, caller argument order and state owner. A per-room flag has different consequences from one saved on each character. |
| A loaded item is missing | Identify who owns delivery. Inspect the craft workpiece, the bag or the issuing room as appropriate. Check the execution error before issuing another copy. |
| A scheduled action runs twice or never finishes | Identify every start path, inspect its stored phase, and check that each terminal or interrupted path releases the saved state. |
| A live definition needs replacing | Search `builderrefs prog <id>` and `prog list &<name>`, compile the replacement, then update the owning editors and callers. |

For lookup while building, use the [function reference](https://futuremud.com/docs/futureprog/functions), [type reference](https://futuremud.com/docs/futureprog/types) and [collection extensions](https://futuremud.com/docs/futureprog/collections).

## Appendix: original FutureProgs

These are the progs used by the original worlds. Each listing starts with the name, ordered parameters and return type. Terminal colours, line numbers and display wrapping have been removed. The listings retain the original world IDs and behaviour. In particular, adapt the ownership and placement of newly loaded items before reusing the starting kit or reward counter.

| Look up an example | What to follow |
| --- | --- |
| [White Clan commencement](#whiteclanoncommence) | How a character’s roles change the contents of a starting bag. |
| [Savannah introduction](#blackclanintro) | How a room starts, stages and ends a shared welcome. |
| [Subway heartbeat](#subwayheartbeat) | How a timer advances a train while a zone remembers the station. |
| [Redemption counter](#redeemexplosive) | How one command handles browsing and purchase. |
| [Dreadzone control progs](#dreadzone-control-progs) | How console commands, recurring checks and delayed actions cooperate. |

### WhiteClanOnCommence

The original White Clan starting-location action selects clothes by role, packs a bag, then attempts to hand it to the new character. The final call schedules a separate White Clan introduction. The [modern outfit example](#issue-a-starting-outfit) covers personal ownership, placement and repeat issue. `WhiteClanIntro` was a separate prog in this game. The Savannah listing below is the **Black** Clan introduction.

`WhiteClanOnCommence(ch character) → void`

```futureprog
var bag as item
bag = LoadItem(829, "color1=\"pure white\"")
var newitem as item
if (@ch.Roles.Any(x, @x.Id == 18 or @x.Id == 19 or @x.Id == 22))
  // Slacks
  newitem = LoadItem(777, "color1=\"pure white\"")
  forceput(@newitem, @bag)
  // Shirt
  newitem = LoadItem(778, "color1=\"pure white\"")
  forceput(@newitem, @bag)
else
  // Trousers
  newitem = LoadItem(779, "color1=\"pure white\"")
  forceput(@newitem, @bag)
  // Dress Shirt
  newitem = LoadItem(780, "color1=\"pure white\"")
  forceput(@newitem, @bag)
  // Coat
  newitem = LoadItem(781, "color1=\"pure white\"")
  forceput(@newitem, @bag)
  // Neck Tie
  newitem = LoadItem(784, "color1=\"pure white\"")
  forceput(@newitem, @bag)
end if
if (@ch.Roles.Any(x, @x.Id == 21 or @x.Id == 23))
  // Cape
  newitem = LoadItem(782, "color1=\"pure white\"")
  forceput(@newitem, @bag)
  // Gloves
  newitem = LoadItem(783, "color1=\"pure white\"")
  forceput(@newitem, @bag)
end if
// Boots
newitem = LoadItem(776)
forceput(@newitem, @bag)
// Belt
newitem = LoadItem(12, "color1=\"pure white\"")
forceput(@newitem, @bag)
// Socks
newitem = LoadItem(26, "color1=\"pure white\"")
forceput(@newitem, @bag)
if (@ch.Gender == ToGender("Female"))
  // Panties
  newitem = LoadItem(14, "color1=\"pure white\"")
  forceput(@newitem, @bag)
  // Bra
  newitem = LoadItem(13, "color1=\"pure white\"")
  forceput(@newitem, @bag)
else
  // Briefs
  newitem = LoadItem(15, "color1=\"pure white\"")
  forceput(@newitem, @bag)
end if
// Sunglasses
newitem = LoadItem(85, "color1=\"pure white\"")
forceput(@newitem, @bag)
// Cap
newitem = LoadItem(330, "color1=\"pure white\"")
forceput(@newitem, @bag)
// Beanie
newitem = LoadItem(188, "color1=\"pure white\"")
forceput(@newitem, @bag)
// Towel
newitem = LoadItem(49, "color1=\"pure white\"")
forceput(@newitem, @bag)
// Give them the bag or drop it
if (not(silentget(@ch, @bag)))
  put(@bag, @ch.Location)
end if
if (@ch.Roles.Any(x, @x.Id == 18))
  addtattoo(@ch, 4, "neck", @ch, 100, 1.0)
end if
if (@ch.Roles.Any(x, @x.Id == 19))
  addtattoo(@ch, 6, "neck", @ch, 100, 1.0)
end if
if (@ch.Roles.Any(x, @x.Id == 20))
  addtattoo(@ch, 8, "neck", @ch, 100, 1.0)
end if
if (@ch.Roles.Any(x, @x.Id == 21))
  addtattoo(@ch, 10, "neck", @ch, 100, 1.0)
end if
if (@ch.Roles.Any(x, @x.Id == 22))
  addtattoo(@ch, 12, "neck", @ch, 100, 1.0)
end if
if (@ch.Roles.Any(x, @x.Id == 22))
  addtattoo(@ch, 14, "neck", @ch, 100, 1.0)
end if
delayprog WhiteClanIntro 8000 @ch @ch.Location 0
```

### BlackClanIntro

This is Savannah’s spoken introduction from the `Savanah_Echo` export. Follow one entrant through stage zero, leaving the room mid-scene, and the closing line. The room owns the Boolean `isvideoplaying` so a second entrant cannot start an overlapping presentation. The language and accent belong to the original setting. Compare the shorter [issuing-desk welcome](#a-delayed-welcome-at-the-issuing-desk).

`BlackClanIntro(ch character, cell location, stage number) → void`

```futureprog
var playing as boolean
playing = GetRegister(@cell, "isvideoplaying")
if (@stage == 0 and not(isnull(@playing)) and @playing == true)
  return
end if
setregister @cell "isvideoplaying" true
var accent as accent
var language as language
accent = ToAccent(57)
language = ToLanguage("English")

if (@ch.Location != @cell)
  sendloclang(@cell, "A short series of three gentle chimes, rising in pitch, precedes the sweet, homely voice of #FSAVANNAH#0, issuing at an audible volume from some hidden source nearby,\n\t\"Well, someone is busier than a moth in a mitten! Leavin' before a girl is done.\"", @language, @accent)
  sendloclang(@ch.Location, "A short series of three gentle chimes, rising in pitch, precedes the sweet, homely voice of #FSAVANNAH#0, issuing at an audible volume from some hidden source nearby,\n\t\"Well, someone is busier than a moth in a mitten! Leavin' before a girl is done. If you ever decide you want to hear the introduction again from little old me, just holler SAVANNAH, PLAY INTRODUCTION.\"", @language, @accent)
  setregister @cell "isvideoplaying" false
  return
end if

var echo as text
switch (@stage)
  case (0)
    echo = "A short series of three gentle chimes, rising in pitch, precedes the sweet, homely voice of #FSAVANNAH#0, issuing at an audible volume from some hidden source nearby,\n\t\"Well howdy " + @ch.Name + "! It's nice to see you up and about.\""
  case (1)
    echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"My name is Savannah, and you can think of me as your friend throughout this whole experience. I am an artificial intelligence built into the Lab, and this experient: well, it's my whole reason for being!\""
  case (2)
    echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"What is the experiment? Well, I can't tell you everything, but its codename is SAFARI and you don't need to be a certified genius to see why we call this place you're in the Safari Zone.\""
  case (3)
    echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"Now you may be feeling a little fuzzy in the brainpan, but don't worry, that's all normal. You will probably also feel a little sick in the breadbasket over the next few days which is a normal side effect of the cryosleep. Do let us know if the nausea and headaches don't clear up, but I'm afraid you're stuck with the amnesia! Our scientists haven't quite cracked that one yet!\""
  case (4)
    echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"You're probably wondering how you got yourself into this situation but let me tell you it's all above board but strictly classified. Just know that what you're doing here is helping all of humanity, in fact, it may just be the most important experiment ever conducted since your monkey ancestors started rubbing two sticks together! It really is that vital!\""
  case (5)
    echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"What do we want you to do? Just be you! It's important that your actions are authentic. Don't overthink it!\""
  case (6)
    echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"You are a part of the Black Clan. Think of them as your family - clan means family after all! You don't always have to like your family but they'll always be your family, no matter what. Like any family, there is a bit of a hierarchy. In this experiment we've chosen to name those positions after chess pieces but that's just a little insider joke, don't read too much into it.\""
  case (7)
    echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"There's also a White Clan. Try not to think of them as your enemy, because they're not. But they are rivals. They have their own goals and not all of them will align with your clan's goals.\""
  case (8)
    if (@ch.Roles.Any(x, @x.Id == 24))
      echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"The rank you were assigned is Pawn. I know that word has some negative connotations but try not to let it get you down. The first thing you should do is go find a Bishop. Bishops are responsible for onboarding pawns. They'll get you sorted out and help you work through any worries that might be on your mind.\""
      break
    end if
    if (@ch.Roles.Any(x, @x.Id == 25))
      echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"The rank you were assigned is Knight. The world is a dangerous place full of uncertainties and risks, and the Lab is no different. Like the Knights of old, you're going to be expected to face down challenges and adversity with courage and valour. Your entire clan are counting on you. I suggest that your first action should be to seek out a Bishop. They will give you your first quest.\""
      break
    end if
    if (@ch.Roles.Any(x, @x.Id == 26))
      echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"The rank you were assigned is Bishop. As a Bishop, you are responsible for the Pawns and the Knights first and foremost. You might just as well have been called a Shepherd, and they are your flock. You answer to the Rooks. The Rooks will interpret the will of the King and give you strategic objectives. It is up to you and your Knights and Pawns to achieve them.\""
      break
    end if
    if (@ch.Roles.Any(x, @x.Id == 27))
      echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"The rank you were assigned is Rook. This is a position of privilege, and don't you forget it! You answer to nobody but the King. The King will set your priorities, but you are responsible for thinking for yourself and turning those priorities into strategies. Your main responsibility is organising the Bishops. Try to let them do their job wherever you can, but in times of urgency you can step in and take command of the lower ranks directly.\""
      break
    end if
    if (@ch.Roles.Any(x, @x.Id == 28))
      echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"The rank you were assigned is Queen. You answer to nobody but the King, not even the Rooks. In fact, think of yourself as separate from the rest because they don't answer to you either. As a Queen, you are an agent of the King's will and the King's protector. Your number one priority is to protect the King, but from time to time the King will also give you tasks that are too important to be trusted to the rank and file.\""
      break
    end if
    if (@ch.Roles.Any(x, @x.Id == 29))
      echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"The rank you were assigned is King. Well, lucky you! You must be someone special. You are responsible for the whole clan, and it is a big responsibility. I'll be in contact with you from time to time to set goals, and it's up to you to organise your clan to achieve them. Give guidance to the Rooks, so that they can organise the rank and file. Use the Queens wisely as you may need to rely on them for your safety at times. You also have enhanced privileges, we'll talk about those later!\""
      break
    end if
  case (9)
    echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"Anyway, I think that's enough chin-wagging from little old me! I'd better let you mosey on out there and get started. Oh! I've dropped a little duffel bag of things for you to get started. Don't forget to get dressed and take it with you.\""
  case (10)
    echo = "The sweet, homely voice of #FSAVANNAH#0 continues, \"Welcome to the Lab.\""
end switch

sendloclang(@cell, @echo, @language, @accent)
if (@stage >= 0 and @stage < 10)
  delayprog BlackClanIntro 25000 @ch @cell (@stage + 1)
else
  setregister @cell "isvideoplaying" false
end if
```

### SubwayHeartbeat

One call with step 1 begins a stop. The zone remembers which station the train serves, and each queued call advances the step. Watch the station overlay appear when the doors open and the base overlay return when they close. Step 6 ends this arrival. The next arrival needs another start from the service. To adapt it, identify your carriage, platforms, overlay packages, announcement language and accent, and declare the zone’s Number register `subwaystation`.

`SubwayHeartbeat(step number) → void`

```futureprog
// Work out what station we're currently at
var station as number
station = GetRegister(ToZone(1), "subwaystation")

// First time the prog has run
if (@station == 0)
  station = 1
  SetRegister ToZone(1) "subwaystation" @station
end if

// If we're at the first step, we have to advance to the next stop
if (@step == 1)
  switch (@station)
    case (1)
      station = 2
    case (2)
      station = 3
    case (3)
      station = 1
  end switch
  // Don't forget to update the register variable on the zone
  SetRegister ToZone(1) "subwaystation" @station
end if

// This is the room that is the train carriage
var trainroom as location
trainroom = ToLocation(13)

// This is whichever room is the station's platform for which station we are at
var stationroom as location
var thisstop as text
var nextstop as text
switch (@station)
  case (1)
    stationroom = ToLocation(11)
    thisstop = "Station Two"
    nextstop = "Station Three"
  case (2)
    stationroom = ToLocation(12)
    thisstop = "Station Three"
    nextstop = "Station One"
  case (3)
    stationroom = ToLocation(10)
    thisstop = "Station One"
    nextstop = "Station Two"
end switch

// Grabbing language and accent for announcements so we don't have to keep doing this each time
var westron as language
westron = ToLanguage("Westron")
var accent as accent
accent = ToAccent("Gondorian Noble")

// Delay will be used as a timer for the next steps. In milliseconds.
var delaytime as number

// Do different things depending on which step we're at
switch (@step)
  case (1)
    sendloclang(@stationroom, "#3The station's intercom system turns on and an automated voice announces, \"The next train to arrive on the platform will be the all-stations service on the city loop. Next stop will be " + @nextstop + ". Please stand back for your safety.\"#0", @westron, @accent)
    sendloclang(@trainroom, "#3An intercom inside the train carriage announces, \"This train is now arriving at " + @thisstop + ".\"#0", @westron, @accent)
    delaytime = 15000
  case (2)
    sendloc(@stationroom, "#3A train pulls in to a stop at the platform, the screeching sound of metal on metal filling the air as it brakes.#0")
    sendloc(@trainroom, "#3The train brakes and comes to a halt at a station. With a slight hiss and a tired mechnical grind, the doors of the train open.#0")

    // Swap in the correct overlay
    // First, reset all the rooms to base - this one had no exits
    SwapOverlay("Subway System")
    // Next, swap in only the overlay with the exit we want
    switch (@station)
      case (1)
        SwapOverlay("AtStationTwo")
      case (2)
        SwapOverlay("AtStationThree")
      case (3)
        SwapOverlay("AtStationOne")
    end switch

    delaytime = 45000
  case (4)
    sendloclang(@stationroom, "#3The station's intercom system turns on and an automated voice announces, \"The train's next stop will be " + @nextstop + ". The train will shortly depart.\"#0", @westron, @accent)
    sendloclang(@trainroom, "#3From outside the carriage you can hear an automated voice announce, \"The train's next stop will be " + @nextstop + ". The train will shortly depart.\"#0", @westron, @accent)
    delaytime = 15000
  case (5)
    sendloclang(@stationroom, "#3The station's intercom system turns on and an automated voice announces, \"Please stand clear, doors closing.\"#0", @westron, @accent)
    sendloclang(@trainroom, "#3From outside the carriage you can hear an automated voice announce, \"Please stand clear, doors closing.\"#0", @westron, @accent)
    delaytime = 15000
  case (6)
    sendloc(@stationroom, "#3With a slight hiss and a tired mechnical grind, the doors of the train shut. The train quickly accelerates and departs the station down the tunnel.#0")
    sendloc(@trainroom, "#3With a slight hiss and a tired mechnical grind, the doors of the train shut. The train begins accelerating towards its destination.#0")
    // Remove the exit
    SwapOverlay("Subway System")
end switch

// Set up the next step. Don't do anything if we're at step #6
if (@step < 6)
  delayprog SubwayHeartbeat @delaytime (@step + 1)
end if
```

### RedeemExplosive

This in-game reward counter implements three branches of a command. A player can list rewards, preview an option or spend held tokens. Read the final branch to see where the purchased item goes when the recipient’s hands are full. The token and reward prototype IDs and the fallback room are specific to the original counter.

`RedeemExplosive(ch character, perc perceivable, cmd text, commands text collection) → void`

```futureprog
if (@commands.Count == 0 or @commands.Count > 2)
  send(@ch, "You can use the REDEEM LIST to view a list of redeemable explosives, REDEEM PREVIEW <option> to preview an option, or REDEEM TOKEN <option> to redeem your explosive.")
  return
end if
cmd = GetIndex(@commands, 0)
if (@cmd == "list")
  send(@ch, "You can select the following options for explosives:\n\t#3dynamite#0 - #31 token#0 - #2a stick of dynamite#0\n\t#3dynamitebundle#0 - #32 tokens#0 - #2a bundle of dynamite#0\n\t#3tnt#0 - #33 tokens#0 - #2a bundle of radio-detonated TNT#0\n\t#3detonator#0 - #31 token#0 - #2a radio detonator#0\n\t#3flaregun#0 - #31 token#0 - #2a flare gun#0\n\t#3flares#0 - #31 token#0 - #26 flares for the flaregun#0")
  return
end if
if (@cmd == "preview")
  if (@commands.Count < 2)
    send(@ch, "Which explosive did you want to preview? See REDEEM LIST for a list of keywords.")
    return
  end if
  cmd = GetIndex(@commands, 1)
  switch (@cmd)
    case ("dynamite")
      send(@ch, "A single stick of dynamite with a lightable fuse. It produces a relatively localised explosion.")
      return
    case ("dynamitebundle")
      send(@ch, "Several sticks of dynamites bundled together with a single fuse. Produces a big explosion that can affect an entire room.")
      return
    case ("tnt")
      send(@ch, "TNT with a battery-powered radio detonator. Produces a big room-size explosion and detonated remotely with the handheld detonator.")
      return
    case ("detonator")
      send(@ch, "A battery powered handheld detonator for detonating TNT.")
      return
    case ("flaregun")
      send(@ch, "A flaregun for firing flares.")
      return
    case ("flares")
      send(@ch, "A bundle of 6 flares that are ammunition for the flaregun, and which illuminate the sky for several minutes.")
      return
  end switch
  send(@ch, "Which explosive did you want to preview? See REDEEM LIST for a list of keywords.")
  return
end if
if (@cmd == "token")
  if (@commands.Count < 2)
    send(@ch, "Which explosive did you want to redeem? See REDEEM LIST for a list of keywords.")
    return
  end if
  var tokens as item
  tokens = @ch.HeldItems.First(x, @x.Proto == 857)
  if (isnull(@tokens))
    send(@ch, "You must be holding explosive tokens to redeem anything.")
    return
  end if
  cmd = GetIndex(@commands, 1)
  var item as item
  switch (@cmd)
    case ("dynamite")
      item = LoadItem(865)
      takedelete(@tokens,1)
      sendloc(@ch.Location, "$0 $0|redeem|redeems $1 for 1 explosive token.", @ch, @item)
      if (not(silentget(@ch, @item)))
        put(@item, ToLocation(422))
        send(@ch, "#1Your hands were full so the item was put on the ground.#0")
      end if
      return
    case ("dynamitebundle")
      if (@tokens.Quantity < 2)
        send(@ch, "You need to be holding at least 2 explosives tokens to redeem this option.")
        return
      end if
      takedelete(@tokens, 2)
      item = LoadItem(866)
      sendloc(@ch.Location, "$0 $0|redeem|redeems $1 for 2 explosive tokens.", @ch, @item)
      if (not(silentget(@ch, @item)))
        put(@item, ToLocation(422))
        send(@ch, "#1Your hands were full so the item was put on the ground.#0")
      end if
      return
    case ("tnt")
      if (@tokens.Quantity < 3)
        send(@ch, "You need to be holding at least 3 explosives tokens to redeem this option.")
        return
      end if
      takedelete(@tokens, 3)
      item = LoadItem(867)
      sendloc(@ch.Location, "$0 $0|redeem|redeems $1 for 3 explosive tokens.", @ch, @item)
      if (not(silentget(@ch, @item)))
        put(@item, ToLocation(422))
        send(@ch, "#1Your hands were full so the item was put on the ground.#0")
      end if
      return
    case ("detonator")
      takedelete(@tokens, 1)
      item = LoadItem(868)
      sendloc(@ch.Location, "$0 $0|redeem|redeems $1 for 1 explosive token.", @ch, @item)
      if (not(silentget(@ch, @item)))
        put(@item, ToLocation(422))
        send(@ch, "#1Your hands were full so the item was put on the ground.#0")
      end if
      return
    case ("flaregun")
      takedelete(@tokens, 1)
      item = LoadItem(870)
      sendloc(@ch.Location, "$0 $0|redeem|redeems $1 for 1 explosive token.", @ch, @item)
      if (not(silentget(@ch, @item)))
        put(@item, ToLocation(422))
        send(@ch, "#1Your hands were full so the item was put on the ground.#0")
      end if
      return
    case ("flares")
      takedelete(@tokens, 1)
      item = LoadItem(869, 6)
      sendloc(@ch.Location, "$0 $0|redeem|redeems $1 for 1 explosive token.", @ch, @item)
      if (not(silentget(@ch, @item)))
        put(@item, ToLocation(422))
        send(@ch, "#1Your hands were full so the item was put on the ground.#0")
      end if
      return
  end switch
  send(@ch, "Which explosive did you want to preview? See REDEEM LIST for a list of keywords.")
  return
end if
  send(@ch, "You can use the REDEEM LIST to view a list of redeemable explosives, REDEEM PREVIEW <option> to preview an option, or REDEEM TOKEN <option> to redeem your explosive.")
```

### Dreadzone control progs

Start at the power console to follow a player's action, or at the tick to see how the area keeps running afterwards. For a short route through the design, follow [the factory button](#batteryfactorypushbutton) into [the power-up sequence](#batteryfactorypowerup). The button checks the current state, marks activation underway and queues the first stage.

| Follow a workflow | Progs in reading order |
| --- | --- |
| Start at the console | [DreadZoneConsoleCommand](#dreadzoneconsolecommand), [DreadZonePowerUp](#dreadzonepowerup) |
| Follow the regular tick | [DreadZoneTick](#dreadzonetick), [DreadZoneActive](#dreadzoneactive), [DreadZoneCheckRobots](#dreadzonecheckrobots), [DoDreadzoneWave](#dodreadzonewave) |
| Read the two control-room stages | [CheckRobotsInControlRoom](#checkrobotsincontrolroom), [RobotsInControlRoom](#robotsincontrolroom) |
| Activate and run the factory | [BatteryFactoryPushButton](#batteryfactorypushbutton), [BatteryFactoryPowerUp](#batteryfactorypowerup), [BatteryFactoryActive](#batteryfactoryactive) |
| Follow timeout, shutdown and reset | [DreadZoneTimeout](#dreadzonetimeout), [DreadZonePowerDown](#dreadzonepowerdown), [DreadZoneReset](#dreadzonereset) |

The listings share zone registers and refer to the original control rooms, NPC templates and item prototypes. Three helpers called by these progs have no body in the export. `DreadzoneDifficulty` supplies the difficulty, `IsDreadzoneMob` identifies encounter NPCs and `EchoAnnaDreadzone` delivers the announcer’s lines. The original game’s [reward counter](#redeemexplosive) is listed separately above.

#### DreadZoneConsoleCommand

`DreadZoneConsoleCommand(ch character, perc perceivable, cmd text, commands text collection) → void`

```futureprog
if (@commands.Count < 2)
  sendloc(ToLocation(423), "$0 type|types a bunch of text into the console and hits enter, but the console merely displays an incomprehensible syntax error.", @ch)
  return
end if
if (getindex(@commands,0) == "kill")
  if (getindex(@commands,1) == "C:\MERTLAWWILLLTD\OCAWARSIM.EXE")
    if (GetRegister(ToZone(5), "dreadzoneactive") == false or GetRegister(ToZone(5), "dreadzoneactivating") or GetRegister(ToZone(5), "dreadzonedeactivating"))
      sendloc(ToLocation(423), "$0 type|types the KILL C:\MERTLAWWILLLTD\OCAWARSIM.EXE command into the console and hits enter, and the console displays a warning about error no. 921: invalid override attempt due to system state.", @ch)
      return
    end if
    if (GetRegister(ToZone(5), "dreadzonenextactiontime") > now())
      sendloc(ToLocation(423), "$0 type|types the KILL C:\MERTLAWWILLLTD\OCAWARSIM.EXE command into the console and hits enter, and the console displays a warning about error no. 7331: cannot be deactivated so soon after powerup: venting required.", @ch)
      return
    end if
    sendloc(ToLocation(423), "$0 type|types the KILL C:\MERTLAWWILLLTD\OCAWARSIM.EXE command into the console and hits enter, and the console seems to respond with...a system hang, the screen freezing up on the entry.", @ch)
    setregister ToZone(5) "dreadzonedeactivating" true
    delayprog DreadZonePowerDown 10000 1
    return
  else
    sendloc(ToLocation(423), "$0 type|types the KILL C:\MERTLAWWILLLTD\OCAWARSIM.EXE command into the console and hits enter, and the console displays a warning about error no. 71: no such running process found.", @ch)
    return
  end if
end if
if (getindex(@commands, 0) != "run")
  sendloc(ToLocation(423), "$0 type|types a bunch of text into the console and hits enter, but the console merely displays an incomprehensible syntax error.", @ch)
  return
end if
var primed as boolean
primed = GetRegister(ToZone(5), "dreadzonewarmupprimed")
switch (getindex(@commands,1))
  case ("PS_POWER_ON")
    if (GetRegister(ToZone(5), "dreadzoneactive") or GetRegister(ToZone(5), "dreadzoneactivating") or GetRegister(ToZone(5), "dreadzonedeactivating"))
     sendloc(ToLocation(423), "$0 type|types the RUN PS_POWER_ON command into the console and hits enter, and the console displays a warning about error no. 1121: system is in invalid state.", @ch)
     return
    end if
    if (@primed)
     sendloc(ToLocation(423), "$0 type|types the RUN PS_POWER_ON command into the console and hits enter, and the console displays a warning about error no. 1476: power station circuits already primed.", @ch)
     return
    end if
    sendloc(ToLocation(423), "$0 type|types the RUN PS_POWER_ON command into the console and hits enter, and the console grinds for a few seconds without acknowledgement. Then, the system status indicator blinks green. It displays a message indicating the the power station circuits are primed.", @ch)
    setregister ToZone(5) "dreadzonewarmupprimed" true
    return
  case ("C:\MERTLAWWILLLTD\OCAWARSIM.EXE")
    if (GetRegister(ToZone(5), "dreadzoneactive") or GetRegister(ToZone(5), "dreadzoneactivating") or GetRegister(ToZone(5), "dreadzonedeactivating"))
     sendloc(ToLocation(423), "$0 type|types the RUN PS_POWER_ON command into the console and hits enter, and the console displays a warning about error no. 1121: system is in invalid state.", @ch)
     return
    end if
    if (not(@primed))
      sendloc(ToLocation(423), "$0 type|types the RUN C:\MERTLAWWILLLTD\OCAWARSIM.EXE command into the console and hits enter, and the console displays a warning about error no. 5647: power station circuits are not yet primed.", @ch)
      return
    end if
    sendloc(ToLocation(423), "$0 type|types the RUN C:\MERTLAWWILLLTD\OCAWARSIM.EXE command into the console and hits enter, and the console grinds for a few seconds before making what an affirmative sounding bell. It displays a message indicating the the power station startup sequence has commenced.", @ch)
    setregister ToZone(5) "dreadzonewarmupprimed" true
    setregister ToZone(5) "dreadzoneactivating" true
    delayprog DreadZonePowerUp 10000 1
    return
end switch
sendloc(ToLocation(423), "$0 type|types a bunch of commands into the console and hits enter, but the console merely displays an incomprehensible syntax error.", @ch)
return
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### DreadZonePowerUp

`DreadZonePowerUp(stage number) → void`

```futureprog
var language as language
var accent as accent
language = ToLanguage("English")
accent = ToAccent(56)
var delaytime as number
switch (@stage)
  case (1)
    sendloc(ToLocation(423), "The console makes a series of grinding and whirring sounds as internal hardware begins powering up.")
    delaytime = 10000
  case (2)
    sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. I have detected that the Dread Zone experiment is being activated. Good luck and have fun! Thank you for your participation.\"", ToLanguage("English"), ToAccent(235))
    sendzonelang(ToZone(2), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. I have detected that the Dread Zone experiment is being activated. Good luck and have fun! Thank you for your participation.\"", ToLanguage("English"), ToAccent(235))
    sendzonelang(ToZone(4), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. I have detected that the Dread Zone experiment is being activated. Good luck and have fun! Thank you for your participation.\"", ToLanguage("English"), ToAccent(235))
    delaytime = 10000
  case (3)
    sendloc(ToLocation(423), "Arcane and incomprehensible computer code flashes by on the computer screen faster than any human could possibly read it, as the console continues to grind and whir.")
    delaytime = 18000
  case (4)
    sendzone(ToZone(5), "The overhead lights flicker off for a moment, and then restart with a shudder that can be felt through the ground.")
    delaytime = 13000
  case (5)
    sendloclang(ToLocation(423), "As pages and pages of convoluted computer code scroll by on the console's monitor, an electronic voice emanates from the speaker,\n\"Welcome to the Dread Zone Module.\"", @language, @accent)
    delaytime = 15000
  case (6)
    sendloclang(ToLocation(423), "The electronic voice begins:\n\t\"The...\"\n...followed by an electronic screech of static noise, before continuing again...\n\t\"...Company directs your attention to the lack of any warranty for this unit, implied or otherwise.\"", @language, @accent)
    delaytime = 15000
  case (7)
    sendloclang(ToLocation(423), "The electronic voice continues:\n\t\"Use at your own risk.\"", @language, @accent)
    delaytime = 15000
  case (8)
    sendloclang(ToLocation(423), "The electronic voice continues:\n\t\"This Dread Zone unit has been fitted with a safety module that will power down after 12 hours, in the case of unforseen accidents.\"", @language, @accent)
    delaytime = 15000
  case (9)
    sendzone(ToZone(5), "The entire ground shudders for around three seconds, and the deep, bass-noted tone of what sounds like something far below the ground impacting another similarly big thing cannot fail to be heard and felt by all.")
    sendzone(ToZone(2), "The entire ground shudders for around three seconds, and the deep, bass-noted tone of what sounds like something far below the ground impacting another similarly big thing cannot fail to be heard and felt by all.")
    sendzone(ToZone(4), "The entire ground shudders for around three seconds, and the deep, bass-noted tone of what sounds like something far below the ground impacting another similarly big thing cannot fail to be heard and felt by all.")
    delaytime = 15000
  case (10)
    sendloclang(ToLocation(423), "The electronic voice continues:\n\t\"This Dread Zone unit has been upgraded to include the escalation module, which means that the enemies will get progressively harder over time. The difficulty will escalate every 5 minutes.\"", @language, @accent)
    delaytime = 15000
  case (11)
    var dz as zone
    dz = ToZone(5)
    sendloclang(ToLocation(423), "The electronic voice continues:\n\t\"The power plant module has now been successfully activated.\"", @language, @accent)
    delaytime = 25000
    setregister @dz "dreadzoneactive" true
    setregister @dz "dreadzoneactivating" false
    setregister @dz "dreadzonedeactivating" false
    setregister @dz "dreadzonenextactiontime" (now() + 1h)
    setregister @dz "dreadzonemanualdeactivationtime" (now() + 6h)
    setregister @dz "dreadzoneactivationrealtime" now()
    var armoury as location
    armoury = ToLocation(422)
    foreach (item in @armoury.Items.Where(x, @x.Proto == 25 or @x.Proto == 45 or @x.Proto == 105 or @x.Proto == 212 or @x.Proto == 214 or @x.Proto == 215 or @x.Proto == 305))
      seton(@item, true, true)
    end for
    armoury = ToLocation(421)
    foreach (item in @armoury.Items.Where(x, @x.Proto == 25 or @x.Proto == 45 or @x.Proto == 105 or @x.Proto == 212 or @x.Proto == 214 or @x.Proto == 215 or @x.Proto == 305))
      seton(@item, true, true)
    end for
    foreach (item in @dz.Items.Where(x, @x.Proto == 296))
      seton(@item, true, true)
    end for
  case (12)
    sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Attention subjects. I must inform you that the robots have had their hunter-killer protocol activated. Stay safe out there!\"", ToLanguage("English"), ToAccent(235))
    setregister ToZone(5) "killmodeactive" true
  default
    break
end switch
if (@stage < 13)
  delayprog DreadZonePowerUp @delaytime (@stage + 1)
end if
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### DreadZoneTick

`DreadZoneTick(ch character) → void`

```futureprog
var dz as zone
dz = ToZone(5)
if (GetRegister(@dz, "dreadzoneactive") and not(GetRegister(@dz, "dreadzonedeactivating")))
  @DreadzoneActive(@dz)
  if (GetRegister(@dz, "batteryfactoryactive"))
    @BatteryFactoryActive(@dz)
  end if
end if
@DreadZoneCheckRobots()
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### DreadZoneActive

`DreadZoneActive(dz zone) → void`

```futureprog
if (GetRegister(@dz, "dreadzonemanualdeactivationtime") < now())
  @DreadZoneTimeout()
  return
end if
@CheckRobotsInControlRoom()
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### DreadZoneCheckRobots

`DreadZoneCheckRobots() → void`

```futureprog
var dz as zone
dz = ToZone(5)

// Make sure there is at least one dreadzone roomba wandering around
if (@dz.Characters.Where(x, @x.Race.Id == 3).Count < 1)
  loadnpc(70, ToLocation(417))
end if

if (not(GetRegister(@dz, "dreadzoneactive")))
  // Spawn ant-bots if less than 2 people are in the zone and it's inactive
  if (random(1,50) == 1 and @dz.Characters.Where(x, @x.Race.Id == 12).Count < 2)
    if (random(1,2) == 1)
      loadnpc(33, ToLocation(261))
    else
      loadnpc(33, ToLocation(301))
    end if
  end if

  // Kill any robots in the maintenance bays
  foreach (ch in ToLocation(460).Characters.Where(x, @IsDreadzoneMob(@x)))
    kill(@ch, false)
  end foreach
  foreach (ch in ToLocation(465).Characters.Where(x, @IsDreadzoneMob(@x)))
    kill(@ch, false)
  end foreach
  // If inactive, we're done
  return
end if

// Check to see if there are no kill-bots, i.e. the zone is clear
if (@dz.Characters.Where(x, @IsDreadzoneMob(@x)).Count <= 0 and not(GetRegister(@dz, "dreadzonewaveactive")))
  // Start a new wave
  @DoDreadzoneWave(0)
  return
end if
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### DoDreadzoneWave

`DoDreadzoneWave(stage number) → void`

```futureprog
var dz as zone
dz = ToZone(5)
if (@stage == 0)
  setregister @dz "dreadzonewaveactive" true
  @echoannadreadzone(("Attention test subjects! Another wave will be starting in two minutes. The current difficulty level is at " + ToText(@dreadzonedifficulty()) +  "."))
  delayprog dodreadzonewave 60000 1
  return
end if
var random as number
random = random(1,10) + @dreadzonedifficulty()
var bfcell as location
bfcell = ToLocation(465)
var ppcell as location
ppcell = ToLocation(460)
var finalcell as location
finalcell = @ppcell
switch (@random)
  case (1)
    // Easy Puppies
    @echoannadreadzone("The next wave will be an adorable assortment of puppies! Don't let their cute little paws fool you, they're deadly. These ones are coming for the battery factory.")
    LoadNpc(67, @bfcell)
    LoadNpc(67, @bfcell)
    LoadNpc(67, @bfcell)
    LoadNpc(67, @bfcell)
    break
  case (2)
    // Easy Puppies
    @echoannadreadzone("The next wave will be an adorable assortment of puppies! Don't let their cute little paws fool you, they're deadly. These ones are coming for the power plant.")
    LoadNpc(67, @ppcell)
    LoadNpc(67, @ppcell)
    LoadNpc(67, @ppcell)
    LoadNpc(67, @ppcell)
    break
  case (3)
    // Puppies and Handsbot
    @echoannadreadzone("The next wave will be an adorable assortment of puppies and their robot friends! Watch out, they're as deadly as they are cute!")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(67, @finalcell)
    LoadNpc(67, @finalcell)
    LoadNpc(67, @finalcell)
    LoadNpc(26, @finalcell)
    break
  case (4)
    // Handbots and Shieldbot
    @echoannadreadzone("The next wave will be a little bit of a HANDFUL. I hope you can HANDle it.")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(26, @finalcell)
    LoadNpc(26, @finalcell)
    LoadNpc(66, @finalcell)
    LoadNpc(26, @finalcell)
    break
  case (5)
    // Handbots and Hammerbot
    @echoannadreadzone("The next wave will pack a little bit of a punch, so watch out test subjects!")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(26, @finalcell)
    LoadNpc(26, @finalcell)
    LoadNpc(19, @finalcell)
    LoadNpc(26, @finalcell)
    break
  case (6)
    // Handbots, Shieldbot and Hammerbot
    @echoannadreadzone("The next wave is nicely balanced. But with good teamwork, I'm sure you can overcome it!")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(26, @finalcell)
    LoadNpc(26, @finalcell)
    LoadNpc(19, @finalcell)
    LoadNpc(66, @finalcell)
    break
  case (7)
    // Handbots, Sawbot and Hammerbot
    @echoannadreadzone("The next wave is a classic. A little bit of all the old favourites.")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(26, @finalcell)
    LoadNpc(26, @finalcell)
    LoadNpc(19, @finalcell)
    LoadNpc(18, @finalcell)
    break
  case (8)
    // Two Hammerbots and a Shield bot
    @echoannadreadzone("The next wave is a real knock-out punch.")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(19, @finalcell)
    LoadNpc(66, @finalcell)
    LoadNpc(19, @finalcell)
    break
  case (9)
    // Robot lions
    @echoannadreadzone("The next wave is a wild one. I hope none of you get caught LION around!")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(68, @finalcell)
    LoadNpc(68, @finalcell)
    LoadNpc(68, @finalcell)
    break
  case (10)
    // Sword bot, 2 x Shield Bot
    @echoannadreadzone("The next wave will be pretty tanky. I call it sword and board!")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(66, @finalcell)
    LoadNpc(66, @finalcell)
    LoadNpc(65, @finalcell)
    break
  case (11)
    // 2 pairs of hammerbot/sawbot
    @echoannadreadzone("The next wave is cut in two - half and half. Perhaps you test subjects should do the same!")
    LoadNpc(18, @ppcell)
    LoadNpc(19, @ppcell)
    LoadNpc(18, @bfcell)
    LoadNpc(19, @bfcell)
    break
  case (12)
    // 2 pairs of hammerbot/swordbot
    @echoannadreadzone("The next wave is slashed into two parts. Perhaps you test subjects should do the same!")
    LoadNpc(65, @ppcell)
    LoadNpc(19, @ppcell)
    LoadNpc(65, @bfcell)
    LoadNpc(19, @bfcell)
    break
  case (13)
    // Hippo
    @echoannadreadzone("The next wave is a hippo. Hmm, that can't be right. Let me check my notes...yes, it says here, just a hippo.")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(69, @finalcell)
    break
  case (14)
    // All Five bots
    @echoannadreadzone("The next wave will be a little bit of all the robots. Variety is the spice of life, after all!")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(18, @finalcell)
    LoadNpc(66, @finalcell)
    LoadNpc(19, @finalcell)
    LoadNpc(65, @finalcell)
    LoadNpc(26, @finalcell)
    break
  default
    // All Five bots
    @echoannadreadzone("The next wave will be a little bit of all the robots. Variety is the spice of life, after all!")
    if (random(1,2)==1)
      finalcell = @bfcell
    end if
    LoadNpc(18, @finalcell)
    LoadNpc(66, @finalcell)
    LoadNpc(19, @finalcell)
    LoadNpc(65, @finalcell)
    LoadNpc(26, @finalcell)
    break
end switch
setregister @dz "dreadzonewaveactive" false
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### CheckRobotsInControlRoom

`CheckRobotsInControlRoom() → void`

```futureprog
if (GetRegister(ToZone(5), "dreadzonedeactivating"))
  return
end if
var bf as location
var pp as location
bf = ToLocation(267)
pp = ToLocation(423)
if (@bf.Characters.Any(x, @IsDreadzoneMob(@x)) and Not(GetRegister(ToZone(5), "robotsincontrolroom")))
  @RobotsInControlRoom(1, true)
  return
end if
if (@pp.Characters.Any(x, @IsDreadzoneMob(@x)) and Not(GetRegister(ToZone(5), "robotsincontrolroom")))
  @RobotsInControlRoom(1, false)
  return
end if
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### RobotsInControlRoom

`RobotsInControlRoom(stage number, isbattery boolean) → void`

```futureprog
if (@stage == 1)
  setregister ToZone(5) "robotsincontrolroom" true
else
  var bf as location
  var pp as location
  bf = ToLocation(267)
  pp = ToLocation(423)
  var robot as race
  robot = ToRace(8)
  if (@isbattery and not(@bf.Characters.Any(x, SameRace(@x.Race, @robot))))
    sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"I'mdetecting that the killer robot problem has been all sorted. Good going subjects!\"", ToLanguage("English"), ToAccent(235))
    setregister ToZone(5) "robotsincontrolroom" false
    return
  end if
  if (not(@isbattery) and not(@pp.Characters.Any(x, SameRace(@x.Race, @robot))))
    sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"I'm detecting that the killer robot problem has been all sorted. Good going subjects!\"", ToLanguage("English"), ToAccent(235))
    setregister ToZone(5) "robotsincontrolroom" false
    return
  end if
end if
switch (@stage)
  case (1)
    if (@isbattery)
      sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. I have detected killer robots in the Battery Factory. Their presence in the factory substantially increases the risk of catastrophic explosion. If they are not removed soon, I'm afraid I'll have to cut the experiment short. I'm sorry!\"", ToLanguage("English"), ToAccent(235))
    else
      sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. I have detected killer robots in the Power Plant. Their presence in the power plant substantially increases the risk of catastrophic explosion. If they are not removed soon, I'm afraid I'll have to cut the experiment short. I'm sorry!\"", ToLanguage("English"), ToAccent(235))
    end if
  case (2)
    if (@isbattery)
      sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. This is just a friendly reminder about the killer robot problem in the battery factory.\"", ToLanguage("English"), ToAccent(235))
    else
      sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. This is just a friendly reminder about the killer robot problem in the power plant.\"", ToLanguage("English"), ToAccent(235))
    end if
  case (3)
    sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"I'm sorry subjects, the risk of explosion is simply too high. I am deactivating the Dread Zone experiment. I'm sorry for any inconvenience.\"", ToLanguage("English"), ToAccent(235))
    setregister ToZone(5) "robotsincontrolroom" false
    setregister ToZone(5) "dreadzonedeactivating" true
    delayprog DreadZonePowerDown 1000 1
end switch
if (@stage == 1 or @stage == 2)
  delayprog RobotsInControlRoom 60000 (@stage + 1) @isbattery
end if
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### BatteryFactoryPushButton

`BatteryFactoryPushButton(ch character, perc perceivable, cmd text, commands text collection) → void`

```futureprog
if (@commands.Count == 0)
  send(@ch, "Push what?")
  return
end if
if (getindex(@commands,0) != "button")
  send(@ch, "Buttons are the only sort of thing you can push around here.")
  return
end if
var dz as zone
dz = ToZone(5)
if (not(GetRegister(@dz, "dreadzoneactive")) or GetRegister(@dz, "batteryfactoryactive") or GetRegister(@dz, "batteryfactoryactivating"))
  sendloc(ToLocation(267), "$0 $0|push|pushes the button...but nothing seems to happen.", @ch, @perc)
  return
end if
sendloc(ToLocation(267), "$0 $0|push|pushes the button...and the sound of something mechanical moving underneath can be heard.", @ch, @perc)
setregister @dz "batteryfactoryactivating" true
delayprog BatteryFactoryPowerUp 10000 1
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### BatteryFactoryPowerUp

`BatteryFactoryPowerUp(stage number) → void`

```futureprog
var language as language
var accent as accent
language = ToLanguage("English")
accent = ToAccent(56)
var delaytime as number
switch (@stage)
  case (1)
    sendloc(ToLocation(267), "A plink and a plonk can be heard below as something moves about below.")
	delaytime = 10000
  case (2)
    sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. I have detected that the Battery Plant in the Dread Zone is now active. For your safety, the Dread Zone will now be isolated from the rest of the lab. It will remain so until the experiment is powered down. Thank you for your participation.\"", ToLanguage("English"), ToAccent(235))
	sendzonelang(ToZone(2), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. I have detected that the Battery Plant in the Dread Zone is now active. For your safety, the Dread Zone will now be isolated from the rest of the lab. It will remain so until the experiment is powered down. Thank you for your participation.\"", ToLanguage("English"), ToAccent(235))
	sendzonelang(ToZone(4), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. I have detected that the Battery Plant in the Dread Zone is now active. For your safety, the Dread Zone will now be isolated from the rest of the lab. It will remain so until the experiment is powered down. Thank you for your participation.\"", ToLanguage("English"), ToAccent(235))
	delaytime = 10000
        var exit as exit
        exit = ToLocation(121).Exits.First(x, @x.destination.Id == 124)
        setlocked(@exit.Door, true, true)
  case (3)
	sendloc(ToLocation(267), "You think you can hear the sound of servos whirring below, but it's quite indistinct.")
	delaytime = 18000
  case (4)
	sendzone(ToZone(5), "The battery charging units suddenly start to hum with energy, and the massive and complicated piece of machinery begins to make clanking noises.")
	delaytime = 13000
  case (5)
	sendloclang(ToLocation(267), "An electronic voice emanates from somewhere nearby,\n\t\"The battery factory module has now been successfully activated.\"", @language, @accent)
	delaytime = 22000
        setregister ToZone(5) "batteryfactoryactive" true
        setregister ToZone(5) "batteryfactoryactivating" false
        setregister ToZone(5) "batteryfactorydeactivating" false
        setregister ToZone(5) "batteryfactorynextactiontime" (now() + 1h)
        setregister ToZone(5) "batterytick" 0
        var loc as location
        loc = ToLocation(265)
        foreach (item in @loc.Items.Where(x, @x.Proto == 25 or @x.Proto == 45 or @x.Proto == 105 or @x.Proto == 212 or @x.Proto == 214 or @x.Proto == 215 or @x.Proto == 305))
          seton(@item, true, true)
        end for
        loc = ToLocation(268)
        foreach (item in @loc.Items.Where(x, @x.Proto == 25 or @x.Proto == 45 or @x.Proto == 105 or @x.Proto == 212 or @x.Proto == 214 or @x.Proto == 215 or @x.Proto == 305))
          seton(@item, true, true)
        end for
end switch
if (@stage < 5)
  delayprog BatteryFactoryPowerUp @delaytime (@stage + 1)
end if
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### BatteryFactoryActive

Follow a reward from its selection through loading, filling and placement. In the supplied listing, case 16 fills the “Magic Juice” item and exits the branch before placing it. When adapting that reward, put the filled item into the delivery container as case 14 does.

`BatteryFactoryActive(dz zone) → void`

```futureprog
var batterytick as number
batterytick = GetRegister(ToZone(5), "batterytick")
if (@batterytick > 12)
  var bf as location
  bf = ToLocation(267)
  setregister ToZone(5) "batterytick" 0
  sendloc(@bf, "With a clutter and a clank, an item drops out of a massive piece of complicated-looking machinery and falls into its basket.")
  var random as number
  var item as item
  var container as item
  container = @bf.Items.First(x, @x.Proto == 271)
  random = Random(1,10) + @DreadZoneDifficulty()
  switch (@random)
    case (1)
      // 4 x AAA Batteries
    item = LoadItem(201,4)
    forceput(@item,@container)
    break
    case (2)
      // 4 x C Batteries
      item = LoadItem(198,4)
    forceput(@item,@container)
    break
    case (3)
      // Container of gasoline
      item = LoadItem(677)
    forceput(@item,@container)
    break
    case (4)
    item = LoadCoins(ToCurrency(2), "1 day")
    forceput(@item, @container)
    break
    case (5)
      // 3 x 9mm Round
      item = LoadItem(206,3)
    forceput(@item,@container)
    break
    case (6)
      // 2 x 5.56mm Round
      item = LoadItem(286,2)
    forceput(@item,@container)
    break
    case (7)
      item = LoadCoins(ToCurrency(2), "36 hours")
    forceput(@item, @container)
    break
    case (8)
      // 7.62mm Round
      item = LoadItem(297)
    forceput(@item,@container)
    break
    case (9)
      // Healing Food
    item = LoadItem(858)
    forceput(@item,@container)
    break
    case (10)
      // 2 x 7.62mm Rounds, 2 x 5.56mm Rounds, 3 x 9mm Rounds
      item = LoadItem(297, 2)
    forceput(@item,@container)
    item = LoadItem(286, 2)
    forceput(@item,@container)
    item = LoadItem(206, 3)
    forceput(@item,@container)
    break
    case (11)
      item = LoadCoins(ToCurrency(2), "2 days")
    forceput(@item, @container)
    break
    case (12)
      // Explosive Token
      item = LoadItem(857)
    forceput(@item,@container)
    break
    case (13)
      item = LoadCoins(ToCurrency(2), "60 hours")
    forceput(@item, @container)
    break
    case (14)
      // Liquid Staminex
      item = LoadItem(861)
    SetLiquid(@item, 39)
    forceput(@item, @container)
    break
    case (15)
      item = LoadCoins(ToCurrency(2), "3 days")
    forceput(@item, @container)
    break
    case (16)
      // Magic Juice
      item = LoadItem(854)
    SetLiquid(@item, 54)
    break
    case (17)
      // 2 x Healing Food
    item = LoadItem(858, 2)
    forceput(@item,@container)
    break
    case (18)
      // 2 Explosive Tokens
      item = LoadItem(857, 2)
    forceput(@item,@container)
    break
    case (19)
      item = LoadCoins(ToCurrency(2), "4 days")
    forceput(@item, @container)
    break
    case (20)
      // Shyallapan
      item = LoadItem(860)
    SetLiquid(@item, 43)
    forceput(@item, @container)
    break
    break
    case (21)
      // Pet token
      item = LoadItem(856)
    forceput(@item,@container)
    break
    case (22)
      item = LoadItem(857, 3)
    forceput(@item,@container)
    break
    case (23)
      item = LoadCoins(ToCurrency(2), "5 days")
    forceput(@item, @container)
    break
    case (24)
      item = LoadItem(853)
    forceput(@item,@container)
    break
    case (25)
      item = LoadItem(853)
    forceput(@item,@container)
    break
    case (26)
      item = LoadCoins(ToCurrency(2), "7 days")
    forceput(@item, @container)
    break
    case (27)
      item = LoadCoins(ToCurrency(2), "10 days")
    forceput(@item, @container)
    break
    case (28)
      // Slap patches
      item = LoadItem(862)
    forceput(@item,@container)
    break
    case (29)
      item = LoadItem(863)
    forceput(@item,@container)
    break
    case (30)
      item = LoadItem(857, 4)
    forceput(@item,@container)
    break
    case (31)
      item = LoadCoins(ToCurrency(2), "15 days")
    forceput(@item, @container)
    break
    case (32)
    item = LoadCoins(ToCurrency(2), "20 days")
    forceput(@item, @container)
    break
    default
    item = LoadCoins(ToCurrency(2), "25 days")
    forceput(@item, @container)
    break
  end switch
  return
end if
setregister ToZone(5) "batterytick" (@batterytick + 1)
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### DreadZoneTimeout

`DreadZoneTimeout() → void`

```futureprog
sendzonelang(ToZone(5), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. The Dreadzone experiment has been running a little long, so I have commenced the deactivation procedure. I hope everyone is okay!\"", ToLanguage("English"), ToAccent(235))
sendzonelang(ToZone(2), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. The Dreadzone experiment has been running a little long, so I have commenced the deactivation procedure. I hope everyone is okay!\"", ToLanguage("English"), ToAccent(235))
sendzonelang(ToZone(4), "The husky, reassuring voice of #FA.N.N.A.#0 emanates from nearby, saying\n\t\"Hello Subjects. The Dreadzone experiment has been running a little long, so I have commenced the deactivation procedure. I hope everyone is okay!\"", ToLanguage("English"), ToAccent(235))
setregister ToZone(5) "dreadzonedeactivating" true
setregister ToZone(5) "batteryfactorydeactivating" true
setregister ToZone(5) "dreadzonemanualdeactivationtime" (now() + 12h)
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### DreadZonePowerDown

`DreadZonePowerDown(stage number) → void`

```futureprog
var language as language
var accent as accent
language = ToLanguage("English")
accent = ToAccent(56)
var delaytime as number
switch (@stage)
  case (1)
    sendloc(ToLocation(423), "The console makes a loud thud like something heavy stopped suddenly, and then makes a beep boop sound.")
    delaytime = 10000
  case (2)
    sendloclang(ToLocation(423), "An electronic voice emanates from the speaker,\n\"The Dread Zone module is being deactivated. Please stand by while we safely power everything down.\"", @language, @accent)
    sendloclang(ToLocation(267), "An electronic voice emanates from somewhere nearby,\n\"The Dread Zone module is being deactivated. Please stand by while we safely power everything down.\"", @language, @accent)
    delaytime = 10000
  case (3)
    sendzone(ToZone(5), "Every hair on your body stands on end momentarily, as if there were a large static charge nearby.")
    delaytime = 18000
  case (4)
    sendzone(ToZone(5), "The overhead lights flicker off for a moment, and then restart with a shudder that can be felt through the ground.")
    delaytime = 13000
  case (5)
    sendloclang(ToLocation(423), "An electronic voice emanates from the speaker,\n\"The Dread Zone module has now been powered down. It may take several hours for the system to reset. Thank you for choosing the Dread Zone.\"", @language, @accent)
    sendloclang(ToLocation(267), "An electronic voice emanates from somewhere nearby,\n\"The Dread Zone module has now been powered down. It may take several hours for the system to reset. Thank you for choosing the Dread Zone.\"", @language, @accent)
    @DreadZoneReset()
  default
    break
end switch
if (@stage < 5)
  delayprog DreadZonePowerDown @delaytime (@stage + 1)
end if
```

[Back to the Dreadzone index](#dreadzone-control-progs)

#### DreadZoneReset

`DreadZoneReset() → void`

```futureprog
var dz as zone
dz = ToZone(5)
setregister @dz "dreadzoneactive" false
setregister @dz "dreadzoneactivating" false
setregister @dz "dreadzonedeactivating" false
setregister @dz "batteryfactoryactive" false
setregister @dz "batteryfactoryactivating" false
setregister @dz "batteryfactorydeactivating" false
setregister @dz "killmodeactive" false
setregister @dz "robotsincontrolroom" false
setregister @dz "dreadzonewarmupprimed" false
setregister @dz "dreadzonenextactiontime" (now() + 2h)
setregister @dz "dreadzoneactivationrealtime" now()
var exit as exit
exit = ToLocation(121).Exits.First(x, @x.destination.Id == 124)
setlocked(@exit.Door, false, true)
var items as item collection
items = @dz.Items.Where(x, @x.Proto == 296 or @x.Proto == 25 or @x.Proto == 45 or @x.Proto == 105 or @x.Proto == 212 or @x.Proto == 214 or @x.Proto == 215 or @x.Proto == 305)
foreach (item in @items)
  seton(@item, false, true)
end for
```

[Back to the Dreadzone index](#dreadzone-control-progs)
