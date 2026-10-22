#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp.Magic.Powers;

/// <summary>Mindwipe applies source-derived reserve losses and a typed timed impairment.</summary>
public sealed class PsionicMindwipePower : PsionicTargetedPowerBase
{
	public override string PowerType => "Mindwipe";
	public override string DatabaseType => "psionicmindwipe";
	protected override string DefaultVerb => "mindwipe";
	public double StaminaFraction { get; private set; }
	public double StunFraction { get; private set; }

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psionicmindwipe", (power, world) => new PsionicMindwipePower(power, world));
		MagicPowerFactory.RegisterBuilderLoader("psionicmindwipe", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicMindwipePower(world, school, name, trait)));
	}

	private PsionicMindwipePower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait)
		: base(world, school, name, trait)
	{
		Blurb = "Scramble a target's thoughts and impair their mental actions";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} MINDWIPE <target> to impair a target's mind for 10 to 80 minutes.";
		PowerDistance = MagicPowerDistance.SamePlaneOnly;
		SkillCheckDifficulty = Difficulty.Normal;
		StaminaFraction = 0.25;
		StunFraction = 0.25;
		FailEcho = PsionicPowerEmotes.Get("mindwipe", "FailEcho", "You attempt to wipe the target's mind, but it resists.");
		SuccessEcho = PsionicPowerEmotes.Get("mindwipe", "SuccessEcho", "You scramble the target's thoughts.");
		DoDatabaseInsert();
	}

	private PsionicMindwipePower(MagicPower model, IFuturemud world) : base(model, world)
	{
		var root = XElement.Parse(model.Definition);
		StaminaFraction = ParseFraction(root.Element("StaminaFraction")?.Value ?? "0.25", "stamina");
		StunFraction = ParseFraction(root.Element("StunFraction")?.Value ?? "0.25", "stun");
	}

	protected override XElement SaveDefinition() => SaveTargetedDefinition(
		new XElement("StaminaFraction", StaminaFraction), new XElement("StunFraction", StunFraction));

	protected override string SubtypeHelpText => $"{base.SubtypeHelpText}\n\t#3staminafraction <0-1 or %>#0 - fraction of target maximum stamina drained\n\t#3stunfraction <0-1 or %>#0 - fraction of target maximum stun damaged";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopForSwitch();
		if (field is "staminafraction" or "stamina") return SetFraction(actor, command, true);
		if (field is "stunfraction" or "stun") return SetFraction(actor, command, false);
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool SetFraction(ICharacter actor, StringStack command, bool stamina)
	{
		var raw = command.SafeRemainingArgument;
		var percent = raw.EndsWith('%');
		if (!double.TryParse(raw.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
		{
			actor.OutputHandler.Send("Enter a fraction from 0 to 1 (or a percentage from 0% to 100%).");
			return false;
		}
		if (percent) value /= 100.0;
		if (value < 0.0 || value > 1.0)
		{
			actor.OutputHandler.Send("Enter a fraction from 0 to 1 (or a percentage from 0% to 100%).");
			return false;
		}
		if (stamina) StaminaFraction = value; else StunFraction = value;
		Changed = true;
		actor.OutputHandler.Send($"Mindwipe now uses {value.ToString("P2", actor).ColourValue()} of target maximum {(stamina ? "stamina" : "stun")}.");
		return true;
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose thoughts do you want to scramble?", out var target) || target is null) return;
		if (target == actor) { actor.OutputHandler.Send("You cannot scramble your own mind this way."); return; }
		if (!PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} refuses involuntary mental traffic.");
			return;
		}
		if (target.EffectsOfType<PsionicMindwipeEffect>().Any())
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is already mentally impaired.");
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.MindSayPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			ConsumePowerCosts(actor, Verb);
			return;
		}

		var staminaLoss = PsionicMindwipePolicy.FractionOfMaximum(target.MaximumStamina, StaminaFraction);
		var stunLoss = PsionicMindwipePolicy.FractionOfMaximum(target.HealthStrategy.MaxStun(target), StunFraction);
		if (!ConfiguredItemConsequence.Apply(target, new ItemConsequence(Stun: stunLoss, Stamina: staminaLoss)))
		{
			actor.OutputHandler.Send("The target's condition prevents that mental assault from taking hold.");
			ConsumePowerCosts(actor, Verb);
			return;
		}

		SendSuccess(actor, target);
		target.AddEffect(new PsionicMindwipeEffect(target, this), PsionicMindwipePolicy.Duration(actor.TraitValue(SkillCheckTrait)));
		target.OutputHandler.Send("A heavy weight settles over your mind, making it difficult to think clearly.");
		PsionicActivityNotifier.Notify(actor, this, "a mindwipe", target);
		ConsumePowerCosts(actor, Verb);
	}

	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Stamina loss: {StaminaFraction.ToString("P2", actor).ColourValue()} of maximum");
		sb.AppendLine($"Stun loss: {StunFraction.ToString("P2", actor).ColourValue()} of maximum");
		sb.AppendLine("Duration: 10 minutes per full ten skill points, minus one step, clamped to 10-80 minutes");
		sb.AppendLine("Mental action modifier: -25 to initiating and resisting psionic actions and eligible skill use");
	}

	private static double ParseFraction(string raw, string channel)
	{
		if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ||
		    !double.IsFinite(value) || value < 0.0 || value > 1.0)
			throw new ApplicationException($"Invalid mindwipe {channel} fraction.");
		return value;
	}
}
