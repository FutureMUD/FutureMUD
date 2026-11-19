#nullable enable annotations

using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.NPC.AI;

public class ShopkeeperAI : PathingAIBase
{
    public static void RegisterLoader()
    {
        RegisterAIType("Shopkeeper", (ai, gameworld) => new ShopkeeperAI(ai, gameworld));
        RegisterAIBuilderInformation("shopkeeper", (game, name) => new ShopkeeperAI(game, name), new ShopkeeperAI().HelpText);
    }

    private IFutureProg _onSomeoneEntersProg;
    private IFutureProg _onSomeoneUnwelcomeEntersProg;
    private IFutureProg _onSomeoneBuysProg;
    private IFutureProg _onLeaveForRestockProg;
    private IFutureProg _onArriveBackFromRestockProg;
    private TimeSpan _restockStartDelay;

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("OnSomeoneEntersProg", _onSomeoneEntersProg?.Id ?? 0L),
            new XElement("RestockStartDelay", _restockStartDelay.TotalMilliseconds),
            new XElement("OnSomeoneUnwelcomeEntersProg", _onSomeoneUnwelcomeEntersProg?.Id ?? 0L),
            new XElement("OnSomeoneBuysProg", _onSomeoneBuysProg?.Id ?? 0L),
            new XElement("OnLeaveForRestockProg", _onLeaveForRestockProg?.Id ?? 0L),
            new XElement("OnArriveBackFromRestockProg", _onArriveBackFromRestockProg?.Id ?? 0L),
            new XElement("OpenDoors", OpenDoors),
            new XElement("UseKeys", UseKeys),
            new XElement("SmashLockedDoors", SmashLockedDoors),
            new XElement("CloseDoorsBehind", CloseDoorsBehind),
            new XElement("UseDoorguards", UseDoorguards),
            new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay)
        ).ToString();
    }

    private ShopkeeperAI()
    {
    }

    protected ShopkeeperAI(IFuturemud gameworld, string name) : base(gameworld, name, "Shopkeeper")
    {
        OpenDoors = true;
        UseKeys = true;
        SmashLockedDoors = false;
        MoveEvenIfObstructionInWay = true;
        UseDoorguards = true;
        _restockStartDelay = TimeSpan.FromSeconds(30);
        DatabaseInitialise();
    }

    protected ShopkeeperAI(Models.ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
    {
        XElement root = XElement.Parse(ai.Definition);
        OpenDoors = bool.Parse(root.Element("OpenDoors").Value);
        UseKeys = bool.Parse(root.Element("UseKeys").Value);
        SmashLockedDoors = bool.Parse(root.Element("SmashLockedDoors").Value);
        MoveEvenIfObstructionInWay = bool.Parse(root.Element("MoveEvenIfObstructionInWay").Value);
        UseDoorguards = bool.Parse(root.Element("UseDoorguards").Value);
        _onSomeoneEntersProg = long.TryParse(root.Element("OnSomeoneEntersProg")?.Value ?? "0", out long value)
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
            TimeSpan.FromMilliseconds(double.Parse(root.Element("RestockStartDelay")?.Value ?? root.Element("EngageEmote")?.Value ?? "2000"));
    }

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        switch (type)
        {
            case EventType.CharacterEnterRoomFinish:
                return base.HandleEvent(type, arguments) || ProgressRestock((ICharacter)arguments[0]);
            case EventType.CharacterEnterRoomFinishWitness:
                return HandleWitnessCharacterEnterRoom((ICharacter)arguments[0], (IRoom)arguments[1],
                    (IRoomExit)arguments[2], (ICharacter)arguments[3]);
            case EventType.WitnessBuyItemInShop:
                return HandleWitnessBuyItem((ICharacter)arguments[0], (ICharacter)arguments[1], (IShop)arguments[2],
                    (IMerchandise)arguments[3], (IEnumerable<IGameItem>)arguments[4]);
            case EventType.ItemRequiresRestocking:
                return arguments[1] is IPermanentShop permanentShop &&
                       HandleItemRequiresRestocking((ICharacter)arguments[0], permanentShop,
                           (IMerchandise)arguments[2], (int)arguments[3]);
            case EventType.MinuteTick:
                return ProgressRestock((ICharacter)arguments[0]) || base.HandleEvent(type, arguments);
            case EventType.HourTick:
                return HandleHourTick((ICharacter)arguments[0]);
            default:
                return base.HandleEvent(type, arguments);
        }
    }

    private bool HandleHourTick(ICharacter employee)
    {
        if (!employee.AffectedBy<RestockingMerchandise>() && employee.Location.Shop?.IsClockedIn(employee) == true)
        {
            List<IGameItem> stockInRoom = employee.Body.HeldItems
                                      .Concat(employee.Location.GameItems.SelectMany(x => x.ShallowItems))
                                      .Where(x => x.AffectedBy<ItemOnDisplayInShop>()).ToList();

            foreach (IGameItem item in stockInRoom)
            {
                ItemOnDisplayInShop effect = item.EffectsOfType<ItemOnDisplayInShop>().First();
                var preferred = effect.Merchandise.PreferredDisplayContainer;
                if (preferred is not null && preferred.Location == employee.Location && item.ContainedIn != preferred &&
                    employee.Body.CanPut(item, preferred, null, 0, true) &&
                    (item.ContainedIn is not null
                        ? employee.Body.CanGet(item, item.ContainedIn, 0)
                        : employee.Body.CanGet(item, 0)))
                {
                    if (item.ContainedIn != null)
                    {
                        employee.Body.Get(item, item.ContainedIn);
                    }
                    else
                    {
                        employee.Body.Get(item);
                    }

                    employee.Body.Put(item, preferred, null);
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

    private bool HandleItemRequiresRestocking(ICharacter employee, IPermanentShop shop, IMerchandise merchandise, int quantity)
    {
        if (quantity <= 0 || shop.StockroomRoom is null || !shop.IsClockedIn(employee) ||
            !ReferenceEquals(shop, merchandise.Shop))
        {
            return false;
        }

        var pending = employee.EffectsOfType<RestockingMerchandise>().FirstOrDefault();
        if (pending is not null)
        {
            if (pending.TargetMerchandise == merchandise)
            {
                pending.QuantityToRestock += quantity;
                return true;
            }

            return false;
        }

        if (shop.EmployeesOnDuty.Any(x => x.EffectsOfType<RestockingMerchandise>()
                                           .Any(y => y.TargetMerchandise == merchandise)))
        {
            return false;
        }

        RestockingMerchandise effect = new(employee, merchandise, quantity, _onLeaveForRestockProg);
        employee.AddEffect(effect);
        employee.AddEffect(
            new DelayedAction(employee, _ =>
            {
                effect.Ready = true;
                ProgressRestock(employee);
            }, "Getting ready to restock"),
            _restockStartDelay);
        return true;
    }

    private bool HandleWitnessBuyItem(ICharacter customer, ICharacter employee, IShop shop, IMerchandise merchandise,
        IEnumerable<IGameItem> items)
    {
        _onSomeoneBuysProg?.Execute(employee, customer, shop, merchandise, items);
        return false;
    }

    private bool HandleWitnessCharacterEnterRoom(ICharacter customer, IRoom room, IRoomExit cellExit,
        ICharacter employee)
    {
        if (room.Shop?.IsClockedIn(employee) != true)
        {
            return false;
        }

        if (room.Shop.IsClockedIn(customer))
        {
            return false;
        }

        if (room.Shop.IsWelcomeCustomer(customer))
        {
            _onSomeoneEntersProg?.Execute(employee, customer, cellExit);
        }
        else
        {
            _onSomeoneUnwelcomeEntersProg?.Execute(employee, customer, cellExit);
        }

        return false;
    }

    private bool ProgressRestock(ICharacter employee)
    {
        var request = employee.EffectsOfType<RestockingMerchandise>().FirstOrDefault();
        if (request?.Ready != true || !employee.State.IsAble() || employee.State.IsInStatis() ||
            employee.Movement is not null ||
            employee.Combat is not null || employee.AffectedBy<FollowingPath>() ||
            employee.Effects.Any(x => x.IsBlockingEffect("movement") || x.IsBlockingEffect("combat-engage")))
        {
            return false;
        }

        if (request.TargetMerchandise.Shop is not IPermanentShop shop)
        {
            employee.RemoveEffect(request);
            return false;
        }

        if (request.CarriedItem is { } item && !employee.Body.HeldItems.Any(x => ReferenceEquals(x, item)))
        {
            request.CarriedItem = null;
            request.CarriedQuantity = 0;
        }

        // Finish delivering anything already in hand, even if the shift ended.
        if (request.CarriedItem is null && (!shop.IsClockedIn(employee) || shop.StockroomRoom is null))
        {
            employee.RemoveEffect(request);
            return false;
        }

        if (request.CarriedItem is not null)
        {
            if (employee.Location == request.DisplayRoom && shop.ShopfrontRooms.Contains(employee.Location))
            {
                if (!DeliverStock(employee, request, shop))
                {
                    return false;
                }

                if (request.QuantityToRestock <= 0 || !shop.IsClockedIn(employee) || shop.StockroomRoom is null)
                {
                    employee.RemoveEffect(request);
                    return true;
                }
            }
        }
        else if (employee.Location == shop.StockroomRoom)
        {
            var stock = StockroomItems(request.TargetMerchandise, shop).ToList();
            if (stock.Count == 0)
            {
                employee.RemoveEffect(request);
                return true;
            }

            var route = FindDisplayPath(employee, request.TargetMerchandise, shop);
            if (route is null)
            {
                return false;
            }

            request.DisplayRoom = route.Room;
            if (!PickUpStock(employee, request, stock))
            {
                return false;
            }

            if (employee.Location == route.Room)
            {
                DeliverStock(employee, request, shop);
                if (request.QuantityToRestock <= 0)
                {
                    employee.RemoveEffect(request);
                }

                return true;
            }
        }

        CheckPathingEffect(employee, true);
        return true;
    }

    private static IEnumerable<IGameItem> StockroomItems(IMerchandise merchandise, IPermanentShop shop)
    {
        var stockroom = shop.StockroomRoom;
        var displayContainers = shop.DisplayContainers.ToHashSet();
        if (merchandise.PreferredDisplayContainer is { } preferred)
        {
            displayContainers.Add(preferred);
        }

        return stockroom.GameItems.SelectMany(x => x.DeepItems)
            .Where(x => x.Quantity > 0 && x.AffectedBy<ItemOnDisplayInShop>(merchandise))
            .Where(x => x.ContainedIn is null || !displayContainers.Contains(x.ContainedIn))
            .Distinct();
    }

    private static bool PickUpStock(ICharacter employee, RestockingMerchandise request, IEnumerable<IGameItem> stock)
    {
        foreach (var item in stock)
        {
            int quantity = Math.Min(request.QuantityToRestock, item.Quantity);
            if (quantity <= 0)
            {
                continue;
            }

            var held = item.ContainedIn is { } container
                ? employee.Body.Get(item, container, quantity, null, false, ItemCanGetIgnore.None, null)
                : employee.Body.Get(item, quantity, null, false, ItemCanGetIgnore.None, null);
            if (held is null || !employee.Body.HeldItems.Any(x => ReferenceEquals(x, held)))
            {
                continue;
            }

            request.CarriedItem = held;
            request.CarriedQuantity = quantity;
            return true;
        }

        return false;
    }

    private static bool DeliverStock(ICharacter employee, RestockingMerchandise request, IPermanentShop shop)
    {
        var item = request.CarriedItem;
        if (item is null)
        {
            return false;
        }

        int quantity = Math.Min(item.Quantity, request.CarriedQuantity);
        var preferred = request.TargetMerchandise.PreferredDisplayContainer;
        var containers = shop.DisplayContainers.Where(x => x != preferred);
        if (preferred is not null)
        {
            containers = containers.Prepend(preferred);
        }

        foreach (var container in containers.Where(x => x.Location == employee.Location))
        {
            if (employee.Body.CanPut(item, container, null, quantity, false) &&
                employee.Body.Put(item, container, null, quantity, null, false, false, null) is not null)
            {
                FinishDelivery(request, quantity);
                return true;
            }
        }

        if (!employee.Body.CanDrop(item, quantity))
        {
            return false;
        }

        int beforeDrop = item.Quantity;
        employee.Body.Drop(item, quantity);
        if (employee.Body.HeldItems.Any(x => ReferenceEquals(x, item)) && item.Quantity > beforeDrop - quantity)
        {
            return false;
        }

        FinishDelivery(request, quantity);
        return true;
    }

    private static void FinishDelivery(RestockingMerchandise request, int quantity)
    {
        request.QuantityToRestock -= quantity;
        request.CarriedItem = null;
        request.CarriedQuantity = 0;
    }

    public override bool HandlesEvent(params EventType[] types)
    {
        return types.Any(type =>
        {
            switch (type)
            {
                case EventType.CharacterEnterRoomFinish:
                case EventType.CharacterEnterRoomFinishWitness:
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

    protected override bool IsPathingEnabled(ICharacter character)
    {
        var request = character.EffectsOfType<RestockingMerchandise>().FirstOrDefault();
        return request?.Ready == true && request.TargetMerchandise.Shop is IPermanentShop shop &&
               (request.CarriedItem is not null || (shop.IsClockedIn(character) && shop.StockroomRoom is not null));
    }

    protected override (IRoom? Target, IEnumerable<IRoomExit>) GetPath(ICharacter ch)
    {
        var request = ch.EffectsOfType<RestockingMerchandise>().FirstOrDefault();
        if (request?.Ready != true || request.TargetMerchandise.Shop is not IPermanentShop shop)
        {
            return (null, []);
        }

        if (request.CarriedItem is not null)
        {
            var route = FindDisplayPath(ch, request.TargetMerchandise, shop);
            request.DisplayRoom = route?.Room;
            return route is null || route.SpatialPath is not null
                ? (null, [])
                : (route.Room, route.Exits);
        }

        return shop.StockroomRoom is { } stockroom && stockroom != ch.Location
            ? (stockroom, ch.PathBetween(stockroom, 10, GetSuitabilityFunction(ch)))
            : (null, []);
    }

    protected override (IRoom? Target, ISpatialPath? Path) GetSpatialPath(ICharacter ch)
    {
        var request = ch.EffectsOfType<RestockingMerchandise>().FirstOrDefault();
        if (request?.CarriedItem is null || request.TargetMerchandise.Shop is not IPermanentShop shop)
        {
            return (null, null);
        }

        var route = FindDisplayPath(ch, request.TargetMerchandise, shop);
        return (route?.Room, route?.SpatialPath);
    }

    private sealed record DisplayRoute(IRoom Room, IReadOnlyList<IRoomExit> Exits, ISpatialPath? SpatialPath = null);

    private DisplayRoute? FindDisplayPath(ICharacter employee, IMerchandise merchandise, IPermanentShop shop)
    {
        var shopRooms = shop.AllShopRooms.ToHashSet();
        var canUseExit = GetSuitabilityFunction(employee);
        var preferred = merchandise.PreferredDisplayContainer?.LocationLevelPerceivable?.Location;
        bool ShopExit(IRoomExit exit) => shopRooms.Contains(exit.Destination) && canUseExit(exit);
        bool hasRouteRooms = shopRooms.Any(x => x.RouteDefinition is not null);
        foreach (var room in shop.ShopfrontRooms
                     .OrderByDescending(x => x == preferred)
                     .ThenBy(x => x.Characters.Count(shop.IsClockedIn)))
        {
            if (room == employee.Location)
            {
                return new DisplayRoute(room, []);
            }

            var route = employee.PathBetween(room, 10, ShopExit).ToList();
            if (route.Count > 0)
            {
                return new DisplayRoute(room, route);
            }

            if (hasRouteRooms && TryFindSpatialPath(employee, room, 10.0, ShopExit, out var spatial) &&
                spatial is not null && RequiresSpatialFollowing(spatial))
            {
                return new DisplayRoute(room, [], spatial);
            }
        }

        return null;
    }
}
