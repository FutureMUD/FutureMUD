#nullable enable

using MudSharp.Computers;

namespace MudSharp.GameItems.Interfaces;

public interface IFileSignalGenerator : ISignalSourceComponent, IComputerFileOwner
{
	string SignalFileName { get; }
	double ParsedSignalValue { get; }
	bool FileValueValid { get; }
	string FileStatus { get; }
}
