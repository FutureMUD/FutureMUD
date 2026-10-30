#nullable enable

using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces;

/// <summary>Physical pages: prefer blank paper unless destruction of the selected written page is explicit.</summary>
public interface IConsumablePaper : IGameItemComponent
{
	bool CanConsumePage(ICharacter actor, bool allowWritten = false);
	bool ConsumePage(ICharacter actor, bool allowWritten = false);
}
