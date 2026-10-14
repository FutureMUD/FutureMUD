#nullable enable

using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.Models;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedCreation
{
	private sealed record GateBirth(Models.Room SouthRoom, Models.RoomOverlay SouthOverlay,
		Models.Room NorthRoom, Models.RoomOverlay NorthOverlay, long SourceRoomId,
		long SourceOverlayId, long DestinationRoomId, long DestinationOverlayId);

	private GateBirth? _gate;

	internal void StageGateTopology(Models.Room southRoom, Models.RoomOverlay southOverlay,
		Models.Room northRoom, Models.RoomOverlay northOverlay, long sourceRoomId,
		long sourceOverlayId, long destinationRoomId, long destinationOverlayId)
	{
		if (_gate is not null || _shelter is not null || ReferenceEquals(southRoom, northRoom) ||
			ReferenceEquals(southOverlay, northOverlay) || sourceRoomId <= 0 || sourceOverlayId <= 0 ||
			destinationRoomId <= 0 || destinationOverlayId <= 0 || sourceRoomId == destinationRoomId ||
			!southRoom.Temporary || !northRoom.Temporary)
			throw new InvalidOperationException("A Nilaz creation requires exactly two owned temporary rooms and distinct permanent endpoints.");
		southOverlay.Room = southRoom;
		northOverlay.Room = northRoom;
		Context.Rooms.AddRange(southRoom, northRoom);
		Context.RoomOverlays.AddRange(southOverlay, northOverlay);
		Claim(SpellOwnedEntityKind.Room, southRoom);
		Claim(SpellOwnedEntityKind.Room, northRoom);
		Claim(SpellOwnedEntityKind.RoomOverlay, southOverlay);
		Claim(SpellOwnedEntityKind.RoomOverlay, northOverlay);
		_gate = new(southRoom, southOverlay, northRoom, northOverlay, sourceRoomId,
			sourceOverlayId, destinationRoomId, destinationOverlayId);
	}

	/// <summary>Assign generated IDs and all three paired exits in the original creation transaction.</summary>
	internal void FinishGateTopology()
	{
		if (_gate is not { } gate) return;
		gate.SouthRoom.CurrentOverlayId = gate.SouthOverlay.Id;
		gate.NorthRoom.CurrentOverlayId = gate.NorthOverlay.Id;
		var sourcePortal = GateExit(gate.SourceRoomId, gate.SouthRoom.Id,
			CardinalDirection.Unknown, CardinalDirection.Unknown, "enter", "leave", "gate", "outside");
		var corridor = GateExit(gate.SouthRoom.Id, gate.NorthRoom.Id,
			CardinalDirection.North, CardinalDirection.South, "go", "go", "north", "south");
		var destinationPortal = GateExit(gate.NorthRoom.Id, gate.DestinationRoomId,
			CardinalDirection.Unknown, CardinalDirection.Unknown, "leave", "enter", "outside", "gate");
		Context.Exits.AddRange(sourcePortal, corridor, destinationPortal);
		Claim(SpellOwnedEntityKind.Exit, sourcePortal);
		Claim(SpellOwnedEntityKind.Exit, corridor);
		Claim(SpellOwnedEntityKind.Exit, destinationPortal);
		Context.RoomOverlaysExits.AddRange(
			new RoomOverlayExit { RoomOverlayId = gate.SourceOverlayId, Exit = sourcePortal },
			new RoomOverlayExit { RoomOverlay = gate.SouthOverlay, Exit = sourcePortal },
			new RoomOverlayExit { RoomOverlay = gate.SouthOverlay, Exit = corridor },
			new RoomOverlayExit { RoomOverlay = gate.NorthOverlay, Exit = corridor },
			new RoomOverlayExit { RoomOverlay = gate.NorthOverlay, Exit = destinationPortal },
			new RoomOverlayExit { RoomOverlayId = gate.DestinationOverlayId, Exit = destinationPortal });
		Context.SaveChanges();
	}

	private static Models.Exit GateExit(long first, long second, CardinalDirection firstDirection,
		CardinalDirection secondDirection, string firstVerb, string secondVerb,
		string firstKeyword, string secondKeyword) => new()
	{
		RoomId1 = first, RoomId2 = second,
		Direction1 = (int)firstDirection, Direction2 = (int)secondDirection,
		TimeMultiplier = 1, AcceptsDoor = false,
		MaximumSizeToEnter = (int)SizeCategory.Titanic,
		MaximumSizeToEnterUpright = (int)SizeCategory.Titanic,
		BlockedLayers = string.Empty,
		Verb1 = firstVerb, Verb2 = secondVerb,
		PrimaryKeyword1 = firstKeyword, PrimaryKeyword2 = secondKeyword,
		Keywords1 = firstKeyword, Keywords2 = secondKeyword,
		OutboundDescription1 = "through", OutboundDescription2 = "through",
		InboundDescription1 = "from", InboundDescription2 = "from",
		OutboundTarget1 = firstKeyword, OutboundTarget2 = secondKeyword,
		InboundTarget1 = secondKeyword, InboundTarget2 = firstKeyword
	};
}
