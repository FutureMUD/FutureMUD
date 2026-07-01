using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Economy.Property;

#nullable enable

namespace MudSharp.Economy.Employment;

public enum EmploymentLegacyBridgeDivergenceKind
{
	LegacyActiveJobWithoutContract,
	EmploymentContractWithoutLegacyJob,
	LegacyActiveJobOutsideEmploymentHost
}

public sealed record EmploymentLegacyBridgeDivergence(
	EmploymentLegacyBridgeDivergenceKind Kind,
	string Employer,
	string Character,
	string LegacyJob,
	string EmploymentContract,
	string Details);

public sealed record EmploymentLegacyBridgeReport(
	int EmploymentHostCount,
	int ActiveEmploymentContractCount,
	int LegacyJobListingCount,
	int ActiveLegacyJobCount,
	int HostBackedLegacyJobCount,
	int MatchedHostBackedJobCount,
	IReadOnlyCollection<EmploymentLegacyBridgeDivergence> Divergences);

public static class EmploymentLegacyBridgeReporter
{
	public static EmploymentLegacyBridgeReport Build(IFuturemud gameworld)
	{
		var hosts = EmploymentHosts(gameworld)
		            .DistinctBy(x => (x.EmploymentHostType, x.Id))
		            .ToList();
		var contracts = hosts
		                .SelectMany(host => host.EmploymentContracts
		                                       .Where(x => x.Status is EmploymentStatus.Active or EmploymentStatus.Suspended)
		                                       .Select(contract => (Host: host, Contract: contract)))
		                .ToList();
		var legacyListings = (gameworld.JobListings ?? Enumerable.Empty<IJobListing>())
		                     .DistinctBy(x => x.Id)
		                     .ToList();
		var legacyJobs = (gameworld.ActiveJobs ?? Enumerable.Empty<IActiveJob>())
		                 .Where(x => !x.IsJobComplete)
		                 .DistinctBy(x => x.Id)
		                 .ToList();
		var divergences = new List<EmploymentLegacyBridgeDivergence>();
		var hostBackedLegacyJobs = 0;
		var matchedHostBackedJobs = 0;

		foreach (var job in legacyJobs.OrderBy(x => x.Listing.Name).ThenBy(x => x.Character.Name))
		{
			if (job.Listing.Employer is not IEmploymentHost host)
			{
				divergences.Add(new EmploymentLegacyBridgeDivergence(
					EmploymentLegacyBridgeDivergenceKind.LegacyActiveJobOutsideEmploymentHost,
					DescribeEmployer(job.Listing.Employer),
					DescribeCharacter(job.Character),
					DescribeLegacyJob(job),
					string.Empty,
					"Legacy PC job is outside the employment-host model and should not be bootstrapped automatically."));
				continue;
			}

			hostBackedLegacyJobs++;
			var hasContract = contracts.Any(x => SameHost(x.Host, host) && SameCharacter(x.Contract.Employee, job.Character));
			if (hasContract)
			{
				matchedHostBackedJobs++;
				continue;
			}

			divergences.Add(new EmploymentLegacyBridgeDivergence(
				EmploymentLegacyBridgeDivergenceKind.LegacyActiveJobWithoutContract,
				DescribeHost(host),
				DescribeCharacter(job.Character),
				DescribeLegacyJob(job),
				string.Empty,
				"Legacy active job has no active or suspended employment-host contract for the same character."));
		}

		foreach (var (host, contract) in contracts.OrderBy(x => x.Host.EmploymentHostName).ThenBy(x => x.Contract.Employee.Name))
		{
			var hasLegacyJob = legacyJobs.Any(x => x.Listing.Employer is IEmploymentHost legacyHost &&
			                                     SameHost(legacyHost, host) &&
			                                     SameCharacter(x.Character, contract.Employee));
			if (hasLegacyJob)
			{
				continue;
			}

			divergences.Add(new EmploymentLegacyBridgeDivergence(
				EmploymentLegacyBridgeDivergenceKind.EmploymentContractWithoutLegacyJob,
				DescribeHost(host),
				DescribeCharacter(contract.Employee),
				string.Empty,
				DescribeContract(contract),
				"Employment-host contract has no matching legacy active job. This is expected for new-model-only staff but useful during migration audits."));
		}

		return new EmploymentLegacyBridgeReport(
			hosts.Count,
			contracts.Count,
			legacyListings.Count,
			legacyJobs.Count,
			hostBackedLegacyJobs,
			matchedHostBackedJobs,
			divergences);
	}

	public static string Render(EmploymentLegacyBridgeReport report, ICharacter? voyeur = null)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Employment legacy bridge divergence report");
		sb.AppendLine(
			$"Employment hosts: {report.EmploymentHostCount.ToString("N0", voyeur).ColourValue()}; active/suspended contracts: {report.ActiveEmploymentContractCount.ToString("N0", voyeur).ColourValue()}; legacy listings: {report.LegacyJobListingCount.ToString("N0", voyeur).ColourValue()}; active legacy jobs: {report.ActiveLegacyJobCount.ToString("N0", voyeur).ColourValue()}.");
		sb.AppendLine(
			$"Host-backed legacy jobs: {report.HostBackedLegacyJobCount.ToString("N0", voyeur).ColourValue()}, matched to employment contracts: {report.MatchedHostBackedJobCount.ToString("N0", voyeur).ColourValue()}.");
		if (!report.Divergences.Any())
		{
			sb.AppendLine("No legacy/new employment divergences were found.");
			return sb.ToString();
		}

		var rows = report.Divergences
		                 .OrderBy(x => x.Kind)
		                 .ThenBy(x => x.Employer)
		                 .ThenBy(x => x.Character)
		                 .Select(x => new List<string>
		                 {
			                 x.Kind.DescribeEnum(),
			                 x.Employer,
			                 x.Character,
			                 string.IsNullOrWhiteSpace(x.LegacyJob) ? "-" : x.LegacyJob,
			                 string.IsNullOrWhiteSpace(x.EmploymentContract) ? "-" : x.EmploymentContract,
			                 x.Details
		                 })
		                 .ToList();
		sb.AppendLine();
		sb.AppendLine(voyeur is null
			? StringUtilities.GetTextTable(rows, new[] { "Kind", "Employer", "Character", "Legacy Job", "Contract", "Details" }, 120, true, Telnet.Green)
			: StringUtilities.GetTextTable(rows, new[] { "Kind", "Employer", "Character", "Legacy Job", "Contract", "Details" }, voyeur, Telnet.Green));
		return sb.ToString();
	}

	private static bool SameHost(IEmploymentHost lhs, IEmploymentHost rhs)
	{
		return lhs.EmploymentHostType == rhs.EmploymentHostType && lhs.Id == rhs.Id;
	}

	private static bool SameCharacter(ICharacter lhs, ICharacter rhs)
	{
		return CharacterInstanceIdentityComparer.IdentityId(lhs) == CharacterInstanceIdentityComparer.IdentityId(rhs);
	}

	private static string DescribeHost(IEmploymentHost host)
	{
		return $"{host.EmploymentHostType.DescribeEnum()} #{host.Id:N0} {host.EmploymentHostName}";
	}

	private static string DescribeEmployer(IFrameworkItem employer)
	{
		return $"{employer.FrameworkItemType} #{employer.Id:N0} {employer.Name}";
	}

	private static string DescribeCharacter(ICharacter character)
	{
		return $"#{CharacterInstanceIdentityComparer.IdentityId(character):N0} {character.Name}";
	}

	private static string DescribeLegacyJob(IActiveJob job)
	{
		return $"#{job.Id:N0} {job.Listing.Name}";
	}

	private static string DescribeContract(IEmploymentContract contract)
	{
		return $"#{contract.Id:N0} {contract.Role.DescribeEnum()} {contract.Status.DescribeEnum()}";
	}

	private static IEnumerable<IEmploymentHost> EmploymentHosts(IFuturemud gameworld)
	{
		return EmploymentHostDiscovery.LoadedHosts(gameworld);
	}
}
