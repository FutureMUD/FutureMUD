#nullable enable

namespace MudSharp.Models;

public partial class Race
{
	public long? AgeDescriptionProgId { get; set; }
	public virtual FutureProg? AgeDescriptionProg { get; set; }
}
