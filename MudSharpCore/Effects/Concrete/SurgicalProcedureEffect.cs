using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class SurgicalProcedureEffect : StagedCharacterActionWithTarget
{
	public SurgicalProcedureEffect(ICharacter surgeon, ICharacter patient, ISurgicalProcedure procedure,
		IEnumerable<Action<IPerceivable>> actions, string actionDescription, string[] blocks, string ldescAddendum,
		int fireOnCount,
		IEnumerable<TimeSpan> timesBetweenTicks, List<(IGameItem Item, DesiredItemState State)> additionalInventory,
		params object[] additionalArguments)
		: base(
			surgeon, patient, actionDescription, $"@ cease|ceases &0's {procedure.ProcedureName} of $1", "", blocks,
			ldescAddendum,
			actions, fireOnCount, timesBetweenTicks)
	{
		ActionQueue = new Queue<Action<IPerceivable>>(actions);
		ActionDescription = actionDescription;
		FireOnCount = fireOnCount;
		TimesBetweenTicks = new Queue<TimeSpan>(timesBetweenTicks);
		CurrentCount = 0;
		Procedure = procedure;
		Patient = patient;
		Surgeon = surgeon;
		AdditionalArguments = additionalArguments;
		AdditionalInventory = additionalInventory;
		SetupAdditionalInventory();
	}

	public ISurgicalProcedure Procedure { get; set; }
	public ICharacter Surgeon { get; set; }
	public ICharacter Patient { get; set; }
	public object[] AdditionalArguments { get; set; }
	public override bool CanBeStoppedByPlayer => true;

	public List<(IGameItem Item, DesiredItemState State)> AdditionalInventory { get; }

	protected override void ReleaseEventHandlers()
	{
		base.ReleaseEventHandlers();
		foreach (var item in AdditionalInventory)
		{
			item.Item.OnDeath -= Item_OnDeath;
			item.Item.OnDeleted -= Item_OnDeath;
			if (item.State == DesiredItemState.InRoom)
			{
				item.Item.OnRemovedFromLocation -= Item_OnRemovedFromLocation;
			}

			if (item.State == DesiredItemState.Held || item.State == DesiredItemState.Wielded ||
			    item.State == DesiredItemState.WieldedOneHandedOnly ||
			    item.State == DesiredItemState.WieldedTwoHandedOnly ||
			    item.State == DesiredItemState.Worn)
			{
				Surgeon.Body.OnInventoryChange -= Body_OnInventoryChange;
			}
		}
	}

	protected void SetupAdditionalInventory()
	{
		foreach (var item in AdditionalInventory)
		{
			item.Item.OnDeath += Item_OnDeath;
			item.Item.OnDeleted += Item_OnDeath;
			if (item.State == DesiredItemState.InRoom)
			{
				item.Item.OnRemovedFromLocation += Item_OnRemovedFromLocation;
			}

			if (item.State == DesiredItemState.Held || item.State == DesiredItemState.Wielded ||
			    item.State == DesiredItemState.WieldedOneHandedOnly ||
			    item.State == DesiredItemState.WieldedTwoHandedOnly || item.State == DesiredItemState.Worn)
			{
				Surgeon.Body.OnInventoryChange -= Body_OnInventoryChange;
				Surgeon.Body.OnInventoryChange += Body_OnInventoryChange;
			}
		}
	}

	private void Body_OnInventoryChange(Body.InventoryState oldState, Body.InventoryState newState, IGameItem item)
	{
		switch (AdditionalInventory.Where(x => x.Item == item).Select(x => x.State)
		                           .DefaultIfEmpty(DesiredItemState.Unknown).First())
		{
			case DesiredItemState.Held:
				if (newState != Body.InventoryState.Held && newState != Body.InventoryState.Wielded)
				{
					CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"{CancelEmoteString} because #0 are|is no longer holding $2.", Surgeon, Surgeon, Patient,
						item)));
					OnStopAction = null;
					CharacterOwner.RemoveEffect(this, true);
				}

				break;
			case DesiredItemState.Wielded:
			case DesiredItemState.WieldedOneHandedOnly:
			case DesiredItemState.WieldedTwoHandedOnly:
				if (newState != Body.InventoryState.Wielded)
				{
					CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"{CancelEmoteString} because #0 are|is no longer wielding $2.", Surgeon, Surgeon, Patient,
						item)));
					OnStopAction = null;
					CharacterOwner.RemoveEffect(this, true);
				}

				break;
			case DesiredItemState.Worn:
				if (newState != Body.InventoryState.Worn)
				{
					CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"{CancelEmoteString} because #0 are|is no longer wearing $2.", Surgeon, Surgeon, Patient,
						item)));
					OnStopAction = null;
					CharacterOwner.RemoveEffect(this, true);
				}

				break;
		}
	}

	private void Item_OnRemovedFromLocation(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(
			new Emote($"{CancelEmoteString} because $2 is no longer there.", Surgeon, Surgeon, Patient, owner)));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	private void Item_OnDeath(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(
			new Emote($"{CancelEmoteString} because #0 no longer have|has $2.", Surgeon, Surgeon, Patient, owner)));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	public override string WhyCannotMoveEmoteString
	{
		get =>
			$"@ cannot move because $0 $0|are|is {Procedure.DescribeProcedureGerund(Surgeon, Patient, AdditionalArguments)}.";

		set { }
	}

	public override void RemovalEffect()
	{
		ReleaseEventHandlers();
		var outcome = Gameworld.GetCheck(Procedure.Check)
		                       .Check(Surgeon, Procedure.GetProcedureDifficulty(Surgeon, Patient, AdditionalArguments),
								   Procedure.CheckTrait,
			                       Patient,
			                       Procedure.BaseCheckBonus);
		if (CurrentCount == FireOnCount)
		{
			Procedure.CompleteProcedure(Surgeon, Patient, outcome, AdditionalArguments);
		}
		else
		{
			Procedure.AbortProcedure(Surgeon, Patient, outcome, AdditionalArguments);
		}
	}
}