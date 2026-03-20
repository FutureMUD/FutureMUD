#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private static IEnumerable<AnimalRaceTemplate> GetSerpentRaceTemplates()
	{
		static AnimalRaceTemplate Snake(string name, SizeCategory size, double health, string loadout, string description,
			string feature, string behaviour, string habitat, string combatStrategyKey = "Beast Clincher")
		{
			return new AnimalRaceTemplate(
				name,
				name,
				null,
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
			"forest floor and humid undergrowth");
		yield return Snake("Tree Python", SizeCategory.Normal, 0.4, "serpent-constrictor",
			"It is lithe and branch-coloured, its body patterned to vanish among leaves and bark.",
			"Its tail and balance suggest a serpent comfortable above the ground as well as on it.",
			"It coils with patient precision, as though every branch were already mapped in its head.",
			"branch, canopy and warm woodland");
		yield return Snake("Boa", SizeCategory.Normal, 0.6, "serpent-constrictor",
			"It is thick-bodied and muscular, patterned in warm browns that blur into mottled cover.",
			"Its body has the dense, deliberate strength of a snake made to hold struggling prey.",
			"It moves with unhurried certainty, wasting no motion.",
			"jungle floor and broken scrub");
		yield return Snake("Anaconda", SizeCategory.Large, 1.0, "serpent-constrictor",
			"It is huge and river-dark, its olive body marked by darker saddles and circles.",
			"Its immense girth gives it the oppressive presence of something built to overpower by sheer mass.",
			"It glides with awful patience, whether through water or mud.",
			"swamp, oxbow and flooded jungle");
		yield return Snake("Cobra", SizeCategory.Small, 0.2, "serpent-neurotoxic",
			"It is slender and smooth-scaled, with a neck capable of spreading into a threatening hood.",
			"Its lifted posture and focused stare are as much a warning as its venom.",
			"It radiates poised hostility, ready to strike from a short violent distance.",
			"scrub, ruin and warm grassland");
		yield return Snake("Adder", SizeCategory.Small, 0.2, "serpent-hemotoxic",
			"It is short, thick and darkly patterned, with a triangular head and heavy body.",
			"Its build is compact and efficient, ideal for short-range ambush rather than pursuit.",
			"It lies still with the quiet, ugly confidence of a pitfall in living form.",
			"heath, rocky ground and cool scrub");
		yield return Snake("Rattlesnake", SizeCategory.Small, 0.2, "serpent-hemotoxic",
			"It is dusty-scaled and thick-bodied, with a broad head and a rattle at the end of its tail.",
			"Its tail advertises danger in a way few serpents ever bother to do.",
			"It looks like an animal willing to warn once and punish once.",
			"desert, scrub and dry canyon");
		yield return Snake("Viper", SizeCategory.Small, 0.2, "serpent-cytotoxic",
			"It is broad-headed and rough-scaled, its body set in heavy looping curves.",
			"Its triangular skull and thick neck make the danger obvious at a glance.",
			"It remains still until movement matters, then seems all teeth and impact.",
			"rocky scrub and tangled brush");
		yield return Snake("Mamba", SizeCategory.Small, 0.2, "serpent-neurotoxic",
			"It is long, clean-scaled and unnervingly elegant, its body all speed and reach.",
			"Its narrow coffin-shaped head gives it a hard, severe silhouette.",
			"It has a tense, predatory quickness that suggests lightning-fast violence.",
			"dry woodland and warm savannah",
			"Beast Skirmisher");
		yield return Snake("Coral Snake", SizeCategory.Small, 0.2, "serpent-neurotoxic",
			"It is slim and brilliantly banded, its scales arranged in warning colours too vivid to ignore.",
			"Its beauty feels deliberate, almost theatrical, like a banner for its own danger.",
			"It moves in a neat, purposeful line, calm in the confidence of its venom.",
			"leaf litter and warm woodland");
		yield return Snake("Moccasin", SizeCategory.Small, 0.2, "serpent-hemotoxic",
			"It is dark and muscular, with a broad head and heavy body built for close wet-country ambush.",
			"Its thick neck and blunt face give it a solid, ugly forcefulness.",
			"It looks as though it would rather hold its ground than flee.",
			"swamp, marsh edge and dark water");
	}
}
