# FutureMUD Industrial ItemSeeder Master Era Design Reference

## Contract

`industrial` is the canonical ItemSeeder key; `revolution` remains an accepted compatibility alias and the internal VehicleSeeder token. The era tag is `Era / Industrial Era`, the manifest module is `industrial`, and era-specific stable references begin `industrial_`.

Current code makes this era selectable; Modern, Nuclear and Information remain inactive. That flag does not certify production readiness: the [clothing, footwear and uniforms specification](./FutureMUD_Industrialised_Clothing_Footwear_Uniforms_Design_Reference.md) identifies draft content and outstanding acceptance gates. Earlier claims that all activation gates were satisfied were premature. This documentation change does not modify activation.

The [approved clothing Wave 1 scope](./Industrialised_Clothing_Wave1_Evidence_and_Coverage.md) replaces the former 70 Industrial-only clothing quota with 20 planned Industrial-only bases, accepted in the user's scope/count approval on 2026-09-03 (Gate 1 passed). With other domains unchanged, the approved planning Industrial-only total is 600. An Industrial selection would additionally admit 112 of the new shared clothing bases and 113 reused earlier identities; these are not extra Industrial-only prototypes. Clothing infrastructure and prerequisite resolution passed Gate 2 on 2026-09-04. Its 84 proposed clothing ensembles still require production authoring and live wearability proof at Gates 3–7. Conventional colours are overridable outfit defaults, with no approved fixed locks.

## Scope

The draft Industrial delta contains 650 ordinary rows on top of 5,800 shared rows and compatible durable pre-industrial stock. These are current source counts, not accepted production quotas. Its intended coverage is mechanised production and extraction, steam infrastructure, rail and telegraph society, manufactured household goods, early mass retail, sanitation, civic institutions and the beginning of practical electrical systems.

Clothing's former 70 Industrial and 600 shared base allocations are superseded by coverage-based acceptance. With other domains unchanged, approved totals become `580 + C_industrial` and `5,200 + C_shared`, with skins and reused bases counted separately. Follow the clothing reference's inventory-first waves, production-route/value distinctions, variable-colour default, authored prose and complete outfit requirements. Plan all later bands without admitting later-only content to Industrial.

Priority domains are:

- factories, machine shops, mines, mills, foundries and occupational safety;
- steam plant support, solid-fuel handling, pressure-service fittings and maintenance tools;
- rail stations, coupled rolling-stock support, freight handling and bicycles;
- telegraphy, early telephony, printing, photography and office administration;
- municipal water, gas lighting, fire response, hospitals, schools and policing;
- canned and packaged goods, department retail, domestic sewing and manufactured furnishings;
- era-specific weapons support only where the runtime component graph is already meaningful.

## Boundaries

Hand tools, simple containers, furniture and textiles unchanged from earlier periods reuse existing stock. Fully motorised consumer society belongs to Modern. Do not imply automated production lines, broadcast electronics or digital controls through inert descriptions.

The vehicle milestone contains 40 `vehicle_revolution_*` canonical graphs: bicycles, road and service vehicles, route-bound tram and rail stock, agricultural and industrial vehicles, and supported surface-water vessels. Rail vehicles are independently route-bound because the current subsystem does not model coupled consists or steam-drive simulation. Aircraft, submarines and free-coordinate movement are excluded.

## Implementation stages

1. Close shared and Industrial dependency ledger entries in their owning seeders.
2. Approve domain coverage and reconciled allocations; gather usable labour-relative price evidence rather than relying on generic anchors.
3. Implement the shared loader and shared rows before delta rows.
4. Add Industrial lifecycle, craft and outfit relationships where they create play rather than decorative graph size.
5. Generate admission and executable manifests, then test fresh, repeat, update and customised records.
6. Make `industrial` selectable only in the activation change, never in advance.

## Activation gate

Activation requires populated shared and Industrial source files, no unresolved prerequisites, stable-reference uniqueness, reviewed supported descriptions, meaningful crafts/lifecycle links, complete wearable outfits, manifest/replay success, full DatabaseSeeder and relevant runtime tests, fresh and populated database replay, generated export parity and representative live-MUD inspection. Publish actual accepted counts and the approved reconciliation of the original targets. All seven clothing gates are necessary but not sufficient for whole-Stage-2 acceptance.

The earlier 650 delta rows, 5,800 shared rows, 2,337 craft products, 1,290 lifecycle participants, 100 outfits and 40 vehicles describe the draft implementation. They do not close the revised content gates. Earlier replay/presence-check history is retained in the programme; the replacement catalogue needs new tests and live evidence. If a database or live-MUD fixture is unavailable, record its gate as outstanding rather than passed. No missing fixture or successful generic preflight authorises activation.
