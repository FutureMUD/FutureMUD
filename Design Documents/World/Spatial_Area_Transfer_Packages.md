# Spatial Area Transfer Packages

## Purpose

Spatial area transfer packages let a senior administrator export one or more self-contained zones from a running FutureMUD installation and recreate them in another installation. The workflow is intentionally conservative:

- the package is human-readable, versioned JSON;
- every imported database identity is newly allocated;
- references inside the package use deterministic local keys rather than source database IDs;
- installation-owned dependencies resolve by exact name during preflight;
- imports create new zones and never merge with or overwrite existing spatial content;
- unsupported state blocks export when silently dropping it would make the result misleading;
- every deliberate omission is retained in the package and shown in-game.

The file suffix is `.fmsa.json`. Files are read and written only beneath the server's `Spatial Packages` directory. Package names cannot contain a path. Versions 1 through 3 are accepted.

## Builder Workflow

The command requires `SeniorAdmin` permission.

On the source installation:

```text
spatialpackage export zone "Harbour Ward" harbour-ward
spatialpackage export zones harbour-district "Harbour Ward" "Warehouse Quarter"
```

The multi-zone form takes the package name first, followed by one or more quoted zone names or IDs. It preserves exits between every selected zone. Copy the generated `.fmsa.json` file into the target installation's corresponding directory.

On the target installation:

```text
cell package new "Harbour Import"
spatialpackage validate harbour-ward "Prime Material" "Imported Harbour Ward"
spatialpackage import harbour-ward "Prime Material" confirm "Imported Harbour Ward"
```

The target overlay package must be `Under Design`. The target shard must already exist because shards own installation-level clocks, calendars, celestial objects, and sky configuration. The optional final argument overrides the imported name only for a single-zone package; multi-zone packages retain every packaged name.

`validate` is read-only. `import` repeats the complete preflight and also requires the literal `confirm` keyword. All selected zones and their cross-zone links use one serializable transaction, with every zone name rechecked inside the transaction.

## Version 3 Payload

| Field | Purpose |
| --- | --- |
| `format` | Constant `futuremud-spatial-area`. |
| `version` | Schema version. Versions 1 through 3 are accepted. |
| `integritySha256` | SHA-256 of the canonical payload with this field empty. |
| `createdUtc` | Export timestamp. |
| `source` / `sourceZones` | Diagnostic source IDs and names for each zone, shard, and active overlay package. The singular field preserves version-1 context. Source IDs are never reused. |
| `zone` / `zones` | Zone-local keys, names, geography, ambient light, weather/forage dependencies, default cells, and clock/timezone aliases. |
| `rooms` | Deterministic room keys, diagnostic source IDs, owning zone keys, and integer coordinates. |
| `cells` | Deterministic cell keys, parent rooms, active overlay data, route-cell data, explicit forage override, tags, local covers, and magic-resource amounts. |
| `exits` | Every exit whose endpoints are both selected, including cross-zone links, with directional sides, door capability, size limits, climb/fall state, travel multiplier, and blocked layers. |
| `areas` | Fully-contained `AREA` groups, their room membership, and optional weather-controller reference. |
| `omissions` | Structured codes and exact builder-facing descriptions of content deliberately excluded. |

Each export assigns keys in stable source-ID order:

```text
zone-00001
room-00001
cell-00001
exit-00001
```

References within the package use only these keys. Source database IDs remain diagnostic provenance and have no import semantics.

Version 1 remains checksum-compatible and is upgraded logically while reading. A version-1 room record with no packaged cell is treated as stale source metadata: validation warns with `empty-room-skipped`, and import does not create it. Version-2 exports omit such a record and include a corresponding omission.

## Overlay and Route-Cell Behaviour

The currently active overlay is exported for every cell, including:

- cell name and description;
- terrain;
- hearing profile;
- outdoors type;
- atmosphere kind and name;
- ambient and added light;
- safe-quit state;
- the exact packaged exits active in that overlay.

Cells may use active overlays from different source packages. Their active data is exported independently and imported into the builder's selected target package. Historical and inactive overlay revisions are not transferred.

Version 2 also transfers route length, default coordinate, direction names, room-equivalent length, topology version, landmarks, and anchors for every packaged exit. An anchor for an omitted boundary exit is omitted together with that exit.

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

Clock dependencies resolve by clock alias. A timezone resolves by alias or description on that clock. A source clock absent from the target shard is an error. A clock that exists only on the target shard receives its primary timezone with a warning. Dependency source IDs are never a fallback, preventing a coincidentally reused ID from linking to the wrong target object.

## Integrity and Validation

Validation occurs before any database write and checks:

- file containment and the 16 MiB size limit;
- strict JSON with unknown fields rejected;
- package format, supported version, and SHA-256 integrity;
- zone, room, cell, and exit count limits;
- unique local keys;
- room-to-zone and cell/exit reference closure;
- overlay-to-exit endpoint consistency;
- valid per-zone default and fall cells;
- valid route geometry, landmarks, and exit anchors;
- finite geographic, light, resource, and travel values;
- enum values known to the receiving engine;
- cardinal versus non-cardinal exit-side consistency;
- exact availability of every target dependency;
- target name uniqueness for every packaged zone;
- an open `Under Design` target overlay package.

The importer allocates rows in dependency order: zones and non-empty rooms, cells, overlays and route geometry, exits, route anchors and overlay-exit links, then each zone's default cell. All IDs are database-generated. Existing spatial content is untouched.

## Deliberate Boundaries and Diagnostics

Export fails when selected content contains state that cannot be represented faithfully:

- hosted vehicle interiors;
- agriculture fields;
- persisted cell effects;
- persistent surface-liquid state;
- installed door items;
- fall exits whose destination is outside the selected zones.

The following content is omitted because it is not self-contained spatial topology:

- temporary cells (including dwelling interiors), their rooms, and their links;
- characters and game items;
- exits whose other endpoint is in an unselected zone;
- any `AREA` group that also contains a room outside the selected zones;
- cell event hooks.

Exit door capability and permitted door size are transferred, but an installed physical door item is not.

Export, validation, and import enumerate omissions in the in-game result. A boundary-exit entry identifies the direction or verb, both source cell IDs and names, and the unselected destination zone. This lets builders distinguish expected boundary loss from unexpectedly incomplete selections.

## Planned Extensions

Fully-contained `AREA` groups, including their weather-controller reference, are carried in version 3. Temporary cells (such as dwelling interiors) are deliberately skipped, together with their rooms and links.

1. Optional boundary-link manifests that a builder can explicitly reconnect after import.
2. Installed door and selected static item packaging with their complete item-prototype dependency graphs.
3. Agriculture, surface-liquid, ranged-cover state beyond local profile references, and selected portable effects.
4. Additional overlay revisions and review history.

Older servers must reject later schema versions until they explicitly implement them.
