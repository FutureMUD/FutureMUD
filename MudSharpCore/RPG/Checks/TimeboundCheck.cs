using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.RPG.Checks;

/// <summary>
///     A timebound check is a StandardCheck that once evaluated, returns the same outcome for a fixed time period
/// </summary>
public class TimeboundCheck : StandardCheck
{
	public TimeboundCheck(Check check, IFuturemud game)
		: base(check, game)
	{
		LoadFromXml(XElement.Parse(check.CheckTemplate.Definition));
	}

	protected int MinSeconds { get; set; }
	protected int MaxSeconds { get; set; }

	protected TimeSpan GetTimeSpan => TimeSpan.FromSeconds(RandomUtilities.Random(MinSeconds, MaxSeconds + 1));

	protected Outcome ExistingOutcome(IPerceivableHaveTraits checkee, Difficulty difficulty,
		IPerceivable target = null, ITraitDefinition trait = null, IUseTrait tool = null, double externalBonus = 0)
	{
		var checks = checkee.EffectsOfType<ICheckResultEffect>().ToList();
		return checks.FirstOrDefault(x => x.SameCheck(Type, difficulty, target, trait, tool))?.Outcome ??
		       Outcome.None;
	}

	public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
		ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var existing = ExistingOutcome(checkee, difficulty, target, trait, null, externalBonus);
		if (existing != Outcome.None)
		{
			return new CheckOutcome
			{
				AcquiredTraits = Enumerable.Empty<ITraitDefinition>(),
				Outcome = existing
			};
		}

		var result = base.Check(checkee, difficulty, trait, target, externalBonus, traitUseType, customParameters);
		checkee.AddEffect(new CheckResult(checkee, Type, difficulty, result, trait, target), GetTimeSpan);
		return result;
	}

	#region Overrides of StandardCheck

	public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
		IPerceivable target = null,
		IUseTrait tool = null, double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var existing = ExistingOutcome(checkee, difficulty, target, tool?.Trait, tool, externalBonus);
		if (existing != Outcome.None)
		{
			return new CheckOutcome
			{
				AcquiredTraits = Enumerable.Empty<ITraitDefinition>(),
				Outcome = existing
			};
		}

		var result = base.Check(checkee, difficulty, tool?.Trait, target, externalBonus, traitUseType,
			customParameters);
		checkee.AddEffect(new CheckResult(checkee, Type, difficulty, result, tool?.Trait, target), GetTimeSpan);
		return result;
	}

	#endregion

	protected void LoadFromXml(XElement root)
	{
		MinSeconds = root.Attribute("minimum_time").Value.GetIntFromOrdinal() ?? 1;
		MaxSeconds = root.Attribute("maximum_time").Value.GetIntFromOrdinal() ?? 1;
	}
}