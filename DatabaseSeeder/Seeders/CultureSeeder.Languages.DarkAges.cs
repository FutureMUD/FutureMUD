#nullable enable

using MudSharp.RPG.Checks;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private static readonly IReadOnlyDictionary<string, string[]> DarkAgesAndMedievalLanguageCoverage =
		new Dictionary<string, string[]>
		{
			["Medieval Early Anglo-Saxon"] = ["Old English", "Latin"],
			["Medieval Anglo-Danish"] = ["Old English", "Old Norse", "Latin"],
			["Medieval Norse"] = ["Old Norse", "Latin"],
			["Medieval Norman"] = ["Old Norman", "Old French", "Latin"],
			["Medieval Anglo-Norman"] = ["Anglo-Norman", "Middle English", "Latin"],
			["Medieval High English British"] = ["Middle English", "Anglo-Norman", "Latin"],
			["Medieval Irish Gaelic"] = ["Medieval Gaelic", "Latin"],
			["Medieval Scottish Gaelic Lowland"] = ["Medieval Gaelic", "Middle English", "Latin"],
			["Medieval Carolingian Frankish"] = ["Old High German", "Old Low Franconian", "Latin"],
			["Medieval Capetian French"] = ["Old French", "Latin"],
			["Medieval Imperial German"] = ["Middle High German", "Middle Low German", "Latin"],
			["Medieval Christian Iberian"] = ["Old Castilian", "Old Leonese", "Old Aragonese", "Galician-Portuguese", "Old Catalan", "Latin"],
			["Medieval Andalusian Arabic"] = ["Andalusi Arabic", "Quranic Arabic"],
			["Medieval Byzantine Greek"] = ["Medieval Greek", "Koine Greek"],
			["Medieval Abbasid Arabic"] = ["Mashriqi Arabic", "Quranic Arabic", "Persian"],
			["Medieval Fatimid Arabic"] = ["Mashriqi Arabic", "Quranic Arabic"],
			["Medieval Seljuk Ayyubid Mamluk"] = ["Oghuz Turkic", "Persian", "Mashriqi Arabic", "Quranic Arabic"],
			["Medieval Magyar"] = ["Old Hungarian", "Latin"],
			["Medieval Rus Novgorod"] = ["Old East Slavic", "Church Slavonic"],
			["Medieval Steppe Turkic Mongol"] = ["Cuman-Kipchak", "Middle Mongol"],
			["Medieval North Indian Rajput"] = ["Western Apabhramsha", "Sanskrit"],
			["Medieval South Indian Chola"] = ["Medieval Tamil", "Sanskrit"],
			["Medieval Song Chinese"] = ["Late Middle Chinese", "Literary Chinese"],
			["Medieval Goryeo Korean"] = ["Early Middle Korean", "Literary Chinese"],
			["Medieval Heian Kamakura Japanese"] = ["Medieval Japanese", "Literary Chinese"]
		};

	private static readonly HistoricalLanguageSeed[] DarkAgesAndMedievalLanguages =
	[
		new("Latin", "an unknown learned Italic language",
		[
			HistoricalAccent("Carolingian", "Medieval Latin", "The careful Latin of Carolingian schools, courts and scriptoria.", Difficulty.Trivial),
			HistoricalAccent("Insular", "Medieval Latin", "The Latin pronunciation tradition of Britain and Ireland.", Difficulty.VeryEasy),
			HistoricalAccent("Iberian Medieval", "Medieval Latin", "The Latin pronunciation tradition of the medieval Iberian kingdoms.", Difficulty.VeryEasy)
		]),
		new("Old English", "an unknown West Germanic language",
		[
			HistoricalAccent("West Saxon", "Southern Old English", "The southern variety that became the principal late Old English literary standard.", Difficulty.Trivial),
			HistoricalAccent("Mercian", "Anglian Old English", "The Old English variety of the Midlands and much of the Anglo-Danish borderland."),
			HistoricalAccent("Northumbrian", "Anglian Old English", "The northern Old English variety spoken from the Humber into southern Scotland."),
			HistoricalAccent("Kentish", "Southern Old English", "The distinctive southeastern Old English variety of Kent.", Difficulty.Easy)
		]),
		new("Old Norse", "an unknown North Germanic language",
		[
			HistoricalAccent("Old West Norse", "West Norse", "The western Norse continuum of Norway and the North Atlantic.", Difficulty.Trivial),
			HistoricalAccent("Old East Norse", "East Norse", "The eastern Norse continuum of Denmark and Sweden."),
			HistoricalAccent("Norse-Gaelic", "Contact Norse", "A Norse variety shaped by sustained contact in Ireland, the Isles and western Britain.", Difficulty.Easy)
		]),
		new("Old Norman", "an unknown medieval Romance language",
		[
			HistoricalAccent("Continental Norman", "Norman", "The northern Romance variety of ducal Normandy.", Difficulty.Trivial),
			HistoricalAccent("Channel Norman", "Norman", "The insular Norman variety of the Channel Islands.")
		]),
		new("Anglo-Norman", "an unknown medieval Romance language",
		[
			HistoricalAccent("Insular Court", "Anglo-Norman", "The prestigious French of aristocratic and administrative life in England.", Difficulty.Trivial),
			HistoricalAccent("Western Insular", "Anglo-Norman", "An Anglo-Norman variety of western Britain shaped by stronger contact with English and Welsh."),
			HistoricalAccent("Hiberno-Norman", "Anglo-Norman", "The Anglo-Norman variety carried to lordships in Ireland.", Difficulty.Easy)
		]),
		new("Old French", "an unknown medieval Romance language",
		[
			HistoricalAccent("Francien", "Oïl", "The central Old French variety associated with the Capetian domain.", Difficulty.Trivial),
			HistoricalAccent("Picard", "Oïl", "The northern Oïl variety of Picardy."),
			HistoricalAccent("Champenois", "Oïl", "The eastern Oïl variety of Champagne."),
			HistoricalAccent("Burgundian", "Oïl", "The southeastern Oïl variety of Burgundy.", Difficulty.Easy)
		]),
		new("Middle English", "an unknown West Germanic language",
		[
			HistoricalAccent("East Midlands", "Midlands Middle English", "The influential Middle English variety of the eastern Midlands.", Difficulty.Trivial),
			HistoricalAccent("West Midlands", "Midlands Middle English", "The Middle English variety of the western Midlands."),
			HistoricalAccent("Northern", "Northern Middle English", "The northern English continuum from the Humber towards the Scottish Lowlands."),
			HistoricalAccent("Southern", "Southern Middle English", "The southern Middle English continuum descended chiefly from West Saxon."),
			HistoricalAccent("Kentish", "Southern Middle English", "The distinctive southeastern Middle English variety of Kent.", Difficulty.Easy),
			HistoricalAccent("Lowland Scots", "Northern Middle English", "The northern British variety developing in the burghs and Lowlands of medieval Scotland.", Difficulty.Easy)
		]),
		new("Medieval Gaelic", "an unknown Goidelic language",
		[
			HistoricalAccent("Irish", "Medieval Gaelic", "The medieval Gaelic continuum as spoken in Ireland.", Difficulty.Trivial),
			HistoricalAccent("Scottish", "Medieval Gaelic", "The medieval Gaelic continuum as spoken in Alba and the western seaboard.")
		]),
		new("Old High German", "an unknown West Germanic language",
		[
			HistoricalAccent("Rhine Franconian", "Franconian Old High German", "A central Franconian variety of the middle Rhine."),
			HistoricalAccent("East Franconian", "Franconian Old High German", "The eastern Franconian variety prominent in Carolingian vernacular writing.", Difficulty.Trivial),
			HistoricalAccent("Alemannic", "Upper Old High German", "The Upper German variety of Alemannic regions.", Difficulty.Easy),
			HistoricalAccent("Bavarian", "Upper Old High German", "The Upper German variety of Bavaria and Austria.", Difficulty.Easy)
		]),
		new("Old Low Franconian", "an unknown West Germanic language",
		[
			HistoricalAccent("Lower Rhine", "Low Franconian", "The unshifted Franconian variety of the lower Rhine.", Difficulty.Trivial),
			HistoricalAccent("Meuse-Rhine", "Low Franconian", "A transitional Low Franconian variety of the Meuse and lower Rhine.")
		]),
		new("Middle High German", "an unknown West Germanic language",
		[
			HistoricalAccent("Swabian Courtly", "Upper Middle High German", "The prestigious southern courtly and poetic variety around 1200.", Difficulty.Trivial),
			HistoricalAccent("East Franconian", "Central Middle High German", "The eastern Franconian Middle High German variety."),
			HistoricalAccent("Rhine Franconian", "Central Middle High German", "The central German variety of the middle Rhine."),
			HistoricalAccent("Alemannic", "Upper Middle High German", "The Middle High German continuum of Alemannic lands."),
			HistoricalAccent("Bavarian", "Upper Middle High German", "The Middle High German continuum of Bavaria and Austria.")
		]),
		new("Middle Low German", "an unknown West Germanic language",
		[
			HistoricalAccent("Westphalian", "western Middle Low German", "The western Middle Low German variety associated with Westphalia.", Difficulty.Trivial),
			HistoricalAccent("Eastphalian", "central Middle Low German", "The central Middle Low German variety associated with Eastphalia."),
			HistoricalAccent("North Low Saxon", "northern Middle Low German", "The northern coastal Middle Low German variety.", Difficulty.Easy)
		]),
		new("Old Castilian", "an unknown Iberian Romance language",
		[
			HistoricalAccent("Burgalese", "Old Castilian", "The northern-central Castilian variety associated with Burgos.", Difficulty.Trivial),
			HistoricalAccent("Toledan", "Old Castilian", "The central Castilian variety of reconquered Toledo, shaped by sustained language contact."),
			HistoricalAccent("Leonese Border", "Old Castilian", "A western Castilian variety with neighbouring Leonese influence.", Difficulty.Easy)
		]),
		new("Old Leonese", "an unknown Iberian Romance language",
		[
			HistoricalAccent("Leonese", "central Old Leonese", "The Old Leonese variety of the kingdom's central heartland.", Difficulty.Trivial),
			HistoricalAccent("Asturian", "northern Old Leonese", "The northern Astur-Leonese variety associated with Asturias."),
			HistoricalAccent("Mirandese Border", "western Old Leonese", "A western border variety of the Astur-Leonese continuum.", Difficulty.Easy)
		]),
		new("Old Aragonese", "an unknown Iberian Romance language",
		[
			HistoricalAccent("Pyrenean", "northern Old Aragonese", "The northern Old Aragonese variety of the Pyrenean valleys.", Difficulty.Trivial),
			HistoricalAccent("Zaragozan", "southern Old Aragonese", "The expanding southern variety associated with Zaragoza.", Difficulty.Easy)
		]),
		new("Galician-Portuguese", "an unknown western Iberian Romance language",
		[
			HistoricalAccent("Galician", "northern Galician-Portuguese", "The northern variety associated with Galicia.", Difficulty.Trivial),
			HistoricalAccent("Portucalense", "southern Galician-Portuguese", "The southern variety associated with the county and kingdom of Portugal."),
			HistoricalAccent("Courtly Lyric", "literary Galician-Portuguese", "The cultivated lyric register used across Iberian courts.", Difficulty.Easy)
		]),
		new("Old Catalan", "an unknown eastern Iberian Romance language",
		[
			HistoricalAccent("Eastern Catalan", "eastern Old Catalan", "The eastern Old Catalan variety of the coastal counties.", Difficulty.Trivial),
			HistoricalAccent("Western Catalan", "western Old Catalan", "The western Old Catalan variety of inland and frontier regions.", Difficulty.Easy)
		]),
		new("Andalusi Arabic", "an unknown western Arabic language",
		[
			HistoricalAccent("Cordoban", "Urban Andalusi", "The prestigious urban Andalusi variety of Córdoba.", Difficulty.Trivial),
			HistoricalAccent("Sevillian", "Urban Andalusi", "The urban Andalusi variety of Seville."),
			HistoricalAccent("Upper March", "Frontier Andalusi", "The northeastern frontier variety associated with Zaragoza and the Upper March.", Difficulty.Easy),
			HistoricalAccent("Berber-Influenced", "Contact Andalusi", "An Andalusi variety shaped by sustained contact with Amazigh speakers.", Difficulty.Easy)
		]),
		new("Quranic Arabic", "an unknown classical Arabic language",
		[
			HistoricalAccent("Classical Recitation", "Classical Arabic", "A formal learned pronunciation used for scripture, scholarship and high literature.", Difficulty.Trivial),
			HistoricalAccent("Andalusi Recitation", "Classical Arabic", "A western learned recitation tradition of al-Andalus."),
			HistoricalAccent("Mashriqi Recitation", "Classical Arabic", "An eastern learned recitation tradition of the medieval Islamic world.")
		]),
		new("Medieval Greek", "an unknown Byzantine Greek language",
		[
			HistoricalAccent("Constantinopolitan", "Byzantine Koine", "The prestigious vernacular of Constantinople.", Difficulty.Trivial),
			HistoricalAccent("Anatolian", "Byzantine Regional", "A medieval Greek variety of Byzantine Anatolia."),
			HistoricalAccent("Aegean", "Byzantine Regional", "A medieval Greek variety of the Aegean islands and coasts."),
			HistoricalAccent("Southern Italian", "Byzantine Regional", "The Greek variety of Byzantine and post-Byzantine southern Italy.", Difficulty.Easy)
		]),
		new("Koine Greek", "an unknown learned Greek language",
		[
			HistoricalAccent("Byzantine Learned", "Learned Greek", "The conservative learned register used by Byzantine clergy, officials and scholars.", Difficulty.Trivial),
			HistoricalAccent("Ecclesiastical", "Learned Greek", "The Greek pronunciation tradition of Byzantine worship and scripture.")
		]),
		new("Mashriqi Arabic", "an unknown eastern Arabic language",
		[
			HistoricalAccent("Abbasid Iraqi", "Medieval Mashriqi", "The urban eastern Arabic variety of Abbasid Iraq.", Difficulty.Trivial),
			HistoricalAccent("Fatimid Egyptian", "Medieval Mashriqi", "The Arabic variety of Fatimid Egypt."),
			HistoricalAccent("Levantine", "Medieval Mashriqi", "A medieval eastern Arabic variety of Syria and the Levant.")
		]),
		new("Persian", "an unknown Iranian language",
		[
			HistoricalAccent("Khorasani", "Early New Persian", "The northeastern Early New Persian variety central to the first great Persian literary courts.", Difficulty.Trivial),
			HistoricalAccent("Iraqi Persian", "Early New Persian", "A western Persian variety of Iraq and western Iran."),
			HistoricalAccent("Seljuk Court", "Early New Persian", "The Persian court and administrative idiom of the Seljuk world.")
		]),
		new("Oghuz Turkic", "an unknown Oghuz Turkic language",
		[
			HistoricalAccent("Seljuk Anatolian", "Western Oghuz", "The western Oghuz variety of Seljuk Anatolia.", Difficulty.Trivial),
			HistoricalAccent("Turkmen", "Western Oghuz", "The Oghuz continuum of Turkmen tribal confederations."),
			HistoricalAccent("Khorasani Oghuz", "Eastern Oghuz", "An eastern Oghuz variety of Khorasan and neighbouring regions.", Difficulty.Easy)
		]),
		new("Old Hungarian", "an unknown Uralic language",
		[
			HistoricalAccent("Pannonian", "Old Hungarian", "The Old Hungarian variety of the Carpathian Basin.", Difficulty.Trivial),
			HistoricalAccent("Transylvanian", "Old Hungarian", "An eastern Old Hungarian variety of Transylvania."),
			HistoricalAccent("Steppe-Contact", "Old Hungarian", "A variety retaining stronger contact influence from neighbouring steppe peoples.", Difficulty.Easy)
		]),
		new("Old East Slavic", "an unknown East Slavic language",
		[
			HistoricalAccent("Kievan", "Southern Rus", "The central-southern Old East Slavic variety associated with Kyiv.", Difficulty.Trivial),
			HistoricalAccent("Novgorodian", "Northern Rus", "The distinctive northwestern vernacular preserved in Novgorod birch-bark letters.", Difficulty.Easy),
			HistoricalAccent("Suzdalian", "Northeastern Rus", "A northeastern Old East Slavic variety of the Rostov-Suzdal lands.")
		]),
		new("Church Slavonic", "an unknown learned Slavic language",
		[
			HistoricalAccent("Rus Recension", "Church Slavonic", "The learned Church Slavonic recension of medieval Rus.", Difficulty.Trivial),
			HistoricalAccent("Bulgarian Recension", "Church Slavonic", "A southern Slavic learned and liturgical recension.")
		]),
		new("Cuman-Kipchak", "an unknown Kipchak Turkic language",
		[
			HistoricalAccent("Cuman", "Western Kipchak", "The western Kipchak variety of the Cuman confederations.", Difficulty.Trivial),
			HistoricalAccent("Kipchak", "Steppe Kipchak", "A central steppe Kipchak variety."),
			HistoricalAccent("Mamluk Kipchak", "Diaspora Kipchak", "The Kipchak variety of military and courtly communities in the Islamic world.", Difficulty.Easy)
		]),
		new("Middle Mongol", "an unknown Mongolic language",
		[
			HistoricalAccent("Imperial Central", "Middle Mongol", "The central prestige variety of the early Mongol imperial confederation.", Difficulty.Trivial),
			HistoricalAccent("Eastern", "Middle Mongol", "An eastern Middle Mongol regional variety."),
			HistoricalAccent("Western", "Middle Mongol", "A western Middle Mongol regional variety.")
		]),
		new("Western Apabhramsha", "an unknown Middle Indo-Aryan language",
		[
			HistoricalAccent("Maru-Gurjara", "Western Apabhramsha", "A western Apabhramsha variety associated with Rajasthan and Gujarat.", Difficulty.Trivial),
			HistoricalAccent("Shauraseni Literary", "Literary Apabhramsha", "A conservative literary Apabhramsha used across northern courts and learned communities.")
		]),
		new("Sanskrit", "an unknown classical Indo-Aryan language",
		[
			HistoricalAccent("Northern Scholastic", "Classical Sanskrit", "A northern Indian learned recitation tradition.", Difficulty.Trivial),
			HistoricalAccent("Chola Grantha", "Classical Sanskrit", "A southern learned tradition transmitted alongside Tamil through Grantha writing.")
		]),
		new("Medieval Tamil", "an unknown Dravidian language",
		[
			HistoricalAccent("Chola Heartland", "Chola Tamil", "The medieval Tamil variety of the Kaveri basin and Chola heartland.", Difficulty.Trivial),
			HistoricalAccent("Pandya", "Southern Tamil", "The southern Tamil variety of the Pandya country."),
			HistoricalAccent("Lankan", "Insular Tamil", "A medieval Tamil variety of northern Sri Lanka.", Difficulty.Easy)
		]),
		new("Late Middle Chinese", "an unknown Sinitic language",
		[
			HistoricalAccent("Northern Court", "Northern Sinitic", "The northern prestige speech associated with the Song court and Kaifeng.", Difficulty.Trivial),
			HistoricalAccent("Lower Yangtze", "Southern Sinitic", "A Sinitic variety of the lower Yangtze region."),
			HistoricalAccent("Sichuan", "Western Sinitic", "A western regional Sinitic variety of Sichuan.", Difficulty.Easy),
			HistoricalAccent("Southern Coastal", "Southern Sinitic", "A southern coastal Sinitic variety increasingly distinct from northern speech.", Difficulty.Easy)
		]),
		new("Literary Chinese", "an unknown learned Sinitic language",
		[
			HistoricalAccent("Song Scholarly", "Literary Sinitic Reading", "The learned Song reading tradition for Literary Chinese.", Difficulty.Trivial),
			HistoricalAccent("Goryeo Scholarly", "Literary Sinitic Reading", "A Korean scholarly reading tradition for Literary Chinese."),
			HistoricalAccent("Japanese Kanbun", "Literary Sinitic Reading", "A Japanese scholarly reading tradition for Literary Chinese.", Difficulty.Easy)
		]),
		new("Early Middle Korean", "an unknown Koreanic language",
		[
			HistoricalAccent("Kaegyong", "Goryeo Korean", "The central prestige variety of the Goryeo capital Kaegyong.", Difficulty.Trivial),
			HistoricalAccent("Southeastern", "Goryeo Korean", "A southeastern variety retaining features of the older Silla heartland."),
			HistoricalAccent("Northern Frontier", "Goryeo Korean", "A northern frontier variety shaped by contact with neighbouring peoples.", Difficulty.Easy)
		]),
		new("Medieval Japanese", "an unknown Japonic language",
		[
			HistoricalAccent("Heian Capital", "Capital Japanese", "The prestigious court variety of Heian-kyō inherited into classical literary usage.", Difficulty.Trivial),
			HistoricalAccent("Kamakura Eastern", "Eastern Japanese", "The eastern warrior and regional variety associated with Kamakura."),
			HistoricalAccent("Western Provincial", "Western Japanese", "A western provincial variety outside the capital."),
			HistoricalAccent("Kyushu", "Western Japanese", "A southwestern medieval Japanese variety of Kyushu.", Difficulty.Easy)
		])
	];

	private static readonly HistoricalScriptSeed[] DarkAgesAndMedievalScripts =
	[
		new("Latin", "the Latin script", "an alphabetic script", "The alphabet used by the Latin Church and adapted throughout western and central Europe for learned and vernacular writing.", "Alphabet", 1.0, 1.0,
			["Latin", "Old English", "Old Norman", "Anglo-Norman", "Old French", "Middle English", "Medieval Gaelic", "Old High German", "Old Low Franconian", "Middle High German", "Middle Low German", "Old Castilian", "Old Leonese", "Old Aragonese", "Galician-Portuguese", "Old Catalan", "Old Hungarian", "Cuman-Kipchak"]),
		new("Anglo-Saxon Futhorc", "Anglo-Saxon runes", "a runic script", "The expanded Anglo-Frisian runic alphabet used for Old English inscriptions and occasional manuscript notation.", "Runic Alphabet", 1.0, 0.8, ["Old English"]),
		new("Younger Futhark", "Younger Futhark runes", "a runic script", "The sixteen-rune Scandinavian writing system of the Viking Age, including long-branch and short-twig traditions.", "Runic Alphabet", 1.0, 0.8, ["Old Norse"]),
		new("Greek", "the Greek script", "an alphabetic script", "The Greek alphabet used for Byzantine vernacular, learned and ecclesiastical writing.", "Alphabet", 1.0, 1.0, ["Medieval Greek", "Koine Greek"]),
		new("Cyrillic", "the Cyrillic script", "a Slavic alphabetic script", "The early Cyrillic alphabet used for Church Slavonic and the vernacular writing of medieval Rus.", "Alphabet", 1.0, 1.0, ["Old East Slavic", "Church Slavonic"]),
		new("Glagolitic", "the Glagolitic script", "a Slavic alphabetic script", "The earliest alphabet devised for Slavonic liturgy, retained in particular regional and ecclesiastical traditions.", "Alphabet", 1.0, 1.0, ["Church Slavonic"]),
		new("Arabic", "the Arabic script", "a cursive abjad", "The right-to-left Arabic-derived writing system used for Arabic, Persian and many Islamicate languages.", "Abjad", 0.8, 1.2, ["Andalusi Arabic", "Quranic Arabic", "Mashriqi Arabic", "Persian", "Oghuz Turkic", "Cuman-Kipchak"]),
		new("Old Hungarian", "the Old Hungarian script", "a runiform alphabetic script", "The historical right-to-left script associated with Hungarian before and alongside the dominance of Latin writing.", "Alphabet", 1.0, 0.9, ["Old Hungarian"]),
		new("Mongolian", "the traditional Mongolian script", "a vertical alphabetic script", "The Old Uyghur-derived vertical script adopted for Mongolian at the beginning of the thirteenth century.", "Vertical Alphabet", 0.9, 1.1, ["Middle Mongol"]),
		new("Devanagari", "the Devanagari script", "an Indic abugida", "The North Indian Brahmi-derived abugida that became a major vehicle for Sanskrit and related literary languages by the eleventh century.", "Abugida", 0.8, 1.2, ["Western Apabhramsha", "Sanskrit"]),
		new("Tamil", "the Tamil script", "an Indic abugida", "The southern Brahmi-derived script used for Tamil inscriptions and manuscripts throughout the Chola period.", "Abugida", 0.8, 1.2, ["Medieval Tamil"]),
		new("Grantha", "the Grantha script", "an Indic abugida", "The South Indian script used especially to represent Sanskrit accurately in Tamil-speaking regions.", "Abugida", 0.8, 1.2, ["Sanskrit"]),
		new("Chinese", "Han characters", "a logosyllabic script", "The shared Han character tradition used for Literary Chinese and adapted for the written cultures of China, Korea and Japan.", "Logosyllabary", 0.5, 2.0, ["Late Middle Chinese", "Literary Chinese", "Early Middle Korean", "Medieval Japanese"]),
		new("Idu", "the Idu writing system", "a mixed character script", "A Korean system that used Han characters for their meanings and sounds to represent Korean vocabulary and grammar.", "Hybrid", 0.6, 1.8, ["Early Middle Korean"]),
		new("Japanese", "the mixed kanji and kana script", "a mixed East Asian script", "The Japanese writing system combining Han characters with the hiragana and katakana syllabaries that developed from them.", "Hybrid", 0.75, 1.5, ["Medieval Japanese"])
	];

	private static readonly HistoricalMutualIntelligibilitySeed[] DarkAgesAndMedievalMutualIntelligibilities =
	[
		new("Old English", "Old Norse", Difficulty.Hard),
		new("Old English", "Middle English", Difficulty.ExtremelyHard),
		new("Old Norman", "Anglo-Norman", Difficulty.Hard),
		new("Old Norman", "Old French", Difficulty.Hard),
		new("Anglo-Norman", "Old French", Difficulty.VeryHard),
		new("Old High German", "Old Low Franconian", Difficulty.VeryHard),
		new("Old High German", "Middle High German", Difficulty.VeryHard),
		new("Old Low Franconian", "Middle High German", Difficulty.ExtremelyHard),
		new("Middle High German", "Middle Low German", Difficulty.VeryHard),
		new("Old Low Franconian", "Middle Low German", Difficulty.Hard),
		new("Old French", "Old Castilian", Difficulty.Insane),
		new("Old Castilian", "Old Leonese", Difficulty.Hard),
		new("Old Castilian", "Old Aragonese", Difficulty.Hard),
		new("Old Castilian", "Galician-Portuguese", Difficulty.VeryHard),
		new("Old Aragonese", "Old Catalan", Difficulty.Hard),
		new("Old Leonese", "Galician-Portuguese", Difficulty.Hard),
		new("Andalusi Arabic", "Quranic Arabic", Difficulty.ExtremelyHard),
		new("Andalusi Arabic", "Mashriqi Arabic", Difficulty.VeryHard),
		new("Quranic Arabic", "Mashriqi Arabic", Difficulty.ExtremelyHard),
		new("Medieval Greek", "Koine Greek", Difficulty.VeryHard),
		new("Oghuz Turkic", "Cuman-Kipchak", Difficulty.VeryHard),
		new("Old East Slavic", "Church Slavonic", Difficulty.VeryHard),
		new("Western Apabhramsha", "Sanskrit", Difficulty.ExtremelyHard)
	];

	private void SeedDarkAgesAndMedievalLanguages()
	{
		SeedHistoricalLanguagePack(
			DarkAgesAndMedievalLanguages,
			DarkAgesAndMedievalScripts,
			DarkAgesAndMedievalMutualIntelligibilities);
	}
}
