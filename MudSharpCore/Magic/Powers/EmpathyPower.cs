#nullable enable

using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public sealed class EmpathyPower : PsionicTargetedPowerBase
{
	public override string PowerType => "Empathy";
	public override string DatabaseType => "empathy";
	protected override string DefaultVerb => "empathy";

	public TimeSpan TransferInterval { get; private set; }
	public int MaxWounds { get; private set; }
	public double SafetyHealthPercent { get; private set; }
	public string StartEcho { get; private set; }
	public string TransferEcho { get; private set; }
	public string StopEcho { get; private set; }
	public string SafetyEcho { get; private set; }

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("empathy", (power, gameworld) => new EmpathyPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("empathy", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new EmpathyPower(gameworld, school, name, trait));
	}

	private EmpathyPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) :
		base(gameworld, school, name, trait)
	{
		Blurb = "Take wounds from another person into yourself";
		_showHelpText =
			$"Use {school.SchoolVerb.ToUpperInvariant()} EMPATHY <target> to transfer wounds from the target to yourself one at a time.";
		PowerDistance = MagicPowerDistance.SameLocationOnly;
		SkillCheckDifficulty = Difficulty.Normal;
		TransferInterval = TimeSpan.FromSeconds(10);
		MaxWounds = 0;
		SafetyHealthPercent = 0.75;
		FailEcho = "You reach for $1's pain, but cannot take hold of it.";
		SuccessEcho = string.Empty;
		StartEcho = "@ reach|reaches toward $1's pain with intense psychic focus.";
		TransferEcho = "@ shudder|shudders as a wound passes from $1 into &0.";
		StopEcho = "Your empathic link to $1 fades.";
		SafetyEcho = "You recoil from taking another wound before the pain overwhelms you.";
		DoDatabaseInsert();
	}

	private EmpathyPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		TransferInterval = TimeSpan.FromSeconds(double.Parse(root.Element("TransferIntervalSeconds")?.Value ?? "10"));
		MaxWounds = int.Parse(root.Element("MaxWounds")?.Value ?? "0");
		SafetyHealthPercent = double.Parse(root.Element("SafetyHealthPercent")?.Value ?? "0.75");
		StartEcho = root.Element("StartEcho")?.Value ?? "@ reach|reaches toward $1's pain with intense psychic focus.";
		TransferEcho = root.Element("TransferEcho")?.Value ?? "@ shudder|shudders as a wound passes from $1 into &0.";
		StopEcho = root.Element("StopEcho")?.Value ?? "Your empathic link to $1 fades.";
		SafetyEcho = root.Element("SafetyEcho")?.Value ??
		             "You recoil from taking another wound before the pain overwhelms you.";
	}

	protected override XElement SaveDefinition()
	{
		return SaveTargetedDefinition(
			new XElement("TransferIntervalSeconds", TransferInterval.TotalSeconds),
			new XElement("MaxWounds", MaxWounds),
			new XElement("SafetyHealthPercent", SafetyHealthPercent),
			new XElement("StartEcho", new XCData(StartEcho)),
			new XElement("TransferEcho", new XCData(TransferEcho)),
			new XElement("StopEcho", new XCData(StopEcho)),
			new XElement("SafetyEcho", new XCData(SafetyEcho))
		);
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose wounds do you want to take?", out var target) || target is null)
		{
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You cannot empathically transfer your own wounds to yourself.");
			return;
		}

		var candidateCount = target.Wounds.Count();
		if (candidateCount == 0)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} has no wounds for you to take.");
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.EmpathyPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			return;
		}

		var transferCount = MaxWounds <= 0 ? candidateCount : Math.Min(MaxWounds, candidateCount);
		var actions = Enumerable.Range(0, transferCount)
		                        .Select(_ => (Action<IPerceivable>)(_ => TransferOneWound(actor, target)))
		                        .ToList();
		actor.AddEffect(new StagedCharacterActionWithTarget(
				actor,
				target,
				"transferring wounds empathically",
				StopEcho,
				"You cannot move while maintaining the empathic link.",
				["general", "movement"],
				"transferring wounds empathically",
				actions,
				transferCount,
				TransferInterval,
				_ => actor.OutputHandler.Send(new EmoteOutput(new Emote(StopEcho, actor, actor, target)))
			),
			TransferInterval);

		if (!string.IsNullOrWhiteSpace(StartEcho))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(StartEcho, actor, actor, target)));
		}

		PsionicActivityNotifier.Notify(actor, this, "an empathic wound transfer");
		ConsumePowerCosts(actor, Verb);
	}

	private void TransferOneWound(ICharacter actor, ICharacter target)
	{
		if (actor.State.IsDead() || target.State.IsDead() || actor.Location != target.Location)
		{
			actor.RemoveAllEffects<StagedCharacterActionWithTarget>(x => x.Target == target, true);
			return;
		}

		var wound = target.Wounds
		                  .Where(x => x.Severity > WoundSeverity.None)
		                  .OrderByDescending(x => x.Severity)
		                  .ThenByDescending(x => x.CurrentDamage)
		                  .FirstOrDefault();
		if (wound is null)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} has no more wounds for you to take.");
			actor.RemoveAllEffects<StagedCharacterActionWithTarget>(x => x.Target == target, true);
			return;
		}

		if (!PassesSafetyThreshold(actor, wound))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(SafetyEcho, actor, actor, target)));
			actor.RemoveAllEffects<StagedCharacterActionWithTarget>(x => x.Target == target, true);
			return;
		}

		if (!BodypartMappingUtilities.TryMapWound(actor.Body, wound, out var targetPart, out var targetSeveredPart,
			    out var whyNot) || targetPart is null)
		{
			actor.OutputHandler.Send(whyNot);
			actor.RemoveAllEffects<StagedCharacterActionWithTarget>(x => x.Target == target, true);
			return;
		}

		if (!target.TryTransferWoundTo(wound, actor, targetPart, targetSeveredPart))
		{
			actor.OutputHandler.Send("The wound slips out of your empathic grasp.");
			actor.RemoveAllEffects<StagedCharacterActionWithTarget>(x => x.Target == target, true);
			return;
		}

		if (!string.IsNullOrWhiteSpace(TransferEcho))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(TransferEcho, actor, actor, target)));
		}
	}

	private bool PassesSafetyThreshold(ICharacter actor, IWound wound)
	{
		if (SafetyHealthPercent <= 0.0)
		{
			return true;
		}

		var maxHp = actor.HealthStrategy.MaxHP(actor);
		if (maxHp <= 0.0)
		{
			return true;
		}

		var projected = actor.Wounds.Sum(x => x.CurrentDamage) + wound.CurrentDamage;
		return projected / maxHp <= SafetyHealthPercent;
	}

	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Transfer Interval: {TransferInterval.Describe(actor).ColourValue()}");
		sb.AppendLine($"Max Wounds: {(MaxWounds <= 0 ? "All".ColourValue() : MaxWounds.ToString("N0", actor).ColourValue())}");
		sb.AppendLine($"Safety Health Percent: {SafetyHealthPercent.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Start Echo: {StartEcho.ColourCommand()}");
		sb.AppendLine($"Transfer Echo: {TransferEcho.ColourCommand()}");
		sb.AppendLine($"Stop Echo: {StopEcho.ColourCommand()}");
		sb.AppendLine($"Safety Echo: {SafetyEcho.ColourCommand()}");
	}

	protected override string SubtypeHelpText => $@"{base.SubtypeHelpText}
	#3interval <seconds>#0 - sets time between wound transfers
	#3max <number|0>#0 - sets maximum wounds, or 0 for all current wounds
	#3safety <percent>#0 - sets max projected HP damage percentage, or 0 to disable
	#3startecho <emote>#0 - sets the starting emote
	#3transferecho <emote>#0 - sets each wound-transfer emote
	#3stopecho <emote>#0 - sets cancellation/completion emote
	#3safetyecho <emote>#0 - sets safety-stop emote";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "interval":
				return BuildingCommandInterval(actor, command);
			case "max":
			case "maxwounds":
				return BuildingCommandMax(actor, command);
			case "safety":
				return BuildingCommandSafety(actor, command);
			case "startecho":
				return BuildingCommandEcho(actor, command, "start");
			case "transferecho":
				return BuildingCommandEcho(actor, command, "transfer");
			case "stopecho":
				return BuildingCommandEcho(actor, command, "stop");
			case "safetyecho":
				return BuildingCommandEcho(actor, command, "safety");
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandInterval(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var seconds) || seconds <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive number of seconds.");
			return false;
		}

		TransferInterval = TimeSpan.FromSeconds(seconds);
		Changed = true;
		actor.OutputHandler.Send($"Empathy now transfers wounds every {TransferInterval.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMax(ICharacter actor, StringStack command)
	{
		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send("You must enter a non-negative number of wounds, or 0 for all.");
			return false;
		}

		MaxWounds = value;
		Changed = true;
		actor.OutputHandler.Send(MaxWounds == 0
			? "Empathy will now transfer all current wounds."
			: $"Empathy will now transfer at most {MaxWounds.ToString("N0", actor).ColourValue()} wounds.");
		return true;
	}

	private bool BuildingCommandSafety(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument.TrimEnd('%'), out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a non-negative percentage, or 0 to disable safety.");
			return false;
		}

		SafetyHealthPercent = value > 1.0 ? value / 100.0 : value;
		Changed = true;
		actor.OutputHandler.Send(SafetyHealthPercent <= 0.0
			? "Empathy safety threshold is disabled."
			: $"Empathy stops at {SafetyHealthPercent.ToString("P2", actor).ColourValue()} projected HP damage.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command, string which)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote should be used?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		switch (which)
		{
			case "start":
				StartEcho = command.SafeRemainingArgument;
				break;
			case "transfer":
				TransferEcho = command.SafeRemainingArgument;
				break;
			case "stop":
				StopEcho = command.SafeRemainingArgument;
				break;
			case "safety":
				SafetyEcho = command.SafeRemainingArgument;
				break;
		}

		Changed = true;
		actor.OutputHandler.Send($"The {which} echo is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
}
