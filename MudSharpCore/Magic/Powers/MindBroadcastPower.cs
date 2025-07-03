using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public class MindBroadcastPower : MagicPowerBase
{
	public override string PowerType => "Mind Broadcast";
	public override string DatabaseType => "mindbroadcast";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindbroadcast",
			(power, gameworld) => new MindBroadcastPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("mindbroadcast", (gameworld, school, name, actor, command) => {
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

			return new MindBroadcastPower(gameworld, school, name, skill);
		});
	}
	protected override XElement SaveDefinition()
	{
                var definition = new XElement("Definition",
                        new XElement("Verb", Verb),
			new XElement("EmoteText", new XCData(EmoteText)),
			new XElement("FailEmoteText", new XCData(FailEmoteText)),
			new XElement("TargetEmoteText", new XCData(TargetEmoteText)),
			new XElement("UnknownIdentityDescription", new XCData(UnknownIdentityDescription)),
			new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
			new XElement("UseAccent", UseAccent),
			new XElement("UseLanguage", UseLanguage),
			new XElement("TargetCanSeeIdentityProg", TargetCanSeeIdentityProg.Id),
			new XElement("TargetIncluded", TargetIncluded.Id),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id),
			new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
			new XElement("CanInvokePowerProg", CanInvokePowerProg?.Id ?? 0L),
			new XElement("WhyCantInvokePowerProg", WhyCantInvokePowerProg?.Id ?? 0L),
                        new XElement("Distance", (int)PowerDistance)
                );
                AddBaseDefinition(definition);
                return definition;
        }

	private MindBroadcastPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name)
	{
		Blurb = "Send a message to everyone in your room";
		_showHelpText = $"You can use {school.SchoolVerb.ToUpperInvariant()} BROADCAST <MESSAGE> to send a message to everyone in the same room as you.";
		SkillCheckTrait = trait;
		SkillCheckDifficulty = Difficulty.Normal;
		MinimumSuccessThreshold = Outcome.MinorFail;
		EmoteText = "You muster your force of will to amplify your thoughts, and broadcast the following to all present";
		FailEmoteText = "You are unable to amplify your thoughts due to a small lapse in concentration.";
		TargetEmoteText = "You feel the voice of {0} in your mind saying";
		TargetCanSeeIdentityProg = Gameworld.AlwaysTrueProg;
		TargetIncluded = Gameworld.AlwaysTrueProg;
		UnknownIdentityDescription = "an unknown entity";
		Verb = "broadcast";
		UseLanguage = false;
		UseAccent = false;
		PowerDistance = MagicPowerDistance.AdjacentLocationsOnly;
		DoDatabaseInsert();
	}

	protected MindBroadcastPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("UnknownIdentityDescription");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no UnknownIdentityDescription in the definition XML for power {Id} ({Name}).");
		}

		UnknownIdentityDescription = element.Value;

		PowerDistance = (MagicPowerDistance)int.Parse(root.Element("PowerDistance")?.Value ?? "2");
		
		element = root.Element("TargetCanSeeIdentityProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no TargetCanSeeIdentityProg in the definition XML for power {Id} ({Name}).");
		}

		TargetCanSeeIdentityProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("Verb");
		if (element == null)
		{
			throw new ApplicationException($"There was no Verb in the definition XML for power {Id} ({Name}).");
		}

		Verb = element.Value;

		element = root.Element("UseLanguage");
		if (element == null)
		{
			throw new ApplicationException($"There was no UseLanguage in the definition XML for power {Id} ({Name}).");
		}

		UseLanguage = bool.Parse(element.Value);

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

		element = root.Element("TargetEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no TargetEmoteText in the definition XML for power {Id} ({Name}).");
		}

		TargetEmoteText = element.Value;

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

		element = root.Element("UseAccent");
		if (element == null)
		{
			throw new ApplicationException($"There was no UseAccent in the definition XML for power {Id} ({Name}).");
		}

		UseAccent = bool.Parse(element.Value);

		element = root.Element("TargetIncluded");
		if (element != null)
		{
			TargetIncluded = long.TryParse(element.Value, out value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);

			if (TargetIncluded == null)
			{
				throw new ApplicationException(
					$"The TargetIncluded specified an invalid prog in the definition XML for power {Id} ({Name}).");
			}

			if (TargetIncluded.ReturnType != ProgVariableTypes.Boolean)
			{
				throw new ApplicationException(
					$"The TargetIncluded specified a prog that doesn't return boolean in the definition XML for power {Id} ({Name}).");
			}

			if (!TargetIncluded.MatchesParameters(new[] { ProgVariableTypes.Character }))
			{
				throw new ApplicationException(
					$"The TargetIncluded specified  a prog that does not match the required parameter pattern of a single character in the definition XML for power {Id} ({Name}).");
			}
		}
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

		if (CanInvokePowerProg.ExecuteBool(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor)?.ToString() ??
									 "You cannot send any broadcasts.");
			return;
		}

		var check = Gameworld.GetCheck(CheckType.MindBroadcastPower);
		var outcome = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait);
		if (outcome < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(FailEmoteText, actor, actor)));
			return;
		}

		var text = command.RemainingArgument;
		if (text.Length > Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.OutputHandler.Send($"You can't send so much text at once. Keep it under {Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		var targets = AcquireAllValidTargets(actor, PowerDistance);

		if (UseLanguage)
		{
			var langInfo = new PsychicLanguageInfo(actor.CurrentLanguage, UseAccent ? actor.CurrentAccent : null, text, outcome, actor);
			foreach (var target in targets.AsEnumerable())
			{
				if (TargetIncluded?.Execute<bool?>(target) == false)
				{
					continue;
				}

				var emote = new LanguageOutput(new Emote(GetAppropriateTargetEmote(actor, target), actor, actor, target), langInfo, null);
				target.OutputHandler.Send(emote);
			}

			actor.OutputHandler.Send(new EmoteOutput(new Emote(string.Format(EmoteText, text).ProperSentences().Fullstop(), actor, actor)));
		}
		else
		{
			foreach (var target in targets.AsEnumerable())
			{
				if (TargetIncluded?.Execute<bool?>(target) == false)
				{
					continue;
				}

				var emote = new EmoteOutput(new Emote($"{GetAppropriateTargetEmote(actor, target)} \"{text.ProperSentences().Fullstop()}\"", actor, PermitLanguageOptions.IgnoreLanguage, actor, target), flags: OutputFlags.NoLanguage);
				target.OutputHandler.Send(emote);
			}

			actor.OutputHandler.Send(new EmoteOutput(new Emote($"{EmoteText} \"{text.ProperSentences().Fullstop()}\"", actor, PermitLanguageOptions.IgnoreLanguage, actor), flags: OutputFlags.NoLanguage));
		}
	}

	public override IEnumerable<string> Verbs => new[] { Verb };

	public string Verb { get; set; }
	public bool UseLanguage { get; set; }
	public string EmoteText { get; set; }
	public string FailEmoteText { get; set; }
	public string TargetEmoteText { get; set; }
	public IFutureProg TargetCanSeeIdentityProg { get; protected set; }
	public string UnknownIdentityDescription { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public bool UseAccent { get; protected set; }
	public IFutureProg TargetIncluded { get; protected set; }
	public MagicPowerDistance PowerDistance { get; protected set; }

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Power Verb: {Verb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Power Distance: {PowerDistance.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Target Can See Identity Prog: {TargetCanSeeIdentityProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Target Included Prog: {TargetIncluded.MXPClickableFunctionName()}");
		sb.AppendLine($"Unknown Identity Desc: {UnknownIdentityDescription.ColourCharacter()}");
		sb.AppendLine($"Use Language: {UseLanguage.ToColouredString()}");
		sb.AppendLine($"Use Accent: {UseAccent.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Emotes:");
		sb.AppendLine();
		sb.AppendLine($"Emote: {EmoteText.ColourCommand()}");
		sb.AppendLine($"Self Emote: {FailEmoteText.ColourCommand()}");
		sb.AppendLine($"Detected Target Emote: {TargetEmoteText.ColourCommand()}");
	}

	public string GetAppropriateHowSeen(ICharacter connecter, ICharacter connectee)
	{
		if (TargetCanSeeIdentityProg.ExecuteBool(connecter, connectee))
		{
			return connecter.HowSeen(connectee, flags: PerceiveIgnoreFlags.IgnoreConsciousness);
		}

		return UnknownIdentityDescription.ColourCharacter();
	}

	public string GetAppropriateTargetEmote(ICharacter connecter, ICharacter connectee)
	{
		if (TargetCanSeeIdentityProg.ExecuteBool(connecter, connectee))
		{
			return string.Format(TargetEmoteText, "$0", "{0}");
		}

		return string.Format(TargetEmoteText, UnknownIdentityDescription.ColourCharacter(), "{0}");
	}

	#region Building Commands
	/// <inheritdoc />
	protected override string SubtypeHelpText => @"	#3begin <verb>#0 - sets the verb to activate this power
	#3end <verb>#0 - sets the verb to end this power
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <difficulty>#0 - sets the difficulty of the skill check
	#3threshold <outcome>#0 - sets the minimum outcome for skill success
	#3distance <distance>#0 - sets the distance that this power works at
	#3uselanguage#0 - toggles using language (if off, language checks are skipped)
	#3useaccent#0 - toggles using accents if language is used
	#3unknown <desc>#0 - a substitute for sdesc if power user identity is not known
	#3identityprog <prog>#0 - sets a prog that controls whether the target knows who the power user is
	#3targetprog <prog>#0 - sets a prog that controls whether the target can hear the message
	#3emote <emote>#0 - an emote sent to the user when the power is invoked. Don't put a fullstop at the end.
	#3failemote <emote>#0 - an emote sent to the user when the power fails
	#3targetemote <emote>#0 - an emote sent to the target. Use #6{0}#0 instead of $0 for the user. Don't put a fullstop at the end.

#6Note - for emotes, use $0 for the user and $1 for the target";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "beginverb":
			case "begin":
			case "startverb":
			case "start":
			case "verb":
				return BuildingCommandBeginVerb(actor, command);
			case "distance":
				return BuildingCommandDistance(actor, command);
			case "skill":
			case "trait":
				return BuildingCommandSkill(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "threshold":
				return BuildingCommandThreshold(actor, command);
			case "uselanguage":
				return BuildingCommandUseLanguage(actor);
			case "useaccent":
				return BuildingCommandUseAccent(actor);
			case "identityprog":
				return BuildingCommandIdentityProg(actor, command);
			case "unknown":
				return BuildingCommandUnknown(actor, command);
			case "emote":
				return BuildingCommandEmote(actor, command);
			case "failemote":
				return BuildingCommandFailEmote(actor, command);
			case "targetemote":
				return BuildingCommandTargetEmote(actor, command);
			case "targetprog":
				return BuildingCommandTargetProg(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	#region Building Subcommands
	private bool BuildingCommandDistance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"At what distance should this power be able to be used? The valid options are {Enum.GetValues<MagicPowerDistance>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out MagicPowerDistance value))
		{
			actor.OutputHandler.Send($"That is not a valid distance. The valid options are {Enum.GetValues<MagicPowerDistance>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		PowerDistance = value;
		Changed = true;
		actor.OutputHandler.Send($"This magic power can now be used against {value.LongDescription().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTargetProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to determine if the target can hear the message?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, [
			[ProgVariableTypes.Character],
			[ProgVariableTypes.Character, ProgVariableTypes.Character]
		]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		TargetIncluded = prog;
		Changed = true;
		actor.OutputHandler.Send($"This power now uses the {prog.MXPClickableFunctionName()} prog to determine if the target can hear the message.");
		return true;
	}

	private bool BuildingCommandTargetEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an emote.");
			return false;
		}

		var emoteText = command.SafeRemainingArgument;
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		if (!emoteText.IsValidFormatString([false]))
		{
			actor.OutputHandler.Send($"The only valid curly-braces reference in this output is {{0}}.");
			return false;
		}

		EmoteText = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The echo sent when this power is used is now {EmoteText.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandFailEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an emote.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FailEmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The echo sent when this power is used and fails is now {FailEmoteText.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an emote.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The echo sent when this power is used is now {EmoteText.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandUnknown(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description should be shown instead of the real short description if the target can't see the identity?");
			return false;
		}

		UnknownIdentityDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The target will see the text {UnknownIdentityDescription.ColourCharacter()} instead of the user's short description if unknown.");
		return true;
	}

	private bool BuildingCommandIdentityProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to determine if the target gets the short description of the user, or the unknown description?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, [
			[ProgVariableTypes.Character],
			[ProgVariableTypes.Character, ProgVariableTypes.Character]
		]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		TargetCanSeeIdentityProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This power now uses the {prog.MXPClickableFunctionName()} prog to determine if the target gets the real description of the power user.");
		return true;
	}

	private bool BuildingCommandUseAccent(ICharacter actor)
	{
		UseAccent = !UseAccent;
		Changed = true;
		actor.OutputHandler.Send($"The message sent by this power {UseAccent.NowNoLonger()} uses the user's accent when language is involved.");
		return true;
	}

	private bool BuildingCommandUseLanguage(ICharacter actor)
	{
		UseLanguage = !UseLanguage;
		Changed = true;
		actor.OutputHandler.Send($"The message sent by this power {UseLanguage.NowNoLonger()} uses language and requires language understanding.");
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

	private bool BuildingCommandBeginVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should be used to activate this power?");
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