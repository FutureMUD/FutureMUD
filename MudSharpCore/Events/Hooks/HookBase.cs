using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp.Events.Hooks;

public abstract class HookBase : SaveableItem, IHook
{
    public sealed override string FrameworkItemType => "Hook";

    protected static Dictionary<string, Func<Models.Hooks, IFuturemud, IHook>> HookLoaders =
        new();

    protected HookBase(Models.Hooks hook, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _id = hook.Id;
        _name = hook.Name;
        Category = hook.Category;
        Type = (EventType)hook.TargetEventType;
    }

    protected HookBase(IFuturemud gameworld, EventType type)
    {
        Gameworld = gameworld;
        Type = type;
    }

    public static IHook LoadHook(Models.Hooks hook, IFuturemud gameworld)
    {
        if (HookLoaders.ContainsKey(hook.Type))
        {
            return HookLoaders[hook.Type](hook, gameworld);
        }

        throw new NotSupportedException();
    }

    public static void SetupHooks()
    {
        foreach (Type type in Futuremud.GetAllTypes().Where(x => x.IsSubclassOf(typeof(HookBase)))
                )
        {
            MethodInfo method = type.GetMethod("RegisterLoader", BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, null);
        }
    }

    #region IHook Members

    public abstract Func<EventType, object[], bool> Function { get; }

    public EventType Type { get; }

    public string Category { get; set; }

    string IHook.Name
    {
        get => _name;
        set => _name = value;
    }

    public abstract string InfoForHooklist { get; }

    #endregion

    #region Implementation of ISaveable

    public sealed override void Save()
    {
        Models.Hooks dbitem = FMDB.Context.Hooks.Find(Id);
        dbitem.Category = Category;
        dbitem.Name = Name;
        dbitem.Definition = SaveDefinition().ToString();
        Changed = false;
    }

    protected abstract XElement SaveDefinition();

    #endregion
}