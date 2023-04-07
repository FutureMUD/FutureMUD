using System;
using MudSharp.Framework;

namespace MudSharp.NPC.Templates;

public class HeightWeightModel : FrameworkItem, IHeightWeightModel
{
	public double BMIMultiplier { get; }
	public double MeanBMI { get; }
	public double MeanHeight { get; }
	public double StandardDeviationBMI { get; }
	public double StandardDeviationHeight { get; }

	public HeightWeightModel(MudSharp.Models.HeightWeightModel model)
	{
		_id = model.Id;
		_name = model.Name;
		MeanHeight = model.MeanHeight;
		MeanBMI = model.MeanBmi;
		StandardDeviationHeight = model.StddevHeight;
		StandardDeviationBMI = model.StddevBmi;
		BMIMultiplier = model.Bmimultiplier;
	}

	public override string FrameworkItemType => "HeightWeightModel";

	public (double, double) GetRandomHeightWeight()
	{
		var height = RandomUtilities.RandomNormal(MeanHeight, StandardDeviationHeight);
		var bmi = RandomUtilities.RandomNormal(MeanBMI, StandardDeviationBMI);
		var weight = Math.Pow(height, 2) * bmi * BMIMultiplier;
		return (height, weight);
	}
}