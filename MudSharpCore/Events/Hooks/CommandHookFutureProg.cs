using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Events.Hooks;

public class CommandHookFutureProg : CommandHook, IHookWithProgs
{
	private IFutureProg CommandProg;
	public IEnumerable<IFutureProg> FutureProgs => new IFutureProg[] { CommandProg };

	public CommandHookFutureProg(Models.Hooks hook, IFuturemud gameworld)
		: base(hook, gameworld)
	{
	}

	public override string FrameworkItemType => "CommandHookFutureProg";

	public new static void RegisterLoader()
	{
		HookLoaders.Add("CommandHookFutureProg", (hook, gameworld) => new CommandHookFutureProg(hook, gameworld));
	}

	protected override void LoadFromXml(XElement root)
	{
		CommandProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("FutureProg").Value));
		if (CommandProg.ReturnType != FutureProgVariableTypes.Text)
		{
			Console.WriteLine("Warning: CommandHookFutureProg " + Id + " has a non-text prog.");
		}

		base.LoadFromXml(root);
	}

	protected override string CommandToExecute(object[] parameters)
	{
		return CommandProg.Execute(parameters).ToString();
	}

	public override string InfoForHooklist => $"Executes from {CommandProg.MXPClickableFunctionName()}";
}