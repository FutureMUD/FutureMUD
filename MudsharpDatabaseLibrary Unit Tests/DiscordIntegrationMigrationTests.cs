#nullable enable

using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Migrations;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DiscordIntegrationMigrationTests
{
	[TestMethod]
	public void Migration_OnlyCreatesDiscordTables_PreservesExistingWorldSchema()
	{
		var operations = new DiscordRemoteOperations().UpOperations;
		var tables = operations.OfType<CreateTableOperation>().ToArray();
		CollectionAssert.AreEquivalent(new[]
		{
			"DiscordAccountLinks", "DiscordHooks", "DiscordIntegrationSettings", "DiscordRemoteInvocations"
		}, tables.Select(x => x.Name).ToArray());
		Assert.IsTrue(operations.All(x => x is CreateTableOperation or CreateIndexOperation));
		var links = tables.Single(x => x.Name == "DiscordAccountLinks");
		Assert.AreEqual(20, links.Columns.Single(x => x.Name == "DiscordUserId").MaxLength);
		var hooks = tables.Single(x => x.Name == "DiscordHooks");
		Assert.IsTrue(hooks.ForeignKeys.Any(x => x.PrincipalTable == "FutureProgs"));
		var history = tables.Single(x => x.Name == "DiscordRemoteInvocations");
		Assert.IsTrue(history.Columns.Any(x => x.Name == "ProgId"));
		Assert.IsTrue(history.Columns.Any(x => x.Name == "HookRevision"));
	}
}
