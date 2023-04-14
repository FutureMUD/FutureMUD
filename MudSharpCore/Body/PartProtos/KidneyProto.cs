using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class KidneyProto : InternalOrganProto
{
	public KidneyProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public KidneyProto(KidneyProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new KidneyProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Kidney;

	public new double PainFactor => 0.0;
}