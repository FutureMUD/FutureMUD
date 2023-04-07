using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Database;

namespace MudSharp.GameItems;

public partial class GameItem : IHaveMagicResource
{
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
		return _magicResourceAmounts[resource] >= amount;
	}

	public bool UseResource(IMagicResource resource, double amount)
	{
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
		_magicResourceAmounts[resource] += amount;
		_magicResourceAmounts[resource] = Math.Min(_magicResourceAmounts[resource], resource.ResourceCap(this));
		ResourcesChanged = true;
	}

	private readonly List<IMagicResourceRegenerator> _magicResourceGenerators = new();
	public IEnumerable<IMagicResourceRegenerator> MagicResourceGenerators => _magicResourceGenerators;

	private readonly Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate> _generatorDelegateDictionary =
		new();

	public void AddMagicResourceGenerator(IMagicResourceRegenerator generator)
	{
		if (!_magicResourceGenerators.Contains(generator))
		{
			_magicResourceGenerators.Add(generator);
			var hbdelegate = generator.GetOnMinuteDelegate(this);
			_generatorDelegateDictionary[generator] = hbdelegate;
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += hbdelegate;
			ResourcesChanged = true;
		}
	}

	public void RemoveMagicResourceGenerator(IMagicResourceRegenerator generator)
	{
		if (_magicResourceGenerators.Contains(generator))
		{
			_magicResourceGenerators.Remove(generator);
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= _generatorDelegateDictionary[generator];
			_generatorDelegateDictionary.Remove(generator);
			ResourcesChanged = true;
		}
	}

	public void SaveMagic(MudSharp.Models.GameItem item)
	{
		foreach (var resource in _magicResourceAmounts)
		{
			var dbresource = item.GameItemsMagicResources.FirstOrDefault(x => x.MagicResourceId == resource.Key.Id);
			if (dbresource == null)
			{
				dbresource = new Models.GameItemMagicResource
				{
					GameItem = item,
					MagicResourceId = resource.Key.Id
				};
				FMDB.Context.GameItemsMagicResources.Add(dbresource);
			}

			dbresource.Amount = resource.Value;
		}

		foreach (var dbresource in item.GameItemsMagicResources
		                               .Where(x => _magicResourceAmounts.All(y => y.Key.Id != x.MagicResourceId))
		                               .ToList())
		{
			item.GameItemsMagicResources.Remove(dbresource);
		}

		_resourcesChanged = false;
	}

	public void LoadMagic(MudSharp.Models.GameItem dbitem)
	{
		foreach (var resource in dbitem.GameItemsMagicResources)
		{
			_magicResourceAmounts.Add(Gameworld.MagicResources.Get(resource.MagicResourceId), resource.Amount);
		}

		foreach (var resource in Gameworld.MagicResources.Where(x =>
			         !_magicResourceAmounts.ContainsKey(x) && x.ShouldStartWithResource(this)))
		{
			_magicResourceAmounts.Add(resource, resource.StartingResourceAmount(this));
			ResourcesChanged = true;
		}
	}
}