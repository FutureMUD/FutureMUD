namespace MudSharp.Work.Crafts;

public class CraftPhase : ICraftPhase
{
	public ICraft Craft { get; set; }
	public int PhaseNumber { get; set; }
	public double PhaseLengthInSeconds { get; set; }
	public string Echo { get; set; }
	public string FailEcho { get; set; }

	public CraftPhase(ICraft craft, int phaseNumber, double phaseLengthInSeconds, string echo, string failEcho)
	{
		Craft = craft;
		PhaseNumber = phaseNumber;
		PhaseLengthInSeconds = phaseLengthInSeconds;
		Echo = echo;
		FailEcho = failEcho;
	}

	public CraftPhase(Models.CraftPhase phase, ICraft craft)
	{
		Craft = craft;
		PhaseNumber = phase.PhaseNumber;
		PhaseLengthInSeconds = phase.PhaseLengthInSeconds;
		Echo = phase.Echo;
		FailEcho = phase.FailEcho;
	}

	public CraftPhase(ICraftPhase rhs, ICraft newCraft)
	{
		Craft = newCraft;
		PhaseLengthInSeconds = rhs.PhaseLengthInSeconds;
		PhaseNumber = rhs.PhaseNumber;
		Echo = rhs.Echo;
		FailEcho = rhs.FailEcho;
	}
}