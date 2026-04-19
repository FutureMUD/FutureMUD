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

public class ItemComponentConfigurationAction : CharacterActionWithTarget
{
	private readonly IReadOnlyCollection<IInventoryPlan> _inventoryPlans;
	private readonly Func<CheckOutcome> _checkAction;
	private readonly Func<CheckOutcome, bool> _successAction;
	private readonly Func<CheckOutcome, IList<IGameItem>>? _successExemptItemsAction;
	private readonly Action<CheckOutcome>? _failureAction;
	private readonly Action<CheckOutcome>? _abjectFailureAction;
	private readonly string _beginEmote;
	private readonly string _continueEmote;
	private readonly string _successEmote;
	private readonly string _failureEmote;
	private readonly TimeSpan _stageDuration;
	private readonly int _totalStages;
	private readonly string _cancelEmote;
	private bool _completedSuccessfully;
	private bool _toolHandlersAttached;
	private bool _teardownStarted;
	private bool _plansFinalised;
	private IList<IGameItem>? _successExemptItems;

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
		Func<CheckOutcome, IList<IGameItem>>? successExemptItemsAction = null,
		Action<CheckOutcome>? failureAction = null,
		Action<CheckOutcome>? abjectFailureAction = null)
		: this(owner, target, tool, [inventoryPlan], actionDescription, beginEmote, continueEmote, cancelEmote,
			successEmote, failureEmote, stageDuration, totalStages, checkAction, successAction,
			successExemptItemsAction, failureAction, abjectFailureAction)
	{
	}

	public ItemComponentConfigurationAction(
		ICharacter owner,
		IGameItem target,
		IGameItem tool,
		IEnumerable<IInventoryPlan> inventoryPlans,
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
		Func<CheckOutcome, IList<IGameItem>>? successExemptItemsAction = null,
		Action<CheckOutcome>? failureAction = null,
		Action<CheckOutcome>? abjectFailureAction = null)
		: base(owner, target)
	{
		_inventoryPlans = inventoryPlans.ToList();
		Tool = tool;
		_checkAction = checkAction;
		_successAction = successAction;
		_successExemptItemsAction = successExemptItemsAction;
		_failureAction = failureAction;
		_abjectFailureAction = abjectFailureAction;
		_beginEmote = beginEmote;
		_continueEmote = continueEmote;
		_successEmote = successEmote;
		_failureEmote = failureEmote;
		_stageDuration = stageDuration;
		_totalStages = Math.Max(1, totalStages);
		_cancelEmote = cancelEmote;
		ActionDescription = actionDescription;
		CancelEmoteString = cancelEmote;
		WhyCannotMoveEmoteString = "@ cannot move because #0 are|is working on $1.";
		LDescAddendum = "working on $1";
		_blocks.Add("general");
		_blocks.Add("movement");
		AttachToolEventHandlers();
	}

	public IInventoryPlan InventoryPlan => _inventoryPlans.First();
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
		_completedSuccessfully = outcome.IsPass() && _successAction(outcome);
		if (_completedSuccessfully)
		{
			_successExemptItems = _successExemptItemsAction?.Invoke(outcome);
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
		_teardownStarted = true;
		ReleaseEventHandlers();
		if (_plansFinalised)
		{
			return;
		}

		_plansFinalised = true;
		foreach (var plan in _inventoryPlans.Distinct())
		{
			if (_completedSuccessfully && _successExemptItems?.Any() == true)
			{
				plan.FinalisePlanWithExemptions(_successExemptItems);
				continue;
			}

			plan.FinalisePlan();
		}

	}

	protected override void ReleaseEventHandlers()
	{
		DetachToolEventHandlers();
		base.ReleaseEventHandlers();
	}

	private void AttachToolEventHandlers()
	{
		if (_toolHandlersAttached)
		{
			return;
		}

		Tool.OnDeath -= Tool_OnDeath;
		Tool.OnDeath += Tool_OnDeath;
		Tool.OnDeleted -= Tool_OnDeath;
		Tool.OnDeleted += Tool_OnDeath;
		Tool.OnRemovedFromLocation -= Tool_OnRemovedFromLocation;
		Tool.OnRemovedFromLocation += Tool_OnRemovedFromLocation;
		CharacterOwner.Body.OnInventoryChange -= Body_OnInventoryChange;
		CharacterOwner.Body.OnInventoryChange += Body_OnInventoryChange;
		_toolHandlersAttached = true;
	}

	private void DetachToolEventHandlers()
	{
		if (!_toolHandlersAttached)
		{
			return;
		}

		Tool.OnDeath -= Tool_OnDeath;
		Tool.OnDeleted -= Tool_OnDeath;
		Tool.OnRemovedFromLocation -= Tool_OnRemovedFromLocation;
		CharacterOwner.Body.OnInventoryChange -= Body_OnInventoryChange;
		_toolHandlersAttached = false;
	}

	private void Body_OnInventoryChange(Body.InventoryState oldState, Body.InventoryState newState, IGameItem item)
	{
		if (_teardownStarted || !ReferenceEquals(item, Tool))
		{
			return;
		}

		if (newState == Body.InventoryState.Held || newState == Body.InventoryState.Wielded)
		{
			return;
		}

		CancelAction($"{_cancelEmote} because #0 are|is no longer holding $1.");
	}

	private void Tool_OnRemovedFromLocation(IPerceivable owner)
	{
		if (_teardownStarted)
		{
			return;
		}

		CancelAction($"{_cancelEmote} because $1 is no longer there.");
	}

	private void Tool_OnDeath(IPerceivable owner)
	{
		if (_teardownStarted)
		{
			return;
		}

		CancelAction($"{_cancelEmote} because #0 no longer have|has $1.");
	}

	private void CancelAction(string emoteText)
	{
		if (_teardownStarted)
		{
			return;
		}

		_teardownStarted = true;
		ReleaseEventHandlers();
		CharacterOwner.OutputHandler.Handle(
			new EmoteOutput(new Emote(emoteText, CharacterOwner, Target, Tool), flags: OutputFlags.SuppressObscured));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}
}
