#nullable enable

using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	// Opt-in duplicate of the protected reader sequence; diagnostic reads do not repair state.
	private static int RunOrderedSandReaderDiagnostics(string[] args)
	{
		var input = JsonSerializer.Deserialize<SandKnifeReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime);
		using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true);
		ConfigureStormHands(host.Native); FinaliseStormTags(database, host.Native.World);
		var world = host.Native.World; var service = new SpellOwnedItemService(world);
		host.Native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(service);
		var item = (GameItem)world.TryGetItem(input.Item, true)!;
		IGameItem? bag = null;
		void Observe(string stage)
		{
			using var observer = NewIndependentContext(database.ConnectionString);
			var lifecycle = host.Store.Find(input.Lifecycle)!;
			var sibling = observer.GameItems.AsNoTracking().SingleOrDefault(x => x.Id == input.Sibling);
			Console.WriteLine("ARMSand-removal-diagnostic=" + JsonSerializer.Serialize(new {
				stage, item = item.Id, item.Deleted, item.Destroyed, lifecycle.State, lifecycle.Diagnostic,
				ownedRowExists = observer.GameItems.Any(x => x.Id == item.Id),
				directCell = item.DirectLocation?.Id, inheritedCell = item.Location?.Id,
				container = item.ContainedIn?.Id, heldBy = item.GetItemType<IHoldable>()?.HeldBy?.Id,
				siblingExists = sibling is not null, siblingContainer = sibling?.ContainerId,
				bagExists = observer.GameItems.Any(x => x.Id == input.Bag),
				bagContents = bag?.GetItemType<IContainer>()?.Contents.Select(x => x.Id).ToArray()
			}));
		}
		Require(item.SpellCreationOrigin?.DeadlineUtc == input.Deadline && item.SpellCreationOrigin.IsTemporary && !host.Items.Has(input.Bag), "Fresh loading lost deadline or preloaded custodian.");
		Observe("loaded");
		service.ReconcileRetirements(RuntimeClock.UtcNow); Require(!item.Deleted, "Fresh process expired output early.");
		Observe("before-expiry");
		clock.Advance(input.Deadline - RuntimeClock.UtcNow); service.ReconcileRetirements(RuntimeClock.UtcNow);
		Require(!item.Deleted && host.Store.Find(input.Lifecycle)!.State == SpellLifecycleState.Retiring, "Unloaded foreign custodian did not hold removal.");
		Observe("unloaded-custodian-hold");
		bag = world.TryGetItem(input.Bag, true)!; bag.FinaliseLoadTimeTasks();
		var permanent = world.TryGetItem(input.Permanent, true)!;
		Require(permanent.Prototype.Id == input.PermanentPrototype && permanent.SpellCreationOrigin is { IsTemporary: false, DeadlineUtc: null } && ReferenceEquals(permanent.ContainedIn, bag), "Fresh reload lost permanent staff prototype, lifetime or foreign custody.");
		Require(ReferenceEquals(item.ContainedIn, bag), "Native foreign bag did not reconnect exact sand-knife.");
		Observe("custodian-loaded");
		service.ReconcileRetirements(RuntimeClock.UtcNow); Observe("reconcile-first");
		service.ReconcileRetirements(RuntimeClock.UtcNow); Observe("reconcile-second");
		world.SaveManager.Flush(); Observe("flushed");
		using var db = NewIndependentContext(database.ConnectionString);
		Require(item.Deleted, "Diagnostic reader: exact owned leaf is not Deleted.");
		Require(host.Store.Find(input.Lifecycle)!.State == SpellLifecycleState.Completed, "Diagnostic reader: lifecycle is not Completed.");
		Require(db.GameItems.Find(input.Sibling)?.ContainerId == input.Bag, "Diagnostic reader: foreign sibling custody changed.");
		Require(db.GameItems.Any(x => x.Id == input.Bag), "Diagnostic reader: foreign bag row missing.");
		clock.Advance(TimeSpan.FromDays(30)); service.ReconcileRetirements(RuntimeClock.UtcNow);
		Require(!permanent.Deleted && db.GameItems.Any(x => x.Id == input.Permanent) && db.GameItems.Find(input.Permanent)!.ContainerId == input.Bag, "Permanent output expired or changed foreign custody after restart.");
		var definition = XElement.Parse(db.MagicSpells.AsNoTracking().Single(x => x.Id == input.Spell).Definition);
		Require(definition.Element("StockIdentity")?.Value == ArmageddonSandKnifeStock.Key && definition.Element("Effects")!.Element("Effect")!.Element("Lifecycle")!.Element("Seconds")!.Value == ArmageddonSandKnifeStock.LifetimeSeconds, "Persisted stock formula drifted.");
		Console.WriteLine("ARMSand-diagnostic-reader=passed unchanged-four-predicates exact-owned-leaf lifecycle foreign-sibling-and-bag");
		return 0;
	}
}
