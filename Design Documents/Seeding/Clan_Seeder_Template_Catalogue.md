# Clan Seeder Template Catalogue

## Purpose

`ClanSeeder` installs stock clans that builders can clone and adapt. The templates demonstrate ranks, rank paths,
appointments, appointment hierarchies, and privilege delegation; they are not intended to prescribe a single accurate
structure for every culture represented by their names.

All stock entries have `IsTemplate` enabled and are hidden from the player `who` and notable-member surfaces. Cloned
organisations should be renamed, reviewed for setting-specific titles, and have their privileges, election rules, pay,
property, and membership visibility checked before use.

## Catalogue

The seeder currently installs 82 templates. The original 20 templates remain available:

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

## Catholic catalogue

The 32 Catholic templates add 133 ranks and 250 appointments, including 56 elected offices. They install no members, paygrades, assets, live external reservations or ballot history. Every core office has recall disabled. The eight alternative profiles are explicit provisioning choices, not additional templates.

See [Catholic provisioning](Catholic_Clan_Template_Provisioning.md) for staging, bindings, qualifications and deliberate abstractions.

| Template | Alias |
| --- | --- |
| Catholic Church Template | `catholicchurch` |
| Catholic Diocese Template | `catholicdiocese` |
| Catholic Archdiocese Template | `catholicarchdiocese` |
| Catholic Parish Template | `catholicparish` |
| Catholic Abbey Template | `catholicabbey` |
| Catholic Convent Template | `catholicconvent` |
| Catholic Friary Template | `catholicfriary` |
| Catholic Religious Order Template | `catholicorder` |
| Catholic Religious Province Template | `catholicprovince` |
| Catholic Military Order Template | `catholicmilitaryorder` |
| Franciscan Order Template | `franciscanorder` |
| Franciscan Province Template | `franciscanprovince` |
| Franciscan Friary Template | `franciscanfriary` |
| Dominican Order Template | `dominicanorder` |
| Dominican Province Template | `dominicanprovince` |
| Dominican Priory Template | `dominicanpriory` |
| Carmelite Order Template | `carmeliteorder` |
| Carmelite Province Template | `carmeliteprovince` |
| Carmelite Priory Template | `carmelitepriory` |
| Augustinian Order Template | `augustinianorder` |
| Augustinian Province Template | `augustinianprovince` |
| Augustinian Priory Template | `augustinianpriory` |
| Benedictine Abbey Template | `benedictineabbey` |
| Benedictine Dependent Priory Template | `benedictinepriory` |
| Benedictine Congregation Template | `benedictinecongregation` |
| Cistercian Order Template | `cistercianorder` |
| Cistercian Abbey Template | `cistercianabbey` |
| Carthusian Order Template | `carthusianorder` |
| Carthusian Charterhouse Template | `carthusianhouse` |
| Poor Clare Monastery Template | `poorclaremonastery` |
| Premonstratensian Order Template | `norbertineorder` |
| Premonstratensian Abbey Template | `norbertineabbey` |

## Seeder Behaviour

The 30 expanded templates are defined as validated data in `ClanSeeder.Templates.cs`. Each definition supplies:

- A unique stock name and alias
- A builder-facing description and organisational sphere
- At least four ranks with explicit rank paths and progressively delegated privilege tiers
- At least five appointments, including a top office and a subordinate appointment hierarchy

The Catholic definitions preserve explicit privilege flags and all native governance fields; the expanded catalogue's four-rank/five-office authoring minimum does not apply to them. The authoritative field-level catalogue and source references are embedded in `DatabaseSeeder/Seeders/ClanSeeder/Catholic/CatholicTemplates.json` and consumed by the installer. It emits generic declarative profiles and a Catholic provisioning prog suite; the runtime has no Catholic catalogue or commands.

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

## Other template address defaults

Selected non-Catholic templates now install explicit direct-address rules for newly created template identities, including junior-to-officer Sir/Ma'am with rank-based peer/superior fallbacks, ship Captain, civic and academic offices, British-inspired royal/peerage styles, and Latin role vocatives for the Roman legion, city and cult. Staff roles usually retain rank address; occupations and seniority do not automatically confer an honorific. Gang and intelligence-agency styles are members-only. Existing templates and live clans are preserved on rerun.

See the [complete address review](Clan_Seeder_Forms_of_Address_Review.md) for the disposition of all 50 non-Catholic templates, source references, agreed choices and remaining cultural, religious and legacy Roman ambiguities. The generic engine and schema are unchanged by this catalogue addition.
