using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.NPC.AI;

public class ScavengeAI : ArtificialIntelligenceBase
{
	public ScavengeAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	/// <summary>
	///     Prog returns true, parameters character, item
	/// </summary>
	public IFutureProg WillScavengeItemProg { get; set; }

	/// <summary>
	///     Prog returns void, parameters character, item
	/// </summary>
	public IFutureProg OnScavengeItemProg { get; set; }

	public string ScavengeDelayDiceExpression { get; set; }

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WillScavengeItemProg", WillScavengeItemProg?.Id ?? 0L),
			new XElement("ScavengeDelayDiceExpression", new XCData(ScavengeDelayDiceExpression)),
			new XElement("OnScavengeItemProg", OnScavengeItemProg?.Id ?? 0L)
		).ToString();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("Scavenge", (ai, gameworld) => new ScavengeAI(ai, gameworld));
	}

	private void LoadFromXml(XElement root)
	{
		WillScavengeItemProg =
			long.TryParse(root.Element("WillScavengeItemProg").Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(root.Element("WillScavengeItemProg").Value);

		if (WillScavengeItemProg == null)
		{
			throw new ApplicationException(
				$"AI {Id} pointed to a WillScavengeItem Prog that was not correct - {root.Element("WillScavengeItemProg").Value}.");
		}

		if (WillScavengeItemProg.ReturnType != FutureProgVariableTypes.Boolean)
		{
			throw new ApplicationException(
				$"AI {Id} WillScavengeItem prog returns {WillScavengeItemProg.ReturnType.Describe()} - expected Boolean");
		}

		if (
			!WillScavengeItemProg.MatchesParameters(new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			}))
		{
			throw new ApplicationException(
				$"AI {Id} WillScavengeItem prog does not accept the right parameters - should be character, item");
		}

		OnScavengeItemProg =
			long.TryParse(root.Element("OnScavengeItemProg").Value, out value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(root.Element("OnScavengeItemProg").Value);

		if (WillScavengeItemProg == null)
		{
			throw new ApplicationException(
				$"AI {Id} pointed to a OnScavengeItemProg Prog that was not correct - {root.Element("OnScavengeItemProg").Value}.");
		}

		if (
			!WillScavengeItemProg.MatchesParameters(new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			}))
		{
			throw new ApplicationException(
				$"AI {Id} OnScavengeItemProg prog does not accept the right parameters - should be character, item");
		}

		ScavengeDelayDiceExpression = root.Element("ScavengeDelayDiceExpression").Value;
		if (!Dice.IsDiceExpression(ScavengeDelayDiceExpression))
		{
			throw new ApplicationException($"AI {Id} had an illegal dice expression.");
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.CharacterEntersGame:
			case EventType.CharacterEnterCellFinish:
			case EventType.CharacterStopMovement:
			case EventType.CharacterStopMovementClosedDoor:
			case EventType.MinuteTick:
				var character = arguments[0];
				CreateScavengeEvent(character);
				return false;
			case EventType.CharacterBeginMovement:
				character = arguments[0];
				PurgeScavengeEvent(character);
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
				case EventType.CharacterEntersGame:
				case EventType.CharacterEnterCellFinish:
				case EventType.CharacterStopMovement:
				case EventType.CharacterStopMovementClosedDoor:
				case EventType.CharacterBeginMovement:
				case EventType.MinuteTick:
					return true;
			}
		}

		return false;
	}

	private void CreateScavengeEvent(ICharacter character)
	{
		if (character.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (
			character.Effects.Any(x => x.IsEffectType<WaitingForScavenge>()))
		{
			return;
		}

		character.AddEffect(
			new WaitingForScavenge(character, x => { CheckScavengeEvent(character); }),
			TimeSpan.FromSeconds(Dice.Roll(ScavengeDelayDiceExpression)));
	}

	private void PurgeScavengeEvent(ICharacter character)
	{
		character.RemoveAllEffects(x => x.IsEffectType<WaitingForScavenge>());
	}

	private void CheckScavengeEvent(ICharacter character)
	{
		if (character.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		var targetItem =
			character.Location.LayerGameItems(character.RoomLayer).FirstOrDefault(
				x => character.CanSee(x) && ((bool?)WillScavengeItemProg.Execute(character, x) ?? false));
		if (targetItem != null)
		{
			OnScavengeItemProg.Execute(character, targetItem);
		}

		CreateScavengeEvent(character);
	}
}