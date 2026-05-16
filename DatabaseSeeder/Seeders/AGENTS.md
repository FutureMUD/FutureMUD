# Scope
This document defines rules and working guidance for the `DatabaseSeeder/Seeders` module, with a focus on clan template seeding and clone-friendly clan design.

It inherits from:
- [Solution AGENTS](../AGENTS.md)
- [Project AGENTS](../AGENTS.md)

## Clan Terminology
- `Clan`: The engine's general-purpose organisation construct. Military units, companies, councils, guilds, gangs, governments, and ships can all be modeled as clans.
- `Rank`: The single mandatory vertical status every clan member has. A member always has exactly one rank.
- `Appointment`: Optional office-holding layered on top of rank. Members may hold zero or more appointments.
- `Paygrade`: Optional direct compensation band for a member. A member may have zero or one direct paygrade, but may also receive pay transitively from an appointment-linked paygrade.
- `Template clan`: A clan marked with `IsTemplate = true` and intended to be copied rather than used directly.
- `Clone`: A live clan created from a template through `clan create template ...`.
- `Election`: Appointment-driven office turnover. Elections live on appointments, not on clans or ranks.

## Code Hotspots
- `DatabaseSeeder/Seeders/ClanSeeder.cs`
  Creates stock clan templates and any supporting FutureProgs required by seeded templates.
- `MudSharpCore/Commands/Modules/ClanModule.cs`
  Handles cloning template clans into live clans. Any template feature that depends on copied appointments, paygrades, or election metadata must be reflected here.
- `MudSharpCore/Community/Clan.cs`
  Template clans skip election loading during runtime load. Live clones do not.
- `MudSharpCore/Community/Appointment.cs`
  Encapsulates appointment privileges, optional paygrade linkage, election settings, and by-election behavior.
- `MudSharpCore/Community/Election.cs`
  Handles nomination, voting, installation, and runoff logic for elected appointments.

## Text Markup Reference
- [Character Description System](../../Design%20Documents/Markup/Character_Description_System.md)
- [Human Seeder Description Patterns](../../Design%20Documents/Markup/Human_Seeder_Description_Patterns.md)

## Design Guidance
- Prefer rank to represent a member's durable place in the organisation.
- Prefer appointment to represent a billet, office, post, or special duty that may change without changing core rank.
- Prefer paygrades to represent reusable compensation bands rather than baking pay into rank names.
- When a real-world organisation is being simplified for gameplay, preserve recognisable structure over exhaustive bureaucracy.
- Keep template data builder-friendly. Generic names are often better than highly setting-specific lore names.
- Avoid requiring builders to repair cloned templates by hand unless the behavior is inherently world-specific.

## Template Versus Live Clans
- Template clans should be safe reference material and safe copy sources.
- Template clans do not load live elections at runtime because `Clan.FinaliseLoad` skips election loading when `IsTemplate` is true.
- Election-enabled appointments are still valid on template data, but their real value is in live clones.
- If a template depends on election eligibility or electorate rules that reference the concrete cloned clan, the clone path must rewrite those progs for the new clan.

## Rank, Appointment, And Paygrade Usage
- Members always have exactly one rank.
- Appointments are optional and stackable. Use them for command posts, civic offices, department heads, watch commanders, shipboard specialists, and similar roles.
- Paygrades may attach directly to a member or indirectly through an appointment. When modeling organisations with temporary command billets, prefer appointment paygrades only when the billet itself should grant extra pay.
- Use `RankPath` to separate promotion ladders that should not be freely crossed, such as enlisted/officer/warrant or civilian/board.

## Election Guidance
- Elections belong to appointments.
- `CanNominateProg`, `WhyCantNominateProg`, and `NumberOfVotesProg` currently take a single `character` parameter.
- For template clans, any election prog that must reference the concrete cloned clan should use a stable placeholder convention and be rewritten during clone.
- Use strict electorate rules only where the template would otherwise be misleading to builders.
- Secret ballots, term lengths, nomination periods, voting periods, and holder counts should all survive cloning intact.

## Seeder Guidance
- Clan seeding must be idempotent. Always guard by stable template names before creating data.
- If adding a new template clan, also update `ShouldSeedData` detection so reruns advertise missing templates correctly.
- Prefer helper methods when adding repeated clan boilerplate: clan creation, rank creation, appointment creation, paygrade creation, and rank-paygrade linking.
- When seeding FutureProgs for template support, use deterministic names and comments so future clone logic can identify intent.

## Builder Workflows
- Builders can inspect templates with `clan templates` and `clan view <template>`.
- Builders create live organisations from templates with `clan create template <template> <alias> <name>`.
- After cloning, builders are expected to customise names, privileges, pay, insignia, and any setting-specific appointments that do not fit their world exactly.
- Templates should aim to remove setup tedium, not to eliminate all tuning.

## Common Edge Cases
- A member can hold a command appointment without changing rank. Model acting command or specialist posts this way.
- Appointment holder counts matter for elections. `MaximumSimultaneousHolders` should be explicit for elected offices.
- Unlimited-holder appointments are usually a poor fit for elections.
- Template election progs that reference a specific clan by id or name must be regenerated during clone or they will point at the wrong clan.
- Rank names and appointment names used by clone-generated progs should remain stable inside the template definition.
- If a template mixes direct member paygrades with appointment paygrades, document that clearly because pay can stack.

## User Stories
- A builder wants a modern army and should be able to clone a usable baseline with enlisted, warrant, officer, and staff appointments already wired.
- A builder wants a company and should be able to clone one with paygrades and a board-elected CEO instead of hand-building every tier.
- A builder wants a town council and should be able to choose between broad-population mayoral elections and councillor-elected mayors.
- A builder wants naval organisations and ships with recognisable command billets instead of starting from a blank clan.
- A builder wants a mercenary company template that captures the feel of a captain-led fighting force without requiring historical perfection.

## Practical Examples
- Military templates: rank carries career standing; appointments carry command posts like CO, XO, adjutant, `S1`, or chief engineer.
- Civic templates: resident or councillor rank/office models baseline membership; mayor is usually an appointment and often elected.
- Company templates: employee through board are career tiers; CEO is a special appointment with board-driven election logic.
- Nautical templates: rank or broad station shows standing, while appointments handle captain, master, boatswain, surgeon, and other shipboard roles.
