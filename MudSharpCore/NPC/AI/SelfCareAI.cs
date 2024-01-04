using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.FutureProg.Statements.Manipulation;
using System.Text;

namespace MudSharp.NPC.AI;

public class SelfCareAI : ArtificialIntelligenceBase
{
	private readonly List<string> _bleedingEmotes = new();
	public IEnumerable<string> BleedingEmotes => _bleedingEmotes;

	private SelfCareAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	private SelfCareAI()
	{

	}

	private SelfCareAI(IFuturemud gameworld, string name) : base(gameworld, name, "SelfCare")
	{
		BindingDelayDiceExpression = "250+1d2750";
		BleedingEmoteDelayDiceExpression = "3000+1d2000";
		DatabaseInitialise();
	}

	public string BleedingEmote => BleedingEmotes.GetRandomElement();
	public string BleedingEmoteDelayDiceExpression { get; set; }
	public string BindingDelayDiceExpression { get; set; }

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Bleeding Emote Delay: {BleedingEmoteDelayDiceExpression.ColourValue()} seconds");
		sb.AppendLine($"Bind Self Delay: {BindingDelayDiceExpression.ColourValue()} seconds");
		sb.AppendLine($"Emotes:");
		sb.AppendLine();
		if (_bleedingEmotes.Count == 0)
		{
			sb.AppendLine($"\tNone");
		}
		else
		{
			foreach (var emote in _bleedingEmotes)
			{
				sb.AppendLine($"\t{emote.ColourCommand()}");
			}
		}
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => @"	#3binddelay <expression>#0 - a dice expression in milliseconds for delay binding self
	#3bleedingdelay <expression>#0 - a dice expression in milliseconds for delay emoting about bleeding
	#3addemote <emote>#0 - adds a bleeding emote to the list. $0 is the NPC.
	#3rememote <##>#0 - removes an emote from the list";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "binddelay":
				return BuildingCommandBindDelay(actor, command);
			case "bleedingdelay":
				return BuildingCommandBleedingEmoteDelay(actor, command);
			case "addemote":
				return BuildingCommandBleedingAddEmote(actor, command);
			case "deleteemote":
			case "delemote":
			case "rememote":
			case "removeemote":
				return BuildingCommandBleedingRemoveEmote(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandBleedingRemoveEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which emote would you like to remove?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var index) || index < 1 || index > _bleedingEmotes.Count)
		{
			actor.OutputHandler.Send($"You must enter a valid number between {1.ToString("N0", actor).ColourValue()} and {_bleedingEmotes.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"You delete the emote {_bleedingEmotes[index-1].ColourCommand()}.");
		_bleedingEmotes.RemoveAt(index-1);
		return true;
	}

	private bool BuildingCommandBleedingAddEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to add?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		_bleedingEmotes.Add(command.SafeRemainingArgument);
		Changed = true;
		actor.OutputHandler.Send($"You add the emote {command.SafeRemainingArgument.ColourCommand()} to the list of emotes.");
		return true;
	}

	private bool BuildingCommandBleedingEmoteDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid dice expression for milliseconds before emoting about bleeding.");
			return false;
		}

		if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		BleedingEmoteDelayDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now wait {BleedingEmoteDelayDiceExpression.ColourValue()} milliseconds before emoting about bleeding.");
		return true;
	}

	private bool BuildingCommandBindDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid dice expression for milliseconds before entering the bind command.");
			return false;
		}

		if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		BindingDelayDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now wait {BindingDelayDiceExpression.ColourValue()} milliseconds before binding itself.");
		return true;
	}

	public static void RegisterLoader()
	{
		RegisterAIType("SelfCare", (ai, gameworld) => new SelfCareAI(ai, gameworld));
		RegisterAIBuilderInformation("selfcare", (gameworld, name) => new SelfCareAI(gameworld, name), new SelfCareAI().HelpText);
	}

	private void LoadFromXml(XElement root)
	{
		BindingDelayDiceExpression = root.Element("BindingDelayDiceExpression").Value;
		BleedingEmoteDelayDiceExpression = root.Element("BleedingEmoteDelayDiceExpression").Value;
		_bleedingEmotes.AddRange(root.Elements("BleedingEmote").Select(x => x.Value));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("BindingDelayDiceExpression", new XCData(BindingDelayDiceExpression)),
			new XElement("BleedingEmoteDelayDiceExpression", new XCData(BleedingEmoteDelayDiceExpression)),
			from emote in BleedingEmotes select new XElement("BleedingEmote", new XCData(emote))
		).ToString();
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.LeaveCombat:
				return HandleLeaveCombat((ICharacter)arguments[0]);
			case EventType.NoLongerTargettedInCombat:
				return HandleNoLongerTargetted((ICharacter)arguments[1]);
			case EventType.NoLongerEngagedInMelee:
				return HandleNoLongerEngagedInMelee((ICharacter)arguments[0]);
			case EventType.BleedTick:
				return HandleBleedTick((ICharacter)arguments[0], (double)arguments[1]);
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.LeaveCombat:
				case EventType.NoLongerTargettedInCombat:
				case EventType.NoLongerEngagedInMelee:
				case EventType.BleedTick:
					return true;
			}
		}

		return false;
	}

	private bool HandleSelfCare(ICharacter character)
	{
		if (character.EffectsOfType<DelayedAction>().Any(x => x.ActionDescription == "self care tick"))
		{
			return false;
		}

		if (
			character.VisibleWounds(character, WoundExaminationType.Self)
			         .Any(x => x.PeekBleed(1.0, character.LongtermExertion) > 0.0))
		{
			// TODO - make sure the emote doesn't fire every bleed tick
			if (BleedingEmotes.Any() &&
			    character.EffectsOfType<DelayedAction>().All(x => x.ActionDescription != "self care tick") &&
				Dice.Roll("1d6") == 1
				)
			{
				character.AddEffect(
					new DelayedAction(character,
						x => { character.OutputHandler.Handle(new EmoteOutput(new Emote(BleedingEmote, character))); },
						"self care tick"), TimeSpan.FromMilliseconds(Dice.Roll(BleedingEmoteDelayDiceExpression)));
			}

			if (character.IsEngagedInMelee)
			{
				return false;
			}

			if (character.Effects.Any(x => x.IsBlockingEffect("general")))
			{
				return false;
			}

			if (character.Movement != null)
			{
				return false;
			}

			if (!CharacterState.Able.HasFlag(character.State))
			{
				return false;
			}

			character.AddEffect(
				new DelayedAction(character, x => { character.ExecuteCommand("bind self"); }, "self care tick"),
				TimeSpan.FromMilliseconds(Dice.Roll(BindingDelayDiceExpression)));
			return true;
		}

		return false;
	}

	private bool HandleBleedTick(ICharacter character, double amount)
	{
		return HandleSelfCare(character);
	}

	private bool HandleNoLongerEngagedInMelee(ICharacter character)
	{
		return HandleSelfCare(character);
	}

	private bool HandleNoLongerTargetted(ICharacter character)
	{
		return HandleSelfCare(character);
	}

	private bool HandleLeaveCombat(ICharacter character)
	{
		return HandleSelfCare(character);
	}
}