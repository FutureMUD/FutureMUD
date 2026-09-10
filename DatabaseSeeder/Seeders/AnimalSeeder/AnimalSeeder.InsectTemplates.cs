#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private static IEnumerable<AnimalRaceTemplate> GetInsectRaceTemplates()
    {
        static AnimalRaceTemplate Insect(
            string name,
            string bodyKey,
            SizeCategory size,
            double health,
            string model,
            string loadout,
            AnimalDescriptionPack pack,
            string description)
        {
            return new AnimalRaceTemplate(
                name,
                name,
                description,
                bodyKey,
                size,
                false,
                health,
                model,
                model,
                "arthropod",
                loadout,
                pack,
                false,
                AnimalBreathingMode.Insect,
                null,
                string.Equals(bodyKey, "Winged Insectoid", StringComparison.OrdinalIgnoreCase)
                    ? "winged-insectoid"
                    : "insectoid"
            );
        }

        yield return Insect("Ant", "Insectoid", SizeCategory.Tiny, 0.05, "Insect", "insect-mandible",
            InsectPack("an ant larva", "a young ant", "an ant",
                "It is tiny, hard-bodied and narrowly segmented, with strong little mandibles.",
                "Its antennae and purposeful gait make it look wholly given over to work.",
                "It hurries with the single-minded discipline of a colony creature.",
                "nest tunnel and colony mound"),
            description: """
            In nest tunnels, flowers, bark, grass, eaves, drains and rotting cover, ants fill the role of chitinous small arthropods. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by segmented bodies, antennae, mandibles, wings or springing legs according to the line. The impression is completed by busy, precise movement that can seem mindless or unnervingly purposeful, giving the animal a readable rhythm even before it acts.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a ant is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Beetle", "Beetle", SizeCategory.VerySmall, 0.1, "Insect", "bombardier-beetle",
            InsectPack("a beetle grub", "a young beetle", "a beetle",
                "It is armoured and compact, the body protected by hard shell and jointed legs.",
                "Its casing makes it seem more like a moving seed or stone than a vulnerable animal.",
                "It trundles with patient little determination.",
                "leaf litter, bark and rotten wood"),
            description: """
            Beetles occupy nest tunnels, flowers, bark, grass, eaves, drains and rotting cover as chitinous small arthropods. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by segmented bodies, antennae, mandibles, wings or springing legs according to the line. The impression is completed by busy, precise movement that can seem mindless or unnervingly purposeful, giving the animal a readable rhythm even before it acts.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a beetle is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Centipede", "Centipede", SizeCategory.VerySmall, 0.12, "Centipede", "insect-mandible",
            InsectPack("a centipede hatchling", "a young centipede", "a centipede",
                "It is long, low and many-legged, with a segmented body and restless venomous mandibles.",
                "Its rippling gait and twitching antennae make it look like a line of bad intentions given chitin.",
                "It scuttles in a smooth, unnerving rush that never seems to stop.",
                "stone crack, leaf litter and damp rot"),
            description: """
            Where nest tunnels, flowers, bark, grass, eaves, drains and rotting cover meets daily life, centipedes are recognisable as chitinous small arthropods. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by segmented bodies, antennae, mandibles, wings or springing legs according to the line. The impression is completed by busy, precise movement that can seem mindless or unnervingly purposeful, giving the animal a readable rhythm even before it acts.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a centipede is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Mantis", "Insectoid", SizeCategory.Small, 0.2, "Insect", "insect-mandible",
            InsectPack("a mantis nymph", "a young mantis", "a mantis",
                "It is narrow-bodied and green-limbed, with a triangular head and folded grasping forelegs.",
                "Its posture is so intent and predatory that it looks like a prayer made entirely of murder.",
                "It holds itself unnaturally still until something edible moves.",
                "leaf, stem and warm scrub"),
            description: """
            Mantises are chitinous small arthropods of nest tunnels, flowers, bark, grass, eaves, drains and rotting cover. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: segmented bodies, antennae, mandibles, wings or springing legs according to the line. Watch one move and the emphasis falls on busy, precise movement that can seem mindless or unnervingly purposeful, a pattern that tells more than size alone.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a mantis is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Dragonfly", "Winged Insectoid", SizeCategory.VerySmall, 0.1, "Winged Insect", "insect-mandible",
            InsectPack("a dragonfly nymph", "a young dragonfly", "a dragonfly",
                "It is long-bodied and fine-waisted, with large eyes and two pairs of transparent wings.",
                "Its huge eyes and whirring wings make it look built entirely around pursuit and balance.",
                "It hovers with an eerie precision before darting away like a thrown needle.",
                "pond edge and still water"),
            description: """
            In nest tunnels, flowers, bark, grass, eaves, drains and rotting cover, dragonflies fill the role of chitinous small arthropods. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: segmented bodies, antennae, mandibles, wings or springing legs according to the line. Watch one move and the emphasis falls on busy, precise movement that can seem mindless or unnervingly purposeful, a pattern that tells more than size alone.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a dragonfly is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Bee", "Winged Insectoid", SizeCategory.VerySmall, 0.1, "Winged Insect", "insect-stinger",
            InsectPack("a bee larva", "a young bee", "a bee",
                "It is fuzzy-bodied and striped, with busy wings and a compact abdomen.",
                "Its hind legs and pollen-dusted body make its relationship with flowers immediately obvious.",
                "It darts from bloom to bloom with humming urgency.",
                "flower patch and hive"),
            description: """
            Bees occupy nest tunnels, flowers, bark, grass, eaves, drains and rotting cover as chitinous small arthropods. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: segmented bodies, antennae, mandibles, wings or springing legs according to the line. Watch one move and the emphasis falls on busy, precise movement that can seem mindless or unnervingly purposeful, a pattern that tells more than size alone.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a bee is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Butterfly", "Winged Insectoid", SizeCategory.VerySmall, 0.1, "Winged Insect", "insect-mandible",
            InsectPack("a caterpillar", "a young butterfly", "a butterfly",
                "It is delicately winged and brightly patterned, with a narrow body and fine antennae.",
                "Its wings are all display and fragility, painted more boldly than a practical creature really should be.",
                "It flutters in erratic graceful arcs between rests.",
                "flowering meadow and garden edge"),
            description: """
            Where nest tunnels, flowers, bark, grass, eaves, drains and rotting cover meets daily life, butterflies are recognisable as chitinous small arthropods. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: segmented bodies, antennae, mandibles, wings or springing legs according to the line. Watch one move and the emphasis falls on busy, precise movement that can seem mindless or unnervingly purposeful, a pattern that tells more than size alone.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a butterfly is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Wasp", "Winged Insectoid", SizeCategory.VerySmall, 0.1, "Winged Insect", "insect-stinger",
            InsectPack("a wasp larva", "a young wasp", "a wasp",
                "It is narrow-waisted and sharply striped, with a harder, meaner look than a bee.",
                "Its polished body and obvious sting give it a brittle, weaponized elegance.",
                "It hangs in the air with aggressive focus.",
                "eaves, thicket and summer air"),
            description: """
            Wasps are chitinous small arthropods of nest tunnels, flowers, bark, grass, eaves, drains and rotting cover. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Its strongest visual signs are segmented bodies, antennae, mandibles, wings or springing legs according to the line. Those features support busy, precise movement that can seem mindless or unnervingly purposeful, making the creature feel suited to its ground rather than merely placed there.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a wasp is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Hornet", "Winged Insectoid", SizeCategory.VerySmall, 0.15, "Winged Insect", "insect-stinger",
            InsectPack("a hornet larva", "a young hornet", "a hornet",
                "It is larger and heavier than a common wasp, with a thick abdomen and loud wings.",
                "Its jaws and sting both look substantial enough to matter to things far bigger than itself.",
                "It hovers with audible threat and bad intent.",
                "woodland edge and paper nest"),
            description: """
            In nest tunnels, flowers, bark, grass, eaves, drains and rotting cover, hornets fill the role of chitinous small arthropods. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The eye is drawn to segmented bodies, antennae, mandibles, wings or springing legs according to the line. Those features support busy, precise movement that can seem mindless or unnervingly purposeful, making the creature feel suited to its ground rather than merely placed there.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a hornet is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Moth", "Winged Insectoid", SizeCategory.VerySmall, 0.08, "Winged Insect", "insect-mandible",
            InsectPack("a moth larva", "a young moth", "a moth",
                "It is soft-bodied and powder-winged, with feathered antennae and muted colours.",
                "Its wings look built more for drifting and fluttering than aggressive flight.",
                "It seems drawn toward light and warmth rather than conflict.",
                "lamplit eave and night garden"),
            description: """
            Moths occupy nest tunnels, flowers, bark, grass, eaves, drains and rotting cover as chitinous small arthropods. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The most telling cues are segmented bodies, antennae, mandibles, wings or springing legs according to the line. Those features support busy, precise movement that can seem mindless or unnervingly purposeful, making the creature feel suited to its ground rather than merely placed there.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a moth is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Grasshopper", "Insectoid", SizeCategory.VerySmall, 0.12, "Insect", "insect-mandible",
            InsectPack("a grasshopper nymph", "a young grasshopper", "a grasshopper",
                "It is long-legged and green-brown, the hindlegs built like springs beneath a light body.",
                "Its folded limbs make it look perpetually compressed and ready to launch.",
                "It pauses only briefly before another hopping burst.",
                "grass, scrub and warm field"),
            description: """
            Where nest tunnels, flowers, bark, grass, eaves, drains and rotting cover meets daily life, grasshoppers are recognisable as chitinous small arthropods. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Most of its character sits in segmented bodies, antennae, mandibles, wings or springing legs according to the line. Those features support busy, precise movement that can seem mindless or unnervingly purposeful, making the creature feel suited to its ground rather than merely placed there.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. For characters nearby, a grasshopper is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Insect("Cockroach", "Insectoid", SizeCategory.VerySmall, 0.1, "Insect", "insect-mandible",
            InsectPack("a roach nymph", "a young roach", "a cockroach",
                "It is broad-backed and dark, with spined legs and a flattened armoured body.",
                "Its shape is unlovely but undeniably effective, built to survive neglect and violence alike.",
                "It runs with fast, ugly persistence.",
                "crack, drain and ruined corner"),
            description: """
            Cockroaches are chitinous small arthropods of nest tunnels, flowers, bark, grass, eaves, drains and rotting cover. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Look closer and the outline is all segmented bodies, antennae, mandibles, wings or springing legs according to the line. In motion they are marked by busy, precise movement that can seem mindless or unnervingly purposeful, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are small enough to overlook and numerous enough to matter, shaping harvests, homes and old fears. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
    }
}
