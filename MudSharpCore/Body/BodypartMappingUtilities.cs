#nullable enable

using MudSharp.Body.PartProtos;
using MudSharp.Health;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Body;

public static class BodypartMappingUtilities
{
	public static IBodypart? MapBodypart(IBodyPrototype targetPrototype, IBodypart? sourcePart,
		bool allowFallback = false)
	{
		if (sourcePart is null)
		{
			return null;
		}

		var strict = MapBodypartStrict(targetPrototype, sourcePart);
		return strict ?? (allowFallback ? FindFallbackBodypart(targetPrototype, sourcePart) : null);
	}

	public static bool TryMapWound(IBody targetBody, IWound wound, out IBodypart? targetPart,
		out IBodypart? targetSeveredPart, out string whyNot)
	{
		targetPart = MapBodypart(targetBody.Prototype, wound.Bodypart, true);
		if (targetPart is null)
		{
			targetSeveredPart = null;
			whyNot =
				$"The wound on {wound.Bodypart?.FullDescription() ?? "the body"} has no matching location on {targetBody.Actor.HowSeen(targetBody.Actor, true)}.";
			return false;
		}

		targetSeveredPart = null;
		if (wound.SeveredBodypart is not null)
		{
			targetSeveredPart = MapBodypart(targetBody.Prototype, wound.SeveredBodypart);
			if (targetSeveredPart is null)
			{
				whyNot =
					$"The severed state for {wound.SeveredBodypart.FullDescription()} cannot be mapped to {targetBody.Actor.HowSeen(targetBody.Actor, true)}.";
				return false;
			}
		}

		whyNot = string.Empty;
		return true;
	}

	private static IBodypart? MapBodypartStrict(IBodyPrototype targetPrototype, IBodypart sourcePart)
	{
		return targetPrototype.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.CountsAs(sourcePart)) ??
		       targetPrototype.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Id == sourcePart.Id) ??
		       targetPrototype.AllBodypartsBonesAndOrgans.FirstOrDefault(x =>
			       x.BodypartType == sourcePart.BodypartType &&
			       x.Alignment == sourcePart.Alignment &&
			       x.Orientation == sourcePart.Orientation &&
			       x.Shape.Id == sourcePart.Shape.Id);
	}

	private static IBodypart? FindFallbackBodypart(IBodyPrototype targetPrototype, IBodypart sourcePart)
	{
		var candidates = targetPrototype.AllBodypartsBonesAndOrgans.AsEnumerable();
		if (sourcePart.BodypartType.IsBone())
		{
			var boneCandidates = candidates.Where(x => x.BodypartType.IsBone()).ToList();
			if (boneCandidates.Any())
			{
				candidates = boneCandidates;
			}
		}
		else if (sourcePart.BodypartType.IsOrgan())
		{
			var organCandidates = candidates.Where(x => x.BodypartType.IsOrgan()).ToList();
			if (organCandidates.Any())
			{
				candidates = organCandidates;
			}
		}

		var mappedUpstream = sourcePart.UpstreamConnection is not null
			? MapBodypart(targetPrototype, sourcePart.UpstreamConnection, true)
			: null;

		return candidates
		       .Select(x => (Part: x, Score: ScoreFallbackBodypart(sourcePart, x, mappedUpstream)))
		       .OrderByDescending(x => x.Score)
		       .ThenByDescending(x => x.Part.IsVital)
		       .ThenByDescending(x => x.Part.IsCore)
		       .ThenByDescending(x => x.Part.RelativeHitChance)
		       .Select(x => x.Part)
		       .FirstOrDefault();
	}

	private static int ScoreFallbackBodypart(IBodypart sourcePart, IBodypart targetPart, IBodypart? mappedUpstream)
	{
		var score = GetEquivalentTypeScore(sourcePart, targetPart);

		if (sourcePart.BodypartType.IsBone() == targetPart.BodypartType.IsBone())
		{
			score += 350;
		}

		if (sourcePart.BodypartType.IsOrgan() == targetPart.BodypartType.IsOrgan())
		{
			score += 350;
		}

		if (sourcePart.Alignment == targetPart.Alignment)
		{
			score += 250;
		}

		if (sourcePart.Orientation == targetPart.Orientation)
		{
			score += 250;
		}

		if (sourcePart.Shape.Id == targetPart.Shape.Id)
		{
			score += 150;
		}

		if (sourcePart.Size == targetPart.Size)
		{
			score += 75;
		}

		if (sourcePart.IsCore == targetPart.IsCore)
		{
			score += 200;
		}

		if (sourcePart.IsVital == targetPart.IsVital)
		{
			score += 250;
		}

		if (sourcePart.Significant == targetPart.Significant)
		{
			score += 75;
		}

		if (mappedUpstream is not null)
		{
			if (targetPart == mappedUpstream)
			{
				score += 500;
			}
			else if (targetPart.DownstreamOfPart(mappedUpstream))
			{
				score += 425;
			}
		}

		return score;
	}

	private static int GetEquivalentTypeScore(IBodypart sourcePart, IBodypart targetPart)
	{
		if (sourcePart.BodypartType == targetPart.BodypartType)
		{
			return 1200;
		}

		return (sourcePart.BodypartType, targetPart.BodypartType) switch
		{
			(BodypartTypeEnum.Brain, BodypartTypeEnum.PositronicBrain) => 1100,
			(BodypartTypeEnum.PositronicBrain, BodypartTypeEnum.Brain) => 1100,
			(BodypartTypeEnum.Brain, BodypartTypeEnum.SensorArray) => 800,
			(BodypartTypeEnum.PositronicBrain, BodypartTypeEnum.SensorArray) => 750,
			(BodypartTypeEnum.SensorArray, BodypartTypeEnum.Brain) => 700,
			(BodypartTypeEnum.SensorArray, BodypartTypeEnum.PositronicBrain) => 750,
			(BodypartTypeEnum.Eye, BodypartTypeEnum.SensorArray) => 1000,
			(BodypartTypeEnum.Ear, BodypartTypeEnum.SensorArray) => 1000,
			(BodypartTypeEnum.SensorArray, BodypartTypeEnum.Eye) => 900,
			(BodypartTypeEnum.SensorArray, BodypartTypeEnum.Ear) => 900,
			(BodypartTypeEnum.Heart, BodypartTypeEnum.PowerCore) => 1100,
			(BodypartTypeEnum.PowerCore, BodypartTypeEnum.Heart) => 1100,
			(BodypartTypeEnum.Mouth, BodypartTypeEnum.SpeechSynthesizer) => 900,
			(BodypartTypeEnum.Tongue, BodypartTypeEnum.SpeechSynthesizer) => 900,
			(BodypartTypeEnum.Esophagus, BodypartTypeEnum.SpeechSynthesizer) => 750,
			(BodypartTypeEnum.Trachea, BodypartTypeEnum.SpeechSynthesizer) => 750,
			(BodypartTypeEnum.SpeechSynthesizer, BodypartTypeEnum.Mouth) => 750,
			(BodypartTypeEnum.SpeechSynthesizer, BodypartTypeEnum.Tongue) => 750,
			(BodypartTypeEnum.SpeechSynthesizer, BodypartTypeEnum.Esophagus) => 650,
			(BodypartTypeEnum.SpeechSynthesizer, BodypartTypeEnum.Trachea) => 650,
			(BodypartTypeEnum.Lung, BodypartTypeEnum.Blowhole) => 700,
			(BodypartTypeEnum.Lung, BodypartTypeEnum.Gill) => 700,
			(BodypartTypeEnum.Gill, BodypartTypeEnum.Lung) => 700,
			(BodypartTypeEnum.Blowhole, BodypartTypeEnum.Lung) => 700,
			(BodypartTypeEnum.Trachea, BodypartTypeEnum.Blowhole) => 650,
			(BodypartTypeEnum.Trachea, BodypartTypeEnum.Gill) => 650,
			(BodypartTypeEnum.Blowhole, BodypartTypeEnum.Trachea) => 650,
			(BodypartTypeEnum.Gill, BodypartTypeEnum.Trachea) => 650,
			_ => 0
		};
	}
}
