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

namespace MudSharp.NPC.AI;

public class SelfCareAI : ArtificialIntelligenceBase
{
	public IEnumerable<string> BleedingEmotes;

	private SelfCareAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	public string BleedingEmote => BleedingEmotes.GetRandomElement();
	public string BleedingEmoteDelayDiceExpression { get; set; }
	public string BindingDelayDiceExpression { get; set; }

	public static void RegisterLoader()
	{
		RegisterAIType("SelfCare", (ai, gameworld) => new SelfCareAI(ai, gameworld));
	}

	private void LoadFromXml(XElement root)
	{
		BindingDelayDiceExpression = root.Element("BindingDelayDiceExpression").Value;
		BleedingEmoteDelayDiceExpression = root.Element("BleedingEmoteDelayDiceExpression").Value;
		BleedingEmotes = root.Elements("BleedingEmote").Select(x => x.Value).ToList();
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
			if (BleedingEmotes.Any())
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