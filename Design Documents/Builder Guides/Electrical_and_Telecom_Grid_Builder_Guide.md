---
slug: electrical-and-telecom-grids
category: Technology
summary: Build city and expedition power, water and sewage systems, connect telephone exchanges, and let crews extend utilities with projects and crafts.
---

# Electrical, telecommunications and liquid grids

A grid has a **creator**, a set of **rooms**, and **equipment** connected to it. Coverage makes a service possible in a room. What players can actually draw depends on the equipment. An electrical feeder supplies watts, a telephone feeder powers an exchange, and a liquid supplier contributes the contents of a reservoir. A street can belong to four separate grids for power, telephones, drinking water and sewage.

Start with the [two-room electrical installation](#supply-the-office-from-the-plant) and [telephone exchange](#power-the-exchange-and-place-a-call), or go directly to [city and expedition power](#choose-how-a-settlement-gets-its-electricity), [water and sewage](#build-a-water-main-and-a-separate-sewer), or [player-built extensions](#extend-a-utility-through-a-player-built-project).

## Build the equipment

For this example use a plant room with an east exit into an office. Give each fixture a distinct noun or keyword so commands such as `connect feeder generator` identify the intended pair. Load the creators in the plant room. Deleting a creator deletes its grid.

Use `comp typehelp <type>` to inspect a component. Create a component revision with `comp edit new <type>`, give it a unique `comp set name`, configure it and `comp edit submit`. Have a reviewer use `comp review <id>` and `accept edit <comment>`. Reuse Current components with the settings below where available.

| Item | Components on that item | Settings for the example |
| --- | --- | --- |
| Electrical panel | `ElectricGridCreator` | Creates the electrical grid. Load one. |
| Generator | `UnlimitedGenerator` **and** `PowerSocket` | Set `comp set watts 2000` on the generator component. The socket component exposes its sibling generator through powered female connectors. Keep the two compatible default sockets. |
| Plant feeder | `ElectricGridFeeder` | Keep its powered male connector matching the generator socket. This feeds the electrical grid. |
| Office outlet | `ElectricGridOutlet` | Keep a powered female connector matching the appliance plug. |
| Desk lamp | `ElectricLight`, `PowerSupply` and `Connectable` | Use a powered male connector on the Connectable component, matching the office outlet's type name. Set the light's wattage and size the supply for the load. |
| Exchange | `TelecommunicationsGridCreator` | Set `comp set prefix 735` and `comp set digits 4`, using an unused prefix in your game. Load one. |
| Exchange feeder | `TelecommunicationsGridFeeder` | Keep the powered male connector matching a free electrical outlet or generator socket. |
| Two telephone jacks | `TelecommunicationsGridOutlet` | Each jack owns a line. Create a female powered connector matching the handsets. |
| Two handsets | `Telephone` | `comp set watts 5` draws line power. Give each a compatible powered male connector. |

Check connector **type name**, **gender**, **power flag** and available ports. A new telephone normally has a male `TelephoneLine` connector, while a new telecom outlet starts with female `telephone`. On the outlet prototype, `comp set connection add female TelephoneLine true` adds the matching jack. For an appliance plug use `comp set connection add male <actual-socket-type> true` on its Connectable prototype. Read the configured socket type from the generator or outlet component before choosing the plug.

For each item, use `item edit new`, set its noun, short and full descriptions, material, size and weight, then `item set add <current-component-id>` for **each** listed component. Submit the item with `item edit submit`, review it, and load the Current item with `item load <item-prototype-id>`. Put fixtures and detachable items in the intended room. [Item authoring walkthrough](Item_Authoring_Guidelines.md)

## Supply the office from the plant

Load the panel, generator and electrical feeder in the plant. Read the new electrical grid ID from `grid audit` or the panel. As an Admin in the plant, replace the angle-bracket fields with your IDs.

```text
grid status <electric-grid-id>
grid connect feeder <electric-grid-id>
connect feeder generator
switch generator on
grid expand <electric-grid-id> east
east
grid connect outlet <electric-grid-id>
connect lamp outlet
switch lamp on
grid status <electric-grid-id>
```

Load the office outlet and lamp in the office before the two connection commands there. Check the available watts and draw in `grid status`, then look at the lamp. Switch the generator off in the plant and confirm loss of light at the office. Switch it on again and confirm recovery. Inspect the physical plug and the feeder's grid membership separately if the grid reports zero supply. A generator can run on its own item while sending zero watts to the grid.

For a hardwired appliance, put `GridPowerSupply` on the same item as the powered machine, configure its `watts` limit and use `grid connect <appliance> <electric-grid-id>` in the covered room. That arrangement replaces the detachable plug, cord supply and outlet. For a fuel-driven generator, substitute `FuelGenerator` for `UnlimitedGenerator` on the generator item. Configure `liquid`, `capacity`, `consumption` and `watts`, fill it with that fuel, and keep the socket component. Check capacity against all loads that may switch on together.

## Power the exchange and place a call

Return to the plant. Load the exchange, its feeder, a telephone jack and a handset there. Use distinct keywords such as `exchangefeed`, `plantjack` and `plantphone`. For the first test use a second instance of the generator and socket item, with a distinct keyword such as `exchangegen`. This lets you cut telephone power independently. For a shared plant, connect the telecom feeder to an electrical grid outlet in the plant and verify that the electrical grid has spare capacity under load.

```text
grid audit
grid connect exchangefeed <telecom-grid-id>
connect exchangefeed exchangegen
switch exchangegen on
grid expand <telecom-grid-id> east
grid connect plantjack <telecom-grid-id>
connect plantphone plantjack
switch plantphone on
look plantphone
east
grid connect officejack <telecom-grid-id>
connect officephone officejack
switch officephone on
look officephone
grid status <telecom-grid-id>
```

Load `officejack` and `officephone` in the office before connecting them. Read the subscribers' allocated numbers from the live telephone or exchange. With a character at each end and the receiving phone on the hook, run `dial plantphone <office-number>` at the plant, `answer officephone` in the office, then `transmitwith officephone <message>`. Confirm the other end hears it. Finish with `hangup officephone`. A successful ring proves routing to the line. A conversation checks both endpoints.

Use `grid setnumber <jack> <number|auto>` for a deliberate reservation, or use `grid setrings <telecom-grid-id> <count>` to change the timeout. A handset attached to a jack uses the jack's line. A direct-wired handset can instead be assigned with `grid connect <phone> <telecom-grid-id>` in a covered room and own its number directly. When two separately powered exchanges should route calls to each other, use distinct prefixes and `grid link <first-grid-id> <second-grid-id>`. Test a local call on each exchange before dialling across them.

## Choose how a settlement gets its electricity

**Municipal service.** For a city where ordinary play assumes the lights work, install the `UnlimitedGenerator`, socket and feeder from the plant example in a maintained plant room. Set the generator above the total expected draw. A 2,000 W generator serving ten 60 W lamps has 1,400 W left for other concurrent loads. Switch it on, extend coverage down the streets and assign the lamps or outlets. “Unlimited” describes the fuel supply. The wattage ceiling still matters, and disconnecting the plant still cuts service. A city that should never ask players to maintain its power can put the equipment in a staff-controlled room.

**Expedition service.** Give the players an installation project, a socket-equipped `FuelGenerator`, a fuel can and a lamp with a compatible plug. On the generator component set `liquid <fuel>`, `capacity 20 l`, `consumption 2 l` and `watts 1000` using `comp set`. Consumption is specified per hour at peak load. A full tank at that rate budgets about ten hours at peak. Use a liquid your players can acquire, and use the volume unit recognized by your game.

For a player-deployable base, make the packed kit an ordinary item consumed as a material for a **local** installation project. Its finish prog creates the actual panel at the chosen site. Carrying an already commissioned panel from town does not move its grid footprint, and destroying the creator deletes its grid. The following small project hook commissions a panel, a feeder and the first camp outlet. Use flat, ground-level campsites, an approved owner-present installation project and a room with no electrical service for this example. Replace `1001`, `1002` and `1003` with your **Current** panel, feeder and outlet item prototype IDs. Use the generator and lamp assemblies from the two-room example. The three installed fixtures stay in camp.

**`CommissionCampPower(job project) → void`**

```futureprog
var site = @job.location
var installer = @job.owner
if (@installer.location != @site)
  send(@installer, "Return to the campsite before commissioning the panel.")
  return
end if
if (@site.electricgrids.count > 0)
  send(@installer, "This campsite already has an electrical grid.")
  return
end if
var panel = loaditem(1001, @installer, "")
put(@panel, @site)
var ids = @site.electricgrids
if (@ids.count != 1)
  send(@installer, "Inspect the installed panel. Its grid could not be identified.")
  return
end if
var gid = @ids.first(n, true)
var feeder = loaditem(1002, @installer, "")
put(@feeder, @site)
if (connecttogrid(@gid, @feeder) == false)
  send(@installer, "Inspect the feeder. It did not join the camp grid.")
  return
end if
var outlet = loaditem(1003, @installer, "")
put(@outlet, @site)
if (connecttogrid(@gid, @outlet) == false)
  send(@installer, "Inspect the outlet. It did not join the camp grid.")
  return
end if
setregister @site "GridExpansionGrid" @gid
send(@installer, "Connect and fuel the generator, then plug a lamp into the outlet.")
```

Declare `GridExpansionGrid` as shown under **Mark the route** before compiling. The loader argument puts the creator's initial coverage in the installer's room. `put` places the physical fixture. Attach the prog with `project set finish CommissionCampPower`. Check site eligibility at project start as well, and have the owner present for the last shift. A failed finish check reports the problem after the project has spent its materials. For a staffed repair, inspect the existing panel and feeder before replacing either one.

At camp the players open the generator tank, `pour fuelcan into generator`, `connect feeder generator` and `switch generator on`. At the installed outlet, `connect lamp outlet` and `switch lamp on`. The same pattern can commission more player-built fixtures with `connecttogrid`. Check the item's prototype, that it is actually at the site, that the grid covers the site and that the installer controls it before exposing a choice of fixture to a player. The ordinary `connect` command then handles the physical plug. Turn the generator off, verify loss of light, refuel and restart it. To build past the camp, mark a neighbouring exit and use the extension project below.

## Build a water main and a separate sewer

Each `LiquidGridCreator` establishes one liquid grid. Make two headworks items and record their separate IDs as **water** and **sewer**. Room coverage can overlap. A liquid grid pools the contents of its suppliers, including their mixtures. Label both the live items and the IDs before connecting anything. A shared grid for potable water and sewage would mix them.

| Fixture | Components on that item | Purpose |
| --- | --- | --- |
| Municipal reservoir | `WaterSource` + `LiquidGridSupplier` | The reservoir starts full of its default liquid and refills over time. The supplier makes that stored volume available to the water main. |
| Camp cistern | `LiquidContainer` + `LiquidGridSupplier` | Players bring the water. With no refill component the supply runs down as they use it. |
| Drinking tap | `GridLiquidSource` | Withdraws liquid already present on its assigned grid. An empty grid gives an empty tap. |
| Sewage inlet tank | `LiquidContainer` + `LiquidGridSupplier` | Accepts waste poured in locally and makes it available for removal on the sewer grid. |
| Sewer takeoff | `GridLiquidSource` | Makes the sewer's pooled contents available to a treatment pump. |
| Treatment pump and disposal tank | One `LiquidPump` item and a **separate** `LiquidConsumingProp` item | The powered pump moves liquid to the disposal tank, which destroys the volume it holds over time. |

### Hands-off drinking water

1. In the pump house load a water headworks creator and reservoir. Configure the `WaterSource` component with `comp set defaultliquid <potable-liquid>`, `comp set capacity 200 l` and `comp set refillrate 60 l`. Leave automatic refill enabled. Give its item a single liquid container capability so the supplier draws from the intended reservoir.
2. Use `grid connect reservoir <water-id>` in the pump house, then `grid expand <water-id> east` to cover the street. In the street load the drinking tap and run `grid connect tap <water-id>`.
3. With a partly empty jug, use `fill jug tap 1 l` and examine the jug. Check `grid status <water-id>` just after a draw and again after a refill interval. A sustained 30 L/min draw leaves headroom at 60 L/min. A burst larger than the stored 200 L can still empty the reservoir. Size storage for bursts and the refill rate for sustained demand. This gives the city an ongoing water supply without water deliveries.

### Run sewage to a disposal tank

1. Load a second creator at the treatment works. Extend its **sewer** grid to the street and use `grid connect inlet <sewer-id>` on the open sewage inlet tank there. In the works assign `grid connect takeoff <sewer-id>`. The takeoff is for withdrawal. The inlet's tank is where a character deposits waste with `pour bucket into inlet`.
2. In the works, configure `comp set rate 1 l` on a `LiquidPump` and `comp set capacity 20 l` and `comp set consume 2 l` on a `LiquidConsumingProp`. The first rate transfers one litre per second and the second destroys up to two litres per second in the disposal tank. Leave this **terminal tank off the sewer grid** so the discharged contents cannot recirculate.
3. With the standard `LiquidLine` connectors, use `connect pump takeoff` (pump's female inlet to the takeoff's male port), then `connect pump disposal` (pump's male outlet to the tank's female port). Finally `connect pump outlet` to a powered electrical outlet sized for the pump's configured `watts`. Connect the liquid lines first so the pump selects the liquid destination on its male line. The electrical connection supplies the pump.
4. Before connecting the pump takeoff, pour a measured bucketful into the street inlet and record the sewer volume. Complete the three connections in the order above and watch the sewer volume fall. The disposal tank receives and consumes the liquid, and the water grid's mixture stays clean. With several supply tanks the grid draws from them proportionally. If the works cannot keep up, sewage accumulates in the inlets until their tanks fill.

For an unattended **local drain** without a plant, put `LiquidConsumingProp` and `LiquidGridSupplier` on each sewer inlet instead. Waste deposited into one of these drains temporarily counts toward the sewer's volume and disappears as that inlet consumes it. Every inlet can accept more waste as space frees. This arrangement consumes only what is in each drain's own tank. Use the treatment pump when plain storage tanks elsewhere on the grid also need emptying. Both patterns can run indefinitely. Their intake volume and disposal rate are finite.

### Water on an expedition

Install a fresh `LiquidGridCreator` at the campsite, then assign an open `LiquidContainer` + `LiquidGridSupplier` cistern and a `GridLiquidSource` standpipe to that grid. For a player installation project, adapt `CommissionCampPower` to load the **liquid** creator and cistern. Record `site.grids` before commissioning and select the single **new** ID afterwards whose `grid(id).gridtype` is `"Liquid"`. This matters at camps that already have a sewer. Save the potable and waste grid IDs separately wherever players select their service.

With a 40 L cistern, have the crew bring two 10 L cans. Use `pour can into cistern` for each delivery, then `fill bottle standpipe 1 l`. Expect about 19 L to remain after that draw. An empty cistern stops service until the next delivery. Give an established camp a spring by replacing the cistern with the replenishing reservoir design. Add a separate waste grid and a local drain for a low-maintenance camp, or add a pump when the camp grows to need central treatment. To isolate the works, disconnect the liquid intake. The pump checks for a connected power producer, and switching off an upstream generator alone can leave an already running pump transferring liquid.

## Extend a utility through a player-built project

Suppose the plant has power, a water main and a sewer. A crew should be able to lay one of those lines through an approved exit. The plant may send its power east and its sewer south. A local project gives the crew a fixed worksite, materials to supply, and an identifiable job that owns the reservation until completion or cancellation.

### Mark the route

Declare these registers once as an administrator. `Location` is FutureProg's room type. `Grid` is a real Prog type, but persistent dictionaries have **text keys**, and `Exit` is a runtime reference that the register cannot save. Store the grid ID as text in each dictionary and the exit's `keyword` as its value. Cardinal exits use short keywords such as `e`. A non-cardinal exit uses its primary keyword.

```text
register location GridExpansionDirections text dictionary
register location GridExpansionGrid number
register location GridConstructionOwners number dictionary
register project GridWorkGrid number
register project GridWorkTarget location
register project GridWorkDirection text
```

Stand in the plant. In this illustrative world, grid 42 is electrical and grid 87 is the sewer. Substitute IDs from `grid audit`. Enter both pairs in a single setting because the command replaces the dictionary. `GridExpansionGrid` selects which marked route the next crew will build. A staff-controlled room setting works for a city. An expedition can expose the same selection through a surveying interaction restricted to its own grids and exits.

```text
setregister location here GridExpansionDirections (42 e 87 s)
setregister location here GridExpansionGrid 42
```

`GridConstructionOwners` starts empty. Its value is an active project ID on **both** rooms. This also stops a second crew approaching the same destination from another source. Different grids can be under construction in the same room. The project saves its own grid ID, direction and destination so selecting water for the next crew cannot redirect a power line already under construction.

### Write the permission check and its refusal message

Create each named prog with the parameters in the order shown. For example, use `prog edit new GridLineOwner`, `prog set return number`, `prog set parameter add site location`, `prog set parameter add key text` and `prog set text`. Paste only the fenced body and submit the editor. Keep all these progs dynamic so every call sees current room state. [FutureProg authoring walkthrough](FutureProg_Builder_Guide.md)

**`GridLineOwner(site location, key text) → number`**

```futureprog
var owners = getregister(@site, "GridConstructionOwners")
return dictionarygetdefault(@owners, @key, 0)
```

**`GridLineDirection(site location, key text) → text`**

```futureprog
var routes = getregister(@site, "GridExpansionDirections")
return dictionarygetdefault(@routes, @key, "")
```

**`GridLineProblem(site location, gid number, expected number) → text`**

```futureprog
var utility = grid(@gid)
if (isnull(@utility))
  return "Select an existing utility grid for this worksite."
end if
if (@site.grids.any(n, @n == @gid) == false)
  return "The selected grid does not reach this worksite."
end if
var key = totext(@gid)
var direction = @GridLineDirection(@site, @key)
if (@direction == "")
  return "Survey an exit for this grid before starting work."
end if
var routes = @site.exits.where(x, @x.keyword == @direction)
if (@routes.count != 1)
  return "The surveyed exit is missing or ambiguous. Survey the route again."
end if
var passage = @routes.first(x, true)
var target = @passage.destination
if (@target.grids.any(n, @n == @gid))
  return "The selected grid already reaches the far room."
end if
if (@GridLineOwner(@site, @key) != @expected)
  return "The worksite reservation does not match. Check for another crew."
end if
if (@GridLineOwner(@target, @key) != @expected)
  return "The far room reservation does not match. Check for another crew."
end if
return ""
```

**`WhyGridLine(actor character) → text`**

```futureprog
var site = @actor.location
return @GridLineProblem(@site, getregister(@site, "GridExpansionGrid"), 0)
```

**`CanGridLine(actor character) → boolean`**

```futureprog
return @WhyGridLine(@actor) == ""
```

An empty message means the route is available. Previewing a project or asking why it is unavailable never claims it. Add any local franchise or surveying permission to `WhyGridLine` so the Boolean and the message stay in step.

### Reserve the rooms, lay the line and release the reservation

**`SetGridLineOwner(site location, key text, owner number) → void`**

```futureprog
var owners = getregister(@site, "GridConstructionOwners")
if (@owner == 0)
  setregister @site "GridConstructionOwners" dictionarywithoutkey(@owners, @key)
else
  setregister @site "GridConstructionOwners" dictionaryset(@owners, @key, @owner)
end if
```

**`StartGridLine(job project) → void`**

```futureprog
var site = @job.location
var gid = getregister(@site, "GridExpansionGrid")
var problem = @GridLineProblem(@site, @gid, 0)
if (@problem != "")
  send(@job.owner, @problem)
  return
end if
var key = totext(@gid)
var direction = @GridLineDirection(@site, @key)
var passage = @site.exits.first(x, @x.keyword == @direction)
var target = @passage.destination
setregister @job "GridWorkGrid" @gid
setregister @job "GridWorkDirection" @direction
setregister @job "GridWorkTarget" @target
@SetGridLineOwner(@site, @key, @job.id)
@SetGridLineOwner(@target, @key, @job.id)
```

**`ReleaseGridLine(job project) → void`**

```futureprog
var key = totext(getregister(@job, "GridWorkGrid"))
var site = @job.location
var target = getregister(@job, "GridWorkTarget")
if (@GridLineOwner(@site, @key) == @job.id)
  @SetGridLineOwner(@site, @key, 0)
end if
if (isnull(@target))
  return
end if
if (@GridLineOwner(@target, @key) == @job.id)
  @SetGridLineOwner(@target, @key, 0)
end if
```

**`InstallGridLine(job project) → boolean`**

```futureprog
var site = @job.location
var gid = getregister(@job, "GridWorkGrid")
var key = totext(@gid)
var problem = @GridLineProblem(@site, @gid, @job.id)
if (@problem == "")
  var direction = @GridLineDirection(@site, @key)
  var passage = @site.exits.first(x, @x.keyword == @direction)
  var target = getregister(@job, "GridWorkTarget")
  if (@direction != getregister(@job, "GridWorkDirection") or @passage.destination != @target)
    problem = "The surveyed route changed during construction."
  else
    if (extendgrid(@gid, @target))
      send(@job.owner, "The utility line now reaches the far room.")
    else
      problem = "The grid could not be extended. Inspect both rooms."
    end if
  end if
end if
@ReleaseGridLine(@job)
if (@problem != "")
  send(@job.owner, @problem)
  return false
end if
return true
```

**`FinishGridLine(job project) → void`**

```futureprog
@InstallGridLine(@job)
```

`extendgrid` adds room coverage. It does not choose an exit, enforce adjacency or install equipment. Those are separate decisions. Keeping the destination on the job also lets cancellation release the original far room after someone edits the map. A failed completion check leaves the grid untouched and tells the owner why. Project phase actions do not veto completion or reimburse materials, so mark out a new route before commissioning a replacement job.

### Attach the progs and commission the first extension

Create a **local** project. Set a name, tagline, a Boolean appearance prog taking one character and at least one meaningful labour or material requirement. For example, require a roll of cable and a shift of electrical work for the power line. Use pipe and plumbing labour for a water or sewer job. If each template buys different materials, restrict its permission prog to the corresponding grid or utility service. Two liquid grids can serve different purposes.

```text
project set can CanGridLine
project set why WhyGridLine
project set start StartGridLine
project set cancel ReleaseGridLine
project set phase 1 action add prog
project set phase 1 action <action-name> prog FinishGridLine
```

Use the action name shown for the new phase action. Attach it to the phase that physically lays the line. For a project whose line goes live only after the whole job, use `project set finish FinishGridLine` **instead** of the phase action. Submit and approve the revision. See the [project guide](Projects_System_Builder_Workflows.md) for labour, materials, approval, supplying and joining a shift.

1. Before setting a direction, run `prog execute WhyGridLine self` in a covered room. Expect the surveying message. Set the route and run `prog execute CanGridLine self`. Expect `true`.
2. Start the approved project with `project start <name>`. A second character trying the same route should see the reservation message. Cancel the first project and confirm the second character can now start it.
3. Supply and finish the restarted project. Verify that `grid status <id>` gained one location and that another grid in the source room kept its old count. Retrying should now report that the grid already reaches it. Install an outlet or tap there to test service under load.
4. On another unbuilt route, change the surveyed exit during construction, then complete the job. Neither the original nor the replacement room should gain coverage. Restore the survey and confirm a new crew can start.

## Use a successful craft phase to fit the final joint

A city can commission the pipe as a project but require a plumber to perform the final joint as a craft. In this variant the **project owns the surveyed route**, and the **craft's normal product** makes the connection. An unsuccessful skill roll cannot lay the pipe. The craft-wide `onfinish` hook receives only the character and also fires when a failed craft reaches its end. A normal `prog` product can run on the successful phase and receives the items actually consumed.

Try this first at one pump house with a fixed pipe jig. Make three Current, non-stackable item prototypes for a numbered work ticket, an accepted installation slip, and a rejected slip. Substitute their prototype IDs for `1010`, `1011` and `1012` below. Give only the accepted slip the tag required for sign-off. Declare the link between the ticket and its job.

```text
register item GridWorkOrder project
```

**`IssueGridTicket(job project) → void`**

```futureprog
@StartGridLine(@job)
var gid = getregister(@job, "GridWorkGrid")
if (@gid > 0)
  var ticket = loaditem(1010)
  setregister @ticket "GridWorkOrder" @job
  put(@ticket, @job.location)
  send(@job.owner, "Pick up the work ticket at the pipe jig.")
end if
```

On this project use `project set start IssueGridTicket`, use `project set cancel ReleaseGridLine` and `project set finish ReleaseGridLine` for cleanup, and require one accepted slip as the final project's material. Leave the line-installing phase action off this variant. The player gains coverage at the craft phase and then supplies the slip to close the job. Run one pilot job at this jig at a time, and remove obsolete tickets and slips before issuing another. The material requirement checks the slip’s tag, so use the project-action version for concurrent contracts that need independent proof of delivery.

**`WhyFitGridJoint(actor character) → text`**

```futureprog
var tickets = @actor.helditems.where(i, isnull(getregister(@i, "GridWorkOrder")) == false)
if (@tickets.count != 1)
  return "Hold the single work ticket for this joint."
end if
var ticket = @tickets.first(i, true)
var job = getregister(@ticket, "GridWorkOrder")
if (@job.location != @actor.location)
  return "Take the ticket to its surveyed worksite."
end if
var gid = getregister(@job, "GridWorkGrid")
return @GridLineProblem(@job.location, @gid, @job.id)
```

**`CanFitGridJoint(actor character) → boolean`**

```futureprog
return @WhyFitGridJoint(@actor) == ""
```

**`FitGridJoint(inputs item collection dictionary) → item`**

```futureprog
var ticket = collectiondictionarygetfirst(@inputs, "1")
if (isnull(@ticket))
  return loaditem(1012)
end if
var job = getregister(@ticket, "GridWorkOrder")
if (isnull(@job))
  return loaditem(1012)
end if
if (@InstallGridLine(@job))
  return loaditem(1011)
end if
return loaditem(1012)
```

On an otherwise valid craft, set up the first input and first normal product below. The `"1"` passed to `collectiondictionarygetfirst` is the input's **one-based slot number**, not the item's ID. Put the jig in the craft's tool requirements. Keep the ticket for this job as the only eligible ticket within reach.

```text
craft set canuse CanFitGridJoint
craft set whycannotuse WhyFitGridJoint
craft set input add simple
craft set input 1 item 1010
craft set input 1 quantity 1
craft set product add prog
craft set product 1 prog FitGridJoint
craft set failphase 2
craft set phase edit 2 15 "$0 checks $i1 at $t1 and completes $p1." "$0 fails to seal the joint at $t1."
```

Create phase 2 and tool 1 before editing the phase. Make phase 2 the final phase in this example. The first mention of `$i1` in the normal echo reserves the ticket as an input. The first `$p1` schedules the product. The failure echo contains no `$p1`. The product gets input items (and optionally liquid inputs), without an implicit character or room. The ticket lets it find the same saved job and recheck both reservations and the mapped exit.

Run a success, a failed roll, and a cancellation. Success should yield an accepted slip and new coverage once. A failed roll should leave coverage unchanged. A changed route on the success branch should yield a rejected slip. The **project** holds the reservation across a craft interruption. Cancel the project to release it and start a freshly surveyed order when abandoning the work. A joint already fitted stays fitted when a later sign-off is cancelled. [Craft phase and product authoring](Crafting_System_Builder_Workflows.md)

## Trace a failed connection

| Observation | What to check |
| --- | --- |
| Grid assignment reports the wrong type or an uncovered room | Check the ID with `grid audit`. Stand inside existing coverage to expand through each exit in turn. Assign the correct live fixture from its room. |
| The generator has output, but grid supply reads zero | Check the generator's own PowerSocket, a powered matching feeder connection, and the feeder's assigned grid. The panel alone supplies no watts. |
| An office fixture stays off | Check spare grid capacity, its switch, outlet assignment, plug match and sibling PowerSupply, or its hardwired GridPowerSupply. |
| The telephone has no number or never rings | Check exchange coverage, the telecom feeder's upstream power, and the jack's assigned number. Confirm the handset has line power, is on, and is on the hook. |
| The water main reads empty | Inspect the supplier’s actual tank and liquid, its grid assignment and refill settings. Putting a tap in the street does not add supply. |
| Waste remains in an inlet | Verify the inlet and takeoff share the sewer ID, check the pump’s two liquid ports, then inspect the disposal tank’s free capacity and consumption rate. |
| Calls work locally but fail across exchanges | Check both exchange prefixes and their link with `grid status`. The link supplies routing and each exchange needs its own power. |

Use the powered rooms to build the sensor, camera and lighting examples in [Automation systems](Automation_Systems_Builder_Guide.md).
