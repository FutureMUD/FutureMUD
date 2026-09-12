using MudSharp.Character;

#nullable enable
namespace MudSharp.Magic.Vancian;

/// <summary>Only observed, uninterrupted episodes count. Nothing about an unfinished episode is persisted.</summary>
public sealed class VancianSleepTracker : IDisposable
{
	private readonly ICharacter _owner;
	private readonly VancianMagicService _service;
	private readonly Dictionary<long, (DateTime Start, bool Awarded)> _episodes = [];
	private readonly HashSet<ICharacter> _observed = new(ReferenceEqualityComparer.Instance);
	private DateTime? _lastObservation;
	private bool _disposed;
	private bool _wasSleeping;
	public VancianSleepTracker(ICharacter owner, VancianMagicService service)
	{
		_owner = owner; _service = service;
		owner.Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += Tick;
		owner.OnQuit += Quit;
		Tick();
	}
	public void Interrupt() { _episodes.Clear(); }
	private void Quit(IPerceivable _) => Dispose();
	private void StateChanged(IPerceivable _) => Tick();
	public void Tick()
	{
		if (_disposed) return;
		var now = _service.UtcNow;
		if (_lastObservation is { } last && (now < last || now - last > TimeSpan.FromSeconds(15))) _episodes.Clear();
		_lastObservation = now;
		var instances = _owner.Identity?.Instances.Cast<ICharacter>().Append(_owner).ToArray() ?? [_owner];
		foreach (var instance in instances)
			if (_observed.Add(instance)) instance.OnStateChanged += StateChanged;
		foreach (var instance in _observed.Where(x => !instances.Any(y => ReferenceEquals(x, y))).ToArray())
		{ instance.OnStateChanged -= StateChanged; _observed.Remove(instance); }
		var capabilities = _owner.Capabilities.OfType<IVancianMagicCapability>().Where(x => x.RecoveryMode != VancianRecoveryMode.PreparationAction).DistinctBy(x => x.Id).ToArray();
		var sleeping = Qualifies(_owner);
		var woke = _wasSleeping && !sleeping && _owner.State.IsConscious() && !_owner.State.HasFlag(CharacterState.Sleeping);
		foreach (var capability in capabilities)
		{
			if (sleeping)
			{
				if (!_episodes.TryGetValue(capability.Id, out var episode)) _episodes[capability.Id] = episode = (_service.UtcNow, false);
				if (!episode.Awarded && _service.UtcNow - episode.Start >= capability.RequiredSleepDuration)
				{
					var result = _service.AwardSleep(_owner, capability);
					if (result.Success) _episodes[capability.Id] = (episode.Start, true);
				}
			}
			else
			{
				// A wake immediately after the threshold still accounts for the last observed segment.
				if (woke && _episodes.TryGetValue(capability.Id, out var episode) && !episode.Awarded && _service.UtcNow - episode.Start >= capability.RequiredSleepDuration)
					_service.AwardSleep(_owner, capability);
				if (woke && capability.RecoveryMode == VancianRecoveryMode.SleepAutomatic)
				{
					var result = _service.RequestRefresh(_owner, capability);
					_owner.OutputHandler.Send(result.Message);
				}
				_episodes.Remove(capability.Id);
			}
		}
		foreach (var key in _episodes.Keys.Where(key => capabilities.All(c => c.Id != key)).ToArray()) _episodes.Remove(key);
		_wasSleeping = sleeping;
	}
	public static bool Qualifies(ICharacter owner)
	{
		if (!owner.State.HasFlag(CharacterState.Sleeping) || owner.State.HasFlag(CharacterState.Unconscious) ||
			owner.State.HasFlag(CharacterState.Dead) || owner.State.HasFlag(CharacterState.Stasis)) return false;
		if (owner.Identity?.FocusedInstance is { } focused && !focused.State.HasFlag(CharacterState.Sleeping)) return false;
		return owner.Identity?.Instances.All(x => ReferenceEquals(x, owner) || x.State.HasFlag(CharacterState.Sleeping) ||
			x.State.HasFlag(CharacterState.Stasis) || x.ControlPolicy == CharacterInstanceControlPolicy.NotControllable) ?? true;
	}
	public void Dispose()
	{
		if (_disposed) return;
		_disposed = true; _episodes.Clear();
		_owner.Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= Tick;
		foreach (var instance in _observed) instance.OnStateChanged -= StateChanged;
		_observed.Clear(); _owner.OnQuit -= Quit;
	}
}
