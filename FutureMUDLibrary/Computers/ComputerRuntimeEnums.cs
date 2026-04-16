#nullable enable

namespace MudSharp.Computers;

public enum ComputerCompilationStatus
{
	NotCompiled = 0,
	Compiled = 1,
	Failed = 2
}

public enum ComputerExecutableKind
{
	Function = 0,
	Program = 1
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

public enum ComputerProcessStatus
{
	NotStarted = 0,
	Running = 1,
	Sleeping = 2,
	Completed = 3,
	Failed = 4,
	Killed = 5
}
