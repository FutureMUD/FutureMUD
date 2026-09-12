using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;

#nullable enable
namespace MudSharp.Magic.Vancian;

public static class VancianItemAccess
{
	public static IEnumerable<IGameItem> AccessibleItems(ICharacter actor)
	{
		var seen = new HashSet<long>();
		var queue = new Queue<IGameItem>(actor.ContextualItems ?? []);
		while (queue.TryDequeue(out var item))
		{
			if (!seen.Add(item.Id) || item.Deleted || !actor.CanSee(item) || !actor.CanManipulateItem(item).Truth) continue;
			yield return item;
			if (item.GetItemType<IOpenable>() is { IsOpen: false }) continue;
			if (item.GetItemType<IContainer>() is not { } container) continue;
			foreach (var child in container.Contents.Where(x => container.CanTake(actor, x, 0))) queue.Enqueue(child);
		}
	}
	public static bool Accessible(ICharacter actor, IGameItem item) => AccessibleItems(actor).Any(x => ReferenceEquals(x, item));
	public static IEnumerable<ISpellbook> Books(ICharacter actor) => AccessibleItems(actor).Select(x => x.GetItemType<ISpellbook>()).Where(x => x is not null);
	public static bool Usable(ICharacter actor, IGameItem item, VancianWritingComponentProto proto)
	{
		if (proto.ConfigurationErrors().Count != 0 || !Accessible(actor, item) || item.GetItemType<IOpenable>()?.IsOpen == false) return false;
		if (!VancianPolicy.Permits(actor.Gameworld.FutureProgs.Get(proto.UseProgId), proto.UseProgId == 0, actor, item)) return false;
		if (!proto.RequireReadable) return true;
		var readable = item.GetItemType<IReadable>();
		return readable is not null && readable.Writings.Any(actor.CanRead);
	}
	public static bool UsableBook(ICharacter actor, IVancianMagicCapability capability, ISpellbook book, IMagicSpell spell)
	{
		if (book.DataError is not null || book.Prototype is not SpellbookGameItemComponentProto proto || !Usable(actor, book.Parent, proto)) return false;
		if (!VancianPolicy.Permits(actor.Gameworld.FutureProgs.Get(proto.EligibilityProgId), proto.EligibilityProgId == 0, book.Parent, spell)) return false;
		return VancianPolicy.Permits(actor.Gameworld.FutureProgs.Get(capability.PolicyProgs.GetValueOrDefault("bookuse")), !capability.PolicyProgs.ContainsKey("bookuse"), actor, capability, book.Parent);
	}
}
