using MudSharp.Framework;
using MudSharp.Health;
using System;
using System.Collections.Generic;

namespace MudSharp.Combat;

public interface IFireProfile
{
    string Name { get; }
    DamageType DamageType { get; }
    double DamagePerTick { get; }
    double PainPerTick { get; }
    double StunPerTick { get; }
    double ThermalLoadPerTick { get; }
    double SpreadChance { get; }
    double MinimumOxidation { get; }
    bool SelfOxidising { get; }
    TimeSpan TickFrequency { get; }
    IEnumerable<ITag> ExtinguishTags { get; }
}
