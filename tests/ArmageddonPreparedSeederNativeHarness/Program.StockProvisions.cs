#nullable enable
using System.Diagnostics;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedStockProvisionAcceptance(TestDatabase database)
	{
		Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		var players = TraditionPlayers(database);
		var before = PreparedDatabaseChecksum(database);
		var rollback = ArmageddonCatalogueProvisions.Install(factory, stage =>
		{
			if (stage == ArmageddonInstallCheckpoint.BeforeCommit) throw new InvalidOperationException("S3 provision injected precommit failure");
		});
		Require(rollback.Status == ArmageddonInstallStatus.Failed && before == PreparedDatabaseChecksum(database),
			"Actual MySQL provision rollback changed tables: " + string.Join("\n", rollback.Messages));
		var lostAck = ArmageddonCatalogueProvisions.Install(factory, stage =>
		{
			if (stage == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("S3 provision injected lost acknowledgement");
		});
		Require(lostAck.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && lostAck.Identities.Count == 6,
			"Provision lost-ack was not durable: " + string.Join("\n", lostAck.Messages));
		var committed = PreparedDatabaseChecksum(database);
		var rerun = ArmageddonCatalogueProvisions.Install(factory);
		Require(rerun.Status == ArmageddonInstallStatus.Completed && committed == PreparedDatabaseChecksum(database),
			"Provision no-op rerun changed tables: " + string.Join("\n", rerun.Messages));
		Require(!rerun.Messages.Any(x => x.Contains("preserved builder edit")), "Native provider defaults were mistaken for provision builder edits.");
		RunStockProvisionReader(database.Name);
		using (var db = factory())
		{
			db.GameItemProtos.Single(x => x.Id == lostAck.Identities["arm.item.food.bread"]).FullDescription = "The builder's sustaining flatbread.";
			db.Liquids.Single(x => x.Id == lostAck.Identities["arm.liquid.wine"]).TasteText = "The builder's spiced wine.";
			var alias = db.SeederManagedRecords.Single(x => x.StableKey == "arm.binding.material.food");
			var binding = System.Text.Json.JsonSerializer.Deserialize<WeaponAliasReceipt>(alias.SeedBaseline!)!;
			db.Materials.Single(x => x.Id == binding.NativeId).Name = "Builder prepared-food material";
			db.SaveChanges();
		}
		var edited = PreparedDatabaseChecksum(database);
		var preserved = ArmageddonCatalogueProvisions.Install(factory);
		Require(preserved.Status == ArmageddonInstallStatus.Completed && edited == PreparedDatabaseChecksum(database),
			"Provision rerun overwrote builder content: " + string.Join("\n", preserved.Messages));
		Require(players == TraditionPlayers(database), "Provision installation changed players.");
		RunStockProvisionReader(database.Name);
		Console.WriteLine("ARMS3-provisions-installation=passed actual-MySQL three-foods empty-bottle wine water-binding rollback lost-ack stable-IDs builder-edits renamed-shared-material no-player-writes no-live-items; complete-S3:NOT_QUALIFIED");
	}

	private static void RunStockProvisionReader(string name)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--stock-provisions-reader"); start.ArgumentList.Add(name);
		using var child = Process.Start(start)!;
		var output = child.StandardOutput.ReadToEndAsync(); var errors = child.StandardError.ReadToEndAsync();
		if (!child.WaitForExit(60000))
		{
			child.Kill(true); Require(child.WaitForExit(30000), "Owned S3 provision reader did not stop.");
			throw new TimeoutException("S3 provision cold reader exceeded 60 seconds.");
		}
		Require(child.ExitCode == 0, output.GetAwaiter().GetResult() + errors.GetAwaiter().GetResult());
		Console.Write(output.GetAwaiter().GetResult());
	}

	private static int ReadStockProvisionCatalogue(string name)
	{
		using var database = TestDatabase.OpenExistingOwned(name);
		ConfigureNativeDatabase(database.ConnectionString);
		var before = PreparedDatabaseChecksum(database);
		using var db = NewIndependentContext(database.ConnectionString);
		var records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == "stock-catalogue").ToArray();
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		world.SetupGet(x => x.Drugs).Returns(new All<IDrug>());
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var decorators = new All<IStackDecorator>(); world.SetupGet(x => x.StackDecorators).Returns(decorators);
		foreach (var row in db.StackDecorators) decorators.Add(StackDecorator.LoadStackDecorator(row));
		var units = new Mock<IUnitManager>(); world.SetupGet(x => x.UnitManager).Returns(units.Object);
		var settings = ArmageddonInstallation.Read(db)!;
		var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
		var litres = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Litre]).BaseMultiplier;
		units.SetupGet(x => x.BaseWeightToKilograms).Returns(1.0 / kilograms);
		units.SetupGet(x => x.BaseFluidToLitres).Returns(1.0 / litres);
		var liquids = new All<ILiquid>(); world.SetupGet(x => x.Liquids).Returns(liquids);
		var wineId = records.Single(x => x.StableKey == "arm.liquid.wine").LogicalId!.Value;
		var wine = new MudSharp.Form.Material.Liquid(db.Liquids.Include(x => x.LiquidsTags).Single(x => x.Id == wineId), world.Object);
		liquids.Add(wine);
		Require(wine.Density == 990 && wine.WaterLitresPerLitre == 0.88 && wine.AlcoholLitresPerLitre == 0.12, "Native wine composition/density changed.");
		var mixture = new LiquidMixture(wine, litres, world.Object);
		Require(Math.Abs(mixture.TotalWeight - 0.99 * kilograms) < 0.000001, "One litre of native wine did not weigh 0.99 kg.");
		var parent = new Mock<IGameItem>(); parent.SetupGet(x => x.Gameworld).Returns(world.Object);
		var manager = new GameItemComponentManager();
		foreach (var key in new[] { "arm.item.food.bread", "arm.item.food.fruit", "arm.item.food.stew" })
		{
			var id = records.Single(x => x.StableKey == key + ".component").LogicalId;
			var proto = manager.GetProto(db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == id), world.Object);
			Require(proto is FoodGameItemComponentProto && proto.CanSubmit(), "Native food component not ready: " + key);
			foreach (var remaining in new[] { 2, 0 })
			{
				var food = (IEdible)proto!.LoadComponent(new Db.GameItemComponent { Id = 1, Definition = $"<Definition><Bites>{remaining}</Bites></Definition>" }, parent.Object);
				Require(food.TotalBites == 5 && food.BitesRemaining == remaining && food.SatiationPoints == 5 && food.WaterLitres == 0.1,
					"Native food reload changed remaining bites/nutrition: " + key);
			}
		}
		var bottleId = records.Single(x => x.StableKey == "arm.item.drink_container.component").LogicalId;
		var bottleProto = manager.GetProto(db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == bottleId), world.Object)!;
		Require(bottleProto is LiquidContainerGameItemComponentProto && bottleProto.CanSubmit(), "Native bottle component not ready.");
		var empty = (ILiquidContainer)bottleProto.LoadComponent(new Db.GameItemComponent { Id = 2, Definition = "<Definition Open=\"false\"><Locks/><Mix/></Definition>" }, parent.Object);
		Require(empty.LiquidCapacity == litres && (empty.LiquidMixture is null || empty.LiquidMixture.IsEmpty), "Empty bottle refilled on reload.");
		var fullComponent = bottleProto.LoadComponent(new Db.GameItemComponent { Id = 3,
			Definition = new System.Xml.Linq.XElement("Definition", new System.Xml.Linq.XAttribute("Open", true),
				new System.Xml.Linq.XElement("Locks"), mixture.SaveToXml()).ToString() }, parent.Object);
		Require(Math.Abs(fullComponent.ComponentWeight - 0.99 * kilograms) < 0.000001, "Native bottle did not propagate correct wine mass.");
		parent.Verify(x => x.Delete(), Times.Never);
		Require(before == PreparedDatabaseChecksum(database), "Native provision reader wrote world content.");
		Console.WriteLine("ARMS3-provisions-reader=passed fresh-process native-food-prototypes partial/exhausted-component-reload no-refill native-wine density/mass native-empty/full-bottle no-writes; actual-eating/drinking:NOT_RUN full-MUD-boot:NOT_RUN");
		return 0;
	}
}
