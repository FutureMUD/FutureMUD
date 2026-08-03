using MudSharp.RPG.Checks;

using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces
{
    public interface ISheath : IGameItemComponent
    {
        SizeCategory MaximumSize { get; }
        IWieldable Content { get; set; }
        Difficulty StealthDrawDifficulty { get; }
        bool DesignedForGuns { get; }
        bool CanSheath(IGameItem item);
        string WhyCannotSheath(IGameItem item);
    }

	/// <summary>
	/// A sheath with more than one independently selectable slot. The legacy
	/// <see cref="ISheath.Content"/> member remains the first item for callers that
	/// only understand one-slot sheaths.
	/// </summary>
	public interface IMultiSlotSheath : ISheath
	{
		int Capacity { get; }
		IEnumerable<IWieldable> WieldableContents { get; }
		bool TryAdd(IWieldable content);
		bool TryRemove(IWieldable content);
	}
}
