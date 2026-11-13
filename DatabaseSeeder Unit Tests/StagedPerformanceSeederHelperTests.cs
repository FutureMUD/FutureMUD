#nullable enable

using System;
using System.Linq;
using DatabaseSeeder.Seeders.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Performances;
using FutureProg = MudSharp.Models.FutureProg;

namespace DatabaseSeederUnitTests;

[TestClass]
public class StagedPerformanceSeederHelperTests
{
	[TestMethod]
	public void EnsureWork_CreatesPublishedWorkAndReusesStableSeedKeyWithoutReplacingIt()
	{
		using var context = CreateContext();
		var definition = ValidWork("parish.high-mass", "Parish High Mass");

		Assert.AreEqual(StagedPerformanceSeedResult.Created,
			StagedPerformanceSeederHelper.EnsureWork(context, "parish.high-mass", definition));
		var row = context.StagedPerformanceRecords.Single();
		Assert.IsFalse(definition.Published);
		var installed = System.Text.Json.JsonSerializer.Deserialize<PerformanceWork>(row.Definition)!;
		Assert.AreEqual("work", row.Kind);
		Assert.AreEqual("parish.high-mass", installed.SeedKey);
		Assert.AreEqual(0L, installed.OwnerId);
		Assert.IsTrue(installed.Published);
		Assert.IsFalse(installed.StaffAuthored);

		installed.FailureText = "A builder's customization.";
		row.Definition = System.Text.Json.JsonSerializer.Serialize(installed);
		context.SaveChanges();

		var changedSource = ValidWork("parish.high-mass", "Parish High Mass");
		changedSource.FailureText = "A changed seeder default.";
		Assert.AreEqual(StagedPerformanceSeedResult.AlreadyInstalled,
			StagedPerformanceSeederHelper.EnsureWork(context, "parish.high-mass", changedSource));
		Assert.AreEqual("A builder's customization.",
			System.Text.Json.JsonSerializer.Deserialize<PerformanceWork>(row.Definition)!.FailureText);
		Assert.AreEqual(1, context.StagedPerformanceRecords.Count());
	}

	[TestMethod]
	public void EnsureWork_NameCollisionWithBuilderWork_ReportsAndPreservesExistingRecord()
	{
		using var context = CreateContext();
		var builderWork = ValidWork(null, "Parish High Mass");
		builderWork.OwnerId = 234;
		context.StagedPerformanceRecords.Add(new StagedPerformanceRecord
		{
			Kind = "work",
			Definition = System.Text.Json.JsonSerializer.Serialize(builderWork)
		});
		context.SaveChanges();
		var original = context.StagedPerformanceRecords.Single().Definition;

		var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
			StagedPerformanceSeederHelper.EnsureWork(context, "parish.high-mass", ValidWork("parish.high-mass", "Parish High Mass")));
		StringAssert.Contains(error.Message, "existing work without this seed key");
		Assert.AreEqual(original, context.StagedPerformanceRecords.Single().Definition);
		Assert.AreEqual(1, context.StagedPerformanceRecords.Count());
	}

	[TestMethod]
	public void EnsureWork_RejectsDuplicateSeedKeysAndSeedDefinitionChanges()
	{
		using var context = CreateContext();
		var seeded = ValidWork("parish.high-mass", "Parish High Mass");
		seeded.Published = true;
		context.StagedPerformanceRecords.AddRange(
			new StagedPerformanceRecord { Kind = "work", Definition = System.Text.Json.JsonSerializer.Serialize(seeded) },
			new StagedPerformanceRecord { Kind = "work", Definition = System.Text.Json.JsonSerializer.Serialize(seeded) });
		context.SaveChanges();
		var duplicate = Assert.ThrowsExactly<InvalidOperationException>(() =>
			StagedPerformanceSeederHelper.EnsureWork(context, "parish.high-mass", seeded));
		StringAssert.Contains(duplicate.Message, "More than one");

		using var single = CreateContext();
		single.StagedPerformanceRecords.Add(new StagedPerformanceRecord { Kind = "work", Definition = System.Text.Json.JsonSerializer.Serialize(seeded) });
		single.SaveChanges();
		var renamed = ValidWork("parish.high-mass", "A new display name");
		var changed = Assert.ThrowsExactly<InvalidOperationException>(() =>
			StagedPerformanceSeederHelper.EnsureWork(single, "parish.high-mass", renamed));
		StringAssert.Contains(changed.Message, "already assigned");
		Assert.AreEqual("Parish High Mass",
			System.Text.Json.JsonSerializer.Deserialize<PerformanceWork>(single.StagedPerformanceRecords.Single().Definition)!.Name);

		seeded.Published = false;
		single.StagedPerformanceRecords.Single().Definition = System.Text.Json.JsonSerializer.Serialize(seeded);
		single.SaveChanges();
		var unpublished = Assert.ThrowsExactly<InvalidOperationException>(() =>
			StagedPerformanceSeederHelper.EnsureWork(single, "parish.high-mass", seeded));
		StringAssert.Contains(unpublished.Message, "no longer published");
	}

	[TestMethod]
	public void EnsureWork_RequiresPublicCharacterEligibilityProg()
	{
		using var context = CreateContext();
		var privateProg = BooleanCharacterProg(publicProg: false);
		context.FutureProgs.Add(privateProg.Prog);
		context.FutureProgsParameters.Add(privateProg.Parameter);
		context.SaveChanges();
		var work = ValidWork("parish.high-mass", "Parish High Mass");
		work.Roles[0].EligibilityProgId = privateProg.Prog.Id;

		var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
			StagedPerformanceSeederHelper.EnsureWork(context, "parish.high-mass", work));
		StringAssert.Contains(error.Message, "must be public");
		Assert.AreEqual(0, context.StagedPerformanceRecords.Count());

		privateProg.Prog.Public = true;
		context.SaveChanges();
		Assert.AreEqual(StagedPerformanceSeedResult.Created,
			StagedPerformanceSeederHelper.EnsureWork(context, "parish.high-mass", work));
		Assert.IsFalse(System.Text.Json.JsonSerializer.Deserialize<PerformanceWork>(context.StagedPerformanceRecords.Single().Definition)!.StaffAuthored);

		context.StagedPerformanceRecords.Remove(context.StagedPerformanceRecords.Single());
		privateProg.Parameter.ParameterType = (long)ProgVariableTypes.Location;
		context.SaveChanges();
		error = Assert.ThrowsExactly<InvalidOperationException>(() =>
			StagedPerformanceSeederHelper.EnsureWork(context, "parish.high-mass", work));
		StringAssert.Contains(error.Message, "one Character parameter");
	}

	[TestMethod]
	public void EnsureWork_AllowsPrivateEligibilityProgOnlyForExplicitTrustedStaffAuthoredDefinition()
	{
		using var context = CreateContext();
		var privateProg = BooleanCharacterProg(publicProg: false);
		context.FutureProgs.Add(privateProg.Prog);
		context.FutureProgsParameters.Add(privateProg.Parameter);
		context.SaveChanges();
		var work = ValidWork("staff.chapel-rite", "Staff Chapel Rite");
		work.Roles[0].EligibilityProgId = privateProg.Prog.Id;

		Assert.AreEqual(StagedPerformanceSeedResult.Created,
			StagedPerformanceSeederHelper.EnsureWork(context, "staff.chapel-rite", work, trustedStaffAuthored: true));
		var installed = System.Text.Json.JsonSerializer.Deserialize<PerformanceWork>(context.StagedPerformanceRecords.Single().Definition)!;
		Assert.IsTrue(installed.StaffAuthored);
		Assert.AreEqual("staff.chapel-rite", installed.SeedKey);
	}

	[TestMethod]
	public void EnsureWork_ReportsStructuralValidationBeforeWriting()
	{
		using var context = CreateContext();
		var invalid = ValidWork("parish.high-mass", "Parish High Mass");
		invalid.Roles[0].Mark = "missing-place";

		var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
			StagedPerformanceSeederHelper.EnsureWork(context, "parish.high-mass", invalid));
		StringAssert.Contains(error.Message, "starting mark");
		Assert.AreEqual(0, context.StagedPerformanceRecords.Count());
	}

	[TestMethod]
	public void EnsureHelpfile_ReusesPendingAndPersistedTopicAcrossUpdates()
	{
		using var context = CreateContext();
		var topic = HelpfileSeederHelper.EnsureHelpfile(context, "Venue guide", h =>
		{
			h.Category = "Performances";
			h.Subcategory = "Venues";
			h.TagLine = "Find the starting mark.";
			h.PublicText = "Meet in the hall.";
			h.Keywords = "venue hall";
			h.LastEditedBy = "System";
			h.LastEditedDate = DateTime.UtcNow;
		});
		Assert.AreSame(topic, HelpfileSeederHelper.EnsureHelpfile(context, "Venue guide", h => h.Keywords = "hall mark"));
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var revised = HelpfileSeederHelper.EnsureHelpfile(context, "Venue Guide", h => h.PublicText = "Meet in the gallery.");
		context.SaveChanges();
		Assert.AreEqual(topic.Id, revised.Id);
		Assert.AreEqual("hall mark", revised.Keywords);
		Assert.AreEqual("Meet in the gallery.", context.Helpfiles.Single().PublicText);
	}

	private static FuturemudDatabaseContext CreateContext() =>
		new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options);

	private static PerformanceWork ValidWork(string? key, string name) => new()
	{
		SeedKey = key,
		Name = name,
		Roles = [new PerformanceRole { Key = "leader", Mark = "altar", Mode = PerformanceMode.Npc, Required = true }],
		Places = ["altar"],
		Cues = [new PerformanceCue { Role = "leader", Place = "altar", Verb = PerformanceVerb.Speak, Text = "We gather." }]
	};

	private static (FutureProg Prog, FutureProgsParameter Parameter) BooleanCharacterProg(bool publicProg)
	{
		var prog = new FutureProg
		{
			FunctionName = "SeededEligibility",
			FunctionComment = "Eligibility test",
			FunctionText = "return true",
			ReturnType = (long)ProgVariableTypes.Boolean,
			Category = "General",
			Subcategory = "Seeder Tests",
			Public = publicProg,
			StaticType = 0
		};
		return (prog, new FutureProgsParameter
		{
			FutureProg = prog,
			ParameterIndex = 0,
			ParameterType = (long)ProgVariableTypes.Character,
			ParameterName = "character"
		});
	}
}
