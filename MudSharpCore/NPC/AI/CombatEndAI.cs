using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.NPC.AI;

public class CombatEndAI : ArtificialIntelligenceBase
{
	public IFutureProg WillAcceptTruce { get; set; }
	public IFutureProg WillAcceptTargetIncapacitated { get; set; }

	public IFutureProg OnOfferedTruce { get; set; }

	public IFutureProg OnTargetIncapacitated { get; set; }
	public IFutureProg OnNoNaturalTargets { get; set; }

	protected CombatEndAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		var definition = XElement.Parse(ai.Definition);
		WillAcceptTruce = Gameworld.FutureProgs.Get(long.Parse(definition.Element("WillAcceptTruce").Value));
		WillAcceptTargetIncapacitated =
			Gameworld.FutureProgs.Get(long.Parse(definition.Element("WillAcceptTargetIncapacitated").Value));
		OnOfferedTruce = Gameworld.FutureProgs.Get(long.Parse(definition.Element("OnOfferedTruce").Value));
		OnTargetIncapacitated =
			Gameworld.FutureProgs.Get(long.Parse(definition.Element("OnTargetIncapacitated").Value));
		OnNoNaturalTargets = Gameworld.FutureProgs.Get(long.Parse(definition.Element("OnNoNaturalTargets").Value));
	}

	public static void RegisterLoader()
	{
		RegisterAIType("CombatEnd", (ai, gameworld) => new CombatEndAI(ai, gameworld));
	}

	#region Overrides of ArtificialIntelligenceBase

	private void HandleNoNaturalTargets(ICharacter actor)
	{
		OnNoNaturalTargets?.Execute(actor);
		actor.Combat?.TruceRequested(actor);
	}

	private void HandleTargetIncapacitated(ICharacter actor, ICharacter target)
	{
		OnTargetIncapacitated?.Execute(actor, target);
		if (actor.Combat == null || target.Combat == null)
		{
			return;
		}

		if ((bool?)WillAcceptTargetIncapacitated?.Execute(actor, target) ?? false)
		{
			actor.CombatTarget = actor.Combat.Combatants.Where(x => x.CombatTarget == actor)
			                          .Except(target)
			                          .GetRandomElement();
			if (actor.Combat.CanFreelyLeaveCombat(actor))
			{
				actor.Combat.LeaveCombat(actor);
			}

			if (target.Combat.CanFreelyLeaveCombat(target))
			{
				target.Combat.LeaveCombat(target);
			}
		}
	}

	private void HandleTruceOffered(ICharacter actor, ICharacter target)
	{
		OnOfferedTruce?.Execute(actor, target);
		if ((bool?)WillAcceptTruce?.Execute(actor, target) ?? false)
		{
			actor.Combat.TruceRequested(actor);
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.NoNaturalTargets:
				HandleNoNaturalTargets(arguments[0]);
				return true;
			case EventType.TargetIncapacitated:
				HandleTargetIncapacitated(arguments[0], arguments[1]);
				return true;
			case EventType.TruceOffered:
				HandleTruceOffered(arguments[0], arguments[1]);
				return true;
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.NoNaturalTargets:
				case EventType.TargetIncapacitated:
				case EventType.TruceOffered:
					return true;
			}
		}

		return false;
	}

	#endregion
}