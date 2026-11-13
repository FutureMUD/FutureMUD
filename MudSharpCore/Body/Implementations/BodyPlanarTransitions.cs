using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Effects.Concrete;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	public void EjectInventoryForPlanarTransition() => EjectInventoryForPlanarTransition([], false);

	public void EjectInventoryForPlanarTransition(System.Collections.Generic.IReadOnlyCollection<long> destinationPlanes,
		bool retainPlaneSpecificInventory = false, bool ejectInventory = true)
	{
		var executor = Actor;
		var originalActiveBody = executor.Body;
		var origin = executor.Location;
		var originLayer = executor.RoomLayer;
		SpatialLocation? originPosition = origin is null ? null : new SpatialLocation(origin, originLayer, executor.RoutePositionMetres);
		void DropAtOrigin(IGameItem item)
		{
			if (item.Deleted || item.Destroyed || item.InInventoryOf is not null || item.ContainedIn is not null || item.Location is not null) return;
			if (item is GameItem native && originPosition is { } point)
			{
				if (!native.TryDropPrepared(point)) return;
			}
			else item.Drop(origin);
			if (item.Deleted || item.Destroyed || item.InInventoryOf is not null || item.ContainedIn is not null || !ReferenceEquals(item.Location, origin)) return;
			item.RoomLayer = originLayer;
			if (origin is not null && origin.RouteDefinition is null) origin.Insert(item);
			else if (originPosition is { } destination) item.InsertAtSpatialLocation(destination);
		}
		bool ActorCurrent() => ReferenceEquals(Actor, executor) && ReferenceEquals(executor.Body, originalActiveBody) &&
			ReferenceEquals(executor.Location, origin) && executor.RoomLayer == originLayer;
		bool RootCurrent(IGameItem item) => ActorCurrent() && !item.Deleted && !item.Destroyed &&
			item.ContainedIn is null && ReferenceEquals(item.InInventoryOf, this) &&
			DirectItems.Any(x => ReferenceEquals(x, item));
		var roots = _heldItems.Select(x => x.Item1)
			.Concat(_wieldedItems.Select(x => x.Item1))
			.Concat(_wornItems.Select(x => x.Item))
			.Distinct<IGameItem>(ReferenceEqualityComparer.Instance).ToArray();
		var retained = retainPlaneSpecificInventory
			? roots.Where(x => x.Effects.OfType<IPlaneSpecificInventoryRetention>().Any(a =>
				destinationPlanes.Contains(a.PlaneId) && a.IsReadyForTransition)).ToHashSet<IGameItem>(ReferenceEqualityComparer.Instance)
			: new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);

		// On return to a different plane, retained gear loses only its ethereal sight flag.
		foreach (var item in roots)
		{
			if (!ActorCurrent()) return;
			if (!RootCurrent(item)) continue;
			foreach (var affinity in item.EffectsOfType<EtherealItemAffinityEffect>().ToArray())
			{
				bool CanChange() => RootCurrent(item) && item.Effects.Any(x => ReferenceEquals(x, affinity));
				if (!destinationPlanes.Contains(affinity.PlaneId) || ejectInventory && !retained.Contains(item))
					affinity.SetEtherealVisibility(false, CanChange);
				else if (retained.Contains(item))
					affinity.SetEtherealVisibility(true, CanChange);
			}
		}
		if (!ejectInventory) return;

		// Traverse the original root trees before dropping roots. A ready child remains in its immediate parent,
		// including a parent that is itself dropped; non-ready children are detached and dropped in the origin room.
		if (retainPlaneSpecificInventory)
		{
			var visited = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
			bool TreeCurrent(IGameItem root, IGameItem parent)
			{
				if (!ActorCurrent() || root.Deleted || root.Destroyed || root.ContainedIn is not null ||
					!ReferenceEquals(root.InInventoryOf, this) || !DirectItems.Any(x => ReferenceEquals(x, root))) return false;
				var path = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
				for (var current = parent; current is not null && path.Add(current); current = current.ContainedIn)
					if (ReferenceEquals(current, root)) return true;
				return false;
			}
			void EjectNonReadyContents(IGameItem root, IGameItem parent)
			{
				if (!TreeCurrent(root, parent) || !visited.Add(parent) || parent.Deleted || parent.Destroyed) return;
				var container = parent.GetItemType<IContainer>();
				if (container is null) return;
				foreach (var child in container.Contents.ToArray())
				{
					if (!TreeCurrent(root, parent)) return;
					if (parent.Deleted || parent.Destroyed || !ReferenceEquals(parent.GetItemType<IContainer>(), container) ||
						!container.Contents.Any(x => ReferenceEquals(x, child))) continue;
					var childReady = child.Effects.OfType<IPlaneSpecificInventoryRetention>().Any(x =>
						destinationPlanes.Contains(x.PlaneId) && x.IsReadyForTransition);
					EjectNonReadyContents(root, child);
					if (!TreeCurrent(root, parent)) return;
					if (childReady) continue;
					if (child.Deleted || child.Destroyed || !ReferenceEquals(child.ContainedIn, parent)) continue;
					var detached = container.Take(executor, child, 0);
					if (detached is null || !ReferenceEquals(detached, child) || detached.Deleted || detached.Destroyed ||
						detached.ContainedIn is not null || detached.InInventoryOf is not null || detached.Location is not null) continue;
					// Take may invoke content. A now-detached child still needs a safe destination,
					// even when that callback moves the actor or its original container.
					DropAtOrigin(detached);
					if (!ActorCurrent()) return;
				}
			}
			foreach (var item in roots)
			{
				if (!ActorCurrent()) return;
				EjectNonReadyContents(item, item);
			}
		}

		foreach (var item in roots)
		{
			if (!ActorCurrent()) return;
			if (item.Deleted || item.Destroyed || !ReferenceEquals(item.InInventoryOf, this) || item.ContainedIn is not null ||
				!DirectItems.Any(x => ReferenceEquals(x, item)) || WornItems.Any(x => ReferenceEquals(x, item)) || WieldedItems.Any(x => ReferenceEquals(x, item))) continue;
			if (item.GetItemTypes<ITransientItemPolicy>().Any(x => x.Triggers.HasFlag(ItemRetirementTrigger.EtherealTransition) &&
				(x.RetirementPlaneId == 0 || destinationPlanes.Contains(x.RetirementPlaneId))))
				ItemRetirement.Retire(item, ItemRetirementTrigger.EtherealTransition);
		}
		if (!ActorCurrent()) return;
		var allItems = roots
			.Concat(_implants.Select(x => x.Parent))
			.Concat(_prosthetics.Select(x => x.Parent))
			.Distinct().ToList();
		var toDrop = allItems.Where(x => !retained.Contains(x) && !x.Deleted && !x.Destroyed &&
			(x.ContainedIn is null && ReferenceEquals(x.InInventoryOf, this) && DirectItems.Any(y => ReferenceEquals(y, x)) ||
			 _implants.Any(y => ReferenceEquals(y.Parent, x)) || _prosthetics.Any(y => ReferenceEquals(y.Parent, x)))).ToArray();
		if (toDrop.Length == 0 && _implants.Count == 0 && _prosthetics.Count == 0) return;

		if (toDrop.Length > 0)
			OutputHandler.Handle(new EmoteOutput(new Emote("@ shimmer|shimmers faintly as &0's physical belongings slip free and fall away.", Actor, Actor)));
		if (!ActorCurrent()) return;
		// Callback-acquired or transferred items are outside this transition's captured roots.
		ClearCapturedDirectInventoryState(toDrop.ToHashSet<IGameItem>(ReferenceEqualityComparer.Instance));
		ClearImplantsAndProsthetics();
		foreach (var item in toDrop)
		{
			// These roots are already detached; drain them to the captured origin even if a prior drop moved the actor.
			if (item.Deleted || item.Destroyed || item.InInventoryOf is not null || item.ContainedIn is not null || item.Location is not null) continue;
			DropAtOrigin(item);
		}
	}
}
