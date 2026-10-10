#nullable enable

namespace MudSharp.Character;

public partial class Character
{
	public bool IsArchived { get; private set; }

	// Exact owned projection rows have already been removed. Deregister this physical
	// instance without room-leave events, controller callbacks, or identity archival.
	internal void ReleaseCommittedProjectionRuntime(Character owner)
	{
		_noSave = true;
		StopNeedsHeartbeat();
		ClearForcedTransformationHeartbeatRegistration();
		PauseMagicResourceGeneratorHeartbeats();
		Gameworld.SaveManager.Abort(this);
		Gameworld.EffectScheduler.Destroy(this);
		Gameworld.Scheduler.Destroy(this);
		((MudSharp.Effects.EffectHandler)EffectHandler).ForgetCommittedRetirementEffects();
		MudSharp.Form.Material.EnvironmentalExposureService.ForgetCommittedProjectionBody(Gameworld, Body);
		if (Location is MudSharp.Construction.Room room) room.ReconcileNativeCharacterMembership(this, false);
		MudSharp.Construction.RouteSpatialService.Instance.UntrackPerceivable(this);
		ClearInstanceLocation();
		SetInstanceEmbodied(false);
		SetInstanceControllable(false);
		Controller = null;
		QueuedMoveCommands.Clear();
		Body.Actor = owner;
		owner.ForgetSecondaryInstance(this);
		Changed = false;
	}

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
