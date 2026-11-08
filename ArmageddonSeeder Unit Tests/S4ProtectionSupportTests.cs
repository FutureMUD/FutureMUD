#nullable enable
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Form.Material;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using NativeArmour = MudSharp.Combat.ArmourType;

namespace MudSharp_Unit_Tests;

[TestClass]
public class S4ProtectionSupportTests
{
	[TestMethod]
	public void CreationSix_RealTypedSupplyAndCraftCanBeLoadedAfterContextClear()
	{
		using var db = Fixture(); var result = Install(db); db.ChangeTracker.Clear();
		var item = db.GameItemProtos.Include(x => x.GameItemProtosTags).Include(x => x.GameItemProtosGameItemComponentProtos)
			.Single(x => x.Id == result.Identities[ArmageddonStockSupportInstaller.CreationRankSixKey]);
		Assert.AreEqual(10.0, item.Weight); // Fixture kilogram is 1000 native weight units.
		Assert.AreEqual(result.Identities[ArmageddonStockSupportInstaller.CreationRankSixTagKey], item.GameItemProtosTags.Single().TagId);
		Assert.AreEqual("Creation rank 6", db.Tags.Single(x => x.Id == item.GameItemProtosTags.Single().TagId).Name);
		CollectionAssert.AreEquivalent(new[] { "Holdable", "Stackable" }, item.GameItemProtosGameItemComponentProtos.Select(link => db.GameItemComponentProtos.Single(x => x.Id == link.GameItemComponentProtoId && x.RevisionNumber == link.GameItemComponentRevision).Type).ToArray());
		var craft = db.Crafts.Include(x => x.CraftInputs).Include(x => x.CraftProducts).Include(x => x.CraftPhases).Single(x => x.Id == result.Identities[ArmageddonStockSupportInstaller.CreationRankSixRecipeKey]);
		Assert.AreEqual(420.0, craft.CraftPhases.Single().PhaseLengthInSeconds);
		var input = XElement.Parse(craft.CraftInputs.Single().Definition); Assert.AreEqual(7, (int)input.Element("Quantity")!);
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.component_blank").LogicalId, (long)input.Element("TargetItemId")!);
		var product = XElement.Parse(craft.CraftProducts.Single().Definition); Assert.AreEqual(item.Id, (long)product.Element("ProductProducedId")!); Assert.AreEqual(1, (int)product.Element("Quantity")!);
		Assert.AreEqual(result.Identities[ArmageddonStockSupportInstaller.CreationRankSixGateKey], craft.CanUseProgId);
		var gate = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == craft.CanUseProgId);
		StringAssert.Contains(gate.FunctionText, ">= 90");
		using var compiler = new OfflineProgCompilation([gate]); var compiled = compiler.Compile(gate.Id);
		Assert.AreEqual(ProgVariableTypes.Boolean, compiled.ReturnType); Assert.IsTrue(compiled.MatchesParameters([ProgVariableTypes.Character]));
		Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(210, new ArmageddonStockCatalogue().Blueprints.Count);
	}

	[TestMethod]
	[DataRow(false)] [DataRow(true)]
	public void Armour_ColdNativeLoadPreservesSupportedAndExcludedDamageChannels(bool electrical)
	{
		using var db = Fixture(); var result = Install(db); db.ChangeTracker.Clear();
		var key = electrical ? ArmageddonStockSupportInstaller.ElectricalArmourKey : ArmageddonStockSupportInstaller.ExternalArmourKey;
		var row = db.ArmourTypes.AsNoTracking().Single(x => x.Id == result.Identities[key]);
		// Only the world/body/material boundary is mocked; real persisted armour XML and native damage evaluation run.
		var native = new NativeArmour(row, Mock.Of<IFuturemud>());
		foreach (var kind in new[] { DamageType.Crushing, DamageType.Burning, DamageType.Chemical, DamageType.Electrical, DamageType.Sonic, DamageType.Arcane, DamageType.Hypoxia, DamageType.Cellular, DamageType.Necrotic, DamageType.Eldritch })
		{
			var absorbs = electrical ? kind == DamageType.Electrical : kind is DamageType.Crushing or DamageType.Burning or DamageType.Chemical or DamageType.Electrical or DamageType.Sonic or DamageType.Arcane;
			var damage = new MudSharp.Health.Damage { DamageType = kind, DamageAmount = 12, PainAmount = 7, StunAmount = 4, PenetrationOutcome = Outcome.NotTested };
			var hit = native.AbsorbDamageViaSpell(damage, Mock.Of<ISolid>(), ItemQuality.Standard, Mock.Of<IHaveWounds>(), false);
			if (absorbs)
			{
				Assert.IsNull(hit.PassedOn, kind.ToString()); Assert.IsNotNull(hit.Absorbed); Assert.AreEqual(12.0, hit.Absorbed.DamageAmount); Assert.AreEqual(7.0, hit.Absorbed.PainAmount); Assert.AreEqual(4.0, hit.Absorbed.StunAmount);
			}
			else
			{
				Assert.IsNotNull(hit.PassedOn, kind.ToString()); Assert.AreEqual(12.0, hit.PassedOn.DamageAmount); Assert.AreEqual(7.0, hit.PassedOn.PainAmount); Assert.AreEqual(4.0, hit.PassedOn.StunAmount);
			}
		}
	}

	[TestMethod]
	public void Rerun_BuilderArmourAndSupplyEditsKeepExactIdentities()
	{
		using var db = Fixture(); var first = Install(db); db.ChangeTracker.Clear();
		var armour = db.ArmourTypes.Single(x => x.Id == first.Identities[ArmageddonStockSupportInstaller.ExternalArmourKey]); armour.Name = "Builder ward";
		var original = armour.Definition;
		armour.Definition = armour.Definition.Replace("><![CDATA[0]]></Expression>", "><![CDATA[damage/2]]></Expression>");
		var edited = armour.Definition;
		Assert.AreNotEqual(original, edited);
		var item = db.GameItemProtos.Single(x => x.Id == first.Identities[ArmageddonStockSupportInstaller.CreationRankSixKey]); item.ShortDescription = "a builder component";
		db.SaveChanges(); var before = db.SeederManagedRecords.Count(); var second = Install(db); db.ChangeTracker.Clear();
		Assert.AreEqual(before, db.SeederManagedRecords.Count()); foreach (var (key, id) in first.Identities) Assert.AreEqual(id, second.Identities[key]);
		Assert.AreEqual("Builder ward", db.ArmourTypes.Single(x => x.Id == armour.Id).Name); Assert.AreEqual(edited, db.ArmourTypes.Single(x => x.Id == armour.Id).Definition);
		Assert.AreEqual("a builder component", db.GameItemProtos.Single(x => x.Id == item.Id).ShortDescription);
	}

	[TestMethod]
	[DataRow("blank")] [DataRow("stackable")] [DataRow("clay")] [DataRow("craft")]
	public void MissingTypedDependencies_RefuseBeforeAnySupplementalWrites(string failure)
	{
		using var db = Fixture();
		var key = failure switch { "blank" => "arm.item.component_blank", "stackable" => "arm.catalogue.component.stackable", "clay" => "arm.binding.material.clay", _ => ArmageddonStockProgression.CraftTraitKey };
		db.SeederManagedRecords.Remove(db.SeederManagedRecords.Single(x => x.StableKey == key)); db.SaveChanges();
		var rows = (db.SeederManagedRecords.Count(), db.GameItemProtos.Count(), db.ArmourTypes.Count(), db.Crafts.Count(), db.FutureProgs.Count());
		var result = ArmageddonStockSupportInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join("\n", result.Messages)); Assert.AreEqual(0, result.Identities.Count);
		Assert.AreEqual(rows, (db.SeederManagedRecords.Count(), db.GameItemProtos.Count(), db.ArmourTypes.Count(), db.Crafts.Count(), db.FutureProgs.Count()));
	}

	[TestMethod]
	public void UnownedArmourNameCollision_RefusesWithoutAdoptionOrOtherWrites()
	{
		using var db = Fixture(); db.ArmourTypes.Add(new ArmourType { Name = "Armageddon electrical protection", Definition = "<Definition />" }); db.SaveChanges();
		var before = db.SeederManagedRecords.Count(); var result = ArmageddonStockSupportInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status); Assert.AreEqual(before, db.SeederManagedRecords.Count()); Assert.AreEqual(1, db.ArmourTypes.Count());
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == ArmageddonStockSupportInstaller.CreationRankSixKey));
	}

	[TestMethod]
	[DataRow("revision")] [DataRow("missing")] [DataRow("retired")] [DataRow("module")] [DataRow("competing")]
	public void ArmourOwnership_InvalidClaimsStillRefusePrepareWithoutChanges(string failure)
	{
		using var db = Fixture(); var installed = Install(db); db.ChangeTracker.Clear();
		var claim = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.ExternalArmourKey);
		switch (failure)
		{
			case "revision": claim.RevisionNumber = 1; break;
			case "missing": db.ArmourTypes.Remove(db.ArmourTypes.Single(x => x.Id == claim.LogicalId)); break;
			case "retired": claim.Retired = true; break;
			case "module": claim.Module = "unknown"; break;
			case "competing": db.SeederManagedRecords.Add(new SeederManagedRecord { Seeder = "Other owner", StableKey = "other.armour", EntityType = nameof(ArmourType), LogicalId = claim.LogicalId }); break;
		}
		db.SaveChanges(); var before = (db.SeederManagedRecords.Count(), db.ArmourTypes.Count(), db.GameItemProtos.Count());
		Assert.ThrowsExactly<InvalidOperationException>(() => ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db), ArmageddonProgressionMode.Simple));
		Assert.AreEqual(before, (db.SeederManagedRecords.Count(), db.ArmourTypes.Count(), db.GameItemProtos.Count()));
		Assert.AreEqual(installed.Identities[ArmageddonStockSupportInstaller.ExternalArmourKey], claim.LogicalId);
	}

	private static ArmageddonInstallResult Install(FuturemudDatabaseContext db)
	{
		var result = ArmageddonStockSupportInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages)); return result;
	}
	private static FuturemudDatabaseContext Fixture() => (FuturemudDatabaseContext)typeof(ArmageddonStockSupportInstallerTests)
		.GetMethod("Fixture", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [ArmageddonProgressionMode.Simple, false])!;
}
