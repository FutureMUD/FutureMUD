using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;
using GameItemComponentProto = MudSharp.GameItems.GameItemComponentProto;

namespace MudSharp.Construction.Autobuilder;

public class AutobuilderFactory
{
	private static readonly Dictionary<string, Func<AutobuilderRoomTemplate, IFuturemud, IAutobuilderRoom>>
		RoomLoaders =
			new();

	private static readonly Dictionary<string, Func<IFuturemud, string, IAutobuilderRoom>> BuilderRoomLoaders =
		new();

	private static readonly Dictionary<string, Func<AutobuilderAreaTemplate, IFuturemud, IAutobuilderArea>>
		AreaLoaders =
			new();

	private static readonly Dictionary<string, Func<IFuturemud, string, IAutobuilderArea>> BuilderAreaLoaders =
		new();

	public static IEnumerable<string> RoomLoaderTypes => BuilderRoomLoaders.Keys;
	public static IEnumerable<string> AreaLoaderTypes => BuilderAreaLoaders.Keys;

	public static void RegisterLoader(string typeName,
		Func<AutobuilderRoomTemplate, IFuturemud, IAutobuilderRoom> loaderFunc)
	{
		RoomLoaders[typeName.ToLowerInvariant()] = loaderFunc;
	}

	public static void RegisterBuilderLoader(string typeName,
		Func<IFuturemud, string, IAutobuilderRoom> loaderFunc)
	{
		BuilderRoomLoaders[typeName.ToLowerInvariant()] = loaderFunc;
	}

	public static void RegisterLoader(string typeName,
		Func<AutobuilderAreaTemplate, IFuturemud, IAutobuilderArea> loaderFunc)
	{
		AreaLoaders[typeName.ToLowerInvariant()] = loaderFunc;
	}

	public static void RegisterBuilderLoader(string typeName,
		Func<IFuturemud, string, IAutobuilderArea> loaderFunc)
	{
		BuilderAreaLoaders[typeName.ToLowerInvariant()] = loaderFunc;
	}

	public static IAutobuilderRoom LoadRoom(AutobuilderRoomTemplate room, IFuturemud gameworld)
	{
		return RoomLoaders[room.TemplateType.ToLowerInvariant()](room, gameworld);
	}

	public static IAutobuilderArea LoadArea(AutobuilderAreaTemplate area, IFuturemud gameworld)
	{
		return AreaLoaders[area.TemplateType.ToLowerInvariant()](area, gameworld);
	}

	public static IAutobuilderRoom LoadRoomFromBuilder(IFuturemud gameworld, string type, string name)
	{
		if (!BuilderRoomLoaders.ContainsKey(type.ToLowerInvariant()))
		{
			return null;
		}

		return BuilderRoomLoaders[type.ToLowerInvariant()](gameworld, name);
	}


	public static IAutobuilderArea LoadAreaFromBuilder(IFuturemud gameworld, string type, string name)
	{
		if (!BuilderAreaLoaders.ContainsKey(type.ToLowerInvariant()))
		{
			return null;
		}

		return BuilderAreaLoaders[type.ToLowerInvariant()](gameworld, name);
	}

	public static void InitialiseAutobuilders()
	{
		foreach (
			var type in
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(x => x.IsSubclassOf(typeof(Rooms.AutobuilderRoomBase)) ||
			                    x.IsSubclassOf(typeof(Areas.AutobuilderAreaBase))))
		{
			var method = type.GetMethod("RegisterAutobuilderLoader", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, new object[] { });
		}
	}
}