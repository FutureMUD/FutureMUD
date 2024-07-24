using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SecondWindMerit : CharacterMeritBase, ISecondWindMerit
{

	protected SecondWindMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		Emote = root.Element("Emote").Value;
		RecoveryMessage = root.Element("RecoveryMessage").Value;
		RecoveryDuration = TimeSpan.FromSeconds(double.Parse(root.Element("RecoveryDuration").Value));
	}

	protected SecondWindMerit(){}

	protected SecondWindMerit(IFuturemud gameworld, string name) : base(gameworld, name, "SecondWind", "@ have|has an additional second wind")
	{
		Emote = "$0 get|gets a burst of adrenaline as #0 %0|get|gets &0's second wind.";
		RecoveryMessage = "You feel as if you have recovered from the arrival of your second wind";
		RecoveryDuration = TimeSpan.FromSeconds(1230);
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("Emote", new XCData(Emote)));
		root.Add(new XElement("RecoveryMessage", new XCData(RecoveryMessage)));
		root.Add(new XElement("RecoveryDuration", RecoveryDuration.TotalSeconds));
		return root;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("SecondWind",
			(merit, gameworld) => new SecondWindMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("SecondWind", (gameworld, name) => new SecondWindMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("SecondWind", "Gives an additional second wind in combat", new SecondWindMerit().HelpText);
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Used Emote: {Emote.ColourCommand()}");
		sb.AppendLine($"Recovery Message: {RecoveryMessage.ColourCommand()}");
		sb.AppendLine($"Recovery Duration: {RecoveryDuration.DescribePreciseBrief().ColourValue()}");
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3emote <emote>#0 - sets the emote
	#3recovery <message>#0 - sets the recovery message
	#3duration <seconds>#0 - sets the recovery duration in seconds";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "emote":
				return BuildingCommandEmote(actor, command);
			case "recovery":
				return BuildingCommandRecovery(actor, command);
			case "duration":
				return BuildingCommandDuration(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How long in seconds should this second wind be locked out for after use?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		RecoveryDuration = TimeSpan.FromSeconds(value);
		Changed = true;
		actor.OutputHandler.Send($"This merit's second wind will be locked out for {RecoveryDuration.DescribePreciseBrief().ColourValue()} after use.");
		return true;
	}

	private bool BuildingCommandRecovery(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What recovery message do you want to set ");
			return false;
		}

		RecoveryMessage = command.SafeRemainingArgument.Fullstop().ProperSentences();
		actor.OutputHandler.Send($"You set the recovery message to {RecoveryMessage.ColourCommand()}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to set for this second wind? Use $0 for the person getting the second wind.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		Emote = command.SafeRemainingArgument;
		actor.OutputHandler.Send($"You set the emote to {Emote.ColourCommand()}");
		Changed = true;
		return true;
	}

	public string Emote { get; set; }

	public TimeSpan RecoveryDuration { get; set; }

	public string RecoveryMessage { get; set; }
}