using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using OpenAI_API.Moderation;

namespace MudSharp.Character.Name;

public class RandomNameProfile : SaveableItem, IEditableItem, IRandomNameProfile
{
	private readonly Dictionary<NameUsage, string> _nameUsageDiceExpressionsDictionary =
		new();

	public IReadOnlyDictionary<NameUsage, string> NameUsageDiceExpressions => _nameUsageDiceExpressionsDictionary;

	private readonly CollectionDictionary<NameUsage, (string Value, int Weight)> _randomNamesDictionary =
		new();

	public IReadOnlyDictionary<NameUsage, List<(string Value, int Weight)>> RandomNames =>
		_randomNamesDictionary.ToDictionary();

	public RandomNameProfile(MudSharp.Models.RandomNameProfile profile, INameCulture culture)
	{
		Gameworld = culture.Gameworld;
		_id = profile.Id;
		_name = profile.Name;
		Culture = culture;
		Gender = (Gender)profile.Gender;
		_useForChargenSuggestionsProg = Gameworld.FutureProgs.Get(profile.UseForChargenSuggestionsProgId ?? 0) ??
										Gameworld.AlwaysFalseProg;
		foreach (var item in profile.RandomNameProfilesDiceExpressions)
		{
			_nameUsageDiceExpressionsDictionary.Add((NameUsage)item.NameUsage, item.DiceExpression);
		}

		foreach (var item in profile.RandomNameProfilesElements)
		{
			_randomNamesDictionary.Add((NameUsage)item.NameUsage, (item.Name, item.Weighting));
		}
	}

	public RandomNameProfile(INameCulture culture, Gender gender, string name)
	{
		Gameworld = culture.Gameworld;
		_useForChargenSuggestionsProg = Gameworld.AlwaysFalseProg;
		foreach (var usage in culture.NameCultureElements)
		{
			_nameUsageDiceExpressionsDictionary[usage.Usage] = usage.MinimumCount > 0
				? $"1d{usage.MaximumCount}"
				: $"1d{usage.MaximumCount + 1}-1";
		}

		using (new FMDB())
		{
			var dbitem = new Models.RandomNameProfile
			{
				Name = name,
				Gender = (short)gender,
				NameCultureId = culture.Id,
				UseForChargenSuggestionsProgId = _useForChargenSuggestionsProg?.Id
			};
			foreach (var expression in _nameUsageDiceExpressionsDictionary)
			{
				dbitem.RandomNameProfilesDiceExpressions.Add(new Models.RandomNameProfilesDiceExpressions
				{
					NameUsage = (int)expression.Key,
					RandomNameProfile = dbitem,
					DiceExpression = expression.Value
				});
			}

			FMDB.Context.RandomNameProfiles.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		_name = name;
		Gender = gender;
		Culture = culture;
	}

	public RandomNameProfile(RandomNameProfile rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.RandomNameProfile
			{
				Name = newName,
				Gender = (short)rhs.Gender,
				UseForChargenSuggestionsProgId = rhs._useForChargenSuggestionsProg?.Id
			};
			foreach (var expression in rhs._nameUsageDiceExpressionsDictionary)
			{
				dbitem.RandomNameProfilesDiceExpressions.Add(new Models.RandomNameProfilesDiceExpressions
					{ DiceExpression = expression.Value, NameUsage = (int)expression.Key, RandomNameProfile = dbitem });
			}

			foreach (var usage in rhs._randomNamesDictionary)
			foreach (var element in usage.Value)
			{
				dbitem.RandomNameProfilesElements.Add(new Models.RandomNameProfilesElements
				{
					RandomNameProfile = dbitem,
					NameUsage = (int)usage.Key,
					Name = element.Value,
					Weighting = element.Weight
				});
			}

			FMDB.Context.RandomNameProfiles.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			_name = newName;
			Culture = rhs.Culture;
			Gender = rhs.Gender;
			_useForChargenSuggestionsProg = rhs._useForChargenSuggestionsProg;
			foreach (var item in dbitem.RandomNameProfilesDiceExpressions)
			{
				_nameUsageDiceExpressionsDictionary.Add((NameUsage)item.NameUsage, item.DiceExpression);
			}

			foreach (var item in dbitem.RandomNameProfilesElements)
			{
				_randomNamesDictionary.Add((NameUsage)item.NameUsage, (item.Name, item.Weighting));
			}
		}
	}

	public override string FrameworkItemType => "RandomNameProfile";

	public INameCulture Culture { get; private set; }

	public Gender Gender { get; private set; }

	public bool IsCompatibleGender(Gender gender)
	{
		if (Gender == Gender.NonBinary)
		{
			return true;
		}

		if (Gender == Gender.Indeterminate)
		{
			return true;
		}

		return Gender == gender;
	}

	public bool IsReady => _nameUsageDiceExpressionsDictionary.All(x => _randomNamesDictionary.ContainsKey(x.Key));

	private IFutureProg _useForChargenSuggestionsProg;

	public IFutureProg UseForChargenSuggestionsProg => _useForChargenSuggestionsProg;

	public bool UseForChargenNameSuggestions(ICharacterTemplate template)
	{
		return _useForChargenSuggestionsProg is null ||
			   _useForChargenSuggestionsProg.Execute<bool?>(template) == true;
	}

	public string GetRandomNameElement(NameUsage usage)
	{
		return _randomNamesDictionary[usage].GetWeightedRandom(x => x.Weight).Value;
	}

	public IPersonalName GetRandomPersonalName(bool nonSaving = false)
	{
		var allResults = new List<string>();
		var resultsDictionary = new Dictionary<NameUsage, List<string>>();
		foreach (var element in _nameUsageDiceExpressionsDictionary)
		{
			resultsDictionary.Add(element.Key, new List<string>());
			if (!_randomNamesDictionary[element.Key].Any())
			{
				resultsDictionary[element.Key].Add("Unnamed");
				allResults.Add("Unnamed");
			}
						
			var number = Dice.Roll(element.Value);
			for (var i = 0; i < number; i++)
			{
				var result = _randomNamesDictionary[element.Key].GetWeightedRandom();
				if (result == null)
				{
					break;
				}

				if (allResults.Contains(result))
				{
					i--;
					continue;
				}

				resultsDictionary[element.Key].Add(result);
				allResults.Add(result);
			}
		}

		return new PersonalName(Culture, resultsDictionary, nonSaving);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.RandomNameProfiles.Find(Id);
		dbitem.Name = Name;
		dbitem.Gender = (short)Gender;
		dbitem.UseForChargenSuggestionsProgId = _useForChargenSuggestionsProg?.Id;
		FMDB.Context.RandomNameProfilesDiceExpressions.RemoveRange(dbitem.RandomNameProfilesDiceExpressions);
		foreach (var expression in _nameUsageDiceExpressionsDictionary)
		{
			dbitem.RandomNameProfilesDiceExpressions.Add(new Models.RandomNameProfilesDiceExpressions
				{ DiceExpression = expression.Value, NameUsage = (int)expression.Key, RandomNameProfile = dbitem });
		}

		FMDB.Context.RandomNameProfilesElements.RemoveRange(dbitem.RandomNameProfilesElements);
		foreach (var usage in _randomNamesDictionary)
		foreach (var element in usage.Value)
		{
			dbitem.RandomNameProfilesElements.Add(new Models.RandomNameProfilesElements
			{
				RandomNameProfile = dbitem,
				NameUsage = (int)usage.Key,
				Name = element.Value,
				Weighting = element.Weight
			});
		}

		Changed = false;
	}

	private const string BuildingCommandHelp = @"You can use the following options with this subcommand:

	name <name> - renames the random name profile
	gender <gender> - changes the gender of the random name profile
	chargen <prog> - sets a prog to control using this random name profile for suggestions in chargen
	dice <element> <dice expression> - sets the number of elements that get generated with each name using a dice expression
	<element> add <name> [<weight>] - adds a new name of a specified name element type
	<element> remove <name> - removes a name of a specified name element type
	<element> weight <name> <weight> - changes the weighting of a particular name of a specified name element type";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "gender":
				return BuildingCommandGender(actor, command);
			case "dice":
			case "chance":
			case "chances":
				return BuildingCommandDice(actor, command);
			case "chargen":
				return BuildingCommandChargen(actor, command);
			case "help":
			case "?":
			case "":
				actor.OutputHandler.Send(BuildingCommandHelp);
				return false;
		}

		var name = command.Last;
		var element = Culture.NameCultureElements.FirstOrDefault(x => x.Name.EqualTo(name)) ??
					  (name.TryParseEnum<NameUsage>(out var usage)
						  ? Culture.NameCultureElements.FirstOrDefault(x => x.Usage == usage)
						  : null);
		if (element == null)
		{
			actor.OutputHandler.Send(
				$"The naming culture that this random name profile is tied to has no such name element. The valid choices are {Culture.NameCultureElements.Select(x => x.Name.ColourValue()).ListToString()}.");
			return false;
		}

		return BuildingCommandElement(actor, command, element);
	}

	private bool BuildingCommandChargen(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog should control whether this random name profile is used for suggestions for names in chargen?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new ProgVariableTypes[]
			{
				ProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_useForChargenSuggestionsProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This random name profile now uses the {prog.MXPClickableFunctionName()} prog to control whether it is used for suggestions in chargen.");
		return true;
	}

	private bool BuildingCommandElement(ICharacter actor, StringStack command, NameCultureElement element)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Do you want to {"add".ColourCommand()}, {"remove".ColourCommand()} or set the {"weight".ColourCommand()} of an existing element?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "create":
			case "new":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What name do you want to add?");
					return false;
				}

				var name = command.PopSpeech().TitleCase();
				var weight = 100;
				if (!command.IsFinished)
				{
					if (!int.TryParse(command.SafeRemainingArgument, out weight) || weight <= 0)
					{
						actor.OutputHandler.Send(
							"If you specify a weight, you must specify a valid number greater than zero.");
						return false;
					}
				}

				_randomNamesDictionary.Add(element.Usage, (name, weight));
				actor.OutputHandler.Send(
					$"You add a new {element.Name.ColourName()} to the {Name.ColourName()} random name profile: {name.ColourName()} @ {weight.ToString("N0", actor).ColourValue()}");
				Changed = true;
				return true;
			case "remove":
			case "rem":
			case "delete":
			case "del":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send($"What {element.Name.ColourName()} do you want to remove?");
					return false;
				}

				name = command.SafeRemainingArgument;
				var count = _randomNamesDictionary[element.Usage].RemoveAll(x => x.Value.EqualTo(name));
				if (count == 0)
				{
					actor.OutputHandler.Send(
						$"There is no such {element.Name.ColourName()} for you to remove from the {Name.ColourName()} random name profile.");
					return false;
				}

				Changed = true;
				actor.OutputHandler.Send(
					$"You remove {count.ToString("N0", actor).ColourValue()} {element.Name.Pluralise().ColourName()} with the value of {name.ColourName()} from the {Name.ColourName()} random name profile.");
				return true;
			case "weight":
			case "chance":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send($"What {element.Name.ColourName()} do you want to change the weight of?");
					return false;
				}

				name = command.PopSpeech().TitleCase();
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What new weight do you want to give it?");
					return false;
				}

				if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
				{
					actor.OutputHandler.Send("You must enter a valid number greater than zero.");
					return false;
				}

				count = _randomNamesDictionary[element.Usage].RemoveAll(x => x.Value.EqualTo(name));
				if (count == 0)
				{
					actor.OutputHandler.Send(
						$"There is no such {element.Name.ColourName()} for you to change the weight of in the {Name.ColourName()} random name profile.");
					return false;
				}

				_randomNamesDictionary[element.Usage].Add((name, value));
				Changed = true;
				actor.OutputHandler.Send(
					$"The {element.Name.ColourName()} with a value of {name.ColourName()} will now be weighted at {value.ToString("N0", actor).ColourValue()} in the {Name.ColourName()} random name profile.");
				return true;
		}

		actor.OutputHandler.Send(
			$"You can {"add".ColourCommand()}, {"remove".ColourCommand()} or set the {"weight".ColourCommand()} of an existing element.");
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this random name profile?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.RandomNameProfiles.Where(x => x.Culture == Culture).Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a random name profile with that name for this name culture. Names must be unique per name culture.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {_name.ColourName()} random name profile to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandGender(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which gender would you like to set as the requisite gender for this random name profile?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Gender>(out var gender))
		{
			actor.OutputHandler.Send(
				$"That is not a valid gender. The valid options are {Enum.GetNames<Gender>().Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		Gender = gender;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} random name profile is now applicable to the {gender.DescribeEnum()} gender.");
		return true;
	}

	private bool BuildingCommandDice(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which name element would you like to set the dice roll for? The valid choices are {Culture.NameCultureElements.Select(x => x.Name.ColourValue()).ListToString()}.");
			return false;
		}

		var name = command.SafeRemainingArgument;
		var element = Culture.NameCultureElements.FirstOrDefault(x => x.Name.EqualTo(name)) ??
					  (name.TryParseEnum<NameUsage>(out var theEnum)
						  ? Culture.NameCultureElements.FirstOrDefault(x => x.Usage == theEnum)
						  : null);
		if (element == null)
		{
			actor.OutputHandler.Send(
				$"The naming culture that this random name profile is tied to has no such name element. The valid choices are {Culture.NameCultureElements.Select(x => x.Name.ColourValue()).ListToString()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a dice expression for the number of elements.");
			return false;
		}

		if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send("That is not a valid dice expression.");
			return false;
		}

		_nameUsageDiceExpressionsDictionary[element.Usage] = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"The dice expression for the number of {element.Name.Pluralise().ColourName()} is now {command.SafeRemainingArgument.ColourCommand()}.");
		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Random Name Profile #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Name Culture: {Culture.Name.ColourName()}");
		sb.AppendLine($"Gender: {Gender.DescribeEnum().ColourValue()}");
		sb.AppendLine(
			$"Chargen Prog: {_useForChargenSuggestionsProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine($"Element Chances:");
		foreach (var chance in _nameUsageDiceExpressionsDictionary)
		{
			sb.AppendLine($"\t{chance.Key.DescribeEnum().ColourValue()}: {chance.Value.ColourCommand()}");
		}

		sb.AppendLine();
		foreach (var usage in _randomNamesDictionary)
		{
			sb.AppendLine();
			sb.AppendLine($"{usage.Key.DescribeEnum(true).Pluralise()}:");
			sb.AppendLine();
			var totalWeight = (double)usage.Value.Sum(x => x.Weight);
			var strings = usage.Value.OrderByDescending(x => x.Weight).Select(x =>
				$"{x.Value.ColourName()} [{(x.Weight / totalWeight).ToString("P3", actor)}]").ToArray();
			if (!strings.Any())
			{
				continue;
			}

			var longest = strings.Max(x => x.Length);
			sb.AppendLineColumns((uint)actor.LineFormatLength, (uint)(actor.LineFormatLength / (longest + 4)), strings);
		}

		return sb.ToString();
	}
}