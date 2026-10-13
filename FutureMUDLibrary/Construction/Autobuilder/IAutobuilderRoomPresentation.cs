#nullable enable

using System;
using MudSharp.Framework;
using MudSharp.Work.Foraging;

namespace MudSharp.Construction.Autobuilder;

/// <summary>Native presentation resolved without allocating, registering or saving a room.</summary>
public sealed record AutobuilderRoomPresentation(string RoomName, string RoomDescription, ITerrain Terrain,
	RoomOutdoorsType OutdoorsType, double AmbientLightFactor, IForagableProfile? ForagableProfile);

/// <summary>An immutable presentation and a callback-free native template-state fence.</summary>
public sealed record AutobuilderRoomPresentationSnapshot(AutobuilderRoomPresentation Presentation, string Definition, Func<bool> Check)
{
	public bool IsCurrent => Check();
}

/// <summary>Optional deterministic presentation support for callers that own private room persistence.</summary>
public interface IAutobuilderRoomPresentation : IAutobuilderRoom
{
	IFuturemud Gameworld { get; }
	string PresentationDefinition { get; }
	AutobuilderRoomPresentation ResolvePresentation(ITerrain? specifiedTerrain = null);
	AutobuilderRoomPresentationSnapshot CaptureDefaultPresentation();
}
