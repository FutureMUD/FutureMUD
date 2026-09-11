#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitProgBatchTests
{
	[TestMethod]
	public void BatchAllocatesAllIdsInTwoSavesAndPreservesEditedBodiesOnRerun()
	{
		var saves = new SaveCounter();
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).AddInterceptors(saves).Options);
		using var lookup = CultureToolkitManagedEntities.BeginLookupScope(context);
		var definitions = Enumerable.Range(0, 25).Select(i => new CultureProgDefinition($"test.{i}", $"Batch{i}", "return true",
			ProgVariableTypes.Boolean, [(ProgVariableTypes.Chargen, "ch")])).ToArray();
		var conflicts = new List<string>();
		var result = CultureToolkitProgSeeder.UpsertMany(context, "renaissance", definitions, conflicts);
		Assert.AreEqual(2, saves.Count);
		Assert.AreEqual(25, result.Values.Select(x => x.Id).Distinct().Count());
		Assert.AreEqual(25, context.FutureProgsParameters.Count());
		result["test.3"].FunctionText = "return false";
		context.SaveChanges();
		CultureToolkitProgSeeder.UpsertMany(context, "renaissance", definitions, conflicts);
		Assert.AreEqual(25, context.FutureProgs.Count());
		Assert.AreEqual("return false", result["test.3"].FunctionText);
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
	}

	private sealed class SaveCounter : SaveChangesInterceptor
	{
		public int Count { get; private set; }
		public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
		{
			Count++;
			return result;
		}
	}
}
