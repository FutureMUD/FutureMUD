using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body.PartProtos;

public class TracheaProto : InternalOrganProto
{
	public TracheaProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public TracheaProto(TracheaProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new TracheaProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Trachea;

	public new double PainFactor => 0.0;

	public override string FrameworkItemType => "TracheaProto";

	#region Overrides of BodypartPrototype

	public override bool PartDamageEffects(IBody owner, CanUseBodypartResult why)
	{
		var result = base.PartDamageEffects(owner, why);
		if (result)
		{
			owner.Send($"Your throat feels tight! You cannot breathe!");
		}

		return result;
	}

	#endregion

	#region Overrides of InternalOrganProto

	public override void HandleChangedOrganFunction(IBody body, double oldFunctionFactor, double newFunctionFactor)
	{
		if (oldFunctionFactor > newFunctionFactor)
		{
			if (newFunctionFactor <= 0.0)
			{
				body.OutputHandler.Send("Your throat has closed up and you cannot breathe!".Colour(Telnet.BoldMagenta));
			}
			else if (newFunctionFactor <= 0.3)
			{
				body.OutputHandler.Send(
					"You are starting to find it pretty hard to breathe due to your throat.".Colour(Telnet.BoldYellow));
			}
			else if (newFunctionFactor < 0.6)
			{
				body.OutputHandler.Send(
					"Your breathing is becoming a little difficult due to your throat.".Colour(Telnet.BoldYellow));
			}
		}
		else if (newFunctionFactor < oldFunctionFactor)
		{
			if (oldFunctionFactor <= 0 && newFunctionFactor > 0)
			{
				body.OutputHandler.Send(
					"Your throat opens up and you can breathe through it again.".Colour(Telnet.BoldGreen));
			}

			if (oldFunctionFactor < 0.6 && newFunctionFactor >= 0.6)
			{
				body.OutputHandler.Send(
					"You no longer feel like you're having any difficulty breathing due to your throat.".Colour(
						Telnet.BoldGreen));
			}
			else if (oldFunctionFactor <= 0.3 && newFunctionFactor > 0.3)
			{
				body.OutputHandler.Send(
					"You no longer feel like it's pretty hard to breathe due to your throat, though it's not totally clear."
						.Colour(Telnet.BoldGreen));
			}
		}
	}

	protected override bool AffectedByBloodBuildup => true;
	protected override double BloodVolumeForTotalFailure => 0.2;

	#endregion
}