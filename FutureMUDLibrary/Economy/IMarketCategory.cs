using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System.Collections.Generic;

namespace MudSharp.Economy;
#nullable enable
public interface IMarketCategory : ISaveable, IEditableItem, IProgVariable
{
    string Description { get; }
    MarketCategoryType CategoryType { get; }
    IEnumerable<MarketCategoryComponent> CombinationComponents { get; }
    bool BelongsToCategory(IGameItem item);
    bool BelongsToCategory(IGameItemProto proto);
    double ElasticityFactorAbove { get; }
    double ElasticityFactorBelow { get; }
    IMarketCategory Clone(string newName);
}
