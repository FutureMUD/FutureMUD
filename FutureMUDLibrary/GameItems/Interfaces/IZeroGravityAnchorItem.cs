namespace MudSharp.GameItems.Interfaces;

public interface IZeroGravityAnchorItem : IGameItemComponent
{
	bool AllowsZeroGravityPushOff { get; }
}
