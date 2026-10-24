#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace DatabaseSeeder.Seeders;

/// <summary>Player-facing setting text; stable ownership keys remain independent of presentation.</summary>
internal static class ArmageddonPresentation
{
	internal static bool Playable(string profile) => profile is "human" or "city_elf" or "desert_elf" or "dwarf" or "half_giant" or "mul";

	internal static string RaceDescription(string profile) => profile switch
	{
		"human" => "Humans are the most widespread people of Zalanthas, found among city dwellers, desert tribes, merchants and slaves. Their balanced abilities suit many ways of life, and their upbringing determines their native language.",
		"city_elf" => "City elves are tall, lean and quick, with pointed ears and close tribal loyalties shaped by life in the cities. They favour agility over strength, speak Allundean and Sirihish, and prize running over riding.",
		"desert_elf" => "Desert elves are tribal wanderers whose lives centre on the open wastes and the bonds of their people. Fleet and enduring but physically slight, they speak Allundean and have exceptional stamina for life in the desert.",
		"dwarf" => "Dwarves are short, broad and hairless, known for pursuing a personal focus with unwavering determination. They combine strength and endurance with a command of Mirukkim and Sirihish.",
		"half_elf" => "Half-elves stand between human and elven ancestry, often making their own place among people who regard them as outsiders. Their abilities blend both heritages, with a native language from their upbringing and some knowledge of Allundean.",
		"half_giant" => "Half-giants tower over the other city peoples and often find work where immense size and strength are valued. They have great physical power and stamina, balanced by limited agility and a formidable appetite.",
		"mul" => "Muls are the powerful, sterile offspring of humans and dwarves, often raised for heavy labour or the fighting pits. Strong and enduring, they have additional stamina and learn Mirukkim alongside the language of their upbringing.",
		_ => throw new ArgumentOutOfRangeException(nameof(profile), profile, "Unknown heritage profile.")
	};

	internal static string CultureDescription(string culture) => culture switch
	{
		"allanaki" => "Allanak is a hard southern city where nobles, templars, merchants and labourers live beneath the rule of its sorcerer-king. You speak with a Southern accent, with Sirihish as your native language unless your ancestry supplies another.",
		"labyrinth" => "The Labyrinth is Allanak's crowded northern slum, where close neighbours, street bargains and watchful eyes shape daily life. You speak with a Rinthi accent, with Sirihish as your native language unless your ancestry supplies another.",
		"tuluki" => "Tuluk's northern city life is shaped by its templars, social ranks and a taste for art and subtle expression. You speak with a Northern accent, with Sirihish as your native language unless your ancestry supplies another.",
		"nomadic" => "Human desert tribes make their homes beyond the cities, moving with the needs and traditions of their people. You begin speaking Bendune with a Tribal accent.",
		"desert_elven" => "Desert elven upbringing centres on tribal ties, long journeys on foot and survival in the wastes. You begin speaking Allundean with a Desert accent.",
		_ => throw new ArgumentOutOfRangeException(nameof(culture), culture, "Unknown upbringing.")
	};

	private static readonly IReadOnlyDictionary<string, string> RoleThemes = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		["Warrior"] = "Warriors make their living by force of arms, serving as soldiers, guards, mercenaries and pit fighters.",
		["Ranger"] = "Rangers live by their knowledge of the wastes, following tracks, finding food and guiding others through dangerous country.",
		["Merchant"] = "Merchants turn materials, craftsmanship and a keen eye for value into a living in the markets of Zalanthas.",
		["Assassin"] = "Assassins rely on patience, concealment and sudden violence to remove a target before open battle begins.",
		["Burglar"] = "Burglars slip into places others consider secure, using quiet movement and deft hands to reach their prize.",
		["Pick-pocket"] = "Pick-pockets make a living in crowds, taking valuables through stealth and sleight of hand.",
		["Psionicist"] = "Psionicists devote themselves to the Way, developing the mind as a tool for perception, influence and attack.",
		["Acrobat"] = "Acrobats train their balance and movement for performance, climbing and quick escapes.",
		["Archer"] = "Archers practise fighting at a distance with bow and arrow.",
		["Armormaker"] = "Armormakers turn tough materials into protective clothing and keep damaged armour serviceable.",
		["Bard"] = "Bards earn their keep through performance and a ready ear for the speech of others, learning languages readily.",
		["Caravan Guide"] = "Caravan guides lead travellers and pack animals along the trade routes of the wastes.",
		["Con Artist"] = "Con artists survive through deception, misdirection and an eye for an easy mark.",
		["Forester"] = "Foresters work the wooded lands, finding game and shaping harvested timber.",
		["General Crafter"] = "General crafters make everyday goods from a range of useful materials.",
		["House Servant"] = "House servants attend the practical needs of a household, from its kitchens to its private rooms.",
		["Hunter"] = "Hunters track game and turn a successful kill into food and useful materials.",
		["Jeweler"] = "Jewelers shape small, valuable materials into ornaments for trade and display.",
		["Linguist"] = "Linguists bridge the peoples of Zalanthas through the study of spoken languages.",
		["Mercenary"] = "Mercenaries sell their fighting skills to those who can pay, and clear alcohol and active drugs from their bodies more quickly.",
		["Nomad"] = "Nomads learn the practical skills of travelling and finding a living beyond city walls.",
		["Physician"] = "Physicians tend the injured and prepare remedies for those who can reach their care.",
		["Scavenger"] = "Scavengers search discarded goods and rough country for things worth keeping or selling.",
		["Stonecrafter"] = "Stonecrafters shape stone into useful goods in a world where metal is scarce.",
		["Subguild Guard"] = "Guards train to watch over people and property and to intervene when trouble begins.",
		["Subguild Rebel"] = "Rebels develop the rough, practical talents needed to resist authority and survive its attention.",
		["Subguild Tailor"] = "Tailors make and mend clothing, supplying the daily needs and fashions of city life.",
		["Thief"] = "Thieves supplement their livelihood with concealed movement and the taking of other people's property.",
		["Thug"] = "Thugs rely on intimidation and close violence to get their way in the streets.",
		["Tinker"] = "Tinkers make and repair small practical goods, turning simple materials into useful tools.",
		["Weaponcrafter"] = "Weaponcrafters fashion the arms carried by hunters, guards and soldiers.",
		["Jihaen Templar"] = "Jihaen templars are Tuluk's male warrior-priests, trained to enforce the city's authority through arms and discipline.",
		["Lirathan Templar"] = "Lirathan templars are Tuluk's female priestesses, cultivating the Way to uncover secrets and exert their authority.",
		["Southern Templar"] = "Southern templars wield Allanak's religious and civic authority in service to its sorcerer-king."
	};

	internal static string RoleDescription(string name, IEnumerable<string> openingSkills) =>
		RoleThemes[name] + " Starting skills include " + openingSkills.Distinct().Take(6).ListToString() + ".";

	internal static string MagicDescription(ArmageddonStockCapability capability, ArmageddonStockProgression stock)
	{
		var theme = capability.Name switch
		{
			"Earth" => "Earth elementalists draw upon stone and soil to shape the world around them.",
			"Fire" => "Fire elementalists channel heat and flame, turning a destructive force to their purposes.",
			"Water" => "Water elementalists command a substance more precious than coin in the thirsty world of Zalanthas.",
			"Wind" => "Wind elementalists call on the restless air for movement and power.",
			"Shadow" => "Shadow elementalists work with darkness and concealment, reaching beyond the safety of ordinary sight.",
			"Lightning" => "Lightning elementalists wield the sudden, violent energy of the storm.",
			"Void" => "Void elementalists reach into forces beyond the familiar substance of the world.",
			"Sorcerer" => "Sorcerers study magic across elemental boundaries, gathering the power to work their spells.",
			"Preserver" => "Preservers draw magical power while seeking to spare the life around them.",
			"Defiler" => "Defilers seize magical power from living surroundings, leaving damaged land in their wake.",
			_ => throw new ArgumentOutOfRangeException(nameof(capability))
		};
		var openings = stock.Bindings.Where(x => x.CapabilityKey == capability.Key && x.Starting && !x.Support)
			.Select(x => stock.SpellNames[x.ContentKey]).Distinct().Take(5).ToArray();
		return theme + " You begin with " + openings.ListToString() + " and develop further spells through practice.";
	}
}
