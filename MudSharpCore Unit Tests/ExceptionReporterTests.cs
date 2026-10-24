#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Server;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using Moq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ExceptionReporterTests
{
	[TestMethod]
	public void Format_BrokenAggregateChild_RetainsHealthySiblingStack()
	{
		var error = new AggregateException(new BrokenTextException(), CaptureOriginalFailure());
		var report = ExceptionReporter.Format(error, "test", false);
		StringAssert.Contains(report, nameof(CaptureOriginalFailure));
		StringAssert.Contains(report, "healthy sibling failure");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static Exception CaptureOriginalFailure()
	{
		try { throw new InvalidOperationException("healthy sibling failure"); }
		catch (Exception error) { return error; }
	}

	[TestMethod]
	public void Format_BothMySqlProviders_RecordsCodesInNestedFailures()
	{
		// Providers construct coded errors internally; use their pinned constructor contracts without a live server.
		var connector = (Exception)typeof(MySqlConnector.MySqlException).GetConstructor(
			BindingFlags.Instance | BindingFlags.NonPublic, null,
			[typeof(MySqlConnector.MySqlErrorCode), typeof(string), typeof(string)], null)!
			.Invoke([(MySqlConnector.MySqlErrorCode)1213, "40001", "connector deadlock"]);
		var legacy = (Exception)typeof(MySql.Data.MySqlClient.MySqlException).GetConstructor(
			BindingFlags.Instance | BindingFlags.NonPublic, null,
			[typeof(string), typeof(int)], null)!
			.Invoke(["legacy deadlock", 1213]);
		// This provider's SQLSTATE constructor does not populate Number; exercise that independently.
		var legacyState = (Exception)typeof(MySql.Data.MySqlClient.MySqlException).GetConstructor(
			BindingFlags.Instance | BindingFlags.NonPublic, null,
			[typeof(uint), typeof(string), typeof(string)], null)!
			.Invoke([1213U, "40001", "legacy deadlock"]);
		var report = ExceptionReporter.Format(new AggregateException(connector, legacy, legacyState), "database", true);
		StringAssert.Contains(report, "MySQL number=1213; code=LockDeadlock; SQLSTATE=40001; transient=True");
		StringAssert.Contains(report, "MySQL number=1213; SQLSTATE=");
		StringAssert.Contains(report, "MySQL number=0; SQLSTATE=40001");
	}

	[TestMethod]
	public async Task BackgroundNotifications_QueuedByWorker_AreDeliveredOnlyByDrainOwner()
	{
		var queue = new ExceptionNotificationQueue();
		var payloads = new System.Collections.Generic.List<string>();
		await Task.Run(() => { queue.Enqueue("first"); queue.Enqueue("second"); });
		Assert.AreEqual(0, payloads.Count);
		var ownerThread = Environment.CurrentManagedThreadId;
		using var fallback = new StringWriter();
		queue.Drain(report =>
		{
			Assert.AreEqual(ownerThread, Environment.CurrentManagedThreadId);
			payloads.Add(report);
			if (report == "first") throw new IOException("disconnected");
		}, fallback);
		queue.Drain(payloads.Add, fallback);
		CollectionAssert.AreEqual(new[] { "first", "second" }, payloads);
		StringAssert.Contains(fallback.ToString(), "notification unavailable");
	}

	[TestMethod]
	public void LoadEffect_UnknownType_RetainsLoaderCauseAndOwnerIdentifiers()
	{
		var owner = new Mock<IPerceivable>();
		owner.SetupGet(x => x.Id).Returns(123);
		owner.SetupGet(x => x.FrameworkItemType).Returns("Character");
		var error = Assert.ThrowsExactly<ApplicationException>(() =>
			Effect.LoadEffect(XElement.Parse("<Effect><Type>ReporterTestMissingEffect</Type></Effect>"), owner.Object));

		Assert.IsInstanceOfType(error.InnerException, typeof(System.Collections.Generic.KeyNotFoundException));
		var report = ExceptionReporter.Format(error, "load", true);
		StringAssert.Contains(report, "Data.EntityId: 123");
		StringAssert.Contains(report, "Data.EntityType: Character");
		StringAssert.Contains(report, "LoadEffect: ReporterTestMissingEffect");
	}

	[TestMethod]
	public void EffectScheduler_CallbackFailure_PreservesExceptionAndOwnerWithoutRescheduling()
	{
		var owner = new Mock<IPerceivable>();
		owner.SetupGet(x => x.Id).Returns(456);
		owner.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		var effect = new Mock<IEffect>();
		effect.SetupGet(x => x.Owner).Returns(owner.Object);
		var failure = new InvalidOperationException("callback failed");
		effect.Setup(x => x.ExpireEffect()).Throws(failure);
		var scheduler = new EffectScheduler(new Mock<IFuturemud>().Object);
		scheduler.AddSchedule(new EffectSchedule(effect.Object, TimeSpan.Zero));

		Assert.AreSame(failure, Assert.ThrowsExactly<InvalidOperationException>(scheduler.CheckSchedules));
		Assert.AreEqual(0, scheduler.ScheduleCount);
		Assert.IsFalse(scheduler.IsScheduled(effect.Object));
		var report = ExceptionReporter.Format(failure, "schedule", true);
		StringAssert.Contains(report, "EffectSchedule.Fire:");
		StringAssert.Contains(report, "Data.EntityId: 456");
	}

	[TestMethod]
	public void Scheduler_CallbackFailure_PreservesExceptionAndAddsOperation()
	{
		var failure = new InvalidOperationException("callback failed");
		var scheduler = new Scheduler();
		scheduler.AddSchedule(new Schedule(() => throw failure, ScheduleType.System, TimeSpan.Zero, "fixture"));
		Assert.AreSame(failure, Assert.ThrowsExactly<InvalidOperationException>(scheduler.CheckSchedules));
		Assert.AreEqual(0, scheduler.ScheduleCount);
		StringAssert.Contains(ExceptionReporter.Format(failure, "schedule", true), "Schedule.Fire:");
	}

	[TestMethod]
	public void Annotate_FailingContext_KeepsExistingIdentifiersAndOtherFields()
	{
		var error = new Exception();
		error.Data["EntityId"] = 42L;
		ExceptionReporter.Annotate(error, ("EntityId", () => 99L),
			("EntityType", () => throw new InvalidOperationException()), ("Operation", () => "load"));
		Assert.AreEqual(42L, error.Data["EntityId"]);
		Assert.AreEqual("load", error.Data["Operation"]);
	}

	[TestMethod]
	public void Format_NullReflectionTypesAndNestedAggregate_PreservesEveryFailure()
	{
		var reflection = new ReflectionTypeLoadException([null, typeof(string)],
			[new FileNotFoundException("missing dependency", "dependency.dll")]);
		var error = new InvalidOperationException("wrapper", new AggregateException(reflection,
			new HttpRequestException(HttpRequestError.ConnectionError, "http failed", null, HttpStatusCode.BadGateway)));

		var report = ExceptionReporter.Format(error, "test", true);

		StringAssert.Contains(report, "missing dependency");
		StringAssert.Contains(report, "failed type slots=1");
		StringAssert.Contains(report, "root/inner/aggregate[0]/loader[0]");
		StringAssert.Contains(report, "File=dependency.dll");
		StringAssert.Contains(report, "HTTP status=502; category=ConnectionError");
	}

	[TestMethod]
	public void Format_BrokenContextAndExceptionText_RetainsOriginalAndOtherContext()
	{
		var report = ExceptionReporter.Format(new BrokenTextException(), "player input", false,
			("Broken", () => throw new InvalidOperationException()), ("Room ID", () => "42"));

		StringAssert.Contains(report, "original failure");
		StringAssert.Contains(report, "Broken: unavailable");
		StringAssert.Contains(report, "Room ID: 42");
		StringAssert.Contains(report, "Engine version:");
		StringAssert.Contains(report, "Build identity:");
		StringAssert.Contains(report, "Runtime:");
		StringAssert.Contains(report, "UTC:");
		StringAssert.Contains(report, "Process terminating: False");
#if DEBUG
		StringAssert.Contains(report, "Configuration: Debug");
#else
		StringAssert.Contains(report, "Configuration: Release");
#endif
	}

	[TestMethod]
	public void Format_WrappedConcurrencyException_RecordsKeysAndPropertyNamesWithoutValues()
	{
		using var context = new ReportContext();
		var row = new ReportRow { Id = 42, Secret = "original-secret" };
		context.Attach(row);
		row.Secret = "replacement-secret";
		context.ChangeTracker.DetectChanges();
		// Updating a tracked row absent from the isolated store produces a real concurrency exception.
		var error = Assert.ThrowsExactly<DbUpdateConcurrencyException>(() => context.SaveChanges());

		var report = ExceptionReporter.Format(new Exception("wrapper", error), "save", true);

		StringAssert.Contains(report, "Id=42");
		StringAssert.Contains(report, "state=Modified");
		StringAssert.Contains(report, "table=ReportRows");
		StringAssert.Contains(report, "Modified properties: Secret");
		Assert.IsFalse(report.Contains("original-secret"));
		Assert.IsFalse(report.Contains("replacement-secret"));
	}

	[TestMethod]
	public void Format_ParserAndSocketFailures_RecordsSpecificLocationsAndCodes()
	{
		var error = new AggregateException(new XmlException("bad XML", null, 12, 34),
			new System.Text.Json.JsonException("bad JSON", "$.item", 4, 9),
			new Newtonsoft.Json.JsonReaderException("bad legacy JSON", "items[0]", 7, 8, null),
			new SocketException((int)SocketError.ConnectionReset));

		var report = ExceptionReporter.Format(error, "test", false);

		StringAssert.Contains(report, "XML line=12; position=34");
		StringAssert.Contains(report, "JSON path=$.item; line=4; byte position=9");
		StringAssert.Contains(report, "JSON path=items[0]; line=7; position=8");
		StringAssert.Contains(report, "Socket error=ConnectionReset");
	}

	[TestMethod]
	public void Format_Data_UsesOnlyAllowlistedBoundedScalarValues()
	{
		var error = new Exception("test");
		error.Data["EntityId"] = 42L;
		error.Data["Operation"] = new string('x', 5000);
		error.Data["DefinitionId"] = new BrokenTextException();
		error.Data["Password"] = "do-not-export";
		var report = ExceptionReporter.Format(error, "test", false);

		StringAssert.Contains(report, "Data.EntityId: 42");
		StringAssert.Contains(report, "[truncated]");
		StringAssert.Contains(report, "[non-scalar value omitted]");
		Assert.IsFalse(report.Contains("do-not-export"));
		Assert.IsFalse(report.Contains(new string('x', 257)));
	}

	[TestMethod]
	public async Task Format_Operation_FlowsAcrossAwaitAndIsolatesWorkers()
	{
		var previous = ExceptionReporter.Operation;
		try
		{
			ExceptionReporter.Operation = "Main loop";
			await Task.Run(async () =>
			{
				ExceptionReporter.Operation = "Worker";
				await Task.Yield();
				StringAssert.Contains(ExceptionReporter.Format(new Exception(), "test", false), "Operation: Worker");
			});
			Assert.AreEqual("Main loop", ExceptionReporter.Operation);
		}
		finally { ExceptionReporter.Operation = previous; }
	}

	[TestMethod]
	public async Task Write_ConcurrentReports_PreservesAllFilesAndExactNotificationPayloads()
	{
		var directory = Directory.CreateTempSubdirectory("futuremud-exception-tests-");
		try
		{
			var reports = Enumerable.Range(0, 20).Select(index => $"Report {index}: unicode café").ToArray();
			await Task.WhenAll(reports.Select(report => Task.Run(() =>
			{
				var calls = 0;
				Assert.IsTrue(ExceptionReporter.Write(report, payload =>
				{
					Assert.AreEqual(report, payload);
					calls++;
				}, directory.FullName));
				Assert.AreEqual(1, calls);
			})));
			var files = directory.GetFiles("*.txt");
			Assert.AreEqual(reports.Length, files.Length);
			CollectionAssert.AreEquivalent(reports, files.Select(file => File.ReadAllText(file.FullName)).ToArray());
		}
		finally { directory.Delete(true); }
	}

	[TestMethod]
	public void Write_FileFailure_FallsBackAndStillNotifiesWithoutThrowing()
	{
		using var fallback = new StringWriter();
		string? notification = null;
		Assert.IsFalse(ExceptionReporter.Write("original report", text => notification = text,
			Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "absent"), fallback));
		Assert.AreEqual("original report", notification);
		StringAssert.Contains(fallback.ToString(), "original report");
		Assert.IsFalse(ExceptionReporter.Write("original report", _ => throw new IOException(), "\0", new BrokenWriter()));
	}

	[TestMethod]
	public void Write_NotificationFailure_LeavesLocalReportIntact()
	{
		var directory = Directory.CreateTempSubdirectory("futuremud-exception-tests-");
		try
		{
			using var fallback = new StringWriter();
			Assert.IsTrue(ExceptionReporter.Write("original report", _ => throw new IOException(), directory.FullName, fallback));
			Assert.AreEqual("original report", File.ReadAllText(directory.GetFiles().Single().FullName));
			StringAssert.Contains(fallback.ToString(), "notification unavailable");
		}
		finally { directory.Delete(true); }
	}

	private sealed class BrokenTextException : Exception
	{
		public BrokenTextException() : base("original failure") { }
		public override string ToString() => throw new InvalidOperationException("broken text");
	}

	private sealed class BrokenWriter : StringWriter
	{
		public override void WriteLine(string? value) => throw new IOException();
	}

	private sealed class ReportRow
	{
		public long Id { get; set; }
		public string Secret { get; set; } = string.Empty;
	}

	private sealed class ReportContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseInMemoryDatabase(Guid.NewGuid().ToString());
		protected override void OnModelCreating(ModelBuilder model) => model.Entity<ReportRow>().ToTable("ReportRows");
	}
}
