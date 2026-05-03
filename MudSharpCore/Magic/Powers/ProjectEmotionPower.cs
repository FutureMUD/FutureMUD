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

public sealed class ProjectEmotionPower : PsionicTargetedPowerBase
{
	public override string PowerType => "Project Emotion";
	public override string DatabaseType => "projectemotion";
	protected override string DefaultVerb => "projectemotion";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("projectemotion", (power, gameworld) => new ProjectEmotionPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("projectemotion", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new ProjectEmotionPower(gameworld, school, name, trait));
	}

	private ProjectEmotionPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name, trait)
	{
		Blurb = "Inject an involuntary feeling into a target mind";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} PROJECTEMOTION <target> <feeling> to put a feeling into a mind.";
		FailEcho = "You reach for $1's feelings, but cannot touch them.";
		DoDatabaseInsert();
	}

	private ProjectEmotionPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
	}

	protected override XElement SaveDefinition()
	{
		return SaveTargetedDefinition();
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose mind do you want to project emotion into?", out var target) || target is null)
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What feeling do you want to project?");
			return;
		}

		if (!PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} refuses involuntary mental traffic.");
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.MindSayPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			return;
		}

		PsionicTrafficHelper.DeliverEmotion(actor, target, School, command.SafeRemainingArgument);
		ConsumePowerCosts(actor, Verb);
	}
}

