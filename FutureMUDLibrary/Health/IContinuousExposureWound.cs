#nullable enable

namespace MudSharp.Health;

/// <summary>A persisted wound that can receive further increments from the same environmental exposure.</summary>
public interface IContinuousExposureWound : IWound
{
	string? ExposureKey { get; set; }
	/// <summary>Applies one already-resisted increment, including this wound type's bodypart modifiers.</summary>
	void SufferAdditionalExposureDamage(IDamage damage);
}
