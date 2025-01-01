using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;

namespace MudSharp.Effects.Concrete;

public class CharacterActionWithTargetAndTool : CharacterActionWithTarget
{
	public CharacterActionWithTargetAndTool(ICharacter owner, IPerceivable target,
		IEnumerable<(IGameItem Item, DesiredItemState State)> additionalInventory) : base(owner, target)
	{
		AdditionalInventory = additionalInventory.ToList();
		SetupAdditionalInventory();
	}

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
				CharacterOwner.Body.OnInventoryChange -= Body_OnInventoryChange;
			}
		}
	}

	protected void SetupAdditionalInventory()
	{
		foreach (var item in AdditionalInventory)
		{
			item.Item.OnDeath -= Item_OnDeath;
			item.Item.OnDeath += Item_OnDeath;
			item.Item.OnDeleted -= Item_OnDeath;
			item.Item.OnDeleted += Item_OnDeath;
			if (item.State == DesiredItemState.InRoom)
			{
				item.Item.OnRemovedFromLocation -= Item_OnRemovedFromLocation;
				item.Item.OnRemovedFromLocation += Item_OnRemovedFromLocation;
			}

			if (item.State == DesiredItemState.Held || item.State == DesiredItemState.Wielded ||
			    item.State == DesiredItemState.WieldedOneHandedOnly ||
			    item.State == DesiredItemState.WieldedTwoHandedOnly || item.State == DesiredItemState.Worn)
			{
				CharacterOwner.Body.OnInventoryChange -= Body_OnInventoryChange;
				CharacterOwner.Body.OnInventoryChange += Body_OnInventoryChange;
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
						$"{CancelEmoteString} because #0 are|is no longer holding $2.", CharacterOwner, CharacterOwner,
						Target, item)));
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
						$"{CancelEmoteString} because #0 are|is no longer wielding $2.", CharacterOwner, CharacterOwner,
						Target, item)));
					OnStopAction = null;
					CharacterOwner.RemoveEffect(this, true);
				}

				break;
			case DesiredItemState.Worn:
				if (newState != Body.InventoryState.Worn)
				{
					CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"{CancelEmoteString} because #0 are|is no longer wearing $2.", CharacterOwner, CharacterOwner,
						Target, item)));
					OnStopAction = null;
					CharacterOwner.RemoveEffect(this, true);
				}

				break;
		}
	}

	private void Item_OnRemovedFromLocation(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"{CancelEmoteString} because $2 is no longer there.", CharacterOwner, CharacterOwner, Target, owner)));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	private void Item_OnDeath(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"{CancelEmoteString} because #0 no longer have|has $2.", CharacterOwner, CharacterOwner, Target, owner)));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}
}