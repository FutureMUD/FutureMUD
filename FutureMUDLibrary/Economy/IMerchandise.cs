using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Economy.Currency;

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
		decimal EffectivePrice { get; }
		IGameItem PreferredDisplayContainer { get; }
		string ListDescription { get; }
		IGameItemProto Item { get; }
		IGameItemSkin Skin { get; }
		bool WillSell { get; }
		bool WillBuy { get; }
		decimal BaseBuyModifier { get; }
		double MinimumConditionToBuy { get; }
		int MaximumStockLevelsToBuy { get; }
		bool IgnoreMarketPricing { get; }
		bool DefaultMerchandiseForItem { get; }
		bool PermitItemDecayOnStockedItems { get; }

		void Delete();

		bool IsMerchandiseFor(IGameItem item, bool ignoreDefault = false);
		bool BuildingCommand(ICharacter actor, StringStack command);
		void ShowToBuilder(ICharacter actor);
		void ShopCurrencyChanged(ICurrency oldCurrency, ICurrency newCurrency);
		event EventHandler OnDelete;
	}
}
