#nullable enable

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Implementations;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Commands;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void SeedRegressionP2Fixture(TestDatabase database, IFuturemud world)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var decorator = new Db.StackDecorator { Name = "ARMRegression stack", Type = "Pile", Description = "Ordinary native stack regression",
			Definition = "<Definition><Range Min='0' Max='2147483647' Item='a pile of '/></Definition>" };
		db.StackDecorators.Add(decorator); db.SaveChanges();
		db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Stackable").Definition = $"<Definition Decorator='{decorator.Id}'/>";
		var trait = world.Traits.First().Id;
		var ranged = new Db.RangedWeaponTypes { Name = "ARMRegression firearm", FireTraitId = trait, OperateTraitId = trait,
			RangedWeaponType = (int)RangedWeaponType.ModernFirearm, SpecificAmmunitionGrade = "ARMRegression round", AmmunitionCapacity = 5,
			AccuracyBonusExpression = "0", DamageBonusExpression = "0", StaminaPerLoadStage = 7, StaminaToFire = 3,
			LoadDelay = 1, ReadyDelay = 1, FireDelay = 1, DefaultRangeInRooms = 1 };
		var ammo = new Db.AmmunitionTypes { Name = "ARMRegression ammunition", SpecificType = "ARMRegression round",
			RangedWeaponTypes = ((int)RangedWeaponType.ModernFirearm).ToString(CultureInfo.InvariantCulture),
			DamageExpression = "0", StunExpression = "0", PainExpression = "0", ProjectileCount = 1 };
		db.RangedWeaponTypes.Add(ranged); db.AmmunitionTypes.Add(ammo); db.SaveChanges();
		Db.GameItemComponentProto Component(string type, string xml)
		{
			var row = new Db.GameItemComponentProto { Id = db.GameItemComponentProtos.Max(x => x.Id) + 1, Name = "ARMRegression " + type,
				Type = type, Description = type, Definition = xml, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			db.GameItemComponentProtos.Add(row); db.SaveChanges(); return row;
		}
		var gunXml = new XElement("Definition", new[] { "LoadEmote", "ReadyEmote", "UnloadEmote", "UnreadyEmote", "UnreadyEmoteNoChamberedRound", "FireEmote", "FireEmoteNoChamberedRound" }
			.Select(x => new XElement(x, "@ operate|operates $1.")), new XElement("RangedWeaponType", ranged.Id),
			new XElement("MeleeWeaponType", db.WeaponTypes.First().Id), new XElement("InternalMagazineCapacity", 5), new XElement("CycleType", "Manual"));
		var gun = Component("InternalMagazineGun", gunXml.ToString());
		var round = Component("Ammunition", $"<Definition><AmmoType>{ammo.Id}</AmmoType><CasingProto>0</CasingProto><BulletProto>0</BulletProto></Definition>");
		var hold = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Holdable");
		foreach (var (name, component) in new[] { ("gun", gun), ("round", round) })
		{
			var row = new Db.GameItemProto { Id = db.GameItemProtos.Max(x => x.Id) + 1, Name = "ARMRegression " + name, Keywords = name,
				ShortDescription = "a regression " + name, FullDescription = "An ordinary disposable native regression item.",
				MaterialId = world.Materials.First().Id, Size = 1, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard,
				EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			foreach (var part in new[] { hold, component }) row.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = part.Id });
			db.GameItemProtos.Add(row); db.SaveChanges();
		}
	}

	private static void ConfigureRegressionP2Fixture(RetirementHost host, TestDatabase database)
	{
		var native = host.Native; var world = native.World;
		ConfigureStormHands(native);
		foreach (var name in new[] { "_riders", "_seenTargets" })
		{
			var field = typeof(MudSharp.Character.Character).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!;
			if (field.GetValue(native.Actor) is null) field.SetValue(native.Actor, Activator.CreateInstance(field.FieldType));
		}
		native.WorldMock.Setup(x => x.GetStaticDouble("RouteCellImmediateDistanceMetres")).Returns(RouteSpatialConfiguration.Default.ImmediateDistanceMetres);
		native.WorldMock.Setup(x => x.GetStaticDouble("RouteCellProximateDistanceMetres")).Returns(RouteSpatialConfiguration.Default.ProximateDistanceMetres);
		native.WorldMock.Setup(x => x.GetStaticDouble("RouteCellDistantDistanceMetres")).Returns(RouteSpatialConfiguration.Default.DistantDistanceMetres);
		native.WorldMock.Setup(x => x.GetStaticDouble("RouteCellVeryDistantDistanceMetres")).Returns(RouteSpatialConfiguration.Default.VeryDistantDistanceMetres);
		native.WorldMock.Setup(x => x.GetStaticDouble("RouteCellDefaultRoomEquivalentMetres")).Returns(RouteSpatialConfiguration.Default.DefaultRoomEquivalentMetres);
		using var db = NewIndependentContext(database.ConnectionString);
		var decorators = new All<IStackDecorator>();
		decorators.Add(new PileDecorator(db.StackDecorators.Single(x => x.Name == "ARMRegression stack")));
		native.WorldMock.SetupGet(x => x.StackDecorators).Returns(decorators);
		var stackModel = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Stackable");
		typeof(StackableGameItemComponentProto).GetProperty(nameof(StackableGameItemComponentProto.DescriptionDecorator))!
			.SetValue(world.ItemComponentProtos.Get(stackModel.Id, 0), decorators.First());
		var ranged = new All<IRangedWeaponType>(); ranged.Add(new RangedWeaponTypeDefinition(db.RangedWeaponTypes.Single(x => x.Name == "ARMRegression firearm"), world));
		var ammo = new All<IAmmunitionType>(); ammo.Add(new AmmunitionType(db.AmmunitionTypes.Single(x => x.Name == "ARMRegression ammunition"), world));
		native.WorldMock.SetupGet(x => x.RangedWeaponTypes).Returns(ranged); native.WorldMock.SetupGet(x => x.AmmunitionTypes).Returns(ammo);
		var components = db.GameItemComponentProtos.Include(x => x.EditableItem).Where(x => x.Name.StartsWith("ARMRegression")).ToArray()
			.ToDictionary(x => x.Id, x => (IGameItemComponentProto)(x.Type == "InternalMagazineGun" ? typeof(InternalMagazineGunGameItemComponentProto) : typeof(AmmunitionGameItemComponentProto))
				.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(Db.GameItemComponentProto), typeof(IFuturemud)], null)!.Invoke([x, world]));
		var oldCatalogue = world.ItemComponentProtos;
		var catalogue = new Mock<IUneditableRevisableAll<IGameItemComponentProto>>();
		catalogue.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>())).Returns<long, int>((id, revision) => components.GetValueOrDefault(id) ?? oldCatalogue.Get(id, revision));
		native.WorldMock.SetupGet(x => x.ItemComponentProtos).Returns(catalogue.Object);
		foreach (var row in db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosTags).Include(x => x.GameItemProtosGameItemComponentProtos).Where(x => x.Name.StartsWith("ARMRegression")))
			host.Prototypes.Add(row.Id, new GameItemProto(row, world));
	}

	private static Cell RegressionP2SecondCell(NativeRuntime native, TestDatabase database, Cell source, long id, bool create)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		if (create)
		{
			db.Cells.Add(new Db.Cell { Id = id, RoomId = source.Room.Id, EffectData = "<Effects/>" }); db.SaveChanges();
			var overlay = new Db.CellOverlay { Id = id, CellId = id, CellName = "A second disposable regression cell", CellDescription = "Native movement acceptance.",
				Name = "ARMRegression overlay", CellOverlayPackageId = source.CurrentOverlay.Package.Id, CellOverlayPackageRevisionNumber = source.CurrentOverlay.Package.RevisionNumber,
				TerrainId = source.CurrentOverlay.Terrain.Id, AmbientLightFactor = 1, SafeQuit = true };
			db.CellOverlays.Add(overlay); db.SaveChanges(); db.Cells.Find(id)!.CurrentOverlayId = overlay.Id; db.SaveChanges();
			foreach (var (cellId, length, position) in new[] { (source.Id, 100m, 25m), (id, 200m, 125m) })
				db.RouteCells.Add(new Db.RouteCell { CellId = cellId, LengthMetres = length, DefaultPositionMetres = position,
					MetresPerRoomEquivalent = 100, PositiveDirectionName = "ahead", NegativeDirectionName = "behind", TopologyVersion = 1 });
			db.SaveChanges();
		}
		var model = db.Cells.Include(x => x.CellOverlays).Include(x => x.CellsMagicResources).AsNoTracking().Single(x => x.Id == id);
		var destination = new Cell(model, source.Room); destination.PostLoadTasks(model);
		source.ReloadRouteDefinition(db.RouteCells.AsNoTracking().Single(x => x.CellId == source.Id));
		destination.ReloadRouteDefinition(db.RouteCells.AsNoTracking().Single(x => x.CellId == id));
		var cells = new All<ICell>(); cells.Add(source); cells.Add(destination); native.WorldMock.SetupGet(x => x.Cells).Returns(cells);
		return destination;
	}

	private sealed record RegressionP2SavedStack(long Id, int Quantity, ItemOwnershipReference? Owner, long? Body, long? Character, long? Cell, bool Deleted = false);
	private sealed record RegressionP2Reader(string Database, FixtureIds Fixture, DateTime Now, long Destination, RegressionP2SavedStack[] Stacks, string Scenario, int ExpectedTotal = 8, long? CallerItem = null, double? CallerCondition = null);
	private static int RunRegressionP2Reader(string[] args)
	{
		var input = JsonSerializer.Deserialize<RegressionP2Reader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, corpseAnimationAnatomy: true);
		ConfigureRegressionP2Fixture(host, database);
		var world = host.Native.World; var source = CreateAreaCell(host.Native, database.ConnectionString, input.Fixture.CellId, create: false);
		var destination = RegressionP2SecondCell(host.Native, database, source, input.Destination, create: false);
		// Inventory scenarios are saved after travel teardown, with ordinary-cell positions.
		source.ReloadRouteDefinition(null!); destination.ReloadRouteDefinition(null!);
		SetPrivateMember(host.Native.Actor, "Location", source);
		foreach (var saved in input.Stacks.Where(x => x.Character.HasValue).DistinctBy(x => x.Character))
		{
			var actor = world.TryGetCharacter(saved.Character!.Value, true)!;
			SetPrivateMember(actor, "Location", source);
			using var db = NewIndependentContext(database.ConnectionString);
			((Body)actor.Body).LoadInventory(db.Bodies.Include(x => x.BodiesGameItems).Single(x => x.Id == saved.Body));
		}
		foreach (var saved in input.Stacks)
		{
			if (saved.Deleted)
			{
				using var absent = NewIndependentContext(database.ConnectionString);
				Require(saved.Quantity == 0 && !absent.GameItems.Any(x => x.Id == saved.Id) &&
					!absent.GameItemComponents.Any(x => x.GameItemId == saved.Id) &&
					!absent.BodiesGameItems.Any(x => x.GameItemId == saved.Id) &&
					!absent.CellsGameItems.Any(x => x.GameItemId == saved.Id) && world.TryGetItem(saved.Id, true) is null,
					"Absorbed native stack or its persistence joins survived cold reload.");
				continue;
			}
			using (var stored = NewIndependentContext(database.ConnectionString))
			{
				Require(stored.GameItems.AsNoTracking().Any(x => x.Id == saved.Id), "Cold live stack item row must survive the caller's later flush.");
				var cells = stored.CellsGameItems.AsNoTracking().Where(x => x.GameItemId == saved.Id).Select(x => x.CellId).ToArray();
				var bodies = stored.BodiesGameItems.AsNoTracking().Where(x => x.GameItemId == saved.Id).Select(x => x.BodyId).ToArray();
				Require(cells.SequenceEqual(saved.Cell.HasValue ? new[] { saved.Cell.Value } : Array.Empty<long>()) &&
					bodies.SequenceEqual(saved.Body.HasValue ? new[] { saved.Body.Value } : Array.Empty<long>()),
					"Cold stored custody joins must name the exact expected cell and body before runtime placement.");
			}
			var item = (GameItem)world.TryGetItem(saved.Id, true)!; item.FinaliseLoadTimeTasks();
			if (saved.Cell.HasValue) (saved.Cell == source.Id ? source : destination).Insert(item, true);
			Require(item.Quantity == saved.Quantity && item.OwnershipReference == saved.Owner &&
				item.GetItemType<IHoldable>()!.HeldBy?.Id == saved.Body && item.DirectLocation?.Id == saved.Cell && item.ContainedIn is null,
				"Cold native stack lost quantity, title or exact custody.");
			Require(saved.Body is null || item.GetItemType<IHoldable>()!.HeldBy!.HeldItems.Any(x => ReferenceEquals(x, item)), "Cold native body lost exact stack membership.");
			using var db = NewIndependentContext(database.ConnectionString);
			Require(db.BodiesGameItems.Count(x => x.GameItemId == item.Id) == (saved.Body.HasValue ? 1 : 0) &&
				db.CellsGameItems.Count(x => x.GameItemId == item.Id) == (saved.Cell.HasValue ? 1 : 0), "Cold loading lost saved custody joins.");
		}
		if (input.CallerItem.HasValue)
		{
			using var stored = NewIndependentContext(database.ConnectionString);
			Require(input.CallerCondition.HasValue && Same(stored.GameItems.AsNoTracking().Single(x => x.Id == input.CallerItem.Value).Condition, input.CallerCondition.Value),
				"Fresh reader must retain the unrelated caller's dirty field through absorbed-source deletion.");
		}
		Require(input.Stacks.Sum(x => x.Quantity) == input.ExpectedTotal, "Cold fixture quantity must remain exactly eight.");
		Console.WriteLine($"ARMRegression-reader={input.Scenario} passed fresh-native-GameItems native-Body.LoadInventory exact-{input.ExpectedTotal}-quantity title custody");
		return 0;
	}

	private static int RunOrderedNpcRegressionP2(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture, bool stackMergeOnly = false)
	{
		var native = host.Native; var world = native.World; var service = world.SpellOwnedCorpseAnimations!;
		ConfigureRegressionP2Fixture(host, database);
		var source = (Cell)caster.Location; var destination = RegressionP2SecondCell(native, database, source, source.Id + 100, create: true);
		foreach (var actor in source.Characters) actor.SetRoutePosition(25);
		void Expire(ScriptedAiCharacterInstance actor)
		{
			var origin = service.CommandGrant(actor.InstanceId, caster.Id)!;
			var provenance = XElement.Parse(XElement.Parse(origin.Provenance).Element("Source")!.Value);
			var until = DateTime.Parse(provenance.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
			clock.Advance(until - RuntimeClock.UtcNow);
			Require(!service.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied, "Expire only the original order control.");
		}
		void Finish(ScriptedAiCharacterInstance actor)
		{
			using (CommandExecutionScope.EnterIndependent()) actor.Teleport(source, RoomLayer.GroundLevel, false, false, source.RouteDefinition?.DefaultPositionMetres);
			// Route-aware corpse restoration is a later topology adapter. Keep routes active for
			// travel assertions, then normalize this disposable fixture before its ordinary retirement.
			source.ReloadRouteDefinition(null!); destination.ReloadRouteDefinition(null!);
			foreach (var present in source.Perceivables.Concat(destination.Perceivables).ToArray()) present.SetRoutePosition(null);
			world.SaveManager.Flush();
			Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why); restored(actor);
		}
		Action<ICharacter>? operation = null; var invoked = 0;
		var configuredRegression = new System.Collections.Generic.HashSet<CommandableAI>();
		var configuredReady = new System.Collections.Generic.HashSet<CommandableAI>();
		NPCCommandTree.Instance.Commands.Add(["checkpointregression"], new Command<ICharacter>((actor, _) => { ++invoked; operation!(actor); }, states: CharacterState.Awake, name: "CheckpointRegression"));
		void Dispatch(ScriptedAiCharacterInstance actor, Action<ICharacter> action)
		{
			var ai = actor.AIs.OfType<CommandableAI>().Single();
			if (configuredRegression.Add(ai)) Require(ai.BuildingCommand(caster, new StringStack("included checkpointregression")), "Fixture command allowlist failed.");
			operation = action; var before = invoked; order(actor, caster, "checkpointregression"); Require(invoked == before + 1, "Actual AI must dispatch exactly once.");
		}
		foreach (var scenario in stackMergeOnly ? Array.Empty<string>() : new[] { "ordered", "companion", "gap-expire", "direct" })
		{
			var actor = scenario == "ordered" ? animated : cast();
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				source.ReloadRouteDefinition(db.RouteCells.AsNoTracking().Single(x => x.CellId == source.Id));
				destination.ReloadRouteDefinition(db.RouteCells.AsNoTracking().Single(x => x.CellId == destination.Id));
			}
			foreach (var present in source.Characters) present.SetRoutePosition(25);
			var mover = scenario == "direct" ? foe : actor;
			var riders = (System.Collections.Generic.List<ICharacter>)typeof(MudSharp.Character.Character).GetField("_riders", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(mover)!;
			if (scenario == "companion") riders.Add(foe);
			var gaps = 0;
			void Gap(ICharacter moving, ILocation cell)
			{
				if (!ReferenceEquals(moving, mover)) return;
				++gaps;
				Require(ReferenceEquals(moving.Location, source) && moving.RoomLayer == RoomLayer.GroundLevel && moving.RoutePositionMetres == 25 &&
					!source.Characters.Any(x => ReferenceEquals(x, moving)), "Actual native gap must retain the complete source position.");
				if (scenario == "gap-expire") Expire(actor);
			}
			source.OnCharacterLeaves += Gap;
			try
			{
				if (scenario == "direct") mover.Teleport(destination, RoomLayer.InAir, false, false, 150);
				else Dispatch(actor, ch => ch.Teleport(destination, RoomLayer.InAir, false, false, 150));
			}
			finally { source.OnCharacterLeaves -= Gap; riders.Clear(); }
			var arrived = scenario != "gap-expire";
			Require(gaps == 1 && ReferenceEquals(mover.Location, arrived ? destination : source) && mover.RoomLayer == (arrived ? RoomLayer.InAir : RoomLayer.GroundLevel) &&
				mover.RoutePositionMetres == (arrived ? 150 : 25) && (arrived ? destination : source).Characters.Count(x => ReferenceEquals(x, mover)) == 1 &&
				!(arrived ? source : destination).Characters.Any(x => ReferenceEquals(x, mover)), "Native Teleport changed membership/layer/route incorrectly.");
			if (scenario == "companion") Require(ReferenceEquals(foe.Location, destination) && foe.RoomLayer == RoomLayer.InAir && foe.RoutePositionMetres == 150 && destination.Characters.Count(x => ReferenceEquals(x, foe)) == 1, "Actual companion did not arrive coherently.");
			using (CommandExecutionScope.EnterIndependent()) { mover.Teleport(source, RoomLayer.GroundLevel, false, false, 25); foe.Teleport(source, RoomLayer.GroundLevel, false, false, 25); }
			Finish(actor); Console.WriteLine($"ARMRegression-teleport={scenario} passed actual-Character.Cell cross-layer route-gap exact-membership");
		}
		GameItem NewItem(string name, int quantity = 1)
		{
			var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == name).CreateNew(caster);
			if (item.GetItemType<IStackable>() is { } stack) stack.Quantity = quantity;
			world.Add(item); source.Insert(item, true); item.Login(); world.SaveManager.Flush(); return item;
		}
		foreach (var scenario in stackMergeOnly
			? new[] { "ordered-valid", "ordered-parser-valid", "direct-valid", "owner", "delete-expire", "delete-refill", "delete-relocate", "delete-title", "description-expire", "description-refill", "delete-provider-refusal", "delete-provider-outer-refill", "delete-outer-success" }
			: new[] { "owner", "survivor-relocation", "source-relocation", "direct-no-merge" })
		{
			var actor = stackMergeOnly && scenario == "ordered-valid" ? animated : cast(); var getter = scenario is "direct-no-merge" or "direct-valid" ? caster : actor;
			var survivor = NewItem("ARM03B2B stack", 5); var item = NewItem("ARM03B2B stack", 3);
			survivor.SetOwner(caster); item.SetOwner(caster);
			((Body)getter.Body).GetWithoutMerge(survivor);
			Require(getter.Body.HeldItems.Any(x => ReferenceEquals(x, survivor)) && survivor.CanMerge(item), "Prepare an actual compatible held native stack.");
			var callbacks = 0;
			item.OnRemovedFromLocation += _ =>
			{
				++callbacks;
				if (scenario == "owner") survivor.SetOwner(foe);
				if (scenario == "survivor-relocation") { getter.Body.Drop(survivor, silent: true); source.Extract(survivor); destination.Insert(survivor, true); }
				if (scenario == "source-relocation") { item.GetItemType<IHoldable>()!.HeldBy = null; destination.Insert(item, true); }
			};
			var deletionCallbacks = 0;
			var inGet = true;
			Action? dirtyUnrelatedCaller = null;
			var descriptionCallbacks = 0;
			if (stackMergeOnly) survivor.GetItemType<StackableGameItemComponent>()!.DescriptionUpdate += (_, _) =>
			{
				if (!inGet) return;
				++descriptionCallbacks;
				Require(item.Quantity == 0 && survivor.Quantity == 8, "Description observers must see already conserved native stack quantities.");
				if (scenario == "description-expire") { Expire(actor); getter.Body.Drop(survivor, silent: true); Require(getter.Body.HeldItems.Any(x => ReferenceEquals(x, survivor)), "Expired reentrant description Drop must refuse."); }
				if (scenario == "description-refill") { using var independent = CommandExecutionScope.EnterIndependent(); item.GetItemType<IStackable>()!.Quantity = 2; }
			};
			if (stackMergeOnly) item.OnDeleted += _ =>
			{
				++deletionCallbacks;
				if (!inGet) return;
				Require(item.Quantity == 0 && survivor.Quantity == 8, "Deletion observers must see already conserved native stack quantities.");
				dirtyUnrelatedCaller?.Invoke();
				if (scenario == "delete-expire")
				{
					Expire(actor); getter.Body.Drop(survivor, silent: true);
					Require(getter.Body.HeldItems.Any(x => ReferenceEquals(x, survivor)), "Reentrant Drop must still refuse expired command authority.");
				}
				using var independent = CommandExecutionScope.EnterIndependent();
				if (scenario == "delete-refill") item.GetItemType<IStackable>()!.Quantity = 2;
				if (scenario == "delete-relocate") { item.GetItemType<IHoldable>()!.HeldBy = null; destination.Insert(item, true); }
				if (scenario == "delete-title") item.SetOwner(foe);
			};
			Exception? providerRefusal = null;
			IGameItem? acquired = null;
			IGameItem? eventItem = null; var getEvents = 0;
			MudSharp.Body.InventoryChangeEvent getObserver = (_, state, changed) =>
			{
				if (state != MudSharp.Body.InventoryState.Held) return;
				++getEvents; eventItem = changed;
			};
			getter.Body.OnInventoryChange += getObserver;
			// The survivor is saved during merging; use an untouched item for dirty caller work.
			var callerUnrelatedItem = scenario is "delete-provider-outer-refill" or "delete-outer-success" ? NewItem("ARM03B2B stack", 1) : null;
			using var caller = callerUnrelatedItem is not null ? new FMDB() : null;
			var callerContext = caller is not null ? FMDB.Context : null;
			Db.GameItem? unrelatedCallerRow = null;
			if (callerContext is not null)
			{
				var tracked = callerContext.GameItems.Include(x => x.GameItemComponents).Include(x => x.CellsGameItems).Single(x => x.Id == item.Id);
				Require(tracked.GameItemComponents.Count == 2 && tracked.CellsGameItems.Single().CellId == source.Id, "Pretrack the live source's native component and exact floor graph.");
				unrelatedCallerRow = callerContext.GameItems.Find(callerUnrelatedItem!.Id)!;
				dirtyUnrelatedCaller = () =>
				{
					unrelatedCallerRow.Condition = 0.75;
					callerContext.Entry(unrelatedCallerRow).Property(x => x.Condition).IsModified = true;
				};
			}
			if (scenario is "delete-provider-refusal" or "delete-provider-outer-refill")
			{
				using var db = NewIndependentContext(database.ConnectionString);
				db.Database.ExecuteSqlRaw($"CREATE TRIGGER arm_stack_delete_refusal BEFORE DELETE ON GameItems FOR EACH ROW BEGIN IF OLD.Id = {item.Id} THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'owned stack deletion fixture refusal'; END IF; END");
			}
			void Get(ICharacter ch)
			{
				try { acquired = ch.Body.Get(item, 0, null, true, MudSharp.GameItems.ItemCanGetIgnore.None, []); }
				catch (Exception ex) when (scenario is "delete-provider-refusal" or "delete-provider-outer-refill" && ex.ToString().Contains("owned stack deletion fixture refusal")) { providerRefusal = ex; }
			}
			if (scenario == "direct-no-merge") ((Body)getter.Body).GetWithoutMerge(item);
			else if (scenario == "direct-valid") Get(getter);
			else if (scenario == "ordered-parser-valid") { order(actor, caster, "get stack"); acquired = eventItem; }
			else Dispatch(actor, Get);
			getter.Body.OnInventoryChange -= getObserver;
			inGet = false;
			if (scenario is "delete-provider-refusal" or "delete-provider-outer-refill")
			{
				Require(providerRefusal is not null, "Actual owned SQL trigger must refuse the absorbed DELETE.");
				if (callerContext is not null)
				{
					Require(ReferenceEquals(FMDB.Context, callerContext), "Immediate deletion must restore the outer caller context.");
					using var independent = CommandExecutionScope.EnterIndependent();
					item.GetItemType<IStackable>()!.Quantity = 2;
				}
				using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm_stack_delete_refusal");
			}
			if (stackMergeOnly && scenario != "owner")
			{
				var retained = scenario is "delete-refill" or "delete-relocate" or "delete-title" or "description-refill" or "delete-provider-refusal" or "delete-provider-outer-refill";
				Require(callbacks == 1 && descriptionCallbacks == 1 && deletionCallbacks == (scenario == "description-refill" ? 0 : 1) && item.Deleted == !retained && !survivor.Deleted &&
					survivor.Quantity == 8 && item.Quantity == (scenario is "delete-refill" or "description-refill" or "delete-provider-outer-refill" ? 2 : 0),
					$"Successful native Get must conserve 5+3 once and clean only the unchanged absorbed stack: {scenario}, source={item.Quantity}/{item.Deleted}, survivor={survivor.Quantity}.");
				Require(scenario is "delete-provider-refusal" or "delete-provider-outer-refill" ? acquired is null && getEvents == 0 : ReferenceEquals(acquired, survivor) && getEvents == 1 && ReferenceEquals(eventItem, survivor),
					"Native Get must return and notify its live survivor exactly once, with no success event after a provider exception.");
				Require(getter.Body.HeldItems.Count(x => ReferenceEquals(x, survivor)) == 1 &&
					!getter.Body.HeldItems.Any(x => ReferenceEquals(x, item)), "Merge must retain one survivor and no absorbed hand membership.");
				Require(!retained || (scenario == "delete-relocate" ? ReferenceEquals(item.DirectLocation, destination) && item.InInventoryOf is null :
					ReferenceEquals(item.DirectLocation, source) && item.InInventoryOf is null), "Cleanup destroyed callback-established source custody.");
				Require(survivor.OwnershipReference == new ItemOwnershipReference(caster.FrameworkItemType, caster.Identity.Id) &&
					item.OwnershipReference == new ItemOwnershipReference(caster.FrameworkItemType, scenario == "delete-title" ? foe.Identity.Id : caster.Identity.Id), "Merge cleanup changed native title.");
			}
			else
			{
			Require(callbacks == 1 && !item.Deleted && !survivor.Deleted && item.Quantity == 3 && survivor.Quantity == 5 &&
				survivor.OwnershipReference == new ItemOwnershipReference(caster.FrameworkItemType, scenario == "owner" ? foe.Identity.Id : caster.Identity.Id) &&
				item.OwnershipReference == new ItemOwnershipReference(caster.FrameworkItemType, caster.Identity.Id),
				"Removal callback must preserve both native quantities and titles without merging.");
			Require(scenario == "direct-no-merge" ? ReferenceEquals(item.GetItemType<IHoldable>()!.HeldBy, getter.Body) :
				ReferenceEquals(item.DirectLocation, scenario == "source-relocation" ? destination : source) && item.InInventoryOf is null, "Accepted Get lost source custody or failed safe fallback.");
			Require(scenario == "survivor-relocation" ? ReferenceEquals(survivor.DirectLocation, destination) && survivor.InInventoryOf is null :
				ReferenceEquals(survivor.GetItemType<IHoldable>()!.HeldBy, getter.Body), "Accepted Get destroyed callback-established survivor custody.");
			}
			world.SaveManager.Flush();
			if (callerContext is not null)
			{
				callerContext.SaveChanges();
				Require(callerContext.Entry(unrelatedCallerRow!).State != EntityState.Detached && ReferenceEquals(callerContext.GameItems.Find(callerUnrelatedItem!.Id), unrelatedCallerRow),
					"Immediate deletion must preserve the caller's unrelated tracked item row.");
				Require(Same(unrelatedCallerRow!.Condition, 0.75),
					"Immediate deletion must preserve the unrelated caller's dirty field.");
				Require(!callerContext.ChangeTracker.Entries<Db.GameItem>().Any(x => x.Entity.Id == item.Id && x.State == EntityState.Deleted),
					"Failed absorbed deletion must not remain pending in the caller context after independent refill and flush.");
				if (scenario == "delete-provider-outer-refill") Require(!item.Deleted && item.Quantity == 2 && survivor.Quantity == 8 && ReferenceEquals(item.DirectLocation, source) && item.InInventoryOf is null &&
					getter.Body.HeldItems.Count(x => ReferenceEquals(x, survivor)) == 1 && !getter.Body.HeldItems.Any(x => ReferenceEquals(x, item)),
					"Independent refill and later caller flush must preserve both live stacks and their exact runtime custody.");
				else
				{
					Require(!callerContext.ChangeTracker.Entries<Db.GameItem>().Any(x => x.Entity.Id == item.Id) &&
						!callerContext.ChangeTracker.Entries<Db.GameItemComponent>().Any(x => x.Entity.GameItemId == item.Id),
						"Successful deletion must evict only the source's cached item/component graph.");
					using (var absent = NewIndependentContext(database.ConnectionString)) Require(!absent.GameItems.Any(x => x.Id == item.Id) &&
						!absent.GameItemComponents.Any(x => x.GameItemId == item.Id) && !absent.CellsGameItems.Any(x => x.GameItemId == item.Id) &&
						!absent.BodiesGameItems.Any(x => x.GameItemId == item.Id), "Successful independent deletion must remove the exact durable source graph.");
					var loader = ArchiveRoots();
					SetPrivateField(loader, "_items", host.Items);
					SetPrivateField(loader, "_bootTimeCachedGameItems", new System.Collections.Generic.Dictionary<long, Db.GameItem>());
					var count = loader.Items.Count();
					Require(!loader.Items.Has(item.Id) && loader.TryGetItem(item.Id, true) is null && !loader.Items.Has(item.Id) && loader.Items.Count() == count,
						"Actual Futuremud.TryGetItem in the restored caller must return null and register no absorbed ghost.");
					Console.WriteLine("ARMRegression-outer-success=passed actual-Futuremud.TryGetItem absent-row no-registration exact-source-graph-eviction unrelated-caller-entry-retained");
				}
			}
			var saved = new[] { survivor, item }.Select(x => new RegressionP2SavedStack(x.Id, x.Quantity, x.OwnershipReference,
				x.GetItemType<IHoldable>()!.HeldBy?.Id, x.GetItemType<IHoldable>()!.HeldBy is null ? null : getter.Identity.Id, x.DirectLocation?.Id, x.Deleted)).ToArray();
			RunItemReaderProcess(new RegressionP2Reader(database.Name, fixture, RuntimeClock.UtcNow, destination.Id, saved, scenario, scenario is "delete-refill" or "description-refill" or "delete-provider-outer-refill" ? 10 : 8,
				callerUnrelatedItem?.Id, callerUnrelatedItem is null ? null : 0.75), "--regression-p2-reader");
			if (scenario == "delete-provider-refusal")
			{
				using (CommandExecutionScope.EnterIndependent()) item.Delete();
				Require(item.Deleted && deletionCallbacks == 1 && survivor.Quantity == 8, "Failed native deletion must retry without replaying observers or crediting units twice.");
				var retry = new[] { saved[0], saved[1] with { Deleted = true, Cell = null, Body = null, Character = null } };
				RunItemReaderProcess(new RegressionP2Reader(database.Name, fixture, RuntimeClock.UtcNow, destination.Id, retry, "delete-provider-retry"), "--regression-p2-reader");
			}
			// The outer stack-call fixture ends before unrelated corpse retirement.
			caller?.Dispose();
			callerUnrelatedItem?.Delete();
			foreach (var stack in new[] { survivor, item }.Where(x => !x.Deleted)) { if (stack.GetItemType<IHoldable>()!.HeldBy is { } body) body.Take(stack); stack.Delete(); }
			Finish(actor); Console.WriteLine($"ARMRegression-get={scenario} passed real-Body.Get removal-callback exact-{(scenario is "delete-refill" or "description-refill" or "delete-provider-outer-refill" ? 10 : 8)}-quantity title runtime-custody native-save cold-native-reader deletion-observers:{deletionCallbacks}");
		}
		if (stackMergeOnly) return 0;
		Mock.Get(native.Body.Race).SetupGet(x => x.RaceUsesStamina).Returns(true);
		Mock.Get(world.GetCheck(CheckType.CombatRecoveryCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.CombatRecoveryCheck, Outcome.Pass));
		typeof(CombatBase).GetProperty("RecoveryTimeExpression", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, new TraitExpression("1", world));
		foreach (var scenario in new[] { "ordered", "chamber-refusal", "direct", "ordinary-refusal", "empty" })
		{
			var actor = cast(); var readier = scenario is "direct" or "ordinary-refusal" ? foe : actor;
			var gunItem = NewItem("ARMRegression gun"); var round = NewItem("ARMRegression round");
			// Preserve the corpse owner's unrelated gear, while giving this fixture two free hands.
			var unrelated = readier.Body.HeldItems.ToArray(); foreach (var gear in unrelated) readier.Body.Drop(gear, silent: true);
			((Body)readier.Body).GetWithoutMerge(gunItem); ((Body)readier.Body).GetWithoutMerge(round);
			var gun = gunItem.GetItemType<InternalMagazineGunGameItemComponent>()!;
			if (scenario == "empty") readier.Body.Drop(round, silent: true);
			else gun.Load(readier);
			Require(scenario == "empty" || gun.MagazineContents.Single() == round && ReferenceEquals(round.ContainedIn, gunItem) && round.GetItemType<IHoldable>()!.HeldBy is null, "Actual native gun.Load must adopt its sole round.");
			order(actor, caster, "hit opponent"); actor.RemoveAllEffects<IdleCombatant>(fireRemovalAction: true);
			var combat = actor.Combat!;
			if (!ReferenceEquals(readier, actor)) { if (!ReferenceEquals(readier.Combat, combat)) combat.JoinCombat(readier); readier.CombatTarget = actor; }
			((Body)readier.Body).CurrentStamina = 100;
			ReadyRangedWeaponMove move;
			if (ReferenceEquals(readier, actor))
			{
				if (configuredReady.Add(actor.AIs.OfType<CommandableAI>().Single())) Require(actor.AIs.OfType<CommandableAI>().Single().BuildingCommand(caster, new StringStack("included ready")), "Allowlist actual Ready command.");
				order(actor, caster, "ready gun"); move = (ReadyRangedWeaponMove)actor.ChooseMove();
			}
			else move = new ReadyRangedWeaponMove { Assailant = readier, Weapon = gun };
			var ai = actor.AIs.OfType<CommandableAI>().Single();
			var policyField = typeof(CommandableAI).GetField("_canCommandProg", BindingFlags.NonPublic | BindingFlags.Instance)!;
			var policy = (IFutureProg)policyField.GetValue(ai)!; var chamberFaults = 0;
			var fault = new Mock<IFutureProg>();
			fault.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns<object[]>(arguments =>
			{
				if (scenario == "chamber-refusal" && new StackTrace().GetFrames().Any(x => x.GetMethod()?.Name == "ChamberRound")) { ++chamberFaults; Expire(actor); }
				return policy.ExecuteBool(arguments);
			});
			// Capture the callback-bearing policy through an actual new ordered Ready dispatch.
			if (scenario == "chamber-refusal")
			{
				policyField.SetValue(ai, fault.Object); order(actor, caster, "ready gun"); move = (ReadyRangedWeaponMove)actor.ChooseMove();
			}
			if (scenario == "ordinary-refusal") { readier.Body.Drop(gunItem, silent: true); source.Extract(gunItem); destination.Insert(gunItem, true); }
			try { combat.CombatAction(readier, move); }
			finally { policyField.SetValue(ai, policy); }
			var accepted = scenario is "ordered" or "direct" or "empty";
			Require(readier.CurrentStamina == (accepted ? 93 : 100) && move.UsesStaminaWithResult(accepted ? new CombatMoveResult { MoveWasSuccessful = true } : CombatMoveResult.Irrelevant) == accepted,
				"Actual CombatAction must debit exactly seven stamina only for accepted readying.");
			Require(scenario != "chamber-refusal" || chamberFaults == 1, "Fault must occur in actual chamber admission.");
			Require(ReferenceEquals(gunItem.GetItemType<IHoldable>()!.HeldBy, scenario == "ordinary-refusal" ? null : readier.Body) &&
				ReferenceEquals(gunItem.DirectLocation, scenario == "ordinary-refusal" ? destination : null) && gunItem.ContainedIn is null, "Actual gun lost its final native custody.");
			Require(ReferenceEquals(gun.ChamberedRound?.Parent, accepted && scenario != "empty" ? round : null) &&
				gun.MagazineContents.Count() == (accepted || scenario == "empty" ? 0 : 1) && round.Quantity == 1 && !round.Deleted,
				"Refusal/acceptance must preserve actual chamber outcome, ammunition and custody.");
			Require(round.GetItemType<IHoldable>()!.HeldBy is null && (scenario == "empty"
				? ReferenceEquals(round.DirectLocation, source) && round.ContainedIn is null && round.InInventoryOf is null
				: ReferenceEquals(round.ContainedIn, gunItem) && round.DirectLocation is null && ReferenceEquals(round.InInventoryOf, gunItem.InInventoryOf)), "Actual ammunition lost its final native custody.");
			using (CommandExecutionScope.EnterIndependent())
			{
				if (ReferenceEquals(readier.Combat, combat) && !ReferenceEquals(readier, actor)) combat.LeaveCombat(readier);
				if (gunItem.GetItemType<IHoldable>()!.HeldBy is { } holder) holder.Take(gunItem);
				gunItem.Delete(); if (!round.Deleted) round.Delete();
				foreach (var gear in unrelated) ((Body)readier.Body).GetWithoutMerge(gear);
			}
			Finish(actor); Console.WriteLine($"ARMRegression-ready={scenario} passed native-InternalMagazine.Load actual-ReadyMove CombatAction exact-stamina admission-outcome ammo-custody");
		}
		return 0;
	}
}
