#nullable enable
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Autobuilder;
using MudSharp.Construction.Autobuilder.Rooms;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Decorators;
using MudSharp.Health;
using MudSharp.Work.Crafts;
using MudSharp.Work.Foraging;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedStockSupplyAcceptance(TestDatabase database)
	{
		Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		var players = TraditionPlayers(database);
		using (var db = factory())
		{
			var attribute = db.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 && x.OwnerScope == 0 && x.DerivedType == 0).Id;
			ArmageddonInstallation.Prepare(factory, ArmageddonProgressionMode.Simple, capacityAttribute: attribute);
		}
		var foundations = ArmageddonProgressionInstaller.Install(factory);
		Require(foundations.Status == ArmageddonInstallStatus.Completed, string.Join("\n", foundations.Messages));
		var before = PreparedDatabaseChecksum(database);
		var rollback = ArmageddonCatalogueSupplies.Install(factory, stage =>
		{
			if (stage == ArmageddonInstallCheckpoint.BeforeCommit) throw new InvalidOperationException("S3 injected before commit");
		});
		Require(rollback.Status == ArmageddonInstallStatus.Failed && before == PreparedDatabaseChecksum(database),
			"S3 actual MySQL rollback changed table contents: " + string.Join("\n", rollback.Messages));
		Console.WriteLine("ARMS3-supplies-rollback=passed actual-MySQL all-table-checksums-preserved");
		var lostAck = ArmageddonCatalogueSupplies.Install(factory, stage =>
		{
			if (stage == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("S3 injected lost acknowledgement");
		});
		Require(lostAck.Status == ArmageddonInstallStatus.CommittedConfirmationFailed, string.Join("\n", lostAck.Messages));
		Require(lostAck.Identities.Count == 68 && players == TraditionPlayers(database), "S3 lost-ack batch changed players or lost identities.");
		var committed = PreparedDatabaseChecksum(database);
		var rerun = ArmageddonCatalogueSupplies.Install(factory);
		Require(rerun.Status == ArmageddonInstallStatus.Completed && committed == PreparedDatabaseChecksum(database),
			"S3 rerun churned native rows, children or ownership: " + string.Join("\n", rerun.Messages));
		Require(!rerun.Messages.Any(x => x.Contains("preserved builder edit")), "MySQL DATETIME round-trip was mistaken for a builder edit.");
		VerifyStockSupplyRepair(factory);
		RunStockSupplyReader(database.Name);
		using (var db = factory())
		{
			var blankId = lostAck.Identities["arm.item.component_blank"];
			db.GameItemProtos.Single(x => x.Id == blankId).ShortDescription = "a builder's handworked blank";
			var craftId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.material.abjuration.rank0.recipe").LogicalId;
			var input = db.CraftInputs.Single(x => x.CraftId == craftId);
			var definition = XElement.Parse(input.Definition); definition.Element("Quantity")!.Value = "2"; input.Definition = definition.ToString();
			var forage = db.Foragables.Single(x => x.Name == "Armageddon component blank").Id;
			db.ForagableProfilesForagables.RemoveRange(db.ForagableProfilesForagables.Where(x => x.ForagableId == forage));
			db.SaveChanges();
		}
		var edited = PreparedDatabaseChecksum(database);
		var preserved = ArmageddonCatalogueSupplies.Install(factory);
		Require(preserved.Status == ArmageddonInstallStatus.Completed && edited == PreparedDatabaseChecksum(database),
			"S3 rerun changed builder customisations: " + string.Join("\n", preserved.Messages));
		Require(players == TraditionPlayers(database), "S3 install/rerun changed player tables.");
		RunStockSupplyReader(database.Name);
		Console.WriteLine("ARMS3-supplies-installation=passed 64-items 64-recipes 61-tags four-templates actual-MySQL rollback lost-ack stable-IDs native-child-IDs builder-edits removed-shared-join preserved no-player-writes; complete-S3:NOT_QUALIFIED");
		PreparedStockWeaponAcceptance(database);
	}

	private static void VerifyStockSupplyRepair(Func<FuturemudDatabaseContext> factory)
	{
		long inputId, productId, craftId;
		using (var db = factory())
		{
			var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.material.abjuration.rank0.recipe");
			craftId = record.LogicalId!.Value;
			var input = db.CraftInputs.Single(x => x.CraftId == craftId); inputId = input.Id;
			var product = db.CraftProducts.Single(x => x.CraftId == craftId); productId = product.Id;
			Require(input.OriginalAdditionTime.Kind == DateTimeKind.Unspecified && product.OriginalAdditionTime.Kind == DateTimeKind.Unspecified,
				"Native MySQL DATETIME fixture did not exercise unzoned ordering values.");
			var inputXml = XElement.Parse(input.Definition); inputXml.Element("Quantity")!.Value = "2"; input.Definition = inputXml.ToString();
			var productXml = XElement.Parse(product.Definition); productXml.Element("Quantity")!.Value = "2"; product.Definition = productXml.ToString();
			var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
			foreach (var (field, definition, added) in new[] { ("Inputs", input.Definition, input.OriginalAdditionTime), ("Products", product.Definition, product.OriginalAdditionTime) })
			{
				var children = JsonNode.Parse(baseline[field])!.AsArray(); children[0]!["Definition"] = definition;
				children[0]!["Added"] = DateTime.SpecifyKind(added, DateTimeKind.Utc).ToString("O"); baseline[field] = children.ToJsonString();
			}
			// A previous stock version used these quantities and UTC metadata. This is isolated
			// historical fixture data, never an alternative production catalogue or balance.
			record.SeedBaseline = JsonSerializer.Serialize(baseline); db.SaveChanges();
		}
		var repair = ArmageddonCatalogueSupplies.Install(factory);
		Require(repair.Status == ArmageddonInstallStatus.Completed && !repair.Messages.Any(x => x.Contains("preserved builder edit")),
			"Stock repair was suppressed by DATETIME Kind: " + string.Join("\n", repair.Messages));
		using (var db = factory())
		{
			Require((int)XElement.Parse(db.CraftInputs.Single(x => x.Id == inputId && x.CraftId == craftId).Definition).Element("Quantity")! == 1 &&
				(int)XElement.Parse(db.CraftProducts.Single(x => x.Id == productId && x.CraftId == craftId).Definition).Element("Quantity")! == 1,
				"Stock repair failed or replaced the native input/product IDs used by retained crafts.");
		}
		Console.WriteLine("ARMS3-supplies-stock-repair=passed actual-MySQL UTC-baseline native-DATETIME normalisation input/product-update original-child-IDs retained");
	}

	private static void RunStockSupplyReader(string name)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
		start.ArgumentList.Add("--stock-supplies-reader"); start.ArgumentList.Add(name);
		using var child = Process.Start(start)!;
		var output = child.StandardOutput.ReadToEndAsync(); var errors = child.StandardError.ReadToEndAsync();
		if (!child.WaitForExit(60000))
		{
			child.Kill(true); Require(child.WaitForExit(30000), "Owned S3 reader did not stop after timeout.");
			throw new TimeoutException("S3 supply restart exceeded 60 seconds.");
		}
		Require(child.ExitCode == 0, output.GetAwaiter().GetResult() + errors.GetAwaiter().GetResult());
		Console.Write(output.GetAwaiter().GetResult());
	}

	private static int ReadStockSupplyCatalogue(string name)
	{
		using var database = TestDatabase.OpenExistingOwned(name);
		ConfigureNativeDatabase(database.ConnectionString);
		var before = PreparedDatabaseChecksum(database);
		using var db = NewIndependentContext(database.ConnectionString);
		var records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == "stock-catalogue").ToArray();
		long[] Ids(string type) => records.Where(x => x.EntityType == type).Select(x => x.LogicalId!.Value).ToArray();
		var itemIds = Ids(nameof(Db.GameItemProto)); var craftIds = Ids(nameof(Db.Craft)); var componentIds = Ids(nameof(Db.GameItemComponentProto));
		var progIds = Ids(nameof(Db.FutureProg)); var forageIds = Ids(nameof(Db.Foragable)); var templateIds = Ids(nameof(Db.AutobuilderRoomTemplate));
		Require(itemIds.Length == 64 && craftIds.Length == 64 && Ids(nameof(Db.Tag)).Length == 61 && Ids(nameof(Db.AutobuilderRoomTemplate)).Length == 4 && Ids(nameof(Db.Foragable)).Length == 1,
			"Durable S3 native counts changed.");
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		var progs = new All<IFutureProg>(); world.SetupGet(x => x.FutureProgs).Returns(progs);
		var traits = new All<ITraitDefinition>(); world.SetupGet(x => x.Traits).Returns(traits);
		var craftingId = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockProgression.CraftTraitKey).LogicalId!.Value;
		var trait = new Mock<ITraitDefinition>(); trait.SetupGet(x => x.Id).Returns(craftingId); trait.SetupGet(x => x.Name).Returns("Renamed Component Crafting");
		trait.SetupGet(x => x.Type).Returns(ProgVariableTypes.Trait); trait.SetupGet(x => x.GetObject).Returns(trait.Object); traits.Add(trait.Object);
		MudSharp.FutureProg.FutureProg.Initialise();
		foreach (var row in db.FutureProgs.Include(x => x.FutureProgsParameters).Where(x => progIds.Contains(x.Id)))
		{
			var prog = FutureProgFactory.CreateNew(row, world.Object); Require(prog.Compile(), prog.CompileError); progs.Add(prog);
		}
		var materials = new All<ISolid>(); world.SetupGet(x => x.Materials).Returns(materials);
		foreach (var row in db.Materials)
		{
			var material = new Mock<ISolid>(); material.SetupGet(x => x.Id).Returns(row.Id); material.SetupGet(x => x.Name).Returns(row.Name); materials.Add(material.Object);
		}
		var decorators = new All<IStackDecorator>(); world.SetupGet(x => x.StackDecorators).Returns(decorators);
		foreach (var row in db.StackDecorators) decorators.Add(StackDecorator.LoadStackDecorator(row));
		var components = new RevisableAll<IGameItemComponentProto>(); world.SetupGet(x => x.ItemComponentProtos).Returns(components);
		var manager = new GameItemComponentManager();
		foreach (var row in db.GameItemComponentProtos.Include(x => x.EditableItem).Where(x => componentIds.Contains(x.Id)))
		{
			var component = manager.GetProto(row, world.Object); Require(component is not null && component.CanSubmit(), "Native component registration/readiness failed: " + row.Name); components.Add(component!);
		}
		var tags = new All<ITag>(); world.SetupGet(x => x.Tags).Returns(tags);
		foreach (var row in db.Tags) tags.Add(new MudSharp.Framework.Tag(row, world.Object));
		world.SetupGet(x => x.ItemGroups).Returns(new All<IGameItemGroup>());
		world.SetupGet(x => x.HealthStrategies).Returns(new All<IHealthStrategy>());
		world.SetupGet(x => x.ItemSkins).Returns(new RevisableAll<IGameItemSkin>());
		var items = new RevisableAll<IGameItemProto>(); world.SetupGet(x => x.ItemProtos).Returns(items);
		foreach (var row in db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosGameItemComponentProtos).Include(x => x.GameItemProtosTags).Where(x => itemIds.Contains(x.Id)))
		{
			var item = new MudSharp.GameItems.GameItemProto(row, world.Object);
			Require(item.Weight > 0 && item.Material is not null && item.Components.All(x => x is not null), "Native item dependency loading failed: " + row.Name); items.Add(item);
		}
		var catalogue = new ArmageddonStockCatalogue();
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character); actor.SetupGet(x => x.GetObject).Returns(actor.Object);
		foreach (var row in db.Crafts.Include(x => x.EditableItem).Include(x => x.CraftInputs).Include(x => x.CraftProducts).Include(x => x.CraftPhases).Where(x => craftIds.Contains(x.Id)))
		{
			var craft = new MudSharp.Work.Crafts.Craft(row, world.Object); Require(craft.CraftIsValid && craft.CanSubmit(), "Native recipe loading/readiness failed: " + row.Name);
			var key = records.Single(x => x.EntityType == nameof(Db.Craft) && x.LogicalId == row.Id).StableKey[..^".recipe".Length];
			var minimum = catalogue.ByKey[key].Recipe!.MinimumComponentCrafting;
			actor.Setup(x => x.TraitValue(trait.Object, TraitBonusContext.None)).Returns(minimum - 1);
			Require(!craft.CanUseProg.ExecuteBool(actor.Object) && !craft.AppearInCraftsListProg.ExecuteBool(actor.Object), "Below-threshold recipe admitted: " + key);
			actor.Setup(x => x.TraitValue(trait.Object, TraitBonusContext.None)).Returns(minimum);
			Require(craft.CanUseProg.ExecuteBool(actor.Object) && craft.AppearInCraftsListProg.ExecuteBool(actor.Object), "At-threshold recipe denied: " + key);
		}
		var forages = new RevisableAll<IForagable>(); world.SetupGet(x => x.Foragables).Returns(forages);
		foreach (var row in db.Foragables.Include(x => x.EditableItem).Where(x => forageIds.Contains(x.Id)))
		{
			var forage = new Foragable(row, world.Object); Require(forage.CanSubmit(), "Native foraging output is not ready."); forages.Add(forage);
		}
		var terrains = new All<ITerrain>(); world.SetupGet(x => x.Terrains).Returns(terrains);
		foreach (var row in db.Terrains) { var terrain = new Mock<ITerrain>(); terrain.SetupGet(x => x.Id).Returns(row.Id); terrains.Add(terrain.Object); }
		world.SetupGet(x => x.ForagableProfiles).Returns(new RevisableAll<IForagableProfile>());
		AutobuilderRoomByTerrain.RegisterAutobuilderLoader();
		foreach (var row in db.AutobuilderRoomTemplates.Where(x => templateIds.Contains(x.Id)))
		{
			var template = AutobuilderFactory.LoadRoom(row, world.Object);
			Require(template is AutobuilderRoomByTerrain { DefaultInfo.DefaultTerrain: not null }, "Native shelter template failed loading.");
		}
		Require(before == PreparedDatabaseChecksum(database), "Native catalogue reader changed the database.");
		Console.WriteLine("ARMS3-supplies-reader=passed fresh-process durable-prototypes native-components items recipes progs forage room-template-factories thresholds exact-ID-lookup renamed-skill no-writes; external-material/terrain/trait-references:bounded-mocks craft-execution:NOT_RUN full-MUD-boot:NOT_RUN");
		return 0;
	}
}
