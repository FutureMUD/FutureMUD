#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public sealed class PsychicBoltPower : PsionicTargetedPowerBase
{
	public override string PowerType => "Psychic Bolt";
	public override string DatabaseType => "psychicbolt";
	protected override string DefaultVerb => "psychicbolt";

	public double StunAmount { get; private set; }
	public DamageType DamageType { get; private set; }
	public string TargetEcho { get; private set; }

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psychicbolt", (power, gameworld) => new PsychicBoltPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("psychicbolt", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new PsychicBoltPower(gameworld, school, name, trait));
	}

	private PsychicBoltPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) :
		base(gameworld, school, name, trait)
	{
		Blurb = "Strike a target's mind with stun-only psychic force";
		_showHelpText =
			$"Use {school.SchoolVerb.ToUpperInvariant()} PSYCHICBOLT <target> to deal stun-only psychic damage.";
		PowerDistance = MagicPowerDistance.AnyConnectedMindOrConnectedTo;
		SkillCheckDifficulty = Difficulty.Normal;
		StunAmount = 20.0;
		DamageType = DamageType.Eldritch;
		FailEcho = "You hurl force at $1's mind, but it scatters before impact.";
		SuccessEcho = "@ hurl|hurls a bolt of invisible psychic force at $1.";
		TargetEcho = "Invisible psychic force crashes through your mind.";
		DoDatabaseInsert();
	}

	private PsychicBoltPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		StunAmount = double.Parse(root.Element("StunAmount")?.Value ?? "20");
		DamageType = Enum.Parse<DamageType>(root.Element("DamageType")?.Value ?? nameof(DamageType.Eldritch), true);
		TargetEcho = root.Element("TargetEcho")?.Value ?? "Invisible psychic force crashes through your mind.";
	}

	protected override XElement SaveDefinition()
	{
		return SaveTargetedDefinition(
			new XElement("StunAmount", StunAmount),
			new XElement("DamageType", DamageType),
			new XElement("TargetEcho", new XCData(TargetEcho))
		);
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose mind do you want to strike?", out var target) || target is null)
		{
			return;
		}

		if (!PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} refuses involuntary mental traffic.");
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.PsychicBoltPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			return;
		}

		var stun = StunAmount * Math.Max(1, outcome.SuccessDegrees());
		var wounds = target.PassiveSufferDamage(new Damage
		{
			DamageType = DamageType,
			DamageAmount = 0.0,
			PainAmount = 0.0,
			ShockAmount = 0.0,
			StunAmount = stun,
			ActorOrigin = actor,
			PenetrationOutcome = outcome
		}).ToList();
		wounds.ProcessPassiveWounds();
		target.StartHealthTick();

		if (!string.IsNullOrWhiteSpace(SuccessEcho))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(SuccessEcho, actor, actor, target)));
		}

		if (!string.IsNullOrWhiteSpace(TargetEcho))
		{
			target.OutputHandler.Send(new EmoteOutput(new Emote(TargetEcho, target, actor, target)));
		}

		PsionicActivityNotifier.Notify(actor, this, "a bolt of hostile psychic force");
		ConsumePowerCosts(actor, Verb);
	}

	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Stun Amount: {StunAmount.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Damage Type: {DamageType.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Target Echo: {TargetEcho.ColourCommand()}");
	}

	protected override string SubtypeHelpText => $@"{base.SubtypeHelpText}
	#3stun <amount>#0 - sets the base stun-only damage
	#3damagetype <type>#0 - sets the damage type used by the health pipeline
	#3targetecho <emote|none>#0 - sets the target-only echo";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "stun":
			case "stunamount":
				return BuildingCommandStun(actor, command);
			case "damage":
			case "damagetype":
				return BuildingCommandDamageType(actor, command);
			case "targetecho":
				return BuildingCommandTargetEcho(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandStun(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive stun amount.");
			return false;
		}

		StunAmount = value;
		Changed = true;
		actor.OutputHandler.Send($"This power now deals {StunAmount.ToString("N2", actor).ColourValue()} base stun.");
		return true;
	}

	private bool BuildingCommandDamageType(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out DamageType value))
		{
			actor.OutputHandler.Send(
				$"Valid damage types are {Enum.GetValues<DamageType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		DamageType = value;
		Changed = true;
		actor.OutputHandler.Send($"This power now uses {DamageType.DescribeEnum().ColourValue()} damage.");
		return true;
	}

	private bool BuildingCommandTargetEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.SafeRemainingArgument.EqualToAny("none", "clear", "delete"))
		{
			TargetEcho = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This power no longer has a target echo.");
			return true;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		TargetEcho = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The target echo is now {TargetEcho.ColourCommand()}.");
		return true;
	}
}
