#nullable enable

using MudSharp.Events;
using MudSharp.Construction;
using MudSharp.Magic;
using MudSharp.Magic.Powers;

namespace MudSharp.Effects.Concrete;

public sealed class DelayedPsychicSuggestionEffect : TimedPsychicEffect, IHandleEventsEffect
{
	private bool _triggered;
	public long SourceId { get; }
	public long PowerId { get; }
	public string Trigger { get; }
	public long SubjectId { get; }
	public string Payload { get; }
	public string Emotion { get; }
	public DelayedPsychicSuggestionEffect(ICharacter owner, ICharacter source, PsychicTechniquePower power,
		string trigger, long subjectId, string payload, string emotion) : base(owner, power)
	{
		SourceId = CharacterInstanceIdentityComparer.IdentityId(source); PowerId = power.Id;
		Trigger = trigger; SubjectId = subjectId; Payload = payload; Emotion = emotion;
	}
	private DelayedPsychicSuggestionEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		SourceId = (long)root.Attribute("source")!; PowerId = (long)root.Attribute("power")!;
		Trigger = (string)root.Attribute("trigger")!; SubjectId = (long?)root.Attribute("subject") ?? 0;
		Payload = root.Value; Emotion = (string?)root.Attribute("emotion") ?? "";
	}
	public static void InitialiseEffectType() => RegisterFactory("DelayedPsychicSuggestion", (xml, owner) => new DelayedPsychicSuggestionEffect(xml, owner));
	protected override string SpecificEffectType => "DelayedPsychicSuggestion";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) => $"An untriggered psychic suggestion ({Trigger}).";
	protected override XElement SaveDefinition() => WithOrigin(new XElement("Effect", new XAttribute("source", SourceId), new XAttribute("power", PowerId),
		new XAttribute("trigger", Trigger), new XAttribute("subject", SubjectId), new XAttribute("emotion", Emotion), new XCData(Payload)));
	public bool HandlesEvent(params EventType[] types) => types.Any(x => x is EventType.CharacterEnterCell or EventType.CharacterEnterCellWitness or EventType.JoinCombat or EventType.EngageInCombat or EventType.EngagedInCombat);
	public bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (Owner is not ICharacter owner) return false;
		if (Trigger == "cell" && type == EventType.CharacterEnterCell && owner.Location?.Id == SubjectId ||
		    Trigger == "encounter" && type is EventType.CharacterEnterCell or EventType.CharacterEnterCellWitness &&
		    owner.Location?.CharactersInSpatialVicinity(owner).Any(x => CharacterInstanceIdentityComparer.IdentityId(x) == SubjectId && owner.CanSee(x)) == true ||
		    Trigger == "combat" && type is EventType.JoinCombat or EventType.EngageInCombat or EventType.EngagedInCombat) Activate();
		return false;
	}
	public override void ExpireEffect()
	{
		if (Trigger == "delay") Activate();
		else Owner.RemoveEffect(this);
	}
	private void Activate()
	{
		if (_triggered || Owner is not ICharacter target) return;
		_triggered = true;
		Owner.RemoveEffect(this);
		var source = Gameworld.TryGetCharacter(SourceId, true);
		if (source is null || Gameworld.MagicPowers.Get(PowerId) is not PsychicTechniquePower power) return;
		var result = MentalActionService.Resolve(new(source, target, power, MentalActionKind.Influence, true),
			power.SkillCheckTrait, power.SkillCheckDifficulty, power.MinimumSuccessThreshold);
		if (!result.Succeeded) return;
		if (string.IsNullOrEmpty(Emotion)) PsionicTrafficHelper.DeliverThought(source, target, power.School, Payload);
		else
		{
			var effect = PsychicEmotionEffect.Create(target, Emotion, power.Amount, SourceId);
			effect.OriginPowerId = PowerId;
			target.AddEffect(effect, power.Duration);
			PsionicTrafficHelper.DeliverEmotion(source, target, power.School, Emotion);
		}
		PsionicActivityNotifier.Notify(source, power, "an activated suggestion", target);
	}
}
