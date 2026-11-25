---
slug: futureprog
category: Builder tools
summary: Write and attach FutureProgs for gate admission, custom craft products, starting outfits and water main construction. Learn signatures, collections, events, registers and item ownership.
---

# FutureProg for builders

A builder writes a **prog** when a game system needs a rule or action particular to their world. A gatekeeper needs to know whom to admit. A pottery recipe needs to turn a customer's order into the right vessels. The systems already know how to answer a knock and finish a firing. The prog supplies the part the builder decides.

Start with the caller and the language below, then follow the [east gate](#admit-a-visitor-at-the-east-gate), [custom pottery order](#fulfil-a-custom-pottery-order), [starting outfit](#issue-a-starting-outfit) or [water main extension](#extend-a-water-main). Each named source block gives its parameters in editor order and its return type. Substitute your world's catalogue names and IDs where indicated. If you have already built the gate or kiln from its companion guide, inspect the existing named progs before creating replacements.

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

The White Clan commencement prog packed a white bag one prototype at a time, varied the clothes according to chargen roles, then tried to put the bag in the character's hands. That is a useful distinction to preserve: **chargen decides who gets which kit; the equipment definition says what is in it**. An outfit template can now handle garment fit, load arguments, bag contents, placement and personal ownership in one issue.

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

| Authored example | Pattern worth borrowing |
| --- | --- |
| White Clan commencement | A role selects the contents of a real starting kit. Keep the kit in an outfit template and have the commencement prog choose it. Handle any role-specific tattoos separately from repeatable equipment issue. |
| Savannah introduction (`Savanah_Echo`) | A shared room flag prevents two people starting the same presentation over one another. Send personal instructions to the entrant and room narration to witnesses. Decide who clears the flag if the scene is interrupted. |
| Subway | A zone register records the station, while a Number argument records the current step of this arrival. A `switch` can announce, open and close in sequence. A delayed step should still verify that it is acting on the intended service. |
| Dreadzone | Several small progs coordinate a longer-running area. Keep the condition for starting a phase, the change it makes and its reset separately named. For example, repeated button presses should see that activation is already underway. |
| Redemption counter | A command can offer `list` and `preview` before committing a redemption. Validate the argument count before reading an option. Only the committing branch should spend the player's token or issue the selected reward. |

These scripts were written for particular worlds. Their room numbers, catalogue IDs, roles and timings are local decisions. Reuse the relationship between the caller, its state and its effects when adapting one.

### A delayed welcome at the issuing desk

The Savannah scene and the subway both pass a step to a later call. Here the hall porter gives a new arrival time to read the kit message before explaining where to change. The second stage checks whether the recruit is still in the hall. Leaving during the welcome ends it quietly.

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

Call `@HallWelcome(@ch, @ch.location, 1)` from an existing welcome prog, after issue if the greeting refers to a satchel. Stage 2 queues nothing. This private message needs no shared room flag. A public performance playing to everyone in the hall would need a single start path and a room-level guard like the one in the Savannah example.

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
