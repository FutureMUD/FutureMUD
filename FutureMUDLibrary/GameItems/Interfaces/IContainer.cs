using MudSharp.Character;
using MudSharp.PerceptionEngine;
using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces
{
	public enum WhyCannotPutReason {
		/// <summary>
		///     This is not an IContainer and thus cannot accept any item contents
		/// </summary>
		NotContainer,

		/// <summary>
		///     The container is closed and must first be opened
		/// </summary>
		ContainerClosed,

		/// <summary>
		///     The container is over capacity and cannot accept this item
		/// </summary>
		ContainerFull,

		/// <summary>
		///     The item is larger than the containers maximum size
		/// </summary>
		ItemTooLarge,

		/// <summary>
		/// This container filters what items can go into it, and this item does not meet the criteria
		/// </summary>
		NotCorrectItemType,

		/// <summary>
		/// Containers cannot be put into themselves
		/// </summary>
		CantPutContainerInItself,

		/// <summary>
		/// The full quantity of item as specified is too much, but a lesser quantity would fit
		/// </summary>
		ContainerFullButCouldAcceptLesserQuantity
	}

	public enum WhyCannotGetContainerReason {
		NotContainer,
		ContainerClosed,
		NotContained,
		UnlawfulAction
	}

	public interface IContainer : IGameItemComponent {
		IEnumerable<IGameItem> Contents { get; }
		string ContentsPreposition { get; }
		bool Transparent { get; }
		bool CanPut(IGameItem item);
		void Put(ICharacter putter, IGameItem item, bool allowMerge = true);
		WhyCannotPutReason WhyCannotPut(IGameItem item);
		bool CanTake(ICharacter taker, IGameItem item, int quantity);
		IGameItem Take(ICharacter taker, IGameItem item, int quantity);
		WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item);

		/// <summary>
		/// If a WhyCannotPut call with this item returned ContainerFullButCouldAcceptLesserQuantity result, this function can be called to get the lesser quantity
		/// </summary>
		/// <param name="item">The item that previously returned the ContainerFullButCouldAcceptLesserQuantity result with WhyCannotPut</param>
		/// <returns>The quantity of item that will fit in the container</returns>
		int CanPutAmount(IGameItem item);

		void Empty(ICharacter emptier, IContainer intoContainer, IEmote playerEmote = null);
	}
}