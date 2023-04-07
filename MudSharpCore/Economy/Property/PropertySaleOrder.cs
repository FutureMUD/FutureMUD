using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MoreLinq;

namespace MudSharp.Economy.Property;

public class PropertySaleOrder : SaveableItem, IPropertySaleOrder
{
	public PropertySaleOrder(Models.PropertySaleOrder dbitem, IFuturemud gameworld, IProperty property)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_property = property;
		_reservePrice = dbitem.ReservePrice;
		_startOfListing = new MudDateTime(dbitem.StartOfListing, Gameworld);
		_durationOfListing = TimeSpan.FromDays(dbitem.DurationOfListingDays);
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

	public PropertySaleOrder(IProperty property, decimal reservePrice)
	{
		Gameworld = property.Gameworld;
		_property = property;
		_reservePrice = reservePrice;
		_startOfListing = property.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		_durationOfListing = TimeSpan.FromDays(Gameworld.GetStaticInt("DefaultPropertySaleListingDays"));
		using (new FMDB())
		{
			var dbitem = new Models.PropertySaleOrder
			{
				PropertyId = property.Id,
				ReservePrice = ReservePrice,
				OrderStatus = (int)OrderStatus,
				StartOfListing = StartOfListing?.GetDateTimeString() ?? "Never",
				DurationOfListingDays = DurationOfListing.TotalDays,
				PropertyOwnerConsentInfo = new XElement("Owners",
						from owner in _propertyOwnerConsent
						select new XElement("Owner", new XAttribute("id", owner.Key.Owner.Id),
							new XAttribute("type", owner.Key.Owner.FrameworkItemType),
							new XAttribute("consent", owner.Value))
					)
					.ToString()
			};
			FMDB.Context.PropertySaleOrders.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private IProperty _property;
	private decimal _reservePrice;
	private readonly Dictionary<IPropertyOwner, bool> _propertyOwnerConsent = new();
	private PropertySaleOrderStatus _orderStatus;
	private MudDateTime _startOfListing;
	private TimeSpan _durationOfListing;
	public override string FrameworkItemType => "PropertySaleOrder";

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.PropertySaleOrders.Find(Id);
		dbitem.DurationOfListingDays = _durationOfListing.TotalDays;
		dbitem.StartOfListing = _startOfListing?.GetDateTimeString() ?? "Never";
		dbitem.OrderStatus = (int)_orderStatus;
		dbitem.ReservePrice = _reservePrice;
		dbitem.PropertyOwnerConsentInfo =
			new XElement("Owners",
					from owner in _propertyOwnerConsent
					select new XElement("Owner", new XAttribute("id", owner.Key.Owner.Id),
						new XAttribute("type", owner.Key.Owner.FrameworkItemType),
						new XAttribute("consent", owner.Value))
				)
				.ToString();
		Changed = false;
	}

	#endregion

	#region Implementation of IPropertySaleOrder

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.PropertySaleOrders.Find(Id);
				if (dbitem != null)
				{
					dbitem.Property.SaleOrderId = null;
					FMDB.Context.PropertySaleOrders.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
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

	public bool ShowForSale => OrderStatus == PropertySaleOrderStatus.Approved &&
	                           Property.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime >= StartOfListing;

	public decimal ReservePrice
	{
		get => _reservePrice;
		set
		{
			_reservePrice = value;
			Changed = true;
		}
	}

	public IReadOnlyDictionary<IPropertyOwner, bool> PropertyOwnerConsent => _propertyOwnerConsent;

	public void SetConsent(IPropertyOwner owner)
	{
		_propertyOwnerConsent[owner] = true;
		Changed = true;
		if (_propertyOwnerConsent.All(x => x.Value))
		{
			OrderStatus = PropertySaleOrderStatus.Approved;
			if (StartOfListing < Property.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime)
			{
				StartOfListing = Property.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
			}
		}
	}

	public void ChangeConsentDueToSale(IPropertyOwner newOwner)
	{
		var consent = _propertyOwnerConsent.All(x => x.Value);
		_propertyOwnerConsent.Clear();
		_propertyOwnerConsent[newOwner] = consent;
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

		Changed = true;
	}

	public PropertySaleOrderStatus OrderStatus
	{
		get => _orderStatus;
		set
		{
			_orderStatus = value;
			Changed = true;
		}
	}

	public MudDateTime StartOfListing
	{
		get => _startOfListing;
		set
		{
			_startOfListing = value;
			Changed = true;
		}
	}

	public TimeSpan DurationOfListing
	{
		get => _durationOfListing;
		set
		{
			_durationOfListing = value;
			Changed = true;
		}
	}

	#endregion
}