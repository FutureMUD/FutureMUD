using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Economy
{
    public interface IMerchandise : ISaveable, IKeywordedItem, IProgVariable
    {
        IShop Shop { get; }
        bool AutoReordering { get; }
        decimal EffectiveAutoReorderPrice { get; }
        bool PreserveVariablesOnReorder { get; }
        int MinimumStockLevels { get; }
        double MinimumStockLevelsByWeight { get; }
        decimal BasePrice { get; }
        decimal EffectivePrice { get; }
        decimal EffectivePriceForWeight(double weight);
        MerchandiseType MerchandiseType { get; }
        IGameItem PreferredDisplayContainer { get; }
        string ListDescription { get; }
        IGameItemProto Item { get; }
        IGameItemSkin Skin { get; }
        ISolid CommodityMaterial { get; }
        ITag CommodityTag { get; }
        bool CommodityRequiresNoCharacteristics { get; }
        IReadOnlyDictionary<ICharacteristicDefinition, ICharacteristicValue> CommodityCharacteristicRequirements { get; }
        double CommodityPricingWeight { get; }
        string CommodityDescriptor { get; }
        bool WillSell { get; }
        bool WillBuy { get; }
        decimal BaseBuyModifier { get; }
        double MinimumConditionToBuy { get; }
        int MaximumStockLevelsToBuy { get; }
        bool IgnoreMarketPricing { get; }
        bool DefaultMerchandiseForItem { get; }
        bool PermitItemDecayOnStockedItems { get; }
        decimal SalesMarkupMultiplier { get; set; }

        void Delete();

        bool IsMerchandiseFor(IGameItem item, bool ignoreDefault = false);
        bool IsMerchandiseForCommodity(ICommodity commodity);
        bool BuildingCommand(ICharacter actor, StringStack command);
        void ShowToBuilder(ICharacter actor);
        void ShopCurrencyChanged(ICurrency oldCurrency, ICurrency newCurrency);
        bool CanReprice(decimal multiplier);
        void Reprice(decimal multiplier);
        event EventHandler OnDelete;
    }
}
