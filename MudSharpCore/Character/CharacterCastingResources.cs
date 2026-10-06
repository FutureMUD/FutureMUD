using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using System.Threading;

#nullable enable
namespace MudSharp.Character;

public partial class Character
{
	private int _castingCapacityReconciliationDepth;
	private int _castingCapacityRestorationDepth;
	internal bool CastingCapacityRestorationActive => _castingCapacityRestorationDepth > 0;
	private bool CastingCapacityMutationActive => _castingCapacityReconciliationDepth > 0 && !CastingCapacityRestorationActive;
	internal IDisposable DeferCastingCapacityReconciliation(bool reconcileOnDispose = true) =>
		new CapacityRestorationScope(MagicCastingService.Owner(this) as Character ?? this, reconcileOnDispose, true);
	internal IDisposable DeferCastingCapacityReconciliationForMutation() =>
		new CapacityRestorationScope(MagicCastingService.Owner(this) as Character ?? this, true, false);

	private sealed class CapacityRestorationScope : IDisposable
	{
		private Character? _actor;
		private readonly bool _reconcileOnDispose;
		private readonly bool _preventResourceUse;
		public CapacityRestorationScope(Character actor, bool reconcileOnDispose, bool preventResourceUse)
		{
			_actor = actor;
			_reconcileOnDispose = reconcileOnDispose;
			_preventResourceUse = preventResourceUse;
			actor._castingCapacityReconciliationDepth++;
			if (preventResourceUse) actor._castingCapacityRestorationDepth++;
		}
		public void Dispose()
		{
			var actor = _actor;
			_actor = null;
			if (actor is null) return;
			if (_preventResourceUse) actor._castingCapacityRestorationDepth--;
			if (--actor._castingCapacityReconciliationDepth == 0 && _reconcileOnDispose) actor.ReconcileCastingResourceCapacities();
		}
	}

	private readonly Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate> _castingGenerators = [];
	private bool IsCastingReserve(IMagicResource resource) => MagicCastingService.IsConfiguredReserve(Gameworld, resource.Id);
	private ICharacter CastingResourceOwner(IMagicResource resource) => IsCastingReserve(resource) ? MagicCastingService.Owner(this) : this;
	private bool IsCastingGenerator(IMagicResourceRegenerator generator) => generator.GeneratedResources.All(IsCastingReserve);
	private sealed record GenerationContext(ICharacter Recipient, Func<IMagicResource, bool> Allows);
	private static readonly AsyncLocal<GenerationContext?> GenerationFilter = new();
	private static void GenerateFiltered(ICharacter recipient, HeartbeatManagerDelegate generate, Func<IMagicResource, bool> allow)
	{
		var previous = GenerationFilter.Value;
		try { GenerationFilter.Value = new(recipient, allow); generate(); }
		finally { GenerationFilter.Value = previous; }
	}

	private IEnumerable<ICharacter> CastingGenerationBodies() => IsPlayerCharacter
		? [Identity?.FocusedInstance ?? this]
		: (Identity?.Instances ?? [this]).Where(x => !x.State.HasFlag(CharacterState.Dead) && !x.State.HasFlag(CharacterState.Stasis)).OrderBy(x => x.InstanceId);

	private IEnumerable<(ICharacter Actor, IMagicCapability Capability, long Reserve, bool Passive)> CastingGenerationPolicies()
	{
		foreach (var actor in CastingGenerationBodies())
		foreach (var capability in actor.Capabilities)
		{
			if (capability is IMagicCastingCapability { HasCastingPolicy: true } configured)
			{
				if (configured.CastingPolicy is { Enabled: true } policy && configured.CastingConfigurationErrors().Count == 0)
					yield return (actor, capability, policy.ReserveResourceId, policy.PassiveEntitlement);
				continue;
			}
			// An independent legacy grant retains its authored generators even if another policy uses the same reserve.
			foreach (var resource in capability.Regenerators.SelectMany(x => x.GeneratedResources).Where(IsCastingReserve).DistinctBy(x => x.Id))
				yield return (actor, capability, resource.Id, true);
		}
	}

	internal void ReconcileCastingResources()
	{
		var owner = MagicCastingService.Owner(this);
		if (!ReferenceEquals(owner, this)) { (owner as Character)?.ReconcileCastingResources(); return; }
		// Ownership persists while a policy is disabled or detached. Initialising a zero bucket grants no energy or route.
		var reserves = Gameworld.MagicCapabilities.OfType<IMagicCastingCapability>()
			.Where(x => x.CastingPolicy is not null).Select(x => x.CastingPolicy!.ReserveResourceId).ToHashSet();
		foreach (var id in reserves)
		{
			var resource = Gameworld.MagicResources.Get(id);
			if (resource is null) continue;
			if (!_magicResourceAmounts.ContainsKey(resource)) { _magicResourceAmounts[resource] = 0; ResourcesChanged = true; }
		}
		ReconcileCastingResourceCapacities();
		foreach (var instance in Identity?.Instances.OfType<Character>() ?? [this])
		{
			foreach (var generator in instance._magicResourceGenerators.Where(instance.IsCastingGenerator).ToArray()) instance.RemoveMagicResourceGenerator(generator);
		}
		var generationPolicies = CastingGenerationPolicies().ToArray();
		var desired = generationPolicies.Where(x => x.Passive && !generationPolicies.Any(y => !y.Passive && y.Reserve == x.Reserve))
			.SelectMany(x => x.Capability.Regenerators.Where(g => g.GeneratedResources.Any(r => r.Id == x.Reserve)))
			.Distinct().ToHashSet();
		foreach (var (generator, callback) in _castingGenerators.Where(x => !desired.Contains(x.Key)).ToArray())
		{
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= callback; _castingGenerators.Remove(generator);
		}
		foreach (var generator in desired.Where(x => !_castingGenerators.ContainsKey(x)))
		{
			HeartbeatManagerDelegate callback = () =>
			{
				var live = CastingGenerationPolicies().Where(x => !x.Actor.State.HasFlag(CharacterState.Dead) && !x.Actor.State.HasFlag(CharacterState.Stasis)).ToArray();
				var bindings = live.Where(x => x.Passive && x.Capability.Regenerators.Contains(generator) &&
					!live.Any(y => !y.Passive && y.Reserve == x.Reserve))
					.GroupBy(x => x.Reserve).Select(x => x.First())
					.GroupBy(x => x.Actor.InstanceId);
				foreach (var binding in bindings)
				{
					var active = binding.First().Actor;
					var allowed = binding.Select(x => x.Reserve).ToHashSet();
					GenerateFiltered(active, generator.GetOnMinuteDelegate(active), r => allowed.Contains(r.Id) && Gameworld.MagicCasting?.QuarantineReason(active, reserveId: r.Id) is null);
				}
			};
			_castingGenerators.Add(generator, callback);
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += callback;
		}
	}

	internal void ReconcileCastingResourceCapacities()
	{
		if (_castingCapacityReconciliationDepth > 0) return;
		var owner = MagicCastingService.Owner(this);
		if (!ReferenceEquals(owner, this)) { (owner as Character)?.ReconcileCastingResourceCapacities(); return; }
		foreach (var resource in _magicResourceAmounts.Keys.Where(IsCastingReserve).ToArray())
		{
			if (!MagicResourceCapacity.TryGetCap(resource, this, out var cap, out _)) continue;
			var balance = _magicResourceAmounts[resource];
			// Invalid configurations retain recoverable balances; a valid lower maximum only removes excess.
			if (!double.IsFinite(balance) || balance <= cap) continue;
			_magicResourceAmounts[resource] = cap; ResourcesChanged = true;
		}
	}

	private void PauseCastingGenerators()
	{
		foreach (var callback in _castingGenerators.Values) Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= callback;
		_castingGenerators.Clear();
	}
}
