using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.Character.Heritage;

public class Ethnicity : SaveableItem, IEthnicity
{
	public Ethnicity(MudSharp.Models.Ethnicity ethnicity, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = ethnicity.Id;
		_name = ethnicity.Name;
		ChargenBlurb = ethnicity.ChargenBlurb;
		CharacteristicChoices = new Dictionary<ICharacteristicDefinition, ICharacteristicProfile>();
		AvailabilityProg = gameworld.FutureProgs.Get(ethnicity.AvailabilityProgId ?? 0);
		ParentRace = gameworld.Races.Get(ethnicity.ParentRaceId ?? 0);
		foreach (var item in ethnicity.EthnicitiesCharacteristics)
		{
			var definition = gameworld.Characteristics.Get(item.CharacteristicDefinitionId);
			var profile = gameworld.CharacteristicProfiles.Get(item.CharacteristicProfileId);
			if (definition == null || profile == null)
			{
				continue;
			}

			CharacteristicChoices.Add(definition, profile);
		}

		// Fix bad building
		if (ParentRace is not null)
		{
			foreach (var item in ParentRace.Characteristics(Gender.Indeterminate))
			{
				if (CharacteristicChoices.ContainsKey(item))
				{
					continue;
				}

				var all = gameworld.CharacteristicProfiles.Where(x => x.IsProfileFor(item))
				                   .OrderByDescending(x => x.TargetDefinition == item)
				                   .FirstOrDefault(x => x.Type == "All");
				if (all is null)
				{
					continue;
				}

				CharacteristicChoices[item] = all;
				Changed = true;
			}
		}

		foreach (var item in ethnicity.EthnicitiesChargenResources)
		{
			_costs.Add(new ChargenResourceCost
			{
				Amount = item.Amount,
				RequirementOnly = item.RequirementOnly,
				Resource = gameworld.ChargenResources.Get(item.ChargenResourceId)
			});
		}

		EthnicGroup = ethnicity.EthnicGroup;
		EthnicSubgroup = ethnicity.EthnicSubgroup;
		PopulationBloodModel = gameworld.PopulationBloodModels.Get(ethnicity.PopulationBloodModelId ?? 0);

		foreach (var item in ethnicity.ChargenAdvicesEthnicities)
		{
			_chargenAdvices.Add(gameworld.ChargenAdvices.Get(item.ChargenAdviceId));
		}

		TolerableTemperatureCeilingEffect = ethnicity.TolerableTemperatureCeilingEffect;
		TolerableTemperatureFloorEffect = ethnicity.TolerableTemperatureFloorEffect;
	}

	public Ethnicity(IFuturemud gameworld, IRace race, string name)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.Ethnicity
			{
				Name = name,
				ParentRaceId = race.Id,
				ChargenBlurb = "An undescribed ethnicity.",
				TolerableTemperatureCeilingEffect = 0.0,
				TolerableTemperatureFloorEffect = 0.0
			};
			FMDB.Context.Ethnicities.Add(dbitem);
			FMDB.Context.SaveChanges();
			foreach (var characteristic in race.Characteristics(Gender.Indeterminate))
			{
				var profiles = Gameworld.CharacteristicProfiles.Where(x => x.TargetDefinition == characteristic)
				                        .ToList();
				var profile = profiles.FirstOrDefault(x => x.Type == "All") ?? profiles.First();
				dbitem.EthnicitiesCharacteristics.Add(new Models.EthnicitiesCharacteristics
				{
					CharacteristicDefinitionId = characteristic.Id,
					Ethnicity = dbitem,
					CharacteristicProfileId = profile.Id
				});

				CharacteristicChoices[characteristic] = profile;
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		_name = name;
		ParentRace = race;
		ChargenBlurb = "An undescribed ethnicity.";
		TolerableTemperatureCeilingEffect = 0.0;
		TolerableTemperatureFloorEffect = 0.0;
	}

	public Ethnicity(IEthnicity rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		ParentRace = rhs.ParentRace;
		_name = newName;
		TolerableTemperatureCeilingEffect = rhs.TolerableTemperatureCeilingEffect;
		TolerableTemperatureFloorEffect = rhs.TolerableTemperatureFloorEffect;
		ChargenBlurb = rhs.ChargenBlurb;
		EthnicGroup = rhs.EthnicGroup;
		EthnicSubgroup = rhs.EthnicSubgroup;
		PopulationBloodModel = rhs.PopulationBloodModel;
		foreach (var characteristic in rhs.CharacteristicChoices)
		{
			CharacteristicChoices[characteristic.Key] = characteristic.Value;
		}

		foreach (var cost in rhs.Costs)
		{
			_costs.Add(new ChargenResourceCost
			{
				Resource = cost.Resource,
				RequirementOnly = cost.RequirementOnly,
				Amount = cost.Amount
			});
		}

		foreach (var advice in rhs.ChargenAdvices)
		{
			_chargenAdvices.Add(advice);
		}

		using (new FMDB())
		{
			var dbitem = new Models.Ethnicity
			{
				Name = Name,
				TolerableTemperatureCeilingEffect = TolerableTemperatureCeilingEffect,
				TolerableTemperatureFloorEffect = TolerableTemperatureFloorEffect,
				ParentRaceId = ParentRace.Id,
				AvailabilityProgId = AvailabilityProg?.Id,
				ChargenBlurb = ChargenBlurb,
				EthnicGroup = EthnicGroup,
				EthnicSubgroup = EthnicSubgroup,
				PopulationBloodModelId = PopulationBloodModel?.Id
			};
			FMDB.Context.Ethnicities.Add(dbitem);
			FMDB.Context.SaveChanges();

			foreach (var characteristic in CharacteristicChoices)
			{
				dbitem.EthnicitiesCharacteristics.Add(new Models.EthnicitiesCharacteristics
				{
					Ethnicity = dbitem,
					CharacteristicDefinitionId = characteristic.Key.Id,
					CharacteristicProfileId = characteristic.Value.Id
				});
			}

			foreach (var cost in Costs)
			{
				FMDB.Context.EthnicitiesChargenResources.Add(new Models.EthnicitiesChargenResources
				{
					Ethnicity = dbitem,
					ChargenResourceId = cost.Resource.Id,
					Amount = cost.Amount,
					RequirementOnly = cost.RequirementOnly
				});
			}

			foreach (var advice in ChargenAdvices)
			{
				FMDB.Context.ChargenAdvicesEthnicities.Add(new Models.ChargenAdvicesEthnicities
				{
					ChargenAdviceId = advice.Id,
					Ethnicity = dbitem
				});
			}

			FMDB.Context.SaveChanges();

			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "Ethnicity";

	public override string ToString()
	{
		return $"Ethnicity ID={Id} Name={Name}";
	}

	#region IEthnicity Members

	public IRace ParentRace { get; }

	public string EthnicGroup { get; protected set; }
	public string EthnicSubgroup { get; protected set; }
	public IPopulationBloodModel PopulationBloodModel { get; protected set; }
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

	public string ChargenBlurb { get; protected set; }

	public IDictionary<ICharacteristicDefinition, ICharacteristicProfile> CharacteristicChoices { get; protected set; } = new Dictionary<ICharacteristicDefinition, ICharacteristicProfile>();

	public IFutureProg AvailabilityProg { get; protected set; }

	public bool ChargenAvailable(ICharacterTemplate template)
	{
		return _costs.Where(x => x.RequirementOnly)
		             .All(x => template.Account.AccountResources.ValueOrDefault(x.Resource, 0) >= x.Amount) &&
		       ((bool?)AvailabilityProg?.Execute(template) ?? true);
	}

	private readonly List<ChargenResourceCost> _costs = new();

	public IEnumerable<ChargenResourceCost> Costs => _costs;

	public int ResourceCost(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => !x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public int ResourceRequirement(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public double TolerableTemperatureFloorEffect { get; protected set; }
	public double TolerableTemperatureCeilingEffect { get; protected set; }

	#endregion

	#region IFutureProgVariable Members

	private static FutureProgVariableTypes DotReferenceHandler(string property)
	{
		var returnVar = FutureProgVariableTypes.Error;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = FutureProgVariableTypes.Number;
				break;
			case "name":
				returnVar = FutureProgVariableTypes.Text;
				break;
			case "ethnicgroup":
			case "group":
				returnVar = FutureProgVariableTypes.Text;
				break;
			case "ethnicsubgroup":
			case "subgroup":
				returnVar = FutureProgVariableTypes.Text;
				break;
			case "parentrace":
				returnVar = FutureProgVariableTypes.Race;
				break;
		}

		return returnVar;
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "group", FutureProgVariableTypes.Text },
			{ "subgroup", FutureProgVariableTypes.Text },
			{ "ethnicgroup", FutureProgVariableTypes.Text },
			{ "ethnicsubgroup", FutureProgVariableTypes.Text },
			{ "parentrace", FutureProgVariableTypes.Race }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The Id of the ethnicity" },
			{ "name", "The name of the ethnicity" },
			{ "group", "The ethnic group to which this ethnicity belongs" },
			{ "subgroup", "The ethnic sub group to which this ethnicity belongs" },
			{ "parentrace", "Which race this ethnicity belongs to" },
			{ "ethnicgroup", "An alias for the GROUP property" },
			{ "ethnicsubgroup", "An alias for the SUBGROUP property" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Ethnicity, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public IFutureProgVariable GetProperty(string property)
	{
		IFutureProgVariable returnVar = null;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = new NumberVariable(Id);
				break;
			case "name":
				returnVar = new TextVariable(Name);
				break;
			case "ethnicgroup":
			case "group":
				returnVar = new TextVariable(EthnicGroup);
				break;
			case "ethnicsubgroup":
			case "subgroup":
				returnVar = new TextVariable(EthnicSubgroup);
				break;
			case "parentrace":
				returnVar = ParentRace;
				break;
		}

		return returnVar;
	}

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Ethnicity;

	public object GetObject => this;

	#endregion

	public override void Save()
	{
		var dbitem = FMDB.Context.Ethnicities.Find(Id);
		dbitem.Name = Name;
		dbitem.ChargenBlurb = ChargenBlurb;
		dbitem.EthnicGroup = EthnicGroup;
		dbitem.EthnicSubgroup = EthnicSubgroup;
		dbitem.AvailabilityProgId = AvailabilityProg?.Id;
		dbitem.PopulationBloodModelId = PopulationBloodModel?.Id;
		dbitem.TolerableTemperatureCeilingEffect = TolerableTemperatureCeilingEffect;
		dbitem.TolerableTemperatureFloorEffect = TolerableTemperatureFloorEffect;

		FMDB.Context.EthnicitiesCharacteristics.RemoveRange(dbitem.EthnicitiesCharacteristics);
		foreach (var characteristic in CharacteristicChoices)
		{
			dbitem.EthnicitiesCharacteristics.Add(new Models.EthnicitiesCharacteristics
			{
				Ethnicity = dbitem,
				CharacteristicDefinitionId = characteristic.Key.Id,
				CharacteristicProfileId = characteristic.Value.Id
			});
		}

		FMDB.Context.EthnicitiesChargenResources.RemoveRange(dbitem.EthnicitiesChargenResources);
		foreach (var cost in _costs)
		{
			dbitem.EthnicitiesChargenResources.Add(new Models.EthnicitiesChargenResources
			{
				Ethnicity = dbitem,
				ChargenResourceId = cost.Resource.Id,
				Amount = cost.Amount,
				RequirementOnly = cost.RequirementOnly
			});
		}

		FMDB.Context.ChargenAdvicesEthnicities.RemoveRange(dbitem.ChargenAdvicesEthnicities);
		foreach (var advice in _chargenAdvices)
		{
			dbitem.ChargenAdvicesEthnicities.Add(new Models.ChargenAdvicesEthnicities
			{
				Ethnicity = dbitem,
				ChargenAdviceId = advice.Id
			});
		}

		Changed = false;
	}

	private const string HelpText = @"You can use the following options with this command:

    name <name> - renames this ethnicity
    group <group> - sets an ethnic group
    group clear - clears an ethnic group
    subgroup <group> - sets an ethnic subgroup
    subgroup clear - clears an ethnic subgroup
    desc - drops you into an editor for the ethnicity description
    availability <prog> - sets a prog that controls appearance in character creation
    bloodmodel <model> - sets the population blood model for this ethnicity
    tempfloor <amount> - sets the tolerable temperature floor modifier
    tempceiling <amount> - sets the tolerable temperature ceiling modifier
    characteristic <which> <profile> - sets the characteristic profile for this ethnicity
    advice <which> - toggles a chargen advice applying to this ethnicity
    cost <resource> <amount> - sets a cost for character creation
    require <resource> <amount> - sets a non-cost requirement for character creation
    cost <resource> clear - clears a resource cost for character creation";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "group":
				return BuildingCommandGroup(actor, command);
			case "subgroup":
				return BuildingCommandSubgroup(actor, command);
			case "availability":
			case "chargen":
			case "availabilityprog":
			case "chargenprog":
				return BuildingCommandChargenProg(actor, command);
			case "blood":
			case "bloodmodel":
				return BuildingCommandBloodModel(actor, command);
			case "tempfloor":
				return BuildingCommandTempFloor(actor, command);
			case "tempceiling":
				return BuildingCommandTempCeiling(actor, command);
			case "characteristic":
			case "char":
			case "variable":
			case "var":
				return BuildingCommandCharacteristic(actor, command);
			case "advice":
				return BuildingCommandAdvice(actor, command);
			case "cost":
			case "costs":
				return BuildingCommandCost(actor, command, true);
			case "require":
			case "requirement":
				return BuildingCommandCost(actor, command, false);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
		}

		actor.OutputHandler.Send(HelpText);
		return false;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(ChargenBlurb))
		{
			sb.AppendLine("Replacing:\n");
			sb.AppendLine(ChargenBlurb.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
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
		ChargenBlurb = text.Trim().ProperSentences();
		Changed = true;
		handler.Send($"You set the description of this ethnicity to:\n\n{ChargenBlurb.Wrap(80, "\t")}");
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
				actor.OutputHandler.Send("This ethnicity has no such cost to clear.");
				return false;
			}

			Changed = true;
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} ethnicity will no longer cost or require any {which.PluralName.ColourValue()}.");
			return true;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var cost))
		{
			actor.OutputHandler.Send("You must enter a valid cost.");
			return false;
		}

		_costs.RemoveAll(x => x.Resource == which);
		_costs.Add(new ChargenResourceCost
		{
			Resource = which,
			Amount = cost,
			RequirementOnly = !isCost
		});
		Changed = true;
		actor.OutputHandler.Send(
			$"This ethnicity will now {(isCost ? "cost" : "require, but not cost,")} {cost.ToString("N0", actor).ColourValue()} {(cost == 1 ? which.Name.ColourValue() : which.PluralName.ColourValue())}.");
		return true;
	}

	private bool BuildingCommandAdvice(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which chargen advice would you like to toggle applying to this ethnicity?");
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
				$"This ethnicity will no longer trigger the {which.AdviceTitle.ColourValue()} piece of chargen advice.");
		}
		else
		{
			_chargenAdvices.Add(which);
			actor.OutputHandler.Send(
				$"This ethnicity will now trigger the {which.AdviceTitle.ColourValue()} piece of chargen advice.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandCharacteristic(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which characteristic definition do you want to edit the profile for? The valid choices are {CharacteristicChoices.Keys.Select(x => x.Name.ColourValue()).ListToString()}.");
			return false;
		}

		var name = command.PopSpeech();
		var choice = CharacteristicChoices.Keys.FirstOrDefault(x => x.Name.EqualTo(name)) ??
		             CharacteristicChoices.Keys.FirstOrDefault(x =>
			             x.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
		if (choice == null)
		{
			actor.OutputHandler.Send(
				$"That is not a valid characteristic definition. The valid choices are {CharacteristicChoices.Keys.Select(x => x.Name.ColourValue()).ListToString()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which characteristic profile do you want to set for the {choice.Name.ColourValue()} characteristic? The valid choices are {Gameworld.CharacteristicProfiles.Where(x => x.TargetDefinition == choice).Select(x => x.Name.ColourValue()).ListToString()}.");
			return false;
		}

		var profileName = command.SafeRemainingArgument;
		var profile = Gameworld.CharacteristicProfiles.Where(x => x.TargetDefinition == choice)
		                       .FirstOrDefault(x => x.Name.EqualTo(profileName)) ??
		              Gameworld.CharacteristicProfiles.Where(x => x.TargetDefinition == choice).FirstOrDefault(x =>
			              x.Name.StartsWith(profileName, StringComparison.InvariantCultureIgnoreCase));
		if (profile == null)
		{
			actor.OutputHandler.Send(
				$"There is no such characteristic profile for the {choice.Name.ColourValue()} characteristic. The valid choices are {Gameworld.CharacteristicProfiles.Where(x => x.TargetDefinition == choice).Select(x => x.Name.ColourValue()).ListToString()}.");
			return false;
		}

		CharacteristicChoices[choice] = profile;
		Changed = true;
		actor.OutputHandler.Send(
			$"This ethnicity will now use the {profile.Name.ColourValue()} characteristic profile to constrain values for the {choice.Name.ColourValue()} characteristic.");
		return true;
	}

	private bool BuildingCommandGroup(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var previous = Gameworld.Ethnicities.Where(x => x.ParentRace == ParentRace)
			                        .SelectNotNull(x => x.EthnicGroup?.ColourValue()).Distinct();
			actor.OutputHandler.Send(
				$"Which ethnic group would you like this ethnicity to belong to?\nThe {ParentRace.Name.ColourName()} race has the following ethnic groups already: {previous.ListToString()}");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			EthnicGroup = null;
			Changed = true;
			actor.OutputHandler.Send($"The {Name.ColourName()} ethnicity is no longer a part of any ethnic group.");
			return true;
		}

		var group = command.SafeRemainingArgument.TitleCase();
		EthnicGroup = group;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} ethnicity is now a part of the {EthnicGroup.ColourValue()} ethnic group.");
		return true;
	}

	private bool BuildingCommandSubgroup(ICharacter actor, StringStack command)
	{
		if (string.IsNullOrEmpty(EthnicGroup))
		{
			actor.OutputHandler.Send(
				"This ethnicity does not yet have an ethnic group. It must have an ethnic group before it can have a subgroup.");
			return false;
		}

		if (command.IsFinished)
		{
			var previous = Gameworld.Ethnicities
			                        .Where(x => x.ParentRace == ParentRace && x.EthnicGroup.EqualTo(EthnicGroup))
			                        .SelectNotNull(x => x.EthnicSubgroup?.ColourValue()).Distinct();
			actor.OutputHandler.Send(
				$"Which ethnic subgroup would you like this ethnicity to belong to?\nThe {ParentRace.Name.ColourName()} race's {EthnicGroup.ColourValue()} ethnic group has the following subgroups already: {previous.ListToString()}");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			EthnicSubgroup = null;
			Changed = true;
			actor.OutputHandler.Send($"The {Name.ColourName()} ethnicity is no longer a part of any ethnic subgroup.");
			return true;
		}

		var group = command.SafeRemainingArgument.TitleCase();
		EthnicSubgroup = group;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} ethnicity is now a part of the {EthnicSubgroup.ColourValue()} ethnic subgroup.");
		return true;
	}

	private bool BuildingCommandChargenProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What prog do you want to use to determine availability of this ethnicity during character creation?");
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
			$"This ethnicity will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine whether it is a valid selection in chargen.");
		return true;
	}

	private bool BuildingCommandBloodModel(ICharacter actor, StringStack command)
	{
		if (ParentRace.BloodModel == null)
		{
			actor.OutputHandler.Send("The parent race of this ethnicity does not use a blood model.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a valid population blood model (see {"show popbloodmodels".FluentTagMXP("send", "href='show popbloodmodels' hint='View the list of population blood models'")}), or use {"clear".ColourCommand()} to clear the existing one.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			PopulationBloodModel = null;
			Changed = true;
			actor.OutputHandler.Send($"This ethnicity will no longer use a specific population blood model.");
			return true;
		}

		var model = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.PopulationBloodModels.Get(value)
			: Gameworld.PopulationBloodModels.GetByName(command.SafeRemainingArgument);
		if (model == null)
		{
			actor.OutputHandler.Send("There is no such population blood model.");
			return false;
		}

		if (model.BloodModel != ParentRace.BloodModel)
		{
			actor.OutputHandler.Send(
				$"The parent race for this ethnicity uses the {ParentRace.BloodModel.Name.ColourName()} blood model, not the {model.BloodModel.Name.ColourName()} as the population model you specified does.");
			return false;
		}

		PopulationBloodModel = model;
		Changed = true;
		actor.OutputHandler.Send($"This ethnicity now uses the {model.Name.ColourName()} population blood model.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this ethnicity?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Ethnicities.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already an ethnicity with that name. Ethnicity names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You change the name of the {_name.ColourName()} ethnicity to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandTempCeiling(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What modifier do you want this ethnicity to have to the ceiling of tolerable temperatures?");
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
			$"This ethnicity now has a tolerable temperature ceiling that is {Gameworld.UnitManager.DescribeBonus(result, Framework.Units.UnitType.TemperatureDelta, actor).ColourValue()} different than usual.");
		return true;
	}

	private bool BuildingCommandTempFloor(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What modifier do you want this ethnicity to have to the floor of tolerable temperatures?");
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
			$"This ethnicity now has a tolerable temperature floor that is {Gameworld.UnitManager.DescribeBonus(result, Framework.Units.UnitType.TemperatureDelta, actor).ColourValue()} different than usual.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Ethnicity #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Race: {ParentRace.Name.ColourValue()}");
		sb.AppendLine($"Group: {EthnicGroup?.ColourValue() ?? ""}");
		sb.AppendLine($"Subgroup: {EthnicSubgroup?.ColourValue() ?? ""}");
		sb.AppendLine(
			$"Availability Prog: {AvailabilityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Population Blood Model: {PopulationBloodModel?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Tolerable Temperature Ceiling: {Gameworld.UnitManager.DescribeBonus(TolerableTemperatureCeilingEffect, Framework.Units.UnitType.TemperatureDelta, actor).ColourValue()}");
		sb.AppendLine(
			$"Tolerable Temperature Floor: {Gameworld.UnitManager.DescribeBonus(TolerableTemperatureFloorEffect, Framework.Units.UnitType.TemperatureDelta, actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(ChargenBlurb.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Characteristics:");
		sb.AppendLine();
		foreach (var choice in CharacteristicChoices.OrderBy(x => x.Key.Name))
		{
			sb.AppendLine($"\t{choice.Key.Name.ColourName()}: {choice.Value.Name.ColourValue()}");
		}

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
}