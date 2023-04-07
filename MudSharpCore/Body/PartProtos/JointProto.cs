using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body.PartProtos;

public class JointProto : DrapeableBodypartProto
{
	public JointProto(DrapeableBodypartProto rhs, string newName) : base(rhs, newName)
	{
	}

	public JointProto(Models.BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Joint;
}