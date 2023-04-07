using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Events.Hooks;

public class HookOnInput : HookBase, IHookWithProgs
{
	private readonly List<IFutureProg> _futureProgs = new();
	public IEnumerable<IFutureProg> FutureProgs => _futureProgs;

	public HookOnInput(string name, string command, IFuturemud gameworld, EventType type, IFutureProg prog) : base(
		gameworld, type)
	{
		TargetCommand = command;
		_name = name;
		Category = "Uncategorised";
		_futureProgs.Add(prog);
		using (new FMDB())
		{
			var dbitem = new Models.Hooks();
			FMDB.Context.Hooks.Add(dbitem);
			dbitem.Name = name;
			dbitem.Definition = new XElement("Definition", new XElement("TargetCommand", new XCData(TargetCommand)),
				new XElement("FutureProg", prog.Id)).ToString();
			dbitem.Category = "Uncategorised";
			dbitem.TargetEventType = (int)type;
			dbitem.Type = "HookOnInput";
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		Gameworld.Add(this);
	}

	public HookOnInput(Models.Hooks hook, IFuturemud gameworld) : base(hook, gameworld)
	{
		LoadFromXml(XElement.Parse(hook.Definition));
	}

	public string TargetCommand { get; set; }

	#region Overrides of Item

	public override string FrameworkItemType { get; } = "HookOnInput";

	#endregion

	#region Overrides of HookBase

	public override Func<EventType, object[], bool> Function
	{
		get
		{
			return (type, paramList) =>
			{
				if (type != EventType.CommandInput && type != EventType.SelfCommandInput)
				{
					return false;
				}

				var cmd = (type == EventType.CommandInput ? paramList[2] : paramList[1]).ToString();
				if (!cmd.EqualTo(TargetCommand))
				{
					return false;
				}

				var ss = (StringStack)(type == EventType.CommandInput ? paramList[3] : paramList[2]);
				ss.PopAll();

				foreach (var prog in _futureProgs)
				{
					if (type == EventType.CommandInput)
					{
						prog.Execute((ICharacter)paramList[0], paramList[1], cmd, ss.Memory);
					}
					else
					{
						prog.Execute((ICharacter)paramList[0], cmd, ss.Memory);
					}
				}

				return true;
			};
		}
	}

	#endregion

	public void LoadFromXml(XElement element)
	{
		TargetCommand = element.Element("TargetCommand").Value;
		foreach (var item in element.Elements("FutureProg"))
		{
			_futureProgs.Add(long.TryParse(item.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(item.Value));
		}
	}

	public static void RegisterLoader()
	{
		HookLoaders.Add("HookOnInput", (hook, gameworld) => new HookOnInput(hook, gameworld));
	}

	public override string InfoForHooklist =>
		$"Executes {_futureProgs.SelectNotNull(x => x.MXPClickableFunctionName()).ListToString()}";
}