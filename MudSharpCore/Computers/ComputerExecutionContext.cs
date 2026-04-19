#nullable enable

using System;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Computers;

internal sealed class ComputerExecutionContext
{
	public required IComputerExecutableOwner Owner { get; init; }
	public required IComputerHost Host { get; init; }
	public required IFuturemud Gameworld { get; init; }
	public ICharacter? Actor { get; init; }
	public IComputerTerminalSession? Session { get; init; }
	public ComputerRuntimeProcess? Process { get; init; }
	public string? PendingTerminalInput { get; internal set; }
	public ComputerSignal? PendingSignalInput { get; internal set; }

	public string? ConsumePendingTerminalInput()
	{
		var input = PendingTerminalInput;
		PendingTerminalInput = null;
		return input;
	}

	public ComputerSignal? ConsumePendingSignalInput()
	{
		var input = PendingSignalInput;
		PendingSignalInput = null;
		return input;
	}
}

internal sealed class ComputerExecutionContextScope : IDisposable
{
	[ThreadStatic] private static ComputerExecutionContext? _current;
	private readonly ComputerExecutionContext? _prior;

	public ComputerExecutionContextScope(ComputerExecutionContext context)
	{
		_prior = _current;
		_current = context;
	}

	public static ComputerExecutionContext? Current => _current;

	public void Dispose()
	{
		_current = _prior;
	}
}

internal sealed class ComputerProgramWaitException : Exception
{
	public ComputerProgramWaitException(ComputerProcessWaitType waitType, string? waitArgument = null,
		long? waitingCharacterId = null, long? waitingTerminalItemId = null)
	{
		WaitType = waitType;
		WaitArgument = waitArgument;
		WaitingCharacterId = waitingCharacterId;
		WaitingTerminalItemId = waitingTerminalItemId;
	}

	public ComputerProcessWaitType WaitType { get; }
	public string? WaitArgument { get; }
	public long? WaitingCharacterId { get; }
	public long? WaitingTerminalItemId { get; }
}
