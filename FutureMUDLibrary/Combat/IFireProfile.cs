using System;
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.Health;

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
