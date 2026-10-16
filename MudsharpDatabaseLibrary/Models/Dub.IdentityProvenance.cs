#nullable enable

namespace MudSharp.Models;

public partial class Dub
{
	/// <summary>Null preserves the unknown provenance of dubs created before this field existed.</summary>
	public bool? WasIdentityConcealed { get; set; }
}
