#nullable enable

using MudSharp.Character;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems;

/// <summary>Runs actual pickup callbacks while preserving the action's captured custody.</summary>
internal static class ItemPickupAttempt
{
	internal static bool TryRun(IGameItem item, ICharacter actor, Func<bool> sourceCurrent)
	{
		foreach (var permission in item.GetItemTypes<IItemInteractionPermissions>().ToArray())
		{
			if (!sourceCurrent() || !permission.HandlePickupAttempt(actor) || !sourceCurrent()) return false;
		}
		return sourceCurrent();
	}
}
