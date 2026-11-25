#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Construction;
using MudSharp.Discord;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.AIStorytellers;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class DiscordRemoteServiceTests
{
	[TestMethod]
	public void BridgeWrite_ClosedTransport_DoesNotEscapeIntoGameplay()
	{
		Assert.IsFalse(DiscordConnection.TryWriteBridgeFrame(new FailingWriteStream(), [1, 2, 3]));
		using var closed = new MemoryStream();
		closed.Dispose();
		Assert.IsFalse(DiscordConnection.TryWriteBridgeFrame(closed, [1]));
	}

	[TestMethod]
	public void Process_RetainedOldInvocationIdentity_DoesNotReplayAfterRetentionWindow()
	{
		var f = new Fixture();
		var request = f.Request("hooks.run");
		var confirmation = f.Service.Process(request);
		Assert.AreEqual("completed", f.Service.Process(request with { ConfirmationId = confirmation.ConfirmationId }).Status);
		f.Now = f.Now.AddDays(31);
		f.Store.Invocations[request.RequestId.ToString("D")].Arguments = "";
		f.Store.Invocations[request.RequestId.ToString("D")].ResultText = "Detailed output expired.";
		var restarted = new DiscordRemoteService(f.World.Object, f.Store, () => f.Now, _ => f.Account.Object);
		Assert.AreEqual("completed", restarted.Process(request).Status);
		f.Prog.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Once);
	}

	[TestMethod]
	public void Process_UnlinkedCaller_CannotDiscoverHooks()
	{
		var f = new Fixture();
		f.Store.Link = null;
		Assert.AreEqual("denied", f.Service.Process(f.Request()).Status);
	}

	[TestMethod]
	public void Process_DemotedBetweenPreviewAndConfirmation_DoesNotExecute()
	{
		var f = new Fixture();
		var request = f.Request("hooks.run");
		var preview = f.Service.Process(request);
		Assert.AreEqual("confirmation", preview.Status);
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.Player);
		Assert.AreEqual("denied", f.Service.Process(request with { ConfirmationId = preview.ConfirmationId }).Status);
		f.Prog.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void Process_ConfirmedMutationDuplicate_ExecutesOnlyOnce()
	{
		var f = new Fixture();
		var request = f.Request("hooks.run");
		var preview = f.Service.Process(request);
		var confirmed = request with { ConfirmationId = preview.ConfirmationId };
		Assert.AreEqual("completed", f.Service.Process(confirmed).Status);
		Assert.AreEqual("completed", f.Service.Process(confirmed).Status);
		f.Prog.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Once);
	}

	[TestMethod]
	public void Process_ProgEditedAfterPreview_InvalidatesConfirmation()
	{
		var f = new Fixture();
		var request = f.Request("hooks.run");
		var preview = f.Service.Process(request);
		f.Prog.SetupGet(x => x.FunctionText).Returns("new source");
		Assert.AreEqual("denied", f.Service.Process(request with { ConfirmationId = preview.ConfirmationId }).Status);
		f.Prog.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void Process_ExpiredConfirmation_DoesNotExecute()
	{
		var f = new Fixture();
		var request = f.Request("hooks.run");
		var preview = f.Service.Process(request);
		f.Now = f.Now.AddMinutes(3);
		Assert.AreEqual("expired", f.Service.Process(request with { ConfirmationId = preview.ConfirmationId }).Status);
		f.Prog.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void Process_RestartAfterInvoking_RemainsIndeterminateWithoutReplay()
	{
		var f = new Fixture();
		var request = f.Request("hooks.run");
		var preview = f.Service.Process(request);
		f.Store.Invocations[request.RequestId.ToString("D")].Status = "invoking";
		var restarted = new DiscordRemoteService(f.World.Object, f.Store, () => f.Now, _ => f.Account.Object);
		Assert.AreEqual("indeterminate", restarted.Process(request with { ConfirmationId = preview.ConfirmationId }).Status);
		f.Prog.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void Process_ReusedIdWithDifferentArguments_IsDenied()
	{
		var f = new Fixture();
		var request = f.Request();
		_ = f.Service.Process(request);
		Assert.AreEqual("denied", f.Service.Process(request with { Arguments = "{\"category\":\"other\"}" }).Status);
	}

	[TestMethod]
	public void Process_SeniorMaintainerCannotBypassFounderExecutionPolicy()
	{
		var f = new Fixture();
		f.Store.Hook.MinimumAuthority = (int)PermissionLevel.Founder;
		Assert.AreEqual("denied", f.Service.Process(f.Request("hooks.run")).Status);
	}

	[TestMethod]
	public void Process_SuspendedAccountCannotUseSelfService()
	{
		var f = new Fixture();
		f.Account.SetupGet(x => x.AccountStatus).Returns(AccountStatus.Suspended);
		Assert.AreEqual("denied", f.Service.Process(f.Request("account.resources")).Status);
	}

	[TestMethod]
	public void Policy_AllPlannedOperationsHaveExplicitAuthority()
	{
		Assert.AreEqual(54, DiscordRemotePolicy.Operations.Count);
		Assert.IsFalse(DiscordRemotePolicy.CanUse("unknown", null));
		Assert.IsFalse(DiscordRemotePolicy.CanUse("admin.ban", null));
		Assert.IsTrue(DiscordRemotePolicy.CanUse("game.info", null));
	}

	[TestMethod]
	public void Landmarks_ListAndDetailUseTheSameCharacterFreeVisibility()
	{
		var f = new Fixture();
		var rooms = new All<IRoom>();
		f.World.SetupGet(x => x.Rooms).Returns(rooms);
		var filter = new Mock<IFutureProg>();
		var open = AddLandmark(1, "Harbour", null);
		var conditional = AddLandmark(2, "Beacon", filter.Object);
		AddLandmark(3, "Hidden court", new Mock<IFutureProg>().Object);
		object allowed = true;
		filter.Setup(x => x.ExecuteWithStatus(out allowed, It.Is<object[]>(a =>
			a.Length == 2 && a[0] == conditional && a[1] == null))).Returns(true);
		open.EffectsOfType<LandmarkEffect>().Single().LandmarkDescriptionTexts.Add((90, "Private lore"));
		open.EffectsOfType<LandmarkEffect>().Single().LandmarkDescriptionTexts.Add((f.Prog.Object.Id, "Public lore"));
		f.Prog.Setup(x => x.ExecuteWithStatus(out allowed, It.Is<object[]>(a => a.Length == 1 && a[0] == null))).Returns(true);

		var list = f.Request("game.landmarks");
		var detail = f.Request("game.landmark") with { Arguments = "{\"id\":3}" };
		StringAssert.Contains(f.Service.Process(list).Text, "Hidden court");
		StringAssert.Contains(f.Service.Process(detail).Text, "Hidden court");
		StringAssert.Contains(f.Service.Process(f.Request("game.landmark") with { Arguments = """{"id":1}""" }).Text, "Private lore");
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.Player);
		Assert.AreEqual("denied", f.Service.Process(detail).Status);
		var publicList = f.Service.Process(f.Request("game.landmarks")).Text;
		StringAssert.Contains(publicList, "Harbour");
		StringAssert.Contains(publicList, "Beacon");
		Assert.IsFalse(publicList.Contains("Hidden court"));
		var hidden = f.Service.Process(detail with { RequestId = Guid.NewGuid() }).Text;
		Assert.AreEqual(hidden, f.Service.Process(detail with { RequestId = Guid.NewGuid(), Arguments = "{\"id\":999}" }).Text);
		var publicDetail = f.Service.Process(detail with { RequestId = Guid.NewGuid(), Arguments = "{\"id\":1}" }).Text;
		Assert.IsFalse(publicDetail.Contains("Private lore"));
		StringAssert.Contains(publicDetail, "Public lore");

		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.Guide);
		Assert.AreEqual(publicList, f.Service.Process(f.Request("game.landmarks")).Text);
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.JuniorAdmin);
		StringAssert.Contains(f.Service.Process(f.Request("game.landmarks")).Text, "Hidden court");
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.SeniorAdmin);
		f.Account.SetupGet(x => x.AccountStatus).Returns(AccountStatus.Suspended);
		Assert.IsFalse(f.Service.Process(f.Request("game.landmarks")).Text.Contains("Hidden court"));
		f.Store.Link = null;
		Assert.AreEqual(publicList, f.Service.Process(f.Request("game.landmarks")).Text);

		for (var id = 4; id <= 22; id++) { AddLandmark(id, new string('L', 1000), null); }
		var longPage = f.Service.Process(f.Request("game.landmarks")).Text;
		StringAssert.Contains(longPage, "/game landmarks after:21");
		Assert.IsFalse(longPage.Contains("[Result truncated]"));
		var nextPage = f.Service.Process(f.Request("game.landmarks") with { Arguments = """{"after":21}""" }).Text;
		StringAssert.Contains(nextPage, "#22");
		Assert.IsFalse(nextPage.Contains("#21"));

		IRoom AddLandmark(long id, string name, IFutureProg? prog)
		{
			var room = new Mock<IRoom> { DefaultValue = DefaultValue.Mock };
			room.SetupGet(x => x.Id).Returns(id);
			room.Setup(x => x.PublicLandmarkDescription(true)).Returns(name);
			room.Setup(x => x.PublicLandmarkDescription(false)).Returns("A room description.");
			var landmark = new LandmarkEffect(room.Object, false, "Example sphere", prog);
			room.Setup(x => x.EffectsOfType<LandmarkEffect>(null)).Returns([landmark]);
			rooms.Add(room.Object);
			return room.Object;
		}
	}

	[TestMethod]
	public void PublicRoomDetails_UsesOnlyUnqualifiedTraitAndWritingText()
	{
		Assert.AreEqual("A wall. Faded letters.", ("check{Lore,20}{Secret door}{A wall.} " +
			"writing{Common,Runic}{Password}{Faded letters.}").SubstitutePublicRoomDetails());
		Assert.AreEqual("A gesture.", "sign{ASL}{The password}{A gesture.}".SubstitutePublicRoomDetails());
		Assert.AreEqual("", "check{Lore,20}{Secret}writing{Common,Runic}{Password}sign{ASL}{Secret}".SubstitutePublicRoomDetails());
	}

	[TestMethod]
	public void StorytellerReports_PageByIdAndRequireCurrentSeniorAdminAuthority()
	{
		var f = new Fixture();
		var storytellers = new All<IAIStoryteller>();
		for (var id = 12; id > 0; id--)
		{
			var target = Mock.Of<IAIStoryteller>(x => x.Id == id && x.Name == $"Keeper {id}" &&
				x.GetOverview() == new StorytellerOverview(3, "attention", 7, 100, 20, 2));
			storytellers.Add(target);
		}
		f.World.SetupGet(x => x.AIStorytellers).Returns(storytellers);
		var overview = f.Request("storyteller.overview");
		var first = f.Service.Process(overview);
		Assert.AreEqual("completed", first.Status);
		StringAssert.Contains(first.Text, "3 queued, active: attention | 7 requests | 100 input / 20 output tokens | 2 failures");
		StringAssert.Contains(first.Text, "after:10");
		Assert.IsFalse(first.Text.Contains("#11 Keeper"));
		var next = f.Service.Process(f.Request("storyteller.overview") with { Arguments = """{"after":10}""" });
		StringAssert.Contains(next.Text, "#11 Keeper 11");
		Assert.IsFalse(next.Text.Contains("#10 Keeper"));
		var selected = storytellers.Get(12)!;
		Mock.Get(selected).Setup(x => x.DescribeUsage(It.IsAny<IFormatProvider>())).Returns("Recent failures: test");
		var detail = f.Service.Process(f.Request("storyteller.status") with { Arguments = """{"target":"keeper 12"}""" });
		StringAssert.Contains(detail.Text, "Recent failures: test");
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.Admin);
		foreach (var op in new[] { "overview", "status", "pause", "resume" })
		{
			Assert.AreEqual("denied", f.Service.Process(f.Request($"storyteller.{op}")).Status);
		}
	}

	[TestMethod]
	public void StorytellerControls_ConfirmOneExactTargetAndRecheckStateAndAuthority()
	{
		using var db = new InMemoryDatabaseTestScope();
		var model = AIStorytellerPauseTests.CreateModel(false);
		model.Id = 8;
		model.Name = "North Watch";
		db.Context.AIStorytellers.Add(model);
		db.Context.SaveChanges();
		var f = new Fixture();
		var paused = false;
		var target = new Mock<IAIStoryteller>();
		target.SetupGet(x => x.Id).Returns(8);
		target.SetupGet(x => x.Name).Returns("North Watch");
		target.SetupGet(x => x.IsPaused).Returns(() => paused);
		target.Setup(x => x.Pause()).Callback(() => paused = true);
		target.Setup(x => x.Unpause()).Callback(() => paused = false);
		var other = Mock.Of<IAIStoryteller>(x => x.Id == 9 && x.Name == "North Watch");
		f.World.SetupGet(x => x.AIStorytellers).Returns(new All<IAIStoryteller> { target.Object, other });
		DiscordRemoteRequest Request(string op) => f.Request($"storyteller.{op}") with { Arguments = """{"target":"8"}""" };
		foreach (var selector in new[] { "North", "North Watch", "999" })
		{
			var invalid = f.Service.Process(Request("pause") with { Arguments = System.Text.Json.JsonSerializer.Serialize(new { target = selector }) });
			Assert.IsNull(invalid.ConfirmationId);
			Assert.AreEqual("error", invalid.Status);
		}

		var pause = Request("pause");
		var preview = f.Service.Process(pause);
		Assert.AreEqual("confirmation", preview.Status);
		StringAssert.Contains(preview.Text, "North Watch (#8)");
		StringAssert.Contains(preview.Text, "discards");
		Assert.IsFalse(paused);
		var confirm = pause with { ConfirmationId = preview.ConfirmationId };
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.Admin);
		Assert.AreEqual("denied", f.Service.Process(confirm).Status);
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.SeniorAdmin);
		paused = true;
		Assert.AreEqual("denied", f.Service.Process(confirm).Status);
		target.Verify(x => x.Pause(), Times.Never);

		paused = false;
		pause = Request("pause");
		confirm = pause with { ConfirmationId = f.Service.Process(pause).ConfirmationId };
		Assert.AreEqual("completed", f.Service.Process(confirm).Status);
		Assert.IsTrue(paused);
		Assert.IsTrue(ReloadPaused());
		Assert.AreEqual("completed", f.Service.Process(confirm).Status);
		target.Verify(x => x.Pause(), Times.Once);
		var resume = Request("resume");
		Assert.AreEqual("completed", f.Service.Process(resume with { ConfirmationId = f.Service.Process(resume).ConfirmationId }).Status);
		target.Verify(x => x.Unpause(), Times.Once);
		Assert.IsFalse(paused);
		Assert.IsFalse(ReloadPaused());
		Mock.Get(other).Verify(x => x.Pause(), Times.Never);
		Mock.Get(other).Verify(x => x.Unpause(), Times.Never);

		bool ReloadPaused()
		{
			db.Context.ChangeTracker.Clear();
			var saved = db.Context.AIStorytellers.Single();
			return new MudSharp.RPG.AIStorytellers.AIStoryteller(saved, f.World.Object).IsPaused;
		}
	}

	[TestMethod]
	public void EconomyDigest_ReportsTransitionsOnceAcrossRestartsAndRearmsAfterRecovery()
	{
		var f = new Fixture();
		var analytics = new Mock<IEconomyAnalyticsService>();
		f.World.SetupGet(x => x.EconomyAnalytics).Returns(analytics.Object);
		f.World.SetupGet(x => x.EconomicZones).Returns(new All<IEconomicZone>());
		f.World.SetupGet(x => x.Currencies).Returns(new All<ICurrency>());
		var shortfall = new EconomyRisk("reserve-shortfall", "Half covered.", 50m, 8, 9);
		var overdue = new EconomyRisk("stale-snapshot", "Scheduled capture is late.");
		var risks = new List<EconomyRisk> { shortfall, overdue };
		analytics.Setup(x => x.GetDigestRisks()).Returns(risks);
		var messages = new List<string>();
		void Send(string user, string message)
		{
			Assert.AreEqual("42", user);
			messages.Add(message);
		}
		string Preference(string arguments) => f.Service.Process(f.Request("economy.digest") with { Arguments = arguments }).Text;

		StringAssert.Contains(Preference("{}"), "off");
		f.Service.ProcessEconomyDigest(() => true, Send);
		StringAssert.Contains(Preference("""{"enabled":true}"""), "on");
		f.Service.ProcessEconomyDigest(() => false, Send);
		analytics.Verify(x => x.GetDigestRisks(), Times.Never);
		f.Service.ProcessEconomyDigest(() => true, Send);
		StringAssert.Contains(messages.Single(), "Snapshot overdue");
		StringAssert.Contains(messages.Single(), "Reserve shortfall in Zone (#8), Currency (#9)");
		StringAssert.Contains(Preference("""{"enabled":true}"""), "on");
		risks[0] = shortfall with { Description = "Quarter covered." };
		var restarted = new DiscordRemoteService(f.World.Object, f.Store, () => f.Now, _ => f.Account.Object);
		restarted.ProcessEconomyDigest(() => true, Send);
		Assert.AreEqual(1, messages.Count);
		risks.Clear();
		restarted.ProcessEconomyDigest(() => true, Send);
		restarted.ProcessEconomyDigest(() => true, Send);
		Assert.AreEqual(2, messages.Count);
		StringAssert.Contains(messages[1], "Reserve shortfall cleared");
		Assert.IsFalse(messages[1].Contains("Snapshot"));
		risks.AddRange([shortfall, overdue]);
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.JuniorAdmin);
		Assert.AreEqual("denied", f.Service.Process(f.Request("economy.digest")).Status);
		restarted.ProcessEconomyDigest(() => true, Send);
		Assert.AreEqual(2, messages.Count);
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.Admin);
		restarted.ProcessEconomyDigest(() => true, Send);
		Assert.AreEqual(messages[0], messages[2]);
		// A successful capture and a second missed deadline can both happen while the bot is offline.
		risks[1] = overdue with { LastScheduledSnapshotId = 19 };
		restarted.ProcessEconomyDigest(() => true, Send);
		restarted.ProcessEconomyDigest(() => true, Send);
		StringAssert.Contains(messages[3], "Snapshot overdue");
		Assert.IsFalse(messages[3].Contains("Reserve shortfall"));
		StringAssert.Contains(Preference("""{"enabled":false}"""), "off");
		risks.Clear();
		restarted.ProcessEconomyDigest(() => true, Send);
		Assert.AreEqual(4, messages.Count);
	}

	private sealed class Fixture
	{
		public readonly Mock<IFuturemud> World = new();
		public readonly Mock<IAccount> Account = new();
		public readonly Mock<IAuthority> Authority = new();
		public readonly Mock<IFutureProg> Prog = new();
		public readonly MemoryStore Store = new();
		public DateTime Now = new(2026, 11, 10, 0, 0, 0, DateTimeKind.Utc);
		public DiscordRemoteService Service { get; }

		public Fixture()
		{
			Authority.SetupGet(x => x.Level).Returns(PermissionLevel.SeniorAdmin);
			Account.SetupGet(x => x.Authority).Returns(Authority.Object);
			Account.SetupGet(x => x.AccountStatus).Returns(AccountStatus.Normal);
			Account.SetupGet(x => x.Id).Returns(1);
			Account.SetupGet(x => x.Name).Returns("Staff");
			Prog.SetupGet(x => x.Id).Returns(1);
			Prog.SetupGet(x => x.FunctionText).Returns("original source");
			Prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Text);
			Prog.SetupGet(x => x.Parameters).Returns(
				[ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text]);
			object output = "done";
			Prog.Setup(x => x.ExecuteWithStatus(out output, It.IsAny<object[]>())).Returns(true);
			var progs = new All<IFutureProg>();
			progs.Add(Prog.Object);
			World.SetupGet(x => x.FutureProgs).Returns(progs);
			Service = new DiscordRemoteService(World.Object, Store, () => Now, _ => Account.Object);
		}

		public DiscordRemoteRequest Request(string operation = "hooks.list") => new()
		{
			DiscordUserId = 42, ChannelId = 7, GuildId = 9, Operation = operation,
			Arguments = operation == "hooks.run" ? "{\"hook\":\"test\",\"arguments\":\"hello\"}" : "{}"
		};
	}

	private sealed class FailingWriteStream : MemoryStream
	{
		public override void Write(byte[] buffer, int offset, int count) => throw new IOException("Transport closed.");
	}

	private sealed class MemoryStore : IDiscordRemoteStore
	{
		public DiscordAccountLink? Link = new() { DiscordUserId = "42", AccountId = 1 };
		public DiscordHook Hook = new() { Id = 1, Key = "test", ProgId = 1, Enabled = true };
		public readonly Dictionary<string, DiscordRemoteInvocation> Invocations = [];
		private readonly Dictionary<string, string> _settings = [];
		public DiscordAccountLink? FindLink(string userId) => Link?.DiscordUserId == userId ? Link : null;
		public void SaveLink(DiscordAccountLink link) => Link = link;
		public IReadOnlyList<DiscordAccountLink> EconomyDigestSubscribers() =>
			Link?.EconomyRiskDigestState is null ? [] : [new DiscordAccountLink
			{
				DiscordUserId = Link.DiscordUserId, AccountId = Link.AccountId,
				CreatedUtc = Link.CreatedUtc, EconomyRiskDigestState = Link.EconomyRiskDigestState
			}];
		public bool TryAdvanceEconomyDigest(DiscordAccountLink link, string state)
		{
			if (Link is null || Link.DiscordUserId != link.DiscordUserId || Link.AccountId != link.AccountId ||
			    Link.CreatedUtc != link.CreatedUtc || Link.EconomyRiskDigestState != link.EconomyRiskDigestState)
			{
				return false;
			}
			Link.EconomyRiskDigestState = state;
			return true;
		}
		public void RemoveLink(string userId) => Link = null;
		public IReadOnlyList<DiscordHook> Hooks() => [Hook];
		public void SaveHook(DiscordHook hook) => Hook = hook;
		public DiscordRemoteInvocation? FindInvocation(string id) => Invocations.GetValueOrDefault(id);
		public void SaveInvocation(DiscordRemoteInvocation invocation) => Invocations[invocation.Id] = invocation;
		public IReadOnlyList<DiscordRemoteInvocation> RecentInvocations() => Invocations.Values.ToList();
		public bool HookInvokedSince(long hookId, DateTime cutoff) => Invocations.Values.Any(x =>
			x.HookId == hookId && x.UpdatedUtc > cutoff && x.Status is "invoking" or "completed" or "failed" or "indeterminate");
		public string? GetSetting(string key) => _settings.GetValueOrDefault(key);
		public void SetSetting(string key, string value) => _settings[key] = value;
		public void Recover()
		{
			foreach (var invocation in Invocations.Values.Where(x => x.Status is "invoking" or "queued"))
			{
				invocation.Status = "indeterminate";
			}
		}
	}
}
