# Butchering System

## Purpose

The butchering system lets player characters turn corpses and severed bodyparts into authored item products. It supports three player-facing workflows:

- `butcher <corpse|part> [subcategory]` for organic corpses and bodyparts.
- `salvage <wreck|part> [subcategory]` for non-organic corpse-like remains.
- `skin <corpse>` for removing pelt, hide, skin, or similar whole-corpse products.

The same runtime objects power butchering and salvage. The race's butchery profile chooses whether the breakdown verb is `butcher` or `salvage`; skinning is available separately when the profile includes pelt products.

## Runtime Model

Butchering data is split into two builder-authored object types:

- Race butchery profiles define the verb, required tool tag, skill checks, staged emotes, and which products can be produced.
- Butchery products define the target body prototype, required bodyparts, optional subcategory, optional can-produce prog, and the item prototypes loaded by the product.

Races reference one race butchery profile. Corpse and severed-bodypart item components expose `IButcherable`, which gives the runtime access to the original character, original body, remaining parts, decay state, and already-butchered subcategories.

Player commands validate the target before starting a staged effect:

- The target must be an `IButcherable` with an original race that has a butchery profile.
- The target must not be too decayed.
- The profile verb must match `butcher` or `salvage`.
- The profile's required tool plan must be feasible.
- The target must not already be being butchered, salvaged, or skinned.
- The selected breakdown path must have both a check and at least one phase emote.
- If the profile has no required tool, its active emotes must not refer to `$2`, because no tool item is available for that emote token.

When a command starts, it adds a staged action effect to the actor. The effect emits each configured phase emote and blocks general action and movement until completion or cancellation. If the tool is deleted, quits, or leaves the actor's held or wielded inventory state, the action cancels.

On completion, the corpse or severed-bodypart component loads the matching products into the actor's location. Full breakdowns consume the corpse or bodypart. Subcategory breakdowns mark that subcategory as already processed and leave the corpse or bodypart available for other subcategories or final breakdown.

## Product Applicability

A butchery product applies to a corpse or bodypart only when all of the following are true:

- The target's original body prototype counts as the product's target body prototype.
- The target currently contains every required bodypart for the product.
- The product's can-produce prog, if any, returns true for the actor and target item.
- For subcategory commands, the product's subcategory matches the requested subcategory.

This matters most for severed bodyparts and partially dismantled bodies. Products are evaluated against the actual remaining parts exposed by `IButcherable.Parts`, not merely the original race's full body plan.

## Damage And Outcomes

Each product item has a normal result and may also have a damaged result. The runtime computes the damage ratio for the product's matched bodyparts:

- If the damage ratio is at or above the product item's damaged threshold, the damaged proto is produced when one is configured.
- If the actor's butchery or skinning check is a fail or major fail, the damaged proto is also used when configured.
- If no damaged proto is configured, the normal proto is produced even when the bodypart is damaged or the check fails.

Stackable product prototypes load as one stack with the configured quantity. Non-stackable prototypes load as separate items.

## Persistence

The database stores profile and product definitions:

- `RaceButcheryProfiles`
- `RaceButcheryProfiles_BreakdownChecks`
- `RaceButcheryProfiles_BreakdownEmotes`
- `RaceButcheryProfiles_SkinningEmotes`
- `RaceButcheryProfiles_ButcheryProducts`
- `ButcheryProducts`
- `ButcheryProducts_BodypartProtos`
- `ButcheryProductItems`

Runtime corpse and severed-bodypart item component XML stores live progress:

- Corpses store whether they have been skinned.
- Corpses and severed bodyparts store already-butchered subcategories.

That live progress prevents repeated skinning or repeated subcategory harvesting after a save and reload.

## Builder Workflow

Build butchery data in this order:

1. Build or identify the item prototypes that should be produced.
2. Create butchery products with `butcheryproduct`.
3. Create or clone a race butchery profile with `butchery`.
4. Add checks and phase emotes to the profile.
5. Attach products to the profile.
6. Attach the profile to a race with `race set butcher <profile>`.

### Butchery Products

Use `butcheryproduct edit new <name> <body> <bodypart> <proto> [quantity]` to create a product, then refine it with `butcheryproduct set`.

Important settings:

- `category <category>|clear` sets the optional subcategory used by `butcher corpse <subcategory>` or `salvage wreck <subcategory>`.
- `pelt` toggles whether the product is produced by `skin` instead of `butcher` or `salvage`.
- `body <body> [parts ...]` changes the target body prototype and required bodyparts.
- `part <part>` toggles an additional required bodypart. A product must always have at least one required part.
- `prog <prog>` sets a boolean prog accepting `(character, item)` to decide whether the product can be produced.
- `item add <quantity> <proto> [<damaged quantity> <damaged proto> <damage%>]` adds an output line.
- `item <id|number> quantity <quantity>` changes normal quantity.
- `item <id|number> proto <proto>` changes normal proto.
- `item <id|number> threshold <damage%>` changes the threshold for damaged output.
- `item <id|number> damaged <quantity> <proto>` changes damaged output.
- `item <id|number> nodamaged` clears damaged output.

`butcheryproduct show` displays both the row number and database ID for product items. Builder commands accept either.

### Race Butchery Profiles

Use `butchery edit new <name>` or `butchery clone <old> <name>` to create a profile.

Important settings:

- `verb butcher|salvage` chooses the player command used for breakdown.
- `tool <tag>|none` sets or clears the required held tool tag.
- `skindiff <difficulty>` sets the skinning check difficulty.
- `can <prog>` sets a boolean prog accepting `(character, item)` for overall access.
- `why <prog>` sets a text prog accepting `(character, item)` for custom failure messaging.
- `product <product>` toggles a product on the profile.
- `check main <trait> <difficulty>` sets the full-breakdown check.
- `check <subcategory> <trait> <difficulty>` sets a subcategory check.
- `check <subcategory> remove` removes a subcategory check and its emotes.

Phase emotes are required for any runtime path players can use. Use `$0` for the actor, `$1` for the corpse or bodypart, and `$2` for the tool. Only use `$2` when the profile has a required tool.

Skinning phases:

- `skinemote add <seconds> <emote>`
- `skinemote remove <number>`
- `skinemote swap <number> <number>`
- `skinemote edit <number> <emote>`
- `skinemote delay <number> <seconds>`

Main breakdown phases:

- `emote add <seconds> <emote>`
- `emote remove <number>`
- `emote swap <number> <number>`
- `emote edit <number> <emote>`
- `emote delay <number> <seconds>`

Subcategory breakdown phases:

- `subemote <subcategory> add <seconds> <emote>`
- `subemote <subcategory> remove <number>`
- `subemote <subcategory> swap <number> <number>`
- `subemote <subcategory> edit <number> <emote>`
- `subemote <subcategory> delay <number> <seconds>`

## Builder Checklist

Before attaching a profile to a race, verify:

- The profile has the correct verb.
- The tool tag is set, or all phase emotes avoid `$2`.
- `check main` exists if full breakdown should be possible.
- Main breakdown has at least one `emote`.
- Each product subcategory has a matching `check <subcategory>` and at least one `subemote`.
- Skinning has at least one pelt product, at least one `skinemote`, and a suitable skinning difficulty.
- Product required bodyparts exist on the intended target body and represent the actual part that gates the product.
- Damaged products have sensible thresholds and damaged quantities.
- Any can-produce or can-butcher progs accept `(character, item)` and return the expected type.

## Current Limitations

- The first staged action tick is currently scheduled by the command's initial delay, while phase delays control time between subsequent phase emotes.
- Profiles and products are direct editable items rather than revisioned items, so changes affect live content after save.
- Skinning is only supported on whole corpses, not severed bodyparts.
- There is no stock-data wizard for generating complete butchery packages from a body prototype; builders author products and profiles explicitly.
