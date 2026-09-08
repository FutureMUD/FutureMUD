#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitFreeSkillsTests
{
	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void EitherSeederOrderWiresStockAndPreservesIndependentGrantsOutsideItsBlock(bool cultureFirst)
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false))
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		context.Accounts.Add(new Account { Name = "Fixture" });
		context.Races.Add(new Race { Name = "Human", AllowedGenders = "Male Female" });
		context.FutureProgs.Add(new FutureProg { FunctionName = "AlwaysTrue", FunctionText = "return true", ReturnType = (long)ProgVariableTypes.Boolean });
		context.SaveChanges();
		var conflicts = new List<string>();
		void SeedHelper()
		{
			var helper = new FutureProg
			{
				FunctionName = "CultureFixedSkills", FunctionText = "var skills as trait collection\nreturn @skills",
				ReturnType = (long)(ProgVariableTypes.Trait | ProgVariableTypes.Collection)
			};
			helper.FutureProgsParameters.Add(new FutureProgsParameter { ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Toon });
			context.FutureProgs.Add(helper);
			context.SaveChanges();
			context.SeederManagedRecords.Add(new SeederManagedRecord
			{
				Seeder = "CultureSeeder", EntityType = "FutureProg", StableKey = "language.fixed-skills", Module = "medieval", LogicalId = helper.Id,
				ManifestVersion = "fixture", AppliedAt = DateTime.UtcNow
			});
			context.SaveChanges();
			CultureToolkitFreeSkills.Reconcile(context, conflicts);
		}
		var answers = new Dictionary<string, string>
		{
			["rpp"] = "no", ["bp"] = "no", ["class"] = "no", ["role-first"] = "race", ["attributemode"] = "order",
			["skillmode"] = "picker", ["merits"] = "merit", ["customdescs"] = "no"
		};
		if (cultureFirst) SeedHelper();
		new ChargenSeeder().SeedData(context, answers);
		if (!cultureFirst) SeedHelper();
		var target = context.FutureProgs.Single(x => x.FunctionName == "ChargenFreeSkills");
		Assert.AreEqual(0, conflicts.Count);
		StringAssert.Contains(target.FunctionText, CultureToolkitFreeSkills.Start);
		target.FunctionText = target.FunctionText.Replace("// Class-Based skills?", "// Builder independent grant\nadditem skills ToTrait(999)");
		context.SaveChanges();
		new ChargenSeeder().SeedData(context, answers);
		StringAssert.Contains(target.FunctionText, "additem skills ToTrait(999)");
		Assert.AreEqual(1, target.FunctionText.Split(CultureToolkitFreeSkills.Start).Length - 1);
		target.FunctionText = target.FunctionText.Replace("not(Contains(@skills, @cultureSkill))", "true");
		context.SaveChanges();
		var modified = target.FunctionText;
		CultureToolkitFreeSkills.Reconcile(context, conflicts);
		Assert.AreEqual(modified, target.FunctionText);
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
	}
}
