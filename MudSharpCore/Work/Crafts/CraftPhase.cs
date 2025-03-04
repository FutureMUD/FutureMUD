using Microsoft.EntityFrameworkCore;
using MudSharp.Body;

namespace MudSharp.Work.Crafts;

public class CraftPhase : ICraftPhase
{
	public ICraft Craft { get; set; }
	public int PhaseNumber { get; set; }
	public double PhaseLengthInSeconds { get; set; }
	public string Echo { get; set; }
	public string FailEcho { get; set; }
	public ExertionLevel ExertionLevel { get; set; }
	public double StaminaUsage { get; set; }

	public CraftPhase(ICraft craft, int phaseNumber, double phaseLengthInSeconds, string echo, string failEcho)
	{
		Craft = craft;
		PhaseNumber = phaseNumber;
		PhaseLengthInSeconds = phaseLengthInSeconds;
		Echo = echo;
		FailEcho = failEcho;
		ExertionLevel = ExertionLevel.Stasis;
		StaminaUsage = 0.0;
	}

	public CraftPhase(Models.CraftPhase phase, ICraft craft)
	{
		Craft = craft;
		PhaseNumber = phase.PhaseNumber;
		PhaseLengthInSeconds = phase.PhaseLengthInSeconds;
		Echo = phase.Echo;
		FailEcho = phase.FailEcho;
		ExertionLevel = (ExertionLevel)phase.ExertionLevel;
		StaminaUsage = phase.StaminaUsage;
	}

	public CraftPhase(ICraftPhase rhs, ICraft newCraft)
	{
		Craft = newCraft;
		PhaseLengthInSeconds = rhs.PhaseLengthInSeconds;
		PhaseNumber = rhs.PhaseNumber;
		Echo = rhs.Echo;
		FailEcho = rhs.FailEcho;
		ExertionLevel = rhs.ExertionLevel;
		StaminaUsage = rhs.StaminaUsage;
	}

	public Models.CraftPhase CreateDBPhase()
	{
		var dbphase = new Models.CraftPhase
		{
			Echo = Echo,
			FailEcho = FailEcho,
			PhaseLengthInSeconds = PhaseLengthInSeconds,
			PhaseNumber = PhaseNumber,
			ExertionLevel = (int)ExertionLevel,
			StaminaUsage = StaminaUsage
		};
		return dbphase;
	}
}