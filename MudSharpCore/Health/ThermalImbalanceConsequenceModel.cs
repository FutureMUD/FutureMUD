#nullable enable

using System;
using MudSharp.Framework;

namespace MudSharp.Health;

public static class ThermalImbalanceConsequenceModel
{
	public const string EnabledStaticConfiguration = "TemperatureImbalanceEnabled";
	public const string MaximumHypothermiaMovementDelayMultiplier = "TemperatureImbalanceMaximumHypothermiaMovementDelayMultiplier";
	public const string MaximumHyperthermiaMovementDelayMultiplier = "TemperatureImbalanceMaximumHyperthermiaMovementDelayMultiplier";
	public const string MaximumHypothermiaStaminaMultiplier = "TemperatureImbalanceMaximumHypothermiaStaminaMultiplier";
	public const string MaximumHyperthermiaStaminaMultiplier = "TemperatureImbalanceMaximumHyperthermiaStaminaMultiplier";
	public const string MinimumHypothermiaStaminaRegenerationMultiplier = "TemperatureImbalanceMinimumHypothermiaStaminaRegenerationMultiplier";
	public const string MinimumHyperthermiaStaminaRegenerationMultiplier = "TemperatureImbalanceMinimumHyperthermiaStaminaRegenerationMultiplier";
	public const string SevereOrganPenaltyFraction = "TemperatureImbalanceSevereOrganPenaltyFraction";
	public const string CriticalThresholdsForMaximumOrganPenalty = "TemperatureImbalanceCriticalThresholdsForMaximumOrganPenalty";
	public const string MaximumHypothermiaBrainPenalty = "TemperatureImbalanceMaximumHypothermiaBrainPenalty";
	public const string MaximumHypothermiaHeartPenalty = "TemperatureImbalanceMaximumHypothermiaHeartPenalty";
	public const string MaximumHypothermiaKidneyPenalty = "TemperatureImbalanceMaximumHypothermiaKidneyPenalty";
	public const string MaximumHypothermiaLiverPenalty = "TemperatureImbalanceMaximumHypothermiaLiverPenalty";
	public const string MaximumHyperthermiaBrainPenalty = "TemperatureImbalanceMaximumHyperthermiaBrainPenalty";
	public const string MaximumHyperthermiaHeartPenalty = "TemperatureImbalanceMaximumHyperthermiaHeartPenalty";
	public const string MaximumHyperthermiaKidneyPenalty = "TemperatureImbalanceMaximumHyperthermiaKidneyPenalty";
	public const string MaximumHyperthermiaLiverPenalty = "TemperatureImbalanceMaximumHyperthermiaLiverPenalty";

	public static bool IsHypothermic(BodyTemperatureStatus status)
	{
		return status < BodyTemperatureStatus.NormalTemperature;
	}

	public static bool IsHyperthermic(BodyTemperatureStatus status)
	{
		return status > BodyTemperatureStatus.NormalTemperature;
	}

	public static double StageSeverity(BodyTemperatureStatus status)
	{
		return Math.Abs((int)status - (int)BodyTemperatureStatus.NormalTemperature) switch
		{
			<= 1 => 0.0,
			2 => 0.15,
			3 => 0.40,
			4 => 0.70,
			_ => 1.0
		};
	}

	public static double MovementDelayMultiplier(
		BodyTemperatureStatus status,
		double maximumHypothermiaMovementDelayMultiplier,
		double maximumHyperthermiaMovementDelayMultiplier)
	{
		return 1.0 + (MaximumMultiplierFor(status, maximumHypothermiaMovementDelayMultiplier,
			maximumHyperthermiaMovementDelayMultiplier) - 1.0) * StageSeverity(status);
	}

	public static double StaminaMultiplier(
		BodyTemperatureStatus status,
		double maximumHypothermiaStaminaMultiplier,
		double maximumHyperthermiaStaminaMultiplier)
	{
		return 1.0 + (MaximumMultiplierFor(status, maximumHypothermiaStaminaMultiplier,
			maximumHyperthermiaStaminaMultiplier) - 1.0) * StageSeverity(status);
	}

	public static double StaminaRegenerationMultiplier(
		BodyTemperatureStatus status,
		double minimumHypothermiaStaminaRegenerationMultiplier,
		double minimumHyperthermiaStaminaRegenerationMultiplier)
	{
		var floor = IsHypothermic(status)
			? minimumHypothermiaStaminaRegenerationMultiplier
			: minimumHyperthermiaStaminaRegenerationMultiplier;
		return 1.0 - (1.0 - floor) * StageSeverity(status);
	}

	public static double OrganPenaltySeverity(
		BodyTemperatureStatus status,
		double imbalanceProgress,
		double hotterThreshold,
		double colderThreshold,
		double severePenaltyFraction,
		double criticalThresholdsForMaximumOrganPenalty)
	{
		severePenaltyFraction = Math.Clamp(severePenaltyFraction, 0.0, 1.0);

		return status switch
		{
			BodyTemperatureStatus.SevereHypothermia or BodyTemperatureStatus.SevereHyperthermia => severePenaltyFraction,
			BodyTemperatureStatus.CriticalHypothermia or BodyTemperatureStatus.CriticalHyperthermia =>
				severePenaltyFraction +
				(1.0 - severePenaltyFraction) *
				CriticalProgressSeverity(status, imbalanceProgress, hotterThreshold, colderThreshold,
					criticalThresholdsForMaximumOrganPenalty),
			_ => 0.0
		};
	}

	public static double CriticalProgressSeverity(
		BodyTemperatureStatus status,
		double imbalanceProgress,
		double hotterThreshold,
		double colderThreshold,
		double criticalThresholdsForMaximumOrganPenalty)
	{
		if (criticalThresholdsForMaximumOrganPenalty <= 0.0 ||
		    !status.In(BodyTemperatureStatus.CriticalHypothermia, BodyTemperatureStatus.CriticalHyperthermia))
		{
			return 0.0;
		}

		var threshold = status == BodyTemperatureStatus.CriticalHyperthermia
			? hotterThreshold
			: Math.Abs(colderThreshold);

		if (threshold <= 0.0)
		{
			return 0.0;
		}

		return Math.Clamp(Math.Abs(imbalanceProgress) / (threshold * criticalThresholdsForMaximumOrganPenalty), 0.0,
			1.0);
	}

	public static double PenaltyAmount(
		BodyTemperatureStatus status,
		double penaltySeverity,
		double maximumHypothermiaPenalty,
		double maximumHyperthermiaPenalty)
	{
		if (penaltySeverity <= 0.0)
		{
			return 0.0;
		}

		return penaltySeverity *
		       (IsHypothermic(status) ? maximumHypothermiaPenalty : maximumHyperthermiaPenalty);
	}

	public static double EffectSeverityScore(BodyTemperatureStatus status, double imbalanceProgress)
	{
		return Math.Abs((int)status - (int)BodyTemperatureStatus.NormalTemperature) * 1_000_000.0 +
		       Math.Abs(imbalanceProgress);
	}

	private static double MaximumMultiplierFor(
		BodyTemperatureStatus status,
		double maximumHypothermiaMultiplier,
		double maximumHyperthermiaMultiplier)
	{
		return IsHypothermic(status) ? maximumHypothermiaMultiplier : maximumHyperthermiaMultiplier;
	}
}
