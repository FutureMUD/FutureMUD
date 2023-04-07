using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MudSharp.Framework;

namespace MudSharp.Work.Crafts.Inputs;

public class CraftInputFactory
{
	public static CraftInputFactory Factory { get; } = new();

	private CraftInputFactory()
	{
		DatabaseConstructors = new Dictionary<string, Func<Models.CraftInput, ICraft, IFuturemud, ICraftInput>>();
		BuilderConstructors = new Dictionary<string, Func<ICraft, IFuturemud, ICraftInput>>();
		var iType = typeof(ICraftInput);
		foreach (
			var type in
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(x => x.GetInterfaces().Contains(iType)))
		{
			var method = type.GetMethod("RegisterCraftInput", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, new object[] { });
		}
	}

	public ICraftInput LoadInput(Models.CraftInput input, ICraft craft, IFuturemud gameworld)
	{
		return DatabaseConstructors[input.InputType](input, craft, gameworld);
	}

	public ICraftInput LoadInput(string builderSuppliedType, ICraft craft, IFuturemud gameworld)
	{
		return BuilderConstructors.ValueOrDefault(builderSuppliedType.ToLowerInvariant(), null)
		                          ?.Invoke(craft, gameworld);
	}

	private static Dictionary<string, Func<Models.CraftInput, ICraft, IFuturemud, ICraftInput>> DatabaseConstructors;
	private static Dictionary<string, Func<ICraft, IFuturemud, ICraftInput>> BuilderConstructors;

	public static void RegisterCraftInputType(string type,
		Func<Models.CraftInput, ICraft, IFuturemud, ICraftInput> constructorFunc)
	{
		DatabaseConstructors[type] = constructorFunc;
	}

	public static void RegisterCraftInputTypeForBuilders(string type,
		Func<ICraft, IFuturemud, ICraftInput> constructorFunc)
	{
		BuilderConstructors[type] = constructorFunc;
	}

	public IEnumerable<string> ValidBuilderTypes => BuilderConstructors.Keys.ToArray();
}