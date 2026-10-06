#nullable enable

namespace MudSharp.Character;

public partial class Character
{
	public bool IsArchived { get; private set; }

	internal void ReleaseArchivedRuntime()
	{
		IsArchived = true;
		_noSave = true;
		Changed = false;
		if (this is MudSharp.NPC.NPC npc) npc.ReleaseEventSubscriptions();
		if (CharacterController is MudSharp.NPC.NPCController controller) controller.Dispose();
		StopNeedsHeartbeat();
		ClearForcedTransformationHeartbeatRegistration();
		PauseMagicResourceGeneratorHeartbeats();
		Gameworld.SaveManager.Abort(this);
		Gameworld.EffectScheduler.Destroy(this);
		Gameworld.Scheduler.Destroy(this);
		EffectHandler.RemoveAllEffects();
		if (Body is Body.Implementations.Body body) body.ReleaseArchivedRuntime();
		Gameworld.ForgetArchivedCharacter(this);
	}
}
