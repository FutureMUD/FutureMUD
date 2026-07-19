#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
    public void SeedModernHeritage()
    {
        AddEthnicity(_humanRace, "Germanic", "Western European", "O-A High Negative", 0, 0,
            description:
            "The Germanic peoples are an Indo-European ethno-linguistic group found in Britain, France, Germany and the Low Countries. They are typically characteristised by fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "Italic", "Mediterranian", "O-A High Negative", 0, 0,
            description:
            "The Italic peoples are an Indo-European ethno-linguistic group with their origins on the Italian peninsula. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Hispanic", "Mediterranian", "O-A High Negative", 0, 0,
            description:
            "The Hispanic peoples are an Indo-European ethno-linguistic group with their origins on the Iberian Peninsula. Due to extensive colonisation of the Americas, many American ethnic groups are ultimately Hispanic as well. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Celtic", "Western European", "Majority O Minor A", 0, 0,
            description:
            "The Celtic peoples are an Indo-European ethno-linguistic group predominantly found in the British Isles and France. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Slavic", "Eastern European", "O-A High Negative", 0, 0,
            description:
            "The Slavic peoples are an Indo-European ethno-linguistic group found in Central Europe, Eastern Europe and Central Asia. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Greek", "Mediterranian", "O-A High Negative", 0, 0,
            description:
            "The Greek peoples are an Indo-European ethno-linguistic group found to some extent most everywhere in the eastern Mediterranian (though predominantly Greece, Anatolia and Southern Italy). They are typically characterised by fair to olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Turkish", "Middle Eastern", "A Dominant", 0, 0,
            description:
            "The Turkish peoples are an Indo-European ethno-linguistic group found predominantly in Turkey and Central Asia. They are typically characterised by dark olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Arabic", "Middle Eastern", "Majority O Minor A", 0, 0,
            description:
            "The Arabic peoples are an Indo-European ethno-linguistic group found predominantly in the Arabian Peninsula, North Africa and the Horn of Africa. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Persian", "Middle Eastern", "Majority O Minor A", 0, 0,
            description:
            "The Persian peoples are an Indo-European ethno-linguistic group found in Central Asia and the Middle East. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Scandanavian", "Western European", "A Dominant", 0, 0,
            description:
            "The Scandanavian peoples are an Indo-European ethno-linguistic group found in Northern Europe. They are typically characterised by very fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "North African", "African", "Majority O", 0, 0,
            description:
            "The North African peoples are an Afro-Asiatic ethno-linguistic group found in North Africa. They emerged as a distinct group from Sub-Saharan Africans due to the isolating effect of the desert. They are characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Bantu", "African", "Majority O", 0, 0,
            description:
            "The Bantu peoples are a Sub-Saharan ethno-linguistic family originating in West Africa, but found across the vast majority of the continent. They are characterised by dark brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Khoisan", "African", "Majority O", 0, 0,
            description:
            "The Khoisan peoples are a Sub-Saharan ethno-linguistic group found mainly in Southern Africa. They are characterised by dark brown skin, dark hair and dark, hooded eyes.");
        AddEthnicity(_humanRace, "Swahili", "African", "Majority O", 0, 0,
            description:
            "The Swahili peoples are a Sub-Saharan ethno-linguistic group found along the eastern coast of Africa, ethnically related to the Bantu and Arabic peoples. They are characterised by brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Polynesian", "Oceanic", "Majority O", 0, 0,
            description:
            "The Polynesian peoples are an Oceanic ethno-linguistic group found across the eastern and southern pacific. They are characterised by light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Austronesian", "Oceanic", "B Dominant", 0, 0,
            description:
            "The Austronesian peoples are found mostly around the Indian Ocean, including the Madagascans, Taiwanese, Malaysians, Fillipinos and Indonesians. They are characterised by light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Melanesian", "Oceanic", "Majority O", 0, 0,
            description:
            "The Melanesian peoples are an Oceanic ethno-linguistic group found in Papua New Guinea, Fiji, Vanuatu and other nearby islands. They are characterised by brown skin, dark hair and dark eyes, although a significant number also have blonde hair.");
        AddEthnicity(_humanRace, "Dravidian", "Indian", "B Dominant", 0, 0,
            description:
            "The Dravidian peoples are an Indo-European ethno-linguistic group found in southern India and Sri Lanka. They are typically characterised by dark brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Indo-Aryan", "Indian", "B Dominant", 0, 0,
            description:
            "The Indo-Aryan peoples are an Indo-European ethno-linguistic group found in northern India, Bangladesh and Pakistan. They are typically characterised by light brown or brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Afro-Carribean", "African", "Majority O", 0, 0,
            description:
            "The Afro-Carribean peoples are the descendents of African slaves brought to the Carribean since European colonisation of the area. They come from a wide variety of Sub-Saharan African ethnicities and many have mixed heritage with European or Native American influences. Because of this, they have significant variety of physical characteristics. Skin colour ranges from light brown to dark brown, hair colour tends to be dark but both light and dark eyes do occur.");
        AddEthnicity(_humanRace, "Afro-American", "African", "Majority O", 0, 0,
            description:
            "The African-American peoples are the descendents of African slaves brought to North America since European colonisation of the area. They come from a wide variety of Sub-Saharan African ethnicities and many have mixed heritage with European or Native American influences. Because of this, they have significant variety of physical characteristics. Skin colour ranges from light brown to dark brown, hair colour tends to be dark but both light and dark eyes do occur.");
        AddEthnicity(_humanRace, "Eskimo", "Aboriginal", "A Dominant", 0, 0,
            description:
            "The Eskimo are an ethno-linguistic group who have traditionally inhabited the northern polar region.They are typically characterised by dark golden or reddish toned skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Amerindian", "Aboriginal", "Majority O Minor A", 0, 0,
            description:
            "The Amerindian are a North American ethno-linguistic family representing a number of distinct cultural groups in the United States and Canada. There is considerable variation in physical characteristics, but they tend to be characterised by golden, reddish toned or brown skin, and generally have dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Mesoamerican", "Aboriginal", "Majority O", 0, 0,
            description:
            "The Mesoamericans are a Central American ethno-linguistic family representing a number of distinct cultural groups in Mexico and Central America. They tend to be characterised by reddish toned or light brown skin, and generally have dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Andean", "Aboriginal", "Overwhelmingly O", 0, 0,
            description:
            "The Andeans are a South American ethno-linguistic family representing a number of distinct cultural groups in South America. They tend to be characterised by brown or olive skin, dark hair, and light or dark eyes.");
        AddEthnicity(_humanRace, "Mongolian", "East Asian", "B Dominant", 0, 0,
            description:
            "The Mongolians are an ethno-linguistic family representing both the peoples of Mongolia and many minority populations across Russia, China and other Northern Asian countries. They tend to be characterised by golden skin tones, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Han", "East Asian", "B Dominant", 0, 0,
            description:
            "The Han are an ethnic group native to east Asia, predominantly China, and are the most numerous ethnic group in the entire world, with a large diaspora. They tend to be characterised by golden skin tones, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Yamato", "East Asian", "B Dominant", 0, 0,
            description:
            "The Yamato are the predominant ethno-linguistic group in Japan. They are characterised by golden skin tones, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Korean", "East Asian", "B Dominant", 0, 0,
            description:
            "The Korean are an ethno-linguistic group found predominantly in the Korean peninsula and Manchuria. They are characterised by golden skin tones, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Tibetan", "East Asian", "B Dominant", 0, 0,
            description:
            "The Tibetans are an ethnic group found in areas of China, Indian, Nepal, and Bhutan. They are characterised by golden skin tones, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Uyghur", "East Asian", "A Dominant", 0, 0,
            description:
            "The Uyghur are an ethnic group of Turkic origin that live in Eastern and Central Asia (primarily China and the various Turkic states in Central Asia). They are characterised by fair or golden skin tones, dark hair and dark eyes (although fair haired Uyghur are not unheard of).");
        AddEthnicity(_humanRace, "Ainu", "Aboriginal", "B Dominant", 0, 0,
            description:
            "The Ainu are a small ethnic group found in Northern Japan and parts of Russia. They are characterised by fair or golden skin tones, dark hair and dark eyes. In particular, their men are renowned for their thick facial hair.");
        AddEthnicity(_humanRace, "Aboriginal Australian", "Aboriginal", "Majority O Minor A", 0, 0,
            description:
            "The Aboriginal Australians are an ethno-linguistic group found in and around Australia. They are characterised by dark brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Austro-Asiatic", "Aboriginal", "B Dominant", 0, 0,
            description:
            "The Austro-Asiatics are an ethno-linguistic group located in South East Asia, predominently Myanmar and Cambodia. They are characterised by golden or light brown skin tones, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Tai-Kadai", "East Asian", "B Dominant", 0, 0,
            description:
            "The Tai-Kadai are an ethno-linguistic group located in South East Asia, predominently Thailand and Laos. They are characterised by golden skin tones, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Anglo-Saxon", "Western European", "Majority O Minor A", 0, 0,
            description:
            "The Anglo-Saxon peoples are an Indo-European ethno-linguistic group found in the British Isles and various former British Colonies such as the USA, Canada, South Africa and Australia. They are typically characteristised by fair skin, fair hair and light eyes.");

        AddEthnicityVariable("Aboriginal Australian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Aboriginal Australian", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Aboriginal Australian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Aboriginal Australian", "Ears", "All Ears");
        AddEthnicityVariable("Aboriginal Australian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Aboriginal Australian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Aboriginal Australian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Aboriginal Australian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Aboriginal Australian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Aboriginal Australian", "Nose", "All Noses");
        AddEthnicityVariable("Aboriginal Australian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Aboriginal Australian", "Skin Colour", "dark_skin");
        AddEthnicityVariable("Afro-American", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Afro-American", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Afro-American", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Afro-American", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Afro-American", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Afro-American", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Afro-American", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Afro-American", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Afro-American", "Nose", "All Noses");
        AddEthnicityVariable("Afro-American", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Afro-American", "Skin Colour", "dark_skin");
        AddEthnicityVariable("Afro-Carribean", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Afro-Carribean", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Afro-Carribean", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Afro-Carribean", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Afro-Carribean", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Afro-Carribean", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Afro-Carribean", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Afro-Carribean", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Afro-Carribean", "Nose", "All Noses");
        AddEthnicityVariable("Afro-Carribean", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Afro-Carribean", "Skin Colour", "dark_skin");
        AddEthnicityVariable("Ainu", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Ainu", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Ainu", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Ainu", "Ears", "All Ears");
        AddEthnicityVariable("Ainu", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Ainu", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Ainu", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Ainu", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Ainu", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Ainu", "Nose", "All Noses");
        AddEthnicityVariable("Ainu", "Ears", "All Ears");
        AddEthnicityVariable("Ainu", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Ainu", "Skin Colour", "golden_skin");
        AddEthnicityVariable("Amerindian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Amerindian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Amerindian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Amerindian", "Ears", "All Ears");
        AddEthnicityVariable("Amerindian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Amerindian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Amerindian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Amerindian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Amerindian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Amerindian", "Nose", "All Noses");
        AddEthnicityVariable("Amerindian", "Ears", "All Ears");
        AddEthnicityVariable("Amerindian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Amerindian", "Skin Colour", "redbrown_skin");
        AddEthnicityVariable("Andean", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Andean", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Andean", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Andean", "Ears", "All Ears");
        AddEthnicityVariable("Andean", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Andean", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Andean", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Andean", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Andean", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Andean", "Nose", "All Noses");
        AddEthnicityVariable("Andean", "Ears", "All Ears");
        AddEthnicityVariable("Andean", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Andean", "Skin Colour", "redbrown_skin");
        AddEthnicityVariable("Anglo-Saxon", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Anglo-Saxon", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Anglo-Saxon", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Anglo-Saxon", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Anglo-Saxon", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Anglo-Saxon", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Anglo-Saxon", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Anglo-Saxon", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Anglo-Saxon", "Nose", "All Noses");
        AddEthnicityVariable("Anglo-Saxon", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Anglo-Saxon", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Arabic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Arabic", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Arabic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Arabic", "Ears", "All Ears");
        AddEthnicityVariable("Arabic", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Arabic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Arabic", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Arabic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Arabic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Arabic", "Nose", "All Noses");
        AddEthnicityVariable("Arabic", "Ears", "All Ears");
        AddEthnicityVariable("Arabic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Arabic", "Skin Colour", "swarthy_skin");
        AddEthnicityVariable("Austro-Asiatic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Austro-Asiatic", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Austro-Asiatic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Austro-Asiatic", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Austro-Asiatic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Austro-Asiatic", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Austro-Asiatic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Austro-Asiatic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Austro-Asiatic", "Nose", "All Noses");
        AddEthnicityVariable("Austro-Asiatic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Austro-Asiatic", "Skin Colour", "golden_skin");
        AddEthnicityVariable("Austronesian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Austronesian", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Austronesian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Austronesian", "Ears", "All Ears");
        AddEthnicityVariable("Austronesian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Austronesian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Austronesian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Austronesian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Austronesian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Austronesian", "Nose", "All Noses");
        AddEthnicityVariable("Austronesian", "Ears", "All Ears");
        AddEthnicityVariable("Austronesian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Austronesian", "Skin Colour", "golden_skin");
        AddEthnicityVariable("Bantu", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Bantu", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Bantu", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Bantu", "Ears", "All Ears");
        AddEthnicityVariable("Bantu", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Bantu", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Bantu", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Bantu", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Bantu", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Bantu", "Nose", "All Noses");
        AddEthnicityVariable("Bantu", "Ears", "All Ears");
        AddEthnicityVariable("Bantu", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Bantu", "Skin Colour", "dark_skin");
        AddEthnicityVariable("Celtic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Celtic", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Celtic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Celtic", "Ears", "All Ears");
        AddEthnicityVariable("Celtic", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Celtic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Celtic", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Celtic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Celtic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Celtic", "Nose", "All Noses");
        AddEthnicityVariable("Celtic", "Ears", "All Ears");
        AddEthnicityVariable("Celtic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Celtic", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Dravidian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Dravidian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Dravidian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Dravidian", "Ears", "All Ears");
        AddEthnicityVariable("Dravidian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Dravidian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Dravidian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Dravidian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Dravidian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Dravidian", "Nose", "All Noses");
        AddEthnicityVariable("Dravidian", "Ears", "All Ears");
        AddEthnicityVariable("Dravidian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Dravidian", "Skin Colour", "dark_skin");
        AddEthnicityVariable("Eskimo", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Eskimo", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Eskimo", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Eskimo", "Ears", "All Ears");
        AddEthnicityVariable("Eskimo", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Eskimo", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Eskimo", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Eskimo", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Eskimo", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Eskimo", "Nose", "All Noses");
        AddEthnicityVariable("Eskimo", "Ears", "All Ears");
        AddEthnicityVariable("Eskimo", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Eskimo", "Skin Colour", "redbrown_skin");
        AddEthnicityVariable("Germanic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Germanic", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Germanic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Germanic", "Ears", "All Ears");
        AddEthnicityVariable("Germanic", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Germanic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Germanic", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Germanic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Germanic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Germanic", "Nose", "All Noses");
        AddEthnicityVariable("Germanic", "Ears", "All Ears");
        AddEthnicityVariable("Germanic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Germanic", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Greek", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Greek", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Greek", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Greek", "Ears", "All Ears");
        AddEthnicityVariable("Greek", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Greek", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Greek", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Greek", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Greek", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Greek", "Nose", "All Noses");
        AddEthnicityVariable("Greek", "Ears", "All Ears");
        AddEthnicityVariable("Greek", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Greek", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Han", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Han", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Han", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Han", "Ears", "All Ears");
        AddEthnicityVariable("Han", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Han", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Han", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Han", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Han", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Han", "Nose", "All Noses");
        AddEthnicityVariable("Han", "Ears", "All Ears");
        AddEthnicityVariable("Han", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Han", "Skin Colour", "golden_skin");
        AddEthnicityVariable("Hispanic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Hispanic", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Hispanic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Hispanic", "Ears", "All Ears");
        AddEthnicityVariable("Hispanic", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Hispanic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Hispanic", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Hispanic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Hispanic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Hispanic", "Nose", "All Noses");
        AddEthnicityVariable("Hispanic", "Ears", "All Ears");
        AddEthnicityVariable("Hispanic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Hispanic", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Indo-Aryan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Indo-Aryan", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Indo-Aryan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Indo-Aryan", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Indo-Aryan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Indo-Aryan", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Indo-Aryan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Indo-Aryan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Indo-Aryan", "Nose", "All Noses");
        AddEthnicityVariable("Indo-Aryan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Indo-Aryan", "Skin Colour", "dark_skin");
        AddEthnicityVariable("Italic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Italic", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Italic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Italic", "Ears", "All Ears");
        AddEthnicityVariable("Italic", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Italic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Italic", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Italic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Italic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Italic", "Nose", "All Noses");
        AddEthnicityVariable("Italic", "Ears", "All Ears");
        AddEthnicityVariable("Italic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Italic", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Khoisan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Khoisan", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Khoisan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Khoisan", "Ears", "All Ears");
        AddEthnicityVariable("Khoisan", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Khoisan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Khoisan", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Khoisan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Khoisan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Khoisan", "Nose", "All Noses");
        AddEthnicityVariable("Khoisan", "Ears", "All Ears");
        AddEthnicityVariable("Khoisan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Khoisan", "Skin Colour", "dark_skin");
        AddEthnicityVariable("Korean", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Korean", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Korean", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Korean", "Ears", "All Ears");
        AddEthnicityVariable("Korean", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Korean", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Korean", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Korean", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Korean", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Korean", "Nose", "All Noses");
        AddEthnicityVariable("Korean", "Ears", "All Ears");
        AddEthnicityVariable("Korean", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Korean", "Skin Colour", "golden_skin");
        AddEthnicityVariable("Melanesian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Melanesian", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Melanesian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Melanesian", "Ears", "All Ears");
        AddEthnicityVariable("Melanesian", "Facial Hair Colour", "black_brown_blonde_grey_hair");
        AddEthnicityVariable("Melanesian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Melanesian", "Hair Colour", "black_brown_blonde_grey_hair");
        AddEthnicityVariable("Melanesian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Melanesian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Melanesian", "Nose", "All Noses");
        AddEthnicityVariable("Melanesian", "Ears", "All Ears");
        AddEthnicityVariable("Melanesian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Melanesian", "Skin Colour", "swarthy_skin");
        AddEthnicityVariable("Mesoamerican", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Mesoamerican", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Mesoamerican", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Mesoamerican", "Ears", "All Ears");
        AddEthnicityVariable("Mesoamerican", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Mesoamerican", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Mesoamerican", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Mesoamerican", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Mesoamerican", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Mesoamerican", "Nose", "All Noses");
        AddEthnicityVariable("Mesoamerican", "Ears", "All Ears");
        AddEthnicityVariable("Mesoamerican", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Mesoamerican", "Skin Colour", "redbrown_skin");
        AddEthnicityVariable("Mongolian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Mongolian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Mongolian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Mongolian", "Ears", "All Ears");
        AddEthnicityVariable("Mongolian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Mongolian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Mongolian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Mongolian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Mongolian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Mongolian", "Nose", "All Noses");
        AddEthnicityVariable("Mongolian", "Ears", "All Ears");
        AddEthnicityVariable("Mongolian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Mongolian", "Skin Colour", "golden_skin");
        AddEthnicityVariable("North African", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("North African", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("North African", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("North African", "Ears", "All Ears");
        AddEthnicityVariable("North African", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("North African", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("North African", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("North African", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("North African", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("North African", "Nose", "All Noses");
        AddEthnicityVariable("North African", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("North African", "Skin Colour", "swarthy_skin");
        AddEthnicityVariable("Persian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Persian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Persian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Persian", "Ears", "All Ears");
        AddEthnicityVariable("Persian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Persian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Persian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Persian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Persian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Persian", "Nose", "All Noses");
        AddEthnicityVariable("Persian", "Ears", "All Ears");
        AddEthnicityVariable("Persian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Persian", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Polynesian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Polynesian", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Polynesian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Polynesian", "Ears", "All Ears");
        AddEthnicityVariable("Polynesian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Polynesian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Polynesian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Polynesian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Polynesian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Polynesian", "Nose", "All Noses");
        AddEthnicityVariable("Polynesian", "Ears", "All Ears");
        AddEthnicityVariable("Polynesian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Polynesian", "Skin Colour", "golden_skin");
        AddEthnicityVariable("Scandanavian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Scandanavian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Scandanavian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Scandanavian", "Ears", "All Ears");
        AddEthnicityVariable("Scandanavian", "Facial Hair Colour", "blonde_red_grey_hair");
        AddEthnicityVariable("Scandanavian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Scandanavian", "Hair Colour", "blonde_red_grey_hair");
        AddEthnicityVariable("Scandanavian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Scandanavian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Scandanavian", "Nose", "All Noses");
        AddEthnicityVariable("Scandanavian", "Ears", "All Ears");
        AddEthnicityVariable("Scandanavian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Scandanavian", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Slavic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Slavic", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Slavic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Slavic", "Ears", "All Ears");
        AddEthnicityVariable("Slavic", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Slavic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Slavic", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Slavic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Slavic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Slavic", "Nose", "All Noses");
        AddEthnicityVariable("Slavic", "Ears", "All Ears");
        AddEthnicityVariable("Slavic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Slavic", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Swahili", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Swahili", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Swahili", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Swahili", "Ears", "All Ears");
        AddEthnicityVariable("Swahili", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Swahili", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Swahili", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Swahili", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Swahili", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Swahili", "Nose", "All Noses");
        AddEthnicityVariable("Swahili", "Ears", "All Ears");
        AddEthnicityVariable("Swahili", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Swahili", "Skin Colour", "dark_skin");
        AddEthnicityVariable("Tai-Kadai", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Tai-Kadai", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Tai-Kadai", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Tai-Kadai", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Tai-Kadai", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Tai-Kadai", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Tai-Kadai", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Tai-Kadai", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Tai-Kadai", "Nose", "All Noses");
        AddEthnicityVariable("Tai-Kadai", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Tai-Kadai", "Skin Colour", "golden_skin");
        AddEthnicityVariable("Tibetan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Tibetan", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Tibetan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Tibetan", "Ears", "All Ears");
        AddEthnicityVariable("Tibetan", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Tibetan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Tibetan", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Tibetan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Tibetan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Tibetan", "Nose", "All Noses");
        AddEthnicityVariable("Tibetan", "Ears", "All Ears");
        AddEthnicityVariable("Tibetan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Tibetan", "Skin Colour", "golden_skin");
        AddEthnicityVariable("Turkish", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Turkish", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Turkish", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Turkish", "Ears", "All Ears");
        AddEthnicityVariable("Turkish", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Turkish", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Turkish", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Turkish", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Turkish", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Turkish", "Nose", "All Noses");
        AddEthnicityVariable("Turkish", "Ears", "All Ears");
        AddEthnicityVariable("Turkish", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Turkish", "Skin Colour", "olive_skin");
        AddEthnicityVariable("Uyghur", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Uyghur", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Uyghur", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Uyghur", "Ears", "All Ears");
        AddEthnicityVariable("Uyghur", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Uyghur", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Uyghur", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Uyghur", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Uyghur", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Uyghur", "Nose", "All Noses");
        AddEthnicityVariable("Uyghur", "Ears", "All Ears");
        AddEthnicityVariable("Uyghur", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Uyghur", "Skin Colour", "golden_skin");
        AddEthnicityVariable("Yamato", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Yamato", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Yamato", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Yamato", "Ears", "All Ears");
        AddEthnicityVariable("Yamato", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Yamato", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Yamato", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Yamato", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Yamato", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Yamato", "Nose", "All Noses");
        AddEthnicityVariable("Yamato", "Ears", "All Ears");
        AddEthnicityVariable("Yamato", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Yamato", "Skin Colour", "golden_skin");
        _context.SaveChanges();

        AddCulture("Western Upper Class", "Western European",
            "The upper class are those with sufficient means to support themselves and their lifestyle without the need to hold employment. They may have investments, money-making assets or simply enormous cash reserves. The upper class are typically the cultural and political elite.");
        AddCulture("Western Middle Class", "Western European",
            "The middle class are typically well educated and relatively affluent, often making up the bulk of professional workers such as doctors, lawyers, engineers and scientists. They usually have some savings or non-liquid assets, and may be politically active.");
        AddCulture("Western Working Class", "Western European",
            "The working class are those who work for a living in the cities of the world. They are factory workers, construction workers, utility workers and service workers. Their livelihoods are often insecure because they depend on their jobs and have little savings or assets.");
        AddCulture("Western Rural Poor", "Western European",
            "The rural poor are often amongst the poorest people in society, but they have some advantages that the urban poor (or working class) do not. They often own their land and homes, meagre though they might be, and they often have access to supplementary off-market commodities such as food, goods and basic services from their fellow rural poor.");

        AddCulture("Soviet Upper Class", "Eastern European",
            "The upper class are those with sufficient means to support themselves and their lifestyle without the need to hold employment. They may have investments, money-making assets or simply enormous cash reserves. The upper class are typically the cultural and political elite.");
        AddCulture("Soviet Middle Class", "Eastern European",
            "The middle class are typically well educated and relatively affluent, often making up the bulk of professional workers such as doctors, lawyers, engineers and scientists. They usually have some savings or non-liquid assets, and may be politically active.");
        AddCulture("Soviet Working Class", "Eastern European",
            "The working class are those who work for a living in the cities of the world. They are factory workers, construction workers, utility workers and service workers. Their livelihoods are often insecure because they depend on their jobs and have little savings or assets.");
        AddCulture("Soviet Peasant Class", "Eastern European",
            "The rural poor are often amongst the poorest people in society, but they have some advantages that the urban poor (or working class) do not. They often own their land and homes, meagre though they might be, and they often have access to supplementary off-market commodities such as food, goods and basic services from their fellow rural poor.");

        AddCulture("East Asian Upper Class", "Given and Family",
            "The upper class are those with sufficient means to support themselves and their lifestyle without the need to hold employment. They may have investments, money-making assets or simply enormous cash reserves. The upper class are typically the cultural and political elite.");
        AddCulture("East Asian Middle Class", "Given and Family",
            "The middle class are typically well educated and relatively affluent, often making up the bulk of professional workers such as doctors, lawyers, engineers and scientists. They usually have some savings or non-liquid assets, and may be politically active.");
        AddCulture("East Asian Working Class", "Given and Family",
            "The working class are those who work for a living in the cities of the world. They are factory workers, construction workers, utility workers and service workers. Their livelihoods are often insecure because they depend on their jobs and have little savings or assets.");
        AddCulture("East Asian Rural Poor", "Given and Family",
            "The rural poor are often amongst the poorest people in society, but they have some advantages that the urban poor (or working class) do not. They often own their land and homes, meagre though they might be, and they often have access to supplementary off-market commodities such as food, goods and basic services from their fellow rural poor.");

        AddCulture("Third World Upper Class", "Given and Family",
            "The upper class are those with sufficient means to support themselves and their lifestyle without the need to hold employment. They may have investments, money-making assets or simply enormous cash reserves. The upper class are typically the cultural and political elite.");
        AddCulture("Third World Middle Class", "Given and Family",
            "The middle class are typically well educated and relatively affluent, often making up the bulk of professional workers such as doctors, lawyers, engineers and scientists. They usually have some savings or non-liquid assets, and may be politically active.");
        AddCulture("Third World Working Class", "Given and Family",
            "The working class are those who work for a living in the cities of the world. They are factory workers, construction workers, utility workers and service workers. Their livelihoods are often insecure because they depend on their jobs and have little savings or assets.");
        AddCulture("Third World Rural Poor", "Given and Family",
            "The rural poor are often amongst the poorest people in society, but they have some advantages that the urban poor (or working class) do not. They often own their land and homes, meagre though they might be, and they often have access to supplementary off-market commodities such as food, goods and basic services from their fellow rural poor.");

        ApplyModernEthnicityNameCultureMappings();
        SeedModernCultureCoverage();
    }

	private void SeedModernCultureCoverage()
	{
		SeedHistoricalCultures(ModernBroadCultures);
	}

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
}
