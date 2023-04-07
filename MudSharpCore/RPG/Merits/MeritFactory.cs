using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.RPG.Merits;

public static class MeritFactory
{
	private static readonly Dictionary<string, Func<Merit, IFuturemud, IMerit>> MeritInitialisers =
		new();

	public static void InitialiseMerits()
	{
		var mType = typeof(IMerit);
		foreach (
			var method in
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(x => x.GetInterfaces().Contains(mType))
			        .Select(
				        type => type.GetMethod("RegisterMeritInitialiser", BindingFlags.Public | BindingFlags.Static))
			        .Where(method => method != null))
		{
			method.Invoke(null, null);
		}
	}

	public static void RegisterMeritInitialiser(string type, Func<Merit, IFuturemud, IMerit> func)
	{
		MeritInitialisers[type] = func;
	}

	public static IMerit LoadMerit(Merit merit, IFuturemud gameworld)
	{
		if (MeritInitialisers.ContainsKey(merit.Type))
		{
			return MeritInitialisers[merit.Type](merit, gameworld);
		}

		throw new ApplicationException("MeritFactory.LoadMerit couldn't find a Merit Initialiser for type " +
		                               merit.Type + ".");
	}
}