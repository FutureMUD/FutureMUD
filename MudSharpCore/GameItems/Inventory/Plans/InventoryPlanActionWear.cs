using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanActionWear : InventoryPlanAction
{
	public InventoryPlanActionWear(XElement root, IFuturemud gameworld)
		: base(root, gameworld, DesiredItemState.Worn)
	{
	}

	public InventoryPlanActionWear(IFuturemud gameworld, long primaryTag, long secondaryTag,
		Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector)
		: base(gameworld, DesiredItemState.Worn, primaryTag, secondaryTag, primaryselector, secondaryselector)
	{
	}

	#region Overrides of InventoryPlanAction

	public override XElement SaveToXml()
	{
		return new XElement("Action",
			new XAttribute("state", "worn"),
			new XAttribute("tag", DesiredTag?.Id ?? 0),
			new XAttribute("secondtag", DesiredSecondaryTag?.Id ?? 0),
			new XAttribute("inplaceoverride", ItemsAlreadyInPlaceOverrideFitnessScore),
			new XAttribute("inplacemultiplier", ItemsAlreadyInPlaceMultiplier),
			new XAttribute("originalreference", OriginalReference?.ToString() ?? "")
		);
	}

	public override string Describe(ICharacter voyeur)
	{
		return $"Wearing {DesiredTag?.Name.A_An_RespectPlurals(colour: Telnet.Cyan) ?? "an item"}";
	}

	#endregion

	protected double GetItemFitness(ICharacter executor, IGameItem item)
	{
		if (!item.IsA(DesiredTag) || !(PrimaryItemSelector?.Invoke(item) ?? true))
		{
			return 0.0;
		}

		if (executor.Body.WornItems.Contains(item))
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
		// Worn items next
		var items =
			executor.Body.WornItems.Where(
				x =>
					x.IsA(DesiredTag) &&
					(PrimaryItemSelector?.Invoke(x) ?? true) &&
					executor.Body.CanRemoveItem(x) &&
					x.IsItemType<IWearable>() &&
					executor.Body.CanRemoveItem(x, ItemCanGetIgnore.IgnoreFreeHands) &&
					executor.Body.CanWear(x));
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		// Already wielded items next
		items =
			executor.Body.WieldedItems.Where(
				x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && x.IsItemType<IWearable>() &&
				     executor.Body.CanWear(x));
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		// Already held items next
		items =
			executor.Body.HeldItems.Where(
				x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && x.IsItemType<IWearable>() &&
				     executor.Body.CanWear(x));
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
			        .Where(x => x.IsA(DesiredTag) &&
			                    (PrimaryItemSelector?.Invoke(x) ?? true) && x.IsItemType<IWearable>() &&
			                    executor.Body.CanWear(x)
			        );
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
							        y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true) &&
							        y.Parent.IsItemType<IWearable>() && executor.Body.CanWear(y.Parent)
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

		// In containers in inventory
		foreach (var container in executor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>()))
		{
			items = container.Contents.Where(
				x =>
					x.IsA(DesiredTag) &&
					(PrimaryItemSelector?.Invoke(x) ?? true) &&
					executor.Body.CanGet(x, container.Parent, 0,
						ItemCanGetIgnore.IgnoreInventoryPlans | ItemCanGetIgnore.IgnoreFreeHands) &&
					x.IsItemType<IWearable>() && executor.Body.CanWear(x)
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
					x.IsA(DesiredTag) &&
					(PrimaryItemSelector?.Invoke(x) ?? true) && x.IsItemType<IWearable>() &&
					executor.Body.CanWear(x));
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
							        y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true) &&
							        y.Parent.IsItemType<IWearable>() && executor.Body.CanWear(y.Parent)
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
			        .Where(x => x.IsA(DesiredTag) &&
			                    (PrimaryItemSelector?.Invoke(x) ?? true) && x.IsItemType<IWearable>() &&
			                    executor.Body.CanWear(x));
		foreach (var item in items)
		{
			var fitness = GetItemFitness(executor, item);
			if (fitness > 0.0)
			{
				potentialItems.Add((item, fitness, internalOrder++));
			}
		}

		// In containers in location
		foreach (var container in executor.Location.LayerGameItems(executor.RoomLayer)
		                                  .SelectNotNull(x => x.GetItemType<IContainer>())
		                                  .Where(x => executor.Location.CanGetAccess(x.Parent, executor)))
		{
			items = container.Contents.Where(
				x =>
					x.IsA(DesiredTag) &&
					(PrimaryItemSelector?.Invoke(x) ?? true) &&
					executor.Body.CanGet(x, container.Parent, 0,
						ItemCanGetIgnore.IgnoreInventoryPlans | ItemCanGetIgnore.IgnoreFreeHands) &&
					x.IsItemType<IWearable>() && executor.Body.CanWear(x)
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

	public IWearProfile DesiredProfile { get; init; }
}