#nullable enable
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;

namespace MudSharp.Magic.Powers;

public sealed class PsychicCirclePower : PsychicTechniquePower
{
	protected override string DefaultVerb => "psychiccircle";
	public static void RegisterLoader() => Register("psychiccircle", (m,w) => new PsychicCirclePower(m,w), (w,s,n,t) => new PsychicCirclePower(w,s,n,t));
	private PsychicCirclePower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private PsychicCirclePower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise("begin, invite <target>, dismiss <target>, end; participants use PSICIRCLE.");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var action = command.PopForSwitch();
		var circle = actor.EffectsOfType<PsychicCircleEffect>().FirstOrDefault(x => x.Power == this);
		if (action == "end") { if (circle is not null) actor.RemoveEffect(circle, true); return; }
		if (action == "begin")
		{
			if (circle is not null || actor.AffectedBy<PsychicCircleMembership>()) return;
			if (!TryPrepareTarget(actor, new StringStack("self"), "", out _)) return;
			if (!CanMaintain(actor)) return;
			if (!Resolve(actor, actor, MentalActionKind.Communication, false, out _)) return;
			actor.AddEffect(new PsychicCircleEffect(actor, this), Duration);
			SendSuccess(actor, actor);
			Complete(actor, actor, "a psychic circle");
			return;
		}
		if (circle is null) { actor.OutputHandler.Send("Begin a psychic circle first."); return; }
		if (!TryPrepareTarget(actor, command, "Which mind?", out var target) || target is null) return;
		if (action == "dismiss") { circle.Leave(target); return; }
		if (action != "invite") { actor.OutputHandler.Send("Choose begin, invite, dismiss, or end."); return; }
		if (circle.Members.Count >= CircleMemberLimit || target.AffectedBy<PsychicCircleMembership>()) { actor.OutputHandler.Send("That mind cannot join this circle."); return; }
		target.RemoveAllEffects<PsychicCircleInvitation>();
		target.AddEffect(new PsychicCircleInvitation(target, circle), TimeSpan.FromMinutes(2));
		SendEcho("TargetInviteEcho", target, actor, target);
		SendEcho("InviteEcho", actor, actor, target);
	}
	public static void ParticipantCommand(ICharacter actor, StringStack command)
	{
		var action = command.PopForSwitch();
		var invitation = actor.EffectsOfType<PsychicCircleInvitation>().FirstOrDefault();
		if (action is "accept" or "decline")
		{
			if (invitation is null) { actor.OutputHandler.Send("You have no pending circle invitation."); return; }
			actor.RemoveEffect(invitation);
			if (action == "accept" && !invitation.Circle.Join(actor)) actor.OutputHandler.Send("That invitation is no longer available.");
			return;
		}
		var membership = actor.EffectsOfType<PsychicCircleMembership>().FirstOrDefault();
		if (membership is null) { actor.OutputHandler.Send("You are not part of a psychic circle."); return; }
		switch (action)
		{
			case "leave": membership.Circle.Leave(actor); break;
			case "say": membership.Circle.Send(actor, command.SafeRemainingArgument); break;
			default: actor.OutputHandler.Send("Use PSICIRCLE SAY <text> or PSICIRCLE LEAVE."); break;
		}
	}
}
