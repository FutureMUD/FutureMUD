# RPI Shop Conversion Mapping

## Source Semantics

RPI Engine stores shop data on keeper mobs rather than as standalone shop records. The archived source confirms that a keeper with `FLAG_KEEPER` reads a `shop_data` payload from `mobs.*`.

The important location fields are:

- `shop_vnum` - the room where the shop transaction is allowed
- `store_vnum` - the storage room used for listed and delivered stock

RPI commerce checks `shop_vnum` before allowing normal shop commands. If `shop_vnum` is positive and the keeper is not in that room, the player is told to find the keeper at the shop. RPI stock delivery code loads delivery objects into `store_vnum`, not into the keeper's current room.

This means the converter does not need NPC reset placement to map the normal case:

- `shop_vnum` maps to a FutureMUD `PermanentShop` shopfront cell
- `store_vnum` maps to the FutureMUD permanent shop stockroom cell

Some archived shops have `shop_vnum` set to `0`. RPI treats those as not tied to a specific room. FutureMUD permanent shops require a physical shopfront, and the preserved corpus does not include reset files to infer keeper placement, so this pass marks those records invalid rather than guessing.

## Parsed Fields

`RpiNpcWorldfileParser` now parses the structured shop payload:

- base `shop_vnum`, `store_vnum`, `markup`, and `discount`
- economy profile 1 from the shop header line
- economy profiles 2-3 for ordinary shops, or profiles 2-7 for `merch_seven` shops
- `nobuy_flags`
- delivery object vnums
- `trades_in` legacy item-type values

The raw second economy line remains in export data as `AdditionalEconomyValues` so older audit output can still inspect the original tokens.

## FutureMUD Mapping

The converter adds three shop-specific commands:

- `analyze-shops`
- `export-shops`
- `apply-shops`

`apply-shops` follows the same safety model as the other converter apply passes: it requires a seeded FutureMUD baseline, defaults to dry-run mode, and only writes when `--execute` is supplied.

Each ready RPI shopkeeper payload maps to one FutureMUD permanent shop:

| RPI value | FutureMUD target |
| --- | --- |
| keeper mob vnum | source/audit identity only |
| `shop_vnum` | shopfront cell in `ShopsStoreroomCells` |
| `store_vnum` | `Shop.StockroomCellId` |
| `markup` | delivery merchandise sell price multiplier |
| `discount` | delivery merchandise reorder/buyback price multiplier |
| delivery vnums | `Merchandise` rows where imported item prototypes exist |
| `trades_in` item types | buyback only for delivered item prototypes with matching legacy item type |
| economy profiles and `nobuy_flags` | preserved as export/audit data; not applied to runtime pricing in v1 |

Imported merchandise uses fixed legacy pricing:

- `BasePrice` is the imported item prototype cost multiplied by RPI `markup`
- `AutoReorderPrice` is the imported item prototype cost multiplied by RPI `discount`
- `SalesMarkupMultiplier` is `1.0`
- `IgnoreMarketPricing` is enabled so the imported price mirrors the archived shop data
- `AutoReordering` is enabled with `MinimumStockLevels = 1`, matching RPI's delivery behavior of ensuring at least one delivered object exists

## Import Dependencies

Run order matters:

1. `apply-clans --execute`
2. `apply-items --execute`
3. `apply-rooms --execute`
4. `apply-shops --execute`
5. `apply-crafts --execute`
6. `apply-npcs --execute`

The shop pass needs:

- at least one FutureMUD economic zone, used as the default economic zone for imported shops
- imported cells matching RPI `shop_vnum` and positive `store_vnum`
- imported item prototypes with `RPIIMPORT|...` provenance markers for delivery merchandise

Existing shop detection is structural because FutureMUD shops do not have an editable builder-comment provenance field. `apply-shops` skips an import if another shop already has the generated name or the same shopfront/stockroom pair.

## Known Limits

- `shop_vnum = 0` records are invalid in this pass because there is no preserved reset placement data to infer a shopfront.
- Live NPC shopkeeper AI, employment records, shifts, and clock-in behavior are not attached yet.
- Broad RPI buyback categories do not become generic FutureMUD buy rules. V1 only sets `WillBuy` on delivered merchandise whose legacy item type appears in `trades_in`.
- RPI economy flag matrices, `nobuy_flags`, `buy_flags`, and material filters are preserved for audit but not yet mapped to FutureMUD markets, tags, progs, or deal rules.
- Tills, display containers, bank accounts, taxes, line of credit, and employee manager/proprietor records remain builder follow-up.
