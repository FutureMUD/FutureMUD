using System;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;

namespace MudSharp.NPC.Templates;

public class HeightWeightModel : SaveableItem, IHeightWeightModel
{
	public double BMIMultiplier { get; private set; }
	public double MeanBMI { get; private set; }
	public double MeanHeight { get; private set; }
	public double StandardDeviationBMI { get; private set; }
	public double StandardDeviationHeight { get; private set; }
	public double? MeanWeight { get; private set; }
	public double? StandardDeviationWeight { get; private set; }

	public HeightWeightModel(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		BMIMultiplier = 0.1;
		MeanHeight = 177;
		MeanBMI = 25.6;
		StandardDeviationBMI = 4.9;
		StandardDeviationHeight = 7.6;
		using (new FMDB())
		{
			var dbitem = new Models.HeightWeightModel
			{
				Name = Name,
				Bmimultiplier = BMIMultiplier,
				MeanHeight = MeanHeight,
				MeanBmi = MeanBMI,
				StddevBmi = StandardDeviationBMI,
				StddevHeight = StandardDeviationHeight,
			};
			FMDB.Context.HeightWeightModels.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private HeightWeightModel(HeightWeightModel rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		BMIMultiplier = rhs.BMIMultiplier;
		MeanHeight = rhs.MeanHeight;
		MeanBMI = rhs.MeanBMI;
		StandardDeviationBMI = rhs.StandardDeviationBMI;
		StandardDeviationHeight = rhs.StandardDeviationHeight;
		MeanWeight = rhs.MeanWeight;
		StandardDeviationWeight = rhs.StandardDeviationWeight;
		using (new FMDB())
		{
			var dbitem = new Models.HeightWeightModel
			{
				Name = Name,
				Bmimultiplier = BMIMultiplier,
				MeanHeight = MeanHeight,
				MeanBmi = MeanBMI,
				StddevBmi = StandardDeviationBMI,
				StddevHeight = StandardDeviationHeight,
				MeanWeight = MeanWeight,
				StddevWeight = StandardDeviationWeight
			};
			FMDB.Context.HeightWeightModels.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public HeightWeightModel(IFuturemud gameworld, MudSharp.Models.HeightWeightModel model)
	{
		Gameworld = gameworld;
		_id = model.Id;
		_name = model.Name;
		MeanHeight = model.MeanHeight;
		MeanBMI = model.MeanBmi;
		StandardDeviationHeight = model.StddevHeight;
		StandardDeviationBMI = model.StddevBmi;
		MeanWeight = model.MeanWeight;
		StandardDeviationWeight = model.StddevWeight;
		BMIMultiplier = model.Bmimultiplier;
	}

	public IHeightWeightModel Clone(string newName)
	{
		return new HeightWeightModel(this, newName);
	}

	public override string FrameworkItemType => "HeightWeightModel";

	public (double, double) GetRandomHeightWeight()
	{
		var height = RandomUtilities.RandomNormal(MeanHeight, StandardDeviationHeight);
		double weight;
		if (MeanWeight is not null && StandardDeviationWeight is not null)
		{
			weight = RandomUtilities.RandomNormal(MeanWeight.Value, StandardDeviationWeight.Value);
		}
		else
		{
			var bmi = RandomUtilities.RandomNormal(MeanBMI, StandardDeviationBMI);
			weight = Math.Pow(height, 2) * bmi * BMIMultiplier;
		}

		return (height, weight);
	}

	public (double Height, double Weight) GetHeightWeight(double heightStdDev, double bmiStdDev)
	{
		var height = MeanHeight + heightStdDev * StandardDeviationHeight;
		var weight = Math.Pow(height, 2) * (MeanBMI + bmiStdDev * StandardDeviationBMI) * BMIMultiplier;
		return (height, weight);
	}

	/// <summary>
	/// Calculates BMI
	/// </summary>
	/// <param name="height">The height in cm</param>
	/// <param name="weight">The weight in grams</param>
	/// <returns>The BMI in kg/m3</returns>
	public static double GetBMI(double height, double weight)
	{
		weight /= 1000;
		height /= 100;
		return weight / (Math.Pow(height, 2));
	}

	/// <summary>
	/// Calculates the Standard Deviation of Weight based on known BMI/Height values
	/// </summary>
	/// <param name="meanHeight">The mean height in cm</param>
	/// <param name="meanBMI">The mean BMI in kg/m3</param>
	/// <param name="stdDevHeight">The standard deviation of height in cm</param>
	/// <param name="stdDevBMI">The standard deviation of BMI in kg/m3</param>
	/// <returns>The standard deviation of weight in grams</returns>
	public static double GetStdDevWeight(double meanHeight, double meanBMI, double stdDevHeight, double stdDevBMI)
	{
		meanHeight /= 100;
		stdDevHeight /= 100;
		return Math.Sqrt(Math.Pow(2 * meanBMI * meanHeight * stdDevHeight, 2) + Math.Pow(Math.Pow(meanHeight, 2) * stdDevBMI, 2)) * 1000;
	}

	/// <summary>
	/// Calculates the Standard Deviation of BMI based on known height and weight values
	/// </summary>
	/// <param name="meanHeight">The mean height in cm</param>
	/// <param name="meanBMI">The mean BMI in kg/m3</param>
	/// <param name="stdDevHeight">The standard deviation of height in cm</param>
	/// <param name="stdDevWeight">The standard deviation of weight in grams</param>
	/// <returns>The standard deviation of BMI in kg/m3</returns>
	public static double GetStdDevBMI(double meanHeight, double meanBMI, double stdDevHeight, double stdDevWeight)
	{
		meanHeight /= 100;
		stdDevWeight /= 1000;
		stdDevHeight /= 100;
		return Math.Sqrt(Math.Pow(stdDevWeight,2)-Math.Pow(2*meanBMI*meanHeight*stdDevHeight, 2)) / Math.Pow(meanHeight, 2);
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.HeightWeightModels.Find(Id);
		dbitem.Name = Name;
		dbitem.MeanHeight = MeanHeight;
		dbitem.MeanBmi = MeanBMI;
		dbitem.StddevBmi = StandardDeviationBMI;
		dbitem.StddevHeight = StandardDeviationHeight;
		dbitem.MeanWeight = MeanWeight;
		dbitem.StddevWeight = StandardDeviationWeight;
		dbitem.Bmimultiplier = BMIMultiplier;
		Changed = false;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Height/Weight Model #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Mean Height: {Gameworld.UnitManager.DescribeExact(MeanHeight,UnitType.Length, actor).ColourValue()}");
		
		if (MeanWeight is not null && StandardDeviationWeight is not null)
		{
			sb.AppendLine($"Mean Weight: {Gameworld.UnitManager.DescribeExact(MeanWeight.Value, UnitType.Mass, actor).ColourValue()}");
		}
		else
		{
			sb.AppendLine($"Mean BMI: {Gameworld.UnitManager.DescribeExact(MeanBMI, UnitType.BMI, actor).ColourValue()}");
		}

		sb.AppendLine($"Standard Deviation Height: {Gameworld.UnitManager.DescribeExact(StandardDeviationHeight, UnitType.Length, actor).ColourValue()}");
		if (MeanWeight is not null && StandardDeviationWeight is not null)
		{
			sb.AppendLine($"Standard Deviation Weight: {Gameworld.UnitManager.DescribeExact(StandardDeviationWeight.Value, UnitType.Mass, actor).ColourValue()}");
		}
		else
		{
			sb.AppendLine($"Standard Deviation BMI: {Gameworld.UnitManager.DescribeExact(StandardDeviationBMI, UnitType.BMI, actor).ColourValue()}");
		}
			
		return sb.ToString();
	}

	public const string BuildingCommandHelp = @"You can use the following options with this command:

	#3name <name>#0 - renames this model
	#3meanbmi <value>#0 - sets the mean (average) BMI
	#3stddevbmi <value>#0 - sets the standard deviation of BMI
	#3meanheight <value>#0 - sets the mean (average) height
	#3stddevheight <value>#0 - sets the standard deviation of height
	#3meanweight <value>#0 - sets the mean (average) BMI via weight
	#3stddevweight <value#0 - sets the standard deviation of BMI via weight

Note - models only use one of either BMI or weight. Setting one switches off the other.";

	public bool BuildingCommand(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, ss);
			case "meanbmi":
			case "avgbmi":
			case "averagebmi":
				return BuildingCommandMeanBMI(actor, ss);
			case "meanheight":
			case "avgheight":
			case "averageheight":
				return BuildingCommandMeanHeight(actor, ss);
			case "stddevbmi":
				return BuildingCommandStdDevBMI(actor, ss);
			case "stddevheight":
				return BuildingCommandStdDevHeight(actor, ss);
			case "meanweight":
			case "avgweight":
			case "averageweight":
				return BuildingCommandMeanWeight(actor, ss);
			case "stddevweight":
				return BuildingCommandStdDevWeight(actor, ss);
			default:
				actor.OutputHandler.Send(BuildingCommandHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandStdDevHeight(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the standard deviation of height for this model?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(ss.SafeRemainingArgument, UnitType.Length, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid height.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.OutputHandler.Send("Heights cannot be negative or zero.");
			return false;
		}

		StandardDeviationHeight = value;
		Changed = true;
		actor.OutputHandler.Send($"The standard deviation of height for this model is now {Gameworld.UnitManager.DescribeExact(value, UnitType.Length, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandStdDevBMI(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the standard deviation of BMI for this model?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.OutputHandler.Send("BMIs cannot be negative or zero.");
			return false;
		}

		StandardDeviationBMI = value;
		Changed = true;
		actor.OutputHandler.Send($"The standard deviation for BMI for this model is now {Gameworld.UnitManager.DescribeExact(value, UnitType.BMI, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMeanHeight(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the mean (average) height for this model?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(ss.SafeRemainingArgument, UnitType.Length, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid height.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.OutputHandler.Send("Heights cannot be negative or zero.");
			return false;
		}

		MeanHeight = value;
		Changed = true;
		actor.OutputHandler.Send($"The mean height for this model is now {Gameworld.UnitManager.DescribeExact(value, UnitType.Length, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMeanBMI(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the mean (average) BMI for this model?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.OutputHandler.Send("BMIs cannot be negative or zero.");
			return false;
		}

		MeanBMI = value;
		MeanWeight = null;
		StandardDeviationWeight = null;
		Changed = true;
		actor.OutputHandler.Send($"The mean BMI for this model is now {Gameworld.UnitManager.DescribeExact(value, UnitType.BMI, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMeanWeight(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the mean (average) weight for this model?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(ss.SafeRemainingArgument, UnitType.Mass, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid weight.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.OutputHandler.Send("Weights cannot be negative or zero.");
			return false;
		}

		MeanWeight = value;
		if (StandardDeviationWeight is null)
		{
			StandardDeviationWeight = MeanWeight * 0.1;
		}
		Changed = true;
		actor.OutputHandler.Send($"The mean weight for this model is now {Gameworld.UnitManager.DescribeExact(value, UnitType.Mass, actor).ColourValue()}, which leads to a BMI of {Gameworld.UnitManager.DescribeExact(MeanBMI, UnitType.BMI, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandStdDevWeight(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the standard deviation of weight be for this model?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(ss.SafeRemainingArgument, UnitType.Mass, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid weight.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.OutputHandler.Send("Weights cannot be negative or zero.");
			return false;
		}

		StandardDeviationWeight = value;
		Changed = true;
		actor.OutputHandler.Send($"The standard deviation of weight for this model is now {Gameworld.UnitManager.DescribeExact(value, UnitType.Mass, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this Height/Weight Model?");
			return false;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (Gameworld.HeightWeightModels.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a Height/Weight Model called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this Height/Weight Model from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}
}