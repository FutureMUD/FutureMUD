using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health.Wounds;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp.Health.Strategies;

public class SimpleLivingHealthStrategy : BaseHealthStrategy
{
	private SimpleLivingHealthStrategy(HealthStrategy strategy, IFuturemud gameworld)
		: base(strategy)
	{
		LoadDefinition(XElement.Parse(strategy.Definition), gameworld);
	}

	public ITraitExpression HealingTickDamageExpression { get; set; }
	public ITraitExpression HealingTickStunExpression { get; set; }
	public ITraitExpression HealingTickPainExpression { get; set; }
	public ITraitExpression MaximumHitPointsExpression { get; set; }
	public ITraitExpression MaximumStunExpression { get; set; }
	public ITraitExpression MaximumPainExpression { get; set; }

	public double PercentageHealthPerPenalty { get; set; }
	public double PercentageStunPerPenalty { get; set; }
	public double PercentagePainPerPenalty { get; set; }

	public double HPGrace { get; set; } = 1.1;
	public TimeSpan BleedMessageCooldown { get; set; }
	public int InternalBleedingRecoveryRollSides { get; set; }
	public double InternalBleedingRecoveryMinRatio { get; set; }
	public double InternalBleedingRecoveryFlatReduction { get; set; }
	public double InternalBleedingRecoveryMultiplier { get; set; }
	public double InternalBleedingTotalDecayFlatReduction { get; set; }
	public double InternalBleedingTotalDecayMultiplier { get; set; }
	public double AirwayBloodMinorThreshold { get; set; }
	public double AirwayBloodModerateThreshold { get; set; }
	public double AirwayBloodSevereThreshold { get; set; }
	public double DigestiveBloodMinorThreshold { get; set; }
	public double DigestiveBloodModerateThreshold { get; set; }
	public double DigestiveBloodSevereThreshold { get; set; }
	public double CardiacHypoxiaThreshold { get; set; }
	public double BloodReplacementHypoxiaThreshold { get; set; }
	public double HydratingHypoxiaThreshold { get; set; }
	public double TissueDieOffHarmfulMultiplier { get; set; }
	public double TissueDieOffDeadlyMultiplier { get; set; }
	public double TissueDieOffKidneyWasteThreshold { get; set; }
	public double TissueDieOffKidneyWasteMultiplier { get; set; }
	public double BloodLossDeadThreshold { get; set; }
	public double BrainActivityUnconsciousThreshold { get; set; }
	public double BloodLossUnconsciousThreshold { get; set; }
	public double AnesthesiaDeathThreshold { get; set; }
	public double AnesthesiaUnconsciousThreshold { get; set; }
	public double ContaminantHydratingRemovalFraction { get; set; }
	public double ContaminantBloodRemovalFraction { get; set; }
	public double BloodCleanseFraction { get; set; }
	public double BloodRegenerationFraction { get; set; }
	public double BloodGainThirstPointsPerTick { get; set; }
	public double BloodGainWaterLitresPerBloodChange { get; set; }

	public override string HealthStrategyType => "SimpleLiving";
	public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;

	public static void RegisterHealthStrategyLoader()
	{
		RegisterHealthStrategy("SimpleLiving", (strategy, game) => new SimpleLivingHealthStrategy(strategy, game));
	}

	private void LoadDefinition(XElement root, IFuturemud gameworld)
	{
		var element = root.Element("MaximumHitPointsExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a MaximumHitPointsExpression element.");
		}

		if (!long.TryParse(element.Value, out var value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a MaximumHitPointsExpression element that did not contain an ID.");
		}

		MaximumHitPointsExpression = gameworld.TraitExpressions.Get(value);

		PercentageHealthPerPenalty = LoadDouble(root, "PercentageHealthPerPenalty", 1.0);
		PercentageStunPerPenalty = LoadDouble(root, "PercentageStunPerPenalty", 1.0);
		PercentagePainPerPenalty = LoadDouble(root, "PercentagePainPerPenalty", 1.0);
		HPGrace = LoadDouble(root, "HPGrace", 1.1);
		BleedMessageCooldown = LoadTimeSpanFromSeconds(root, "BleedMessageCooldown", 15);
		InternalBleedingRecoveryRollSides = LoadInt(root, "InternalBleedingRecoveryRollSides", 20);
		InternalBleedingRecoveryMinRatio = LoadDouble(root, "InternalBleedingRecoveryMinRatio", 0.05);
		InternalBleedingRecoveryFlatReduction = LoadDouble(root, "InternalBleedingRecoveryFlatReduction", 0.01);
		InternalBleedingRecoveryMultiplier = LoadDouble(root, "InternalBleedingRecoveryMultiplier", 0.66);
		InternalBleedingTotalDecayFlatReduction = LoadDouble(root, "InternalBleedingTotalDecayFlatReduction", 1.0);
		InternalBleedingTotalDecayMultiplier = LoadDouble(root, "InternalBleedingTotalDecayMultiplier", 0.98);
		AirwayBloodMinorThreshold = LoadDouble(root, "AirwayBloodMinorThreshold", 0.025);
		AirwayBloodModerateThreshold = LoadDouble(root, "AirwayBloodModerateThreshold", 0.1);
		AirwayBloodSevereThreshold = LoadDouble(root, "AirwayBloodSevereThreshold", 0.2);
		DigestiveBloodMinorThreshold = LoadDouble(root, "DigestiveBloodMinorThreshold", 0.2);
		DigestiveBloodModerateThreshold = LoadDouble(root, "DigestiveBloodModerateThreshold", 0.33);
		DigestiveBloodSevereThreshold = LoadDouble(root, "DigestiveBloodSevereThreshold", 0.66);
		CardiacHypoxiaThreshold = LoadDouble(root, "CardiacHypoxiaThreshold", 0.3);
		BloodReplacementHypoxiaThreshold = LoadDouble(root, "BloodReplacementHypoxiaThreshold", 0.5);
		HydratingHypoxiaThreshold = LoadDouble(root, "HydratingHypoxiaThreshold", 0.66);
		TissueDieOffHarmfulMultiplier = LoadDouble(root, "TissueDieOffHarmfulMultiplier", 1.0);
		TissueDieOffDeadlyMultiplier = LoadDouble(root, "TissueDieOffDeadlyMultiplier", 5.0);
		TissueDieOffKidneyWasteThreshold = LoadDouble(root, "TissueDieOffKidneyWasteThreshold", 1000.0);
		TissueDieOffKidneyWasteMultiplier = LoadDouble(root, "TissueDieOffKidneyWasteMultiplier", 0.5);
		BloodLossDeadThreshold = LoadDouble(root, "BloodLossDeadThreshold", 0.5);
		BrainActivityUnconsciousThreshold = LoadDouble(root, "BrainActivityUnconsciousThreshold", 0.9);
		BloodLossUnconsciousThreshold = LoadDouble(root, "BloodLossUnconsciousThreshold", 0.6);
		AnesthesiaDeathThreshold = LoadDouble(root, "AnesthesiaDeathThreshold", 10.0);
		AnesthesiaUnconsciousThreshold = LoadDouble(root, "AnesthesiaUnconsciousThreshold", 1.0);
		ContaminantHydratingRemovalFraction = LoadDouble(root, "ContaminantHydratingRemovalFraction", 0.001);
		ContaminantBloodRemovalFraction = LoadDouble(root, "ContaminantBloodRemovalFraction", 0.002);
		BloodCleanseFraction = LoadDouble(root, "BloodCleanseFraction", 0.00025);
		BloodRegenerationFraction = LoadDouble(root, "BloodRegenerationFraction", 0.0005);
		BloodGainThirstPointsPerTick = LoadDouble(root, "BloodGainThirstPointsPerTick", -0.01);
		BloodGainWaterLitresPerBloodChange = LoadDouble(root, "BloodGainWaterLitresPerBloodChange", -0.85);

		element = root.Element("MaximumStunExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a MaximumStunExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a MaximumStunExpression element that did not contain an ID.");
		}

		MaximumStunExpression = gameworld.TraitExpressions.Get(value);

		element = root.Element("MaximumPainExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a MaximumPainExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a MaximumPainExpression element that did not contain an ID.");
		}

		MaximumPainExpression = gameworld.TraitExpressions.Get(value);

		element = root.Element("HealingTickDamageExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a HealingTickDamageExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a HealingTickDamageExpression element that did not contain an ID.");
		}

		HealingTickDamageExpression = gameworld.TraitExpressions.Get(value);

		element = root.Element("HealingTickStunExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a HealingTickStunExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a HealingTickStunExpression element that did not contain an ID.");
		}

		HealingTickStunExpression = gameworld.TraitExpressions.Get(value);

		element = root.Element("HealingTickPainExpression");
		if (element == null)
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} did not contain a HealingTickPainExpression element.");
		}

		if (!long.TryParse(element.Value, out value))
		{
			throw new ApplicationException(
				$"ComplexLivingHealthStrategy ID {Id} had a HealingTickPainExpression element that did not contain an ID.");
		}

		HealingTickPainExpression = gameworld.TraitExpressions.Get(value);
	}

	public override IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart)
	{
		if (bodypart == null)
		{
			return Enumerable.Empty<IWound>();
		}

		IGameItem lodgedItem = null;
		LodgeDamageExpression.Parameters["damage"] = damage.DamageAmount;
		LodgeDamageExpression.Parameters["type"] = (int)damage.DamageType;
		if (Dice.Roll(0, 100) < Convert.ToDouble(LodgeDamageExpression.Evaluate()))
		{
			lodgedItem = damage.LodgableItem;
		}

		return new[]
		{
			new SimpleOrganicWound(owner.Gameworld, (ICharacter)owner, damage.DamageAmount, damage.PainAmount,
				damage.StunAmount, damage.DamageType, damage.Bodypart, lodgedItem, damage.ToolOrigin,
				damage.ActorOrigin)
		};
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
		var charOwner = (ICharacter)thing;
		var totalBlood = charOwner.Body.TotalBloodVolumeLitres;
		if (charOwner.Body.CurrentBloodVolumeLitres > 0)
		{
			var bleedingWounds =
				thing.Wounds.Select(
					     x => x.Bleed(charOwner.Body.CurrentBloodVolumeLitres, charOwner.Body.CurrentExertion,
						     charOwner.Body.TotalBloodVolumeLitres))
				     .Where(x => x.BloodAmount > 0)
				     .ToList();
			charOwner.Body.CurrentBloodVolumeLitres -= bleedingWounds.Sum(x => x.BloodAmount);
			var visibleBleedingWounds = bleedingWounds.Where(x => x.Visible).ToList();
			if (visibleBleedingWounds.Any() && !charOwner.Body.EffectsOfType<ISuppressBleedMessage>().Any())
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
					new EmoteOutput(new Emote($"$0 bleed|bleeds from {messages.ListToString()}", charOwner,
						perceivables.ToArray())));
				if (!charOwner.Body.EffectsOfType<ISuppressBleedMessage>().Any())
				{
					charOwner.Body.AddEffect(new SuppressBleedMessage(charOwner.Body, null),
						BleedMessageCooldown);
				}
			}

			var internalBleeders = charOwner.Body.EffectsOfType<IInternalBleedingEffect>().ToList();
			var internalBleeding = internalBleeders.Sum(x => x.BloodlossPerTick);
			if (internalBleeding > 0)
			{
				var message = !thing.EffectsOfType<ISuppressBleedMessage>().Any() && charOwner.IsBreathing;
				var airwayBlood = 0.0;
				var digestiveBlood = 0.0;
				charOwner.Body.CurrentBloodVolumeLitres -= internalBleeding;
				foreach (var effect in internalBleeders)
				{
					if (Dice.Roll(1, InternalBleedingRecoveryRollSides) == 1 &&
					    effect.BloodlossPerTick > effect.BloodlossTotal * InternalBleedingRecoveryMinRatio)
					{
						effect.BloodlossPerTick = Math.Max(Math.Min(
							effect.BloodlossPerTick - InternalBleedingRecoveryFlatReduction,
							effect.BloodlossPerTick * InternalBleedingRecoveryMultiplier), 0);
					}

					if (effect.BloodlossPerTick <= 0.0)
					{
						effect.BloodlossTotal = Math.Max(0,
							Math.Min(effect.BloodlossTotal - InternalBleedingTotalDecayFlatReduction,
								effect.BloodlossTotal * InternalBleedingTotalDecayMultiplier));
					}

					if (effect.BloodlossPerTick <= 0.0 && effect.BloodlossTotal <= 0.0)
					{
						charOwner.Body.RemoveEffect(effect);
						continue;
					}

					effect.BloodlossTotal += effect.BloodlossPerTick;
					if (effect.Organ is LungProto)
					{
						airwayBlood += effect.BloodlossTotal;
					}
					else if (effect.Organ is TracheaProto)
					{
						airwayBlood += effect.BloodlossTotal;
					}
					else if (effect.Organ is StomachProto)
					{
						digestiveBlood += effect.BloodlossTotal;
					}
					else if (effect.Organ is EsophagusProto)
					{
						digestiveBlood += effect.BloodlossTotal;
					}
				}

				if (airwayBlood > AirwayBloodMinorThreshold && message)
				{
					var amount = "a small amount of";
					var action = "";
					if (airwayBlood > AirwayBloodModerateThreshold)
					{
						amount = "some";
					}
					else if (airwayBlood > AirwayBloodSevereThreshold)
					{
						amount = "a large amount of";
						action = "violently ";
					}

					charOwner.OutputHandler.Handle(
						new EmoteOutput(
							new Emote($"@ {action}cough|coughs up {amount} of {charOwner.Race.BloodLiquid.Name}.",
								charOwner)));
				}

				if (digestiveBlood > DigestiveBloodMinorThreshold && message)
				{
					var amount = "a small amount of";
					var action = "";
					if (digestiveBlood > DigestiveBloodModerateThreshold)
					{
						amount = "some";
					}
					else if (digestiveBlood > DigestiveBloodSevereThreshold)
					{
						amount = "a large amount of";
						action = "violently ";
					}

					charOwner.OutputHandler.Handle(
						new EmoteOutput(
							new Emote($"@ {action}spew|spews up {amount} of {charOwner.Race.BloodLiquid.Name}.",
								charOwner)));
				}
			}
		}

		var hypoxia = 0.0;
		var heartFactor =
			charOwner.Body.Organs.OfType<HeartProto>()
			         .Select(x => x.OrganFunctionFactor(charOwner.Body))
			         .DefaultIfEmpty(0)
			         .Sum();
		if (heartFactor < CardiacHypoxiaThreshold)
		{
			hypoxia += double.Parse(charOwner.Gameworld.GetStaticConfiguration("HypoxiaPerHeartFactor")) *
			           (1.0 - heartFactor / CardiacHypoxiaThreshold);
		}

		var bloodReplacementToBloodRatio =
			(charOwner.Body.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			          .FirstOrDefault(x => x.Applies() && x.Consequence == LiquidInjectionConsequence.BloodVolume)?
			          .Volume ?? 0) / totalBlood;
		var hydrateToBloodRatio =
			(charOwner.Body.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			          .FirstOrDefault(x => x.Applies() && x.Consequence == LiquidInjectionConsequence.Hydrating)?
			          .Volume ?? 0) / totalBlood;

		if (bloodReplacementToBloodRatio > BloodReplacementHypoxiaThreshold)
		{
			hypoxia += (bloodReplacementToBloodRatio - BloodReplacementHypoxiaThreshold) *
			           double.Parse(
				           charOwner.Gameworld.GetStaticConfiguration("HypoxiaFromBloodReplacementPerVolume"));
		}

		if (hydrateToBloodRatio > HydratingHypoxiaThreshold)
		{
			hypoxia += (hydrateToBloodRatio - HydratingHypoxiaThreshold) *
			           double.Parse(charOwner.Gameworld.GetStaticConfiguration("HypoxiaFromHydratePerVolume"));
		}

		if (charOwner.NeedsToBreathe && !charOwner.CanBreathe)
		{
			charOwner.OutputHandler.Send("You can't breathe! You are suffocating!");
			foreach (var organ in charOwner.Body.Organs)
			{
				var hypoxiaWound =
					charOwner.Body.Wounds.FirstOrDefault(
						x => x.Bodypart == organ && x.DamageType == DamageType.Hypoxia);
				if (hypoxiaWound == null)
				{
					charOwner.Body.SufferDamage(new Damage
					{
						ActorOrigin = charOwner,
						DamageAmount = organ.HypoxiaDamagePerTick,
						DamageType = DamageType.Hypoxia,
						Bodypart = organ,
						PainAmount = organ.HypoxiaDamagePerTick,
						PenetrationOutcome = Outcome.NotTested
					});
					continue;
				}

				hypoxiaWound.OriginalDamage += organ.HypoxiaDamagePerTick + hypoxia;
				hypoxiaWound.CurrentPain = hypoxiaWound.OriginalDamage + hypoxia;
				hypoxiaWound.CurrentDamage = hypoxiaWound.OriginalDamage + hypoxia;
			}
		}
		else if (hypoxia > 0)
		{
			foreach (var organ in charOwner.Body.Organs)
			{
				var hypoxiaWound =
					charOwner.Body.Wounds.FirstOrDefault(
						x => x.Bodypart == organ && x.DamageType == DamageType.Hypoxia);
				if (hypoxiaWound == null)
				{
					charOwner.Body.SufferDamage(new Damage
					{
						ActorOrigin = charOwner,
						DamageAmount = organ.HypoxiaDamagePerTick,
						DamageType = DamageType.Hypoxia,
						Bodypart = organ,
						PainAmount = organ.HypoxiaDamagePerTick,
						PenetrationOutcome = Outcome.NotTested
					});
					continue;
				}

				hypoxiaWound.OriginalDamage += hypoxia;
				hypoxiaWound.CurrentPain = hypoxia;
				hypoxiaWound.CurrentDamage = hypoxia;
			}
		}

		var dieoffFactor =
			double.Parse(charOwner.Gameworld.GetStaticConfiguration("TissueDieOffMultiplierPerVolumeContaminant"));
		var tissueDieOff =
			charOwner.Body.EffectsOfType<IHarmfulBloodAdditiveEffect>().Where(x => x.Applies()).Select(x =>
			{
				switch (x.Consequence)
				{
					case LiquidInjectionConsequence.Harmful:
						return TissueDieOffHarmfulMultiplier * x.Volume * dieoffFactor;
					case LiquidInjectionConsequence.Deadly:
						return TissueDieOffDeadlyMultiplier * x.Volume * dieoffFactor;
					case LiquidInjectionConsequence.KidneyWaste:
						return x.Volume > TissueDieOffKidneyWasteThreshold
							? TissueDieOffKidneyWasteMultiplier * (x.Volume - TissueDieOffKidneyWasteThreshold) *
							  dieoffFactor
							: 0.0;
					default:
						return 0.0;
				}
			}).DefaultIfEmpty(0).Sum();

		if (tissueDieOff > 0)
		{
			foreach (var organ in charOwner.Body.Organs)
			{
				if (organ is BrainProto)
				{
					continue;
				}

				var wound =
					charOwner.Body.Wounds.FirstOrDefault(
						x => x.Bodypart == organ && x.DamageType == DamageType.Cellular);
				if (wound == null)
				{
					charOwner.Body.SufferDamage(new Damage
					{
						ActorOrigin = charOwner,
						DamageAmount = tissueDieOff,
						DamageType = DamageType.Cellular,
						Bodypart = organ,
						PainAmount = tissueDieOff,
						PenetrationOutcome = Outcome.NotTested
					});
					continue;
				}

				wound.OriginalDamage += tissueDieOff;
				wound.CurrentPain = tissueDieOff;
				wound.CurrentDamage = tissueDieOff;
			}
		}

		return EvaluateStatus(thing);
	}

	private static string ColourRatio(string text, double ratio, string replacement)
	{
		ANSIColour colour;
		var boldColour = false;
		if (ratio >= 1.0)
		{
			colour = Telnet.Green;
		}
		else if (ratio > 0.8)
		{
			colour = Telnet.Green;
			boldColour = true;
		}
		else if (ratio > 0.4)
		{
			colour = Telnet.Yellow;
			boldColour = true;
		}
		else if (ratio > 0.2)
		{
			colour = Telnet.Red;
			boldColour = true;
		}
		else
		{
			colour = Telnet.Red;
		}

		return string.Format(text,
			boldColour ? colour.Bold + replacement + Telnet.RESETALL : colour.Colour + replacement + Telnet.RESETALL);
	}

	public override string ReportConditionPrompt(IHaveWounds owner, PromptType type)
	{
		var charOwner = (ICharacter)owner;
		var maxHp = MaximumHitPointsExpression.Evaluate(charOwner);
		var maxStun = MaximumStunExpression.Evaluate(charOwner);
		var maxPain = MaximumPainExpression.Evaluate(charOwner);
		var bloodlossRatio = charOwner.Body.CurrentBloodVolumeLitres / charOwner.Body.TotalBloodVolumeLitres;
		string extraStatusDescription;
		switch (bloodlossRatio)
		{
			case >= 1.0:
				extraStatusDescription = "no blood loss".Colour(Telnet.Green);
				break;
			case >= 0.95:
				extraStatusDescription = "very minor blood loss".Colour(Telnet.Yellow);
				break;
			case >= 0.90:
				extraStatusDescription = "minor blood loss".Colour(Telnet.Yellow);
				break;
			case >= 0.825:
				extraStatusDescription = "moderate blood loss".Colour(Telnet.Red);
				break;
			case >= 0.75:
				extraStatusDescription = "major blood loss".Colour(Telnet.Red);
				break;
			case >= 0.675:
				extraStatusDescription = "severe blood loss".Colour(Telnet.Red);
				break;
			case >= 0.6:
				extraStatusDescription = "very severe blood loss".Colour(Telnet.Red);
				break;
			case >= 0.5:
				extraStatusDescription = "critical blood loss".Colour(Telnet.Red);
				break;
			default:
				extraStatusDescription = "life-threatening blood loss".Colour(Telnet.Red);
				break;
		}

		var heartFactor =
			charOwner.Body.Organs.OfType<HeartProto>()
			         .Select(x => x.OrganFunctionFactor(charOwner.Body))
			         .DefaultIfEmpty(0)
			         .Sum();
		if (heartFactor <= 0)
		{
			extraStatusDescription += ", cardiac arrest";
		}
		else if (heartFactor < CardiacHypoxiaThreshold)
		{
			extraStatusDescription += ", heart attack";
		}
		else
		{
			if (charOwner.NeedsToBreathe && !charOwner.CanBreathe)
			{
				if (charOwner.Location.Atmosphere is ILiquid)
				{
					extraStatusDescription += ", drowning";
				}
				else
				{
					extraStatusDescription += ", suffocating";
				}
			}
		}

		return string.Format(charOwner, "<{0} {1} {2}{3}>",
			ColourRatio("HP: {0}",
				(maxHp - owner.Wounds.Sum(x => x.CurrentDamage)) / maxHp,
				string.Format(charOwner, "{0:N0}/{1:N0}", maxHp - owner.Wounds.Sum(x => x.CurrentDamage), maxHp)),
			ColourRatio("ST: {0}",
				(maxStun - owner.Wounds.Sum(x => x.CurrentStun)) / maxStun,
				string.Format(charOwner, "{0:N0}/{1:N0}", maxStun - owner.Wounds.Sum(x => x.CurrentStun), maxStun)),
			ColourRatio("PN: {0}",
				(maxPain - owner.Wounds.Sum(x => x.CurrentPain)) / maxPain,
				string.Format(charOwner, "{0:N0}/{1:N0}", maxPain - owner.Wounds.Sum(x => x.CurrentPain), maxPain)),
			$" - {extraStatusDescription}"
		);
	}

	public override double GetHealingTickAmount(IWound wound, Outcome outcome, HealthDamageType type)
	{
		ITraitExpression whichExpression;
		switch (type)
		{
			case HealthDamageType.Damage:
				whichExpression = HealingTickDamageExpression;
				break;
			case HealthDamageType.Pain:
				whichExpression = HealingTickPainExpression;
				break;
			case HealthDamageType.Shock:
				return 0;
			case HealthDamageType.Stun:
				whichExpression = HealingTickStunExpression;
				break;
			default:
				return 0;
		}

		whichExpression.Formula.Parameters["originaldamage"] = wound.OriginalDamage;
		whichExpression.Formula.Parameters["damage"] = wound.CurrentDamage;
		whichExpression.Formula.Parameters["pain"] = wound.CurrentPain;
		whichExpression.Formula.Parameters["stun"] = wound.CurrentStun;
		whichExpression.Formula.Parameters["shock"] = wound.CurrentShock;
		whichExpression.Formula.Parameters["outcome"] = outcome.SuccessDegrees();
		return whichExpression.Evaluate((ICharacter)wound.Parent);
	}

	#region Overrides of BaseHealthStrategy

	public override bool KidneyFunctionActive => true;

	public override double MaxHP(IHaveWounds owner)
	{
		return owner is not ICharacter charOwner ? 0.0 : MaximumHitPointsExpression.Evaluate(charOwner);
	}

	public override double MaxStun(IHaveWounds owner)
	{
		return owner is not ICharacter charOwner ? 0.0 : MaximumStunExpression.Evaluate(charOwner);
	}

	public override double MaxPain(IHaveWounds owner)
	{
		return owner is not ICharacter charOwner ? 0.0 : MaximumPainExpression.Evaluate(charOwner);
	}

	public override HealthTickResult EvaluateStatus(IHaveWounds thing)
	{
		var charOwner = (ICharacter)thing;
		if (charOwner.State.HasFlag(CharacterState.Dead))
		{
			return HealthTickResult.Dead;
		}

		var maxHP = MaximumHitPointsExpression.Evaluate(charOwner);

		var currentDamage = thing.Wounds.Sum(x => x.CurrentDamage);
		if (currentDamage >= maxHP * HPGrace)
		{
			return HealthTickResult.Dead;
		}

		var totalBlood = charOwner.Body.TotalBloodVolumeLitres;
		var bloodRatio = charOwner.Body.CurrentBloodVolumeLitres / totalBlood;

		if (bloodRatio < BloodLossDeadThreshold)
		{
			return HealthTickResult.Dead;
		}

		var brainActivity =
			charOwner.Body.Organs.OfType<BrainProto>()
			         .Select(x => x.OrganFunctionFactor(charOwner.Body))
			         .DefaultIfEmpty(0)
			         .Sum();
		if (brainActivity <= 0)
		{
			return HealthTickResult.Dead;
		}

		if (brainActivity < BrainActivityUnconsciousThreshold)
		{
			return HealthTickResult.Unconscious;
		}

		if (
			charOwner.Body.Organs.OfType<HeartProto>()
			         .Select(x => x.OrganFunctionFactor(charOwner.Body))
			         .DefaultIfEmpty(0)
			         .Sum() <= 0.0)
		{
			return HealthTickResult.Unconscious;
		}

		if (currentDamage >= maxHP)
		{
			return HealthTickResult.Unconscious;
		}

		if (bloodRatio < BloodLossUnconsciousThreshold)
		{
			return HealthTickResult.Unconscious;
		}

		var maxStun = MaximumStunExpression.Evaluate(charOwner);
		var maxPain = MaximumPainExpression.Evaluate(charOwner);

		if (thing.Wounds.Sum(x => x.CurrentPain) >= maxPain)
		{
			return HealthTickResult.PassOut;
		}

		if (thing.Wounds.Sum(x => x.CurrentStun) >= maxStun)
		{
			return HealthTickResult.Unconscious;
		}

		var anasthesia =
			charOwner.Body.EffectsOfType<Anesthesia>().Select(x => x.IntensityPerGramMass).DefaultIfEmpty(0).Sum();
		if (anasthesia >= AnesthesiaDeathThreshold)
		{
			return HealthTickResult.Dead;
		}

		return anasthesia >= AnesthesiaUnconsciousThreshold
			? HealthTickResult.Unconscious
			: HealthTickResult.None;
	}

	private void RemoveContaminants(IBody body, List<IHarmfulBloodAdditiveEffect> contaminants)
	{
		// try hydrating contaminants first
		var hydrate = contaminants.FirstOrDefault(x => x.Consequence == LiquidInjectionConsequence.Hydrating);
		if (hydrate != null)
		{
			hydrate.Volume -= body.TotalBloodVolumeLitres * ContaminantHydratingRemovalFraction;
			if (hydrate.Volume <= 0)
			{
				body.RemoveEffect(hydrate);
			}

			return;
		}

		var blood = contaminants.FirstOrDefault(x => x.Consequence == LiquidInjectionConsequence.BloodVolume);
		if (blood != null)
		{
			blood.Volume -= body.TotalBloodVolumeLitres * ContaminantBloodRemovalFraction;
			if (blood.Volume <= 0)
			{
				body.RemoveEffect(blood);
			}
		}
	}

	public override void PerformBloodGain(IHaveWounds owner)
	{
		// Blood gain requires that the patient be well hydrated, and also has an impact on hydration
		var cOwner = (ICharacter)owner;
		var body = cOwner.Body;
		var totalBlood = body.TotalBloodVolumeLitres;
		var contaminants =
			body.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			    .Where(
				    x =>
					    x.Applies() &&
					    (x.Consequence == LiquidInjectionConsequence.BloodVolume ||
					     x.Consequence == LiquidInjectionConsequence.Hydrating))
			    .ToList();

		if (body.CurrentBloodVolumeLitres < totalBlood || contaminants.Any())
		{
			double bloodChange = 0;
			if (!body.NeedsModel.Status.HasFlag(NeedsResult.Parched) &&
			    !body.NeedsModel.Status.HasFlag(NeedsResult.Thirsty))
			{
				if (body.CurrentBloodVolumeLitres == totalBlood)
				{
					RemoveContaminants(body, contaminants);
					bloodChange = totalBlood * BloodCleanseFraction;
				}
				else
				{
					var newBlood = Math.Min(body.CurrentBloodVolumeLitres + BloodRegenerationFraction * totalBlood,
						totalBlood);
					bloodChange = newBlood - body.CurrentBloodVolumeLitres;
					body.CurrentBloodVolumeLitres = newBlood;
				}
			}

			body.FulfilNeeds(new NeedFulfiller
			{
				ThirstPoints = BloodGainThirstPointsPerTick,
				WaterLitres = BloodGainWaterLitresPerBloodChange * bloodChange
			});
		}
	}

	#endregion

	#region Overrides of BaseHealthStrategy

	public override double WoundPenaltyFor(IHaveWounds owner)
	{
		if (owner is not ICharacter charOwner)
		{
			return 0;
		}

		var penalty =
			charOwner.Wounds.Sum(
				x =>
					x.CurrentDamage / PercentageHealthPerPenalty + x.CurrentPain / PercentagePainPerPenalty +
					x.CurrentStun / PercentageStunPerPenalty) /
			(MaximumHitPointsExpression.Evaluate(charOwner) + MaximumStunExpression.Evaluate(charOwner) +
			 MaximumPainExpression.Evaluate(charOwner));
		return -1 * (int)penalty;
	}

	public override void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
	{
		var cOwner = (ICharacter)owner;
		foreach (var liquid in mixture.Instances)
		{
			if (liquid.Liquid.Drug != null)
			{
				cOwner.Body.Dose(liquid.Liquid.Drug, DrugVector.Injected,
					liquid.Amount * liquid.Liquid.DrugGramsPerUnitVolume);
			}

			if (liquid.Liquid.InjectionConsequence == LiquidInjectionConsequence.Benign)
			{
				continue;
			}

			switch (liquid.Liquid.InjectionConsequence)
			{
				case LiquidInjectionConsequence.Hydrating:
					cOwner.Body.FulfilNeeds(new NeedFulfiller
					{
						ThirstPoints = liquid.Liquid.DrinkSatiatedHoursPerLitre /
						               (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
						WaterLitres = liquid.Liquid.WaterLitresPerLitre /
						              (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
						AlcoholLitres = liquid.Liquid.AlcoholLitresPerLitre /
						                (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres),
						SatiationPoints = liquid.Liquid.FoodSatiatedHoursPerLitre /
						                  (liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres)
					});
					continue;
				case LiquidInjectionConsequence.BloodReplacement:
					if (!(liquid is BloodLiquidInstance blood))
					{
						continue;
					}

					cOwner.Body.CurrentBloodVolumeLitres +=
						liquid.Amount * owner.Gameworld.UnitManager.BaseFluidToLitres;

					if (cOwner.Body.Bloodtype?.IsCompatibleWithDonorBlood(blood.BloodType) != false)
					{
						continue;
					}

					break;
			}

			var effect =
				cOwner.Body
				      .EffectsOfType<IHarmfulBloodAdditiveEffect>()
				      .FirstOrDefault(x => x.Consequence == liquid.Liquid.InjectionConsequence);
			if (effect == null)
			{
				effect = new HarmfulBloodAdditive(cOwner.Body, liquid.Liquid.InjectionConsequence, 0);
				cOwner.Body.AddEffect(effect);
			}

			effect.Volume += liquid.Amount;
		}
	}

	#endregion
}
