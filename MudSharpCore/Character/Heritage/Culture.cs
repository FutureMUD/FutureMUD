using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.Character.Heritage;

public class Culture : SaveableItem, ICulture
{
	private readonly Dictionary<Gender, string> _personWord = new();

	public Culture(MudSharp.Models.Culture culture, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = culture.Id;
		_name = culture.Name;
		Description = culture.Description;
		foreach (var nc in culture.CulturesNameCultures)
		{
			_genderNameCultures[(Gender)nc.Gender] = gameworld.NameCultures.Get(nc.NameCultureId);
		}

		PrimaryCalendar = gameworld.Calendars.Get(culture.PrimaryCalendarId);
		SkillStartingValueProg = gameworld.FutureProgs.Get(culture.SkillStartingValueProgId);
		AvailabilityProg = gameworld.FutureProgs.Get(culture.AvailabilityProgId ?? 0);
		if (!string.IsNullOrEmpty(culture.PersonWordMale))
		{
			_personWord.Add(Gender.Male, culture.PersonWordMale);
		}

		if (!string.IsNullOrEmpty(culture.PersonWordFemale))
		{
			_personWord.Add(Gender.Female, culture.PersonWordFemale);
		}

		if (!string.IsNullOrEmpty(culture.PersonWordNeuter))
		{
			_personWord.Add(Gender.Neuter, culture.PersonWordNeuter);
			_personWord.Add(Gender.NonBinary, culture.PersonWordNeuter);
		}

		_personWord.Add(Gender.Indeterminate, culture.PersonWordIndeterminate);
		foreach (var item in culture.CulturesChargenResources)
		{
			_costs.Add(new ChargenResourceCost
			{
				Amount = item.Amount,
				RequirementOnly = item.RequirementOnly,
				Resource = gameworld.ChargenResources.Get(item.ChargenResourceId)
			});
		}

		foreach (var item in culture.ChargenAdvicesCultures)
		{
			_chargenAdvices.Add(gameworld.ChargenAdvices.Get(item.ChargenAdviceId));
		}

		TolerableTemperatureCeilingEffect = culture.TolerableTemperatureCeilingEffect;
		TolerableTemperatureFloorEffect = culture.TolerableTemperatureFloorEffect;
	}

	public Culture(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.Culture
			{
				Name = name,
				AvailabilityProgId = Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysTrue")?.Id,
				Description = "An undescribed culture",
				PersonWordMale = "person",
				PersonWordFemale = "person",
				PersonWordIndeterminate = "person",
				PersonWordNeuter = "person",
				PrimaryCalendarId = Gameworld.Calendars.First().Id,
				SkillStartingValueProgId = Gameworld.FutureProgs.First(x => x.FunctionName == "AlwaysOne").Id,
				TolerableTemperatureCeilingEffect = 0.0,
				TolerableTemperatureFloorEffect = 0.0
			};
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = 1,
				Gender = (short)Gender.Male
			});
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = 1,
				Gender = (short)Gender.Female
			});
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = 1,
				Gender = (short)Gender.Neuter
			});
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = 1,
				Gender = (short)Gender.NonBinary
			});
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = 1,
				Gender = (short)Gender.Indeterminate
			});
			FMDB.Context.Cultures.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			_name = name;
			AvailabilityProg = Gameworld.FutureProgs.Get(dbitem.AvailabilityProgId ?? 0);
			SkillStartingValueProg = Gameworld.FutureProgs.Get(dbitem.SkillStartingValueProgId);
			_personWord[Gender.Male] = "person";
			_personWord[Gender.Female] = "person";
			_personWord[Gender.Neuter] = "person";
			_personWord[Gender.NonBinary] = "person";
			_personWord[Gender.Indeterminate] = "person";
			PrimaryCalendar = Gameworld.Calendars.Get(dbitem.PrimaryCalendarId);
			TolerableTemperatureCeilingEffect = 0.0;
			TolerableTemperatureFloorEffect = 0.0;
			Description = dbitem.Description;
		}
	}

	public Culture(ICulture rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.Culture
			{
				Name = newName,
				AvailabilityProgId = rhs.AvailabilityProg?.Id,
				Description = rhs.Description,
				PersonWordMale = rhs.PersonWord(Gender.Male),
				PersonWordFemale = rhs.PersonWord(Gender.Female),
				PersonWordIndeterminate = rhs.PersonWord(Gender.Indeterminate),
				PersonWordNeuter = rhs.PersonWord(Gender.Neuter),
				PrimaryCalendarId = rhs.PrimaryCalendar.Id,
				SkillStartingValueProgId = rhs.SkillStartingValueProg.Id,
				TolerableTemperatureCeilingEffect = rhs.TolerableTemperatureCeilingEffect,
				TolerableTemperatureFloorEffect = rhs.TolerableTemperatureFloorEffect
			};
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = rhs.NameCultureForGender(Gender.Male).Id,
				Gender = (short)Gender.Male
			});
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = rhs.NameCultureForGender(Gender.Female).Id,
				Gender = (short)Gender.Female
			});
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = rhs.NameCultureForGender(Gender.Neuter).Id,
				Gender = (short)Gender.Neuter
			});
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = rhs.NameCultureForGender(Gender.NonBinary).Id,
				Gender = (short)Gender.NonBinary
			});
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = rhs.NameCultureForGender(Gender.Indeterminate).Id,
				Gender = (short)Gender.Indeterminate
			});
			FMDB.Context.Cultures.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			_name = newName;
			AvailabilityProg = rhs.AvailabilityProg;
			SkillStartingValueProg = rhs.SkillStartingValueProg;
			_personWord[Gender.Male] = rhs.PersonWord(Gender.Male);
			_personWord[Gender.Female] = rhs.PersonWord(Gender.Female);
			_personWord[Gender.Neuter] = rhs.PersonWord(Gender.Neuter);
			_personWord[Gender.NonBinary] = rhs.PersonWord(Gender.Neuter);
			_personWord[Gender.Indeterminate] = rhs.PersonWord(Gender.Indeterminate);
			PrimaryCalendar = rhs.PrimaryCalendar;
			TolerableTemperatureCeilingEffect = rhs.TolerableTemperatureCeilingEffect;
			TolerableTemperatureFloorEffect = rhs.TolerableTemperatureFloorEffect;
			Description = rhs.Description;
			_genderNameCultures[Gender.Male] = rhs.NameCultureForGender(Gender.Male);
			_genderNameCultures[Gender.Female] = rhs.NameCultureForGender(Gender.Female);
			_genderNameCultures[Gender.Neuter] = rhs.NameCultureForGender(Gender.Neuter);
			_genderNameCultures[Gender.NonBinary] = rhs.NameCultureForGender(Gender.NonBinary);
			_genderNameCultures[Gender.Indeterminate] = rhs.NameCultureForGender(Gender.Indeterminate);
		}
	}

	public override string FrameworkItemType => "Culture";

	private Dictionary<Gender, INameCulture> _genderNameCultures = new();

	public INameCulture NameCultureForGender(Gender gender)
	{
		return _genderNameCultures[gender];
	}

	public IEnumerable<INameCulture> NameCultures => _genderNameCultures.Values.Distinct();

	public IFutureProg SkillStartingValueProg { get; protected set; }

	public ICalendar PrimaryCalendar { get; protected set; }
	private readonly List<IChargenAdvice> _chargenAdvices = new();

	public IEnumerable<IChargenAdvice> ChargenAdvices => _chargenAdvices;

	public bool ToggleAdvice(IChargenAdvice advice)
	{
		Changed = true;
		if (_chargenAdvices.Contains(advice))
		{
			_chargenAdvices.Remove(advice);
			return false;
		}

		_chargenAdvices.Add(advice);
		return true;
	}

	public string PersonWord(Gender gender)
	{
		return _personWord.ValueOrDefault(gender, null) ?? _personWord[Gender.Indeterminate];
	}

	public string Description { get; protected set; }

	public override string ToString()
	{
		return $"Culture ID={Id} Name={Name}";
	}

	#region IFutureProgVariable Members

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Culture, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			default:
				throw new ArgumentException();
		}
	}

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Culture;

	public object GetObject => this;

	#endregion

	#region ICulture Members

	private readonly List<ChargenResourceCost> _costs = new();

	public int ResourceCost(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => !x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public int ResourceRequirement(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public IFutureProg AvailabilityProg { get; protected set; }

	public bool ChargenAvailable(ICharacterTemplate template)
	{
		return _costs.Where(x => x.RequirementOnly)
		             .All(x => template.Account.AccountResources.ValueOrDefault(x.Resource, 0) >= x.Amount) &&
		       ((bool?)AvailabilityProg?.Execute(template) ?? true);
	}

	public double TolerableTemperatureFloorEffect { get; protected set; }
	public double TolerableTemperatureCeilingEffect { get; protected set; }

	private const string HelpInfo = @"You can use the following options with this subcommand:

    name <name> - renames this culture
    calendar <which> - changes the calender used by this culture
    nameculture <which> [all|male|female|neuter|nb|indeterminate] - changes the name culture used by this culture
    male|female|neuter|indeterminate <word> - changes the gendered words for someone of this culture
    desc - drops you into an editor for the culture description
    availability <prog> - sets a prog that controls appearance in character creation
    tempfloor <amount> - sets the tolerable temperature floor modifier
    tempceiling <amount> - sets the tolerable temperature ceiling modifier
    advice <which> - toggles a chargen advice applying to this culture
    cost <resource> <amount> - sets a cost for character creation
    require <resource> <amount> - sets a non-cost requirement for character creation
    cost <resource> clear - clears a resource cost for character creation";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "calendar":
				return BuildingCommandCalendar(actor, command);
			case "nameculture":
			case "name_culture":
			case "name culture":
			case "culture":
				return BuildingCommandNameCulture(actor, command);
			case "male":
				return BuildingCommandPersonWord(actor, command, Gender.Male);
			case "female":
				return BuildingCommandPersonWord(actor, command, Gender.Female);
			case "neuter":
				return BuildingCommandPersonWord(actor, command, Gender.Neuter);
			case "indeterminate":
				return BuildingCommandPersonWord(actor, command, Gender.Indeterminate);
			case "chargen":
			case "chargenprog":
			case "chargen prog":
			case "chargen_prog":
			case "availability":
			case "availability_prog":
			case "availability prog":
				return BuildingCommandChargenProg(actor, command);
			case "tempfloor":
			case "temp floor":
			case "temp_floor":
				return BuildingCommandTempFloor(actor, command);
			case "tempceiling":
			case "temp ceiling":
			case "temp_ceiling":
				return BuildingCommandTempCeiling(actor, command);
			case "skillprog":
			case "skill prog":
			case "skill":
			case "skill_prog":
				return BuildingCommandSkillProg(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "advice":
				return BuildingCommandAdvice(actor, command);
			case "cost":
			case "costs":
				return BuildingCommandCost(actor, command, true);
			case "require":
			case "requirement":
				return BuildingCommandCost(actor, command, false);
			default:
				actor.OutputHandler.Send(HelpInfo);
				return false;
		}
	}

	private bool BuildingCommandAdvice(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which chargen advice would you like to toggle applying to this culture?");
			return false;
		}

		var which = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.ChargenAdvices.Get(value)
			: Gameworld.ChargenAdvices.GetByName(command.SafeRemainingArgument);
		if (which == null)
		{
			actor.OutputHandler.Send("There is no such chargen advice.");
			return false;
		}

		if (_chargenAdvices.Contains(which))
		{
			_chargenAdvices.Remove(which);
			actor.OutputHandler.Send(
				$"This culture will no longer trigger the {which.AdviceTitle.ColourValue()} piece of chargen advice.");
		}
		else
		{
			_chargenAdvices.Add(which);
			actor.OutputHandler.Send(
				$"This culture will now trigger the {which.AdviceTitle.ColourValue()} piece of chargen advice.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandCost(ICharacter actor, StringStack command, bool isCost)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a chargen resource.");
			return false;
		}

		var which = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ChargenResources.Get(value)
			: Gameworld.ChargenResources.GetByName(command.Last) ??
			  Gameworld.ChargenResources.FirstOrDefault(x => x.Alias.EqualTo(command.Last));
		if (which == null)
		{
			actor.OutputHandler.Send("There is no such chargen resource.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a cost, or use the keyword {"clear".ColourCommand()} to clear a cost.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			var amount = _costs.RemoveAll(x => x.Resource == which);
			if (amount == 0)
			{
				actor.OutputHandler.Send("This culture has no such cost to clear.");
				return false;
			}

			Changed = true;
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} culture will no longer cost or require any {which.PluralName.ColourValue()}.");
			return true;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var cost))
		{
			actor.OutputHandler.Send("You must enter a valid cost.");
			return false;
		}

		_costs.Add(new ChargenResourceCost
		{
			Resource = which,
			Amount = cost,
			RequirementOnly = !isCost
		});
		Changed = true;
		actor.OutputHandler.Send(
			$"This culture will now {(isCost ? "cost" : "require, but not cost,")} {cost.ToString("N0", actor).ColourValue()} {(cost == 1 ? which.Name.ColourValue() : which.PluralName.ColourValue())}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(Description))
		{
			sb.AppendLine("Replacing:\n");
			sb.AppendLine(Description.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
		}

		sb.AppendLine("Enter the description in the editor below.");
		sb.AppendLine();
		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(BuildingCommandDescPost, BuildingCommandDescCancel, 1.0);
		return true;
	}

	private void BuildingCommandDescCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the description.");
	}

	private void BuildingCommandDescPost(string text, IOutputHandler handler, object[] arg3)
	{
		Description = text.Trim().ProperSentences();
		Changed = true;
		handler.Send($"You set the description of this culture to:\n\n{Description.Wrap(80, "\t")}");
	}

	private bool BuildingCommandSkillProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What prog do you want to use to determine the starting values of skills for this culture?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a number value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes>
			    { FutureProgVariableTypes.Chargen, FutureProgVariableTypes.Trait, FutureProgVariableTypes.Number }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts three parameters: a Chargen, a Trait and a Number, whereas {prog.MXPClickableFunctionNameWithId()} does not.");
			return false;
		}

		SkillStartingValueProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This culture will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine the starting skill values of newly created characters of this culture.");
		return true;
	}

	private bool BuildingCommandTempCeiling(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What modifier do you want this culture to have to the ceiling of tolerable temperatures?");
			return false;
		}

		var result = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument,
			Framework.Units.UnitType.TemperatureDelta, out var success);
		if (!success)
		{
			actor.OutputHandler.Send(
				"That is not a valid temperature. Don't forget to specify the units, e.g. 5C, -9F etc.");
			return false;
		}

		TolerableTemperatureCeilingEffect = result;
		Changed = true;
		actor.OutputHandler.Send(
			$"This culture now has a tolerable temperature ceiling that is {Gameworld.UnitManager.DescribeBonus(result, Framework.Units.UnitType.TemperatureDelta, actor).ColourValue()} different than usual.");
		return true;
	}

	private bool BuildingCommandTempFloor(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What modifier do you want this culture to have to the floor of tolerable temperatures?");
			return false;
		}

		var result = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument,
			Framework.Units.UnitType.TemperatureDelta, out var success);
		if (!success)
		{
			actor.OutputHandler.Send(
				"That is not a valid temperature. Don't forget to specify the units, e.g. 5C, -9F etc.");
			return false;
		}

		TolerableTemperatureFloorEffect = result;
		Changed = true;
		actor.OutputHandler.Send(
			$"This culture now has a tolerable temperature floor that is {Gameworld.UnitManager.DescribeBonus(result, Framework.Units.UnitType.TemperatureDelta, actor).ColourValue()} different than usual.");
		return true;
	}

	private bool BuildingCommandChargenProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What prog do you want to use to determine availability of this culture during character creation?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Chargen }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts one parameter: a Chargen, whereas {prog.MXPClickableFunctionNameWithId()} does not.");
			return false;
		}

		AvailabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This culture will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine whether it is a valid selection in chargen.");
		return true;
	}

	private bool BuildingCommandPersonWord(ICharacter actor, StringStack command, Gender gender)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What person word do you want to set for individuals of this culture who are of the {gender.DescribeEnum()} gender?");
			return false;
		}

		_personWord[gender] = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send(
			$"The word used to refer to individuals from this culture of the {gender.DescribeEnum()} gender will now be {_personWord[gender].ColourValue()}.");
		return true;
	}

	private bool BuildingCommandNameCulture(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which name culture do you want to set for this culture?");
			return false;
		}

		var culture = Gameworld.NameCultures.GetByIdOrName(command.PopSpeech());
		if (culture == null)
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return false;
		}

		switch (command.SafeRemainingArgument.CollapseString().ToLowerInvariant())
		{
			case "all":
			case "":
				_genderNameCultures[Gender.Male] = culture;
				_genderNameCultures[Gender.Female] = culture;
				_genderNameCultures[Gender.Neuter] = culture;
				_genderNameCultures[Gender.NonBinary] = culture;
				_genderNameCultures[Gender.Indeterminate] = culture;
				Changed = true;
				actor.OutputHandler.Send(
					$"This culture will now use the {culture.Name.ColourName()} name culture for all genders.");
				return true;
			case "male":
			case "men":
			case "man":
			case "masculine":
				_genderNameCultures[Gender.Male] = culture;
				Changed = true;
				actor.OutputHandler.Send(
					$"This culture will now use the {culture.Name.ColourName()} name culture for males.");
				return true;
			case "female":
			case "feminine":
			case "woman":
			case "women":
				_genderNameCultures[Gender.Female] = culture;
				Changed = true;
				actor.OutputHandler.Send(
					$"This culture will now use the {culture.Name.ColourName()} name culture for females.");
				return true;
			case "neuter":
			case "neutral":
				_genderNameCultures[Gender.Neuter] = culture;
				Changed = true;
				actor.OutputHandler.Send(
					$"This culture will now use the {culture.Name.ColourName()} name culture for neuter genders.");
				return true;
			case "nonbinary":
			case "nb":
				_genderNameCultures[Gender.NonBinary] = culture;
				Changed = true;
				actor.OutputHandler.Send(
					$"This culture will now use the {culture.Name.ColourName()} name culture for non-binary.");
				return true;
			case "indeterminate":
				_genderNameCultures[Gender.Indeterminate] = culture;
				Changed = true;
				actor.OutputHandler.Send(
					$"This culture will now use the {culture.Name.ColourName()} name culture for indeterminate genders.");
				return true;
			default:
				actor.OutputHandler.Send($"The valid values are {new List<string>
				{
					"male",
					"female",
					"non-binary",
					"neuter",
					"indeterminate",
					"all"
				}.Select(x => x.ColourValue()).ListToString()}.");
				return false;
		}
	}

	private bool BuildingCommandCalendar(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which calendar would you like to use for this culture in determining birthdays and such?");
			return false;
		}

		var calendar = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Calendars.Get(value)
			: Gameworld.Calendars.GetByName(command.SafeRemainingArgument);
		if (calendar == null)
		{
			actor.OutputHandler.Send("There is no such calendar.");
			return false;
		}

		PrimaryCalendar = calendar;
		Changed = true;
		actor.OutputHandler.Send(
			$"This culture will now use the {PrimaryCalendar.ShortName.ColourName()} calendar for the purposes of birthdays.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this culture?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Cultures.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a culture with that name. Culture names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You change the name of the {_name.ColourName()} culture to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Culture #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Calendar: {PrimaryCalendar.ShortName.ColourValue()}");
		sb.AppendLine($"Name Culture Male: {NameCultureForGender(Gender.Male).Name.ColourValue()}");
		sb.AppendLine($"Name Culture Female: {NameCultureForGender(Gender.Female).Name.ColourValue()}");
		sb.AppendLine($"Name Culture Non-Binary: {NameCultureForGender(Gender.NonBinary).Name.ColourValue()}");
		sb.AppendLine($"Name Culture Neuter: {NameCultureForGender(Gender.Neuter).Name.ColourValue()}");
		sb.AppendLine($"Name Culture Indeterminate: {NameCultureForGender(Gender.Indeterminate).Name.ColourValue()}");
		sb.AppendLine($"Person Word Male: {_personWord[Gender.Male].ColourCommand()}");
		sb.AppendLine($"Person Word Female: {_personWord[Gender.Female].ColourCommand()}");
		sb.AppendLine($"Person Word Neuter: {_personWord[Gender.Neuter].ColourCommand()}");
		sb.AppendLine($"Person Word Indeterminate: {_personWord[Gender.Indeterminate].ColourCommand()}");
		sb.AppendLine(
			$"Chargen Availability Prog: {AvailabilityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Starting Skill Value Prog: {SkillStartingValueProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Tolerable Temperature Ceiling: {Gameworld.UnitManager.DescribeBonus(TolerableTemperatureCeilingEffect, Framework.Units.UnitType.TemperatureDelta, actor).ColourValue()}");
		sb.AppendLine(
			$"Tolerable Temperature Floor: {Gameworld.UnitManager.DescribeBonus(TolerableTemperatureFloorEffect, Framework.Units.UnitType.TemperatureDelta, actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));

		if (_costs.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Chargen Costs:");
			foreach (var cost in _costs)
			{
				sb.AppendLine(
					$"\t{$"{cost.Amount.ToString("N0", actor)} {cost.Resource.Alias}".ColourValue()}{(cost.RequirementOnly ? " [not spent]".Colour(Telnet.BoldYellow) : "")}");
			}
		}

		if (_chargenAdvices.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Chargen Advice:");
			foreach (var advice in _chargenAdvices)
			{
				sb.AppendLine();
				sb.AppendLine(
					$"\t{advice.AdviceTitle.ColourCommand()} @ {advice.TargetStage.DescribeEnum(true).ColourValue()} (prog: {advice.ShouldShowAdviceProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)})");
				sb.AppendLine();
				sb.AppendLine(advice.AdviceText.Wrap(actor.InnerLineFormatLength, "\t\t"));
			}
		}

		return sb.ToString();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Cultures.Find(Id);
		dbitem.Name = Name;
		FMDB.Context.CulturesNameCultures.RemoveRange(dbitem.CulturesNameCultures);
		foreach (var nc in _genderNameCultures)
		{
			dbitem.CulturesNameCultures.Add(new CulturesNameCultures
			{
				Culture = dbitem,
				NameCultureId = nc.Value.Id,
				Gender = (short)nc.Key
			});
		}

		dbitem.SkillStartingValueProgId = SkillStartingValueProg.Id;
		dbitem.TolerableTemperatureCeilingEffect = TolerableTemperatureCeilingEffect;
		dbitem.TolerableTemperatureFloorEffect = TolerableTemperatureFloorEffect;
		dbitem.PersonWordFemale = _personWord[Gender.Female];
		dbitem.PersonWordIndeterminate = _personWord[Gender.Indeterminate];
		dbitem.PersonWordMale = _personWord[Gender.Male];
		dbitem.PersonWordNeuter = _personWord[Gender.Neuter];
		dbitem.AvailabilityProgId = AvailabilityProg?.Id;
		dbitem.Description = Description;
		dbitem.PrimaryCalendarId = PrimaryCalendar.Id;

		FMDB.Context.CulturesChargenResources.RemoveRange(dbitem.CulturesChargenResources);
		foreach (var cost in _costs)
		{
			dbitem.CulturesChargenResources.Add(new CulturesChargenResources
			{
				Culture = dbitem,
				ChargenResourceId = cost.Resource.Id,
				Amount = cost.Amount,
				RequirementOnly = cost.RequirementOnly
			});
		}

		FMDB.Context.ChargenAdvicesCultures.RemoveRange(dbitem.ChargenAdvicesCultures);
		foreach (var advice in _chargenAdvices)
		{
			dbitem.ChargenAdvicesCultures.Add(new ChargenAdvicesCultures
			{
				Culture = dbitem,
				ChargenAdviceId = advice.Id
			});
		}

		Changed = false;
	}

	#endregion
}