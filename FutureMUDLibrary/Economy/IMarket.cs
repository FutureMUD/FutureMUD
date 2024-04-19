using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;

namespace MudSharp.Economy;
#nullable enable

public interface IMarket : ISaveable, IEditableItem
{
	IEconomicZone EconomicZone { get; }
	string Description { get; }
	IEnumerable<IMarketInfluence> MarketInfluences { get; }
	IEnumerable<IMarketCategory> MarketCategories { get; }
	decimal PriceMultiplierForCategory(IMarketCategory? category);
	decimal PriceMultiplierForItem(IGameItem item);
	decimal PriceMultiplierForItem(IGameItemProto item);
	void ApplyMarketInfluence(IMarketInfluence influence);
	void RemoveMarketInfluence(IMarketInfluence influence);
	IMarket Clone(string newName);
}