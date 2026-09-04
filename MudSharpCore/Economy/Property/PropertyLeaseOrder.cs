using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MoreLinq;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
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
        _rekeyOnLeaseEnd = dbitem.RekeyOnLeaseEnd;
        _feeIncreasePercentageAfterLeaseTerm = dbitem.FeeIncreasePercentageAfterLeaseTerm;
        _listedForLease = dbitem.ListedForLease;

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
        _rekeyOnLeaseEnd = false;
        _feeIncreasePercentageAfterLeaseTerm = 0.0M;

        using (new FMDB())
        {
            Models.PropertyLeaseOrder dbitem = new()
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
                RekeyOnLeaseEnd = RekeyOnLeaseEnd,
                ListedForLease = ListedForLease,
                FeeIncreasePercentageAfterLeaseTerm = FeeIncreasePercentageAfterLeaseTerm,
                PropertyOwnerConsentInfo = new XElement("Owners",
                        from owner in _propertyOwnerConsent
                        select new XElement("Owner", new XAttribute("id", owner.Key.OwnerId),
                            new XAttribute("type", owner.Key.OwnerFrameworkItemType),
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
    private bool _rekeyOnLeaseEnd;
    private bool _listedForLease;
    private decimal _feeIncreasePercentageAfterLeaseTerm;

    public override string FrameworkItemType => "PropertyLeaseOrder";

    public ProgVariableTypes Type => ProgVariableTypes.PropertyLeaseOrder;
    public object GetObject => this;

    public IProgVariable GetProperty(string property)
    {
        return property.ToLowerInvariant() switch
        {
            "id" => new NumberVariable(Id),
            "name" => new TextVariable($"{Property.Name} lease order #{Id:N0}"),
            "property" => Property,
            "priceperinterval" => new NumberVariable(PricePerInterval),
            "bondrequired" => new NumberVariable(BondRequired),
            "interval" => new TextVariable(Interval.Describe(Property.EconomicZone.FinancialPeriodReferenceCalendar)),
            "minimumduration" => new TimeSpanVariable(MinimumLeaseDuration),
            "maximumduration" => new TimeSpanVariable(MaximumLeaseDuration),
            "allowautorenew" => new BooleanVariable(AllowAutoRenew),
            "automaticallyrelist" => new BooleanVariable(AutomaticallyRelistAfterLeaseTerm),
            "allownovation" => new BooleanVariable(AllowLeaseNovation),
            "rekeyonend" => new BooleanVariable(RekeyOnLeaseEnd),
            "feeincreasepercentage" => new NumberVariable(FeeIncreasePercentageAfterLeaseTerm),
            "listed" => new BooleanVariable(ListedForLease),
            "consentcount" => new NumberVariable(PropertyOwnerConsent.Count),
            "ownerconsentcount" => new NumberVariable(PropertyOwnerConsent.Values.Count(x => x)),
            "charactereligibilityprog" => new TextVariable(CanLeaseProgCharacter?.Name ?? string.Empty),
            "claneligibilityprog" => new TextVariable(CanLeaseProgClan?.Name ?? string.Empty),
            _ => throw new NotSupportedException($"Unsupported property lease order property {property}.")
        };
    }

    public static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.PropertyLeaseOrder,
            new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["id"] = ProgVariableTypes.Number,
                ["name"] = ProgVariableTypes.Text,
                ["property"] = ProgVariableTypes.Property,
                ["priceperinterval"] = ProgVariableTypes.Number,
                ["bondrequired"] = ProgVariableTypes.Number,
                ["interval"] = ProgVariableTypes.Text,
                ["minimumduration"] = ProgVariableTypes.TimeSpan,
                ["maximumduration"] = ProgVariableTypes.TimeSpan,
                ["allowautorenew"] = ProgVariableTypes.Boolean,
                ["automaticallyrelist"] = ProgVariableTypes.Boolean,
                ["allownovation"] = ProgVariableTypes.Boolean,
                ["rekeyonend"] = ProgVariableTypes.Boolean,
                ["feeincreasepercentage"] = ProgVariableTypes.Number,
                ["listed"] = ProgVariableTypes.Boolean,
                ["consentcount"] = ProgVariableTypes.Number,
                ["ownerconsentcount"] = ProgVariableTypes.Number,
                ["charactereligibilityprog"] = ProgVariableTypes.Text,
                ["claneligibilityprog"] = ProgVariableTypes.Text
            },
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["id"] = "The stable property-lease-order identity.",
                ["name"] = "A generated description of this lease order.",
                ["property"] = "The property governed by this lease order.",
                ["priceperinterval"] = "The proposed lease charge per interval.",
                ["bondrequired"] = "The required lease bond.",
                ["interval"] = "The human-readable recurring payment interval.",
                ["minimumduration"] = "The minimum permitted lease duration.",
                ["maximumduration"] = "The maximum permitted lease duration.",
                ["allowautorenew"] = "Whether a lease may auto-renew.",
                ["automaticallyrelist"] = "Whether the order relists after a lease ends.",
                ["allownovation"] = "Whether the lease may be novated.",
                ["rekeyonend"] = "Whether locks are rekeyed when the lease ends.",
                ["feeincreasepercentage"] = "The price and bond increase percentage after a lease term.",
                ["listed"] = "Whether the order is currently listed for lease.",
                ["consentcount"] = "The number of owners whose consent is tracked.",
                ["ownerconsentcount"] = "The number of owners who have given consent.",
                ["charactereligibilityprog"] = "The character eligibility prog name, or empty text.",
                ["claneligibilityprog"] = "The clan eligibility prog name, or empty text."
            });
    }

    #region Overrides of SaveableItem

    public override void Save()
    {
        Models.PropertyLeaseOrder dbitem = FMDB.Context.PropertyLeaseOrders.Find(Id);
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
        dbitem.RekeyOnLeaseEnd = _rekeyOnLeaseEnd;
        dbitem.AutomaticallyRelistAfterLeaseTerm = _automaticallyRelistAfterLeaseTerm;
        dbitem.ListedForLease = _listedForLease;
        dbitem.PropertyOwnerConsentInfo = new XElement("Owners",
                from owner in _propertyOwnerConsent
                select new XElement("Owner", new XAttribute("id", owner.Key.OwnerId),
                    new XAttribute("type", owner.Key.OwnerFrameworkItemType), new XAttribute("consent", owner.Value))
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
                Models.PropertyLeaseOrder dbitem = FMDB.Context.PropertyLeaseOrders.Find(Id);
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
            if (oldLease.Leaseholder is ICharacter leaseholder)
            {
                Property.ClaimShops(leaseholder);
                Property.ClaimStables(leaseholder);
                Property.ClaimHospitals(leaseholder);
            }
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

        if (_rekeyOnLeaseEnd)
        {
            Property.RekeyAllLocks();
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
        bool consent = _propertyOwnerConsent.All(x => x.Value);
        _propertyOwnerConsent.Clear();
        _propertyOwnerConsent[newOwner] = consent;
        ListedForLease = _propertyOwnerConsent.All(x => x.Value) && Property.LeaseOrder == this;
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

    public bool RekeyOnLeaseEnd
    {
        get => _rekeyOnLeaseEnd;
        set
        {
            _rekeyOnLeaseEnd = value;
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
