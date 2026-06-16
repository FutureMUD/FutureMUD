# Drug System Documentation

This folder contains the dedicated drug-system documentation for FutureMUD.

## Documents
- [Drug System Design](./Drug_System_Design.md) is the developer-facing implementation map. It explains `IDrug`, persistence, dosing, metabolism, delivery vectors, effect aggregation, and extension points.
- [Drug Builder Guide](./Drug_Builder_Guide.md) is the content-facing guide for builders, seeder developers, and AI agents authoring drugs, medicines, venoms, poisons, tinctures, food doses, smokeables, salves, and related delivery items.

## Relationship To Health
Drugs are implemented as part of the health runtime, but they are intentionally cross-cutting. A finished drug feature may involve health code, item components, fluid materials, combat attacks, magic spell effects, FutureProg hooks, and DatabaseSeeder content.

