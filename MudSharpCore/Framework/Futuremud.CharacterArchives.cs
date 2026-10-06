#nullable enable

using MudSharp.Character;

namespace MudSharp.Framework;

public partial class Futuremud
{
	public ICharacterArchiveService CharacterArchives { get; } = new CharacterArchiveService();

	/// <summary>Final release after durable compaction; ordinary Destroy intentionally caches actors.</summary>
	public void ForgetArchivedCharacter(ICharacter character)
	{
		if (character is not Character.Character { IsArchived: true })
		{
			throw new InvalidOperationException("Only a durably archived runtime character can be forgotten.");
		}
		MudSharp.Form.Material.EnvironmentalExposureService.ForgetExisting(this, character);
		_actors.RemoveAll(x => x.Id == character.Id);
		_characters.RemoveAll(x => x.Id == character.Id);
		_NPCs.RemoveAll(x => x.Id == character.Id);
		_cachedActors.RemoveAll(x => x.Id == character.Id);
		DestroyListeners(character);
	}
}
