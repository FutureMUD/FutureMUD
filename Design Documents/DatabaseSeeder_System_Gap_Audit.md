# FutureMUD Database Seeder System Gap Audit

## Scope
This document audits the Database Seeder against the builder-facing systems a fully operational FutureMUD game commonly needs.

It is intentionally focused on stock worldbuilding support, not on every runtime subsystem.

The audit is based on:

- the current concrete seeder classes in `DatabaseSeeder/Seeders`
- the existing subsystem seeder-state documents, especially economy and health
- builder-facing command surfaces in `MudSharpCore/Commands/Modules`
- the top-level runtime and design-document surface in `MudSharpCore` and `Design Documents`

## Exclusions
The following are intentionally excluded from stock-seeder ambitions except as scaffolding:

- room maps, cell layouts, exits, and placed area content
- bespoke NPC populations tied to a specific map or storyline
- live institutions that only make sense once a world map and politics exist, such as specific branches, probate offices, auction houses, or shops in concrete cells
- one-off setting lore, religions, dynasties, governments, or named factions that are too setting-specific to be reusable
- large bespoke FutureProg networks whose value depends on a specific game's custom content rather than on a reusable engine baseline

## Current Seeder Roster
The current registered seeder surface in `DatabaseSeeder/Seeders` is:

- `CoreDataSeeder`
- `TimeSeeder`
- `CelestialSeeder`
- `AttributeSeeder`
- `SkillSeeder`
- `SkillPackageSeeder`
- `CurrencySeeder`
- `HumanSeeder`
- `ClanSeeder`
- `CombatSeeder`
- `ChargenSeeder`
- `CultureSeeder`
- `ArenaSeeder`
- `UsefulSeeder`
- `AIStorytellerSeeder`
- `HealthSeeder`
- `EconomySeeder`
- `AnimalSeeder`
- `WeatherSeeder`
- `MythicalAnimalSeeder`
- `RobotSeeder`
- `ItemSeeder`
- `LawSeeder`

## Rubric
Each system is classified with the following fields:

- `Current State`: what is already seeded, and by which seeders
- `Runtime Breadth vs Seeder Breadth`: what the engine supports beyond stock seeding
- `Seeder Fit`: `Strong`, `Possible`, or `Poor`
- `Template Strategy`: stock templates, per-setting packs, or builder-authored only
- `Recommended Seeder Scope`: `None`, `Scaffold`, `Template Library`, or `Near-complete baseline`
- `Notes / Risks`: why the recommendation stops where it does

## Foundations and World Scaffolding

### Core Data
Fully operational games need a baseline of canonical static strings, bootstrap defaults, and stock engine records that other systems expect to exist.

- `Current State`: `CoreDataSeeder` seeds foundational records such as the stock item health strategy and other core bootstrap data.
- `Runtime Breadth vs Seeder Breadth`: the runtime surface is broader than the current seeder and includes many foundational records that other seeders still treat implicitly.
- `Seeder Fit`: `Possible`
- `Template Strategy`: common engine baseline only, not setting packs
- `Recommended Seeder Scope`: `Scaffold`
- `Notes / Risks`: this is foundational, high-risk content. It should seed only canonical engine-owned defaults and helper records, not try to encode world identity.

### Attributes, Skills, and Skill Packages
Fully operational games need trait definitions, checks, decorators, improvers, and starter packages so other systems can function and characters can interact with them.

- `Current State`: `AttributeSeeder`, `SkillSeeder`, and `SkillPackageSeeder` provide the baseline trait and skill scaffolding. Other seeders rely on them heavily.
- `Runtime Breadth vs Seeder Breadth`: the runtime can support many more traits, checks, languages, and package philosophies than the stock set currently covers.
- `Seeder Fit`: `Strong`
- `Template Strategy`: one canonical baseline plus optional setting or genre packages
- `Recommended Seeder Scope`: `Near-complete baseline`
- `Notes / Risks`: this is high-value reusable content. The main gap is breadth, not suitability. Additional optional packages are a straightforward seeder win.

### Time
Fully operational games need at least one working calendar, clock, and timezone model so scheduling, chargen, law, economy, and weather systems function.

- `Current State`: `TimeSeeder` already seeds calendars, clocks, timezones, and related links as repair-capable canonical content.
- `Runtime Breadth vs Seeder Breadth`: the runtime supports more possible world-calendar variants than the current stock baseline likely exposes.
- `Seeder Fit`: `Strong`
- `Template Strategy`: common real-world and fantasy calendar packs
- `Recommended Seeder Scope`: `Near-complete baseline`
- `Notes / Risks`: this is an excellent stock system because calendars and clocks are global scaffolding rather than bespoke map content.

### Celestial, Weather, and Climate
Fully operational games need the astronomical and environmental shell that drives daylight, seasons, weather events, and climate behaviour.

- `Current State`: `CelestialSeeder` and `WeatherSeeder` already provide meaningful stock coverage. Weather is now repair-capable by stable keys, and climate templates are already a stock concept.
- `Runtime Breadth vs Seeder Breadth`: the runtime can support more celestial topologies, region assignments, and world-specific environmental tuning than the stock packages currently expose.
- `Seeder Fit`: `Strong`
- `Template Strategy`: astronomy packs and climate packs by world style
- `Recommended Seeder Scope`: `Near-complete baseline`
- `Notes / Risks`: seed the world shell, not the full map integration. Regional assignment and final map-specific controller setup should remain builder-owned.

### Chargen
Fully operational games need a playable chargen flow with resources, stages, roles, helper progs, and screen/storyboard structure.

- `Current State`: `ChargenSeeder` already seeds substantial chargen scaffolding and is repair-capable.
- `Runtime Breadth vs Seeder Breadth`: the runtime chargen model can support many more setting-specific flows, restrictions, and narrative wrappers than the stock package provides.
- `Seeder Fit`: `Strong`
- `Template Strategy`: multiple starter chargen packages by game style
- `Recommended Seeder Scope`: `Near-complete baseline`
- `Notes / Risks`: the current baseline is already useful. The main opportunity is alternative chargen packages for different genres or stricter RPI styles.

### Cultures, Languages, Ethnicities, and Heritages
Fully operational games need at least one coherent naming, culture, language, and ethnicity ecosystem so chargen and race presentation are usable.

- `Current State`: `CultureSeeder` is already one of the strongest template-library seeders, with language, script, culture, ethnicity, and name-pack support. `HumanSeeder` and `RobotSeeder` also contribute race-facing cultural content.
- `Runtime Breadth vs Seeder Breadth`: the runtime supports much more setting-specific authoring than the stock packs can ever fully represent.
- `Seeder Fit`: `Strong`
- `Template Strategy`: per-setting culture packs, not one canonical answer
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: this should stay template-driven. Builders benefit from cloneable baselines, but cultures are too identity-defining for a single stock answer to be sufficient.

## Character and Body Ecosystems

### Humans
Fully operational games need a playable baseline sapient race with anatomy, characteristics, health defaults, corpse models, and chargen-facing presentation.

- `Current State`: `HumanSeeder` provides the stock humanoid baseline and seeds a large amount of practical runtime setup beyond just race data.
- `Runtime Breadth vs Seeder Breadth`: the runtime can support many more human variants, baseline trait assumptions, and presentation packs than the seeder exposes today.
- `Seeder Fit`: `Strong`
- `Template Strategy`: one canonical human baseline plus optional setting-specific heritage and presentation packs
- `Recommended Seeder Scope`: `Near-complete baseline`
- `Notes / Risks`: this is core bootstrap content and should remain deep. The gaps are mostly in optional breadth, not whether it belongs in the seeder.

### Animals
Fully operational games often need a stock fauna baseline for ecology, hunting, mounts, predators, and ambience.

- `Current State`: `AnimalSeeder` now provides broad and deep stock coverage for animal bodies, races, prose, attacks, venoms, and health integration.
- `Runtime Breadth vs Seeder Breadth`: the engine can support more species, ecological niches, and world-specific rosters than any one stock package should try to cover.
- `Seeder Fit`: `Strong`
- `Template Strategy`: reusable fauna families by ecology or genre
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: animal support is an excellent seeder candidate, but final species rosters should remain builder-curated per game.

### Mythical Creatures
Fully operational fantasy or mythic games often need a stock catalogue of fantasy creatures and hybrid playable forms.

- `Current State`: `MythicalAnimalSeeder` is already an optional template package with meaningful stock breadth.
- `Runtime Breadth vs Seeder Breadth`: runtime support can handle additional races, hybrid bodies, and setting-specific presentation beyond the current catalogue.
- `Seeder Fit`: `Strong`
- `Template Strategy`: optional fantasy packs by setting tone
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: this should remain optional. It is a very good seeder package, but only for worlds that want it.

### Robots
Fully operational science-fiction or post-human games often need a stock robot ecosystem with bodies, races, maintenance procedures, and support culture.

- `Current State`: `RobotSeeder` already supplies a strong optional robot package.
- `Runtime Breadth vs Seeder Breadth`: the engine supports more robot chassis, component ecosystems, and setting-specific identities than the current stock package covers.
- `Seeder Fit`: `Strong`
- `Template Strategy`: optional robot packs by tech level or setting family
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: like mythical creatures, this is a strong optional template domain rather than universal baseline content.

### Health, Medicine, and Disfigurement
Fully operational games need functioning wound care, procedures, supplies, health strategies, and visible consequences of injury.

- `Current State`: health support is already spread across `HealthSeeder`, `HumanSeeder`, `AnimalSeeder`, `RobotSeeder`, `ItemSeeder`, `UsefulSeeder`, `SkillSeeder`, and `SkillPackageSeeder`. The existing health seeder-state document already treats this as a strong stock subsystem.
- `Runtime Breadth vs Seeder Breadth`: the runtime remains broader than stock seeding in high-tech medicine, advanced implants, non-default health strategies, and some specialist medical items.
- `Seeder Fit`: `Strong`
- `Template Strategy`: tech-level medical packages plus optional specialist add-ons
- `Recommended Seeder Scope`: `Near-complete baseline`
- `Notes / Risks`: this is a strong stock system because it benefits from shared baselines. The main gap is not suitability but the breadth of advanced or niche content.

## Social, Legal, and Economic Institutions

### Clans and Template Organisations
Fully operational games usually need at least a few institution patterns for militaries, companies, councils, crews, gangs, or guilds.

- `Current State`: `ClanSeeder` already seeds stock template organisations and is explicitly designed around clone-friendly templates.
- `Runtime Breadth vs Seeder Breadth`: the clan runtime is far broader than the current template list. Many organisation families remain builder-authored.
- `Seeder Fit`: `Strong`
- `Template Strategy`: broad template families by institution type
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: this is exactly the kind of reusable content that belongs in a seeder. The seeder should provide recognisable structures, not named lore factions.

### Law and Justice
Fully operational games often need at least one functioning law-enforcement and legal-authority baseline so crimes, arrest, and civic consequences are playable.

- `Current State`: `LawSeeder` already provides repair-capable stock legal content. `EconomySeeder`, `Estate_Probate_and_Item_Ownership`, and command surfaces show adjacent runtime integration.
- `Runtime Breadth vs Seeder Breadth`: the runtime can support more governance styles, crime policies, and enforcement flavours than the current stock package likely provides.
- `Seeder Fit`: `Strong`
- `Template Strategy`: legal template families by governance style
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: law is reusable at the framework level, but final crimes, penalties, and political flavour should still be tuned per world.

### Economy
Fully operational games need money, at least one economic zone, and enough market logic that prices, trade, and value have coherent defaults.

- `Current State`: `CurrencySeeder` and `EconomySeeder` provide strong partial coverage. The economy seeder-state document confirms good support for currencies, economic zones, stock markets, market categories, influence templates, populations, and shoppers.
- `Runtime Breadth vs Seeder Breadth`: runtime support is much broader and includes taxes, banks, bank account types, property, auction houses, jobs, and shops. Those are only partially or not at all seeded as dedicated stock packages.
- `Seeder Fit`: `Strong`
- `Template Strategy`: era packs and policy-template packs
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: economy is a good seeder domain up to reusable policy and market scaffolding. It becomes a poor stock fit when it reaches map-bound retail and institution placement.

### Property, Estates, and Probate
Fully operational games with ownership and inheritance systems need property policy, probate flows, and item-ownership rules that align with the economy.

- `Current State`: the runtime and command surface are rich, but there is no dedicated seeder package for property, estates, or probate. Existing support is indirect through economy and law adjacent data.
- `Runtime Breadth vs Seeder Breadth`: the runtime supports property records, sale and lease flows, estate claims, probate timings, morgue integration, and ownership metadata. Stock seeding does not yet package these into reusable defaults.
- `Seeder Fit`: `Possible`
- `Template Strategy`: zone-policy scaffolds and sample workflow templates, not live property portfolios
- `Recommended Seeder Scope`: `Scaffold`
- `Notes / Risks`: probate policy and ownership conventions are good seeder candidates. Concrete properties, conveyancers, auction houses, probate offices, and morgue rooms are too map-specific to be broadly seeded as live content.

### Jobs, Work, and Employment
Fully operational games often need employer/job scaffolding so labour, recruitment, and salary systems exist beyond pure roleplay.

- `Current State`: the runtime economy supports job listings and active jobs, and the broader `MudSharpCore/Work` subsystem supports crafts and projects. There is no dedicated seeder for employment templates.
- `Runtime Breadth vs Seeder Breadth`: the runtime can support persistent employers, eligibility progs, economic-zone job listings, active jobs, crafts, projects, and production chains well beyond current stock seeding.
- `Seeder Fit`: `Possible`
- `Template Strategy`: generic job and employer templates, not live staffing for named institutions
- `Recommended Seeder Scope`: `Scaffold`
- `Notes / Risks`: sample job frameworks, pay bands, and helper progs would be valuable. Live jobs tied to named clans, branches, or properties become highly world-specific very quickly.

## Gameplay Subsystems and Content Frameworks

### Combat Defaults
Fully operational games need combat settings, weapon families, attack suites, message styles, and default combat behaviours that produce a playable baseline immediately.

- `Current State`: `CombatSeeder` is already a major stock baseline, and non-human defaults are reinforced by `AnimalSeeder`, `MythicalAnimalSeeder`, and `RobotSeeder`.
- `Runtime Breadth vs Seeder Breadth`: the combat runtime supports more specialised settings, weapons, and behaviour policies than the stock catalogue, but the current seeding already covers the most important shared defaults.
- `Seeder Fit`: `Strong`
- `Template Strategy`: genre and tech-style combat packs
- `Recommended Seeder Scope`: `Near-complete baseline`
- `Notes / Risks`: combat defaults are highly reusable. The remaining gap is mainly breadth and modularisation, not whether they belong in stock seeding.

### Arenas
Fully operational games do not strictly require arenas, but worlds that use structured combat, tournaments, or training benefit from ready-made scaffolding.

- `Current State`: `ArenaSeeder` already provides a repair-capable stock arena scaffold.
- `Runtime Breadth vs Seeder Breadth`: the arena runtime can support more bespoke event structures and world-specific venues than the stock package encodes.
- `Seeder Fit`: `Strong`
- `Template Strategy`: generic arena scaffolds and event-type packs
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: seed the reusable scaffold, not a world's final venue geography or factional tournaments.

### Items, Item Components, and Crafting Scaffolds
Fully operational games need generic props, tools, consumables, components, and at least some crafting scaffolding so core loops are usable.

- `Current State`: `UsefulSeeder` and `ItemSeeder` provide a large stock item and component baseline, and `ItemSeederCrafting` shows that the item seeder already touches crafting-adjacent support. The item runtime itself is much broader and heavily builder-driven.
- `Runtime Breadth vs Seeder Breadth`: the engine supports a very large item-component catalogue, revisioned prototypes, skins, groups, telecoms, implants, thermal sources, and rich crafting/project systems that the seeder only samples.
- `Seeder Fit`: `Strong`
- `Template Strategy`: generic item and component libraries by tech level, plus example craft packs
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: stock generic toolkits belong in the seeder. Full merchandise catalogues, bespoke shop stock, and complete profession trees do not. Crafting should focus on sample patterns and reusable primitives, not exhaustive content.

### NPC AI and Group AI
Fully operational games benefit from reusable AI definitions and group behaviours so builders are not starting every NPC ecosystem from zero.

- `Current State`: there is no dedicated general-purpose AI seeder. `AIStorytellerSeeder` covers one adjacent optional system, and other seeders create helper progs for their own needs.
- `Runtime Breadth vs Seeder Breadth`: the NPC runtime already supports reusable individual AI definitions, group AI templates, event hooks, and builder command workflows, but these are not packaged into a general stock AI library.
- `Seeder Fit`: `Possible`
- `Template Strategy`: reusable behaviour packs, not full live NPC populations
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: generic AIs such as guards, passive townsfolk, predators, herd animals, patrol groups, and shopkeepers are good stock candidates. Named NPC templates, spawn populations, and map-tied patrol routes should remain builder-authored.

### Magic
Fully operational magic-enabled games need a full dependency chain of schools, capabilities, resources, regenerators, powers, spells, and the merits or items that expose them.

- `Current State`: there is no dedicated magic seeder. The magic overview document explicitly calls out this gap.
- `Runtime Breadth vs Seeder Breadth`: the runtime supports schools, resources, regenerators, capabilities, powers, spells, merits, drug integration, item integration, and cell integration, but none of this is packaged into stock seeding yet.
- `Seeder Fit`: `Possible`
- `Template Strategy`: optional per-setting or per-magic-style template packs
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: magic is too setting-defining for one canonical stock answer, but it is still a good optional template domain. The seeder should provide coherent starter schools, not assume every world wants the same metaphysics.

### AI Storyteller and Event-Hook Examples
Fully operational games do not require AI storytelling, but stock examples are valuable for worlds that want assisted narrative tooling or event-driven examples.

- `Current State`: `AIStorytellerSeeder` already exists as a repeatable stock example package, and the event/hook runtime is documented and available.
- `Runtime Breadth vs Seeder Breadth`: the runtime can support more hook-driven and AI-assisted behaviour than the stock examples currently cover.
- `Seeder Fit`: `Strong`
- `Template Strategy`: optional example packs
- `Recommended Seeder Scope`: `Template Library`
- `Notes / Risks`: this is a strong optional seeder domain because examples are more valuable than completeness. It should remain clearly example-driven rather than pretending to be universal baseline gameplay.

### FutureProg and Helper Scaffolding
Fully operational games need helper progs, predicates, and common stock logic that other seeders and builder systems can depend on.

- `Current State`: many seeders already create helper progs incidentally, but there is no dedicated library-style seeder for generic reusable FutureProg scaffolding.
- `Runtime Breadth vs Seeder Breadth`: the `ProgModule` and event/hook system expose a much broader programmable surface than the current stock seeding packages formalise.
- `Seeder Fit`: `Strong`
- `Template Strategy`: shared helper library, plus optional themed packs
- `Recommended Seeder Scope`: `Scaffold`
- `Notes / Risks`: reusable helper progs belong in the seeder. Large lore-specific scripting packages do not. This should support other seeders rather than become a kitchen-sink content dump.

## Cross-System Takeaways

### Strong existing stock baselines
These systems already have meaningful seeder support and should mostly be extended, not reinvented:

- attributes, skills, and skill packages
- time
- celestial, weather, and climate
- chargen
- cultures and naming packs
- humans
- animals
- mythical creatures
- robots
- health and medicine
- clans
- law
- combat
- arenas
- items and common components
- AI storyteller examples

### Runtime-rich but under-seeded systems
These systems have strong runtime capability but incomplete stock packaging:

- economy beyond currencies, markets, and shopper baselines
- property, estates, and probate
- jobs and employer scaffolding
- NPC AI and group AI template libraries
- magic
- reusable FutureProg helper libraries

### Systems that should remain mostly builder-authored
These should only receive scaffolds or templates, not full live stock content:

- room maps and physical settlement layouts
- live shop placement and stock decisions
- concrete property portfolios tied to named cells
- probate offices, morgues, auction houses, and banks placed into specific rooms
- named NPC populations and spawn ecology
- one-off lore factions, religions, dynasties, or governments

## Prioritised Backlog Matrix

### High Value / Low Ambiguity
- expand clan template families for more institution archetypes
- expand law template families by governance style
- add more chargen starter packages by game style
- expand culture, language, and naming packs
- add additional economy policy packs around taxes, bank templates, and account-type templates
- add a dedicated shared helper-prog seeder for stock reusable predicates and utility logic

### High Value / Needs Design
- add a general NPC AI and group AI template-library seeder
- add a magic template-library seeder with one or more coherent starter schools
- add property and probate scaffolding that seeds policy and helper defaults without pretending to seed live map content
- add employment scaffolding for generic employers, pay bands, and job-listing helpers
- deepen crafting support into reusable sample craft trees without trying to ship a full world economy catalogue

### Nice to Have
- more arena template variants
- more optional mythic and robot packs
- more optional health extensions for high-tech or niche medical play
- more optional combat packages by tech level or genre
- more optional AI storyteller example packs

### Do Not Seed
- complete room or settlement maps
- concrete placed shops with final stock and staffing
- concrete live property ownership portfolios
- named lore factions and one-off world governments
- map-tied NPC spawn populations and bespoke patrol graphs
- bespoke setting script networks whose value depends on one world's unique fiction

## Recommended Implementation Direction
If seeder work continues from this audit, the best next principle is:

- seed reusable system scaffolds deeply where the engine benefits from a common baseline
- seed template libraries where multiple recognisable world styles are plausible
- stop short of map-specific, polity-specific, or lore-specific live content

In practical terms, FutureMUD already has a strong Database Seeder for world scaffolding, bodies, chargen, climate, law, and several optional content ecosystems. The biggest remaining opportunity is not to seed entire finished worlds, but to turn the runtime-rich unseeded systems into clone-friendly template libraries and policy scaffolds.
