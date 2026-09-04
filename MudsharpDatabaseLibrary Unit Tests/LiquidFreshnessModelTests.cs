#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LiquidFreshnessModelTests
{
	[TestMethod]
	public void LiquidFreshness_HasNullableThresholdsAndRestrictiveSelfReferences()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;port=3306;database=dbo;uid=futuremud;password=unused", ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);
		var liquid = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(Liquid));

		Assert.IsNotNull(liquid);
		Assert.IsTrue(liquid.FindProperty(nameof(Liquid.StaleAfterSeconds))!.IsNullable);
		Assert.IsTrue(liquid.FindProperty(nameof(Liquid.SpoilAfterSeconds))!.IsNullable);
		var selfReferences = liquid.GetForeignKeys().Where(x => x.PrincipalEntityType == liquid &&
			x.Properties.Any(y => y.Name is nameof(Liquid.StaleLiquidId) or nameof(Liquid.SpoiledLiquidId))).ToArray();
		Assert.AreEqual(2, selfReferences.Length);
		Assert.IsTrue(selfReferences.All(x => x.DeleteBehavior == DeleteBehavior.Restrict));
	}
}
