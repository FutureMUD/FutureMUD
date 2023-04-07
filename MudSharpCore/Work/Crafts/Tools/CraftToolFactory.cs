using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MudSharp.Framework;

namespace MudSharp.Work.Crafts.Tools;

public class CraftToolFactory
{
	public static CraftToolFactory Factory { get; } = new();

	private CraftToolFactory()
	{
		DatabaseConstructors = new Dictionary<string, Func<Models.CraftTool, ICraft, IFuturemud, ICraftTool>>();
		BuilderConstructors = new Dictionary<string, Func<ICraft, IFuturemud, ICraftTool>>();
		var iType = typeof(ICraftTool);
		foreach (
			var type in
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(x => x.GetInterfaces().Contains(iType)))
		{
			var method = type.GetMethod("RegisterCraftTool", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, new object[] { });
		}
	}

	public ICraftTool LoadTool(Models.CraftTool tool, ICraft craft, IFuturemud gameworld)
	{
		return DatabaseConstructors[tool.ToolType](tool, craft, gameworld);
	}

	public ICraftTool LoadTool(string builderSuppliedType, ICraft craft, IFuturemud gameworld)
	{
		return BuilderConstructors.ValueOrDefault(builderSuppliedType.ToLowerInvariant(), null)
		                          ?.Invoke(craft, gameworld);
	}

	private static Dictionary<string, Func<Models.CraftTool, ICraft, IFuturemud, ICraftTool>> DatabaseConstructors;
	private static Dictionary<string, Func<ICraft, IFuturemud, ICraftTool>> BuilderConstructors;

	public static void RegisterCraftToolType(string type,
		Func<Models.CraftTool, ICraft, IFuturemud, ICraftTool> constructorFunc)
	{
		DatabaseConstructors[type] = constructorFunc;
	}

	public static void RegisterCraftToolTypeForBuilders(string type,
		Func<ICraft, IFuturemud, ICraftTool> constructorFunc)
	{
		BuilderConstructors[type] = constructorFunc;
	}

	public IEnumerable<string> ValidBuilderTypes => BuilderConstructors.Keys.ToArray();
}