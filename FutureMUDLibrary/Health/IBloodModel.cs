using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Health
{
    public interface IBloodModel : IFrameworkItem
    {
        IEnumerable<IBloodtype> Bloodtypes { get; }
        IEnumerable<IBloodtypeAntigen> Antigens { get; }
    }
}
