using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.FutureProg;
using System.Xml.Linq;
using MudSharp.PerceptionEngine;

namespace MudSharp.Magic.Powers;

public class MindExpelPower : MagicPowerBase
{
	public override string PowerType => "Mind Expel";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindexpel", (power, gameworld) => new MindExpelPower(power, gameworld));
	}
	
    protected override XElement SaveDefinition()
    {
        var definition = new XElement("Definition",
            new XElement("Verb", Verb),
            new XElement("EmoteText", new XCData(EmoteText)),
            new XElement("EmoteTextSelf", new XCData(EmoteTextSelf)),
            new XElement("EchoToExpelledTarget", new XCData(EchoToExpelledTarget)),
            new XElement("EchoToNonExpelledTarget", new XCData(EchoToNonExpelledTarget)),
            new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
            new XElement("SkillCheckDifficultyProg", SkillCheckDifficultyProg.Id),
            new XElement("SkillCheckTrait", SkillCheckTrait.Id)
        );
        return definition;
    }

    protected MindExpelPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("Verb");
		if (element == null)
		{
			throw new ApplicationException($"The MindExpelPower #{Id} ({Name}) was missing a Verb element.");
		}

		Verb = element.Value.ToLowerInvariant();

		element = root.Element("EmoteText");
		if (element == null)
		{
			throw new ApplicationException($"The MindExpelPower #{Id} ({Name}) was missing a EmoteText element.");
		}

		EmoteText = element.Value.ToLowerInvariant();

		element = root.Element("EmoteTextSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindExpelPower #{Id} ({Name}) was missing a EmoteTextSelf element.");
		}

		EmoteTextSelf = element.Value.ToLowerInvariant();

		element = root.Element("EchoToExpelledTarget");
		if (element == null)
		{
			throw new ApplicationException($"The MindExpelPower #{Id} ({Name}) was missing a EchoToExpelledTarget element.");
		}

		EchoToExpelledTarget = element.Value.ToLowerInvariant();

		element = root.Element("EchoToNonExpelledTarget");
		if (element == null)
		{
			throw new ApplicationException($"The MindExpelPower #{Id} ({Name}) was missing a EchoToNonExpelledTarget element.");
		}

		EchoToNonExpelledTarget = element.Value.ToLowerInvariant();

		element = root.Element("MinimumSuccessThreshold");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindExpelPower #{Id} ({Name}) was missing a MinimumSuccessThreshold element.");
		}

		if (!int.TryParse(element.Value, out var ivalue))
		{
			if (!CheckExtensions.GetOutcome(element.Value, out var outcome))
			{
				throw new ApplicationException(
					$"The MindExpelPower #{Id} ({Name}) had a MinimumSuccessThreshold value that did not map to a valid Outcome.");
			}

			MinimumSuccessThreshold = outcome;
		}
		else
		{
			MinimumSuccessThreshold = (Outcome)ivalue;
		}

		element = root.Element("SkillCheckTrait");
		if (element == null)
		{
			throw new ApplicationException($"The MindExpelPower #{Id} ({Name}) was missing a SkillCheckTrait element.");
		}

		var trait = long.TryParse(element.Value, out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(element.Value);

		SkillCheckTrait = trait ?? throw new ApplicationException(
			$"The MindExpelPower #{Id} ({Name}) had a SkillCheckTrait element that pointed to a null Trait.");

		element = root.Element("SkillCheckDifficultyProg");
		if (element == null)
		{
			throw new ApplicationException($"The MindExpelPower #{Id} ({Name}) was missing a SkillCheckDifficultyProg element.");
		}
		SkillCheckDifficultyProg = Gameworld.FutureProgs.GetByIdOrName(element.Value);
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, verb);
		if (!truth)
		{
			actor.OutputHandler.Send($"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		actor.OutputHandler.Send(EmoteTextSelf);
		if (!string.IsNullOrEmpty(EmoteText))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor), flags: OutputFlags.SuppressSource));
		}

		var check = Gameworld.GetCheck(CheckType.MindExpelPower);
		var results = check.CheckAgainstAllDifficulties(actor, Difficulty.Normal, SkillCheckTrait);
		var sb = new StringBuilder();
		foreach (var effect in actor.EffectsOfType<MindConnectedToEffect>())
		{
			var difficultyText = SkillCheckDifficultyProg.Execute<string>(effect.OriginatorCharacter, actor);
			if (!difficultyText.TryParseEnum<Difficulty>(out var difficulty))
			{
				difficulty = Difficulty.Normal;
			}

			if (results[difficulty].Outcome < MinimumSuccessThreshold)
			{
				if (!string.IsNullOrEmpty(EchoToNonExpelledTarget))
				{
					effect.OriginatorCharacter.OutputHandler.Send(new EmoteOutput(new Emote(EchoToNonExpelledTarget, actor, actor)));
				}
				continue;
			}

			sb.AppendLine($"You successfully expel the presence of {effect.OriginatorCharacter.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreDisguises)} from your mind.");

			if (!string.IsNullOrEmpty(EchoToExpelledTarget))
			{
				effect.OriginatorCharacter.OutputHandler.Send(new EmoteOutput(new Emote(EchoToExpelledTarget, actor, actor)));
			}
		}

		if (sb.Length == 0)
		{
			sb.AppendLine($"You don't feel as if you have dispelled any foreign presences from your mind.");
		}

		actor.OutputHandler.Send(sb.ToString());
		ConsumePowerCosts(actor, Verb);
	}

	public override IEnumerable<string> Verbs => new[]
	{
		Verb
	};

	public string Verb { get; protected set; }
	public IFutureProg SkillCheckDifficultyProg { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public string EmoteText { get; protected set; }
	public string EmoteTextSelf { get; protected set; }
	public string EchoToExpelledTarget { get; protected set; }
	public string EchoToNonExpelledTarget { get; protected set; }

    protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
    {
        sb.AppendLine($"Power Verb: {Verb.ColourCommand()}");
        sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
        sb.AppendLine($"Skill Check Difficulty Prog: {SkillCheckDifficultyProg.MXPClickableFunctionName()}");
        sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
        sb.AppendLine();
        sb.AppendLine("Emotes:");
        sb.AppendLine();
        sb.AppendLine($"Emote: {EmoteText.ColourCommand()}");
        sb.AppendLine($"Self Emote: {EmoteTextSelf.ColourCommand()}");
        sb.AppendLine($"Expelled Target Emote: {EchoToExpelledTarget.ColourCommand()}");
        sb.AppendLine($"Non-Expelled Target Emote: {EchoToNonExpelledTarget.ColourCommand()}");
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
            case "verb":
                return BuildingCommandVerb(actor, command);
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

    private bool BuildingCommandVerb(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which verb should be used to end this power when active?");
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
