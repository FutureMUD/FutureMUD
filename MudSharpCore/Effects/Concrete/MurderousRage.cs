using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class MurderousRage : Effect, IRageEffect, IScoreAddendumEffect
{
	public ICharacter CharacterOwner { get; set; }
	public IBody BodyOwner { get; set; }
	public bool IsRaging => IntensityPerGramMass >= 5.0;
	public bool IsSuperRaging => IntensityPerGramMass >= 10.0;

	private string StatusDescription
	{
		get
		{
			if (_intensityPerGramMass >= 10.0)
			{
				return "Nobody and nothing is safe from your rage, you see the world only in red!";
			}

			if (_intensityPerGramMass >= 5.0)
			{
				return "You cannot keep your rage contained any longer, you have to destroy something!";
			}

			if (_intensityPerGramMass >= 3.0)
			{
				return
					"You can barely contain the rage that is building up inside you, and you feel a deep need to lash out at something; anything to relieve this tension.";
			}

			if (_intensityPerGramMass >= 1.5)
			{
				return "You feel a churning rage building up inside you, threatening to spill over.";
			}

			if (_intensityPerGramMass >= 1.0)
			{
				return "You feel irriated, like everything around you is bothering you.";
			}

			if (_intensityPerGramMass >= 0.5)
			{
				return "You feel a little anxious; restless and distracted.";
			}

			return "";
		}
	}

	public bool ShowInScore => _intensityPerGramMass >= 0.5;

	public bool ShowInHealth => true;

	public string ScoreAddendum => StatusDescription.Colour(Telnet.BoldRed);

	private double _intensityPerGramMass;

	public double IntensityPerGramMass
	{
		get => _intensityPerGramMass;
		set
		{
			if (value >= 10.0 && _intensityPerGramMass < 10.0)
			{
				Owner?.OutputHandler?.Send("Nobody and nothing is safe from your rage, you see the world only in red!");
			}
			else if (value >= 5.0 && _intensityPerGramMass < 5.0)
			{
				Owner?.OutputHandler?.Send(
					"You cannot keep your rage contained any longer, you have to destroy something!");
				Owner?.OutputHandler.Handle(new EmoteOutput(
					new Emote("@ lets out a cry of undeniable rage, red-faced and shaking.", CharacterOwner,
						CharacterOwner), flags: OutputFlags.SuppressSource));
			}
			else if (value >= 3.0 && _intensityPerGramMass < 3.0)
			{
				Owner?.OutputHandler?.Send(
					"You can barely contain the rage that is building up inside you, and you feel a deep need to lash out at something; anything to relieve this tension.");
				Owner?.OutputHandler.Handle(new EmoteOutput(
					new Emote("@ is breathing heavily and angrily, nostrils flaring with each breath.", CharacterOwner,
						CharacterOwner), flags: OutputFlags.SuppressSource));
			}
			else if (value >= 1.5 && _intensityPerGramMass < 1.5)
			{
				Owner?.OutputHandler?.Send(
					"You feel a churning rage building up inside you, threatening to spill over.");
				Owner?.OutputHandler.Handle(new EmoteOutput(
					new Emote(
						"@ is developing a noticeable tic, the corner of &0's eye and mouth on one side of &0's face regularly twitching.",
						CharacterOwner, CharacterOwner), flags: OutputFlags.SuppressSource));
			}
			else if (value >= 1.0 && _intensityPerGramMass < 1.0)
			{
				Owner?.OutputHandler?.Send("You feel irritated, like everything around you is bothering you.");
				Owner?.OutputHandler.Handle(new EmoteOutput(
					new Emote("The corner of $0's mouth twitches almost imperceptibly, but you manage to notice it.",
						CharacterOwner, CharacterOwner),
					flags: OutputFlags.NoticeCheckRequired | OutputFlags.SuppressSource));
			}
			else if (value >= 0.5 && _intensityPerGramMass < 0.5)
			{
				Owner?.OutputHandler?.Send("You feel a little anxious; restless and distracted.");
			}
			else if (value < 5.0 && _intensityPerGramMass >= 5.0)
			{
				Owner?.OutputHandler?.Send(
					"Your rage is still unleashed, but you feel like you are at least in the driver's seat in your own mind again.");
			}
			else if (value < 5.0 && _intensityPerGramMass >= 5.0)
			{
				Owner?.OutputHandler?.Send(
					"You feel like you have a lid on your rage again, but it is only just below the surface.");
			}
			else if (value < 1.5 && _intensityPerGramMass >= 1.5)
			{
				Owner?.OutputHandler?.Send("You no longer feel quite as much rage, but are still quite irritated.");
			}
			else if (value < 1.0 && _intensityPerGramMass >= 1.0)
			{
				Owner?.OutputHandler?.Send("You no longer feel irritated, merely anxious, restless and distracted.");
			}
			else if (value < 0.5 && _intensityPerGramMass >= 0.5)
			{
				Owner?.OutputHandler?.Send("You no longer feel anxious, restless and distracted.");
			}

			_intensityPerGramMass = value;
		}
	}

	public MurderousRage(IBody owner, double intensity) : base(owner)
	{
		_intensityPerGramMass = intensity;
		BodyOwner = owner;
		CharacterOwner = owner.Actor;
	}

	protected override string SpecificEffectType => "MurderousRage";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Has an intensity of {_intensityPerGramMass:N2} of murderous rage.";
	}

	public override void ExpireEffect()
	{
		BodyOwner.Reschedule(this, TimeSpan.FromSeconds(7));
		if (IntensityPerGramMass < 5.0)
		{
			return;
		}

		if (!CharacterState.Able.HasFlag(CharacterOwner.State))
		{
			return;
		}

		if (CharacterOwner.IsHelpless)
		{
			CharacterOwner.OutOfContextExecuteCommand("struggle");
			return;
		}

		if (CharacterOwner.Combat != null && CharacterOwner.CombatTarget != null)
		{
			if (CharacterOwner.MeleeRange && !CharacterOwner.CombatStrategyMode.IsMeleeDesiredStrategy() &&
			    IntensityPerGramMass >= 10.0 && CharacterOwner.CombatTarget is ICharacter)
			{
				CharacterOwner.Send(
					"You can no longer stand to be so far away from your target, you must destroy them from closer!");
				CharacterOwner.TakeOrQueueCombatAction(
					SelectedCombatAction.GetEffectCharge(CharacterOwner, (ICharacter)CharacterOwner.CombatTarget));
				return;
			}

			return;
		}

		var potentialTargets = CharacterOwner.Combat?.Combatants.Except(CharacterOwner).OfType<ICharacter>().Where(x =>
			(IntensityPerGramMass >= 10.0 || !CharacterOwner.IsAlly(x)) && CharacterOwner.CanEngage(x) &&
			CharacterOwner.CanSee(x)).ToList();
		if (potentialTargets?.Any() == true)
		{
			var target = potentialTargets.GetRandomElement();
			CharacterOwner.Send($"Your seething rage compels you to attack {target.HowSeen(CharacterOwner)}!");
			CharacterOwner.Engage(target);
			return;
		}

		var roomTargets = CharacterOwner.Location.Characters.Except(CharacterOwner).Where(x =>
			(IntensityPerGramMass >= 10.0 || !CharacterOwner.IsAlly(x)) && CharacterOwner.CanEngage(x) &&
			CharacterOwner.CanSee(x)).ToList();
		if (roomTargets.Any())
		{
			var target = roomTargets.GetRandomElement();
			CharacterOwner.Send($"Your seething rage compels you to attack {target.HowSeen(CharacterOwner)}!");
			CharacterOwner.Engage(target);
			return;
		}

		var weapon = BodyOwner.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>()).FirstOrDefault(x =>
			x.WeaponType.UsableAttacks(CharacterOwner, x.Parent, null, x.HandednessForWeapon(CharacterOwner), false,
				BuiltInCombatMoveType.MeleeWeaponSmashItem).Any());
		var roomItems = CharacterOwner.Location.LayerGameItems(CharacterOwner.RoomLayer)
		                              .Where(x => x.IsItemType<IDestroyable>() && CharacterOwner.CanSee(x)).ToList();
		var doorItems = CharacterOwner.Location.ExitsFor(CharacterOwner).SelectNotNull(x => x.Exit.Door?.Parent)
		                              .Where(x => x.IsItemType<IDestroyable>() && CharacterOwner.CanSee(x)).ToList();
		if (roomItems.Any() || doorItems.Any())
		{
			var target = roomItems.GetRandomElement() ?? doorItems.GetRandomElement();
			CharacterOwner.Send(
				$"Without a suitable outlet for your rage, you feel compelled to smash up {target.HowSeen(CharacterOwner)}!");
			var attack = weapon?.WeaponType
			                   .UsableAttacks(CharacterOwner, weapon.Parent, null,
				                   weapon.HandednessForWeapon(CharacterOwner), false,
				                   BuiltInCombatMoveType.MeleeWeaponSmashItem)
			                   .Where(x => CharacterOwner.CanSpendStamina(x.StaminaCost))
			                   .GetWeightedRandom(x => x.Weighting);
			if (attack != null)
			{
				CharacterOwner.TakeOrQueueCombatAction(
					SelectedCombatAction.GetEffectSmashItem(CharacterOwner, target, null, weapon, attack));
				return;
			}

			var uattack = CharacterOwner.Race
			                            .UsableNaturalWeaponAttacks(CharacterOwner, target, false,
				                            BuiltInCombatMoveType.UnarmedSmashItem)
			                            .Where(x => CharacterOwner.CanSpendStamina(x.Attack.StaminaCost))
			                            .GetWeightedRandom(x => x.Attack.Weighting);
			if (uattack != null)
			{
				CharacterOwner.TakeOrQueueCombatAction(
					SelectedCombatAction.GetEffectSmashItemUnarmed(CharacterOwner, target, null, uattack));
				return;
			}
		}
	}
}