#nullable enable

namespace MudSharp.Computers;

public enum ComputerCompilationStatus
{
	NotCompiled = 0,
	Compiled = 1,
	Failed = 2
}

public enum ComputerProcessWaitType
{
	None = 0,
	Sleep = 1,
	UserInput = 2,
	Signal = 3
}

public enum ComputerPowerLossBehaviour
{
	Terminate = 0,
	RestartIfAutorun = 1,
	PersistSuspended = 2
}
