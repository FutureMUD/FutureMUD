using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.FutureProg;

namespace MudSharp.Events.Hooks
{
    public interface IHookWithProgs : IHook
    {
        IEnumerable<IFutureProg> FutureProgs { get; }
    }
}
