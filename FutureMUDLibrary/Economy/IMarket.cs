using JetBrains.Annotations;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Economy;
#nullable enable

public interface IMarket : ISaveable, IEditableItem, IProgVariable
{
    IEconomicZone EconomicZone { get; }
    string Description { get; }
    IEnumerable<IMarketInfluence> MarketInfluences { get; }
    IEnumerable<IMarketCategory> MarketCategories { get; }
    decimal EffectiveIncomeFactorForPopulation(IMarketPopulation population);
    decimal FlatPriceAdjustmentForCategory(IMarketCategory? category);
    decimal PriceMultiplierForCategory(IMarketCategory? category);
    decimal FlatPriceAdjustmentForItem(IGameItem item);
    decimal PriceMultiplierForItem(IGameItem item);
    decimal FlatPriceAdjustmentForItem(IGameItemProto item);
    decimal PriceMultiplierForItem(IGameItemProto item);
    void ApplyMarketInfluence(IMarketInfluence influence);
    void RemoveMarketInfluence(IMarketInfluence influence);
    IMarket Clone(string newName);
}
