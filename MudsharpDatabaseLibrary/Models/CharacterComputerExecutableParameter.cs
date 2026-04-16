#nullable enable

namespace MudSharp.Models;

public partial class CharacterComputerExecutableParameter
{
	public long Id { get; set; }
	public long CharacterComputerExecutableId { get; set; }
	public int ParameterIndex { get; set; }
	public string ParameterName { get; set; } = null!;
	public string ParameterTypeDefinition { get; set; } = null!;

	public virtual CharacterComputerExecutable CharacterComputerExecutable { get; set; } = null!;
}
