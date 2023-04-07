using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

public static class GroupAITypeFactory
{
	static GroupAITypeFactory()
	{
		InitialiseGroupAITypes();
	}

	private static Dictionary<string, Func<XElement, IFuturemud, IGroupAIType>> _databaseLoaders = new();

	private static Dictionary<string, Func<string, IFuturemud, (IGroupAIType Type, string Error)>> _builderLoaders =
		new();

	public static void RegisterGroupAIType(string typeName, Func<XElement, IFuturemud, IGroupAIType> databaseLoader,
		Func<string, IFuturemud, (IGroupAIType Type, string Error)> builderLoader)
	{
		_databaseLoaders[typeName] = databaseLoader;
		_builderLoaders[typeName] = builderLoader;
	}

	public static IGroupAIType LoadFromDatabase(XElement root, IFuturemud gameworld)
	{
		var typename = root.Attribute("typename").Value;
		if (typename == "invalid")
		{
			return null;
		}

		return _databaseLoaders[typename](root, gameworld);
	}

	public static (IGroupAIType Type, string Error) LoadFromBuilderArguments(string typeName, string arguments,
		IFuturemud gameworld)
	{
		if (!_builderLoaders.ContainsKey(typeName))
		{
			return (null, "There is no such Group AI Type.");
		}

		return _builderLoaders[typeName](arguments, gameworld);
	}

	public static IEnumerable<string> GetBuilderTypeNames()
	{
		return _builderLoaders.Keys.ToList();
	}

	public static void InitialiseGroupAITypes()
	{
		var iType = typeof(IGroupAIType);
		foreach (
			var type in
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(x => x.GetInterfaces().Contains(iType)))
		{
			var method = type.GetMethod("RegisterGroupAIType", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, new object[] { });
		}
	}
}