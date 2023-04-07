using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanActionDrop : InventoryPlanAction
{
	public InventoryPlanActionDrop(XElement root, IFuturemud gameworld)
		: base(root, gameworld, DesiredItemState.InRoom)
	{
	}

	public InventoryPlanActionDrop(IFuturemud gameworld, long primaryTag, long secondaryTag,
		Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector)
		: base(gameworld, DesiredItemState.InRoom, primaryTag, secondaryTag, primaryselector, secondaryselector)
	{
	}

	#region Overrides of InventoryPlanAction

	public override XElement SaveToXml()
	{
		return new XElement("Action",
			new XAttribute("state", "dropped"),
			new XAttribute("tag", DesiredTag?.Id ?? 0),
			new XAttribute("secondtag", DesiredSecondaryTag?.Id ?? 0),
			new XAttribute("inplaceoverride", ItemsAlreadyInPlaceOverrideFitnessScore),
			new XAttribute("inplacemultiplier", ItemsAlreadyInPlaceMultiplier),
			new XAttribute("originalreference", OriginalReference?.ToString() ?? "")
		);
	}


	public override string Describe(ICharacter voyeur)
	{
		return $"In Room {DesiredTag?.Name.A_An_RespectPlurals(colour: Telnet.Cyan) ?? "an item"}";
	}

	#endregion

	public override IGameItem ScoutSecondary(ICharacter executor, IGameItem item)
	{
		return null;
	}

	public override IGameItem ScoutTarget(ICharacter executor)
	{
		// In location
		var item =
			executor.Location.LayerGameItems(executor.RoomLayer).FirstOrDefault(
				x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true));
		if (item != null)
		{
			return item;
		}

		// Already held items next
		item =
			executor.Body.HeldItems.FirstOrDefault(
				x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true));
		if (item != null)
		{
			return item;
		}

		// Wielded items next
		item =
			executor.Body.WieldedItems.FirstOrDefault(
				x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true));
		if (item != null)
		{
			return item;
		}

		// Worn items next
		item =
			executor.Body.WornItems.FirstOrDefault(
				x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && executor.Body.CanRemoveItem(x));
		if (item != null)
		{
			return item;
		}

		// Attached to worn items next
		item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<IBelt>())
			        .Select(
				        x =>
					        x.ConnectedItems.FirstOrDefault(
							        y => y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true))?
						        .Parent)
			        .FirstOrDefault(x => x != null);
		if (item != null)
		{
			return item;
		}

		// Sheathed next
		item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>())
			        .SelectNotNull(x => x.Content?.Parent)
			        .FirstOrDefault(x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true));
		if (item != null)
		{
			return item;
		}

		// In containers in inventory
		item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>())
			        .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
			        .SelectMany(x => x.Contents)
			        .FirstOrDefault(x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true));
		if (item != null)
		{
			return item;
		}

		// Attached to room items next
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<IBelt>())
			        .Select(
				        x =>
					        x.ConnectedItems.FirstOrDefault(
							        y => y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true))?
						        .Parent)
			        .FirstOrDefault(x => x != null);
		if (item != null)
		{
			return item;
		}

		// Sheathed in room item next
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<ISheath>())
			        .SelectNotNull(x => x.Content?.Parent)
			        .FirstOrDefault(x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true));
		if (item != null)
		{
			return item;
		}

		// In containers in location
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<IContainer>())
			        .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
			        .SelectMany(x => x.Contents)
			        .FirstOrDefault(x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true));
		return item;
	}
}