---
slug: item-authoring
category: Items
summary: Build the Wayfarer Inn's cider mug and lockable guest-room door. Configure components, approve revisions, group items in room descriptions, add damage and set per-item Characteristics.
---

# Building items and equipment

The Wayfarer Inn needs a mug that holds cider and a guest-room door that opens with the issued key. For both, start with an **item prototype** for the object's identity, description, material and size. Attach **component prototypes** to give it behaviours such as holding liquid, being carried or locking. A component prototype can serve several item prototypes. Each **live item** made from them has its own ID and state, such as how much cider is left or which key opens the door. [The Wayfarer Inn](Hospitality_Builder_Guide.md)

## Choose and approve the parts

Find component types with `comp types` and read a type's builder help with `comp typehelp <type>`. Then use `comp list <type>` to see reusable Current component prototypes. For example, `liquidcontainer` is a type, while `WayfarerCiderMugVessel` will be one particular liquid-container design. You can add more than one component to do different jobs. Check the capability overlap if the editor refuses an attachment. An item already using an openable container may reject another component that also supplies its opening behaviour. [Item component reference](https://futuremud.com/docs/items/components)

Use Admin access for the component editor and Junior Admin access or higher for the item editor. Commands with angle brackets need values from your world. When creating a component or item, note the ID and revision shown by `comp edit` or `item edit`. Submit using `comp edit submit` or `item edit submit`. On an account with review authority, open `comp review <id>` or `item review <id>` and use `accept edit <comment>` after reviewing that proposal. Use `comp show <id>` or `item show <id>` to confirm the intended revision is **Current**. Approve a new component before attaching it to an item. [Editable items and review](Generic_Editable_Item_Builder_Workflows.md)

## Build the Wayfarer cider mug

### Make a liquid-holding component

The inn pours a quarter-litre serving into a mug. Give the mug 300 mL capacity so that serving leaves room at the top. A new liquid container starts non-closable. Leave it that way so the mug is open for pouring and drinking. The default liquid is empty. `defaultliquid` can make a newly loaded container come full of a selected liquid, which is useful for a prefilled bottle. Use `none` on this mug so the kitchen supplies the cider.

```text
comp typehelp liquidcontainer
comp edit new liquidcontainer
comp set name WayfarerCiderMugVessel
comp set capacity 300ml
comp set defaultliquid none
comp edit
comp edit submit
```

Review the component as described above, using its ID and a comment such as `accept edit Empty open mug with 300ml capacity`. Check the capacity and default liquid on the Current revision.

### Make and load the item prototype

Use `show materials` to find a suitable ceramic or stoneware solid. The primary noun is what players can try typing to target the object. Use a single word such as `mug`. Give the prototype a distinct lookup name so builders, crafts and merchandise can select it precisely.

```text
item edit new
item set unique wayfarer_cider_mug
item set noun mug
item set sdesc a stoneware cider mug
item set size "very small"
item set weight 350g
item set material <stoneware-solid-id>
item set add <mug-vessel-component-id>
item set desc
```

In the description editor, enter something such as “A wide loop handle joins the thick wall of this stoneware mug. The inn's wheel mark is stamped into the base.” Submit the text with `@` on its own line. Check `item edit` for Holdable. If your game does not attach it automatically, find a Current one with `comp list holdable` and attach it with `item set add <holdable-component-id>`.

```text
item validate
item edit submit
```

Have the item reviewed and check that its material and both components appear on the Current revision. Load a copy with `item load wayfarer_cider_mug`. If the load cannot put it into your hands, look for it in the room. With an open jug of cider held and the mug within reach, pour one serving and drink from it.

```text
pour 250ml jug into mug
look mug
drink mug
```

Use the mug's **item prototype** in the inn's cider merchandise, and stock the actual filled mugs. The restaurant serves those items with their liquid. The kitchen can still keep empty copies for refilling, and staff collect the empty drink vessels by hand after service. [Serving cider by the mug](Hospitality_Builder_Guide.md#serve-cider-by-the-mug)

## Revise a prototype and compare live copies

Use a **revision** for an update that should keep the same prototype ID. Use `item clone <id-or-unique-name> [<new name>]` for another item, such as a traveller's cup that should coexist with the inn's mug. Check the clone's noun and give it a distinct unique lookup name before submission.

Suppose the inn wants a more descriptive handle on its mug. Open the mug by ID, check that the editor has selected a draft revision, change its description, and review the difference before submission. Opening a Current item without specifying an old revision creates a draft when there is no editable revision to resume. Keep one of the earlier mugs for comparison.

```text
item edit wayfarer_cider_mug
item set sdesc a loop-handled stoneware cider mug
item diff
item validate
item edit submit
```

Approve the revision and load a fresh mug. `item show <id> <revision>` lets a builder inspect a specific prototype revision. `locateitem *<mug-prototype-id>` shows the actual loaded item IDs and their prototype revisions. The freshly loaded mug uses the new Current revision. The earlier mug retains its saved revision and state until updated. Check for a skin or an individual description override if the rendered name differs from the prototype. [Individual descriptions and skins](../Items/Item_System_Content_Workflows.md#skins-groups-and-related-content)

If the change is to the mug's **capacity**, edit the component with `comp edit <mug-vessel-component-id>`, submit and approve it separately. Item prototypes store the component revision they reference. For a single item prototype, open a new item revision, remove the old component reference and attach the approved one before reviewing the item revision. Read the resulting component ID and revision on the item to confirm the intended reference. A shared component approval is worth checking against every item design that uses it before updating their references.

For planned world maintenance, `comp update` reconciles item prototype references and loaded instances. `comp update all` also processes stored items. These are world-wide commands with a confirmation step. Schedule them for a maintenance window. Review a few old and new instances first, especially when changing capacity, contents or hardware. For example, updating a live liquid container to a smaller capacity can reduce its current liquid volume. Replacing a component with an entirely different component ID can remove its old instance state. [Revision and update workflows](../Items/Item_System_Content_Workflows.md#revision-and-update-workflows)

## Install the River Room's lockable door

### Build the door and blank key

Use `lockingdoor` for a door with a built-in lock. The separate `door` and `lock` components are useful if you want a plain door that takes an installable lock item. In this example, use the default **Lever Lock** type for both the locking door and the key. If you change the type with `comp set type <type>`, use the same type on both. Set lock-picking or forcing difficulty only to match your world's intended access rules.

```text
comp edit new lockingdoor
comp set name WayfarerGuestDoor
comp set installed guest-room door
comp edit submit
```

```text
comp edit new key
comp set name WayfarerGuestKey
comp edit submit
```

Approve the two components. Build an oak door and a metal key. Before submitting each item, use `item edit` to confirm it has Holdable. If needed, attach a Current Holdable component as you did for the mug. The installer needs to hold the loose door, so choose a weight they can handle.

```text
item edit new
item set unique wayfarer_river_door
item set noun door
item set sdesc a fitted oak door
item set material <oak-solid-id>
item set size large
item set weight 15kg
item set add <locking-door-component-id>
item set desc
```

Describe the door's joinery and fittings, submit the description, then use `item validate` and `item edit submit`. Set up the key separately.

```text
item edit new
item set unique wayfarer_guest_key
item set noun key
item set sdesc a small brass guest-room key
item set material <brass-solid-id>
item set size tiny
item set weight 30g
item set add <key-component-id>
item set desc
```

Describe the blank key and submit it for review the same way. Confirm both item prototypes are Current before loading them.

An installed door belongs to the exit. Stand on the landing where the River Room is north. In the appropriate room overlay, make the exit accept the exact size of the door item. This is a separate setting from how large a character can pass through the exit. Use an empty exit or arrange to remove an existing door before fitting a replacement. [Building exits and door support](Room_Building_Builder_Guide.md)

### Pair the key and install the door

As an administrator on the landing, load the two items. While the door is loose and unlocked, give its built-in lock a combination, then pair the held blank key to it. The combination belongs to that lock instance. If the pairing reports that the lock has no combination, set one and retry. Use a distinct pairing for each guestroom even if both use the same component design.

```text
room set door north large
item load wayfarer_river_door
item load wayfarer_guest_key
locksmith set door random
locksmith pair door key
install door north
```

The installed door starts open. Give the key to an ordinary character and have them hold it while using `close north`, `lock north`, `unlock north` and `open north`. Check access from the opposite side, too. A guest without the matching key should find the closed and locked door blocks passage. If a key fails, check the lock type and pairing on those actual items. If installation fails, check that the door is held, the exit is visible and vacant, and the configured size exactly matches the item's size.

At the zone's conveyancing office, have the authorised inn owner register the working key while holding it. Then attach that registered key to the hotel's River Room.

```text
property addkey "Wayfarer Inn" key "River Room Key" 5cr
roomrent key add "Wayfarer Inn" "River Room" River Room Key
```

Registering the key lets the hotel issue it on check-in. The lock still follows the pairing you set on the physical items. If a door is replaced or a lost key needs to stop working, plan the re-keying and confirm the currently issued key matches the new lock before letting the room again. [Hotel room keys and checkout](Hospitality_Builder_Guide.md#register-rooms-keys-and-furnishings)

## Group spare mugs in the common room

An item group changes how several visible items are described together in a room. Each mug still has its own contents and state and can be targeted individually. A group does not make a stack or a physical container. Use a room-specific form when the description only makes sense at a particular location.

With Admin access, give the group a name and a keyword that differs from the single item's noun. This example groups visible mugs in the common room only. Use the IDs returned when you create the group and form.

```text
itemgroup add Wayfarer Mugs
itemgroup <mug-group-id> keywords mugcollection crockery
itemgroup <mug-group-id> form add simple
itemgroup <mug-group-id> form <form-id> room <common-room-id>
itemgroup <mug-group-id> form <form-id> name mug
itemgroup <mug-group-id> form <form-id> ldesc Several stoneware mugs are set out for the next round.
itemgroup <mug-group-id> form <form-id> desc
```

In the editor, describe the group as a whole, for example “These mugs all carry the Wayfarer's wheel mark.” Submit the text with `@` alone. Check the room and description with `itemgroup show <mug-group-id>`. The `room` command toggles membership. A form with an empty room list applies everywhere. If several forms match, the first applicable form is used, so create specific forms before an unrestricted fallback.

Attach the group to a revision of the mug, then submit and approve.

```text
item edit wayfarer_cider_mug
item set group <mug-group-id>
item edit submit
```

In the common room, put several newly loaded mugs directly in the room on the same layer. They show the group's line when there are multiple visible members. `look mugcollection` displays the detailed group description and its included items, while `look mug` targets an individual mug. A single member still displays its own long description. Mugs inside an ordinary container, including on a table, follow that container's display rules. `item set group none` clears the assignment on a later revision. Use `simple` for fixed prose. A `stacking` form is available for count-based text through a stack decorator. [Content workflow reference](../Items/Item_System_Content_Workflows.md#item-groups)

## Make the cider mug breakable

Attach a `Destroyable` component if the inn's stoneware should take ordinary item damage and eventually break. The component sets the damage budget and multiplies each incoming damage type. For example, `Crushing 1.5` takes 150% of that damage and a multiplier of zero ignores that type. Material still matters for the mug's physical properties and compatibility with other systems. The component supplies its explicit damage modifiers.

Use a dedicated component so tuning the mug does not accidentally alter other items. This example starts with 8 hit points and increased vulnerability to crushing. Use a positive HP formula across every quality and size you expect to load. The formula can use `quality` and `size`. A constant gives the same base threshold to this whole batch.

```text
comp edit new destroyable
comp set name WayfarerBreakableStoneware
comp set hp 8
comp set damage Crushing 1.5
comp edit submit
```

Approve the component. Then make an ordinary item prototype for the result, such as `wayfarer_broken_mug`, a few curved fragments of stoneware. Give it its own noun, description, material, weight and size. Approve it before linking it. The shard prototype needs none of the mug's liquid, lock or merchandise behaviours.

```text
item edit wayfarer_cider_mug
item set add <approved-destroyable-component-id>
item set destroyed <approved-broken-mug-prototype-id>
item validate
item edit submit
```

Once the mug's new revision is approved, place a fresh empty mug in a work area and use `evaluate mug` to inspect its condition. A character who has a suitable in-game smashing attack can `smash mug` while it sits loose in the room. Use `evaluate` again if it survives. A hit may do too little damage to matter. On destruction, the mug leaves play and its configured replacement appears in its place. Use `item set destroyed none` if the broken item should simply disappear. Keep the standard item health strategy unless your game has a specific alternative. `item set strategy none` restores that default.

If players should be able to repair a chipped mug, use a repair kit whose material or tag filters, allowed damage types and severity cover stoneware. Repairing damage and replacing a destroyed mug are separate workflows. A door's `smashable` setting controls its own forced-entry interaction. Choose that policy separately from giving the mug ordinary item damage. [Configuring repair kits](Health_System_Builder_Guide.md#calibrate-a-mechanical-repair-kit)

## Give each mug a glaze with Characteristics

A **Characteristic definition** is a field such as glaze colour. **Values** are what that field can contain. A **profile** supplies the pool used when a new item needs a random value. The `Variable` component attaches the definition and profile to the item design. The item then stores the chosen value. Descriptions can render it wherever the appropriate markup appears.

For the Wayfarer, make a standard text definition for a glaze colour. A standard value supplies its name as text, so it can sit naturally in the description. Use the IDs reported when creating the two values to avoid matching another definition's similarly named value.

```text
characteristic definition add WayfarerGlaze standard
characteristic value add WayfarerGlaze "slate blue"
characteristic value add WayfarerGlaze "moss green"
characteristic profile new WayfarerGlaze standard WayfarerGlazes
characteristic profile set WayfarerGlazes value <slate-blue-value-id>
characteristic profile set WayfarerGlazes value <moss-green-value-id>
characteristic profile show WayfarerGlazes
```

A Standard profile's `value` command toggles membership. Check that the profile contains both glazes. Check `characteristic definition show WayfarerGlaze` for the markup pattern. With a new definition of this name, it is `wayfarerglaze`.

```text
comp edit new variable
comp set name WayfarerMugGlaze
comp set variable add WayfarerGlaze WayfarerGlazes
comp edit submit
```

Approve the component, then attach it to a new revision of the mug. Put the literal token `$wayfarerglaze` into the short description. You can use the same token in the full description, for example “A $wayfarerglaze glaze covers the outside, stopping short of the foot.” Use plain wording for the noun so players can always type `mug`.

```text
item edit wayfarer_cider_mug
item set add <approved-variable-component-id>
item set sdesc a $wayfarerglaze stoneware mug
item edit submit
```

After approval, a normal `item load wayfarer_cider_mug` picks a value from the profile. For a specific order, pass a value name or ID explicitly. Quote a name with spaces.

```text
item load wayfarer_cider_mug wayfarerglaze="slate blue"
item load wayfarer_cider_mug wayfarerglaze=<moss-green-value-id>
```

Look at each mug to see the resolved description. An unrecognised override can fall back to the profile, so confirm the rendered value when a colour matters. Changing the profile changes future random selections. It does not recolour items that already have a saved value. Use a skin when you want a designed appearance variant, and a Characteristic when you want individual values that descriptions or crafts can reuse. A variable craft product can set a chosen glaze or inherit a value from a suitable input. [Characteristic markup](../Markup/Character_Description_System.md#characteristic-variables) · [Craft products](Crafting_System_Builder_Workflows.md)

## Build a wool scarf

Alderbridge's weaver sells a wool scarf. Its Wearable component maps it to the neck of the bodies that should be able to wear it.

### Choose a wear profile and a component

```text
show wears
comp typehelp wearable
comp list wearable
```

Find a neck wear profile for the intended body. Use `show wear <profile-id>` to inspect where that profile actually fits. You can reuse a Current Wearable component that already uses the right profile. To make a dedicated component, use the following commands and note its ID from `comp edit`.

```text
comp edit new wearable
comp set name WorkshopScarfWearable
comp set add <neck-profile-id>
comp set default <neck-profile-id>
comp edit
comp edit submit
```

If the selected profile is already marked as the default, the default command can be skipped. On a reviewer account, open `comp review <component-id>` and inspect the proposed profile and defaults. Type `accept edit <comment>` when ready. Use `comp show <component-id>` to confirm that the reviewed revision is Current.

### Set the item properties and attach the component

Use `show materials` to find the wool solid in your game. Give the item a stable unique lookup name and a noun players can type. Use `workshop_wool_scarf` to select this prototype in a builder command and `scarf` to target the live item as a player.

```text
item edit new
item set unique workshop_wool_scarf
item set noun scarf
item set sdesc a soft wool scarf
item set material <wool-material-id>
item set size small
item set weight 150g
item set add <approved-wearable-component-id>
item edit
```

If the item has no Holdable component, look for a Current one with `comp list holdable` and use `item set add <holdable-component-id>`. Some games add Holdable to new item prototypes automatically. You need Holdable for a scarf players can pick up. A fixed room fixture can omit it.

Next use `item set desc` to open the editor. Enter a full description such as “Soft wool is woven into a narrow scarf with a plain hem at either end.” Submit with `@` on a line by itself. Add an existing tag with `item set tag <id or name>` if this scarf should count towards a craft or another tag-based rule. Submit and have the item reviewed.

```text
item edit submit
item review <item-prototype-id>
accept edit Workshop scarf wear profile and description checked
item show <item-prototype-id>
```

Check that the item is now Current and that its approved component and material are the ones you intended.

### Fit the scarf on its wearer

Load one scarf with `item load workshop_wool_scarf`. Check your inventory and the room. If you can take the item, the load puts it in your inventory. Otherwise it appears in the room. Give the scarf to its intended wearer and have them try it on.

```text
look scarf
wear scarf
inventory
remove scarf
```

Expect the correct full description and a successful wear on the intended body. The inventory should show the scarf worn after `wear` and held after `remove` if the tester has a free hand. If someone cannot put it on, check the profile's body locations, the layers already worn and the garment's fit.

If your game uses `EnableWearableSizing`, inspect the Wearable component's sizing options. Wear profiles describe body locations. Sizing adds a separate instance value for fit. An item configured to fit on first wear captures that fit only after a successful wear. [Garment sizing](../Items/Item_System_Content_Workflows.md#garment-sizing-commands-and-loading)


## Troubleshoot an item build

| Symptom | What to check |
| --- | --- |
| The editor refuses a component | Check that you selected the intended Current component and that it can coexist with the item's other components. |
| The mug will not fill | Check that both vessels are open, the mug has a liquid container with free capacity, and the source vessel is held and has cider. |
| The loose door is present but installation fails | Hold the door, confirm the exit accepts its exact size and check that the exit is vacant. |
| A key fits neither door | Check the key type and the combination on the live items. Register the working key after pairing it. |
| The room shows each mug separately | Check the loaded prototype revision, the group assignment, form locations and how many group members are visible on that layer. |
| Every mug seems to have the same glaze | Inspect the profile and the token in the effective description. Load two distinct explicit values to check the selection. |
| A fresh item works but an older one behaves differently | Compare their item revisions and referenced components. Check saved condition, contents, pairing, fit and description overrides separately. |

## Where to go next

- **Wear, fit and equipment sets.** Use wear profiles for body locations and sizing for individual fit. Start with the scarf above, then expand to a complete outfit that checks layers and body variation. Outfit templates can distribute the pieces through a reusable load definition. [Garment sizing and loading](../Items/Item_System_Content_Workflows.md#garment-sizing-commands-and-loading)
- **Repair, salvage and decay.** Follow a worn tool through damage, repair and end-of-life recovery. Add material-based salvage and consider timed changes separately from damage and destruction. [Ordinary item salvage](../Items/Item_System_Component_Authoring.md#ordinary-item-salvage)
- **Stock, loot and placement.** Cover how finished prototypes reach players through crafts, shop merchandise, loot tables and room placement. One workshop could craft the Wayfarer's mugs, sell the finished goods and salvage damaged returns. [Crafting](Crafting_System_Builder_Workflows.md) · [Shops and stock](Shop_and_Banking_Builder_Guide.md#stock-existing-goods) · [Loot tables](../Features/LootTables.md)
- **Ownership and handover.** Use legal ownership for valuables, borrowed gear or estate property and distinguish the owner from whoever is holding an item. [Estates, probate and item ownership](../Economy/Estate_Probate_and_Item_Ownership.md)

## Core Properties

### Description Properties

| Field | Command and Wayfarer example | Where it is used |
| --- | --- | --- |
| **Unique name** | `item set unique wayfarer_cider_mug` | An optional, stable lookup name for the **prototype**. Use it with `item load wayfarer_cider_mug` and other builders' commands that accept prototype names. Crafts and merchandise can also reference the prototype by ID. A unique name belongs to one prototype ID across its revisions. |
| **Name or noun** | `item set noun mug`, also `item set name mug` | The primary noun used by systems that need a compact item name, such as bundles or sheath contents. Use a single word. Multiple prototypes can have the noun `mug`. Include that word in the player-facing short description so the item is easy to target with `look mug` and `get mug`. |
| **Short description (sdesc)** | `item set sdesc a loop-handled stoneware cider mug` | The short noun phrase used in inventories, emotes and other messages that refer to the item. Usually starts with `a` or `an` and leaves off the final full stop. Players can target words in the visible description, so keep the noun stable when varying the glaze or other details. |
| **Long description (ldesc)** | `item set ldesc A rack of spare mugs stands beside the kitchen door.` | The item's line in a room's contents display. Use a full sentence for a fixture with a reliable position. For a portable mug, `item set ldesc none` returns to the generated line, which uses the short description and the item's current position. The installed door's exit label comes from the door component's `installed` setting. |
| **Full description (desc)** | `item set desc` | Opens the editor for the text a player sees on `look mug`. Describe what a character can observe, such as the shape of the handle, the glaze and the wheel stamp on the foot. The liquid component contributes information about the mug's actual contents. Submit the editor with `@` on its own line. |

Prefer stable unique names for things other builders will place or refer to. Use a prefix such as `wayfarer_` to make related prototypes easy to find. The editor rejects a unique name used by another active prototype and reserves numeric lookup for IDs. Use `item set unique clear` to remove the optional name. Copy a distinct item with a different unique name. For example, `mug` is a useful noun for both the inn's stoneware mug and a traveller's tin mug.

Treat the short description as a phrase that fits into a sentence such as “a courier lifts a stoneware cider mug.” Keep volatile facts such as “full of cider” out of its base description. Put location-specific prose in an ldesc only when the item will stay in that location. Save the maker's stamp and worn marks for the full description. If a live item renders different text than you expect, check its skin and instance overrides, along with the [extra descriptions](#extra-descriptions).

### Description markup

`item set desc` shows the material, writing and trait-check syntax and lists the Characteristic tokens supplied by the item's Variable component. The short and full descriptions also process inline ANSI colour codes. An explicit ldesc supports material substitutions, Characteristics and trait checks. Put writing blocks and longer viewer-specific details in the full description.

| Markup | What it renders | Wayfarer example |
| --- | --- | --- |
| `@material` | The item's current material name in lower case. | `The handle is made from @material.` |
| `@matdesc` | The material's description in lower case, as authored on the solid material. | Preview a sentence such as `The vessel is fashioned from @matdesc.` and adjust the grammar to fit the material's text. |
| `$wayfarerglaze` | The ordinary text of this mug's saved glaze value. | `A $wayfarerglaze glaze covers the wall of the mug.` |
| `$wayfarerglazebasic` and `$wayfarerglazefancy` | The basic and fancy text for that same value, where the value type provides different wording. | Use a basic form in the short description and a more detailed form in the full description. The Standard glaze values above use the same text for all three forms. |
| `&a_an[…]` | Supplies an indefinite article after the phrase has been rendered. | `&a_an[$wayfarerglaze stoneware mug]` can render as “a slate blue stoneware mug.” |
| `#6…#0` | Colours an inline span cyan, then resets the terminal styling. | `A small #6wheel#0 emblem is painted beneath the handle.` |

Copy the token from the editor and choose the ordinary, `basic` or `fancy` form that fits the sentence. Load one copy for each intended glaze when checking a pattern. Keep the prose intelligible with terminal colours disabled. [Writing blocks](#writing-blocks) · [Trait checks](#trait-checks) · [Characteristic markup](../Markup/Character_Description_System.md#characteristic-variables)

### Physical Properties

| Property | How to set it | Why it matters |
| --- | --- | --- |
| **Weight** | `item set weight 350g` for an empty mug, or `item set weight 15kg` for the loose oak door. | The prototype sets the item's own mass. Contents contribute to the combined weight of a filled container. Carrying a loaded container and fitting through an exit are different questions. Supply units explicitly. |
| **Size** | `item set size "very small"` for the mug, or `item set size large` for the door. Use `show itemsizes` for the categories. | A discrete measure of bulk used by fit checks, including container size limits and the exact size an exit accepts for an installed door. Liquid capacity and garment fit come from their own settings. A light, bulky item can still be too large for a container. |
| **Primary material** | `show materials`, then `item set material <solid-id-or-name>`. | Choose the main solid, such as stoneware for the mug, oak for the door or brass for the key. Other metals or finishes can appear in prose. Rules can use the selected material for buoyancy, interaction and repair eligibility, and it supplies `@material` and `@matdesc`. The cider itself is the liquid in the component. |
| **Base quality** | `item set quality standard` or `item set quality good`. Use `show itemquality` to see the scale. | The usual starting quality for a new copy. Crafts, skins and other creation paths can affect the resulting instance. An item's current condition and damage are separate from its authored base quality. |

[Quality values](#quality-values) · [Size values](#size-values)

### Classification Properties

- Use `item set tag <id-or-name>` and `item set untag <id-or-name>` to classify the item for recipes and other rules. Tags are hierarchical. A mug tagged with a specific serviceware tag can also satisfy a rule that accepts its parent tag.
- `item set canskin` toggles whether players can make skins for the prototype. Use it for items where cosmetic variation fits the world. [Skins](#skins)
- `item set hidden` toggles visibility in player-facing information and searches. The in-room visibility of a live item still depends on normal perception and placement.

### Optional properties

| Setting | Command | Where to use it |
| --- | --- | --- |
| **Base cost** | `set currency <currency>`, then `item set cost 2cr` using your currency's amount syntax. | Sets the item's intrinsic catalogue value, stored in global base currency. This can feed shop profiles that use item cost, pawn appraisal and furnishing replacement estimates. A restaurant can set a separate price for a serving. Choose the empty mug's value based on the vessel and set the cider line's actual price in the restaurant. |
| **Custom colour** | `item set colour cyan` or `item set colour "bold cyan"`. Use `item set colour none` to return to the default. | Sets the item's terminal display colour in contexts that apply it. Reserve it for a consistent display convention. Describe actual glaze colour in the text or a Characteristic so the colour remains clear on any client. |
| **High priority** | `item set priority` toggles the flag. | Orders the item earlier in the room's item list. Use it sparingly for a fixture players need to notice, such as a room's key-return box. Check `item edit` before toggling again. |
| **Builder comment** | `item set comment <text>` or `item set comment append <text>`. | Records builder context, such as which inn menu uses the mug or why the door is sized Large. `item list +<keyword>` can search the builder comment. |

### On-load progs

Use `item set onload <prog-id-or-name>` to attach a FutureProg that initialises a freshly created item. For example, an inn can use one to initialise an inspection status on each new guest-room key when that status requires a scripted decision. For a fixed starting value, set a registered variable's default on the prototype with `item set register <variable> <value>`.

Give the prog an `Item` parameter, or `Item, Character` in that order if the prog uses the character who loaded it. Use a Void prog for initialisation. Character can be absent on creation paths without a loader. Check for that case before using the character. The factory can invoke the prog before the item is placed in a room or someone's inventory. Use it for the new item's state, and use other hooks for behaviour that depends on placement. Restoring a saved item from storage restores its persisted state.

The `onload` command toggles the named prog. Use `item edit` to inspect the current list and `item set onload clear` to remove all its on-load progs. Approve the new item revision and load a fresh copy to verify the initialization. [FutureProg for builders](FutureProg_Builder_Guide.md) · [Item lifecycle](../Items/Item_Lifecycle.md)

### Extra descriptions

An extra description slot uses a Boolean prog with one `Character` parameter, the viewer. Use it when someone can recognise something about the item based on a skill, membership or another condition. A slot can replace the short description, replace the full description, append an addendum to the full description, or supply a combination of those fields.

For the Wayfarer mug, let a knowledgeable appraiser recognise the wheel stamp. Create `RecogniseWayfarerStamp` as a dynamic Boolean prog with a `Character` parameter named `viewer`. Use an existing Appraisal trait or substitute the name of one your game uses.

```text
return GetTrait(@viewer, ToTrait("Appraisal")) >= 35
```

On the mug's editable item revision, add a slot bound to that prog. Use the slot number the editor reports.

```text
item set extra add <appraisal-prog-id>
item set extra <slot-number> addendum The stamped wheel is the mark of the Wayfarer Inn.
```

Leave the ordinary description in place so every viewer can see the wheel. Only a qualifying viewer sees the recognition text. Use `item set extra <slot-number> sdesc <text>` or `item set extra <slot-number> desc <text>` when you need a replacement instead. For each field the engine uses the first matching slot that supplies text. `item set extra swap <first> <second>` changes the order and `item set extra remove <slot-number>` removes a slot. A skin or live instance override can take precedence over a short or full replacement. The matching addendum can still appear on the full description. [Item description precedence](../Items/Item_System_Presentation_and_Integration.md)

## Item Components

- Item components are added to items to give them functionality

- Some item component types are mutually exclusive with each other and you can usually only put one of each type on an item (e.g. no more than one IContainer, IOpenable, IMeleeWeapon etc).

- A full list of item component types available is in [Item Component Types](../Data/Item_Component_Types.json)

- A full list of available item component prototypes to attach to items is in [Seeded Item Components](../Data/Seeded_Item_Components.json)

## Supporting Information

### Skins

Skins provide presentation variation without duplicating item behaviour definitions.

An item skin can override:
- item name
- short description
- long description
- full description
- quality

Skins are important whenever:
- behaviour should stay constant but appearance should vary
- players or builders need cosmetic variants
- content should be customised without cloning behaviour-heavy item prototypes
- Players themselves can write the variants with optional admin review and approval, rather than relying solely on an admin builder workflow

Admins can use `itemskin set suggestdesc [<optional extra context>]` to generate AI-assisted full-description override suggestions for a skin. This is not available to ordinary player skin authors because it can incur external API cost.

### Writing Blocks

Use a writing block when an inscription or label should read differently depending on who looks at it. Supply a language and script that exist in your world. The reader needs both and must meet any minimum language-skill threshold. Write the fallback so it describes what the reader can still see.

```text
writing{language,script,style=<style>,colour=<colour>,minskill=<value>}{readable text}{unreadable text}
```

The last three fields are optional. `skill` is an alias for `minskill`, and `color` is an alias for `colour`. Style and colour describe the writing itself in the reading hint. Use valid names from your world. For example, if the inn's stamp is lettered in Common using Trade Script, add this to the mug's full description.

```text
writing{Common,Trade Script,minskill=30}{The letters spell "Wayfarer Inn".}{A line of small letters curves around the wheel stamp.}
```

Use ordinary description text around the block, such as the location of the stamp. Preview the full description with both a fluent reader and someone who lacks the script. A readable inscription and the recognition addendum above cover different things. A reader can sound out the letters, while a knowledgeable appraiser can identify the maker's mark.

### Trait Checks

Use a trait check to reveal a detail when the viewer's trait value meets or exceeds a threshold. Use an existing trait's name or ID. The short, long and full item descriptions can use this markup. For example, the mug's full description can show what an experienced potter notices about the glaze.

```text
check{trait,minvalue}{text if the viewer meets the threshold}{fallback text}
check{Pottery,35}{The glaze has fired evenly, with no crazing along the rim.}{A smooth glaze runs around the rim.}
```

Give the viewer a useful fallback or leave the final braces empty to omit that sentence below the threshold. Use [extra descriptions](#extra-descriptions) when the condition needs a FutureProg, such as membership of a particular clan. [Room description trait checks](../Markup/Room_Description_Markup.md#checktraitminvaluepassfail)

### Quality Values

The complete list of qualities and their associated numeric values are as follows:

- Terrible = 0,
- ExtremelyBad = 1,
- Bad = 2,
- Poor = 3,
- Substandard = 4,
- Standard = 5,
- Good = 6,
- VeryGood = 7,
- Great = 8,
- Excellent = 9,
- Heroic = 10,
- Legendary = 11

### Size Values

The complete list of sizes, their associated numeric values and examples of things that would be that size are as follows:

- Nanoscopic = 0 -> amoeba, bacteria
- Microscopic = 1 -> dust motes, mites
- Miniscule = 2 -> microchips, splinters
- Tiny = 3 -> coins, notes, marbles, nails
- VerySmall = 4 -> pouches, wallets, watches
- Small = 5 -> underwear, hats, bags, daggers, shoes, phones
- Normal = 6 -> shirts, dresses, backpacks, swords, shields, computers, chairs
- Large = 7 -> humans, polearms, doors, trunks, fridges, motorbikes
- VeryLarge = 8 -> ogres, sofas, cars
- Huge = 9 -> giants, trucks, trains
- Enormous = 10 -> dragons, small ships, normal buildings, jumbo jets
- Gigantic = 11 -> ships, skyscrapers
- Titanic = 12 -> world trees, generation ships, titans

