using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using Material = MudSharp.Models.Material;
using Liquid = MudSharp.Models.Liquid;
using Gas = MudSharp.Models.Gas;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalExposureSeederTests
{
	private static FuturemudDatabaseContext Context()
	{
		var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false))
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		context.Materials.AddRange(new Material { Name = "flesh", HeatDamagePoint = 412.0389 }, new Material { Name = "PTFE" },
			new Material { Name = "glass" }, new Material { Name = "builder's unknown stone" });
		context.Liquids.Add(new Liquid { Name = "water", Description = "water" });
		context.Gases.Add(new Gas { Name = "Air", Description = "air" });
		context.SaveChanges(); return context;
	}
	private static Dictionary<string, string> Options(bool natural = true, bool fantasy = false) => new() { ["natural"] = natural ? "yes" : "no", ["fantasy"] = fantasy ? "yes" : "no" };
	[TestMethod]
	public void OptOut_AuditsEveryRowWithoutInstallingOrEnablingHazards()
	{
		using var context = Context();
		var report = new EnvironmentalExposureSeeder().SeedData(context, Options(false));
		Assert.AreEqual(1, context.Liquids.Count()); Assert.AreEqual(1, context.Gases.Count());
		Assert.AreEqual(0, context.StaticConfigurations.Count()); Assert.AreEqual(6, ExposureCatalogueAudit.Capture(context).Count);
		StringAssert.Contains(report, "Audited 4 solids");
	}
	[TestMethod]
	public void NaturalInstall_UsesSelectiveRulesAndKeepsNegativeControls()
	{
		using var context = Context(); new EnvironmentalExposureSeeder().SeedData(context, Options());
		Assert.IsNull(context.Liquids.Single(x => x.Name == "water").SurfaceReactionInfo);
		Assert.IsNull(context.Gases.Single(x => x.Name == "Air").SurfaceReactionInfo);
		Assert.IsFalse(context.Liquids.Any(x => x.Name == "infernal lava"));
		Assert.IsFalse(context.Materials.Any(x => x.Name == "exposure accursed bone"));
		var ptfe = context.Materials.Single(x => x.Name == "PTFE"); var glass = context.Materials.Single(x => x.Name == "glass");
		XElement Rule(string liquid, long material) => XElement.Parse(context.Liquids.Single(x => x.Name == liquid).SurfaceReactionInfo).Elements("Reaction").Single(x => (long?)x.Attribute("Material") == material);
		Assert.AreEqual(true, (bool?)Rule("hydrochloric acid", ptfe.Id).Attribute("NoReaction"));
		Assert.AreEqual(true, (bool?)Rule("hydrochloric acid", glass.Id).Attribute("NoReaction"));
		Assert.AreEqual(false, (bool?)Rule("hydrofluoric acid", glass.Id).Attribute("NoReaction"));
		Assert.AreEqual(55, context.Materials.Single(x => x.Name == "flesh").HeatDamagePoint);
		Assert.AreEqual(0, context.StaticConfigurations.Count());
	}
	[TestMethod]
	public void Rerun_RepairsMissingOwnedRowsAndPreservesCustomRulesAndTransmission()
	{
		using var context = Context(); var seeder = new EnvironmentalExposureSeeder(); var options = Options(true, true);
		seeder.SeedData(context, options); context.SaveChanges();
		var acid = context.Liquids.Single(x => x.Name == "hydrochloric acid");
		var root = XElement.Parse(acid.SurfaceReactionInfo); var rule = root.Elements("Reaction").First(); var id = (string)rule.Attribute("Id")!;
		rule.SetAttributeValue("DamageRate", 7.125); root.Add(new XElement("CustomBuilderMetadata", "keep")); acid.SurfaceReactionInfo = root.ToString();
		var flesh = context.Materials.Single(x => x.Name == "flesh"); flesh.ExposureInfo = "<Exposure Liquid='0.123'/>"; flesh.HeatDamagePoint = 71;
		context.Liquids.Remove(context.Liquids.Single(x => x.Name == "lava")); context.SaveChanges();
		var count = context.Liquids.Count(); var report = seeder.SeedData(context, options); context.SaveChanges();
		Assert.AreEqual(count + 1, context.Liquids.Count()); Assert.AreEqual(1, context.Liquids.Count(x => x.Name == "lava"));
		var after = XElement.Parse(acid.SurfaceReactionInfo);
		Assert.AreEqual(7.125, (double?)after.Elements("Reaction").Single(x => (string?)x.Attribute("Id") == id).Attribute("DamageRate"));
		Assert.AreEqual("keep", (string?)after.Element("CustomBuilderMetadata"));
		Assert.AreEqual("<Exposure Liquid='0.123'/>", flesh.ExposureInfo); Assert.AreEqual(71, flesh.HeatDamagePoint);
		StringAssert.Contains(report, "preserved builder edit");
		Assert.AreEqual(context.MagicalSubstances.Count(), context.MagicalSubstances.Select(x => x.Name).Distinct().Count());
		Assert.AreEqual(context.SeederManagedRecords.Count(), context.SeederManagedRecords.Select(x => new { x.Seeder, x.EntityType, x.StableKey }).Distinct().Count());
	}
	[TestMethod]
	public void UnownedNameCollision_IsReportedAndNeverAdopted()
	{
		using var context = Context(); context.Liquids.Add(new Liquid { Name = "lava", Description = "builder's lava" }); context.SaveChanges();
		var report = new EnvironmentalExposureSeeder().SeedData(context, Options());
		Assert.AreEqual("builder's lava", context.Liquids.Single(x => x.Name == "lava").Description);
		Assert.IsNull(context.Liquids.Single(x => x.Name == "lava").SurfaceReactionInfo);
		StringAssert.Contains(report, "name collision: liquid:lava");
	}
	[TestMethod]
	public void FantasyAndPreparations_HaveDistinctRoutesTargetsAndLifecycle()
	{
		using var context = Context(); new EnvironmentalExposureSeeder().SeedData(context, Options(true, true));
		Assert.IsTrue(context.Liquids.Any(x => x.Name == "sacred water"));
		var sacred = XElement.Parse(context.Liquids.Single(x => x.Name == "sacred water").SurfaceReactionInfo);
		var accursed = context.Materials.Single(x => x.Name == "exposure accursed bone");
		Assert.IsTrue(sacred.Elements("Reaction").Where(x => (bool?)x.Attribute("NoReaction") != true).All(x => (long?)x.Attribute("Material") == accursed.Id));
		var spells = context.MagicSpells.ToList().Select(x => XElement.Parse(x.Definition)).ToArray();
		Assert.IsTrue(spells.Any(x => (string?)x.Element("Trigger")?.Attribute("type") == "substanceitem"));
		Assert.IsTrue(spells.Any(x => (int?)x.Element("Effects")?.Element("Effect")?.Element("Routes") == (int)ExposureRoute.Inhalation));
		Assert.AreEqual(0, context.MagicCapabilities.Count()); Assert.AreEqual(0, context.Rooms.Count());
		Assert.AreEqual(5, context.MagicalSubstances.Count());
	}
	[TestMethod]
	public void FantasyRerun_PreservesBuilderRemovedAccursedTag()
	{
		using var context = Context(); var seeder = new EnvironmentalExposureSeeder();
		seeder.SeedData(context, Options(true, true));
		var material = context.Materials.Single(x => x.Name == "exposure accursed bone");
		var relation = material.MaterialsTags.Single();
		material.MaterialsTags.Remove(relation); context.Remove(relation); context.SaveChanges();
		var report = seeder.SeedData(context, Options(true, true));
		Assert.AreEqual(0, material.MaterialsTags.Count);
		StringAssert.Contains(report, "preserved builder edit");
	}

	[TestMethod]
	public void UncertainMaterials_RemainExplicitAndNoBroadOrganicAssumptionIsMade()
	{
		Assert.AreEqual(ExposureMaterialFamily.Uncertain, ExposureCatalogue.Classify("unobtainium", new[] { "Materials / Organic" }));
		Assert.AreEqual(ExposureMaterialFamily.Uncertain, ExposureCatalogue.Classify("copper ore", new[] { "Materials / Stone / Metal Ore" }));
		using var context = Context(); new EnvironmentalExposureSeeder().SeedData(context, Options());
		Assert.AreEqual("unsupported/uncertain", ExposureCatalogueAudit.Capture(context).Single(x => x.Name == "builder's unknown stone").Disposition);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CombinedGasUpgrade_SplitsStockRoutesButPreservesCustomisedCombinedRule(bool customised)
	{
		using var context = Context();
		var seeder = new EnvironmentalExposureSeeder();
		seeder.SeedData(context, Options());
		var gas = context.Gases.Single(x => x.Name == "Chlorine");
		var materialId = context.Materials.Single(x => x.Name == "flesh").Id;
		var root = XElement.Parse(gas.SurfaceReactionInfo);
		var external = root.Elements("Reaction").Single(x => (long?)x.Attribute("Material") == materialId && (int?)x.Attribute("Routes") == (int)ExposureRoute.GasContact);
		var inhaled = root.Elements("Reaction").Single(x => (long?)x.Attribute("Material") == materialId && (int?)x.Attribute("Routes") == (int)ExposureRoute.Inhalation);
		var externalId = (string)external.Attribute("Id")!;
		var record = context.SeederManagedRecords.Single(x => x.EntityType == "GasReactions" && x.StableKey == "profile:Chlorine");
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		baseline.Remove("rule:" + (string)inhaled.Attribute("Id")!);
		inhaled.Remove();
		external.SetAttributeValue("Routes", (int)(ExposureRoute.GasContact | ExposureRoute.Inhalation));
		baseline["rule:" + externalId] = external.ToString(SaveOptions.DisableFormatting);
		record.SeedBaseline = JsonSerializer.Serialize(baseline);
		if (customised) external.SetAttributeValue("DamageRate", 7.125);
		var preserved = external.ToString(SaveOptions.DisableFormatting);
		gas.SurfaceReactionInfo = root.ToString();
		context.SaveChanges();

		var report = seeder.SeedData(context, Options());
		var rules = XElement.Parse(gas.SurfaceReactionInfo).Elements("Reaction").Where(x => (long?)x.Attribute("Material") == materialId).ToArray();
		Assert.AreEqual(customised ? 1 : 2, rules.Length);
		Assert.IsTrue(rules.Any(x => (string?)x.Attribute("Id") == externalId));
		if (customised)
		{
			Assert.AreEqual(preserved, rules.Single().ToString(SaveOptions.DisableFormatting));
			StringAssert.Contains(report, "deferred its separate inhalation rule");
		}
		else
		{
			Assert.AreEqual(0.6, (double?)rules.Single(x => (int?)x.Attribute("Routes") == (int)ExposureRoute.GasContact).Attribute("DamageRate"));
			Assert.AreEqual(18.0, (double?)rules.Single(x => (int?)x.Attribute("Routes") == (int)ExposureRoute.Inhalation).Attribute("DamageRate"));
		}
		var after = gas.SurfaceReactionInfo;
		seeder.SeedData(context, Options());
		Assert.AreEqual(after, gas.SurfaceReactionInfo, "A second rerun must neither duplicate nor reinterpret the preserved routes.");
	}

	[TestMethod]
	public void GasRerun_ReportsPreExistingBuilderOverlapWithoutDeletingEitherCustomisation()
	{
		using var context = Context();
		var seeder = new EnvironmentalExposureSeeder();
		seeder.SeedData(context, Options());
		var gas = context.Gases.Single(x => x.Name == "Chlorine");
		var materialId = context.Materials.Single(x => x.Name == "flesh").Id;
		var root = XElement.Parse(gas.SurfaceReactionInfo);
		var rules = root.Elements("Reaction").Where(x => (long?)x.Attribute("Material") == materialId).ToArray();
		rules.Single(x => (int?)x.Attribute("Routes") == (int)ExposureRoute.GasContact).SetAttributeValue("Routes", (int)(ExposureRoute.GasContact | ExposureRoute.Inhalation));
		rules.Single(x => (int?)x.Attribute("Routes") == (int)ExposureRoute.Inhalation).SetAttributeValue("DamageRate", 23);
		gas.SurfaceReactionInfo = root.ToString();
		context.SaveChanges();
		var report = seeder.SeedData(context, Options());
		var after = XElement.Parse(gas.SurfaceReactionInfo).Elements("Reaction").Where(x => (long?)x.Attribute("Material") == materialId).ToArray();
		CollectionAssert.AreEquivalent(rules.Select(x => x.ToString()).ToArray(), after.Select(x => x.ToString()).ToArray());
		StringAssert.Contains(report, "both builder definitions are preserved");
		StringAssert.Contains(report, "remain inactive until the builder resolves");
	}

	[DataTestMethod]
	[DataRow(false, 2, 45.0)]
	[DataRow(true, 5, 10.0)]
	public void StockChlorine_RespiratoryRatePenetratesSeededOrganArmourWithoutChangingCombat(bool animal, int quality, double expectedPerOrgan)
	{
		using var context = Context();
		new EnvironmentalExposureSeeder().SeedData(context, Options());
		var materialId = context.Materials.Single(x => x.Name == "flesh").Id;
		var rules = XElement.Parse(context.Gases.Single(x => x.Name == "Chlorine").SurfaceReactionInfo).Elements("Reaction")
			.Where(x => (long?)x.Attribute("Material") == materialId).ToArray();
		var inhaled = rules.Single(x => (int?)x.Attribute("Routes") == (int)ExposureRoute.Inhalation);
		var rate = (double)inhaled.Attribute("DamageRate")!;
		var externalRate = (double)rules.Single(x => (int?)x.Attribute("Routes") == (int)ExposureRoute.GasContact).Attribute("DamageRate")!;
		Assert.AreEqual(0.6, externalRate);

		string definition;
		if (animal)
		{
			var seeder = new AnimalSeeder();
			typeof(AnimalSeeder).GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(seeder, context);
			typeof(AnimalSeeder).GetMethod("SetupArmourTypes", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, null);
			definition = context.ArmourTypes.Single(x => x.Name == "Non-Human Natural Organ Armour").Definition;
		}
		else definition = (string)typeof(HumanSeeder).GetMethod("BuildHumanOrganArmourDefinition", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, null)!;
		var armour = new MudSharp.Combat.ArmourType(new ArmourType { Definition = definition }, Mock.Of<IFuturemud>());
		var owner = Mock.Of<ICharacter>();
		var material = Mock.Of<ISolid>();
		double Resolve(double seconds, double strength = 1, double? overrideRate = null, bool ordinary = false)
		{
			var amount = (overrideRate ?? rate) / 3 * seconds * strength;
			var damage = new MudSharp.Health.Damage
			{
				DamageType = DamageType.Chemical, DamageAmount = amount, PainAmount = amount,
				ExposureContext = ordinary ? null : new ExposureDamageContext(ExposureRoute.Inhalation, ExposureSourceKind.Atmosphere,
					"stock-chlorine", "chemical", "chemical", Guid.Parse((string)inhaled.Attribute("Id")!), seconds)
			};
			var wounds = new List<IWound>();
			return armour.AbsorbDamage(damage, (ItemQuality)quality, material, owner, ref wounds).SufferedDamage?.DamageAmount ?? 0;
		}
		Assert.AreEqual(expectedPerOrgan, Resolve(10), 1e-9);
		Assert.AreEqual(Resolve(10), Enumerable.Range(0, 40).Sum(_ => Resolve(0.25)), 1e-9);
		Assert.AreEqual(0, Resolve(10, overrideRate: externalRate), 1e-9, "The old shared rate is absorbed by actual stock organ armour.");
		Assert.AreEqual(0, Resolve(10, strength: 0.25), 1e-9, "The seeded respiratory draught reduces the calibrated packet before ordinary armour.");
		Assert.AreEqual(60 - quality * (animal ? 1 : 0.75), Resolve(10, ordinary: true), 1e-9, "Ordinary combat keeps strike-based dissipation.");
	}
}
