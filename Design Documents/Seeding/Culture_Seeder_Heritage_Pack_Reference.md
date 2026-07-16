# Culture Seeder Heritage Pack Reference

## Purpose

This document is the maintainer reference for real-world `CultureSeeder` ethnicity and culture content. It is the heritage companion to the [Culture Seeder Language Pack Reference](./Culture_Seeder_Language_Pack_Reference.md).

The stock catalogue separates three concepts:

- a **naming culture** describes how names are formed;
- an **ethnicity** describes a specific inherited or community peoplehood and carries the most specific naming-culture link;
- a **culture** describes adopted social, regional, courtly, political or civic customs and is intentionally selectable independently of ethnicity.

This permits, for example, Bavarian, Swabian, Franconian, Saxon, Thuringian, Frisian and Low German ethnicities to use the same broad `Medieval Imperial German (c. 1200)` culture without claiming that those ethnic identities are interchangeable. The culture and ethnicity records do not impose a hard compatibility relationship: builders may restrict combinations with availability FutureProgs if a particular game requires it.

## Pack Coverage

| Pack | Specific ethnicities | Broader cultures | Heritage status |
| --- | ---: | ---: | --- |
| Earth-Modern | 36 established ethnicity records | 26 regional cultures plus 16 established socioeconomic cultures | Expanded regional-culture coverage |
| Earth-Antiquity | 56 specific ethnicity records | 25 regional, civic and status cultures | Existing reference implementation |
| Earth-DarkAgesAndMedieval | 25 compatibility/base ethnicities plus 64 specific variants | 25 polity or regional cultures | Expanded specific-ethnicity coverage |
| Earth-RenaissanceEurope | 51 specific ethnicity records | 29 regional, political and diasporic cultures | New broad-culture coverage |
| Earth-RenaissanceWorldExpansion | 25 compatibility/base ethnicities plus 100 specific variants | 25 polity or regional cultures | Expanded specific-ethnicity coverage |

The base ethnicity in each generated historical naming row is retained as a stable compatibility record. Specific variants are additive and use era-qualified names so running multiple packs cannot overwrite a similarly named people from another period.

## Modelling Policy

Use a separate ethnicity when the distinction represents a durable peoplehood, regional descent community, clan-family complex or historically recognised ethno-regional identity. Examples include:

- Bavarian, Swabian, Franconian, Saxon, Frisian and Low German within the medieval Empire;
- Castilian, Leonese, Aragonese, Catalan, Galician, Portuguese and Basque within Christian Iberia;
- Egyptian Arab, Syrian Arab, Copt, Circassian Mamluk and Turkic Mamluk within the Mamluk political sphere;
- Kannadiga, Telugu, Tamil and Tuluva within the Vijayanagara sphere;
- the regional Han communities represented by Northern, Jiangnan, Wu, Gan, Hakka, Yue and the three Min branches;
- Darod, Hawiye, Isaaq, Dir and Digil-Mirifle Somali clan-family groupings;
- Karanga, Zezuru, Korekore and Manyika within the late-medieval Shona world.

Use a culture when the identity can be learned, adopted or shared across several ethnicities. Cultures may describe a polity (`Renaissance German Imperial`), court and administrative world (`Timurid Persianate Court`), regional civic sphere (`Renaissance Low Countries`) or diasporic religious society (`Renaissance Jewish`).

Religion alone is not modelled as ethnicity. Where religious communities also formed durable historical peoplehoods, their descriptions identify the community rather than asserting a biological distinction.

## Dark Ages and Medieval Matrix

Every one of the 25 naming cultures has one stable base ethnicity and at least one more specific option. Particularly composite rows include:

| Naming culture | Specific ethnicity coverage |
| --- | --- |
| Medieval Early Anglo-Saxon | West Saxon; Mercian; Northumbrian |
| Medieval Anglo-Danish | Danelaw English; Anglo-Dane |
| Medieval Norse | Norwegian; Danish; Swedish Norse |
| Medieval Irish Gaelic | Connacht; Munster Gael |
| Medieval Scottish Gaelic Lowland | Highland Gael; Gall-Gaidheal |
| Medieval Carolingian Frankish | Austrasian; Neustrian Frank |
| Medieval Imperial German | Bavarian; Swabian; Franconian; Saxon; Thuringian; Frisian; Low German |
| Medieval Christian Iberian | Castilian; Leonese; Aragonese; Catalan; Galician; Portuguese; Basque |
| Medieval Andalusian Arabic | Andalusi Arab; Andalusi Berber; Muladi; Mozarab |
| Medieval Byzantine Greek | Constantinopolitan; Anatolian Greek |
| Medieval Abbasid Arabic | Iraqi Arab; Persian; Syriac |
| Medieval Fatimid Arabic | Egyptian Arab; Copt; Kutama Amazigh |
| Medieval Seljuk Ayyubid Mamluk | Oghuz Turk; Kurdish; Syrian Arab; Kipchak Mamluk |
| Medieval Magyar | Szekely; Pecheneg |
| Medieval Rus Novgorod | Novgorodian; Kievan Rus |
| Medieval Steppe Turkic Mongol | Cuman; Mongol; Alan |
| Medieval North Indian Rajput | Chauhan; Paramara Rajput |
| Medieval South Indian Chola | Tamil; Telugu |
| Medieval Song Chinese | Northern; Jiangnan; Wu-speaking; Min-speaking Han |
| Medieval Goryeo Korean | Gyeonggi; Gyeongsang Korean |
| Medieval Heian Kamakura Japanese | Kinai; Kanto Japanese |

The remaining Norman, Anglo-Norman, high-English and Capetian rows likewise receive regional variants. The live seed array is authoritative for exact names and descriptions.

## Renaissance Europe Matrix

The established 50-ethnicity catalogue is retained and joined by the previously missing Albanian ethnicity. Its specific records now sit alongside 29 broader cultures, including:

- `Renaissance German Imperial` for German, Austrian and other HRE regional identities;
- `Renaissance Northern Italian` and `Renaissance Southern Italian` for the many existing Italian ethnicities;
- `Renaissance Iberian` for Castilian, Catalan, Galician and Portuguese identities, with a separate Basque culture;
- `Renaissance Jewish` for Ashkenazi, Sephardi and Mizrahi communities;
- separate Gaelic, Welsh and Breton cultures;
- West, East and South Slavic cultures plus Polish, Hungarian, Baltic and Estonian options;
- Danish, Swedish and Icelandic cultures rather than one fictive uniform Scandinavian identity;
- Byzantine Roman, Ottoman, Cossack borderland, Arabic, Persian, Maghrebi and Albanian cultures.

The inherited `Finno-Ugric` naming key remains in use for Baltic and Estonian records because persisted name-culture names are compatibility contracts. The culture descriptions explicitly identify this as an older catalogue simplification.

## Renaissance World Matrix

Every one of the 25 naming cultures has a base compatibility ethnicity plus specific regional or peoplehood options. The largest composite rows are:

| Naming culture | Specific ethnicity coverage |
| --- | --- |
| Renaissance Mamluk Arabic | Egyptian Arab; Syrian Arab; Egyptian Copt; Circassian Mamluk; Turkic Mamluk; Levantine Bedouin |
| Renaissance Timurid Persianate | Chagatai Turk; Tajik Persian; Khorasani Persian; Timurid Mongol |
| Renaissance Delhi Sultanate | Hindustani; Afghan Pashtun; Punjabi; Delhi Turk; Indo-Persian |
| Renaissance Vijayanagara | Kannadiga; Telugu; Tamil; Tuluva |
| Renaissance Ming Chinese | Northern; Jiangnan; Wu; Gan; Hakka; Yue; Southern Min; Eastern Min; Northern Min Han |
| Renaissance Joseon Korean | Gyeonggi; Gyeongsang; Jeolla; Hamgyong; Jeju |
| Renaissance Muromachi Japanese | Kinai; Kanto; Tohoku; Kyushu Japanese |
| Renaissance Tibetan | U-Tsang; Khampa; Amdo Tibetan |
| Renaissance Majapahit Javanese | Central Javanese; Eastern Javanese; Sundanese; Madurese |
| Renaissance Malay | Malaccan; Sumatran; Bornean Malay; Acehnese |
| Renaissance Ethiopian Highland | Amhara; Tigrayan; Agaw |
| Renaissance Somali | Darod; Hawiye; Isaaq; Dir; Digil-Mirifle |
| Renaissance Swahili | Lamu; Mombasa; Zanzibar; Kilwa Swahili |
| Renaissance Mande Sahelian | Mandinka; Soninke; Bambara; Songhay |
| Renaissance Hausa | Kano; Katsina; Gobir; Zazzau Hausa |
| Renaissance Kongo | Mpemba; Mbata; Soyo Bakongo |
| Renaissance Shona | Karanga; Zezuru; Korekore; Manyika |
| Renaissance Amazigh | Tashelhit; Central Atlas; Riffian; Kabyle; Nafusi Amazigh; Maghrebi Arab |

Armenian, Georgian, Kurdish, Rajput, Bengali, Dai Viet and Ayutthaya rows also receive multiple specific options. The live seed array is authoritative for the exact catalogue.

## Modern Regional Cultures

The original Modern socioeconomic cultures remain useful and are not removed. The regional catalogue complements them with 26 cultures matching the modern naming and ethnicity coverage, including Chinese, Japanese, Korean, South Asian, Southeast Asian, Central Asian, Oceanic, North African, sub-Saharan, Swahili, Indigenous American and Aboriginal Australian options.

These cultures are deliberately broad. A game that needs nation, language, religion or community-level distinctions should add narrower culture records while retaining the stock entries as fallbacks.

## Repeatability Contract

Heritage seeding uses the following stable keys:

- ethnicity: case-insensitive ethnicity name;
- culture: case-insensitive culture name;
- ethnicity naming link: ethnicity id plus gender;
- culture naming link: culture id plus gender;
- ethnicity characteristic: ethnicity id plus characteristic definition and profile.

Rerunning a pack:

- updates stock ethnicity and culture descriptions and metadata;
- replaces the stock five-gender naming links without duplicating them;
- recreates a characteristic join when its profile has drifted, because the profile id is part of that join's composite key;
- preserves compatibility/base ethnicities while upserting era-qualified specific variants;
- falls back to the template ethnicity's naming link or the stock `Simple` / `Given and Family` name culture when heritage is installed without the matching name package.

## Verification Contract

`CultureSeederHeritageCoverageTests` verifies:

- both generated historical packs have ethnicity coverage for all 25 naming cultures;
- every Dark Ages row has at least a base and one specific ethnicity;
- every Renaissance World row has at least a base and two specific ethnicities;
- the German, Iberian, Mamluk and Ming composite rows retain their intended specific groups;
- Modern and Renaissance Europe broad-culture catalogues retain their exact stock counts;
- a real two-pass Dark Ages expansion creates no duplicates, keeps five gender-specific naming links and repairs a deliberately drifted characteristic-profile join.

## Research Anchors

The catalogue is a playable abstraction informed by broad scholarly classifications rather than a claim that pre-modern identity was fixed or equivalent to modern nationality:

- [Holy Roman Empire and the growth of a German imperial identity](https://www.britannica.com/place/Holy-Roman-Empire)
- [The Mamluks in Egypt and Syria, New Cambridge History of Islam](https://cris.haifa.ac.il/en/publications/the-maml%C5%ABks-in-egypt-and-syria-the-turkish-maml%C5%ABk-sultanate-648-7/)
- [UNESCO Silk Roads: Timurid cultural and political setting](https://en.unesco.org/silkroad/knowledge-bank/timurid-empire)
- [Encyclopaedia Iranica: Delhi Sultanate](https://www.iranicaonline.org/articles/delhi-sultanate/)
- [Metropolitan Museum: Art of the Vijayanagara Empire](https://www.metmuseum.org/toah/hd/vija/hd_vija.htm)
- [Phonemic evidence for the major historical Chinese regional language groupings](https://arxiv.org/abs/1802.05820)
- [Kikongo language-cluster and regional history study](https://pmc.ncbi.nlm.nih.gov/articles/PMC3695462/)
- [Cambridge study on the later creation of a unified Shona standard](https://www.cambridge.org/core/journals/journal-of-african-history/article/abs/early-missionaries-and-the-ethnolinguistic-factor-during-the-invention-of-tribalism-in-zimbabwe/432B1DC1B78AD3CBAD4F9EA331A6BB88)

Ethnicity labels and boundaries remain builder-facing game judgements. They should be revised conservatively when better scholarship or a game's own setting assumptions require a different abstraction.
