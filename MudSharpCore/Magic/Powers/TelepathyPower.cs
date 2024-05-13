using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public partial class TelepathyPower : SustainedMagicPower
{
	public override string PowerType => "Telepathy";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("telepathy", (power, gameworld) => new TelepathyPower(power, gameworld));
	}

    protected override XElement SaveDefinition()
    {
        var definition = new XElement("Definition",
            new XElement("Verb", Verb),
            new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
            new XElement("SkillCheckTrait", SkillCheckTrait.Id),
            new XElement("BeginEmoteText", new XCData(BeginEmoteText)),
            new XElement("EndEmoteText", new XCData(EndEmoteText)),
            new XElement("ShowFeels", ShowFeels),
            new XElement("ShowThinks", ShowThinks),
            new XElement("ShowThinkEmote", ShowThinkEmote),
            new XElement("Distance", (int)PowerDistance)
        );
        SaveSustainedDefinition(definition);
        return definition;
    }

    protected TelepathyPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("ShowFeels");
		if (element == null)
		{
			throw new ApplicationException($"There was no ShowFeels in the definition XML for power {Id} ({Name}).");
		}

		ShowFeels = bool.Parse(element.Value);

		element = root.Element("ShowThinks");
		if (element == null)
		{
			throw new ApplicationException($"There was no ShowThinks in the definition XML for power {Id} ({Name}).");
		}

		ShowThinks = bool.Parse(element.Value);

		element = root.Element("ShowThinkEmote");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no ShowThinkEmote in the definition XML for power {Id} ({Name}).");
		}

		ShowThinkEmote = bool.Parse(element.Value);

		element = root.Element("ShowThinkerDescription");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no ShowThinkerDescription in the definition XML for power {Id} ({Name}).");
		}

		ShowThinkerDescription = long.TryParse(element.Value, out var progid)
			? Gameworld.FutureProgs.Get(progid)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("Distance");
		if (element == null)
		{
			throw new ApplicationException($"There was no Distance in the definition XML for power {Id} ({Name}).");
		}

		PowerDistance = int.TryParse(element.Value, out var value)
			? (MagicPowerDistance)value
			: (MagicPowerDistance)Enum.Parse(typeof(MagicPowerDistance), element.Value);

		element = root.Element("Verb");
		if (element == null)
		{
			throw new ApplicationException($"There was no Verb in the definition XML for power {Id} ({Name}).");
		}

		Verb = element.Value;

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

		element = root.Element("BeginEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no BeginEmoteText in the definition XML for power {Id} ({Name}).");
		}

		BeginEmoteText = element.Value;

		element = root.Element("EndEmoteText");
		if (element == null)
		{
			throw new ApplicationException($"There was no EndEmoteText in the definition XML for power {Id} ({Name}).");
		}

		EndEmoteText = element.Value;
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

		if ((bool?)CanInvokePowerProg?.Execute(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor).ToString());
			return;
		}

		if (actor.EffectsOfType<MagicTelepathyEffect>().Any(x => x.TelepathyPower == this))
		{
			actor.RemoveAllEffects(x => x.GetSubtype<MagicTelepathyEffect>()?.TelepathyPower == this, true);
			actor.OutputHandler.Send(new EmoteOutput(new Emote(EndEmoteText, actor, actor)));
			return;
		}

		var check = Gameworld.GetCheck(CheckType.MagicTelepathyCheck);
		var result = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait, null);

		actor.AddEffect(new MagicTelepathyEffect(actor, this), GetDuration(result.SuccessDegrees()));
		actor.OutputHandler.Send(new EmoteOutput(new Emote(BeginEmoteText, actor, actor)));
		ConsumePowerCosts(actor, verb);
	}

	#region Overrides of SustainedMagicPower

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		actor.RemoveAllEffects(x => x.GetSubtype<MagicTelepathyEffect>()?.TelepathyPower == this, true);
	}

	#endregion

	public override IEnumerable<string> Verbs => new[] { Verb };

	public bool ShowFeels { get; protected set; }
	public bool ShowThinks { get; protected set; }
	public bool ShowThinkEmote { get; protected set; }
	public IFutureProg ShowThinkerDescription { get; protected set; }
	public MagicPowerDistance PowerDistance { get; protected set; }
	public string Verb { get; protected set; }

	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }

	public string BeginEmoteText { get; protected set; }
	public string EndEmoteText { get; protected set; }

    protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
    {
        sb.AppendLine($"Power Verb: {Verb.ColourCommand()}");
        sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
        sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
        sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
        sb.AppendLine($"Show Thinker Desc Prog: {ShowThinkerDescription.MXPClickableFunctionName()}");
        sb.AppendLine($"Show Feels: {ShowFeels.ToColouredString()}");
        sb.AppendLine($"Show Thinks: {ShowThinks.ToColouredString()}");
        sb.AppendLine($"Show Think Emote: {ShowThinkEmote.ToColouredString()}");
        sb.AppendLine($"Power Distance: {PowerDistance.DescribeEnum().ColourValue()}");
        sb.AppendLine();
        sb.AppendLine("Emotes:");
        sb.AppendLine();
        sb.AppendLine($"Begin Emote: {BeginEmoteText.ColourCommand()}");
        sb.AppendLine($"End Emote: {EndEmoteText.ColourCommand()}");
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