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
    public void SeedMedievalHeritage()
    {
        AddEthnicity(_humanRace, "German", "Germanic", "O-A High Negative", 0, 0,
            description:
            "The German peoples are a people found in many parts of Central Europe. They are typically characteristised by fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "Austrian", "Germanic", "O-A High Negative", 0, 0,
            description:
            "The Austrian peoples are a Germanic people found all over the Holy Roman Empire, starting to emerge as a distinct identity from other Germans. They are typically characteristised by fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "Dutch", "Germanic", "O-A High Negative", 0, 0,
            description:
            "The Dutch peoples are a people found in the low countries of the northern coast of Central Europe. They are typically characteristised by fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "French", "Germanic", "O-A High Negative", 0, 0,
            description:
            "The French peoples are a people found in many areas of France and surrounding Central and Western Europe. They are typically characteristised by fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "Occitan", "Germanic", "O-A High Negative", 0, 0,
            description:
            "The Occitan peoples are a people found in the Southern parts of the Kingdom of France. They are typically characteristised by fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "English", "Germanic", "Majority O Minor A", 0, 0,
            description:
            "The English peoples are the majority population of the southern and central parts of Britain. They are typically characteristised by fair skin, fair hair and light eyes.");

        AddEthnicity(_humanRace, "Venetian", "Italic", "O-A High Negative", 0, 0,
            description:
            "The Venetian peoples are a group of Italian peoples from Northeast of the Italian peninsula. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Florentine", "Italic", "O-A High Negative", 0, 0,
            description:
            "The Florentine peoples are a group of Italian peoples from North of the Italian peninsula. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Neapolitan", "Italic", "O-A High Negative", 0, 0,
            description:
            "The Neapolitan peoples are a group of Italian peoples from south of the Italian peninsula. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Milanese", "Italic", "O-A High Negative", 0, 0,
            description:
            "The Milanese (or sometimes Lombard) peoples are a group of Italian peoples from Northwest of the Italian peninsula. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Sicilian", "Italic", "O-A High Negative", 0, 0,
            description:
            "The Sicilian peoples are a group of Italian peoples from the island of Sicily. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Corsican", "Italic", "O-A High Negative", 0, 0,
            description:
            "The Corsican peoples are a group of Italian peoples from the island of Corsica. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Sardinian", "Italic", "O-A High Negative", 0, 0,
            description:
            "The SiciSardinianlian peoples are a group of Italian peoples from the island of Sardinia. They are typically characterised by olive skin, dark hair and dark eyes.");

        AddEthnicity(_humanRace, "Castilian", "Iberian", "O-A High Negative", 0, 0,
            description:
            "The Castilian peoples are an Iberian people from the Kingdom of Castille in central Iberia. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Catalan", "Iberian", "O-A High Negative", 0, 0,
            description:
            "The Catalan peoples are an Iberian people from the Kingdom of Aragon in eastern Iberia. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Galicians", "Iberian", "O-A High Negative", 0, 0,
            description:
            "The Galicians peoples are an Iberian people from the Kingdom of Galicia, in northwest Iberia. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Portugese", "Iberian", "O-A High Negative", 0, 0,
            description:
            "The Portugese peoples are an Iberian people from the Kingdom of Portugal, in western Iberia. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Basque", "Iberian", "O-A High Negative", 0, 0,
            description:
            "The Basque peoples are a unique ethno-linguistic group in northern Iberia and southern France. They are typically characterised by olive skin, dark hair and dark eyes.");


        AddEthnicity(_humanRace, "Ashkenazi Jewish", "Jewish", "O-A High Negative", 0, 0,
            description:
            "The Ashkenazi Jewish peoples are a diaspora of Jewish people into Europe, especially Central Europe. They are typically characterised by light or olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Mizrahi Jewish", "Jewish", "O-A High Negative", 0, 0,
            description:
            "The Mizrahi Jewish peoples are a diaspora of Jewish people into Persia, Mesopotamia and the Levant. They are typically characterised by light or olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Sephardic Jewish", "Jewish", "O-A High Negative", 0, 0,
            description:
            "The Sephardic Jewish peoples are a diaspora of Jewish people into Iberia and Portugal. They are typically characterised by light or olive skin, dark hair and dark eyes.");

        AddEthnicity(_humanRace, "Gaelic", "Celtic", "Majority O Minor A", 0, 0,
            description:
            "The Gaelic peoples are a Celtic people found in Scotland and Ireland. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Welsh", "Celtic", "Majority O Minor A", 0, 0,
            description:
            "The Welsh peoples are a Celtic people found in Wales. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Breton", "Celtic", "Majority O Minor A", 0, 0,
            description:
            "The Breton peoples are a Celtic people found in Northwestern France. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");


        AddEthnicity(_humanRace, "Polish", "Slavic", "O-A High Negative", 0, 0,
            description:
            "The Polish peoples are a Slavic people found in Central and Eastern Europe. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Czech", "Slavic", "O-A High Negative", 0, 0,
            description:
            "The Czech peoples are a the people of Bohemia and many surrounding areas. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Slovak", "Slavic", "O-A High Negative", 0, 0,
            description:
            "The Slovak peoples are a Slavic people found mostly in Moravia, under the rule of Hungary. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Ruthenian", "Slavic", "O-A High Negative", 0, 0,
            description:
            "The Ruthenian peoples are a Slavic people of Eastern Europe, in the Grand Duchy of Lithuania. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Ukrainian", "Slavic", "O-A High Negative", 0, 0,
            description:
            "The Ukrainian peoples are a Slavic people of Eastern Europe, primarily in the Grand Duchy of Lithuania. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Russian", "Slavic", "O-A High Negative", 0, 0,
            description:
            "The Russian peoples are a Slavic people of Eastern Europe, primarily in the Grand Duchy of Moscow. They are typically characterised by fair skin, fair hair and either light or dark eyes.");




        AddEthnicity(_humanRace, "Croat", "Slavic", "O-A High Negative", 0, 0,
            description:
            "The Croat peoples are a Slavic people of Eastern Europe, subjects of the Hungarian Empire. They are typically characterised by fair skin, dark hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Serb", "Slavic", "O-A High Negative", 0, 0,
            description:
            "The Serb peoples are a Slavic people of Eastern Europe, subjects of the Hungarian Empire. They are typically characterised by fair skin, dark hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Bosniak", "Slavic", "O-A High Negative", 0, 0,
            description:
            "The Bosniak peoples are a Slavic people of Eastern Europe, closely related to Serbians and characterised by their Muslim faith. They are subjects of the Hungarian Empire. They are typically characterised by fair skin, dark hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Vlach", "Slavic", "O-A High Negative", 0, 0,
            description:
            "The Vlach peoples are a Slavic people of Eastern Europe, largely subjects of the Ottoman Empire. They are typically characterised by fair skin, dark hair and either light or dark eyes.");


        AddEthnicity(_humanRace, "Lithuanian", "Baltic", "O-A High Negative", 0, 0,
            description:
            "The Lithuanian peoples are a Baltic people of Eastern Europe. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Estonian", "Baltic", "O-A High Negative", 0, 0,
            description:
            "The Estonian peoples are a Baltic people of Eastern Europe. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Latvian", "Baltic", "O-A High Negative", 0, 0,
            description:
            "The Latvian peoples are a Baltic people of Eastern Europe. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Prussian", "Baltic", "O-A High Negative", 0, 0,
            description:
            "The Prussian peoples are a Baltic people of Eastern Europe, subjects of the Teutonic Order. They are typically characterised by fair skin, fair hair and either light or dark eyes.");


        AddEthnicity(_humanRace, "Hungarian", "Magyar", "O-A High Negative", 0, 0,
            description:
            "The Hungarian (or Magyar) peoples are a people who live in Hungary, allegedly descended from Atilla the Hun. They are typically characterised by fair skin, dark hair and either light or dark eyes.");




        AddEthnicity(_humanRace, "Norse", "Nordic", "A Dominant", 0, 0,
            description:
            "The Norse peoples are the Nordic peoples of Norway in Scandanavia. They are typically characterised by very fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "Danish", "Nordic", "A Dominant", 0, 0,
            description:
            "The Danish peoples are the Nordic peoples of Denmark in Northern Central Europe. They are typically characterised by very fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "Swedish", "Nordic", "A Dominant", 0, 0,
            description:
            "The Swedish peoples are the Nordic peoples of Sweden, in Scandanavia. They are typically characterised by very fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "Icelandic", "Nordic", "A Dominant", 0, 0,
            description:
            "The Icelandic peoples are the Nordic peoples of Iceland, in the North Atlantic Sea. They are typically characterised by very fair skin, fair hair and light eyes.");




        AddEthnicity(_humanRace, "Roman", "Greek", "O-A High Negative", 0, 0,
            description:
            "The Roman peoples are a grouping of many different peoples, ethnic Christians who lived under the Eastern Roman rule. Most of the original ethnic group distinctions were lost and people simply identified themselves as Christian Romans. They are typically characterised by fair to olive skin, dark hair and dark eyes.");


        AddEthnicity(_humanRace, "Ottoman", "Turkic", "A Dominant", 0, 0,
            description:
            "The Ottoman peoples are a turkic people who live in the Ottoman Empire. They are typically characterised by dark olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Cossack", "Turkic", "A Dominant", 0, 0,
            description:
            "The Cossack peoples are a turkic people who live in the Pontian-Caspian Steppes. They are typically characterised by dark olive skin, dark hair and dark eyes.");



        AddEthnicity(_humanRace, "Arabic", "Middle Eastern", "Majority O Minor A", 0, 0,
            description:
            "The Arabic peoples are an Indo-European ethno-linguistic group found predominantly in the Arabian Peninsula, North Africa and the Horn of Africa. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Persian", "Middle Eastern", "Majority O Minor A", 0, 0,
            description:
            "The Persian peoples are an Indo-European ethno-linguistic group found in Central Asia and the Middle East. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Moorish", "African", "Majority O", 0, 0,
            description:
            "The Moorish peoples are a fusion of Afro-Asiatic, Hispanic and Arabic ethno-linguistic group found in Southern Iberia. They are characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "North African", "African", "Majority O", 0, 0,
            description:
            "The North African peoples are an Afro-Asiatic ethno-linguistic group found in North Africa. They are characterised by olive or light brown skin, dark hair and dark eyes.");


        #region Features
        // Distinctive Feature

        AddEthnicityVariable("German", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Dutch", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("French", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Occitan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("English", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Venetian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Florentine", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Neapolitan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Milanese", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Sicilian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Corsican", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Sardinian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Castilian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Catalan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Galicians", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Portugese", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Basque", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Ashkenazi Jewish", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Mizrahi Jewish", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Sephardic Jewish", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Gaelic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Welsh", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Breton", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Polish", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Czech", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Slovak", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Ruthenian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Ukrainian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Russian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Croat", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Serb", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Bosniak", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Vlach", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Lithuanian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Estonian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Latvian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Prussian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Hungarian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Norse", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Danish", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Swedish", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Icelandic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Roman", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Ottoman", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Cossack", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Arabic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Persian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Moorish", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("North African", "Distinctive Feature", "All Distinctive Features");

        // Eye Shape

        AddEthnicityVariable("German", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Dutch", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("French", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Occitan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("English", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Venetian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Florentine", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Neapolitan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Milanese", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Sicilian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Corsican", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Sardinian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Castilian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Catalan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Galicians", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Portugese", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Basque", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Ashkenazi Jewish", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Mizrahi Jewish", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Sephardic Jewish", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Gaelic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Welsh", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Breton", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Polish", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Czech", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Slovak", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Ruthenian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Ukrainian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Russian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Croat", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Serb", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Bosniak", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Vlach", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Lithuanian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Estonian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Latvian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Prussian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Hungarian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Norse", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Danish", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Swedish", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Icelandic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Roman", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Ottoman", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Cossack", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Arabic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Persian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Moorish", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("North African", "Eye Shape", "All Eye Shapes");

        // Hair Styles

        AddEthnicityVariable("German", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Dutch", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("French", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Occitan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("English", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Venetian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Florentine", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Neapolitan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Milanese", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Sicilian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Corsican", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Sardinian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Castilian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Catalan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Galicians", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Portugese", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Basque", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Ashkenazi Jewish", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Mizrahi Jewish", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Sephardic Jewish", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Gaelic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Welsh", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Breton", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Polish", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Czech", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Slovak", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Ruthenian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Ukrainian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Russian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Croat", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Serb", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Bosniak", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Vlach", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Lithuanian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Estonian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Latvian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Prussian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Hungarian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Norse", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Danish", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Swedish", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Icelandic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Roman", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Ottoman", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Cossack", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Arabic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Persian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Moorish", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("North African", "Hair Style", "All Hair Styles");

        // Facial Hair Styles

        AddEthnicityVariable("German", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Dutch", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("French", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Occitan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("English", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Venetian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Florentine", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Neapolitan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Milanese", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Sicilian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Corsican", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Sardinian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Castilian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Catalan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Galicians", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Portugese", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Basque", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Ashkenazi Jewish", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Mizrahi Jewish", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Sephardic Jewish", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Gaelic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Welsh", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Breton", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Polish", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Czech", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Slovak", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Ruthenian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Ukrainian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Russian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Croat", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Serb", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Bosniak", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Vlach", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Lithuanian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Estonian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Latvian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Prussian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Hungarian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Norse", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Danish", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Swedish", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Icelandic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Roman", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Ottoman", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Cossack", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Arabic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Persian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Moorish", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("North African", "Facial Hair Style", "All Facial Hair Styles");

        // Frames

        AddEthnicityVariable("German", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Dutch", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("French", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Occitan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("English", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Venetian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Florentine", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Neapolitan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Milanese", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Sicilian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Corsican", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Sardinian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Castilian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Catalan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Galicians", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Portugese", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Basque", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Ashkenazi Jewish", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Mizrahi Jewish", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Sephardic Jewish", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Gaelic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Welsh", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Breton", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Polish", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Czech", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Slovak", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Ruthenian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Ukrainian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Russian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Croat", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Serb", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Bosniak", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Vlach", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Lithuanian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Estonian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Latvian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Prussian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Hungarian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Norse", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Danish", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Swedish", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Icelandic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Roman", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Ottoman", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Cossack", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Arabic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Persian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Moorish", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("North African", "Humanoid Frame", "All Frames");

        // Noses

        AddEthnicityVariable("German", "Nose", "All Noses");
        AddEthnicityVariable("Dutch", "Nose", "All Noses");
        AddEthnicityVariable("French", "Nose", "All Noses");
        AddEthnicityVariable("Occitan", "Nose", "All Noses");
        AddEthnicityVariable("English", "Nose", "All Noses");
        AddEthnicityVariable("Venetian", "Nose", "All Noses");
        AddEthnicityVariable("Florentine", "Nose", "All Noses");
        AddEthnicityVariable("Neapolitan", "Nose", "All Noses");
        AddEthnicityVariable("Milanese", "Nose", "All Noses");
        AddEthnicityVariable("Sicilian", "Nose", "All Noses");
        AddEthnicityVariable("Corsican", "Nose", "All Noses");
        AddEthnicityVariable("Sardinian", "Nose", "All Noses");
        AddEthnicityVariable("Castilian", "Nose", "All Noses");
        AddEthnicityVariable("Catalan", "Nose", "All Noses");
        AddEthnicityVariable("Galicians", "Nose", "All Noses");
        AddEthnicityVariable("Portugese", "Nose", "All Noses");
        AddEthnicityVariable("Basque", "Nose", "All Noses");
        AddEthnicityVariable("Ashkenazi Jewish", "Nose", "All Noses");
        AddEthnicityVariable("Mizrahi Jewish", "Nose", "All Noses");
        AddEthnicityVariable("Sephardic Jewish", "Nose", "All Noses");
        AddEthnicityVariable("Gaelic", "Nose", "All Noses");
        AddEthnicityVariable("Welsh", "Nose", "All Noses");
        AddEthnicityVariable("Breton", "Nose", "All Noses");
        AddEthnicityVariable("Polish", "Nose", "All Noses");
        AddEthnicityVariable("Czech", "Nose", "All Noses");
        AddEthnicityVariable("Slovak", "Nose", "All Noses");
        AddEthnicityVariable("Ruthenian", "Nose", "All Noses");
        AddEthnicityVariable("Ukrainian", "Nose", "All Noses");
        AddEthnicityVariable("Russian", "Nose", "All Noses");
        AddEthnicityVariable("Croat", "Nose", "All Noses");
        AddEthnicityVariable("Serb", "Nose", "All Noses");
        AddEthnicityVariable("Bosniak", "Nose", "All Noses");
        AddEthnicityVariable("Vlach", "Nose", "All Noses");
        AddEthnicityVariable("Lithuanian", "Nose", "All Noses");
        AddEthnicityVariable("Estonian", "Nose", "All Noses");
        AddEthnicityVariable("Latvian", "Nose", "All Noses");
        AddEthnicityVariable("Prussian", "Nose", "All Noses");
        AddEthnicityVariable("Hungarian", "Nose", "All Noses");
        AddEthnicityVariable("Norse", "Nose", "All Noses");
        AddEthnicityVariable("Danish", "Nose", "All Noses");
        AddEthnicityVariable("Swedish", "Nose", "All Noses");
        AddEthnicityVariable("Icelandic", "Nose", "All Noses");
        AddEthnicityVariable("Roman", "Nose", "All Noses");
        AddEthnicityVariable("Ottoman", "Nose", "All Noses");
        AddEthnicityVariable("Cossack", "Nose", "All Noses");
        AddEthnicityVariable("Arabic", "Nose", "All Noses");
        AddEthnicityVariable("Persian", "Nose", "All Noses");
        AddEthnicityVariable("Moorish", "Nose", "All Noses");
        AddEthnicityVariable("North African", "Nose", "All Noses");

        // Person Word

        AddEthnicityVariable("German", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Dutch", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("French", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Occitan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("English", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Venetian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Florentine", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Neapolitan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Milanese", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Sicilian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Corsican", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Sardinian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Castilian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Catalan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Galicians", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Portugese", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Basque", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Ashkenazi Jewish", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Mizrahi Jewish", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Sephardic Jewish", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Gaelic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Welsh", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Breton", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Polish", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Czech", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Slovak", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Ruthenian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Ukrainian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Russian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Croat", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Serb", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Bosniak", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Vlach", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Lithuanian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Estonian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Latvian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Prussian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Hungarian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Norse", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Danish", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Swedish", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Icelandic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Roman", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Ottoman", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Cossack", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Arabic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Persian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Moorish", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("North African", "Person Word", "Weighted Person Words");

        // Hair Colour

        AddEthnicityVariable("German", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Dutch", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("French", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Occitan", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("English", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Venetian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Florentine", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Neapolitan", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Milanese", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sicilian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Corsican", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sardinian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Castilian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Catalan", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Galicians", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Portugese", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Basque", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Ashkenazi Jewish", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Mizrahi Jewish", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sephardic Jewish", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Gaelic", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Welsh", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Breton", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Polish", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Czech", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Slovak", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Ruthenian", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Ukrainian", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Russian", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Croat", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Serb", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Bosniak", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Vlach", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Lithuanian", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Estonian", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Latvian", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Prussian", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Hungarian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Norse", "Hair Colour", "blonde_red_grey_hair");
        AddEthnicityVariable("Danish", "Hair Colour", "blonde_red_grey_hair");
        AddEthnicityVariable("Swedish", "Hair Colour", "blonde_red_grey_hair");
        AddEthnicityVariable("Icelandic", "Hair Colour", "blonde_red_grey_hair");
        AddEthnicityVariable("Roman", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Ottoman", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Cossack", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Arabic", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Persian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Moorish", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("North African", "Hair Colour", "black_brown_grey_hair");

        // Facial Hair Colour

        AddEthnicityVariable("German", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Dutch", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("French", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Occitan", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("English", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Venetian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Florentine", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Neapolitan", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Milanese", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sicilian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Corsican", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sardinian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Castilian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Catalan", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Galicians", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Portugese", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Basque", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Ashkenazi Jewish", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Mizrahi Jewish", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sephardic Jewish", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Gaelic", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Welsh", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Breton", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Polish", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Czech", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Slovak", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Ruthenian", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Ukrainian", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Russian", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Croat", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Serb", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Bosniak", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Vlach", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Lithuanian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Estonian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Latvian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Prussian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Hungarian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Norse", "Facial Hair Colour", "blonde_red_grey_hair");
        AddEthnicityVariable("Danish", "Facial Hair Colour", "blonde_red_grey_hair");
        AddEthnicityVariable("Swedish", "Facial Hair Colour", "blonde_red_grey_hair");
        AddEthnicityVariable("Icelandic", "Facial Hair Colour", "blonde_red_grey_hair");
        AddEthnicityVariable("Roman", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Ottoman", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Cossack", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Arabic", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Persian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Moorish", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("North African", "Facial Hair Colour", "black_brown_grey_hair");

        // Skin Colours

        AddEthnicityVariable("German", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Dutch", "Skin Colour", "fair_skin");
        AddEthnicityVariable("French", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Occitan", "Skin Colour", "fair_skin");
        AddEthnicityVariable("English", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Venetian", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Florentine", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Neapolitan", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Milanese", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Sicilian", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Corsican", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Sardinian", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Castilian", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Catalan", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Galicians", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Portugese", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Basque", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Ashkenazi Jewish", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Mizrahi Jewish", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Sephardic Jewish", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Gaelic", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Welsh", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Breton", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Polish", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Czech", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Slovak", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Ruthenian", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Ukrainian", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Russian", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Croat", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Serb", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Bosniak", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Vlach", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Lithuanian", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Estonian", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Latvian", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Prussian", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Hungarian", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Norse", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Danish", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Swedish", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Icelandic", "Skin Colour", "fair_skin");
        AddEthnicityVariable("Roman", "Skin Colour", "olive_skin");
        AddEthnicityVariable("Ottoman", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Cossack", "Skin Colour", "fair_olive_skin");
        AddEthnicityVariable("Arabic", "Skin Colour", "swarthy_skin");
        AddEthnicityVariable("Persian", "Skin Colour", "olive_skin");
        AddEthnicityVariable("Moorish", "Skin Colour", "swarthy_skin");
        AddEthnicityVariable("North African", "Skin Colour", "swarthy_skin");

        // Eye Colour

        AddEthnicityVariable("German", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Dutch", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("French", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Occitan", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("English", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Venetian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Florentine", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Neapolitan", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Milanese", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Sicilian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Corsican", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Sardinian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Castilian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Catalan", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Galicians", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Portugese", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Basque", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Ashkenazi Jewish", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Mizrahi Jewish", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Sephardic Jewish", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Gaelic", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Welsh", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Breton", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Polish", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Czech", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Slovak", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Ruthenian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Ukrainian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Russian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Croat", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Serb", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Bosniak", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Vlach", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Lithuanian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Estonian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Latvian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Prussian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Hungarian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Norse", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Danish", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Swedish", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Icelandic", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Roman", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Ottoman", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Cossack", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Arabic", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Persian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Moorish", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("North African", "Eye Colour", "brown_blue_eyes");
        #endregion
        ApplyMedievalEuropeEthnicityNameCultureMappings();
        SeedRenaissanceEuropeCultureCoverage();
        _context.SaveChanges();
    }

	private void SeedRenaissanceEuropeCultureCoverage()
	{
		SeedHistoricalEthnicityVariants(RenaissanceEuropeEthnicityVariants);
		SeedHistoricalCultures(RenaissanceEuropeBroadCultures);
	}

	private static readonly HistoricalEthnicityVariantSeed[] RenaissanceEuropeEthnicityVariants =
	[
		new("Albanian", "Vlach", "Albanian", "Balkan", "Albanian-Speaking Communities",
			"Albanians belong to northern and southern Albanian-speaking clans, towns and lordships across the Adriatic and Balkan borderlands.")
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
