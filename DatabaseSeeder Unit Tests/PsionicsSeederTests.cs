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
		context.Races.Add(new Race { Name = "Human" });
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
			var contact = XElement.Parse(context.MagicPowers.Single(x => x.MagicSchoolId == school.Id && x.Name == "Mind Contact").Definition);
			Assert.AreNotEqual(contact.Element("TargetCanSeeIdentityProg")!.Value, contact.Element("TargetEligibilityProg")!.Value);
			Assert.AreEqual(((int)(school.Name == "Basic Psionics" ? MudSharp.Magic.MagicPowerDistance.SameZoneOnly : MudSharp.Magic.MagicPowerDistance.SameShardOnly)).ToString(), contact.Element("PowerDistance")!.Value);
			var back = XElement.Parse(context.MagicPowers.Single(x => x.MagicSchoolId == school.Id && x.Name == "Connect Back").Definition);
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
		var power = context.MagicPowers.Single(x => x.Name == "Mind Contact" && x.MagicSchoolId == context.MagicSchools.Single(y => y.Name == "Basic Psionics").Id);
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
			foreach (var (field, echo) in MudSharp.Magic.PsionicPowerEmotes.Spells[stock.Verb])
				Assert.AreEqual(echo, typeof(MagicSpell).GetProperty(field)!.GetValue(spell));
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
	[TestMethod]
	public void PlayerContent_HasSchoolScopedNamesSyntaxAndHumanProjection()
	{
		using var context = Context();
		new PsionicsSeeder().SeedData(context, new Dictionary<string, string> { ["install-psionics"] = "yes" });
		Assert.AreEqual(2, context.MagicPowers.Count(x => x.Name == "Mind Contact"));
		foreach (var power in context.MagicPowers)
		{
			Assert.IsFalse(power.Name.Contains(":"), power.Name);
			StringAssert.Contains(power.ShowHelp, "Syntax:", power.Name);
			StringAssert.Contains(power.ShowHelp, context.MagicSchools.Find(power.MagicSchoolId)!.SchoolVerb + " ", power.Name);
			Assert.IsFalse(power.ShowHelp.Contains("EnablePsychometric") || power.ShowHelp.Contains("builder") || power.ShowHelp.Contains("example power") || power.ShowHelp.Contains("{range}"), power.Name);
		}
		var projection = XElement.Parse(context.MagicSpells.Single(x => x.Name == "Advanced Psionics: project").Definition);
		Assert.AreEqual(context.Races.Single(x => x.Name == "Human").Id, (long)projection.Element("Effects")!.Element("Effect")!.Element("Race")!);
	}

	[TestMethod]
	public void Rerun_RepairsLegacyContentAndPreservesRenamedCustomContent()
	{
		using var context = Context();
		var seeder = new PsionicsSeeder();
		var answers = new Dictionary<string, string> { ["install-psionics"] = "yes" };
		seeder.SeedData(context, answers);
		var power = context.MagicPowers.First(x => x.Name == "Somatic Sense");
		var id = power.Id;
		power.Name = "Advanced Psionics: somaticsense";
		power.ShowHelp = MudSharp.Magic.PsionicStockContent.Powers.Single(x => x.Verb == "somaticsense").LegacyHelp;
		var xml = XElement.Parse(power.Definition);
		xml.Element("SeededIdentity")!.Remove();
		power.Definition = xml.ToString();
		context.SaveChanges();
		seeder.SeedData(context, answers);
		Assert.AreEqual("Somatic Sense", power.Name);
		StringAssert.Contains(power.ShowHelp, "apsi somaticsense");
		power.Name = "Vital Awareness";
		power.Blurb = "Custom blurb";
		power.ShowHelp = "Custom help";
		context.SaveChanges();
		var count = context.MagicPowers.Count();
		seeder.SeedData(context, answers);
		Assert.AreEqual(count, context.MagicPowers.Count());
		Assert.AreEqual(id, power.Id);
		Assert.AreEqual("Vital Awareness", power.Name);
		Assert.AreEqual("Custom blurb", power.Blurb);
		Assert.AreEqual("Custom help", power.ShowHelp);
	}

	[TestMethod]
	public void MissingHuman_PreventsInstallation()
	{
		using var context = Context();
		context.Races.Remove(context.Races.Single(x => x.Name == "Human"));
		context.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => new PsionicsSeeder().SeedData(context, new Dictionary<string, string> { ["install-psionics"] = "yes" }));
		Assert.AreEqual(0, context.MagicPowers.Count());
	}

	[TestMethod]
	public void Rerun_RepairsOldProjectionRaceButPreservesCustomRace()
	{
		using var context = Context();
		var seeder = new PsionicsSeeder();
		var answers = new Dictionary<string, string>();
		seeder.SeedData(context, answers);
		var spell = context.MagicSpells.Single(x => x.Name == "Advanced Psionics: project");
		var xml = XElement.Parse(spell.Definition);
		xml.Element("Effects")!.Element("Effect")!.SetElementValue("Race", context.Races.OrderBy(x => x.Id).First().Id);
		spell.Definition = xml.ToString();
		spell.Description = "Uses ordinary spell targeting, materials, costs, resistance and cleanup. Power access controls spell knowledge.";
		context.SaveChanges();
		seeder.SeedData(context, answers);
		xml = XElement.Parse(spell.Definition);
		Assert.AreEqual(context.Races.Single(x => x.Name == "Human").Id, (long)xml.Element("Effects")!.Element("Effect")!.Element("Race")!);
		context.Races.Add(new Race { Name = "Custom Astral Race" });
		context.SaveChanges();
		var customId = context.Races.Single(x => x.Name == "Custom Astral Race").Id;
		xml.Element("Effects")!.Element("Effect")!.SetElementValue("Race", customId);
		spell.Definition = xml.ToString();
		spell.Description = "Uses ordinary spell targeting, materials, costs, resistance and cleanup. Power access controls spell knowledge.";
		context.SaveChanges();
		seeder.SeedData(context, answers);
		Assert.AreEqual(customId, (long)XElement.Parse(spell.Definition).Element("Effects")!.Element("Effect")!.Element("Race")!);
	}

}
