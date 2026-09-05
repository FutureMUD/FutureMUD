#nullable enable

using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class PsychicEmotionEffect : TimedPsychicEffect, ICheckBonusEffect, IPacifismEffect
{
	public static readonly string[] Modes = ["fear", "calm", "courage", "agitation", "affinity", "aversion"];
	public string Emotion { get; }
	public double Intensity { get; }
	public long SubjectId { get; }
	public PsychicEmotionEffect(IPerceivable owner, string emotion, double intensity, long subjectId) : base(owner)
	{ Emotion = emotion; Intensity = intensity; SubjectId = subjectId; }
	protected PsychicEmotionEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		Emotion = (string)root.Attribute("emotion")!;
		Intensity = (double)root.Attribute("intensity")!;
		SubjectId = (long?)root.Attribute("subject") ?? 0;
	}
	public static PsychicEmotionEffect Create(IPerceivable owner, string emotion, double intensity, long subjectId) => emotion == "fear"
		? new PsychicFearEffect(owner, intensity, subjectId) : new PsychicEmotionEffect(owner, emotion, intensity, subjectId);
	public static void InitialiseEffectType() => RegisterFactory("PsychicEmotion", (xml, owner) => new PsychicEmotionEffect(xml, owner));
	protected override string SpecificEffectType => "PsychicEmotion";
	public override bool SavingEffect => true;
	protected override XElement SaveDefinition() => WithOrigin(new XElement("Effect", new XAttribute("emotion", Emotion), new XAttribute("intensity", Intensity), new XAttribute("subject", SubjectId)));
	public override string Describe(IPerceiver voyeur) => $"Psychically induced {Emotion}.";
	public bool AppliesToCheck(CheckType type) => type.IsDefensiveCombatAction() || type.IsGeneralActivityCheck();
	public double CheckBonus => Emotion == "courage" ? Intensity : Emotion == "agitation" ? -Intensity : 0;
	public bool IsPeaceful => Emotion == "calm" && (Owner is not ICharacter actor || !MudSharp.NPC.AI.PsychicDispositionQuery.HasLegalDuties(actor));
	public bool IsSuperPeaceful => false;
	public static double Disposition(ICharacter actor, ICharacter subject) => actor.CombinedEffectsOfType<PsychicEmotionEffect>()
		.Where(x => x.SubjectId == CharacterInstanceIdentityComparer.IdentityId(subject))
		.Sum(x => x.Emotion == "affinity" ? x.Intensity : x.Emotion == "aversion" ? -x.Intensity : 0);
}

public sealed class PsychicFearEffect : PsychicEmotionEffect, IFearEffect
{
	public PsychicFearEffect(IPerceivable owner, double intensity, long subjectId) : base(owner, "fear", intensity, subjectId) { }
	private PsychicFearEffect(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public new static void InitialiseEffectType() => RegisterFactory("PsychicFear", (xml, owner) => new PsychicFearEffect(xml, owner));
	protected override string SpecificEffectType => "PsychicFear";
}
