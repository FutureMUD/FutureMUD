using MudSharp.Models;
using MudSharp.Character;

namespace MudSharp.CharacterCreation.Resources;

/// <summary>
///     Simple resources that do not regenerate or change except when awarded, e.g. RPP
/// </summary>
public class SimpleChargenResource : ChargenResourceBase
{
	public SimpleChargenResource(ChargenResource resource) : base(resource)
	{
	}

	public override string FrameworkItemType => "ChargenResource";

	public override void UpdateOnSave(ICharacter character, int oldMinutes, int newMinutes)
	{
		// Do nothing
	}
}