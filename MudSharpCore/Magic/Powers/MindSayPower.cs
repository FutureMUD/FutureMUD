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
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Construction;

namespace MudSharp.Magic.Powers;

public class MindSayPower : MagicPowerBase
{
	public override string PowerType => "Mind Say";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindsay", (power, gameworld) => new MindSayPower(power, gameworld));
	}

    protected override XElement SaveDefinition()
    {
        var definition = new XElement("Definition",
            new XElement("SayVerb", SayVerb),
            new XElement("TellVerb", TellVerb),
            new XElement("EmoteText", new XCData(EmoteText)),
            new XElement("FailEmoteText", new XCData(FailEmoteText)),
            new XElement("TargetEmoteText", new XCData(TargetEmoteText)),
            new XElement("UnknownIdentityDescription", new XCData(UnknownIdentityDescription)),
            new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
            new XElement("UseAccent", UseAccent),
            new XElement("UseLanguage", UseLanguage),
            new XElement("TargetCanSeeIdentityProg", TargetCanSeeIdentityProg.Id),
            new XElement("SkillCheckTrait", SkillCheckTrait.Id)
        );
        return definition;
    }

    protected MindSayPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("UnknownIdentityDescription");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no UnknownIdentityDescription in the definition XML for power {Id} ({Name}).");
		}

		UnknownIdentityDescription = element.Value;

		element = root.Element("TargetCanSeeIdentityProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no TargetCanSeeIdentityProg in the definition XML for power {Id} ({Name}).");
		}

		TargetCanSeeIdentityProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("SayVerb");
		if (element == null)
		{
			throw new ApplicationException($"There was no SayVerb in the definition XML for power {Id} ({Name}).");
		}

		SayVerb = element.Value;

		element = root.Element("TellVerb");
		if (element == null)
		{
			throw new ApplicationException($"There was no TellVerb in the definition XML for power {Id} ({Name}).");
		}

		TellVerb = element.Value;

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

		ConnectMindEffect effect = null;
		if (verb.EqualTo(TellVerb))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send($"Which of your mental connections do you want to {TellVerb} something to?");
				return;
			}

			var targetCharacter = actor.EffectsOfType<ConnectMindEffect>().Where(x => x.School == School)
			                           .Select(x => x.TargetCharacter)
			                           .GetFromItemListByKeyword(command.PopSpeech(), actor);
			if (targetCharacter == null)
			{
				actor.OutputHandler.Send(
					$"You aren't connected to the mind of anyone like that via {School.Name.Colour(School.PowerListColour)}.");
				return;
			}

			effect = actor.EffectsOfType<ConnectMindEffect>()
			              .First(x => x.School == School && x.TargetCharacter == targetCharacter);
		}
		else
		{
			effect = actor.EffectsOfType<ConnectMindEffect>().FirstOrDefault(x => x.School == School);
			if (effect == null)
			{
				actor.OutputHandler.Send(
					$"Your mind isn't connected to the mind of anyone else via {School.Name.Colour(School.PowerListColour)}.");
				return;
			}
		}

		if ((bool?)CanInvokePowerProg.Execute(actor, effect.TargetCharacter) == false)
		{
			actor.OutputHandler.Send(string.Format(
				WhyCantInvokePowerProg.Execute(actor, effect.TargetCharacter)?.ToString() ??
				"You cannot send any messages to {0}.", GetAppropriateHowSeen(effect.TargetCharacter, actor)));
			return;
		}

		var check = Gameworld.GetCheck(CheckType.MindSayPower);
		var outcome = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait, effect.TargetCharacter);
		if (outcome < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Send(
				new EmoteOutput(new Emote(FailEmoteText, actor, actor, effect.TargetCharacter)));
			return;
		}

		var text = command.RemainingArgument;
		if (text.Length > Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.OutputHandler.Send($"You can't send so much text at once. Keep it under {Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		if (UseLanguage)
		{
			var langInfo = new PsychicLanguageInfo(actor.CurrentLanguage, UseAccent ? actor.CurrentAccent : null,
				text, outcome, actor);
			effect.TargetCharacter.OutputHandler.Send(new LanguageOutput(
				new Emote(GetAppropriateTargetEmote(actor, effect.TargetCharacter), actor, actor,
					effect.TargetCharacter), langInfo, null));
			actor.OutputHandler.Send(new LanguageOutput(new Emote(EmoteText, actor, actor, effect.TargetCharacter),
				langInfo, null));
		}
		else
		{
			effect.TargetCharacter.OutputHandler.Send(new EmoteOutput(new Emote(
				string.Format(GetAppropriateTargetEmote(actor, effect.TargetCharacter), 0,
					text.ProperSentences().Fullstop()), actor, actor, effect.TargetCharacter)));
			actor.OutputHandler.Send(new EmoteOutput(new Emote(
				string.Format(EmoteText, text.ProperSentences().Fullstop()), actor, actor, effect.TargetCharacter)));
		}
	}

	public override IEnumerable<string> Verbs => new[] { SayVerb, TellVerb };

	public string SayVerb { get; set; }
	public string TellVerb { get; set; }
	public bool UseLanguage { get; set; }
	public string EmoteText { get; set; }
	public string FailEmoteText { get; set; }
	public string TargetEmoteText { get; set; }
	public IFutureProg TargetCanSeeIdentityProg { get; protected set; }
	public string UnknownIdentityDescription { get; protected set; }
	public bool UseAccent { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }

    protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
    {
        sb.AppendLine($"Say Verb: {SayVerb.ColourCommand()}");
        sb.AppendLine($"Tell Verb: {TellVerb.ColourCommand()}");
        sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
        sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
        sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
        sb.AppendLine($"Target Can See Identity Prog: {TargetCanSeeIdentityProg.MXPClickableFunctionName()}");
        sb.AppendLine($"Unknown Identity Desc: {UnknownIdentityDescription.ColourCharacter()}");
        sb.AppendLine($"Use Language: {UseLanguage.ToColouredString()}");
        sb.AppendLine($"Use Accent: {UseAccent.ToColouredString()}");
        sb.AppendLine();
        sb.AppendLine("Emotes:");
        sb.AppendLine();
        sb.AppendLine($"Emote: {EmoteText.ColourCommand()}");
        sb.AppendLine($"Fail Emote: {FailEmoteText.ColourCommand()}");
        sb.AppendLine($"Detected Target Emote: {TargetEmoteText.ColourCommand()}");
    }

    public string GetAppropriateHowSeen(ICharacter connecter, ICharacter connectee)
	{
		if ((bool?)TargetCanSeeIdentityProg.Execute(connecter, connectee) == true)
		{
			return connecter.HowSeen(connectee, flags: PerceiveIgnoreFlags.IgnoreConsciousness);
		}

		return UnknownIdentityDescription.ColourCharacter();
	}

	public string GetAppropriateTargetEmote(ICharacter connecter, ICharacter connectee)
	{
		if ((bool?)TargetCanSeeIdentityProg.Execute(connecter, connectee) == true)
		{
			return string.Format(TargetEmoteText, "$0");
		}

		return string.Format(TargetEmoteText, UnknownIdentityDescription.ColourCharacter());
	}

    #region Building Commands
    /// <inheritdoc />
    protected override string SubtypeHelpText => @"	#3begin <verb>#0 - sets the verb to activate this power
    #3end <verb>#0 - sets the verb to end this power
    #3skill <which>#0 - sets the skill used in the skill check
    #3difficulty <difficulty>#0 - sets the difficulty of the skill check
    #3threshold <outcome>#0 - sets the minimum outcome for skill success
    #3distance <distance>#0 - sets the distance that this power can be used at";

    /// <inheritdoc />
    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "tell":
            case "tellverb":
                return BuildingCommandTellVerb(actor, command);
            case "say":
            case "sayverb":
                return BuildingCommandSayVerb(actor, command);
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

    private bool BuildingCommandSayVerb(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which verb should be used to activate this power as a say?");
            return false;
        }

        var verb = command.SafeRemainingArgument.ToLowerInvariant();
        if (TellVerb.EqualTo(verb))
        {
            actor.OutputHandler.Send("The say and tell cannot be the same.");
            return false;
        }

        var costs = InvocationCosts[SayVerb].ToList();
        InvocationCosts[verb] = costs;
        InvocationCosts.Remove(SayVerb);
        SayVerb = verb;
        Changed = true;
        actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to invoke the power as a say.");
        return true;
    }

    private bool BuildingCommandTellVerb(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which verb should be used to activate this power as a tell?");
            return false;
        }

        var verb = command.SafeRemainingArgument.ToLowerInvariant();
        if (SayVerb.EqualTo(verb))
        {
            actor.OutputHandler.Send("The say and tell verb cannot be the same.");
            return false;
        }

        var costs = InvocationCosts[TellVerb].ToList();
        InvocationCosts[verb] = costs;
        InvocationCosts.Remove(TellVerb);
        TellVerb = verb;
        Changed = true;
        actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to invoke the power as a tell.");
        return true;
    }
    #endregion Building Subcommands
    #endregion Building Commands
}