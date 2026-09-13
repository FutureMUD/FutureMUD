using MudSharp.Database;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Models;

#nullable enable annotations

namespace MudSharp.Construction;

public partial class Cell : IHaveMagicResource
{
    public void LoadMagic(MudSharp.Models.Cell cell)
    {
        foreach (CellMagicResource resource in cell.CellsMagicResources)
        {
			_pendingMagicResourceAmounts[resource.MagicResourceId] = resource.Amount;
        }
		CompleteMagicLoad();
	}

	private readonly Dictionary<long, double> _pendingMagicResourceAmounts = new();

	internal void CompleteMagicLoad()
	{
		foreach (var resource in _pendingMagicResourceAmounts.ToArray())
		{
			if (Gameworld.MagicResources.Get(resource.Key) is not { } definition) continue;
			_magicResourceAmounts[definition] = resource.Value;
			_pendingMagicResourceAmounts.Remove(resource.Key);
		}

        foreach (IMagicResource resource in Gameworld.MagicResources.Where(x =>
                     !_magicResourceAmounts.ContainsKey(x) && !IsEnvironmentalOutput(x) && x.ShouldStartWithResource(this)))
        {
            _magicResourceAmounts.Add(resource, resource.StartingResourceAmount(this));
            ResourcesChanged = true;
        }
    }

    public void SaveMagic(MudSharp.Models.Cell cell)
    {
		if (PendingEnvironmentalOperationId.HasValue) return;
        foreach (KeyValuePair<IMagicResource, double> resource in _magicResourceAmounts)
        {
            CellMagicResource dbresource = cell.CellsMagicResources.FirstOrDefault(x => x.MagicResourceId == resource.Key.Id);
            if (dbresource == null)
            {
                dbresource = new CellMagicResource
                {
                    Cell = cell,
                    MagicResourceId = resource.Key.Id
                };
                FMDB.Context.CellsMagicResources.Add(dbresource);
            }

            dbresource.Amount = resource.Value;
        }

        foreach (CellMagicResource dbresource in cell.CellsMagicResources
                                       .Where(x => !_pendingMagicResourceAmounts.ContainsKey(x.MagicResourceId) &&
										   _magicResourceAmounts.All(y => y.Key.Id != x.MagicResourceId))
                                       .ToList())
        {
            cell.CellsMagicResources.Remove(dbresource);
        }

        _resourcesChanged = false;
    }

    public IEnumerable<IMagicResource> MagicResources => _magicResourceAmounts.Keys;
    private readonly DoubleCounter<IMagicResource> _magicResourceAmounts = new();
    public IReadOnlyDictionary<IMagicResource, double> MagicResourceAmounts => _magicResourceAmounts;

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
		if (PendingEnvironmentalOperationId.HasValue) return false;
		if (Gameworld.EnvironmentalMagic?.TryInspectResource(this, resource, out var output) == true)
			return double.IsFinite(amount) && amount >= 0.0 && output.IsValid &&
				Math.Min(output.Balance, output.Maximum) >= amount;
        return _magicResourceAmounts[resource] >= amount;
    }

    public bool UseResource(IMagicResource resource, double amount)
    {
		if (PendingEnvironmentalOperationId.HasValue) return false;
		if (Gameworld.EnvironmentalMagic?.TryMutateResource(this, resource, EnvironmentalResourceMutation.Debit,
			amount, out var success) == true) return success;
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
		if (PendingEnvironmentalOperationId.HasValue) return;
		if (Gameworld.EnvironmentalMagic?.TryMutateResource(this, resource, EnvironmentalResourceMutation.Add,
			amount, out _) == true) return;
        _magicResourceAmounts[resource] += amount;
        _magicResourceAmounts[resource] = Math.Max(0.0,
            Math.Min(_magicResourceAmounts[resource], resource.ResourceCap(this)));
        ResourcesChanged = true;
    }

    private readonly List<IMagicResourceRegenerator> _magicResourceGenerators = new();
	private long? SelectedEnvironmentalProfileId => Id <= 0 ? null : EnvironmentBindingMode switch
		{
			EnvironmentalMagicBindingMode.Explicit => EnvironmentalMagicProfileId,
			EnvironmentalMagicBindingMode.Inherit => CurrentOverlay?.Terrain?.EnvironmentalMagicProfileId,
			_ => null
		};
	private IEnvironmentalMagicProfile? EnvironmentalProfile => SelectedEnvironmentalProfileId is { } id
		? Gameworld.MagicResourceRegenerators.Get(id) as IEnvironmentalMagicProfile : null;

	private bool IsEnvironmentalOutput(IMagicResource resource)
	{
		if (!SelectedEnvironmentalProfileId.HasValue) return false;
		// Unknown selected definitions cannot safely determine which starting-resource policies to run.
		return EnvironmentalProfile is not { } profile || profile.Outputs.Any(x => x.ResourceId == resource.Id);
	}

	public IEnumerable<IMagicResourceRegenerator> MagicResourceGenerators => EnvironmentalProfile is { } profile
		? _magicResourceGenerators.Append(profile) : _magicResourceGenerators;
    private Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate> _generatorDelegateDictionary = new();

    public void AddMagicResourceGenerator(IMagicResourceRegenerator generator)
    {
		if (generator is IEnvironmentalMagicProfile)
		{
			Gameworld.EnvironmentalMagic?.SetBinding(this, EnvironmentalMagicBindingMode.Explicit, generator.Id);
			return;
		}
        if (!_magicResourceGenerators.Contains(generator))
        {
            _magicResourceGenerators.Add(generator);
            HeartbeatManagerDelegate hbdelegate = generator.GetOnMinuteDelegate(this);
            _generatorDelegateDictionary[generator] = hbdelegate;
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += hbdelegate;
            ResourcesChanged = true;
        }
    }

    public void RemoveMagicResourceGenerator(IMagicResourceRegenerator generator)
    {
		if (generator is IEnvironmentalMagicProfile)
		{
			if (EnvironmentalProfile?.Id == generator.Id)
				Gameworld.EnvironmentalMagic?.SetBinding(this, EnvironmentalMagicBindingMode.Disabled, null);
			return;
		}
        if (_magicResourceGenerators.Contains(generator))
        {
            _magicResourceGenerators.Remove(generator);
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= _generatorDelegateDictionary[generator];
            _generatorDelegateDictionary.Remove(generator);
            ResourcesChanged = true;
        }
    }
}
