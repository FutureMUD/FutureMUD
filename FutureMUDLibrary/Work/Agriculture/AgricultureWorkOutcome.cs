#nullable enable

using System;
using MudSharp.GameItems;

namespace MudSharp.Work.Agriculture;

public sealed class AgricultureWorkOutcome
{
	public static AgricultureWorkOutcome Neutral { get; } = FromSkill(35.0, 0.0, 0.0, 0.0);

	private AgricultureWorkOutcome(
		double weightedFarmingSkill,
		double labourHours,
		double weightedSupervisorSkill,
		double supervisorHours,
		double cropYieldMultiplier,
		double seedYieldMultiplier,
		double beneficialScoreMultiplier,
		double harmfulScoreMultiplier,
		int cropHealthDelta,
		int cropYieldDelta,
		ItemQuality outputQuality)
	{
		WeightedFarmingSkill = weightedFarmingSkill;
		LabourHours = labourHours;
		WeightedSupervisorSkill = weightedSupervisorSkill;
		SupervisorHours = supervisorHours;
		CropYieldMultiplier = cropYieldMultiplier;
		SeedYieldMultiplier = seedYieldMultiplier;
		BeneficialScoreMultiplier = beneficialScoreMultiplier;
		HarmfulScoreMultiplier = harmfulScoreMultiplier;
		CropHealthDelta = cropHealthDelta;
		CropYieldDelta = cropYieldDelta;
		OutputQuality = outputQuality;
	}

	public double WeightedFarmingSkill { get; }
	public double LabourHours { get; }
	public double WeightedSupervisorSkill { get; }
	public double SupervisorHours { get; }
	public double AverageFarmingSkill => LabourHours > 0.0 ? WeightedFarmingSkill / LabourHours : 35.0;
	public double AverageSupervisorSkill => SupervisorHours > 0.0 ? WeightedSupervisorSkill / SupervisorHours : 0.0;
	public double CropYieldMultiplier { get; }
	public double SeedYieldMultiplier { get; }
	public double BeneficialScoreMultiplier { get; }
	public double HarmfulScoreMultiplier { get; }
	public int CropHealthDelta { get; }
	public int CropYieldDelta { get; }
	public ItemQuality OutputQuality { get; }

	public static AgricultureWorkOutcome FromSkill(
		double weightedFarmingSkill,
		double labourHours,
		double weightedSupervisorSkill,
		double supervisorHours)
	{
		var averageSkill = labourHours > 0.0 ? weightedFarmingSkill / labourHours : 35.0;
		var averageSupervisorSkill = supervisorHours > 0.0 ? weightedSupervisorSkill / supervisorHours : averageSkill;
		var supervisorShare = labourHours > 0.0 ? Math.Clamp(supervisorHours / labourHours, 0.0, 0.35) : 0.0;
		var supervisorBonus = supervisorShare * Math.Clamp((averageSupervisorSkill - averageSkill) / 100.0, -0.2, 0.35);
		var skillEffect = Math.Clamp((averageSkill - 35.0) / 100.0 + supervisorBonus, -0.25, 0.35);
		var quality = (ItemQuality)Math.Clamp((int)ItemQuality.Standard + (int)Math.Round(skillEffect * 8.0),
			(int)ItemQuality.Poor,
			(int)ItemQuality.Great);

		return new AgricultureWorkOutcome(
			weightedFarmingSkill,
			labourHours,
			weightedSupervisorSkill,
			supervisorHours,
			Math.Clamp(1.0 + skillEffect * 0.6, 0.85, 1.25),
			Math.Clamp(1.0 + skillEffect * 0.8, 0.80, 1.30),
			Math.Clamp(1.0 + skillEffect * 0.7, 0.80, 1.30),
			Math.Clamp(1.0 - skillEffect * 0.7, 0.75, 1.25),
			(int)Math.Round(skillEffect * 16.0),
			(int)Math.Round(skillEffect * 20.0),
			quality);
	}

	public string DescribeEffect()
	{
		if (LabourHours <= 0.0)
		{
			return string.Empty;
		}

		return $" Farming expertise produced a {OutputQuality.Describe()} commodity outcome and a {CropYieldMultiplier:P0} crop-yield factor.";
	}
}
