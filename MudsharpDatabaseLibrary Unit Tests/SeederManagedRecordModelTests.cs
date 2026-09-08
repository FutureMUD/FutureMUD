#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudsharpDatabaseLibrary_Unit_Tests;

[TestClass]
public class SeederManagedRecordModelTests
{
	[TestMethod]
	public void SeedBaselineAllowsUnbaselinedExistingRecordsAndUnboundedFieldSnapshots()
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		var property = context.Model.FindEntityType(typeof(SeederManagedRecord))!.FindProperty("SeedBaseline")!;
		Assert.IsTrue(property.IsNullable);
		Assert.AreEqual("longtext", property.GetColumnType());
		Assert.IsNull(new SeederManagedRecord().SeedBaseline);
	}
}
