#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using Moq;
using MudSharp.Magic.Powers;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void Templars_FreshInstallCompilesProgsAndPreservesBuilderChangesOnRerun()
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, true);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var spells = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, spells.Status, string.Join("\n", spells.Messages));
		var devices = ArmageddonStockFocusDeviceInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, devices.Status, string.Join("\n", devices.Messages));
		// The stock-spell fixture represents the older magic-only package. Supply the complete
		// separately owned canonical skill graph required by the additive setting classes.
		foreach (var entry in ArmageddonSkillPackage.Skills)
		{
			var skill = new TraitDefinition { Name = "Canonical " + entry.Name, Type = 0,
				OwnerScope = (int)MudSharp.Body.Traits.TraitOwnerScope.Character, DecoratorId = 1, ImproverId = 1,
				Expression = new TraitExpression { Name = entry.Key + ".cap", Expression = "0" } };
			db.TraitDefinitions.Add(skill);
			db.SaveChanges();
			ArmageddonSettingOwnership.RecordNew(db, ArmageddonSkillPackage.Module, entry.Key, skill, nameof(TraitDefinition.Name));
			ArmageddonSettingOwnership.RecordNew(db, ArmageddonSkillPackage.Module, entry.Key + ".cap", skill.Expression, nameof(TraitExpression.Expression));
		}
		// These tests use a deliberately reduced human foundation; supply the native bindings required by this module.
		if (!db.Races.Any(x => x.Name == "Organic Humanoid"))
			db.Races.Add(new Race { Name = "Organic Humanoid", BaseBodyId = db.Races.Single(x => x.Name == "Human").BaseBodyId });
		foreach (var name in new[] { "Agility", "Willpower" })
			if (!db.TraitDefinitions.Any(x => x.Name == name)) db.TraitDefinitions.Add(new TraitDefinition { Name = name, Type = 1 });
		foreach (var name in new[] { "Neck", "Torso" })
			if (!db.BodypartShapes.Any(x => x.Name == name)) db.BodypartShapes.Add(new BodypartShape { Name = name });
		db.Accents.Add(new Accent { Name = "Standard", LanguageId = db.Languages.OrderBy(x => x.Id).First().Id });
		var sirihishId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.skill.sirihish").LogicalId!.Value;
		db.Languages.Add(new Language { Name = "Sirihish", LinkedTraitId = sirihishId, DifficultyModel = 1 });
		if (!db.Checks.Any(x => x.Type == (int)MudSharp.RPG.Checks.CheckType.AnalyzeEligibilityCheck))
			db.Checks.Add(new Check { Type = (int)MudSharp.RPG.Checks.CheckType.AnalyzeEligibilityCheck,
				TraitExpression = new TraitExpression { Name = "Analyze fixture", Expression = "0" } });
		db.SaveChanges();
		var messages = new List<string>();
		ArmageddonMundaneProgression.Install(db, messages);
		ArmageddonTemplarInstaller.Install(db, messages);
		db.ChangeTracker.Clear();
		var claims = db.SeederManagedRecords.Where(x => x.Module == ArmageddonTemplarInstaller.Module).ToDictionary(x => x.StableKey);
		Assert.AreEqual(3, claims.Values.Count(x => x.EntityType == nameof(ChargenRole)));
		Assert.AreEqual(32, claims.Values.Count(x => x.EntityType == nameof(MagicPower)));
		Assert.AreEqual("psi", db.MagicSchools.Single(x => x.Id == claims["arm.school.lirathan_templar"].LogicalId).SchoolVerb);
		foreach (var (key, name) in new[] { ("normal", "User of the Way"), ("psionicist", "Psionicist"), ("lirathan_templar", "Lirathan Templar") })
			Assert.AreEqual(name, db.MagicCapabilities.Single(x => x.Id == claims["arm.capability." + key + "_psionics"].LogicalId).Name);
		var contactId = claims["arm.power.lirathan.contact"].LogicalId!.Value;
		var contactHelp = db.MagicPowers.Single(x => x.Id == contactId);
		StringAssert.Contains(contactHelp.ShowHelp, "same shard");
		Assert.IsFalse(contactHelp.ShowHelp.Contains("{range}"));
		Assert.IsFalse(contactHelp.Blurb.Contains("same the same"));
		var normalMerit = claims[ArmageddonTemplarInstaller.NormalPsionicsMeritKey].LogicalId!.Value;
		var psionicistMerit = claims[ArmageddonTemplarInstaller.PsionicistMeritKey].LogicalId!.Value;
		foreach (var mainRole in ArmageddonMundaneProgression.Roles.Where(x => x.Primary))
		{
			var id = db.SeederManagedRecords.Single(x => x.StableKey == mainRole.Key).LogicalId!.Value;
			Assert.IsTrue(db.ChargenRolesMerits.Any(x => x.ChargenRoleId == id && x.MeritId == (mainRole.Name == "Psionicist" ? psionicistMerit : normalMerit)));
			Assert.IsFalse(db.ChargenRolesMerits.Any(x => x.ChargenRoleId == id && x.MeritId == (mainRole.Name == "Psionicist" ? normalMerit : psionicistMerit)));
		}
		var normalPowers = XElement.Parse(db.MagicCapabilities.Single(x => x.Id == claims["arm.capability.normal_psionics"].LogicalId).Definition).Elements("Power").ToArray();
		var psionicistPowers = XElement.Parse(db.MagicCapabilities.Single(x => x.Id == claims["arm.capability.psionicist_psionics"].LogicalId).Definition).Elements("Power").ToArray();
		var lirathanPowers = XElement.Parse(db.MagicCapabilities.Single(x => x.Id == claims["arm.capability.lirathan_templar_psionics"].LogicalId).Definition).Elements("Power").ToArray();
		Assert.AreEqual(5, normalPowers.Length);
		Assert.AreEqual(29, psionicistPowers.Length);
		Assert.AreEqual(22, lirathanPowers.Length);
		foreach (var common in normalPowers)
		{
			Assert.IsTrue(psionicistPowers.Any(x => XNode.DeepEquals(x, common)), "Psionicist must share normal powers and canonical traits.");
			Assert.IsTrue(lirathanPowers.Any(x => XNode.DeepEquals(x, common)), "Lirathan must share normal powers and canonical traits.");
		}
		var magicRules = XElement.Parse(db.Merits.Single(x => x.Id == claims[ArmageddonTemplarInstaller.MagicPsionicProgressionKey].LogicalId).Definition).Element("Rules")!.Elements("Rule").ToArray();
		Assert.AreEqual(30, magicRules.Length);
		foreach (var magicRole in magicRules.Select(x => (long)x.Attribute("role")!).Distinct())
		{
			Assert.IsTrue(db.ChargenRolesMerits.Any(x => x.ChargenRoleId == magicRole && x.MeritId == normalMerit));
			Assert.IsTrue(db.ChargenRolesMerits.Any(x => x.ChargenRoleId == magicRole && x.MeritId == claims[ArmageddonTemplarInstaller.MagicPsionicProgressionKey].LogicalId));
			var expel = magicRules.Single(x => (long)x.Attribute("role")! == magicRole && x.Attribute("parent") is not null);
			Assert.AreEqual(70.0, (double)expel.Attribute("threshold")!);
		}
		foreach (var wearable in db.GameItemComponentProtos.Where(x => x.Type == "Wearable").AsEnumerable())
		{
			var profiles = XElement.Parse(wearable.Definition).Element("Profiles")!;
			Assert.IsTrue(db.WearProfiles.Any(x => x.Id == long.Parse(profiles.Attribute("Default")!.Value)));
			foreach (var profile in profiles.Elements("Profile"))
				Assert.IsTrue(db.WearProfiles.Any(x => x.Id == long.Parse(profile.Value)));
		}
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == "arm.skill.analyze"), "Analyze must reuse the native capability binding.");
		var analyze = db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)MudSharp.RPG.Checks.CheckType.AnalyzeEligibilityCheck).TraitExpression.Expression;
		foreach (var id in claims.Values.Where(x => x.EntityType == nameof(ChargenRole)).Select(x => x.LogicalId!.Value)) StringAssert.Contains(analyze, id.ToString());
		Assert.IsFalse(db.FutureProgs.Any(x => x.FunctionName == "SouthernTemplarReserveCap"), "Templars use the common Mana capacity rather than a separate grade-based pool.");
		Assert.AreEqual(1, db.MagicResources.Count(x => (x.MagicResourceType & 1) != 0));
		var progs = db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray();
		using (var compiler = new OfflineProgCompilation(progs))
			foreach (var row in claims.Values.Where(x => x.EntityType == nameof(MudSharp.Models.FutureProg))) compiler.Compile(row.LogicalId!.Value);
		var world = new Mock<IFuturemud>();
		var runtimeRoles = new All<MudSharp.CharacterCreation.Roles.IChargenRole>();
		foreach (var row in db.ChargenRoles)
			runtimeRoles.Add(Mock.Of<MudSharp.CharacterCreation.Roles.IChargenRole>(x => x.Id == row.Id && x.Name == row.Name && x.RoleType == (MudSharp.CharacterCreation.Roles.ChargenRoleType)row.Type));
		world.SetupGet(x => x.Roles).Returns(runtimeRoles);
		var rules = XElement.Parse(db.Merits.Single(x => x.Id == claims[ArmageddonTemplarInstaller.ProgressionKey].LogicalId).Definition).Element("Rules")!.Elements("Rule");
		var languageIds = db.Languages.Select(x => x.LinkedTraitId).AsEnumerable().Concat(ArmageddonSkillPackage.Skills.Where(x => x.Language)
			.Select(x => db.SeederManagedRecords.Single(r => r.StableKey == x.Key).LogicalId!.Value)).ToHashSet();
		foreach (var rule in rules)
		{
			var trait = db.TraitDefinitions.Include(x => x.Expression).Single(x => x.Id == (long)rule.Attribute("trait")!);
			var actor = Mock.Of<MudSharp.Character.ICharacter>(x => x.Roles == new[] { runtimeRoles.Get((long)rule.Attribute("role")!) });
			var capExpression = new MudSharp.Body.Traits.TraitExpression(trait.Expression.Expression, world.Object);
			Assert.AreEqual(languageIds.Contains(trait.Id) ? 200 : (double)rule.Attribute("cap")!, capExpression.Evaluate(actor), 0.000001,
				"Native cap expression for " + trait.Name + " and role " + rule.Attribute("role")!.Value);
		}
		var traits = new All<MudSharp.Body.Traits.ITraitDefinition>();
		foreach (var row in db.TraitDefinitions) traits.Add(Mock.Of<MudSharp.Body.Traits.ITraitDefinition>(x => x.Id == row.Id && x.Name == row.Name));
		var runtimeProgs = new All<IFutureProg>();
		foreach (var row in progs) runtimeProgs.Add(Mock.Of<IFutureProg>(x => x.Id == row.Id && x.FunctionName == row.FunctionName));
		var schools = new All<IMagicSchool>();
		foreach (var row in db.MagicSchools) schools.Add(Mock.Of<IMagicSchool>(x => x.Id == row.Id && x.Name == row.Name && x.SchoolVerb == row.SchoolVerb));
		world.SetupGet(x => x.Traits).Returns(traits);
		world.SetupGet(x => x.FutureProgs).Returns(runtimeProgs);
		world.SetupGet(x => x.AlwaysTrueProg).Returns(runtimeProgs.Get(1));
		world.SetupGet(x => x.MagicSchools).Returns(schools);
		var loadedPowers = new All<IMagicPower>();
		world.SetupGet(x => x.MagicPowers).Returns(loadedPowers);
		var powerIds = claims.Values.Where(x => x.EntityType == nameof(MagicPower)).Select(x => x.LogicalId!.Value).ToArray();
		foreach (var row in db.MagicPowers.Where(x => powerIds.Contains(x.Id)))
			loadedPowers.Add(MagicPowerFactory.LoadPower(row, world.Object));
		Assert.AreEqual(32, loadedPowers.Count());
		foreach (var verb in new[] { "say", "connectback", "probe", "sensepresence", "vanish", "rejuvenate", "disorient", "project", "suggest", "compel", "conceal", "babble" })
			Assert.AreEqual(1, loadedPowers.Count(x => x.Verbs.Contains(verb)), "Unique shared Armageddon verb: " + verb);
		var compel = (PsionicControlPower)loadedPowers.Get(claims["arm.power.psionicist.compel"].LogicalId!.Value);
		var control = (PsionicControlPower)loadedPowers.Get(claims["arm.power.lirathan.control"].LogicalId!.Value);
		Assert.IsFalse(compel.RequireSameLocation);
		Assert.AreEqual(MagicPowerDistance.AnyConnectedMind, compel.PowerDistance);
		Assert.IsTrue(control.RequireSameLocation);
		foreach (var verb in new[] { "contact", "barrier", "expel", "allspeak", "hear", "thoughtsense", "empathy", "clairaudience", "trace", "magicksense", "locate", "mindblast", "mindwipe", "control", "masquerade", "illusion", "cathexis", "dome", "shadowwalk", "silentpsi" })
			Assert.AreEqual(1, loadedPowers.Count(x => x.Verbs.Contains(verb)), "Unique usable Lirathan verb: " + verb);
		var cap = db.MagicCapabilities.Single(x => x.Id == claims["arm.capability.southern_templar"].LogicalId);
		var casting = XElement.Parse(cap.Definition).Element("Casting")!;
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonProgressionInstaller.SourceResourceKey).LogicalId, (long?)casting.Attribute("source"));
		Assert.AreEqual((long?)casting.Attribute("source"), (long?)casting.Attribute("reserve"));
		var restorer = ConditionalMagicItemPolicy.Load(XElement.Parse(db.GameItemComponentProtos.Single(x => x.Id == claims["arm.templar.southern.regen_medallion.component"].LogicalId).Definition));
		Assert.AreEqual((long)casting.Attribute("reserve")!, restorer.ResourceId);
		Assert.IsFalse((bool)casting.Attribute("enabled")!);
		var shrine = db.GameItemComponentProtos.Single(x => x.Id == claims["arm.templar.southern.shrine.component"].LogicalId);
		var policy = ConditionalMagicItemPolicy.Load(XElement.Parse(shrine.Definition));
		Assert.AreEqual(12, policy.Choices.Length);
		Assert.IsTrue(policy.TerritoryProgId > 0);
		var permanent = System.Text.Json.JsonSerializer.Deserialize<PatronCastingPolicy>(casting.Element("Patron")!.Value)!;
		Assert.AreEqual(14, permanent.Incantations!.Count, "Every Southern admission needs a route-local fallback without modifying canonical spells.");
		Assert.IsFalse(policy.Choices.Any(x => permanent.PermanentSpells.Contains(x.SpellId)));
		var role = db.ChargenRoles.Single(x => x.Id == claims["arm.class.lirathan_templar"].LogicalId);
		role.Name = "Builder Lirathan";
		var pressure = db.TraitDefinitions.Include(x => x.Expression).Single(x => x.Id == claims["arm.skill.pressure_points"].LogicalId);
		pressure.Expression.Expression = "123";
		var removedMerit = db.ChargenRolesMerits.First(x => x.ChargenRoleId == role.Id);
		var removedMeritId = removedMerit.MeritId;
		db.ChargenRolesMerits.Remove(removedMerit);
		var gem = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Id == claims["arm.templar.southern.gem"].LogicalId);
		var bindingId = claims["arm.templar.southern.gem.component"].LogicalId;
		var pokeActionId = claims["arm.templar.jihaen.pressure_point.action"].LogicalId;
		var pokeAction = db.CombatActions.Single(x => x.Id == pokeActionId);
		var pokeXml = XElement.Parse(pokeAction.AdditionalInfo);
		var pokeEffect = pokeXml.Elements("Effect").Single(x => (string?)x.Attribute("type") == "pressurepoint");
		Assert.AreEqual(45.0, (double)pokeEffect.Attribute("stomachseconds")!);
		Assert.AreEqual(4.0, (double)pokeEffect.Attribute("nauseaintensity")!);
		foreach (var location in new[] { "leftarm", "rightarm", "leftleg", "rightleg" })
			Assert.AreEqual(5.0, (double)pokeEffect.Attribute(location + "seconds")!);
		pokeEffect.SetAttributeValue("stomachseconds", 61);
		pokeEffect.SetAttributeValue("leftarmseconds", 9);
		pokeEffect.Add(new XElement("Anatomy", new XElement("Location", new XAttribute("name", "leftarm"), new XAttribute("parts", "tentacle"))));
		pokeAction.AdditionalInfo = pokeXml.ToString();
		db.RacesCombatActions.Remove(db.RacesCombatActions.Single(x => x.CombatActionId == pokeActionId));
		db.GameItemProtosGameItemComponentProtos.Remove(gem.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemComponentProtoId == bindingId));
		shrine.Definition = (policy with { SelectionLimit = 1 }).Save().ToString();
		var customPower = db.MagicPowers.Single(x => x.Id == claims["arm.power.psionicist.probe"].LogicalId);
		customPower.ShowHelp = "Builder Probe help";
		var merchantKey = ArmageddonMundaneProgression.Roles.Single(r => r.Name == "Merchant" && r.Primary).Key;
		var merchantId = db.SeederManagedRecords.Single(x => x.StableKey == merchantKey).LogicalId!.Value;
		db.ChargenRolesMerits.Remove(db.ChargenRolesMerits.Single(x => x.ChargenRoleId == merchantId && x.MeritId == normalMerit));
		db.SaveChanges();
		ArmageddonMundaneProgression.Install(db, messages);
		ArmageddonTemplarInstaller.Install(db, messages);
		db.ChangeTracker.Clear();
		Assert.AreEqual("Builder Lirathan", db.ChargenRoles.Single(x => x.Id == role.Id).Name);
		Assert.AreEqual("Builder Probe help", db.MagicPowers.Single(x => x.Id == customPower.Id).ShowHelp);
		Assert.IsFalse(db.ChargenRolesMerits.Any(x => x.ChargenRoleId == merchantId && x.MeritId == normalMerit));
		foreach (var claim in claims.Values) Assert.AreEqual(claim.LogicalId, db.SeederManagedRecords.Single(x => x.StableKey == claim.StableKey).LogicalId);
		Assert.AreEqual("123", db.TraitDefinitions.Include(x => x.Expression).Single(x => x.Id == pressure.Id).Expression.Expression);
		Assert.IsFalse(db.ChargenRolesMerits.Any(x => x.ChargenRoleId == role.Id && x.MeritId == removedMeritId));
		Assert.IsFalse(db.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemProtoId == gem.Id && x.GameItemComponentProtoId == bindingId));
		Assert.IsFalse(db.RacesCombatActions.Any(x => x.CombatActionId == pokeActionId), "Rerun resurrected a builder-removed racial action.");
		Assert.AreEqual(pokeXml.ToString(), db.CombatActions.Single(x => x.Id == pokeActionId).AdditionalInfo, "Rerun replaced customized poke durations or anatomy.");
		Assert.AreEqual(1, ConditionalMagicItemPolicy.Load(XElement.Parse(db.GameItemComponentProtos.Single(x => x.Id == shrine.Id).Definition)).SelectionLimit);
		CollectionAssert.AreEquivalent(claims.Keys.ToArray(), db.SeederManagedRecords.Where(x => x.Module == ArmageddonTemplarInstaller.Module).Select(x => x.StableKey).ToArray());
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
	}
}
