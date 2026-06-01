using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.RPG.Law.PatrolStrategies;

public class InvestigationPatrolStrategy : CrimeTargetedPatrolStrategyBase
{
	public InvestigationPatrolStrategy(IFuturemud gameworld, string strategyData = null) : base(gameworld, strategyData,
		"InvestigationPatrol")
	{
	}

	public override string Name => "InvestigationPatrol";

	public double EvidenceReliability { get; private set; } = 0.85;
	public TimeSpan SceneSearchTime { get; private set; } = TimeSpan.FromMinutes(2);

	protected override void LoadAdditionalStrategyData(XElement root)
	{
		if (double.TryParse(root.Attribute("evidenceReliability")?.Value, NumberStyles.Float,
			    CultureInfo.InvariantCulture, out double reliability))
		{
			EvidenceReliability = Math.Clamp(reliability, 0.0, 1.0);
		}

		if (double.TryParse(root.Attribute("sceneSearchSeconds")?.Value, NumberStyles.Float,
			    CultureInfo.InvariantCulture, out double sceneSearch))
		{
			SceneSearchTime = TimeSpan.FromSeconds(Math.Max(1.0, sceneSearch));
		}
	}

	public override IEnumerable<ICrime> SelectDispatchCrimes(ILegalAuthority authority)
	{
		return authority.KnownCrimes
		                .Where(NeedsInvestigation)
		                .OrderByDescending(x => x.Law.EnforcementPriority)
		                .ThenByDescending(x => x.RealTimeOfCrime);
	}

	public static bool NeedsInvestigation(ICrime crime)
	{
		if (crime is null ||
		    !crime.IsKnownCrime ||
		    crime.HasBeenFinalised ||
		    crime.CrimeLocation is null)
		{
			return false;
		}

		int totalCharacteristics = crime.Criminal?.CharacteristicDefinitions.Count() ?? 0;
		return totalCharacteristics > 0 && crime.CriminalCharacteristics.Count < totalCharacteristics;
	}

	protected override bool TargetCrimeStillValid(IPatrol patrol)
	{
		return NeedsInvestigation(patrol.TargetCrime);
	}

	protected override void HandleArrivedAtTargetNode(IPatrol patrol, ICell node)
	{
		base.HandleArrivedAtTargetNode(patrol, node);
		if (DateTime.UtcNow - patrol.LastArrivedTime < SceneSearchTime)
		{
			return;
		}

		ICrime crime = patrol.TargetCrime;
		ICharacter criminal = crime.Criminal;
		bool identityKnown = criminal is not null &&
		                     patrol.PatrolLeader.CanSee(criminal) &&
		                     patrol.PatrolLeader.Location == criminal.Location;
		crime.RecordInvestigationEvidence(patrol.PatrolLeader, EvidenceReliability, identityKnown);
		patrol.PatrolLeader.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ inspect|inspects the area carefully, collecting evidence about a reported crime.",
			patrol.PatrolLeader,
			patrol.PatrolLeader)));
		patrol.CompletePatrol();
	}

	public override string HelpText => @"Investigation patrol configuration:

	#3radius <rooms>#0 - sets how far from a patrol node a reported crime may be to dispatch this patrol
	#3reliability <0.0-1.0>#0 - sets how reliably the patrol records suspect details
	#3search <timespan>#0 - sets how long the patrol searches the crime scene";

	public override bool BuildingCommand(ICharacter actor, IPatrolRoute patrol, StringStack command)
	{
		switch (command.PeekSpeech().ToLowerInvariant())
		{
			case "reliability":
			case "quality":
				command.PopSpeech();
				return BuildingCommandReliability(actor, command);
			case "search":
			case "scene":
			case "time":
				command.PopSpeech();
				return BuildingCommandSearchTime(actor, command);
			default:
				return base.BuildingCommand(actor, patrol, command);
		}
	}

	private bool BuildingCommandReliability(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
		    !double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ||
		    value < 0.0 ||
		    value > 1.0)
		{
			actor.OutputHandler.Send("What reliability between 0.0 and 1.0 should this investigation use?");
			return false;
		}

		EvidenceReliability = value;
		actor.OutputHandler.Send(
			$"This investigation patrol will now record evidence with {EvidenceReliability.ToString("P0", actor).ColourValue()} reliability.");
		return true;
	}

	private bool BuildingCommandSearchTime(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
		    !MudSharp.TimeAndDate.MudTimeSpan.TryParse(command.SafeRemainingArgument, out MudSharp.TimeAndDate.MudTimeSpan value) ||
		    value.TotalSeconds <= 0.0)
		{
			actor.OutputHandler.Send("How long should the patrol spend searching the crime scene?");
			return false;
		}

		SceneSearchTime = value;
		actor.OutputHandler.Send(
			$"This investigation patrol will now spend {SceneSearchTime.Describe(actor).ColourValue()} searching the crime scene.");
		return true;
	}

	public override string ShowConfiguration(ICharacter actor, IPatrolRoute patrol)
	{
		return base.ShowConfiguration(actor, patrol) +
		       $"Evidence Reliability: {EvidenceReliability.ToString("P0", actor).ColourValue()}\n" +
		       $"Scene Search Time: {SceneSearchTime.Describe(actor).ColourValue()}\n";
	}

	protected override void SaveAdditionalStrategyData(XElement root)
	{
		root.Add(
			new XAttribute("evidenceReliability", EvidenceReliability.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("sceneSearchSeconds", SceneSearchTime.TotalSeconds.ToString(CultureInfo.InvariantCulture)));
	}
}
