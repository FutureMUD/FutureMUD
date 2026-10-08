#nullable enable annotations

using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Interfaces
{
	public interface ITollkeeperModeEffect : IEffectSubtype
	{
		IRoomExit? Exit { get; }
		long ExitId { get; }
		long GuardRoomId { get; }
	}
}
