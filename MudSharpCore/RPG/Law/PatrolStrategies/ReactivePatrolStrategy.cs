using MudSharp.Construction;
using MudSharp.TimeAndDate;
using System.Globalization;

namespace MudSharp.RPG.Law.PatrolStrategies;

public class ReactivePatrolStrategy : CrimeTargetedPatrolStrategyBase
{
	public ReactivePatrolStrategy(IFuturemud gameworld, string strategyData = null) : base(gameworld, strategyData,
		"ReactivePatrol")
	{
	}

	public override string Name => "ReactivePatrol";

	public TimeSpan DispatchWindow { get; private set; } = TimeSpan.FromMinutes(30);
	public TimeSpan MaximumDuration { get; private set; } = TimeSpan.FromMinutes(15);

	protected override void LoadAdditionalStrategyData(XElement root)
	{
		if (double.TryParse(root.Attribute("dispatchWindowSeconds")?.Value, NumberStyles.Float,
			    CultureInfo.InvariantCulture, out double dispatchWindow))
		{
			DispatchWindow = TimeSpan.FromSeconds(Math.Max(1.0, dispatchWindow));
		}

		if (double.TryParse(root.Attribute("maximumDurationSeconds")?.Value, NumberStyles.Float,
			    CultureInfo.InvariantCulture, out double maximumDuration))
		{
			MaximumDuration = TimeSpan.FromSeconds(Math.Max(1.0, maximumDuration));
		}
	}

	public override IEnumerable<ICrime> SelectDispatchCrimes(ILegalAuthority authority)
	{
		return authority.KnownCrimes
		                .Where(x => IsEligibleResponseCrime(x, DispatchWindow))
		                .OrderByDescending(x => x.Law.EnforcementPriority)
		                .ThenByDescending(x => x.RealTimeOfCrime);
	}

	public static bool IsEligibleResponseCrime(ICrime crime, TimeSpan dispatchWindow)
	{
		return crime is not null &&
		       crime.IsKnownCrime &&
		       !crime.HasBeenFinalised &&
		       !crime.HasBeenEnforced &&
		       crime.CrimeLocation is not null &&
		       crime.Law.CrimeType.IsViolentCrime() &&
		       crime.Law.EnforcementStrategy > EnforcementStrategy.NoActiveEnforcement &&
		       DateTime.UtcNow - crime.RealTimeOfCrime <= dispatchWindow;
	}

	protected override bool TargetCrimeStillValid(IPatrol patrol)
	{
		return base.TargetCrimeStillValid(patrol) &&
		       IsEligibleResponseCrime(patrol.TargetCrime, DispatchWindow + MaximumDuration);
	}

	protected override void HandleArrivedAtTargetNode(IPatrol patrol, ICell node)
	{
		base.HandleArrivedAtTargetNode(patrol, node);

		if (DateTime.UtcNow - patrol.PatrolStartTime >= MaximumDuration ||
		    !patrol.PatrolRoute.TimeOfDays.Contains(patrol.PatrolLeader.Location.CurrentTimeOfDay))
		{
			patrol.CompletePatrol();
			return;
		}

		if (DateTime.UtcNow - patrol.LastArrivedTime < patrol.PatrolRoute.LingerTimeMajorNode)
		{
			return;
		}

		patrol.NextMajorNode = NextAreaNode(patrol, patrol.TargetCrime, node);
		if (patrol.NextMajorNode == node)
		{
			patrol.CompletePatrol();
		}
	}

	public override string HelpText => @"Reactive patrol configuration:

	#3radius <rooms>#0 - sets how far from a patrol node a violent reported crime may be to dispatch this patrol
	#3window <timespan>#0 - sets how recent a violent crime report must be to dispatch this patrol
	#3duration <timespan>#0 - sets how long the reactive patrol remains in the area";

	public override bool BuildingCommand(ICharacter actor, IPatrolRoute patrol, StringStack command)
	{
		switch (command.PeekSpeech().ToLowerInvariant())
		{
			case "window":
			case "dispatch":
				command.PopSpeech();
				return BuildingCommandTimeSpan(actor, command, "dispatch window", value => DispatchWindow = value);
			case "duration":
			case "length":
				command.PopSpeech();
				return BuildingCommandTimeSpan(actor, command, "maximum duration", value => MaximumDuration = value);
			default:
				return base.BuildingCommand(actor, patrol, command);
		}
	}

	private static bool BuildingCommandTimeSpan(ICharacter actor, StringStack command, string name, Action<TimeSpan> setter)
	{
		if (command.IsFinished || !MudTimeSpan.TryParse(command.SafeRemainingArgument, out MudTimeSpan value) ||
		    value.TotalSeconds <= 0.0)
		{
			actor.OutputHandler.Send($"What positive length of time should the {name} be?");
			return false;
		}

		setter(value);
		actor.OutputHandler.Send($"The reactive patrol {name} is now {value.Describe(actor).ColourValue()}.");
		return true;
	}

	public override string ShowConfiguration(ICharacter actor, IPatrolRoute patrol)
	{
		return base.ShowConfiguration(actor, patrol) +
		       $"Dispatch Window: {DispatchWindow.Describe(actor).ColourValue()}\n" +
		       $"Maximum Duration: {MaximumDuration.Describe(actor).ColourValue()}\n";
	}

	protected override void SaveAdditionalStrategyData(XElement root)
	{
		root.Add(
			new XAttribute("dispatchWindowSeconds", DispatchWindow.TotalSeconds.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("maximumDurationSeconds", MaximumDuration.TotalSeconds.ToString(CultureInfo.InvariantCulture)));
	}
}
