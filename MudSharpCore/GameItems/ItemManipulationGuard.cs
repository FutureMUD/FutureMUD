using System.Threading;
using MudSharp.Body;

#nullable enable

namespace MudSharp.GameItems;

/// <summary>Actor-driven component operations share physical access checks. Null actors denote system operations.</summary>
public static class ItemManipulationGuard
{
	private sealed record RemoteOperation(ICharacter Actor, Func<IGameItem, bool> Eligible);
	private static readonly AsyncLocal<RemoteOperation?> Remote = new();

	public static bool CanManipulate(ICharacter? actor, out string reason, params IGameItem?[] items)
	{
		reason = string.Empty;
		if (actor is null)
		{
			return true;
		}

		var remote = Remote.Value;
		if (remote?.Actor == actor)
		{
			if (items.All(x => x is not null && remote.Eligible(x)))
			{
				return true;
			}

			reason = "You can no longer reach that object with your telekinesis.";
			return false;
		}

		// Neural controls operate installed internal implants, without a physical hand action.
		if (items.Length > 0 && items[0]?.GetItemType<IImplantRespondToCommands>() is not null &&
		    items.All(x => x?.GetItemType<IImplant>() is { External: false, InstalledBody: { } body } &&
		                   body.Actor == actor))
		{
			return true;
		}

		if (!actor.CanPerformManualAction(out reason))
		{
			return false;
		}

		foreach (var item in items)
		{
			if (item is null)
			{
				reason = "That item is no longer accessible.";
				return false;
			}

			var result = actor.CanManipulateItem(item);
			if (!result.Truth)
			{
				reason = result.Message;
				return false;
			}
		}

		return true;
	}

	// Only the telekinesis operation dispatcher supplies this scope; normal callers cannot opt out of anatomy/access.
	internal static IDisposable BeginTelekineticOperation(ICharacter actor, Func<IGameItem, bool> eligible)
	{
		var previous = Remote.Value;
		Remote.Value = new RemoteOperation(actor, eligible);
		return new RestoreScope(previous);
	}

	private sealed class RestoreScope(RemoteOperation? previous) : IDisposable
	{
		public void Dispose() => Remote.Value = previous;
	}
}
