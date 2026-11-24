---
slug: primary-production-and-agriculture
category: Agriculture
summary: Build forage grounds, hunting and butchery, tenant farms, working mines and one-time scavenging. Connect producers to crafts and projects with item counts, commodity mass and tags.
---

# Primary Production and Agriculture

When the next player arrives, what should they find? Sorrel grows back along a riverbank. A second hunter can dress the boar the first hunter skinned. A tenant inherits an established orchard. The last cart of ore can exhaust a mine, and a second search of a plundered strongbox should turn up nothing. Choose the system that can remember the part that matters to your players.

| What the player does | Where the game remembers it | Where the output goes next |
| --- | --- | --- |
| Gathers a handful of greens, sticks or stones | Each room keeps a balance for each yield on its effective foragable profile. | A foragable produces an item or commodity. Grazing animals can also draw directly from the pool. |
| Takes a hide, trophy or primal | The corpse or severed part records what has been taken. A race butchery profile defines eligible products. | The tanner, carver or cook takes over with ordinary crafts. |
| Cultivates land across seasons | A field holds crop, orchard, pasture or woodland state. An apiary can share it. | Field operations use local projects for labour and supplies. Harvested goods can feed crafts and shops. |
| Works a quarry or extends a mine | A local project holds the work. A room or marker can hold an authored ore reserve. | Completion actions reveal deposits or release commodities. A craft can process each batch. |
| Searches a ruined building | A loot table rolls the contents. The room, chest or instance can remember the search. | Items and commodities appear where the caller places the package. |

Use the [craft guide](Crafting_System_Builder_Workflows.md) and [project guide](Projects_System_Builder_Workflows.md) for creating, phasing and approving definitions. Commands below assume the relevant definition is open for editing. Replace angle-bracketed values with IDs, names or revisions from your game. Names such as **Wayfarer Boar Hide** are proposed content. Stock farming definitions are identified where used.

Find the worked examples for [the hunting wood](#the-wayfarer-inns-hunting-wood), [the tenant grange](#the-leased-grange-from-sowing-to-the-second-season), [the north drift](#the-iron-mine-beyond-the-north-drift) and [the customs house](#the-ruined-customs-house).

## Choose the unit the player cares about

Suppose the Wayfarer carpenter needs **100 iron nails** to repair the stable doors. Your smith reckons those nails weigh **2 kg**. Both figures describe this particular batch. Which one should the carpenter have to supply?

| Decision | 2 kg of iron commodity with the Nails pile tag | 100 units of an Iron Nail item prototype |
| --- | --- | --- |
| Buying and issuing stock | Weigh out part of a larger pile. A smith can supply irregular batches and one recipe can ask for 400 g while another asks for 2 kg. | Count out an exact number. A customer can ask for six nails, and a recipe can use exactly six regardless of their configured unit weight. |
| What the engine knows | Solid material, mass and secondary pile tag describe the lot. Quality and configured commodity characteristics can distinguish grades. Individual nail count and identity would be assumptions. | The prototype can supply its own noun, descriptions, components and characteristics. Use a stackable prototype for interchangeable units. Separate items when an individual nail must keep its own condition or other state. |
| Cost of the choice | Fewer prototypes for a family of bulk goods. The lot can divide at masses that make little sense for whole nails, and a player cannot target the “seventeenth nail” in it. | One prototype can make all 100 units. A stack can keep inventories manageable. Many loose objects add inventory clutter, and distinct designs or behaviours take authoring work. |
| Best fit here | Stock sold by weight, bulk construction, or a forge taking an iron lot back for melting. | A joiner counting fasteners, a player carrying three spare nails, or a particular engraved nail that matters in play. |

**Wire the choice into the consumer.** These are alternative first inputs on the open repair craft. Choose one. Adding both makes both supplies necessary.

```text
craft set input add commodity
craft set input 1 material iron
craft set input 1 piletag Nails
craft set input 1 weight 2kg
```

```text
craft set input add simple
craft set input 1 item <iron-nail-prototype>
craft set input 1 quantity 100
```

For the bulk version, a qualifying 2.5 kg pile can supply 2 kg and leave 500 g. A commodity craft input looks for one pile large enough. For the counted version, a `tag` input with `tag Nails` and `quantity 100` can admit several nail designs when substitution makes sense. Use discrete nail prototypes with that classification and keep mass-based supplies out of the counted recipe. A shared word in two descriptions does not convert kilograms into units. If the town sells by weight and the joiner works by count, give the smith a **sort and bundle nails** craft that consumes 2 kg and makes 100 stackable 20 g nails. If the world also permits the reverse conversion, balance its mass and any wastage in both directions. [Item prototypes and components](Item_Authoring_Guidelines.md)

## Give tags a job at each end of the chain

A tag pays for itself the next time you add content. If the butcher and three crafts require a **Skinning Tool**, a new bone-handled knife can work in all four places as soon as its prototype gets the right tag. The builders of those four activities need no edits. Likewise, a new tagged prospecting site can use the town's existing survey project, and a new cereal can qualify for a mill that already accepts grain. Use the most specific appropriate tag at the source and the narrowest common parent the consumer should accept.

| Put the tag here | Example | Who reads it |
| --- | --- | --- |
| Item prototype | A bone-handled knife under **Skinning Tool**, a boar shoulder under **Pork Primal** | A butchery tool requirement or a craft `tool` checks the knife. A craft `input` checks the shoulder. A reusable tool belongs in a tool slot. |
| Solid material | **Millable Grain** on rye and barley | A `commoditytag` requirement accepts either material. A recipe that must produce rye flour can ask for exact rye instead. |
| Commodity output | **Seeds** on planting stock, **Nails** on iron fasteners, **Quarried Block** on limestone | A pile tag describes the form or intended use of this lot. A stonemason's craft can distinguish dressed blocks from loose chips of the same limestone. Set the required pile tag on the consuming input as well. |
| Room | **Hematite Ground** on selected workings | A discovery action reads `locationtag`. Another crew can reuse the same project in every qualifying room. A tag on a pick or deposit prop cannot satisfy that site check. |
| Deposit marker prototype | **Hematite Exposure** on the visible vein | Discovery reads `duplicate` to decide whether the site already has a marker. The tag can cover several visually different marker prototypes. |
| Recipe inputs, tools and products | A mill takes a family of grain materials, uses a tagged millstone and produces a flour lot with a selected tag | The recipe expresses which substitutions are legitimate and passes a recognisable product to the next recipe. Use the craft's **category** to organise the recipe list. |

Use `tag list` to look for an existing branch first. To create a new family use `tag new Fasteners`, then `tag new Nails Fasteners` if those names are available. On an open nail prototype use `item set tag <nails-tag-id>`. A requirement for Fasteners also admits its descendants. If that door really needs nails, accepting Fasteners would also admit a sibling such as Rivets. Use the tag ID when two branches have the same leaf name. A material's tag and a commodity's secondary pile tag are two separate selections. For stock sowing, for example, the solid carries **Agriculture Seedable** and the particular pile carries **Seeds**.

For example, mark the rye solid with `material edit rye` and `material set tag <millable-grain-tag-id>`. The latter toggles the tag, so inspect the material before repeating it. Give the harvested grain lot a **Milling Stock** pile tag in its crop output. On the miller's open craft these settings accept 5 kg of any suitable grain delivered for milling.

```text
craft set input add commoditytag
craft set input 1 material <millable-grain-tag-id>
craft set input 1 piletag <milling-stock-tag-id>
craft set input 1 weight 5kg
```

With a `commoditytag` craft input, `material` selects a tag on the solid and `piletag` selects the tag on the lot. Prefer those explicit fields. The short `tag` command on this input selects the material tag, while `tag` on an exact-material `commodity` input selects the pile tag. On an open smithing craft, the other end of the nails example is `craft set product add commodity`, `craft set product 1 commodity iron`, `craft set product 1 weight 2kg` and `craft set product 1 tag Nails`. That lot can go straight into the carpenter's recipe.

For a generated district, give the mine-bearing rooms real framework tags through the autobuilder feature mask. In `room new "Feature Rectangle"` use the ore-ground tag ID in the mask entries for those rooms and a room template that applies framework tags. A FutureProg can check the result with `istagged(@site, <tag-id>)`. Describing reddish seams in prose gives a prospector a clue. The framework tag makes the site eligible. Store quantities, ownership and depletion separately, since a tag alone cannot tell you how much remains. [Room templates and feature masks](Room_Building_Builder_Guide.md)

## 1. Foraging and wild resources

A **foragable** defines a possible find. A **foragable profile** groups finds and sets a capacity and hourly recovery for each named yield. The yield name is the pool the player searches. Several different edible finds can draw on `greens`. A pile tag such as **Herbal Ingredient** classifies the goods after they leave the bank.

### A riverbank sprig of sorrel

Make an approved bunch-of-sorrel prototype. Give it the ingredient tag the kitchen uses and an edible component if players can eat it directly. Create the result, then approve it through the normal review workflow before adding it to the profile.

```text
foragable edit new
foragable set name Sorrel
foragable set types greens
foragable set proto <sorrel-prototype-id>
foragable set quantity 1
foragable set chance 10
foragable set consumption 25%
foragable edit submit
```

```text
fp edit new
fp set name Riverbank Greens
fp set yield greens 4 1
fp set foragable Sorrel
fp edit submit
```

Once the profile is Current, stand in each bank room and run `room set forage <profile-id>`. Players can `forage greens`. A room first uses its own profile, then its zone's, then its terrain's. A shared profile gives the same rules to both banks. Each bank keeps its own stock.

A successful bunch costs a quarter of **one point**. The first bank falls from 4 to 3.75 and the untouched bank stays at 4. An hourly recovery of 1 can bring the first bank back to 4 at the next recovery tick. Downtime does not grant missed ticks. `chance 10` is a relative selection weight among eligible results. With a second equally eligible find of weight 30, sorrel has one share in four on an untargeted choice between those two. Difficulty and permitted check outcomes determine eligibility first. Raising the bunch size to six increases output sixfold while the successful search still costs 0.25. Set both figures deliberately.

A single flint or a walking stick makes sense as an item. For leaves traded by weight, switch a result to `foragable set commodity material <leaf-solid> tag <herb-tag> weight 50g`. The inn can now buy bulk leaf and its cook can reserve a measured portion.

## 2. Hunting, skinning and butchery

### The Wayfarer Inn's hunting wood

**Give the herd a reason to move.** Put the same vegetation pool in two reachable clearings in terrain the wildlife group will enter. For this wood use `greens` so villagers and animals can deplete the same food supply. On the boar race, inspect its existing diet, including inherited entries. If it lacks greens, an example addition is `race set yield add greens 2 0 $0 noses through the low growth.` The numbers are hours of hunger and thirst satisfied per yield point. For a smaller bite use `race set yield greens bite 0.25`. Give the boar template the resulting race and choose its spawner's monitored zone and population limits. Use `gait list` to find a Wildlife template whose preferred habitat includes these clearings. Create the live group with `group new <template> "Wayfarer Sounder"` and add each live boar with `group addmember "Wayfarer Sounder" <boar>`. A compatible Animal AI and Wildlife group policy let hungry members move between feeding sites. A hunter who leaves the back clearing untouched preserves a feeding opportunity there. A thirsty group, a satiated boar, an impassable route or another food source can change where the animals go. Cutting a managed woodland field draws on a different balance. [NPC templates and spawners](NPC_Builder_Guide.md)

**Build the carcass from the inn's order backwards.** The tanner wants a hide, the carver wants tusks and the kitchen wants a shoulder. Author those as explicit item prototypes. Tag the shoulder **Pork Primal** so a later cooking craft can accept suitable shoulders from several boar races. Tag each eligible knife **Skinning Tool**. On the boar's actual body plan, identify the surviving parts each output needs.

```text
butcheryproduct edit new "Wayfarer Boar Hide" <body> <hide-bearing-part> <hide-proto>
butcheryproduct set pelt
butcheryproduct edit new "Wayfarer Boar Tusk" <body> <tusk-part> <tusk-proto>
butcheryproduct set category trophy
butcheryproduct edit new "Wayfarer Boar Shoulder" <body> <shoulder-part> <shoulder-proto>
```

For a pair of independently recoverable tusks, give each side its own product and required part. A single product requiring both parts makes the pair depend on both surviving. Each product can specify a damaged output and threshold. A torn hide that a tanner can still cut into strips gives a wounded animal a useful, lower-value result.

Create `butchery edit new "Wayfarer Boar"` or clone a suitable profile. Set `butchery set tool <skinning-tool-tag>`, choose `check main <trait> <difficulty>` and `check trophy <trait> <difficulty>`, and attach the three products using `butchery set product <product>` once each. Give the main, trophy and skinning paths their own phase emotes using `butchery set emote add`, `butchery set subemote trophy add` and `butchery set skinemote add`. Each takes seconds followed by the emote. Set `butchery set skindiff <difficulty>`. For example, the skinning emote can describe the hunter drawing the hide free, while the trophy emote describes working a tusk loose. Set `race set butcher <profile>` on the boar race once those paths are complete. The same system supports `salvage` as the profile verb on an appropriate body. [Product, check and phase commands](../Crafting/Butchering_System.md)

**What the players can do.** At the carcass a hunter can `skin boar`, `butcher boar trophy`, then `butcher boar`. A companion can take over after the hide is removed. The remains remember completed categories. Final breakdown gives the remaining eligible non-pelt products and consumes the carcass, so a hunter who wants the hide must take it first. To make a buyer, create a shoulder merchandise profile on the inn shop and use `shop merch set willbuy` if buying is disabled. Fund the shop and set its buying price. A hunter can sell a whole primal there. At the inn a separate portioning craft consumes the shoulder and produces cuts. Keeping those steps separate lets the hunter sell an intact primal and the cook decide how to use it. [The Wayfarer Inn](Hospitality_Builder_Guide.md)

## 3. Fields and agriculture

### The leased grange, from sowing to the second season

The inn offers to buy rye, apples and honey. Give the tenant a bread field and an orchard on adjoining rooms of the same property. Those rooms need different profiles because one must support annual crops and the other must support orchard use.

1. **Lay out the holding.** Open the property with `property edit <grange>`. Stand in each missing plot and use `property set room` once. It toggles membership. Have the owner advertise the lease and the tenant take it through the conveyancing office. A field on owned land checks that the initiator is an authorised owner or leaseholder (or an administrator). Other workers can contribute labour to the active local project. A game with common land can leave its fields outside property ownership. [Property and leasing commands](Clan_Builder_Guide.md)
2. **Create the fields.** As a builder stand in the bread field and use `field create Arable Field`, then in the orchard use `field create Orchard Grove`. These are stock profiles. Use `field profile list` to find equivalents in your world. `terrain set agriculture <profile>` supplies a default for future `field create` calls. Each room still needs a field created in it.
3. **Decide what the mill and orchard buy.** Inspect `field crop show Rye` and `field crop show Apples`. Rye can yield both grain and straw. Give each output the solid and pile tag its buyer expects. Use `field crop set <crop> output list` before editing a numbered output. Crop outputs use grams. Set a local planting window on your crop if appropriate, e.g. `field crop set Rye planting group Autumn Spring`. A long sowing project must still be eligible when it finishes.
4. **Start the tenant's work in the right room.** In the bread field the tenant uses `field start "Sow Crop" Rye`. In the orchard they use `field start "Plant Orchard" Apples`. These commands capture the intended field, operation and target. A scripted steward uses `StartFieldProject` with the initiator onsite. Use `project preview <project>` to see which supply the project will take, `project supply <project>` to contribute it and `project join <project> <labour>` to join a role. Starting a project does not put the tenant to work automatically.
5. **Make seed policy match the story.** Stock sowing accepts a mass of an eligible seedable material carrying the **Seeds** pile tag. Rye listed on a crop's descriptive seed requirement does not enforce a rye-only payment. If the grange contract requires rye seed, make a dedicated sowing operation and project with an exact rye `commodity` material requirement, a Seeds pile tag and the desired `amount`. That fixes the required delivery for this project. The generic sowing template does not bind the chosen crop to the supplied solid, so a mixed-crop farm needs a separate authoring decision if it promises that guarantee. For a village that exchanges seed freely, the shared requirement is simpler.
6. **Bring in the harvest.** `field look` shows the plot's use and condition. Use `field harvest` when the crop is ready, then supply the harvest labour. Weather, field health, work outcome and configured base output affect the pile the team brings home. A skilled supervisor can improve the work of several labourers. To hire NPCs for a paid job, fund the project's reserve and set the role's hourly rate. [Project labour and pay](Projects_System_Builder_Workflows.md)
7. **Keep bees alongside the apples.** Once the orchard is established, `field start "Install Apiary"` uses the same land. The stock project needs two hive items and a stand. Set the crop's pollination response, for example `field crop set Apples pollination Strong 1 2`. Healthy hives within range support the growing and setting fruit. `field start "Tend Apiary"` maintains the hives and `field start "Harvest Apiary"` draws on stores. For a low apple yield, check the crop's stage and whether the hives are healthy and in range before increasing the crop's base mass.

**What players can do next season.** The tenant must sow the harvested rye plot again. The apple trees remain planted and can enter another fruiting cycle. The beekeeper can tend the hives between fruit harvests. The inn can order each product separately, and a new tenant can take over the established orchard.

**Pasture and woodland.** An adjoining pasture can hold a field herd measured in headcount, condition and secondary yield. `field start "Collect Herd Products" <herd>` can supply wool or milk from configured animals. If a particular animal must be led to market, `field herd draw <herd> 1` can produce a live NPC when the herd has a matching NPC template. That animal now has a body and can enter the butchery chain. To supply the basket maker, use a separate `Managed Woodland` field and `field start "Plant Woodland" "Willow Coppice"`. Once mature, a crew can `field start "Coppice Woodland"` to cut a share of the yield and keep the stand. [Field profiles, operations, pasture and pollination](Agriculture_Builder_Workflows.md)

## 4. Production through crafts, projects and progs

**One basket maker cutting rods.** On a craft to gather weaving rods, add a `field` input and a tool tagged **Coppicing Hook**. Set the field input as follows. The numbers are an example on the field's 0–100 scale.

```text
craft set input add field
craft set input 1 woodland Willow Coppice
craft set input 1 health 40
craft set input 1 yield 15
craft set input 1 consume 10
```

Give the craft a product of willow commodity with a **Basketry Rods** pile tag. A basket-weaving craft can use those rods back at the workshop. The gathering craft checks land access and takes its yield when it reserves the input. Leave enough yield in the stand for regrowth and price any failed batch with that debit in mind.

**A crew quarrying limestone.** Give a local project a labour requirement for the quarry crew and a `commodityoutput` phase action set to limestone, `weight 60kg` and the **Quarried Block** tag. One completion delivers a lot a hauler can split. A mason's dressing craft takes 20 kg of limestone with that pile tag and can output one dressed block and chips. Three batches can use the original lot. A different road-metal recipe can use the chips. This simple quarry suits a town with a dependable stone supply. To sell the last available cartload, give the site a reserve as in the mine below.

### The iron mine beyond the north drift

**Stage 1. Prospect a site.** Two neighbouring rock rooms look promising, but only the east bank bears **Hematite Ground**. Apply the framework tag when laying out that room. Create an approved, stationary vein marker with the **Hematite Exposure** item tag. Give the locals a survey project with qualified labour. With that project's first phase open for authoring, add the two actions below. Replace `<reveal>` and `<sample>` with the action names shown by the editor after each addition. Quote a multi-word name.

```text
project set phase 1 action add resourcediscovery
project set phase 1 action <reveal> sort 10
project set phase 1 action <reveal> locationtag <hematite-ground-tag>
project set phase 1 action <reveal> item <vein-marker-prototype>
project set phase 1 action <reveal> duplicate <hematite-exposure-tag>
project set phase 1 action <reveal> failureecho The trench bottoms out in grey shale.
project set phase 1 action <reveal> alreadyecho The survey stakes still mark the exposed seam.
project set phase 1 action add commodityoutput
project set phase 1 action <sample> sort 20
project set phase 1 action <sample> material hematite
project set phase 1 action <sample> weight 1.5kg
project set phase 1 action <sample> tag <sample-ore-tag>
project set phase 1 action <sample> discovery <vein-marker-prototype>
```

The east-bank crew gets a vein and one sample. Another crew at the same marker finds the stakes. The west-bank crew finds shale. Only fresh discovery produces the sample. Keep **Sample Ore** separate from **Raw Ore** if the assay office buys samples and the smelter takes cartloads. Give the physical marker a suitable in-world lifetime. Removing a marker that supplies the only duplicate check can reopen the discovery opportunity.

**Stage 2. Sell the last cartload once.** For a single seam per room, let the room own the remaining number of 20 kg loads. The marker tells players where they can dig. Give the reserve a persistent number and initialise it once when the site is stocked. Never refill it simply because somebody starts work or surveys again. For example, at the east bank:

```text
register location OreLoadsLeft number
register location OreCrew number
setregister location here OreLoadsLeft 3
setregister location here OreCrew 0
```

Make a local extraction project with one work phase. Put a single fixed 20 kg hematite output tagged **Raw Ore** in a loot table called **Hematite Cartload** (the group and commodity syntax is in chapter 5). This lets a prog request one known package and read the placement result. Use a `prog` completion action for this project. Attaching an additional unconditional `commodityoutput` would issue a second load.

| Attach this prog | What to put in it |
| --- | --- |
| `project set why WhyMineOre`, returning text from one character | Inspect `@actor.location`. Require a visible exposure at the work site, `OreLoadsLeft` above zero and `OreCrew` equal to zero. Return an appropriate message such as “The seam is worked out” or “A crew already has this face”. Return empty text for an available face. |
| `project set can CanMineOre`, returning boolean from one character | `return @WhyMineOre(@actor) == ""`. Both progs only read state, so browsing the catalogue never claims the face. |
| `project set start ClaimOreFace`, taking one project | Recheck the site and store `@job.id` as `OreCrew`. A second crew now waits even if it previewed the project first. |
| Phase action `prog TakeOreLoad`, taking one project | Recheck the marker, remaining load and ownership at `@job.location`. Materialise the cartload and debit only a successful receipt. Use the active project's room even if the foreman has gone home. |
| `project set cancel ReleaseOreFace` and `project set finish ReleaseOreFace`, each taking one project | Clear `OreCrew` only when it still equals this project's ID. Cancellation before delivery leaves the stock available. |

For a normal ground-level room, the body of `TakeOreLoad(job project)` can be written as below. In this example table 42 revision 0 is the approved cartload, and 73 is the marker's tag ID. Substitute the values from your world. Where one room contains several distinct work sites or layers, put the balance on the individual exposure and have the project remember that marker.

```futureprog
var site = @job.location
var loads = getregister(@site, "OreLoadsLeft")
if (getregister(@site, "OreCrew") != @job.id or @loads < 1)
  return
end if
if (@site.items.any(i, istagged(@i, 73)) == false)
  return
end if
var receipt = loadloottable(42, 0, @site, "default")
if (startswith(@receipt, "OK"))
  var remaining = @loads - 1
  setregister @site "OreLoadsLeft" @remaining
else
  send(@job.owner, "The cart could not be filled. The load remains in the seam.")
end if
```

When the third cart leaves, the reserve reads zero. An old marker can remain as an exhausted working. Placement and register persistence have separate save boundaries. The crew check prevents competing jobs from claiming the same last load during ordinary play. A server interruption between package commitment and the register save requires reconciliation.

**Stage 3. Extend the north drift.** Let a surveyor mark a direction before a crew takes the contract. In this design, only one extension can occupy a face at a time. Use short cardinal keywords such as `n` and `e` consistently. Store text for a proposed route because an unopened shaft has no exit.

```text
register location ShaftDirection text
register location ShaftCrew number
register default location ShaftCrew 0
register project CutDirection text
register location ShaftDraftRoom location
setregister location here ShaftDirection n
```

Staff can mark the direction directly. For a player-driven expedition, attach an interaction to a survey stake that accepts a direction from an authorised surveyor at the face. Write the same `ShaftDirection` value and refuse changes while `ShaftCrew` is occupied. The project lifecycle supplies the remaining hooks.

1. **Eligibility.** Write `WhyCutShaft(actor character) → text`. Require a designated mine face, a permitted direction, no current exit in that direction, no reserved crew and no outstanding draft room. Compare the requested keyword against `@actor.location.exits`. Use `CanCutShaft(actor character) → boolean` to return whether the explanation is empty. Attach them with `project set why` and `project set can`.
2. **Reservation.** Attach `StartCutShaft(job project) → void` with `project set start`. Recheck the face, copy the selected direction to `CutDirection` on the project and write the active ID to `ShaftCrew` on the room. Cancellation clears only that crew's reservation. A changed survey or a rival crew cannot redirect an existing contract.
3. **Construction.** On the last phase add `action add prog` and set its `prog` to `CutShaft(job project)`. Recheck the owner ID, the saved direction and the outgoing exits. Resolve an editable package dedicated to this extension, its template and the site's zone. The completion body below opens a dedicated package and resolves a tunnel template by its unique room name. Use separate packages for independent extensions so approval activates only the intended work.
4. **Opening.** Have a builder inspect the resulting draft, approve it and swap it into use. A game that explicitly delegates opening to a trusted prog can use `ApproveOverlay(package, builder, comment)` followed by `SwapOverlay(package)` after successful linking. Once the shaft is open, clear `ShaftDraftRoom` and the matching reservation. A draft left for review must keep its room reference, so a fresh crew cannot create another copy while it waits.

```futureprog
var site = @job.location
var direction = getregister(@job, "CutDirection")
if (getregister(@site, "ShaftCrew") != @job.id or @direction != getregister(@site, "ShaftDirection"))
  return
end if
if (@site.exits.any(x, @x.keyword == @direction) or isnull(getregister(@site, "ShaftDraftRoom")) == false)
  return
end if
var template = locationbyuniquename("north-drift-template")
if (isnull(@template))
  return
end if
var package = CreateOverlay(@job.owner, "North Drift " + totext(@job.id))
if (isnull(@package))
  return
end if
var shaft = CreateRoom(@package, @site.zone, @template)
if (isnull(@shaft))
  return
end if
setregister @site "ShaftDraftRoom" @shaft
NameRoom(@shaft, @package, "Beyond the North Drift")
var way = LinkRooms(@site, @shaft, @package, @direction)
if (isnull(@way))
  send(@job.owner, "The heading needs a surveyor to reconcile its connection.")
  return
end if
```

Give the model tunnel room the unique name `north-drift-template` or substitute your own key. The project ID gives each draft package its own name. If a creation attempt has already left a package under that name, inspect the existing draft before retrying. `LinkRooms` checks both ends and adds the return exit. If linking fails after creation, the recorded draft room lets the builder repair the connection without digging another room. Returning from a phase action does not undo or veto project completion. Use the recorded link and package state when announcing a successful opening.

For a newly opened shaft that intersects ore, place a vein node there and initialise its reserve once as part of opening the working. For example, place the approved exposure item in the new room and seed `OreLoadsLeft` to two for a short seam. Barren shafts get neither. The miners can now extract two cartloads using stage 2. If you want a hidden survey opportunity instead, lay out tagged ground for stage 1 and leave the visible marker for the prospectors. Copying an overlay's description is insufficient to mark the room as ore-bearing. [Project authoring](Projects_System_Builder_Workflows.md) · [FutureProg and persistent registers](FutureProg_Builder_Guide.md) · [Room overlays](Room_Building_Builder_Guide.md)

## 5. Scavenging and loot tables

### The collapsed toll booth

Scavengers might turn up an old copper lot, an identifiable brass tally or an empty drawer. A single group makes one choice. Weights of 3, 1 and 6 give those choices 30%, 10% and 60% respectively. If the drawer should always contain a document as well, put the document in its own group. Adding it as a fourth choice would make it compete with the other three.

```text
loottable new "Old Toll Drawer"
loottable set group add default drawer
loottable set choice add default drawer copper 3 commodity <copper-solid-id> tag <scrap-tag-id> mass 40g 80g
loottable set choice add default drawer tally 1 item <tally-proto-id> revision <revision> quantity 1 1
loottable set choice add default drawer dust 6 nothing
loottable validate "Old Toll Drawer"
loottable preview "Old Toll Drawer" default 202
```

Copper scrap connects to other recipes through its **Scrap Metal** pile tag. The tally is an explicit item because its description, seal and identity matter to a collector. Preview shows the plan without placing goods. Once satisfied, submit the table and have its revision approved. Record the exact ID and revision for callers. For the mine's **Hematite Cartload**, use the same editor with one group, one commodity choice of weight 1, the hematite material ID, the Raw Ore tag ID and `mass 20kg 20kg`. Every successful load of that table has a known payload.

### The ruined customs house

Set a closed, openable strongbox in the customs house. The first explorer to open it gets one search. The first explorer can leave everything there for a later visitor to take. The box remembers whether its contents have been generated, independently of who owns them now.

1. Make the box a container with room for the largest possible result. For a simple customs post, reuse the toll drawer table. For a richer house, give its own table a group that invokes the existing table with `loottable set choice add default <group> drawer 1 table <drawer-id> <revision> default`. That rolls the drawer's choices at the group's destination. To place a roll *inside a container generated by the same table*, give that earlier container choice an `as <key>` and use `into <key>` on the later group.
2. Declare `register item CustomsSearchState number`. Use 0 for untouched, 1 while opening the package and 2 for searched. Set the initial state with `register default item CustomsSearchState 0`. Install a hook on this box, or install the same hook on each new instance created by your dungeon. Give the ordinary hook prog the parameters `opener character` and `box item` in that order.
3. In the prog, inspect the receipt. Below, 81 and 0 stand for the chosen approved table ID and revision. Reachable item hooks can run during a load, so claim the box before calling the table. Settle that claim even if the director authored a perfectly good empty drawer.

```futureprog
if (getregister(@box, "CustomsSearchState") != 0)
  return
end if
setregister @box "CustomsSearchState" 1
var receipt = loadloottable(81, 0, @box, "default")
if (startswith(@receipt, "OK"))
  setregister @box "CustomsSearchState" 2
else
  setregister @box "CustomsSearchState" 0
  send(@opener, "The tray catches before you can search it.")
end if
```

```text
hook create OpenCustomsBox StockCustomsBox ItemOpened
hook install OpenCustomsBox <strongbox-keyword>
```

`StockCustomsBox` is the prog above. Opening triggers it on the box after the character opens it. Closing and opening it again leaves existing contents alone. A successful receipt with zero items or post-commit warnings still spends the search. A placement error, such as a box too small for the chosen item, releases the claim. Correct the cause before retrying. If a restart leaves state 1, inspect the box and any recorded receipt before deciding whether to reopen the claim. Register persistence and loot commitment are separate operations. [Hook attachment and event parameters](../AI/Event_System_for_AI_and_Hooks.md) · [Loot choices and receipts](../Features/LootTables.md)

**Choose the lifetime deliberately.** For one chance per generated dungeon, put the hook and claim on that instance's unique chest. For one chance in the entire persistent ruin even if its furniture is replaced, put the claim on the permanent room and direct the package into the present box. A fixed random seed reproduces a roll when called again. The claim is what prevents a second payout.

## Best practices and traps

| Builder's question | Decision to make before players arrive |
| --- | --- |
| Can the next tradesperson use what the first one makes? | Follow one actual unit through both definitions. Compare the prototype or solid, item count or mass, pile tag, and any required grade. A shared noun such as “ore” only helps readers. |
| How much does one successful action cost the world? | Set output size, depletion and regrowth together. A multi-item forage find charges its pool once. A basket maker can spend field yield before finishing the basket. A repeatable quarry project issues its configured mass every time unless you author a reserve. |
| When should substitution work? | Put a family tag on tools and inputs when alternatives are welcome. Ask for a leaf tag, exact material or exact prototype when a sibling would produce the wrong dish, fitting or sample. Use **Seeds** and **Raw Ore** consistently in both production and consumption. |
| What will the second worker see? | Tell hunters that final breakdown consumes the remains. Show prospectors an old survey marker and miners an exhausted face. Let a searched strongbox contain what the first visitor actually left. These outcomes give the next player information they can act on. |
| Where can work be authorised, interrupted and resumed? | Bind agricultural work to the correct field, mineral work to its recorded site and a shaft to its reserved direction. Give cancellation the identity of the project releasing the claim. Preserve a partial draft for a builder to recover. |

The inn's buyer can now order shoulders by count, rye by mass and honey by harvest. Its carpenter can accept a weighed lot of nails or count out a specific fitting, according to the recipe you chose. The same ingredients, tools and sites can support the next shop, craft or work crew through the tags already on them.
