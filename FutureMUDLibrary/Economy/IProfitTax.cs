using MudSharp.Framework;
using MudSharp.Framework.Revision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Economy
{
    public interface IProfitTax : IEditableItem
    {
        string MerchantDescription { get; }
        bool Applies(IShop shop, decimal grossProfit, decimal netProfit);
        decimal TaxValue(IShop shop, decimal grossProfit, decimal netProfit);
        void Delete();
    }
}
