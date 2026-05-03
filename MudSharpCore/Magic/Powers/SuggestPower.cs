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

public sealed class SuggestPower : PsionicTargetedPowerBase
{
	public override string PowerType => "Suggest";
	public override string DatabaseType => "suggest";
	protected override string DefaultVerb => "suggest";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("suggest", (power, gameworld) => new SuggestPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("suggest", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new SuggestPower(gameworld, school, name, trait));
	}

	private SuggestPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name, trait)
	{
		Blurb = "Inject an involuntary thought into a target mind";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} SUGGEST <target> [<emotion wrapper>] <thought> to put a thought into a mind.";
		FailEcho = "You reach for $1's thoughts, but cannot plant your suggestion.";
		DoDatabaseInsert();
	}

	private SuggestPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
	}

	protected override XElement SaveDefinition()
	{
		return SaveTargetedDefinition();
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose mind do you want to suggest a thought to?", out var target) || target is null)
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What thought do you want to suggest?");
			return;
		}

		if (!PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} refuses involuntary mental traffic.");
			return;
		}

		var (emotion, thought) = ParseEmotionWrapper(command.SafeRemainingArgument);
		if (string.IsNullOrWhiteSpace(thought))
		{
			actor.OutputHandler.Send("What thought do you want to suggest?");
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.MindSayPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			return;
		}

		if (!string.IsNullOrWhiteSpace(emotion))
		{
			var emotionOutcome = CheckPower(actor, target, CheckType.MindSayPower);
			if (emotionOutcome >= MinimumSuccessThreshold)
			{
				PsionicTrafficHelper.DeliverEmotion(actor, target, School, emotion, notifySource: false);
			}
		}

		PsionicTrafficHelper.DeliverThought(actor, target, School, thought);
		ConsumePowerCosts(actor, Verb);
	}

	private static (string? Emotion, string Thought) ParseEmotionWrapper(string text)
	{
		var trimmed = text.Trim();
		if (trimmed.Length < 3)
		{
			return (null, trimmed);
		}

		var pairs = new[] { ('(', ')'), ('[', ']'), ('*', '*'), ('-', '-') };
		foreach (var (open, close) in pairs)
		{
			if (trimmed[0] != open)
			{
				continue;
			}

			var index = trimmed.IndexOf(close, 1);
			if (index <= 1)
			{
				continue;
			}

			return (trimmed[1..index].Trim(), trimmed[(index + 1)..].Trim());
		}

		return (null, trimmed);
	}
}

