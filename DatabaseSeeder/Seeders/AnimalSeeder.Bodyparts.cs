#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    #region Bodyparts
    private void AddShape(string name)
    {
        _context.BodypartShapes.Add(new BodypartShape
        {
            Name = name
        });
    }

    private void AddBodypartUsage(string bodypart, string usage, BodyProto body)
    {
        _context.BodyProtosAdditionalBodyparts.Add(new BodyProtosAdditionalBodyparts
        {
            BodyProto = body,
            Usage = usage,
            Bodypart = _cachedBodyparts[bodypart]
        });
    }

    private void AddRacialBodypartUsage(string bodypart, string usage, Race race)
    {
        BodypartProto? part = _cachedBodyparts.TryGetValue(bodypart, out BodypartProto? cachedPart)
            ? cachedPart
            : SeederBodyUtilities.FindBodypartOnBodyOrAncestors(_context, race.BaseBody, bodypart);
        if (part is null)
        {
            return;
        }

        if (_context.RacesAdditionalBodyparts.Any(x =>
                x.RaceId == race.Id &&
                x.BodypartId == part.Id &&
                x.Usage == usage))
        {
            return;
        }

        _context.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
        {
            Race = race,
            Usage = usage,
            Bodypart = part
        });
    }

    private static int GetAnimalRelativeHitChance(BodyProto body, string alias, int fallback)
    {
        return body.Name switch
        {
            "Quadruped Base" => GetQuadrupedRelativeHitChance(alias, fallback),
            "Ungulate" => GetUngulateRelativeHitChance(alias, fallback),
            "Toed Quadruped" => GetToedQuadrupedRelativeHitChance(alias, fallback),
            "Pinniped" => GetPinnipedRelativeHitChance(alias, fallback),
            "Avian" => GetAvianRelativeHitChance(alias, fallback),
            "Vermiform" or "Serpentine" => GetSerpentRelativeHitChance(alias, fallback),
            "Piscine" => GetFishRelativeHitChance(alias, fallback),
            "Cetacean" => GetCetaceanRelativeHitChance(alias, fallback),
            "Cephalopod" => GetCephalopodRelativeHitChance(alias, fallback),
            "Jellyfish" => GetJellyfishRelativeHitChance(alias, fallback),
            "Insectoid" or "Winged Insectoid" => GetInsectoidRelativeHitChance(alias, fallback),
            "Beetle" => GetInsectoidRelativeHitChance(alias, fallback),
            "Centipede" => GetCentipedeRelativeHitChance(alias, fallback),
            _ => fallback
        };
    }

    private static int GetQuadrupedRelativeHitChance(string alias, int fallback)
    {
        return alias switch
        {
            "abdomen" => 90,
            "rbreast" or "lbreast" => 60,
            "urflank" or "ulflank" or "lrflank" or "llflank" => 100,
            "belly" => 80,
            "rshoulder" or "lshoulder" => 35,
            "uback" or "lback" => 100,
            "withers" => 20,
            "rrump" or "lrump" => 40,
            "rloin" or "lloin" => 35,
            "neck" => 30,
            "bneck" => 18,
            "throat" => 10,
            "head" => 45,
            "bhead" => 25,
            "rjaw" or "ljaw" => 12,
            "rcheek" or "lcheek" => 10,
            "reyesocket" or "leyesocket" => 5,
            "reye" or "leye" => 3,
            "rear" or "lear" => 4,
            "muzzle" => 15,
            "mouth" => 10,
            "tongue" => 1,
            "nose" => 5,
            "ruforeleg" or "luforeleg" or "ruhindleg" or "luhindleg" => 50,
            "rfknee" or "lfknee" or "rrknee" or "rlknee" => 12,
            "rlforeleg" or "llforeleg" or "rlhindleg" or "llhindleg" => 35,
            "rfhock" or "lfhock" or "rrhock" or "lrhock" => 8,
            "utail" or "mtail" or "ltail" => 20,
            "groin" => 8,
            "testicles" or "penis" => 1,
            "rwingbase" or "lwingbase" => 15,
            "rwing" or "lwing" => 60,
            "udder" => 10,
            "rhorn" or "lhorn" or "horn" or "rantler" or "lantler" or "rtusk" or "ltusk" => 2,
            _ => fallback
        };
    }

    private static int GetUngulateRelativeHitChance(string alias, int fallback)
    {
        return alias switch
        {
            "rfhoof" or "lfhoof" or "rrhoof" or "lrhoof" => 15,
            "rffrog" or "lffrog" or "rrfrog" or "lrfrog" => 1,
            _ => GetQuadrupedRelativeHitChance(alias, fallback)
        };
    }

    private static int GetToedQuadrupedRelativeHitChance(string alias, int fallback)
    {
        return alias switch
        {
            "rfpaw" or "lfpaw" or "rrpaw" or "lrpaw" => 15,
            "rfclaw" or "lfclaw" or "rrclaw" or "lrclaw" => 3,
            "rrdewclaw" or "lrdewclaw" => 1,
            _ => GetQuadrupedRelativeHitChance(alias, fallback)
        };
    }

    private static int GetCentipedeRelativeHitChance(string alias, int fallback)
    {
        return alias switch
        {
            "thorax" => 70,
            "midbody" => 80,
            "hindbody" => 70,
            "tail" => 18,
            "head" => 28,
            "mandibles" => 8,
            "reye" or "leye" => 3,
            "rantenna" or "lantenna" => 2,
            _ when alias.StartsWith("rleg", System.StringComparison.OrdinalIgnoreCase) ||
                   alias.StartsWith("lleg", System.StringComparison.OrdinalIgnoreCase) => 14,
            _ => GetInsectoidRelativeHitChance(alias, fallback)
        };
    }

    private static int GetPinnipedRelativeHitChance(string alias, int fallback)
    {
        return alias switch
        {
            "rfrontflipper" or "lfrontflipper" or "rhindflipper" or "lhindflipper" => 50,
            "tail" => 15,
            "rtusk" or "ltusk" => 2,
            _ => GetQuadrupedRelativeHitChance(alias, fallback)
        };
    }

    private static int GetAvianRelativeHitChance(string alias, int fallback)
    {
        return alias switch
        {
            "abdomen" => 65,
            "rbreast" or "lbreast" => 55,
            "urflank" or "ulflank" or "lrflank" or "llflank" => 45,
            "belly" => 55,
            "rshoulder" or "lshoulder" => 20,
            "uback" or "lback" => 55,
            "rump" => 30,
            "loin" => 25,
            "neck" => 22,
            "bneck" => 12,
            "throat" => 8,
            "head" => 28,
            "bhead" => 18,
            "rcheek" or "lcheek" => 6,
            "reyesocket" or "leyesocket" => 4,
            "reye" or "leye" => 2,
            "rear" or "lear" => 2,
            "beak" => 10,
            "tongue" => 1,
            "nose" => 3,
            "rupperleg" or "lupperleg" => 20,
            "rknee" or "lknee" => 5,
            "rlowerleg" or "llowerleg" => 16,
            "rankle" or "lankle" => 4,
            "rfoot" or "lfoot" => 10,
            "rtalons" or "ltalons" => 3,
            "tail" => 18,
            "groin" => 4,
            "rwingbase" or "lwingbase" => 10,
            "rwing" or "lwing" => 65,
            _ => fallback
        };
    }

    private static int GetSerpentRelativeHitChance(string alias, int fallback)
    {
        return alias switch
        {
            "head" => 25,
            "mouth" => 8,
            "fangs" => 1,
            "reyesocket" or "leyesocket" => 3,
            "reye" or "leye" => 2,
            "tongue" => 1,
            "neck" => 12,
            "ubody" => 80,
            "mbody" => 100,
            "lbody" => 70,
            "tail" => 35,
            _ => fallback
        };
    }

    private static int GetFishRelativeHitChance(string alias, int fallback)
    {
        return alias switch
        {
            "abdomen" => 80,
            "rbreast" or "lbreast" => 50,
            "urflank" or "ulflank" or "lrflank" or "llflank" => 90,
            "belly" => 65,
            "uback" or "lback" => 80,
            "loin" => 55,
            "dorsalfin" or "analfin" or "rpectoralfin" or "lpectoralfin" or "rpelvicfin" or "lpelvicfin" => 12,
            "neck" => 12,
            "rgill" or "lgill" => 10,
            "head" => 35,
            "reyesocket" or "leyesocket" => 4,
            "reye" or "leye" => 2,
            "mouth" => 12,
            "peduncle" => 30,
            "caudalfin" => 20,
            _ => fallback
        };
    }

    private static int GetCetaceanRelativeHitChance(string alias, int fallback)
    {
        return alias switch
        {
            "abdomen" => 90,
            "rbreast" or "lbreast" => 55,
            "urflank" or "ulflank" or "lrflank" or "llflank" => 100,
            "belly" => 70,
            "uback" or "lback" => 90,
            "loin" => 60,
            "dorsalfin" => 10,
            "rpectoralfin" or "lpectoralfin" => 18,
            "neck" => 10,
            "blowhole" => 4,
            "head" => 40,
            "reyesocket" or "leyesocket" => 4,
            "reye" or "leye" => 2,
            "mouth" => 14,
            "stock" => 25,
            "fluke" => 30,
            _ => fallback
        };
    }

    private static int GetCephalopodRelativeHitChance(string alias, int fallback)
    {
        if (alias.StartsWith("arm", StringComparison.OrdinalIgnoreCase))
        {
            return 18;
        }

        if (alias.StartsWith("tentacle", StringComparison.OrdinalIgnoreCase))
        {
            return 20;
        }

        return alias switch
        {
            "abdomen" => 70,
            "mouth" => 8,
            "head" => 25,
            "mantle" => 55,
            "reye" or "leye" => 3,
            _ => fallback
        };
    }

    private static int GetJellyfishRelativeHitChance(string alias, int fallback)
    {
        if (alias.StartsWith("tendril", StringComparison.OrdinalIgnoreCase))
        {
            return 6;
        }

        return alias switch
        {
            "body" => 50,
            _ => fallback
        };
    }

    private static int GetInsectoidRelativeHitChance(string alias, int fallback)
    {
        if (alias.StartsWith("rleg", StringComparison.OrdinalIgnoreCase) ||
            alias.StartsWith("lleg", StringComparison.OrdinalIgnoreCase))
        {
            return 5;
        }

        return alias switch
        {
            "thorax" => 35,
            "head" => 18,
            "abdomen" => 30,
            "rantenna" or "lantenna" => 1,
            "mandibles" => 2,
            "reye" or "leye" => 1,
            "rwingbase" or "lwingbase" => 3,
            "rwing" or "lwing" => 10,
            _ => fallback
        };
    }

    private void AddBodypart(BodyProto body, string alias, string name, string shape,
        BodypartTypeEnum type, string? upstreamPartName, Alignment alignment,
        Orientation orientation, int hitPoints, int severThreshold, int hitChance, int displayOrder,
        string material, SizeCategory size, string limb, bool isSignificant = true, double infectability = 1.0,
        bool isVital = false, double hypoxia = 0.0, double implantSpace = 0, double implantSpaceOccupied = 0,
        bool isCore = true, double bleedMultiplier = 1.0, double damageMultiplier = 1.0,
        double painMultiplier = 1.0, double stunMultiplier = 0.0)
    {
        BodypartProto bodypart = new()
        {
            BodypartShape = _cachedShapes[shape],
            Body = body,
            Name = alias,
            Description = name,
            BodypartType = (int)type,
            Alignment = (int)alignment,
            Location = (int)orientation,
            BleedModifier = bleedMultiplier,
            DamageModifier = damageMultiplier,
			PainModifier = painMultiplier,
			StunModifier = stunMultiplier,
			MaxLife = ResolveAnimalBodypartLife(alias, size, hitPoints),
			SeveredThreshold = _sever ? NormalizeAnimalSeverThreshold(alias, severThreshold, size) : -1,
			SeverFormula = ResolveAnimalSeverFormula(alias, size),
			IsCore = isCore,
			IsVital = isVital,
			Significant = isSignificant,
            RelativeInfectability = infectability,
            HypoxiaDamagePerTick = hypoxia,
            ImplantSpace = implantSpace,
            ImplantSpaceOccupied = implantSpaceOccupied,
            Size = (int)size,
            DisplayOrder = displayOrder,
            RelativeHitChance = ResolveAnimalRelativeHitChance(body, alias, type, size, hitChance),
            DefaultMaterial = _cachedMaterials[material],
            ArmourType = _naturalArmour
        };

        switch (type)
        {
            case BodypartTypeEnum.Grabbing:
                bodypart.Unary = false;
                break;
            case BodypartTypeEnum.Wielding:
                bodypart.Unary = true;
                bodypart.MaxSingleSize = (int)SizeCategory.Normal;
                break;
            case BodypartTypeEnum.GrabbingWielding:
                bodypart.Unary = true;
                bodypart.MaxSingleSize = (int)SizeCategory.Normal;
                break;
        }

        _context.BodypartProtos.Add(bodypart);
        _cachedBodyparts[alias] = bodypart;
        _cachedLimbs.Add(limb, bodypart);
        if (!string.IsNullOrEmpty(upstreamPartName))
        {
            _cachedBodypartUpstreams.Add((bodypart, _cachedBodyparts[upstreamPartName]));
        }
    }

    private void AddOrgan(BodyProto body, string alias, string description, BodypartTypeEnum type,
        double implantSpaceOccupied, int hitPoints, double bleedModifier, double infectionModifier,
        double hypoxiaDamage, double damageModifier = 1.0, double stunModifier = 0.0, double painModifier = 1.0)
    {
        BodypartProto organ = new()
        {
            Name = alias,
            Description = description,
            Body = body,
            BodypartType = (int)type,
            IsCore = true,
            IsOrgan = 1,
            IsVital = true,
            MaxLife = hitPoints,
            SeveredThreshold = -1,
            DisplayOrder = 1,
            BleedModifier = bleedModifier,
            DamageModifier = damageModifier,
            PainModifier = painModifier,
            StunModifier = stunModifier,
            HypoxiaDamagePerTick = hypoxiaDamage,
            RelativeInfectability = infectionModifier,
            Size = (int)SizeCategory.Small,
            Location = (int)Orientation.Irrelevant,
            Alignment = (int)Alignment.Irrelevant,
            BodypartShape = _cachedShapes["organ"],
            RelativeHitChance = 0,
            DefaultMaterial = _cachedMaterials["viscera"],
            ImplantSpaceOccupied = implantSpaceOccupied,
            ArmourType = _organArmour
        };
        _context.BodypartProtos.Add(organ);
        _cachedOrgans[alias] = organ;
    }

    private void AddOrganCoverage(string whichOrgan, string whichBodypart, int hitChance, bool isPrimary = false)
    {
        _context.BodypartInternalInfos.Add(new BodypartInternalInfos
        {
            BodypartProto = _cachedBodyparts[whichBodypart],
            InternalPart = _cachedOrgans[whichOrgan],
            HitChance = hitChance,
            IsPrimaryOrganLocation = isPrimary
        });
    }

    private void AddBone(BodyProto body, string alias, string description, BodypartTypeEnum type, int hitPoints,
        string material, SizeCategory size = SizeCategory.Small)
    {
        BodypartProto bone = new()
        {
            Name = alias,
            Body = body,
            Description = description,
            BodypartType = (int)type,
            MaxLife = hitPoints,
            DefaultMaterial = _cachedMaterials[material],
            Size = (int)size,
            RelativeHitChance = 0,
            StunModifier = 0,
            Location = (int)Orientation.Irrelevant,
            Alignment = (int)Alignment.Irrelevant,
            BodypartShape = _cachedShapes["bone"],
            HypoxiaDamagePerTick = 0,
            BleedModifier = 0,
            DamageModifier = 1.0,
            PainModifier = 1.0,
            RelativeInfectability = 0.0,
            DisplayOrder = 1,
            SeveredThreshold = -1,
            IsCore = true,
            IsOrgan = 0,
            IsVital = false,
            ArmourType = _boneArmour
        };
        _context.BodypartProtos.Add(bone);
        _cachedBones[alias] = bone;
    }

    private void AddBoneInternal(string whichBone, string whichBodypart, int hitChance, bool isPrimary = true)
    {
        _context.BodypartInternalInfos.Add(new BodypartInternalInfos
        {
            BodypartProto = _cachedBodyparts[whichBodypart],
            InternalPart = _cachedBones[whichBone],
            HitChance = hitChance,
            IsPrimaryOrganLocation = isPrimary
        });
    }

    private void AddBoneCover(string bone, string organ, double coverage)
    {
        _context.BoneOrganCoverages.Add(new BoneOrganCoverage
        {
            Bone = _cachedBones[bone],
            Organ = _cachedOrgans[organ],
            CoverageChance = coverage
        });
    }

    private void AddBodypartGroupDescriberShape(BodyProto body, string describedAs, string comment,
        params (string Shape, int MinCount, int MaxCount)[] includedShapes)
    {
        BodypartGroupDescriber describer = new()
        {
            DescribedAs = describedAs,
            Comment = comment,
            Type = "shape"
        };
        _context.BodypartGroupDescribers.Add(describer);
        foreach ((string? shape, int minCount, int maxCount) in includedShapes)
        {
            describer.BodypartGroupDescribersShapeCount.Add(new BodypartGroupDescribersShapeCount
            {
                Target = _cachedShapes[shape],
                BodypartGroupDescriptionRule = describer,
                MinCount = minCount,
                MaxCount = maxCount
            });
        }

        describer.BodypartGroupDescribersBodyProtos.Add(new BodypartGroupDescribersBodyProtos
        { BodypartGroupDescriber = describer, BodyProto = body });
    }

    private void AddBodypartGroupDescriberDirect(BodyProto body, string describedAs, string comment,
        params (string Part, bool Mandatory)[] includedParts)
    {
        BodypartGroupDescriber describer = new()
        {
            DescribedAs = describedAs,
            Comment = comment,
            Type = "bodypart"
        };
        _context.BodypartGroupDescribers.Add(describer);
        foreach ((string? part, bool mandatory) in includedParts)
        {
            describer.BodypartGroupDescribersBodypartProtos.Add(new BodypartGroupDescribersBodypartProtos
            {
                BodypartGroupDescriber = describer,
                BodypartProto = _cachedBodyparts[part],
                Mandatory = mandatory
            });
        }

        describer.BodypartGroupDescribersBodyProtos.Add(new BodypartGroupDescribersBodyProtos
        { BodypartGroupDescriber = describer, BodyProto = body });
    }

    #endregion
}
