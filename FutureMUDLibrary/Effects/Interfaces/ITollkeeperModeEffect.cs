#nullable enable annotations

using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Interfaces
{
	public interface ITollkeeperModeEffect : IEffectSubtype
	{
		ICellExit? Exit { get; }
		long ExitId { get; }
		long GuardCellId { get; }
	}
}
