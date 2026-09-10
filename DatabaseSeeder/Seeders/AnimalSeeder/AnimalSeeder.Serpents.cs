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
    private static IEnumerable<AnimalRaceTemplate> GetSerpentRaceTemplates()
    {
        static AnimalRaceTemplate Snake(string name, SizeCategory size, double health, string loadout, string description,
            string feature, string behaviour, string habitat, string combatStrategyKey = "Beast Clincher",
            string? raceDescription = null)
        {
            return new AnimalRaceTemplate(
                name,
                name,
                raceDescription ?? throw new InvalidOperationException($"Animal race {name} is missing a seeded description."),
                "Serpentine",
                size,
                string.Equals(name, "Tree Python", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "Anaconda", StringComparison.OrdinalIgnoreCase),
                health,
                "Serpent",
                "Serpent",
                "standard-mammal",
                loadout,
                SerpentPack($"a {name.ToLowerInvariant()} hatchling", $"a young {name.ToLowerInvariant()}", $"a {name.ToLowerInvariant()}",
                    description, feature, behaviour, habitat),
                false,
                AnimalBreathingMode.Simple,
                null,
                "serpent",
                null,
                combatStrategyKey
            );
        }

        yield return Snake("Python", SizeCategory.Small, 0.1, "serpent-constrictor",
            "It is heavy-bodied and beautifully patterned, its coils thick with latent strength.",
            "Its head is wedge-shaped and calm, giving away little of what those muscles can do.",
            "It carries itself with the slow confidence of a constrictor that trusts its body more than venom.",
            "forest floor and humid undergrowth",
            raceDescription: """
            In forest floors, scrub, branches, deserts, marshes and warm ruins, pythons fill the role of legless, scaled predators. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Watch one move and the emphasis falls on patient stillness followed by sudden, exact movement, a pattern that tells more than size alone.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Tree Python", SizeCategory.Normal, 0.4, "serpent-constrictor",
            "It is lithe and branch-coloured, its body patterned to vanish among leaves and bark.",
            "Its tail and balance suggest a serpent comfortable above the ground as well as on it.",
            "It coils with patient precision, as though every branch were already mapped in its head.",
            "branch, canopy and warm woodland",
            raceDescription: """
            Tree pythons occupy forest floors, scrub, branches, deserts, marshes and warm ruins as legless, scaled predators. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Watch one move and the emphasis falls on patient stillness followed by sudden, exact movement, a pattern that tells more than size alone.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Boa", SizeCategory.Normal, 0.6, "serpent-constrictor",
            "It is thick-bodied and muscular, patterned in warm browns that blur into mottled cover.",
            "Its body has the dense, deliberate strength of a snake made to hold struggling prey.",
            "It moves with unhurried certainty, wasting no motion.",
            "jungle floor and broken scrub",
            raceDescription: """
            Where forest floors, scrub, branches, deserts, marshes and warm ruins meets daily life, boas are recognisable as legless, scaled predators. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Watch one move and the emphasis falls on patient stillness followed by sudden, exact movement, a pattern that tells more than size alone.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Anaconda", SizeCategory.Large, 1.0, "serpent-constrictor",
            "It is huge and river-dark, its olive body marked by darker saddles and circles.",
            "Its immense girth gives it the oppressive presence of something built to overpower by sheer mass.",
            "It glides with awful patience, whether through water or mud.",
            "swamp, oxbow and flooded jungle",
            raceDescription: """
            Anacondas are legless, scaled predators of forest floors, scrub, branches, deserts, marshes and warm ruins. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Its strongest visual signs are smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Those features support patient stillness followed by sudden, exact movement, making the creature feel suited to its ground rather than merely placed there.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Cobra", SizeCategory.Small, 0.2, "serpent-neurotoxic",
            "It is slender and smooth-scaled, with a neck capable of spreading into a threatening hood.",
            "Its lifted posture and focused stare are as much a warning as its venom.",
            "It radiates poised hostility, ready to strike from a short violent distance.",
            "scrub, ruin and warm grassland",
            raceDescription: """
            In forest floors, scrub, branches, deserts, marshes and warm ruins, cobras fill the role of legless, scaled predators. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The eye is drawn to smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Those features support patient stillness followed by sudden, exact movement, making the creature feel suited to its ground rather than merely placed there.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Adder", SizeCategory.Small, 0.2, "serpent-hemotoxic",
            "It is short, thick and darkly patterned, with a triangular head and heavy body.",
            "Its build is compact and efficient, ideal for short-range ambush rather than pursuit.",
            "It lies still with the quiet, ugly confidence of a pitfall in living form.",
            "heath, rocky ground and cool scrub",
            raceDescription: """
            Adders occupy forest floors, scrub, branches, deserts, marshes and warm ruins as legless, scaled predators. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The most telling cues are smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Those features support patient stillness followed by sudden, exact movement, making the creature feel suited to its ground rather than merely placed there.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Rattlesnake", SizeCategory.Small, 0.2, "serpent-hemotoxic",
            "It is dusty-scaled and thick-bodied, with a broad head and a rattle at the end of its tail.",
            "Its tail advertises danger in a way few serpents ever bother to do.",
            "It looks like an animal willing to warn once and punish once.",
            "desert, scrub and dry canyon",
            raceDescription: """
            Where forest floors, scrub, branches, deserts, marshes and warm ruins meets daily life, rattlesnakes are recognisable as legless, scaled predators. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Most of its character sits in smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. Those features support patient stillness followed by sudden, exact movement, making the creature feel suited to its ground rather than merely placed there.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Snake("Viper", SizeCategory.Small, 0.2, "serpent-cytotoxic",
            "It is broad-headed and rough-scaled, its body set in heavy looping curves.",
            "Its triangular skull and thick neck make the danger obvious at a glance.",
            "It remains still until movement matters, then seems all teeth and impact.",
            "rocky scrub and tangled brush",
            raceDescription: """
            Vipers are legless, scaled predators of forest floors, scrub, branches, deserts, marshes and warm ruins. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Look closer and the outline is all smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. In motion they are marked by patient stillness followed by sudden, exact movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. A viper can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Snake("Mamba", SizeCategory.Small, 0.2, "serpent-neurotoxic",
            "It is long, clean-scaled and unnervingly elegant, its body all speed and reach.",
            "Its narrow coffin-shaped head gives it a hard, severe silhouette.",
            "It has a tense, predatory quickness that suggests lightning-fast violence.",
            "dry woodland and warm savannah",
            "Beast Skirmisher",
            raceDescription: """
            In forest floors, scrub, branches, deserts, marshes and warm ruins, mambas fill the role of legless, scaled predators. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The first things to register are smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. In motion they are marked by patient stillness followed by sudden, exact movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. A mamba can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Snake("Coral Snake", SizeCategory.Small, 0.2, "serpent-neurotoxic",
            "It is slim and brilliantly banded, its scales arranged in warning colours too vivid to ignore.",
            "Its beauty feels deliberate, almost theatrical, like a banner for its own danger.",
            "It moves in a neat, purposeful line, calm in the confidence of its venom.",
            "leaf litter and warm woodland",
            raceDescription: """
            Coral snakes occupy forest floors, scrub, branches, deserts, marshes and warm ruins as legless, scaled predators. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Its profile is built around smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. In motion they are marked by patient stillness followed by sudden, exact movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. A coral snake can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Snake("Moccasin", SizeCategory.Small, 0.2, "serpent-hemotoxic",
            "It is dark and muscular, with a broad head and heavy body built for close wet-country ambush.",
            "Its thick neck and blunt face give it a solid, ugly forcefulness.",
            "It looks as though it would rather hold its ground than flee.",
            "swamp, marsh edge and dark water",
            raceDescription: """
            Where forest floors, scrub, branches, deserts, marshes and warm ruins meets daily life, moccasins are recognisable as legless, scaled predators. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The creature presents smooth coils, wedge-shaped heads, watchful tongues and muscles hidden beneath patterned scales. In motion they are marked by patient stillness followed by sudden, exact movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are treated with caution even when small, because a serpent's danger is rarely measured by noise or size alone. A moccasin can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
    }

    private void SeedSerpents(BodyProto wormProto, BodyProto serpentProto)
    {
        ResetCachedParts();
        int order = 1;
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodyparts...");

        #region Bodyparts

        AddBodypart(wormProto, "head", "head", "head back", BodypartTypeEnum.Wear, null, Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(wormProto, "mouth", "mouth", "mouth", BodypartTypeEnum.Wear, "head", Alignment.Front,
            Orientation.Highest, 40, -1, 50, order++, "Bony Flesh", SizeCategory.Small, "Head", true,
            isVital: false, implantSpace: 0, stunMultiplier: 1.0);
        AddBodypart(serpentProto, "fangs", "fangs", "fang", BodypartTypeEnum.Wear, "mouth", Alignment.Front,
            Orientation.Highest, 40, -1, 50, order++, "Tooth", SizeCategory.Small, "Head", true, isVital: false,
            implantSpace: 0, stunMultiplier: 1.0);
        AddBodypart(wormProto, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.Wear,
            "head", Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh",
            SizeCategory.Small, "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(wormProto, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.Wear, "head",
            Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(wormProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket",
            Alignment.FrontRight, Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head",
            true, isVital: false, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(wormProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket",
            Alignment.FrontLeft, Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head",
            true, isVital: false, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(serpentProto, "tongue", "tongue", "tongue", BodypartTypeEnum.Tongue, "mouth", Alignment.Front,
            Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(wormProto, "neck", "neck", "neck", BodypartTypeEnum.Wear, "head", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(wormProto, "ubody", "upper body", "serpent body", BodypartTypeEnum.Wear, "neck",
            Alignment.Front, Orientation.High, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(wormProto, "mbody", "middle body", "serpent body", BodypartTypeEnum.Wear, "ubody",
            Alignment.Front, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(wormProto, "lbody", "lower body", "serpent body", BodypartTypeEnum.Wear, "mbody",
            Alignment.Front, Orientation.Low, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(wormProto, "tail", "tail", "tail", BodypartTypeEnum.Wear, "lbody", Alignment.Rear,
            Orientation.Lowest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Tail", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.5);

        #endregion

        _context.SaveChanges();

        #region Bones

        AddBone(serpentProto, "fskull", "frontal skull bone", BodypartTypeEnum.NonImmobilisingBone, 200,
            "Compact Bone");
        AddBone(serpentProto, "cvertebrae", "cervical vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(serpentProto, "dvertebrae", "dorsal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(serpentProto, "lvertebrae", "lumbar vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(serpentProto, "cavertebrae", "caudal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        _context.SaveChanges();

        AddBoneInternal("fskull", "head", 100);
        AddBoneInternal("cvertebrae", "neck", 100);
        AddBoneInternal("dvertebrae", "ubody", 100);
        AddBoneInternal("lvertebrae", "mbody", 100);
        AddBoneInternal("lvertebrae", "lbody", 100, false);
        AddBoneInternal("cavertebrae", "tail", 100);
        _context.SaveChanges();

        #endregion

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Organs...");

        #region Organs

        AddOrgan(wormProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, stunModifier: 1.0);
        AddOrgan(wormProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
        AddOrgan(wormProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(wormProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(wormProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(wormProto, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
            0.05);
        AddOrgan(wormProto, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
            0.05);
        AddOrgan(wormProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(wormProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(wormProto, "rlung", "right lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(wormProto, "llung", "left lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(wormProto, "trachea", "trachea", BodypartTypeEnum.Trachea, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(wormProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(wormProto, "spinalcord", "spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(wormProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
        AddOrgan(wormProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

        AddOrganCoverage("brain", "head", 100, true);
        AddOrganCoverage("brain", "reyesocket", 85);
        AddOrganCoverage("brain", "leyesocket", 85);
        AddOrganCoverage("brain", "reye", 85);
        AddOrganCoverage("brain", "leye", 85);
        AddOrganCoverage("brain", "mouth", 10);

        AddOrganCoverage("linnerear", "head", 5, true);
        AddOrganCoverage("rinnerear", "head", 5, true);
        AddOrganCoverage("esophagus", "neck", 50, true);
        AddOrganCoverage("trachea", "neck", 50, true);

        AddOrganCoverage("rlung", "ubody", 100, true);
        AddOrganCoverage("llung", "ubody", 100, true);

        AddOrganCoverage("heart", "ubody", 33, true);

        AddOrganCoverage("spinalcord", "neck", 10, true);
        AddOrganCoverage("spinalcord", "ubody", 2);

        AddOrganCoverage("liver", "mbody", 33, true);
        AddOrganCoverage("spleen", "mbody", 20, true);
        AddOrganCoverage("stomach", "mbody", 20, true);

        AddOrganCoverage("lintestines", "lbody", 5, true);
        AddOrganCoverage("sintestines", "lbody", 50, true);

        AddOrganCoverage("rkidney", "lbody", 20, true);
        AddOrganCoverage("lkidney", "lbody", 20, true);

        AddBoneCover("fskull", "brain", 100);
        AddBoneCover("cvertebrae", "spinalcord", 100);
        AddBoneCover("dvertebrae", "spinalcord", 100);
        AddBoneCover("lvertebrae", "spinalcord", 100);
        AddBoneCover("cavertebrae", "spinalcord", 100);
        _context.SaveChanges();

        #endregion

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
                RootBody = wormProto,
                RootBodypart = _cachedBodyparts[rootPart],
                LimbDamageThresholdMultiplier = damageThreshold,
                LimbPainThresholdMultiplier = painThreshold
            };
            _context.Limbs.Add(limb);
            limbs[name] = limb;
        }

        AddLimb("Torso", LimbType.Torso, "ubody", 1.0, 1.0);
        AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0);
        AddLimb("Tail", LimbType.Appendage, "tail", 0.5, 0.5);
        _context.SaveChanges();

        foreach (Limb limb in limbs.Values)
        {
            foreach (BodypartProto part in _cachedLimbs[limb.Name])
            {
                _context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
            }

            switch (limb.Name)
            {
                case "Torso":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["spinalcord"] });
                    break;
            }
        }

        _context.SaveChanges();

        #endregion

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Groups...");

        #region Groups

        AddBodypartGroupDescriberShape(serpentProto, "body", "The whole torso of a worm",
            ("serpent body", 1, 3),
            ("tail", 0, 1),
            ("neck", 0, 1)
        );

        AddBodypartGroupDescriberShape(serpentProto, "eyes", "A pair of serpent eyes",
            ("eye socket", 2, 2),
            ("eye", 0, 2)
        );

        #endregion

        _context.SaveChanges();

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");

        #region Races

        SeedAnimalRaces(GetSerpentRaceTemplates(),
            ("Serpentine", serpentProto));

        #endregion
    }
}
