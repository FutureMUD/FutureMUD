using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using MudSharp.RPG.Checks;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

/// <summary>
/// Marks an ordinary item as explicitly eligible for the item-salvage workflow.
/// </summary>
public interface ISalvageable : IGameItemComponent
{
	ITraitDefinition Trait { get; }
	Difficulty Difficulty { get; }
	ITag? RequiredToolTag { get; }
	IInventoryPlanTemplate ToolTemplate { get; }
	IEnumerable<(string Emote, double Delay)> Stages { get; }
	bool CanSalvage(out string reason);
	double MaximumOutputWeight(bool success);
	IEnumerable<IGameItem> CreateProducts(ICharacter actor, bool success);
}
