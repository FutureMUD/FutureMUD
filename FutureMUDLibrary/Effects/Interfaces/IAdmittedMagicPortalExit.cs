#nullable enable

using MudSharp.Character;
using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Interfaces;

/// <summary>An opt-in portal may refuse native traversal after its endpoint policy or lifetime changes.</summary>
public interface IAdmittedMagicPortalExit
{
	bool AllowsTraversal(ICharacter actor, IRoomExit exit);
}
