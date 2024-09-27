using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;
using static Mysqlx.Error.Types;

namespace MudSharp.RPG.Law.PunishmentStrategies;
public class PunishmentStrategyMultiple : PunishmentStrategyBase
{
	private readonly List<IPunishmentStrategy> _strategies = new();

	public PunishmentStrategyMultiple(IFuturemud gameworld, XElement root, ILegalAuthority authority) : base(gameworld,
		root)
	{
		foreach (var item in root.Elements("Member"))
		{
			_strategies.Add(PunishmentStrategyFactory.LoadStrategy(gameworld, item.InnerXML(), authority));
		}
	}

	public PunishmentStrategyMultiple(IFuturemud gameworld) : base(gameworld)
	{
	}

	public override string TypeSpecificHelpText => @"
	add <type> - adds a new punishment to the list
	remove <#> - removes a punishment from the list
	<#> ... - edits the properties of the punishment in the list";

	public override bool BuildingCommand(ICharacter actor, ILegalAuthority authority, StringStack command)
	{
		switch (command.PopForSwitch())
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
			default:
				if (int.TryParse(command.Last, out var index) && index > 0 && index < _strategies.Count)
				{
					return _strategies[index - 1].BuildingCommand(actor, authority, command);
				}

				return base.BuildingCommand(actor, authority, command.GetUndo());
		}
	}

	private bool BuildingCommandDelete(ICharacter actor, StringStack command)
	{
		if (!_strategies.Any())
		{
			actor.OutputHandler.Send("There aren't any punishments in the list for you to delete.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the index of the punishment that you wish to delete?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0 || value > _strategies.Count)
		{
			actor.OutputHandler.Send($"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_strategies.Count.ToString("N0", actor).ColourValue()}");
			return false;
		}

		var punishment = _strategies[value - 1];
		_strategies.RemoveAt(value - 1);
		actor.OutputHandler.Send($"You delete the punishment {punishment.Describe(actor)}.");
		return true;
	}

	private bool BuildingCommandCreate(ICharacter actor, ILegalAuthority authority, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What type of punishment do you want to add to the list?");
			return false;
		}

		var strategy = PunishmentStrategyFactory.GetStrategyFromBuilderInput(Gameworld, authority, command.PopSpeech());
		if (strategy is null)
		{
			actor.OutputHandler.Send($"That is not a valid strategy type. Valid types are {PunishmentStrategyFactory.ValidTypes.ListToColouredString()}.");
			return false;
		}

		_strategies.Add(strategy);
		actor.OutputHandler.Send($"You add the punishment {strategy.Describe(actor)} to the list of punishments.");
		authority.Changed = true;
		return true;
	}

	public override string Describe(IPerceiver voyeur)
	{
		if (_strategies.Count == 0)
		{
			return "no punishment";
		}
		return $"{_strategies.Select(x => x.Describe(voyeur)).ListToString()}";
	}

	public override PunishmentResult GetResult(ICharacter actor, ICrime crime, double severity = 0)
	{
		var result = new PunishmentResult();
		foreach (var strategy in _strategies)
		{
			result += strategy.GetResult(actor, crime, severity);
		}

		return result;
	}

	/// <inheritdoc />
	public override PunishmentOptions GetOptions(ICharacter actor, ICrime crime)
	{
		var result = new PunishmentOptions();
		foreach (var strategy in _strategies)
		{
			result += strategy.GetOptions(actor, crime);
		}

		return result;
	}

	protected override void SaveSpecificType(XElement root)
	{
		root.Add(new XAttribute("type", "multi"));
		foreach (var item in _strategies)
		{
			root.Add(new XElement("Member", item.SaveResultXElement()));
		}
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Multiple Punishments".ColourName());
		sb.AppendLine("Applies all of the following punishments:");
		var i = 0;
		foreach (var item in _strategies)
		{
			sb.AppendLine($"\t{(++i).ToString("N0", actor)}) {item.Describe(actor).Colour(Telnet.BoldRed)}");
		}

		BaseShowText(actor, sb);
		return sb.ToString();
	}
}
