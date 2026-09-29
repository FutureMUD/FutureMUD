#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.NPC.AI;

public enum AnimalEngagementPurpose { Hunt, SelfDefence, Territory, ProtectYoung, SeasonalAggression, ThreatResponse }
public enum AnimalPeoplePreyPolicy { Never, Desperate, Eligible }
public enum AnimalPreySelection { Safest, Nearest, LargestManageable }
public enum AnimalHuntOpening { Direct, Ambush, TrapWait }
public enum AnimalHuntFollowup { Fight, Extract, VenomWithdrawal }
public enum AnimalHuntPhase { Approach, Opening, Fighting, Withdrawing, Shadowing, Abandoned }

/// <summary>Only observations available to the assessor and its own condition belong in this snapshot.</summary>
public sealed record AnimalAssessmentInput(
	double SizeAdvantage, double VisibleInjury, double VisibleVulnerability, double TacticalAdvantage,
	double SupportAdvantage, double VisibleArmament, double OwnInjury, double OwnFatigue);

public sealed record AnimalAssessmentResult(double Score, IReadOnlyDictionary<string, double> Contributions);

public interface IAnimalThreatAssessment
{
	AnimalAssessmentResult Assess(AnimalAssessmentInput input, IReadOnlyDictionary<string, double> weights,
		double confidenceBias = 0.0);
}
