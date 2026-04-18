using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Magic;

public static class SpellTriggerFactory
{
    static SpellTriggerFactory()
    {
        SetupFactory();
    }

    private static readonly Dictionary<string, Func<XElement, IMagicSpell, IMagicTrigger>> _loadTimeFactories =
        new(StringComparer.InvariantCultureIgnoreCase);

    public static IMagicTrigger LoadTrigger(XElement definition, IMagicSpell parent)
    {
        string type = definition.Attribute("type")?.Value ?? string.Empty;
        if (!_loadTimeFactories.ContainsKey(type))
        {
            throw new ApplicationException(
                $"Unknown spell trigger type {type} in SpellTriggerFactory.LoadTrigger, from spell #{parent.Id}");
        }

        return _loadTimeFactories[type](definition, parent);
    }

    public static void RegisterLoadTimeFactory(string type, Func<XElement, IMagicSpell, IMagicTrigger> factory)
    {
        _loadTimeFactories[type] = factory;
    }

    private static readonly
        Dictionary<string,
            (Func<StringStack, IMagicSpell, (IMagicTrigger Trigger, string Error)> Loader,
            string Blurb,
            string TargetTypes,
            string BuilderHelp)
        >
        _builderFactories = new(StringComparer.InvariantCultureIgnoreCase);

    public static (IMagicTrigger Trigger, string Error) LoadTriggerFromBuilderInput(string type,
        StringStack furtherArguments, IMagicSpell parent)
    {
        if (!_builderFactories.ContainsKey(type))
        {
            return (null, "There is no such magic spell trigger type.");
        }

        return _builderFactories[type].Loader(furtherArguments, parent);
    }

    public static void RegisterBuilderFactory(string type,
        Func<StringStack, IMagicSpell, (IMagicTrigger Trigger, string Error)> factory,
        string blurb,
        string targetTypes,
        string builderHelp)
    {
        _builderFactories[type] = (factory, blurb, targetTypes, builderHelp);
    }

    public static IEnumerable<string> MagicTriggerTypes => _builderFactories.Keys;

    public static (string Blurb, string TargetTypes, string BuilderHelp) BuilderInfoForType(string type)
    {
        (Func<StringStack, IMagicSpell, (IMagicTrigger Trigger, string Error)> _, string blurb, string targets, string help) = _builderFactories[type];
        return (blurb, targets, help);
    }

    public static void SetupFactory()
    {
        Type fpType = typeof(IMagicTrigger);
        foreach (
            Type type in Futuremud.GetAllTypes().Where(x => x.GetInterfaces().Contains(fpType)))
        {
            MethodInfo method = type.GetMethod("RegisterFactory", BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, null);
        }
    }
}