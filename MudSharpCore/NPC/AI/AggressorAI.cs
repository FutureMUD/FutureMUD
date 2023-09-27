using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.NPC.AI;

public class AggressorAI : ArtificialIntelligenceBase
{
	public AggressorAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	public IFutureProg WillAttackProg { get; set; }
	public string EngageDelayDiceExpression { get; set; }
	public string EngageEmote { get; set; }
	public override bool CountsAsAggressive => true;

	public static void RegisterLoader()
	{
		RegisterAIType("Aggressor", (ai, gameworld) => new AggressorAI(ai, gameworld));
	}

	private void LoadFromXml(XElement root)
	{
		WillAttackProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillAttackProg").Value));
		EngageDelayDiceExpression = root.Element("EngageDelayDiceExpression").Value;
		EngageEmote = root.Element("EngageEmote")?.Value;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WillAttackProg", WillAttackProg.Id),
			new XElement("EngageDelayDiceExpression", new XCData(EngageDelayDiceExpression)),
			new XElement("EngageEmote", new XCData(EngageEmote))
		).ToString();
	}

	private bool CheckAllTargetsForAttack(ICharacter ch)
	{
		if (ch.State.HasFlag(CharacterState.Dead) || ch.State.HasFlag(CharacterState.Stasis))
		{
			return false;
		}

		if (!CharacterState.Able.HasFlag(ch.State))
		{
			return false;
		}

		if (ch.Combat != null && ch.CombatTarget is ICharacter ctch && WillAttack(ch, ctch))
		{
			return false;
		}

		if (ch.Effects.Any(x => x.IsBlockingEffect("combat-engage") || x.IsBlockingEffect("general")))
		{
			return false;
		}

		foreach (var tch in ch.Location.LayerCharacters(ch.RoomLayer).Except(ch).Shuffle())
		{
			if (CheckForAttack(ch, tch))
			{
				return true;
			}
		}

		var range = (uint)ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
		                    .Where(x => x.IsReadied || x.CanReady(ch))
		                    .Select(x => (int)x.WeaponType.DefaultRangeInRooms)
		                    .DefaultIfEmpty(0).Max();
		// TODO - natural attacks
		if (range > 0)
			//TODO: With this, AI can find you through doorways it doesn't have direct LOS into, which doesn't seem fair
			//Worth revisiting at some point.
		{
			foreach (var tch in ch.Location.CellsInVicinity(range, true, true).Except(ch.Location)
			                      .SelectMany(x => x.Characters).ToList())
			{
				if (CheckForAttack(ch, tch))
				{
					return true;
				}
			}
		}

		return false;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.TenSecondTick:
				var ch = (ICharacter)arguments[0];
				return CheckAllTargetsForAttack(ch);
			case EventType.CharacterEnterCellWitness:
				return CheckForAttack((ICharacter)arguments[3], (ICharacter)arguments[0]);
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterEnterCellWitness:
				case EventType.TenSecondTick:
					return true;
			}
		}

		return false;
	}

	private void EngageTarget(ICharacter ch, IPerceiver tp)
	{
		if (ch.State.HasFlag(CharacterState.Dead) || ch.Corpse != null)
		{
			return;
		}

		if (tp is ICharacter tch && tch.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (!ch.CanEngage(tp))
		{
			return;
		}

		if (!string.IsNullOrWhiteSpace(EngageEmote))
		{
			ch.OutputHandler.Handle(new EmoteOutput(new Emote(EngageEmote, ch, ch, tp)));
		}

		ch.Engage(tp, ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
		                .Where(x => x.IsReadied || x.CanReady(ch)).Select(x => (int)x.WeaponType.DefaultRangeInRooms)
		                .DefaultIfEmpty(-1).Max() > 0);
	}

	public bool CheckForAttack(ICharacter aggressor, ICharacter target)
	{
		if (!WillAttack(aggressor, target))
		{
			return false;
		}

		aggressor.AddEffect(
			new BlockingDelayedAction(aggressor, perceiver => { EngageTarget(aggressor, target); },
				"preparing to engage in combat", new[] { "general", "combat-engage", "movement" }, null),
			TimeSpan.FromMilliseconds(Dice.Roll(EngageDelayDiceExpression)));

		return true;
	}

	private bool WillAttack(ICharacter aggressor, ICharacter target)
	{
		if (aggressor.State.HasFlag(CharacterState.Dead) || aggressor.State.HasFlag(CharacterState.Stasis))
		{
			return false;
		}

		if (aggressor.Combat != null)
		{
			return false;
		}

		if (aggressor == target)
		{
			return false;
		}

		if (!CharacterState.Able.HasFlag(aggressor.State))
		{
			return false;
		}

		if (!aggressor.CanSee(target))
		{
			return false;
		}

		if (!((bool?)WillAttackProg.Execute(aggressor, target) ?? false))
		{
			return false;
		}

		if (aggressor.CombatTarget != target && !aggressor.CanEngage(target))
		{
			return false;
		}

		if (aggressor.Effects.Any(x => x.IsBlockingEffect("combat-engage")) ||
		    aggressor.Effects.Any(x => x.IsBlockingEffect("general")))
		{
			return false;
		}

		return true;
	}
}