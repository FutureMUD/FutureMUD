# Medieval Testing Quality Gates

This document records the second-pass quality tests that should be added or strengthened for the medieval suite.

## Problem With Current Tests

The current tests are useful for dispatcher wiring, count thresholds, component presence, and broad documentation. They are not enough to prevent generic matrix output.

The second-pass tests must reject shallow implementations where cultures differ only by:

- Builder notes
- Tags
- Appended cue phrases
- `regional pattern NN` craft names
- Generic stable-reference families
- Main garments without complete outfit support

## Required Test Categories

### 1. Complete Outfit Count Tests

Expose a testing accessor such as:

```csharp
internal static IReadOnlyDictionary<string, IReadOnlyCollection<MedievalOutfitSpec>> MedievalOutfitsForTesting
```

The current implementation exposes tuple-shaped accessors for the same contract:

```csharp
internal static IReadOnlyCollection<(string Key, bool RequiredForAllOutfits, IReadOnlyCollection<string> RequiredForRoles)> MedievalOutfitSlotsForTesting
internal static IReadOnlyCollection<(string OutfitReference, string CultureKey, string SexGenderPresentation, string SocialClassRole, string DisplayName, IReadOnlyDictionary<string, string> SlotItemStableReferences, IReadOnlyCollection<string> IntentionallySharedOrGenericSlots)> MedievalOutfitsForTesting
internal static IReadOnlyCollection<string> MedievalOutfitReferencedItemStableReferencesForTesting
```

Assert:

- 18 cultures.
- 12 outfits per culture.
- 216 outfits total.
- Each culture has `male` and `female` variants for:
  - `peasant`
  - `artisan`
  - `merchant`
  - `noble`
  - `religious`
  - `military`

### 2. Outfit Slot Completeness Tests

Every outfit must include:

- `underlayer`
- `lower_body`
- `leg_or_sock_layer`
- `footwear`
- `bodywear`
- `outerwear` or documented exception
- `headwear`
- `belt_or_sash`
- `worn_container`
- `fastener_or_jewellery`
- `role_item` where required

Every slot item must resolve to a seeded stable reference unless the slot is marked as intentionally shared and points to a common stable reference.

MED-OUTFIT-001 treats the current generated status-role wardrobe as craftable shared scaffold content. Tests should require those references to resolve to seeded item specs, while later explicit outfit-item goals can tighten the culture-specific thresholds.

### 3. Culture Identity Threshold Tests

Every outfit must include at least four culture-specific or culture-cluster-specific items.

Examples of culture-specific stable-reference prefixes:

```text
medieval_clothing_norse_
medieval_jewellery_norse_
medieval_clothing_song_china_
medieval_household_song_china_
```

Examples of culture-cluster prefixes:

```text
medieval_western_
medieval_islamic_
medieval_steppe_
medieval_rus_steppe_
```

Generic `medieval_common_` items do not count toward this threshold.

### 4. Sex Differentiation Tests

For each culture/class pair:

- male and female outfits must differ in at least two slots
- exceptions must be documented as intentionally unisex

Differences can include:

- main bodywear
- lower-body layer
- headwear
- fastener/jewellery
- outerwear
- role item

### 5. Class Differentiation Tests

Within each culture/sex:

- peasant, artisan, merchant, noble, religious, and military outfits must not collapse into the same set.
- each class must differ from the nearest lower class in at least two slots.
- noble and merchant outfits must include higher-status materials, trim, or accessories.
- religious outfits must include a devotional, book, robe, habit, veil, or role marker.
- military outfits must include arming clothing or military role accessories.

### 6. Vocabulary Tests

Each culture should have required vocabulary tokens in item short descriptions, full descriptions, final craft names, or outfit slot names.

Examples:

```csharp
["norse"] = ["hangerok", "oval brooch", "sea cloak", "runic", "leg wraps"];
["song_china"] = ["cross-collar", "scholar", "tea", "paper register", "official cap"];
["andalusi"] = ["qamis", "sirwal", "burnous", "turban", "tiraz"];
["rus_novgorod"] = ["rubakha", "kaftan", "birchbark", "fur-edged", "onuchi"];
```

### 7. Craft Name Quality Tests

Explicit culture final crafts must not use:

```text
regional pattern
regional medieval meal platter
regional drinking vessel
regional devotional token
regional record tablet
```

Generic baseline crafts may use neutral names only if the item is explicitly marked as generic baseline.

### 8. Food Input Sanity Tests

For food references containing terms like:

```text
bread
flatbread
bannock
pottage
stew
gruel
pilaf
noodle
feast
ration
stockfish
curd
cheese
tea
ale
wine
mead
kumis
```

the corresponding craft should include at least one food commodity/liquid input.

Forbidden pattern for prepared food:

```text
Furniture Panel Stock as the only substantial input
```

Allowed tableware exception:

```text
trenchers
platters
boards
bowls
cups
jugs
crocks
casks
skins
```

### 9. Writing Component Sanity Tests

For references containing:

```text
wax_tablet
wooden_tablet
record_tablet
birchbark
note_board
tally
```

the component list should not use `PaperSheet_Scroll` unless the item is actually paper/parchment scroll-like.

Use `InscribableSurface`-style components where available.

### 10. Exact Documentation Tests

Require:

- Every exact outfit reference appears in `Medieval_Outfit_Catalogue.md`.
- Every explicit culture item appears in `Medieval_Culture_Catalogue.md` or in generated catalogue data with tests.
- Broad patterns document only generic baseline families.
- Any deferred component-gap prop appears in docs with a reason.

### 11. Runtime Component Tests

Where components exist:

| Item Type | Required Component Check |
| --- | --- |
| sealed charters/packets/bales | `Sealable` |
| signets/guild stamps/official chops | `SealStamp` |
| balance scales/weights/measures | `MeasuringInstrument` |
| books/codices/registers | `Book` |
| sheets/charters/scrolls | `PaperSheet` or scroll-like surface |
| wax/wood/birchbark surfaces | `InscribableSurface` or documented limitation |

### 12. Production Chain Tests

Retain the existing upstream stock tests, but add final-product consumption checks:

- Mail items consume `Mail Panel Stock` or `Armour Ring Stock`.
- Lamellar items consume `Armour Lamella Stock`.
- Crossbow items consume `Crossbow Tiller Stock`, `Crossbow Prod Stock`, and `Crossbow Lockwork Stock`.
- Noble garments consume `Silk Brocade Panel`, `Embroidered Trim Stock`, or comparable luxury stock.
- Tablet-woven or banded garments consume `Tablet-Woven Band Stock`.
- Footwear consumes `Turnshoe Upper Stock` or documented equivalent.

## Acceptance Standard

A shallow generic matrix should fail the medieval content test suite even if it creates many item prototypes.

A successful implementation should let a test enumerate every culture, sex, and social class and produce a complete outfit with real item references from head to foot.
