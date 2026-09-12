# DatabaseSeeder instructions

Inherits [repository instructions](../AGENTS.md). This project is the interactive installer and stock-content seeder for FutureMUD worlds, including supported later installs and reruns.

## Ownership and structure

- Concrete seeders implement `IDatabaseSeeder` and are discovered by reflection; do not add registration in `Program` or a filesystem scan.
- Keep each seeder's partials/private helpers in `Seeders/<SeederClassName>/`, and shared helpers in `Seeders/Utilities/`. Preserve namespaces when moving files.
- Use and dispose `FuturemudDatabaseContext` for data access. Keep console questions and output clear.
- Installer-wide assets live in `Assets/Database/` and `Assets/Manifests/`; preserve that layout in build and publish output.

## Repeatability and stock ownership

- Prefer safe, additive, idempotent installs where stock ownership is clear. Do not claim every existing seeder is safely rerunnable: declare honest repeatability/update semantics in metadata and design documentation.
- Fix incorrect stock data in its source definition. Repair/update paths are for existing databases or explicitly documented reconciliation of stock-owned data, not substitutes for correcting a bad initial install.
- Prefer deterministic lookup/upsert behaviour with stable ownership boundaries. Do not overwrite builder-owned customisations merely to make a rerun converge.
- Ask few questions. Install coexisting stock options together when safe rather than introducing unnecessary setup forks.
- Reuse shared answer keys and the generic `SeederChoice` answer-memory flow. Keep prerequisites specific and actionable.
- `ExtraPackagesAvailable` alone is not a promise of repeatability. Extend the shared framework before inventing a bespoke pattern for foundational or complex seeders.
- For editable entities with `(Id, RevisionNumber)` keys, inspect mappings/migrations before assuming generated IDs. Tables such as `DisfigurementTemplates` can require explicit allocation of the next available entity ID.

## Task-specific references

- For repeatability, ownership, answer-memory or workflow changes, read the affected section of the [repeatability strategy](../Design%20Documents/Seeding/DatabaseSeeder_Repeatability_Strategy.md).
- For enabled-seeder metadata, dependency/order, question contracts or Debug replay changes, use `futuremud-database-seeder` and review replay-profile drift. Catalogue-only edits do not automatically require that skill.
- For clan templates or their clone behaviour, use [ClanSeeder instructions](Seeders/ClanSeeder/AGENTS.md). They do not govern unrelated seeders.
- For economy seeding, consult the relevant `Design Documents/Economy/` runtime, integration, and seeder-state documents. Treat broader economy seeding as an explicit product decision informed by the opportunity matrix; inspect the current seeder inventory rather than relying on a fixed count in prose.
- For emitted markup, use [character descriptions](../Design%20Documents/Markup/Character_Description_System.md) or [human seeder patterns](../Design%20Documents/Markup/Human_Seeder_Description_Patterns.md), as applicable.

Seeder regressions belong in `DatabaseSeeder Unit Tests`; long-running seeded-climate regressions belong in `MudSharpCore Climate Tests`. Preserve the additional workflow/replay verification gates when those contracts change.
