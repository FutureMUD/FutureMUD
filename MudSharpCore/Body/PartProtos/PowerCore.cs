using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class PowerCore : InternalOrganProto
{
	protected PowerCore(InternalOrganProto rhs, string newName) : base(rhs, newName)
	{
	}

	public PowerCore(BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.PowerCore;

	public override IBodypart Clone(string newName)
	{
		return new PowerCore(this, newName);
	}

	public override string FrameworkItemType => "PowerCore";

	public override void HandleChangedOrganFunction(IBody body, double oldFunctionFactor, double newFunctionFactor)
	{
		if (newFunctionFactor == oldFunctionFactor)
		{
			return;
		}

		if (newFunctionFactor < oldFunctionFactor)
		{
			if (newFunctionFactor <= 0.0 && oldFunctionFactor > 0.0)
			{
				body.OutputHandler.Send(
					"Your power core is catastrophically damaged and your systems begin their emergency shutdown procedure."
						.Colour(Telnet.BoldMagenta));
			}
			else if (newFunctionFactor <= 0.3 && oldFunctionFactor > 0.3)
			{
				body.OutputHandler.Send(
					"Your power core is critically damaged and is in danger of shutting down.".Colour(
						Telnet.BoldMagenta));
			}
			else if (newFunctionFactor <= 0.5 && oldFunctionFactor > 0.5)
			{
				body.OutputHandler.Send(
					"Your power core has been severely damaged and is having trouble keeping your secondary systems supplied with power."
						.Colour(Telnet.BoldMagenta));
			}
			else if (newFunctionFactor <= 0.75 && oldFunctionFactor > 0.75)
			{
				body.OutputHandler.Send(
					"Your power core has been moderately damaged and some non-critical systems are experienced intermittent power shortages."
						.Colour(Telnet.BoldMagenta));
			}
		}
		else
		{
			if (newFunctionFactor >= 0.0 && oldFunctionFactor <= 0.0)
			{
				body.OutputHandler.Send("Your power core has come back online.".Colour(Telnet.BoldGreen));
			}

			if (newFunctionFactor >= 0.3 && oldFunctionFactor <= 0.3)
			{
				body.OutputHandler.Send(
					"Your power core is no longer at imminent risk of shutting down.".Colour(Telnet.BoldGreen));
			}

			if (newFunctionFactor >= 0.65 && oldFunctionFactor <= 0.65)
			{
				body.OutputHandler.Send(
					"Your power core is once again able to supply your secondary systems.".Colour(Telnet.BoldGreen));
			}

			if (newFunctionFactor >= 0.85 && oldFunctionFactor <= 0.85)
			{
				body.OutputHandler.Send(
					"Your power core is no longer causing intermittent outages to non-critical systems.".Colour(
						Telnet.BoldGreen));
			}
		}
	}
}