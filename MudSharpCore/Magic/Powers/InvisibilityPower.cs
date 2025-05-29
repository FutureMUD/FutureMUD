using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;

namespace MudSharp.Magic.Powers;

public class InvisibilityPower : SustainedMagicPower
{
	public override string PowerType => "Invisibility";
	public override string DatabaseType => "invisibility";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("invisibility", (power, gameworld) => new InvisibilityPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("invisibility", (gameworld, school, name, actor, command) => {
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

			return new InvisibilityPower(gameworld, school, name, skill);
		});
	}

	protected override XElement SaveDefinition()
	{
		var definition = new XElement("Definition",
			new XElement("ConnectVerb", BeginVerb),
			new XElement("DisconnectVerb", EndVerb),
			new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id),
			new XElement("InvisibilityAppliesProg", InvisibilityAppliesProg.Id),
			new XElement("CanEndPowerProg", CanEndPowerProg.Id),
			new XElement("WhyCantEndPowerProg", WhyCantEndPowerProg.Id),
			new XElement("EmoteText", new XCData(EmoteText)),
			new XElement("FailEmoteText", new XCData(FailEmoteText)),
			new XElement("EndPowerEmoteText", new XCData(EndPowerEmoteText)),
			new XElement("PerceptionTypes", (long)PerceptionTypes)
		);
		SaveSustainedDefinition(definition);
		return definition;
	}

	private InvisibilityPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("StartPowerVerb");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no StartPowerVerb in the definition XML for power {Id} ({Name}).");
		}

		BeginVerb = element.Value;

		element = root.Element("EndPowerVerb");
		if (element == null)
		{
			throw new ApplicationException($"There was no EndPowerVerb in the definition XML for power {Id} ({Name}).");
		}

		EndVerb = element.Value;

		element = root.Element("EmoteText");
		if (element == null)
		{
			throw new ApplicationException($"There was no EmoteText in the definition XML for power {Id} ({Name}).");
		}

		EmoteText = element.Value;

		element = root.Element("FailEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no FailEmoteText in the definition XML for power {Id} ({Name}).");
		}

		FailEmoteText = element.Value;

		element = root.Element("EndPowerEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EndPowerEmoteText in the definition XML for power {Id} ({Name}).");
		}

		EndPowerEmoteText = element.Value;

		element = root.Element("SkillCheckDifficulty");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SkillCheckDifficulty in the definition XML for power {Id} ({Name}).");
		}

		SkillCheckDifficulty = (Difficulty)int.Parse(element.Value);

		element = root.Element("SkillCheckTrait");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SkillCheckTrait in the definition XML for power {Id} ({Name}).");
		}

		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(element.Value));

		element = root.Element("MinimumSuccessThreshold");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no MinimumSuccessThreshold in the definition XML for power {Id} ({Name}).");
		}

		MinimumSuccessThreshold = (Outcome)int.Parse(element.Value);

		element = root.Element("InvisibilityAppliesProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no InvisibilityAppliesProg in the definition XML for power {Id} ({Name}).");
		}

		InvisibilityAppliesProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("CanEndPowerProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no CanEndPowerProg in the definition XML for power {Id} ({Name}).");
		}

		CanEndPowerProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("WhyCantEndPowerProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no WhyCantEndPowerProg in the definition XML for power {Id} ({Name}).");
		}

		WhyCantEndPowerProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("PerceptionTypes");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no PerceptionTypes in the definition XML for power {Id} ({Name}).");
		}

		PerceptionTypes = (PerceptionTypes)long.Parse(element.Value);
	}

	private InvisibilityPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name)
	{
		Blurb = "Become invisible to sight";
		_showHelpText = @$"You can use #3{school.SchoolVerb.ToUpperInvariant()} INVIS#0 to become invisible and #3{school.SchoolVerb.ToUpperInvariant()} VIS#0 to end this invisibility effect."; ;
		BeginVerb = "invis";
		EndVerb = "vis";
		SkillCheckTrait = trait;
		SkillCheckDifficulty = Difficulty.VeryEasy;
		MinimumSuccessThreshold = Outcome.Fail;
		ConcentrationPointsToSustain = 1.0;
		InvisibilityAppliesProg = Gameworld.AlwaysTrueProg;
		CanEndPowerProg = Gameworld.AlwaysTrueProg;
		WhyCantEndPowerProg = Gameworld.UniversalErrorTextProg;
		PerceptionTypes = PerceptionTypes.All;
		EmoteText = "@ rapidly fade|fades from view and become|becomes invisible.";
		FailEmoteText = "@ appear|appears to fade out of view momentarily but return|returns to normal.";
		EndPowerEmoteText = "@ appear|appears out of thin air.";
		DoDatabaseInsert();
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

		if (verb.EqualTo(EndVerb))
		{
			if (!actor.AffectedBy<MagicInvisibility>(this))
			{
				actor.OutputHandler.Send("You are not currently using that power.");
				return;
			}

			if (CanEndPowerProg?.Execute<bool?>(actor) == false)
			{
				actor.OutputHandler.Send(WhyCantEndPowerProg?.Execute<string>(actor) ?? "You can't do that right now.");
				return;
			}

			actor.RemoveAllEffects<MagicInvisibility>(x => x.InvisibilityPower == this, true);
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EndPowerEmoteText, actor, actor)));
			ConsumePowerCosts(actor, verb);
			return;
		}

		if (actor.AffectedBy<MagicInvisibility>(this))
		{
			actor.OutputHandler.Send("You are already using that power.");
			return;
		}

		if (CanInvokePowerProg?.Execute<bool?>(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg?.Execute<string>(actor) ?? "You cannot use that power.");
			return;
		}

		var check = Gameworld.GetCheck(CheckType.InvisibilityPower);
		var outcome = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait, null);
		if (outcome < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(FailEmoteText, actor, actor)));
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor)));
		actor.AddEffect(new MagicInvisibility(actor, this), GetDuration(outcome.SuccessDegrees()));
		ConsumePowerCosts(actor, verb);
	}

	#region Overrides of SustainedMagicPower

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		actor.RemoveAllEffects<MagicInvisibility>(x => x.InvisibilityPower == this, true);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(EndPowerEmoteText, actor, actor)));
	}

	#endregion

	public string EndVerb { get; protected set; }

	public string BeginVerb { get; protected set; }

	public override IEnumerable<string> Verbs => new[] { EndVerb, BeginVerb };

	public IFutureProg InvisibilityAppliesProg { get; protected set; }
	public IFutureProg CanEndPowerProg { get; protected set; }
	public IFutureProg WhyCantEndPowerProg { get; protected set; }

	public string EmoteText { get; protected set; }
	public string FailEmoteText { get; protected set; }
	public string EndPowerEmoteText { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public PerceptionTypes PerceptionTypes { get; protected set; }

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Begin Verb: {BeginVerb.ColourCommand()}");
		sb.AppendLine($"End Verb: {EndVerb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Invisibility Applies Prog: {InvisibilityAppliesProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Can End Prog: {CanEndPowerProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Why Can't End Prog: {WhyCantEndPowerProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Perception Types: {PerceptionTypes.GetSingleFlags().Select(x => x.DescribeEnum().ColourValue()).ListToString()}");
		sb.AppendLine();
		sb.AppendLine("Emotes:");
		sb.AppendLine();
		sb.AppendLine($"Emote: {EmoteText.ColourCommand()}");
		sb.AppendLine($"Fail Emote: {FailEmoteText.ColourCommand()}");
		sb.AppendLine($"End Emote: {EndPowerEmoteText.ColourCommand()}");
	}

	#region Building Commands
	/// <inheritdoc />
	protected override string SubtypeHelpText => @"	#3begin <verb>#0 - sets the verb to activate this power
	#3end <verb>#0 - sets the verb to end this power
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <difficulty>#0 - sets the difficulty of the skill check
	#3threshold <outcome>#0 - sets the minimum outcome for skill success
	#3applies <prog>#0 - sets the prog that controls whether invisibility applies
	#3canend <prog>#0 - sets the prog that controls whether the power can be voluntarily ended
	#3whycantend <prog>#0 - sets a prog that gives the error message for canend
	#3perception <type>#0 - toggle a perception type as being relevant
	#3emote <emote>#0 - sets the power use emote. $0 is the power user
	#3failemote <emote>#0 - sets the fail emote for power use. $0 is the power user
	#3endemote <emote>#0 - sets the emote for ending the power. $0 is the power user";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "failemote":
				return BuildingCommandFailEmote(actor, command);
			case "endemote":
				return BuildingCommandEndEmote(actor, command);
			case "emote":
				return BuildingCommandEmote(actor, command);
			case "perception":
				return BuildingCommandPerception(actor, command);
			case "whycantend":
			case "whycantendprog":
				return BuildingCommandWhyCantEndProg(actor, command);
			case "canend":
			case "canendprog":
				return BuildingCommandCanEndProg(actor, command);
			case "applies":
				return BuildingCommandApplies(actor, command);
			case "beginverb":
			case "begin":
			case "startverb":
			case "start":
				return BuildingCommandBeginVerb(actor, command);
			case "endverb":
			case "end":
			case "cancelverb":
			case "cancel":
				return BuildingCommandEndVerb(actor, command);
			case "skill":
			case "trait":
				return BuildingCommandSkill(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "threshold":
				return BuildingCommandThreshold(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	#region Building Subcommands

	private bool BuildingCommandFailEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the power fail use emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FailEmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The power fail use emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEndEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the power end emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EndPowerEmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The power end emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the power use emote to?");
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
		actor.OutputHandler.Send($"The power use emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPerception(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which perception type would you like to toggle?\nValid types are {Enum.GetValues<PerceptionTypes>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<PerceptionTypes>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid perception type.\nValid types are {Enum.GetValues<PerceptionTypes>().ListToColouredString()}.");
			return false;
		}

		if (value == PerceptionTypes.All)
		{
			PerceptionTypes = value;
			Changed = true;
			actor.OutputHandler.Send("This invisibility effect now applies to all perception types.");
			return true;
		}

		if (value == PerceptionTypes.None)
		{
			PerceptionTypes = value;
			Changed = true;
			actor.OutputHandler.Send("This invisibility effect no longer applies to any perception types.");
			return true;
		}

		if (PerceptionTypes.HasFlag(value))
		{
			PerceptionTypes &= ~value;
			Changed = true;
			actor.OutputHandler.Send($"This invisibility effect no longer applies to the {value.DescribeEnum().ColourName()} perception type(s).\nThe applicable types are now {PerceptionTypes.DescribeEnum().ColourName()}.");
			return true;
		}

		PerceptionTypes |= value;
		Changed = true;
		actor.OutputHandler.Send($"This invisibility effect now applies to the {value.DescribeEnum().ColourName()} perception type(s).\nThe applicable types are now {PerceptionTypes.DescribeEnum().ColourName()}.");
		return true;
	}

	private bool BuildingCommandWhyCantEndProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog do you want to use for the error about voluntary ending?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Text,
			[ProgVariableTypes.Character]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WhyCantEndPowerProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The prog {prog.MXPClickableFunctionName()} will now give an error message for why the effect cannot be voluntarily ended.");
		return true;
	}

	private bool BuildingCommandCanEndProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog do you want to use for whether the power can be voluntarily ended?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[ProgVariableTypes.Character]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CanEndPowerProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The prog {prog.MXPClickableFunctionName()} will now control whether the invisibility effect can be voluntarily ended.");
		return true;
	}

	private bool BuildingCommandApplies(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog do you want to use for whether the invisibility applies?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.Character]
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		InvisibilityAppliesProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The prog {prog.MXPClickableFunctionName()} will now control whether the invisibility effect applies.");
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
			actor.OutputHandler.Send($"What difficulty should the skill check for this power be? See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		SkillCheckDifficulty = value;
		Changed = true;
		actor.OutputHandler.Send($"This power's skill check will now be at a difficulty of {value.DescribeColoured()}.");
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

	private bool BuildingCommandEndVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should be used to end this power when active?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();
		if (BeginVerb.EqualTo(verb))
		{
			actor.OutputHandler.Send("The begin and verb cannot be the same.");
			return false;
		}

		var costs = InvocationCosts[EndVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(EndVerb);
		EndVerb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to end the power.");
		return true;
	}

	private bool BuildingCommandBeginVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should be used to activate this power?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();
		if (EndVerb.EqualTo(verb))
		{
			actor.OutputHandler.Send("The begin and verb cannot be the same.");
			return false;
		}

		var costs = InvocationCosts[BeginVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(BeginVerb);
		BeginVerb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to begin the power.");
		return true;
	}
	#endregion Building Subcommands
	#endregion Building Commands
}