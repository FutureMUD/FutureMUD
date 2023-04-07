using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MudSharp.Accounts;
using MudSharp.Framework;

namespace MudSharp.GameItems;

public class GameItemComponentManager : IGameItemComponentManager
{
	private readonly List<string> _primaryTypes = new();

	private readonly Dictionary<string, Func<IFuturemud, IAccount, IGameItemComponentProto>>
		_registeredComponentProtos = new();

	private readonly Dictionary<string,
			Func<MudSharp.Models.GameItemComponentProto, IFuturemud, IGameItemComponentProto>>
		_registeredDatabaseLoaders =
			new();

	private readonly List<(string Name, string Blurb, string Help)> _typeHelpInfo = new();

	public IEnumerable<(string Name, string Blurb, string Help)> TypeHelpInfo => _typeHelpInfo;

	public GameItemComponentManager()
	{
		foreach (
			var type in
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(x => x.IsSubclassOf(typeof(GameItemComponentProto))))
		{
			var method = type.GetMethod("RegisterComponentInitialiser", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, new object[] { this });
		}
	}

	public IEnumerable<string> PrimaryTypes => _primaryTypes;

	public void AddTypeHelpInfo(string name, string blurb, string help)
	{
		_typeHelpInfo.Add((name, blurb, help));
	}

	public void AddBuilderLoader(string name, bool primary,
		Func<IFuturemud, IAccount, IGameItemComponentProto> initialiser)
	{
		_registeredComponentProtos.Add(name, initialiser);
		if (primary)
		{
			_primaryTypes.Add(name);
		}
	}

	public void AddDatabaseLoader(string name,
		Func<MudSharp.Models.GameItemComponentProto, IFuturemud, IGameItemComponentProto> initialiser)
	{
		_registeredDatabaseLoaders.Add(name, initialiser);
	}

	public IGameItemComponentProto GetProto(string name, IFuturemud gameworld, IAccount account)
	{
		var proto = _registeredComponentProtos.TryGetValue(name.ToLowerInvariant(), out var output)
			? output(gameworld, account)
			: null;
		if (proto != null)
			// This line is an easy way for us to have game item components that don't need to repeat a bunch of code to insert themselves into the database when initialised, because the non-base types haven't been initialised when that happens
		{
			gameworld.SaveManager.Flush();
		}

		return proto;
	}

	public IGameItemComponentProto GetProto(MudSharp.Models.GameItemComponentProto dbproto, IFuturemud gameworld)
	{
		return _registeredDatabaseLoaders.TryGetValue(dbproto.Type, out var output) ? output(dbproto, gameworld) : null;
	}
}