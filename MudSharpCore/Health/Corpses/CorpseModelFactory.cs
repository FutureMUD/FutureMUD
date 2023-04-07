using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MudSharp.Framework;

namespace MudSharp.Health.Corpses;

public static class CorpseModelFactory
{
	private static readonly Dictionary<string, Func<MudSharp.Models.CorpseModel, IFuturemud, ICorpseModel>> Loaders =
		new();

	public static void RegisterCorpseModelTypeLoaders()
	{
		var cmType = typeof(ICorpseModel);
		foreach (
			var type in Futuremud.GetAllTypes().Where(x => x.GetInterfaces().Contains(cmType)))
		{
			var method = type.GetMethod("RegisterTypeLoader", BindingFlags.Public | BindingFlags.Static);
			method?.Invoke(null, null);
		}
	}

	public static void RegisterCorpseModelType(string type,
		Func<MudSharp.Models.CorpseModel, IFuturemud, ICorpseModel> loaderFunc)
	{
		Loaders.Add(type, loaderFunc);
	}

	public static ICorpseModel LoadCorpseModel(MudSharp.Models.CorpseModel model, IFuturemud gameworld)
	{
		if (!Loaders.ContainsKey(model.Type))
		{
			throw new ApplicationException(
				$"Tried to load a corpse model of type {model.Type}, which is not an actual type.");
		}

		return Loaders[model.Type](model, gameworld);
	}
}