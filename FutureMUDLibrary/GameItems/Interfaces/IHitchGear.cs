using System;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

[Flags]
public enum HitchGearRole
{
	None = 0,
	TowBar = 1 << 0,
	Yoke = 1 << 1,
	Harness = 1 << 2,
	LeadRope = 1 << 3,
	Chain = 1 << 4,
	Rope = 1 << 5,
	Traces = 1 << 6
}

public interface IHitchGear : IDragAid
{
	HitchGearRole Roles { get; }
	double MaximumTowedWeight { get; }
}
