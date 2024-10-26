using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.CharacterCreation.Resources;

/// <summary>
///     Simple resources that do not regenerate or change except when awarded, e.g. RPP
/// </summary>
public class SimpleChargenResource : ChargenResourceBase
{
	public SimpleChargenResource(IFuturemud gameworld, ChargenResource resource) : base(gameworld, resource)
	{
	}

	public override void UpdateOnSave(ICharacter character, int oldMinutes, int newMinutes)
	{
		// Do nothing
	}

	/// <inheritdoc />
	public override bool DisplayChangesOnLogin => true;
}