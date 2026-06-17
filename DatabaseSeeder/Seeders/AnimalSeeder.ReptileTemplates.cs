#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private static IEnumerable<AnimalRaceTemplate> GetReptileAmphibianRaceTemplates()
    {
        static AnimalRaceTemplate Reptile(
            string name,
            string bodyKey,
            SizeCategory size,
            double health,
            string model,
            string loadout,
            string ageProfile,
            AnimalDescriptionPack pack,
            string combatStrategyKey = "Beast Brawler",
            string? description = null)
        {
            return new AnimalRaceTemplate(
                name,
                name,
                description ?? throw new InvalidOperationException($"Animal race {name} is missing a seeded description."),
                bodyKey,
                size,
                false,
                health,
                model,
                model,
                ageProfile,
                loadout,
                pack,
                false,
                AnimalBreathingMode.Simple,
                null,
                string.Equals(bodyKey, "Anuran", StringComparison.OrdinalIgnoreCase) ? "anuran" : "reptilian",
                null,
                combatStrategyKey
            );
        }

        yield return Reptile("Lizard", "Reptilian", SizeCategory.VerySmall, 0.15, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling lizard", "a young lizard", "a lizard",
                "It is small, scaled and quick, with a narrow head and darting tongue.",
                "Its feet are neatly clawed and its long tail works constantly for balance.",
                "It moves in short bursts of stillness and speed.",
                "sun-warmed rock and scrub"),
            description: """
            Lizards are scaled cold-blooded animals of sun-warmed stone, scrub, branches, river margins, deserts and marshes. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. The impression is completed by economical stillness broken by abrupt speed, giving the animal a readable rhythm even before it acts.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Gecko", "Reptilian", SizeCategory.VerySmall, 0.1, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling gecko", "a young gecko", "a gecko",
                "It is slight and soft-scaled, with large eyes and an agile little body.",
                "Its toes look oddly adept at gripping stone and wall alike.",
                "It seems made for sudden darting movement and impossible perches.",
                "warm wall, tree trunk and night garden"),
            description: """
            In sun-warmed stone, scrub, branches, river margins, deserts and marshes, geckos fill the role of scaled cold-blooded animals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. The impression is completed by economical stillness broken by abrupt speed, giving the animal a readable rhythm even before it acts.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Skink", "Reptilian", SizeCategory.VerySmall, 0.12, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling skink", "a young skink", "a skink",
                "It is smooth-scaled and glossy, with a narrow wedge of a head and a quick, shining body.",
                "Its tiny limbs and glassy scales make it look like something poured from bronze and taught to run.",
                "It flickers between cover and sunlight in nervous bursts of speed.",
                "leaf litter, warm stone and dry scrub"),
            description: """
            Skinks occupy sun-warmed stone, scrub, branches, river margins, deserts and marshes as scaled cold-blooded animals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. The impression is completed by economical stillness broken by abrupt speed, giving the animal a readable rhythm even before it acts.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Iguana", "Reptilian", SizeCategory.Small, 0.4, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling iguana", "a young iguana", "an iguana",
                "It is long and scaled, with a dewlap at the throat and a ridged crest along the back.",
                "Its tail and claws suggest a powerful climber with no wish to be handled.",
                "It has the sleepy arrogance of a sunning reptile entirely confident in its own indifference.",
                "branch, warm rock and humid woodland"),
            description: """
            Where sun-warmed stone, scrub, branches, river margins, deserts and marshes meets daily life, iguanas are recognisable as scaled cold-blooded animals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. The impression is completed by economical stillness broken by abrupt speed, giving the animal a readable rhythm even before it acts.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Monitor Lizard", "Reptilian", SizeCategory.Small, 0.7, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling monitor lizard", "a young monitor lizard", "a monitor lizard",
                "It is long-necked and powerfully built, with a muscular tail and a head that looks all teeth and suspicion.",
                "Its claws and heavy body suggest a reptile equally capable of digging, climbing and tearing at prey.",
                "It moves with deliberate confidence, as though it expects smaller creatures to yield the path.",
                "river margin, dry woodland and broken rock"),
            description: """
            Monitor lizards are scaled cold-blooded animals of sun-warmed stone, scrub, branches, river margins, deserts and marshes. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. Watch one move and the emphasis falls on economical stillness broken by abrupt speed, a pattern that tells more than size alone.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Chameleon", "Reptilian", SizeCategory.VerySmall, 0.12, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling chameleon", "a young chameleon", "a chameleon",
                "It is narrow-bodied and turret-eyed, with grasping feet and skin that shifts through subtle colour.",
                "Its curled tail and independently watchful eyes make it look perfectly adapted to branches and patience.",
                "It advances one careful step at a time, as if every movement must be negotiated with the leaves.",
                "branch, thorn scrub and warm woodland"),
            description: """
            In sun-warmed stone, scrub, branches, river margins, deserts and marshes, chameleons fill the role of scaled cold-blooded animals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. Watch one move and the emphasis falls on economical stillness broken by abrupt speed, a pattern that tells more than size alone.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Komodo Dragon", "Reptilian", SizeCategory.Normal, 1.0, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling Komodo dragon", "a young Komodo dragon", "a Komodo dragon",
                "It is huge for a lizard, deep-bodied and rough-scaled, with a heavy tail and muscular neck.",
                "Its head and jaws look brutally practical, more like a patient butcher than a quick darting hunter.",
                "It moves with slow predatory certainty, conserving effort until violence is close.",
                "dry island forest, savannah scrub and rocky shore"),
            description: """
            Komodo dragons occupy sun-warmed stone, scrub, branches, river margins, deserts and marshes as scaled cold-blooded animals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. Watch one move and the emphasis falls on economical stillness broken by abrupt speed, a pattern that tells more than size alone.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Tegu", "Reptilian", SizeCategory.Small, 0.6, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling tegu", "a young tegu", "a tegu",
                "It is stout and glossy-scaled, with strong limbs, a blunt head and a muscular tail.",
                "Its jaws and claws suggest a versatile reptile equally willing to root, raid or bite.",
                "It moves with confident curiosity through warm cover.",
                "rainforest edge, savannah scrub and riverbank"),
            description: """
            Where sun-warmed stone, scrub, branches, river margins, deserts and marshes meets daily life, tegus are recognisable as scaled cold-blooded animals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. Watch one move and the emphasis falls on economical stillness broken by abrupt speed, a pattern that tells more than size alone.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Caiman", "Reptilian", SizeCategory.Normal, 1.0, "Crocodilian", "crocodilian", "reptile",
            ReptilePack("a hatchling caiman", "a young caiman", "a caiman",
                "It is dark and armoured, with a narrow crocodilian head and a powerful tail.",
                "Its eyes and nostrils sit high, made for watching from still water with almost nothing exposed.",
                "It waits with compact, aquatic menace.",
                "tropical marsh, oxbow lake and reed-choked river"),
            combatStrategyKey: "Beast Drowner",
            description: """
            Caimen are scaled cold-blooded animals of sun-warmed stone, scrub, branches, river margins, deserts and marshes. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            What distinguishes the line is scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. Those features support economical stillness broken by abrupt speed, making the creature feel suited to its ground rather than merely placed there.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Frilled Lizard", "Reptilian", SizeCategory.VerySmall, 0.2, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling frilled lizard", "a young frilled lizard", "a frilled lizard",
                "It is lean and long-tailed, with a folded collar of skin about its neck.",
                "When tense, that frill threatens to transform its silhouette from small lizard to startling warning sign.",
                "It looks ready to flare, sprint and scramble away in one abrupt sequence.",
                "dry woodland, termite mound and warm scrub"),
            description: """
            In sun-warmed stone, scrub, branches, river margins, deserts and marshes, frilled lizards fill the role of scaled cold-blooded animals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The creature is best read through scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. Those features support economical stillness broken by abrupt speed, making the creature feel suited to its ground rather than merely placed there.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Thorny Devil", "Reptilian", SizeCategory.VerySmall, 0.15, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling thorny devil", "a young thorny devil", "a thorny devil",
                "It is small and squat, covered in spikes, ridges and desert-coloured armour.",
                "Its false-head hump and thorned outline make it look like a walking burr.",
                "It moves with slow, stiff caution through heat and sand.",
                "red desert, spinifex and dry sandy plain"),
            description: """
            Thorny devils occupy sun-warmed stone, scrub, branches, river margins, deserts and marshes as scaled cold-blooded animals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Its strongest visual signs are scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. Those features support economical stillness broken by abrupt speed, making the creature feel suited to its ground rather than merely placed there.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Blue-Tongued Skink", "Reptilian", SizeCategory.VerySmall, 0.18, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling blue-tongued skink", "a young blue-tongued skink", "a blue-tongued skink",
                "It is glossy, thick-bodied and short-legged, with a blunt head and a vivid blue tongue.",
                "Its heavy body and warning display make it look more stubborn than swift.",
                "It holds its ground with small reptilian confidence.",
                "garden edge, woodland floor and dry grass"),
            description: """
            Where sun-warmed stone, scrub, branches, river margins, deserts and marshes meets daily life, blue-tongued skinks are recognisable as scaled cold-blooded animals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The eye is drawn to scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. Those features support economical stillness broken by abrupt speed, making the creature feel suited to its ground rather than merely placed there.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Reptile("Bearded Dragon", "Reptilian", SizeCategory.VerySmall, 0.2, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling bearded dragon", "a young bearded dragon", "a bearded dragon",
                "It is broad-headed and spiky, with a rough beard of scales beneath the throat.",
                "Its flattened body and watchful basking posture suit hot stone, dust and open scrub.",
                "It seems calm until its throat darkens and the small warning display begins.",
                "arid woodland, rock ledge and sun-warmed scrub"),
            description: """
            Bearded dragons are scaled cold-blooded animals of sun-warmed stone, scrub, branches, river margins, deserts and marshes. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            At a glance, the form resolves into scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. In motion they are marked by economical stillness broken by abrupt speed, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. A bearded dragon can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Reptile("Gila Monster", "Reptilian", SizeCategory.Small, 0.35, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling Gila monster", "a young Gila monster", "a Gila monster",
                "It is heavy and bead-scaled, boldly patterned in black and orange-pink.",
                "Its slow movement and strong jaws give it a warning quality quite unlike a harmless basking lizard.",
                "It advances with deliberate, venomous confidence.",
                "desert wash, rocky scrub and hot burrow"),
            description: """
            In sun-warmed stone, scrub, branches, river margins, deserts and marshes, gila monsters fill the role of scaled cold-blooded animals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The visible essentials are scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. In motion they are marked by economical stillness broken by abrupt speed, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. A gila monster can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Reptile("Basilisk Lizard", "Reptilian", SizeCategory.VerySmall, 0.2, "Reptilian", "reptile", "reptile",
            ReptilePack("a hatchling basilisk lizard", "a young basilisk lizard", "a basilisk lizard",
                "It is light and crested, with long toes, a whip tail and a nervous bright-eyed face.",
                "Its limbs and splayed feet look adapted for explosive runs over water and mud.",
                "It balances between branch and bank like a lizard prepared to flee in any direction.",
                "streamside forest, riverbank and tropical thicket"),
            description: """
            Basilisk lizards occupy sun-warmed stone, scrub, branches, river margins, deserts and marshes as scaled cold-blooded animals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Look closer and the outline is all scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. In motion they are marked by economical stillness broken by abrupt speed, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. A basilisk lizard can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Reptile("Turtle", "Reptilian", SizeCategory.Small, 0.5, "Chelonian", "chelonian", "reptile",
            ReptilePack("a hatchling turtle", "a young turtle", "a turtle",
                "It is shell-backed and deliberate, with a broad body housed beneath a domed carapace.",
                "Its beaked mouth and sturdy limbs give it a look at once ancient and stubborn.",
                "It proceeds with patient certainty, unconcerned by urgency.",
                "pond edge, marsh and slow river"),
            description: """
            Where sun-warmed stone, scrub, branches, river margins, deserts and marshes meets daily life, turtles are recognisable as scaled cold-blooded animals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The first things to register are scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. In motion they are marked by economical stillness broken by abrupt speed, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. A turtle can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Reptile("Tortoise", "Reptilian", SizeCategory.Small, 0.6, "Chelonian", "chelonian", "reptile",
            ReptilePack("a hatchling tortoise", "a young tortoise", "a tortoise",
                "It is thick-shelled and land-bound, with a high carapace and sturdy columnar limbs.",
                "Its beak and shell together make it look more like a walking fortress than an agile animal.",
                "It advances with stubborn, ancient patience.",
                "dry scrub, warm plain and dusty yard"),
            description: """
            Tortoises are scaled cold-blooded animals of sun-warmed stone, scrub, branches, river margins, deserts and marshes. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. The impression is completed by economical stillness broken by abrupt speed, giving the animal a readable rhythm even before it acts.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. A tortoise can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Reptile("Crocodile", "Reptilian", SizeCategory.Large, 1.4, "Crocodilian", "crocodilian", "reptile",
            ReptilePack("a hatchling crocodile", "a young crocodile", "a crocodile",
                "It is low and broad, with plated hide, a long jaw and a powerful tail.",
                "Its teeth are impossible to ignore, and the entire body looks built around sudden explosive force.",
                "It lies with perfect stillness until movement becomes violence.",
                "riverbank, swamp and warm backwater"),
            combatStrategyKey: "Beast Drowner",
            description: """
            In sun-warmed stone, scrub, branches, river margins, deserts and marshes, crocodiles fill the role of scaled cold-blooded animals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. The impression is completed by economical stillness broken by abrupt speed, giving the animal a readable rhythm even before it acts.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. A crocodile can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Reptile("Alligator", "Reptilian", SizeCategory.Large, 1.2, "Crocodilian", "crocodilian", "reptile",
            ReptilePack("a hatchling alligator", "a young alligator", "an alligator",
                "It is armoured and broad-snouted, with dark hide and a tail thick with muscle.",
                "Its profile is blunt and heavy, all the more dangerous for being so calm.",
                "It has the patient menace of something that expects prey to come within reach eventually.",
                "marsh, bayou and reed-choked water"),
            combatStrategyKey: "Beast Drowner",
            description: """
            Alligators occupy sun-warmed stone, scrub, branches, river margins, deserts and marshes as scaled cold-blooded animals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by scales, claws, tails and watchful eyes shaped by basking, ambush and quick retreat. The impression is completed by economical stillness broken by abrupt speed, giving the animal a readable rhythm even before it acts.

            They are easy to dismiss until the mouth opens, the tail lashes, or the still water moves. A alligator can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Reptile("Frog", "Anuran", SizeCategory.VerySmall, 0.1, "Anuran", "anuran", "amphibian",
            ReptilePack("a tadpole", "a young frog", "a frog",
                "It is compact and damp-skinned, with wide-set eyes and powerful hindlegs folded beneath the body.",
                "Its shape is all crouch, leap and sudden stillness.",
                "It looks ready to spring out of danger faster than anything should.",
                "pond edge, reed bed and wet grass"),
            description: """
            Where pond edges, reed beds, damp soil and evening grass meets daily life, frogs are recognisable as damp-skinned amphibians. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by wide mouths, folded hindlegs, soft skin and eyes set for watching from cover. The impression is completed by crouched patience followed by sudden, elastic leaps or hops, giving the animal a readable rhythm even before it acts.

            They are humble signs of wet ground and night air, noticed most when their calls fill the dark. A frog can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Reptile("Toad", "Anuran", SizeCategory.VerySmall, 0.12, "Anuran", "anuran", "amphibian",
            ReptilePack("a tadpole", "a young toad", "a toad",
                "It is squat and warty-skinned, with a broad mouth and heavy hindlegs.",
                "Its rougher skin and thicker body give it a sturdier, earthier look than a frog.",
                "It waits in stillness broken by abrupt ungainly hops.",
                "garden edge, damp soil and evening grass"),
            description: """
            Toads are damp-skinned amphibians of pond edges, reed beds, damp soil and evening grass. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: wide mouths, folded hindlegs, soft skin and eyes set for watching from cover. Watch one move and the emphasis falls on crouched patience followed by sudden, elastic leaps or hops, a pattern that tells more than size alone.

            They are humble signs of wet ground and night air, noticed most when their calls fill the dark. A toad can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
    }
}
