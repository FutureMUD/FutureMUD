#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
namespace MudSharp_Unit_Tests;
[TestClass]
public class OptionalSettingProductTests
{
	[TestMethod]
	public void BaseInstaller_DoesNotLoadOptionalSettingContentOrOfferItsSkillPreset()
	{
		Assert.IsNull(typeof(CoreDataSeeder).Assembly.GetType("DatabaseSeeder.Seeders.ArmageddonSettingSeeder"));
		Assert.IsNull(typeof(CoreDataSeeder).Assembly.GetType("DatabaseSeeder.Seeders.ArmageddonWorldfileSeeder"));
		Assert.IsFalse(typeof(CoreDataSeeder).Assembly.GetManifestResourceNames().Any(x => x.Contains("Armageddon", StringComparison.OrdinalIgnoreCase)));
		Assert.IsFalse(SeederCatalogue.GetEnabledSeeders().Any(x => x.Name.StartsWith("Armageddon", StringComparison.Ordinal)));
		Assert.IsFalse(SkillPackageSeeder.Presets.Any(x => x.Complexity == "armageddon"));
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		var question = new SkillPackageSeeder().SeederQuestions.Single(x => x.Id == "skillpreset");
		var answer = question.Validator("Armageddon", db);
		Assert.IsFalse(answer.Success); StringAssert.Contains(answer.error, "standalone");
		var combat = new CombatSeeder().SeederQuestions.Single(x => x.Id == "skilloption").Validator("armageddon", db);
		Assert.IsFalse(combat.Success); StringAssert.Contains(combat.error, "standalone");
		Assert.IsFalse(db.ChangeTracker.HasChanges());
	}
}
