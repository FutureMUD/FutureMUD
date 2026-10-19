#nullable enable

using System;
using MudSharp.Body;
using MudSharp.GameItems;

namespace MudSharp.Construction;

public enum TerrainMedium
{
	Liquid,
	Silt
}

public enum ImmersionExtent
{
	None,
	Partial,
	Full
}

/// <summary>
/// A categorical ground-level medium. ImmersionSize is the largest bottom-resting
/// subject fully covered; it is not a physical length or a new spatial layer.
/// </summary>
public sealed record TerrainImmersionProfile(TerrainMedium Medium, SizeCategory ImmersionSize,
	bool ObscuresItems = false, bool RestrictRunning = false)
{
	public ImmersionContact ContactFor(SizeCategory subjectSize)
	{
		var gap = (int)subjectSize - (int)ImmersionSize;
		return gap <= 0
			? new(ImmersionExtent.Full, Orientation.Highest, gap)
			: new(ImmersionExtent.Partial, gap switch
			{
				1 => Orientation.High,
				2 => Orientation.Centre,
				3 => Orientation.Low,
				_ => Orientation.Lowest
			}, gap);
	}
}

public readonly record struct ImmersionContact(ImmersionExtent Extent, Orientation HighestImmersed,
	int SizeGap)
{
	public static ImmersionContact None => new(ImmersionExtent.None, Orientation.Irrelevant, int.MaxValue);
	public bool IsWading => Extent != ImmersionExtent.None && SizeGap is 1 or 2;
	public bool Covers(Orientation orientation) => Extent == ImmersionExtent.Full ||
		Extent == ImmersionExtent.Partial && orientation is >= Orientation.Lowest and <= Orientation.Highest &&
		orientation <= HighestImmersed;
}
