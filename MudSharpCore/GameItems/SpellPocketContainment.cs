#nullable enable

using MudSharp.GameItems.Interfaces;
using MudSharp.Character;

namespace MudSharp.GameItems;

/// <summary>Checks native, declared containment edges before adoption and after custody callbacks.</summary>
internal static class SpellPocketContainment
{
	internal const int MaximumGraphItems = 256;
	internal static bool HasPocketAncestor(IGameItem item)
	{
		var seen = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		for (var current = item; current is not null; current = current.ContainedIn)
		{
			if (!seen.Add(current) || current.IsItemType<ISpellPocket>()) return true;
		}
		return false;
	}

	internal static bool CanAccess(ICharacter? actor, IGameItem container)
	{
		var seen = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		for (var current = container; current is not null; current = current.ContainedIn)
		{
			if (!seen.Add(current)) return false;
			if (current.GetItemType<ISpellPocket>() is { } pocket && (actor is null || !pocket.CanAccess(actor))) return false;
		}
		return true;
	}

	internal static string? AdmissionError(IGameItem item, IGameItem destination)
	{
		var ancestors = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		ISpellPocket? pocket = null;
		for (var current = destination; current is not null; current = current.ContainedIn)
		{
			if (ReferenceEquals(current, item) || !ancestors.Add(current)) return "Cyclic containment is forbidden.";
			if (current.GetItemType<ISpellPocket>() is { } found) pocket = found;
		}
		if (pocket is null) return null;
		if (pocket.Anchor is null || pocket.Parent.Gameworld.SpellOwnedPockets?.IsActive(pocket.Parent) != true)
			return "This pocket is unbound, expired or closing.";
		if (!TryGraph([item], out var incoming, out var error) || !TryGraph(pocket.Contents, out var stored, out error)) return error;
		if (incoming.Any(x => x.IsItemType<ISpellPocket>() || ReferenceEquals(x, pocket.Parent) ||
			x.Components.OfType<IProvideItemTargetProjections>().Any(p => p.TargetProjections.Any())))
			return "Pocket spaces and physical occupants cannot enter a pocket.";
		if (incoming.Any(x => x.Id <= 0 || x.Prototype?.Morphs == true || x.Quantity <= 0 || !double.IsFinite(x.Weight) || x.Weight < 0))
			return "Morphing items and invalid quantities or weights cannot enter a pocket.";
		var combined = incoming.Concat(stored).Distinct<IGameItem>(ReferenceEqualityComparer.Instance).ToArray();
		var persistentIds = new HashSet<long>();
		if (combined.Any(x => x.Id > 0 && !persistentIds.Add(x.Id)))
			return "Distinct item instances cannot share a persisted identity inside a pocket.";
		if (combined.Length > MaximumGraphItems)
			return "This pocket has reached its bounded item limit.";
		if (item.Size > pocket.Anchor.Configuration.MaximumSize) return "This item exceeds the pocket's maximum size.";
		var alreadyInside = stored.Any(x => ReferenceEquals(x, item));
		var addedWeight = alreadyInside ? 0 : item.Weight;
		var weight = pocket.Contents.Sum(x => x.Weight) + addedWeight;
		if (!double.IsFinite(weight) || weight < 0 || weight > pocket.Anchor.Capacity) return "This pocket is over capacity.";
		return null;
	}

	internal static IEnumerable<IGameItem> Children(IGameItem item) =>
		item.Components.OfType<IContainer>().SelectMany(x => x.Contents)
			.Concat(item.AttachedAndConnectedItems ?? []).Concat(item.LodgedItems ?? [])
			.Distinct<IGameItem>(ReferenceEqualityComparer.Instance);

	internal static bool TryGraph(IEnumerable<IGameItem> roots, out IGameItem[] graph, out string? error)
	{
		var seen = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		var visiting = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		var ids = new HashSet<long>();
		error = null;
		bool Visit(IGameItem item)
		{
			if (visiting.Contains(item)) return false;
			if (seen.Contains(item)) return false;
			if (item.Deleted || !seen.Add(item) || seen.Count > MaximumGraphItems || item.Id > 0 && !ids.Add(item.Id)) return false;
			visiting.Add(item);
			foreach (var child in Children(item)) if (!Visit(child)) return false;
			visiting.Remove(item); return true;
		}
		foreach (var root in roots)
			if (!Visit(root)) { graph = []; error = "Deleted, cyclic, duplicate or oversized item custody cannot enter or evacuate a pocket."; return false; }
		graph = seen.ToArray(); return true;
	}
}
