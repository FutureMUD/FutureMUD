using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;
using MudSharp.Combat;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanActionWield : InventoryPlanAction
{
	private readonly Func<ICharacter, IGameItem, AttackHandednessOptions> _attackHandednessEvaluator;
	public AttackHandednessOptions Options { get; set; }

	public InventoryPlanActionWield(XElement root, IFuturemud gameworld)
		: base(root, gameworld, DesiredItemState.Wielded)
	{
		Options = (AttackHandednessOptions)int.Parse(root.Attribute("wieldstate").Value);
	}

	public InventoryPlanActionWield(IFuturemud gameworld, long primaryTag, long secondaryTag,
		Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector,
		Func<ICharacter, IGameItem, AttackHandednessOptions> handednessEvaluator = null)
		: base(gameworld, DesiredItemState.Wielded, primaryTag, secondaryTag, primaryselector, secondaryselector)
	{
		_attackHandednessEvaluator = handednessEvaluator;
	}

	#region Overrides of InventoryPlanAction

	public override XElement SaveToXml()
	{
		return new XElement("Action",
			new XAttribute("state", "wielded"),
			new XAttribute("tag", DesiredTag?.Id ?? 0),
			new XAttribute("secondtag", DesiredSecondaryTag?.Id ?? 0),
			new XAttribute("wieldstate", (int)Options),
			new XAttribute("inplaceoverride", ItemsAlreadyInPlaceOverrideFitnessScore),
			new XAttribute("inplacemultiplier", ItemsAlreadyInPlaceMultiplier),
			new XAttribute("originalreference", OriginalReference?.ToString() ?? "")
		);
	}

	public override string Describe(ICharacter voyeur)
	{
		return $"Wielding {DesiredTag?.Name.A_An_RespectPlurals(colour: Telnet.Cyan) ?? "an item"}";
	}

	/// <inheritdoc />
	public override bool RequiresFreeHandsToExecute(ICharacter who, IGameItem item)
	{
		return true;
	}

	#endregion

	protected double GetItemFitness(ICharacter executor, IGameItem item)
	{
		if (!item.IsA(DesiredTag) || !(PrimaryItemSelector?.Invoke(item) ?? true))
		{
			return 0.0;
		}

		if (executor.Body.HeldOrWieldedItems.Contains(item))
		{
			if (ItemsAlreadyInPlaceOverrideFitnessScore)
			{
				switch (Options)
				{
					case AttackHandednessOptions.Any:
						return double.MaxValue;
					case AttackHandednessOptions.OneHandedOnly:
						if (executor.Body.WieldedHandCount(item) == 1 ||
						    executor.Body.CanWield(
							    item, ItemCanWieldFlags.IgnoreFreeHands | ItemCanWieldFlags.RequireOneHand))
						{
							return double.MaxValue;
						}

						break;
					case AttackHandednessOptions.TwoHandedOnly:
						if (executor.Body.WieldedHandCount(item) == 2 ||
						    executor.Body.CanWield(
							    item, ItemCanWieldFlags.IgnoreFreeHands | ItemCanWieldFlags.RequireTwoHands))
						{
							return double.MaxValue;
						}

						break;
				}
			}
			else
			{
				return (PrimaryItemFitnessScorer?.Invoke(item) ?? 1.0) * ItemsAlreadyInPlaceMultiplier;
			}
		}

		return PrimaryItemFitnessScorer?.Invoke(item) ?? 1.0;
	}

	public override IGameItem ScoutSecondary(ICharacter executor, IGameItem item)
	{
		return null;
	}

	private bool CanWield(ICharacter ch, IGameItem item)
	{
		switch (_attackHandednessEvaluator?.Invoke(ch, item) ?? AttackHandednessOptions.Any)
		{
			case AttackHandednessOptions.Any:
				return ch.Body.CanWield(item, ItemCanWieldFlags.IgnoreFreeHands);
			case AttackHandednessOptions.OneHandedOnly:
				return ch.Body.WieldedHandCount(item) == 1 ||
				       ch.Body.CanWield(
					       item, ItemCanWieldFlags.IgnoreFreeHands | ItemCanWieldFlags.RequireOneHand);
			case AttackHandednessOptions.TwoHandedOnly:
				return ch.Body.WieldedHandCount(item) == 2 ||
				       ch.Body.CanWield(
					       item, ItemCanWieldFlags.IgnoreFreeHands | ItemCanWieldFlags.RequireTwoHands);
		}

		return false;
	}

	private bool CanDraw(ICharacter ch, IGameItem item)
	{
		switch (_attackHandednessEvaluator?.Invoke(ch, item) ?? AttackHandednessOptions.Any)
		{
			case AttackHandednessOptions.Any:
				return ch.Body.CanDraw(item, null, ItemCanWieldFlags.IgnoreFreeHands);
			case AttackHandednessOptions.OneHandedOnly:
				return ch.Body.CanDraw(item, null,
					ItemCanWieldFlags.IgnoreFreeHands | ItemCanWieldFlags.RequireOneHand);
			case AttackHandednessOptions.TwoHandedOnly:
				return ch.Body.CanDraw(item, null,
					ItemCanWieldFlags.IgnoreFreeHands | ItemCanWieldFlags.RequireTwoHands);
		}

		return false;
	}

	public override IGameItem ScoutTarget(ICharacter executor)
	{
		var potentialItems = new List<(IGameItem Item, double Fitness, int InternalOrder)>();
		var internalOrder = 0;
		// Already wielded items first
		var items =
			executor.Body.WieldedItems.Where(
				x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && CanWield(executor, x));
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
				x => x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) && x.IsItemType<IWieldable>() &&
				     CanWield(executor, x));
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
			                    (PrimaryItemSelector?.Invoke(x) ?? true) &&
			                    CanDraw(executor, x)
			        );
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
					x.IsA(DesiredTag) &&
					(PrimaryItemSelector?.Invoke(x) ?? true) &&
					x.IsItemType<IWieldable>() &&
					executor.Body.CanRemoveItem(x, ItemCanGetIgnore.IgnoreFreeHands) &&
					CanWield(executor, x));
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
							        y.Parent.IsItemType<IWieldable>() &&
							        CanWield(executor, y.Parent)
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
					CanWield(executor, x)
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
					(PrimaryItemSelector?.Invoke(x) ?? true) &&
					x.IsItemType<IWieldable>() &&
					CanWield(executor, x) &&
					x.IsItemType<IHoldable>() &&
					x.GetItemType<IHoldable>().IsHoldable);
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
							        y.Parent.IsItemType<IWieldable>() &&
							        CanWield(executor, y.Parent)
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
			                    (PrimaryItemSelector?.Invoke(x) ?? true) &&
			                    CanDraw(executor, x));
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
					CanWield(executor, x)
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

		var scoutedItem = potentialItems.OrderByDescending(x => x.Fitness).ThenBy(x => x.InternalOrder).FirstOrDefault()
		                                .Item;
		if (_attackHandednessEvaluator != null)
		{
			Options = _attackHandednessEvaluator.Invoke(executor, scoutedItem);
		}

		switch (Options)
		{
			case AttackHandednessOptions.OneHandedOnly:
				DesiredState = DesiredItemState.WieldedOneHandedOnly;
				break;
			case AttackHandednessOptions.TwoHandedOnly:
				DesiredState = DesiredItemState.WieldedTwoHandedOnly;
				break;
		}

		return scoutedItem;
	}
}