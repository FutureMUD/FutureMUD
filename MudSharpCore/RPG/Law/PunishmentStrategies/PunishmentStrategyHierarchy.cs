﻿using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Law.PunishmentStrategies;

public class PunishmentStrategyHierarchy : PunishmentStrategyBase
{
	private readonly List<(IFutureProg Prog, IPunishmentStrategy Strategy)> _strategyHierarchy = new();

	public PunishmentStrategyHierarchy(IFuturemud gameworld, XElement root, ILegalAuthority authority) : base(gameworld,
		root)
	{
		foreach (var item in root.Elements("Member"))
		{
			_strategyHierarchy.Add((Gameworld.FutureProgs.Get(long.Parse(item.Attribute("prog").Value)),
				PunishmentStrategyFactory.LoadStrategy(gameworld, item.InnerXML(), authority)));
		}
	}

	public PunishmentStrategyHierarchy(IFuturemud gameworld) : base(gameworld)
	{
	}

	public override string TypeSpecificHelpText => @"
    add <type> <prog> - adds a new punishment at the end of the hierarchy
    remove <#> - removes a punishment from the hierarchy
    swap <#> <#> - swaps the order of two punishments in the hierarchy
    prog <#> <prog> - edits the prog associated with a punishment
    <#> ... - edits the properties of the punishment in the hierarchy";

	public override bool BuildingCommand(ICharacter actor, ILegalAuthority authority, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandCreate(actor, authority, command);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandDelete(actor, command);
			case "swap":
			case "switch":
			case "order":
			case "reorder":
				return BuildingCommandSwap(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			default:
				if (int.TryParse(command.Last, out var index) && index > 0 && index < _strategyHierarchy.Count)
				{
					return _strategyHierarchy[index - 1].Strategy.BuildingCommand(actor, authority, command);
				}

				return base.BuildingCommand(actor, authority, command.GetUndo());
		}
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which numbered punishment do you want to change the prog for?");
			return false;
		}

		if (!_strategyHierarchy.Any())
		{
			actor.OutputHandler.Send("There aren't any punishments in the hierarchy for you to change the prog of.");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0 || value > _strategyHierarchy.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_strategyHierarchy.Count.ToString("N0", actor).ColourValue()}");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog would you like to use as a filter?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new[]
			{
				new[] { FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Text }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_strategyHierarchy[value - 1] = (prog, _strategyHierarchy[value - 1].Strategy);
		actor.OutputHandler.Send(
			$"The {value.ToOrdinal()} punishment will now use the {prog.MXPClickableFunctionName()} as a filter.");
		return true;
	}

	private bool BuildingCommandSwap(ICharacter actor, StringStack command)
	{
		if (_strategyHierarchy.Count < 2)
		{
			actor.OutputHandler.Send("There must be at least two elements in the list before you can swap them.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the index of the 1st value that you'd like to swap?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var index1) || index1 <= 0 || index1 > _strategyHierarchy.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_strategyHierarchy.Count.ToString("N0", actor).ColourValue()}");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the index of the 2nd value that you'd like to swap?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var index2) || index2 <= 0 || index2 > _strategyHierarchy.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_strategyHierarchy.Count.ToString("N0", actor).ColourValue()}");
			return false;
		}

		if (index1 == index2)
		{
			actor.OutputHandler.Send("You cannot swap an item with itself.");
			return false;
		}

		_strategyHierarchy.Swap(index1, index2);
		actor.OutputHandler.Send(
			$"You swap the positions of the {index1.ToOrdinal()} and {index2.ToOrdinal()} punishments in the hierarchy.");
		return true;
	}

	private bool BuildingCommandDelete(ICharacter actor, StringStack command)
	{
		if (!_strategyHierarchy.Any())
		{
			actor.OutputHandler.Send("There aren't any punishments in the hierarchy for you to delete.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the index of the punishment that you wish to delete?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0 || value > _strategyHierarchy.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_strategyHierarchy.Count.ToString("N0", actor).ColourValue()}");
			return false;
		}

		var punishment = _strategyHierarchy[value - 1];
		_strategyHierarchy.RemoveAt(value - 1);
		actor.OutputHandler.Send(
			$"You delete the punishment {punishment.Strategy.Describe(actor).Colour(Telnet.BoldRed)}.");
		return true;
	}

	private bool BuildingCommandCreate(ICharacter actor, ILegalAuthority authority, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What type of punishment do you want to add to the hierarchy?");
			return false;
		}

		var strategy = PunishmentStrategyFactory.GetStrategyFromBuilderInput(Gameworld, authority, command.PopSpeech());
		throw new NotImplementedException();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"a hierarchy of {_strategyHierarchy.Count.ToString("N0", voyeur).ColourIncludingReset(Telnet.Green)} different punishments";
	}

	public override PunishmentResult GetResult(ICharacter actor, ICrime crime)
	{
		foreach (var strategy in _strategyHierarchy)
		{
			if (strategy.Prog.Execute<bool?>(actor, crime) == true)
			{
				return strategy.Strategy.GetResult(actor, crime);
			}
		}

		return new PunishmentResult();
	}

	protected override void SaveSpecificType(XElement root)
	{
		root.Add(new XAttribute("type", "hierarchy"));
		foreach (var item in _strategyHierarchy)
		{
			root.Add(new XElement("Member", new XAttribute("prog", item.Prog.Id), item.Strategy.SaveResultXElement()));
		}
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Punishment Hierarchy".ColourName());
		sb.AppendLine("Applies the following punishments in order, based on prog criteria:");
		var i = 0;
		foreach (var item in _strategyHierarchy)
		{
			sb.AppendLine(
				$"\t{(++i).ToString("N0", actor)}) [{item.Prog.MXPClickableFunctionName()}] {item.Strategy.Describe(actor).Colour(Telnet.BoldRed)}");
		}

		BaseShowText(actor, sb);
		return sb.ToString();
	}
}