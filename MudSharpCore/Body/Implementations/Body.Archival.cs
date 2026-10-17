#nullable enable

namespace MudSharp.Body.Implementations;

public partial class Body
{
	internal void SetIDFromCommittedProjection(Models.Body row)
	{
		SetIDFromDatabase(row);
		_noSave = false;
		Changed = false;
	}
	internal void ReleaseArchivedRuntime() => ReleaseRetiredRuntime(false);

	internal void ReleaseCommittedProjectionRuntime() => ReleaseRetiredRuntime(true);

	private void ReleaseRetiredRuntime(bool committedProjection)
	{
		_noSave = true;
		Changed = false;
		EndStaminaTick(true);
		EndDrugTick();
		EndHealthTickRegistration();
		foreach (var wound in Wounds) Gameworld.SaveManager.Abort(wound);
		Gameworld.SaveManager.Abort(this);
		Gameworld.EffectScheduler.Destroy(this);
		Gameworld.Scheduler.Destroy(this);
		if (committedProjection) ((MudSharp.Effects.EffectHandler)EffectHandler).ForgetCommittedRetirementEffects();
		else EffectHandler.RemoveAllEffects();
		Gameworld.Destroy(this);
	}
}
