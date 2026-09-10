#nullable enable

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private void SeedDarkAgesAndMedievalHeritageExpansion()
	{
		SeedHistoricalEthnicityVariants(DarkAgesAndMedievalEthnicityVariants);
	}

	private static readonly HistoricalEthnicityVariantSeed[] DarkAgesAndMedievalEthnicityVariants =
	[
		new("Medieval West Saxon", "Early Anglo-Saxon", "Medieval Early Anglo-Saxon", "Germanic", "West Saxon Kingdom",
			"West Saxons belong to the southern English communities centred on Wessex, with strong royal, monastic and shire identities."),
		new("Medieval Mercian", "Early Anglo-Saxon", "Medieval Early Anglo-Saxon", "Germanic", "Mercian Midlands",
			"Mercians belong to the English-speaking peoples of the Midlands and the former Mercian sphere."),
		new("Medieval Northumbrian", "Early Anglo-Saxon", "Medieval Early Anglo-Saxon", "Germanic", "Northumbrian Kingdoms",
			"Northumbrians belong to the northern English communities shaped by Bernician, Deiran and monastic traditions."),

		new("Danelaw English", "Anglo-Danish English", "Medieval Anglo-Danish", "North Sea Germanic", "Danelaw English",
			"Danelaw English descend from English-speaking communities living under long Scandinavian settlement and law."),
		new("Anglo-Dane", "Anglo-Danish English", "Medieval Anglo-Danish", "North Sea Germanic", "Mixed Anglo-Scandinavian",
			"Anglo-Danes belong to families formed within the mixed English and Scandinavian communities of the Danelaw."),

		new("Viking-Age Norwegian", "Viking-Age Norse", "Medieval Norse", "Nordic", "Norwegian Norse",
			"Norwegian Norse belong to the coastal, valley and island communities of western Scandinavia."),
		new("Viking-Age Dane", "Viking-Age Norse", "Medieval Norse", "Nordic", "Danish Norse",
			"Danish Norse belong to the communities of Jutland, Zealand and the Danish islands."),
		new("Viking-Age Swede", "Viking-Age Norse", "Medieval Norse", "Nordic", "Swedish Norse",
			"Swedish Norse belong to the communities around Svealand, Gotaland and the Baltic-facing trade routes."),

		new("Cotentin Norman", "Conquest-Era Norman", "Medieval Norman", "Western European", "Cotentin Normandy",
			"Cotentin Normans belong to the western Norman peninsula, where seafaring and ducal military service are prominent."),

		new("Marcher Anglo-Norman", "Anglo-Norman", "Medieval Anglo-Norman", "Western European", "Welsh and Scottish Marches",
			"Marcher Anglo-Normans belong to settler and military families established along the contested western and northern frontiers."),

		new("High Medieval Northern English", "High Medieval English", "Medieval High English British", "Western European", "Northern England",
			"Northern English communities retain strong regional customs shaped by Northumbria, Scandinavian settlement and the borderlands."),
		new("High Medieval Midland English", "High Medieval English", "Medieval High English British", "Western European", "English Midlands",
			"Midland English communities belong to the market towns, villages and shires of central England."),

		new("Medieval Connacht Gael", "Medieval Irish Gael", "Medieval Irish Gaelic", "Celtic", "Connacht",
			"Connacht Gaels belong to the western Irish kingdoms and kin groups associated with Connacht."),
		new("Medieval Munster Gael", "Medieval Irish Gael", "Medieval Irish Gaelic", "Celtic", "Munster",
			"Munster Gaels belong to the southern Irish kingdoms and kin groups associated with Munster."),

		new("Medieval Highland Gael", "High Medieval Scottish Gael", "Medieval Scottish Gaelic Lowland", "Celtic and Western European", "Scottish Highlands",
			"Highland Gaels belong to the Gaelic-speaking kindreds of the Highlands and western seaboard."),
		new("Medieval Gall-Gaidheal", "High Medieval Scottish Gael", "Medieval Scottish Gaelic Lowland", "Celtic and Nordic", "Hebrides and Galloway",
			"The Gall-Gaidheil belong to mixed Norse-Gaelic communities of the Hebrides, western coasts and Galloway."),

		new("Carolingian Austrasian Frank", "Carolingian Frank", "Medieval Carolingian Frankish", "Germanic and Romance", "Austrasia",
			"Austrasian Franks belong to the eastern Frankish heartlands along the Rhine and Meuse."),
		new("Carolingian Neustrian Frank", "Carolingian Frank", "Medieval Carolingian Frankish", "Germanic and Romance", "Neustria",
			"Neustrian Franks belong to the western Frankish lands where Romance speech and Frankish political identity intermingled."),

		new("Capetian Francien", "High Medieval French", "Medieval Capetian French", "Western European", "Ile-de-France",
			"Franciens belong to the central royal lands around Paris and Orleans that increasingly shaped Capetian institutions."),
		new("Capetian Picard", "High Medieval French", "Medieval Capetian French", "Western European", "Picardy and Artois",
			"Picards belong to the northern French towns and lordships of Picardy and neighbouring Artois."),

		new("High Medieval Bavarian", "High Medieval German", "Medieval Imperial German", "Germanic", "Bavaria",
			"Bavarians belong to the southern German duchy and its Alpine, Danubian and ecclesiastical communities."),
		new("High Medieval Swabian", "High Medieval German", "Medieval Imperial German", "Germanic", "Swabia",
			"Swabians belong to the Alemannic southwest of the Empire, including its towns, lordships and old ducal lands."),
		new("High Medieval Franconian", "High Medieval German", "Medieval Imperial German", "Germanic", "Franconia",
			"Franconians belong to the Main and middle Rhine regions associated with the old Frankish stem duchy."),
		new("High Medieval Saxon", "High Medieval German", "Medieval Imperial German", "Germanic", "Saxony",
			"Saxons belong to the northern and eastern German lands shaped by the old Saxon duchy and later territorial lordships."),
		new("High Medieval Thuringian", "High Medieval German", "Medieval Imperial German", "Germanic", "Thuringia",
			"Thuringians belong to the central uplands and towns of Thuringia between Saxon and Franconian spheres."),
		new("High Medieval Frisian", "High Medieval German", "Medieval Imperial German", "Germanic", "Frisian Coast",
			"Frisians belong to the autonomous coastal communities along the North Sea, organised around maritime trade and local law."),
		new("High Medieval Low German", "High Medieval German", "Medieval Imperial German", "Germanic", "Low German North",
			"Low Germans belong to the northern towns and rural communities whose speech and commerce face the North and Baltic seas."),

		new("High Medieval Castilian", "High Medieval Castilian", "Medieval Christian Iberian", "Iberian", "Castile",
			"Castilians belong to the expanding central Iberian kingdoms and frontier communities of Castile."),
		new("High Medieval Leonese", "High Medieval Castilian", "Medieval Christian Iberian", "Iberian", "Leon",
			"Leonese belong to the western kingdom of Leon and its Asturian and Duero valley communities."),
		new("High Medieval Aragonese", "High Medieval Castilian", "Medieval Christian Iberian", "Iberian", "Aragon",
			"Aragonese belong to the Pyrenean and Ebro communities of the Crown of Aragon."),
		new("High Medieval Catalan", "High Medieval Castilian", "Medieval Christian Iberian", "Iberian", "Catalonia",
			"Catalans belong to the counties and maritime towns of Catalonia within the Crown of Aragon."),
		new("High Medieval Galician", "High Medieval Castilian", "Medieval Christian Iberian", "Iberian", "Galicia",
			"Galicians belong to the northwestern Iberian communities of Galicia, with close ties to Leon and Portugal."),
		new("High Medieval Portuguese", "High Medieval Castilian", "Medieval Christian Iberian", "Iberian", "Portugal",
			"Portuguese belong to the Atlantic kingdom and frontier communities that emerged from the county of Portugal."),
		new("High Medieval Basque", "High Medieval Castilian", "Medieval Christian Iberian", "Iberian", "Basque Country and Navarre",
			"Basques belong to the western Pyrenean communities of Navarre and the neighbouring Basque-speaking valleys."),

		new("High Medieval Andalusi Arab", "High Medieval Andalusi", "Medieval Andalusian Arabic", "Iberian and Middle Eastern", "Andalusi Arab",
			"Andalusi Arabs belong to Arabic-speaking urban, landed and scholarly families of al-Andalus."),
		new("High Medieval Andalusi Berber", "High Medieval Andalusi", "Medieval Andalusian Arabic", "Iberian and North African", "Andalusi Amazigh",
			"Andalusi Berbers descend from Amazigh settlers and military communities established throughout al-Andalus."),
		new("High Medieval Muladi", "High Medieval Andalusi", "Medieval Andalusian Arabic", "Iberian", "Muladi",
			"Muladis descend from local Iberian families incorporated into the Arabic-speaking Muslim society of al-Andalus."),
		new("High Medieval Mozarab", "High Medieval Andalusi", "Medieval Andalusian Arabic", "Iberian", "Mozarab",
			"Mozarabs belong to Christian communities living within or shaped by the Arabic-speaking society of al-Andalus."),

		new("Middle Byzantine Constantinopolitan", "Middle Byzantine Roman", "Medieval Byzantine Greek", "Greek and Eastern Roman", "Constantinople",
			"Constantinopolitans belong to the cosmopolitan capital and its court, guild, clerical and maritime communities."),
		new("Middle Byzantine Anatolian Greek", "Middle Byzantine Roman", "Medieval Byzantine Greek", "Greek and Eastern Roman", "Anatolia",
			"Anatolian Greeks belong to the towns, villages and military districts of Byzantine Asia Minor."),

		new("Abbasid-Era Iraqi Arab", "Abbasid-Era Iraqi Arab", "Medieval Abbasid Arabic", "Middle Eastern", "Iraqi Arab",
			"Iraqi Arabs belong to the urban and tribal Arabic-speaking communities of lower and central Mesopotamia."),
		new("Abbasid-Era Persian", "Abbasid-Era Iraqi Arab", "Medieval Abbasid Arabic", "Iranian", "Abbasid Persian",
			"Abbasid-era Persians belong to Iranian families active in the caliphate's administration, scholarship, towns and eastern provinces."),
		new("Abbasid-Era Syriac", "Abbasid-Era Iraqi Arab", "Medieval Abbasid Arabic", "Aramaean", "Syriac Christian",
			"Syriac communities preserve Aramaic-speaking Christian traditions across Mesopotamia and the eastern provinces."),

		new("Fatimid-Era Egyptian Arab", "Fatimid-Era Egyptian Arab", "Medieval Fatimid Arabic", "North African and Middle Eastern", "Egyptian Arab",
			"Egyptian Arabs belong to Arabic-speaking urban, village and tribal communities of the Fatimid realm."),
		new("Fatimid-Era Copt", "Fatimid-Era Egyptian Arab", "Medieval Fatimid Arabic", "Egyptian", "Coptic Egyptian",
			"Copts belong to Egypt's indigenous Christian communities, with strong village, clerical and administrative traditions."),
		new("Fatimid-Era Kutama", "Fatimid-Era Egyptian Arab", "Medieval Fatimid Arabic", "North African", "Kutama Amazigh",
			"The Kutama belong to the Amazigh communities whose military support was central to the early Fatimid state."),

		new("High Medieval Oghuz Turk", "High Medieval Oghuz Turk", "Medieval Seljuk Ayyubid Mamluk", "Turkic and Middle Eastern", "Oghuz Turk",
			"Oghuz Turks belong to the pastoral, military and settled Turkic communities of the Seljuk and successor realms."),
		new("High Medieval Kurdish", "High Medieval Oghuz Turk", "Medieval Seljuk Ayyubid Mamluk", "Iranian", "Kurdish Principalities",
			"Kurds belong to the highland tribes, towns and military households spanning northern Mesopotamia, Syria and western Iran."),
		new("High Medieval Syrian Arab", "High Medieval Oghuz Turk", "Medieval Seljuk Ayyubid Mamluk", "Middle Eastern", "Syrian Arab",
			"Syrian Arabs belong to the Arabic-speaking urban, village and tribal communities of Syria and the Jazira."),
		new("High Medieval Kipchak Mamluk", "High Medieval Oghuz Turk", "Medieval Seljuk Ayyubid Mamluk", "Central Asian", "Kipchak Military Household",
			"Kipchak Mamluks belong to military households recruited from the Eurasian steppe and trained within Islamic courts."),

		new("Conquest-Period Szekely", "Conquest-Period Magyar", "Medieval Magyar", "Magyar", "Szekely Frontier",
			"Szekelys belong to Hungarian-speaking frontier communities organised around military service in the eastern Carpathians."),
		new("Conquest-Period Pecheneg", "Conquest-Period Magyar", "Medieval Magyar", "Turkic", "Pecheneg Settlers",
			"Pechenegs belong to Turkic steppe communities incorporated into the Hungarian kingdom as settlers and frontier soldiers."),

		new("High Medieval Novgorodian Rus", "High Medieval Rus", "Medieval Rus Novgorod", "Eastern Slavic", "Novgorod Land",
			"Novgorodians belong to the northern Rus trading republic and its Slavic, Finnic and Norse-connected hinterland."),
		new("High Medieval Kievan Rus", "High Medieval Rus", "Medieval Rus Novgorod", "Eastern Slavic", "Kievan Lands",
			"Kievan Rus belong to the southern principalities and river communities centred on Kyiv and the middle Dnieper."),

		new("High Medieval Cuman", "High Medieval Kipchak Turk", "Medieval Steppe Turkic Mongol", "Central Asian", "Cuman-Kipchak Steppe",
			"Cumans belong to the western Kipchak confederations of the Pontic and Caspian steppes."),
		new("High Medieval Mongol", "High Medieval Kipchak Turk", "Medieval Steppe Turkic Mongol", "Central Asian", "Mongol Confederations",
			"Mongols belong to the eastern steppe clans and confederations united through pastoral mobility and military households."),
		new("High Medieval Alan", "High Medieval Kipchak Turk", "Medieval Steppe Turkic Mongol", "Iranian", "Alan Steppe and Caucasus",
			"Alans belong to Iranian-speaking mounted communities of the northern Caucasus and western steppe."),

		new("Early Medieval Chauhan Rajput", "Early Medieval Rajput", "Medieval North Indian Rajput", "South Asian", "Chauhan Clan",
			"Chauhans belong to a prominent Rajput lineage associated with Ajmer, Sambhar and later Delhi."),
		new("Early Medieval Paramara Rajput", "Early Medieval Rajput", "Medieval North Indian Rajput", "South Asian", "Paramara Clan",
			"Paramaras belong to a Rajput lineage associated with Malwa and neighbouring central Indian courts."),

		new("Chola-Era Tamil", "Chola-Era Tamil", "Medieval South Indian Chola", "South Asian", "Tamil Country",
			"Tamils belong to the core agrarian, temple, mercantile and military communities of the Chola realm."),
		new("Chola-Era Telugu", "Chola-Era Tamil", "Medieval South Indian Chola", "South Asian", "Telugu Country",
			"Telugus belong to eastern Deccan communities linked to the Chola sphere through war, marriage, trade and administration."),

		new("Song Northern Han", "Song Han Chinese", "Medieval Song Chinese", "East Asian", "North China",
			"Northern Han communities belong to the Yellow River plains and northern prefectures shaped by migration and frontier rule."),
		new("Song Jiangnan Han", "Song Han Chinese", "Medieval Song Chinese", "East Asian", "Jiangnan",
			"Jiangnan Han communities belong to the prosperous lower Yangtze cities, market towns and rice-growing countryside."),
		new("Song Wu-Speaking Han", "Song Han Chinese", "Medieval Song Chinese", "East Asian", "Wu Region",
			"Wu-speaking Han communities belong to the lower Yangtze and Taihu regions with strong urban and mercantile traditions."),
		new("Song Min-Speaking Han", "Song Han Chinese", "Medieval Song Chinese", "East Asian", "Fujian",
			"Min-speaking Han communities belong to Fujian's coastal, river-valley and mountain settlements."),

		new("Goryeo Gyeonggi Korean", "Goryeo Korean", "Medieval Goryeo Korean", "East Asian", "Gyeonggi",
			"Gyeonggi Koreans belong to communities around the Goryeo capital and the central court region."),
		new("Goryeo Gyeongsang Korean", "Goryeo Korean", "Medieval Goryeo Korean", "East Asian", "Gyeongsang",
			"Gyeongsang Koreans belong to the southeastern communities descended from the former Silla heartland."),

		new("Heian-Kamakura Kinai Japanese", "Heian-Kamakura Japanese", "Medieval Heian Kamakura Japanese", "East Asian", "Kinai",
			"Kinai Japanese belong to the courtly, temple and provincial communities around Kyoto, Nara and the central home provinces."),
		new("Heian-Kamakura Kanto Japanese", "Heian-Kamakura Japanese", "Medieval Heian Kamakura Japanese", "East Asian", "Kanto",
			"Kanto Japanese belong to the eastern warrior, estate and village communities associated with Kamakura rule.")
	];
}
