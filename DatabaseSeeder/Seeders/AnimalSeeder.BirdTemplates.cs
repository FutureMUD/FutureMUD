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
            string? description = null,
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
                "town squares, rooftops and cliff ledges"));
        yield return Bird("Parrot", SizeCategory.VerySmall, 0.1, "Small Bird", "bird-small",
            BirdPack("a parrot chick", "a young parrot", "a parrot",
                "It is bright-feathered and compact, with a hooked beak and expressive eyes.",
                "Its beak and feet both look clever enough to manipulate seed, perch and object alike.",
                "It watches the world with obvious intelligence and curiosity.",
                "jungle canopy and warm woodland"));
        yield return Bird("Swallow", SizeCategory.VerySmall, 0.1, "Songbird", "bird-small",
            BirdPack("a swallow chick", "a young swallow", "a swallow",
                "It is slim and delicate, with pointed wings and a forked tail.",
                "Its entire body looks designed around speed and precision in the air.",
                "It seems almost too restless to stay still for long.",
                "barn eaves, cliff faces and open sky"));
        yield return Bird("Sparrow", SizeCategory.VerySmall, 0.05, "Songbird", "bird-small",
            BirdPack("a sparrow chick", "a young sparrow", "a sparrow",
                "It is tiny and brown-feathered, with a compact beak and bright round eye.",
                "Its short hops and quick movements make it look like a living scrap of feather and urgency.",
                "It busies itself with constant pecks, flutters and little bursts of motion.",
                "hedges, eaves and garden edge"));
        yield return Bird("Finch", SizeCategory.VerySmall, 0.05, "Songbird", "bird-small",
            BirdPack("a finch chick", "a young finch", "a finch",
                "It is tiny, neat and brightly marked, with a short seed-cracking beak.",
                "Its trim profile and clean plumage give it a delicate, jewel-like quality.",
                "It flicks and chirrs with lively, nervous energy.",
                "woodland edge, scrub and orchard"));
        yield return Bird("Robin", SizeCategory.VerySmall, 0.05, "Songbird", "bird-small",
            BirdPack("a robin chick", "a young robin", "a robin",
                "It is small and round-bodied, with soft plumage and an alert stance.",
                "Its breast stands out brightly against the softer browns and greys of the rest of its body.",
                "It looks bold for so small a bird, ready to claim a branch or patch of ground as its own.",
                "woodland edge, garden and hedgerow"));
        yield return Bird("Wren", SizeCategory.VerySmall, 0.05, "Songbird", "bird-small",
            BirdPack("a wren chick", "a young wren", "a wren",
                "It is tiny and compact, with a cocked tail and fine brown plumage.",
                "Its small sharp beak and bright eye make it look busy even while standing still.",
                "It seems constantly on the verge of vanishing into a crack or clump of brush.",
                "hedge, thicket and undergrowth"));
        yield return Bird("Quail", SizeCategory.VerySmall, 0.1, "Small Bird", "bird-fowl",
            BirdPack("a quail chick", "a young quail", "a quail",
                "It is squat and earth-toned, with rounded wings and a neat compact body.",
                "Its markings are ideal for melting into dry grass and leaf litter.",
                "It looks much happier running under cover than taking to the air.",
                "grassland and scrub undergrowth"),
            combatStrategyKey: "Beast Coward");
        yield return Bird("Duck", SizeCategory.Small, 0.2, "Waterfowl", "bird-fowl",
            BirdPack("a duckling", "a young drake", "a duck",
                "It is broad-billed and web-footed, with water-shedding plumage and a low-slung body.",
                "Its bill and feet make its life on water obvious at a glance.",
                "It carries itself with a comic, practical assurance.",
                "pond, marsh and slow river"),
            combatStrategyKey: "Beast Brawler");
        yield return Bird("Goose", SizeCategory.Small, 0.4, "Waterfowl", "bird-fowl",
            BirdPack("a gosling", "a young goose", "a goose",
                "It is long-necked and broad-bodied, with a heavy bill and strong webbed feet.",
                "Its reach and angry hiss suggest that it is much more formidable than its shape first implies.",
                "It looks argumentative even at rest.",
                "lake edge, marsh pasture and open water"),
            combatStrategyKey: "Beast Brawler");
        yield return Bird("Swan", SizeCategory.Small, 0.4, "Waterfowl", "bird-fowl",
            BirdPack("a cygnet", "a young swan", "a swan",
                "It is long-necked and elegantly proportioned, clothed in clean layered plumage.",
                "Its broad wings and poised carriage give it a regal, almost theatrical profile.",
                "It glides or stalks with deliberate grace.",
                "lake, broad river and ornamental water"),
            combatStrategyKey: "Beast Brawler");
        yield return Bird("Grouse", SizeCategory.Small, 0.2, "Small Bird", "bird-fowl",
            BirdPack("a grouse chick", "a young grouse", "a grouse",
                "It is round-bodied and heavily feathered, marked in mottled browns and greys.",
                "Its camouflage is excellent, and its dense plumage suits cold scrub and rough ground.",
                "It seems far more comfortable bursting into brief noisy flight than soaring.",
                "moor, heath and rough forest floor"));
        yield return Bird("Pheasant", SizeCategory.Small, 0.2, "Small Bird", "bird-fowl",
            BirdPack("a pheasant chick", "a young pheasant", "a pheasant",
                "It is long-tailed and elegant, with layered feathers and a light but athletic build.",
                "Its head and neck carry the sort of colouring that can flare brilliantly in full-grown males.",
                "It has the alert, skittish poise of a bird that trusts its legs before its wings.",
                "field margin, light woodland and hedgerow"));
        yield return Bird("Chicken", SizeCategory.Small, 0.2, "Waterfowl", "bird-fowl",
            BirdPack("a chick", "a young chicken", "a chicken",
                "It is compact and practical, with a thick body, short wings and sturdy scratching feet.",
                "Its comb, wattles and blunt beak give it an unmistakably domestic look.",
                "It fusses and scratches with constant barnyard purpose.",
                "yard, coop and farmstead"),
            combatStrategyKey: "Beast Brawler");
        yield return Bird("Turkey", SizeCategory.Small, 0.35, "Waterfowl", "bird-fowl",
            BirdPack("a poult", "a young turkey", "a turkey",
                "It is broad and heavy, with a fan-like tail and powerful legs.",
                "Its bare head and fleshy wattles give it an oddly severe look.",
                "It struts with a mixture of caution and bluster.",
                "woodland edge and rough pasture"),
            combatStrategyKey: "Beast Brawler");
        yield return Bird("Seagull", SizeCategory.Small, 0.2, "Small Bird", "bird-small",
            BirdPack("a gull chick", "a young gull", "a gull",
                "It is light-bodied and strong-winged, with a sharp bill and webbed feet.",
                "Its hard eyes and hooked posture suggest a bird always ready to steal or scavenge.",
                "It moves with loud, unapologetic confidence.",
                "shoreline, harbour and windswept coast"));
        yield return Bird("Albatross", SizeCategory.Small, 0.35, "Raptor", "bird-small",
            BirdPack("an albatross chick", "a young albatross", "an albatross",
                "It is long-winged and ocean-built, with a sturdy bill and narrow body.",
                "Its immense wings suggest a creature that belongs more to the wind than to the earth.",
                "It gives the impression of effortless distance and cold salt air.",
                "open ocean and lonely cliff rookery"),
            combatStrategyKey: "Beast Swooper");
        yield return Bird("Heron", SizeCategory.Small, 0.2, "Wader", "bird-small",
            BirdPack("a heron chick", "a young heron", "a heron",
                "It is narrow-bodied and long-necked, with immense legs and a dagger bill.",
                "Its posture is all patient angles, every line arranged for stalking and striking into water.",
                "It stands with statuesque stillness until movement becomes necessary.",
                "reedbed, tidal flat and shallow river"));
        yield return Bird("Crane", SizeCategory.Small, 0.2, "Wader", "bird-small",
            BirdPack("a crane chick", "a young crane", "a crane",
                "It is tall, elegant and long-legged, with a lifted neck and measured stride.",
                "Its clean lines and poised head give it a ceremonial, almost courtly quality.",
                "It moves with calm precision and guarded dignity.",
                "wet meadow, marsh and open floodplain"));
        yield return Bird("Flamingo", SizeCategory.Small, 0.2, "Wader", "bird-small",
            BirdPack("a flamingo chick", "a young flamingo", "a flamingo",
                "It is long-legged and improbably narrow, balanced above broad webbed feet.",
                "Its bent bill and tall stance make it look perfectly specialized for filtering food from shallow water.",
                "It seems built from equal parts grace and absurdity.",
                "salt flat, lagoon and shallow estuary"));
        yield return Bird("Peacock", SizeCategory.Small, 0.2, "Small Bird", "bird-fowl",
            BirdPack("a peachick", "a young peafowl", "a peafowl",
                "It is long-bodied and richly feathered, with a proud neck and elaborate tail coverts.",
                "Its carriage alone suggests a bird that expects to be noticed.",
                "It moves like something born for display rather than concealment.",
                "garden, palace grounds and light woodland"));
        yield return Bird("Ibis", SizeCategory.Small, 0.2, "Wader", "bird-small",
            BirdPack("an ibis chick", "a young ibis", "an ibis",
                "It is slim, long-necked and down-curved of bill, with a tidy compact body.",
                "Its beak looks perfect for probing wet mud and shallow water.",
                "It picks its way through marshy ground with neat concentration.",
                "marsh, floodplain and river shallows"));
        yield return Bird("Pelican", SizeCategory.Small, 0.5, "Waterfowl", "bird-small",
            BirdPack("a pelican chick", "a young pelican", "a pelican",
                "It is broad-winged and heavy-billed, with a loose throat pouch beneath the beak.",
                "Its enormous bill dominates its entire profile and looks made to scoop whole buckets of water.",
                "It has the ungainly authority of a bird too specialized to care how it looks.",
                "coast, estuary and warm lake"));
        yield return Bird("Crow", SizeCategory.Small, 0.2, "Small Bird", "bird-small",
            BirdPack("a crow chick", "a young crow", "a crow",
                "It is black-feathered and sharp-beaked, with bright intelligent eyes.",
                "Its careful gaze gives it the unsettling impression of actually considering what it sees.",
                "It carries itself with clever, opportunistic self-possession.",
                "woodland edge, field and refuse heap"));
        yield return Bird("Raven", SizeCategory.Small, 0.2, "Small Bird", "bird-small",
            BirdPack("a raven chick", "a young raven", "a raven",
                "It is larger and heavier than a crow, with deep black plumage and a thick bill.",
                "Its shaggy throat and heavy head give it a sombre, ancient look.",
                "It watches with a grave intensity that seems almost human.",
                "cliff, conifer forest and lonely moor"));
        yield return Bird("Emu", SizeCategory.Normal, 0.8, "Flightless Bird", "bird-flightless",
            BirdPack("an emu chick", "a young emu", "an emu",
                "It is tall and long-legged, clothed in shaggy weatherproof feathers.",
                "Its narrow head and powerful thighs make it look built for speed over rough ground.",
                "It seems more likely to outrun danger than to rise above it.",
                "scrubland and open plain"),
            combatStrategyKey: "Beast Skirmisher");
        yield return Bird("Ostrich", SizeCategory.Normal, 0.8, "Flightless Bird", "bird-flightless",
            BirdPack("an ostrich chick", "a young ostrich", "an ostrich",
                "It is huge, long-necked and massively legged, its body topped by soft black-and-white plumage.",
                "Its feet and legs are terrifyingly strong for any creature willing to stand in kicking distance.",
                "It carries itself with wary hauteur and tremendous athletic tension.",
                "savannah and dry open plain"),
            combatStrategyKey: "Beast Skirmisher");
        yield return Bird("Moa", SizeCategory.Normal, 0.8, "Flightless Bird", "bird-flightless",
            BirdPack("a moa chick", "a young moa", "a moa",
                "It is immense and heavy-bodied, with stout legs and a thick neck.",
                "It has the grounded, prehistoric solidity of a bird that never needed the sky.",
                "It lumbers with powerful, long-paced strides.",
                "open forest and grassy upland"),
            combatStrategyKey: "Beast Behemoth");
        yield return Bird("Vulture", SizeCategory.Small, 0.35, "Raptor", "bird-raptor",
            BirdPack("a vulture chick", "a young vulture", "a vulture",
                "It is broad-winged and bare-headed, with a hooked bill and a hunched stance.",
                "Its naked head and deep chest make it look purpose-built for feeding where others would balk.",
                "It has the patient circling assurance of a scavenger with time on its side.",
                "cliff, thermal and arid plain"),
            combatStrategyKey: "Beast Swooper");
        yield return Bird("Hawk", SizeCategory.Small, 0.35, "Raptor", "bird-raptor",
            BirdPack("a hawk chick", "a young hawk", "a hawk",
                "It is sharp-winged and compact, with bright predatory eyes and a hooked bill.",
                "Its talons and chest look built for sudden impact and secure killing grip.",
                "It seems alert to every movement around it.",
                "woodland edge, hill country and open field"),
            combatStrategyKey: "Beast Swooper");
        yield return Bird("Eagle", SizeCategory.Normal, 0.7, "Raptor", "bird-raptor",
            BirdPack("an eaglet", "a young eagle", "an eagle",
                "It is broad-winged and heavy-bodied, with a fierce hooked bill and enormous talons.",
                "Its head and shoulders carry the hard, imperial line of a dominant aerial predator.",
                "It watches the world with remorseless patience.",
                "mountain, cliff and open sky"),
            combatStrategyKey: "Beast Swooper");
        yield return Bird("Falcon", SizeCategory.Small, 0.35, "Raptor", "bird-raptor",
            BirdPack("a falcon chick", "a young falcon", "a falcon",
                "It is streamlined and narrow-winged, with a small hooked bill and an athlete's frame.",
                "Its wings and body together suggest sudden acceleration more than lingering soar.",
                "It looks taut, precise and built for devastating speed.",
                "cliff face, open plain and high air"),
            combatStrategyKey: "Beast Swooper");
        yield return Bird("Woodpecker", SizeCategory.Small, 0.35, "Small Bird", "bird-small",
            BirdPack("a woodpecker chick", "a young woodpecker", "a woodpecker",
                "It is compact and upright, with a chisel beak and climbing feet.",
                "Its tail and claws give it the strange purposeful look of a creature designed to cling to bark.",
                "It looks restless, mechanical and industrious all at once.",
                "forest trunk and orchard tree"));
        yield return Bird("Owl", SizeCategory.Small, 0.35, "Raptor", "bird-raptor",
            BirdPack("an owlet", "a young owl", "an owl",
                "It is broad-headed and soft-feathered, with a round face and great front-facing eyes.",
                "Its silent plumage and hooked beak make it look like a patient engine of nocturnal death.",
                "It gives off an eerie stillness even when awake.",
                "forest, ruin and moonlit field"),
            combatStrategyKey: "Beast Swooper");
        yield return Bird("Kingfisher", SizeCategory.Small, 0.35, "Small Bird", "bird-small",
            BirdPack("a kingfisher chick", "a young kingfisher", "a kingfisher",
                "It is neat and jewel-bright, with a large head and long pointed bill.",
                "Its beak and compact muscular body are perfect for sudden dives into water.",
                "It seems all alert balance and explosive precision.",
                "stream bank and shaded river"));
        yield return Bird("Stork", SizeCategory.Small, 0.35, "Wader", "bird-small",
            BirdPack("a stork chick", "a young stork", "a stork",
                "It is tall and clean-lined, carried on long legs beneath a narrow body and powerful wings.",
                "Its long bill and poised stride make it look every inch a wetland hunter.",
                "It advances with quiet confidence and measured economy.",
                "marsh, river shallows and flooded field"));
        yield return Bird("Penguin", SizeCategory.Small, 0.2, "Waterfowl", "bird-fowl",
            BirdPack("a penguin chick", "a young penguin", "a penguin",
                "It is dense-bodied and short-winged, with flipper-like limbs and tight waterproof feathers.",
                "Its upright stance looks almost comical until its heavy body turns decisively toward the water.",
                "It seems awkward on land in exactly the way something superb underwater often does.",
                "icy shore and cold sea"));
    }
}
