#nullable enable

using MudSharp.RPG.Checks;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private void SeedRenaissanceWorldExpansionLanguages()
	{
		SeedHistoricalLanguagePack(
			RenaissanceWorldLanguages,
			RenaissanceWorldScripts,
			RenaissanceWorldMutualIntelligibilities);
	}

	private static readonly IReadOnlyDictionary<string, string[]> RenaissanceWorldLanguageCoverage =
		new Dictionary<string, string[]>
		{
			["Renaissance Armenian"] = ["Armenian"],
			["Renaissance Georgian"] = ["Georgian"],
			["Renaissance Kurdish"] = ["Kurmanji Kurdish", "Gorani"],
			["Renaissance Mamluk Arabic"] = ["Mashriqi Arabic", "Quranic Arabic", "Coptic"],
			["Renaissance Timurid Persianate"] = ["Persian", "Chagatai"],
			["Renaissance Delhi Sultanate"] = ["Hindavi", "Persian", "Pashto"],
			["Renaissance Rajput"] = ["Old Rajasthani", "Sanskrit"],
			["Renaissance Bengali"] = ["Middle Bengali", "Persian"],
			["Renaissance Vijayanagara"] = ["Kannada", "Telugu", "Tamil", "Sanskrit"],
			["Renaissance Ming Chinese"] = ["Ming Guanhua", "Wu Chinese", "Yue", "Southern Min", "Eastern Min", "Northern Min", "Classical Chinese"],
			["Renaissance Joseon Korean"] = ["Middle Korean", "Classical Chinese"],
			["Renaissance Muromachi Japanese"] = ["Late Middle Japanese", "Classical Chinese"],
			["Renaissance Tibetan"] = ["Central Tibetan", "Khams Tibetan", "Amdo Tibetan", "Classical Tibetan"],
			["Renaissance Dai Viet"] = ["Middle Vietnamese", "Classical Chinese"],
			["Renaissance Ayutthaya Thai"] = ["Ayutthaya Thai"],
			["Renaissance Majapahit Javanese"] = ["Middle Javanese", "Sanskrit"],
			["Renaissance Malay"] = ["Classical Malay", "Quranic Arabic"],
			["Renaissance Ethiopian Highland"] = ["Amharic", "Ge'ez"],
			["Renaissance Somali"] = ["Somali", "Maay", "Quranic Arabic"],
			["Renaissance Swahili"] = ["Swahili", "Quranic Arabic"],
			["Renaissance Mande Sahelian"] = ["Manding", "Songhay", "Quranic Arabic"],
			["Renaissance Hausa"] = ["Hausa", "Quranic Arabic"],
			["Renaissance Kongo"] = ["Kikongo"],
			["Renaissance Shona"] = ["Karanga", "Korekore", "Zezuru"],
			["Renaissance Amazigh"] = ["Tashelhit", "Central Atlas Tamazight", "Tarifit", "Kabyle", "Nafusi", "Mahgrebi Arabic"]
		};

	private static readonly HistoricalLanguageSeed[] RenaissanceWorldLanguages =
	[
		new("Armenian", "an unknown Armenian language", [
			HistoricalAccent("eastern-armenian", "eastern Armenian", "The Eastern Armenian speech of the Ararat plain and the eastern highlands.", Difficulty.Trivial),
			HistoricalAccent("western-armenian", "western Armenian", "The Western Armenian speech of the plateau west of Lake Van."),
			HistoricalAccent("karabakh", "eastern Armenian", "The Armenian variety of the Artsakh and Karabakh highlands."),
			HistoricalAccent("cilician", "western Armenian", "The prestigious Cilician Armenian tradition retained by merchants, clergy and emigrant communities.", Difficulty.Easy)
		]),
		new("Georgian", "an unknown Kartvelian language", [
			HistoricalAccent("kartlian", "eastern Georgian", "The central-eastern Georgian variety of Kartli and the royal cities.", Difficulty.Trivial),
			HistoricalAccent("kakhetian", "eastern Georgian", "The eastern Georgian variety of Kakheti."),
			HistoricalAccent("imeretian", "western Georgian", "The western Georgian variety of Imereti."),
			HistoricalAccent("highland", "highland Georgian", "A conservative highland Georgian variety shaped by the mountain communities.", Difficulty.Easy)
		]),
		new("Kurmanji Kurdish", "an unknown northwestern Iranian language", [
			HistoricalAccent("botan", "southeastern Kurmanji", "The Kurmanji variety associated with Botan and the upper Tigris.", Difficulty.Trivial),
			HistoricalAccent("hakkari", "southeastern Kurmanji", "The mountain Kurmanji variety of Hakkari."),
			HistoricalAccent("badinan", "southern Kurmanji", "The Kurmanji variety of the Bahdinan principalities."),
			HistoricalAccent("anatolian", "western Kurmanji", "A western Kurmanji variety of eastern Anatolia.", Difficulty.Easy)
		]),
		new("Gorani", "an unknown northwestern Iranian language", [
			HistoricalAccent("hawrami", "Gorani", "The conservative Hawrami variety of the central Zagros.", Difficulty.Trivial),
			HistoricalAccent("shahrizori", "Gorani", "A Gorani variety associated with Shahrizor and neighbouring valleys.")
		]),
		new("Mashriqi Arabic", "an unknown eastern Arabic language", [
			HistoricalAccent("egyptian", "Egyptian Arabic", "The urban Arabic of Mamluk Cairo and the Nile Delta.", Difficulty.Trivial),
			HistoricalAccent("levantine", "Levantine Arabic", "The urban Arabic continuum of Damascus, Aleppo and the Levant."),
			HistoricalAccent("iraqi", "Mesopotamian Arabic", "A Mesopotamian Arabic variety heard along the eastern caravan routes.", Difficulty.Easy),
			HistoricalAccent("hejazi", "Arabian Arabic", "The western Arabian variety associated with the pilgrimage cities.", Difficulty.Easy)
		]),
		new("Quranic Arabic", "an unknown classical Arabic language", [
			HistoricalAccent("classical", "classical Arabic", "A careful learned pronunciation of Classical and Quranic Arabic.", Difficulty.Trivial),
			HistoricalAccent("hafs", "canonical recitation", "The Hafs from Asim Quranic recitation tradition."),
			HistoricalAccent("warsh", "canonical recitation", "The Warsh from Nafi Quranic recitation tradition.")
		]),
		new("Coptic", "an unknown Egyptian language", [
			HistoricalAccent("bohairic", "Lower Egyptian Coptic", "The Bohairic Coptic variety of the Nile Delta.", Difficulty.Trivial),
			HistoricalAccent("sahidic", "Upper Egyptian Coptic", "The Sahidic Coptic literary and liturgical variety of Upper Egypt.")
		]),
		new("Persian", "an unknown southwestern Iranian language", [
			HistoricalAccent("khorasani", "eastern Persian", "The Persian of Khorasan and the Timurid court at Herat.", Difficulty.Trivial),
			HistoricalAccent("transoxian", "eastern Persian", "The Persian of Samarkand, Bukhara and Transoxiana."),
			HistoricalAccent("iranian", "western Persian", "A western Iranian Persian variety."),
			HistoricalAccent("indo-persian", "Indo-Persian", "The Persian of north Indian courts, administrators and learned circles.", Difficulty.Easy)
		]),
		new("Chagatai", "an unknown eastern Turkic language", [
			HistoricalAccent("herati", "Timurid Chagatai", "The cultivated Chagatai of the Timurid literary circle at Herat.", Difficulty.Trivial),
			HistoricalAccent("samarkandi", "Transoxian Chagatai", "The Chagatai variety of Samarkand and its court."),
			HistoricalAccent("ferghanan", "eastern Chagatai", "An eastern Chagatai variety of the Ferghana valley.")
		]),
		new("Hindavi", "an unknown central Indo-Aryan language", [
			HistoricalAccent("dehlavi", "Delhi Hindavi", "The Hindavi vernacular of Delhi and its immediate hinterland.", Difficulty.Trivial),
			HistoricalAccent("upper-doab", "Upper Doab Hindavi", "A Hindavi variety of the upper Ganges-Yamuna doab."),
			HistoricalAccent("punjabi-march", "northwestern Hindavi", "A northwestern Hindavi variety coloured by Punjabi and Afghan contact.", Difficulty.Easy)
		]),
		new("Pashto", "an unknown eastern Iranian language", [
			HistoricalAccent("ghilji", "southern Pashto", "The Pashto variety associated with Ghilji communities.", Difficulty.Trivial),
			HistoricalAccent("karlani", "southern Pashto", "A southern Pashto variety associated with Karlani confederacies."),
			HistoricalAccent("yusufzai", "northern Pashto", "A northern Pashto variety associated with Yusufzai communities.", Difficulty.Easy)
		]),
		new("Old Rajasthani", "an unknown western Indo-Aryan language", [
			HistoricalAccent("marwari", "western Rajasthani", "The western Rajasthani variety of Marwar.", Difficulty.Trivial),
			HistoricalAccent("mewari", "southern Rajasthani", "The southern Rajasthani variety of Mewar."),
			HistoricalAccent("dhundhari", "eastern Rajasthani", "The eastern Rajasthani variety of Dhundhar.")
		]),
		new("Sanskrit", "an unknown classical Indo-Aryan language", [
			HistoricalAccent("northern-scholastic", "scholastic Sanskrit", "A northern Indian scholastic recitation of Sanskrit.", Difficulty.Trivial),
			HistoricalAccent("southern-scholastic", "scholastic Sanskrit", "A southern Indian scholastic recitation of Sanskrit."),
			HistoricalAccent("courtly", "courtly Sanskrit", "The polished Sanskrit pronunciation of a royal poet or learned courtier.")
		]),
		new("Middle Bengali", "an unknown eastern Indo-Aryan language", [
			HistoricalAccent("rarh", "western Bengali", "The Middle Bengali variety of Rarh in the western delta.", Difficulty.Trivial),
			HistoricalAccent("varendra", "northern Bengali", "The Middle Bengali variety of Varendra."),
			HistoricalAccent("vanga", "eastern Bengali", "The Middle Bengali variety of the eastern and central delta."),
			HistoricalAccent("kamrupa", "northeastern Bengali", "A northeastern literary and regional variety near Kamrupa.", Difficulty.Easy)
		]),
		new("Kannada", "an unknown Dravidian language", [
			HistoricalAccent("vijayanagara-court", "courtly Kannada", "The prestigious Kannada of the Vijayanagara court and capital.", Difficulty.Trivial),
			HistoricalAccent("northern", "northern Kannada", "A northern Kannada variety of the upper Deccan."),
			HistoricalAccent("southern", "southern Kannada", "A southern Kannada variety of the Kaveri uplands.")
		]),
		new("Telugu", "an unknown Dravidian language", [
			HistoricalAccent("rayalaseema", "interior Telugu", "The Telugu variety of the dry southern interior and Vijayanagara frontier.", Difficulty.Trivial),
			HistoricalAccent("coastal", "coastal Telugu", "The Telugu variety of the eastern coastal plains."),
			HistoricalAccent("telangana", "northern Telugu", "A northern Deccan Telugu variety.", Difficulty.Easy)
		]),
		new("Tamil", "an unknown Dravidian language", [
			HistoricalAccent("chola", "eastern Tamil", "The Tamil variety of the eastern Kaveri lands.", Difficulty.Trivial),
			HistoricalAccent("pandya", "southern Tamil", "The southern Tamil variety of the old Pandya country."),
			HistoricalAccent("kongu", "western Tamil", "The western Tamil variety of Kongu.")
		]),
		new("Ming Guanhua", "an unknown Sinitic language", [
			HistoricalAccent("nanjing-court", "southern Guanhua", "The prestigious Nanjing-influenced official speech of the early and middle Ming.", Difficulty.Trivial),
			HistoricalAccent("beijing", "northern Guanhua", "The northern court variety heard in Beijing after the capital's transfer."),
			HistoricalAccent("central-plains", "northern Guanhua", "A Guanhua variety of the central plains.", Difficulty.Easy)
		]),
		new("Wu Chinese", "an unknown Sinitic language", [
			HistoricalAccent("suzhou", "Taihu Wu", "The Wu variety of Suzhou and the lower Yangtze elite.", Difficulty.Trivial),
			HistoricalAccent("hangzhou", "Taihu Wu", "The Wu variety of Hangzhou."),
			HistoricalAccent("ningbo", "coastal Wu", "The coastal Wu variety of Ningbo.", Difficulty.Easy)
		]),
		new("Yue", "an unknown Sinitic language", [
			HistoricalAccent("guangzhou", "Pearl River Yue", "The Yue variety of Guangzhou and the Pearl River delta.", Difficulty.Trivial),
			HistoricalAccent("guangxi", "inland Yue", "An inland Yue variety of eastern Guangxi.", Difficulty.Easy)
		]),
		new("Southern Min", "an unknown coastal Sinitic language", [
			HistoricalAccent("Quanzhou", "Quanzhou Southern Min", "The Southern Min variety of Quanzhou and its trading hinterland.", Difficulty.Trivial),
			HistoricalAccent("Zhangzhou", "Zhangzhou Southern Min", "The Southern Min variety of Zhangzhou."),
			HistoricalAccent("Chaozhou", "Chaoshan Southern Min", "The divergent Southern Min variety of the Chaoshan region.", Difficulty.Easy)
		]),
		new("Eastern Min", "an unknown eastern Fujian Sinitic language", [
			HistoricalAccent("Fuzhou", "Fuzhou Eastern Min", "The Eastern Min variety associated with Fuzhou.", Difficulty.Trivial),
			HistoricalAccent("Fuqing", "coastal Eastern Min", "A coastal Eastern Min variety associated with Fuqing.", Difficulty.Easy)
		]),
		new("Northern Min", "an unknown inland Fujian Sinitic language", [
			HistoricalAccent("Jianou", "Jianou Northern Min", "The Northern Min variety associated with Jianou.", Difficulty.Trivial),
			HistoricalAccent("Jianyang", "Jianyang Northern Min", "A Northern Min variety associated with Jianyang.", Difficulty.Easy)
		]),
		new("Classical Chinese", "an unknown classical Sinitic language", [
			HistoricalAccent("ming-scholastic", "Chinese scholastic", "A Ming learned reading tradition for Literary Chinese.", Difficulty.Trivial),
			HistoricalAccent("korean-reading", "Korean scholastic", "A Joseon Korean reading tradition for Literary Chinese.", Difficulty.Easy),
			HistoricalAccent("japanese-reading", "Japanese scholastic", "A Muromachi Japanese reading tradition for Literary Chinese.", Difficulty.Easy),
			HistoricalAccent("vietnamese-reading", "Vietnamese scholastic", "A Dai Viet reading tradition for Literary Chinese.", Difficulty.Easy)
		]),
		new("Middle Korean", "an unknown Koreanic language", [
			HistoricalAccent("hanseong-court", "central Korean", "The central Middle Korean variety of the Joseon court at Hanseong.", Difficulty.Trivial),
			HistoricalAccent("southeastern", "southeastern Korean", "A southeastern Middle Korean variety."),
			HistoricalAccent("southwestern", "southwestern Korean", "A southwestern Middle Korean variety."),
			HistoricalAccent("northern", "northern Korean", "A northern Middle Korean variety.", Difficulty.Easy)
		]),
		new("Late Middle Japanese", "an unknown Japonic language", [
			HistoricalAccent("kyoto-court", "central Japanese", "The prestigious Late Middle Japanese of Kyoto and the Muromachi court.", Difficulty.Trivial),
			HistoricalAccent("eastern", "eastern Japanese", "An eastern warrior-house variety associated with Kamakura and the Kanto."),
			HistoricalAccent("inland-sea", "western Japanese", "A western variety of the Inland Sea trade corridor."),
			HistoricalAccent("kyushu", "Kyushu Japanese", "A southern Japanese variety of Kyushu.", Difficulty.Easy)
		]),
		new("Central Tibetan", "an unknown Tibetic language", [
			HistoricalAccent("u", "Central Tibetan", "The Central Tibetan variety of U and Lhasa.", Difficulty.Trivial),
			HistoricalAccent("tsang", "Central Tibetan", "The Central Tibetan variety of Tsang and Shigatse.")
		]),
		new("Khams Tibetan", "an unknown Tibetic language", [
			HistoricalAccent("derge", "Khams Tibetan", "A Khams Tibetan variety associated with Derge.", Difficulty.Trivial),
			HistoricalAccent("chamdo", "Khams Tibetan", "A Khams Tibetan variety associated with Chamdo.")
		]),
		new("Amdo Tibetan", "an unknown Tibetic language", [
			HistoricalAccent("kokonor", "Amdo Tibetan", "An Amdo Tibetan variety of the Kokonor region.", Difficulty.Trivial),
			HistoricalAccent("rebkong", "Amdo Tibetan", "An Amdo Tibetan variety associated with Rebkong.")
		]),
		new("Classical Tibetan", "an unknown classical Tibetic language", [
			HistoricalAccent("monastic", "scholastic Tibetan", "A conservative monastic recitation of Classical Tibetan.", Difficulty.Trivial),
			HistoricalAccent("courtly", "scholastic Tibetan", "A formal court and chancery reading of Classical Tibetan.")
		]),
		new("Middle Vietnamese", "an unknown Vietic language", [
			HistoricalAccent("red-river", "northern Vietnamese", "The Middle Vietnamese variety of the Red River heartland.", Difficulty.Trivial),
			HistoricalAccent("thanh-nghe", "south-central Vietnamese", "The Middle Vietnamese variety of Thanh Hoa and Nghe An."),
			HistoricalAccent("southern-frontier", "southern Vietnamese", "A frontier variety carried into the newly incorporated southern lands.", Difficulty.Easy)
		]),
		new("Ayutthaya Thai", "an unknown Southwestern Tai language", [
			HistoricalAccent("ayutthaya-court", "central Tai", "The prestigious Tai variety of the Ayutthaya court and lower Chao Phraya.", Difficulty.Trivial),
			HistoricalAccent("suphanburi", "western central Tai", "A western central Tai variety associated with Suphanburi."),
			HistoricalAccent("sukhothai", "northern central Tai", "A northern central Tai variety associated with Sukhothai.", Difficulty.Easy)
		]),
		new("Middle Javanese", "an unknown Javanic language", [
			HistoricalAccent("majapahit-court", "eastern Javanese", "The cultivated Middle Javanese of the late Majapahit court.", Difficulty.Trivial),
			HistoricalAccent("north-coast", "coastal Javanese", "The Middle Javanese of the north-coast trading ports."),
			HistoricalAccent("central", "central Javanese", "A central Javanese inland variety.", Difficulty.Easy)
		]),
		new("Classical Malay", "an unknown Malayic language", [
			HistoricalAccent("malacca-court", "western Classical Malay", "The prestigious Classical Malay of the Malacca court and chancery.", Difficulty.Trivial),
			HistoricalAccent("pasai", "northern Classical Malay", "The Classical Malay variety of Pasai and northern Sumatra."),
			HistoricalAccent("peninsular", "peninsular Malay", "A peninsular trading variety of Classical Malay.")
		]),
		new("Amharic", "an unknown Ethiopian Semitic language", [
			HistoricalAccent("shewa", "central Amharic", "The Amharic variety of Shewa and the central royal domains.", Difficulty.Trivial),
			HistoricalAccent("gondar", "northern Amharic", "A northern highland Amharic variety."),
			HistoricalAccent("gojjam", "western Amharic", "A western highland Amharic variety.")
		]),
		new("Ge'ez", "an unknown classical Ethiopian Semitic language", [
			HistoricalAccent("ecclesiastical", "Ethiopian liturgical", "The formal ecclesiastical recitation of Ge'ez.", Difficulty.Trivial),
			HistoricalAccent("court-chronicle", "Ethiopian learned", "A learned reading used for royal chronicles and courtly texts.")
		]),
		new("Somali", "an unknown Cushitic language", [
			HistoricalAccent("northern", "northern Somali", "A northern Somali variety of the Gulf of Aden coast.", Difficulty.Trivial),
			HistoricalAccent("benadir", "coastal Somali", "A coastal southern variety associated with the Benadir ports.")
		]),
		new("Maay", "an unknown divergent Cushitic language", [
			HistoricalAccent("Bay", "northern Maay", "The Maay variety associated with the Bay region.", Difficulty.Trivial),
			HistoricalAccent("Bakool", "western Maay", "A western Maay variety associated with Bakool."),
			HistoricalAccent("Lower Shabelle", "southern Maay", "A southern inter-riverine Maay variety.", Difficulty.Easy)
		]),
		new("Swahili", "an unknown Sabaki Bantu language", [
			HistoricalAccent("kiamu", "northern Swahili", "The northern Swahili variety of Lamu and its archipelago.", Difficulty.Trivial),
			HistoricalAccent("kimvita", "central Swahili", "The central coastal Swahili variety of Mombasa."),
			HistoricalAccent("kiunguja", "southern Swahili", "The Swahili variety of Zanzibar."),
			HistoricalAccent("kilwa", "southern Swahili", "A southern coastal variety associated with Kilwa.")
		]),
		new("Manding", "an unknown Mande language", [
			HistoricalAccent("mandinka", "western Manding", "A western Manding variety associated with the old Mali heartlands.", Difficulty.Trivial),
			HistoricalAccent("bamana", "central Manding", "A central Manding variety of the upper Niger."),
			HistoricalAccent("dyula", "trade Manding", "A Manding trade variety carried by Muslim merchants.")
		]),
		new("Songhay", "an unknown Songhay language", [
			HistoricalAccent("gao", "eastern Songhay", "The Songhay variety of Gao and the imperial heartland.", Difficulty.Trivial),
			HistoricalAccent("timbuktu", "western Songhay", "The Songhay variety of Timbuktu and the Niger bend."),
			HistoricalAccent("dendi", "southern Songhay", "A southern Songhay variety of Dendi.", Difficulty.Easy)
		]),
		new("Hausa", "an unknown Chadic language", [
			HistoricalAccent("kano", "eastern Hausa", "The Hausa variety of Kano and its trading hinterland.", Difficulty.Trivial),
			HistoricalAccent("katsina", "western Hausa", "The Hausa variety of Katsina."),
			HistoricalAccent("zazzau", "southern Hausa", "The southern Hausa variety of Zazzau.")
		]),
		new("Kikongo", "an unknown Kikongo language cluster", [
			HistoricalAccent("mbanza-kongo", "central Kikongo", "The Kikongo variety of the royal capital and its central province.", Difficulty.Trivial),
			HistoricalAccent("nsundi", "northern Kikongo", "A northern variety associated with Nsundi."),
			HistoricalAccent("soyo", "coastal Kikongo", "A western coastal variety associated with Soyo.", Difficulty.Easy),
			HistoricalAccent("mbata", "eastern Kikongo", "An eastern variety associated with Mbata.", Difficulty.Easy)
		]),
		new("Karanga", "an unknown southern Bantu language", [
			HistoricalAccent("great-zimbabwe", "southern Karanga", "A Karanga variety of the southern plateau and Great Zimbabwe tradition.", Difficulty.Trivial),
			HistoricalAccent("masvingo", "southern Karanga", "A southern plateau Karanga variety.")
		]),
		new("Korekore", "an unknown southern Bantu language", [
			HistoricalAccent("mutapa", "northern Korekore", "The northern plateau variety associated with the emerging Mutapa state.", Difficulty.Trivial),
			HistoricalAccent("zambezi", "northern Korekore", "A Korekore variety of the middle Zambezi country.")
		]),
		new("Zezuru", "an unknown southern Bantu language", [
			HistoricalAccent("central-plateau", "central Zezuru", "A Zezuru variety of the central Zimbabwe plateau.", Difficulty.Trivial),
			HistoricalAccent("eastern-plateau", "eastern Zezuru", "An eastern central-plateau Zezuru variety.")
		]),
		new("Tashelhit", "an unknown Amazigh language", [
			HistoricalAccent("Sous", "lowland Tashelhit", "The Tashelhit variety of the Sous valley.", Difficulty.Trivial),
			HistoricalAccent("high-atlas", "mountain Tashelhit", "A Tashelhit variety of the western High Atlas."),
			HistoricalAccent("Anti-Atlas", "mountain Tashelhit", "A Tashelhit variety of the Anti-Atlas.")
		]),
		new("Central Atlas Tamazight", "an unknown Amazigh language", [
			HistoricalAccent("Middle Atlas", "Central Atlas", "The Tamazight variety of the Middle Atlas.", Difficulty.Trivial),
			HistoricalAccent("eastern-atlas", "Central Atlas", "An eastern Central Atlas variety.")
		]),
		new("Tarifit", "an unknown Amazigh language", [
			HistoricalAccent("central-rif", "Riffian", "The Tarifit variety of the central Rif.", Difficulty.Trivial),
			HistoricalAccent("eastern-rif", "Riffian", "An eastern Riffian variety.")
		]),
		new("Kabyle", "an unknown Amazigh language", [
			HistoricalAccent("Greater Kabylia", "Kabyle", "The Kabyle variety of the Djurdjura and Greater Kabylia.", Difficulty.Trivial),
			HistoricalAccent("Lesser Kabylia", "Kabyle", "An eastern Kabyle variety of Lesser Kabylia.")
		]),
		new("Nafusi", "an unknown Amazigh language", [
			HistoricalAccent("jabal-nafusa", "Nafusi", "The Nafusi variety of the Jabal Nafusa communities.", Difficulty.Trivial),
			HistoricalAccent("zuwara", "eastern Amazigh", "A closely related coastal eastern Amazigh variety associated with Zuwara.", Difficulty.Easy)
		]),
		new("Mahgrebi Arabic", "an unknown western Arabic language", [
			HistoricalAccent("moroccan", "western Maghrebi Arabic", "The Arabic varieties of the Moroccan cities and plains.", Difficulty.Trivial),
			HistoricalAccent("algerian", "central Maghrebi Arabic", "The Arabic varieties of the central Maghreb."),
			HistoricalAccent("tunisian", "eastern Maghrebi Arabic", "The Arabic varieties of Ifriqiya and Tunis.")
		])
	];

	private static readonly HistoricalScriptSeed[] RenaissanceWorldScripts =
	[
		new("Armenian", "the Armenian script", "an unfamiliar angular alphabet", "The Armenian alphabet is the established manuscript and inscriptional script of Armenian.", "Alphabet", 1.0, 1.0, ["Armenian"]),
		new("Georgian Mkhedruli", "the Georgian Mkhedruli script", "an unfamiliar rounded alphabet", "Mkhedruli is the secular and chancery hand of late-medieval Georgian writing.", "Alphabet", 1.0, 1.0, ["Georgian"]),
		new("Georgian Khutsuri", "the Georgian Khutsuri script", "an unfamiliar ecclesiastical alphabet", "Khutsuri combines Asomtavruli capitals with Nuskhuri minuscule for Georgian ecclesiastical manuscripts.", "Alphabet", 1.0, 1.1, ["Georgian"]),
		new("Arabic", "the Arabic script", "a flowing right-to-left script", "The Arabic-derived abjad writes Arabic and was adapted for Persian, Turkic, Iranian, South Asian and African languages.", "Abjad", 0.8, 1.2, ["Kurmanji Kurdish", "Gorani", "Mashriqi Arabic", "Quranic Arabic", "Persian", "Chagatai", "Hindavi", "Pashto", "Classical Malay", "Somali", "Swahili", "Manding", "Songhay", "Hausa", "Tashelhit", "Central Atlas Tamazight", "Tarifit", "Kabyle", "Nafusi", "Mahgrebi Arabic"]),
		new("Coptic", "the Coptic script", "an unfamiliar Greek-derived alphabet", "The Coptic alphabet writes the late Egyptian language and remains important in Christian liturgy.", "Alphabet", 1.0, 1.0, ["Coptic"]),
		new("Nagari", "the Nagari script", "an unfamiliar South Asian abugida", "Late-medieval Nagari hands write Sanskrit and northern and western Indo-Aryan vernaculars.", "Abugida", 0.9, 1.2, ["Hindavi", "Old Rajasthani", "Sanskrit"]),
		new("Bengali", "the Bengali script", "an unfamiliar eastern South Asian abugida", "The eastern Brahmic script writes Middle Bengali and regional Sanskrit manuscripts.", "Abugida", 0.9, 1.2, ["Middle Bengali", "Sanskrit"]),
		new("Kannada", "the Kannada script", "an unfamiliar southern South Asian abugida", "The late-medieval Kannada hand writes Kannada and Sanskrit in the western Deccan.", "Abugida", 0.9, 1.2, ["Kannada", "Sanskrit"]),
		new("Telugu", "the Telugu script", "an unfamiliar southern South Asian abugida", "The closely related late-medieval Telugu hand writes Telugu and Sanskrit in the eastern Deccan.", "Abugida", 0.9, 1.2, ["Telugu", "Sanskrit"]),
		new("Tamil", "the Tamil script", "an unfamiliar southern South Asian abugida", "The Tamil abugida writes Tamil texts and inscriptions across the southern peninsula.", "Abugida", 0.9, 1.2, ["Tamil"]),
		new("Chinese", "Chinese characters", "an unfamiliar field of complex logographs", "Traditional Chinese characters carry Classical Chinese across East Asia and can also represent regional Sinitic speech.", "Logographic", 0.5, 2.0, ["Ming Guanhua", "Wu Chinese", "Yue", "Southern Min", "Eastern Min", "Northern Min", "Classical Chinese", "Middle Korean", "Late Middle Japanese", "Middle Vietnamese"]),
		new("Korean", "the Hunminjeongeum alphabet", "an unfamiliar alphabet arranged in syllable blocks", "The new Joseon alphabet was completed in 1443 and promulgated in 1446 for Middle Korean.", "Alphabet", 0.75, 1.3, ["Middle Korean"]),
		new("Japanese", "the Japanese kana and kanji system", "an unfamiliar mixture of syllabic and logographic signs", "Muromachi Japanese writing combines kana syllabaries with Chinese-derived kanji.", "Hybrid", 0.75, 1.5, ["Late Middle Japanese"]),
		new("Tibetan", "the Tibetan script", "an unfamiliar Himalayan abugida", "The Tibetan abugida writes Classical Tibetan and the major Tibetic regional languages, preserving a conservative spelling tradition.", "Abugida", 0.8, 1.3, ["Central Tibetan", "Khams Tibetan", "Amdo Tibetan", "Classical Tibetan"]),
		new("Chu Nom", "the Chu Nom script", "an unfamiliar field of Vietnamese-adapted logographs", "Chu Nom adapts and creates Chinese-style characters to represent the Vietnamese vernacular.", "Logographic", 0.5, 2.0, ["Middle Vietnamese"]),
		new("Thai", "the Thai script", "an unfamiliar Southeast Asian abugida", "The Thai abugida writes the Tai language of Ayutthaya and related learned vocabulary.", "Abugida", 0.8, 1.3, ["Ayutthaya Thai"]),
		new("Kawi", "the Kawi script", "an unfamiliar island Southeast Asian abugida", "Kawi is a Brahmi-derived script used from the eighth through sixteenth centuries for Old and Middle Javanese, Sanskrit and Old Malay.", "Abugida", 0.8, 1.3, ["Middle Javanese", "Sanskrit", "Classical Malay"]),
		new("Jawi", "the Jawi script", "a flowing right-to-left Malay script", "Jawi adapts the Arabic script with additional letters for Classical Malay.", "Abjad", 0.8, 1.2, ["Classical Malay"]),
		new("Ethiopic", "the Ethiopic script", "an unfamiliar angular syllabic script", "The Ethiopic abugida originated for Ge'ez and was also used for Amharic and other Ethiopian languages.", "Abugida", 0.8, 1.3, ["Amharic", "Ge'ez"])
	];

	private static readonly HistoricalMutualIntelligibilitySeed[] RenaissanceWorldMutualIntelligibilities =
	[
		new("Quranic Arabic", "Mashriqi Arabic", Difficulty.ExtremelyHard),
		new("Quranic Arabic", "Mahgrebi Arabic", Difficulty.ExtremelyHard),
		new("Mashriqi Arabic", "Mahgrebi Arabic", Difficulty.VeryHard),
		new("Hindavi", "Old Rajasthani", Difficulty.ExtremelyHard),
		new("Central Tibetan", "Khams Tibetan", Difficulty.VeryHard),
		new("Central Tibetan", "Amdo Tibetan", Difficulty.ExtremelyHard),
		new("Khams Tibetan", "Amdo Tibetan", Difficulty.ExtremelyHard),
		new("Classical Tibetan", "Central Tibetan", Difficulty.ExtremelyHard),
		new("Classical Tibetan", "Khams Tibetan", Difficulty.ExtremelyHard),
		new("Classical Tibetan", "Amdo Tibetan", Difficulty.ExtremelyHard),
		new("Karanga", "Korekore", Difficulty.Hard),
		new("Karanga", "Zezuru", Difficulty.Hard),
		new("Korekore", "Zezuru", Difficulty.VeryHard),
		new("Tashelhit", "Central Atlas Tamazight", Difficulty.VeryHard),
		new("Central Atlas Tamazight", "Tarifit", Difficulty.ExtremelyHard),
		new("Tarifit", "Kabyle", Difficulty.ExtremelyHard),
		new("Kabyle", "Nafusi", Difficulty.ExtremelyHard)
	];
}
