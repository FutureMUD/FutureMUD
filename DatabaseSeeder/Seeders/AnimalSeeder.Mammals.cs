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
    private static IEnumerable<AnimalRaceTemplate> GetMammalRaceTemplates()
    {
        static AnimalRaceTemplate Mammal(
            string name,
            string adjective,
            string bodyKey,
            SizeCategory size,
            double health,
            string model,
            string ageProfile,
            string loadout,
            AnimalDescriptionPack pack,
            string description,
            bool canClimb = false,
            string? bloodProfile = null,
            IReadOnlyList<AnimalBodypartUsageTemplate>? usages = null,
            string combatStrategyKey = "Beast Brawler")
        {
            return new AnimalRaceTemplate(
                name,
                adjective,
                description,
                bodyKey,
                size,
                canClimb,
                health,
                model,
                model,
                ageProfile,
                loadout,
                pack,
                true,
                AnimalBreathingMode.Simple,
                bloodProfile,
                bodyKey switch
                {
                    "Ungulate" => "ungulate",
                    "Toed Quadruped" => "toed-quadruped",
                    _ => null
                },
                usages,
                combatStrategyKey
            );
        }

        List<AnimalBodypartUsageTemplate> udder = new()
        { new("udder", "female") };
        List<AnimalBodypartUsageTemplate> hornedMale = new()
        { new("rhorn", "male"), new("lhorn", "male") };
        List<AnimalBodypartUsageTemplate> hornedFemale = new()
        { new("udder", "female"), new("rhorn", "male"), new("lhorn", "male") };
        List<AnimalBodypartUsageTemplate> antlered = new()
        { new("udder", "female"), new("rantler", "male"), new("lantler", "male") };
        List<AnimalBodypartUsageTemplate> antleredGeneral = new()
        { new("udder", "female"), new("rantler", "general"), new("lantler", "general") };
        List<AnimalBodypartUsageTemplate> tuskedMale = new()
        { new("rtusk", "male"), new("ltusk", "male") };
        List<AnimalBodypartUsageTemplate> tuskedGeneral = new()
        { new("rtusk", "general"), new("ltusk", "general") };
        List<AnimalBodypartUsageTemplate> rhinoHorn = new()
        { new("udder", "female"), new("horn", "general") };

        yield return Mammal("Rabbit", "Leporine", "Toed Quadruped", SizeCategory.VerySmall, 0.3, "Lagomorph",
            "tiny-fast", "small-herbivore",
            MammalPack("a rabbit kit", "a young buck rabbit", "a young doe rabbit", "a buck rabbit", "a doe rabbit",
                "It has soft fur, large ears and quick, spring-loaded hindlegs.",
                "Its wide-set eyes and alert ears leave it looking permanently watchful.",
                "It looks ready to bolt at the first hint of danger.",
                "burrows, hedgerows and open grassland"),
            combatStrategyKey: "Beast Coward",
            description: """
            Rabbits are small, long-eared grazing mammals of burrows, hedgerows, heath and open grassland. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            At a glance, the form resolves into soft fur, high-set ears and hindquarters built for abrupt leaps. In motion they are marked by watchful, spring-loaded movement and an instinct for vanishing into cover, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are usually read as prey animals: nervous, quick and most dangerous only when cornered or handled carelessly. A rabbit can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Mammal("Hare", "Hare", "Toed Quadruped", SizeCategory.VerySmall, 0.5, "Lagomorph",
            "tiny-fast", "small-herbivore",
            MammalPack("a leveret", "a young jack hare", "a young jill hare", "a jack hare", "a jill hare",
                "It is long-limbed and lean, with oversized ears and a rangier frame than a rabbit.",
                "Its powerful hindquarters look built for explosive speed across open ground.",
                "It stands tense and light on its feet, as if made to run.",
                "heath, scrub and open country"),
            combatStrategyKey: "Beast Coward",
            description: """
            In burrows, hedgerows, heath and open grassland, hares fill the role of small, long-eared grazing mammals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The visible essentials are soft fur, high-set ears and hindquarters built for abrupt leaps. In motion they are marked by watchful, spring-loaded movement and an instinct for vanishing into cover, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are usually read as prey animals: nervous, quick and most dangerous only when cornered or handled carelessly. A hare can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Mammal("Beaver", "Beaver", "Toed Quadruped", SizeCategory.Small, 0.8, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a beaver kit", "a young male beaver", "a young female beaver", "a male beaver", "a female beaver",
                "It is stocky and brown-furred, with a broad flat tail and heavy incisors.",
                "Its teeth and webbed feet betray a life of gnawing timber and working in water.",
                "It has the purposeful, busy air of a tireless builder.",
                "riverbanks and quiet waterways"),
            description: """
            Beavers occupy riverbanks, marsh margins and sheltered waterways as water-edge mammals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Look closer and the outline is all dense fur, practical paws and frames shaped by swimming as much as by walking. In motion they are marked by purposeful movement between mud, bank and water, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They make river country feel inhabited, useful to trappers and watchers but never quite domestic in their habits. A beaver can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Mammal("Otter", "Otter", "Toed Quadruped", SizeCategory.VerySmall, 0.5, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("an otter pup", "a young male otter", "a young female otter", "a male otter", "a female otter",
                "It has sleek waterproof fur, a long body and quick webbed paws.",
                "Its whiskered muzzle and supple frame suit it to swift hunting in water.",
                "It moves with playful grace and easy confidence.",
                "rivers, marshes and sheltered coasts"),
            combatStrategyKey: "Beast Skirmisher",
            description: """
            Where riverbanks, marsh margins and sheltered waterways meets daily life, otters are recognisable as water-edge mammals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The first things to register are dense fur, practical paws and frames shaped by swimming as much as by walking. In motion they are marked by purposeful movement between mud, bank and water, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They make river country feel inhabited, useful to trappers and watchers but never quite domestic in their habits. A otter can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Mammal("Dog", "Canine", "Toed Quadruped", SizeCategory.Small, 0.8, "Small Canid",
            "standard-mammal", "doglike",
            MammalPack("a puppy", "a young dog", "a young dog", "a dog", "a bitch",
                "It has a compact, muscular frame, keen nose and a coat that varies by breed.",
                "Its bright eyes and alert ears give it an eager, highly social look.",
                "It carries itself with the curious, companionable energy of a domestic hunter.",
                "farmsteads, roads and human settlements") with
            { AddDogBreedDescriptions = true },
            """
            Dogs are social canids shaped by long association with settlements of farmsteads, roads, camps and the edges of habitation. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by keen noses, alert ears and bodies that vary widely by line and purpose. The impression is completed by companionable attention mixed with a hunter's readiness, giving the animal a readable rhythm even before it acts.

            They can be workmate, guardian, scavenger or threat depending on training and treatment. A dog can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, bloodProfile: "canine");
        yield return Mammal("Cat", "Feline", "Toed Quadruped", SizeCategory.Small, 0.5, "Domestic Cat",
            "standard-mammal", "cat",
            MammalPack("a kitten", "a young tomcat", "a young cat", "a tomcat", "a cat",
                "It is compact and graceful, with a supple spine and soft fur.",
                "Its eyes are bright, its whiskers keen and its paws are armed with fine retractile claws.",
                "It has the poised, self-contained air of a patient little predator.",
                "hearths, rooftops and the edges of human habitation") with
            { UseCatCoatDescriptions = true },
            """
            Cats occupy hearths, rooftops, barns and the quieter corners of settlements as small, graceful domestic felids. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: supple spines, bright eyes, whiskers and fine retractile claws. Watch one move and the emphasis falls on poised self-possession and sudden little bursts of predatory focus, a pattern that tells more than size alone.

            They live close to people without ever seeming fully owned, useful around vermin and inscrutable around affection. A cat can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, bloodProfile: "feline");
        yield return Mammal("Wolf", "Lupine", "Toed Quadruped", SizeCategory.Normal, 1.0, "Large Canid",
            "standard-mammal", "wolfpack",
            MammalPack("a wolf pup", "a young male wolf", "a young female wolf", "a male wolf", "a female wolf",
                "It is long-legged and powerfully built, with a deep chest and thick grey coat.",
                "Its heavy jaws, triangular ears and keen eyes make it look every inch a pack hunter.",
                "It carries itself with wary confidence and relentless focus.",
                "forest, tundra and broken wilderness"),
            """
            In woodland edges, scrub, dry plains and open wilderness, wolfs fill the role of lean, sharp-sensed canid predators. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by long muzzles, quick feet and a rangy build meant for scent, pursuit and opportunism. The impression is completed by wary confidence and a habit of testing distance before committing, giving the animal a readable rhythm even before it acts.

            Their presence at the edge of camp or pasture is enough to make livestock restless and travellers attentive. A wolf can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, bloodProfile: "canine");
        yield return Mammal("Fox", "Vulpine", "Toed Quadruped", SizeCategory.Small, 0.8, "Small Canid",
            "standard-mammal", "small-predator",
            MammalPack("a fox kit", "a young dog fox", "a young vixen", "a dog fox", "a vixen",
                "It is slim, light-footed and wrapped in a neat coat of reddish fur.",
                "Its pointed muzzle, sharp ears and brush tail give it a quick, crafty look.",
                "It seems perpetually ready to slip away into cover.",
                "woodland edges, scrub and farmland"),
            """
            Foxes occupy woodland edges, scrub, dry plains and open wilderness as lean, sharp-sensed canid predators. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by long muzzles, quick feet and a rangy build meant for scent, pursuit and opportunism. The impression is completed by wary confidence and a habit of testing distance before committing, giving the animal a readable rhythm even before it acts.

            Their presence at the edge of camp or pasture is enough to make livestock restless and travellers attentive. A fox can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Coyote", "Coyote", "Toed Quadruped", SizeCategory.Small, 0.8, "Small Canid",
            "standard-mammal", "wolfpack",
            MammalPack("a coyote pup", "a young male coyote", "a young female coyote", "a male coyote", "a female coyote",
                "It is rangy and dust-coloured, with narrow shoulders and a brushy tail.",
                "Its wary amber eyes and oversized ears mark it as a watchful opportunist.",
                "It looks nervous, clever and ready to run or bite in an instant.",
                "dry scrub, prairie and rough hill country"),
            """
            Where woodland edges, scrub, dry plains and open wilderness meets daily life, coyotes are recognisable as lean, sharp-sensed canid predators. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by long muzzles, quick feet and a rangy build meant for scent, pursuit and opportunism. The impression is completed by wary confidence and a habit of testing distance before committing, giving the animal a readable rhythm even before it acts.

            Their presence at the edge of camp or pasture is enough to make livestock restless and travellers attentive. A coyote can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Mammal("Hyena", "Hyena", "Toed Quadruped", SizeCategory.Small, 0.8, "Small Canid",
            "standard-mammal", "wolfpack",
            MammalPack("a hyena cub", "a young male hyena", "a young female hyena", "a male hyena", "a female hyena",
                "It has a sloping back, coarse spotted coat and a massively developed neck and shoulders.",
                "Its heavy jaws look capable of breaking bone with ugly ease.",
                "It gives off a rough, shambling power rather than feline grace.",
                "savannah, thorn scrub and dry plains"),
            """
            Hyenas occupy savannah, thorn scrub and dry plains as rough, heavy-jawed predators and scavengers. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            At a glance, the form resolves into sloped backs, powerful necks, coarse coats and jaws made for cracking bone. In motion they are marked by shambling confidence and watchful opportunism, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They gather around carrion, camps and battlefields, where their laughter-like calls make the dark feel occupied. Around settlements, roads or camps, hyenas add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Mammal("Lion", "Leonine", "Toed Quadruped", SizeCategory.Normal, 1.3, "Big Felid",
            "standard-mammal", "big-cat",
            MammalPack("a lion cub", "a young lion", "a young lioness", "a lion", "a lioness",
                "It is large, deep-chested and heavily muscled beneath a close tawny coat.",
                "Its broad head and predatory shoulders make it look built to dominate prey with raw strength.",
                "It carries itself with slow assurance and the easy authority of an apex hunter.",
                "grassland and open savannah"),
            """
            Where grassland, forest edge, jungle, rocky scrub and broken cover meets daily life, lions are recognisable as large predatory felids. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: heavy shoulders, padded paws, deep jaws and muscle arranged for ambush and impact. Watch one move and the emphasis falls on a coiled, silent confidence that makes stillness feel dangerous, a pattern that tells more than size alone.

            They are not merely wildlife but territorial facts, the kind of animals that reshape routes, grazing patterns and local caution. A lion can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Mammal("Tiger", "Tigrine", "Toed Quadruped", SizeCategory.Normal, 1.3, "Big Felid",
            "standard-mammal", "big-cat",
            MammalPack("a tiger cub", "a young tiger", "a young tigress", "a tiger", "a tigress",
                "It is massive and sinewy, striped black over a rich orange coat.",
                "Its broad skull, huge forequarters and deep jaws speak of ambush and overpowering force.",
                "It moves with the silent certainty of a predator that expects the world to part before it.",
                "thick jungle, swamp and broken forest"),
            """
            Tigers are large predatory felids of grassland, forest edge, jungle, rocky scrub and broken cover. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The most telling cues are heavy shoulders, padded paws, deep jaws and muscle arranged for ambush and impact. Those features support a coiled, silent confidence that makes stillness feel dangerous, making the creature feel suited to its ground rather than merely placed there.

            They are not merely wildlife but territorial facts, the kind of animals that reshape routes, grazing patterns and local caution. A tiger can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Mammal("Sabretooth Tiger", "Sabretoothed", "Toed Quadruped", SizeCategory.Large, 1.5, "Big Felid",
            "standard-mammal", "big-cat",
            MammalPack("a sabretooth cub", "a young male sabretooth", "a young female sabretooth", "a male sabretooth", "a female sabretooth",
                "It is a hulking great cat, deep in the chest and powerful through the shoulders, with elongated upper fangs that dominate its face.",
                "Its forequarters are built for grappling impact rather than speed alone, and the paired sabres give it a terrifying silhouette.",
                "It carries itself with the confidence of a predator that ends struggles up close and violently.",
                "cold steppe, broken woodland and glacial plain"),
            """
            In grassland, forest edge, jungle, rocky scrub and broken cover, sabretooth tigers fill the role of large predatory felids. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Most of its character sits in heavy shoulders, padded paws, deep jaws and muscle arranged for ambush and impact. Those features support a coiled, silent confidence that makes stillness feel dangerous, making the creature feel suited to its ground rather than merely placed there.

            They are not merely wildlife but territorial facts, the kind of animals that reshape routes, grazing patterns and local caution. A sabretooth tiger can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, bloodProfile: "feline", combatStrategyKey: "Beast Brawler");
        yield return Mammal("Cheetah", "Cheetah", "Toed Quadruped", SizeCategory.Small, 0.8, "Big Felid",
            "standard-mammal", "wolfpack",
            MammalPack("a cheetah cub", "a young male cheetah", "a young female cheetah", "a male cheetah", "a female cheetah",
                "It is long-limbed and lightly built, with a spotted coat and dark tear-streaks beneath the eyes.",
                "Its narrow chest, fine waist and springy back look made for speed more than wrestling strength.",
                "It stands alert and taut, like something meant to explode into motion.",
                "open savannah and sparse scrub"),
            """
            Cheetahs occupy grassland, forest edge, jungle, rocky scrub and broken cover as large predatory felids. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            What distinguishes the line is heavy shoulders, padded paws, deep jaws and muscle arranged for ambush and impact. Those features support a coiled, silent confidence that makes stillness feel dangerous, making the creature feel suited to its ground rather than merely placed there.

            They are not merely wildlife but territorial facts, the kind of animals that reshape routes, grazing patterns and local caution. A cheetah can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Leopard", "Leopard", "Toed Quadruped", SizeCategory.Small, 0.8, "Big Felid",
            "standard-mammal", "wolfpack",
            MammalPack("a leopard cub", "a young male leopard", "a young female leopard", "a male leopard", "a female leopard",
                "It is compact, muscled and rosetted in gold and black.",
                "Its heavy shoulders and powerful neck suggest an animal built to drag kills into trees.",
                "It has the quiet, coiled air of a born ambush predator.",
                "forest, rocky scrub and broken woodland"),
            """
            Where grassland, forest edge, jungle, rocky scrub and broken cover meets daily life, leopards are recognisable as large predatory felids. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The creature is best read through heavy shoulders, padded paws, deep jaws and muscle arranged for ambush and impact. Those features support a coiled, silent confidence that makes stillness feel dangerous, making the creature feel suited to its ground rather than merely placed there.

            They are not merely wildlife but territorial facts, the kind of animals that reshape routes, grazing patterns and local caution. A leopard can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Panther", "Panther", "Toed Quadruped", SizeCategory.Small, 0.8, "Big Felid",
            "standard-mammal", "wolfpack",
            MammalPack("a panther cub", "a young male panther", "a young female panther", "a male panther", "a female panther",
                "It is sleek and dark-coated, its musculature visible beneath a glossy pelt.",
                "Its eyes and whiskered muzzle stand out sharply against its shadow-black fur.",
                "It moves like a piece of living night, smooth and deliberate.",
                "forest canopy, jungle edge and rocky cover"),
            """
            Panthers are large predatory felids of grassland, forest edge, jungle, rocky scrub and broken cover. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Its profile is built around heavy shoulders, padded paws, deep jaws and muscle arranged for ambush and impact. In motion they are marked by a coiled, silent confidence that makes stillness feel dangerous, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are not merely wildlife but territorial facts, the kind of animals that reshape routes, grazing patterns and local caution. Around settlements, roads or camps, panthers add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Jaguar", "Jaguar", "Toed Quadruped", SizeCategory.Small, 0.8, "Big Felid",
            "standard-mammal", "wolfpack",
            MammalPack("a jaguar cub", "a young male jaguar", "a young female jaguar", "a male jaguar", "a female jaguar",
                "It is thickset and broad-headed, patterned with heavy rosettes marked by central spots.",
                "Its jaws look unusually deep and powerful even for a great cat.",
                "It has a dense, forceful presence rather than a runner's lightness.",
                "rainforest, swamp and riverbank"),
            """
            In grassland, forest edge, jungle, rocky scrub and broken cover, jaguars fill the role of large predatory felids. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The creature presents heavy shoulders, padded paws, deep jaws and muscle arranged for ambush and impact. In motion they are marked by a coiled, silent confidence that makes stillness feel dangerous, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are not merely wildlife but territorial facts, the kind of animals that reshape routes, grazing patterns and local caution. Around settlements, roads or camps, jaguars add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Jackal", "Jackal", "Toed Quadruped", SizeCategory.Small, 0.8, "Small Canid",
            "standard-mammal", "doglike",
            MammalPack("a jackal pup", "a young male jackal", "a young female jackal", "a male jackal", "a female jackal",
                "It is a lean tawny canine with a narrow face and brush tail.",
                "Its tall ears and restless expression make it look perpetually alert for danger or carrion.",
                "It gives off a scavenger's cunning and a hunter's opportunism.",
                "dry plains, scrub and broken desert"),
            """
            Jackals are lean, sharp-sensed canid predators of woodland edges, scrub, dry plains and open wilderness. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: long muzzles, quick feet and a rangy build meant for scent, pursuit and opportunism. Watch one move and the emphasis falls on wary confidence and a habit of testing distance before committing, a pattern that tells more than size alone.

            Their presence at the edge of camp or pasture is enough to make livestock restless and travellers attentive. A jackal can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Mammal("Deer", "Cervine", "Ungulate", SizeCategory.Normal, 0.8, "Small Ungulate",
            "stock-mammal", "antlered-herbivore",
            MammalPack("a fawn", "a young stag", "a young doe", "a stag", "a doe",
                "It is slender and long-legged, with a neat coat and finely boned head.",
                "Its large ears, dark eyes and delicate muzzle give it a perpetually sensitive look.",
                "It stands ready to leap away at any sudden sound.",
                "woodland, meadow and forest edge"),
            """
            Where pastures, plains, woodland edges, marshes and cold open country meets daily life, deer are recognisable as hoofed grazing animals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The visible essentials are long legs, weight-bearing hooves and frames shaped for grazing, migration or sudden flight. In motion they are marked by herd-aware caution and a readiness to bolt, charge or hold ground according to size, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are central to hunting, herding and wilderness travel, familiar enough to be useful but never entirely harmless. Around settlements, roads or camps, deer add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, usages: antlered, combatStrategyKey: "Beast Coward");
        yield return Mammal("Moose", "Moose", "Ungulate", SizeCategory.Large, 1.5, "Large Ungulate",
            "stock-mammal", "antlered-herbivore",
            MammalPack("a moose calf", "a young bull moose", "a young cow moose", "a bull moose", "a cow moose",
                "It is towering and long-legged, with a heavy neck and dark shaggy coat.",
                "Its pendulous muzzle and deep shoulders make it look awkward until it begins to move.",
                "It has the placid bulk of an animal that can still become terrifyingly violent when pressed.",
                "swamp, taiga and cold forest"),
            """
            Moose are hoofed grazing animals of pastures, plains, woodland edges, marshes and cold open country. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by long legs, weight-bearing hooves and frames shaped for grazing, migration or sudden flight. The impression is completed by herd-aware caution and a readiness to bolt, charge or hold ground according to size, giving the animal a readable rhythm even before it acts.

            They are central to hunting, herding and wilderness travel, familiar enough to be useful but never entirely harmless. Around settlements, roads or camps, moose add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, usages: antlered, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Elk", "Elk", "Ungulate", SizeCategory.Large, 1.3, "Large Ungulate",
            "large-hooved", "antlered-herbivore",
            MammalPack("an elk calf", "a young bull elk", "a young cow elk", "a bull elk", "a cow elk",
                "It is tall-shouldered and deep-bodied, with a dark neck and long legs built for covering country.",
                "Its broad muzzle and sweeping antlers give it the look of a grazer that can still become dangerous in an instant.",
                "It holds itself with a rangy, watchful calm more suited to open woodland than close pens.",
                "mountain meadow, open forest and cold grassland"),
            """
            In pastures, plains, woodland edges, marshes and cold open country, elk fill the role of hoofed grazing animals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by long legs, weight-bearing hooves and frames shaped for grazing, migration or sudden flight. The impression is completed by herd-aware caution and a readiness to bolt, charge or hold ground according to size, giving the animal a readable rhythm even before it acts.

            They are central to hunting, herding and wilderness travel, familiar enough to be useful but never entirely harmless. Around settlements, roads or camps, elk add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, usages: antlered, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Reindeer", "Reindeer", "Ungulate", SizeCategory.Large, 1.1, "Large Ungulate",
            "stock-mammal", "antlered-herbivore",
            MammalPack("a reindeer calf", "a young bull reindeer", "a young cow reindeer", "a bull reindeer", "a cow reindeer",
                "It is compact and shaggy-coated, with broad hooves and a thick neck built for cold country.",
                "Its muzzle is blunt and furred, and even its antlers look shaped by hard seasons and long migration.",
                "It moves with steady endurance, as though distance and weather were ordinary inconveniences.",
                "tundra, lichen plain and northern woodland"),
            """
            Reindeer occupy pastures, plains, woodland edges, marshes and cold open country as hoofed grazing animals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by long legs, weight-bearing hooves and frames shaped for grazing, migration or sudden flight. The impression is completed by herd-aware caution and a readiness to bolt, charge or hold ground according to size, giving the animal a readable rhythm even before it acts.

            They are central to hunting, herding and wilderness travel, familiar enough to be useful but never entirely harmless. Around settlements, roads or camps, reindeer add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, usages: antleredGeneral, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Pig", "Porcine", "Ungulate", SizeCategory.Normal, 1.0, "Small Ungulate",
            "stock-mammal", "herbivore-charge",
            MammalPack("a piglet", "a young boar", "a young sow", "a boar", "a sow",
                "It is thick-bodied and snub-legged, with a broad snout and stiff bristles.",
                "Its rooting nose and wedge-shaped head make it look built to shove through earth and brush.",
                "It has a blunt, practical sort of confidence.",
                "farm pens, muddy woodland and scrub"),
            """
            In muddy woodland, scrub, farm pens and marsh edges, pigs fill the role of stout rooting ungulates. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: wedge-shaped heads, bristled hides, practical snouts and stubborn shoulders. Watch one move and the emphasis falls on blunt confidence and a habit of shoving through whatever stands in the way, a pattern that tells more than size alone.

            They are valuable as domestic animals or dangerous as wild animals, especially when tusks, young or food are involved. Around settlements, roads or camps, pigs add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, usages: udder);
        yield return Mammal("Boar", "Boar", "Ungulate", SizeCategory.Normal, 1.2, "Small Ungulate",
            "stock-mammal", "tusked-herbivore",
            MammalPack("a piglet", "a young boar", "a young sow", "a boar", "a sow",
                "It is broad-shouldered and bristled, with a wedge head and restless little eyes.",
                "Its protruding tusks and dense neck make it look ready to crash straight through trouble.",
                "It gives off the prickly aggression of a wild rooting animal.",
                "thick scrub, marsh edge and rough woodland"),
            usages: tuskedMale, combatStrategyKey: "Beast Behemoth",
            description: """
            Boars occupy muddy woodland, scrub, farm pens and marsh edges as stout rooting ungulates. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: wedge-shaped heads, bristled hides, practical snouts and stubborn shoulders. Watch one move and the emphasis falls on blunt confidence and a habit of shoving through whatever stands in the way, a pattern that tells more than size alone.

            They are valuable as domestic animals or dangerous as wild animals, especially when tusks, young or food are involved. Around settlements, roads or camps, boars add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Mammal("Warthog", "Warthog", "Ungulate", SizeCategory.Normal, 1.2, "Small Ungulate",
            "stock-mammal", "tusked-herbivore",
            MammalPack("a warthog piglet", "a young male warthog", "a young female warthog", "a male warthog", "a female warthog",
                "It is high-shouldered and sparse-haired, with a long face and ridged snout.",
                "Its curved tusks and facial bosses give it a rugged, ill-tempered look.",
                "It looks as though it would rather charge than retreat.",
                "dry savannah and thorn scrub"),
            usages: tuskedMale, combatStrategyKey: "Beast Behemoth",
            description: """
            Where muddy woodland, scrub, farm pens and marsh edges meets daily life, warthogs are recognisable as stout rooting ungulates. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: wedge-shaped heads, bristled hides, practical snouts and stubborn shoulders. Watch one move and the emphasis falls on blunt confidence and a habit of shoving through whatever stands in the way, a pattern that tells more than size alone.

            They are valuable as domestic animals or dangerous as wild animals, especially when tusks, young or food are involved. Around settlements, roads or camps, warthogs add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Mammal("Sheep", "Ovine", "Ungulate", SizeCategory.Normal, 0.8, "Small Ungulate",
            "stock-mammal", "bovid",
            MammalPack("a lamb", "a young ram", "a young ewe", "a ram", "a ewe",
                "It is woolly and round-bodied, with a narrow face and placid eyes.",
                "Its thick fleece makes it look larger and softer than the muscle beneath would suggest.",
                "It has the mild, herd-bound stillness of a grazing prey animal.",
                "pasture, upland meadow and folded hills"),
            """
            Sheep are sure-footed herd animals of pastures, crags, upland meadows and folded hills. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Its strongest visual signs are narrow faces, hooves built for uncertain ground, thick coats or fleece, and wary horizontal gazes. Those features support herd-bound caution mixed with stubborn, practical footing, making the creature feel suited to its ground rather than merely placed there.

            They mark the working edge of pastoral life, providing fibre, milk or meat while testing fences and patience. Around settlements, roads or camps, sheep add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, bloodProfile: "ovine", usages: hornedFemale, combatStrategyKey: "Beast Coward");
        yield return Mammal("Goat", "Caprine", "Ungulate", SizeCategory.Normal, 0.8, "Small Ungulate",
            "standard-mammal", "goat",
            MammalPack("a kid goat", "a young billy goat", "a young nanny goat", "a billy goat", "a nanny goat",
                "It is narrow-bodied and sure-footed, with a rough coat and bright horizontal pupils.",
                "Its beard, angular skull and sweeping horns give it a stubbornly self-possessed look.",
                "It looks happiest when climbing where other animals should not.",
                "crags, rough pasture and broken hills"),
            """
            In pastures, crags, upland meadows and folded hills, goats fill the role of sure-footed herd animals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The eye is drawn to narrow faces, hooves built for uncertain ground, thick coats or fleece, and wary horizontal gazes. Those features support herd-bound caution mixed with stubborn, practical footing, making the creature feel suited to its ground rather than merely placed there.

            They mark the working edge of pastoral life, providing fibre, milk or meat while testing fences and patience. Around settlements, roads or camps, goats add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, usages: hornedFemale, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Llama", "Llama", "Ungulate", SizeCategory.Normal, 1.0, "Large Ungulate",
            "stock-mammal", "camelid-spitter",
            MammalPack("a cria", "a young male llama", "a young female llama", "a male llama", "a female llama",
                "It is long-necked and narrow-faced, with a thick woolly coat and large watchful eyes.",
                "Its upright neck and neatly placed feet give it a wary, elegant silhouette.",
                "It holds itself with an aloof, steady calm.",
                "high pasture, dry plain and mountain track"),
            """
            Llamas occupy high pasture, dry plain, desert track and mountain road as long-necked, dry-country herd animals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The most telling cues are padded feet, long lashes, thick coats or humps and a frame made for distance. Those features support aloof patience and stubborn endurance, making the creature feel suited to its ground rather than merely placed there.

            They belong to caravan, pack and upland work, carrying themselves as if hardship were a normal condition of travel. Around settlements, roads or camps, llamas add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, usages: udder, combatStrategyKey: "Beast Artillery");
        yield return Mammal("Alpaca", "Alpaca", "Ungulate", SizeCategory.Normal, 0.8, "Large Ungulate",
            "stock-mammal", "camelid-spitter",
            MammalPack("a cria", "a young male alpaca", "a young female alpaca", "a male alpaca", "a female alpaca",
                "It is smaller and fluffier than a llama, with a dense soft fleece and compact body.",
                "Its rounded face and oversized eyes give it a gentle, almost toy-like look.",
                "It moves with quiet delicacy and herd-bred caution.",
                "cool pasture and upland meadow"),
            """
            Where high pasture, dry plain, desert track and mountain road meets daily life, alpacas are recognisable as long-necked, dry-country herd animals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Most of its character sits in padded feet, long lashes, thick coats or humps and a frame made for distance. Those features support aloof patience and stubborn endurance, making the creature feel suited to its ground rather than merely placed there.

            They belong to caravan, pack and upland work, carrying themselves as if hardship were a normal condition of travel. Around settlements, roads or camps, alpacas add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, usages: udder, combatStrategyKey: "Beast Artillery");
        yield return Mammal("Camel", "Camelid", "Ungulate", SizeCategory.Large, 1.2, "Horse",
            "large-hooved", "camelid-spitter",
            MammalPack("a camel calf", "a young bull camel", "a young cow camel", "a bull camel", "a cow camel",
                "It is high-backed and long-legged, with broad padded feet, long lashes and a heavy-lipped muzzle.",
                "Its deep chest and swaying hump make the whole frame look engineered for thirst, distance and bad terrain.",
                "It carries itself with patient hauteur and the stubborn composure of an animal that expects the world to be hot and difficult.",
                "desert caravan, dry steppe and stony waste"),
            """
            Camels are long-necked, dry-country herd animals of high pasture, dry plain, desert track and mountain road. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Look closer and the outline is all padded feet, long lashes, thick coats or humps and a frame made for distance. In motion they are marked by aloof patience and stubborn endurance, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They belong to caravan, pack and upland work, carrying themselves as if hardship were a normal condition of travel. For characters nearby, a camel is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """, usages: udder, combatStrategyKey: "Beast Artillery");
        yield return Mammal("Bear", "Ursine", "Toed Quadruped", SizeCategory.Large, 1.4, "Bear",
            "large-hooved", "bear",
            MammalPack("a bear cub", "a young male bear", "a young female bear", "a male bear", "a female bear",
                "It is huge and broad, with dense fur, thick shoulders and a heavy swinging gait.",
                "Its paws are enormous and its claws long enough to dig, tear and hold with dreadful force.",
                "It radiates blunt strength and the certainty that it is difficult to stop once angered.",
                "forest, mountain and rough northern country"),
            description: """
            In forest, mountain, bamboo thicket and rough northern country, bears fill the role of broad, powerful ursine mammals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The first things to register are dense fur, deep chests, massive paws and claws meant to dig, climb, tear and hold. In motion they are marked by slow weight until provoked, when the strength becomes immediate and alarming, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are respected at a distance, feared around stores and cubs, and remembered wherever the wilderness presses close. For characters nearby, a bear is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Mammal("Mouse", "Murine", "Toed Quadruped", SizeCategory.Tiny, 0.1, "Small Rodent",
            "tiny-fast", "nuisance-bite",
            MammalPack("a mouse pinkie", "a young buck mouse", "a young doe mouse", "a buck mouse", "a doe mouse",
                "It is tiny and fine-boned, with a pointed muzzle, soft fur and a whip-like tail.",
                "Its whiskers and oversized ears are constantly in motion.",
                "It looks born to dart from cover to cover and vanish into impossibly small gaps.",
                "granaries, burrows and the hidden corners of habitation"),
            combatStrategyKey: "Beast Coward",
            description: """
            Mice are small gnawing mammals of burrows, granaries, drains, cages, grass cover and hidden settlement corners. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by sharp incisors, twitching whiskers, quick feet and compact bodies built for sheltering in tight places. The impression is completed by nervous speed, constant investigation and sudden disappearance, giving the animal a readable rhythm even before it acts.

            They are pests, pets, prey or omens of neglect depending on where they appear and how many follow. For characters nearby, a mouse is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Mammal("Rat", "Rat", "Toed Quadruped", SizeCategory.Tiny, 0.2, "Small Rodent",
            "tiny-fast", "nuisance-bite",
            MammalPack("a rat pup", "a young buck rat", "a young doe rat", "a buck rat", "a doe rat",
                "It is narrow-bodied and coarse-furred, with bright eyes and a long scaly tail.",
                "Its blunt muzzle and heavy incisors give it a more robust look than a mouse.",
                "It moves with shabby confidence and constant low-level vigilance.",
                "sewers, granaries and ruined corners"),
            combatStrategyKey: "Beast Coward",
            description: """
            In burrows, granaries, drains, cages, grass cover and hidden settlement corners, rats fill the role of small gnawing mammals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by sharp incisors, twitching whiskers, quick feet and compact bodies built for sheltering in tight places. The impression is completed by nervous speed, constant investigation and sudden disappearance, giving the animal a readable rhythm even before it acts.

            They are pests, pets, prey or omens of neglect depending on where they appear and how many follow. For characters nearby, a rat is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Mammal("Hamster", "Cricetid", "Toed Quadruped", SizeCategory.Tiny, 0.1, "Small Rodent",
            "tiny-fast", "nuisance-bite",
            MammalPack("a hamster pup", "a young male hamster", "a young female hamster", "a male hamster", "a female hamster",
                "It is tiny, round-bodied and plush-coated, with cheek pouches that seem large for its size.",
                "Its small paws and blunt face make it look built for digging and hoarding.",
                "It gives off a nervous but stubborn domestic energy.",
                "burrows, cages and dry scrub"),
            combatStrategyKey: "Beast Coward",
            description: """
            Hamsters occupy burrows, granaries, drains, cages, grass cover and hidden settlement corners as small gnawing mammals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by sharp incisors, twitching whiskers, quick feet and compact bodies built for sheltering in tight places. The impression is completed by nervous speed, constant investigation and sudden disappearance, giving the animal a readable rhythm even before it acts.

            They are pests, pets, prey or omens of neglect depending on where they appear and how many follow. For characters nearby, a hamster is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Mammal("Guinea Pig", "Guinea Pig", "Toed Quadruped", SizeCategory.Tiny, 0.1, "Small Rodent",
            "tiny-fast", "nuisance-bite",
            MammalPack("a guinea pig pup", "a young boar guinea pig", "a young sow guinea pig", "a boar guinea pig", "a sow guinea pig",
                "It is plump, low to the ground and entirely tailless, wrapped in soft fur.",
                "Its bright eyes and twitching nose make it look gentle and perpetually concerned.",
                "It seems more likely to whistle in alarm than to stand its ground.",
                "grassy cover and domestic hutches"),
            combatStrategyKey: "Beast Coward",
            description: """
            Where burrows, granaries, drains, cages, grass cover and hidden settlement corners meets daily life, guinea pigs are recognisable as small gnawing mammals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by sharp incisors, twitching whiskers, quick feet and compact bodies built for sheltering in tight places. The impression is completed by nervous speed, constant investigation and sudden disappearance, giving the animal a readable rhythm even before it acts.

            They are pests, pets, prey or omens of neglect depending on where they appear and how many follow. For characters nearby, a guinea pig is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Mammal("Ferret", "Ferret", "Toed Quadruped", SizeCategory.VerySmall, 0.2, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a ferret kit", "a young hob ferret", "a young jill ferret", "a hob ferret", "a jill ferret",
                "It is long, narrow and supple, with short legs and silky fur.",
                "Its sharp little face and lively black eyes make it look permanently curious.",
                "It moves in quick, snaking bursts full of mischief and predatory interest.",
                "burrows, barns and cramped passages"),
            combatStrategyKey: "Beast Skirmisher",
            description: """
            Ferrets occupy burrows, barns, hedgerows, riverbanks, snow country and ruined walls as low, flexible small predators. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: long bodies, short legs, sharp faces and teeth that make their size easy to misjudge. Watch one move and the emphasis falls on snaking bursts of predatory curiosity, a pattern that tells more than size alone.

            They are tolerated for hunting vermin, cursed for raiding stores and admired uneasily for their fearless persistence. For characters nearby, a ferret is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Mammal("Stoat", "Stoat", "Toed Quadruped", SizeCategory.VerySmall, 0.15, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a stoat kit", "a young male stoat", "a young female stoat", "a male stoat", "a female stoat",
                "It is slim, fine-boned and bright-eyed, with a narrow muzzle and a tail tipped in black.",
                "Its little body is made for darting into holes after prey, and in cold regions its coat can pale almost to white.",
                "It twitches with predatory focus despite its size, every movement fast and exact.",
                "hedgerow, stone wall and snowy field"),
            """
            Where burrows, barns, hedgerows, riverbanks, snow country and ruined walls meets daily life, stoats are recognisable as low, flexible small predators. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: long bodies, short legs, sharp faces and teeth that make their size easy to misjudge. Watch one move and the emphasis falls on snaking bursts of predatory curiosity, a pattern that tells more than size alone.

            They are tolerated for hunting vermin, cursed for raiding stores and admired uneasily for their fearless persistence. For characters nearby, a stoat is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Weasel", "Weasel", "Toed Quadruped", SizeCategory.VerySmall, 0.2, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a weasel kit", "a young male weasel", "a young female weasel", "a male weasel", "a female weasel",
                "It is tiny, lithe and intensely streamlined, with a narrow head and fluid spine.",
                "Its teeth look too sharp for such a small face, and its paws are quick and clever.",
                "It gives the impression of restless, purposeful violence in miniature.",
                "hedgerow, burrow and rough grass"),
            """
            Weasels are low, flexible small predators of burrows, barns, hedgerows, riverbanks, snow country and ruined walls. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            What distinguishes the line is long bodies, short legs, sharp faces and teeth that make their size easy to misjudge. Those features support snaking bursts of predatory curiosity, making the creature feel suited to its ground rather than merely placed there.

            They are tolerated for hunting vermin, cursed for raiding stores and admired uneasily for their fearless persistence. For characters nearby, a weasel is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Polecat", "Polecat", "Toed Quadruped", SizeCategory.VerySmall, 0.25, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a polecat kit", "a young male polecat", "a young female polecat", "a male polecat", "a female polecat",
                "It is long-bodied and dark-masked, with coarse fur and a low, flexible frame.",
                "Its face is pointed and expressive, and the whole animal has the musky look of something that would rather bite than be cornered.",
                "It moves in low, sinuous bursts that make every tunnel and hedge-line look like cover.",
                "hedgebank, rough pasture and ruin"),
            """
            In burrows, barns, hedgerows, riverbanks, snow country and ruined walls, polecats fill the role of low, flexible small predators. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The creature is best read through long bodies, short legs, sharp faces and teeth that make their size easy to misjudge. Those features support snaking bursts of predatory curiosity, making the creature feel suited to its ground rather than merely placed there.

            They are tolerated for hunting vermin, cursed for raiding stores and admired uneasily for their fearless persistence. For characters nearby, a polecat is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Mink", "Mink", "Toed Quadruped", SizeCategory.VerySmall, 0.25, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a mink kit", "a young male mink", "a young female mink", "a male mink", "a female mink",
                "It is sleek and dark-furred, with a supple body, neat ears and bright, hard eyes.",
                "Its long tail and slightly webbed feet hint at a hunter equally comfortable on bank or in water.",
                "It gives off the quiet intent of a predator that has never needed to be large to be dangerous.",
                "riverbank, marsh edge and reed-choked pool"),
            """
            Minks occupy burrows, barns, hedgerows, riverbanks, snow country and ruined walls as low, flexible small predators. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Its strongest visual signs are long bodies, short legs, sharp faces and teeth that make their size easy to misjudge. Those features support snaking bursts of predatory curiosity, making the creature feel suited to its ground rather than merely placed there.

            They are tolerated for hunting vermin, cursed for raiding stores and admired uneasily for their fearless persistence. For characters nearby, a mink is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Shrew", "Shrew", "Toed Quadruped", SizeCategory.Tiny, 0.1, "Small Rodent",
            "tiny-fast", "small-predator",
            MammalPack("a shrew pup", "a young male shrew", "a young female shrew", "a male shrew", "a female shrew",
                "It is tiny and sharp-snouted, with velvety fur and a body that seems perpetually in motion.",
                "Its needle teeth and frantic little paws make it look far too fierce for something so small.",
                "It lives at a pitch of constant hunger and nervous violence, never still for long.",
                "leaf litter, root tangle and damp grass"),
            """
            Shrews are small gnawing mammals of burrows, granaries, drains, cages, grass cover and hidden settlement corners. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: sharp incisors, twitching whiskers, quick feet and compact bodies built for sheltering in tight places. Watch one move and the emphasis falls on nervous speed, constant investigation and sudden disappearance, a pattern that tells more than size alone.

            They are pests, pets, prey or omens of neglect depending on where they appear and how many follow. For characters nearby, a shrew is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Badger", "Badger", "Toed Quadruped", SizeCategory.Small, 0.8, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a badger cub", "a young boar badger", "a young sow badger", "a boar badger", "a sow badger",
                "It is low, stocky and massively built for digging, with coarse fur and a striped face.",
                "Its shoulders and foreclaws are thick and workmanlike, made for earth and stubborn labor.",
                "It has the dour, immovable look of an animal that dislikes being disturbed.",
                "setts, hedgebanks and mixed woodland"),
            """
            Where burrows, barns, hedgerows, riverbanks, snow country and ruined walls meets daily life, badgers are recognisable as low, flexible small predators. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The eye is drawn to long bodies, short legs, sharp faces and teeth that make their size easy to misjudge. Those features support snaking bursts of predatory curiosity, making the creature feel suited to its ground rather than merely placed there.

            They are tolerated for hunting vermin, cursed for raiding stores and admired uneasily for their fearless persistence. For characters nearby, a badger is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Mammal("Wolverine", "Wolverine", "Toed Quadruped", SizeCategory.Small, 0.5, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a wolverine cub", "a young male wolverine", "a young female wolverine", "a male wolverine", "a female wolverine",
                "It is broad-pawed and thick-furred, with a heavy neck and deep-set eyes.",
                "Its teeth and shoulders make it look disproportionately formidable for its size.",
                "It radiates surly endurance and a willingness to fight far above its weight.",
                "snow country, tundra and cold forest"),
            """
            Wolverines are low, flexible small predators of burrows, barns, hedgerows, riverbanks, snow country and ruined walls. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            At a glance, the form resolves into long bodies, short legs, sharp faces and teeth that make their size easy to misjudge. In motion they are marked by snaking bursts of predatory curiosity, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are tolerated for hunting vermin, cursed for raiding stores and admired uneasily for their fearless persistence. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Mammal("Cow", "Bovine", "Ungulate", SizeCategory.Large, 1.5, "Large Ungulate",
            "stock-mammal", "bovid",
            MammalPack("a calf", "a young bull", "a young cow", "a bull", "a cow",
                "It is large and broad-bodied, with a heavy gut and a blunt moist muzzle.",
                "Its calm eyes and sweeping ribs speak of a lifetime spent grazing and ruminating.",
                "It has the placid, heavy patience of domestic stock.",
                "pasture, open field and cattle yard"),
            """
            In pasture, prairie, marsh, open range and cattle yards, cows fill the role of heavy-bodied horned grazers. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The visible essentials are broad chests, dense shoulders, sweeping horns and heavy necks built for mass rather than grace. In motion they are marked by slow patience backed by sudden, punishing force, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They anchor pastoral life and frontier danger alike, becoming ordinary only to those who forget how much weight they carry. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, bloodProfile: "bovine", usages: hornedFemale, combatStrategyKey: "Beast Coward");
        yield return Mammal("Ox", "Ox", "Ungulate", SizeCategory.Large, 1.7, "Large Ungulate",
            "stock-mammal", "bovid",
            MammalPack("a calf", "a young bull", "a young cow", "an ox", "a cow",
                "It is broad-backed and deep-chested, with strong shoulders and a thick neck.",
                "Its sheer frame suggests hauling power rather than speed or elegance.",
                "It stands with the settled endurance of a working beast.",
                "field, road and draft yard"),
            """
            Oxen occupy pasture, prairie, marsh, open range and cattle yards as heavy-bodied horned grazers. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Look closer and the outline is all broad chests, dense shoulders, sweeping horns and heavy necks built for mass rather than grace. In motion they are marked by slow patience backed by sudden, punishing force, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They anchor pastoral life and frontier danger alike, becoming ordinary only to those who forget how much weight they carry. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, usages: hornedFemale, combatStrategyKey: "Beast Brawler");
        yield return Mammal("Bison", "Bison", "Ungulate", SizeCategory.Large, 1.5, "Large Ungulate",
            "stock-mammal", "bovid",
            MammalPack("a bison calf", "a young bull bison", "a young cow bison", "a bull bison", "a cow bison",
                "It is massive in front, thick-maned and heavy headed, tapering behind into lighter hindquarters.",
                "Its dense shoulder hump and horned brow make it look like a wall of living muscle.",
                "It has a slow, gathered momentum that hints at tremendous explosive power.",
                "prairie, steppe and open range"),
            """
            Where pasture, prairie, marsh, open range and cattle yards meets daily life, bison are recognisable as heavy-bodied horned grazers. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The first things to register are broad chests, dense shoulders, sweeping horns and heavy necks built for mass rather than grace. In motion they are marked by slow patience backed by sudden, punishing force, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They anchor pastoral life and frontier danger alike, becoming ordinary only to those who forget how much weight they carry. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, usages: hornedFemale, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Buffalo", "Buffalo", "Ungulate", SizeCategory.Large, 1.5, "Large Ungulate",
            "stock-mammal", "bovid",
            MammalPack("a buffalo calf", "a young bull buffalo", "a young cow buffalo", "a bull buffalo", "a cow buffalo",
                "It is dense and dark-coated, with a broad chest and sweeping horns.",
                "Its thick hide and low-slung head make it look like an animal built to weather punishment.",
                "It stands with a dour, dangerous calm.",
                "marsh, floodplain and rough pasture"),
            """
            Buffalo are heavy-bodied horned grazers of pasture, prairie, marsh, open range and cattle yards. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by broad chests, dense shoulders, sweeping horns and heavy necks built for mass rather than grace. The impression is completed by slow patience backed by sudden, punishing force, giving the animal a readable rhythm even before it acts.

            They anchor pastoral life and frontier danger alike, becoming ordinary only to those who forget how much weight they carry. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, usages: hornedFemale, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Horse", "Equine", "Ungulate", SizeCategory.Large, 1.5, "Horse",
            "large-hooved", "herbivore-charge",
            MammalPack("a foal", "a colt", "a filly", "a stallion", "a mare",
                "It is deep-bodied and strong-legged, with a flowing mane and powerful hindquarters.",
                "Its long face, lively ears and large dark eyes give it an intelligent, responsive look.",
                "It carries itself with a mixture of high-strung energy and practiced athleticism.",
                "plain, pasture and stable"),
            """
            In plains, stables, pack trails, roads and open pasture, horses fill the role of strong-legged working and riding animals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by deep chests, long faces, powerful hindquarters and limbs made for travel. The impression is completed by athletic alertness, endurance and a social awareness that rewards skilled handling, giving the animal a readable rhythm even before it acts.

            They are companions of road, war and labour, valuable precisely because speed and trust are never completely separate. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, canClimb: false, bloodProfile: "equine", usages: udder, combatStrategyKey: "Beast Physical Avoider");
        yield return Mammal("Donkey", "Asinine", "Ungulate", SizeCategory.Normal, 1.2, "Donkey",
            "large-hooved", "herbivore-charge",
            MammalPack("a donkey foal", "a young jack donkey", "a young jenny donkey", "a jack donkey", "a jenny donkey",
                "It is compact and strong-boned, with long ears, a narrow mane and tough little hooves.",
                "Its deep chest, sure-footed stance and patient eyes give it the look of an animal made for dry tracks and difficult loads.",
                "It carries itself with wary stubbornness and a surprising reserve of endurance.",
                "scrub pasture, stony road and stable yard"),
            """
            Donkeys occupy plains, stables, pack trails, roads and open pasture as strong-legged working and riding animals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by deep chests, long faces, powerful hindquarters and limbs made for travel. The impression is completed by athletic alertness, endurance and a social awareness that rewards skilled handling, giving the animal a readable rhythm even before it acts.

            They are companions of road, war and labour, valuable precisely because speed and trust are never completely separate. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, canClimb: false, bloodProfile: "equine", usages: udder, combatStrategyKey: "Beast Physical Avoider");
        yield return Mammal("Mule", "Mule", "Ungulate", SizeCategory.Large, 1.4, "Mule",
            "large-hooved", "herbivore-charge",
            MammalPack("a mule foal", "a young john mule", "a young molly mule", "a john mule", "a molly mule",
                "It is rangy and powerfully made, with the clean limbs of a horse, the long ears of a donkey and a hard-working frame.",
                "Its strong back, dense muscle and sure-footed balance make it look bred for loads, slopes and long unfashionable roads.",
                "It stands with practical patience, but every line suggests that forcing it would be harder than persuading it.",
                "pack trail, mountain road and working stable"),
            """
            Where plains, stables, pack trails, roads and open pasture meets daily life, mules are recognisable as strong-legged working and riding animals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by deep chests, long faces, powerful hindquarters and limbs made for travel. The impression is completed by athletic alertness, endurance and a social awareness that rewards skilled handling, giving the animal a readable rhythm even before it acts.

            They are companions of road, war and labour, valuable precisely because speed and trust are never completely separate. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, canClimb: false, bloodProfile: "equine", usages: udder, combatStrategyKey: "Beast Physical Avoider");
        yield return Mammal("Rhinocerous", "Ceratorhine", "Ungulate", SizeCategory.Large, 1.5, "Pachyderm",
            "large-hooved", "rhino",
            MammalPack("a rhino calf", "a young bull rhino", "a young cow rhino", "a bull rhino", "a cow rhino",
                "It is enormous and slab-sided, wrapped in thick folded hide like natural armour.",
                "Its snout carries a formidable horn and its barrel body looks difficult to injure or divert.",
                "It gives off an air of dangerous, heavy indifference.",
                "grassland, thorn scrub and muddy wallow"),
            """
            Rhinoceroses are enormous thick-skinned giants of savannah, forest edge, tundra, mudbank and watering place. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: pillar limbs, immense heads, heavy hides and imposing tusks, horns or trunks. Watch one move and the emphasis falls on deliberate momentum that makes their size feel like terrain in motion, a pattern that tells more than size alone.

            They are never background animals; wherever they live, roads, fences, armies and settlements must account for them. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, usages: rhinoHorn, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Hippopotamus", "Hippopotamine", "Ungulate", SizeCategory.Large, 1.5, "Pachyderm",
            "stock-mammal", "hippo",
            MammalPack("a hippo calf", "a young bull hippo", "a young cow hippo", "a bull hippo", "a cow hippo",
                "It is barrel-shaped and heavy-skinned, with a huge head and tiny watchful ears.",
                "Its mouth is absurdly broad, and the tusks within it look more than capable of maiming anything nearby.",
                "It has the bad-tempered stillness of an animal secure in its size.",
                "river, mudbank and reed-choked water"),
            """
            In savannah, forest edge, tundra, mudbank and watering place, hippopotamuses fill the role of enormous thick-skinned giants. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: pillar limbs, immense heads, heavy hides and imposing tusks, horns or trunks. Watch one move and the emphasis falls on deliberate momentum that makes their size feel like terrain in motion, a pattern that tells more than size alone.

            They are never background animals; wherever they live, roads, fences, armies and settlements must account for them. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, usages: tuskedGeneral, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Elephant", "Elephantine", "Ungulate", SizeCategory.VeryLarge, 2.0, "Pachyderm",
            "elephant", "elephant",
            MammalPack("an elephant calf", "a young bull elephant", "a young cow elephant", "a bull elephant", "a cow elephant",
                "It is colossal, grey-skinned and vast, with pillar legs and an immense domed head.",
                "Its trunk, fanlike ears and sweeping ivory tusks make it one of the most imposing animals imaginable.",
                "It moves with slow certainty, each step suggesting immense restrained force.",
                "savannah, forest edge and watering place"),
            """
            Elephants occupy savannah, forest edge, tundra, mudbank and watering place as enormous thick-skinned giants. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: pillar limbs, immense heads, heavy hides and imposing tusks, horns or trunks. Watch one move and the emphasis falls on deliberate momentum that makes their size feel like terrain in motion, a pattern that tells more than size alone.

            They are never background animals; wherever they live, roads, fences, armies and settlements must account for them. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, usages: tuskedGeneral, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Mammoth", "Mammothine", "Ungulate", SizeCategory.VeryLarge, 2.2, "Pachyderm",
            "elephant", "elephant",
            MammalPack("a mammoth calf", "a young bull mammoth", "a young cow mammoth", "a bull mammoth", "a cow mammoth",
                "It is immense and shaggy, its towering frame wrapped in long coarse hair above pillar-like legs.",
                "Its domed skull, humped shoulders and sweeping tusks make it look like winter itself learned to charge.",
                "It moves with heavy, deliberate momentum, each stride carrying the patient force of an age-old giant.",
                "tundra, glacial plain and cold steppe"),
            """
            Where savannah, forest edge, tundra, mudbank and watering place meets daily life, mammoths are recognisable as enormous thick-skinned giants. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: pillar limbs, immense heads, heavy hides and imposing tusks, horns or trunks. Watch one move and the emphasis falls on deliberate momentum that makes their size feel like terrain in motion, a pattern that tells more than size alone.

            They are never background animals; wherever they live, roads, fences, armies and settlements must account for them. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, usages: tuskedGeneral, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Oliphant", "Oliphantine", "Ungulate", SizeCategory.Huge, 3.0, "Oliphant",
            "elephant", "elephant",
            MammalPack("an oliphant calf", "a young bull oliphant", "a young cow oliphant", "a bull oliphant", "a cow oliphant",
                "It is an overwhelming elephantine giant, with pillar legs, a high war-tower back and tusks long enough to dominate the whole front of the beast.",
                "Its sheer height and thick hide make ordinary elephants look almost modest beside it.",
                "It moves with slow, ground-shaking confidence, built to scatter formations and carry heavy burdens through dust, heat and battle noise.",
                "broad savannah, dry steppe and war-camp stockyards"),
            """
            Oliphants are enormous thick-skinned giants of savannah, forest edge, tundra, mudbank and watering place. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The most telling cues are pillar limbs, immense heads, heavy hides and imposing tusks, horns or trunks. Those features support deliberate momentum that makes their size feel like terrain in motion, making the creature feel suited to its ground rather than merely placed there.

            They are never background animals; wherever they live, roads, fences, armies and settlements must account for them. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, usages: tuskedGeneral, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Giraffe", "Giraffine", "Ungulate", SizeCategory.VeryLarge, 1.0, "Large Ungulate",
            "large-hooved", "herbivore-charge",
            MammalPack("a giraffe calf", "a young bull giraffe", "a young cow giraffe", "a bull giraffe", "a cow giraffe",
                "It is immensely tall, with a spotted coat, long legs and an even longer neck.",
                "Its small horn-like ossicones and impossibly elevated head give it a peculiar dignity.",
                "It lopes rather than walks, with an awkward grace all its own.",
                "dry savannah and scattered acacia country"),
            combatStrategyKey: "Beast Physical Avoider",
            description: """
            Where pastures, plains, woodland edges, marshes and cold open country meets daily life, giraffes are recognisable as hoofed grazing animals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by long legs, weight-bearing hooves and frames shaped for grazing, migration or sudden flight. The impression is completed by herd-aware caution and a readiness to bolt, charge or hold ground according to size, giving the animal a readable rhythm even before it acts.

            They are central to hunting, herding and wilderness travel, familiar enough to be useful but never entirely harmless. Around settlements, roads or camps, giraffes add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Mammal("Giant Panda", "Panda", "Toed Quadruped", SizeCategory.Normal, 1.1, "Bear",
            "stock-mammal", "bear",
            MammalPack("a panda cub", "a young male panda", "a young female panda", "a male giant panda", "a female giant panda",
                "It is round-bodied and black-and-white, with heavy paws and a broad blunt face.",
                "Its powerful jaws and thick forelimbs look built for stripping bamboo as much as for defence.",
                "It carries itself with shambling calm, but the bulk behind that gentleness is obvious.",
                "misty bamboo forest and high mountain woodland"),
            """
            Giant pandas occupy forest, mountain, bamboo thicket and rough northern country as broad, powerful ursine mammals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Its profile is built around dense fur, deep chests, massive paws and claws meant to dig, climb, tear and hold. In motion they are marked by slow weight until provoked, when the strength becomes immediate and alarming, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are respected at a distance, feared around stores and cubs, and remembered wherever the wilderness presses close. For characters nearby, a giant panda is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """, canClimb: true, combatStrategyKey: "Beast Behemoth");
        yield return Mammal("Red Panda", "Ailurine", "Toed Quadruped", SizeCategory.VerySmall, 0.4, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a red panda cub", "a young male red panda", "a young female red panda", "a male red panda", "a female red panda",
                "It is small and russet-furred, with a masked face and a long ringed tail.",
                "Its neat paws and flexible body make it look at home among branches and cold forest cover.",
                "It moves with quiet, treewise caution.",
                "temperate bamboo forest and mossy highland woodland"),
            """
            Where forest, mountain, bamboo thicket and rough northern country meets daily life, red pandas are recognisable as broad, powerful ursine mammals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The creature presents dense fur, deep chests, massive paws and claws meant to dig, climb, tear and hold. In motion they are marked by slow weight until provoked, when the strength becomes immediate and alarming, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are respected at a distance, feared around stores and cubs, and remembered wherever the wilderness presses close. For characters nearby, a red panda is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """, canClimb: true, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Macaque", "Macacine", "Toed Quadruped", SizeCategory.Small, 0.7, "Primate",
            "standard-mammal", "small-predator",
            MammalPack("a macaque infant", "a young male macaque", "a young female macaque", "a male macaque", "a female macaque",
                "It is compact and quick-limbed, with a clever face, grasping paws and a restless tail.",
                "Its eyes are bright and socially watchful, missing little of what happens nearby.",
                "It carries itself with bold curiosity and the quick temper of a troop animal.",
                "forest edge, temple grove and rocky mountain woodland"),
            """
            In forest edges, temple groves, rocky woodland and settlement margins, macaques fill the role of quick-limbed social primates. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Most of its character sits in grasping hands, mobile faces, watchful eyes and bodies suited to climbing and quarrelling. Those features support bold curiosity, troop awareness and sudden temper, making the creature feel suited to its ground rather than merely placed there.

            They are close enough to seem clever and far enough from people to make that cleverness troublesome. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, canClimb: true, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Capybara", "Capybara", "Toed Quadruped", SizeCategory.Small, 0.7, "Capybara",
            "standard-mammal", "small-herbivore",
            MammalPack("a capybara pup", "a young male capybara", "a young female capybara", "a male capybara", "a female capybara",
                "It is barrel-bodied and blunt-headed, with coarse brown fur and small rounded ears.",
                "Its webbed feet and placid eyes suit a grazer that treats water as easy refuge.",
                "It has a composed, social calm that makes its size feel less threatening than practical.",
                "riverbank, marsh grass and flooded savannah"),
            """
            In burrows, granaries, drains, cages, grass cover and hidden settlement corners, capybaras fill the role of small gnawing mammals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: sharp incisors, twitching whiskers, quick feet and compact bodies built for sheltering in tight places. Watch one move and the emphasis falls on nervous speed, constant investigation and sudden disappearance, a pattern that tells more than size alone.

            They are pests, pets, prey or omens of neglect depending on where they appear and how many follow. For characters nearby, a capybara is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """, combatStrategyKey: "Beast Coward");
        yield return Mammal("Sloth", "Folivoran", "Toed Quadruped", SizeCategory.Small, 0.5, "Sloth",
            "standard-mammal", "small-herbivore",
            MammalPack("a sloth infant", "a young male sloth", "a young female sloth", "a male sloth", "a female sloth",
                "It is shaggy and long-limbed, with curved claws and a face fixed in mild patience.",
                "Its whole frame seems arranged for hanging from branches rather than hurrying across ground.",
                "It moves with deliberate slowness, spending motion as though every gesture must matter.",
                "rainforest canopy and humid river forest"),
            """
            Where riverbanks, rainforest floors, termite scrub and dry woodland meets daily life, sloths are recognisable as unusual mammals with highly specialised bodies. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The visible essentials are distinctive snouts, claws, armour, bills or tails that make their habits visible at a glance. In motion they are marked by methodical caution and strange efficiency, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They tend to be remembered by silhouette first, then by the trouble or wonder their specialisation creates. A sloth can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, canClimb: true, combatStrategyKey: "Beast Coward");
        yield return Mammal("Anteater", "Myrmecophagine", "Toed Quadruped", SizeCategory.Normal, 0.8, "Anteater",
            "standard-mammal", "small-predator",
            MammalPack("an anteater pup", "a young male anteater", "a young female anteater", "a male anteater", "a female anteater",
                "It has a long tubular snout, coarse fur and enormous curved foreclaws.",
                "Its narrow head and heavy claws look specialised for opening mounds and defending itself awkwardly but seriously.",
                "It snuffles with intent, as though scent matters more than almost anything it sees.",
                "savannah, gallery forest and termite-rich scrub"),
            """
            Anteaters are unusual mammals with highly specialised bodies of riverbanks, rainforest floors, termite scrub and dry woodland. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by distinctive snouts, claws, armour, bills or tails that make their habits visible at a glance. The impression is completed by methodical caution and strange efficiency, giving the animal a readable rhythm even before it acts.

            They tend to be remembered by silhouette first, then by the trouble or wonder their specialisation creates. A anteater can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Armadillo", "Dasypodid", "Toed Quadruped", SizeCategory.VerySmall, 0.5, "Armadillo",
            "standard-mammal", "small-herbivore",
            MammalPack("an armadillo pup", "a young male armadillo", "a young female armadillo", "a male armadillo", "a female armadillo",
                "It is low and plated, with a tapering snout and small digging claws.",
                "Its banded armour gives it the look of a burrowing creature wrapped in natural mail.",
                "It noses along close to the ground, cautious and methodical.",
                "dry scrub, grassland and forest floor"),
            """
            In riverbanks, rainforest floors, termite scrub and dry woodland, armadillos fill the role of unusual mammals with highly specialised bodies. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by distinctive snouts, claws, armour, bills or tails that make their habits visible at a glance. The impression is completed by methodical caution and strange efficiency, giving the animal a readable rhythm even before it acts.

            They tend to be remembered by silhouette first, then by the trouble or wonder their specialisation creates. A armadillo can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, combatStrategyKey: "Beast Coward");
        yield return Mammal("Tapir", "Tapirine", "Ungulate", SizeCategory.Normal, 1.0, "Large Ungulate",
            "stock-mammal", "herbivore-charge",
            MammalPack("a tapir calf", "a young male tapir", "a young female tapir", "a male tapir", "a female tapir",
                "It is sturdy and thick-bodied, with a short flexible trunk and rounded ears.",
                "Its compact power and padded feet make it look like a forest animal made to push through dense cover.",
                "It has a shy, heavy-footed caution until startled into surprising force.",
                "rainforest, swamp margin and shadowed river trail"),
            """
            Tapirs are hoofed grazing animals of pastures, plains, woodland edges, marshes and cold open country. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: long legs, weight-bearing hooves and frames shaped for grazing, migration or sudden flight. Watch one move and the emphasis falls on herd-aware caution and a readiness to bolt, charge or hold ground according to size, a pattern that tells more than size alone.

            They are central to hunting, herding and wilderness travel, familiar enough to be useful but never entirely harmless. Around settlements, roads or camps, tapirs add practical consequences: food, noise, labour, risk, nuisance or warning.
            """, combatStrategyKey: "Beast Physical Avoider");
        yield return Mammal("Kangaroo", "Macropod", "Toed Quadruped", SizeCategory.Normal, 0.9, "Kangaroo",
            "stock-mammal", "small-herbivore",
            MammalPack("a joey", "a young buck kangaroo", "a young doe kangaroo", "a buck kangaroo", "a doe kangaroo",
                "It is upright and long-tailed, with powerful hindquarters and small forepaws.",
                "Its feet, tail and deep haunches make it look built for bounding distance over open country.",
                "It holds itself alert and spring-loaded, ready to flee or lash out if pressed.",
                "dry woodland, scrubland and open plain"),
            """
            Kangaroos occupy dry woodland, scrub, gum forest, burrows and open plains as distinctive pouched mammals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            What distinguishes the line is specialised paws, tails, hindquarters or climbing grips suited to their particular niche. Those features support watchful, energy-conserving movement broken by sudden bounds, climbs or stubborn shoves, making the creature feel suited to its ground rather than merely placed there.

            They make a landscape feel particular, carrying local habits that travellers quickly learn not to treat as ordinary livestock. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, combatStrategyKey: "Beast Physical Avoider");
        yield return Mammal("Wallaby", "Macropod", "Toed Quadruped", SizeCategory.Small, 0.7, "Kangaroo",
            "stock-mammal", "small-herbivore",
            MammalPack("a joey", "a young buck wallaby", "a young doe wallaby", "a buck wallaby", "a doe wallaby",
                "It is compact and long-tailed, with large hind feet and a neat, alert head.",
                "Its body is the smaller cousin of a kangaroo's, made for quick bounds through brush and rock.",
                "It looks wary, nimble and ready to vanish into cover.",
                "brushland, rocky scrub and open forest"),
            """
            Where dry woodland, scrub, gum forest, burrows and open plains meets daily life, wallabies are recognisable as distinctive pouched mammals. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The creature is best read through specialised paws, tails, hindquarters or climbing grips suited to their particular niche. Those features support watchful, energy-conserving movement broken by sudden bounds, climbs or stubborn shoves, making the creature feel suited to its ground rather than merely placed there.

            They make a landscape feel particular, carrying local habits that travellers quickly learn not to treat as ordinary livestock. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """, combatStrategyKey: "Beast Physical Avoider");
        yield return Mammal("Koala", "Phascolarctid", "Toed Quadruped", SizeCategory.Small, 0.5, "Koala",
            "standard-mammal", "small-herbivore",
            MammalPack("a koala joey", "a young male koala", "a young female koala", "a male koala", "a female koala",
                "It is round, grey and woolly, with a dark leathery nose and strong grasping paws.",
                "Its claws and compact body make it look made for clinging to trunks and high branches.",
                "It seems sleepy and particular, wrapped in the calm of an animal that trusts its tree.",
                "eucalyptus woodland and open gum forest"),
            """
            Koalas are distinctive pouched mammals of dry woodland, scrub, gum forest, burrows and open plains. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Its profile is built around specialised paws, tails, hindquarters or climbing grips suited to their particular niche. In motion they are marked by watchful, energy-conserving movement broken by sudden bounds, climbs or stubborn shoves, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They make a landscape feel particular, carrying local habits that travellers quickly learn not to treat as ordinary livestock. A koala can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, canClimb: true, combatStrategyKey: "Beast Coward");
        yield return Mammal("Wombat", "Vombatid", "Toed Quadruped", SizeCategory.Small, 0.8, "Wombat",
            "standard-mammal", "small-herbivore",
            MammalPack("a wombat joey", "a young male wombat", "a young female wombat", "a male wombat", "a female wombat",
                "It is squat, muscular and blunt-faced, with dense fur and sturdy digging claws.",
                "Its low body and heavy shoulders look engineered for burrows and stubborn defence.",
                "It moves with practical determination and very little interest in being hurried.",
                "burrow, dry woodland and grassy scrub"),
            """
            In dry woodland, scrub, gum forest, burrows and open plains, wombats fill the role of distinctive pouched mammals. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The creature presents specialised paws, tails, hindquarters or climbing grips suited to their particular niche. In motion they are marked by watchful, energy-conserving movement broken by sudden bounds, climbs or stubborn shoves, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They make a landscape feel particular, carrying local habits that travellers quickly learn not to treat as ordinary livestock. A wombat can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, combatStrategyKey: "Beast Brawler");
        yield return Mammal("Platypus", "Monotreme", "Toed Quadruped", SizeCategory.VerySmall, 0.3, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a puggle", "a young male platypus", "a young female platypus", "a male platypus", "a female platypus",
                "It is low and sleek, with dense brown fur, webbed feet and a broad duck-like bill.",
                "Its odd mix of bill, tail and swimming paws makes it look assembled for quiet river hunting.",
                "It seems shy and secretive, more comfortable beneath rippled water than on open ground.",
                "freshwater creek, burrowed bank and shaded pool"),
            """
            Platypuses occupy riverbanks, rainforest floors, termite scrub and dry woodland as unusual mammals with highly specialised bodies. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by distinctive snouts, claws, armour, bills or tails that make their habits visible at a glance. The impression is completed by methodical caution and strange efficiency, giving the animal a readable rhythm even before it acts.

            They tend to be remembered by silhouette first, then by the trouble or wonder their specialisation creates. A platypus can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Tasmanian Devil", "Dasyurid", "Toed Quadruped", SizeCategory.Small, 0.8, "Mustelid",
            "standard-mammal", "small-predator",
            MammalPack("a Tasmanian devil joey", "a young male Tasmanian devil", "a young female Tasmanian devil", "a male Tasmanian devil", "a female Tasmanian devil",
                "It is dark, stocky and heavy-jawed, with a pale chest mark and a blunt restless face.",
                "Its head and teeth look oversized for its body, made for tearing carrion and quarrelling over scraps.",
                "It radiates noisy, low-slung ferocity.",
                "scrub, forest edge and carrion-rich night country"),
            """
            Tasmanian devils occupy dry woodland, scrub, gum forest, burrows and open plains as distinctive pouched mammals. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            At a glance, the form resolves into specialised paws, tails, hindquarters or climbing grips suited to their particular niche. In motion they are marked by watchful, energy-conserving movement broken by sudden bounds, climbs or stubborn shoves, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They make a landscape feel particular, carrying local habits that travellers quickly learn not to treat as ordinary livestock. A tasmanian devil can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, combatStrategyKey: "Beast Skirmisher");
        yield return Mammal("Dingo", "Canine", "Toed Quadruped", SizeCategory.Small, 0.8, "Small Canid",
            "standard-mammal", "doglike",
            MammalPack("a dingo pup", "a young male dingo", "a young female dingo", "a male dingo", "a female dingo",
                "It is lean and tawny, with upright ears, a narrow muzzle and rangy desert-bred limbs.",
                "Its alert eyes and efficient frame give it the look of a wild canid built for distance and opportunism.",
                "It watches with wary confidence, neither tame nor wastefully aggressive.",
                "dry woodland, spinifex plain and desert edge"),
            """
            In woodland edges, scrub, dry plains and open wilderness, dingos fill the role of lean, sharp-sensed canid predators. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: long muzzles, quick feet and a rangy build meant for scent, pursuit and opportunism. Watch one move and the emphasis falls on wary confidence and a habit of testing distance before committing, a pattern that tells more than size alone.

            Their presence at the edge of camp or pasture is enough to make livestock restless and travellers attentive. A dingo can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """, bloodProfile: "canine", combatStrategyKey: "Beast Skirmisher");
    }

    private void SeedQuadruped(BodyProto quadrupedBody, BodyProto ungulateBody, BodyProto toedQuadruped)
    {
        ResetCachedParts();
        int order = 1;
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodyparts...");

        #region Torso

        AddBodypart(quadrupedBody, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null,
            Alignment.Front, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "rbreast", "right breast", "breast", BodypartTypeEnum.Wear, "abdomen",
            Alignment.FrontRight, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "lbreast", "left breast", "breast", BodypartTypeEnum.Wear, "abdomen",
            Alignment.FrontLeft, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "urflank", "upper right flank", "flank", BodypartTypeEnum.Wear, "rbreast",
            Alignment.Right, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "ulflank", "upper left flank", "flank", BodypartTypeEnum.Wear, "lbreast",
            Alignment.Left, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "lrflank", "lower right flank", "flank", BodypartTypeEnum.Wear, "abdomen",
            Alignment.RearRight, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "llflank", "lower left flank", "flank", BodypartTypeEnum.Wear, "abdomen",
            Alignment.RearLeft, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "belly", "belly", "belly", BodypartTypeEnum.Wear, "abdomen",
            Alignment.Front, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "rshoulder", "right shoulder", "shoulder", BodypartTypeEnum.Wear, "rbreast",
            Alignment.FrontRight, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "lshoulder", "left shoulder", "shoulder", BodypartTypeEnum.Wear, "lbreast",
            Alignment.FrontLeft, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "uback", "upper back", "upper back", BodypartTypeEnum.Wear, "abdomen",
            Alignment.Front, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "lback", "lower back", "lower back", BodypartTypeEnum.Wear, "abdomen",
            Alignment.Rear, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "withers", "withers", "withers", BodypartTypeEnum.Wear, "uback",
            Alignment.Front, Orientation.High, 80, -1, 50, order++, "Flesh", SizeCategory.Normal, "Torso", true,
            isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "rrump", "right rump", "rump", BodypartTypeEnum.Wear, "lback",
            Alignment.RearRight, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "lrump", "left rump", "rump", BodypartTypeEnum.Wear, "lback",
            Alignment.RearLeft, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "rloin", "right loin", "loin", BodypartTypeEnum.Wear, "belly",
            Alignment.RearRight, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "lloin", "left loin", "loin", BodypartTypeEnum.Wear, "belly",
            Alignment.RearLeft, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);

        #endregion

        #region Head

        AddBodypart(quadrupedBody, "neck", "neck", "neck", BodypartTypeEnum.Wear, "uback", Alignment.Front,
            Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
            implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "bneck", "neck back", "neck back", BodypartTypeEnum.Wear, "neck",
            Alignment.Rear, Orientation.Highest, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "throat", "throat", "throat", BodypartTypeEnum.Wear, "neck",
            Alignment.Front, Orientation.Highest, 40, 50, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "head", "head", "face", BodypartTypeEnum.Wear, "neck", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(quadrupedBody, "bhead", "head back", "head back", BodypartTypeEnum.Wear, "bneck",
            Alignment.Rear, Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(quadrupedBody, "rjaw", "right jaw", "jaw", BodypartTypeEnum.Wear, "head",
            Alignment.FrontRight, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "ljaw", "left jaw", "jaw", BodypartTypeEnum.Wear, "head",
            Alignment.FrontLeft, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "rcheek", "right cheek", "cheek", BodypartTypeEnum.Wear, "head",
            Alignment.Right, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "lcheek", "left cheek", "cheek", BodypartTypeEnum.Wear, "head",
            Alignment.Left, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.Wear,
            "head", Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh",
            SizeCategory.Small, "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.Wear,
            "head", Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh",
            SizeCategory.Small, "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket",
            Alignment.FrontRight, Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket",
            Alignment.FrontLeft, Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
        AddBodypart(quadrupedBody, "rear", "right ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Right,
            Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "lear", "left ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Left,
            Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
            implantSpace: 5, stunMultiplier: 0.2);
        AddBodypart(quadrupedBody, "muzzle", "muzzle", "muzzle", BodypartTypeEnum.Wear, "head",
            Alignment.Front, Orientation.Highest, 50, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
            true, isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(quadrupedBody, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "muzzle", Alignment.Front,
            Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(quadrupedBody, "tongue", "tongue", "tongue", BodypartTypeEnum.Tongue, "mouth", Alignment.Front,
            Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);
        AddBodypart(quadrupedBody, "nose", "nose", "nose", BodypartTypeEnum.Wear, "mouth", Alignment.Front,
            Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
            isVital: true, implantSpace: 5, stunMultiplier: 1.0);

        #endregion

        #region Legs

        AddBodypart(quadrupedBody, "ruforeleg", "right upper foreleg", "upper foreleg", BodypartTypeEnum.Wear,
            "rshoulder", Alignment.FrontRight, Orientation.Low, 80, 100, 100, order++, "Bony Flesh",
            SizeCategory.Normal, "Right Foreleg");
        AddBodypart(quadrupedBody, "luforeleg", "left upper foreleg", "upper foreleg", BodypartTypeEnum.Wear,
            "lshoulder", Alignment.FrontLeft, Orientation.Low, 80, 100, 100, order++, "Bony Flesh",
            SizeCategory.Normal, "Left Foreleg");
        AddBodypart(quadrupedBody, "ruhindleg", "right upper hindleg", "upper hindleg", BodypartTypeEnum.Wear,
            "rrump", Alignment.RearRight, Orientation.Low, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Right Hindleg");
        AddBodypart(quadrupedBody, "luhindleg", "left upper hindleg", "upper hindleg", BodypartTypeEnum.Wear,
            "lrump", Alignment.RearLeft, Orientation.Low, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal,
            "Left Hindleg");
        AddBodypart(quadrupedBody, "rfknee", "right front knee", "knee", BodypartTypeEnum.Wear, "ruforeleg",
            Alignment.FrontRight, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Right Foreleg");
        AddBodypart(quadrupedBody, "lfknee", "left front knee", "knee", BodypartTypeEnum.Wear, "luforeleg",
            Alignment.FrontLeft, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Left Foreleg");
        AddBodypart(quadrupedBody, "rrknee", "right rear knee", "knee", BodypartTypeEnum.Wear, "ruhindleg",
            Alignment.RearRight, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Right Hindleg");
        AddBodypart(quadrupedBody, "rlknee", "left rear knee", "knee", BodypartTypeEnum.Wear, "luhindleg",
            Alignment.RearLeft, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Left Hindleg");
        AddBodypart(quadrupedBody, "rlforeleg", "right lower foreleg", "lower foreleg", BodypartTypeEnum.Wear,
            "rfknee", Alignment.FrontRight, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh",
            SizeCategory.Normal, "Right Foreleg");
        AddBodypart(quadrupedBody, "llforeleg", "left lower foreleg", "lower foreleg", BodypartTypeEnum.Wear,
            "lfknee", Alignment.FrontLeft, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh",
            SizeCategory.Normal, "Left Foreleg");
        AddBodypart(quadrupedBody, "rlhindleg", "right lower hindleg", "lower hindleg", BodypartTypeEnum.Wear,
            "rrknee", Alignment.RearRight, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh",
            SizeCategory.Normal, "Right Hindleg");
        AddBodypart(quadrupedBody, "llhindleg", "left lower hindleg", "lower hindleg", BodypartTypeEnum.Wear,
            "rlknee", Alignment.RearLeft, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh",
            SizeCategory.Normal, "Left Hindleg");
        AddBodypart(quadrupedBody, "rfhock", "right front hock", "front hock", BodypartTypeEnum.Wear,
            "rlforeleg", Alignment.FrontRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh",
            SizeCategory.Normal, "Right Foreleg");
        AddBodypart(quadrupedBody, "lfhock", "left front hock", "front hock", BodypartTypeEnum.Wear,
            "llforeleg", Alignment.FrontLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh",
            SizeCategory.Normal, "Left Foreleg");
        AddBodypart(quadrupedBody, "rrhock", "right rear hock", "rear hock", BodypartTypeEnum.Wear,
            "rlhindleg", Alignment.RearRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh",
            SizeCategory.Normal, "Right Hindleg");
        AddBodypart(quadrupedBody, "lrhock", "left rear hock", "rear hock", BodypartTypeEnum.Wear, "llhindleg",
            Alignment.RearLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Left Hindleg");

        #endregion

        #region Tail

        AddBodypart(quadrupedBody, "utail", "upper tail", "tail", BodypartTypeEnum.Wear, "lback",
            Alignment.Rear, Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");
        AddBodypart(quadrupedBody, "mtail", "middle tail", "tail", BodypartTypeEnum.Wear, "utail",
            Alignment.Rear, Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");
        AddBodypart(quadrupedBody, "ltail", "lower tail", "tail", BodypartTypeEnum.Wear, "mtail",
            Alignment.Rear, Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");

        #endregion

        #region Genitals

        AddBodypart(quadrupedBody, "groin", "groin", "groin", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
            Orientation.Low, 30, -1, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals");
        AddBodypart(quadrupedBody, "testicles", "testicles", "testicles", BodypartTypeEnum.Wear, "groin",
            Alignment.Rear, Orientation.Low, 10, 30, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals",
            true, isCore: false);
        AddBodypart(quadrupedBody, "penis", "penis", "penis", BodypartTypeEnum.Wear, "groin", Alignment.Rear,
            Orientation.Low, 10, 30, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals", true,
            isCore: false);
        AddBodypartUsage("penis", "male", quadrupedBody);
        AddBodypartUsage("testicles", "male", quadrupedBody);

        #endregion

        #region Wings

        AddBodypart(quadrupedBody, "rwingbase", "right wing base", "wing base", BodypartTypeEnum.Wear, "uback",
            Alignment.FrontRight, Orientation.High, 40, -1, 100, order++, "Flesh", SizeCategory.Normal,
            "Right Wing", true, isCore: false);
        AddBodypart(quadrupedBody, "lwingbase", "left wing base", "wing base", BodypartTypeEnum.Wear, "uback",
            Alignment.FrontLeft, Orientation.High, 40, -1, 100, order++, "Flesh", SizeCategory.Normal, "Left Wing",
            true, isCore: false);
        AddBodypart(quadrupedBody, "rwing", "right wing", "wing", BodypartTypeEnum.Wing, "rwingbase",
            Alignment.FrontRight, Orientation.High, 40, 50, 100, order++, "Flesh", SizeCategory.Normal,
            "Right Wing", true, isCore: false);
        AddBodypart(quadrupedBody, "lwing", "left wing", "wing", BodypartTypeEnum.Wing, "lwingbase",
            Alignment.FrontLeft, Orientation.High, 40, 50, 100, order++, "Flesh", SizeCategory.Normal, "Left Wing",
            true, isCore: false);

        #endregion

        #region Misceallaneous

        AddBodypart(quadrupedBody, "udder", "udder", "udder", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
            Orientation.Low, 40, 60, 100, order++, "Flesh", SizeCategory.Normal, "Torso", false, isCore: false);
        AddBodypart(quadrupedBody, "rhorn", "right horn", "horn", BodypartTypeEnum.Wear, "head",
            Alignment.FrontRight, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
            false, isCore: false);
        AddBodypart(quadrupedBody, "lhorn", "left horn", "horn", BodypartTypeEnum.Wear, "head",
            Alignment.FrontLeft, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
            false, isCore: false);
        AddBodypart(quadrupedBody, "horn", "horn", "horn", BodypartTypeEnum.Wear, "head", Alignment.Front,
            Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head", false, isCore: false);
        AddBodypart(quadrupedBody, "rantler", "right antler", "antler", BodypartTypeEnum.Wear, "head",
            Alignment.FrontRight, Orientation.Highest, 40, 60, 100, order++, "Antler", SizeCategory.Small, "Head",
            false, isCore: false);
        AddBodypart(quadrupedBody, "lantler", "left antler", "antler", BodypartTypeEnum.Wear, "head",
            Alignment.FrontLeft, Orientation.Highest, 40, 60, 100, order++, "Antler", SizeCategory.Small, "Head",
            false, isCore: false);
        AddBodypart(quadrupedBody, "rtusk", "right tusk", "tusk", BodypartTypeEnum.Wear, "rjaw",
            Alignment.FrontRight, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
            false, isCore: false);
        AddBodypart(quadrupedBody, "ltusk", "left tusk", "tusk", BodypartTypeEnum.Wear, "ljaw",
            Alignment.FrontLeft, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
            false, isCore: false);

        #endregion

        #region Ungulates

        AddBodypart(ungulateBody, "rfhoof", "right front hoof", "hoof", BodypartTypeEnum.Wear, "rfhock",
            Alignment.FrontRight, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Right Foreleg");
        AddBodypart(ungulateBody, "lfhoof", "left front hoof", "hoof", BodypartTypeEnum.Wear, "lfhock",
            Alignment.FrontLeft, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Left Foreleg");
        AddBodypart(ungulateBody, "rrhoof", "right rear hoof", "hoof", BodypartTypeEnum.Wear, "rrhock",
            Alignment.RearRight, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Right Hindleg");
        AddBodypart(ungulateBody, "lrhoof", "left rear hoof", "hoof", BodypartTypeEnum.Wear, "lrhock",
            Alignment.RearLeft, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Left Hindleg");
        AddBodypart(ungulateBody, "rffrog", "right front frog", "hoof", BodypartTypeEnum.Standing, "rfhoof",
            Alignment.FrontRight, Orientation.Lowest, 20, 50, 10, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Right Foreleg");
        AddBodypart(ungulateBody, "lffrog", "left front frog", "hoof", BodypartTypeEnum.Standing, "lfhoof",
            Alignment.FrontLeft, Orientation.Lowest, 20, 50, 10, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Left Foreleg");
        AddBodypart(ungulateBody, "rrfrog", "right rear frog", "hoof", BodypartTypeEnum.Standing, "rrhoof",
            Alignment.RearRight, Orientation.Lowest, 20, 50, 10, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Right Hindleg");
        AddBodypart(ungulateBody, "lrfrog", "left rear frog", "hoof", BodypartTypeEnum.Standing, "lrhoof",
            Alignment.RearLeft, Orientation.Lowest, 20, 50, 10, order++, "Dense Bony Flesh", SizeCategory.Small,
            "Left Hindleg");

        #endregion

        #region Toed

        AddBodypart(toedQuadruped, "rfpaw", "right front paw", "paw", BodypartTypeEnum.Standing, "rfhock",
            Alignment.FrontRight, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
            "Right Foreleg");
        AddBodypart(toedQuadruped, "lfpaw", "left front paw", "paw", BodypartTypeEnum.Standing, "lfhock",
            Alignment.FrontLeft, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
            "Left Foreleg");
        AddBodypart(toedQuadruped, "rrpaw", "right rear paw", "paw", BodypartTypeEnum.Standing, "rrhock",
            Alignment.RearRight, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
            "Right Hindleg");
        AddBodypart(toedQuadruped, "lrpaw", "left rear paw", "paw", BodypartTypeEnum.Standing, "lrhock",
            Alignment.RearLeft, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
            "Left Hindleg");
        AddBodypart(toedQuadruped, "rfclaw", "right front claws", "claw", BodypartTypeEnum.Wear, "rfpaw",
            Alignment.FrontRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Right Foreleg", false, isVital: false);
        AddBodypart(toedQuadruped, "lfclaw", "left front claws", "claw", BodypartTypeEnum.Wear, "lfpaw",
            Alignment.FrontLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Left Foreleg", false, isVital: false);
        AddBodypart(toedQuadruped, "rrclaw", "right rear claws", "claw", BodypartTypeEnum.Wear, "rrpaw",
            Alignment.RearRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Right Hindleg", false, isVital: false);
        AddBodypart(toedQuadruped, "lrclaw", "left rear claws", "claw", BodypartTypeEnum.Wear, "lrpaw",
            Alignment.RearLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Left Hindleg", false, isVital: false);
        AddBodypart(toedQuadruped, "rrdewclaw", "right dewclaw", "dewclaw", BodypartTypeEnum.Wear, "rrpaw",
            Alignment.RearRight, Orientation.Lowest, 10, 50, 5, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Right Hindleg", false, isVital: false);
        AddBodypart(toedQuadruped, "lrdewclaw", "left dewclaw", "dewclaw", BodypartTypeEnum.Wear, "lrpaw",
            Alignment.RearLeft, Orientation.Lowest, 10, 50, 5, order++, "Dense Bony Flesh", SizeCategory.Normal,
            "Left Hindleg", false, isVital: false);

        #endregion

        _context.SaveChanges();

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Organs...");

        #region Organs

        AddOrgan(quadrupedBody, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1,
            stunModifier: 1.0);
        AddOrgan(quadrupedBody, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
        AddOrgan(quadrupedBody, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(quadrupedBody, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(quadrupedBody, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(quadrupedBody, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
            0.05);
        AddOrgan(quadrupedBody, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
            0.05);
        AddOrgan(quadrupedBody, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(quadrupedBody, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
            painModifier: 3.0);
        AddOrgan(quadrupedBody, "rlung", "right lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(quadrupedBody, "llung", "left lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(quadrupedBody, "trachea", "trachea", BodypartTypeEnum.Trachea, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(quadrupedBody, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
        AddOrgan(quadrupedBody, "uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(quadrupedBody, "mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0,
            0.05, stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(quadrupedBody, "lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
            stunModifier: 1.0, painModifier: 2.0);
        AddOrgan(quadrupedBody, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
        AddOrgan(quadrupedBody, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

        AddOrganCoverage("brain", "head", 100, true);
        AddOrganCoverage("brain", "bhead", 100);
        AddOrganCoverage("brain", "rcheek", 85);
        AddOrganCoverage("brain", "lcheek", 85);
        AddOrganCoverage("brain", "reyesocket", 85);
        AddOrganCoverage("brain", "leyesocket", 85);
        AddOrganCoverage("brain", "reye", 85);
        AddOrganCoverage("brain", "leye", 85);
        AddOrganCoverage("brain", "muzzle", 10);
        AddOrganCoverage("brain", "mouth", 30);
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
        AddOrganCoverage("heart", "lshoulder", 20);

        AddOrganCoverage("uspinalcord", "bneck", 10, true);
        AddOrganCoverage("uspinalcord", "neck", 2);
        AddOrganCoverage("uspinalcord", "throat", 5);
        AddOrganCoverage("mspinalcord", "uback", 10, true);
        AddOrganCoverage("mspinalcord", "withers", 2);
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
        AddOrganCoverage("lintestines", "rloin", 10);
        AddOrganCoverage("lintestines", "lloin", 10);

        AddOrganCoverage("rkidney", "lback", 20, true);
        AddOrganCoverage("lkidney", "lback", 20, true);
        AddOrganCoverage("rkidney", "belly", 5);
        AddOrganCoverage("lkidney", "belly", 5);
        _context.SaveChanges();

        #endregion

        _context.SaveChanges();

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bones...");

        #region Bones

        AddBone(quadrupedBody, "smaxillary", "superior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rimaxillary", "right inferior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "limaxillary", "left inferior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "fskull", "frontal skull bone", BodypartTypeEnum.NonImmobilisingBone, 200,
            "Compact Bone");
        AddBone(quadrupedBody, "cvertebrae", "cervical vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "dvertebrae", "dorsal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lvertebrae", "lumbar vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "svertebrae", "sacral vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "cavertebrae", "caudal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rscapula", "right scapula", BodypartTypeEnum.NonImmobilisingBone, 150,
            "Compact Bone");
        AddBone(quadrupedBody, "lscapula", "left scapula", BodypartTypeEnum.NonImmobilisingBone, 150,
            "Compact Bone");
        AddBone(quadrupedBody, "rhumerus", "right humerus", BodypartTypeEnum.Bone, 140, "Compact Bone");
        AddBone(quadrupedBody, "lhumerus", "left humerus", BodypartTypeEnum.Bone, 140, "Compact Bone");
        AddBone(quadrupedBody, "rradius", "right radius", BodypartTypeEnum.Bone, 140, "Compact Bone");
        AddBone(quadrupedBody, "lradius", "left radius", BodypartTypeEnum.Bone, 140, "Compact Bone");
        AddBone(quadrupedBody, "rulna", "right ulna", BodypartTypeEnum.Bone, 120, "Compact Bone");
        AddBone(quadrupedBody, "lulna", "left ulna", BodypartTypeEnum.Bone, 120, "Compact Bone");
        AddBone(quadrupedBody, "rcarpal", "right carpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
        AddBone(quadrupedBody, "lcarpal", "left carpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
        AddBone(quadrupedBody, "rmetacarpal", "right metacarpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
        AddBone(quadrupedBody, "lmetacarpal", "left metacarpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
        AddBone(quadrupedBody, "sternum", "sternum", BodypartTypeEnum.NonImmobilisingBone, 200, "Compact Bone");
        AddBone(quadrupedBody, "rrib1", "right first rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib1", "left first rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib2", "right second rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib2", "left second rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib3", "right third rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib3", "left third rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib4", "right fourth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib4", "left fourth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib5", "right fifth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib5", "left fifth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib6", "right sixth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib6", "left sixth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib7", "right seventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib7", "left seventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib8", "right eighth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib8", "left eighth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib9", "right ninth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib9", "left ninth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib10", "right tenth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib10", "left tenth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib11", "right eleventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib11", "left eleventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rrib12", "right twelth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "lrib12", "left twelth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
            "Compact Bone");
        AddBone(quadrupedBody, "rilium", "right ilium", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
        AddBone(quadrupedBody, "lilium", "left ilium", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
        AddBone(quadrupedBody, "sacrum", "sacrum", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
        AddBone(quadrupedBody, "rpubis", "right pubis", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
        AddBone(quadrupedBody, "lpubis", "left pubis", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
        AddBone(quadrupedBody, "rischium", "right ischium", BodypartTypeEnum.NonImmobilisingBone, 150,
            "Compact Bone");
        AddBone(quadrupedBody, "lischium", "left ischium", BodypartTypeEnum.NonImmobilisingBone, 150,
            "Compact Bone");
        AddBone(quadrupedBody, "rfemur", "right femur", BodypartTypeEnum.Bone, 200, "Compact Bone");
        AddBone(quadrupedBody, "lfemur", "left femur", BodypartTypeEnum.Bone, 200, "Compact Bone");
        AddBone(quadrupedBody, "rpatella", "right patella", BodypartTypeEnum.Bone, 90, "Compact Bone");
        AddBone(quadrupedBody, "lpatella", "left patella", BodypartTypeEnum.Bone, 90, "Compact Bone");
        AddBone(quadrupedBody, "rtibia", "right tibia", BodypartTypeEnum.Bone, 150, "Compact Bone");
        AddBone(quadrupedBody, "ltibia", "left tibia", BodypartTypeEnum.Bone, 150, "Compact Bone");
        AddBone(quadrupedBody, "rfibula", "right fibula", BodypartTypeEnum.Bone, 150, "Compact Bone");
        AddBone(quadrupedBody, "lfibula", "left fibula", BodypartTypeEnum.Bone, 150, "Compact Bone");
        AddBone(quadrupedBody, "rcalcaneus", "right calcaneus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(quadrupedBody, "lcalcaneus", "left calcaneus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(quadrupedBody, "rtalus", "right talus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(quadrupedBody, "ltalus", "left talus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(quadrupedBody, "rtarsus", "right tarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(quadrupedBody, "ltarsus", "left tarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(quadrupedBody, "rmetatarsus", "right metatarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
        AddBone(quadrupedBody, "lmetatarsus", "left metatarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");

        // TORSO BONES
        AddBoneInternal("sternum", "abdomen", 50);
        AddBoneInternal("rrib1", "rshoulder", 10);
        AddBoneInternal("lrib1", "lshoulder", 10);
        AddBoneInternal("rrib2", "rbreast", 5);
        AddBoneInternal("lrib2", "lbreast", 5);
        AddBoneInternal("rrib3", "rbreast", 5);
        AddBoneInternal("lrib3", "lbreast", 5);
        AddBoneInternal("rrib4", "rbreast", 5);
        AddBoneInternal("lrib4", "lbreast", 5);
        AddBoneInternal("rrib5", "rbreast", 5);
        AddBoneInternal("lrib5", "lbreast", 5);
        AddBoneInternal("rrib6", "rbreast", 5);
        AddBoneInternal("lrib6", "lbreast", 5);
        AddBoneInternal("rrib7", "rbreast", 5);
        AddBoneInternal("lrib7", "lbreast", 5);
        AddBoneInternal("rrib8", "rbreast", 5);
        AddBoneInternal("lrib8", "lbreast", 5);
        AddBoneInternal("rrib9", "rbreast", 5);
        AddBoneInternal("lrib9", "lbreast", 5);
        AddBoneInternal("rrib10", "rbreast", 5);
        AddBoneInternal("lrib10", "lbreast", 5);
        AddBoneInternal("rrib11", "rbreast", 5);
        AddBoneInternal("lrib11", "lbreast", 5);
        AddBoneInternal("rrib12", "rbreast", 5);
        AddBoneInternal("lrib12", "lbreast", 5);
        AddBoneInternal("cvertebrae", "bneck", 35);
        AddBoneInternal("dvertebrae", "uback", 20);
        AddBoneInternal("dvertebrae", "withers", 20, false);
        AddBoneInternal("lvertebrae", "lback", 20);
        AddBoneInternal("svertebrae", "utail", 90);
        AddBoneInternal("svertebrae", "mtail", 90, false);
        AddBoneInternal("cavertebrae", "ltail", 80);
        AddBoneInternal("sacrum", "lback", 15);
        AddBoneInternal("rilium", "rrump", 50);
        AddBoneInternal("lilium", "lrump", 50);
        AddBoneInternal("rilium", "lback", 4, false);
        AddBoneInternal("lilium", "lback", 4, false);
        AddBoneInternal("rpubis", "groin", 20);
        AddBoneInternal("lpubis", "groin", 20);
        AddBoneInternal("rischium", "rrump", 20);
        AddBoneInternal("lischium", "lrump", 20);

        // HEAD BONES
        AddBoneInternal("smaxillary", "muzzle", 100);
        AddBoneInternal("smaxillary", "mouth", 40, false);
        AddBoneInternal("smaxillary", "rcheek", 20, false);
        AddBoneInternal("smaxillary", "lcheek", 20, false);
        AddBoneInternal("rimaxillary", "rjaw", 100);
        AddBoneInternal("limaxillary", "ljaw", 100);
        AddBoneInternal("fskull", "head", 100);
        AddBoneInternal("fskull", "bhead", 40, false);

        // ARM BONES
        AddBoneInternal("rscapula", "rshoulder", 100);
        AddBoneInternal("lscapula", "lshoulder", 100);
        AddBoneInternal("rhumerus", "ruforeleg", 50);
        AddBoneInternal("lhumerus", "luforeleg", 50);
        AddBoneInternal("rradius", "rlforeleg", 33);
        AddBoneInternal("lradius", "llforeleg", 33);
        AddBoneInternal("rulna", "rlforeleg", 33);
        AddBoneInternal("lulna", "llforeleg", 33);
        AddBoneInternal("rcarpal", "rfhock", 50);
        AddBoneInternal("lcarpal", "lfhock", 50);
        AddBoneInternal("rmetacarpal", "rfhoof", 50);
        AddBoneInternal("lmetacarpal", "lfhoof", 50);
        AddBoneInternal("rmetacarpal", "rfpaw", 50);
        AddBoneInternal("lmetacarpal", "lfpaw", 50);

        // LEG BONES
        AddBoneInternal("rfemur", "ruforeleg", 50);
        AddBoneInternal("lfemur", "luforeleg", 50);
        AddBoneInternal("rpatella", "rrknee", 100);
        AddBoneInternal("lpatella", "rlknee", 100);
        AddBoneInternal("rtibia", "rlforeleg", 100);
        AddBoneInternal("ltibia", "llforeleg", 100);
        AddBoneInternal("rfibula", "rlforeleg", 33);
        AddBoneInternal("lfibula", "llforeleg", 33);
        AddBoneInternal("rcalcaneus", "rrhock", 20);
        AddBoneInternal("lcalcaneus", "lrhock", 20);
        AddBoneInternal("rtalus", "rrhock", 20);
        AddBoneInternal("ltalus", "lrhock", 20);
        AddBoneInternal("rtarsus", "rrhock", 50);
        AddBoneInternal("ltarsus", "lrhock", 50);
        AddBoneInternal("rmetatarsus", "rrhoof", 50);
        AddBoneInternal("lmetatarsus", "lrhoof", 50);
        AddBoneInternal("rmetatarsus", "rrpaw", 50);
        AddBoneInternal("lmetatarsus", "lrpaw", 50);
        _context.SaveChanges();

        AddBoneCover("fskull", "brain", 100);
        AddBoneCover("smaxillary", "brain", 90);

        AddBoneCover("cvertebrae", "uspinalcord", 100);
        AddBoneCover("dvertebrae", "mspinalcord", 100);
        AddBoneCover("lvertebrae", "lspinalcord", 100);

        AddBoneCover("sternum", "heart", 80);
        AddBoneCover("sternum", "rlung", 17.5);
        AddBoneCover("sternum", "llung", 17.5);
        AddBoneCover("lrib1", "heart", 5);
        AddBoneCover("lrib2", "heart", 10);
        AddBoneCover("lrib3", "heart", 15);
        AddBoneCover("lrib4", "heart", 15);
        AddBoneCover("lrib5", "heart", 15);
        AddBoneCover("lrib6", "heart", 15);
        AddBoneCover("lrib1", "llung", 10);
        AddBoneCover("lrib2", "llung", 15);
        AddBoneCover("lrib3", "llung", 20);
        AddBoneCover("lrib4", "llung", 20);
        AddBoneCover("lrib5", "llung", 20);
        AddBoneCover("lrib6", "llung", 20);
        AddBoneCover("lrib7", "llung", 20);
        AddBoneCover("rrib1", "rlung", 10);
        AddBoneCover("rrib2", "rlung", 15);
        AddBoneCover("rrib3", "rlung", 20);
        AddBoneCover("rrib4", "rlung", 20);
        AddBoneCover("rrib5", "rlung", 20);
        AddBoneCover("rrib6", "rlung", 20);
        AddBoneCover("rrib7", "rlung", 20);

        AddBoneCover("rrib6", "liver", 30);
        AddBoneCover("rrib7", "liver", 45);
        AddBoneCover("lrib6", "liver", 30);
        AddBoneCover("lrib7", "liver", 45);

        AddBoneCover("lrib8", "liver", 80);
        AddBoneCover("lrib8", "spleen", 25);
        AddBoneCover("rrib8", "liver", 80);
        AddBoneCover("rrib8", "spleen", 25);

        AddBoneCover("lrib9", "liver", 60);
        AddBoneCover("lrib9", "spleen", 20);
        AddBoneCover("rrib9", "liver", 60);
        AddBoneCover("rrib9", "spleen", 20);

        AddBoneCover("lrib10", "liver", 15);
        AddBoneCover("lrib10", "lkidney", 20);
        AddBoneCover("rrib10", "liver", 15);
        AddBoneCover("rrib10", "rkidney", 20);

        AddBoneCover("rscapula", "rlung", 70);
        AddBoneCover("lscapula", "llung", 70);

        AddBoneCover("rilium", "sintestines", 20);
        AddBoneCover("lilium", "sintestines", 20);
        AddBoneCover("rilium", "lintestines", 40);
        AddBoneCover("lilium", "lintestines", 40);

        AddBoneCover("rischium", "lintestines", 40);
        AddBoneCover("lischium", "lintestines", 40);
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
                RootBody = quadrupedBody,
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
        AddLimb("Right Foreleg", LimbType.Leg, "ruforeleg", 0.5, 0.5);
        AddLimb("Left Foreleg", LimbType.Leg, "luforeleg", 0.5, 0.5);
        AddLimb("Right Hindleg", LimbType.Leg, "ruhindleg", 0.5, 0.5);
        AddLimb("Left Hindleg", LimbType.Leg, "luhindleg", 0.5, 0.5);
        AddLimb("Tail", LimbType.Appendage, "utail", 0.5, 0.5);
        AddLimb("Right Wing", LimbType.Wing, "rwingbase", 0.5, 0.5);
        AddLimb("Left Wing", LimbType.Wing, "lwingbase", 0.5, 0.5);
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
                case "Right Wing":
                case "Left Wing":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["uspinalcord"] });
                    break;
                case "Genitals":
                case "Right Foreleg":
                case "Left Foreleg":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["mspinalcord"] });
                    break;
                case "Leg Hindleg":
                case "Right Hindleg":
                case "Tail":
                    _context.LimbsSpinalParts.Add(new LimbsSpinalPart
                    { Limb = limb, BodypartProto = _cachedOrgans["lspinalcord"] });
                    break;
            }
        }

        _context.SaveChanges();

        #endregion

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Group Describers...");

        #region Groups

        AddBodypartGroupDescriberShape(quadrupedBody, "body", "The whole torso of a quadruped",
            ("abdomen", 1, 1),
            ("belly", 1, 1),
            ("withers", 1, 1),
            ("breast", 1, 2),
            ("flank", 1, 2),
            ("loin", 1, 2),
            ("shoulder", 1, 2),
            ("upper back", 1, 1),
            ("lower back", 1, 1),
            ("rump", 1, 2),
            ("neck", 0, 1),
            ("neck back", 0, 1),
            ("throat", 0, 1),
            ("udder", 0, 1)
        );
        AddBodypartGroupDescriberShape(quadrupedBody, "legs", "Four legs of a quadruped",
            ("upper hindleg", 1, 2),
            ("upper foreleg", 1, 2),
            ("lower hindleg", 0, 2),
            ("lower foreleg", 0, 2),
            ("knee", 0, 4),
            ("front hock", 0, 2),
            ("rear hock", 0, 2),
            ("hoof", 0, 4),
            ("paw", 0, 4),
            ("claw", 0, 4),
            ("frog", 0, 4),
            ("dewclaw", 0, 2)
        );
        AddBodypartGroupDescriberShape(quadrupedBody, "forelegs", "Both forelegs of a quadruped",
            ("upper foreleg", 2, 2),
            ("lower foreleg", 0, 2),
            ("knee", 0, 2),
            ("front hock", 0, 2),
            ("hoof", 0, 2),
            ("paw", 0, 2),
            ("claw", 0, 2),
            ("frog", 0, 2)
        );
        AddBodypartGroupDescriberShape(quadrupedBody, "hindlegs", "Both hindlegs of a quadruped",
            ("upper hindleg", 2, 2),
            ("lower hindleg", 0, 2),
            ("knee", 0, 2),
            ("rear hock", 0, 2),
            ("hoof", 0, 2),
            ("paw", 0, 2),
            ("claw", 0, 2),
            ("frog", 0, 2),
            ("dewclaw", 0, 2)
        );
        AddBodypartGroupDescriberShape(quadrupedBody, "tail", "The tail bodyparts",
            ("tail", 1, 3)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "head", "A quadruped head",
            ("face", 1, 1),
            ("head back", 0, 1),
            ("eye socket", 0, 2),
            ("eye", 0, 2),
            ("ear", 0, 2),
            ("jaw", 0, 2),
            ("muzzle", 0, 1),
            ("nose", 0, 1),
            ("mouth", 0, 1),
            ("tongue", 0, 1),
            ("cheek", 0, 2),
            ("throat", 0, 1),
            ("withers", 0, 1),
            ("neck", 0, 1),
            ("neck back", 0, 1),
            ("horn", 0, 2),
            ("tusk", 0, 2),
            ("antler", 0, 2)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "wings", "A pair of quadruped wings",
            ("wing base", 2, 2),
            ("wing", 2, 2)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "back", "A quadruped back",
            ("upper back", 1, 1),
            ("lower back", 1, 1),
            ("flank", 0, 4),
            ("rump", 0, 2),
            ("withers", 0, 1),
            ("neck back", 0, 1)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "eyes", "A pair of quadruped eyes",
            ("eye socket", 2, 2),
            ("eye", 0, 2)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "ears", "A pair of quadruped ears",
            ("ear", 2, 2)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "horns", "A pair of quadruped horns",
            ("horn", 2, 2)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "tusks", "A pair of quadruped tusks",
            ("tusk", 2, 2)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "antlers", "A pair of quadruped antlers",
            ("antler", 2, 2)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "hooves", "A group of quadruped hooves",
            ("hoof", 2, 4)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "paws", "A group of quadruped paws",
            ("paw", 2, 4)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "claws", "A group of quadruped claws",
            ("claw", 2, 4)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "knees", "A group of quadruped knees",
            ("knee", 2, 4)
        );

        AddBodypartGroupDescriberShape(quadrupedBody, "shoulders", "A group of quadruped shoulders",
            ("shoulder", 2, 4)
        );

        _context.SaveChanges();

        #endregion

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");

        #region Races

        SeedAnimalRaces(GetMammalRaceTemplates(),
            ("Ungulate", ungulateBody),
            ("Toed Quadruped", toedQuadruped));

        #endregion
    }
}
