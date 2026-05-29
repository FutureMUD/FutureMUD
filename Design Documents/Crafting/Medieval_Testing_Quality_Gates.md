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

## Required Test Categories

### 1. Explicit Culture Catalogue Counts

Expose a testing accessor that distinguishes explicit culture catalogue items from generic baseline items.

Suggested accessor:

```csharp
internal static IReadOnlyDictionary<string, IReadOnlyCollection<string>> MedievalExplicitCultureStableReferencesForTesting
```

Then assert minimums per culture:

| Surface | Minimum |
| --- | ---: |
| explicit clothing/accessory | 12 |
| explicit military/equipment | 8 |
| explicit food/beverage | 8 |
| explicit writing/admin | 6 |
| explicit household/devotional/luxury | 5 |

Generic baseline references should not count.

### 2. Vocabulary Tests

Each culture should have required vocabulary tokens in item short descriptions, full descriptions, and/or final craft names.

Examples:

```csharp
["norse"] = ["hangerok", "oval brooch", "sea cloak", "runic", "stockfish"];
["song_china"] = ["cross-collar", "scholar", "tea", "paper register", "official chop"];
["andalusi"] = ["qamis", "sirwal", "burnous", "turban", "glazed"];
["rus_novgorod"] = ["rubakha", "kaftan", "birchbark", "fur-edged", "river"];
```

### 3. Craft Name Quality Tests

Explicit culture final crafts must not use:

```text
regional pattern
regional medieval meal platter
regional drinking vessel
regional devotional token
regional record tablet
```

Generic baseline crafts may use neutral names only if the item is explicitly marked as generic baseline.

### 4. Food Input Sanity Tests

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

### 5. Writing Component Sanity Tests

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

### 6. Exact Documentation Tests

The second-pass exact catalogue should not be satisfied by broad pattern documentation.

Require:

- Every explicit culture stable reference appears by exact text in `Medieval_Culture_Catalogue.md`.
- Broad patterns document only generic baseline families.
- Any deferred component-gap prop appears in docs with a reason.

### 7. Runtime Component Tests

Where components exist:

| Item Type | Required Component Check |
| --- | --- |
| sealed charters/packets/bales | `Sealable` |
| signets/guild stamps/official chops | `SealStamp` |
| balance scales/weights/measures | `MeasuringInstrument` |
| books/codices/registers | `Book` |
| sheets/charters/scrolls | `PaperSheet` or scroll-like surface |
| wax/wood/birchbark surfaces | `InscribableSurface` or documented limitation |

### 8. Production Chain Tests

Retain the existing upstream stock tests, but add final-product consumption checks:

- Mail items consume `Mail Panel Stock` or `Armour Ring Stock`.
- Lamellar items consume `Armour Lamella Stock`.
- Crossbow items consume `Crossbow Tiller Stock`, `Crossbow Prod Stock`, and `Crossbow Lockwork Stock`.
- Noble garments consume `Silk Brocade Panel`, `Embroidered Trim Stock`, or comparable luxury stock.
- Tablet-woven or banded garments consume `Tablet-Woven Band Stock`.

## Acceptance Standard

A shallow generic matrix should fail the medieval content test suite even if it creates many item prototypes.
