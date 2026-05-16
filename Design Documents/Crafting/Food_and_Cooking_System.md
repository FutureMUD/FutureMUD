# FutureMUD Food And Cooking System

## Scope
This document describes the current new-style food system centred on `PreparedFood`, recipe-initialised prepared foods, and the `cook` command facade over crafts.

The legacy `Food` component still exists for old content. It is intentionally not mutated by the prepared-food implementation.

## Runtime Model
`IPreparedFood` extends `IEdible` and is implemented by the `PreparedFood` item component. The same component is used for:

- directly loaded food items such as apples, berries, summoned bread, shop stock, and forageable prototype yields
- foods created by crafts through `CookedFoodProduct`
- prepared foods later used as ingredients in another cooking craft

Direct foods are complete as soon as their ordinary item prototype is loaded. They store base nutrition, bites, serving scope, taste/short/full templates, a default ingredient ledger, default drug doses, quality scaling, shelf life, stale/spoiled behaviour, and liquid absorption limits on the component prototype.

Recipe-created foods still load an ordinary prepared-food item prototype. The craft product then appends ingredient ledger entries, drug doses, liquid contributions, freshness information, and approved transferable effects from the craft inputs. The product is an initializer, not a second food implementation.

`CookedFoodProduct` can optionally remove input drugs and transferable food effects while still recording the consumed ingredients. This is intended for processes such as boiling, filtering, or otherwise purifying food and liquids before they become the final prepared food. The flag is off by default so ordinary recipes preserve ingredient doses and effects.

## Serving Scope
Prepared food has two serving scopes:

- `WholeItem`: one item is one food serving, such as one apple, one loaf, or one muffin.
- `PerStackUnit`: each stack unit is one serving, such as berries, nuts, mushrooms, roots, or ration pieces.

The eating path allows `PerStackUnit` prepared foods to be eaten from a stack without first splitting off a single item. Nutrition, bites, drug dose delivery, and serving depletion are per stack unit. When the current unit is fully eaten, the stack quantity is decremented and the next unit starts with a fresh bite count.

## Freshness
Prepared food stores `CreatedAt`, optional stale and spoiled timers, stale/spoiled nutrition multipliers, and stale/spoiled drug doses. Freshness is evaluated lazily at consumption and display time.

By default, stale food is less useful nutritionally and spoiled food can be made nutritionally worthless. Builders can make stale or spoiled food actively harmful by configuring stale drug doses or a stale-eat FutureProg.

## Taste And Description Templates
Prepared food renders taste, short-description, and full-description templates lazily against the live item. This matters for direct `loaditem`, spell-created items, shop stock, and FutureProg-created variants because item variables may be applied after component construction.

Supported template tokens include:

- `{quality}`
- `{freshness}`
- `{primary}`
- `{ingredients}`
- `{additives}`
- `{ingredient:<role>}`
- `{var:<definition name>}`
- `{bites}`
- `{servings}`

The `{var:...}` token reads the parent item's `IVariable` component at render time.

## Drugs And Liquids
Prepared food can carry ingested drug doses from three sources:

- default profile doses authored on the component prototype
- recipe-propagated doses from prepared-food or liquid inputs
- later liquid exposure absorbed by the prepared food

Food and liquid drinking both use the shared ingested-dose helper. Only drugs that support the `Ingested` vector are delivered by eating or drinking.

Liquid exposure is handled before ordinary contamination. Prepared food absorbs up to its configured limit and turns absorbed liquid into ingredient ledger entries and ingested-capable drug doses. Any overflow continues through normal item contamination.

Cooking recipes that enable `CookedFoodProduct` purification omit propagated drug doses from prepared-food and liquid inputs and suppress `IIngredientTransferEffect` transfer from consumed item inputs. The output prototype's own default profile doses and stale doses are still honoured.

## Cooking Crafts
Cooking uses the existing craft system. `CookedFoodProduct` is a craft product type that loads a prepared-food item prototype and initializes the same runtime component from consumed craft inputs.

Builders can use:

- `cook list`
- `cook <recipe>`
- `cook begin <recipe>`
- `cook resume <progress item>`
- `cook view <recipe>`
- `cook preview <recipe>`

The `cook` command only filters and dispatches cooking crafts. Tools, phases, echoes, checks, availability progs, and interruption behaviour remain ordinary craft behaviour.

## Foraging And Direct Creation
Forageable item yields do not need a new forage process. Point the `Foragable` at a prepared-food item prototype, and the normal forage item-loading path produces usable food.

Likewise, FutureProg `loaditem`, spell `createitem`, shop prototype loading, and direct builder load all work with prepared food because the item prototype itself owns a complete food profile.

## Stock Data
`CookingSeeder` installs starter content:

- direct prepared-food component profiles
- direct apple, berry, mushroom, muffin, and baked-apple item prototypes
- stackable per-unit berry and mushroom examples
- stock cooking and forageable food tags
- a sample `bake apple` craft with `CookedFoodProduct`

The package is idempotent and tracks its own records by stable component names, item short descriptions, tags, and recipe names.
