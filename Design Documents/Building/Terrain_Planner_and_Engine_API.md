# Terrain Planner and Engine API

Planner project schema 2 stores signed Z levels, each with its own `cells` collection, and vertical connection overrides. Schema 1 projects load as Z=0 and save as schema 2, preserving terrain, tag IDs/names, unresolved features and colours. The CLR cell collection remains `Rooms`.

## Product boundary

FutureMUD Terrain Planner & Engine API 2.x is one hosted product under `TerrainPlanner/`. `TerrainPlanner.Server` is the ASP.NET Core host and read-only Engine API, `TerrainPlanner.Client` is the Interactive WebAssembly UI, `TerrainPlanner.Contracts` owns transport and map contracts, and `TerrainPlanner.Deployment` verifies signed releases. The old desktop planner, unfinished Blazor experiments, and standalone Terrain API are retired.

The service is deliberately read-only toward the game database. It never creates accounts, rooms, tags, or terrains and never uploads database backups. It is typically installed beside the Web MUD Client and relies on the same Caddy prerequisite, but it uses its own hostname and loopback Kestrel listener on port `5010`.

## Authentication and API

Authentication uses existing FutureMUD account rows and the legacy password representation without rewriting it. A session is issued only when an account is registered, has normal access status, and its authority group is `Admin` or higher—the same threshold as `room new`. Suspension or authority demotion is checked against MySQL on every authenticated request.

The application cookie is HTTPS-only, HttpOnly, SameSite Strict, idle-expiring after 30 minutes, and absolutely expiring after eight hours. Login is limited by source address and normalized account name. Login and logout require antiforgery tokens. Data-protection keys must be persisted in the durable installer-owned data directory, never a versioned release directory.

Authenticated read-only endpoints are:

- `POST /api/v1/auth/login`
- `POST /api/v1/auth/logout`
- `GET /api/v1/auth/session`
- `GET /api/v1/terrains`
- `GET /api/v1/tags`

`GET /Terrain` remains an authenticated deprecated alias throughout planner 2.x. It advertises `/api/v1/terrains` as its successor and is removed in the next major planner release. Catalogue endpoints return DTOs only: terrain ID/name/editor colour/editor glyph, and tag ID/short name/full hierarchical name/parent ID. Queries are no-tracking. `/health/live` checks the process; `/health/ready` checks database connectivity.

## Planner model and workflow

The UI renders the visible viewport to one HTML canvas. Pointer input is batched before crossing into WebAssembly, so a 200 x 200 map does not create 40,000 Blazor components. The desktop-first workspace provides terrain and tag palettes, search, coordinate rulers, pan/zoom, paint, flood fill, rectangle, eyedropper, erase, undo/redo, room inspection, layer/map clears, and keyboard shortcuts.

A room has one terrain and zero or more tags. Painting terrain replaces the terrain; setting terrain `0` also clears tags. Tag painting is additive and tag erasing removes only the active tag. Tags use deterministic accessible marker colours with per-project overrides; the inspector and used-tag legend show hierarchical names. Every live tag remains paintable, including tags with duplicate short names or characters that were unsafe in the former name-based mask format.

Resize preserves the bottom-left overlap. Cropping removes north/east rooms and requires confirmation. Canvas updates are versioned so queued input captured before a resize cannot modify the replacement grid, and cleared rooms are removed from the canvas cache rather than drawn with a fallback colour. Projects autosave in browser local storage, namespaced by game origin and account ID, and can be imported/exported as schema-versioned JSON. They contain dimensions, terrain IDs, tag IDs plus display names, legend colours, unresolved historical tag IDs, and catalogue revision—not passwords, cookies, or tokens.

Catalogues refresh conditionally every five minutes after first load. A cached catalogue keeps local editing available during API/database outages. Open projects preserve their tag IDs and report live renames/deletions; catalogue refresh never silently changes an existing mask.

## Mask contract

### Stacked plans

All Z levels share XY dimensions. Add empty levels, duplicate terrain/tags into another level, delete levels or renumber them using the Z controls. Negative levels are supported. Painting, filling, inspection and clears affect the selected level; resizing changes all levels and preserves their southwest overlap. Undo/redo covers painting, levels, resizing and vertical settings, with a 100-operation / 32 MiB snapshot budget. Structural changes discard overrides involving endpoints that no longer form adjacent occupied pairs.

Matching occupied XY cells on Z and Z+1 have ordinary up/down exits by default. The selected-cell inspector edits the shared connection from either endpoint: suppress it, require climbing with a difficulty, and/or enable falling toward the lower room. A fall connection without climbing requires flight to travel upward. Holes and missing Z levels are never bridged. Canvas indicators show up/down connections and mark suppressed links with a cross. Level changes invalidate queued canvas events.

Ordinary up/down exits use the terrain layers shared by their endpoints, respecting blocked layers, so ground-level stairs remain available on terrains that also support rooftops or flight. Climb/fall/fly exits retain the highest-layer up / lowest-layer down boundary rules.

Complete-stack export targets only the new `terrain stack` area type. It includes terrain/tag IDs and vertical settings, with no names or UI metadata. Missing catalogue IDs or unresolved historical features block game export but remain preserved in project JSON. Legacy masks below still export/import only the selected Z level.

The shared codec source is owned by `FutureMUDLibrary` and source-linked into planner contracts under a planner namespace, avoiding runtime/database dependencies in WebAssembly and duplicate public CLR type identities in the server. The wire document uses camel-case JSON records with version, width, height, levels, occupied rooms (X/Y/Z, terrain, tags) and vertical overrides (X/Y/lower Z, suppressed, climb, difficulty, fall). `FMS1:` prefixes unpadded Base64url containing a 32-byte SHA-256 digest of the JSON followed by its Brotli-compressed bytes. The digest detects paste damage; it is not authentication. Decoding strips whitespace only and bounds decompression before deserialization. The planner uses the managed BrotliSharpLib encoder at quality 4 because native .NET Brotli compression is unavailable in WebAssembly; the dependency-free shared codec accepts this encoder as a callback, while the engine uses native .NET Brotli decoding. This keeps export available offline without adding an API endpoint.

Use **Copy token** for exports below 8,000 characters, or **Copy editor paste** for wrapped 1,000-character lines. Start `room new <area> [prog=<prog>] <room-template> editor`, paste the wrapped text, and submit `@` on its own line; `*cancel` abandons the import. Do not paste a large token as one line: the connection limit remains 40,960 bytes. Clipboard denial leaves selectable export text available.

Both sides enforce dimensions 1–200, at most 32 levels, 200,000 grid positions, 40,000 occupied rooms, 40,000 connection overrides, 16 MiB decompressed JSON and 4 MiB encoded input (including editor whitespace). The engine validates the entire payload and current terrain/tag IDs before creating rooms. Invalid input and editor cancellation create no rooms. These are separate game rooms connected by exits, not the engine's within-room `RoomLayer` elevations. Planner Z does not assign persistent world XYZ coordinates.

### Existing rectangle masks

Both masks contain exactly `width * height` comma-separated rooms. Entry zero is the south-west `(0,0)` room, entries proceed east across the southern row, and later rows proceed north.

- Terrain mask: one terrain ID per entry; `0` means no room is created.
- Feature/tag mask: positive tag IDs separated by `|` inside a room; an untagged room is an empty entry.

The UI displays hierarchical tag names but exports numeric IDs. `Feature Rectangle` resolves each supplied ID to the exact framework tag before the room template is created, while still passing the tag names into descriptive feature rules. This keeps duplicate short names unambiguous and makes tag renames harmless to an existing mask. Name-based feature masks are rejected; import retains unknown positive IDs for review and round-tripping, but the engine rejects them until the tag exists again. Separate copy/import controls and the combined export panel preserve empty entries. Clipboard denial leaves selectable mask text available.

## Deployment and release

Packages are self-contained for `win-x64`, `linux-x64`, and `linux-arm64`. Each archive contains one versioned runtime directory with the server and client, deployment verifier, installers/updaters, Caddy fragment, configuration template, and deployment guide. Durable configuration and keys sit outside immutable `releases/<version>` directories; `current` selects the active release. Installers health-check activation, roll back on failure, and retain two prior releases.

The Windows installer creates durable configuration before creating a release, so its intentional first-run configuration stop is safely repeatable. It runs `FutureMUDTerrainPlanner` as the low-privilege built-in `LocalService` account, enables an unrestricted service SID, and grants that SID access only to the durable shared configuration/key directory. It discovers the existing Web MUD Client Caddy executable and active Caddyfile from the `FutureMUD Web Client HTTPS` scheduled task; explicit paths remain available as an operator override. The installer validates and reloads Caddy with the Caddyfile adapter and preserves a direct pre-existing planner site block rather than adding a duplicate one.

The release product ID is `terrainplanner` and the stable tag is `terrainplanner-vX.Y.Z`. `terrainapi` is retired and cannot be newly published, but its last historical download remains visible. The production workflow creates SHA-256 checksums and an Ed25519-signed update manifest for all three runtime archives.
