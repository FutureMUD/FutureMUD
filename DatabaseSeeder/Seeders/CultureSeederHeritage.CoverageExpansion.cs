#nullable enable

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private void SeedModernCultureCoverageExpansion()
	{
		SeedHistoricalCultures(ModernBroadCultures);
	}

	private void SeedRenaissanceEuropeCultureCoverageExpansion()
	{
		SeedHistoricalEthnicityVariants(RenaissanceEuropeEthnicityVariants);
		SeedHistoricalCultures(RenaissanceEuropeBroadCultures);
	}

	private static readonly HistoricalEthnicityVariantSeed[] RenaissanceEuropeEthnicityVariants =
	[
		new("Albanian", "Vlach", "Albanian", "Balkan", "Albanian-Speaking Communities",
			"Albanians belong to northern and southern Albanian-speaking clans, towns and lordships across the Adriatic and Balkan borderlands.")
	];

	private static readonly HistoricalCultureSeed[] ModernBroadCultures =
	[
		new("Modern Germanic", "Modern Germanic", "Modern Germanic",
			"A broad modern northwestern and central European culture shaped by German-speaking and Low Countries institutions, urban life, education and family traditions."),
		new("Modern Italic", "Modern Italic", "Modern Italic",
			"A broad modern Italian culture joining strong regional identities through shared civic, culinary, familial and national institutions."),
		new("Modern Iberian", "Modern Iberian", "Modern Iberian",
			"A broad modern Iberian culture encompassing the related but distinct public traditions of Spain, Portugal and their regional communities."),
		new("Modern Celtic", "Modern Celtic", "Modern Celtic",
			"A modern Celtic cultural sphere shaped by Irish, Scottish, Welsh, Breton and diasporic traditions alongside wider state identities."),
		new("Modern Slavic", "Modern Slavic", "Modern Slavic",
			"A broad modern Slavic cultural sphere whose communities share related languages and historical reference points while retaining strong national and regional identities."),
		new("Modern Greek", "Modern Greek", "Modern Greek",
			"Modern Greek culture centres on Greek language, family and civic life, Orthodox traditions and a far-reaching maritime and diasporic world."),
		new("Modern Turkic", "Modern Turkic", "Modern Turkic",
			"A broad modern Turkic cultural sphere linking Anatolian and Central Asian communities through related languages and layered imperial, national and local traditions."),
		new("Modern Arabic", "Modern Arabic", "Modern Arabic",
			"A broad modern Arabic cultural sphere joining many regional societies through Arabic language, literature, media and overlapping religious and historical traditions."),
		new("Modern Persian", "Modern Persian", "Modern Persian",
			"Modern Persian culture is shaped by Persian language and literature, family and civic traditions, and long-standing Iranian and diasporic institutions."),
		new("Modern Scandinavian", "Modern Scandinavian", "Modern Scandinavian",
			"Modern Scandinavian culture joins the closely connected Nordic societies of Denmark, Norway and Sweden while preserving distinct national customs."),
		new("Modern North African", "Modern North African", "Modern North African",
			"Modern North African culture encompasses Amazigh, Arab and other communities shaped by Maghrebi cities, villages, deserts, coasts and diasporas."),
		new("Modern Sub-Saharan", "Modern Sub-Saharan", "Modern Sub-Saharan",
			"A deliberately broad modern sub-Saharan cultural option for games that do not require a more specific West, Central, East or Southern African setting."),
		new("Modern Swahili", "Modern Swahili", "Modern Swahili",
			"Modern Swahili culture belongs to the multilingual coastal and inland communities connected by Kiswahili across eastern Africa."),
		new("Modern Oceanic", "Modern Oceanic", "Modern Oceanic",
			"A broad modern Oceanic culture representing Pacific island communities when a game does not distinguish Polynesian, Melanesian and other local traditions."),
		new("Modern South Asian", "Modern Indian", "Modern Indian",
			"A broad modern South Asian culture for games treating the subcontinent at a regional level while ethnicity and language carry more specific identities."),
		new("Modern Southeast Asian", "Modern Southeast Asian", "Modern Southeast Asian",
			"A broad modern Southeast Asian culture for multilingual mainland and island societies connected by trade, migration and shared regional institutions."),
		new("Modern Afro-Caribbean", "Modern Afro-Caribbean", "Modern Afro-Caribbean",
			"Modern Afro-Caribbean culture is shaped by African diasporic inheritance, Caribbean creole societies, island and mainland histories, and global migration."),
		new("Modern Afro-American", "Modern Afro-American", "Modern Afro-American",
			"Modern African-American culture is shaped by Black American family, religious, artistic, civic and regional traditions formed through slavery, emancipation and continuing community life."),
		new("Modern Indigenous North American", "Modern Indigenous North American", "Modern Indigenous North American",
			"A broad Indigenous North American option for games that do not model individual First Nations, Native American, Inuit or Alaska Native cultures separately."),
		new("Modern Indigenous Latin American", "Modern Indigenous Latin American", "Modern Indigenous Latin American",
			"A broad Indigenous Latin American culture for Andean, Mesoamerican and other Native communities when a game does not require nation-level distinctions."),
		new("Modern Central Asian", "Modern Central Asian", "Modern Central Asian",
			"Modern Central Asian culture encompasses the region's Turkic, Mongolic, Iranian and Tibetic communities through shared histories of pastoralism, cities, empires and modern states."),
		new("Modern Chinese", "Modern Chinese", "Modern Chinese",
			"Modern Chinese culture is a broad civic and civilisational identity containing strong regional, linguistic, religious and diasporic differences."),
		new("Modern Japanese", "Modern Japanese", "Modern Japanese",
			"Modern Japanese culture joins national institutions and mass culture with persistent regional, island, class and family traditions."),
		new("Modern Korean", "Modern Korean", "Modern Korean",
			"Modern Korean culture is shaped by Korean language, family and educational traditions, rapid modernisation, division and a global diaspora."),
		new("Modern Aboriginal Australian", "Modern Aboriginal Australian", "Modern Aboriginal Australian",
			"A broad Aboriginal Australian option for games that do not model the continent's many specific nations, language groups and kinship systems."),
		new("Modern Anglo-Saxon", "Modern Anglo-Saxon", "Modern Anglo-Saxon",
			"A broad modern Anglophone culture shaped by English language institutions and the varied national and diasporic societies that developed around them.")
	];

	private static readonly HistoricalCultureSeed[] RenaissanceEuropeBroadCultures =
	[
		new("Renaissance German Imperial", "German", "German",
			"German imperial culture belongs to the towns, courts, estates and rural communities of the German-speaking Holy Roman Empire. It is a shared political and cultural frame for Bavarian, Swabian, Franconian, Saxon, Austrian and other regional ethnic identities."),
		new("Renaissance Low Countries", "Dutch", "Dutch",
			"Low Countries culture belongs to the densely urbanised counties, duchies, ports and market villages of the Rhine-Meuse-Scheldt delta."),
		new("Renaissance French", "French", "French",
			"Renaissance French culture belongs to the expanding royal kingdom and its court, towns, noble households and provincial communities."),
		new("Renaissance English", "English", "English",
			"Renaissance English culture belongs to the kingdom's shires, towns, ports and court, shaped by common law, parish life and a growing vernacular public sphere."),
		new("Renaissance Northern Italian", "Italian", "Italian",
			"Northern Italian culture belongs to the communes, courts, republics and trading cities of the Po valley and upper peninsula."),
		new("Renaissance Southern Italian", "Italian", "Italian",
			"Southern Italian culture belongs to the kingdoms, baronies, towns and agrarian communities of Naples, Sicily and the southern peninsula."),
		new("Renaissance Iberian", "Iberian", "Iberian",
			"Renaissance Iberian culture is a broad Christian-kingdom frame for Castilian, Leonese, Aragonese, Catalan, Galician and Portuguese ethnic identities."),
		new("Renaissance Basque", "Basque", "Basque",
			"Renaissance Basque culture belongs to the western Pyrenean valleys, ports and chartered communities on both sides of the mountains."),
		new("Renaissance Jewish", "Jewish Male", "Jewish Female",
			"Renaissance Jewish culture belongs to diverse Ashkenazi, Sephardi and Mizrahi communities joined by religion, law, learning, household practice and diasporic networks."),
		new("Renaissance Gaelic", "Irish", "Irish",
			"Renaissance Gaelic culture belongs to Irish and Highland Scottish lordships organised around kinship, learned households, pastoral wealth and customary law."),
		new("Renaissance Welsh", "Welsh", "Welsh",
			"Renaissance Welsh culture belongs to Welsh-speaking communities preserving strong poetic, kinship and regional traditions under English rule."),
		new("Renaissance Breton", "Breton", "Breton",
			"Renaissance Breton culture belongs to the duchy's Breton- and Gallo-speaking towns, parishes, noble houses and Atlantic ports."),
		new("Renaissance Polish", "Polish", "Polish",
			"Renaissance Polish culture belongs to the kingdom's noble commonwealth, towns, villages and Catholic institutions while encompassing strong regional differences."),
		new("Renaissance West Slavic", "Western Slavic", "Western Slavic",
			"West Slavic culture is a broad option for Czech and Slovak communities shaped by central European towns, estates, churches and territorial crowns."),
		new("Renaissance East Slavic", "Eastern Slavic", "Eastern Slavic",
			"East Slavic culture belongs to Ruthenian, Ukrainian and Russian communities linked by Orthodox traditions, related speech and the legacies of Rus."),
		new("Renaissance South Slavic", "Western Slavic", "Western Slavic",
			"South Slavic culture is a broad option for Croat, Serb, Bosniak and neighbouring Balkan communities divided among Adriatic, Hungarian and Ottoman political worlds."),
		new("Renaissance Baltic", "Finno-Ugric", "Finno-Ugric",
			"Baltic culture is a broad regional option for Lithuanian, Latvian and Prussian communities; its inherited naming key is retained for compatibility despite that key's older linguistic simplification."),
		new("Renaissance Estonian", "Finno-Ugric", "Finno-Ugric",
			"Renaissance Estonian culture belongs to Finnic-speaking rural and urban communities under Livonian and Baltic German institutions."),
		new("Renaissance Hungarian", "Hungarian", "Hungarian",
			"Renaissance Hungarian culture belongs to the crown lands' noble counties, market towns, villages and frontier communities."),
		new("Renaissance Danish", "Danish", "Danish",
			"Renaissance Danish culture belongs to the Danish crown's islands, Jutland towns, estates and maritime networks."),
		new("Renaissance Swedish", "Swedish", "Swedish",
			"Renaissance Swedish culture belongs to the Swedish crown's farming districts, mining regions, towns and Baltic-facing communities."),
		new("Renaissance Icelandic", "Danish", "Danish",
			"Renaissance Icelandic culture belongs to dispersed farming households linked through assemblies, church institutions, law and manuscript tradition."),
		new("Renaissance Byzantine Roman", "Hellenic", "Hellenic",
			"Late Byzantine Roman culture belongs to Greek-speaking Orthodox communities carrying the civic, courtly and religious inheritance of the eastern Roman world."),
		new("Renaissance Ottoman", "Turkish", "Turkish",
			"Ottoman culture belongs to the multilingual court, military, urban and provincial institutions of the expanding Ottoman state."),
		new("Renaissance Cossack Borderland", "Turkish", "Turkish",
			"Cossack borderland culture belongs to mobile military communities forming along the steppe frontier from Slavic, Turkic and other local populations."),
		new("Renaissance Arabic", "Levantine", "Levantine",
			"Renaissance Arabic culture is a broad frame for Arabic-speaking urban, rural and tribal communities of the Levant and neighbouring lands."),
		new("Renaissance Persian", "Persian", "Persian",
			"Renaissance Persian culture belongs to Persian-speaking courts, towns, estates and learned networks across Iran and the wider Persianate world."),
		new("Renaissance Maghrebi", "Morrocan", "Morrocan",
			"Renaissance Maghrebi culture belongs to the Amazigh and Arab cities, villages, mountain communities, oases and tribal confederations of North Africa."),
		new("Renaissance Albanian", "Albanian", "Albanian",
			"Renaissance Albanian culture belongs to northern and southern Albanian-speaking clans, towns and lordships navigating Venetian, Ottoman and Adriatic political worlds.")
	];
}
