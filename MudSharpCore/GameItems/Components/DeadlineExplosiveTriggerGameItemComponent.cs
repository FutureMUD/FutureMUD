#nullable enable

namespace MudSharp.GameItems.Components;

internal static class ExplosiveDeadlineScheduler
{
	internal static bool IsDue(DateTime deadlineUtc, DateTime nowUtc)
	{
		return nowUtc >= deadlineUtc;
	}

	internal static bool TryGetDeadline(DateTime nowUtc, TimeSpan delay, out DateTime deadlineUtc)
	{
		deadlineUtc = default;
		if (delay <= TimeSpan.Zero)
		{
			return false;
		}

		var utcNow = nowUtc.Kind == DateTimeKind.Utc ? nowUtc : nowUtc.ToUniversalTime();
		if (delay.Ticks > DateTime.MaxValue.Ticks - utcNow.Ticks)
		{
			return false;
		}

		deadlineUtc = new DateTime(utcNow.Ticks + delay.Ticks, DateTimeKind.Utc);
		return true;
	}
}

public abstract class DeadlineExplosiveTriggerGameItemComponent : GameItemComponent
{
	private bool _heartbeatSubscribed;

	protected DeadlineExplosiveTriggerGameItemComponent(IGameItem parent, IGameItemComponentProto proto,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
	}

	protected DeadlineExplosiveTriggerGameItemComponent(MudSharp.Models.GameItemComponent component,
		IGameItem parent)
		: base(component, parent)
	{
	}

	protected DeadlineExplosiveTriggerGameItemComponent(DeadlineExplosiveTriggerGameItemComponent rhs,
		IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		DetonationDeadlineUtc = rhs.DetonationDeadlineUtc;
	}

	protected DateTime? DetonationDeadlineUtc { get; private set; }
	protected bool HasActiveDeadline => DetonationDeadlineUtc is not null;

	protected void LoadDeadline(XElement root)
	{
		if (long.TryParse(root.Element("DetonationDeadlineUtcTicks")?.Value, out var ticks) && ticks > 0 &&
		    ticks <= DateTime.MaxValue.Ticks)
		{
			DetonationDeadlineUtc = new DateTime(ticks, DateTimeKind.Utc);
		}
	}

	protected void SaveDeadline(XElement root)
	{
		root.Add(new XElement("DetonationDeadlineUtcTicks", DetonationDeadlineUtc?.Ticks ?? 0L));
	}

	protected void StartDeadline(DateTime deadlineUtc)
	{
		DetonationDeadlineUtc = deadlineUtc.Kind == DateTimeKind.Utc
			? deadlineUtc
			: deadlineUtc.ToUniversalTime();
		Changed = true;
		EnsureHeartbeatSubscription();
		CheckDeadline();
	}

	protected void CancelDeadline()
	{
		DetonationDeadlineUtc = null;
		Changed = true;
		RemoveHeartbeatSubscription();
	}

	protected TimeSpan RemainingDuration(DateTime? now = null)
	{
		return DetonationDeadlineUtc is { } deadline
			? deadline - (now ?? RuntimeClock.UtcNow)
			: TimeSpan.Zero;
	}

	public override void Login()
	{
		base.Login();
		if (!HasActiveDeadline)
		{
			return;
		}

		EnsureHeartbeatSubscription();
		CheckDeadline();
	}

	public override void Quit()
	{
		RemoveHeartbeatSubscription();
		base.Quit();
	}

	public override void Delete()
	{
		RemoveHeartbeatSubscription();
		base.Delete();
	}

	private void EnsureHeartbeatSubscription()
	{
		if (_heartbeatSubscribed || !HasActiveDeadline)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat += CheckDeadline;
		_heartbeatSubscribed = true;
	}

	private void RemoveHeartbeatSubscription()
	{
		if (!_heartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat -= CheckDeadline;
		_heartbeatSubscribed = false;
	}

	private void CheckDeadline()
	{
		if (DetonationDeadlineUtc is not { } deadline || !ExplosiveDeadlineScheduler.IsDue(deadline, RuntimeClock.UtcNow))
		{
			return;
		}

		DetonationDeadlineUtc = null;
		Changed = true;
		RemoveHeartbeatSubscription();
		Parent.GetItemType<IDetonatable>()?.Detonate();
	}
}
