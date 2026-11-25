#nullable enable

using System.Globalization;
using MudSharp.Character.Name;
using MudSharp.Effects.Concrete;
using MudSharp.Network;
using MudSharp.RPG.Law;

namespace MudSharp.Discord;

internal sealed partial class DiscordRemoteService
{
	private const int JusticePageSize = 6;

	private string JusticeStatus(string selector, int page)
	{
		selector = selector.Trim();
		if (page < 1) { throw new ArgumentException("Page numbers start at 1."); }
		if (selector.Length == 0)
		{
			var jurisdictions = _world.LegalAuthorities.OrderBy(x => x.Id).ToList();
			var start = ReportPageStart(page, jurisdictions.Count, JusticePageSize);
			if (jurisdictions.Count == 0) { return "No jurisdictions are configured."; }
			var list = new StringBuilder("Select a jurisdiction with /justice status jurisdiction:<ID or exact name>.\n");
			foreach (var entry in jurisdictions.Skip(start).Take(JusticePageSize))
			{
				list.AppendLine($"#{entry.Id} {ReportLabel(entry.Name)}");
			}
			if (jurisdictions.Count - start > JusticePageSize) { list.AppendLine($"Continue with /justice status page:{page + 1}"); }
			return list.ToString().TrimEnd();
		}

		return JurisdictionReport(ResolveJurisdiction(selector), page);
	}

	private ILegalAuthority ResolveJurisdiction(string selector)
	{
		if (long.TryParse(selector.TrimStart('#'), NumberStyles.None, CultureInfo.InvariantCulture, out var id))
		{
			return _world.LegalAuthorities.Get(id) ?? throw new ArgumentException("Unknown jurisdiction ID. Use /justice status to list jurisdictions.");
		}
		var matches = _world.LegalAuthorities.Where(x => x.Name.Equals(selector, StringComparison.OrdinalIgnoreCase)).Take(2).ToList();
		if (matches.Count > 1) { throw new ArgumentException("Several jurisdictions share that name. Select one by ID."); }
		return matches.FirstOrDefault() ?? throw new ArgumentException("Unknown jurisdiction. Use /justice status to list names and IDs.");
	}

	private string JurisdictionReport(ILegalAuthority jurisdiction, int page)
	{
		var waiting = jurisdiction.KnownCrimes
			.Select(x => (Id: x.CriminalId, Defendant: x.Criminal))
			.DistinctBy(x => x.Defendant, ReferenceEqualityComparer.Instance)
			.Where(x => x.Defendant is { } defendant && !defendant.AffectedBy<OnTrial>(jurisdiction) &&
				(defendant.AffectedBy<AwaitingSentencing>(jurisdiction) || defendant.AffectedBy<OnBail>(jurisdiction)))
			.DistinctBy(x => x.Id)
			.OrderBy(x => x.Id)
			.ToList();
		var patrols = jurisdiction.Patrols.OrderBy(x => x.Id).ToList();
		var offset = ReportPageStart(page, Math.Max(waiting.Count, patrols.Count), JusticePageSize);
		var report = new StringBuilder($"Justice status — {ReportLabel(jurisdiction.Name)} (#{jurisdiction.Id})\n");
		report.AppendLine(jurisdiction.TrialRequestBlocker() is { } blocker
			? $"NPC hearing unavailable — this jurisdiction {blocker}."
			: "NPC judge available in court.");
		if (jurisdiction.CourtLocation is { } court)
		{
			var playerJudges = _world.Connections.Where(x => x.State == ConnectionState.Open)
				.Select(x => x.ControlPuppet?.Actor)
				.OfType<ICharacter>()
				.Where(x => x.Location == court && x.State.IsAble())
				.Count(actor => _world.LegalAuthorities.FirstOrDefault(a => a.CourtLocation == court &&
					a.HasJudgementAuthority(actor)) == jurisdiction);
			report.AppendLine($"Connected judges for player-held trials in court: {playerJudges}. Eligibility depends on the defendant.");
		}
		report.AppendLine($"Waiting for trial ({waiting.Count})");
		foreach (var (defendantId, defendant) in waiting.Skip(offset).Take(JusticePageSize))
		{
			var status = defendant!.AffectedBy<OnBail>(jurisdiction) ? "on bail" :
				jurisdiction.IsInRemandRoom(defendant) ? "in remand" : "at large / bail revoked";
			report.AppendLine($"#{defendantId} {ReportLabel(defendant.PersonalName.GetName(NameStyle.FullName))} — {status}");
		}
		report.AppendLine($"Active patrols ({patrols.Count})");
		foreach (var patrol in patrols.Skip(offset).Take(JusticePageSize))
		{
			var phase = patrol.PatrolPhase == PatrolPhase.Preperation ? "Preparation" : patrol.PatrolPhase.DescribeEnum();
			report.AppendLine($"#{patrol.Id} {ReportLabel(patrol.PatrolRoute.Name)} — {ReportLabel(patrol.PatrolStrategy.Name)}, {phase}, members: {patrol.PatrolMembers.Count()}");
		}
		if (Math.Max(waiting.Count, patrols.Count) - offset > JusticePageSize)
		{
			report.AppendLine($"Continue with /justice status jurisdiction:{jurisdiction.Id} page:{page + 1}");
		}
		return report.ToString().TrimEnd();
	}
}
