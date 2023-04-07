using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private readonly Dictionary<string, Language> _languages = new();

	public void AddLanguage(string name, string unknownDescription, FutureProg? canSelectProg = null)
	{
		var decorator = _context.TraitDecorators.First(x => x.Name == "Language Skill");
		var improver = _context.Improvers.First(x => x.Name == "Language Improver");
		var capFormula = _context.Languages.FirstOrDefault()?.LinkedTrait.Expression.Expression ??
		                 $"10+(9.5 * {_intelligenceTrait.Alias}:{_intelligenceTrait.Id})";
		var expression = new TraitExpression { Name = $"{name} Skill Cap", Expression = capFormula };

		var trait = new TraitDefinition
		{
			Name = name,
			Type = 0,
			DecoratorId = decorator.Id,
			TraitGroup = "Language",
			AvailabilityProg = canSelectProg ?? _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			TeachableProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
			LearnableProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			TeachDifficulty = 7,
			LearnDifficulty = 7,
			Hidden = false,
			Expression = expression,
			ImproverId = improver.Id,
			DerivedType = 0,
			ChargenBlurb = string.Empty,
			BranchMultiplier = 0.1
		};
		_context.TraitDefinitions.Add(trait);
		_context.SaveChanges();

		var language = new Language
		{
			Name = name,
			UnknownLanguageDescription = unknownDescription,
			LanguageObfuscationFactor = 0.1,
			DifficultyModel = 1,
			LinkedTrait = trait
		};
		_context.Languages.Add(language);
		_context.SaveChanges();
		_languages[name] = language;

		var accent = new Accent
		{
			Name = "Foreign",
			Suffix = "with a foreign accent",
			VagueSuffix = "with a foreign accent",
			Difficulty = (int)Difficulty.Normal,
			Description = $"The heavily-foreign accent of a non-native learner of the {name} language",
			Group = "Foreign",
			Language = language
		};
		_context.Accents.Add(accent);
		_context.SaveChanges();
		language.DefaultLearnerAccent = accent;
		_context.SaveChanges();
	}

	private void AddAccent(string language, string name, string suffix, string vague, int difficulty,
		string description, string group, FutureProg? prog = null)
	{
		_context.Accents.Add(new Accent
		{
			Name = name.TitleCase(),
			Suffix = suffix,
			VagueSuffix = vague,
			Difficulty = difficulty,
			Description = description,
			Group = group,
			Language = _languages[language],
			ChargenAvailabilityProgId = prog?.Id
		});
		_context.SaveChanges();
	}

	private void AddAccent(string language, string name, string suffix, string vague, Difficulty difficulty,
		string description, string group, FutureProg? prog = null)
	{
		_context.Accents.Add(new Accent
		{
			Name = name.TitleCase(),
			Suffix = suffix,
			VagueSuffix = vague,
			Difficulty = (int)difficulty,
			Description = description,
			Group = group,
			Language = _languages[language],
			ChargenAvailabilityProgId = prog?.Id
		});
		_context.SaveChanges();
	}

	private void AddMutualIntelligability(string from, string to, Difficulty difficulty, bool twoWay = false)
	{
		_context.MutualIntelligabilities.Add(new MutualIntelligability
		{
			TargetLanguage = _languages[from],
			ListenerLanguage = _languages[to],
			IntelligabilityDifficulty = (int)difficulty
		});
		if (twoWay)
			_context.MutualIntelligabilities.Add(new MutualIntelligability
			{
				TargetLanguage = _languages[to],
				ListenerLanguage = _languages[from],
				IntelligabilityDifficulty = (int)difficulty
			});
		_context.SaveChanges();
	}

	public void AddScript(string name, string known, string unknown, string description, string subtype, double length,
		double ink, params string[] languages)
	{
		var sb = new StringBuilder();
		sb.AppendLine("switch (@skill.Name)");
		foreach (var language in languages)
		{
			sb.AppendLine($"  case (\"{language}\")");
			sb.AppendLine("    return true");
		}

		sb.AppendLine("end switch");
		sb.AppendLine("return false");

		var prog = new FutureProg
		{
			FunctionName = $"CanPick{name.Replace("-", "").Replace(" ", "")}ScriptKnowledge",
			FunctionComment =
				$"Controls whether someone can pick the {name} script knowledge during character creation. Will let them pick it if they have one of the language skills that the script is designed for.",
			FunctionText = sb.ToString(),
			ReturnType = 4,
			Category = "Knowledges",
			Subcategory = "Scripts",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		_context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 0, ParameterName = "ch", ParameterType = 8200 });
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 1, ParameterName = "skill", ParameterType = 16384 });
		_context.SaveChanges();

		var knowledge = new Knowledge
		{
			Name = $"{name} Script",
			Description = $"Knowledge of the use of the {name} Script",
			LongDescription = description,
			Type = "Script",
			Subtype = subtype,
			LearnableType = (int)(LearnableType.LearnableAtChargen | LearnableType.LearnableFromTeacher),
			LearnDifficulty = (int)Difficulty.VeryHard,
			TeachDifficulty = (int)Difficulty.VeryHard,
			LearningSessionsRequired = 10,
			CanAcquireProg = prog,
			CanLearnProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue")
		};
		_context.Knowledges.Add(knowledge);
		_context.SaveChanges();

		var script = new Script
		{
			Name = name,
			DocumentLengthModifier = length,
			InkUseModifier = ink,
			KnownScriptDescription = known,
			UnknownScriptDescription = unknown,
			KnowledgeId = knowledge.Id
		};
		_context.Scripts.Add(script);
		foreach (var language in languages)
			script.ScriptsDesignedLanguages.Add(new ScriptsDesignedLanguage
				{ Script = script, Language = _languages[language] });
		_context.SaveChanges();
	}

	private void SeedMiddleEarthLanguages()
	{
		var canSelectMiddleEarthLanguageProg = new FutureProg
		{
			FunctionName = "CanSelectMiddleEarthLanguage",
			Category = "Chargen",
			Subcategory = "Skills",
			FunctionComment = "Used to determine which languages a character can pick at character creation",
			ReturnType = (int)FutureProgVariableTypes.Boolean,
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
			ParameterType = (long)FutureProgVariableTypes.Chargen,
			ParameterName = "ch"
		});
		canSelectMiddleEarthLanguageProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = canSelectMiddleEarthLanguageProg,
			ParameterIndex = 1,
			ParameterType = (long)FutureProgVariableTypes.Trait,
			ParameterName = "trait"
		});
		_context.FutureProgs.Add(canSelectMiddleEarthLanguageProg);
		_context.SaveChanges();

		#region Language

		AddLanguage("Westron", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
		AddLanguage("Sindarin", "an unknown elvish language", canSelectMiddleEarthLanguageProg);
		AddLanguage("Adunaic", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
		AddLanguage("Khuzdul", "an unknown dwarven language", canSelectMiddleEarthLanguageProg);
		AddLanguage("Quenya", "an unknown elvish language", canSelectMiddleEarthLanguageProg);
		AddLanguage("Silvan", "an unknown elvish language", canSelectMiddleEarthLanguageProg);
		AddLanguage("Rohirric", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
		AddLanguage("Haradic", "an unknown mannish language", canSelectMiddleEarthLanguageProg);
		AddLanguage("Black Speech", "a harsh, unknown language", canSelectMiddleEarthLanguageProg);
		AddLanguage("Orkish", "a harsh, unknown language", canSelectMiddleEarthLanguageProg);
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
			"The Japanese script includes a series of Kata and Kanji, in a mixed approach somewhere in between a syllabic script and a symbolic script.",
			"Hybrid", 0.75, 1.5, "Japanese");
		AddScript("Korean", "the Korean script", "an East Asian script",
			"The Korean script, also known as Hangul, is an alphabetic script designed to support the Korean language.",
			"Hybrid", 0.75, 1.5, "Japanese");
		AddScript("Chinese", "the Chinese script", "an East Asian script",
			"The Japanese script includes a series of Kata and Kanji, in a mixed approach somewhere in between a syllabic script and a symbolic script.",
			"Hybrid", 0.5, 2.0, "Japanese");
		AddScript("Hebrew", "the Japanese script", "an East Asian script",
			"The Japanese script includes a series of Kata and Kanji, in a mixed approach somewhere in between a syllabic script and a symbolic script.",
			"Hybrid", 0.5, 2.0, "Japanese");
		AddScript("Arabic", "the Japanese script", "an East Asian script",
			"The Japanese script includes a series of Kata and Kanji, in a mixed approach somewhere in between a syllabic script and a symbolic script.",
			"Hybrid", 0.5, 2.0, "Japanese");
		AddScript("Sanskrit", "the Japanese script", "an East Asian script",
			"The Japanese script includes a series of Kata and Kanji, in a mixed approach somewhere in between a syllabic script and a symbolic script.",
			"Hybrid", 0.5, 2.0, "Japanese");

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

		#endregion
	}

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

		#endregion
	}

	private void SeedMedievalEuropeLanguages()
	{
	}
}