#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FutureProgStorageTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void GeneratedProgramsCanUseMoreThanTheLegacyTextLimit()
	{
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;database=unused_model_only;uid=unused", ServerVersion.Parse("8.0.36-mysql")).Options);
		Assert.AreEqual("mediumtext", db.Model.FindEntityType(typeof(FutureProg))!.FindProperty(nameof(FutureProg.FunctionText))!.GetColumnType());
	}
}
