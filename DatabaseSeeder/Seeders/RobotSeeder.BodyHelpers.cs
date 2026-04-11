#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private static int NormalizeRobotSeverThreshold(string alias, int severedThreshold, SizeCategory size,
		BodypartTypeEnum type)
	{
		if (severedThreshold <= 0)
		{
			return severedThreshold;
		}

		if (type == BodypartTypeEnum.Eye ||
		    alias.Contains("sensor", StringComparison.OrdinalIgnoreCase) ||
		    alias.Contains("antenna", StringComparison.OrdinalIgnoreCase) ||
		    alias.Contains("mandible", StringComparison.OrdinalIgnoreCase))
		{
			return Math.Min(severedThreshold, 18);
		}

		return size switch
		{
			SizeCategory.Tiny => Math.Min(severedThreshold, 12),
			SizeCategory.VerySmall => Math.Min(severedThreshold, 18),
			_ => Math.Min(severedThreshold, 27)
		};
	}

	private BodypartProto AddRobotOrgan(
        BodyProto body,
        string alias,
        string description,
        string shapeName,
        BodypartTypeEnum type,
        string parentAlias,
        Alignment alignment,
        Orientation orientation,
        SizeCategory size,
        bool isVital,
        bool isCore,
        IEnumerable<string> internalLocations)
    {
        BodypartProto organ = AddBodypart(body, alias, description, shapeName, type, parentAlias, alignment, orientation, 0, 60, 75,
            2000 + _context.BodypartProtos.Count(x => x.BodyId == body.Id), _circuitryMaterial, _robotInternalArmour, size,
            isVital: isVital, isCore: isCore, isOrgan: true, bleedModifier: 0.0, painModifier: 0.0, stunModifier: 1.0, damageModifier: 1.0);

        foreach (string locationAlias in internalLocations)
        {
            BodypartProto? location = FindBodypartOnBody(body, locationAlias);
            if (location is null)
            {
                continue;
            }

            if (_context.BodypartInternalInfos.Any(x => x.BodypartProtoId == location.Id && x.InternalPartId == organ.Id))
            {
                continue;
            }

            _context.BodypartInternalInfos.Add(new BodypartInternalInfos
            {
                BodypartProto = location,
                InternalPart = organ,
                HitChance = 1.0,
                IsPrimaryOrganLocation = true,
                ProximityGroup = string.Empty
            });
        }

        _context.SaveChanges();
        return organ;
    }

    private BodypartProto AddBodypart(
        BodyProto body,
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
        Material material,
        ArmourType armour,
        SizeCategory size,
        bool significant = true,
        bool isVital = false,
        bool isCore = false,
        bool isOrgan = false,
        double bleedModifier = 0.0,
        double painModifier = 0.0,
        double damageModifier = 1.0,
        double stunModifier = 1.0,
        BodypartProto? countAs = null)
    {
        BodypartProto part = new()
        {
            Body = body,
            Name = alias,
            Description = description,
			BodypartShape = _context.BodypartShapes.First(x => x.Name == shapeName),
			BodypartType = (int)type,
			Alignment = (int)alignment,
			Location = (int)orientation,
			RelativeHitChance = relativeHitChance,
			SeveredThreshold = NormalizeRobotSeverThreshold(alias, severedThreshold, size, type),
			MaxLife = maxLife,
            DisplayOrder = displayOrder,
            DefaultMaterial = material,
            Size = (int)size,
            Significant = significant,
            IsVital = isVital,
            IsCore = isCore,
            IsOrgan = isOrgan ? 1 : 0,
            ImplantSpace = isOrgan ? 0.0 : 1.0,
            ImplantSpaceOccupied = 0.0,
            BleedModifier = bleedModifier,
            PainModifier = painModifier,
            DamageModifier = damageModifier,
            StunModifier = stunModifier,
            RelativeInfectability = 0.0,
            ArmourType = armour,
            CountAs = countAs
        };
        _context.BodypartProtos.Add(part);
        _context.SaveChanges();

        if (!string.IsNullOrWhiteSpace(parentAlias) && FindBodypartOnBody(body, parentAlias) is { } parent)
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

    private void AddMissingBodyMovement(BodyProto source, BodyProto target)
    {
        foreach (BodyProtosPositions? position in _context.BodyProtosPositions.Where(x => x.BodyProtoId == source.Id).ToList())
        {
            if (_context.BodyProtosPositions.Any(x => x.BodyProtoId == target.Id && x.Position == position.Position))
            {
                continue;
            }

            _context.BodyProtosPositions.Add(new BodyProtosPositions
            {
                BodyProto = target,
                Position = position.Position
            });
        }

        long nextId = _context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
        foreach (MoveSpeed? speed in _context.MoveSpeeds.Where(x => x.BodyProtoId == source.Id).OrderBy(x => x.Id).ToList())
        {
            if (_context.MoveSpeeds.Any(x => x.BodyProtoId == target.Id && x.Alias == speed.Alias))
            {
                continue;
            }

            _context.MoveSpeeds.Add(new MoveSpeed
            {
                Id = nextId++,
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

    private void AddLimb(BodyProto body, string name, LimbType limbType, BodypartProto root, IEnumerable<BodypartProto> parts)
    {
        Limb limb = new()
        {
            Name = name,
            LimbType = (int)limbType,
            RootBody = body,
            RootBodypart = root,
            LimbDamageThresholdMultiplier = 0.5,
            LimbPainThresholdMultiplier = 0.0
        };
        _context.Limbs.Add(limb);
        _context.SaveChanges();

        foreach (BodypartProto part in parts)
        {
            _context.LimbsBodypartProto.Add(new LimbBodypartProto
            {
                Limb = limb,
                BodypartProto = part
            });
        }

        _context.SaveChanges();
    }

    private void AddLimbPart(BodyProto body, string limbRootAlias, string partAlias)
    {
        BodypartProto? root = FindBodypartOnBody(body, limbRootAlias);
        BodypartProto? part = FindBodypartOnBody(body, partAlias);
        if (root is null || part is null)
        {
            return;
        }

        List<long> bodyIds = SeederBodyUtilities.GetBodyAndAncestorIds(_context, body);
        Limb? limb = _context.Limbs.FirstOrDefault(x => bodyIds.Contains(x.RootBodyId) && x.RootBodypartId == root.Id);
        if (limb is null)
        {
            return;
        }

        if (_context.LimbsBodypartProto.Any(x => x.LimbId == limb.Id && x.BodypartProtoId == part.Id))
        {
            return;
        }

        _context.LimbsBodypartProto.Add(new LimbBodypartProto
        {
            Limb = limb,
            BodypartProto = part
        });
        _context.SaveChanges();
    }

    private void EnsureDefaultSmashingBodypart(BodyProto body, string alias)
    {
        if (FindBodypartOnBody(body, alias) is not { } part)
        {
            return;
        }

        body.DefaultSmashingBodypart = part;
        _context.SaveChanges();
    }

    private BodypartProto? FindBodypartOnBody(BodyProto body, string alias)
    {
        return SeederBodyUtilities.FindBodypartOnBodyOrAncestors(_context, body, alias);
    }

    private void AddBodypartRemoval(BodyProto body, params string[] aliases)
    {
        bool dirty = false;
        foreach (string? alias in aliases.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            BodypartProto? bodypart = FindBodypartOnBody(body, alias);
            if (bodypart is null)
            {
                continue;
            }

            if (_context.BodyProtosAdditionalBodyparts.Any(x =>
                    x.BodyProtoId == body.Id &&
                    x.BodypartId == bodypart.Id &&
                    x.Usage == "remove"))
            {
                continue;
            }

            _context.BodyProtosAdditionalBodyparts.Add(new BodyProtosAdditionalBodyparts
            {
                BodyProto = body,
                Bodypart = bodypart,
                Usage = "remove"
            });
            dirty = true;
        }

        if (dirty)
        {
            _context.SaveChanges();
        }
    }
}
