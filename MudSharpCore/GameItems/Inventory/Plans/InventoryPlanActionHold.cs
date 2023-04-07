using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanActionHold : InventoryPlanAction
{
	public InventoryPlanActionHold(XElement root, IFuturemud gameworld)
		: base(root, gameworld, DesiredItemState.Held)
	{
		Quantity = int.Parse(root.Attribute("quantity")?.Value ?? "1");
		QuantityIsOptional = bool.Parse(root.Attribute("optionalquantity")?.Value ?? "false");
	}

	public InventoryPlanActionHold(IFuturemud gameworld, long primaryTag, long secondaryTag,
		Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector, int quantity = 0)
		: base(gameworld, DesiredItemState.Held, primaryTag, secondaryTag, primaryselector, secondaryselector)
	{
		Quantity = quantity;
	}

	#region Overrides of InventoryPlanAction

	public override XElement SaveToXml()
	{
		return new XElement("Action",
			new XAttribute("state", "held"),
			new XAttribute("tag", DesiredTag?.Id ?? 0),
			new XAttribute("secondtag", DesiredSecondaryTag?.Id ?? 0),
			new XAttribute("quantity", Quantity),
			new XAttribute("optionalquantity", QuantityIsOptional),
			new XAttribute("inplaceoverride", ItemsAlreadyInPlaceOverrideFitnessScore),
			new XAttribute("inplacemultiplier", ItemsAlreadyInPlaceMultiplier),
			new XAttribute("originalreference", OriginalReference?.ToString() ?? "")
		);
	}

	public override string Describe(ICharacter voyeur)
	{
		return
			$"Holding {(QuantityIsOptional ? "up to " : "")}{Quantity.ToString("N0", voyeur)} of {DesiredTag?.Name.ColourName() ?? "an item"}";
	}

	#endregion

	public int Quantity { get; set; }

	public bool QuantityIsOptional { get; set; }

	protected double GetItemFitness(ICharacter executor, IGameItem item)
	{
		if (!item.IsA(DesiredTag) || !(PrimaryItemSelector?.Invoke(item) ?? true) ||
		    (item.GetItemType<IStackable>()?.Quantity ?? 1) < Quantity)
		{
			return 0.0;
		}

		if (executor.Body.HeldItems.Contains(item))
		{
			if (ItemsAlreadyInPlaceOverrideFitnessScore)
			{
				return double.MaxValue;
			}

			return (PrimaryItemFitnessScorer?.Invoke(item) ?? 1.0) * ItemsAlreadyInPlaceMultiplier;
		}

		return PrimaryItemFitnessScorer?.Invoke(item) ?? 1.0;
	}

	public override IGameItem ScoutSecondary(ICharacter executor, IGameItem item)
	{
		return null;
	}

	public override IGameItem ScoutTarget(ICharacter executor)
	{
		var potentialItems = new List<(IGameItem Item, double Fitness, int InternalOrder)>();
		var internalOrder = 0;
		// Already held items first
		var items =
			executor.Body.HeldItems.Where(
				x =>
					((x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity || QuantityIsOptional) &&
					x.IsA(DesiredTag) &&
					(PrimaryItemSelector?.Invoke(x) ?? true));
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		// Wielded items next
		items =
			executor.Body.WieldedItems.Where(
				x =>
					((x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity || QuantityIsOptional) &&
					x.IsA(DesiredTag) &&
					(PrimaryItemSelector?.Invoke(x) ?? true));
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		// Worn items next
		items =
			executor.Body.WornItems.Where(
				x =>
					((x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity || QuantityIsOptional) &&
					x.IsA(DesiredTag) &&
					(PrimaryItemSelector?.Invoke(x) ?? true) &&
					executor.Body.CanRemoveItem(x, ItemCanGetIgnore.IgnoreFreeHands));
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		// Attached to worn items next
		items =
			executor.Inventory.SelectNotNull(x => x.GetItemType<IBelt>())
			        .SelectMany(
				        x =>
					        x.ConnectedItems.Where(
						        y =>
							        ((y.Parent.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity ||
							         QuantityIsOptional) &&
							        y.Parent.IsA(DesiredTag) &&
							        (PrimaryItemSelector?.Invoke(y.Parent) ?? true) &&
							        executor.Body.CanGet(y.Parent, Quantity,
								        ItemCanGetIgnore.IgnoreInventoryPlans | ItemCanGetIgnore.IgnoreFreeHands)
					        ))
			        .SelectNotNull(x => x?.Parent)
			;
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		// Sheathed next
		items =
			executor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>())
			        .SelectNotNull(x => x.Content?.Parent)
			        .Where(
				        x =>
					        ((x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity || QuantityIsOptional) &&
					        x.IsA(DesiredTag) &&
					        (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        executor.Body.CanDraw(x, null, ItemCanWieldFlags.IgnoreFreeHands)
			        );
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		// In containers next
		foreach (var container in executor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>()))
		{
			items = container.Contents.Where(
				x => ((x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity || QuantityIsOptional) &&
				     x.IsA(DesiredTag) &&
				     (PrimaryItemSelector?.Invoke(x) ?? true) &&
				     executor.Body.CanGet(x, container.Parent, Quantity,
					     ItemCanGetIgnore.IgnoreInventoryPlans | ItemCanGetIgnore.IgnoreFreeHands)
			);
			foreach (var item in items)
			{
				var fitness = GetItemFitness(executor, item);
				if (fitness > 0.0)
				{
					potentialItems.Add((item, fitness, internalOrder++));
				}
			}
		}

		// In location
		items =
			executor.Location.LayerGameItems(executor.RoomLayer).Where(
				x =>
					((x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity || QuantityIsOptional) &&
					x.IsA(DesiredTag) &&
					(PrimaryItemSelector?.Invoke(x) ?? true) && executor.Body.CanGet(x, Quantity,
						ItemCanGetIgnore.IgnoreInventoryPlans | ItemCanGetIgnore.IgnoreFreeHands));
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		// Attached to room items next
		items =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<IBelt>())
			        .Where(x => executor.Location.CanGetAccess(x.Parent, executor))
			        .SelectMany(
				        x =>
					        x.ConnectedItems.Where(
						        y =>
							        ((y.Parent.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity ||
							         QuantityIsOptional) &&
							        y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true) &&
							        executor.Body.CanGet(y.Parent, Quantity,
								        ItemCanGetIgnore.IgnoreInventoryPlans | ItemCanGetIgnore.IgnoreFreeHands)
					        ))
			        .SelectNotNull(x => x?.Parent)
			;
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		// Sheathed in room item next
		items =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<ISheath>())
			        .Where(x => executor.Location.CanGetAccess(x.Parent, executor))
			        .SelectNotNull(x => x.Content?.Parent)
			        .Where(
				        x =>
					        ((x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity || QuantityIsOptional) &&
					        x.IsA(DesiredTag) &&
					        (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        executor.Body.CanDraw(x, null, ItemCanWieldFlags.IgnoreFreeHands)
			        );
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		foreach (var container in executor.Location.LayerGameItems(executor.RoomLayer)
		                                  .SelectNotNull(x => x.GetItemType<IContainer>())
		                                  .Where(x => executor.Location.CanGetAccess(x.Parent, executor)))
		{
			items = container.Contents.Where(
				x => ((x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity || QuantityIsOptional) &&
				     x.IsA(DesiredTag) &&
				     (PrimaryItemSelector?.Invoke(x) ?? true) &&
				     executor.Body.CanGet(x, container.Parent, Quantity,
					     ItemCanGetIgnore.IgnoreInventoryPlans | ItemCanGetIgnore.IgnoreFreeHands)
			);
			foreach (var item in items)
			{
				var fitness = GetItemFitness(executor, item);
				if (fitness > 0.0)
				{
					potentialItems.Add((item, fitness, internalOrder++));
				}
			}
		}

		return potentialItems.OrderByDescending(x => x.Fitness).ThenBy(x => x.InternalOrder).FirstOrDefault().Item;
	}
}