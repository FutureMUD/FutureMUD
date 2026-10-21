#nullable enable

using System.Collections.Generic;

namespace MudSharp.Models;

public partial class Race
{
	public virtual ICollection<RacesMerits> RacesMerits { get; set; } = new HashSet<RacesMerits>();
}
