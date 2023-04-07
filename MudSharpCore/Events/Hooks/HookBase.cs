using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Events.Hooks;

public abstract class HookBase : SaveableItem, IHook
{
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

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; }

	#endregion

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
		foreach (var type in Futuremud.GetAllTypes().Where(x => x.IsSubclassOf(typeof(HookBase)))
		        )
		{
			var method = type.GetMethod("RegisterLoader", BindingFlags.Public | BindingFlags.Static);
			if (method != null)
			{
				method.Invoke(null, null);
			}
		}
	}

	#region IHook Members

	public abstract Func<EventType, object[], bool> Function { get; }

	public EventType Type { get; }

	public string Category { get; set; }

	public abstract string InfoForHooklist { get; }

	#endregion

	#region Implementation of ISaveable

	public override void Save()
	{
		var dbitem = FMDB.Context.Hooks.Find(Id);
		dbitem.Category = Category;
		Changed = true;
	}

	#endregion
}