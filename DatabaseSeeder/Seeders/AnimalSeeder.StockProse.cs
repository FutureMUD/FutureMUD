#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private static readonly HashSet<string> DomesticAnimalRaceNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Dog",
        "Cat",
        "Horse",
        "Cow",
        "Ox",
        "Sheep",
        "Goat",
        "Pig",
        "Llama",
        "Alpaca",
        "Donkey",
        "Mule"
    };

    private static readonly HashSet<string> PredatorAnimalRaceNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Wolf",
        "Fox",
        "Coyote",
        "Hyena",
        "Lion",
        "Tiger",
        "Cheetah",
        "Leopard",
        "Panther",
        "Jaguar",
        "Jackal",
        "Bear",
        "Crocodile",
        "Alligator",
        "Shark",
        "Eagle",
        "Hawk",
        "Falcon",
        "Owl",
        "Python",
        "Cobra",
        "Viper"
    };

    private static readonly IReadOnlyDictionary<string, string> AnimalEthnicityDescriptions =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Dog:Terrier"] = SeederDescriptionHelpers.JoinParagraphs(
                "Terriers are compact, wiry dogs bred to go to ground after vermin, and even the stock terrier line looks as though it was built to wriggle into burrows and come back bloody but victorious.",
                "They tend toward dense coats, hard little bodies and a bright, confrontational alertness. Every part of the breed suggests relentless forward energy rather than patience or decorative calm.",
                "Around people, terriers are valued for pest control, courage and stubborn loyalty, though the same fearlessness that makes them useful also makes them difficult animals to intimidate or fully quiet."
            ),
            ["Dog:Setter"] = SeederDescriptionHelpers.JoinParagraphs(
                "Setters are long-bodied hunting dogs bred to quarter ground, catch scent on the air and hold a poised point when game is near, giving the stock line an elegant but clearly functional silhouette.",
                "They are usually leaner and silkier than rough working breeds, with disciplined movement and a carriage that keeps the head high. Even at rest they look like animals listening for instruction and distant birds.",
                "People keep setters for cooperative field work, where intelligence, trainability and stamina matter as much as speed. In domestic settings they often read as refined sporting dogs rather than rough kennel stock."
            ),
            ["Dog:Pointer"] = SeederDescriptionHelpers.JoinParagraphs(
                "Pointers are built around one signature behaviour: the arrested stillness of a dog that has found hidden game and turned its whole body into a directional signal for the hunter behind it.",
                "The stock pointer line is spare, athletic and air-scenting, with long legs, a clean frame and the tense balance of an animal that expects to stop hard, focus and explode forward again at command.",
                "In human hands pointers are specialists, prized where coordinated hunting matters. Their usefulness comes not from brute force, but from precision, discipline and the ability to make the invisible visible."
            ),
            ["Dog:Retriever"] = SeederDescriptionHelpers.JoinParagraphs(
                "Retrievers are larger sporting dogs bred to go out after downed game and bring it back intact, so the stock line favours a generous mouth, steady nerves and a cooperative temper.",
                "They typically present as balanced, broad-chested and strong without looking coarse. Their movement suggests endurance and willingness rather than frenzy, and their faces often read as open and attentive.",
                "Around people, retrievers bridge work and companionship especially well. Hunters value the breed for reliable field performance, while households prize the same patience and eagerness to please."
            ),
            ["Dog:Spaniel"] = SeederDescriptionHelpers.JoinParagraphs(
                "Spaniels are flushing dogs first and foremost, bred to work close to brush, reeds and dense cover where hidden birds or small game must be put suddenly into motion.",
                "The stock spaniel line combines a soft, feathery coat with a compact, busy frame and pendulous ears. They look like dogs made to crash through cover, circle back and remain eager for the next command.",
                "People value spaniels as affectionate companions and as practical field dogs. Their friendliness softens their sporting origins, but the breed still carries a lively working intelligence under its softer outline."
            ),
            ["Dog:Water Dog"] = SeederDescriptionHelpers.JoinParagraphs(
                "Water dogs are specialised swimmers and retrievers, shaped for cold water, repeated dives and the awkward labour of bringing birds or equipment back through chop and current.",
                "The stock line is usually dense-coated through the torso, strong through the shoulders and noticeably at home in wet conditions. Even on land they often seem like animals waiting to be given something to fetch from the water.",
                "People keep water dogs aboard boats, around marshes and anywhere a sure-swimming companion can turn accident or missed shot into recoverable work. Their usefulness makes them welcome far beyond ornamental value."
            ),
            ["Dog:Sighthound"] = SeederDescriptionHelpers.JoinParagraphs(
                "Sighthounds are pursuit dogs bred to run down visible prey in the open, and the stock line is all long levers, deep chest and finely tuned acceleration rather than wrestling strength.",
                "They are narrow-waisted, long-muzzled and visibly aerodynamic, with a flexible spine and a gait that always seems on the verge of becoming a sprint. Even standing still, they read as animals designed by distance and speed.",
                "To people, sighthounds are tools of coursing, prestige and spectacle. Their beauty is inseparable from their function, because the same lines that make them elegant are the ones that let them outrun living things."
            ),
            ["Dog:Scenthound"] = SeederDescriptionHelpers.JoinParagraphs(
                "Scenthounds are tracking specialists bred to follow an odour trail long after sight is useless, so the stock line is defined less by grace than by persistence, nasal power and dogged concentration.",
                "They tend to be low, long and loose-skinned, with pendulous ears and heavy mouths that help mark them as olfactory specialists. Their whole build suggests an animal meant to stay nose-down and keep going.",
                "People prize scenthounds anywhere trails matter more than speed, from hunting to search work. They are practical dogs of patience and method, not theatrical bursts of brilliance."
            ),
            ["Dog:Bulldog"] = SeederDescriptionHelpers.JoinParagraphs(
                "Bulldogs are compact, broad-fronted dogs whose stock line still carries the exaggerated jaw, chest and shoulder strength associated with rough handling and confrontational breeding histories.",
                "They present as squat, heavy-headed and physically stubborn, with a short muzzle and a body that looks built to brace and push. The impression is one of blunt endurance rather than speed.",
                "Around people, bulldogs sit uneasily between companion breed and reminder of harsher working origins. Their modern appeal often comes from loyalty and recognisable looks, even while the body still advertises its ancestry."
            ),
            ["Dog:Mastiff"] = SeederDescriptionHelpers.JoinParagraphs(
                "Mastiffs are giant guard dogs, and the stock line is defined by scale, mass and the quiet confidence of an animal that rarely needs to prove its strength twice.",
                "They are broad-skulled, deep-bodied and heavy-limbed, with a size that dominates space before they ever make a threatening movement. Their slowness reads less as laziness than as stored force.",
                "People keep mastiffs to guard households, compounds and prestige holdings. Much of their value lies in presence alone, because a dog this large can deter trouble before any bite is required."
            ),
            ["Dog:Herding Dog"] = SeederDescriptionHelpers.JoinParagraphs(
                "Herding dogs are workers bred to manipulate larger animals through movement, intelligence and pressure rather than brute force, giving the stock line a quick, reactive and highly attentive quality.",
                "They tend to be agile, observant and lightly coiled, with bodies suited to abrupt turns and repeated bursts of speed. Even idle individuals often look like they are mapping motion and waiting for something to organise.",
                "Among people, herding dogs are valued for responsiveness and problem-solving as much as livestock skill. They transition well into companionship precisely because their core trait is cooperative attentiveness."
            ),
            ["Dog:Lap Dog"] = SeederDescriptionHelpers.JoinParagraphs(
                "Lap dogs are companion breeds shaped as much by human preference as by any hard external environment, and the stock line emphasises small size, neotenous features and intimate familiarity with indoor life.",
                "They are usually fine-boned, expressive and immediately approachable in scale, with large eyes, soft coats or decorative outlines that make them read as personal rather than utilitarian animals.",
                "People keep lap dogs for presence, status and affection. Their value lies in constant proximity, emotional attachment and the way they fold neatly into the rhythms of domestic life."
            ),
            ["Dog:Mongrel"] = SeederDescriptionHelpers.JoinParagraphs(
                "Mongrels are mixed dogs without a single fixed pedigree, so the stock line represents hardy commonality rather than a carefully curated type and often reads as the practical dog left behind by breed fashion.",
                "They vary more than pure lines in coat, proportions and facial detail, but many carry a balanced sturdiness that comes from not being driven toward one extreme silhouette or specialised behaviour alone.",
                "In human settlements mongrels are everyday dogs: companions, scavengers, guardians and opportunists. Their adaptability is their defining strength, and people often trust them because they are so plainly creatures of lived reality."
            ),
            ["Bear:Black Bear"] = SeederDescriptionHelpers.JoinParagraphs(
                "Black bears are the smaller and generally more cautious members of the stock bear line, their build compact enough to climb well but still carrying the unmistakable authority of a heavy omnivore.",
                "They are dark-coated, strong through the shoulders and equipped with claws suited to both foraging and defence. Compared to larger bear species, they often look quicker, warier and more inclined to retreat than stand ground.",
                "Where people share territory with black bears, the relationship is defined by boundaries, food security and respectful caution. They are dangerous, but less because they seek conflict than because they are immensely strong animals living close to settlement edges."
            ),
            ["Bear:Moon Bear"] = SeederDescriptionHelpers.JoinParagraphs(
                "Moon bears are recognisable at once by the pale crescent across the chest, a mark that gives the stock line a dramatic contrast against otherwise dark fur and a slightly more arboreal cast than heavier bears.",
                "They remain solid, clawed omnivores, but often read as more agile and climbing-oriented than bulkier northern cousins. Their body language mixes curiosity, strength and abrupt volatility.",
                "To people, moon bears occupy the complicated space between striking emblematic animal and hazardous wild neighbour. Their memorable appearance makes them vivid in story, but their temperament still demands distance."
            ),
            ["Bear:Brown Bear"] = SeederDescriptionHelpers.JoinParagraphs(
                "Brown bears are large, muscular omnivores whose stock line emphasises hump-backed shoulder power, heavy skulls and the physical confidence of an animal that can take almost any space it wants.",
                "They tend to look thicker, rougher and more territorially assured than smaller bear species. The body is all weight, claw reach and stored momentum, whether the animal is foraging quietly or rising to threaten.",
                "Around people, brown bears are treated with deep caution and a certain grim respect. They are game, danger, legend and ecological force all at once, impossible to ignore in any landscape they share."
            ),
            ["Bear:Polar Bear"] = SeederDescriptionHelpers.JoinParagraphs(
                "Polar bears are the most cold-specialised of the stock bear ethnicities, their pale coats, long frames and heavy paws advertising a life spent on ice, coast and freezing water.",
                "They are leaner in outline than some inland bears, but no less powerful for it. The long neck, white fur and predatory head make them read less as omnivores of the woods than as marine hunters cast onto the land.",
                "To people, polar bears represent raw environmental hostility made flesh. They are not simply wildlife in difficult country; they are part of what makes that country difficult in the first place."
            )
        };

    internal static string AnimalCultureDescriptionForTesting =>
        SeederDescriptionHelpers.JoinParagraphs(
            "The stock Animal culture represents the untutored social default used for wild or non-sapient creatures in the seeder, a baseline that assumes instinct, territory, routine and bodily communication rather than formal institutions.",
            "It does not try to model a human-style society projected onto beasts. Instead, it implies a world of scent marks, nesting places, migration routes, breeding cycles, dominance signals and the learned habits that keep an animal alive in its ecological niche.",
            "For builders, this culture is practical glue: it gives stock animals a coherent chargen and naming context without pretending they share a civilisation. For players and world design, it reinforces that these creatures belong first to habitat and behaviour, and only secondarily to human interpretation."
        );

    internal static string BuildRaceDescriptionForTesting(AnimalRaceTemplate template)
    {
        AnimalDescriptionVariant primaryAdult = GetPrimaryAdultVariant(template.DescriptionPack);
        AnimalDescriptionVariant secondaryAdult = GetSecondaryAdultVariant(template.DescriptionPack) ?? primaryAdult;
        string summary = string.IsNullOrWhiteSpace(template.Description)
            ? $"{template.Name}s are stock {template.Adjective.ToLowerInvariant()} animals seeded for the default FutureMUD catalogue."
            : $"{SeederDescriptionHelpers.EnsureTrailingPeriod(template.Description)} Adults are most often represented as {primaryAdult.ShortDescription}.";
        return SeederDescriptionHelpers.JoinParagraphs(
            summary,
            primaryAdult.FullDescription,
            BuildAnimalInteractionParagraph(template.Name, secondaryAdult.FullDescription)
        );
    }

    internal static string BuildEthnicityDescriptionForTesting(string raceName, string ethnicityName)
    {
        if (AnimalEthnicityDescriptions.TryGetValue($"{raceName}:{ethnicityName}", out string? description))
        {
            return description;
        }

        if (!RaceTemplates.TryGetValue(raceName, out AnimalRaceTemplate? template))
        {
            return SeederDescriptionHelpers.JoinParagraphs(
                $"The stock {ethnicityName.ToLowerInvariant()} ethnicity provides the default presentation for {raceName.ToLowerInvariant()} characters.",
                "This line preserves the ordinary body plan, instincts and outward cues associated with the parent race.",
                "Builders can treat it as the unexceptional baseline from which more specialised lineages or regional variants might later diverge."
            );
        }

        AnimalDescriptionVariant primaryAdult = GetPrimaryAdultVariant(template.DescriptionPack);
        return SeederDescriptionHelpers.JoinParagraphs(
            $"The {ethnicityName.ToLowerInvariant()} ethnicity represents the common or default stock line of the {raceName.ToLowerInvariant()} race.",
            primaryAdult.FullDescription,
            BuildAnimalInteractionParagraph(raceName, primaryAdult.FullDescription)
        );
    }

    private static AnimalDescriptionVariant GetPrimaryAdultVariant(AnimalDescriptionPack pack)
    {
        return pack.AdultMale.FirstOrDefault() ??
               pack.AdultFemale.FirstOrDefault() ??
               pack.JuvenileMale.FirstOrDefault() ??
               pack.JuvenileFemale.FirstOrDefault() ??
               pack.BabyMale.First();
    }

    private static AnimalDescriptionVariant? GetSecondaryAdultVariant(AnimalDescriptionPack pack)
    {
        return pack.AdultMale.Skip(1).FirstOrDefault() ??
               pack.AdultFemale.Skip(1).FirstOrDefault() ??
               pack.AdultFemale.FirstOrDefault() ??
               pack.AdultMale.FirstOrDefault();
    }

    private static string BuildAnimalInteractionParagraph(string raceName, string fallbackDetail)
    {
        if (DomesticAnimalRaceNames.Contains(raceName))
        {
            return $"Around people, {raceName.ToLowerInvariant()} stock is usually understood through husbandry, labour or companionship rather than wilderness alone. {SeederDescriptionHelpers.TrimLeadingSentencePrefix(fallbackDetail, "This ")}";
        }

        if (PredatorAnimalRaceNames.Contains(raceName))
        {
            return $"Where {raceName.ToLowerInvariant()} territory overlaps with settlement, the species is treated first as a risk and only second as a curiosity. {SeederDescriptionHelpers.TrimLeadingSentencePrefix(fallbackDetail, "This ")}";
        }

        return $"People usually meet {raceName.ToLowerInvariant()} stock as part of a broader landscape, whether as game, herd animal, pest, scavenger or simply a sign of local ecology. {SeederDescriptionHelpers.TrimLeadingSentencePrefix(fallbackDetail, "This ")}";
    }
}
