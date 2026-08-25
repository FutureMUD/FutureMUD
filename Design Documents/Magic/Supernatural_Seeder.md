# Supernatural Seeder

## Current Implementation

The Supernatural Seeder installs a builder-facing stock catalogue of angels, fallen angels, demons, gods, spirits, ghosts, werewolves, and mechanically supported undead. It is idempotent and can be rerun to restore missing stock supernatural races, cultures, name cultures, attacks, body prototypes, form merits, corpse models, description patterns, and non-breather settings.

All seeded supernatural races are unavailable to normal chargen by default. Builders can enable them through the usual chargen, role, merit, or staff workflows after deciding how supernatural characters should fit their game.

The angelic catalogue follows Maimonides' ten ranks: Chayot HaKodesh, Ophanim, Erelim, Hashmallim, Seraphim, Malakhim, Elohim, Bene Elohim, Cherubim, and Ishim. The demon catalogue mirrors those fallen ranks and adds common stock demons such as incubus, succubus, fury, imp, familiar, fiend, and hellhound.

## Mechanics Seeded

The seeder uses existing FutureMUD systems rather than adding a new supernatural runtime model:

- Race records carry anatomy, health strategy, communication model, natural attacks, breathing and needs configuration, chargen availability, attributes, ethnicity, and description variables.
- The natural attack catalogue contains forty stock supernatural attacks. These use existing combat move types for sonic screeches, ranged natural attacks, spitting, breath weapons, trips, staggering blows, pushback, clinches, forced movement, and wing buffeting.
- Angelic sonic attacks such as `Heavenly Choir`, `Canticle of Awe`, `Trumpet Peal`, and `Word of Command` use the existing area-style `ScreechAttack` mechanic, target ear-shaped bodyparts, and are written as choir or command-voice effects rather than single-target strikes.
- Demonic, spirit, undead, and therianthrope attacks are cloned from existing animal or unarmed donor attacks so builders get varied examples without the seeder adding new combat engine mechanics.
- Body prototypes carry the base planar presence XML for supernatural forms such as incorporeal spirits, dual-natured angels, astral demons, and ordinary material werewolves or undead.
- Horned fiend and familiar supernatural bodies add stock tail aliases so tail attacks such as `Barbed Tail Slap` have real bodyparts to bind to. The tail subtree is cloned from `Quadruped Base`, where the stock quadruped tail aliases are directly authored, rather than from the inherited `Toed Quadruped` child body.
- Additional body forms are supplied as stock `Additional Body Form` merits. These are examples and builder tools, not automatic race-level transformations.
- Spirits, ghosts, angels, demons, gods, and undead use explicit non-breather settings with hunger and thirst rates set to zero.
- Werewolves use living needs and seeded alternate-form merits for hybrid and wolf-form examples.
- Physical undead use a non-decaying corpse model; spirit-like beings use a non-decaying dissipating-spirit corpse model.

## Combat balance ownership

All 46 stock races carry complete combat metadata: tier, morphology baseline, pain tolerance, natural-armour quality, attack-profile family, charge eligibility and signature action. The seeder reconciles race health multiplier, tolerance, attributes and dice, strategy, natural armour and natural-attack links on every rerun. Builder-created attack clones and unrelated custom links are not part of that owned set.

Stock supernatural attack records are reconciled rather than only created. The owned fields include move type, all check difficulties, recovery, expressions, damage type, intentions, delay, stamina, weighting, target count, bodypart shape, payload, range data and combat message. Physical claws, bites, impacts and mauls use owned Strength expressions. Radiant and infernal energy use quadratic Aura expressions; sonic, command, spirit and undead force use quadratic Willpower expressions. The owned attacks have explicit accuracy, defence difficulty and high selection weights rather than retaining stale animal-donor balance. Breath and sonic attacks are deliberately slower and can affect up to four total targets through their existing runtime payloads.

If a world's attribute package has no `Aura`, `Luck` or `Spirit` attribute, Aura-family expressions bind to Willpower rather than creating a new core attribute during a catalogue rerun.

The tiers are benchmark roles, not setting power ranks: nuisance (`Imp`, `Familiar`), minor, serious, elite, monster, great beast, party boss, and avatar. `Balrog`, `Chayot HaKodesh`, `Fallen Chayot HaKodesh` and `Lesser God` are party bosses. `Greater God` is an embodied, defeatable avatar. Higher angelic, infernal and divine humanoids gain most of their threat from Aura- or Will-led signature actions rather than behemoth-level physical Strength. Only sufficiently large, corporeal bodies receive `Behemoth Charge` links.

Custom body refreshes preserve unique anatomy but reconcile the combat fields of cloned reference parts. Organs remain internal with no armour or direct hit weighting; bones retain bone armour and cannot become externally weighted flesh on a rerun.

## Builder Workflow

Builders normally use the seeded races as templates:

1. Run the prerequisite Human, Animal, Mythical Animal, Combat, Health, Culture, and Stock Merit support packages.
2. Run the Supernatural Seeder.
3. Clone or edit the stock cultures, ethnicities, description patterns, and name cultures for the world's cosmology.
4. Attach the seeded `Additional Body Form` merits through chargen roles, curses, staff grants, NPC templates, or custom FutureProgs.
5. Enable individual supernatural races in chargen only when the world is ready for player-facing supernatural play.

For physical combat testing of `Spirit`, `Ghost`, `Ancestral Spirit`, `Nature Spirit`, `Elemental Spirit`, `Specter` or `Wraith`, retain one incorporeal control and make a separate manifested fixture with `corporeality set <target> corporeal Prime`. Do not count the incorporeal control toward physical win-rate bands.

The seeded form merits deliberately do not force full-moon or cosmology-specific behavior. Builders can add condition progs and auto-transform settings when their setting defines those rules.

## Boundaries and Future Work

The seeder includes undead only where current mechanics support them as races or body forms. It does not implement post-death ghost creation, possession, remote corpse vessels, vampire feeding, lich phylacteries, automatic werewolf lunar transformation, divine worship economies, new combat engine move types, or a new race-owned multi-form model.

Those behaviours should be implemented as future runtime features using the existing body-form, merit, effect, FutureProg, plane, needs, and health systems as integration points.
