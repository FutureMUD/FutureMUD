using MudSharp.Body;

namespace MudSharp.Work.Crafts;

public interface ICraftPhase
{
	ICraft Craft { get; set; }
	int PhaseNumber { get; set; }
	double PhaseLengthInSeconds { get; set; }
	string Echo { get; set; }
	string FailEcho { get; set; }
	ExertionLevel ExertionLevel { get; set; }
	double StaminaUsage { get; set; }
	Models.CraftPhase CreateDBPhase();
}