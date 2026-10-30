using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace MudSharp.Health;

/// <summary>
/// Configurable recurring drug symptoms. Formula inputs are intensity, resistance, fullness, stamina, sleeping, unconscious, stage and damage.
/// Chances are probabilities per configured trial interval; damage and resource losses are per successful episode.
/// </summary>
public sealed class DrugSymptomsAdditionalInfo : DrugAdditionalInfo
{
	public long ResistanceTraitId { get; set; }
	public int InitialStage { get; set; }
	public double TickIntervalSeconds { get; set; } = 1.0;
	public bool OnsetPerDose { get; set; }
	public List<DrugPermanentTraitChange> OnsetTraitChanges { get; set; } = [];
	public List<DrugPermanentTraitChange> ExpiryTraitChanges { get; set; } = [];
	public List<DrugSymptomStage> Stages { get; set; } = [new()];
	public List<long> ImmuneRaceIds { get; set; } = [];
	[JsonIgnore]
	public override string DatabaseString => JsonSerializer.Serialize(this);

	public static DrugSymptomsAdditionalInfo Parse(string? text)
	{
		try
		{
			var result = JsonSerializer.Deserialize<DrugSymptomsAdditionalInfo>(text ?? "{}");
			if (result is null || result.Stages is null || result.Stages.Count == 0 ||
				result.Stages.Exists(x => x is null)) return new();
			result.InitialStage = Math.Clamp(result.InitialStage, 0, result.Stages.Count - 1);
			if (!double.IsFinite(result.TickIntervalSeconds) || result.TickIntervalSeconds < 1.0) result.TickIntervalSeconds = 1.0;
			result.OnsetTraitChanges ??= [];
			result.ExpiryTraitChanges ??= [];
			result.ImmuneRaceIds ??= [];
			result.OnsetTraitChanges.RemoveAll(x => x is null || !double.IsFinite(x.Amount) || !double.IsFinite(x.MinimumValue));
			result.ExpiryTraitChanges.RemoveAll(x => x is null || !double.IsFinite(x.Amount) || !double.IsFinite(x.MinimumValue));
			foreach (var stage in result.Stages)
			{
				stage.NextStages ??= [];
				stage.NextStageWeights ??= [];
				stage.Echoes ??= [];
				stage.Outcomes ??= [];
				stage.Outcomes.RemoveAll(x => x is null || !double.IsFinite(x.Weight) || x.Weight <= 0 || !Enum.IsDefined(x.Action));
				stage.Echoes.RemoveAll(string.IsNullOrWhiteSpace);
				if (!Enum.IsDefined(stage.DamageType)) stage.DamageType = DamageType.Cellular;
			}
			return result;
		}
		catch (JsonException)
		{
			return new();
		}
	}
}

/// <summary>A probabilistic permanent change to the owned raw trait, without incorporating temporary bonuses.</summary>
public sealed class DrugPermanentTraitChange
{
	public long TraitId { get; set; }
	public string Chance { get; set; } = "0";
	public double Amount { get; set; }
	public double MinimumValue { get; set; } = 0.0;
}

/// <summary>A zero-based stage of a drug's symptom profile; destination weights default to one.</summary>
public sealed class DrugSymptomStage
{
	public string Chance { get; set; } = "0";
	public string Damage { get; set; } = "0";
	public string Pain { get; set; } = "0";
	public string Stun { get; set; } = "0";
	public string Stamina { get; set; } = "0";
	public string MaximumHitPointsBonus { get; set; } = "0";
	public double MaximumHitPointsDurationSeconds { get; set; }
	public bool MaximumHitPointsCapacityOnly { get; set; }
	public bool SleepAtZeroStamina { get; set; }
	public bool PreventNaturalStunRecovery { get; set; }
	public bool RecoveryBlockOnlyWhenUnconscious { get; set; } = true;
	public bool CeaseCombatOnParalysis { get; set; }
	public DamageType DamageType { get; set; } = DamageType.Cellular;
	public bool AwakeOnly { get; set; }
	public bool KillSleeping { get; set; }
	public string TransitionChance { get; set; } = "0";
	public List<int> NextStages { get; set; } = [];
	public Dictionary<int, string> NextStageWeights { get; set; } = [];
	public List<string> Echoes { get; set; } = [];
	public List<DrugSymptomOutcome> Outcomes { get; set; } = [];
}

/// <summary>One mutually exclusive weighted episode. None retains the stage's sensory echoes.</summary>
public enum DrugSymptomAction { None, Sit, DropReadiedItems, ReleaseGrapple, Babble, Residue }

public sealed class DrugSymptomOutcome
{
	public double Weight { get; set; } = 1;
	public DrugSymptomAction Action { get; set; }
	public string PrivateEcho { get; set; } = "";
	public string RoomEmote { get; set; } = "";
	public double DurationSeconds { get; set; } = 600;
	public long ResidueMaterialId { get; set; }
	public double ResidueKilograms { get; set; } = 0.01;
	public string ClothingBodypart { get; set; } = "";
	public bool ReleaseGrapple { get; set; }
}
