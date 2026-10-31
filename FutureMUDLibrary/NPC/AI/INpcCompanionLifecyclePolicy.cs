#nullable enable

namespace MudSharp.NPC.AI;

/// <summary>Optional restrictions on the existing bodyguard/companion logout and reunion lifecycle.</summary>
public interface INpcCompanionLifecyclePolicy
{
	bool CompanionRequiresColocation { get; }
	bool CompanionFollowsHandler => false;
}
