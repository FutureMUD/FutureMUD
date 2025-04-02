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
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Magic.Powers;

public class MindAnesthesiaPower : SustainedMagicPower
{
	public override string PowerType => "Anesthesia";
	public static TimeSpan RampInterval { get; private set; } = TimeSpan.FromSeconds(15);

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
					new XAttribute("power", item.Value.Intensity),
					new XAttribute("multiplier", item.Value.Cost),
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
			var costMultiplier = double.Parse(sub.Attribute("multiplier")?.Value ?? "1.0");
			PowerLevelVerbs[verb] = (powerLevel, costMultiplier);
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

			if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
			{
				throw new ApplicationException(
					$"The ApplicabilityProg in the definition XML for power {Id} ({Name}) did not return boolean.");
			}

			if (!prog.MatchesParameters(new[] { ProgVariableTypes.Character }))
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
		var (powerLevel, costMultiplier) = PowerLevelVerbs[powerVerb];

		var target = AcquireTarget(actor, command.PopSpeech(), PowerDistance);
		if (target == null)
		{
			actor.OutputHandler.Send("You can't find any target like that to target.");
			return;
		}

		if (!CanInvokePowerProg.ExecuteBool(actor, target))
		{
			actor.OutputHandler.Send(string.Format(
				WhyCantInvokePowerProg.Execute(actor, target)?.ToString() ??
				"You cannot anaesthetise {0}.", target.HowSeen(actor)));
			return;
		}

		if (target.EffectsOfType<MagicAnesthesia>(x =>
				x.AnesthesiaPower == this && x.CharacterTarget == target && x.TargetIntensity == powerLevel).Any())
		{
			actor.OutputHandler.Send($"You are already anaesthetising {target.HowSeen(actor)} at that intensity.");
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

		actor.AddEffect(new MagicAnesthesia(actor, this, target, powerLevel, costMultiplier),
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
	public Dictionary<string, (double Intensity, double Cost)> PowerLevelVerbs { get; } = new();
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
			sb.AppendLine($"Begin Verb (@{verb.Value.Intensity.ToString("N3", actor).ColourValue()}/x{verb.Value.Cost.ToString("N3", actor).ColourValue()}): {verb.Key.ColourCommand()}");
		}
		sb.AppendLine($"End Verb: {CancelVerb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Power Distance: {PowerDistance.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Resist Check Difficulty: {ResistCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Resist Check Interval: {ResistCheckInterval.DescribePreciseBrief().ColourValue()}");
		sb.AppendLine($"Ramp Rate Per Tick: {RampRatePerTick.ToString("P3", actor).ColourValue()}");
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
	#3ramprate <time>#0 - how fast the effect will increase in intensity
	#3rampamount <%>#0 - what percentage of maximum to increase per tick
	#3emote <emote>#0 - sets the emote when this power is used
	#3emotetarget <emote>#0 - sets an echo the target sees when this power is used on them
	#3failemote <emote>#0 - sets the emote when this power is used but skill check failed
	#3failemotetarget <emote>#0 - sets an echo the target sees when this power is failed to be used on them
	#3endemote <emote>#0 - sets an emote when this power is ended
	#3endemotetarget <emote>#0 - sets an echo for the target when this power ends
	#3resistemote <emote>#0 - sets an emote for when this power is resisted by the target
	#3resistemotetarget <emote>#0 - sets an echo for the target when this power is resisted by them

#6Note: For all echoes/emotes above, $0 is the caster, $1 is the target.#0
#6Note: Anesthesia intensity >1 = unconscious, >2.5 = breathing stops#0";

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
			case "removeverb":
				return BuildingCommandRemoveVerb(actor, command);
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
			case "ramp":
			case "ramprate":
				return BuildingCommandRampRate(actor, command);
			case "rampamount":
				return BuildingCommandRampAmount(actor, command);
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

	private bool BuildingCommandRampAmount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the amount of anesthesia effect added per tick as this effect ramps up?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value) || value <= 0.0 || value > 1.0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage between {0.ToStringP2Colour(actor)} and {1.ToStringP2Colour(actor)}.");
			return false;
		}

		RampRatePerTick = value;
		Changed = true;
		actor.OutputHandler.Send($"The anesthesia effect will now increase by {value.ToStringP2Colour(actor)} of its maximum intensity per tick.");
		return true;
	}

	private bool BuildingCommandRampRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid timespan.");
			return false;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out var ts))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid timespan.");
			return false;
		}

		RampInterval = ts.AsTimeSpan();
		Changed = true;
		actor.OutputHandler.Send($"The anesthesia effect will now increase in intensity every {ts.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandRemoveVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which begin verb do you want to remove?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();
		if (!PowerLevelVerbs.ContainsKey(verb))
		{
			actor.OutputHandler.Send($"There is no such begin verb as {verb.ColourCommand()}.");
			return false;
		}

		PowerLevelVerbs.Remove(verb);
		InvocationCosts.Remove(verb);
		Changed = true;
		actor.OutputHandler.Send($"This power no longer has the {verb.ColourCommand()} invocation verb.");
		return true;
	}

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
		if (PowerLevelVerbs.Keys.Any(x => x.EqualTo(verb)))
		{
			actor.OutputHandler.Send("The begin and end verbs cannot be the same.");
			return false;
		}

		var costs = InvocationCosts[CancelVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(CancelVerb);
		CancelVerb = verb;
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

		var verb = command.PopSpeech().ToLowerInvariant();
		if (CancelVerb.EqualTo(verb))
		{
			actor.OutputHandler.Send("The begin and end verbs cannot be the same.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What anesthetic intensity should this verb usage generate?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the cost multiplier for this level of the power?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var cost))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		PowerLevelVerbs[verb] = (value, cost);
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to begin the power at {value.ToString("P2", actor).ColourValue()} intensity with a {cost.ToString("P2").ColourValue()} sustain cost multiplier.");
		return true;
	}
	#endregion Building Subcommands
	#endregion Building Commands
}