using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body.PartProtos;

public class EarProto : InternalOrganProto
{
	public EarProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public EarProto(EarProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new EarProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Ear;

	#region Overrides of BodypartPrototype

	public override void HandleChangedOrganFunction(IBody body, double oldFunctionFactor, double newFunctionFactor)
	{
		if (newFunctionFactor < oldFunctionFactor)
		{
			if (newFunctionFactor <= 0.0)
			{
				body.OutputHandler.Send($"You can no longer hear out of your {FullDescription()}.".Colour(Telnet.Red));
				if (!body.CanHear(body))
				{
					body.OutputHandler.Send("You can't hear! You're deaf!".Colour(Telnet.Red));
				}
			}
			else if (newFunctionFactor < 0.3)
			{
				body.OutputHandler.Send(
					$"It's getting very hard to hear out of your {FullDescription()}.".Colour(Telnet.BoldYellow));
			}
		}
		else if (newFunctionFactor > oldFunctionFactor)
		{
			if (oldFunctionFactor <= 0.0 && newFunctionFactor > 0.0)
			{
				body.OutputHandler.Send(
					$"You can hear out of your {FullDescription()} again.".Colour(Telnet.BoldGreen));
			}
			else if (oldFunctionFactor < 0.3 && newFunctionFactor >= 0.3)
			{
				body.OutputHandler.Send(
					$"It's no longer hard to hear out of your {FullDescription()}.".Colour(Telnet.BoldGreen));
			}
		}
	}

	#endregion
}