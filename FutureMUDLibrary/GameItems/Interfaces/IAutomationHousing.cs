#nullable enable

using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces;

public interface IAutomationHousing : IGameItemComponent
{
	IEnumerable<IGameItem> ConcealedItems { get; }
	bool CanAccessHousing(ICharacter actor, out string error);
	bool CanConcealItem(IGameItem item, out string error);
}
