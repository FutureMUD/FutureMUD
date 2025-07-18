namespace MudSharp.Combat;

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Construction.Boundary;
using System.Collections.Generic;

public interface IRangedScatterStrategy
{
    IPerceiver? GetScatterTarget(ICharacter shooter, IPerceiver originalTarget, IEnumerable<ICellExit> path);
}
