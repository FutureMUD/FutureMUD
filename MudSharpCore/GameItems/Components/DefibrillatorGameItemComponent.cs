using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class DefibrillatorGameItemComponent : GameItemComponent, IDefibrillator
{
	protected DefibrillatorGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (DefibrillatorGameItemComponentProto)newProto;
	}

	#region IDefibrillator Implementation

	public bool CanShock(ICharacter shocker, IBody target)
	{
		if (!(Parent.GetItemType<IProducePower>()?.CanDrawdownSpike(_prototype.WattagePerShock) ?? false))
		{
			return false;
		}

		if (CharacterState.Able.HasFlag(target.Actor.State))
		{
			return false;
		}

		var targetParts = target.Bodyparts.Where(x => x.Organs.Any(y => y is HeartProto)).ToList();
		if (targetParts.All(x => target.WornItemsProfilesFor(x).Any(y => y.Item2.PreventsRemoval)))
		{
			return false;
		}

		if (target.Organs.OfType<HeartProto>().Select(x => x.OrganFunctionFactor(target)).DefaultIfEmpty(0).Sum() >=
		    0.3 && !_prototype.CanShockHealthyHearts)
		{
			return false;
		}

		return true;
	}

	public string WhyCannotShock(ICharacter shocker, IBody target)
	{
		if (!(Parent.GetItemType<IProducePower>()?.CanDrawdownSpike(_prototype.WattagePerShock) ?? false))
		{
			return $"{Parent.HowSeen(shocker, true)} does not have enough power to deliver a shock.";
		}

		if (CharacterState.Able.HasFlag(target.Actor.State))
		{
			return $"{target.Actor.HowSeen(shocker, true)} is conscious and therefore not in need of defibrillation.";
		}

		var targetParts = target.Bodyparts.Where(x => x.Organs.Any(y => y is HeartProto)).ToList();
		var preventingRemoval = targetParts.SelectMany(x =>
			target.WornItemsProfilesFor(x).Where(y => y.Item2.PreventsRemoval).Select(y => y.Item1)).ToList();
		if (preventingRemoval.Any())
		{
			return
				$"{preventingRemoval.Select(x => x.HowSeen(shocker)).ListToString().ProperSentences()} {(preventingRemoval.Count == 1 ? "is" : "are")} in the way, you need to get at bare skin to defibrillate.";
		}

		if (target.Organs.OfType<HeartProto>().Select(x => x.OrganFunctionFactor(target)).DefaultIfEmpty(0).Sum() >=
		    0.3 && !_prototype.CanShockHealthyHearts)
		{
			return
				$"{Parent.HowSeen(shocker, true)} is fitted with safety devices that prevent it from being used on healthy hearts.";
		}

		throw new ApplicationException("Unknown WhyCannotShock reason in DefibrillatorGameItemComponent");
	}

	public void Shock(ICharacter shocker, IBody target)
	{
		if (!CanShock(shocker, target))
		{
			shocker.Send(WhyCannotShock(shocker, target));
			return;
		}

		Parent.GetItemType<IProducePower>().DrawdownSpike(_prototype.WattagePerShock);
		shocker.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.DefibrillationEmote, shocker, shocker,
			target.Actor, Parent)));
		var heartFunctions = target.Organs.OfType<HeartProto>()
		                           .Select(x => Tuple.Create(x, x.OrganFunctionFactor(target))).ToList();
		var heartFunction = heartFunctions.Select(x => x.Item2).DefaultIfEmpty(0).Sum();

		// Used on Healthy heart
		if (heartFunction >= 0.3)
		{
			var damageAmount = Gameworld.GetStaticDouble("DefibrillatorBaseDamage") +
			                   (int)Parent.Quality * Gameworld.GetStaticDouble("DefibrillatorDamagePerQuality");
			foreach (var heart in heartFunctions)
			{
				target.SufferDamage(new Damage
				{
					ActorOrigin = shocker,
					ToolOrigin = Parent,
					DamageType = DamageType.Electrical,
					DamageAmount = damageAmount,
					Bodypart = heart.Item1
				});
			}
		}

		var check = Gameworld.GetCheck(CheckType.Defibrillate);
		var result = check.Check(shocker, Difficulty.Normal, target,
			externalBonus: StandardCheck.BonusesPerDifficultyLevel * ((int)Parent.Quality - 5));

		if (RandomUtilities.DoubleRandom(0.0, 1.0) <= (1.0 + result.Outcome.CheckDegrees() * 0.15) *
		    Gameworld.GetStaticDouble(heartFunction <= 0.0
			    ? "DefibrillatorReviveChance"
			    : "DefibrillatorStabiliseChance"))
		{
			foreach (var heart in heartFunctions)
			{
				if (target.EffectsOfType<IStablisedOrganFunction>().All(x => x.Organ != heart.Item1))
				{
					var canBreathe = target.CanBreathe;
					target.AddEffect(new StablisedOrganFunction(target, heart.Item1, 0.3, ExertionLevel.Heavy));
					if (target.NeedsToBreathe && target.CanBreathe && !canBreathe)
					{
						target.OutputHandler.Handle(new EmoteOutput(
							new Emote("@ gasp|gasps suddenly as &0's breathing returns.", target, target)));
					}

					target.CheckHealthStatus();
				}
			}
		}
	}

	public double PowerConsumptionInWatts => 0.0;

	public void OnPowerCutIn()
	{
	}

	public void OnPowerCutOut()
	{
	}

	#endregion

	#region Constructors

	public DefibrillatorGameItemComponent(DefibrillatorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public DefibrillatorGameItemComponent(MudSharp.Models.GameItemComponent component,
		DefibrillatorGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	public DefibrillatorGameItemComponent(DefibrillatorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new DefibrillatorGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion
}