using MoreLinq;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Property;

public class PropertySaleOrder : SaveableItem, IPropertySaleOrder
{
    public PropertySaleOrder(Models.PropertySaleOrder dbitem, IFuturemud gameworld, IProperty property)
    {
        Gameworld = gameworld;
        _id = dbitem.Id;
        _property = property;
        _reservePrice = dbitem.ReservePrice;
        _startOfListing = MudDateTime.FromStoredStringOrFallback(dbitem.StartOfListing, Gameworld,
            StoredMudDateTimeFallback.CurrentDateTime, "PropertySaleOrder", dbitem.Id, property.Name, "StartOfListing");
        _durationOfListing = TimeSpan.FromDays(dbitem.DurationOfListingDays);
        _orderStatus = (PropertySaleOrderStatus)dbitem.OrderStatus;
        foreach (XElement element in XElement.Parse(dbitem.PropertyOwnerConsentInfo).Elements("Owner"))
        {
            long ownerId = long.Parse(element.Attribute("id").Value);
            string ownerType = element.Attribute("type").Value;
            IPropertyOwner owner = property.PropertyOwners.FirstOrDefault(x =>
                x.OwnerId == ownerId &&
                x.OwnerFrameworkItemType.EqualTo(ownerType));
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
            Models.PropertySaleOrder dbitem = new()
            {
                PropertyId = property.Id,
                ReservePrice = ReservePrice,
                OrderStatus = (int)OrderStatus,
                StartOfListing = StartOfListing?.GetDateTimeString() ?? "Never",
                DurationOfListingDays = DurationOfListing.TotalDays,
                PropertyOwnerConsentInfo = new XElement("Owners",
                        from owner in _propertyOwnerConsent
                        select new XElement("Owner", new XAttribute("id", owner.Key.OwnerId),
                            new XAttribute("type", owner.Key.OwnerFrameworkItemType),
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

    public ProgVariableTypes Type => ProgVariableTypes.PropertySaleOrder;
    public object GetObject => this;

    public IProgVariable GetProperty(string property)
    {
        return property.ToLowerInvariant() switch
        {
            "id" => new NumberVariable(Id),
            "name" => new TextVariable($"{Property.Name} sale order #{Id:N0}"),
            "property" => Property,
            "reserveprice" => new NumberVariable(ReservePrice),
            "orderstatus" => new TextVariable(OrderStatus.DescribeEnum()),
            "start" => StartOfListing,
            "duration" => new TimeSpanVariable(DurationOfListing),
            "showforsale" => new BooleanVariable(ShowForSale),
            "consentcount" => new NumberVariable(PropertyOwnerConsent.Count),
            "ownerconsentcount" => new NumberVariable(PropertyOwnerConsent.Values.Count(x => x)),
            _ => throw new NotSupportedException($"Unsupported property sale order property {property}.")
        };
    }

    public static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.PropertySaleOrder,
            new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["id"] = ProgVariableTypes.Number,
                ["name"] = ProgVariableTypes.Text,
                ["property"] = ProgVariableTypes.Property,
                ["reserveprice"] = ProgVariableTypes.Number,
                ["orderstatus"] = ProgVariableTypes.Text,
                ["start"] = ProgVariableTypes.MudDateTime,
                ["duration"] = ProgVariableTypes.TimeSpan,
                ["showforsale"] = ProgVariableTypes.Boolean,
                ["consentcount"] = ProgVariableTypes.Number,
                ["ownerconsentcount"] = ProgVariableTypes.Number
            },
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["id"] = "The stable property-sale-order identity.",
                ["name"] = "A generated description of this sale order.",
                ["property"] = "The property governed by this sale order.",
                ["reserveprice"] = "The reserve price for this sale order.",
                ["orderstatus"] = "The current sale-order status.",
                ["start"] = "The in-world listing start time.",
                ["duration"] = "The configured sale-listing duration.",
                ["showforsale"] = "Whether the order is currently visible for sale.",
                ["consentcount"] = "The number of owners whose consent is tracked.",
                ["ownerconsentcount"] = "The number of owners who have given consent."
            });
    }

    #region Overrides of SaveableItem

    public override void Save()
    {
        Models.PropertySaleOrder dbitem = FMDB.Context.PropertySaleOrders.Find(Id);
        dbitem.DurationOfListingDays = _durationOfListing.TotalDays;
        dbitem.StartOfListing = _startOfListing?.GetDateTimeString() ?? "Never";
        dbitem.OrderStatus = (int)_orderStatus;
        dbitem.ReservePrice = _reservePrice;
        dbitem.PropertyOwnerConsentInfo =
            new XElement("Owners",
                    from owner in _propertyOwnerConsent
                    select new XElement("Owner", new XAttribute("id", owner.Key.OwnerId),
                        new XAttribute("type", owner.Key.OwnerFrameworkItemType),
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
                Models.PropertySaleOrder dbitem = FMDB.Context.PropertySaleOrders.Find(Id);
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
        bool consent = _propertyOwnerConsent.All(x => x.Value);
        _propertyOwnerConsent.Clear();
        _propertyOwnerConsent[newOwner] = consent;
        Changed = true;
    }

    public void ResetConsent()
    {
        Dictionary<IPropertyOwner, bool> old = _propertyOwnerConsent.ToDictionary();
        _propertyOwnerConsent.Clear();
        foreach (IPropertyOwner owner in Property.PropertyOwners)
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
