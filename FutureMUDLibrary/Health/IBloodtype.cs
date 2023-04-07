using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Health
{
    public interface IBloodtype : IFrameworkItem
    {
        IEnumerable<IBloodtypeAntigen> Antigens { get; }

        bool IsCompatibleWithDonorBlood(IBloodtype donorBloodtype);
    }
}
