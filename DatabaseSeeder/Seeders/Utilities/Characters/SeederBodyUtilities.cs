#nullable enable

using MudSharp.Body;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        HashSet<string> excluded = new(excludedAliases ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        List<BodypartProto> sourceParts = context.BodypartProtos
            .Where(x => x.BodyId == source.Id)
            .OrderBy(x => x.DisplayOrder ?? 0)
            .ThenBy(x => x.Id)
            .ToList();
        Dictionary<long, BodypartProto> sourcePartLookup = sourceParts.ToDictionary(x => x.Id);
        Dictionary<long, BodypartProto> clonedParts = new();

        foreach (BodypartProto? sourcePart in sourceParts.Where(x => !excluded.Contains(x.Name)))
        {
            BodypartProto clonedPart = CloneBodypart(sourcePart, target);
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
            foreach (BodyProtosAdditionalBodyparts? usage in context.BodyProtosAdditionalBodyparts.Where(x => x.BodyProtoId == source.Id).ToList())
            {
                if (!clonedParts.TryGetValue(usage.BodypartId, out BodypartProto? clonedBodypart))
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
        foreach (BodyProtosPositions? position in context.BodyProtosPositions.Where(x => x.BodyProtoId == source.Id).ToList())
        {
            context.BodyProtosPositions.Add(new BodyProtosPositions
            {
                BodyProto = target,
                Position = position.Position
            });
        }

        long nextSpeedId = context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
        foreach (MoveSpeed? speed in context.MoveSpeeds.Where(x => x.BodyProtoId == source.Id).OrderBy(x => x.Id).ToList())
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
        List<BodyProto> bodyChain = GetBodyChain(context, source);
        Dictionary<long, int> bodyOrder = bodyChain
            .Select((body, index) => (body.Id, index))
            .ToDictionary(x => x.Id, x => x.index);
        HashSet<string> excluded = new(excludedAliases ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        List<long> bodyIds = bodyChain.Select(x => x.Id).ToList();
        List<BodypartProto> sourceParts = context.BodypartProtos
            .Where(x => bodyIds.Contains(x.BodyId))
            .AsEnumerable()
            .OrderBy(x => bodyOrder[x.BodyId])
            .ThenBy(x => x.DisplayOrder ?? 0)
            .ThenBy(x => x.Id)
            .ToList();
        Dictionary<long, BodypartProto> sourcePartLookup = sourceParts.ToDictionary(x => x.Id);
        Dictionary<long, BodypartProto> clonedParts = new();

        foreach (BodypartProto? sourcePart in sourceParts.Where(x => !excluded.Contains(x.Name)))
        {
            BodypartProto clonedPart = CloneBodypart(sourcePart, target);
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
            HashSet<(long BodypartId, string Usage)> existingUsages = new();
            foreach (BodyProtosAdditionalBodyparts? usage in context.BodyProtosAdditionalBodyparts
                         .Where(x => bodyIds.Contains(x.BodyProtoId))
                         .ToList())
            {
                if (!clonedParts.TryGetValue(usage.BodypartId, out BodypartProto? clonedBodypart))
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
        List<BodyProto> bodyChain = GetBodyChain(context, source);
        Dictionary<long, int> bodyOrder = bodyChain
            .Select((body, index) => (body.Id, index))
            .ToDictionary(x => x.Id, x => x.index);
        List<long> bodyIds = bodyChain.Select(x => x.Id).ToList();

        foreach (int positionId in context.BodyProtosPositions
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

        Dictionary<string, MoveSpeed> selectedSpeeds = new(StringComparer.OrdinalIgnoreCase);
        foreach (MoveSpeed? speed in context.MoveSpeeds
                     .Where(x => bodyIds.Contains(x.BodyProtoId))
                     .AsEnumerable()
                     .OrderBy(x => bodyOrder[x.BodyProtoId])
                     .ThenBy(x => x.Id)
                     .ToList())
        {
            selectedSpeeds[speed.Alias] = speed;
        }

        long nextSpeedId = context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
        foreach (MoveSpeed? speed in selectedSpeeds.Values
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
        foreach (MoveSpeed? speed in context.MoveSpeeds.Where(x => x.BodyProtoId == body.Id).ToList())
        {
            context.MoveSpeeds.Remove(speed);
        }

        foreach (BodyProtosPositions? position in context.BodyProtosPositions.Where(x => x.BodyProtoId == body.Id).ToList())
        {
            context.BodyProtosPositions.Remove(position);
        }

        foreach (BodyProtosAdditionalBodyparts? usage in context.BodyProtosAdditionalBodyparts.Where(x => x.BodyProtoId == body.Id).ToList())
        {
            context.BodyProtosAdditionalBodyparts.Remove(usage);
        }

        foreach (BodypartGroupDescribersBodyProtos? bodyLink in context.BodypartGroupDescribersBodyProtos.Where(x => x.BodyProtoId == body.Id).ToList())
        {
            context.BodypartGroupDescribersBodyProtos.Remove(bodyLink);
        }

        List<string> aliases = context.BodypartProtos
            .Where(x => x.BodyId == body.Id)
            .Select(x => x.Name)
            .AsEnumerable()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (aliases.Any())
        {
            RemoveBodyparts(context, body, aliases);
        }

        List<long> limbIds = context.Limbs.Where(x => x.RootBodyId == body.Id).Select(x => x.Id).ToList();
        foreach (LimbBodypartProto? limbPart in context.LimbsBodypartProto.Where(x => limbIds.Contains(x.LimbId)).ToList())
        {
            context.LimbsBodypartProto.Remove(limbPart);
        }

        foreach (LimbsSpinalPart? spinalPart in context.LimbsSpinalParts.Where(x => limbIds.Contains(x.LimbId)).ToList())
        {
            context.LimbsSpinalParts.Remove(spinalPart);
        }

        foreach (Limb? limb in context.Limbs.Where(x => x.RootBodyId == body.Id).ToList())
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

        List<long> bodyIds = GetBodyAndAncestorIds(context, body);
        HashSet<long> bodyIdSet = bodyIds.ToHashSet();
        IReadOnlyList<BodypartProto> parts = GetEffectiveBodyparts(context, body);
        if (!parts.Any())
        {
            return [];
        }

        List<BodypartProto> externalParts = parts
            .Where(x => !IsBoneType(x) && x.IsOrgan == 0)
            .OrderBy(x => bodyIds.IndexOf(x.BodyId))
            .ThenBy(x => x.DisplayOrder ?? 0)
            .ThenBy(x => x.Id)
            .ToList();
        if (!externalParts.Any())
        {
            return [];
        }

        List<long> limbIds = context.Limbs
            .Where(x => bodyIdSet.Contains(x.RootBodyId))
            .Select(x => x.Id)
            .ToList();
        if (!limbIds.Any())
        {
            return externalParts;
        }

        HashSet<long> linkedBodypartIds = context.LimbsBodypartProto
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
        List<BodypartProto> effectiveParts = new();
        foreach (BodyProto currentBody in GetBodyChain(context, body))
        {
            List<BodypartProto> currentParts = context.BodypartProtos
                .Where(x => x.BodyId == currentBody.Id)
                .OrderBy(x => x.DisplayOrder ?? 0)
                .ThenBy(x => x.Id)
                .ToList();

            foreach (BodypartProto part in currentParts)
            {
                effectiveParts.RemoveAll(x =>
                    x.Id == part.CountAsId ||
                    x.Name.Equals(part.Name, StringComparison.OrdinalIgnoreCase));
                effectiveParts.Add(part);
            }

            List<long> removals = context.BodyProtosAdditionalBodyparts
                .Where(x => x.BodyProtoId == currentBody.Id && x.Usage == "remove")
                .Select(x => x.BodypartId)
                .ToList();
            if (!removals.Any())
            {
                continue;
            }

            HashSet<long> effectivePartIds = effectiveParts.Select(x => x.Id).ToHashSet();
            List<BodypartProtoBodypartProtoUpstream> upstreams = context.BodypartProtoBodypartProtoUpstream
                .Where(x => effectivePartIds.Contains(x.Child) && effectivePartIds.Contains(x.Parent))
                .ToList();
            HashSet<long> removeIds = new();
            foreach (long removalId in removals.Where(effectivePartIds.Contains))
            {
                foreach (long id in ExpandSubtree(removalId, upstreams))
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
        List<long> bodyIds = GetBodyAndAncestorIds(context, body);
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
        List<BodypartProto> sourceLookup = context.BodypartProtos
            .Where(x => x.BodyId == sourceBody.Id)
            .ToList();
        IReadOnlyDictionary<string, IReadOnlyList<BodypartProto>> sourcePartsByAlias = BuildBodypartAliasLookup(sourceLookup);
        List<BodypartProto> sourceParts = sourcePartsByAlias.Values.SelectMany(x => x).ToList();
        IReadOnlyDictionary<string, BodypartProto> sourcePartLookup = BuildBodypartLookup(sourceParts);
        if (!sourcePartLookup.TryGetValue(sourceRootAlias, out BodypartProto? sourceRoot))
        {
            throw new InvalidOperationException($"Could not find bodypart alias {sourceRootAlias} on body {sourceBody.Name}.");
        }

        HashSet<long> bodyPartIds = sourceParts.Select(x => x.Id).ToHashSet();
        List<BodypartProtoBodypartProtoUpstream> upstreams = context.BodypartProtoBodypartProtoUpstream
            .Where(x => bodyPartIds.Contains(x.Child) && bodyPartIds.Contains(x.Parent))
            .ToList();
        HashSet<long> subtreeIds = ExpandSubtree(sourceRoot.Id, upstreams);
        List<BodypartProto> parts = sourceParts
            .Where(x => subtreeIds.Contains(x.Id))
            .OrderBy(x => x.DisplayOrder ?? 0)
            .ThenBy(x => x.Id)
            .ToList();
        Dictionary<long, BodypartProto> partLookupById = parts.ToDictionary(x => x.Id);
        Dictionary<long, BodypartProto> clonedParts = new();

        foreach (BodypartProto part in parts)
        {
            BodypartProto clonedPart = CloneBodypart(part, targetBody);
            if (aliasOverrides is not null && aliasOverrides.TryGetValue(part.Name, out string? replacementAlias))
            {
                clonedPart.Name = replacementAlias;
            }

            context.BodypartProtos.Add(clonedPart);
            clonedParts[part.Id] = clonedPart;
        }

        context.SaveChanges();
        ApplyCountAsMappings(context, partLookupById, clonedParts);

        foreach (BodypartProtoBodypartProtoUpstream? upstream in upstreams.Where(x => subtreeIds.Contains(x.Child)))
        {
            if (!clonedParts.TryGetValue(upstream.Child, out BodypartProto? child))
            {
                continue;
            }

            if (clonedParts.TryGetValue(upstream.Parent, out BodypartProto? parent))
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
            List<Limb> limbRoots = context.Limbs
                .Where(x => x.RootBodyId == sourceBody.Id && subtreeIds.Contains(x.RootBodypartId))
                .ToList();
            CloneLimbs(context, sourceBody, targetBody, clonedParts, limbRoots);
        }

        context.SaveChanges();
        return clonedParts;
    }

    public static void RemoveBodyparts(FuturemudDatabaseContext context, BodyProto body, IEnumerable<string> aliases)
    {
        HashSet<string> aliasSet = new(aliases, StringComparer.OrdinalIgnoreCase);
        List<BodypartProto> bodyParts = context.BodypartProtos
            .Where(x => x.BodyId == body.Id)
            .ToList();
        IReadOnlyDictionary<string, IReadOnlyList<BodypartProto>> partLookup = BuildBodypartAliasLookup(bodyParts);
        List<BodypartProtoBodypartProtoUpstream> upstreams = context.BodypartProtoBodypartProtoUpstream
            .Where(x => bodyParts.Select(y => y.Id).Contains(x.Child) && bodyParts.Select(y => y.Id).Contains(x.Parent))
            .ToList();
        HashSet<long> removeIds = new();

        foreach (string alias in aliasSet)
        {
            if (!partLookup.TryGetValue(alias, out IReadOnlyList<BodypartProto>? partsForAlias))
            {
                continue;
            }

            foreach (BodypartProto part in partsForAlias)
            {
                foreach (long id in ExpandSubtree(part.Id, upstreams))
                {
                    removeIds.Add(id);
                }
            }
        }

        if (!removeIds.Any())
        {
            return;
        }

        List<Limb> limbs = context.Limbs.Where(x => x.RootBodyId == body.Id).ToList();
        foreach (Limb? limb in limbs.Where(x => removeIds.Contains(x.RootBodypartId)).ToList())
        {
            context.Limbs.Remove(limb);
        }

        foreach (LimbBodypartProto? limbPart in context.LimbsBodypartProto.Where(x => removeIds.Contains(x.BodypartProtoId)).ToList())
        {
            context.LimbsBodypartProto.Remove(limbPart);
        }

        foreach (LimbsSpinalPart? spinalPart in context.LimbsSpinalParts.Where(x => removeIds.Contains(x.BodypartProtoId)).ToList())
        {
            context.LimbsSpinalParts.Remove(spinalPart);
        }

        foreach (BodypartInternalInfos? internalInfo in context.BodypartInternalInfos
                     .Where(x => removeIds.Contains(x.BodypartProtoId) || removeIds.Contains(x.InternalPartId))
                     .ToList())
        {
            context.BodypartInternalInfos.Remove(internalInfo);
        }

        foreach (BoneOrganCoverage? coverage in context.BoneOrganCoverages
                     .Where(x => removeIds.Contains(x.BoneId) || removeIds.Contains(x.OrganId))
                     .ToList())
        {
            context.BoneOrganCoverages.Remove(coverage);
        }

        foreach (BodypartProtoBodypartProtoUpstream? upstream in context.BodypartProtoBodypartProtoUpstream
                     .Where(x => removeIds.Contains(x.Child) || removeIds.Contains(x.Parent))
                     .ToList())
        {
            context.BodypartProtoBodypartProtoUpstream.Remove(upstream);
        }

        foreach (BodypartGroupDescribersBodypartProtos? describerPart in context.BodypartGroupDescribersBodypartProtos
                     .Where(x => removeIds.Contains(x.BodypartProtoId))
                     .ToList())
        {
            context.BodypartGroupDescribersBodypartProtos.Remove(describerPart);
        }

        foreach (BodyProtosAdditionalBodyparts? additionalPart in context.BodyProtosAdditionalBodyparts
                     .Where(x => x.BodyProtoId == body.Id && removeIds.Contains(x.BodypartId))
                     .ToList())
        {
            context.BodyProtosAdditionalBodyparts.Remove(additionalPart);
        }

        foreach (BodypartProto? part in bodyParts.Where(x => removeIds.Contains(x.Id)).ToList())
        {
            context.BodypartProtos.Remove(part);
        }

        foreach (Limb? limb in context.Limbs.Where(x => x.RootBodyId == body.Id).ToList())
        {
            int limbBodypartCount = context.LimbsBodypartProto.Count(x => x.LimbId == limb.Id);
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
        List<BodyProto> bodies = new();
        HashSet<long> visited = new();
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
        foreach ((long sourcePartId, BodypartProto? clonedPart) in clonedParts)
        {
            if (!sourcePartLookup[sourcePartId].CountAsId.HasValue)
            {
                continue;
            }

            if (clonedParts.TryGetValue(sourcePartLookup[sourcePartId].CountAsId!.Value, out BodypartProto? mappedCountAs))
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
        HashSet<long> partIds = sourcePartIds.ToHashSet();
        foreach (BodypartProtoBodypartProtoUpstream? upstream in context.BodypartProtoBodypartProtoUpstream
                     .Where(x => partIds.Contains(x.Child) && partIds.Contains(x.Parent))
                     .ToList())
        {
            if (!clonedParts.TryGetValue(upstream.Child, out BodypartProto? child) ||
                !clonedParts.TryGetValue(upstream.Parent, out BodypartProto? parent))
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
        List<long> clonedPartIds = sourcePartIds.ToList();

        foreach (BodypartInternalInfos? internalInfo in context.BodypartInternalInfos
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

        foreach (BoneOrganCoverage? coverage in context.BoneOrganCoverages
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
        Dictionary<long, Limb> limbMap = new();
        foreach (Limb sourceLimb in (sourceLimbs ?? context.Limbs.Where(x => x.RootBodyId == source.Id).ToList()))
        {
            if (!clonedParts.TryGetValue(sourceLimb.RootBodypartId, out BodypartProto? clonedRoot))
            {
                continue;
            }

            Limb clonedLimb = new()
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

        foreach (long sourceLimb in limbMap.Keys)
        {
            foreach (LimbBodypartProto? bodypart in context.LimbsBodypartProto.Where(x => x.LimbId == sourceLimb).ToList())
            {
                if (!clonedParts.TryGetValue(bodypart.BodypartProtoId, out BodypartProto? clonedBodypart))
                {
                    continue;
                }

                context.LimbsBodypartProto.Add(new LimbBodypartProto
                {
                    Limb = limbMap[sourceLimb],
                    BodypartProto = clonedBodypart
                });
            }

            foreach (LimbsSpinalPart? spinalPart in context.LimbsSpinalParts.Where(x => x.LimbId == sourceLimb).ToList())
            {
                if (!clonedParts.TryGetValue(spinalPart.BodypartProtoId, out BodypartProto? clonedSpinalPart))
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
        List<long> sourceBodyIds = bodyIds.ToList();
        Dictionary<long, int> bodyOrder = sourceBodyIds
            .Select((id, index) => (id, index))
            .ToDictionary(x => x.id, x => x.index);
        Dictionary<long, Limb> limbMap = new();

        foreach (Limb? sourceLimb in context.Limbs
                     .Where(x => sourceBodyIds.Contains(x.RootBodyId))
                     .AsEnumerable()
                     .OrderBy(x => bodyOrder[x.RootBodyId])
                     .ThenBy(x => x.Id)
                     .ToList())
        {
            if (!clonedParts.TryGetValue(sourceLimb.RootBodypartId, out BodypartProto? clonedRoot))
            {
                continue;
            }

            Limb clonedLimb = new()
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

        foreach (long sourceLimbId in limbMap.Keys)
        {
            foreach (LimbBodypartProto? bodypart in context.LimbsBodypartProto.Where(x => x.LimbId == sourceLimbId).ToList())
            {
                if (!clonedParts.TryGetValue(bodypart.BodypartProtoId, out BodypartProto? clonedBodypart))
                {
                    continue;
                }

                context.LimbsBodypartProto.Add(new LimbBodypartProto
                {
                    Limb = limbMap[sourceLimbId],
                    BodypartProto = clonedBodypart
                });
            }

            foreach (LimbsSpinalPart? spinalPart in context.LimbsSpinalParts.Where(x => x.LimbId == sourceLimbId).ToList())
            {
                if (!clonedParts.TryGetValue(spinalPart.BodypartProtoId, out BodypartProto? clonedSpinalPart))
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
        HashSet<long> inheritedBodyIds = new();
        BodyProto? currentBody = source;
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

        List<Limb> inheritedLimbs = context.Limbs
            .Where(x => inheritedBodyIds.Contains(x.RootBodyId))
            .ToList();
        if (!inheritedLimbs.Any())
        {
            return;
        }

        List<long> inheritedLimbIds = inheritedLimbs.Select(x => x.Id).ToList();
        List<long> sourcePartIds = clonedParts.Keys.ToList();
        HashSet<(long LimbId, long BodypartProtoId)> existingBodypartLinks = context.LimbsBodypartProto
            .Where(x => inheritedLimbIds.Contains(x.LimbId))
            .ToList()
            .Select(x => (x.LimbId, x.BodypartProtoId))
            .ToHashSet();
        foreach (LimbBodypartProto? bodypartLink in context.LimbsBodypartProto
                     .Where(x => inheritedLimbIds.Contains(x.LimbId) && sourcePartIds.Contains(x.BodypartProtoId))
                     .ToList())
        {
            BodypartProto clonedBodypart = clonedParts[bodypartLink.BodypartProtoId];
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

        HashSet<(long LimbId, long BodypartProtoId)> existingSpinalLinks = context.LimbsSpinalParts
            .Where(x => inheritedLimbIds.Contains(x.LimbId))
            .ToList()
            .Select(x => (x.LimbId, x.BodypartProtoId))
            .ToHashSet();
        foreach (LimbsSpinalPart? spinalLink in context.LimbsSpinalParts
                     .Where(x => inheritedLimbIds.Contains(x.LimbId) && sourcePartIds.Contains(x.BodypartProtoId))
                     .ToList())
        {
            BodypartProto clonedSpinalPart = clonedParts[spinalLink.BodypartProtoId];
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
        foreach (BodypartGroupDescriber? sourceDescriber in context.BodypartGroupDescribers
                     .Where(x => x.BodypartGroupDescribersBodyProtos.Any(y => y.BodyProtoId == source.Id))
                     .ToList())
        {
            BodypartGroupDescriber clonedDescriber = new()
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

            foreach (BodypartGroupDescribersShapeCount? shapeCount in context.BodypartGroupDescribersShapeCount
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

            foreach (BodypartGroupDescribersBodypartProtos? bodypart in context.BodypartGroupDescribersBodypartProtos
                         .Where(x => x.BodypartGroupDescriberId == sourceDescriber.Id)
                         .ToList())
            {
                if (!clonedParts.TryGetValue(bodypart.BodypartProtoId, out BodypartProto? clonedBodypart))
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
        HashSet<long> result = new()
        { rootId };
        Queue<long> queue = new();
        queue.Enqueue(rootId);

        Dictionary<long, List<long>> parentLookup = upstreams
            .GroupBy(x => x.Parent)
            .ToDictionary(x => x.Key, x => x.Select(y => y.Child).ToList());

        while (queue.Count > 0)
        {
            long current = queue.Dequeue();
            if (!parentLookup.TryGetValue(current, out List<long>? children))
            {
                continue;
            }

            foreach (long child in children)
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
        BodypartProto part = new()
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

        if (parentAlias is not null && _parts.TryGetValue(parentAlias, out BodypartProto? parent))
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
        Limb limb = new()
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

        foreach (string? alias in partAliases.Where(_parts.ContainsKey))
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
