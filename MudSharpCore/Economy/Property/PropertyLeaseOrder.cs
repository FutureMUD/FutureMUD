using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MoreLinq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Intervals;
using Org.BouncyCastle.Asn1.Cms;

namespace MudSharp.Economy.Property;

public class PropertyLeaseOrder : SaveableItem, IPropertyLeaseOrder
{
	public PropertyLeaseOrder(Models.PropertyLeaseOrder dbitem, IProperty property)
	{
		Gameworld = property.Gameworld;
		_id = dbitem.Id;
		_property = property;
		_pricePerInterval = dbitem.PricePerInterval;
		_bondRequired = dbitem.BondRequired;
		if (!RecurringInterval.TryParse(dbitem.Interval, out _interval))
		{
			_interval = new RecurringInterval { IntervalAmount = 14, Type = IntervalType.Daily, Modifier = 0 };
		}

		_minimumLeaseDuration = TimeSpan.FromDays(dbitem.MinimumLeaseDurationDays);
		_maximumLeaseDuration = TimeSpan.FromDays(dbitem.MaximumLeaseDurationDays);
		_canLeaseProgCharacter = Gameworld.FutureProgs.Get(dbitem.CanLeaseProgCharacterId ?? 0L);
		_canLeaseProgClan = Gameworld.FutureProgs.Get(dbitem.CanLeaseProgClanId ?? 0L);
		_allowAutoRenew = dbitem.AllowAutoRenew;
		_automaticallyRelistAfterLeaseTerm = dbitem.AutomaticallyRelistAfterLeaseTerm;
		_allowLeaseNovation = dbitem.AllowLeaseNovation;
		_feeIncreasePercentageAfterLeaseTerm = dbitem.FeeIncreasePercentageAfterLeaseTerm;
		_listedForLease = dbitem.ListedForLease;

		foreach (var element in XElement.Parse(dbitem.PropertyOwnerConsentInfo).Elements("Owner"))
		{
			var ownerId = long.Parse(element.Attribute("id").Value);
			var ownerType = element.Attribute("type").Value;
			var owner = property.PropertyOwners.FirstOrDefault(x =>
				x.Owner.FrameworkItemEquals(ownerId, ownerType));
			if (owner == null)
			{
#if DEBUG
				throw new ApplicationException("Property Owner not found");
#else
					continue;
#endif
			}

			_propertyOwnerConsent[owner] = bool.Parse(element.Attribute("consent").Value);
		}
	}

	public PropertyLeaseOrder(IProperty property, decimal pricePerInterval, decimal bondRequired)
	{
		Gameworld = property.Gameworld;
		_property = property;
		_pricePerInterval = pricePerInterval;
		_bondRequired = bondRequired;
		_interval = new RecurringInterval { IntervalAmount = 14, Type = IntervalType.Daily, Modifier = 0 };
		_minimumLeaseDuration = TimeSpan.FromDays(Gameworld.GetStaticInt("MinimumLeaseDurationDays"));
		_maximumLeaseDuration = TimeSpan.FromDays(Gameworld.GetStaticInt("MaximumLeaseDurationDays"));
		_canLeaseProgCharacter = Gameworld.FutureProgs.GetByName("AlwaysTrue");
		_canLeaseProgClan = Gameworld.FutureProgs.GetByName("AlwaysTrue");
		_allowAutoRenew = true;
		_automaticallyRelistAfterLeaseTerm = true;
		_allowLeaseNovation = true;
		_feeIncreasePercentageAfterLeaseTerm = 0.0M;

		using (new FMDB())
		{
			var dbitem = new Models.PropertyLeaseOrder
			{
				PropertyId = property.Id,
				PricePerInterval = PricePerInterval,
				BondRequired = BondRequired,
				Interval = Interval.ToString(),
				CanLeaseProgCharacterId = CanLeaseProgCharacter?.Id,
				CanLeaseProgClanId = CanLeaseProgClan?.Id,
				MinimumLeaseDurationDays = MinimumLeaseDuration.TotalDays,
				MaximumLeaseDurationDays = MaximumLeaseDuration.TotalDays,
				AllowAutoRenew = AllowAutoRenew,
				AutomaticallyRelistAfterLeaseTerm = AutomaticallyRelistAfterLeaseTerm,
				AllowLeaseNovation = AllowLeaseNovation,
				ListedForLease = ListedForLease,
				FeeIncreasePercentageAfterLeaseTerm = FeeIncreasePercentageAfterLeaseTerm,
				PropertyOwnerConsentInfo = new XElement("Owners",
						from owner in _propertyOwnerConsent
						select new XElement("Owner", new XAttribute("id", owner.Key.Owner.Id),
							new XAttribute("type", owner.Key.Owner.FrameworkItemType),
							new XAttribute("consent", owner.Value))
					)
					.ToString()
			};
			FMDB.Context.PropertyLeaseOrders.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private IProperty _property;
	private decimal _pricePerInterval;
	private decimal _bondRequired;
	private readonly Dictionary<IPropertyOwner, bool> _propertyOwnerConsent = new();
	private RecurringInterval _interval;
	private IFutureProg _canLeaseProgCharacter;
	private IFutureProg _canLeaseProgClan;
	private TimeSpan _minimumLeaseDuration;
	private TimeSpan _maximumLeaseDuration;
	private bool _allowAutoRenew;
	private bool _automaticallyRelistAfterLeaseTerm;
	private bool _allowLeaseNovation;
	private bool _listedForLease;
	private decimal _feeIncreasePercentageAfterLeaseTerm;

	public override string FrameworkItemType => "PropertyLeaseOrder";

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.PropertyLeaseOrders.Find(Id);
		dbitem.PricePerInterval = _pricePerInterval;
		dbitem.BondRequired = _bondRequired;
		dbitem.FeeIncreasePercentageAfterLeaseTerm = _feeIncreasePercentageAfterLeaseTerm;
		dbitem.Interval = _interval.ToString();
		dbitem.CanLeaseProgCharacterId = CanLeaseProgCharacter?.Id;
		dbitem.CanLeaseProgClanId = CanLeaseProgClan?.Id;
		dbitem.MinimumLeaseDurationDays = _minimumLeaseDuration.TotalDays;
		dbitem.MaximumLeaseDurationDays = _maximumLeaseDuration.TotalDays;
		dbitem.AllowAutoRenew = _allowAutoRenew;
		dbitem.AllowLeaseNovation = _allowLeaseNovation;
		dbitem.AutomaticallyRelistAfterLeaseTerm = _automaticallyRelistAfterLeaseTerm;
		dbitem.ListedForLease = _listedForLease;
		dbitem.PropertyOwnerConsentInfo = new XElement("Owners",
				from owner in _propertyOwnerConsent
				select new XElement("Owner", new XAttribute("id", owner.Key.Owner.Id),
					new XAttribute("type", owner.Key.Owner.FrameworkItemType), new XAttribute("consent", owner.Value))
			)
			.ToString();
		Changed = false;
	}

	#endregion

	#region Implementation of IPropertyLeaseOrder

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.PropertyLeaseOrders.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.PropertyLeaseOrders.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public void DoEndOfLease(IPropertyLease oldLease)
	{
		_pricePerInterval = Math.Round(_pricePerInterval * (1.0M + _feeIncreasePercentageAfterLeaseTerm));
		_bondRequired = Math.Round(_bondRequired * (1.0M + _feeIncreasePercentageAfterLeaseTerm));
		Changed = true;

		if (oldLease.AutoRenew && oldLease.PaymentBalance >= 0.0M && oldLease.BondClaimed <= 0.0M)
		{
			Property.Lease = RenewLease(oldLease);
			return;
		}

		if (_automaticallyRelistAfterLeaseTerm)
		{
			_listedForLease = true;
		}
		else
		{
			if (Property.LeaseOrder == this)
			{
				Property.LeaseOrder = null;
			}
		}
	}

	public IPropertyLease RenewLease(IPropertyLease oldLease)
	{
		return new PropertyLease(Property, oldLease.Leaseholder, oldLease.DeclaredTenants, oldLease.LeaseEnd,
			oldLease.LastLeasePayment, this, _pricePerInterval,
			oldLease.BondPayment, oldLease.PaymentBalance);
	}

	public IPropertyLease CreateLease(IFrameworkItem lesee, TimeSpan duration)
	{
		return new PropertyLease(Property, lesee,
			Property.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime,
			Property.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime + duration, this);
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

	public decimal PricePerInterval
	{
		get => _pricePerInterval;
		set
		{
			_pricePerInterval = value;
			Changed = true;
		}
	}

	public decimal BondRequired
	{
		get => _bondRequired;
		set
		{
			_bondRequired = value;
			Changed = true;
		}
	}

	public IReadOnlyDictionary<IPropertyOwner, bool> PropertyOwnerConsent => _propertyOwnerConsent;

	public void SetConsent(IPropertyOwner owner)
	{
		_propertyOwnerConsent[owner] = true;
		ListedForLease = _propertyOwnerConsent.All(x => x.Value) && Property.LeaseOrder == this;
		Changed = true;
	}

	public void ChangeConsentDueToSale(IPropertyOwner newOwner)
	{
		var consent = _propertyOwnerConsent.All(x => x.Value);
		_propertyOwnerConsent.Clear();
		_propertyOwnerConsent[newOwner] = consent;
		ListedForLease = _propertyOwnerConsent.All(x => x.Value) && Property.LeaseOrder == this;
		Changed = true;
	}

	public void ResetConsent()
	{
		var old = _propertyOwnerConsent.ToDictionary();
		_propertyOwnerConsent.Clear();
		foreach (var owner in Property.PropertyOwners)
		{
			_propertyOwnerConsent[owner] = old.ContainsKey(owner) && old[owner];
		}

		ListedForLease = _propertyOwnerConsent.All(x => x.Value) && Property.LeaseOrder == this;
		Changed = true;
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

	public IFutureProg CanLeaseProgCharacter
	{
		get => _canLeaseProgCharacter;
		set
		{
			_canLeaseProgCharacter = value;
			Changed = true;
		}
	}

	public IFutureProg CanLeaseProgClan
	{
		get => _canLeaseProgClan;
		set
		{
			_canLeaseProgClan = value;
			Changed = true;
		}
	}

	public TimeSpan MinimumLeaseDuration
	{
		get => _minimumLeaseDuration;
		set
		{
			_minimumLeaseDuration = value;
			Changed = true;
		}
	}

	public TimeSpan MaximumLeaseDuration
	{
		get => _maximumLeaseDuration;
		set
		{
			_maximumLeaseDuration = value;
			Changed = true;
		}
	}

	public bool AllowAutoRenew
	{
		get => _allowAutoRenew;
		set
		{
			_allowAutoRenew = value;
			Changed = true;
		}
	}

	public bool AutomaticallyRelistAfterLeaseTerm
	{
		get => _automaticallyRelistAfterLeaseTerm;
		set
		{
			_automaticallyRelistAfterLeaseTerm = value;
			Changed = true;
		}
	}

	public bool AllowLeaseNovation
	{
		get => _allowLeaseNovation;
		set
		{
			_allowLeaseNovation = value;
			Changed = true;
		}
	}

	public decimal FeeIncreasePercentageAfterLeaseTerm
	{
		get => _feeIncreasePercentageAfterLeaseTerm;
		set
		{
			_feeIncreasePercentageAfterLeaseTerm = value;
			Changed = true;
		}
	}

	public bool ListedForLease
	{
		get => _listedForLease;
		set
		{
			_listedForLease = value;
			Changed = true;
		}
	}

	#endregion
}