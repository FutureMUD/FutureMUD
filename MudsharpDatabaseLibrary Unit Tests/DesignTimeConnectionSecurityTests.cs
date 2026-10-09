#nullable enable

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using MySqlConnector;

namespace MudsharpDatabaseLibrary_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class DesignTimeConnectionSecurityTests
{
	[DataTestMethod]
	[DataRow(null)]
	[DataRow(" ")]
	public void CreateDbContext_NoDesignTimeConnection_UsesCredentialFreeModelConfiguration(string? configured)
	{
		WithConnection(configured, () =>
		{
			using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
			var connection = new MySqlConnectionStringBuilder(context.Database.GetConnectionString());
			Assert.AreEqual(string.Empty, connection.Password);
			Assert.AreEqual("futuremud_design_time", connection.UserID);
			Assert.AreEqual("futuremud_design_time", connection.Database);
			Assert.AreEqual(string.Empty, new MySqlConnectionStringBuilder(context.ConnectionString).Password);
			Assert.IsNotNull(context.Model.FindEntityType(typeof(Accent)));
		});
	}

	[TestMethod]
	public void CreateDbContext_ExplicitEnvironmentConnection_UsesConfiguredConnection()
	{
		const string testConnection = "server=localhost;port=1;database=test_fixture;uid=test_fixture;password=test-only-factory-value";
		WithConnection(testConnection, () =>
		{
			using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
			var connection = new MySqlConnectionStringBuilder(context.Database.GetConnectionString());
			Assert.AreEqual("test_fixture", connection.UserID);
			Assert.AreEqual("test_fixture", connection.Database);
			Assert.AreEqual((uint)1, connection.Port);
			Assert.AreEqual("test-only-factory-value", connection.Password);
			Assert.AreEqual(testConnection, context.ConnectionString);
		});
	}

	private static void WithConnection(string? connection, Action test)
	{
		const string variable = "FUTUREMUD_EF_CONNECTION_STRING";
		var original = Environment.GetEnvironmentVariable(variable);
		try
		{
			Environment.SetEnvironmentVariable(variable, connection);
			test();
		}
		finally
		{
			Environment.SetEnvironmentVariable(variable, original);
		}
	}
}
