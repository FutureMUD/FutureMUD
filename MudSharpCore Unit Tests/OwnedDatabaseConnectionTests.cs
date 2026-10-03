#nullable enable

using System;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using MySqlConnector;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class OwnedDatabaseConnectionTests
{
	private const string Unowned = "Server=127.0.0.1;Port=1;Database=shared_default;User ID=unused;Connection Timeout=1";

	[TestMethod]
	public void FMDB_NewSession_ValidatorRejectsBeforeProviderAutodetectionOrNetwork()
	{
		var previous = FMDB.ConnectionValidator; var connection = FMDB.ConnectionString;
		var field = typeof(FMDB).GetField("_defaultSession", BindingFlags.NonPublic | BindingFlags.Static)!;
		var session = field.GetValue(null); field.SetValue(null, null);
		try
		{
			FMDB.ConnectionString = Unowned;
			FMDB.ConnectionValidator = (boundary, candidate) =>
			{
				Assert.AreEqual("new-session-before-autodetect", boundary);
				Assert.AreEqual(System.Data.ConnectionState.Closed, candidate.State);
				throw new InvalidOperationException("owned gate");
			};
			Assert.AreEqual("owned gate", Assert.ThrowsException<InvalidOperationException>(() => new FMDB()).Message);
			Assert.IsNull(FMDB.Context);
		}
		finally { field.SetValue(null, session); FMDB.ConnectionString = connection; FMDB.ConnectionValidator = previous; }
	}

	[TestMethod]
	public void FMDB_ReusedSession_ValidatesActualConnectionBeforeIncrementingOrUsingContext()
	{
		var previous = FMDB.ConnectionValidator;
		var field = typeof(FMDB).GetField("_defaultSession", BindingFlags.NonPublic | BindingFlags.Static)!;
		var saved = field.GetValue(null);
		using var connection = new MySqlConnection(Unowned);
		using var context = Context();
		var type = typeof(FMDB).GetNestedType("DatabaseSession", BindingFlags.NonPublic)!;
		var session = Activator.CreateInstance(type, true)!;
		type.GetProperty("Connection")!.SetValue(session, connection);
		type.GetProperty("Context")!.SetValue(session, context);
		field.SetValue(null, session);
		try
		{
			FMDB.ConnectionValidator = (boundary, candidate) =>
			{
				Assert.AreEqual("reuse-explicit", boundary); Assert.AreSame(connection, candidate);
				throw new InvalidOperationException("stale session gate");
			};
			Assert.AreEqual("stale session gate", Assert.ThrowsException<InvalidOperationException>(() => new FMDB()).Message);
			Assert.AreEqual(0u, type.GetProperty("InstanceCount")!.GetValue(session));
		}
		finally { field.SetValue(null, saved); FMDB.ConnectionValidator = previous; }
	}

	private static FuturemudDatabaseContext Context(Action<string, DbConnection>? validate = null)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseMySql(Unowned, new MySqlServerVersion(new Version(8, 0, 45)));
		if (validate is not null) options.AddInterceptors(new FMDB.ValidatedConnectionInterceptor(validate));
		return new(options.Options);
	}

	[TestMethod]
	public async Task EF_SyncAndAsyncOpening_RejectsBeforeNetworkConnection()
	{
		using var context = Context((boundary, candidate) =>
		{
			Assert.IsTrue(boundary is "ef-opening" or "ef-opening-async");
			Assert.AreEqual(System.Data.ConnectionState.Closed, candidate.State);
			throw new InvalidOperationException(boundary);
		});
		Assert.AreEqual("ef-opening", Assert.ThrowsException<InvalidOperationException>(() => context.Database.OpenConnection()).Message);
		Assert.AreEqual("ef-opening-async", (await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => context.Database.OpenConnectionAsync())).Message);
	}
}
