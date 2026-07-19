#nullable enable

using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
    private void SeedModernLanguages()
    {
        #region Languages

        AddLanguage("English", "an unknown germanic language");
        AddLanguage("German", "an unknown germanic language");
        AddLanguage("French", "an unknown romance language");
        AddLanguage("Italian", "an unknown romance language");
        AddLanguage("Spanish", "an unknown romance language");
        AddLanguage("Dutch", "an unknown germanic language");
        AddLanguage("Russian", "an unknown slavic language");
        AddLanguage("Polish", "an unknown slavic language");
        AddLanguage("Mandarin", "an unknown east asian language");
        AddLanguage("Yue", "an unknown east asian language");
        AddLanguage("Japanese", "an unknown east asian language");
        AddLanguage("Korean", "an unknown east asian language");
        AddLanguage("Arabic", "an unknown middle-eastern language");
        AddLanguage("Hebrew", "an unknown middle-eastern language");
        AddLanguage("Greek", "an unknown romance language");
        AddLanguage("Portugese", "an unknown romance language");
        AddLanguage("Ukranian", "an unknown slavic language");
        AddLanguage("Belarusian", "an unknown slavic language");
        AddLanguage("Farsi", "an unknown middle-eastern language");
        AddLanguage("Irish Gaelic", "an unknown celtic language");
        AddLanguage("Scottish Gaelic", "an unknown celtic language");
        AddLanguage("Afrikaans", "an unknown germanic language");
        AddLanguage("Navajo", "an unknown north american language");
        AddLanguage("Yupik", "an unknown north american language");
        AddLanguage("Apache", "an unknown north american language");
        AddLanguage("Sioux", "an unknown north american language");
        AddLanguage("Cherokee", "an unknown north american language");
        AddLanguage("Hindi", "an unknown asian language");
        AddLanguage("Malay", "an unknown asian language");
        AddLanguage("Bengali", "an unknown asian language");
        AddLanguage("Hausa", "an unknown african language");
        AddLanguage("Swahili", "an unknown african language");
        AddLanguage("Turkish", "an unknown middle-eastern language");
        AddLanguage("Swedish", "an unknown germanic language");
        AddLanguage("Norwegian", "an unknown germanic language");
        AddLanguage("Danish", "an unknown european language");
        AddLanguage("Finnish", "an unknown european language");
        AddLanguage("Romani", "an unknown european language");

        #endregion

        #region Scripts

        AddScript("Latin", "the Latin script", "a European script",
            "The latin script is based on the script used by Roman speakers of the Latin language, and widely used across Western Europe. There are small variations between languages to show sounds unique to that language but it is generally very similar across languages.",
            "Alphabet", 1.0, 1.0, "English", "German", "French", "Italian", "Spanish", "Dutch", "Polish", "Portugese",
            "Irish Gaelic", "Scottish Gaelic", "Afrikaans", "Navajo", "Yupik", "Apache", "Sioux", "Cherokee", "Swedish",
            "Norwegian", "Danish", "Finnish", "Romani");
        AddScript("Cryllic", "the Cryllic script", "a European script",
            "The Cryllic script is based on the Old Church Slavonic script and used by a variety of Slavic and former Soviet languages. There are small variations between languages to show sounds unique to that language but it is generally very similar across languages.",
            "Alphabet", 1.0, 1.0, "Russian", "Ukranian", "Belarusian");
        AddScript("Greek", "the Greek script", "a European script",
            "The Greek script was once widely used across the world but is nowadays mostly limited to the use of the Greek language and as a source of symbols for Mathematics.",
            "Alphabet", 1.0, 1.0, "Greek");
        AddScript("Japanese", "the Japanese script", "an East Asian script",
            "Japanese writing combines kana syllabaries with kanji logographs.",
            "Hybrid", 0.75, 1.5, "Japanese");
        AddScript("Korean", "the Korean script", "an East Asian script",
            "The Korean script, also known as Hangul, is a featural alphabet arranged into syllable blocks.",
            "Alphabet", 0.75, 1.5, "Korean");
        AddScript("Chinese", "the Chinese script", "an East Asian script",
            "Chinese writing uses logographs shared across the written standards represented by Mandarin and Yue.",
            "Logographic", 0.5, 2.0, "Mandarin", "Yue");
        AddScript("Hebrew", "the Hebrew script", "a right-to-left script",
            "The Hebrew abjad writes Hebrew from right to left, with optional marks for vowels and cantillation.",
            "Abjad", 0.8, 1.2, "Hebrew");
        AddScript("Arabic", "the Arabic script", "a flowing right-to-left script",
            "The Arabic abjad writes Arabic and, with additional letters, modern Persian from right to left.",
            "Abjad", 0.8, 1.2, "Arabic", "Farsi");
        AddScript("Sanskrit", "the Devanagari script", "a South Asian script",
            "This legacy-named script record represents the Devanagari abugida used for Hindi and many Sanskrit texts.",
            "Abugida", 0.8, 1.3, "Hindi");

        #endregion

        #region Accents

        AddAccent("Afrikaans", "kaapse", "with a Kaapse accent", "with a Kaapse accent", 2,
            "The accent of the Western Cape (Kaapse)", "african");
        AddAccent("Afrikaans", "kaapse malay", "with a Kaapse Malay accent", "with a Kaapse Malay accent", 4,
            "The accent of a Malay ethnic from the Western Cape", "african");
        AddAccent("Afrikaans", "namibian", "with a Namibian accent", "with a Namibian accent", 4,
            "The accent of a speaker from Namibia", "african");
        AddAccent("Afrikaans", "oranjerivera", "with an Oranjerivera accent", "with an Oranjerivera accent", 2,
            "The accent of the Orange River region (Northern Cape)", "african");
        AddAccent("Afrikaans", "pretorian", "with a Pretorian accent", "with a Pretorian accent", 2,
            "The accent of Pretoria in the Eastern Cape", "african");
        AddAccent("Afrikaans", "swartland brei", "with the Swartland Brei accent", "with the Swartland Brei accent", 4,
            "The distinctive Swartland Brei accent", "african");
        AddAccent("Apache", "standard", "with a Standard accent", "with a Standard accent", 2,
            "The standard accent of a speaker of Apache", "standard");
        AddAccent("Arabic", "egyptian", "in the Egyptian dialect", "with a North-African accent", 3,
            "the accent of someone from Egypt", "egyptian");
        AddAccent("Arabic", "bahrainian", "with a Bahranian accent", "with a Gulf accent", 2,
            "the accent of someone from Bahranian", "gulf");
        AddAccent("Arabic", "emirati", "with an Emirati accent", "with a Gulf accent", 2,
            "the accent of someone from the Emirates", "gulf");
        AddAccent("Arabic", "kuwaiti", "with a Kuwaiti accent", "with a Gulf accent", 2,
            "the accent of someone from Kuwait", "gulf");
        AddAccent("Arabic", "qatari", "with a Qatari accent", "with a Gulf accent", 2,
            "the accent of someone from Qatar", "gulf");
        AddAccent("Arabic", "hassaniya", "in the Hassaniya dialect", "with a North-African accent", 3,
            "the accent of someone from Mauritania", "hassaniya");
        AddAccent("Arabic", "iraqi", "with an Iraqi accent", "with a Gulf accent", 2, "the accent of someone from Iraq",
            "iraqi");
        AddAccent("Arabic", "jordanian", "with a Jordanian accent", "with a Levantine accent", 2,
            "the accent of someone from Jordan", "levantine");
        AddAccent("Arabic", "lebanese", "with a Lebanese accent", "with a Levantine accent", 2,
            "the accent of someone from Lebanon", "levantine");
        AddAccent("Arabic", "palestinian", "with a Palestinian accent", "with a Levantine accent", 2,
            "the accent of someone from Palestine", "levantine");
        AddAccent("Arabic", "syrian", "with a Syrian accent", "with a Levantine accent", 2,
            "the accent of someone from Syria", "levantine");
        AddAccent("Arabic", "algerian", "with an Algerian accent", "with a North-African accent", 2,
            "the accent of someone from Algeria", "north african");
        AddAccent("Arabic", "libyan", "with a Libyan accent", "with a North-African accent", 2,
            "the accent of someone from Libya", "north african");
        AddAccent("Arabic", "moroccan", "with a Moroccan accent", "with a North-African accent", 2,
            "the accent of someone from Morocco", "north african");
        AddAccent("Arabic", "tunisian", "with a Tunisian accent", "with a North-African accent", 2,
            "the accent of someone from Tunisia", "north african");
        AddAccent("Arabic", "hejazi", "in the Hejazi dialect", "with a Saudi accent", 2,
            "the accent of someone from Western Saudi Arabia", "saudi");
        AddAccent("Arabic", "najdi", "in the Najdi dialect", "with a Saudi accent", 2,
            "the accent of someone from Eastern Saudi Arabia", "saudi");
        AddAccent("Arabic", "yemeni", "in the Yemeni dialect", "with a Saudi accent", 2,
            "the accent of someone from Yemen", "saudi");
        AddAccent("Belarusian", "middle", "in the Middle dialect", "in the Middle dialect", 2,
            "The dialect of Belarusian spoken in the central city of Minsk and the surrounding area", "middle");
        AddAccent("Belarusian", "northeastern", "in the Northeastern dialect", "in the Northeastern dialect", 2,
            "The dialect of Belarusian spoken in the Northeastern city of Vitebsk and the surrounding area",
            "northeastern");
        AddAccent("Belarusian", "southwestern", "in the Southwestern dialect", "in the Southwestern dialect", 2,
            "The dialect of Belarusian spoken in the Southwestern cities of Hrodna and Baranavichy and the surrounding area",
            "southwestern");
        AddAccent("Belarusian", "west palyesian", "in the West Palyesian dialect", "in the West Palyesian dialect", 3,
            "The dialect of Belarusian spoken in the Palyesia region of Belarusia, including the cities of Brest and Pinsk",
            "west palyesian");
        AddAccent("Bengali", "bakerganj", "in the Bakerganj dialect", "in an Eastern dialect", 3,
            "the Bakerganj dialect of Bengali", "eastern");
        AddAccent("Bengali", "bikrampur", "in the Bikrampur dialect", "in an Eastern dialect", 3,
            "the Bikrampur dialect of Bengali", "eastern");
        AddAccent("Bengali", "chittagong", "in the Chittagong dialect", "in an Eastern dialect", 3,
            "the Chittagong dialect of Bengali", "eastern");
        AddAccent("Bengali", "comilla", "in the Comilla dialect", "in an Eastern dialect", 3,
            "the Comilla dialect of Bengali", "eastern");
        AddAccent("Bengali", "faridpur", "in the Faridpur dialect", "in an Eastern dialect", 3,
            "the Faridpur dialect of Bengali", "eastern");
        AddAccent("Bengali", "feni", "in the Feni dialect", "in an Eastern dialect", 3, "The Feni dialect of Bengali",
            "eastern");
        AddAccent("Bengali", "hatia", "in the Hatia dialect", "in an Eastern dialect", 3,
            "the Hatia dialect of Bengali", "eastern");
        AddAccent("Bengali", "manikganj", "in the Manikganj dialect", "in an Eastern dialect", 3,
            "the Manikgani dialect of Bengali", "eastern");
        AddAccent("Bengali", "mymensingh", "in the Mymensingh dialect", "in an Eastern dialect", 3,
            "the Mymensingh dialect of Bengali", "eastern");
        AddAccent("Bengali", "rajshahi", "in the Rajshahi dialect", "in an Eastern dialect", 3,
            "the Rajshahi dialect of Bengali", "eastern");
        AddAccent("Bengali", "ramganj", "in the Ramganj dialect", "in an Eastern dialect", 3,
            "The Ramganj dialect of Bengali", "eastern");
        AddAccent("Bengali", "sandwip", "in the Sandwip dialect", "in an Eastern dialect", 3,
            "the Sandwip dialect of Bengali", "eastern");
        AddAccent("Bengali", "sylhet", "in the Sylhet dialect", "in an Eastern dialect", 3,
            "the Sylhet dialect of Bengali", "eastern");
        AddAccent("Bengali", "bogra", "in the Bogra dialect", "in a Northern dialect", 3,
            "The Bogra dialect of Bengali", "northern");
        AddAccent("Bengali", "dinajpur", "in the Dinajpur dialect", "in a Northern dialect", 3,
            "The Dinajpur dialect of Bengali", "northern");
        AddAccent("Bengali", "malda", "in the Malda dialect", "in a Northern dialect", 3,
            "The Malda dialect of Bengali", "northern");
        AddAccent("Bengali", "pabna", "in the Pabna dialect", "in a Northern dialect", 3,
            "The Pabna dialect of Bengali", "northern");
        AddAccent("Bengali", "rangpur", "in the Rangpur dialect", "in a Northern dialect", 3,
            "The Rangpur dialect of Bengali", "northern");
        AddAccent("Bengali", "chuadanga", "in the Chuadanga dialect", "in a Southern dialect", 3,
            "The Chuadanga dialect of Bengali", "southern");
        AddAccent("Bengali", "jessore", "in the Jessore dialect", "in a Southern dialect", 3,
            "The Jessore dialect of Bengali", "southern");
        AddAccent("Bengali", "khulna", "in the Khulna dialect", "in a Southern dialect", 3,
            "The Khulna dialect of Bengali", "southern");
        AddAccent("Bengali", "medinipur", "in the Medinipur dialect", "in a Southern dialect", 3,
            "The Medinipur dialect of Bengali", "southern");
        AddAccent("Bengali", "nadia", "in the Nadia dialect", "in a West Central dialect", 3,
            "the Nadia dialect of Bengali", "west central");
        AddAccent("Cherokee", "standard", "with a Standard accent", "with a Standard accent", 2,
            "The standard accent of a speaker of Cherokee", "standard");
        AddAccent("Danish", "insular", "in an Insular dialect", "in an Insular dialect", 3,
            "Insular dialects of Danish are spoken in the Danish islands of Zealand, Funen, Lolland, Falster and Møn.",
            "insular");
        AddAccent("Danish", "jutlandic", "in the Jutlandic dialect", "in the Jutlandic dialect", 3,
            "Jutlandic dialects of Danish are spoken in Jutland.", "jutlandic");
        AddAccent("Danish", "rigsdansk", "in the Rigsdansk dialect", "in the Rigsdansk dialect", 1,
            "The Rigsdansk is the standard spoken version of Danish, very similar to that spoken in the city of Copenhagen.",
            "standard");
        AddAccent("Dutch", "geldric", "with a Geldric accent", "with a Dutch accent", 2, "the Geldric accent", "dutch");
        AddAccent("Dutch", "hollandic", "with a Hollandic accent", "with a Dutch accent", 1, "the Hollandic accent",
            "dutch");
        AddAccent("Dutch", "zealandic", "with a Zealandic accent", "with a Dutch accent", 2, "the Zealandic accent",
            "dutch");
        AddAccent("Dutch", "brabantic", "with a Brabantic accent", "with a Flemish accent", 2, "the Brabantic accent",
            "flemish");
        AddAccent("Dutch", "east flemish", "with an East Flemish accent", "with a Flemish accent", 2,
            "the East Flemish accent", "flemish");
        AddAccent("Dutch", "limburgish", "with a Limburgish accent", "with a Flemish accent", 2,
            "the Limburgish accent", "flemish");
        AddAccent("Dutch", "west flemish", "with a West Flemish accent", "with a Flemish accent", 2,
            "the West Flemish accent", "flemish");
        AddAccent("English", "african american", "with an African-American accent", "with an American accent", 4,
            "the accent and/or dialect of speakers of Afro American Vernacular English", "american");
        AddAccent("English", "baltimore", "with a Baltimore accent", "with an American accent", 2,
            "the Baltimore accent", "american");
        AddAccent("English", "eastern new england", "with an Eastern New England accent", "with an American accent", 2,
            "the Eastern New England accent", "american");
        AddAccent("English", "general american", "with a General American accent", "with an American accent", 1,
            "the accent of newsreaders and actors in America", "american");
        AddAccent("English", "hawaiian", "with a Hawaiian accent", "with an American accent", 2,
            "the Hawaiian accent associated with Polynesian speakers", "american");
        AddAccent("English", "inland north", "with an Inland North accent", "with an American accent", 1,
            "the Inland North accent", "american");
        AddAccent("English", "latino", "with a Latino accent", "with an American accent", 4,
            "the accent of a typical Latino speaking American English", "american");
        AddAccent("English", "new yorker", "with a New Yorker accent", "with an American accent", 2,
            "the New Yorker accent", "american");
        AddAccent("English", "north central", "with a North Central accent", "with an American accent", 2,
            "the North Central accent", "american");
        AddAccent("English", "north midland", "with a North Midland accent", "with an American accent", 2,
            "the North Midland accent", "american");
        AddAccent("English", "pacific northwest", "with a Pacific Northwest accent", "with an American accent", 2,
            "the Pacific Northwest accent", "american");
        AddAccent("English", "pennsylvanian", "with a Pennsylvanian accent", "with an American accent", 2,
            "the Pennsylvanian accent", "american");
        AddAccent("English", "reservation", "with a Reservation accent", "with a Native American accent", 3,
            "The stereotypical accent of English spoken by native americans from reservations or those wanting to sound like them",
            "american");
        AddAccent("English", "saint louis", "with a Saint Louis accent", "with an American accent", 2,
            "the Saint Louis accent", "american");
        AddAccent("English", "south midland", "with a South Midland accent", "with an American accent", 2,
            "the South Midland accent", "american");
        AddAccent("English", "texan", "with a Texan accent", "with an American accent", 3, "the Texan Accent",
            "american");
        AddAccent("English", "west coast", "with a West Coast accent", "with an American accent", 2,
            "the West Coast American accent", "american");
        AddAccent("English", "western new england", "with a Western New England accent", "with an American accent", 2,
            "the Western New England accent", "american");
        AddAccent("English", "western pennsylvanian", "with a Western Pennsylvanian accent", "with an American accent",
            2, "the Western Pennsylvanian accent", "american");
        AddAccent("English", "cajun", "with a Cajun accent", "with an American South accent", 3, "the Cajun accent",
            "american south");
        AddAccent("English", "charleston", "with a Charleston accent", "with an American South accent", 2,
            "the Charleston accent", "american south");
        AddAccent("English", "georgian", "with a Georgian drawl", "with an American South accent", 3,
            "the Georgian Accent", "american south");
        AddAccent("English", "kentucky", "with a Kentucky accent", "with an American South accent", 2,
            "the Kentucky accent", "american south");
        AddAccent("English", "new orleans", "with a New Orleans accent", "with an American South accent", 3,
            "the New Orleans accent", "american south");
        AddAccent("English", "chinese", "with a Chinese accent", "with an Asian accent", 4,
            "the Chinese pronunciation of English", "asian");
        AddAccent("English", "japanese", "with a Japanese accent", "with an Asian accent", 4,
            "the Japanese pronunciation of English", "asian");
        AddAccent("English", "korean", "with a Korean accent", "with an Asian accent", 4,
            "the Korean pronunciation of English", "asian");
        AddAccent("English", "southeast asian", "with a Southeast Asian accent", "with an Asian accent", 4,
            "the Southeast Asian pronunciation of English", "asian");
        AddAccent("English", "broad australian", "with a Broad Australian accent", "with an Australian accent", 4,
            "the Broad Australian accent", "australian");
        AddAccent("English", "general australian", "with a General Australian accent", "with an Australian accent", 3,
            "the General Australian Accent", "australian");
        AddAccent("English", "refined australian", "with a Refined Australian accent", "with an Australian accent", 2,
            "the Refined Australian accent", "australian");
        AddAccent("English", "cockney", "with a Cockney accent", "with an English accent", 4, "the Cockney Accent",
            "british");
        AddAccent("English", "received", "with a Received Pronunciation", "with an English accent", 1,
            "Received Pronunciation", "british");
        AddAccent("English", "welsh", "with a Welsh accent", "with an English accent", 4, "the Welsh Accent",
            "british");
        AddAccent("English", "eastern european", "with an Eastern European accent", "with an Eastern European accent",
            4, "the Eastern European pronunciation of English", "eastern european");
        AddAccent("English", "cardiff", "with a Cardiff accent", "with an English accent", 3, "the Cardiff accent",
            "english");
        AddAccent("English", "cheshire", "with a Chesire accent", "with an English accent", 3, "the Cheshire accent",
            "english");
        AddAccent("English", "cornish", "with a Cornish accent", "with an English accent", 3, "the Cornish accent",
            "english");
        AddAccent("English", "east anglian", "with an East Anglian accent", "with an English accent", 3,
            "the East Anglian accent", "english");
        AddAccent("English", "east midlands", "with an East Midlands accent", "with an English accent", 3,
            "the East Midlands accent", "english");
        AddAccent("English", "home counties", "with a Home Counties accent", "with an English accent", 3,
            "the Home Counties accent", "english");
        AddAccent("English", "lancashire", "with a Lancashire accent", "with an English accent", 3,
            "the Lancashire accent", "english");
        AddAccent("English", "north london", "with a North London accent", "with an English accent", 2,
            "the North London accent", "english");
        AddAccent("English", "scouse", "with a Scouse accent", "with an English accent", 3, "the Scouse accent",
            "english");
        AddAccent("English", "south london", "with a South London accent", "with an English accent", 2,
            "the South London accent", "english");
        AddAccent("English", "west country", "with a West Country accent", "with an English accent", 3,
            "the West Country accent", "english");
        AddAccent("English", "west midlands", "with a West Midlands accent", "with an English accent", 3,
            "the West Midlands accent", "english");
        AddAccent("English", "yorkshire", "with a Yorkshire accent", "with an English accent", 3,
            "the Yorkshire accent", "english");
        AddAccent("English", "french", "with a French accent", "with a European accent", 4,
            "the French pronunciation of English", "european");
        AddAccent("English", "german", "with a German accent", "with a European accent", 4,
            "the German pronunciation of English", "european");
        AddAccent("English", "italian", "with an Italian accent", "with a European accent", 4,
            "the Italian pronunciation of English", "european");
        AddAccent("English", "spanish", "with a Spanish accent", "with a European accent", 4,
            "the continental Spanish pronunciation of English", "european");
        AddAccent("English", "babu", "with a Babu accent", "with an Indian accent", 4,
            "the English of the Bengali and Nepal region", "indian");
        AddAccent("English", "indian", "with a standard Indian accent", "with an Indian accent", 4,
            "the standard English of the Indian subcontinent", "indian");
        AddAccent("English", "southern indian", "with a Southern Indian accent", "with an Indian accent", 4,
            "the English of the Southern part of the Indian Subcontinent", "indian");
        AddAccent("English", "connacht", "with a Connacht accent", "with an Irish accent", 3, "the Connacht accent",
            "irish");
        AddAccent("English", "corkonian", "with a Corkonian accent", "with an Irish accent", 2, "the Corkonian accent",
            "irish");
        AddAccent("English", "dublin", "with a Dublin accent", "with an Irish accent", 2, "the Dublin accent", "irish");
        AddAccent("English", "leinster", "with a Leinster accent", "with an Irish accent", 3, "the Leinster accent",
            "irish");
        AddAccent("English", "mid ulster", "with a Mid Ulster accent", "with an Irish accent", 3,
            "the Mid Ulster accent", "irish");
        AddAccent("English", "munster", "with a Munster accent", "with an Irish accent", 3, "the Munster accent",
            "irish");
        AddAccent("English", "ulster scots", "with an Ulster Scots accent", "with an Irish accent", 3,
            "the Ulster Scots accent", "irish");
        AddAccent("English", "middle-eastern", "with a Middle-Eastern accent", "with a Middle-Eastern accent", 4,
            "the Middle-Eastern pronunciation of English, such as a native Arabic speaker would learn",
            "middle-eastern");
        AddAccent("English", "border scots", "with a Border Scots accent", "with a Scottish accent", 4,
            "the Border Scots accent", "scottish");
        AddAccent("English", "lowland scots", "with a Lowland Scots accent", "with a Scottish accent", 4,
            "the Lowland Scots accent", "scottish");
        AddAccent("English", "northern scots", "with a Northern Scots accent", "with a Scottish accent", 4,
            "the Northern Scots accent", "scottish");
        AddAccent("Farsi", "afghani", "with an Afghani accent", "with an Eastern accent", 2,
            "The Aghani accent of Farsi", "afghani");
        AddAccent("Farsi", "arabic", "with an Arabic accent", "with a Middle-Eastern accent", 3,
            "The accent of a native Arabic speaker in Farsi", "arabic");
        AddAccent("Farsi", "armenian", "with an Armenian accent", "with an Armenian accent", 3,
            "The Armenian accent of Farsi", "armenian");
        AddAccent("Farsi", "esfahani", "with an Esfahani accent", "with an Eastern accent", 2,
            "The Esfahani accent of Farsi", "iranian");
        AddAccent("Farsi", "eva khahar", "with the Eva Khahar accent", "with a Tehranian accent", 2,
            "The Eva Khahar accent of Farsi", "iranian");
        AddAccent("Farsi", "farangi", "with a Farangi accent", "with a Southern accent", 2,
            "The Farangi accent of Farsi", "iranian");
        AddAccent("Farsi", "gilaki", "with a Qilaki accent", "with a Northern accent", 2, "The Qilaki accent of Farsi",
            "iranian");
        AddAccent("Farsi", "khorosani", "with a Khorosani accent", "with an Eastern accent", 2,
            "The Khorosani accent of Farsi", "iranian");
        AddAccent("Farsi", "laati", "with a Laati accent", "with a Tehranian accent", 2, "The Laati accent of Farsi",
            "iranian");
        AddAccent("Farsi", "mashadi", "with a Mashadi accent", "with an Eastern accent", 2,
            "The Mashadi accent of Farsi", "iranian");
        AddAccent("Farsi", "qazvini", "with a Qazvini accent", "with a Western accent", 2,
            "The Qazvini accent of Farsi", "iranian");
        AddAccent("Farsi", "shirazi", "with a Shirazi accent", "with a Southern accent", 2,
            "The Shirazi accent of Farsi", "iranian");
        AddAccent("Farsi", "tehrani", "with a Tehrani accent", "with a Tehranian accent", 2,
            "The Tehrani accent of Farsi", "iranian");
        AddAccent("Farsi", "yazdi", "with a Vazdi accent", "with an Eastern accent", 2, "The Vazdi accent of Farsi",
            "iranian");
        AddAccent("Farsi", "jidi", "with a Jewish accent", "with a Jewish accent", 3,
            "The Jidi (or Jewish) accent of Farsi", "jewish");
        AddAccent("Farsi", "tabrizi", "with a Tabrizi accent", "with a Turkish accent", 3,
            "The Tabrizi accent of Farsi", "turkish");
        AddAccent("Farsi", "turkish", "with a Turkish accent", "with a Turkish accent", 3,
            "The accent of a native Turkish speaker in Farsi", "turkish");
        AddAccent("Finnish", "ingrian", "in the Ingrian dialect", "in an eastern dialect", 3,
            "The Ingrian dialect is an eastern dialect of Finnish closely related to Savonian.", "eastern");
        AddAccent("Finnish", "karelian", "in the Karelian dialect", "in an eastern dialect", 5,
            "Karelian is a dialect (some might say a separate language) of Finnish spoken in Karelia, an area in the far east of Finland or the far west of Russia (depending on time and politics).",
            "eastern");
        AddAccent("Finnish", "savonian", "in the Savonian dialect", "in an eastern dialect", 3,
            "The Savonian dialect of Finnish, spoken around the south-eastern city of Savo. Because of its relative remoteness from the Swedish and Norwegian borders, it has less influence on the language from these forms.",
            "eastern");
        AddAccent("Finnish", "yleiskieli", "in the standard Yleiskieli form", "in the standard Yleiskieli form", 1,
            "The Yleiskieli, meaning 'standard language', is the standardised spoken form of Finnish. It is often encountered in news and media, as well as acting as a formal register for speach.",
            "standard");
        AddAccent("Finnish", "far northern", "in the Far Northern dialect", "in a western dialect", 2,
            "The Far Northern dialect is the dialect of Finns living in regions of Lapland.", "western");
        AddAccent("Finnish", "kven", "in the Kven dialect", "in a western dialect", 5,
            "The Kven dialect (some might say a separate language) of Finnish is spoken in Finnmark and Troms in Norway, and is the result of Finnish emigration to the area in the 18th centuries.",
            "western");
        AddAccent("Finnish", "meänkieli", "in the Meänkieli dialect", "in a western dialect", 3,
            "The Meänkieli dialect is a variant of the far northern dialect that became distinct when its speakers became Swedish subjects following the Russian annexation of Finland in the 19th century.",
            "western");
        AddAccent("Finnish", "north ostrobothian", "in the North Ostrobothian dialect", "in a western dialect", 2,
            "The North Ostrobothian dialect is spoken in the central and northern parts of Ostrobothia.", "western");
        AddAccent("Finnish", "south ostrobothian", "in the South Ostrobothian dialect", "in a western dialect", 2,
            "The South Ostrobothian dialect is spoken in the southern parts of Ostrobothia.", "western");
        AddAccent("Finnish", "southwestern", "in the Southwestern dialect", "in a western dialect", 2,
            "The South-Western dialect is a variety of western Finnish spoken in Southwest Finland and Satakunta.",
            "western");
        AddAccent("Finnish", "tavastian", "in the Tavastian dialect", "in a western dialect", 1,
            "The Tavastian dialect of Finnish is spoken in the Tavastia region of Finland. It closely resembles the standard form of the language.",
            "western");
        AddAccent("French", "cambodian", "with a Cambodian accent", "with an Asian accent", 4, "the Cambodian accent",
            "asian french");
        AddAccent("French", "vietnamese", "with a Vietnamese accent", "with an Asian accent", 4,
            "the Vietnamese accent", "asian french");
        AddAccent("French", "aostan", "with an Aostan accent", "with a French accent", 2, "the Aostan accent",
            "french");
        AddAccent("French", "belgian", "with a Belgian accent", "with a French accent", 2, "the Belgian accent",
            "french");
        AddAccent("French", "meridional", "with a Meridional accent", "with a French accent", 2,
            "the Meridional accent", "french");
        AddAccent("French", "metropolitan", "with a Metropolitan accent", "with a French accent", 1,
            "the Metropolitan accent", "french");
        AddAccent("French", "swiss", "with a Swiss accent", "with a French accent", 2, "the Swiss accent", "french");
        AddAccent("French", "Acadian", "with an Acadian accent", "with a Canadian accent", 3, "the Acadian accent",
            "french canadian");
        AddAccent("French", "chiac", "with a Chiac accent", "with a Canadian accent", 3, "the Chiac accent",
            "french canadian");
        AddAccent("French", "newfoundland", "with a Newfoundland accent", "with a Canadian accent", 2,
            "the Newfoundland accent", "french canadian");
        AddAccent("French", "quebec", "with a Québécois accent", "with a Canadian accent", 2, "the Québécois accent",
            "french canadian");
        AddAccent("French", "new england", "with a New England accent", "with an American accent", 3,
            "the New England accent", "united stated french");
        AddAccent("French", "louisiana", "with a Lousiana accent", "with an American accent", 3, "the Louisiana accent",
            "united states french");
        AddAccent("German", "austrian", "with an Austrian accent", "with an Austrian accent", 2, "the Austrian Accent",
            "austrian");
        AddAccent("German", "bavarian", "with a Bavarian accent", "with a German accent", 2, "the Bavarian accent",
            "german");
        AddAccent("German", "berlinerisch", "with a Berlinerisch accent", "with a German accent", 1,
            "the Berlin Accent", "german");
        AddAccent("German", "franconian", "with a Franconian accent", "with a German accent", 3,
            "the Franconian accent", "german");
        AddAccent("German", "hannoverian", "with a Hannoverian accent", "with a German accent", 1,
            "the Hannoverian accent", "german");
        AddAccent("German", "plattdeutsch", "with a Plattdüütsch accent", "with a German accent", 4,
            "the Plattdüütsch accent", "german");
        AddAccent("German", "saxon", "with a Saxon accent", "with a German accent", 4, "the Saxon accent", "german");
        AddAccent("German", "swabian", "with a Swabian accent", "with a German accent", 3, "the Swabian accent",
            "german");
        AddAccent("German", "western indian", "with a Western Indian accent", "with an Indian accent", 4,
            "the English of the Western part of the Indian Subcontinent", "indian");
        AddAccent("Greek", "cappadocian", "in the Cappadocian dialect", "in an Asian dialect", 3,
            "the form of Greek spoken by Greek-speaking peoples in the Cappadocia region of Asia Minor, particularly Turkey",
            "asian");
        AddAccent("Greek", "pontic", "in the Pontic dialect", "in an Asian dialect", 3,
            "the form of greek spoken by Greek-speaking peoples in Asia Minor, particularly around the Black Sea and into Ukraine and Russia",
            "asian");
        AddAccent("Greek", "athenian", "in an Athenian Greek idiom", "in a Core Dialect", 1,
            "the standard modern Greek dialect spoken around Athens, considered to be essentially \"neutral\" greek",
            "core");
        AddAccent("Greek", "northern", "in a Northern Greek idiom", "in a Core Dialect", 1,
            "the standard modern Greek dialect with a northern idiom", "core");
        AddAccent("Greek", "southern", "in a Southern Greek idiom", "in a Core Dialect", 1,
            "the standard modern Greek dialect with a southern idiom", "core");
        AddAccent("Greek", "italiot", "in the Italiot dialect", "in an Italian dialect", 3,
            "the form of Greek spoken by Greek-speaking peoples in Italy", "italian");
        AddAccent("Greek", "tsakonian", "in the Tsakonian dialect", "in a Strange dialect", 6,
            "Tsakonian is a highly divergent variety, sometimes classified as a separate language because of not being intelligible to speakers of standard Greek. It is spoken in a small mountainous area slightly inland from the east coast of the Peloponnese peninsula. It is unique among all other modern varietiesin that it is thought to derive not from the ancient Attic–Ionian Koiné, but from Doric or from a mixed form of a late, ancient Laconian variety of the Koiné influenced by Doric. It used to be spoken earlier in a wider area of the Peloponnese, including Laconia, the historical home of the Doric Spartans.",
            "strange");
        AddAccent("Hausa", "bausanchi", "in the Bausanchi dialect", "in an Eastern dialect", 2,
            "The dialect of Hausa from Bauchi", "eastern");
        AddAccent("Hausa", "dauranchi", "in the Dauranchi dialect", "in an Eastern dialect", 1,
            "The dialect of Hausa from Daura. Also one of the two standard dialects as far as government and media are concerned.",
            "eastern");
        AddAccent("Hausa", "gudduranci", "in the Gudduranci dialect", "in an Eastern dialect", 2,
            "The dialect of Hausa from Katagum Misau and Borno", "eastern");
        AddAccent("Hausa", "hadejanci", "in the Hadejanci dialect", "in an Eastern dialect", 2,
            "The dialect of Hausa from Hadejiya", "eastern");
        AddAccent("Hausa", "kananci", "in the Kananci dialect", "in an Eastern dialect", 1,
            "The dialect of Hausa from Kano. Also one of the two standard dialects as far as government and media are concerned.",
            "eastern");
        AddAccent("Hausa", "gaananci", "in the Gaananci dialect", "in a Ghanaian dialect", 4,
            "The dialect of Hausa spoken in Ghana, Togo, and the Ivory Coast", "ghanaian");
        AddAccent("Hausa", "arawci", "in the Arawci dialect", "in a Northern dialect", 2, "A northern Hausa dialect",
            "northern");
        AddAccent("Hausa", "arewa", "in the Arewa dialect", "in a Northern dialect", 2, "A northern Hausa dialect",
            "northern");
        AddAccent("Hausa", "zazzaganci", "in the Zazzaganci dialect", "in a Southern dialect", 2,
            "A southern Hausa dialect spoken in Zazzau", "southern");
        AddAccent("Hausa", "west african", "in the West African dialect", "in a West African dialect", 4,
            "This dialect represents the accent of west africans who speak Hausa as a second language, often picked up through movies and music.",
            "west african");
        AddAccent("Hausa", "arewanci", "in the Arewanci dialect", "in a Western dialect", 2,
            "The dialect of Hausa from Gobir, Adar, Kebbi and Zamfara", "western");
        AddAccent("Hausa", "katsinanci", "in the Katsinanci dialect", "in a Western dialect", 2,
            "The dialect of Hausa from Katsina", "western");
        AddAccent("Hausa", "kurhwayanci", "in the Kurhwayanci dialect", "in a Western dialect", 2,
            "The dialect of Hausa from Kurfey in Niger", "western");
        AddAccent("Hausa", "sakkwatanci", "in the Sakkwatanci dialect", "in a Western dialect", 2,
            "The dialect of Hausa from Sokoto", "western");
        AddAccent("Hebrew", "mizrahi", "in the Mizrahi dialect", "with an Arabic accent", 2,
            "a dialect of Hebrew used by Mizrahi jews (jews living in Arabic countries)", "arabic");
        AddAccent("Hebrew", "yemenite", "in the Yemenite dialect", "with an Arabic accent", 2,
            "a dialect of Hebrew used by jews in Yemen", "arabic");
        AddAccent("Hebrew", "israeli", "in the Israeli dialect", "with a Levantine accent", 1,
            "the accent of a standard speaker of Hebrew in Israel", "levantine");
        AddAccent("Hebrew", "ashekenazi", "in the Ashekenazi dialect", "in a Liturgical dialect", 3,
            "a dialect of Hebrew used by Ashekenazi jews in liturgical reading", "liturgical");
        AddAccent("Hebrew", "samaritan", "in the Samaritan dialect", "in a Liturgical dialect", 3,
            "a dialect of Hebrew used by Samaritans in liturgical reading", "liturgical");
        AddAccent("Hebrew", "sephardi", "in the Sephardi dialect", "in a Liturgical dialect", 3,
            "a dialect of Hebrew used by Sephardi jews in liturgical reading", "liturgical");
        AddAccent("Hebrew", "tiberian", "in the Tiberian vocalization", "in a Liturgical dialect", 3,
            "a dialect of Hebrew used in liturgical reading", "liturgical");
        AddAccent("Hindi", "awadhi", "in the Awadhi dialect", "with an Eastern accent", 2,
            "the Awadhi dialect, spoken in north and north-central Uttar Pradesh", "eastern");
        AddAccent("Hindi", "bagheli", "in the Badheli dialect", "with an Eastern accent", 2,
            "The Bagheli dialect, spoken in north-central Madhya Pradesh and south-eastern Uttar Pradesh", "eastern");
        AddAccent("Hindi", "bihari", "in the Bihari dialect", "with an Eastern accent", 2,
            "The Bihari dialect, spoken in Bihar and Jharkhand", "eastern");
        AddAccent("Hindi", "carribean", "in the Carribean dialect", "with an Eastern accent", 4,
            "The Carribean dialect is spoken by Indo-Carribeans, closely related to other eastern Hindi dialects",
            "eastern");
        AddAccent("Hindi", "chhattisgarhi", "in the Chhattisgarhi dialect", "with an Eastern accent", 2,
            "The Chhattisgarhi dialect, spoken in south-eastern Madhya Pradesh and north and central Chhattisgarh",
            "eastern");
        AddAccent("Hindi", "fijian", "in the Fijian dialect", "with an Eastern accent", 4,
            "The Fijian dialect, closely related to the Awadhi dialect, is spoken by ethnic Indians in Fiji",
            "eastern");
        AddAccent("Hindi", "hinglish", "in the Hinglish dialect", "in a Pretentious accent", 7,
            "Hinglish is a mixture of Hindustani and English spoken by educated urban populations in some Indian cities. It often code switches between Hindi and English in the middle of a sentence. To other Indians, this dialect sounds pretentious and aloof",
            "hinglish");
        AddAccent("Hindi", "dakhini", "in the Dakhini dialect", "with a Pakistani accent", 6,
            "The Dakhini dialect, a subset of Urdu spoken largely in the Hyperabad province", "pakistani");
        AddAccent("Hindi", "urdu", "in the Urdu dialect", "with a Pakistani accent", 6,
            "Technically a language of its own, Urdu is highly mutually intelligable with standard Hindi and mixes Hindi, Arabic and Persian vocabulary. Spoken in Pakistan.",
            "pakistani");
        AddAccent("Hindi", "bombay bat", "in the Bombay Bat", "in the Bombay Bat", 5,
            "Technically a language of its own", "pidgin");
        AddAccent("Hindi", "braj bhasha", "in the Braj Bhasha dialect", "with a Western accent", 3,
            "The Braj Bhasha dialect of Hindi, spoken in western Uttar Pradesh", "western");
        AddAccent("Hindi", "bundeli", "in the Bundeli dialect", "with a Western accent", 2,
            "the Bundeli dialect, spoken in south-western Uttar Pradesh and parts of Madhya Pradesh", "western");
        AddAccent("Hindi", "haryanvi", "in the Haryanvi dialect", "with a Western accent", 2,
            "The Haryanvi dialect, spoken in Haryana and parts of Delhi", "western");
        AddAccent("Hindi", "hindustani", "with a Hindustani accent", "with a Western accent", 1,
            "The standard dialect of spoken Hindi, based on the Khariboli dialect of Delhi", "western");
        AddAccent("Hindi", "kannauji", "in the Kannauji dialect", "with a Western accent", 2,
            "the Kannauji dialect, spoken in west-central Uttar Pradesh", "western");
        AddAccent("Irish Gaelic", "connacht", "with a Connacht accent", "with an Irish accent", 2,
            "The accent of a speaker from Connacht", "irish");
        AddAccent("Irish Gaelic", "munster", "with a Munster accent", "with an Irish accent", 2,
            "The accent of a speaker from Munster", "irish");
        AddAccent("Irish Gaelic", "ulster", "with an Ulster accent", "with an Irish accent", 2,
            "The accent of a speaker from Ulster", "irish");
        AddAccent("Italian", "libyan", "with a Libyan accent", "with an African accent", 4, "the Libyan accent",
            "african");
        AddAccent("Italian", "maltese", "with a Maltese accent", "with an African accent", 4, "the Maltese accent",
            "african");
        AddAccent("Italian", "somali", "with a Somali accent", "with an African accent", 4, "the Somali accent",
            "african");
        AddAccent("Italian", "corsican", "with a Corsican accent", "with a Regional accent", 4, "the Corsican accent",
            "corsican");
        AddAccent("Italian", "genoese", "with a Genoese accent", "with a Regional accent", 3, "the Genoese accent",
            "genoese");
        AddAccent("Italian", "roman", "with a Roman accent", "with a Central Italian accent", 1, "the Roman accent",
            "italian");
        AddAccent("Italian", "milanese", "with a Milanese accent", "with a Regional accent", 3, "the Milanese accent",
            "milanese");
        AddAccent("Italian", "neapolitan", "with a Neapolitan accent", "with a Southern accent", 3,
            "the Neapolitan accent", "southern");
        AddAccent("Italian", "sicilian", "with a Sicilian accent", "with a Southern accent", 3, "the Sicilian accent",
            "southern");
        AddAccent("Italian", "swiss", "with a Swiss accent", "with a European accent", 4, "the Swiss accent", "swiss");
        AddAccent("Italian", "florentine", "with a Florentine accent", "with a Regional accent", 3,
            "the Florentine accent", "tuscan");
        AddAccent("Italian", "tuscan", "with a Tuscan accent", "with a Regional accent", 2, "the Tuscan accent",
            "tuscan");
        AddAccent("Italian", "venetian", "with a Venetian accent", "with a Regional accent", 3, "the Venetian accent",
            "venetian");
        AddAccent("Japanese", "tohoku", "in the Tohoku dialect", "in an eastern dialect", 2,
            "The accent and dialect of speakers of Japanese from the northeast of Honshū", "eastern");
        AddAccent("Japanese", "hachijo", "in the Hachijo dialect", "in an outer dialect", 4,
            "The accent and dialect of speakers of Japanese particularly from Hachijo", "outside");
        AddAccent("Japanese", "izumo", "in the Izumo dialect", "in an outer dialect", 3,
            "The accent and dialect of speakers of Japanese particularly from Izumo", "outside");
        AddAccent("Japanese", "kyushu", "in the Kyushu dialect", "in an outer dialect", 3,
            "The accent and dialect of speakers of Japanese particularly from Kyushu", "outside");
        AddAccent("Japanese", "okinawan", "in the Okinawan dialect", "in an outer dialect", 5,
            "The accent and dialect of speakers of Japanese from Okinawa", "outside");
        AddAccent("Japanese", "chugoku", "in the Chugoku dialect", "in a western dialect", 2,
            "The accent and dialect of speakers of Japanese from Chugoku and Northwestern Kansai regions", "western");
        AddAccent("Japanese", "kansai", "in the Kansai dialect", "in a western dialect", 2,
            "The accent and dialect of speakers of Japanese in areas around Osaka", "western");
        AddAccent("Japanese", "shikoku", "in the Shikoku dialect", "in a western dialect", 3,
            "The accent and dialect of speakers of Japanese in Shikoku", "western");
        AddAccent("Korean", "chungcheong", "in the Chungcheong dialect", "in a central dialect", 2,
            "The accent and dialect of speakers of Korean from Chungcheong province", "central");
        AddAccent("Korean", "yeongseo", "in the Yeongseo dialect", "in a central dialect", 2,
            "The accent and dialect of speakers of Korean from Yeongseo, Gangwon and Kangwon", "central");
        AddAccent("Korean", "jeju", "in the Jeju dialect", "in a distinctive dialect", 5,
            "The accent and dialect of speakers of Korean from Jejudo, distinct enough to be considered a separate language by some. Very difficult for others to understand.",
            "jeju");
        AddAccent("Korean", "hamgyong", "in the Hamgyong dialect", "in a northeastern dialect", 2,
            "The accent and dialect of speakers of Korean from northeastern korea", "northeastern");
        AddAccent("Korean", "hwanghae", "in the Hwanghae dialect", "in a northwestern dialect", 2,
            "The accent and dialect of speakers of Korean from Hwanghae", "northwestern");
        AddAccent("Korean", "pyongan", "in the Pyongan dialect", "in a northwestern dialect", 2,
            "The accent and dialect of speakers of Korean from Pyongan, Chagang and Liaoning, also forming the basis for standard Northe Korean.",
            "northwestern");
        AddAccent("Korean", "yukchin", "in the Yukchin dialect", "in a northwestern dialect", 2,
            "The accent and dialect of speakers of Korean from Yukchin region of northeastern hamgyong",
            "northwestern");
        AddAccent("Korean", "gyeongsang", "in the Gyeongsang dialect", "in a southeastern dialect", 2,
            "The accent and dialect of speakers of Korean from Gyeongsang province in South Korea.", "southeastern");
        AddAccent("Korean", "jeolla", "in the Jeolla dialect", "in a southwestern dialect", 2,
            "The accent and dialect of speakers of Korean from Jeolla province in South Korea", "southwestern");
        AddAccent("Korean", "yeongdong", "in the Yeongdong dialect", "in a Yeongdong dialect", 4,
            "The accent and dialect of speakers of Korean from Yeongdong, Gangwon and Kangwon. Fairly distinct from central Korean dialects.",
            "yeongdong");
        AddAccent("Malay", "brunei", "in the Brunei dialect", "in the Brunei dialect", 5,
            "The variety of Malay spoken in the Sultanate of Brunei", "brunei");
        AddAccent("Malay", "indonesian", "in the Indonesian dialect", "in the Indonesian dialect", 5,
            "Bahasa Indonesia, the dialect of the Malay language spoken in the Indonesian archipeligo", "indonesian");
        AddAccent("Malay", "malaysian", "in the Malaysian dialect", "in the Malaysian dialect", 5,
            "Bahasa Melayu, the dialect of Malay language spoken in Malaysia", "malaysian");
        AddAccent("Malay", "singaporean", "in the Singaporean dialect", "in the Singaporean dialect", 5,
            "The variety of Malay spoken in the island nation of Singapore", "singaporean");
        AddAccent("Mandarin", "northeastern", "in the northeastern dialect", "with a northeastern dialect", 2,
            "The accent and dialect of speakers of Mandarin Chinese from the northeast, particularly Manchuria.",
            "northeastern");
        AddAccent("Mandarin", "central plains", "in the Central Plains dialect", "with a regional dialect", 5,
            "The accent and dialect of speakers of Mandarin Chinese from Henan, Shaanxi, Xinjiang and Dungan.",
            "plains");
        AddAccent("Mandarin", "lanyin", "in the Lanyin dialect", "with a regional dialect", 4,
            "The accent and dialect of speakers of Mandarin Chinese from Gansu, Ningxia and Xinjiang", "plains");
        AddAccent("Mandarin", "jiaoliao", "in the Jiaoliao dialect", "with a regional dialect", 3,
            "The accent and dialect of speakers of Mandarin Chinese from the Shandong and Liaodong Peninsulas",
            "regional");
        AddAccent("Mandarin", "jilu", "in the Jilu dialect", "with a regional dialect", 4,
            "The accent and dialect of speakers of Mandarin Chinese from Hebei, Shandong and Tianjin", "regional");
        AddAccent("Mandarin", "southwestern", "in the Southwestern dialect", "with a regional dialect", 4,
            "The accent and dialect of speakers of Mandarin Chinese from Hubei, Sichuan, Guizhou, Yunnan, Hunan, Guangxi and Shaanxi",
            "southwestern");
        AddAccent("Mandarin", "jianghuai", "in the Jianghuai dialect", "with a regional dialect", 5,
            "The accent and dialect of speakers of Mandarin Chinese from Jiangsu and Anhui.", "yangtze");
        AddAccent("Navajo", "standard", "with a Standard accent", "with a Standard accent", 2,
            "The standard accent of a speaker of Navajo", "standard");
        AddAccent("Norwegian", "bokmål", "in the Bokmål dialect", "in the Bokmål dialect", 1,
            "The Bokmål dialect is usually only seen in writing, and uses vocabulary and spelling very close to Danish. This is the most common written form of the language. It is not impossible to speak in a Bokmål fashion, but it would sound strange even to those accustomed to reading it; it would also make the speaker sound a little Danish. Interestingly, the Bokmål dialect merges the masculine and feminine genders into a single common gender, as in Danish.",
            "literary");
        AddAccent("Norwegian", "høgnorsk", "in the Høgnorsk dialect", "in the Høgnorsk dialect", 2,
            "The Høgnorsk dialect is usually only seen in writing, and uses a vocabulary and spelling that is very close to the original pre-Danish conquest Norweigan. It is very rarely spoken and preserves some spellings and features that are not common in modern Norwegian spoken dialects.",
            "literary");
        AddAccent("Norwegian", "nynorsk", "in the Nynorsk dialect", "in the Nynorsk dialect", 1,
            "The Nynorsk dialect is usually only seen in writing, and uses a vocabulary and spelling that is much closer to modern spoken Norwegian dialects. Nonetheless, it is relatively uncommon to hear someone speaking in this particular form; it would be considered fairly formal.",
            "literary");
        AddAccent("Norwegian", "riksmål", "in the Riksmål dialect", "in the Riksmål dialect", 1,
            "The Riksmål dialect is usually only seen in writing, and is an attempt to unify the Nynorsk and Bokmål forms, retaining features of both. Although not usually spoken, it is perhaps the most likely of the literary forms to be heard spoken and corresponds to a very formal register; perhaps being used in a context like a job interview for example.",
            "literary");
        AddAccent("Norwegian", "nordnorsk", "in the Nordnorsk dialect", "in a Northern dialect", 4,
            "The Nordnorsk dialect (meaning North Norwegian) is a particular variety of spoken Norweigan that is found in the far North of Norway, in areas such as Finnmark, Nordland and Troms. It is fairly similar to Trøndelag Norwegian, with only minor differences in word choice and idioms, but those two are otherwise reasonably distinct from southern and western varieties.",
            "northern");
        AddAccent("Norwegian", "trøndersk", "in the Trøndersk dialect", "in a Northern dialect", 4,
            "The Trøndersk dialect (named for Trøndelag) is a particular variety of spoken Norweigan that is found in the North of Norway, in areas such as Trøndelag, Nordmøre, Bindal and Frostviken. It is fairly similar to Nordnorsk Norwegian, with only minor differences in word choice and idioms, but those two are otherwise reasonably distinct from southern and western varieties.",
            "northern");
        AddAccent("Norwegian", "midlandsmål", "in the Midlandsmål dialect", "in a Southern dialect", 4,
            "The Midlandsmål dialect is a particular variety of spoken Norweigan that is found in the Midland region of Norway.",
            "southern");
        AddAccent("Norwegian", "østnorsk", "in the Østnorsk dialect", "in a Southern dialect", 4,
            "The Østnorsk dialect (meaning Eastern Norwegian) is a particular variety of spoken Norweigan that is found in the South East of Norway.",
            "southern");
        AddAccent("Norwegian", "sørlandsk", "in the Sørlandsk dialect", "in a Southern dialect", 4,
            "The Sørlandsk dialect (meaning Southern Norwegian) is a particular variety of spoken Norweigan that is found in the far South of Norway.",
            "southern");
        AddAccent("Norwegian", "vestlandsk", "in the Vestlandsk dialect", "in a Southern dialect", 4,
            "The Vestlandsk dialect (meaning Western Norwegian) is a particular variety of spoken Norweigan that is found in the South West of Norway.",
            "southern");
        AddAccent("Polish", "Malopolskie", "with a Malopolskie accent", "with a nasal Polish accent", 2,
            "the Malopolskie (Lesser Poland) accent", "nasal");
        AddAccent("Polish", "wielkopolska", "with a Wielkopolska accent", "with a nasal Polish accent", 2,
            "the Wielkopolska (Greater Poland) accent", "nasal");
        AddAccent("Polish", "kashubian", "in the Kashubian dialect", "in a Polish dialect", 2, "the Kashubian dialect",
            "regional");
        AddAccent("Polish", "silesian", "in the Silesian dialect", "in a regional Polish dialect", 2,
            "the Silesian dialect", "regional");
        AddAccent("Polish", "podhale", "with a Podhale accent", "with a rural Polish accent", 2, "the Podhale accent",
            "rural");
        AddAccent("Portugese", "angolan", "with an Angolan accent", "with an African accent", 3,
            "The form of Portugese spoken in Angola in Africa", "african");
        AddAccent("Portugese", "mozambican", "with a Mozambican accent", "with an African accent", 3,
            "The form of Portugese spoken in Mozambique in Africa", "african");
        AddAccent("Portugese", "goan", "with a Goan accent", "with an Asian accent", 3,
            "The form of Portugese spoken in the Indian colony of Goa", "asian");
        AddAccent("Portugese", "macaun", "with a Macaun accent", "with an Asian accent", 3,
            "The form of Portugese spoken in the Chinese colony Macau", "asian");
        AddAccent("Portugese", "timorese", "with a Timorese accent", "with an Asian accent", 3,
            "The form of Portugese spoken in the Indonesian colony of Timor", "asian");
        AddAccent("Portugese", "northern brazilian", "with a Northern Brazilian accent", "with a Brazilian accent", 2,
            "The form of Portugese spoken in Northern Brazil", "brazilian");
        AddAccent("Portugese", "rio de janeiro", "with a Rio De Janeiro accent", "with a Brazilian accent", 2,
            "The form of Southern Brazilian spoken in Rio De Janeiro, a prestige accent in Brazil", "brazilian");
        AddAccent("Portugese", "sao paulo", "with a Sao Paulo accent", "with a Brazilian accent", 2,
            "The form of Southern Brazilian spoken in Sao Paulo, a prestige accent in Brazil", "brazilian");
        AddAccent("Portugese", "southern brazilian", "with a Southern Brazilian accent", "with a Brazilian accent", 2,
            "The form of Portugese spoken in Southern Brazil", "brazilian");
        AddAccent("Portugese", "barranquenho", "with a Barranquenho accent", "with a European accent", 3,
            "The form of Portugese spoken in the town of Barrancos, a heavily Spanish influenced border town",
            "european");
        AddAccent("Portugese", "central", "with a Central accent", "with a European accent", 2,
            "The form of Portugese spoken in the central regions of Portugal", "european");
        AddAccent("Portugese", "lisbon", "with a Lisbonian accent", "with a European accent", 2,
            "The form of Portugese spoken in the capital Lisbon; the basis of standard Portugese", "european");
        AddAccent("Portugese", "northern", "with a Northern accent", "with a European accent", 2,
            "The form of Portugese spoken in the northern regions of Portugal", "european");
        AddAccent("Portugese", "southern", "with a Southern accent", "with a European accent", 2,
            "The form of Portugese spoken in the southern regions of Portugal", "european");
        AddAccent("Romani", "balkan", "in a Balkan dialect", "in a Non-Vlax dialect", 4,
            "The Balkan dialect is spoken by Romani speakers in and around the Black Sea.", "balkan");
        AddAccent("Romani", "baltic", "in a Baltic dialect", "in a Non-Vlax dialect", 4,
            "The Baltic dialect is spoken by Romani speakers in and around Estonia, Latvia, Russia and Poland.",
            "northern");
        AddAccent("Romani", "carpathian", "in a Carpathian dialect", "in a Non-Vlax dialect", 4,
            "The Carpathian dialect is spoken by Romani speakers in and around Slovakia and Moravia.", "northern");
        AddAccent("Romani", "kalo", "in a Kalo dialect", "in a Non-Vlax dialect", 4,
            "The Kalo dialect is spoken by Romani speakers in Finland.", "northern");
        AddAccent("Romani", "sinte", "in a Sinte dialect", "in a Non-Vlax dialect", 4,
            "The Sinte dialect is spoken by Romani speakers in and around Serbian, Croatia and Slovenia.", "northern");
        AddAccent("Romani", "welsh", "in a Welsh dialect", "in a Non-Vlax dialect", 4,
            "The Welsh dialect is spoken by Romani speakers in Wales, and is descended from Baltic Romani immigrants to the area.",
            "northern");
        AddAccent("Romani", "vlax", "in a Vlax dialect", "in a Vlax dialect", 4,
            "The Vlax dialect is historically spoken by Romani in the Transvaal and Wallachian areas, but is also a substantial source of origin for other Romani populations further abroad.",
            "vlax");
        AddAccent("Russian", "caucasian", "with a Caucasian accent", "with a Regional accent", 2,
            "the Caucasian accent", "caucasian");
        AddAccent("Russian", "vladivostoksky", "with a Vladivostoksky accent", "with an Eastern accent", 2,
            "the Vladivostoksky accent", "eastern");
        AddAccent("Russian", "arkangelsk", "with an Arkangelsk accent", "with a Northern accent", 2,
            "the Arkangelsk accent", "northern");
        AddAccent("Russian", "petrogradsky", "with a Petrogradsky accent", "with a Northern accent", 2,
            "the Petrogradsky accent", "northern");
        AddAccent("Russian", "muscovy", "with a Muscovy accent", "with a Russian accent", 1, "the Muscovy accent",
            "russian");
        AddAccent("Russian", "belarusian", "with a Belarusian accent", "with a Ruthenian accent", 2,
            "the Belarusian accent", "ruthenian");
        AddAccent("Russian", "crimean", "with a Crimean accent", "with a Ukranian accent", 2, "the Crimean accent",
            "ukranian");
        AddAccent("Russian", "kievan", "with a Kievan accent", "with a Ukranian accent", 2, "the Kievan accent",
            "ukranian");
        AddAccent("Russian", "odessan", "with an Odessan accent", "with a Ukranian accent", 2, "the Odessan accent",
            "ukranian");
        AddAccent("Scottish Gaelic", "edinburgh", "with an Edinburgh accent", "with a Scottish accent", 2,
            "The accent of speakers from Edinburgh (Dun Eideann)", "scottish");
        AddAccent("Scottish Gaelic", "glasgow", "with a Glasweigan accent", "with a Scottish accent", 2,
            "The accent of speakers from Glasgow (Glaschu)", "scottish");
        AddAccent("Scottish Gaelic", "highlands", "with a Highlands accent", "with a Scottish accent", 2,
            "The accent of speakers from the Highlands", "scottish");
        AddAccent("Scottish Gaelic", "inverness", "with an Inverness accent", "with a Scottish accent", 2,
            "The accent of speakers from Inverness (Inbhir Nis)", "scottish");
        AddAccent("Scottish Gaelic", "western isles", "with a Western Isles accent", "with a Scottish accent", 2,
            "The accent of speakers from the Western Isles", "scottish");
        AddAccent("Sioux", "standard", "with a Standard accent", "with a Standard accent", 2,
            "The standard accent of a speaker of Sioux", "standard");
        AddAccent("Spanish", "filipino", "with a Filipino accent", "with an Asian accent", 3, "the Filipino accent",
            "asian spanish");
        AddAccent("Spanish", "cuban", "with a Cuban accent", "with a Caribbean accent", 3, "the Cuban accent",
            "caribbean");
        AddAccent("Spanish", "dominican", "with a Dominican accent", "with a Caribbean accent", 3,
            "the Dominican accent", "caribbean");
        AddAccent("Spanish", "panamanian", "with a Panamanian accent", "with a Caribbean accent", 3,
            "the Panamanian accent", "caribbean");
        AddAccent("Spanish", "puerto rican", "with a Puerto Rican accent", "with a Caribbean accent", 3,
            "the Puerto Rican accent", "caribbean");
        AddAccent("Spanish", "venezuelan", "with a Venezuelan accent", "with a Caribbean accent", 3,
            "the Venezuelan accent", "caribbean");
        AddAccent("Spanish", "madrilenian", "with a Madrilenian accent", "with a Central accent", 2,
            "the Madrilenian accent", "central spain");
        AddAccent("Spanish", "toledan", "with a Toledan accent", "with a Central accent", 2, "the Toledan accent",
            "central spain");
        AddAccent("Spanish", "valencian", "with a Valencian accent", "with a Central accent", 2, "the Valencian accent",
            "central spain");
        AddAccent("Spanish", "central mexican", "with a Central Mexican accent", "with a Mexican accent", 3,
            "the Central Mexican accent", "mexican");
        AddAccent("Spanish", "southern mexican", "with a Southern Mexican accent", "with a Mexican accent", 3,
            "the Southern Mexican accent", "mexican");
        AddAccent("Spanish", "aragonese", "with an Aragonese accent", "with a Northern accent", 2,
            "the Aragonese accent", "northern spain");
        AddAccent("Spanish", "basque", "with a Basque accent", "with a Northern accent", 2, "the Basque accent",
            "northern spain");
        AddAccent("Spanish", "cantabrian", "with a Cantabrian accent", "with a Northern accent", 2,
            "the Cantabrian accent", "northern spain");
        AddAccent("Spanish", "castillan", "with a Castillan accent", "with a Northern accent", 1,
            "the Castillan accent", "northern spain");
        AddAccent("Spanish", "navarran", "with a Navarran accent", "with a Northern accent", 2, "the Navarran accent",
            "northern spain");
        AddAccent("Spanish", "argentinian", "with an Argentinian accent", "with a South American accent", 3,
            "the Argentinian accent", "south american");
        AddAccent("Spanish", "bolivian", "with a Bolivian accent", "with a South American accent", 3,
            "the Bolivian accent", "south american");
        AddAccent("Spanish", "chilean", "with a Chilean accent", "with a Chilean accent", 3, "the Chilean accent",
            "south american");
        AddAccent("Spanish", "colombian", "with a Colombian accent", "with a South American accent", 3,
            "the Colombian accent", "south american");
        AddAccent("Spanish", "ecuadorian", "with an Ecuadorian accent", "with a South American accent", 3,
            "the Ecuadorian accent", "south american");
        AddAccent("Spanish", "paraguayan", "with a Paraguayan accent", "with a South American accent", 3,
            "the Paraguayan accent", "south american");
        AddAccent("Spanish", "peruvian", "with a Peruvian accent", "with a South American accent", 3,
            "the Peruvian accent", "south american");
        AddAccent("Spanish", "uruguayan", "with a Uruguayan accent", "with a South American accent", 3,
            "the Uruguayan accent", "south american");
        AddAccent("Spanish", "andalusian", "with an Andalusian accent", "with a Southern accent", 2,
            "the Andalusian accent", "southern spain");
        AddAccent("Spanish", "murcian", "with a Murcian accent", "with a Southern accent", 2, "the Murcian accent",
            "southern spain");
        AddAccent("Swahili", "chimwiini", "in the Chimwiini dialect", "in the Chinwiini dialect", 6,
            "The dialect of Swahili spoken by some ethnic minorities in Barawa in Somalia. Somewhat hard for those who aren't familiar with it to understand.",
            "chimwiini");
        AddAccent("Swahili", "kisetla", "in the Kisetla dialect", "in the Kisetla dialect", 5,
            "A version of Swahili spoken by white settlers in Swahili-speaking countries.", "foreign");
        AddAccent("Swahili", "kibajuni", "in the Kibajuni dialect", "in the Kibajuni dialect", 1,
            "The dialect of Swahili spoken by the Bajuni minority ethnic group in Somalia and Kenya. Somewhat hard for those who aren't familiar with it to understand.",
            "kibajuni");
        AddAccent("Swahili", "kimwani", "in the Kimwani dialect", "in the Kimwani dialect", 6,
            "The dialect of Swahili spoken in the Kerimba islands and northern coastal Mozambique. Somewhat hard for those who aren't familiar with it to understand.",
            "kimwani");
        AddAccent("Swahili", "chichifundi", "in the Chichifundi dialect", "in a Mombasa-Lamu dialect", 2,
            "A dialect of Swahili spoken in southern Kenyan coastal areas.", "mombasa-lamu");
        AddAccent("Swahili", "kimrima", "in the Kimrima dialect", "in a Mombasa-Lamu dialect", 2,
            "The dialect of Swahili spoken in Pangani, Vanga, Dar es Salaam, Rufiji and Mafia Island.", "mombasa-lamu");
        AddAccent("Swahili", "kivumba", "in the Kivumba dialect", "in a Mombasa-Lamu dialect", 2,
            "A dialect of Swahili spoken in southern Kenyan coastal areas.", "mombasa-lamu");
        AddAccent("Swahili", "lamu", "in the Lamu dialect", "in a Mombasa-Lamu dialect", 2,
            "The dialect of Swahili spoken in and around the island of Lamu. Considered by some (especially the speakers of it) to be the original dialect of Swahili.",
            "mombasa-lamu");
        AddAccent("Swahili", "mombasa", "in the Mombasa dialect", "in a Mombasa-Lamu dialect", 2,
            "The dialect of Swahili spoken in and around Mombasa area.", "mombasa-lamu");
        AddAccent("Swahili", "nosse be", "in the Nosse Be dialect", "in a Mombasa-Lamu dialect", 2,
            "The dialect of Swahili spoken in Madagascar.", "mombasa-lamu");
        AddAccent("Swahili", "kimakunduchi", "in the Kimakunduchi dialect", "in a Pemba dialect", 2,
            "A dialect of Swahili spoken in rural areas near Zanzibar. This particular dialect used to be called Kihadimu, which literally means language of the serfs.",
            "pemba");
        AddAccent("Swahili", "kipemba", "in the Kipemba dialect", "in a Pemba dialect", 2,
            "The dialect of Swahili spoken in Pemba Island off the coast of Tanzania.", "pemba");
        AddAccent("Swahili", "kitumbatu", "in the Kitumbatu dialect", "in a Pemba dialect", 2,
            "A dialect of Swahili spoken in rural areas near Zanzibar.", "pemba");
        AddAccent("Swahili", "makunduchi", "in the Makunduchi dialect", "in a Pemba dialect", 2,
            "The dialect of Swahili spoken in Makunduchi in Tanzania.", "pemba");
        AddAccent("Swahili", "sheng", "in the Sheng dialect", "in the Sheng dialect", 5,
            "A mixture of Swahili, English and other ethnic Kenyan languages. Something of a street language in Nairobi, but also fashionable and cosmopolitan to those wanting to sound 'gangster'.",
            "sheng");
        AddAccent("Swahili", "kiunguja", "in the Kiunguja dialect", "in the Kiunguja dialect", 1,
            "The dialect of Swahili spoken in Zanzibar Town, considered the modern standard version. This is the dialect of government, education and media.",
            "standard");
        AddAccent("Swedish", "finnish", "in the Finnish dialect", "in an Finnish dialect", 2,
            "The Finnish dialect is the form of Swedish spoken in Finland. It is very closely related to the Helsinki dialect.",
            "finish");
        AddAccent("Swedish", "ostrobothnian", "in the Ostrobothnian dialect", "in a Finnish dialect", 7,
            "The Ostrobothnian dialect is the form of Swedish spoken in Finland, in one of the few areas outside of major cities where Swedish is the majority language over Finnish. It is very difficult for other Swedish speakers (especially non-Finns) to understand.",
            "finnish");
        AddAccent("Swedish", "dalsland", "in the Dalsland dialect", "in a Götaland dialect", 2,
            "The Dalsland dialect is a Götaland dialect spoken roughly between the South Sweden and Svealand dialect areas. It may be perceived as mildly rustic by Urbanised Swedes.",
            "götaland");
        AddAccent("Swedish", "northern halland", "in the Northern Halland dialect", "in a Götaland dialect", 2,
            "The Northern Halland dialect is a Götaland dialect spoken in northern parts of the Halland province. It may be perceived as mildly rustic by Urbanised Swedes.",
            "götaland");
        AddAccent("Swedish", "northern småland", "in the Northern Småland dialect", "in a Götaland dialect", 2,
            "The Northern Småland dialect is a Götaland dialect spoken in the northern parts of Småland. It may be perceived as mildly rustic by Urbanised Swedes.",
            "götaland");
        AddAccent("Swedish", "östergötland", "in the Östergötland dialect", "in a Götaland dialect", 2,
            "The Östergötland dialect is a Götaland dialect spoken roughly between the South Sweden and Svealand dialect areas. It may be perceived as mildly rustic by Urbanised Swedes.",
            "götaland");
        AddAccent("Swedish", "västergötland", "in the Västergötland dialect", "in a Götaland dialect", 2,
            "The Västergötland dialect is a Götaland dialect spoken roughly between the South Sweden and Svealand dialect areas. It may be perceived as mildly rustic by Urbanised Swedes.",
            "götaland");
        AddAccent("Swedish", "ångermanland", "in the Ångermanlandska dialect", "in a Norrland dialect", 4,
            "Ångermanlandska is a Norrland dialect spoken in southern Ångermanland. It preserves some archaic vowel pronounciations and is quite distinctive in sound.",
            "norrland");
        AddAccent("Swedish", "kalix", "in the Kalix dialect", "in a Norrland dialect", 4,
            "Kalix, as a Norrland dialect is a northern Swedish accent heavily influenced by both East Old Nordic and West Old Nordic, as well as Sami to a lesser degree. It is spoken in Kalix and Överkalix municipalities.",
            "norrland");
        AddAccent("Swedish", "lulemål", "in the Lulemål dialect", "in a Norrland dialect", 4,
            "Lulemål, as a Norrland dialect is a northern Swedish accent heavily influenced by both East Old Nordic and West Old Nordic. It is spoken in Boden and Lulea municipalities.",
            "norrland");
        AddAccent("Swedish", "nordvästerbottniska", "in the Nordvästerbottniska dialect", "in a Norrland dialect", 4,
            "Nordvästerbottniska is a Norrland dialect spoken in northern Västerbotten and parts of Lappland. It preserves many archaic Swedish features and is very distinctive.",
            "norrland");
        AddAccent("Swedish", "nybyggarmål", "in the Nybyggarmål dialect", "in a Norrland dialect", 4,
            "Nybyggarmålis also known as 'settler swedish', and is spoken in Lappland. It is so called because it was heavily settled by ethnic swedes.",
            "norrland");
        AddAccent("Swedish", "sydvästerbottniska", "in the Sydvästerbottniska dialect", "in a Norrland dialect", 4,
            "Sydvästerbottniska is a Norrland dialect spoken in southern Västerbotten and parts of the borders with Norway and Finland. It preserves some archaic vowel pronounciations and is quite distinctive in sound.",
            "norrland");
        AddAccent("Swedish", "blekinge", "in the Blekinge dialect", "in a South Swedish dialect", 4,
            "The Blekinge dialect is spoken in the southwesternmost portion of Sweden, in territory historically held by the Danish. Like all Southern Swedish dialects, it is heavily influenced by Danish and contains many of the same features that they do.",
            "south swedish");
        AddAccent("Swedish", "skåne", "in the Skåne dialect", "in a South Swedish dialect", 4,
            "The Skåne dialect is spoken in the southwesternmost portion of Sweden, in territory historically held by the Danish. Like all Southern Swedish dialects, it is heavily influenced by Danish and contains many of the same features that they do.",
            "south swedish");
        AddAccent("Swedish", "southern halland", "in the Southern Halland dialect", "in a South Swedish dialect", 4,
            "The Southern Halland dialect is spoken in the southwesternmost portion of Sweden, in territory historically held by the Danish. Like all Southern Swedish dialects, it is heavily influenced by Danish and contains many of the same features that they do.",
            "south swedish");
        AddAccent("Swedish", "southern småland", "in the Southern Småland dialect", "in a South Swedish dialect", 4,
            "The Southern Småland dialect is spoken in the southern parts of Sweden. Like all Southern Swedish dialects, it is heavily influenced by Danish and contains many of the same features that they do.",
            "south swedish");
        AddAccent("Swedish", "gothenburg", "in the Gothenburg dialect", "in an Urban dialect", 2,
            "The Gothenburg dialect is a form of urban Swedish, very similar to Rikssvenska but with its own distinct regional flair.",
            "standard");
        AddAccent("Swedish", "helsinki", "in the Helsinki dialect", "in an Urban dialect", 1,
            "The Helsinki dialect, also known as högsvenska or 'high swedish' is a prestige urban accent of the Swedish language. It is closely related to the Stockholm rikssvenska dialect.",
            "standard");
        AddAccent("Swedish", "stockholm", "in the Stockholm dialect", "in an Urban dialect", 1,
            "The Stockholm dialect, also known as rikssvenska or 'realm swedish' is a prestige urban accent of the Swedish language. It is the closest thing to a standard spoken version of the language, although spoken Swedish has no such form.",
            "standard");
        AddAccent("Swedish", "åland", "in the Åland dialect", "in a Svealand dialect", 2,
            "The Åland dialect is the form of Swedish spoken on the island of Åland, which is an autonomous territory of Finland. The language itself shares a mixture of features between the Uppland dialect of Swedish and Finnish Swedish, but is quite distinctive.",
            "svealand");
        AddAccent("Swedish", "svealand", "in the Svealand dialect", "in a Svealand dialect", 2,
            "The Svealand dialect is a Svealand dialect spoken in the Svealand region north of Stockholm. It has a mixture of features from southern and northern variants of Swedish, as well as a distinct way of adding the 'r' sound in certain contexts where other Swedish speakers would not. It is very closely related to the Rikssvenska dialect, and can be considered its ancestor.",
            "svealand");
        AddAccent("Swedish", "uppland", "in the Uppland dialect", "in a Svealand dialect", 2,
            "The Uppland dialect is a Svealand dialect spoken in the Uppland region around Uppsala. It has a mixture of features from southern and northern variants of Swedish, as well as a distinct way of adding the 'r' sound in certain contexts where other Swedish speakers would not. It is very closely related to the Rikssvenska dialect, and can be considered its ancestor.",
            "svealand");
        AddAccent("Turkish", "eastern anatolian", "in the Eastern Anatolian dialect", "in an Anatolian dialect", 3,
            "The dialect of Turkish spoken in Eastern Anatolia in Turkey.", "anatolian");
        AddAccent("Turkish", "northeastern anatolian", "in the Northeastern Anatolian dialect",
            "in an Anatolian dialect", 3,
            "The dialect of Turkish spoken in Northeastern Anatolia in Turkey in cities such as Trabzon and Rize.",
            "anatolian");
        AddAccent("Turkish", "western anatolian", "in the Western Anatolian dialect", "in an Anatolian dialect", 3,
            "The dialect of Turkish spoken in Western Anatolia in Turkey.", "anatolian");
        AddAccent("Turkish", "cypriot", "in the Cypriot dialect", "in the Cypriot dialect", 5,
            "The language of the Turkish population of Cyprus. A high degree of influence from Greek, somewhat distinct from other forms of turkish.",
            "cypriot");
        AddAccent("Turkish", "eastern rumelian", "in the Eastern Rumelian dialect", "in a European dialect", 3,
            "The dialect of Turkish spoken in Eastern Rumelia (Greece, Western Turkey, Bulgaria).", "european");
        AddAccent("Turkish", "western rumelian", "in the Western Rumelian dialect", "in a European dialect", 3,
            "The dialect of Turkish spoken in Western Rumelia (Romania, Yugoslavia).", "european");
        AddAccent("Turkish", "azerbaijani", "in the Azerbaijani dialect", "in an Oghuz dialect", 5,
            "Considered its own language, but highly mutually intelligable with standard Turkish. The language of Azerbaijan.",
            "oghuz");
        AddAccent("Turkish", "syrian", "in the Syrian dialect", "in an Oghuz dialect", 5,
            "A variety of Turkish spoken by ethnic Turkmen in Syria. Very similar to Turkmen.", "oghuz");
        AddAccent("Turkish", "turkmen", "in the Turkmen dialect", "in an Oghuz dialect", 5,
            "Considered its own language, but highly mutually intelligable with standard Turkish. The language of Turkmenistan.",
            "oghuz");
        AddAccent("Turkish", "istanbul", "in the Istanbul standard accent", "in the Istanbul standard accent", 1,
            "The standard accent and dialect of the Turkish language, spoken in Istanbul and the official language of government, media and education.",
            "standard");
        AddAccent("Ukranian", "balachka", "in the Balachka dialect", "in a Foreign dialect", 4,
            "Spoken in the Kuban region of Russia, by the Kuban Cossacks. The Kuban Cossacks being descendants of the Zaporozhian Cossacks are beginning to consider themselves as a separate ethnic identity. Their dialect is based on Middle Dnieprian with the Ukrainian grammar. It includes dialectical words of central Ukrainian with frequent inclusion of Russian vocabulary, in particular for modern concepts and items. It varies somewhat from one area to another.",
            "foreign");
        AddAccent("Ukranian", "upper sannian", "in the Upper Sannian dialect", "in a Foreign dialect", 3,
            "Spoken in the border area between Ukraine and Poland in the San river valley. Often confused as Lemko or Lyshak.",
            "foreign");
        AddAccent("Ukranian", "central polissian", "in the Central Polissian dialect", "in a Northern dialect", 4,
            "Spoken in the northwestern part of the Kiev Oblast, in the northern part of Zhytomyr and the northeastern part of the Rivne Oblast.",
            "northern");
        AddAccent("Ukranian", "eastern polissian", "in the East Polissian dialect", "in a Northern dialect", 3,
            "Spoken in Chernihiv (excluding the southeastern districts), in the northern part of Sumy, and in the southeastern portion of the Kiev Oblast as well as in the adjacent areas of Russia, which include the southwestern part of the Bryansk Oblast (the area around Starodub), as well as in some areas in the Kursk, Voronezh and Belgorod Oblasts.[8] No linguistic border can be defined. The vocabulary approaches Russian as the language approaches the Russian Federation. Both Ukrainian and Russian grammar sets can be applied to this dialect. Thus, this dialect can be considered a transitional dialect between Ukrainian and Russian.",
            "northern");
        AddAccent("Ukranian", "west polissian", "in the West Polissian dialect", "in a Northern dialect", 4,
            "Spoken in the northern part of the Volyn Oblast, the northwestern part of the Rivne Oblast as well as in the adjacent districts of the Brest Voblast in Belarus. The dialect spoken in Belarus uses Belarusian grammar, and thus is considered by some to be a dialect of Belarusian.",
            "northern");
        AddAccent("Ukranian", "middle dnieprian", "in the Middle Dnieprian dialect", "in a Southeastern dialect", 2,
            "The basis of the Standard Literary Ukrainian. It is spoken in the central part of Ukraine, primarily in the southern and eastern part of the Kiev Oblast). In addition, the dialects spoken in Cherkasy, Poltava and Kiev regions are considered to be close to \"standard\" Ukrainian.",
            "southeastern");
        AddAccent("Ukranian", "slobozhan", "in the Slobozhan dialect", "in a Southeastern dialect", 2,
            "Spoken in Kharkiv, Sumy, Luhansk, and the northern part of Donetsk, as well as in the Voronezh and Belgorod regions of Russia.[4] This dialect is formed from a gradual mixture of Russian and Ukrainian, with progressively more Russian in the northern and eastern parts of the region. Thus, there is no linguistic border between Russian and Ukrainian, and thus, both grammar sets can be applied. This dialect is considered a transitional dialect between Ukrainian and Russian.",
            "southeastern");
        AddAccent("Ukranian", "steppe", "in the Steppe dialect", "in a Southeastern dialect", 2,
            "Is spoken in southern and southeastern Ukraine. This dialect was originally the main language of the Zaporozhian Cossacks.",
            "southeastern");
        AddAccent("Ukranian", "boyko", "in the Boyko dialect", "in a Southwestern dialect", 2,
            "Spoken by the Boyko people on the northern side of the Carpathian Mountains in the Lviv and Ivano-Frankivsk Oblasts. It can also be heard across the border in the Subcarpathian Voivodeship of Poland.",
            "southwestern");
        AddAccent("Ukranian", "bukovynian", "in the Bukovynian dialect", "in a Southwestern dialect", 2,
            "Spoken in the Chernivtsi Oblast of Ukraine. This dialect has some distinct vocabulary borrowed from Romanian.",
            "southwestern");
        AddAccent("Ukranian", "hutsul", "in the Hutsul dialect", "in a Southwestern dialect", 2,
            "Spoken by the Hutsul people on the northern slopes of the Carpathian Mountains, in the extreme southern parts of the Ivano-Frankivsk Oblast, as well as in parts of the Chernivtsi and Transcarpathian Oblasts.",
            "southwestern");
        AddAccent("Ukranian", "lemko", "in the Lemko dialect", "in a Southwestern dialect", 2,
            "Spoken by the Lemko people, most of whose homeland rests outside the current political borders of Ukraine in the Prešov Region of Slovakia along the southern side of the Carpathian Mountains, and in the southeast of modern Poland, along the northern sides of the Carpathians.",
            "southwestern");
        AddAccent("Ukranian", "podillian", "in the Podillian dialect", "in a Southwestern dialect", 2,
            "Spoken in the southern parts of the Vinnytsia and Khmelnytskyi Oblasts, in the northern part of the Odessa Oblast, and in the adjacent districts of the Cherkasy Oblast, the Kirovohrad Oblast and the Mykolaiv Oblast.",
            "southwestern");
        AddAccent("Ukranian", "rusyn", "in the Rusyn dialect", "in a Southwestern dialect", 4,
            "Spoken by the Rusyn people, who live in Transcarpathia around Uzhhorod. It is similar to the Lemko dialect but differs from them by the active use of Russian and Hungarian elements. There is an active movement to make this dialect a separate language distinct from Ukrainian.",
            "southwestern");
        AddAccent("Ukranian", "upper dniestrian", "in the Upper Dniestrian dialect", "in a Southwestern dialect", 2,
            "Considered to be the main Galician dialect, spoken in the Lviv, Ternopil and Ivano-Frankivsk Oblasts. Its distinguishing characteristics are the influence of Polish and the German vocabulary, which is reminiscent of the Austro-Hungarian rule.",
            "southwestern");
        AddAccent("Ukranian", "volynian", "in the Volynian dialect", "in a Southwestern dialect", 2,
            "Spoken in Rivne and Volyn, as well as in parts of Zhytomyr and Ternopil. It is also used in Chełm in Poland.",
            "southwestern");
        AddAccent("Yue", "gaoyang", "in the Gao-Yang dialect", "with a rural dialect", 4,
            "The accent and dialect of speakers of Yue Chinese from the coastal areas of Yangjiang and Lianjiang",
            "coastal");
        AddAccent("Yue", "qinlian", "in the Qin-Lian dialect", "with a rural dialect", 4,
            "The accent and dialect of speakers of Yue Chinese from the coastal areas of Beihai, Qinzhou and Fangcheng",
            "coastal");
        AddAccent("Yue", "wuhua", "in the Wu-Hua dialect", "with a rural dialect", 4,
            "The accent and dialect of speakers of Yue Chinese from the coastal areas of Wuchuan and Huazhou",
            "coastal");
        AddAccent("Yue", "goulou", "in the Gou-Lou dialect", "with a pearl river dialect", 2,
            "The accent and dialect of speakers of Yue Chinese from western Guangdong and eastern Guangxi",
            "pearl river");
        AddAccent("Yue", "yongxun", "in the Yong-Xun dialect", "with a pearl river dialect", 2,
            "The accent and dialect of speakers of Yue Chinese from the Yong Yu Xun valley", "pearl river");
        AddAccent("Yue", "taishan", "in the Taishan dialect", "with a rural dialect", 4,
            "The accent and dialect of rural speakers of Yue Chinese from Fujian and Guangdong, also very common in Chinese immigrants in other countries",
            "taishan");
        AddAccent("Yupik", "standard", "with a Standard accent", "with a Standard accent", 2,
            "The standard accent of a speaker of Yupik", "standard");

        SeedModernLanguageCoverage();

        #endregion
    }

	private void SeedModernLanguageCoverage()
	{
		SeedHistoricalLanguagePack(
			ModernLanguageCoverage,
			ModernScriptCoverage,
			ModernMutualIntelligibilityCoverage);

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

	private static readonly Dictionary<string, string[]> ModernLanguageCoverageMap =
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

	private static readonly Dictionary<string, string[]> AntiquityLanguageCoverageMap =
		new(System.StringComparer.OrdinalIgnoreCase)
		{
			["Scythian-Sarmatian"] = ["Scythian", "Sarmatian"],
			["Kushite"] = ["Meroitic"]
		};

	private static readonly Dictionary<string, string[]> RenaissanceEuropeLanguageCoverageMap =
		new(System.StringComparer.OrdinalIgnoreCase)
		{
			["Albanian"] = ["Albanian"]
		};

	private static readonly HistoricalLanguageSeed[] ModernLanguageCoverage =
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

	private static readonly HistoricalScriptSeed[] ModernScriptCoverage =
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

	private static readonly HistoricalMutualIntelligibilitySeed[] ModernMutualIntelligibilityCoverage =
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
}
