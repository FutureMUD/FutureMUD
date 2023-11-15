using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Character;

namespace MudSharp.Magic.Capabilities;

public static class MagicCapabilityFactory
{
	public static Dictionary<string, Func<MagicCapability, IFuturemud, IMagicCapability>> CapabilityLoaders = new();

	public static Dictionary<string, Func<IFuturemud, ICharacter, StringStack, string, IMagicCapability>> BuilderLoaders = new(StringComparer.InvariantCultureIgnoreCase);

	public static void RegisterLoader(string type, Func<MagicCapability, IFuturemud, IMagicCapability> loader)
	{
		CapabilityLoaders[type] = loader;
	}

	public static void RegisterBuilderLoader(string type, Func<IFuturemud, ICharacter, StringStack, string, IMagicCapability> loader)
	{
		BuilderLoaders[type] = loader;
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

			method = type.GetMethod("RegisterBuilderLoader", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, new object[] { });
		}

		_initialised = true;
	}

	public static IMagicCapability LoadCapability(MagicCapability capability, IFuturemud gameworld)
	{
		InitialiseFactory();
		return CapabilityLoaders[capability.CapabilityModel](capability, gameworld);
	}

	public static IMagicCapability? LoaderFromBuilderInput(IFuturemud gameworld, ICharacter actor, StringStack input)
	{
		InitialiseFactory();
		if (input.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a magic capability type. The valid types are {BuilderLoaders.Keys.Select(x => x.ColourName()).ListToString()}.");
			return null;
		}
		var type = input.PopSpeech().ToLowerInvariant();
		if (!BuilderLoaders.ContainsKey(type))
		{
			actor.OutputHandler.Send($"That is not a valid magic capability type. The valid types are {BuilderLoaders.Keys.Select(x => x.ColourName()).ListToString()}.");
			return null;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a name for your new magic capability.");
			return null;
		}

		var name = input.PopSpeech().ToLowerInvariant().TitleCase();
		if (gameworld.MagicCapabilities.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a magic capability called {name.ColourName()}. Names must be unique.");
			return null;
		}

		return BuilderLoaders[type](gameworld, actor, input, name);
	}
}