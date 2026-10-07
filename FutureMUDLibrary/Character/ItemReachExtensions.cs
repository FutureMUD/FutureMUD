using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Planes;

#nullable enable

namespace MudSharp.Character;

public static class ItemReachExtensions
{
	/// <summary>Checks physical access, including every containing item, without requiring a hand or recording a crime.</summary>
	public static (bool Truth, string Message) CanReachItem(this ICharacter actor, IGameItem item,
		bool requireInventoryPermission = true)
	{
		var ancestors = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		var current = item;
		while (true)
		{
			if (current is null || current.Deleted || !ancestors.Add(current))
			{
				return (false, "That item is no longer accessible.");
			}

			if (!actor.CanInteractPlanar(current, PlanarInteractionKind.Inventory, out var planarMessage))
			{
				return (false, planarMessage);
			}

			if (current.GetItemType<IAutomationMountable>()?.MountHost is { } mountHost)
			{
				if (!mountHost.CanAccessMounts(actor, out var mountMessage))
				{
					return (false, mountMessage);
				}

				current = mountHost.Parent;
				continue;
			}

			if (current.ContainedIn is not { } container)
			{
				break;
			}

			if (container.GetItemType<IOpenable>()?.IsOpen == false &&
			    container.GetItemType<IContainer>()?.Contents.Contains(current) == true)
			{
				return (false, $"You must open {container.HowSeen(actor)} before you can reach inside it.");
			}

			current = container;
		}

		if (current.InInventoryOf is { } owner)
		{
			if (owner != actor.Body)
			{
				if (!actor.ColocatedWith(owner.Actor))
				{
					return (false, $"{item.HowSeen(actor, true)} is too far away for you to reach.");
				}

				if (!actor.CanInteractPlanar(owner.Actor, PlanarInteractionKind.Inventory, out var planarMessage))
				{
					return (false, planarMessage);
				}

				if (requireInventoryPermission && !owner.Actor.WillingToPermitInventoryManipulation(actor))
				{
					return (false, $"{owner.Actor.HowSeen(actor, true)} is not willing to let you manipulate things in their possession.");
				}
			}
		}
		else
		{
			var installedExit = current.GetItemType<IDoor>()?.InstalledExit;
			if (actor.Location is null ||
			    (installedExit is not null
				    ? !installedExit.Rooms.Contains(actor.Location)
				    : current.Location != actor.Location || current.RoomLayer != actor.RoomLayer))
			{
				return (false, $"{item.HowSeen(actor, true)} is too far away for you to reach.");
			}

			foreach (var ancestor in ancestors)
			{
				if (!actor.Location.CanGetAccess(ancestor, actor))
				{
					return (false, actor.Location.WhyCannotGetAccess(ancestor, actor));
				}
			}
		}

		return (true, string.Empty);
	}
}
