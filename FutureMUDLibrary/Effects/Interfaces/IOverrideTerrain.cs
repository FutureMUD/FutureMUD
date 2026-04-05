using MudSharp.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Interfaces
{
    public interface IOverrideTerrain : IEffect
    {
        ITerrain Terrain { get; }
    }
}
