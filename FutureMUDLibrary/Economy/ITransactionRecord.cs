using System;
using JetBrains.Annotations;
using MudSharp.Economy.Currency;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy
{
    
    public enum ShopTransactionType
    {
        Sale,
        Restock,
        Stock,
        StockLoss,
        PriceAdjustment,
        Deposit,
        Withdrawal,
        AccessCashDraw,
        Purchase
    }

    public interface ITransactionRecord : ILateInitialisingItem
    {
        ShopTransactionType TransactionType { get; }
        ICurrency Currency { get; }
        decimal PretaxValue { get; }
        decimal Tax { get; }
        IShop Shop { get; }
        IEconomicZone EconomicZone { get; }
        long? ThirdPartyId { get; }
        DateTime RealDateTime { get; }
        MudDateTime MudDateTime { get; }
        decimal NetValue { get; }
        [CanBeNull] IMerchandise Merchandise { get; set; }
    }
}