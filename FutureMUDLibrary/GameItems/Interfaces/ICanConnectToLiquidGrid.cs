#nullable enable
using MudSharp.Construction.Grids;

namespace MudSharp.GameItems.Interfaces;

public interface ICanConnectToLiquidGrid : ICanConnectToGrid
{
    ILiquidGrid? LiquidGrid { get; set; }
}
