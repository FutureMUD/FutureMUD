using MudSharp.Form.Material;

namespace MudSharp.Effects.Interfaces;

public interface IAffectAtmosphere : IEffect
{
    IFluid Atmosphere { get; }
}
