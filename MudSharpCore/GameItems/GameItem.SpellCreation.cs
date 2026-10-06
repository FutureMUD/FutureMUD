#nullable enable

using MudSharp.Magic;
using MudSharp.Models;

namespace MudSharp.GameItems;

public partial class GameItem
{
	private bool _spellOwnedDeathObserversNotified;
	public SpellOwnedItemOrigin? SpellCreationOrigin { get; private set; }

	internal void ActivateCommittedSpellItem(Models.GameItem row, SpellLifecycleOrigin origin,
		IReadOnlyList<(GameItemComponent Component, Models.GameItemComponent Row)> components)
	{
		SpellCreationOrigin = new(origin.Id, origin.Mode, origin.DeadlineUtc);
		CompleteCommittedInitialisation(row);
		foreach (var (component, model) in components) component.ActivateCommittedSpellComponent(model);
		_noSave = false;
	}
}
