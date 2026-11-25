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

**Replace animals after a hunt.** Give the boars their own `npcspawner` with strategy `Simple`, the boar template, the hunting zone and a spawn room in cover. For a sounder of four, set `npcspawner set target 4` and then `npcspawner set minimum 3`. Three survivors can remain undisturbed. At the next population check after the count falls to two, the spawner can bring it back to four. It counts matching NPCs in the monitored zone, including animals outside the sounder. Use `npc instances <boar-template-id>` and `group show "Wayfarer Sounder"` to identify newly arrived boars, then add each locally with `group addmember "Wayfarer Sounder" <boar>`. Population replenishment and membership are separate. The spawner offers an on-spawn prog, but the engine has no native FutureProg operation to join a live wildlife group. For a hands-off population use individually configured animals; for this named sounder assign the replacements as part of managing the wood.

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
4. **Start the tenant's work in the right room.** In the bread field the tenant uses `field start "Sow Crop" Rye`. In the orchard they use `field start "Plant Orchard" Apples`. These commands capture the intended field, operation and target. A scripted steward uses `StartFieldProject` with the initiator onsite. Run `project supply <project>` on its own to list the material requirements. For the stock sowing project, use `project preview <project> "Seed Stock"` to see which pile will be taken, `project supply <project> "Seed Stock"` to contribute it and `project join <project> <labour>` to join a role. Starting a project does not put the tenant to work automatically.
5. **Make seed policy match the story.** Stock sowing accepts a mass of an eligible seedable material carrying the **Seeds** pile tag. Rye listed on a crop's descriptive seed requirement does not enforce a rye-only payment. If the grange contract requires rye seed, make a dedicated sowing operation and project with an exact rye `commodity` material requirement, a Seeds pile tag and the desired `amount`. That fixes the required delivery for this project. The generic sowing template does not bind the chosen crop to the supplied solid, so a mixed-crop farm needs a separate authoring decision if it promises that guarantee. For a village that exchanges seed freely, the shared requirement is simpler.
6. **Bring in the harvest.** `field look` shows the plot's use and condition. Use `field harvest` when the crop is ready, then supply the harvest labour. Weather, field health, work outcome and configured base output affect the pile the team brings home. A skilled supervisor can improve the work of several labourers. To hire NPCs for a paid job, fund the project's reserve and set the role's hourly rate. [Project labour and pay](Projects_System_Builder_Workflows.md)
7. **Keep bees alongside the apples.** Once the orchard is established, `field start "Install Apiary"` uses the same land. The stock project needs two hive items and a stand. Set the crop's pollination response, for example `field crop set Apples pollination Strong 1 2`. Healthy hives within range support the growing and setting fruit. `field start "Tend Apiary"` maintains the hives and `field start "Harvest Apiary"` draws on stores. For a low apple yield, check the crop's stage and whether the hives are healthy and in range before increasing the crop's base mass.

**What players can do next season.** The tenant must sow the harvested rye plot again. The apple trees remain planted and can enter another fruiting cycle. The beekeeper can tend the hives between fruit harvests. The inn can order each product separately, and a new tenant can take over the established orchard.

**Supply the first season.** Give the tenant a supplier before the grange has ever harvested anything. A seed merchant can stock a starting lot of rye with the **Seeds** pile tag. Stock harvests can yield a small **Seeds** pile for the next sowing. In worlds with the Antiquity recipes, reserve some of the main **Seeded Yield** rye as well. The `select antiquity seed stock` craft turns that grain into a smaller **Seeds** pile of the same material. Keep the rest for the mill. For the apiary, a basket maker can make two wicker beehives and a carpenter can build a wooden hive stand. Worlds with the Antiquity catalogue can use `antiquity_wicker_beehive` and `antiquity_wooden_hive_stand` or sell the finished equipment at a farm supplier. Local equivalents need the **Bee Hive** and **Hive Stand** item tags. Supply them to the installation project by their requirement names:

```text
project supply <apiary-project> "Bee Hives"
project supply <apiary-project> "Hive Stand"
```

**Recruit harvest hands.** Give a farmhand template the `projectworker` AI, enable its `search` setting and set a walkable search range and minimum hourly pay. For example, create `ai edit new projectworker GrangeHand`, inspect `ai show GrangeHand` and set `ai set range 3`. Set its currency and pay threshold to local values. Attach it with `npc set ai add GrangeHand` and approve the template for new workers. Place an eligible, idle worker within reach. Once the tenant starts the harvest, the owner can advertise and fund the current phase:

```text
project pay <harvest-project> currency <local-currency>
project pay <harvest-project> labour "Field Labour" <hourly-wage>
project fund <harvest-project> <cash-amount>
project pay <harvest-project>
```

Choose a wage at or above the farmhand's minimum and fund the expected hours. An available worker looks for a reachable, funded slot on its search cadence. The field starter still supplies materials separately. [Paid project workers](Projects_System_Builder_Workflows.md#paid-project-worker-ai)

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
register default location OreCrew 0
register default location OreLoadsLeft 0
setregister location here OreLoadsLeft 3
setregister location here OreCrew 0
```

Make a one-phase local extraction project. Give **Hematite Cartload** one fixed, 20 kg **Raw Ore** choice as described in chapter 5. Substitute the approved table ID and revision for 42 and 0 below, and the **Hematite Exposure** tag ID for 73. These examples assume a single ground-level working in each room. With several workings in a room, store the reserve on the individual marker and save the selected marker on the project.

Create each named prog with the parameters shown, in order. Use dynamic progs so each call sees the current room balances. Load the helper progs before their callers. [FutureProg editor and compilation](FutureProg_Builder_Guide.md)

**`OreFaceProblem(site location) → text`**

```futureprog
if (@site.items.any(i, istagged(@i, 73)) == false)
  return "The crew needs a surveyed exposure here."
end if
if (getregister(@site, "OreLoadsLeft") < 1)
  return "The seam is worked out."
end if
if (getregister(@site, "OreCrew") != 0)
  return "Another crew has this face."
end if
return ""
```

**`WhyMineOre(actor character) → text`**

```futureprog
return @OreFaceProblem(@actor.location)
```

**`CanMineOre(actor character) → boolean`**

```futureprog
return @WhyMineOre(@actor) == ""
```

**`ClaimOreFace(job project) → void`**

```futureprog
var site = @job.location
if (@OreFaceProblem(@site) == "")
  setregister @site "OreCrew" @job.id
end if
```

**`ReleaseOreFace(job project) → void`**

```futureprog
var site = @job.location
if (getregister(@site, "OreCrew") == @job.id)
  setregister @site "OreCrew" 0
end if
```

On the open extraction template, attach the following progs. After adding the action, replace `<action-name>` with its name shown by the editor. This is the only action that issues ore.

```text
project set why WhyMineOre
project set can CanMineOre
project set start ClaimOreFace
project set cancel ReleaseOreFace
project set finish ReleaseOreFace
project set phase 1 action add prog
project set phase 1 action <action-name> prog TakeOreLoad
```

**`TakeOreLoad(job project) → void`**

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

### Extend the north drift

Give the north drift a **Mine Face** room tag (74 in these examples). At the model tunnel use `room set uniquename north-drift-template`. Use ordinary ground-level rooms here. The surveyor chooses a direction before anyone starts digging. The **cut** creates a named draft package, a room and a reciprocal exit. The **opening** makes the reviewed route available to players. Below are three ways to order the same cut: a crew project, a timbering craft, or a foreman's command at the face.

Declare the registers once. On an initial face, `setregister location here ShaftDirection n` marks an unopened north heading. Use `n`, `s`, `e` or `w` for a level drift, and `d` or `u` for a vertical shaft. Staff can mark another direction when this face is free. `ShaftCrew` records a positive project ID for a crew or the negative ID of a unique physical work order for an individual miner. Zero means available. Store the package as soon as it exists so a partial draft cannot be mistaken for a free face.

```text
register location ShaftDirection text
register location ShaftCrew number
register default location ShaftCrew 0
register location ShaftDraftPackage overlaypackage
register location ShaftDraftRoom location
register project CutDirection text
register item ShaftSite location
register item ShaftBearing text
register item ShaftSurveyor character
```

#### Shared survey and construction progs

**`HeadingProblem(site location, bearing text, expected number) → text`**

```futureprog
if (isnull(@site) or isnull(@bearing))
  return "No heading has been surveyed."
end if
if (istagged(@site, 74) == false)
  return "This is not a mine face."
end if
if (@bearing != "n" and @bearing != "s" and @bearing != "w" and @bearing != "e" and @bearing != "d" and @bearing != "u")
  return "Survey a compass heading or vertical shaft first."
end if
if (@bearing != getregister(@site, "ShaftDirection"))
  return "The survey marks have changed."
end if
if (isnull(getregister(@site, "ShaftDraftPackage")) == false or isnull(getregister(@site, "ShaftDraftRoom")) == false)
  return "A draft at this face awaits the surveyor."
end if
if (getregister(@site, "ShaftCrew") != @expected)
  return "This face is reserved under a different order."
end if
if (@site.exits.any(x, @x.keyword == @bearing))
  return "A passage already runs that way."
end if
return ""
```

**`DraftHeading(surveyor character, site location, bearing text, claim number) → boolean`**

```futureprog
if (isnull(@surveyor) or @claim == 0)
  return false
end if
var problem = @HeadingProblem(@site, @bearing, @claim)
if (@problem != "")
  send(@surveyor, @problem)
  return false
end if
var tunnel = locationbyuniquename("north-drift-template")
if (isnull(@tunnel))
  send(@surveyor, "The survey office needs to designate a model tunnel.")
  return false
end if
var package = CreateOverlay(@surveyor, "North Drift " + totext(@site.id) + " " + totext(@claim))
if (isnull(@package))
  send(@surveyor, "Ask the survey office to inspect the existing plans.")
  return false
end if
setregister @site "ShaftDraftPackage" @package
var shaft = CreateRoom(@package, @site.zone, @tunnel)
if (isnull(@shaft))
  send(@surveyor, "The package exists but the heading needs a room.")
  return false
end if
setregister @site "ShaftDraftRoom" @shaft
if (NameRoom(@shaft, @package, "Beyond the North Drift") == false)
  send(@surveyor, "The survey office must finish naming the draft.")
  return false
end if
var way = LinkCells(@site, @shaft, @package, @bearing)
if (isnull(@way))
  send(@surveyor, "The draft room exists. Ask the survey office to reconcile its connection.")
  return false
end if
send(@surveyor, "Both ends of the new heading are ready for inspection.")
return true
```

`LinkCells` checks both endpoints in the chosen package, including conflicts a player cannot see in the current overlay. Creating the room and linking it are separate operations. Keep the recorded draft on failure. A phase prog returning false cannot rewind the project or a craft.

#### Crew project

Use this route for a heading that takes several shifts. Configure timber and labour on a local project. Bind the following five progs with `project set why WhyCutShaft`, `project set can CanCutShaft`, `project set start StartCutShaft`, `project set finish ReleaseCutShaft` and `project set cancel ReleaseCutShaft`. On the final phase use `project set phase last action add prog` and `project set phase last action <action-name> prog CutShaft`. The project keeps the original direction even if the foreman travels.

**`WhyCutShaft(actor character) → text`**

```futureprog
var site = @actor.location
return @HeadingProblem(@site, getregister(@site, "ShaftDirection"), 0)
```

**`CanCutShaft(actor character) → boolean`**

```futureprog
return @WhyCutShaft(@actor) == ""
```

**`StartCutShaft(job project) → void`**

```futureprog
var site = @job.location
var bearing = getregister(@site, "ShaftDirection")
if (@HeadingProblem(@site, @bearing, 0) == "")
  setregister @job "CutDirection" @bearing
  setregister @site "ShaftCrew" @job.id
end if
```

**`CutShaft(job project) → void`**

```futureprog
@DraftHeading(@job.owner, @job.location, getregister(@job, "CutDirection"), @job.id)
```

**`ReleaseCutShaft(job project) → void`**

```futureprog
var site = @job.location
if (getregister(@site, "ShaftCrew") == @job.id and isnull(getregister(@site, "ShaftDraftPackage")))
  setregister @site "ShaftCrew" 0
end if
```

#### Issue a work order for a craft or command

For the two alternatives, make a non-stackable **North Drift work order** item, a fixed timbering bench at the face, and ordinary item prototypes for a **cutting report** and **survey query**. Give each miner a fresh work order. At the marked face the mine's builder can issue it using `prog execute IssueHeadingOrder <miner> <order>`. Keep issuance under the game's mine authority. The numeric claim belongs to this particular item, so a copy of the same prototype cannot spend it. These two routes work independently of the crew project.

**`IssueHeadingOrder(miner character, ticket item) → boolean`**

```futureprog
if (isnull(@ticket) or isnull(@miner))
  return false
end if
var site = @miner.location
var bearing = getregister(@site, "ShaftDirection")
if (@HeadingProblem(@site, @bearing, 0) != "" or isnull(getregister(@ticket, "ShaftSite")) == false)
  return false
end if
var claim = 0 - @ticket.id
setregister @ticket "ShaftSite" @site
setregister @ticket "ShaftBearing" @bearing
setregister @ticket "ShaftSurveyor" @miner
setregister @site "ShaftCrew" @claim
return true
```

**`OrderProblem(miner character, ticket item) → text`**

```futureprog
if (isnull(@ticket) or isnull(@miner))
  return "Bring your work order to the face."
end if
var site = getregister(@ticket, "ShaftSite")
if (isnull(@site) or @site != @miner.location or getregister(@ticket, "ShaftSurveyor") != @miner)
  return "This order belongs to another surveyor or another face."
end if
return @HeadingProblem(@site, getregister(@ticket, "ShaftBearing"), 0 - @ticket.id)
```

**`UseHeadingOrder(miner character, ticket item) → boolean`**

```futureprog
if (@OrderProblem(@miner, @ticket) != "")
  return false
end if
return @DraftHeading(@miner, getregister(@ticket, "ShaftSite"), getregister(@ticket, "ShaftBearing"), 0 - @ticket.id)
```

**Craft example — timber the heading.** Use a normal two-phase craft with a qualification check for the miner. Set up the first input as one order, the first tool as a stationary timbering bench at the face, and add shoring timber as a separate consumed input if desired. Have the miner hold their own order before starting. The normal product receives the actual items consumed, indexed by input slot. Use that product for the cut so a failed check cannot create the room. A craft-wide `onfinish` receives only the character and can also run on a failed craft.

**`WhyTimberHeading(actor character) → text`**

```futureprog
var orders = @actor.helditems.where(i, isnull(getregister(@i, "ShaftSite")) == false)
if (@orders.count != 1)
  return "Hold exactly one issued heading order."
end if
return @OrderProblem(@actor, @orders.first(i, true))
```

**`CanTimberHeading(actor character) → boolean`**

```futureprog
return @WhyTimberHeading(@actor) == ""
```

**`TimberHeading(inputs item collection dictionary) → item`**

```futureprog
var ticket = collectiondictionarygetfirst(@inputs, "1")
if (isnull(@ticket) == false)
  var miner = getregister(@ticket, "ShaftSurveyor")
  if (@UseHeadingOrder(@miner, @ticket))
    return loaditem(9022)
  end if
end if
return loaditem(9023)
```

Replace 9022 and 9023 with the report and query prototype IDs. On the open craft, after creating both phases and the bench tool, bind the definitions below. Reference the shoring input in the first phase if you added it. Keep other orders outside the inventory plan's reach. The product rechecks the actual ticket and the reserved origin even on a resumed craft.

```text
craft set canuse CanTimberHeading
craft set whycannotuse WhyTimberHeading
craft set tool 1 item <timbering-bench-prototype>
craft set tool 1 room
craft set input add simple
craft set input 1 item <work-order-prototype>
craft set input 1 quantity 1
craft set product add prog
craft set product 1 prog TimberHeading
craft set failphase 2
craft set phase edit 2 30 "$0 checks $i1 at $t1 and files $p1." "$0 finds a fault in the shoring at $t1."
```

The miner uses `craft begin <craft-name>` at the bench. Failure produces no new heading. If work is interrupted, resume the craft with its progress item. If an order is lost or was consumed by a failed craft, the survey office can revoke that specific reservation, then issue a fresh order as described below.

**Command-hook example — the foreman's board.** Install the hook in the board's room. The foreman holds the issued order and types `cutheading`. This route treats the order as authority to cut immediately. Use the craft or crew project where time, materials or a failed skill check matter. A room `CommandInput` hook receives the character, the perceivable hosting the hook, the command and a text collection of arguments. Use a dedicated verb and handle it even when the order is refused.

**`CutHeadingCommand(actor character, host perceivable, verb text, args text collection) → boolean`**

```futureprog
if (@args.count > 0)
  send(@actor, "Hold your survey order and type CUTHEADING.")
  return true
end if
var problem = @WhyTimberHeading(@actor)
if (@problem != "")
  send(@actor, @problem)
  return true
end if
var ticket = @actor.helditems.first(i, isnull(getregister(@i, "ShaftSite")) == false)
@UseHeadingOrder(@actor, @ticket)
return true
```

```text
hook create NorthDriftBoard CutHeadingCommand CommandInput cutheading
hook conditional NorthDriftBoard on
hook install here NorthDriftBoard
```

#### Inspect, open or recover the heading

1. Identify the recorded draft at the **original face** using `sniff here` and `room package list all`. The package name includes the origin room ID and the claim. Inspect it with `room package show <package-id>`. To preview it, use `room overlay <package-id> <revision>`, inspect both ends and the return passage, then `room overlay clear`. A personal preview does not open the mine for other players.
2. Open the draft with `room package open <package-id>` and `room package submit North drift surveyed and shored`. A reviewer uses `room package review <package-id>`, then `accept edit Both endpoints inspected`. Activate the approved package with `room package swap <package-id>`. From the original face without a preview override, follow the new exit and return. Check that its destination is the recorded room.
3. Back at the original face, verify the matching claim before clearing it. Use `setregister location here ShaftDraftRoom null`, `setregister location here ShaftDraftPackage null` and `setregister location here ShaftDirection ""`, then `setregister location here ShaftCrew 0` last. A second use of the old order fails its claim check. For a failed or cancelled craft with **no draft**, first reconcile or end any resumable craft, match the negative claim to the order's item ID, and clear only that claim. A new order must have a new item ID.
4. If the draft contains an unlinked room, keep the reservation while a builder repairs the connection in that package. Use the existing draft room ID. Once both ends agree, continue with review and opening. Do the same inspection after a restart partway through construction. Creating a package, creating a room, linking and saving registers have distinct save boundaries.

If your setting allows miners to open a surveyed heading immediately, put the approval in the success branch of `DraftHeading`, after `LinkCells` returns an exit. Call `ApproveOverlay(@package, @surveyor, "Survey order fulfilled")`. Only if that returns true, call `SwapOverlay(@package)` and check its result before announcing the opening. That choice applies equally to a work order spent at the bench or at the board. Keep a refused or partially activated package on record for inspection.

To extend the mine again, mark the new working with the **Mine Face** room tag and set its own `ShaftDirection` when the surveyor picks the next bearing. For a newly opened shaft that intersects ore, place a vein node there and initialise its reserve once as part of opening the working. For example, place the approved exposure item in the new room and seed `OreLoadsLeft` to two for a short seam. Barren shafts get neither. The miners can now extract two cartloads using stage 2. If you want a hidden survey opportunity instead, lay out tagged ground for stage 1 and leave the visible marker for the prospectors. Copying an overlay's description is insufficient to mark the room as ore-bearing. [Project authoring](Projects_System_Builder_Workflows.md) · [FutureProg and persistent registers](FutureProg_Builder_Guide.md) · [Room overlays](Room_Building_Builder_Guide.md)

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

**`StockCustomsBox(opener character, box item) → void`**

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
hook install <strongbox-keyword> OpenCustomsBox
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
