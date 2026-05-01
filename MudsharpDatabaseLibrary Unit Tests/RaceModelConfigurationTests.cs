#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RaceModelConfigurationTests
{
	private static readonly string[] AgeColumnNames =
	[
		nameof(Race.ChildAge),
		nameof(Race.YouthAge),
		nameof(Race.YoungAdultAge),
		nameof(Race.AdultAge),
		nameof(Race.ElderAge),
		nameof(Race.VenerableAge)
	];

	[TestMethod]
	public void RaceAgeColumns_DoNotUseDatabaseDefaults()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(
				"server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using FuturemudDatabaseContext context = new(options);

		var entityType = context.Model.FindEntityType(typeof(Race));
		Assert.IsNotNull(entityType);

		foreach (var columnName in AgeColumnNames)
		{
			var property = entityType.FindProperty(columnName);
			Assert.IsNotNull(property, $"{columnName} should be mapped on Race.");
			Assert.IsNull(property.FindAnnotation(RelationalAnnotationNames.DefaultValue),
				$"{columnName} should not have a CLR database default annotation.");
			Assert.IsNull(property.FindAnnotation(RelationalAnnotationNames.DefaultValueSql),
				$"{columnName} should not have a SQL database default annotation.");
		}
	}
}
