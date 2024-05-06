using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public class MindAnesthesiaPower : SustainedMagicPower
{
	public override string PowerType => "Anesthesia";
	public static TimeSpan RampInterval { get; } = TimeSpan.FromSeconds(15);

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("anesthesia", (power, gameworld) => new MindAnesthesiaPower(power, gameworld));
	}

    /// <inheritdoc />
    protected override XElement SaveDefinition()
    {
        var definition = new XElement("Definition",
            new XElement("BeginVerbs", 
				from item in PowerLevelVerbs
				select new XElement("Verb",
					new XAttribute("power", item.Value),
					item.Key
                )
            ),
            new XElement("CancelVerb", CancelVerb),
            new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
            new XElement("SkillCheckTrait", SkillCheckTrait.Id),
            new XElement("ApplicabilityProg", AppliesProg.Id),
            new XElement("EmoteText", new XCData(EmoteText)),
            new XElement("EmoteTextTarget", new XCData(EmoteTextTarget)),
            new XElement("FailEmoteText", new XCData(FailEmoteText)),
            new XElement("FailEmoteTextTarget", new XCData(FailEmoteTextTarget)),
            new XElement("EndPowerEmoteText", new XCData(EndPowerEmoteText)),
            new XElement("EndPowerEmoteTextTarget", new XCData(EndPowerEmoteTextTarget)),
            new XElement("TargetResistanceEmoteText", new XCData(TargetResistanceEmoteText)),
            new XElement("TargetResistanceEmoteTextTarget", new XCData(TargetResistanceEmoteTextTarget)),
            new XElement("RampRatePerTick", RampRatePerTick),
            new XElement("TickLength", TickLength.TotalSeconds),
            new XElement("PowerDistance", (int)PowerDistance),
			new XElement("ResistCheckDifficulty", (int)ResistCheckDifficulty),
            new XElement("ResistCheckInterval", ResistCheckInterval.TotalSeconds)
        );
        SaveSustainedDefinition(definition);
        return definition;
    }

    protected MindAnesthesiaPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("BeginVerbs");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no BeginVerbs in the definition XML for power {Id} ({Name}).");
		}

		foreach (var sub in element.Elements())
		{
			var verb = sub.Value;
			var powerLevel = double.Parse(sub.Attribute("power")?.Value ??
			                              throw new ApplicationException(
				                              $"There was a BeginVerb with no power attribute in the definition XML for power {Id} ({Name})."));
			PowerLevelVerbs[verb] = powerLevel;
		}

		element = root.Element("CancelVerb");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no CancelVerb in the definition XML for power {Id} ({Name}).");
		}

		CancelVerb = element.Value;

		element = root.Element("RampRatePerTick");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no RampRatePerTick in the definition XML for power {Id} ({Name}).");
		}

		RampRatePerTick = double.Parse(element.Value);

		element = root.Element("TickLength");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no TickLength in the definition XML for power {Id} ({Name}).");
		}

		TickLength = TimeSpan.FromSeconds(double.Parse(element.Value));

		element = root.Element("ApplicabilityProg");
		if (element != null)
		{
			var prog = long.TryParse(element.Value, out var lvalue)
				? Gameworld.FutureProgs.Get(lvalue)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (prog == null)
			{
				throw new ApplicationException(
					$"There was an invalid ApplicabilityProg in the definition XML for power {Id} ({Name}).");
			}

			if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
			{
				throw new ApplicationException(
					$"The ApplicabilityProg in the definition XML for power {Id} ({Name}) did not return boolean.");
			}

			if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
			{
				throw new ApplicationException(
					$"The ApplicabilityProg in the definition XML for power {Id} ({Name}) was not compatible with a single character parameter.");
			}

			AppliesProg = prog;
		}

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
			ResistCheckInterval = TimeSpan.MaxValue;
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

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		actor.OutputHandler.Send(
			$"You are unable to sustain your efforts to maintain the {Name.Colour(Telnet.Magenta)} power.");
		actor.RemoveAllEffects<MagicAnesthesia>(x => x.AnesthesiaPower == this, true);
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

		if (CancelVerb.StartsWith(verb, StringComparison.InvariantCultureIgnoreCase))
		{
			UseCommandCancel(actor, command);
			return;
		}

		if (!PowerLevelVerbs.Any(x => x.Key.StartsWith(verb, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.OutputHandler.Send(
				$"You can either {PowerLevelVerbs.OrderBy(x => x.Value).Select(x => x.Key.ColourCommand()).ListToString(conjunction: "or ")} to begin affecting someone or {CancelVerb.ColourCommand()} to end it.");
			return;
		}

		var powerVerb = PowerLevelVerbs.Keys.FirstOrDefault(x => x.EqualTo(verb)) ??
		                PowerLevelVerbs.Keys.First(x =>
			                x.StartsWith(verb, StringComparison.InvariantCultureIgnoreCase));
		var powerLevel = PowerLevelVerbs[powerVerb];

		var target = AcquireTarget(actor, command.PopSpeech(), PowerDistance);
		if (target == null)
		{
			actor.OutputHandler.Send("You can't find any target like that to target.");
			return;
		}

		if ((bool?)CanInvokePowerProg.Execute(actor, target) == false)
		{
			actor.OutputHandler.Send(string.Format(
				WhyCantInvokePowerProg.Execute(actor, target)?.ToString() ??
				"You cannot anesthetise {0}.", target.HowSeen(actor)));
			return;
		}

		if (target.EffectsOfType<MagicAnesthesia>(x =>
			    x.AnesthesiaPower == this && x.CharacterTarget == target && x.TargetIntensity == powerLevel).Any())
		{
			actor.OutputHandler.Send($"You are already anesthetising {target.HowSeen(actor)} at that intensity.");
			return;
		}

		ConsumePowerCosts(actor, powerVerb);

		var check = Gameworld.GetCheck(CheckType.MagicAnesthesiaPower);
		var results = check.CheckAgainstAllDifficulties(actor, SkillCheckDifficulty, SkillCheckTrait, target);

		if (TargetGetsResistanceCheck)
		{
			var resistCheck = Gameworld.GetCheck(CheckType.ResistMagicAnesthesiaPower);
			var resistResults = resistCheck.CheckAgainstAllDifficulties(target, ResistCheckDifficulty, null, actor);
			var outcome = new OpposedOutcome(results, resistResults, SkillCheckDifficulty, ResistCheckDifficulty);
			if (outcome.Outcome != OpposedOutcomeDirection.Proponent)
			{
				if (!string.IsNullOrWhiteSpace(FailEmoteText))
				{
					actor.OutputHandler.Handle(new EmoteOutput(new Emote(FailEmoteText, actor, actor, target)));
				}

				if (!string.IsNullOrWhiteSpace(FailEmoteTextTarget))
				{
					target.OutputHandler.Send(new EmoteOutput(new Emote(FailEmoteTextTarget, actor, actor, target)));
				}

				return;
			}
		}
		else
		{
			var result = results[SkillCheckDifficulty];
			if (result < MinimumSuccessThreshold)
			{
				if (!string.IsNullOrWhiteSpace(FailEmoteText))
				{
					actor.OutputHandler.Handle(new EmoteOutput(new Emote(FailEmoteText, actor, actor, target)));
				}

				if (!string.IsNullOrWhiteSpace(FailEmoteTextTarget))
				{
					target.OutputHandler.Send(new EmoteOutput(new Emote(FailEmoteTextTarget, actor, actor, target)));
				}

				return;
			}
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor, target)));
		if (!string.IsNullOrWhiteSpace(EmoteTextTarget))
		{
			target.OutputHandler.Send(new EmoteOutput(new Emote(EmoteTextTarget, actor, actor, target)));
		}

		actor.AddEffect(new MagicAnesthesia(actor, this, target, powerLevel),
			TargetGetsResistanceCheck ? ResistCheckInterval : RampInterval);
		target.Body.CheckDrugTick();
	}

	private void UseCommandCancel(ICharacter actor, StringStack command)
	{
		if (!actor.AffectedBy<MagicAnesthesia>(this))
		{
			actor.OutputHandler.Send("You are not anesthetising anyone.");
			return;
		}

		if (command.IsFinished)
		{
			actor.RemoveAllEffects(x => x.IsEffectType<MagicAnesthesia>(this), true);
			return;
		}

		var targets = actor.EffectsOfType<MagicAnesthesia>(x => x.AnesthesiaPower == this)
		                   .Select(x => x.CharacterTarget).ToList();
		var target = targets.GetFromItemListByKeyword(command.PopSpeech(), actor);
		if (target == null)
		{
			actor.OutputHandler.Send("You are not anesthetising anyone like that.");
			return;
		}

		actor.RemoveAllEffects<MagicAnesthesia>(x => x.AnesthesiaPower == this && x.CharacterTarget == target, true);
	}

	public string CancelVerb { get; protected set; }
	public Dictionary<string, double> PowerLevelVerbs { get; } = new();
	public double RampRatePerTick { get; protected set; }
	public TimeSpan TickLength { get; protected set; }
	public IFutureProg AppliesProg { get; protected set; }
	public override IEnumerable<string> Verbs => PowerLevelVerbs.Keys.AsEnumerable().Append(CancelVerb);
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
	public bool TargetGetsResistanceCheck { get; protected set; }
	public string TargetResistanceEmoteText { get; protected set; }
	public string TargetResistanceEmoteTextTarget { get; protected set; }
	public TimeSpan ResistCheckInterval { get; protected set; }

    protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
    {
        foreach (var verb in PowerLevelVerbs)
        {
            sb.AppendLine($"Begin Verb (@{verb.Value.ToString("N3", actor).ColourValue()}): {verb.Key.ColourCommand()}");
        }
        sb.AppendLine($"End Verb: {CancelVerb.ColourCommand()}");
        sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
        sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
        sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
        sb.AppendLine($"Power Distance: {PowerDistance.DescribeEnum().ColourValue()}");
        sb.AppendLine($"Resist Check Difficulty: {ResistCheckDifficulty.DescribeColoured()}");
        sb.AppendLine($"Resist Check Interval: {ResistCheckInterval.DescribePreciseBrief().ColourValue()}");
        sb.AppendLine($"Ramp Rate Per Tick: {RampRatePerTick.ToString("N3", actor).ColourValue()}");
        sb.AppendLine($"Tick Interval: {TickLength.DescribePreciseBrief().ColourValue()}");
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