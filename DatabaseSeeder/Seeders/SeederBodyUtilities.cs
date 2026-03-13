#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal static class SeederBodyUtilities
{
	public static void CloneBodyDefinition(
		FuturemudDatabaseContext context,
		BodyProto source,
		BodyProto target,
		IEnumerable<string>? excludedAliases = null,
		bool cloneAdditionalUsages = true)
	{
		var excluded = new HashSet<string>(excludedAliases ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
		var sourceParts = context.BodypartProtos
			.Where(x => x.BodyId == source.Id)
			.OrderBy(x => x.DisplayOrder ?? 0)
			.ThenBy(x => x.Id)
			.ToList();
		var sourcePartLookup = sourceParts.ToDictionary(x => x.Id);
		var clonedParts = new Dictionary<long, BodypartProto>();

		foreach (var sourcePart in sourceParts.Where(x => !excluded.Contains(x.Name)))
		{
			var clonedPart = CloneBodypart(sourcePart, target);
			context.BodypartProtos.Add(clonedPart);
			clonedParts[sourcePart.Id] = clonedPart;
		}

		context.SaveChanges();
		ApplyCountAsMappings(context, sourcePartLookup, clonedParts);
		CloneSharedBodyRelationships(context, clonedParts.Keys, clonedParts);
		CloneLimbs(context, source, target, clonedParts);
		CloneBodypartGroupDescribers(context, source, target, clonedParts);

		if (cloneAdditionalUsages)
		{
			foreach (var usage in context.BodyProtosAdditionalBodyparts.Where(x => x.BodyProtoId == source.Id).ToList())
			{
				if (!clonedParts.TryGetValue(usage.BodypartId, out var clonedBodypart))
				{
					continue;
				}

				context.BodyProtosAdditionalBodyparts.Add(new BodyProtosAdditionalBodyparts
				{
					BodyProto = target,
					Bodypart = clonedBodypart,
					Usage = usage.Usage
				});
			}
		}

		context.SaveChanges();
	}

	public static void CloneBodyPositionsAndSpeeds(FuturemudDatabaseContext context, BodyProto source, BodyProto target)
	{
		foreach (var position in context.BodyProtosPositions.Where(x => x.BodyProtoId == source.Id).ToList())
		{
			context.BodyProtosPositions.Add(new BodyProtosPositions
			{
				BodyProto = target,
				Position = position.Position
			});
		}

		var nextSpeedId = context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		foreach (var speed in context.MoveSpeeds.Where(x => x.BodyProtoId == source.Id).OrderBy(x => x.Id).ToList())
		{
			context.MoveSpeeds.Add(new MoveSpeed
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

		context.SaveChanges();
	}

	public static IReadOnlyDictionary<long, BodypartProto> CloneBodypartSubtree(
		FuturemudDatabaseContext context,
		BodyProto sourceBody,
		BodyProto targetBody,
		string sourceRootAlias,
		string? targetParentAlias = null,
		IReadOnlyDictionary<string, string>? aliasOverrides = null,
		bool cloneLimbs = true)
	{
		var sourceLookup = context.BodypartProtos
			.Where(x => x.BodyId == sourceBody.Id)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		if (!sourceLookup.TryGetValue(sourceRootAlias, out var sourceRoot))
		{
			throw new InvalidOperationException($"Could not find bodypart alias {sourceRootAlias} on body {sourceBody.Name}.");
		}

		var bodyPartIds = sourceLookup.Values.Select(x => x.Id).ToHashSet();
		var upstreams = context.BodypartProtoBodypartProtoUpstream
			.Where(x => bodyPartIds.Contains(x.Child) && bodyPartIds.Contains(x.Parent))
			.ToList();
		var subtreeIds = ExpandSubtree(sourceRoot.Id, upstreams);
		var parts = sourceLookup.Values
			.Where(x => subtreeIds.Contains(x.Id))
			.OrderBy(x => x.DisplayOrder ?? 0)
			.ThenBy(x => x.Id)
			.ToList();
		var partLookupById = parts.ToDictionary(x => x.Id);
		var clonedParts = new Dictionary<long, BodypartProto>();

		foreach (var part in parts)
		{
			var clonedPart = CloneBodypart(part, targetBody);
			if (aliasOverrides is not null && aliasOverrides.TryGetValue(part.Name, out var replacementAlias))
			{
				clonedPart.Name = replacementAlias;
			}

			context.BodypartProtos.Add(clonedPart);
			clonedParts[part.Id] = clonedPart;
		}

		context.SaveChanges();
		ApplyCountAsMappings(context, partLookupById, clonedParts);

		var targetLookup = context.BodypartProtos
			.Where(x => x.BodyId == targetBody.Id)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

		foreach (var upstream in upstreams.Where(x => subtreeIds.Contains(x.Child)))
		{
			if (!clonedParts.TryGetValue(upstream.Child, out var child))
			{
				continue;
			}

			if (clonedParts.TryGetValue(upstream.Parent, out var parent))
			{
				context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
				{
					ChildNavigation = child,
					ParentNavigation = parent
				});
				continue;
			}

			if (upstream.Child == sourceRoot.Id && targetParentAlias is not null &&
			    targetLookup.TryGetValue(targetParentAlias, out var targetParent))
			{
				context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
				{
					ChildNavigation = child,
					ParentNavigation = targetParent
				});
			}
		}

		CloneSharedBodyRelationships(context, subtreeIds, clonedParts);

		if (cloneLimbs)
		{
			var limbRoots = context.Limbs
				.Where(x => x.RootBodyId == sourceBody.Id && subtreeIds.Contains(x.RootBodypartId))
				.ToList();
			CloneLimbs(context, sourceBody, targetBody, clonedParts, limbRoots);
		}

		context.SaveChanges();
		return clonedParts;
	}

	public static void RemoveBodyparts(FuturemudDatabaseContext context, BodyProto body, IEnumerable<string> aliases)
	{
		var aliasSet = new HashSet<string>(aliases, StringComparer.OrdinalIgnoreCase);
		var bodyParts = context.BodypartProtos
			.Where(x => x.BodyId == body.Id)
			.ToList();
		var partLookup = bodyParts.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var upstreams = context.BodypartProtoBodypartProtoUpstream
			.Where(x => bodyParts.Select(y => y.Id).Contains(x.Child) && bodyParts.Select(y => y.Id).Contains(x.Parent))
			.ToList();
		var removeIds = new HashSet<long>();

		foreach (var alias in aliasSet)
		{
			if (!partLookup.TryGetValue(alias, out var part))
			{
				continue;
			}

			foreach (var id in ExpandSubtree(part.Id, upstreams))
			{
				removeIds.Add(id);
			}
		}

		if (!removeIds.Any())
		{
			return;
		}

		var limbs = context.Limbs.Where(x => x.RootBodyId == body.Id).ToList();
		foreach (var limb in limbs.Where(x => removeIds.Contains(x.RootBodypartId)).ToList())
		{
			context.Limbs.Remove(limb);
		}

		foreach (var limbPart in context.LimbsBodypartProto.Where(x => removeIds.Contains(x.BodypartProtoId)).ToList())
		{
			context.LimbsBodypartProto.Remove(limbPart);
		}

		foreach (var spinalPart in context.LimbsSpinalParts.Where(x => removeIds.Contains(x.BodypartProtoId)).ToList())
		{
			context.LimbsSpinalParts.Remove(spinalPart);
		}

		foreach (var internalInfo in context.BodypartInternalInfos
			         .Where(x => removeIds.Contains(x.BodypartProtoId) || removeIds.Contains(x.InternalPartId))
			         .ToList())
		{
			context.BodypartInternalInfos.Remove(internalInfo);
		}

		foreach (var coverage in context.BoneOrganCoverages
			         .Where(x => removeIds.Contains(x.BoneId) || removeIds.Contains(x.OrganId))
			         .ToList())
		{
			context.BoneOrganCoverages.Remove(coverage);
		}

		foreach (var upstream in context.BodypartProtoBodypartProtoUpstream
			         .Where(x => removeIds.Contains(x.Child) || removeIds.Contains(x.Parent))
			         .ToList())
		{
			context.BodypartProtoBodypartProtoUpstream.Remove(upstream);
		}

		foreach (var describerPart in context.BodypartGroupDescribersBodypartProtos
			         .Where(x => removeIds.Contains(x.BodypartProtoId))
			         .ToList())
		{
			context.BodypartGroupDescribersBodypartProtos.Remove(describerPart);
		}

		foreach (var additionalPart in context.BodyProtosAdditionalBodyparts
			         .Where(x => x.BodyProtoId == body.Id && removeIds.Contains(x.BodypartId))
			         .ToList())
		{
			context.BodyProtosAdditionalBodyparts.Remove(additionalPart);
		}

		foreach (var part in bodyParts.Where(x => removeIds.Contains(x.Id)).ToList())
		{
			context.BodypartProtos.Remove(part);
		}

		foreach (var limb in context.Limbs.Where(x => x.RootBodyId == body.Id).ToList())
		{
			var limbBodypartCount = context.LimbsBodypartProto.Count(x => x.LimbId == limb.Id);
			if (limbBodypartCount == 0)
			{
				context.Limbs.Remove(limb);
			}
		}

		context.SaveChanges();
	}

	private static BodypartProto CloneBodypart(BodypartProto sourcePart, BodyProto target)
	{
		return new BodypartProto
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
	}

	private static void ApplyCountAsMappings(
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<long, BodypartProto> sourcePartLookup,
		IReadOnlyDictionary<long, BodypartProto> clonedParts)
	{
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

		context.SaveChanges();
	}

	private static void CloneSharedBodyRelationships(
		FuturemudDatabaseContext context,
		IEnumerable<long> sourcePartIds,
		IReadOnlyDictionary<long, BodypartProto> clonedParts)
	{
		var clonedPartIds = sourcePartIds.ToList();

		foreach (var internalInfo in context.BodypartInternalInfos
			         .Where(x => clonedPartIds.Contains(x.BodypartProtoId) && clonedPartIds.Contains(x.InternalPartId))
			         .ToList())
		{
			context.BodypartInternalInfos.Add(new BodypartInternalInfos
			{
				BodypartProto = clonedParts[internalInfo.BodypartProtoId],
				InternalPart = clonedParts[internalInfo.InternalPartId],
				HitChance = internalInfo.HitChance,
				IsPrimaryOrganLocation = internalInfo.IsPrimaryOrganLocation,
				ProximityGroup = internalInfo.ProximityGroup
			});
		}

		foreach (var coverage in context.BoneOrganCoverages
			         .Where(x => clonedPartIds.Contains(x.BoneId) && clonedPartIds.Contains(x.OrganId))
			         .ToList())
		{
			context.BoneOrganCoverages.Add(new BoneOrganCoverage
			{
				Bone = clonedParts[coverage.BoneId],
				Organ = clonedParts[coverage.OrganId],
				CoverageChance = coverage.CoverageChance
			});
		}
	}

	private static void CloneLimbs(
		FuturemudDatabaseContext context,
		BodyProto source,
		BodyProto target,
		IReadOnlyDictionary<long, BodypartProto> clonedParts,
		IEnumerable<Limb>? sourceLimbs = null)
	{
		var limbMap = new Dictionary<long, Limb>();
		foreach (var sourceLimb in (sourceLimbs ?? context.Limbs.Where(x => x.RootBodyId == source.Id).ToList()))
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
			context.Limbs.Add(clonedLimb);
			limbMap[sourceLimb.Id] = clonedLimb;
		}

		context.SaveChanges();

		foreach (var sourceLimb in limbMap.Keys)
		{
			foreach (var bodypart in context.LimbsBodypartProto.Where(x => x.LimbId == sourceLimb).ToList())
			{
				if (!clonedParts.TryGetValue(bodypart.BodypartProtoId, out var clonedBodypart))
				{
					continue;
				}

				context.LimbsBodypartProto.Add(new LimbBodypartProto
				{
					Limb = limbMap[sourceLimb],
					BodypartProto = clonedBodypart
				});
			}

			foreach (var spinalPart in context.LimbsSpinalParts.Where(x => x.LimbId == sourceLimb).ToList())
			{
				if (!clonedParts.TryGetValue(spinalPart.BodypartProtoId, out var clonedSpinalPart))
				{
					continue;
				}

				context.LimbsSpinalParts.Add(new LimbsSpinalPart
				{
					Limb = limbMap[sourceLimb],
					BodypartProto = clonedSpinalPart
				});
			}
		}
	}

	private static void CloneBodypartGroupDescribers(
		FuturemudDatabaseContext context,
		BodyProto source,
		BodyProto target,
		IReadOnlyDictionary<long, BodypartProto> clonedParts)
	{
		foreach (var sourceDescriber in context.BodypartGroupDescribers
			         .Where(x => x.BodypartGroupDescribersBodyProtos.Any(y => y.BodyProtoId == source.Id))
			         .ToList())
		{
			var clonedDescriber = new BodypartGroupDescriber
			{
				DescribedAs = sourceDescriber.DescribedAs,
				Comment = sourceDescriber.Comment,
				Type = sourceDescriber.Type
			};
			context.BodypartGroupDescribers.Add(clonedDescriber);
			context.BodypartGroupDescribersBodyProtos.Add(new BodypartGroupDescribersBodyProtos
			{
				BodypartGroupDescriber = clonedDescriber,
				BodyProto = target
			});

			foreach (var shapeCount in context.BodypartGroupDescribersShapeCount
				         .Where(x => x.BodypartGroupDescriptionRuleId == sourceDescriber.Id)
				         .ToList())
			{
				context.BodypartGroupDescribersShapeCount.Add(new BodypartGroupDescribersShapeCount
				{
					BodypartGroupDescriptionRule = clonedDescriber,
					Target = shapeCount.Target,
					MinCount = shapeCount.MinCount,
					MaxCount = shapeCount.MaxCount
				});
			}

			foreach (var bodypart in context.BodypartGroupDescribersBodypartProtos
				         .Where(x => x.BodypartGroupDescriberId == sourceDescriber.Id)
				         .ToList())
			{
				if (!clonedParts.TryGetValue(bodypart.BodypartProtoId, out var clonedBodypart))
				{
					continue;
				}

				context.BodypartGroupDescribersBodypartProtos.Add(new BodypartGroupDescribersBodypartProtos
				{
					BodypartGroupDescriber = clonedDescriber,
					BodypartProto = clonedBodypart,
					Mandatory = bodypart.Mandatory
				});
			}
		}
	}

	private static HashSet<long> ExpandSubtree(long rootId, IEnumerable<BodypartProtoBodypartProtoUpstream> upstreams)
	{
		var result = new HashSet<long> { rootId };
		var queue = new Queue<long>();
		queue.Enqueue(rootId);

		var parentLookup = upstreams
			.GroupBy(x => x.Parent)
			.ToDictionary(x => x.Key, x => x.Select(y => y.Child).ToList());

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
			if (!parentLookup.TryGetValue(current, out var children))
			{
				continue;
			}

			foreach (var child in children)
			{
				if (result.Add(child))
				{
					queue.Enqueue(child);
				}
			}
		}

		return result;
	}
}

internal sealed class SeedBodyBuilder
{
	private readonly FuturemudDatabaseContext _context;
	private readonly BodyProto _body;
	private readonly Dictionary<string, BodypartProto> _parts;

	public SeedBodyBuilder(FuturemudDatabaseContext context, BodyProto body)
	{
		_context = context;
		_body = body;
		_parts = _context.BodypartProtos
			.Where(x => x.BodyId == body.Id)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
	}

	public BodypartProto AddBodypart(
		string alias,
		string description,
		string shapeName,
		BodypartTypeEnum type,
		string? parentAlias,
		Alignment alignment,
		Orientation orientation,
		int relativeHitChance,
		int severedThreshold,
		int maxLife,
		int displayOrder,
		string materialName,
		SizeCategory size,
		bool isVital = false,
		bool isCore = false,
		bool isOrgan = false,
		int implantSpace = 0,
		double bleedModifier = 1.0,
		double painModifier = 1.0,
		double damageModifier = 1.0,
		double stunModifier = 1.0)
	{
		var part = new BodypartProto
		{
			Body = _body,
			Name = alias,
			Description = description,
			BodypartShape = _context.BodypartShapes.First(x => x.Name == shapeName),
			BodypartType = (int)type,
			Alignment = (int)alignment,
			Location = (int)orientation,
			RelativeHitChance = relativeHitChance,
			SeveredThreshold = severedThreshold,
			MaxLife = maxLife,
			DisplayOrder = displayOrder,
			DefaultMaterial = _context.Materials.First(x => x.Name == materialName),
			Size = (int)size,
			IsVital = isVital,
			IsCore = isCore,
			IsOrgan = isOrgan ? 1 : 0,
			ImplantSpace = implantSpace,
			BleedModifier = bleedModifier,
			PainModifier = painModifier,
			DamageModifier = damageModifier,
			StunModifier = stunModifier,
			ArmourType = _context.ArmourTypes.FirstOrDefault()
		};
		_context.BodypartProtos.Add(part);
		_context.SaveChanges();
		_parts[alias] = part;

		if (parentAlias is not null && _parts.TryGetValue(parentAlias, out var parent))
		{
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				ChildNavigation = part,
				ParentNavigation = parent
			});
			_context.SaveChanges();
		}

		return part;
	}

	public Limb AddLimb(string name, LimbType limbType, string rootPartAlias, params string[] partAliases)
	{
		var limb = new Limb
		{
			Name = name,
			LimbType = (int)limbType,
			RootBody = _body,
			RootBodypart = _parts[rootPartAlias],
			LimbDamageThresholdMultiplier = 0.5,
			LimbPainThresholdMultiplier = 0.5
		};
		_context.Limbs.Add(limb);
		_context.SaveChanges();

		foreach (var alias in partAliases.Where(_parts.ContainsKey))
		{
			_context.LimbsBodypartProto.Add(new LimbBodypartProto
			{
				Limb = limb,
				BodypartProto = _parts[alias]
			});
		}

		_context.SaveChanges();
		return limb;
	}

	public IReadOnlyDictionary<string, BodypartProto> Parts => _parts;
}
