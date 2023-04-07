using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class IntestinesProto : InternalOrganProto
{
	public IntestinesProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public IntestinesProto(IntestinesProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new IntestinesProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Intestines;

	public new double PainFactor => 0.0;

	public override string FrameworkItemType => "IntestinesProto";
}