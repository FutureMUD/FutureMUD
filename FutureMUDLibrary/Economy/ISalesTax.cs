using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Economy
{
    public interface ISalesTax : IEditableItem, ISaveable
    {
        string MerchantDescription { get; }
        bool Applies(IMerchandise merchandise, ICharacter purchaser);
        decimal TaxValue(IMerchandise merchandise, ICharacter purchaser);
        decimal TaxValue(IMerchandise merchandise, ICharacter purchaser, decimal saleValue);
        void Delete();
    }
}
