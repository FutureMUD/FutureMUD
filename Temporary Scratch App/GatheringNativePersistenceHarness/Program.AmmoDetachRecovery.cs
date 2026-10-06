#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.NPC.AI;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunAmmoDetachRecovery(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture)
	{
		ConfigureRegressionP2Fixture(host, database);
		var native = host.Native; var world = native.World;
		native.WorldMock.Setup(x => x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(item => { if (!host.Items.Any(x => ReferenceEquals(x, item))) host.Items.Add(item); });
		typeof(Body).GetField("_encumbranceLimitExpression", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.SetValue(null, new MudSharp.Body.Traits.TraitExpression("1000", world));
		var trait = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var configured = new HashSet<CommandableAI>(); var first = true;
		var selected = Environment.GetEnvironmentVariable("FUTUREMUD_AMMO_DETACH_CASE")?.Split(',');
		var scenarios = new[] { "unload-title", "unload-quantity", "unload-throw", "unload-bag-throw", "unload-body-throw", "unload-double-throw", "cycle-title", "cycle-throw", "cycle-bag-throw", "cycle-body-throw", "cycle-double-throw" };
		Require(selected is null || selected.All(scenarios.Contains), "Every requested detach recovery case must exist.");
		foreach (var scenario in scenarios.Where(x => selected is null || selected.Contains(x)))
		{
			var actor = first ? animated : cast(); first = false; var body = (Body)actor.Body;
			var ai = actor.AIs.OfType<CommandableAI>().Single();
			if (configured.Add(ai)) foreach (var action in new[] { "load", "unload", "ready" }) Require(ai.BuildingCommand(caster, new StringStack("included " + action)), "Extended test whitelist explicitly admits native gun operation.");
			var unrelated = body.HeldItems.ToArray(); foreach (var item in unrelated) body.Drop(item, silent: true);
			GameItem New(string name, int quantity = 1)
			{
				var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == name).CreateNew(caster);
				if (item.GetItemType<IStackable>() is { } stack) stack.Quantity = quantity;
				world.Add(item); actor.Location.Insert(item, true); item.Login(); item.SetOwner(caster); world.SaveManager.Flush(); return item;
			}
			var cycle = scenario.StartsWith("cycle", StringComparison.Ordinal);
			var gunItem = New("ARMRegression gun"); var gun = gunItem.GetItemType<InternalMagazineGunGameItemComponent>()!;
			var source = New(cycle ? "ARMRegression aftermath-round" : "ARMRegression round", cycle ? 1 : 3);
			var survivor = cycle ? null : New("ARMRegression round", 5);
			var bag = scenario.Contains("bag", StringComparison.Ordinal) ? New("ARM03B2B bag") : null;
			Require(body.GetWithoutMerge(gunItem) == gunItem && body.GetWithoutMerge(source) == source, "Native receiver holds exact preload participants.");
			gun.Load(actor); Require(source.ContainedIn == gunItem && gun.MagazineContents.Single() == source, "Actual native Load installs exact participant.");
			if (cycle) Require(gun.Ready(actor) && gun.ChamberedRound!.Parent == source && !gun.MagazineContents.Any(), "Actual native Ready establishes one nonstack chambered round.");
			else Require(body.GetWithoutMerge(survivor!) == survivor, "Preserve independent held quantity5 survivor.");
			body.CurrentStamina = 100; Require(actor.SetTraitValue(trait, 40), "Native reader trait baseline."); world.SaveManager.Flush();
			var original = new InvalidOperationException("exact post-slot-clear containment callback " + scenario);
			var secondary = new ApplicationException("exact recovery movement callback " + scenario);
			var callbacks = 0; var recoveries = 0; var operating = false;
			var oldProximity = world.ProximityEventService; var proximity = new Mock<IProximityEventService>();
			proximity.Setup(x => x.BeginChange(It.IsAny<ProximityChangeCause>(), It.IsAny<IPerceivable[]>())).Returns<ProximityChangeCause, IPerceivable[]>((cause, affected) =>
			{
				var batch = new Mock<IProximityChangeBatch>();
				batch.Setup(x => x.Complete()).Callback(() =>
				{
					if (!operating || !affected.Any(x => ReferenceEquals(x, source))) return;
					if (cause == ProximityChangeCause.Movement && scenario.EndsWith("double-throw", StringComparison.Ordinal) && callbacks == 1 && recoveries++ == 0) throw secondary;
					if (cause != ProximityChangeCause.Containment || callbacks != 0 || source.ContainedIn is not null) return;
					Require(cycle ? gun.ChamberedRound is null : !gun.MagazineContents.Any(), "Actual native containment Complete observes exact owner slot already cleared.");
					++callbacks; using var independent = CommandExecutionScope.EnterIndependent();
					if (scenario.EndsWith("title", StringComparison.Ordinal)) source.SetOwner(foe);
					if (scenario == "unload-quantity") { source.GetItemType<IStackable>()!.Quantity = 2; survivor!.GetItemType<IStackable>()!.Quantity = 6; }
					if (bag is not null) bag.GetItemType<IContainer>()!.Put(actor, source);
					if (scenario.Contains("body", StringComparison.Ordinal))
					{
						if (survivor is not null)
						{
							body.Drop(survivor, silent: true);
							Require(survivor.DirectLocation == actor.Location && survivor.InInventoryOf is null && actor.Location.GameItems.Count(x => ReferenceEquals(x, survivor)) == 1, "Independent drop frees a real hand while conserving the quantity5 survivor.");
						}
						Require(body.GetWithoutMerge(source) == source, "Independent body claim installs actual exact native hand membership.");
					}
					if (scenario.Contains("throw", StringComparison.Ordinal)) throw original;
				});
				return batch.Object;
			});
			native.WorldMock.SetupGet(x => x.ProximityEventService).Returns(proximity.Object);
			Exception? observed = null;
			try { operating = true; order(actor, caster, cycle ? "ready gun" : "unload gun"); }
			catch (Exception error) { observed = error; }
			finally { operating = false; native.WorldMock.SetupGet(x => x.ProximityEventService).Returns(oldProximity); }
			Require(callbacks == 1, "One actual post-slot-clear native containment callback.");
			Require(scenario.Contains("throw", StringComparison.Ordinal) ? ReferenceEquals(observed, original) : observed is null, $"Recovery must preserve exact original callback exception even if recovery itself throws: {scenario}; observed:{observed}.");
			if (scenario.EndsWith("double-throw", StringComparison.Ordinal))
			{
				Require(recoveries == 1, "Exercise the secondary native recovery failure once.");
				// Movement completed its exact floor pointer before the observer threw. Complete only
				// that captured floor membership, preserving the original failure receipt above.
				Require(source.DirectLocation == actor.Location && source.InInventoryOf is null && source.ContainedIn is null && !actor.Location.GameItems.Any(x => ReferenceEquals(x, source)), "Secondary exception follows actual native floor pointer placement before membership.");
				using (CommandExecutionScope.EnterIndependent()) actor.Location.Insert(source, true);
				Console.WriteLine($"ARMAmmoDetach-recovery-retry={scenario} explicit-independent-membership-retry after-primary-and-secondary-failures");
			}
			Require(!source.Deleted && source.Quantity + (survivor?.Quantity ?? 0) == (cycle ? 1 : 8) && !gun.MagazineContents.Any() && gun.ChamberedRound is null, "Exact source and independent survivor quantities conserve; no stale gun slot survives.");
			Require(scenario.Contains("body", StringComparison.Ordinal) ? source.GetItemType<IHoldable>()!.HeldBy == body && body.HeldItems.Count(x => ReferenceEquals(x, source)) == 1 && source.DirectLocation is null && source.ContainedIn is null :
				bag is not null ? source.ContainedIn == bag && bag.GetItemType<IContainer>()!.Contents.Count(x => ReferenceEquals(x, source)) == 1 && source.DirectLocation is null && source.GetItemType<IHoldable>()!.HeldBy is null :
				source.DirectLocation == actor.Location && actor.Location.GameItems.Count(x => ReferenceEquals(x, source)) == 1 && source.InInventoryOf is null && source.ContainedIn is null, "Detached source completes floor once; real independent bag/body custody survives recovery.");
			Require(source.OwnershipReference == new ItemOwnershipReference(foe.FrameworkItemType, foe.Identity.Id) == scenario.EndsWith("title", StringComparison.Ordinal), "Post-detach title changes survive.");
			world.SaveManager.Flush();
			var items = new[] { gunItem, source }.Concat(survivor is null ? [] : new[] { survivor }).Concat(bag is null ? [] : new[] { bag }).Select(x => new FirearmSavedItem(x.Id, x.OwnershipReference, x.GetItemType<IHoldable>()!.HeldBy?.Id, x.DirectLocation?.Id, x.ContainedIn?.Id, Quantity: x.Quantity, Prototype: x.Prototype.Id)).ToArray();
			RunItemReaderProcess(new FirearmAuthorityReader(database.Name, fixture, RuntimeClock.UtcNow, ((ICharacter)actor.GetTrait(trait).Owner).Id, body.Id, actor.CurrentStamina, trait.Id, actor.TraitRawValue(trait), foe.Body.Id, foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun), gunItem.Id, gunItem.Condition, null, [], items, "ammo-detach-" + scenario), "--firearm-authority-reader");
			Console.WriteLine($"ARMAmmoDetach={scenario} passed actual-post-slot-clear native-containment-callback quantity:{source.Quantity}+{survivor?.Quantity ?? 0} exact-floor-or-independent-custody original-exception:{ReferenceEquals(observed, original)} secondary-recovery:{recoveries} fresh-native-reader");
			using (CommandExecutionScope.EnterIndependent())
			{
				Require(world.SpellOwnedCorpseAnimations!.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
				foreach (var item in new[] { source, survivor, gunItem, bag }.OfType<GameItem>()) { if (item.GetItemType<IHoldable>()?.HeldBy is { } holder) holder.Take(item); item.Delete(); }
				foreach (var item in unrelated) body.GetWithoutMerge(item); world.SaveManager.Flush(); restored(actor);
			}
		}
		return 0;
	}
}
