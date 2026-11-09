#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederGasTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void EnsureStockGases_RerunPreservesIdsAndDoesNotDuplicateTags()
	{
		using FuturemudDatabaseContext context = BuildContext();

		CoreDataSeeder.EnsureStockGases(context);
		Dictionary<string, long> originalIds = context.Gases.ToDictionary(x => x.Name, x => x.Id);
		CoreDataSeeder.EnsureStockGases(context);

		Assert.AreEqual(CoreDataSeeder.StockGasNamesForTesting.Count, context.Gases.Count());
		foreach ((string gasName, long gasId) in originalIds)
		{
			Assert.AreEqual(gasId, context.Gases.Single(x => x.Name == gasName).Id,
				$"Expected rerun to preserve the ID for {gasName}.");
		}

		Assert.IsFalse(context.GasesTags
			.GroupBy(x => new { x.GasId, x.TagId })
			.Any(x => x.Count() > 1), "Expected rerun not to duplicate gas tag links.");
	}

	[TestMethod]
	public void EnsureStockGases_AllowsDuplicateNamesInUnrelatedTagHierarchies()
	{
		using FuturemudDatabaseContext context = BuildContext();
		var firstParent = new Tag { Id = 1, Name = "First Parent" };
		var secondParent = new Tag { Id = 2, Name = "Second Parent" };
		context.Tags.AddRange(
			firstParent,
			secondParent,
			new Tag { Id = 3, Name = "Apiary Product", Parent = firstParent, ParentId = firstParent.Id },
			new Tag { Id = 4, Name = "Apiary Product", Parent = secondParent, ParentId = secondParent.Id },
			new Tag { Id = 5, Name = "Gases", Parent = firstParent, ParentId = firstParent.Id });
		context.SaveChanges();

		CoreDataSeeder.EnsureStockGases(context);

		Assert.AreEqual(4, context.Tags.Count(x => x.Name == "Apiary Product"));
		Assert.AreEqual(firstParent.Id, context.Tags.Single(x => x.Id == 3).ParentId);
		Assert.AreEqual(secondParent.Id, context.Tags.Single(x => x.Id == 4).ParentId);
		Assert.AreEqual(firstParent.Id, context.Tags.Single(x => x.Id == 5).ParentId);
		Assert.AreEqual(1, context.Tags.Count(x => x.Name == "Gases" && x.Parent!.Name == "Materials"));
		Assert.AreEqual(1, context.Tags.Count(x => x.Name == "Breathable Atmospheres" && x.Parent!.Name == "Gases"));
	}
}
