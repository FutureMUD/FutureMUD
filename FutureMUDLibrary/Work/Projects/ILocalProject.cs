using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using System.Collections.Generic;

namespace MudSharp.Work.Projects
{
    public interface ILocalProject : IActiveProject
    {
		RoomLayer RoomLayer { get; }
		double? RoutePositionMetres { get; }
		SpatialLocation SpatialLocation { get; }
		bool IsAtProjectSite(ICharacter character);
		void HandleAtProjectSite(string text);
        IEnumerable<(ICharacter Character, IProjectLabourRequirement Role)> Workers { get; }
    }
}
