using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.Health.Wounds;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Health.Strategies;

public class RobotHealthStrategy : BaseHealthStrategy
{
	private static readonly TraitExpressionBuilderField<RobotHealthStrategy>[] TraitExpressionFields =
	[
		new("MaximumHitPointsExpression", ["maxhp", "maximumhitpointsexpression"], "Maximum Hit Points Expression",
			x => x.MaximumHitPointsExpression, (x, value) => x.MaximumHitPointsExpression = value),
		new("MaximumStunExpression", ["maxstun", "maximumstunexpression"], "Maximum Stun Expression",
			x => x.MaximumStunExpression, (x, value) => x.MaximumStunExpression = value)
	];

	private static readonly DoubleBuilderField<RobotHealthStrategy>[] DoubleFields =
	[
		PercentageField<RobotHealthStrategy>("PercentageHealthPerPenalty", ["percentagehealthperpenalty", "healthpenalty"], "Percentage Health Per Penalty",
			x => x.PercentageHealthPerPenalty, (x, value) => x.PercentageHealthPerPenalty = value),
		PercentageField<RobotHealthStrategy>("PercentageStunPerPenalty", ["percentagestunperpenalty", "stunpenalty"], "Percentage Stun Per Penalty",
			x => x.PercentageStunPerPenalty, (x, value) => x.PercentageStunPerPenalty = value),
		PercentageField<RobotHealthStrategy>("PowerCoreCriticalThreshold", ["powercorecriticalthreshold"], "Power Core Critical Threshold",
			x => x.PowerCoreCriticalThreshold, (x, value) => x.PowerCoreCriticalThreshold = value),
		UnitField<RobotHealthStrategy>("HydraulicFluidParalysisThreshold", ["hydraulicfluidparalysisthreshold"], "Hydraulic Fluid Paralysis Threshold",
			UnitType.FluidVolume,
			x => x.HydraulicFluidParalysisThreshold, (x, value) => x.HydraulicFluidParalysisThreshold = value)
	];

	private static readonly TimeSpanBuilderField<RobotHealthStrategy>[] TimeSpanFields =
	[
		new("BleedMessageCooldown", ["bleedmessagecooldown"], "Bleed Message Cooldown",
			x => x.BleedMessageCooldown, (x, value) => x.BleedMessageCooldown = value)
	];

	private const string TypeBlurb =
		"A robotic health model with hydraulic leakage, stun limits, and power-core failure handling.";

	private static readonly string TypeHelp = BuildTypeHelp(TypeBlurb,
		GetBuilderFieldHelpText(TraitExpressionFields)
			.Concat(GetBuilderFieldHelpText(DoubleFields))
			.Concat(GetBuilderFieldHelpText(TimeSpanFields)));

	protected RobotHealthStrategy(HealthStrategy strategy, IFuturemud gameworld)
		: base(strategy, gameworld)
	{
		LoadDefinition(XElement.Parse(strategy.Definition), gameworld);
	}

	private RobotHealthStrategy(IFuturemud gameworld, string name)
		: base(gameworld, name)
	{
		MaximumHitPointsExpression = CreateDefaultExpression(gameworld, $"{name} Max HP", "100");
		MaximumStunExpression = CreateDefaultExpression(gameworld, $"{name} Max Stun", "100");
		PercentageHealthPerPenalty = 1.0;
		PercentageStunPerPenalty = 1.0;
		BleedMessageCooldown = TimeSpan.FromSeconds(15);
		PowerCoreCriticalThreshold = 0.3;
		HydraulicFluidParalysisThreshold = 0.0;
		DoDatabaseInsert(HealthStrategyType);
	}

	private RobotHealthStrategy(RobotHealthStrategy rhs, string name)
		: base(rhs, name)
	{
		MaximumHitPointsExpression = CloneExpression(rhs.MaximumHitPointsExpression, Gameworld);
		MaximumStunExpression = CloneExpression(rhs.MaximumStunExpression, Gameworld);
		PercentageHealthPerPenalty = rhs.PercentageHealthPerPenalty;
		PercentageStunPerPenalty = rhs.PercentageStunPerPenalty;
		BleedMessageCooldown = rhs.BleedMessageCooldown;
		PowerCoreCriticalThreshold = rhs.PowerCoreCriticalThreshold;
		HydraulicFluidParalysisThreshold = rhs.HydraulicFluidParalysisThreshold;
		DoDatabaseInsert(HealthStrategyType);
	}

	public override string HealthStrategyType => "Robot";
	public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;
	public override bool RequiresSpinalCord => false;

	public ITraitExpression MaximumHitPointsExpression { get; set; }
	public ITraitExpression MaximumStunExpression { get; set; }
	public double PercentageHealthPerPenalty { get; set; }
	public double PercentageStunPerPenalty { get; set; }
	public TimeSpan BleedMessageCooldown { get; set; }
	public double PowerCoreCriticalThreshold { get; set; }
	public double HydraulicFluidParalysisThreshold { get; set; }

	protected override IEnumerable<string> SubtypeBuilderHelpText =>
		GetBuilderFieldHelpText(TraitExpressionFields)
			.Concat(GetBuilderFieldHelpText(DoubleFields))
			.Concat(GetBuilderFieldHelpText(TimeSpanFields));

	public static void RegisterHealthStrategyLoader()
	{
		RegisterHealthStrategy("Robot",
			(strategy, game) => new RobotHealthStrategy(strategy, game),
			(game, name) => new RobotHealthStrategy(game, name),
			TypeHelp,
			TypeBlurb);
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
		PercentageHealthPerPenalty = LoadDouble(root, "PercentageHealthPerPenalty", 1.0);
		PercentageStunPerPenalty = LoadDouble(root, "PercentageStunPerPenalty", 1.0);

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
		BleedMessageCooldown = LoadTimeSpanFromSeconds(root, "BleedMessageCooldown", 15);
		PowerCoreCriticalThreshold = LoadDouble(root, "PowerCoreCriticalThreshold", 0.3);
		HydraulicFluidParalysisThreshold = LoadDouble(root, "HydraulicFluidParalysisThreshold", 0.0);
	}

	protected override void SaveSubtypeDefinition(XElement root)
	{
		SaveBuilderFields(root, this, TraitExpressionFields);
		SaveBuilderFields(root, this, DoubleFields);
		SaveBuilderFields(root, this, TimeSpanFields);
	}

	protected override void AppendSubtypeShow(StringBuilder sb, ICharacter actor)
	{
		sb.AppendLine();
		AppendBuilderFieldShow(sb, actor, this, TraitExpressionFields);
		AppendBuilderFieldShow(sb, actor, this, DoubleFields);
		AppendBuilderFieldShow(sb, actor, this, TimeSpanFields);
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (TryBuildingCommand(actor, command.GetUndo(), TraitExpressionFields))
		{
			return true;
		}

		if (TryBuildingCommand(actor, command.GetUndo(), DoubleFields))
		{
			return true;
		}

		if (TryBuildingCommand(actor, command.GetUndo(), TimeSpanFields))
		{
			return true;
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	public override IHealthStrategy Clone(string name)
	{
		return new RobotHealthStrategy(this, name);
	}

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

		IGameItem lodgedItem = CheckDamageLodges(damage) ? damage.LodgableItem : null;

		return
		[
			new RobotWound(owner.Gameworld, owner, damage.DamageAmount, damage.StunAmount, damage.DamageType,
				damage.Bodypart, lodgedItem, damage.ToolOrigin, damage.ActorOrigin)
		];
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

	private static string WoundCountDesc(int count)
	{
		return count switch
		{
			1 => "a wound",
			2 => "a couple of wounds",
			3 or 4 => "a few wounds",
			5 or 6 or 7 or 8 => "several wounds",
			_ => "many wounds"
		};
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
							BleedMessageCooldown);
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
		var lossOfConsciousnessEffect = charOwner.Body.EffectsOfType<ILossOfConsciousnessEffect>().FirstOrDefault(x => x.Applies());
		return EvaluateStatusFromVitals(
			charOwner.State.HasFlag(CharacterState.Dead),
			charOwner.Body.OrganFunction<PositronicBrain>(),
			lossOfConsciousnessEffect?.UnconType,
			charOwner.Body.OrganFunction<PowerCore>(),
			charOwner.Body.CurrentBloodVolumeLitres,
			HydraulicFluidParalysisThreshold,
			thing.Wounds.Sum(x => x.CurrentStun),
			MaximumStunExpression.Evaluate(charOwner),
			charOwner.Body.EffectsOfType<IPreventPassOut>().Any(x => x.Applies()));
	}

	internal static HealthTickResult EvaluateStatusFromVitals(bool isDead, double brainActivity,
		HealthTickResult? lossOfConsciousnessType, double powerCoreFunction, double currentBloodVolumeLitres,
		double hydraulicFluidParalysisThreshold, double totalStun, double maxStun, bool preventPassOut)
	{
		if (isDead)
		{
			return HealthTickResult.Dead;
		}

		if (brainActivity <= 0.0)
		{
			return HealthTickResult.Dead;
		}

		if (lossOfConsciousnessType.HasValue)
		{
			return lossOfConsciousnessType.Value;
		}

		if (powerCoreFunction <= 0.0)
		{
			return HealthTickResult.Unconscious;
		}

		if (currentBloodVolumeLitres <= hydraulicFluidParalysisThreshold)
		{
			return HealthTickResult.Paralyzed;
		}

		if (totalStun >= maxStun && !preventPassOut)
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

		return type switch
		{
			PromptType.Classic => ReportClassicPrompt(charOwner, stunRatio, painRatio, bloodlossRatio, totalBreath),
			PromptType.Full => ReportFullPrompt(charOwner, stunRatio, painRatio, bloodlossRatio, totalBreath, false),
			PromptType.FullBrief => ReportFullPrompt(charOwner, stunRatio, painRatio, bloodlossRatio, totalBreath, true),
			_ => ">"
		};
	}

	private string ReportFullPrompt(ICharacter charOwner, double stunRatio, double painRatio, double bloodlossRatio,
		double breathRatio, bool brief)
	{
		var sb = new StringBuilder();
		sb.Append("<");
		var power = charOwner.Body.OrganFunction<PowerCore>();
		if (power < PowerCoreCriticalThreshold)
		{
			sb.Append("*** YOUR POWER CORE IS CRITICAL! ***".Colour(Telnet.BoldRed));
			sb.Append(">\n<");
		}

		if (charOwner.NeedsToBreathe && !charOwner.CanBreathe)
		{
			if (breathRatio > 0.0)
			{
				sb.Append("*** YOU ARE HOLDING YOUR BREATH! ***".Colour(Telnet.BoldBlue));
				sb.Append(">\n<");
			}
			else if (charOwner.BreathingFluid is ILiquid)
			{
				sb.Append("*** YOU ARE DROWNING! ***".Colour(Telnet.BoldCyan));
				sb.Append(">\n<");
			}
			else
			{
				sb.Append("*** YOU ARE SUFFOCATING! ***".Colour(Telnet.BoldYellow));
				sb.Append(">\n<");
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

		var fluidLossDescription = FluidLossDescriptionForPrompt(
			bloodlossRatio,
			charOwner.Race.BloodLiquid?.Name);
		if (!string.IsNullOrEmpty(fluidLossDescription))
		{
			sb.Append(fluidLossDescription);
		}

		sb.Append(">");
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
			sb.Append("      ");
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
			sb.Append("      ");
		}

		sb.Append($" / {CirculatoryFluidPromptLabel(charOwner.Race.BloodLiquid?.Name)}: ");
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
			sb.Append("      ");
		}

		if (breathRatio < 1.0)
		{
			sb.Append(" / Breath: ");
			if (breathRatio <= 0.0)
			{
				sb.Append("      ");
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

	internal static string CirculatoryFluidName(string? liquidName)
	{
		return string.IsNullOrWhiteSpace(liquidName)
			? "circulatory fluid"
			: liquidName.ToLowerInvariant();
	}

	internal static string CirculatoryFluidPromptLabel(string? liquidName)
	{
		if (string.IsNullOrWhiteSpace(liquidName))
		{
			return "Hydraulics";
		}

		return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(liquidName.ToLowerInvariant());
	}

	internal static string FluidLossDescriptionForPrompt(double bloodlossRatio, string? liquidName)
	{
		var fluidName = CirculatoryFluidName(liquidName);
		if (bloodlossRatio >= 0.95)
		{
			return string.Empty;
		}

		if (bloodlossRatio >= 0.80)
		{
			return $"very minor {fluidName} loss".Colour(Telnet.Yellow);
		}

		if (bloodlossRatio >= 0.65)
		{
			return $"minor {fluidName} loss".Colour(Telnet.Yellow);
		}

		if (bloodlossRatio >= 0.5)
		{
			return $"moderate {fluidName} loss".Colour(Telnet.Red);
		}

		if (bloodlossRatio >= 0.35)
		{
			return $"major {fluidName} loss".Colour(Telnet.Red);
		}

		if (bloodlossRatio >= 0.2)
		{
			return $"severe {fluidName} loss".Colour(Telnet.Red);
		}

		if (bloodlossRatio >= 0.1)
		{
			return $"critical {fluidName} loss".Colour(Telnet.Red);
		}

		return $"hyper-critical {fluidName} loss".Colour(Telnet.Red);
	}
}
