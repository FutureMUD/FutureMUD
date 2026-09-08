#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitAccentTests
{
	[TestMethod]
	public void NativeEligibilityExcludesForeignAccentsPreservesLearnerAndBuilderConditions()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var language = new Language { Name = "Fixture" };
		context.Add(language);
		context.SaveChanges();
		var local = new Accent { Name = "Regional", Group = "Native", LanguageId = language.Id };
		var foreign = new Accent { Name = "Foreign", Group = "foreign", LanguageId = language.Id };
		var custom = new Accent { Name = "Custom", Group = "foreign", LanguageId = language.Id, ChargenAvailabilityProgId = 999 };
		context.AddRange(local, foreign, custom);
		context.SaveChanges();
		language.DefaultLearnerAccentId = foreign.Id;
		foreach (var accent in new[] { local, foreign, custom })
			context.SeederManagedRecords.Add(new SeederManagedRecord
			{
				Seeder = "CultureSeeder", EntityType = "Accent", StableKey = $"fixture.{accent.Id}", Module = "medieval", LogicalId = accent.Id,
				ManifestVersion = "fixture", AppliedAt = DateTime.UtcNow,
				SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { [nameof(Accent.ChargenAvailabilityProgId)] = "null" })
			});
		context.SaveChanges();
		var bindings = new Dictionary<long, IReadOnlyList<long>> { [language.Id] = [42] };
		var conflicts = new List<string>();
		var first = CultureToolkitAccents.Upsert(context, "medieval", bindings, conflicts).Single();
		using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		var prog = compiler.Compile(first.EligibilityProgId);
		var ch = new Mock<IChargen>();
		var ethnicity = new Mock<IEthnicity>();
		ch.SetupGet(x => x.Type).Returns(ProgVariableTypes.Chargen);
		ch.SetupGet(x => x.GetObject).Returns(ch.Object);
		ch.Setup(x => x.GetProperty("ethnicity")).Returns(ethnicity.Object);
		ethnicity.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(42));
		Assert.IsFalse(prog.ExecuteBool(ch.Object));
		ethnicity.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(43));
		Assert.IsTrue(prog.ExecuteBool(ch.Object));
		Assert.IsNull(local.ChargenAvailabilityProgId);
		Assert.AreEqual(999L, custom.ChargenAvailabilityProgId);
		Assert.AreEqual(foreign.Id, language.DefaultLearnerAccentId);
		CultureToolkitAccents.Upsert(context, "medieval", bindings, conflicts);
		Assert.AreEqual(0, conflicts.Count);
		foreign.ChargenAvailabilityProgId = 998;
		context.SaveChanges();
		CultureToolkitAccents.Upsert(context, "medieval", bindings, conflicts);
		Assert.AreEqual(998L, foreign.ChargenAvailabilityProgId);
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
	}
}
