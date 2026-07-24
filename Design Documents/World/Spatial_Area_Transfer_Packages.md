# Spatial Area Transfer Packages

## Purpose

Spatial area transfer packages let a senior administrator export a self-contained ordinary zone from a running FutureMUD installation and recreate it in another installation. The workflow is intentionally conservative:

- the package is human-readable, versioned JSON;
- every imported database identity is newly allocated;
- references inside the package use deterministic local keys rather than source database IDs;
- installation-owned dependencies resolve by exact name during preflight;
- imports create a new zone and never merge with or overwrite existing spatial content;
- unsupported state blocks export when silently dropping it would make the result misleading.

The version 1 file suffix is `.fmsa.json`. Files are read and written only beneath the server's `Spatial Packages` directory. Package names cannot contain a path.

## Builder Workflow

The command requires `SeniorAdmin` permission.

On the source installation:

```text
spatialpackage export zone "Harbour Ward" harbour-ward
```

Copy `Spatial Packages/harbour-ward.fmsa.json` to the corresponding directory on the target installation.

On the target installation:

```text
cell package new "Harbour Import"
spatialpackage validate harbour-ward "Prime Material" "Imported Harbour Ward"
spatialpackage import harbour-ward "Prime Material" confirm "Imported Harbour Ward"
```

The target overlay package must be `Under Design`. The target shard must already exist because shards own installation-level clocks, calendars, celestial objects, and sky configuration. The optional final argument overrides the imported zone name.

`validate` is read-only. `import` repeats the complete preflight and also requires the literal `confirm` keyword. The import uses a serializable database transaction and rechecks zone-name uniqueness inside that transaction.

## Version 1 Payload

The top-level object contains:

| Field | Purpose |
| --- | --- |
| `format` | Constant `futuremud-spatial-area`. |
| `version` | Schema version. Version 1 is currently accepted. |
| `integritySha256` | SHA-256 of the canonical payload with this field empty. |
| `createdUtc` | Export timestamp. |
| `source` | Diagnostic source IDs and names for the zone, shard, and active overlay package. Source IDs are never reused on import. |
| `zone` | Zone name, geography, ambient light, weather/forage dependencies, default-cell key, and clock/timezone aliases. |
| `rooms` | Deterministic room keys, source IDs for diagnostics, and integer coordinates. |
| `cells` | Deterministic cell keys, parent-room keys, active overlay data, explicit forage override, tags, local covers, and magic-resource amounts. |
| `exits` | Internal exit endpoints and both directional sides, door capability, size limits, climb/fall state, travel multiplier, and blocked layers. |

Each export assigns keys in stable source-ID order:

```text
room-00001
cell-00001
exit-00001
```

References within the package use only these keys. Source database IDs remain diagnostic provenance and have no import semantics.

## Overlay Behaviour

Version 1 exports the currently active overlay for every cell, including:

- cell name and description;
- terrain;
- hearing profile;
- outdoors type;
- atmosphere kind and name;
- ambient and added light;
- safe-quit state;
- the exact internal exits active in that overlay.

A zone may have cells whose active overlays came from different source overlay packages. Their active data is still exported independently. On import, all cells receive a new overlay in the builder's selected target package. Other historical or inactive source overlay revisions are not transferred.

## Dependency Resolution

The target installation supplies engine-owned dependencies. Preflight resolves them by exact, case-insensitive name:

- terrain;
- liquid or gas atmosphere, with the fluid kind checked;
- hearing profile;
- zone and explicit cell forage profiles;
- weather controller;
- cell tags;
- local ranged covers;
- magic resources.

Clock dependencies resolve by clock alias. A timezone resolves by alias or description on that clock. A source clock absent from the target shard is an error. A clock that exists only on the target shard is assigned its primary timezone with a warning.

Dependency source IDs are never used as a fallback. This prevents a coincidentally reused ID from linking the imported zone to the wrong target object.

## Integrity and Validation

Validation occurs before any database write and checks:

- file containment and the 16 MiB size limit;
- strict JSON with unknown fields rejected;
- package format, version, and SHA-256 integrity;
- room, cell, and exit count limits;
- unique local keys;
- room/cell/exit reference closure;
- overlay-to-exit endpoint consistency;
- valid default and fall cells;
- finite geographic, light, resource, and travel values;
- enum values known to the receiving engine;
- cardinal versus non-cardinal exit-side consistency;
- exact availability of every target dependency;
- target zone-name uniqueness;
- an open `Under Design` target overlay package.

The importer allocates rows in dependency order: zone and rooms, cells, overlays, exits, overlay-exit links, then the zone default cell. All IDs are database-generated. Existing zones, rooms, cells, overlays, and exits are untouched.

## Deliberate Version 1 Boundaries

Export fails when a zone contains state that cannot yet be represented faithfully:

- route cells, including length, landmarks, and exit anchors;
- room-scale hosted vehicle interiors;
- temporary cells;
- agriculture fields;
- persisted cell effects;
- persistent surface-liquid state;
- installed door items;
- fall exits whose destination is outside the exported zone.

The following data is not spatial content and is omitted with diagnostics:

- characters and game items;
- exits crossing the exported zone boundary;
- membership in cross-cutting `AREA` groups.
- cell event hooks, which are behavioural installation content rather than spatial topology.

Exit door capability and permitted door size are transferred, but an installed physical door item is not.

## Planned Extensions

A later schema version can add these independently without weakening version 1 validation:

1. Route-cell definitions, landmarks, and per-exit anchors.
2. Cross-zone `AREA` packages containing multiple zone fragments and area weather.
3. Optional boundary-link manifests that a builder can explicitly reconnect after import.
4. Installed door and selected static item packaging with their complete item-prototype dependency graphs.
5. Agriculture, surface-liquid, ranged-cover state beyond local profile references, and selected portable effects.
6. Additional overlay revisions and review history.

Older servers must reject later schema versions until they explicitly implement them.
