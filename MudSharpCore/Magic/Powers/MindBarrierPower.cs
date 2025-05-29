using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public class MindBarrierPower : SustainedMagicPower
{
	public override string PowerType => "Barrier";
	public override string DatabaseType => "mindbarrier";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindbarrier",
			(power, gameworld) => new MindBarrierPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("mindbarrier", (gameworld, school, name, actor, command) => {
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

			return new MindBarrierPower(gameworld, school, name, skill);
		});
	}

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
		var definition = new XElement("Definition",
			new XElement("BeginVerb", BeginVerb),
			new XElement("EndVerb", EndVerb),
			new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id),
			new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
			new XElement("AppliesToCharacterProg", AppliesToCharacterProg.Id),
			new XElement("EmoteForBegin", new XCData(EmoteForBegin)),
			new XElement("EmoteForBeginSelf", new XCData(EmoteForBeginSelf)),
			new XElement("EmoteForEnd", new XCData(EmoteForEnd)),
			new XElement("EmoteForEndSelf", new XCData(EmoteForEndSelf)),
			new XElement("BlockEmoteSelf", new XCData(BlockEmoteSelf)),
			new XElement("BlockEmoteTarget", new XCData(BlockEmoteTarget)),
			new XElement("OvercomeEmoteSelf", new XCData(OvercomeEmoteSelf)),
			new XElement("OvercomeEmoteTarget", new XCData(OvercomeEmoteTarget)),
			new XElement("EndWhenNotSustainingError", new XCData(EndWhenNotSustainingError)),
			new XElement("BeginWhenAlreadySustainingError", new XCData(BeginWhenAlreadySustainingError)),
			new XElement("PermitAllies", PermitAllies),
			new XElement("PermitTrustedAllies", PermitTrustedAllies),
			new XElement("FailIfOvercome", FailIfOvercome),
			new XElement("Bonuses",
				from bonus in TargetCheckBonusPerOutcome
				select new XElement("Bonus",
					new XAttribute("outcome", (int)bonus.Key),
					new XAttribute("bonus", bonus.Value)
				)
			)
		);
		SaveSustainedDefinition(definition);
		return definition;
	}

	private MindBarrierPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name)
	{
		Blurb = "Guard against foreign presences in your mind";
		_showHelpText = $"You can use {school.SchoolVerb.ToUpperInvariant()} BARRIER to start maintaining a mental barrier against intrusion, and {school.SchoolVerb.ToUpperInvariant()} ENDBARRIER to relax your barrier.";
		BeginVerb = "barrier";
		EndVerb = "endbarrier";
		SkillCheckTrait = trait;
		SkillCheckDifficulty = Difficulty.VeryEasy;
		MinimumSuccessThreshold = Outcome.Fail;
		ConcentrationPointsToSustain = 1.0;
		PermitAllies = false;
		PermitTrustedAllies = false;
		AppliesToCharacterProg = Gameworld.AlwaysTrueProg;
		FailIfOvercome = true;
		EmoteForBegin = "";
		EmoteForBeginSelf = "You steel your mind against foreign presences.";
		EmoteForEnd = "";
		EmoteForEndSelf = "You drop your mental barriers, relaxing your vigilance against foreign presences.";
		BlockEmoteSelf = "You feel the presence of $1 attempting to break through your mental barrier.";
		BlockEmoteTarget = "The mind of $0 is shielded against your attempts to connect with it.";
		OvercomeEmoteSelf = "You feel your mental barrier shatter as the presence of $1 enters your mind.";
		OvercomeEmoteTarget = "You shatter the mental barrier shielding $0's mind against foreign presences.";
		EndWhenNotSustainingError = "You are not currently shielding your mind against foreign presences.";
		BeginWhenAlreadySustainingError = "You are already shielding your mind against foreign presences.";
		TargetCheckBonusPerOutcome[Outcome.MajorFail] = 5.0;
		TargetCheckBonusPerOutcome[Outcome.Fail] = 3.0;
		TargetCheckBonusPerOutcome[Outcome.MinorFail] = 1.0;
		TargetCheckBonusPerOutcome[Outcome.MinorPass] = 0.0;
		TargetCheckBonusPerOutcome[Outcome.Pass] = -1.0;
		TargetCheckBonusPerOutcome[Outcome.MajorPass] = -2.0;
		DoDatabaseInsert();
	}

	protected MindBarrierPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("BeginVerb");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a BeginVerb element.");
		}

		BeginVerb = element.Value.ToLowerInvariant();

		element = root.Element("EndVerb");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EndVerb element.");
		}

		EndVerb = element.Value.ToLowerInvariant();

		element = root.Element("SkillCheckDifficulty");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a SkillCheckDifficulty element.");
		}
		SkillCheckDifficulty = (Difficulty)int.Parse(element.Value);

		element = root.Element("SkillCheckTrait");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a SkillCheckTrait element.");
		}
		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(element.Value));

		element = root.Element("MinimumSuccessThreshold");
		MinimumSuccessThreshold = (Outcome)int.Parse(element?.Value ?? ((int)Outcome.MinorFail).ToString());

		element = root.Element("PermitAllies");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a PermitAllies element.");
		}
		PermitAllies = bool.Parse(element.Value);

		element = root.Element("PermitTrustedAllies");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a PermitTrustedAllies element.");
		}
		PermitTrustedAllies = bool.Parse(element.Value);

		element = root.Element("FailIfOvercome");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a FailIfOvercome element.");
		}
		FailIfOvercome = bool.Parse(element.Value);

		element = root.Element("AppliesToCharacterProg");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a AppliesToCharacterProg element.");
		}
		AppliesToCharacterProg = Gameworld.FutureProgs.GetByIdOrName(element.Value);

		element = root.Element("EmoteForBegin");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EmoteForBegin element.");
		}
		EmoteForBegin = element.Value;

		element = root.Element("EmoteForBeginSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EmoteForBeginSelf element.");
		}
		EmoteForBeginSelf = element.Value;

		element = root.Element("EmoteForEnd");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EmoteForEnd element.");
		}
		EmoteForEnd = element.Value;

		element = root.Element("EmoteForEndSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EmoteForEndSelf element.");
		}
		EmoteForEndSelf = element.Value;

		element = root.Element("BlockEmoteSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a BlockEmoteSelf element.");
		}
		BlockEmoteSelf = element.Value;

		element = root.Element("BlockEmoteTarget");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a BlockEmoteTarget element.");
		}
		BlockEmoteTarget = element.Value;

		element = root.Element("OvercomeEmoteSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a OvercomeEmoteSelf element.");
		}
		OvercomeEmoteSelf = element.Value;

		element = root.Element("OvercomeEmoteTarget");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a OvercomeEmoteTarget element.");
		}
		OvercomeEmoteTarget = element.Value;

		element = root.Element("EndWhenNotSustainingError");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EndWhenNotSustainingError element.");
		}
		EndWhenNotSustainingError = element.Value;

		element = root.Element("BeginWhenAlreadySustainingError");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a BeginWhenAlreadySustainingError element.");
		}
		BeginWhenAlreadySustainingError = element.Value;

		element = root.Element("Bonuses");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a Bonuses element.");
		}

		foreach (var sub in element.Elements("Bonus"))
		{
			var outcomeAttribute = sub.Attribute("outcome");
			if (outcomeAttribute == null)
			{
				throw new ApplicationException(
					$"The Bonus \"{sub}\" had an invalid outcome attribute in the definition XML for power {Id} ({Name}).");
			}
			var outcome = (Outcome)int.Parse(sub.Value);

			var bonusAttribute = sub.Attribute("bonus");
			if (bonusAttribute == null)
			{
				throw new ApplicationException(
					$"The Bonus \"{sub}\" had an invalid outcome attribute in the definition XML for power {Id} ({Name}).");
			}
			var bonus = double.Parse(bonusAttribute.Value);

			TargetCheckBonusPerOutcome[outcome] = bonus;
			foreach (var value in Enum.GetValues(typeof(Outcome)).OfType<Outcome>())
			{
				if (value < Outcome.MajorFail)
				{
					continue;
				}

				if (TargetCheckBonusPerOutcome.ContainsKey(value))
				{
					continue;
				}

				TargetCheckBonusPerOutcome[value] = 0.0;
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

		if (verb.EqualTo(BeginVerb))
		{
			UseCommandBegin(actor, command);
			return;
		}

		UseCommandEnd(actor, command);
	}

	private void UseCommandEnd(ICharacter actor, StringStack command)
	{
		var effects = actor.EffectsOfType<MindBarrierEffect>().Where(x => x.PowerOrigin == this).ToList();
		if (!effects.Any())
		{
			actor.OutputHandler.Send(EndWhenNotSustainingError);
			return;
		}

		foreach (var effect in effects)
		{
			actor.RemoveEffect(effect, true);
		}
		ConsumePowerCosts(actor, EndVerb);
	}

	private void UseCommandBegin(ICharacter actor, StringStack command)
	{
		var effects = actor.EffectsOfType<MindBarrierEffect>().Where(x => x.PowerOrigin == this).ToList();
		if (effects.Any())
		{
			actor.OutputHandler.Send(BeginWhenAlreadySustainingError);
			return;
		}
		var check = Gameworld.GetCheck(CheckType.MindBarrierPowerCheck);
		var result = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait);
		actor.OutputHandler.Send(EmoteForBeginSelf);
		if (!string.IsNullOrEmpty(EmoteForBegin))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteForBegin, actor, actor), flags: OutputFlags.SuppressSource));
		}
		actor.AddEffect(new MindBarrierEffect(actor, TargetCheckBonusPerOutcome[result], this), GetDuration(result.SuccessDegrees()));
		ConsumePowerCosts(actor, BeginVerb);
	}

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		var effects = actor.EffectsOfType<MindBarrierEffect>().Where(x => x.PowerOrigin == this).ToList();
		foreach (var effect in effects)
		{
			actor.RemoveEffect(effect, true);
		}
	}

	public override IEnumerable<string> Verbs => new[] { BeginVerb, EndVerb };

	public string BeginVerb { get; protected set; }
	public string EndVerb { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public bool PermitAllies { get; protected set; }
	public bool PermitTrustedAllies { get; protected set; }
	public IFutureProg AppliesToCharacterProg { get; protected set; }
	public Dictionary<Outcome, double> TargetCheckBonusPerOutcome { get; } = new();
	public bool FailIfOvercome { get; protected set; }
	public string EmoteForBegin { get; protected set; }
	public string EmoteForBeginSelf { get; protected set; }
	public string EmoteForEnd { get; protected set; }
	public string EmoteForEndSelf { get; protected set; }
	public string BlockEmoteSelf { get; protected set; }
	public string BlockEmoteTarget { get; protected set; }
	public string OvercomeEmoteSelf { get; protected set; }
	public string OvercomeEmoteTarget { get; protected set; }
	public string EndWhenNotSustainingError { get; protected set; }
	public string BeginWhenAlreadySustainingError { get; protected set; }

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Begin Verb: {BeginVerb.ColourCommand()}");
		sb.AppendLine($"End Verb: {EndVerb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Applies Character Prog: {AppliesToCharacterProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Permit Allies: {PermitAllies.ToColouredString()}");
		sb.AppendLine($"Permit Trusted Allies: {PermitAllies.ToColouredString()}");
		sb.AppendLine($"Fail If Overcome: {FailIfOvercome.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Check Bonuses (for opposition):");
		foreach (var item in TargetCheckBonusPerOutcome.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.DescribeColour()}: {item.Value.ToBonusString(actor)}");
		}
		sb.AppendLine();
		sb.AppendLine("Emotes:");
		sb.AppendLine();
		sb.AppendLine($"Begin Emote: {EmoteForBegin.ColourCommand()}");
		sb.AppendLine($"Emote Self: {EmoteForBeginSelf.ColourCommand()}");
		sb.AppendLine($"End Emote: {EmoteForEnd.ColourCommand()}");
		sb.AppendLine($"End Emote Self: {EmoteForEndSelf.ColourCommand()}");
		sb.AppendLine($"Block Emote Self: {BlockEmoteSelf.ColourCommand()}");
		sb.AppendLine($"Block Emote Target: {BlockEmoteTarget.ColourCommand()}");
		sb.AppendLine($"Overcome Emote Self: {OvercomeEmoteSelf.ColourCommand()}");
		sb.AppendLine($"Overcome Emote Target: {OvercomeEmoteTarget.ColourCommand()}");
		sb.AppendLine($"End Not Sustain Emote: {EndWhenNotSustainingError.ColourCommand()}");
		sb.AppendLine($"Begin Already Sustaining Emote: {BeginWhenAlreadySustainingError.ColourCommand()}");
	}

	#region Building Commands
	/// <inheritdoc />
	protected override string SubtypeHelpText => @"	#3begin <verb>#0 - sets the verb to activate this power
	#3end <verb>#0 - sets the verb to end this power
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <difficulty>#0 - sets the difficulty of the skill check
	#3threshold <outcome>#0 - sets the minimum outcome for skill success
	#3distance <distance>#0 - sets the distance that this power can be used at
	#3allies#0 - toggles whether allies can bypass the barrier
	#3trusted#0 - toggles whether trusted allies can bypass the barrier
	#3applies <prog>#0 - sets the prog that controls whether the barrier applies
	#3breakonfail#0 - toggles whether the barrier breaks and ends when someone hostile overcomes it
	#3bonus <outcome> <bonus>#0 - sets the bonus for overcoming the power based on casting outcome
	#3beginemote <emote>#0 - sets the emote for starting this power. If blank, doesn't echo to others
	#3beginemoteself <emote>#0 - sets the self emote for starting this power
	#3endemote <emote>#0 - sets the emote for ending this power. If blank, doesn't echo to others
	#3endemoteself <emote>#0 - sets the self emote for ending this power
	#3blockemote <emote>#0 - sets the emote for blocking a connect
	#3blockemoteself <emote>#0 - sets the self emote for blocking a connect. If blank, doesn't echo
	#3overcomeemote <emote>#0 - sets the emote for a target overcoming the barrier
	#3overcomeemoteself <emote>#0 - sets the self emote for a target overcoming the barrier
	#3errorbeginemote <emote>#0 - sets the emote for trying to invoke the power when already invoked
	#3errorendemote <emote>#0 - sets the emote for trying to end the power when not sustaining

#6Note - for all emotes, $0 is the power user and $1 the intruder#0.";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "allies":
				return BuildingCommandAllies(actor);
			case "trusted":
				return BuildingCommandTrusted(actor);
			case "applies":
			case "appliesprog":
				return BuildingCommandAppliesToCharacterProg(actor, command);
			case "bonus":
				return BuildingCommandBonus(actor, command);
			case "breakonfail":
				return BuildingCommandBreakOnFail(actor, command);
			case "beginemote":
				return BuildingCommandBeginEmote(actor, command);
			case "beginemoteself":
				return BuildingCommandBeginEmoteSelf(actor, command);
			case "endemote":
				return BuildingCommandEndEmote(actor, command);
			case "endemoteself":
				return BuildingCommandEndEmoteSelf(actor, command);
			case "blockemote":
				return BuildingCommandBlockEmote(actor, command);
			case "blockemoteself":
				return BuildingCommandBlockEmoteSelf(actor, command);
			case "overcomeemote":
				return BuildingCommandOvercomeEmote(actor, command);
			case "overcomeemoteself":
				return BuildingCommandOvercomeEmoteSelf(actor, command);
			case "errorbeginemote":
				return BuildingCommandErrorBeginEmote(actor, command);
			case "errorendemote":
				return BuildingCommandErrorEndEmote(actor, command);
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

	private bool BuildingCommandAllies(ICharacter actor)
	{
		PermitAllies = !PermitAllies;
		Changed = true;
		actor.OutputHandler.Send($"This power will {PermitAllies.NowNoLonger()} permit allies to bypass the barrier.");
		return true;
	}

	private bool BuildingCommandTrusted(ICharacter actor)
	{
		PermitTrustedAllies = !PermitTrustedAllies;
		Changed = true;
		actor.OutputHandler.Send($"This power will {PermitTrustedAllies.NowNoLonger()} permit trusted allies to bypass the barrier.");
		return true;
	}

	private bool BuildingCommandAppliesToCharacterProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
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

		AppliesToCharacterProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The prog that controls whether a character is affected by the barrier is now {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which outcome do you want to set the bonus for?\nValid options are {Enum.GetValues<Outcome>().ListToColouredString()}.");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum<Outcome>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid outcome.\nValid options are {Enum.GetValues<Outcome>().ListToColouredString()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What bonus should be applied to the target's overcome check based on that outcome?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var bonus))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid bonus.");
			return false;
		}

		TargetCheckBonusPerOutcome[value] = bonus;
		Changed = true;
		actor.OutputHandler.Send($"The bonus for the target's overcome check when the user gets an outcome of {value.DescribeColour()} is now {bonus.ToBonusString()}.");
		return true;
	}

	private bool BuildingCommandBreakOnFail(ICharacter actor, StringStack command)
	{
		FailIfOvercome = !FailIfOvercome;
		Changed = true;
		actor.OutputHandler.Send($"This power will {FailIfOvercome.NowNoLonger()} fail if bypassed by a hostile target.");
		return true;
	}

	#region Building Subcommands
	private bool BuildingCommandBeginEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			EmoteForBegin = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This power will no longer echo to others when used.");
			return true;
		}

		var text = command.SafeRemainingArgument.ProperSentences();
		var testEmote = new Emote(text, new DummyPerceiver(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage);
			return false;
		}

		EmoteForBegin = text;
		Changed = true;
		actor.OutputHandler.Send($"The emote for beginning this power is now {text.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandBeginEmoteSelf(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an emote.");
			return true;
		}

		var text = command.SafeRemainingArgument.ProperSentences();
		var testEmote = new Emote(text, new DummyPerceiver(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage);
			return false;
		}

		EmoteForBeginSelf = text;
		Changed = true;
		actor.OutputHandler.Send($"The self emote for beginning this power is now {text.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEndEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			EmoteForEnd = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This power will no longer echo to others when ended.");
			return true;
		}

		var text = command.SafeRemainingArgument.ProperSentences();
		var testEmote = new Emote(text, new DummyPerceiver(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage);
			return false;
		}

		EmoteForEnd = text;
		Changed = true;
		actor.OutputHandler.Send($"The emote for ending this power is now {text.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEndEmoteSelf(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an emote.");
			return true;
		}

		var text = command.SafeRemainingArgument.ProperSentences();
		var testEmote = new Emote(text, new DummyPerceiver(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage);
			return false;
		}

		EmoteForEndSelf = text;
		Changed = true;
		actor.OutputHandler.Send($"The self emote for ending this power is now {text.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandBlockEmoteSelf(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			BlockEmoteSelf = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This power will no longer echo to the user when others are blocked.");
			return true;
		}

		var text = command.SafeRemainingArgument.ProperSentences();
		var testEmote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage);
			return false;
		}

		BlockEmoteSelf = text;
		Changed = true;
		actor.OutputHandler.Send($"The self-emote for blocking an intrusion is now {text.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandBlockEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an emote.");
			return true;
		}

		var text = command.SafeRemainingArgument.ProperSentences();
		var testEmote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage);
			return false;
		}

		BlockEmoteTarget = text;
		Changed = true;
		actor.OutputHandler.Send($"The emote shown to the target when blocked is now {text.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandOvercomeEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an emote.");
			return true;
		}

		var text = command.SafeRemainingArgument.ProperSentences();
		var testEmote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage);
			return false;
		}

		OvercomeEmoteTarget = text;
		Changed = true;
		actor.OutputHandler.Send($"The emote shown to the target when a barrier is overcome is now {text.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandOvercomeEmoteSelf(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an emote.");
			return true;
		}

		var text = command.SafeRemainingArgument.ProperSentences();
		var testEmote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage);
			return false;
		}

		OvercomeEmoteSelf = text;
		Changed = true;
		actor.OutputHandler.Send($"The emote shown to the user when an intruder is blocked is now {text.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandErrorBeginEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an emote.");
			return true;
		}

		var text = command.SafeRemainingArgument.ProperSentences();
		var testEmote = new Emote(text, new DummyPerceiver(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage);
			return false;
		}

		BeginWhenAlreadySustainingError = text;
		Changed = true;
		actor.OutputHandler.Send($"The emote shown to the user when trying to use the power while already sustaining is now {text.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandErrorEndEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an emote.");
			return true;
		}

		var text = command.SafeRemainingArgument.ProperSentences();
		var testEmote = new Emote(text, new DummyPerceiver(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage);
			return false;
		}

		EndWhenNotSustainingError = text;
		Changed = true;
		actor.OutputHandler.Send($"The emote shown to the user when trying to end the power while not sustaining is now {text.ColourCommand()}.");
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
