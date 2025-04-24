using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.Magic;

public static class SpellEffectFactory
{
	static SpellEffectFactory()
	{
		SetupFactory();
	}

	private static readonly Dictionary<string, Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>>
		_loadTimeFactories =
			new(StringComparer.InvariantCultureIgnoreCase);

	public static IMagicSpellEffectTemplate LoadEffect(XElement definition, IMagicSpell parent)
	{
		var type = definition.Attribute("type")?.Value ?? string.Empty;
		if (!_loadTimeFactories.ContainsKey(type))
		{
			throw new ApplicationException(
				$"Unknown spell trigger type {type} in SpellTriggerFactory.LoadTrigger, from spell #{parent.Id}");
		}

		return _loadTimeFactories[type](definition, parent);
	}

	public static void RegisterLoadTimeFactory(string type,
		Func<XElement, IMagicSpell, IMagicSpellEffectTemplate> factory)
	{
		_loadTimeFactories[type] = factory;
	}

	private static readonly
		Dictionary<string, 
			(Func<StringStack, IMagicSpell, (IMagicSpellEffectTemplate Trigger, string Error)> Factory,
			string Blurb,
			string Help,
			bool Instant,
			bool RequiresTarget,
			string[] MatchingTriggers
			)
		>
		_builderFactories = new(StringComparer.InvariantCultureIgnoreCase);

	public static (IMagicSpellEffectTemplate Trigger, string Error) LoadEffectFromBuilderInput(string type,
		StringStack furtherArguments, IMagicSpell parent)
	{
		if (!_builderFactories.ContainsKey(type))
		{
			return (null, "There is no such magic spell trigger type.");
		}

		return _builderFactories[type].Factory(furtherArguments, parent);
	}

	public static void RegisterBuilderFactory(string type,
		Func<StringStack, IMagicSpell, (IMagicSpellEffectTemplate Trigger, string Error)> factory,
		string blurb,
		string help,
		bool instant,
		bool requiresTarget,
		string[] matchingTriggers)
	{
		_builderFactories[type] = (factory, blurb, help, instant, requiresTarget, matchingTriggers);
	}

	public static IEnumerable<string> MagicEffectTypes => _builderFactories.Keys;

	public static (string Blurb, string BuilderHelp, bool Instant, bool RequiresTarget, string[] MatchingTriggers) BuilderInfoForType(string type)
	{
		var (_, blurb, help, instant, requiresTarget, matchingTriggers) = _builderFactories[type];
		return (blurb, help, instant, requiresTarget, matchingTriggers);
	}

	public static void SetupFactory()
	{
		var fpType = typeof(IMagicSpellEffectTemplate);
		foreach (
			var type in Futuremud.GetAllTypes().Where(x => x.GetInterfaces().Contains(fpType)))
		{
			var method = type.GetMethod("RegisterFactory", BindingFlags.Public | BindingFlags.Static);
			method?.Invoke(null, null);
		}
	}
}