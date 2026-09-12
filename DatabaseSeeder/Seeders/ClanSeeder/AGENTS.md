# Clan template seeding instructions

Inherits [repository instructions](../../../AGENTS.md), [DatabaseSeeder instructions](../../AGENTS.md), and [seeder module instructions](../AGENTS.md). These rules concern clan templates and their clone behaviour, not all seeded content.

## Model and terminology

- A `Clan` is the general organisation construct: armies, companies, councils, guilds, gangs, governments and ships can all be clans.
- A member has exactly one `Rank`: durable vertical standing. Use `RankPath` for ladders that should not be crossed freely, such as enlisted/officer/warrant or civilian/board.
- `Appointment` is optional, stackable office-holding layered over rank. Use it for command billets, acting command, civic offices, department heads and specialists rather than changing career rank.
- A member has zero or one direct `Paygrade`; appointment-linked paygrades can add compensation. Use reusable compensation bands, and document cases where direct and appointment pay stack.
- A template clan has `IsTemplate = true`; a clone is a live clan made through `clan create template ...`.
- Elections belong to appointments, not clans or ranks.

## Source map: follow only the affected path

| Surface | Source |
| --- | --- |
| Stock templates and supporting progs | `DatabaseSeeder/Seeders/ClanSeeder/ClanSeeder.cs` |
| Copying templates, appointments, pay and election metadata | `MudSharpCore/Commands/Modules/ClanModule.cs` |
| Template/live load distinction | `MudSharpCore/Community/Clan.cs` |
| Appointment privileges, pay and election configuration | `MudSharpCore/Community/Appointment.cs` |
| Nomination, voting, installation and runoff | `MudSharpCore/Community/Election.cs` |

Paths in this table are repository-relative. When editing runtime files, also apply their owning instructions.

## Template and clone invariants

- Templates must be safe reference material and copy sources. `Clan.FinaliseLoad` skips election loading for templates; live clones do load elections.
- Election-enabled appointments remain valid on templates. Preserve secret ballots, term lengths, nomination/voting periods, holder counts, privileges, and pay links when cloning.
- Election progs requiring the concrete cloned clan must use an identifiable placeholder convention and be rewritten/regenerated for the new clan. Never leave a clone's prog pointing at the template's ID/name.
- `CanNominateProg`, `WhyCantNominateProg`, and `NumberOfVotesProg` use a single `character` parameter; confirm the live signature when changing this contract.
- Set `MaximumSimultaneousHolders` explicitly for elected offices. Unlimited-holder appointments are usually unsuitable for elections.
- Keep template rank/appointment names stable where clone-generated progs depend on them. Use deterministic prog names and comments to make intent identifiable.

## Seeder and content design

- Clan seeding must be idempotent: guard creation with stable template identities/names. When adding a template, update `ShouldSeedData` detection so reruns advertise missing templates.
- Reuse helpers for clan, rank, appointment, paygrade and rank/paygrade-link creation.
- Preserve recognisable organisational structure without reproducing exhaustive real-world bureaucracy. Generic, builder-friendly names normally serve stock templates better than setting-specific lore.
- Avoid requiring manual repairs after cloning unless the remaining choice genuinely depends on the world. Templates should remove setup tedium, not eliminate all customisation.
- Use strict electorate rules when necessary to avoid misleading builders; do not impose complexity without a content reason.

Examples of the intended distinction:

- Armies: rank records career standing; CO, XO, adjutant, `S1` and chief engineer are appointments. Preserve useful enlisted/warrant/officer/staff distinctions.
- Companies: employee-to-board standing is separate from a board-elected CEO appointment.
- Councils: distinguish a mayor elected by the broader population from a councillor-elected mayor.
- Ships: use broad standing/rank plus captain, master, boatswain, surgeon and other billets.
- Mercenary companies: retain a recognisable captain-led organisation without requiring historical perfection.

## Builder workflow and verification

Builders inspect with `clan templates` and `clan view <template>`, then clone with `clan create template <template> <alias> <name>`. Names, privileges, pay, insignia and world-specific offices remain customisable.

For changes affecting clones, verify the copied configuration and rewritten progs, not just the seeded template. Cover missing-template detection and repeatability for new stock content. Use the appropriate seeder/runtime test suites for the paths changed.

For character-description markup emitted by this seeder, consult [character descriptions](../../../Design%20Documents/Markup/Character_Description_System.md) or [human seeder patterns](../../../Design%20Documents/Markup/Human_Seeder_Description_Patterns.md) only as needed.
