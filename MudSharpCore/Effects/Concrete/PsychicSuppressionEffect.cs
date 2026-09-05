#nullable enable

namespace MudSharp.Effects.Concrete;

public class PsychicSuppressionEffect : TimedPsychicEffect
{
	public string Kind { get; }
	public long SubjectId { get; }
	public PsychicSuppressionEffect(IPerceivable owner, string kind, long subjectId) : base(owner)
	{ Kind = kind; SubjectId = subjectId; }
	protected PsychicSuppressionEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		Kind = (string)xml.Element("Effect")!.Attribute("kind")!;
		SubjectId = (long)xml.Element("Effect")!.Attribute("subject")!;
	}
	public static void InitialiseEffectType() => RegisterFactory("PsychicSuppression", (xml, owner) => new PsychicSuppressionEffect(xml, owner));
	protected override string SpecificEffectType => "PsychicSuppression";
	public override bool SavingEffect => true;
	protected override XElement SaveDefinition() => WithOrigin(new XElement("Effect", new XAttribute("kind", Kind), new XAttribute("subject", SubjectId)));
	public override string Describe(IPerceiver voyeur) => $"Psychic suppression of {Kind} #{SubjectId.ToString("N0", voyeur)}.";
	public static bool Suppresses(ICharacter actor, string kind, long id) => actor.CombinedEffectsOfType<PsychicSuppressionEffect>()
		.Any(x => x.Kind == kind && x.SubjectId == id);
}

public sealed class PsychicSkillSuppressionEffect : PsychicSuppressionEffect, INoTraitGainEffect
{
	public PsychicSkillSuppressionEffect(IPerceivable owner, long traitId) : base(owner, "skill", traitId) { }
	private PsychicSkillSuppressionEffect(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public new static void InitialiseEffectType() => RegisterFactory("PsychicSkillSuppression", (xml, owner) => new PsychicSkillSuppressionEffect(xml, owner));
	protected override string SpecificEffectType => "PsychicSkillSuppression";
	public MudSharp.Body.Traits.ITraitDefinition Trait => Gameworld.Traits.Get(SubjectId)!;
	public override bool Applies(object target) => target is MudSharp.Body.Traits.ITraitDefinition trait && trait.Id == SubjectId;
}
