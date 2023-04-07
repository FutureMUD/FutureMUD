using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class WingProto : DrapeableBodypartProto
{
	public WingProto(DrapeableBodypartProto rhs, string newName) : base(rhs, newName)
	{
	}

	public WingProto(BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Wing;

	public override bool PartDamageEffects(IBody owner, CanUseBodypartResult why)
	{
		var result = base.PartDamageEffects(owner, why);
		if (why != CanUseBodypartResult.CanUse)
		{
			owner.Actor.CheckCanFly();
		}

		return result;
	}
}