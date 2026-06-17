namespace MudSharp.Effects.Interfaces;

public interface IRespirationModifierEffect : IEffectSubtype
{
	double BreathingDriveMultiplier { get; }
	double HypoxiaDamageMultiplier { get; }
	double AirwayToleranceMultiplier { get; }
}
