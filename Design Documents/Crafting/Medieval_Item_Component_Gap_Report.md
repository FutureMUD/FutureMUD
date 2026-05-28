# Medieval Item Component Gap Report

This report records which medieval stock items use implemented engine components and which remain data-only props until new runtime support exists.

## Implemented In V1

The following component families are live and are in scope for medieval v1:

| Component Family | Stock Prototype Use |
| --- | --- |
| `SealStamp` | Signet rings, office seal matrices, notary kits, and guild stamps use `SealStamp_Antiquity_BronzeSignet` until setting-specific seal stamp variants are worth splitting. |
| `Sealable` | Charters, envelopes, document bundles, account rolls, document satchels, ledger chests, trade bales, chests, strongboxes, notary kits, and tax/customs kits use `Sealable_Document_Wax`, `Sealable_Envelope`, or `Sealable_Container_Wax`. |
| `MeasuringInstrument` | Balance scales, standard/false weights, grain measures, wine measures, oil measures, and tax/customs kits use the existing weight, dry-measure, or fluid-volume measurement prototypes. |
| `OfferingReceiver` | `medieval_offering_basin` uses `OfferingReceiver_Antiquity_VotiveBasin` until medieval-specific devotional receiver variants are worth splitting. |
| `Crossbow` and ammunition | `medieval_weapon_common_crossbow` uses the live `Crossbow` component; `medieval_weapon_common_crossbow_bolts` uses `Ammo_BroadheadBolt`. |
| Worn and carried containers | Military harnesses, packs, quivers, satchels, pouches, physician bags, and clothing pouches use existing wear/container prototypes. |
| Furniture and liquid containers | Furniture surfaces, barrels, cups, pots, bowls, sacks, chests, shelves, and desks use existing container/liquid-container prototypes. |
| Medical basics | Bandages and poultice cloths use `Bandage_Simple`; stretchers use the existing drag-aid stretcher component. |

Implemented live component stable references:

- `medieval_writing_{culture}_office_bundle`
- `medieval_writing_parchment_charter`
- `medieval_writing_sealable_envelope`
- `medieval_writing_account_roll`
- `medieval_writing_document_satchel`
- `medieval_writing_ledger_chest`
- `medieval_writing_office_signet_ring`
- `medieval_writing_office_seal_matrix`
- `medieval_writing_notary_kit`
- `medieval_writing_guild_stamp`
- `medieval_household_boarded_chest`
- `medieval_household_lockable_strongbox`
- `medieval_trade_sealable_bale`
- `medieval_trade_tax_customs_kit`
- `medieval_trade_balance_scale`
- `medieval_trade_standard_weight_set`
- `medieval_trade_false_weight_set`
- `medieval_trade_grain_measure`
- `medieval_food_wine_measure_jug`
- `medieval_food_oil_measure_jug`
- `medieval_offering_basin`
- `medieval_weapon_common_crossbow`
- `medieval_weapon_common_crossbow_bolts`

## Prop-Only V1 Gaps

These items are seeded because builders need the visible stock, but richer behaviour remains deferred:

| Gap | Stable Reference | Current Treatment |
| --- | --- | --- |
| Length/surveying measurement | `medieval_surveyor_measuring_rope` | Holdable prop with measurement tags; no `MeasuringInstrument` component until length/surveying modes exist. |
| Musical instruments | `medieval_music_psaltery` | Holdable leisure prop until instrument components exist. |
| Rules-aware game sets | `medieval_game_chess_set` | Container/prop until game-set rules exist. |
| Animal tack and harness | `medieval_horse_tack_display_set` | Trade/decor prop until animal tack and harness behaviour exists. |

## Not Component Gaps

The broader food, furniture, jewellery, equipment, medical, and writing stock deliberately uses existing generic components where the engine already has enough behaviour:

- A foodway platter or ration does not need a dedicated cuisine component in v1.
- A war banner does not need a rules-aware standard component in v1.
- A devotional token does not need prayer/offering behaviour in v1 unless it is meant to receive offerings; the seeded offering basin is the deliberate live receiver surface.
- Door bars, lockplates, and keyrings are security props, not door runtime replacements.
- Stained glass panels, glazed roof tiles, and lantern panes are visible craft stock rather than architecture/window runtime.
- Writing tools without a live scribing implement component are still useful as visible craftable stock; writable surfaces continue to use `PaperSheet` or book components where appropriate.

## Shared Historic Foundations

Cross-era apparatus is promoted to `historic_*` instead of being duplicated as culture-specific medieval stock when the object is genuinely general: hearths, updraft kilns, looms, sewing needles, shears, awls, dye vats, tanning racks, querns, oil lamps, anvils, tongs, hammers, and bellows.

Antiquity-specific garments, weapons, jewellery, foodways, and status goods remain `antiquity_*`; medieval silhouettes and technology remain `medieval_*`.
