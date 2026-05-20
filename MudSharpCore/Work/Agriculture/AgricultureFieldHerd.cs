using MudSharp.Framework;

namespace MudSharp.Work.Agriculture;

public class AgricultureFieldHerd : FrameworkItem, IAgricultureFieldHerd
{
	public AgricultureFieldHerd(long id, IAgricultureHerdDefinition definition, int headCount, double condition)
	{
		_id = id;
		_name = definition?.Name ?? "Unknown Herd";
		Definition = definition;
		HeadCount = headCount;
		Condition = condition;
	}

	public override string FrameworkItemType => "AgricultureFieldHerd";
	public IAgricultureHerdDefinition Definition { get; }
	public int HeadCount { get; internal set; }
	public double Condition { get; internal set; }
}
