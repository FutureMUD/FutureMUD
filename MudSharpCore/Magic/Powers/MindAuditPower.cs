﻿using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.FutureProg;
using System.Xml.Linq;
using MudSharp.PerceptionEngine;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.FutureProg.Statements;

namespace MudSharp.Magic.Powers;

public class MindAuditPower : MagicPowerBase
{
	public override string PowerType => "Audit";
	public override string DatabaseType => "mindaudit";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindaudit", (power, gameworld) => new MindAuditPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("mindaudit", (gameworld, school, name, actor, command) => {
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which skill do you want to use for the skill check?");
				return null;
			}

			var skill = gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
			if (skill is null)
			{
				actor.OutputHandler.Send("There is no such skill or attribute.");
				return null;
			}

			return new MindAuditPower(gameworld, school, name, skill);
		});
	}

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
                var definition = new XElement("Definition",
                        new XElement("Verb", Verb),
			new XElement("EmoteText", new XCData(EmoteText)),
			new XElement("EmoteTextSelf", new XCData(EmoteTextSelf)),
			new XElement("EchoToDetectedTarget", new XCData(EchoToDetectedTarget)),
			new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold), 
			new XElement("SkillCheckDifficultyProg", SkillCheckDifficultyProg.Id),
			new XElement("ShouldEchoDetectionProg", ShouldEchoDetectionProg.Id),
                        new XElement("SkillCheckTrait", SkillCheckTrait.Id)
                );
                AddBaseDefinition(definition);
                return definition;
        }

	private MindAuditPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name)
	{
		Blurb = "Check for foreign presences in your mind";
		_showHelpText = $"You can use {school.SchoolVerb.ToUpperInvariant()} AUDIT command to check for foreign presences in your mind.";
		Verb = "audit";
		SkillCheckTrait = trait;
		SkillCheckDifficultyProg = Gameworld.AlwaysZeroProg;
		MinimumSuccessThreshold = Outcome.MinorPass;
		EmoteText = "@ close|closes &0's eyes for a brief moment and looks deep in concentration.";
		EmoteTextSelf = "You close your eyes and search your mind for foreign presences.";
		EchoToDetectedTarget = "You feel as if your presence in $0's mind has been detected.";
		ShouldEchoDetectionProg = Gameworld.AlwaysFalseProg;
		DoDatabaseInsert();
	}

	protected MindAuditPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("Verb");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a Verb element.");
		}

		Verb = element.Value.ToLowerInvariant();

		element = root.Element("EmoteText");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a EmoteText element.");
		}

		EmoteText = element.Value.ToLowerInvariant();

		element = root.Element("EmoteTextSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a EmoteTextSelf element.");
		}

		EmoteTextSelf = element.Value.ToLowerInvariant();

		element = root.Element("EchoToDetectedTarget");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a EchoToDetectedTarget element.");
		}

		EchoToDetectedTarget = element.Value.ToLowerInvariant();

		element = root.Element("MinimumSuccessThreshold");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindAuditPower #{Id} ({Name}) was missing a MinimumSuccessThreshold element.");
		}

		if (!int.TryParse(element.Value, out var ivalue))
		{
			if (!CheckExtensions.GetOutcome(element.Value, out var outcome))
			{
				throw new ApplicationException(
					$"The MindAuditPower #{Id} ({Name}) had a MinimumSuccessThreshold value that did not map to a valid Outcome.");
			}

			MinimumSuccessThreshold = outcome;
		}
		else
		{
			MinimumSuccessThreshold = (Outcome)ivalue;
		}

		element = root.Element("SkillCheckTrait");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a SkillCheckTrait element.");
		}

		var trait = long.TryParse(element.Value, out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(element.Value);

		SkillCheckTrait = trait ?? throw new ApplicationException(
			$"The MindAuditPower #{Id} ({Name}) had a SkillCheckTrait element that pointed to a null Trait.");

		element = root.Element("SkillCheckDifficultyProg");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a SkillCheckDifficultyProg element.");
		}
		SkillCheckDifficultyProg = Gameworld.FutureProgs.GetByIdOrName(element.Value);

		element = root.Element("ShouldEchoDetectionProg");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a ShouldEchoDetectionProg element.");
		}
		ShouldEchoDetectionProg = Gameworld.FutureProgs.GetByIdOrName(element.Value);
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, verb);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		actor.OutputHandler.Send(EmoteTextSelf);
		if (!string.IsNullOrEmpty(EmoteText))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor), flags: OutputFlags.SuppressSource));
		}

		var check = Gameworld.GetCheck(CheckType.MindAuditPower);
		var results = check.CheckAgainstAllDifficulties(actor, Difficulty.Normal, SkillCheckTrait);
		var sb = new StringBuilder();
		foreach (var effect in actor.EffectsOfType<MindConnectedToEffect>())
		{
			var difficultyText = SkillCheckDifficultyProg.Execute<string>(effect.OriginatorCharacter, actor);
			if (!difficultyText.TryParseEnum<Difficulty>(out var difficulty))
			{
				difficulty = Difficulty.Normal;
			}

			if (results[difficulty].Outcome < MinimumSuccessThreshold)
			{
				continue;
			}

			sb.AppendLine($"You detect the presence of {effect.OriginatorCharacter.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreDisguises)} in your mind.");

			if (!string.IsNullOrEmpty(EchoToDetectedTarget) && ShouldEchoDetectionProg.Execute<bool?>(effect.OriginatorCharacter, actor) == true)
			{
				effect.OriginatorCharacter.OutputHandler.Send(new EmoteOutput(new Emote(EchoToDetectedTarget, actor, actor)));
			}
		}

		if (sb.Length == 0)
		{
			sb.AppendLine($"You don't detect any foreign presences in your mind.");
		}

		actor.OutputHandler.Send(sb.ToString());
		ConsumePowerCosts(actor, Verb);
	}

	public override IEnumerable<string> Verbs => new[]
	{
		Verb
	};

	public string Verb { get; protected set; }
	public IFutureProg SkillCheckDifficultyProg { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public IFutureProg ShouldEchoDetectionProg { get; protected set; }
	public string EmoteText { get; protected set; }
	public string EmoteTextSelf { get; protected set; }
	public string EchoToDetectedTarget { get; protected set; }

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Power Verb: {Verb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty Prog: {SkillCheckDifficultyProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Should Detection Echo Prog: {ShouldEchoDetectionProg.MXPClickableFunctionName()}");
		sb.AppendLine();
		sb.AppendLine("Emotes:");
		sb.AppendLine();
		sb.AppendLine($"Emote: {EmoteText.ColourCommand()}");
		sb.AppendLine($"Self Emote: {EmoteTextSelf.ColourCommand()}");
		sb.AppendLine($"Detected Target Emote: {EchoToDetectedTarget.ColourCommand()}");
	}

	#region Building Commands
	/// <inheritdoc />
	protected override string SubtypeHelpText => @"	#3verb <verb>#0 - sets the verb to activate this power
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <prog>#0 - sets the difficulty prog of the skill check
	#3threshold <outcome>#0 - sets the minimum outcome for skill success
	#3distance <distance>#0 - sets the distance that this power can be used at
	#3emote <echo>#0 - sets the emote for using the power
	#3emoteself <echo>#0 - sets the self echo for using this power
	#3emotetarget <echo>#0 - sets the echo sent to a detected target ($0 = auditor)";

	/// <inheritdoc />
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
			case "shouldecho":
			case "shouldechoprog":
			case "echoprog":
				return BuildingCommandShouldEchoProg(actor, command);
			case "emote":
				return BuildingCommandEmote(actor, command);
			case "emoteself":
			case "selfemote":
			case "self":
				return BuildingCommandEmoteSelf(actor, command);
			case "echotarget":
			case "emotetarget":
				return BuildingCommandEchoTarget(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}



	#region Building Subcommands
	private bool BuildingCommandEchoTarget(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the echo to detected targets to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EchoToDetectedTarget = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The echo to detected targets for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEmoteSelf(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the echo to self to?");
			return false;
		}

		EmoteTextSelf = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The echo to self for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the non-self echo to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The non-self echo for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandShouldEchoProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use to control whether the target gets an echo about being detected?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, [
			[ProgVariableTypes.Character],
			[ProgVariableTypes.Character, ProgVariableTypes.Character]
		]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ShouldEchoDetectionProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This power will now use the {prog.MXPClickableFunctionName()} prog to control whether the target gets an echo about being detected.");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What is the minimum success threshold for this power to work? See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Outcome value))
		{
			actor.OutputHandler.Send($"That is not a valid outcome. See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
			return false;
		}

		MinimumSuccessThreshold = value;
		Changed = true;
		actor.OutputHandler.Send($"The power user will now need to achieve a {value.DescribeColour()} in order to activate this power.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What prog should be used to determine the difficulty of the skill check?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Text,
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.Character]
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		SkillCheckDifficultyProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This power's skill check will now be at a difficulty determined by the prog {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill or trait should be used for this power's skill check?");
			return false;
		}

		var skill = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (skill is null)
		{
			actor.OutputHandler.Send("That is not a valid skill or trait.");
			return false;
		}

		SkillCheckTrait = skill;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the {skill.Name.ColourName()} skill for its skill check.");
		return true;
	}

	private bool BuildingCommandVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should be used to end this power when active?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();

		var costs = InvocationCosts[Verb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(Verb);
		Verb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to invoke the power.");
		return true;
	}
	#endregion Building Subcommands
	#endregion Building Commands
}
