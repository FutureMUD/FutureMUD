#nullable enable

using System;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using Moq;
using MudSharp.FutureProg;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.NPC.AI;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed class CustodyGetCallback : Effect, IRemoveOnGet
	{
		private readonly Action _callback;
		public CustodyGetCallback(GameItem owner, Action callback) : base(owner) => _callback = callback;
		protected override string SpecificEffectType => "CheckpointCustodyGet";
		public override string Describe(IPerceiver voyeur) => "Disposable custody callback";
		public override void RemovalEffect() => _callback();
	}

	private sealed class CustodyMergeGate : Effect
	{
		private readonly Action _callback;
		public CustodyMergeGate(GameItem owner, Action callback) : base(owner) => _callback = callback;
		protected override string SpecificEffectType => "CheckpointCustodyMergeGate";
		public override string Describe(IPerceiver voyeur) => "Disposable merge eligibility callback";
		public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
		{
			_callback();
			return false;
		}
	}

	private static int RunCustodyMerge(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture)
	{
		var world = host.Native.World;
		ConfigureRegressionP2Fixture(host, database);
		var floor = (Room)caster.Location;
		var destination = RegressionP2SecondRoom(host.Native, database, floor, floor.Id + 100, true);
		floor.ReloadRouteDefinition(null!); destination.ReloadRouteDefinition(null!);
		foreach (var present in floor.Perceivables.Concat(destination.Perceivables).ToArray()) present.SetRoutePosition(null);
		var service = world.SpellOwnedCorpseAnimations!;
		void Expire(ScriptedAiCharacterInstance actor)
		{
			var grant = service.CommandGrant(actor.InstanceId, caster.Id)!;
			var provenance = XElement.Parse(XElement.Parse(grant.Provenance).Element("Source")!.Value);
			clock.Advance(DateTime.Parse(provenance.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture,
				DateTimeStyles.RoundtripKind) - RuntimeClock.UtcNow);
			Require(!service.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied, "Expire only the originating command grant.");
		}
		GameItem NewItem(string name, int quantity = 1)
		{
			var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == name).CreateNew(caster);
			if (item.GetItemType<IStackable>() is { } stack) stack.Quantity = quantity;
			world.Add(item); floor.Insert(item, true); item.Login(); world.SaveManager.Flush();
			return item;
		}
		var first = true;
		foreach (var kind in new[] { "container", "give" })
		foreach (var scenario in new[] { "authority-ancestor", "authority-reach", "preflight-relocate", "partial-preflight-refill", "ordered-valid", "direct-valid", "callback-refuse", "callback-relocate", "delete-expire", "delete-refill" })
		{
			if (kind == "give" && scenario == "authority-ancestor") continue;
			var actor = first ? animated : cast(); first = false;
			var giver = scenario == "direct-valid" ? caster : actor;
			var receiver = kind == "give" ? foe : giver;
			var survivor = NewItem("ARM03B2B stack", 5);
			var item = NewItem("ARM03B2B stack", 3);
			survivor.SetOwner(caster); item.SetOwner(caster);
			((Body)receiver.Body).GetWithoutMerge(survivor);
			GameItem? bag = kind == "container" ? NewItem("ARM03B2B bag") : null;
			if (bag is not null)
			{
				floor.Extract(item); bag.GetItemType<IContainer>()!.Put(giver, item, false);
				Require(ReferenceEquals(item.ContainedIn, bag) && bag.GetItemType<IContainer>()!.Contents.Single() == item &&
					item.DirectLocation is null, "Prepare actual native container source.");
			}
			else ((Body)giver.Body).GetWithoutMerge(item);
			Require(receiver.Body.HeldItems.Contains(survivor) && survivor.CanMerge(item), "Prepare compatible native survivor.");
			world.SaveManager.Flush();
			var alternate = bag is not null && scenario == "preflight-relocate" ? NewItem("ARM03B2B bag") : null;
			var ancestor = scenario == "authority-ancestor" ? NewItem("ARM03B2B bag") : null;
			if (ancestor is not null)
			{
				bag!.OverrideSdesc = "an innerbag"; ancestor.OverrideSdesc = "an outerbag";
				floor.Extract(bag); ancestor.GetItemType<IContainer>()!.Put(giver, bag, false);
				Require(ancestor.GetItemType<IOpenable>()!.CanClose(giver.Body) && bag.GetItemType<IOpenable>()!.IsOpen, "Declare closable native ancestor with open nested bag.");
			}
			var prepared = false; var relocated = false;
			var eligibility = 0; var removed = 0; var deletions = 0; var heldEvents = 0;
			IGameItem? announced = null;
			InventoryChangeEvent observe = (_, state, changed) => { if (state == InventoryState.Held) { ++heldEvents; announced = changed; } };
			receiver.Body.OnInventoryChange += observe;
			if (scenario is "authority-reach" or "authority-ancestor") survivor.AddEffect(new CustodyMergeGate(survivor, () =>
			{ if (new StackTrace().GetFrames().Any(x => x.GetMethod()?.Name == "PrepareGetPlacement")) prepared = true; }));
			if (scenario == "callback-refuse") survivor.AddEffect(new CustodyMergeGate(survivor, () => { if (++eligibility == 1) Expire(actor); }));
			if (scenario == "partial-preflight-refill") survivor.AddEffect(new CustodyMergeGate(survivor, () =>
			{
				if (!new StackTrace().GetFrames().Any(x => x.GetMethod()?.Name == "PrepareGetPlacement") || eligibility != 0) return;
				++eligibility;
				using var independent = CommandExecutionScope.EnterIndependent();
				item.GetItemType<IStackable>()!.Quantity = 4;
			}));
			if (scenario == "preflight-relocate") survivor.AddEffect(new CustodyMergeGate(survivor, () =>
			{
				if (!new StackTrace().GetFrames().Any(x => x.GetMethod()?.Name == "PrepareGetPlacement") || eligibility != 0) return;
				++eligibility;
				using var independent = CommandExecutionScope.EnterIndependent();
				if (bag is not null)
				{
					bag.GetItemType<IContainer>()!.Take(giver, item, 0);
					alternate!.GetItemType<IContainer>()!.Put(giver, item, false);
				}
				else { giver.Body.Take(item); ((Body)caster.Body).GetWithoutMerge(item); }
			}));
			if (scenario == "callback-relocate") item.AddEffect(new CustodyGetCallback(item, () =>
			{
				++removed;
				using var independent = CommandExecutionScope.EnterIndependent();
				item.GetItemType<IHoldable>()!.HeldBy = null; destination.Insert(item, true);
			}));
			var refusedDrop = kind == "give" && scenario == "delete-expire" ? giver.Body.HeldItems.First(x => !ReferenceEquals(x, item)) : survivor;
			var running = true;
			item.OnDeleted += _ =>
			{
				if (!running) return;
				++deletions;
				Require(item.Quantity == 0 && survivor.Quantity == 8, "Deletion callbacks must see conserved quantities.");
				if (scenario == "delete-expire")
				{
					Expire(actor); giver.Body.Drop(refusedDrop, silent: true);
					Require(giver.Body.HeldItems.Contains(refusedDrop), "Originating callback must not escape expired authority through its original body.");
				}
				if (scenario == "delete-refill")
				{
					using var independent = CommandExecutionScope.EnterIndependent();
					item.GetItemType<IStackable>()!.Quantity = 2;
				}
			};
			var ai = actor.AIs.OfType<CommandableAI>().Single();
			var policyField = typeof(CommandableAI).GetField("_canCommandProg", BindingFlags.Instance | BindingFlags.NonPublic)!;
			var policy = (IFutureProg)policyField.GetValue(ai)!;
			if (scenario is "authority-reach" or "authority-ancestor")
			{
				var fault = new Mock<IFutureProg>();
				fault.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns<object[]>(arguments =>
				{
					var frames = new StackTrace().GetFrames();
					if (prepared && !relocated && !frames.Any(x => x.GetMethod()?.Name is "PrepareGetPlacement" or "CanGet" or "CanGive") &&
						frames.Any(x => x.GetMethod()?.DeclaringType == typeof(Body) && x.GetMethod()?.Name == (bag is null ? "GivePhysicalItem" : "Get")))
					{
						relocated = true;
						using var independent = CommandExecutionScope.EnterIndependent();
						if (ancestor is not null) ancestor.GetItemType<IOpenable>()!.Close();
						else if (bag is not null) { floor.Extract(bag); destination.Insert(bag, true); }
						else receiver.Teleport(destination, RoomLayer.GroundLevel, false, false);
						Require(service.CanCommand(actor.InstanceId, caster.Id), "Reach fault must retain the original grant.");
					}
					return policy.ExecuteBool(arguments);
				});
				policyField.SetValue(ai, fault.Object);
			}
			try
			{
				if (scenario == "direct-valid")
				{
					if (bag is not null) giver.Body.Get(item, bag, silent: true);
					else giver.Body.Give(item, receiver.Body);
				}
				else order(actor, caster, ancestor is not null ? "get stack outerbag@innerbag" : bag is not null ? scenario == "partial-preflight-refill" ? "get 2 stack bag" : "get stack bag" : scenario == "partial-preflight-refill" ? "give 2 stack opponent" : "give stack opponent");
			}
			finally { policyField.SetValue(ai, policy); }
			receiver.Body.OnInventoryChange -= observe; running = false;
			var accepted = scenario is "ordered-valid" or "direct-valid" or "delete-expire" or "delete-refill";
			Require(survivor.Quantity == (accepted ? 8 : 5) && item.Quantity == (accepted ? scenario == "delete-refill" ? 2 : 0 : scenario == "partial-preflight-refill" ? 4 : 3) &&
				item.Deleted == (accepted && scenario != "delete-refill") && !survivor.Deleted,
				$"Custody merge failed exact source/survivor totals: {kind}/{scenario}, source={item.Quantity}/{item.Deleted}, survivor={survivor.Quantity}.");
			Require(heldEvents == (accepted ? 1 : 0) && (!accepted || ReferenceEquals(announced, survivor)), "Notify only the live acquired survivor once.");
			Require(deletions == (accepted ? 1 : 0) && (scenario is not ("callback-refuse" or "preflight-relocate" or "partial-preflight-refill") || eligibility > 0) &&
				removed == (scenario == "callback-relocate" ? 1 : 0), "Expected native eligibility/removal/deletion callback was not exercised.");
			Require(receiver.Body.HeldItems.Count(x => ReferenceEquals(x, survivor)) == 1 &&
				!receiver.Body.HeldItems.Contains(item) && (kind != "give" || accepted || scenario is "callback-relocate" or "preflight-relocate" or "partial-preflight-refill" or "authority-reach" || giver.Body.HeldItems.Contains(item)),
				"Merge left wrong survivor/source body membership.");
			Require(scenario != "authority-reach" || prepared && relocated && (bag is not null ? ReferenceEquals(bag.DirectLocation, destination) : ReferenceEquals(receiver.Location, destination)), "Actual final authority callback must establish independent remote custody.");
			Require(scenario != "authority-ancestor" || prepared && relocated && !ancestor!.GetItemType<IOpenable>()!.IsOpen && bag!.GetItemType<IOpenable>()!.IsOpen, "Actual final policy must close only the outer native ancestor.");
			if (!item.Deleted)
				Require(scenario == "preflight-relocate" ? alternate is null ? ReferenceEquals(item.GetItemType<IHoldable>()!.HeldBy, caster.Body) && caster.Body.HeldItems.Contains(item) : ReferenceEquals(item.ContainedIn, alternate) && alternate.GetItemType<IContainer>()!.Contents.Contains(item) :
					scenario is "callback-refuse" or "partial-preflight-refill" or "authority-reach" or "authority-ancestor" ? bag is null ? ReferenceEquals(item.GetItemType<IHoldable>()!.HeldBy, giver.Body) : ReferenceEquals(item.ContainedIn, bag) :
					ReferenceEquals(item.DirectLocation, scenario == "callback-relocate" ? destination : floor) && item.InInventoryOf is null,
					"Refusal/independent callback/fallback lost exact source custody.");
			Require(item.OwnershipReference == survivor.OwnershipReference && survivor.OwnershipReference == new ItemOwnershipReference(caster.FrameworkItemType, caster.Identity.Id), "Transfer changed native title.");
			world.SaveManager.Flush();
			var saved = new[] { survivor, item }.Select(x => new RegressionP2SavedStack(x.Id, x.Quantity, x.OwnershipReference,
				x.GetItemType<IHoldable>()!.HeldBy?.Id, x.GetItemType<IHoldable>()!.HeldBy?.Actor.Identity.Id, x.DirectLocation?.Id, x.Deleted, x.ContainedIn?.Id, x.GetItemType<IHoldable>()!.HeldBy?.Actor.Location?.Id)).ToArray();
			RunItemReaderProcess(new RegressionP2Reader(database.Name, fixture, RuntimeClock.UtcNow, destination.Id, saved,
				$"custody-{kind}-{scenario}", scenario == "delete-refill" ? 10 : scenario == "partial-preflight-refill" ? 9 : 8, ContainerItem: bag?.Id, OtherContainerItem: alternate?.Id, ContainerRoom: bag?.Location?.Id, AncestorItem: ancestor?.Id), "--regression-p2-reader");
			using (CommandExecutionScope.EnterIndependent())
			{
				foreach (var stack in new[] { survivor, item }.Where(x => !x.Deleted)) { stack.GetItemType<IHoldable>()!.HeldBy?.Take(stack); stack.Delete(); }
				bag?.Delete(); alternate?.Delete(); ancestor?.Delete();
				if (!ReferenceEquals(foe.Location, floor)) foe.Teleport(floor, RoomLayer.GroundLevel, false, false);
				Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why); restored(actor);
			}
			Console.WriteLine($"ARMCustody={kind}/{scenario} passed native-command/body exact-{(scenario == "delete-refill" ? 10 : scenario == "partial-preflight-refill" ? 9 : 8)}-total title callback-custody survivor-events:{heldEvents} cold-reader");
		}
		return 0;
	}
}
