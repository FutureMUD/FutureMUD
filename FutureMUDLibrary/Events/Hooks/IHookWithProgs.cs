using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Events.Hooks
{
    public interface IHookWithProgs : IHook
    {
        IEnumerable<IFutureProg> FutureProgs { get; }
    }
}
