#nullable enable

using MudSharp.Effects.Concrete;

namespace MudSharp.RPG.Law;

internal static class TrialCourt
{
	internal static bool HasJudgementAuthority(this ILegalAuthority jurisdiction, ICharacter actor) =>
		actor.IsAdministrator() || jurisdiction.GetEnforcementAuthority(actor)?.CanConvict == true;

	/// <summary>The court-side reason an NPC hearing cannot begin for an otherwise eligible prisoner.</summary>
	internal static string? TrialRequestBlocker(this ILegalAuthority jurisdiction)
	{
		var court = jurisdiction.CourtLocation;
		if (court is null) { return "does not have a courtroom"; }
		if (court.Characters.Any(x => x.AffectedBy<OnTrial>(jurisdiction))) { return "already has a trial being heard"; }
		var judgePatrols = jurisdiction.Patrols.Where(x => x.PatrolStrategy.Name == "Judge").ToList();
		if (judgePatrols.Count == 0) { return "does not have any hearings scheduled"; }
		return judgePatrols.Any(x => x.PatrolLeader?.Location == court)
			? null
			: "does not have a judge available in the courtroom";
	}
}
