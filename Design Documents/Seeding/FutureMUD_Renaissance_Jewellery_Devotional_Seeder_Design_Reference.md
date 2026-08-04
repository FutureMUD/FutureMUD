# FutureMUD Renaissance Jewellery and Devotional Seeder Design Reference

## Scope

This is the canonical source contract for 940 stock item prototypes owned by the Renaissance item-seeder branch. It covers 940 jewellery and devotional forms plus 0 doors, locks, gates, latches, keys, and fittings. The companion CSV and FDesc CSV are authoritative for every literal `CreateItem(...)` call.

## Authoring rules

- Stable references are lowercase product identifiers with no content-pass or duplicated-segment labels.
- Public descriptions describe visible form, material, fitting, finish, and wear. They do not make unsupported mechanical, magical, or sacred-effect claims.
- Jewellery uses one supported wearable profile plus one destruction profile. Devotional fixtures use only actually supported portable or fixed composition.
- Doors and gates use the exact `Door_*` profiles; loose locks, keys, and latches use only the supplied warded and latch components. No row claims custom key pairing.
- Quality expresses workmanship; cost, mass, material, tags, and component profiles are source data rather than inferred from a display name.

## Culture and admission

Culture is maintained in the catalogue admission field and builder note. Renaissance rows with a `Renaissance / Early Modern` availability value are seeded once under Renaissance ownership and installed by the Early Modern dispatcher as explicit earlier-era admissions. The `preindustrial_*` rows are genuine four-era stock only.

## Validation

`scripts/generate-renaissance-earlymodern-jewellery-doors.py --check` verifies counts, unique references, C# output, and both CSV companions. Seeder tests additionally verify the stable-reference, dependency, culture-admission, direct-craft, and description contracts.
