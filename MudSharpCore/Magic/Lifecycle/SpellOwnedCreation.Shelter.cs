#nullable enable

using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.Models;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedCreation
{
	private (Models.Room Room, Models.RoomOverlay Overlay, long AnchorId, long AnchorOverlayId, string Keyword)? _shelter;

	internal void StageShelterTopology(Models.Room room, Models.RoomOverlay overlay, long anchorId, long anchorOverlayId, string keyword)
	{
		if (_shelter is not null) throw new InvalidOperationException("One shelter topology per creation transaction.");
		Context.Rooms.Add(room);
		Context.RoomOverlays.Add(overlay);
		Claim(SpellOwnedEntityKind.Room, room);
		Claim(SpellOwnedEntityKind.RoomOverlay, overlay);
		_shelter = (room, overlay, anchorId, anchorOverlayId, keyword);
	}

	/// <summary>IDs and nullable cyclic overlay FK are resolved inside the original private transaction.</summary>
	internal void FinishShelterTopology()
	{
		if (_shelter is not { } birth) return;
		birth.Room.CurrentOverlayId = birth.Overlay.Id;
		var exit = new Models.Exit
		{
			RoomId1 = birth.AnchorId, RoomId2 = birth.Room.Id,
			Direction1 = (int)CardinalDirection.Unknown, Direction2 = (int)CardinalDirection.Unknown,
			TimeMultiplier = 1, AcceptsDoor = false, MaximumSizeToEnter = (int)SizeCategory.Titanic,
			MaximumSizeToEnterUpright = (int)SizeCategory.Titanic, BlockedLayers = "",
			Verb1 = "enter", Verb2 = "leave", PrimaryKeyword1 = birth.Keyword, PrimaryKeyword2 = "outside",
			Keywords1 = birth.Keyword, Keywords2 = "outside",
			OutboundDescription1 = "into", OutboundDescription2 = "out of", InboundDescription1 = "from", InboundDescription2 = "from",
			OutboundTarget1 = birth.Keyword, OutboundTarget2 = "outside", InboundTarget1 = "outside", InboundTarget2 = birth.Keyword
		};
		Context.Exits.Add(exit);
		Claim(SpellOwnedEntityKind.Exit, exit);
		Context.RoomOverlaysExits.Add(new RoomOverlayExit { RoomOverlay = birth.Overlay, Exit = exit });
		Context.RoomOverlaysExits.Add(new RoomOverlayExit { RoomOverlayId = birth.AnchorOverlayId, Exit = exit });
		Context.SaveChanges();
	}
}
