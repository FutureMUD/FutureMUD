#nullable enable

namespace MudSharp.GameItems.Interfaces;

/// <summary>An equipped item that assists native climbing checks.</summary>
public interface IClimbingAid : IGameItemComponent
{
	double CheckBonus { get; }
	bool WorksHeld { get; }
	bool WorksWorn { get; }
}
