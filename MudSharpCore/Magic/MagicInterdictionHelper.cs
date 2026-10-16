using MudSharp.Construction;

#nullable enable

namespace MudSharp.Magic;

internal sealed record MagicInterdictionResult(
	IMagicInterdictionEffect Effect,
	IPerceivable Owner,
	MagicInterdictionMode Mode,
	IMagicInterdictionUse? Use = null
);

internal static class MagicInterdictionHelper
{
	public static MagicInterdictionResult? GetInterdiction(ICharacter source, IPerceivable? target, IMagicSchool school,
		bool allowReflection, params SpellAdditionalParameter[] additionalParameters)
	{
		return GetInterdiction(source, target, school, allowReflection, [], additionalParameters);
	}

	public static MagicInterdictionResult? GetInterdiction(ICharacter source, IPerceivable? target, IMagicSchool school,
		bool allowReflection, IEnumerable<MagicInterdictionTag> tags, params SpellAdditionalParameter[] additionalParameters)
	{
		var context = new MagicInterdictionContext(source, target, school, tags.ToList(), additionalParameters.ToList());
		foreach ((IMagicInterdictionEffect effect, IPerceivable owner) in GetRelevantEffects(source, target, additionalParameters))
		{
			var limited = effect is IMagicInterdictionUseEffect { UsesAreLimited: true };
			var use = limited ? ((IMagicInterdictionUseEffect)effect).CaptureUse(context) : null;
			if (limited && use is null) continue;
			var shouldInterdict =
				effect is IMagicContextualInterdictionEffect contextual
					? contextual.ShouldInterdict(context)
					: effect.ShouldInterdict(source, school);

			if (!shouldInterdict && use?.IsCurrent != false)
			{
				continue;
			}

			return new MagicInterdictionResult(
				effect,
				owner,
				allowReflection ? effect.Mode : MagicInterdictionMode.Fail,
				use
			);
		}

		return null;
	}

	public static bool TryCommitReflection(MagicInterdictionResult result) =>
		result.Mode == MagicInterdictionMode.Reflect && (result.Use?.TryCommit() ?? true);

	private static IEnumerable<(IMagicInterdictionEffect Effect, IPerceivable Owner)> GetRelevantEffects(
		ICharacter source,
		IPerceivable? target,
		IEnumerable<SpellAdditionalParameter> additionalParameters)
	{
		List<(IMagicInterdictionEffect Effect, IPerceivable Owner)> results = [];
		HashSet<IMagicInterdictionEffect> seen = [];

		AddEffects(source, MagicInterdictionCoverage.Outgoing, results, seen);
		AddEffects(source?.Location, MagicInterdictionCoverage.Outgoing, results, seen);

		foreach (IRoom room in ResolveIncomingRooms(target, additionalParameters))
		{
			AddEffects(room, MagicInterdictionCoverage.Incoming, results, seen);
		}

		if (target is ICharacter targetCharacter)
		{
			AddEffects(targetCharacter, MagicInterdictionCoverage.Incoming, results, seen);
		}

		return results;
	}

	private static IEnumerable<IRoom> ResolveIncomingRooms(IPerceivable? target,
		IEnumerable<SpellAdditionalParameter> additionalParameters)
	{
		HashSet<IRoom> rooms = [];

		if (target is IRoom cellTarget)
		{
			rooms.Add(cellTarget);
		}

		if (target?.Location is not null)
		{
			rooms.Add(target.Location);
		}

		foreach (IRoom room in additionalParameters
			         .Select(x => x.Item)
			         .OfType<IRoom>())
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
