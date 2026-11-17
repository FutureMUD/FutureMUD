#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Accounts;
using MudSharp.Database;
using MudSharp.Framework;
using MySql.Data.MySqlClient;
using Moq;
using MudSharp.Menus;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Accounts;

[TestClass]
public class AccountSecurityNativeTests
{
	[TestMethod]
	[TestCategory("Integration")]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	public void AccountSecurityNative_AbandonedWorkCannotRunItsCallback(int abandon)
	{
		var world = new Mock<IFuturemud>();
		var controller = new Mock<IPlayerController>();
		var closing = false;
		controller.SetupGet(x => x.Closing).Returns(() => closing);
		controller.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		var menu = new QueueMenu(world.Object);
		menu.AssumeControl(controller.Object);
		using var started = new ManualResetEventSlim();
		using var release = new ManualResetEventSlim();
		var callerThread = Environment.CurrentManagedThreadId;
		var callbackThread = 0;
		var callback = false;
		Assert.IsTrue(AccountSecurityWorkQueue.TryQueue(menu, () =>
		{
			started.Set();
			if (!release.Wait(TimeSpan.FromSeconds(5))) throw new TimeoutException();
			Assert.AreNotEqual(callerThread, Environment.CurrentManagedThreadId);
			return 42;
		}, result => { Assert.AreEqual(42, result); callback = true; callbackThread = Environment.CurrentManagedThreadId; }));
		try
		{
			Assert.IsTrue(started.Wait(TimeSpan.FromSeconds(5)));
			if (abandon == 1) closing = true;
			if (abandon == 2) menu.LoseControl(controller.Object);
		}
		finally { release.Set(); }
		Assert.IsTrue(SpinWait.SpinUntil(() =>
		{
			AccountSecurityWorkQueue.ProcessCompleted(world.Object);
			return AccountSecurityWorkQueue.PendingCount(world.Object) == 0;
		}, TimeSpan.FromSeconds(5)));
		Assert.AreEqual(abandon == 0, callback);
		if (callback) Assert.AreEqual(callerThread, callbackThread);
	}

	private sealed class QueueMenu(IFuturemud world) : Menu(string.Empty, world)
	{
		public override int Timeout => 1000;
		public override void AssumeControl(IController controller) { Controller = controller; OutputHandler = controller.OutputHandler; }
		public override void SilentAssumeControl(IController controller) => AssumeControl(controller);
		public override bool ExecuteCommand(string command) => true;
		public override void LoseControl(IController controller) => OutputHandler = null!;
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void AccountSecurityNative_UpgradeAndConcurrentReset()
	{
		var connection = Environment.GetEnvironmentVariable("FUTUREMUD_ACCOUNT_NATIVE_CONNECTION");
		if (string.IsNullOrWhiteSpace(connection)) Assert.Inconclusive("Requires the owned account-security acceptance runner.");
		var settings = new MySqlConnectionStringBuilder(connection);
		Assert.AreEqual("127.0.0.1", settings.Server);
		Assert.AreEqual("root", settings.UserID);
		Assert.AreEqual(string.Empty, settings.Password);
		Assert.IsTrue(settings.Database is "account_upgrade" or "authored_celestial_smoke");
		using (var sql = new MySqlConnection(connection))
		{
			sql.Open();
			using var identity = new MySqlCommand("SELECT @@datadir", sql);
			var expected = Environment.GetEnvironmentVariable("FUTUREMUD_ACCOUNT_NATIVE_DATADIR");
			Assert.IsFalse(string.IsNullOrWhiteSpace(expected));
			Assert.AreEqual(Path.GetFullPath(expected!).TrimEnd('\\', '/'), Path.GetFullPath((string)identity.ExecuteScalar()!).TrimEnd('\\', '/'), true);
		}
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseMySql(connection, ServerVersion.AutoDetect(connection)).Options;
		using var context = new FuturemudDatabaseContext(options);
		if (settings.Database == "account_upgrade")
		{
			string oldHash;
			using (var sql = new MySqlConnection(connection))
			{
				sql.Open();
				using var read = new MySqlCommand("SELECT Password FROM Accounts WHERE Name='admin'", sql);
				oldHash = SecurityUtilities.GetPasswordHash("old native credential", 4242);
				using var replace = new MySqlCommand("UPDATE Accounts SET Password=@hash, Salt=4242, RecoveryCode=@recovery WHERE Name='admin'", sql);
				replace.Parameters.AddWithValue("@hash", oldHash);
				replace.Parameters.AddWithValue("@recovery", SecurityUtilities.GetPasswordHash("legacy code", 4242));
				replace.ExecuteNonQuery();
			}
			var migrations = context.Database.GetMigrations().ToList();
			Assert.IsTrue(migrations.Last().EndsWith("_HardenAccountCredentialRecovery", StringComparison.Ordinal));
			context.Database.Migrate();
			var row = context.Accounts.AsNoTracking().Single(x => x.Name == "admin");
			Assert.AreEqual(oldHash, row.Password);
			Assert.AreEqual(0L, row.CredentialVersion);
			Assert.IsNull(AccountSecurityState.CheckRecovery(row, "legacy code", DateTime.UtcNow));
			context.GetService<IMigrator>().Migrate(migrations[^2]);
			context.GetService<IMigrator>().Migrate(migrations[^1]);
			Assert.AreEqual(oldHash, context.Accounts.AsNoTracking().Single(x => x.Name == "admin").Password);
			Assert.IsFalse(context.Database.HasPendingModelChanges());
			return;
		}
		context.Database.Migrate();
		Assert.IsFalse(context.Accounts.Any(x => x.Name == "f05owner"));
		var template = context.Accounts.First(x => x.Name == "admin");
		var owner = (MudSharp.Models.Account)context.Entry(template).CurrentValues.ToObject();
		owner.Id = 0;
		owner.Name = "f05owner";
		owner.Email = string.Empty;
		owner.IsRegistered = false;
		owner.RegistrationCode = "F05manual";
		owner.Salt = 535;
		owner.Password = SecurityUtilities.GetPasswordHash("old", owner.Salt);
		owner.AuthorityGroupId = context.AuthorityGroups.First(x => x.AuthorityLevel == 2).Id;
		context.Accounts.Add(owner);
		context.SaveChanges();
        var development = (MudSharp.Models.Account)context.Entry(owner).CurrentValues.ToObject();
        development.Id = 0;
        development.Name = "f05development";
        context.Accounts.Add(development);
        context.SaveChanges();
		var race = (MudSharp.Models.Account)context.Entry(owner).CurrentValues.ToObject();
		race.Id = 0;
		race.Name = "f05race";
		context.Accounts.Add(race);
		context.SaveChanges();
		using (var first = new FuturemudDatabaseContext(options))
		using (var second = new FuturemudDatabaseContext(options))
		{
			var left = first.Accounts.Single(x => x.Id == race.Id);
			var right = second.Accounts.Single(x => x.Id == race.Id);
			var now = DateTime.UtcNow;
			var permission = new AccountPasswordPermission(race.Id, 0, now.AddMinutes(5));
			Assert.IsTrue(AccountSecurityState.ReplacePassword(left, permission, Argon2AccountCredentialHasher.DummyHash, now));
			Assert.IsTrue(AccountSecurityState.ReplacePassword(right, permission, Argon2AccountCredentialHasher.DummyHash, now));
			first.SaveChanges();
			Assert.Throws<DbUpdateConcurrencyException>(() => second.SaveChanges());
		}
		context.Entry(race).State = EntityState.Detached;
		context.Accounts.Where(x => x.Id == race.Id).ExecuteDelete();
		var hasher = new Argon2AccountCredentialHasher();
		var hash = hasher.Hash("synthetic cryptography acceptance");
		Assert.IsTrue(hasher.Verify("synthetic cryptography acceptance", hash));
		Assert.IsFalse(hasher.Verify("incorrect", hash));
	}
}
