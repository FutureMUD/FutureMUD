using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class ShopkeeperAI : PathingAIBase
{
	public static void RegisterLoader()
	{
		RegisterAIType("Shopkeeper", (ai, gameworld) => new ShopkeeperAI(ai, gameworld));
	}

	private IFutureProg _onSomeoneEntersProg;
	private IFutureProg _onSomeoneUnwelcomeEntersProg;
	private IFutureProg _onSomeoneBuysProg;
	private IFutureProg _onLeaveForRestockProg;
	private IFutureProg _onArriveBackFromRestockProg;
	private TimeSpan _restockStartDelay;

	protected ShopkeeperAI(Models.ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		UseKeys = true;
		SmashLockedDoors = false;
		MoveEvenIfObstructionInWay = true;
		UseDoorguards = true;

		var root = XElement.Parse(ai.Definition);
		_onSomeoneEntersProg = long.TryParse(root.Element("OnSomeoneEntersProg")?.Value ?? "0", out var value)
			? gameworld.FutureProgs.Get(value)
			: gameworld.FutureProgs.GetByName(root.Element("OnSomeoneEntersProg").Value);
		_onSomeoneUnwelcomeEntersProg =
			long.TryParse(root.Element("OnSomeoneUnwelcomeEntersProg")?.Value ?? "0", out value)
				? gameworld.FutureProgs.Get(value)
				: gameworld.FutureProgs.GetByName(root.Element("OnSomeoneUnwelcomeEntersProg").Value);
		_onSomeoneBuysProg = long.TryParse(root.Element("OnSomeoneBuysProg")?.Value ?? "0", out value)
			? gameworld.FutureProgs.Get(value)
			: gameworld.FutureProgs.GetByName(root.Element("OnSomeoneBuysProg").Value);
		_onLeaveForRestockProg = long.TryParse(root.Element("OnLeaveForRestockProg")?.Value ?? "0", out value)
			? gameworld.FutureProgs.Get(value)
			: gameworld.FutureProgs.GetByName(root.Element("OnLeaveForRestockProg").Value);
		_onArriveBackFromRestockProg =
			long.TryParse(root.Element("OnArriveBackFromRestockProg")?.Value ?? "0", out value)
				? gameworld.FutureProgs.Get(value)
				: gameworld.FutureProgs.GetByName(root.Element("OnArriveBackFromRestockProg").Value);
		_restockStartDelay =
			TimeSpan.FromMilliseconds(double.Parse(root.Element("RestockStartDelay")?.Value ?? "2000"));
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.CharacterEnterCellFinish:
				return HandleCharacterEnterCell((ICharacter)arguments[0], (ICell)arguments[1]) ||
				       base.HandleEvent(type, arguments);
			case EventType.CharacterEnterCellFinishWitness:
				return HandleWitnessCharacterEnterCell((ICharacter)arguments[0], (ICell)arguments[1],
					(ICellExit)arguments[2], (ICharacter)arguments[3]);
			case EventType.WitnessBuyItemInShop:
				return HandleWitnessBuyItem((ICharacter)arguments[0], (ICharacter)arguments[1], (IShop)arguments[2],
					(IMerchandise)arguments[3], (IEnumerable<IGameItem>)arguments[4]);
			case EventType.ItemRequiresRestocking:
				return HandleItemRequiresRestocking((ICharacter)arguments[0], (IShop)arguments[1],
					(IMerchandise)arguments[2], (int)arguments[3]);
			case EventType.MinuteTick:
				return HandleMinuteTick((ICharacter)arguments[0]) || base.HandleEvent(type, arguments);
			case EventType.HourTick:
				return HandleHourTick((ICharacter)arguments[0]);
			default:
				return base.HandleEvent(type, arguments);
		}
	}

	private bool HandleHourTick(ICharacter employee)
	{
		if (employee.Location.Shop?.IsClockedIn(employee) == true)
		{
			var stockInRoom = employee.Body.HeldItems
			                          .Concat(employee.Location.GameItems.SelectMany(x => x.ShallowItems))
			                          .Where(x => x.AffectedBy<ItemOnDisplayInShop>()).ToList();

			foreach (var item in stockInRoom)
			{
				var effect = item.EffectsOfType<ItemOnDisplayInShop>().First();
				if (effect.Merchandise.PreferredDisplayContainer != null &&
				    item.ContainedIn != effect.Merchandise.PreferredDisplayContainer &&
				    employee.Body.CanPut(item, effect.Merchandise.PreferredDisplayContainer, null, 0, true) &&
				    item.ContainedIn != null
					    ? employee.Body.CanGet(item, item.ContainedIn, 0)
					    : employee.Body.CanGet(item, 0)
				   )
				{
					if (item.ContainedIn != null)
					{
						employee.Body.Get(item, item.ContainedIn);
					}
					else
					{
						employee.Body.Get(item);
					}

					employee.Body.Put(item, effect.Merchandise.PreferredDisplayContainer, null);
					continue;
				}

				if (item.ContainedIn == null && employee.Body.CanGet(item, 0) &&
				    employee.Location.Shop.DisplayContainers.Any(x =>
					    x.Location == employee.Location && employee.Body.CanPut(item, x, null, 0, false)))
				{
					employee.Body.Get(item);
					employee.Body.Put(item,
						employee.Location.Shop.DisplayContainers.First(x =>
							x.Location == employee.Location && employee.Body.CanPut(item, x, null, 0, false)), null);
				}
			}

			return true;
		}

		return false;
	}

	private bool HandleMinuteTick(ICharacter employee)
	{
		// TODO - check restocks that couldn't happen before
		return false;
	}

	private bool HandleItemRequiresRestocking(ICharacter employee, IShop shop, IMerchandise merchandise, int quantity)
	{
		if (employee.AffectedBy<RestockingMerchandise>())
		{
			if (employee.EffectsOfType<RestockingMerchandise>().First().TargetMerchandise == merchandise)
			{
				employee.EffectsOfType<RestockingMerchandise>().First().QuantityToRestock += quantity;
				return true;
			}

			return false;
		}

		if (shop.EmployeesOnDuty.Any(x => x.EffectsOfType<RestockingMerchandise>()
		                                   .Any(y => y.TargetMerchandise == merchandise)))
		{
			return false;
		}

		var effect = new RestockingMerchandise(employee, merchandise, quantity, _onLeaveForRestockProg);
		employee.AddEffect(effect);
		employee.AddEffect(
			new DelayedAction(employee, perc => CheckPathingEffect(employee, true), "Getting ready to restock"),
			_restockStartDelay);
		return true;
	}

	private bool HandleWitnessBuyItem(ICharacter customer, ICharacter employee, IShop shop, IMerchandise merchandise,
		IEnumerable<IGameItem> items)
	{
		_onSomeoneBuysProg?.Execute(employee, customer, shop, merchandise, items);
		return false;
	}

	private bool HandleWitnessCharacterEnterCell(ICharacter customer, ICell cell, ICellExit cellExit,
		ICharacter employee)
	{
		if (cell.Shop?.IsClockedIn(employee) != true)
		{
			return false;
		}

		if (cell.Shop.IsClockedIn(customer))
		{
			return false;
		}

		if (cell.Shop.IsWelcomeCustomer(customer))
		{
			_onSomeoneEntersProg?.Execute(employee, customer, cellExit);
		}
		else
		{
			_onSomeoneUnwelcomeEntersProg?.Execute(employee, customer, cellExit);
		}

		return false;
	}

	private bool HandleCharacterEnterCell(ICharacter employee, ICell cell)
	{
		var effect = employee.EffectsOfType<RestockingMerchandise>().FirstOrDefault();
		if (effect == null)
		{
			return false;
		}

		if (effect.CurrentGameItems.Any())
		{
			if (employee.AffectedBy<FollowingPath>())
			{
				return false;
			}

			foreach (var item in employee.Body.HeldItems.Where(
				         x => x.AffectedBy<ItemOnDisplayInShop>(effect.TargetMerchandise)).ToList())
			{
				if (effect.TargetMerchandise.PreferredDisplayContainer != null &&
				    effect.TargetMerchandise.PreferredDisplayContainer.Location == employee.Location &&
				    employee.Body.CanPut(item, effect.TargetMerchandise.PreferredDisplayContainer, null, 0, false))
				{
					employee.Body.Put(item, effect.TargetMerchandise.PreferredDisplayContainer, null);
					continue;
				}

				if (effect.TargetMerchandise.Shop.DisplayContainers.FirstOrDefault(
					    x => x.Location == employee.Location && employee.Body.CanPut(item, x, null, 0, false)) is
				    IGameItem container)
				{
					employee.Body.Put(item, container, null);
					continue;
				}

				effect.QuantityToRestock -= item.Quantity;
				employee.Body.Drop(item);
				effect.CurrentGameItems.Remove(item);
			}

			if (effect.QuantityToRestock > 0)
			{
				CheckPathingEffect(employee, true);
				return true;
			}

			employee.RemoveEffect(effect);
			return true;
		}

		if (cell != effect.TargetMerchandise.Shop.StockroomCell)
		{
			return false;
		}

		var plan = new InventoryPlanTemplate(Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
				{
					new InventoryPlanActionHold(Gameworld, 0, 0,
						item => item.AffectedBy<ItemOnDisplayInShop>(
							effect.TargetMerchandise), null, 1)
					{
						ItemsAlreadyInPlaceMultiplier = 0.0,
						OriginalReference = "target"
					}
				}
			)
		}).CreatePlan(employee);

		var quantity = 0;
		while (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			var results = plan.ExecuteWholePlan();
			var item = results.First(x => x.OriginalReference.Equals("target")).PrimaryTarget;
			quantity += item.Quantity;
			effect.CurrentGameItems.Add(item);
			if (quantity >= effect.QuantityToRestock)
			{
				break;
			}
		}

		plan.FinalisePlanNoRestore();
		CreatePathingEffect(employee);
		return true;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return types.Any(type =>
		{
			switch (type)
			{
				case EventType.CharacterEnterCellFinish:
				case EventType.CharacterEnterCellFinishWitness:
				case EventType.WitnessBuyItemInShop:
				case EventType.ItemRequiresRestocking:
				case EventType.MinuteTick:
				case EventType.HourTick:
					return true;
				default:
					return false;
			}
		}) || base.HandlesEvent(types);
	}

	#region Overrides of PathingAIBase

	protected override bool IsPathingEnabled(ICharacter character)
	{
		var effect = character.EffectsOfType<RestockingMerchandise>().FirstOrDefault();
		if (effect == null)
		{
			return false;
		}

		return effect.CellExitQueue.Count > 0;
	}

	#endregion

	protected override IEnumerable<ICellExit> GetPath(ICharacter ch)
	{
		var effect = ch.EffectsOfType<RestockingMerchandise>().First();
		var shop = effect.TargetMerchandise.Shop;
		if (effect.CurrentGameItems.Any())
		{
			if (effect.TargetMerchandise.PreferredDisplayContainer != null)
			{
				var path = ch.PathBetween(
					effect.TargetMerchandise.PreferredDisplayContainer.LocationLevelPerceivable, 10,
					GetSuitabilityFunction(ch)).ToList();
				if (path.Any())
				{
					return path;
				}
			}

			return ch.PathBetween(
				shop.ShopfrontCells.WhereMin(x => x.Characters.Count(y => shop.IsClockedIn(y))).GetRandomElement(),
				10, GetSuitabilityFunction(ch));
		}

		if (shop?.StockroomCell == null || ch.Location == shop.StockroomCell)
		{
			return Enumerable.Empty<ICellExit>();
		}

		return ch.PathBetween(shop.StockroomCell, 10, GetSuitabilityFunction(ch))
		         .ToList();
	}
}