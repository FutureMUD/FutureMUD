#nullable enable

namespace MudSharp.Construction;

public partial class Room
{
	public string? UniqueName { get; private set; }

	public bool TrySetUniqueName(string? value, out string error)
	{
		var name = RoomLookupExtensions.NormaliseUniqueName(value);
		if (!RoomLookupExtensions.IsValidUniqueName(name))
		{
			error = "A room unique name must be at most 255 characters and cannot be a numeric ID.";
			return false;
		}
		if (_isCombatSimulationRoom && name is not null)
		{
			error = "Combat simulation rooms cannot have unique names.";
			return false;
		}
		var conflict = Gameworld.Rooms.GetUniqueNameConflict(name, Id);
		if (conflict is not null)
		{
			error = $"Room #{conflict.Id} already uses that unique name.";
			return false;
		}
		if (!string.Equals(UniqueName, name, StringComparison.Ordinal))
		{
			UniqueName = name;
			Changed = true;
		}
		error = string.Empty;
		return true;
	}

	// Validate before constructing/registering any rooms. Never choose a winner or repair persisted keys.
	internal static void ValidatePersistedUniqueNames(IEnumerable<Models.Room> rooms)
	{
		var names = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var room in rooms)
		{
			var name = RoomLookupExtensions.NormaliseUniqueName(room.UniqueName);
			if (name is null) continue;
			if (!RoomLookupExtensions.IsValidUniqueName(name) || !string.Equals(name, room.UniqueName, StringComparison.Ordinal))
				throw new InvalidOperationException($"Room #{room.Id} has an invalid unique name. Explicitly resolve the persisted value before loading.");
			if (names.TryGetValue(name, out var otherId) && otherId != room.Id)
				throw new InvalidOperationException($"Room unique name '{name}' is duplicated by room IDs {otherId} and {room.Id}. Explicitly resolve the collision before loading.");
			names[name] = room.Id;
		}
	}
}
