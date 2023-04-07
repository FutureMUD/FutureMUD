using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class StomachProto : InternalOrganProto
{
	public StomachProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public StomachProto(StomachProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new StomachProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Stomach;

	public new double PainFactor => 0.0;

	public override string FrameworkItemType => "StomachProto";
}