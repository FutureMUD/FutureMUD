#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PsychicWitnessReportingTests
{
	[TestMethod]
	public void Report_RestartRecoveryDeliversOnceAndNeverUsesCriminalAsWitness()
	{
		var memory = new CrimeWitnessMemory { Kind = CrimeWitnessSourceKind.Virtual, SourceId = 7, ReportDueUtc = DateTime.UtcNow.AddMinutes(-1), IdentityKnown = true, Reliability = 0.8 };
		var crime = Load(new XElement("Witnesses", memory.Save()).ToString(), "", out _, out var schedules);
		var authority = Mock.Get(crime.LegalAuthority);
		schedules.Single().Fire();
		schedules.Single().Fire();
		Assert.IsTrue(crime.WitnessMemories.Single().ReportDelivered);
		Assert.IsNull(crime.WitnessMemories.Single().ReportDueUtc);
		authority.Verify(x => x.ReportVirtualCrime(crime, It.Is<CrimeWitnessMemory>(m => m.SourceId == 7 && m.IdentityKnown && m.Reliability == 0.8)), Times.Once);
		authority.Verify(x => x.ReportCrime(It.IsAny<ICrime>(), It.IsAny<ICharacter>(), It.IsAny<bool>(), It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	public void Report_ImmediateDefaultIsSynchronousAndFinalisationCancelsPendingDelivery()
	{
		var crime = Load(null, "", out _, out var schedules);
		crime.QueueVirtualReport(7, true, 0.7, TimeSpan.Zero);
		Assert.IsTrue(crime.WitnessMemories.Single().ReportDelivered);
		Assert.AreEqual(0, schedules.Count);
		crime.QueueVirtualReport(8, false, 0.5, TimeSpan.FromMinutes(1));
		crime.HasBeenFinalised = true;
		schedules.Single().Fire();
		Assert.IsFalse(crime.WitnessMemories[1].ReportDelivered);
		Assert.IsNull(crime.WitnessMemories[1].ReportDueUtc);
		Mock.Get(crime.LegalAuthority).Verify(x => x.ReportVirtualCrime(crime, It.IsAny<CrimeWitnessMemory>()), Times.Once);
	}

	[TestMethod]
	public void Load_LegacyWitnessesGainRecallWithoutInventingVirtualReports()
	{
		var crime = Load(null, "42 43", out _, out var schedules);
		Assert.IsTrue(crime.CanWitnessRecall(42));
		Assert.IsTrue(crime.CanWitnessRecall(43));
		Assert.AreEqual(2, crime.WitnessMemories.Count);
		Assert.AreEqual(0, schedules.Count);
		var caster = new Mock<ICharacter>();
		caster.SetupGet(x => x.Id).Returns(99);
		crime.ForgetWitness(crime.WitnessMemories[0], caster.Object, TimeSpan.FromMinutes(5), false);
		Assert.IsFalse(crime.CanWitnessRecall(42));
		Assert.IsTrue(crime.CanWitnessRecall(43));
		crime.RestoreWitness(crime.WitnessMemories[0], caster.Object);
		Assert.IsTrue(crime.CanWitnessRecall(42));
		Assert.AreEqual(2, crime.WitnessMemories[0].Audit.Count);
	}

	[TestMethod]
	public void Restart_SchedulesOnlyPendingAvailableSourcesAndDefersSuppressedSources()
	{
		var now = DateTime.UtcNow;
		var pending = new CrimeWitnessMemory { Kind = CrimeWitnessSourceKind.Virtual, SourceId = 1, ReportDueUtc = now.AddSeconds(-1) };
		pending.Forget(now, TimeSpan.FromMinutes(5), false, 99);
		var delivered = new CrimeWitnessMemory { Kind = CrimeWitnessSourceKind.Virtual, SourceId = 2, ReportDelivered = true, ReportDueUtc = now };
		var permanent = new CrimeWitnessMemory { Kind = CrimeWitnessSourceKind.Virtual, SourceId = 3, ReportDueUtc = now };
		permanent.Forget(now, TimeSpan.Zero, true, 99);
		var crime = Load(new XElement("Witnesses", pending.Save(), delivered.Save(), permanent.Save()).ToString(), "", out _, out var schedules);
		Assert.AreEqual(1, schedules.Count);
		Assert.IsTrue(schedules[0].TriggerETA >= pending.SuppressedUntilUtc!.Value);
		schedules[0].Fire(); // Even an early callback must recheck recall.
		Assert.AreEqual(2, schedules.Count);
		Assert.IsFalse(crime.WitnessMemories[0].ReportDelivered);
		Assert.IsTrue(crime.WitnessMemories[1].ReportDelivered);
		Assert.IsTrue(crime.WitnessMemories[2].PermanentlyForgotten);
	}

	[TestMethod]
	public void PendingBurst_ProcessesOnlyScheduledSourcesWithoutWorldCrimeEnumeration()
	{
		var now = DateTime.UtcNow;
		var xml = new XElement("Witnesses", Enumerable.Range(1, 1000).Select(id =>
		{
			var memory = new CrimeWitnessMemory { Kind = CrimeWitnessSourceKind.Virtual, SourceId = id, ReportDueUtc = now };
			memory.Forget(now, TimeSpan.FromMinutes(1), false, 9);
			return memory.Save();
		}));
		var crime = Load(xml.ToString(), "", out var world, out var schedules);
		Assert.AreEqual(1000, schedules.Count);
		foreach (var schedule in schedules.ToArray()) schedule.Fire();
		Assert.AreEqual(2000, schedules.Count);
		Assert.IsFalse(crime.WitnessMemories.Any(x => x.ReportDelivered));
		world.VerifyGet(x => x.Crimes, Times.Never);
		world.VerifyGet(x => x.LegalAuthorities, Times.Never);
	}

	private static Crime Load(string? memory, string ids, out Mock<IFuturemud> world, out List<ISchedule> schedules)
	{
		var pending = new List<ISchedule>();
		schedules = pending;
		var scheduler = new Mock<IScheduler>();
		scheduler.Setup(x => x.AddSchedule(It.IsAny<ISchedule>())).Callback<ISchedule>(pending.Add);
		world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Scheduler).Returns(scheduler.Object);
		world.SetupGet(x => x.Cells).Returns(new All<ICell>());
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		var law = new Mock<ILaw>();
		law.SetupGet(x => x.Gameworld).Returns(world.Object);
		law.SetupGet(x => x.Authority).Returns(new Mock<ILegalAuthority>().Object);
		return new Crime(new MudSharp.Models.Crime
		{
			Id = 1, CriminalId = 2, TimeOfCrime = MudDateTime.Never.GetDateTimeString(), RealTimeOfCrime = DateTime.UtcNow,
			CriminalCharacteristics = "", WitnessIds = ids, WitnessMemory = memory,
			CriminalShortDescription = "a person", CriminalFullDescription = "A person."
		}, law.Object, world.Object);
	}
}
