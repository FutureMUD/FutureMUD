using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Magic.Powers;

public abstract class MagicalMeleeAttackPower : MagicPowerBase
{
	public override string PowerType => "Magical Melee Attack";
	public MagicalMeleeAttackPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		throw new NotImplementedException();
	}

	public override IEnumerable<string> Verbs => Enumerable.Empty<string>();
}