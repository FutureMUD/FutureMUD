using MudSharp.Body.Needs;
using MudSharp.Framework.Save;

#nullable enable

namespace MudSharp.Character;

public partial class Character
{
	internal void DetachCombatSimulationLocation()
	{
		Location = null;
	}

	internal void RestoreCombatSimulationEffects(XElement effects)
	{
		LoadEffects(effects);
		ScheduleCachedEffects();
	}

	internal void CopyCombatSimulationStateFrom(ICharacter source)
	{
		NeedsModel = NeedsModelFactory.ConvertNeedsModel(source.NeedsModel.ModelName, this, source.NeedsModel);
		State = source.State;
		CombatSettings = source.CombatSettings;
		CombatStrategyMode = source.CombatStrategyMode;
		PositionState = source.PositionState;
		PositionModifier = source.PositionModifier;
	}

	internal void InitialiseCombatSimulationIdentity(long characterId, long bodyId, long instanceId)
	{
		InitialiseWithoutPersistence(characterId);
		if (Body is LateKeywordedInitialisingItem body)
		{
			body.InitialiseWithoutPersistence(bodyId);
		}

		_handedness = Body.Handedness;
		_instanceId = instanceId;
		Body.BaseLiverAlcoholRemovalKilogramsPerHour = LiverFunction(this);
		RevalidateCombatSettingsAfterInitialisation();
	}
}
