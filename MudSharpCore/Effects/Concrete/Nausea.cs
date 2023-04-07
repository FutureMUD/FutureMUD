using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class Nausea : Effect, ICheckBonusEffect, IScoreAddendumEffect
{
	public Nausea(IPerceivable owner, double intesitypergrammass, double bac) : base(owner)
	{
		_intensityPerGramMass = intesitypergrammass;
		BloodAlcoholContent = bac;
		IntensityToBonusConversionRate = owner.Gameworld.GetStaticDouble("NauseaIntensityToBonusConversionRate");
	}

	protected override string SpecificEffectType => "Nausea";

	public bool AppliesToCheck(CheckType type)
	{
		switch (type)
		{
			case CheckType.ExactTimeCheck:
			case CheckType.VagueTimeCheck:
			case CheckType.WoundCloseCheck:
			case CheckType.StunRecoveryCheck:
			case CheckType.PainRecoveryCheck:
			case CheckType.ShockRecoveryCheck:
			case CheckType.HealingCheck:
			case CheckType.InfectionHeartbeat:
			case CheckType.InfectionSpread:
				return false;
		}

		return true;
	}

	private double _bloodAlcoholContent;

	public double BloodAlcoholContent
	{
		get => _bloodAlcoholContent;
		set
		{
			var oldValue = _bloodAlcoholContent;
			_bloodAlcoholContent = value;
			IntensitiesChanged(oldValue, _intensityPerGramMass);
		}
	}

	public double CheckBonus => Math.Min(0.0,
		-1 * (IntensityPerGramMass * IntensityToBonusConversionRate + GetIntensityFromBAC(BloodAlcoholContent)));

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Nausea @ {IntensityPerGramMass:N2}, BAC {BloodAlcoholContent} total {_intensityPerGramMass * GetIntensityFromBAC(BloodAlcoholContent) * GetHungerThirstMultiplier}, giving a penalty of {CheckBonus:N3} to skill checks.";
	}

	public double IntensityToBonusConversionRate { get; set; }

	private double _intensityPerGramMass;

	public double GetHungerThirstMultiplier
	{
		get
		{
			var bodyOwner = (IBody)Owner;
			var hungerMultiplier = 0.0;
			switch (bodyOwner.NeedsModel.Status & NeedsResult.HungerOnly)
			{
				case NeedsResult.AbsolutelyStuffed:
					hungerMultiplier = 2.0;
					break;
				case NeedsResult.Full:
					hungerMultiplier = 1.25;
					break;
				case NeedsResult.Peckish:
					hungerMultiplier = 1.0;
					break;
				case NeedsResult.Hungry:
					hungerMultiplier = 0.5;
					break;
				case NeedsResult.Starving:
					hungerMultiplier = 0.25;
					break;
			}

			var thirstMultiplier = 0.0;
			switch (bodyOwner.NeedsModel.Status & NeedsResult.ThirstOnly)
			{
				case NeedsResult.Sated:
					thirstMultiplier = 0.3;
					break;
				case NeedsResult.NotThirsty:
					thirstMultiplier = 0.2;
					break;
				case NeedsResult.Thirsty:
					thirstMultiplier = 0.1;
					break;
				case NeedsResult.Parched:
					thirstMultiplier = 0.0;
					break;
			}

			return hungerMultiplier + thirstMultiplier;
		}
	}

	private string StatusDescription
	{
		get
		{
			var multiplier = GetHungerThirstMultiplier;
			var effectiveValue = _intensityPerGramMass * GetIntensityFromBAC(BloodAlcoholContent) * multiplier;
			if (effectiveValue >= 10.0)
			{
				return "Your nausea is all encompassing; it's all you can think or feel at the moment.";
			}

			if (effectiveValue >= 4.8)
			{
				return "You feel like you are moments away from needing to vomit.";
			}

			if (effectiveValue >= 4.0)
			{
				return
					"You feel like you are losing control of your nausea, things are progressing towards their inevitable conclusion.";
			}

			if (effectiveValue >= 3.25)
			{
				return "Your nausea is getting worse, and feels manifestly unpleasant.";
			}

			if (effectiveValue >= 1.5)
			{
				return "Your nausea is starting to reach fairly unpleasant levels.";
			}

			if (effectiveValue >= 1.0)
			{
				return "You feel a little nauseous.";
			}

			if (effectiveValue >= 0.5)
			{
				return "You begin to feel a little ill at ease.";
			}

			return "";
		}
	}

	public bool ShowInScore => _intensityPerGramMass * GetHungerThirstMultiplier >= 0.5;

	public bool ShowInHealth => true;
	public string ScoreAddendum => StatusDescription.Colour(Telnet.BoldRed);

	public double GetIntensityFromBAC(double bac)
	{
		if (bac < 0.135)
		{
			return 0.0;
		}

		return (bac - 0.135) * 50.0;
	}

	private void IntensitiesChanged(double oldBac, double oldIntensity)
	{
		var multiplier = GetHungerThirstMultiplier;
		var oldEffectiveValue = (oldIntensity + GetIntensityFromBAC(oldBac)) * multiplier;
		var effectiveValue = (_intensityPerGramMass + GetIntensityFromBAC(BloodAlcoholContent)) * multiplier;
		if (effectiveValue >= 10.0 && oldEffectiveValue < 10.0)
		{
			Owner?.OutputHandler?.Send(
				"Your nausea is all encompassing; it's all you can think or feel at the moment.");
		}
		else if (effectiveValue >= 4.8 && oldEffectiveValue < 4.8)
		{
			Owner?.OutputHandler?.Send("You feel like you are moments away from needing to vomit.");
		}
		else if (effectiveValue >= 4.0 && oldEffectiveValue < 4.0)
		{
			Owner?.OutputHandler?.Send(
				"You feel like you are losing control of your nausea, things are progressing towards their inevitable conclusion.");
		}
		else if (effectiveValue >= 3.25 && oldEffectiveValue < 3.25)
		{
			Owner?.OutputHandler?.Send("Your nausea is getting worse, and feels manifestly unpleasant.");
		}
		else if (effectiveValue >= 1.5 && oldEffectiveValue < 1.5)
		{
			Owner?.OutputHandler?.Send("Your nausea is starting to reach fairly unpleasant levels.");
		}
		else if (effectiveValue >= 1.0 && oldEffectiveValue < 1.0)
		{
			Owner?.OutputHandler?.Send("You feel a little nauseous.");
		}
		else if (effectiveValue >= 0.5 && oldEffectiveValue < 0.5)
		{
			Owner?.OutputHandler?.Send("You begin to feel a little ill at ease.");
		}
		else if (effectiveValue < 4.8 && oldEffectiveValue >= 4.8)
		{
			Owner?.OutputHandler?.Send("You no longer feel like you are moments away from needing to vomit.");
		}
		else if (effectiveValue < 4.0 && oldEffectiveValue >= 4.0)
		{
			Owner?.OutputHandler?.Send("Your nausea is back under control, but still manifestly unpleasant.");
		}
		else if (effectiveValue < 3.25 && oldEffectiveValue >= 3.25)
		{
			Owner?.OutputHandler?.Send("Your nausea is starting to lose its edge, but is still fairly unpleasant.");
		}
		else if (effectiveValue < 1.5 && oldEffectiveValue >= 1.5)
		{
			Owner?.OutputHandler?.Send("Your nausea is back down to a level that feels merely unpleasant.");
		}
		else if (effectiveValue < 1.0 && oldEffectiveValue >= 1.0)
		{
			Owner?.OutputHandler?.Send("You no longer feel nauseous per se, merely a little ill at ease.");
		}
		else if (effectiveValue < 0.5 && oldEffectiveValue >= 0.5)
		{
			Owner?.OutputHandler?.Send("You no longer feel ill at ease.");
		}
	}

	public double IntensityPerGramMass
	{
		get => _intensityPerGramMass;
		set
		{
			var oldValue = _intensityPerGramMass;
			_intensityPerGramMass = value;
			IntensitiesChanged(BloodAlcoholContent, oldValue);
		}
	}

	public override void ExpireEffect()
	{
		var bodyOwner = (IBody)Owner;
		if (bodyOwner.Actor.State.HasFlag(CharacterState.Dead) || bodyOwner.Actor.State.HasFlag(CharacterState.Stasis))
		{
			RemovalEffect();
			return;
		}

		bodyOwner.Reschedule(this, TimeSpan.FromSeconds(35));
		var multiplier = GetHungerThirstMultiplier;
		if (IntensityPerGramMass * multiplier < 5.0)
		{
			return;
		}

		if (multiplier > 0.4)
		{
			var prepend = "";
			if (bodyOwner.Actor.PositionState.Upright &&
			    IntensityPerGramMass * multiplier * (int)bodyOwner.CurrentExertion < Dice.Roll(1, 100))
			{
				prepend = "fall|falls to &0's knees and ";
				bodyOwner.SetState(PositionKneeling.Instance);
			}

			var amountMultiplier = 1.0;
			var contents = "the contents of &0's stomach";
			switch (bodyOwner.NeedsModel.Status & NeedsResult.HungerOnly)
			{
				case NeedsResult.AbsolutelyStuffed:
					contents = "all of &0's partially digested recent meal";
					break;
				case NeedsResult.Full:
					contents = "the not-insubstantial contents of &0's stomach";
					break;
				case NeedsResult.Peckish:
					contents = "the contents of &0's stomach";
					amountMultiplier = 0.8;
					break;
				case NeedsResult.Hungry:
					contents = "the meagre contents of &0's stomach";
					amountMultiplier = 0.5;
					break;
				case NeedsResult.Starving:
					switch (bodyOwner.NeedsModel.Status & NeedsResult.ThirstOnly)
					{
						case NeedsResult.Sated:
							contents = "a large amount of liquid";
							break;
						case NeedsResult.NotThirsty:
							contents = "mostly liquid";
							amountMultiplier = 0.8;
							break;
						case NeedsResult.Thirsty:
							contents = "a little liquid mixed with bile";
							amountMultiplier = 0.5;
							break;
						case NeedsResult.Parched:
							contents = "nothing but bile";
							amountMultiplier = 0.1;
							break;
					}

					break;
			}

			var bloodDesc = "";
			var digestiveBlood = bodyOwner.EffectsOfType<IInternalBleedingEffect>()
			                              .Where(x => x.Organ is StomachProto || x.Organ is TracheaProto)
			                              .Sum(x => x.BloodlossTotal);
			if (digestiveBlood > 0.02)
			{
				bloodDesc = "a substantial amount of blood";
			}
			else if (digestiveBlood > 0.01)
			{
				bloodDesc = "a worrying amount of blood";
			}
			else if (digestiveBlood > 0.005)
			{
				bloodDesc = "a substantial amount of blood";
			}
			else if (digestiveBlood > 0.003)
			{
				bloodDesc = "a small amount of blood";
			}

			if (digestiveBlood > 0.001)
			{
				bloodDesc = "a hint of blood";
			}

			foreach (var effect in bodyOwner.EffectsOfType<IInternalBleedingEffect>()
			                                .Where(x => x.Organ is StomachProto || x.Organ is TracheaProto).ToList())
			{
				effect.BloodlossTotal = effect.BloodlossTotal / 2;
			}

			var ejecta = new[] { contents, bloodDesc }.ListToString();

			bodyOwner.Actor.OutputHandler.Handle(new EmoteOutput(
				new Emote($"@ {prepend}vomit|vomits {ejecta} all over the ground.", bodyOwner.Actor, bodyOwner.Actor)));
			bodyOwner.NeedsModel.FulfilNeeds(new NeedFulfiller
			{
				WaterLitres = -0.5 * amountMultiplier,
				ThirstPoints = -2.0 * amountMultiplier,
				Calories = -200 * amountMultiplier,
				SatiationPoints = -4.0 * amountMultiplier
			}, true);
			bodyOwner.RemoveAllEffects(x => x.IsEffectType<DelayedNeedsFulfillment>());
			bodyOwner.SpendStamina(50);
		}
	}
}