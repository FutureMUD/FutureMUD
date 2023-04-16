using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health.Wounds;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Health.Strategies;

public class RobotHealthStrategy : BaseHealthStrategy
{
	public static void RegisterHealthStrategyLoader()
	{
		RegisterHealthStrategy("Robot", (strategy, game) => new RobotHealthStrategy(strategy, game));
	}

	protected RobotHealthStrategy(Models.HealthStrategy strategy, IFuturemud gameworld) : base(strategy)
	{
		LoadDefinition(XElement.Parse(strategy.Definition), gameworld);
	}

	private void LoadDefinition(XElement root, IFuturemud gameworld)
	{
		var element = root.Element("MaximumHitPointsExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"RobotHealthStrategy ID {Id} did not contain a MaximumHitPointsExpression element.");
		}

		if (!long.TryParse(element.Value, out var value))
		{
			throw new ApplicationException(
				$"RobotHealthStrategy ID {Id} had a MaximumHitPointsExpression element that did not contain an ID.");
		}

		MaximumHitPointsExpression = gameworld.TraitExpressions.Get(value);
		PercentageHealthPerPenalty = double.Parse(root.Element("PercentageHealthPerPenalty")?.Value ?? "1.0");
		PercentageStunPerPenalty = double.Parse(root.Element("PercentageStunPerPenalty")?.Value ?? "1.0");

		element = root.Element("MaximumStunExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"RobotHealthStrategy ID {Id} did not contain a MaximumStunExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"RobotHealthStrategy ID {Id} had a MaximumStunExpression element that did not contain an ID.");
		}

		MaximumStunExpression = gameworld.TraitExpressions.Get(value);
	}

	public override string HealthStrategyType => "Robot";

	public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;

	public ITraitExpression MaximumHitPointsExpression { get; set; }
	public ITraitExpression MaximumStunExpression { get; set; }
	public double PercentageHealthPerPenalty { get; set; }
	public double PercentageStunPerPenalty { get; set; }
	public override bool RequiresSpinalCord => false;

	public override double MaxHP(IHaveWounds owner)
	{
		return owner is not ICharacter charOwner ? 0.0 : MaximumHitPointsExpression.Evaluate(charOwner);
	}

	public override double MaxStun(IHaveWounds owner)
	{
		return owner is not ICharacter charOwner ? 0.0 : MaximumStunExpression.Evaluate(charOwner);
	}

	public override double WoundPenaltyFor(IHaveWounds owner)
	{
		if (owner is not ICharacter charOwner)
		{
			return 0;
		}

		var penalty =
			charOwner.Wounds.Sum(
				x =>
					x.CurrentDamage / PercentageHealthPerPenalty + x.CurrentStun / PercentageStunPerPenalty) /
			(MaximumHitPointsExpression.Evaluate(charOwner) + MaximumStunExpression.Evaluate(charOwner));
		return -1 * penalty;
	}

	public override IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart)
	{
		if (bodypart == null)
		{
			return Enumerable.Empty<IWound>();
		}

#if DEBUG
		if (double.IsInfinity(damage.DamageAmount) || double.IsInfinity(damage.PainAmount) ||
		    double.IsInfinity(damage.StunAmount) ||
		    double.IsNaN(damage.DamageAmount) || double.IsNaN(damage.PainAmount) || double.IsNaN(damage.StunAmount))
		{
			throw new ApplicationException("Invalid damage/pain/stun in SufferDamage.");
		}
#endif

		IGameItem lodgedItem = null;
		LodgeDamageExpression.Parameters["damage"] = damage.DamageAmount;
		LodgeDamageExpression.Parameters["type"] = (int)damage.DamageType;
		if (damage.DamageType.CanLodge() && Dice.Roll(0, 100) < Convert.ToDouble(LodgeDamageExpression.Evaluate()))
		{
			lodgedItem = damage.LodgableItem;
		}

		return new[]
		{
			new RobotWound(owner.Gameworld, owner, damage.DamageAmount, damage.StunAmount, damage.DamageType,
				damage.Bodypart, lodgedItem, damage.ToolOrigin, damage.ActorOrigin)
		};
	}

	public override void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
	{
		var cOwner = (ICharacter)owner;
		foreach (var liquid in mixture.Instances)
		{
			if (liquid.Liquid.InjectionConsequence != LiquidInjectionConsequence.BloodReplacement)
			{
				continue;
			}

			if (!liquid.Liquid.LiquidCountsAs(cOwner.Race.BloodLiquid))
			{
				continue;
			}

			cOwner.Body.CurrentBloodVolumeLitres += liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres;
		}
	}

	private string WoundCountDesc(int count)
	{
		switch (count)
		{
			case 1:
				return "a wound";
			case 2:
				return "a couple of wounds";
			case 3:
			case 4:
				return "a few wounds";
			case 5:
			case 6:
			case 7:
			case 8:
				return "several wounds";
			default:
				return "many wounds";
		}
	}

	public override HealthTickResult PerformHealthTick(IHaveWounds thing)
	{
		if (thing is not ICharacter charOwner)
		{
			return HealthTickResult.None;
		}

		var isBleeding = false;
		if (charOwner.Body.CurrentBloodVolumeLitres > 0 && charOwner.LongtermExertion != ExertionLevel.Stasis)
		{
			var bleedingWounds =
				thing.Wounds.Select(
					     x => x.Bleed(charOwner.Body.CurrentBloodVolumeLitres, charOwner.Body.CurrentExertion,
						     charOwner.Body.TotalBloodVolumeLitres))
				     .Where(x => x.BloodAmount > 0)
				     .ToList();
			var bleeding = bleedingWounds.Sum(x => x.BloodAmount);
			if (bleeding != 0)
			{
				charOwner.Body.CurrentBloodVolumeLitres -= bleeding;
			}

			if (bleeding > 0)
			{
				charOwner.HandleEvent(EventType.BleedTick, charOwner, bleeding);
				isBleeding = true;
			}

			var visibleBleedingWounds = bleedingWounds.Where(x => x.Visible).ToList();
			if (visibleBleedingWounds.Any())
			{
				if (!charOwner.Body.EffectsOfType<ISuppressBleedMessage>().Any())
				{
					var messages = new List<string>();
					var perceivables = new List<IPerceivable> { charOwner };
					if (visibleBleedingWounds.Any(x => x.CoverItem == null))
					{
						var wounds = visibleBleedingWounds.Where(x => x.CoverItem == null).ToList();
						var countDesc = WoundCountDesc(wounds.Count);
						messages.Add(
							$"{countDesc} on &0's {wounds.Select(x => x.Bodypart).Distinct().Select(x => x.FullDescription()).ListToString()}");
					}

					if (visibleBleedingWounds.Any(x => x.CoverItem != null))
					{
						var wounds = visibleBleedingWounds.Where(x => x.CoverItem != null).ToList();
						var countDesc = WoundCountDesc(wounds.Count);
						var index = 1;
						messages.Add(
							$"{countDesc} underneath {wounds.Select(x => x.CoverItem).Distinct().Select(x => $"${index++}").ListToString()}");
						perceivables.AddRange(wounds.Select(x => x.CoverItem).Distinct());
					}

					charOwner.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"$0 leak|leaks {charOwner.Race.BloodLiquid.Name.ToLowerInvariant().Colour(charOwner.Race.BloodLiquid.DisplayColour)} from {messages.ListToString()}",
								charOwner,
								perceivables.ToArray()), flags: OutputFlags.InnerWrap | OutputFlags.SuppressObscured));
					if (!charOwner.Body.EffectsOfType<ISuppressBleedMessage>().Any())
					{
						charOwner.Body.AddEffect(new SuppressBleedMessage(charOwner.Body, null),
							TimeSpan.FromSeconds(15));
					}
				}

				foreach (var witness in charOwner.Location?.Perceivables ?? Enumerable.Empty<IPerceivable>())
				{
					witness.HandleEvent(EventType.WitnessBleedTick, charOwner, bleeding, witness);
				}
			}
		}

		if (charOwner.State.HasFlag(CharacterState.Dead))
		{
			// Only process bleeding for dead people
			if (!isBleeding)
			{
				charOwner.EndHealthTick();
			}

			return HealthTickResult.Dead;
		}

		return EvaluateStatus(thing);
	}

	public override HealthTickResult EvaluateStatus(IHaveWounds thing)
	{
		var charOwner = (ICharacter)thing;
		if (charOwner.State.HasFlag(CharacterState.Dead))
		{
			return HealthTickResult.Dead;
		}

		var brainActivity = charOwner.Body.OrganFunction<PositronicBrain>();
		if (brainActivity <= 0)
		{
			return HealthTickResult.Dead;
		}

		if (charOwner.Body.EffectsOfType<ILossOfConsciousnessEffect>().Any(x => x.Applies()))
		{
			return charOwner.Body.EffectsOfType<ILossOfConsciousnessEffect>().First(x => x.Applies()).UnconType;
		}

		if (
			charOwner.Body.OrganFunction<PowerCore>() <= 0.0)
		{
			return HealthTickResult.Unconscious;
		}

		if (charOwner.Body.CurrentBloodVolumeLitres <= 0.00)
		{
			return HealthTickResult.Paralyzed;
		}

		var maxStun = MaximumStunExpression.Evaluate(charOwner);
		if (thing.Wounds.Sum(x => x.CurrentStun) >= maxStun &&
		    !charOwner.Body.EffectsOfType<IPreventPassOut>().Any(x => x.Applies()))
		{
			return HealthTickResult.Unconscious;
		}

		return HealthTickResult.None;
	}

	public override string ReportConditionPrompt(IHaveWounds owner, PromptType type)
	{
		var charOwner = (ICharacter)owner;
		var stunRatio = owner.Wounds.Sum(x => x.CurrentStun) /
		                MaximumStunExpression.Evaluate(charOwner);
		var painRatio = 0.0;
		var bloodlossRatio = charOwner.Body.CurrentBloodVolumeLitres / charOwner.Body.TotalBloodVolumeLitres;
		var totalBreath = charOwner.Body.HeldBreathPercentage;

		switch (type)
		{
			case PromptType.Classic:
				return ReportClassicPrompt(charOwner, stunRatio, painRatio, bloodlossRatio, totalBreath);
			case PromptType.Full:
				return ReportFullPrompt(charOwner, stunRatio, painRatio, bloodlossRatio, totalBreath, false);
			case PromptType.FullBrief:
				return ReportFullPrompt(charOwner, stunRatio, painRatio, bloodlossRatio, totalBreath, true);
		}

		return ">";
	}

	private string ReportFullPrompt(ICharacter charOwner, double stunRatio, double painRatio, double bloodlossRatio,
		double breathRatio, bool brief)
	{
		var sb = new StringBuilder();
		var power = charOwner.Body.OrganFunction<PowerCore>();
		if (power < 0.3)
		{
			sb.AppendLine("*** YOUR POWER CORE IS CRITICAL! ***".Colour(Telnet.BoldRed));
		}

		if (charOwner.NeedsToBreathe && !charOwner.CanBreathe)
		{
			if (breathRatio > 0.0)
			{
				sb.AppendLine("*** YOU ARE HOLDING YOUR BREATH! ***".Colour(Telnet.BoldBlue));
			}
			else if (charOwner.BreathingFluid is ILiquid)
			{
				sb.AppendLine("*** YOU ARE DROWNING! ***".Colour(Telnet.BoldCyan));
			}
			else
			{
				sb.AppendLine("*** YOU ARE SUFFOCATING! ***".Colour(Telnet.BoldYellow));
			}
		}

		if (stunRatio > 0.05)
		{
			if (stunRatio < 0.2)
			{
				sb.Append($", {"dizzy".Colour(Telnet.BoldCyan)}");
			}
			else if (stunRatio < 0.4)
			{
				sb.Append($", {"dazed".Colour(Telnet.BoldCyan)}");
			}
			else if (stunRatio < 0.6)
			{
				sb.Append($", {"stunned".Colour(Telnet.BoldBlue)}");
			}
			else if (stunRatio < 0.8)
			{
				sb.Append($", {"severly stunned".Colour(Telnet.BoldBlue)}");
			}
			else if (stunRatio < 1.0)
			{
				sb.Append($", {"practically catatonic".Colour(Telnet.BoldMagenta)}");
			}
			else
			{
				sb.Append($", {"knocked out".Colour(Telnet.BoldMagenta)}");
			}
		}

		if (bloodlossRatio <= 0.98 && sb.Length != 0)
		{
			sb.Append(" and have ");
		}

		if (bloodlossRatio >= 0.98)
		{
			//sb.Append($" and have {"no blood loss".Colour(Telnet.Green)}");
		}
		else if (bloodlossRatio >= 0.95)
		{
			sb.Append($"{"very minor hydraulic fluid loss".Colour(Telnet.Yellow)}");
		}
		else if (bloodlossRatio >= 0.80)
		{
			sb.Append($"{"minor hydraulic fluid loss".Colour(Telnet.Yellow)}");
		}
		else if (bloodlossRatio >= 0.65)
		{
			sb.Append($"{"moderate hydraulic fluid loss".Colour(Telnet.Red)}");
		}
		else if (bloodlossRatio >= 0.5)
		{
			sb.Append($"{"major hydraulic fluid loss".Colour(Telnet.Red)}");
		}
		else if (bloodlossRatio >= 0.35)
		{
			sb.Append($"{"severe hydraulic fluid loss".Colour(Telnet.Red)}");
		}
		else if (bloodlossRatio >= 0.2)
		{
			sb.Append($"{"very severe hydraulic fluid loss".Colour(Telnet.Red)}");
		}
		else if (bloodlossRatio >= 0.1)
		{
			sb.Append($"{"critical hydraulic fluid loss".Colour(Telnet.Red)}");
		}
		else
		{
			sb.Append($"{"hyper-critical hydraulic fluid loss".Colour(Telnet.Red)}");
		}

		return sb.ToString();
	}

	private string ReportClassicPrompt(ICharacter charOwner, double stunRatio, double painRatio, double bloodlossRatio,
		double breathRatio)
	{
		var sb = new StringBuilder();
		sb.Append(" / Stun: ");
		if (stunRatio <= 0.0)
		{
			sb.Append($"{Telnet.Blue.Colour}**{Telnet.Cyan.Colour}**{Telnet.BoldCyan.Colour}**{Telnet.RESETALL}");
		}
		else if (stunRatio <= 0.1667)
		{
			sb.Append($"{Telnet.Blue.Colour}**{Telnet.Cyan.Colour}**{Telnet.BoldCyan.Colour}* {Telnet.RESETALL}");
		}
		else if (stunRatio <= 0.3333)
		{
			sb.Append($"{Telnet.Blue.Colour}**{Telnet.Cyan.Colour}**  {Telnet.RESET}");
		}
		else if (stunRatio <= 0.5)
		{
			sb.Append($"{Telnet.Blue.Colour}**{Telnet.Cyan.Colour}*   {Telnet.RESET}");
		}
		else if (stunRatio <= 0.6667)
		{
			sb.Append($"{Telnet.Blue.Colour}**    {Telnet.RESET}");
		}
		else if (stunRatio <= 0.8335)
		{
			sb.Append($"{Telnet.Blue.Colour}*     {Telnet.RESET}");
		}
		else if (stunRatio >= 1.0)
		{
			sb.Append($"      ");
		}

		var stamRatio = 1.0 - charOwner.CurrentStamina / charOwner.MaximumStamina;
		sb.Append(" / Stam: ");
		if (stamRatio <= 0.0)
		{
			sb.Append($"{Telnet.Red.Colour}||{Telnet.Yellow.Colour}||{Telnet.Green.Colour}||{Telnet.RESET}");
		}
		else if (stamRatio <= 0.1667)
		{
			sb.Append($"{Telnet.Red.Colour}||{Telnet.Yellow.Colour}||{Telnet.Green.Colour}| {Telnet.RESET}");
		}
		else if (stamRatio <= 0.3333)
		{
			sb.Append($"{Telnet.Red.Colour}||{Telnet.Yellow.Colour}||  {Telnet.RESET}");
		}
		else if (stamRatio <= 0.5)
		{
			sb.Append($"{Telnet.Red.Colour}||{Telnet.Yellow.Colour}|   {Telnet.RESET}");
		}
		else if (stamRatio <= 0.6667)
		{
			sb.Append($"{Telnet.Red.Colour}||    {Telnet.RESET}");
		}
		else if (stamRatio <= 0.8335)
		{
			sb.Append($"{Telnet.Red.Colour}|     {Telnet.RESET}");
		}
		else if (stamRatio >= 1.0)
		{
			sb.Append($"      ");
		}

		sb.Append(" / Hydraulics: ");
		if (bloodlossRatio >= 0.95)
		{
			sb.Append($"{Telnet.Red.Colour}**{Telnet.Red.Bold}**{Telnet.White.Bold}**{Telnet.RESETALL}");
		}
		else if (bloodlossRatio >= 0.80)
		{
			sb.Append($"{Telnet.Red.Colour}**{Telnet.Red.Colour}**{Telnet.White.Colour}* {Telnet.RESETALL}");
		}
		else if (bloodlossRatio >= 0.6)
		{
			sb.Append($"{Telnet.Red.Colour}**{Telnet.Red.Colour}**  {Telnet.RESETALL}");
		}
		else if (bloodlossRatio >= 0.4)
		{
			sb.Append($"{Telnet.Red.Colour}**{Telnet.Red.Colour}*   {Telnet.RESETALL}");
		}
		else if (bloodlossRatio >= 0.2)
		{
			sb.Append($"{Telnet.Red.Colour}**    {Telnet.RESET}");
		}
		else if (bloodlossRatio >= 0.1)
		{
			sb.Append($"{Telnet.Red.Colour}*     {Telnet.RESET}");
		}
		else
		{
			sb.Append($"      ");
		}

		if (breathRatio < 1.0)
		{
			sb.Append(" / Breath: ");
			if (breathRatio <= 0.0)
			{
				sb.Append($"      ");
			}
			else if (breathRatio <= 0.1667)
			{
				sb.Append($"{Telnet.Blue.Colour}|     {Telnet.RESET}");
			}
			else if (breathRatio <= 0.3333)
			{
				sb.Append($"{Telnet.Blue.Colour}||    {Telnet.RESET}");
			}
			else if (breathRatio <= 0.5)
			{
				sb.Append($"{Telnet.Blue.Colour}||{Telnet.Cyan.Colour}|   {Telnet.RESET}");
			}
			else if (breathRatio <= 0.6667)
			{
				sb.Append($"{Telnet.Blue.Colour}||{Telnet.Cyan.Colour}||  {Telnet.RESET}");
			}
			else if (breathRatio <= 0.8335)
			{
				sb.Append($"{Telnet.Blue.Colour}||{Telnet.Cyan.Colour}||{Telnet.BoldCyan.Colour}| {Telnet.RESET}");
			}
			else
			{
				sb.Append($"{Telnet.Blue.Colour}||{Telnet.Cyan.Colour}||{Telnet.BoldCyan.Colour}||{Telnet.RESET}");
			}
		}

		return sb.ToString();
	}
}