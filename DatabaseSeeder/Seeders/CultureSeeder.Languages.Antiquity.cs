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
    private void SeedRomanLanguages()
    {
        #region Language

        AddLanguage("Latin", "an unknown italic language");
        AddLanguage("Koine Greek", "an unknown greek language");
        AddLanguage("Attic Greek", "an unknown greek language");
        AddLanguage("Ionic Greek", "an unknown greek language");
        AddLanguage("Doric Greek", "an unknown greek language");
        AddLanguage("Aeolic Greek", "an unknown greek language");
        AddLanguage("Etruscan", "an unknown italic language");
        AddLanguage("Oscan", "an unknown italic language");
        AddLanguage("Umbrian", "an unknown italic language");
        AddLanguage("Venetic", "an unknown italic language");
        AddLanguage("Illyrian", "an unknown illyrian language");
        AddLanguage("Thracian", "an unknown illyrian language");
        AddLanguage("Dacian", "an unknown illyrian language");
        AddLanguage("Phoenician", "an unknown asian language");
        AddLanguage("Gaulish", "an unknown celtic language");
        AddLanguage("Ligurian", "an unknown celtic language");
        AddLanguage("Raetic", "an unknown celtic language");
        AddLanguage("Brittonic", "an unknown celtic language");
        AddLanguage("Goidelic", "an unknown celtic language");
        AddLanguage("Pictish", "an unknown celtic language");
        AddLanguage("Celtiberian", "an unknown celtic language");
        AddLanguage("Gallaecian", "an unknown celtic language");
        AddLanguage("Galatian", "an unknown celtic language");
        AddLanguage("Iberian", "an unknown vasconic language");
        AddLanguage("Tartessian", "an unknown vasconic language");
        AddLanguage("Aquitanian", "an unknown vasconic language");
        AddLanguage("Nuragic", "an unknown vasconic language");
        AddLanguage("Corsican", "an unknown vasconic language");
        AddLanguage("Coptic", "an unknown asian language");
        AddLanguage("Demotic", "an unknown asian language");
        AddLanguage("Aramaic", "an unknown asian language");
        AddLanguage("Numidian", "an unknown african language");
        AddLanguage("Phrygian", "an unknown anatolian language");
        AddLanguage("Anatolian", "an unknown anatolian language");
        AddLanguage("Arabic", "an unknown african language");
        AddLanguage("Hebrew", "an unknown african language");
        AddLanguage("Parthian", "an unknown asian language");
        AddLanguage("Belgic", "an unknown germanic language");
        AddLanguage("Germanic", "an unknown germanic language");
        AddLanguage("Ariya", "an unknown asian language");
        AddLanguage("Classical Egyptian", "an unknown african language");

        #endregion

        #region Scripts

        AddScript("Latin", "the Latin script", "a phoenician-derived script",
            "The Latin script is the script that the Romans use to write the Latin language. It is derived from the closely related Etruscan script. It is also often applied to the languages of subjects and neighbours as they Latinize.",
            "Phoenician Alphabetic", 1.0, 1.0, "Latin", "Etruscan", "Oscan", "Umbrian", "Venetic", "Illyrian",
            "Thracian", "Dacian", "Phoenician", "Gaulish", "Ligurian", "Raetic", "Brittonic", "Goidelic", "Pictish",
            "Celtiberian", "Gallaecian", "Galatian", "Iberian", "Tartessian", "Aquitanian", "Nuragic", "Corsican",
            "Belgic", "Germanic");
        AddScript("Greek", "the Greek script", "a phoenician-derived script",
            "The Greek script is a direct derivation from the Phoenician script, and in turn the inspiration for most other alphabetic scripts. Many other languages are sometimes or always written in Greek due to the process of Hellenization that followed Alexander the Great's conquests.",
            "Phoenician Alphabetic", 1.0, 1.0, "Koine Greek", "Attic Greek", "Ionic Greek", "Doric Greek",
            "Aeolic Greek", "Illyrian", "Thracian", "Dacian", "Coptic", "Galatian", "Phrygian", "Anatolian");
        AddScript("Phrygian", "the Phrygian script", "a phoenician-derived script",
            "The Phrygian script is an alphabetic script used for the language of the same name using Greek-looking symbols but with often different values for the sounds.",
            "Phoenician Alphabetic", 1.0, 1.0, "Phrygian");
        AddScript("Lydian", "the Lydian script", "a phoenician-derived script",
            "The Lydian script is an alphabetic script used for the language of the same name using Greek-looking symbols but with often different values for the sounds.",
            "Phoenician Alphabetic", 1.0, 1.0, "Anatolian");
        AddScript("Carian", "the Carian script", "a phoenician-derived script",
            "The Carian script is an alphabetic script used for the Anatolian dialect of the same name.",
            "Phoenician Alphabetic", 1.0, 1.0, "Anatolian");
        AddScript("Sidetic", "the Sidetic script", "a phoenician-derived script",
            "The Sidetic script is an alphabetic script used for the language of the same name using Greek-looking symbols but with often different values for the sounds.",
            "Phoenician Alphabetic", 1.0, 1.0, "Anatolian");
        AddScript("Etruscan", "the Etruscan script", "a phoenician-derived alphabetic script",
            "The Etruscan script is a similar script to Latin, derived from Greek.", "Phoenician Alphabetic", 1.0, 1.0,
            "Etruscan", "Oscan", "Umbrian", "Raetic");
        AddScript("Aramaic", "the Aramaic script", "a phoenician-derived script",
            "The Aramaic script is a form of Phoenician designed, an Abjad, which is used extensively in the Levantine to write various triconsonantal languages.",
            "Phoenician Abjad", 0.8, 1.2, "Aramaic", "Hebrew", "Coptic", "Parthian", "Arabic", "Ariya");
        AddScript("Syriac", "the Syriac script", "a phoenician-derived script",
            "The Syriac script is a cursive form of Aramaic designed to write the dialect of the Syriac people, but which is widely applied to other languages of the Fertile Crescent.",
            "Phoenician Abjad", 0.8, 1.2, "Aramaic", "Hebrew", "Parthian", "Arabic", "Ariya");
        AddScript("Nabatean", "the Nabatean script", "a phoenician-derived script",
            "The Nabatean script is an abjad based on Aramaic that is used in the Kingdom of Nabatea to write their dialect of Arabic.",
            "Phoenician Abjad", 0.8, 1.2, "Arabic");
        AddScript("Phoenician", "the Phoenician script", "a phoenician-derived script",
            "The Phoenician script is an abjad originally based on modified Egyptian hieroglyphs that influenced almost all of the writing systems in Europe, Asia and Africa. It is designed to write triconsonantal languages and therefore does not contain vowels. In Hebrew, this script is usually called \"Old Hebrew\" and in Aramaic it is known as \"Samaritan\".",
            "Phoenician Abjad", 0.8, 1.2, "Phoenician", "Hebrew", "Aramaic");
        AddScript("Ashurit", "the Ashurit script", "a phoenician-derived script",
            "The Ashurit script is an abjad used to write the Hebrew language, sometimes also known as Hebrew Square script.",
            "Phoenician Abjad", 0.8, 1.2, "Hebrew");
        AddScript("Coptic", "the Coptic script", "a phoenician-derived script",
            "The Coptic script is derived from the Greek script and is used to write the Coptic language in Egypt.",
            "Phoenician Alphabetic", 1.0, 1.0, "Coptic");
        AddScript("Tartesian", "the Tartesian script", "a phoenician-derived script",
            "This script, related to Phoenician, was used to write the Tartessian language prior to the Romanization of this area.",
            "Phoenician Alphabetic", 1.0, 1.0, "Tartessian");
        AddScript("Meridional", "the Meridional script", "a phoenician-derived script",
            "This script, related to Phoenician and possibly Ionic Greek was used to write the Iberian language prior to the Romanization of this area.",
            "Phoenician Alphabetic", 1.0, 1.0, "Iberian");
        AddScript("Levantine Iberian", "the Levantine Iberian script", "a phoenician-derived script",
            "The Levantine Iberian script, also known as the Northern Iberian script, was used to write both the Iberian and Celtiberian languages prior to the Romanisation of this area.",
            "Phoenician Alphabetic", 1.0, 1.0, "Iberian", "Celtiberian");
        AddScript("Greco-Iberian", "the Greco-Iberian script", "a phoenician-derived script",
            "The Greco-Iberian script was a script adapted from a subset of Ionic Greek with several additions that was used to write the Iberian language, in addition to its other native scripts.",
            "Phoenician Alphabetic", 1.0, 1.0, "Iberian");

        AddScript("Neo-Assyrian Cuneiform", "the Neo-Assyrian Cuneiform script", "a cuneiform script",
            "Neo-Assyrian Cuneiform was originally designed to write the Akkadian language but largely survives in educated circles in Parthia, where it is used to write Parthian.",
            "Cuneiform", 1.5, 1.0, "Parthian", "Ariya");
        AddScript("Persian Cuneiform", "the Persian Cuneiform script", "a cuneiform script",
            "Persian Cuneiform is an archaic way of writing the Ariyan language (also known as Old Persian).",
            "Cuneiform", 1.5, 1.0, "Parthian", "Ariya");

        AddScript("Egyptian Hieroglyhpic", "the Egyptian Hieroglyhpic script", "a hieroglyphic script",
            "Egyptian Hieroglyphs are an archaic writing form still seen in monumental form in Egypt. It is a largely logographic system but it does have some phoenetic elements.",
            "Hieroglyphic", 0.8, 1.2, "Classical Egyptian");
        AddScript("Hieratic", "the Hieratic script", "a hieroglyphic script",
            "Hieratic is an archaic curvsive written form of Egyptian Hieroglyphs used by scholars in the late classical period.",
            "Hieroglyphic", 0.8, 1.2, "Classical Egyptian");
        AddScript("Demotic", "the Demotic script", "a hieroglyphic script",
            "Demotic is an semi-archaic evolution of the earlier Hieratic script, which primarily survives in formal writing and scribes writing both the Demotic and Coptic languages.",
            "Hieroglyphic", 0.8, 1.2, "Classical Egyptian", "Demotic", "Coptic");

        #endregion

        #region Mutual Intelligibilities

        AddMutualIntelligability("Oscan", "Umbrian", Difficulty.VeryHard, true);
        AddMutualIntelligability("Oscan", "Venetic", Difficulty.VeryHard, true);
        AddMutualIntelligability("Umbrian", "Venetic", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Latin", "Venetic", Difficulty.Insane, true);
        AddMutualIntelligability("Latin", "Umbrian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Latin", "Oscan", Difficulty.ExtremelyHard, true);

        AddMutualIntelligability("Aquitanian", "Iberian", Difficulty.Insane, true);
        AddMutualIntelligability("Aquitanian", "Tartessian", Difficulty.Insane, true);
        AddMutualIntelligability("Iberian", "Tartessian", Difficulty.VeryHard, true);

        AddMutualIntelligability("Koine Greek", "Ionic Greek", Difficulty.Hard, true);
        AddMutualIntelligability("Koine Greek", "Doric Greek", Difficulty.Hard, true);
        AddMutualIntelligability("Koine Greek", "Aeolic Greek", Difficulty.Hard, true);
        AddMutualIntelligability("Ionic Greek", "Doric Greek", Difficulty.Hard, true);
        AddMutualIntelligability("Ionic Greek", "Aeolic Greek", Difficulty.Hard, true);
        AddMutualIntelligability("Doric Greek", "Aeolic Greek", Difficulty.Hard, true);
        AddMutualIntelligability("Koine Greek", "Attic Greek", Difficulty.Normal, true);
        AddMutualIntelligability("Ionic Greek", "Attic Greek", Difficulty.Hard, true);
        AddMutualIntelligability("Doric Greek", "Attic Greek", Difficulty.Hard, true);
        AddMutualIntelligability("Aeolic Greek", "Attic Greek", Difficulty.Hard, true);

        AddMutualIntelligability("Illyrian", "Thracian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Illyrian", "Dacian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Dacian", "Thracian", Difficulty.Hard, true);

        AddMutualIntelligability("Gaulish", "Brittonic", Difficulty.Hard, true);
        AddMutualIntelligability("Gaulish", "Ligurian", Difficulty.Hard, true);
        AddMutualIntelligability("Gaulish", "Raetic", Difficulty.Hard, true);
        AddMutualIntelligability("Gaulish", "Goidelic", Difficulty.VeryHard, true);
        AddMutualIntelligability("Gaulish", "Celtiberian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Gaulish", "Gallaecian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Gaulish", "Galatian", Difficulty.Insane, true);
        AddMutualIntelligability("Brittonic", "Ligurian", Difficulty.Hard, true);
        AddMutualIntelligability("Brittonic", "Raetic", Difficulty.Hard, true);
        AddMutualIntelligability("Brittonic", "Goidelic", Difficulty.Hard, true);
        AddMutualIntelligability("Brittonic", "Celtiberian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Brittonic", "Gallaecian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Brittonic", "Galatian", Difficulty.Insane, true);
        AddMutualIntelligability("Ligurian", "Raetic", Difficulty.Hard, true);
        AddMutualIntelligability("Ligurian", "Goidelic", Difficulty.VeryHard, true);
        AddMutualIntelligability("Ligurian", "Celtiberian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Ligurian", "Gallaecian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Ligurian", "Galatian", Difficulty.Insane, true);
        AddMutualIntelligability("Raetic", "Goidelic", Difficulty.VeryHard, true);
        AddMutualIntelligability("Raetic", "Celtiberian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Raetic", "Gallaecian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Raetic", "Galatian", Difficulty.Insane, true);
        AddMutualIntelligability("Goidelic", "Celtiberian", Difficulty.VeryHard, true);
        AddMutualIntelligability("Goidelic", "Gallaecian", Difficulty.VeryHard, true);
        AddMutualIntelligability("Goidelic", "Galatian", Difficulty.Insane, true);
        AddMutualIntelligability("Celtiberian", "Gallaecian", Difficulty.Hard, true);
        AddMutualIntelligability("Celtiberian", "Galatian", Difficulty.Insane, true);
        AddMutualIntelligability("Pictish", "Brittonic", Difficulty.Normal, true);
        AddMutualIntelligability("Pictish", "Gaulish", Difficulty.Hard, true);
        AddMutualIntelligability("Pictish", "Ligurian", Difficulty.Hard, true);
        AddMutualIntelligability("Pictish", "Raetic", Difficulty.Hard, true);
        AddMutualIntelligability("Pictish", "Goidelic", Difficulty.Normal, true);
        AddMutualIntelligability("Pictish", "Celtiberian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Pictish", "Gallaecian", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Pictish", "Galatian", Difficulty.Insane, true);

        AddMutualIntelligability("Belgic", "Germanic", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Belgic", "Gaulish", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Belgic", "Brittonic", Difficulty.Insane, true);

        AddMutualIntelligability("Ariya", "Parthian", Difficulty.Normal, true);
        AddMutualIntelligability("Coptic", "Demotic", Difficulty.Insane, true);
        AddMutualIntelligability("Demotic", "Classical Egyptian", Difficulty.Insane, true);

        #endregion

        #region Accents

        AddAccent("Latin", "Roman", "in the Roman dialect", "in a Roman dialect", Difficulty.Trivial,
            "This is Sermo Latinus or Sermo Familiaris, the \"correct\" Latin spoken by the Patrician class in Rome, and the variety of Latin that would later come to be understood to be \"Classical Latin\".",
            "Roman");
        AddAccent("Latin", "Vulgar Roman", "in the Vulgar Roman dialect", "in a Roman dialect", Difficulty.VeryEasy,
            "This variety of Latin is known as Sermo Vulgaris, or \"The speech of the masses\", and is the way that most plebian citizens of Rome spoke. It is looked down upon for \"good families\" to speak in this register.",
            "Roman");
        AddAccent("Latin", "Rustic Roman", "in a Rustic Roman dialect", "in a Roman dialect", Difficulty.VeryEasy,
            "This variety of Latin is known as Sermo Vulgaris Rustica, or \"The speech of the rural masses\", and is the way that plebians (and privately, some Patricians) who resided primarily in the countryside around Rome spoke.",
            "Roman");
        AddAccent("Latin", "Old Roman", "in the Old Roman dialect", "in a Roman dialect", Difficulty.VeryEasy,
            "This is an archaic register of the Patrician speech of Rome, with some distinct pronunciation differences. Few but the most conservative and elderly Roman gentlemen would speak in such a dialect, except perhaps to sound profound when quoting from a source from history.",
            "Roman");
        AddAccent("Latin", "Etruscan", "with an Etruscan accent", "in a Latin dialect", Difficulty.Easy,
            "This is the variety of Latin spoken by a native but Romanised Etruscan.", "Latin");
        AddAccent("Latin", "Oscan", "with an Oscan accent", "in a Latin dialect", Difficulty.Easy,
            "This is the variety of Latin spoken by a native but Romanised Oscan - someone from the Southern-Central part of Italy.",
            "Latin");
        AddAccent("Latin", "Umbrian", "with an Umbrian accent", "in a Latin dialect", Difficulty.Easy,
            "This is the variety of Latin spoken by a native but Romanised Umbrian - someone from the North-Eastern part of the Italian peninsular.",
            "Latin");
        AddAccent("Latin", "Sabine", "with a Sabine accent", "in a Latin dialect", Difficulty.Easy,
            "This is the variety of Latin spoken by a native but Romanised Sabine - neighbours and enemies of Rome from antiquity, long since subjugated.",
            "Latin");
        AddAccent("Latin", "Sardinian", "with a Sardinian accent", "in a provincal dialect", Difficulty.Easy,
            "This is the variety of Latin spoken by a native but Romanised Sardinian.", "Provincal");
        AddAccent("Latin", "Corsican", "with a Corsican accent", "in a provincal dialect", Difficulty.Normal,
            "This is the variety of Latin spoken by a native but Romanised Corsican.", "Provincal");
        AddAccent("Latin", "Sicilian", "with a Sicilian accent", "in a provincal dialect", Difficulty.Normal,
            "This is the variety of Latin spoken by a native but Romanised Sicilian.", "Provincal");
        AddAccent("Latin", "Gallic", "with a Gallic accent", "in a provincal dialect", Difficulty.Normal,
            "This is the variety of Latin spoken by a native but Romanised Gaul.", "Provincal");
        AddAccent("Latin", "German", "with a German accent", "in a provincal dialect", Difficulty.Normal,
            "This is the variety of Latin spoken by a native but Romanised German.", "Provincal");
        AddAccent("Latin", "Illyrian", "with an Illyrian accent", "in a provincal dialect", Difficulty.Normal,
            "This is the variety of Latin spoken by a native but Romanised Illyrian.", "Provincal");
        AddAccent("Latin", "Greek", "with a Greek accent", "in a provincal dialect", Difficulty.Normal,
            "This is the variety of Latin spoken by a native but Romanised Greek.", "Provincal");
        AddAccent("Latin", "Iberian", "with an Iberian accent", "in a provincal dialect", Difficulty.Normal,
            "This is the variety of Latin spoken by a native but Romanised Iberian.", "Provincal");
        AddAccent("Latin", "Egyptian", "with an Egyptian accent", "in a provincal dialect", Difficulty.Normal,
            "This is the variety of Latin spoken by a native but Romanised Egyptian.", "Provincal");
        AddAccent("Latin", "Punic", "with a Punic accent", "in a provincal dialect", Difficulty.Normal,
            "This is the variety of Latin spoken by a native but Romanised North African.", "Provincal");

        AddAccent("Classical Egyptian", "Standard", "with a Standard accent", "in a Standard dialect",
            Difficulty.Normal, "This is the form of the Classical Egyptian language that survives in writing.",
            "Standard");

        AddAccent("Etruscan", "Aretti", "with an Arettian accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Aretti.", "Urban");
        AddAccent("Etruscan", "Atria", "with an Atrian accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Atria.", "Urban");
        AddAccent("Etruscan", "Caisra", "with a Caisran accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Caisra.", "Urban");
        AddAccent("Etruscan", "Clevsi", "with a Clevsian accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Clevsi.", "Urban");
        AddAccent("Etruscan", "Curtun", "with a Curtunian accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Curtun.", "Urban");
        AddAccent("Etruscan", "Perusna", "with a Perusnaian accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Perusna.", "Urban");
        AddAccent("Etruscan", "Puplana", "with a Puplanian accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Puplana.", "Urban");
        AddAccent("Etruscan", "Tarchuna", "with a Trachunan accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Tarchuna.", "Urban");
        AddAccent("Etruscan", "Vatluna", "with a Vatlunan accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Vatluna.", "Urban");
        AddAccent("Etruscan", "Veia", "with a Veian accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Veia.", "Urban");
        AddAccent("Etruscan", "Velathri", "with a Velathrian accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Velathri.", "Urban");
        AddAccent("Etruscan", "Velch", "with a Velchian accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Velch.", "Urban");
        AddAccent("Etruscan", "Velzna", "with a Velznan accent", "in an urban dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those from the Etruscan city of Velzna.", "Urban");
        AddAccent("Etruscan", "Rural", "with a rural accent", "in a ruran dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those in Etruscan cultural areas from inland rural areas.",
            "Rural");
        AddAccent("Etruscan", "Coastal", "with a rural-coastal accent", "in a rural dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Etruscan spoken by those Etruscan cultural areas from coastal rural areas.",
            "Rural");

        AddAccent("Oscan", "Samnite", "with a Samnitic accent", "in a Samnitic accent", Difficulty.ExtremelyEasy,
            "This is the variety of Oscan spoken by members of the Samnite tribe.", "Samnite");
        AddAccent("Oscan", "Aurunci", "with a Auruncian accent", "in an Opician accent", Difficulty.ExtremelyEasy,
            "This is the variety of Oscan spoken by members of the Aurunci tribe.", "Opici");
        AddAccent("Oscan", "Sidicini", "with a Sidicinian accent", "in a Samnitic accent", Difficulty.ExtremelyEasy,
            "This is the variety of Oscan spoken by members of the Sidicini tribe.", "Samnite");
        AddAccent("Oscan", "Hernici", "with a Hernician accent", "in a Samnitic accent", Difficulty.ExtremelyEasy,
            "This is the variety of Oscan spoken by members of the Hernici tribe.", "Samnite");
        AddAccent("Oscan", "Marrucini", "with a Marrucinian accent", "in a Samnitic accent", Difficulty.ExtremelyEasy,
            "This is the variety of Oscan spoken by members of the Marrucini tribe.", "Samnite");
        AddAccent("Oscan", "Paeligni", "with a Paelignian accent", "in an Opician accent", Difficulty.ExtremelyEasy,
            "This is the variety of Oscan spoken by members of the Paeligni tribe.", "Opici");
        AddAccent("Oscan", "Campani", "with a Campanian accent", "in an Opician accent", Difficulty.ExtremelyEasy,
            "This is the variety of Oscan spoken by members of the Campani tribe.", "Opici");
        AddAccent("Oscan", "Bruttii", "with a Bruttian accent", "in a Lucanian accent", Difficulty.ExtremelyEasy,
            "This is the variety of Oscan spoken by members of the Bruttii tribe.", "Lucani");
        AddAccent("Oscan", "Itali", "with an Italian accent", "in a Lucanian accent", Difficulty.ExtremelyEasy,
            "This is the variety of Oscan spoken by members of the Itali tribe.", "Lucani");

        AddAccent("Umbrian", "Sabine", "with a Sabine accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Umbrian spoken by members of the Sabine tribe.", "Native");
        AddAccent("Umbrian", "Marsi", "with a Marsian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Umbrian spoken by members of the Marsi tribe.", "Native");
        AddAccent("Umbrian", "Volsci", "with a Volscian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Umbrian spoken by members of the Volsci tribe.", "Native");
        AddAccent("Umbrian", "Piceni", "with a Picenian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Umbrian spoken by members of the South Piceni tribe.", "Native");

        AddAccent("Venetic", "Veneti", "with a Venetian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Venetic spoken by members of the Veneti tribe.", "Native");
        AddAccent("Venetic", "Histri", "with a Histrian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Venetic spoken by members of the Histri tribe.", "Native");
        AddAccent("Venetic", "Carni", "with a Carnian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Venetic spoken by members of the Carni tribe.", "Native");
        AddAccent("Venetic", "Catari", "with a Catarian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Venetic spoken by members of the Catari tribe.", "Native");
        AddAccent("Venetic", "Catali", "with a Catalian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Venetic spoken by members of the Catali tribe.", "Native");
        AddAccent("Venetic", "Liburni", "with a Liburnian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Venetic spoken by members of the Liburni tribe.", "Native");
        AddAccent("Venetic", "Lopsi", "with a Lopsian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Venetic spoken by members of the Lopsi tribe.", "Native");
        AddAccent("Venetic", "Venetulani", "with a Venetulanian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the variety of Venetic spoken by members of the Venetulani tribe.", "Native");

        AddAccent("Nuragic", "Balari", "in a Balarian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Balari tribe in Sardinia.", "Native");
        AddAccent("Nuragic", "Iolei", "in a Ioleian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Iolei tribe in Sardinia.", "Native");
        AddAccent("Nuragic", "Corsi", "in a Corsian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Corsi tribe in Sardinia.", "Native");

        AddAccent("Corsican", "Sardinian", "in a Sardinian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Corsican speaking tribes in North-Eastern Sardinia.", "Native");
        AddAccent("Corsican", "Titiani", "in a Titianian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Titiani tribe in Corsica.", "Native");
        AddAccent("Corsican", "Belatoni", "in a Belatonian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Belatoni tribe in Corsica.", "Native");
        AddAccent("Corsican", "Subasani", "in a Subasanian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Subasani tribe in Corsica.", "Native");
        AddAccent("Corsican", "Cumanesi", "in a Cumanesian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Cumanesi tribe in Corsica.", "Native");
        AddAccent("Corsican", "Sumbri", "in a Sumbrian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Sumbri tribe in Corsica.", "Native");
        AddAccent("Corsican", "Lucinni", "in a Lucinnian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Lucinni tribe in Corsica.", "Native");
        AddAccent("Corsican", "Cervini", "in a Cervinian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Cervini tribe in Corsica.", "Native");
        AddAccent("Corsican", "Tarabeni", "in a Tarabenian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Tarabeni tribe in Corsica.", "Native");
        AddAccent("Corsican", "Opini", "in a Opinian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Opini tribe in Corsica.", "Native");
        AddAccent("Corsican", "Macrini", "in a Macrinian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Macrini tribe in Corsica.", "Native");
        AddAccent("Corsican", "Cilebensi", "in a Cilebensian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Cilebensi tribe in Corsica.", "Native");
        AddAccent("Corsican", "Venacini", "in a Venacinian accent", "in a native accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Venacini tribe in Corsica.", "Native");

        AddAccent("Thracian", "Thracian", "in a Thracian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Thracian tribe with the Thracian language.", "Western");
        AddAccent("Thracian", "Moesian", "in a Moesian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Moesian tribe with the Thracian language.", "Western");
        AddAccent("Thracian", "Macedonian", "in a Macedonian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Macedonian tribe with the Thracian language.", "Western");
        AddAccent("Thracian", "Dacian", "in a Dacian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Dacian tribe with the Thracian language.", "Western");
        AddAccent("Thracian", "Scythian", "in a Scythian accent", "in a Northern accent", Difficulty.Normal,
            "This is the native accent of the Lesser-Scythian tribe with the Thracian language.", "Northern");
        AddAccent("Thracian", "Sarmatian", "in a Sarmatian accent", "in an Eastern accent", Difficulty.Easy,
            "This is the native accent of the Sarmatian tribe with the Thracian language.", "Eastern");
        AddAccent("Thracian", "Bithynian", "in a Bithynian accent", "in an Eastern accent", Difficulty.Easy,
            "This is the native accent of the Bithynian tribe with the Thracian language.", "Eastern");
        AddAccent("Thracian", "Mysian", "in a Mysian accent", "in an Eastern accent", Difficulty.Easy,
            "This is the native accent of the Mysian tribe with the Thracian language, who are from Anatolia.",
            "Eastern");
        AddAccent("Thracian", "Pannonian", "in a Pannonian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Pannonian tribe with the Thracian language.", "Western");

        AddAccent("Dacian", "Dacian", "in a Dacian accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Dacian tribe with the Dacian language.", "Northern");
        AddAccent("Dacian", "Moesian", "in a Moesian accent", "in a Southern accent", Difficulty.VeryEasy,
            "This is the native accent of the Moesian tribe with the Dacian language.", "Southern");
        AddAccent("Dacian", "Getaen", "in a Getaen accent", "in a Southern accent", Difficulty.VeryEasy,
            "This is the native accent of the Getea tribe with the Dacian language.", "Southern");
        AddAccent("Dacian", "Tribalian", "in a Tribalian accent", "in a Southern accent", Difficulty.VeryEasy,
            "This is the native accent of the Tribalii tribe with the Dacian language.", "Southern");
        AddAccent("Dacian", "Carpian", "in a Carpian accent", "in an Eastern accent", Difficulty.Normal,
            "This is the native accent of the Carpi tribe with the Dacian language.", "Eastern");
        AddAccent("Dacian", "Costobocian", "in a Costobocian accent", "in an Eastern accent", Difficulty.Normal,
            "This is the native accent of the Costoboci tribe with the Dacian language.", "Eastern");

        AddAccent("Illyrian", "Taulantii", "in a Taulantian accent", "in an Illyrian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Taulantii tribe of the Illyrian kingdom.", "Illyrian");
        AddAccent("Illyrian", "Pleraei", "in a Pleraeian accent", "in an Illyrian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Pleraei tribe of the Illyrian kingdom.", "Illyrian");
        AddAccent("Illyrian", "Endirundini", "in a Endirundinian accent", "in an Illyrian accent",
            Difficulty.ExtremelyEasy, "This is the native accent of the Endirundini tribe of the Illyrian kingdom.",
            "Illyrian");
        AddAccent("Illyrian", "Sasaei", "in a Sasaeian accent", "in an Illyrian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Sasaei tribe of the Illyrian kingdom.", "Illyrian");
        AddAccent("Illyrian", "Grabaei", "in a Grabaeian accent", "in an Illyrian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Grabaei tribe of the Illyrian kingdom.", "Illyrian");
        AddAccent("Illyrian", "Labeatae", "in a Labeataean accent", "in an Illyrian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Labeatae tribe of the Illyrian kingdom.", "Illyrian");
        AddAccent("Illyrian", "Dalmatae", "in a Dalmatian accent", "in a Dalmatian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Dalmatae tribe of the Dalmatian Illyrian speakers.", "Dalmatian");
        AddAccent("Illyrian", "Daorsei", "in a Daorseian accent", "in a Dalmatian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Daorsei tribe of the Dalmatian Illyrian speakers.", "Dalmatian");
        AddAccent("Illyrian", "Daraemaestae", "in a Daraemaestaean accent", "in a Dalmatian accent",
            Difficulty.ExtremelyEasy,
            "This is the native accent of the Daraemaestae tribe of the Dalmatian Illyrian speakers.", "Dalmatian");
        AddAccent("Illyrian", "Derentini", "in a Derentinian accent", "in a Dalmatian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Derentini tribe of the Dalmatian Illyrian speakers.", "Dalmatian");
        AddAccent("Illyrian", "Deuri", "in a Deurian accent", "in a Dalmatian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Deuri tribe of the Dalmatian Illyrian speakers.", "Dalmatian");
        AddAccent("Illyrian", "Iapydes", "in a Iapydesian accent", "in a Dalmatian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Iapydes tribe of the Dalmatian Illyrian speakers.", "Dalmatian");
        AddAccent("Illyrian", "Melcumani", "in a Melcumanian accent", "in a Dalmatian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Melcumani tribe of the Dalmatian Illyrian speakers.", "Dalmatian");
        AddAccent("Illyrian", "Narensi", "in a Narensian accent", "in a Dalmatian accent", Difficulty.ExtremelyEasy,
            "This is the native accent of the Narensi tribe of the Dalmatian Illyrian speakers.", "Dalmatian");
        AddAccent("Illyrian", "Sardeates", "in a Sardeatesian accent", "in a Dalmatian accent",
            Difficulty.ExtremelyEasy,
            "This is the native accent of the Sardeates tribe of the Dalmatian Illyrian speakers.", "Dalmatian");
        AddAccent("Illyrian", "Messapii", "in a Messapian accent", "in a Messapian accent", Difficulty.Normal,
            "This is the native accent of the Messapii tribe of the Illyrian speakers in Salento in Italia.",
            "Messapian");
        AddAccent("Illyrian", "Amantini", "in a Amantinian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Amantini tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Andes", "in a Andesian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Andes tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Azali", "in a Azalian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Azali tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Breuci", "in a Breucian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Breuci tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Ditiones", "in a Ditionesian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Ditiones tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Jasi", "in a Jasian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Jasi tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Pirustae", "in a Pirustaean accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Pirustae tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Ceraunii", "in a Ceraunian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Ceraunii tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Glintidiones", "in a Glintidionesian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Glintidiones tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Scirtari", "in a Scirtarian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Scirtari tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Siculotae", "in a Siculotaean accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Siculotae tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Daesitates", "in a Daesitatesian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Daesitates tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Mazaei", "in a Mazaeian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Mazaei tribe of the Illyrian speakers in Pannonia.", "Pannonian");
        AddAccent("Illyrian", "Segestani", "in a Segestanian accent", "in a Pannonian accent", Difficulty.Easy,
            "This is the native accent of the Segestani tribe of the Illyrian speakers in Pannonia.", "Pannonian");


        AddAccent("Aeolic Greek", "Boeotia", "in a Boeotian accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Aeolic Greek speakers from Boeotia in Central Greece.", "Greek");
        AddAccent("Aeolic Greek", "Thessaly", "in a Thessalian accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Aeolic Greek speakers from Thessaly in Northern Greece.", "Greek");
        AddAccent("Aeolic Greek", "Lesbos", "in a Lesbian accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Aeolic Greek speakers from the island of Lesbos.", "Greek");
        AddAccent("Aeolic Greek", "Aeolia", "in an Aeolian accent", "in an Asian accent", Difficulty.ExtremelyEasy,
            "This is the accent of Aeolic Greek speakers from the Greek colony of Aeolia in Asia Minor.", "Asian");
        AddAccent("Aeolic Greek", "Anatolian", "in an Anatolian accent", "in an Asian accent", Difficulty.Easy,
            "This is the accent of Aeolic Greek speakers from broader Analtolia in Asia Minor.", "Asian");

        AddAccent("Doric Greek", "Laconian", "in a Laconian accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Doric Greek speakers from Laconia in Southern Greece.", "Greek");
        AddAccent("Doric Greek", "Argolic", "in an Argolic accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Doric Greek speakers from North-Eastern Peloponnese such as Argos in Southern Greece.",
            "Greek");
        AddAccent("Doric Greek", "Corinthian", "in a Corinthian accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Doric Greek speakers from the Isthmus of Corinth in Greece.", "Greek");
        AddAccent("Doric Greek", "Achaean", "in an Achaean accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Doric Greek speakers from Achaea in Greece.", "Greek");
        AddAccent("Doric Greek", "Delphian", "in an Delphian accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Doric Greek speakers from Delphi in Greece.", "Greek");
        AddAccent("Doric Greek", "Epirus", "in an Epirote accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Doric Greek speakers from Epirus in Northwestern Greece.", "Greek");
        AddAccent("Doric Greek", "Crete", "in an Cretan accent", "in an Aegean accent", Difficulty.Normal,
            "This is the accent of Doric Greek speakers from Crete in the Eastern Mediterranian.", "Aegean");
        AddAccent("Doric Greek", "Rhodes", "in an Rhodian accent", "in an Aegean accent", Difficulty.Normal,
            "This is the accent of Doric Greek speakers from Rhodes in the Eastern Mediterranian.", "Aegean");
        AddAccent("Doric Greek", "Anatolian", "in an Anatolian accent", "in an Aegean accent", Difficulty.Normal,
            "This is the accent of Doric Greek speakers from Doric Greek colonies in South-Eastern Anatolia.",
            "Aegean");
        AddAccent("Doric Greek", "Apulian", "in an Apulian accent", "in a Magna Graecian accent", Difficulty.Normal,
            "This is the accent of Doric Greek speakers from Doric Greek colonies in Apulia in Magna Graecia in Italia.",
            "Italian");
        AddAccent("Doric Greek", "Lucanian", "in a Lucanian accent", "in a Magna Graecian accent", Difficulty.Normal,
            "This is the accent of Doric Greek speakers from Doric Greek colonies in Lucania in Magna Graecia in Italia.",
            "Italian");
        AddAccent("Doric Greek", "Sicilian", "in a Sicilian accent", "in a Magna Graecian accent", Difficulty.Normal,
            "This is the accent of Doric Greek speakers from Doric Greek colonies in Sicilia in Magna Graecia in Italia.",
            "Italian");
        AddAccent("Doric Greek", "Campanian", "in a Campanian accent", "in a Magna Graecian accent", Difficulty.Normal,
            "This is the accent of Doric Greek speakers from Doric Greek colonies in Campania in Magna Graecia in Italia.",
            "Italian");

        AddAccent("Attic Greek", "Attic", "in a Attic accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Attic Greek speakers from the eponymous Attica in Greece.", "Greek");
        AddAccent("Attic Greek", "Lemnosian", "in a Lemnosian accent", "in an Aegean accent", Difficulty.VeryEasy,
            "This is the accent of Attic Greek speakers from the island of Lemnos in the Aegean.", "Aegean");
        AddAccent("Attic Greek", "Skyros", "in a Skyrosian accent", "in an Aegean accent", Difficulty.VeryEasy,
            "This is the accent of Attic Greek speakers from the island of Skyros in the Aegean.", "Aegean");
        AddAccent("Attic Greek", "Massilian", "in a Massilian accent", "in a Western accent", Difficulty.VeryEasy,
            "This is the accent of Attic Greek speakers from the Greek Colony of Massilia.", "Western");

        AddAccent("Ionic Greek", "Euboean", "in a Euboean accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Ionic Greek speakers from Euboea in Greece.", "Greek");
        AddAccent("Ionic Greek", "Chiosian", "in a Chiosian accent", "in an Aegean accent", Difficulty.ExtremelyEasy,
            "This is the accent of Ionic Greek speakers from the island of Chios in the Aegean.", "Aegean");
        AddAccent("Ionic Greek", "Samosian", "in a Samosian accent", "in an Aegean accent", Difficulty.ExtremelyEasy,
            "This is the accent of Ionic Greek speakers from the island of Samos in the Aegean.", "Aegean");

        AddAccent("Koine Greek", "Attic", "in an Attic accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Attic Greek speakers using the Koine Greek dialect.", "Greek");
        AddAccent("Koine Greek", "Doric", "in a Doric accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Doric Greek speakers using the Koine Greek dialect.", "Greek");
        AddAccent("Koine Greek", "Aeolian", "in an Aeolian accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Aeolic Greek speakers using the Koine Greek dialect.", "Greek");
        AddAccent("Koine Greek", "Ionic", "in an Ionic accent", "in a Greek accent", Difficulty.ExtremelyEasy,
            "This is the accent of Ionic Greek speakers using the Koine Greek dialect.", "Greek");
        AddAccent("Koine Greek", "Roman", "in a Roman accent", "in a Foreign accent", Difficulty.Easy,
            "This is the accent of educated Roman speakers using the Koine Greek dialect.", "Foreign");
        AddAccent("Koine Greek", "Punic", "in a Punic accent", "in a Foreign accent", Difficulty.Easy,
            "This is the accent of educated Punic speakers using the Koine Greek dialect.", "Foreign");
        AddAccent("Koine Greek", "Egyptian", "in an Egyptian accent", "in a Foreign accent", Difficulty.Easy,
            "This is the accent of educated Egyptian speakers using the Koine Greek dialect.", "Foreign");

        AddAccent("Coptic", "Sahidic", "in a Sahidic accent", "in an Upper Egyptian accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of speakers from Thebes, and also represents the prestige literary way of transcribing the language amongst scribes.",
            "Upper Egyptian");
        AddAccent("Coptic", "Akhmimic", "in an Akhmimic accent", "in an Upper Egyptian accent",
            Difficulty.ExtremelyEasy,
            "This is the accent and dialect of speakers from Akhmim (also known as Khemmis to the Ptolemies) in Upper Egypt.",
            "Upper Egyptian");
        AddAccent("Coptic", "Assiutic", "in an Assiutic accent", "in an Upper Egyptian accent",
            Difficulty.ExtremelyEasy,
            "This is the accent and dialect of speakers from Asyut (also known as Lycopolis to the Ptolemies) in Upper Egypt.",
            "Upper Egyptian");
        AddAccent("Coptic", "Bohairic", "in a Bohairic accent", "in an Lower Egyptian accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of speakers from a broad area around Memphis in Lower Egypt.",
            "Lower Egyptian");
        AddAccent("Coptic", "Phiomic", "in a Phiomic accent", "in an Lower Egyptian accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of speakers from Phiom (also known as Krokodopolis to the Ptolemies) in Lower Egypt.",
            "Lower Egyptian");
        AddAccent("Coptic", "Pemdjed", "in a Pemdjed accent", "in an Lower Egyptian accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of speakers from Pemdje (also known as Oxyrhynchus to the Ptolemies) in Lower Egypt.",
            "Lower Egyptian");
        AddAccent("Coptic", "Greek", "in a Greek accent", "in a Foreign accent", Difficulty.Normal,
            "This is the accent of Ptolemaic Greek speakers who are bilingual in Coptic.", "Greek");

        AddAccent("Demotic", "Standard", "in a Standard accent", "in a Standard accent", Difficulty.Normal,
            "This is the dialect of Demotic that survives in the written form of the language.", "Standard");

        AddAccent("Aramaic", "Hasmonaean", "in a Hasmonaean accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Aramaic speakers in the kingdom of Judea.", "Western");
        AddAccent("Aramaic", "Galilean", "in a Galilean accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Aramaic speakers in the kingdom of Galilee.", "Western");
        AddAccent("Aramaic", "Samarian", "in a Samarian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Aramaic speakers in the kingdom of Samaria.", "Western");
        AddAccent("Aramaic", "Nabatean", "in a Nabatean accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Aramaic speakers in the kingdom of Nabatea.", "Western");
        AddAccent("Aramaic", "Palmyrene", "in a Palmyrene accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Aramaic speakers from the city of Palmyra in Syria.", "Western");
        AddAccent("Aramaic", "Phoenician", "in a Phoenician accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Aramaic speakers in the kingdom of Phoenicia.", "Western");
        AddAccent("Aramaic", "Palestinian", "in a Palestinian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Aramaic speakers in the kingdom of Palestine.", "Western");
        AddAccent("Aramaic", "Babylonian", "in a Babylonian accent", "in an Eastern accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Aramaic speakers in the kingdom of Babylon.", "Eastern");
        AddAccent("Aramaic", "Syriac", "in a Syriac accent", "in an Eastern accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Aramaic speakers in the Syrian provinces.", "Eastern");

        AddAccent("Phoenician", "Tyrian", "in a Tyrian accent", "in a Levantine accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the city of Tyre.", "Levantine");
        AddAccent("Phoenician", "Sidonian", "in a Sidonian accent", "in a Levantine accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the city of Sidon.", "Levantine");
        AddAccent("Phoenician", "Cretan", "in a Cretan accent", "in a Levantine accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the island of Crete.", "Levantine");
        AddAccent("Phoenician", "Cypriot", "in a Cypriot accent", "in a Levantine accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the island of Cyprus.", "Levantine");
        AddAccent("Phoenician", "Carthaginian", "in a Carthaginian accent", "in a Punic accent",
            Difficulty.ExtremelyEasy, "This is the accent and dialect of Phoenician speakers from Carthage.", "Punic");
        AddAccent("Phoenician", "Sicilian", "in a Sicilian accent", "in a Punic accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the Punic colonies in Sicily.", "Punic");
        AddAccent("Phoenician", "Sardinian", "in a Sardinian accent", "in a Punic accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the Punic colonies in Sardinia.", "Punic");
        AddAccent("Phoenician", "Corsican", "in a Corsican accent", "in a Punic accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the Punic colonies in Corsica.", "Punic");
        AddAccent("Phoenician", "Leptitan", "in a Leptitan accent", "in a Punic accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the Punic city of Leptis Magna.", "Punic");
        AddAccent("Phoenician", "Balearic", "in a Balearic accent", "in a Punic accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the Balearic islands in the Western Mediterranian.",
            "Punic");
        AddAccent("Phoenician", "Hispanic", "in a Hispanic accent", "in a Punic accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the Punic colonies in Hispania.", "Punic");
        AddAccent("Phoenician", "Tingian", "in a Tingian accent", "in a Punic accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phoenician speakers from the Punic colony of Tingi on the Atlantic coast.",
            "Punic");

        AddAccent("Hebrew", "Samarian", "in a Samarian accent", "in a Levantine accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Samarians.", "Levantine");
        AddAccent("Hebrew", "Galilean", "in a Galilean accent", "in a Levantine accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Galileans.", "Levantine");
        AddAccent("Hebrew", "Sadducean", "in a Sadducean accent", "in a Levantine accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Sadduceans.", "Levantine");
        AddAccent("Hebrew", "Pharisean", "in a Pharisean accent", "in a Levantine accent", Difficulty.ExtremelyEasy,
            "This is the accent and dialect of Phariseans.", "Levantine");
        AddAccent("Hebrew", "Babylonian", "in a Babylonian accent", "in an Exile accent", Difficulty.Easy,
            "This is the accent and dialect of Jews in Babylon.", "Exile");
        AddAccent("Hebrew", "Egyptian", "in an Egyptian accent", "in an Exile accent", Difficulty.Easy,
            "This is the accent and dialect of Jews in Egypt.", "Exile");

        AddAccent("Numidian", "Cirtan", "in a Cirtan accent", "in a Numidian accent", Difficulty.ExtremelyEasy,
            "This is the accent of Numidians from the city of Cirta.", "Numidian");
        AddAccent("Numidian", "Tebessan", "in a Tebessan accent", "in a Numidian accent", Difficulty.ExtremelyEasy,
            "This is the accent of Numidians from the city of Tebessa.", "Numidian");
        AddAccent("Numidian", "Thuggan", "in a Thuggan accent", "in a Numidian accent", Difficulty.ExtremelyEasy,
            "This is the accent of Numidians from the city of Thugga.", "Numidian");

        AddAccent("Numidian", "Hippontic", "in a Hippontic accent", "in a Numidian accent", Difficulty.ExtremelyEasy,
            "This is the accent of Numidian speakers from the area around Hippo Regis.", "Numidian");
        AddAccent("Numidian", "Numidian", "in a Numidian accent", "in a Numidian accent", Difficulty.ExtremelyEasy,
            "This is the accent of Numidian speakers from the southern and inland areas of Numidia.", "Numidian");
        AddAccent("Numidian", "Caesariaensian", "in a Caesariaensian accent", "in a Mauritanian accent",
            Difficulty.ExtremelyEasy,
            "This is the accent of Numidian speakers from central Mauritania around Caesarea.", "Mauritanian");
        AddAccent("Numidian", "Tingitanian", "in a Tingitanian accent", "in a Mauritanian accent",
            Difficulty.ExtremelyEasy, "This is the accent of Numidian speakers from western Mauritania around Tingis.",
            "Mauritanian");
        AddAccent("Numidian", "Sitifensian", "in a Sitifensian accent", "in a Mauritanian accent",
            Difficulty.ExtremelyEasy, "This is the accent of Numidian speakers from eastern Mauritania around Sitifis.",
            "Mauritanian");
        AddAccent("Numidian", "Carthaginian", "in a Carthaginian accent", "in an African accent",
            Difficulty.ExtremelyEasy,
            "This is the accent of Numidian speakers from the province of Africa surrounding Carthage.", "African");
        AddAccent("Numidian", "Hadrumetian", "in a Hadrumetian accent", "in an African accent",
            Difficulty.ExtremelyEasy,
            "This is the accent of Numidian speakers from the province of Africa surrounding Hadrumetum.", "African");
        AddAccent("Numidian", "Tripolitanian", "in a Tripolitanian accent", "in an African accent",
            Difficulty.ExtremelyEasy, "This is the accent of Numidian speakers from the province of Tripolitania.",
            "African");
        AddAccent("Numidian", "Cyrenican", "in a Cyrenican accent", "in an African accent", Difficulty.ExtremelyEasy,
            "This is the accent of Numidian speakers from the province of Cyrenica.", "African");

        AddAccent("Phrygian", "Gordion", "in a Gordion accent", "in a Phyrgian accent", Difficulty.ExtremelyEasy,
            "This is the accent of Phyrgian speakers from the city of Gordium.", "Phyrgian");
        AddAccent("Phrygian", "Pacatanian", "in a Pacatanian accent", "in a Phyrgian accent", Difficulty.ExtremelyEasy,
            "This is the accent of Phyrgian speakers from the region of Pacatania.", "Phyrgian");
        AddAccent("Phrygian", "Salutarian", "in a Salutarian accent", "in a Phyrgian accent", Difficulty.ExtremelyEasy,
            "This is the accent of Phyrgian speakers from the region of Salutaris.", "Phyrgian");

        AddAccent("Anatolian", "Pisidian", "in the Pisidian dialect", "in a Luwian dialect", Difficulty.Easy,
            "This is the dialect of Anatolian from Pisidia.", "Luwian");
        AddAccent("Anatolian", "Lycian", "in the Lycian dialect", "in a Luwian dialect", Difficulty.Easy,
            "This is the dialect of Anatolian from Lycia.", "Luwian");
        AddAccent("Anatolian", "Sidetic", "in the Sidetic dialect", "in a Luwian dialect", Difficulty.Easy,
            "This is the dialect of Anatolian from Side.", "Luwian");
        AddAccent("Anatolian", "Carian", "in the Carian dialect", "in a Luwian dialect", Difficulty.Easy,
            "This is the dialect of Anatolian from Caria.", "Luwian");
        AddAccent("Anatolian", "Milyan", "in the Milyan dialect", "in a Luwian dialect", Difficulty.Easy,
            "This is the dialect of Anatolian from Milya.", "Luwian");
        AddAccent("Anatolian", "Luwian", "in the Luwian dialect", "in a Luwian dialect", Difficulty.Easy,
            "This is the dialect of Anatolian from Luwia.", "Luwian");
        AddAccent("Anatolian", "Cappadocian", "in the Cappadocian dialect", "in a Luwian dialect", Difficulty.Easy,
            "This is the dialect of Anatolian from Cappadocia.", "Luwian");
        AddAccent("Anatolian", "Isaurian", "in the Isaurian dialect", "in a Luwian dialect", Difficulty.Easy,
            "This is the dialect of Anatolian from Isauria.", "Luwian");
        AddAccent("Anatolian", "Lydian", "in the Lydian dialect", "in a Lydian dialect", 6,
            "This is the dialect of Anatolian from Lydia.", "Lydian");
        AddAccent("Anatolian", "Palaic", "in the Palaic dialect", "in a Palaic dialect", 6,
            "This is the dialect of Anatolian from Pala.", "Palaic");

        AddAccent("Arabic", "Ammonite", "in the Ammonite dialect", "in a Levantine dialect", Difficulty.Easy,
            "This is the dialect of Caananite speakers of Arabic from the kingdom of Ammon.", "Levantine");
        AddAccent("Arabic", "Edomite", "in the Edomite dialect", "in a Levantine dialect", Difficulty.Easy,
            "This is the dialect of Caananite speakers of Arabic from the kingdom of Edom.", "Levantine");
        AddAccent("Arabic", "Moabite", "in the Moabite dialect", "in a Levantine dialect", Difficulty.Easy,
            "This is the dialect of Caananite speakers of Arabic from the kingdom of Moab.", "Levantine");
        AddAccent("Arabic", "Hismaic", "in the Hismaic dialect", "in a North Arabian dialect", Difficulty.Easy,
            "This is the dialect of speakers of Arabic from Hisma in Northern Arabia.", "North Arabian");
        AddAccent("Arabic", "Nabatean", "in the Nabatean dialect", "in a North Arabian dialect", Difficulty.Easy,
            "This is the dialect of speakers of Arabic from the Kingdom of Nabatea.", "North Arabian");
        AddAccent("Arabic", "Safaitic", "in the Safaitic dialect", "in a North Arabian dialect", Difficulty.Easy,
            "This is the dialect of nomadic speakers of Arabic from the Arabian deserts.", "North Arabian");
        AddAccent("Arabic", "Dadanitic", "in the Dadanitic dialect", "in a North Arabian dialect", Difficulty.Easy,
            "This is the dialect of speakers of Arabic from the oasis of Dadan in Northern Arabia.", "North Arabian");
        AddAccent("Arabic", "Sabaean", "in the Sabaean dialect", "in a South Arabian dialect", Difficulty.Easy,
            "This is the dialect of speakers of Arabic from the the Kingdom of Saba in Southern Arabia.",
            "South Arabian");
        AddAccent("Arabic", "Minaean", "in the Minaean dialect", "in a South Arabian dialect", Difficulty.Easy,
            "This is the dialect of speakers of Arabic from the the city states of Al-Jawf in Southern Arabia.",
            "South Arabian");
        AddAccent("Arabic", "Qatabaanian", "in the Qatabaanian dialect", "in a South Arabian dialect", Difficulty.Easy,
            "This is the dialect of speakers of Arabic from the the Kingdom of Qataab in Southern Arabia.",
            "South Arabian");
        AddAccent("Arabic", "Hadramitic", "in the Hadramitic dialect", "in a South Arabian dialect", Difficulty.Easy,
            "This is the dialect of speakers of Arabic from the the Kingdom of Hadramaut in Southern Arabia.",
            "South Arabian");

        AddAccent("Parthian", "Nisan", "in a Nisan accent", "in a Parthian accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Parthian from the Parthian city of Nisa.", "Parthian");
        AddAccent("Parthian", "Mervian", "in a Mervian accent", "in a Parthian accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Parthian from the Parthian city of Merv.", "Parthian");
        AddAccent("Parthian", "Arshakian", "in an Arshakian accent", "in a Parthian accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Parthian from the Parthian city of Arshak.", "Parthian");
        AddAccent("Parthian", "Zadracartan", "in a Zadracartan accent", "in a Parthian accent",
            Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Parthian from the Parthian city of Zadracarta.", "Parthian");
        AddAccent("Parthian", "Hecatompylosian", "in a Hecatompylosian accent", "in a Parthian accent",
            Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Parthian from the Parthian city of Hecatompylos.", "Parthian");
        AddAccent("Parthian", "Vagharshapatian", "in a Vagharshapatian accent", "in a Causasian accent",
            Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Parthian from the Armenian city of Vagharshapat.", "Causasian");
        AddAccent("Parthian", "Artaxatian", "in an Artaxatian accent", "in a Causasian accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Parthian from the Armenian city of Artaxata.",
            "Causasian");
        AddAccent("Parthian", "Tigranocertan", "in a Tigranocertan accent", "in a Causasian accent",
            Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Parthian from the Armenian city of Tigranocerta.", "Causasian");
        AddAccent("Parthian", "Asamosatan", "in an Asamosatan accent", "in a Causasian accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Parthian from the Armenian city of Asamosata.",
            "Causasian");
        AddAccent("Parthian", "Iberian", "in an Iberian accent", "in a Causasian accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Parthian from the Arcasid Kingdom of Iberia.", "Causasian");
        AddAccent("Parthian", "Albanian", "in an Albanian accent", "in a Causasian accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Parthian from the Arcasid Kingdom of Albania.", "Causasian");

        AddAccent("Aquitanian", "Aquitani", "in an Aquitani accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Aquitani.", "Northern");
        AddAccent("Aquitanian", "Ausci", "in an Ausci accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Ausci.", "Northern");
        AddAccent("Aquitanian", "Elusatesi", "in an Elusatesi accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Elusates.", "Northern");
        AddAccent("Aquitanian", "Sotiatesi", "in a Sotiatesi accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Sotiates.", "Northern");
        AddAccent("Aquitanian", "Volcatesi", "in a Volcatesi accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Volcates.", "Northern");
        AddAccent("Aquitanian", "Vasatesi", "in a Vasatesi accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Vasates.", "Northern");
        AddAccent("Aquitanian", "Tarbelli", "in a Tarbelli accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Tarbelli.", "Northern");
        AddAccent("Aquitanian", "Sibuzatesi", "in a Sibuzatesi accent", "in a Northern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Aquitanian from the tribe of Sibuzates.",
            "Northern");
        AddAccent("Aquitanian", "Bigerronesi", "in a Bigerronesi accent", "in a Northern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Aquitanian from the tribe of Bigerrones.",
            "Northern");
        AddAccent("Aquitanian", "Conveni", "in an Conveni accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Conveni.", "Northern");
        AddAccent("Aquitanian", "Vasconesi", "in a Vasconesi accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Vasconesi.", "Southern");
        AddAccent("Aquitanian", "Vescetani", "in a Vescetani accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Vescetani.", "Southern");
        AddAccent("Aquitanian", "Ceretesi", "in an Ceretesi accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Ceretes.", "Southern");
        AddAccent("Aquitanian", "Ilergetesi", "in an Ilergetesi accent", "in a Southern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Aquitanian from the tribe of Ilergetes.",
            "Southern");
        AddAccent("Aquitanian", "Varduli", "in a Varduli accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Varduli.", "Southern");
        AddAccent("Aquitanian", "Caristii", "in a Caristii accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Caristii.", "Southern");
        AddAccent("Aquitanian", "Autrigonesi", "in an Autrigonesi accent", "in a Southern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Aquitanian from the tribe of Autrigones.",
            "Southern");
        AddAccent("Aquitanian", "Cantabri", "in a Cantabri accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Aquitanian from the tribe of Cantabri.", "Southern");

        AddAccent("Iberian", "Ausetani", "in an Ausetani accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Iberian from the tribe of Ausetani.", "Northern");
        AddAccent("Iberian", "Ilergetesi", "in an Ilergetesi accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Iberian from the tribe of Ilergetes.", "Northern");
        AddAccent("Iberian", "Indigetesi", "in an Indigetesi accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Iberian from the tribe of Indigetes.", "Northern");
        AddAccent("Iberian", "Laietani", "in a Laietani accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Iberian from the tribe of Laietani.", "Northern");
        AddAccent("Iberian", "Cassetani", "in a Cassetani accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Iberian from the tribe of Cassetani.", "Northern");
        AddAccent("Iberian", "Ilercavonesi", "in an Ilercavonesi accent", "in a Southern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Iberian from the tribe of Ilercavones.",
            "Southern");
        AddAccent("Iberian", "Edetani", "in an Edetani accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Iberian from the tribe of Edetani.", "Southern");
        AddAccent("Iberian", "Contestani", "in a Contestani accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Iberian from the tribe of Contestani.", "Southern");
        AddAccent("Iberian", "Basetetani", "in a Basetetani accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Iberian from the tribe of Basetetani.", "Southern");
        AddAccent("Iberian", "Oretani", "in an Oretani accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Iberian from the tribe of Oretani.", "Southern");

        AddAccent("Tartessian", "Turduli", "in a Turduli accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Tartessian from the tribe of Turduli.", "Northern");
        AddAccent("Tartessian", "Turdetani", "in a Turdetani accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Tartessian from the tribe of Turdetani.", "Southern");

        AddAccent("Gaulish", "Pictonian", "in a Pictonian accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Pictones tribal federation.", "Central");
        AddAccent("Gaulish", "Aremorican", "in a Aremorican accent", "in a Northwestern accent",
            Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Aremorican tribal federation.", "Northwestern");
        AddAccent("Gaulish", "Aulercian", "in a Aulercian accent", "in a Northwestern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Aulercian tribal federation.", "Northwestern");
        AddAccent("Gaulish", "Carnutian", "in a Carnutian accent", "in a Northwestern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Carnutian tribal federation.", "Northwestern");
        AddAccent("Gaulish", "Vindelician", "in a Vindelician accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Vindelic tribal federation.", "Central");
        AddAccent("Gaulish", "Helvetian", "in a Helvetian accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Helvetican tribal federation.", "Central");
        AddAccent("Gaulish", "Biturigan", "in a Biturigan accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Biturigan tribal federation.", "Central");
        AddAccent("Gaulish", "Santonian", "in a Santonian accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Santonian tribal federation.", "Central");
        AddAccent("Gaulish", "Lemovician", "in a Lemovician accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Lemovician tribal federation.", "Central");
        AddAccent("Gaulish", "Arvernian", "in a Arvernian accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Arvernian tribal federation.", "Central");
        AddAccent("Gaulish", "Lemovician", "in a Lemovician accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Lemovician tribal federation.", "Central");
        AddAccent("Gaulish", "Volcaean", "in a Volcaean accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Volcaean tribal federation.", "Central");
        AddAccent("Gaulish", "Alluvian", "in an Alluvian accent", "in a Cisalpine accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Alluvian tribal federation.", "Cisalpine");
        AddAccent("Gaulish", "Haeduian", "in a Haeduian accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Gaulish from the Haeduian tribal federation.", "Central");
        AddAccent("Gaulish", "Lepontic", "in a Lepontic accent", "in a Cisalpine accent", Difficulty.Easy,
            "This is the accent of speakers of Gaulish from the Lepontic tribal federation.", "Cisalpine");
        AddAccent("Gaulish", "Raetic", "in a Raetic accent", "in a Cisalpine accent", Difficulty.Easy,
            "This is the accent of speakers of Gaulish from Raetia.", "Cisalpine");

        AddAccent("Ligurian", "Genuan", "in a Genuan accent", "in a Coastal accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Ligurian from the city of Genua.", "Coastal");
        AddAccent("Ligurian", "Albingaunian", "in a Albingaunian accent", "in a Coastal accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Ligurian from the city of Albingaunum.",
            "Coastal");
        AddAccent("Ligurian", "Segestan", "in a Segestan accent", "in a Coastal accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Ligurian from the city of Segesta.", "Coastal");
        AddAccent("Ligurian", "Hastan", "in a Hastan accent", "in an Inland accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Ligurian from the city of Hasta.", "Inland");
        AddAccent("Ligurian", "Dertonan", "in a Dertonan accent", "in an Inland accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Ligurian from the city of Dertona.", "Inland");

        AddAccent("Raetic", "Raetic", "in a Raetic accent", "in a Raetic accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Raetic from the province of Raetia.", "Raetic");
        AddAccent("Raetic", "Lepontic", "in a Lepontic accent", "in a Lepontic accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Raetic from the province of Lepontia.", "Lepontic");

        AddAccent("Brittonic", "Icenian", "in an Icenian accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Brittonic from the Iceni tribe.", "Southern");
        AddAccent("Brittonic", "Trinovantian", "in a Trinovantian accent", "in a Southern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Brittonic from the Trinovanti tribe.",
            "Southern");
        AddAccent("Brittonic", "Cantian", "in a Cantian accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Brittonic from the Canti tribe.", "Southern");
        AddAccent("Brittonic", "Durotrigan", "in a Durotrigan accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Brittonic from the Durotrigan tribe.", "Southern");
        AddAccent("Brittonic", "Dumnonian", "in a Dumnonian accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Brittonic from the Dumnonian tribe.", "Southern");
        AddAccent("Brittonic", "Dobunnian", "in a Dobunnian accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Brittonic from the Dobunnian tribe.", "Southern");
        AddAccent("Brittonic", "Demetian", "in a Demetian accent", "in a Southern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Brittonic from the Demetian tribe.", "Southern");
        AddAccent("Brittonic", "Ordovitian", "in an Ordovitian accent", "in a Northern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Brittonic from the Ordovitian tribe.",
            "Northern");
        AddAccent("Brittonic", "Coritanian", "in a Coritanian accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Brittonic from the Coritani tribe.", "Northern");
        AddAccent("Brittonic", "Cornovian", "in a Cornovian accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Brittonic from the Cornovian tribe.", "Northern");
        AddAccent("Brittonic", "Brigantic", "in a Brigantic accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Brittonic from the Brigantic tribe.", "Northern");

        AddAccent("Goidelic", "Votadinian", "in a Votadinian accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Goidelic from the Votadinian tribe.", "Northern");
        AddAccent("Goidelic", "Damnonian", "in a Damnonian accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Goidelic from the Damnonian tribe.", "Northern");
        AddAccent("Goidelic", "Caledonian", "in a Caledonian accent", "in a Pictish accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Goidelic from the Caledonian tribe.", "Pictish");
        AddAccent("Goidelic", "Taexalian", "in a Taexalian accent", "in a Pictish accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Goidelic from the Taexalian tribe.", "Pictish");
        AddAccent("Goidelic", "Hibernian", "in a Hibernian accent", "in a Hibernian accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Goidelic from the island of Hibernia.", "Hibernian");

        AddAccent("Pictish", "Caledonian", "in a Caledonian accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Pictish from the Caledonian tribe.", "Northern");
        AddAccent("Pictish", "Taexalian", "in a Taexalian accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Pictish from the Taexalian tribe.", "Northern");
        AddAccent("Pictish", "Damnonian", "in a Damnonian accent", "in a Northern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Pictish from the Damnonian tribe.", "Northern");

        AddAccent("Celtiberian", "Celtiberian", "in a Celtiberian accent", "in a Central accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Celtiberian from the Celtiberi tribe.",
            "Central");
        AddAccent("Celtiberian", "Carpetanian", "in a Carpetanian accent", "in a Central accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Celtiberian from the Carpetani tribe.",
            "Central");
        AddAccent("Celtiberian", "Vaccaeian", "in a Vaccaeian accent", "in a Central accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Celtiberian from the Vaccaei tribe.", "Central");
        AddAccent("Celtiberian", "Turmodigian", "in a Turmodigian accent", "in a Central accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Celtiberian from the Turmodigi tribe.",
            "Central");
        AddAccent("Celtiberian", "Cantabrian", "in a Cantabrian accent", "in a Central accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Celtiberian from the Cantabri tribe.",
            "Central");
        AddAccent("Celtiberian", "Aquitanian", "in an Aquitanian accent", "in an Aquitanian accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Celtiberian from the Aquitania region.",
            "Aquitanian");
        AddAccent("Celtiberian", "Turdulian", "in a Turdulian accent", "in an Southern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Celtiberian from the Turduli tribe.",
            "Southern");
        AddAccent("Celtiberian", "Celtician", "in a Celtician accent", "in an Southern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Celtiberian from the Celtici tribe.",
            "Southern");
        AddAccent("Celtiberian", "Oretanian", "in an Oretanian accent", "in an Southern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Celtiberian from the Oretani tribe.",
            "Southern");

        AddAccent("Gallaecian", "Callaecian", "in a Callaecian accent", "in an Northern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Gallaecian from the Callaeci tribe.",
            "Northern");
        AddAccent("Gallaecian", "Asturesian", "in an Asturesian accent", "in an Northern accent",
            Difficulty.ExtremelyEasy, "This is the accent of speakers of Gallaecian from the Asturesi tribe.",
            "Northern");
        AddAccent("Gallaecian", "Oppidanian", "in an Oppidanian accent", "in an Southern accent", Difficulty.Easy,
            "This is the accent of speakers of Gallaecian from the Oppidani tribe.", "Southern");
        AddAccent("Gallaecian", "Lusitanian", "in a Lusitanian accent", "in an Southern accent", Difficulty.Easy,
            "This is the accent of speakers of Gallaecian from the Lusitani tribe.", "Southern");

        AddAccent("Galatian", "Ancyran", "in an Ancyran accent", "in a Galatian accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Galatian from the Tectosages tribe in Ancyra.", "Galatian");
        AddAccent("Galatian", "Pessinusian", "in a Pessinusian accent", "in a Galatian accent",
            Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Galatian from the Tolistobogii tribe in Pessinus.", "Galatian");
        AddAccent("Galatian", "Tavian", "in a Tavian accent", "in a Galatian accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Galatian from the Trocmi tribe in Tavium.", "Galatian");
        AddAccent("Galatian", "Aigosagesian", "in an Aigosagesian accent", "in an Anatolian accent", Difficulty.Easy,
            "This is the accent of speakers of Galatian from the Aigosages tribe.", "Anatolian");
        AddAccent("Galatian", "Dagutenian", "in a Dagutenian accent", "in an Anatolian accent", Difficulty.Easy,
            "This is the accent of speakers of Galatian from the Daguteni tribe.", "Anatolian");
        AddAccent("Galatian", "Inovantenian", "in an Inovantenian accent", "in an Anatolian accent", Difficulty.Easy,
            "This is the accent of speakers of Galatian from the Inovanteni tribe.", "Anatolian");
        AddAccent("Galatian", "Okondianian", "in an Okondianian accent", "in an Anatolian accent", Difficulty.Easy,
            "This is the accent of speakers of Galatian from the Okondiani tribe.", "Anatolian");
        AddAccent("Galatian", "Rigosagesian", "in a Rigosagesian accent", "in an Anatolian accent", Difficulty.Easy,
            "This is the accent of speakers of Galatian from the Rigosages tribe.", "Anatolian");

        AddAccent("Belgic", "Belgaean", "in a Belgaean accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Belgic from the Belgae tribe.", "Western");

        AddAccent("Belgic", "Nervian", "in a Belgaean accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Belgic from the Belgae tribe.", "Western");
        AddAccent("Belgic", "Morinian", "in a Morinian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Belgic from the Morini tribe.", "Western");
        AddAccent("Belgic", "Veliocassian", "in a Veliocassian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Belgic from the Veliocassi tribe.", "Western");
        AddAccent("Belgic", "Bellovacian", "in a Bellovacian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Belgic from the Bellovaci tribe.", "Western");
        AddAccent("Belgic", "Sennonian", "in a Sennonian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Belgic from the Sennoni tribe.", "Western");
        AddAccent("Belgic", "Aduatacian", "in a Aduatacian accent", "in an Eastern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Belgic from the Aduataci tribe.", "Eastern");
        AddAccent("Belgic", "Eburonian", "in a Eburonian accent", "in an Eastern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Belgic from the Eburoni tribe.", "Eastern");
        AddAccent("Belgic", "Treverian", "in a Treverian accent", "in an Eastern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Belgic from the Treveri tribe.", "Eastern");
        AddAccent("Belgic", "Menapian", "in a Menapian accent", "in an Eastern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Belgic from the Menapi tribe.", "Eastern");

        AddAccent("Germanic", "Saxonian", "in a Saxonian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Germanic from the Saxones tribe.", "Western");
        AddAccent("Germanic", "Anglian", "in an Anglian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Germanic from the Anglii tribe.", "Western");
        AddAccent("Germanic", "Suebian", "in a Suebian accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Germanic from the Suebii tribe.", "Western");
        AddAccent("Germanic", "Istavonic", "in a Istavonic accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Germanic from the Istavones tribe.", "Western");
        AddAccent("Germanic", "Ingvaeonic", "in a Ingvaeonic accent", "in a Western accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Germanic from the Ingvaeoni tribe.", "Western");
        AddAccent("Germanic", "Gothonic", "in a Gothonic accent", "in an Eastern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Germanic from the Gothoni tribe.", "Eastern");
        AddAccent("Germanic", "Vandal", "in a Vandal accent", "in an Eastern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Germanic from the Vandal tribe.", "Eastern");
        AddAccent("Germanic", "Bastarnae", "in a Bastarnae accent", "in an Eastern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Germanic from the Bastarni tribe.", "Eastern");
        AddAccent("Germanic", "Irminonic", "in an Irminonic accent", "in an Eastern accent", Difficulty.ExtremelyEasy,
            "This is the accent of speakers of Germanic from the Irminoni tribe.", "Eastern");
        AddAccent("Germanic", "Cimbrian", "in a Cimbrian accent", "in a Northern accent", Difficulty.Easy,
            "This is the accent of speakers of Germanic from the Cimbri tribe.", "Northern");
        AddAccent("Germanic", "Teutonian", "in a Teutonian accent", "in a Northern accent", Difficulty.Easy,
            "This is the accent of speakers of Germanic from the Teutones tribe.", "Northern");
        AddAccent("Germanic", "Herulian", "in a Herulian accent", "in a Northern accent", Difficulty.Easy,
            "This is the accent of speakers of Germanic from the Heruli tribe.", "Northern");
        AddAccent("Germanic", "Gutonian", "in a Gutonian accent", "in a Northern accent", Difficulty.Easy,
            "This is the accent of speakers of Germanic from the Gutones tribe.", "Northern");
        AddAccent("Germanic", "Suionian", "in a Suionian accent", "in a Northern accent", Difficulty.Easy,
            "This is the accent of speakers of Germanic from the Suioni tribe.", "Northern");
        AddAccent("Germanic", "Raumarician", "in a Raumarician accent", "in a Northern accent", Difficulty.Easy,
            "This is the accent of speakers of Germanic from the Raumarici tribe.", "Northern");
        AddAccent("Germanic", "Rugian", "in a Rugian accent", "in a Northern accent", Difficulty.Easy,
            "This is the accent of speakers of Germanic from the Rugi tribe.", "Northern");

        AddAccent("Ariya", "Arachosian", "in an Arachosian accent", "in an Avestan accent", Difficulty.Easy,
            "This is the accent of speakers of Ariya from the region of Arachosia.", "Avestan");
        AddAccent("Ariya", "Arian", "in an Arian accent", "in an Avestan accent", Difficulty.Easy,
            "This is the accent of speakers of Ariya from the region of Aria.", "Avestan");
        AddAccent("Ariya", "Bactrian", "in a Bactrian accent", "in an Avestan accent", Difficulty.Easy,
            "This is the accent of speakers of Ariya from the region of Bactria.", "Avestan");
        AddAccent("Ariya", "Margianan", "in a Margianan accent", "in an Avestan accent", Difficulty.Easy,
            "This is the accent of speakers of Ariya from the region of Margiana.", "Avestan");
        AddAccent("Ariya", "Persian", "in a Persian accent", "in a Persik accent", Difficulty.Easy,
            "This is the accent of speakers of Ariya from the Persia Proper.", "Persik");
        AddAccent("Ariya", "Utian", "in a Utian accent", "in a Persik accent", Difficulty.Easy,
            "This is the accent of speakers of Ariya from the Utia.", "Persik");
        AddAccent("Ariya", "Susianan", "in a Susianan accent", "in a Persik accent", Difficulty.Easy,
            "This is the accent of speakers of Ariya from the Susiana.", "Persik");
        AddAccent("Ariya", "Median", "in a Median accent", "in a Persik accent", Difficulty.Easy,
            "This is the accent of speakers of Ariya from the Median.", "Persik");
        AddAccent("Ariya", "Mesopotamian", "in a Mesopotamian accent", "in a Mesopotamian accent", Difficulty.Easy,
            "This is the accent of speakers of Ariya from the city states of Mesopotamia.", "Persik");

        SeedAntiquityLanguageCoverage();

        #endregion
    }

	private void SeedAntiquityLanguageCoverage()
	{
		SeedHistoricalLanguagePack(
			AntiquityLanguageCoverage,
			AntiquityScriptCoverage,
			AntiquityMutualIntelligibilityCoverage);
	}

	private static readonly HistoricalLanguageSeed[] AntiquityLanguageCoverage =
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

	private static readonly HistoricalScriptSeed[] AntiquityScriptCoverage =
	[
		new("Greek", "the Greek script", "an alphabetic Mediterranean script",
			"Greek letters were also used to record foreign names and short texts around the Black Sea.",
			"Alphabet", 1.0, 1.0, ["Scythian", "Sarmatian"]),
		new("Meroitic", "the Meroitic script", "an unfamiliar Nile Valley script",
			"The Meroitic alphasyllabary was used in the Kingdom of Kush in hieroglyphic and cursive forms.",
			"Alphasyllabary", 0.9, 1.2, ["Meroitic"])
	];

	private static readonly HistoricalMutualIntelligibilitySeed[] AntiquityMutualIntelligibilityCoverage =
	[
		new("Scythian", "Sarmatian", Difficulty.Hard)
	];
}
