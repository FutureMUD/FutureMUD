using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework.Revision;

namespace MudSharp.Economy
{
    public enum IncomeType
    {
        SalariesPaid,
        CapitalGains,
        Dividends
    }

    public interface IIncomeTax : IEditableItem
    {
        bool Applies(ICharacter actor, IncomeType type);
        decimal Rate { get; }
    }
}
