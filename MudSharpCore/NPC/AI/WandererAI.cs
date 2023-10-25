using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Movement;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.NPC.AI;

public class WandererAI : ArtificialIntelligenceBase
{
	protected string EmoteText;
	protected bool OpenDoors;
	protected IBodyPrototype TargetBodyPrototype;
	protected IMoveSpeed TargetMoveSpeed;
	protected string WanderTimeDiceExpression;
	protected IFutureProg WillWanderIntoCellProg;

	public WandererAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	public static void RegisterLoader()
	{
		RegisterAIType("Wanderer", (ai, gameworld) => new WandererAI(ai, gameworld));
	}

	private void LoadFromXml(XElement root)
	{
		WillWanderIntoCellProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("FutureProg")?.Value ?? "0"));
		WanderTimeDiceExpression = root.Element("WanderTimeDiceExpression")?.Value ?? "1d40+100";
		TargetBodyPrototype = Gameworld.BodyPrototypes.Get(long.Parse(root.Element("TargetBody")?.Value ?? "0"));
		TargetMoveSpeed = TargetBodyPrototype?.Speeds.Get(long.Parse(root.Element("TargetSpeed")?.Value ?? "0"));
		EmoteText = root.Element("EmoteText")?.Value;
		var element = root.Element("OpenDoors");
		OpenDoors = element != null && bool.Parse(element.Value);
	}

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("FutureProg", WillWanderIntoCellProg?.Id ?? 0),
            new XElement("WanderTimeDiceExpression", new XCData(WanderTimeDiceExpression)),
            new XElement("TargetBody", TargetBodyPrototype?.Id ?? 0),
            new XElement("TargetSpeed", TargetMoveSpeed?.Id ?? 0),
            new XElement("OpenDoors", OpenDoors),
            new XElement("EmoteText", new XCData(EmoteText))
        ).ToString();
    }

    private void CreateEvaluateAffect(ICharacter character, int seconds = 10)
	{
		if (character.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (character.Effects.Any(x => x.IsEffectType<WandererWaiting>()))
		{
			return;
		}

		if (character.Movement != null || character.Combat != null)
		{
			return;
		}

		character.AddEffect(new WandererWaiting(character, actor => EvaluateWander(character)),
			TimeSpan.FromSeconds(seconds));
	}

	private void CancelEvaluateAffect(ICharacter character)
	{
		character.RemoveAllEffects(
			x => x.IsEffectType<WandererWaiting>());
	}

	private void EvaluateWander(ICharacter character)
	{
		if (character.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (character.Movement != null)
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (character.EffectsOfType<FollowingPath>().Any())
		{
			// Don't wander while following a path from other AI
			CreateEvaluateAffect(character);
			return;
		}

		if (!CharacterState.Able.HasFlag(character.State) || character.Combat != null ||
		    character.Effects.Any(x => x.IsBlockingEffect("movement")))
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (character.Effects.Any(x => x.IsEffectType<WandererWaiting>()))
		{
			return;
		}

		IEnumerable<ICellExit> options =
			character.Location.ExitsFor(character)
			         .Where(
				         x => (bool?)WillWanderIntoCellProg?.Execute(character, x.Destination, character.Location) !=
				              false)
			         .Where(
				         x =>
					         x.Exit.Door?.IsOpen != false || (x.Exit.Door.CanOpen(character.Body) && OpenDoors))
			         .ToList();

		if (!options.Any())
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (character.PositionState.MoveRestrictions != MovementAbility.Free)
		{
			var upright = character.MostUprightMobilePosition();
			if (upright == null)
			{
				CreateEvaluateAffect(character);
				return;
			}

			character.MovePosition(upright, null, null);
		}

		var choice = options.GetRandomElement();
		if (!character.Move(choice, !string.IsNullOrEmpty(EmoteText) ? new Emote(EmoteText, character) : null))
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (!(choice.Exit?.Door?.IsOpen ?? true) && OpenDoors)
		{
			character.Body.Open(choice.Exit.Door, null, null);
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.CharacterEnterCellFinish:
			case EventType.CharacterStopMovement:
			case EventType.CharacterStopMovementClosedDoor:
			case EventType.CharacterCannotMove:
				CreateEvaluateAffect(arguments[0], Dice.Roll(WanderTimeDiceExpression));
				return false;

			case EventType.EngagedInCombat:
				CancelEvaluateAffect(arguments[1]);
				return false;
			case EventType.EngageInCombat:
				CancelEvaluateAffect(arguments[0]);
				return false;

			case EventType.CharacterEntersGame:
			case EventType.LeaveCombat:
				var character = arguments[0];
				if (TargetMoveSpeed != null)
				{
					character.CurrentSpeeds[PositionStanding.Instance] = TargetMoveSpeed;
				}

				CreateEvaluateAffect(arguments[0], Dice.Roll(WanderTimeDiceExpression));
				return false;

			case EventType.FiveSecondTick:
				CreateEvaluateAffect(arguments[0]);
				return false;
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterEnterCellFinish:
				case EventType.CharacterStopMovement:
				case EventType.CharacterStopMovementClosedDoor:
				case EventType.CharacterCannotMove:
				case EventType.EngagedInCombat:
				case EventType.EngageInCombat:
				case EventType.CharacterEntersGame:
				case EventType.LeaveCombat:
				case EventType.FiveSecondTick:
					return true;
			}
		}

		return false;
	}
}