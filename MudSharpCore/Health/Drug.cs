using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Health;



public class Drug : SaveableItem, IDrug
{
	public Drug(MudSharp.Models.Drug drug, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = drug.Id;
		_name = drug.Name;
		DrugVectors = (DrugVector)drug.DrugVectors;
		IntensityPerGram = drug.IntensityPerGram;
		RelativeMetabolisationRate = drug.RelativeMetabolisationRate;
		foreach (var item in drug.DrugsIntensities)
		{
			DrugTypeMulipliers[(DrugType)item.DrugType] = (item.RelativeIntensity, AdditionalInfoFor((DrugType)item.DrugType, item.AdditionalEffects));
		}
	}

	#region Overrides of Item

	public override string FrameworkItemType => "Drug";

	#endregion

	#region Overrides of SaveableItem

	/// <summary>Tells the object to perform whatever save action it needs to do</summary>
	public override void Save()
	{
		var dbitem = FMDB.Context.Drugs.Find(Id);
		dbitem.Name = Name;
		dbitem.DrugVectors = (int)DrugVectors;
		dbitem.IntensityPerGram = IntensityPerGram;
		dbitem.RelativeMetabolisationRate = RelativeMetabolisationRate;
		FMDB.Context.DrugsIntensities.RemoveRange(dbitem.DrugsIntensities);
		foreach (var item in DrugTypeMulipliers)
		{
			var dbmult = new Models.DrugIntensity
			{
				DrugType = (int)item.Key,
				RelativeIntensity = item.Value.Multiplier,
				AdditionalEffects = item.Value.ExtraInfo.DatabaseString
			};
			dbitem.DrugsIntensities.Add(dbmult);
		}

		Changed = false;
	}

	#endregion

	#region Implementation of IDrug

	public IEnumerable<DrugType> DrugTypes => DrugTypeMulipliers.Select(x => x.Key).AsEnumerable();
	public DrugVector DrugVectors { get; set; }
	public double IntensityPerGram { get; set; }
	public double RelativeMetabolisationRate { get; set; }
	public Dictionary<DrugType, (double Multiplier, DrugAdditionalInfo ExtraInfo)> DrugTypeMulipliers { get; } = new();

	public T AdditionalInfoFor<T>(DrugType type) where T : DrugAdditionalInfo
	{
		return (T)DrugTypeMulipliers.ValueOrDefault(type, default).ExtraInfo;
	}

	private DrugAdditionalInfo AdditionalInfoFor(DrugType type, string extra)
	{
		switch (type)
		{
			case DrugType.NeutraliseDrugEffect:
				return new NeutraliseDrugAdditionalInfo
				{
					NeutralisedTypes = extra
					                   .Split(' ')
					                   .Select(int.Parse)
					                   .Cast<DrugType>()
					                   .ToList()
				};
			case DrugType.BodypartDamage:
				return new BodypartDamageAdditionalInfo
				{
					BodypartTypes = extra
					                .Split(' ')
					                .Select(int.Parse)
					                .Cast<BodypartTypeEnum>()
					                .ToList()
				};
			case DrugType.HealingRate:
				return new HealingRateAdditionalInfo
				{
					HealingRateIntensity = double.Parse(extra.Split(" ")[0]),
					HealingDifficultyIntensity = double.Parse(extra.Split(" ")[1])
				};
			case DrugType.MagicAbility:
				return new MagicAbilityAdditionalInfo
				{
					MagicCapabilityIds = extra
					                     .Split(' ')
					                     .Select(long.Parse)
					                     .ToList()
				};
			case DrugType.NeutraliseSpecificDrug:
				return new NeutraliseSpecificDrugAdditionalInfo()
				{
					NeutralisedIds = extra
					                 .Split(' ')
					                 .Select(long.Parse)
					                 .ToList()
				};
			default:
				return null;
		}
	}

	public const string HelpText = @"";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "intensity":
				return BuildingCommandIntensity(actor, command);
			case "metabolism":
				return BuildingCommandMetabolism(actor, command);
			case "inhaled":
				return BuildingCommandInhaled(actor);
			case "injected":
				return BuildingCommandInjected(actor);
			case "ingested":
				return BuildingCommandIngested(actor);
			case "touch":
				return BuildingCommandTouch(actor);
			case "type":
				return BuildingCommandType(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "intensity":
				return BuildingCommandTypeIntensity(actor, command);
			case "healingrate":
				return BuildingCommandHealingRate(actor, command);
			case "magic":
			case "magiccapability":
			case "magicability":
			case "capability":
			case "ability":
				return BuildingCommandMagicCapability(actor, command);
			case "neutralise":
				return BuildingCommandNeutralise(actor, command);
			case "neutralisespecific":
				return BuildingCommandNeutraliseSpecific(actor, command);
			case "damage":
				return BuildingCommandDamage(actor, command);
			default:
				return false;
		}
	}

	private bool BuildingCommandDamage(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandNeutraliseSpecific(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandNeutralise(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandMagicCapability(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandHealingRate(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandTypeIntensity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which drug type effect do you want to change the intensity of? The valid choices are {Enum.GetValues<DrugType>().ListToColouredString()}.");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum<DrugType>(out var type))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid drug type. The valid choices are {Enum.GetValues<DrugType>().ListToColouredString()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the intensity of that drug effect?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		if (value <= 0.0)
		{
			DrugTypeMulipliers.Remove(type);
			Changed = true;
			actor.OutputHandler.Send($"This drug will not have any effects of type {type.DescribeEnum().ColourValue()}.");
			return true;
		}

		if (DrugTypeMulipliers.ContainsKey(type))
		{
			DrugTypeMulipliers[type] = (value, DrugTypeMulipliers[type].ExtraInfo);
		}
		else
		{
			switch (type)
			{
				case DrugType.NeutraliseDrugEffect:

				default:
					DrugTypeMulipliers[type] = (value, null);
					break;
			}
		}

		Changed = true;
		actor.OutputHandler.Send($"");
		return true;
	}

	private bool BuildingCommandTouch(ICharacter actor)
	{
		DrugVectors &= ~DrugVector.Touched;
		Changed = true;
		actor.OutputHandler.Send($"This drug can {DrugVectors.HasFlag(DrugVector.Touched).NowNoLonger()} be absorbed by touch contact.");
		return true;
	}

	private bool BuildingCommandIngested(ICharacter actor)
	{
		DrugVectors &= ~DrugVector.Ingested;
		Changed = true;
		actor.OutputHandler.Send($"This drug can {DrugVectors.HasFlag(DrugVector.Ingested).NowNoLonger()} be ingested.");
		return true;
	}

	private bool BuildingCommandInjected(ICharacter actor)
	{
		DrugVectors &= ~DrugVector.Injected;
		Changed = true;
		actor.OutputHandler.Send($"This drug can {DrugVectors.HasFlag(DrugVector.Injected).NowNoLonger()} be injected.");
		return true;
	}

	private bool BuildingCommandInhaled(ICharacter actor)
	{
		DrugVectors &= ~DrugVector.Inhaled;
		Changed = true;
		actor.OutputHandler.Send($"This drug can {DrugVectors.HasFlag(DrugVector.Inhaled).NowNoLonger()} be inhaled.");
		return true;
	}

	private bool BuildingCommandMetabolism(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the relative metabolism of this drug per gram?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		RelativeMetabolisationRate = value;
		Changed = true;
		actor.OutputHandler.Send($"This drug now has a relative metabolism of {RelativeMetabolisationRate.ToStringP2Colour(actor)}.");
		return true;
	}

	private bool BuildingCommandIntensity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the relative intensity of this drug per gram?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		IntensityPerGram = value;
		Changed = true;
		actor.OutputHandler.Send($"This drug now has a relative intensity per gram of {IntensityPerGram.ToStringP2Colour(actor)}.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this drug?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (Gameworld.Drugs.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a drug called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the drug {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public virtual string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Drug #{Id:N0} - {Name}");
		sb.AppendLine($"Intensity per Gram: {IntensityPerGram}");
		sb.AppendLine($"Relative Metabolisation Rate: {RelativeMetabolisationRate}");
		sb.AppendLine($"Vectors: {DrugVectors.Describe()}");
		sb.AppendLine(
			$"Effects: \n{DrugTypes.Select(y => DescribeEffect(y, voyeur)).ListToLines(true)}");

		return sb.ToString();
	}

	public string DescribeEffect(DrugType type, IPerceiver voyeur)
	{
		switch (type)
		{
			case DrugType.NeutraliseDrugEffect:
				var types = AdditionalInfoFor<NeutraliseDrugAdditionalInfo>(DrugType.NeutraliseDrugEffect)
				            .NeutralisedTypes
				            .Select(x => x.DescribeEnum().ColourValue())
				            .ToList();
				return $"Neutralising {types.ListToString()} @ {IntensityForType(type).ToString("N4", voyeur)}";
			case DrugType.BodypartDamage:
				return
					$"Damaging {AdditionalInfoFor<BodypartDamageAdditionalInfo>(DrugType.BodypartDamage).BodypartTypes.Select(x => x.DescribeEnum().Pluralise().ColourValue()).ListToString()} @ {IntensityForType(type).ToString("N4", voyeur)}";
			case DrugType.HealingRate:
				var split = AdditionalInfoFor<HealingRateAdditionalInfo>(DrugType.HealingRate);
				return
					$"HealingRate Mult ({split.HealingRateIntensity.ToString("N4", voyeur)}) Diff ({split.HealingDifficultyIntensity.ToString("N4", voyeur)}) @ {IntensityForType(type).ToString("N4", voyeur)}";
			case DrugType.MagicAbility:
				var capabilities = AdditionalInfoFor<MagicAbilityAdditionalInfo>(DrugType.MagicAbility)
				                   .MagicCapabilityIds
				                   .SelectNotNull(x => Gameworld.MagicCapabilities.Get(x))
				                   .ToList();
				return
					$"MagicAbility of {capabilities.Select(x => x.Name.Colour(x.School.PowerListColour)).ListToString()} @ {IntensityForType(type).ToString("N4", voyeur)}";
		}

		return $"{type.DescribeEnum()} @ {IntensityForType(type).ToString("N4", voyeur)}";
	}

	public double IntensityForType(DrugType type)
	{
		return DrugTypeMulipliers.ValueOrDefault(type, default).Multiplier * IntensityPerGram;
	}

	#endregion

	#region IFutureProgVariable Implementation

	public ProgVariableTypes Type => ProgVariableTypes.Drug;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "intensitypergram":
				return new NumberVariable(IntensityPerGram);
			case "metabolisationrate":
			case "metabolizationrate":
				return new NumberVariable(RelativeMetabolisationRate);
			case "vectors":
				return new CollectionVariable(
					DrugVectors.GetFlags().OfType<DrugVector>().Select(x => new TextVariable(x.Describe())).ToList(),
					ProgVariableTypes.Text);
			case "types":
				return new CollectionVariable(DrugTypes.Select(x => new TextVariable(x.DescribeEnum())).ToList(),
					ProgVariableTypes.Text);
			case "itensities":
				return new CollectionVariable(DrugTypes.Select(x => new NumberVariable(IntensityForType(x))).ToList(),
					ProgVariableTypes.Number);
		}

		throw new NotImplementedException();
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "intensitypergram", ProgVariableTypes.Number },
			{ "metabolisationrate", ProgVariableTypes.Text | ProgVariableTypes.Collection },
			{ "metabolizationrate", ProgVariableTypes.Number },
			{ "vectors", ProgVariableTypes.Text | ProgVariableTypes.Collection },
			{ "types", ProgVariableTypes.Text | ProgVariableTypes.Collection },
			{ "intensities", ProgVariableTypes.Number | ProgVariableTypes.Collection }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The unique database id of the drug" },
			{ "name", "The name of the drug" },
			{
				"intensitypergram",
				"The hard-coded intensity of the drug per gram ingested. Has different effects per drug type"
			},
			{ "metabolisationrate", "The relative rate at which the drug is metabolised (removed) by the body" },
			{ "metabolizationrate", "An alias for the metabolisationrate property" },
			{ "vectors", "The potential vectors for the drug affecting someone" },
			{ "types", "The drug effect types contained in this drug" },
			{ "intensities", "The intensities for each drug type, ordered the same way as the 'types' property" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Drug, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}