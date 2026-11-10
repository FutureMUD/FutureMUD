#nullable enable

namespace MudSharp.GameItems.Interfaces;

/// <summary>Identifies an item that may remain with its owner during a transition to a particular plane.</summary>
public interface IPlaneSpecificInventoryRetention
{
	long PlaneId { get; }
	bool IsReadyForTransition { get; }
	bool IsVisibleToThatPlane { get; }
}
