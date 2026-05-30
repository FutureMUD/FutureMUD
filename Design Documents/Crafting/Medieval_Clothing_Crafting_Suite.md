# Medieval Clothing and Outfit Crafting Suite

The medieval clothing suite now targets complete outfit coverage. The goal is not a handful of culturally specific garments. The goal is that a builder can dress male and female characters of different social classes in each culture without inventing missing pieces.

## Scope

This suite covers:

- Underlayers
- Lower-body garments
- Leg and sock layers
- Footwear
- Main body garments
- Outerwear
- Headwear
- Belts, sashes, and girdles
- Worn containers
- Fasteners, jewellery, and class markers
- Role-specific clothing and accessories for artisans, merchants, religious figures, and military roles

Military armour and weapons remain primarily in the equipment suite, but military outfits should include arming clothing and worn military accessories.

## Architecture

Use three levels of clothing content:

| Level | Purpose |
| --- | --- |
| Shared common pieces | Items that can be reused across cultures without erasing identity, such as plain linen undergarments, simple footwraps, or basic leather belts. |
| Culture-cluster pieces | Items shared by related cultures, such as Western European braies, Islamic sirwal, steppe high boots, or Rus/steppe fur hats. |
| Explicit culture pieces | Named culture-specific garments and accessories, such as Norse hangerok apron dresses, Byzantine silk dalmatics, Song cross-collar robes, Gaelic brat mantles, or Andalusi burnous cloaks. |

The generated culture/status wardrobe is no longer normally seeded as medieval clothing content. Any future shared baseline items must be explicitly named as `medieval_common_*` or `medieval_baseline_*` items and must not masquerade as culture-specific reskins. Complete outfits now point to explicit authored outfit-piece item specs, or to a slot that is deliberately marked shared/common.

MED-OUTFIT-001 added the executable outfit catalogue. MED-OUTFIT-002 through MED-OUTFIT-004 filled all 216 outfits across the 18-culture medieval set. MED-OUTFIT-008 replaces the generated-then-patched clothing architecture with a single authored source path. See [Era Seeder Shared Architecture](./Era_Seeder_Shared_Architecture.md) for the shared records and configuration model.

- shared era records live in `ItemSeeder.Rework.EraDefinitions.cs`
- `MedievalClothingItemSpecs()` returns authored explicit outfit-piece specs
- `SeedMedievalClothing()` seeds those specs through `SeedEraItemSpecs(...)`
- explicit outfit-piece item data and craft data are derived together from the authored catalogue seam
- direct generated clothing/outfit-piece files are no longer the source of truth

## Authored Outfit Piece Catalogue

Every explicit outfit-piece entry has one final item spec. The maintained catalogue carries the stable reference, noun, short description, full description, material, material behaviour, quality, size, weight, cost, tags, components, craft inputs/tools, outfit reference, slot keys, culture key, sex/gender presentation, social class/role, and builder notes.

Player-facing short and full descriptions describe visible cut, silhouette, material, colour, construction, trim, practical use, and social finish. They must not contain builder/admin/meta language, and they must not say that an item "belongs to" an outfit, "fills" an outfit slot, or exists for an explicit catalogue.

Culture keys, outfit references, sex/gender presentation, social class, slot, and piece target remain builder metadata. They belong in tags, stable references, tests, catalogue documentation, and builder notes rather than visible item descriptions.

Variable colour is expected for colourable garments and wearable textile, leather, and fur pieces. Use `Variable_DrabColour` for plain work, field, military, and rough common garments; `Variable_FineColour` for fine or high-status material; and the `Variable_2*` components for contrast bands, borders, tiraz work, embroidery, tablet-woven bands, panels, cuffs, hems, fur edging, or visible trim. Keep `$colour`, `$colour1`, and `$colour2` tokens in the authored short and full descriptions.

Do not use direct culture adjectives in player-facing descriptions. Use silhouette, construction, trim, layering, fastening, material, and social use instead. Direct culture names are still appropriate in tests, docs, stable references, tags, and builder notes.

## Outfit Axes

Every culture should receive outfits for:

| Axis | Values |
| --- | --- |
| Sex/gender presentation | `male`, `female` |
| Social class/role | `peasant`, `artisan`, `merchant`, `noble`, `religious`, `military` |

That means 12 outfits per culture and 216 outfits across the 18-culture medieval catalogue.

## Required Outfit Slots

Every outfit should include these slots:

| Slot | Required? | Notes |
| --- | --- | --- |
| `underlayer` | Required | Shirt, shift, chemise, qamis, rubakha, under-robe, or equivalent. |
| `lower_body` | Required | Braies, trousers, hose, sirwal, skirt, wrap, lower robe layer, or equivalent. |
| `leg_or_sock_layer` | Required unless merged with footwear | Hose, socks, footwraps, leg wraps, onuchi, boot socks. |
| `footwear` | Required | Shoes, boots, sandals, slippers, cloth shoes. |
| `bodywear` | Required | Main visible garment. Usually culture-specific. |
| `outerwear` | Required or documented exception | Cloak, mantle, coat, burnous, sagion, felt cloak, riding coat. |
| `headwear` | Required | Coif, veil, wimple, cap, turban, fur hat, official cap. Usually culture-specific. |
| `belt_or_sash` | Required | Belt, sash, girdle, cord, arming belt. |
| `worn_container` | Required for most outfits | Pouch, purse, book pouch, field pouch, document satchel, writing sleeve pouch. |
| `fastener_or_jewellery` | Required | Brooch, ring pin, cloak clasp, badge, pendant, belt mount. |
| `role_item` | Required for merchant, religious, and military | Tool apron, trade seal, devotional object, book pouch, scabbard, quiver, badge, official chop cord. |

The code-side `EraOutfitSlotSpec` list for medieval clothing uses these exact keys: `underlayer`, `lower_body`, `leg_or_sock_layer`, `footwear`, `bodywear`, `outerwear`, `headwear`, `belt_or_sash`, `worn_container`, `fastener_or_jewellery`, and `role_item`.

## Sharing Rules

Sharing is allowed when the item is genuinely common.

Allowed sharing examples:

- `medieval_common_linen_braies`
- `medieval_common_linen_shift`
- `medieval_common_wool_footwraps`
- `medieval_common_turnshoe_ankle_shoes`
- `medieval_western_linen_coif`
- `medieval_islamic_wrapped_turban`
- `medieval_steppe_high_riding_boots`
- `medieval_rus_steppe_fur_hat`

Sharing limits:

- Every outfit must include at least four culture-specific or culture-cluster-specific items.
- Every class must have at least two class-specific items.
- Male and female variants for the same class must differ in at least two slots unless marked intentionally unisex.
- Main bodywear, headwear, and outerwear should usually carry culture identity.
- Shoes, belts, pouches, and underlayers may be shared more often than bodywear and headwear.

## Complete Outfit Example

A Norse female peasant outfit should be closer to:

| Slot | Example Piece |
| --- | --- |
| underlayer | linen underdress |
| lower_body | wool under-skirt or wrapped lower layer |
| leg_or_sock_layer | wool leg wraps |
| footwear | rough leather shoes |
| bodywear | hangerok apron dress |
| outerwear | heavy sea cloak |
| headwear | wool headcloth |
| belt_or_sash | woven belt |
| worn_container | leather belt pouch |
| fastener_or_jewellery | oval brooch pair |
| role_item | small household key ring or bead string |

It should not be merely:

```text
a coarse wool work tunic with practical gores and pinned straps
```

## Craft Inputs

Final outfit-piece crafts should use appropriate stock:

| Piece Type | Suggested Inputs |
| --- | --- |
| ordinary linen/wool clothing | `Garment Cloth`, `Spun Yarn` |
| merchant/civic clothing | `Broadcloth Stock`, `Spun Yarn` |
| noble/court clothing | `Silk Brocade Panel`, `Embroidered Trim Stock`, `Spun Yarn` |
| tablet-banded or braid-trimmed clothing | `Tablet-Woven Band Stock`, `Garment Cloth`, `Spun Yarn` |
| military underlayers | `Quilted Armour Padding`, `Garment Cloth`, `Spun Yarn` |
| shoes, boots, slippers | `Turnshoe Upper Stock`, `Leather Strap` |
| belts, pouches, scabbard-adjacent accessories | `Prepared Leather Panel`, `Leather Strap`, metal `Tool Blank Stock` where fittings exist |
| brooches, pins, pendants, belt mounts | bronze/silver `Tool Blank Stock`, hammer/anvil/small metal tools |
| fur-edged or winter garments | hair/fur-tagged material input where available, plus garment cloth |

## Craft Naming

Explicit outfit-piece final crafts should use object names:

```text
sew a $colour hangerok apron dress
stitch a $colour brat mantle
tailor a $colour silk dalmatic
sew a $colour cross-collar scholar robe
make a $colour fur-edged kaftan
```

Avoid:

```text
sew work tunic regional pattern 03
make headwear regional pattern 17
```

Generic baseline items may retain neutral craft names only if they are explicitly named as shared baseline content.

## Documentation Requirements

`Medieval_Outfit_Catalogue.md` must list every complete outfit by exact outfit reference:

```text
medieval_outfit_{culture}_{sex}_{class}
```

For each outfit, list the expected slot contents. The slot contents may be stable references or item names if the exact refs are generated from catalogue data, but tests should expose and validate the actual stable references.

## Test Requirements

Add tests that verify:

- 216 complete outfit definitions exist.
- Every outfit has required slots.
- Every outfit references item stable references that exist.
- Outfit slot references are exposed through testing accessors, including intentionally shared/generic slot markers.
- The six MED-OUTFIT-002 cultures have 12 explicit outfits each with no generic/shared slot markers.
- The four MED-OUTFIT-003 cultures have 12 explicit outfits each with no generic/shared slot markers.
- The eight MED-OUTFIT-004 cultures have 12 explicit outfits each with no generic/shared slot markers.
- Explicit outfit-piece final craft names include the named object and reject `regional pattern`.
- Cluster vocabulary tests cover high-belted Carolingian dress, Capetian burgher/guild garments, German/HRE civic and militia clothing, and Iberian saya/pellote/manto/toca/frontier riding garments.
- Eastern-cluster vocabulary tests cover Andalusi qamis/sirwal/burnous/tiraz, Byzantine skaramangion/sagion/silk dalmatic, Abbasid qamis/qaba/caftan/scholar robe, Fatimid linen/cotton/tiraz/court kaftan, Seljuk riding caftans and bowcase belts, Rus rubakha/onuchi/fur/birchbark, steppe felt riding caftans and bowcase-and-quiver belts, and Song cross-collar robes, official caps, padded winter robes, and cloth shoes.
- Explicit outfit-piece craft inputs exercise linen, cotton, silk, paper, lamellar, felt, and fur stock families where those materials appear in the target rows.
- Every outfit has at least four culture-specific or cluster-specific pieces.
- Every class differs from the others in at least two slots.
- Male/female variants differ in at least two slots unless documented as unisex.
- Explicit outfit-piece craft names do not use `regional pattern`.
- Exact outfit references appear in `Medieval_Outfit_Catalogue.md`.
