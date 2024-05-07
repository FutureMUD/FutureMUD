using System;
using System.Collections.Generic;
using System.Linq;
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

		if ((bool?)CanInvokePowerProg.Execute(actor, target) == false)
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

}