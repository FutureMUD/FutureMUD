using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Events.Hooks;

public class CommandHookFutureProg : CommandHook, IHookWithProgs, IExecuteProgHook
{
	private IFutureProg _commandProg;
	public IEnumerable<IFutureProg> FutureProgs => new[] { _commandProg };

	public CommandHookFutureProg(Models.Hooks hook, IFuturemud gameworld)
		: base(hook, gameworld)
	{
	}

	public new static void RegisterLoader()
	{
		HookLoaders.Add("CommandHookFutureProg", (hook, gameworld) => new CommandHookFutureProg(hook, gameworld));
	}

	protected override void LoadFromXml(XElement root)
	{
		_commandProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("FutureProg").Value));
		if (_commandProg?.ReturnType != ProgVariableTypes.Text)
		{
			Console.WriteLine("Warning: CommandHookFutureProg " + Id + " has a non-text prog.");
		}

		base.LoadFromXml(root);
	}

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("FutureProg", _commandProg.Id),
			new XElement("CommandExecutorIndex", _commandExecuterIndex),
			new XElement("CommandToExecute", "")
		);
	}

	protected override string CommandToExecute(object[] parameters)
	{
		return _commandProg.Execute(parameters).ToString();
	}

	public override string InfoForHooklist => $"Executes from {_commandProg.MXPClickableFunctionName()}";

	/// <inheritdoc />
	public void AddProg(IFutureProg prog)
	{
		_commandProg = prog;
	}

	public bool RemoveProg(IFutureProg prog)
	{
		return false;
	}
}