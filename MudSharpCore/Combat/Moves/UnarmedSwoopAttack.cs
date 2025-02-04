using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using Org.BouncyCastle.Asn1.X509;
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
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote("@ dive|dives sharply toward a lower target.", Assailant, Assailant), flags: OutputFlags.SuppressSource | OutputFlags.SuppressObscured));
		var lowerLayers = Assailant.Location.Terrain(Assailant).TerrainLayers
		                           .Where(x => 
			                           x.IsLowerThan(startingLayer) &&
									   (x.IsHigherThan(targetLayer) || x == targetLayer)
			                        )
		                           .ToList();
		while (Assailant.RoomLayer != targetLayer)
		{
			Assailant.RoomLayer = lowerLayers.HighestLayer();
			lowerLayers.Remove(Assailant.RoomLayer);
			Assailant.OutputHandler.Handle(Assailant.RoomLayer == targetLayer ? new EmoteOutput(new Emote("@ dive|dives sharply from above towards $1.", Assailant, Assailant, Target), flags: OutputFlags.SuppressSource | OutputFlags.SuppressObscured) : new EmoteOutput(new Emote("@ dive|dives sharply from above and right on by towards a lower target.", Assailant, Assailant), flags: OutputFlags.SuppressSource | OutputFlags.SuppressObscured));
			Assailant.OffensiveAdvantage += Gameworld.GetStaticDouble("AdvantagePerLayerSwoopAttack");
		}

		var result = base.ResolveMove(defenderMove);
		if (!result.MoveWasSuccessful)
		{
			Assailant.MeleeRange = true;
			if (Target.CombatTarget == Assailant)
			{
				Target.MeleeRange = true;
			}
		}
		return result;
	}
}