using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body.PartProtos;

public class EyeProto : DrapeableBodypartProto
{
	public EyeProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public EyeProto(EyeProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new EyeProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Eye;

	public override string FrameworkItemType => "EyeProto";

	#region Overrides of BodypartPrototype

	public override bool PartDamageEffects(IBody owner, CanUseBodypartResult why)
	{
		var result = base.PartDamageEffects(owner, why);
		if (result)
		{
			owner.Send($"You can no longer see out of your {ShortDescription()}!");
			if (!owner.CanSee(owner))
			{
				owner.Send("You can't see! You're blind!");
			}
		}

		return result;
	}

	#endregion
}