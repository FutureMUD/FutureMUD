using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
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
		var generators = Capabilities.SelectMany(x => x.Regenerators).Distinct().ToList();
		foreach (var generator in generators)
		{
			if (!_magicResourceGenerators.Contains(generator))
			{
				AddMagicResourceGenerator(generator);
				foreach (var resource in generator.GeneratedResources)
				{
					if (!_magicResourceAmounts.ContainsKey(resource))
					{
						_magicResourceAmounts[resource] = 0.0;
					}
				}

				MagicChanged = true;
			}
		}

		foreach (var generator in _magicResourceGenerators.Where(x => !generators.Contains(x)).ToArray())
		{
			RemoveMagicResourceGenerator(generator);
		}
	}

	public void SaveMagic(MudSharp.Models.Character character)
	{
		foreach (var resource in _magicResourceAmounts)
		{
			var dbresource =
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

		foreach (var dbresource in character.CharactersMagicResources
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
		foreach (var resource in character.CharactersMagicResources)
		{
			_magicResourceAmounts.Add(Gameworld.MagicResources.Get(resource.MagicResourceId), resource.Amount);
		}

		foreach (var item in Capabilities.SelectMany(x => x.Regenerators))
		{
			AddMagicResourceGenerator(item);
		}

		foreach (var resource in Gameworld.MagicResources.Where(x =>
			         !_magicResourceAmounts.ContainsKey(x) && x.ShouldStartWithResource(this)))
		{
			_magicResourceAmounts.Add(resource, resource.StartingResourceAmount(this));
			ResourcesChanged = true;
		}
	}

	#endregion

	#region Implementation of IHaveMagicResource

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
		var old = _magicResourceAmounts[resource];
		_magicResourceAmounts[resource] += amount;
		_magicResourceAmounts[resource] = Math.Min(_magicResourceAmounts[resource], resource.ResourceCap(this));
		if (old != _magicResourceAmounts[resource])
		{
			ResourcesChanged = true;
		}
	}

	private readonly List<IMagicResourceRegenerator> _magicResourceGenerators = new();
	public IEnumerable<IMagicResourceRegenerator> MagicResourceGenerators => _magicResourceGenerators;
	private Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate> _generatorDelegateDictionary = new();

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

	#endregion
}