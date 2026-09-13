#nullable enable

namespace MudSharp.Models;

public partial class Cell
{
	/// <summary>0 inherits the terrain, 1 selects a profile, and 2 explicitly disables environmental magic.</summary>
	public int EnvironmentalMagicBindingMode { get; set; }

	/// <summary>Retained as an identity when a referenced profile is missing so the runtime can diagnose it.</summary>
	public long? EnvironmentalMagicProfileId { get; set; }

	public virtual CellEnvironmentalState? EnvironmentalState { get; set; }
}
