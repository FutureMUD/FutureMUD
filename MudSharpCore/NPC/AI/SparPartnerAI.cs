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

namespace MudSharp.NPC.AI;

public class SparPartnerAI : ArtificialIntelligenceBase
{
	public SparPartnerAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	public IFutureProg WillSparProg { get; set; }
	public string EngageDelayDiceExpression { get; set; }
	public string EngageEmote { get; set; }
	public string RefuseEmote { get; set; }

	public static void RegisterLoader()
	{
		RegisterAIType("Spar Partner", (ai, gameworld) => new SparPartnerAI(ai, gameworld));
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