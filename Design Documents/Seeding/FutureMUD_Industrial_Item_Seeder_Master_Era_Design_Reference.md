# FutureMUD Industrial ItemSeeder Master Era Design Reference

## Contract

`industrial` is the canonical ItemSeeder key; `revolution` remains an accepted compatibility alias and the internal VehicleSeeder token. The era tag is `Era / Industrial Era`, the manifest module is `industrial`, and era-specific stable references begin `industrial_`.

This era is not currently selectable. It becomes the first later era to activate only after the shared industrialised layer and Industrial delta satisfy the programme gates.

## Scope

The Industrial delta target is 650 ordinary prototypes on top of the 5,800 shared layer and compatible durable pre-industrial stock. It represents mechanised production and extraction, steam infrastructure, rail and telegraph society, manufactured household goods, early mass retail, sanitation, civic institutions and the beginning of practical electrical systems.

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

The vehicle milestone includes bicycles and coupled rail stock plus source-backed steam/early-motor road and water vehicles that the existing runtime can express. Aircraft and free-coordinate movement are excluded.

## Implementation stages

1. Close shared and Industrial dependency ledger entries in their owning seeders.
2. Establish domain TSV allocations and value-index anchors.
3. Implement the shared loader and shared rows before delta rows.
4. Add Industrial lifecycle, craft and outfit relationships where they create play rather than decorative graph size.
5. Generate admission and executable manifests, then test fresh, repeat, update and customised records.
6. Make `industrial` selectable only in the activation change, never in advance.

## Activation gate

Activation requires populated shared and Industrial source files, no unresolved prerequisites, stable-reference uniqueness, supported descriptions, manifest/replay success, full DatabaseSeeder tests, fresh and populated database replay, generated export parity and representative live-MUD inspection. The activation review must publish actual counts and explain any variance from the 650 target.
