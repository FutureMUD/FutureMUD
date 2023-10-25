using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanActionSheath : InventoryPlanAction
{
	public InventoryPlanActionSheath(XElement root, IFuturemud gameworld)
		: base(root, gameworld, DesiredItemState.Sheathed)
	{
	}

	public InventoryPlanActionSheath(IFuturemud gameworld, long primaryTag, long secondaryTag,
		Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector)
		: base(gameworld, DesiredItemState.Sheathed, primaryTag, secondaryTag, primaryselector, secondaryselector)
	{
	}

	#region Overrides of InventoryPlanAction

	public override XElement SaveToXml()
	{
		return new XElement("Action",
			new XAttribute("state", "sheathed"),
			new XAttribute("tag", DesiredTag?.Id ?? 0),
			new XAttribute("secondtag", DesiredSecondaryTag?.Id ?? 0),
			new XAttribute("inplaceoverride", ItemsAlreadyInPlaceOverrideFitnessScore),
			new XAttribute("inplacemultiplier", ItemsAlreadyInPlaceMultiplier),
			new XAttribute("originalreference", OriginalReference?.ToString() ?? "")
		);
	}

	public override string Describe(ICharacter voyeur)
	{
		return
			$"Sheathed {DesiredTag?.Name.A_An_RespectPlurals(colour: Telnet.Cyan) ?? "an item"} in {DesiredSecondaryTag?.Name.A_An_RespectPlurals(colour: Telnet.Cyan) ?? "another item"}";
	}

	#endregion

	public override IGameItem ScoutSecondary(ICharacter executor, IGameItem item)
	{
		if (item is null)
		{
			return null;
		}

		return
			executor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>())
			        .FirstOrDefault(
				        x =>
					        x.Parent.IsA(DesiredSecondaryTag) && (SecondaryItemSelector?.Invoke(x.Parent) ?? true) &&
					        (x.Content == null || x.Content.Parent == item) && x.MaximumSize >= item.Size)?.Parent;
	}

	public override IGameItem ScoutTarget(ICharacter executor)
	{
		var sheaths =
			executor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>())
			        .Where(
				        x =>
					        x.Parent.IsA(DesiredSecondaryTag) && (SecondaryItemSelector?.Invoke(x.Parent) ?? true) &&
					        x.Content == null)
			        .ToList();

		// Already Sheathed next
		var item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>())
			        .SelectNotNull(x => x.Content?.Parent)
			        .FirstOrDefault(x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true));
		if (item != null)
		{
			return item;
		}

		// Already wielded items first
		item =
			executor.Body.WieldedItems.FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					sheaths.Any(y => y.MaximumSize >= x.Size));
		if (item != null)
		{
			return item;
		}

		// Already held items next
		item =
			executor.Body.HeldItems.FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && x.IsItemType<IWieldable>() &&
					sheaths.Any(y => y.MaximumSize >= x.Size));
		if (item != null)
		{
			return item;
		}

		// Worn items next
		item =
			executor.Body.WornItems.FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && executor.Body.CanRemoveItem(x) &&
					x.IsItemType<IWieldable>() && sheaths.Any(y => y.MaximumSize >= x.Size));
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
						        y =>
							        y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true) &&
							        y.Parent.IsItemType<IWieldable>() &&
							        sheaths.Any(z => z.MaximumSize >= y.Parent.Size))?.Parent)
			        .FirstOrDefault(x => x != null);
		if (item != null)
		{
			return item;
		}

		// In containers in inventory
		item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>())
			        .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
			        .SelectMany(x => x.Contents)
			        .FirstOrDefault(
				        x =>
					        x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        x.IsItemType<IWieldable>() && sheaths.Any(y => y.MaximumSize >= x.Size));
		if (item != null)
		{
			return item;
		}

		// In location
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && x.IsItemType<IWieldable>() &&
					x.IsItemType<IHoldable>() &&
					x.GetItemType<IHoldable>().IsHoldable && 
					sheaths.Any(y => y.MaximumSize >= x.Size));
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
						        y =>
							        y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true) &&
							        y.Parent.IsItemType<IWieldable>() &&
							        sheaths.Any(z => z.MaximumSize >= y.Parent.Size))?.Parent)
			        .FirstOrDefault(x => x != null);
		if (item != null)
		{
			return item;
		}

		// Sheathed in room item next
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<ISheath>())
			        .SelectNotNull(x => x.Content?.Parent)
			        .FirstOrDefault(
				        x =>
					        x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        sheaths.Any(y => y.MaximumSize >= x.Size));
		if (item != null)
		{
			return item;
		}

		// In containers in location
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<IContainer>())
			        .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
			        .SelectMany(x => x.Contents)
			        .FirstOrDefault(
				        x =>
					        x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        x.IsItemType<IWieldable>() && sheaths.Any(y => y.MaximumSize >= x.Size));
		return item;
	}
}