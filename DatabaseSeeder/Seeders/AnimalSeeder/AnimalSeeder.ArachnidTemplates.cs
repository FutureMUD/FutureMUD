#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private static IEnumerable<AnimalRaceTemplate> GetArachnidRaceTemplates()
    {
        static AnimalRaceTemplate Arthropod(
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
                string.Equals(bodyKey, "Scorpion", StringComparison.OrdinalIgnoreCase) ? "scorpion" : "arachnid"
            );
        }

        yield return Arthropod("Spider", "Arachnid", SizeCategory.VerySmall, 0.08, "Arachnid", "spider-venomous",
            InsectPack("a spiderling", "a young spider", "a spider",
                "It is a many-legged little hunter, all delicate limbs and poised body.",
                "Its fangs and dark clustered eyes lend a sinister edge to an otherwise tiny frame.",
                "It waits with taut stillness until movement invites violence.",
                "web, wall corner and dark crevice"),
            description: """
            In webs, stone cracks, burrows, warm scrub and dry ruins, spiders fill the role of many-legged arachnids. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The first things to register are jointed legs, hard or hairy bodies, clustered eyes, fangs, claws or arched stingers. In motion they are marked by taut stillness, deliberate menace and sudden predatory commitment, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They make dark corners and loose stones feel occupied, whether as minor pests or genuine hazards. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Arthropod("Tarantula", "Arachnid", SizeCategory.Small, 0.25, "Arachnid", "tarantula",
            InsectPack("a tarantula spiderling", "a young tarantula", "a tarantula",
                "It is thick-bodied and hairy, with a heavy abdomen and surprisingly substantial legs.",
                "Its sheer size for a spider makes the fangs and clustered eyes much harder to dismiss.",
                "It advances slowly, with a confidence that borders on contempt.",
                "burrow, scrub and warm stone"),
            description: """
            Tarantulas occupy webs, stone cracks, burrows, warm scrub and dry ruins as many-legged arachnids. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Its profile is built around jointed legs, hard or hairy bodies, clustered eyes, fangs, claws or arched stingers. In motion they are marked by taut stillness, deliberate menace and sudden predatory commitment, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They make dark corners and loose stones feel occupied, whether as minor pests or genuine hazards. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Arthropod("Scorpion", "Scorpion", SizeCategory.Small, 0.3, "Scorpion", "scorpion",
            InsectPack("a young scorpion", "a young scorpion", "a scorpion",
                "It is armoured and low to the ground, with grasping claws and a curved tail arched over the body.",
                "The sting held over its back gives it a constant look of loaded malice.",
                "It creeps with deliberate menace, every movement measured and ready.",
                "rock crack, desert floor and dry ruin"),
            description: """
            Where webs, stone cracks, burrows, warm scrub and dry ruins meets daily life, scorpions are recognisable as many-legged arachnids. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The creature presents jointed legs, hard or hairy bodies, clustered eyes, fangs, claws or arched stingers. In motion they are marked by taut stillness, deliberate menace and sudden predatory commitment, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They make dark corners and loose stones feel occupied, whether as minor pests or genuine hazards. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
    }
}
