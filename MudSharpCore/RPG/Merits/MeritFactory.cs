using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.RPG.Merits;

#nullable enable
public static class MeritFactory
{
	private static readonly Dictionary<string, Func<Merit, IFuturemud, IMerit>> MeritInitialisers =
		new();

	private static readonly Dictionary<string, Func<IFuturemud, string, IMerit>> MeritBuilderInitialisers = new();

	private static readonly Dictionary<string, (string Blurb, string HelpText)> MeritHelpTexts = new();

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
			method?.Invoke(null, null);
		}
	}

	public static void RegisterMeritInitialiser(string type, Func<Merit, IFuturemud, IMerit> func)
	{
		MeritInitialisers[type] = func;
	}

	public static void RegisterBuilderMeritInitialiser(string type, Func<IFuturemud, string, IMerit> func)
	{
		MeritBuilderInitialisers[type] = func;
	}

	public static void RegisterMeritHelp(string type, string blurb, string helpText)
	{
		MeritHelpTexts[type] = (blurb,helpText);
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

	public static IMerit? LoadMeritFromBuilder(IFuturemud gameworld, string type, string name)
	{
		if (MeritBuilderInitialisers.ContainsKey(type))
		{
			return MeritBuilderInitialisers[type](gameworld, name);
		}

		return null;
	}


	public static IEnumerable<string> Types => MeritBuilderInitialisers.Keys.ToList();

	public static IEnumerable<(string Type, string Blurb, string HelpText)> TypeHelps => MeritHelpTexts.Select(x => (x.Key, x.Value.Blurb, x.Value.HelpText)).ToList();
}