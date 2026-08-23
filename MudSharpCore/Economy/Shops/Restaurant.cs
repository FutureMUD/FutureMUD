using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Payment;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.RPG.Law;
using MudSharp.Work.Crafts;
using System.Text;
using DbRestaurant = MudSharp.Models.Restaurant;
using DbRestaurantCell = MudSharp.Models.RestaurantCell;
using DbRestaurantStorageContainer = MudSharp.Models.RestaurantStorageContainer;
using DbRestaurantTable = MudSharp.Models.RestaurantTable;

#nullable enable

namespace MudSharp.Economy.Shops;

/// <summary>
/// A permanent shop with table sessions and a made-to-order service queue. Stock, payment,
/// taxes and employment deliberately remain inherited shop responsibilities; this type owns only
/// restaurant-specific configuration and operational state.
/// </summary>
public sealed class Restaurant : PermanentShop, IRestaurant
{
	private sealed record RestaurantCellAssignment(ICell Cell, RestaurantCellRole Role);

	private sealed class PendingJoinRequest
	{
		public required Guid Id { get; init; }
		public required long SessionId { get; init; }
		public required long RequesterCharacterId { get; init; }
		public required string RequesterName { get; init; }
		public required HashSet<long> ApproverCharacterIds { get; init; }
		public bool Resolved { get; set; }
	}

	/// <summary>
	/// Tracks a live NPC-owned craft without persisting transient active-craft state. Server recovery
	/// already marks preparing orders as recoverable failures, so this state intentionally vanishes
	/// with the process rather than leaving an order orphaned after a restart.
	/// </summary>
	private sealed record NpcCraftJob(long OrderId, long CraftId, int CraftRevisionNumber,
		IReadOnlySet<long> PreExistingItemIds,
		IReadOnlyDictionary<long, int> PreExistingOutputQuantities);

	/// <summary>
	/// A service container supplied from a configured kitchen store. A null source denotes a
	/// container created for this service operation and therefore safe to delete on rollback.
	/// </summary>
	private sealed record KitchenContainerAllocation(IGameItem Item, IContainer? SourceStorage);

	private readonly List<RestaurantCellAssignment> _cells = new();
	private readonly HashSet<long> _tableIds = new();
	private readonly List<IRestaurantMenuItem> _menuItems = new();
	private readonly List<IRestaurantTableSession> _tableSessions = new();
	private readonly List<IRestaurantOrder> _orders = new();
	private readonly List<IRestaurantStorageContainer> _storageContainers = new();
	private readonly Dictionary<Guid, PendingJoinRequest> _pendingJoinRequests = new();
	private readonly Dictionary<long, NpcCraftJob> _npcCraftJobs = new();
	private bool _automatedService;
	private bool _simulateCrafting;
	private TimeSpan _handlingTime;
	private TimeSpan _maximumBatchWait;
	private TimeSpan _tableCleanupInterval;
	private DateTime? _lastTableCleanupSweepAtUtc;
	private string _chefStartEmote = RestaurantServiceEmotes.DefaultChefStart;
	private string _chefOpenEmote = RestaurantServiceEmotes.DefaultChefOpen;
	private string _chefPlateEmote = RestaurantServiceEmotes.DefaultChefPlate;
	private string _chefReadyEmote = RestaurantServiceEmotes.DefaultChefReady;
	private string _serverServeEmote = RestaurantServiceEmotes.DefaultServerServe;
	private string _serverClearEmote = RestaurantServiceEmotes.DefaultServerClear;
	private string _serverReturnEmote = RestaurantServiceEmotes.DefaultServerReturn;
	private long? _takeawayBagPrototypeId;
	private int? _takeawayBagPrototypeRevisionNumber;
	private IGameItemProto? _takeawayBagPrototype;
	private bool _heartbeatAttached;

	public Restaurant(Models.Shop shop, IFuturemud gameworld) : base(shop, gameworld)
	{
		var config = shop.Restaurant ?? throw new ApplicationException(
			$"Restaurant shop #{shop.Id:N0} ({shop.Name}) has no restaurant configuration.");
		_automatedService = config.AutomatedService;
		_simulateCrafting = config.SimulateCrafting;
		_handlingTime = TimeSpan.FromSeconds(Math.Max(0, config.HandlingSeconds));
		_maximumBatchWait = TimeSpan.FromSeconds(Math.Max(0, config.MaximumBatchWaitSeconds));
		_tableCleanupInterval = TimeSpan.FromSeconds(Math.Max(0, config.CleanupIntervalSeconds));
		_chefStartEmote = RestaurantServiceEmotes.Normalize(RestaurantServiceEmoteType.ChefStart, config.ChefStartEmote);
		_chefOpenEmote = RestaurantServiceEmotes.Normalize(RestaurantServiceEmoteType.ChefOpen, config.ChefOpenEmote);
		_chefPlateEmote = RestaurantServiceEmotes.Normalize(RestaurantServiceEmoteType.ChefPlate, config.ChefPlateEmote);
		_chefReadyEmote = RestaurantServiceEmotes.Normalize(RestaurantServiceEmoteType.ChefReady, config.ChefReadyEmote);
		_serverServeEmote = RestaurantServiceEmotes.Normalize(RestaurantServiceEmoteType.ServerServe, config.ServerServeEmote);
		_serverClearEmote = RestaurantServiceEmotes.Normalize(RestaurantServiceEmoteType.ServerClear, config.ServerClearEmote);
		_serverReturnEmote = RestaurantServiceEmotes.Normalize(RestaurantServiceEmoteType.ServerReturn, config.ServerReturnEmote);
		_takeawayBagPrototypeId = config.TakeawayBagPrototypeId;
		_takeawayBagPrototypeRevisionNumber = config.TakeawayBagPrototypeRevisionNumber;

		foreach (var assignment in config.Cells)
		{
			var cell = gameworld.Cells.Get(assignment.CellId);
			if (cell is null)
			{
				continue;
			}

			_cells.Add(new RestaurantCellAssignment(cell, (RestaurantCellRole)assignment.Role));
			if (!ShopfrontCells.Contains(cell))
			{
				AddShopfrontCell(cell);
			}
		}

		if (!_cells.Any())
		{
			foreach (var cell in ShopfrontCells)
			{
				_cells.Add(new RestaurantCellAssignment(cell, RestaurantCellRole.Service));
			}
		}

		foreach (var table in config.Tables)
		{
			_tableIds.Add(table.GameItemId);
		}

		foreach (var storage in config.StorageContainers)
		{
			_storageContainers.Add(new RestaurantStorageContainer(storage, this));
		}

		foreach (var item in config.MenuItems.OrderBy(x => x.SortOrder).ThenBy(x => x.Id))
		{
			if (!Merchandises.Any(x => x.Id == item.MerchandiseId))
			{
				continue;
			}

			_menuItems.Add(new RestaurantMenuItem(item, this));
		}

		var sessions = config.TableSessions
			.OrderBy(x => x.Id)
			.Select(x => new RestaurantTableSession(x, this))
			.ToList();
		_tableSessions.AddRange(sessions);
		var menus = _menuItems.OfType<RestaurantMenuItem>().ToDictionary(x => x.Id);
		var sessionsById = sessions.ToDictionary(x => x.Id);
		foreach (var order in config.Orders.OrderBy(x => x.Id))
		{
			if (!menus.TryGetValue(order.RestaurantMenuItemId, out var menu))
			{
				continue;
			}

			sessionsById.TryGetValue(order.RestaurantTableSessionId ?? 0, out var session);
			var loaded = new RestaurantOrder(order, this, session, menu);
			_orders.Add(loaded);
			session?.AddOrder(loaded, false);
		}

		Changed = false;
	}

	public Restaurant(IEconomicZone zone, ICell originalShopFront, string name)
		: base(zone, originalShopFront, name, "Restaurant")
	{
		_automatedService = false;
		_simulateCrafting = false;
		_handlingTime = TimeSpan.FromSeconds(15);
		_maximumBatchWait = TimeSpan.FromSeconds(90);
		_tableCleanupInterval = TimeSpan.FromMinutes(2);
		_cells.Add(new RestaurantCellAssignment(originalShopFront, RestaurantCellRole.Service));

		using (new FMDB())
		{
			var config = new DbRestaurant
			{
				ShopId = Id,
				AutomatedService = false,
				SimulateCrafting = false,
				HandlingSeconds = 15,
				MaximumBatchWaitSeconds = 90,
				CleanupIntervalSeconds = 120,
				ChefStartEmote = RestaurantServiceEmotes.DefaultChefStart,
				ChefOpenEmote = RestaurantServiceEmotes.DefaultChefOpen,
				ChefPlateEmote = RestaurantServiceEmotes.DefaultChefPlate,
				ChefReadyEmote = RestaurantServiceEmotes.DefaultChefReady,
				ServerServeEmote = RestaurantServiceEmotes.DefaultServerServe,
				ServerClearEmote = RestaurantServiceEmotes.DefaultServerClear,
				ServerReturnEmote = RestaurantServiceEmotes.DefaultServerReturn
			};
			config.Cells.Add(new DbRestaurantCell
			{
				RestaurantShopId = Id,
				CellId = originalShopFront.Id,
				Role = (int)RestaurantCellRole.Service
			});
			FMDB.Context.Restaurants.Add(config);
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public override string FrameworkItemType => "Restaurant";
	public bool AutomatedService
	{
		get => _automatedService;
		set
		{
			_automatedService = value;
			Changed = true;
		}
	}

	public bool SimulateCrafting
	{
		get => _simulateCrafting;
		set
		{
			_simulateCrafting = value;
			Changed = true;
		}
	}

	public TimeSpan HandlingTime
	{
		get => _handlingTime;
		set
		{
			_handlingTime = value < TimeSpan.Zero ? TimeSpan.Zero : value;
			Changed = true;
		}
	}

	public TimeSpan MaximumBatchWait
	{
		get => _maximumBatchWait;
		set
		{
			_maximumBatchWait = value < TimeSpan.Zero ? TimeSpan.Zero : value;
			Changed = true;
		}
	}

	public TimeSpan TableCleanupInterval
	{
		get => _tableCleanupInterval;
		set
		{
			_tableCleanupInterval = value < TimeSpan.Zero ? TimeSpan.Zero : value;
			Changed = true;
		}
	}

	public string ChefStartEmote
	{
		get => _chefStartEmote;
		set => SetServiceEmote(RestaurantServiceEmoteType.ChefStart, value);
	}

	public string ChefOpenEmote
	{
		get => _chefOpenEmote;
		set => SetServiceEmote(RestaurantServiceEmoteType.ChefOpen, value);
	}

	public string ChefPlateEmote
	{
		get => _chefPlateEmote;
		set => SetServiceEmote(RestaurantServiceEmoteType.ChefPlate, value);
	}

	public string ChefReadyEmote
	{
		get => _chefReadyEmote;
		set => SetServiceEmote(RestaurantServiceEmoteType.ChefReady, value);
	}

	public string ServerServeEmote
	{
		get => _serverServeEmote;
		set => SetServiceEmote(RestaurantServiceEmoteType.ServerServe, value);
	}

	public string ServerClearEmote
	{
		get => _serverClearEmote;
		set => SetServiceEmote(RestaurantServiceEmoteType.ServerClear, value);
	}

	public string ServerReturnEmote
	{
		get => _serverReturnEmote;
		set => SetServiceEmote(RestaurantServiceEmoteType.ServerReturn, value);
	}

	public IGameItemProto? TakeawayBagPrototype
	{
		get => _takeawayBagPrototype ??= _takeawayBagPrototypeId.HasValue
			? Gameworld.ItemProtos.Get(_takeawayBagPrototypeId.Value, _takeawayBagPrototypeRevisionNumber ?? 0)
			: null;
		set
		{
			_takeawayBagPrototype = value;
			_takeawayBagPrototypeId = value?.Id;
			_takeawayBagPrototypeRevisionNumber = value?.RevisionNumber;
			Changed = true;
		}
	}

	public string GetServiceEmote(RestaurantServiceEmoteType type)
	{
		return type switch
		{
			RestaurantServiceEmoteType.ChefStart => ChefStartEmote,
			RestaurantServiceEmoteType.ChefOpen => ChefOpenEmote,
			RestaurantServiceEmoteType.ChefPlate => ChefPlateEmote,
			RestaurantServiceEmoteType.ChefReady => ChefReadyEmote,
			RestaurantServiceEmoteType.ServerServe => ServerServeEmote,
			RestaurantServiceEmoteType.ServerClear => ServerClearEmote,
			RestaurantServiceEmoteType.ServerReturn => ServerReturnEmote,
			_ => RestaurantServiceEmotes.DefaultFor(type)
		};
	}

	public void SetServiceEmote(RestaurantServiceEmoteType type, string? emote)
	{
		var normalized = RestaurantServiceEmotes.Normalize(type, emote);
		switch (type)
		{
			case RestaurantServiceEmoteType.ChefStart:
				_chefStartEmote = normalized;
				break;
			case RestaurantServiceEmoteType.ChefOpen:
				_chefOpenEmote = normalized;
				break;
			case RestaurantServiceEmoteType.ChefPlate:
				_chefPlateEmote = normalized;
				break;
			case RestaurantServiceEmoteType.ChefReady:
				_chefReadyEmote = normalized;
				break;
			case RestaurantServiceEmoteType.ServerServe:
				_serverServeEmote = normalized;
				break;
			case RestaurantServiceEmoteType.ServerClear:
				_serverClearEmote = normalized;
				break;
			case RestaurantServiceEmoteType.ServerReturn:
				_serverReturnEmote = normalized;
				break;
		}

		Changed = true;
	}

	public IEnumerable<ICell> ServiceCells => _cells
		.Where(x => x.Role == RestaurantCellRole.Service)
		.Select(x => x.Cell)
		.Distinct();
	public IEnumerable<ICell> InternalCells => _cells
		.Where(x => x.Role == RestaurantCellRole.Internal)
		.Select(x => x.Cell)
		.Distinct();
	public IEnumerable<ICell> KitchenCells => _cells
		.Where(x => x.Role == RestaurantCellRole.Kitchen)
		.Select(x => x.Cell)
		.Distinct();
	public IEnumerable<IGameItem> RestaurantTables => _tableIds
		.Select(FindRestaurantItem)
		.Where(x => x is not null)
		.Cast<IGameItem>();
	public IEnumerable<IRestaurantMenuItem> MenuItems => _menuItems;
	public IEnumerable<IRestaurantTableSession> TableSessions => _tableSessions;
	public IEnumerable<IRestaurantOrder> Orders => _orders;
	public IEnumerable<IRestaurantStorageContainer> StorageContainers => _storageContainers;

	public override void PostLoadInitialisation()
	{
		base.PostLoadInitialisation();
		foreach (var interruptedOrder in _orders.OfType<RestaurantOrder>()
		         .Where(x => x.Status == RestaurantOrderStatus.Preparing))
		{
			interruptedOrder.MarkFailed("Preparation was interrupted by server recovery; manager action or refund is required.");
		}

		foreach (var session in _tableSessions.OfType<RestaurantTableSession>()
		         .Where(x => x.Status is RestaurantTableSessionStatus.Active or RestaurantTableSessionStatus.AbandonmentPending or RestaurantTableSessionStatus.OrderingClosed))
		{
			foreach (var participant in session.Participants.Where(x => x.Accepted))
			{
				if (ActiveActor(participant.CharacterId) is { } character && IsWithinTableServiceBoundary(character.Location))
				{
					TrackParticipant(character, session);
				}
			}
		}

		if (_heartbeatAttached)
		{
			return;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += RestaurantHeartbeat;
		_heartbeatAttached = true;
	}

	internal void StopService()
	{
		if (!_heartbeatAttached)
		{
			return;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= RestaurantHeartbeat;
		_heartbeatAttached = false;
	}

	public bool IsWithinRestaurant(ICell? cell)
	{
		return cell is not null && _cells.Any(x => x.Cell.Id == cell.Id);
	}

	private bool IsWithinTableServiceBoundary(ICell? cell)
	{
		return IsWithinRestaurant(cell);
	}

	public IRestaurantTableSession? TableSessionFor(IGameItem table)
	{
		return _tableSessions.FirstOrDefault(x =>
			x.TableGameItemId == table.Id &&
			x.Status is RestaurantTableSessionStatus.Active or RestaurantTableSessionStatus.AbandonmentPending or RestaurantTableSessionStatus.OrderingClosed);
	}

	public IRestaurantTableSession? TableSessionFor(ICharacter character)
	{
		var identityId = CharacterInstanceIdentityComparer.IdentityId(character);
		return _tableSessions.OfType<RestaurantTableSession>().FirstOrDefault(x =>
			(x.Status is RestaurantTableSessionStatus.Active or RestaurantTableSessionStatus.AbandonmentPending or RestaurantTableSessionStatus.OrderingClosed) &&
			x.HasPresentAcceptedParticipant(identityId));
	}

	public TimeSpan EstimateWait(ICharacter customer, IRestaurantMenuItem menuItem, int quantity = 1)
	{
		var queuedAhead = _orders
			.OfType<RestaurantOrder>()
			.Count(x => x.Status is RestaurantOrderStatus.Queued or RestaurantOrderStatus.Preparing);
		var preparationTime = RestaurantServiceRules.PreparationTime(menuItem.PreparationTime, menuItem.Craft?.PhaseLengths);
		return RestaurantServiceRules.EstimateWait(preparationTime, HandlingTime, queuedAhead,
			MaximumBatchWait, quantity);
	}

	public string ShowMenu(ICharacter actor)
	{
		var activeItems = _menuItems
			.OfType<RestaurantMenuItem>()
			.Where(x => x.IsActive)
			.OrderBy(x => x.SortOrder)
			.ThenBy(x => x.Name)
			.ToList();
		if (!activeItems.Any())
		{
			return $"{Name.ColourName()} does not currently have a menu.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{Name.ColourName()} Menu".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		var indexedItems = activeItems.Select((item, index) => new { Item = item, Index = index + 1 });
		foreach (var group in indexedItems
		         .GroupBy(x => x.Item.Merchandise.MerchandiseType.DescribeEnum())
		         .OrderBy(x => x.Key))
		{
			sb.AppendLine(group.Key.ColourName());
			sb.AppendLine(StringUtilities.GetTextTable(
			group.Select(entry =>
			{
				var item = entry.Item;
				var isCraftedMenuItem = item.FulfilmentMode is RestaurantFulfilmentMode.CraftAndBring or RestaurantFulfilmentMode.CraftAndPlate;
				var available = !item.IsValid(out var invalidReason)
					? $"Unavailable: {invalidReason}"
					: !isCraftedMenuItem && !StockedItems(item.Merchandise).Any()
						? "Unavailable: out of stock"
						: $"{(item.DineInAvailable ? "Dine-in" : "")} {(item.TakeawayAvailable ? "Takeaway" : "")}".Trim();
				return new[]
				{
					entry.Index.ToString("N0", actor),
					item.Name.ColourName(),
					item.Description,
					Currency.Describe(GetPriceCalculation(actor, item.Merchandise, 1).TotalPrice, CurrencyDescriptionPatternType.Short).ColourValue(),
					available,
					EstimateWait(actor, item).Describe(actor).ColourValue()
				};
			}),
				["#", "Item", "Description", "Price", "Available", "Estimated Wait"],
				actor.LineFormatLength,
				truncatableColumnIndex: 2,
				colour: Telnet.Yellow,
				unicodeTable: actor.Account.UseUnicode));
			sb.AppendLine();
		}
		sb.AppendLine();
		sb.AppendLine(@"Use #3order table <table>#0 to start or join table service
Use #3order <item> [quantity] [for <participant>]#0 to order at your table
Use #3order takeaway <item> [quantity]#0 to pay in advance and order for takeaway.".SubstituteANSIColour());
		return sb.ToString();
	}

	public RestaurantOperationResult TryJoinTable(ICharacter requester, IGameItem table)
	{
		if (!IsWithinRestaurant(requester.Location) || !ServiceCells.Any(x => x.Id == requester.Location?.Id))
		{
			return RestaurantOperationResult.Fail("You must be in one of this restaurant's table-service areas to use a table.");
		}

		if (!IsRegisteredTable(table, out var reason))
		{
			return RestaurantOperationResult.Fail(reason);
		}

		var priorSession = TableSessionFor(requester);
		if (priorSession is not null && priorSession.TableGameItemId != table.Id)
		{
			return RestaurantOperationResult.Fail("You are already an accepted participant at another active restaurant table.");
		}

		var session = TableSessionFor(table) as RestaurantTableSession;
		if (session is null)
		{
			session = new RestaurantTableSession(this, table, requester);
			_tableSessions.Add(session);
			TrackParticipant(requester, session);
			return RestaurantOperationResult.Succeed($"You begin table service at {table.HowSeen(requester)}. You can now place dine-in orders.");
		}

		var requesterId = CharacterInstanceIdentityComparer.IdentityId(requester);
		if (session.HasAcceptedParticipant(requesterId))
		{
			TrackParticipant(requester, session);
			return RestaurantOperationResult.Succeed($"You are already a participant at {table.HowSeen(requester)}.");
		}

		if (session.Status != RestaurantTableSessionStatus.Active)
		{
			return RestaurantOperationResult.Fail("That table's service session is no longer accepting participants.");
		}

		var eligibleParticipants = session.Participants
			.Where(x => x.Accepted)
			.Select(x => ActiveActor(x.CharacterId))
			.Where(x => x is not null && IsWithinTableServiceBoundary(x.Location))
			.Cast<ICharacter>()
			.ToList();
		if (!eligibleParticipants.Any())
		{
			return RestaurantOperationResult.Fail("There is no current table participant available to approve a new guest.");
		}

		var automaticJoin = eligibleParticipants.Any(participant =>
			RestaurantServiceRules.CanAutomaticallyJoin(
				participant.Party is not null && participant.Party == requester.Party,
				participant.IsAlly(requester)));
		if (automaticJoin)
		{
			session.AddParticipant(requester);
			TrackParticipant(requester, session);
			return RestaurantOperationResult.Succeed($"You join the table service at {table.HowSeen(requester)}.");
		}

		if (_pendingJoinRequests.Values.Any(x => !x.Resolved && x.SessionId == session.Id && x.RequesterCharacterId == requesterId))
		{
			return RestaurantOperationResult.Fail("Your request to join this table is already awaiting a participant's answer.");
		}

		var request = new PendingJoinRequest
		{
			Id = Guid.NewGuid(),
			SessionId = session.Id,
			RequesterCharacterId = requesterId,
			RequesterName = requester.PersonalName.GetName(NameStyle.FullName),
			ApproverCharacterIds = eligibleParticipants
				.Select(CharacterInstanceIdentityComparer.IdentityId)
				.ToHashSet()
		};
		_pendingJoinRequests.Add(request.Id, request);
		requester.AddEffect(new RestaurantTableJoinRequesterEffect(requester, this, request.Id));
		foreach (var participant in eligibleParticipants)
		{
			participant.AddEffect(new Accept(participant,
				new RestaurantTableJoinProposal(this, request.Id, CharacterInstanceIdentityComparer.IdentityId(participant),
					request.RequesterName, table.HowSeen(participant))), TimeSpan.FromMinutes(2));
		}

		return RestaurantOperationResult.Succeed("The current table participants have been asked to accept or decline your request to join. You cannot order or view the bill until one accepts.");
	}

	internal void ResolveTableJoinProposal(Guid requestId, long approverCharacterId, bool accepted)
	{
		if (!_pendingJoinRequests.TryGetValue(requestId, out var request) || request.Resolved)
		{
			return;
		}

		var requester = ActiveActor(request.RequesterCharacterId);
		var session = _tableSessions.OfType<RestaurantTableSession>().FirstOrDefault(x => x.Id == request.SessionId);
		var approver = ActiveActor(approverCharacterId);
		if (!accepted)
		{
			request.ApproverCharacterIds.Remove(approverCharacterId);
			if (request.ApproverCharacterIds.Any(x => ActiveActor(x) is { } actor && IsWithinTableServiceBoundary(actor.Location)))
			{
				return;
			}

			CompleteJoinRequest(request, false, "No current participant accepted your request to join the table.");
			return;
		}

		if (requester is null || !IsWithinTableServiceBoundary(requester.Location) || session is null ||
			session.Status != RestaurantTableSessionStatus.Active || approver is null ||
			!IsWithinTableServiceBoundary(approver.Location) || !session.HasAcceptedParticipant(approverCharacterId))
		{
			CompleteJoinRequest(request, false, "The table-join request was no longer valid when it was answered.");
			return;
		}

		if (TableSessionFor(requester) is { } otherSession && otherSession.Id != session.Id)
		{
			CompleteJoinRequest(request, false, "The requester is already participating at another restaurant table.");
			return;
		}

		session.AddParticipant(requester);
		TrackParticipant(requester, session);
		CompleteJoinRequest(request, true, $"{approver.PersonalName.GetName(NameStyle.FullName)} accepts you at the table. You can now place dine-in orders.");
	}

	public RestaurantOperationResult TryOrderDineIn(ICharacter orderer, RestaurantMenuItem menuItem, int quantity,
		ICharacter? recipient = null)
	{
		var session = TableSessionFor(orderer) as RestaurantTableSession;
		if (session is null || !session.HasAcceptedParticipant(CharacterInstanceIdentityComparer.IdentityId(orderer)))
		{
			return RestaurantOperationResult.Fail("You must be an accepted participant at a restaurant table before placing a dine-in order.");
		}

		if (session.Status == RestaurantTableSessionStatus.OrderingClosed)
		{
			return RestaurantOperationResult.Fail("That table has been closed to further orders because its bill is fully paid.");
		}

		if (session.Status != RestaurantTableSessionStatus.Active)
		{
			return RestaurantOperationResult.Fail("You must be an accepted participant at an active restaurant table before placing a dine-in order.");
		}

		if (!IsWithinTableServiceBoundary(orderer.Location))
		{
			return RestaurantOperationResult.Fail("You must remain within this restaurant to order at this table.");
		}

		var table = FindRestaurantItem(session.TableGameItemId);
		if (table is null)
		{
			return RestaurantOperationResult.Fail("Your designated restaurant table no longer exists, so it cannot accept further orders.");
		}

		if (!IsRegisteredTable(table, out var tableReason))
		{
			return RestaurantOperationResult.Fail($"Your table is no longer available for orders: {tableReason}");
		}

		recipient ??= orderer;
		if (!session.HasAcceptedParticipant(CharacterInstanceIdentityComparer.IdentityId(recipient)))
		{
			return RestaurantOperationResult.Fail("You can only order for an accepted participant at your table.");
		}

		return CreateOrder(orderer, recipient, session, menuItem, quantity, RestaurantOrderType.DineIn, null);
	}

	public RestaurantOperationResult TryOrderTakeaway(ICharacter orderer, RestaurantMenuItem menuItem, int quantity,
		IPaymentMethod paymentMethod)
	{
		if (!IsWithinTableServiceBoundary(orderer.Location))
		{
			return RestaurantOperationResult.Fail("You must be in the restaurant to place a takeaway order.");
		}

		return CreateOrder(orderer, orderer, null, menuItem, quantity, RestaurantOrderType.Takeaway, paymentMethod);
	}

	public RestaurantOperationResult TryPayBill(ICharacter payer, RestaurantTableSession session,
		IEnumerable<RestaurantOrder> orders, decimal amount, IPaymentMethod paymentMethod, string paymentDescription)
	{
		if (!session.HasAcceptedParticipant(CharacterInstanceIdentityComparer.IdentityId(payer)))
		{
			return RestaurantOperationResult.Fail("You are not an accepted participant at that table and cannot access its bill.");
		}

		var unpaid = orders
			.Where(x => x.TableSession?.Id == session.Id)
			.Where(x => x.OutstandingBalance > 0.0M)
			.OrderBy(x => x.CreatedAtUtc)
			.ToList();
		var outstanding = unpaid.Sum(x => x.OutstandingBalance);
		if (outstanding <= 0.0M)
		{
			return RestaurantOperationResult.Fail("There is no unpaid balance matching that payment.");
		}

		if (amount <= 0.0M)
		{
			return RestaurantOperationResult.Fail("You must pay a positive amount.");
		}

		amount = RestaurantServiceRules.PaymentToApply(amount, outstanding);
		if (!CanTakePayment(paymentMethod, amount, out var reason))
		{
			return RestaurantOperationResult.Fail(reason);
		}

		TakePayment(paymentMethod, amount);
		var remaining = amount;
		foreach (var order in unpaid)
		{
			var applied = Math.Min(order.OutstandingBalance, remaining);
			order.AddPayment(payer, applied, paymentDescription, $"Table session #{session.Id:N0}");
			remaining -= applied;
			if (remaining <= 0.0M)
			{
				break;
			}
		}

		return RestaurantOperationResult.Succeed($"You pay {Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()} toward the table bill.");
	}

	public RestaurantOperationResult TryCloseTableOrdering(ICharacter closer, RestaurantTableSession session)
	{
		if (!session.HasPresentAcceptedParticipant(CharacterInstanceIdentityComparer.IdentityId(closer)))
		{
			return RestaurantOperationResult.Fail("You are not currently participating at that restaurant table.");
		}

		if (session.Status == RestaurantTableSessionStatus.OrderingClosed)
		{
			return RestaurantOperationResult.Fail("That table is already closed to further orders.");
		}

		if (session.Status != RestaurantTableSessionStatus.Active)
		{
			return RestaurantOperationResult.Fail("That table cannot be closed to further orders in its current state.");
		}

		if (session.Orders.OfType<RestaurantOrder>().Any(x => x.OutstandingBalance > 0.0M))
		{
			return RestaurantOperationResult.Fail("The table bill must be paid in full before you can close it to further orders.");
		}

		return session.CloseOrdering()
			? RestaurantOperationResult.Succeed("The table is now closed to further orders. Existing orders will still be served.")
			: RestaurantOperationResult.Fail("The table could not be closed to further orders.");
	}

	public string ShowBill(ICharacter viewer, RestaurantTableSession session)
	{
		if (!session.HasAcceptedParticipant(CharacterInstanceIdentityComparer.IdentityId(viewer)))
		{
			return "You are not an accepted participant at that table and cannot view its bill.";
		}

		var orders = session.Orders.OfType<RestaurantOrder>().OrderBy(x => x.CreatedAtUtc).ToList();
		if (!orders.Any())
		{
			return "No orders have been placed at this table yet.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Table Bill #{session.Id.ToString("N0", viewer)}".GetLineWithTitle(viewer, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine(StringUtilities.GetTextTable(
			orders.Select(order => new[]
			{
				order.Id.ToString("N0", viewer),
				order.MenuItem.Name,
				order.OrdererCharacterName.ColourName(),
				order.RecipientCharacterName.ColourName(),
				order.Status.DescribeEnum().ColourCommand(),
				Currency.Describe(order.Price, CurrencyDescriptionPatternType.Short).ColourValue(),
				Currency.Describe(order.AmountPaid, CurrencyDescriptionPatternType.Short).ColourValue(),
				Currency.Describe(order.OutstandingBalance, CurrencyDescriptionPatternType.Short).ColourError()
			}),
			new[] { "#", "Item", "Ordered By", "For", "Status", "Price", "Paid", "Owing" },
			viewer.LineFormatLength,
			truncatableColumnIndex: 1,
			colour: Telnet.Yellow,
			unicodeTable: viewer.Account.UseUnicode));
		var outstanding = orders.Sum(x => x.OutstandingBalance);
		sb.AppendLine($"Total outstanding: {Currency.Describe(outstanding, CurrencyDescriptionPatternType.Short).ColourError()}");
		return sb.ToString();
	}

	public string ShowServiceQueue(ICharacter viewer)
	{
		var orders = _orders
			.OfType<RestaurantOrder>()
			.Where(x => x.Status is RestaurantOrderStatus.Queued or RestaurantOrderStatus.Preparing or RestaurantOrderStatus.ReadyForService or RestaurantOrderStatus.Failed)
			.OrderBy(x => x.CreatedAtUtc)
			.ToList();
		if (!orders.Any())
		{
			return "There are no active or recovery-required restaurant orders.";
		}

		return StringUtilities.GetTextTable(
			orders.Select(x => new[]
			{
				x.Id.ToString("N0", viewer),
				x.MenuItem.Name,
				x.OrderType.DescribeEnum(),
				x.OrdererCharacterName,
				x.RecipientCharacterName,
				x.Status.DescribeEnum(),
				x.ExpectedReadyAtUtc?.ToString("g", viewer) ?? "Recovery required",
				x.OperationalNotes
			}),
			new[] { "#", "Item", "Type", "Orderer", "For", "Status", "Expected", "Notes" },
			viewer.LineFormatLength,
			truncatableColumnIndex: 7,
			colour: Telnet.Yellow,
			unicodeTable: viewer.Account.UseUnicode);
	}

	public IReadOnlyDictionary<long, decimal> EqualSplitSuggestion(RestaurantTableSession session)
	{
		return RestaurantServiceRules.SuggestEqualSplit(
			session.Participants.Where(x => x.Accepted).Select(x => x.CharacterId),
			session.Orders.OfType<RestaurantOrder>().Sum(x => x.OutstandingBalance));
	}

	public RestaurantOperationResult TryPrepareOrder(ICharacter chef, RestaurantOrder order, IGameItem? craftedOutput = null)
	{
		if (!CanPerformDuty(chef, EmploymentRole.Chef, EmploymentRole.Crafter))
		{
			return RestaurantOperationResult.Fail("You must be clocked in with chef duties at this restaurant to prepare an order.");
		}

		if (!IsWithinRestaurant(chef.Location))
		{
			return RestaurantOperationResult.Fail("You must be in the restaurant to prepare an order.");
		}

		var chefId = CharacterInstanceIdentityComparer.IdentityId(chef);
		var alreadyClaimedByThisChef = order.Status == RestaurantOrderStatus.Preparing &&
		                              order.PreparedByEmployeeId == chefId &&
		                              craftedOutput is not null;
		if (order.Restaurant != this || (order.Status != RestaurantOrderStatus.Queued && !alreadyClaimedByThisChef))
		{
			return RestaurantOperationResult.Fail("That order is not awaiting preparation at this restaurant.");
		}

		if (order.MenuItem.FulfilmentMode is RestaurantFulfilmentMode.CraftAndBring or RestaurantFulfilmentMode.CraftAndPlate)
		{
			if (craftedOutput is null)
			{
				return RestaurantOperationResult.Fail("This order requires a real craft output. Craft the configured item and then specify it when marking the order ready.");
			}

			if (craftedOutput.Prototype.Id != order.MenuItem.Merchandise.Item.Id)
			{
				return RestaurantOperationResult.Fail("That item is not the configured output for this menu item.");
			}

			if (order.Quantity > 1 && craftedOutput.Quantity < order.Quantity)
			{
				return RestaurantOperationResult.Fail("This order needs one crafted serving per quantity, or a matching stack of the configured output.");
			}

			if (!alreadyClaimedByThisChef)
			{
				order.MarkPreparing(chef, $"Prepared by {chef.PersonalName.GetName(NameStyle.FullName)}.");
			}
			MoveToKitchen(craftedOutput);
			return FinishPreparation(order, new[] { craftedOutput }, chef);
		}

		var products = TakeStockForOrder(order, chef).ToList();
		if (!products.Any())
		{
			return RestaurantOperationResult.Fail("The restaurant does not currently have the required stock for that order.");
		}

		if (!alreadyClaimedByThisChef)
		{
			order.MarkPreparing(chef, $"Prepared by {chef.PersonalName.GetName(NameStyle.FullName)}.");
		}
		return FinishPreparation(order, products, chef);
	}

	public RestaurantOperationResult TryServeOrder(ICharacter server, RestaurantOrder order)
	{
		if (!CanPerformDuty(server, EmploymentRole.Server, EmploymentRole.Courier))
		{
			return RestaurantOperationResult.Fail("You must be clocked in with server duties at this restaurant to serve an order.");
		}

		if (!IsWithinRestaurant(server.Location))
		{
			return RestaurantOperationResult.Fail("You must be in the restaurant to serve an order.");
		}

		if (order.Restaurant != this || order.Status != RestaurantOrderStatus.ReadyForService)
		{
			return RestaurantOperationResult.Fail("That order is not ready for service.");
		}

		if (!CanReleaseForBatch(order))
		{
			return RestaurantOperationResult.Fail("That order is being held briefly so related table items can be served together.");
		}

		if (!DeliverOrder(order, server))
		{
			return RestaurantOperationResult.Fail("The recipient, table or prepared item is not available for service, so the order remains ready.");
		}

		return order.Status == RestaurantOrderStatus.Served
			? RestaurantOperationResult.Succeed("You serve the order.")
			: RestaurantOperationResult.Succeed("You continue the visible service for that order.");
	}

	public RestaurantOperationResult TryClearTable(ICharacter server, IGameItem table)
	{
		if (!CanPerformDuty(server, EmploymentRole.Server, EmploymentRole.Courier))
		{
			return RestaurantOperationResult.Fail("You must be clocked in with server duties at this restaurant to clear tables.");
		}

		if (!IsRegisteredTable(table, out var tableReason))
		{
			return RestaurantOperationResult.Fail(tableReason);
		}

		var tableContainer = table.GetItemType<IContainer>();
		if (tableContainer is null)
		{
			return RestaurantOperationResult.Fail("That designated table no longer has a usable container component.");
		}

		var items = EligibleEmptyServiceware(tableContainer, server).ToList();
		if (!items.Any())
		{
			return RestaurantOperationResult.Fail("There are no eligible empty serving containers on that table to clear.");
		}

		if (!IsAtTable(server, table))
		{
			return TryMoveTowardsCell(server, table.TrueLocations.FirstOrDefault())
				? RestaurantOperationResult.Succeed("You make your way to the table to clear it.")
				: RestaurantOperationResult.Fail("You cannot reach that table to clear it.");
		}

		foreach (var item in items)
		{
			if (!server.Body.CanGet(item, table, 0))
			{
				return RestaurantOperationResult.Fail(server.Body.WhyCannotGet(item, table, 0));
			}
		}

		foreach (var item in items)
		{
			server.Body.Get(item, table, 0, null, true);
			if (!ServerHasItem(server, item))
			{
				return RestaurantOperationResult.Fail("You could not take the empty serving container from the table.");
			}
		}

		IPerceivable cleared = items.Count == 1 ? items[0] : new PerceivableGroup(items);
		if (items.Count > 1 && items.All(x => x.CanBeBundled) && PileGameItemComponentProto.ItemPrototype is not null)
		{
			var bundle = PileGameItemComponentProto.CreateNewBundle(items);
			Gameworld.Add(bundle);
			bundle.RoomLayer = server.RoomLayer;
			server.Location!.Insert(bundle, true);
			server.Body.Get(bundle, silent: true);
		}

		EmitServiceEmote(RestaurantServiceEmoteType.ServerClear, server, cleared, table);
		return RestaurantOperationResult.Succeed($"You clear {cleared.HowSeen(server)} from the table.");
	}

	/// <summary>
	/// Allows an employment-worker NPC to use the same chef and server transitions as a PC. Craft
	/// lines are deliberately started and resumed through the native craft APIs so every craft phase
	/// uses the normal engine effects, materials, outputs and echoes rather than a simulated result.
	/// </summary>
	public bool TryHandleNpcService(ICharacter employee, IReadOnlySet<EmploymentAICapability> capabilities)
	{
		if (!IsWithinRestaurant(employee.Location))
		{
			return false;
		}

		if (capabilities.Contains(EmploymentAICapability.CanDeliverItems))
		{
			foreach (var readyOrder in _orders
			         .OfType<RestaurantOrder>()
			         .Where(x => x.Status == RestaurantOrderStatus.ReadyForService)
			         .OrderBy(x => x.ReadyAtUtc)
			         .ToList())
			{
				if (TryServeOrder(employee, readyOrder).Success)
				{
					return true;
				}
			}
		}

		if (capabilities.Contains(EmploymentAICapability.CanCraft))
		{
			if (TryHandleNpcCraft(employee))
			{
				return true;
			}

			var nextPreparation = _orders
				.OfType<RestaurantOrder>()
				.Where(x => x.Status == RestaurantOrderStatus.Queued)
				.Where(x => x.MenuItem.FulfilmentMode is not (RestaurantFulfilmentMode.CraftAndBring or RestaurantFulfilmentMode.CraftAndPlate))
				.OrderBy(x => x.CreatedAtUtc)
				.FirstOrDefault();
			var kitchen = KitchenCells.FirstOrDefault() ?? StockroomCell;
			if (nextPreparation is not null && kitchen is not null && !IsAtCell(employee, kitchen))
			{
				return TryMoveTowardsCell(employee, kitchen);
			}

			if (nextPreparation is not null && TryPrepareOrder(employee, nextPreparation).Success)
			{
				return true;
			}
		}

		if (!capabilities.Contains(EmploymentAICapability.CanDeliverItems))
		{
			return false;
		}

		if (TryReturnDirtyServiceware(employee))
		{
			return true;
		}

		foreach (var readyOrder in _orders
		         .OfType<RestaurantOrder>()
		         .Where(x => x.Status == RestaurantOrderStatus.ReadyForService)
		         .OrderBy(x => x.ReadyAtUtc)
		         .ToList())
		{
			if (TryServeOrder(employee, readyOrder).Success)
			{
				return true;
			}
		}

		if (RestaurantServiceRules.IsTableCleanupDue(_lastTableCleanupSweepAtUtc, TableCleanupInterval, DateTime.UtcNow))
		{
			_lastTableCleanupSweepAtUtc = DateTime.UtcNow;
			foreach (var table in RestaurantTables)
			{
				if (TryClearTable(employee, table).Success)
				{
					return true;
				}
			}
		}

		return false;
	}

	public RestaurantOperationResult CancelOrder(ICharacter employee, RestaurantOrder order, string reason)
	{
		if (!IsManager(employee) && !IsProprietor(employee) && !employee.IsAdministrator())
		{
			return RestaurantOperationResult.Fail("Only a restaurant manager or proprietor can cancel an order.");
		}

		if (order.Restaurant != this)
		{
			return RestaurantOperationResult.Fail("That order does not belong to this restaurant.");
		}

		if (order.Status is RestaurantOrderStatus.Served or RestaurantOrderStatus.Refunded)
		{
			return RestaurantOperationResult.Fail("That order can no longer be cancelled. Use the refund workflow if appropriate.");
		}

		order.MarkCancelled($"Cancelled by {employee.PersonalName.GetName(NameStyle.FullName)}. {reason}".Trim());
		return order.AmountPaid > 0.0M
			? RestaurantOperationResult.Succeed("The order is cancelled. Its recorded prepayment remains available for a separately auditable refund.")
			: RestaurantOperationResult.Succeed("The order is cancelled and no longer creates a table liability.");
	}

	public RestaurantOperationResult RefundOrder(ICharacter employee, ICharacter recipient, RestaurantOrder order, decimal amount,
		IPaymentMethod refundMethod)
	{
		if (!IsManager(employee) && !IsProprietor(employee) && !employee.IsAdministrator())
		{
			return RestaurantOperationResult.Fail("Only a restaurant manager or proprietor can issue a refund.");
		}

		if (order.Restaurant != this)
		{
			return RestaurantOperationResult.Fail("That order does not belong to this restaurant.");
		}

		amount = Math.Min(amount, order.AmountPaid);
		if (amount <= 0.0M)
		{
			return RestaurantOperationResult.Fail("There is no paid amount available to refund.");
		}

		if (refundMethod.Currency != Currency || refundMethod.AccessibleMoneyForCredit() < amount)
		{
			return RestaurantOperationResult.Fail("That refund method cannot currently provide the required amount.");
		}

		refundMethod.GivePayment(amount);
		order.AddPayment(recipient, amount, "Refund", $"Restaurant order #{order.Id:N0}", true);
		var refundedPretax = order.Price <= 0.0M ? amount : order.PretaxPrice * amount / order.Price;
		var refundedTax = order.Price <= 0.0M ? 0.0M : order.Tax * amount / order.Price;
		if (order.Status is not (RestaurantOrderStatus.Cancelled or RestaurantOrderStatus.Failed) &&
			order.GrossAmountPaid >= order.Price && order.AmountPaid <= 0.0M)
		{
			order.MarkRefunded($"Refunded {Currency.Describe(amount, CurrencyDescriptionPatternType.Short)}.");
		}
		else
		{
			order.AddOperationalNote($"Partially refunded {Currency.Describe(amount, CurrencyDescriptionPatternType.Short)}.");
		}

		if (order.RevenueRecognised)
		{
			AddTransaction(new TransactionRecord(ShopTransactionType.Refund, Currency, this,
				EconomicZone.ZoneForTimePurposes.DateTime(), employee, refundedPretax * -1.0M, refundedTax * -1.0M,
				order.MenuItem.Merchandise));
			EconomicZone.ReportSalesTaxCollected(this, refundedTax * -1.0M);
		}
		else
		{
			// The customer payment never became revenue, so retain the cash movement without reversing sales or tax.
			AddTransaction(new TransactionRecord(ShopTransactionType.Withdrawal, Currency, this,
				EconomicZone.ZoneForTimePurposes.DateTime(), employee, amount, 0.0M, order.MenuItem.Merchandise));
		}

		return RestaurantOperationResult.Succeed($"You refund {Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()}.");
	}

	public RestaurantOperationResult AddRestaurantCell(ICell cell, RestaurantCellRole role)
	{
		if (_cells.Any(x => x.Cell.Id == cell.Id && x.Role == role))
		{
			return RestaurantOperationResult.Fail("That cell already has that restaurant role.");
		}

		_cells.Add(new RestaurantCellAssignment(cell, role));
		AddShopfrontCell(cell);
		Changed = true;
		return RestaurantOperationResult.Succeed($"Cell #{cell.Id:N0} is now a {role.DescribeEnum()} cell for this restaurant.");
	}

	public RestaurantOperationResult SetStorageRole(IGameItem item, RestaurantStorageRole role, bool add)
	{
		if (role == RestaurantStorageRole.None)
		{
			return RestaurantOperationResult.Fail("You must specify a restaurant storage role.");
		}

		if (item.GetItemType<IContainer>() is null)
		{
			return RestaurantOperationResult.Fail("That item is not a container.");
		}

		if (!item.TrueLocations.Any(x => KitchenCells.Any(y => y.Id == x.Id)))
		{
			return RestaurantOperationResult.Fail("Restaurant storage containers must be physically located in a configured kitchen cell.");
		}

		var storage = _storageContainers.OfType<RestaurantStorageContainer>()
			.FirstOrDefault(x => x.GameItemId == item.Id);
		if (storage is null)
		{
			if (!add)
			{
				return RestaurantOperationResult.Fail("That item does not have that restaurant storage role.");
			}

			storage = new RestaurantStorageContainer(this, item, role);
			_storageContainers.Add(storage);
			Changed = true;
			return RestaurantOperationResult.Succeed($"{item.HowSeen(null)} is now configured for {role.DescribeEnum()} storage.");
		}

		var roles = add ? storage.Roles | role : storage.Roles & ~role;
		if (roles == storage.Roles)
		{
			return RestaurantOperationResult.Fail($"That item is {(add ? "already" : "not")} configured for {role.DescribeEnum()} storage.");
		}

		storage.SetRoles(roles);
		if (roles == RestaurantStorageRole.None)
		{
			_storageContainers.Remove(storage);
		}

		Changed = true;
		return RestaurantOperationResult.Succeed($"{item.HowSeen(null)} {(add ? "now has" : "no longer has")} the {role.DescribeEnum()} storage role.");
	}

	public RestaurantOperationResult RemoveRestaurantCell(ICell cell, RestaurantCellRole role)
	{
		var assignment = _cells.FirstOrDefault(x => x.Cell.Id == cell.Id && x.Role == role);
		if (assignment is null)
		{
			return RestaurantOperationResult.Fail("That cell does not have that restaurant role.");
		}

		if (role == RestaurantCellRole.Service && ServiceCells.Count() <= 1)
		{
			return RestaurantOperationResult.Fail("A restaurant must retain at least one table-service cell.");
		}

		if (role == RestaurantCellRole.Kitchen && _storageContainers
			.OfType<RestaurantStorageContainer>()
			.Any(x => Gameworld.TryGetItem(x.GameItemId, true)?.TrueLocations.Any(y => y.Id == cell.Id) == true))
		{
			return RestaurantOperationResult.Fail("Remove or relocate the restaurant storage containers in that kitchen cell before removing its kitchen role.");
		}

		_cells.Remove(assignment);
		if (!_cells.Any(x => x.Cell.Id == cell.Id))
		{
			RemoveShopfrontCell(cell);
		}

		Changed = true;
		return RestaurantOperationResult.Succeed("That restaurant cell role has been removed.");
	}

	public RestaurantOperationResult AddRestaurantTable(IGameItem table)
	{
		if (!IsRegisteredTableCandidate(table, out var reason))
		{
			return RestaurantOperationResult.Fail(reason);
		}

		if (!_tableIds.Add(table.Id))
		{
			return RestaurantOperationResult.Fail("That table is already registered with this restaurant.");
		}

		Changed = true;
		return RestaurantOperationResult.Succeed($"{table.HowSeen(null)} is now a designated restaurant table.");
	}

	public RestaurantOperationResult RemoveRestaurantTable(IGameItem table)
	{
		var session = TableSessionFor(table) as RestaurantTableSession;
		if (session is not null && session.Orders.OfType<RestaurantOrder>().Any(x =>
			(x.Status is RestaurantOrderStatus.Queued or RestaurantOrderStatus.Preparing or RestaurantOrderStatus.ReadyForService) ||
			x.OutstandingBalance > 0.0M))
		{
			return RestaurantOperationResult.Fail("That table has an active service session with unsettled or unserved orders. Finish or cancel those lines first.");
		}

		if (!_tableIds.Remove(table.Id))
		{
			return RestaurantOperationResult.Fail("That table is not registered with this restaurant.");
		}

		session?.Close();
		CancelJoinRequestsForSession(session?.Id);
		Changed = true;
		return RestaurantOperationResult.Succeed("That item is no longer a designated restaurant table.");
	}

	public RestaurantMenuItem AddMenuItem(IMerchandise merchandise)
	{
		var menu = new RestaurantMenuItem(this, merchandise);
		_menuItems.Add(menu);
		Changed = true;
		return menu;
	}

	public bool RemoveMenuItem(RestaurantMenuItem menu)
	{
		if (_orders.OfType<RestaurantOrder>().Any(x => x.MenuItem.Id == menu.Id))
		{
			return false;
		}

		if (!_menuItems.Remove(menu))
		{
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.RestaurantMenuItems.Find(menu.Id);
			if (dbitem is not null)
			{
				FMDB.Context.RestaurantMenuItems.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		Changed = true;
		return true;
	}

	protected override void Save(Models.Shop dbitem)
	{
		base.Save(dbitem);
		var context = FMDB.Context;
		var config = context.Restaurants.Find(Id);
		if (config is null)
		{
			return;
		}

		config.AutomatedService = AutomatedService;
		config.SimulateCrafting = SimulateCrafting;
		config.HandlingSeconds = (int)Math.Ceiling(HandlingTime.TotalSeconds);
		config.MaximumBatchWaitSeconds = (int)Math.Ceiling(MaximumBatchWait.TotalSeconds);
		config.CleanupIntervalSeconds = (int)Math.Ceiling(TableCleanupInterval.TotalSeconds);
		config.ChefStartEmote = ChefStartEmote;
		config.ChefOpenEmote = ChefOpenEmote;
		config.ChefPlateEmote = ChefPlateEmote;
		config.ChefReadyEmote = ChefReadyEmote;
		config.ServerServeEmote = ServerServeEmote;
		config.ServerClearEmote = ServerClearEmote;
		config.ServerReturnEmote = ServerReturnEmote;
		config.TakeawayBagPrototypeId = TakeawayBagPrototype?.Id ?? _takeawayBagPrototypeId;
		config.TakeawayBagPrototypeRevisionNumber = TakeawayBagPrototype?.RevisionNumber ?? _takeawayBagPrototypeRevisionNumber;

		var existingCells = context.RestaurantCells.Where(x => x.RestaurantShopId == Id).ToList();
		context.RestaurantCells.RemoveRange(existingCells);
		foreach (var assignment in _cells.DistinctBy(x => (x.Cell.Id, x.Role)))
		{
			context.RestaurantCells.Add(new DbRestaurantCell
			{
				RestaurantShopId = Id,
				CellId = assignment.Cell.Id,
				Role = (int)assignment.Role
			});
		}

		var existingTables = context.RestaurantTables.Where(x => x.RestaurantShopId == Id).ToList();
		context.RestaurantTables.RemoveRange(existingTables);
		foreach (var tableId in _tableIds)
		{
			context.RestaurantTables.Add(new DbRestaurantTable { RestaurantShopId = Id, GameItemId = tableId });
		}

		var existingStorage = context.RestaurantStorageContainers.Where(x => x.RestaurantShopId == Id).ToList();
		context.RestaurantStorageContainers.RemoveRange(existingStorage);
		foreach (var storage in _storageContainers.OfType<RestaurantStorageContainer>()
			         .Where(x => x.Roles != RestaurantStorageRole.None))
		{
			context.RestaurantStorageContainers.Add(new DbRestaurantStorageContainer
			{
				RestaurantShopId = Id,
				GameItemId = storage.GameItemId,
				Roles = (int)storage.Roles
			});
		}

	}

	private RestaurantOperationResult CreateOrder(ICharacter orderer, ICharacter recipient, RestaurantTableSession? session,
		RestaurantMenuItem menuItem, int quantity, RestaurantOrderType orderType, IPaymentMethod? paymentMethod)
	{
		if (!IsTrading && !orderer.IsAdministrator())
		{
			return RestaurantOperationResult.Fail("This restaurant is not currently trading.");
		}

		if (quantity < 1)
		{
			return RestaurantOperationResult.Fail("You must order a quantity of at least one.");
		}

		if (!menuItem.IsActive)
		{
			return RestaurantOperationResult.Fail("That menu item is not currently active.");
		}

		if (!menuItem.IsValid(out var validityReason))
		{
			return RestaurantOperationResult.Fail($"That menu item is not available because {validityReason}.");
		}

		if (orderType == RestaurantOrderType.DineIn && !menuItem.DineInAvailable)
		{
			return RestaurantOperationResult.Fail("That menu item is not available for dine-in service.");
		}

		if (orderType == RestaurantOrderType.Takeaway && !menuItem.TakeawayAvailable)
		{
			return RestaurantOperationResult.Fail("That menu item is not available as takeaway.");
		}

		var isCraftedMenuItem = menuItem.FulfilmentMode is RestaurantFulfilmentMode.CraftAndBring or RestaurantFulfilmentMode.CraftAndPlate;
		if (isCraftedMenuItem)
		{
			if (CanShopProg?.ExecuteBool(orderer, menuItem.Merchandise.Item.Id,
				menuItem.Merchandise.Item.Tags.Select(x => x.Name)) == false)
			{
				var why = WhyCannotShopProg?.Execute(orderer, menuItem.Merchandise.Item.Id,
					menuItem.Merchandise.Item.Tags.Select(x => x.Name))?.ToString();
				return RestaurantOperationResult.Fail($"That menu item cannot currently be ordered because {why ?? "of an unknown reason"}.");
			}
		}
		else
		{
			var stockCheck = CanBuy(orderer, menuItem.Merchandise, quantity,
				orderType == RestaurantOrderType.Takeaway ? paymentMethod : null);
			if (!stockCheck.Truth)
			{
				return RestaurantOperationResult.Fail($"That menu item cannot currently be ordered because {stockCheck.Reason}.");
			}
		}

		var pricingActor = orderType == RestaurantOrderType.Takeaway && paymentMethod is LineOfCreditPayment lineOfCredit &&
			!lineOfCredit.Account.IsAccountOwner(orderer)
			? Gameworld.TryGetCharacter(lineOfCredit.Account.AccountOwnerId, true) ?? orderer
			: orderer;
		var calculation = GetPriceCalculation(pricingActor, menuItem.Merchandise, quantity);
		if (orderType == RestaurantOrderType.Takeaway && !CanTakePayment(paymentMethod, calculation.TotalPrice, out var paymentReason))
		{
			return RestaurantOperationResult.Fail(paymentReason);
		}

		var expectedReady = DateTime.UtcNow + EstimateWait(orderer, menuItem, quantity);
		var order = new RestaurantOrder(this, session, menuItem, orderer, recipient, orderType, quantity, calculation,
			expectedReady);
		_orders.Add(order);
		session?.AddOrder(order);
		if (orderType == RestaurantOrderType.Takeaway)
		{
			try
			{
				TakePayment(paymentMethod!, calculation.TotalPrice);
				order.AddPayment(orderer, calculation.TotalPrice, DescribePayment(paymentMethod!), $"Takeaway order #{order.Id:N0}");
			}
			catch (Exception ex)
			{
				order.MarkFailed($"Prepayment could not be completed: {ex.Message}");
				return RestaurantOperationResult.Fail("The prepayment failed. The order has been recorded for manager recovery rather than entering preparation.");
			}
		}

		var price = Currency.Describe(calculation.TotalPrice, CurrencyDescriptionPatternType.Short);
		var eta = EstimateWait(orderer, menuItem, quantity).Describe(orderer);
		return RestaurantOperationResult.Succeed(orderType == RestaurantOrderType.Takeaway
			? $"You place and prepay your takeaway order for {price.ColourValue()}. Estimated wait: {eta.ColourValue()}."
			: $"You add {menuItem.Name.ColourName()} to the table bill for {price.ColourValue()}. Estimated wait: {eta.ColourValue()}. The liability remains yours even when it is served to someone else.");
	}

	private bool CanTakePayment(IPaymentMethod? paymentMethod, decimal amount, out string reason)
	{
		if (paymentMethod is null)
		{
			reason = "You must specify a payment method.";
			return false;
		}

		if (paymentMethod.Currency != Currency)
		{
			reason = "That payment method uses a different currency from this restaurant.";
			return false;
		}

		if (paymentMethod is ShopCashPayment && !TillItems.Any() && StockroomCell is null)
		{
			reason = "This restaurant is currently missing its till, and so cannot do cash transactions.";
			return false;
		}

		if (paymentMethod is BankPayment && BankAccount is null)
		{
			reason = "This restaurant has no bank account configured for bank-item payments.";
			return false;
		}

		if (paymentMethod.AccessibleMoneyForPayment() < amount)
		{
			reason = $"That payment method has only {Currency.Describe(paymentMethod.AccessibleMoneyForPayment(), CurrencyDescriptionPatternType.Short).ColourValue()} available.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private void TakePayment(IPaymentMethod paymentMethod, decimal amount)
	{
		if (paymentMethod is ShopCashPayment)
		{
			ExpectedCashBalance += amount;
		}

		paymentMethod.TakePayment(amount);
	}

	private static string DescribePayment(IPaymentMethod paymentMethod)
	{
		return paymentMethod switch
		{
			ShopCashPayment => "Cash",
			BankPayment => "Bank payment item",
			LineOfCreditPayment => "Line of credit",
			_ => paymentMethod.GetType().Name
		};
	}

	private IEnumerable<IGameItem> TakeStockForOrder(RestaurantOrder order, ICharacter? employee)
	{
		var stocked = StockedItems(order.MenuItem.Merchandise).ToList();
		if (stocked.Sum(x => x.Quantity) < order.Quantity)
		{
			return Enumerable.Empty<IGameItem>();
		}

		var products = new List<IGameItem>();
		for (var index = 0; index < order.Quantity; index++)
		{
			var stockItem = StockedItems(order.MenuItem.Merchandise).FirstOrDefault();
			if (stockItem is null)
			{
				return Enumerable.Empty<IGameItem>();
			}

			var product = stockItem.Quantity > 1 ? stockItem.Get(null!, 1) : stockItem;
			DisposeFromStock(employee, product);
			MoveToKitchen(product);
			products.Add(product);
		}

		return products;
	}

	private RestaurantOperationResult FinishPreparation(RestaurantOrder order, IEnumerable<IGameItem> products,
		ICharacter? employee)
	{
		IGameItem? finalDeliveryItem = null;
		foreach (var product in products)
		{
			try
			{
				if (order.MenuItem.FulfilmentMode == RestaurantFulfilmentMode.OpenAndBring)
				{
					var openable = product.GetItemType<IOpenable>();
					if (openable is not null && !openable.IsOpen)
					{
						var employeeCanUseNativeOpen = employee is not null &&
							product.TrueLocations.Any(x => x.Id == employee.Location?.Id) &&
							employee.Body.CanOpen(openable);
						if (employeeCanUseNativeOpen)
						{
							employee!.Body.Open(openable, null!, null!);
						}
						else
						{
							openable.Open();
						}

						if (employee is not null)
						{
							EmitServiceEmote(RestaurantServiceEmoteType.ChefOpen, employee, product);
						}
					}
				}

				order.AddProducedItem(product, RestaurantOrderItemRole.Product);
				IGameItem deliveryItem = product;
				if (order.MenuItem.FulfilmentMode == RestaurantFulfilmentMode.CraftAndPlate)
				{
					if (!TryPackageItem(order, deliveryItem, order.MenuItem.ServingContainerPrototype,
						RestaurantOrderItemRole.ServingContainer, employee, out deliveryItem, out var plateReason))
					{
						order.MarkFailed(plateReason);
						return RestaurantOperationResult.Fail(plateReason);
					}
				}

				if (order.OrderType == RestaurantOrderType.Takeaway && order.MenuItem.TakeawayContainerPrototype is not null)
				{
					if (!TryPackageItem(order, deliveryItem, order.MenuItem.TakeawayContainerPrototype,
						RestaurantOrderItemRole.TakeawayContainer, employee, out deliveryItem, out var packageReason))
					{
						order.MarkFailed(packageReason);
						return RestaurantOperationResult.Fail(packageReason);
					}
				}

				finalDeliveryItem = deliveryItem;
			}
			catch (Exception ex)
			{
				order.MarkFailed($"Preparation failed: {ex.Message}");
				return RestaurantOperationResult.Fail("The order could not be prepared and has been recorded as failed for recovery or refund.");
			}
		}

		order.MarkReady(employee);
		if (employee is not null)
		{
			EmitServiceEmote(RestaurantServiceEmoteType.ChefReady, employee, finalDeliveryItem,
				ActiveActor(order.RecipientCharacterId), TableForOrder(order));
		}
		return RestaurantOperationResult.Succeed("The order is ready for a server to deliver.");
	}

	private bool TryPackageItem(RestaurantOrder order, IGameItem item, IGameItemProto? prototype,
		RestaurantOrderItemRole role, ICharacter? employee, out IGameItem packaged, out string reason)
	{
		packaged = item;
		if (prototype is null)
		{
			reason = "The configured container is unavailable.";
			return false;
		}

		var storageRole = role == RestaurantOrderItemRole.ServingContainer
			? RestaurantStorageRole.Servingware
			: RestaurantStorageRole.TakeawayContainers;
		var containerAllocation = TakeStoredKitchenContainer(prototype, storageRole, employee);
		var containerItem = containerAllocation?.Item ?? prototype.CreateNew();
		containerAllocation ??= new KitchenContainerAllocation(containerItem, null);
		if (containerAllocation.SourceStorage is null)
		{
			Gameworld.Add(containerItem);
			containerItem.Login();
		}

		var container = containerItem.GetItemType<IContainer>();
		if (container is null || !container.CanPut(item))
		{
			ReturnOrDisposeKitchenContainer(containerAllocation);
			reason = "The configured serving or packaging container cannot hold the prepared item.";
			return false;
		}

		if (employee is { } serviceEmployee &&
			(ServerHasItem(serviceEmployee, item) || item.TrueLocations.Any(x => x.Id == serviceEmployee.Location?.Id)))
		{
			if (!ServerHasItem(serviceEmployee, item))
			{
				if (item.ContainedIn is not null)
				{
					serviceEmployee.Body.Get(item, item.ContainedIn, 0, null, false);
				}
				else
				{
					serviceEmployee.Body.Get(item, 0, null, false);
				}
			}

			if (!ServerHasItem(serviceEmployee, item))
			{
				ReturnOrDisposeKitchenContainer(containerAllocation);
				reason = "The employee could not retrieve the prepared item for plating or packaging.";
				return false;
			}

			if (!serviceEmployee.Body.HeldOrWieldedItems.Contains(item))
			{
				serviceEmployee.Body.Take(item);
			}

			if (!serviceEmployee.Body.CanPut(item, containerItem, null, 0, true))
			{
				ReturnOrDisposeKitchenContainer(containerAllocation);
				reason = "The employee could not put the prepared item into the configured serving or packaging container.";
				return false;
			}

			serviceEmployee.Body.Put(item, containerItem, null, 0, null, false);
		}
		else
		{
			item.InInventoryOf?.Take(item);
			item.ContainedIn?.Take(item);
			item.Location?.Extract(item);
			item.Drop(null);
			container.Put(null, item, allowMerge: false);
		}

		if (!container.Contents.Contains(item))
		{
			ReturnOrDisposeKitchenContainer(containerAllocation);
			reason = "The prepared item could not be placed into the configured serving or packaging container.";
			return false;
		}

		if (role == RestaurantOrderItemRole.ServingContainer && employee is not null)
		{
			EmitServiceEmote(RestaurantServiceEmoteType.ChefPlate, employee, item, containerItem);
		}
		MoveToKitchen(containerItem);
		order.AddProducedItem(containerItem, role);
		packaged = containerItem;
		reason = string.Empty;
		return true;
	}

	private void MoveToKitchen(IGameItem item)
	{
		item.InInventoryOf?.Take(item);
		item.ContainedIn?.Take(item);
		item.Location?.Extract(item);
		(KitchenCells.FirstOrDefault() ?? StockroomCell ?? ServiceCells.First()).Insert(item, newStack: true);
	}

	private bool TryHandleNpcCraft(ICharacter chef)
	{
		var chefId = CharacterInstanceIdentityComparer.IdentityId(chef);
		if (_npcCraftJobs.TryGetValue(chefId, out var job))
		{
			return AdvanceNpcCraft(chef, job);
		}

		if (chef.EffectsOfType<IActiveCraftEffect>().Any())
		{
			return true;
		}

		var order = _orders
			.OfType<RestaurantOrder>()
			.Where(x => x.Status == RestaurantOrderStatus.Queued)
			.Where(x => x.MenuItem.FulfilmentMode is RestaurantFulfilmentMode.CraftAndBring or RestaurantFulfilmentMode.CraftAndPlate)
			.OrderBy(x => x.CreatedAtUtc)
			.FirstOrDefault();
		if (order?.MenuItem.Craft is not { } craft)
		{
			return false;
		}

		var kitchen = KitchenCells.FirstOrDefault();
		if (kitchen is not null && !IsAtCell(chef, kitchen))
		{
			return TryMoveTowardsCell(chef, kitchen);
		}

		if (!craft.AppearInCraftsList(chef))
		{
			AddOperationalNoteOnce(order,
				$"Waiting for an employed chef who can perform {craft.Name}.");
			return false;
		}

		var canStart = craft.CanDoCraft(chef, null!, allowStartOnly: true, ignoreToolAndMaterialFailure: false);
		if (!canStart.Success)
		{
			AddOperationalNoteOnce(order, $"Waiting for real craft inputs: {canStart.Error}");
			return false;
		}

		var preExistingItems = chef.Location?
			.LayerGameItems(chef.RoomLayer)
			.Select(x => x.Id)
			.ToHashSet() ?? [];
		var preExistingOutputQuantities = chef.Location?
			.LayerGameItems(chef.RoomLayer)
			.Where(x => x.Prototype.Id == order.MenuItem.Merchandise.Item.Id)
			.ToDictionary(x => x.Id, x => x.Quantity) ?? [];
		order.MarkPreparing(chef, $"Claimed for real craft preparation by {chef.PersonalName.GetName(NameStyle.FullName)}.");
		EmitServiceEmote(RestaurantServiceEmoteType.ChefStart, chef,
			new DummyPerceivable(order.MenuItem.Name, location: chef.Location));
		try
		{
			craft.BeginCraft(chef);
			_npcCraftJobs[chefId] = new NpcCraftJob(order.Id, craft.Id, craft.RevisionNumber, preExistingItems,
				preExistingOutputQuantities);
			return true;
		}
		catch (Exception ex)
		{
			order.MarkFailed($"The employed chef could not start the real craft: {ex.Message}");
			return true;
		}
	}

	private bool AdvanceNpcCraft(ICharacter chef, NpcCraftJob job)
	{
		if (_orders.OfType<RestaurantOrder>().FirstOrDefault(x => x.Id == job.OrderId) is not { } order ||
			order.Status != RestaurantOrderStatus.Preparing || order.MenuItem.Craft is not { } craft ||
			craft.Id != job.CraftId || craft.RevisionNumber != job.CraftRevisionNumber)
		{
			_npcCraftJobs.Remove(CharacterInstanceIdentityComparer.IdentityId(chef));
			return false;
		}

		var activeEffect = chef.EffectsOfType<IActiveCraftEffect>()
			.FirstOrDefault(x => x.Component.Craft.Id == craft.Id && x.Component.Craft.RevisionNumber == craft.RevisionNumber);
		if (activeEffect is not null)
		{
			return true;
		}

		var interruptedCraft = chef.Location?
			.LayerGameItems(chef.RoomLayer)
			.SelectNotNull(x => x?.GetItemType<IActiveCraftGameItemComponent>())
			.FirstOrDefault(x => x.Craft is not null && x.Craft.Id == craft.Id &&
			                     x.Craft.RevisionNumber == craft.RevisionNumber && !x.HasFinished);
		if (interruptedCraft is not null)
		{
			var canResume = craft.CanResumeCraft(chef, interruptedCraft);
			if (canResume.Success)
			{
				craft.ResumeCraft(chef, interruptedCraft);
				return true;
			}

			order.MarkFailed($"The employed chef could not resume the real craft: {canResume.Error}");
			_npcCraftJobs.Remove(CharacterInstanceIdentityComparer.IdentityId(chef));
			return true;
		}

		var craftedOutputs = chef.Location?
			.LayerGameItems(chef.RoomLayer)
			.Where(x => x.Prototype.Id == order.MenuItem.Merchandise.Item.Id)
			.ToList() ?? [];
		var craftedOutput = craftedOutputs
			.Where(x => !job.PreExistingItemIds.Contains(x.Id))
			.OrderByDescending(x => x.Quantity)
			.FirstOrDefault();
		if (craftedOutput is not null)
		{
			craftedOutput = IsolateCraftedOutput(chef.Location!, craftedOutput, craftedOutput.Quantity, order.Quantity);
		}
		else
		{
			var mergedOutput = craftedOutputs
				.Select(x => new
				{
					Item = x,
					NewQuantity = RestaurantServiceRules.NewlyProducedQuantity(x.Quantity,
						job.PreExistingOutputQuantities.GetValueOrDefault(x.Id))
				})
				.Where(x => x.NewQuantity >= order.Quantity)
				.OrderByDescending(x => x.NewQuantity)
				.FirstOrDefault();
			if (mergedOutput is not null)
			{
				craftedOutput = IsolateCraftedOutput(chef.Location!, mergedOutput.Item, mergedOutput.NewQuantity,
					order.Quantity);
			}
		}
		_npcCraftJobs.Remove(CharacterInstanceIdentityComparer.IdentityId(chef));
		if (craftedOutput is null)
		{
			order.MarkFailed("The employed chef finished the craft without producing the configured menu output.");
			StowCraftTools(chef, craft);
			return true;
		}

		if (!TryPrepareOrder(chef, order, craftedOutput).Success)
		{
			order.MarkFailed("The real craft output could not be accepted by the restaurant service queue.");
		}

		StowCraftTools(chef, craft);

		return true;
	}

	/// <summary>
	/// Separates exactly the quantity produced for this order from a stack. Crafts release their
	/// products through normal cell insertion, which can merge a new stack into pre-existing stock;
	/// restaurant queue ownership must therefore keep the newly produced serving as a distinct item.
	/// </summary>
	private static IGameItem? IsolateCraftedOutput(ICell kitchen, IGameItem output, int newlyProducedQuantity,
		int orderQuantity)
	{
		if (newlyProducedQuantity < orderQuantity || output.Quantity < orderQuantity)
		{
			return null;
		}

		if (output.Quantity == orderQuantity)
		{
			return output;
		}

		if (output.GetItemType<IStackable>() is not { } stackable)
		{
			return null;
		}

		var isolatedOutput = stackable.Split(orderQuantity);
		isolatedOutput.RoomLayer = output.RoomLayer;
		kitchen.Insert(isolatedOutput, newStack: true);
		return isolatedOutput;
	}

	private static void AddOperationalNoteOnce(RestaurantOrder order, string note)
	{
		if (!order.OperationalNotes.Contains(note, StringComparison.Ordinal))
		{
			order.AddOperationalNote(note);
		}
	}

	private IEnumerable<IGameItem> EligibleEmptyServiceware(IContainer tableContainer, ICharacter server)
	{
		var servedContainerIds = _orders.OfType<RestaurantOrder>()
			.SelectMany(x => x.ProducedItems)
			.OfType<RestaurantOrderItem>()
			.Where(x => x.Delivered && x.Role == RestaurantOrderItemRole.ServingContainer && x.GameItemId.HasValue)
			.Select(x => x.GameItemId!.Value)
			.ToHashSet();
		return tableContainer.Contents
			.Where(x => servedContainerIds.Contains(x.Id))
			.Where(x => x.GetItemType<IContainer>()?.Contents.Any() != true)
			.Where(x => tableContainer.CanTake(server, x, 0));
	}

	private bool TryReturnDirtyServiceware(ICharacter server)
	{
		var dirtyServicewareIds = _orders.OfType<RestaurantOrder>()
			.SelectMany(x => x.ProducedItems)
			.OfType<RestaurantOrderItem>()
			.Where(x => x.Delivered && x.Role == RestaurantOrderItemRole.ServingContainer && x.GameItemId.HasValue)
			.Select(x => x.GameItemId!.Value)
			.ToHashSet();
		var dirtyItem = dirtyServicewareIds
			.Select(FindRestaurantItem)
			.FirstOrDefault(x => x is not null && ServerHasItem(server, x));
		var bundle = server.Body.ExternalItems
			.FirstOrDefault(x => x.GetItemType<PileGameItemComponent>()?.Contents
				.Any(y => dirtyServicewareIds.Contains(y.Id)) == true);
		var returnItem = bundle ?? dirtyItem;
		if (returnItem is null)
		{
			return false;
		}

		var kitchen = KitchenCells.FirstOrDefault() ?? StockroomCell;
		if (kitchen is null)
		{
			return false;
		}

		if (!IsAtCell(server, kitchen))
		{
			return TryMoveTowardsCell(server, kitchen);
		}

		if (!server.Body.HeldOrWieldedItems.Contains(returnItem))
		{
			server.Body.Take(returnItem);
		}

		server.Body.Drop(returnItem, silent: true);
		EmitServiceEmote(RestaurantServiceEmoteType.ServerReturn, server, returnItem);
		if (returnItem.GetItemType<PileGameItemComponent>() is { } pile)
		{
			var contents = pile.Contents.ToList();
			// Unbundle onto the kitchen floor first. Each item is then returned to the first store
			// that can actually accept it; anything that fits nowhere intentionally remains on the
			// floor rather than leaving the server holding a bundle.
			pile.Empty(null!, null!);
			foreach (var item in contents)
			{
				if (StorageContainersFor(RestaurantStorageRole.Servingware, server)
					    .FirstOrDefault(x => x.CanPut(item)) is not { } storage)
				{
					continue;
				}

				item.Location?.Extract(item);
				storage.Put(null, item, allowMerge: false);
			}
		}
		else if (StorageContainersFor(RestaurantStorageRole.Servingware, server)
				.FirstOrDefault(x => x.CanPut(returnItem)) is { } storage)
		{
			returnItem.Location?.Extract(returnItem);
			storage.Put(null, returnItem, allowMerge: false);
		}
		return true;
	}

	private IEnumerable<IContainer> StorageContainersFor(RestaurantStorageRole role, ICharacter employee)
	{
		return _storageContainers
			.OfType<RestaurantStorageContainer>()
			.Where(x => x.Roles.HasFlag(role))
			.OrderBy(x => x.GameItemId)
			.Select(x => Gameworld.TryGetItem(x.GameItemId, true))
			.Where(x => x is not null && x.TrueLocations.Any(y => y.Id == employee.Location?.Id))
			.Select(x => x!.GetItemType<IContainer>())
			.Where(x => x is not null)
			.Cast<IContainer>();
	}

	private KitchenContainerAllocation? TakeStoredKitchenContainer(IGameItemProto prototype,
		RestaurantStorageRole role, ICharacter? employee)
	{
		if (employee is null || !KitchenCells.Any(x => x.Id == employee.Location?.Id))
		{
			return null;
		}

		foreach (var storage in StorageContainersFor(role, employee))
		{
			var item = storage.Contents.FirstOrDefault(x =>
				x.Prototype.Id == prototype.Id &&
				x.Prototype.RevisionNumber == prototype.RevisionNumber &&
				x.GetItemType<IContainer>()?.Contents.Any() != true);
			if (item is null || !storage.CanTake(employee, item, 0))
			{
				continue;
			}

			storage.Take(employee, item, 0);
			return new KitchenContainerAllocation(item, storage);
		}

		return null;
	}

	private void ReturnOrDisposeKitchenContainer(KitchenContainerAllocation allocation)
	{
		if (allocation.SourceStorage is not null && allocation.SourceStorage.CanPut(allocation.Item))
		{
			allocation.SourceStorage.Put(null, allocation.Item, allowMerge: false);
			return;
		}

		if (allocation.SourceStorage is null)
		{
			allocation.Item.Delete();
			return;
		}

		MoveToKitchen(allocation.Item);
	}

	private void StowCraftTools(ICharacter employee, ICraft craft)
	{
		if (!KitchenCells.Any(x => x.Id == employee.Location?.Id))
		{
			return;
		}

		foreach (var tool in employee.Body.HeldOrWieldedItems
			.Where(item => craft.Tools.Any(x => x.IsTool(item)))
			.ToList())
		{
			var storage = StorageContainersFor(RestaurantStorageRole.Tools, employee)
				.FirstOrDefault(x => x.CanPut(tool));
			if (storage is null)
			{
				continue;
			}

			employee.Body.Drop(tool, silent: true);
			tool.Location?.Extract(tool);
			storage.Put(null, tool, allowMerge: false);
		}
	}

	private static bool ServerHasItem(ICharacter server, IGameItem item)
	{
		return item.IsInInventory(server.Body);
	}

	private bool TryRetrieveForService(ICharacter server, IGameItem item)
	{
		if (ServerHasItem(server, item))
		{
			return true;
		}

		if (item.InInventoryOf is not null)
		{
			return false;
		}

		var sourceCell = item.TrueLocations.FirstOrDefault();
		if (sourceCell is null)
		{
			return false;
		}

		if (!IsAtCell(server, sourceCell))
		{
			return TryMoveTowardsCell(server, sourceCell);
		}

		if (!server.Body.CanGet(item, 0))
		{
			return false;
		}

		server.Body.Get(item, 0, null, true);
		return ServerHasItem(server, item);
	}

	private static bool IsAtCell(ICharacter character, ICell? cell)
	{
		return cell is not null && character.Location?.Id == cell.Id;
	}

	private static bool IsAtTable(ICharacter character, IGameItem table)
	{
		return table.TrueLocations.Any(x => x.Id == character.Location?.Id);
	}

	private static bool TryMoveTowardsCell(ICharacter character, ICell? destination)
	{
		if (destination is null)
		{
			return false;
		}

		if (IsAtCell(character, destination))
		{
			return true;
		}

		var path = character.PathBetween(destination, 12, true).ToList();
		return path.Any() && character.Move(path[0]);
	}

	private IGameItem? TableForOrder(RestaurantOrder order)
	{
		return order.TableSession is RestaurantTableSession session
			? FindRestaurantItem(session.TableGameItemId)
			: null;
	}

	private void EmitServiceEmote(RestaurantServiceEmoteType type, ICharacter employee, IPerceivable? primary = null,
		IPerceivable? secondary = null, IPerceivable? tertiary = null)
	{
		var emote = new Emote(GetServiceEmote(type), employee,
			primary ?? new DummyPerceivable(location: employee.Location),
			secondary ?? new DummyPerceivable(location: employee.Location),
			tertiary ?? new DummyPerceivable(location: employee.Location));
		if (!emote.Valid)
		{
			var fallback = new Emote("@ attend|attends to the restaurant service.", employee);
			BroadcastServiceEmote(employee, fallback);
			return;
		}

		BroadcastServiceEmote(employee, emote);
	}

	/// <summary>
	/// NPCs normally use a non-player output handler. Dispatch restaurant emotes from the employee's
	/// actual cell rather than relying on that handler, so visible service remains visible to diners
	/// even while no player is possessing or monitoring the employee.
	/// </summary>
	private static void BroadcastServiceEmote(ICharacter employee, IEmote emote)
	{
		if (employee.Location is { } location)
		{
			location.HandleLocal(employee, employee.RoomLayer, new EmoteOutput(emote));
			return;
		}

		employee.OutputHandler.Handle(new EmoteOutput(emote));
	}

	private bool DeliverOrder(RestaurantOrder order, ICharacter? server)
	{
		if (order.OrderType == RestaurantOrderType.Takeaway)
		{
			return DeliverTakeawayCohort(order, server);
		}

		var recipient = ActiveActor(order.RecipientCharacterId);
		if (recipient is null)
		{
			return false;
		}

		IGameItem? table = null;
		IContainer? tableContainer = null;
		if (order.TableSession is RestaurantTableSession tableSession)
		{
			table = FindRestaurantItem(tableSession.TableGameItemId);
			tableContainer = table?.GetItemType<IContainer>();
			if (table is null || tableContainer is null || !table.TrueLocations.Any(x => x.Id == recipient.Location?.Id))
			{
				return false;
			}
		}
		else if (!IsWithinTableServiceBoundary(recipient.Location))
		{
			return false;
		}

		var roots = DeliveryRoots([order]).ToList();
		if (!roots.Any())
		{
			order.MarkFailed("No recoverable prepared items were available for service.");
			return false;
		}

		var root = roots[0];

		if (server is null)
		{
			if (recipient.Body.CanGet(root, 0))
			{
				recipient.Body.Get(root, silent: true);
			}
			else
			{
				root.InsertAtSource(recipient);
			}
		}
		else if (table is not null && tableContainer is not null)
		{
			if (!ServerHasItem(server, root))
			{
				if (!TryRetrieveForService(server, root))
				{
					return false;
				}

				return true;
			}

			if (!IsAtTable(server, table))
			{
				return TryMoveTowardsCell(server, table.TrueLocations.FirstOrDefault());
			}

			if (!server.Body.HeldOrWieldedItems.Contains(root))
			{
				server.Body.Take(root);
			}

			if (!server.Body.CanPut(root, table, null, 0, true))
			{
				return false;
			}

			server.Body.Put(root, table, null, 0, null, true);
			if (!tableContainer.Contents.Contains(root))
			{
				return false;
			}

			EmitServiceEmote(RestaurantServiceEmoteType.ServerServe, server, root, recipient, table);
		}
		else
		{
			if (!ServerHasItem(server, root))
			{
				if (!TryRetrieveForService(server, root))
				{
					return false;
				}

				return true;
			}

			if (!IsAtCell(server, recipient.Location))
			{
				return TryMoveTowardsCell(server, recipient.Location);
			}

			if (!server.Body.CanGive(root, recipient.Body, 0))
			{
				return false;
			}

			server.Body.Give(root, recipient.Body);
		}

		MarkDeliveredForRoot([order], root);
		if (order.ProducedItems.OfType<RestaurantOrderItem>().Any(x => !x.Delivered))
		{
			return true;
		}

		order.MarkServed(server);
		RecogniseSale(order, server);
		if (server is null)
		{
			recipient.OutputHandler.Send($"A server brings you {root.HowSeen(recipient)}.");
		}
		return true;
	}

	private bool DeliverTakeawayCohort(RestaurantOrder order, ICharacter? server)
	{
		var recipient = ActiveActor(order.RecipientCharacterId);
		if (recipient is null || !IsWithinRestaurant(recipient.Location))
		{
			return false;
		}

		var cohort = _orders
			.OfType<RestaurantOrder>()
			.Where(x => x.OrderType == RestaurantOrderType.Takeaway)
			.Where(x => x.RecipientCharacterId == order.RecipientCharacterId)
			.Where(x => x.Status == RestaurantOrderStatus.ReadyForService)
			.Where(CanReleaseForBatch)
			.OrderBy(x => x.ReadyAtUtc)
			.ToList();
		if (!cohort.Any())
		{
			return false;
		}

		var roots = DeliveryRoots(cohort).ToList();
		if (!roots.Any())
		{
			foreach (var failedOrder in cohort)
			{
				failedOrder.MarkFailed("No recoverable prepared items were available for takeaway collection.");
			}

			return false;
		}

		var existingBagRoots = cohort
			.SelectMany(x => x.ProducedItems)
			.OfType<RestaurantOrderItem>()
			.Where(x => x.Role == RestaurantOrderItemRole.TakeawayBag && x.GameItemId.HasValue)
			.Select(x => x.GameItemId!.Value)
			.ToHashSet();
		if (TakeawayBagPrototype is not null && !roots.All(x => existingBagRoots.Contains(x.Id)))
		{
			var kitchen = KitchenCells.FirstOrDefault() ?? StockroomCell;
			if (server is not null && kitchen is not null && !IsAtCell(server, kitchen))
			{
				return TryMoveTowardsCell(server, kitchen);
			}

			if (!TryPackageTakeawayRoots(cohort, roots, server, out var bagReason))
			{
				foreach (var cohortOrder in cohort)
				{
					AddOperationalNoteOnce(cohortOrder, bagReason);
				}

				return false;
			}

			roots = DeliveryRoots(cohort).ToList();
		}

		foreach (var root in roots)
		{
			if (server is null)
			{
				if (recipient.Body.CanGet(root, 0))
				{
					recipient.Body.Get(root, silent: true);
				}
				else
				{
					root.InsertAtSource(recipient);
				}
			}
			else
			{
				if (!ServerHasItem(server, root))
				{
					return TryRetrieveForService(server, root);
				}

				if (!IsAtCell(server, recipient.Location))
				{
					return TryMoveTowardsCell(server, recipient.Location);
				}

				if (!server.Body.CanGive(root, recipient.Body, 0))
				{
					return false;
				}

				server.Body.Give(root, recipient.Body);
			}

			MarkDeliveredForRoot(cohort, root);
		}

		foreach (var cohortOrder in cohort)
		{
			if (cohortOrder.ProducedItems.OfType<RestaurantOrderItem>().All(x => x.Delivered))
			{
				cohortOrder.MarkServed(server);
				RecogniseSale(cohortOrder, server);
			}
		}

		if (server is null)
		{
			recipient.OutputHandler.Send($"A server brings you {roots.Select(x => x.HowSeen(recipient)).ListToString()}.");
		}

		return true;
	}

	private IEnumerable<IGameItem> DeliveryRoots(IEnumerable<RestaurantOrder> orders)
	{
		var items = orders
			.SelectMany(x => x.ProducedItems)
			.OfType<RestaurantOrderItem>()
			.Where(x => !x.Delivered && x.GameItemId.HasValue)
			.Select(x => FindRestaurantItem(x.GameItemId!.Value))
			.Where(x => x is not null)
			.Cast<IGameItem>()
			.DistinctBy(x => x.Id)
			.ToList();
		return items.Where(item => !items.Any(other => other.Id != item.Id && IsContainedBy(item, other)));
	}

	private static bool IsContainedBy(IGameItem item, IGameItem possibleContainer)
	{
		var current = item.ContainedIn;
		while (current is not null)
		{
			if (current.Id == possibleContainer.Id)
			{
				return true;
			}

			current = current.ContainedIn;
		}

		return false;
	}

	private void MarkDeliveredForRoot(IEnumerable<RestaurantOrder> orders, IGameItem root)
	{
		foreach (var record in orders.SelectMany(x => x.ProducedItems).OfType<RestaurantOrderItem>()
			         .Where(x => !x.Delivered && x.GameItemId.HasValue))
		{
			var item = FindRestaurantItem(record.GameItemId!.Value);
			if (item is not null && (item.Id == root.Id || IsContainedBy(item, root)))
			{
				record.MarkDelivered();
			}
		}
	}

	private bool TryPackageTakeawayRoots(IReadOnlyCollection<RestaurantOrder> cohort,
		IReadOnlyCollection<IGameItem> roots, ICharacter? employee, out string reason)
	{
		if (!RestaurantTakeawayBagPacking.TryPlan(TakeawayBagPrototype!, roots, out var packingPlan, out reason))
		{
			return false;
		}

		var bagAllocations = new List<KitchenContainerAllocation>();
		foreach (var bagContents in packingPlan)
		{
			var bagAllocation = TakeStoredKitchenContainer(TakeawayBagPrototype!, RestaurantStorageRole.TakeawayBags,
				employee);
			var bag = bagAllocation?.Item ?? TakeawayBagPrototype!.CreateNew();
			bagAllocation ??= new KitchenContainerAllocation(bag, null);
			if (bagAllocation.SourceStorage is null)
			{
				Gameworld.Add(bag);
				bag.Login();
			}

			var container = bag.GetItemType<IContainer>();
			if (container is null)
			{
				ReturnOrDisposeKitchenContainer(bagAllocation);
				RollbackTakeawayBags(bagAllocations);
				reason = "The configured takeaway bag is no longer a usable container.";
				return false;
			}

			bagAllocations.Add(bagAllocation);
			foreach (var root in bagContents)
			{
				if (!container.CanPut(root))
				{
					RollbackTakeawayBags(bagAllocations);
					reason = $"The configured takeaway bag cannot hold {root.HowSeen(null)}.";
					return false;
				}

				root.InInventoryOf?.Take(root);
				root.ContainedIn?.Take(root);
				root.Location?.Extract(root);
				container.Put(null, root, allowMerge: false);
			}
		}

		foreach (var bagAllocation in bagAllocations)
		{
			MoveToKitchen(bagAllocation.Item);
			cohort.First().AddProducedItem(bagAllocation.Item, RestaurantOrderItemRole.TakeawayBag);
		}

		reason = string.Empty;
		return true;
	}

	private void RollbackTakeawayBags(IEnumerable<KitchenContainerAllocation> bags)
	{
		foreach (var allocation in bags)
		{
			var bag = allocation.Item;
			foreach (var item in bag.GetItemType<IContainer>()?.Contents.ToList() ?? [])
			{
				bag.GetItemType<IContainer>()!.Take(null!, item, 0);
				MoveToKitchen(item);
			}

			ReturnOrDisposeKitchenContainer(allocation);
		}
	}

	private void RecogniseSale(RestaurantOrder order, ICharacter? employee)
	{
		if (order.RevenueRecognised)
		{
			return;
		}

		AddTransaction(new TransactionRecord(ShopTransactionType.Sale, Currency, this,
			EconomicZone.ZoneForTimePurposes.DateTime(), employee, order.PretaxPrice, order.Tax,
			order.MenuItem.Merchandise));
		EconomicZone.ReportSalesTaxCollected(this, order.Tax);
		order.RecogniseRevenue();
	}

	private bool CanReleaseForBatch(RestaurantOrder order)
	{
		if (order.OrderType == RestaurantOrderType.Takeaway)
		{
			var relatedTakeaway = _orders
				.OfType<RestaurantOrder>()
				.Where(x => x.OrderType == RestaurantOrderType.Takeaway)
				.Where(x => x.RecipientCharacterId == order.RecipientCharacterId)
				.Where(x => x.Status is RestaurantOrderStatus.Queued or RestaurantOrderStatus.Preparing or RestaurantOrderStatus.ReadyForService)
				.ToList();
			return RestaurantServiceRules.IsBatchReady(relatedTakeaway.All(x => x.Status == RestaurantOrderStatus.ReadyForService),
				order.ReadyAtUtc, MaximumBatchWait, DateTime.UtcNow);
		}

		if (order.TableSession is not RestaurantTableSession session)
		{
			return true;
		}

		var related = session.Orders
			.OfType<RestaurantOrder>()
			.Where(x => x.Status is RestaurantOrderStatus.Queued or RestaurantOrderStatus.Preparing or RestaurantOrderStatus.ReadyForService)
			.ToList();
		if (related.All(x => x.Status == RestaurantOrderStatus.ReadyForService))
		{
			return true;
		}

		return RestaurantServiceRules.IsBatchReady(related.All(x => x.Status == RestaurantOrderStatus.ReadyForService),
			order.ReadyAtUtc, MaximumBatchWait, DateTime.UtcNow);
	}

	private void RestaurantHeartbeat()
	{
		CleanInvalidJoinRequests();
		ProcessAbandonments();
		if (AutomatedService)
		{
			foreach (var order in _orders.OfType<RestaurantOrder>().Where(x => x.Status == RestaurantOrderStatus.Queued).ToList())
			{
				if ((order.MenuItem.FulfilmentMode is RestaurantFulfilmentMode.CraftAndBring or RestaurantFulfilmentMode.CraftAndPlate) && !SimulateCrafting)
				{
					continue;
				}

				if (order.ExpectedReadyAtUtc > DateTime.UtcNow)
				{
					continue;
				}

				if (order.MenuItem.FulfilmentMode is RestaurantFulfilmentMode.CraftAndBring or RestaurantFulfilmentMode.CraftAndPlate)
				{
					var items = Enumerable.Range(0, order.Quantity)
						.Select(_ => order.MenuItem.Merchandise.Item.CreateNew())
						.ToList();
					foreach (var item in items)
					{
						Gameworld.Add(item);
						item.Login();
						MoveToKitchen(item);
					}

					order.MarkPreparing(null, "Prepared by configured simulated crafting fallback.");
					FinishPreparation(order, items, null);
					continue;
				}

				var itemsFromStock = TakeStockForOrder(order, null).ToList();
				if (!itemsFromStock.Any())
				{
					order.MarkFailed("Required stock was unavailable for automated service.");
					continue;
				}

				order.MarkPreparing(null, "Prepared by configured automated service.");
				FinishPreparation(order, itemsFromStock, null);
			}

			foreach (var order in _orders.OfType<RestaurantOrder>().Where(x => x.Status == RestaurantOrderStatus.ReadyForService).ToList())
			{
				if (CanReleaseForBatch(order))
				{
					DeliverOrder(order, null);
				}
			}
		}
	}

	private void ProcessAbandonments()
	{
		foreach (var session in _tableSessions.OfType<RestaurantTableSession>().Where(x => x.Status is RestaurantTableSessionStatus.Active or RestaurantTableSessionStatus.AbandonmentPending or RestaurantTableSessionStatus.OrderingClosed).ToList())
		{
			var presentParticipant = session.Participants
				.Where(x => x.Accepted)
				.Select(x => ActiveActor(x.CharacterId))
				.FirstOrDefault(x => x is not null && IsWithinTableServiceBoundary(x.Location));
			if (presentParticipant is not null)
			{
				if (session.Status == RestaurantTableSessionStatus.AbandonmentPending)
				{
					session.MarkParticipantPresence(presentParticipant, true);
				}
				continue;
			}

			var unpaid = session.Orders.OfType<RestaurantOrder>().Where(x => x.OutstandingBalance > 0.0M).ToList();
			if (!unpaid.Any())
			{
				if (session.Orders.All(x => x.Status is RestaurantOrderStatus.Served or RestaurantOrderStatus.Cancelled or RestaurantOrderStatus.Refunded or RestaurantOrderStatus.Failed))
				{
					session.Close();
				}
				continue;
			}

			if (session.Status == RestaurantTableSessionStatus.OrderingClosed)
			{
				continue;
			}

			session.BeginAbandonment();
			if (session.AbandonmentPendingAtUtc is null || DateTime.UtcNow - session.AbandonmentPendingAtUtc.Value < TimeSpan.FromSeconds(10))
			{
				continue;
			}

			var crimeLocation = FindRestaurantItem(session.TableGameItemId)?.TrueLocations.FirstOrDefault() ?? ServiceCells.FirstOrDefault();
			foreach (var debt in unpaid.GroupBy(x => x.OrdererCharacterId))
			{
				var debtor = Gameworld.TryGetCharacter(debt.Key, true);
				if (debtor is null)
				{
					continue;
				}

				var amount = debt.Sum(x => x.OutstandingBalance);
				AutomaticCrimeExtensions.CheckPossibleCrime(Gameworld, debtor, CrimeTypes.SkippingBill, null,
					FindRestaurantItem(session.TableGameItemId),
					$"automatic=restaurant-skip-bill;restaurant={Id};session={session.Id};table={session.TableGameItemId};amount={amount}",
					null, true, crimeLocation);
			}

			session.MarkAbandoned();
			CancelJoinRequestsForSession(session.Id);
		}
	}

	internal void NotifyParticipantLocationChanged(long sessionId, ICharacter character)
	{
		var session = _tableSessions.OfType<RestaurantTableSession>().FirstOrDefault(x => x.Id == sessionId);
		if (session is null || !session.HasAcceptedParticipant(CharacterInstanceIdentityComparer.IdentityId(character)))
		{
			return;
		}

		session.MarkParticipantPresence(character, IsWithinTableServiceBoundary(character.Location));
		if (!IsWithinTableServiceBoundary(character.Location))
		{
			foreach (var effect in character.EffectsOfType<RestaurantTableParticipantEffect>()
			             .Where(x => x.RestaurantId == Id && x.SessionId == sessionId)
			             .ToList())
			{
				character.RemoveEffect(effect);
			}

			CleanInvalidJoinRequests();
		}
	}

	internal void NotifyJoinRequesterLocationChanged(Guid requestId, ICharacter requester)
	{
		if (!_pendingJoinRequests.TryGetValue(requestId, out var request) || request.Resolved ||
			request.RequesterCharacterId != CharacterInstanceIdentityComparer.IdentityId(requester))
		{
			return;
		}

		if (!IsWithinTableServiceBoundary(requester.Location))
		{
			CompleteJoinRequest(request, false,
				"Your request to join the table is cancelled because you have left the restaurant service area.");
		}
	}

	private void TrackParticipant(ICharacter character, RestaurantTableSession session)
	{
		if (!character.AffectedBy<RestaurantTableParticipantEffect>(x => x.RestaurantId == Id && x.SessionId == session.Id))
		{
			character.AddEffect(new RestaurantTableParticipantEffect(character, this, session.Id));
		}
	}

	private void CleanInvalidJoinRequests()
	{
		foreach (var request in _pendingJoinRequests.Values.Where(x => !x.Resolved).ToList())
		{
			var requester = ActiveActor(request.RequesterCharacterId);
			var session = _tableSessions.OfType<RestaurantTableSession>().FirstOrDefault(x => x.Id == request.SessionId);
			if (requester is null || !IsWithinTableServiceBoundary(requester.Location) || session?.Status != RestaurantTableSessionStatus.Active)
			{
				CompleteJoinRequest(request, false, "The table-join request has expired because the requester or table service is no longer available.");
				continue;
			}

			request.ApproverCharacterIds.RemoveWhere(id => ActiveActor(id) is not { } actor ||
				!IsWithinTableServiceBoundary(actor.Location) || !session.HasAcceptedParticipant(id));
			if (!request.ApproverCharacterIds.Any())
			{
				CompleteJoinRequest(request, false, "The table-join request has expired because no current participant remains available to approve it.");
			}
		}
	}

	private void CompleteJoinRequest(PendingJoinRequest request, bool accepted, string requesterMessage)
	{
		if (request.Resolved)
		{
			return;
		}

		request.Resolved = true;
		_pendingJoinRequests.Remove(request.Id);
		ActiveActor(request.RequesterCharacterId)?.OutputHandler.Send(requesterMessage);
		foreach (var participant in Gameworld.Actors.ToList())
		{
			participant.RemoveAllEffects<Accept>(x => x.Proposal is RestaurantTableJoinProposal proposal && proposal.RequestId == request.Id, true);
			participant.RemoveAllEffects<RestaurantTableJoinRequesterEffect>(x => x.RequestId == request.Id, true);
		}
	}

	private void CancelJoinRequestsForSession(long? sessionId)
	{
		if (sessionId is null)
		{
			return;
		}

		foreach (var request in _pendingJoinRequests.Values.Where(x => x.SessionId == sessionId.Value).ToList())
		{
			CompleteJoinRequest(request, false, "The table service session has closed.");
		}
	}

	private bool IsRegisteredTable(IGameItem table, out string reason)
	{
		if (!_tableIds.Contains(table.Id))
		{
			reason = "That is not a designated restaurant table.";
			return false;
		}

		return IsRegisteredTableCandidate(table, out reason);
	}

	private bool IsRegisteredTableCandidate(IGameItem table, out string reason)
	{
		if (table.GetItemType<ITable>() is null || table.GetItemType<IContainer>() is null)
		{
			reason = "A restaurant table must be a physical item with both table and container components.";
			return false;
		}

		if (!table.TrueLocations.Any(cell => ServiceCells.Any(service => service.Id == cell.Id)))
		{
			reason = "A restaurant table must be located in one of the restaurant's table-service cells.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private bool CanPerformDuty(ICharacter employee, params EmploymentRole[] roles)
	{
		if (employee.IsAdministrator())
		{
			return true;
		}

		var employeeId = CharacterInstanceIdentityComparer.IdentityId(employee);
		return Employment.EmploymentContracts.Any(contract =>
			contract.Status == EmploymentStatus.Active &&
			CharacterInstanceIdentityComparer.IdentityId(contract.Employee) == employeeId &&
			(roles.Contains(contract.Role) || contract.Role is EmploymentRole.Manager or EmploymentRole.Proprietor));
	}

	private ICharacter? ActiveActor(long identityId)
	{
		// Dynamically loaded NPCs are always visible in a cell, but are not necessarily retained in
		// the global actor collection. Restaurant participants may be NPCs, so include the current
		// restaurant occupants as well as online actors when resolving a persisted participant ID.
		return Gameworld.Actors
			.Concat(AllShopCells.SelectMany(x => x.Characters))
			.DistinctBy(CharacterInstanceIdentityComparer.IdentityId)
			.FirstOrDefault(x => CharacterInstanceIdentityComparer.IdentityId(x) == identityId && x.Location is not null);
	}

	private IGameItem? FindRestaurantItem(long id)
	{
		var restaurantCells = AllShopCells
			.Concat(_cells.Select(x => x.Cell))
			.DistinctBy(x => x.Id)
			.ToList();
		return restaurantCells
			.SelectMany(x => x.GameItems.SelectMany(y => y.DeepItems)
				.Concat(x.Characters.SelectMany(y => y.Body.HeldOrWieldedItems.SelectMany(z => z.DeepItems))))
			.FirstOrDefault(x => x.Id == id && !x.Deleted);
	}
}
