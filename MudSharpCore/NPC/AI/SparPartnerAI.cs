using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.FutureProg.Statements.Manipulation;
using System.Text;

namespace MudSharp.NPC.AI;

public class SparPartnerAI : ArtificialIntelligenceBase
{
	public SparPartnerAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	private SparPartnerAI()
	{
	}

	private SparPartnerAI(IFuturemud gameworld, string name) : base(gameworld, name, "Spar Partner")
	{
		WillSparProg = Gameworld.AlwaysTrueProg;
		EngageDelayDiceExpression = "1000+1d2000";
		DatabaseInitialise();
	}

	public IFutureProg WillSparProg { get; set; }
	public string EngageDelayDiceExpression { get; set; }
	public string EngageEmote { get; set; }
	public string RefuseEmote { get; set; }

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Will Spar Prog: {WillSparProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Engage Delay: {EngageDelayDiceExpression.ColourValue()} seconds");
		sb.AppendLine($"Engage Emote: {EngageEmote?.ColourCommand() ?? ""}");
		sb.AppendLine($"Refuse Emote: {RefuseEmote?.ColourCommand() ?? ""}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => @"	#3will <prog>#0 - sets the prog that controls if the NPC will spar
	#3delay <expression>#0 - a dice expression in milliseconds for the delay before engaging
	#3engage <emote>#0 - sets the emote when engaging ($0 = NPC, $1 = spar proponent)
	#3engage clear#0 - clears the engage emote
	#3refuse <emote>#0 - sets the emote when refusing to engage ($0 = NPC, $1 = spar proponent)
	#3refuse clear#0 - clears the refuse emote";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "delay":
				return BuildingCommandDelay(actor, command);
			case "engage":
				return BuildingCommandEngage(actor, command);
			case "refuse":
				return BuildingCommandRefuse(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandRefuse(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify an emote or use #3clear#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
		{
			RefuseEmote = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer do any emote when refusing to accept a spar invitation.");
			return true;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		RefuseEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This NPC will do the emote {command.SafeRemainingArgument.ColourCommand()} when refusing to accept a spar.");
		return true;
	}

	private bool BuildingCommandEngage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify an emote or use #3clear#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
		{
			EngageEmote = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer do any emote when accepting a spar invitation.");
			return true;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EngageEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This NPC will do the emote {command.SafeRemainingArgument.ColourCommand()} when accepting a spar.");
		return true;
	}

	private bool BuildingCommandDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid dice expression for milliseconds before engaging.");
			return false;
		}

		if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		EngageDelayDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now wait {EngageDelayDiceExpression.ColourValue()} milliseconds before engaging after accepting a spar.");
		return true;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WillSparProg", WillSparProg?.Id ?? 0L),
			new XElement("EngageDelayDiceExpression", new XCData(EngageDelayDiceExpression)),
			new XElement("EngageEmote", new XCData(EngageEmote)),
			new XElement("RefuseEmote", new XCData(RefuseEmote))
		).ToString();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("Spar Partner", (ai, gameworld) => new SparPartnerAI(ai, gameworld));
		RegisterAIBuilderInformation("sparpartner", (game,name) => new SparPartnerAI(game, name), new SparPartnerAI().HelpText);
	}

	private void LoadFromXml(XElement root)
	{
		WillSparProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillSparProg").Value));
		EngageDelayDiceExpression = root.Element("EngageDelayDiceExpression").Value;
		EngageEmote = root.Element("EngageEmote")?.Value;
		RefuseEmote = root.Element("RefuseEmote")?.Value;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = (ICharacter)arguments[0];
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.SparInvitation:
				return CheckForSpar((ICharacter)arguments[0], (ICharacter)arguments[1]);
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.SparInvitation:
					return true;
			}
		}

		return false;
	}

	public bool CanAccept(ICharacter npc, ICharacter invitee)
	{
		if (!npc.Combat?.Friendly ?? false)
		{
			return false;
		}

		if (npc.Combat?.Combatants.Contains(invitee) ?? false)
		{
			return false;
		}

		if (!CharacterState.Able.HasFlag(npc.State))
		{
			return false;
		}

		if (npc.Effects.Any(x => x.IsBlockingEffect("combat-engage")))
		{
			return false;
		}

		if (!(bool?)WillSparProg.Execute(npc, invitee) ?? false)
		{
			if (!string.IsNullOrWhiteSpace(RefuseEmote))
			{
				npc.OutputHandler.Handle(new EmoteOutput(new Emote(RefuseEmote, npc, npc, invitee)));
			}

			return false;
		}

		return true;
	}

	public bool CheckForSpar(ICharacter npc, ICharacter invitee)
	{
		if (!CanAccept(npc, invitee))
		{
			return false;
		}

		npc.AddEffect(new BlockingDelayedAction(npc, perceiver =>
			{
				if (!CanAccept(npc, invitee))
				{
					return;
				}

				if (!string.IsNullOrWhiteSpace(EngageEmote))
				{
					npc.OutputHandler.Handle(new EmoteOutput(new Emote(EngageEmote, npc, npc, invitee)));
				}

				npc.EffectsOfType<IProposalEffect>()
				   .FirstOrDefault(x => x.Proposal is SparInvitationProposal)?.Proposal.Accept();
			}, "preparing to accept a spar invitation", new[] { "general", "combat-engage" }, null),
			TimeSpan.FromMilliseconds(Dice.Roll(EngageDelayDiceExpression)));

		return true;
	}
}