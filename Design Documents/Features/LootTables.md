# Loot Tables

Loot tables are revisioned, builder-authored definitions for atomically creating item and commodity graphs. They are generic engine content: a caller supplies an exact table revision, variant, destination and optional deterministic seed. Tables do not own depletion, one-shot state, Region bindings or gameplay conditions.

## Definition

Each revision stores a canonical XML definition and SHA-256 hash. A definition contains named variants; each variant contains explicitly ordered roll groups; and each group selects one positively weighted choice per repetition. Groups target either the invocation target or a stable item key produced exactly once by an earlier group.

Choices create an exact item-prototype revision, create a commodity from an exact solid and optional tag, invoke an exact nested loot-table revision and variant, or explicitly create nothing. Item choices may author quantity, quality, characteristic values and a result key. Commodity choices author a mass range. Nesting is acyclic and a realised plan is limited to 1,000 leaves.

Deterministic decisions use algorithm version 1 (`sha256-path-v1`). Semantic decision paths include exact table revision, variant, group key, repetition, choice key and field. Integer selection uses rejection sampling. Canonical saves preserve explicit group and choice ordering and sort only unordered semantic collections such as variants and characteristic assignments.

## Builder surface

Use `loottable` (alias `lt`) for normal editable-revision lifecycle commands: `list`, `show`, `new`, `clone`, `edit`, `close`, `revise`, `submit` and `review`. Definition editing uses `set variant`, `set group` and `set choice`. `loottable validate` resolves every exact reference and destination constraint. `loottable preview` produces the complete realised plan and digest without creating objects. `loottable load` is the administrator test path and accepts `here`, `into <item>` or `to <character>`.

## FutureProg

`loadloottable` returns Text and has seeded and unseeded overloads for Location, Item and Character destinations:

```
loadloottable(Number tableId, Number revision, Location target, Text variant) -> Text
loadloottable(Number tableId, Number revision, Location target, Text variant, Number seed) -> Text
loadloottable(Number tableId, Number revision, Item target, Text variant) -> Text
loadloottable(Number tableId, Number revision, Item target, Text variant, Number seed) -> Text
loadloottable(Number tableId, Number revision, Character target, Text variant) -> Text
loadloottable(Number tableId, Number revision, Character target, Text variant, Number seed) -> Text
```

Success returns a canonical `OK` receipt with exact revision/hash, algorithm, variant, actual seed, root item IDs, created count and plan digest. Failure returns a stable `ERROR code=...` receipt. Creation is staged, all destinations are preflighted, merging is disabled, and any creation, placement, event or persistence failure deletes every staged object and leaves no residue.

## Persistence

Migration `AddLootTables` adds one `LootTables` table keyed by `(Id, RevisionNumber)` and linked to `EditableItems`. It stores name, algorithm version and canonical definition payload. No existing content is migrated automatically.
