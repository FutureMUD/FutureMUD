#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Database;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AttributeDescriberSeederTests
{
	private static FuturemudDatabaseContext Seed(string decorator, string choice = "labmud")
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new FuturemudDatabaseContext(options);
		new AttributeSeeder().SeedData(context, new Dictionary<string, string>
		{
			["choice"] = choice, ["decorator"] = decorator
		});
		return context;
	}

	[TestMethod]
	[DataRow("soi")]
	[DataRow("dnd")]
	[DataRow("labmud")]
	[DataRow("split")]
	[DataRow("3stats")]
	[DataRow("rpi")]
	[DataRow("simple")]
	[DataRow("arm")]
	public void RpiDecorator_AssignsSelectedDecoratorToEveryAttributePackage(string choice)
	{
		using var context = Seed("rpi", choice);
		var row = context.TraitDecorators.Single();
		Assert.IsTrue(context.TraitDefinitions.All(x => x.DecoratorId == row.Id));
		Assert.IsFalse(string.IsNullOrWhiteSpace(new RangeDecorator(row).Decorate(285)));
	}

	[TestMethod]
	public void Rerun_PreservesCustomAndInstalledDescribers()
	{
		using var context = Seed("labmud");
		var strength = context.TraitDecorators.Single(x => x.Name == "Strength Attribute");
		strength.Contents = strength.Contents.Replace("Worldshaking", "Custom Strength");
		context.SaveChanges();
		var before = context.TraitDecorators.ToDictionary(x => x.Id, x => x.Contents);
		new AttributeSeeder().SeedData(context, new Dictionary<string, string>());
		Assert.AreEqual(before.Count, context.TraitDecorators.Count());
		foreach (var row in context.TraitDecorators)
		{
			Assert.AreEqual(before[row.Id], row.Contents);
		}
	}
}
