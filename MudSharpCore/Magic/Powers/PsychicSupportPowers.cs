#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public sealed class SomaticSensePower : PsychicTechniquePower
{
	protected override string DefaultVerb => "somaticsense";
	public static void RegisterLoader() => Register("somaticsense", (m,w) => new SomaticSensePower(m,w), (w,s,n,t) => new SomaticSensePower(w,s,n,t));
	private SomaticSensePower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private SomaticSensePower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise("<target> to sense bodily distress.");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose physical distress do you want to sense?", out var target) || target is null) return;
		if (!Resolve(actor, target, MentalActionKind.Investigation, true, out var result)) return;
		var fatigue = target.MaximumStamina <= 0 ? 0 : target.CurrentStamina / target.MaximumStamina;
		actor.OutputHandler.Send($"You sense {target.HowSeen(actor)} as {target.State.Describe()}, " +
			(fatigue < 0.2 ? "exhausted" : fatigue < 0.6 ? "tired" : "rested") + ".");
		if (result.Outcome >= Outcome.Pass)
		{
			actor.OutputHandler.Send(target.Wounds.Any(x => x.CurrentPain > 0) ? "There is pain within that body." : "You sense no wound pain.");
			actor.OutputHandler.Send(target.Wounds.Any() ? $"The worst wound feels {target.Wounds.Max(x => x.Severity).DescribeEnum().ToLowerInvariant()}." : "You sense no wounds.");
		}
		Complete(actor, target, "a somatic reading");
	}
}

public sealed class PsychicTransferPower : PsychicTechniquePower
{
	protected override string DefaultVerb => "transferfocus";
	public static void RegisterLoader() => Register("transferfocus", (m,w) => new PsychicTransferPower(m,w), (w,s,n,t) => new PsychicTransferPower(w,s,n,t));
	private PsychicTransferPower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private PsychicTransferPower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise("<target> <lend|siphon> to transfer focus.");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose resources do you want to transfer?", out var target) || target is null) return;
		var mode = command.PopForSwitch();
		if (mode is not ("lend" or "siphon") || target == actor) { actor.OutputHandler.Send("Choose another mind and either lend or siphon."); return; }
		if (mode == "lend" && !target.IsTrustedAlly(actor)) { actor.OutputHandler.Send("That mind has not trusted you to lend it strength."); return; }
		var resource = Gameworld.MagicResources.Get(ResourceId);
		if (resource is null) { actor.OutputHandler.Send("This power has no configured resource."); return; }
		if (!Resolve(actor, target, MentalActionKind.ResourceTransfer, mode == "siphon", out _)) return;
		// Resolution has already paid the invocation cost, before reactions or resource transfer.
		var transfer = MagicResourceTransfer.Transfer(mode == "siphon" ? target : actor, mode == "siphon" ? actor : target, resource, Amount, Loss);
		actor.OutputHandler.Send($"You transfer {transfer.Received.ToString("N2", actor).ColourValue()} {resource.Name.ColourName()}.");
		PsionicActivityNotifier.Notify(actor, this, "a psychic resource transfer", target);
	}
}

public sealed class DisruptConcentrationPower : PsychicTechniquePower
{
	protected override string DefaultVerb => "disruptconcentration";
	public static void RegisterLoader() => Register("disruptconcentration", (m,w) => new DisruptConcentrationPower(m,w), (w,s,n,t) => new DisruptConcentrationPower(w,s,n,t));
	private DisruptConcentrationPower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private DisruptConcentrationPower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise("<target> to challenge a sustained effect.");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose concentration do you want to disrupt?", out var target) || target is null) return;
		if (!Resolve(actor, target, MentalActionKind.Disruption, true, out _)) return;
		var effect = target.EffectsOfType<ConcentrationConsumingEffect>().OrderByDescending(x => x.ConcentrationPointsConsumed).FirstOrDefault();
		effect?.ChallengeConcentration(SkillCheckDifficulty);
		actor.OutputHandler.Send("You strike at the other mind's concentration.");
		Complete(actor, target, "a concentration disruption");
	}
}

public sealed class DreamsendPower : PsychicTechniquePower
{
	protected override string DefaultVerb => "dreamsend";
	public static void RegisterLoader() => Register("dreamsend", (m,w) => new DreamsendPower(m,w), (w,s,n,t) => new DreamsendPower(w,s,n,t));
	private DreamsendPower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private DreamsendPower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise("<target> <dream text> to send a brief dream.");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose dreams do you want to reach?", out var target) || target is null) return;
		var text = command.SafeRemainingArgument.Sanitise().RawText();
		if (string.IsNullOrWhiteSpace(text) || text.Length > 2000) { actor.OutputHandler.Send("Supply dream text of at most 2000 characters."); return; }
		if (!target.State.HasFlag(CharacterState.Sleeping) || target.AffectedBy<INoDreamEffect>() || target.AffectedBy<IDreamingEffect>() ||
		    target.Location?.EffectsOfType<INoDreamEffect>().Any() == true)
		{ actor.OutputHandler.Send("That mind is not available to receive a dream."); return; }
		if (!Resolve(actor, target, MentalActionKind.Influence, true, out _)) return;
		target.OutputHandler.Send($"Within a dream, you experience:\n{text}");
		actor.OutputHandler.Send("You send a brief dream into the sleeping mind.");
		PsionicTrafficHelper.Audit(actor, target, "sent a dream to", text);
		Complete(actor, target, "a dream sending");
	}
}
