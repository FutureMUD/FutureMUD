# ArmageddonSeeder Unit Tests

Inherits the repository instructions. Use MSTest and deterministic fixtures. Installer-specific tests live here; shared runtime tests remain with their owning project. External corpus checks must explicitly skip when their private packet is absent; never embed or publish the packet. Mark complete installer matrices Integration and keep fast checks separate.

Apply the [seeder test policy](../Design%20Documents/Seeding/Seeder_Test_Policy.md). Preserve importer transformations, runtime loading, ownership, recovery and installation behaviour. Do not freeze spell rosters, authored defaults, private-corpus inventories or setting prose. A catalogue exception must identify a concrete high-risk compatibility failure.
