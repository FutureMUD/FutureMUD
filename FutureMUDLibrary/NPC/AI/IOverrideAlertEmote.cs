#nullable enable

namespace MudSharp.NPC.AI;

public interface IOverrideAlertEmote
{
	string? AlertEmote { get; }
	string? DistantAlertEmote { get; }
}
