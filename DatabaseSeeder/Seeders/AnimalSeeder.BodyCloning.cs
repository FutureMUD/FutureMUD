#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private void CloneBodyDefinition(BodyProto source, BodyProto target, IEnumerable<string>? excludedAliases = null,
		bool cloneAdditionalUsages = true)
	{
		var excluded = new HashSet<string>(excludedAliases ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
		var sourceParts = _context.BodypartProtos
			.Where(x => x.BodyId == source.Id)
			.OrderBy(x => x.DisplayOrder ?? 0)
			.ThenBy(x => x.Id)
			.ToList();
		var sourcePartLookup = sourceParts.ToDictionary(x => x.Id);
		var clonedParts = new Dictionary<long, BodypartProto>();

		foreach (var sourcePart in sourceParts.Where(x => !excluded.Contains(x.Name)))
		{
			var clonedPart = new BodypartProto
			{
				Body = target,
				Name = sourcePart.Name,
				Description = sourcePart.Description,
				BodypartType = sourcePart.BodypartType,
				BodypartShape = sourcePart.BodypartShape,
				DisplayOrder = sourcePart.DisplayOrder,
				MaxLife = sourcePart.MaxLife,
				SeveredThreshold = sourcePart.SeveredThreshold,
				PainModifier = sourcePart.PainModifier,
				BleedModifier = sourcePart.BleedModifier,
				RelativeHitChance = sourcePart.RelativeHitChance,
				Location = sourcePart.Location,
				Alignment = sourcePart.Alignment,
				Unary = sourcePart.Unary,
				MaxSingleSize = sourcePart.MaxSingleSize,
				IsOrgan = sourcePart.IsOrgan,
				WeightLimit = sourcePart.WeightLimit,
				IsCore = sourcePart.IsCore,
				StunModifier = sourcePart.StunModifier,
				DamageModifier = sourcePart.DamageModifier,
				DefaultMaterial = sourcePart.DefaultMaterial,
				Significant = sourcePart.Significant,
				RelativeInfectability = sourcePart.RelativeInfectability,
				HypoxiaDamagePerTick = sourcePart.HypoxiaDamagePerTick,
				IsVital = sourcePart.IsVital,
				ArmourType = sourcePart.ArmourType,
				ImplantSpace = sourcePart.ImplantSpace,
				ImplantSpaceOccupied = sourcePart.ImplantSpaceOccupied,
				Size = sourcePart.Size
			};
			_context.BodypartProtos.Add(clonedPart);
			clonedParts[sourcePart.Id] = clonedPart;
		}

		_context.SaveChanges();

		foreach (var (sourcePartId, clonedPart) in clonedParts)
		{
			if (!sourcePartLookup[sourcePartId].CountAsId.HasValue)
			{
				continue;
			}

			if (clonedParts.TryGetValue(sourcePartLookup[sourcePartId].CountAsId!.Value, out var mappedCountAs))
			{
				clonedPart.CountAs = mappedCountAs;
			}
			else
			{
				clonedPart.CountAsId = sourcePartLookup[sourcePartId].CountAsId;
			}
		}

		_context.SaveChanges();

		var clonedPartIds = clonedParts.Keys.ToList();

		foreach (var upstream in _context.BodypartProtoBodypartProtoUpstream
			         .Where(x => clonedPartIds.Contains(x.Child) && clonedPartIds.Contains(x.Parent))
			         .ToList())
		{
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				ChildNavigation = clonedParts[upstream.Child],
				ParentNavigation = clonedParts[upstream.Parent]
			});
		}

		foreach (var internalInfo in _context.BodypartInternalInfos
			         .Where(x => clonedPartIds.Contains(x.BodypartProtoId) && clonedPartIds.Contains(x.InternalPartId))
			         .ToList())
		{
			_context.BodypartInternalInfos.Add(new BodypartInternalInfos
			{
				BodypartProto = clonedParts[internalInfo.BodypartProtoId],
				InternalPart = clonedParts[internalInfo.InternalPartId],
				HitChance = internalInfo.HitChance,
				IsPrimaryOrganLocation = internalInfo.IsPrimaryOrganLocation,
				ProximityGroup = internalInfo.ProximityGroup
			});
		}

		foreach (var coverage in _context.BoneOrganCoverages
			         .Where(x => clonedPartIds.Contains(x.BoneId) && clonedPartIds.Contains(x.OrganId))
			         .ToList())
		{
			_context.BoneOrganCoverages.Add(new BoneOrganCoverage
			{
				Bone = clonedParts[coverage.BoneId],
				Organ = clonedParts[coverage.OrganId],
				CoverageChance = coverage.CoverageChance
			});
		}

		var limbMap = new Dictionary<long, Limb>();
		foreach (var sourceLimb in _context.Limbs.Where(x => x.RootBodyId == source.Id).ToList())
		{
			if (!clonedParts.TryGetValue(sourceLimb.RootBodypartId, out var clonedRoot))
			{
				continue;
			}

			var clonedLimb = new Limb
			{
				Name = sourceLimb.Name,
				LimbType = sourceLimb.LimbType,
				RootBody = target,
				RootBodypart = clonedRoot,
				LimbDamageThresholdMultiplier = sourceLimb.LimbDamageThresholdMultiplier,
				LimbPainThresholdMultiplier = sourceLimb.LimbPainThresholdMultiplier
			};
			_context.Limbs.Add(clonedLimb);
			limbMap[sourceLimb.Id] = clonedLimb;
		}

		_context.SaveChanges();

		foreach (var sourceLimb in limbMap.Keys)
		{
			foreach (var bodypart in _context.LimbsBodypartProto.Where(x => x.LimbId == sourceLimb))
			{
				if (!clonedParts.TryGetValue(bodypart.BodypartProtoId, out var clonedBodypart))
				{
					continue;
				}

				_context.LimbsBodypartProto.Add(new LimbBodypartProto
				{
					Limb = limbMap[sourceLimb],
					BodypartProto = clonedBodypart
				});
			}

			foreach (var spinalPart in _context.LimbsSpinalParts.Where(x => x.LimbId == sourceLimb))
			{
				if (!clonedParts.TryGetValue(spinalPart.BodypartProtoId, out var clonedSpinalPart))
				{
					continue;
				}

				_context.LimbsSpinalParts.Add(new LimbsSpinalPart
				{
					Limb = limbMap[sourceLimb],
					BodypartProto = clonedSpinalPart
				});
			}
		}

		foreach (var sourceDescriber in _context.BodypartGroupDescribers
			         .Where(x => x.BodypartGroupDescribersBodyProtos.Any(y => y.BodyProtoId == source.Id))
			         .ToList())
		{
			var clonedDescriber = new BodypartGroupDescriber
			{
				DescribedAs = sourceDescriber.DescribedAs,
				Comment = sourceDescriber.Comment,
				Type = sourceDescriber.Type
			};
			_context.BodypartGroupDescribers.Add(clonedDescriber);
			_context.BodypartGroupDescribersBodyProtos.Add(new BodypartGroupDescribersBodyProtos
			{
				BodypartGroupDescriber = clonedDescriber,
				BodyProto = target
			});

			foreach (var shapeCount in _context.BodypartGroupDescribersShapeCount
				         .Where(x => x.BodypartGroupDescriptionRuleId == sourceDescriber.Id))
			{
				_context.BodypartGroupDescribersShapeCount.Add(new BodypartGroupDescribersShapeCount
				{
					BodypartGroupDescriptionRule = clonedDescriber,
					Target = shapeCount.Target,
					MinCount = shapeCount.MinCount,
					MaxCount = shapeCount.MaxCount
				});
			}

			foreach (var bodypart in _context.BodypartGroupDescribersBodypartProtos
				         .Where(x => x.BodypartGroupDescriberId == sourceDescriber.Id))
			{
				if (!clonedParts.TryGetValue(bodypart.BodypartProtoId, out var clonedBodypart))
				{
					continue;
				}

				_context.BodypartGroupDescribersBodypartProtos.Add(new BodypartGroupDescribersBodypartProtos
				{
					BodypartGroupDescriber = clonedDescriber,
					BodypartProto = clonedBodypart,
					Mandatory = bodypart.Mandatory
				});
			}
		}

		if (cloneAdditionalUsages)
		{
			foreach (var usage in _context.BodyProtosAdditionalBodyparts.Where(x => x.BodyProtoId == source.Id).ToList())
			{
				if (!clonedParts.TryGetValue(usage.BodypartId, out var clonedBodypart))
				{
					continue;
				}

				_context.BodyProtosAdditionalBodyparts.Add(new BodyProtosAdditionalBodyparts
				{
					BodyProto = target,
					Bodypart = clonedBodypart,
					Usage = usage.Usage
				});
			}
		}

		_context.SaveChanges();
	}

	private void CloneBodyPositionsAndSpeeds(BodyProto source, BodyProto target)
	{
		foreach (var position in _context.BodyProtosPositions.Where(x => x.BodyProtoId == source.Id).ToList())
		{
			_context.BodyProtosPositions.Add(new BodyProtosPositions
			{
				BodyProto = target,
				Position = position.Position
			});
		}

		var nextSpeedId = _context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		foreach (var speed in _context.MoveSpeeds.Where(x => x.BodyProtoId == source.Id).OrderBy(x => x.Id).ToList())
		{
			_context.MoveSpeeds.Add(new MoveSpeed
			{
				Id = nextSpeedId++,
				BodyProto = target,
				PositionId = speed.PositionId,
				Alias = speed.Alias,
				FirstPersonVerb = speed.FirstPersonVerb,
				ThirdPersonVerb = speed.ThirdPersonVerb,
				PresentParticiple = speed.PresentParticiple,
				Multiplier = speed.Multiplier,
				StaminaMultiplier = speed.StaminaMultiplier
			});
		}

		_context.SaveChanges();
	}
}
