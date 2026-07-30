# FutureMUD Renaissance Household Expansion Dependency Request

## Purpose

This addendum records the new reusable item-component prototypes assumed by the 600-row Renaissance household expansion. The original household catalogue's fourteen requested component prototypes and four requested materials remain authoritative in `FutureMUD_Renaissance_PrimaryIndustry_UsefulSeeder_Impact_Reference.md`.

Implementation status: all six profiles in this addendum, the thirteen supportable profiles from the original request, and `CashRegister_PreIndustrial_TillChest` are now seeded and exported. The till chest uses `LockingCashRegister`, retaining shop/currency behavior while supplying the requested built-in lock.

No new runtime item-component type is required. The display plinth uses the existing `Container` type. The five lockable furniture profiles use the existing `LockingContainer` type, which supplies their container, openable, lockable, and lock interfaces without component stacking.

## New component prototypes

| Exact component name | Existing type | Required configuration | Catalogue use | Safe fallback before seeding |
| --- | --- | --- | --- | --- |
| `Container_PreIndustrial_Display_Plinth` | `Container` | Open surface; non-transparent; non-locking; normal-to-large supported-item capacity; no lid; intended for fixed or portable stands | Plinths, pedestals, urn stands, lamp stands, offering stands, and folding display supports | `Container_Side_Table` or `Container_Small_Table`, accepting table-oriented capacity and naming |
| `LockingContainer_PreIndustrial_SmallCabinet` | `LockingContainer` | Small/normal opaque cabinet; openable; built-in lock; non-transparent | Lockable nightstands, wall cabinets, compact cupboards, and small garment cabinets | `LockingContainer_Lockbox`, accepting incorrect horizontal form and reduced furniture capacity |
| `LockingContainer_PreIndustrial_LargeCabinet` | `LockingContainer` | Large/very-large opaque upright cabinet; openable; built-in lock; non-transparent | Lockable wardrobes, armoires, cupboards, armoury cabinets, and large institutional cabinets | `LockingContainer_Footlocker`, accepting incorrect chest orientation |
| `LockingContainer_PreIndustrial_DrawerChest` | `LockingContainer` | Large opaque drawer-chest capacity; openable; built-in lock securing the unit; drawers represented as one logical inventory | Lockable chests of drawers, coin drawers, archive drawers, and specialist sorting cabinets | `LockingContainer_Footlocker`, losing drawer form |
| `LockingContainer_PreIndustrial_DisplayCabinet` | `LockingContainer` | Large transparent cabinet; openable; built-in lock; transparent contents | Secured glass-front cabinets, specimen cases, plate cabinets, and court or guild display furniture | `Container_Display_Case`, losing built-in locking |
| `LockingContainer_PreIndustrial_Desk` | `LockingContainer` | Normal/large opaque desk storage; openable; built-in lock; intended to coexist with a `Table_*` component | Lockable writing desks, counting desks, secretaries, and escritoires | `Container_Desk_Drawers` or `Container_Writing_Desk_Drawers`, losing built-in locking |

## Existing dependencies reused by the expansion

The expansion continues to presume the original component requests:

```text
CashRegister_PreIndustrial_TillChest
Container_PreIndustrial_CompartmentBox
Container_PreIndustrial_LiddedBasket
Container_PreIndustrial_LiddedHamper
LContainer_PreIndustrial_Cup_150ml
LContainer_PreIndustrial_Bowl_750ml
LContainer_PreIndustrial_Basin_5L
LContainer_PreIndustrial_Ewer_2L
LContainer_PreIndustrial_Pitcher_4L
LContainer_PreIndustrial_Pot_12L
LContainer_PreIndustrial_StorageJar_12L
LContainer_PreIndustrial_StorageJar_30L
LContainer_PreIndustrial_Vat_125L
LContainer_PreIndustrial_Vat_500L
```

The materials `gourd shell`, `papier-mache`, and `birch bark` are now seeded. Requests for `mother of pearl` resolve through the existing alias of canonical `mother-of-pearl`; no duplicate solid material is created. This expansion requests **no additional materials**.

## Implementation and verification contract

- Seed these six profiles before the expanded catalogue is implemented.
- Each profile provides the sole `IContainer`/`IOpenable`/`ILockable` implementation on its item. Do not combine it with another dry, locking, or liquid-container provider.
- The display cabinet must expose transparency; the other locking profiles remain opaque.
- The drawer chest represents one logical inventory rather than independent per-drawer subcontainers.
- `LockingContainer_PreIndustrial_Desk` may coexist with one `Table_*` component because table occupancy does not provide another container interface.
- `Container_PreIndustrial_Display_Plinth` must be reusable on both fixed and genuinely portable display furniture; portability comes from the item prototype's `Holdable` component, not from this container profile.
- Export the prototypes to `Design Documents/Data/Seeded_Item_Components.json` and add source-truth, exclusivity, and idempotency tests.
