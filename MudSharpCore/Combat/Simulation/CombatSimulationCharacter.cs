using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.GameItems;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;

#nullable enable

namespace MudSharp.Combat.Simulation;

internal sealed class CombatSimulationCharacter(IFuturemud gameworld, ICharacterTemplate template)
	: Character.Character(gameworld, template)
{
	public override bool IsPlayerCharacter => false;

	public override IGameItem? Die()
	{
		if (State.HasFlag(CharacterState.Dead))
		{
			return null;
		}

		OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("RegularDeathEmote"), this, this)));
		State = CharacterState.Dead;
		PositionState = PositionSprawled.Instance;
		Combat?.LeaveCombat(this);
		Movement?.CancelForMoverOnly(this);
		Body.Die();
		return null;
	}
}

internal sealed class CombatSimulationNpc(
	IFuturemud gameworld,
	ICharacterTemplate template,
	INPCTemplate npcTemplate) : NPC.NPC(gameworld, template, npcTemplate)
{
	public override IGameItem? Die()
	{
		if (State.HasFlag(CharacterState.Dead))
		{
			return null;
		}

		OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("RegularDeathEmote"), this, this)));
		State = CharacterState.Dead;
		PositionState = PositionSprawled.Instance;
		Combat?.LeaveCombat(this);
		Movement?.CancelForMoverOnly(this);
		Body.Die();
		return null;
	}
}
