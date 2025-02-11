using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.Economy;
public interface IShopper : IEditableItem
{
	IEconomicZone EconomicZone { get; }
	RecurringInterval ShoppingInterval { get; }
	MudDateTime NextShop { get; }
	void DoShop();
	IShopper Clone(string name);
	string ShopperType { get; }
}
