using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Economy;

namespace MudSharp.GameItems.Interfaces
{
	public interface IMarketGoodWeightItem : IGameItemComponent
	{
		decimal MultiplierForCategory(IMarketCategory category);
	}
}
