#nullable enable

using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;

namespace MudSharp.Commands.Modules;

internal partial class EconomyModule
{
	private static void EconomyAnalyticsEmployment(ICharacter actor, StringStack command)
	{
		long? zoneId = null;
		var details = false;
		while (!command.IsFinished)
		{
			var token = command.PopSpeech();
			if (token.EqualTo("details"))
			{
				details = true;
				continue;
			}
			if (!TryResolveAnalyticsZone(actor, token, true, out zoneId))
			{
				return;
			}
		}

		var market = actor.Gameworld.EconomyAnalytics.GetEmploymentMarket(zoneId);
		var pcJobs = market.Openings.Where(x => x.AcceptsPcs).ToList();
		var sb = new StringBuilder();
		sb.AppendLine("Employment Market".ColourName());
		sb.AppendLine($"PC-accessible postings: {pcJobs.Count.ToString("N0", actor).ColourValue()}; finite vacancies: {pcJobs.Sum(x => x.VacantPositions ?? 0).ToString("N0", actor).ColourValue()}; unlimited postings: {pcJobs.Count(x => x.VacantPositions is null).ToString("N0", actor).ColourValue()}.");
		sb.AppendLine($"NPC workers employed: {market.EmployedNpcs.ToString("N0", actor).ColourValue()}; free job-seeking NPCs: {market.FreeNpcs.ToString("N0", actor).ColourValue()}.");
		sb.AppendLine($"Employment-host vacant postings: {market.Openings.Count(x => !x.Legacy).ToString("N0", actor).ColourValue()}; vacant positions: {market.Openings.Where(x => !x.Legacy).Sum(x => x.VacantPositions ?? 0).ToString("N0", actor).ColourValue()}.");
		sb.AppendLine("Active and suspended contracts count as employed. Free workers are loaded, living NPCs with job search enabled and no current job anywhere; eligibility, wages, schedules and immediate availability still apply. Host totals use the employer's zone; free workers use their current location.");
		sb.AppendLine(StringUtilities.GetTextTable(market.Roles
			.Where(x => x.EmployedNpcs > 0 || x.VacantPostings > 0)
			.Select(x => new[] { x.Name, x.EmployedNpcs.ToString("N0", actor), x.VacantPostings.ToString("N0", actor), x.VacantPositions.ToString("N0", actor) }),
			new[] { "Host Role", "NPC Workers", "Postings", "Vacancies" }, actor.LineFormatLength,
			colour: Telnet.Yellow, unicodeTable: actor.Account.UseUnicode));
		sb.AppendLine(StringUtilities.GetTextTable(market.FreeCapabilities.Where(x => x.Value > 0)
			.Select(x => new[] { x.Key, x.Value.ToString("N0", actor) }),
			new[] { "Free Worker Capability (may overlap)", "NPCs" }, actor.LineFormatLength,
			colour: Telnet.Yellow, unicodeTable: actor.Account.UseUnicode));
		if (details)
		{
			sb.AppendLine("Hosts and openings: first 100 of each, ordered by name. Legacy jobs are included in overall totals and opening details; host role tables cover unified employment.");
			sb.AppendLine(StringUtilities.GetTextTable(market.Hosts.OrderBy(x => x.Name).Take(100)
				.Select(x => new[] { x.Name, x.EmployedNpcs.ToString("N0", actor), x.VacantPostings.ToString("N0", actor), x.VacantPositions.ToString("N0", actor) }),
				new[] { "Host", "NPC Workers", "Postings", "Vacancies" }, actor.LineFormatLength,
				colour: Telnet.Yellow, unicodeTable: actor.Account.UseUnicode));
			sb.AppendLine(StringUtilities.GetTextTable(market.Openings.OrderBy(x => x.Employer).ThenBy(x => x.Id).Take(100)
				.Select(x => new[] { x.Id.ToString("N0", actor), x.Legacy ? "Legacy" : "Host", x.Employer, x.Role,
					x.AcceptsPcs ? "PC/NPC" : "NPC only", x.VacantPositions?.ToString("N0", actor) ?? "Unlimited" }),
				new[] { "ID", "System", "Employer", "Job/Role", "Applicants", "Vacancies" }, actor.LineFormatLength,
				colour: Telnet.Yellow, unicodeTable: actor.Account.UseUnicode));
		}
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void EconomyAnalyticsIncome(ICharacter actor, EconomyVolumeResult result, EconomyQueryWindowKind window,
		long? zoneId)
	{
		var displayCurrency = actor.Gameworld.EconomyAnalytics.GlobalDisplayCurrency;
		var sb = new StringBuilder();
		sb.AppendLine("PC Employment Income".ColourName());
		sb.AppendLine($"Window: {window.DescribeEnum().ColourName()}; zone: {(zoneId.HasValue ? actor.Gameworld.EconomicZones.Get(zoneId.Value)?.Name ?? zoneId.Value.ToString("N0", actor) : "All").ColourName()}.");
		if (result.WindowStartUtc != DateTime.MinValue)
		{
			sb.AppendLine($"UTC interval: {result.WindowStartUtc.ToString("g", actor).ColourValue()} to {result.WindowEndUtc.ToString("g", actor).ColourValue()}.");
		}
		sb.AppendLine("Collected/settled pay to ordinary PCs only; excludes unpaid accruals, staff avatars, NPCs and organisational receipts. Income hooks collect forward from deployment; earlier ledger coverage does not imply complete payroll history.");
		sb.AppendLine($"Ledger coverage begins: {result.CoverageStartUtc.ToString("g", actor).ColourValue()}");
		foreach (var (activity, label) in new[] { (EconomicActivityType.Wage, "Jobs"),
			         (EconomicActivityType.ProjectPayment, "Projects"), (EconomicActivityType.ClanPayment, "Clan paydays") })
		{
			sb.AppendLine($"{label}: {DescribeGlobalValue(displayCurrency, result.PcIncome.Where(x => x.Key.Activity == activity).Sum(x => x.Value)).ColourValue()}");
		}
		sb.AppendLine($"Total PC income: {DescribeGlobalValue(displayCurrency, result.PcIncome.Values.Sum()).ColourValue()}");
		actor.OutputHandler.Send(sb.ToString());
	}
}
