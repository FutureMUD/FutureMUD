# Loot Tables

Loot tables let a builder describe a package of generated items and commodities in a form that another human can inspect. The engine plans the whole package first and then creates it atomically: either every result reaches its intended destination, or every staged result is removed.

A caller supplies an exact table revision, variant, destination and optional deterministic seed. Tables do not own depletion, one-shot state, Region bindings or gameplay conditions.

## Mental model

A LootTable is an ordered recipe:

1. A **variant** is a named version of the recipe, such as `default`, `damaged` or `premium`.
2. A variant runs its **groups** from top to bottom.
3. Each group repeats within its authored range and selects one weighted **choice** each time.
4. A choice creates an item, creates a commodity, invokes another exact LootTable revision, or deliberately creates nothing.
5. An item choice may provide a **local key**. Later groups can direct their products inside that particular generated container.

Table nesting and physical containment are different. Invoking a child LootTable means “run this other recipe here.” The invoking group's destination decides whether the child results go to the outer target or inside an earlier keyed container.

For example:

```text
Group 1: vessel -> create an envelope at the outer target; provide key "vessel"
Group 2: contents -> create 125g carbon steel inside "vessel"
Group 3: nested -> invoke the child table at the outer target

Outer target
├── envelope from Group 1
│   └── 125g carbon steel from Group 2
└── child table's root item from Group 3
```

Changing Group 3's destination from `target` to `vessel` would place the child table's root inside the first envelope instead.

## Definition

Each revision stores a canonical XML definition and SHA-256 hash. A definition contains named variants; each variant contains explicitly ordered roll groups; and each group selects one positively weighted choice per repetition. Groups target either the invocation target or a stable item key produced exactly once by an earlier group.

Choices create an exact item-prototype revision, create a commodity from an exact solid and optional tag, invoke an exact nested loot-table revision and variant, or explicitly create nothing. Item choices may author quantity, quality, characteristic values, initial open/lock state and a result key. Commodity choices author a mass range. Nesting is acyclic and a realised plan is limited to 1,000 leaves.

Deterministic decisions use algorithm version 1 (`sha256-path-v1`). Semantic decision paths include exact table revision, variant, group key, repetition, choice key and field. Integer selection uses rejection sampling. Canonical saves preserve explicit group and choice ordering and sort only unordered semantic collections such as variants and characteristic assignments.

## Builder workflow

Use `loottable` (alias `lt`) for normal editable-revision lifecycle commands: `list`, `show`, `new`, `clone`, `edit`, `close`, `revise`, `submit` and `review`.

A small container example can be authored as follows:

```text
loottable new "Example Parcel"
loottable set group add default vessel
loottable set choice add default vessel envelope 1 item 53 revision 0 quantity 1 1 quality 5 5 as vessel
loottable set group add default contents into vessel
loottable set choice add default contents steel 1 commodity 24 tag 264 mass 125g 125g
loottable set group add default child
loottable set choice add default child nested 1 table 1 0 default
loottable validate "Example Parcel"
loottable preview "Example Parcel" default 202
loottable edit
```

`loottable show` presents each variant as a table. Its columns mean:

| Column | Meaning |
|---|---|
| Group | Execution order and the builder's stable group name. |
| Repeat | How many selections this group makes. |
| Destination | `Outer target` or an earlier local item key. |
| Choice | The stable semantic key used by deterministic planning. |
| Weight / Chance | Relative selection weight and its displayed probability within this group. |
| Result | The resolved item, commodity or exact nested-table reference and its authored ranges. |

Commodity mass accepts explicit units such as `125g` and `1.5kg`. Bare numeric values remain accepted as engine base mass units for compatibility, but new definitions should use explicit units.

Items start in their prototype's normal open and unlocked state. Add `closed` when a generated openable item must begin closed, or `locked` when a generated item with a built-in lock must begin closed and locked. These states are applied only after all planned contents have been inserted, so the full package remains atomic.

`loottable validate` resolves every exact reference, confirms local destinations, rejects nesting cycles and enforces the expansion limit. `loottable preview` produces the complete realised plan and digest without creating anything. `loottable load` is the administrator test path and accepts `here`, `into <item>` or `to <character>`.

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
