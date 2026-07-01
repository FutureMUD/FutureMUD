using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.RPG.Law;

public static class EnforcementCustodyHelper
{
	public static ICrime SelectArrestableCrime(ILegalAuthority authority, ICharacter criminal)
	{
		var crimes = authority.KnownCrimesForIndividual(criminal)
		                      .Where(x => x.CriminalIdentityIsKnown)
		                      .Where(x => !x.HasBeenFinalised)
		                      .Where(x => !x.BailPosted)
		                      .Where(x => x.Law.EnforcementStrategy.IsArrestable())
		                      .ToList();

		if (!crimes.Any())
		{
			return null;
		}

		return crimes
		       .WhereMax(x => x.Law.EnforcementStrategy)
		       .OrderByDescending(x => x.Law.EnforcementPriority)
		       .First();
	}

	public static bool IsBeingDraggedBy(IEnumerable<ICharacter> enforcers, ICharacter criminal)
	{
		return enforcers.Any(x => x.CombinedEffectsOfType<Dragging>().Any(y => y.Target == criminal));
	}

	public static Dragging BeginDragging(ICharacter dragger, ICharacter criminal, IEnumerable<ICharacter> helpers)
	{
		if (criminal.AffectedBy<Dragging.DragTarget>())
		{
			return null;
		}

		var drag = new Dragging(dragger, null, criminal);
		dragger.AddEffect(drag);
		dragger.OutputHandler.Send(new EmoteOutput(new Emote("@ begin|begins to drag $1.", dragger, dragger, criminal)));

		foreach (var helper in helpers
		                      .Where(x => !CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(x, dragger))
		                      .Where(x => x.ColocatedWith(dragger))
		                      .Where(x => x.State.IsAble())
		                      .Where(x => x.Effects.All(y => !y.Applies() || !y.IsBlockingEffect("general"))))
		{
			drag.AddHelper(helper, null);
			helper.OutputHandler.Send(new EmoteOutput(new Emote("@ begin|begins to help $1 to drag $2.", helper, helper, dragger, criminal)));
		}

		return drag;
	}

	public static void ReleaseGrapplesAndCombatAgainst(ICharacter criminal, IEnumerable<ICharacter> enforcers)
	{
		var enforcerList = enforcers
		                   .Where(x => x.ColocatedWith(criminal))
		                   .ToList();

		foreach (var enforcer in enforcerList)
		{
			enforcer.RemoveAllEffects<IGrappling>(
				x => CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(x.Target, criminal),
				true);
			enforcer.RemoveAllEffects<ClinchEffect>(
				x => CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(x.Target, criminal),
				true);
			criminal.RemoveAllEffects<ClinchEffect>(
				x => CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(x.Target, enforcer),
				true);

			if (enforcer.CombatTarget is ICharacter enforcerTarget &&
			    CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(enforcerTarget, criminal))
			{
				enforcer.CombatTarget = null;
			}

			if (criminal.CombatTarget is ICharacter criminalTarget &&
			    CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(criminalTarget, enforcer))
			{
				criminal.CombatTarget = null;
			}
		}

		foreach (var enforcer in enforcerList)
		{
			LeaveCombatIfAble(enforcer);
		}

		LeaveCombatIfAble(criminal);
	}

	private static void LeaveCombatIfAble(ICharacter character)
	{
		if (character.Combat?.CanFreelyLeaveCombat(character) == true)
		{
			character.Combat.LeaveCombat(character);
		}
	}
}
