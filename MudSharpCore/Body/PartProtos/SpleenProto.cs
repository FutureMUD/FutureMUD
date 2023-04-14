using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class SpleenProto : InternalOrganProto
{
	public SpleenProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public SpleenProto(SpleenProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new SpleenProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Spleen;

	public new double PainFactor => 0.0;
}