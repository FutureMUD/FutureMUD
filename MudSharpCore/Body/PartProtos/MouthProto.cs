using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class MouthProto : DrapeableBodypartProto
{
	public MouthProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public MouthProto(MouthProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new MouthProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Mouth;

	public override string FrameworkItemType => "MouthProto";
}