#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System;

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

    private void SeedAquatic(BodyProto fishProto, BodyProto crabProto, BodyProto malacostracanProto, BodyProto octopusProto,
        BodyProto jellyfishProto, BodyProto pinnipedProto, BodyProto cetaceanProto)
    {
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Fish...");

        #region Fish

        ResetCachedParts();
        int order = 1;

        #region Torso

        AddBodypart(fishProto, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.BonyDrapeable, null, Alignment.Front,
            Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "rbreast", "right breast", "breast", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.FrontRight, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "lbreast", "left breast", "breast", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.FrontLeft, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "urflank", "upper right flank", "flank", BodypartTypeEnum.BonyDrapeable, "rbreast",
            Alignment.Right, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "ulflank", "upper left flank", "flank", BodypartTypeEnum.BonyDrapeable, "lbreast",
            Alignment.Left, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "lrflank", "lower right flank", "flank", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.RearRight, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "llflank", "lower left flank", "flank", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.RearLeft, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "belly", "belly", "belly", BodypartTypeEnum.Wear, "abdomen", Alignment.Front,
            Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "uback", "upper back", "upper back", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.Front, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "lback", "lower back", "lower back", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.Rear, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "loin", "loin", "loin", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
            Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(fishProto, "dorsalfin", "dorsal fin", "fin", BodypartTypeEnum.Fin, "uback", Alignment.Rear,
            Orientation.Highest, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");
        AddBodypart(fishProto, "analfin", "anal fin", "fin", BodypartTypeEnum.Fin, "loin", Alignment.Rear,
            Orientation.Lowest, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");
        AddBodypart(fishProto, "rpectoralfin", "right pectoral fin", "fin", BodypartTypeEnum.Fin, "urflank",
            Alignment.Right, Orientation.Centre, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");
        AddBodypart(fishProto, "lpectoralfin", "left pectoral fin", "fin", BodypartTypeEnum.Fin, "ulflank",
            Alignment.Left, Orientation.Centre, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");
        AddBodypart(fishProto, "rpelvicfin", "right pelvic fin", "fin", BodypartTypeEnum.Fin, "belly",
            Alignment.Right, Orientation.Centre, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");
        AddBodypart(fishProto, "lpelvicfin", "left pelvic fin", "fin", BodypartTypeEnum.Fin, "belly",
            Alignment.Left, Orientation.Low, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");

        #endregion

        #region Head

        AddBodypart(fishProto, "neck", "neck", "neck", BodypartTypeEnum.BonyDrapeable, "uback", Alignment.Front,
            Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(fishProto, "rgill", "right gills", "gill", BodypartTypeEnum.Gill, "urflank", Alignment.Right,
            Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
            implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(fishProto, "lgill", "left gills", "gill", BodypartTypeEnum.Gill, "ulflank", Alignment.Left,
            Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
            implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(fishProto, "head", "head", "face", BodypartTypeEnum.BonyDrapeable, "neck", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(fishProto, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.BonyDrapeable, "head",
            Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(fishProto, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.BonyDrapeable, "head",
            Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(fishProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket", Alignment.FrontRight,
            Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(fishProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket", Alignment.FrontLeft,
            Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(fishProto, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "head", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);

        #endregion

        #region Tail

        AddBodypart(fishProto, "peduncle", "peduncle", "Peduncle", BodypartTypeEnum.Wear, "lback", Alignment.Rear,
    Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");
        AddBodypart(fishProto, "caudalfin", "caudal fin", "fin", BodypartTypeEnum.Fin, "peduncle", Alignment.Rear,
            Orientation.Centre, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Tail");

        #endregion

        _context.SaveChanges();

        #region Bones

        AddBone(fishProto, "fskull", "frontal skull bone", BodypartTypeEnum.NonImmobilisingBone, 200,
            "Compact Bone");
        AddBone(fishProto, "cvertebrae", "cervical vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(fishProto, "dvertebrae", "dorsal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(fishProto, "lvertebrae", "lumbar vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(fishProto, "cavertebrae", "caudal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        _context.SaveChanges();

        AddBoneInternal("fskull", "head", 100);
        AddBoneInternal("cvertebrae", "neck", 100);
        AddBoneInternal("dvertebrae", "uback", 100);
        AddBoneInternal("lvertebrae", "lback", 100);
        AddBoneInternal("cavertebrae", "peduncle", 100);
        _context.SaveChanges();

        #endregion

        #region Organs

        AddOrgan(fishProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, stunModifier: 1.0);
        AddOrgan(fishProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
        AddOrgan(fishProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(fishProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(fishProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(fishProto, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
            0.05);
        AddOrgan(fishProto, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
            0.05);
        AddOrgan(fishProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(fishProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(fishProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(fishProto, "uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(fishProto, "mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(fishProto, "lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(fishProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
        AddOrgan(fishProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

        AddOrganCoverage("brain", "head", 100, true);
        AddOrganCoverage("brain", "reyesocket", 85);
        AddOrganCoverage("brain", "leyesocket", 85);
        AddOrganCoverage("brain", "reye", 85);
        AddOrganCoverage("brain", "leye", 85);

        AddOrganCoverage("linnerear", "head", 10, true);
        AddOrganCoverage("rinnerear", "head", 10, true);
        AddOrganCoverage("esophagus", "neck", 20, true);

        AddOrganCoverage("heart", "lbreast", 33, true);

        AddOrganCoverage("uspinalcord", "neck", 2, true);
        AddOrganCoverage("mspinalcord", "uback", 10, true);
        AddOrganCoverage("lspinalcord", "lback", 10, true);
        AddOrganCoverage("lspinalcord", "peduncle", 10);

        AddOrganCoverage("liver", "abdomen", 33, true);
        AddOrganCoverage("spleen", "abdomen", 20, true);
        AddOrganCoverage("stomach", "abdomen", 20, true);
        AddOrganCoverage("liver", "uback", 15);
        AddOrganCoverage("spleen", "uback", 10);
        AddOrganCoverage("stomach", "uback", 5);

        AddOrganCoverage("lintestines", "belly", 5, true);
        AddOrganCoverage("sintestines", "belly", 50, true);
        AddOrganCoverage("lintestines", "lback", 5);
        AddOrganCoverage("sintestines", "lback", 33);
        AddOrganCoverage("lintestines", "loin", 5);

        AddOrganCoverage("rkidney", "lback", 20, true);
        AddOrganCoverage("lkidney", "lback", 20, true);
        AddOrganCoverage("rkidney", "belly", 5);
        AddOrganCoverage("lkidney", "belly", 5);

        AddBoneCover("fskull", "brain", 100);
        AddBoneCover("cvertebrae", "uspinalcord", 100);
        AddBoneCover("dvertebrae", "mspinalcord", 100);
        AddBoneCover("lvertebrae", "lspinalcord", 100);
        AddBoneCover("cavertebrae", "lspinalcord", 100);
        _context.SaveChanges();

        #endregion

        _context.SaveChanges();

        foreach ((BodypartProto? child, BodypartProto? parent) in _cachedBodypartUpstreams)
        {
            _context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
            {
                Child = child.Id,
                Parent = parent.Id
            });
        }

        _context.SaveChanges();

        #region Limbs

        Dictionary<string, Limb> limbs = new(StringComparer.OrdinalIgnoreCase);

        void AddLimb(string name, LimbType limbType, string rootPart, double damageThreshold,
            double painThreshold, BodyProto proto)
        {
            Limb limb = new()
            {
                Name = name,
                LimbType = (int)limbType,
                RootBody = proto,
                RootBodypart = _cachedBodyparts[rootPart],
                LimbDamageThresholdMultiplier = damageThreshold,
                LimbPainThresholdMultiplier = painThreshold
            };
            _context.Limbs.Add(limb);
            limbs[name] = limb;
        }

        AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0, fishProto);
        AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0, fishProto);
        AddLimb("Tail", LimbType.Appendage, "peduncle", 0.5, 0.5, fishProto);
        _context.SaveChanges();

        foreach (Limb limb in limbs.Values)
        {
            foreach (BodypartProto part in _cachedLimbs[limb.Name])
            {
                _context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
            }

            switch (limb.Name)
            {
                case "Torso":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["uspinalcord"] });
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["mspinalcord"] });
                    break;
                case "Tail":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["lspinalcord"] });
                    break;
            }
        }

        _context.SaveChanges();

        #endregion

        #region Groups

        AddBodypartGroupDescriberShape(fishProto, "body", "The whole torso of a fish",
            ("abdomen", 0, 1),
            ("breast", 0, 2),
            ("flank", 0, 4),
            ("belly", 0, 1),
            ("loin", 0, 1),
            ("upper back", 1, 1),
            ("lower back", 1, 1),
            ("neck", 0, 1),
            ("gill", 0, 2),
            ("peduncle", 0, 1)
        );

        AddBodypartGroupDescriberShape(fishProto, "fins", "All the fins of a fish",
            ("fin", 2, 6)
        );

        AddBodypartGroupDescriberDirect(fishProto, "pectoral fins", "The pectoral fins of a fish",
            ("rpectoralfin", true),
            ("lpectoralfin", true)
        );

        AddBodypartGroupDescriberShape(fishProto, "head", "The eyes of a fish",
            ("eye socket", 0, 2),
            ("eye", 0, 2)
        );

        AddBodypartGroupDescriberDirect(fishProto, "gills", "The gills of a fish",
            ("rgill", true),
            ("lgill", true)
        );

        AddBodypartGroupDescriberShape(fishProto, "head", "The whole head of a fish",
            ("face", 0, 1),
            ("eye socket", 0, 2),
            ("eye", 0, 2),
            ("mouth", 0, 1),
            ("neck", 0, 1),
            ("gill", 0, 2)
        );

        AddBodypartGroupDescriberDirect(fishProto, "pelvic fins", "The pelvic fins of a fish",
            ("rpelvicfin", true),
            ("lpelvicfin", true)
        );

        AddBodypartGroupDescriberDirect(fishProto, "tail", "The whole of a fish",
            ("caudalfin", true),
            ("peduncle", true)
        );

        #endregion

        _context.SaveChanges();

        #endregion

        #region Crabs

        SeedDecapodBody(crabProto);
        SeedMalacostracanBody(malacostracanProto);

        #endregion

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Cephalopods...");

        #region Cephalopods

        ResetCachedParts();
        order = 1;

        #region Torso

        AddBodypart(octopusProto, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null, Alignment.Front,
            Orientation.Centre, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(octopusProto, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "abdomen", Alignment.Front,
            Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);

        #endregion

        #region Head

        AddBodypart(octopusProto, "head", "head", "head", BodypartTypeEnum.Wear, "abdomen", Alignment.Front,
            Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(octopusProto, "mantle", "mantle", "mantle", BodypartTypeEnum.Wear, "head", Alignment.Front,
            Orientation.Highest, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
            implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(octopusProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "head", Alignment.FrontRight,
            Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(octopusProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "head", Alignment.FrontLeft,
            Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);

        #endregion

        #region Appendages

        AddBodypart(octopusProto, "arm1", "1st Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
            Alignment.Front,
            Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "1st Arm");
        AddBodypart(octopusProto, "arm2", "2nd Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
            Alignment.Front,
            Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "2nd Arm");
        AddBodypart(octopusProto, "arm3", "3rd Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
            Alignment.Front,
            Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "3rd Arm");
        AddBodypart(octopusProto, "arm4", "4th Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
            Alignment.Front,
            Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "4th Arm");
        AddBodypart(octopusProto, "arm5", "5th Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
            Alignment.Front,
            Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "5th Arm");
        AddBodypart(octopusProto, "arm6", "6th Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
            Alignment.Front,
            Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "6th Arm");
        AddBodypart(octopusProto, "arm7", "7th Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
            Alignment.Front,
            Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "7th Arm");
        AddBodypart(octopusProto, "arm8", "8th Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
            Alignment.Front,
            Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "8th Arm");
        AddBodypart(octopusProto, "tentacle1", "1st Tentacle", "tentacle", BodypartTypeEnum.GrabbingWielding, "abdomen",
            Alignment.Front,
            Orientation.Lowest, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "1st Tentacle", isCore: false);
        AddBodypart(octopusProto, "tentacle2", "2nd Tentacle", "tentacle", BodypartTypeEnum.GrabbingWielding, "abdomen",
            Alignment.Front,
            Orientation.Lowest, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "2nd Tentacle", isCore: false);

        #endregion

        _context.SaveChanges();

        #region Organs

        AddOrgan(octopusProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, stunModifier: 1.0);
        AddOrgan(octopusProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
        AddOrgan(octopusProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(octopusProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(octopusProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(octopusProto, "intestines", "intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
            0.05);
        AddOrgan(octopusProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(octopusProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(octopusProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(octopusProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
        AddOrgan(octopusProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

        AddOrganCoverage("brain", "head", 100, true);
        AddOrganCoverage("brain", "reye", 85);
        AddOrganCoverage("brain", "leye", 85);
        AddOrganCoverage("brain", "mouth", 5);
        AddOrganCoverage("linnerear", "head", 10, true);
        AddOrganCoverage("rinnerear", "head", 10, true);
        AddOrganCoverage("esophagus", "abdomen", 20, true);
        AddOrganCoverage("heart", "mantle", 33, true);
        AddOrganCoverage("liver", "mantle", 33, true);
        AddOrganCoverage("spleen", "mantle", 20, true);
        AddOrganCoverage("stomach", "mantle", 20, true);
        AddOrganCoverage("intestines", "mantle", 5, true);
        AddOrganCoverage("rkidney", "mantle", 20, true);
        AddOrganCoverage("lkidney", "mantle", 20, true);
        _context.SaveChanges();

        #endregion

        _context.SaveChanges();

        foreach ((BodypartProto? child, BodypartProto? parent) in _cachedBodypartUpstreams)
        {
            _context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
            {
                Child = child.Id,
                Parent = parent.Id
            });
        }

        _context.SaveChanges();

        #region Limbs

        limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);
        AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0, octopusProto);
        AddLimb("Head", LimbType.Head, "head", 1.0, 1.0, octopusProto);
        AddLimb("1st Arm", LimbType.Arm, "arm1", 0.5, 0.5, octopusProto);
        AddLimb("2nd Arm", LimbType.Arm, "arm2", 0.5, 0.5, octopusProto);
        AddLimb("3rd Arm", LimbType.Arm, "arm3", 0.5, 0.5, octopusProto);
        AddLimb("4th Arm", LimbType.Arm, "arm4", 0.5, 0.5, octopusProto);
        AddLimb("5th Arm", LimbType.Arm, "arm5", 0.5, 0.5, octopusProto);
        AddLimb("6th Arm", LimbType.Arm, "arm6", 0.5, 0.5, octopusProto);
        AddLimb("7th Arm", LimbType.Arm, "arm7", 0.5, 0.5, octopusProto);
        AddLimb("8th Arm", LimbType.Arm, "arm8", 0.5, 0.5, octopusProto);
        AddLimb("1st Tentacle", LimbType.Appendage, "tentacle1", 0.5, 0.5, octopusProto);
        AddLimb("2nd Tentacle", LimbType.Appendage, "tentacle2", 0.5, 0.5, octopusProto);
        _context.SaveChanges();

        foreach (Limb limb in limbs.Values)
        {
            foreach (BodypartProto part in _cachedLimbs[limb.Name])
            {
                _context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
            }
        }

        #endregion

        #region Groups

        AddBodypartGroupDescriberShape(octopusProto, "body", "The whole torso of a cephalopod",
            ("abdomen", 0, 1),
            ("head", 0, 1),
            ("mantle", 0, 1),
            ("mouth", 0, 1)
        );

        AddBodypartGroupDescriberShape(octopusProto, "arms", "All the arms of a cephalopod",
            ("arm", 2, 8)
        );

        AddBodypartGroupDescriberShape(octopusProto, "tentacles", "All the tentacles of a cephalopod",
            ("tentacle", 2, 2)
        );

        AddBodypartGroupDescriberShape(octopusProto, "eyes", "The eyes of a cephalopod",
            ("eye", 0, 2)
        );

        #endregion

        _context.SaveChanges();

        #endregion

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Jellyfish...");

        #region Jellyfish

        ResetCachedParts();
        order = 1;

        #region Torso

        AddBodypart(jellyfishProto, "body", "body", "abdomen", BodypartTypeEnum.Wear, null, Alignment.Front,
            Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);

        #endregion

        #region Appendages

        for (int i = 1; i < 11; i++)
        {
            AddBodypart(jellyfishProto, $"tendril{i}", $"{i.ToOrdinal()} tendril", "tendril",
                BodypartTypeEnum.Wear, "body", Alignment.Front,
                Orientation.Lowest, 30, 50, 100, order++, "Flesh", SizeCategory.Small, $"Tendril{i}");
        }

        #endregion

        _context.SaveChanges();

        foreach ((BodypartProto? child, BodypartProto? parent) in _cachedBodypartUpstreams)
        {
            _context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
            {
                Child = child.Id,
                Parent = parent.Id
            });
        }

        _context.SaveChanges();

        #region Limbs

        limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);
        AddLimb("Torso", LimbType.Torso, "body", 1.0, 1.0, jellyfishProto);
        for (int i = 1; i < 11; i++)
        {
            AddLimb($"Tendril{i}", LimbType.Appendage, $"tendril{i}", 0.5, 0.5, jellyfishProto);
        }

        _context.SaveChanges();

        foreach (Limb limb in limbs.Values)
        {
            foreach (BodypartProto part in _cachedLimbs[limb.Name])
            {
                _context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
            }
        }

        #endregion

        #region Groups

        AddBodypartGroupDescriberShape(jellyfishProto, "tendrils", "All the tendrils of a jellyfish",
            ("tendril", 2, 10)
        );

        #endregion

        _context.SaveChanges();

        #endregion Jellyfish

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Pinnipeds...");

        #region Pinnipeds

        ResetCachedParts();
        order = 1;

        #region Torso

        AddBodypart(pinnipedProto, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null,
            Alignment.Front, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "rbreast", "right breast", "breast", BodypartTypeEnum.Wear, "abdomen",
            Alignment.FrontRight, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "lbreast", "left breast", "breast", BodypartTypeEnum.Wear, "abdomen",
            Alignment.FrontLeft, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "urflank", "upper right flank", "flank", BodypartTypeEnum.Wear, "rbreast",
            Alignment.Right, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "ulflank", "upper left flank", "flank", BodypartTypeEnum.Wear, "lbreast",
            Alignment.Left, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "lrflank", "lower right flank", "flank", BodypartTypeEnum.Wear, "abdomen",
            Alignment.RearRight, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "llflank", "lower left flank", "flank", BodypartTypeEnum.Wear, "abdomen",
            Alignment.RearLeft, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "belly", "belly", "belly", BodypartTypeEnum.Wear, "abdomen",
            Alignment.Front, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "rshoulder", "right shoulder", "shoulder", BodypartTypeEnum.Wear, "rbreast",
            Alignment.FrontRight, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "lshoulder", "left shoulder", "shoulder", BodypartTypeEnum.Wear, "lbreast",
            Alignment.FrontLeft, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "uback", "upper back", "upper back", BodypartTypeEnum.Wear, "abdomen",
            Alignment.Front, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "lback", "lower back", "lower back", BodypartTypeEnum.Wear, "abdomen",
            Alignment.Rear, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "withers", "withers", "withers", BodypartTypeEnum.Wear, "uback",
            Alignment.Front, Orientation.High, 80, -1, 50, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "rrump", "right rump", "rump", BodypartTypeEnum.Wear, "lback",
            Alignment.RearRight, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "lrump", "left rump", "rump", BodypartTypeEnum.Wear, "lback",
            Alignment.RearLeft, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "rloin", "right loin", "loin", BodypartTypeEnum.Wear, "belly",
            Alignment.RearRight, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "lloin", "left loin", "loin", BodypartTypeEnum.Wear, "belly",
            Alignment.RearLeft, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);

        #endregion

        #region Head

        AddBodypart(pinnipedProto, "neck", "neck", "neck", BodypartTypeEnum.Wear, "uback", Alignment.Front,
            Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "bneck", "neck back", "neck back", BodypartTypeEnum.Wear, "neck",
            Alignment.Rear, Orientation.Highest, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "throat", "throat", "throat", BodypartTypeEnum.Wear, "neck",
            Alignment.Front, Orientation.Highest, 40, 50, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "head", "head", "face", BodypartTypeEnum.Wear, "neck", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(pinnipedProto, "bhead", "head back", "head back", BodypartTypeEnum.Wear, "bneck",
            Alignment.Rear, Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(pinnipedProto, "rjaw", "right jaw", "jaw", BodypartTypeEnum.Wear, "head",
            Alignment.FrontRight, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "ljaw", "left jaw", "jaw", BodypartTypeEnum.Wear, "head",
            Alignment.FrontLeft, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "rcheek", "right cheek", "cheek", BodypartTypeEnum.Wear, "head",
            Alignment.Right, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "lcheek", "left cheek", "cheek", BodypartTypeEnum.Wear, "head",
            Alignment.Left, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.Wear,
            "head", Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh",
            SizeCategory.Small, "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.Wear,
            "head", Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh",
            SizeCategory.Small, "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket",
            Alignment.FrontRight, Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket",
            Alignment.FrontLeft, Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(pinnipedProto, "rear", "right ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Right,
            Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "lear", "left ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Left,
            Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(pinnipedProto, "muzzle", "muzzle", "muzzle", BodypartTypeEnum.Wear, "head",
            Alignment.Front, Orientation.Highest, 50, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(pinnipedProto, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "muzzle", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(pinnipedProto, "tongue", "tongue", "tongue", BodypartTypeEnum.Tongue, "mouth", Alignment.Front,
            Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(pinnipedProto, "nose", "nose", "nose", BodypartTypeEnum.Wear, "mouth", Alignment.Front,
            Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);

        #endregion

        #region Legs

        AddBodypart(pinnipedProto, "rfrontflipper", "right front flipper", "front flipper", BodypartTypeEnum.Standing,
            "rshoulder", Alignment.FrontRight, Orientation.Low, 80, 100, 100, order++, "Bony Flesh",
            SizeCategory.Normal, "Right Foreleg");
        AddBodypart(pinnipedProto, "lfrontflipper", "left front flipper", "front flipper", BodypartTypeEnum.Standing,
            "lshoulder", Alignment.FrontLeft, Orientation.Low, 80, 100, 100, order++, "Bony Flesh",
            SizeCategory.Normal, "Left Foreleg");
        AddBodypart(pinnipedProto, "rhindflipper", "right hind flipper", "hind flipper", BodypartTypeEnum.Standing,
            "rrump", Alignment.RearRight, Orientation.Low, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Right Hindleg");
        AddBodypart(pinnipedProto, "lhindflipper", "left hind flipper", "hind flipper", BodypartTypeEnum.Standing,
            "lrump", Alignment.RearLeft, Orientation.Low, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Left Hindleg");

        #endregion

        #region Tail

        AddBodypart(pinnipedProto, "tail", "tail", "tail", BodypartTypeEnum.Wear, "lback",
            Alignment.Rear, Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");

        #endregion

        #region Genitals

        AddBodypart(pinnipedProto, "groin", "groin", "groin", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
            Orientation.Low, 30, -1, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals");
        AddBodypart(pinnipedProto, "testicles", "testicles", "testicles", BodypartTypeEnum.Wear, "groin",
            Alignment.Rear, Orientation.Low, 10, 30, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals",
            true, isCore: false);
        AddBodypart(pinnipedProto, "penis", "penis", "penis", BodypartTypeEnum.Wear, "groin", Alignment.Rear,
            Orientation.Low, 10, 30, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals", true,
            isCore: false);
        AddBodypartUsage("penis", "male", pinnipedProto);
        AddBodypartUsage("testicles", "male", pinnipedProto);

        #endregion

        #region Misceallaneous

        AddBodypart(pinnipedProto, "rtusk", "right tusk", "tusk", BodypartTypeEnum.Wear, "rjaw",
            Alignment.FrontRight, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
            false, isCore: false);
        AddBodypart(pinnipedProto, "ltusk", "left tusk", "tusk", BodypartTypeEnum.Wear, "ljaw",
            Alignment.FrontLeft, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
            false, isCore: false);

        #endregion

        _context.SaveChanges();

        #region Organs

        AddOrgan(pinnipedProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1,
            stunModifier: 1.0);
        AddOrgan(pinnipedProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
        AddOrgan(pinnipedProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(pinnipedProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(pinnipedProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(pinnipedProto, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
            0.05);
        AddOrgan(pinnipedProto, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
            0.05);
        AddOrgan(pinnipedProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(pinnipedProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(pinnipedProto, "rlung", "right lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(pinnipedProto, "llung", "left lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(pinnipedProto, "trachea", "trachea", BodypartTypeEnum.Trachea, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(pinnipedProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(pinnipedProto, "uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(pinnipedProto, "mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0,
            0.05, stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(pinnipedProto, "lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(pinnipedProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
        AddOrgan(pinnipedProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

        AddOrganCoverage("brain", "head", 100, true);
        AddOrganCoverage("brain", "bhead", 100);
        AddOrganCoverage("brain", "rcheek", 85);
        AddOrganCoverage("brain", "lcheek", 85);
        AddOrganCoverage("brain", "reyesocket", 85);
        AddOrganCoverage("brain", "leyesocket", 85);
        AddOrganCoverage("brain", "reye", 85);
        AddOrganCoverage("brain", "leye", 85);
        AddOrganCoverage("brain", "muzzle", 10);
        AddOrganCoverage("brain", "mouth", 30);
        AddOrganCoverage("brain", "lear", 10);
        AddOrganCoverage("brain", "rear", 10);

        AddOrganCoverage("linnerear", "lear", 33, true);
        AddOrganCoverage("rinnerear", "rear", 33, true);
        AddOrganCoverage("esophagus", "throat", 50, true);
        AddOrganCoverage("esophagus", "neck", 20);
        AddOrganCoverage("esophagus", "bneck", 5);
        AddOrganCoverage("trachea", "throat", 50, true);
        AddOrganCoverage("trachea", "neck", 20);
        AddOrganCoverage("trachea", "bneck", 5);

        AddOrganCoverage("rlung", "rbreast", 100, true);
        AddOrganCoverage("llung", "lbreast", 100, true);
        AddOrganCoverage("rlung", "uback", 15);
        AddOrganCoverage("llung", "uback", 15);
        AddOrganCoverage("rlung", "rshoulder", 66);
        AddOrganCoverage("llung", "lshoulder", 66);

        AddOrganCoverage("heart", "lbreast", 33, true);
        AddOrganCoverage("heart", "lshoulder", 20);

        AddOrganCoverage("uspinalcord", "bneck", 10, true);
        AddOrganCoverage("uspinalcord", "neck", 2);
        AddOrganCoverage("uspinalcord", "throat", 5);
        AddOrganCoverage("mspinalcord", "uback", 10, true);
        AddOrganCoverage("mspinalcord", "withers", 2);
        AddOrganCoverage("lspinalcord", "lback", 10, true);

        AddOrganCoverage("liver", "abdomen", 33, true);
        AddOrganCoverage("spleen", "abdomen", 20, true);
        AddOrganCoverage("stomach", "abdomen", 20, true);
        AddOrganCoverage("liver", "uback", 15);
        AddOrganCoverage("spleen", "uback", 10);
        AddOrganCoverage("stomach", "uback", 5);

        AddOrganCoverage("lintestines", "belly", 5, true);
        AddOrganCoverage("sintestines", "belly", 50, true);
        AddOrganCoverage("lintestines", "lback", 5);
        AddOrganCoverage("sintestines", "lback", 33);
        AddOrganCoverage("lintestines", "groin", 5);
        AddOrganCoverage("lintestines", "rloin", 10);
        AddOrganCoverage("lintestines", "lloin", 10);

        AddOrganCoverage("rkidney", "lback", 20, true);
        AddOrganCoverage("lkidney", "lback", 20, true);
        AddOrganCoverage("rkidney", "belly", 5);
        AddOrganCoverage("lkidney", "belly", 5);
        _context.SaveChanges();

        #endregion

        _context.SaveChanges();

        #region Bones

        AddBone(pinnipedProto, "smaxillary", "superior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rimaxillary", "right inferior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "limaxillary", "left inferior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "fskull", "frontal skull bone", BodypartTypeEnum.NonImmobilisingBone, 200,
            "Compact Bone");
        AddBone(pinnipedProto, "cvertebrae", "cervical vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "dvertebrae", "dorsal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lvertebrae", "lumbar vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "svertebrae", "sacral vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "cavertebrae", "caudal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rscapula", "right scapula", BodypartTypeEnum.NonImmobilisingBone, 150,
            "Compact Bone");
        AddBone(pinnipedProto, "lscapula", "left scapula", BodypartTypeEnum.NonImmobilisingBone, 150,
            "Compact Bone");
        AddBone(pinnipedProto, "rhumerus", "right humerus", BodypartTypeEnum.Bone, 140, "Compact Bone");
        AddBone(pinnipedProto, "lhumerus", "left humerus", BodypartTypeEnum.Bone, 140, "Compact Bone");
        AddBone(pinnipedProto, "rradius", "right radius", BodypartTypeEnum.Bone, 140, "Compact Bone");
        AddBone(pinnipedProto, "lradius", "left radius", BodypartTypeEnum.Bone, 140, "Compact Bone");
        AddBone(pinnipedProto, "rulna", "right ulna", BodypartTypeEnum.Bone, 120, "Compact Bone");
        AddBone(pinnipedProto, "lulna", "left ulna", BodypartTypeEnum.Bone, 120, "Compact Bone");
        AddBone(pinnipedProto, "rcarpal", "right carpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
        AddBone(pinnipedProto, "lcarpal", "left carpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
        AddBone(pinnipedProto, "rmetacarpal", "right metacarpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
        AddBone(pinnipedProto, "lmetacarpal", "left metacarpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
        AddBone(pinnipedProto, "sternum", "sternum", BodypartTypeEnum.NonImmobilisingBone, 200, "Compact Bone");
        AddBone(pinnipedProto, "rrib1", "right first rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib1", "left first rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib2", "right second rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib2", "left second rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib3", "right third rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib3", "left third rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib4", "right fourth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib4", "left fourth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib5", "right fifth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib5", "left fifth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib6", "right sixth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib6", "left sixth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib7", "right seventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib7", "left seventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib8", "right eighth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib8", "left eighth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib9", "right ninth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib9", "left ninth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib10", "right tenth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib10", "left tenth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib11", "right eleventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib11", "left eleventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rrib12", "right twelth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "lrib12", "left twelth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(pinnipedProto, "rilium", "right ilium", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
        AddBone(pinnipedProto, "lilium", "left ilium", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
        AddBone(pinnipedProto, "sacrum", "sacrum", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
        AddBone(pinnipedProto, "rpubis", "right pubis", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
        AddBone(pinnipedProto, "lpubis", "left pubis", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
        AddBone(pinnipedProto, "rischium", "right ischium", BodypartTypeEnum.NonImmobilisingBone, 150,
            "Compact Bone");
        AddBone(pinnipedProto, "lischium", "left ischium", BodypartTypeEnum.NonImmobilisingBone, 150,
            "Compact Bone");
        AddBone(pinnipedProto, "rfemur", "right femur", BodypartTypeEnum.Bone, 200, "Compact Bone");
        AddBone(pinnipedProto, "lfemur", "left femur", BodypartTypeEnum.Bone, 200, "Compact Bone");
        AddBone(pinnipedProto, "rpatella", "right patella", BodypartTypeEnum.Bone, 90, "Compact Bone");
        AddBone(pinnipedProto, "lpatella", "left patella", BodypartTypeEnum.Bone, 90, "Compact Bone");
        AddBone(pinnipedProto, "rtibia", "right tibia", BodypartTypeEnum.Bone, 150, "Compact Bone");
        AddBone(pinnipedProto, "ltibia", "left tibia", BodypartTypeEnum.Bone, 150, "Compact Bone");
        AddBone(pinnipedProto, "rfibula", "right fibula", BodypartTypeEnum.Bone, 150, "Compact Bone");
        AddBone(pinnipedProto, "lfibula", "left fibula", BodypartTypeEnum.Bone, 150, "Compact Bone");
        AddBone(pinnipedProto, "rcalcaneus", "right calcaneus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(pinnipedProto, "lcalcaneus", "left calcaneus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(pinnipedProto, "rtalus", "right talus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(pinnipedProto, "ltalus", "left talus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(pinnipedProto, "rtarsus", "right tarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(pinnipedProto, "ltarsus", "left tarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(pinnipedProto, "rmetatarsus", "right metatarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(pinnipedProto, "lmetatarsus", "left metatarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");

        // TORSO BONES
        AddBoneInternal("sternum", "abdomen", 50);
        AddBoneInternal("rrib1", "rshoulder", 10);
        AddBoneInternal("lrib1", "lshoulder", 10);
        AddBoneInternal("rrib2", "rbreast", 5);
        AddBoneInternal("lrib2", "lbreast", 5);
        AddBoneInternal("rrib3", "rbreast", 5);
        AddBoneInternal("lrib3", "lbreast", 5);
        AddBoneInternal("rrib4", "rbreast", 5);
        AddBoneInternal("lrib4", "lbreast", 5);
        AddBoneInternal("rrib5", "rbreast", 5);
        AddBoneInternal("lrib5", "lbreast", 5);
        AddBoneInternal("rrib6", "rbreast", 5);
        AddBoneInternal("lrib6", "lbreast", 5);
        AddBoneInternal("rrib7", "rbreast", 5);
        AddBoneInternal("lrib7", "lbreast", 5);
        AddBoneInternal("rrib8", "rbreast", 5);
        AddBoneInternal("lrib8", "lbreast", 5);
        AddBoneInternal("rrib9", "rbreast", 5);
        AddBoneInternal("lrib9", "lbreast", 5);
        AddBoneInternal("rrib10", "rbreast", 5);
        AddBoneInternal("lrib10", "lbreast", 5);
        AddBoneInternal("rrib11", "rbreast", 5);
        AddBoneInternal("lrib11", "lbreast", 5);
        AddBoneInternal("rrib12", "rbreast", 5);
        AddBoneInternal("lrib12", "lbreast", 5);
        AddBoneInternal("cvertebrae", "bneck", 35);
        AddBoneInternal("dvertebrae", "uback", 20);
        AddBoneInternal("dvertebrae", "withers", 20, false);
        AddBoneInternal("lvertebrae", "lback", 20);
        AddBoneInternal("svertebrae", "tail", 90);
        AddBoneInternal("cavertebrae", "tail", 80);
        AddBoneInternal("sacrum", "lback", 15);
        AddBoneInternal("rilium", "rrump", 50);
        AddBoneInternal("lilium", "lrump", 50);
        AddBoneInternal("rilium", "lback", 4, false);
        AddBoneInternal("lilium", "lback", 4, false);
        AddBoneInternal("rpubis", "groin", 20);
        AddBoneInternal("lpubis", "groin", 20);
        AddBoneInternal("rischium", "rrump", 20);
        AddBoneInternal("lischium", "lrump", 20);

        // HEAD BONES
        AddBoneInternal("smaxillary", "muzzle", 100);
        AddBoneInternal("smaxillary", "mouth", 40, false);
        AddBoneInternal("smaxillary", "rcheek", 20, false);
        AddBoneInternal("smaxillary", "lcheek", 20, false);
        AddBoneInternal("rimaxillary", "rjaw", 100);
        AddBoneInternal("limaxillary", "ljaw", 100);
        AddBoneInternal("fskull", "head", 100);
        AddBoneInternal("fskull", "bhead", 40, false);

        // ARM BONES
        AddBoneInternal("rscapula", "rshoulder", 100);
        AddBoneInternal("lscapula", "lshoulder", 100);
        AddBoneInternal("rhumerus", "rfrontflipper", 50);
        AddBoneInternal("lhumerus", "lfrontflipper", 50);
        AddBoneInternal("rradius", "rfrontflipper", 33);
        AddBoneInternal("lradius", "lfrontflipper", 33);
        AddBoneInternal("rulna", "rfrontflipper", 33);
        AddBoneInternal("lulna", "lfrontflipper", 33);
        AddBoneInternal("rcarpal", "rfrontflipper", 50);
        AddBoneInternal("lcarpal", "lfrontflipper", 50);
        AddBoneInternal("rmetacarpal", "rfrontflipper", 50);
        AddBoneInternal("lmetacarpal", "lfrontflipper", 50);

        // LEG BONES
        AddBoneInternal("rfemur", "rhindflipper", 50);
        AddBoneInternal("lfemur", "lhindflipper", 50);
        AddBoneInternal("rpatella", "rhindflipper", 100);
        AddBoneInternal("lpatella", "lhindflipper", 100);
        AddBoneInternal("rtibia", "rhindflipper", 100);
        AddBoneInternal("ltibia", "lhindflipper", 100);
        AddBoneInternal("rfibula", "rhindflipper", 33);
        AddBoneInternal("lfibula", "lhindflipper", 33);
        AddBoneInternal("rcalcaneus", "rhindflipper", 20);
        AddBoneInternal("lcalcaneus", "lhindflipper", 20);
        AddBoneInternal("rtalus", "rhindflipper", 20);
        AddBoneInternal("ltalus", "lhindflipper", 20);
        AddBoneInternal("rtarsus", "rhindflipper", 50);
        AddBoneInternal("ltarsus", "lhindflipper", 50);
        AddBoneInternal("rmetatarsus", "rhindflipper", 50);
        AddBoneInternal("lmetatarsus", "lhindflipper", 50);
        _context.SaveChanges();

        AddBoneCover("fskull", "brain", 100);
        AddBoneCover("smaxillary", "brain", 90);

        AddBoneCover("cvertebrae", "uspinalcord", 100);
        AddBoneCover("dvertebrae", "mspinalcord", 100);
        AddBoneCover("lvertebrae", "lspinalcord", 100);

        AddBoneCover("sternum", "heart", 80);
        AddBoneCover("sternum", "rlung", 17.5);
        AddBoneCover("sternum", "llung", 17.5);
        AddBoneCover("lrib1", "heart", 5);
        AddBoneCover("lrib2", "heart", 10);
        AddBoneCover("lrib3", "heart", 15);
        AddBoneCover("lrib4", "heart", 15);
        AddBoneCover("lrib5", "heart", 15);
        AddBoneCover("lrib6", "heart", 15);
        AddBoneCover("lrib1", "llung", 10);
        AddBoneCover("lrib2", "llung", 15);
        AddBoneCover("lrib3", "llung", 20);
        AddBoneCover("lrib4", "llung", 20);
        AddBoneCover("lrib5", "llung", 20);
        AddBoneCover("lrib6", "llung", 20);
        AddBoneCover("lrib7", "llung", 20);
        AddBoneCover("rrib1", "rlung", 10);
        AddBoneCover("rrib2", "rlung", 15);
        AddBoneCover("rrib3", "rlung", 20);
        AddBoneCover("rrib4", "rlung", 20);
        AddBoneCover("rrib5", "rlung", 20);
        AddBoneCover("rrib6", "rlung", 20);
        AddBoneCover("rrib7", "rlung", 20);

        AddBoneCover("rrib6", "liver", 30);
        AddBoneCover("rrib7", "liver", 45);
        AddBoneCover("lrib6", "liver", 30);
        AddBoneCover("lrib7", "liver", 45);

        AddBoneCover("lrib8", "liver", 80);
        AddBoneCover("lrib8", "spleen", 25);
        AddBoneCover("rrib8", "liver", 80);
        AddBoneCover("rrib8", "spleen", 25);

        AddBoneCover("lrib9", "liver", 60);
        AddBoneCover("lrib9", "spleen", 20);
        AddBoneCover("rrib9", "liver", 60);
        AddBoneCover("rrib9", "spleen", 20);

        AddBoneCover("lrib10", "liver", 15);
        AddBoneCover("lrib10", "lkidney", 20);
        AddBoneCover("rrib10", "liver", 15);
        AddBoneCover("rrib10", "rkidney", 20);

        AddBoneCover("rscapula", "rlung", 70);
        AddBoneCover("lscapula", "llung", 70);

        AddBoneCover("rilium", "sintestines", 20);
        AddBoneCover("lilium", "sintestines", 20);
        AddBoneCover("rilium", "lintestines", 40);
        AddBoneCover("lilium", "lintestines", 40);

        AddBoneCover("rischium", "lintestines", 40);
        AddBoneCover("lischium", "lintestines", 40);
        _context.SaveChanges();

        #endregion

        _context.SaveChanges();

        foreach ((BodypartProto? child, BodypartProto? parent) in _cachedBodypartUpstreams)
        {
            _context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
            {
                Child = child.Id,
                Parent = parent.Id
            });
        }

        _context.SaveChanges();

        #region Limbs

        limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);
        AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0, pinnipedProto);
        AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0, pinnipedProto);
        AddLimb("Genitals", LimbType.Genitals, "groin", 0.5, 0.5, pinnipedProto);
        AddLimb("Right Foreleg", LimbType.Leg, "rfrontflipper", 0.5, 0.5, pinnipedProto);
        AddLimb("Left Foreleg", LimbType.Leg, "lfrontflipper", 0.5, 0.5, pinnipedProto);
        AddLimb("Right Hindleg", LimbType.Leg, "rhindflipper", 0.5, 0.5, pinnipedProto);
        AddLimb("Left Hindleg", LimbType.Leg, "lhindflipper", 0.5, 0.5, pinnipedProto);
        AddLimb("Tail", LimbType.Appendage, "tail", 0.5, 0.5, pinnipedProto);
        _context.SaveChanges();

        foreach (Limb limb in limbs.Values)
        {
            foreach (BodypartProto part in _cachedLimbs[limb.Name])
            {
                _context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
            }

            switch (limb.Name)
            {
                case "Torso":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["uspinalcord"] });
                    break;
                case "Genitals":
                case "Right Foreleg":
                case "Left Foreleg":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["mspinalcord"] });
                    break;
                case "Leg Hindleg":
                case "Right Hindleg":
                case "Tail":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["lspinalcord"] });
                    break;
            }
        }

        _context.SaveChanges();

        #endregion

        #region Groups

        AddBodypartGroupDescriberShape(pinnipedProto, "body", "The whole torso of a pinniped",
            ("abdomen", 1, 1),
            ("belly", 1, 1),
            ("withers", 1, 1),
            ("breast", 1, 2),
            ("flank", 1, 2),
            ("loin", 1, 2),
            ("shoulder", 1, 2),
            ("upper back", 1, 1),
            ("lower back", 1, 1),
            ("rump", 1, 2),
            ("neck", 0, 1),
            ("neck back", 0, 1),
            ("throat", 0, 1)
        );
        AddBodypartGroupDescriberShape(pinnipedProto, "flippers", "Four flippers of a pinniped",
            ("front flipper", 1, 2),
            ("hind flipper", 1, 2)
        );
        AddBodypartGroupDescriberShape(pinnipedProto, "front flippers", "Both front flippers of a pinniped",
            ("front flipper", 2, 2)
        );
        AddBodypartGroupDescriberShape(pinnipedProto, "hind flippers", "Both hind flippers of a pinniped",
            ("hind flipper", 2, 2)
        );

        AddBodypartGroupDescriberShape(pinnipedProto, "head", "A pinniped head",
            ("face", 1, 1),
            ("head back", 0, 1),
            ("eye socket", 0, 2),
            ("eye", 0, 2),
            ("ear", 0, 2),
            ("jaw", 0, 2),
            ("muzzle", 0, 1),
            ("nose", 0, 1),
            ("mouth", 0, 1),
            ("tongue", 0, 1),
            ("cheek", 0, 2),
            ("throat", 0, 1),
            ("withers", 0, 1),
            ("neck", 0, 1),
            ("neck back", 0, 1),
            ("tusk", 0, 2)
        );

        AddBodypartGroupDescriberShape(pinnipedProto, "back", "A pinniped back",
            ("upper back", 1, 1),
            ("lower back", 1, 1),
            ("flank", 0, 4),
            ("rump", 0, 2),
            ("withers", 0, 1),
            ("neck back", 0, 1)
        );

        AddBodypartGroupDescriberShape(pinnipedProto, "eyes", "A pair of pinniped eyes",
            ("eye socket", 2, 2),
            ("eye", 0, 2)
        );

        AddBodypartGroupDescriberShape(pinnipedProto, "ears", "A pair of pinniped ears",
            ("ear", 2, 2)
        );

        AddBodypartGroupDescriberShape(pinnipedProto, "tusks", "A pair of pinniped tusks",
            ("tusk", 2, 2)
        );

        AddBodypartGroupDescriberShape(pinnipedProto, "shoulders", "A group of pinniped shoulders",
            ("shoulder", 2, 4)
        );

        _context.SaveChanges();

        #endregion

        #endregion

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Cetaceans...");

        #region Cetaceans

        ResetCachedParts();
        order = 1;

        #region Torso

        AddBodypart(cetaceanProto, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null, Alignment.Front,
            Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "rbreast", "right breast", "breast", BodypartTypeEnum.Wear, "abdomen",
            Alignment.FrontRight, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "lbreast", "left breast", "breast", BodypartTypeEnum.Wear, "abdomen",
            Alignment.FrontLeft, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "urflank", "upper right flank", "flank", BodypartTypeEnum.Wear, "rbreast",
            Alignment.Right, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "ulflank", "upper left flank", "flank", BodypartTypeEnum.Wear, "lbreast",
            Alignment.Left, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "lrflank", "lower right flank", "flank", BodypartTypeEnum.Wear, "abdomen",
            Alignment.RearRight, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "llflank", "lower left flank", "flank", BodypartTypeEnum.Wear, "abdomen",
            Alignment.RearLeft, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "belly", "belly", "belly", BodypartTypeEnum.Wear, "abdomen", Alignment.Front,
            Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "uback", "upper back", "upper back", BodypartTypeEnum.Wear, "abdomen",
            Alignment.Front, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "lback", "lower back", "lower back", BodypartTypeEnum.Wear, "abdomen",
            Alignment.Rear, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "loin", "loin", "loin", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
            Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(cetaceanProto, "dorsalfin", "dorsal fin", "fin", BodypartTypeEnum.Fin, "uback", Alignment.Rear,
            Orientation.Highest, 20, 35, 100, order++, "Flesh", SizeCategory.Normal, "Torso");
        AddBodypart(cetaceanProto, "rpectoralfin", "right pectoral fin", "fin", BodypartTypeEnum.Fin, "urflank",
            Alignment.Right, Orientation.Centre, 20, 35, 100, order++, "Flesh", SizeCategory.Normal, "Torso");
        AddBodypart(cetaceanProto, "lpectoralfin", "left pectoral fin", "fin", BodypartTypeEnum.Fin, "ulflank",
            Alignment.Left, Orientation.Centre, 20, 35, 100, order++, "Flesh", SizeCategory.Normal, "Torso");

        #endregion

        #region Head

        AddBodypart(cetaceanProto, "neck", "neck", "neck", BodypartTypeEnum.Wear, "uback", Alignment.Front,
            Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(cetaceanProto, "head", "head", "face", BodypartTypeEnum.Wear, "neck", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(cetaceanProto, "blowhole", "blowhole", "blowhole", BodypartTypeEnum.Blowhole, "head",
            Alignment.Front,
            Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
            implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(cetaceanProto, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.Wear, "head",
            Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(cetaceanProto, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.Wear, "head",
            Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(cetaceanProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket", Alignment.FrontRight,
            Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(cetaceanProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket", Alignment.FrontLeft,
            Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(cetaceanProto, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "head", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);

        #endregion

        #region Tail

        AddBodypart(cetaceanProto, "stock", "tail stock", "tail", BodypartTypeEnum.Wear, "lback", Alignment.Rear,
            Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");
        AddBodypart(cetaceanProto, "fluke", "fluke", "tail", BodypartTypeEnum.Fin, "stock", Alignment.Rear,
            Orientation.Centre, 20, 35, 100, order++, "Flesh", SizeCategory.Normal, "Tail");

        #endregion

        _context.SaveChanges();

        #region Organs

        AddOrgan(cetaceanProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, stunModifier: 1.0);
        AddOrgan(cetaceanProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
        AddOrgan(cetaceanProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(cetaceanProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(cetaceanProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(cetaceanProto, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
            0.05);
        AddOrgan(cetaceanProto, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
            0.05);
        AddOrgan(cetaceanProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(cetaceanProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(cetaceanProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(cetaceanProto, "uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(cetaceanProto, "mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(cetaceanProto, "lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(cetaceanProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
        AddOrgan(cetaceanProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

        AddOrganCoverage("brain", "head", 100, true);
        AddOrganCoverage("brain", "reyesocket", 85);
        AddOrganCoverage("brain", "leyesocket", 85);
        AddOrganCoverage("brain", "reye", 85);
        AddOrganCoverage("brain", "leye", 85);

        AddOrganCoverage("linnerear", "head", 10, true);
        AddOrganCoverage("rinnerear", "head", 10, true);
        AddOrganCoverage("esophagus", "neck", 20, true);

        AddOrganCoverage("heart", "lbreast", 33, true);

        AddOrganCoverage("uspinalcord", "neck", 2, true);
        AddOrganCoverage("mspinalcord", "uback", 10, true);
        AddOrganCoverage("lspinalcord", "lback", 10, true);

        AddOrganCoverage("liver", "abdomen", 33, true);
        AddOrganCoverage("spleen", "abdomen", 20, true);
        AddOrganCoverage("stomach", "abdomen", 20, true);
        AddOrganCoverage("liver", "uback", 15);
        AddOrganCoverage("spleen", "uback", 10);
        AddOrganCoverage("stomach", "uback", 5);

        AddOrganCoverage("lintestines", "belly", 5, true);
        AddOrganCoverage("sintestines", "belly", 50, true);
        AddOrganCoverage("lintestines", "lback", 5);
        AddOrganCoverage("sintestines", "lback", 33);
        AddOrganCoverage("lintestines", "loin", 5);

        AddOrganCoverage("rkidney", "lback", 20, true);
        AddOrganCoverage("lkidney", "lback", 20, true);
        AddOrganCoverage("rkidney", "belly", 5);
        AddOrganCoverage("lkidney", "belly", 5);
        _context.SaveChanges();

        #endregion

        _context.SaveChanges();

        foreach ((BodypartProto? child, BodypartProto? parent) in _cachedBodypartUpstreams)
        {
            _context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
            {
                Child = child.Id,
                Parent = parent.Id
            });
        }

        _context.SaveChanges();

        #region Limbs

        limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);
        AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0, cetaceanProto);
        AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0, cetaceanProto);
        AddLimb("Tail", LimbType.Appendage, "stock", 0.5, 0.5, cetaceanProto);
        _context.SaveChanges();

        foreach (Limb limb in limbs.Values)
        {
            foreach (BodypartProto part in _cachedLimbs[limb.Name])
            {
                _context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
            }

            switch (limb.Name)
            {
                case "Torso":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["uspinalcord"] });
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["mspinalcord"] });
                    break;
                case "Tail":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["lspinalcord"] });
                    break;
            }
        }

        _context.SaveChanges();

        #endregion

        #region Groups

        AddBodypartGroupDescriberShape(cetaceanProto, "body", "The whole torso of a cetacean",
            ("abdomen", 0, 1),
            ("breast", 0, 2),
            ("flank", 0, 4),
            ("belly", 0, 1),
            ("loin", 0, 1),
            ("upper back", 1, 1),
            ("lower back", 1, 1),
            ("neck", 0, 1),
            ("blowhole", 0, 2),
            ("tail", 0, 2)
        );

        AddBodypartGroupDescriberShape(cetaceanProto, "fins", "All the fins of a cetacean",
            ("fin", 2, 3)
        );

        AddBodypartGroupDescriberDirect(cetaceanProto, "pectoral fins", "The pectoral fins of a cetacean",
            ("rpectoralfin", true),
            ("lpectoralfin", true)
        );

        AddBodypartGroupDescriberShape(cetaceanProto, "eyes", "The eyes of a cetacean",
            ("eye socket", 0, 2),
            ("eye", 0, 2)
        );

        AddBodypartGroupDescriberShape(cetaceanProto, "head", "The whole head of a cetacean",
            ("face", 0, 1),
            ("eye socket", 0, 2),
            ("eye", 0, 2),
            ("mouth", 0, 1),
            ("neck", 0, 1),
            ("blowhole", 0, 2)
        );

        AddBodypartGroupDescriberDirect(cetaceanProto, "tail", "The whole of a cetacean's tail",
            ("fluke", true),
            ("stock", true)
        );

        #endregion

        _context.SaveChanges();

        #endregion

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");

        #region Races

        SeedAnimalRaces(GetAquaticRaceTemplates(),
            ("Piscine", fishProto),
            ("Decapod", crabProto),
            ("Malacostracan", malacostracanProto),
            ("Cephalopod", octopusProto),
            ("Jellyfish", jellyfishProto),
            ("Pinniped", pinnipedProto),
            ("Cetacean", cetaceanProto));

        #endregion
    }
}
