#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp.Magic.Powers;

/// <summary>Mindblast uses the native health pipeline and target-relative quarter-maximum losses.</summary>
public sealed class PsionicMindblastPower : PsionicTargetedPowerBase
{
	public override string PowerType => "Mindblast";
	public override string DatabaseType => "psionicmindblast";
	protected override string DefaultVerb => "mindblast";
	public double HealthFraction { get; private set; }
	public double StunFraction { get; private set; }

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psionicmindblast", (power, world) => new PsionicMindblastPower(power, world));
		MagicPowerFactory.RegisterBuilderLoader("psionicmindblast", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicMindblastPower(world, school, name, trait)));
	}

	private PsionicMindblastPower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait)
		: base(world, school, name, trait)
	{
		Blurb = "Inflict a potentially lethal blast of psychic force";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} MINDBLAST <target> to attack a target's health and stun reserves.";
		PowerDistance = MagicPowerDistance.SamePlaneOnly;
		SkillCheckDifficulty = Difficulty.Normal;
		HealthFraction = 0.25;
		StunFraction = 0.25;
		FailEcho = PsionicPowerEmotes.Get("mindblast", "FailEcho", "You attempt to blast the target's mind, but it resists.");
		SuccessEcho = PsionicPowerEmotes.Get("mindblast", "SuccessEcho", "You focus your thoughts and lash out with concentrated mental force.");
		DoDatabaseInsert();
	}

	private PsionicMindblastPower(MagicPower model, IFuturemud world) : base(model, world)
	{
		var root = XElement.Parse(model.Definition);
		HealthFraction = ParseFraction(root.Element("HealthFraction")?.Value ?? "0.25", "health");
		StunFraction = ParseFraction(root.Element("StunFraction")?.Value ?? "0.25", "stun");
	}

	protected override XElement SaveDefinition() => SaveTargetedDefinition(
		new XElement("HealthFraction", HealthFraction), new XElement("StunFraction", StunFraction));
	protected override string SubtypeHelpText => $"{base.SubtypeHelpText}\n\t#3healthfraction <0-1>#0 - fraction of target maximum health damage\n\t#3stunfraction <0-1>#0 - fraction of target maximum stun damage";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopForSwitch();
		if (field is "healthfraction" or "health") return SetFraction(actor, command, true);
		if (field is "stunfraction" or "stun") return SetFraction(actor, command, false);
		return base.BuildingCommand(actor, command.GetUndo());
	}
	private bool SetFraction(ICharacter actor, StringStack command, bool health)
	{
		var raw = command.SafeRemainingArgument;
		var percent = raw.EndsWith('%');
		if (!double.TryParse(raw.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
		{
			actor.OutputHandler.Send("Enter a fraction from 0 to 1 (or a percentage from 0% to 100%)."); return false;
		}
		if (percent) value /= 100.0;
		if (value < 0.0 || value > 1.0)
		{
			actor.OutputHandler.Send("Enter a fraction from 0 to 1 (or a percentage from 0% to 100%)."); return false;
		}
		if (health) HealthFraction = value; else StunFraction = value;
		Changed = true;
		actor.OutputHandler.Send($"This power now deals {value.ToString("P2", actor).ColourValue()} of target maximum {(health ? "health" : "stun")}.");
		return true;
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose mind do you want to blast?", out var target) || target is null) return;
		if (target == actor) { actor.OutputHandler.Send("You cannot blast your own mind."); return; }
		if (!PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} refuses involuntary mental traffic.");
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.PsychicBoltPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			ConsumePowerCosts(actor, Verb);
			return;
		}

		var consequence = new ItemConsequence(
			HealthFraction: HealthFraction,
			Stun: PsionicMindwipePolicy.FractionOfMaximum(target.HealthStrategy.MaxStun(target), StunFraction));
		if (!ConfiguredItemConsequence.Apply(target, consequence))
		{
			actor.OutputHandler.Send("The target's condition prevents that mental assault from taking hold.");
			ConsumePowerCosts(actor, Verb);
			return;
		}
		SendSuccess(actor, target);
		target.OutputHandler.Send("Your body shudders as your consciousness is assaulted by a blast of mental energy.");
		PsionicActivityNotifier.Notify(actor, this, "a mindblast", target);
		ConsumePowerCosts(actor, Verb);
	}

	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Health fraction of target maximum: {HealthFraction.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Stun fraction of target maximum: {StunFraction.ToString("P2", actor).ColourValue()}");
	}

	private static double ParseFraction(string raw, string channel)
	{
		if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ||
		    !double.IsFinite(value) || value < 0.0 || value > 1.0)
			throw new ApplicationException($"Invalid mindblast {channel} fraction.");
		return value;
	}
}
