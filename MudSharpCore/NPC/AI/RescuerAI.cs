using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class RescuerAI : ArtificialIntelligenceBase
{
	protected RescuerAI(Models.ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("IsFriendProg", IsFriendProg?.Id ?? 0L)
		).ToString();
	}

	public IFutureProg IsFriendProg { get; set; }

	public static void RegisterLoader()
	{
		RegisterAIType("Rescuer", (ai, gameworld) => new RescuerAI(ai, gameworld));
	}

	private void LoadFromXml(XElement root)
	{
		IsFriendProg =
			long.TryParse(
				root.Element("IsFriendProg")?.Value ??
				throw new ApplicationException($"The RescuerAI with ID {Id} did not have an IsFriendProg attribute"),
				out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(root.Element("IsFriendProg").Value);
		if (IsFriendProg == null)
		{
			throw new ApplicationException($"The RescuerAI with ID {Id} specified an invalid IsFriendProg.");
		}

		if (!IsFriendProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			throw new ApplicationException(
				$"The RescuerAI with ID {Id} specified an IsFriendProg that did not return boolean.");
		}

		if (!IsFriendProg.MatchesParameters(new[] { FutureProgVariableTypes.Character }) &&
			!IsFriendProg.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character }))
		{
			throw new ApplicationException(
				$"The RescuerAI with ID {Id} specified an IsFriendProg that did not accept the right parameters.");
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.TenSecondTick:
				return HandleTenSecondTick((ICharacter)arguments[0]);
		}

		return false;
	}

	private bool HandleTenSecondTick(ICharacter character)
	{
		if (!IsGenerallyAble(character))
		{
			return false;
		}

		if (character.AffectedBy<IRescueEffect>())
		{
			return false;
		}

		ICharacter friendlyCombatant;
		if (IsFriendProg.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character }))
		{
			friendlyCombatant = character.Location
										 .LayerCharacters(character.RoomLayer)
										 .Where(x => x != character && x.Combat != null &&
													 IsFriendProg.Execute<bool?>(character, x) == true)
										 .GetWeightedRandom(x => x.MeleeRange ? 100.0 : 1.0);
		}
		else
		{
			friendlyCombatant = character.Location
										 .LayerCharacters(character.RoomLayer)
										 .Where(x => x != character && x.Combat != null &&
													 IsFriendProg.Execute<bool?>(x) == true)
										 .GetWeightedRandom(x => x.MeleeRange ? 100.0 : 1.0);
		}

		if (friendlyCombatant == null)
		{
			return false;
		}

		if (character.Combat != friendlyCombatant.Combat)
		{
			var opponent = friendlyCombatant.Combat.Combatants
			                                .Where(x =>
				                                x.CombatTarget == friendlyCombatant &&
				                                x.ColocatedWith(character) &&
				                                character.CanEngage(x)
			                                )
			                                .GetRandomElement();
			character.Engage(opponent);
		}

		character.AddEffect(new Rescue(character, friendlyCombatant));
		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.TenSecondTick:
					return true;
			}
		}

		return false;
	}
}