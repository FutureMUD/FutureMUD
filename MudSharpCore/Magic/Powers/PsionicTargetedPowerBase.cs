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

public abstract class PsionicTargetedPowerBase : MagicPowerBase
{
	protected PsionicTargetedPowerBase(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		Verb = root.Element("Verb")?.Value ?? throw new ApplicationException($"Missing Verb in power {Id} ({Name}).");
		PowerDistance = Enum.Parse<MagicPowerDistance>(root.Element("PowerDistance")?.Value ?? nameof(MagicPowerDistance.AnyConnectedMindOrConnectedTo), true);
		SkillCheckDifficulty = (Difficulty)int.Parse(root.Element("SkillCheckDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		MinimumSuccessThreshold = (Outcome)int.Parse(root.Element("MinimumSuccessThreshold")?.Value ?? ((int)Outcome.MinorPass).ToString());
		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(root.Element("SkillCheckTrait")?.Value ?? "0")) ??
		                  throw new ApplicationException($"Invalid SkillCheckTrait in power {Id} ({Name}).");
		FailEcho = root.Element("FailEcho")?.Value ?? "You cannot quite force the thought into shape.";
		SuccessEcho = root.Element("SuccessEcho")?.Value ?? string.Empty;
		DetectableWithDetectMagic = (Difficulty)int.Parse(root.Element("DetectableWithDetectMagic")?.Value ?? ((int)Difficulty.Normal).ToString());
	}

	protected PsionicTargetedPowerBase(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) :
		base(gameworld, school, name)
	{
		IsPsionic = true;
		Verb = DefaultVerb;
		PowerDistance = MagicPowerDistance.AnyConnectedMindOrConnectedTo;
		SkillCheckTrait = trait;
		SkillCheckDifficulty = Difficulty.Normal;
		MinimumSuccessThreshold = Outcome.MinorPass;
		FailEcho = "You cannot quite force the thought into shape.";
		SuccessEcho = string.Empty;
		DetectableWithDetectMagic = Difficulty.Normal;
	}

	protected abstract string DefaultVerb { get; }
	public string Verb { get; protected set; }
	public MagicPowerDistance PowerDistance { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public string FailEcho { get; protected set; }
	public string SuccessEcho { get; protected set; }
	public Difficulty DetectableWithDetectMagic { get; protected set; }
	public override IEnumerable<string> Verbs => [Verb];

	protected XElement SaveTargetedDefinition(params object[] additional)
	{
		var definition = new XElement("Definition",
			new XElement("Verb", Verb),
			new XElement("PowerDistance", PowerDistance),
			new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id),
			new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
			new XElement("DetectableWithDetectMagic", (int)DetectableWithDetectMagic),
			new XElement("FailEcho", new XCData(FailEcho)),
			new XElement("SuccessEcho", new XCData(SuccessEcho))
		);
		foreach (var item in additional)
		{
			definition.Add(item);
		}

		AddBaseDefinition(definition);
		return definition;
	}

	protected bool TryPrepareTarget(ICharacter actor, StringStack command, string missingTargetEcho,
		out ICharacter? target)
	{
		target = null;
		var (truth, missing) = CanAffordToInvokePower(actor, Verb);
		if (!truth)
		{
			actor.OutputHandler.Send($"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return false;
		}

		if (!HandleGeneralUseRestrictions(actor))
		{
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(missingTargetEcho);
			return false;
		}

		var targetText = command.PopSpeech();
		target = targetText.EqualToAny("me", "self")
			? actor
			: AcquireTarget(actor, targetText, PowerDistance);
		if (target is null)
		{
			actor.OutputHandler.Send("You cannot find any eligible mind by that description.");
			return false;
		}

		if (CanInvokePowerProg.ExecuteBool(actor, target) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor, target)?.ToString() ??
			                         $"You cannot use that power on {target.HowSeen(actor)}.");
			return false;
		}

		return true;
	}

	protected CheckOutcome CheckPower(ICharacter actor, ICharacter target, CheckType type)
	{
		return Gameworld.GetCheck(type).Check(actor, SkillCheckDifficulty, SkillCheckTrait, target);
	}

	protected void SendFailure(ICharacter actor, ICharacter target)
	{
		actor.OutputHandler.Send(new EmoteOutput(new Emote(FailEcho, actor, actor, target)));
	}

	public bool TargetFilter(ICharacter owner, ICharacter target)
	{
		return TargetIsValid(owner, target);
	}

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Verb: {Verb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Power Distance: {PowerDistance.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Detect Difficulty: {DetectableWithDetectMagic.Describe().ColourValue()}");
		sb.AppendLine($"Fail Echo: {FailEcho.ColourCommand()}");
		sb.AppendLine($"Success Echo: {(string.IsNullOrWhiteSpace(SuccessEcho) ? "None".ColourError() : SuccessEcho.ColourCommand())}");
		ShowSubtypeDetails(actor, sb);
	}

	protected virtual void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
	}

	protected override string SubtypeHelpText => @"	#3verb <verb>#0 - sets the command verb
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <difficulty>#0 - sets the skill check difficulty
	#3threshold <outcome>#0 - sets the minimum success threshold
	#3distance <distance>#0 - sets the target distance policy
	#3detect <difficulty>#0 - sets how hard this power's effects are to detect
	#3failecho <emote>#0 - sets the failure echo
	#3successecho <emote|none>#0 - sets an optional success echo";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "verb":
				return BuildingCommandVerb(actor, command);
			case "skill":
			case "trait":
				return BuildingCommandSkill(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "threshold":
				return BuildingCommandThreshold(actor, command);
			case "distance":
				return BuildingCommandDistance(actor, command);
			case "detect":
				return BuildingCommandDetect(actor, command);
			case "failecho":
				return BuildingCommandFailEcho(actor, command);
			case "successecho":
				return BuildingCommandSuccessEcho(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should activate this power?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();
		var costs = InvocationCosts[Verb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(Verb);
		Verb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This power now uses the verb {Verb.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill or trait should be used?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send("That is not a valid skill or trait.");
			return false;
		}

		SkillCheckTrait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This power now checks {trait.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"Valid difficulties are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		SkillCheckDifficulty = value;
		Changed = true;
		actor.OutputHandler.Send($"This power now checks at {value.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out Outcome value))
		{
			actor.OutputHandler.Send($"That is not a valid outcome. See {"show outcomes".MXPSend("show outcomes")}.");
			return false;
		}

		MinimumSuccessThreshold = value;
		Changed = true;
		actor.OutputHandler.Send($"This power now requires at least {value.DescribeColour()}.");
		return true;
	}

	private bool BuildingCommandDistance(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out MagicPowerDistance value))
		{
			actor.OutputHandler.Send($"Valid distances are {Enum.GetValues<MagicPowerDistance>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		PowerDistance = value;
		Changed = true;
		actor.OutputHandler.Send($"This power can now target {value.LongDescription().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDetect(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"Valid difficulties are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		DetectableWithDetectMagic = value;
		Changed = true;
		actor.OutputHandler.Send($"Effects from this power are now detected at {value.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandFailEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote should be shown on failure?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FailEcho = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The failure echo is now {FailEcho.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSuccessEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.SafeRemainingArgument.EqualToAny("none", "clear", "delete"))
		{
			SuccessEcho = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This power no longer has a success echo.");
			return true;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		SuccessEcho = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The success echo is now {SuccessEcho.ColourCommand()}.");
		return true;
	}
}

