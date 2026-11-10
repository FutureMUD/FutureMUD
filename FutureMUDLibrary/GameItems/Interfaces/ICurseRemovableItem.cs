#nullable enable

namespace MudSharp.GameItems.Interfaces;

/// <summary>A component whose curse can be lifted without removing unrelated item restrictions.</summary>
public interface ICurseRemovableItem : IGameItemComponent
{
	bool IsCursed { get; }
	/// <summary>Removes this component's curse and its explicitly designated restrictions.</summary>
	bool RemoveCurse();
}
