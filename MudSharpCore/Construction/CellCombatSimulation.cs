#nullable enable

namespace MudSharp.Construction;

public partial class Cell
{
	internal void RemoveCombatSimulationArtifact(ICharacter character)
	{
		base.Leave(character);
		RouteSpatialService.Instance.UntrackPerceivable(character);
	}

	internal void RestoreCombatSimulationEffects(XElement effects)
	{
		LoadEffects(effects);
		ScheduleCachedEffects();
	}
}
