#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private static IEnumerable<AnimalRaceTemplate> GetAquaticRaceTemplates()
    {
        static AnimalRaceTemplate Aquatic(
            string name,
            string adjective,
            string bodyKey,
            SizeCategory size,
            double health,
            string model,
            string loadout,
            AnimalDescriptionPack pack,
            AnimalBreathingMode breathingMode,
            bool canClimb = false,
            string combatStrategyKey = "Beast Brawler",
            string? description = null)
        {
            return new AnimalRaceTemplate(
                name,
                adjective,
                description ?? throw new InvalidOperationException($"Animal race {name} is missing a seeded description."),
                bodyKey,
                size,
                canClimb,
                health,
                model,
                model,
                "aquatic-bird-fish",
                loadout,
                pack,
                false,
                breathingMode,
                null,
                bodyKey switch
                {
                    "Piscine" => "fish",
                    "Decapod" => "decapod",
                    "Malacostracan" => "malacostracan",
                    "Cephalopod" => "cephalopod",
                    "Jellyfish" => "jellyfish",
                    "Pinniped" => "pinniped",
                    "Cetacean" => "cetacean",
                    _ => null
                },
                null,
                combatStrategyKey
            );
        }

        yield return Aquatic("Carp", "Carp", "Piscine", SizeCategory.Small, 0.2, "Small Fish", "fish",
            AquaticPack("a carp fry", "a young carp", "a carp",
                "It is broad-backed and thick-scaled, with a blunt head and deep body.",
                "Its heavy flanks and slow powerful tailbeats suit muddy ponds and quiet rivers.",
                "It looks durable rather than fast.",
                "pond, river and silted water"),
            AnimalBreathingMode.Freshwater,
            description: """
            Carp are sturdy freshwater fish of ponds, canals and silted rivers, more often noticed by a slow bronze roll beneath the surface than by any dramatic strike. They make quiet water feel occupied and patient.

            A carp's deep body, blunt head and heavy scales suit muddy bottoms, weed beds and the lazy search for food among stirred sediment. It is not built to dazzle with speed; it survives by endurance, wariness and an ability to prosper where the water is far from clear.

            People meet carp as food, ornament, pond life and familiar river stock. Their value is often domestic and practical, but a large carp moving under still water can still carry the old mystery of things living just out of reach.
            """);
        yield return Aquatic("Cod", "Cod", "Piscine", SizeCategory.Small, 0.2, "Medium Fish", "fish",
            AquaticPack("a cod fry", "a young cod", "a cod",
                "It is pale-sided and thick-bodied, with a broad head and tapering tail.",
                "Its body is that of a practical cold-water hunter built to cruise and gulp.",
                "It hangs in the water with patient, practical intent.",
                "cold sea and offshore bank"),
            AnimalBreathingMode.Saltwater,
            description: """
            Cod are practical cold-sea fish of offshore banks and working waters. They belong less to glittering shoals near the surface than to the steady, edible abundance that draws boats into hard weather and long labour.

            The broad head, pale flanks and tapering body make a cod look built to cruise, gulp and endure. It has the plain efficiency of a bottom-haunting hunter rather than the nervous flash of smaller schooling fish.

            Around coastal communities, cod are livelihood before spectacle: catch, ration, trade good and reason to sail. Their presence ties deep water to kitchens, markets and the risks people accept to bring food ashore.
            """);
        yield return Aquatic("Haddock", "Haddock", "Piscine", SizeCategory.Small, 0.2, "Medium Fish", "fish",
            AquaticPack("a haddock fry", "a young haddock", "a haddock",
                "It is neat-bodied and silvery, marked in darker mottling along the back.",
                "Its fin shape and taut body suit active swimming through open water.",
                "It looks alert and constantly in motion.",
                "cool sea and coastal shelf"),
            AnimalBreathingMode.Saltwater,
            description: """
            Haddock are cool-water sea fish of coastal shelves and offshore banks, neat-bodied and restless in the shifting light below the surface. They are recognisable as working-sea creatures rather than ornamental ones.

            Dark mottling along the back, pale sides and taut fins give a haddock an alert, clean-lined look. It seems made for active movement through cold water, feeding without the brute heaviness of cod or the flash of a surface shoal.

            For people, haddock are ordinary in the important way: food, trade and the daily proof that the sea supports settlement at a price. They add texture to fishing cultures without demanding legend around every net.
            """);
        yield return Aquatic("Koi", "Koi", "Piscine", SizeCategory.Small, 0.2, "Small Fish", "fish",
            AquaticPack("a koi fry", "a young koi", "a koi",
                "It is deep-bodied and ornamental, its scales often bright and richly patterned.",
                "Its rounded shape and bold colouration make it stand out more than most fish would dare.",
                "It drifts with serene, showy confidence.",
                "garden pond and still water"),
            AnimalBreathingMode.Freshwater,
            description: """
            Koi are ornamental carp whose colours turn still water into display. In a pond or garden pool they draw the eye deliberately, moving like living brushstrokes through reflection, weed and shadow.

            Their rounded bodies, broad fins and patterned scales make them poor candidates for invisibility and excellent signs of care. A koi does not need speed to command attention; it carries value in colour, size, calm and the way it changes the mood of water around it.

            People keep koi as symbols, luxuries, pets and cultivated beauties. They are domesticated spectacle rather than wild hazard, though their serenity depends on a maintained place where predators and hunger have been kept at a polite distance.
            """);
        yield return Aquatic("Pilchard", "Pilchard", "Piscine", SizeCategory.Small, 0.2, "Small Fish", "fish",
            AquaticPack("a pilchard fry", "a young pilchard", "a pilchard",
                "It is slim and silvery, built in the plain, efficient shape of a schooling fish.",
                "Its narrow body and shining flanks help it disappear among many of its own kind.",
                "It looks made for constant quick movement in company.",
                "coastal sea and schooling water"),
            AnimalBreathingMode.Saltwater,
            description: """
            Pilchards are small coastal schooling fish, silver-bodied and easily lost in the flashing agreement of many bodies turning together. A single pilchard is minor; a mass of them can make the sea appear to glitter with nerves.

            Their slim bodies and quick tails favour synchrony over individuality. They live by becoming difficult to single out, folding one bright movement into another until predator, fisher and current all read the shoal before the fish.

            They matter as bait, food and seasonal sign. Where pilchards run, larger mouths and human boats often follow, turning a modest fish into the centre of a wider coastal rhythm.
            """);
        yield return Aquatic("Perch", "Perch", "Piscine", SizeCategory.Small, 0.2, "Small Fish", "fish",
            AquaticPack("a perch fry", "a young perch", "a perch",
                "It is neatly built, with a spined dorsal fin and barred body.",
                "Its profile looks half predator, half ambusher, ideal for striking from reed shade.",
                "It waits more than it rushes.",
                "lake, river and weed bed"),
            AnimalBreathingMode.Freshwater,
            description: """
            Perch are freshwater fish of lakes, slow rivers and weed beds, marked by a watchful stillness that suits reed shade and broken light. They feel like fish of edges rather than open water.

            A spined dorsal fin, barred sides and compact predatory shape give a perch a prickly readiness. It waits well, holds position well and strikes from cover with more purpose than its size might suggest.

            People know perch as catch, pond predator and familiar sign of healthy freshwater. They are not grand fish, but they make small waters feel alive with ambush, appetite and quick local knowledge.
            """);
        yield return Aquatic("Herring", "Herring", "Piscine", SizeCategory.VerySmall, 0.1, "Small Fish", "fish",
            AquaticPack("a herring fry", "a young herring", "a herring",
                "It is narrow-bodied and silver bright, with a simple schooling silhouette.",
                "Its shape is all speed, synchrony and life spent among flashes of similar bodies.",
                "It looks nervous and constantly ready to turn as one with a shoal.",
                "open sea and cold coastal water"),
            AnimalBreathingMode.Saltwater,
            description: """
            Herring are narrow silver fish of cold coastal water and open sea, most impressive when they vanish into a shoal and return as a single shifting brightness. They are creatures of number, timing and collective motion.

            Their simple bodies are tuned for speed, turning and survival among countless near-identical neighbours. Light breaks across their flanks in brief flashes, making the individual hard to hold in the eye.

            They feed birds, larger fish, markets and whole fishing seasons. Herring are humble only one at a time; in abundance they become weather, economy and migration made edible.
            """);
        yield return Aquatic("Mackerel", "Mackerel", "Piscine", SizeCategory.VerySmall, 0.1, "Small Fish", "fish",
            AquaticPack("a mackerel fry", "a young mackerel", "a mackerel",
                "It is sleek and metallic, marked in dark bars over a silver body.",
                "Its torpedo shape and strong tail mark it as a relentless swimmer.",
                "It has the restless speed of something that never fully stops.",
                "open water and current"),
            AnimalBreathingMode.Saltwater,
            description: """
            Mackerel are fast open-water fish, metallic and restless, with dark bars over silver flanks that seem to retain the motion of current even when still. They belong to active water rather than mud or weed.

            The torpedo shape, strong tail and tight fins give a mackerel a sense of constant forward intent. It looks less like a creature that waits for opportunity than one that creates it by never quite stopping.

            For fishers and coastal tables, mackerel mean season, speed and oily abundance. Their arrival can feel sudden and generous, a bright run of life cutting through the sea before moving on.
            """);
        yield return Aquatic("Anchovy", "Anchovy", "Piscine", SizeCategory.VerySmall, 0.1, "Small Fish", "fish",
            AquaticPack("a anchovy fry", "a young anchovy", "a anchovy",
                "It is very small and slim, almost pure silver and motion.",
                "Its body is built around schooling, darting and avoiding bigger mouths.",
                "It seems little more than a quick bright line in the water.",
                "coastal water and estuary"),
            AnimalBreathingMode.Saltwater,
            description: """
            Anchovies are tiny silver fish of coastal waters and estuaries, quick enough to be glimpsed more as flicker than form. Their importance comes from scale of numbers rather than individual presence.

            Each anchovy is a narrow bright line built for schooling, darting and becoming hard to isolate. In motion they make the water seem to tremble, as if the shoal were one nervous surface with many small hearts.

            They are bait, food and foundation, feeding larger fish, birds and human appetites alike. An anchovy is easy to overlook until one follows the chain of hunger that depends on it.
            """);
        yield return Aquatic("Sardine", "Sardine", "Piscine", SizeCategory.VerySmall, 0.1, "Small Fish", "fish",
            AquaticPack("a sardine fry", "a young sardine", "a sardine",
                "It is compact and silver-scaled, with a schooling fish's simple efficient body.",
                "Its flanks catch light readily, vanishing and reappearing with every turn.",
                "It looks born to move in a living glittering mass.",
                "coastal sea and schooling water"),
            AnimalBreathingMode.Saltwater,
            description: """
            Sardines are compact schooling fish of coastal seas, plain at first glance but brilliant when a shoal turns in sunlight. They bring the open water's abundance close enough for nets, gulls and market baskets.

            Their small bodies favour tight coordination, quick changes of direction and the safety of many flanks catching light together. A sardine's life is individual only in the biological sense; visually and ecologically, it is part of a moving crowd.

            People meet sardines as food, bait and seasonal plenty. They carry the practical romance of fishing towns: salt, oil, silver scales and the knowledge that small fish can feed large communities.
            """);
        yield return Aquatic("Pollock", "Pollock", "Piscine", SizeCategory.Small, 0.2, "Medium Fish", "fish",
            AquaticPack("a pollock fry", "a young pollock", "a pollock",
                "It is long and pale, with a strong tail and deep mouth.",
                "Its build looks suited to active hunting in cool water rather than lurking on the bottom.",
                "It gives off a lean, hungry practicality.",
                "offshore water and cold sea"),
            AnimalBreathingMode.Saltwater,
            description: """
            Pollock are lean cold-water fish of offshore currents, less iconic than cod but shaped by the same practical sea. They carry a hungry, cruising quality that suits grey water and working boats.

            A long pale body, strong tail and deep mouth make a pollock look like an active hunter rather than a bottom-bound weight. It moves through cool water with efficient appetite, built to take advantage of whatever the current carries near.

            For people, pollock are catch, trade and provision: rarely glamorous, often necessary. They give the sea's labouring edge another familiar name in the net.
            """);
        yield return Aquatic("Salmon", "Salmon", "Piscine", SizeCategory.Small, 0.2, "Medium Fish", "fish",
            AquaticPack("a salmon fry", "a young salmon", "a salmon",
                "It is sleek and powerful, with clean fins and a muscular body.",
                "Its shape is that of a tireless swimmer made for long distances and hard currents.",
                "It looks as though it could climb water itself if it had to.",
                "river mouth and migrating current"),
            AnimalBreathingMode.Freshwater,
            description: """
            Salmon are powerful migrating fish, equally tied to river mouths, cold currents and the hard upstream work of return. Their lives make water feel like a road with memory.

            The clean fins, muscular body and determined head suggest endurance more than ornament. A salmon can be beautiful, but the deeper impression is purpose: silver strength driving through current, falls and exhaustion toward spawning ground.

            People read salmon as food, wealth, season and omen of river health. Where they run, settlements gather stories about return, plenty and the thin line between abundance and loss.
            """);
        yield return Aquatic("Tuna", "Tuna", "Piscine", SizeCategory.Normal, 0.8, "Large Fish", "fish",
            AquaticPack("a tuna fry", "a young tuna", "a tuna",
                "It is thick and torpedo-shaped, all dense muscle and efficient fin.",
                "Its body seems built for speed, endurance and open-water pursuit.",
                "It exudes relentless forward momentum even while still.",
                "blue water and pelagic sea"),
            AnimalBreathingMode.Saltwater,
            description: """
            Tuna are dense open-ocean swimmers, built for blue water, speed and relentless distance. They feel less like fish of place than fish of motion, belonging to currents too wide for shore-bound habits.

            The thick torpedo body, compact fins and heavy muscle make a tuna seem engineered by the need to keep moving. Even hauled still, it carries the impression of stored velocity and deep-water pressure.

            People value tuna as prize, food and proof of serious fishing reach. A tuna connects the table to far horizons, hard gear and the dangerous generosity of the pelagic sea.
            """);

        yield return Aquatic("Shark", "Shark", "Piscine", SizeCategory.Large, 1.5, "Shark", "shark",
            AquaticPack("a shark pup", "a young shark", "a shark",
                "It is sleek and immensely muscular, its body arranged around relentless forward movement.",
                "Its mouth of teeth and cold unblinking eyes leave no doubt that it is an apex predator.",
                "It carries itself with the remorseless calm of something that expects to eat what it catches.",
                "open sea and dark water"),
            AnimalBreathingMode.Saltwater,
            combatStrategyKey: "Beast Drowner",
            description: """
            Sharks are apex marine predators whose presence changes the meaning of open water. A fin, a pale belly in the dark or the slow turn of a powerful body can make a whole stretch of sea feel suddenly claimed.

            Their shape is spare and severe: heavy tail, cutting fins, cold eyes and a mouth built from layered teeth. They do not need frantic movement to threaten; the calm approach is often worse, because it suggests confidence in the outcome.

            People meet sharks as danger, omen, quarry and ecological fact. They are not simply large fish but the sea's reminder that beneath trade routes, beaches and boats there are older forms of appetite.
            """);

        yield return Aquatic("Small Crab", "Small Crab", "Decapod", SizeCategory.VerySmall, 0.2, "Crab", "crab-small",
            AquaticPack("a tiny crab larva", "a young small crab", "a small crab",
                "It is a compact little crustacean, shell-backed and side-stepping on multiple jointed legs.",
                "Its claws are tiny but very real, and its body is mostly armour and irritation.",
                "It skitters with prickly sideways certainty.",
                "tidal rock, shoreline pool and shallow reef"),
            AnimalBreathingMode.Saltwater,
            combatStrategyKey: "Beast Clincher",
            description: """
            Small crabs occupy reefs, tidal flats, rocky seabeds, creeks, estuaries and mangroves as armoured aquatic arthropods. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Look closer and the outline is all jointed legs, shells, claws, antennae and tails that make armour part of ordinary movement. In motion they are marked by sideways certainty, sudden retreats and stubborn little assertions of space, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are familiar around nets and shallows, but their claws and shells make them feel like small machines made of hunger. Around settlements, roads or camps, small crabs add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Aquatic("Crab", "Crab", "Decapod", SizeCategory.Small, 0.6, "Crab", "crab-large",
            AquaticPack("a crab larva", "a young crab", "a crab",
                "It is broad-shelled and low to the ground, with stalked eyes and heavy claws.",
                "Its carapace and stance make it look like a piece of hostile shoreline come to life.",
                "It skitters sideways with abrupt mechanical decisiveness.",
                "reef, mangrove and tidal flat"),
            AnimalBreathingMode.Saltwater,
            combatStrategyKey: "Beast Clincher",
            description: """
            Where reefs, tidal flats, rocky seabeds, creeks, estuaries and mangroves meets daily life, crabs are recognisable as armoured aquatic arthropods. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The first things to register are jointed legs, shells, claws, antennae and tails that make armour part of ordinary movement. In motion they are marked by sideways certainty, sudden retreats and stubborn little assertions of space, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are familiar around nets and shallows, but their claws and shells make them feel like small machines made of hunger. Around settlements, roads or camps, crabs add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Aquatic("Giant Crab", "Giant Crab", "Decapod", SizeCategory.Normal, 1.2, "Large Crab", "crab-giant",
            AquaticPack("a giant crab larva", "a young giant crab", "a giant crab",
                "It is enormous for a crab, all shell, claw and stubborn lateral movement.",
                "Its claws are large enough to be taken seriously by anything close enough to see them.",
                "It has the blunt confidence of a creature that trusts its armour.",
                "reef shelf and deep tidal cave"),
            AnimalBreathingMode.Saltwater,
            combatStrategyKey: "Beast Clincher",
            description: """
            Giant crabs are armoured aquatic arthropods of reefs, tidal flats, rocky seabeds, creeks, estuaries and mangroves. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by jointed legs, shells, claws, antennae and tails that make armour part of ordinary movement. The impression is completed by sideways certainty, sudden retreats and stubborn little assertions of space, giving the animal a readable rhythm even before it acts.

            They are familiar around nets and shallows, but their claws and shells make them feel like small machines made of hunger. Around settlements, roads or camps, giant crabs add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Aquatic("Lobster", "Lobster", "Malacostracan", SizeCategory.Small, 0.6, "Crustacean", "crab-large",
            AquaticPack("a lobster larva", "a young lobster", "a lobster",
                "It is long-bodied and armoured, with heavy claws and a muscular tail fan.",
                "Its segmented abdomen and antennae give it a more elongate, primeval look than a crab.",
                "It creeps with slow, wary intent until startled into sudden retreat.",
                "reef crevice and rocky seabed"),
            AnimalBreathingMode.Saltwater,
            description: """
            In reefs, tidal flats, rocky seabeds, creeks, estuaries and mangroves, lobsters fill the role of armoured aquatic arthropods. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by jointed legs, shells, claws, antennae and tails that make armour part of ordinary movement. The impression is completed by sideways certainty, sudden retreats and stubborn little assertions of space, giving the animal a readable rhythm even before it acts.

            They are familiar around nets and shallows, but their claws and shells make them feel like small machines made of hunger. Around settlements, roads or camps, lobsters add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Aquatic("Shrimp", "Shrimp", "Malacostracan", SizeCategory.VerySmall, 0.1, "Crustacean", "crab-small",
            AquaticPack("a shrimp larva", "a young shrimp", "a shrimp",
                "It is narrow, translucent and finely jointed, with long feelers and a curled body.",
                "Its tail and antennae make it look like a scrap of articulated motion more than a settled animal.",
                "It flicks backward through the water in sudden nervous bursts.",
                "reef, estuary and weed bed"),
            AnimalBreathingMode.Saltwater,
            description: """
            Shrimp occupy reefs, tidal flats, rocky seabeds, creeks, estuaries and mangroves as armoured aquatic arthropods. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by jointed legs, shells, claws, antennae and tails that make armour part of ordinary movement. The impression is completed by sideways certainty, sudden retreats and stubborn little assertions of space, giving the animal a readable rhythm even before it acts.

            They are familiar around nets and shallows, but their claws and shells make them feel like small machines made of hunger. Around settlements, roads or camps, shrimp add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Aquatic("Prawn", "Prawn", "Malacostracan", SizeCategory.VerySmall, 0.15, "Crustacean", "crab-small",
            AquaticPack("a prawn larva", "a young prawn", "a prawn",
                "It is slender-bodied and semi-translucent, with long antennae and delicate claws.",
                "Its tail looks stronger and more active than the rest of its lightly armoured body.",
                "It appears ready to snap backward away from danger at any moment.",
                "estuary, mangrove and warm shallows"),
            AnimalBreathingMode.Saltwater,
            description: """
            Where reefs, tidal flats, rocky seabeds, creeks, estuaries and mangroves meets daily life, prawns are recognisable as armoured aquatic arthropods. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by jointed legs, shells, claws, antennae and tails that make armour part of ordinary movement. The impression is completed by sideways certainty, sudden retreats and stubborn little assertions of space, giving the animal a readable rhythm even before it acts.

            They are familiar around nets and shallows, but their claws and shells make them feel like small machines made of hunger. Around settlements, roads or camps, prawns add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Aquatic("Crayfish", "Crayfish", "Malacostracan", SizeCategory.VerySmall, 0.2, "Crustacean", "crab-small",
            AquaticPack("a crayfish larva", "a young crayfish", "a crayfish",
                "It is a small fresh-water crustacean with a shell-backed body, claws and curling tail.",
                "Its squat armour and feelers make it look like a tiny river-bottom bruiser.",
                "It creeps with stubborn purpose and sudden jerks of retreat.",
                "stream, creek bed and muddy bank"),
            AnimalBreathingMode.Freshwater,
            description: """
            Crayfish are armoured aquatic arthropods of reefs, tidal flats, rocky seabeds, creeks, estuaries and mangroves. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: jointed legs, shells, claws, antennae and tails that make armour part of ordinary movement. Watch one move and the emphasis falls on sideways certainty, sudden retreats and stubborn little assertions of space, a pattern that tells more than size alone.

            They are familiar around nets and shallows, but their claws and shells make them feel like small machines made of hunger. Around settlements, roads or camps, crayfish add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);

        yield return Aquatic("Jellyfish", "Jellyfish", "Jellyfish", SizeCategory.Small, 0.1, "Jellyfish", "jellyfish",
            AquaticPack("a young jellyfish", "a young jellyfish", "a jellyfish",
                "It is little more than a translucent bell trailing fine tendrils beneath it.",
                "Its body is delicate-looking, but the drifting tentacles promise a sting out of all proportion to its substance.",
                "It pulses and drifts with eerie, indifferent grace.",
                "open water and current"),
            AnimalBreathingMode.Partless,
            combatStrategyKey: "Beast Artillery",
            description: """
            In open water, current lines and quiet coastal drift, jellyfish fill the role of soft-bodied drifting sea creatures. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: translucent bells, trailing tendrils and almost impossible delicacy. Watch one move and the emphasis falls on slow pulses, passive drift and eerie indifference, a pattern that tells more than size alone.

            They are beautiful at a distance and painful at a touch, teaching caution where the water looks empty. Around settlements, roads or camps, jellyfish add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);

        yield return Aquatic("Octopus", "Octopus", "Cephalopod", SizeCategory.Small, 0.4, "Cephalopod", "cephalopod",
            AquaticPack("a young octopus", "a young octopus", "an octopus",
                "It is soft-bodied and watchful, with an intelligent eye and a ring of dexterous arms.",
                "Its suckered limbs and changing skin make it look too adaptable and too aware for comfort.",
                "It seems to think before it moves, and that alone is unsettling.",
                "reef, wreck and rocky sea floor"),
            AnimalBreathingMode.Saltwater, true, "Beast Clincher",
            description: """
            Octopuses occupy reefs, wrecks, rocky floors, open sea and deep water as intelligent soft-bodied sea hunters. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: mantles, arms, suckers, large eyes and bodies able to change, cling and vanish. Watch one move and the emphasis falls on deliberate stillness broken by jets of speed or sudden camouflage, a pattern that tells more than size alone.

            They feel less like simple animals than watchful problems, especially when a hand reaches where an arm can answer. Around settlements, roads or camps, octopuses add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Aquatic("Squid", "Squid", "Cephalopod", SizeCategory.Small, 0.4, "Cephalopod", "cephalopod",
            AquaticPack("a young squid", "a young squid", "a squid",
                "It is narrow-bodied and swift, with fins along the mantle and a crown of arms at the front.",
                "Its shape is that of a creature built for jetting bursts of speed and sudden turns.",
                "It always seems one moment away from vanishing into darker water.",
                "open sea and deep current"),
            AnimalBreathingMode.Saltwater,
            combatStrategyKey: "Beast Clincher",
            description: """
            Where reefs, wrecks, rocky floors, open sea and deep water meets daily life, squid are recognisable as intelligent soft-bodied sea hunters. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: mantles, arms, suckers, large eyes and bodies able to change, cling and vanish. Watch one move and the emphasis falls on deliberate stillness broken by jets of speed or sudden camouflage, a pattern that tells more than size alone.

            They feel less like simple animals than watchful problems, especially when a hand reaches where an arm can answer. Around settlements, roads or camps, squid add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Aquatic("Giant Squid", "Giant Squid", "Cephalopod", SizeCategory.Large, 1.0, "Giant Cephalopod", "cephalopod",
            AquaticPack("a young giant squid", "a young giant squid", "a giant squid",
                "It is an outsized cephalopod, all long mantle, heavy arms and impossible dark eyes.",
                "Its scale alone turns an already alien body plan into something bordering on monstrous.",
                "It suggests abyssal depth and cold water where light does not matter much.",
                "deep ocean and black water"),
            AnimalBreathingMode.Saltwater,
            combatStrategyKey: "Beast Clincher",
            description: """
            Giant squid are intelligent soft-bodied sea hunters of reefs, wrecks, rocky floors, open sea and deep water. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The most telling cues are mantles, arms, suckers, large eyes and bodies able to change, cling and vanish. Those features support deliberate stillness broken by jets of speed or sudden camouflage, making the creature feel suited to its ground rather than merely placed there.

            They feel less like simple animals than watchful problems, especially when a hand reaches where an arm can answer. Around settlements, roads or camps, giant squid add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);

        yield return Aquatic("Sea Lion", "Sea Lion", "Pinniped", SizeCategory.Normal, 0.8, "Pinniped", "pinniped",
            AquaticPack("a sea lion pup", "a young sea lion", "a sea lion",
                "It is thick-bodied and sleek-coated, with strong foreflippers and a long whiskered muzzle.",
                "Its body speaks of a creature equally willing to haul out on rock or surge through surf.",
                "It has the loud self-assurance of a communal coastal animal.",
                "rocky shore and rolling surf"),
            AnimalBreathingMode.Blowhole,
            description: """
            In ice edges, rocky shores, sandbars and rolling surf, sea lions fill the role of flippered marine mammals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Most of its character sits in sleek coats, whiskered muzzles, blubbered bodies and limbs made for water first. Those features support awkward land movement transformed into easy swimming power, making the creature feel suited to its ground rather than merely placed there.

            They gather where sea and land meet, noisy, watchful and more formidable than their soft-eyed faces imply. Around settlements, roads or camps, sea lions add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Aquatic("Seal", "Seal", "Pinniped", SizeCategory.Normal, 0.8, "Pinniped", "pinniped",
            AquaticPack("a seal pup", "a young seal", "a seal",
                "It is smooth and streamlined, with huge dark eyes and whiskers around a neat muzzle.",
                "Its body is all blubber, grace and flipper-powered practicality.",
                "It looks soft until it begins to move, and then it becomes something fluid and efficient.",
                "ice edge, sandbar and cold coast"),
            AnimalBreathingMode.Blowhole,
            description: """
            Seals occupy ice edges, rocky shores, sandbars and rolling surf as flippered marine mammals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            What distinguishes the line is sleek coats, whiskered muzzles, blubbered bodies and limbs made for water first. Those features support awkward land movement transformed into easy swimming power, making the creature feel suited to its ground rather than merely placed there.

            They gather where sea and land meet, noisy, watchful and more formidable than their soft-eyed faces imply. Around settlements, roads or camps, seals add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Aquatic("Walrus", "Walrus", "Pinniped", SizeCategory.Large, 1.2, "Walrus", "pinniped",
            AquaticPack("a walrus calf", "a young walrus", "a walrus",
                "It is vast, whiskered and thick-skinned, with tusks curving down from a heavy face.",
                "Its blubbery body and tusks together make it seem both absurd and dangerous at once.",
                "It carries itself with ancient, cumbersome authority.",
                "ice floe and cold northern shore"),
            AnimalBreathingMode.Blowhole,
            description: """
            Where ice edges, rocky shores, sandbars and rolling surf meets daily life, walruses are recognisable as flippered marine mammals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The creature is best read through sleek coats, whiskered muzzles, blubbered bodies and limbs made for water first. Those features support awkward land movement transformed into easy swimming power, making the creature feel suited to its ground rather than merely placed there.

            They gather where sea and land meet, noisy, watchful and more formidable than their soft-eyed faces imply. Around settlements, roads or camps, walruses add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);

        yield return Aquatic("Dolphin", "Dolphin", "Cetacean", SizeCategory.Normal, 0.8, "Dolphin", "dolphin",
            AquaticPack("a dolphin calf", "a young dolphin", "a dolphin",
                "It is sleek and bright-skinned, with a curved mouthline and fluid muscular body.",
                "Its eyes and posture give it an impression of curiosity that most fish never approach.",
                "It moves with confident playful intelligence.",
                "open coastal water and warm current"),
            AnimalBreathingMode.Blowhole,
            combatStrategyKey: "Beast Skirmisher",
            description: """
            Dolphins are large breath-holding marine mammals of coastal waters, cold currents, pelagic seas and migratory routes. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Its profile is built around smooth skin, flukes, deep chests and shapes designed for long movement through water. In motion they are marked by fluid assurance and an intelligence that changes the mood of the sea around them, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They turn ocean into encounter rather than emptiness, inspiring awe, fear and stories wherever their backs break the surface. For characters nearby, a dolphin is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Aquatic("Porpoise", "Porpoise", "Cetacean", SizeCategory.Normal, 0.8, "Dolphin", "dolphin",
            AquaticPack("a porpoise calf", "a young porpoise", "a porpoise",
                "It is shorter and more compact than a dolphin, with a neat blunt head and dark smooth skin.",
                "Its body is streamlined without looking delicate, built for fast efficient movement through chill water.",
                "It surfaces and dives with understated assurance.",
                "coastal current and cool sea"),
            AnimalBreathingMode.Blowhole,
            combatStrategyKey: "Beast Skirmisher",
            description: """
            In coastal waters, cold currents, pelagic seas and migratory routes, porpoises fill the role of large breath-holding marine mammals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The creature presents smooth skin, flukes, deep chests and shapes designed for long movement through water. In motion they are marked by fluid assurance and an intelligence that changes the mood of the sea around them, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They turn ocean into encounter rather than emptiness, inspiring awe, fear and stories wherever their backs break the surface. For characters nearby, a porpoise is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Aquatic("Orca", "Orca", "Cetacean", SizeCategory.Large, 1.6, "Small Whale", "orca",
            AquaticPack("an orca calf", "a young orca", "an orca",
                "It is black-and-white and powerfully built, with a massive jawline and heavy tail stock.",
                "Its markings are striking, but its size and confidence are what truly dominate the eye.",
                "It cuts through the water with top-predator certainty.",
                "cold current and open sea"),
            AnimalBreathingMode.Blowhole,
            combatStrategyKey: "Beast Behemoth",
            description: """
            Orcas occupy coastal waters, cold currents, pelagic seas and migratory routes as large breath-holding marine mammals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            At a glance, the form resolves into smooth skin, flukes, deep chests and shapes designed for long movement through water. In motion they are marked by fluid assurance and an intelligence that changes the mood of the sea around them, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They turn ocean into encounter rather than emptiness, inspiring awe, fear and stories wherever their backs break the surface. For characters nearby, a orca is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Aquatic("Baleen Whale", "Baleen Whale", "Cetacean", SizeCategory.VeryLarge, 2.0, "Large Whale", "baleen-whale",
            AquaticPack("a whale calf", "a young whale", "a baleen whale",
                "It is enormous, smooth-backed and impossibly heavy even by marine standards.",
                "Its size is its most striking feature, overwhelming any detail of fin, eye or jaw.",
                "It moves like weather given flesh.",
                "deep sea and migratory water"),
            AnimalBreathingMode.Blowhole,
            combatStrategyKey: "Beast Behemoth",
            description: """
            Where coastal waters, cold currents, pelagic seas and migratory routes meets daily life, baleen whales are recognisable as large breath-holding marine mammals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The visible essentials are smooth skin, flukes, deep chests and shapes designed for long movement through water. In motion they are marked by fluid assurance and an intelligence that changes the mood of the sea around them, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They turn ocean into encounter rather than emptiness, inspiring awe, fear and stories wherever their backs break the surface. For characters nearby, a baleen whale is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Aquatic("Toothed Whale", "Toothed Whale", "Cetacean", SizeCategory.VeryLarge, 2.0, "Large Whale", "toothed-whale",
            AquaticPack("a whale calf", "a young whale", "a toothed whale",
                "It is huge and deep-bodied, with a blunt skull and muscular flukes.",
                "Its jaw and head proportions mark it as a hunter rather than a placid filter-feeder.",
                "It advances through the sea with grim, inexorable power.",
                "deep ocean and pelagic current"),
            AnimalBreathingMode.Blowhole,
            combatStrategyKey: "Beast Behemoth",
            description: """
            Toothed whales are large breath-holding marine mammals of coastal waters, cold currents, pelagic seas and migratory routes. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by smooth skin, flukes, deep chests and shapes designed for long movement through water. The impression is completed by fluid assurance and an intelligence that changes the mood of the sea around them, giving the animal a readable rhythm even before it acts.

            They turn ocean into encounter rather than emptiness, inspiring awe, fear and stories wherever their backs break the surface. For characters nearby, a toothed whale is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
    }
}
