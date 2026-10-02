using MudSharp.Database;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Character;

public partial class Character : IMagicUser
{
    #region Implementation of IMagicUser

    private bool _magicChanged;

    public bool MagicChanged
    {
        get => _magicChanged;
        set
        {
            _magicChanged = value;
            if (value)
            {
                Changed = true;
            }
        }
    }

    public IEnumerable<IMagicCapability> Capabilities
    {
        get
        {
            if (IsAdministrator())
            {
                return Gameworld.MagicCapabilities.ToList();
            }

            return Merits.OfType<IMagicCapabilityMerit>()
                         .Where(x => x.Applies(this)).SelectMany(x => x.Capabilities)
                         .Concat(CombinedEffectsOfType<IGiveMagicCapabilityEffect>().SelectMany(x => x.Capabilities))
                         .Distinct()
                         .ToList();
        }
    }

    private readonly List<IMagicPower> _learnedPowers = new();

    public IEnumerable<IMagicPower> Powers =>
        _learnedPowers.Concat(Capabilities.SelectMany(x => x.InherentPowers(this)));

    public void LearnPower(IMagicPower power)
    {
        if (!_learnedPowers.Contains(power))
        {
            _learnedPowers.Add(power);
            MagicChanged = true;
        }
    }

    public void ForgetPower(IMagicPower power)
    {
        _learnedPowers.Remove(power);
        MagicChanged = true;
    }

    public void CheckResources()
    {
		UpdateVancianSleepTracker();
        List<IMagicResourceRegenerator> generators = Capabilities.SelectMany(x => x.Regenerators).Where(x => !IsCastingGenerator(x)).Distinct().ToList();
        foreach (IMagicResourceRegenerator generator in generators)
        {
            if (!_magicResourceGenerators.Contains(generator))
            {
                AddMagicResourceGenerator(generator);
                foreach (IMagicResource resource in generator.GeneratedResources)
                {
                    if (!_magicResourceAmounts.ContainsKey(resource))
                    {
                        _magicResourceAmounts[resource] = 0.0;
                    }
                }

                MagicChanged = true;
            }
        }

        foreach (IMagicResourceRegenerator generator in _magicResourceGenerators.Where(x => !generators.Contains(x)).ToArray())
        {
            RemoveMagicResourceGenerator(generator);
        }
		Gameworld.MagicCasting?.Reconcile(this);
    }

    public void SaveMagic(MudSharp.Models.Character character)
    {
        foreach (KeyValuePair<IMagicResource, double> resource in _magicResourceAmounts)
        {
            CharactersMagicResources dbresource =
                character.CharactersMagicResources.FirstOrDefault(x => x.MagicResourceId == resource.Key.Id);
            if (dbresource == null)
            {
                dbresource = new CharactersMagicResources
                {
                    Character = character,
                    MagicResourceId = resource.Key.Id
                };
                FMDB.Context.CharactersMagicResources.Add(dbresource);
            }

            dbresource.Amount = resource.Value;
        }

        foreach (CharactersMagicResources dbresource in character.CharactersMagicResources
                                            .Where(x => _magicResourceAmounts.All(y => y.Key.Id != x.MagicResourceId))
                                            .ToList())
        {
            character.CharactersMagicResources.Remove(dbresource);
        }

        _magicChanged = false;
        _resourcesChanged = false;
    }

    public void LoadMagic(MudSharp.Models.Character character)
    {
        foreach (CharactersMagicResources resource in character.CharactersMagicResources)
        {
            _magicResourceAmounts.Add(Gameworld.MagicResources.Get(resource.MagicResourceId), resource.Amount);
        }

        foreach (IMagicResourceRegenerator item in Capabilities.SelectMany(x => x.Regenerators).Where(x => !IsCastingGenerator(x)))
        {
            AddMagicResourceGenerator(item);
        }

        foreach (IMagicResource resource in Gameworld.MagicResources.Where(x =>
                     !_magicResourceAmounts.ContainsKey(x) && !IsCastingReserve(x) && x.ShouldStartWithResource(this)))
        {
            _magicResourceAmounts.Add(resource, resource.StartingResourceAmount(this));
            ResourcesChanged = true;
        }
    }

    #endregion

    #region Implementation of IHaveMagicResource

    public IEnumerable<IMagicResource> MagicResources => MagicResourceAmounts.Keys;
    private readonly DoubleCounter<IMagicResource> _magicResourceAmounts = new();
    public IReadOnlyDictionary<IMagicResource, double> MagicResourceAmounts =>
		MudSharp.Magic.Casting.MagicCastingService.Owner(this) is { } owner && !ReferenceEquals(owner, this)
			? _magicResourceAmounts.Where(x => !IsCastingReserve(x.Key)).Concat(owner.MagicResourceAmounts.Where(x => IsCastingReserve(x.Key))).ToDictionary(x => x.Key, x => x.Value)
			: _magicResourceAmounts;

    private bool _resourcesChanged;

    public bool ResourcesChanged
    {
        get => _resourcesChanged;
        set
        {
            _resourcesChanged = value;
            if (value)
            {
                Changed = true;
            }
        }
    }

    public bool CanUseResource(IMagicResource resource, double amount)
    {
		var owner = CastingResourceOwner(resource);
		if (!ReferenceEquals(owner, this)) return owner.CanUseResource(resource, amount);
		if (IsCastingReserve(resource))
		{
			if (!double.IsFinite(amount) || amount < 0 || !double.IsFinite(_magicResourceAmounts[resource]) ||
				!MagicResourceCapacity.TryGetCap(resource, this, out var cap, out _)) return false;
			var available = CastingCapacityMutationActive ? _magicResourceAmounts[resource] : Math.Min(_magicResourceAmounts[resource], cap);
			return available >= amount;
		}
        return _magicResourceAmounts[resource] >= amount;
    }

    public bool UseResource(IMagicResource resource, double amount)
    {
		var owner = CastingResourceOwner(resource);
		if (!ReferenceEquals(owner, this)) return owner.UseResource(resource, amount);
		if (IsCastingReserve(resource))
		{
			if (!double.IsFinite(amount) || amount < 0 || !double.IsFinite(_magicResourceAmounts[resource]) ||
				!MagicResourceCapacity.TryGetCap(resource, this, out var cap, out _)) return false;
			var available = CastingCapacityMutationActive ? _magicResourceAmounts[resource] : Math.Min(_magicResourceAmounts[resource], cap);
			if (available < amount) return false;
			_magicResourceAmounts[resource] = available - amount; ResourcesChanged = true;
			return true;
		}
        if (_magicResourceAmounts[resource] >= amount)
        {
            _magicResourceAmounts[resource] -= amount;
            ResourcesChanged = true;
            return true;
        }

        _magicResourceAmounts[resource] = 0;
        return false;
    }

    public void AddResource(IMagicResource resource, double amount)
    {
		if (GenerationFilter.Value is { } filter &&
			(ReferenceEquals(filter.Recipient, this) || IsCastingReserve(resource) && ReferenceEquals(MagicCastingService.Owner(filter.Recipient), this)) &&
			!filter.Allows(resource)) return;
		var owner = CastingResourceOwner(resource);
		if (!ReferenceEquals(owner, this)) { owner.AddResource(resource, amount); return; }
        if (!CanRunCharacterOngoingProcesses && !(IsCastingReserve(resource) && CastingGenerationBodies().Any(x =>
			!x.State.HasFlag(CharacterState.Dead) && !x.State.HasFlag(CharacterState.Stasis))))
        {
            PauseMagicResourceGeneratorHeartbeats();
            return;
        }

        if (!double.IsFinite(amount) || !MagicResourceCapacity.TryGetCap(resource, this, out var cap, out _)) return;
        double old = _magicResourceAmounts[resource];
		if (!double.IsFinite(old) || IsCastingReserve(resource) && old < 0) return;
        var credited = old + amount;
		if (!double.IsFinite(credited)) return;
		// A compound mutation applies legitimate deltas before its completed maximum clamps the reserve.
        _magicResourceAmounts[resource] = Math.Max(0.0,
			IsCastingReserve(resource) && CastingCapacityMutationActive ? credited : Math.Min(credited, cap));
        if (old != _magicResourceAmounts[resource])
        {
            ResourcesChanged = true;
        }
    }

    private readonly List<IMagicResourceRegenerator> _magicResourceGenerators = new();
    public IEnumerable<IMagicResourceRegenerator> MagicResourceGenerators => _magicResourceGenerators.Concat(_castingGenerators.Keys).Distinct();
    private Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate> _generatorDelegateDictionary = new();

    private void ResumeMagicResourceGeneratorHeartbeats()
    {
		UpdateVancianSleepTracker();
        if (!CanRunCharacterOngoingProcesses)
        {
            PauseMagicResourceGeneratorHeartbeats();
            return;
        }

        foreach (IMagicResourceRegenerator generator in _magicResourceGenerators)
        {
            if (_generatorDelegateDictionary.ContainsKey(generator))
            {
                continue;
            }

            HeartbeatManagerDelegate hbdelegate = () => GenerateFiltered(this, generator.GetOnMinuteDelegate(this), r => !IsCastingReserve(r));
            _generatorDelegateDictionary[generator] = hbdelegate;
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += hbdelegate;
        }
		ReconcileCastingResources();
    }

    private void PauseMagicResourceGeneratorHeartbeats()
    {
		PauseCastingGenerators();
		StopVancianSleepTracker();
        foreach (HeartbeatManagerDelegate hbdelegate in _generatorDelegateDictionary.Values.ToList())
        {
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= hbdelegate;
        }

        _generatorDelegateDictionary.Clear();
    }

    public void AddMagicResourceGenerator(IMagicResourceRegenerator generator)
    {
		if (generator is MudSharp.Magic.Environment.IEnvironmentalMagicProfile)
		{
			Gameworld.SystemMessage($"Environmental regenerator #{generator.Id} cannot be attached to character #{Id}; it is inactive.", true);
			return;
		}
        if (!_magicResourceGenerators.Contains(generator))
        {
            _magicResourceGenerators.Add(generator);
            ResumeMagicResourceGeneratorHeartbeats();
            ResourcesChanged = true;
        }
    }

    public void RemoveMagicResourceGenerator(IMagicResourceRegenerator generator)
    {
        if (_magicResourceGenerators.Contains(generator))
        {
            _magicResourceGenerators.Remove(generator);
            if (_generatorDelegateDictionary.Remove(generator, out HeartbeatManagerDelegate hbdelegate))
            {
                Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= hbdelegate;
            }

            ResourcesChanged = true;
        }
    }

    #endregion
}
