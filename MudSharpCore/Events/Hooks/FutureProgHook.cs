using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Events.Hooks;

public class FutureProgHook : HookBase, IHookWithProgs
{
	private readonly List<IFutureProg> _futureProgs = new();

	public FutureProgHook(Models.Hooks hook, IFuturemud gameworld)
		: base(hook, gameworld)
	{
		LoadFromXml(XElement.Parse(hook.Definition));
	}

	public IEnumerable<IFutureProg> FutureProgs => _futureProgs;

	public FutureProgHook(string name, IFuturemud gameworld, EventType type, IFutureProg prog) : base(gameworld, type)
	{
		_name = name;
		Category = "Uncategorised";
		_futureProgs.Add(prog);
		using (new FMDB())
		{
			var dbitem = new Models.Hooks();
			FMDB.Context.Hooks.Add(dbitem);
			dbitem.Name = name;
			dbitem.Definition = new XElement("Definition", new XElement("FutureProg", prog.Id)).ToString();
			dbitem.Category = "Uncategorised";
			dbitem.TargetEventType = (int)type;
			dbitem.Type = "FutureProgHook";
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		Gameworld.Add(this);
	}

	public override Func<EventType, object[], bool> Function
	{
		get
		{
			return (type, paramList) =>
			{
				if (type != Type)
				{
					return false;
				}

				foreach (var prog in _futureProgs)
				{
					prog.Execute(paramList);
				}

				return true;
			};
		}
	}

	public static void RegisterLoader()
	{
		HookLoaders.Add("FutureProgHook", (hook, gameworld) => new FutureProgHook(hook, gameworld));
	}

	public void LoadFromXml(XElement element)
	{
		foreach (var item in element.Elements("FutureProg"))
		{
			_futureProgs.Add(long.TryParse(item.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(item.Value));
		}
	}

	public override string InfoForHooklist =>
		$"Executes {_futureProgs.SelectNotNull(x => x.MXPClickableFunctionName()).ListToString()}";
}