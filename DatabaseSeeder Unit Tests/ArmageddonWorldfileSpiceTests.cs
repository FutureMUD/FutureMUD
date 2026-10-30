#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Construction;
using DatabaseSeeder.Seeders;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Emotions;
using MudSharp.Planes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;
using NativeProg = MudSharp.FutureProg.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileSpiceTests
{
	[TestMethod]
	public void WritingImplements_MapSourcePensAndPersistOpeningStockWithoutInventingInkCharges()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, archive);
		if (!db.Colours.Any(x => x.Name == "black")) db.Colours.Add(new Db.Colour { Id = 9000, Name = "black" });
		var pens = archive.Items.Where(x => x.ItemType == 21).ToArray();
		Assert.AreEqual(7, pens.Length);
		foreach (var source in pens)
		{
			var proto = new Db.GameItemProto { Id = source.Vnum, Name = source.ShortDescription };
			db.GameItemProtos.Add(proto);
			Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(source.Vnum, proto);
			Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(proto.Id, (source, source.Values));
			db.GameItems.Add(new Db.GameItem { Id = proto.Id, GameItemProtoId = proto.Id, EffectData = "<Effects/>" });
		}
		var variantSource = pens.Single(x => x.Vnum == 1506);
		var variant = new Db.GameItemProto { Id = 990000, Name = "Reset quill" };
		db.GameItemProtos.Add(variant);
		Field<Dictionary<string, Db.GameItemProto>>(writer, "_resetItemVariants").Add("fixture-quill", variant);
		Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(variant.Id, (variantSource, variantSource.Values));
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallWritingImplements", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		foreach (var proto in db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos))
		{
			var link = proto.GameItemProtosGameItemComponentProtos.Single();
			var component = db.GameItemComponentProtos.Single(x => x.Id == link.GameItemComponentProtoId);
			Assert.AreEqual("ScribingImplement", component.Type);
			var definition = XElement.Parse(component.Definition);
			Assert.AreEqual(proto.Id is 1595 or 20039 ? "Stylus" : "Quill", definition.Element("ImplementType")!.Value);
			Assert.AreEqual(0, (int)definition.Element("TotalUses")!);
			Assert.AreEqual(db.Colours.Single(x => x.Name == "black").Id, (long)definition.Element("Colour")!);
		}
		Assert.AreEqual(7, db.GameItemComponents.Count());
		Assert.IsTrue(db.GameItemComponents.AsEnumerable().All(x => (int)XElement.Parse(x.Definition).Element("RemainingUses")! == 0));
	}

	[TestMethod]
	public void HealingFruit_UsesInstalledHealingAndEligibilityWithoutManaOrPartialMixedPotions()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, archive);
		var filter = new Db.FutureProg { Id = 990010, FunctionName = "FruitEligibilityFixture", FunctionText = "return true",
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() };
		filter.FutureProgsParameters.Add(new Db.FutureProgsParameter { ParameterIndex = 0, ParameterName = "target", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		filter.FutureProgsParameters.Add(new Db.FutureProgsParameter { ParameterIndex = 1, ParameterName = "caster", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.Add(filter);
		var spell = new Db.MagicSpell { Id = 990011, Name = "Mend Flesh", MagicSchoolId = 1, TargetEmote = "",
			Definition = ArmageddonReviewedUtilityContent.MendFlesh().BuildDefinition(1, 2, filter.Id).ToString() };
		db.MagicSpells.Add(spell);
		db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "Fixture", StableKey = ArmageddonReviewedUtilityContent.MendFleshKey,
			EntityType = nameof(Db.MagicSpell), LogicalId = spell.Id });
		foreach (var source in archive.Items.Where(x => x.Vnum is 20070 or 37404))
		{
			var proto = new Db.GameItemProto { Id = source.Vnum, Name = source.ShortDescription };
			db.GameItemProtos.Add(proto);
			Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(source.Vnum, proto);
			Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(proto.Id, (source, source.Values));
			db.GameItems.Add(new Db.GameItem { Id = proto.Id, GameItemProtoId = proto.Id, EffectData = "<Effects/>" });
		}
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallHealingPotions", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var payload = XElement.Parse(db.MagicSpells.Single(x => x.Id != spell.Id).Definition);
		Assert.AreEqual("2*2*2", payload.Element("Effects")!.Element("Effect")!.Element("HealingAmount")!.Value);
		Assert.IsFalse(payload.Element("Costs")!.Elements().Any());
		Assert.AreEqual("substancecharacter", (string?)payload.Element("Trigger")!.Attribute("type"));
		var substance = XElement.Parse(db.MagicalSubstances.Single().Definition);
		Assert.IsFalse(substance.Elements("Binding").Any(), "Only the eligibility-checked on-swallow prog delivers this preparation.");
		Assert.AreEqual(1, db.GameItemComponents.Count(x => x.GameItemId == 20070));
		Assert.AreEqual(0, db.GameItemComponents.Count(x => x.GameItemId == 37404), "Mixed potions cannot silently lose an unmapped payload.");
		var consume = db.FutureProgs.Single(x => x.FunctionName.StartsWith("ArmageddonWorldfilePotionConsume"));
		StringAssert.Contains(consume.FunctionText, "@FruitEligibilityFixture(@actor, @actor)");
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfilePotion"))) compiler.Compile(prog.Id);
	}

	[TestMethod]
	public void AuthoredNotes_PreserveLanguagesAndUseOneScriptInPrototypesAndOpeningStock()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, archive);
		foreach (var name in new[] { "Sirihish", "Tatlum", "Cavilish" })
			if (!db.Languages.Any(x => x.Name == name)) db.Languages.Add(new Db.Language { Id = 9000 + db.Languages.Local.Count, Name = name });
		if (!db.Colours.Any(x => x.Name == "black")) db.Colours.Add(new Db.Colour { Id = 9000, Name = "black" });
		foreach (var source in archive.Items.Where(x => x.Vnum is 1503 or 3427 or 25507 or 64160))
		{
			var proto = new Db.GameItemProto { Id = 100_000 + source.Vnum, Name = source.ShortDescription };
			db.GameItemProtos.Add(proto);
			Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(source.Vnum, proto);
			Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(proto.Id, (source, source.Values));
			if (source.Vnum == 1503) continue;
			var keywordComponent = new Db.GameItemComponentProto { Id = proto.Id, Type = "Keyword Descriptions",
				Definition = new XElement("Definition", new XElement("KeywordDescriptions", source.ExtraDescriptions.Select(x =>
					new XElement("Entry", new XElement("Keyword", x.Keyword), new XElement("Prose", x.Description))))).ToString() };
			db.GameItemComponentProtos.Add(keywordComponent);
			proto.GameItemProtosGameItemComponentProtos.Add(new Db.GameItemProtosGameItemComponentProtos { GameItemProtoId = proto.Id, GameItemComponentProtoId = keywordComponent.Id });
			db.GameItems.Add(new Db.GameItem { Id = proto.Id, GameItemProtoId = proto.Id, EffectData = "<Effects/>" });
		}
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallNoteComponents", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var script = db.Scripts.Single(x => x.Name == "Zalanthan");
		Assert.AreEqual(11, db.Writings.Count());
		Assert.IsTrue(db.Writings.All(x => x.ScriptId == script.Id));
		CollectionAssert.AreEquivalent(new[] { "Sirihish", "Tatlum", "Cavilish" }, db.Writings.Select(x => x.Language.Name).Distinct().ToArray());
		Assert.AreEqual(3, db.ScriptsDesignedLanguages.Count(x => x.ScriptId == script.Id));
		foreach (var stock in db.GameItems.Include(x => x.GameItemComponents))
		{
			var component = stock.GameItemComponents.Single();
			var state = XElement.Parse(component.Definition);
			var definition = XElement.Parse(db.GameItemComponentProtos.Single(x => x.Id == component.GameItemComponentProtoId).Definition);
			CollectionAssert.AreEqual(definition.Element("InitialReadables")!.Elements().Select(x => (long)x.Attribute("Id")!).ToArray(),
				state.Element("Writings")!.Elements().Select(x => (long)x.Attribute("Id")!).ToArray());
		}
		Assert.IsFalse(db.GameItemComponentProtos.Where(x => x.Type == "Keyword Descriptions").AsEnumerable()
			.Any(x => XElement.Parse(x.Definition).Descendants("Entry").Any()), "Page keywords cannot bypass native reading checks.");
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		compiler.Compile(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileWritingKnowledge").Id);
	}

	private static readonly string[] SpiceNames = ["Methelinoc", "Melem Tuek", "Krelez", "Kemen", "Aggross", "Zharal", "Thodeliv", "Krentakh", "Qel"];
	[TestMethod]
	public void SpiceConsumption_InstallsAllSourcePrototypesAndCompilesConsumptionPrograms()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var spices = archive.Items.Where(x => x.ItemType == 25).ToArray();
		Assert.AreEqual(90, spices.Length);

		db.Materials.Add(new Db.Material { Id = 100, Name = "feces", Density = 1000 });
		db.SaveChanges();
		var writer = new ArmageddonWorldfileWriter(db, new ArmageddonWorldfilePackage { Items = spices });
		var protos = new Dictionary<int, Db.GameItemProto>();
		var effective = new Dictionary<long, (ArmWorldItem Source, int[] Values)>();
		foreach (var spice in spices.Concat(archive.Items.Where(x => x.ItemType is 17 or 19 or 24 or 27 or 33 or 34 || x.Vnum is 13248 or 37099 or 1503 or 1504 or 14004 or 14006 or 21998))
			.Concat(archive.Items.Where(x => x.ItemType == 5).Take(3))
			.Concat(archive.Items.Where(x => x.ItemType == 5 && (x.Flags[1] & (1 << 20)) != 0).Take(2)).DistinctBy(x => x.Vnum))
		{
			var proto = new Db.GameItemProto { Id = 100_000 + spice.Vnum, Name = spice.ShortDescription };
			protos.Add(spice.Vnum, proto);
			effective.Add(proto.Id, (spice, spice.Values));
		}
		foreach (var pinch in archive.Items.Where(x => x.Vnum is >= 20341 and <= 20349))
			if (!protos.ContainsKey(pinch.Vnum)) protos.Add(pinch.Vnum, new Db.GameItemProto { Id = 200_000 + pinch.Vnum, Name = pinch.ShortDescription });
		Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Clear();
		foreach (var pair in protos) Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(pair.Key, pair.Value);
		Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Clear();
		foreach (var pair in effective) Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(pair.Key, pair.Value);
		db.GameItemProtos.AddRange(protos.Values);
		AddAttribute(db, 10, "Strength");
		AddAttribute(db, 11, "Agility");
		AddAttribute(db, 12, "Endurance");
		db.StackDecorators.Add(new Db.StackDecorator { Id = 1, Name = "Bites", Type = "Bites", Definition = "<Definition/>" });
		db.Races.Add(new Db.Race { Id = 20, Name = "Mul", BaseBodyId = 1, DefaultHealthStrategyId = 1 });
		db.Clocks.Add(new Db.Clock { Id = 20, PrimaryTimezoneId = 20,
			Definition = "<Clock><InGameSecondsPerRealSecond>6</InGameSecondsPerRealSecond><SecondsPerMinute>60</SecondsPerMinute></Clock>" });
		db.Timezones.Add(new Db.Timezone { Id = 20, ClockId = 20, Name = "Test", Description = "Test" });
		db.Calendars.Single().FeedClockId = 20;
		db.VariableDefinitions.Add(new Db.VariableDefinition
		{
			OwnerTypeDefinition = ProgVariableTypes.Character.ToStorageString(),
			Property = ArmageddonGatheringRegisters.Ecology,
			ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString()
		});
		db.SeederManagedRecords.Add(new Db.SeederManagedRecord
		{
			Seeder = "Fixture", StableKey = ArmageddonProgressionInstaller.ManaResourceKey,
			EntityType = nameof(Db.MagicResource), LogicalId = 13
		});
		var openingSpice = spices.First(x => x.Values[2] == 1 && x.Values[0] > 0 && x.Values[1] is >= 141 and <= 149);
		const long openingItemId = 900_001;
		db.GameItems.Add(new Db.GameItem
		{
			Id = openingItemId, GameItemProtoId = protos[openingSpice.Vnum].Id, GameItemProtoRevision = 0,
			Quality = 0, Condition = 1.0, RoomLayer = 0, MaterialId = 1, Size = 0, OwnerType = "Shop",
			EffectData = "<Effects/>"
		});
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallWillpowerSkill", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(writer, null);

		typeof(ArmageddonWorldfileWriter).GetMethod("InstallLegacyPoisonEffects", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(writer, null);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallPoisonDelivery", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(writer, null);
		const long cureId = 990_001;
		var cureDrug = db.Drugs.Local.Single(x => x.Name == "Generic Poison");
		db.MagicSpells.Add(new Db.MagicSpell { Id = cureId, Name = "Purge Toxin", Definition =
			ArmageddonToxinSpellContent.Create(cureDrug.Id).Single(x => x.Content.Key == ArmageddonToxinSpellContent.PurgeToxinKey).Content.BuildDefinition(10, 1, 0).ToString() });
		db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "Fixture", StableKey = "arm.spell.purge_toxin", EntityType = nameof(Db.MagicSpell), LogicalId = cureId });
		db.SaveChanges();
		var kelwinCure = new Db.MagicSpell { Id = cureId - 1, Name = "Cleanse Venom", Definition = db.MagicSpells.Local.Single(x => x.Id == cureId).Definition };
		db.MagicSpells.Add(kelwinCure);
		db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "Fixture", StableKey = "npc-service.kelwin.cure", EntityType = nameof(Db.MagicSpell), LogicalId = kelwinCure.Id });
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallPoisonCleansing", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		var leaf = db.FutureProgs.Local.Single(x => x.FunctionName == "ArmageddonWorldfileCurativeLeaf" + protos[37099].Id);
		Assert.IsTrue(protos[37099].GameItemProtosOnLoadProgs.Any(), "The retained leaf gains its attached cure without adding a reset.");
		var cureXml = XElement.Parse(kelwinCure.Definition).Element("Effects")!.Elements().Single();
		Assert.IsTrue((bool)cureXml.Element("FirstMatchingGroup")!);
		var eligible = cureXml.Element("DrugGroups")!.Descendants("Drug").Select(x => (long)x).ToHashSet();
		Assert.IsTrue(eligible.Contains(db.Drugs.Local.Single(x => x.Name == "Skellebain (third stage)").Id));
		Assert.IsFalse(eligible.Contains(db.Drugs.Local.Single(x => x.Name == "Methelinoc Toxin").Id));
		Assert.IsFalse(eligible.Contains(db.Drugs.Local.Single(x => x.Name == "Plague Toxin").Id));
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallNoteComponents", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		Assert.IsTrue(db.GameItemComponentProtos.Local.Any(x => x.Type == "Book" && (int)XElement.Parse(x.Definition).Element("PageCount")! == 2));
		Assert.IsTrue(db.GameItemComponentProtos.Local.Any(x => x.Type == "PaperSheet"));
		foreach (var writtenNote in new[] { 14004, 14006, 21998 })
			Assert.IsFalse(protos[writtenNote].GameItemProtosGameItemComponentProtos.Any(), "Unmapped authored notes must not gain consumable blank pages.");
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallSpiceConsumption", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(writer, null);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallMulRage", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(writer, null);
		db.SaveChanges();

		var programs = db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileSpice", StringComparison.Ordinal)).ToArray();
		Assert.AreEqual(90, programs.Count(x => x.FunctionName.StartsWith("ArmageddonWorldfileSpiceStock", StringComparison.Ordinal)),
			"Each source spice receives a stock initialiser, even when its legacy effect is invalid.");
		Assert.IsTrue(programs.Any(x => x.FunctionText.Contains("dosedrug(", StringComparison.Ordinal)));
		Assert.IsTrue(programs.Any(x => x.FunctionText.Contains("setregister @item", StringComparison.Ordinal)));
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var program in programs) compiler.Compile(program.Id);
		compiler.Compile(leaf.Id);
		AssertCurativeLeafTreatment(leaf, cureDrug);
		foreach (var program in db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfilePoisonDose", StringComparison.Ordinal)))
			compiler.Compile(program.Id);
		foreach (var program in db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileProportionalPoison", StringComparison.Ordinal) ||
			x.FunctionName.StartsWith("ArmageddonWorldfileCleansePoisonedItem", StringComparison.Ordinal) ||
			x.FunctionName.StartsWith("ArmageddonWorldfileWeaponPoison", StringComparison.Ordinal) ||
			x.FunctionName.StartsWith("ArmageddonWorldfileInitialWeaponPoison", StringComparison.Ordinal) ||
			x.FunctionName.StartsWith("ArmageddonWorldfilePoisonVial", StringComparison.Ordinal) ||
			x.FunctionName.StartsWith("ArmageddonWorldfilePoisonDraught", StringComparison.Ordinal) ||
			x.FunctionName.StartsWith("ArmageddonWorldfileAntidote", StringComparison.Ordinal) || x.FunctionName == "ArmageddonWorldfileFoodPoison"))
			compiler.Compile(program.Id);
		foreach (var program in db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileMulRage", StringComparison.Ordinal)))
			compiler.Compile(program.Id);
		Assert.AreEqual(30, db.ProgSchedules.Single(x => x.Name == "Armageddon Mul Rage and innate Willpower").IntervalModifier);
		var rageMerit = db.Merits.Single(x => x.Type == "IgnoreHitPointUnconsciousness");
		Assert.AreEqual("Mul Rage", rageMerit.Name);
		Assert.IsTrue(db.RacesMerits.Any(x => x.RaceId == 20 && x.MeritId == rageMerit.Id));
		var applicability = (long)XElement.Parse(rageMerit.Definition).Element("ApplicabilityProg")!;
		Assert.AreEqual("return hascombatfrenzyeffect(@actor)", db.FutureProgs.Single(x => x.Id == applicability).FunctionText);

		var willpower = db.TraitDefinitions.Single(x => x.Type == 0 && x.Name == "Willpower");
		Assert.IsTrue(willpower.Hidden);
		Assert.AreEqual("100", db.TraitExpressions.Single(x => x.Id == willpower.ExpressionId).Expression);
		var openingItem = db.GameItems.Single(x => x.Id == openingItemId);
		Assert.AreEqual(5, db.VariableValues.Count(x => x.ReferenceId == openingItemId), "Opening stock receives spice state and poison-delivery registers directly.");
		Assert.AreEqual(2, db.HooksPerceivables.Count(x => x.GameItemId == openingItemId), "Persisted opening stock receives both smoke and sale hooks directly.");
		AssertSellPolicyExecutes(programs.Single(x => x.FunctionName == "ArmageddonWorldfileSpiceSell" + protos[openingSpice.Vnum].Id), openingSpice.Values[0]);
		var rolledSpice = spices.First(x => x.Values[2] == 1 && x.Values[0] > 0 && x.Values[1] is >= 141 and <= 149);
		AssertSmokeConsumesOneMgAndDeletesTheLastCharge(programs.Single(x => x.FunctionName == "ArmageddonWorldfileSpiceConsume" + protos[rolledSpice.Vnum].Id),
			db.Drugs.Single(x => x.Name == SpiceNames[rolledSpice.Values[1] - 141]));
		var looseSpice = spices.First(x => x.Values[2] == 0 && x.Values[0] >= 5 && x.Values[1] is >= 141 and <= 149);
		var pinchProto = protos[20200 + looseSpice.Values[1]];
		var shaveProg = programs.Single(x => x.FunctionName == "ArmageddonWorldfileSpiceShave" + protos[looseSpice.Vnum].Id);
		AssertShavingConservesUnitsAndKeepsSourceOnFailedGet(shaveProg, protos[looseSpice.Vnum], pinchProto);
		var rollProg = programs.Single(x => x.FunctionName == "ArmageddonWorldfileSpiceRoll" + protos[looseSpice.Vnum].Id);
		AssertRollingPreservesDoseAndRequiresPaper(rollProg, protos[64150 + looseSpice.Values[1]].Id);

		Assert.AreEqual(21, db.Drugs.Local.Count, "Nine source spices, Qel's variant, ten toxin exposures and a separate plague symptom profile are seeded.");
		Assert.AreEqual(10, db.SeederManagedRecords.Local.Count(x => x.StableKey.StartsWith("spice.drug.", StringComparison.Ordinal)));
		Assert.AreEqual(10, db.SeederManagedRecords.Local.Count(x => x.StableKey.StartsWith("poison.drug.", StringComparison.Ordinal)));
		var plague = db.Drugs.Local.Single(x => x.Name == "Plague Toxin");
		var infection = DrugInfectionAdditionalInfo.Parse(plague.DrugsIntensities.Single().AdditionalEffects);
		Assert.AreEqual((int)DrugType.Infection, plague.DrugsIntensities.Single().DrugType);
		Assert.AreEqual(InfectionRecoveryMode.FixedDuration, infection.RecoveryMode);
		Assert.AreEqual(30000, infection.DurationSeconds);
		Assert.IsTrue(infection.CureWithDrug);
		Assert.IsTrue(infection.SpreadOnSymptomEpisode);
		Assert.AreEqual(0.0001, infection.SpreadChance);
		var plagueSymptoms = db.Drugs.Local.Single(x => x.Id == infection.SymptomDrugId);
		Assert.AreEqual("Plague Symptoms", plagueSymptoms.Name);
		Assert.IsFalse(plagueSymptoms.DrugsIntensities.Any(x => x.DrugType == (int)DrugType.Infection));
		var expiry = DrugSymptomsAdditionalInfo.Parse(plagueSymptoms.DrugsIntensities.Single(x => x.DrugType == (int)DrugType.Symptoms).AdditionalEffects);
		Assert.AreEqual(-1, expiry.ExpiryTraitChanges.Single().Amount);
		var qel = db.Drugs.Local.Single(x => x.Name == "Qel");
		Assert.IsFalse(qel.DrugsIntensities.Any(x => x.DrugType == (int)DrugType.Dependence || x.DrugType == (int)DrugType.Overdose));
		var dependence = db.Drugs.Local.Single(x => x.Name == "Melem Tuek").DrugsIntensities.Single(x => x.DrugType == (int)DrugType.Dependence);
		StringAssert.Contains(dependence.AdditionalEffects!, "|2 0.5 0.5|");
		Assert.IsTrue(db.FutureProgs.Any(x => x.FunctionText.Contains("0.001", StringComparison.Ordinal)), "A rolled puff doses one authored potency unit (1 mg). ");
	}

	[TestMethod]
	public void SpiceDrugFactory_UsesConfiguredDependenceOverdoseAndEffectModifiers()
	{
		var drug = ArmageddonWorldfileWriter.CreateSpiceDrug(144, 1, 2, 3, 4, 5, false);
		var dependence = drug.DrugsIntensities.Single(x => x.DrugType == (int)DrugType.Dependence).AdditionalEffects!;
		StringAssert.StartsWith(dependence, "10 16 75 1 75 16 ");
		StringAssert.Contains(dependence, "|2 0.5 0.5|");
		var overdose = drug.DrugsIntensities.Single(x => x.DrugType == (int)DrugType.Overdose);
		StringAssert.Contains(overdose.AdditionalEffects!, "4 0.002|Armageddon Spice");
		var modifier = drug.DrugsIntensities.Single(x => x.DrugType == (int)DrugType.Modifier);
		StringAssert.Contains(modifier.AdditionalEffects!, "0.25 0 0 1 0 0 0 0");
	}

	[TestMethod]
	public void MulRageSpellMapping_AugmentsActualCanonicalStockWithoutChangingOrdinaryFury()
	{
		var fury = ArmageddonExistingFactorySpellContent.RousedFury(new ArmageddonFuryBindings(12, 1))
			.BuildDefinition(13, 21, 22);
		var originalLifetime = new XElement(fury.Descendants("Lifetime").Single());
		var configured = XElement.Parse(ArmageddonWorldfileWriter.ConfigureMulRageSpellDefinition(fury.ToString(), true, 20, 300));
		var profile = EmotionalStockProfile.Read(configured.Descendants("SourceProfile").Single());
		Assert.IsTrue(profile.CanonicalGradePolicy);
		Assert.AreEqual(12L, profile.TraitId);
		Assert.AreEqual(60, profile.UnitSeconds);
		Assert.AreEqual(7, profile.CapUnits);
		Assert.IsTrue(XNode.DeepEquals(originalLifetime, configured.Descendants("Lifetime").Single()));
		Assert.AreEqual(20L, profile.FrenzyOnFury!.RaceId);
		Assert.AreEqual(300L, profile.FrenzyOnFury.SuppressionDrugId);
		Assert.AreEqual(15.0, profile.FrenzyOnFury.OffenceBonus);
		Assert.AreEqual(5.0, profile.FrenzyOnFury.MeleeDamageBonus);
		var calm = ArmageddonExistingFactorySpellContent.StillAnger(new ArmageddonCalmBindings(12, 1,
			Enumerable.Range(1, 7).ToDictionary(x => x, _ => Difficulty.Normal))).BuildDefinition(13, 21, 22);
		var calmXml = XElement.Parse(ArmageddonWorldfileWriter.ConfigureMulRageSpellDefinition(calm.ToString(), false, 20, 300));
		var calmProfile = EmotionalStockProfile.Read(calmXml.Descendants("SourceProfile").Single());
		Assert.IsTrue(calmProfile.ClearCombatFrenzyOnCalm);
		Assert.IsTrue(calmProfile.CanonicalGradePolicy);
		Assert.AreEqual(7, calmProfile.Saves.Count);
	}

	[TestMethod]
	[DataRow(0, 0)]
	[DataRow(15, 38)]
	[DataRow(24, 92)]
	[DataRow(99, 92)]
	[DataRow(100, 100)]
	public void WillpowerNpcFloor_UsesSourceWisdomLearningTable(int wisdom, int expected) =>
		Assert.AreEqual(expected, ArmageddonWorldfileWriter.SourceWisdomLearningValue(wisdom));

	private static void AddAttribute(FuturemudDatabaseContext db, long id, string name) =>
	db.TraitDefinitions.Add(new Db.TraitDefinition { Id = id, Name = name, Type = 1 });

	private static void AssertRollingPreservesDoseAndRequiresPaper(Db.FutureProg definition, long rollProto)
	{
		foreach (var acceptsPage in new[] { false, true })
		{
			var fixture = Runtime(definition, new Db.Drug { Id = 1, Name = "Spice" }, 3, rollProto);
			var paper = RuntimeItem(1003, "paper");
			paper.Setup(x => x.HasKeyword("paper", fixture.Actor.Object, It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
			var page = new Mock<IConsumablePaper>();
			page.Setup(x => x.CanConsumePage(fixture.Actor.Object, false)).Returns(acceptsPage);
			page.Setup(x => x.ConsumePage(fixture.Actor.Object, false)).Returns(acceptsPage);
			paper.Setup(x => x.GetItemType<IConsumablePaper>()).Returns(page.Object);
			var items = new[] { fixture.Source.Object, paper.Object };
			fixture.Actor.Setup(x => x.GetProperty("helditems")).Returns(new CollectionVariable(items, ProgVariableTypes.Item));
			fixture.Body.SetupGet(x => x.ExternalItems).Returns(items);
			var spiceConsumed = false;
			fixture.Source.Setup(x => x.Delete()).Callback(() => spiceConsumed = true);
			fixture.Body.Setup(x => x.CanGet(fixture.Pinch.Object, 0)).Returns(() => spiceConsumed);
			Assert.IsTrue(fixture.Prog.ExecuteBool(fixture.Actor.Object, fixture.Source.Object, "make", new[] { "smoke", "paper", "spice" }));
			fixture.Source.Verify(x => x.Delete(), acceptsPage ? Times.Once() : Times.Never());
			page.Verify(x => x.ConsumePage(fixture.Actor.Object, false), acceptsPage ? Times.Once() : Times.Never());
			if (acceptsPage)
				fixture.Registers.Verify(x => x.SetValue(fixture.Pinch.Object, ArmageddonWorldfileWriter.SpiceUnitsRegister,
					It.Is<IProgVariable>(value => Equals(value.GetObject, 3m))), Times.Once);
		}
	}

	private static void AssertCurativeLeafTreatment(Db.FutureProg definition, Db.Drug drug)
	{
		var fixture = Runtime(definition, drug);
		fixture.Actor.Setup(x => x.GetProperty("inventory"))
			.Returns(new CollectionVariable(new[] { fixture.Source.Object }, ProgVariableTypes.Item));
		Assert.IsFalse(fixture.Prog.ExecuteBool(fixture.Actor.Object, fixture.Source.Object, "apply", Array.Empty<string>()));
		Assert.IsFalse(fixture.Prog.ExecuteBool(fixture.Actor.Object, fixture.Source.Object, "apply", new[] { "other", "self" }));
		Assert.IsTrue(fixture.Prog.ExecuteBool(fixture.Actor.Object, fixture.Source.Object, "apply", new[] { "spice" }));
		Assert.IsTrue(fixture.Prog.ExecuteBool(fixture.Actor.Object, fixture.Source.Object, "apply", new[] { "spice", "missing" }));
		fixture.Source.Verify(x => x.Delete(), Times.Never);
		fixture.Actor.Setup(x => x.TargetActor("self")).Returns(fixture.Actor.Object);
		Predicate<DrugDosage>? removed = null;
		fixture.Body.Setup(x => x.RemoveDrugDosages(It.IsAny<Predicate<DrugDosage>>()))
			.Callback<Predicate<DrugDosage>>(predicate => removed = predicate);
		Assert.IsTrue(fixture.Prog.ExecuteBool(fixture.Actor.Object, fixture.Source.Object, "apply", new[] { "spice", "self" }));
		fixture.Source.Verify(x => x.Delete(), Times.Once, "Even an unaffected patient consumes the source leaf.");
		Assert.IsNotNull(removed);
		Assert.IsTrue(removed(new DrugDosage { Drug = fixture.Drug.Object, SpellOriginId = Guid.NewGuid(), SpellOriginFamily = "arm.effect.poison" }));
		Assert.IsFalse(removed(new DrugDosage { Drug = fixture.Drug.Object }));
		Assert.IsFalse(removed(new DrugDosage { Drug = fixture.Drug.Object, SpellOriginId = Guid.NewGuid(), SpellOriginFamily = "unrelated" }));
	}

	private static void AssertSellPolicyExecutes(Db.FutureProg definition, int initialUnits)
	{
		NativeProg.Initialise();
		var world = new Mock<IFuturemud>();
		var registers = new Mock<IVariableRegister>();
		registers.Setup(x => x.GetType(ProgVariableTypes.Item, ArmageddonWorldfileWriter.SpiceUnitsRegister)).Returns(ProgVariableTypes.Number);
		registers.Setup(x => x.GetType(ProgVariableTypes.Character, ArmageddonGatheringRegisters.Ecology)).Returns(ProgVariableTypes.Number);
		var held = new List<IGameItem>();
		var actor = new Mock<ICharacter>();
		actor.As<IProgVariable>().SetupGet(x => x.GetObject).Returns(actor.Object);
		actor.As<IProgVariable>().SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		actor.As<IProgVariable>().Setup(x => x.GetProperty("id")).Returns(new NumberVariable(1));
		actor.Setup(x => x.GetProperty("helditems")).Returns(() => new CollectionVariable(held, ProgVariableTypes.Item));
		actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>())).Returns(true);
		actor.Setup(x => x.HasDubFor(It.IsAny<IPerceivable>(), It.IsAny<string>())).Returns(false);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		var fresh = RuntimeItem(1001, "fresh");
		var used = RuntimeItem(1002, "used");
		held.Add(fresh.Object);
		held.Add(used.Object);
		registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), ArmageddonWorldfileWriter.SpiceUnitsRegister))
			.Returns((IProgVariable item, string _) => new NumberVariable(ReferenceEquals(item.GetObject, used.Object) ? initialUnits - 1 : initialUnits));
		world.SetupGet(x => x.VariableRegister).Returns(registers.Object);
		var progs = new All<IFutureProg>();
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		var native = (IFutureProg)typeof(NativeProg).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
			[typeof(Db.FutureProg), typeof(IFuturemud)], null)!.Invoke([definition, world.Object]);
		progs.Add(native);
		Assert.IsTrue(native.Compile(), native.FunctionName + ": " + native.CompileError);
		Assert.IsFalse(native.ExecuteBool(actor.Object, fresh.Object, "sell", new[] { "fresh" }));
		Assert.IsTrue(native.ExecuteBool(actor.Object, used.Object, "sell", new[] { "used" }));
	}

	private static void AssertSmokeConsumesOneMgAndDeletesTheLastCharge(Db.FutureProg definition, Db.Drug definitionDrug)
	{
		var fixture = Runtime(definition, definitionDrug);
		Assert.IsTrue(fixture.Prog.ExecuteBool(fixture.Actor.Object, fixture.Source.Object, "smoke", new[] { "spice" }));
		fixture.Body.Verify(x => x.Dose(fixture.Drug.Object, DrugVector.Inhaled, 0.001), Times.Once);
		fixture.Registers.Verify(x => x.SetValue(fixture.Source.Object, ArmageddonWorldfileWriter.SpiceUnitsRegister,
			It.Is<IProgVariable>(v => Equals(v.GetObject, 0m))), Times.Once);
		fixture.Source.Verify(x => x.Delete(), Times.Once);
	}

	private static void AssertShavingConservesUnitsAndKeepsSourceOnFailedGet(Db.FutureProg definition,
		Db.GameItemProto sourceProto, Db.GameItemProto pinchProto)
	{
		var success = ShavingRuntime(definition, sourceProto.Id, pinchProto.Id, canGet: true);
		Assert.IsTrue(success.Prog.ExecuteBool(success.Actor.Object, success.Source.Object, "shave", new[] { "brick" }));
		success.Registers.Verify(x => x.SetValue(success.Source.Object, ArmageddonWorldfileWriter.SpiceUnitsRegister,
			It.Is<IProgVariable>(v => Equals(v.GetObject, 2m))), Times.Once);
		success.Body.Verify(x => x.Get(success.Pinch.Object, 0, null, true), Times.Once);
		success.Source.Verify(x => x.Delete(), Times.Never);

		var failure = ShavingRuntime(definition, sourceProto.Id, pinchProto.Id, canGet: false);
		Assert.IsTrue(failure.Prog.ExecuteBool(failure.Actor.Object, failure.Source.Object, "shave", new[] { "brick" }));
		failure.Registers.Verify(x => x.SetValue(failure.Source.Object, ArmageddonWorldfileWriter.SpiceUnitsRegister, It.IsAny<IProgVariable>()), Times.Never);
		failure.Pinch.Verify(x => x.Delete(), Times.Once);
		failure.Source.Verify(x => x.Delete(), Times.Never);
	}

	private static (IFutureProg Prog, Mock<IGameItem> Source, Mock<IGameItem> Pinch, Mock<ICharacter> Actor,
		Mock<IBody> Body, Mock<IDrug> Drug, Mock<IVariableRegister> Registers) ShavingRuntime(Db.FutureProg definition,
		long sourceProto, long pinchProto, bool canGet)
	{
		var fakeDrug = new Db.Drug { Id = 1, Name = "Spice" };
		var fixture = Runtime(definition, fakeDrug, initialUnits: 5, pinchPrototypeId: pinchProto);
		fixture.Body.Setup(x => x.CanGet(fixture.Pinch.Object, 0)).Returns(canGet);
		fixture.Body.Setup(x => x.Get(fixture.Pinch.Object, 0, null, true));
		fixture.Source.Setup(x => x.GetProperty("proto")).Returns(new NumberVariable(sourceProto));
		fixture.Pinch.Setup(x => x.GetProperty("proto")).Returns(new NumberVariable(pinchProto));
		return fixture;
	}

	private static (IFutureProg Prog, Mock<IGameItem> Source, Mock<IGameItem> Pinch, Mock<ICharacter> Actor,
		Mock<IBody> Body, Mock<IDrug> Drug, Mock<IVariableRegister> Registers) Runtime(Db.FutureProg definition,
		Db.Drug definitionDrug, int initialUnits = 1, long? pinchPrototypeId = null)
	{
		NativeProg.Initialise();
		var source = RuntimeItem(1001, "spice");
		var pinch = RuntimeItem(1002, "pinch");
		var drug = new Mock<IDrug>();
		drug.SetupGet(x => x.Id).Returns(definitionDrug.Id);
		drug.As<IProgVariable>().SetupGet(x => x.GetObject).Returns(drug.Object);
		drug.As<IProgVariable>().SetupGet(x => x.Type).Returns(ProgVariableTypes.Drug);
		drug.SetupGet(x => x.DrugVectors).Returns(DrugVector.Inhaled);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1L));
		var actor = new Mock<ICharacter>();
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.Setup(x => x.EffectsOfType<ISpellProjectionBoundary>()).Returns([]);
		actor.As<IProgVariable>().SetupGet(x => x.GetObject).Returns(actor.Object);
		actor.As<IProgVariable>().SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		actor.As<IProgVariable>().Setup(x => x.GetProperty("id")).Returns(new NumberVariable(1));
		var held = new List<IGameItem> { source.Object };
		actor.Setup(x => x.GetProperty("helditems")).Returns(() => new CollectionVariable(held, ProgVariableTypes.Item));
		actor.Setup(x => x.GetProperty("isawake")).Returns(new BooleanVariable(true));
		actor.Setup(x => x.GetProperty("incombat")).Returns(new BooleanVariable(false));
		actor.Setup(x => x.GetProperty("isincorporeal")).Returns(new BooleanVariable(false));
		actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>())).Returns(true);
		actor.Setup(x => x.HasDubFor(It.IsAny<IPerceivable>(), It.IsAny<string>())).Returns(false);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		var room = new Mock<IRoom>();
		room.As<IProgVariable>().SetupGet(x => x.GetObject).Returns(room.Object);
		room.As<IProgVariable>().SetupGet(x => x.Type).Returns(ProgVariableTypes.Location);
		room.SetupGet(x => x.Characters).Returns([]);
		actor.Setup(x => x.GetProperty("location")).Returns(room.Object);
		source.Setup(x => x.HasKeyword("spice", actor.Object, It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		source.Setup(x => x.HasKeyword("brick", actor.Object, It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		pinch.Setup(x => x.HasKeyword("pinch", actor.Object, It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		var itemValues = new Dictionary<long, decimal> { [source.Object.Id] = initialUnits, [pinch.Object.Id] = 3 };
		var registers = new Mock<IVariableRegister>();
		registers.Setup(x => x.GetType(ProgVariableTypes.Item, ArmageddonWorldfileWriter.PoisonKindRegister)).Returns(ProgVariableTypes.Number);
		registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), ArmageddonWorldfileWriter.PoisonKindRegister)).Returns(new NumberVariable(7));
		registers.Setup(x => x.SetValue(It.IsAny<IProgVariable>(), ArmageddonWorldfileWriter.PoisonKindRegister, It.IsAny<IProgVariable>())).Returns(true);
		registers.Setup(x => x.GetType(ProgVariableTypes.Item, ArmageddonWorldfileWriter.SpiceUnitsRegister)).Returns(ProgVariableTypes.Number);
		registers.Setup(x => x.SetValue(It.IsAny<IProgVariable>(), ArmageddonWorldfileWriter.SpiceUnitsRegister, It.IsAny<IProgVariable>()))
			.Callback<IProgVariable, string, IProgVariable>((item, _, value) => itemValues[((IGameItem)item.GetObject).Id] = (decimal)value.GetObject)
			.Returns(true);
		registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), ArmageddonWorldfileWriter.SpiceUnitsRegister))
			.Returns((IProgVariable item, string _) => new NumberVariable(itemValues[((IGameItem)item.GetObject).Id]));
		registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), ArmageddonGatheringRegisters.Ecology)).Returns(new NumberVariable(0));
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.VariableRegister).Returns(registers.Object);
		var runtimeProgs = new All<IFutureProg>();
		world.SetupGet(x => x.FutureProgs).Returns(runtimeProgs);
		// This fixture exercises an unpoisoned spice; the generated poison dispatcher is compiled with the complete seeder graph above.
		var poisonDispatcher = new Mock<IFutureProg>();
		poisonDispatcher.SetupGet(x => x.Id).Returns(long.MaxValue);
		poisonDispatcher.SetupGet(x => x.FunctionName).Returns("ArmageddonWorldfileSpicePoison");
		poisonDispatcher.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		poisonDispatcher.SetupGet(x => x.Parameters).Returns(new[] { ProgVariableTypes.Character, ProgVariableTypes.Item });
		poisonDispatcher.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		poisonDispatcher.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(true);
		runtimeProgs.Add(poisonDispatcher.Object);
		var drugs = new All<IDrug>(); drugs.Add(drug.Object); world.SetupGet(x => x.Drugs).Returns(drugs);
		if (pinchPrototypeId is { } pinchId)
		{
			var proto = new Mock<IGameItemProto>();
			proto.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
			proto.SetupGet(x => x.Components).Returns([]);
			proto.Setup(x => x.CreateNew(It.IsAny<ICharacter>(), It.IsAny<IGameItemSkin>(), 1, It.IsAny<string>()))
				.Returns([pinch.Object]);
			var itemProtos = new Mock<IUneditableRevisableAll<IGameItemProto>>();
			itemProtos.Setup(x => x.Get(pinchId)).Returns(proto.Object);
			world.SetupGet(x => x.ItemProtos).Returns(itemProtos.Object);
			pinch.Setup(x => x.GetItemType<IHoldable>()).Returns((IHoldable?)null);
		}
		var native = (IFutureProg)typeof(NativeProg).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
			[typeof(Db.FutureProg), typeof(IFuturemud)], null)!.Invoke([definition, world.Object]);
		runtimeProgs.Add(native);
		Assert.IsTrue(native.Compile(), native.FunctionName + ": " + native.CompileError);
		return (native, source, pinch, actor, body, drug, registers);
	}

	private static Mock<IGameItem> RuntimeItem(long id, string keyword)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.As<IProgVariable>().SetupGet(x => x.GetObject).Returns(item.Object);
		item.As<IProgVariable>().SetupGet(x => x.Type).Returns(ProgVariableTypes.Item);
		item.As<IProgVariable>().Setup(x => x.GetProperty("id")).Returns(new NumberVariable(id));
		item.Setup(x => x.HasKeyword(keyword, It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		return item;
	}

	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
}
