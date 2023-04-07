using MudSharp.Models;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body.PartProtos;

public class MinorBoneProto : BaseBoneProto
{
	public MinorBoneProto(BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public MinorBoneProto(MinorBoneProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new MinorBoneProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.MinorBone;

	#region Overrides of Item

	public override string FrameworkItemType => "Bone";

	#endregion

	public override bool CanBeImmobilised => true;
	public override bool CriticalBone => false;

	public override double BoneHealingModifier => 3.0;
}