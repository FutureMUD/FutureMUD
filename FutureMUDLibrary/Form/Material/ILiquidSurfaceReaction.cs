using MudSharp.Framework;
using MudSharp.Health;
using System.Collections.Generic;

namespace MudSharp.Form.Material;

public interface ILiquidSurfaceReaction
{
    IEnumerable<ITag> TargetTags { get; }
    DamageType DamageType { get; }
    double DamagePerTick { get; }
    double PainPerTick { get; }
    double StunPerTick { get; }
}
