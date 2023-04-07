using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class BlockingDelayedAction : Effect, IActionEffect, ILDescSuffixEffect, IRemoveOnStateChange
{
	private readonly List<string> _blocks = new();
	public ICharacter CharacterOwner { get; set; }

	public BlockingDelayedAction(ICharacter owner, Action<IPerceivable> action, string actionDescription,
		string block, string ldescAddendum, Action<IPerceivable> onStopAction = null)
		: base(owner)
	{
		Action = action;
		ActionDescription = actionDescription;
		_blocks.Add(block);
		OnStopAction = onStopAction;
		LDescAddendumEmote = ldescAddendum;
		CharacterOwner = owner;
	}

	public BlockingDelayedAction(ICharacter owner, Action<IPerceivable> action, string actionDescription,
		IEnumerable<string> blocks, string ldescAddendum, Action<IPerceivable> onStopAction = null)
		: base(owner)
	{
		Action = action;
		ActionDescription = actionDescription;
		_blocks.AddRange(blocks);
		LDescAddendumEmote = ldescAddendum;
		CharacterOwner = owner;
		OnStopAction = onStopAction;
	}

	public Action<IPerceivable> OnStopAction { get; set; }

	protected override string SpecificEffectType => "BlockingDelayedAction";

	public string ActionDescription { get; set; }
	public string LDescAddendumEmote { get; set; }
	public Action<IPerceivable> Action { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Blocking Delayed Action - {ActionDescription}";
	}

	public override bool CanBeStoppedByPlayer => true;

	public override void ExpireEffect()
	{
		if ((bool?)ApplicabilityProg?.Execute(Owner, null, null) ?? true)
		{
			Action(Owner);
		}

		Owner.RemoveEffect(this);
	}

	#region Overrides of Effect

	/// <summary>
	///     Fires when an effect is removed, including a matured scheduled effect
	/// </summary>
	public override void RemovalEffect()
	{
		OnStopAction?.Invoke(Owner);
	}

	#endregion

	public override IEnumerable<string> Blocks => _blocks;

	public override bool IsBlockingEffect(string blockingType)
	{
		return string.IsNullOrEmpty(blockingType) || _blocks.Contains(blockingType);
	}

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return ActionDescription;
	}

	public override string ToString()
	{
		return $"BlockingDelayedAction Effect ({ActionDescription})";
	}

	public virtual string SuffixFor(IPerceiver voyeur)
	{
		return new EmoteOutput(new Emote(LDescAddendumEmote, CharacterOwner, CharacterOwner)).ParseFor(voyeur)
			.ToLowerInvariant();
	}

	public bool SuffixApplies()
	{
		return !string.IsNullOrEmpty(LDescAddendumEmote);
	}

	public bool ShouldRemove(CharacterState newState)
	{
		return newState.HasFlag(CharacterState.Dead) || !CharacterState.Able.HasFlag(newState);
	}
}