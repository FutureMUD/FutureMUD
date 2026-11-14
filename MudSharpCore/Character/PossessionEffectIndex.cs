#nullable enable

using System.Runtime.CompilerServices;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Character;

/// <summary>Indexes effects on registered actor/item owners; snapshots allow removal callbacks to re-enter safely.</summary>
internal sealed class PossessionEffectIndex
{
	private static readonly ConditionalWeakTable<IFuturemud, PossessionEffectIndex> Indexes = new();
	private readonly HashSet<IPerceivable> _owners = new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<(long Identity, long Instance), HashSet<IEffect>> _effects = new();
	private bool _initialised;
	internal int RegisteredOwnerCount => _owners.Count;

	internal static PossessionEffectIndex For(IFuturemud gameworld) => Indexes.GetValue(gameworld, _ => new());

	internal void RegisterOwner(IPerceivable owner)
	{
		if (!_owners.Add(owner)) return;
		foreach (var effect in owner.EffectsOfType<IPossessedBodyEffect>()) Add(owner, effect);
		foreach (var effect in owner.EffectsOfType<ILiveBodyPossessionEffect>()) Add(owner, effect);
		foreach (var effect in owner.EffectsOfType<ICorpsePossessionEffect>()) Add(owner, effect);
	}

	internal void UnregisterOwner(IPerceivable owner)
	{
		if (!_owners.Remove(owner)) return;
		foreach (var bucket in _effects.Values) bucket.RemoveWhere(x => ReferenceEquals(x.Owner, owner));
		foreach (var key in _effects.Where(x => x.Value.Count == 0).Select(x => x.Key).ToArray()) _effects.Remove(key);
	}

	internal void Add(IPerceivable owner, IEffect effect)
	{
		if (!_owners.Contains(owner) || !TryKey(effect, out var key)) return;
		if (!_effects.TryGetValue(key, out var bucket))
		{
			bucket = new HashSet<IEffect>(ReferenceEqualityComparer.Instance);
			_effects.Add(key, bucket);
		}
		bucket.Add(effect);
	}

	internal void Remove(IEffect effect)
	{
		if (!TryKey(effect, out var key) || !_effects.TryGetValue(key, out var bucket)) return;
		bucket.Remove(effect);
		if (bucket.Count == 0) _effects.Remove(key);
	}

	private void EnsureInitialised(IFuturemud world)
	{
		if (_initialised) return;
		_initialised = true;
		foreach (var owner in (world.Actors?.AsEnumerable() ?? Enumerable.Empty<ICharacter>())
			         .Concat(world.CachedActors?.AsEnumerable() ?? Enumerable.Empty<ICharacter>()).Cast<IPerceivable>()
			         .Concat(world.Items?.AsEnumerable() ?? Enumerable.Empty<MudSharp.GameItems.IGameItem>())) RegisterOwner(owner);
	}

	internal T[] ForAnchor<T>(ICharacter anchor) where T : class, IEffect
	{
		EnsureInitialised(anchor.Gameworld);
		var identity = CharacterInstanceIdentityComparer.IdentityId(anchor);
		var instance = anchor.InstanceId;
		var result = new List<T>();
		if (_effects.TryGetValue((identity, instance), out var exact)) result.AddRange(exact.OfType<T>());
		if (instance != 0 && _effects.TryGetValue((identity, 0), out var legacy)) result.AddRange(legacy.OfType<T>());
		return result.ToArray();
	}

	internal T[] All<T>(IFuturemud world) where T : class, IEffect
	{
		EnsureInitialised(world);
		return _effects.Values.SelectMany(x => x).OfType<T>().ToArray();
	}

	private static bool TryKey(IEffect effect, out (long Identity, long Instance) key)
	{
		key = effect switch
		{
			IPossessedBodyEffect x => (x.AnchorCharacterId, x.AnchorInstanceId),
			ILiveBodyPossessionEffect x => (x.AnchorCharacterId, x.AnchorInstanceId),
			ICorpsePossessionEffect x => (x.AnchorCharacterId, x.AnchorInstanceId),
			_ => default
		};
		return effect is IPossessedBodyEffect or ILiveBodyPossessionEffect or ICorpsePossessionEffect;
	}
}
