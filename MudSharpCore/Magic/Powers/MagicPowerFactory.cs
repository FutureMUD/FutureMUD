using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Magic.Powers;

public static class MagicPowerFactory
{
	public static Dictionary<string, Func<MagicPower, IFuturemud, IMagicPower>> PowerLoaders = new();

	public static void RegisterLoader(string type, Func<MagicPower, IFuturemud, IMagicPower> loader)
	{
		PowerLoaders[type] = loader;
	}

	private static bool _initialised;

	private static void InitialiseFactory()
	{
		if (_initialised)
		{
			return;
		}

		var iType = typeof(IMagicPower);
		foreach (
			var type in
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(x => x.GetInterfaces().Contains(iType)))
		{
			var method = type.GetMethod("RegisterLoader", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, new object[] { });
		}

		_initialised = true;
	}

	public static IMagicPower LoadPower(MagicPower power, IFuturemud gameworld)
	{
		InitialiseFactory();
		return PowerLoaders[power.PowerModel](power, gameworld);
	}
}