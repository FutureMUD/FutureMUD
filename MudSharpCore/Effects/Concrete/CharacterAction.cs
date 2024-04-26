using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public abstract class CharacterAction : Effect, IActionEffect, ILDescSuffixEffect
{
	public virtual string CancelEmoteString { get; init; }
	public virtual string WhyCannotMoveEmoteString { get; init; }
	public Action<IPerceivable> OnStopAction { get; set; }
	public ICharacter CharacterOwner { get; init; }
	public string ActionDescription { get; init; }
	public string LDescAddendum { get; init; }
	public Action<IPerceivable> Action { get; init; }

	#region Constructors

	protected CharacterAction(ICharacter owner, Action<IPerceivable> action,
		string actionDescription, string cancelEmote, string cannotMoveEmote, string block, string ldescAddendum,
		Action<IPerceivable> onStopAction = null)
		: base(owner)
	{
		CharacterOwner = owner;
		Action = action;
		ActionDescription = actionDescription;
		_blocks.Add(block);
		OnStopAction = onStopAction;
		CancelEmoteString = cancelEmote;
		WhyCannotMoveEmoteString = cannotMoveEmote;
		LDescAddendum = ldescAddendum;
	}

	protected CharacterAction(ICharacter owner, Action<IPerceivable> action,
		string actionDescription, string cancelEmote, string cannotMoveEmote, IEnumerable<string> blocks,
		string ldescAddendum,
		Action<IPerceivable> onStopAction = null)
		: base(owner)
	{
		Action = action;
		ActionDescription = actionDescription;
		_blocks.AddRange(blocks);
		OnStopAction = onStopAction;
		CancelEmoteString = cancelEmote;
		WhyCannotMoveEmoteString = cannotMoveEmote;
		CharacterOwner = owner;
		LDescAddendum = ldescAddendum;
	}

	protected CharacterAction(ICharacter owner) : base(owner)
	{
		CharacterOwner = owner;
	}

	#endregion

	public override bool CanBeStoppedByPlayer => true;

	public bool SuffixApplies()
	{
		return !string.IsNullOrEmpty(LDescAddendum);
	}

	public override void ExpireEffect()
	{
		Owner.RemoveEffect(this);
		if ((bool?)ApplicabilityProg?.Execute(Owner, null, null) ?? true)
		{
			Action(Owner);
		}

		ReleaseEventHandlers();
	}

	public override void RemovalEffect()
	{
		OnStopAction?.Invoke(Owner);
		ReleaseEventHandlers();
	}

	protected readonly List<string> _blocks = new();

	public override IEnumerable<string> Blocks => _blocks;

	public override bool IsBlockingEffect(string blockingType)
	{
		return string.IsNullOrEmpty(blockingType) || _blocks.Contains(blockingType);
	}

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return new EmoteOutput(new Emote(ActionDescription, CharacterOwner, CharacterOwner),
			style: OutputStyle.TextFragment).ParseFor(voyeur);
	}

	public override void Login()
	{
		SetupEventHandlers();
	}

	protected virtual void SetupEventHandlers()
	{
		CharacterOwner.OnDeleted += TargetDeleted;
		CharacterOwner.OnQuit += TargetQuit;
		CharacterOwner.OnDeath += TargetDied;
		CharacterOwner.OnEngagedInMelee += TargetEngagedInMelee;
		CharacterOwner.OnStateChanged += TargetStateChanged;
		CharacterOwner.OnStartMove += TargetMoved;
		CharacterOwner.OnMoved += TargetMoved;
		CharacterOwner.OnWantsToMove += TargetWantsToMove;
	}

	protected virtual void ReleaseEventHandlers()
	{
		CharacterOwner.OnDeleted -= TargetDeleted;
		CharacterOwner.OnQuit -= TargetQuit;
		CharacterOwner.OnDeath -= TargetDied;
		CharacterOwner.OnEngagedInMelee -= TargetEngagedInMelee;
		CharacterOwner.OnStateChanged -= TargetStateChanged;
		CharacterOwner.OnStartMove -= TargetMoved;
		CharacterOwner.OnMoved -= TargetMoved;
		CharacterOwner.OnWantsToMove -= TargetWantsToMove;
	}

	protected virtual EmoteOutput GetCancelEmote(string emoteString)
	{
		return new EmoteOutput(new Emote(emoteString, CharacterOwner, CharacterOwner));
	}

	protected virtual EmoteOutput GetWhyCannotMoveEmote(IMove mover)
	{
		return new EmoteOutput(new Emote(WhyCannotMoveEmoteString, mover, Owner));
	}

	public override string ToString()
	{
		return Describe(CharacterOwner);
	}

	protected virtual EmoteOutput GetLDescSuffixEmote()
	{
		return new EmoteOutput(new Emote(LDescAddendum, CharacterOwner, CharacterOwner));
	}

	public string SuffixFor(IPerceiver voyeur)
	{
		return GetLDescSuffixEmote().ParseFor(voyeur).ToLowerInvariant();
	}

	#region Event Handlers

	protected virtual void TargetDeleted(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because #0 are|is no longer there."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected virtual void TargetQuit(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because #0 are|is no longer there."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected virtual void TargetDied(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote($"{CancelEmoteString} because #0 have|has died."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected virtual void TargetMoved(object sender, MoveEventArgs args)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because #0 are|is no longer there."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected virtual void TargetWantsToMove(IPerceivable perceivable, PerceivableRejectionResponse response)
	{
		response.Reason =
			GetWhyCannotMoveEmote(CharacterOwner)
				.ParseFor(CharacterOwner);
		response.Rejected = true;
	}

	protected virtual void TargetEngagedInMelee(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because #0 have|has been engaged in melee!"));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected virtual void TargetStateChanged(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"{CancelEmoteString} because #0 are|is now {CharacterOwner.State.Describe()}."));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	#endregion
}