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

namespace MudSharp.Magic.Capabilities;

public class SkillLevelBasedMagicCapability : SaveableItem, IMagicCapability
{
	protected SkillLevelBasedMagicCapability(Models.MagicCapability capability, IFuturemud gameworld)
	{
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
		ConcentrationDifficultyExpression = new Expression(root.Element("ConcentrationDifficultyExpression").Value);
		Regenerators = root.Element("Regenerators")?.Elements().SelectNotNull(x =>
			               long.TryParse(x.Value, out var value)
				               ? gameworld.MagicResourceRegenerators.Get(value)
				               : gameworld.MagicResourceRegenerators.GetByName(x.Value)).ToList() ??
		               Enumerable.Empty<IMagicResourceRegenerator>();
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
	public Expression ConcentrationDifficultyExpression { get; set; }
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

	public Difficulty GetConcentrationDifficulty(double concentrationPercentageOfCapability,
		double individualPowerConcentrationPercentage)
	{
		ConcentrationDifficultyExpression.Parameters["total"] = concentrationPercentageOfCapability;
		ConcentrationDifficultyExpression.Parameters["power"] = individualPowerConcentrationPercentage;
		return (Difficulty)(int)Math.Round(Math.Max(0,
			Math.Min((int)Difficulty.Impossible, (double)ConcentrationDifficultyExpression.Evaluate())));
	}

	public IEnumerable<IMagicResourceRegenerator> Regenerators { get; }

	#endregion



	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	public string Show(ICharacter actor)
	{
		throw new NotImplementedException();
	}

	public override void Save()
	{
		throw new NotImplementedException();
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