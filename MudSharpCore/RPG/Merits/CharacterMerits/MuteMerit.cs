using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MuteMerit : CharacterMeritBase, IMuteMerit
{
	protected MuteMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		LanguageOptions = (PermitLanguageOptions)int.Parse(root.Element("PermitLanguageOption")?.Value ?? "2");
	}

	protected MuteMerit(){}

	protected MuteMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Mute", "@ are|is mute")
	{
		LanguageOptions = PermitLanguageOptions.LanguageIsGasping;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("PermitLanguageOption", (int)LanguageOptions));
		return root;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Mute",
			(merit, gameworld) => new MuteMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Mute", (gameworld, name) => new MuteMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Mute", "Makes a person unable to speak", new MuteMerit().HelpText);
	}

	public PermitLanguageOptions LanguageOptions { get; private set; }

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Mute Language Option: {LanguageOptions.DescribeEnum().ColourValue()}");
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3option <option>#0 - changes the speech replacement option";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "option":
				return BuildingCommandOption(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private IEnumerable<PermitLanguageOptions> _availableOptions => [
		PermitLanguageOptions.LanguageIsGasping,
		PermitLanguageOptions.LanguageIsBuzzing,
		PermitLanguageOptions.LanguageIsChoking,
		PermitLanguageOptions.LanguageIsClicking,
		PermitLanguageOptions.LanguageIsError,
		PermitLanguageOptions.LanguageIsMuffling
	];

	private bool BuildingCommandOption(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What option should be set for the effects of this mute merit? The valid options are: {_availableOptions.Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out PermitLanguageOptions option) || !_availableOptions.Contains(option))
		{
			actor.OutputHandler.Send($"That is not a valid option. The valid options are: {_availableOptions.Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		LanguageOptions = option;
		Changed = true;
		actor.OutputHandler.Send($"When the mute character tries to speak, their speech will now use the {option.DescribeEnum().ColourValue()} option.");
		return true;
	}
}