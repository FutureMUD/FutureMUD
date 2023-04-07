using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class LiverProto : InternalOrganProto
{
	public LiverProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public LiverProto(LiverProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new LiverProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Liver;

	public new double PainFactor => 0.0;

	public override string FrameworkItemType => "LiverProto";
}