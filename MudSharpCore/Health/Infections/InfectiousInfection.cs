#nullable enable

using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Health.Infections;

public class InfectiousInfection : SimpleInfection
{
	public InfectiousInfection(Difficulty virulenceDifficulty, double intensity, IBody owner, IWound wound,
		IBodypart bodypart, double virulence)
		: base(virulenceDifficulty, intensity, owner, wound, bodypart, virulence)
	{
	}

	public InfectiousInfection(MudSharp.Models.Infection infection, IBody body, IWound wound, IBodypart bodypart)
		: base(infection, body, wound, bodypart)
	{
	}

	public override InfectionType InfectionType => InfectionType.Infectious;

	public override void Spread(Outcome outcome)
	{
		base.Spread(outcome);
		SpreadToOtherCharacters(outcome);
	}

	protected virtual void SpreadToOtherCharacters(Outcome outcome)
	{
		var source = Owner.Actor;
		if (source?.Location == null)
		{
			return;
		}

		var targets = source.LocalThingsAndProximities()
		                    .Where(x => x.Thing is ICharacter)
		                    .Select(x => ((ICharacter)x.Thing, x.Proximity))
		                    .Where(x => x.Item1 != source)
		                    .GroupBy(x => x.Item1)
		                    .Select(x => (Character: x.Key, Proximity: x.Min(y => y.Proximity)))
		                    .ToList();

		foreach (var (target, proximity) in targets)
		{
			if (target.State.HasFlag(CharacterState.Dead) || target.State.HasFlag(CharacterState.Stasis))
			{
				continue;
			}

			if (RandomUtilities.DoubleRandom(0.0, 1.0) > GetSpreadChance(proximity, outcome))
			{
				continue;
			}

			SpreadToTarget(target);
		}
	}

	protected virtual double GetSpreadChance(Proximity proximity, Outcome outcome)
	{
		var baseChance = proximity switch
		{
			Proximity.Intimate => Gameworld.GetStaticDouble("InfectiousInfectionSpreadChanceIntimate"),
			Proximity.Immediate => Gameworld.GetStaticDouble("InfectiousInfectionSpreadChanceImmediate"),
			Proximity.Proximate => Gameworld.GetStaticDouble("InfectiousInfectionSpreadChanceProximate"),
			Proximity.Distant => Gameworld.GetStaticDouble("InfectiousInfectionSpreadChanceDistant"),
			Proximity.VeryDistant => Gameworld.GetStaticDouble("InfectiousInfectionSpreadChanceVeryDistant"),
			_ => 0.0
		};

		var outcomeMultiplier = 1.0 + Math.Max(0, outcome.FailureDegrees()) * 0.25;
		var intensityMultiplier = Math.Max(0.5, Intensity);
		return Math.Min(0.95, baseChance * outcomeMultiplier * intensityMultiplier);
	}

	protected virtual void SpreadToTarget(ICharacter target)
	{
		var existingWound = target.Wounds.FirstOrDefault(x => x.Infection?.InfectionType == InfectionType);
		if (existingWound != null)
		{
			return;
		}

		var targetWound = target.Wounds.Where(x => x.EligableForInfection()).GetRandomElement();
		if (targetWound != null)
		{
			targetWound.Infection = CreateSpreadInfection(target.Body, targetWound, targetWound.Bodypart);
			target.Body.StartHealthTick();
			return;
		}

		if (target.Body.PartInfections.Any(x => x.InfectionType == InfectionType && x.Bodypart == null))
		{
			return;
		}

		target.Body.AddInfection(CreateSpreadInfection(target.Body, null, null));
		target.Body.StartHealthTick();
	}
}
