using MudSharp.Framework;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class ForcedTransformationBaselineEffect : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("ForcedTransformationBaseline", (effect, owner) =>
			new ForcedTransformationBaselineEffect(effect, owner));
	}

	public ForcedTransformationBaselineEffect(IPerceivable owner, long baselineBodyId)
		: base(owner)
	{
		BaselineBodyId = baselineBodyId;
	}

	private ForcedTransformationBaselineEffect(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		BaselineBodyId = long.Parse(effect.Element("Effect")?.Element("BaselineBodyId")?.Value ?? "0");
	}

	public long BaselineBodyId { get; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("BaselineBodyId", BaselineBodyId)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Preserving a baseline form for mandatory transformations.";
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "ForcedTransformationBaseline";
}
