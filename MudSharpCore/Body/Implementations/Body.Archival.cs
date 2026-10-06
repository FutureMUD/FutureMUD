#nullable enable

namespace MudSharp.Body.Implementations;

public partial class Body
{
	internal void ReleaseArchivedRuntime()
	{
		_noSave = true;
		Changed = false;
		EndStaminaTick(true);
		EndDrugTick();
		EndHealthTick();
		foreach (var wound in Wounds) Gameworld.SaveManager.Abort(wound);
		Gameworld.SaveManager.Abort(this);
		Gameworld.EffectScheduler.Destroy(this);
		Gameworld.Scheduler.Destroy(this);
		EffectHandler.RemoveAllEffects();
		Gameworld.Destroy(this);
	}
}
