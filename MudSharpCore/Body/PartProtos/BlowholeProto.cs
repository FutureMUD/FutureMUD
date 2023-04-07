using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class BlowholeProto : DrapeableBodypartProto
{
	public BlowholeProto(DrapeableBodypartProto rhs, string newName) : base(rhs, newName)
	{
	}

	public BlowholeProto(Models.BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Blowhole;
}