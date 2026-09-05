#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Analytics;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.Work.Projects;

namespace MudSharp_Unit_Tests.Economy;

[TestClass]
public class EconomyEmploymentAnalyticsTests
{
	[TestMethod]
	public void CollectedProjectIncome_CompletedProjectAndRefund_RecordsOnlyLabourWithoutInventingZone()
	{
		var analytics = new Mock<IEconomyAnalyticsService>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.EconomyAnalytics).Returns(analytics.Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var payable = new MudSharp.Models.ProjectPayable
		{
			Id = 42, ActiveProjectId = 12, ProjectOwnerCharacterId = 7, CharacterId = 9,
			CurrencyId = 3, Amount = 15M, PayableType = (int)ProjectPayableType.Labour
		};
		ProjectPaymentService.RecordCollectedLabourIncome(actor.Object, payable);
		payable.PayableType = (int)ProjectPayableType.ProjectRefund;
		ProjectPaymentService.RecordCollectedLabourIncome(actor.Object, payable);
		analytics.Verify(x => x.RecordActivity(It.Is<EconomicActivityEvent>(e =>
			e.ActivityType == EconomicActivityType.ProjectPayment && e.CurrencyId == 3 && e.Amount == 15M &&
			e.SourceId == 7 && e.DestinationId == 9 && e.EconomicZoneId == null && e.ReferenceId == 42)), Times.Once);
		analytics.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void EmploymentSnapshot_CountsAndIncome_PreservesUnitsAndZeroValues()
	{
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(1);
		currency.SetupGet(x => x.BaseCurrencyToGlobalBaseCurrencyConversion).Returns(4M);
		var currencies = new All<ICurrency>();
		currencies.Add(currency.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Currencies).Returns(currencies);
		var service = new EconomyAnalyticsService(gameworld.Object);
		var market = EconomyAnalyticsService.BuildEmploymentMarket([], [], [Worker(1).Object], null, _ => 1);
		var volume = new EconomyVolumeResult(DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow,
			0M, 0M, new Dictionary<EconomicActivityType, decimal>(), new Dictionary<EconomicControlBucket, decimal>(), 1)
		{
			PcIncome = new Dictionary<(long, EconomicActivityType), decimal> { [(1, EconomicActivityType.Wage)] = 100M }
		};
		var snapshot = new MudSharp.Models.EconomySnapshot();
		service.AddEmploymentSnapshotEntries(snapshot, market, volume, currency.Object);

		var free = snapshot.Entries.Single(x => x.Metric == (int)EconomyHoldingMetric.NpcFree);
		Assert.AreEqual(1M, free.Amount);
		Assert.AreEqual(1M, free.GlobalBaseValue);
		var pay = snapshot.Entries.Single(x => x.Metric == (int)EconomyHoldingMetric.PcJobIncome);
		Assert.AreEqual(25M, pay.Amount);
		Assert.AreEqual(100M, pay.GlobalBaseValue);
		Assert.AreEqual(0M, snapshot.Entries.Single(x => x.Metric == (int)EconomyHoldingMetric.PcClanIncome).Amount);
		Assert.AreEqual(0M, snapshot.Entries.Single(x => x.Metric == (int)EconomyHoldingMetric.NpcEmployed).Amount);
	}

	[TestMethod]
	public void EmploymentMarket_VacanciesAndContracts_SeparatesPostingsPositionsAndUniqueWorkers()
	{
		var worker = Worker(1);
		var host = Host(1, 1);
		host.SetupGet(x => x.EmploymentContracts).Returns([
			Contract(host.Object, worker.Object, EmploymentRole.Manager, EmploymentStatus.Active),
			Contract(host.Object, worker.Object, EmploymentRole.MedicalWorker, EmploymentStatus.Suspended),
			Contract(host.Object, Worker(2).Object, EmploymentRole.Employee, EmploymentStatus.Ended)]);
		host.SetupGet(x => x.JobOpenings).Returns([
			Opening(host.Object, 1, false, 5, 2, true),
			Opening(host.Object, 2, true, 2, 0, true),
			Opening(host.Object, 3, false, 1, 1, false)]);
		var market = EconomyAnalyticsService.BuildEmploymentMarket([host.Object], [], [worker.Object], null, _ => 1);

		Assert.AreEqual(1, market.EmployedNpcs);
		Assert.AreEqual(0, market.FreeNpcs);
		Assert.AreEqual(2, market.Openings.Count);
		Assert.AreEqual(1, market.Openings.Count(x => x.AcceptsPcs));
		Assert.AreEqual(5, market.Hosts.Single().VacantPositions);
		Assert.AreEqual(1, market.Roles.Single(x => x.Name == EmploymentRole.Manager.DescribeEnum()).EmployedNpcs);
		Assert.AreEqual(1, market.Roles.Single(x => x.Name == EmploymentRole.MedicalWorker.DescribeEnum()).EmployedNpcs);
		var counts = EconomyAnalyticsService.EmploymentCounts(market);
		Assert.AreEqual(2, counts[EconomyHoldingMetric.HostVacantPostings]);
		Assert.AreEqual(5, counts[EconomyHoldingMetric.HostVacantPositions]);
	}

	[TestMethod]
	public void EmploymentMarket_FreePool_ExcludesJobsInOtherZonesAndDeduplicatesCapabilities()
	{
		var employedElsewhere = Worker(1);
		var free = Worker(2);
		var dead = Worker(3);
		dead.SetupGet(x => x.State).Returns(CharacterState.Dead);
		var disabled = Worker(4, false);
		var outside = Worker(5);
		var host = Host(1, 2);
		host.SetupGet(x => x.EmploymentContracts).Returns([
			Contract(host.Object, employedElsewhere.Object, EmploymentRole.Manager, EmploymentStatus.Suspended)]);
		var market = EconomyAnalyticsService.BuildEmploymentMarket([host.Object], [],
			[employedElsewhere.Object, free.Object, free.Object, dead.Object, disabled.Object, outside.Object],
			1, x => x.Id == 5 ? 2 : 1);

		Assert.AreEqual(0, market.EmployedNpcs);
		Assert.AreEqual(1, market.FreeNpcs);
		Assert.AreEqual(1, market.FreeCapabilities[EmploymentAICapability.CanPerformMedicalServices.DescribeEnum()]);
		Assert.AreEqual(1, market.FreeCapabilities[EmploymentAICapability.CanManageEmploymentHost.DescribeEnum()]);
		Assert.AreEqual(0, market.Hosts.Count);
	}

	[TestMethod]
	public void EmploymentMarket_LegacyListings_HandlesUnlimitedFullArchivedAndCompletedJobs()
	{
		var worker = Worker(1);
		var job = new Mock<IActiveJob>();
		job.SetupGet(x => x.Character).Returns(worker.Object);
		var unlimited = Listing(0, []);
		var full = Listing(1, [job.Object]);
		var archived = Listing(3, []);
		archived.SetupGet(x => x.IsArchived).Returns(true);
		var completedJob = new Mock<IActiveJob>();
		completedJob.SetupGet(x => x.IsJobComplete).Returns(true);
		var partlyVacant = Listing(2, [job.Object, completedJob.Object]);
		var market = EconomyAnalyticsService.BuildEmploymentMarket([], [unlimited.Object, full.Object,
			archived.Object, partlyVacant.Object], [worker.Object], null, _ => 1);

		Assert.AreEqual(2, market.Openings.Count);
		Assert.AreEqual(1, market.Openings.Count(x => x.VacantPositions is null));
		Assert.AreEqual(1, market.Openings.Single(x => x.VacantPositions.HasValue).VacantPositions);
		Assert.AreEqual(1, market.EmployedNpcs);
		Assert.AreEqual(0, market.FreeNpcs);
	}

	[TestMethod]
	public void IncomeAggregation_RecipientAndClassification_ExcludesPcEmployersNpcsStaffAndInternalTransfers()
	{
		var rows = new[]
		{
			Activity(EconomicControlBucket.Institutional, EconomicControlBucket.DirectPc, 10),
			Activity(EconomicControlBucket.DirectPc, EconomicControlBucket.Npc, 20),
			Activity(EconomicControlBucket.DirectPc, EconomicControlBucket.SharedPcControlled, 30),
			Activity(EconomicControlBucket.Institutional, EconomicControlBucket.Staff, 40),
			Activity(EconomicControlBucket.Institutional, EconomicControlBucket.DirectPc, 50,
				EconomicVolumeClassification.InternalMovement),
			Activity(EconomicControlBucket.Institutional, EconomicControlBucket.DirectPc, 60)
		};
		rows[5].CurrencyId = 2;
		var result = EconomyAnalyticsService.BuildVolumeAggregateQuery(rows.AsQueryable()).ToList();
		Assert.AreEqual(70M, result.Sum(x => x.PcIncomeGlobalBaseValue));
		Assert.AreEqual(10M, result.Where(x => x.CurrencyId == 1).Sum(x => x.PcIncomeGlobalBaseValue));
		Assert.AreEqual(210M, result.Sum(x => x.TotalGlobalBaseValue));
	}

	private static MudSharp.Models.EconomicActivityRecord Activity(EconomicControlBucket source,
		EconomicControlBucket destination, decimal value,
		EconomicVolumeClassification classification = EconomicVolumeClassification.Exchange)
	{
		return new MudSharp.Models.EconomicActivityRecord
		{
			CurrencyId = 1, ActivityType = (int)EconomicActivityType.Wage,
			SourceControlBucket = (int)source, DestinationControlBucket = (int)destination,
			DestinationType = "Character", GlobalBaseValue = value, VolumeClassification = (int)classification
		};
	}

	private static Mock<IJobListing> Listing(int maximum, IActiveJob[] jobs)
	{
		var listing = new Mock<IJobListing>();
		listing.SetupGet(x => x.IsReadyToBePosted).Returns(true);
		listing.SetupGet(x => x.MaximumNumberOfSimultaneousEmployees).Returns(maximum);
		listing.SetupGet(x => x.ActiveJobs).Returns(jobs);
		listing.SetupGet(x => x.Employer).Returns(Mock.Of<IFrameworkItem>());
		return listing;
	}

	private static Mock<INPC> Worker(long id, bool search = true)
	{
		var ai = (EmploymentWorkerAI)Activator.CreateInstance(typeof(EmploymentWorkerAI), true)!;
		typeof(EmploymentWorkerAI).GetProperty(nameof(EmploymentWorkerAI.SearchEnabled))!.SetValue(ai, search);
		var capabilities = (HashSet<EmploymentAICapability>)typeof(EmploymentWorkerAI)
			.GetField("_capabilities", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(ai)!;
		capabilities.Add(EmploymentAICapability.CanPerformMedicalServices);
		capabilities.Add(EmploymentAICapability.CanManageEmploymentHost);
		var worker = new Mock<INPC>();
		worker.SetupGet(x => x.Id).Returns(id);
		worker.SetupGet(x => x.AIs).Returns([ai, ai]);
		return worker;
	}

	private static Mock<IEmploymentHost> Host(long id, long zoneId)
	{
		var zone = new Mock<IEconomicZone>();
		zone.SetupGet(x => x.Id).Returns(zoneId);
		var market = new Mock<IMarket>();
		market.SetupGet(x => x.EconomicZone).Returns(zone.Object);
		var host = new Mock<IEmploymentHost>();
		host.SetupGet(x => x.Id).Returns(id);
		host.SetupGet(x => x.Market).Returns(market.Object);
		host.SetupGet(x => x.EmploymentContracts).Returns([]);
		host.SetupGet(x => x.JobOpenings).Returns([]);
		return host;
	}

	private static IEmploymentContract Contract(IEmploymentHost host, ICharacter worker, EmploymentRole role,
		EmploymentStatus status)
	{
		var contract = new Mock<IEmploymentContract>();
		contract.SetupGet(x => x.Employer).Returns(host);
		contract.SetupGet(x => x.Employee).Returns(worker);
		contract.SetupGet(x => x.Role).Returns(role);
		contract.SetupGet(x => x.Status).Returns(status);
		return contract.Object;
	}

	private static IJobOpening Opening(IEmploymentHost host, long id, bool npcOnly, int maximum, int occupied,
		bool accepts)
	{
		var opening = new Mock<IJobOpening>();
		opening.SetupGet(x => x.Employer).Returns(host);
		opening.SetupGet(x => x.Id).Returns(id);
		opening.SetupGet(x => x.NpcApplicationsOnly).Returns(npcOnly);
		opening.SetupGet(x => x.MaxPositions).Returns(maximum);
		opening.SetupGet(x => x.OccupiedPositions).Returns(occupied);
		opening.SetupGet(x => x.AcceptsApplications).Returns(accepts);
		return opening.Object;
	}
}
