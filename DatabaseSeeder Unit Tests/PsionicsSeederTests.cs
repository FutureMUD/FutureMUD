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
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PsionicsSeederTests
{
	private static FuturemudDatabaseContext Context()
	{
		var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false))
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		context.TraitDefinitions.Add(new TraitDefinition { Name = "Example Skill", Type = 0, DecoratorId = 1, ImproverId = 1 });
		context.Races.Add(new Race { Name = "Example Race" });
		context.SaveChanges();
		return context;
	}
	[TestMethod]
	public void ContactVariants_HaveSeparateProgsRangesAndDistinctVerbs()
	{
		using var context = Context();
		new PsionicsSeeder().SeedData(context, new Dictionary<string, string> { ["install-psionics"] = "yes" });
		foreach (var school in context.MagicSchools)
		{
			var contact = XElement.Parse(context.MagicPowers.Single(x => x.Name == school.Name + ": contact").Definition);
			Assert.AreNotEqual(contact.Element("TargetCanSeeIdentityProg")!.Value, contact.Element("TargetEligibilityProg")!.Value);
			Assert.AreEqual(((int)(school.Name == "Basic Psionics" ? MudSharp.Magic.MagicPowerDistance.SameZoneOnly : MudSharp.Magic.MagicPowerDistance.SameShardOnly)).ToString(), contact.Element("PowerDistance")!.Value);
			var back = XElement.Parse(context.MagicPowers.Single(x => x.Name == school.Name + ": connectback").Definition);
			Assert.AreEqual(((int)MudSharp.Magic.MagicPowerDistance.AnyConnectedMindOrConnectedTo).ToString(), back.Element("PowerDistance")!.Value);
			Assert.AreEqual("disconnectback", back.Element("DisconnectVerb")!.Value);
			Assert.AreEqual(6, contact.Elements().Where(x => x.Name.LocalName.Contains("Emote") && x.Name.LocalName.Contains("Connect") || x.Name.LocalName.Contains("Emote") && x.Name.LocalName.Contains("Disconnect")).Select(x => x.Value).Distinct().Count());
		}
	}

	[TestMethod]
	public void Rerun_RepairsRecognisedPlaceholdersButKeepsAuthoredEmotes()
	{
		using var context = Context();
		var seeder = new PsionicsSeeder();
		var answers = new Dictionary<string, string> { ["install-psionics"] = "yes" };
		seeder.SeedData(context, answers);
		var power = context.MagicPowers.Single(x => x.Name == "Basic Psionics: contact");
		var xml = XElement.Parse(power.Definition);
		xml.SetElementValue("EmoteForConnect", "You feel a mental presence shift.");
		xml.SetElementValue("SelfEmoteForConnect", "My custom contact echo.");
		power.Definition = xml.ToString(); context.SaveChanges();
		seeder.SeedData(context, answers);
		xml = XElement.Parse(power.Definition);
		Assert.AreEqual("My custom contact echo.", xml.Element("SelfEmoteForConnect")!.Value);
		Assert.AreEqual(MudSharp.Magic.PsionicPowerEmotes.Get("connectmind", "EmoteForConnect"), xml.Element("EmoteForConnect")!.Value);
	}

	[TestMethod]
	public void InstallAndRerun_PreserveCustomisationsAndNeverGrantAccess()
	{
		using var context = Context();
		var seeder = new PsionicsSeeder();
		var answers = new Dictionary<string, string> { ["install-psionics"] = "yes" };
		seeder.SeedData(context, answers);
		Assert.AreEqual(2, context.MagicSchools.Count());
		Assert.AreEqual(2, context.MagicCapabilities.Count());
		Assert.AreEqual(4, context.MagicSpells.Count());
		foreach (var stock in MudSharp.Magic.PsionicStockContent.SpellPowers)
		{
			var spell = context.MagicSpells.Single(x => x.Name == "Advanced Psionics: " + stock.Verb);
			Assert.AreEqual(stock.Seconds.ToString(), context.TraitExpressions.Find(spell.EffectDurationExpressionId)!.Expression);
			Assert.IsTrue(stock.Cost > 0 && stock.Cost <= MudSharp.Magic.PsionicStockContent.FocusCap);
		}
		Assert.AreEqual(0, context.PerceiverMerits.Count());
		Assert.AreEqual(0, context.CharacterTraits.Count());
		Assert.AreEqual(0, context.StaticConfigurations.Count());
		var power = context.MagicPowers.First(x => x.PowerModel == "forgetting");
		var xml = XElement.Parse(power.Definition);
		Assert.AreEqual("false", xml.Element("Permanent")!.Value);
		xml.SetElementValue("Duration", 71);
		power.Definition = xml.ToString();
		context.SaveChanges();
		var count = context.MagicPowers.Count();
		seeder.SeedData(context, answers);
		Assert.AreEqual(count, context.MagicPowers.Count());
		Assert.AreEqual("71", XElement.Parse(power.Definition).Element("Duration")!.Value);
		Assert.AreEqual(0, context.PerceiverMerits.Count());
	}
	[TestMethod]
	public void DecliningPackage_DoesNotWriteMagicContent()
	{
		using var context = Context();
		new PsionicsSeeder().SeedData(context, new Dictionary<string, string> { ["install-psionics"] = "no" });
		Assert.AreEqual(0, context.MagicSchools.Count());
		Assert.AreEqual(0, context.MagicResources.Count());
	}
	[TestMethod]
	public void ConflictingPowerIdentity_IsReportedWithoutOverwritingIt()
	{
		using var context = Context();
		var answers = new Dictionary<string, string> { ["install-psionics"] = "yes" };
		var seeder = new PsionicsSeeder();
		seeder.SeedData(context, answers);
		var power = context.MagicPowers.First();
		power.PowerModel = "custom-conflict";
		context.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => seeder.SeedData(context, answers));
		Assert.AreEqual("custom-conflict", power.PowerModel);
	}
}
