# Analyse capability

`analyse <carried item>` (alias `analyze`) identifies possible recipes understood by the character. It is a binary capability, not a trait. The target must be a visible external inventory item, including worn items; room items, other characters' possessions and closed-container contents are outside this command's targeting surface.

## Eligibility and installation

`CheckType.AnalyzeEligibilityCheck` is persisted as **229**. The command checks it once at `Difficulty.Normal`, with no trait parameter, and uses `IsPass()`. Both skill seeders install expression `0` using **Static Check**, so enabling this optional feature is an explicit content choice. Existing worlds missing the check receive the same disabled deterministic setup during normal check loading. The loader creates a zero-modifier static template if the existing Static Check template cannot supply it; it does not overwrite customised templates or existing checks.

Configure the check expression through the normal check/trait-expression builder tools:

- Universal opt-in: `100`.
- Roster: `100 * {allowed class=<resolved class role IDs>}`.
- Exact pairs: `100 * ({c1 class=<class ID>} * {s1 role=<subclass ID>} + {c2 class=<other class ID>} * {s2 role=<other subclass ID>})`.

The angle-bracket values are placeholders, never literal installation values. Resolve stable content keys to actual IDs in the target world. `class=` selects Class roles; `role=` permits any role type. Use disjoint pairs to keep the expression binary. There are no trait-expression parameters, no `variable` trait and no Analyse trait, cap, starting value, branch, improvement or per-item history. All eleven difficulty modifiers must be zero. With this setup, 0 always fails and 100 always passes, including when bonuses stage the difficulty to Automatic or Impossible. The Static Check has no improvement or branching path. The ordinary Capability Check template has nonzero modifiers and is unsuitable.

Trait-expression race, culture, merit and role options resolve their references from the owning expression's gameworld and cache only those objects. This applies to database loading, string construction and builder editing; it avoids first-world references when several worlds share a process.

## Recipe access and output

The read-only `CraftAnalysis` view selects valid **Current** revisions, requires the actual `AppearInCraftsListProg` to return true (including for administrators), possession and a positive raw value of any configured `CheckTrait`, and permission from `CanUseProg` when supplied. Authored minimum crafting thresholds, tribe/knowledge gates and other restrictions belong in those existing progs. There is no new universal proficiency number: a recipe without a check trait may intentionally be available to anyone who passes its progs.

Only successful products participate. An inaccessible match never stops searching other recipes. Hidden, prohibited, untrained, unsupported and unmatched recipes all yield the same message: “You cannot determine a recipe for that item with your current crafting understanding.” No names, counts or restriction reasons disclose hidden candidates.

For each eligible alternative the command lists ingredient requirements with `ICraftInput.HowSeen` and tool requirements with `ICraftTool.HowSeen` and desired state. Fire and workstations are shown when represented by the recipe's tools; no fire or station is inferred from a product name, phase echo or script. This is a recipe view, not proof of this item's inputs or manufacture history. It does not check material availability, reserve tools, execute a craft, roll crafting checks, teach a recipe, consume anything or disassemble the target.

## Reverse matching contract

`ICraftProduct.MatchesForAnalysis` is distinct from the broad `IsItem` search hook. Its default is false. Overrides must be side-effect free and must not execute production progs to guess an answer.

| Product | Supported match |
| --- | --- |
| Exact `SimpleProduct` | Prototype identity and skin identity, plus the prototype material; no material-defining input override |
| Exact `CookedFoodProduct` | Same fixed prototype, skin and material checks; displayed inputs remain possible recipe requirements, not the food's ingredient ledger |
| `LiquidProduct` | Prototype, skin, prototype material and its existing liquid/volume predicate; no material override |
| `CommodityProduct` | Existing exact material, tag and characteristic-set predicate, with every characteristic fixed; no input-derived material or characteristics |
| Variable, input-derived, scripted, unused/scrap-input, money, NPC and diagnostic products | No reverse match until their type implements a defensible matcher |

Item prototype **IDs** match across prototype revisions, as existing production resolves the currently loaded prototype by ID. Skin IDs must agree, including an absent skin. Only current valid recipe revisions are offered; obsolete recipe revisions do not reveal historical recipes. Quantity/weight of a recipe batch is a requirement, not evidence that a particular item was produced in that batch. No actual provenance is claimed.

## Future Armageddon seeder binding manifest

This is an integration handoff, not a change to the active Armageddon branch. Resolve each package-owned main-guild class role through its stable managed-record identity to the target database's actual `ChargenRoles.Id`. Reject missing, ambiguous, duplicate or non-Class bindings; never guess IDs from names or illustrative examples. Only then write `100 * {allowed class=<seven comma-separated IDs>}` into this check's expression. Do not seed an Analyse trait or copy source openings/caps into player data.

| Capability | Recovered main guild | Source line | Opening/cap/go-on (provenance only) |
| --- | --- | --- | --- |
| `arm.capability.analyze` | Psionicist | 890 | 10 / 90 / 80 |
| `arm.capability.analyze` | Pick-pocket | 1132 | 10 / 90 / 80 |
| `arm.capability.analyze` | Burglar | 1192 | 10 / 90 / 80 |
| `arm.capability.analyze` | Warrior | 1260 | 10 / 90 / 80 |
| `arm.capability.analyze` | Assassin | 1322 | 10 / 90 / 80 |
| `arm.capability.analyze` | Ranger | 1396 | 5 / 40 / 30 |
| `arm.capability.analyze` | Merchant | 1490 | 10 / 90 / 80 |

There are **no recovered subguild grants**. Universal mode is an explicit alternative for other games. A pair matrix requires separately authored allowed pairs; it is not inferred from this historical roster. No Armageddon class identities appear in generic runtime logic. Actual Armageddon IDs remain installation-specific and must be resolved by that future seeder integration; this independent branch does not modify or run the active package.

## Verification boundaries

The [2026-10-16 implementation receipt](./Analyse_Implementation_Receipt_20261016.md) records exact automated/native outcomes, evidence paths and baseline suite failures.

Focused runtime tests exercise static eligibility and role predicates in separate worlds, allowed and denied pairs, no roles and universal mode, bonus-driven difficulty staging, recipe access and eligible alternatives, output requirements and conservative matching. Native acceptance uses an isolated MySQL restore of a known clean world, the current branch's binaries, visible carried targets, hidden and denied recipes, repeat queries and representative restart checks. Local receipt paths and exact counts are recorded in the implementation handoff; a successful build alone does not prove command behaviour.
