#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class ItemComponentConfigurationAction : CharacterActionWithTargetAndTool
{
	private readonly Func<CheckOutcome> _checkAction;
	private readonly Func<CheckOutcome, bool> _successAction;
	private readonly Action<CheckOutcome>? _failureAction;
	private readonly Action<CheckOutcome>? _abjectFailureAction;
	private readonly string _beginEmote;
	private readonly string _continueEmote;
	private readonly string _successEmote;
	private readonly string _failureEmote;
	private readonly TimeSpan _stageDuration;
	private readonly int _totalStages;

	public ItemComponentConfigurationAction(
		ICharacter owner,
		IGameItem target,
		IGameItem tool,
		IInventoryPlan inventoryPlan,
		string actionDescription,
		string beginEmote,
		string continueEmote,
		string cancelEmote,
		string successEmote,
		string failureEmote,
		TimeSpan stageDuration,
		int totalStages,
		Func<CheckOutcome> checkAction,
		Func<CheckOutcome, bool> successAction,
		Action<CheckOutcome>? failureAction = null,
		Action<CheckOutcome>? abjectFailureAction = null)
		: base(owner, target, [(tool, DesiredItemState.Held)])
	{
		InventoryPlan = inventoryPlan;
		Tool = tool;
		_checkAction = checkAction;
		_successAction = successAction;
		_failureAction = failureAction;
		_abjectFailureAction = abjectFailureAction;
		_beginEmote = beginEmote;
		_continueEmote = continueEmote;
		_successEmote = successEmote;
		_failureEmote = failureEmote;
		_stageDuration = stageDuration;
		_totalStages = Math.Max(1, totalStages);
		ActionDescription = actionDescription;
		CancelEmoteString = cancelEmote;
		WhyCannotMoveEmoteString = "@ cannot move because #0 are|is working on $1.";
		LDescAddendum = "working on $1";
		_blocks.Add("general");
		_blocks.Add("movement");
	}

	public IInventoryPlan InventoryPlan { get; }
	public IGameItem Tool { get; }
	public int CurrentStage { get; private set; }

	protected override string SpecificEffectType => "ItemComponentConfigurationAction";

	public override string Describe(IPerceiver voyeur)
	{
		return $"{ActionDescription} on {Target.HowSeen(voyeur)}";
	}

	public override void InitialEffect()
	{
		if (string.IsNullOrWhiteSpace(_beginEmote))
		{
			return;
		}

		CharacterOwner.OutputHandler.Handle(
			new EmoteOutput(new Emote(_beginEmote, CharacterOwner, Target, Tool), flags: OutputFlags.SuppressObscured));
	}

	public override void ExpireEffect()
	{
		CurrentStage++;
		if (CurrentStage < _totalStages)
		{
			if (!string.IsNullOrWhiteSpace(_continueEmote))
			{
				CharacterOwner.OutputHandler.Handle(
					new EmoteOutput(new Emote(_continueEmote, CharacterOwner, Target, Tool),
						flags: OutputFlags.SuppressObscured));
			}

			Owner.Reschedule(this, _stageDuration);
			return;
		}

		var outcome = _checkAction();
		if (outcome.IsPass() && _successAction(outcome))
		{
			if (!string.IsNullOrWhiteSpace(_successEmote))
			{
				CharacterOwner.OutputHandler.Handle(
					new EmoteOutput(new Emote(_successEmote, CharacterOwner, Target, Tool),
						flags: OutputFlags.SuppressObscured));
			}
		}
		else
		{
			_failureAction?.Invoke(outcome);
			if (!string.IsNullOrWhiteSpace(_failureEmote))
			{
				CharacterOwner.OutputHandler.Handle(
					new EmoteOutput(new Emote(_failureEmote, CharacterOwner, Target, Tool),
						flags: OutputFlags.SuppressObscured));
			}

			if (outcome.IsAbjectFailure)
			{
				_abjectFailureAction?.Invoke(outcome);
			}
		}

		Owner.RemoveEffect(this, true);
	}

	public override void RemovalEffect()
	{
		InventoryPlan.FinalisePlan();
		ReleaseEventHandlers();
	}
}
