using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Listeners;

namespace MudSharp.Economy.Property;

public class PropertyLease : SaveableItem, IPropertyLease
{
	public PropertyLease(Models.PropertyLease dbitem, IProperty property)
	{
		Gameworld = property.Gameworld;
		_property = property;
		_id = dbitem.Id;
		_leaseOrder = property.LeaseOrder?.Id == dbitem.LeaseOrderId
			? property.LeaseOrder
			: property.ExpiredLeaseOrders.First(x => x.Id == dbitem.LeaseOrderId);
		_leaseHolderReference = new FrameworkItemReference(dbitem.LeaseholderReference, Gameworld);
		_pricePerInterval = dbitem.PricePerInterval;
		_bondPayment = dbitem.BondPayment;
		_bondClaimed = dbitem.BondClaimed;
		_paymentBalance = dbitem.PaymentBalance;
		_leaseStart = new MudDateTime(dbitem.LeaseStart, Gameworld);
		_leaseEnd = new MudDateTime(dbitem.LeaseEnd, Gameworld);
		_lastLeasePayment = new MudDateTime(dbitem.LastLeasePayment, Gameworld);
		_autoRenew = dbitem.AutoRenew;
		_interval = RecurringInterval.Parse(dbitem.Interval);
		SetupListeners();
	}

	public PropertyLease(IProperty property, IFrameworkItem leaseholder, MudDateTime startDate, MudDateTime endDate,
		IPropertyLeaseOrder leaseOrder)
	{
		Gameworld = property.Gameworld;
		_property = property;
		_leaseOrder = leaseOrder;
		_leaseholder = leaseholder;
		_leaseHolderReference = new FrameworkItemReference
			{ Gameworld = Gameworld, FrameworkItemType = leaseholder.FrameworkItemType, Id = leaseholder.Id };
		_pricePerInterval = leaseOrder.PricePerInterval;
		_bondPayment = leaseOrder.BondRequired;
		_paymentBalance = 0.0M;
		_leaseStart = startDate;
		_leaseEnd = endDate;
		_lastLeasePayment = startDate;
		_autoRenew = leaseOrder.AllowAutoRenew;
		_interval = leaseOrder.Interval;
		using (new FMDB())
		{
			var dbitem = new Models.PropertyLease
			{
				PropertyId = property.Id,
				LeaseOrderId = LeaseOrder.Id,
				LeaseholderReference = _leaseHolderReference.ToString(),
				PricePerInterval = PricePerInterval,
				BondPayment = BondPayment,
				PaymentBalance = PaymentBalance,
				LeaseStart = LeaseStart.GetDateTimeString(),
				LeaseEnd = LeaseEnd.GetDateTimeString(),
				LastLeasePayment = LastLeasePayment.GetDateTimeString(),
				AutoRenew = AutoRenew,
				BondReturned = BondReturned,
				Interval = Interval.ToString(),
				TenantInfo = new XElement("Tenants",
					from tenant in _declaredTenantReferences
					select new XElement("Tenant", new XAttribute("id", tenant.Id),
						new XAttribute("type", tenant.FrameworkItemType))
				).ToString()
			};
			FMDB.Context.PropertyLeases.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		SetupListeners();
	}

	public PropertyLease(IProperty property, IFrameworkItem leaseholder, IEnumerable<IFrameworkItem> tenants,
		MudDateTime startDate, MudDateTime lastLeasePayment, IPropertyLeaseOrder leaseOrder, decimal price,
		decimal bond, decimal carryoverBalance)
	{
		Gameworld = property.Gameworld;
		_property = property;
		_leaseOrder = leaseOrder;
		_leaseholder = leaseholder;
		_leaseHolderReference = new FrameworkItemReference
			{ Gameworld = Gameworld, FrameworkItemType = leaseholder.FrameworkItemType, Id = leaseholder.Id };
		_pricePerInterval = price;
		_bondPayment = bond;
		_paymentBalance = carryoverBalance;
		_leaseStart = startDate;
		_leaseEnd = startDate + leaseOrder.MinimumLeaseDuration;
		_declaredTenants = new List<IFrameworkItem>(tenants);
		_declaredTenantReferences.AddRange(tenants.Select(x => new FrameworkItemReference
			{ Gameworld = Gameworld, FrameworkItemType = x.FrameworkItemType, Id = x.Id }));
		_lastLeasePayment = lastLeasePayment;
		_autoRenew = true;
		_interval = leaseOrder.Interval;
		using (new FMDB())
		{
			var dbitem = new Models.PropertyLease
			{
				PropertyId = property.Id,
				LeaseOrderId = LeaseOrder.Id,
				LeaseholderReference = _leaseHolderReference.ToString(),
				PricePerInterval = PricePerInterval,
				BondPayment = BondPayment,
				PaymentBalance = PaymentBalance,
				LeaseStart = LeaseStart.GetDateTimeString(),
				LeaseEnd = LeaseEnd.GetDateTimeString(),
				LastLeasePayment = LastLeasePayment.GetDateTimeString(),
				AutoRenew = AutoRenew,
				BondReturned = BondReturned,
				Interval = Interval.ToString(),
				TenantInfo = new XElement("Tenants",
					from tenant in _declaredTenantReferences
					select new XElement("Tenant", new XAttribute("id", tenant.Id),
						new XAttribute("type", tenant.FrameworkItemType))
				).ToString()
			};
			FMDB.Context.PropertyLeases.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		SetupListeners();
	}

	private IProperty _property;
	private FrameworkItemReference _leaseHolderReference;
	private IFrameworkItem _leaseholder;
	private IPropertyLeaseOrder _leaseOrder;
	private decimal _pricePerInterval;
	private decimal _bondPayment;
	private decimal _paymentBalance;
	private decimal _bondClaimed;
	private RecurringInterval _interval;
	private MudDateTime _leaseStart;
	private MudDateTime _leaseEnd;
	private MudDateTime _lastLeasePayment;
	private bool _autoRenew;
	private bool _bondReturned;
	private readonly List<FrameworkItemReference> _declaredTenantReferences = new();
	private List<IFrameworkItem> _declaredTenants;
	private ITemporalListener _leaseEndListener;
	private ITemporalListener _paymentIntervalListener;

	private void SetupListeners()
	{
		_leaseEndListener = new DateListener(_leaseEnd, 0, DoLeaseEnd, Array.Empty<object>(), $"End of Lease for Property #{Property.Id} {Property.Name.ColourName()}");
		Gameworld.Add(_leaseEndListener);
		_paymentIntervalListener =
			_interval.CreateRecurringListenerFromInterval(_lastLeasePayment, DoLeaseInterval,
				Array.Empty<object>(), $"Lease Payment for Property #{_property.Id} {_property.Name.ColourName()}");
	}

	private void ReleaseListeners()
	{
		_leaseEndListener?.CancelListener();
		_paymentIntervalListener?.CancelListener();
	}

	private void DoLeaseInterval(object[] objects)
	{
		// Don't charge if this listener fires after the lease has ended
		if (_leaseEnd.Calendar.CurrentDateTime >= LeaseEnd)
		{
			return;
		}

		_paymentBalance -= PricePerInterval;
		Changed = true;
	}

	private void DoLeaseEnd(object[] objects)
	{
		Property.ExpireLease(this);
		foreach (var owner in Property.PropertyOwners)
		{
			if (owner.RevenueAccount != null)
			{
				owner.RevenueAccount.DepositFromTransaction(_bondClaimed * owner.ShareOfOwnership,
					$"Claimed bond from {Property.Name}");
				owner.RevenueAccount.Bank.CurrencyReserves[Property.EconomicZone.Currency] +=
					_bondClaimed * owner.ShareOfOwnership;
			}
		}

		_leaseOrder.DoEndOfLease(this);
		ReleaseListeners();
	}

	public void TerminateLease()
	{
		AutoRenew = false;
		foreach (var key in Property.PropertyKeys.Where(x => !x.IsReturned).ToList())
		{
			key.GameItem = key.GameItem.DeepCopy(true);
			_bondClaimed += key.CostToReplace;
			key.IsReturned = true;
		}

		if (_bondClaimed > _bondPayment)
		{
			_bondClaimed = _bondPayment;
		}

		DoLeaseEnd(Array.Empty<object>());
	}

	public bool IsAuthorisedLeaseHolder(ICharacter who)
	{
		return Leaseholder == who || (Leaseholder is IClan clan && who.ClanMemberships.Any(x =>
			x.Clan == clan && x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty)));
	}

	public override string FrameworkItemType => "PropertyLease";

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.PropertyLeases.Find(Id);
		dbitem.LeaseOrderId = LeaseOrder.Id;
		dbitem.LeaseholderReference = _leaseHolderReference.ToString();
		dbitem.PricePerInterval = PricePerInterval;
		dbitem.BondPayment = BondPayment;
		dbitem.BondClaimed = BondClaimed;
		dbitem.PaymentBalance = PaymentBalance;
		dbitem.LeaseStart = LeaseStart.GetDateTimeString();
		dbitem.LeaseEnd = LeaseEnd.GetDateTimeString();
		dbitem.LastLeasePayment = LastLeasePayment.GetDateTimeString();
		dbitem.AutoRenew = AutoRenew;
		dbitem.BondReturned = BondReturned;
		dbitem.Interval = Interval.ToString();
		dbitem.TenantInfo = new XElement("Tenants",
			from tenant in _declaredTenantReferences
			select new XElement("Tenant", new XAttribute("id", tenant.Id),
				new XAttribute("type", tenant.FrameworkItemType))
		).ToString();

		FMDB.Context.SaveChanges();
		Changed = false;
	}

	#endregion

	#region Implementation of IPropertyLease

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.PropertyLeases.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.PropertyLeases.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public IPropertyLeaseOrder LeaseOrder => _leaseOrder;

	public void MakePayment(decimal paymentAmount)
	{
		foreach (var owner in _property.PropertyOwners)
		{
			owner.RevenueAccount.DepositFromTransaction(paymentAmount * owner.ShareOfOwnership,
				$"Lease Revenue from {_property.Name}");
		}

		PaymentBalance += paymentAmount;
	}

	private void EnsureLoadTenants()
	{
		_declaredTenants ??= _declaredTenantReferences.SelectNotNull(x => x.GetItem).ToList();
	}

	public bool IsTenant(ICharacter character, bool ignoreIndirectTenancy)
	{
		EnsureLoadTenants();
		if (_declaredTenants.Any(x => x == character))
		{
			return true;
		}

		if (ignoreIndirectTenancy)
		{
			return false;
		}

		if (_declaredTenants.OfType<IClan>().Any(x => character.ClanMemberships.Any(y =>
			    y.Clan == x && y.NetPrivileges.HasFlag(ClanPrivilegeType.CanAccessLeasedProperties))))
		{
			return true;
		}

		if (_declaredTenants.OfType<IShop>().Any(x => x.IsManager(character)))
		{
			return true;
		}

		return false;
	}

	public void DeclareTenant(IFrameworkItem tenant)
	{
		EnsureLoadTenants();
		if (!_declaredTenants.Contains(tenant))
		{
			_declaredTenants.Add(tenant);
			_declaredTenantReferences.Add(new FrameworkItemReference
				{ Gameworld = Gameworld, FrameworkItemType = tenant.FrameworkItemType, Id = tenant.Id });
			Changed = true;
		}
	}

	public void RemoveTenant(IFrameworkItem tenant)
	{
		_declaredTenantReferences.RemoveAll(x => x.GetItem.Equals(tenant));
		Changed = true;
		EnsureLoadTenants();
		if (_declaredTenants.Contains(tenant))
		{
			_declaredTenants.Remove(tenant);
		}
	}

	public IProperty Property
	{
		get => _property;
		set
		{
			_property = value;
			Changed = true;
		}
	}

	public IFrameworkItem Leaseholder
	{
		get => _leaseholder ??= _leaseHolderReference.GetItem;
		set
		{
			_leaseholder = value;
			_leaseHolderReference = new FrameworkItemReference(value.Id, value.FrameworkItemType, Gameworld);
			Changed = true;
		}
	}

	public decimal PricePerInterval
	{
		get => _pricePerInterval;
		set
		{
			_pricePerInterval = value;
			Changed = true;
		}
	}

	public decimal BondPayment
	{
		get => _bondPayment;
		set
		{
			_bondPayment = value;
			Changed = true;
		}
	}

	public decimal PaymentBalance
	{
		get => _paymentBalance;
		set
		{
			_paymentBalance = value;
			Changed = true;
		}
	}

	public decimal BondClaimed
	{
		get => _bondClaimed;
		set
		{
			_bondClaimed = value;
			Changed = true;
		}
	}

	public RecurringInterval Interval
	{
		get => _interval;
		set
		{
			_interval = value;
			Changed = true;
		}
	}

	public MudDateTime LeaseStart
	{
		get => _leaseStart;
		set
		{
			_leaseStart = value;
			Changed = true;
		}
	}

	public MudDateTime LeaseEnd
	{
		get => _leaseEnd;
		set
		{
			_leaseEnd = value;
			Changed = true;
		}
	}

	public MudDateTime LastLeasePayment
	{
		get => _lastLeasePayment;
		set
		{
			_lastLeasePayment = value;
			Changed = true;
		}
	}

	public bool AutoRenew
	{
		get => _autoRenew;
		set
		{
			_autoRenew = value;
			Changed = true;
		}
	}

	public bool BondReturned
	{
		get => _bondReturned;
		set
		{
			_bondReturned = value;
			Changed = true;
		}
	}

	public IEnumerable<IFrameworkItem> DeclaredTenants
	{
		get
		{
			EnsureLoadTenants();
			return _declaredTenants;
		}
	}

	#endregion
}