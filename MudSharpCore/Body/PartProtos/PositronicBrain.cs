using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class PositronicBrain : InternalOrganProto
{
	public PositronicBrain(InternalOrganProto rhs, string newName) : base(rhs, newName)
	{
	}

	public PositronicBrain(BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.PositronicBrain;

	public override IBodypart Clone(string newName)
	{
		return new PositronicBrain(this, newName);
	}

	public override string FrameworkItemType => "PositronicBrain";


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
					"Your processor is suffering from critical damage-induced degradation of performance and risks total, catastrophic failure."
						.Colour(Telnet.BoldMagenta));
			}
			else if (newFunctionFactor <= 0.65 && oldFunctionFactor > 0.65)
			{
				body.OutputHandler.Send(
					"Severe damage to your processor is causing reduced integrity of core system processes and memory."
						.Colour(Telnet.BoldMagenta));
			}
			else if (newFunctionFactor <= 0.85 && oldFunctionFactor > 0.85)
			{
				body.OutputHandler.Send(
					"Minor damage to your processor is causing non-critical glitches and blips in your systems.".Colour(
						Telnet.BoldMagenta));
			}
		}
		else
		{
			if (newFunctionFactor >= 0.3 && oldFunctionFactor < 0.3)
			{
				body.OutputHandler.Send("Your processor is no longer critically damaged.".Colour(Telnet.BoldGreen));
			}

			if (newFunctionFactor >= 0.65 && oldFunctionFactor < 0.65)
			{
				body.OutputHandler.Send("Your processor is no longer severely damaged.".Colour(Telnet.BoldGreen));
			}

			if (newFunctionFactor >= 0.85 && oldFunctionFactor < 0.85)
			{
				body.OutputHandler.Send("Your processor is no longer moderately damaged.".Colour(Telnet.BoldGreen));
			}
		}
	}
}