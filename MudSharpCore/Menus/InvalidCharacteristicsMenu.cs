using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.Menus;
#nullable enable
public class InvalidCharacteristicsMenu : CharacterLoginMenu
{
	public static bool IsRequired(ICharacter character)
	{
		return character
		       .RawCharacteristicValues
		       .Any(x => x.OngoingValidityProg?.Execute<bool?>(character) == false);
	}

	public List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> InvalidValues { get; }
	private bool _firstTime;

	public InvalidCharacteristicsMenu(ICharacter character, bool firstTime) : base(character)
	{
		_firstTime = firstTime;
		InvalidValues = character
		                .RawCharacteristics
		                .Where(x => x.Value.OngoingValidityProg?.Execute<bool?>(character) == false)
		                .ToList();
	}

	public override string Prompt => "> \n";

	public override bool ExecuteCommand(string command)
	{
		if (string.IsNullOrWhiteSpace(command))
		{
			OutputHandler.Send(ShowMenuText());
			return true;
		}

		if (command.EqualToAny("quit", "close", "stop", "back"))
		{
			_nextContext = new LoggedInMenu(Character.Account, Gameworld);
			return true;
		}

		var current = InvalidValues.First();
		var choices = Gameworld.CharacteristicValues.Where(x =>
			current.Definition.IsValue(x) && x.OngoingValidityProg?.Execute<bool?>(Character) != false).ToList();
		if (int.TryParse(command, out var value))
		{
			if (value < 1 || value > choices.Count)
			{
				OutputHandler.Send(
					$"You must select a value between {1.ToString("N0", Account).ColourValue()} and {choices.Count.ToString("N0", Account).ColourValue()}.");
				return true;
			}

			var choice = choices.ElementAt(value - 1);
			Character.SetCharacteristic(current.Definition, choice);
			InvalidValues.RemoveAt(0);
			if (!InvalidValues.Any())
			{
				DoLogin(_firstTime);
			}

			return true;
		}

		var textChoice = choices.FirstOrDefault(x => x.GetValue.EqualTo(command));
		if (textChoice != null)
		{
			Character.SetCharacteristic(current.Definition, textChoice);
			InvalidValues.RemoveAt(0);
			if (!InvalidValues.Any())
			{
				OutputHandler.Send("\n");
				DoLogin(_firstTime);
			}
			else
			{
				OutputHandler.Send(ShowMenuText());
			}

			return true;
		}

		OutputHandler.Send("That is not a valid selection.");
		return true;
	}

	public string ShowMenuText()
	{
		var current = InvalidValues.First();
		var sb = new StringBuilder();
		sb.AppendLine();
		sb.AppendLine($"Invalid Value for {current.Definition.Name}".Colour(Telnet.Cyan));
		sb.AppendLine();
		sb.AppendLine(
			$"Your character {Character.PersonalName.GetName(NameStyle.FullName).ColourName()} has a value of {current.Value.Name.ColourValue()} for the {current.Definition.Name.ColourValue()} characteristic, which is no longer a valid selection for {Character.Gender.Objective()}. You must make a new selection from the values below:"
				.Wrap(Account.InnerLineFormatLength));
		sb.AppendLine();
		var choices = Gameworld.CharacteristicValues.Where(x =>
			current.Definition.IsValue(x) && x.OngoingValidityProg?.Execute<bool?>(Character) != false).ToList();
		var index = 1;
		sb.AppendLine(choices.Select(x => $"{index++.ToString("N0", Character)}: {x.GetValue}").ArrangeStringsOntoLines(
			(uint)Account.LineFormatLength / 30,
			(uint)Account.LineFormatLength));
		sb.AppendLine();
		sb.AppendLine($"Please make your choice below:");
		return sb.ToString();
	}

	public override void AssumeControl(IController controller)
	{
		base.AssumeControl(controller);
		OutputHandler.Send(ShowMenuText());
	}

	public override void SilentAssumeControl(IController controller)
	{
		base.SilentAssumeControl(controller);
		OutputHandler.Send(ShowMenuText());
	}
}