/*------------------------------------------------------------------------\
|  constants.c : Program Constants                    www.middle-earth.us | 
|  Copyright (C) 2004, Shadows of Isildur: Traithe                        |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/

#include "structs.h"




const char *verbose_dirs[] = {
  "the north",
  "the east",
  "the south",
  "the west",
  "above",
  "below",
  "outside",
  "inside",
  "the northeast",
  "the northwest",
  "the southeast",
  "the southwest",
  "\n"
};

const char *season_string[6] = {
  "spring",
  "summer",
  "autumn",
  "early winter",
  "deep winter",
  "late winter"
};

const char *month_short_name[12] = {
  "First",
  "Second",
  "Third",
  "Fourth",
  "Fifth",
  "Sixth",
  "Seventh",
  "Eighth",
  "Ninth",
  "Tenth",
  "Eleventh",
  "Twelfth"
};

const char *month_lkup[] = {
  "(null)",
  "Narvinye",
  "Nenime",
  "Sulime",
  "Viresse",
  "Lotesse",
  "Narie",
  "Cermie",
  "Urime",
  "Yavannie",
  "Narquelie",
  "Hisime",
  "Ringare",
  "\n"
};

const char *somatics[] = {
      "an unknown somatic effect",
      "a muscle cramp",
      "twitching",
      "tremors",
      "paralysis",
      "stomach ulcer",
      "vomiting",
      "vomiting blood",
      "blindness",
      "blurred vision",
      "double vision",
      "dilated pupils",
      "contracted pupils",
      "lacrimation",
      "ptosis",
      "tinnitus",
      "deafness",
      "ear imbalance",
      "anosmia",
      "rhinitis",
      "salivation",
      "toothache",
      "dry mouth",
      "halitosis",
      "difficulty breathing",
      "wheezing",
      "rapid breathing",
      "shallow breathing",
      "fluidous lungs",
      "heart palpitations",
      "coughing fits",
      "pneumonia",
      "psychosis",
      "delerium ",
      "a comatose state",
      "convulsions",
      "a headache",
      "confusion",
      "parethesias",
      "ataxia",
      "nervous imbalance",
      "cyanosis of the skin",
      "dryness of the skin",
      "corrosion of the skin",
      "jaundice of the skin",
      "redness of the skin",
      "a rash on the skin",
      "hairloss",
      "edema of the skin",
      "burns on the skin",
      "pallor of the skin",
      "the sweats",
      "weight loss",
      "lethargy",
      "appetite loss",
      "low blood pressure",
      "high blood pressure",
      "a fast pulse",
      "a slow pulse",
      "hyperthermia",
      "hypothermia",
      "minorly stunned",
      "severely stunned",
      "a jarred left arm",
      "a broken left arm",
      "a jarred right arm",
      "a broken right arm",
      "a jarred left leg",
      "a broken left leg",
      "a jarred right leg",
      "a broken right leg",
      "winded",
      "a broken rib",
      "paralysing spider venom",
      "necrotic spider venom",
      "sleeping spider venom",
      "twitching snake venom",
      "necrotic snake venom",
      "drowsy snake venom",
      "toad poison",
      "toad-induced visions",
      "plant drowsiness",
      "concentrated dream moss",
      "vicious snowfruit",
      "plant naseua",
      "boiled nightcap mushroom",
      "mashed screamcaps",
      "mashed scarbane root",
      "crushed bright weed",
      "fermented bile fruit",
      "plant spider anti-venom",
      "plant wraithcure",
      "wraithcurse",
      "soulspite",
      "benign snowfruit",
};


const char *skills[] = {
  "Unused",
  "Brawling",
  "Small-Blade",
  "Sword",
  "Axe",
  "Polearm",
  "Club",
  "Flail",
  "Double-Handed",
  "Sole-Wield",
  "Shield-Use",
  "Dual-Wield",
  "Throwing",
  "Blowgun",
  "Sling",
  "Hunting-bow",
  "Warbow",
  "Avert",
  "Defunct",

  "Sneak",
  "Hide",
  "Steal",
  "Picklock",
  "Forage",
  "Barter",
  "Ride",
  "Butchery",
  "Poisoning",
  "Herbalism",

  "Clairvoyance",
  "Danger-Sense",
  "Empathy",
  "Hex",
  "Psychic-Bolt",
  "Prescience",
  "Aura-Sight",
  "Telepathy",

  "Dodge",
  "Metalcraft",
  "Woodcraft",
  "Lumberjack",
  "Cookery",
  "Hideworking",
  "Brewing",
  "Literacy",
  "Apothecary",
  "Mining",
  "Tracking",
  "Healing",

  "Taliska",
  "Haladin",
  "Thrunon",
  "Beast-Tongue",
  "Valarin",
  "Nandorin",
  "Druag",
  "Sindarin",
  "Quenya",
  "Avarin",
  "Khuzdul",
  "Orkish",
  "Trollish",

  "Sarati",
  "Tengwar",
  "Cirth",
  "Valarin-Script",

  "Black-Wise",
  "Grey-Wise",
  "White-Wise",
  "Runecasting",
  "Astronomy",
  "Eavesdrop",
  "\n"
};

const char *where[] = {
  "<used as light>          ",	// 0
  "<worn on finger>         ",	// 1
  "<worn on finger>         ",
  "<worn at neck>           ",
  "<worn at neck>           ",
  "<worn on body>           ",
  "<worn on head>           ",
  "<worn on legs>           ",
  "<worn on feet>           ",
  "<worn on hands>          ",
  "<worn on arms>           ",	// 10
  "<worn as shield>         ",
  "<worn about body>        ",
  "<worn about waist>       ",
  "<worn on right wrist>    ",
  "<worn on left wrist>     ",
  "<wielded primary>        ",
  "<wielded secondary>      ",
  "<wielded both hands>     ",
  "<held>                   ",
  "<worn on belt>           ",	// 20
  "<worn on belt>           ",
  "<across the back>        ",
  "<over the eyes>          ",
  "<worn at throat>         ",
  "<worn on the ears>       ",
  "<worn over shoulder>     ",
  "<worn over shoulder>     ",
  "<worn on right ankle>    ",
  "<worn on left ankle>     ",
  "<worn in hair>           ",	// 30
  "<worn on face>           ",
  "",
  "",
  "<about upper right arm>  ",	// 34
  "<about upper left arm>   ",	// 35
  "\n"
};

const char *locations[] = {
  "hand",
  "finger",
  "finger",
  "neck",
  "neck",
  "body",
  "head",
  "legs",
  "feet",
  "hands",
  "arms",
  "hands",
  "body",
  "waist",
  "wrist",
  "wrist",
  "hand",
  "hand",
  "hand",
  "hand",
  "belt",
  "belt",
  "back",
  "eyes",
  "throat",
  "ears",
  "shoulder",
  "shoulder",
  "ankle",
  "ankle",
  "hair",
  "face",
  "something",
  "something",
  "arm",
  "arm",
  "\n"
};


const char *color_liquid[] = {
  "clear",
  "brown",
  "clear",
  "brown",
  "dark",
  "golden",
  "red",
  "green",
  "clear",
  "light green",
  "white",
  "brown",
  "black",
  "red",
  "clear",
  "black"
};

const char *fullness[] = {
  "less than half ",
  "about half ",
  "more than half ",
  ""
};


const char *exit_bits[] = {
  "IsDoor",
  "Closed",
  "Locked",
  "RSClosed",
  "RSLocked",
  "PickProof",
  "Secret",
  "Trapped",
  "Toll",
  "IsGate",
  "\n"
};

const int earth_grid[] = {
  120,				/* Inside */
  320,				/* City */
  170,				/* Road */
  110,				/* Trail */
  100,				/* Field */
  90,				/* Woods */
  80,				/* Forest */
  80,				/* Hills */
  65,				/* Mountains */
  120,				/* Swamp */
  270,				/* Water_swim */
  340,				/* Water_noswim */
  585,				/* Ocean */
  510,				/* Dock */
  230,				/* Reef */
  580,				/* Crowsnest */
  135,				/* Pasture */
  95,				/* Heath */
  75,				/* Pit */
  100				/* Lean-to  */
};

const int wind_grid[] = {
  640,				/* Inside */
  175,				/* City */
  220,				/* Road */
  240,				/* Trail */
  80,				/* Field */
  240,				/* Woods */
  280,				/* Forest */
  120,				/* Hills */
  100,				/* Mountains */
  140,				/* Swamp */
  90,				/* Water_swim */
  60,				/* Water_noswim */
  50,				/* Ocean */
  75,				/* Dock */
  300,				/* Reef */
  55,				/* Crowsnest */
  120,				/* Pasture */
  65,				/* Heath */
  850,				/* Pit */
  100				/* Lean-to */
};

const int fire_grid[] = {
  120,				/* Inside */
  115,				/* City */
  175,				/* Road */
  190,				/* Trail */
  210,				/* Field */
  275,				/* Woods */
  350,				/* Forest */
  150,				/* Hills */
  135,				/* Mountains */
  475,				/* Swamp */
  525,				/* Water_swim */
  675,				/* Water_noswim */
  895,				/* Ocean */
  520,				/* Dock */
  475,				/* Reef */
  340,				/* Crowsnest */
  125,				/* Pasture */
  140,				/* Heath */
  125,				/* Pit */
  100				/* Lean-to  */
};

const int water_grid[] = {
  450,				/* Inside */
  275,				/* City */
  300,				/* Road */
  275,				/* Trail */
  200,				/* Field */
  140,				/* Woods */
  100,				/* Forest */
  175,				/* Hills */
  225,				/* Mountains */
  75,				/* Swamp */
  60,				/* Water_swim */
  50,				/* Water_noswim */
  30,				/* Ocean */
  65,				/* Dock */
  95,				/* Reef */
  100,				/* Crowsnest */
  275,				/* Pasture */
  175,				/* Heath */
  675,				/* Pit */
  100				/* Lean-to  */
};

const int shadow_grid[] = {
  70,				/* Inside */
  90,				/* City */
  200,				/* Road */
  320,				/* Trail */
  410,				/* Field */
  500,				/* Woods */
  540,				/* Forest */
  490,				/* Hills */
  570,				/* Mountains */
  70,				/* Swamp */
  500,				/* Water_swim */
  500,				/* Water_noswim */
  500,				/* Ocean */
  340,				/* Dock */
  300,				/* Reef */
  650,				/* Crowsnest */
  200,				/* Pasture */
  65,				/* Heath */
  520,				/* Pit */
  510,				/* Lean-to */
};



const char *seasons[] = {
  "Spring",
  "Summer",
  "Autumn",
  "Winter",
  "\n"
};

const char *affected_bits[] = {
  "Undefined",
  "Invisible",
  "Infravision",
  "Detect-Invisible",
  "Detect-Magic",
  "Sense-Life",			/* 5 */
  "Transporting",
  "Sanctuary",
  "Group",
  "Curse",
  "Magic-only",			/* 10 */
  "Poison",
  "AScan",
  "AFallback",
  "Undefined",
  "Undefined",			/* 15 */
  "Sleep",
  "Dodge",
  "ASneak",
  "AHide",
  "Fear",			/* 20 */
  "Follow",
  "Hooded",
  "Charm",			/* was affected_bits[21] */
  "\n"
};



const char *smallgood_types[] = {
  "smallgoods",
  "ore",
  "grain",
  "fur",
  "meat",
  "\n"
};

const char *action_bits[] = {
  "Memory",
  "Sentinel",
  "Rescuer",
  "IsNPC",
  "NoVNPC",
  "Aggressive",
  "Stayzone",
  "Wimpy",
  "Sent-Aggro",
  "BulkTrader",
  "NoOrder",
  "NoBuy",
  "Enforcer",
  "PackAnimal",
  "Vehicle",
  "Stop",
  "Squeezer",
  "Pariah",
  "Mount",
  "Venemous",                   /* Mob has, or should have, some venom */
  "PCOwned",
  "Wildlife",			/* Mob won't attack other wildlife */
  "Stayput",			/* Mob saves and reloads after boot */
  "Passive",			/* Mob won't assist clan brother in combat */
  "Auctioneer",			/* Mob is an auctioneer - auctions.cpp */
  "Econzone",			/* NPC, if keeper, uses econ zone price dis/markups */
  "Jailer",
  "\n"
};


const char *position_types[] = {
  "Dead",
  "Mortally wounded",
  "Unconscious",
  "Stunned",
  "Sleeping",
  "Resting",
  "Sitting",
  "Fighting",
  "Standing",
  "\n"
};

const char *connected_types[] = {
  "Playing",
  "Entering Name",
  "Confirming Name",
  "Entering Password",
  "Entering New Password",
  "Confirming New password",
  "Choosing Gender",
  "Reading Message of the Day",
  "Main Menu",
  "Changing Password",
  "Confirming Changed Password",
  "Rolling Attributes",
  "Selecting Race",
  "Decoy Screen",
  "Creation Menu",
  "Selecting Attributes",
  "New Player Menu",
  "Documents Menu",
  "Selecting Documentation",
  "Reading Documentation",
  "Picking Skills",
  "New Player",
  "Age Select",
  "Height-Frame Select",
  "New Char Intro Msg",
  "New Char Intro Wait",
  "Creation Comment",
  "Read Reject Message",
  "Web Connection",
  "\n"
};

const char *sex_types[] = {
  "Sexless",
  "Male",
  "Female",
  "\n"
};

const char *sex_noun[] = {
  "it",
  "him",
  "her",
  "\n",
};

const char *weather_room[] = {
  "foggy",
  "cloudy",
  "rainy",
  "stormy",
  "snowy",
  "blizzard",
  "night",
  "nfoggy",
  "nrainy",
  "nstormy",
  "nsnowy",
  "nblizzard",
  "day",
  "\n"
};
