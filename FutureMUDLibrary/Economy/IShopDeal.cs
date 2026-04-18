#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate;
using System;

namespace MudSharp.Economy;

[Flags]
public enum ShopDealApplicability
{
    Sell = 1,
    Buy = 2,
    Both = Sell | Buy
}

public enum ShopDealType
{
    Sale = 0,
    Volume = 1
}

public enum ShopDealTargetType
{
    AllMerchandise = 0,
    Merchandise = 1,
    ItemTag = 2
}

public interface IShopDeal : IFrameworkItem, ISaveable
{
    IShop Shop { get; }
    ShopDealType DealType { get; }
    ShopDealTargetType TargetType { get; }
    IMerchandise? TargetMerchandise { get; }
    ITag? TargetTag { get; }
    decimal PriceAdjustmentPercentage { get; }
    int MinimumQuantity { get; }
    ShopDealApplicability Applicability { get; }
    IFutureProg? EligibilityProg { get; }
    MudDateTime Expiry { get; }
    bool IsCumulative { get; }
    bool IsExpired { get; }

    bool AppliesToMerchandise(IMerchandise merchandise);
    bool Applies(IMerchandise merchandise, ICharacter? shopper, int quantity, ShopDealApplicability applicability,
        MudDateTime now);
    bool BuildingCommand(ICharacter actor, StringStack command);
    void ShowToBuilder(ICharacter actor);
    void Delete();
}
