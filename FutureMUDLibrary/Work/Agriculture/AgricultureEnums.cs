using MudSharp.Framework;

namespace MudSharp.Work.Agriculture;

public enum AgricultureFieldUse
{
	Fallow = 0,
	Crop = 1,
	Pasture = 2,
	Woodland = 3,
	Orchard = 4
}

public enum AgricultureCropStage
{
	None = 0,
	Planted = 1,
	Germinating = 2,
	Growing = 3,
	Setting = 4,
	Harvestable = 5,
	Overripe = 6,
	Failed = 7
}

public enum AgricultureOperationType
{
	Improve = 0,
	Sow = 1,
	Harvest = 2,
	Graze = 3,
	Herd = 4,
	Woodland = 5,
	Clear = 6,
	PlantOrchard = 7,
	InstallApiary = 8,
	TendApiary = 9,
	HarvestApiary = 10,
	RemoveApiary = 11,
	HarvestHerdProducts = 12
}

public enum AgricultureTargetType
{
	None = 0,
	Crop = 1,
	Herd = 2,
	Woodland = 3
}

public enum AgriculturePollinationDependency
{
	None = 0,
	Beneficial = 1,
	Strong = 2,
	Required = 3
}

public enum AgricultureScoreType
{
	Moisture = 0,
	Drainage = 1,
	Nutrients = 2,
	Salinity = 3,
	Topsoil = 4,
	Tilth = 5,
	Rockiness = 6,
	Weeds = 7,
	Pests = 8,
	Fence = 9,
	Pasture = 10,
	Condition = 11,
	Custom1 = 100,
	Custom2 = 101,
	Custom3 = 102,
	Custom4 = 103,
	Custom5 = 104,
	Custom6 = 105,
	Custom7 = 106,
	Custom8 = 107,
	Custom9 = 108,
	Custom10 = 109,
	Custom11 = 110,
	Custom12 = 111
}

public static class AgricultureScoreExtensions
{
	public static string DescribeBand(this int score)
	{
		return score switch
		{
			<= 10 => "ruined",
			<= 25 => "very poor",
			<= 40 => "poor",
			<= 60 => "fair",
			<= 75 => "good",
			<= 90 => "very good",
			_ => "excellent"
		};
	}

	public static string DescribeBandColoured(this int score)
	{
		return score switch
		{
			<= 25 => score.DescribeBand().Colour(Telnet.Red),
			<= 40 => score.DescribeBand().Colour(Telnet.Yellow),
			<= 75 => score.DescribeBand().Colour(Telnet.Green),
			_ => score.DescribeBand().Colour(Telnet.BoldGreen)
		};
	}
}
