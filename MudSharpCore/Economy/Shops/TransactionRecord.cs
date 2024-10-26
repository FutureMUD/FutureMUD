using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;
using MudSharp.Economy.Currency;

namespace MudSharp.Economy.Shops;

public class TransactionRecord : LateInitialisingItem, ITransactionRecord
{
    public override string FrameworkItemType => "TransactionRecord";

    public override void Save()
    {
        var dbitem = FMDB.Context.ShopTransactionRecords.Find(Id);
        dbitem.MudDateTime = MudDateTime.GetDateTimeString();
        dbitem.RealDateTime = RealDateTime;
        dbitem.Tax = Tax;
        dbitem.PretaxValue = PretaxValue;
        dbitem.ThirdPartyId = ThirdPartyId;
        dbitem.ShopId = Shop.Id;
        dbitem.TransactionType = (int)TransactionType;
        dbitem.CurrencyId = Currency.Id;
        Changed = false;
    }

    public override object DatabaseInsert()
    {
        var dbitem = new Models.ShopTransactionRecord();
        FMDB.Context.ShopTransactionRecords.Add(dbitem);

        dbitem.CurrencyId = Currency.Id;
        dbitem.MudDateTime = MudDateTime.GetDateTimeString();
        dbitem.RealDateTime = RealDateTime;
        dbitem.PretaxValue = PretaxValue;
        dbitem.Tax = Tax;
        dbitem.ThirdPartyId = ThirdPartyId;
        dbitem.ShopId = Shop.Id;
        dbitem.TransactionType = (int)TransactionType;

        return dbitem;
    }

    public override void SetIDFromDatabase(object dbitem)
    {
        _id = ((Models.ShopTransactionRecord)dbitem).Id;
    }

    public TransactionRecord(ShopTransactionType type, ICurrency currency, IShop shop, MudDateTime mudtime,
        ICharacter thirdparty, decimal pretaxvalue, decimal tax)
    {
        Gameworld = shop.Gameworld;
        Gameworld.SaveManager.AddInitialisation(this);
        TransactionType = type;
        Currency = currency;
        Shop = shop;
        EconomicZone = shop.EconomicZone;
        MudDateTime = mudtime;
        RealDateTime = DateTime.UtcNow;
        ThirdPartyId = thirdparty?.Id;
        PretaxValue = pretaxvalue;
        Tax = tax;
    }

    public TransactionRecord(Models.ShopTransactionRecord record, IShop shop, IFuturemud gameworld)
    {
        _id = record.Id;
        TransactionType = (ShopTransactionType)record.TransactionType;
        Shop = shop;
        EconomicZone = shop.EconomicZone;
        Currency = gameworld.Currencies.Get(record.CurrencyId);
        MudDateTime = new MudDateTime(record.MudDateTime, gameworld);
        RealDateTime = record.RealDateTime;
        ThirdPartyId = record.ThirdPartyId;
        PretaxValue = record.PretaxValue;
        Tax = record.Tax;
    }

    public ShopTransactionType TransactionType { get; private set; }
    public ICurrency Currency { get; private set; }
    public decimal PretaxValue { get; private set; }
    public decimal Tax { get; private set; }
    public IShop Shop { get; private set; }
    public IEconomicZone EconomicZone { get; private set; }
    public long? ThirdPartyId { get; private set; }
    public DateTime RealDateTime { get; private set; }
    public MudDateTime MudDateTime { get; private set; }

    public decimal NetValue
    {
        get
        {
            switch (TransactionType)
            {
                case ShopTransactionType.Sale:
                    return PretaxValue;
                case ShopTransactionType.Restock:
                    return -1 * PretaxValue;
                case ShopTransactionType.Stock:
                    return PretaxValue;
                case ShopTransactionType.Purchase:
                    return PretaxValue;
                case ShopTransactionType.StockLoss:
                    return -1 * PretaxValue;
                case ShopTransactionType.PriceAdjustment:
                    return PretaxValue;
                case ShopTransactionType.Float:
                    return PretaxValue;
                case ShopTransactionType.Withdrawal:
                    return -1 * PretaxValue;
                default:
                    return 0.0M;
            }
        }
    }
}