#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Computers;
using MudSharp.Framework.Scheduling;

namespace MudSharp_Unit_Tests;

public partial class ComputerWorkspaceRuntimeTests
{
	[TestMethod]
	public void ComputerFileTransferService_UniqueUnknownUserNames_DoNotRetainFailureState()
	{
		var (service, host) = BackoffFixture();
		for (var i = 0; i < 10000; i++)
		{
			var result = service.Authenticate(host, host, $"unknown{i:D5}{new string('x', 40)}", "incorrect");
			Assert.IsFalse(result.Success);
		}

		Assert.AreEqual(0, AuthenticationBackoffs(service).Count);
	}

	[TestMethod]
	public void ComputerFileTransferService_UserNameLength_RejectsOversizedNamesBeforeRetention()
	{
		var (service, host) = BackoffFixture();
		Assert.IsTrue(service.CreateAccount(host, new string('a', 64), "fixture", out var error), error);
		Assert.IsTrue(service.Authenticate(host, host, new string('a', 64), "fixture").Success);
		Assert.IsFalse(service.CreateAccount(host, new string('a', 65), "fixture", out error));
		StringAssert.Contains(error, "64 characters");
		var result = service.Authenticate(host, host, new string('x', 100000), "incorrect");
		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.ErrorMessage, "64 characters");
		Assert.AreEqual(0, AuthenticationBackoffs(service).Count);
	}

	[TestMethod]
	public void ComputerFileTransferService_ExistingAccount_PreservesBackoffAndSuccessCleanup()
	{
		var (service, host) = BackoffFixture();
		Assert.IsTrue(service.CreateAccount(host, "alice", "fixture", out _));
		for (var i = 0; i < 5; i++)
		{
			var result = service.Authenticate(host, host, "alice", "incorrect");
			StringAssert.Contains(result.ErrorMessage, "not correct");
		}
		StringAssert.Contains(service.Authenticate(host, host, "alice", "fixture").ErrorMessage, "temporarily delayed");
		var cache = AuthenticationBackoffs(service);
		var key = $"{host.FileOwnerId}:alice";
		cache[key] = (0, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow);

		Assert.IsTrue(service.Authenticate(host, host, " ALICE ", "fixture").Success);
		Assert.AreEqual(0, cache.Count);
	}

	[TestMethod]
	public void ComputerFileTransferService_DisabledAccount_DoesNotRetainFailureState()
	{
		var (service, host) = BackoffFixture();
		Assert.IsTrue(service.CreateAccount(host, "disabled", "fixture", out _));
		Assert.IsTrue(service.SetAccountEnabled(host, "disabled", false, out _));
		Assert.IsFalse(service.Authenticate(host, host, "disabled", "incorrect").Success);
		Assert.AreEqual(0, AuthenticationBackoffs(service).Count);
	}

	[TestMethod]
	public void ComputerFileTransferService_ExpiredFailureEntries_ArePrunedOnAuthentication()
	{
		var (service, host) = BackoffFixture();
		var cache = AuthenticationBackoffs(service);
		cache["old:partial"] = (1, null, DateTime.UtcNow.AddMinutes(-6));
		cache["old:cooldown"] = (0, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow);
		cache["live:partial"] = (1, null, DateTime.UtcNow);

		Assert.IsFalse(service.Authenticate(host, host, "unknown", "incorrect").Success);

		Assert.AreEqual(1, cache.Count);
		Assert.IsTrue(cache.ContainsKey("live:partial"));
	}

	[TestMethod]
	public void ComputerFileTransferService_ManyExistingAccounts_EnforcesCapacityWithoutEvictingBackoffs()
	{
		var (service, host) = BackoffFixture();
		for (var i = 0; i < 1024; i++)
		{
			var name = $"account{i}";
			Assert.IsTrue(service.CreateAccount(host, name, "fixture", out _));
			Assert.IsFalse(service.Authenticate(host, host, name, "incorrect").Success);
		}
		Assert.IsTrue(service.CreateAccount(host, "extra", "fixture", out _));
		StringAssert.Contains(service.Authenticate(host, host, "extra", "fixture").ErrorMessage, "temporarily delayed");
		Assert.AreEqual(1024, AuthenticationBackoffs(service).Count);
		Assert.IsTrue(service.Authenticate(host, host, "account0", "fixture").Success);
		Assert.IsTrue(service.Authenticate(host, host, "extra", "fixture").Success);
		Assert.AreEqual(1023, AuthenticationBackoffs(service).Count);
	}

	private static (ComputerFileTransferService Service, StubComputerHost Host) BackoffFixture()
	{
		var world = CreateGameworld(new Mock<IScheduler>());
		var service = new ComputerFileTransferService(world.Object);
		var host = new StubComputerHost { Powered = true, Name = "FTP fixture", OwnerHostItemId = 970L };
		Assert.IsTrue(host.SetNetworkServiceEnabled("ftp", true, out _));
		return (service, host);
	}

	private static Dictionary<string, (int FailedAttempts, DateTime? NextAttemptUtc, DateTime LastFailureUtc)> AuthenticationBackoffs(ComputerFileTransferService service) =>
		(Dictionary<string, (int, DateTime?, DateTime)>)typeof(ComputerFileTransferService)
			.GetField("_authenticationBackoffs", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(service)!;
}
