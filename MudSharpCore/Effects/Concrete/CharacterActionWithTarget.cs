using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class CharacterActionWithTarget : CharacterAction
{
	public CharacterActionWithTarget(ICharacter owner, IPerceivable target, Action<IPerceivable> action,
		string actionDescription, string cancelEmote, string cannotMoveEmote, string block, string ldescAddendum,
		Action<IPerceivable> onStopAction = null)
		: base(owner, action, actionDescription, cancelEmote, cannotMoveEmote, block, ldescAddendum, onStopAction)
	{
		Target = target;
		TargetMortal = target as IMortal;
		TargetCombatant = target as ICombatant;
		TargetMover = target as IMove;
		TargetCharacter = target as ICharacter;
		SetupEventHandlers();
	}

	public CharacterActionWithTarget(ICharacter owner, IPerceivable target, Action<IPerceivable> action,
		string actionDescription, string cancelEmote, string cannotMoveEmote, IEnumerable<string> blocks,
		string ldescAddendum,
		Action<IPerceivable> onStopAction = null)
		: base(owner, action, actionDescription, cancelEmote, cannotMoveEmote, blocks, ldescAddendum, onStopAction)
	{
		Target = target;
		TargetMortal = target as IMortal;
		TargetCombatant = target as ICombatant;
		TargetMover = target as IMove;
		TargetCharacter = target as ICharacter;
		SetupEventHandlers();
	}

	protected CharacterActionWithTarget(ICharacter owner, IPerceivable target) : base(owner)
	{
		CharacterOwner = owner;
		Target = target;
		TargetMortal = target as IMortal;
		TargetCombatant = target as ICombatant;
		TargetMover = target as IMove;
		TargetCharacter = target as ICharacter;
		TargetItem = target as IGameItem;
		SetupEventHandlers();
	}

	public IPerceivable Target { get; set; }
	public IMortal TargetMortal { get; set; }
	public ICombatant TargetCombatant { get; set; }
	public IMove TargetMover { get; set; }
	public ICharacter TargetCharacter { get; set; }
	public IGameItem TargetItem { get; set; }

	protected virtual bool PreventsTargetFromMoving => false;

	protected override string SpecificEffectType => "CharacterActionWithTarget";

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return new EmoteOutput(new Emote(ActionDescription, CharacterOwner, CharacterOwner, Target),
			style: OutputStyle.TextFragment).ParseFor(voyeur);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Character Action With Target for {Target.HowSeen(voyeur)} - {ActionDescription}";
	}

	protected override void SetupEventHandlers()
	{
		Target.OnDeleted -= TargetDeleted;
		Target.OnDeleted += TargetDeleted;
		Target.OnQuit -= TargetQuit;
		Target.OnQuit += TargetQuit;
		if (TargetMortal != null)
		{
			TargetMortal.OnDeath -= TargetDied;
			TargetMortal.OnDeath += TargetDied;
		}

		if (TargetCombatant != null)
		{
			TargetCombatant.OnEngagedInMelee -= TargetEngagedInMelee;
			TargetCombatant.OnEngagedInMelee += TargetEngagedInMelee;
		}

		if (TargetMover != null)
		{
			TargetMover.OnStartMove -= TargetMoved;
			TargetMover.OnStartMove += TargetMoved;
			TargetMover.OnMoved -= TargetMoved;
			TargetMover.OnMoved += TargetMoved;
			if (PreventsTargetFromMoving)
			{
				TargetMover.OnWantsToMove -= TargetWantsToMove;
				TargetMover.OnWantsToMove += TargetWantsToMove;
			}
		}

		if (TargetCharacter != null)
		{
			TargetCharacter.OnStateChanged -= TargetStateChanged;
			TargetCharacter.OnStateChanged += TargetStateChanged;
		}

		if (TargetItem != null)
		{
			TargetItem.OnRemovedFromLocation -= TargetRemovedFromLocation;
			TargetItem.OnRemovedFromLocation += TargetRemovedFromLocation;
			TargetItem.OnInventoryChange -= TargetInventoryChanged;
			TargetItem.OnInventoryChange += TargetInventoryChanged;
		}

		base.SetupEventHandlers();
	}

	protected override void ReleaseEventHandlers()
	{
		Target.OnDeleted -= TargetDeleted;
		Target.OnQuit -= TargetQuit;
		if (TargetMortal != null)
		{
			TargetMortal.OnDeath -= TargetDied;
		}

		if (TargetCombatant != null)
		{
			TargetCombatant.OnEngagedInMelee -= TargetEngagedInMelee;
		}

		if (TargetMover != null)
		{
			TargetMover.OnMoved -= TargetMoved;
			TargetMover.OnStartMove -= TargetMoved;
			TargetMover.OnWantsToMove -= TargetWantsToMove;
		}

		if (TargetCharacter != null)
		{
			TargetCharacter.OnStateChanged -= TargetStateChanged;
		}

		if (TargetItem != null)
		{
			TargetItem.OnRemovedFromLocation -= TargetRemovedFromLocation;
			TargetItem.OnInventoryChange -= TargetInventoryChanged;
		}

		base.ReleaseEventHandlers();
	}

	protected override EmoteOutput GetCancelEmote(string emoteString)
	{
		return new EmoteOutput(new Emote(emoteString, CharacterOwner, CharacterOwner, Target));
	}

	protected override EmoteOutput GetWhyCannotMoveEmote(IMove mover)
	{
		return new EmoteOutput(new Emote(WhyCannotMoveEmoteString, mover, Owner, Target));
	}

	protected override EmoteOutput GetLDescSuffixEmote()
	{
		return new EmoteOutput(new Emote(LDescAddendum, CharacterOwner, CharacterOwner, Target));
	}

	#region Event Handlers

	private void TargetInventoryChanged(InventoryState oldState, InventoryState newState, IGameItem item)
	{
		if (oldState.In(InventoryState.Held, InventoryState.Wielded) &&
		    newState.In(InventoryState.Held, InventoryState.Wielded))
		{
			return;
		}

		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because $1 $1|are|is no longer where it should be."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected override void TargetDeleted(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because {(perceivable == Target ? "$1" : "#0")} {(perceivable == Target ? "$1|" : "%0|")}are|is no longer there."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected override void TargetQuit(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because {(perceivable == Target ? "$1" : "#0")} {(perceivable == Target ? "$1|" : "%0|")}are|is no longer there."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected virtual void TargetRemovedFromLocation(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because {(perceivable == Target ? "$1" : "#0")} {(perceivable == Target ? "$1|" : "%0|")}are|is no longer there."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected override void TargetDied(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because {(perceivable == Target ? "$1" : "#0")} {(perceivable == Target ? "$1|" : "%0|")}have|has died."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected override void TargetMoved(object sender, MoveEventArgs args)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because {(args.Mover == Target ? "$1" : "#0")} {(args.Mover == Target ? "$1|" : "%0|")}are|is no longer there."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected override void TargetWantsToMove(IPerceivable perceivable, PerceivableRejectionResponse response)
	{
		response.Reason =
			GetWhyCannotMoveEmote(perceivable == Owner ? CharacterOwner : TargetMover)
				.ParseFor(perceivable == Owner ? CharacterOwner : TargetMover);
		OnStopAction = null;
		response.Rejected = true;
	}

	protected override void TargetEngagedInMelee(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because {(perceivable == Target ? "$1" : "#0")} {(perceivable == Target ? "$1|" : "%0|")}have|has been engaged in melee!"));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected void TargetStateChanged(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because {(perceivable == Target ? "$1" : "#0")} {(perceivable == Target ? "$1|" : "%0|")}are|is now {TargetCharacter.State.Describe()}."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	#endregion
}