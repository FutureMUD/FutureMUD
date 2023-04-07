using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class Grappling : Effect, IGrappling
{
	public override bool CanBeStoppedByPlayer => true;

	public override IEnumerable<string> Blocks => new[] { "general", "combat-engage" };

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return $"grappling with {Target.HowSeen(CharacterOwner, colour: false)}";
	}

	public override bool IsBlockingEffect(string blockingType)
	{
		return Blocks.Any(x => x.EqualTo(blockingType));
	}

	public ICharacter CharacterOwner { get; set; }
	public ICharacter Target { get; set; }
	public IBeingGrappled TargetEffect { get; set; }

	public Grappling(ICharacter owner, ICharacter target) : base(owner)
	{
		CharacterOwner = owner;
		Target = target;
		RegisterEvents();
		TargetEffect = new BeingGrappled(target, this);
		Target.Body.AddEffect(TargetEffect);
	}

	protected void RegisterEvents()
	{
		CharacterOwner.OnDeath += Grapple_NoLongerValid;
		CharacterOwner.OnDeleted += Grapple_NoLongerValid;
		CharacterOwner.OnQuit += Grapple_NoLongerValid;
		CharacterOwner.OnStateChanged += CharacterOwner_OnStateChanged;
		CharacterOwner.OnWantsToMove += CharacterOwner_OnWantsToMove;
		CharacterOwner.OnLeaveCombat += CharacterOwner_OnLeaveCombat;
		CharacterOwner.OnJoinCombat += CharacterOwner_OnJoinCombat;
		CharacterOwner.OnPositionChanged += CharacterOwner_OnPositionChanged;
		Target.OnJoinCombat += Target_OnJoinCombat;
		Target.OnDeath += Grapple_NoLongerValid;
		Target.OnDeleted += Grapple_NoLongerValid;
		Target.OnQuit += Grapple_NoLongerValid;
		Target.OnWantsToMove += Target_OnWantsToMove;
		Target.OnLeaveCombat += Target_OnLeaveCombat;
		Target.OnPositionChanged += Target_OnPositionChanged;
	}

	private void CharacterOwner_OnPositionChanged(IPerceivable owner)
	{
		if (!CharacterOwner.PositionState.Upright && Target.PositionState.Upright)
		{
			if (!LimbsUnderControl.Any())
			{
				CharacterOwner.OutputHandler.Handle(new EmoteOutput(
					new Emote(
						$"@ release|releases $1 from &0's grapple as #0 %0|are|is now {CharacterOwner.PositionState.DescribeLocationMovementParticiple.ToLowerInvariant()}.",
						CharacterOwner, CharacterOwner, Target), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
				RemoveAndCleanup();
			}
		}
	}

	private void Target_OnPositionChanged(IPerceivable owner)
	{
		if (CharacterOwner.PositionState.Upright && !Target.PositionState.Upright)
		{
			CharacterOwner.PositionState = PositionKneeling.Instance;
		}
	}

	private void Target_OnJoinCombat(IPerceivable owner)
	{
		if (CharacterOwner.Combat == null)
		{
			Target.Combat.JoinCombat(CharacterOwner);
		}
	}

	private void CharacterOwner_OnJoinCombat(IPerceivable owner)
	{
		if (Target.Combat == null)
		{
			CharacterOwner.Combat.JoinCombat(Target);
		}
	}

	private void Target_OnLeaveCombat(IPerceivable owner)
	{
		if (CharacterOwner.Combat == Target.Combat && CharacterOwner.Combat.Friendly)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ release|releases $1 from &0's grapple as the combat ends.", CharacterOwner, CharacterOwner,
					Target), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			RemoveAndCleanup();
		}
	}

	private void CharacterOwner_OnLeaveCombat(IPerceivable owner)
	{
		if (CharacterOwner.Combat.Friendly)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ release|releases $1 from &0's grapple as the combat ends.", CharacterOwner, CharacterOwner,
					Target), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			RemoveAndCleanup();
		}
	}

	protected void ReleaseEvents()
	{
		CharacterOwner.OnDeath -= Grapple_NoLongerValid;
		CharacterOwner.OnDeleted -= Grapple_NoLongerValid;
		CharacterOwner.OnQuit -= Grapple_NoLongerValid;
		CharacterOwner.OnStateChanged -= CharacterOwner_OnStateChanged;
		CharacterOwner.OnWantsToMove -= CharacterOwner_OnWantsToMove;
		CharacterOwner.OnLeaveCombat -= CharacterOwner_OnLeaveCombat;
		CharacterOwner.OnJoinCombat -= CharacterOwner_OnJoinCombat;
		CharacterOwner.OnPositionChanged -= CharacterOwner_OnPositionChanged;
		Target.OnJoinCombat -= Target_OnJoinCombat;
		Target.OnDeath -= Grapple_NoLongerValid;
		Target.OnDeleted -= Grapple_NoLongerValid;
		Target.OnQuit -= Grapple_NoLongerValid;
		Target.OnWantsToMove -= Target_OnWantsToMove;
		Target.OnLeaveCombat -= Target_OnLeaveCombat;
		Target.OnPositionChanged -= Target_OnPositionChanged;
	}

	private void Target_OnWantsToMove(IPerceivable owner, PerceivableRejectionResponse response)
	{
		response.Rejected = true;
		response.Reason = "You can't move while you are being grappled. You must struggle free first.";
	}

	private void CharacterOwner_OnWantsToMove(IPerceivable owner, PerceivableRejectionResponse response)
	{
		if (!TargetEffect.UnderControl)
		{
			response.Rejected = true;
			response.Reason = $"You can't move until you have your grapple target fully under control.";
		}

		response.Rejected = true;
		response.Reason = $"You can't move when grappling someone, you must convert your movement to a drag.";
	}

	private void CharacterOwner_OnStateChanged(IPerceivable owner)
	{
		if (!CharacterState.Able.HasFlag(CharacterOwner.State))
		{
			RemoveAndCleanup();
		}
	}

	private void Grapple_NoLongerValid(IPerceivable owner)
	{
		RemoveAndCleanup();
	}

	private void RemoveAndCleanup()
	{
		ReleaseEvents();
		Target.Body.RemoveEffect(TargetEffect);
		CharacterOwner.RemoveEffect(this);
	}

	public override void RemovalEffect()
	{
		ReleaseEvents();
		Target.Body.RemoveEffect(TargetEffect, true);
	}

	protected override string SpecificEffectType => "Grappling";

	private readonly List<ILimb> _limbsUnderControl = new();
	public IEnumerable<ILimb> LimbsUnderControl => _limbsUnderControl;

	public override string Describe(IPerceiver voyeur)
	{
		return $"Grappling with {Target.HowSeen(voyeur)}.";
	}

	public (bool StillGrappled, IEnumerable<ILimb> FreedLimbs) StruggleResult(OpposedOutcomeDegree degree)
	{
		var limbsFree = (int)degree + 1;
		if (limbsFree > _limbsUnderControl.Count)
		{
			var result = (false, LimbsUnderControl.ToList());
			_limbsUnderControl.Clear();
			return result;
		}

		var limbs = _limbsUnderControl.PickUpToRandom(limbsFree);
		_limbsUnderControl.RemoveAll(x => limbs.Contains(x));
		return (true, limbs);
	}

	public void AddLimb(ILimb limb)
	{
		if (_limbsUnderControl.Contains(limb))
		{
			return;
		}

		var wasInControl = TargetEffect.UnderControl;
		_limbsUnderControl.Add(limb);

		if (!wasInControl && TargetEffect.UnderControl)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is now completely in control of $1!",
				CharacterOwner, CharacterOwner, Target)));
			Target.CombatTarget = null;

			if (CharacterOwner.Combat != null &&
			    CharacterOwner.CombatStrategyMode == Combat.CombatStrategyMode.GrappleForControl)
			{
				CharacterOwner.CombatTarget = null;
				if (CharacterOwner.Combat.CanFreelyLeaveCombat(CharacterOwner) &&
				    CharacterOwner.Combat.CanFreelyLeaveCombat(Target))
				{
					var combat = CharacterOwner.Combat;
					if (!combat.LeaveCombat(CharacterOwner))
					{
						combat.LeaveCombat(Target);
					}
				}
			}
		}

		Target.Body.ReevaluateLimbAndPartDamageEffects();
	}

	public string SuffixFor(IPerceiver voyeur)
	{
		if (TargetEffect.UnderControl)
		{
			return $"holding {Target.HowSeen(voyeur)} subdued";
		}

		return $"grappling with {Target.HowSeen(voyeur)}";
	}

	public bool SuffixApplies()
	{
		return true;
	}
}