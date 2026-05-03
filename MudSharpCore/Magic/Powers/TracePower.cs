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

public sealed class TracePower : PsionicTargetedPowerBase
{
	public override string PowerType => "Trace";
	public override string DatabaseType => "trace";
	protected override string DefaultVerb => "trace";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("trace", (power, gameworld) => new TracePower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("trace", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new TracePower(gameworld, school, name, trait));
	}

	private TracePower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name, trait)
	{
		Blurb = "Inspect active mind links around a target";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} TRACE <target> to inspect active mental links around a mind.";
		FailEcho = "You trace the surface of $1's mind, but the connections elude you.";
		DoDatabaseInsert();
	}

	private TracePower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
	}

	protected override XElement SaveDefinition()
	{
		return SaveTargetedDefinition();
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose mind do you want to trace?", out var target) || target is null)
		{
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.MagicTelepathyCheck);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			return;
		}

		var results = Gameworld.GetCheck(CheckType.MagicTelepathyCheck)
		                       .CheckAgainstAllDifficulties(actor, SkillCheckDifficulty, SkillCheckTrait, target);
		var links = TraceLinks(target).DistinctBy(x => x.Character).ToList();
		if (!links.Any())
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} has no active mind links that you can detect.");
			ConsumePowerCosts(actor, Verb);
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"You trace the active mind links around {target.HowSeen(actor, true)}:");
		foreach (var link in links)
		{
			var concealment = link.Character.EffectsOfType<IMindContactConcealmentEffect>()
			                      .FirstOrDefault(x => x.ConcealsIdentityFrom(link.Character, actor, School));
			var difficulty = concealment is null
				? SkillCheckDifficulty
				: SkillCheckDifficulty.StageUp(concealment.AuditDifficultyStages);
			var visible = results[difficulty] >= MinimumSuccessThreshold;
			var description = visible
				? link.Character.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreConsciousness)
				: concealment?.UnknownIdentityDescription.ColourCharacter() ?? "an unknown mind".ColourCharacter();
			sb.AppendLine($"\t{link.Direction}: {description} [{link.School.Name.Colour(link.School.PowerListColour)}]");
		}

		actor.OutputHandler.Send(sb.ToString());
		ConsumePowerCosts(actor, Verb);
	}

	private static IEnumerable<(ICharacter Character, string Direction, IMagicSchool School)> TraceLinks(ICharacter target)
	{
		foreach (var effect in target.EffectsOfType<ConnectMindEffect>())
		{
			yield return (effect.TargetCharacter, "outbound", effect.School);
		}

		foreach (var effect in target.EffectsOfType<MindConnectedToEffect>())
		{
			yield return (effect.OriginatorCharacter, "inbound", effect.School);
		}
	}
}

