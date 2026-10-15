#nullable enable

using MudSharp.GameItems.Interfaces;

namespace MudSharp.Character;

/// <summary>Conservative exact-identity admission; the legacy character hook alone still has an unimplemented disguise path.</summary>
public static class CharacterRecognitionAuthority
{
	public static bool IsObscured(ICharacter target, ICharacter observer) => target.IdentityIsObscuredTo(observer) ||
		target.Body.WornItems.SelectMany(x => x.Components.OfType<IObscureIdentity>()).Any(x => x.CurrentlyApplies);
}
