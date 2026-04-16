#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Computers;

public interface IComputerProcess
{
	string ProcessName { get; }
	IComputerProgramDefinition Program { get; }
	IComputerHost Host { get; }
	ComputerProcessWaitType WaitType { get; }
	DateTime? WakeTimeUtc { get; }
	string? WaitArgument { get; }
	bool IsRunning { get; }
	ComputerPowerLossBehaviour PowerLossBehaviour { get; }
}

public interface IComputerFile
{
	string FileName { get; }
	string TextContents { get; }
	long SizeInBytes { get; }
	DateTime CreatedAtUtc { get; }
	DateTime LastModifiedAtUtc { get; }
}

public interface IComputerFileSystem
{
	long CapacityInBytes { get; }
	long UsedBytes { get; }
	IEnumerable<IComputerFile> Files { get; }
}

public interface IComputerBuiltInApplication
{
	string Id { get; }
	string Name { get; }
	string Summary { get; }
	bool IsNetworkService { get; }
}

public interface IComputerHost
{
	bool Powered { get; }
	IComputerFileSystem? FileSystem { get; }
	IEnumerable<IComputerExecutable> Executables { get; }
	IEnumerable<IComputerProcess> Processes { get; }
	IEnumerable<IComputerBuiltInApplication> BuiltInApplications { get; }
}
