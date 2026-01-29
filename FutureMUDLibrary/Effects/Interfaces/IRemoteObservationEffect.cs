using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Construction;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Interfaces
{
    public interface IRemoteObservationEffect : IEffectSubtype
    {
        void HandleOutput(IOutput output, ILocation location);
        void HandleOutput(string text, ILocation location);
    }
}
