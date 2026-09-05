#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;

namespace MudSharp.Magic.Powers;

public sealed class EmotionalInfluencePower : PsychicTechniquePower
{
	protected override string DefaultVerb => "emotionalinfluence";
	public static void RegisterLoader() => Register("emotionalinfluence", (m,w) => new EmotionalInfluencePower(m,w), (w,s,n,t) => new EmotionalInfluencePower(w,s,n,t));
	private EmotionalInfluencePower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private EmotionalInfluencePower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise("<target> <read|fear|calm|courage|agitation|affinity|aversion> [subject].");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose emotions do you want to reach?", out var target) || target is null) return;
		var mode = command.PopForSwitch();
		if (mode != "read" && !PsychicEmotionEffect.Modes.Contains(mode)) { actor.OutputHandler.Send("Choose read or a supported emotion."); return; }
		var subject = mode is "affinity" or "aversion" ? actor.TargetActor(command.SafeRemainingArgument) : actor;
		if (subject is null) { actor.OutputHandler.Send("Specify a person you can identify here."); return; }
		if (!Resolve(actor, target, mode == "read" ? MentalActionKind.Investigation : MentalActionKind.Influence, true, out _)) return;
		if (mode == "read")
		{
			var emotions = target.CombinedEffectsOfType<PsychicEmotionEffect>().Select(x => x.Emotion).Distinct().ToList();
			actor.OutputHandler.Send(emotions.Any() ? $"You sense {emotions.ListToString()} within that mind." : "No distinct emotional influence emerges.");
		}
		else
		{
			target.RemoveAllEffects<PsychicEmotionEffect>(x => x.Emotion == mode && x.SubjectId == CharacterInstanceIdentityComparer.IdentityId(subject));
			var effect = PsychicEmotionEffect.Create(target, mode, Amount, CharacterInstanceIdentityComparer.IdentityId(subject));
			effect.OriginPowerId = Id;
			target.AddEffect(effect, Duration);
			PsionicTrafficHelper.DeliverEmotion(actor, target, School, mode);
		}
		Complete(actor, target, "emotional attunement");
	}
}
