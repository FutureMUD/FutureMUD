#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

/// <summary>A resisted, private mental vision delivered only to the selected target.</summary>
public sealed class MentalVisionPower : PsionicTargetedPowerBase
{
	public override string PowerType => "Masquerade";
	public override string DatabaseType => "psionicvision";
	protected override string DefaultVerb => "masquerade";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psionicvision", (power, world) => new MentalVisionPower(power, world));
		MagicPowerFactory.RegisterBuilderLoader("psionicvision", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new MentalVisionPower(world, school, name, trait)));
	}
	private MentalVisionPower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name, trait)
	{
		Blurb = "Project a private sensory vision into a target's mind";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} MASQUERADE <target> <vision> to project a private mental image.";
		PowerDistance = MagicPowerDistance.SamePlaneOnly;
		FailEcho = PsionicPowerEmotes.Get("masquerade", "FailEcho", "You cannot seem to send the vision into that mind.");
		SuccessEcho = PsionicPowerEmotes.Get("masquerade", "SuccessEcho", "You send a vision into the target's mind.");
		DoDatabaseInsert();
	}
	private MentalVisionPower(MagicPower model, IFuturemud world) : base(model, world) { }
	protected override XElement SaveDefinition() => SaveTargetedDefinition();
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose mind do you want to show a vision?", out var target) || target is null) return;
		if (command.IsFinished) { actor.OutputHandler.Send("What vision do you want to project?"); return; }
		var vision = command.SafeRemainingArgument.Sanitise().NormaliseSpacing().ProperSentences().Fullstop();
		if (string.IsNullOrWhiteSpace(vision))
		{
			actor.OutputHandler.Send("The vision needs readable text."); return;
		}
		if (!PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} refuses involuntary mental traffic."); return;
		}
		var outcome = CheckPower(actor, target, CheckType.MindSayPower);
		if (outcome < MinimumSuccessThreshold) { SendFailure(actor, target); ConsumePowerCosts(actor, Verb); return; }
		SendSuccess(actor, target);
		target.OutputHandler.Send($"A vivid image unfolds in your mind: {vision}");
		PsionicActivityNotifier.Notify(actor, this, "a private mental vision", target);
		ConsumePowerCosts(actor, Verb);
	}
}
