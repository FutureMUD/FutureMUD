#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private static IEnumerable<AnimalRaceTemplate> GetSerpentRaceTemplates()
    {
        static AnimalRaceTemplate Snake(string name, SizeCategory size, double health, string loadout, string description,
            string feature, string behaviour, string habitat, string combatStrategyKey = "Beast Clincher",
            string? raceDescription = null)
        {
            return new AnimalRaceTemplate(
                name,
                name,
                raceDescription ?? throw new InvalidOperationException($"Animal race {name} is missing a seeded description."),
                "Serpentine",
                size,
                string.Equals(name, "Tree Python", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "Anaconda", StringComparison.OrdinalIgnoreCase),
                health,
                "Serpent",
                "Serpent",
                "standard-mammal",
                loadout,
                SerpentPack($"a {name.ToLowerInvariant()} hatchling", $"a young {name.ToLowerInvariant()}", $"a {name.ToLowerInvariant()}",
                    description, feature, behaviour, habitat),
                false,
                AnimalBreathingMode.Simple,
                null,
                "serpent",
                null,
                combatStrategyKey
            );
        }

        yield return Snake("Python", SizeCategory.Small, 0.1, "serpent-constrictor",
            "It is heavy-bodied and beautifully patterned, its coils thick with latent strength.",
            "Its head is wedge-shaped and calm, giving away little of what those muscles can do.",
            "It carries itself with the slow confidence of a constrictor that trusts its body more than venom.",
            "forest floor and humid undergrowth",
            raceDescription: """
            In forest floors, scrub, branches, deserts, marshes and warm ruins, pythons fill the role of legless, scaled predators. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Watch one move and the emphasis falls on patient stillness followed by sudden, exact movement, a pattern that tells more than size alone.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Tree Python", SizeCategory.Normal, 0.4, "serpent-constrictor",
            "It is lithe and branch-coloured, its body patterned to vanish among leaves and bark.",
            "Its tail and balance suggest a serpent comfortable above the ground as well as on it.",
            "It coils with patient precision, as though every branch were already mapped in its head.",
            "branch, canopy and warm woodland",
            raceDescription: """
            Tree pythons occupy forest floors, scrub, branches, deserts, marshes and warm ruins as legless, scaled predators. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Watch one move and the emphasis falls on patient stillness followed by sudden, exact movement, a pattern that tells more than size alone.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Boa", SizeCategory.Normal, 0.6, "serpent-constrictor",
            "It is thick-bodied and muscular, patterned in warm browns that blur into mottled cover.",
            "Its body has the dense, deliberate strength of a snake made to hold struggling prey.",
            "It moves with unhurried certainty, wasting no motion.",
            "jungle floor and broken scrub",
            raceDescription: """
            Where forest floors, scrub, branches, deserts, marshes and warm ruins meets daily life, boas are recognisable as legless, scaled predators. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Watch one move and the emphasis falls on patient stillness followed by sudden, exact movement, a pattern that tells more than size alone.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Anaconda", SizeCategory.Large, 1.0, "serpent-constrictor",
            "It is huge and river-dark, its olive body marked by darker saddles and circles.",
            "Its immense girth gives it the oppressive presence of something built to overpower by sheer mass.",
            "It glides with awful patience, whether through water or mud.",
            "swamp, oxbow and flooded jungle",
            raceDescription: """
            Anacondas are legless, scaled predators of forest floors, scrub, branches, deserts, marshes and warm ruins. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Its strongest visual signs are smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Those features support patient stillness followed by sudden, exact movement, making the creature feel suited to its ground rather than merely placed there.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Cobra", SizeCategory.Small, 0.2, "serpent-neurotoxic",
            "It is slender and smooth-scaled, with a neck capable of spreading into a threatening hood.",
            "Its lifted posture and focused stare are as much a warning as its venom.",
            "It radiates poised hostility, ready to strike from a short violent distance.",
            "scrub, ruin and warm grassland",
            raceDescription: """
            In forest floors, scrub, branches, deserts, marshes and warm ruins, cobras fill the role of legless, scaled predators. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The eye is drawn to smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Those features support patient stillness followed by sudden, exact movement, making the creature feel suited to its ground rather than merely placed there.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Adder", SizeCategory.Small, 0.2, "serpent-hemotoxic",
            "It is short, thick and darkly patterned, with a triangular head and heavy body.",
            "Its build is compact and efficient, ideal for short-range ambush rather than pursuit.",
            "It lies still with the quiet, ugly confidence of a pitfall in living form.",
            "heath, rocky ground and cool scrub",
            raceDescription: """
            Adders occupy forest floors, scrub, branches, deserts, marshes and warm ruins as legless, scaled predators. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The most telling cues are smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Those features support patient stillness followed by sudden, exact movement, making the creature feel suited to its ground rather than merely placed there.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Rattlesnake", SizeCategory.Small, 0.2, "serpent-hemotoxic",
            "It is dusty-scaled and thick-bodied, with a broad head and a rattle at the end of its tail.",
            "Its tail advertises danger in a way few serpents ever bother to do.",
            "It looks like an animal willing to warn once and punish once.",
            "desert, scrub and dry canyon",
            raceDescription: """
            Where forest floors, scrub, branches, deserts, marshes and warm ruins meets daily life, rattlesnakes are recognisable as legless, scaled predators. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Most of its character sits in smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Those features support patient stillness followed by sudden, exact movement, making the creature feel suited to its ground rather than merely placed there.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Viper", SizeCategory.Small, 0.2, "serpent-cytotoxic",
            "It is broad-headed and rough-scaled, its body set in heavy looping curves.",
            "Its triangular skull and thick neck make the danger obvious at a glance.",
            "It remains still until movement matters, then seems all teeth and impact.",
            "rocky scrub and tangled brush",
            raceDescription: """
            Vipers are legless, scaled predators of forest floors, scrub, branches, deserts, marshes and warm ruins. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Look closer and the outline is all smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. In motion they are marked by patient stillness followed by sudden, exact movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. A viper can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Snake("Mamba", SizeCategory.Small, 0.2, "serpent-neurotoxic",
            "It is long, clean-scaled and unnervingly elegant, its body all speed and reach.",
            "Its narrow coffin-shaped head gives it a hard, severe silhouette.",
            "It has a tense, predatory quickness that suggests lightning-fast violence.",
            "dry woodland and warm savannah",
            "Beast Skirmisher",
            raceDescription: """
            In forest floors, scrub, branches, deserts, marshes and warm ruins, mambas fill the role of legless, scaled predators. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The first things to register are smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. In motion they are marked by patient stillness followed by sudden, exact movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. A mamba can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Snake("Coral Snake", SizeCategory.Small, 0.2, "serpent-neurotoxic",
            "It is slim and brilliantly banded, its scales arranged in warning colours too vivid to ignore.",
            "Its beauty feels deliberate, almost theatrical, like a banner for its own danger.",
            "It moves in a neat, purposeful line, calm in the confidence of its venom.",
            "leaf litter and warm woodland",
            raceDescription: """
            Coral snakes occupy forest floors, scrub, branches, deserts, marshes and warm ruins as legless, scaled predators. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Its profile is built around smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. In motion they are marked by patient stillness followed by sudden, exact movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. A coral snake can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Snake("Moccasin", SizeCategory.Small, 0.2, "serpent-hemotoxic",
            "It is dark and muscular, with a broad head and heavy body built for close wet-country ambush.",
            "Its thick neck and blunt face give it a solid, ugly forcefulness.",
            "It looks as though it would rather hold its ground than flee.",
            "swamp, marsh edge and dark water",
            raceDescription: """
            Where forest floors, scrub, branches, deserts, marshes and warm ruins meets daily life, moccasins are recognisable as legless, scaled predators. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The creature presents smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. In motion they are marked by patient stillness followed by sudden, exact movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. A moccasin can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
    }
}
