#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;

namespace MudSharp.Magic.SpellEffects;

public enum SpellTravelAnchorMode
{
	ArcaneMarkedRoom,
	AnchorRuneLiveRoom
}

/// <summary>One exact, recheckable item/effect/room binding; never a name or numeric-room selector.</summary>
public sealed record ResolvedSpellTravelAnchor(SpellTravelAnchorMode Mode, IGameItem Item,
	IEffectSubtype Effect, IRoom Destination)
{
	public bool IsCurrentFor(ICharacter caster) =>
		SpellTravelAnchorResolver.TryResolve(caster, Item, Mode, out var fresh, out _) &&
		ReferenceEquals(fresh!.Effect, Effect) && ReferenceEquals(fresh.Destination, Destination);
}

public static class SpellTravelAnchorResolver
{
	public static bool TryResolve(ICharacter caster, IGameItem item, SpellTravelAnchorMode mode,
		out ResolvedSpellTravelAnchor? anchor, out string? error)
	{
		anchor = null;
		error = "A current travel anchor that admits you is required.";
		if (caster.Body is not { } body || !ReferenceEquals(body.Actor, caster) ||
			item.Id <= 0 || item.Deleted || item.Destroyed ||
			!ReferenceEquals(caster.Gameworld, item.Gameworld) ||
			!ReferenceEquals(caster.Gameworld.Items.Get(item.Id), item)) return false;
		if (mode == SpellTravelAnchorMode.ArcaneMarkedRoom)
		{
			if (!item.IsOwnedBy(caster) || !ReferenceEquals(item.InInventoryOf, body) || item.ContainedIn is not null ||
				!item.TrueLocations.Contains(caster.Location)) return false;
			var marks = item.Effects.OfType<ISpellMarkedRoomEffect>().Where(x => x.IsActive && x.ItemId == item.Id &&
				x is IMagicTagEffect { Caster: { } marker } &&
				CharacterInstanceIdentityComparer.IdentityId(marker) == CharacterInstanceIdentityComparer.IdentityId(caster)).ToArray();
			if (marks.Length != 1 || caster.Gameworld.Rooms.Get(marks[0].SavedRoomId) is not { } saved ||
				saved.Temporary || !ReferenceEquals(saved.Gameworld, caster.Gameworld)) return false;
			anchor = new(mode, item, marks[0], saved);
		}
		else if (mode == SpellTravelAnchorMode.AnchorRuneLiveRoom)
		{
			var runes = item.Effects.OfType<ILiveTravelAnchorEffect>()
				.Where(x => x.IsActive && x.ItemId == item.Id).ToArray();
			var rooms = item.TrueLocations.Distinct<IRoom>(ReferenceEqualityComparer.Instance).ToArray();
			if (runes.Length != 1 || !runes[0].AllowsTravel(caster) || rooms.Length != 1 || rooms[0].Temporary ||
				!ReferenceEquals(caster.Gameworld.Rooms.Get(rooms[0].Id), rooms[0])) return false;
			anchor = new(mode, item, runes[0], rooms[0]);
		}
		else return false;
		error = null;
		return true;
	}
}
