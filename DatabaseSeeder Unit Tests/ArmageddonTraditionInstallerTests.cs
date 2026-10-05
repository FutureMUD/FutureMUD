#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonTraditionInstallerTests
{
	private static (FuturemudDatabaseContext Db, ArmageddonTraditionInstallPlan Plan) Fixture()
	{
		var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false))
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		db.MagicSchools.Add(new() { Id = 1, Name = "explicit school" }); db.MagicResources.Add(new()
		{ Id = 1, Name = "explicit mana", Type = "simple", Definition = "<Definition/>", MagicResourceType = (int)(MagicResourceType.PlayerResource | MagicResourceType.LocationResource) });
		db.TraitDecorators.Add(new() { Id = 1, Name = "explicit decorator", Type = "SimpleNumeric", Contents = "" });
		db.TraitExpressions.Add(new() { Id = 1, Name = "native cap", Expression = "100" });
		db.Improvers.Add(new() { Id = 1, Name = "native use", Type = "classic", Definition = "<Definition/>" });
		db.TraitDefinitions.Add(new() { Id = 1, Name = "native gather", Type = 0, OwnerScope = 1, ImproverId = 1, ExpressionId = 1 });
		foreach (var (id, value) in new[] { (1L, "false"), (2L, "true") }) db.FutureProgs.Add(new()
		{ Id = id, FunctionName = "Always" + value, FunctionText = "return " + value, ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() });
		var gathering = new XElement("Gathering", new XAttribute("version", 2), Enum.GetValues<MagicGatheringMethodKind>().Select(x =>
			new XElement("Method", new XAttribute("key", Guid.NewGuid()), new XAttribute("kind", x), new XAttribute("alias", x.ToString().ToLowerInvariant()),
				new XAttribute("name", "native " + x), new XAttribute("destination", 1), new XAttribute("source", x == MagicGatheringMethodKind.Gentle ? 1 : 0),
				new XAttribute("stamina", x == MagicGatheringMethodKind.Self ? 1 : 0), new XAttribute("duration", 30), new XAttribute("min", 1), new XAttribute("max", 5),
				x == MagicGatheringMethodKind.Land ? new XElement("Land", new XAttribute("damage", 1), new XElement("Source", new XAttribute("key", Guid.NewGuid()), new XAttribute("selector", "crop"), new XAttribute("ratio", 1))) : null)));
		db.MagicCapabilities.Add(new() { Id = 1, Name = "native template", CapabilityModel = "skilllevel", MagicSchoolId = 1,
			Definition = new XElement("Definition", new XElement("ConcentrationTrait", 1), new XElement("ConcentrationCapabilityExpression", "3"),
				new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators", new XElement("Regenerator", 123)), gathering).ToString() });
		var spells = new Dictionary<string, long>();
		foreach (var content in ArmageddonMagicInstaller.Content(new(false, 0, 0, new Dictionary<string, long>(), 0, 0, 0, 0, 0, 0, 0, 0, 0)))
		{
			var row = content.SpellRow(1, 1, 1); row.Definition = content.BuildDefinition(1, 1, 0).ToString(); db.MagicSpells.Add(row); db.SaveChanges(); spells.Add(content.Key, row.Id);
		}
		db.SaveChanges(); db.ChangeTracker.Clear();
		return (db, new(true, 1, 1, 1, 1, 1, 2, 1, spells, new Dictionary<string, long> { ["arm.support.gather"] = 1 }));
	}
	private static ArmageddonTraditionInstallResult Run(FuturemudDatabaseContext db, ArmageddonTraditionInstallPlan plan)
	{ db.ChangeTracker.Clear(); return ArmageddonTraditionInstaller.Install(db, plan); }
	private static void Installed(ArmageddonTraditionInstallResult result) => Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
	private static XElement Policy(FuturemudDatabaseContext db, string variant) => XElement.Parse(db.MagicCapabilities.Single(x => x.Name == "Armageddon partial " + variant).Definition).Element("Casting")!;

	[TestMethod] public void ExactSourceRowsPreserveRootsCapsAndSupportBridge()
	{
		var rows = ArmageddonTraditionInstaller.SourceRows; Assert.AreEqual(94, rows.Count); Assert.AreEqual(82, rows.Count(x => x.Kind == "spell"));
		CollectionAssert.AreEquivalent(new[] { "arm.spell.sense_enchantment", "arm.spell.unravel_enchantment", "arm.spell.gust_hands", "arm.spell.unyielding_veil" },
			rows.Where(x => x.Kind == "spell" && x.ParentKey is null).Select(x => x.Key).ToArray());
		Assert.IsTrue(rows.Where(x => x.Kind == "spell").All(x => x.Opening == (x.ParentKey is null ? 60 : 30)));
		Assert.IsTrue(rows.Where(x => x.Kind == "spell").All(x => x.RawCap == (x.Key == "arm.spell.mend_flesh" ? 60 : 90)));
		Assert.AreEqual("arm.support.component_crafting", rows.Single(x => x.Key == "arm.spell.read_enchantment").ParentKey);
		Assert.AreEqual("arm.spell.shadow_passage", rows.Single(x => x.Key == "arm.support.component_crafting").ParentKey);
		foreach (var row in rows.Where(x => x.ParentKey is not null)) Assert.AreEqual(rows.Single(x => x.Key == row.ParentKey).BranchesAt, row.ParentThreshold);
	}
	[TestMethod] public void DeclineDoesNotInspectDisposedContext()
	{
		var (db, plan) = Fixture(); db.Dispose(); Assert.AreEqual(ArmageddonInstallStatus.Declined, ArmageddonTraditionInstaller.Install(db, plan with { Install = false }).Status);
	}
	[TestMethod] public void RealSkillDefinitionsAndPartialPathsHaveNoPlaceholderGrants()
	{
		var (db, plan) = Fixture(); using (db)
		{
			var result = Run(db, plan); Installed(result); Assert.AreEqual(171, result.Identities.Count);
			var skills = db.TraitDefinitions.Where(x => x.TraitGroup == "Armageddon Spell").ToArray(); Assert.AreEqual(82, skills.Length);
			Assert.AreEqual(82, skills.Select(x => x.ExpressionId).Distinct().Count());
			foreach (var row in ArmageddonTraditionInstaller.SourceRows.Where(x => x.Kind == "spell"))
			{
				var skill = skills.Single(x => x.Name == "Armageddon " + row.HistoricalName);
				Assert.AreEqual(row.RawCap.ToString(System.Globalization.CultureInfo.InvariantCulture), db.TraitExpressions.Single(x => x.Id == skill.ExpressionId).Expression);
				var record = db.SeederManagedRecords.Single(x => x.StableKey == row.Key + ".skill");
				StringAssert.Contains(record.SeedBaseline, row.Key);
			}
			CollectionAssert.AreEquivalent(new[] { "arm.spell.sense_enchantment", "arm.spell.unravel_enchantment", "arm.spell.draw_water" }, result.AvailableSpells.ToArray());
			Assert.AreEqual(79, result.UnavailableSpells.Count); Assert.AreEqual(5, db.MagicSpells.Count());
			foreach (var variant in ArmageddonTraditionInstaller.Variants)
			{
				var policy = Policy(db, variant); Assert.AreEqual(3, policy.Elements("Admission").Count()); Assert.AreEqual(2, policy.Elements("Admission").Count(x => (bool)x.Attribute("starting")!));
				Assert.IsFalse((bool)policy.Attribute("passive")!); Assert.AreEqual(1, policy.Elements("SupportGrant").Count());
				var draw = policy.Elements("Admission").Single(x => (long)x.Attribute("spell")! == plan.ImplementedSpells["arm.spell.draw_water"]);
				Assert.AreEqual(30, (double)draw.Attribute("opening")!); Assert.AreEqual(80, (double)draw.Element("Prerequisite")!.Attribute("proficiency")!);
			}
			Assert.AreEqual(0, db.CharacterTraits.Count()); Assert.AreEqual(0, db.CharacterCastingEnrolments.Count()); Assert.AreEqual(0, db.ChargenRoles.Count());
		}
	}
	[TestMethod] public void DefaultMatrixCopiesBuilderMethodsWithoutPassiveRegenerators()
	{
		var (db, plan) = Fixture(); using (db)
		{
			var template = db.MagicCapabilities.Find(1L)!.Definition; Installed(Run(db, plan));
			foreach (var (variant, expected) in new[] { ("sorcerer", new[] { "Self", "Gentle", "Land" }), ("preserver", new[] { "Self", "Gentle" }), ("defiler", new[] { "Self", "Land" }) })
			{
				var xml = XElement.Parse(db.MagicCapabilities.Single(x => x.Name == "Armageddon partial " + variant).Definition);
				CollectionAssert.AreEquivalent(expected, xml.Element("Gathering")!.Elements("Method").Select(x => (string)x.Attribute("kind")!).ToArray());
				Assert.IsFalse(xml.Element("Regenerators")!.Elements().Any()); Assert.IsTrue(xml.Element("Gathering")!.Elements().All(x => (double)x.Attribute("duration")! == 30));
			}
			Assert.AreEqual(template, db.MagicCapabilities.Find(1L)!.Definition);
			var keys = db.MagicCapabilities.Where(x => x.Id != 1).AsEnumerable().SelectMany(x => XElement.Parse(x.Definition).Descendants().Attributes("key")).Select(x => x.Value).ToArray();
			Assert.AreEqual(keys.Length, keys.Distinct().Count());
		}
	}
	[TestMethod] public void ExplicitMethodChoicesCanOverrideProposedMatrix()
	{
		var (db, plan) = Fixture(); using (db)
		{
			var choices = ArmageddonTraditionInstaller.Variants.ToDictionary(x => x, _ => (IReadOnlyList<MagicGatheringMethodKind>)new[] { MagicGatheringMethodKind.Land });
			Installed(Run(db, plan with { AllowedMethods = choices }));
			Assert.IsTrue(db.MagicCapabilities.Where(x => x.Id != 1).AsEnumerable().All(x => XElement.Parse(x.Definition).Element("Gathering")!.Elements("Method").Single().Attribute("kind")!.Value == "Land"));
		}
	}
	[TestMethod] public void StableRerunsPreserveEveryIdentityBaselineAndExternalSupport()
	{
		var (db, plan) = Fixture(); using (db)
		{
			var first = Run(db, plan); Installed(first); var records = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.StableKey, x => (x.LogicalId, x.SeedBaseline, x.AppliedAt));
			var support = JsonSerializer.Serialize(db.TraitDefinitions.Find(1L), new JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles });
			Installed(Run(db, plan)); foreach (var row in db.SeederManagedRecords) Assert.AreEqual(records[row.StableKey], (row.LogicalId, row.SeedBaseline, row.AppliedAt));
			Assert.AreEqual(support, JsonSerializer.Serialize(db.TraitDefinitions.Find(1L), new JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles }));
		}
	}
	[TestMethod] public void BuilderEditsAndCloneSurviveRepeatedReruns()
	{
		var (db, plan) = Fixture(); using (db)
		{
			Installed(Run(db, plan)); var cap = db.MagicCapabilities.Single(x => x.Name == "Armageddon partial sorcerer");
			cap.Name = "Builder caster"; var xml = XElement.Parse(cap.Definition); xml.Element("Casting")!.Elements("Admission").Last().Remove(); cap.Definition = xml.ToString();
			var skill = db.TraitDefinitions.Single(x => x.Name == "Armageddon heal"); skill.Name = "Builder remedy";
			var expression = db.TraitExpressions.Find(skill.ExpressionId)!; expression.Expression = "55";
			db.MagicCapabilities.Add(new() { Name = "Unowned clone", Definition = cap.Definition, MagicSchoolId = cap.MagicSchoolId, CapabilityModel = cap.CapabilityModel });
			db.SaveChanges(); var edited = cap.Definition;
			for (var i = 0; i < 2; i++) { var result = Run(db, plan); Installed(result); Assert.IsTrue(result.Messages.Any(x => x.Contains("preserved builder edit"))); }
			Assert.AreEqual(edited, db.MagicCapabilities.Find(cap.Id)!.Definition); Assert.AreEqual("55", db.TraitExpressions.Find(expression.Id)!.Expression);
			Assert.IsTrue(db.TraitDefinitions.Any(x => x.Name == "Builder remedy")); Assert.IsTrue(db.MagicCapabilities.Any(x => x.Name == "Unowned clone"));
		}
	}
	[DataTestMethod] [DataRow(false)] [DataRow(true)] public void MissingOrRetiredOwnedIdentityBlocksWholeBatch(bool retired)
	{
		var (db, plan) = Fixture(); using (db)
		{
			Installed(Run(db, plan)); var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.capability.preserver");
			if (retired) record.Retired = true; else db.MagicCapabilities.Remove(db.MagicCapabilities.Find(record.LogicalId) !);
			db.SaveChanges(); var before = db.SeederManagedRecords.Count(); Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status);
			Assert.AreEqual(before, db.SeederManagedRecords.Count()); Assert.IsFalse(!retired && db.MagicCapabilities.Any(x => x.Id == record.LogicalId));
		}
	}
	[TestMethod] public void UnownedCollisionRefusesAdoptionBeforeAnyOwnedWrites()
	{
		var (db, plan) = Fixture(); using (db)
		{
			db.TraitDefinitions.Add(new() { Name = "Armageddon heal" }); db.SaveChanges();
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status); Assert.AreEqual(0, db.SeederManagedRecords.Count());
		}
	}
	[TestMethod] public void CompetingIdentityClaimBlocksWithoutChangingOriginalBaseline()
	{
		var (db, plan) = Fixture(); using (db)
		{
			Installed(Run(db, plan)); var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.capability.sorcerer"); var baseline = record.SeedBaseline;
			db.SeederManagedRecords.Add(new() { Seeder = "foreign", Module = "foreign", StableKey = "foreign", EntityType = record.EntityType, LogicalId = record.LogicalId }); db.SaveChanges();
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status); Assert.AreEqual(baseline, db.SeederManagedRecords.Find(record.Id)!.SeedBaseline);
		}
	}
	[TestMethod] public void WrongRosterIdentityOrResourceRefusesBeforeMutation()
	{
		var (db, plan) = Fixture(); using (db)
		{
			var spells = new Dictionary<string, long>(plan.ImplementedSpells) { ["arm.spell.wardcraft"] = 999 };
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan with { ImplementedSpells = spells }).Status);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan with { SourceResource = 999 }).Status); Assert.AreEqual(0, db.SeederManagedRecords.Count());
		}
	}
	[TestMethod] public void MissingOrNonImprovingSupportCannotBecomePlaceholder()
	{
		var (db, plan) = Fixture(); using (db)
		{
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan with { SupportSkills = new Dictionary<string, long>() }).Status);
			db.Improvers.Find(1L)!.Type = "non-improving"; db.SaveChanges(); Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status);
		}
	}
	[DataTestMethod]
	[DataRow("selector")]
	[DataRow("scar")]
	[DataRow("collateral")]
	[DataRow("prog")]
	public void NativeLandValidationRejectsMalformedCopiedPolicyBeforeWrites(string fault)
	{
		var (db, plan) = Fixture(); using (db)
		{
			var template = db.MagicCapabilities.Find(1L)!;
			var xml = XElement.Parse(template.Definition);
			var land = xml.Element("Gathering")!.Elements("Method").Single(x => x.Attribute("kind")!.Value == "Land").Element("Land")!;
			if (fault == "selector") land.Element("Source")!.SetAttributeValue("selector", "invented-biomass");
			if (fault == "scar") land.SetAttributeValue("damage", 0);
			if (fault == "collateral") { land.Element("Source")!.SetAttributeValue("collateral", true); land.Element("Source")!.SetAttributeValue("allowAbsent", true); }
			if (fault == "prog") land.Element("Source")!.SetAttributeValue("ratioProg", plan.AlwaysTrueProg);
			template.Definition = xml.ToString(); db.SaveChanges();
			var result = Run(db, plan);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join("\n", result.Messages));
			Assert.IsTrue(result.Messages.Any(x => x.StartsWith("Native gathering template:", StringComparison.Ordinal)));
			Assert.AreEqual(0, db.SeederManagedRecords.Count()); Assert.AreEqual(1, db.MagicCapabilities.Count());
		}
	}
}
