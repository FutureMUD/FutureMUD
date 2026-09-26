#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudsharpDatabaseLibrary_Unit_Tests;

[TestClass]
public class CelestialDefinitionModelTests
{
	[TestMethod]
	public void Definition_AllowsDenseAuthoredSources_WithoutChangingExistingEncoding()
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		var property = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(Celestial))!
			.FindProperty(nameof(Celestial.Definition))!;
		Assert.AreEqual("longtext", property.GetColumnType());
		Assert.IsFalse(property.IsNullable);
		Assert.AreEqual("utf8", property.FindAnnotation("MySql:CharSet")!.Value);
		Assert.AreEqual("utf8_general_ci", property.GetCollation());
	}
}
