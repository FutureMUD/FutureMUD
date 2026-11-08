#nullable enable

namespace MudSharp.Effects.Interfaces;

/// <summary>Releases runtime resources when an effect is detached, including silent removal.</summary>
/// <remarks>This notification is independent of optional gameplay removal actions and must be idempotent.</remarks>
public interface IEffectRemovalNotification : IEffectSubtype
{
	void OnRemovedFromOwner();
}
