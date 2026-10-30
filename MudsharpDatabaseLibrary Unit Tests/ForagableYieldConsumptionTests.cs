#nullable enable

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ForagableYieldConsumptionTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void Foragable_ExplicitZeroIsNotTheStoreGeneratedDefaultSentinel()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;database=unused_model_only;uid=unused", ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);
		var property = context.Model.FindEntityType(typeof(Foragable))!
			.FindProperty(nameof(Foragable.YieldConsumptionPercentage))!;

		// EF must send an explicit zero on insert rather than treating it as unset
		// and silently receiving the database's compatibility default of 100.
		Assert.AreEqual(100.0, property.Sentinel);
		Assert.AreEqual(100.0, property.GetDefaultValue());
		Assert.AreEqual(100.0, new Foragable().YieldConsumptionPercentage);
		Assert.AreNotEqual(property.Sentinel, new Foragable { YieldConsumptionPercentage = 0.0 }.YieldConsumptionPercentage);
	}
}
