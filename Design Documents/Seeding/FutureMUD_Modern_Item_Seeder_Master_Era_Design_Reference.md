# FutureMUD Modern ItemSeeder Master Era Design Reference

## Contract

`modern` is both the canonical ItemSeeder and VehicleSeeder token. The era tag is `Era / Modern Era`, the manifest module is `modern`, and era-specific stable references begin `modern_`.

Modern work follows the completed shared-plus-Industrial milestone. The era remains non-selectable until its own activation gate passes.

## Scope

The Modern delta target is 700 ordinary prototypes. It describes mass electrification and motorisation, mature telephone and broadcast systems, consumer appliances, standardised offices and retail, plastics and synthetic goods, modern medicine and safety, logistics and service-station infrastructure.

Priority domains are:

- domestic electrical appliances, laundry, refrigeration, cleaning and climate control;
- motor-vehicle service, road safety, filling/repair stations and mass transit support;
- retail checkout, vending, warehousing, packaging and commercial food service;
- typewriting, photocopying, telephone exchanges, radio, film and recorded media;
- hospitals, laboratories, emergency response, occupational and public safety;
- power tools, standardised fasteners, compressors and portable workshop equipment;
- schools, offices, hotels, recreation and mass-produced personal goods.

## Technology profile use

Modern is the first era where all five technology-profile dimensions routinely affect stock. A profile selects compatible component families for power, paper, telecommunications, network/media and vehicle service. It must not cause brand-like duplicates or turn common non-interface goods into regional variants.

## Boundaries

Earlier mechanical forms continue through shared or prior-era stable references. Integrated circuits, personal computing and packet networks primarily belong to Nuclear or Information depending on form. A decorative powered object uses `PoweredProp` only when its presentation is truthful; working tools use `PowerTool`, and dedicated appliance components are preferred when the runtime exposes useful behaviour.

## Activation gate

Activation requires all Modern TSV domains, profile compatibility validation, exact component/material/substance/tag closure, lifecycle and craft consistency, populated manifests, repeatability coverage, full DatabaseSeeder tests, fresh and populated database replay, export parity and representative in-game inspection. The review records actual delta count and any justified target variance.
