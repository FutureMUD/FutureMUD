#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MonsterAIStockTemplateTests
{
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

	[TestMethod]
	[DataRow("Mythical", "Supernatural")]
	[DataRow("Supernatural", "Mythical")]
	public void Seed_PackOrderAndReruns_PreserveIdsClonesAndLegacyRows(string first, string second)
	{
		using var context = Context();
		var legacy = new ArtificialIntelligence { Name = "AnimalMythicGuardian", Type = "Animal", Definition = "<legacy/>" };
		var wildlife = new ArtificialIntelligence { Name = "Wildlife - Mythic Venom Web Hunter", Type = "Animal", Definition = "<wildlife/>" };
		var clone = new ArtificialIntelligence { Name = "My Dragon Controller", Type = "Monster", Definition = "<custom/>" };
		context.ArtificialIntelligences.AddRange(legacy, wildlife, clone); context.SaveChanges();
		Assert.IsTrue(MonsterAIStockTemplates.HasMissing(context, first));
		MonsterAIStockTemplates.Seed(context, first); MonsterAIStockTemplates.Seed(context, second);
		Assert.IsFalse(MonsterAIStockTemplates.HasMissing(context, first)); Assert.IsFalse(MonsterAIStockTemplates.HasMissing(context, second));
		var ids = context.ArtificialIntelligences.Where(x => x.Type == "Monster").ToDictionary(x => x.Name, x => x.Id);
		var stock = context.ArtificialIntelligences.Single(x => x.Name == MonsterAIStockTemplates.NightStalker); stock.Definition = "<old/>"; context.SaveChanges();
		MonsterAIStockTemplates.Seed(context, second); MonsterAIStockTemplates.Seed(context, first);
		Assert.AreEqual(MonsterAIStockTemplates.Names.Count + 3, context.ArtificialIntelligences.Count());
		Assert.IsTrue(context.ArtificialIntelligences.Where(x => x.Type == "Monster").All(x => ids[x.Name] == x.Id));
		Assert.AreEqual("<legacy/>", legacy.Definition); Assert.AreEqual("<wildlife/>", wildlife.Definition); Assert.AreEqual("<custom/>", clone.Definition);
		Assert.AreEqual("Monster", stock.Type); Assert.IsNotNull(XElement.Parse(stock.Definition).Element("Monster"));
	}

	[TestMethod]
	public void Seed_IncompatibleNameCollision_IsReportedWithoutConvertingIt()
	{
		using var context = Context();
		var custom = new ArtificialIntelligence { Name = MonsterAIStockTemplates.LairGuardian, Type = "Animal", Definition = "<custom/>" };
		context.ArtificialIntelligences.Add(custom); context.SaveChanges();
		Assert.ThrowsExactly<InvalidOperationException>(() => MonsterAIStockTemplates.Seed(context, "Mythical"));
		Assert.AreEqual("Animal", custom.Type); Assert.AreEqual("<custom/>", custom.Definition);
	}
}
