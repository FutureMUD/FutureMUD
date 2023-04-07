using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.Work.Projects
{
    public interface ILocalProject : IActiveProject
    {
        IEnumerable<(ICharacter Character, IProjectLabourRequirement Role)> Workers { get; }
    }
}
