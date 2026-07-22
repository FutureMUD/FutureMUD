#nullable enable

using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.GameItems.Components;

internal static class ButcherySpatialPlacement
{
	public static void Place(IGameItem item, ICharacter worker, RoomLayer layer, bool newStack = false)
	{
		item.RoomLayer = layer;
		item.InsertAtSource(worker, newStack);
	}
}
