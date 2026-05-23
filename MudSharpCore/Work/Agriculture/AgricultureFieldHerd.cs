using MudSharp.Framework;

namespace MudSharp.Work.Agriculture;

public class AgricultureFieldHerd : FrameworkItem, IAgricultureFieldHerd
{
	public AgricultureFieldHerd(long id, IAgricultureHerdDefinition definition, int headCount, double condition,
		int secondaryYieldPotential = 0)
	{
		_id = id;
		_name = definition?.Name ?? "Unknown Herd";
		Definition = definition;
		HeadCount = headCount;
		Condition = condition;
		SecondaryYieldPotential = secondaryYieldPotential.ClampScore();
	}

	public override string FrameworkItemType => "AgricultureFieldHerd";
	public IAgricultureHerdDefinition Definition { get; }
	public int HeadCount { get; internal set; }
	public double Condition { get; internal set; }
	public int SecondaryYieldPotential { get; internal set; }
}
