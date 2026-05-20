#nullable enable

namespace MudSharp.Work.Agriculture;

public sealed class AgricultureCommodityYield
{
	public AgricultureCommodityYield(string materialName, double baseWeight, string tagName = "")
	{
		MaterialName = materialName;
		BaseWeight = baseWeight;
		TagName = tagName;
	}

	public string MaterialName { get; }
	public string TagName { get; }
	public double BaseWeight { get; }
}
