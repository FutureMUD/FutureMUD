#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Community.Boards;
using MudSharp.Framework;
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

public sealed class PresciencePower : MagicPowerBase
{
	public override string PowerType => "Prescience";
	public override string DatabaseType => "prescience";

	public string Verb { get; private set; }
	public ITraitDefinition SkillCheckTrait { get; private set; }
	public Difficulty SkillCheckDifficulty { get; private set; }
	public Outcome MinimumSuccessThreshold { get; private set; }
	public string BoardIdOrName { get; private set; }
	public string SubjectTemplate { get; private set; }
	public string AuthorTemplate { get; private set; }
	public string PromptText { get; private set; }
	public string FailEcho { get; private set; }
	public string SuccessEcho { get; private set; }
	public override IEnumerable<string> Verbs => [Verb];

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("prescience", (power, gameworld) => new PresciencePower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("prescience", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new PresciencePower(gameworld, school, name, trait));
	}

	private PresciencePower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) :
		base(gameworld, school, name)
	{
		IsPsionic = true;
		Blurb = "Submit a prescient question to an administrator board";
		_showHelpText =
			$"Use {school.SchoolVerb.ToUpperInvariant()} PRESCIENCE to enter a question for staff to answer through dreams or other story methods.";
		Verb = "prescience";
		SkillCheckTrait = trait;
		SkillCheckDifficulty = Difficulty.Normal;
		MinimumSuccessThreshold = Outcome.MinorPass;
		BoardIdOrName = "Staff";
		SubjectTemplate = "Prescience: {character}";
		AuthorTemplate = "{character}";
		PromptText = "Enter the question you want to send to the unseen currents.";
		FailEcho = "You open yourself to the future, but the vision remains silent.";
		SuccessEcho = "You open yourself to the future and shape a question in your mind.";
		DoDatabaseInsert();
	}

	private PresciencePower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		Verb = root.Element("Verb")?.Value ?? "prescience";
		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(root.Element("SkillCheckTrait")?.Value ?? "0")) ??
		                  throw new ApplicationException($"Invalid SkillCheckTrait in power {Id} ({Name}).");
		SkillCheckDifficulty = (Difficulty)int.Parse(root.Element("SkillCheckDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		MinimumSuccessThreshold = (Outcome)int.Parse(root.Element("MinimumSuccessThreshold")?.Value ?? ((int)Outcome.MinorPass).ToString());
		BoardIdOrName = root.Element("BoardIdOrName")?.Value ?? "Staff";
		SubjectTemplate = root.Element("SubjectTemplate")?.Value ?? "Prescience: {character}";
		AuthorTemplate = root.Element("AuthorTemplate")?.Value ?? "{character}";
		PromptText = root.Element("PromptText")?.Value ??
		             "Enter the question you want to send to the unseen currents.";
		FailEcho = root.Element("FailEcho")?.Value ?? "You open yourself to the future, but the vision remains silent.";
		SuccessEcho = root.Element("SuccessEcho")?.Value ??
		              "You open yourself to the future and shape a question in your mind.";
	}

	protected override XElement SaveDefinition()
	{
		var definition = new XElement("Definition",
			new XElement("Verb", Verb),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id),
			new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
			new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
			new XElement("BoardIdOrName", BoardIdOrName),
			new XElement("SubjectTemplate", new XCData(SubjectTemplate)),
			new XElement("AuthorTemplate", new XCData(AuthorTemplate)),
			new XElement("PromptText", new XCData(PromptText)),
			new XElement("FailEcho", new XCData(FailEcho)),
			new XElement("SuccessEcho", new XCData(SuccessEcho))
		);
		AddBaseDefinition(definition);
		return definition;
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, Verb);
		if (!truth)
		{
			actor.OutputHandler.Send($"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		if (!HandleGeneralUseRestrictions(actor))
		{
			return;
		}

		if (CanInvokePowerProg.ExecuteBool(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor)?.ToString() ?? "You cannot use that power right now.");
			return;
		}

		var outcome = Gameworld.GetCheck(CheckType.PresciencePower).Check(actor, SkillCheckDifficulty, SkillCheckTrait);
		if (outcome < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(FailEcho, actor, actor)));
			return;
		}

		var board = Gameworld.Boards.GetByIdOrName(BoardIdOrName);
		if (board is null)
		{
			actor.OutputHandler.Send(
				$"The configured board {BoardIdOrName.ColourCommand()} could not be found. An administrator must update this power.");
			return;
		}

		if (!string.IsNullOrWhiteSpace(SuccessEcho))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(SuccessEcho, actor, actor)));
		}

		actor.OutputHandler.Send(PromptText.SubstituteANSIColour());
		actor.EditorMode(PostAction, CancelAction, 1.0, suppliedArguments: new object[] { actor, board });
		PsionicActivityNotifier.Notify(actor, this, "a prescient question");
		ConsumePowerCosts(actor, Verb);
	}

	private void CancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send("You let the prescient question fade before it forms.");
	}

	private void PostAction(string text, IOutputHandler handler, object[] args)
	{
		if (text.Length > 5000)
		{
			handler.Send("Prescience questions must be under 5000 characters in length.");
			return;
		}

		var actor = (ICharacter)args[0];
		var board = (IBoard)args[1];
		var subject = FormatTemplate(SubjectTemplate, actor).TitleCase();
		var author = FormatTemplate(AuthorTemplate, actor);
		board.MakeNewPost(author, subject, text.Trim());
		handler.Send($"You commit your question to the currents of fate as {subject.ColourName()}.");
	}

	private string FormatTemplate(string text, ICharacter actor)
	{
		return text
		       .Replace("{character}", actor.PersonalName.GetName(Character.Name.NameStyle.SimpleFull))
		       .Replace("{account}", actor.Account.Name)
		       .Replace("{school}", School.Name)
		       .Replace("{power}", Name)
		       .NormaliseSpacing()
		       .Trim();
	}

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Verb: {Verb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Board: {BoardIdOrName.ColourValue()}");
		sb.AppendLine($"Subject Template: {SubjectTemplate.ColourCommand()}");
		sb.AppendLine($"Author Template: {AuthorTemplate.ColourCommand()}");
		sb.AppendLine($"Prompt Text: {PromptText.ColourCommand()}");
		sb.AppendLine($"Fail Echo: {FailEcho.ColourCommand()}");
		sb.AppendLine($"Success Echo: {SuccessEcho.ColourCommand()}");
	}

	protected override string SubtypeHelpText => @"	#3verb <verb>#0 - sets the command verb
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <difficulty>#0 - sets the skill check difficulty
	#3threshold <outcome>#0 - sets the minimum success threshold
	#3board <which>#0 - sets the board to receive posts
	#3subject <template>#0 - sets post subject template
	#3author <template>#0 - sets board author template
	#3prompt <text>#0 - sets editor prompt text
	#3failecho <emote>#0 - sets failure echo
	#3successecho <emote|none>#0 - sets success echo";

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
			case "board":
				return BuildingCommandBoard(actor, command);
			case "subject":
				return BuildingCommandTemplate(actor, command, "subject");
			case "author":
				return BuildingCommandTemplate(actor, command, "author");
			case "prompt":
				return BuildingCommandTemplate(actor, command, "prompt");
			case "failecho":
				return BuildingCommandEcho(actor, command, "fail");
			case "successecho":
				return BuildingCommandEcho(actor, command, "success");
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
		actor.OutputHandler.Send($"Prescience now uses {Verb.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send("That is not a valid skill or trait.");
			return false;
		}

		SkillCheckTrait = trait;
		Changed = true;
		actor.OutputHandler.Send($"Prescience now checks {trait.Name.ColourName()}.");
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
		actor.OutputHandler.Send($"Prescience now checks at {value.DescribeColoured()}.");
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
		actor.OutputHandler.Send($"Prescience now requires at least {value.DescribeColour()}.");
		return true;
	}

	private bool BuildingCommandBoard(ICharacter actor, StringStack command)
	{
		var board = Gameworld.Boards.GetByIdOrName(command.SafeRemainingArgument);
		if (board is null)
		{
			actor.OutputHandler.Send($"There is no such board. See the {"boards".MXPSend("boards")} command for a list.");
			return false;
		}

		BoardIdOrName = board.Id.ToString();
		Changed = true;
		actor.OutputHandler.Send($"Prescience will now post to the {board.Name.ColourName()} board.");
		return true;
	}

	private bool BuildingCommandTemplate(ICharacter actor, StringStack command, string which)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What text should be used? You can use {character}, {account}, {school}, and {power}.");
			return false;
		}

		switch (which)
		{
			case "subject":
				SubjectTemplate = command.SafeRemainingArgument;
				break;
			case "author":
				AuthorTemplate = command.SafeRemainingArgument;
				break;
			case "prompt":
				PromptText = command.SafeRemainingArgument;
				break;
		}

		Changed = true;
		actor.OutputHandler.Send($"The {which} template is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command, string which)
	{
		if (which.EqualTo("success") && (command.IsFinished || command.SafeRemainingArgument.EqualToAny("none", "clear", "delete")))
		{
			SuccessEcho = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("Prescience no longer has a success echo.");
			return true;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote should be used?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		if (which.EqualTo("fail"))
		{
			FailEcho = command.SafeRemainingArgument;
		}
		else
		{
			SuccessEcho = command.SafeRemainingArgument;
		}

		Changed = true;
		actor.OutputHandler.Send($"The {which} echo is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
}
