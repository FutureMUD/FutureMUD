#nullable enable

namespace MudSharp.RPG.Merits.Interfaces;

/// <summary>Multiplies effective liver clearance after organ function, for alcohol and all active drugs.</summary>
public interface ILiverClearanceMerit : ICharacterMerit
{
	double ClearanceMultiplier { get; }
}
