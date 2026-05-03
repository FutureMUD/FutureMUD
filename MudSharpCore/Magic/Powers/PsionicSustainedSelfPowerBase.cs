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

public abstract class PsionicSustainedSelfPowerBase : SustainedMagicPower
{
	protected PsionicSustainedSelfPowerBase(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		BeginVerb = root.Element("BeginVerb")?.Value ?? throw new ApplicationException($"Missing BeginVerb in power {Id} ({Name}).");
		EndVerb = root.Element("EndVerb")?.Value ?? throw new ApplicationException($"Missing EndVerb in power {Id} ({Name}).");
		SkillCheckDifficulty = (Difficulty)int.Parse(root.Element("SkillCheckDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		MinimumSuccessThreshold = (Outcome)int.Parse(root.Element("MinimumSuccessThreshold")?.Value ?? ((int)Outcome.MinorPass).ToString());
		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(root.Element("SkillCheckTrait")?.Value ?? "0")) ??
		                  throw new ApplicationException($"Invalid SkillCheckTrait in power {Id} ({Name}).");
		BeginEmote = root.Element("BeginEmote")?.Value ?? "You gather your psionic focus.";
		EndEmote = root.Element("EndEmote")?.Value ?? "You release your psionic focus.";
		FailEmote = root.Element("FailEmote")?.Value ?? "Your focus slips away from you.";
	}

	protected PsionicSustainedSelfPowerBase(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait)
		: base(gameworld, school, name)
	{
		IsPsionic = true;
		BeginVerb = DefaultBeginVerb;
		EndVerb = DefaultEndVerb;
		SkillCheckTrait = trait;
		SkillCheckDifficulty = Difficulty.Normal;
		MinimumSuccessThreshold = Outcome.MinorPass;
		ConcentrationPointsToSustain = 1.0;
		SustainPenalty = Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel") * -1.0;
		DetectableWithDetectMagic = Difficulty.Normal;
		BeginEmote = "You gather your psionic focus.";
		EndEmote = "You release your psionic focus.";
		FailEmote = "Your focus slips away from you.";
	}

	protected abstract string DefaultBeginVerb { get; }
	protected abstract string DefaultEndVerb { get; }
	public string BeginVerb { get; protected set; }
	public string EndVerb { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public string BeginEmote { get; protected set; }
	public string EndEmote { get; protected set; }
	public string FailEmote { get; protected set; }
	public override IEnumerable<string> Verbs => [BeginVerb, EndVerb];

	protected XElement SaveSustainedSelfDefinition(params object[] additional)
	{
		var definition = new XElement("Definition",
			new XElement("BeginVerb", BeginVerb),
			new XElement("EndVerb", EndVerb),
			new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id),
			new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
			new XElement("BeginEmote", new XCData(BeginEmote)),
			new XElement("EndEmote", new XCData(EndEmote)),
			new XElement("FailEmote", new XCData(FailEmote))
		);
		foreach (var item in additional)
		{
			definition.Add(item);
		}

		AddBaseDefinition(definition);
		SaveSustainedDefinition(definition);
		return definition;
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (verb.EqualTo(BeginVerb))
		{
			UseBegin(actor);
			return;
		}

		UseEnd(actor);
	}

	private void UseBegin(ICharacter actor)
	{
		if (ActiveEffects(actor).Any())
		{
			actor.OutputHandler.Send("You are already sustaining that power.");
			return;
		}

		var (truth, missing) = CanAffordToInvokePower(actor, BeginVerb);
		if (!truth)
		{
			actor.OutputHandler.Send($"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		if (CanInvokePowerProg.ExecuteBool(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor)?.ToString() ?? "You cannot use that power right now.");
			return;
		}

		if (!HandleGeneralUseRestrictions(actor))
		{
			return;
		}

		var outcome = Gameworld.GetCheck(CheckType.MagicTelepathyCheck).Check(actor, SkillCheckDifficulty, SkillCheckTrait);
		if (outcome < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(FailEmote, actor, actor)));
			return;
		}

		actor.AddEffect(CreateEffect(actor), GetDuration(outcome.SuccessDegrees()));
		actor.OutputHandler.Send(new EmoteOutput(new Emote(BeginEmote, actor, actor)));
		ConsumePowerCosts(actor, BeginVerb);
	}

	private void UseEnd(ICharacter actor)
	{
		var effect = ActiveEffects(actor).FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not currently sustaining that power.");
			return;
		}

		actor.RemoveEffect(effect, true);
		actor.OutputHandler.Send(new EmoteOutput(new Emote(EndEmote, actor, actor)));
		ConsumePowerCosts(actor, EndVerb);
	}

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		foreach (var effect in ActiveEffects(actor).ToList())
		{
			actor.RemoveEffect(effect, true);
		}
	}

	protected abstract IEffect CreateEffect(ICharacter actor);
	protected abstract IEnumerable<IEffect> ActiveEffects(ICharacter actor);

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Begin Verb: {BeginVerb.ColourCommand()}");
		sb.AppendLine($"End Verb: {EndVerb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Begin Emote: {BeginEmote.ColourCommand()}");
		sb.AppendLine($"End Emote: {EndEmote.ColourCommand()}");
		sb.AppendLine($"Fail Emote: {FailEmote.ColourCommand()}");
		ShowSubtypeDetails(actor, sb);
	}

	protected virtual void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
	}

	protected override string SubtypeHelpText => @"	#3begin <verb>#0 - sets the activation verb
	#3end <verb>#0 - sets the ending verb
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <difficulty>#0 - sets the skill check difficulty
	#3threshold <outcome>#0 - sets the minimum success threshold
	#3beginemote <emote>#0 - sets the activation emote
	#3endemote <emote>#0 - sets the ending emote
	#3failemote <emote>#0 - sets the failure emote";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "begin":
			case "beginverb":
				return BuildingCommandBegin(actor, command);
			case "end":
			case "endverb":
				return BuildingCommandEnd(actor, command);
			case "skill":
			case "trait":
				return BuildingCommandSkill(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "threshold":
				return BuildingCommandThreshold(actor, command);
			case "beginemote":
				return BuildingCommandEmote(actor, command, "begin");
			case "endemote":
				return BuildingCommandEmote(actor, command, "end");
			case "failemote":
				return BuildingCommandEmote(actor, command, "fail");
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandBegin(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should activate this power?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();
		if (verb.EqualTo(EndVerb))
		{
			actor.OutputHandler.Send("The begin and end verbs cannot match.");
			return false;
		}

		var costs = InvocationCosts[BeginVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(BeginVerb);
		BeginVerb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This power now begins with {BeginVerb.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEnd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should end this power?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();
		if (verb.EqualTo(BeginVerb))
		{
			actor.OutputHandler.Send("The begin and end verbs cannot match.");
			return false;
		}

		var costs = InvocationCosts[EndVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(EndVerb);
		EndVerb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This power now ends with {EndVerb.ColourCommand()}.");
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

	private bool BuildingCommandEmote(ICharacter actor, StringStack command, string which)
	{
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

		switch (which)
		{
			case "begin":
				BeginEmote = command.SafeRemainingArgument;
				break;
			case "end":
				EndEmote = command.SafeRemainingArgument;
				break;
			case "fail":
				FailEmote = command.SafeRemainingArgument;
				break;
		}

		Changed = true;
		actor.OutputHandler.Send($"The {which} emote is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
}

