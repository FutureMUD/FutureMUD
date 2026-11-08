#nullable enable

using System;

namespace MudSharp.GameItems.Interfaces;

/// <summary>Optional native retirement on world reloading or inventory logout, independent of timed morphing.</summary>
[Flags]
public enum ItemRetirementTrigger
{
	None = 0,
	BulkDrop = 1,
	EtherealTransition = 2,
	TransientBodyDeath = 4,
	HostBodyChange = 8
}

public interface ITransientItemPolicy : IGameItemComponent
{
	bool RetireOnLoad { get; }
	bool RetireOnLogout { get; }
	ItemRetirementTrigger Triggers { get; }
	long RetirementPlaneId { get; }
}
