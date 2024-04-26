using System.Linq;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class DrawAndWieldMove : CombatMoveBase
{
	public IWieldable Weapon { get; init; }

	public IWield SpecificHand { get; init; }

	public Emote PlayerEmote { get; set; }

	public ItemCanWieldFlags Flags { get; init; }

	#region Overrides of CombatMoveBase

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (Weapon != null && Assailant.Body.ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
		                               .All(x => x.Content != Weapon))
		{
			Assailant.Send("You no longer possess the item that you wanted to draw.");
			return new CombatMoveResult();
		}

		Assailant.Body.Draw(Weapon?.Parent, SpecificHand, PlayerEmote, flags: Flags);
		return new CombatMoveResult { RecoveryDifficulty = Difficulty.Easy };
	}

	#endregion

	#region Overrides of CombatMoveBase

	public override string Description { get; } = "Attempting to draw and then wield something.";

	public override double BaseDelay => 0.3;

	#endregion
}