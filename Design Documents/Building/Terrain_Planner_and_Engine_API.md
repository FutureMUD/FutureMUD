# Terrain Planner and Engine API

## Product boundary

FutureMUD Terrain Planner & Engine API 2.x is one hosted product under `TerrainPlanner/`. `TerrainPlanner.Server` is the ASP.NET Core host and read-only Engine API, `TerrainPlanner.Client` is the Interactive WebAssembly UI, `TerrainPlanner.Contracts` owns transport and map contracts, and `TerrainPlanner.Deployment` verifies signed releases. The old desktop planner, unfinished Blazor experiments, and standalone Terrain API are retired.

The service is deliberately read-only toward the game database. It never creates accounts, rooms, tags, or terrains and never uploads database backups. It is typically installed beside the Web MUD Client and relies on the same Caddy prerequisite, but it uses its own hostname and loopback Kestrel listener on port `5010`.

## Authentication and API

Authentication uses existing FutureMUD account rows and the legacy password representation without rewriting it. A session is issued only when an account is registered, has normal access status, and its authority group is `Admin` or higher—the same threshold as `cell new`. Suspension or authority demotion is checked against MySQL on every authenticated request.

The application cookie is HTTPS-only, HttpOnly, SameSite Strict, idle-expiring after 30 minutes, and absolutely expiring after eight hours. Login is limited by source address and normalized account name. Login and logout require antiforgery tokens. Data-protection keys must be persisted in the durable installer-owned data directory, never a versioned release directory.

Authenticated read-only endpoints are:

- `POST /api/v1/auth/login`
- `POST /api/v1/auth/logout`
- `GET /api/v1/auth/session`
- `GET /api/v1/terrains`
- `GET /api/v1/tags`

`GET /Terrain` remains an authenticated deprecated alias throughout planner 2.x. It advertises `/api/v1/terrains` as its successor and is removed in the next major planner release. Catalogue endpoints return DTOs only: terrain ID/name/editor colour/editor glyph, and tag ID/short name/full hierarchical name/parent ID. Queries are no-tracking. `/health/live` checks the process; `/health/ready` checks database connectivity.

## Planner model and workflow

The UI renders the visible viewport to one HTML canvas. Pointer input is batched before crossing into WebAssembly, so a 200 x 200 map does not create 40,000 Blazor components. The desktop-first workspace provides terrain and tag palettes, search, coordinate rulers, pan/zoom, paint, flood fill, rectangle, eyedropper, erase, undo/redo, cell inspection, layer/map clears, and keyboard shortcuts.

A cell has one terrain and zero or more tags. Painting terrain replaces the terrain; setting terrain `0` also clears tags. Tag painting is additive and tag erasing removes only the active tag. Tags use deterministic accessible marker colours with per-project overrides; the inspector and used-tag legend show hierarchical names. Every live tag remains paintable, including tags with duplicate short names or characters that were unsafe in the former name-based mask format.

Resize preserves the bottom-left overlap. Cropping removes north/east cells and requires confirmation. Canvas updates are versioned so queued input captured before a resize cannot modify the replacement grid, and cleared cells are removed from the canvas cache rather than drawn with a fallback colour. Projects autosave in browser local storage, namespaced by game origin and account ID, and can be imported/exported as schema-versioned JSON. They contain dimensions, terrain IDs, tag IDs plus display names, legend colours, unresolved historical tag IDs, and catalogue revision—not passwords, cookies, or tokens.

Catalogues refresh conditionally every five minutes after first load. A cached catalogue keeps local editing available during API/database outages. Open projects preserve their tag IDs and report live renames/deletions; catalogue refresh never silently changes an existing mask.

## Mask contract

Both masks contain exactly `width * height` comma-separated cells. Entry zero is the south-west `(0,0)` cell, entries proceed east across the southern row, and later rows proceed north.

- Terrain mask: one terrain ID per entry; `0` means no cell is created.
- Feature/tag mask: positive tag IDs separated by `|` inside a cell; an untagged cell is an empty entry.

The UI displays hierarchical tag names but exports numeric IDs. `Feature Rectangle` resolves each supplied ID to the exact framework tag before the room template is created, while still passing the tag names into descriptive feature rules. This keeps duplicate short names unambiguous and makes tag renames harmless to an existing mask. Name-based feature masks are rejected; import retains unknown positive IDs for review and round-tripping, but the engine rejects them until the tag exists again. Separate copy/import controls and the combined export panel preserve empty entries. Clipboard denial leaves selectable mask text available.

## Deployment and release

Packages are self-contained for `win-x64`, `linux-x64`, and `linux-arm64`. Each archive contains one versioned runtime directory with the server and client, deployment verifier, installers/updaters, Caddy fragment, configuration template, and deployment guide. Durable configuration and keys sit outside immutable `releases/<version>` directories; `current` selects the active release. Installers health-check activation, roll back on failure, and retain two prior releases.

The Windows installer creates durable configuration before creating a release, so its intentional first-run configuration stop is safely repeatable. It runs `FutureMUDTerrainPlanner` as the low-privilege built-in `LocalService` account, enables an unrestricted service SID, and grants that SID access only to the durable shared configuration/key directory. It discovers the existing Web MUD Client Caddy executable and active Caddyfile from the `FutureMUD Web Client HTTPS` scheduled task; explicit paths remain available as an operator override. The installer validates and reloads Caddy with the Caddyfile adapter and preserves a direct pre-existing planner site block rather than adding a duplicate one.

The release product ID is `terrainplanner` and the stable tag is `terrainplanner-vX.Y.Z`. `terrainapi` is retired and cannot be newly published, but its last historical download remains visible. The production workflow creates SHA-256 checksums and an Ed25519-signed update manifest for all three runtime archives.
