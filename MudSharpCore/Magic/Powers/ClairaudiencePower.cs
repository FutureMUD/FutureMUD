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

public sealed class ClairaudiencePower : SustainedMagicPower
{
	public override string PowerType => "Clairaudience";
	public override string DatabaseType => "clairaudience";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("clairaudience", (power, gameworld) => new ClairaudiencePower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("clairaudience", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new ClairaudiencePower(gameworld, school, name, trait));
	}

	private ClairaudiencePower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name)
	{
		IsPsionic = true;
		Blurb = "Hear audible output through a contacted mind";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} CLAIRAUDIENCE <target> to hear audible output at that mind's location.";
		BeginVerb = "clairaudience";
		EndVerb = "endclairaudience";
		PowerDistance = MagicPowerDistance.AnyConnectedMindOrConnectedTo;
		SkillCheckTrait = trait;
		SkillCheckDifficulty = Difficulty.Normal;
		MinimumSuccessThreshold = Outcome.MinorPass;
		ConcentrationPointsToSustain = 1.0;
		SustainPenalty = Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel") * -1.0;
		DetectableWithDetectMagic = Difficulty.Normal;
		BeginEmote = "You open your hearing through $1's mind.";
		EndEmote = "You close your remote hearing.";
		FailEmote = "You reach for $1's senses, but cannot tune them.";
		DoDatabaseInsert();
	}

	private ClairaudiencePower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		BeginVerb = root.Element("BeginVerb")?.Value ?? "clairaudience";
		EndVerb = root.Element("EndVerb")?.Value ?? "endclairaudience";
		PowerDistance = Enum.Parse<MagicPowerDistance>(root.Element("PowerDistance")?.Value ?? nameof(MagicPowerDistance.AnyConnectedMindOrConnectedTo), true);
		SkillCheckDifficulty = (Difficulty)int.Parse(root.Element("SkillCheckDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		MinimumSuccessThreshold = (Outcome)int.Parse(root.Element("MinimumSuccessThreshold")?.Value ?? ((int)Outcome.MinorPass).ToString());
		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(root.Element("SkillCheckTrait")?.Value ?? "0")) ??
		                  throw new ApplicationException($"Invalid SkillCheckTrait in power {Id} ({Name}).");
		BeginEmote = root.Element("BeginEmote")?.Value ?? "You open your hearing through $1's mind.";
		EndEmote = root.Element("EndEmote")?.Value ?? "You close your remote hearing.";
		FailEmote = root.Element("FailEmote")?.Value ?? "You reach for $1's senses, but cannot tune them.";
	}

	public string BeginVerb { get; private set; }
	public string EndVerb { get; private set; }
	public MagicPowerDistance PowerDistance { get; private set; }
	public Difficulty SkillCheckDifficulty { get; private set; }
	public ITraitDefinition SkillCheckTrait { get; private set; }
	public Outcome MinimumSuccessThreshold { get; private set; }
	public string BeginEmote { get; private set; }
	public string EndEmote { get; private set; }
	public string FailEmote { get; private set; }
	public override IEnumerable<string> Verbs => [BeginVerb, EndVerb];

	protected override XElement SaveDefinition()
	{
		var definition = new XElement("Definition",
			new XElement("BeginVerb", BeginVerb),
			new XElement("EndVerb", EndVerb),
			new XElement("PowerDistance", PowerDistance),
			new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id),
			new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
			new XElement("BeginEmote", new XCData(BeginEmote)),
			new XElement("EndEmote", new XCData(EndEmote)),
			new XElement("FailEmote", new XCData(FailEmote))
		);
		AddBaseDefinition(definition);
		SaveSustainedDefinition(definition);
		return definition;
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (verb.EqualTo(EndVerb))
		{
			UseEnd(actor, command);
			return;
		}

		UseBegin(actor, command);
	}

	private void UseBegin(ICharacter actor, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, BeginVerb);
		if (!truth)
		{
			actor.OutputHandler.Send($"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		if (!HandleGeneralUseRestrictions(actor))
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Whose mind do you want to hear through?");
			return;
		}

		var target = AcquireTarget(actor, command.PopSpeech(), PowerDistance);
		if (target is null)
		{
			actor.OutputHandler.Send("You cannot find any eligible mind by that description.");
			return;
		}

		if (actor.EffectsOfType<MagicClairaudienceConcentrationEffect>().Any(x => x.Power == this && x.TargetCharacter == target))
		{
			actor.OutputHandler.Send("You are already hearing through that mind.");
			return;
		}

		if (CanInvokePowerProg.ExecuteBool(actor, target) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor, target)?.ToString() ??
			                         $"You cannot use that power on {target.HowSeen(actor)}.");
			return;
		}

		var outcome = Gameworld.GetCheck(CheckType.MagicTelepathyCheck).Check(actor, SkillCheckDifficulty, SkillCheckTrait, target);
		if (outcome < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(FailEmote, actor, actor, target)));
			return;
		}

		actor.AddEffect(new MagicClairaudienceConcentrationEffect(actor, this, target), GetDuration(outcome.SuccessDegrees()));
		actor.OutputHandler.Send(new EmoteOutput(new Emote(BeginEmote, actor, actor, target)));
		ConsumePowerCosts(actor, BeginVerb);
	}

	private void UseEnd(ICharacter actor, StringStack command)
	{
		var effects = actor.EffectsOfType<MagicClairaudienceConcentrationEffect>().Where(x => x.Power == this).ToList();
		if (!effects.Any())
		{
			actor.OutputHandler.Send("You are not currently hearing through any mind with that power.");
			return;
		}

		var effect = effects.First();
		if (!command.IsFinished)
		{
			var targetText = command.PopSpeech();
			var target = effects.Select(x => x.TargetCharacter)
			                    .OfType<ICharacter>()
			                    .GetFromItemListByKeyword(targetText, actor);
			effect = effects.FirstOrDefault(x => x.TargetCharacter == target);
		}
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not hearing through any mind like that.");
			return;
		}

		actor.RemoveEffect(effect, true);
		actor.OutputHandler.Send(new EmoteOutput(new Emote(EndEmote, actor, actor, effect.TargetCharacter ?? actor)));
		ConsumePowerCosts(actor, EndVerb);
	}

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		foreach (var effect in actor.EffectsOfType<MagicClairaudienceConcentrationEffect>().Where(x => x.Power == this).ToList())
		{
			actor.RemoveEffect(effect, true);
		}
	}

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Begin Verb: {BeginVerb.ColourCommand()}");
		sb.AppendLine($"End Verb: {EndVerb.ColourCommand()}");
		sb.AppendLine($"Power Distance: {PowerDistance.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
		sb.AppendLine($"Begin Emote: {BeginEmote.ColourCommand()}");
		sb.AppendLine($"End Emote: {EndEmote.ColourCommand()}");
		sb.AppendLine($"Fail Emote: {FailEmote.ColourCommand()}");
	}
}

