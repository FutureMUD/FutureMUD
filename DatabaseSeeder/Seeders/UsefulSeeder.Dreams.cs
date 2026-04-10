using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseSeeder.Seeders
{
    public partial class UsefulSeeder
    {
        internal void SeedDreams(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
        {
            void AddDream(string name, int priority, IEnumerable<(string Text, string Command, int SecondsDelay)> phases)
            {
                if (context.Dreams.Any(x => x.Name == name))
                {
                    return;
                }

                Dream dream = new()
                {
                    Name = name,
                    Priority = priority,
                    OnlyOnce = false
                };
                context.Dreams.Add(dream);
                var phaseId = 1;
                foreach ((string Text, string Command, int SecondsDelay) phase in phases)
                {
                    context.DreamPhases.Add(new DreamPhase
                    {
                        Dream = dream,
                        PhaseId = phaseId++,
                        DreamerText = phase.Text,
                        DreamerCommand = phase.Command,
                        WaitSeconds = phase.SecondsDelay
                    });
                }
            }

            bool modernDreams = questionAnswers["dream-eras"].Contains("modern");
            bool oldDreams = questionAnswers["dream-eras"].Contains("old");

            #region Universal Dreams
            AddDream("The Endless Fall", 140,
            [
               ("There is a moment of terrible certainty, and then the ground is gone beneath you.", "", 15),
           ("You are falling through *1{cold darkness|thin grey mist|empty air lit by a pale glow|a vast black space with no end in sight}. Your stomach twists and your limbs flail, but there is nothing to catch hold of.", "", 20),
           ("For an instant, you think you see *1{the ground rushing up|shapes below you|jagged shadows beneath the mist|something waiting under you}, and panic grips you harder than the fall itself.", "", 15),
           ("Just before impact, you jolt with the sickening certainty that this time you will hit.", "", 10),
        ]
            );

            AddDream("Footsteps Behind You", 130,
            [
               ("You are moving quickly, though you do not remember deciding to run.", "", 15),
           ("Behind you, there are footsteps. *1{Slow and steady|Hurried and uneven|Soft but close|Heavy and deliberate}. No matter how you change direction, they remain behind you.", "", 20),
           ("You pass through *2{narrow passages|open spaces|crowded places that never help|doorways that lead nowhere}, but nothing puts distance between you and the thing pursuing you.", "", 20),
           ("You try to look back, but dread holds your head forward. You know, with dreamlike certainty, that if you see what is chasing you, something terrible will happen.", "", 20),
           ("The footsteps suddenly speed up.", "", 8),
        ]
            );

            AddDream("Crumbling Teeth", 125,
            [
               ("Something feels wrong in your mouth; a faint looseness, a tiny shift where there should be none.", "", 15),
           ("You probe at a tooth with your tongue, and it moves. Then another. Then several at once.", "", 20),
           ("When you spit into your hand, *1{small white fragments|whole teeth slick with saliva|bloody chips of enamel|more teeth than should fit in a mouth} fall into your palm.", "", 20),
           ("You try to call for help, but more teeth come loose and clatter together between your lips.", "", 15),
           ("No matter how many you spit out, there are always more.", "", 12),
        ]
            );

            AddDream("Late Again", 115,
            [
               ("You are already late for something important, though you cannot clearly remember what it is.", "", 15),
           ("Every small task takes too long. You cannot find *1{your shoes|the right door|what you were supposed to carry|the thing you know you must not forget}.", "", 18),
           ("Whenever you think you are finally ready, some new problem appears: *2{a hallway longer than it should be|steps that lead the wrong way|a room full of strangers in your path|a missing item you swear you had a moment ago}.", "", 20),
           ("You feel the weight of other people's disappointment before you even arrive.", "", 15),
           ("At last you reach where you were going, only to know instantly that you are far too late.", "", 12),
        ]
            );

            AddDream("No Voice", 120,
            [
               ("Something is wrong. You need to speak immediately.", "", 12),
           ("You open your mouth to shout, but only *1{air|a dry rasp|a tiny broken whisper|nothing at all} comes out.", "", 18),
           ("You try again, straining until it hurts, but your voice refuses to rise above *1{a whisper|a croak|a breathy murmur|silence}.", "", 18),
           ("The people around you continue as though they cannot hear you, or worse, as though they cannot even see you trying.", "", 18),
           ("Urgency builds into panic, and still your throat will not give you a real sound.", "", 12),
        ]
            );

            AddDream("Seen and Exposed", 105,
            [
               ("At first it is only a vague discomfort, the sense that something about you is wrong.", "", 15),
           ("Then you realise with a flood of shame that you are *1{underdressed|half-dressed|missing something essential|far more exposed than everyone else}.", "", 20),
           ("People turn to look. Some stare openly. Some pretend not to notice, which is somehow worse.", "", 18),
           ("You try to cover yourself with *2{your hands|whatever cloth you can find|an object that is far too small|nothing useful at all}, but every movement only makes you feel more conspicuous.", "", 18),
           ("You need to leave, but there is nowhere private to go.", "", 12),
        ]
            );

            AddDream("Endless Corridors", 110,
            [
               ("You are trying to get somewhere you feel certain you have been before.", "", 15),
           ("The passage ahead branches into *1{more hallways|identical rooms|stairways that twist back on themselves|doorways that all look the same}.", "", 20),
           ("Every turn seems familiar until you take it, and then it becomes clear you are still lost.", "", 18),
           ("You keep expecting to find *2{an exit|someone who can guide you|a room that matters|the place you were actually seeking}, but each discovery leads only to more confusion.", "", 20),
           ("The longer you wander, the more certain you become that the place is shifting around you.", "", 15),
        ]
            );

            AddDream("Legs Like Stone", 118,
            [
               ("Danger is near. You know it before you know from where.", "", 12),
           ("You try to run, but your legs feel *1{heavy as stone|thick as mud|slow and disconnected|as though they belong to someone else}.", "", 18),
           ("Each step takes enormous effort. The ground seems to cling to your feet, dragging at every movement.", "", 18),
           ("Behind you, *2{something approaches|the threat gets closer|you hear movement you cannot match|time is running out faster than your body can move}.", "", 18),
           ("You force yourself harder and harder, but never manage more than a weak, desperate stumble.", "", 12),
        ]
            );

            AddDream("Water Rising", 112,
            [
               ("You become aware of water around your feet, cold and unwelcome.", "", 15),
           ("It rises steadily to *1{your ankles|your knees|your waist|your chest}, and there is no clear way out.", "", 20),
           ("You search for something solid to climb, something to hold, but everything is *2{too far away|slippery|unstable|already sinking}.", "", 20),
           ("When you try to breathe calmly, the air feels thin. When you panic, the water seems to rise faster.", "", 18),
           ("At the last moment before you go under, you wake with the feeling of your heart pounding in your throat.", "", 10),
        ]
            );

            AddDream("The Wrong Home", 95,
            [
               ("You have returned to a place that should feel safe and familiar.", "", 15),
           ("At first everything seems normal, but then you notice *1{a door where no door should be|rooms in the wrong places|furniture you do not recognise|windows looking onto impossible views}.", "", 20),
           ("The longer you stay, the clearer it becomes that this place only resembles home. It is wearing the shape of it without the comfort.", "", 20),
           ("You move from room to room, hoping for recognition, but each step makes the place feel more wrong.", "", 18),
           ("Somewhere inside, you realise that if this is not home, then you do not know where home is.", "", 12),
        ]
            );

            AddDream("Looking for the Right Room", 95,
    [
       ("You are inside a building with a simple purpose, trying to find the room you need.", "", 15),
   ("The corridors are ordinary enough: *1{plain walls and closed doors|soft light and quiet corners|worn floors and familiar turns|open spaces broken by doorways and stairs}.", "", 20),
   ("You pass *2{one room that seems almost right|a person who gives vague directions|a sign that does not help as much as it should|a doorway that feels familiar and then doesn't}.", "", 18),
   ("At last you open a door, certain this must be the place, only to find *2{another corridor|the wrong room entirely|people expecting someone else|that you need to keep looking}.", "", 15),
]
    );

            AddDream("The Everyday Shop", 90,
            [
               ("You are in a shop for something simple, though you cannot quite remember what it was when you first entered.", "", 15),
   ("Shelves and tables are laid out around you, full of *1{familiar goods|ordinary objects|things you almost recognise|items that seem more important than they should}.", "", 20),
   ("You pick things up, set them down, compare one to another, and somehow never feel any closer to being finished.", "", 18),
   ("When you finally think you've found the right thing, you notice *2{there is another version beside it|you are in the wrong part of the shop|you no longer need it|you have forgotten what you came for in the first place}.", "", 15),
]
            );

            AddDream("Waiting to Leave", 92,
            [
               ("You are preparing to go somewhere routine, and there is no urgency to it.", "", 12),
   ("Other people drift nearby, making their own arrangements, while you wait for *1{the right moment|the signal to leave|someone to be ready|the sense that it is finally time}.", "", 18),
   ("A bag is *2{packed and unpacked|checked once, then again|set down and picked up|present but never quite settled in your hand}.", "", 18),
   ("It feels as though departure is always only a few moments away, but those moments stretch on gently without ending.", "", 15),
]
            );

            AddDream("Back in the Classroom", 88,
            [
               ("You are seated in a classroom, though you do not remember arriving there.", "", 15),
   ("Around you, people are *1{turning pages|quietly speaking|waiting for something to begin|settling into their seats}.", "", 18),
   ("A lesson is underway, and while you are not distressed, you have the odd feeling that you should already understand what is being discussed.", "", 18),
   ("You look down to find *2{notes half-finished in your own hand|a page you don't remember reading|an empty space where you meant to write|words that seem familiar but refuse to stay in mind}.", "", 18),
]
            );

            AddDream("A House to Tidy", 85,
            [
               ("You are moving through a house with the quiet understanding that there are small things to put in order.", "", 15),
   ("In one room, *1{chairs are slightly out of place|drawers stand a little open|clothes rest over a railing|cups and books sit where they were left}.", "", 20),
   ("You straighten one thing, then another, and the work is neither difficult nor finished.", "", 18),
   ("Whenever one space begins to look right, *2{another room calls for attention|something small catches your eye|you realise you have forgotten where an object belongs|the house seems larger than before}.", "", 18),
]
            );

            AddDream("A Familiar Conversation", 93,
            [
               ("You are speaking with someone you know well, or feel that you ought to know well.", "", 15),
   ("The conversation moves easily from one small topic to another: *1{weather and plans|food and ordinary errands|people you both seem to know|places you have both been}.", "", 20),
   ("Now and then you lose the thread, but the other person continues as though nothing has been lost.", "", 16),
   ("You nod, reply, and listen, with the strange sense that the talk matters and does not matter at the same time.", "", 16),
]
            );

            AddDream("Finding a Seat", 84,
            [
               ("You enter a place where people are already seated, and you begin to look for a place of your own.", "", 14),
   ("There are seats everywhere, but each one is *1{taken at the last moment|not quite suitable|meant for someone else|awkwardly placed}.", "", 18),
   ("You move quietly between *2{rows of chairs|small tables|benches near the wall|places that seem almost available}, trying not to interrupt anyone.", "", 18),
   ("Eventually you do sit down, though even then it feels as if you may have to move again soon.", "", 14),
]
            );

            AddDream("The Kitchen Light", 82,
            [
               ("You are standing in a kitchen lit by *1{early daylight|late afternoon light|the soft glow of a lamp|the pale brightness of a room not fully awake}.", "", 18),
   ("There is something simple to do: *2{make a drink|slice some bread|put away clean dishes|look for something in a cupboard}.", "", 20),
   ("You move from counter to table to sink in an easy rhythm, touching ordinary objects that feel more distinct than they do in waking life.", "", 18),
   ("Nothing unusual happens, and yet the room seems full of calm significance, as though this small task is enough for now.", "", 16),
]
            );

            AddDream("The Missing Small Thing", 94,
            [
               ("You need one small, ordinary thing before you can continue.", "", 12),
   ("It is *1{a key|a note|a book|a cup|a piece of clothing|something you had only a moment ago}, and it ought to be easy to find.", "", 18),
   ("You check *2{a table, then a drawer|one pocket after another|the place where it should obviously be|several sensible places in a sensible order}, but it never appears.", "", 20),
   ("The search remains calm, almost methodical, even as the object becomes more strangely absent with every passing moment.", "", 16),
]
            );

            AddDream("Walking an Ordinary Street", 87,
            [
               ("You are walking along a street that feels familiar without belonging to any place you can name.", "", 15),
   ("You pass *1{doors and windows lit from within|parked vehicles sitting quietly|gardens or yards in careful disorder|people moving about their own business}.", "", 20),
   ("Now and then something catches your eye: *2{a house you almost recognise|a corner you feel you have turned before|a person who seems about to greet you|a shopfront or gate that seems important for no clear reason}.", "", 18),
   ("You continue on, not hurrying and not stopping, with the sense that simply walking is the whole purpose of the dream.", "", 16),
]
            );

            AddDream("Waiting in Line", 78,
    [
       ("You are standing in a quiet line, waiting your turn for something ordinary.", "", 12),
   ("The line moves a little, then stops again.", "", 10),
]
    );

            AddDream("Misplaced Glasses", 76,
            [
               ("You need your glasses for some small task, but they are not where you expected.", "", 12),
   ("You search a table, a pocket, and a nearby shelf without concern.", "", 10),
]
            );

            AddDream("Making Tea", 74,
            [
               ("You are making a hot drink in an ordinary kitchen.", "", 12),
   ("You wait for the water, listening to the soft sounds of the room.", "", 10),
]
            );

            AddDream("Folding Laundry", 70,
            [
               ("You are folding clean clothes into neat little stacks.", "", 12),
   ("Each item seems to take a little longer than it should, but there is no rush.", "", 10),
]
            );

            AddDream("Looking for a Pen", 73,
            [
               ("You need a pen to write something down, but cannot immediately find one.", "", 12),
   ("You open a drawer and sort through a few ordinary objects.", "", 10),
]
            );

            AddDream("Watering Plants", 68,
            [
               ("You are watering a row of indoor plants or flowers.", "", 12),
   ("You move from one to the next, careful not to spill.", "", 10),
]
            ); AddDream("Searching a Pocket", 72,
    [
       ("You pat at a pocket or fold in your clothing, looking for some small thing you meant to keep close.", "", 12),
   ("It is not there, though you are sure it was a moment ago.", "", 10),
    ]
    );

            AddDream("Sweeping the Floor", 70,
            [
               ("You are sweeping dust and little scraps into a neat pile.", "", 12),
   ("No matter how carefully you gather it, a little always remains behind.", "", 10),
]
            );

            AddDream("At the Doorway", 74,
            [
               ("You stand in a doorway, pausing before going in or out.", "", 12),
   ("For a while, it feels enough simply to remain there.", "", 10),
]
            );

            AddDream("Carrying Water", 71,
            [
               ("You are carrying water carefully, trying not to spill any.", "", 12),
   ("The surface trembles with each step, but never quite tips over.", "", 10),
]
            );

            AddDream("Buttoning Up", 69,
            [
               ("You are fastening your clothing with calm, deliberate fingers.", "", 12),
   ("One fastening slips loose, and you must begin that part again.", "", 10),
]
            );

            AddDream("A Seat by the Wall", 68,
            [
               ("You find a place to sit beside a wall and settle yourself there.", "", 12),
   ("People pass now and then, but nothing asks anything of you.", "", 10),
]
            );

            AddDream("Sorting Small Objects", 73,
            [
               ("You are arranging a number of small objects into tidy groups.", "", 12),
   ("When you finish, you immediately feel that they should be arranged another way instead.", "", 10),
]
            );

            AddDream("Washing Your Hands", 67,
            [
               ("You are washing your hands slowly and carefully.", "", 12),
   ("The water runs over your skin until you forget why you began.", "", 10),
]
            );

            AddDream("Closing the Shutters", 66,
            [
               ("You move through a room, closing coverings against the outside light.", "", 12),
   ("Each one changes the room a little, making it quieter and dimmer.", "", 10),
]
            );

            AddDream("A Bowl of Fruit", 65,
            [
               ("You are choosing from a bowl or basket of fruit, turning one piece in your hand.", "", 12),
   ("None seem wrong, but none seem quite the one you want either.", "", 10),
]
            );

            AddDream("The Warm Hearth", 98,
    [
       ("You step into a sheltered place where the air is warm and still, and at once some quiet part of you begins to unclench.", "", 16),
   ("A gentle glow fills the room from *1{a fire|lamplight|sunlight through a high opening|some other warm and steady source of light}, touching everything with softness.", "", 18),
   ("There is a place waiting for you to sit, and the simple comfort of it feels as though it has been kept ready just for your arrival.", "", 18),
   ("For a little while, you do nothing at all except rest in warmth, and that is more than enough.", "", 14),
]
    );

            AddDream("At the Table", 94,
            [
               ("You come upon a table already laid out, not grandly, but generously, with the quiet promise of enough for everyone.", "", 16),
   ("There is food and drink in pleasing abundance: *1{fresh bread and fruit|warm soup and simple fare|sweet things in little dishes|familiar comforts you cannot quite name}.", "", 18),
   ("Around you are people whose presence feels easy. No one is demanding anything. No one is in a hurry.", "", 18),
   ("You sit, eat, and feel the deep contentment of being welcomed without question.", "", 14),
]
            );

            AddDream("The Open Door", 92,
            [
               ("You have been looking for the right place for what feels like a long time, though without fear or urgency.", "", 16),
   ("Then you see a door standing open, with *1{warm light beyond it|familiar voices within|the scent of good things waiting|the unmistakeable feeling of belonging}.", "", 18),
   ("As you step through, you know at once that this is where you were meant to arrive.", "", 16),
   ("Relief moves through you so gently that it almost feels like joy.", "", 14),
]
            );

            AddDream("Running Freely", 96,
            [
               ("You begin to run, not from danger, but from sheer gladness at being able to move.", "", 14),
   ("Your body feels light and capable. Each step carries you easily over *1{soft ground|open fields|rolling hills|a path that seems to welcome your feet}.", "", 18),
   ("The air is cool in your lungs and the rhythm of your movement is effortless, almost playful.", "", 18),
   ("You keep running simply because it feels wonderful to do so.", "", 14),
]
            );

            AddDream("The Lost Thing Found", 90,
            [
               ("You have been searching for something small but precious, though you never felt truly afraid it was gone forever.", "", 16),
   ("At last you notice it in *1{a pocket you had forgotten|a quiet corner|the bottom of a box or basket|plain sight, where it somehow went unseen before}.", "", 18),
   ("The instant you pick it up, a simple happiness passes through you, brighter than the thing itself ought to warrant.", "", 18),
   ("For a moment, the whole world feels tidier and more forgiving.", "", 14),
]
            );

            AddDream("Someone Glad to See You", 101,
            [
               ("You see someone approaching from a distance, and before you know who they are, you already feel your heart lift.", "", 16),
   ("When they recognise you, their face opens with unmistakable warmth: *1{a smile of relief|a delighted laugh|the joy of old affection|the quiet certainty of welcome}.", "", 18),
   ("They come to you without hesitation, and all awkwardness or separation simply falls away.", "", 18),
   ("To be wanted so plainly and so kindly fills you with a deep and lasting gladness.", "", 14),
]
            );

            AddDream("Morning Light", 88,
            [
               ("You wake within the dream to the sense of morning already settled gently around you.", "", 14),
   ("Light falls across the room in *1{broad golden bands|soft pale brightness|clear clean shafts through an opening|a warm glow that makes everything look kind}.", "", 18),
   ("The day ahead feels open and manageable, with none of the heaviness that so often accompanies waking.", "", 18),
   ("You lie still for a little while, content to feel the quiet goodness of the moment.", "", 14),
]
            );

            AddDream("A Familiar Song", 86,
            [
               ("At first you hear only a few notes, carried from somewhere nearby.", "", 14),
   ("Then the tune becomes clear, and you realise it is *1{a song you have always known|a melody from long ago|something simple and dear|music that feels older than memory}.", "", 18),
   ("Soon you are humming along, and others nearby seem to know it too, their voices or movements joining without effort.", "", 18),
   ("The shared rhythm fills you with a soft happiness that asks for nothing more.", "", 14),
]
            );

            AddDream("Clear Water", 91,
            [
               ("You come upon water so clear that you can see every pale stone and shifting ripple beneath its surface.", "", 16),
   ("You kneel to touch it, and it is *1{cool and refreshing|clean and lively|silken over your hands|so pure that it seems to brighten everything around it}.", "", 18),
   ("Whether you drink, wash, or simply watch it moving, the sight of it brings a feeling of deep ease.", "", 18),
   ("You remain there for some time, restored by nothing more complicated than clarity and freshness.", "", 14),
]
            );

            AddDream("The Burden Lifted", 97,
            [
               ("You become aware that you have been carrying something heavy for a very long time.", "", 16),
   ("At last you set it down, or *1{someone gently takes it from you|it slips away of its own accord|you realise it need not be carried any longer|you find the proper place to leave it}.", "", 18),
   ("The change is immediate. Your shoulders loosen, your breathing deepens, and your whole body seems to remember how to rest.", "", 18),
   ("Freed of that weight, you feel lighter than you have in a very long time.", "", 14),
]
            );

            AddDream("Not a Moment's Privacy", 118,
    [
       ("You become aware of a growing, undeniable need to relieve yourself.", "", 14),
   ("At first it seems simple enough; all you need is a little privacy. Yet every place you try is *1{already occupied|too open to the world|missing a door or screen|suddenly full of people who should not be there}.", "", 20),
   ("You move on quickly, trying to remain calm, but each new place is somehow worse than the last.", "", 18),
   ("The urgency grows sharper and sharper, while privacy remains always just out of reach.", "", 14),
]
    );

            AddDream("The Room That Changes", 114,
            [
               ("You find what seems at last to be the right place, and hurry toward it with relief already beginning to rise.", "", 15),
   ("But when you step inside, it is not what it should be at all. It becomes *1{a sleeping room|a pantry or store room|a place where people are talking quietly|some other entirely unsuitable space}.", "", 20),
   ("You back out and try another, only for that one too to become *2{the wrong room|a place with no privacy|a room already in use|something stranger still}.", "", 20),
   ("Again and again you come close to relief, only for the world to shift at the last possible moment.", "", 15),
]
            );

            AddDream("No Latch on the Door", 112,
            [
               ("At last you find a small private room and slip inside, grateful beyond words.", "", 14),
   ("Yet the door will not fasten. The latch is *1{broken|missing|too loose to hold|there, but somehow useless}, and the door keeps easing open again.", "", 20),
   ("Each time you try to settle yourself, you must stop to pull it closed again, certain someone will enter if you do not.", "", 18),
   ("You spend the whole dream on the very brink of relief, unable to trust the door for even a moment.", "", 14),
]
            );

            AddDream("Too Many People About", 116,
            [
               ("You need to relieve yourself badly, but for some reason the world has grown unbearably crowded.", "", 14),
   ("Everywhere you look there are people: *1{passing by in little groups|standing and talking|waiting nearby without purpose|appearing suddenly from nowhere}.", "", 20),
   ("Even places that ought to be private are full of interruption, with strangers behaving as though nothing is unusual.", "", 18),
   ("You keep searching for a moment alone, but the dream will not grant you one.", "", 14),
]
            );

            AddDream("Almost in Time", 120,
            [
               ("The need is urgent now, and you know you must find somewhere to go immediately.", "", 14),
   ("You spot a likely place in the distance and hurry toward it, but the path becomes *1{longer than it should be|full of little obstacles|twisted and indirect|somehow impossible to cross quickly}.", "", 20),
   ("Each time you think you have finally arrived, there is one more thing in the way: *2{another door|a narrow passage|someone blocking your path|the sudden realisation that this is not the place after all}.", "", 20),
   ("You remain forever only a few moments away from relief, and never quite reach it.", "", 14),
]
            );

            AddDream("The House of Breathing Walls", 82,
    [
       ("You are walking through a house that seems built of ordinary materials until you notice the walls are breathing. They swell and settle with a slow, sleeping rhythm, as though the whole place is alive and only pretending to be still.", "", 18),
   ("Doors appear where no doors were before, opening into *1{rooms full of hanging cloth and candle smoke|stairways descending into water without wetness|small chambers lined with mirrors that reflect strangers instead of you|vast empty halls too large to fit inside the house}.", "", 24),
   ("Now and then the walls exhale and whisper *2{your name in a voice you almost recognise|fragments of forgotten promises|words that mean nothing and yet feel important|the sound of distant laughter from another age}.", "", 20),
   ("You understand that the house wants to show you something, but each time you think you are near the heart of it, the corridors gently rearrange themselves and lead you elsewhere.", "", 16),
]
    );

            AddDream("The Orchard of Glass Fruit", 78,
            [
               ("You come upon an orchard where every tree is heavy with fruit made of glass. They hang in clusters, shining with colours too vivid to belong to the waking world.", "", 18),
   ("When the wind moves through the branches, the whole orchard rings softly. The fruit knocks together with the sound of *1{tiny bells|distant chimes underwater|teeth in a silver bowl|ice breaking far away}.", "", 22),
   ("You reach up and pick one. Inside the clear flesh is *2{a small beating heart|a folded scrap of writing|your own eye staring back at you|a miniature landscape beneath a yellow sky}. It is beautiful, and you are terribly afraid to drop it.", "", 24),
   ("Before you can decide whether to keep it or return it, the fruit in your hand grows warm, then hollow, then vanishes entirely, leaving only the ringing of the trees.", "", 16),
]
            );

            AddDream("The River That Flows Upward", 80,
            [
               ("You stand beside a river whose waters do not descend, but rise. They pour uphill in glittering silence, climbing over stones and banks as though gravity has politely stepped aside.", "", 18),
   ("Within the current drift *1{chairs with no one in them|fish shaped like keys|books whose pages turn by themselves|white masks looking down into the water from beneath the surface}.", "", 22),
   ("You kneel to touch the river and find that it feels like *2{cool smoke|silk drawn across the skin|the air before a storm|water only in memory, not in truth}. Your fingers come away dry, yet you are certain you have been altered by the contact.", "", 24),
   ("Far upstream, or perhaps far above, something vast passes through the current, and the whole river trembles with the effort of carrying it.", "", 16),
]
            );

            AddDream("Teeth in the Moon", 76,
            [
               ("The moon hangs unusually low and bright, so near that you can see details no one should be able to see. Its pale face is smooth at first, then begins slowly to part.", "", 18),
   ("Inside is a mouth full of *1{human teeth|rows of pearl-like stones|little white doors opening and closing|narrow windows lit from within}. The moon smiles, and the night around you grows quieter, as though listening.", "", 22),
   ("Something falls from that smile: *2{milk-white feathers|small coins stamped with your profile|teeth that ring when they strike the ground|notes of music you can almost gather in your hands}.", "", 22),
   ("You try to decide whether the sight fills you with wonder or terror, and realise too late that the moon is deciding the same about you.", "", 16),
]
            );

            AddDream("A Choir of Empty Clothes", 74,
            [
               ("You enter a wide open place where garments hang in the air without bodies to support them. Robes, shirts, cloaks, and veils drift at different heights as though arranged by an invisible hand.", "", 18),
   ("They sway together and begin to sing. The voices come not from mouths but from the folds themselves: *1{thin and sweet like winter wind|deep and solemn as bells at dusk|childlike and wavering|many-voiced and perfectly matched}.", "", 22),
   ("As they turn in the air, you see that some are lined with *2{stars where fabric ought to be|old letters sewn into the hems|moving water beneath the seams|nothing at all, a perfect and impossible emptiness}.", "", 22),
   ("One garment descends and hovers before you, opening gently as though inviting you to step inside and become its missing song.", "", 16),
]
            );

            AddDream("The Staircase Beneath the Sea", 84,
            [
               ("You are walking down a staircase at the bottom of the sea. Water towers all around you, yet the steps themselves are dry, lit by a dim and patient light with no visible source.", "", 18),
   ("On either side of the stair move creatures that never quite resolve into any one form: *1{fish with human hands|birds swimming like eels|faces growing from coral and watching you pass|lanterns that drift as though alive}.", "", 22),
   ("The deeper you descend, the more the sea resembles a cathedral. Great pillars rise into the blue gloom, and overhead something tolls *2{once like a bell|in a slow series of heartbeats|with the sound of pebbles dropped into a well|without sound at all, yet you feel it in your bones}.", "", 24),
   ("At the foot of the stairs there waits a closed door of green metal. You know with total certainty that it opens inward, though you cannot imagine what 'inward' means beneath the sea.", "", 16),
]
            );

            AddDream("The Garden of Hands", 79,
            [
               ("You come to a garden where flowers do not grow. Instead, hands rise from the soil on slender stems, opening and closing gently in the breeze.", "", 18),
   ("Some are *1{old and knotted|small and delicate|ink-stained|covered in rings that gleam like dew}. They turn toward you as sunflowers turn toward the sun.", "", 20),
   ("When you walk among them, the hands brush your clothing and offer you *2{keys made of wax|seeds that pulse like hearts|tiny folded maps of places you have never been|nothing visible, only the unmistakable feeling of a gift}.", "", 24),
   ("At the garden's centre stands one great flower whose bloom is a pair of your own hands clasped together, patiently waiting for you to understand why they were planted here.", "", 16),
]
            );

            AddDream("The Market of Names", 83,
            [
               ("You arrive at a crowded market where nothing visible is being sold. People lean over stalls and examine empty trays with intense concentration, bartering in low urgent voices.", "", 18),
   ("Only after a while do you realise the merchants are selling names. Each one lifts invisible offerings and praises them: *1{old names with deep roots|bright names that taste of summer rain|narrow clever names for thieves and birds|heavy royal names that leave a metallic feeling on the tongue}.", "", 24),
   ("You are invited to try one. The moment it is placed upon you, the whole market changes. *2{The colours sharpen|everyone bows as though recognising you|your hands become those of another person|you remember a lifetime that was not yours a moment before}.", "", 24),
   ("Then the merchant asks what name you have brought in exchange, and you discover with a quiet horror that you cannot remember the one you arrived with.", "", 16),
]
            );

            AddDream("The Library of Unwritten Letters", 81,
    [
       ("You enter a library where every shelf is filled not with books, but with sealed letters. They are sorted by *1{colour|weight|the sound they make when touched|a system you almost understand}.", "", 18),
   ("When you lift one, you find the envelope addressed in your own hand to *2{someone you have never met|a version of yourself long forgotten|a city that does not exist|a name that makes your throat tighten for no reason}. The seal is warm, as though it has only just now been closed.", "", 24),
   ("Somewhere among the shelves, unseen voices whisper fragments of what the letters contain. You catch *1{apologies never spoken|directions to impossible places|love declarations written too late|warnings that arrive before the danger}.", "", 22),
   ("You know with strange certainty that opening even one of them would alter the order of your entire life, and that you have already opened one before.", "", 16),
]
    );

            AddDream("The Field of Sleeping Keys", 76,
            [
               ("You come upon a broad field where keys lie half-buried in the soil like seeds. Their heads and teeth catch the light in dull glimmers across the earth.", "", 18),
   ("Now and then one twitches, turns over, or gives a little metallic sigh in its sleep. Some are *1{rusted and ancient|bright as if newly forged|made of black metal that drinks the light|too delicate to open anything in the waking world}.", "", 22),
   ("You kneel and dig one free. The moment it touches your palm, you feel *2{a door somewhere swing gently shut|a room remembering your absence|a hidden lock turning inside your own chest|the nearness of something waiting to be opened}.", "", 24),
   ("When you look up, the whole field has quietly turned so that all the keys are pointing at you.", "", 16),
]
            );

            AddDream("The Feast of Shadows", 79,
            [
               ("A long table is laid in a dim hall, and every place is occupied, though no bodies are present. Only shadows sit in the chairs, cast by no visible guests.", "", 18),
   ("The platters hold *1{fruit that glows from within|bread black as charcoal and steaming gently|silver fish made of light|dishes that appear empty until you blink}. The shadows eat with perfect table manners, lifting invisible cups and inclining their heads to one another.", "", 26),
   ("One shadow gestures for you to sit. The place set before you contains *2{your favourite meal from childhood, altered slightly|something you have always wanted and never named|a single white seed on a golden plate|nothing at all, but the scent of a meal that makes you ache}.", "", 24),
   ("As you reach for it, you notice your own shadow is already seated among the others and does not look pleased to see you standing.", "", 16),
]
            );

            AddDream("The Bell Inside the Mountain", 85,
            [
               ("You are deep inside a mountain, walking a tunnel that seems less carved than remembered into being. The stone around you pulses with a dim mineral light.", "", 18),
   ("Far ahead, a bell rings. Its sound is *1{golden and sorrowful|low as thunder under snow|so pure it seems to erase distance|heavy enough to shake dust from the ceiling}. With each toll, the tunnel changes shape slightly.", "", 24),
   ("The walls contain veins of *2{milk-white crystal|dark metal that reflects not you but strangers|running water trapped in stone|something like handwriting buried beneath the rock}. They guide you toward the bell even when you turn away.", "", 24),
   ("At last you enter a cavern vast enough to contain weather, and see the bell swinging by itself over an abyss with no visible bottom.", "", 16),
]
            );

            AddDream("The Mirror That Remembers First", 78,
            [
               ("You find a mirror standing alone in an empty place. Its glass is clear, but it reflects events a little before they happen.", "", 18),
   ("You see your reflection *1{raise a hand before you think to do it|smile with knowledge you do not possess|turn away from something behind you that has not yet arrived|begin to weep without apparent cause}.", "", 22),
   ("The longer you watch, the further ahead it moves. Soon it is showing *2{tomorrow's wounds|rooms you have not entered yet|conversations you have not had|a version of yourself making a choice you desperately want explained}.", "", 24),
   ("When you finally try to step away, the reflection remains where it is and watches you leave with immense patience.", "", 16),
]
            );

            AddDream("The City Folded in Cloth", 74,
            [
               ("You stand above a city made entirely of folded cloth. Streets are seams, towers are pleated fabric, and bridges sag like ribbons between soft-built houses.", "", 18),
   ("Wind moves through it and changes everything. A gust turns *1{a palace into a market|a stair into a river|a row of houses into a flock of birds|an alley into the inside of a sleeve}. Nothing tears, yet nothing stays the same.", "", 24),
   ("Tiny figures walk the textile streets carrying *2{needles longer than spears|lanterns stitched from skin-thin paper|bundles of thread that hum softly|maps embroidered with moving constellations}. They seem terribly busy and terribly small.", "", 24),
   ("Somewhere in the centre of the cloth city is a knot all the inhabitants fear to untie, and as you watch, you realise the knot is your sleeping body.", "", 16),
]
            );

            AddDream("The Sea of Faces", 80,
            [
               ("You look out over a calm sea and realise the waves are made not of water, but of faces rising and sinking in endless succession.", "", 18),
   ("Some are *1{laughing softly|sleeping with closed eyes|mouths open in silent songs|watching you with grave and endless patience}. They never drown and never quite surface.", "", 22),
   ("The tide brings to shore *2{photographs too wet to hold|shells shaped like ears|letters written on skin-thin vellum|names spoken by no mouth you can identify}. Each offering feels personal, though none can be placed.", "", 24),
   ("Out beyond the breakers, a single vast face the size of a moon opens its eyes, and all the smaller waves turn toward it at once.", "", 16),
]
            );

            AddDream("The Tailor of Clouds", 77,
            [
               ("High above the earth, on a platform of nothing solid, a tailor is cutting garments from clouds. The shears make no sound, only little flashes of lightning at each snip.", "", 18),
   ("Bolts of vapour lie folded in neat stacks: *1{storm-grey wool|thin white silk that trails rain|gold-edged mist for royalty|dark thundercloth too heavy for mortal shoulders}. The tailor measures the sky with a ribbon marked in unknown units.", "", 26),
   ("When the tailor notices you, they hold up *2{a coat lined with stars|a veil made of dawn|gloves dripping tiny storms|a plain shirt of white cloud that somehow frightens you most} and consider whether it would fit.", "", 24),
   ("You understand that once dressed in such clothing, you would no longer cast a human shadow upon the ground.", "", 16),
]
            );

            AddDream("The Room Full of Clocks That Bloom", 73,
            [
               ("You enter a conservatory where clocks grow from the earth like flowers. Their stems are brass, their petals enamel, and their faces open toward the light.", "", 18),
   ("Some bloom fully and begin to chime. Others remain closed buds, ticking faintly from within. Still others shed *1{minute hands like petals|little gears like seeds|numbers soft as pollen|droplets of time that vanish before they touch the ground}.", "", 26),
   ("A gardener moves among them watering the roots with *2{milk from a silver can|black sand poured in careful circles|music ladled from a bowl|memories carried in glass jars}. Wherever the liquid falls, the clocks age visibly.", "", 24),
   ("When you lean close to smell one blossom, it tells you the precise hour at which you first became yourself.", "", 16),
]
            );

            AddDream("The Bridge Woven from Hair", 83,
            [
               ("You come to a deep gorge crossed only by a bridge woven from countless strands of hair. It is thick as rope in places, fine as spider silk in others, yet somehow bears its own weight.", "", 18),
   ("As you step onto it, the strands whisper. They murmur *1{old gossip in voices of the dead|songs mothers sing to sleepless children|the private thoughts of strangers passing below|your own forgotten dreams from years ago}.", "", 24),
   ("Halfway across you look down and see the gorge filled not with rock, but with *2{slow-turning constellations|pages drifting upward like birds|teeth set into the cliffside like windows|a darkness braided just like the bridge itself}.", "", 24),
   ("The far side is always only a little further on, yet each step leaves you less certain of which side you started from.", "", 16),
]
            );

            AddDream("The Smile That Stayed", 25,
    [
       ("You are speaking with someone who seems perfectly ordinary at first: *1{a friend|a family member|a neighbour|someone you feel you ought to know well}. Their voice is right, their clothes are right, even the way they stand is right.", "", 18),
   ("Then they smile, and never stop smiling. The conversation continues as though nothing is wrong, but the smile remains fixed through every word, showing *2{too many teeth|teeth that are a little too even|a mouth that seems slightly too wide|a grin that does not belong to the tone of the voice}.", "", 24),
   ("You wait for the expression to change, to soften, to become human again, but it does not. Even when they lower their voice, even when they say something kind, the smile holds with terrible patience.", "", 22),
   ("At last you understand that whatever is speaking to you learned the shape of friendliness, but not the reason for it.", "emote whimper|whimpers in ~!poss sleep.", 12),
]
    );

            AddDream("The Familiar Stranger", 25,
            [
               ("Someone approaches you with the easy certainty of an old acquaintance. Before they even speak, you feel that you know them, though you cannot say from where.", "", 16),
   ("They begin to talk about shared memories: *1{a room you once slept in|a meal you once ate|a day of heavy rain|a place from childhood}. Every detail is almost right, but each one is skewed by some tiny impossible error.", "", 22),
   ("You study their face and find it composed of convincing parts: the right eyes, the right mouth, the right voice. Yet none of those parts seem to belong together, as if the person has been assembled from your memories by something that never met a human being.", "", 24),
   ("When you finally say, 'I don't know you,' they answer, very softly, 'You did the first time.'", "", 14),
]
            );

            AddDream("Behind the Eyes", 25,
            [
               ("You are with someone who is trying very hard to appear calm. They blink a little too seldom, and when they do, it seems delayed, as though they have remembered a moment too late that people are meant to blink.", "", 20),
   ("As they look at you, you become aware of movement behind their eyes: *1{something pale shifting in the sockets|many tiny motions where only two should be|the suggestion of a second pair of pupils|a shape pressed close against the inside, trying not to be seen}.", "", 26),
   ("They keep speaking in a normal voice, but now every word feels like a distraction, something meant to keep your attention on the mouth while whatever lives behind the eyes rearranges itself.", "", 22),
   ("Then they smile with relief, as though they can tell you have finally noticed the correct part of them.", "emote shudder|shudders violently in ~!poss sleep.", 12),
]
            );

            AddDream("The Sleeper Beside You", 25,
            [
               ("You wake within the dream and become aware that someone is lying beside you. Their back is turned, and their breathing is slow and regular.", "", 16),
   ("At first there is nothing alarming in this, only a vague uncertainty that they should not be there. Then you notice the breathing is wrong: *1{too deep and hollow|too perfectly timed|coming in little copied bursts|accompanied by a faint second breath underneath the first}.", "", 24),
   ("You wait for them to stir, but they remain motionless for too long. At last, very slowly, the head turns toward you while the rest of the body stays still beneath the covers.", "", 22),
   ("The face is someone you know, but only from the nose downward. Above that, the features are smooth and unfinished, as though whatever made them ran out of certainty halfway through.", "emote gasp|gasps sharply in ~!poss sleep.", 10),
]
            );

            AddDream("The Guest Who Would Not Eat", 25,
            [
               ("There is a guest at the table, and no one but you seems to find them strange. They are polite, attentive, and answer every question with just the right amount of warmth.", "", 18),
               ("Food is served. Others eat. The guest lifts *1{a spoon|a cup|a piece of bread|a knife and fork} with perfect imitation of ordinary manners, yet nothing ever reaches their mouth. Somehow the motions happen, and the plate remains unchanged.", "", 24),
               ("Now and then they nod and say the meal is excellent. When laughter rises around the table, they join in a half-second too late, as though listening for the proper cue.", "", 20),
               ("At last the guest turns to you and asks, in a voice full of careful interest, 'How do you know when you have eaten enough?'", "", 14),
            ]
            );

            AddDream("The Child at the Door", 25,
            [
               ("A child stands in a doorway watching you. There is nothing immediately wrong with them; they are simply standing there, quiet and patient, hands at their sides.", "", 18),
   ("Then you notice that the child never shifts weight, never fidgets, never makes the small movements living children cannot help making. They only watch with *1{an adult stillness|a perfect blankness|a smile too old for the face|eyes that never once leave yours}.", "", 24),
   ("When you speak, they answer correctly, yet every reply feels memorised, as though learned from listening through walls. Their voice is *2{too careful|too flat|just slightly behind the movement of the mouth|nearly but not quite the right pitch}.", "", 22),
   ("You realise with a slow coldness that if you invited it closer, it would come gladly, and would not stop where a child ought to stop.", "", 14),
]
            );

            AddDream("Your Reflection Learns", 25,
            [
               ("You catch sight of yourself in a mirror and are relieved, at first, to see nothing unusual.", "", 14),
   ("Then the reflection begins to lag by the smallest fraction. You turn your head, and it follows a heartbeat later. You raise a hand, and it studies the gesture before repeating it.", "", 22),
   ("Soon it is no longer merely delayed. It is practising. It repeats your expressions with unsettling concentration: *1{a smile it has not quite mastered|a blink that lasts too long|a frown assembled from separate parts of the face|a look of surprise that seems delighted rather than alarmed}.", "", 26),
   ("At last you stand perfectly still, and the reflection continues moving on its own, pleased to have no further need of instruction.", "", 14),
]
            );

            AddDream("Skin Like Clothing", 25,
            [
               ("You are speaking with someone when you notice a seam near the hairline, so fine that you first mistake it for a shadow.", "", 18),
   ("As they continue talking, the seam deepens. A little fold of skin lifts at the edge like cloth caught on a nail. Beneath it is *1{another face pressing upward|darkness with something wet moving in it|a pale inner surface not meant for air|the suggestion of many tiny mouths opening and closing}.", "", 26),
   ("The person does not seem distressed. If anything, they seem embarrassed to have been noticed before they were ready. Their hands rise slowly, not to cover the tear, but to smooth it wider.", "", 22),
   ("With terrible gentleness they say, 'I was hoping this one would pass for longer.'", "emote moan|moans uneasily in ~!poss sleep.", 12),
]
            );

            AddDream("The Crowd with One Face", 25,
            [
               ("You find yourself among a crowd of ordinary people moving about their business. At first the strangeness is only a feeling, a pressure at the back of the mind.", "", 16),
   ("Then you begin to notice that every face in the crowd is the same face, altered only by *1{age|hairstyle|expression|angle of light}. They speak in different voices, wear different clothes, and move with different habits, but the same features recur again and again.", "", 24),
   ("No one else seems alarmed. The crowd flows past in a hundred small human ways, yet every time someone turns toward you, it is the same person arriving from another direction.", "", 22),
   ("Eventually one of them leans close and asks, with sincere concern, 'Why do you keep pretending we're different?'", "", 14),
]
            );

            AddDream("The Thing Under the Bed Knows Your Name", 25,
            [
               ("You are alone in a room that ought to feel safe. There is a bed, a chair, a door, and the ordinary arrangement of familiar things.", "", 16),
   ("From beneath the bed comes a voice speaking your name. It does not growl or snarl; it speaks with *1{gentleness|curiosity|the tone of someone trying not to frighten you|the soft patience of a loved one waking you from sleep}.", "", 24),
   ("It says it has been listening to you for a very long time. It knows *2{which floorboards you avoid|which lies you regret most|the sound of your breathing when you are afraid|the words you say in your sleep when you think no one hears}.", "", 24),
   ("You do not look beneath the bed. The dream does not require that. It is enough to know that whatever is under there has learned you so thoroughly that it can almost pass for someone who cares.", "emote cry|cries out softly in ~!poss sleep.", 12),
]
            );
            #endregion

            #region Modern Only
            if (modernDreams)
            {
                AddDream("Crossing the Car Park", 71,
                [
                   ("You are walking across a large car park toward some ordinary errand.", "", 12),
               ("Everything is still except for the sound of your own footsteps.", "", 10),
            ]
                );

                AddDream("Checking the Mail", 67,
                [
                   ("You open a letterbox or slot to see whether anything has arrived.", "", 12),
               ("Inside are papers and envelopes that seem important only for a moment.", "", 10),
            ]
                );

                AddDream("A Quiet Bus Ride", 72,
                [
                   ("You are riding on a bus, watching familiar streets pass by the window.", "", 14),
               ("No one speaks to you, and you are content to sit and watch.", "", 10),
            ]
                );

                AddDream("A Lift Ride", 69,
                [
                   ("You step into a lift with a few other quiet people.", "", 10),
               ("It rises or falls for longer than expected before the doors open.", "", 10),
            ]
                );

                AddDream("Battery on One Percent", 118,
    [
       ("You notice your phone in your hand and see the battery indicator glowing red at *1{1%|2%|3%}. At once, everything else in the dream seems to depend on keeping it alive just a little longer.", "", 16),
   ("You need it for something important: *2{to call someone back|to show a message|to find your way|to prove something that only the screen can show}. Every tap feels expensive, every second of brightness wasteful.", "", 20),
   ("You lower the screen brightness, close things in a hurry, and start searching for power. There must be a charger somewhere, but all you can find are *3{cables with the wrong connector|outlets hidden behind furniture|charging stations already in use|adapters that almost fit but do not}.", "", 22),
   ("The percentage drops anyway. *1{1%|2%|3%} becomes 0, the screen freezes on something vital, and you stab at the button with growing disbelief.", "", 18),
   ("Just as the screen goes black, you are seized by the certainty that whatever mattered most was about to appear.", "", 12),
]
    );

                AddDream("Unread", 112,
                [
                   ("Your phone begins with a single notification, then another, then a whole stack of them sliding down the screen.", "", 14),
   ("Messages arrive from *1{friends|coworkers|family|numbers you ought to recognise}, mixed with reminders, alerts, updates, and little red badges that multiply faster than you can clear them.", "", 20),
   ("Each message seems to matter in a different way. One asks where you are. One says it is urgent. One contains only *2{a missed call|three dots that never resolve|a photograph you cannot quite interpret|a short sentence that feels heavier than it should}.", "", 22),
   ("You open one thread and find ten more underneath it. You answer one message and two new ones appear. Somewhere in the flood is the thing you actually need to respond to, but the screen keeps shifting before you can find it.", "", 22),
   ("At the height of it, the phone vibrates continuously in your hand like a trapped little animal.", "", 10),
]
                );

                AddDream("Still on Camera", 106,
                [
                   ("You are in an online meeting that seems to have been going on for a very long time.", "", 14),
   ("Rows of little faces fill the screen. Some are listening. Some are frozen mid-expression. Some have their cameras off, represented only by *1{initials in circles|blank silhouettes|old profile pictures|names without faces}.", "", 20),
   ("You are trying to appear normal while quietly dealing with *2{a room that will not stay tidy|background noise that keeps returning|the suspicion that you have joined the wrong meeting|the awful feeling that everyone can see more of you than you intended}.", "", 22),
   ("Then comes the deeper dread: are you muted? Are you unmuted? The tiny icon seems to change every time you look. You begin speaking and no one reacts. You stop speaking and suddenly several people turn toward the screen as if they heard everything.", "", 22),
   ("A voice says your name and asks if you can share your screen.", "", 10),
]
                );

                AddDream("Password Reset", 110,
                [
                   ("You are trying to sign in to something essential, something you use all the time and ought to access without thinking.", "", 14),
   ("Your usual password fails. You try *1{the older one|the obvious variation|the one with the extra number|the one you are certain must be right}, and it fails too.", "", 20),
   ("The site tells you that you have only a few attempts left. Then it asks you to confirm your identity with *2{a code sent elsewhere|a question you do not remember setting|a recovery method you cannot access|a device that is no longer with you}.", "", 22),
   ("You request a reset link, but the email never arrives, or arrives in an inbox that belongs to some stranger version of your life. Every step promises access and produces only another locked page.", "", 22),
   ("At the end, the screen politely informs you that for your security, you must try again later.", "", 12),
]
                );

                AddDream("Rerouting", 102,
                [
                   ("You are following directions on a map app, walking or driving through a place that becomes less familiar with every turn.", "", 16),
   ("The voice tells you to continue for *1{three minutes|five hundred metres|one more block|a short distance}, and then immediately changes its mind. Recalculating. Rerouting. Recalculating again.", "", 22),
   ("You pass *2{intersections that branch too many ways|roads closed without explanation|lanes that become something else halfway along|paths that look right until you are already committed to them}.", "", 22),
   ("The little marker representing you drifts slightly off the map, then spins, then snaps into place facing the wrong direction. The destination remains tantalisingly near, always just beyond the next instruction.", "", 20),
   ("Eventually the map offers a route that leads directly back to where you began.", "", 12),
]
                );

                AddDream("Infinite Scroll", 96,
                [
                   ("You pick up your phone for one small reason and find yourself already inside a feed.", "", 14),
   ("Images, posts, videos, and headlines pass beneath your thumb in a smooth endless stream: *1{news that sounds important|people living brighter lives|funny things you nearly laugh at|arguments you do not remember choosing to watch}.", "", 22),
   ("Every few swipes you think you have reached the end, only for the feed to refresh with more. Something catches your eye, then vanishes when you try to return to it. Another post seems written specifically for you, though by whom you cannot tell.", "", 22),
   ("Time passes strangely. The light around you changes, or the room changes, or perhaps you do. Still the feed continues, offering *2{one more clip|one more update|one more thing you ought to know|one more reason not to put the device down}.", "", 20),
   ("Somewhere within it is a post you are desperate to find again, but the scroll will never take you back to exactly the right place.", "", 12),
]
                );

                AddDream("The Wrong Charger", 104,
                [
                   ("You have a device that must be charged before anything else can happen.", "", 14),
   ("You find a cable immediately and feel relief, but the connector is *1{slightly too large|slightly too small|the wrong shape entirely|almost right except for one stubborn detail}.", "", 18),
   ("So you search further and turn up a tangle of wires, plugs, docks, and adapters. Every new piece looks promising until you try it. One fits the device but not the wall. One fits the wall but not the device. One fits both and carries no power at all.", "", 24),
   ("People nearby offer helpful suggestions involving *2{a drawer full of old electronics|someone else's charger|an adapter with too many moving parts|a cable you were just holding a moment ago}. None of it works for more than a second.", "", 20),
   ("The battery remains stubbornly low while the pile of almost-correct equipment grows around you.", "", 12),
]
                );

                AddDream("Package Out for Delivery", 92,
                [
                   ("You are waiting for a package that feels oddly important, though you are not entirely sure what is inside.", "", 14),
   ("The tracking page updates constantly: *1{Out for delivery|One stop away|Delayed|Arriving today by end of day}. Each message changes your mood at once.", "", 20),
   ("You stay close to where you think it should arrive. Then you are told it was attempted while you were elsewhere, or handed to *2{a neighbour you do not know|a collection point you cannot locate|a safe place that does not sound safe at all|someone whose name means nothing to you}.", "", 22),
   ("You follow fresh instructions through halls, doors, gates, and little maps with pulsing dots, always arriving just after the package has moved on again.", "", 20),
   ("At last you hold a box of exactly the right size, only to realise it has somebody else's name on it.", "", 12),
]
                );

                AddDream("Too Many Tabs Open", 98,
                [
                   ("You are working at a computer and realise you have more tabs open than can be sensibly seen.", "", 14),
   ("Tiny slivers line the top of the browser, each one containing *1{a task you meant to finish|an article you meant to read|a form you still need to submit|something you opened for just a second and never closed}.", "", 22),
   ("You begin trying to sort them. Close the unnecessary ones, group the important ones, find the tab playing *2{faint music|a voice from a video|an ad you cannot silence|some small looping sound}. But every click reveals another layer underneath.", "", 24),
   ("One tab contains the information you urgently need, but its title is cut off, and every tab you open turns out to be almost, but not quite, the correct one.", "", 18),
   ("By the end, the browser has become a kind of maze built out of your own unfinished intentions.", "", 12),
]
                );

                AddDream("Group Chat Drift", 100,
                [
                   ("You are part of a group chat that is active enough to demand attention, but too active to fully follow.", "", 14),
   ("Messages leap from topic to topic. A joke about *1{someone's lunch|a shared memory|a missing file|weekend plans} is immediately followed by a serious discussion, then a photo, then several reactions that make sense only if you saw what came before.", "", 22),
   ("You try to contribute, but each time you type, the conversation has already moved on. You erase one message, rewrite another, and finally send something just as the group pivots to *2{different plans|a different person|a different crisis|an entirely different tone}.", "", 22),
   ("Then someone replies directly to you with perfect confidence, as if you have committed yourself to something specific. Others add thumbs-up signs, laughing reactions, and short answers that assume you know exactly what has been decided.", "", 20),
   ("You scroll up looking for the point where you lost the thread and discover there may never have been one.", "", 12),
]
                );
            }

            AddDream("The Museum of Forgotten Faces", 77,
            [
               ("You wander through a museum whose walls are crowded with portraits. Each face feels familiar, though you do not truly know any of them.", "", 18),
   ("As you pass, the painted eyes follow you. Some faces slowly change into *1{older versions of themselves|children who resemble the elders|your own features altered by lives you never lived|the faces of strangers from dreams you do not remember having}.", "", 24),
   ("Beneath each portrait is a little plaque, but the writing never stays still. It shifts between *2{names that vanish as you read them|dates from impossible centuries|sentences that seem to describe your private thoughts|symbols that you somehow understand only when looking away}.", "", 22),
   ("At the end of the hall hangs an empty frame of great size, and the longer you stand before it, the more certain you become that it is not empty at all.", "", 16),
]
            );

            AddDream("The Clock with Soft Bones", 75,
            [
               ("In the middle of a bare room stands an enormous clock. Its face is familiar enough, but its frame is made not of wood or metal, only of pale curved bones that bend slightly when the room breathes.", "", 18),
   ("The hands move irregularly: *1{too quickly for an hour, too slowly for a minute|backward in careful little jerks|in circles within circles|not around the face at all, but through it like fish through water}.", "", 22),
   ("Each tick produces something instead of sound: *2{a falling feather|a droplet of black milk|a memory you almost recover and then lose|a tiny door opening somewhere out of sight}. You begin to feel the clock is measuring not time, but some softer substance that exists inside you.", "", 26),
   ("When you finally look away, the ticking continues from inside your own chest.", "", 14),
]
            );
            #endregion

            #region Pre-Modern Only
            if (oldDreams)
            {
                AddDream("Walking Through the Woods", 70,
            [
                ("You are walking through a quiet wood, following a path that is familiar but hard to remember.", "", 12),
            ("The trees around you are *1{tall and straight|gnarled and twisted|sparse and scraggly|thick with leaves}, and the ground is *2{soft with moss|hard-packed dirt|strewn with leaves|dotted with flowers}.", "", 18),
        ]
            );

                AddDream("Storm Over the Fields", 104,
    [
       ("You are out in open country, where the land stretches gently away and the sky feels much too large.", "", 16),
   ("At first it is only a change in the air: *1{a chill on your skin|a hush among the grasses|a weight in the clouds|the scent of rain on dry earth}. Then you see the darkening sky rolling in across the fields.", "", 20),
   ("You know there is work yet to do before the storm breaks: *2{tools to gather|animals to bring in|bundles to cover|something important still lying out in the open}. Your hands move quickly, but the weather moves quicker.", "", 22),
   ("A distant rumble passes over the land, and the first heavy drops begin to fall.", "", 14),
]
    );

                AddDream("Calling the Flock", 98,
                [
                   ("You are walking across rough pasture, calling softly for animals that ought to be nearby.", "", 15),
   ("The land is full of little rises and hollows, and every time you think you have spotted them, it turns out to be *1{a patch of pale grass|a stone in the distance|a shadow beneath a tree|something that only looked alive from far away}.", "", 20),
   ("At last you hear movement and low sounds carried on the wind. One by one, *2{sheep|goats|other small animals|a scattered little herd} appear from behind the ground's uneven folds.", "", 20),
   ("You feel the quiet satisfaction of counting them and finding that, for now at least, none are truly lost.", "", 14),
]
                );

                AddDream("Drawing Water", 96,
                [
                   ("You stand beside a well or water source in the cool stillness of the day.", "", 15),
   ("The rope runs through your hands, rough and familiar, while the bucket descends into *1{dark water|clear water deep below|a place hidden from the light|the echoing cool beneath the earth}.", "", 18),
   ("When you draw it back up, the weight feels good in your arms. The water gleams, and for a few moments the whole task seems complete and orderly in a deeply pleasing way.", "", 20),
   ("You carry it away carefully, listening to the soft shifting sound of water with every step.", "", 14),
]
                );

                AddDream("Haymaking", 92,
                [
                   ("The day is warm and dry, and all around you cut grass lies in long sweet-smelling rows.", "", 16),
   ("You work steadily, turning and gathering it with others or alone; the labour is tiring, but it has the satisfying rhythm of necessary work.", "", 20),
   ("Now and then you pause to look across the land and judge the weather. *1{The sun still holds|thin clouds begin to gather|a breeze stirs the cut grass|everything smells of summer and dust}.", "", 18),
   ("The pile grows higher, and with it grows the feeling that something important is being safely brought in.", "", 14),
]
                );

                AddDream("The Orchard Path", 88,
                [
                   ("You walk through rows of trees heavy with leaf and fruit, their branches arching overhead in a hush of green.", "", 16),
   ("The ground beneath is soft with fallen things: *1{blossoms|leaves|small windfalls|twigs and grass}. Somewhere nearby bees hum, and the air is sweet.", "", 20),
   ("You reach up to take one piece of fruit, turning it in your hand. It seems *2{perfectly ripe|warmer from the sun than you expected|heavier than it looks|so fragrant that it briefly fills the whole dream}.", "", 18),
   ("For a little while, there is nothing to do but walk slowly among the trees and be glad of their shade.", "", 14),
]
                );

                AddDream("Mending the Fence", 90,
                [
                   ("You are following the boundary of a field or yard, looking for the place where the fence has given way.", "", 15),
   ("At first the work seems simple. Then you find the damage: *1{a broken rail|a loosened post|a gap beneath the line|a section bent and half-collapsed}.", "", 18),
   ("You set about putting it right with patient hands, testing each part, straightening what can be straightened, making do with what is at hand.", "", 20),
   ("When at last you stand back, the boundary is whole again, and the sight of it gives you a small, solid pride.", "", 14),
]
                );

                AddDream("The Cart Track", 86,
                [
                   ("You are travelling slowly along a country track, with fields, hedges, or open ground spreading out on either side.", "", 16),
   ("The movement is steady and uneven, marked by the soft creak of wood, the shift of weight, and the long rhythm of going somewhere without hurry.", "", 20),
   ("You pass *1{a gate hanging open|a distant farmhouse or cottage|workers in a field|a line of trees marking water or a boundary}. None of it is strange, but all of it seems quietly important.", "", 20),
   ("By the time the track bends out of sight, you have forgotten where you started and no longer mind.", "", 14),
]
                );

                AddDream("Night in the Byre", 94,
                [
                   ("You are in a dim shelter with animals resting nearby, the darkness warm and close rather than frightening.", "", 15),
   ("You hear *1{slow breathing|the shift of hooves on straw|the rustle of animals settling|the occasional soft lowing or snort} in the dark. The sounds come and go like waves.", "", 20),
   ("Some small task keeps you there: *2{checking on them|bringing feed|making sure all is secure|simply keeping watch for a while}. The nearness of living things is strangely comforting.", "", 20),
   ("Beyond the walls is night and weather and distance. Within is warmth, breath, and the peace of things safely gathered in.", "", 14),
]
                );

                AddDream("Seedtime", 84,
                [
                   ("You are walking a strip of fresh-turned earth, scattering seed with a practised motion of the hand.", "", 15),
   ("Each cast falls a little differently: *1{lightly and wide|close and careful|in a neat arc|with a rhythm your body seems to know without thinking}. The soil receives it all without complaint.", "", 20),
   ("Behind you the ground looks only slightly changed, yet you feel the quiet gravity of having placed something living into the earth.", "", 18),
   ("It is work full of patience, full of trust, and in the dream that trust feels easy.", "", 14),
]
                );

                AddDream("The River Ford", 100,
                [
                   ("You come to a shallow crossing where water moves over stones in a clear, steady rush.", "", 15),
   ("The crossing is possible, but not effortless. You test each step carefully while the current presses at your legs or at the feet of *1{the animal beside you|the cart you guide|the load you carry|your own unsteady footing}.", "", 22),
   ("For a few moments all your thought narrows to the next stone, the next step, the next little adjustment to keep balance and continue forward.", "", 18),
   ("Then you are across, and the far bank feels more solid and welcoming than any ground has a right to feel.", "", 14),
]
                );

                AddDream("The Empty Fold", 112,
    [
       ("You wake before dawn with the uneasy certainty that something is wrong, though the yard and fields lie quiet under the dim light.", "", 16),
   ("When you go to check the animals, the fold or pen stands open where it should have been fastened. Inside there is only *1{trampled straw|an overturned bucket|the marks of hooves in the mud|a stillness that feels accusatory}.", "", 22),
   ("You hurry along hedges, through gates, and across dark pasture, calling until your throat hurts. Now and then you hear *2{a distant bleat|movement in the brush|the creak of something shifting in the wind|nothing at all but your own footsteps}.", "", 22),
   ("By the time the sky begins to pale, you still have not found what was lost, and the morning feels colder for it.", "", 14),
]
    );

                AddDream("Blight in the Garden", 104,
                [
                   ("You are walking among rows of growing things that ought to be healthy at this time of year.", "", 15),
   ("At first only a few leaves are wrong: *1{spotted brown|curled black at the edges|gone pale and sickly|drooping as though scorched}. Then you see the damage everywhere you look.", "", 20),
   ("You kneel to touch one plant after another, hoping each time to find you were mistaken. Instead the sickness seems to spread ahead of your gaze, claiming *2{the kitchen herbs|the beans or peas|the roots beneath the soil|every tender green thing in reach}.", "", 22),
   ("You stand at last in the middle of the ruined rows, helpless before the quiet thoroughness of loss.", "", 14),
]
                );

                AddDream("The Cow That Would Not Rise", 108,
                [
                   ("There is trouble in the byre or shelter, and you feel it before you understand what your eyes are seeing.", "", 15),
   ("One of the larger animals lies on its side in the straw, breathing hard, its eyes wide and wet. You call to it, pull at it, urge it up, but it only *1{shudders weakly|shifts and settles again|kicks once without strength|turns its head toward you and does not rise}.", "", 24),
   ("Others gather around with anxious voices, each offering *2{advice|a prayer|some hurried remedy|the hopeless reassurance that it may yet stand}. The air is close with straw, sweat, and dread.", "", 20),
   ("The whole dream hangs on the terrible waiting, on the hope that the creature will rise, and the creeping knowledge that it may not.", "", 14),
]
                );

                AddDream("Rain at Harvest", 110,
                [
                   ("The crop stands ready, and the whole countryside seems poised on the edge of being gathered in.", "", 15),
   ("Then the weather turns. Clouds mass over the land, and wind runs through the stalks in uneasy waves. You and others hurry to save what can be saved, bundling and carrying while the first drops strike the ground.", "", 22),
   ("Soon the rain comes in earnest, flattening what stood proud a moment before. *1{Bundles darken and sag|the cut rows turn heavy and sodden|the ground becomes slick underfoot|all your haste begins to seem too little}.", "", 22),
   ("You keep working long after it is clear the storm will take more than you can protect.", "", 14),
]
                );

                AddDream("The Broken Wheel", 96,
                [
                   ("You are on a lonely road or track with a load that must reach its destination.", "", 15),
   ("For a time the journey is only slow and tiring. Then comes the sharp crack of timber, and one side of the cart lurches violently downward.", "", 18),
   ("You stand in the dust or mud staring at the damage: *1{a wheel split at the rim|a spoke broken clean through|the axle warped and groaning|the whole weight leaning wrong}. Around you there is *2{no help in sight|no house nearby|too much open country|the sinking feeling of wasted time}.", "", 24),
   ("You begin unloading or lifting or cursing under your breath, already knowing the day has turned against you.", "", 14),
]
                );

                AddDream("The Fox in the Hen Yard", 102,
                [
                   ("A sudden flurry of noise breaks the quiet: wings beating, birds crying out, something alive moving where it should not be.", "", 14),
   ("You run toward the yard and arrive just in time to see a lean shape dart between shadows. Feathers drift through the air. The birds are in wild confusion, crashing into one another and the fence alike.", "", 22),
   ("You grab for *1{a stick|a stone|the gate|anything at all}, shouting as you pursue the intruder. It slips through *2{a gap you had not noticed|the fence itself|a hole under the boards|the darkness beyond the yard} before you can stop it.", "", 22),
   ("When the noise dies, what remains is the dreadful work of counting what is left.", "", 14),
]
                );

                AddDream("No Fire", 106,
                [
                   ("The room is cold, far colder than it should be, and the hearth is dark.", "", 15),
   ("You kneel to coax flame from tinder and ember, but nothing takes properly. The wood is *1{damp|too green|crumbled to uselessness|there and yet somehow unburnable}. Smoke curls up in mean little breaths, stinging your eyes.", "", 22),
   ("You try again and again with numb fingers while the cold seems to spread through the walls and floor. Others wait nearby wrapped in silence, depending on you more than they say.", "", 20),
   ("Still the hearth refuses warmth, and the room grows heavier with each failure.", "", 14),
]
                );

                AddDream("The Missing Child", 120,
                [
                   ("At first it is only the smallest gap in the pattern of things, a missing voice where there should be one.", "", 14),
   ("You look up expecting to see the child close by, but they are not there. What follows is a mounting search through *1{the yard and outbuildings|the lane and nearby fields|tall grass and hedges|every place a child should and should not be}.", "", 22),
   ("You call until panic roughens your voice. Other people join, each taking a different direction, each returning with nothing but *2{shaken heads|more questions|fresh fear|the insistence that they cannot have gone far}.", "", 22),
   ("The dream tightens around the sick certainty that somewhere very close, disaster may already be waiting.", "", 14),
]
                );

                AddDream("The Well Runs Low", 94,
                [
                   ("You lower the bucket expecting the familiar sound of water below, but hear instead a thin, delayed splash.", "", 15),
   ("When you draw it up, there is scarcely enough in it to wet the bottom. You try again, letting it fall deeper, and again bring up only *1{muddy water|a little foul-smelling trickle|less than before|proof of how little remains}.", "", 22),
   ("Soon others are standing nearby, each silent in their own way, each understanding what it means. The day suddenly feels hotter, the mouths drier, the distance to other water much farther than it was yesterday.", "", 22),
   ("You stare down into the dark shaft as though it might yield more if only you wanted it hard enough.", "", 14),
]
                );

                AddDream("The Crows Descend", 98,
                [
                   ("You are in a field where seed or young shoots have only lately been set to grow.", "", 15),
   ("Then the crows come, first in ones and twos, then in a black restless company. They settle *1{on the furrows|along the fence posts|in the nearby trees|all across the field itself}, watching and hopping boldly nearer.", "", 20),
   ("You shout and wave your arms and run at them, and they rise only long enough to circle and settle again. Wherever they land, they peck at *2{the seed|the tender shoots|the soft earth you worked so hard to turn|the promise of what has not yet had time to grow}.", "", 24),
   ("The whole dream is filled with the maddening sense of being outnumbered by small losses that will not stop.", "", 14),
]
                );
            }
            #endregion
        }
    }
}
