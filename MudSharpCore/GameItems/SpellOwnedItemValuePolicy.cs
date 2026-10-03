#nullable enable

using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.GameItems;

internal static class SpellOwnedItemValuePolicy
{
	internal const string Refusal = "temporary spell-created material cannot become permanent value";

	internal static bool ContainsTemporaryValue(IPerceivable value) => value switch
	{
		IGameItem item => ContainsTemporaryItem(item),
		PerceivableGroup group => group.Members.Any(ContainsTemporaryValue),
		_ => false
	};

	private static bool ContainsTemporaryItem(IGameItem root)
	{
		var seen = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		var pending = new Stack<IGameItem>(); pending.Push(root);
		while (pending.TryPop(out var item))
		{
			if (!seen.Add(item)) continue;
			if (item.SpellCreationOrigin?.IsTemporary == true) return true;
			foreach (var child in (item.DeepItems ?? []).Concat(item.AttachedAndConnectedItems ?? []).Concat(item.LodgedItems ?? [])) pending.Push(child);
		}
		return false;
	}

	internal static void RequireOrdinaryValue(IPerceivable value, string operation)
	{
		if (ContainsTemporaryValue(value)) throw new InvalidOperationException($"{operation} refused: {Refusal}.");
	}
}
