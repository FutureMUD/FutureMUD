using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Magic;

internal sealed record MagicInterdictionResult(
	IMagicInterdictionEffect Effect,
	IPerceivable Owner,
	MagicInterdictionMode Mode
);

internal static class MagicInterdictionHelper
{
	public static MagicInterdictionResult? GetInterdiction(ICharacter source, IPerceivable? target, IMagicSchool school,
		bool allowReflection, params SpellAdditionalParameter[] additionalParameters)
	{
		foreach ((IMagicInterdictionEffect effect, IPerceivable owner) in GetRelevantEffects(source, target, additionalParameters))
		{
			if (!effect.ShouldInterdict(source, school))
			{
				continue;
			}

			return new MagicInterdictionResult(
				effect,
				owner,
				allowReflection ? effect.Mode : MagicInterdictionMode.Fail
			);
		}

		return null;
	}

	private static IEnumerable<(IMagicInterdictionEffect Effect, IPerceivable Owner)> GetRelevantEffects(
		ICharacter source,
		IPerceivable? target,
		IEnumerable<SpellAdditionalParameter> additionalParameters)
	{
		List<(IMagicInterdictionEffect Effect, IPerceivable Owner)> results = [];
		HashSet<IMagicInterdictionEffect> seen = [];

		AddEffects(source, MagicInterdictionCoverage.Outgoing, results, seen);
		AddEffects(source.Location, MagicInterdictionCoverage.Outgoing, results, seen);

		foreach (ICell room in ResolveIncomingRooms(target, additionalParameters))
		{
			AddEffects(room, MagicInterdictionCoverage.Incoming, results, seen);
		}

		if (target is ICharacter targetCharacter)
		{
			AddEffects(targetCharacter, MagicInterdictionCoverage.Incoming, results, seen);
		}

		return results;
	}

	private static IEnumerable<ICell> ResolveIncomingRooms(IPerceivable? target,
		IEnumerable<SpellAdditionalParameter> additionalParameters)
	{
		HashSet<ICell> rooms = [];

		if (target is ICell cellTarget)
		{
			rooms.Add(cellTarget);
		}

		if (target?.Location is not null)
		{
			rooms.Add(target.Location);
		}

		foreach (ICell room in additionalParameters
			         .Where(x => x.Item is ICell)
			         .Select(x => (ICell)x.Item))
		{
			rooms.Add(room);
		}

		return rooms;
	}

	private static void AddEffects(IPerceivable? owner, MagicInterdictionCoverage requiredCoverage,
		ICollection<(IMagicInterdictionEffect Effect, IPerceivable Owner)> results,
		ISet<IMagicInterdictionEffect> seen)
	{
		if (owner is null)
		{
			return;
		}

		foreach (IMagicInterdictionEffect effect in owner.EffectsOfType<IMagicInterdictionEffect>()
		             .Where(x => CoverageMatches(x.Coverage, requiredCoverage)))
		{
			if (!seen.Add(effect))
			{
				continue;
			}

			results.Add((effect, owner));
		}
	}

	private static bool CoverageMatches(MagicInterdictionCoverage actual, MagicInterdictionCoverage required)
	{
		return actual == MagicInterdictionCoverage.Both || actual == required;
	}
}
