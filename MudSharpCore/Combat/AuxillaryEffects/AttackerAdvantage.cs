using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Combat.AuxillaryEffects;
#nullable enable
internal class AttackerAdvantage : IAuxillaryEffect
{
	public required IFuturemud Gameworld { get; set; }
	public required double BonusPerDegree { get; set; }
	public required ITraitDefinition DefenseTrait { get; set; }
	public required Difficulty DefenseDifficulty { get; set; }

	public XElement Save()
	{
		throw new NotImplementedException();
	}

	public void ApplyEffect(ICharacter attacker, IPerceiver? target, CheckOutcome outcome)
	{
		if (target is not ICharacter tch)
		{
			return;
		}

		var defenseCheck = Gameworld.GetCheck(CheckType.CombatMoveCheck);
		var defenderOutcome = defenseCheck.Check(tch, DefenseDifficulty, DefenseTrait, attacker);
		throw new NotImplementedException();
	}
}
