#nullable enable

using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseSeeder.Seeders;
using MudSharp.FutureProg;
using MudSharp.Models;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonBandagingSeederTests
{
	[TestMethod]
	public void CriticalRecovery_UsesSupportedAttributeAliasesInPriorityOrder()
	{
		var constitution = new Db.TraitDefinition { Id = 1, Name = "Constitution", Type = 1 };
		var endurance = new Db.TraitDefinition { Id = 2, Name = "Endurance", Type = 1 };
		Assert.AreSame(constitution, ArmageddonWorldfileWriter.ResolveRecoveryAttribute([constitution]));
		Assert.AreSame(endurance, ArmageddonWorldfileWriter.ResolveRecoveryAttribute([constitution, endurance]));
	}

	[TestMethod]
	public void RetainedWorldfileContainsHeldBandageToolsAtSourceSubtypeFive()
	{
		var tools = ArmageddonWorldfileTestData.Load().ForInstallation().Items
			.Where(x => x.ItemType == 7 && x.Values.Length > 1 && x.Values[0] == 5).ToArray();
		Assert.IsTrue(tools.Length > 0);
		Assert.IsTrue(tools.Any(x => x.Values[1] >= 8));
	}

	[TestMethod]
	public void BandagingProgsCompileAndAttachToEffectiveVariantsAndOpeningStock()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var source = ArmageddonWorldfileTestData.Load().ForInstallation().Items.First(x => x.ItemType == 7 && x.Values[0] == 5 && x.Values[1] >= 8);
		var writer = new ArmageddonWorldfileWriter(db, new ArmageddonWorldfilePackage { Items = [source] });
		if (!db.TraitDefinitions.AsEnumerable().Any(x => x.Type == 0 && x.Name.Equals("Bandage", System.StringComparison.OrdinalIgnoreCase)))
			db.TraitDefinitions.Add(new Db.TraitDefinition { Id = 990_001, Name = "Bandage", Type = 0 });
		var genericPoison = new Db.Drug { Id = 990_002, Name = "Generic Poison" };
		var magicalPoison = new Db.Drug { Id = 990_003, Name = "Magical Poison" };
		db.Drugs.AddRange(genericPoison, magicalPoison);
		var purge = new Db.MagicSpell { Id = 990_004, Name = "Purge Toxin", MagicSchoolId = 1,
			Definition = new XElement("Definition", new XElement("Effects", new XElement("Effect", new XAttribute("type", "removepoison"),
				new XElement("Drug", magicalPoison.Id)))).ToString() };
		db.MagicSpells.Add(purge);
		db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "Fixture", StableKey = "arm.spell.purge_toxin",
			EntityType = nameof(Db.MagicSpell), LogicalId = purge.Id });
		var baseProto = new Db.GameItemProto { Id = 990_010, RevisionNumber = 0, Name = source.ShortDescription };
		var variantProto = new Db.GameItemProto { Id = 990_011, RevisionNumber = 0, Name = source.ShortDescription + " variant" };
		var nonBandage = new Db.GameItemProto { Id = 990_013, RevisionNumber = 0, Name = "Other tool" };
		db.GameItemProtos.AddRange(baseProto, variantProto, nonBandage);
		Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(source.Vnum, baseProto);
		Field<Dictionary<string, Db.GameItemProto>>(writer, "_resetItemVariants").Add(source.Vnum + ":fixture", variantProto);
		Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(baseProto.Id, (source, source.Values));
		var variantValues = source.Values.ToArray();
		variantValues[1] = 10;
		Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(variantProto.Id, (source, variantValues));
		var otherValues = source.Values.ToArray(); otherValues[0] = 1;
		Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(nonBandage.Id, (source, otherValues));
		const long openingStockId = 990_012;
		db.GameItems.Add(new Db.GameItem { Id = openingStockId, GameItemProtoId = variantProto.Id, GameItemProtoRevision = 0,
			EffectData = "<Effects/>" });
		var poisonIds = Field<Dictionary<int, long>>(writer, "_poisonDrugIds");
		poisonIds.Add(1, genericPoison.Id);
		db.SaveChanges();

		typeof(ArmageddonWorldfileWriter).GetMethod("InstallBandaging", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var progs = db.FutureProgs.AsEnumerable().Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileBandaging", System.StringComparison.Ordinal)).ToArray();
		var commandProgs = progs.Where(x => x.FunctionName == $"ArmageddonWorldfileBandaging{baseProto.Id}" ||
			x.FunctionName == $"ArmageddonWorldfileBandaging{variantProto.Id}").ToArray();
		Assert.AreEqual(2, commandProgs.Length);
		Assert.IsTrue(commandProgs.All(x => x.FunctionText.Contains($"todrug({magicalPoison.Id})", System.StringComparison.Ordinal)));
		Assert.IsTrue(commandProgs.All(x => x.FunctionText.Contains($"todrug({genericPoison.Id})", System.StringComparison.Ordinal)));
		Assert.IsTrue(commandProgs.All(x => x.FunctionText.IndexOf($"todrug({magicalPoison.Id})", System.StringComparison.Ordinal) <
			x.FunctionText.IndexOf($"todrug({genericPoison.Id})", System.StringComparison.Ordinal)), "Magical poison must be tested before generic poison.");
		Assert.IsTrue(commandProgs.All(x => x.FunctionText.Contains("check(@actor, @bandage, 5) > 0", System.StringComparison.Ordinal)));
		Assert.IsTrue(commandProgs.All(x => x.FunctionText.Contains("@actor.helditems.Any(held, @held == @item)", System.StringComparison.Ordinal)));
		Assert.IsTrue(db.HooksPerceivables.Any(x => x.GameItemId == openingStockId));
		Assert.IsTrue(baseProto.GameItemProtosOnLoadProgs.Any());
		Assert.IsTrue(variantProto.GameItemProtosOnLoadProgs.Any());
		Assert.IsFalse(nonBandage.GameItemProtosOnLoadProgs.Any());
		using var compiler = new MudSharp.Framework.OfflineProgCompilation(
			db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray(), db.VariableDefinitions.AsNoTracking().ToArray());
		foreach (var prog in progs)
		{
			try { compiler.Compile(prog.Id); }
			catch (System.InvalidOperationException error)
			{
				Assert.Fail($"{error.Message}\n{prog.FunctionText}");
			}
		}
	}

	[TestMethod]
	public void ImportedHitPointStrategiesUseNegativeHealthAndEnduranceRecoveryDefaults()
	{
		var definition = ArmageddonWorldfileWriter.ConfigureArmageddonHitPointPolicy(new XElement("Definition"), 456);
		Assert.AreEqual("true", definition.Element("UseNegativeHitPoints")?.Value);
		Assert.AreEqual("-11", definition.Element("DeathHitPoints")?.Value);
		Assert.AreEqual("0", definition.Element("UnconsciousHitPoints")?.Value);
		Assert.AreEqual("-3", definition.Element("MortallyWoundedHitPoints")?.Value);
		Assert.AreEqual("true", definition.Element("UseCriticalRecovery")?.Value);
		Assert.AreEqual("456", definition.Element("CriticalRecoveryTrait")?.Value);
		Assert.AreEqual("5", definition.Element("CriticalRecoveryDifficulty")?.Value);
		Assert.AreEqual("6", definition.Element("MortalRecoveryDifficulty")?.Value);
		Assert.AreEqual("1", definition.Element("CriticalRecoveryHealing")?.Value);
		Assert.AreEqual("1", definition.Element("CriticalRecoveryDamage")?.Value);
	}
	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;

}
