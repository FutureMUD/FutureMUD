using MudSharp.Character;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Moves;

public class UnarmedSwoopAttack : NaturalAttackMove
{
	public UnarmedSwoopAttack(ICharacter owner, NaturalAttack attack, ICharacter target) : base(owner, attack, target)
	{
		Target = target;
	}


	public ICharacter Target { get; set; }

	public override string Description => $"Performing a swoop attack";

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.SwoopAttackUnarmed;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var startingLayer = Assailant.RoomLayer;
		var targetLayer = Target.RoomLayer;

		Assailant.OutputHandler.Send($"You dive sharply towards {Target.HowSeen(Assailant)}.");
		Assailant.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ dive|dives sharply toward a lower target.", Assailant, Assailant),
			flags: OutputFlags.SuppressSource | OutputFlags.SuppressObscured));
		while (Assailant.RoomLayer != targetLayer)
		{
			switch (Assailant.RoomLayer)
			{
				case Construction.RoomLayer.GroundLevel:
					break;
				case Construction.RoomLayer.Underwater:
					break;
				case Construction.RoomLayer.InTrees:
					break;
				case Construction.RoomLayer.HighInTrees:
					break;
				case Construction.RoomLayer.InAir:
					break;
				case Construction.RoomLayer.HighInAir:
					break;
				case Construction.RoomLayer.OnRooftops:
					break;
			}
		}

		return base.ResolveMove(defenderMove);
	}
}