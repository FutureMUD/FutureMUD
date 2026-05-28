#nullable enable

using MudSharp.Movement;

namespace MudSharp.Effects.Interfaces;

public interface ITrackIntensityEffect : IEffectSubtype
{
	double VisualTrackIntensityMultiplier { get; }
	double OlfactoryTrackIntensityMultiplier { get; }
	double VisualTrackIntensityBonus { get; }
	double OlfactoryTrackIntensityBonus { get; }
	TrackCircumstances AdditionalTrackCircumstances { get; }
}
