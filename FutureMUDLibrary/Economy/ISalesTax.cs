using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.Economy
{
    public interface ISalesTax : IEditableItem, ISaveable
    {
        string MerchantDescription { get; }
        bool Applies(IMerchandise merchandise, ICharacter purchaser);
        decimal TaxValue(IMerchandise merchandise, ICharacter purchaser);
        void Delete();
    }
}
