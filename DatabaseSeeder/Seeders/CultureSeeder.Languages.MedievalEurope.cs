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
    private void SeedMedievalEuropeLanguages()
    {
        #region Languages
        AddLanguage("Latin", "an unknown romance language");
        AddLanguage("French", "an unknown romance language");
        AddLanguage("Occitan", "an unknown romance language");
        AddLanguage("Italian", "an unknown romance language");
        AddLanguage("Lombard", "an unknown romance language");
        AddLanguage("Venetian", "an unknown romance language");
        AddLanguage("Neapolitan", "an unknown romance language");
        AddLanguage("Sicilian", "an unknown romance language");
        AddLanguage("Castilian", "an unknown romance language");
        AddLanguage("Aragonese", "an unknown romance language");
        AddLanguage("Leonese", "an unknown romance language");
        AddLanguage("Galician", "an unknown romance language");
        AddLanguage("Wallachian", "an unknown romance language");

        AddLanguage("English", "an unknown germanic language");
        AddLanguage("High German", "an unknown germanic language");
        AddLanguage("Low German", "an unknown germanic language");
        AddLanguage("Dutch", "an unknown germanic language");
        AddLanguage("Frisian", "an unknown germanic language");
        AddLanguage("Yiddish", "an unknown germanic language");
        AddLanguage("Swedish", "an unknown nordic language");
        AddLanguage("Norwegian", "an unknown germanic language");
        AddLanguage("Icelandic", "an unknown germanic language");
        AddLanguage("Danish", "an unknown germanic language");

        AddLanguage("Russian", "an unknown slavic language");
        AddLanguage("Ruthenian", "an unknown slavic language");
        AddLanguage("Church Slavonic", "an unknown slavic language");
        AddLanguage("Polish", "an unknown slavic language");
        AddLanguage("Slovak", "an unknown slavic language");
        AddLanguage("Wendish", "an unknown slavic language");
        AddLanguage("Czech", "an unknown slavic language");
        AddLanguage("Prussian", "an unknown slavic language");
        AddLanguage("Lithuanian", "an unknown slavic language");
        AddLanguage("Latvian", "an unknown slavic language");
        AddLanguage("Serbo-Croatian", "an unknown slavic language");
        AddLanguage("Slovenian", "an unknown slavic language");
        AddLanguage("Bulgarian", "an unknown slavic language");

        AddLanguage("Quranic Arabic", "an unknown eastern language");
        AddLanguage("Mahgrebi Arabic", "an unknown eastern language");
        AddLanguage("Mashriqi Arabic", "an unknown eastern language");
        AddLanguage("Aramaic", "an unknown eastern language");
        AddLanguage("Berber", "an unknown eastern language");
        AddLanguage("Hebrew", "an unknown eastern language");
        AddLanguage("Greek", "an unknown eastern language");
        AddLanguage("Koine Greek", "an unknown eastern language");

        AddLanguage("Persian", "an unknown eastern language");
        AddLanguage("Armenian", "an unknown eastern language");
        AddLanguage("Coptic", "an unknown eastern language");

        AddLanguage("Irish Gaelic", "an unknown celtic language");
        AddLanguage("Scottish Gaelic", "an unknown celtic language");
        AddLanguage("Welsh", "an unknown celtic language");
        AddLanguage("Manx", "an unknown celtic language");
        AddLanguage("Breton", "an unknown celtic language");
        AddLanguage("Cornish", "an unknown celtic language");

        AddLanguage("Turkish", "an unknown eastern language");

        AddLanguage("Finnish", "an unknown uralic language");
        AddLanguage("Hungarian", "an unknown uralic language");
        AddLanguage("Estonian", "an unknown uralic language");
        AddLanguage("Karelian", "an unknown uralic language");

        AddLanguage("Romani", "an unknown eastern language");
        AddLanguage("Basque", "an unknown european language");

        #endregion

        #region Scripts

        AddScript("Latin", "the Latin script", "a European script",
            "The latin script is based on the script used by Roman speakers of the Latin language, and widely used across Europe. There are small variations between languages to show sounds unique to that language but it is generally very similar across languages.",
            "Alphabet", 1.0, 1.0,
            "Latin",
            "French",
            "Occitan",
            "Italian",
            "Lombard",
            "Venetian",
            "Neapolitan",
            "Sicilian",
            "Castilian",
            "Aragonese",
            "Leonese",
            "Galician",
            "English",
            "High German",
            "Low German",
            "Dutch",
            "Frisian",
            "Polish",
            "Slovak",
            "Wendish",
            "Czech",
            "Prussian",
            "Lithuanian",
            "Latvian",
            "Irish Gaelic",
            "Scottish Gaelic",
            "Manx",
            "Breton",
            "Cornish",
            "Finnish",
            "Hungarian",
            "Estonian",
            "Karelian",
            "Romani",
            "Basque",
            "Welsh",
            "Danish",
            "Norwegian",
            "Icelandic",
            "Swedish");

        AddScript("Cyrillic", "the Cyrillic script", "a European script",
            "The Cyrillic script is based on the Old Church Slavonic script and used by a variety of Slavic languages. There are small variations between languages to show sounds unique to that language but it is generally very similar across languages.",
            "Alphabet", 1.0, 1.0,
            "Russian",
            "Ruthenian",
            "Church Slavonic",
            "Bulgarian",
            "Serbo-Croatian",
            "Wallachian");

        AddScript("Greek", "the Greek script", "a European script",
            "The Greek script was once widely used as a lingua-franca but is nowadays mostly limited to the use of the Greek language and the classics.",
            "Alphabet", 1.0, 1.0,
            "Greek",
            "Koine Greek"
            );

        AddScript("Glagolitic", "the Glagolitic script", "a European script",
            "The Glagolitic script is a mostly extinct Slavic script, originally used to write Old Church Slavonic. It is still known in some Orthodox religious circles.",
            "Alphabet", 1.0, 1.0,
            "Church Slavonic");


        AddScript("Hebrew", "the Hebrew script", "an Eastern script",
            "The Hebrew script is used to write the Hebrew liturgic language and the Yiddish language, and is a right-to-left abjad.",
            "Abjad", 0.5, 1.0,
            "Hebrew",
            "Yiddish");

        AddScript("Arabic", "the Arabic script", "an Eastern script",
            "The Arabic script is used to write the various dialects of Arabic, and is a right-to-left abjad.",
            "Abjad", 0.5, 1.0,
            "Quranic Arabic",
            "Mahgrebi Arabic",
            "Mashriqi Arabic",
            "Aramaic",
            "Persian",
            "Turkish");

        AddScript("Syriac", "the Syriac script", "an Eastern script",
            "The Syriac script is a right to left alphabet used to write the Aramaic or Syriac language.",
            "Syriac", 0.5, 1.0,
            "Aramaic");

        AddScript("Armenian", "the Armenian script", "an Eastern script",
            "The Armenian script is used to write the Armenian language, distinct from other European or Eastern scripts at the time.",
            "Alphabet", 1.0, 1.0,
            "Armenian");

        AddScript("Coptic", "the Coptic script", "a European script",
            "The Coptic script related to but distinct from the Greek script, and is used in Coptic language liturgical texts.",
            "Alphabet", 1.0, 1.0,
            "Coptic");


        #endregion

        #region Accents

        AddAccent("Latin", "liturgical", "in liturgical form", "in liturgical form", Difficulty.Trivial, "The liturgical form of latin based on reconstructed pronounciation of the written form that had been in common use since the classical period", "standard");
        AddAccent("Latin", "classical", "in classical form", "in classical form", Difficulty.Trivial, "The classical form of latin as it was pronounced in the Roman empire", "standard");
        AddAccent("Latin", "neo-classical", "in neo-classical form", "in classical form", Difficulty.Trivial, "The neo-classical form of latin popularised amongst reneissance humanists, based on a reconstructed pronounciation of classical latin", "standard");

        AddAccent("French", "francien", "in the Francien dialect", "in a French dialect", Difficulty.Trivial, "The prestige dialect, spoken in court and used in official business", "french");
        AddAccent("French", "normand", "in the Normand dialect", "in a French dialect", Difficulty.VeryEasy, "The Normand dialect of the French language", "french");
        AddAccent("French", "mainot", "in the Mainiot dialect", "in a French dialect", Difficulty.VeryEasy, "The Mainot dialect of the French language", "french");
        AddAccent("French", "angevin", "in the Angevin dialect", "in a French dialect", Difficulty.VeryEasy, "The Anvegin dialect of the French language", "french");
        AddAccent("French", "gallo", "in the Gallo dialect", "in a French dialect", Difficulty.VeryEasy, "The Gallo dialect of the French language", "french");
        AddAccent("French", "poitevin-saintongeais", "in the Poitevin-Saintongeais dialect", "in a French dialect", Difficulty.VeryEasy, "The Poitevin-Saintongeais dialect of the French language", "french");
        AddAccent("French", "crossaint", "in the Croissant dialect", "in a French dialect", Difficulty.VeryEasy, "The Crossaint dialect of the French language", "french");
        AddAccent("French", "bourguignon", "in the Bourguignon dialect", "in a French dialect", Difficulty.VeryEasy, "The Bourguignon dialect of the French language", "french");
        AddAccent("French", "champenois", "in the Champenois dialect", "in a French dialect", Difficulty.VeryEasy, "The Champenois dialect of the French language", "french");
        AddAccent("French", "picard", "in the Picard dialect", "in a French dialect", Difficulty.VeryEasy, "The Picard dialect of the French language", "french");
        AddAccent("French", "wallon", "in the Wallon dialect", "in a French dialect", Difficulty.VeryEasy, "The Wallon dialect of the French language", "french");
        AddAccent("French", "lorrain", "in the Lorrain dialect", "in a French dialect", Difficulty.VeryEasy, "The Lorrain dialect of the French language", "french");
        AddAccent("French", "franc-comtois", "in the Franc-Comtois dialect", "in a French dialect", Difficulty.VeryEasy, "The Franc-Comtois dialect of the French language", "french");
        AddAccent("French", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the French language", "foreign");
        AddAccent("French", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal, "A German speaker's rendition of the French language", "foreign");
        AddAccent("French", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the French language", "foreign");
        AddAccent("French", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal, "An Italian speaker's rendition of the French language", "foreign");
        AddAccent("French", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the French language", "foreign");
        AddAccent("French", "british", "with a British accent", "in a foreign dialect", Difficulty.Normal, "A British speaker's rendition of the French language", "foreign");
        AddAccent("French", "flemish", "with a Flemish accent", "in a foreign dialect", Difficulty.Normal, "A Flemish speaker's rendition of the French language", "foreign");
        AddAccent("French", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the French language", "foreign");
        AddAccent("French", "arabic", "with an Arabic accent", "in a foreign dialect", Difficulty.Normal, "An Arabic speaker's rendition of the French language", "foreign");
        AddAccent("French", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the French language", "foreign");

        AddAccent("Occitan", "gascon", "in the Gascon dialect", "in an Occitan dialect", Difficulty.VeryEasy, "The dialect of the Occitan language spoken in the Gascony region", "occitan");
        AddAccent("Occitan", "languedocien", "in the Languedocien dialect", "in an Occitan dialect", Difficulty.VeryEasy, "The dialect of the Occitan language spoken in the Languedoc region", "occitan");
        AddAccent("Occitan", "limousin", "in the Limousin dialect", "in an Occitan dialect", Difficulty.VeryEasy, "The dialect of the Occitan language spoken in the Limousin region", "occitan");
        AddAccent("Occitan", "auvergnat", "in the Auvergnat dialect", "in an Occitan dialect", Difficulty.VeryEasy, "The dialect of the Occitan language spoken in the Auvergnat region", "occitan");
        AddAccent("Occitan", "provençal", "in the Provençal dialect", "in an Occitan dialect", Difficulty.VeryEasy, "The dialect of the Occitan language spoken in the Provence region", "occitan");
        AddAccent("Occitan", "vivaro-alpine", "in the Vivaro-Alpine dialect", "in an Occitan dialect", Difficulty.VeryEasy, "The dialect of the Occitan language spoken in the Alpine region", "occitan");
        AddAccent("Occitan", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker's rendition of the Occitan language", "occitan");
        AddAccent("Occitan", "german", "with a German accent", "in a foreign dialect", Difficulty.VeryEasy, "A German speaker's rendition of the Occitan language", "occitan");
        AddAccent("Occitan", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Occitan language", "foreign");
        AddAccent("Occitan", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal, "An Italian speaker's rendition of the Occitan language", "foreign");
        AddAccent("Occitan", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the Occitan language", "foreign");
        AddAccent("Occitan", "british", "with a British accent", "in a foreign dialect", Difficulty.Normal, "A British speaker's rendition of the Occitan language", "foreign");
        AddAccent("Occitan", "flemish", "with a Flemish accent", "in a foreign dialect", Difficulty.Normal, "A Flemish speaker's rendition of the Occitan language", "foreign");
        AddAccent("Occitan", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Occitan language", "foreign");

        AddAccent("Italian", "florentine", "in the Florentine dialect", "in an Italian dialect", Difficulty.Trivial, "The prestige dialect of the Italian language", "italian");
        AddAccent("Italian", "lombard", "with a Lombard accent", "in an Italian dialect", Difficulty.VeryEasy, "A Lombard speaker's rendition of the Florentine Italian language", "italian");
        AddAccent("Italian", "venetian", "with a Venetian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Venetian speaker's rendition of the Florentine Italian language", "italian");
        AddAccent("Italian", "neapolitan", "with a Neapolitan accent", "in an Italian dialect", Difficulty.VeryEasy, "A Neapolitan speaker's rendition of the Florentine Italian language", "italian");
        AddAccent("Italian", "sicilian", "with a Sicilian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Sicilian speaker's rendition of the Florentine Italian language", "italian");
        AddAccent("Italian", "sardinian", "with a Sardinian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Sardinian speaker's rendition of the Florentine Italian language", "italian");
        AddAccent("Italian", "corsican", "with a Corsican accent", "in an Italian dialect", Difficulty.VeryEasy, "A Corsican speaker's rendition of the Florentine Italian language", "italian");
        AddAccent("Italian", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker's rendition of the Florentine Italian language", "occitan");
        AddAccent("Italian", "german", "with a German accent", "in a foreign dialect", Difficulty.VeryEasy, "A German speaker's rendition of the Florentine Italian language", "occitan");
        AddAccent("Italian", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Florentine Italian language", "foreign");
        AddAccent("Italian", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Florentine Italian language", "foreign");
        AddAccent("Italian", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the Florentine Italian language", "foreign");
        AddAccent("Italian", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the Florentine Italian language", "foreign");
        AddAccent("Italian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Florentine Italian language", "foreign");

        AddAccent("Lombard", "lombard", "in the Lombard dialect", "in an Italian dialect", Difficulty.Trivial, "The primary dialect of the Lombard language", "italian");
        AddAccent("Lombard", "florentine", "with a Florentine accent", "in an Italian dialect", Difficulty.VeryEasy, "A Florentine speaker's rendition of the Lombard language", "italian");
        AddAccent("Lombard", "venetian", "with a Venetian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Venetian speaker's rendition of the Lombard language", "italian");
        AddAccent("Lombard", "neapolitan", "with a Neapolitan accent", "in an Italian dialect", Difficulty.VeryEasy, "A Neapolitan speaker's rendition of the Lombard language", "italian");
        AddAccent("Lombard", "sicilian", "with a Sicilian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Sicilian speaker's rendition of the Lombard language", "italian");
        AddAccent("Lombard", "sardinian", "with a Sardinian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Sardinian speaker's rendition of the Lombard language", "italian");
        AddAccent("Lombard", "corsican", "with a Corsican accent", "in an Italian dialect", Difficulty.VeryEasy, "A Corsican speaker's rendition of the Lombard language", "italian");
        AddAccent("Lombard", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker's rendition of the Lombard language", "occitan");
        AddAccent("Lombard", "german", "with a German accent", "in a foreign dialect", Difficulty.VeryEasy, "A German speaker's rendition of the Lombard language", "occitan");
        AddAccent("Lombard", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Lombard language", "foreign");
        AddAccent("Lombard", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Lombard language", "foreign");
        AddAccent("Lombard", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the Lombard language", "foreign");
        AddAccent("Lombard", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the Lombard language", "foreign");
        AddAccent("Lombard", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Lombard language", "foreign");

        AddAccent("Venetian", "venetian", "in the Venetian dialect", "in an Italian dialect", Difficulty.Trivial, "The primary dialect of the Venetian language", "italian");
        AddAccent("Venetian", "florentine", "with a Florentine accent", "in an Italian dialect", Difficulty.VeryEasy, "A Florentine speaker's rendition of the Venetian language", "italian");
        AddAccent("Venetian", "lombard", "with a Lombard accent", "in an Italian dialect", Difficulty.VeryEasy, "A Lombard speaker's rendition of the Venetian language", "italian");
        AddAccent("Venetian", "neapolitan", "with a Neapolitan accent", "in an Italian dialect", Difficulty.VeryEasy, "A Neapolitan speaker's rendition of the Venetian language", "italian");
        AddAccent("Venetian", "sicilian", "with a Sicilian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Sicilian speaker's rendition of the Venetian language", "italian");
        AddAccent("Venetian", "sardinian", "with a Sardinian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Sardinian speaker's rendition of the Venetian language", "italian");
        AddAccent("Venetian", "corsican", "with a Corsican accent", "in an Italian dialect", Difficulty.VeryEasy, "A Corsican speaker's rendition of the Venetian language", "italian");
        AddAccent("Venetian", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker's rendition of the Venetian language", "occitan");
        AddAccent("Venetian", "german", "with a German accent", "in a foreign dialect", Difficulty.VeryEasy, "A German speaker's rendition of the Venetian language", "occitan");
        AddAccent("Venetian", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Venetian language", "foreign");
        AddAccent("Venetian", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Venetian language", "foreign");
        AddAccent("Venetian", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the Venetian language", "foreign");
        AddAccent("Venetian", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the Venetian language", "foreign");
        AddAccent("Venetian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Venetian language", "foreign");

        AddAccent("Neapolitan", "neapolitan", "in the Neapolitan dialect", "in an Italian dialect", Difficulty.Trivial, "The primary dialect of the Neapolitan language", "italian");
        AddAccent("Neapolitan", "florentine", "with a Florentine accent", "in an Italian dialect", Difficulty.VeryEasy, "A Florentine speaker's rendition of the Neapolitan language", "italian");
        AddAccent("Neapolitan", "lombard", "with a Lombard accent", "in an Italian dialect", Difficulty.VeryEasy, "A Lombard speaker's rendition of the Neapolitan language", "italian");
        AddAccent("Neapolitan", "venetian", "with a Venetian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Venetian speaker's rendition of the Neapolitan language", "italian");
        AddAccent("Neapolitan", "sicilian", "with a Sicilian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Sicilian speaker's rendition of the Neapolitan language", "italian");
        AddAccent("Neapolitan", "sardinian", "with a Sardinian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Sardinian speaker's rendition of the Neapolitan language", "italian");
        AddAccent("Neapolitan", "corsican", "with a Corsican accent", "in an Italian dialect", Difficulty.VeryEasy, "A Corsican speaker's rendition of the Neapolitan language", "italian");
        AddAccent("Neapolitan", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker's rendition of the Neapolitan language", "occitan");
        AddAccent("Neapolitan", "german", "with a German accent", "in a foreign dialect", Difficulty.VeryEasy, "A German speaker's rendition of the Neapolitan language", "occitan");
        AddAccent("Neapolitan", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Neapolitan language", "foreign");
        AddAccent("Neapolitan", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Neapolitan language", "foreign");
        AddAccent("Neapolitan", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the Neapolitan language", "foreign");
        AddAccent("Neapolitan", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the Neapolitan language", "foreign");
        AddAccent("Neapolitan", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Neapolitan language", "foreign");

        AddAccent("Sicilian", "neapolitan", "in the Sicilian dialect", "in an Italian dialect", Difficulty.Trivial, "The primary dialect of the Sicilian language", "italian");
        AddAccent("Sicilian", "florentine", "with a Florentine accent", "in an Italian dialect", Difficulty.VeryEasy, "A Florentine speaker's rendition of the Sicilian language", "italian");
        AddAccent("Sicilian", "lombard", "with a Lombard accent", "in an Italian dialect", Difficulty.VeryEasy, "A Lombard speaker's rendition of the Sicilian language", "italian");
        AddAccent("Sicilian", "venetian", "with a Venetian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Venetian speaker's rendition of the Sicilian language", "italian");
        AddAccent("Sicilian", "neapolitan", "with a Neapolitan accent", "in an Italian dialect", Difficulty.VeryEasy, "A Neapolitan speaker's rendition of the Sicilian language", "italian");
        AddAccent("Sicilian", "sardinian", "with a Sardinian accent", "in an Italian dialect", Difficulty.VeryEasy, "A Sardinian speaker's rendition of the Sicilian language", "italian");
        AddAccent("Sicilian", "corsican", "with a Corsican accent", "in an Italian dialect", Difficulty.VeryEasy, "A Corsican speaker's rendition of the Sicilian language", "italian");
        AddAccent("Sicilian", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker's rendition of the Sicilian language", "occitan");
        AddAccent("Sicilian", "german", "with a German accent", "in a foreign dialect", Difficulty.VeryEasy, "A German speaker's rendition of the Sicilian language", "occitan");
        AddAccent("Sicilian", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Sicilian language", "foreign");
        AddAccent("Sicilian", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Sicilian language", "foreign");
        AddAccent("Sicilian", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the Sicilian language", "foreign");
        AddAccent("Sicilian", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the Sicilian language", "foreign");
        AddAccent("Sicilian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Sicilian language", "foreign");

        AddAccent("Castilian", "castilian", "in the Castilian dialect", "in an Iberian dialect", Difficulty.Trivial, "The primary dialect of the Castilian language", "iberian");
        AddAccent("Castilian", "aragonese", "with an Aragonese accent", "in an Iberian dialect", Difficulty.VeryEasy, "An Aragonese speaker's rendition of the Castilian language", "iberian");
        AddAccent("Castilian", "leonese", "with a Leonese accent", "in an Iberian dialect", Difficulty.VeryEasy, "A Leonese speaker's rendition of the Castilian language", "iberian");
        AddAccent("Castilian", "galician", "with a Galician accent", "in an Iberian dialect", Difficulty.VeryEasy, "A Galician speaker's rendition of the Castilian language", "iberian");
        AddAccent("Castilian", "basque", "with a Basque accent", "in an Iberian dialect", Difficulty.Easy, "A Basque speaker's rendition of the Castilian language", "iberian");
        AddAccent("Castilian", "moorish", "with a Moorish accent", "in a foreign dialect", Difficulty.Normal, "A Moor speaker's rendition of the Castilian language", "foreign");
        AddAccent("Castilian", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal, "A French speaker's rendition of the Castilian language", "foreign");
        AddAccent("Castilian", "italian", "with a Italian accent", "in a foreign dialect", Difficulty.Normal, "An Italian speaker's rendition of the Castilian language", "foreign");
        AddAccent("Castilian", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal, "A German speaker's rendition of the Castilian language", "foreign");
        AddAccent("Castilian", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Castilian language", "foreign");
        AddAccent("Castilian", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Castilian language", "foreign");
        AddAccent("Castilian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Castilian language", "foreign");

        AddAccent("Aragonese", "aragonese", "in the Aragonese dialect", "in an Iberian dialect", Difficulty.Trivial, "The primary dialect of the Aragonese language", "iberian");
        AddAccent("Aragonese", "castilian", "with an Castilian accent", "in an Iberian dialect", Difficulty.VeryEasy, "A Castilian speaker's rendition of the Aragonese language", "iberian");
        AddAccent("Aragonese", "leonese", "with a Leonese accent", "in an Iberian dialect", Difficulty.VeryEasy, "A Leonese speaker's rendition of the Aragonese language", "iberian");
        AddAccent("Aragonese", "galician", "with a Galician accent", "in an Iberian dialect", Difficulty.VeryEasy, "A Galician speaker's rendition of the Aragonese language", "iberian");
        AddAccent("Aragonese", "basque", "with a Basque accent", "in an Iberian dialect", Difficulty.Easy, "A Basque speaker's rendition of the Aragonese language", "iberian");
        AddAccent("Aragonese", "moorish", "with a Moorish accent", "in a foreign dialect", Difficulty.Normal, "A Moor speaker's rendition of the Aragonese language", "foreign");
        AddAccent("Aragonese", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal, "A French speaker's rendition of the Aragonese language", "foreign");
        AddAccent("Aragonese", "italian", "with a Italian accent", "in a foreign dialect", Difficulty.Normal, "An Italian speaker's rendition of the Aragonese language", "foreign");
        AddAccent("Aragonese", "sardinian", "with a Sardinian accent", "in a foreign dialect", Difficulty.Normal, "A Sardinian speaker's rendition of the Aragonese language", "foreign");
        AddAccent("Aragonese", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal, "A German speaker's rendition of the Aragonese language", "foreign");
        AddAccent("Aragonese", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Aragonese language", "foreign");
        AddAccent("Aragonese", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Aragonese language", "foreign");
        AddAccent("Aragonese", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Aragonese language", "foreign");

        AddAccent("Leonese", "leonese", "in the Leonese dialect", "in an Iberian dialect", Difficulty.Trivial, "The primary dialect of the Leonese language", "iberian");
        AddAccent("Leonese", "castilian", "with an Castilian accent", "in an Iberian dialect", Difficulty.VeryEasy, "A Castilian speaker's rendition of the Leonese language", "iberian");
        AddAccent("Leonese", "aragonese", "with a Aragonese accent", "in an Iberian dialect", Difficulty.VeryEasy, "An Aragonese speaker's rendition of the Leonese language", "iberian");
        AddAccent("Leonese", "galician", "with a Galician accent", "in an Iberian dialect", Difficulty.VeryEasy, "A Galician speaker's rendition of the Leonese language", "iberian");
        AddAccent("Leonese", "basque", "with a Basque accent", "in an Iberian dialect", Difficulty.Easy, "A Basque speaker's rendition of the Leonese language", "iberian");
        AddAccent("Leonese", "moorish", "with a Moorish accent", "in a foreign dialect", Difficulty.Normal, "A Moor speaker's rendition of the Leonese language", "foreign");
        AddAccent("Leonese", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal, "A French speaker's rendition of the Leonese language", "foreign");
        AddAccent("Leonese", "italian", "with a Italian accent", "in a foreign dialect", Difficulty.Normal, "An Italian speaker's rendition of the Leonese language", "foreign");
        AddAccent("Leonese", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal, "A German speaker's rendition of the Leonese language", "foreign");
        AddAccent("Leonese", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Leonese language", "foreign");
        AddAccent("Leonese", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Leonese language", "foreign");
        AddAccent("Leonese", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Leonese language", "foreign");

        AddAccent("Galician", "galician", "in the Galician dialect", "in an Iberian dialect", Difficulty.Trivial, "The Galician dialect of the Galician language", "iberian");
        AddAccent("Galician", "portugese", "in the Portugese dialect", "in an Iberian dialect", Difficulty.Trivial, "The Portugese dialect of the Galician language", "iberian");
        AddAccent("Galician", "castilian", "with an Castilian accent", "in an Iberian dialect", Difficulty.VeryEasy, "A Castilian speaker's rendition of the Galician language", "iberian");
        AddAccent("Galician", "aragonese", "with a Aragonese accent", "in an Iberian dialect", Difficulty.VeryEasy, "An Aragonese speaker's rendition of the Galician language", "iberian");
        AddAccent("Galician", "leonese", "with a Leonese accent", "in an Iberian dialect", Difficulty.VeryEasy, "A Leonese speaker's rendition of the Galician language", "iberian");
        AddAccent("Galician", "basque", "with a Basque accent", "in an Iberian dialect", Difficulty.Easy, "A Basque speaker's rendition of the Galician language", "iberian");
        AddAccent("Galician", "moorish", "with a Moorish accent", "in a foreign dialect", Difficulty.Normal, "A Moor speaker's rendition of the Galician language", "foreign");
        AddAccent("Galician", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal, "A French speaker's rendition of the Galician language", "foreign");
        AddAccent("Galician", "italian", "with a Italian accent", "in a foreign dialect", Difficulty.Normal, "An Italian speaker's rendition of the Galician language", "foreign");
        AddAccent("Galician", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal, "A German speaker's rendition of the Galician language", "foreign");
        AddAccent("Galician", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Galician language", "foreign");
        AddAccent("Galician", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Galician language", "foreign");
        AddAccent("Galician", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Galician language", "foreign");


        AddAccent("Wallachian", "romanian", "in the Romanian dialect", "in a Wallachian dialect", Difficulty.Trivial, "The Romanian dialect of the Wallachian language", "wallachian");
        AddAccent("Wallachian", "aromanian", "in the Aromanian dialect", "in a Wallachian dialect", Difficulty.Trivial, "The Aromanian dialect of the Wallachian language", "wallachian");
        AddAccent("Wallachian", "meglenite", "in the Meglenite dialect", "in a Wallachian dialect", Difficulty.Trivial, "The Meglenite dialect of the Wallachian language, spoken in Macedonia and Greece", "wallachian");
        AddAccent("Wallachian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Wallachian language", "foreign");

        AddAccent("English", "london", "in the London dialect", "in an English dialect", Difficulty.Trivial, "The London prestige dialect of the English language", "english");
        AddAccent("English", "anglian", "in the Anglian dialect", "in an English dialect", Difficulty.Trivial, "The East Anglian dialect of the English language", "english");
        AddAccent("English", "midlands", "in the Midlands dialect", "in an English dialect", Difficulty.Trivial, "The Midlands dialect of the English language", "english");
        AddAccent("English", "northumbrian", "in the Northumbrian dialect", "in an English dialect", Difficulty.Trivial, "The Northumbrian dialect of the English language", "english");
        AddAccent("English", "scots", "in the Scots dialect", "in an English dialect", Difficulty.Trivial, "The Scots dialect of the English language", "english");
        AddAccent("English", "west-country", "in the West Country dialect", "in an English dialect", Difficulty.Trivial, "The West Country dialect of the English language", "english");
        AddAccent("English", "welsh", "with a Welsh accent", "in a foreign dialect", Difficulty.VeryEasy, "A Welsh speaker's rendition of the English language", "british");
        AddAccent("English", "irish", "with an Irish accent", "in a foreign dialect", Difficulty.VeryEasy, "An Irish speaker's rendition of the English language", "british");
        AddAccent("English", "scottish", "with an Scottish accent", "in a foreign dialect", Difficulty.VeryEasy, "A Scottish speaker's rendition of the English language", "british");
        AddAccent("English", "cornish", "with an Cornish accent", "in a foreign dialect", Difficulty.VeryEasy, "A Cornish speaker's rendition of the English language", "british");
        AddAccent("English", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal, "A German speaker's rendition of the English language", "foreign");
        AddAccent("English", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal, "A French speaker's rendition of the English language", "foreign");
        AddAccent("English", "italian", "with a Italian accent", "in a foreign dialect", Difficulty.Normal, "A Italian speaker's rendition of the English language", "foreign");
        AddAccent("English", "spanish", "with a Spanish accent", "in a foreign dialect", Difficulty.Normal, "A Spanish speaker's rendition of the English language", "foreign");
        AddAccent("English", "scandanavian", "with a Scandanavian accent", "in a foreign dialect", Difficulty.Normal, "A Scandanavian speaker's rendition of the English language", "foreign");
        AddAccent("English", "dutch", "with a Dutch accent", "in a foreign dialect", Difficulty.Normal, "A Dutch speaker's rendition of the English language", "foreign");
        AddAccent("English", "frisian", "with a Frisian accent", "in a foreign dialect", Difficulty.Normal, "A Frisian speaker's rendition of the English language", "foreign");
        AddAccent("English", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the English language", "foreign");

        AddAccent("High German", "thuringian", "in the Thuringian dialect", "in a Central dialect", Difficulty.ExtremelyEasy, "The Thuringian dialect of the High German language", "central");
        AddAccent("High German", "upper saxon", "in the Upper Saxon dialect", "in a Central dialect", Difficulty.ExtremelyEasy, "The Upper Saxon dialect of the High German language", "central");
        AddAccent("High German", "south marchian", "in the South Marchian dialect", "in a Central dialect", Difficulty.ExtremelyEasy, "The South Marchian dialect of the High German language", "central");
        AddAccent("High German", "lusatian", "in the Lusatian dialect", "in a Central dialect", Difficulty.ExtremelyEasy, "The Lusatian dialect of the High German language", "central");
        AddAccent("High German", "Silesian", "in the Silesian dialect", "in a Central dialect", Difficulty.ExtremelyEasy, "The Silesian dialect of the High German language", "central");
        AddAccent("High German", "prussian", "in the Prussian dialect", "in a Central dialect", Difficulty.ExtremelyEasy, "The Prussian dialect of the High German language", "central");
        AddAccent("High German", "central-franconian", "in the Central Franconian dialect", "in a Central dialect", Difficulty.ExtremelyEasy, "The Central Franconian dialect of the High German language", "central");
        AddAccent("High German", "rhine", "in the Rhine-Franconian dialect", "in a Central dialect", Difficulty.ExtremelyEasy, "The Rhine-Franconian dialect of the High German language", "central");
        AddAccent("High German", "east-franconian", "in the East Franconian dialect", "in a High Franconian dialect", Difficulty.ExtremelyEasy, "The East Franconian dialect of the High German language", "high franconian");
        AddAccent("High German", "south-franconian", "in the South Franconian dialect", "in a High Franconian dialect", Difficulty.ExtremelyEasy, "The South Franconian dialect of the High German language", "high franconian");
        AddAccent("High German", "swabian", "in the Swabian dialect", "in an Upper German dialect", Difficulty.Easy, "The Swabian dialect of the High German language", "upper german");
        AddAccent("High German", "alemannic", "in the Alemannic dialect", "in an Upper German dialect", Difficulty.Easy, "The Alemannic dialect of the High German language", "upper german");
        AddAccent("High German", "bavarian", "in the Bavarian dialect", "in an Upper German dialect", Difficulty.Easy, "The Bavarian dialect of the High German language", "upper german");
        AddAccent("High German", "austrian", "in the Austrian dialect", "in an Upper German dialect", Difficulty.Easy, "The Austrian dialect of the High German language", "upper german");
        AddAccent("High German", "low german", "with a Low German accent", "in a foreign germanic dialect", Difficulty.Normal, "A Low German speaker's rendition of the High German language", "germanic");
        AddAccent("High German", "dutch", "with a Dutch accent", "in a foreign germanic dialect", Difficulty.Normal, "A Dutch speaker's rendition of the High German language", "germanic");
        AddAccent("High German", "scandanavian", "with a Scandanavian accent", "in a foreign germanic dialect", Difficulty.Normal, "A Scandanavian speaker's rendition of the High German language", "germanic");
        AddAccent("High German", "frisian", "with a Frisian accent", "in a foreign germanic dialect", Difficulty.Normal, "A Frisian speaker's rendition of the High German language", "germanic");
        AddAccent("High German", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker's rendition of the High German language", "occitan");
        AddAccent("High German", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.VeryEasy, "An Italian speaker's rendition of the High German language", "occitan");
        AddAccent("High German", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the High German language", "foreign");
        AddAccent("High German", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the High German language", "foreign");
        AddAccent("High German", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the High German language", "foreign");
        AddAccent("High German", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the High German language", "foreign");
        AddAccent("High German", "slavic", "with a Slavic accent", "in a foreign dialect", Difficulty.Normal, "A Slavic speaker's rendition of the High German language", "foreign");
        AddAccent("High German", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the language", "foreign");

        AddAccent("Low German", "middle-low-german", "in the Middle Low German dialect", "in a Low German dialect", Difficulty.Trivial, "The Middle Low German dialect of the Low German language, widely used as a lingua franca of the Hanseatic League", "low german");
        AddAccent("Low German", "westphalian", "in the Westphalian dialect", "in a West Low German dialect", Difficulty.VeryEasy, "The Westphalian dialect of Low German, spoken in Westphalia", "west low german");
        AddAccent("Low German", "eastphalian", "in the Eastphalian dialect", "in a West Low German dialect", Difficulty.VeryEasy, "The Eastphalian dialect of Low German, spoken in eastern Lower Saxony", "west low german");
        AddAccent("Low German", "north-low-saxon", "in the North Low Saxon dialect", "in a North Low German dialect", Difficulty.VeryEasy, "The North Low Saxon dialect of Low German, spoken along the North Sea coast", "north low german");
        AddAccent("Low German", "mecklenburgish", "in the Mecklenburgish dialect", "in an East Low German dialect", Difficulty.VeryEasy, "The Mecklenburgish dialect of Low German, spoken in the Mecklenburg region", "east low german");
        AddAccent("Low German", "pomeranian", "in the Pomeranian dialect", "in an East Low German dialect", Difficulty.VeryEasy, "The Pomeranian dialect of Low German, spoken in Pomerania", "east low german");
        AddAccent("Low German", "low-prussian", "in the Low Prussian dialect", "in an East Low German dialect", Difficulty.VeryEasy, "The Low Prussian dialect of Low German, spoken in parts of Prussia", "east low german");
        AddAccent("Low German", "high german", "with a High German accent", "in a foreign Germanic dialect", Difficulty.Normal, "A High German speaker's rendition of the Low German language", "germanic");
        AddAccent("Low German", "dutch", "with a Dutch accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Dutch speaker's rendition of the Low German language", "germanic");
        AddAccent("Low German", "english", "with an English accent", "in a foreign Germanic dialect", Difficulty.Normal, "An English speaker's rendition of the Low German language", "germanic");
        AddAccent("Low German", "frisian", "with a Frisian accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Frisian speaker's rendition of the Low German language", "germanic");
        AddAccent("Low German", "scandinavian", "with a Scandinavian accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Scandinavian speaker's rendition of the Low German language", "germanic");
        AddAccent("Low German", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker's rendition of the Low German language", "occitan");
        AddAccent("Low German", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.VeryEasy, "An Italian speaker's rendition of the Low German language", "occitan");
        AddAccent("Low German", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Low German language", "foreign");
        AddAccent("Low German", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Low German language", "foreign");
        AddAccent("Low German", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the Low German language", "foreign");
        AddAccent("Low German", "slavic", "with a Slavic accent", "in a foreign dialect", Difficulty.Normal, "A Slavic speaker's rendition of the Low German language", "foreign");
        AddAccent("Low German", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the Low German language", "foreign");
        AddAccent("Low German", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Low German language", "foreign");

        AddAccent("Dutch", "hollandic", "in the Hollandic dialect", "in a Dutch dialect", Difficulty.Trivial, "The Hollandic dialect, spoken in the County of Holland, a key contributor to the later standard form of Dutch", "dutch");
        AddAccent("Dutch", "brabantian", "in the Brabantian dialect", "in a Dutch dialect", Difficulty.VeryEasy, "The Brabantian dialect, spoken in the Duchy of Brabant and influential in the development of standard Dutch", "dutch");
        AddAccent("Dutch", "east-flemish", "in the East Flemish dialect", "in a Dutch dialect", Difficulty.VeryEasy, "The East Flemish dialect, spoken in the eastern part of the County of Flanders", "dutch");
        AddAccent("Dutch", "west-flemish", "in the West Flemish dialect", "in a Dutch dialect", Difficulty.VeryEasy, "The West Flemish dialect, spoken in the western part of the County of Flanders and coastal areas", "dutch");
        AddAccent("Dutch", "zeelandic", "in the Zeelandic dialect", "in a Dutch dialect", Difficulty.VeryEasy, "The Zeelandic dialect, spoken in the County of Zeeland, influenced by both Flemish and Hollandic", "dutch");
        AddAccent("Dutch", "limburgish", "in the Limburgish dialect", "in a Dutch dialect", Difficulty.Easy, "The Limburgish dialect, spoken in the Duchy of Limburg and surrounding areas, with notable tonal features", "dutch");
        AddAccent("Dutch", "low german", "with a Low German accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Low German speaker's rendition of the Dutch language, reflective of the Hanseatic trade influence", "germanic");
        AddAccent("Dutch", "high german", "with a High German accent", "in a foreign Germanic dialect", Difficulty.Normal, "A High German speaker's rendition of the Dutch language, influenced by the southern German states", "germanic");
        AddAccent("Dutch", "english", "with an English accent", "in a foreign Germanic dialect", Difficulty.Normal, "An English speaker's rendition of the Dutch language, as encountered through North Sea trade and cultural exchange", "germanic");
        AddAccent("Dutch", "frisian", "with a Frisian accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Frisian speaker's rendition of the Dutch language, reflecting the close linguistic relationship between Frisian and Dutch", "germanic");
        AddAccent("Dutch", "scandinavian", "with a Scandinavian accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Scandinavian speaker's rendition of the Dutch language, possibly influenced by Hanseatic trade routes and northern European contact", "germanic");
        AddAccent("Dutch", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker's rendition of the Dutch language, reflecting cross-cultural contact and influence from the south", "occitan");
        AddAccent("Dutch", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.VeryEasy, "An Italian speaker's rendition of the Dutch language, influenced by Mediterranean trade and Renaissance cultural exchange", "occitan");
        AddAccent("Dutch", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Dutch language, indicative of cultural contact across Europe", "foreign");
        AddAccent("Dutch", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Dutch language, reflecting links formed through trade and pilgrimage routes", "foreign");
        AddAccent("Dutch", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the Dutch language, influenced by maritime trade and the Habsburg realms", "foreign");
        AddAccent("Dutch", "slavic", "with a Slavic accent", "in a foreign dialect", Difficulty.Normal, "A Slavic speaker's rendition of the Dutch language, reflecting long-distance trade and travel across Europe", "foreign");
        AddAccent("Dutch", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the Dutch language, indicative of distant diplomatic and trade encounters", "foreign");
        AddAccent("Dutch", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Dutch language, without a clearly identifiable origin", "foreign");

        AddAccent("Frisian", "west-frisian", "in the West Frisian dialect", "in a Frisian dialect", Difficulty.Trivial, "The West Frisian dialect, spoken in what is now the province of Friesland in the Northern Netherlands, forming the core of the Frisian language", "frisian");
        AddAccent("Frisian", "east-frisian", "in the East Frisian dialect", "in a Frisian dialect", Difficulty.VeryEasy, "The East Frisian dialect, spoken to the east of the Ems river, influenced by Low German", "frisian");
        AddAccent("Frisian", "north-frisian", "in the North Frisian dialect", "in a Frisian dialect", Difficulty.VeryEasy, "The North Frisian dialect, spoken along the North Sea coast in what is now North Frisia", "frisian");
        AddAccent("Frisian", "dutch", "with a Dutch accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Dutch speaker's rendition of the Frisian language", "germanic");
        AddAccent("Frisian", "low german", "with a Low German accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Low German speaker's rendition of the Frisian language", "germanic");
        AddAccent("Frisian", "english", "with an English accent", "in a foreign Germanic dialect", Difficulty.Normal, "An English speaker's rendition of the Frisian language", "germanic");
        AddAccent("Frisian", "high german", "with a High German accent", "in a foreign Germanic dialect", Difficulty.Normal, "A High German speaker's rendition of the Frisian language", "germanic");
        AddAccent("Frisian", "scandinavian", "with a Scandinavian accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Scandinavian speaker's rendition of the Frisian language", "germanic");
        AddAccent("Frisian", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker's rendition of the Frisian language", "occitan");
        AddAccent("Frisian", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.VeryEasy, "An Italian speaker's rendition of the Frisian language", "occitan");
        AddAccent("Frisian", "franco-provencal", "with a Franco-Provencal accent", "in a foreign dialect", Difficulty.Normal, "A Franco-Provencal speaker's rendition of the Frisian language", "foreign");
        AddAccent("Frisian", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Frisian language", "foreign");
        AddAccent("Frisian", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian speaker's rendition of the Frisian language", "foreign");
        AddAccent("Frisian", "slavic", "with a Slavic accent", "in a foreign dialect", Difficulty.Normal, "A Slavic speaker's rendition of the Frisian language", "foreign");
        AddAccent("Frisian", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the Frisian language", "foreign");
        AddAccent("Frisian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Frisian language", "foreign");

        AddAccent("Yiddish", "southwestern", "in the Southwestern Western Yiddish dialect", "in a Western Yiddish dialect", Difficulty.Trivial, "The Southwestern variety of Western Yiddish, spoken in regions like Alsace and parts of southwestern Germany", "yiddish");
        AddAccent("Yiddish", "midwestern", "in the Midwestern Western Yiddish dialect", "in a Western Yiddish dialect", Difficulty.VeryEasy, "The Midwestern variety of Western Yiddish, spoken in central German lands, influenced by Middle German dialects", "yiddish");
        AddAccent("Yiddish", "northwestern", "in the Northwestern Western Yiddish dialect", "in a Western Yiddish dialect", Difficulty.VeryEasy, "The Northwestern variety of Western Yiddish, spoken closer to Low German areas and the Low Countries", "yiddish");
        AddAccent("Yiddish", "central-eastern", "in an early Central Eastern Yiddish dialect", "in an Eastern Yiddish dialect", Difficulty.Easy, "A nascent form of Eastern Yiddish spoken in Polish-Lithuanian Commonwealth territories (e.g., Poland, Galicia), showing increasing Slavic influence", "yiddish");
        AddAccent("Yiddish", "german", "with a German accent", "in a foreign Germanic dialect", Difficulty.Normal, "A German (High or Low) speaker’s rendition of Yiddish, reflecting the shared linguistic base but differing phonologies and lexical choices", "germanic");
        AddAccent("Yiddish", "dutch", "with a Dutch accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Dutch speaker’s rendition of Yiddish, influenced by Low Franconian sounds", "germanic");
        AddAccent("Yiddish", "english", "with an English accent", "in a foreign Germanic dialect", Difficulty.Normal, "An English speaker’s rendition of Yiddish, reflective of distant but related Germanic roots", "germanic");
        AddAccent("Yiddish", "frisian", "with a Frisian accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Frisian speaker’s rendition of Yiddish, showing a Germanic substrate with different vowel systems", "germanic");
        AddAccent("Yiddish", "scandinavian", "with a Scandinavian accent", "in a foreign Germanic dialect", Difficulty.Normal, "A Scandinavian speaker’s rendition of Yiddish, influenced by North Germanic speech patterns", "germanic");
        AddAccent("Yiddish", "polish", "with a Polish accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Polish speaker’s rendition of Yiddish, reflecting Slavic influence that would shape Eastern Yiddish", "slavic");
        AddAccent("Yiddish", "czech", "with a Czech accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Czech speaker’s rendition of Yiddish, indicative of Central European Slavic contact", "slavic");
        AddAccent("Yiddish", "russian", "with a Russian accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Russian speaker’s rendition of Yiddish, less common in this early period but possible through trade routes", "slavic");
        AddAccent("Yiddish", "french", "with a French accent", "in a foreign dialect", Difficulty.VeryEasy, "A French speaker’s rendition of Yiddish, influenced by Romance phonology", "occitan");
        AddAccent("Yiddish", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.VeryEasy, "An Italian speaker’s rendition of Yiddish, reflecting Mediterranean influence through trade and Jewish migrations", "occitan");
        AddAccent("Yiddish", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian (Castilian or Aragonese) speaker’s rendition of Yiddish, possibly through Sephardic-Ashkenazic contacts", "foreign");
        AddAccent("Yiddish", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker’s rendition of Yiddish, indicating connections among Jewish communities across Europe", "foreign");
        AddAccent("Yiddish", "baltic", "with a Baltic accent", "in a foreign dialect", Difficulty.Normal, "A Baltic speaker’s rendition of Yiddish, indicative of contacts in the eastern Baltic regions", "foreign");
        AddAccent("Yiddish", "finnic", "with a Finnic accent", "in a foreign dialect", Difficulty.Normal, "A Finnic (e.g., Finnish or Karelian) speaker’s rendition of Yiddish, possibly through northern trade routes", "foreign");
        AddAccent("Yiddish", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker’s rendition of Yiddish, reflecting trade and diplomatic links extending into Southeast Europe", "foreign");
        AddAccent("Yiddish", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker’s rendition of the Yiddish language, without a clearly identifiable origin", "foreign");

        AddAccent("Russian", "muscovite", "in the Muscovite dialect", "in a Russian dialect", Difficulty.Trivial, "The Muscovite dialect, spoken in and around Moscow, forming the foundation of the emerging Russian standard", "russian");
        AddAccent("Russian", "novgorodian", "in the Novgorodian dialect", "in a Russian dialect", Difficulty.VeryEasy, "The Novgorodian dialect, a Northern Russian variety centered around the city of Novgorod", "russian");
        AddAccent("Russian", "pskovian", "in the Pskovian dialect", "in a Russian dialect", Difficulty.VeryEasy, "The Pskovian dialect, a Northern Russian variety spoken in the Pskov region", "russian");
        AddAccent("Russian", "tverian", "in the Tverian dialect", "in a Russian dialect", Difficulty.VeryEasy, "The Tverian dialect, a Central Russian variety spoken around Tver", "russian");
        AddAccent("Russian", "ryazanian", "in the Ryazanian dialect", "in a Russian dialect", Difficulty.VeryEasy, "The Ryazanian dialect, a Southern Russian variety spoken in the Ryazan region", "russian");
        AddAccent("Russian", "ruthenian", "with a Ruthenian accent", "in a foreign Slavic dialect", Difficulty.Easy, "A Ruthenian speaker's rendition of the Russian language", "slavic");
        AddAccent("Russian", "polish", "with a Polish accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Polish speaker's rendition of the Russian language", "slavic");
        AddAccent("Russian", "czech", "with a Czech accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Czech speaker's rendition of the Russian language", "slavic");
        AddAccent("Russian", "slovak", "with a Slovak accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Slovak speaker's rendition of the Russian language", "slavic");
        AddAccent("Russian", "wendish", "with a Wendish accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Wendish (Sorbian) speaker's rendition of the Russian language", "slavic");
        AddAccent("Russian", "south slavic", "with a South Slavic accent", "in a foreign Slavic dialect", Difficulty.Normal, "A speaker from the South Slavic lands (e.g., Bulgarian or Serbian) giving their rendition of the Russian language", "slavic");
        AddAccent("Russian", "baltic", "with a Baltic accent", "in a foreign dialect", Difficulty.Normal, "A Baltic (e.g. Lithuanian or Latvian) speaker's rendition of the Russian language", "foreign");
        AddAccent("Russian", "finnic", "with a Finnic accent", "in a foreign dialect", Difficulty.Normal, "A Finnic (e.g., Finnish or Karelian) speaker's rendition of the Russian language", "foreign");
        AddAccent("Russian", "tatar", "with a Tatar accent", "in a foreign dialect", Difficulty.Normal, "A Tatar (Turkic) speaker's rendition of the Russian language, reflecting interactions on the steppe frontiers", "foreign");
        AddAccent("Russian", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal, "A German speaker's rendition of the Russian language, possibly reflecting Hanseatic or Livonian contact", "foreign");
        AddAccent("Russian", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal, "A French speaker's rendition of the Russian language", "foreign");
        AddAccent("Russian", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal, "An Italian speaker's rendition of the Russian language", "foreign");
        AddAccent("Russian", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian (Castilian, Aragonese, Portuguese) speaker's rendition of the Russian language", "foreign");
        AddAccent("Russian", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker's rendition of the Russian language", "foreign");
        AddAccent("Russian", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker's rendition of the Russian language", "foreign");
        AddAccent("Russian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker's rendition of the Russian language", "foreign");

        AddAccent("Ruthenian", "chancery", "in the Chancery Ruthenian dialect", "in a Ruthenian dialect", Difficulty.Trivial, "The Chancery Ruthenian dialect, used in official documents and administration of the Grand Duchy of Lithuania", "ruthenian");
        AddAccent("Ruthenian", "volhynian", "in the Volhynian dialect", "in a Ruthenian dialect", Difficulty.VeryEasy, "The Volhynian dialect, spoken in the Volhynia region, reflecting a transitional form between northern and southern varieties", "ruthenian");
        AddAccent("Ruthenian", "polesian", "in the Polesian dialect", "in a Ruthenian dialect", Difficulty.VeryEasy, "The Polesian dialect, spoken in the marshy lands of Polesia, characterized by conservative phonetic features", "ruthenian");
        AddAccent("Ruthenian", "podolian", "in the Podolian dialect", "in a Ruthenian dialect", Difficulty.VeryEasy, "The Podolian dialect, spoken in the fertile Podolia region, showing southern Ruthenian linguistic traits", "ruthenian");
        AddAccent("Ruthenian", "halychian", "in the Halychian dialect", "in a Ruthenian dialect", Difficulty.VeryEasy, "The Halychian dialect, spoken in and around Halych (Galicia), blending Ruthenian features with neighboring Polish influence", "ruthenian");
        AddAccent("Ruthenian", "russian", "with a Russian accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Russian speaker's rendition of Ruthenian, reflecting East Slavic mutual intelligibility yet distinct phonetic shifts", "slavic");
        AddAccent("Ruthenian", "polish", "with a Polish accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Polish speaker's rendition of Ruthenian, indicative of close cultural and linguistic contact in the Commonwealth", "slavic");
        AddAccent("Ruthenian", "czech", "with a Czech accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Czech speaker's rendition of Ruthenian, showing some West Slavic phonetic traits", "slavic");
        AddAccent("Ruthenian", "slovak", "with a Slovak accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Slovak speaker's rendition of Ruthenian, reflecting another West Slavic influence", "slavic");
        AddAccent("Ruthenian", "wendish", "with a Wendish accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Wendish (Sorbian) speaker's rendition of Ruthenian, bridging East and West Slavic linguistic features", "slavic");
        AddAccent("Ruthenian", "south slavic", "with a South Slavic accent", "in a foreign Slavic dialect", Difficulty.Normal, "A South Slavic speaker's rendition of Ruthenian, introducing Balkan linguistic elements", "slavic");
        AddAccent("Ruthenian", "baltic", "with a Baltic accent", "in a foreign dialect", Difficulty.Normal, "A Baltic (e.g., Lithuanian) speaker’s rendition of Ruthenian, reflecting the multilingual nature of the Grand Duchy", "foreign");
        AddAccent("Ruthenian", "finnic", "with a Finnic accent", "in a foreign dialect", Difficulty.Normal, "A Finnic (Finnish, Karelian) speaker’s rendition of Ruthenian, perhaps encountered through northern trade routes", "foreign");
        AddAccent("Ruthenian", "tatar", "with a Tatar accent", "in a foreign dialect", Difficulty.Normal, "A Tatar (Turkic) speaker’s rendition of Ruthenian, representing steppe frontier interactions", "foreign");
        AddAccent("Ruthenian", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal, "A German speaker’s rendition of Ruthenian, indicative of trade and settlement patterns within the Commonwealth", "foreign");
        AddAccent("Ruthenian", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal, "A French speaker’s rendition of Ruthenian, though less common, possible through diplomatic or scholarly encounters", "foreign");
        AddAccent("Ruthenian", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal, "An Italian speaker’s rendition of Ruthenian, possibly stemming from papal or merchant missions", "foreign");
        AddAccent("Ruthenian", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian (Castilian, Aragonese) speaker’s rendition of Ruthenian, a rare but conceivable contact via trading networks", "foreign");
        AddAccent("Ruthenian", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker’s rendition of Ruthenian, reflecting Europe’s far-reaching cultural exchanges", "foreign");
        AddAccent("Ruthenian", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker’s rendition of Ruthenian, arising from frontier interactions and diplomacy", "foreign");
        AddAccent("Ruthenian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker’s rendition of the Ruthenian language, without a clearly identifiable origin", "foreign");

        AddAccent("Polish", "greater-poland", "in the Greater Poland dialect", "in a Polish dialect", Difficulty.Trivial, "The Greater Poland (Wielkopolska) dialect, centered around Poznań, considered one of the primary bases for later standard Polish", "polish");
        AddAccent("Polish", "lesser-poland", "in the Lesser Poland dialect", "in a Polish dialect", Difficulty.Trivial, "The Lesser Poland (Małopolska) dialect, spoken in and around Kraków, influential in shaping literary Polish", "polish");
        AddAccent("Polish", "mazovian", "in the Mazovian dialect", "in a Polish dialect", Difficulty.VeryEasy, "The Mazovian (Mazowieckie) dialect, spoken around Warsaw, with some distinct phonetic features differentiating it from Lesser and Greater Poland", "polish");
        AddAccent("Polish", "silesian", "in the Silesian dialect", "in a Polish dialect", Difficulty.VeryEasy, "The Silesian dialect, spoken in Silesia, showing certain transitional traits between Polish and Czech", "polish");
        AddAccent("Polish", "russian", "with a Russian accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Russian speaker’s rendition of the Polish language, reflecting East Slavic influences", "slavic");
        AddAccent("Polish", "ruthenian", "with a Ruthenian accent", "in a foreign Slavic dialect", Difficulty.Easy, "A Ruthenian speaker’s rendition of the Polish language, common within the Polish-Lithuanian Commonwealth", "slavic");
        AddAccent("Polish", "czech", "with a Czech accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Czech speaker’s rendition of the Polish language, reflecting close West Slavic linguistic ties", "slavic");
        AddAccent("Polish", "slovak", "with a Slovak accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Slovak speaker’s rendition of the Polish language, bridging West Slavic dialect continuums", "slavic");
        AddAccent("Polish", "wendish", "with a Wendish accent", "in a foreign Slavic dialect", Difficulty.Normal, "A Wendish (Sorbian) speaker’s rendition of the Polish language, showing West Slavic commonalities", "slavic");
        AddAccent("Polish", "south slavic", "with a South Slavic accent", "in a foreign Slavic dialect", Difficulty.Normal, "A South Slavic speaker’s rendition of the Polish language, introducing Balkan elements to West Slavic speech", "slavic");
        AddAccent("Polish", "baltic", "with a Baltic accent", "in a foreign dialect", Difficulty.Normal, "A Baltic (e.g., Lithuanian) speaker’s rendition of the Polish language, indicative of the bilingual milieu of the Commonwealth", "foreign");
        AddAccent("Polish", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal, "A German speaker’s rendition of the Polish language, reflecting historical German settlement and trade in Polish lands", "foreign");
        AddAccent("Polish", "finnic", "with a Finnic accent", "in a foreign dialect", Difficulty.Normal, "A Finnic (Finnish or Estonian) speaker’s rendition of the Polish language, possible through extended trade networks", "foreign");
        AddAccent("Polish", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal, "A French speaker’s rendition of the Polish language, perhaps encountered via diplomatic missions", "foreign");
        AddAccent("Polish", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal, "An Italian speaker’s rendition of the Polish language, introduced through cultural exchange and Renaissance influence", "foreign");
        AddAccent("Polish", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal, "An Iberian (Castilian, Aragonese) speaker’s rendition of the Polish language, illustrating long-distance European ties", "foreign");
        AddAccent("Polish", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal, "An Occitan speaker’s rendition of the Polish language, reflecting Europe’s complex tapestry of linguistic contacts", "foreign");
        AddAccent("Polish", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal, "An Ottoman Turkish speaker’s rendition of the Polish language, reflecting frontier interactions of the Commonwealth", "foreign");
        AddAccent("Polish", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker’s rendition of the Polish language, without a clearly identifiable origin", "foreign");

        AddAccent("Church Slavonic", "old church slavonic", "in the original Old Church Slavonic form", "in a classical Slavic liturgical form", Difficulty.Trivial,
            "The original standardized form of Church Slavonic as established by Saints Cyril and Methodius, widely revered and used in religious texts",
            "standard");

        AddAccent("Church Slavonic", "muscovite", "in the Muscovite recension", "in a Slavic liturgical form", Difficulty.VeryEasy,
            "The Muscovite (Russian) recension of Church Slavonic, adapted to local pronunciation and scribal traditions in the Grand Duchy of Moscow",
            "standard");

        AddAccent("Church Slavonic", "serbian", "in the Serbian recension", "in a Slavic liturgical form", Difficulty.VeryEasy,
            "The Serbian recension of Church Slavonic, reflecting the phonetic and lexical traits of South Slavic monastic communities",
            "standard");

        AddAccent("Church Slavonic", "bulgarian", "in the Bulgarian recension", "in a Slavic liturgical form", Difficulty.VeryEasy,
            "The Bulgarian recension of Church Slavonic, closely aligned with the original literary tradition and used in Bulgarian Orthodox liturgy",
            "standard");

        AddAccent("Church Slavonic", "wallachian", "in the Wallachian recension", "in a Slavic liturgical form", Difficulty.VeryEasy,
            "The Wallachian recension of Church Slavonic, adapted for Orthodox communities in the Romanian principalities, bridging Slavic and Romance spheres",
            "standard");

        AddAccent("Church Slavonic", "ruthenian", "in the Ruthenian recension", "in a Slavic liturgical form", Difficulty.VeryEasy,
            "The Ruthenian recension of Church Slavonic, influenced by East Slavic linguistic elements and used in the Orthodox rites of the Polish-Lithuanian Commonwealth",
            "standard");

        AddAccent("Church Slavonic", "foreign", "with a foreign accent", "in a foreign accent", Difficulty.Normal,
            "A generic non-native speaker’s rendition of Church Slavonic, reflecting external influence but not aligned with any particular recension",
            "foreign");

        AddAccent("Slovak", "western-slovak", "in the Western Slovak dialect", "in a Slovak dialect", Difficulty.Trivial,
            "The Western Slovak dialect, spoken in regions closer to Moravia and influenced by Czech forms", "slovak");
        AddAccent("Slovak", "central-slovak", "in the Central Slovak dialect", "in a Slovak dialect", Difficulty.Trivial,
            "The Central Slovak dialect, a core group of dialects later influential in the eventual standardization of Slovak", "slovak");
        AddAccent("Slovak", "eastern-slovak", "in the Eastern Slovak dialect", "in a Slovak dialect", Difficulty.Trivial,
            "The Eastern Slovak dialect, spoken in the eastern parts of present-day Slovakia, with certain distinctive phonetic traits", "slovak");
        AddAccent("Slovak", "czech", "with a Czech accent", "in a foreign Slavic dialect", Difficulty.VeryEasy,
            "A Czech speaker’s rendition of the Slovak language, reflecting the close linguistic relationship between Czech and Slovak", "slavic");
        AddAccent("Slovak", "polish", "with a Polish accent", "in a foreign Slavic dialect", Difficulty.Easy,
            "A Polish speaker’s rendition of the Slovak language, showing another West Slavic connection", "slavic");
        AddAccent("Slovak", "ruthenian", "with a Ruthenian accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A Ruthenian speaker’s rendition of the Slovak language, reflecting East Slavic influences in the Carpathian regions", "slavic");
        AddAccent("Slovak", "wendish", "with a Wendish accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A Wendish (Sorbian) speaker’s rendition of the Slovak language, bridging West Slavic linguistic terrain", "slavic");
        AddAccent("Slovak", "south slavic", "with a South Slavic accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A South Slavic speaker’s rendition of the Slovak language, introducing Balkan Slavic elements", "slavic");
        AddAccent("Slovak", "hungarian", "with a Hungarian accent", "in a foreign dialect", Difficulty.Normal,
            "A Hungarian speaker’s rendition of the Slovak language, reflecting the multilingual nature of the Kingdom of Hungary", "foreign");
        AddAccent("Slovak", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Slovak language, common in towns with German settlers throughout the region", "foreign");
        AddAccent("Slovak", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal,
            "A French speaker’s rendition of the Slovak language, likely rare but possible via diplomatic or trade routes", "foreign");
        AddAccent("Slovak", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal,
            "An Italian speaker’s rendition of the Slovak language, reflecting distant trade or scholarly connections", "foreign");
        AddAccent("Slovak", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal,
            "An Iberian (Castilian or Aragonese) speaker’s rendition of the Slovak language, through extended European trade networks", "foreign");
        AddAccent("Slovak", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal,
            "An Occitan speaker’s rendition of the Slovak language, symbolizing Europe’s broad tapestry of linguistic contact", "foreign");
        AddAccent("Slovak", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal,
            "An Ottoman Turkish speaker’s rendition of the Slovak language, reflecting frontier tensions and diplomatic encounters", "foreign");
        AddAccent("Slovak", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker’s rendition of the Slovak language, lacking any specific identifiable origin", "foreign");

        AddAccent("Czech", "bohemian", "in the Bohemian dialect", "in a Czech dialect", Difficulty.Trivial,
            "The Bohemian dialect, centered around Prague and the historical region of Bohemia, often serving as a literary and prestige variety", "czech");
        AddAccent("Czech", "moravian", "in the Moravian dialect", "in a Czech dialect", Difficulty.Trivial,
            "The Moravian dialect, spoken in Moravia, reflecting transitional traits between Czech and Slovak forms", "czech");
        AddAccent("Czech", "silesian", "in the Silesian Czech dialect", "in a Czech dialect", Difficulty.VeryEasy,
            "The Silesian Czech dialect, spoken in Czech-influenced areas of Silesia, bridging Czech and Polish influences", "czech");
        AddAccent("Czech", "polish", "with a Polish accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A Polish speaker’s rendition of the Czech language, reflecting close West Slavic linguistic ties", "slavic");
        AddAccent("Czech", "slovak", "with a Slovak accent", "in a foreign Slavic dialect", Difficulty.VeryEasy,
            "A Slovak speaker’s rendition of the Czech language, indicative of the high mutual intelligibility within West Slavic languages", "slavic");
        AddAccent("Czech", "ruthenian", "with a Ruthenian accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A Ruthenian speaker’s rendition of the Czech language, bringing in East Slavic elements", "slavic");
        AddAccent("Czech", "russian", "with a Russian accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A Russian speaker’s rendition of the Czech language, reflecting East Slavic phonetic patterns", "slavic");
        AddAccent("Czech", "wendish", "with a Wendish accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A Wendish (Sorbian) speaker’s rendition of the Czech language, another West Slavic point of reference", "slavic");
        AddAccent("Czech", "south slavic", "with a South Slavic accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A South Slavic speaker’s rendition of the Czech language, introducing Balkan Slavic elements", "slavic");
        AddAccent("Czech", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Czech language, reflecting the significant presence of German communities within the Bohemian lands", "foreign");
        AddAccent("Czech", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal,
            "A French speaker’s rendition of the Czech language, possible through diplomatic or scholarly contact", "foreign");
        AddAccent("Czech", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal,
            "An Italian speaker’s rendition of the Czech language, reflecting Renaissance-era cultural exchanges", "foreign");
        AddAccent("Czech", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal,
            "An Iberian (Castilian or Aragonese) speaker’s rendition of the Czech language, testament to Europe-wide trade and travel", "foreign");
        AddAccent("Czech", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal,
            "An Occitan speaker’s rendition of the Czech language, highlighting Europe’s intricate linguistic mosaic", "foreign");
        AddAccent("Czech", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal,
            "An Ottoman Turkish speaker’s rendition of the Czech language, reflecting occasional diplomatic or mercantile encounters", "foreign");
        AddAccent("Czech", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal, "A generic non-native speaker’s rendition of the Czech language, without a clearly distinguishable origin", "foreign");

        AddAccent("Wendish", "upper-sorbian", "in the Upper Sorbian dialect", "in a Wendish dialect", Difficulty.Trivial,
            "The Upper Sorbian dialect, spoken mainly in the hilly region around Bautzen (Budyšin), known for conservative Slavic features",
            "wendish");
        AddAccent("Wendish", "lower-sorbian", "in the Lower Sorbian dialect", "in a Wendish dialect", Difficulty.Trivial,
            "The Lower Sorbian dialect, spoken in the marshy lowlands around Cottbus (Chóśebuz), with distinct phonetic developments",
            "wendish");
        AddAccent("Wendish", "czech", "with a Czech accent", "in a foreign Slavic dialect", Difficulty.VeryEasy,
            "A Czech speaker’s rendition of the Wendish language, reflecting close linguistic ties among West Slavic peoples",
            "slavic");
        AddAccent("Wendish", "polish", "with a Polish accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A Polish speaker’s rendition of the Wendish language, another West Slavic connection showcasing mutual intelligibility",
            "slavic");
        AddAccent("Wendish", "slovak", "with a Slovak accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A Slovak speaker’s rendition of the Wendish language, introducing influences from the Carpathian region",
            "slavic");
        AddAccent("Wendish", "ruthenian", "with a Ruthenian accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A Ruthenian speaker’s rendition of the Wendish language, reflecting more distant East Slavic influences",
            "slavic");
        AddAccent("Wendish", "south slavic", "with a South Slavic accent", "in a foreign Slavic dialect", Difficulty.Normal,
            "A South Slavic speaker’s rendition of the Wendish language, adding Balkan Slavic elements to a West Slavic base",
            "slavic");
        AddAccent("Wendish", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Wendish language, reflecting the dominant influence of German states in Lusatia",
            "foreign");
        AddAccent("Wendish", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal,
            "A French speaker’s rendition of the Wendish language, possible through distant scholarly or diplomatic contacts",
            "foreign");
        AddAccent("Wendish", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal,
            "An Italian speaker’s rendition of the Wendish language, indicative of long-range cultural exchanges in Renaissance Europe",
            "foreign");
        AddAccent("Wendish", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal,
            "An Iberian (Castilian or Aragonese) speaker’s rendition of the Wendish language, reflecting pan-European trade and travel",
            "foreign");
        AddAccent("Wendish", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal,
            "An Occitan speaker’s rendition of the Wendish language, showing the complexity of linguistic contacts across the continent",
            "foreign");
        AddAccent("Wendish", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal,
            "An Ottoman Turkish speaker’s rendition of the Wendish language, though rare, possible through diplomatic intermediaries",
            "foreign");
        AddAccent("Wendish", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Wendish language, not attributable to any particular group",
            "foreign");

        AddAccent("Prussian", "bartian", "in the Bartian dialect", "in a Prussian dialect", Difficulty.Trivial,
            "The Bartian dialect, one of the Western Baltic dialects of the Old Prussian language, spoken in the region historically associated with the Bartians",
            "prussian");
        AddAccent("Prussian", "sambian", "in the Sambian dialect", "in a Prussian dialect", Difficulty.VeryEasy,
            "The Sambian dialect, spoken in Samland (Sambia), a core region of Old Prussian-speaking communities",
            "prussian");
        AddAccent("Prussian", "natangian", "in the Natangian dialect", "in a Prussian dialect", Difficulty.VeryEasy,
            "The Natangian dialect, associated with the Natangians, reflecting localized variations in pronunciation and vocabulary",
            "prussian");
        AddAccent("Prussian", "galindian", "in the Galindian dialect", "in a Prussian dialect", Difficulty.VeryEasy,
            "The Galindian dialect, tied to the Galindians, showing distinct Western Baltic linguistic traits within the Old Prussian continuum",
            "prussian");
        AddAccent("Prussian", "lithuanian", "with a Lithuanian accent", "in a foreign Baltic dialect", Difficulty.Normal,
            "A Lithuanian speaker’s rendition of the Prussian language, reflecting close Baltic linguistic kinship",
            "foreign");
        AddAccent("Prussian", "latvian", "with a Latvian accent", "in a foreign Baltic dialect", Difficulty.Normal,
            "A Latvian speaker’s rendition of the Prussian language, drawing on shared Baltic roots but distinct phonetic patterns",
            "foreign");
        AddAccent("Prussian", "polish", "with a Polish accent", "in a foreign dialect", Difficulty.Normal,
            "A Polish speaker’s rendition of the Prussian language, reflecting Slavic influence in the region",
            "foreign");
        AddAccent("Prussian", "german", "with a German accent", "in a foreign dialect", Difficulty.VeryEasy,
            "A German speaker’s rendition of the Prussian language, indicative of the strong Germanization pressures on Prussia",
            "foreign");
        AddAccent("Prussian", "russian", "with a Russian accent", "in a foreign dialect", Difficulty.Normal,
            "A Russian speaker’s rendition of the Prussian language, revealing distant East Slavic contact through trade or conquest",
            "foreign");
        AddAccent("Prussian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Prussian language, reflecting an outsider’s approximation of Baltic sounds",
            "foreign");

        AddAccent("Lithuanian", "aukštaitian", "in the Aukštaitian dialect", "in a Lithuanian dialect", Difficulty.Trivial,
            "The Aukštaitian (High Lithuanian) dialect, forming the main foundation of what would become standard Lithuanian",
            "lithuanian");
        AddAccent("Lithuanian", "samogitian", "in the Samogitian dialect", "in a Lithuanian dialect", Difficulty.VeryEasy,
            "The Samogitian (Žemaitian) dialect, distinctively spoken in northwestern Lithuania with marked phonetic differences",
            "lithuanian");
        AddAccent("Lithuanian", "eastern-aukštaitian", "in the Eastern Aukštaitian dialect", "in a Lithuanian dialect", Difficulty.VeryEasy,
            "The Eastern Aukštaitian dialect, known for more conservative phonetic traits and spoken in eastern Lithuania",
            "lithuanian");
        AddAccent("Lithuanian", "southern-aukštaitian", "in the Southern Aukštaitian dialect", "in a Lithuanian dialect", Difficulty.VeryEasy,
            "The Southern Aukštaitian dialect, spoken in southeastern Lithuania, influenced by its position within the Polish-Lithuanian Commonwealth",
            "lithuanian");
        AddAccent("Lithuanian", "ruthenian", "with a Ruthenian accent", "in a foreign dialect", Difficulty.Normal,
            "A Ruthenian speaker’s rendition of the Lithuanian language",
            "foreign");
        AddAccent("Lithuanian", "polish", "with a Polish accent", "in a foreign dialect", Difficulty.Normal,
            "A Polish speaker’s rendition of the Lithuanian language",
            "foreign");
        AddAccent("Lithuanian", "latvian", "with a Latvian accent", "in a foreign dialect", Difficulty.Normal,
            "A Latvian speaker’s rendition of the Lithuanian language",
            "foreign");
        AddAccent("Lithuanian", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Lithuanian language",
            "foreign");
        AddAccent("Lithuanian", "russian", "with a Russian accent", "in a foreign dialect", Difficulty.Normal,
            "A Russian speaker’s rendition of the Lithuanian language",
            "foreign");
        AddAccent("Lithuanian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Lithuanian language",
            "foreign");

        AddAccent("Latvian", "latgalian", "in the Latgalian dialect", "in a Latvian dialect", Difficulty.Trivial,
            "The Latgalian dialect, spoken in eastern regions, known for preserving older Baltic linguistic features",
            "latvian");
        AddAccent("Latvian", "curonian", "in the Curonian dialect", "in a Latvian dialect", Difficulty.VeryEasy,
            "The Curonian dialect, originating along the coast, influenced by maritime connections and contacts",
            "latvian");
        AddAccent("Latvian", "semigallian", "in the Semigallian dialect", "in a Latvian dialect", Difficulty.VeryEasy,
            "The Semigallian dialect, spoken in the south-central region, reflecting transitional Baltic characteristics",
            "latvian");
        AddAccent("Latvian", "selonian", "in the Selonian dialect", "in a Latvian dialect", Difficulty.VeryEasy,
            "The Selonian dialect, spoken in southeastern parts, retaining some archaic Baltic elements",
            "latvian");
        AddAccent("Latvian", "lithuanian", "with a Lithuanian accent", "in a foreign dialect", Difficulty.Normal,
            "A Lithuanian speaker’s rendition of the Latvian language",
            "foreign");
        AddAccent("Latvian", "prussian", "with a Prussian accent", "in a foreign dialect", Difficulty.Normal,
            "A Prussian speaker’s rendition of the Latvian language",
            "foreign");
        AddAccent("Latvian", "polish", "with a Polish accent", "in a foreign dialect", Difficulty.Normal,
            "A Polish speaker’s rendition of the Latvian language",
            "foreign");
        AddAccent("Latvian", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Latvian language",
            "foreign");
        AddAccent("Latvian", "russian", "with a Russian accent", "in a foreign dialect", Difficulty.Normal,
            "A Russian speaker’s rendition of the Latvian language",
            "foreign");
        AddAccent("Latvian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Latvian language",
            "foreign");

        AddAccent("Serbo-Croatian", "shtokavian", "in the Shtokavian dialect", "in a Serbo-Croatian dialect", Difficulty.Trivial,
            "The Shtokavian dialect, widely spoken and forming the core of the later standard forms of Serbo-Croatian",
            "serbo-croatian");
        AddAccent("Serbo-Croatian", "chakavian", "in the Chakavian dialect", "in a Serbo-Croatian dialect", Difficulty.VeryEasy,
            "The Chakavian dialect, characteristic of coastal and island regions, preserving older linguistic features",
            "serbo-croatian");
        AddAccent("Serbo-Croatian", "kajkavian", "in the Kajkavian dialect", "in a Serbo-Croatian dialect", Difficulty.VeryEasy,
            "The Kajkavian dialect, spoken in the northwestern areas, sharing some similarities with Slovenian and other West South Slavic forms",
            "serbo-croatian");
        AddAccent("Serbo-Croatian", "ijekavian-shtokavian", "in the Ijekavian Shtokavian dialect", "in a Serbo-Croatian dialect", Difficulty.VeryEasy,
            "The Ijekavian Shtokavian dialect, noted for its characteristic 'ije' reflex, prominent in central and eastern regions",
            "serbo-croatian");
        AddAccent("Serbo-Croatian", "ekavian-shtokavian", "in the Ekavian Shtokavian dialect", "in a Serbo-Croatian dialect", Difficulty.VeryEasy,
            "The Ekavian Shtokavian dialect, distinguished by its 'e' vowel reflex, prevalent in more eastern areas",
            "serbo-croatian");
        AddAccent("Serbo-Croatian", "hungarian", "with a Hungarian accent", "in a foreign dialect", Difficulty.Normal,
            "A Hungarian speaker’s rendition of the Serbo-Croatian language",
            "foreign");
        AddAccent("Serbo-Croatian", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Serbo-Croatian language",
            "foreign");
        AddAccent("Serbo-Croatian", "ruthenian", "with a Ruthenian accent", "in a foreign dialect", Difficulty.Normal,
            "A Ruthenian speaker’s rendition of the Serbo-Croatian language",
            "foreign");
        AddAccent("Serbo-Croatian", "polish", "with a Polish accent", "in a foreign dialect", Difficulty.Normal,
            "A Polish speaker’s rendition of the Serbo-Croatian language",
            "foreign");
        AddAccent("Serbo-Croatian", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal,
            "An Italian speaker’s rendition of the Serbo-Croatian language",
            "foreign");
        AddAccent("Serbo-Croatian", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal,
            "An Ottoman speaker’s rendition of the Serbo-Croatian language",
            "foreign");
        AddAccent("Serbo-Croatian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Serbo-Croatian language",
            "foreign");

        AddAccent("Slovenian", "upper-carniolan", "in the Upper Carniolan dialect", "in a Slovenian dialect", Difficulty.Trivial,
            "The Upper Carniolan dialect, centered around the area north of Ljubljana, historically influential in later literary tradition",
            "slovenian");
        AddAccent("Slovenian", "lower-carniolan", "in the Lower Carniolan dialect", "in a Slovenian dialect", Difficulty.VeryEasy,
            "The Lower Carniolan dialect, spoken southeast of Ljubljana, contributing to the broader Carniolan group",
            "slovenian");
        AddAccent("Slovenian", "carinthian", "in the Carinthian dialect", "in a Slovenian dialect", Difficulty.VeryEasy,
            "The Carinthian dialect, spoken in the Slovenian Alpine regions to the north, with some archaic linguistic features",
            "slovenian");
        AddAccent("Slovenian", "styrian", "in the Styrian dialect", "in a Slovenian dialect", Difficulty.VeryEasy,
            "The Styrian dialect, used in the northeastern parts of Slovene-speaking territory, bridging Slovenian and neighboring varieties",
            "slovenian");
        AddAccent("Slovenian", "littoral", "in the Littoral dialect", "in a Slovenian dialect", Difficulty.VeryEasy,
            "The Littoral dialect, spoken in the western regions near the Adriatic, reflecting maritime and Romance influences",
            "slovenian");
        AddAccent("Slovenian", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Slovenian language",
            "foreign");
        AddAccent("Slovenian", "hungarian", "with a Hungarian accent", "in a foreign dialect", Difficulty.Normal,
            "A Hungarian speaker’s rendition of the Slovenian language",
            "foreign");
        AddAccent("Slovenian", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal,
            "An Italian speaker’s rendition of the Slovenian language",
            "foreign");
        AddAccent("Slovenian", "croatian", "with a Croatian accent", "in a foreign dialect", Difficulty.Normal,
            "A Croatian speaker’s rendition of the Slovenian language",
            "foreign");
        AddAccent("Slovenian", "ruthenian", "with a Ruthenian accent", "in a foreign dialect", Difficulty.Normal,
            "A Ruthenian speaker’s rendition of the Slovenian language",
            "foreign");
        AddAccent("Slovenian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Slovenian language",
            "foreign");

        AddAccent("Bulgarian", "eastern-bulgarian", "in the Eastern Bulgarian dialect", "in a Bulgarian dialect", Difficulty.Trivial,
            "The Eastern Bulgarian dialect, known for its distinct reflexes and forming a large part of later standard Bulgarian",
            "bulgarian");
        AddAccent("Bulgarian", "western-bulgarian", "in the Western Bulgarian dialect", "in a Bulgarian dialect", Difficulty.VeryEasy,
            "The Western Bulgarian dialect, spoken in the western regions, differing notably from the eastern varieties in certain vowel and consonant shifts",
            "bulgarian");
        AddAccent("Bulgarian", "moesian", "in the Moesian dialect", "in a Bulgarian dialect", Difficulty.VeryEasy,
            "The Moesian dialect, a northern variety of Bulgarian, preserving older Slavic features",
            "bulgarian");
        AddAccent("Bulgarian", "thracian", "in the Thracian dialect", "in a Bulgarian dialect", Difficulty.VeryEasy,
            "The Thracian dialect, spoken in the southeastern regions, known for specific vowel developments and lexical forms",
            "bulgarian");
        AddAccent("Bulgarian", "shop", "in the Shop dialect", "in a Bulgarian dialect", Difficulty.VeryEasy,
            "The Shop dialect, spoken in the western mountainous regions near Sofia, noted for its distinctive phonetic traits",
            "bulgarian");
        AddAccent("Bulgarian", "ruthenian", "with a Ruthenian accent", "in a foreign dialect", Difficulty.Normal,
            "A Ruthenian speaker’s rendition of the Bulgarian language",
            "foreign");
        AddAccent("Bulgarian", "serbo-croatian", "with a Serbo-Croatian accent", "in a foreign dialect", Difficulty.Normal,
            "A Serbo-Croatian speaker’s rendition of the Bulgarian language",
            "foreign");
        AddAccent("Bulgarian", "greek", "with a Greek accent", "in a foreign dialect", Difficulty.Normal,
            "A Greek speaker’s rendition of the Bulgarian language",
            "foreign");
        AddAccent("Bulgarian", "turkish", "with a Turkish accent", "in a foreign dialect", Difficulty.Normal,
            "A Turkish speaker’s rendition of the Bulgarian language",
            "foreign");
        AddAccent("Bulgarian", "romanian", "with a Romanian accent", "in a foreign dialect", Difficulty.Normal,
            "A Romanian speaker’s rendition of the Bulgarian language",
            "foreign");
        AddAccent("Bulgarian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Bulgarian language",
            "foreign");

        AddAccent("Quranic Arabic", "classical", "in the classical recitation", "in the classical form", Difficulty.Trivial,
            "The classical recitation, widely recognized as the authoritative form of Quranic Arabic, closely adhering to established standards of pronunciation",
            "standard");

        AddAccent("Quranic Arabic", "hafs", "in the Ḥafṣ ‘an ʿĀṣim tradition", "in a canonical recitation", Difficulty.VeryEasy,
            "The Ḥafṣ ‘an ʿĀṣim tradition, one of the most commonly used canonical recitation methods, maintaining precise articulation and vocalization",
            "standard");

        AddAccent("Quranic Arabic", "warsh", "in the Warsh ‘an Nāfiʿ tradition", "in a canonical recitation", Difficulty.VeryEasy,
            "The Warsh ‘an Nāfiʿ tradition, favored in parts of North Africa, offering slight variations in vocalization while preserving the text’s integrity",
            "standard");

        AddAccent("Quranic Arabic", "foreign", "with a foreign accent", "in a foreign accent", Difficulty.Normal,
            "A generic non-native speaker’s rendition of Quranic Arabic",
            "foreign");

        AddAccent("Mahgrebi Arabic", "moroccan", "in the Moroccan dialect", "in a Mahgrebi dialect", Difficulty.Trivial,
            "The Moroccan dialect, reflecting a distinctive phonetic character influenced by Berber and Iberian legacies",
            "mahgrebi");
        AddAccent("Mahgrebi Arabic", "algerian", "in the Algerian dialect", "in a Mahgrebi dialect", Difficulty.VeryEasy,
            "The Algerian dialect, widely spoken in the central Maghreb, with notable phonological shifts",
            "mahgrebi");
        AddAccent("Mahgrebi Arabic", "tunisian", "in the Tunisian dialect", "in a Mahgrebi dialect", Difficulty.VeryEasy,
            "The Tunisian dialect, reflecting maritime connections and a rich blend of influences",
            "mahgrebi");
        AddAccent("Mahgrebi Arabic", "libyan", "in the Libyan dialect", "in a Mahgrebi dialect", Difficulty.VeryEasy,
            "The Libyan dialect, spoken to the east of the Maghreb, bridging towards Mashriqi varieties",
            "mahgrebi");
        AddAccent("Mahgrebi Arabic", "mashriqi", "with a Mashriqi accent", "in a foreign dialect", Difficulty.Normal,
            "A Mashriqi speaker’s rendition of the Mahgrebi Arabic language",
            "foreign");
        AddAccent("Mahgrebi Arabic", "berber", "with a Berber accent", "in a foreign dialect", Difficulty.Normal,
            "A Berber speaker’s rendition of the Mahgrebi Arabic language",
            "foreign");
        AddAccent("Mahgrebi Arabic", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal,
            "An Ottoman speaker’s rendition of the Mahgrebi Arabic language",
            "foreign");
        AddAccent("Mahgrebi Arabic", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal,
            "An Iberian speaker’s rendition of the Mahgrebi Arabic language",
            "foreign");
        AddAccent("Mahgrebi Arabic", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Mahgrebi Arabic language",
            "foreign");

        AddAccent("Mashriqi Arabic", "egyptian", "in the Egyptian dialect", "in a Mashriqi dialect", Difficulty.Trivial,
            "The Egyptian dialect, centered around the Nile Valley, a prestigious form known for its cultural influence",
            "mashriqi");
        AddAccent("Mashriqi Arabic", "levantine", "in the Levantine dialect", "in a Mashriqi dialect", Difficulty.VeryEasy,
            "The Levantine dialect, spoken in the Eastern Mediterranean regions, marked by soft phonetics",
            "mashriqi");
        AddAccent("Mashriqi Arabic", "iraqi", "in the Iraqi dialect", "in a Mashriqi dialect", Difficulty.VeryEasy,
            "The Iraqi dialect, reflecting Mesopotamian linguistic heritage and diverse influences",
            "mashriqi");
        AddAccent("Mashriqi Arabic", "hejazi", "in the Hejazi dialect", "in a Mashriqi dialect", Difficulty.VeryEasy,
            "The Hejazi dialect, spoken in the western Arabian Peninsula, associated with old trade routes and pilgrimage centers",
            "mashriqi");

        AddAccent("Mashriqi Arabic", "mahgrebi", "with a Mahgrebi accent", "in a foreign dialect", Difficulty.Normal,
            "A Mahgrebi speaker’s rendition of the Mashriqi Arabic language",
            "foreign");
        AddAccent("Mashriqi Arabic", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal,
            "An Ottoman speaker’s rendition of the Mashriqi Arabic language",
            "foreign");
        AddAccent("Mashriqi Arabic", "persian", "with a Persian accent", "in a foreign dialect", Difficulty.Normal,
            "A Persian speaker’s rendition of the Mashriqi Arabic language",
            "foreign");
        AddAccent("Mashriqi Arabic", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Mashriqi Arabic language",
            "foreign");

        AddAccent("Aramaic", "western-aramaic", "in the Western Aramaic dialect", "in an Aramaic dialect", Difficulty.Trivial,
            "The Western Aramaic dialect, once widespread in the Levant but greatly diminished by the 16th century, still surviving in a few isolated communities",
            "aramaic");
        AddAccent("Aramaic", "northeastern-neo-aramic", "in the Northeastern Neo-Aramaic dialect", "in an Aramaic dialect", Difficulty.VeryEasy,
            "The Northeastern Neo-Aramaic dialect, spoken in scattered communities of Mesopotamia and the mountains beyond",
            "aramaic");
        AddAccent("Aramaic", "turoyo", "in the Turoyo dialect", "in an Aramaic dialect", Difficulty.VeryEasy,
            "The Turoyo dialect, a Central Neo-Aramaic variety spoken in the Tur Abdin region",
            "aramaic");
        AddAccent("Aramaic", "mandaean", "in the Mandaean dialect", "in an Aramaic dialect", Difficulty.Easy,
            "The Mandaean dialect, used liturgically by Mandaean communities along the lower reaches of the Tigris and Euphrates",
            "aramaic");
        AddAccent("Aramaic", "arabic", "with an Arabic accent", "in a foreign dialect", Difficulty.Normal,
            "An Arabic speaker’s rendition of the Aramaic language",
            "foreign");
        AddAccent("Aramaic", "persian", "with a Persian accent", "in a foreign dialect", Difficulty.Normal,
            "A Persian speaker’s rendition of the Aramaic language",
            "foreign");
        AddAccent("Aramaic", "greek", "with a Greek accent", "in a foreign dialect", Difficulty.Normal,
            "A Greek speaker’s rendition of the Aramaic language",
            "foreign");
        AddAccent("Aramaic", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal,
            "An Ottoman speaker’s rendition of the Aramaic language",
            "foreign");
        AddAccent("Aramaic", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Aramaic language",
            "foreign");

        AddAccent("Berber", "kabyle", "in the Kabyle dialect", "in a Berber dialect", Difficulty.Trivial,
            "The Kabyle dialect, spoken in the Kabylie region of the Atlas Mountains",
            "berber");
        AddAccent("Berber", "tashelhit", "in the Tashelhit (Shilha) dialect", "in a Berber dialect", Difficulty.VeryEasy,
            "The Tashelhit (Shilha) dialect, prevalent in southwestern Morocco’s High Atlas and Anti-Atlas mountains",
            "berber");
        AddAccent("Berber", "zenaga", "in the Zenaga dialect", "in a Berber dialect", Difficulty.VeryEasy,
            "The Zenaga dialect, spoken historically in parts of Mauritania and the southern Sahara",
            "berber");
        AddAccent("Berber", "rifian", "in the Riffian dialect", "in a Berber dialect", Difficulty.VeryEasy,
            "The Riffian dialect, spoken in the Rif mountains of northern Morocco",
            "berber");
        AddAccent("Berber", "zuwara", "in the Zuwara (Nafusi) dialect", "in a Berber dialect", Difficulty.VeryEasy,
            "The Zuwara (Nafusi) dialect, found in northwestern Libya",
            "berber");
        AddAccent("Berber", "arabic", "with an Arabic accent", "in a foreign dialect", Difficulty.Normal,
            "An Arabic speaker’s rendition of the Berber language",
            "foreign");
        AddAccent("Berber", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal,
            "An Iberian speaker’s rendition of the Berber language",
            "foreign");
        AddAccent("Berber", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal,
            "An Italian speaker’s rendition of the Berber language",
            "foreign");
        AddAccent("Berber", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal,
            "An Ottoman speaker’s rendition of the Berber language",
            "foreign");
        AddAccent("Berber", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Berber language",
            "foreign");

        AddAccent("Hebrew", "sephardic", "in the Sephardic tradition", "in a Hebrew tradition", Difficulty.Trivial,
            "The Sephardic tradition, as preserved by communities of the Iberian diaspora, known for a distinctive vowel system",
            "hebrew");
        AddAccent("Hebrew", "ashkenazic", "in the Ashkenazic tradition", "in a Hebrew tradition", Difficulty.VeryEasy,
            "The Ashkenazic tradition, maintained by communities in Central and Eastern Europe, with characteristic vowel shifts",
            "hebrew");
        AddAccent("Hebrew", "yemenite", "in the Yemenite tradition", "in a Hebrew tradition", Difficulty.VeryEasy,
            "The Yemenite tradition, noted for preserving older Semitic phonetic features",
            "hebrew");
        AddAccent("Hebrew", "mizrahi", "in the Mizrahi tradition", "in a Hebrew tradition", Difficulty.VeryEasy,
            "The Mizrahi tradition, associated with communities in the Middle East and North Africa",
            "hebrew");
        AddAccent("Hebrew", "arabic", "with an Arabic accent", "in a foreign dialect", Difficulty.Normal,
            "An Arabic speaker’s rendition of the Hebrew language",
            "foreign");
        AddAccent("Hebrew", "aramaic", "with an Aramaic accent", "in a foreign dialect", Difficulty.Normal,
            "An Aramaic speaker’s rendition of the Hebrew language",
            "foreign");
        AddAccent("Hebrew", "yiddish", "with a Yiddish accent", "in a foreign dialect", Difficulty.Normal,
            "A Yiddish speaker’s rendition of the Hebrew language",
            "foreign");
        AddAccent("Hebrew", "greek", "with a Greek accent", "in a foreign dialect", Difficulty.Normal,
            "A Greek speaker’s rendition of the Hebrew language",
            "foreign");
        AddAccent("Hebrew", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal,
            "An Italian speaker’s rendition of the Hebrew language",
            "foreign");
        AddAccent("Hebrew", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Hebrew language",
            "foreign");

        AddAccent("Greek", "moreot", "in the Moreot (Peloponnesian) dialect", "in a Greek dialect", Difficulty.Trivial,
            "The Moreot (Peloponnesian) dialect, spoken in the Peloponnese, showing features that would influence later standard Greek",
            "greek");
        AddAccent("Greek", "cretan", "in the Cretan dialect", "in a Greek dialect", Difficulty.VeryEasy,
            "The Cretan dialect, known for its rich poetic tradition and distinct phonetic traits",
            "greek");
        AddAccent("Greek", "ionian", "in the Ionian dialect", "in a Greek dialect", Difficulty.VeryEasy,
            "The Ionian dialect, spoken on the Ionian Islands, reflecting Western Greek characteristics",
            "greek");
        AddAccent("Greek", "epirotic", "in the Epirotic dialect", "in a Greek dialect", Difficulty.VeryEasy,
            "The Epirotic dialect, spoken in northwestern Greek regions, retaining conservative phonological features",
            "greek");
        AddAccent("Greek", "italian", "with an Italian accent", "in a foreign dialect", Difficulty.Normal,
            "An Italian speaker’s rendition of the Greek language",
            "foreign");
        AddAccent("Greek", "slavic", "with a Slavic accent", "in a foreign dialect", Difficulty.Normal,
            "A Slavic speaker’s rendition of the Greek language",
            "foreign");
        AddAccent("Greek", "turkish", "with a Turkish accent", "in a foreign dialect", Difficulty.Normal,
            "A Turkish speaker’s rendition of the Greek language",
            "foreign");
        AddAccent("Greek", "arabic", "with an Arabic accent", "in a foreign dialect", Difficulty.Normal,
            "An Arabic speaker’s rendition of the Greek language",
            "foreign");
        AddAccent("Greek", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Greek language",
            "foreign");

        AddAccent("Koine Greek", "classical", "in the classical Koine form", "in a Koine form", Difficulty.Trivial,
            "The classical Koine form, reflecting the standard language of Hellenistic and early Christian texts",
            "koine greek");
        AddAccent("Koine Greek", "byzantine", "in the Byzantine scholarly tradition", "in a Koine form", Difficulty.VeryEasy,
            "The Byzantine scholarly tradition, preserved by the Eastern Orthodox Church and intellectual circles",
            "koine greek");
        AddAccent("Koine Greek", "foreign", "with a foreign accent", "in a foreign accent", Difficulty.Normal,
            "A generic non-native speaker’s rendition of Koine Greek",
            "foreign");

        AddAccent("Persian", "tehrani", "in the Tehrani dialect", "in a Persian dialect", Difficulty.Trivial,
            "The Tehrani dialect, centered around the capital region, forming the basis of later standard Persian",
            "persian");
        AddAccent("Persian", "khorasani", "in the Khorasani dialect", "in a Persian dialect", Difficulty.VeryEasy,
            "The Khorasani dialect, spoken in the northeastern regions, known for certain archaic features",
            "persian");
        AddAccent("Persian", "shirazi", "in the Shirazi dialect", "in a Persian dialect", Difficulty.VeryEasy,
            "The Shirazi dialect, spoken in the south, associated with a rich literary heritage",
            "persian");
        AddAccent("Persian", "tabrizi", "in the Tabrizi dialect", "in a Persian dialect", Difficulty.VeryEasy,
            "The Tabrizi dialect, influenced by Turkic languages due to its location in northwestern Iran",
            "persian");
        AddAccent("Persian", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal,
            "An Ottoman speaker’s rendition of the Persian language",
            "foreign");
        AddAccent("Persian", "arabic", "with an Arabic accent", "in a foreign dialect", Difficulty.Normal,
            "An Arabic speaker’s rendition of the Persian language",
            "foreign");
        AddAccent("Persian", "tatar", "with a Tatar accent", "in a foreign dialect", Difficulty.Normal,
            "A Tatar speaker’s rendition of the Persian language",
            "foreign");
        AddAccent("Persian", "indian", "with an Indian accent", "in a foreign dialect", Difficulty.Normal,
            "An Indian speaker’s rendition of the Persian language",
            "foreign");
        AddAccent("Persian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Persian language",
            "foreign");

        AddAccent("Armenian", "eastern-armenian", "in the Eastern Armenian dialect", "in an Armenian dialect", Difficulty.Trivial,
            "The Eastern Armenian dialect, spoken in the territories east of the Euphrates, including areas around the Ararat plain",
            "armenian");
        AddAccent("Armenian", "western-armenian", "in the Western Armenian dialect", "in an Armenian dialect", Difficulty.VeryEasy,
            "The Western Armenian dialect, prevalent in the Ottoman-controlled Armenian communities, influenced by Turkish and other neighboring languages",
            "armenian");
        AddAccent("Armenian", "karabakh", "in the Karabakh dialect", "in an Armenian dialect", Difficulty.VeryEasy,
            "The Karabakh dialect, an Eastern variety spoken in mountainous areas, noted for its phonetic distinctions",
            "armenian");
        AddAccent("Armenian", "cilician", "in the Cilician dialect", "in an Armenian dialect", Difficulty.VeryEasy,
            "The Cilician dialect, a Western variety historically associated with the Armenian Kingdom of Cilicia",
            "armenian");
        AddAccent("Armenian", "ottoman", "with an Ottoman accent", "in a foreign dialect", Difficulty.Normal,
            "An Ottoman speaker’s rendition of the Armenian language",
            "foreign");
        AddAccent("Armenian", "persian", "with a Persian accent", "in a foreign dialect", Difficulty.Normal,
            "A Persian speaker’s rendition of the Armenian language",
            "foreign");
        AddAccent("Armenian", "georgian", "with a Georgian accent", "in a foreign dialect", Difficulty.Normal,
            "A Georgian speaker’s rendition of the Armenian language",
            "foreign");
        AddAccent("Armenian", "russian", "with a Russian accent", "in a foreign dialect", Difficulty.Normal,
            "A Russian speaker’s rendition of the Armenian language",
            "foreign");
        AddAccent("Armenian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Armenian language",
            "foreign");

        AddAccent("Coptic", "bohairic", "in the Bohairic dialect", "in a Coptic dialect", Difficulty.Trivial,
            "The Bohairic dialect, used in the Nile Delta region, eventually becoming the standard liturgical form",
            "coptic");
        AddAccent("Coptic", "sahidic", "in the Sahidic dialect", "in a Coptic dialect", Difficulty.VeryEasy,
            "The Sahidic dialect, historically spoken in Upper Egypt, once a literary standard before Bohairic",
            "coptic");
        AddAccent("Coptic", "arabic", "with an Arabic accent", "in a foreign dialect", Difficulty.Normal,
            "An Arabic speaker’s rendition of the Coptic language",
            "foreign");
        AddAccent("Coptic", "greek", "with a Greek accent", "in a foreign dialect", Difficulty.Normal,
            "A Greek speaker’s rendition of the Coptic language",
            "foreign");
        AddAccent("Coptic", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Coptic language",
            "foreign");

        AddAccent("Turkish", "istanbulite", "in the Istanbulite variety", "in an Ottoman Turkish variety", Difficulty.Trivial,
            "The Istanbulite variety, centered in the imperial capital and considered prestigious",
            "ottoman");
        AddAccent("Turkish", "anatolian", "in the Anatolian variety", "in an Ottoman Turkish variety", Difficulty.VeryEasy,
            "The Anatolian variety, reflecting older Turkic features and local influences from central Anatolia",
            "ottoman");
        AddAccent("Turkish", "rumelian", "in the Rumelian variety", "in an Ottoman Turkish variety", Difficulty.VeryEasy,
            "The Rumelian variety, spoken in the European territories of the empire, influenced by Balkan languages",
            "ottoman");
        AddAccent("Turkish", "arabic", "with an Arabic accent", "in a foreign dialect", Difficulty.Normal,
            "An Arabic speaker’s rendition of the Ottoman Turkish language",
            "foreign");
        AddAccent("Turkish", "persian", "with a Persian accent", "in a foreign dialect", Difficulty.Normal,
            "A Persian speaker’s rendition of the Ottoman Turkish language",
            "foreign");
        AddAccent("Turkish", "slavic", "with a Slavic accent", "in a foreign dialect", Difficulty.Normal,
            "A Slavic speaker’s rendition of the Ottoman Turkish language",
            "foreign");
        AddAccent("Turkish", "greek", "with a Greek accent", "in a foreign dialect", Difficulty.Normal,
            "A Greek speaker’s rendition of the Ottoman Turkish language",
            "foreign");
        AddAccent("Turkish", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Ottoman Turkish language",
            "foreign");

        AddAccent("Basque", "gipuzkoan", "in the Gipuzkoan dialect", "in a Basque dialect", Difficulty.Trivial,
            "The Gipuzkoan dialect, spoken in Gipuzkoa, considered one of the central Basque varieties",
            "basque");
        AddAccent("Basque", "bizkaian", "in the Bizkaian dialect", "in a Basque dialect", Difficulty.VeryEasy,
            "The Bizkaian dialect, spoken in Biscay, featuring distinctive phonetic characteristics",
            "basque");
        AddAccent("Basque", "upper-navarrese", "in the Upper Navarrese dialect", "in a Basque dialect", Difficulty.VeryEasy,
            "The Upper Navarrese dialect, spoken in the Navarre region, reflecting transitional features",
            "basque");
        AddAccent("Basque", "labourdine", "in the Labourdine dialect", "in a Basque dialect", Difficulty.VeryEasy,
            "The Labourdine dialect, from the coastal area of Labourd in the French Basque country",
            "basque");
        AddAccent("Basque", "lower-navarrese", "in the Lower Navarrese dialect", "in a Basque dialect", Difficulty.VeryEasy,
            "The Lower Navarrese dialect, spoken in parts of the northern (French) side of the Pyrenees",
            "basque");
        AddAccent("Basque", "iberian", "with an Iberian accent", "in a foreign dialect", Difficulty.Normal,
            "An Iberian speaker’s rendition of the Basque language",
            "foreign");
        AddAccent("Basque", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal,
            "A French speaker’s rendition of the Basque language",
            "foreign");
        AddAccent("Basque", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal,
            "An Occitan speaker’s rendition of the Basque language",
            "foreign");
        AddAccent("Basque", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Basque language",
            "foreign");

        AddAccent("Swedish", "svealand", "in the Svealand dialect", "in a Swedish dialect", Difficulty.Trivial,
            "The Svealand dialect, centered around Stockholm and Mälaren Valley, influencing the emerging standard Swedish",
            "swedish");
        AddAccent("Swedish", "götaland", "in the Götaland dialect", "in a Swedish dialect", Difficulty.VeryEasy,
            "The Götaland dialect, spoken in southern and western parts of the kingdom, marked by certain phonetic distinctions",
            "swedish");
        AddAccent("Swedish", "norrland", "in the Norrland dialect", "in a Swedish dialect", Difficulty.VeryEasy,
            "The Norrland dialect, from the northern regions, known for more conservative linguistic features",
            "swedish");
        AddAccent("Swedish", "finland-swedish", "in the Finland-Swedish dialect", "in a Swedish dialect", Difficulty.VeryEasy,
            "The Finland-Swedish dialect, spoken by the Swedish-speaking minority in the eastern domains",
            "swedish");
        AddAccent("Swedish", "danish", "with a Danish accent", "in a foreign dialect", Difficulty.Normal,
            "A Danish speaker’s rendition of the Swedish language",
            "foreign");
        AddAccent("Swedish", "norwegian", "with a Norwegian accent", "in a foreign dialect", Difficulty.Normal,
            "A Norwegian speaker’s rendition of the Swedish language",
            "foreign");
        AddAccent("Swedish", "icelandic", "with an Icelandic accent", "in a foreign dialect", Difficulty.Normal,
            "An Icelandic speaker’s rendition of the Swedish language",
            "foreign");
        AddAccent("Swedish", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Swedish language",
            "foreign");
        AddAccent("Swedish", "english", "with an English accent", "in a foreign dialect", Difficulty.Normal,
            "An English speaker’s rendition of the Swedish language",
            "foreign");
        AddAccent("Swedish", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Swedish language",
            "foreign");

        AddAccent("Norwegian", "eastern-norwegian", "in the Eastern Norwegian dialect", "in a Norwegian dialect", Difficulty.Trivial,
            "The Eastern Norwegian dialect, spoken in regions close to the Swedish border, showing certain transitional features",
            "norwegian");
        AddAccent("Norwegian", "western-norwegian", "in the Western Norwegian dialect", "in a Norwegian dialect", Difficulty.VeryEasy,
            "The Western Norwegian dialect, associated with the fjords and mountains, retaining older Norse characteristics",
            "norwegian");
        AddAccent("Norwegian", "trøndelag", "in the Trøndelag dialect", "in a Norwegian dialect", Difficulty.VeryEasy,
            "The Trøndelag dialect, found in central Norway, known for distinctive intonation and vowel qualities",
            "norwegian");
        AddAccent("Norwegian", "northern-norwegian", "in the Northern Norwegian dialect", "in a Norwegian dialect", Difficulty.VeryEasy,
            "The Northern Norwegian dialect, spoken in the far north, influenced by contact with Sámi and Finnish",
            "norwegian");
        AddAccent("Norwegian", "swedish", "with a Swedish accent", "in a foreign dialect", Difficulty.Normal,
            "A Swedish speaker’s rendition of the Norwegian language",
            "foreign");
        AddAccent("Norwegian", "danish", "with a Danish accent", "in a foreign dialect", Difficulty.Normal,
            "A Danish speaker’s rendition of the Norwegian language",
            "foreign");
        AddAccent("Norwegian", "icelandic", "with an Icelandic accent", "in a foreign dialect", Difficulty.Normal,
            "An Icelandic speaker’s rendition of the Norwegian language",
            "foreign");
        AddAccent("Norwegian", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Norwegian language",
            "foreign");
        AddAccent("Norwegian", "english", "with an English accent", "in a foreign dialect", Difficulty.Normal,
            "An English speaker’s rendition of the Norwegian language",
            "foreign");
        AddAccent("Norwegian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Norwegian language",
            "foreign");

        AddAccent("Danish", "jutlandic", "in the Jutlandic dialect", "in a Danish dialect", Difficulty.Trivial,
            "The Jutlandic dialect, spoken on the Jutland peninsula, showing certain conservative phonetic traits",
            "danish");
        AddAccent("Danish", "insular", "in the Insular Danish dialect", "in a Danish dialect", Difficulty.VeryEasy,
            "The Insular Danish dialect, spoken on the islands such as Zealand, influencing the later standard",
            "danish");
        AddAccent("Danish", "scanian", "in the Scanian dialect", "in a Danish dialect", Difficulty.VeryEasy,
            "The Scanian dialect, historically Danish, though spoken in what is now southern Sweden, bridging Nordic varieties",
            "danish");
        AddAccent("Danish", "bornholm", "in the Bornholm dialect", "in a Danish dialect", Difficulty.VeryEasy,
            "The Bornholm dialect, isolated on the island of Bornholm, preserving archaic features",
            "danish");
        AddAccent("Danish", "swedish", "with a Swedish accent", "in a foreign dialect", Difficulty.Normal,
            "A Swedish speaker’s rendition of the Danish language",
            "foreign");
        AddAccent("Danish", "norwegian", "with a Norwegian accent", "in a foreign dialect", Difficulty.Normal,
            "A Norwegian speaker’s rendition of the Danish language",
            "foreign");
        AddAccent("Danish", "icelandic", "with an Icelandic accent", "in a foreign dialect", Difficulty.Normal,
            "An Icelandic speaker’s rendition of the Danish language",
            "foreign");
        AddAccent("Danish", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Danish language",
            "foreign");
        AddAccent("Danish", "english", "with an English accent", "in a foreign dialect", Difficulty.Normal,
            "An English speaker’s rendition of the Danish language",
            "foreign");
        AddAccent("Danish", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Danish language",
            "foreign");

        AddAccent("Icelandic", "northern-icelandic", "in the Northern Icelandic dialect", "in an Icelandic dialect", Difficulty.Trivial,
            "The Northern Icelandic dialect, considered close to classical Old Norse forms",
            "icelandic");
        AddAccent("Icelandic", "western-icelandic", "in the Western Icelandic dialect", "in an Icelandic dialect", Difficulty.VeryEasy,
            "The Western Icelandic dialect, spoken in the western regions, showing subtle phonetic variations",
            "icelandic");
        AddAccent("Icelandic", "southern-icelandic", "in the Southern Icelandic dialect", "in an Icelandic dialect", Difficulty.VeryEasy,
            "The Southern Icelandic dialect, used in the south, differing slightly in vowel length and intonation",
            "icelandic");
        AddAccent("Icelandic", "eastern-icelandic", "in the Eastern Icelandic dialect", "in an Icelandic dialect", Difficulty.VeryEasy,
            "The Eastern Icelandic dialect, found in the east, preserving some archaic word forms",
            "icelandic");
        AddAccent("Icelandic", "danish", "with a Danish accent", "in a foreign dialect", Difficulty.Normal,
            "A Danish speaker’s rendition of the Icelandic language",
            "foreign");
        AddAccent("Icelandic", "swedish", "with a Swedish accent", "in a foreign dialect", Difficulty.Normal,
            "A Swedish speaker’s rendition of the Icelandic language",
            "foreign");
        AddAccent("Icelandic", "norwegian", "with a Norwegian accent", "in a foreign dialect", Difficulty.Normal,
            "A Norwegian speaker’s rendition of the Icelandic language",
            "foreign");
        AddAccent("Icelandic", "english", "with an English accent", "in a foreign dialect", Difficulty.Normal,
            "An English speaker’s rendition of the Icelandic language",
            "foreign");
        AddAccent("Icelandic", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Icelandic language",
            "foreign");
        AddAccent("Icelandic", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Icelandic language",
            "foreign");

        AddAccent("Finnish", "southwestern", "in the Southwestern dialect", "in a Finnish dialect", Difficulty.Trivial,
            "The Southwestern dialect, spoken in coastal and southwestern regions, influenced by maritime connections",
            "finnish");
        AddAccent("Finnish", "hame", "in the Häme dialect", "in a Finnish dialect", Difficulty.VeryEasy,
            "The Häme dialect, from the central inland region, contributing to features of later standard Finnish",
            "finnish");
        AddAccent("Finnish", "savonian", "in the Savonian dialect", "in a Finnish dialect", Difficulty.VeryEasy,
            "The Savonian dialect, spoken in the eastern woodlands, known for characteristic vowel shifts",
            "finnish");
        AddAccent("Finnish", "karelian", "in a Karelian dialect", "in a Finnish dialect", Difficulty.VeryEasy,
            "A Finnish dialect influenced by neighboring Karelian varieties in the east",
            "finnish");
        AddAccent("Finnish", "swedish", "with a Swedish accent", "in a foreign dialect", Difficulty.Normal,
            "A Swedish speaker’s rendition of the Finnish language",
            "foreign");
        AddAccent("Finnish", "russian", "with a Russian accent", "in a foreign dialect", Difficulty.Normal,
            "A Russian speaker’s rendition of the Finnish language",
            "foreign");
        AddAccent("Finnish", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Finnish language",
            "foreign");
        AddAccent("Finnish", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Finnish language",
            "foreign");

        AddAccent("Hungarian", "transdanubian", "in the Transdanubian dialect", "in a Hungarian dialect", Difficulty.Trivial,
            "The Transdanubian dialect, spoken west of the Danube, showing conservative vowel harmony patterns",
            "hungarian");
        AddAccent("Hungarian", "paloc", "in the Palóc dialect", "in a Hungarian dialect", Difficulty.VeryEasy,
            "The Palóc dialect, found in northern regions, known for certain archaic phonetic features",
            "hungarian");
        AddAccent("Hungarian", "great-plain", "in the Great Plain dialect", "in a Hungarian dialect", Difficulty.VeryEasy,
            "The Great Plain dialect, spoken across the central plains, forming a core area of Hungarian speech",
            "hungarian");
        AddAccent("Hungarian", "northeastern", "in the Northeastern dialect", "in a Hungarian dialect", Difficulty.VeryEasy,
            "The Northeastern dialect, bordering Slavic-speaking lands, reflecting slight external influences",
            "hungarian");
        AddAccent("Hungarian", "slavic", "with a Slavic accent", "in a foreign dialect", Difficulty.Normal,
            "A Slavic speaker’s rendition of the Hungarian language",
            "foreign");
        AddAccent("Hungarian", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Hungarian language",
            "foreign");
        AddAccent("Hungarian", "turkish", "with a Turkish accent", "in a foreign dialect", Difficulty.Normal,
            "A Turkish speaker’s rendition of the Hungarian language",
            "foreign");
        AddAccent("Hungarian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Hungarian language",
            "foreign");

        AddAccent("Estonian", "north-estonian", "in the North Estonian dialect", "in an Estonian dialect", Difficulty.Trivial,
            "The North Estonian dialect, forming the core foundation of later standard Estonian, spoken around Tallinn",
            "estonian");
        AddAccent("Estonian", "south-estonian", "in the South Estonian dialect", "in an Estonian dialect", Difficulty.VeryEasy,
            "The South Estonian dialect, with its own sub-varieties and more archaic features, spoken in the southern regions",
            "estonian");
        AddAccent("Estonian", "finnish", "with a Finnish accent", "in a foreign dialect", Difficulty.Normal,
            "A Finnish speaker’s rendition of the Estonian language",
            "foreign");
        AddAccent("Estonian", "german", "with a German accent", "in a foreign dialect", Difficulty.Normal,
            "A German speaker’s rendition of the Estonian language",
            "foreign");
        AddAccent("Estonian", "russian", "with a Russian accent", "in a foreign dialect", Difficulty.Normal,
            "A Russian speaker’s rendition of the Estonian language",
            "foreign");
        AddAccent("Estonian", "swedish", "with a Swedish accent", "in a foreign dialect", Difficulty.Normal,
            "A Swedish speaker’s rendition of the Estonian language",
            "foreign");
        AddAccent("Estonian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Estonian language",
            "foreign");

        AddAccent("Karelian", "north-karelian", "in the North Karelian dialect", "in a Karelian dialect", Difficulty.Trivial,
            "The North Karelian dialect, spoken in northern Karelian territories, closely related to Eastern Finnish varieties",
            "karelian");
        AddAccent("Karelian", "south-karelian", "in the South Karelian dialect", "in a Karelian dialect", Difficulty.VeryEasy,
            "The South Karelian dialect, used in southern areas, reflecting some influence from Slavic neighbors",
            "karelian");
        AddAccent("Karelian", "ludic", "in the Ludic dialect", "in a Karelian dialect", Difficulty.VeryEasy,
            "The Ludic dialect, a transitional variety blending features of Karelian and Veps",
            "karelian");
        AddAccent("Karelian", "finnish", "with a Finnish accent", "in a foreign dialect", Difficulty.Normal,
            "A Finnish speaker’s rendition of the Karelian language",
            "foreign");
        AddAccent("Karelian", "russian", "with a Russian accent", "in a foreign dialect", Difficulty.Normal,
            "A Russian speaker’s rendition of the Karelian language",
            "foreign");
        AddAccent("Karelian", "estonian", "with an Estonian accent", "in a foreign dialect", Difficulty.Normal,
            "An Estonian speaker’s rendition of the Karelian language",
            "foreign");
        AddAccent("Karelian", "swedish", "with a Swedish accent", "in a foreign dialect", Difficulty.Normal,
            "A Swedish speaker’s rendition of the Karelian language",
            "foreign");
        AddAccent("Karelian", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Karelian language",
            "foreign");

        AddAccent("Irish Gaelic", "munster", "in the Munster dialect", "in an Irish dialect", Difficulty.Trivial,
            "The Munster dialect, spoken in the southern provinces, known for distinctive vowel length and intonation",
            "irish gaelic");
        AddAccent("Irish Gaelic", "connacht", "in the Connacht dialect", "in an Irish dialect", Difficulty.VeryEasy,
            "The Connacht dialect, spoken in the western regions, balancing features between Munster and Ulster",
            "irish gaelic");
        AddAccent("Irish Gaelic", "ulster", "in the Ulster dialect", "in an Irish dialect", Difficulty.VeryEasy,
            "The Ulster dialect, spoken in the northern areas, with some phonetic traits linking it to Scots Gaelic",
            "irish gaelic");

        AddAccent("Irish Gaelic", "english", "with an English accent", "in a foreign dialect", Difficulty.Normal,
            "An English speaker’s rendition of the Irish Gaelic language",
            "foreign");
        AddAccent("Irish Gaelic", "scottish gaelic", "with a Scottish Gaelic accent", "in a foreign dialect", Difficulty.Normal,
            "A Scottish Gaelic speaker’s rendition of the Irish Gaelic language",
            "foreign");
        AddAccent("Irish Gaelic", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal,
            "A French speaker’s rendition of the Irish Gaelic language",
            "foreign");
        AddAccent("Irish Gaelic", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Irish Gaelic language",
            "foreign");

        AddAccent("Scottish Gaelic", "northern", "in the Northern dialect", "in a Scottish dialect", Difficulty.Trivial,
            "The Northern dialect, spoken in the Highlands and Islands, preserving older Gaelic phonetic features",
            "scottish gaelic");
        AddAccent("Scottish Gaelic", "central", "in the Central dialect", "in a Scottish dialect", Difficulty.VeryEasy,
            "The Central dialect, a transitional variety bridging northern and southern Gaelic speech",
            "scottish gaelic");
        AddAccent("Scottish Gaelic", "southern", "in the Southern dialect", "in a Scottish dialect", Difficulty.VeryEasy,
            "The Southern dialect, found in certain Hebridean islands and coastal areas",
            "scottish gaelic");
        AddAccent("Scottish Gaelic", "irish", "with an Irish accent", "in a foreign dialect", Difficulty.Normal,
            "An Irish Gaelic speaker’s rendition of the Scottish Gaelic language",
            "foreign");
        AddAccent("Scottish Gaelic", "english", "with an English accent", "in a foreign dialect", Difficulty.Normal,
            "An English speaker’s rendition of the Scottish Gaelic language",
            "foreign");
        AddAccent("Scottish Gaelic", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal,
            "A French speaker’s rendition of the Scottish Gaelic language",
            "foreign");
        AddAccent("Scottish Gaelic", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Scottish Gaelic language",
            "foreign");

        AddAccent("Manx", "manx", "in the Manx dialect", "in a Manx dialect", Difficulty.Trivial,
            "The Manx dialect, reflecting a Gaelic language variety unique to the Isle of Man",
            "manx");
        AddAccent("Manx", "irish", "with an Irish accent", "in a foreign dialect", Difficulty.Normal,
            "An Irish Gaelic speaker’s rendition of the Manx language",
            "foreign");
        AddAccent("Manx", "scottish gaelic", "with a Scottish Gaelic accent", "in a foreign dialect", Difficulty.Normal,
            "A Scottish Gaelic speaker’s rendition of the Manx language",
            "foreign");
        AddAccent("Manx", "english", "with an English accent", "in a foreign dialect", Difficulty.Normal,
            "An English speaker’s rendition of the Manx language",
            "foreign");
        AddAccent("Manx", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Manx language",
            "foreign");

        AddAccent("Breton", "gwenedeg", "in the Gwenedeg dialect", "in a Breton dialect", Difficulty.Trivial,
            "The Gwenedeg dialect, spoken in the Vannes region, known for distinctive vowel pronunciations",
            "breton");
        AddAccent("Breton", "kerneveg", "in the Kerneveg (Cornouaille) dialect", "in a Breton dialect", Difficulty.VeryEasy,
            "The Kerneveg (Cornouaille) dialect, centered in southwestern Brittany",
            "breton");
        AddAccent("Breton", "leoneg", "in the Leoneg (Léon) dialect", "in a Breton dialect", Difficulty.VeryEasy,
            "The Leoneg (Léon) dialect, spoken in northwestern Brittany, often considered prestigious",
            "breton");
        AddAccent("Breton", "tregerieg", "in the Tregerieg (Trégor) dialect", "in a Breton dialect", Difficulty.VeryEasy,
            "The Tregerieg (Trégor) dialect, found in the northern Brittany area",
            "breton");
        AddAccent("Breton", "french", "with a French accent", "in a foreign dialect", Difficulty.Normal,
            "A French speaker’s rendition of the Breton language",
            "foreign");
        AddAccent("Breton", "english", "with an English accent", "in a foreign dialect", Difficulty.Normal,
            "An English speaker’s rendition of the Breton language",
            "foreign");
        AddAccent("Breton", "occitan", "with an Occitan accent", "in a foreign dialect", Difficulty.Normal,
            "An Occitan speaker’s rendition of the Breton language",
            "foreign");
        AddAccent("Breton", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Breton language",
            "foreign");

        AddAccent("Cornish", "cornish", "in the Cornish dialect", "in a Cornish dialect", Difficulty.Trivial,
            "The Cornish dialect, reflecting the Brythonic Celtic language of Cornwall",
            "cornish");
        AddAccent("Cornish", "english", "with an English accent", "in a foreign dialect", Difficulty.Normal,
            "An English speaker’s rendition of the Cornish language",
            "foreign");
        AddAccent("Cornish", "breton", "with a Breton accent", "in a foreign dialect", Difficulty.Normal,
            "A Breton speaker’s rendition of the Cornish language",
            "foreign");
        AddAccent("Cornish", "welsh", "with a Welsh accent", "in a foreign dialect", Difficulty.Normal,
            "A Welsh speaker’s rendition of the Cornish language",
            "foreign");
        AddAccent("Cornish", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Cornish language",
            "foreign");

        AddAccent("Romani", "vlax", "in the Vlax dialect", "in a Romani dialect", Difficulty.Trivial,
            "The Vlax dialect, influenced by Romanian languages due to the historical settlement in Wallachia and Moldavia",
            "romani");
        AddAccent("Romani", "balkan", "in the Balkan dialect", "in a Romani dialect", Difficulty.VeryEasy,
            "The Balkan dialect, spoken in southeastern Europe, reflecting contact with Greek, Slavic, and Turkish varieties",
            "romani");
        AddAccent("Romani", "central", "in the Central dialect", "in a Romani dialect", Difficulty.VeryEasy,
            "The Central dialect, found in parts of Central Europe, influenced by Germanic and West Slavic languages",
            "romani");
        AddAccent("Romani", "northern", "in the Northern dialect", "in a Romani dialect", Difficulty.VeryEasy,
            "The Northern dialect, spoken in northern and northeastern Europe, showing influence from Baltic and East Slavic languages",
            "romani");
        AddAccent("Romani", "slavic", "with a Slavic accent", "in a foreign dialect", Difficulty.Normal,
            "A Slavic speaker’s rendition of the Romani language",
            "foreign");
        AddAccent("Romani", "germanic", "with a Germanic accent", "in a foreign dialect", Difficulty.Normal,
            "A Germanic speaker’s rendition of the Romani language",
            "foreign");
        AddAccent("Romani", "romance", "with a Romance accent", "in a foreign dialect", Difficulty.Normal,
            "A Romance speaker’s rendition of the Romani language",
            "foreign");
        AddAccent("Romani", "balkan", "with a Balkan accent", "in a foreign dialect", Difficulty.Normal,
            "A Balkan speaker’s rendition of the Romani language",
            "foreign");
        AddAccent("Romani", "foreign", "with a foreign accent", "in a foreign dialect", Difficulty.Normal,
            "A generic non-native speaker’s rendition of the Romani language",
            "foreign");

        AddAccent("Latin", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("French", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Occitan", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Italian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Lombard", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Venetian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Neapolitan", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Sicilian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Castilian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Aragonese", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Leonese", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Galician", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Wallachian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("English", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("High German", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Low German", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Dutch", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Frisian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Yiddish", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Swedish", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Norwegian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Icelandic", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Danish", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Russian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Ruthenian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Church Slavonic", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Polish", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Slovak", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Wendish", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Czech", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Prussian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Lithuanian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Latvian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Serbo-Croatian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Slovenian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Bulgarian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Quranic Arabic", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Mahgrebi Arabic", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Mashriqi Arabic", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Aramaic", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Berber", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Hebrew", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Greek", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Koine Greek", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Persian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Armenian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Coptic", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Irish Gaelic", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Scottish Gaelic", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Welsh", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Manx", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Breton", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Cornish", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Turkish", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Finnish", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Hungarian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Estonian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Karelian", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Romani", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        AddAccent("Basque", "crude", "with a crude accent", "with a crude accent", Difficulty.Hard, "A crude accent, typical of someone who has recently learned the language and not yet mastered its use", "crude");
        #endregion

        #region Mutual Intelligibilities
        AddMutualIntelligability("French", "Latin", Difficulty.Insane, false);
        AddMutualIntelligability("Occitan", "Latin", Difficulty.Insane, false);
        AddMutualIntelligability("French", "Occitan", Difficulty.VeryHard, true);

        AddMutualIntelligability("Italian", "Lombard", Difficulty.Hard, true);
        AddMutualIntelligability("Italian", "Venetian", Difficulty.Hard, true);
        AddMutualIntelligability("Italian", "Neapolitan", Difficulty.Hard, true);
        AddMutualIntelligability("Italian", "Sicilian", Difficulty.VeryHard, true);
        AddMutualIntelligability("Lombard", "Venetian", Difficulty.VeryHard, true);
        AddMutualIntelligability("Lombard", "Neapolitan", Difficulty.VeryHard, true);
        AddMutualIntelligability("Lombard", "Sicilian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Venetian", "Neapolitan", Difficulty.VeryHard, true);
        AddMutualIntelligability("Venetian", "Sicilian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Neapolitan", "Sicilian", Difficulty.VeryHard, true);

        AddMutualIntelligability("Castilian", "Aragonese", Difficulty.VeryHard, false);
        AddMutualIntelligability("Castilian", "Leonese", Difficulty.VeryHard, false);
        AddMutualIntelligability("Castilian", "Galician", Difficulty.ExtremelyHard, false);
        AddMutualIntelligability("Aragonese", "Leonese", Difficulty.VeryHard, false);
        AddMutualIntelligability("Aragonese", "Galician", Difficulty.ExtremelyHard, false);
        AddMutualIntelligability("Leonese", "Galician", Difficulty.ExtremelyHard, false);

        AddMutualIntelligability("Italian", "Latin", Difficulty.VeryHard, false);
        AddMutualIntelligability("Lombard", "Latin", Difficulty.VeryHard, false);
        AddMutualIntelligability("Venetian", "Latin", Difficulty.VeryHard, false);
        AddMutualIntelligability("Neapolitan", "Latin", Difficulty.VeryHard, false);
        AddMutualIntelligability("Sicilian", "Latin", Difficulty.VeryHard, false);
        AddMutualIntelligability("Castilian", "Latin", Difficulty.ExtremelyHard, false);
        AddMutualIntelligability("Aragonese", "Latin", Difficulty.ExtremelyHard, false);
        AddMutualIntelligability("Leonese", "Latin", Difficulty.ExtremelyHard, false);
        AddMutualIntelligability("Galician", "Latin", Difficulty.ExtremelyHard, false);
        AddMutualIntelligability("Wallachian", "Latin", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "French", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Occitan", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Italian", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Lombard", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Venetian", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Neapolitan", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Sicilian", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Castilian", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Aragonese", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Leonese", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Galician", Difficulty.Insane, false);
        AddMutualIntelligability("Latin", "Wallachian", Difficulty.Insane, false);

        AddMutualIntelligability("English", "Frisian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("English", "Dutch", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Dutch", "Low German", Difficulty.VeryHard, true);
        AddMutualIntelligability("Dutch", "High German", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Low German", "High German", Difficulty.VeryHard, true);
        AddMutualIntelligability("English", "Low German", Difficulty.Insane, true);
        AddMutualIntelligability("English", "High German", Difficulty.Insane, true);
        AddMutualIntelligability("Yiddish", "Low German", Difficulty.VeryHard, true);
        AddMutualIntelligability("Yiddish", "High German", Difficulty.Hard, true);

        AddMutualIntelligability("Russian", "Ruthenian", Difficulty.VeryHard, true);
        AddMutualIntelligability("Russian", "Church Slavonic", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Russian", "Polish", Difficulty.Insane, true);
        AddMutualIntelligability("Russian", "Slovak", Difficulty.Insane, true);
        AddMutualIntelligability("Russian", "Czech", Difficulty.Insane, true);
        AddMutualIntelligability("Russian", "Wendish", Difficulty.Insane, true);
        AddMutualIntelligability("Russian", "Prussian", Difficulty.Insane, true);
        AddMutualIntelligability("Ruthenian", "Church Slavonic", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Ruthenian", "Polish", Difficulty.Insane, true);
        AddMutualIntelligability("Ruthenian", "Slovak", Difficulty.Insane, true);
        AddMutualIntelligability("Ruthenian", "Czech", Difficulty.Insane, true);
        AddMutualIntelligability("Ruthenian", "Wendish", Difficulty.Insane, true);
        AddMutualIntelligability("Ruthenian", "Prussian", Difficulty.Insane, true);
        AddMutualIntelligability("Church Slavonic", "Polish", Difficulty.Insane, true);
        AddMutualIntelligability("Church Slavonic", "Slovak", Difficulty.Insane, true);
        AddMutualIntelligability("Church Slavonic", "Czech", Difficulty.Insane, true);
        AddMutualIntelligability("Church Slavonic", "Wendish", Difficulty.Insane, true);
        AddMutualIntelligability("Church Slavonic", "Prussian", Difficulty.Insane, true);
        AddMutualIntelligability("Czech", "Slovak", Difficulty.Hard, true);
        AddMutualIntelligability("Czech", "Wendish", Difficulty.VeryHard, true);
        AddMutualIntelligability("Czech", "Prussian", Difficulty.VeryHard, true);
        AddMutualIntelligability("Czech", "Polish", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Polish", "Slovak", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Polish", "Wendish", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Polish", "Prussian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Slovak", "Wendish", Difficulty.VeryHard, true);
        AddMutualIntelligability("Slovak", "Prussian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Wendish", "Prussian", Difficulty.Insane, true);

        AddMutualIntelligability("Lithuanian", "Latvian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Lithuanian", "Prussian", Difficulty.Insane, true);
        AddMutualIntelligability("Latvian", "Prussian", Difficulty.Insane, true);

        AddMutualIntelligability("Serbo-Croatian", "Slovenian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Serbo-Croatian", "Bulgarian", Difficulty.Insane, true);
        AddMutualIntelligability("Slovenian", "Bulgarian", Difficulty.Insane, true);

        AddMutualIntelligability("Quranic Arabic", "Mahgrebi Arabic", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Quranic Arabic", "Mashriqi Arabic", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Mashriqi Arabic", "Mahgrebi Arabic", Difficulty.VeryHard, true);
        AddMutualIntelligability("Quranic Arabic", "Hebrew", Difficulty.Insane, true);
        AddMutualIntelligability("Quranic Arabic", "Aramaic", Difficulty.Insane, true);
        AddMutualIntelligability("Aramaic", "Hebrew", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Greek", "Koine Greek", Difficulty.ExtremelyHard, true);

        AddMutualIntelligability("Irish Gaelic", "Scottish Gaelic", Difficulty.VeryHard, true);
        AddMutualIntelligability("Irish Gaelic", "Manx", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Scottish Gaelic", "Manx", Difficulty.ExtremelyHard, true);

        AddMutualIntelligability("Breton", "Cornish", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Welsh", "Cornish", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Breton", "Welsh", Difficulty.ExtremelyHard, true);

        AddMutualIntelligability("Finnish", "Estonian", Difficulty.Insane, true);
        AddMutualIntelligability("Finnish", "Karelian", Difficulty.Insane, true);
        AddMutualIntelligability("Estonian", "Karelian", Difficulty.Insane, true);

        AddMutualIntelligability("Norwegian", "Swedish", Difficulty.VeryHard, true);
        AddMutualIntelligability("Norwegian", "Danish", Difficulty.Hard, true);
        AddMutualIntelligability("Norwegian", "Icelandic", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Swedish", "Danish", Difficulty.VeryHard, true);
        AddMutualIntelligability("Swedish", "Icelandic", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Danish", "Icelandic", Difficulty.ExtremelyHard, true);

        SeedRenaissanceEuropeLanguageCoverage();

        #endregion
    }
	private void SeedRenaissanceEuropeLanguageCoverage()
	{
		SeedHistoricalLanguagePack(
			RenaissanceEuropeLanguageCoverage,
			RenaissanceEuropeScriptCoverage,
			[]);
	}

	private static readonly HistoricalLanguageSeed[] RenaissanceEuropeLanguageCoverage =
	[
		new("Albanian", "an unknown Albanian language",
		[
			HistoricalAccent("Gheg", "northern Albanian", "The northern Albanian dialect continuum associated with Gheg speakers.", Difficulty.Trivial),
			HistoricalAccent("Tosk", "southern Albanian", "The southern Albanian dialect continuum associated with Tosk speakers.", Difficulty.Easy),
			HistoricalAccent("Arberesh", "Italo-Albanian", "The Albanian variety carried to southern Italy by late-medieval migrants.", Difficulty.Easy)
		])
	];

	private static readonly HistoricalScriptSeed[] RenaissanceEuropeScriptCoverage =
	[
		new("Latin", "the Latin script", "a Western alphabetic script",
			"Latin letters were used for some of the earliest surviving Albanian writing in Catholic contexts.",
			"Alphabet", 1.0, 1.0, ["Albanian"]),
		new("Greek", "the Greek script", "an Eastern Mediterranean alphabet",
			"Greek letters were also used for Albanian in Orthodox and southern contexts.",
			"Alphabet", 1.0, 1.0, ["Albanian"])
	];
}
