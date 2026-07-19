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
    private void SeedMiddleEarthLanguages()
    {
        FutureProg canSelectMiddleEarthLanguageProg = new()
        {
            FunctionName = "CanSelectMiddleEarthLanguage",
            Category = "Chargen",
            Subcategory = "Skills",
            FunctionComment = "Used to determine which languages a character can pick at character creation",
            ReturnType = (int)ProgVariableTypes.Boolean,
            AcceptsAnyParameters = false,
            Public = false,
            StaticType = 0,
            FunctionText = @"// Special applications can pick whatever they want
if (@ch.Special)
  return true
end if

// If you put any merits that permit you language overrides, put them here

// If you put any roles that permit you language overrides, put them here

// Elves can learn any language as they are supremely educated
if (@ch.Race == ToRace(""Elf""))
	  return true
end if

// For Numenorean-descended, exclude only certain languages
if (@ch.Ethnicity.EthnicGroup == ""Edain"")
  switch (@trait.Name)
	case (""Khuzdul"")
	  // Nobody but Dwarves learns Khuzdul
	  return false
	case (""Silvan"")
	  // Silvan might be known in Fourth-Age Settings but would be unknown before
	  return false
	case (""Logathig"")
	   // Black Numenoreans can know these
	  return @ch.Ethnicity.EthnicSubGroup == ""Fallen""
	case (""Varadja"")
	  return @ch.Ethnicity.EthnicSubGroup == ""Fallen""
	case (""Black Speech"")
	  return @ch.Ethnicity.EthnicSubGroup == ""Fallen""
  end switch
  return true
end if

// Otherwise, check per language
switch (@trait.Name)
  case (""Westron"")
	return true
  case (""Sindarin"")
	if (@ch.Race == ToRace(""Dwarf""))
	  return true
	end if
	return false
  case (""Adunaic"")
	return false
  case (""Khuzdul"")
	return false
  case (""Quenya"")
	return false
  case (""Silvan"")
	return false
  case (""Rohirric"")
	if (@ch.Race == ToRace(""Human"") and @ch.Ethnicity.EthnicSubGroup == ""Northman"")
	  return true
	end if
	return false
  case (""Dunlendish"")
	if (@ch.Race == ToRace(""Human"") and @ch.Ethnicity.EthnicSubGroup == ""Northman"")
	  return true
	end if
	return false
  case (""Haradic"")
	if (@ch.Race == ToRace(""Human"") and @ch.Ethnicity.EthnicGroup == ""Fallen Man"")
	  return true
	end if
	return false
  case (""Black Speech"")
	if (@ch.Race == ToRace(""Orc"") or @ch.Race == ToRace(""Troll""))
	  return true
	end if
	return false
  case (""Orkish"")
	if (@ch.Race == ToRace(""Orc"") or @ch.Race == ToRace(""Troll"") or @ch.Race == ToRace(""Dwarf""))
	  return true
	end if
	return false
  case (""Hobbitish"")
	if (@ch.Race == ToRace(""Hobbit""))
	  return true
	end if
	if (@ch.Race == ToRace(""Human"") and @ch.Ethnicity.Name == ""Eriadoran"")
	  return true
	end if
	return false
  case (""Dalish"")
	if (@ch.Race == ToRace(""Human"") and @ch.Ethnicity.EthnicSubGroup == ""Northman"")
	  return true
	end if
	return false
  case (""Logathig"")
	if (@ch.Race == ToRace(""Human"") and (@ch.Ethnicity.Name == ""Easterling"" or @ch.Ethnicity.Name == ""Dorwinrim""))
	  return true
	end if
	return false
  case (""Varadja"")
	if (@ch.Race == ToRace(""Human"") and @ch.Ethnicity.EthnicGroup == ""Fallen Man"")
	  return true
	end if
	return false
end switch
return false"
        };
        canSelectMiddleEarthLanguageProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = canSelectMiddleEarthLanguageProg,
            ParameterIndex = 0,
            ParameterType = (long)ProgVariableTypes.Chargen,
            ParameterName = "ch"
        });
        canSelectMiddleEarthLanguageProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = canSelectMiddleEarthLanguageProg,
            ParameterIndex = 1,
            ParameterType = (long)ProgVariableTypes.Trait,
            ParameterName = "trait"
        });
        _context.FutureProgs.Add(canSelectMiddleEarthLanguageProg);
        _context.SaveChanges();

        #region Language

        AddLanguage("Westron", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Sindarin", "an unknown elvish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Adunaic", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Atliduk", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Haradaic", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Dunael", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Labba", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Norliduk", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Talathic", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Umitic", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Nahaiduk", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Pukael", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Khuzdul", "an unknown dwarven language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Quenya", "an unknown elvish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Silvan", "an unknown elvish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Rohirric", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Haradic", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Black Speech", "a harsh, unknown language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Orkish", "a harsh, unknown language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Taliska", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Haladin", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Thrunon", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Beast-Tongue", "an unknown bestial language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Valarin", "an unknown ancient language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Nandorin", "an unknown elvish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Druag", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Avarin", "an unknown elvish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Trollish", "a harsh, unknown language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Hobbitish", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Dalish", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Logathig", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        AddLanguage("Varadja", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
        _context.SaveChanges();

        #endregion

        #region Scripts

        AddScript("Tengwar", "the Tengwar script", "a flowing script",
            "The Tengwar script is the script that was originally used for the Elvish languages.  It was eventually adopted for writing most languages.",
            "Tengwar Alphabetic", 1.0, 1.0, "Westron", "Sindarin", "Adunaic", "Quenya", "Rohirric", "Haradic",
            "Black Speech", "Hobbitish", "Dalish", "Logathig", "Varadja", "Silvan");
        AddScript("Cirth", "Cirth runes", "runes",
            "The Cirth is a runic script, used by the Dwarves, but also common for elvish and mannish languages.  The Cirth is largely used for carving in wood or stone.",
            "Cirth Alphabetic", 1.0, 1.0, "Westron", "Sindarin", "Adunaic", "Khuzdul", "Quenya", "Rohirric", "Haradic",
            "Black Speech", "Orkish", "Hobbitish", "Dalish", "Logathig", "Varadja", "Silvan");
        AddScript("Sarati", "the Sarati script", "an archaic script",
            "The Sarati script is an ancient script of Aman, remembered mostly by scholars and loremasters.",
            "Sarati Alphabetic", 1.0, 1.0, "Valarin", "Quenya");
        AddScript("Valarin-Script", "the Valarin script", "an alien script",
            "The Valarin script records the ancient language of the Valar and is almost never encountered outside deep lore.",
            "Valarin Alphabetic", 1.0, 1.0, "Valarin");
        _context.SaveChanges();

        #endregion

        #region Mutual Intelligibilities

        AddMutualIntelligability("Westron", "Adunaic", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Westron", "Rohirric", Difficulty.VeryHard, true);
        AddMutualIntelligability("Westron", "Orkish", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Westron", "Varadja", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Westron", "Logathig", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Westron", "Dalish", Difficulty.ExtremelyHard, true);

        AddMutualIntelligability("Hobbitish", "Adunaic", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Hobbitish", "Westron", Difficulty.Hard, true);
        AddMutualIntelligability("Hobbitish", "Rohirric", Difficulty.VeryHard, true);
        AddMutualIntelligability("Hobbitish", "Orkish", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Hobbitish", "Varadja", Difficulty.Insane, true);
        AddMutualIntelligability("Hobbitish", "Logathig", Difficulty.Insane, true);
        AddMutualIntelligability("Hobbitish", "Dalish", Difficulty.VeryHard, true);

        AddMutualIntelligability("Rohirric", "Dalish", Difficulty.VeryHard, true);
        AddMutualIntelligability("Rohirric", "Orkish", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Rohirric", "Varadja", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Rohirric", "Logathig", Difficulty.VeryHard, true);

        AddMutualIntelligability("Dalish", "Orkish", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Dalish", "Varadja", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Dalish", "Logathig", Difficulty.VeryHard, true);

        AddMutualIntelligability("Haradic", "Varadja", Difficulty.VeryHard, true);
        AddMutualIntelligability("Haradic", "Orkish", Difficulty.VeryHard, true);

        AddMutualIntelligability("Varadja", "Orkish", Difficulty.VeryHard, true);
        AddMutualIntelligability("Logathig", "Orkish", Difficulty.VeryHard, true);

        AddMutualIntelligability("Sindarin", "Quenya", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Sindarin", "Silvan", Difficulty.VeryHard, true);
        AddMutualIntelligability("Quenya", "Silvan", Difficulty.Insane, true);

        AddMutualIntelligability("Adunaic", "Rohirric", Difficulty.ExtremelyHard, true);
        AddMutualIntelligability("Adunaic", "Dalish", Difficulty.VeryHard, true);
        _context.SaveChanges();

        #endregion

        #region Accents

        AddAccent("Westron", "Bree-land", "with a Bree-land accent", "in a Western dialect", Difficulty.VeryEasy,
            "This is the most common accent and dialect, native to Bree-land.", "Western");
        AddAccent("Westron", "Buckland", "with a Buckland accent", "in a Western dialect", Difficulty.VeryEasy,
            "Residents of Buckland have an accent different from that of the rest of the Shire.", "Western");
        AddAccent("Westron", "Shire", "with a Shire accent", "in a Western dialect", Difficulty.VeryEasy,
            "This is the usual accent and dialect of Hobbits of the Shire, rustic and unhurried.", "Western");
        AddAccent("Westron", "Gondorian Noble", "with a proper Gondorian accent", "with a Dunedain accent",
            Difficulty.VeryEasy,
            "This is the version of Westron spoken by the upper class of Gondorian society, very proper.", "Dunedain",
            _ethnicProgs["Gondorian Dunedain"]);
        AddAccent("Westron", "Gondorian Commoner", "with a common Gondorian accent", "in a Western dialect",
            Difficulty.VeryEasy, "This is the variety of Westron spoken by everyday commoners in Gondor.", "Western");
        AddAccent("Westron", "Arnorian", "with an Arnorian accent", "with a Dunedain accent", Difficulty.Easy,
            "This is the variety of Westron spoken by the Dunedain of the North, plain but archaic.", "Dunedain",
            _ethnicProgs["Arnorian Dunedain"]);
        AddAccent("Westron", "Rohirric", "with a Rohirric accent", "in a Northern dialect", Difficulty.Easy,
            "This is the variety of Westron spoken by one of the Rohirrim.", "Northern");
        AddAccent("Westron", "Dorwinion", "with a Dorwinion accent", "in a Northern dialect", Difficulty.Easy,
            "This is the variety of Westron spoken by Dorwinrim.", "Northern");
        AddAccent("Westron", "Dunlendish", "with a Dunlendish accent", "in a Northern dialect", Difficulty.Easy,
            "This is the variety of Westron spoken by Dunlendings.", "Northern");
        AddAccent("Westron", "Dalish", "with a Dalish accent", "in a Northern dialect", Difficulty.Easy,
            "This is the variety of Westron spoken by the men of the Dale.", "Northern");
        AddAccent("Westron", "Haradic", "with a Haradic accent", "in a Southern dialect", Difficulty.Easy,
            "This is the variety of Westron spoken by one of the Haradrim.", "Southern", _ethnicProgs["Haradrim"]);
        AddAccent("Westron", "Umbar", "with an Umbaric accent", "in a Southern dialect", Difficulty.Easy,
            "This is the variety of Westron spoken by one of those dwelling in Umbar.", "Southern",
            _cultureProgs["Corsair"]);
        AddAccent("Westron", "Khandish", "with a Khandish accent", "in a Southern dialect", Difficulty.Easy,
            "This is the variety of Westron spoken by one of those dwelling in Khand.", "Southern",
            _ethnicProgs["Variag"]);
        AddAccent("Westron", "Orkish", "with a crude Orkish accent", "in an Orkish dialect", Difficulty.Easy,
            "This is the variety of Westron spoken by goblins, typically harsh and broken.", "Orkish",
            _raceProgs["Orc"]);
        AddAccent("Westron", "Elven", "with a song-like Elven accent", "in an Elven dialect", Difficulty.Trivial,
            "This is the variety of Westron spoken by Elves.", "Elven", _raceProgs["Elf"]);
        AddAccent("Westron", "Dwarven", "with a harsh Dwarven accent", "in a Dwarven dialect", Difficulty.Easy,
            "This is the variety of Westron spoken by Dwarves, accented with harsh emphasis on certain consonants.",
            "Dwarven", _raceProgs["Dwarf"]);

        AddAccent("Hobbitish", "Eastfarthing", "with an Eastfarthing accent", "in a Shire dialect", Difficulty.Trivial,
            "This is the variety of Hobbitish spoken by Hobbits in the Eastfarthing of the Shire, including the towns of Frogmorton, Brokenborings, and Whitfurrows and the farms of the Marish.",
            "Shire");
        AddAccent("Hobbitish", "Southfarthing", "with a Southfarthing accent", "in a Shire dialect", Difficulty.Trivial,
            "This is the variety of Hobbitish spoken by Hobbits in the Southfarthing of the Shire, including the towns of Gamwich, Cotton, and Longbottom.",
            "Shire");
        AddAccent("Hobbitish", "Westfarthing", "with a Westfarthing accent", "in a Shire dialect", Difficulty.Trivial,
            "This is the variety of Hobbitish spoken by Hobbits in the Westfarthing of the Shire, including the towns of Michel Delving, Tuckborough (part of Tookland), and Hobbiton.",
            "Shire");
        AddAccent("Hobbitish", "Northfarthing", "with a Northfarthing accent", "in a Shire dialect",
            Difficulty.ExtremelyEasy,
            "This is the variety of Hobbitish spoken by Hobbits in the sparsely-populated Northfarthing of the Shire. This is an area rustic to even other Hobbits.",
            "Shire");
        AddAccent("Hobbitish", "Buckland", "with a Buckland accent", "in an Eastern dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Hobbitish spoken by Hobbits in the Buckland area east of the shire. This area is associated with Stoor hobbits.",
            "Eastern");
        AddAccent("Hobbitish", "Bree-land", "with a Bree-land accent", "in an Eastern dialect",
            Difficulty.ExtremelyEasy,
            "This is the variety of Hobbitish spoken by Hobbits in the various towns and villages of Bree-land.",
            "Eastern");
        AddAccent("Hobbitish", "Elven", "with a song-like Elven accent", "in an Elven dialect", Difficulty.Trivial,
            "The accent of Elves who choose to speak this language.", "Elven", _raceProgs["Elf"]);

        AddAccent("Rohirric", "Eastfold", "with an Eastfold accent", "in a Southern dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Rohirric spoken by those in the Eastfold region by the White Mountains and bordering Anorien.",
            "Southern");
        AddAccent("Rohirric", "Westfold", "with a Westfold accent", "in a Southern dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Rohirric spoken by those in the Westfold region by the White Mountains.",
            "Southern");
        AddAccent("Rohirric", "Westemnet", "with a Westemnet accent", "in a Northern dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Rohirric spoken by those in the Westemnet region between Westfold and Isengard.",
            "Northern");
        AddAccent("Rohirric", "Eastemnet", "with an Eastemnet accent", "in a Northern dialect",
            Difficulty.ExtremelyEasy,
            "This is the variety of Rohirric spoken by those in the Eastemnet region East of the Entwash and South of the Wold.",
            "Northern");
        AddAccent("Rohirric", "Wold", "with a Wold accent", "in a Northern dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Rohirric spoken by those in the Wold region east of the Entwood and north of Eastemnet.",
            "Northern");
        AddAccent("Rohirric", "Westmarch", "with a Westmarch accent", "in a Western dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Rohirric spoken by those in the Westmarch region at the Western border of the Riddermark.",
            "Western");
        AddAccent("Rohirric", "Dunlendish", "in the Dunlendish dialect", "in a Western dialect", Difficulty.Hard,
            "This is a dialect of Rohirric spoken by the Dunlendings of Dunland.", "Western");
        AddAccent("Rohirric", "Elven", "with a song-like Elven accent", "in an Elven dialect", Difficulty.Trivial,
            "The accent of Elves who choose to speak this language.", "Elven", _raceProgs["Elf"]);

        AddAccent("Haradic", "Near-Harad", "in the Near-Harad dialect", "in an unknown dialect", Difficulty.Hard,
            "The dialect this language spoken in Near Harad, the area bordering Gondor.", "Near");
        AddAccent("Haradic", "Far-Harad", "in the Far-Harad dialect", "in an unknown dialect", Difficulty.Hard,
            "The dialect this language spoken in Far Harad, the area beyond Near Harad where the Mumakil come from.",
            "Far");
        AddAccent("Haradic", "Elven", "with a song-like Elven accent", "in an Elven dialect", Difficulty.Trivial,
            "The accent of Elves who choose to speak this language.", "Elven", _raceProgs["Elf"]);

        AddAccent("Dalish", "Dalish", "with the accent of the Dale", "in a Mannish dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Dalish spoken by those in the city of Dale.", "Mannish");
        AddAccent("Dalish", "Laketown", "with the accent of Esgaroth", "in a Mannish dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Dalish spoken by those in the city of Esgaroth, or Laketown.", "Mannish");
        AddAccent("Dalish", "Rhovanion", "with the accent of Rhovanion", "in a Mannish dialect",
            Difficulty.ExtremelyEasy,
            "This is the variety of Dalish spoken by those in the Principalities of Rhovanion, west of the Sea of Rhun.",
            "Mannish");
        AddAccent("Dalish", "Beorning", "with the accent of a Beorning", "in a Mannish dialect",
            Difficulty.ExtremelyEasy, "This is the wild variety of Dalish spoken by Beornings.", "Mannish",
            _alwaysFalseProg);
        AddAccent("Dalish", "Dalish", "with a Dwarven accent", "in a Dwarven dialect", Difficulty.Easy,
            "This is the variety of Dalish spoken by Dwarves in Erebor.", "Dwarven", _raceProgs["Dwarf"]);
        AddAccent("Dalish", "Elven", "with a song-like Elven accent", "in an Elven dialect", Difficulty.Trivial,
            "The accent of Elves who choose to speak this language.", "Elven", _raceProgs["Elf"]);

        AddAccent("Logathig", "Rhovanion", "with a Rhovanion accent", "in a Western dialect", Difficulty.Hard,
            "This is the non-native variety of Logathig spoken by those in the Principalities of Rhovanion, west of the Sea of Rhun.",
            "Western");
        AddAccent("Logathig", "Wainrider", "with a Wainrider accent", "in an Eastern dialect", Difficulty.Hard,
            "This is the variety of Logathig spoken by the Wainriders of Rhun.", "Eastern");
        AddAccent("Logathig", "Balchoth", "with a Balchoth accent", "in an Eastern dialect", Difficulty.Hard,
            "This is the variety of Logathig spoken by the Balchoth of Rhun.", "Eastern");
        AddAccent("Logathig", "Dorwinrim", "with a Dorwinrim accent", "in a Western dialect", Difficulty.Hard,
            "This is the variety of Logathig spoken by the Dorwinrim of Dorwinion.", "Western");

        AddAccent("Varadja", "Lower", "in the Lower Khand dialect", "in the Lower Khand dialect", Difficulty.Easy,
            "This is the variety of Varadja spoken by those in the Lower Khand Region.", "Lower");
        AddAccent("Varadja", "Upper", "in the Upper Khand dialect", "in the Upper Khand dialect", Difficulty.Easy,
            "This is the variety of Varadja spoken by those in the Upper Khand Region, beyond the great river.",
            "Upper");

        AddAccent("Quenya", "Lindon", "with a Lindonian accent", "in a Noldor dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Quenya spoken by the Elves of the Grey Havens.", "Noldor", _ethnicProgs["Noldor"]);
        AddAccent("Quenya", "Lothlorien", "with a Lothlorien accent", "in a Noldor dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Quenya spoken by the Elves of Lothlorien.", "Noldor", _ethnicProgs["Noldor"]);
        AddAccent("Quenya", "Rivendell", "with a Rivendell accent", "in a Noldor dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Quenya spoken by the Elves of Rivendell.", "Noldor", _ethnicProgs["Noldor"]);
        AddAccent("Quenya", "Sindarin", "with a Sindarin accent", "in a Sindarin dialect", Difficulty.Easy,
            "This is the variety of Quenya spoken by Sindarin Elves.", "Sindar", _raceProgs["Elf"]);
        AddAccent("Quenya", "Silvan", "with a wild Silvan accent", "in a Silvan dialect", Difficulty.Hard,
            "This is the variety of Quenya spoken by Silvan Elves, which is considered very rustic to Elven ears.",
            "Silvan", _ethnicProgs["Silvan"]);
        AddAccent("Quenya", "Arnorian", "with an Arnorian accent", "in a Mannish dialect", Difficulty.Hard,
            "This is the variety of Quenya spoken by the Dunedain of Arnor and others of that region.", "Mannish",
            _raceProgs["Human"]);
        AddAccent("Quenya", "Gondorian", "with a Gondorian accent", "in a Mannish dialect", Difficulty.Hard,
            "This is the variety of Quenya spoken by the Dunedain of Gondor and others of that region.", "Mannish",
            _raceProgs["Human"]);

        AddAccent("Sindarin", "Lindon", "with a Lindonian accent", "in a Sindar dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Sindarin spoken by the Elves of the Grey Havens.", "Sindar", _raceProgs["Elf"]);
        AddAccent("Sindarin", "Common", "with a simple accent", "in a Sindar dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Sindarin spoken by the Elves of Rivendell and others.  It is most common form of Sindarin, especially among those that frequently interact with humans.",
            "Sindar", _raceProgs["Elf"]);
        AddAccent("Sindarin", "Greenwood", "with a wild Greenwood accent", "in Silvan dialect", Difficulty.VeryEasy,
            "This is the variety of Sindarin spoken by the elves of the Greenwood.  For elvish, this would be considered rustic.",
            "Silvan", _ethnicProgs["Silvan"]);
        AddAccent("Sindarin", "High-Elven", "with a High-Elven accent", "in a Noldor dialect", Difficulty.ExtremelyEasy,
            "This is the variety of Sindarin spoken by High-Elves from ages past.  It is heavily influenced by the Quenyan tongue.",
            "Noldor", _ethnicProgs["Noldor"]);
        AddAccent("Sindarin", "Arnorian", "with an Arnorian accent", "in a Mannish dialect", Difficulty.Normal,
            "This is the variety of Sindarin spoken commonly by the Dunedain of Arnor and others of that region.",
            "Mannish", _raceProgs["Human"]);
        AddAccent("Sindarin", "Gondorian", "with a Gondorian accent", "in a Mannish dialect", Difficulty.Normal,
            "This is the variety of Sindarin spoken by the Dunedain of Gondor and others of that region.", "Mannish",
            _raceProgs["Human"]);
        AddAccent("Sindarin", "Haradic", "with a Haradic accent", "in a Mannish dialect", Difficulty.Hard,
            "This is the variety of Sindarin spoken by some learned individuals in Harad or Umbar.", "Mannish",
            _raceProgs["Human"]);
        AddAccent("Sindarin", "Orkish", "with a harsh Orkish accent", "in an Orkish dialect", Difficulty.ExtremelyHard,
            "On the uncommon chance that an Orc learns the Sindarin tongue, their terrible, harsh dialect is completely unmistakable.",
            "Orkish", _raceProgs["Orc"]);

        AddAccent("Silvan", "Greenwood", "with a Greenwood accent", "in a Greenwood accent", Difficulty.ExtremelyEasy,
            "This is the variety of Silvan (or Avari) spoken only rarely by the Silvan Elves of the Greenwood.",
            "Greenwood", _ethnicProgs["Silvan"]);
        AddAccent("Silvan", "Lothlorien", "with a Lothlorien accent", "in a Lothlorien accent",
            Difficulty.ExtremelyEasy,
            "This is the variety of Silvan (or Avari) spoken only rarely by the Silvan Elves who reside in Lothlorien.",
            "Lothlorien", _ethnicProgs["Silvan"]);
        AddAccent("Silvan", "Sindarin", "with a Sindarin accent", "in a Sindarin accent", Difficulty.ExtremelyEasy,
            "This is the variety of Silvan (or Avari) spoken by the rare Sindarin Elves who both know and care to use Silvan.",
            "Sindarin", _raceProgs["Elf"]);

        AddAccent("Adunaic", "Gondorian", "with a Gondorian accent", "in a Western dialect", Difficulty.Normal,
            "This is the variety of Adunaic spoken by Gondorians.  The Faithful of Numenor largely abandoned Adunaic in favor of Sindarin.",
            "Western", _ethnicProgs["Gondorian Dunedain"]);
        AddAccent("Adunaic", "Arnorian", "with an Arnorian accent", "in a Western dialect", Difficulty.Normal,
            "This is the variety of Adunaic spoken by Arnorians. The Faithful of Numenor largely abandoned Adunaic in favor of Sindarin.",
            "Western", _ethnicProgs["Arnorian Dunedain"]);
        AddAccent("Adunaic", "Elven", "with an Elven accent", "in an Elven dialect", Difficulty.Normal,
            "This is the variety of Adunaic spoken by Elves who choose to speak this language.", "Elven",
            _raceProgs["Elf"]);
        AddAccent("Adunaic", "Black Adunaic", "with a Black Adunaic accent", "in an Eastern dialect", Difficulty.Hard,
            "This is the variety of Adunaic spoken by Black Numenoreans. These men remain largely in Umbar, Harad and Mordor.",
            "Eastern", _ethnicProgs["Black Numenorean"]);
        AddAccent("Adunaic", "Haradic", "with a Haradic accent", "in an Eastern dialect", Difficulty.Hard,
            "This is the variety of Adunaic spoken by the Haradic, and is probably the most common due to the influence of the unfaithful Numenoreans in ancient Harad.",
            "Eastern", _ethnicProgs["Haradrim"]);
        AddAccent("Adunaic", "Archaic", "with an archaic Numenorean accent", "in an Archaic dialect",
            Difficulty.ExtremelyHard,
            "This is the variety of Adunaic as spoken by the Numenoreans before the fall of Numenor. The dialect may be preserved in ancient writings or inscriptions.",
            "Archaic", _alwaysFalseProg);

        AddAccent("Khuzdul", "Ereborian", "with an Ereborian accent", "in an unknown dialect", Difficulty.Normal,
            "This is the secret variety of Khuzdul spoken by the Dwarves of Erebor.", "Dwarven");
        AddAccent("Khuzdul", "Grey Mountains", "with a Grey Mountains accent", "in an unknown dialect",
            Difficulty.Normal, "This is the secret variety of Khuzdul spoken by the Dwarves of the Grey Mountains.",
            "Dwarven");
        AddAccent("Khuzdul", "Iron Hills", "with an Iron Hills accent", "in an unknown dialect", Difficulty.Normal,
            "This is the secret variety of Khuzdul spoken by the Dwarves of the Iron Hills.", "Dwarven");
        AddAccent("Khuzdul", "Khazad-Dum", "with a Khazad-Dum accent", "in an unknown dialect", Difficulty.Normal,
            "This is the secret variety of Khuzdul spoken by the Dwarves of Khazad-Dum, also known as Moria.",
            "Dwarven");
        AddAccent("Khuzdul", "Aglarondian", "with an Aglarondian accent", "in an unknown dialect", Difficulty.Normal,
            "This is the secret variety of Khuzdul spoken by the Dwarves of the Glittering Caves of Aglarond.",
            "Dwarven");
        AddAccent("Khuzdul", "Blue-mountain", "with an accent common to the Blue Mountains", "in an unknown dialect",
            Difficulty.Normal, "This is the secret variety of Khuzdul spoken by the Dwarves of the Blue Mountains.",
            "Dwarven");

        AddAccent("Black Speech", "Elven", "with an Elven accent", "with an Elven accent", Difficulty.Hard,
            "This is the variety of Black Speech when spoken by an Elf.", "Elven", _raceProgs["Elf"]);
        AddAccent("Black Speech", "Mannish", "with a Mannish accent", "with a Mannish accent", Difficulty.Hard,
            "This is the variety of Black Speech as employed by those evil men who serve Sauron.", "Mannish",
            _raceProgs["Human"]);
        AddAccent("Black Speech", "Orkish", "with an Orkish accent", "with an Orkish accent", Difficulty.Hard,
            "This is the variety of Black Speech as employed by those orcs who serve Sauron.", "Orkish",
            _raceProgs["Orc"]);
        AddAccent("Black Speech", "Trollish", "with a Trollish accent", "with a Trollish accent", Difficulty.Hard,
            "This is the variety of Black Speech as employed by those trolls who serve Sauron.", "Trollish",
            _raceProgs["Troll"]);

        AddAccent("Orkish", "Misty Mountains", "in the Misty Mountains dialect", "in an unknown dialect",
            Difficulty.Hard, "This is the dialect spoken by the Orcs of the Misty Mountains.", "Orkish");
        AddAccent("Orkish", "Grey Mountains", "in the Grey Mountains dialect", "in an unknown dialect", Difficulty.Hard,
            "This is the dialect spoken by the Orcs of the Grey Mountains.", "Orkish");
        AddAccent("Orkish", "Isengard", "in the Isengard dialect", "in an unknown dialect", Difficulty.Hard,
            "This is the dialect spoken by the Orcs who serve Saruman in Isengard.", "Orkish");
        AddAccent("Orkish", "Mirkwood", "in the Mirkwood dialect", "in an unknown dialect", Difficulty.Hard,
            "This is the dialect spoken by the Orcs who inhabit Mirkwood and Dol Guldur.", "Orkish");
        AddAccent("Orkish", "Mordorian", "in the Mordorian dialect", "in an unknown dialect", Difficulty.Hard,
            "This is the dialect spoken by the Orcs of the Ephel Duath and other regions of Mordor.", "Orkish");

        _context.SaveChanges();

        #endregion
    }
}
