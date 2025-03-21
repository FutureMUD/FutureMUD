using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanActionAttach : InventoryPlanAction
{
	public InventoryPlanActionAttach(XElement root, IFuturemud gameworld)
		: base(root, gameworld, DesiredItemState.Attached)
	{
	}

	public InventoryPlanActionAttach(IFuturemud gameworld, long primaryTag, long secondaryTag,
		Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector)
		: base(gameworld, DesiredItemState.Attached, primaryTag, secondaryTag, primaryselector, secondaryselector)
	{
	}

	public override IGameItem ScoutSecondary(ICharacter executor, IGameItem item)
	{
		if (item is null)
		{
			return null;
		}

		var beltable = item.GetItemType<IBeltable>();
		return
			executor.Inventory.SelectNotNull(x => x.GetItemType<IBelt>())
			        .FirstOrDefault(
				        x =>
					        x.Parent.IsA(DesiredSecondaryTag) && (SecondaryItemSelector?.Invoke(x.Parent) ?? true) &&
					        x.CanAttachBeltable(beltable) == IBeltCanAttachBeltableResult.Success)?.Parent;
	}

	public override IGameItem ScoutTarget(ICharacter executor)
	{
		var belts =
			executor.Inventory.SelectNotNull(x => x.GetItemType<IBelt>())
			        .Where(x => x.Parent.IsA(DesiredSecondaryTag) && (SecondaryItemSelector?.Invoke(x.Parent) ?? true))
			        .ToList();

		// Attached to inventory items first
		var item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<IBelt>())
			        .Select(
				        x =>
					        x.ConnectedItems.FirstOrDefault(
							        y => y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x.Parent) ?? true))?
						        .Parent)
			        .FirstOrDefault(x => x != null);
		if (item != null)
		{
			return item;
		}

		// Already held items first
		item =
			executor.Body.HeldItems.FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && x.IsItemType<IBeltable>() &&
					belts.Any(
						y => y.CanAttachBeltable(x.GetItemType<IBeltable>()) == IBeltCanAttachBeltableResult.Success));
		if (item != null)
		{
			return item;
		}

		// Wielded items next
		item =
			executor.Body.WieldedItems.FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && x.IsItemType<IBeltable>() &&
					belts.Any(
						y => y.CanAttachBeltable(x.GetItemType<IBeltable>()) == IBeltCanAttachBeltableResult.Success));
		if (item != null)
		{
			return item;
		}

		// Worn items next
		item =
			executor.Body.WornItems.FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && executor.Body.CanRemoveItem(x) &&
					x.IsItemType<IBeltable>() &&
					belts.Any(
						y => y.CanAttachBeltable(x.GetItemType<IBeltable>()) == IBeltCanAttachBeltableResult.Success));
		if (item != null)
		{
			return item;
		}

		// Sheathed next
		item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>())
			        .SelectNotNull(x => x.Content?.Parent)
			        .FirstOrDefault(
				        x =>
					        x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        x.IsItemType<IBeltable>() &&
					        belts.Any(
						        y =>
							        y.CanAttachBeltable(x.GetItemType<IBeltable>()) ==
							        IBeltCanAttachBeltableResult.Success));
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
					        x.IsItemType<IBeltable>() &&
					        belts.Any(
						        y =>
							        y.CanAttachBeltable(x.GetItemType<IBeltable>()) ==
							        IBeltCanAttachBeltableResult.Success));
		if (item != null)
		{
			return item;
		}

		// In location
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && 
					x.IsItemType<IHoldable>() &&
					x.GetItemType<IHoldable>().IsHoldable &&
					x.IsItemType<IBeltable>() &&
					belts.Any(
						y => y.CanAttachBeltable(x.GetItemType<IBeltable>()) == IBeltCanAttachBeltableResult.Success));
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
								        belts.Any(z => z.CanAttachBeltable(y) == IBeltCanAttachBeltableResult.Success))?
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
			        .FirstOrDefault(
				        x =>
					        x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        x.IsItemType<IBeltable>() &&
					        belts.Any(
						        y =>
							        y.CanAttachBeltable(x.GetItemType<IBeltable>()) ==
							        IBeltCanAttachBeltableResult.Success));
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
					        x.IsItemType<IBeltable>() &&
					        belts.Any(
						        y =>
							        y.CanAttachBeltable(x.GetItemType<IBeltable>()) ==
							        IBeltCanAttachBeltableResult.Success));
		return item;
	}

	#region Overrides of InventoryPlanAction

	public override XElement SaveToXml()
	{
		return new XElement("Action", new XAttribute("state", "attached"),
			new XAttribute("tag", DesiredTag?.Id ?? 0), new XAttribute("secondtag", DesiredSecondaryTag?.Id ?? 0),
			new XAttribute("inplaceoverride", ItemsAlreadyInPlaceOverrideFitnessScore),
			new XAttribute("inplacemultiplier", ItemsAlreadyInPlaceMultiplier),
			new XAttribute("originalreference", OriginalReference?.ToString() ?? ""));
	}

	public override string Describe(ICharacter voyeur)
	{
		return
			$"Attached {DesiredTag?.Name.A_An_RespectPlurals(colour: Telnet.Cyan) ?? "an item"} to {DesiredSecondaryTag?.Name.A_An_RespectPlurals(colour: Telnet.Cyan) ?? "another item"}";
	}

	/// <inheritdoc />
	public override bool RequiresFreeHandsToExecute(ICharacter who, IGameItem item)
	{
		return true;
	}

	#endregion
}