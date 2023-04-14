using MudSharp.Models;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body.PartProtos;

public class NonImmobilisingBoneProto : BaseBoneProto
{
	public NonImmobilisingBoneProto(BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public NonImmobilisingBoneProto(NonImmobilisingBoneProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new NonImmobilisingBoneProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.NonImmobilisingBone;

	public override bool CanBeImmobilised => false;
	public override bool CriticalBone => true;
	public override double BoneHealingModifier => 1.0;
}