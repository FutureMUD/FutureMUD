#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MySql.Data.MySqlClient;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MySqlConnectionStringsTests
{
	[TestMethod]
	[DataRow("SslMode=None", MySqlSslMode.Disabled)]
	[DataRow("ssl mode=none", MySqlSslMode.Disabled)]
	[DataRow("SslMode=None;ssl mode=None", MySqlSslMode.Disabled)]
	[DataRow("SSLMode=\" NONE \"", MySqlSslMode.Disabled)]
	[DataRow("SslMode=Disabled", MySqlSslMode.Disabled)]
	[DataRow("SslMode=Required", MySqlSslMode.Required)]
	[DataRow("SslMode=VerifyFull", MySqlSslMode.VerifyFull)]
	[DataRow("", MySqlSslMode.Preferred)]
	public void CreateBuilder_PreservesConnectionOptionsAndLegacyTlsMode(string sslOption, MySqlSslMode expected)
	{
		var builder = MySqlConnectionStrings.CreateBuilder(
			$"server=localhost;database=example;{sslOption};password=\"literal;SslMode=None\"");

		Assert.AreEqual(expected, builder.SslMode);
		Assert.AreEqual("localhost", builder.Server);
		Assert.AreEqual("example", builder.Database);
		Assert.AreEqual("literal;SslMode=None", builder.Password);
	}

	[TestMethod]
	[DataRow("database=first;initial catalog=second;database=final")]
	[DataRow("password=first;pwd=second;password=final")]
	[DataRow("server=first;host=second;server=final")]
	[DataRow("SslMode=Required;ssl mode=Disabled;SslMode=VerifyFull")]
	[DataRow("ssl mode=VerifyFull;SslMode=Disabled;ssl mode=Required")]
	[DataRow("database=first;initial catalog=second;database=")]
	public void CreateBuilder_PreservesTheDriversAliasPrecedence(string connectionString)
	{
		var expected = new MySqlConnectionStringBuilder(connectionString);
		var actual = MySqlConnectionStrings.CreateBuilder(connectionString);

		Assert.AreEqual(expected.ConnectionString, actual.ConnectionString);
		Assert.AreEqual(expected.Database, actual.Database);
		Assert.AreEqual(expected.Server, actual.Server);
		Assert.AreEqual(expected.Password, actual.Password);
		Assert.AreEqual(expected.SslMode, actual.SslMode);
	}

	[TestMethod]
	[DataRow("SslMode=Required;ssl mode=None;SslMode=VerifyFull", MySqlSslMode.VerifyFull)]
	[DataRow("ssl mode=None;SslMode=VerifyFull;ssl mode=None", MySqlSslMode.Disabled)]
	public void CreateBuilder_PreservesAliasPrecedenceWithTheLegacyTlsMode(string connectionString,
		MySqlSslMode expected)
	{
		Assert.AreEqual(expected, MySqlConnectionStrings.CreateBuilder(connectionString).SslMode);
	}

	[TestMethod]
	public void CreateBuilder_StillRejectsAnInvalidTlsMode()
	{
		Assert.ThrowsExactly<ArgumentException>(() =>
			MySqlConnectionStrings.CreateBuilder("server=localhost;SslMode=invalid"));
	}

	[TestMethod]
	public void DatabaseLooksBlank_AcceptsLegacyTlsModeWithoutADatabase()
	{
		Assert.IsTrue(new MySqlDatabaseBackupService().DatabaseLooksBlank("server=localhost;SslMode=None"));
	}
}
