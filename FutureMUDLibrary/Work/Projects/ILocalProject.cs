using MudSharp.Character;
using System.Collections.Generic;

namespace MudSharp.Work.Projects
{
    public interface ILocalProject : IActiveProject
    {
        IEnumerable<(ICharacter Character, IProjectLabourRequirement Role)> Workers { get; }
    }
}
