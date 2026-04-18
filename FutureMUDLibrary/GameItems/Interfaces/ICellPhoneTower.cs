#nullable enable
using MudSharp.Construction;
using MudSharp.Construction.Grids;

namespace MudSharp.GameItems.Interfaces;

public interface ICellPhoneTower : IGameItemComponent, IConsumePower, IOnOff
{
    ITelecommunicationsGrid? TelecommunicationsGrid { get; set; }
    bool IsPowered { get; }
    bool ProvidesCoverage(IZone zone);
}
