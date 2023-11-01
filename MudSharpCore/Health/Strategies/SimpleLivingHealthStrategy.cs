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

		PercentageHealthPerPenalty = double.Parse(root.Element("PercentageHealthPerPenalty")?.Value ?? "1.0");
		PercentageStunPerPenalty = double.Parse(root.Element("PercentageStunPerPenalty")?.Value ?? "1.0");
		PercentagePainPerPenalty = double.Parse(root.Element("PercentagePainPerPenalty")?.Value ?? "1.0");

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
			new SimpleOrganicWound(owner.Gameworld, owner, damage.DamageAmount, damage.PainAmount,
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
						TimeSpan.FromSeconds(15));
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
					if (Dice.Roll(1, 20) == 1 && effect.BloodlossPerTick > effect.BloodlossTotal * 0.05)
					{
						effect.BloodlossPerTick = Math.Max(Math.Min(effect.BloodlossPerTick - 0.01,
							effect.BloodlossPerTick * 0.66), 0);
					}

					if (effect.BloodlossPerTick <= 0.0)
					{
						effect.BloodlossTotal = Math.Max(0,
							Math.Min(effect.BloodlossTotal - 1.0, effect.BloodlossTotal * 0.98));
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

				if (airwayBlood > 0.025 && message)
				{
					var amount = "a small amount of";
					var action = "";
					if (airwayBlood > 0.1)
					{
						amount = "some";
					}
					else if (airwayBlood > 0.2)
					{
						amount = "a large amount of";
						action = "violently ";
					}

					charOwner.OutputHandler.Handle(
						new EmoteOutput(
							new Emote($"@ {action}cough|coughs up {amount} of {charOwner.Race.BloodLiquid.Name}.",
								charOwner)));
				}

				if (digestiveBlood > 0.2 && message)
				{
					var amount = "a small amount of";
					var action = "";
					if (digestiveBlood > 0.33)
					{
						amount = "some";
					}
					else if (digestiveBlood > 0.66)
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
		if (heartFactor < 0.3)
		{
			hypoxia += double.Parse(charOwner.Gameworld.GetStaticConfiguration("HypoxiaPerHeartFactor")) *
			           (1 - 3 * heartFactor);
		}

		var bloodReplacementToBloodRatio =
			(charOwner.Body.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			          .FirstOrDefault(x => x.Applies() && x.Consequence == LiquidInjectionConsequence.BloodVolume)?
			          .Volume ?? 0) / totalBlood;
		var hydrateToBloodRatio =
			(charOwner.Body.EffectsOfType<IHarmfulBloodAdditiveEffect>()
			          .FirstOrDefault(x => x.Applies() && x.Consequence == LiquidInjectionConsequence.Hydrating)?
			          .Volume ?? 0) / totalBlood;

		if (bloodReplacementToBloodRatio > 0.5)
		{
			hypoxia += (bloodReplacementToBloodRatio - 0.5) *
			           double.Parse(
				           charOwner.Gameworld.GetStaticConfiguration("HypoxiaFromBloodReplacementPerVolume"));
		}

		if (hydrateToBloodRatio > 0.66)
		{
			hypoxia += (hydrateToBloodRatio - 0.66) *
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
						return 1.0 * x.Volume * dieoffFactor;
					case LiquidInjectionConsequence.Deadly:
						return 5.0 * x.Volume * dieoffFactor;
					case LiquidInjectionConsequence.KidneyWaste:
						return x.Volume > 1000 ? 0.5 * (x.Volume - 1000) * dieoffFactor : 0.0;
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
		else if (heartFactor < 0.3)
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

		return string.Format(charOwner, "{0} {1} {2}{3}",
			ColourRatio("Hp: {0}",
				(maxHp - owner.Wounds.Sum(x => x.CurrentDamage)) / maxHp,
				string.Format(charOwner, "{0:N0}/{1:N0}", maxHp - owner.Wounds.Sum(x => x.CurrentDamage), maxHp)),
			ColourRatio("St: {0}",
				(maxStun - owner.Wounds.Sum(x => x.CurrentStun)) / maxStun,
				string.Format(charOwner, "{0:N0}/{1:N0}", maxStun - owner.Wounds.Sum(x => x.CurrentStun), maxStun)),
			ColourRatio("Pn: {0}",
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

		if (bloodRatio < 0.5)
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

		if (brainActivity < 0.9)
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

		if (bloodRatio < 0.6)
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
		if (anasthesia >= 50)
		{
			return HealthTickResult.Dead;
		}

		return anasthesia >= 2 ? HealthTickResult.Unconscious : HealthTickResult.None;
	}

	private void RemoveContaminants(IBody body, List<IHarmfulBloodAdditiveEffect> contaminants)
	{
		// try hydrating contaminants first
		var hydrate = contaminants.FirstOrDefault(x => x.Consequence == LiquidInjectionConsequence.Hydrating);
		if (hydrate != null)
		{
			hydrate.Volume -= body.TotalBloodVolumeLitres * 0.001;
			if (hydrate.Volume <= 0)
			{
				body.RemoveEffect(hydrate);
			}

			return;
		}

		var blood = contaminants.FirstOrDefault(x => x.Consequence == LiquidInjectionConsequence.BloodVolume);
		if (blood != null)
		{
			blood.Volume -= body.TotalBloodVolumeLitres * 0.002;
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
					bloodChange = totalBlood * 0.00025;
				}
				else
				{
					var newBlood = Math.Min(body.CurrentBloodVolumeLitres + 0.0005 * totalBlood, totalBlood);
					bloodChange = newBlood - body.CurrentBloodVolumeLitres;
					body.CurrentBloodVolumeLitres = newBlood;
				}
			}

			body.FulfilNeeds(new NeedFulfiller
			{
				ThirstPoints = -0.01,
				WaterLitres = -0.85 * bloodChange
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
						Calories = liquid.Liquid.CaloriesPerLitre /
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