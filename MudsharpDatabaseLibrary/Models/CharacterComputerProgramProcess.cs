#nullable enable

using System;

namespace MudSharp.Models;

public partial class CharacterComputerProgramProcess
{
	public long Id { get; set; }
	public long CharacterComputerExecutableId { get; set; }
	public long OwnerCharacterId { get; set; }
	public string ProcessName { get; set; } = null!;
	public int Status { get; set; }
	public int WaitType { get; set; }
	public DateTime? WakeTimeUtc { get; set; }
	public string? WaitArgument { get; set; }
	public int PowerLossBehaviour { get; set; }
	public string StateJson { get; set; } = null!;
	public string? ResultJson { get; set; }
	public string? LastError { get; set; }
	public DateTime StartedAtUtc { get; set; }
	public DateTime LastUpdatedAtUtc { get; set; }
	public DateTime? EndedAtUtc { get; set; }

	public virtual Character OwnerCharacter { get; set; } = null!;
	public virtual CharacterComputerExecutable CharacterComputerExecutable { get; set; } = null!;
}
