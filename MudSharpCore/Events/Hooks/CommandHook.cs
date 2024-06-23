using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.Events.Hooks;

/// <summary>
///     A CommandHook forces a specified parameter (which must be an IControllable) to execute a command
/// </summary>
public class CommandHook : HookBase, ICommandHook
{
	protected int _commandExecuterIndex;
	private string _commandToExecute;

	public CommandHook(Models.Hooks hook, IFuturemud gameworld)
		: base(hook, gameworld)
	{
		LoadFromXml(XElement.Parse(hook.Definition));
	}
	
	#region IHook Members

	public override Func<EventType, object[], bool> Function
	{
		get
		{
			return (type, paramlist) =>
			{
				if (type != Type)
				{
					return false;
				}

				((IControllable)paramlist.ElementAt(_commandExecuterIndex)).ExecuteCommand(
					CommandToExecute(paramlist));
				return true;
			};
		}
	}

	#endregion

	public static void RegisterLoader()
	{
		HookLoaders.Add("CommandHook", (hook, gameworld) => new CommandHook(hook, gameworld));
	}

	protected virtual void LoadFromXml(XElement root)
	{
		_commandExecuterIndex = int.Parse(root.Attribute("CommandExecutorIndex").Value);
		var element = root.Element("CommandToExecute");
		if (element != null)
		{
			_commandToExecute = element.Value;
		}
	}

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("CommandExecutorIndex", _commandExecuterIndex),
			new XElement("CommandToExecute", new XCData(_commandToExecute))
		);
	}

	protected virtual string CommandToExecute(object[] parameters)
	{
		return _commandToExecute;
	}

	public override string InfoForHooklist => $"Executes {_commandToExecute.ColourCommand()}";

	public string CommandText
	{
		get => _commandToExecute;
		set => _commandToExecute = value;
	}
}