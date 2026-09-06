#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.Models;

namespace DatabaseSeeder_Unit_Tests;

[TestClass]
public class PsionicCombatSeederTests
{
	private static FuturemudDatabaseContext Context()
	{
		var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false))
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		context.TraitDefinitions.Add(new TraitDefinition { Name = "Example Skill", Type = 0, DecoratorId = 1, ImproverId = 1 });
		context.Races.Add(new Race { Name = "Example Race" }); context.SaveChanges(); return context;
	}

	[TestMethod]
	public void Install_AdvancedSuiteCoversAllMechanics_WithoutBasicAccess()
	{
		using var context = Context(); new PsionicsSeeder().SeedData(context, new Dictionary<string, string> { ["install-psionics"] = "yes" });
		foreach (var stock in PsionicStockContent.CombatPowers)
		{
			var power = context.MagicPowers.Single(x => x.Name == "Advanced Psionics: " + stock.Name);
			var xml = XElement.Parse(power.Definition);
			Assert.AreEqual(stock.Defense.HasValue ? "magicdefense" : "magicattack", power.PowerModel);
			var entry = XElement.Parse(context.MagicCapabilities.Single(x => x.Name == "Advanced Psionics").Definition)
				.Elements("Power").Single(x => (long?)x.Attribute("power") == power.Id);
			Assert.AreEqual(stock.Band, (int)entry.Attribute("minvalue")!);
			Assert.IsFalse(XElement.Parse(context.MagicCapabilities.Single(x => x.Name == "Basic Psionics").Definition)
				.Elements("Power").Any(x => (long?)x.Attribute("power") == power.Id));
			if (!stock.Defense.HasValue) Assert.IsNotNull(context.WeaponAttacks.Find((long)xml.Element("WeaponAttack")!));
			Assert.IsTrue(stock.Cost > 0 && stock.Cost <= PsionicStockContent.FocusCap);
		}
		CollectionAssert.AreEquivalent(Enum.GetValues<MagicAttackEffectType>(), PsionicStockContent.CombatPowers.Where(x => x.Rider.HasValue).Select(x => x.Rider!.Value).ToArray());
		CollectionAssert.AreEquivalent(Enum.GetValues<MagicDefenseMode>(), PsionicStockContent.CombatPowers.Where(x => x.Defense.HasValue).Select(x => x.Defense!.Value).ToArray());
		Assert.AreEqual(0, context.CharacterTraits.Count()); Assert.AreEqual(0, context.PerceiverMerits.Count());
		Assert.IsTrue(context.MagicPowers.Any(x => x.PowerModel == "psychicbolt"));
	}

	[TestMethod]
	public void Rerun_PreservesEditedPowerAttackAndUnlock_AndAddsMissingCombatPower()
	{
		using var context = Context(); var seeder = new PsionicsSeeder(); var answers = new Dictionary<string, string> { ["install-psionics"] = "yes" };
		seeder.SeedData(context, answers);
		var power = context.MagicPowers.Single(x => x.Name == "Advanced Psionics: Force Lance");
		var definition = XElement.Parse(power.Definition); definition.SetElementValue("RangeInRooms", 7); power.Definition = definition.ToString();
		var attack = context.WeaponAttacks.Find((long)definition.Element("WeaponAttack")!)!; attack.BaseDelay = 17;
		var capability = context.MagicCapabilities.Single(x => x.Name == "Advanced Psionics");
		var cap = XElement.Parse(capability.Definition); cap.Elements("Power").Single(x => (long?)x.Attribute("power") == power.Id).SetAttributeValue("minvalue", 73);
		var removed = context.MagicPowers.Single(x => x.Name == "Advanced Psionics: Kinetic Parry");
		cap.Elements("Power").Single(x => (long?)x.Attribute("power") == removed.Id).Remove(); capability.Definition = cap.ToString();
		context.MagicPowers.Remove(removed); context.SaveChanges();
		var count = context.MagicPowers.Count(); seeder.SeedData(context, answers); seeder.SeedData(context, answers);
		Assert.AreEqual(count + 1, context.MagicPowers.Count()); Assert.AreEqual(8, context.WeaponAttacks.Count());
		Assert.AreEqual("7", XElement.Parse(power.Definition).Element("RangeInRooms")!.Value); Assert.AreEqual(17, attack.BaseDelay);
		Assert.AreEqual("73", XElement.Parse(capability.Definition).Elements("Power").Single(x => (long?)x.Attribute("power") == power.Id).Attribute("minvalue")!.Value);
		var newPower = context.MagicPowers.Single(x => x.Name == removed.Name);
		Assert.AreEqual(1, XElement.Parse(capability.Definition).Elements("Power").Count(x => (long?)x.Attribute("power") == newPower.Id));
	}

	[TestMethod]
	public void Rerun_ConflictingPowerTypeIsReportedRatherThanOverwritten()
	{
		using var context = Context(); var seeder = new PsionicsSeeder(); var answers = new Dictionary<string, string> { ["install-psionics"] = "yes" };
		seeder.SeedData(context, answers);
		var power = context.MagicPowers.Single(x => x.Name == "Advanced Psionics: Kinetic Barrier"); power.PowerModel = "mindbarrier"; context.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => seeder.SeedData(context, answers)); Assert.AreEqual("mindbarrier", power.PowerModel);
	}

	[TestMethod]
	public void StockCosts_BoundCombatEnduranceAndMaintainDistinctDefenseBudgets()
	{
		var strike = PsionicStockContent.CombatPowers.Single(x => x.Verb == "forcestrike");
		var lance = PsionicStockContent.CombatPowers.Single(x => x.Verb == "forcelance");
		Assert.IsTrue(lance.Cost > strike.Cost);
		foreach (var stock in PsionicStockContent.CombatPowers.Where(x => x.Defense.HasValue))
		{
			Assert.IsTrue(stock.ReactionCost > 0);
			Assert.IsTrue(stock.Cost + 10 * stock.ReactionCost + PsionicStockContent.CombatDefenseUpkeep * 2 < PsionicStockContent.FocusCap);
			Assert.IsTrue(stock.Charges > 0 && stock.Capacity > 0);
		}
	}
}
