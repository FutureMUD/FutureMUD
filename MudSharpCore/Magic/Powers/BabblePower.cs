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

public sealed class BabblePower : PsionicTargetedPowerBase
{
	public override string PowerType => "Babble";
	public override string DatabaseType => "babble";
	protected override string DefaultVerb => "babble";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("babble", (power, gameworld) => new BabblePower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("babble", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new BabblePower(gameworld, school, name, trait));
	}

	private BabblePower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name, trait)
	{
		Blurb = "Scramble a target's outgoing speech";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} BABBLE <target> to make their speech come out as incomprehensible babble.";
		DurationSeconds = 120.0;
		FailEcho = "You brush against $1's speech centres, but cannot tangle them.";
		SuccessEcho = "You tangle $1's speech centres.";
		DoDatabaseInsert();
	}

	private BabblePower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		DurationSeconds = double.Parse(root.Element("DurationSeconds")?.Value ?? "120");
	}

	public double DurationSeconds { get; private set; }

	protected override XElement SaveDefinition()
	{
		return SaveTargetedDefinition(new XElement("DurationSeconds", DurationSeconds));
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose speech do you want to scramble?", out var target) || target is null)
		{
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.ConnectMindPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			return;
		}

		target.AddEffect(new PsionicBabbleEffect(target, this), TimeSpan.FromSeconds(DurationSeconds));
		if (!string.IsNullOrWhiteSpace(SuccessEcho))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(SuccessEcho, actor, actor, target)));
		}

		ConsumePowerCosts(actor, Verb);
	}

	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Duration: {TimeSpan.FromSeconds(DurationSeconds).Describe(actor).ColourValue()}");
	}
}

