#nullable enable

using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.NPC;

namespace MudSharp.RPG.Performances;

public sealed partial class PerformanceService
{
	private void ClaimPerformer(PerformancePlan plan, PerformanceRole role, ICharacter npc)
	{
		var member = CastMember.From(npc);
		if (plan.PerformerReturns.All(r => r.Member != member))
		{
			plan.PerformerReturns.Add(new(member, role.Key, new(npc.Location.Id, npc.RoomLayer), role.ReturnAfterPerformance));
			Save(plan); // Gathering may move the NPC before the first scripted cue.
		}
		PerformanceActions.Claim(npc, plan.RunId);
	}

	private static void ReleaseEarly(PerformancePlan plan, CastMember member, ICharacter npc)
	{
		// Once released mid-show, a former employee/performer cannot be recruited again by cleanup.
		plan.PerformerReturns.RemoveAll(r => r.Member == member);
		PerformanceActions.Release(npc, plan.RunId);
	}

	private void PreparePropReturn(PerformancePlan plan, PerformanceCue cue, ICharacter actor, ICharacter? recipient, IGameItem item)
	{
		if (Work(plan).LeaveProps.Contains(cue.Target)) return;
		var next = cue.Verb == PerformanceVerb.Get ? actor : recipient;
		if (next is null) return;
		var loan = plan.PropReturns.FirstOrDefault(r => r.Item == item.Id);
		var nextInHand = item.ContainedIn is null && ReferenceEquals(item.InInventoryOf, next.Body);
		if (loan is null || loan.Custodian is null && !nextInHand)
		{
			// A player may move a prop between the prompt and the actual pickup.
			var source = BorrowedFrom(cue, actor, item);
			if (source is null)
			{
				if (loan is not null && plan.PropReturns.Remove(loan)) Save(plan);
				return;
			}
			if (loan is null || source.Origin != loan.Origin || source.Container != loan.Container || source.Lender != loan.Lender)
			{
				if (loan is not null) plan.PropReturns.Remove(loan);
				plan.PropReturns.Add(source);
				loan = source;
			}
		}
		var transfer = CastMember.From(next);
		if (loan.TransferTo == transfer) return;
		loan.TransferTo = transfer;
		Save(plan); // Save the expected handoff before running a native action which can fire world events.
	}

	internal static PropReturn? BorrowedFrom(PerformanceCue cue, ICharacter actor, IGameItem item)
	{
		if (actor.Location is null || RootOf(item) is not { } root) return null;
		var lender = root.InInventoryOf?.Actor;
		if (lender is not null && !ReferenceEquals(lender, actor)) return null;
		if (cue.Verb == PerformanceVerb.Get && item.ContainedIn is null && lender is not null) return null;
		if (cue.Verb == PerformanceVerb.Give && (item.ContainedIn is not null || lender is null)) return null;
		if (!actor.CanSee(item) || !actor.CanReachItem(item).Truth) return null;
		if (lender is null && !actor.Location.LayerGameItems(actor.RoomLayer).Contains(root)) return null;
		var origin = new PerformanceMark(actor.Location.Id, actor.RoomLayer);
		return new(cue.Target, item.Id, origin, item.ContainedIn?.Id, lender is null ? null : CastMember.From(lender));
	}

	private static bool AtOrigin(ICharacter actor, PerformanceMark mark) =>
		actor.Location?.Id == mark.Room && actor.RoomLayer == mark.Layer;

	private static bool SafeReturner(ICharacter? npc) =>
		npc is { IsPlayerCharacter: false, Controller: NPCController } && Able(npc) &&
		npc.Party is null && npc.Following is null && npc.RidingMount is null && !npc.Riders.Any() &&
		!npc.CombinedEffectsOfType<IDragParticipant>().Any() && !npc.EffectsOfType<IAffectedByChangeInGuarding>().Any();

	private void ContinueReturns(PerformancePlan plan)
	{
		var expired = DateTime.UtcNow >= plan.CleanupDueUtc;
		var authority = AuthorityProblem(plan);
		var changed = false;
		var busy = new HashSet<CastMember>();
		plan.ReturnProgress.Clear();
		foreach (var loan in plan.PropReturns.ToArray())
		{
			var item = _world.Items.Get(loan.Item);
			var root = RootOf(item);
			var carrier = root?.InInventoryOf?.Actor;
			var member = carrier is null ? null : CastMember.From(carrier);
			string result;
			if (item is null || root is null) result = "unavailable; check it manually.";
			else if (Restored(loan, item)) result = "back at its source.";
			else if (member is null || member != loan.Custodian && member != loan.TransferTo ||
			         !plan.PerformerReturns.Any(r => r.Member == member) || !SafeReturner(carrier) ||
			         item.ContainedIn is not null || !carrier!.Body.HeldOrWieldedItems.Contains(item))
			{
				if (carrier is { IsPlayerCharacter: true } && (member == loan.Custodian || member == loan.TransferTo))
				{
					var mark = plan.Places.FirstOrDefault(x => x.Value == loan.Origin).Key;
					carrier.OutputHandler.Send($"[Performance {plan.Name}] Please return the borrowed {loan.Key}{(mark is null ? "" : $" to its source at {mark}")}.");
				}
				result = "outside the available NPC cast's hands; arrange its return in person.";
			}
			else if (authority is not null) result = "return released because production authority changed.";
			else
			{
				// One destination per mover per heartbeat, even if both hands carry borrowed props.
				if (!busy.Add(member) && !expired) continue;
				PerformanceActions.Claim(carrier, plan.RunId);
				if (!TryReturnProp(plan, loan, item, carrier, expired, out result))
				{
					plan.ReturnProgress.Add($"{loan.Key}: {result}");
					continue;
				}
				busy.Remove(member);
			}
			plan.CleanupReport.Add($"{loan.Key}: {result}");
			plan.PropReturns.Remove(loan);
			changed = true;
		}

		foreach (var duty in plan.PerformerReturns.ToArray())
		{
			if (busy.Contains(duty.Member)) continue;
			var npc = Actor(duty.Member);
			string result;
			if (!expired && authority is null && SafeReturner(npc) &&
			    plan.PropReturns.Any(loan => loan.Lender == duty.Member))
			{
				// Keep the original lender available at a known rendezvous until their property arrives.
				PerformanceActions.Claim(npc!, plan.RunId);
				continue;
			}
			if (!duty.WalkBack) result = "released to normal duties.";
			else if (npc is not null && AtOrigin(npc, duty.Home)) result = "back at the pre-call location.";
			else if (authority is not null || !SafeReturner(npc)) result = "released in place; could not safely return.";
			else if (expired || _world.Rooms.Get(duty.Home.Room) is null) result = "did not reach the pre-call location; released in place.";
			else
			{
				PerformanceActions.Claim(npc!, plan.RunId);
				plan.ReturnProgress.Add($"{duty.Role}: {WalkBack(plan, npc!, duty.Home)}");
				continue;
			}
			if (npc is not null) PerformanceActions.Release(npc, plan.RunId);
			plan.CleanupReport.Add($"{duty.Role}: {result}");
			plan.PerformerReturns.Remove(duty);
			changed = true;
		}
		if (plan.PropReturns.Count == 0 && plan.PerformerReturns.Count == 0)
		{
			plan.CleanupDueUtc = null;
			changed = true;
		}
		if (changed)
		{
			// Inventory, room and effect saves are queued. Commit them before retiring their return records.
			_world.SaveManager.Flush();
			Save(plan);
		}
	}

	private static IGameItem? RootOf(IGameItem? item)
	{
		var seen = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		while (item is { Deleted: false, Destroyed: false } && seen.Add(item))
		{
			if (item.ContainedIn is not { } container) return item;
			item = container;
		}
		return null;
	}

	internal static bool Restored(PropReturn loan, IGameItem item)
	{
		if (RootOf(item) is not { } root) return false;
		var holder = root.InInventoryOf?.Actor;
		if (loan.Lender != (holder is null ? null : CastMember.From(holder)) || item.ContainedIn?.Id != loan.Container)
			return false;
		if (holder is not null) return true;
		return root.Location?.Id == loan.Origin.Room && root.Location.LayerGameItems(loan.Origin.Layer).Contains(root);
	}

	private string WalkBack(PerformancePlan plan, ICharacter npc, PerformanceMark destination)
	{
		if (npc.Movement is not null) return "walking to the return destination.";
		if (npc.Location?.Id == destination.Room)
		{
			if (npc.RoomLayer != destination.Layer && npc.Location.Terrain(npc).TerrainLayers.Contains(destination.Layer) &&
			    npc.CouldTransitionToLayer(destination.Layer) && !npc.Effects.Any(e => e.IsBlockingEffect("movement") || e.IsBlockingEffect("general")))
				FollowingPath.PathTowardsLayer(npc, destination.Layer);
			return "waiting to reach the original layer.";
		}
		PerformanceActions.Try("Travel", npc, _world.Rooms.Get(destination.Room), null, null, null, "", plan.RunId, true, out var reason);
		return reason;
	}

	internal bool TryReturnProp(PerformancePlan plan, PropReturn loan, IGameItem item, ICharacter carrier, bool expired,
		out string result)
	{
		result = "could not reach or use the original destination; arrange its return in person.";
		if (expired) return true;
		var lender = Actor(loan.Lender);
		if (loan.Lender is not null && lender?.Location is null) return true;
		var destination = loan.Origin;
		if (lender is not null)
		{
			// Never track a PC or unrelated lender to another room. They may accept a local handoff.
			if (!AtOrigin(lender, loan.Origin) && !carrier.ColocatedWith(lender) &&
			    (lender.IsPlayerCharacter || plan.PerformerReturns.All(r => r.Member != loan.Lender))) return true;
			destination = new(lender.Location.Id, lender.RoomLayer);
		}
		if (!AtOrigin(carrier, destination))
		{
			result = WalkBack(plan, carrier, destination);
			return false;
		}
		if (carrier.Movement is not null)
		{
			result = "waiting for the carrier to finish moving.";
			return false;
		}
		if (loan.Container is { } containerId)
		{
			var container = _world.Items.Get(containerId);
			if (container is null || RootOf(container) is not { } root || root.InInventoryOf?.Actor != lender ||
			    lender is null && !carrier.Location.LayerGameItems(destination.Layer).Contains(root)) return true;
			if (container.GetItemType<IOpenable>()?.IsOpen == false || !carrier.CanSee(container) ||
			    !carrier.CanReachItem(container).Truth || !carrier.Body.CanPut(item, container, lender, 0, false))
			{
				result = "waiting for access to the original container.";
				return false;
			}
			carrier.Body.Put(item, container, lender, allowLesserAmounts: false);
		}
		else if (lender is not null)
		{
			result = "waiting to hand the prop back to its original holder.";
			if (!carrier.CanSee(lender) || !carrier.Body.CanGive(item, lender.Body)) return false;
			PerformanceActions.Try("Give", carrier, null, null, item, lender, "", plan.RunId, true, out _);
		}
		else
		{
			result = "waiting to set down the prop at its source.";
			if (!carrier.CanPerformManualAction(out _) || !carrier.Body.CanDrop(item, 0)) return false;
			carrier.Body.Drop(item, newStack: true);
		}
		if (!Restored(loan, item)) return false;
		result = "returned to its source.";
		return true;
	}
}
