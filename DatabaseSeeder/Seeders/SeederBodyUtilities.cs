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
		bool cloneAdditionalUsages = true,
		bool cloneGroupDescribers = true)
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
		CloneBodypartUpstreams(context, sourcePartLookup.Keys, clonedParts);
		CloneSharedBodyRelationships(context, clonedParts.Keys, clonedParts);
		CloneLimbs(context, source, target, clonedParts);
		CloneInheritedLimbMemberships(context, source, clonedParts);
		if (cloneGroupDescribers)
		{
			CloneBodypartGroupDescribers(context, source, target, clonedParts);
		}

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

	public static void CloneFlattenedBodyDefinition(
		FuturemudDatabaseContext context,
		BodyProto source,
		BodyProto target,
		IEnumerable<string>? excludedAliases = null,
		bool cloneAdditionalUsages = true)
	{
		var bodyChain = GetBodyChain(context, source);
		var bodyOrder = bodyChain
			.Select((body, index) => (body.Id, index))
			.ToDictionary(x => x.Id, x => x.index);
		var excluded = new HashSet<string>(excludedAliases ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
		var bodyIds = bodyChain.Select(x => x.Id).ToList();
		var sourceParts = context.BodypartProtos
			.Where(x => bodyIds.Contains(x.BodyId))
			.AsEnumerable()
			.OrderBy(x => bodyOrder[x.BodyId])
			.ThenBy(x => x.DisplayOrder ?? 0)
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
		CloneBodypartUpstreams(context, sourcePartLookup.Keys, clonedParts);
		CloneSharedBodyRelationships(context, sourcePartLookup.Keys, clonedParts);
		CloneFlattenedLimbs(context, bodyIds, target, clonedParts);

		if (cloneAdditionalUsages)
		{
			var existingUsages = new HashSet<(long BodypartId, string Usage)>();
			foreach (var usage in context.BodyProtosAdditionalBodyparts
				         .Where(x => bodyIds.Contains(x.BodyProtoId))
				         .ToList())
			{
				if (!clonedParts.TryGetValue(usage.BodypartId, out var clonedBodypart))
				{
					continue;
				}

				if (!existingUsages.Add((clonedBodypart.Id, usage.Usage)))
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

	public static void CloneFlattenedBodyPositionsAndSpeeds(
		FuturemudDatabaseContext context,
		BodyProto source,
		BodyProto target)
	{
		var bodyChain = GetBodyChain(context, source);
		var bodyOrder = bodyChain
			.Select((body, index) => (body.Id, index))
			.ToDictionary(x => x.Id, x => x.index);
		var bodyIds = bodyChain.Select(x => x.Id).ToList();

		foreach (var positionId in context.BodyProtosPositions
			         .Where(x => bodyIds.Contains(x.BodyProtoId))
					 .AsEnumerable()
			         .OrderBy(x => bodyOrder[x.BodyProtoId])
			         .ThenBy(x => x.Position)
			         .Select(x => x.Position)
			         .Distinct()
			         .ToList())
		{
			context.BodyProtosPositions.Add(new BodyProtosPositions
			{
				BodyProto = target,
				Position = positionId
			});
		}

		var selectedSpeeds = new Dictionary<string, MoveSpeed>(StringComparer.OrdinalIgnoreCase);
		foreach (var speed in context.MoveSpeeds
			         .Where(x => bodyIds.Contains(x.BodyProtoId))
					 .AsEnumerable()
					 .OrderBy(x => bodyOrder[x.BodyProtoId])
			         .ThenBy(x => x.Id)
			         .ToList())
		{
			selectedSpeeds[speed.Alias] = speed;
		}

		var nextSpeedId = context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		foreach (var speed in selectedSpeeds.Values
			         .OrderBy(x => bodyOrder[x.BodyProtoId])
			         .ThenBy(x => x.Id))
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

	public static void ClearBodyDefinition(FuturemudDatabaseContext context, BodyProto body)
	{
		foreach (var speed in context.MoveSpeeds.Where(x => x.BodyProtoId == body.Id).ToList())
		{
			context.MoveSpeeds.Remove(speed);
		}

		foreach (var position in context.BodyProtosPositions.Where(x => x.BodyProtoId == body.Id).ToList())
		{
			context.BodyProtosPositions.Remove(position);
		}

		foreach (var usage in context.BodyProtosAdditionalBodyparts.Where(x => x.BodyProtoId == body.Id).ToList())
		{
			context.BodyProtosAdditionalBodyparts.Remove(usage);
		}

		foreach (var bodyLink in context.BodypartGroupDescribersBodyProtos.Where(x => x.BodyProtoId == body.Id).ToList())
		{
			context.BodypartGroupDescribersBodyProtos.Remove(bodyLink);
		}

		var aliases = context.BodypartProtos
			.Where(x => x.BodyId == body.Id)
			.Select(x => x.Name)
			.AsEnumerable()
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();
		if (aliases.Any())
		{
			RemoveBodyparts(context, body, aliases);
		}

		var limbIds = context.Limbs.Where(x => x.RootBodyId == body.Id).Select(x => x.Id).ToList();
		foreach (var limbPart in context.LimbsBodypartProto.Where(x => limbIds.Contains(x.LimbId)).ToList())
		{
			context.LimbsBodypartProto.Remove(limbPart);
		}

		foreach (var spinalPart in context.LimbsSpinalParts.Where(x => limbIds.Contains(x.LimbId)).ToList())
		{
			context.LimbsSpinalParts.Remove(spinalPart);
		}

		foreach (var limb in context.Limbs.Where(x => x.RootBodyId == body.Id).ToList())
		{
			context.Limbs.Remove(limb);
		}

		body.DefaultSmashingBodypartId = null;
		context.SaveChanges();
	}

	internal static IReadOnlyDictionary<string, IReadOnlyList<BodypartProto>> BuildBodypartAliasLookup(
		IEnumerable<BodypartProto> parts)
	{
		return parts
			.OrderBy(x => x.DisplayOrder ?? 0)
			.ThenBy(x => x.Id)
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				x => x.Key,
				x => (IReadOnlyList<BodypartProto>)x.ToList(),
				StringComparer.OrdinalIgnoreCase);
	}

	internal static IReadOnlyDictionary<string, BodypartProto> BuildBodypartLookup(IEnumerable<BodypartProto> parts)
	{
		return BuildBodypartAliasLookup(parts)
			.ToDictionary(x => x.Key, x => x.Value[0], StringComparer.OrdinalIgnoreCase);
	}

	public static IReadOnlyList<BodypartProto> GetExternalBodypartsWithoutLimbCoverage(
		FuturemudDatabaseContext context,
		BodyProto body)
	{
		static bool IsBoneType(BodypartProto part)
		{
			return ((BodypartTypeEnum)part.BodypartType) switch
			{
				BodypartTypeEnum.Bone => true,
				BodypartTypeEnum.NonImmobilisingBone => true,
				BodypartTypeEnum.MinorBone => true,
				BodypartTypeEnum.MinorNonImobilisingBone => true,
				_ => false
			};
		}

		var bodyIds = GetBodyAndAncestorIds(context, body);
		var bodyIdSet = bodyIds.ToHashSet();
		var parts = GetEffectiveBodyparts(context, body);
		if (!parts.Any())
		{
			return [];
		}

		var externalParts = parts
			.Where(x => !IsBoneType(x) && x.IsOrgan == 0)
			.OrderBy(x => bodyIds.IndexOf(x.BodyId))
			.ThenBy(x => x.DisplayOrder ?? 0)
			.ThenBy(x => x.Id)
			.ToList();
		if (!externalParts.Any())
		{
			return [];
		}

		var limbIds = context.Limbs
			.Where(x => bodyIdSet.Contains(x.RootBodyId))
			.Select(x => x.Id)
			.ToList();
		if (!limbIds.Any())
		{
			return externalParts;
		}

		var linkedBodypartIds = context.LimbsBodypartProto
			.Where(x => limbIds.Contains(x.LimbId))
			.Select(x => x.BodypartProtoId)
			.ToHashSet();

		return externalParts
			.Where(x => !linkedBodypartIds.Contains(x.Id))
			.ToList();
	}

	internal static IReadOnlyList<BodypartProto> GetEffectiveBodyparts(
		FuturemudDatabaseContext context,
		BodyProto body)
	{
		var effectiveParts = new List<BodypartProto>();
		foreach (var currentBody in GetBodyChain(context, body))
		{
			var currentParts = context.BodypartProtos
				.Where(x => x.BodyId == currentBody.Id)
				.OrderBy(x => x.DisplayOrder ?? 0)
				.ThenBy(x => x.Id)
				.ToList();

			foreach (var part in currentParts)
			{
				effectiveParts.RemoveAll(x =>
					x.Id == part.CountAsId ||
					x.Name.Equals(part.Name, StringComparison.OrdinalIgnoreCase));
				effectiveParts.Add(part);
			}

			var removals = context.BodyProtosAdditionalBodyparts
				.Where(x => x.BodyProtoId == currentBody.Id && x.Usage == "remove")
				.Select(x => x.BodypartId)
				.ToList();
			if (!removals.Any())
			{
				continue;
			}

			var effectivePartIds = effectiveParts.Select(x => x.Id).ToHashSet();
			var upstreams = context.BodypartProtoBodypartProtoUpstream
				.Where(x => effectivePartIds.Contains(x.Child) && effectivePartIds.Contains(x.Parent))
				.ToList();
			var removeIds = new HashSet<long>();
			foreach (var removalId in removals.Where(effectivePartIds.Contains))
			{
				foreach (var id in ExpandSubtree(removalId, upstreams))
				{
					removeIds.Add(id);
				}
			}

			effectiveParts.RemoveAll(x => removeIds.Contains(x.Id));
		}

		return effectiveParts;
	}

	internal static BodypartProto? FindBodypartOnBodyOrAncestors(
		FuturemudDatabaseContext context,
		BodyProto body,
		string alias)
	{
		var bodyIds = GetBodyAndAncestorIds(context, body);
		return context.BodypartProtos
			.Where(x => bodyIds.Contains(x.BodyId) && x.Name == alias)
			.AsEnumerable()
			.OrderBy(x => bodyIds.IndexOf(x.BodyId))
			.ThenBy(x => x.DisplayOrder ?? 0)
			.ThenBy(x => x.Id)
			.FirstOrDefault();
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
			.ToList();
		var sourcePartsByAlias = BuildBodypartAliasLookup(sourceLookup);
		var sourceParts = sourcePartsByAlias.Values.SelectMany(x => x).ToList();
		var sourcePartLookup = BuildBodypartLookup(sourceParts);
		if (!sourcePartLookup.TryGetValue(sourceRootAlias, out var sourceRoot))
		{
			throw new InvalidOperationException($"Could not find bodypart alias {sourceRootAlias} on body {sourceBody.Name}.");
		}

		var bodyPartIds = sourceParts.Select(x => x.Id).ToHashSet();
		var upstreams = context.BodypartProtoBodypartProtoUpstream
			.Where(x => bodyPartIds.Contains(x.Child) && bodyPartIds.Contains(x.Parent))
			.ToList();
		var subtreeIds = ExpandSubtree(sourceRoot.Id, upstreams);
		var parts = sourceParts
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
			    FindBodypartOnBodyOrAncestors(context, targetBody, targetParentAlias) is { } targetParent)
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
		var partLookup = BuildBodypartAliasLookup(bodyParts);
		var upstreams = context.BodypartProtoBodypartProtoUpstream
			.Where(x => bodyParts.Select(y => y.Id).Contains(x.Child) && bodyParts.Select(y => y.Id).Contains(x.Parent))
			.ToList();
		var removeIds = new HashSet<long>();

		foreach (var alias in aliasSet)
		{
			if (!partLookup.TryGetValue(alias, out var partsForAlias))
			{
				continue;
			}

			foreach (var part in partsForAlias)
			{
				foreach (var id in ExpandSubtree(part.Id, upstreams))
				{
					removeIds.Add(id);
				}
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

	private static List<BodyProto> GetBodyChain(FuturemudDatabaseContext context, BodyProto body)
	{
		var bodies = new List<BodyProto>();
		var visited = new HashSet<long>();
		BodyProto? currentBody = body;

		while (currentBody is not null && visited.Add(currentBody.Id))
		{
			bodies.Add(currentBody);
			currentBody = currentBody.CountsAsId.HasValue
				? context.BodyProtos.Find(currentBody.CountsAsId.Value)
				: null;
		}

		bodies.Reverse();
		return bodies;
	}

	internal static List<long> GetBodyAndAncestorIds(FuturemudDatabaseContext context, BodyProto body)
	{
		return GetBodyChain(context, body)
			.Select(x => x.Id)
			.Reverse()
			.ToList();
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

	private static void CloneBodypartUpstreams(
		FuturemudDatabaseContext context,
		IEnumerable<long> sourcePartIds,
		IReadOnlyDictionary<long, BodypartProto> clonedParts)
	{
		var partIds = sourcePartIds.ToHashSet();
		foreach (var upstream in context.BodypartProtoBodypartProtoUpstream
			         .Where(x => partIds.Contains(x.Child) && partIds.Contains(x.Parent))
			         .ToList())
		{
			if (!clonedParts.TryGetValue(upstream.Child, out var child) ||
			    !clonedParts.TryGetValue(upstream.Parent, out var parent))
			{
				continue;
			}

			context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				ChildNavigation = child,
				ParentNavigation = parent
			});
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
			if (!clonedParts.ContainsKey(internalInfo.BodypartProtoId) || !clonedParts.ContainsKey(internalInfo.InternalPartId))
			{
				continue;
			}

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
			if (!clonedParts.ContainsKey(coverage.BoneId) || !clonedParts.ContainsKey(coverage.OrganId))
			{
				continue;
			}

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

	private static void CloneFlattenedLimbs(
		FuturemudDatabaseContext context,
		IEnumerable<long> bodyIds,
		BodyProto target,
		IReadOnlyDictionary<long, BodypartProto> clonedParts)
	{
		var sourceBodyIds = bodyIds.ToList();
		var bodyOrder = sourceBodyIds
			.Select((id, index) => (id, index))
			.ToDictionary(x => x.id, x => x.index);
		var limbMap = new Dictionary<long, Limb>();

		foreach (var sourceLimb in context.Limbs
			         .Where(x => sourceBodyIds.Contains(x.RootBodyId))
					 .AsEnumerable() 
			         .OrderBy(x => bodyOrder[x.RootBodyId])
			         .ThenBy(x => x.Id)
			         .ToList())
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

		foreach (var sourceLimbId in limbMap.Keys)
		{
			foreach (var bodypart in context.LimbsBodypartProto.Where(x => x.LimbId == sourceLimbId).ToList())
			{
				if (!clonedParts.TryGetValue(bodypart.BodypartProtoId, out var clonedBodypart))
				{
					continue;
				}

				context.LimbsBodypartProto.Add(new LimbBodypartProto
				{
					Limb = limbMap[sourceLimbId],
					BodypartProto = clonedBodypart
				});
			}

			foreach (var spinalPart in context.LimbsSpinalParts.Where(x => x.LimbId == sourceLimbId).ToList())
			{
				if (!clonedParts.TryGetValue(spinalPart.BodypartProtoId, out var clonedSpinalPart))
				{
					continue;
				}

				context.LimbsSpinalParts.Add(new LimbsSpinalPart
				{
					Limb = limbMap[sourceLimbId],
					BodypartProto = clonedSpinalPart
				});
			}
		}

		context.SaveChanges();
	}

	private static void CloneInheritedLimbMemberships(
		FuturemudDatabaseContext context,
		BodyProto source,
		IReadOnlyDictionary<long, BodypartProto> clonedParts)
	{
		var inheritedBodyIds = new HashSet<long>();
		var currentBody = source;
		while (currentBody.CountsAsId is long parentId)
		{
			if (!inheritedBodyIds.Add(parentId))
			{
				break;
			}

			currentBody = context.BodyProtos.Find(parentId);
			if (currentBody is null)
			{
				break;
			}
		}

		if (!inheritedBodyIds.Any())
		{
			return;
		}

		var inheritedLimbs = context.Limbs
			.Where(x => inheritedBodyIds.Contains(x.RootBodyId))
			.ToList();
		if (!inheritedLimbs.Any())
		{
			return;
		}

		var inheritedLimbIds = inheritedLimbs.Select(x => x.Id).ToList();
		var sourcePartIds = clonedParts.Keys.ToList();
		var existingBodypartLinks = context.LimbsBodypartProto
			.Where(x => inheritedLimbIds.Contains(x.LimbId))
			.ToList()
			.Select(x => (x.LimbId, x.BodypartProtoId))
			.ToHashSet();
		foreach (var bodypartLink in context.LimbsBodypartProto
			         .Where(x => inheritedLimbIds.Contains(x.LimbId) && sourcePartIds.Contains(x.BodypartProtoId))
			         .ToList())
		{
			var clonedBodypart = clonedParts[bodypartLink.BodypartProtoId];
			if (!existingBodypartLinks.Add((bodypartLink.LimbId, clonedBodypart.Id)))
			{
				continue;
			}

			context.LimbsBodypartProto.Add(new LimbBodypartProto
			{
				LimbId = bodypartLink.LimbId,
				BodypartProto = clonedBodypart
			});
		}

		var existingSpinalLinks = context.LimbsSpinalParts
			.Where(x => inheritedLimbIds.Contains(x.LimbId))
			.ToList()
			.Select(x => (x.LimbId, x.BodypartProtoId))
			.ToHashSet();
		foreach (var spinalLink in context.LimbsSpinalParts
			         .Where(x => inheritedLimbIds.Contains(x.LimbId) && sourcePartIds.Contains(x.BodypartProtoId))
			         .ToList())
		{
			var clonedSpinalPart = clonedParts[spinalLink.BodypartProtoId];
			if (!existingSpinalLinks.Add((spinalLink.LimbId, clonedSpinalPart.Id)))
			{
				continue;
			}

			context.LimbsSpinalParts.Add(new LimbsSpinalPart
			{
				LimbId = spinalLink.LimbId,
				BodypartProto = clonedSpinalPart
			});
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
		_parts = SeederBodyUtilities.BuildBodypartLookup(_context.BodypartProtos
			.Where(x => x.BodyId == body.Id))
			.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
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
