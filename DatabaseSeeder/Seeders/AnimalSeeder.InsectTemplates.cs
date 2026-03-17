#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.GameItems;

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
			AnimalDescriptionPack pack)
		{
			return new AnimalRaceTemplate(
				name,
				name,
				null,
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
				"nest tunnel and colony mound"));
		yield return Insect("Beetle", "Insectoid", SizeCategory.VerySmall, 0.1, "Insect", "bombardier-beetle",
			InsectPack("a beetle grub", "a young beetle", "a beetle",
				"It is armoured and compact, the body protected by hard shell and jointed legs.",
				"Its casing makes it seem more like a moving seed or stone than a vulnerable animal.",
				"It trundles with patient little determination.",
				"leaf litter, bark and rotten wood"));
		yield return Insect("Mantis", "Insectoid", SizeCategory.Small, 0.2, "Insect", "insect-mandible",
			InsectPack("a mantis nymph", "a young mantis", "a mantis",
				"It is narrow-bodied and green-limbed, with a triangular head and folded grasping forelegs.",
				"Its posture is so intent and predatory that it looks like a prayer made entirely of murder.",
				"It holds itself unnaturally still until something edible moves.",
				"leaf, stem and warm scrub"));
		yield return Insect("Dragonfly", "Winged Insectoid", SizeCategory.VerySmall, 0.1, "Winged Insect", "insect-mandible",
			InsectPack("a dragonfly nymph", "a young dragonfly", "a dragonfly",
				"It is long-bodied and fine-waisted, with large eyes and two pairs of transparent wings.",
				"Its huge eyes and whirring wings make it look built entirely around pursuit and balance.",
				"It hovers with an eerie precision before darting away like a thrown needle.",
				"pond edge and still water"));
		yield return Insect("Bee", "Winged Insectoid", SizeCategory.VerySmall, 0.1, "Winged Insect", "insect-stinger",
			InsectPack("a bee larva", "a young bee", "a bee",
				"It is fuzzy-bodied and striped, with busy wings and a compact abdomen.",
				"Its hind legs and pollen-dusted body make its relationship with flowers immediately obvious.",
				"It darts from bloom to bloom with humming urgency.",
				"flower patch and hive"));
		yield return Insect("Butterfly", "Winged Insectoid", SizeCategory.VerySmall, 0.1, "Winged Insect", "insect-mandible",
			InsectPack("a caterpillar", "a young butterfly", "a butterfly",
				"It is delicately winged and brightly patterned, with a narrow body and fine antennae.",
				"Its wings are all display and fragility, painted more boldly than a practical creature really should be.",
				"It flutters in erratic graceful arcs between rests.",
				"flowering meadow and garden edge"));
		yield return Insect("Wasp", "Winged Insectoid", SizeCategory.VerySmall, 0.1, "Winged Insect", "insect-stinger",
			InsectPack("a wasp larva", "a young wasp", "a wasp",
				"It is narrow-waisted and sharply striped, with a harder, meaner look than a bee.",
				"Its polished body and obvious sting give it a brittle, weaponized elegance.",
				"It hangs in the air with aggressive focus.",
				"eaves, thicket and summer air"));
		yield return Insect("Hornet", "Winged Insectoid", SizeCategory.VerySmall, 0.15, "Winged Insect", "insect-stinger",
			InsectPack("a hornet larva", "a young hornet", "a hornet",
				"It is larger and heavier than a common wasp, with a thick abdomen and loud wings.",
				"Its jaws and sting both look substantial enough to matter to things far bigger than itself.",
				"It hovers with audible threat and bad intent.",
				"woodland edge and paper nest"));
		yield return Insect("Moth", "Winged Insectoid", SizeCategory.VerySmall, 0.08, "Winged Insect", "insect-mandible",
			InsectPack("a moth larva", "a young moth", "a moth",
				"It is soft-bodied and powder-winged, with feathered antennae and muted colours.",
				"Its wings look built more for drifting and fluttering than aggressive flight.",
				"It seems drawn toward light and warmth rather than conflict.",
				"lamplit eave and night garden"));
		yield return Insect("Grasshopper", "Insectoid", SizeCategory.VerySmall, 0.12, "Insect", "insect-mandible",
			InsectPack("a grasshopper nymph", "a young grasshopper", "a grasshopper",
				"It is long-legged and green-brown, the hindlegs built like springs beneath a light body.",
				"Its folded limbs make it look perpetually compressed and ready to launch.",
				"It pauses only briefly before another hopping burst.",
				"grass, scrub and warm field"));
		yield return Insect("Cockroach", "Insectoid", SizeCategory.VerySmall, 0.1, "Insect", "insect-mandible",
			InsectPack("a roach nymph", "a young roach", "a cockroach",
				"It is broad-backed and dark, with spined legs and a flattened armoured body.",
				"Its shape is unlovely but undeniably effective, built to survive neglect and violence alike.",
				"It runs with fast, ugly persistence.",
				"crack, drain and ruined corner"));
	}
}
