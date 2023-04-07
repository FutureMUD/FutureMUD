using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body.PartProtos;

public class GillProto : DrapeableBodypartProto
{
	public GillProto(DrapeableBodypartProto rhs, string newName) : base(rhs, newName)
	{
	}

	public GillProto(Models.BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Gill;
}