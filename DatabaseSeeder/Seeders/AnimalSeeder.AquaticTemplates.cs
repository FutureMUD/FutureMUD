#nullable enable

using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.GameItems;

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
			bool canClimb = false)
		{
			return new AnimalRaceTemplate(
				name,
				adjective,
				null,
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
				}
			);
		}

		foreach (var fish in new[]
		{
			("Carp", SizeCategory.Small, 0.2, "Small Fish", "fish", "It is broad-backed and thick-scaled, with a blunt head and deep body.",
				"Its heavy flanks and slow powerful tailbeats suit muddy ponds and quiet rivers.", "It looks durable rather than fast.", "pond, river and silted water"),
			("Cod", SizeCategory.Small, 0.2, "Medium Fish", "fish", "It is pale-sided and thick-bodied, with a broad head and tapering tail.", "Its body is that of a practical cold-water hunter built to cruise and gulp.", "It hangs in the water with patient, practical intent.", "cold sea and offshore bank"),
			("Haddock", SizeCategory.Small, 0.2, "Medium Fish", "fish", "It is neat-bodied and silvery, marked in darker mottling along the back.", "Its fin shape and taut body suit active swimming through open water.", "It looks alert and constantly in motion.", "cool sea and coastal shelf"),
			("Koi", SizeCategory.Small, 0.2, "Small Fish", "fish", "It is deep-bodied and ornamental, its scales often bright and richly patterned.", "Its rounded shape and bold colouration make it stand out more than most fish would dare.", "It drifts with serene, showy confidence.", "garden pond and still water"),
			("Pilchard", SizeCategory.Small, 0.2, "Small Fish", "fish", "It is slim and silvery, built in the plain, efficient shape of a schooling fish.", "Its narrow body and shining flanks help it disappear among many of its own kind.", "It looks made for constant quick movement in company.", "coastal sea and schooling water"),
			("Perch", SizeCategory.Small, 0.2, "Small Fish", "fish", "It is neatly built, with a spined dorsal fin and barred body.", "Its profile looks half predator, half ambusher, ideal for striking from reed shade.", "It waits more than it rushes.", "lake, river and weed bed"),
			("Herring", SizeCategory.VerySmall, 0.1, "Small Fish", "fish", "It is narrow-bodied and silver bright, with a simple schooling silhouette.", "Its shape is all speed, synchrony and life spent among flashes of similar bodies.", "It looks nervous and constantly ready to turn as one with a shoal.", "open sea and cold coastal water"),
			("Mackerel", SizeCategory.VerySmall, 0.1, "Small Fish", "fish", "It is sleek and metallic, marked in dark bars over a silver body.", "Its torpedo shape and strong tail mark it as a relentless swimmer.", "It has the restless speed of something that never fully stops.", "open water and current"),
			("Anchovy", SizeCategory.VerySmall, 0.1, "Small Fish", "fish", "It is very small and slim, almost pure silver and motion.", "Its body is built around schooling, darting and avoiding bigger mouths.", "It seems little more than a quick bright line in the water.", "coastal water and estuary"),
			("Sardine", SizeCategory.VerySmall, 0.1, "Small Fish", "fish", "It is compact and silver-scaled, with a schooling fish's simple efficient body.", "Its flanks catch light readily, vanishing and reappearing with every turn.", "It looks born to move in a living glittering mass.", "coastal sea and schooling water"),
			("Pollock", SizeCategory.Small, 0.2, "Medium Fish", "fish", "It is long and pale, with a strong tail and deep mouth.", "Its build looks suited to active hunting in cool water rather than lurking on the bottom.", "It gives off a lean, hungry practicality.", "offshore water and cold sea"),
			("Salmon", SizeCategory.Small, 0.2, "Medium Fish", "fish", "It is sleek and powerful, with clean fins and a muscular body.", "Its shape is that of a tireless swimmer made for long distances and hard currents.", "It looks as though it could climb water itself if it had to.", "river mouth and migrating current"),
			("Tuna", SizeCategory.Normal, 0.8, "Large Fish", "fish", "It is thick and torpedo-shaped, all dense muscle and efficient fin.", "Its body seems built for speed, endurance and open-water pursuit.", "It exudes relentless forward momentum even while still.", "blue water and pelagic sea")
		})
		{
			yield return Aquatic(fish.Item1, fish.Item1, "Piscine", fish.Item2, fish.Item3, fish.Item4, fish.Item5,
				AquaticPack($"a {fish.Item1.ToLowerInvariant()} fry", $"a young {fish.Item1.ToLowerInvariant()}", $"a {fish.Item1.ToLowerInvariant()}",
					fish.Item6, fish.Item7, fish.Item8, fish.Item9),
				fish.Item1 switch
				{
					"Carp" => AnimalBreathingMode.Freshwater,
					"Koi" => AnimalBreathingMode.Freshwater,
					"Perch" => AnimalBreathingMode.Freshwater,
					"Salmon" => AnimalBreathingMode.Freshwater,
					_ => AnimalBreathingMode.Saltwater
				});
		}

		yield return Aquatic("Shark", "Shark", "Piscine", SizeCategory.Large, 1.5, "Shark", "shark",
			AquaticPack("a shark pup", "a young shark", "a shark",
				"It is sleek and immensely muscular, its body arranged around relentless forward movement.",
				"Its mouth of teeth and cold unblinking eyes leave no doubt that it is an apex predator.",
				"It carries itself with the remorseless calm of something that expects to eat what it catches.",
				"open sea and dark water"),
			AnimalBreathingMode.Saltwater);

		yield return Aquatic("Small Crab", "Small Crab", "Decapod", SizeCategory.VerySmall, 0.2, "Crab", "crab-small",
			AquaticPack("a tiny crab larva", "a young small crab", "a small crab",
				"It is a compact little crustacean, shell-backed and side-stepping on multiple jointed legs.",
				"Its claws are tiny but very real, and its body is mostly armour and irritation.",
				"It skitters with prickly sideways certainty.",
				"tidal rock, shoreline pool and shallow reef"),
			AnimalBreathingMode.Saltwater);
		yield return Aquatic("Crab", "Crab", "Decapod", SizeCategory.Small, 0.6, "Crab", "crab-large",
			AquaticPack("a crab larva", "a young crab", "a crab",
				"It is broad-shelled and low to the ground, with stalked eyes and heavy claws.",
				"Its carapace and stance make it look like a piece of hostile shoreline come to life.",
				"It skitters sideways with abrupt mechanical decisiveness.",
				"reef, mangrove and tidal flat"),
			AnimalBreathingMode.Saltwater);
		yield return Aquatic("Giant Crab", "Giant Crab", "Decapod", SizeCategory.Normal, 1.2, "Large Crab", "crab-giant",
			AquaticPack("a giant crab larva", "a young giant crab", "a giant crab",
				"It is enormous for a crab, all shell, claw and stubborn lateral movement.",
				"Its claws are large enough to be taken seriously by anything close enough to see them.",
				"It has the blunt confidence of a creature that trusts its armour.",
				"reef shelf and deep tidal cave"),
			AnimalBreathingMode.Saltwater);
		yield return Aquatic("Lobster", "Lobster", "Malacostracan", SizeCategory.Small, 0.6, "Crustacean", "crab-large",
			AquaticPack("a lobster larva", "a young lobster", "a lobster",
				"It is long-bodied and armoured, with heavy claws and a muscular tail fan.",
				"Its segmented abdomen and antennae give it a more elongate, primeval look than a crab.",
				"It creeps with slow, wary intent until startled into sudden retreat.",
				"reef crevice and rocky seabed"),
			AnimalBreathingMode.Saltwater);
		yield return Aquatic("Shrimp", "Shrimp", "Malacostracan", SizeCategory.VerySmall, 0.1, "Crustacean", "crab-small",
			AquaticPack("a shrimp larva", "a young shrimp", "a shrimp",
				"It is narrow, translucent and finely jointed, with long feelers and a curled body.",
				"Its tail and antennae make it look like a scrap of articulated motion more than a settled animal.",
				"It flicks backward through the water in sudden nervous bursts.",
				"reef, estuary and weed bed"),
			AnimalBreathingMode.Saltwater);
		yield return Aquatic("Prawn", "Prawn", "Malacostracan", SizeCategory.VerySmall, 0.15, "Crustacean", "crab-small",
			AquaticPack("a prawn larva", "a young prawn", "a prawn",
				"It is slender-bodied and semi-translucent, with long antennae and delicate claws.",
				"Its tail looks stronger and more active than the rest of its lightly armoured body.",
				"It appears ready to snap backward away from danger at any moment.",
				"estuary, mangrove and warm shallows"),
			AnimalBreathingMode.Saltwater);
		yield return Aquatic("Crayfish", "Crayfish", "Malacostracan", SizeCategory.VerySmall, 0.2, "Crustacean", "crab-small",
			AquaticPack("a crayfish larva", "a young crayfish", "a crayfish",
				"It is a small fresh-water crustacean with a shell-backed body, claws and curling tail.",
				"Its squat armour and feelers make it look like a tiny river-bottom bruiser.",
				"It creeps with stubborn purpose and sudden jerks of retreat.",
				"stream, creek bed and muddy bank"),
			AnimalBreathingMode.Freshwater);

		yield return Aquatic("Jellyfish", "Jellyfish", "Jellyfish", SizeCategory.Small, 0.1, "Jellyfish", "jellyfish",
			AquaticPack("a young jellyfish", "a young jellyfish", "a jellyfish",
				"It is little more than a translucent bell trailing fine tendrils beneath it.",
				"Its body is delicate-looking, but the drifting tentacles promise a sting out of all proportion to its substance.",
				"It pulses and drifts with eerie, indifferent grace.",
				"open water and current"),
			AnimalBreathingMode.Partless);

		yield return Aquatic("Octopus", "Octopus", "Cephalopod", SizeCategory.Small, 0.4, "Cephalopod", "cephalopod",
			AquaticPack("a young octopus", "a young octopus", "an octopus",
				"It is soft-bodied and watchful, with an intelligent eye and a ring of dexterous arms.",
				"Its suckered limbs and changing skin make it look too adaptable and too aware for comfort.",
				"It seems to think before it moves, and that alone is unsettling.",
				"reef, wreck and rocky sea floor"),
			AnimalBreathingMode.Saltwater, true);
		yield return Aquatic("Squid", "Squid", "Cephalopod", SizeCategory.Small, 0.4, "Cephalopod", "cephalopod",
			AquaticPack("a young squid", "a young squid", "a squid",
				"It is narrow-bodied and swift, with fins along the mantle and a crown of arms at the front.",
				"Its shape is that of a creature built for jetting bursts of speed and sudden turns.",
				"It always seems one moment away from vanishing into darker water.",
				"open sea and deep current"),
			AnimalBreathingMode.Saltwater);
		yield return Aquatic("Giant Squid", "Giant Squid", "Cephalopod", SizeCategory.Large, 1.0, "Giant Cephalopod", "cephalopod",
			AquaticPack("a young giant squid", "a young giant squid", "a giant squid",
				"It is an outsized cephalopod, all long mantle, heavy arms and impossible dark eyes.",
				"Its scale alone turns an already alien body plan into something bordering on monstrous.",
				"It suggests abyssal depth and cold water where light does not matter much.",
				"deep ocean and black water"),
			AnimalBreathingMode.Saltwater);

		yield return Aquatic("Sea Lion", "Sea Lion", "Pinniped", SizeCategory.Normal, 0.8, "Pinniped", "pinniped",
			AquaticPack("a sea lion pup", "a young sea lion", "a sea lion",
				"It is thick-bodied and sleek-coated, with strong foreflippers and a long whiskered muzzle.",
				"Its body speaks of a creature equally willing to haul out on rock or surge through surf.",
				"It has the loud self-assurance of a communal coastal animal.",
				"rocky shore and rolling surf"),
			AnimalBreathingMode.Blowhole);
		yield return Aquatic("Seal", "Seal", "Pinniped", SizeCategory.Normal, 0.8, "Pinniped", "pinniped",
			AquaticPack("a seal pup", "a young seal", "a seal",
				"It is smooth and streamlined, with huge dark eyes and whiskers around a neat muzzle.",
				"Its body is all blubber, grace and flipper-powered practicality.",
				"It looks soft until it begins to move, and then it becomes something fluid and efficient.",
				"ice edge, sandbar and cold coast"),
			AnimalBreathingMode.Blowhole);
		yield return Aquatic("Walrus", "Walrus", "Pinniped", SizeCategory.Large, 1.2, "Walrus", "pinniped",
			AquaticPack("a walrus calf", "a young walrus", "a walrus",
				"It is vast, whiskered and thick-skinned, with tusks curving down from a heavy face.",
				"Its blubbery body and tusks together make it seem both absurd and dangerous at once.",
				"It carries itself with ancient, cumbersome authority.",
				"ice floe and cold northern shore"),
			AnimalBreathingMode.Blowhole);

		yield return Aquatic("Dolphin", "Dolphin", "Cetacean", SizeCategory.Normal, 0.8, "Dolphin", "dolphin",
			AquaticPack("a dolphin calf", "a young dolphin", "a dolphin",
				"It is sleek and bright-skinned, with a curved mouthline and fluid muscular body.",
				"Its eyes and posture give it an impression of curiosity that most fish never approach.",
				"It moves with confident playful intelligence.",
				"open coastal water and warm current"),
			AnimalBreathingMode.Blowhole);
		yield return Aquatic("Porpoise", "Porpoise", "Cetacean", SizeCategory.Normal, 0.8, "Dolphin", "dolphin",
			AquaticPack("a porpoise calf", "a young porpoise", "a porpoise",
				"It is shorter and more compact than a dolphin, with a neat blunt head and dark smooth skin.",
				"Its body is streamlined without looking delicate, built for fast efficient movement through chill water.",
				"It surfaces and dives with understated assurance.",
				"coastal current and cool sea"),
			AnimalBreathingMode.Blowhole);
		yield return Aquatic("Orca", "Orca", "Cetacean", SizeCategory.Large, 1.6, "Small Whale", "orca",
			AquaticPack("an orca calf", "a young orca", "an orca",
				"It is black-and-white and powerfully built, with a massive jawline and heavy tail stock.",
				"Its markings are striking, but its size and confidence are what truly dominate the eye.",
				"It cuts through the water with top-predator certainty.",
				"cold current and open sea"),
			AnimalBreathingMode.Blowhole);
		yield return Aquatic("Baleen Whale", "Baleen Whale", "Cetacean", SizeCategory.VeryLarge, 2.0, "Large Whale", "baleen-whale",
			AquaticPack("a whale calf", "a young whale", "a baleen whale",
				"It is enormous, smooth-backed and impossibly heavy even by marine standards.",
				"Its size is its most striking feature, overwhelming any detail of fin, eye or jaw.",
				"It moves like weather given flesh.",
				"deep sea and migratory water"),
			AnimalBreathingMode.Blowhole);
		yield return Aquatic("Toothed Whale", "Toothed Whale", "Cetacean", SizeCategory.VeryLarge, 2.0, "Large Whale", "toothed-whale",
			AquaticPack("a whale calf", "a young whale", "a toothed whale",
				"It is huge and deep-bodied, with a blunt skull and muscular flukes.",
				"Its jaw and head proportions mark it as a hunter rather than a placid filter-feeder.",
				"It advances through the sea with grim, inexorable power.",
				"deep ocean and pelagic current"),
			AnimalBreathingMode.Blowhole);
	}
}
