#nullable enable

using MudSharp.Character;

namespace MudSharp.Computers;

internal interface IComputerMutableOwner : IComputerExecutableOwner
{
	IComputerExecutableDefinition CreateExecutableDefinition(ComputerExecutableKind kind, string name);
	void SaveExecutableDefinition(IComputerExecutableDefinition executable);
	bool DeleteExecutableDefinition(IComputerExecutableDefinition executable, out string error);
	ComputerRuntimeProcess CreateProcessDefinition(ICharacter? actor, ComputerRuntimeProgramBase program);
	void SaveProcessDefinition(ComputerRuntimeProcess process);
	void DeleteProcessDefinition(IComputerProcess process);
}
