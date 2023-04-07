using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using ExpressionEngine;

namespace MudSharp.Health.Strategies;

public abstract class BaseHealthStrategy : FrameworkItem, IHealthStrategy
{
	private static readonly Dictionary<string, Func<HealthStrategy, IFuturemud, IHealthStrategy>>
		_healthStrategyLoaders = new();

	protected Expression LodgeDamageExpression;

	protected RankedRange<WoundSeverity> SeverityRanges = new();
	protected RankedRange<WoundSeverity> PercentageSeverityRanges = new();

	protected BaseHealthStrategy(HealthStrategy strategy)
	{
		_id = strategy.Id;
		_name = strategy.Name;
		LoadDefinition(XElement.Parse(strategy.Definition));
	}

	public override string FrameworkItemType => "HealthStrategy";

	protected static void RegisterHealthStrategy(string type,
		Func<HealthStrategy, IFuturemud, IHealthStrategy> loadFunction)
	{
		_healthStrategyLoaders.Add(type, loadFunction);
	}

	public static void SetupHealthStrategies()
	{
		foreach (
			var type in
			Futuremud.GetAllTypes().Where(x => x.IsSubclassOf(typeof(BaseHealthStrategy))))
		{
			var method = type.GetMethod("RegisterHealthStrategyLoader", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, null);
		}
	}

	public static IHealthStrategy LoadStrategy(HealthStrategy strategy, IFuturemud gameworld)
	{
		if (!_healthStrategyLoaders.ContainsKey(strategy.Type))
		{
			throw new ApplicationException(
				$"BaseHealthStrategy.LoadStrategy was asked to load HealthStrategy ID {strategy.Id} type {strategy.Type}, which is not a valid type.");
		}

		return _healthStrategyLoaders[strategy.Type](strategy, gameworld);
	}

	private void LoadDefinition(XElement root)
	{
		var element = root.Element("SeverityRanges");
		if (element != null)
		{
			foreach (var item in element.Elements("Severity"))
			{
				var lower = double.Parse(item.Attribute("lower").Value);
				var upper = double.Parse(item.Attribute("upper").Value);
				var severity = (WoundSeverity)int.Parse(item.Attribute("value").Value);
				SeverityRanges.Add(severity, lower, upper);
				if (!string.IsNullOrWhiteSpace(item.Attribute("lowerperc")?.Value) &&
				    !string.IsNullOrWhiteSpace(item.Attribute("upperperc")?.Value))
				{
					PercentageSeverityRanges.Add(severity, double.Parse(item.Attribute("lowerperc").Value),
						double.Parse(item.Attribute("upperperc").Value));
				}
				else
				{
					PercentageSeverityRanges.Add(severity, lower / 100.0, upper / 100.0);
				}
			}
		}

		element = root.Element("LodgeDamageExpression");
		LodgeDamageExpression = element != null ? new Expression(element.Value) : new Expression("0");
	}

	#region IHealthStrategy Members

	public abstract string HealthStrategyType { get; }

	public virtual bool RequiresSpinalCord => true;

	public virtual BodyTemperatureStatus CurrentTemperatureStatus(IHaveWounds owner)
	{
		return BodyTemperatureStatus.NormalTemperature;
	}

	public abstract IWound SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart);

	public abstract void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture);

	public virtual void PerformBloodGain(IHaveWounds owner)
	{
		// Do nothing
	}

	public virtual WoundSeverity GetSeverity(double damage)
	{
		return SeverityRanges.Find(damage);
	}

	public virtual WoundSeverity GetSeverityFor(IWound wound, IHaveWounds owner)
	{
		var life = owner is ICharacter ch
			? ch.Body.HitpointsForBodypart(wound.Bodypart)
			: wound.Bodypart?.MaxLife ?? 1.0;
		if (life == 0.0)
		{
			life = 1.0;
		}

		if (wound.UseDamagePercentageSeverities)
		{
			return PercentageSeverityRanges.Find(wound.CurrentDamage / life);
		}

		return SeverityRanges.Find(wound.CurrentDamage);
	}

	public virtual double GetSeverityFloor(WoundSeverity severity, bool usePercentageModel)
	{
		if (usePercentageModel)
		{
			return Math.Max(0, PercentageSeverityRanges.Ranges.First(x => x.Value == severity).LowerBound);
		}

		return Math.Max(0, SeverityRanges.Ranges.First(x => x.Value == severity).LowerBound);
	}

	public virtual double GetSeverityCeiling(WoundSeverity severity, bool usePercentageModel)
	{
		if (usePercentageModel)
		{
			return Math.Max(0, PercentageSeverityRanges.Ranges.First(x => x.Value == severity).UpperBound);
		}

		return Math.Max(1, SeverityRanges.Ranges.First(x => x.Value == severity).UpperBound);
	}

	public abstract HealthTickResult PerformHealthTick(IHaveWounds thing);

	public abstract HealthTickResult EvaluateStatus(IHaveWounds thing);

	public abstract string ReportConditionPrompt(IHaveWounds owner, PromptType type);

	public virtual double GetHealingTickAmount(IWound wound, Outcome outcome, HealthDamageType type)
	{
		return 0;
	}

	public abstract HealthStrategyOwnerType OwnerType { get; }

	public virtual double WoundPenaltyFor(IHaveWounds owner)
	{
		return 0;
	}

	public virtual bool KidneyFunctionActive => false;

	public virtual double CurrentHealthPercentage(IHaveWounds owner)
	{
		return 1.0 - Math.Min(1.0, owner.Wounds.Sum(x => x.CurrentDamage) / MaxHP(owner).IfZero(1.0));
	}

	public abstract double MaxHP(IHaveWounds owner);

	public virtual double MaxPain(IHaveWounds owner)
	{
		return 0;
	}

	public virtual double MaxStun(IHaveWounds owner)
	{
		return 0;
	}

	public virtual bool IsCriticallyInjured(IHaveWounds owner)
	{
		return false;
	}

	public virtual void PerformKidneyFunction(IBody owner)
	{
		// Do nothing 
	}

	public virtual void PerformLiverFunction(IBody owner)
	{
		// Do nothing 
	}

	public virtual void PerformSpleenFunction(IBody owner)
	{
		// Do nothing 
	}

	#endregion
}