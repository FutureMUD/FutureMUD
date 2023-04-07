using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp.CharacterCreation;

public class ChargenStoryboard : IChargenStoryboard
{
	private static readonly IList<Tuple<ChargenStage, ChargenScreenStoryboardFactory>>
		_chargenStageFactoryEnumerable = new List<Tuple<ChargenStage, ChargenScreenStoryboardFactory>>();

	private static readonly IList<(string Name, string Description, string HelpText, ChargenStage Stage)>
		_chargenStageTypeInfos =
			new List<(string Name, string Description, string HelpText, ChargenStage Stage)>();

	private static readonly IList<(string Name, ChargenScreenStoryboardFactory Factory)>
		_chargenNameFactoryEnumerable = new List<(string Name, ChargenScreenStoryboardFactory Factory)>();

	private static ILookup<ChargenStage, ChargenScreenStoryboardFactory> ChargenStageFactories;
	private static ILookup<string, ChargenScreenStoryboardFactory> ChargenNameFactories;

	public static IEnumerable<(string Name, string Description, string HelpText, ChargenStage Stage)>
		ChargenStageTypeInfos => _chargenStageTypeInfos;

	private ChargenStoryboard()
	{
		SetupFactories();
	}

	public ChargenStoryboard(IFuturemud gameworld)
		: this()
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var storyboardList = new List<Tuple<IChargenScreenStoryboard, int>>();
			foreach (var stage in FMDB.Context.ChargenScreenStoryboards.Include(x => x.DependentStages).AsNoTracking())
			{
				var stageEnum = (ChargenStage)Convert.ToInt32(stage.ChargenStage);
				var factory = ChargenStageFactories[stageEnum]
					.FirstOrDefault(x => x.TypeName == stage.ChargenType);
				if (factory == null)
				{
					throw new ApplicationException($"There was no ChargenFactory for stage type {stage.ChargenType}.");
				}

				var storyboard = factory.CreateNew(Gameworld, stage);
				_defaultNextStage[stageEnum] = (ChargenStage)Convert.ToInt32(stage.NextStage);
				foreach (var dependency in stage.DependentStages)
				{
					_stageDependencies[stageEnum].Add((ChargenStage)Convert.ToInt32(dependency.Dependency));
				}

				_stageScreenMap[stageEnum] = storyboard;
				storyboardList.Add(Tuple.Create(storyboard, Convert.ToInt32(stage.Order)));
			}

			_defaultOrder = storyboardList.OrderBy(x => x.Item2).Select(x => x.Item1.Stage).ToList();
			FirstStage = DefaultOrder.First();
		}
	}

	private readonly Dictionary<ChargenStage, ChargenStage> _defaultNextStage = new();

	public IReadOnlyDictionary<ChargenStage, ChargenStage> DefaultNextStage => _defaultNextStage;

	private readonly List<ChargenStage> _defaultOrder;

	public IReadOnlyCollection<ChargenStage> DefaultOrder => _defaultOrder;

	public int OrderOf(ChargenStage stage)
	{
		return _defaultOrder.IndexOf(stage);
	}

	public ChargenStage FirstStage { get; protected set; }

	private readonly CollectionDictionary<ChargenStage, ChargenStage> _stageDependencies = new();

	public IReadOnlyCollectionDictionary<ChargenStage, ChargenStage> StageDependencies =>
		_stageDependencies.AsReadOnlyCollectionDictionary();

	private readonly Dictionary<ChargenStage, IChargenScreenStoryboard> _stageScreenMap = new();

	public IReadOnlyDictionary<ChargenStage, IChargenScreenStoryboard> StageScreenMap => _stageScreenMap;

	public void ReorderStage(ChargenStage stage, ChargenStage afterStage)
	{
		var movingStagePredecessor = _defaultNextStage.FirstOrDefault(x => x.Value == stage).Key;
		var movingStageFollower = _defaultNextStage[stage];
		var afterStageOldFollower = _defaultNextStage[afterStage];

		_defaultNextStage[afterStage] = stage;
		_defaultNextStage[stage] = afterStageOldFollower;
		_defaultNextStage[movingStagePredecessor] = movingStageFollower;

		foreach (var ss in _stageScreenMap.Values)
		{
			ss.Changed = true;
		}
	}

	public void AddDependency(ChargenStage stage, ChargenStage dependingStage)
	{
		if (!_stageDependencies[stage].Contains(dependingStage))
		{
			_stageDependencies.Add(stage, dependingStage);
			_stageScreenMap[stage].Changed = true;
		}
	}

	public void RemoveDependency(ChargenStage stage, ChargenStage dependingStage)
	{
		if (_stageDependencies[stage].Contains(dependingStage))
		{
			_stageDependencies.Remove(stage, dependingStage);
			_stageScreenMap[stage].Changed = true;
		}
	}

	public void SwapStoryboard(ChargenStage stage, string newType)
	{
		var newScreen = _chargenNameFactoryEnumerable.First(x => x.Name.EqualTo(newType)).Factory
		                                             .CreateNew(Gameworld, _stageScreenMap[stage]);
		_stageScreenMap[stage] = newScreen;
	}

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; protected set; }

	#endregion IHaveFuturemud Members

	public static void RegisterFactory(ChargenStage stage, ChargenScreenStoryboardFactory factory, string name,
		string description, string helpinfo)
	{
		_chargenStageFactoryEnumerable.Add(Tuple.Create(stage, factory));
		_chargenNameFactoryEnumerable.Add((name, factory));
		_chargenStageTypeInfos.Add((name, description, helpinfo, stage));
	}

	private static void SetupFactories()
	{
		foreach (
			var type in
			Futuremud.GetAllTypes().Where(x => x.IsSubclassOf(typeof(ChargenScreenStoryboard))))
		{
			var method = type.GetMethod("RegisterFactory", BindingFlags.Public | BindingFlags.Static);
			if (method != null)
			{
				method.Invoke(null, null);
			}
		}

		ChargenStageFactories = _chargenStageFactoryEnumerable.ToLookup(x => x.Item1, x => x.Item2);
		ChargenNameFactories = _chargenNameFactoryEnumerable.ToLookup(x => x.Name, x => x.Factory);
	}
}