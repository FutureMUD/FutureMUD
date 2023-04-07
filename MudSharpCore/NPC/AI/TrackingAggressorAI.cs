using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.NPC.AI.Strategies;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.NPC.AI;

public class TrackingAggressorAI : PathingAIWithProgTargetsBase
{
	public IFutureProg WillAttackProg { get; set; }
	public string EngageDelayDiceExpression { get; set; }
	public string EngageEmote { get; set; }

	public uint MaximumRange { get; set; }
	public override bool CountsAsAggressive => true;

	protected TrackingAggressorAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		WillAttackProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillAttackProg").Value));
		EngageDelayDiceExpression = root.Element("EngageDelayDiceExpression").Value;
		EngageEmote = root.Element("EngageEmote")?.Value;
		MaximumRange = uint.Parse(root.Element("MaximumRange").Value);
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

		foreach (var tch in ch.Location.Characters.Except(ch).Shuffle())
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
				return CheckForAttack((ICharacter)arguments[0], (ICharacter)arguments[3]);
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.TenSecondTick:
				case EventType.CharacterEnterCellFinishWitness:
					return true;
			}
		}

		return base.HandlesEvent(types);
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

	private Func<IPerceivable, bool> GetTargetFunction(ICharacter ch)
	{
		return x =>
		{
			if (x is not ICharacter xCh)
			{
				return false;
			}

			if (!(bool?)WillAttackProg.Execute(ch, x) ?? false)
			{
				return false;
			}

			if (!ch.CanSee(x))
			{
				return false;
			}

			return true;
		};
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

	public static void RegisterLoader()
	{
		RegisterAIType("TrackingAggressor", (ai, gameworld) => new TrackingAggressorAI(ai, gameworld));
	}

	protected override IEnumerable<ICellExit> GetPath(ICharacter ch)
	{
		var target = ch.AcquireTargetAndPath(GetTargetFunction(ch), MaximumRange, GetSuitabilityFunction(ch));
		if (target.Item1 == null || !target.Item2.Any())
		{
			return Enumerable.Empty<ICellExit>();
		}

		return target.Item2;
	}
}