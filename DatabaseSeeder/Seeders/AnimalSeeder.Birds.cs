using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    internal static string? GetAvianSpinalOrganAliasForLimb(string limbName)
    {
        return limbName switch
        {
            "Torso" => "uspinalcord",
            "Genitals" => "mspinalcord",
            "Right Wing" => "mspinalcord",
            "Left Wing" => "mspinalcord",
            "Right Leg" => "lspinalcord",
            "Left Leg" => "lspinalcord",
            "Tail" => "lspinalcord",
            _ => null
        };
    }

    private void SeedAvian(BodyProto avianProto)
    {
        ResetCachedParts();
        int order = 1;
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodyparts...");

        #region Torso

        AddBodypart(avianProto, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null, Alignment.Front,
            Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "rbreast", "right breast", "breast", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.FrontRight, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "lbreast", "left breast", "breast", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.FrontLeft, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "urflank", "upper right flank", "flank", BodypartTypeEnum.BonyDrapeable, "rbreast",
            Alignment.Right, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "ulflank", "upper left flank", "flank", BodypartTypeEnum.BonyDrapeable, "lbreast",
            Alignment.Left, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "lrflank", "lower right flank", "flank", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.RearRight, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "llflank", "lower left flank", "flank", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.RearLeft, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "belly", "belly", "belly", BodypartTypeEnum.Wear, "abdomen", Alignment.Front,
            Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "rshoulder", "right shoulder", "shoulder", BodypartTypeEnum.BonyDrapeable, "rbreast",
            Alignment.FrontRight, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "lshoulder", "left shoulder", "shoulder", BodypartTypeEnum.BonyDrapeable, "lbreast",
            Alignment.FrontLeft, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "uback", "upper back", "upper back", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.Front, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "lback", "lower back", "lower back", BodypartTypeEnum.BonyDrapeable, "abdomen",
            Alignment.Rear, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "rump", "rump", "rump", BodypartTypeEnum.Wear, "lback", Alignment.Rear,
            Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso", true,
            isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "loin", "loin", "loin", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
            Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.2);

        #endregion

        #region Head

        AddBodypart(avianProto, "neck", "neck", "neck", BodypartTypeEnum.BonyDrapeable, "uback", Alignment.Front,
            Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(avianProto, "bneck", "neck back", "neck back", BodypartTypeEnum.BonyDrapeable, "neck",
            Alignment.Rear, Orientation.Highest, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(avianProto, "throat", "throat", "throat", BodypartTypeEnum.Wear, "neck", Alignment.Front,
            Orientation.Highest, 40, 50, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(avianProto, "head", "head", "face", BodypartTypeEnum.BonyDrapeable, "neck", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(avianProto, "bhead", "head back", "head back", BodypartTypeEnum.BonyDrapeable, "bneck",
            Alignment.Rear, Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(avianProto, "rcheek", "right cheek", "cheek", BodypartTypeEnum.BonyDrapeable, "head",
            Alignment.Right, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(avianProto, "lcheek", "left cheek", "cheek", BodypartTypeEnum.BonyDrapeable, "head", Alignment.Left,
            Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(avianProto, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.BonyDrapeable, "head",
            Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(avianProto, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.BonyDrapeable, "head",
            Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(avianProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket",
            Alignment.FrontRight, Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(avianProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket", Alignment.FrontLeft,
            Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(avianProto, "rear", "right ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Right,
            Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "lear", "left ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Left,
            Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(avianProto, "beak", "beak", "beak", BodypartTypeEnum.Mouth, "head", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(avianProto, "tongue", "tongue", "tongue", BodypartTypeEnum.Tongue, "beak", Alignment.Front,
            Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(avianProto, "nose", "nose", "nose", BodypartTypeEnum.BonyDrapeable, "head", Alignment.Front,
            Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);

        #endregion

        #region Legs

        AddBodypart(avianProto, "rupperleg", "right upper leg", "upper leg", BodypartTypeEnum.BonyDrapeable,
            "loin", Alignment.RearRight, Orientation.Low, 80, 100, 100, order++, "Bony Flesh",
            SizeCategory.Normal, "Right Leg");
        AddBodypart(avianProto, "lupperleg", "left upper leg", "upper leg", BodypartTypeEnum.BonyDrapeable, "loin",
            Alignment.RearLeft, Orientation.Low, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Left Leg");
        AddBodypart(avianProto, "rknee", "right knee", "knee", BodypartTypeEnum.BonyDrapeable, "rupperleg",
            Alignment.RearRight, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Right Leg");
        AddBodypart(avianProto, "lknee", "left knee", "knee", BodypartTypeEnum.BonyDrapeable, "lupperleg",
            Alignment.RearLeft, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Left Leg");
        AddBodypart(avianProto, "rlowerleg", "right lower leg", "lower leg", BodypartTypeEnum.BonyDrapeable, "rknee",
            Alignment.RearRight, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Right Leg");
        AddBodypart(avianProto, "llowerleg", "left lower leg", "lower leg", BodypartTypeEnum.BonyDrapeable, "lknee",
            Alignment.RearLeft, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Left Leg");
        AddBodypart(avianProto, "rankle", "right ankle", "ankle", BodypartTypeEnum.BonyDrapeable, "rlowerleg",
            Alignment.RearRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Right Leg");
        AddBodypart(avianProto, "lankle", "left ankle", "ankle", BodypartTypeEnum.BonyDrapeable, "llowerleg",
            Alignment.RearLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Left Leg");
        AddBodypart(avianProto, "rfoot", "right foot", "foot", BodypartTypeEnum.Standing, "rankle",
            Alignment.RearRight, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
            "Right Leg");
        AddBodypart(avianProto, "lfoot", "left foot", "foot", BodypartTypeEnum.Standing, "lankle",
            Alignment.RearLeft, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
            "Left Leg");
        AddBodypart(avianProto, "rtalons", "right talons", "talon", BodypartTypeEnum.Wear, "rfoot",
            Alignment.RearRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Right Leg", false, isVital: false);
        AddBodypart(avianProto, "ltalons", "left talons", "talon", BodypartTypeEnum.Wear, "lfoot",
            Alignment.RearLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Left Leg", false, isVital: false);

        #endregion

        #region Tail

        AddBodypart(avianProto, "tail", "tail", "tail", BodypartTypeEnum.Wear, "uback", Alignment.Rear,
            Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");

        #endregion

        #region Genitals

        AddBodypart(avianProto, "groin", "groin", "groin", BodypartTypeEnum.Wear, "loin", Alignment.Rear,
            Orientation.Low, 30, -1, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals");

        #endregion

        #region Wings

        AddBodypart(avianProto, "rwingbase", "right wing base", "wing base", BodypartTypeEnum.BonyDrapeable, "uback",
            Alignment.FrontRight, Orientation.High, 40, -1, 100, order++, "Flesh", SizeCategory.Normal,
            "Right Wing", true, isCore: true);
        AddBodypart(avianProto, "lwingbase", "left wing base", "wing base", BodypartTypeEnum.BonyDrapeable, "uback",
            Alignment.FrontLeft, Orientation.High, 40, -1, 100, order++, "Flesh", SizeCategory.Normal, "Left Wing",
            true, isCore: true);
        AddBodypart(avianProto, "rwing", "right wing", "wing", BodypartTypeEnum.Wing, "rwingbase",
            Alignment.FrontRight, Orientation.High, 40, 50, 100, order++, "Flesh", SizeCategory.Normal,
            "Right Wing", true, isCore: true);
        AddBodypart(avianProto, "lwing", "left wing", "wing", BodypartTypeEnum.Wing, "lwingbase",
            Alignment.FrontLeft, Orientation.High, 40, 50, 100, order++, "Flesh", SizeCategory.Normal, "Left Wing",
            true, isCore: true);

        #endregion

        _context.SaveChanges();

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Organs...");

        #region Organs

        AddOrgan(avianProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, stunModifier: 1.0);
        AddOrgan(avianProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
        AddOrgan(avianProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(avianProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(avianProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(avianProto, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
            0.05);
        AddOrgan(avianProto, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
            0.05);
        AddOrgan(avianProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(avianProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(avianProto, "rlung", "right lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(avianProto, "llung", "left lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(avianProto, "trachea", "trachea", BodypartTypeEnum.Trachea, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(avianProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(avianProto, "uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(avianProto, "mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(avianProto, "lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(avianProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
        AddOrgan(avianProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

        AddOrganCoverage("brain", "head", 100, true);
        AddOrganCoverage("brain", "bhead", 100);
        AddOrganCoverage("brain", "rcheek", 85);
        AddOrganCoverage("brain", "lcheek", 85);
        AddOrganCoverage("brain", "reyesocket", 85);
        AddOrganCoverage("brain", "leyesocket", 85);
        AddOrganCoverage("brain", "reye", 85);
        AddOrganCoverage("brain", "leye", 85);
        AddOrganCoverage("brain", "beak", 10);
        AddOrganCoverage("brain", "lear", 10);
        AddOrganCoverage("brain", "rear", 10);

        AddOrganCoverage("linnerear", "lear", 33, true);
        AddOrganCoverage("rinnerear", "rear", 33, true);
        AddOrganCoverage("esophagus", "throat", 50, true);
        AddOrganCoverage("esophagus", "neck", 20);
        AddOrganCoverage("esophagus", "bneck", 5);
        AddOrganCoverage("trachea", "throat", 50, true);
        AddOrganCoverage("trachea", "neck", 20);
        AddOrganCoverage("trachea", "bneck", 5);

        AddOrganCoverage("rlung", "rbreast", 100, true);
        AddOrganCoverage("llung", "lbreast", 100, true);
        AddOrganCoverage("rlung", "uback", 15);
        AddOrganCoverage("llung", "uback", 15);
        AddOrganCoverage("rlung", "rshoulder", 66);
        AddOrganCoverage("llung", "lshoulder", 66);

        AddOrganCoverage("heart", "lbreast", 33, true);

        AddOrganCoverage("uspinalcord", "bneck", 10, true);
        AddOrganCoverage("uspinalcord", "neck", 2);
        AddOrganCoverage("uspinalcord", "throat", 5);
        AddOrganCoverage("mspinalcord", "uback", 10, true);
        AddOrganCoverage("lspinalcord", "lback", 10, true);

        AddOrganCoverage("liver", "abdomen", 33, true);
        AddOrganCoverage("spleen", "abdomen", 20, true);
        AddOrganCoverage("stomach", "abdomen", 20, true);
        AddOrganCoverage("liver", "uback", 15);
        AddOrganCoverage("spleen", "uback", 10);
        AddOrganCoverage("stomach", "uback", 5);

        AddOrganCoverage("lintestines", "belly", 5, true);
        AddOrganCoverage("sintestines", "belly", 50, true);
        AddOrganCoverage("lintestines", "lback", 5);
        AddOrganCoverage("sintestines", "lback", 33);
        AddOrganCoverage("lintestines", "groin", 5);
        AddOrganCoverage("lintestines", "loin", 10);

        AddOrganCoverage("rkidney", "lback", 20, true);
        AddOrganCoverage("lkidney", "lback", 20, true);
        AddOrganCoverage("rkidney", "belly", 5);
        AddOrganCoverage("lkidney", "belly", 5);
        _context.SaveChanges();

        #endregion

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bones...");

        AddBone(avianProto, "fskull", "frontal skull bone", BodypartTypeEnum.NonImmobilisingBone, 120, "Compact Bone");
        AddBone(avianProto, "cvertebrae", "cervical vertebrae", BodypartTypeEnum.NonImmobilisingBone, 80, "Compact Bone");
        AddBone(avianProto, "dvertebrae", "dorsal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 80, "Compact Bone");
        AddBone(avianProto, "lvertebrae", "lumbar vertebrae", BodypartTypeEnum.NonImmobilisingBone, 70, "Compact Bone");
        AddBone(avianProto, "cavertebrae", "caudal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 60, "Compact Bone");
        AddBone(avianProto, "sternum", "sternum", BodypartTypeEnum.NonImmobilisingBone, 120, "Compact Bone");
        AddBone(avianProto, "rscapula", "right scapula", BodypartTypeEnum.NonImmobilisingBone, 90, "Compact Bone");
        AddBone(avianProto, "lscapula", "left scapula", BodypartTypeEnum.NonImmobilisingBone, 90, "Compact Bone");
        AddBone(avianProto, "rhumerus", "right humerus", BodypartTypeEnum.Bone, 100, "Compact Bone");
        AddBone(avianProto, "lhumerus", "left humerus", BodypartTypeEnum.Bone, 100, "Compact Bone");
        AddBone(avianProto, "rradius", "right radius", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(avianProto, "lradius", "left radius", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(avianProto, "rulna", "right ulna", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(avianProto, "lulna", "left ulna", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(avianProto, "rmetacarpal", "right metacarpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
        AddBone(avianProto, "lmetacarpal", "left metacarpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
        AddBone(avianProto, "rilium", "right ilium", BodypartTypeEnum.NonImmobilisingBone, 90, "Compact Bone");
        AddBone(avianProto, "lilium", "left ilium", BodypartTypeEnum.NonImmobilisingBone, 90, "Compact Bone");
        AddBone(avianProto, "sacrum", "sacrum", BodypartTypeEnum.NonImmobilisingBone, 90, "Compact Bone");
        AddBone(avianProto, "rfemur", "right femur", BodypartTypeEnum.Bone, 100, "Compact Bone");
        AddBone(avianProto, "lfemur", "left femur", BodypartTypeEnum.Bone, 100, "Compact Bone");
        AddBone(avianProto, "rpatella", "right patella", BodypartTypeEnum.Bone, 40, "Compact Bone");
        AddBone(avianProto, "lpatella", "left patella", BodypartTypeEnum.Bone, 40, "Compact Bone");
        AddBone(avianProto, "rtibia", "right tibia", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(avianProto, "ltibia", "left tibia", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(avianProto, "rfibula", "right fibula", BodypartTypeEnum.Bone, 50, "Compact Bone");
        AddBone(avianProto, "lfibula", "left fibula", BodypartTypeEnum.Bone, 50, "Compact Bone");
        AddBone(avianProto, "rtarsus", "right tarsus", BodypartTypeEnum.Bone, 40, "Compact Bone");
        AddBone(avianProto, "ltarsus", "left tarsus", BodypartTypeEnum.Bone, 40, "Compact Bone");
        AddBone(avianProto, "rmetatarsus", "right metatarsus", BodypartTypeEnum.Bone, 40, "Compact Bone");
        AddBone(avianProto, "lmetatarsus", "left metatarsus", BodypartTypeEnum.Bone, 40, "Compact Bone");
        _context.SaveChanges();

        AddBoneInternal("fskull", "head", 100);
        AddBoneInternal("fskull", "bhead", 35, false);
        AddBoneInternal("cvertebrae", "bneck", 35);
        AddBoneInternal("dvertebrae", "uback", 20);
        AddBoneInternal("lvertebrae", "lback", 20);
        AddBoneInternal("cavertebrae", "tail", 80);
        AddBoneInternal("sternum", "abdomen", 50);
        AddBoneInternal("rscapula", "rshoulder", 100);
        AddBoneInternal("lscapula", "lshoulder", 100);
        AddBoneInternal("rhumerus", "rwingbase", 60);
        AddBoneInternal("lhumerus", "lwingbase", 60);
        AddBoneInternal("rradius", "rwing", 40);
        AddBoneInternal("lradius", "lwing", 40);
        AddBoneInternal("rulna", "rwing", 40);
        AddBoneInternal("lulna", "lwing", 40);
        AddBoneInternal("rmetacarpal", "rwing", 30);
        AddBoneInternal("lmetacarpal", "lwing", 30);
        AddBoneInternal("rilium", "loin", 20);
        AddBoneInternal("lilium", "loin", 20);
        AddBoneInternal("sacrum", "rump", 35);
        AddBoneInternal("rfemur", "rupperleg", 60);
        AddBoneInternal("lfemur", "lupperleg", 60);
        AddBoneInternal("rpatella", "rknee", 100);
        AddBoneInternal("lpatella", "lknee", 100);
        AddBoneInternal("rtibia", "rlowerleg", 60);
        AddBoneInternal("ltibia", "llowerleg", 60);
        AddBoneInternal("rfibula", "rlowerleg", 30);
        AddBoneInternal("lfibula", "llowerleg", 30);
        AddBoneInternal("rtarsus", "rankle", 40);
        AddBoneInternal("ltarsus", "lankle", 40);
        AddBoneInternal("rmetatarsus", "rfoot", 50);
        AddBoneInternal("lmetatarsus", "lfoot", 50);

        AddBoneCover("fskull", "brain", 100);
        AddBoneCover("cvertebrae", "uspinalcord", 100);
        AddBoneCover("dvertebrae", "mspinalcord", 100);
        AddBoneCover("lvertebrae", "lspinalcord", 100);
        AddBoneCover("sternum", "heart", 75);
        AddBoneCover("sternum", "rlung", 25);
        AddBoneCover("sternum", "llung", 25);
        _context.SaveChanges();

        _context.SaveChanges();

        foreach ((BodypartProto? child, BodypartProto? parent) in _cachedBodypartUpstreams)
        {
            _context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
            {
                Child = child.Id,
                Parent = parent.Id
            });
        }

        _context.SaveChanges();

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Limbs...");

        #region Limbs

        Dictionary<string, Limb> limbs = new(StringComparer.OrdinalIgnoreCase);

        void AddLimb(string name, LimbType limbType, string rootPart, double damageThreshold,
            double painThreshold)
        {
            Limb limb = new()
            {
                Name = name,
                LimbType = (int)limbType,
                RootBody = avianProto,
                RootBodypart = _cachedBodyparts[rootPart],
                LimbDamageThresholdMultiplier = damageThreshold,
                LimbPainThresholdMultiplier = painThreshold
            };
            _context.Limbs.Add(limb);
            limbs[name] = limb;
        }

        AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0);
        AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0);
        AddLimb("Genitals", LimbType.Genitals, "groin", 0.5, 0.5);
        AddLimb("Right Leg", LimbType.Leg, "rupperleg", 0.5, 0.5);
        AddLimb("Left Leg", LimbType.Leg, "lupperleg", 0.5, 0.5);
        AddLimb("Tail", LimbType.Appendage, "tail", 0.5, 0.5);
        AddLimb("Right Wing", LimbType.Wing, "rwingbase", 0.5, 0.5);
        AddLimb("Left Wing", LimbType.Wing, "lwingbase", 0.5, 0.5);
        _context.SaveChanges();

        foreach (Limb limb in limbs.Values)
        {
            foreach (BodypartProto part in _cachedLimbs[limb.Name])
            {
                _context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
            }

            string? spinalAlias = GetAvianSpinalOrganAliasForLimb(limb.Name);
            if (spinalAlias is not null)
            {
                _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                {
                    Limb = limb,
                    BodypartProto = _cachedOrgans[spinalAlias]
                });
            }
        }

        _context.SaveChanges();

        #endregion

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Groups...");

        #region Groups

        AddBodypartGroupDescriberShape(avianProto, "body", "The whole torso of an avian",
            ("abdomen", 1, 1),
            ("belly", 1, 1),
            ("breast", 0, 2),
            ("flank", 0, 4),
            ("loin", 0, 1),
            ("shoulder", 0, 2),
            ("upper back", 1, 1),
            ("lower back", 1, 1),
            ("rump", 0, 2),
            ("neck", 0, 1),
            ("neck back", 0, 1),
            ("throat", 0, 1)
        );
        AddBodypartGroupDescriberShape(avianProto, "legs", "Both legs of an avian",
            ("upper leg", 2, 2),
            ("lower leg", 0, 2),
            ("knee", 0, 2),
            ("ankle", 0, 2),
            ("foot", 0, 2),
            ("talon", 0, 2)
        );

        AddBodypartGroupDescriberShape(avianProto, "head", "An avian head",
            ("face", 1, 1),
            ("head back", 0, 1),
            ("eye socket", 0, 2),
            ("eye", 0, 2),
            ("ear", 0, 2),
            ("beak", 0, 1),
            ("nose", 0, 1),
            ("tongue", 0, 1),
            ("cheek", 0, 2),
            ("throat", 0, 1),
            ("neck", 0, 1),
            ("neck back", 0, 1)
        );

        AddBodypartGroupDescriberShape(avianProto, "back", "An avian back",
            ("upper back", 1, 1),
            ("lower back", 1, 1),
            ("flank", 0, 4)
        );

        AddBodypartGroupDescriberShape(avianProto, "wings", "A pair of avian wings",
            ("wing base", 2, 2),
            ("wing", 2, 2)
        );

        AddBodypartGroupDescriberShape(avianProto, "talons", "A pair of avian talons",
            ("talon", 2, 2)
        );

        AddBodypartGroupDescriberShape(avianProto, "feet", "A pair of avian feet",
            ("foot", 2, 2)
        );

        AddBodypartGroupDescriberShape(avianProto, "eyes", "A pair of avian eyes",
            ("eye socket", 2, 2),
            ("eye", 0, 2)
        );

        AddBodypartGroupDescriberShape(avianProto, "ears", "A pair of avian ears",
            ("ear", 2, 2)
        );

        #endregion

        _context.SaveChanges();

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");

        #region Races

        SeedAnimalRaces(GetBirdRaceTemplates(), ("Avian", avianProto));

        #endregion
    }
}
