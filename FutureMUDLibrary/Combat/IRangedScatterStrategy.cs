namespace MudSharp.Combat;

using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using System.Collections.Generic;

#nullable enable

public interface IRangedScatterStrategy
{
    RangedScatterResult? GetScatterTarget(ICharacter shooter, IPerceiver originalTarget, IEnumerable<ICellExit> path);
}
