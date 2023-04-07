using MudSharp.Models;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body.PartProtos;

public class MinorNonImmobilisingBoneProto : BaseBoneProto
{
	public MinorNonImmobilisingBoneProto(BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public MinorNonImmobilisingBoneProto(MinorNonImmobilisingBoneProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new MinorNonImmobilisingBoneProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.MinorNonImobilisingBone;

	#region Overrides of Item

	public override string FrameworkItemType => "Bone";

	#endregion

	public override bool CanBeImmobilised => false;
	public override bool CriticalBone => false;
	public override double BoneHealingModifier => 3.0;
}