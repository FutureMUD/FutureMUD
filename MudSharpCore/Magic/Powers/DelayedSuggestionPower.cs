#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;

namespace MudSharp.Magic.Powers;

public sealed class DelayedSuggestionPower : PsychicTechniquePower
{
	protected override string DefaultVerb => "delayedsuggestion";
	public static void RegisterLoader() => Register("delayedsuggestion", (m,w) => new DelayedSuggestionPower(m,w), (w,s,n,t) => new DelayedSuggestionPower(w,s,n,t));
	private DelayedSuggestionPower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private DelayedSuggestionPower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) =>
		Initialise("<target> <delay seconds|cell here|encounter person|combat> <thought text|emotion mode>.");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose mind should hold a delayed suggestion?", out var target) || target is null) return;
		var trigger = command.PopForSwitch();
		long subjectId = 0;
		var lifetime = Duration;
		switch (trigger)
		{
			case "delay":
				if (!double.TryParse(command.PopSpeech(), out var seconds) || !double.IsFinite(seconds) || seconds < 1 || seconds > Duration.TotalSeconds)
				{ actor.OutputHandler.Send("Specify a delay within the power's duration."); return; }
				lifetime = TimeSpan.FromSeconds(seconds);
				break;
			case "cell":
				if (!command.PopSpeech().EqualTo("here")) { actor.OutputHandler.Send("Use cell here to select this location."); return; }
				subjectId = actor.Location.Id;
				break;
			case "encounter":
				var subject = actor.TargetActor(command.PopSpeech());
				if (subject is null) { actor.OutputHandler.Send("Identify a person here."); return; }
				subjectId = CharacterInstanceIdentityComparer.IdentityId(subject);
				break;
			case "combat": break;
			default: actor.OutputHandler.Send("Choose delay, cell, encounter, or combat."); return;
		}
		var payloadMode = command.PopForSwitch();
		var payload = command.SafeRemainingArgument.Sanitise().RawText();
		if (payloadMode is not ("thought" or "emotion") || string.IsNullOrWhiteSpace(payload) || payload.Length > 2000 ||
		    payloadMode == "emotion" && !PsychicEmotionEffect.Modes.Contains(payload))
		{ actor.OutputHandler.Send("Supply thought text or a supported emotion."); return; }
		if (!Resolve(actor, target, MentalActionKind.Influence, true, out _)) return;
		target.AddEffect(new DelayedPsychicSuggestionEffect(target, actor, this, trigger, subjectId,
			payloadMode == "thought" ? payload : "", payloadMode == "emotion" ? payload : ""), lifetime);
		PsionicTrafficHelper.Audit(actor, target, "planted a delayed suggestion in", payload);
		SendEcho("SuccessEcho", actor, actor, target);
		Complete(actor, target, "a delayed suggestion");
	}
}
