#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ActiveProjectSpatialModelTests
{
	[TestMethod]
	public void ActiveProjectModel_PersistsLayerAndOptionalRouteCoordinate()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(
				"server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);

		var entityType = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(ActiveProject));
		Assert.IsNotNull(entityType);
		var layer = entityType.FindProperty(nameof(ActiveProject.RoomLayer));
		var position = entityType.FindProperty(nameof(ActiveProject.RoutePosition));
		Assert.IsNotNull(layer);
		Assert.IsNotNull(position);
		Assert.IsFalse(layer.IsNullable);
		Assert.IsTrue(position.IsNullable);
		Assert.AreEqual("int(11)", layer.GetColumnType());
		Assert.AreEqual("decimal(18,3)", position.GetColumnType());
	}
}
