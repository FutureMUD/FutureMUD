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
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public class MindBarrierPower : SustainedMagicPower
{
	public override string PowerType => "Barrier";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindbarrier",
			(power, gameworld) => new MindBarrierPower(power, gameworld));
	}

    /// <inheritdoc />
    protected override XElement SaveDefinition()
    {
        var definition = new XElement("Definition",
            new XElement("BeginVerb", BeginVerb),
            new XElement("EndVerb", EndVerb),
            new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
            new XElement("SkillCheckTrait", SkillCheckTrait.Id),
            new XElement("AppliesToCharacterProg", AppliesToCharacterProg.Id),
            new XElement("EmoteForBegin", new XCData(EmoteForBegin)),
            new XElement("EmoteForBeginSelf", new XCData(EmoteForBeginSelf)),
            new XElement("EmoteForEnd", new XCData(EmoteForEnd)),
            new XElement("EmoteForEndSelf", new XCData(EmoteForEndSelf)),
            new XElement("BlockEmoteSelf", new XCData(BlockEmoteSelf)),
            new XElement("BlockEmoteTarget", new XCData(BlockEmoteTarget)),
            new XElement("OvercomeEmoteSelf", new XCData(OvercomeEmoteSelf)),
            new XElement("OvercomeEmoteTarget", new XCData(OvercomeEmoteTarget)),
            new XElement("EndWhenNotSustainingError", new XCData(EndWhenNotSustainingError)),
            new XElement("BeginWhenAlreadySustainingError", new XCData(BeginWhenAlreadySustainingError)),
            new XElement("PermitAllies", PermitAllies),
            new XElement("PermitTrustedAllies", PermitTrustedAllies),
            new XElement("FailIfOvercome", FailIfOvercome),
			new XElement("Bonuses",
				from bonus in TargetCheckBonusPerOutcome
				select new XElement("Bonus",
					new XAttribute("outcome", (int)bonus.Key),
					new XAttribute("bonus", bonus.Value)
                )
            )
        );
        SaveSustainedDefinition(definition);
        return definition;
    }

    protected MindBarrierPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("BeginVerb");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a BeginVerb element.");
		}

		BeginVerb = element.Value.ToLowerInvariant();

		element = root.Element("EndVerb");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EndVerb element.");
		}

		EndVerb = element.Value.ToLowerInvariant();

		element = root.Element("SkillCheckDifficulty");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a SkillCheckDifficulty element.");
		}
		SkillCheckDifficulty = (Difficulty)int.Parse(element.Value);

		element = root.Element("SkillCheckTrait");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a SkillCheckTrait element.");
		}
		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(element.Value));

		element = root.Element("PermitAllies");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a PermitAllies element.");
		}
		PermitAllies = bool.Parse(element.Value);

		element = root.Element("PermitTrustedAllies");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a PermitTrustedAllies element.");
		}
		PermitTrustedAllies = bool.Parse(element.Value);

		element = root.Element("FailIfOvercome");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a FailIfOvercome element.");
		}
		FailIfOvercome = bool.Parse(element.Value);

		element = root.Element("AppliesToCharacterProg");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a AppliesToCharacterProg element.");
		}
		AppliesToCharacterProg = Gameworld.FutureProgs.GetByIdOrName(element.Value);

		element = root.Element("EmoteForBegin");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EmoteForBegin element.");
		}
		EmoteForBegin = element.Value;

		element = root.Element("EmoteForBeginSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EmoteForBeginSelf element.");
		}
		EmoteForBeginSelf = element.Value;

		element = root.Element("EmoteForEnd");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EmoteForEnd element.");
		}
		EmoteForEnd = element.Value;

		element = root.Element("EmoteForEndSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EmoteForEndSelf element.");
		}
		EmoteForEndSelf = element.Value;

		element = root.Element("BlockEmoteSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a BlockEmoteSelf element.");
		}
		BlockEmoteSelf = element.Value;

		element = root.Element("BlockEmoteTarget");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a BlockEmoteTarget element.");
		}
		BlockEmoteTarget = element.Value;

		element = root.Element("OvercomeEmoteSelf");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a OvercomeEmoteSelf element.");
		}
		OvercomeEmoteSelf = element.Value;

		element = root.Element("OvercomeEmoteTarget");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a OvercomeEmoteTarget element.");
		}
		OvercomeEmoteTarget = element.Value;

		element = root.Element("EndWhenNotSustainingError");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a EndWhenNotSustainingError element.");
		}
		EndWhenNotSustainingError = element.Value;

		element = root.Element("BeginWhenAlreadySustainingError");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a BeginWhenAlreadySustainingError element.");
		}
		BeginWhenAlreadySustainingError = element.Value;

		element = root.Element("Bonuses");
		if (element == null)
		{
			throw new ApplicationException($"The MindBarrierPower #{Id} ({Name}) was missing a Bonuses element.");
		}

		foreach (var sub in element.Elements("Bonus"))
		{
			var outcomeAttribute = sub.Attribute("outcome");
			if (outcomeAttribute == null)
			{
				throw new ApplicationException(
					$"The Bonus \"{sub}\" had an invalid outcome attribute in the definition XML for power {Id} ({Name}).");
			}
			var outcome = (Outcome)int.Parse(sub.Value);

			var bonusAttribute = sub.Attribute("bonus");
			if (bonusAttribute == null)
			{
				throw new ApplicationException(
					$"The Bonus \"{sub}\" had an invalid outcome attribute in the definition XML for power {Id} ({Name}).");
			}
			var bonus = double.Parse(bonusAttribute.Value);

			TargetCheckBonusPerOutcome[outcome] = bonus;
			foreach (var value in Enum.GetValues(typeof(Outcome)).OfType<Outcome>())
			{
				if (value < Outcome.MajorFail)
				{
					continue;
				}

				if (TargetCheckBonusPerOutcome.ContainsKey(value))
				{
					continue;
				}

				TargetCheckBonusPerOutcome[value] = 0.0;
			}
		}
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
			UseCommandBegin(actor, command);
			return;
		}

		UseCommandEnd(actor, command);
	}

	private void UseCommandEnd(ICharacter actor, StringStack command)
	{
		var effects = actor.EffectsOfType<MindBarrierEffect>().Where(x => x.PowerOrigin == this).ToList();
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

	private void UseCommandBegin(ICharacter actor, StringStack command)
	{
		var effects = actor.EffectsOfType<MindBarrierEffect>().Where(x => x.PowerOrigin == this).ToList();
		if (effects.Any())
		{
			actor.OutputHandler.Send(BeginWhenAlreadySustainingError);
			return;
		}
		var check = Gameworld.GetCheck(CheckType.MindBarrierPowerCheck);
		var result = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait);
		actor.OutputHandler.Send(EmoteForBeginSelf);
		if (!string.IsNullOrEmpty(EmoteForBegin))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteForBegin, actor, actor), flags: OutputFlags.SuppressSource));
		}
		actor.AddEffect(new MindBarrierEffect(actor, TargetCheckBonusPerOutcome[result], this), GetDuration(result.SuccessDegrees()));
		ConsumePowerCosts(actor, BeginVerb);
	}

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		var effects = actor.EffectsOfType<MindBarrierEffect>().Where(x => x.PowerOrigin == this).ToList();
		foreach (var effect in effects)
		{
			actor.RemoveEffect(effect, true);
		}
	}

	public override IEnumerable<string> Verbs => new[] { BeginVerb, EndVerb };

	public string BeginVerb { get; protected set; }
	public string EndVerb { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public bool PermitAllies { get; protected set; }
	public bool PermitTrustedAllies { get; protected set; }
	public IFutureProg AppliesToCharacterProg { get; protected set; }
	public Dictionary<Outcome, double> TargetCheckBonusPerOutcome { get; } = new();
	public bool FailIfOvercome { get; protected set; }
	public string EmoteForBegin { get; protected set; }
	public string EmoteForBeginSelf { get; protected set; }
	public string EmoteForEnd { get; protected set; }
	public string EmoteForEndSelf { get; protected set; }
	public string BlockEmoteSelf { get; protected set; }
	public string BlockEmoteTarget { get; protected set; }
	public string OvercomeEmoteSelf { get; protected set; }
	public string OvercomeEmoteTarget { get; protected set; }
	public string EndWhenNotSustainingError { get; protected set; }
	public string BeginWhenAlreadySustainingError { get; protected set; }

    protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
    {
        sb.AppendLine($"Begin Verb: {BeginVerb.ColourCommand()}");
        sb.AppendLine($"End Verb: {EndVerb.ColourCommand()}");
        sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
        sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
        sb.AppendLine($"Applies Character Prog: {AppliesToCharacterProg.MXPClickableFunctionName()}");
        sb.AppendLine($"Permit Allies: {PermitAllies.ToColouredString()}");
        sb.AppendLine($"Permit Trusted Allies: {PermitAllies.ToColouredString()}");
        sb.AppendLine($"Fail If Overcome: {FailIfOvercome.ToColouredString()}");
        sb.AppendLine();
        sb.AppendLine("Check Bonuses:");
        foreach (var item in TargetCheckBonusPerOutcome.OrderBy(x => x.Key))
        {
            sb.AppendLine($"\t{item.Key.DescribeColour()}: {item.Value.ToBonusString(actor)}");
        }
        sb.AppendLine();
        sb.AppendLine("Emotes:");
        sb.AppendLine();
        sb.AppendLine($"Begin Emote: {EmoteForBegin.ColourCommand()}");
        sb.AppendLine($"Emote Self: {EmoteForBeginSelf.ColourCommand()}");
        sb.AppendLine($"End Emote: {EmoteForEnd.ColourCommand()}");
        sb.AppendLine($"End Emote Self: {EmoteForEndSelf.ColourCommand()}");
        sb.AppendLine($"End Emote Self: {BlockEmoteSelf.ColourCommand()}");
        sb.AppendLine($"End Emote Target: {BlockEmoteTarget.ColourCommand()}");
        sb.AppendLine($"Overcome Emote Self: {OvercomeEmoteSelf.ColourCommand()}");
        sb.AppendLine($"Overcome Emote Target: {OvercomeEmoteTarget.ColourCommand()}");
        sb.AppendLine($"End Not Sustain Emote: {EndWhenNotSustainingError.ColourCommand()}");
        sb.AppendLine($"Begin Already Sustaining Emote: {BeginWhenAlreadySustainingError.ColourCommand()}");
    }

}
