#nullable enable

using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.NPC.AI;

namespace MudSharp.Economy.Analytics;

public sealed partial class EconomyAnalyticsService
{
	public EconomyEmploymentMarket GetEmploymentMarket(long? economicZoneId = null)
	{
		return BuildEmploymentMarket(EmploymentHostDiscovery.LoadedHosts(_gameworld), _gameworld.JobListings,
			_gameworld.NPCs, economicZoneId, character => character.Location is null
				? null : ResolveCellZone(character.Location.Id));
	}

	internal static EconomyEmploymentMarket BuildEmploymentMarket(IEnumerable<IEmploymentHost> hosts,
		IEnumerable<IJobListing> listings, IEnumerable<ICharacter> npcs, long? zoneId,
		Func<ICharacter, long?> characterZone)
	{
		var allHosts = hosts.DistinctBy(x => (x.FrameworkItemType, x.Id)).ToList();
		var allListings = listings.ToList();
		var allContracts = allHosts.SelectMany(x => x.EmploymentContracts)
			.Where(x => x.Status is EmploymentStatus.Active or EmploymentStatus.Suspended).ToList();
		var employedIds = allContracts.Select(x => x.Employee.Id)
			.Concat(allListings.SelectMany(x => x.ActiveJobs).Where(x => !x.IsJobComplete)
				.Select(x => x.Character.Id)).ToHashSet();
		var selectedHosts = allHosts.Where(x => !zoneId.HasValue || EmploymentClock.EconomicZone(x)?.Id == zoneId)
			.ToList();
		var contracts = selectedHosts.SelectMany(x => x.EmploymentContracts)
			.Where(x => x.Status is EmploymentStatus.Active or EmploymentStatus.Suspended)
			.Where(x => x.Employee is INPC).ToList();
		var openings = selectedHosts.SelectMany(x => x.JobOpenings).Where(x => x.AcceptsApplications).ToList();
		var legacyListings = allListings.Where(x => !zoneId.HasValue || x.EconomicZone.Id == zoneId).ToList();
		var jobs = openings.Select(x => new EconomyJobOpening(x.Employer.EmploymentHostName,
			x.Role.DescribeEnum(), x.Id, false, !x.NpcApplicationsOnly,
			Math.Max(0, x.MaxPositions - x.OccupiedPositions))).ToList();
		jobs.AddRange(legacyListings.Where(x => x.IsReadyToBePosted && !x.IsArchived)
			.Select(x => new EconomyJobOpening(x.Employer.Name, x.Name, x.Id, true, true,
				x.MaximumNumberOfSimultaneousEmployees <= 0 ? null :
					Math.Max(0, x.MaximumNumberOfSimultaneousEmployees - x.ActiveJobs.Count(y => !y.IsJobComplete))))
			.Where(x => x.VacantPositions is null or > 0));
		var free = npcs.OfType<INPC>().DistinctBy(x => x.Id)
			.Where(x => !employedIds.Contains(x.Id) && !x.State.HasFlag(CharacterState.Dead))
			.Where(x => !zoneId.HasValue || characterZone(x) == zoneId)
			.Select(x => x.AIs.OfType<EmploymentWorkerAI>().Where(y => y.SearchEnabled).ToList())
			.Where(x => x.Count > 0).ToList();
		var capabilities = Enum.GetValues<EmploymentAICapability>().ToDictionary(x => x.DescribeEnum(),
			x => free.Count(y => y.Any(z => z.Capabilities.Contains(x))));
		var hostRows = selectedHosts.Select(host => new EconomyEmploymentBreakdown(
			$"{host.EmploymentHostName} ({host.EmploymentHostType.DescribeEnum()} #{host.Id})",
			contracts.Where(x => x.Employer == host).Select(x => x.Employee.Id).Distinct().Count(),
			openings.Count(x => x.Employer == host),
			openings.Where(x => x.Employer == host).Sum(x => Math.Max(0, x.MaxPositions - x.OccupiedPositions))))
			.ToList();
		var roles = Enum.GetValues<EmploymentRole>().Select(role => new EconomyEmploymentBreakdown(
			role.DescribeEnum(), contracts.Where(x => x.Role == role).Select(x => x.Employee.Id).Distinct().Count(),
			openings.Count(x => x.Role == role),
			openings.Where(x => x.Role == role).Sum(x => Math.Max(0, x.MaxPositions - x.OccupiedPositions))))
			.ToList();
		return new EconomyEmploymentMarket(jobs, contracts.Select(x => x.Employee.Id)
			.Concat(legacyListings.SelectMany(x => x.ActiveJobs).Where(x => !x.IsJobComplete && x.Character is INPC)
				.Select(x => x.Character.Id)).Distinct().Count(), free.Count, hostRows, roles, capabilities);
	}

	internal static IReadOnlyDictionary<EconomyHoldingMetric, int> EmploymentCounts(EconomyEmploymentMarket market)
	{
		return new Dictionary<EconomyHoldingMetric, int>
		{
			[EconomyHoldingMetric.PcOpenJobPostings] = market.Openings.Count(x => x.AcceptsPcs),
			[EconomyHoldingMetric.NpcEmployed] = market.EmployedNpcs,
			[EconomyHoldingMetric.NpcFree] = market.FreeNpcs,
			[EconomyHoldingMetric.HostVacantPostings] = market.Openings.Count(x => !x.Legacy),
			[EconomyHoldingMetric.HostVacantPositions] = market.Openings.Where(x => !x.Legacy).Sum(x => x.VacantPositions ?? 0),
			[EconomyHoldingMetric.NpcMedicalWorkers] = market.Roles.First(x => x.Name == EmploymentRole.MedicalWorker.DescribeEnum()).EmployedNpcs,
			[EconomyHoldingMetric.NpcManagers] = market.Roles.First(x => x.Name == EmploymentRole.Manager.DescribeEnum()).EmployedNpcs,
			[EconomyHoldingMetric.FreeMedicalWorkers] = market.FreeCapabilities.GetValueOrDefault(EmploymentAICapability.CanPerformMedicalServices.DescribeEnum()),
			[EconomyHoldingMetric.FreeManagers] = market.FreeCapabilities.GetValueOrDefault(EmploymentAICapability.CanManageEmploymentHost.DescribeEnum())
		};
	}

	internal void AddEmploymentSnapshotEntries(Models.EconomySnapshot snapshot, EconomyEmploymentMarket market,
		EconomyVolumeResult volume, ICurrency primaryCurrency)
	{
		foreach (var (metric, count) in EmploymentCounts(market))
		{
			snapshot.Entries.Add(new Models.EconomySnapshotEntry
			{
				CurrencyId = primaryCurrency.Id, Metric = (int)metric,
				ControlBucket = (int)EconomicControlBucket.Institutional,
				Amount = count, GlobalBaseValue = count, EntityCount = count
			});
		}

		foreach (var currency in _gameworld.Currencies.Where(x => x.BaseCurrencyToGlobalBaseCurrencyConversion > 0.0M))
		{
			foreach (var (activity, metric) in new[]
			         {
				         (EconomicActivityType.Wage, EconomyHoldingMetric.PcJobIncome),
				         (EconomicActivityType.ProjectPayment, EconomyHoldingMetric.PcProjectIncome),
				         (EconomicActivityType.ClanPayment, EconomyHoldingMetric.PcClanIncome)
			         })
			{
				var value = volume.PcIncome.GetValueOrDefault((currency.Id, activity));
				snapshot.Entries.Add(new Models.EconomySnapshotEntry
				{
					CurrencyId = currency.Id, Metric = (int)metric,
					ControlBucket = (int)EconomicControlBucket.DirectPc,
					GlobalBaseValue = value, Amount = value / currency.BaseCurrencyToGlobalBaseCurrencyConversion
				});
			}
		}
	}
}
