#nullable enable
using MudSharp.Construction.Grids;

namespace MudSharp.GameItems.Interfaces;

public interface ICanConnectToTelecommunicationsGrid : ICanConnectToGrid
{
	ITelecommunicationsGrid? TelecommunicationsGrid { get; set; }
}
