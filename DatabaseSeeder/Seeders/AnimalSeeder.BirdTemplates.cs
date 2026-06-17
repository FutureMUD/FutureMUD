#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private static IEnumerable<AnimalRaceTemplate> GetBirdRaceTemplates()
    {
        static AnimalRaceTemplate Bird(
            string name,
            SizeCategory size,
            double health,
            string model,
            string loadout,
            AnimalDescriptionPack pack,
            string description,
            string combatStrategyKey = "Beast Skirmisher")
        {
            return new AnimalRaceTemplate(
                name,
                name,
                description,
                "Avian",
                size,
                false,
                health,
                model,
                model,
                "aquatic-bird-fish",
                loadout,
                pack,
                false,
                AnimalBreathingMode.Simple,
                null,
                "avian",
                null,
                combatStrategyKey
            );
        }

        yield return Bird("Pigeon", SizeCategory.VerySmall, 0.1, "Small Bird", "bird-small",
            BirdPack("a squab", "a young pigeon", "a pigeon",
                "It is plump and short-necked, with neat feathers and a bobbing head.",
                "Iridescent tones gleam faintly about its neck whenever the light catches them.",
                "It struts and coos with urban confidence.",
                "town squares, rooftops and cliff ledges"),
            description: """
            In gardens, thickets, orchards, hedgerows and open sky, pigeons fill the role of small birds of hedges, eaves and woodland edges. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The eye is drawn to light bones, neat plumage, quick beaks and wings made for brief, precise flights. Those features support flickering hops, sudden flutters and a constant awareness of cover, making the creature feel suited to its ground rather than merely placed there.

            They add movement and sound to settled places, becoming part of the weather of daily life. A pigeon can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Bird("Parrot", SizeCategory.VerySmall, 0.1, "Small Bird", "bird-small",
            BirdPack("a parrot chick", "a young parrot", "a parrot",
                "It is bright-feathered and compact, with a hooked beak and expressive eyes.",
                "Its beak and feet both look clever enough to manipulate seed, perch and object alike.",
                "It watches the world with obvious intelligence and curiosity.",
                "jungle canopy and warm woodland"),
            description: """
            Parrots occupy jungle canopy, warm woodland, orchards and river forests as bright, clever climbing birds. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            At a glance, the form resolves into hooked beaks, gripping feet, expressive eyes and plumage that refuses to be ignored. In motion they are marked by social mischief, noisy attention and careful manipulation of anything within reach, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are companionable only on their own terms, clever enough to charm a household and ruin a latch. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Bird("Swallow", SizeCategory.VerySmall, 0.1, "Songbird", "bird-small",
            BirdPack("a swallow chick", "a young swallow", "a swallow",
                "It is slim and delicate, with pointed wings and a forked tail.",
                "Its entire body looks designed around speed and precision in the air.",
                "It seems almost too restless to stay still for long.",
                "barn eaves, cliff faces and open sky"),
            description: """
            Where gardens, thickets, orchards, hedgerows and open sky meets daily life, swallows are recognisable as small birds of hedges, eaves and woodland edges. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by light bones, neat plumage, quick beaks and wings made for brief, precise flights. The impression is completed by flickering hops, sudden flutters and a constant awareness of cover, giving the animal a readable rhythm even before it acts.

            They add movement and sound to settled places, becoming part of the weather of daily life. A swallow can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Bird("Sparrow", SizeCategory.VerySmall, 0.05, "Songbird", "bird-small",
            BirdPack("a sparrow chick", "a young sparrow", "a sparrow",
                "It is tiny and brown-feathered, with a compact beak and bright round eye.",
                "Its short hops and quick movements make it look like a living scrap of feather and urgency.",
                "It busies itself with constant pecks, flutters and little bursts of motion.",
                "hedges, eaves and garden edge"),
            description: """
            Sparrows are small birds of hedges, eaves and woodland edges of gardens, thickets, orchards, hedgerows and open sky. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: light bones, neat plumage, quick beaks and wings made for brief, precise flights. Watch one move and the emphasis falls on flickering hops, sudden flutters and a constant awareness of cover, a pattern that tells more than size alone.

            They add movement and sound to settled places, becoming part of the weather of daily life. A sparrow can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Bird("Finch", SizeCategory.VerySmall, 0.05, "Songbird", "bird-small",
            BirdPack("a finch chick", "a young finch", "a finch",
                "It is tiny, neat and brightly marked, with a short seed-cracking beak.",
                "Its trim profile and clean plumage give it a delicate, jewel-like quality.",
                "It flicks and chirrs with lively, nervous energy.",
                "woodland edge, scrub and orchard"),
            description: """
            In gardens, thickets, orchards, hedgerows and open sky, finches fill the role of small birds of hedges, eaves and woodland edges. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: light bones, neat plumage, quick beaks and wings made for brief, precise flights. Watch one move and the emphasis falls on flickering hops, sudden flutters and a constant awareness of cover, a pattern that tells more than size alone.

            They add movement and sound to settled places, becoming part of the weather of daily life. A finch can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Bird("Robin", SizeCategory.VerySmall, 0.05, "Songbird", "bird-small",
            BirdPack("a robin chick", "a young robin", "a robin",
                "It is small and round-bodied, with soft plumage and an alert stance.",
                "Its breast stands out brightly against the softer browns and greys of the rest of its body.",
                "It looks bold for so small a bird, ready to claim a branch or patch of ground as its own.",
                "woodland edge, garden and hedgerow"),
            description: """
            Robins occupy gardens, thickets, orchards, hedgerows and open sky as small birds of hedges, eaves and woodland edges. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: light bones, neat plumage, quick beaks and wings made for brief, precise flights. Watch one move and the emphasis falls on flickering hops, sudden flutters and a constant awareness of cover, a pattern that tells more than size alone.

            They add movement and sound to settled places, becoming part of the weather of daily life. A robin can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Bird("Wren", SizeCategory.VerySmall, 0.05, "Songbird", "bird-small",
            BirdPack("a wren chick", "a young wren", "a wren",
                "It is tiny and compact, with a cocked tail and fine brown plumage.",
                "Its small sharp beak and bright eye make it look busy even while standing still.",
                "It seems constantly on the verge of vanishing into a crack or clump of brush.",
                "hedge, thicket and undergrowth"),
            description: """
            Where gardens, thickets, orchards, hedgerows and open sky meets daily life, wrens are recognisable as small birds of hedges, eaves and woodland edges. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: light bones, neat plumage, quick beaks and wings made for brief, precise flights. Watch one move and the emphasis falls on flickering hops, sudden flutters and a constant awareness of cover, a pattern that tells more than size alone.

            They add movement and sound to settled places, becoming part of the weather of daily life. A wren can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Bird("Quail", SizeCategory.VerySmall, 0.1, "Small Bird", "bird-fowl",
            BirdPack("a quail chick", "a young quail", "a quail",
                "It is squat and earth-toned, with rounded wings and a neat compact body.",
                "Its markings are ideal for melting into dry grass and leaf litter.",
                "It looks much happier running under cover than taking to the air.",
                "grassland and scrub undergrowth"),
            combatStrategyKey: "Beast Coward",
            description: """
            Quails are earth-bound or ground-loving birds of field margins, moors, coops, scrub and forest floors. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by sturdy legs, rounded bodies and plumage meant for concealment, display or barnyard work. The impression is completed by more trust in running, scratching and sudden short flight than in open soaring, giving the animal a readable rhythm even before it acts.

            They belong to hunting, farming and local noise, small presences that can still dominate a yard or thicket. Around settlements, roads or camps, quails add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Duck", SizeCategory.Small, 0.2, "Waterfowl", "bird-fowl",
            BirdPack("a duckling", "a young drake", "a duck",
                "It is broad-billed and web-footed, with water-shedding plumage and a low-slung body.",
                "Its bill and feet make its life on water obvious at a glance.",
                "It carries itself with a comic, practical assurance.",
                "pond, marsh and slow river"),
            combatStrategyKey: "Beast Brawler",
            description: """
            Ducks occupy ponds, lakes, marshes, slow rivers and sheltered coasts as broad-bodied water birds. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The most telling cues are webbed feet, water-shedding feathers and bills shaped for grazing, filtering or dabbling. Those features support practical movement between water, mud and bank, making the creature feel suited to its ground rather than merely placed there.

            They are familiar around water, useful as food or ornament, and capable of surprising force when defending space. A duck can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Bird("Goose", SizeCategory.Small, 0.4, "Waterfowl", "bird-fowl",
            BirdPack("a gosling", "a young goose", "a goose",
                "It is long-necked and broad-bodied, with a heavy bill and strong webbed feet.",
                "Its reach and angry hiss suggest that it is much more formidable than its shape first implies.",
                "It looks argumentative even at rest.",
                "lake edge, marsh pasture and open water"),
            combatStrategyKey: "Beast Brawler",
            description: """
            Where ponds, lakes, marshes, slow rivers and sheltered coasts meets daily life, geese are recognisable as broad-bodied water birds. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Most of its character sits in webbed feet, water-shedding feathers and bills shaped for grazing, filtering or dabbling. Those features support practical movement between water, mud and bank, making the creature feel suited to its ground rather than merely placed there.

            They are familiar around water, useful as food or ornament, and capable of surprising force when defending space. A goose can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Bird("Swan", SizeCategory.Small, 0.4, "Waterfowl", "bird-fowl",
            BirdPack("a cygnet", "a young swan", "a swan",
                "It is long-necked and elegantly proportioned, clothed in clean layered plumage.",
                "Its broad wings and poised carriage give it a regal, almost theatrical profile.",
                "It glides or stalks with deliberate grace.",
                "lake, broad river and ornamental water"),
            combatStrategyKey: "Beast Brawler",
            description: """
            Swans are broad-bodied water birds of ponds, lakes, marshes, slow rivers and sheltered coasts. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Look closer and the outline is all webbed feet, water-shedding feathers and bills shaped for grazing, filtering or dabbling. In motion they are marked by practical movement between water, mud and bank, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are familiar around water, useful as food or ornament, and capable of surprising force when defending space. Around settlements, roads or camps, swans add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Grouse", SizeCategory.Small, 0.2, "Small Bird", "bird-fowl",
            BirdPack("a grouse chick", "a young grouse", "a grouse",
                "It is round-bodied and heavily feathered, marked in mottled browns and greys.",
                "Its camouflage is excellent, and its dense plumage suits cold scrub and rough ground.",
                "It seems far more comfortable bursting into brief noisy flight than soaring.",
                "moor, heath and rough forest floor"),
            description: """
            In field margins, moors, coops, scrub and forest floors, grouses fill the role of earth-bound or ground-loving birds. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by sturdy legs, rounded bodies and plumage meant for concealment, display or barnyard work. The impression is completed by more trust in running, scratching and sudden short flight than in open soaring, giving the animal a readable rhythm even before it acts.

            They belong to hunting, farming and local noise, small presences that can still dominate a yard or thicket. Around settlements, roads or camps, grouses add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Pheasant", SizeCategory.Small, 0.2, "Small Bird", "bird-fowl",
            BirdPack("a pheasant chick", "a young pheasant", "a pheasant",
                "It is long-tailed and elegant, with layered feathers and a light but athletic build.",
                "Its head and neck carry the sort of colouring that can flare brilliantly in full-grown males.",
                "It has the alert, skittish poise of a bird that trusts its legs before its wings.",
                "field margin, light woodland and hedgerow"),
            description: """
            Pheasants occupy field margins, moors, coops, scrub and forest floors as earth-bound or ground-loving birds. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by sturdy legs, rounded bodies and plumage meant for concealment, display or barnyard work. The impression is completed by more trust in running, scratching and sudden short flight than in open soaring, giving the animal a readable rhythm even before it acts.

            They belong to hunting, farming and local noise, small presences that can still dominate a yard or thicket. Around settlements, roads or camps, pheasants add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Chicken", SizeCategory.Small, 0.2, "Waterfowl", "bird-fowl",
            BirdPack("a chick", "a young chicken", "a chicken",
                "It is compact and practical, with a thick body, short wings and sturdy scratching feet.",
                "Its comb, wattles and blunt beak give it an unmistakably domestic look.",
                "It fusses and scratches with constant barnyard purpose.",
                "yard, coop and farmstead"),
            combatStrategyKey: "Beast Brawler",
            description: """
            Where field margins, moors, coops, scrub and forest floors meets daily life, chickens are recognisable as earth-bound or ground-loving birds. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by sturdy legs, rounded bodies and plumage meant for concealment, display or barnyard work. The impression is completed by more trust in running, scratching and sudden short flight than in open soaring, giving the animal a readable rhythm even before it acts.

            They belong to hunting, farming and local noise, small presences that can still dominate a yard or thicket. Around settlements, roads or camps, chickens add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Turkey", SizeCategory.Small, 0.35, "Waterfowl", "bird-fowl",
            BirdPack("a poult", "a young turkey", "a turkey",
                "It is broad and heavy, with a fan-like tail and powerful legs.",
                "Its bare head and fleshy wattles give it an oddly severe look.",
                "It struts with a mixture of caution and bluster.",
                "woodland edge and rough pasture"),
            combatStrategyKey: "Beast Brawler",
            description: """
            Turkeys are earth-bound or ground-loving birds of field margins, moors, coops, scrub and forest floors. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: sturdy legs, rounded bodies and plumage meant for concealment, display or barnyard work. Watch one move and the emphasis falls on more trust in running, scratching and sudden short flight than in open soaring, a pattern that tells more than size alone.

            They belong to hunting, farming and local noise, small presences that can still dominate a yard or thicket. Around settlements, roads or camps, turkeys add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Seagull", SizeCategory.Small, 0.2, "Small Bird", "bird-small",
            BirdPack("a gull chick", "a young gull", "a gull",
                "It is light-bodied and strong-winged, with a sharp bill and webbed feet.",
                "Its hard eyes and hooked posture suggest a bird always ready to steal or scavenge.",
                "It moves with loud, unapologetic confidence.",
                "shoreline, harbour and windswept coast"),
            description: """
            Seagulls are coastal and ocean-going birds of harbours, cliffs, shorelines, open sea and salt wind. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            What distinguishes the line is strong wings, practical bills and bodies conditioned by distance and weather. Those features support wind-reading confidence and a scavenger's eye for opportunity, making the creature feel suited to its ground rather than merely placed there.

            Their calls and silhouettes mark coasts, storms and ships, turning empty water into watched space. Around settlements, roads or camps, seagulls add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Albatross", SizeCategory.Small, 0.35, "Raptor", "bird-small",
            BirdPack("an albatross chick", "a young albatross", "an albatross",
                "It is long-winged and ocean-built, with a sturdy bill and narrow body.",
                "Its immense wings suggest a creature that belongs more to the wind than to the earth.",
                "It gives the impression of effortless distance and cold salt air.",
                "open ocean and lonely cliff rookery"),
            combatStrategyKey: "Beast Swooper",
            description: """
            In harbours, cliffs, shorelines, open sea and salt wind, albatrosses fill the role of coastal and ocean-going birds. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The creature is best read through strong wings, practical bills and bodies conditioned by distance and weather. Those features support wind-reading confidence and a scavenger's eye for opportunity, making the creature feel suited to its ground rather than merely placed there.

            Their calls and silhouettes mark coasts, storms and ships, turning empty water into watched space. Around settlements, roads or camps, albatrosses add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Heron", SizeCategory.Small, 0.2, "Wader", "bird-small",
            BirdPack("a heron chick", "a young heron", "a heron",
                "It is narrow-bodied and long-necked, with immense legs and a dagger bill.",
                "Its posture is all patient angles, every line arranged for stalking and striking into water.",
                "It stands with statuesque stillness until movement becomes necessary.",
                "reedbed, tidal flat and shallow river"),
            description: """
            Herons occupy reedbeds, tidal flats, flooded fields and shallow rivers as long-legged wading birds. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Its strongest visual signs are lifted bodies, long legs, patient necks and bills made for probing or sudden strikes. Those features support measured stillness followed by precise movement, making the creature feel suited to its ground rather than merely placed there.

            They give marshland a ceremonial quiet, as if every pool and reedbed has its own sentinel. Around settlements, roads or camps, herons add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Crane", SizeCategory.Small, 0.2, "Wader", "bird-small",
            BirdPack("a crane chick", "a young crane", "a crane",
                "It is tall, elegant and long-legged, with a lifted neck and measured stride.",
                "Its clean lines and poised head give it a ceremonial, almost courtly quality.",
                "It moves with calm precision and guarded dignity.",
                "wet meadow, marsh and open floodplain"),
            description: """
            Where reedbeds, tidal flats, flooded fields and shallow rivers meets daily life, cranes are recognisable as long-legged wading birds. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The eye is drawn to lifted bodies, long legs, patient necks and bills made for probing or sudden strikes. Those features support measured stillness followed by precise movement, making the creature feel suited to its ground rather than merely placed there.

            They give marshland a ceremonial quiet, as if every pool and reedbed has its own sentinel. Around settlements, roads or camps, cranes add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Flamingo", SizeCategory.Small, 0.2, "Wader", "bird-small",
            BirdPack("a flamingo chick", "a young flamingo", "a flamingo",
                "It is long-legged and improbably narrow, balanced above broad webbed feet.",
                "Its bent bill and tall stance make it look perfectly specialized for filtering food from shallow water.",
                "It seems built from equal parts grace and absurdity.",
                "salt flat, lagoon and shallow estuary"),
            description: """
            Flamingos are long-legged wading birds of reedbeds, tidal flats, flooded fields and shallow rivers. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            At a glance, the form resolves into lifted bodies, long legs, patient necks and bills made for probing or sudden strikes. In motion they are marked by measured stillness followed by precise movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They give marshland a ceremonial quiet, as if every pool and reedbed has its own sentinel. For characters nearby, a flamingo is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Peacock", SizeCategory.Small, 0.2, "Small Bird", "bird-fowl",
            BirdPack("a peachick", "a young peafowl", "a peafowl",
                "It is long-bodied and richly feathered, with a proud neck and elaborate tail coverts.",
                "Its carriage alone suggests a bird that expects to be noticed.",
                "It moves like something born for display rather than concealment.",
                "garden, palace grounds and light woodland"),
            description: """
            In field margins, moors, coops, scrub and forest floors, peacocks fill the role of earth-bound or ground-loving birds. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: sturdy legs, rounded bodies and plumage meant for concealment, display or barnyard work. Watch one move and the emphasis falls on more trust in running, scratching and sudden short flight than in open soaring, a pattern that tells more than size alone.

            They belong to hunting, farming and local noise, small presences that can still dominate a yard or thicket. Around settlements, roads or camps, peacocks add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Ibis", SizeCategory.Small, 0.2, "Wader", "bird-small",
            BirdPack("an ibis chick", "a young ibis", "an ibis",
                "It is slim, long-necked and down-curved of bill, with a tidy compact body.",
                "Its beak looks perfect for probing wet mud and shallow water.",
                "It picks its way through marshy ground with neat concentration.",
                "marsh, floodplain and river shallows"),
            description: """
            In reedbeds, tidal flats, flooded fields and shallow rivers, ibises fill the role of long-legged wading birds. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The visible essentials are lifted bodies, long legs, patient necks and bills made for probing or sudden strikes. In motion they are marked by measured stillness followed by precise movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They give marshland a ceremonial quiet, as if every pool and reedbed has its own sentinel. For characters nearby, a ibis is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Pelican", SizeCategory.Small, 0.5, "Waterfowl", "bird-small",
            BirdPack("a pelican chick", "a young pelican", "a pelican",
                "It is broad-winged and heavy-billed, with a loose throat pouch beneath the beak.",
                "Its enormous bill dominates its entire profile and looks made to scoop whole buckets of water.",
                "It has the ungainly authority of a bird too specialized to care how it looks.",
                "coast, estuary and warm lake"),
            description: """
            In ponds, lakes, marshes, slow rivers and sheltered coasts, pelicans fill the role of broad-bodied water birds. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The first things to register are webbed feet, water-shedding feathers and bills shaped for grazing, filtering or dabbling. In motion they are marked by practical movement between water, mud and bank, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are familiar around water, useful as food or ornament, and capable of surprising force when defending space. Around settlements, roads or camps, pelicans add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Crow", SizeCategory.Small, 0.2, "Small Bird", "bird-small",
            BirdPack("a crow chick", "a young crow", "a crow",
                "It is black-feathered and sharp-beaked, with bright intelligent eyes.",
                "Its careful gaze gives it the unsettling impression of actually considering what it sees.",
                "It carries itself with clever, opportunistic self-possession.",
                "woodland edge, field and refuse heap"),
            description: """
            Crows are black-feathered clever birds of woodland edges, cliffs, fields, refuse heaps and lonely moors. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by dark plumage, strong bills and eyes that seem to weigh what they see. The impression is completed by opportunistic patience and deliberate, intelligent motion, giving the animal a readable rhythm even before it acts.

            They gather around waste, battlefields and roads, where their intelligence makes them feel less like scenery than witnesses. For characters nearby, a crow is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Raven", SizeCategory.Small, 0.2, "Small Bird", "bird-small",
            BirdPack("a raven chick", "a young raven", "a raven",
                "It is larger and heavier than a crow, with deep black plumage and a thick bill.",
                "Its shaggy throat and heavy head give it a sombre, ancient look.",
                "It watches with a grave intensity that seems almost human.",
                "cliff, conifer forest and lonely moor"),
            description: """
            In woodland edges, cliffs, fields, refuse heaps and lonely moors, ravens fill the role of black-feathered clever birds. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by dark plumage, strong bills and eyes that seem to weigh what they see. The impression is completed by opportunistic patience and deliberate, intelligent motion, giving the animal a readable rhythm even before it acts.

            They gather around waste, battlefields and roads, where their intelligence makes them feel less like scenery than witnesses. For characters nearby, a raven is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Emu", SizeCategory.Normal, 0.8, "Flightless Bird", "bird-flightless",
            BirdPack("an emu chick", "a young emu", "an emu",
                "It is tall and long-legged, clothed in shaggy weatherproof feathers.",
                "Its narrow head and powerful thighs make it look built for speed over rough ground.",
                "It seems more likely to outrun danger than to rise above it.",
                "scrubland and open plain"),
            combatStrategyKey: "Beast Physical Avoider",
            description: """
            Emus occupy scrubland, open plain, rainforest floor, fern cover and cold shore as large or specialised flightless birds. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by powerful legs, reduced wings and bodies committed to running, swimming or ground life. The impression is completed by a grounded confidence that replaces flight with speed, strength or stubborn endurance, giving the animal a readable rhythm even before it acts.

            They unsettle expectations of what birds should be, especially when their feet, claws or mass become the main danger. For characters nearby, a emu is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Ostrich", SizeCategory.Normal, 0.8, "Flightless Bird", "bird-flightless",
            BirdPack("an ostrich chick", "a young ostrich", "an ostrich",
                "It is huge, long-necked and massively legged, its body topped by soft black-and-white plumage.",
                "Its feet and legs are terrifyingly strong for any creature willing to stand in kicking distance.",
                "It carries itself with wary hauteur and tremendous athletic tension.",
                "savannah and dry open plain"),
            combatStrategyKey: "Beast Physical Avoider",
            description: """
            Where scrubland, open plain, rainforest floor, fern cover and cold shore meets daily life, ostriches are recognisable as large or specialised flightless birds. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by powerful legs, reduced wings and bodies committed to running, swimming or ground life. The impression is completed by a grounded confidence that replaces flight with speed, strength or stubborn endurance, giving the animal a readable rhythm even before it acts.

            They unsettle expectations of what birds should be, especially when their feet, claws or mass become the main danger. For characters nearby, a ostrich is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Moa", SizeCategory.Normal, 0.8, "Flightless Bird", "bird-flightless",
            BirdPack("a moa chick", "a young moa", "a moa",
                "It is immense and heavy-bodied, with stout legs and a thick neck.",
                "It has the grounded, prehistoric solidity of a bird that never needed the sky.",
                "It lumbers with powerful, long-paced strides.",
                "open forest and grassy upland"),
            combatStrategyKey: "Beast Behemoth",
            description: """
            Moas are large or specialised flightless birds of scrubland, open plain, rainforest floor, fern cover and cold shore. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: powerful legs, reduced wings and bodies committed to running, swimming or ground life. Watch one move and the emphasis falls on a grounded confidence that replaces flight with speed, strength or stubborn endurance, a pattern that tells more than size alone.

            They unsettle expectations of what birds should be, especially when their feet, claws or mass become the main danger. For characters nearby, a moa is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Vulture", SizeCategory.Small, 0.35, "Raptor", "bird-raptor",
            BirdPack("a vulture chick", "a young vulture", "a vulture",
                "It is broad-winged and bare-headed, with a hooked bill and a hunched stance.",
                "Its naked head and deep chest make it look purpose-built for feeding where others would balk.",
                "It has the patient circling assurance of a scavenger with time on its side.",
                "cliff, thermal and arid plain"),
            combatStrategyKey: "Beast Swooper",
            description: """
            Vultures are birds of prey of cliffs, thermals, mountain air, open fields and forest edges. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The most telling cues are hooked beaks, talons, keen eyes and wings designed around height and sudden attack. Those features support watchful patience overhead and decisive violence when they commit, making the creature feel suited to its ground rather than merely placed there.

            They are admired, trained, feared and read as omens because they make the sky feel predatory. For characters nearby, a vulture is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Hawk", SizeCategory.Small, 0.35, "Raptor", "bird-raptor",
            BirdPack("a hawk chick", "a young hawk", "a hawk",
                "It is sharp-winged and compact, with bright predatory eyes and a hooked bill.",
                "Its talons and chest look built for sudden impact and secure killing grip.",
                "It seems alert to every movement around it.",
                "woodland edge, hill country and open field"),
            combatStrategyKey: "Beast Swooper",
            description: """
            In cliffs, thermals, mountain air, open fields and forest edges, hawks fill the role of birds of prey. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Most of its character sits in hooked beaks, talons, keen eyes and wings designed around height and sudden attack. Those features support watchful patience overhead and decisive violence when they commit, making the creature feel suited to its ground rather than merely placed there.

            They are admired, trained, feared and read as omens because they make the sky feel predatory. For characters nearby, a hawk is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Eagle", SizeCategory.Normal, 0.7, "Raptor", "bird-raptor",
            BirdPack("an eaglet", "a young eagle", "an eagle",
                "It is broad-winged and heavy-bodied, with a fierce hooked bill and enormous talons.",
                "Its head and shoulders carry the hard, imperial line of a dominant aerial predator.",
                "It watches the world with remorseless patience.",
                "mountain, cliff and open sky"),
            combatStrategyKey: "Beast Dropper",
            description: """
            Eagles occupy cliffs, thermals, mountain air, open fields and forest edges as birds of prey. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            What distinguishes the line is hooked beaks, talons, keen eyes and wings designed around height and sudden attack. Those features support watchful patience overhead and decisive violence when they commit, making the creature feel suited to its ground rather than merely placed there.

            They are admired, trained, feared and read as omens because they make the sky feel predatory. For characters nearby, a eagle is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Falcon", SizeCategory.Small, 0.35, "Raptor", "bird-raptor",
            BirdPack("a falcon chick", "a young falcon", "a falcon",
                "It is streamlined and narrow-winged, with a small hooked bill and an athlete's frame.",
                "Its wings and body together suggest sudden acceleration more than lingering soar.",
                "It looks taut, precise and built for devastating speed.",
                "cliff face, open plain and high air"),
            combatStrategyKey: "Beast Swooper",
            description: """
            Where cliffs, thermals, mountain air, open fields and forest edges meets daily life, falcons are recognisable as birds of prey. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The creature is best read through hooked beaks, talons, keen eyes and wings designed around height and sudden attack. Those features support watchful patience overhead and decisive violence when they commit, making the creature feel suited to its ground rather than merely placed there.

            They are admired, trained, feared and read as omens because they make the sky feel predatory. For characters nearby, a falcon is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Woodpecker", SizeCategory.Small, 0.35, "Small Bird", "bird-small",
            BirdPack("a woodpecker chick", "a young woodpecker", "a woodpecker",
                "It is compact and upright, with a chisel beak and climbing feet.",
                "Its tail and claws give it the strange purposeful look of a creature designed to cling to bark.",
                "It looks restless, mechanical and industrious all at once.",
                "forest trunk and orchard tree"),
            description: """
            Woodpeckers are bright, clever climbing birds of jungle canopy, warm woodland, orchards and river forests. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            The body plan is clear: hooked beaks, gripping feet, expressive eyes and plumage that refuses to be ignored. Watch one move and the emphasis falls on social mischief, noisy attention and careful manipulation of anything within reach, a pattern that tells more than size alone.

            They are companionable only on their own terms, clever enough to charm a household and ruin a latch. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Bird("Owl", SizeCategory.Small, 0.35, "Raptor", "bird-raptor",
            BirdPack("an owlet", "a young owl", "an owl",
                "It is broad-headed and soft-feathered, with a round face and great front-facing eyes.",
                "Its silent plumage and hooked beak make it look like a patient engine of nocturnal death.",
                "It gives off an eerie stillness even when awake.",
                "forest, ruin and moonlit field"),
            combatStrategyKey: "Beast Swooper",
            description: """
            Owls are birds of prey of cliffs, thermals, mountain air, open fields and forest edges. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Its profile is built around hooked beaks, talons, keen eyes and wings designed around height and sudden attack. In motion they are marked by watchful patience overhead and decisive violence when they commit, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are admired, trained, feared and read as omens because they make the sky feel predatory. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Bird("Kingfisher", SizeCategory.Small, 0.35, "Small Bird", "bird-small",
            BirdPack("a kingfisher chick", "a young kingfisher", "a kingfisher",
                "It is neat and jewel-bright, with a large head and long pointed bill.",
                "Its beak and compact muscular body are perfect for sudden dives into water.",
                "It seems all alert balance and explosive precision.",
                "stream bank and shaded river"),
            description: """
            Where jungle canopy, warm woodland, orchards and river forests meets daily life, kingfishers are recognisable as bright, clever climbing birds. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            Physically they are defined by hooked beaks, gripping feet, expressive eyes and plumage that refuses to be ignored. The impression is completed by social mischief, noisy attention and careful manipulation of anything within reach, giving the animal a readable rhythm even before it acts.

            They are companionable only on their own terms, clever enough to charm a household and ruin a latch. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Bird("Stork", SizeCategory.Small, 0.35, "Wader", "bird-small",
            BirdPack("a stork chick", "a young stork", "a stork",
                "It is tall and clean-lined, carried on long legs beneath a narrow body and powerful wings.",
                "Its long bill and poised stride make it look every inch a wetland hunter.",
                "It advances with quiet confidence and measured economy.",
                "marsh, river shallows and flooded field"),
            description: """
            Storks occupy reedbeds, tidal flats, flooded fields and shallow rivers as long-legged wading birds. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Look closer and the outline is all lifted bodies, long legs, patient necks and bills made for probing or sudden strikes. In motion they are marked by measured stillness followed by precise movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They give marshland a ceremonial quiet, as if every pool and reedbed has its own sentinel. For characters nearby, a stork is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Mandarin Duck", SizeCategory.Small, 0.2, "Waterfowl", "bird-fowl",
            BirdPack("a mandarin duckling", "a young mandarin duck", "a mandarin duck",
                "It is compact and ornate, with layered plumage that can show sails, stripes and polished colour.",
                "Its tidy bill and webbed feet mark it as a duck even beneath its dramatic display feathers.",
                "It moves with quick waterfowl practicality under a startlingly decorative surface.",
                "wooded pond, slow river and sheltered wetland"),
            combatStrategyKey: "Beast Brawler",
            description: """
            Mandarin ducks occupy ponds, lakes, marshes, slow rivers and sheltered coasts as broad-bodied water birds. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Its profile is built around webbed feet, water-shedding feathers and bills shaped for grazing, filtering or dabbling. In motion they are marked by practical movement between water, mud and bank, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are familiar around water, useful as food or ornament, and capable of surprising force when defending space. Around settlements, roads or camps, mandarin ducks add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Red-Crowned Crane", SizeCategory.Small, 0.25, "Wader", "bird-small",
            BirdPack("a red-crowned crane chick", "a young red-crowned crane", "a red-crowned crane",
                "It is tall and white-bodied, with black wing edges and a vivid red crown.",
                "Its long neck, measured stride and precise bearing give it a ceremonial stillness.",
                "It seems made for marshland dances and patient, elegant watchfulness.",
                "reed marsh, snowy wetland and open floodplain"),
            description: """
            Where reedbeds, tidal flats, flooded fields and shallow rivers meets daily life, red-crowned cranes are recognisable as long-legged wading birds. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The first things to register are lifted bodies, long legs, patient necks and bills made for probing or sudden strikes. In motion they are marked by measured stillness followed by precise movement, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They give marshland a ceremonial quiet, as if every pool and reedbed has its own sentinel. For characters nearby, a red-crowned crane is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Macaw", SizeCategory.Small, 0.25, "Small Bird", "bird-small",
            BirdPack("a macaw chick", "a young macaw", "a macaw",
                "It is large for a parrot, bright-feathered and long-tailed, with a heavy hooked beak.",
                "Its strong bill and clever feet make it look capable of worrying apart nut, branch or latch.",
                "It watches noisily and intelligently, confident in both colour and volume.",
                "rainforest canopy and warm river forest"),
            description: """
            Where jungle canopy, warm woodland, orchards and river forests meets daily life, macaws are recognisable as bright, clever climbing birds. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The visible essentials are hooked beaks, gripping feet, expressive eyes and plumage that refuses to be ignored. In motion they are marked by social mischief, noisy attention and careful manipulation of anything within reach, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are companionable only on their own terms, clever enough to charm a household and ruin a latch. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Bird("Toucan", SizeCategory.Small, 0.25, "Small Bird", "bird-small",
            BirdPack("a toucan chick", "a young toucan", "a toucan",
                "It is compact-bodied and dominated by an enormous bright bill.",
                "Its light frame and oversized beak give it a striking, almost impossible silhouette.",
                "It hops and turns with alert canopy confidence.",
                "tropical forest canopy and fruiting river trees"),
            description: """
            Toucans are bright, clever climbing birds of jungle canopy, warm woodland, orchards and river forests. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Physically they are defined by hooked beaks, gripping feet, expressive eyes and plumage that refuses to be ignored. The impression is completed by social mischief, noisy attention and careful manipulation of anything within reach, giving the animal a readable rhythm even before it acts.

            They are companionable only on their own terms, clever enough to charm a household and ruin a latch. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Bird("Hummingbird", SizeCategory.Tiny, 0.05, "Songbird", "bird-small",
            BirdPack("a hummingbird chick", "a young hummingbird", "a hummingbird",
                "It is tiny and jewel-bright, with needle bill, minute feet and wings built for hovering.",
                "Every part of it looks compressed toward speed, precision and nectar-seeking.",
                "It vibrates through the air with impossible urgency.",
                "flowering scrub, garden edge and tropical forest margin"),
            description: """
            Hummingbirds are small birds of hedges, eaves and woodland edges of gardens, thickets, orchards, hedgerows and open sky. Their habits give them a visible place in the scene, whether they are glimpsed in the wild, kept near habitation or encountered at the edge of danger.

            Its strongest visual signs are light bones, neat plumage, quick beaks and wings made for brief, precise flights. Those features support flickering hops, sudden flutters and a constant awareness of cover, making the creature feel suited to its ground rather than merely placed there.

            They add movement and sound to settled places, becoming part of the weather of daily life. A hummingbird can serve as background wildlife, valuable domestic animal, troublesome vermin or a serious hazard, depending on the scene and the way nearby people have learned to live with it.
            """);
        yield return Bird("Condor", SizeCategory.Normal, 0.7, "Raptor", "bird-raptor",
            BirdPack("a condor chick", "a young condor", "a condor",
                "It is immense and black-winged, with pale flight feathers and a bare watchful head.",
                "Its wingspan and patient gaze make it look like a cliff wind given bone and appetite.",
                "It carries the grave economy of a scavenger that can wait longer than hunger can hide.",
                "Andean cliff, high thermal and open mountain sky"),
            combatStrategyKey: "Beast Swooper",
            description: """
            In cliffs, thermals, mountain air, open fields and forest edges, condors fill the role of birds of prey. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The creature presents hooked beaks, talons, keen eyes and wings designed around height and sudden attack. In motion they are marked by watchful patience overhead and decisive violence when they commit, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are admired, trained, feared and read as omens because they make the sky feel predatory. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Bird("Rhea", SizeCategory.Normal, 0.7, "Flightless Bird", "bird-flightless",
            BirdPack("a rhea chick", "a young rhea", "a rhea",
                "It is long-legged and grey-brown, with a small head and soft, loose feathers.",
                "Its body has the open-country build of a bird that trusts running far more than hiding.",
                "It moves with wary speed across wide grassland.",
                "pampas, scrub plain and open savannah"),
            combatStrategyKey: "Beast Physical Avoider",
            description: """
            In scrubland, open plain, rainforest floor, fern cover and cold shore, rheas fill the role of large or specialised flightless birds. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            The body plan is clear: powerful legs, reduced wings and bodies committed to running, swimming or ground life. Watch one move and the emphasis falls on a grounded confidence that replaces flight with speed, strength or stubborn endurance, a pattern that tells more than size alone.

            They unsettle expectations of what birds should be, especially when their feet, claws or mass become the main danger. For characters nearby, a rhea is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Hoatzin", SizeCategory.Small, 0.2, "Small Bird", "bird-fowl",
            BirdPack("a hoatzin chick", "a young hoatzin", "a hoatzin",
                "It is crested and long-tailed, with chestnut and olive plumage arranged in untidy splendour.",
                "Its odd body and leaf-eating habits give it an ancient, swamp-bound awkwardness.",
                "It clambers and flutters with more character than grace.",
                "flooded forest, oxbow lake and river thicket"),
            combatStrategyKey: "Beast Coward",
            description: """
            Hoatzins occupy field margins, moors, coops, scrub and forest floors as earth-bound or ground-loving birds. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: sturdy legs, rounded bodies and plumage meant for concealment, display or barnyard work. Watch one move and the emphasis falls on more trust in running, scratching and sudden short flight than in open soaring, a pattern that tells more than size alone.

            They belong to hunting, farming and local noise, small presences that can still dominate a yard or thicket. Around settlements, roads or camps, hoatzins add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Cockatoo", SizeCategory.Small, 0.25, "Small Bird", "bird-small",
            BirdPack("a cockatoo chick", "a young cockatoo", "a cockatoo",
                "It is pale and strong-beaked, with an expressive crest and intelligent dark eyes.",
                "Its bill and gripping feet suggest a bird equally ready to crack seed or test anything left unattended.",
                "It carries itself with bright, social mischief.",
                "eucalypt woodland, orchard edge and open forest"),
            description: """
            In jungle canopy, warm woodland, orchards and river forests, cockatoos fill the role of bright, clever climbing birds. They are noticed through movement, sound, tracks and local caution as much as through the body itself.

            Physically they are defined by hooked beaks, gripping feet, expressive eyes and plumage that refuses to be ignored. The impression is completed by social mischief, noisy attention and careful manipulation of anything within reach, giving the animal a readable rhythm even before it acts.

            They are companionable only on their own terms, clever enough to charm a household and ruin a latch. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Bird("Kookaburra", SizeCategory.Small, 0.25, "Small Bird", "bird-small",
            BirdPack("a kookaburra chick", "a young kookaburra", "a kookaburra",
                "It is sturdy and big-headed, with a long heavy bill and compact brown-and-white plumage.",
                "Its whole body seems arranged around a sudden downward strike from a branch.",
                "It watches the ground with blunt predatory patience.",
                "woodland branch, river gum and open forest"),
            combatStrategyKey: "Beast Swooper",
            description: """
            Kookaburras occupy jungle canopy, warm woodland, orchards and river forests as bright, clever climbing birds. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            Physically they are defined by hooked beaks, gripping feet, expressive eyes and plumage that refuses to be ignored. The impression is completed by social mischief, noisy attention and careful manipulation of anything within reach, giving the animal a readable rhythm even before it acts.

            They are companionable only on their own terms, clever enough to charm a household and ruin a latch. Their value in a scene comes from making the environment feel inhabited, with human plans forced to account for another creature's needs.
            """);
        yield return Bird("Cassowary", SizeCategory.Normal, 0.9, "Flightless Bird", "bird-flightless",
            BirdPack("a cassowary chick", "a young cassowary", "a cassowary",
                "It is tall, black-feathered and heavy-legged, with a casque rising from a brilliantly coloured head.",
                "Its deep body, dagger-like claws and forward drive make it look dangerously prehistoric.",
                "It has the tense, private menace of a forest bird that does not like being approached.",
                "rainforest floor and dense tropical thicket"),
            combatStrategyKey: "Beast Brawler",
            description: """
            Cassowaries occupy scrubland, open plain, rainforest floor, fern cover and cold shore as large or specialised flightless birds. Their lives are shaped by feeding, sheltering, avoiding danger and exploiting the opportunities their build allows.

            The body plan is clear: powerful legs, reduced wings and bodies committed to running, swimming or ground life. Watch one move and the emphasis falls on a grounded confidence that replaces flight with speed, strength or stubborn endurance, a pattern that tells more than size alone.

            They unsettle expectations of what birds should be, especially when their feet, claws or mass become the main danger. For characters nearby, a cassowary is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Lyrebird", SizeCategory.Small, 0.2, "Small Bird", "bird-fowl",
            BirdPack("a lyrebird chick", "a young lyrebird", "a lyrebird",
                "It is brown and ground-dwelling, with strong legs and an elaborate trailing tail in full display.",
                "Its careful posture and bright eye give it the air of a performer that notices everything.",
                "It slips through leaf litter with secretive, theatrical precision.",
                "damp forest floor and ferny gully"),
            combatStrategyKey: "Beast Coward",
            description: """
            Where field margins, moors, coops, scrub and forest floors meets daily life, lyrebirds are recognisable as earth-bound or ground-loving birds. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: sturdy legs, rounded bodies and plumage meant for concealment, display or barnyard work. Watch one move and the emphasis falls on more trust in running, scratching and sudden short flight than in open soaring, a pattern that tells more than size alone.

            They belong to hunting, farming and local noise, small presences that can still dominate a yard or thicket. Around settlements, roads or camps, lyrebirds add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
        yield return Bird("Kiwi", SizeCategory.Small, 0.2, "Small Bird", "bird-flightless",
            BirdPack("a kiwi chick", "a young kiwi", "a kiwi",
                "It is round, brown and almost wingless, with hairlike feathers and a long probing bill.",
                "Its nose-led face and sturdy legs make it look built for searching through dark leaf litter.",
                "It shuffles with shy nocturnal purpose.",
                "forest floor, fern cover and damp burrow"),
            combatStrategyKey: "Beast Coward",
            description: """
            Where scrubland, open plain, rainforest floor, fern cover and cold shore meets daily life, kiwis are recognisable as large or specialised flightless birds. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The body plan is clear: powerful legs, reduced wings and bodies committed to running, swimming or ground life. Watch one move and the emphasis falls on a grounded confidence that replaces flight with speed, strength or stubborn endurance, a pattern that tells more than size alone.

            They unsettle expectations of what birds should be, especially when their feet, claws or mass become the main danger. For characters nearby, a kiwi is best treated as a living presence with habits and limits, not as scenery waiting to be ignored.
            """);
        yield return Bird("Penguin", SizeCategory.Small, 0.2, "Waterfowl", "bird-fowl",
            BirdPack("a penguin chick", "a young penguin", "a penguin",
                "It is dense-bodied and short-winged, with flipper-like limbs and tight waterproof feathers.",
                "Its upright stance looks almost comical until its heavy body turns decisively toward the water.",
                "It seems awkward on land in exactly the way something superb underwater often does.",
                "icy shore and cold sea"),
            description: """
            Where ponds, lakes, marshes, slow rivers and sheltered coasts meets daily life, penguins are recognisable as broad-bodied water birds. Their presence can be ordinary, useful, troublesome or alarming depending on how close they come.

            The creature presents webbed feet, water-shedding feathers and bills shaped for grazing, filtering or dabbling. In motion they are marked by practical movement between water, mud and bank, so even a quiet specimen suggests the instincts and pressures that shaped it.

            They are familiar around water, useful as food or ornament, and capable of surprising force when defending space. Around settlements, roads or camps, penguins add practical consequences: food, noise, labour, risk, nuisance or warning.
            """);
    }
}
