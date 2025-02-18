using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Character;

namespace MudSharp.Magic.Powers;

#nullable enable
public static class MagicPowerFactory
{
	public static Dictionary<string, Func<MagicPower, IFuturemud, IMagicPower>> PowerLoaders = new();

	public static DictionaryWithDefault<string, Func<IFuturemud, IMagicSchool, string, ICharacter, StringStack, IMagicPower?>> PowerBuilderLoaders = new();

	public static void RegisterLoader(string type, Func<MagicPower, IFuturemud, IMagicPower> loader)
	{
		PowerLoaders[type] = loader;
	}

	public static void RegisterBuilderLoader(string type, Func<IFuturemud, IMagicSchool, string, ICharacter, StringStack, IMagicPower?> loader)
	{
		PowerBuilderLoaders[type] = loader;
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
			method?.Invoke(null, []);
		}

		_initialised = true;
	}

	public static IMagicPower LoadPower(MagicPower power, IFuturemud gameworld)
	{
		InitialiseFactory();
		return PowerLoaders[power.PowerModel](power, gameworld);
	}

	public static IMagicPower? LoadPowerFromBuilderInput(IFuturemud gameworld, IMagicSchool school, string name, string type, ICharacter actor, StringStack command)
	{
		InitialiseFactory();
		return PowerBuilderLoaders[type]?.Invoke(gameworld, school, name, actor, command);
	}

	public static IEnumerable<string> BuilderTypes
	{
		get
		{
			InitialiseFactory();
			return PowerBuilderLoaders.Keys;
		}
	}
}