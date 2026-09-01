#nullable enable

using MudSharp.GameItems.Interfaces;

namespace MudSharp.Computers;

public interface IImplantComputerHost : IComputerHost, IImplantRespondToCommands
{
}

public interface IImplantComputerPeripheral : IImplantRespondToCommands
{
	IImplantComputerHost? AssignedComputerHost { get; }
}

public interface IImplantComputerStorage : IComputerStorage, IImplantComputerPeripheral
{
}

public interface IImplantComputerTerminal : IInteractiveComputerTerminal, IImplantComputerPeripheral
{
}

public interface IImplantAVRecorder : IDigitalMediaRecorder, IImplantComputerPeripheral
{
}
