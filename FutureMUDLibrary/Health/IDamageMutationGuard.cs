#nullable enable

namespace MudSharp.Health;

/// <summary>Optional invocation-local fence immediately before a native body or item wound mutation.
/// Implementations throw if their captured physical operation is no longer current. Never persisted.</summary>
public interface IDamageMutationGuard
{
	void Validate(IHaveWounds recipient, IDamage damage);
}
