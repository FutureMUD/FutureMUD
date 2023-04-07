using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MudSharp.Framework;

namespace MudSharp.Work.Crafts.Products;

public class CraftProductFactory
{
	public static CraftProductFactory Factory { get; } = new();

	private CraftProductFactory()
	{
		DatabaseConstructors = new Dictionary<string, Func<Models.CraftProduct, ICraft, IFuturemud, ICraftProduct>>();
		BuilderConstructors = new Dictionary<string, Func<ICraft, IFuturemud, bool, ICraftProduct>>();
		var iType = typeof(ICraftProduct);
		foreach (
			var type in
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(x => x.GetInterfaces().Contains(iType)))
		{
			var method = type.GetMethod("RegisterCraftProduct", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, new object[] { });
		}
	}

	public ICraftProduct LoadProduct(Models.CraftProduct product, ICraft craft, IFuturemud gameworld)
	{
		return DatabaseConstructors[product.ProductType](product, craft, gameworld);
	}

	public ICraftProduct LoadProduct(string builderSuppliedType, ICraft craft, IFuturemud gameworld, bool failproduct)
	{
		return BuilderConstructors.ValueOrDefault(builderSuppliedType.ToLowerInvariant(), null)
		                          ?.Invoke(craft, gameworld, failproduct);
	}

	private static Dictionary<string, Func<Models.CraftProduct, ICraft, IFuturemud, ICraftProduct>>
		DatabaseConstructors;

	private static Dictionary<string, Func<ICraft, IFuturemud, bool, ICraftProduct>> BuilderConstructors;

	public static void RegisterCraftProductType(string type,
		Func<Models.CraftProduct, ICraft, IFuturemud, ICraftProduct> constructorFunc)
	{
		DatabaseConstructors[type] = constructorFunc;
	}

	public static void RegisterCraftProductTypeForBuilders(string type,
		Func<ICraft, IFuturemud, bool, ICraftProduct> constructorFunc)
	{
		BuilderConstructors[type] = constructorFunc;
	}

	public IEnumerable<string> ValidBuilderTypes => BuilderConstructors.Keys.ToArray();
}