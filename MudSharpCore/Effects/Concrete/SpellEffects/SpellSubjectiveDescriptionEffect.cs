#nullable enable

using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellSubjectiveDescriptionEffect : MagicSpellEffectBase, IOverrideDescEffect, IPrioritisedOverrideDescEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellSubjectiveDescription",
			(effect, owner) => new SpellSubjectiveDescriptionEffect(effect, owner));
	}

	public SpellSubjectiveDescriptionEffect(IPerceivable owner, IMagicSpellEffectParent parent,
		DescriptionType descriptionType, string description, long fixedPerceiverId, IFutureProg? prog = null,
		int priority = 0, string overrideKey = "")
		: base(owner, parent, prog)
	{
		DescriptionType = descriptionType;
		DescriptionText = description;
		FixedPerceiverId = fixedPerceiverId;
		OverridePriority = priority;
		OverrideKey = overrideKey;
	}

	private SpellSubjectiveDescriptionEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		DescriptionType = (DescriptionType)int.Parse(trueRoot?.Element("DescriptionType")?.Value ?? "0");
		DescriptionText = trueRoot?.Element("Description")?.Value ?? string.Empty;
		FixedPerceiverId = long.Parse(trueRoot?.Element("FixedPerceiver")?.Value ?? "0");
		OverridePriority = int.Parse(trueRoot?.Element("Priority")?.Value ?? "0");
		OverrideKey = trueRoot?.Element("OverrideKey")?.Value ?? string.Empty;
	}

	public DescriptionType DescriptionType { get; }
	public string DescriptionText { get; }
	public long FixedPerceiverId { get; }
	public int OverridePriority { get; }
	public string OverrideKey { get; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("DescriptionType", (int)DescriptionType),
			new XElement("Description", new XCData(DescriptionText)),
			new XElement("FixedPerceiver", FixedPerceiverId),
			new XElement("Priority", OverridePriority),
			new XElement("OverrideKey", new XCData(OverrideKey))
		);
	}

	public bool OverrideApplies(IPerceiver voyeur, DescriptionType type)
	{
		return type == DescriptionType &&
		       (FixedPerceiverId == 0 || voyeur?.Id == FixedPerceiverId) &&
		       Applies(voyeur);
	}

	public string Description(DescriptionType type, bool colour)
	{
		return DescriptionText;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Magically overridden {DescriptionType.DescribeEnum().ToLowerInvariant()} description.";
	}

	protected override string SpecificEffectType => "SpellSubjectiveDescription";
}
