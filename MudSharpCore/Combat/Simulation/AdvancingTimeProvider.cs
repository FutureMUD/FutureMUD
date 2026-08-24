#nullable enable

namespace MudSharp.Combat.Simulation;

internal sealed class AdvancingTimeProvider(DateTimeOffset initialUtc) : TimeProvider
{
	private DateTimeOffset _utcNow = initialUtc;

	public override DateTimeOffset GetUtcNow()
	{
		return _utcNow;
	}

	public void AdvanceTo(DateTime utc)
	{
		var target = new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
		if (target > _utcNow)
		{
			_utcNow = target;
		}
	}
}
