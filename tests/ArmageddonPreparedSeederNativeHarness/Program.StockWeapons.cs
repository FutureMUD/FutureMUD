#nullable enable
using System.Diagnostics;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedStockWeaponAcceptance(TestDatabase database)
	{
		Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		var players = TraditionPlayers(database);
		var before = PreparedDatabaseChecksum(database);
		var rollback = ArmageddonCatalogueWeapons.Install(factory, stage =>
		{
			if (stage == ArmageddonInstallCheckpoint.BeforeCommit) throw new InvalidOperationException("S3 weapon injected precommit failure");
		});
		Require(rollback.Status == ArmageddonInstallStatus.Failed && before == PreparedDatabaseChecksum(database),
			"Actual MySQL weapon rollback changed tables: " + string.Join("\n", rollback.Messages));
		var lostAck = ArmageddonCatalogueWeapons.Install(factory, stage =>
		{
			if (stage == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("S3 weapon injected lost acknowledgement");
		});
		Require(lostAck.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && lostAck.Identities.Count == 49,
			"Weapon lost-ack result was not durable: " + string.Join("\n", lostAck.Messages));
		var committed = PreparedDatabaseChecksum(database);
		var rerun = ArmageddonCatalogueWeapons.Install(factory);
		Require(rerun.Status == ArmageddonInstallStatus.Completed && committed == PreparedDatabaseChecksum(database),
			"Weapon rerun changed native content/identity: " + string.Join("\n", rerun.Messages));
		Require(!rerun.Messages.Any(x => x.Contains("preserved builder edit")), "Initial native weapon values were mistaken for builder edits.");
		RunStockWeaponReader(database.Name);
		using (var db = factory())
		{
			db.GameItemProtos.Single(x => x.Id == lostAck.Identities["arm.item.spark_lash.g1"]).ShortDescription = "a builder's sparkling lash";
			var attackId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.spark_lash.g1.attack").LogicalId;
			db.WeaponAttacks.Single(x => x.Id == attackId).StaminaCost = 8;
			var alias = db.SeederManagedRecords.Single(x => x.StableKey == "arm.binding.weapon.lash");
			var binding = System.Text.Json.JsonSerializer.Deserialize<WeaponAliasReceipt>(alias.SeedBaseline!)!;
			db.WeaponTypes.Single(x => x.Id == binding.NativeId).Name = "Builder renamed improvised combat";
			db.SaveChanges();
		}
		var edited = PreparedDatabaseChecksum(database);
		var preserved = ArmageddonCatalogueWeapons.Install(factory);
		Require(preserved.Status == ArmageddonInstallStatus.Completed && edited == PreparedDatabaseChecksum(database),
			"Weapon rerun changed builder content: " + string.Join("\n", preserved.Messages));
		Require(players == TraditionPlayers(database), "Weapon catalogue changed existing players.");
		RunStockWeaponReader(database.Name);
		Console.WriteLine("ARMS3-weapons-installation=passed actual-MySQL 49-melee-prototypes 49-native-attacks 49-combat-messages rollback lost-ack stable-IDs builder-edits renamed-shared-combat no-player-writes no-live-items; complete-S3:NOT_QUALIFIED");
		PreparedStockProvisionAcceptance(database);
	}

	private sealed record WeaponAliasReceipt(string NativeType, long NativeId);

	private static void RunStockWeaponReader(string name)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--stock-weapons-reader"); start.ArgumentList.Add(name);
		using var child = Process.Start(start)!;
		var output = child.StandardOutput.ReadToEndAsync(); var errors = child.StandardError.ReadToEndAsync();
		if (!child.WaitForExit(60000))
		{
			child.Kill(true); Require(child.WaitForExit(30000), "Owned S3 weapon reader did not stop.");
			throw new TimeoutException("S3 weapon cold reader exceeded 60 seconds.");
		}
		Require(child.ExitCode == 0, output.GetAwaiter().GetResult() + errors.GetAwaiter().GetResult());
		Console.Write(output.GetAwaiter().GetResult());
	}

	private static int ReadStockWeaponCatalogue(string name)
	{
		using var database = TestDatabase.OpenExistingOwned(name);
		ConfigureNativeDatabase(database.ConnectionString);
		var before = PreparedDatabaseChecksum(database);
		using var db = NewIndependentContext(database.ConnectionString);
		var records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == "stock-catalogue").ToArray();
		long[] Ids(string type) => records.Where(x => x.EntityType == type).Select(x => x.LogicalId!.Value).ToArray();
		var attackIds = Ids(nameof(Db.WeaponAttack)); var typeIds = Ids(nameof(Db.WeaponType)); var messageIds = Ids(nameof(Db.CombatMessage));
		Require(attackIds.Length == 49 && typeIds.Length == 49 && messageIds.Length == 49, "Durable weapon counts changed.");
		var catalogue = new ArmageddonStockCatalogue();
		var weaponKeys = catalogue.Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.MeleeItem).Select(x => x.Key).ToHashSet();
		var itemIds = records.Where(x => x.EntityType == nameof(Db.GameItemProto) && weaponKeys.Contains(x.StableKey)).Select(x => x.LogicalId!.Value).ToArray();
		var componentIds = Ids(nameof(Db.GameItemComponentProto));
		Require(itemIds.Length == 49, "Durable weapon prototypes missing.");
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var traits = new All<ITraitDefinition>(); world.SetupGet(x => x.Traits).Returns(traits);
		foreach (var row in db.TraitDefinitions)
		{
			var trait = new Mock<ITraitDefinition>(); trait.SetupGet(x => x.Id).Returns(row.Id); trait.SetupGet(x => x.Name).Returns(row.Name); traits.Add(trait.Object);
		}
		world.SetupGet(x => x.BodypartShapes).Returns(new All<MudSharp.Form.Shape.IBodypartShape>());
		var expressions = new All<ITraitExpression>(); world.SetupGet(x => x.TraitExpressions).Returns(expressions);
		var expressionIds = records.Where(x => x.EntityType == nameof(Db.TraitExpression)).Select(x => x.LogicalId!.Value).ToArray();
		foreach (var row in db.TraitExpressions.Where(x => expressionIds.Contains(x.Id))) expressions.Add(new MudSharp.Body.Traits.TraitExpression(row, world.Object));
		PositionState.SetupPositions();
		var attacks = new All<IWeaponAttack>(); world.SetupGet(x => x.WeaponAttacks).Returns(attacks);
		foreach (var row in db.WeaponAttacks.Where(x => attackIds.Contains(x.Id))) attacks.Add(MudSharp.Combat.WeaponAttack.LoadWeaponAttack(row, world.Object));
		var types = new All<IWeaponType>(); world.SetupGet(x => x.WeaponTypes).Returns(types);
		foreach (var row in db.WeaponTypes.Include(x => x.WeaponAttacks).Where(x => typeIds.Contains(x.Id))) types.Add(new MudSharp.Combat.WeaponType(row, world.Object));
		var materials = new All<ISolid>(); world.SetupGet(x => x.Materials).Returns(materials);
		foreach (var row in db.Materials) { var material = new Mock<ISolid>(); material.SetupGet(x => x.Id).Returns(row.Id); materials.Add(material.Object); }
		var components = new RevisableAll<IGameItemComponentProto>(); world.SetupGet(x => x.ItemComponentProtos).Returns(components);
		var manager = new GameItemComponentManager();
		foreach (var row in db.GameItemComponentProtos.Include(x => x.EditableItem).Where(x => componentIds.Contains(x.Id) && (x.Type == "MeleeWeapon" || x.Type == "Holdable")))
		{
			var component = manager.GetProto(row, world.Object); Require(component is not null && component.CanSubmit(), "Weapon native component readiness failed: " + row.Name); components.Add(component!);
		}
		world.SetupGet(x => x.Tags).Returns(new All<ITag>()); world.SetupGet(x => x.ItemGroups).Returns(new All<IGameItemGroup>());
		world.SetupGet(x => x.HealthStrategies).Returns(new All<IHealthStrategy>()); world.SetupGet(x => x.ItemSkins).Returns(new RevisableAll<IGameItemSkin>());
		var items = new RevisableAll<IGameItemProto>(); world.SetupGet(x => x.ItemProtos).Returns(items);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
		actor.SetupGet(x => x.CombatSettings).Returns(new Mock<ICharacterCombatSettings>().Object);
		foreach (var key in weaponKeys)
		{
			var blueprint = catalogue.ByKey[key];
			var attackId = records.Single(x => x.StableKey == key + ".attack").LogicalId!.Value;
			var attack = attacks.Get(attackId)!;
			Require(attack.Profile.DamageExpression.Evaluate(actor.Object) == blueprint.Configuration.GetProperty("native_attack_damage").GetDouble() &&
				attack.Profile.PainExpression.Evaluate(actor.Object) == blueprint.Configuration.GetProperty("native_attack_pain").GetDouble() &&
				attack.Profile.StunExpression.Evaluate(actor.Object) == 0, "Native damage expressions do not apply approved values: " + key);
			Require(attack.UsableAttack(actor.Object, null, null, AttackHandednessOptions.OneHandedOnly, false, BuiltInCombatMoveType.UseWeaponAttack),
				"Native weapon attack unusable from standing: " + key);
			var itemId = records.Single(x => x.StableKey == key).LogicalId!.Value;
			var row = db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosGameItemComponentProtos).Include(x => x.GameItemProtosTags).Single(x => x.Id == itemId);
			var item = new MudSharp.GameItems.GameItemProto(row, world.Object); items.Add(item);
			Require(item.Material is not null && item.Components.All(x => x is not null) && item.Components.Any(x => x is MudSharp.GameItems.Prototypes.MeleeWeaponGameItemComponentProto),
				"Native weapon item failed dependency load: " + key);
		}
		foreach (var row in db.CombatMessages.Include(x => x.CombatMessagesWeaponAttacks).Where(x => messageIds.Contains(x.Id)))
		{
			var message = new MudSharp.Combat.CombatMessage(row, world.Object);
			Require(message.WeaponAttackIds.Count() == 1 && attackIds.Contains(message.WeaponAttackIds.Single()), "Native combat message lost typed attack binding.");
		}
		Require(before == PreparedDatabaseChecksum(database), "Native weapon reader wrote world content.");
		Console.WriteLine("ARMS3-weapons-reader=passed fresh-process real-native-weapon-types attacks damage/pain/stun-evaluation standing-attack-eligibility melee-components item-prototypes combat-messages no-writes; external-material/trait-references:bounded-mocks combat-execution:NOT_RUN full-MUD-boot:NOT_RUN");
		return 0;
	}
}
