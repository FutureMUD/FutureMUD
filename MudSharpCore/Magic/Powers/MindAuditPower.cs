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
using MudSharp.Combat;
using MudSharp.Effects;

namespace MudSharp.Magic.Powers;

public class MindAuditPower : MagicPowerBase
{
	public override string PowerType => "Audit";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindaudit", (power, gameworld) => new MindAuditPower(power, gameworld));
	}

    /// <inheritdoc />
    protected override XElement SaveDefinition()
    {
        var definition = new XElement("Definition",
            new XElement("Verb", Verb),
			new XElement("EmoteText", new XCData(EmoteText)),
            new XElement("EmoteTextSelf", new XCData(EmoteTextSelf)),
            new XElement("EchoToDetectedTarget", new XCData(EchoToDetectedTarget)),
            new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold), 
            new XElement("SkillCheckDifficultyProg", SkillCheckDifficultyProg.Id),
            new XElement("ShouldEchoDetectionProg", ShouldEchoDetectionProg.Id),
            new XElement("SkillCheckTrait", SkillCheckTrait.Id)
        );
        return definition;
    }

    protected MindAuditPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("Verb");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a Verb element.");
		}

		Verb = element.Value.ToLowerInvariant();

		element = root.Element("EmoteText");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a EmoteText element.");
		}

		EmoteText = element.Value.ToLowerInvariant();

		element = root.Element("EmoteTextSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a EmoteTextSelf element.");
		}

		EmoteTextSelf = element.Value.ToLowerInvariant();

		element = root.Element("EchoToDetectedTarget");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a EchoToDetectedTarget element.");
		}

		EchoToDetectedTarget = element.Value.ToLowerInvariant();

		element = root.Element("MinimumSuccessThreshold");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindAuditPower #{Id} ({Name}) was missing a MinimumSuccessThreshold element.");
		}

		if (!int.TryParse(element.Value, out var ivalue))
		{
			if (!CheckExtensions.GetOutcome(element.Value, out var outcome))
			{
				throw new ApplicationException(
					$"The MindAuditPower #{Id} ({Name}) had a MinimumSuccessThreshold value that did not map to a valid Outcome.");
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
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a SkillCheckTrait element.");
		}

		var trait = long.TryParse(element.Value, out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(element.Value);

		SkillCheckTrait = trait ?? throw new ApplicationException(
			$"The MindAuditPower #{Id} ({Name}) had a SkillCheckTrait element that pointed to a null Trait.");

		element = root.Element("SkillCheckDifficultyProg");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a SkillCheckDifficultyProg element.");
		}
		SkillCheckDifficultyProg = Gameworld.FutureProgs.GetByIdOrName(element.Value);

		element = root.Element("ShouldEchoDetectionProg");
		if (element == null)
		{
			throw new ApplicationException($"The MindAuditPower #{Id} ({Name}) was missing a ShouldEchoDetectionProg element.");
		}
		ShouldEchoDetectionProg = Gameworld.FutureProgs.GetByIdOrName(element.Value);
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

		actor.OutputHandler.Send(EmoteTextSelf);
		if (!string.IsNullOrEmpty(EmoteText))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor), flags: OutputFlags.SuppressSource));
		}

		var check = Gameworld.GetCheck(CheckType.MindAuditPower);
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
				continue;
			}

			sb.AppendLine($"You detect the presence of {effect.OriginatorCharacter.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreDisguises)} in your mind.");

			if (!string.IsNullOrEmpty(EchoToDetectedTarget) && ShouldEchoDetectionProg.Execute<bool?>(effect.OriginatorCharacter, actor) == true)
			{
				effect.OriginatorCharacter.OutputHandler.Send(new EmoteOutput(new Emote(EchoToDetectedTarget, actor, actor)));
			}
		}

		if (sb.Length == 0)
		{
			sb.AppendLine($"You don't detect any foreign presences in your mind.");
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
	public IFutureProg ShouldEchoDetectionProg { get; protected set; }
	public string EmoteText { get; protected set; }
	public string EmoteTextSelf { get; protected set; }
	public string EchoToDetectedTarget { get; protected set; }

    protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
    {
        sb.AppendLine($"Power Verb: {Verb.ColourCommand()}");
        sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
        sb.AppendLine($"Skill Check Difficulty Prog: {SkillCheckDifficultyProg.MXPClickableFunctionName()}");
        sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
        sb.AppendLine($"Should Detection Echo Prog: {ShouldEchoDetectionProg.MXPClickableFunctionName()}");
        sb.AppendLine();
        sb.AppendLine("Emotes:");
        sb.AppendLine();
        sb.AppendLine($"Emote: {EmoteText.ColourCommand()}");
        sb.AppendLine($"Self Emote: {EmoteTextSelf.ColourCommand()}");
        sb.AppendLine($"Detected Target Emote: {EchoToDetectedTarget.ColourCommand()}");
    }
}
