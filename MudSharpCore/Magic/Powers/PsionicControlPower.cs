#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

/// <summary>Resisted one-command control, limited to same-room mortal command execution.</summary>
public sealed class PsionicControlPower : PsionicTargetedPowerBase
{
	public override string PowerType => "Mental Control";
	public override string DatabaseType => "psioniccontrol";
	protected override string DefaultVerb => "control";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psioniccontrol", (power, world) => new PsionicControlPower(power, world));
		MagicPowerFactory.RegisterBuilderLoader("psioniccontrol", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicControlPower(world, school, name, trait)));
	}

	private PsionicControlPower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait)
		: base(world, school, name, trait)
	{
		Blurb = "Force one brief command through a target's mind";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} CONTROL <target> <command> to attempt one restricted command. Only same-room targets are eligible.";
		PowerDistance = MagicPowerDistance.SameLocationOnly;
		SkillCheckDifficulty = Difficulty.Normal;
		FailEcho = PsionicPowerEmotes.Get("control", "FailEcho", "You fail to seize control of the target's actions.");
		SuccessEcho = PsionicPowerEmotes.Get("control", "SuccessEcho", "You seize control of the target's actions for a moment.");
		DoDatabaseInsert();
	}

	private PsionicControlPower(MagicPower model, IFuturemud world) : base(model, world) { }
	protected override XElement SaveDefinition() => SaveTargetedDefinition();

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Whose actions do you want to control?");
			return;
		}

		var targetText = command.PopSpeech();
		var target = AcquireTarget(actor, targetText, PowerDistance);
		if (target is null || target == actor)
		{
			actor.OutputHandler.Send(target is null ? "You cannot find an eligible mind by that description." : "You cannot control your own actions.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What single command do you want the target to perform?");
			return;
		}

		var forcedCommand = command.RemainingArgument.TrimStart();
		if (forcedCommand.Any(x => x is ';' or '\n' or '\r') || PsionicTrafficHelper.IsBlockedCommandRoot(forcedCommand))
		{
			actor.OutputHandler.Send("That command cannot be imposed through mental control.");
			return;
		}

		if (target.Location != actor.Location || !PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.OutputHandler.Send("You cannot establish that control over the target.");
			return;
		}
		if (!CommandExecutionGuards.CanForceTarget(actor, target))
		{
			actor.OutputHandler.Send("You cannot impose control on that target.");
			return;
		}

		if (!CanAffordToInvokePower(actor, verb).Truth)
		{
			actor.Send("You cannot afford that mental control.");
			return;
		}
		if (!HandleGeneralUseRestrictions(actor) || CanInvokePower(actor, target) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor, target)?.ToString() ?? "You cannot use that power on the target.");
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.MindSayPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			ConsumePowerCosts(actor, Verb);
			return;
		}

		SendSuccess(actor, target);
		// Revalidate immediately before execution and rely on the shared forced-command guard,
		// which executes as a mortal and preserves the normal command restriction pipeline.
		if (target.Location == actor.Location && PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target) &&
			CommandExecutionGuards.CanForceTarget(actor, target) && !PsionicTrafficHelper.IsBlockedCommandRoot(forcedCommand))
		{
			CommandExecutionGuards.ExecuteForcedCommand(target, forcedCommand);
		}
		PsionicActivityNotifier.Notify(actor, this, "mental control", target);
		ConsumePowerCosts(actor, Verb);
	}
}
