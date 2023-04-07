using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body.PartProtos;

public class TongueProto : ExternalBodypartProto
{
	public TongueProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public TongueProto(TongueProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new TongueProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Tongue;

	public override string FrameworkItemType => "TongueProto";

	#region Overrides of BodypartPrototype

	public override bool PartDamageEffects(IBody owner, CanUseBodypartResult why)
	{
		var result = base.PartDamageEffects(owner, why);
		if (result)
		{
			owner.Send($"Your {ShortDescription()} has been critically damaged, you won't be able to speak!");
		}

		return result;
	}

	#endregion
}