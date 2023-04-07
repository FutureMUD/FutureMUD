using MudSharp.Character;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat.Moves;

public class ReceiveChargeMove : MeleeWeaponAttack
{
	public ReceiveChargeMove(ICharacter owner, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target)
		: base(owner, weapon, attack, target)
	{
	}

	public override string Description => "Receiving a charge";
}