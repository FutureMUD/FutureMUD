using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Magic.Capabilities;

public static class MagicCapabilityFactory
{
	public static Dictionary<string, Func<MagicCapability, IFuturemud, IMagicCapability>> CapabilityLoaders = new();

	public static void RegisterLoader(string type, Func<MagicCapability, IFuturemud, IMagicCapability> loader)
	{
		CapabilityLoaders[type] = loader;
	}

	private static bool _initialised;

	private static void InitialiseFactory()
	{
		if (_initialised)
		{
			return;
		}

		var iType = typeof(IMagicCapability);
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

	public static IMagicCapability LoadCapability(MagicCapability capability, IFuturemud gameworld)
	{
		InitialiseFactory();
		return CapabilityLoaders[capability.CapabilityModel](capability, gameworld);
	}
}