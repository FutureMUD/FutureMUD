#nullable enable
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MudSharp.Health;

public enum InfectionRecoveryMode { Immune, FixedDuration }

/// <summary>A reusable infection definition stored with the drug effect that establishes it.</summary>
public sealed class DrugInfectionAdditionalInfo : DrugAdditionalInfo
{
	public string Name { get; set; } = "an illness";
	public long SymptomDrugId { get; set; }
	public InfectionRecoveryMode RecoveryMode { get; set; }
	public double DurationSeconds { get; set; } = 30000;
	public double InitialIntensity { get; set; } = 1.0;
	public double SymptomIntensityMultiplier { get; set; } = 1.0;
	public int VirulenceDifficulty { get; set; } = 5;
	public double VirulenceMultiplier { get; set; } = 1.0;
	public double ExposureChance { get; set; } = 1.0;
	public double SpreadIntervalSeconds { get; set; } = 3.0;
	public double SpreadChance { get; set; }
	public double SpreadEpisodeChance { get; set; } = 1.0;
	public bool SpreadOnSymptomEpisode { get; set; }
	public bool CureWithDrug { get; set; } = true;
	[JsonIgnore] public override string DatabaseString => JsonSerializer.Serialize(this);
	[JsonIgnore] public bool IsValid => !string.IsNullOrWhiteSpace(Name) && SymptomDrugId > 0 &&
		Enum.IsDefined(RecoveryMode) && double.IsFinite(DurationSeconds) && DurationSeconds > 0 && DurationSeconds < TimeSpan.MaxValue.TotalSeconds &&
		double.IsFinite(InitialIntensity) && InitialIntensity > 0 &&
		double.IsFinite(SymptomIntensityMultiplier) && SymptomIntensityMultiplier >= 0 &&
		double.IsFinite(VirulenceMultiplier) && VirulenceMultiplier > 0 &&
		Probability(ExposureChance) && Probability(SpreadChance) && Probability(SpreadEpisodeChance) &&
		double.IsFinite(SpreadIntervalSeconds) && SpreadIntervalSeconds >= 1 && SpreadIntervalSeconds < TimeSpan.MaxValue.TotalSeconds;
	private static bool Probability(double value) => double.IsFinite(value) && value is >= 0 and <= 1;
	public static DrugInfectionAdditionalInfo Parse(string? text)
	{
		try { return JsonSerializer.Deserialize<DrugInfectionAdditionalInfo>(text ?? "{}") ?? new(); }
		catch (JsonException) { return new(); }
	}
}

/// <summary>An infection-owned contribution to the ordinary drug-effect reconciler, without physical dosing.</summary>
public interface IDrugSymptomInfection : IInfection
{
	long DefinitionDrugId { get; }
	string? SourceFamily { get; }
	IDrug? SymptomDrug { get; }
	double SymptomIntensity { get; }
	bool CureWithDrug { get; }
	void OnSymptomEpisode();
}
