# Clan Seeder Template Catalogue

## Purpose

`ClanSeeder` installs stock clans that builders can clone and adapt. The templates demonstrate ranks, rank paths,
appointments, appointment hierarchies, and privilege delegation; they are not intended to prescribe a single accurate
structure for every culture represented by their names.

All stock entries have `IsTemplate` enabled and are hidden from the player `who` and notable-member surfaces. Cloned
organisations should be renamed, reviewed for setting-specific titles, and have their privileges, election rules, pay,
property, and membership visibility checked before use.

## Catalogue

The seeder currently installs 50 templates. The original 20 templates remain available:

- Feudalism, peerage, monastic order, and chivalric order
- Roman legion and Roman city
- Generic council and gang
- UK police, army, navy, and air force
- Two local-government election models
- Company, mercenary company, infantry company, and battalion
- Capital ship and age-of-sail warship

The expanded catalogue adds the following 30 templates:

| Family | Templates | Intended adaptation seams |
| --- | --- | --- |
| Kinship and customary governance | Extended Family; Lineage Clan; Tribal Council | Household authority, lineage branches, elders, succession, collective property, customary leadership |
| Historical military | Norse Warband; Steppe Horde; East Asian Imperial Army | Retainers and household guards, mobile confederation command, traditional unit and general staff structures |
| Traditional government | Japanese Feudal Domain; East Asian Imperial Bureaucracy; Islamic Sultanate Court; South Asian Royal Court; West African Royal Court | Domain government, examination ministries, court and fiscal offices, royal household, provincial and oral-history roles |
| Religion | Roman Religious Cult; Buddhist Temple; Daoist Temple; Sufi Order; Hindu Temple | Priesthood or monastic grades, ritual offices, teaching, discipline, hospitality, charity, festivals, and temple finance |
| Trade and maritime | Merchant Guild; Craft Guild; Pirate Crew | Apprenticeship, standards, examinations, market agents, shared funds, shipboard command, and quartermaster authority |
| Modern institutions | University; Hospital; Fire and Rescue Service; Intelligence Agency; Political Party; Labour Union; Non-Governmental Organisation | Academic and clinical departments, emergency stations, intelligence directorates, party branches, workplace representation, programmes and field teams |
| Speculative and science fiction | Space Colony Administration; Civilian Starship Crew; Exploration Corps; Resistance Network | Life support and settlement government, civilian ship departments, expedition commands, and compartmentalised clandestine cells |

## Seeder Behaviour

The 30 expanded templates are defined as validated data in `ClanSeeder.Templates.cs`. Each definition supplies:

- A unique stock name and alias
- A builder-facing description and organisational sphere
- At least four ranks with explicit rank paths and progressively delegated privilege tiers
- At least five appointments, including a top office and a subordinate appointment hierarchy

The shared installer validates duplicate names and aliases, duplicate ranks, missing minimum-rank references, and
forward or missing parent-appointment references before writing the templates. Existing stock templates are detected by
name, so rerunning the seeder adds missing templates without duplicating installed ones.

## Builder Checklist

After cloning a template:

1. Replace culturally generic or borrowed titles with terms appropriate to the game setting.
2. Decide whether seniority should come from ranks, appointments, elections, or a mixture.
3. Review every staff and command privilege, especially induction, dismissal, treasury, bank, jobs, pay, and property.
4. Configure paygrades, elections, calendars, bank accounts, clan halls, and employment-host behaviour where needed.
5. Review membership visibility and whether the organisation should appear in player-facing lists.
6. Delete ranks and offices that do not fit; the templates deliberately expose useful structural possibilities rather
   than claiming that every named culture used every office in every period.

