# FutureMUD Health System: Seeder State and Gaps

## Scope
This document explains two things:

1. How the current stock repo seeds health-relevant data.
2. Which runtime options are broader than the current stock setup, partially implemented, or obvious extension points.

This document is intentionally split into verified current-state observations and inferred extension candidates.

## Seeder Map
The health system is not seeded from only one place.

| Seeder | Verified health contribution | Current-state note |
| --- | --- | --- |
| `HealthSeeder` | Seeds medical knowledges, surgical procedures and phases, a broad tech-level drug catalogue, and optional mammal veterinary procedures | Enabled as a release-ready stock medical seeder |
| `HumanSeeder` | Seeds human health strategies, corpse models, blood models, population blood models, race defaults, chargen needs settings, and breathing-related race flags | This is where a large amount of practical health setup currently lives |
| `AnimalSeeder` | Seeds animal corpse models, health strategies, blood models, population blood models, race defaults, and multiple breathing model assignments | Also carries a large amount of effective health setup |
| `MythicalAnimalSeeder` | Seeds mythic beasts and hybrid folk using the same stock health strategies, corpse models, combat question set, and compatible body frameworks established by `HumanSeeder` and `AnimalSeeder` | Keeps the mythical catalogue separate from the old disabled `FantasySeeder` while still reusing the anatomy and medical compatibility infrastructure where practical |
| `RobotSeeder` | Seeds robot bodies, robot races, robot-specific health strategies, robot liquids and materials, robot corpse models, robot knowledges, and robot maintenance procedures | Rerunnable package that reuses humanoid and selected animal body layouts for compatibility with stock gear and anatomy-aware systems |
| `CoreDataSeeder` | Seeds the stock `GameItem` health strategy and some related static strings such as death messaging | Important for item damage support |
| `ItemSeeder` | Seeds named medical and health-adjacent items such as bandages, splints, tourniquets, suturing tools, prosthetics, and cannula items | Low-tech medical play is well represented here |
| `UsefulSeeder` | Seeds many medical component prototypes, including treatment items, cannula definitions, IV support, and prosthetic components | More component-heavy than `ItemSeeder` |
| `SkillSeeder` | Seeds the skills behind many health-related checks | Medical action quality depends on this seeding |
| `SkillPackageSeeder` | Seeds the package-level distribution of medical and rescue checks | Helps determine who can actually use the health systems well |

## Combat Seeder Coverage
### Verified current state
`CombatSeeder` is the stock source for combat message catalogues, weapon types, attack suites, combat strategies, and the stock unarmed and ranged move families that other seeders build upon.

Combat balance is now also a shared seeder concern:

- `HumanSeeder` asks once for `combat-balance-profile`
- the answer is recorded through the shared `SeederChoice` flow
- `CombatSeeder`, `AnimalSeeder`, `MythicalAnimalSeeder`, and `RobotSeeder` reuse that recorded answer instead of re-asking it
- stock reruns rewrite stock-owned named formulas in place rather than seeding duplicate attacks, races, or armour types

The seeder now treats message style as a shared concern instead of duplicating punctuation and join rules inline. A single internal helper normalizes how `Compact`, `Sentences`, and `Sparse` styles:

- terminate seeded attack strings
- append defense success and failure clauses
- join hit-location follow-up text
- keep standalone ranged, grapple, clinch, and coup-de-grace strings punctuated correctly
- lets later non-human seeders reuse the recorded `Combat Seeder` message-style choice from `SeederChoice` instead of re-asking once combat setup has already been chosen

This matters because the runtime does not display seeded messages verbatim in every case. The current engine assembles many exchanges from multiple seeded fragments:

- melee and natural attacks are combined with seeded defense and hit-bodypart text
- natural attacks still rely on seed strings honouring `{0}` bodypart formatting and `@hand` replacement
- many ranged, skirmish, aim, and movement-fire messages are effectively standalone strings and therefore need their own terminal punctuation

No runtime combat-message contract changes were required in this pass. Compatibility is still defined by the current runtime assemblers in `CombatMessage`, `CombatMessageManager`, and the concrete move classes.

### Stock weapon-suite intent
The stock live weapon suites are now seeded with clearer chassis expectations:

- knives and daggers stay fast, cheap, and strongest in clinch work
- swords stay broad and versatile, with longswords and two-handers covering more control and finishing cases than short blades
- rapiers stay narrow, thrust-centric, and efficient rather than broadly dominant
- axes, clubs, maces, warhammers, mattocks, and improvised bludgeons stay heavier, more punishing, and less efficient in bad positions
- spears and halberds stay reach and control focused
- shields stay situational and defensive, with limited but real offensive coverage

Training weapons remain intentionally weaker mirrors of the live suites. In stock seeding that now means lower damage bands and no extra move-category breadth over the corresponding live family, not a promise that every training motion is less accurate in isolation.

### Damage-band calibration
The stock weapon damage tiers are still seeded as shared expressions rather than per-attack bespoke formulas. Their current calibration is intentionally anchored to the stock human severity thresholds.

Current profile behavior:

- `stock` keeps the existing static / partial / random damage-choice flow and seeds the older shared weapon and unarmed formulas
- `combat-rebalance` suppresses that randomness question and rewrites the same named shared damage expressions with lower deterministic coefficients calibrated around the rebalance human anatomy tables
- neither profile changes the stock attack catalogue or combat-message catalogue identity

The stock-path calibration remains intentionally anchored to the stock human severity thresholds:

- a standard-quality weapon in static mode with nominal strength keeps `Terrible` and `Bad` hits below the severe bands
- `Poor` and `Normal` hits land in the ordinary combat wound bands
- `Good` and `Great` hits move into heavier but still non-exceptional wound territory
- `Coup de Grace` damage stays in a distinctly exceptional lethality band

That keeps ordinary exchanges compatible with the stock bodypart and health-severity expectations without flattening the distinction between routine hits and deliberate finishers.

## Animal Seeder Coverage
### Verified current state
`AnimalSeeder` now uses a typed template catalogue rather than relying on a single large switch-heavy file.

The stock animal seeding path is now organized around:

- family-focused partial files for mammals, birds, serpents, aquatic animals, insects, arachnids, and reptile-amphibian bodies
- reusable template data for race definitions, attack loadouts, venom profiles, height and weight models, description packs, and anatomy audit profiles
- a smaller core builder layer that still handles EF creation of bodies, organs, bones, attacks, liquids, and drugs
- race-specific ethnicity-dependent description progs are generated only after the owning race and ethnicities have been saved, so seeded progs always reference persistent IDs
- all three stock non-human health strategies are now seeded every time (`Non-Human HP`, `Non-Human HP Plus`, and `Non-Human Full Model`), while the selected answer only controls which one becomes the default for races created in that run
- the seeder now also reuses the shared `combat-balance-profile` answer from `HumanSeeder`, so stock versus combat-rebalance bodypart HP, hit chances, natural armour, and damage expressions are chosen without another prompt

### Stock body and race coverage
The stock seeder now covers a broader set of body families than the earlier animal pass.

Current seeded morphology groups include:

- hoofed and toed quadruped mammals
- expanded stock hoofed and small-mammal families such as camels, elk, reindeer, stoats, polecats, minks, and shrews
- avians
- serpents
- fish, sharks, pinnipeds, and cetaceans
- decapod crabs plus lobster and shrimp style malacostracans
- cephalopods and jellyfish
- winged and non-winged insects
- arachnids and scorpions
- starter reptilian and anuran bodies for lizards, geckos, skinks, iguanas, monitor lizards, chelonians, crocodilians, frogs, and toads

### Stock prose coverage
The stock animal catalogue now seeds richer presentation prose alongside its anatomical content.

Current prose expectations include:

- every stock animal race building a three-paragraph long description instead of a single flat summary
- every seeded stock ethnicity and the seeded `Animal` culture using the same three-paragraph minimum
- race prose separating physical profile, behaviour or ecological role, and the way people usually interpret or use the species
- stock ethnicity blurbs for dog breeds, bear lines, and default/common animal lines now reading as distinct lineage descriptions instead of near-duplicate parent-race text
- stock short and full entity-description patterns drawing from richer adult, juvenile, and baby packs rather than falling back to minimal `a <race>` style placeholders

### Anatomy validation
Animal anatomy seeding now uses family-specific audit profiles rather than one implicit mammal-centric assumption.

This matters because:

- endoskeletal families explicitly require key bones, organs, limbs, and bodyparts
- exoskeletal or part-light families can intentionally omit bones without failing validation
- new arthropod, crustacean, reptilian, and amphibian bodies can be checked for completeness using rules appropriate to their morphology
- the stock serpentine body now seeds explicit skull and vertebra bones plus a distinct tail limb so the serpent audit matches live runtime anatomy
- anatomy audits now tolerate duplicate aliases across a `CountsAs` chain by preferring the most specific body definition, which is required for layered aquatic crustacean bodies
- the stock piscine body now seeds its axial bones explicitly, and fish audits treat gills as gill bodyparts rather than internal organs to match the runtime anatomy model
- the stock decapod and malacostracan bodies now do the same for their gill clusters, and both stock crustacean layouts seed an explicit soft underbelly bodypart for ventral targeting
- layered body clones now also carry forward limb memberships from inherited `CountsAs` limbs, which keeps reptilian and anuran paw or claw additions attached to the correct foreleg and hindleg limbs at runtime

### Venom and attack seeding
Animal venoms are now seeded as reusable drug-backed venom profiles with per-profile effect rows.

The stock catalogue now includes reusable profiles for:

- `Neurotoxic`
- `Hemotoxic`
- `Cytotoxic`
- `Irritant`
- `Mixed`

Those venoms are attached to stock venomous animals through dedicated natural weapon attacks, and all stock races now seed at least one natural attack loadout rather than leaving passive or aquatic species with no attack entries.

Animal combat seeding is also more explicit about loadout role and message variety than the earlier stock catalogue.

Current stock expectations include:

- predator and pack-hunter templates carrying downed-bite coverage where that morphology makes sense
- hoofed herbivores and charge animals carrying hoof stomp coverage instead of relying only on barge or gore attacks
- aquatic, avian, arthropod, reptile, and arachnid families using family-specific prose rather than near-duplicate bite text
- alias-driven natural attacks validating both attack keys and plausible bodypart aliases for the intended morphology
- every stock animal loadout now exposing at least one clinch-usable natural attack and at least one non-clinch natural attack, with template and unit-test coverage plus generic fallback bashes, clamps, pecks, or lashes where a family previously only covered one side

Venom attacks are now kept more clearly separate from pure damage moves. The stock jellyfish, serpent, spider, insect, and scorpion venom attacks use moderate direct damage, family-appropriate prose, and smaller quantity or wound-severity gates than a raw lethality-only profile would imply.

## Mythical Animal Seeder Coverage
### Verified current state
`MythicalAnimalSeeder` is now the stock package for mythic creatures and also absorbs the legacy fantasy-beast content that used to live in the old `FantasySeeder`.

The seeder currently:

- reuses the shared non-human seeder questionnaire so builders answer the same health-model and combat-message prompts they already see for stock animals, while reading the already-recorded combat-balance profile from `HumanSeeder` instead of re-asking it
- requires the human and animal body and characteristic infrastructure before installation, so mythic races inherit compatible corpse models, breathing setup, attacks, and body semantics
- resolves its selected default non-human strategy through the same canonical strategy-name mapping used by `AnimalSeeder`, so the stock animal and mythic packages no longer drift on `HP Plus` versus `Full Model` naming
- now includes the older fantasy-only races such as eastern dragons, pegacorns, myconids, plantfolk, ents, dryads, giant arthropods, and giant worm-beasts, so the separate `FantasySeeder` is no longer needed
- installs incrementally, skipping any already-present mythic race entries instead of treating partial overlap as a fatal blocker
- resolves duplicate bodypart aliases deterministically while composing hybrid bodies or reusing partially seeded bodies, so reruns no longer fail while building limb and parent-part lookups
- seeds at least one clinch-capable and one non-clinch natural attack for every mythic race, including previously attackless sapient forms such as myconids, plantfolk, owlkin, and avian people, with template and unit-test coverage rather than live seeder hard-fails
- reruns refresh stock mythic combat tuning in place from the humanoid or animal reference bodies rather than seeding duplicate race copies for the rebalance profile

### Body reuse strategy
The package prefers to reuse existing stock bodies wherever that does not require a major compromise.

Current reuse patterns include:

- direct reuse of stock `Organic Humanoid`, `Ungulate`, `Toed Quadruped`, `Avian`, and `Serpentine` bodies for races that fit those shapes closely
- direct reuse of the stock `Insectoid`, `Arachnid`, and `Scorpion` bodies for giant ants, giant spiders, giant scorpions, and similar arthropod myths
- direct reuse of the stock `Vermiform` body for giant and colossal worm-beasts
- dedicated hybrid bodies for centaurs, merfolk, naga, griffins, hippogriffs, manticores, wyverns, hippocamps, and winged or horned humanoids
- humanoid-form hybrids now keep `CountsAs` links back to `Organic Humanoid` and layer only their extra anatomy plus explicit inherited-part removals, so stock humanoid wear profiles and bodypart group describers continue to apply without duplicating the humanoid catalogue per variant
- direct bodypart-group describers now match inherited override parts via `CountsAs`, which keeps humanoid descriptive grouping intact even when a child body swaps in robot or mythic override parts
- humanoid-parent races for minotaurs, naga, merfolk, selkies, dryads, owlkin, avian people, and centaurs so they can leverage the same characteristic and description ecosystem as humans

### Variation and chargen implications
The humanoid-form mythic races now seed an ethnicity with human-style characteristic coverage rather than only a flat animal description.

That includes reuse of stock profiles such as:

- eye colours
- eye shapes
- noses
- ears
- hair colours
- hair styles
- skin colours
- frames
- race-specific person-word profiles

Humanoid-form mythic races now also keep race-specific stock overlay description variants. That means centaurs, naga, merfolk, selkies, dryads, owlkin, avian people, and similar hybrids still benefit from the broader humanoid characteristic system, but their seeded short and full descriptions now explicitly foreground the mythic anatomy of the race instead of reading like generic organic-humanoid text.

## Robot Seeder Coverage
### Verified current state
`RobotSeeder` is now the stock package for robotic bodies, robotic races, and robot-specific medical content.

The seeder currently:

- installs as a rerunnable package rather than extending the one-shot `HealthSeeder`
- requires the stock humanoid, toed quadruped, arachnid, and insectoid body infrastructure before installation, while avian-backed robot variants are now optional
- seeds robot-specific liquids, materials, armour types, corpse models, a stamina-recovery prog, robot knowledges, and robot-only procedures
- seeds both a sentient articulated robot strategy based on `RobotHealthStrategy` and a utility-construct strategy based on `BrainConstructHealthStrategy`
- seeds all stock attribute definitions onto robot races instead of relying on inherited human race rows being populated in the current EF context
- seeds a selectable `Robot` culture backed by the stock `Simple` name culture and robot-themed random-name profiles
- constrains all robot races to neuter-only except for the cyborg/mechanical-human line, which continues to inherit the human gender matrix
- keeps every robot race on at least one clinch-usable and one non-clinch natural attack through template and unit-test coverage, and corrects utility-frame attack clones that were previously copied from non-combat smash-only donors
- skips already-present bodies, races, strategies, and procedures so the package can be safely re-run on worlds that already imported part of the catalogue
- reads the shared `combat-balance-profile` answer and uses it to choose stock versus combat-rebalance bodypart durability, hit chances, sever formulas, and robot natural-armour definitions without asking another combat-balance question

### Body reuse strategy
The package prefers to reuse existing stock body semantics wherever that preserves compatibility.

Current reuse patterns include:

- non-humanoid robot frames still use self-contained clone bodies where wear-profile reuse is irrelevant
- humanoid robot chassis now keep `CountsAs(Humanoid)` inheritance for wear-profile and bodypart-group compatibility, with the articulated robot shell provided by child-body overrides and explicit inherited-part removals rather than flattening away the humanoid parent link
- inherited humanoid robot overrides therefore keep stock humanoid wear profiles and direct bodypart-group describers usable without seeding duplicate copies of those definitions per chassis
- dedicated humanoid-derived variants for spider crawler, circular-saw hands, pneumatic-hammer hands, sword hands, winged frames, jet frames, mandible heads, wheels, tracks, and cyborgs
- winged and jet frames are now soft dependencies on the stock `Avian` body and are skipped rather than blocking the entire robot package when avian anatomy is absent
- reuse of stock `Toed Quadruped`, `Arachnid`, and `Insectoid` source anatomies for robot dog, spider-crawler lower bodies, and robot cockroach content
- `CountsAs` mappings on robot override or substituted bodyparts so surgery, wear, and other anatomy-aware logic can continue to target the intended baseline chassis
- explicit limb-membership mapping for grafted mandibles and substituted wheel or track assemblies so derived robot frames still initialise cleanly on a fresh install

### Stock robot catalogue
The seeded robot race catalogue currently includes:

- articulated humanoid robots
- humanoid spider-crawler, circular-saw, pneumatic-hammer, sword-hand, winged, jet, mandible, wheeled, tracked, and cyborg variants
- a roomba-style utility robot
- a small tracked utility robot
- a robot dog
- a robot cockroach

The stock robot line also seeds robot-appropriate internals and body defaults:

- positronic brains
- power cores
- speech synthesizers where the chassis is meant to vocalise
- sensor arrays for robot perception
- hydraulic fluid or machine oil as the circulatory liquid
- no sweat, no breathing, and high-stamina defaults through the seeded races and bodies

### Stock prose coverage
Robot seeding now treats descriptive prose as first-class stock data rather than a thin afterthought.

Current prose expectations include:

- every robot race, seeded robot ethnicity, and the seeded `Robot` culture now building a three-paragraph long description
- non-cyborg chassis using multiple stock short and full description variants keyed to chassis identity and role instead of a single minimal stock pattern
- the playable `Cyborg` line continuing to use the humanoid characteristic ecosystem while layering race-specific overlay variants that emphasise the synthetic shell and uneasy human-passing presentation
- representative unit-test coverage for a standard chassis, a utility chassis, and the cyborg overlay path

### Stock robot procedures
`RobotSeeder` now seeds a robot maintenance suite parallel to the organic surgery catalogue.

The current stock robotics procedures include:

- diagnostics
- maintenance examination
- exploratory maintenance
- leak control
- chassis closure
- robot organ extraction
- robot organ replacement
- limb detachment
- limb reattachment

Those procedures target the stock robot base bodies, and the runtime `CountsAs` checks now allow derived wheel, track, and similar variants to match the intended maintenance procedures without duplicating every definition per variant.

## Dedicated Health Seeder State
### Verified current state
`HealthSeeder` is currently enabled.

That matters because it means:

- the repo automatically offers a stock surgery framework for the selected medical tech level
- the stock repo now ships a broader default drug catalogue instead of only two example drugs
- the seeder can optionally install basic veterinary content for stock mammal bodies alongside the human set
- the seeder now reruns conservatively: same-tech reruns repair stock procedures, phases, knowledges, and drugs in place, while higher-tech reruns add or refresh higher-tech stock content without deleting lower-tech content
- human surgery target definitions now resolve external limb aliases through the `Organic Humanoid` to `Humanoid` `CountsAs` chain, while organ-targeted procedures still resolve directly on `Organic Humanoid`
- the stock human leg amputation definitions now target the current humanoid lower-leg aliases (`lshin` and `rshin`) rather than the older `llowerleg` and `rlowerleg` names
- core race and item health defaults are still assembled across multiple seeders rather than only here

## Tech-Level Surgery Matrix
The enabled `HealthSeeder` provides the clearest picture of the stock surgical content model.

| Tech level | Seeded knowledges | Seeded procedures |
| --- | --- | --- |
| Primitive | `Medicine` plus optional `Animal Medicine` | Human: `Hasty Triage`, `Crude Physical`, `Primitive Stitching`, `Exploratory Surgery`, `Arm Amputation`, `Leg Amputation`, `Digit Amputation`, `Trauma Control`, `Organ Extraction`, `Crude Organ Repair`, `Trepanation`, `Windpipe Repair`, `Bone Setting`. Veterinary: `Veterinary Hasty Triage`, `Veterinary Crude Physical`, `Veterinary Stitching`, `Veterinary Exploratory Surgery`, `Veterinary Trauma Control`, `Veterinary Bone Setting`, `Foreleg Amputation`, `Hindleg Amputation` |
| Pre-modern | `Chiurgery`, `Physical Medicine` plus optional `Veterinary Medicine` and `Veterinary Chiurgery` | Human: `Hasty Triage`, `Triage`, `Crude Physical`, `Stitch Up`, `Exploratory Surgery`, `Arm Amputation`, `Leg Amputation`, `Digit Amputation`, `Trauma Control`, `Organ Extraction`, `General Organ Repair`, `Trepanation`, `Cardiac Repair`, `Liver Resection`, `Splenic Repair`, `Gastric Repair`, `Intestinal Resection`, `Kidney Repair`, `Lung Resection`, `Tracheal Repair`, `Esophageal Repair`, `Spinal Stabilisation`, `Inner Ear Repair`, `Bone Setting`. Veterinary: `Veterinary Hasty Triage`, `Veterinary Triage`, `Veterinary Physical`, `Veterinary Stitch Up`, `Veterinary Exploratory Surgery`, `Veterinary Trauma Control`, `Veterinary Bone Setting`, `Foreleg Amputation`, `Hindleg Amputation` |
| Modern | `Diagnostic Medicine`, `Clinical Medicine`, `Surgery` plus optional `Veterinary Medicine` and `Veterinary Surgery` | Human: `Hasty Triage`, `Triage`, `Crude Physical`, `Physical`, `Stitch Up`, `Exploratory Surgery`, `Arm Amputation`, `Leg Amputation`, `Digit Amputation`, `Replantation`, `Cannulation`, `Decannulation`, `Trauma Control`, `Organ Extraction`, `Organ Transplant`, `General Organ Repair`, `Brain Surgery`, `Cardiac Repair`, `Liver Resection`, `Splenic Repair`, `Gastric Repair`, `Intestinal Resection`, `Kidney Repair`, `Lung Resection`, `Tracheal Repair`, `Esophageal Repair`, `Spinal Stabilisation`, `Inner Ear Repair`, `Bone Setting`, `Install Implant`, `Remove Implant`, `Configure Implant Power`, `Configure Implant Interface`. Veterinary: `Veterinary Hasty Triage`, `Veterinary Triage`, `Veterinary Physical`, `Veterinary Stitch Up`, `Veterinary Exploratory Surgery`, `Veterinary Trauma Control`, `Veterinary Bone Setting`, `Foreleg Amputation`, `Hindleg Amputation` |

### Seeder intent
The prompt text in `HealthSeeder` makes the intended capability progression explicit:

- primitive: no replantation, transplantation, resection, or implants
- pre-modern: no replantation, transplantation, or implants
- modern: full procedure set

## Seeded Medical Items and Drugs
### Dedicated health seeder
`HealthSeeder` now seeds a tech-level specific drug catalogue rather than only two examples.

Primitive examples include:

- `Willow Bark Tea`
- `Mandrake Draught`
- `Honey Poultice`
- `Garlic Salve`
- `Mint Infusion`
- `Ephedra Brew`
- `Foxglove Tincture`

Pre-modern examples include:

- `Laudanum`
- `Ether Anaesthetic`
- `Mould Poultice`
- `Distilled Antiseptic`
- `Mint and Ginger Tonic`
- `Digitalis Tincture`
- `Curare Paste`
- `Herbal Burn Salve`
- `Bronchial Smoke`

Modern examples include:

- `General Anaesthetic`
- `Opioid Analgesic`
- `Muscle Relaxant`
- `Local Anaesthetic`
- `Broad-Spectrum Antibiotic`
- `Antifungal Course`
- `Antiemetic`
- `Immunosuppressant`
- `Adrenaline Shot`
- `Bronchodilator`
- `Cardiac Support Agent`
- `Healing Accelerant`
- `Antipyretic`
- `Overdose Antagonist`

These stock drugs now exercise a much larger share of the runtime drug framework, including analgesia, anesthesia, antibiotics, antifungals, nausea control, paralysis, adrenaline, stamina support, organ support, healing-rate modifiers, immunosuppression, and drug neutralisation.

This is much broader than the old stock seeding, but it is still narrower than the full runtime drug framework.

### Item seeders
The item seeders cover a broad low-tech treatment layer.

Representative seeded content includes:

- bandages
- splints
- crutches
- tourniquets
- prosthetics such as peg legs, hook hands, replacement arms, legs, feet, eyes, and hands
- suturing kits and needles
- forceps and related minor surgical tools
- cannula items

`UsefulSeeder` also seeds treatment component prototypes and support content such as:

- `Bandage_Simple`, `Bandage_Good`, `Bandage_Great`
- cannula component prototypes
- IV-related component content
- a large prosthetic component set

The current `HealthSeeder` inventory plans are intentionally limited to tags that are already seeded for stock play and are cross-checked before installation:

- `Scalpel`
- `Bonesaw`
- `Forceps`
- `Arterial Clamp`
- `Surgical Suture Needle`

Diagnostic and physical-examination procedures are therefore intentionally tool-light in stock seeding rather than depending on a larger clinical kit that may not exist in base content.

### Skills and checks
The stock seeding for skills and packages includes many health-relevant checks, including:

- `SutureWoundCheck`
- `CleanWoundCheck`
- `MedicalExaminationCheck`
- `TriageCheck`
- `TraumaControlCheck`
- `PerformCPR`
- `Defibrillate`
- `OrganExtractionCheck`
- `OrganTransplantCheck`
- `CannulationProcedure`
- `DecannulationProcedure`
- `OrganStabilisationCheck`
- `TendWoundCheck`
- `RelocateBoneCheck`
- `SurgicalSetCheck`

That means the repo already treats medical play as skill-gated, even before world builders customize it.

## Runtime Option Matrix
The following table separates runtime capability from stock seeding.

| Runtime option | Stock seeding status | Note |
| --- | --- | --- |
| `BrainHitpointsStrategy` | Seeded | Used in human and animal seeders |
| `ComplexLivingHealthStrategy` | Seeded | Used in human and animal seeders |
| `GameItemHealthStrategy` | Seeded | Seeded in `CoreDataSeeder` |
| `SimpleLivingHealthStrategy` | Not stock seeded | Runtime class exists |
| `ConstructHealthStrategy` | Not stock seeded | Runtime class exists |
| `BrainConstructHealthStrategy` | Seeded | Used by the `Robot Utility Construct` stock strategy |
| `RobotHealthStrategy` | Seeded | Used by the articulated robot stock strategy installed by `RobotSeeder` |
| `Simple` infection | Runtime implemented | Reached through organic wound logic rather than a named stock seeding block |
| `Gangrene` infection | Runtime implemented | Reached through infection progression rather than dedicated stock content |
| `Necrotic`, `Infectious`, `FungalGrowth` infection types | Runtime implemented | Concrete infection classes now exist, but stock seeding still does little to showcase them |
| `StandardCorpseModel` | Seeded | Human and animal seeders create standard corpse models |
| `NonDecayingCorpseModel` | Not broadly stock seeded | Runtime support exists |
| `NoNeeds`, `Passive`, `Active` needs models | Runtime supported | `NoNeeds` is the most explicit stock default in current seeding paths |
| `Lung`, `Gill`, `Blowhole`, `NonBreather`, `Partless` breathing strategies | Runtime supported | Animal seeding demonstrates multiple breathing assignments |
| Broad drug effect families | Mostly unseeded | Runtime drug system is much richer than stock seeded drugs |
| Defibrillator items | Not found in broad stock item seeding search | Runtime support exists through `IDefibrillator` |
| Rebreathers and breathing filters | Not found in broad stock item seeding search | Runtime support exists through component types |
| External organs and advanced implants | Not found in broad stock item seeding search | Runtime support exists through component types and surgery classes |

## Verified Partial or Inactive Areas
The following findings are directly verified in code and matter when describing the current state of the system.

### Runtime broader than implementation
- `DrugType` still exposes more effect families than the stock seeded drugs use, but the gap is much smaller than before.
- Multiple health strategy families exist in runtime without matching stock seeded strategy definitions.
- Seeded strategy XML still only emits the older minimal field set; newer optional strategy tuning properties now load with fallback defaults when omitted.

### Recently resolved runtime gaps
- `LungBreather` now integrates stop-breathing effects and non-rebreather `IProvideGasForBreathing` sources.
- `Infection` now persists virulence multiplier data instead of dropping it on load or save.
- `SimpleInfection` now applies a concrete nausea effect.
- `HealthModule.relocate` now resolves fractures from targeted external bodyparts rather than only direct bone targets.
- The old soft-coded tending helper path has been removed in favor of the effect-driven wound-care flow.
- The player-facing `surgery show` path now exposes richer procedure detail.
- `SimpleOrganicWound` and `BoneFracture` both support anti-inflammatory treatment through bodypart-scoped pain-reduction effects.
- `ConfigureImplantInterfaceProcedure` now reports the correct procedure enum.
- `SurgicalProcedure` now enforces `TargetBodyType` when checking whether a procedure can be started, so human and quadruped surgical content can coexist safely.
- Robot bodies can now be stock seeded rather than existing only as a runtime capability, and robot surgery targeting now honours `CountsAs` mappings for derived chassis parts.
- `SensorArray` is now a first-class organ type in the runtime and the stock robot line uses it for robot perception while retaining eyes and ears as compatible external parts.
- `RobotHealthStrategy` bleed and prompt text now use the configured robot circulatory liquid name instead of assuming hydraulic fluid in all cases.
- Stock animal, mythic, and robot catalogues now seed richer three-paragraph race and ethnicity prose, while mythic humanoids and cyborgs use race-specific overlay variants so humanoid description systems no longer collapse them into generic organic presentation.

## Supported but Unseeded
These are verified runtime features that exist in the codebase but are not broadly reflected in the stock seeding.

- advanced health strategy families for constructs and simpler living models
- expanded strategy tuning XML for health thresholds, bleed handling, hypoxia, blood recovery, kidney waste, and spleen cleanup
- non-decaying corpse models
- high-tech medical items such as defibrillators, rebreathers, breathing filters, external organs, and complex implants
- some remaining drug effect families such as explicit rage or magic-oriented stock examples
- implant installation, removal, and configuration surgery content without matching broad stock implant item content

## Logical Extension Candidates
The following are inference-based extension paths. They are not claims about current behavior; they are the most natural next steps suggested by the current design.

- extend the stock drug catalogue into more niche effect families such as rage, vision control, and magic-facing substances where a game setting wants them
- add broad stock content for defibrillators, rebreathers, external organs, and implant power or interface ecosystems
- formalize flaw-style health templates if builders want negative baseline traits distinct from merits

## Main Takeaways
The current stock repo has a rich health runtime but a split and uneven stock setup.

The most important practical consequences are:

- a lot of effective health setup lives outside `HealthSeeder`
- the stock repo now includes a release-ready medical seeder with tech-level surgery, drugs, and basic mammal veterinary support
- the stock repo also now includes a rerunnable robot package that seeds robot chassis, robot races, robot health strategies, and robot maintenance procedures
- low-tech treatment and prosthetic play are well represented in seeded items
- higher-tech medical play is mostly a runtime capability awaiting fuller stock content
