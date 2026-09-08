#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using DbProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitStartingProgTests
{
	[DataTestMethod]
	[DataRow("antiquity")]
	[DataRow("darkages")]
	[DataRow("medieval")]
	[DataRow("renaissance")]
	[DataRow("earlymodern")]
	public void BackgroundAndNativeHelpersCompileAndKeepEditedBaseAndOriginalProg(string era)
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var original = new DbProg
		{
			FunctionName = "OriginalStartingValue", FunctionText = "return 25 + @boosts * 15", ReturnType = (long)ProgVariableTypes.Number
		};
		var parameters = new[] { (ProgVariableTypes.Toon, "ch"), (ProgVariableTypes.Trait, "trait"), (ProgVariableTypes.Number, "boosts") };
		for (var i = 0; i < parameters.Length; i++) original.FutureProgsParameters.Add(new FutureProgsParameter
		{
			ParameterIndex = i, ParameterName = parameters[i].Item2, ParameterType = (long)parameters[i].Item1
		});
		context.FutureProgs.Add(original);
		context.SaveChanges();
		var catalogue = new CultureToolkitCatalogue();
		var pack = catalogue.Compose(era);
		var cultures = pack.Cultures.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"),
			x => new Culture { Name = CultureToolkitCatalogue.Text(x, "label"), SkillStartingValueProgId = original.Id });
		var ethnicities = pack.Ethnicities.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"),
			x => new Ethnicity { Name = CultureToolkitCatalogue.Text(x, "label") });
		var nativeKeys = catalogue.Document("data.ethnicity_language_defaults.json").GetProperty("defaults").EnumerateArray()
			.Where(x => x.GetProperty("by_era").TryGetProperty(era, out _))
			.SelectMany(x => CultureToolkitCatalogue.Strings(x.GetProperty("by_era").GetProperty(era)));
		// Fixture IDs test the compiled consumer contract, not source binding or live catalogue import.
		var languages = pack.Languages.Select(x => CultureToolkitCatalogue.Text(x, "key")).Concat(nativeKeys).Distinct()
			.ToDictionary(x => x, x => new TraitDefinition { Name = x, Type = 0, OwnerScope = 1 });
		var literacy = new TraitDefinition { Name = "Literacy", Type = 0, OwnerScope = 1 };
		context.Cultures.AddRange(cultures.Values);
		context.Ethnicities.AddRange(ethnicities.Values);
		context.TraitDefinitions.AddRange(languages.Values.Append(literacy));
		context.SaveChanges();
		var conflicts = new List<string>();
		var first = CultureToolkitStartingProgs.Upsert(context, catalogue, pack, cultures, ethnicities, languages, literacy, conflicts);
		Assert.AreEqual(0, conflicts.Count);
		Assert.AreEqual(1, first.OriginalToWrapperProgIds.Count);
		Assert.IsTrue(cultures.Values.All(x => x.SkillStartingValueProgId == first.OriginalToWrapperProgIds[original.Id]));
		var native = context.FutureProgs.Find(first.NativeBaseProgId)!;
		Assert.AreEqual("return 200", native.FunctionText);
		native.FunctionText = "return 240";
		original.FunctionText = "return 35 + @boosts * 20";
		context.SaveChanges();
		var second = CultureToolkitStartingProgs.Upsert(context, catalogue, pack, cultures, ethnicities, languages, literacy, conflicts);
		Assert.AreEqual(first.NativeBaseProgId, second.NativeBaseProgId);
		Assert.AreEqual("return 240", native.FunctionText);
		Assert.AreEqual("return 35 + @boosts * 20", original.FunctionText);
		Assert.AreEqual(first.OriginalToWrapperProgIds[original.Id], second.OriginalToWrapperProgIds[original.Id]);
		Assert.IsTrue(conflicts.Any(x => x.Contains("body")));
	}
}
