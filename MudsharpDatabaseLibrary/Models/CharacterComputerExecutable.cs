#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public partial class CharacterComputerExecutable
{
	public CharacterComputerExecutable()
	{
		Parameters = new HashSet<CharacterComputerExecutableParameter>();
		Processes = new HashSet<CharacterComputerProgramProcess>();
	}

	public long Id { get; set; }
	public long OwnerCharacterId { get; set; }
	public long? OwnerHostItemId { get; set; }
	public long? OwnerStorageItemId { get; set; }
	public string Name { get; set; } = null!;
	public int ExecutableKind { get; set; }
	public int CompilationContext { get; set; }
	public string ReturnTypeDefinition { get; set; } = null!;
	public string SourceCode { get; set; } = null!;
	public int CompilationStatus { get; set; }
	public string CompileError { get; set; } = null!;
	public bool AutorunOnBoot { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime LastModifiedAtUtc { get; set; }

	public virtual Character OwnerCharacter { get; set; } = null!;
	public virtual ICollection<CharacterComputerExecutableParameter> Parameters { get; set; }
	public virtual ICollection<CharacterComputerProgramProcess> Processes { get; set; }
}
