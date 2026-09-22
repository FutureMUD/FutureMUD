#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class GameItemDescriptionOverrideModelTests
{
	[DataTestMethod]
	[DataRow(nameof(GameItem.OverrideSdesc))]
	[DataRow(nameof(GameItem.OverrideDesc))]
	public void GameItemModel_DescriptionOverrides_AreNullableTextWithoutDefaults(string name)
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		var entity = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(GameItem))!;
		var property = entity.FindProperty(name)!;
		Assert.IsTrue(property.IsNullable);
		Assert.AreEqual("text", property.GetColumnType());
		Assert.IsNull(property.GetDefaultValue());
		Assert.IsNull(property.GetDefaultValueSql());
		Assert.AreEqual("utf8", property.GetCharSet());
		Assert.AreEqual("utf8_general_ci", property.GetCollation());
	}
}
