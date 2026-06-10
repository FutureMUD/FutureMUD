using System;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

[Flags]
public enum RidingGearRole
{
	None = 0,
	Saddle = 1 << 0,
	SaddlePad = 1 << 1,
	Bridle = 1 << 2,
	Reins = 1 << 3,
	Bit = 1 << 4,
	Stirrups = 1 << 5,
	PackSaddle = 1 << 6,
	Harness = 1 << 7,
	BitlessControl = 1 << 8
}

public interface IRidingGear : IGameItemComponent
{
	RidingGearRole Roles { get; }
	double ControlBonus { get; }
	double StabilityBonus { get; }
}
