using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using ExpressionEngine;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework.Save;
using MudSharp.Database;
using Org.BouncyCastle.Asn1.Bsi;
using MudSharp.Body.Traits.Subtypes;

namespace MudSharp.Magic.Capabilities;

public class SkillLevelBasedMagicCapability : SaveableItem, IMagicCapability
{
	public IMagicCapability Clone(string newName)
	{
		return new SkillLevelBasedMagicCapability(this, newName);
	}

	protected SkillLevelBasedMagicCapability(SkillLevelBasedMagicCapability rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		School = rhs.School;
		PowerLevel = rhs.PowerLevel;
		ConcentrationTrait = rhs.ConcentrationTrait;
		_name = name;
		ShowMagicResourcesInPrompt = rhs.ShowMagicResourcesInPrompt;
		foreach (var power in rhs._skillPowerMap)
		{
			_skillPowerMap.Add((power.Trait, power.MinValue, power.Power));
		}
		ConcentrationCapabilityExpression = new TraitExpression(rhs.ConcentrationCapabilityExpression.OriginalFormulaText, Gameworld);
		ConcentrationDifficultyExpression = new TraitExpression(rhs.ConcentrationDifficultyExpression.OriginalFormulaText, Gameworld);
		_resourceRegenerators.AddRange(rhs._resourceRegenerators);
		using (new FMDB())
		{
			var dbitem = new Models.MagicCapability
			{
				Name = Name,
				MagicSchoolId = School.Id,
				PowerLevel = PowerLevel,
				Definition = SaveToXml(),
				CapabilityModel = "skilllevelbased"
			};
		}
	}

	protected SkillLevelBasedMagicCapability(Models.MagicCapability capability, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = capability.Id;
		_name = capability.Name;
		PowerLevel = capability.PowerLevel;
		School = gameworld.MagicSchools.Get(capability.MagicSchoolId);
		var root = XElement.Parse(capability.Definition);
		foreach (var item in root.Elements("Power"))
		{
			_skillPowerMap.Add((gameworld.Traits.Get(long.Parse(item.Attribute("trait").Value)),
				double.Parse(item.Attribute("minvalue").Value),
				gameworld.MagicPowers.Get(long.Parse(item.Attribute("power").Value))));
		}

		ConcentrationTrait = gameworld.Traits.Get(long.Parse(root.Element("ConcentrationTrait").Value));
		ConcentrationCapabilityExpression =
			new TraitExpression(root.Element("ConcentrationCapabilityExpression").Value, gameworld);
		ConcentrationDifficultyExpression = new TraitExpression(root.Element("ConcentrationDifficultyExpression").Value, Gameworld);
		_resourceRegenerators.AddRange(root.Element("Regenerators")?.Elements().SelectNotNull(x =>
			               long.TryParse(x.Value, out var value)
				               ? gameworld.MagicResourceRegenerators.Get(value)
				               : gameworld.MagicResourceRegenerators.GetByName(x.Value)).ToList() ??
		               Enumerable.Empty<IMagicResourceRegenerator>());
	}

	public string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ConcentrationTrait", ConcentrationTrait.Id),
			new XElement("ConcentrationCapabilityExpression", ConcentrationCapabilityExpression.OriginalFormulaText),
			new XElement("ConcentrationDifficultyExpression", ConcentrationDifficultyExpression.OriginalFormulaText),
			new XElement("Regenerators",
				from regenerator in Regenerators
				select new XElement("Regenerator", regenerator.Id)
			),
			from power in _skillPowerMap
			select new XElement("Power", 
				new XAttribute("trait", power.Trait.Id),
				new XAttribute("minvalue", power.MinValue),
				new XAttribute("power", power.Power.Id)
			)
		).ToString();
	}

	public static void RegisterLoader()
	{
		MagicCapabilityFactory.RegisterLoader("skilllevel",
			(capability, gameworld) => new SkillLevelBasedMagicCapability(capability, gameworld));
	}

	#region Overrides of Item

	public override string FrameworkItemType => "MagicCapability";

	#endregion

	#region Implementation of IMagicCapability

	public IMagicSchool School { get; set; }
	public int PowerLevel { get; set; }
	public TraitExpression ConcentrationCapabilityExpression { get; set; }
	public TraitExpression ConcentrationDifficultyExpression { get; set; }
	public ITraitDefinition ConcentrationTrait { get; set; }
	public bool ShowMagicResourcesInPrompt { get; set; } = true; // TODO

	private readonly List<(ITraitDefinition Trait, double MinValue, IMagicPower Power)> _skillPowerMap = new();

	public IEnumerable<IMagicPower> InherentPowers(ICharacter actor)
	{
		var powers = new List<IMagicPower>();
		foreach (var (trait, minvalue, power) in _skillPowerMap)
		{
			if ((actor.GetTrait(trait)?.Value ?? 0.0) >= minvalue)
			{
				if (!powers.Contains(power))
				{
					powers.Add(power);
				}
			}
		}

		return powers;
	}

	public IEnumerable<IMagicPower> AllPowers => _skillPowerMap.Select(x => x.Power);

	public double ConcentrationAbility(ICharacter actor)
	{
		return ConcentrationCapabilityExpression.Evaluate(actor, ConcentrationTrait);
	}

	public Difficulty GetConcentrationDifficulty(ICharacter actor, double concentrationPercentageOfCapability,
		double individualPowerConcentrationPercentage)
	{
		ConcentrationDifficultyExpression.Formula.Parameters["total"] = concentrationPercentageOfCapability;
		ConcentrationDifficultyExpression.Formula.Parameters["power"] = individualPowerConcentrationPercentage;
		return (Difficulty)(int)Math.Round(Math.Max(0,
			Math.Min((int)Difficulty.Impossible, (double)ConcentrationDifficultyExpression.Evaluate(actor, ConcentrationTrait))));
	}

	private readonly List<IMagicResourceRegenerator> _resourceRegenerators = new ();
	public IEnumerable<IMagicResourceRegenerator> Regenerators => _resourceRegenerators;

	#endregion


	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this capability
	#3level <##>#0 - sets the power level of this capability
	#3trait <which>#0 - sets the trait that is used with concentration checks
	#3regenerator <which>#0 - toggles a regenerator being given by this capability
	#3power <which> <trait> <minvalue>#0 - gives a power when the character has the trait above the value
	#3power <which>#0 - removes an existing power given by this capability
	#3showpower#0 - toggles showing magic resources in the prompt

The following options have some more complex inputs:

	#3conpoints <formula>#0 - sets the formula for concentration points.

	This is a trait expression. See #3TE HELP#0 for a comprehensive list of options. 
	Uses the concentration trait from above as the variable with name #6variable#0.

	#3concentration <formula>#0 - sets the formula for concentration points

	This is a trait expression. See #3TE HELP#0 for a comprehensive list of options.
	Uses the concentration trait from above as the variable with name #6variable#0.
	Uses the variable #6total#0 for a range 0.0 - 1.0 showing what percentage of total existing powers are being sustained
	Uses the variable #6power#0 for a range 0.0 - 1.0 showing what percentage of total power is used by just this power";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "trait":
				return BuildingCommandTrait(actor, command);
			case "regenerator":
			case "generator":
			case "regen":
				return BuildingCommandRegenerator(actor, command);
			case "showpower":
			case "show":
				return BuildingCommandShowPower(actor);
			case "concentration":
			case "con":
			case "concentrationdifficulty":
			case "condifficulty":
				return BuildingCommandConcentrationDifficulty(actor, command);
			case "concentrationpoints":
			case "points":
			case "conpoints":
				return BuildingCommandConcentrationPoints(actor, command);
			case "power":
				return BuildingCommandPower(actor, command);
			case "level":
			case "powerlevel":
				return BuildingCommandPowerLevel(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandPowerLevel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid power level.");
			return false;
		}

		PowerLevel = value;
		Changed = true;
		actor.OutputHandler.Send($"This capability now has a power level of {PowerLevel.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPower(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which power do you want to add or remove?");
			return false;
		}

		var power = Gameworld.MagicPowers.GetByIdOrName(command.PopSpeech());
		if (power is null)
		{
			actor.OutputHandler.Send("There is no such power.");
			return false;
		}

		if (command.IsFinished)
		{
			if (_skillPowerMap.Any(x => x.Power == power))
			{
				_skillPowerMap.RemoveAll(x => x.Power == power);
				Changed = true;
				actor.OutputHandler.Send($"This capability will no longer give the {power.Name.Colour(power.School.PowerListColour)} power.");
				return true;
			}

			actor.OutputHandler.Send("What trait should be used to determine whether a character has this power?");
			return false;
		}

		if (School != power.School)
		{
			actor.OutputHandler.Send($"You can't add powers of the {power.School.Name.Colour(power.School.PowerListColour)} school to this capability as it is of the {School.Name.Colour(School.PowerListColour)} school.");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.PopSpeech());
		if (trait is null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What value does that trait need to be over to qualify?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		_skillPowerMap.RemoveAll(x => x.Power == power);
		_skillPowerMap.Add((trait, value, power));
		Changed = true;
		actor.OutputHandler.Send($"This capability now gives the {power.Name.Colour(power.School.PowerListColour)} power when {trait.Name.ColourValue()} is at or above {value.ToString("N2", actor).ColourValue()} ({trait.Decorator.Decorate(value).ColourValue()}).");
		return true;
	}

	private bool BuildingCommandConcentrationDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What formula do you want to use to arrive at final concentration difficulty?");
			return false;
		}

		var formula = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (formula.HasErrors())
		{
			actor.OutputHandler.Send(formula.Error);
			return false;
		}

		ConcentrationDifficultyExpression = formula;
		Changed = true;
		actor.OutputHandler.Send($"This capability now uses the formula {command.SafeRemainingArgument.ColourCommand()} to determine concentration difficulty.");
		return true;
	}

	private bool BuildingCommandConcentrationPoints(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What formula do you want to use to arrive at total concentration points?");
			return false;
		}

		var formula = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (formula.HasErrors())
		{
			actor.OutputHandler.Send(formula.Error);
			return false;
		}

		ConcentrationCapabilityExpression = formula;
		Changed = true;
		actor.OutputHandler.Send($"This capability now uses the formula {command.SafeRemainingArgument.ColourCommand()} to determine total concentration points.");
		return true;
	}

	private bool BuildingCommandShowPower(ICharacter actor)
	{
		ShowMagicResourcesInPrompt = !ShowMagicResourcesInPrompt;
		Changed = true;
		actor.OutputHandler.Send($"This capability will {ShowMagicResourcesInPrompt.NowNoLonger()} cause magic resources to be shown in the prompt.");
		return true;
	}

	private bool BuildingCommandRegenerator(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which regenerator do you want to toggle for this capability?");
			return false;
		}

		var regenerator = Gameworld.MagicResourceRegenerators.GetByIdOrName(command.SafeRemainingArgument);
		if (regenerator is null)
		{
			actor.OutputHandler.Send("There is no such regenerator.");
			return false;
		}

		if (_resourceRegenerators.Remove(regenerator))
		{
			Changed = true;
			actor.OutputHandler.Send($"This capability will no longer give the {regenerator.Name.ColourName()} regenerator.");
			return true;
		}

		_resourceRegenerators.Add(regenerator);
		Changed = true;
		actor.OutputHandler.Send($"This capability will now give the {regenerator.Name.ColourName()} regenerator.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which trait should this capability use to determine spell concentration?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		ConcentrationTrait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This capability will now use the {trait.Name.ColourValue()} {(trait is ISkillDefinition ? "skill" : "attribute")} to determine concentration.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this capability?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MagicCapabilities.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a capability with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this capability from {Name.Colour(School.PowerListColour)} to {name.Colour(School.PowerListColour)}.");
		_name = name;
		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Magic Capability #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Magenta, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {"Skill Level Based".ColourValue()}");
		sb.AppendLine($"School: {School.Name.Colour(School.PowerListColour)}");
		sb.AppendLine($"Power Level: {PowerLevel.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Concentration Trait: {ConcentrationTrait.Name.ColourValue()}");
		sb.AppendLine($"Show Resources in Prompt: {ShowMagicResourcesInPrompt.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Regenerators:");
		sb.AppendLine();
		foreach (var regenerator in _resourceRegenerators)
		{
			sb.AppendLine($"\t#{regenerator.Id.ToString("N0", actor)} ({regenerator.Name.ColourName()}): {regenerator.GeneratedResources.Select(x => x.Name.ColourValue()).ListToString()}");
		}
		sb.AppendLine();
		sb.AppendLine("Powers:");
		sb.AppendLine();
		foreach (var power in _skillPowerMap.OrderBy(x => x.Trait.Name).ThenBy(x => x.MinValue).ThenBy(x => x.Power.Name))
		{
			sb.AppendLine($"\t{power.Trait.Name.ColourName()} >= {power.MinValue.ToString("N2", actor).ColourValue()}: {power.Power.Name.Colour(power.Power.School.PowerListColour)}");
		}
		return sb.ToString();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.MagicCapabilities.Find(Id);
		dbitem.Name = Name;
		dbitem.PowerLevel = PowerLevel;
		dbitem.MagicSchoolId = School.Id;
		dbitem.Definition = SaveToXml();
		Changed = false;
	}

	#region Implementation of IFutureProgVariable

	public FutureProgVariableTypes Type => FutureProgVariableTypes.MagicCapability;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "name":
				return new TextVariable(Name);
			case "id":
				return new NumberVariable(Id);
			case "powerlevel":
				return new NumberVariable(PowerLevel);
			case "school":
				return School;
			case "concentrationtrait":
				return ConcentrationTrait;
		}

		throw new ApplicationException("Invalid property requested in MagicCapability.GetProperty");
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", FutureProgVariableTypes.Text },
			{ "id", FutureProgVariableTypes.Number },
			{ "school", FutureProgVariableTypes.MagicSchool },
			{ "powerlevel", FutureProgVariableTypes.Number },
			{ "concentrationtrait", FutureProgVariableTypes.Trait }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", "The name of the magic capability" },
			{ "id", "The Id of the magic capability" },
			{ "school", "The school that this capability belongs to" },
			{ "powerlevel", "The power level of this capability" },
			{ "concentrationtrait", "The trait used for concentrating on sustained spells" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.MagicCapability,
			DotReferenceHandler(), DotReferenceHelp());
	}

	#endregion
}