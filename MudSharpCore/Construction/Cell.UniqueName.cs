#nullable enable

namespace MudSharp.Construction;

public partial class Cell
{
	public string? UniqueName { get; private set; }

	public bool TrySetUniqueName(string? value, out string error)
	{
		var name = CellLookupExtensions.NormaliseUniqueName(value);
		if (!CellLookupExtensions.IsValidUniqueName(name))
		{
			error = "A cell unique name must be at most 255 characters and cannot be a numeric ID.";
			return false;
		}
		if (_isCombatSimulationCell && name is not null)
		{
			error = "Combat simulation cells cannot have unique names.";
			return false;
		}
		var conflict = Gameworld.Cells.GetUniqueNameConflict(name, Id);
		if (conflict is not null)
		{
			error = $"Cell #{conflict.Id} already uses that unique name.";
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

	// Validate before constructing/registering any cells. Never choose a winner or repair persisted keys.
	internal static void ValidatePersistedUniqueNames(IEnumerable<Models.Cell> cells)
	{
		var names = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var cell in cells)
		{
			var name = CellLookupExtensions.NormaliseUniqueName(cell.UniqueName);
			if (name is null) continue;
			if (!CellLookupExtensions.IsValidUniqueName(name) || !string.Equals(name, cell.UniqueName, StringComparison.Ordinal))
				throw new InvalidOperationException($"Cell #{cell.Id} has an invalid unique name. Explicitly resolve the persisted value before loading.");
			if (names.TryGetValue(name, out var otherId) && otherId != cell.Id)
				throw new InvalidOperationException($"Cell unique name '{name}' is duplicated by cell IDs {otherId} and {cell.Id}. Explicitly resolve the collision before loading.");
			names[name] = cell.Id;
		}
	}
}
