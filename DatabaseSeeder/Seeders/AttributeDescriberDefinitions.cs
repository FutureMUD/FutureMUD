#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

internal static class AttributeDescriberDefinitions
{
	private static readonly (int High, string Text)[] HistoricalRanges =
	[
		(0, "Abysmal"), (3, "Terrible"), (6, "Bad"), (9, "Poor"), (11, "Average"),
		(13, "Good"), (15, "Great"), (17, "Excellent"), (20, "Super"), (23, "Epic"), (25, "Legendary")
	];

	// RankedRange selects the earlier interval at a shared boundary and extends the last interval upwards.
	private static readonly int[] UpperBounds =
	[
		30, 40, 55, 75, 100, 140, 190, 250, 350, 500, 750, 1000, int.MaxValue
	];

	internal static string General() => Build("General Attribute Range",
	[
		"Prodigious", "Phenomenal", "Formidable", "Tremendous", "Immense", "Monumental", "Colossal",
		"Titanic", "Mythic", "Demigodlike", "Godlike", "Transcendent", "Ineffable"
	]);

	internal static string LabMud(string attribute) => Build($"{attribute} Attribute Range", attribute switch
	{
		"Strength" =>
		[
			"Prodigious", "Herculean", "Trans-Herculean", "Towering", "Immense", "Monumental", "Colossal",
			"Titanic", "Worldshaking", "Demigodlike", "Godlike", "Worldbreaking", "Immeasurable"
		],
		"Dexterity" =>
		[
			"Deft", "Uncanny", "Preternatural", "Sublime", "Otherworldly", "Transcendent", "Celestial",
			"Empyrean", "Mythic", "Demigodlike", "Godlike", "Beyond Divine", "Ineffable"
		],
		"Constitution" =>
		[
			"Robust", "Stalwart", "Ironclad", "Adamantine", "Inexhaustible", "Deathless", "Eternal",
			"Primordial", "Mythic", "Demigodlike", "Godlike", "Beyond Divine", "Indestructible"
		],
		"Intelligence" =>
		[
			"Brilliant", "Genius", "Sage", "Profound", "Enlightened", "Transcendent", "Transhuman",
			"Cosmic", "Mythic", "Demigodlike", "Godlike", "Beyond Divine", "Ineffable"
		],
		"Willpower" =>
		[
			"Resolute", "Dauntless", "Indomitable", "Adamantine", "Inexorable", "Unconquerable", "Eternal",
			"Primordial", "Mythic", "Demigodlike", "Godlike", "Beyond Divine", "Absolute"
		],
		"Perception" =>
		[
			"Acute", "Piercing", "Uncanny", "Preternatural", "Oracular", "Revelatory", "All-Seeing",
			"Cosmic", "Mythic", "Demigodlike", "Godlike", "Beyond Divine", "Omniscient"
		],
		_ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, "No extended LabMUD vocabulary is defined.")
	});

	private static string Build(string name, string[] upperDescriptors)
	{
		if (upperDescriptors.Length != UpperBounds.Length)
		{
			throw new ArgumentException("Every upper attribute band must have a descriptor.", nameof(upperDescriptors));
		}

		var root = new XElement("ranges",
			new XAttribute("name", name),
			new XAttribute("prefix", string.Empty),
			new XAttribute("suffix", string.Empty),
			new XAttribute("colour_default", true),
			new XAttribute("colour_buffed", true),
			new XAttribute("colour_capped", false));
		var low = -25;
		foreach (var (high, text) in HistoricalRanges.Concat(UpperBounds.Zip(upperDescriptors, (high, text) => (high, text))))
		{
			root.Add(new XElement("range", new XAttribute("low", low), new XAttribute("high", high),
				new XAttribute("text", text)));
			low = high;
		}

		return root.ToString(SaveOptions.DisableFormatting);
	}
}
