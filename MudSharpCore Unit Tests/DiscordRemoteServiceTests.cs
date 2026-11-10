#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Discord;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DiscordRemoteServiceTests
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
		Assert.AreEqual(44, DiscordRemotePolicy.Operations.Count);
		Assert.IsFalse(DiscordRemotePolicy.CanUse("unknown", null));
		Assert.IsFalse(DiscordRemotePolicy.CanUse("admin.ban", null));
		Assert.IsTrue(DiscordRemotePolicy.CanUse("game.info", null));
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
