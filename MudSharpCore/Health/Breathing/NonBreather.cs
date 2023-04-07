using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Form.Material;

namespace MudSharp.Health.Breathing;

public class NonBreather : IBreathingStrategy
{
	public string Name => "non-breather";

	public bool NeedsToBreathe => false;

	public bool IsBreathing(IBody body)
	{
		return false;
	}

	public bool CanBreathe(IBody body)
	{
		return false;
	}

	public void Breathe(IBody body)
	{
		// Do nothing
	}

	public IFluid BreathingFluid(IBody body)
	{
		return null;
	}
}