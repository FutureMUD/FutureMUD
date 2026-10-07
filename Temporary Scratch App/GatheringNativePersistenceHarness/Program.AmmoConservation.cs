#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;
using Moq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void SeedAmmoConservationFixture(TestDatabase database)
	{
		SeedLoadOutputFixture(database);
		SeedAmmoAftermathFixture(database);
		using var db = NewIndependentContext(database.ConnectionString);
		var gun = db.GameItemComponentProtos.Single(x => x.Name == "ARMRegression InternalMagazineGun");
		var xml = XElement.Parse(gun.Definition); xml.Element("InternalMagazineCapacity")!.Value = Environment.GetEnvironmentVariable("FUTUREMUD_AMMO_ACCEPTANCE_CASE") == "split-throw" ? "2" : "8";
		gun.Definition = xml.ToString(); db.SaveChanges();
	}

	private static int RunAmmoConservation(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture)
	{
		ConfigureRegressionP2Fixture(host, database);
		var world = host.Native.World;
		host.Native.WorldMock.Setup(x => x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(item =>
		{ if (!host.Items.Any(x => ReferenceEquals(x, item))) host.Items.Add(item); });
		typeof(Body).GetField("_encumbranceLimitExpression", BindingFlags.Static | BindingFlags.NonPublic)!
			.SetValue(null, new MudSharp.Body.Traits.TraitExpression("1000", world));
		var trait = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var configured = new HashSet<CommandableAI>();
		var cases = new[] { "load-valid", "load-refill", "load-rebind", "load-refused", "load-direct", "unload-valid", "unload-refill", "unload-rebind", "unload-refused", "unload-direct", "split-throw", "split-valid", "load-provider-refusal", "unload-provider-refusal", "load-provider-outer-refill", "unload-provider-outer-refill", "unload-outer-success", "unload-later-expire", "unload-prototype-change" };
		var selected = Environment.GetEnvironmentVariable("FUTUREMUD_AMMO_ACCEPTANCE_CASE");
		var selectedCases = selected?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
		if (selected?.StartsWith("aftermath", StringComparison.Ordinal) == true) return RunAmmoAftermath(database, host, clock, animated, caster, foe, cast, restored, order, fixture);
		Require(string.IsNullOrEmpty(selected) || selectedCases.All(cases.Contains), "Select only a declared disposable ammunition case.");
		var first = true;
		foreach (var scenario in cases.Where(x => string.IsNullOrEmpty(selected) || selectedCases.Contains(x)))
		{
			var actor = first ? animated : cast(); first = false; var body = (Body)actor.Body;
			var ai = actor.AIs.OfType<CommandableAI>().Single();
			if (configured.Add(ai))
				foreach (var action in new[] { "load", "unload" }) Require(ai.BuildingCommand(caster, new StringStack("included " + action)), "Declare extended test whitelist; stock excludes load/unload.");
			var unrelated = body.HeldItems.ToArray(); foreach (var item in unrelated) body.Drop(item, silent: true);
			GameItem New(string name, int quantity = 1)
			{
				var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == name).CreateNew(caster);
				if (item.GetItemType<IStackable>() is { } stack) stack.Quantity = quantity;
				world.Add(item); actor.Location.Insert(item, true); item.Login(); item.SetOwner(caster); world.SaveManager.Flush(); return item;
			}
			var gunItem = New("ARMRegression gun"); var gun = gunItem.GetItemType<InternalMagazineGunGameItemComponent>()!;
			var gunPrototype = (MudSharp.GameItems.Prototypes.InternalMagazineGunGameItemComponentProto)gun.Prototype;
			var previousCapacity = gunPrototype.InternalMagazineCapacity;
			// All guns share this fixture prototype; each scenario must set its own capacity.
			gunPrototype.InternalMagazineCapacity = scenario.StartsWith("split-", StringComparison.Ordinal) ? 2 : 8;
			Console.WriteLine($"ARMAmmo-fixture={scenario} before:{previousCapacity} capacity:{gunPrototype.InternalMagazineCapacity}");
			if (scenario.StartsWith("split-", StringComparison.Ordinal))
			{
				var sourceRound = New("ARMRegression round", 3);
				Require(ReferenceEquals(body.GetWithoutMerge(gunItem), gunItem) && ReferenceEquals(body.GetWithoutMerge(sourceRound), sourceRound), "Acquire split-exception native participants.");
				var clones = new List<GameItem>(); var expected = new InvalidOperationException("native split description callback");
				host.Native.WorldMock.Setup(x => x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(item =>
				{
					if (!host.Items.Any(x => ReferenceEquals(x, item))) host.Items.Add(item);
					if (item is not GameItem clone || ReferenceEquals(clone, sourceRound) || !ReferenceEquals(clone.Prototype, sourceRound.Prototype)) return;
					clones.Add(clone); if (scenario == "split-throw") clone.GetItemType<StackableGameItemComponent>()!.DescriptionUpdate += (_, _) => throw expected;
				});
				try { order(actor, caster, "load gun"); if (scenario == "split-throw") throw new ApplicationException("Expected exact native split callback exception."); }
				catch (InvalidOperationException error) { Require(ReferenceEquals(error, expected), "Preserve the original native split exception."); }
				finally { host.Native.WorldMock.Setup(x => x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(item => { if (!host.Items.Any(x => ReferenceEquals(x, item))) host.Items.Add(item); }); }
				Console.WriteLine($"ARMAmmo-split source:{sourceRound.Quantity} clones:{string.Join(',', clones.Select(x => x.Quantity))} floor:{clones.Count(x => actor.Location.GameItems.Contains(x))}");
				Require(clones.Count == 1 && sourceRound.Quantity == 1 && clones[0].Quantity == 2 && (scenario == "split-throw" ? !gun.MagazineContents.Any() : gun.MagazineContents.SequenceEqual(clones)) &&
					ReferenceEquals(sourceRound.GetItemType<IHoldable>()!.HeldBy, body) && clones[0].GetItemType<IHoldable>()!.HeldBy is null &&
					(scenario == "split-throw" ? clones[0].ContainedIn is null && clones[0].DirectLocation == actor.Location && actor.Location.GameItems.Contains(clones[0]) : clones[0].ContainedIn == gunItem && clones[0].DirectLocation is null) &&
					clones[0].OwnershipReference == sourceRound.OwnershipReference, "Thrown partial Get preserves exact native residual1 + floor split2, title and total3.");
				body.CurrentStamina = 100; Require(actor.SetTraitValue(trait, 40), "Persist split reader baseline."); world.SaveManager.Flush();
				var splitItems = new[] { gunItem, sourceRound, clones[0] }.Select(x => new FirearmSavedItem(x.Id, x.OwnershipReference,
					x.GetItemType<IHoldable>()!.HeldBy?.Id, x.DirectLocation?.Id, x.ContainedIn?.Id, body.WieldedItems.Contains(x), x.Quantity)).ToArray();
				RunItemReaderProcess(new FirearmAuthorityReader(database.Name, fixture, RuntimeClock.UtcNow, ((ICharacter)actor.GetTrait(trait).Owner).Id,
					body.Id, actor.CurrentStamina, trait.Id, actor.TraitRawValue(trait), foe.Body.Id, foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun),
					gunItem.Id, gunItem.Condition, null, gun.MagazineContents.Select(x => x.Id).ToArray(), splitItems, "ammo-" + scenario), "--firearm-authority-reader");
				Console.WriteLine($"ARMAmmo={scenario} passed exact-3-total captured-native-split original-exception cold-reader extended-test-whitelist");
				using (CommandExecutionScope.EnterIndependent())
				{
					body.Take(gunItem); gunItem.Delete(); body.Take(sourceRound); sourceRound.Delete(); clones[0].Delete();
					Require(world.SpellOwnedCorpseAnimations!.TryRetire(actor.InstanceId, MudSharp.Magic.SpellRetirementReason.Dismissal, out var why), why);
					foreach (var item in unrelated) body.GetWithoutMerge(item); world.SaveManager.Flush(); restored(actor);
				}
				continue;
			}
			var unload = scenario.StartsWith("unload", StringComparison.Ordinal);
			var source = New("ARMRegression round", 3); var survivor = New("ARMRegression round", 5);
			Require(ReferenceEquals(body.GetWithoutMerge(gunItem), gunItem), "Acquire the exact native gun.");
			var preloaded = unload ? source : survivor;
			Require(ReferenceEquals(body.GetWithoutMerge(preloaded), preloaded), "Acquire exact native preload round."); gun.Load(actor);
			Require(gun.MagazineContents.Single() == preloaded && preloaded.ContainedIn == gunItem, "Preload the exact source without merge.");
			var extra = scenario == "unload-later-expire" ? New("ARMRegression round", 1) : null;
			if (extra is not null) { extra.SetOwner(foe); Require(ReferenceEquals(body.GetWithoutMerge(extra), extra), "Acquire distinct-title later magazine participant."); gun.Load(actor); }
			var incoming = unload ? survivor : source;
			Require(ReferenceEquals(body.GetWithoutMerge(incoming), incoming), "Hold exact incoming/body-survivor stack without merge.");
			var provider = scenario.Contains("provider", StringComparison.Ordinal);
			var outer = scenario.Contains("outer", StringComparison.Ordinal);
			var callerItem = outer ? New("ARMRegression round", 1) : null;
			using var caller = outer ? new FMDB() : null;
			var callerContext = outer ? FMDB.Context : null;
			var callerRow = outer ? callerContext!.GameItems.Find(callerItem!.Id) : null;
			if (outer) { var tracked = callerContext!.GameItems.Include(x => x.GameItemComponents).Include(x => x.BodiesGameItems).Include(x => x.RoomsGameItems).Single(x => x.Id == source.Id); Require(tracked.GameItemComponents.Count == 3, "Pretrack the exact native ammo source graph."); }
			Exception? providerFailure = null;
			if (provider) { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw($"CREATE TRIGGER arm_ammo_delete_refusal BEFORE DELETE ON GameItems FOR EACH ROW BEGIN IF OLD.Id = {source.Id} THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'owned ammo deletion fixture refusal'; END IF; END"); }
			var deleted = 0; var events = 0; var operating = false; IGameItem? eventItem = null; IGameItem[]? returned = null;
			void Expire()
			{
				var grant = world.SpellOwnedCorpseAnimations!.CommandGrant(actor.InstanceId, caster.Id)!;
				var provenance = XElement.Parse(XElement.Parse(grant.Provenance).Element("Source")!.Value);
				clock.Advance(DateTime.Parse(provenance.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) - RuntimeClock.UtcNow);
				Require(!world.SpellOwnedCorpseAnimations.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied, "Expire only the original command grant.");
			}
			source.OnDeleted += _ =>
			{
				if (!operating) return;
				++deleted; if (outer) { callerRow!.Condition = 0.75; callerContext!.Entry(callerRow).Property(x => x.Condition).IsModified = true; } Require(source.Quantity == 0 && survivor.Quantity == 8, "Native ammunition deletion observers see conserved 5+3 exactly once.");
				using var independent = CommandExecutionScope.EnterIndependent();
				if (!provider && scenario.EndsWith("refill", StringComparison.Ordinal)) source.GetItemType<IStackable>()!.Quantity = 2;
				if (scenario == "load-rebind") actor.Location.Insert(source, true);
			};
			if (scenario == "unload-rebind") source.AddEffect(new CustodyGetCallback(source, () =>
			{ using var independent = CommandExecutionScope.EnterIndependent(); source.GetItemType<IHoldable>()!.HeldBy = null; actor.Location.Insert(source, true); }));
			InventoryChangeEvent observer = (_, state, changed) => { if (state == InventoryState.Held) { ++events; eventItem = changed; if (scenario == "unload-later-expire" && events == 1) Expire(); } };
			var mergeCalls = 0; var prototypeChanges = 0;
			if (scenario == "unload-prototype-change") survivor.AddEffect(new CustodyMergeGate(survivor, () =>
			{
				++mergeCalls;
				if (!new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(Body) &&
					x.GetMethod()?.Name == "CompleteGetPlacementWithResult")) return;
				Require(++prototypeChanges == 1 && source.ContainedIn is null && source.DirectLocation is null &&
					ReferenceEquals(source.GetItemType<IHoldable>()!.HeldBy, body) && !body.HeldOrWieldedItems.Contains(source),
					"Replace the prototype only at the actual post-detach final merge predicate, with exact provisional holder.");
				using var independent = CommandExecutionScope.EnterIndependent();
				var oldProto = source.GetItemType<AmmunitionGameItemComponent>()!.Prototype;
				using var db = NewIndependentContext(database.ConnectionString);
				var row = db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == oldProto.Id && x.RevisionNumber == oldProto.RevisionNumber);
				var replacement = (MudSharp.GameItems.Prototypes.AmmunitionGameItemComponentProto)Activator.CreateInstance(typeof(MudSharp.GameItems.Prototypes.AmmunitionGameItemComponentProto), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { row, world }, null)!;
				typeof(MudSharp.Framework.Revision.EditableItem).GetProperty("Status")!.SetValue(oldProto, MudSharp.Framework.Revision.RevisionStatus.Obsolete);
				Mock.Get(world.ItemComponentProtos).Setup(x => x.GetEnumerator()).Returns(() => new List<IGameItemComponentProto> { replacement }.GetEnumerator());
				using (new FMDB()) Require(source.GetItemType<AmmunitionGameItemComponent>()!.CheckPrototypeForUpdate() && ReferenceEquals(source.GetItemType<AmmunitionGameItemComponent>()!.Prototype, replacement), "Actual final CanMerge callback replaces the native ammo prototype identity.");
			}));
			body.OnInventoryChange += observer;
			var originalOutput = actor.OutputHandler; var outputs = 0;
			var output = new Mock<IOutputHandler>(); output.SetupGet(x => x.Perceiver).Returns(actor);
			output.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<IOutput, bool, bool>((message, newline, nopage) =>
			{
				if (scenario.EndsWith("refused", StringComparison.Ordinal) && new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(InternalMagazineGunGameItemComponent) && x.GetMethod()?.Name == (unload ? "Unload" : "Load")))
				{ ++outputs; Expire(); }
				originalOutput.Send(message, newline, nopage);
			}).Returns(true);
			SetPrivateMember(actor, "OutputHandler", output.Object);
			try
			{
				operating = true;

				if (scenario.EndsWith("direct", StringComparison.Ordinal))
				{
					Expire(); using var independent = CommandExecutionScope.EnterIndependent();
					if (unload) returned = gun.Unload(actor).ToArray(); else gun.Load(actor);
				}
				else order(actor, caster, unload ? "unload gun" : "load gun");
			}
			catch (Exception error) when (provider && error.ToString().Contains("owned ammo deletion fixture refusal")) { providerFailure = error; }
			finally { if (provider) { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm_ammo_delete_refusal"); } operating = false; body.OnInventoryChange -= observer; SetPrivateMember(actor, "OutputHandler", originalOutput); }
			Require(provider == (providerFailure is not null), "Actual owned SQL DELETE refusal is retained without reporting success.");
			if (provider) Require(providerFailure!.ToString().Contains("MergeCommittedStack", StringComparison.Ordinal) &&
				!providerFailure.ToString().Contains("GameItem.DeleteNative", StringComparison.Ordinal) &&
				!providerFailure.ToString().Contains("Cell.Insert", StringComparison.Ordinal), "Refusal originates in the conserved kernel DELETE, never a second floor merge/delete.");
			if (outer)
			{
				Require(ReferenceEquals(FMDB.Context, callerContext), "Absorbed deletion restores the outer caller context.");
				if (provider) { using var independent = CommandExecutionScope.EnterIndependent(); source.GetItemType<IStackable>()!.Quantity = 2; }
				callerContext!.SaveChanges();
				Require(callerContext.Entry(callerRow!).State != EntityState.Detached && Same(callerRow!.Condition, 0.75), "Absorbed deletion preserves unrelated caller tracking and dirty work.");
				if (!provider) Require(!callerContext.ChangeTracker.Entries().Any(x => x.Entity is MudSharp.Models.GameItem row && row.Id == source.Id || x.Metadata.GetForeignKeys().Any(key => key.PrincipalEntityType.ClrType == typeof(MudSharp.Models.GameItem) && key.Properties.Count == 1 && x.Property(key.Properties[0].Name).CurrentValue is long id && id == source.Id)), "Successful deletion evicts only the exact cached source graph.");
			}
			var refused = scenario.EndsWith("refused", StringComparison.Ordinal); Require(!refused || outputs == 1, "Expire at one actual admitted component output."); var claimed = scenario is "unload-rebind" or "unload-prototype-change";
			var merged = !refused && !claimed;
			var retained = provider || refused || claimed || scenario.EndsWith("refill", StringComparison.Ordinal) || scenario == "load-rebind";
			Console.WriteLine($"ARMAmmo-custody={scenario} source:{source.Quantity}/{source.Deleted} survivor:{survivor.Quantity} deletes:{deleted} events:{events} source-cell:{source.DirectLocation?.Id} source-holder:{source.GetItemType<IHoldable>()!.HeldBy?.Id} source-parent:{source.ContainedIn?.Id}");
			Require(survivor.Quantity == (merged ? 8 : 5) && source.Quantity == (refused || claimed ? 3 : scenario.EndsWith("refill", StringComparison.Ordinal) ? 2 : 0) &&
				source.Deleted == !retained && !survivor.Deleted && deleted == (merged ? 1 : 0), "Native ammunition merge/Unload conserves exact source and survivor value and cleanup.");
			Require(!merged || (unload ? gun.MagazineContents.Count() == (extra is null ? 0 : 1) && ReferenceEquals(survivor.GetItemType<IHoldable>()!.HeldBy, body) : gun.MagazineContents.Single() == survivor && survivor.ContainedIn == gunItem), "Exact live survivor retains receiving custody.");
			Require(!unload || events == (merged && !provider ? 1 : 0) && (merged && !provider ? ReferenceEquals(eventItem, survivor) : eventItem is null), "Unload publishes only the live survivor once.");
			Require(returned is null || returned.SequenceEqual([survivor]) && !returned.Single().Deleted, "Direct component Unload returns the live survivor rather than absorbed source.");
			Require(!retained || refused || source.DirectLocation == actor.Location && source.ContainedIn is null && source.GetItemType<IHoldable>()!.HeldBy is null, "Independent claimed/refilled source retains exact cell custody.");
			Require(callerItem is null || callerItem.Quantity == 1 && !callerItem.Deleted && !callerItem.Destroyed && callerItem.DirectLocation == actor.Location && callerItem.GetItemType<IHoldable>()!.HeldBy is null && callerItem.ContainedIn is null, "Exact completion never credits or consumes the nearby caller floor stack.");
			body.CurrentStamina = 100; Require(actor.SetTraitValue(trait, 40), "Persist reader baseline."); world.SaveManager.Flush();
			Require(extra is null || gun.MagazineContents.Single() == extra && extra.Quantity == 1 && extra.ContainedIn == gunItem && extra.OwnershipReference == new ItemOwnershipReference(foe.FrameworkItemType, foe.Identity.Id), "First committed receive expiry preserves exact later participant untouched.");
			Require(scenario != "unload-prototype-change" || prototypeChanges == 1 && mergeCalls >= 3, "Prototype replacement runs at the final executable merge predicate once after preparation.");
			void Read(string suffix)
			{
				var live = new[] { gunItem, source, survivor }.Concat(extra is null ? [] : new[] { extra }).Concat(callerItem is null ? [] : new[] { callerItem }).Where(x => !x.Deleted).Select(x => new FirearmSavedItem(x.Id, x.OwnershipReference,
				x.GetItemType<IHoldable>()!.HeldBy?.Id, x.DirectLocation?.Id, x.ContainedIn?.Id, body.WieldedItems.Contains(x), x.Quantity, Condition: ReferenceEquals(x, callerItem) ? 0.75 : null)).ToArray();
			RunItemReaderProcess(new FirearmAuthorityReader(database.Name, fixture, RuntimeClock.UtcNow, ((ICharacter)actor.GetTrait(trait).Owner).Id,
				body.Id, actor.CurrentStamina, trait.Id, actor.TraitRawValue(trait), foe.Body.Id, foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun),
				gunItem.Id, gunItem.Condition, null, gun.MagazineContents.Select(x => x.Id).ToArray(), live, "ammo-" + scenario + suffix, DeletedItems: source.Deleted ? [source.Id] : []), "--firearm-authority-reader");
			}
			Read("-conserved");
			if (provider && !outer)
			{
				using var independent = CommandExecutionScope.EnterIndependent(); source.Delete(); world.SaveManager.Flush();
				Require(source.Deleted && survivor.Quantity == 8 && deleted == 1, "Ordinary zero-source DELETE retry performs no second debit, credit or ammunition callback."); Read("-ordinary-delete-retry");
			}
			Console.WriteLine($"ARMAmmo={scenario} passed exact-{(merged ? scenario.EndsWith("refill", StringComparison.Ordinal) ? 10 : 8 : 8)}-total live-survivor custody native-output/delete/get callbacks cold-reader extended-test-whitelist");
			// The caller-context acceptance ends before unrelated actor retirement cleanup.
			caller?.Dispose();
			using (CommandExecutionScope.EnterIndependent())
			{
				body.Take(gunItem); gunItem.Delete();
				foreach (var item in new[] { source, survivor }.Concat(extra is null ? [] : new[] { extra }).Concat(callerItem is null ? [] : new[] { callerItem }).Where(x => !x.Deleted)) { item.GetItemType<IHoldable>()!.HeldBy?.Take(item); item.Delete(); }
				Require(world.SpellOwnedCorpseAnimations!.TryRetire(actor.InstanceId, MudSharp.Magic.SpellRetirementReason.Dismissal, out var why), why);
				foreach (var item in unrelated) body.GetWithoutMerge(item); world.SaveManager.Flush(); restored(actor);
			}
		}
		return string.IsNullOrEmpty(selected) ? RunAmmoAftermath(database, host, clock, cast(), caster, foe, cast, restored, order, fixture) : 0;
	}
}
