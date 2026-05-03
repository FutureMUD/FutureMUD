#nullable enable

using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public sealed class CoercePower : PsionicTargetedPowerBase
{
	public override string PowerType => "Coerce";
	public override string DatabaseType => "coerce";
	protected override string DefaultVerb => "coerce";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("coerce", (power, gameworld) => new CoercePower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("coerce", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new CoercePower(gameworld, school, name, trait));
	}

	private CoercePower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name, trait)
	{
		Blurb = "Influence stamina, hunger, thirst, or thoughts";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} COERCE <target> <fatigue|refresh|thirst|quench|hunger|full|thought> [thought] to influence a body or mind.";
		StaminaFractionPerDegree = 0.05;
		NeedHoursPerDegree = 2.0;
		FailEcho = "You press against $1's body and mind, but cannot make it answer.";
		DoDatabaseInsert();
	}

	private CoercePower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		StaminaFractionPerDegree = double.Parse(root.Element("StaminaFractionPerDegree")?.Value ?? "0.05");
		NeedHoursPerDegree = double.Parse(root.Element("NeedHoursPerDegree")?.Value ?? "2.0");
	}

	public double StaminaFractionPerDegree { get; private set; }
	public double NeedHoursPerDegree { get; private set; }

	protected override XElement SaveDefinition()
	{
		return SaveTargetedDefinition(
			new XElement("StaminaFractionPerDegree", StaminaFractionPerDegree),
			new XElement("NeedHoursPerDegree", NeedHoursPerDegree)
		);
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose body or mind do you want to coerce?", out var target) || target is null)
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which mode do you want to use: fatigue, refresh, thirst, quench, hunger, full, or thought?");
			return;
		}

		if (!command.PopSpeech().TryParseEnum(out PsionicCoerceMode mode))
		{
			actor.OutputHandler.Send("That is not a valid coerce mode. Use fatigue, refresh, thirst, quench, hunger, full, or thought.");
			return;
		}

		if (!PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} refuses involuntary mental traffic.");
			return;
		}

		if (mode == PsionicCoerceMode.Thought && command.IsFinished)
		{
			actor.OutputHandler.Send("What thought do you want to coerce?");
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.MindSayPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			return;
		}

		var degrees = Math.Max(1, outcome.SuccessDegrees());
		switch (mode)
		{
			case PsionicCoerceMode.Fatigue:
				ApplyStamina(actor, target, -target.MaximumStamina * StaminaFractionPerDegree * degrees, "fatigue");
				break;
			case PsionicCoerceMode.Refresh:
				ApplyStamina(actor, target, target.MaximumStamina * StaminaFractionPerDegree * degrees, "refresh");
				break;
			case PsionicCoerceMode.Thirst:
				ApplyNeeds(actor, target, 0.0, -NeedHoursPerDegree * degrees, "thirst");
				break;
			case PsionicCoerceMode.Quench:
				ApplyNeeds(actor, target, 0.0, NeedHoursPerDegree * degrees, "quench");
				break;
			case PsionicCoerceMode.Hunger:
				ApplyNeeds(actor, target, -NeedHoursPerDegree * degrees, 0.0, "hunger");
				break;
			case PsionicCoerceMode.Full:
				ApplyNeeds(actor, target, NeedHoursPerDegree * degrees, 0.0, "fullness");
				break;
			case PsionicCoerceMode.Thought:
				PsionicTrafficHelper.DeliverThought(actor, target, School, command.SafeRemainingArgument);
				break;
		}

		ConsumePowerCosts(actor, Verb);
	}

	private static void ApplyStamina(ICharacter actor, ICharacter target, double amount, string label)
	{
		if (amount < 0)
		{
			target.SpendStamina(Math.Abs(amount));
		}
		else
		{
			target.GainStamina(amount);
		}

		actor.OutputHandler.Send($"You coerce {label} through {target.HowSeen(actor, true)}.");
		target.OutputHandler.Send($"A psionic pressure forces {label} through your body.");
		PsionicTrafficHelper.Audit(actor, target, $"coerced {label} in", amount.ToString("N2", actor));
	}

	private static void ApplyNeeds(ICharacter actor, ICharacter target, double hunger, double thirst, string label)
	{
		target.FulfilNeeds(new NeedFulfiller
		{
			SatiationPoints = hunger,
			ThirstPoints = thirst
		}, true);

		actor.OutputHandler.Send($"You coerce {label} through {target.HowSeen(actor, true)}.");
		target.OutputHandler.Send($"A psionic pressure forces {label} through your body.");
		PsionicTrafficHelper.Audit(actor, target, $"coerced {label} in", $"hunger {hunger:N2}, thirst {thirst:N2}");
	}

	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Stamina Fraction Per Degree: {StaminaFractionPerDegree.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Need Hours Per Degree: {NeedHoursPerDegree.ToString("N2", actor).ColourValue()}");
	}
}

