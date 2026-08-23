using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using DbRestaurantOrder = MudSharp.Models.RestaurantOrder;
using DbRestaurantOrderItem = MudSharp.Models.RestaurantOrderItem;
using DbRestaurantPayment = MudSharp.Models.RestaurantPayment;

#nullable enable

namespace MudSharp.Economy.Shops;

public sealed class RestaurantOrder : SaveableItem, IRestaurantOrder
{
	private readonly List<IRestaurantPayment> _payments = new();
	private readonly List<IRestaurantOrderItem> _producedItems = new();
	private RestaurantOrderStatus _status;
	private decimal _amountPaid;
	private DateTime? _expectedReadyAtUtc;
	private DateTime? _readyAtUtc;
	private DateTime? _servedAtUtc;
	private long? _preparedByEmployeeId;
	private long? _servedByEmployeeId;
	private string _operationalNotes;
	private bool _revenueRecognised;

	public RestaurantOrder(Restaurant restaurant, RestaurantTableSession? session, RestaurantMenuItem menuItem,
		ICharacter orderer, ICharacter recipient, RestaurantOrderType orderType, int quantity,
		IShopPriceCalculation calculation, DateTime expectedReadyAtUtc)
	{
		Gameworld = restaurant.Gameworld;
		Restaurant = restaurant;
		TableSession = session;
		MenuItem = menuItem;
		OrderType = orderType;
		_status = RestaurantOrderStatus.Queued;
		OrdererCharacterId = CharacterInstanceIdentityComparer.IdentityId(orderer);
		OrdererCharacterName = orderer.PersonalName.GetName(NameStyle.FullName);
		RecipientCharacterId = CharacterInstanceIdentityComparer.IdentityId(recipient);
		RecipientCharacterName = recipient.PersonalName.GetName(NameStyle.FullName);
		Quantity = quantity;
		PretaxPrice = calculation.TotalPretaxPrice;
		Tax = calculation.IncludedTax;
		Price = calculation.TotalPrice;
		CreatedAtUtc = DateTime.UtcNow;
		LastUpdatedAtUtc = CreatedAtUtc;
		_expectedReadyAtUtc = expectedReadyAtUtc;
		_operationalNotes = string.Empty;

		using (new FMDB())
		{
			var dbitem = new DbRestaurantOrder
			{
				RestaurantShopId = restaurant.Id,
				RestaurantTableSessionId = session?.Id,
				RestaurantMenuItemId = menuItem.Id,
				OrderType = (int)orderType,
				Status = (int)_status,
				OrdererCharacterId = OrdererCharacterId,
				OrdererCharacterName = OrdererCharacterName,
				RecipientCharacterId = RecipientCharacterId,
				RecipientCharacterName = RecipientCharacterName,
				Quantity = quantity,
				PretaxPrice = PretaxPrice,
				Tax = Tax,
				Price = Price,
				AmountPaid = 0.0M,
				RevenueRecognised = false,
				CreatedAtUtc = CreatedAtUtc,
				LastUpdatedAtUtc = LastUpdatedAtUtc,
				ExpectedReadyAtUtc = _expectedReadyAtUtc,
				OperationalNotes = string.Empty
			};
			FMDB.Context.RestaurantOrders.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public RestaurantOrder(DbRestaurantOrder order, Restaurant restaurant, RestaurantTableSession? session,
		RestaurantMenuItem menuItem)
	{
		Gameworld = restaurant.Gameworld;
		Restaurant = restaurant;
		TableSession = session;
		MenuItem = menuItem;
		_id = order.Id;
		OrderType = Enum.IsDefined(typeof(RestaurantOrderType), order.OrderType)
			? (RestaurantOrderType)order.OrderType
			: RestaurantOrderType.DineIn;
		_status = Enum.IsDefined(typeof(RestaurantOrderStatus), order.Status)
			? (RestaurantOrderStatus)order.Status
			: RestaurantOrderStatus.Failed;
		OrdererCharacterId = order.OrdererCharacterId;
		OrdererCharacterName = order.OrdererCharacterName;
		RecipientCharacterId = order.RecipientCharacterId;
		RecipientCharacterName = order.RecipientCharacterName;
		Quantity = order.Quantity;
		PretaxPrice = order.PretaxPrice;
		Tax = order.Tax;
		Price = order.Price;
		_amountPaid = order.AmountPaid;
		_revenueRecognised = order.RevenueRecognised;
		CreatedAtUtc = DateTime.SpecifyKind(order.CreatedAtUtc, DateTimeKind.Utc);
		LastUpdatedAtUtc = DateTime.SpecifyKind(order.LastUpdatedAtUtc, DateTimeKind.Utc);
		_expectedReadyAtUtc = order.ExpectedReadyAtUtc.HasValue ? DateTime.SpecifyKind(order.ExpectedReadyAtUtc.Value, DateTimeKind.Utc) : null;
		_readyAtUtc = order.ReadyAtUtc.HasValue ? DateTime.SpecifyKind(order.ReadyAtUtc.Value, DateTimeKind.Utc) : null;
		_servedAtUtc = order.ServedAtUtc.HasValue ? DateTime.SpecifyKind(order.ServedAtUtc.Value, DateTimeKind.Utc) : null;
		_preparedByEmployeeId = order.PreparedByEmployeeId;
		_servedByEmployeeId = order.ServedByEmployeeId;
		_operationalNotes = order.OperationalNotes;

		foreach (var payment in order.Payments.OrderBy(x => x.Id))
		{
			_payments.Add(new RestaurantPayment(payment, this));
		}

		foreach (var item in order.ProducedItems.OrderBy(x => x.Id))
		{
			_producedItems.Add(new RestaurantOrderItem(item, this));
		}
	}

	public override string FrameworkItemType => "RestaurantOrder";
	public IRestaurant Restaurant { get; }
	public IRestaurantTableSession? TableSession { get; }
	public IRestaurantMenuItem MenuItem { get; }
	public RestaurantOrderType OrderType { get; }
	public RestaurantOrderStatus Status => _status;
	public long OrdererCharacterId { get; }
	public string OrdererCharacterName { get; }
	public long RecipientCharacterId { get; }
	public string RecipientCharacterName { get; }
	public int Quantity { get; }
	public decimal PretaxPrice { get; }
	public decimal Tax { get; }
	public decimal Price { get; }
	public decimal AmountPaid => _amountPaid;
	internal decimal GrossAmountPaid => _payments.Where(x => !x.IsRefund).Sum(x => x.Amount);
	public decimal OutstandingBalance => RestaurantServiceRules.OutstandingLiability(Status, Price, GrossAmountPaid);
	public DateTime CreatedAtUtc { get; }
	public DateTime LastUpdatedAtUtc { get; private set; }
	public DateTime? ExpectedReadyAtUtc => _expectedReadyAtUtc;
	public DateTime? ReadyAtUtc => _readyAtUtc;
	public DateTime? ServedAtUtc => _servedAtUtc;
	public long? PreparedByEmployeeId => _preparedByEmployeeId;
	public long? ServedByEmployeeId => _servedByEmployeeId;
	public bool RevenueRecognised => _revenueRecognised;
	public string OperationalNotes => _operationalNotes;
	public IEnumerable<IRestaurantPayment> Payments => _payments;
	public IEnumerable<IRestaurantOrderItem> ProducedItems => _producedItems;

	public void AddPayment(ICharacter payer, decimal amount, string paymentMethod, string reference, bool refund = false)
	{
		if (amount <= 0.0M)
		{
			return;
		}

		var payment = new RestaurantPayment(this, payer, amount, paymentMethod, reference, refund);
		_payments.Add(payment);
		_amountPaid += refund ? -amount : amount;
		Touch();
	}

	public void MarkPreparing(ICharacter? chef, string note = "")
	{
		if (_status is RestaurantOrderStatus.Cancelled or RestaurantOrderStatus.Refunded or RestaurantOrderStatus.Served)
		{
			return;
		}

		_status = RestaurantOrderStatus.Preparing;
		_preparedByEmployeeId = chef is null ? null : CharacterInstanceIdentityComparer.IdentityId(chef);
		AppendNote(note);
		Touch();
	}

	public void AddProducedItem(IGameItem item, RestaurantOrderItemRole role)
	{
		var produced = new RestaurantOrderItem(this, item, role);
		_producedItems.Add(produced);
		Touch();
	}

	public void MarkReady(ICharacter? employee, string note = "")
	{
		if (_status is RestaurantOrderStatus.Cancelled or RestaurantOrderStatus.Refunded or RestaurantOrderStatus.Served)
		{
			return;
		}

		_status = RestaurantOrderStatus.ReadyForService;
		_readyAtUtc = DateTime.UtcNow;
		_preparedByEmployeeId ??= employee is null ? null : CharacterInstanceIdentityComparer.IdentityId(employee);
		AppendNote(note);
		Touch();
	}

	public void MarkServed(ICharacter? server, string note = "")
	{
		if (_status != RestaurantOrderStatus.ReadyForService)
		{
			return;
		}

		_status = RestaurantOrderStatus.Served;
		_servedAtUtc = DateTime.UtcNow;
		_servedByEmployeeId = server is null ? null : CharacterInstanceIdentityComparer.IdentityId(server);
		AppendNote(note);
		Touch();
	}

	public void MarkFailed(string note)
	{
		if (_status is RestaurantOrderStatus.Served or RestaurantOrderStatus.Cancelled or RestaurantOrderStatus.Refunded)
		{
			return;
		}

		_status = RestaurantOrderStatus.Failed;
		AppendNote(note);
		Touch();
	}

	public void MarkCancelled(string note)
	{
		if (_status is RestaurantOrderStatus.Served or RestaurantOrderStatus.Refunded)
		{
			return;
		}

		_status = RestaurantOrderStatus.Cancelled;
		AppendNote(note);
		Touch();
	}

	public void MarkRefunded(string note)
	{
		_status = RestaurantOrderStatus.Refunded;
		AppendNote(note);
		Touch();
	}

	public void AddOperationalNote(string note)
	{
		AppendNote(note);
		Touch();
	}

	public void RecogniseRevenue()
	{
		if (_revenueRecognised)
		{
			return;
		}

		_revenueRecognised = true;
		Touch();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.RestaurantOrders.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Status = (int)_status;
		dbitem.AmountPaid = _amountPaid;
		dbitem.RevenueRecognised = _revenueRecognised;
		dbitem.LastUpdatedAtUtc = LastUpdatedAtUtc;
		dbitem.ExpectedReadyAtUtc = _expectedReadyAtUtc;
		dbitem.ReadyAtUtc = _readyAtUtc;
		dbitem.ServedAtUtc = _servedAtUtc;
		dbitem.PreparedByEmployeeId = _preparedByEmployeeId;
		dbitem.ServedByEmployeeId = _servedByEmployeeId;
		dbitem.OperationalNotes = _operationalNotes;
		Changed = false;
	}

	private void AppendNote(string note)
	{
		if (string.IsNullOrWhiteSpace(note))
		{
			return;
		}

		_operationalNotes = string.IsNullOrWhiteSpace(_operationalNotes)
			? note
			: $"{_operationalNotes}\n{note}";
	}

	private void Touch()
	{
		LastUpdatedAtUtc = DateTime.UtcNow;
		Changed = true;
	}
}

public sealed class RestaurantPayment : SaveableItem, IRestaurantPayment
{
	public RestaurantPayment(RestaurantOrder order, ICharacter payer, decimal amount, string paymentMethod,
		string reference, bool isRefund)
	{
		Gameworld = order.Gameworld;
		Order = order;
		PayerCharacterId = CharacterInstanceIdentityComparer.IdentityId(payer);
		PayerCharacterName = payer.PersonalName.GetName(NameStyle.FullName);
		Amount = amount;
		IsRefund = isRefund;
		PaymentMethod = paymentMethod;
		Reference = reference;
		PaidAtUtc = DateTime.UtcNow;

		using (new FMDB())
		{
			var dbitem = new DbRestaurantPayment
			{
				RestaurantOrderId = order.Id,
				PayerCharacterId = PayerCharacterId,
				PayerCharacterName = PayerCharacterName,
				Amount = amount,
				IsRefund = isRefund,
				PaymentMethod = paymentMethod,
				Reference = reference,
				PaidAtUtc = PaidAtUtc
			};
			FMDB.Context.RestaurantPayments.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public RestaurantPayment(DbRestaurantPayment payment, RestaurantOrder order)
	{
		Gameworld = order.Gameworld;
		Order = order;
		_id = payment.Id;
		PayerCharacterId = payment.PayerCharacterId;
		PayerCharacterName = payment.PayerCharacterName;
		Amount = payment.Amount;
		IsRefund = payment.IsRefund;
		PaymentMethod = payment.PaymentMethod;
		Reference = payment.Reference;
		PaidAtUtc = DateTime.SpecifyKind(payment.PaidAtUtc, DateTimeKind.Utc);
	}

	public override string FrameworkItemType => "RestaurantPayment";
	public IRestaurantOrder Order { get; }
	public long PayerCharacterId { get; }
	public string PayerCharacterName { get; }
	public decimal Amount { get; }
	public bool IsRefund { get; }
	public string PaymentMethod { get; }
	public string Reference { get; }
	public DateTime PaidAtUtc { get; }

	public override void Save()
	{
		Changed = false;
	}
}

public sealed class RestaurantOrderItem : SaveableItem, IRestaurantOrderItem
{
	private bool _delivered;
	private DateTime? _deliveredAtUtc;

	public RestaurantOrderItem(RestaurantOrder order, IGameItem item, RestaurantOrderItemRole role)
	{
		Gameworld = order.Gameworld;
		Order = order;
		GameItemId = item.Id;
		Role = role;
		CreatedAtUtc = DateTime.UtcNow;

		using (new FMDB())
		{
			var dbitem = new DbRestaurantOrderItem
			{
				RestaurantOrderId = order.Id,
				GameItemId = GameItemId,
				Role = (int)role,
				Delivered = false,
				CreatedAtUtc = CreatedAtUtc
			};
			FMDB.Context.RestaurantOrderItems.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public RestaurantOrderItem(DbRestaurantOrderItem item, RestaurantOrder order)
	{
		Gameworld = order.Gameworld;
		Order = order;
		_id = item.Id;
		GameItemId = item.GameItemId;
		Role = Enum.IsDefined(typeof(RestaurantOrderItemRole), item.Role)
			? (RestaurantOrderItemRole)item.Role
			: RestaurantOrderItemRole.Product;
		_delivered = item.Delivered;
		CreatedAtUtc = DateTime.SpecifyKind(item.CreatedAtUtc, DateTimeKind.Utc);
		_deliveredAtUtc = item.DeliveredAtUtc.HasValue ? DateTime.SpecifyKind(item.DeliveredAtUtc.Value, DateTimeKind.Utc) : null;
	}

	public override string FrameworkItemType => "RestaurantOrderItem";
	public IRestaurantOrder Order { get; }
	public long? GameItemId { get; }
	public RestaurantOrderItemRole Role { get; }
	public bool Delivered => _delivered;
	public DateTime CreatedAtUtc { get; }
	public DateTime? DeliveredAtUtc => _deliveredAtUtc;

	public void MarkDelivered()
	{
		if (_delivered)
		{
			return;
		}

		_delivered = true;
		_deliveredAtUtc = DateTime.UtcNow;
		Changed = true;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.RestaurantOrderItems.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Delivered = _delivered;
		dbitem.DeliveredAtUtc = _deliveredAtUtc;
		Changed = false;
	}
}
