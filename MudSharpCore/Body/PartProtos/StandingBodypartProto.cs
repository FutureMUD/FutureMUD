using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.PartProtos;

public class StandingBodypartProto : DrapeableBodypartProto
{
	public StandingBodypartProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public StandingBodypartProto(StandingBodypartProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new StandingBodypartProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Standing;

	public override string FrameworkItemType => "StandingBodypartProto";

	public override bool PartDamageEffects(IBody owner, CanUseBodypartResult why)
	{
		var result = base.PartDamageEffects(owner, why);
		if (why != CanUseBodypartResult.CanUse)
		{
			owner.CheckPositionStillValid();
		}

		return result;
	}
}