using System.Linq;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class HeartProto : InternalOrganProto
{
	public HeartProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
	}

	public HeartProto(HeartProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new HeartProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Heart;

	public override string FrameworkItemType => "HeartProto";

	#region Overrides of InternalOrganProto

	protected override bool AffectedByBloodBuildup => true;
	protected override double BloodVolumeForTotalFailure => 1.0;

	public override void HandleChangedOrganFunction(IBody body, double oldFunctionFactor, double newFunctionFactor)
	{
		if (newFunctionFactor == oldFunctionFactor)
		{
			return;
		}

		if (newFunctionFactor < oldFunctionFactor)
		{
			if (newFunctionFactor <= 0.3 && oldFunctionFactor > 0.3)
			{
				body.OutputHandler.Send(
					"You feel a crushing pain in your chest, shooting pain in your torso and your heart is beating so fast you are worried it might explode."
						.Colour(Telnet.BoldMagenta));
			}
			else if (newFunctionFactor <= 0.65 && oldFunctionFactor > 0.65)
			{
				body.OutputHandler.Send(
					"You have severe heart pain, and your pulse has begun to race, all accompanied by an understandable but very urgent feeling of anxiety."
						.Colour(Telnet.BoldMagenta));
			}
			else if (newFunctionFactor <= 0.85 && oldFunctionFactor > 0.85)
			{
				body.OutputHandler.Send(
					"You have a little bit of heart pain, and your pulse is elevated above normal.".Colour(
						Telnet.BoldMagenta));
			}
		}
		else
		{
			if (newFunctionFactor >= 0.3 && oldFunctionFactor < 0.3)
			{
				body.OutputHandler.Send(
					"The crushing pain in your chest has abated a little and your pulse is slowing down.".Colour(
						Telnet.BoldGreen));
			}

			if (newFunctionFactor >= 0.65 && oldFunctionFactor < 0.65)
			{
				body.OutputHandler.Send(
					"Your severe heart pain has improved somewhat and your pulse is no longer racing.".Colour(
						Telnet.BoldGreen));
			}

			if (newFunctionFactor >= 0.85 && oldFunctionFactor < 0.85)
			{
				body.OutputHandler.Send(
					"Your heart pain has cleared up and your pulse has returned to normal.".Colour(Telnet.BoldGreen));
			}
		}
	}

	#endregion
}