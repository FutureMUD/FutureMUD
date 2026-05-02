#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
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

public class MindConcealPower : SustainedMagicPower
{
	public override string PowerType => "Mind Conceal";
	public override string DatabaseType => "mindconceal";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindconceal", (power, gameworld) => new MindConcealPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("mindconceal", (gameworld, school, name, actor, command) =>
		{
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

			return new MindConcealPower(gameworld, school, name, skill);
		});
	}

	private MindConcealPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(
		gameworld, school, name)
	{
		Blurb = "Conceal your identity from mental contacts";
		_showHelpText =
			$"You can use {school.SchoolVerb.ToUpperInvariant()} MINDCONCEAL to sustain a hidden mental identity, and {school.SchoolVerb.ToUpperInvariant()} ENDMINDCONCEAL to reveal yourself again.";
		BeginVerb = "mindconceal";
		EndVerb = "endmindconceal";
		SkillCheckTrait = trait;
		SkillCheckDifficulty = Difficulty.Easy;
		MinimumSuccessThreshold = Outcome.MinorFail;
		ConcentrationPointsToSustain = 1.0;
		UnknownIdentityDescription = "an unknown presence";
		AuditDifficultyStages = 1;
		IncludeSubschools = true;
		AppliesToCharacterProg = Gameworld.AlwaysTrueProg;
		EmoteForBegin = "";
		EmoteForBeginSelf = "You veil your mental signature.";
		EmoteForEnd = "";
		EmoteForEndSelf = "You let the veil fall away from your mental signature.";
		BeginWhenAlreadySustainingError = "You are already concealing your mental identity.";
		EndWhenNotSustainingError = "You are not currently concealing your mental identity.";
		DoDatabaseInsert();
	}

	protected MindConcealPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		BeginVerb = root.Element("BeginVerb")?.Value.ToLowerInvariant() ?? "mindconceal";
		EndVerb = root.Element("EndVerb")?.Value.ToLowerInvariant() ?? "endmindconceal";
		SkillCheckDifficulty = (Difficulty)int.Parse(root.Element("SkillCheckDifficulty")?.Value ??
		                                              ((int)Difficulty.Easy).ToString());
		var skillId = long.Parse(root.Element("SkillCheckTrait")!.Value);
		SkillCheckTrait = Gameworld.Traits.Get(skillId) ??
		                  throw new ApplicationException(
			                  $"The MindConcealPower #{Id} ({Name}) had a SkillCheckTrait element that pointed to invalid trait #{skillId}.");
		MinimumSuccessThreshold = (Outcome)int.Parse(root.Element("MinimumSuccessThreshold")?.Value ??
		                                             ((int)Outcome.MinorFail).ToString());
		UnknownIdentityDescription = root.Element("UnknownIdentityDescription")?.Value ?? "an unknown presence";
		AuditDifficultyStages = int.Parse(root.Element("AuditDifficultyStages")?.Value ?? "1");
		IncludeSubschools = bool.Parse(root.Element("IncludeSubschools")?.Value ?? "true");
		var progText = root.Element("AppliesToCharacterProg")?.Value ?? Gameworld.AlwaysTrueProg.Id.ToString();
		AppliesToCharacterProg = Gameworld.FutureProgs.GetByIdOrName(progText) ??
		                         throw new ApplicationException(
			                         $"The MindConcealPower #{Id} ({Name}) had an AppliesToCharacterProg element that pointed to invalid prog {progText}.");
		EmoteForBegin = root.Element("EmoteForBegin")?.Value ?? "";
		EmoteForBeginSelf = root.Element("EmoteForBeginSelf")?.Value ?? "You veil your mental signature.";
		EmoteForEnd = root.Element("EmoteForEnd")?.Value ?? "";
		EmoteForEndSelf = root.Element("EmoteForEndSelf")?.Value ??
		                   "You let the veil fall away from your mental signature.";
		BeginWhenAlreadySustainingError = root.Element("BeginWhenAlreadySustainingError")?.Value ??
		                                  "You are already concealing your mental identity.";
		EndWhenNotSustainingError = root.Element("EndWhenNotSustainingError")?.Value ??
		                            "You are not currently concealing your mental identity.";
	}

	protected override XElement SaveDefinition()
	{
		var definition = new XElement("Definition",
			new XElement("BeginVerb", BeginVerb),
			new XElement("EndVerb", EndVerb),
			new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id),
			new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
			new XElement("AppliesToCharacterProg", AppliesToCharacterProg.Id),
			new XElement("UnknownIdentityDescription", new XCData(UnknownIdentityDescription)),
			new XElement("AuditDifficultyStages", AuditDifficultyStages),
			new XElement("IncludeSubschools", IncludeSubschools),
			new XElement("EmoteForBegin", new XCData(EmoteForBegin)),
			new XElement("EmoteForBeginSelf", new XCData(EmoteForBeginSelf)),
			new XElement("EmoteForEnd", new XCData(EmoteForEnd)),
			new XElement("EmoteForEndSelf", new XCData(EmoteForEndSelf)),
			new XElement("BeginWhenAlreadySustainingError", new XCData(BeginWhenAlreadySustainingError)),
			new XElement("EndWhenNotSustainingError", new XCData(EndWhenNotSustainingError))
		);
		AddBaseDefinition(definition);
		SaveSustainedDefinition(definition);
		return definition;
	}

	public bool AppliesToSchool(IMagicSchool school)
	{
		return school == School || IncludeSubschools && school.IsChildSchool(School);
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
			UseCommandBegin(actor);
			return;
		}

		UseCommandEnd(actor);
	}

	private void UseCommandBegin(ICharacter actor)
	{
		if (actor.EffectsOfType<MindConcealmentEffect>().Any(x => x.PowerOrigin == this))
		{
			actor.OutputHandler.Send(BeginWhenAlreadySustainingError);
			return;
		}

		if (CanInvokePowerProg.ExecuteBool(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor)?.ToString() ?? "You cannot use that power.");
			return;
		}

		var check = Gameworld.GetCheck(CheckType.MagicTelepathyCheck);
		var result = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait);
		if (result < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Send("You fail to conceal your mental identity.");
			ConsumePowerCosts(actor, BeginVerb);
			return;
		}

		actor.OutputHandler.Send(EmoteForBeginSelf);
		if (!string.IsNullOrWhiteSpace(EmoteForBegin))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteForBegin, actor, actor),
				flags: OutputFlags.SuppressSource), OutputRange.Local);
		}

		actor.AddEffect(new MindConcealmentEffect(actor, this), GetDuration(result.SuccessDegrees()));
		ConsumePowerCosts(actor, BeginVerb);
	}

	private void UseCommandEnd(ICharacter actor)
	{
		var effects = actor.EffectsOfType<MindConcealmentEffect>().Where(x => x.PowerOrigin == this).ToList();
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

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		foreach (var effect in actor.EffectsOfType<MindConcealmentEffect>().Where(x => x.PowerOrigin == this).ToList())
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
	public IFutureProg AppliesToCharacterProg { get; protected set; }
	public string UnknownIdentityDescription { get; protected set; }
	public int AuditDifficultyStages { get; protected set; }
	public bool IncludeSubschools { get; protected set; }
	public string EmoteForBegin { get; protected set; }
	public string EmoteForBeginSelf { get; protected set; }
	public string EmoteForEnd { get; protected set; }
	public string EmoteForEndSelf { get; protected set; }
	public string BeginWhenAlreadySustainingError { get; protected set; }
	public string EndWhenNotSustainingError { get; protected set; }

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Begin Verb: {BeginVerb.ColourCommand()}");
		sb.AppendLine($"End Verb: {EndVerb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Unknown Identity Desc: {UnknownIdentityDescription.ColourCharacter()}");
		sb.AppendLine($"Audit Difficulty Stages: {AuditDifficultyStages.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Includes Subschools: {IncludeSubschools.ToColouredString()}");
		sb.AppendLine($"Applies Character Prog: {AppliesToCharacterProg.MXPClickableFunctionName()}");
		sb.AppendLine();
		sb.AppendLine("Emotes:");
		sb.AppendLine();
		sb.AppendLine($"Begin Emote: {EmoteForBegin.ColourCommand()}");
		sb.AppendLine($"Begin Self Emote: {EmoteForBeginSelf.ColourCommand()}");
		sb.AppendLine($"End Emote: {EmoteForEnd.ColourCommand()}");
		sb.AppendLine($"End Self Emote: {EmoteForEndSelf.ColourCommand()}");
	}

	protected override string SubtypeHelpText => @"	#3begin <verb>#0 - sets the verb to activate this power
	#3end <verb>#0 - sets the verb to end this power
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <difficulty>#0 - sets the difficulty of the skill check
	#3threshold <outcome>#0 - sets the minimum outcome for skill success
	#3unknown <desc>#0 - sets the replacement identity shown while concealed
	#3audit <stages>#0 - sets extra difficulty stages for audit/trace attempts
	#3subschools#0 - toggles whether the concealment covers child schools
	#3applies <prog>#0 - sets a prog controlling which observers are affected
	#3beginemote <emote>#0 - sets the emote for beginning this power
	#3beginemoteself <emote>#0 - sets the self emote for beginning this power
	#3endemote <emote>#0 - sets the emote for ending this power
	#3endemoteself <emote>#0 - sets the self emote for ending this power";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "begin":
			case "beginverb":
			case "verb":
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
			case "unknown":
				return BuildingCommandUnknown(actor, command);
			case "audit":
				return BuildingCommandAudit(actor, command);
			case "subschools":
				return BuildingCommandSubschools(actor);
			case "applies":
			case "appliesprog":
				return BuildingCommandApplies(actor, command);
			case "beginemote":
				return BuildingCommandBeginEmote(actor, command);
			case "beginemoteself":
				return BuildingCommandBeginEmoteSelf(actor, command);
			case "endemote":
				return BuildingCommandEndEmote(actor, command);
			case "endemoteself":
				return BuildingCommandEndEmoteSelf(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandBegin(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What verb should activate this power?");
			return false;
		}

		BeginVerb = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send($"This power is now activated with {BeginVerb.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEnd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What verb should end this power?");
			return false;
		}

		EndVerb = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send($"This power is now ended with {EndVerb.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill or attribute should be used for this power?");
			return false;
		}

		var skill = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (skill is null)
		{
			actor.OutputHandler.Send("There is no such skill or attribute.");
			return false;
		}

		SkillCheckTrait = skill;
		Changed = true;
		actor.OutputHandler.Send($"This power now uses {skill.Name.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var value))
		{
			actor.OutputHandler.Send("You must specify a valid difficulty.");
			return false;
		}

		SkillCheckDifficulty = value;
		Changed = true;
		actor.OutputHandler.Send($"The activation check is now {value.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum<Outcome>(out var value))
		{
			actor.OutputHandler.Send("You must specify a valid outcome.");
			return false;
		}

		MinimumSuccessThreshold = value;
		Changed = true;
		actor.OutputHandler.Send($"The power user must now achieve {value.DescribeColour()}.");
		return true;
	}

	private bool BuildingCommandUnknown(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What unknown identity description should observers see?");
			return false;
		}

		UnknownIdentityDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"Concealed observers now see {UnknownIdentityDescription.ColourCharacter()}.");
		return true;
	}

	private bool BuildingCommandAudit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("How many difficulty stages should this add to audit attempts?");
			return false;
		}

		AuditDifficultyStages = value;
		Changed = true;
		actor.OutputHandler.Send($"Audit attempts are now shifted by {value.ToString("N0", actor).ColourValue()} stages.");
		return true;
	}

	private bool BuildingCommandSubschools(ICharacter actor)
	{
		IncludeSubschools = !IncludeSubschools;
		Changed = true;
		actor.OutputHandler.Send($"This concealment will {IncludeSubschools.NowNoLonger()} apply to child schools.");
		return true;
	}

	private bool BuildingCommandApplies(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control which observers are affected?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Character, ProgVariableTypes.Character]
			]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AppliesToCharacterProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This concealment now uses {prog.MXPClickableFunctionName()} to decide observers.");
		return true;
	}

	private bool BuildingCommandBeginEmote(ICharacter actor, StringStack command)
	{
		EmoteForBegin = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send("The begin emote has been updated.");
		return true;
	}

	private bool BuildingCommandBeginEmoteSelf(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the caster see when beginning this power?");
			return false;
		}

		EmoteForBeginSelf = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send("The begin self emote has been updated.");
		return true;
	}

	private bool BuildingCommandEndEmote(ICharacter actor, StringStack command)
	{
		EmoteForEnd = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send("The end emote has been updated.");
		return true;
	}

	private bool BuildingCommandEndEmoteSelf(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the caster see when ending this power?");
			return false;
		}

		EmoteForEndSelf = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send("The end self emote has been updated.");
		return true;
	}
}
