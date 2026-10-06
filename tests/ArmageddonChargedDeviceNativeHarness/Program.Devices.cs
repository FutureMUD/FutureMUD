using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;
using MudSharp.Framework.Scheduling;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static class ChargedDeviceEntryPoint
{
	public static int Main(string[] args) => GNHProgram.DeviceMain(args);
}

internal static partial class GNHProgram
{
	private sealed record DeviceReader(string Database, FixtureIds Fixture, long Item, long Spell, Guid Completed,
		Guid Pending, double Balance, int Charges, double Raw, bool Failure = false);
	internal static int DeviceMain(string[] args)
	{
		try
		{
			OwnedConnections.Install();
			return args.FirstOrDefault() switch { "--device-reader" => ReadDeviceNative(args.Skip(1).Single()), "--device-review-host" => ReadReviewDeviceHost(args.Skip(1).Single()), "--device-quarantine-reader" => ReadDeviceQuarantine(args.Skip(1).Single()), _ => RunDeviceNative() };
		}
		catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
	}
	private static void DevicePrototypes(RetirementHost host, TestDatabase database)
	{
		var world = host.Native.World;
		using var db = NewIndependentContext(database.ConnectionString);
		var previous = world.ItemComponentProtos;
		var components = db.GameItemComponentProtos.Include(x => x.EditableItem).AsNoTracking()
			.Where(x => x.Name.StartsWith("ARM03B2B") || x.Name.StartsWith("ARMDEV"))
			.ToDictionary(x => (x.Id, x.RevisionNumber), x => x.Type == "ChargedMagicDevice" ? (IGameItemComponentProto)new ChargedMagicDeviceGameItemComponentProto(x, world) : previous.Get(x.Id, x.RevisionNumber));
		var catalogue = new Mock<IUneditableRevisableAll<IGameItemComponentProto>>();
		catalogue.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>())).Returns<long, int>((id, revision) => components.GetValueOrDefault((id, revision))!);
		catalogue.Setup(x => x.GetEnumerator()).Returns(() => components.Values.GetEnumerator());
		host.Native.WorldMock.SetupGet(x => x.ItemComponentProtos).Returns(catalogue.Object);
		foreach (var model in db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosTags)
			.Include(x => x.GameItemProtosGameItemComponentProtos).Where(x => x.Name.StartsWith("ARMDEV")))
			host.Prototypes[model.Id] = new GameItemProto(model, world);
	}
	private static int RunDeviceNative()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"ARMDEV-created={database.Name} process:{Environment.ProcessId}");
		var fixture = FixtureSeed.Create(database, "armdev_charged", false);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id);
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var world = native.World; var actor = native.Actor;
		Mock.Get(native.Body.Race).Setup(x => x.GetMaximumLiftWeight(It.IsAny<ICharacter>())).Returns(10000);
		var capability = (SkillLevelBasedMagicCapability)native.Capability;
		var skill = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var spell = new MagicSpell("ARMDEV Native Blindness", capability.School); ((All<IMagicSpell>)world.MagicSpells).Add(spell);
		foreach (var command in new[] { "trigger new self", $"trait {skill.Id}", "difficulty easy", "threshold minorpass",
			"duration ARM02 Duration", $"cost {native.Resource.Id} ARM02 Cost", $"prog {world.FutureProgs.GetByName("rejuvenation_known")!.Id}",
			"castemote A charged veil forms.", "failcastemote The charged veil fails.", "grades fixture", "effect add blindness" })
			Require(spell.BuildingCommand(actor, new StringStack(command)), $"Device spell builder refused {command}");
		foreach (var command in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive",
			$"casting entry add {spell.Id}", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(capability.BuildingCommand(actor, new StringStack(command)), $"Device route refused {command}");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(capability)]); world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		var store = new MagicCastingStateStore();
		var service = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var enrolled = service.Enrol(staff.Object, actor, capability.Id, "ARMDEV legitimate native setup"); Require(enrolled.Allowed, enrolled.Message);
		store.Write(acquired: service.Acquisition(actor, spell.Id)! with { ControlledGrade = 2 });
		actor.SetTraitValue(skill, 60); actor.AddResource(native.Resource, 100); FlushCasting(native);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var component = new Db.GameItemComponentProto { Id = db.GameItemComponentProtos.Max(x => x.Id) + 1,
				Name = "ARMDEV Wand", Type = "ChargedMagicDevice", Description = "Owned native charge fixture",
				Definition = $"<Definition version='1'><Kind>0</Kind><Role>2</Role><Eligibility>1</Eligibility><Capacity>5</Capacity><Seconds>1</Seconds><Spell>{spell.Id}</Spell><Plan><Phase/></Plan></Definition>",
				EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			db.GameItemComponentProtos.Add(component); db.SaveChanges();
			var prototype = new Db.GameItemProto { Id = db.GameItemProtos.Max(x => x.Id) + 1, Name = "ARMDEV Wand", Keywords = "charged wand",
				ShortDescription = "an owned charged wand", FullDescription = "Owned native fixture", MaterialId = world.Materials.First().Id,
				Size = 1, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard, MorphTimeSeconds = 0, MorphEmote = "$0 changes.",
				EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			prototype.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id });
			prototype.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = db.GameItemComponentProtos.Single(x => x.Type == "Holdable" && x.Name.StartsWith("ARM03B2B")).Id });
			db.GameItemProtos.Add(prototype); db.SaveChanges();
		}
		DevicePrototypes(host, database);
		var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARMDEV Wand").CreateNew(actor);
		world.Add(item); actor.Location.Insert(item, true); item.Login(); world.SaveManager.Flush(); native.Body.Get(item, silent: true);
		Require(native.Body.HeldItems.Contains(item), "Native device must be physically held.");
		var device = (ChargedMagicDeviceGameItemComponent)item.GetItemType<IChargedMagicDevice>();
		Console.WriteLine($"ARMDEV-physical held:{native.Body.HeldOrWieldedItems.Contains(item)} visible:{actor.CanSee(item)} manipulate:{actor.CanManipulateItem(item)} deleted:{item.Deleted}");
		var start = service.BeginDeviceProduction(actor, item, capability.Id, spell.Id, 2, 3); Require(start.Status == MagicCastingStatus.Started, start.Message);
		clock.Advance(TimeSpan.FromSeconds(3));
		var complete = service.CompleteDeviceProduction(actor, start.OperationId!.Value); Require(complete.Status == MagicCastingStatus.Succeeded, complete.Message);
		Require(device.Charges == 3 && actor.MagicResourceAmounts[native.Resource] == 70 && !actor.EffectsOfType<SpellBlindnessEffect>().Any(), "Native paid target-free production mismatch.");
		ChargedMagicDeviceGameItemComponent stale;
		using (var db = NewIndependentContext(database.ConnectionString)) stale = new(db.GameItemComponents.AsNoTracking().Single(x => x.Id == device.Id), (ChargedMagicDeviceGameItemComponentProto)device.Prototype, item);
		var raw = actor.TraitRawValue(skill); var acquired = store.Acquisition(actor.Id, spell.Id)!;
		var activation = service.ActivateDevice(actor, item, "self"); Require(activation.Status == MagicCastingStatus.Succeeded, activation.Message);
		Require(device.Charges == 2 && actor.EffectsOfType<SpellBlindnessEffect>().Any() && actor.MagicResourceAmounts[native.Resource] == 70 &&
			actor.TraitRawValue(skill) == raw && store.Acquisition(actor.Id, spell.Id) == acquired && store.Opportunity(actor.Id, skill.Id) is null,
			"Native release changed personal progress/payment or missed its status payload.");
		actor.RemoveAllEffects(x => x is SpellBlindnessEffect, true); FlushCasting(native);
		Console.WriteLine("ARMDEV-production-release=passed real-Character/Body/item/component/proficiency/reserve native-status-effect frozen-duration no-Vancian-enrolment no-charge-use-progress");
		Require(stale.Reserve(Guid.NewGuid()), "Stale bank fixture reservation failed.");
		Refuse(() => service.PersistDevice(stale), "A stale database bank overwrote a consumed charge.");
		using (var db = NewIndependentContext(database.ConnectionString)) Require(db.GameItemComponents.AsNoTracking().Single(x => x.Id == device.Id).Definition == device.PersistedDefinition, "CAS refusal changed durable bank bytes.");
		Console.WriteLine("ARMDEV-bank-CAS=passed stale-native-component-refused durable-consumed-bank-byte-preservation");
		var tombstone = store.Operation(activation.OperationId!.Value)!;
		Require(!MagicDeviceJournal.TryClaim(tombstone with { Definition = "forbidden-overwrite", Stage = "DeviceConsuming" }) && store.Operation(tombstone.Id) == tombstone, "Existing native tombstone was replaced.");
		var concurrentId = Guid.NewGuid();
		using var barrier = new Barrier(2);
		var results = Enumerable.Range(0, 2).Select(n => Task.Run(() =>
		{
			barrier.SignalAndWait(); return MagicDeviceJournal.TryClaim(tombstone with { Id = concurrentId, Definition = "claim-" + n });
		})).ToArray(); Task.WaitAll(results);
		Require(results.Count(x => x.Result) == 1, "Concurrent database inserts did not admit exactly one claim.");
		Console.WriteLine("ARMDEV-insert-only=passed two-concurrent-independent-database-connections one-winner duplicate-key-refusal terminal-tombstone-byte-preservation");
		ReviewDeviceNative(database, fixture, host, clock, service, staff.Object, spell, capability);
		native.Body.Get(item, silent: true);
		// An independent empty carrier exercises actual frozen damage and heal, while the first bank stays homogeneous.
		native.Body.Take(item); actor.Location.Insert(item, true);
		var auxiliary = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARMDEV Wand").CreateNew(actor);
		world.Add(auxiliary); actor.Location.Insert(auxiliary, true); auxiliary.Login(); world.SaveManager.Flush(); native.Body.Get(auxiliary, silent: true);
		var auxiliaryBank = (ChargedMagicDeviceGameItemComponent)auxiliary.GetItemType<IChargedMagicDevice>();
		foreach (var effect in new[] { "damage", "heal" })
		{
			var payload = new MagicSpell(spell, "ARMDEV Native " + effect); ((All<IMagicSpell>)world.MagicSpells).Add(payload);
			foreach (var command in new[] { "effect remove 1", "effect add " + effect, "effect 1 formula " + (effect == "damage" ? "variable/10+grade" : "20") })
				Require(payload.BuildingCommand(actor, new StringStack(command)), "Native payload builder refused " + command);
			Require(capability.BuildingCommand(actor, new StringStack($"casting entry add {payload.Id}")), "Native payload admission failed.");
			Require(((ChargedMagicDeviceGameItemComponentProto)auxiliaryBank.Prototype).BuildingCommand(actor, new StringStack($"spell add {payload.Id}")), "Native carrier whitelist failed.");
			world.SaveManager.Flush(); var grant = service.Grant(staff.Object, actor, capability.Id, payload.Id, "Native device payload fixture"); Require(grant.Allowed, grant.Message);
			store.Write(acquired: service.Acquisition(actor, payload.Id)! with { ControlledGrade = 2 });
			var production = service.BeginDeviceProduction(actor, auxiliary, capability.Id, payload.Id, 2, 1); Require(production.Status == MagicCastingStatus.Started, production.Message);
			clock.Advance(TimeSpan.FromSeconds(1)); var made = service.CompleteDeviceProduction(actor, production.OperationId!.Value); Require(made.Status == MagicCastingStatus.Succeeded, made.Message);
			var damageBefore = actor.Wounds.Sum(x => x.CurrentDamage); var balanceBefore = actor.MagicResourceAmounts[native.Resource];
			if (effect == "damage") actor.SetTraitValue(skill, 5);
			var released = service.ActivateDevice(actor, auxiliary, "self"); Require(released.Status == MagicCastingStatus.Succeeded, released.Message);
			var damageAfter = actor.Wounds.Sum(x => x.CurrentDamage);
			Require(auxiliaryBank.Charges == 0 && actor.MagicResourceAmounts[native.Resource] == balanceBefore &&
				(effect == "damage" ? Math.Abs(damageAfter - damageBefore - 8) < 0.00001 : damageAfter < damageBefore), "Native damage/heal did not change real wounds with frozen potency.");
			actor.SetTraitValue(skill, raw); FlushCasting(native);
			Console.WriteLine($"ARMDEV-{effect}=passed real-native-wounds before:{damageBefore} after:{damageAfter} frozen-producer-variable no-activation-debit");
		}
		native.Body.Take(auxiliary); actor.Location.Insert(auxiliary, true); native.Body.Get(item, silent: true); FlushCasting(native);
		var faultService = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, flush: () => FlushCasting(native),
			checkpoint: boundary => { if (boundary == "DeviceConsumed") throw new InvalidOperationException("ARMDEV injected failure after durable charge removal"); });
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(faultService);
		var failed = faultService.ActivateDevice(actor, item, "self"); Require(failed.Status == MagicCastingStatus.NeedsReview && device.Charges == 1 && !actor.EffectsOfType<SpellBlindnessEffect>().Any(), "Native uncertain failure replayed its effect or failed to consume once.");
		FlushCasting(native);
		RunDeviceReader(new(database.Name, fixture, item.Id, spell.Id, activation.OperationId.Value, failed.OperationId!.Value, actor.MagicResourceAmounts[native.Resource], 1, raw, true));
		Require(faultService.ReconcileOperation(staff.Object, actor, failed.OperationId.Value, "Native controlled injected failure; charge persisted, no effect replay or refund").Allowed, "Explicit staff reconciliation refused.");
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var pending = service.BeginDeviceProduction(actor, item, capability.Id, spell.Id, 2, 1); Require(pending.Status == MagicCastingStatus.Started, pending.Message);
		FlushCasting(native);
		RunDeviceReader(new(database.Name, fixture, item.Id, spell.Id, activation.OperationId.Value, pending.OperationId!.Value, actor.MagicResourceAmounts[native.Resource], 1, raw));
		Console.WriteLine("ARMDEV-native=passed owned-database-delete-on-dispose no-shared-harness-dispatch-edit");
		return 0;
	}
	private static void RunDeviceReader(DeviceReader descriptor)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--device-reader");
		start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(descriptor))));
		using var process = Process.Start(start)!; var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Owned device reader timed out."); }
		Require(process.ExitCode == 0, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult()); Console.Write(output.GetAwaiter().GetResult());
	}
	private static int ReadDeviceNative(string argument)
	{
		var input = JsonSerializer.Deserialize<DeviceReader>(Encoding.UTF8.GetString(Convert.FromBase64String(argument)))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true); DevicePrototypes(host, database);
		var native = host.Native; var actor = native.Actor; var world = native.World;
		var item = world.TryGetItem(input.Item, true)!; var device = (ChargedMagicDeviceGameItemComponent)item.GetItemType<IChargedMagicDevice>();
		var store = new MagicCastingStateStore(); var service = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, flush: () => FlushCasting(native));
		Require(device.DataError is null && device.Charges == input.Charges && device.Reservation == (input.Failure ? (Guid?)null : input.Pending) &&
			actor.MagicResourceAmounts[native.Resource] == input.Balance && store.Operation(input.Completed)!.Stage == "Completed" &&
			store.Operation(input.Pending)!.Stage == (input.Failure ? "NeedsReview" : "DeviceProducing"), "Independent restart lost the exact bank/paid receipt or refunded payment.");
		Require(!MagicDeviceJournal.TryClaim(store.Operation(input.Completed)! with { Stage = "DeviceConsuming" }), "Independent process reused a completed charge claim.");
		Require(service.CompleteDeviceProduction(actor, input.Pending).Status == MagicCastingStatus.Refused &&
			service.ActivateDevice(actor, item, "self").Status == MagicCastingStatus.Refused && !actor.EffectsOfType<SpellBlindnessEffect>().Any(), "Restart replayed production, activation or a status effect.");
		Console.WriteLine($"ARMDEV-reader=passed process:{Environment.ProcessId} item:{input.Item} charges:{device.Charges} balance:{input.Balance} failure:{input.Failure} no-resume/refund/replay persisted-reservation/quarantine completed-tombstone");
		return 0;
	}
}
