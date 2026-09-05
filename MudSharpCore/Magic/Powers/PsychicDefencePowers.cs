#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;

namespace MudSharp.Magic.Powers;

public sealed class GuardMindPower : PsychicTechniquePower
{
	protected override string DefaultVerb => "guardmind";
	public static void RegisterLoader() => Register("guardmind", (m,w) => new GuardMindPower(m,w), (w,s,n,t) => new GuardMindPower(w,s,n,t));
	private GuardMindPower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private GuardMindPower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise("<target> or end to guard a willing linked mind.");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (command.PeekSpeech().EqualTo("end")) { actor.RemoveAllEffects<MaintainedPsychicEffect>(x => x.Power == this, true); return; }
		if (!TryPrepareTarget(actor, command, "Whose mind do you want to guard?", out var target) || target is null) return;
		if (!target.IsTrustedAlly(actor) || !actor.EffectsOfType<ConnectMindEffect>().Any(x => x.TargetCharacter == target && x.School == School))
		{ actor.OutputHandler.Send("You must be connected to a mind that trusts you."); return; }
		if (command.PopForSwitch() == "expel")
		{
			var intrusion = target.EffectsOfType<MindConnectedToEffect>().FirstOrDefault(x => x.OriginatorCharacter != actor);
			if (intrusion is not null && Resolve(actor, intrusion.OriginatorCharacter, MentalActionKind.Disruption, true, out _))
			{
				intrusion.OriginatorCharacter.RemoveEffect(intrusion.OriginatorEffect, true);
				Complete(actor, target, "an assisted mental expulsion");
			}
			actor.Send("You help the willing mind push against foreign presences.");
			return;
		}
		if (!CanMaintain(actor)) return;
		if (!Resolve(actor, target, MentalActionKind.Investigation, false, out _)) return;
		actor.RemoveAllEffects<MaintainedPsychicEffect>(x => x.Power == this, true);
		actor.AddEffect(new MaintainedPsychicEffect(actor, target, this, "guard"), Duration);
		actor.OutputHandler.Send("You extend your mental protection over the linked mind.");
		Complete(actor, target, "a mental aegis");
	}
}

public sealed class PsychicFeedbackPower : PsychicTechniquePower
{
	protected override string DefaultVerb => "psychicfeedback";
	public static void RegisterLoader() => Register("psychicfeedback", (m,w) => new PsychicFeedbackPower(m,w), (w,s,n,t) => new PsychicFeedbackPower(w,s,n,t));
	private PsychicFeedbackPower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private PsychicFeedbackPower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise("begin or end to maintain reactive psychic defence.");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (command.PeekSpeech().EqualTo("end")) { actor.RemoveAllEffects<MaintainedPsychicEffect>(x => x.Power == this, true); return; }
		if (!TryPrepareTarget(actor, new StringStack("self"), "", out _)) return;
		if (!CanMaintain(actor)) return;
		if (!Resolve(actor, actor, MentalActionKind.Investigation, false, out _)) return;
		actor.RemoveAllEffects<MaintainedPsychicEffect>(x => x.Power == this, true);
		actor.AddEffect(new MaintainedPsychicEffect(actor, actor, this, "feedback"), Duration);
		actor.OutputHandler.Send("You prepare your mind to react to intrusion.");
		Complete(actor, actor, "a psychic feedback defence");
	}
}
