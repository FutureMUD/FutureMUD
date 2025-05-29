using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public class ChokePower : SustainedMagicPower
{
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("choke", (power, gameworld) => new ChokePower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("choke", (gameworld, school, name, actor, command) => {
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

			return new ChokePower(gameworld, school, name, skill);
		});
	}

	protected ChokePower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("BeginVerb");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no BeginVerb in the definition XML for power {Id} ({Name}).");
		}

		BeginVerb = element.Value;

		element = root.Element("EndVerb");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EndVerb in the definition XML for power {Id} ({Name}).");
		}

		EndVerb = element.Value;

		element = root.Element("PowerDistance");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no PowerDistance in the definition XML for power {Id} ({Name}).");
		}

		if (!Enum.TryParse<MagicPowerDistance>(element.Value, true, out var dist))
		{
			throw new ApplicationException(
				$"The PowerDistance value specified in power {Id} ({Name}) was not valid. The value was {element.Value}.");
		}

		PowerDistance = dist;

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

		element = root.Element("ResistCheckDifficulty");
		if (element == null)
		{
			ResistCheckDifficulty = Difficulty.Impossible;
			TargetGetsResistanceCheck = false;
			ResistCheckInterval = TimeSpan.Zero;
		}
		else
		{
			ResistCheckDifficulty = (Difficulty)int.Parse(element.Value);
			TargetGetsResistanceCheck = true;
			element = root.Element("ResistCheckInterval");
			if (element == null)
			{
				throw new ApplicationException(
					$"There was no ResistCheckDifficulty in the definition XML for power {Id} ({Name}) when a ResistCheckDifficulty element was set.");
			}

			ResistCheckInterval = TimeSpan.FromSeconds(double.Parse(element.Value));

			element = root.Element("TargetResistanceEmoteText");
			if (element == null)
			{
				throw new ApplicationException(
					$"There was no TargetResistanceEmoteText in the definition XML for power {Id} ({Name}) when a ResistCheckDifficulty element was set.");
			}

			TargetResistanceEmoteText = element.Value;
			var resistEmote = new Emote(TargetResistanceEmoteText, new DummyPerceiver(), new DummyPerceivable(),
				new DummyPerceivable());
			if (!resistEmote.Valid)
			{
				throw new ApplicationException(
					$"There was an issue with the TargetResistanceEmoteText in the definition XML for power {Id} ({Name}): {resistEmote.ErrorMessage}");
			}

			element = root.Element("TargetResistanceEmoteTextTarget");
			if (element == null)
			{
				throw new ApplicationException(
					$"There was no TargetResistanceEmoteTextTarget in the definition XML for power {Id} ({Name}) when a ResistCheckDifficulty element was set.");
			}

			TargetResistanceEmoteTextTarget = element.Value;
			resistEmote = new Emote(TargetResistanceEmoteTextTarget, new DummyPerceiver(), new DummyPerceivable(),
				new DummyPerceivable());
			if (!resistEmote.Valid)
			{
				throw new ApplicationException(
					$"There was an issue with the TargetResistanceEmoteTextTarget in the definition XML for power {Id} ({Name}): {resistEmote.ErrorMessage}");
			}
		}

		element = root.Element("EmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EmoteText in the definition XML for power {Id} ({Name}).");
		}

		EmoteText = element.Value;
		var emote = new Emote(EmoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			throw new ApplicationException(
				$"There was an issue with the EmoteText in the definition XML for power {Id} ({Name}): {emote.ErrorMessage}");
		}

		element = root.Element("EmoteTextTarget");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EmoteTextTarget in the definition XML for power {Id} ({Name}).");
		}

		EmoteTextTarget = element.Value;
		emote = new Emote(EmoteTextTarget, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			throw new ApplicationException(
				$"There was an issue with the EmoteTextTarget in the definition XML for power {Id} ({Name}): {emote.ErrorMessage}");
		}

		element = root.Element("FailEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no FailEmoteText in the definition XML for power {Id} ({Name}).");
		}

		FailEmoteText = element.Value;
		emote = new Emote(FailEmoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			throw new ApplicationException(
				$"There was an issue with the FailEmoteText in the definition XML for power {Id} ({Name}): {emote.ErrorMessage}");
		}

		element = root.Element("FailEmoteTextTarget");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no FailEmoteTextTarget in the definition XML for power {Id} ({Name}).");
		}

		FailEmoteTextTarget = element.Value;
		emote = new Emote(FailEmoteTextTarget, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			throw new ApplicationException(
				$"There was an issue with the FailEmoteTextTarget in the definition XML for power {Id} ({Name}): {emote.ErrorMessage}");
		}

		element = root.Element("EndPowerEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EndPowerEmoteText in the definition XML for power {Id} ({Name}).");
		}

		EndPowerEmoteText = element.Value;
		emote = new Emote(EndPowerEmoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			throw new ApplicationException(
				$"There was an issue with the EndPowerEmoteText in the definition XML for power {Id} ({Name}): {emote.ErrorMessage}");
		}

		element = root.Element("EndPowerEmoteTextTarget");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EndPowerEmoteTextTarget in the definition XML for power {Id} ({Name}).");
		}

		EndPowerEmoteTextTarget = element.Value;
		emote = new Emote(EndPowerEmoteTextTarget, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			throw new ApplicationException(
				$"There was an issue with the EndPowerEmoteTextTarget in the definition XML for power {Id} ({Name}): {emote.ErrorMessage}");
		}
	}

	protected ChokePower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name)
	{
		Blurb = "Choke your target so that they can't breathe";
		_showHelpText = @$"You can use #3{school.SchoolVerb.ToUpperInvariant()} CHOKE <person>#0 to choke someone in your presence, and #3{school.SchoolVerb.ToUpperInvariant()} STOPCHOKE [<person>]#0 to stop choking them. While choked, they will not be able to breathe.";
		PowerDistance = MagicPowerDistance.SameLocationOnly;
		BeginVerb = "choke";
		EndVerb = "stopchoke";
		ResistCheckDifficulty = Difficulty.Normal;
		ResistCheckInterval = TimeSpan.FromSeconds(10);
		TargetGetsResistanceCheck = true;
		SkillCheckTrait = trait;
		SkillCheckDifficulty = Difficulty.Normal;
		MinimumSuccessThreshold = Outcome.Pass;
		ConcentrationPointsToSustain = 1.0;
		EmoteText = "@ throw|throws a withering glance towards $1 as #0 concentrate|concentrates all of &0's malice at &1.";
		EmoteTextTarget = "Your throat tightens up under a hateful hex and you can no longer breathe!";
		FailEmoteText = "@ throw|throws a withering glance towards $1 as #0 concentrate|concentrates all of &0's malice at &1, but appear|appears to be unsuccessful.";
		FailEmoteTextTarget = "Your throat feels briefly tight, but you manage to keep breathing.";
		EndPowerEmoteText = "@ stop|stops focusing &0's withering malice toward $1.";
		EndPowerEmoteTextTarget = "The hateful hex stopping you from breathing has ended.";
		TargetResistanceEmoteText = "@ stop|stops focusing &0's malice towards $1 as #1 $1|appear|appears to have broken free of &0's influence.";
		TargetResistanceEmoteTextTarget = "You have broken free of $0's choking powers.";
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
		var definition = new XElement("Definition",
			new XElement("BeginVerb", BeginVerb),
			new XElement("EndVerb", EndVerb),
			new XElement("PowerDistance", (int)PowerDistance),
			new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id),
			new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
			new XElement("ResistCheckDifficulty", (int)ResistCheckDifficulty),
			new XElement("ResistCheckInterval", ResistCheckInterval.TotalSeconds),
			new XElement("TargetResistanceEmoteText", new XCData(TargetResistanceEmoteText)),
			new XElement("TargetResistanceEmoteTextTarget", new XCData(TargetResistanceEmoteTextTarget)),
			new XElement("EmoteText", new XCData(EmoteText)),
			new XElement("EmoteTextTarget", new XCData(EmoteTextTarget)),
			new XElement("FailEmoteText", new XCData(FailEmoteText)),
			new XElement("FailEmoteTextTarget", new XCData(FailEmoteTextTarget)),
			new XElement("EndPowerEmoteText", new XCData(EndPowerEmoteText)),
			new XElement("EndPowerEmoteTextTarget", new XCData(EndPowerEmoteTextTarget))
		);
		SaveSustainedDefinition(definition);
		return definition;
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

		if (EndVerb.StartsWith(verb, StringComparison.InvariantCultureIgnoreCase))
		{
			UseCommandEnd(actor, command);
			return;
		}

		if (!BeginVerb.StartsWith(verb, StringComparison.InvariantCultureIgnoreCase))
		{
			actor.OutputHandler.Send(
				$"You can either {BeginVerb.ColourCommand()} to begin choking someone or {EndVerb.ColourCommand()} to end it.");
			return;
		}

		var target = AcquireTarget(actor, command.PopSpeech(), PowerDistance);
		if (target == null)
		{
			actor.OutputHandler.Send("You can't find any target like that to choke.");
			return;
		}

		if (CanInvokePowerProg.ExecuteBool(actor, target) == false)
		{
			actor.OutputHandler.Send(string.Format(
				WhyCantInvokePowerProg.Execute(actor, target)?.ToString() ??
				"You cannot choke {0}.", target.HowSeen(actor)));
			return;
		}

		if (target.AffectedBy<BeingMagicChoked>(actor, this))
		{
			actor.OutputHandler.Send($"You are already choking {target.HowSeen(actor)}.");
			return;
		}

		ConsumePowerCosts(actor, BeginVerb);

		var check = Gameworld.GetCheck(CheckType.MagicChokePower);
		var results = check.CheckAgainstAllDifficulties(actor, SkillCheckDifficulty, SkillCheckTrait, target);

		if (TargetGetsResistanceCheck)
		{
			var resistCheck = Gameworld.GetCheck(CheckType.ResistMagicChokePower);
			var resistResults = resistCheck.CheckAgainstAllDifficulties(target, ResistCheckDifficulty, null, actor);
			var outcome = new OpposedOutcome(results, resistResults, SkillCheckDifficulty, ResistCheckDifficulty);
			if (outcome.Outcome != OpposedOutcomeDirection.Proponent)
			{
				actor.OutputHandler.Handle(new EmoteOutput(new Emote(FailEmoteText, actor, actor, target)));
				target.OutputHandler.Send(new EmoteOutput(new Emote(FailEmoteTextTarget, actor, actor, target)));
				return;
			}
		}
		else
		{
			var result = results[SkillCheckDifficulty];
			if (result < MinimumSuccessThreshold)
			{
				actor.OutputHandler.Handle(new EmoteOutput(new Emote(FailEmoteText, actor, actor, target)));
				target.OutputHandler.Send(new EmoteOutput(new Emote(FailEmoteTextTarget, actor, actor, target)));
				return;
			}
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor, target)));
		target.OutputHandler.Send(new EmoteOutput(new Emote(EmoteTextTarget, actor, actor, target)));
		actor.AddEffect(new MagicChoking(actor, this, target), ResistCheckInterval);
	}

	private void UseCommandEnd(ICharacter actor, StringStack command)
	{
		if (!actor.AffectedBy<MagicChoking>(this))
		{
			actor.OutputHandler.Send("You are not choking anyone.");
			return;
		}

		if (command.IsFinished)
		{
			actor.RemoveAllEffects(x => x.IsEffectType<MagicChoking>(this), true);
			return;
		}

		var targets = actor.EffectsOfType<MagicChoking>(x => x.Power == this).Select(x => x.CharacterTarget).ToList();
		var target = targets.GetFromItemListByKeyword(command.PopSpeech(), actor);
		if (target == null)
		{
			actor.OutputHandler.Send("You are not choking anyone like that.");
			return;
		}

		actor.RemoveAllEffects<MagicChoking>(x => x.Power == this && x.CharacterTarget == target, true);
	}

	public override string PowerType => "Choke";

	public override string DatabaseType => "choke";

	public MagicPowerDistance PowerDistance { get; protected set; }
	public string EmoteText { get; protected set; }
	public string FailEmoteText { get; protected set; }
	public string EndPowerEmoteText { get; protected set; }
	public string EmoteTextTarget { get; protected set; }
	public string FailEmoteTextTarget { get; protected set; }
	public string EndPowerEmoteTextTarget { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Difficulty ResistCheckDifficulty { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public string BeginVerb { get; protected set; }
	public string EndVerb { get; protected set; }
	public override IEnumerable<string> Verbs => new[] { BeginVerb, EndVerb };


	public bool TargetGetsResistanceCheck { get; protected set; }
	public string TargetResistanceEmoteText { get; protected set; }
	public string TargetResistanceEmoteTextTarget { get; protected set; }
	public TimeSpan ResistCheckInterval { get; protected set; }

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		actor.OutputHandler.Send(
			$"You are unable to sustain your efforts to maintain the {Name.Colour(Telnet.Magenta)} power.");
		actor.RemoveAllEffects<MagicChoking>(x => x.Power == this, true);
	}

	/// <inheritdoc />
	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Begin Verb: {BeginVerb.ColourCommand()}");
		sb.AppendLine($"End Verb: {EndVerb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Power Distance: {PowerDistance.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Resist Check Difficulty: {ResistCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Resist Check Interval: {ResistCheckInterval.DescribePreciseBrief().ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Emotes:");
		sb.AppendLine();
		sb.AppendLine($"Emote: {EmoteText.ColourCommand()}");
		sb.AppendLine($"Emote Target: {EmoteTextTarget.ColourCommand()}");
		sb.AppendLine($"Fail Emote: {FailEmoteText.ColourCommand()}");
		sb.AppendLine($"Fail Emote Target: {FailEmoteTextTarget.ColourCommand()}");
		sb.AppendLine($"End Emote: {EndPowerEmoteText.ColourCommand()}");
		sb.AppendLine($"End Emote Target: {EndPowerEmoteTextTarget.ColourCommand()}");
		sb.AppendLine($"Resist Emote: {TargetResistanceEmoteText.ColourCommand()}");
		sb.AppendLine($"Resist Emote Target: {TargetResistanceEmoteTextTarget.ColourCommand()}");
	}

	#region Building Commands
	/// <inheritdoc />
	protected override string SubtypeHelpText => @"	#3begin <verb>#0 - sets the verb to activate this power
	#3end <verb>#0 - sets the verb to end this power
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <difficulty>#0 - sets the difficulty of the skill check
	#3threshold <outcome>#0 - sets the minimum outcome for skill success
	#3distance <distance>#0 - sets the distance that this power can be used at
	#3resistdifficulty <difficulty>#0 - sets the difficulty for the target's resist check
	#3resistinterval <seconds>#0 - sets the interval between resistance checks
	#3emote <emote>#0 - sets the emote when this power is used
	#3emotetarget <emote>#0 - sets an echo the target sees when this power is used on them
	#3failemote <emote>#0 - sets the emote when this power is used but skill check failed
	#3failemotetarget <emote>#0 - sets an echo the target sees when this power is failed to be used on them
	#3endemote <emote>#0 - sets an emote when this power is ended
	#3endemotetarget <emote>#0 - sets an echo for the target when this power ends
	#3resistemote <emote>#0 - sets an emote for when this power is resisted by the target
	#3resistemotetarget <emote>#0 - sets an echo for the target when this power is resisted by them

#6Note: For all echoes/emotes above, $0 is the caster, $1 is the target.#0";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
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
			case "distance":
				return BuildingCommandDistance(actor, command);
			case "resistdifficulty":
				return BuildingCommandResistDifficulty(actor, command);
			case "resistinterval":
				return BuildingCommandResistInterval(actor, command);
			case "emote":
				return BuildingCommandEmote(actor, command);
			case "emotetarget":
			case "targetemote":
				return BuilidngCommandEmoteTarget(actor, command);
			case "failemote":
				return BuildingCommandFailEmote(actor, command);
			case "failemotetarget":
				return BuildingCommandFailEmoteTarget(actor, command);
			case "endemote":
				return BuildingCommandEndEmote(actor, command);
			case "endemotetarget":
				return BuildingCommandEndEmoteTarget(actor, command);
			case "resistemote":
				return BuildingCommandResistEmote(actor, command);
			case "resistemotetarget":
				return BuildingCommandResistEmoteTarget(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}


	#region Building Subcommands
	private bool BuildingCommandResistEmoteTarget(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the target resist emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		TargetResistanceEmoteTextTarget = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The target resist emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
	private bool BuildingCommandResistEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the resist emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		TargetResistanceEmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The resist emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
	private bool BuildingCommandEndEmoteTarget(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the end power target emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EndPowerEmoteTextTarget = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The end power target emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
	private bool BuildingCommandEndEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the end power emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EndPowerEmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The end power emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
	private bool BuildingCommandFailEmoteTarget(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the fail target emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FailEmoteTextTarget = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
	private bool BuildingCommandFailEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the fail emote to?");
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
		actor.OutputHandler.Send($"The emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
	private bool BuilidngCommandEmoteTarget(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the target emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EmoteTextTarget = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The target emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the emote to?");
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
		actor.OutputHandler.Send($"The emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandResistInterval(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the interval in seconds between resistance checks?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		if (value <= 0.0)
		{
			ResistCheckDifficulty = Difficulty.Impossible;
			TargetGetsResistanceCheck = false;
			ResistCheckInterval = TimeSpan.Zero;
			Changed = true;
			actor.OutputHandler.Send($"The target will no longer get any kind of resistance check to this power.");
			return true;
		}

		ResistCheckInterval = TimeSpan.FromSeconds(value);

		TargetGetsResistanceCheck = true;
		if (ResistCheckDifficulty == Difficulty.Impossible)
		{
			ResistCheckDifficulty = Difficulty.Insane;
		}

		Changed = true;
		actor.OutputHandler.Send($"The target will now get a resistance check every {ResistCheckInterval.DescribePreciseBrief(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandResistDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty should the resist check for this power be? See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		ResistCheckDifficulty = value;
		Changed = true;
		actor.OutputHandler.Send($"This power's resist check will now be at a difficulty of {value.DescribeColoured()}.");
		return true;
	}

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