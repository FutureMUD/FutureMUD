#nullable enable

using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

/// <summary>
/// Records that a character has positively identified a particular persistent trap instance.
/// It deliberately records an instance GUID rather than an item or cell reference so knowledge remains
/// correct when several traps share an anchor.
/// </summary>
public sealed class TrapKnowledgeEffect : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("TrapKnowledge", (effect, owner) => new TrapKnowledgeEffect(effect, owner));
	}

	public TrapKnowledgeEffect(ICharacter owner, Guid trapInstanceId, long templateId, int templateRevision)
		: base(owner)
	{
		TrapInstanceId = trapInstanceId;
		TemplateId = templateId;
		TemplateRevision = templateRevision;
	}

	private TrapKnowledgeEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var effect = root.Element("Effect")!;
		TrapInstanceId = Guid.Parse(effect.Element("TrapInstanceId")!.Value);
		TemplateId = long.Parse(effect.Element("TemplateId")?.Value ?? "0");
		TemplateRevision = int.Parse(effect.Element("TemplateRevision")?.Value ?? "0");
	}

	public Guid TrapInstanceId { get; }
	public long TemplateId { get; }
	public int TemplateRevision { get; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("TrapInstanceId", TrapInstanceId),
			new XElement("TemplateId", TemplateId),
			new XElement("TemplateRevision", TemplateRevision));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Knows the configuration of trap {TemplateId:N0}r{TemplateRevision:N0}.";
	}

	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "TrapKnowledge";
}

