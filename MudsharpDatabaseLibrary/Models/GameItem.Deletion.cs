#nullable enable

namespace MudSharp.Models;

public enum NativeItemDeletionStage : byte
{
	TeardownInProgress = 1,
	ReadyToDelete = 2
}

public partial class GameItem
{
	public NativeItemDeletionStage? NativeDeletionStage { get; set; }
}
