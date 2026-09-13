#nullable enable

namespace MudSharp.Models;

public partial class Terrain
{
	/// <summary>Optional inherited environmental profile identity; a missing profile remains diagnosable.</summary>
	public long? EnvironmentalMagicProfileId { get; set; }
}
