using System.Linq;
using MudSharp.Character;
using MudSharp.NPC.Templates;

namespace MudSharp.Combat;

public static class CharacterCombatSettingsResolver
{
	public static ICharacterCombatSettings ResolveFallback(ICharacter character, INPCTemplate npcTemplate = null)
	{
		return ResolveExplicit(npcTemplate?.DefaultCombatSetting, character) ??
		       ResolveExplicit(character.Race?.CombatSettings.DefaultCombatSetting, character) ??
		       ResolvePriorityDefault(character);
	}

	private static ICharacterCombatSettings ResolveExplicit(ICharacterCombatSettings setting, ICharacter character)
	{
		if (setting is null || !setting.GlobalTemplate || !setting.CanUse(character))
		{
			return null;
		}

		return setting;
	}

	private static ICharacterCombatSettings ResolvePriorityDefault(ICharacter character)
	{
		return character.Gameworld.CharacterCombatSettings
		                .Where(x => x.GlobalTemplate)
		                .Where(x => x.CanUse(character))
		                .OrderByDescending(x => x.PriorityFor(character))
		                .ThenBy(x => x.Id)
		                .FirstOrDefault();
	}
}
