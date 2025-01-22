using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character.Name;
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
using MudSharp.Models;
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

		foreach (var nc in ethnicity.EthnicitiesNameCultures)
		{
			_genderNameCultures[(Gender)nc.Gender] = gameworld.NameCultures.Get(nc.NameCultureId);
		}

		foreach (var gender in Enum.GetValues<Gender>())
		{
			_genderNameCultures.TryAdd(gender, null);
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

		foreach (var gender in Enum.GetValues<Gender>())
		{
			_genderNameCultures.TryAdd(gender, null);
		}
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

		_genderNameCultures[Gender.Male] = rhs.NameCultureForGender(Gender.Male);
		_genderNameCultures[Gender.Female] = rhs.NameCultureForGender(Gender.Female);
		_genderNameCultures[Gender.Neuter] = rhs.NameCultureForGender(Gender.Neuter);
		_genderNameCultures[Gender.NonBinary] = rhs.NameCultureForGender(Gender.NonBinary);
		_genderNameCultures[Gender.Indeterminate] = rhs.NameCultureForGender(Gender.Indeterminate);

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
					 .All(x => template.Account.AccountResources[x.Resource] >= x.Amount) &&
			   (AvailabilityProg?.ExecuteBool(template, this) ?? true);
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

	private Dictionary<Gender, INameCulture> _genderNameCultures = new();

	public INameCulture? NameCultureForGender(Gender gender)
	{
		return _genderNameCultures[gender];
	}

	public IEnumerable<INameCulture> NameCultures => _genderNameCultures.Values.Where(x => x is not null).Distinct();

	#endregion

	#region IFutureProgVariable Members
	
	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "group", ProgVariableTypes.Text },
			{ "subgroup", ProgVariableTypes.Text },
			{ "ethnicgroup", ProgVariableTypes.Text },
			{ "ethnicsubgroup", ProgVariableTypes.Text },
			{ "parentrace", ProgVariableTypes.Race },
			{ "namecultures", ProgVariableTypes.Text | ProgVariableTypes.Collection},
			{ "tempfloor", ProgVariableTypes.Number },
			{ "tempceiling", ProgVariableTypes.Number },
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
			{ "ethnicsubgroup", "An alias for the SUBGROUP property" },
			{ "namecultures", "A collection of the name cultures for this ethnicity, if present"},
			{ "tempfloor", "The modifier to the floor of tolerable temperatures" },
			{ "tempceiling", "The modifier to the ceiling of tolerable temperatures" },
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Ethnicity, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public IProgVariable GetProperty(string property)
	{
		IProgVariable returnVar = null;
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
			case "namecultures":
				returnVar = new CollectionVariable(NameCultures.SelectNotNull(x => x?.Name).Distinct().ToList(), ProgVariableTypes.Text);
				break;
			case "tempfloor":
				return new NumberVariable(TolerableTemperatureFloorEffect);
			case "tempceiling":
				return new NumberVariable(TolerableTemperatureCeilingEffect);
		}

		return returnVar;
	}

	public ProgVariableTypes Type => ProgVariableTypes.Ethnicity;

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

		FMDB.Context.EthnicitiesNameCultures.RemoveRange(dbitem.EthnicitiesNameCultures);
		foreach (var nc in _genderNameCultures)
		{
			if (nc.Value is null)
			{
				continue;
			}

			dbitem.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures
			{
				Ethnicity = dbitem,
				NameCultureId = nc.Value.Id,
				Gender = (short)nc.Key
			});
		}

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

	#3name <name>#0 - renames this ethnicity
	#3group <group>#0 - sets an ethnic group
	#3group clear#0 - clears an ethnic group
	#3subgroup <group>#0 - sets an ethnic subgroup
	#3subgroup clear#0 - clears an ethnic subgroup
	#3desc#0 - drops you into an editor for the ethnicity description
	#3nameculture <which> [all|male|female|neuter|nb|indeterminate]#0 - changes the name culture used by this ethnicity
	#3nameculture none [all|male|female|neuter|nb|indeterminate]#0 - resets the name to not override its culture
	#3availability <prog>#0 - sets a prog that controls appearance in character creation
	#3bloodmodel <model>#0 - sets the population blood model for this ethnicity
	#3tempfloor <amount>#0 - sets the tolerable temperature floor modifier
	#3tempceiling <amount>#0 - sets the tolerable temperature ceiling modifier
	#3characteristic <which> <profile>#0 - sets the characteristic profile for this ethnicity
	#3advice <which>#0 - toggles a chargen advice applying to this ethnicity
	#3cost <resource> <amount>#0 - sets a cost for character creation
	#3require <resource> <amount>#0 - sets a non-cost requirement for character creation
	#3cost <resource> clear#0 - clears a resource cost for character creation";

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
			case "available":
			case "prog":
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
			case "nameculture":
			case "name_culture":
			case "name culture":
			case "culture":
			case "nc":
				return BuildingCommandNameCulture(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandNameCulture(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which name culture do you want to set for this ethnicity?");
			return false;
		}

		var culture = Gameworld.NameCultures.GetByIdOrName(command.PopSpeech());
		if (culture == null && !command.Last.EqualTo("none"))
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return false;
		}

		string genderDescription;
		switch (command.SafeRemainingArgument.CollapseString().ToLowerInvariant())
		{
			case "all":
			case "":
				_genderNameCultures[Gender.Male] = culture;
				_genderNameCultures[Gender.Female] = culture;
				_genderNameCultures[Gender.Neuter] = culture;
				_genderNameCultures[Gender.NonBinary] = culture;
				_genderNameCultures[Gender.Indeterminate] = culture;
				genderDescription = "all genders";
				break;
			case "male":
			case "men":
			case "man":
			case "masculine":
				_genderNameCultures[Gender.Male] = culture;
				genderDescription = "males";
				break;
			case "female":
			case "feminine":
			case "woman":
			case "women":
				_genderNameCultures[Gender.Female] = culture;
				genderDescription = "females";
				break;
			case "neuter":
			case "neutral":
				_genderNameCultures[Gender.Neuter] = culture;
				Changed = true;
				genderDescription = "neuter genders";
				break;
			case "nonbinary":
			case "nb":
				_genderNameCultures[Gender.NonBinary] = culture;
				genderDescription = "non-binary";
				break;
			case "indeterminate":
				_genderNameCultures[Gender.Indeterminate] = culture;
				genderDescription = "indeterminate genders";
				break;
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

		if (culture is null)
		{
			actor.OutputHandler.Send($"This ethnicity will no longer provide an overriding name culture for {genderDescription}.");
		}
		else
		{
			actor.OutputHandler.Send($"This ethnicity will now use the {culture.Name.ColourName()} name culture for {genderDescription}.");
		}
		Changed = true;
		return true;
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

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, [
			[ProgVariableTypes.Chargen],
			[ProgVariableTypes.Chargen, ProgVariableTypes.Ethnicity],
		]).LookupProg();
		if (prog is null)
		{
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
		sb.AppendLine($"Name Culture Male: {NameCultureForGender(Gender.Male)?.Name.ColourValue() ?? "as per culture".ColourName()}");
		sb.AppendLine($"Name Culture Female: {NameCultureForGender(Gender.Female)?.Name.ColourValue() ?? "as per culture".ColourName()}");
		sb.AppendLine($"Name Culture Non-Binary: {NameCultureForGender(Gender.NonBinary)?.Name.ColourValue() ?? "as per culture".ColourName()}");
		sb.AppendLine($"Name Culture Neuter: {NameCultureForGender(Gender.Neuter)?.Name.ColourValue() ?? "as per culture".ColourName()}");
		sb.AppendLine($"Name Culture Indeterminate: {NameCultureForGender(Gender.Indeterminate)?.Name.ColourValue() ?? "as per culture".ColourName()}");
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