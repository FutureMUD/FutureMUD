using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class SpineProto : InternalOrganProto, ISpineProto
{
	public SpineProto(Models.BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public SpineProto(SpineProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new SpineProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Spine;
}