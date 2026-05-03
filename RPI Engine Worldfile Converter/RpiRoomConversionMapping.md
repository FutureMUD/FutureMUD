# RPI Room Conversion Mapping

## Scope

This pass imports one archived RPI Engine room as one FutureMUD `Room` plus one `Cell`, grouped into one FutureMUD `Zone` and one `CellOverlayPackage` per resolved RPI zone group.

It focuses on:

- room names
- room descriptions
- terrain mapping
- outdoors / safe-quit mapping
- exits
- directional hidden exits
- deterministic zone layout
- structured provenance for unsupported legacy sidecar data

It does not yet recreate:

- reset-driven initial room contents or door state
- room traps as runtime behavior
- room search mechanics
- room progs as generated FutureProgs
- door items
- deity semantics
- weather-sidecar rendering behavior

## Source And Provenance

The authoritative sources are:

- `Old SOI Code/`
- `soiregions-main/rooms.*`

The converter preserves:

- source filename
- source zone
- source vnum
- raw room flags
- raw sector type
- deity
- raw room-sidecar blocks such as `E`, `W`, `P`, `A`, `X`, and secret-search data

## Zone Grouping

Default behavior is one group per `rooms.<zone>` file / thousand-range zone.

Built-in merge rules must be source-backed. The current hardcoded merge is:

- zones `5` and `6` import as one group named `Minas Morgul`

That merge is justified by base-room evidence in the preserved corpus:

- `rooms.5` begins with `Base Room for Minas Morgul`
- `rooms.6` begins with `Baseroom for Morgul Humans`

Other zones are not auto-merged from memory. When no clean base-room name can be derived, the converter falls back to `Zone NN` and records a warning.

## Import Persistence

Room import creates one FutureMUD `CellOverlayPackage` per converted zone group.

`CellOverlayPackage.Id` is a revision-group identifier rather than a database-generated key, so the importer assigns new package ids from the current maximum package id plus one, matching the runtime builder workflow.

Execute-mode imports run inside a database transaction. Intermediate `SaveChanges` calls are still used to obtain generated room, cell, overlay, and exit ids for dependent rows, but an exception rolls the whole execute import back rather than leaving a partial room import behind.

FutureMUD currently stores `CellOverlay.CellDescription` in a `varchar(4000)` column. Converted rooms keep the full effective description for export and audit, but descriptions longer than that limit produce a `cell-description-truncated` warning and are truncated to 4,000 characters only when `apply-rooms --execute` writes the overlay row.

FutureMUD stores exit keywords, primary keywords, verbs, inbound/outbound descriptions, and inbound/outbound targets in `varchar(255)` columns. Converted exit-side text is preserved at full length for export and audit, but overlong values produce `exit-description-truncated`, `exit-keywords-truncated`, or `exit-primary-keyword-truncated` warnings and are truncated only when the importer writes the `Exit` row. Long exit descriptions are especially suspicious because the current pass maps the legacy exit description into all four FutureMUD inbound/outbound description and target fields for that side.

## Terrain Mapping

Sector mapping defaults:

- `INSIDE` -> `Hall`
- `CITY` -> `Urban Street`
- `ROAD` -> `Cobblestone Road` when stone/causeway cues exist, otherwise `Compacted Dirt Road`
- `TRAIL` -> `Trail`
- `FIELD` -> `Field`
- `WOODS` -> `Broadleaf Forest`
- `FOREST` -> `Temperate Coniferous Forest` when conifer cues exist, otherwise `Broadleaf Forest`
- `HILLS` -> `Hills`
- `MOUNTAIN` -> `Mountainside`
- `SWAMP` -> `Temperate Freshwater Swamp` unless tree / swamp-forest cues exist
- `DOCK` -> `Riverbank`, upgraded to `Lake Shore` or `Ocean Surf` from lexical or neighbour cues
- `CROWSNEST` -> `Rooftop`
- `PASTURE` -> `Pasture`
- `HEATH` -> `Heath`
- `PIT` -> `Dungeon`, upgraded to `Cave` from cave/cavern/tunnel cues
- `LEANTO` -> `Cave Entrance`
- `LAKE` -> `Lake`
- `RIVER` -> `River`
- `OCEAN` -> `Ocean`
- `REEF` -> `Reef`
- `UNDERWATER` -> `Deep River`, `Deep Lake`, `Deep Ocean`, or `Underground Water` from lexical and neighbour cues, with `Deep Lake` as the fallback

## Outdoors Type Mapping

Defaults come from the mapped terrain:

- enclosed interiors such as `Hall`, `Dungeon`, and `Cave` map to `Indoors`
- open built spaces such as `Rooftop` and `Cave Entrance` map to `IndoorsClimateExposed`
- other terrains map to `Outdoors`

Room-flag overrides:

- `INDOORS` forces an indoor mode
- `INDOORS` plus open-structure cues like `rooftop`, `gatehouse`, or `battlement` maps to `IndoorsClimateExposed`
- `SAFE_Q` sets `SafeQuit = true`

Other legacy room flags such as `DARK`, `LIGHT`, `NO_MOB`, `NOHIDE`, `FORT`, `TEMPORARY`, `OPEN`, `ROCKY`, and `VEGETATED` are preserved in metadata only in this pass.

## Xerox Rooms

`C` / xerox rooms are resolved during conversion, not at runtime.

The imported room keeps:

- its own vnum
- its own exits
- its own provenance

But it uses the xerox target for:

- effective description
- weather sidecar text

If the xerox target is missing, the room keeps its original description and records a warning.

## Exit Mapping

Each converted RPI exit becomes one shared FutureMUD `Exit`.

Side-specific data is preserved per direction:

- keyword text -> `PrimaryKeyword` and `Keywords`
- exit description text -> directional description fields
- hidden/trapped flags remain directional

One-sided exits are allowed. The shared `Exit` is created, and only the visible overlay side is linked.

## Doors And Gates

RPI Engine doors and gates were not standalone items. In this pass:

- `EX_ISDOOR` and `EX_ISGATE` map to `AcceptsDoor = true`
- no door items are created
- key ids, pick penalties, and pickproof flags are preserved as provenance only
- reset-driven closed/locked initial state is not recreated because `resets.*` are not in the preserved corpus

Door-size heuristics:

- `trapdoor` / `hatch` -> `Small`
- plain `door` -> `Large`
- `double doors` / `carriage doors` -> `VeryLarge`
- `gate` / `portcullis` -> `Huge`
- `great gate`, `massive gate`, `fortress gate` -> `Enormous`

Unless crawl / low-opening cues exist, both `MaximumSizeToEnter` and `MaximumSizeToEnterUpright` follow the inferred door size.

## Hidden, Trapped, Climb, And Fall Exits

Hidden exits:

- `H` and `B` still create normal exits
- the hidden side gets an `ExitHidden` effect on that cell only
- reverse visibility is not inferred automatically
- paired `Q` search difficulty and text are exported as metadata, but no search runtime is created yet

Trapped exits:

- `T` and `B` still create usable exits
- trap presence is preserved in metadata and warnings only

Vertical movement:

- `CLIMB` on a vertical link maps to a climb exit
- `FALL` on a room with an open down exit maps to a directional fall exit using FutureMUD's `FallCell` behavior

## Sidecar Data

The following blocks are parsed and preserved for audit / later passes:

- `E` extra descriptions
- `W` written descriptions
- `P` room programs
- `A` weather descriptions and alas text
- `X` room capacity
- deity values

Room progs are exported as structured raw metadata with command, keys, and program text. They are not translated into FutureProgs in this pass.

## Layout

Coordinates are generated deterministically per zone group:

- seed room is the base room when available, otherwise the lowest vnum
- exits are walked in `N, E, S, W, U, D` order
- direction deltas are `N=(0,1,0)`, `E=(1,0,0)`, `S=(0,-1,0)`, `W=(-1,0,0)`, `U=(0,0,1)`, `D=(0,0,-1)`
- when an intended coordinate is occupied, the nearest free coordinate on the same `Z` is chosen and a warning is recorded

This produces stable overlay layouts without requiring hand-authored maps.
