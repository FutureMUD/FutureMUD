using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class BoneProto : BaseBoneProto
{
	public BoneProto(BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public BoneProto(BoneProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new BoneProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Bone;

	#region Overrides of Item

	public override string FrameworkItemType => "Bone";

	#endregion

	public override bool CanBeImmobilised => true;

	public override bool CriticalBone => true;

	public override double BoneHealingModifier => 1.0;
}