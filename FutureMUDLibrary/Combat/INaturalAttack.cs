using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Combat
{
	public interface INaturalAttack
	{
		IWeaponAttack Attack { get; set; }
		IBodypart Bodypart { get; set; }
		ItemQuality Quality { get; set; }
		bool IsSimilarTo(IWeaponAttack attack, IBodypart part);

		bool UsableAttack(ICharacter attacker, IPerceiver target, bool ignorePosition,
			params BuiltInCombatMoveType[] type);
	}
}