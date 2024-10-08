using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;

namespace MudSharp.Magic.Generators;

public class StateGenerator : BaseMagicResourceGenerator
{
	private List<(IFutureProg StateProg, IEnumerable<(IMagicResource Resource, double Amount)> Resources)> _states =
		new();

	private bool _checkAllStates;

	public StateGenerator(IFuturemud gameworld, string name, IMagicResource resource) : base(gameworld, name)
	{
		_states.Add((Gameworld.AlwaysTrueProg, new List<(IMagicResource, double)> { (resource, 1.0) }));
		_checkAllStates = false;

		using (new FMDB())
		{
			var dbitem = new Models.MagicGenerator
			{
				Name = name,
				Type = "state",
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.MagicGenerators.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected StateGenerator(StateGenerator rhs, string newName) : base(rhs.Gameworld, newName)
	{
		foreach (var state in rhs._states)
		{
			_states.Add((state.StateProg, state.Resources.ToList()));
		}

		_checkAllStates = rhs._checkAllStates;

		using (new FMDB())
		{
			var dbitem = new Models.MagicGenerator
			{
				Name = newName,
				Type = "state",
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.MagicGenerators.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public StateGenerator(Models.MagicGenerator generator, IFuturemud gameworld) : base(generator, gameworld)
	{
		var root = XElement.Parse(generator.Definition);

		_checkAllStates = bool.Parse(root.Element("CheckAllStates")?.Value ?? "false");

		var states = root.Element("States");
		if (states == null)
		{
			throw new ApplicationException($"StateGenerator #{Id} ({Name}) is missing a States element.");
		}

		foreach (var sub in states.Elements())
		{
			var resources = new List<(IMagicResource Resource, double Amount)>();
			foreach (var element in sub.Elements("Resource"))
			{
				var whichResource = long.TryParse(element.Attribute("resource")?.Value ?? "0", out var value)
					? gameworld.MagicResources.Get(value)
					: gameworld.MagicResources.GetByName(element.Attribute("resource")?.Value ?? "0");
				if (whichResource == null)
				{
					throw new ApplicationException(
						$"StateGenerator #{Id} ({Name}) specified an incorrect magic resource.");
				}

				var apm = element.Attribute("amountperminute")?.Value ?? "";

				if (!double.TryParse(apm, out var dvalue))
				{
					throw new ApplicationException(
						$"StateGenerator #{Id} ({Name}) specified an amountperminute amount that wasn't a number.");
				}

				resources.Add((whichResource, dvalue));
			}

			var progElement = sub.Element("StateProg");
			if (progElement == null)
			{
				throw new ApplicationException($"StateGenerator #{Id} ({Name}) did not have a StateProg element.");
			}

			var whichProg = long.TryParse(progElement.Value, out var pvalue)
				? gameworld.FutureProgs.Get(pvalue)
				: gameworld.FutureProgs.GetByName(progElement.Value);
			if (whichProg == null)
			{
				throw new ApplicationException(
					$"StateGenerator #{Id} ({Name}) specified an incorrect StateProg element.");
			}

			if (!whichProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
			{
				throw new ApplicationException(
					$"StateGenerator #{Id} ({Name}) specified a StateProg that doesn't return a boolean.");
			}

			if (!whichProg.MatchesParameters(new[] { FutureProgVariableTypes.MagicResourceHaver }))
			{
				throw new ApplicationException(
					$"StateGenerator #{Id} ({Name}) specified a StateProg that doesn't accept a MagicResourceHaver argument.");
			}

			_states.Add((whichProg, resources));
		}
	}

	protected override HeartbeatManagerDelegate InternalGetOnMinuteDelegate(IHaveMagicResource thing)
	{
		return () =>
		{
			foreach (var state in _states)
			{
				if (state.StateProg.Execute<bool?>(thing) == true)
				{
					foreach (var (resource, amount) in state.Resources)
					{
						thing.AddResource(resource, amount);
					}

					if (!_checkAllStates)
					{
						return;
					}
				}
			}
		};
	}

	#region Overrides of BaseMagicResourceGenerator
	public override string RegeneratorTypeName => "State-Dependent";
	/// <inheritdoc />
	public override IEnumerable<IMagicResource> GeneratedResources =>
		_states.SelectMany(x => x.Resources.Select(y => y.Resource)).Distinct();

	public override IMagicResourceRegenerator Clone(string name)
	{
		return new StateGenerator(this, name);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("CheckAllStates", _checkAllStates),
			new XElement("States",
				from state in _states
				select new XElement("State",
					new XElement("StateProg", state.StateProg.Id),
					from resource in state.Resources
					select new XElement("Resource", new XAttribute("resource", resource.Resource.Id), new XAttribute("amountperminute", resource.Amount))
				)
			)
		);
	}

	protected override string SubtypeHelpText => @"	#3state add <prog> <resource> <amount> [<resource...> <amount...>]#0 - creates a new state with specified resources
	#3state delete <index>#0 - deletes an existing state
	#3state swap <index1> <index2>#0 - swaps the position of two states in the order of resolution
	#3state <index> <resource> <amount>#0 - sets the amount of a resource a state produces (use 0.0 to remove)
	#3all#0 - toggles checking all states each minute rather than just the first valid one";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "state":
				return BuildingCommandState(actor, command);
			case "all":
				return BuildingCommandAll(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandAll(ICharacter actor, StringStack command)
	{
		_checkAllStates = !_checkAllStates;
		Changed = true;
		actor.OutputHandler.Send($"This generator will {_checkAllStates.NowNoLonger()} check all states rather than just the first each minute.");
		return true;
	}

	private bool BuildingCommandState(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to #3add#0 a state, #3delete#0 a state, #3swap#0 two states, or edit the properties of one?".SubstituteANSIColour());
			return false;
		}

		var cmd = command.PopSpeech();
		if (cmd.EqualToAny("remove", "delete"))
		{
			return BuildingCommandStateDelete(actor, command);
		}

		if (cmd.EqualToAny("swap", "reorder", "order"))
		{
			return BuildingCommandStateSwap(actor, command);
		}

		if (cmd.EqualToAny("add", "new", "create"))
		{
			return BuildingCommandStateAdd(actor, command);
		}

		return BuildingCommandStateEdit(actor, command);
	}

	private bool BuildingCommandStateEdit(ICharacter actor, StringStack command)
	{
		if (!int.TryParse(command.PopParentheses(), out var editIndex) || editIndex < 1 || editIndex > _states.Count)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid index equal or less than {_states.Count.ToStringN0(actor)}.");
			return false;
		}

		var resource = Gameworld.MagicResources.GetByIdOrName(command.PopSpeech());
		if (resource is null)
		{
			actor.OutputHandler.Send($"There is no magic resource identified by the text {command.Last.ColourCommand()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"How much {resource.Name.ColourValue()} should be generated per minute when this state is active?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}
		var (prog, resources) = _states[editIndex - 1];
		Changed = true;
		if (value == 0)
		{
			_states[editIndex - 1] = (prog, resources.Where(x => x.Resource != resource).ToList());
			actor.OutputHandler.Send($"The {editIndex.ToOrdinal().ColourValue()} state ({prog.MXPClickableFunctionName()}) will no longer produce the {resource.Name.ColourValue()} resource.");
			return true;
		}

		_states[editIndex - 1] = (prog, resources.Where(x => x.Resource != resource).Concat([(resource, value)]).ToList());
		actor.OutputHandler.Send($"The {editIndex.ToOrdinal().ColourValue()} state ({prog.MXPClickableFunctionName()}) will now produce {value.ToStringN2Colour(actor)} of the {resource.Name.ColourValue()} resource per minute.");
		return true;
	}

	private bool BuildingCommandStateDelete(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which state do you want to remove?");
			return false;
		}

		if (!int.TryParse(command.PopParentheses(), out var index) || index < 1 || index > _states.Count)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid index equal or less than {_states.Count.ToStringN0(actor)}.");
			return false;
		}

		var state = _states[index - 1];
		_states.RemoveAt(index - 1);
		Changed = true;
		actor.OutputHandler.Send($"You remove the {index.ToOrdinal().ColourValue()} state, with prog {state.StateProg.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandStateSwap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the first state that you want to swap?");
			return false;
		}

		if (!int.TryParse(command.PopParentheses(), out var index1) || index1 < 1 || index1 > _states.Count)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid index equal or less than {_states.Count.ToStringN0(actor)}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the second state that you want to swap?");
			return false;
		}

		if (!int.TryParse(command.PopParentheses(), out var index2) || index2 < 1 || index2 > _states.Count)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid index equal or less than {_states.Count.ToStringN0(actor)}.");
			return false;
		}

		if (index1 == index2)
		{
			actor.OutputHandler.Send("You can't swap a state with itself.");
			return false;
		}

		_states.Swap(index1-1, index2-1);
		actor.OutputHandler.Send($"You swap the order of the {index1.ToOrdinal().ColourValue()} ({_states[index2-1].StateProg.MXPClickableFunctionName()}) state and the {index2.ToOrdinal().ColourValue()} ({_states[index1 - 1].StateProg.MXPClickableFunctionName()}) state.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandStateAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog do you want to use to control whether this state is active?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.PopSpeech(), FutureProgVariableTypes.Boolean, [FutureProgVariableTypes.MagicResourceHaver]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		var resources = new List<(IMagicResource Resource, double Amount)>();
		while (!command.IsFinished)
		{
			var resource = Gameworld.MagicResources.GetByIdOrName(command.PopSpeech());
			if (resource is null)
			{
				actor.OutputHandler.Send($"There is no magic resource identified by the text {command.Last.ColourCommand()}.");
				return false;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send($"How much {resource.Name.ColourValue()} should be generated per minute when this state is active?");
				return false;
			}

			if (!double.TryParse(command.SafeRemainingArgument, actor.Account.Culture, out var value))
			{
				actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
				return false;
			}

			resources.Add((resource, value));
		}

		if (resources.Count == 0)
		{
			actor.OutputHandler.Send("You must specify at least one magic resource for this state.");
			return false;
		}

		_states.Add((prog, resources));
		Changed = true;
		actor.OutputHandler.Send($"When the prog {prog.MXPClickableFunctionName()} is true, this will now generate {resources.Select(x => $"{x.Amount.ToBonusString(actor)} {x.Resource.Name.ColourName()}").ListToString()} per minute.");
		return true;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Magic Regenerator #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.BoldMagenta, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine("Type: #2State Dependent Generator#0".SubstituteANSIColour());
		sb.AppendLine($"Check All: {_checkAllStates.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("States".GetLineWithTitle(actor, Telnet.BoldMagenta, Telnet.BoldWhite));
		var i = 1;
		foreach (var state in _states)
		{
			sb.AppendLine();
			sb.AppendLine($"State #{i++} - {state.StateProg.MXPClickableFunctionName()}");
			sb.AppendLine($"Resources Per Minute: {state.Resources.Select(x => $"{x.Amount.ToBonusString(actor)} {x.Resource.Name.ColourName()}").ListToString()}");
		}

		return sb.ToString();
	}

	#endregion
}