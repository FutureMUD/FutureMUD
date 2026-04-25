using MudSharp.Framework;

namespace MudSharp.Construction;

public enum GravityModel
{
	Normal = 0,
	ZeroGravity = 1
}

public static class GravityModelExtensions
{
	public static string Describe(this GravityModel model)
	{
		return model switch
		{
			GravityModel.Normal => "Normal Gravity",
			GravityModel.ZeroGravity => "Zero Gravity",
			_ => "Unknown Gravity"
		};
	}

	public static string DescribeColour(this GravityModel model)
	{
		return model switch
		{
			GravityModel.Normal => "Normal Gravity".ColourValue(),
			GravityModel.ZeroGravity => "Zero Gravity".Colour(Telnet.BoldCyan),
			_ => "Unknown Gravity".ColourError()
		};
	}
}
