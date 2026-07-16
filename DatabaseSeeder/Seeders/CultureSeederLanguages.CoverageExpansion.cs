#nullable enable

using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	internal static IReadOnlyDictionary<string, string[]> ExistingPackLanguageCoverageExpansionForTesting =>
		ModernLanguageCoverageExpansionMap
			.Concat(AntiquityLanguageCoverageExpansionMap)
			.Concat(RenaissanceEuropeLanguageCoverageExpansionMap)
			.ToDictionary(x => x.Key, x => x.Value, System.StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlyCollection<string> ExistingPackLanguageNamesExpansionForTesting =>
		ModernLanguageCoverageExpansion
			.Concat(AntiquityLanguageCoverageExpansion)
			.Concat(RenaissanceEuropeLanguageCoverageExpansion)
			.Select(x => x.Name)
			.ToArray();

	private void SeedModernLanguageCoverageExpansion()
	{
		SeedHistoricalLanguagePack(
			ModernLanguageCoverageExpansion,
			ModernScriptCoverageExpansion,
			ModernMutualIntelligibilityExpansion);

		AddMutualIntelligability("Norwegian", "Swedish", Difficulty.VeryEasy, true);
		AddMutualIntelligability("Norwegian", "Danish", Difficulty.Easy, true);
		AddMutualIntelligability("Swedish", "Danish", Difficulty.Easy, true);
		AddMutualIntelligability("Russian", "Ukranian", Difficulty.Hard, true);
		AddMutualIntelligability("Russian", "Belarusian", Difficulty.Hard, true);
		AddMutualIntelligability("Ukranian", "Belarusian", Difficulty.Easy, true);
		AddMutualIntelligability("Irish Gaelic", "Scottish Gaelic", Difficulty.Hard, true);
		AddMutualIntelligability("Spanish", "Portugese", Difficulty.Hard, true);
		AddMutualIntelligability("Spanish", "Italian", Difficulty.VeryHard, true);
		AddMutualIntelligability("French", "Italian", Difficulty.VeryHard, true);
	}

	private void SeedAntiquityLanguageCoverageExpansion()
	{
		SeedHistoricalLanguagePack(
			AntiquityLanguageCoverageExpansion,
			AntiquityScriptCoverageExpansion,
			AntiquityMutualIntelligibilityExpansion);
	}

	private void SeedRenaissanceEuropeLanguageCoverageExpansion()
	{
		SeedHistoricalLanguagePack(
			RenaissanceEuropeLanguageCoverageExpansion,
			RenaissanceEuropeScriptCoverageExpansion,
			[]);
	}

	private static readonly Dictionary<string, string[]> ModernLanguageCoverageExpansionMap =
		new(System.StringComparer.OrdinalIgnoreCase)
		{
			["Modern Indian"] = ["Tamil", "Telugu"],
			["Modern Turkic"] = ["Uyghur"],
			["Modern North African"] = ["Tashelhit", "Central Atlas Tamazight", "Kabyle"],
			["Modern Sub-Saharan"] = ["Yoruba", "Igbo", "Zulu", "Khoekhoe", "Ju'hoan"],
			["Modern Oceanic"] = ["Maori", "Samoan", "Fijian"],
			["Modern Southeast Asian"] = ["Vietnamese", "Khmer", "Thai", "Lao", "Javanese"],
			["Modern Indigenous Latin American"] =
				["Southern Quechua", "Central Quechua", "Kichwa", "Nahuatl", "Guarani"],
			["Modern Central Asian"] =
				["Mongolian", "Uyghur", "U-Tsang Tibetan", "Khams Tibetan", "Amdo Tibetan"],
			["Modern Aboriginal Australian"] = ["Warlpiri", "Pitjantjatjara", "Djambarrpuyngu"],
			["Ainu ethnicity (Modern Japanese names)"] = ["Hokkaido Ainu", "Sakhalin Ainu", "Kuril Ainu"]
		};

	private static readonly Dictionary<string, string[]> AntiquityLanguageCoverageExpansionMap =
		new(System.StringComparer.OrdinalIgnoreCase)
		{
			["Scythian-Sarmatian"] = ["Scythian", "Sarmatian"],
			["Kushite"] = ["Meroitic"]
		};

	private static readonly Dictionary<string, string[]> RenaissanceEuropeLanguageCoverageExpansionMap =
		new(System.StringComparer.OrdinalIgnoreCase)
		{
			["Albanian"] = ["Albanian"]
		};

	private static readonly HistoricalLanguageSeed[] ModernLanguageCoverageExpansion =
	[
		new("Tamil", "an unknown Dravidian language",
		[
			HistoricalAccent("Chennai", "northeastern Tamil", "The urban northeastern variety centred on Chennai.", Difficulty.Trivial),
			HistoricalAccent("Kongu", "western Tamil", "A western Tamil variety associated with the Kongu region."),
			HistoricalAccent("Jaffna", "Sri Lankan Tamil", "A conservative northern Sri Lankan Tamil variety.", Difficulty.Easy)
		]),
		new("Telugu", "an unknown Dravidian language",
		[
			HistoricalAccent("Coastal Andhra", "coastal Telugu", "The Telugu variety of the coastal Andhra districts.", Difficulty.Trivial),
			HistoricalAccent("Rayalaseema", "southern Telugu", "The southern inland variety associated with Rayalaseema."),
			HistoricalAccent("Telangana", "northwestern Telugu", "The northwestern Telugu variety associated with Telangana.")
		]),
		new("Vietnamese", "an unknown Vietic language",
		[
			HistoricalAccent("Northern", "northern Vietnamese", "The northern variety centred on the Red River delta.", Difficulty.Trivial),
			HistoricalAccent("North-Central", "north-central Vietnamese", "The north-central variety of the Thanh Hoa to Hue corridor.", Difficulty.Easy),
			HistoricalAccent("Southern", "southern Vietnamese", "The southern variety centred on the Mekong region.", Difficulty.Easy)
		]),
		new("Thai", "an unknown Kra-Dai language",
		[
			HistoricalAccent("Central", "central Thai", "The central Thai variety used around Bangkok and the central plain.", Difficulty.Trivial),
			HistoricalAccent("Northern", "northern Tai", "A northern Tai-influenced regional variety.", Difficulty.Easy),
			HistoricalAccent("Isan", "northeastern Tai", "The Lao-related northeastern variety commonly called Isan.", Difficulty.Easy),
			HistoricalAccent("Southern", "southern Thai", "The regional variety of peninsular southern Thailand.", Difficulty.Easy)
		]),
		new("Javanese", "an unknown Austronesian language",
		[
			HistoricalAccent("Central", "central Javanese", "The central court-region variety of Javanese.", Difficulty.Trivial),
			HistoricalAccent("Eastern", "eastern Javanese", "The eastern regional variety of Javanese."),
			HistoricalAccent("Banyumasan", "western Javanese", "The western Banyumasan variety, noted for retaining older features.", Difficulty.Easy)
		]),
		new("Khmer", "an unknown Austroasiatic language",
		[
			HistoricalAccent("Central", "central Khmer", "The central Khmer variety associated with Phnom Penh and the lower Mekong.", Difficulty.Trivial),
			HistoricalAccent("Northern", "northern Khmer", "A northern Khmer variety spoken towards the Dangrek region.", Difficulty.Easy),
			HistoricalAccent("Cardamom", "western Khmer", "A conservative western variety associated with the Cardamom Mountains.", Difficulty.Easy)
		]),
		new("Lao", "an unknown Southwestern Tai language",
		[
			HistoricalAccent("Vientiane", "central Lao", "The central Lao variety associated with Vientiane.", Difficulty.Trivial),
			HistoricalAccent("Luang Prabang", "northern Lao", "The northern Lao variety associated with Luang Prabang."),
			HistoricalAccent("Southern", "southern Lao", "The southern Lao variety of the lower Lao Mekong.", Difficulty.Easy)
		]),
		new("Maori", "an unknown Polynesian language",
		[
			HistoricalAccent("Northland", "northern Maori", "A northern Maori variety associated with Te Tai Tokerau.", Difficulty.Trivial),
			HistoricalAccent("Taranaki", "western Maori", "A western Maori variety associated with Taranaki."),
			HistoricalAccent("Ngai Tahu", "southern Maori", "The southern variety associated with Ngai Tahu.", Difficulty.Easy)
		]),
		new("Samoan", "an unknown Polynesian language",
		[
			HistoricalAccent("Upolu", "Upolu Samoan", "The variety associated with Upolu and the capital region.", Difficulty.Trivial),
			HistoricalAccent("Savaii", "Savaii Samoan", "The regional variety associated with Savaii."),
			HistoricalAccent("Formal", "formal Samoan", "The careful oratorical register used in formal settings.", Difficulty.Easy)
		]),
		new("Fijian", "an unknown Central Pacific language",
		[
			HistoricalAccent("Bauan", "eastern Fijian", "The Bauan variety that underlies the modern national standard.", Difficulty.Trivial),
			HistoricalAccent("Western", "western Fijian", "A western Fijian regional variety.", Difficulty.Easy),
			HistoricalAccent("Northern", "northern Fijian", "A northern island variety of Fijian.", Difficulty.Easy)
		]),
		new("Southern Quechua", "an unknown Quechuan language",
		[
			HistoricalAccent("Cusco", "Cusco Quechua", "The Southern Quechua variety associated with Cusco.", Difficulty.Trivial),
			HistoricalAccent("Ayacucho", "Ayacucho Quechua", "The Southern Quechua variety associated with Ayacucho.", Difficulty.Easy)
		]),
		new("Central Quechua", "an unknown Quechuan language",
		[
			HistoricalAccent("Huaylas", "Huaylas Quechua", "A Central Quechua variety of the Callejon de Huaylas.", Difficulty.Trivial),
			HistoricalAccent("Huanuco", "Huanuco Quechua", "A Central Quechua variety associated with Huanuco.", Difficulty.Easy)
		]),
		new("Kichwa", "an unknown Quechuan language",
		[
			HistoricalAccent("Highland", "highland Kichwa", "The Andean highland variety of Ecuadorian Kichwa.", Difficulty.Trivial),
			HistoricalAccent("Amazonian", "Amazonian Kichwa", "The Amazonian regional variety of Kichwa.", Difficulty.Easy)
		]),
		new("Nahuatl", "an unknown Uto-Aztecan language",
		[
			HistoricalAccent("Central", "central Nahuatl", "A central Mexican variety of Nahuatl.", Difficulty.Trivial),
			HistoricalAccent("Huasteca", "Huasteca Nahuatl", "The northeastern Huasteca variety."),
			HistoricalAccent("Guerrero", "Guerrero Nahuatl", "A southwestern variety associated with Guerrero.", Difficulty.Easy)
		]),
		new("Guarani", "an unknown Tupi-Guarani language",
		[
			HistoricalAccent("Paraguayan", "Paraguayan Guarani", "The widespread Paraguayan variety of Guarani.", Difficulty.Trivial),
			HistoricalAccent("Mbya", "Mbya Guarani", "The Mbya regional variety of Guarani.", Difficulty.Easy)
		]),
		new("Mongolian", "an unknown Mongolic language",
		[
			HistoricalAccent("Khalkha", "central Mongolian", "The central Khalkha variety that underlies the modern standard.", Difficulty.Trivial),
			HistoricalAccent("Chakhar", "southern Mongolian", "A southern Mongolian variety associated with Chakhar."),
			HistoricalAccent("Oirat", "western Mongolic", "A western Mongolic variety associated with Oirat communities.", Difficulty.Easy)
		]),
		new("Hokkaido Ainu", "an unknown Ainu language",
		[
			HistoricalAccent("Saru", "southern Hokkaido Ainu", "The Hokkaido Ainu variety associated with the Saru district.", Difficulty.Trivial),
			HistoricalAccent("Ishikari", "central Hokkaido Ainu", "The Hokkaido Ainu variety associated with the Ishikari basin."),
			HistoricalAccent("Tokachi", "eastern Hokkaido Ainu", "An eastern Hokkaido Ainu variety associated with Tokachi.", Difficulty.Easy)
		]),
		new("Sakhalin Ainu", "an unknown Ainu language",
		[
			HistoricalAccent("Western Sakhalin", "western Sakhalin Ainu", "A Sakhalin Ainu variety of the island's western coast.", Difficulty.Trivial),
			HistoricalAccent("Eastern Sakhalin", "eastern Sakhalin Ainu", "A Sakhalin Ainu variety of the island's eastern coast.", Difficulty.Easy)
		]),
		new("Kuril Ainu", "an unknown Ainu language",
		[
			HistoricalAccent("Northern Kuril", "northern Kuril Ainu", "The Ainu variety of the northern Kuril chain.", Difficulty.Trivial),
			HistoricalAccent("Southern Kuril", "southern Kuril Ainu", "The Ainu variety of the southern Kuril chain.", Difficulty.Easy)
		]),
		new("Uyghur", "an unknown Turkic language",
		[
			HistoricalAccent("Central", "central Uyghur", "The central variety associated with the Ili and Urumqi regions.", Difficulty.Trivial),
			HistoricalAccent("Hotan", "southern Uyghur", "The southern variety associated with Hotan."),
			HistoricalAccent("Lopnor", "eastern Uyghur", "The distinctive eastern variety associated with Lop Nur.", Difficulty.Easy)
		]),
		new("U-Tsang Tibetan", "an unknown Tibetic language",
		[
			HistoricalAccent("Lhasa", "central Tibetan", "The central U-Tsang variety associated with Lhasa.", Difficulty.Trivial),
			HistoricalAccent("Shigatse", "western U-Tsang", "The western U-Tsang variety associated with Shigatse.")
		]),
		new("Khams Tibetan", "an unknown Tibetic language",
		[
			HistoricalAccent("Central Kham", "central Khams", "A central Kham variety of Khams Tibetan.", Difficulty.Trivial),
			HistoricalAccent("Eastern Kham", "eastern Khams", "An eastern Kham variety of Khams Tibetan.", Difficulty.Easy)
		]),
		new("Amdo Tibetan", "an unknown Tibetic language",
		[
			HistoricalAccent("Northeastern", "northeastern Amdo", "The northeastern regional variety of Amdo Tibetan.", Difficulty.Trivial),
			HistoricalAccent("Southern", "southern Amdo", "A southern regional variety of Amdo Tibetan.", Difficulty.Easy)
		]),
		new("Tashelhit", "an unknown Amazigh language",
		[
			HistoricalAccent("Sous", "southern Tashelhit", "The Tashelhit variety of the Sous valley.", Difficulty.Trivial),
			HistoricalAccent("Anti-Atlas", "Anti-Atlas Tashelhit", "The southern mountain variety of the Anti-Atlas.")
		]),
		new("Central Atlas Tamazight", "an unknown Amazigh language",
		[
			HistoricalAccent("Middle Atlas", "Middle Atlas Tamazight", "A Central Atlas variety of the Middle Atlas.", Difficulty.Trivial),
			HistoricalAccent("Eastern High Atlas", "High Atlas Tamazight", "The eastern High Atlas regional variety.")
		]),
		new("Kabyle", "an unknown Amazigh language",
		[
			HistoricalAccent("Greater Kabylia", "western Kabyle", "The western Kabyle variety of Greater Kabylia.", Difficulty.Trivial),
			HistoricalAccent("Lesser Kabylia", "eastern Kabyle", "The eastern Kabyle variety of Lesser Kabylia.")
		]),
		new("Yoruba", "an unknown Volta-Niger language",
		[
			HistoricalAccent("Oyo", "northwestern Yoruba", "The northwestern Yoruba variety associated with Oyo.", Difficulty.Trivial),
			HistoricalAccent("Ijebu", "southeastern Yoruba", "The southeastern Yoruba variety associated with Ijebu."),
			HistoricalAccent("Ekiti", "northeastern Yoruba", "The northeastern Yoruba variety associated with Ekiti.")
		]),
		new("Igbo", "an unknown Volta-Niger language",
		[
			HistoricalAccent("Central", "central Igbo", "The central dialect cluster used by the modern literary standard.", Difficulty.Trivial),
			HistoricalAccent("Onitsha", "western Igbo", "The western variety associated with Onitsha."),
			HistoricalAccent("Owerri", "southern Igbo", "The southern variety associated with Owerri.")
		]),
		new("Zulu", "an unknown Nguni language",
		[
			HistoricalAccent("KwaZulu", "central Zulu", "The central KwaZulu variety of Zulu.", Difficulty.Trivial),
			HistoricalAccent("Northern", "northern Zulu", "A northern regional variety of Zulu."),
			HistoricalAccent("Urban", "urban Zulu", "An urban contact variety with wider South African influence.", Difficulty.Easy)
		]),
		new("Khoekhoe", "an unknown Khoe language",
		[
			HistoricalAccent("Nama", "Nama Khoekhoe", "The Khoekhoe variety associated with Nama communities.", Difficulty.Trivial),
			HistoricalAccent("Damara", "Damara Khoekhoe", "The Khoekhoe variety associated with Damara communities."),
			HistoricalAccent("Haiom", "northern Khoekhoe", "A northern Khoekhoe variety associated with Haiom communities.", Difficulty.Easy)
		]),
		new("Ju'hoan", "an unknown Kx'a language",
		[
			HistoricalAccent("Northern", "northern Ju'hoan", "The northern regional variety of Ju'hoan.", Difficulty.Trivial),
			HistoricalAccent("Southern", "southern Ju'hoan", "The southern regional variety of Ju'hoan.", Difficulty.Easy)
		]),
		new("Warlpiri", "an unknown Pama-Nyungan language",
		[
			HistoricalAccent("Yuendumu", "southern Warlpiri", "The Warlpiri variety associated with Yuendumu.", Difficulty.Trivial),
			HistoricalAccent("Lajamanu", "northern Warlpiri", "The Warlpiri variety associated with Lajamanu.")
		]),
		new("Pitjantjatjara", "an unknown Western Desert language",
		[
			HistoricalAccent("Pitjantjatjara", "western Western Desert", "The western Pitjantjatjara variety.", Difficulty.Trivial),
			HistoricalAccent("Yankunytjatjara", "eastern Western Desert", "The closely related eastern Yankunytjatjara variety.", Difficulty.Easy)
		]),
		new("Djambarrpuyngu", "an unknown Yolngu language",
		[
			HistoricalAccent("Dhuwa", "Dhuwa Yolngu", "A Dhuwa-moiety variety of Djambarrpuyngu.", Difficulty.Trivial),
			HistoricalAccent("Community", "community Djambarrpuyngu", "A contemporary inter-community variety.", Difficulty.Easy)
		])
	];

	private static readonly HistoricalScriptSeed[] ModernScriptCoverageExpansion =
	[
		new("Latin", "the Latin script", "a widespread alphabetic script",
			"The Latin alphabet is adapted to many modern languages through diacritics and additional letters.",
			"Alphabet", 1.0, 1.0,
			["Vietnamese", "Javanese", "Maori", "Samoan", "Fijian", "Southern Quechua", "Central Quechua", "Kichwa",
				"Nahuatl", "Guarani", "Tashelhit", "Central Atlas Tamazight", "Kabyle", "Yoruba", "Igbo", "Zulu",
				"Khoekhoe", "Ju'hoan", "Hokkaido Ainu", "Sakhalin Ainu", "Kuril Ainu", "Warlpiri", "Pitjantjatjara", "Djambarrpuyngu"]),
		new("Japanese", "the Japanese kana and kanji scripts", "an East Asian script",
			"Japanese writing combines kana syllabaries with kanji logographs; katakana is also used to write the modern Ainu languages.",
			"Hybrid", 0.75, 1.5, ["Hokkaido Ainu", "Sakhalin Ainu", "Kuril Ainu"]),
		new("Tamil", "the Tamil script", "a South Asian script",
			"The Tamil abugida is used to write Tamil in South India and Sri Lanka.",
			"Abugida", 0.9, 1.2, ["Tamil"]),
		new("Telugu", "the Telugu script", "a rounded South Asian script",
			"The Telugu abugida is used to write Telugu.",
			"Abugida", 0.9, 1.2, ["Telugu"]),
		new("Thai", "the Thai script", "a looping Southeast Asian script",
			"The Thai abugida writes Thai with consonant letters, dependent vowels and tone marks.",
			"Abugida", 0.9, 1.2, ["Thai"]),
		new("Khmer", "the Khmer script", "an ornate Southeast Asian script",
			"The Khmer abugida writes Khmer with consonant series and dependent vowels.",
			"Abugida", 0.9, 1.3, ["Khmer"]),
		new("Lao", "the Lao script", "a looping Southeast Asian script",
			"The Lao abugida writes Lao with consonant letters, dependent vowels and tone marks.",
			"Abugida", 0.9, 1.2, ["Lao"]),
		new("Javanese", "the Javanese script", "an ornate Southeast Asian script",
			"The traditional Javanese abugida, descended from Kawi, writes Javanese.",
			"Abugida", 0.8, 1.4, ["Javanese"]),
		new("Traditional Mongolian", "the traditional Mongolian script", "a vertical script",
			"Traditional Mongolian is written vertically in columns running from left to right.",
			"Alphabet", 0.9, 1.2, ["Mongolian"]),
		new("Arabic", "the Arabic script", "a flowing right-to-left script",
			"The Arabic script and its extended letters are used for Uyghur alongside other regional standards.",
			"Abjad", 0.8, 1.2, ["Uyghur"]),
		new("Tibetan", "the Tibetan script", "a Himalayan script",
			"The Tibetan abugida is shared across the major written Tibetic traditions.",
			"Abugida", 0.9, 1.3, ["U-Tsang Tibetan", "Khams Tibetan", "Amdo Tibetan"]),
		new("Tifinagh", "the Tifinagh script", "a geometric North African script",
			"Tifinagh is the consonantal script family historically associated with Amazigh languages and revived in modern standards.",
			"Abjad", 1.0, 1.0, ["Tashelhit", "Central Atlas Tamazight", "Kabyle"])
	];

	private static readonly HistoricalMutualIntelligibilitySeed[] ModernMutualIntelligibilityExpansion =
	[
		new("Southern Quechua", "Central Quechua", Difficulty.VeryHard),
		new("Southern Quechua", "Kichwa", Difficulty.VeryHard),
		new("Central Quechua", "Kichwa", Difficulty.Hard),
		new("U-Tsang Tibetan", "Khams Tibetan", Difficulty.Hard),
		new("U-Tsang Tibetan", "Amdo Tibetan", Difficulty.VeryHard),
		new("Khams Tibetan", "Amdo Tibetan", Difficulty.VeryHard),
		new("Tashelhit", "Central Atlas Tamazight", Difficulty.VeryHard),
		new("Tashelhit", "Kabyle", Difficulty.ExtremelyHard),
		new("Central Atlas Tamazight", "Kabyle", Difficulty.VeryHard)
	];

	private static readonly HistoricalLanguageSeed[] AntiquityLanguageCoverageExpansion =
	[
		new("Scythian", "an unknown Eastern Iranian language",
		[
			HistoricalAccent("Pontic", "western Scythian", "The Scythian variety associated with the Pontic steppe.", Difficulty.Trivial),
			HistoricalAccent("Royal", "eastern Scythian", "An eastern steppe variety associated with the Royal Scythians.", Difficulty.Easy)
		]),
		new("Sarmatian", "an unknown Eastern Iranian language",
		[
			HistoricalAccent("Sauromatian", "early Sarmatian", "An early Sarmatian variety associated with the lower Volga.", Difficulty.Trivial),
			HistoricalAccent("Roxolani", "western Sarmatian", "The western Sarmatian variety associated with the Roxolani."),
			HistoricalAccent("Alan", "eastern Sarmatian", "The eastern Sarmatian variety associated with the Alans.", Difficulty.Easy)
		]),
		new("Meroitic", "an unknown language of the Middle Nile",
		[
			HistoricalAccent("Meroe", "central Meroitic", "The court and urban variety associated with Meroe.", Difficulty.Trivial),
			HistoricalAccent("Napata", "northern Meroitic", "A northern Middle Nile variety associated with Napata.", Difficulty.Easy)
		])
	];

	private static readonly HistoricalScriptSeed[] AntiquityScriptCoverageExpansion =
	[
		new("Greek", "the Greek script", "an alphabetic Mediterranean script",
			"Greek letters were also used to record foreign names and short texts around the Black Sea.",
			"Alphabet", 1.0, 1.0, ["Scythian", "Sarmatian"]),
		new("Meroitic", "the Meroitic script", "an unfamiliar Nile Valley script",
			"The Meroitic alphasyllabary was used in the Kingdom of Kush in hieroglyphic and cursive forms.",
			"Alphasyllabary", 0.9, 1.2, ["Meroitic"])
	];

	private static readonly HistoricalMutualIntelligibilitySeed[] AntiquityMutualIntelligibilityExpansion =
	[
		new("Scythian", "Sarmatian", Difficulty.Hard)
	];

	private static readonly HistoricalLanguageSeed[] RenaissanceEuropeLanguageCoverageExpansion =
	[
		new("Albanian", "an unknown Albanian language",
		[
			HistoricalAccent("Gheg", "northern Albanian", "The northern Albanian dialect continuum associated with Gheg speakers.", Difficulty.Trivial),
			HistoricalAccent("Tosk", "southern Albanian", "The southern Albanian dialect continuum associated with Tosk speakers.", Difficulty.Easy),
			HistoricalAccent("Arberesh", "Italo-Albanian", "The Albanian variety carried to southern Italy by late-medieval migrants.", Difficulty.Easy)
		])
	];

	private static readonly HistoricalScriptSeed[] RenaissanceEuropeScriptCoverageExpansion =
	[
		new("Latin", "the Latin script", "a Western alphabetic script",
			"Latin letters were used for some of the earliest surviving Albanian writing in Catholic contexts.",
			"Alphabet", 1.0, 1.0, ["Albanian"]),
		new("Greek", "the Greek script", "an Eastern Mediterranean alphabet",
			"Greek letters were also used for Albanian in Orthodox and southern contexts.",
			"Alphabet", 1.0, 1.0, ["Albanian"])
	];
}
