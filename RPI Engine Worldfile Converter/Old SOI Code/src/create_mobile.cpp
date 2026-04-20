/*------------------------------------------------------------------------\
|  create_mobile.c : Mobile Autocreation Module       www.middle-earth.us |
|  Copyright (C) 2004, Shadows of Isildur: Traithe                        |
|  All original code, derived under license from DIKU GAMMA (0.0).        |
\------------------------------------------------------------------------*/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <unistd.h>


#include "structs.h"
#include "protos.h"
#include "utils.h"
#include "decl.h"
#include "utility.h"


const char *variable_races[] = {
  "Beorian Human",
  "Marachian Human",
  "Haladin Human",
  "Orc",
  "Cave Troll",
  "Noldo Elf",
  "Sinda Elf",
  "Hill Troll",
  "Avar Elf",
  "Wolf",
  "Giant Spider",
  "Wraith",
  "Horse",
  "Warhorse",
  "Bird",
  "Wolfspawn",
  "Ent",
  "Warhorse",
  "Bird",
  "Wraith",
  "Rodent",
  "Spider",
  "Wolf",
  "\n"
};

const char *spider_adj1[] = {
  "brown-striped",
  "coal-black",
  "grey",
  "red-eyed",
  "albino",
  "grey-striped",
  "black-eyed",
  "beady-eyed",
  "loathesome",
  "vile",
  "red-striped",
  "brown",
  "black-striped",
  "dark-green",
  "thick-carapiced",
  "repulsive",
  "green-striped",
  "dusty-grey",
  "\n"
};

const char *spider_adj2[] = {
  "long-legged",
  "hairy",
  "hairless",
  "hairy-legged",
  "blotchy",
  "black-haired",
  "spikey",
  "ooze-crusted",
  "squat",
  "spindly-legged",
  "greasy-bodied",
  "dust-covered",
  "bloated",
  "nimble",
  "thick-bodied",
  "smooth",
  "sleek",
  "\n"
};

const char *rat_adj1[] = {
  "brown",
  "ochre-furred",
  "grey",
  "chalky-coated",
  "charcoal-furred",
  "sable",
  "albino",
  "white",
  "red-eyed",
  "yellow-eyed",
  "black-eyed",
  "glossy-eyed",
  "dull-eyed",
  "long-tailed",
  "short-tailed",
  "tailless",
  "sharp-toothed",
  "toothless",
  "jagged-toothed",
  "rotting-toothed",
  "large-pawed",
  "small-pawed",
  "\n"
};

const char *rat_adj2[] = {
  "scrawny",
  "emaciated",
  "gaunt",
  "puny",
  "bony",
  "raw-boned",
  "malnourished",
  "lean",
  "scab-skinned",
  "foul",
  "greasy",
  "filthy",
  "dirt-covered",
  "disease-ridden",
  "vile",
  "nimble",
  "large",
  "slick",
  "sleek",
  "agile",
  "small",
  "maimed",
  "crippled",
  "\n"
};

const char *wight_adj1[] = {
  "spectral",
  "shadowy",
  "ghostly",
  "etheral",
  "shadow-forged",
  "ghoulish",
  "ghastly",
  "monstrous",
  "gruesome",
  "baleful",
  "malefic",
  "abhorrent",
  "dreadful",
  "hideous",
  "\n"
};

const char *wight_adj2[] = {
  "chill-eyed",
  "blue-eyed",
  "red-eyed",
  "blazing-eyed",
  "fiery-eyed",
  "frost-eyed",
  "faceless",
  "featureless",
  "taloned",
  "clawed",
  "sharp-nailed",
  "razor-toothed",
  "\n"
};

const char *bird_adj1[] = {
  "tiny",
  "miniscule",
  "small",
  "minute",
  "large",
  "sleek",
  "elegant",
  "plump",
  "majestic",
  "beautiful",
  "predatory",
  "gorgeous",
  "broad-winged",
  "wide-winged",
  "large-winged",
  "brightly-plumed",
  "wildly-plumed",
  "fiery-plumed",
  "dull-plumed",
  "drab-plumed",
  "darkly-plumed",
  "ebon-plumed",
  "reddish-plumed",
  "glossy-feathered",
  "fluffy-feathered",
  "bright-red-feathered",
  "drab-feathered",
  "dun-feathered",
  "white-feathered",
  "pink-feathered",
  "greyish-brown-feathered",
  "brownish-grey-feathered",
  "dull-feathered",
  "black-feathered",
  "blue-black-feathered",
  "midnight-feathered",
  "brown-feathered",
  "tan-feathered",
  "beryl-feathered",
  "blue-feathered",
  "dull-red-feathered",
  "dull-blue-feathered",
  "grey-feathered",
  "dull-grey-feathered",
  "striped",
  "spotted",
  "dappled",
  "\n"
};

const char *bird_adj2[] = {
  "sharp-beaked",
  "pointy-beaked",
  "wide-beaked",
  "broad-beaked",
  "small-beaked",
  "narrow-beaked",
  "large-beaked",
  "huge-beaked",
  "spear-beaked",
  "hook-beaked",
  "wide-billed",
  "broad-billed",
  "small-billed",
  "large-billed",
  "hook-billed",
  "sharp-eyed",
  "black-eyed",
  "wide-eyed",
  "sharp-taloned",
  "fiercely-taloned",
  "large-taloned",
  "long-tailed",
  "short-tailed",
  "wide-tailed",
  "thin-tailed",
  "fan-tailed",
  "large-tailed",
  "long-necked",
  "short-necked",
  "long-legged",
  "short-legged",
  "crowned",
  "\n"
};

const char *war_horse_adj1[] = {
  "regal",
  "well-muscled",
  "heavily-muscled",
  "thickly-muscled",
  "delicate",
  "sinewy",
  "hale",
  "noble",
  "hearty",
  "sturdy",
  "proud",
  "striking",
  "massive",
  "compact",
  "narrow-chested",
  "massive-chested",
  "broad-chested",
  "deep-chested",
  "long-backed",
  "well-groomed",
  "elegant",
  "dainty",
  "\n"
};

const char *horse_adj1[] = {
  "dappled",
  "dun-coated",
  "bay-colored",
  "grey-coated",
  "black-coated",
  "roan-colored",
  "chestnut-coated",
  "spotty-coated",
  "brilliant-white-coated",
  "obsidian-colored",
  "blue-black-coated",
  "midnight-black-colored",
  "pearl-white-coated",
  "flaxen-coated",
  "blotch-brown-coated",
  "mottled-brown-coated",
  "blotchy-grey-coated",
  "mottled-grey-coated",
  "smoky-grey-coated",
  "ash-grey-coated",
  "ebon-coated",
  "white-coated",
  "dark-brown-colored",
  "slate-grey-coated",
  "cream-colored",
  "glossy-coated",
  "curly-coated",
  "rough-coated",
  "\n"
};

const char *horse_adj2[] = {
  "slender-legged",
  "long-legged",
  "short-legged",
  "straight-legged",
  "shaggy-fetlocked",
  "long-necked",
  "short-necked",
  "arch-necked",
  "wide-necked",
  "long-muzzled",
  "short-muzzled",
  "short-tailed",
  "wavy-tailed",
  "long-tailed",
  "flowing-maned",
  "silky-maned",
  "short-maned",
  "abundantly-maned",
  "coarse-maned",
  "white-maned",
  "ebon-maned",
  "brown-maned",
  "auburn-maned",
  "dun-maned",
  "wiry-maned",
  "thick-maned",
  "grey-maned",
  "black-maned",
  "long-maned",
  "wavy-maned",
  "long-eared",
  "small-eared",
  "amber-eyed",
  "black-eyed",
  "glint-eyed",
  "\n"
};

const char *ent_coniferous_adj1[] = {
  "tall",
  "towering",
  "short",
  "windblown",
  "top-heavy",
  "squat",
  "perfectly-formed",
  "bent",
  "gnarled",
  "hauntingly-bare",
  "pinecone-studded",
  "sharp-needled",
  "sparsely-needled",
  "prickly-needled",
  "brown-needled",
  "yellow-needled",
  "yellow-green-needled",
  "gray-green-needled",
  "thickly-needled",
  "long-branched",
  "spindly-branched",
  "many-branched",
  "droopy-branched",
  "mossy-branched",
  "bare-branched",
  "gnarled-branched",
  "crooked-branched",
  "bent-branched",
  "lightning-rent",
  "lightning-scorched",
  "\n"
};

const char *ent_deciduous_adj1[] = {
  "tall",
  "towering",
  "short",
  "windblown",
  "top-heavy",
  "squat",
  "bent",
  "gnarled",
  "hauntingly-bare",
  "leafy",
  "lush",
  "healthy-looking",
  "grey-green-leaved",
  "green-leaved",
  "yellow-green-leaved",
  "yellow-leaved",
  "brown-leaved",
  "thickly-leaved",
  "thinly-leaved",
  "many-leaved",
  "heavily-leaved",
  "spotty-leaved",
  "long-branched",
  "spindly-branched",
  "many-branched",
  "droopy-branched",
  "mossy-branched",
  "bare-branched",
  "gnarled-branched",
  "crooked-branched",
  "bent-branched",
  "lightning-rent",
  "lightning-scorched",
  "\n"
};

const char *ent_adj2[] = {
  "wide-trunked",
  "thin-trunked",
  "massive-trunked",
  "narrow-trunked",
  "lumpy-trunked",
  "straight-trunked",
  "hollow-trunked",
  "bumpy-trunked",
  "twisted-trunked",
  "crooked-trunked",
  "black-barked",
  "dark-barked",
  "thickly-barked",
  "thinly-barked",
  "light-barked",
  "pale-barked",
  "grey-barked",
  "ragged-barked",
  "mottled-barked",
  "reddish-barked",
  "mossy-barked",
  "moss-covered",
  "lichen-covered",
  "lichen-spotted",
  "fungus-sporting",
  "heavily-rooted",
  "\n"
};

const char *troll_adj1[] = {
  "immense",
  "huge",
  "gigantic",
  "monstrous",
  "gargantuan",
  "looming",
  "massive",
  "muscular",
  "well-muscled",
  "sinewy",
  "lumpy",
  "malformed",
  "fleshy",
  "pot-bellied",
  "bloated",
  "obese",
  "deformed",
  "powerful",
  "brutish",
  "savage",
  "broad-shouldered",
  "stumpy",
  "coarsely-haired",
  "shaggy-haired",
  "black-haired",
  "oily-haired",
  "seaweed-green-haired",
  "mangy-haired",
  "purplish-haired",
  "bright-red-haired",
  "dull-red-haired",
  "grimy-haired",
  "dirty-haired",
  "greasy-haired",
  "stringy-haired",
  "patchy-haired",
  "bald",
  "balding",
  "shiny-scalped",
  "wiry-haired",
  "bristly-haired",
  "\n"
};

const char *troll_adj2[] = {
  "slime-green-eyed",
  "puce-eyed",
  "watery-eyed",
  "gooey-eyed",
  "bloodshot-eyed",
  "dim-eyed",
  "dull-eyed",
  "pop-eyed",
  "baggy-eyed",
  "glassy-eyed",
  "glassy-grey-eyed",
  "putrid-yellow-eyed",
  "red-eyed",
  "blood-red-eyed",
  "pink-eyed",
  "dirty-brown-eyed",
  "fierce-eyed",
  "sallow-eyed",
  "snaggletoothed",
  "sawtoothed",
  "sloping-browed",
  "underbitten",
  "wide-mouthed",
  "large-eared",
  "grotesquely-scarred",
  "gruesomely-scarred",
  "scar-covered",
  "flat-nosed",
  "blunt-nosed",
  "boil-covered",
  "warty",
  "hairy",
  "scale-covered",
  "scaly-skinned",
  "grey-skinned",
  "black-skinned",
  "slate-grey-skinned",
  "leathery-skinned",
  "jaundiced",
  "leprous",
  "scabby-skinned",
  "albino-skinned",
  "pimply-skinned",
  "ulcerous-skinned",
  "\n"
};

const char *beor_adj1[] = {
  "acned",
  "cadaverous",
  "dirty",
  "dust-covered",
  "doughy",
  "greasy",
  "jaundiced",
  "livid",
  "hearty",
  "scarred",
  "sun-browned",
  "swarthy",
  "wan",
  "waxy",
  "weatherbeaten",
  "broad-shouldered",
  "powerfully-built",
  "aquiline-nosed",
  "beak-nosed",
  "bent-nosed",
  "knob-nosed",
  "flat-nosed",
  "hawk-nosed",
  "pig-nosed",
  "pug-nosed",
  "athletic",
  "brawny",
  "bent",
  "bow-spined",
  "burly",
  "chubby",
  "colossal",
  "brawny",
  "delicate",
  "diminutive",
  "lithe",
  "large",
  "thin",
  "fat",
  "fleshy",
  "fragile",
  "gangly",
  "gaunt",
  "haggard",
  "hunched",
  "husky",
  "lanky",
  "lean",
  "lithe",
  "lissome",
  "lithe",
  "muscled",
  "obese",
  "lanky",
  "paunchy",
  "slender",
  "petite",
  "portly",
  "pot-bellied",
  "pudgy",
  "reedy",
  "rickety",
  "willowy",
  "robust",
  "rotund",
  "rugged",
  "scrawny",
  "runty",
  "sinewy",
  "runty",
  "skeletal",
  "sleek",
  "slight",
  "slender",
  "slim",
  "spindly",
  "squat",
  "stalwart",
  "statuesque",
  "tall",
  "thickset",
  "thin",
  "waspish",
  "well-muscled",
  "whip-thin",
  "willowy",
  "wiry",
  "grey-eyed",
  "smoky-eyed",
  "dark-grey-eyed",
  "stormy-eyed",
  "storm-grey-eyed",
  "brown-eyed",
  "dark-brown-eyed",
  "muddy-brown-eyed",
  "hazel-eyed",
  "black-eyed",
  "dark-eyed",
  "obsidian-eyed",
  "green-eyed",
  "dark-green-eyed",
  "glassy-eyed",
  "squint-eyed",
  "narrow-eyed",
  "one-eyed",
  "\n"
};

const char *beor_adj2[] = {
  "coal-haired",
  "jet-haired",
  "midnight-haired",
  "onyx-haired",
  "raven-haired",
  "auburn-haired",
  "brown-haired",
  "chestnut-haired",
  "dun-haired",
  "russet-haired",
  "sable-haired",
  "taupe-haired",
  "wheat-haired",
  "henna-haired",
  "dusky-haired",
  "ecru-haired",
  "dark-grey-haired",
  "grey-haired",
  "smoky-grey-haired",
  "dull-grey-haired",
  "ash-grey-haired",
  "dirty-grey-haired",
  "white-haired",
  "black-haired",
  "ebon-haired",
  "dark-haired",
  "brown-haired",
  "chocolate-brown-haired",
  "dark-brown-haired",
  "drab-haired",
  "muddy-brown-haired",
  "dirty-brown-haired",
  "murky-brown-haired",
  "gaunt-cheeked",
  "thin-lipped",
  "dry-lipped",
  "thick-lipped",
  "cleft-chinned",
  "wide-jawed",
  "square-jawed",
  "long-faced",
  "drawn-faced",
  "angular-faced",
  "square-faced",
  "square-jawed",
  "comely-faced",
  "careworn",
  "comely-faced",
  "narrow-faced",
  "\n"
};


const char *marach_adj1[] = {
  "acned",
  "cadaverous",
  "dirty",
  "dust-covered",
  "doughy",
  "fair",
  "greasy",
  "jaundiced",
  "pale",
  "livid",
  "pallid",
  "hearty",
  "scarred",
  "sun-browned",
  "wan",
  "waxy",
  "weatherbeaten",
  "beady-eyed",
  "cock-eyed",
  "owlish",
  "rheumy-eyed",
  "squinty-eyed",
  "aquiline-nosed",
  "beak-nosed",
  "bent-nosed",
  "knob-nosed",
  "flat-nosed",
  "hawk-nosed",
  "pig-nosed",
  "pug-nosed",
  "athletic",
  "brawny",
  "bent",
  "bow-spined",
  "burly",
  "chubby",
  "colossal",
  "brawny",
  "large",
  "fat",
  "fleshy",
  "fragile",
  "gaunt",
  "haggard",
  "hunched",
  "husky",
  "muscled",
  "obese",
  "lanky",
  "paunchy",
  "portly",
  "pot-bellied",
  "pudgy",
  "rickety",
  "robust",
  "rotund",
  "rugged",
  "squat",
  "stalwart",
  "statuesque",
  "svelte",
  "tall",
  "thickset",
  "well-muscled",
  "blue-eyed",
  "azure-eyed",
  "green-eyed",
  "emerald-eyed",
  "jade-eyed",
  "grey-eyed",
  "stormy-eyed",
  "\n"
};

const char *marach_adj2[] = {
  "auburn-haired",
  "copper-haired",
  "red-haired",
  "scarlet-haired",
  "sepia-haired",
  "blonde-haired",
  "golden-haired",
  "ginger-haired",
  "honey-haired",
  "flaxen-haired",
  "sandy-haired",
  "sorrel-haired",
  "tawny-haired",
  "bronze-haired",
  "brown-haired",
  "chestnut-haired",
  "dun-haired",
  "russet-haired",
  "taupe-haired",
  "wheat-haired",
  "dusky-haired",
  "angular-faced",
  "aristocratic",
  "comely-faced",
  "careworn",
  "cherubic",
  "comely-faced",
  "drawn-faced",
  "feline-faced",
  "narrow-faced",
  "square-faced",
  "stoop-shouldered",
  "broad-shouldered",
  "drooping-shouldered",
  "\n"
};

const char *wolf_adj1[] = {
  "sinewy",
  "muscular",
  "powerfully-muscled",
  "thickly-muscled",
  "gaunt",
  "bony",
  "skeletal",
  "rawboned",
  "emaciated",
  "lean",
  "lanky",
  "tawny",
  "grizzled",
  "scraggly",
  "fierce",
  "mangy",
  "thickly-furred",
  "mottle-coated",
  "sleek",
  "brambly-furred",
  "wiry-furred",
  "shaggy-furred",
  "long-whiskered",
  "silken-furred",
  "bristly-furred",
  "spiky-furred",
  "gray-furred",
  "smoky-grey-furred",
  "ash-grey-furred",
  //"black-furred",
  //"ebony-furred",
  "sooty-grey-furred",
  "charcoal-grey-furred",
  //"coal-black-furred",
  //"inky-furred",
  //"ebon-furred",
  //"brown-furred",
  //"muddy-brown-furred",
  //"dark-brown-furred",
  //"jet-black",
  "light-grey",
  "dark-grey",
  //"light-brown",
  //"dark-brown",
  //"muddy-brown",
  "matted-furred",
  "\n"
};

const char *wolf_adj2[] = {
  "narrow-muzzled",
  "sharp-muzzled",
  "short-muzzled",
  "long-muzzled",
  "crook-tailed",
  "hook-tailed",
  "long-necked",
  "sharp-eared",
  "wide-eared",
  "one-eared",
  "narrow-eyed",
  "glassy-eyed",
  //"black-eyed",
  //"grey-eyed",
  //"blue-eyed",
  //"white-eyed",
  "glint-eyed",
  "razor-fanged",
  "yellow-fanged",
  "frothy-mawed",
  "sharp-toothed",
  "long-legged",
  "large-pawed",
  "golden-eyed",
  //"black-eyed",
  //"brown-eyed",
  "keen-eyed",
  "large-pawed",
  "white-pawed",
  "feral",
  "\n"
};

const char *warg_adj1[] = {
  "sinewy",
  "muscular",
  "powerfully-muscled",
  "thickly-muscled",
  "grizzled",
  "feral",
  "frightful",
  "monstrous",
  "beastly",
  "hulking",
  "massive",
  "fierce",
  "thickly-furred",
  "mottle-coated",
  "brambly-furred",
  "coarsely-furred",
  "patchy-furred",
  "ridge-backed",
  "wiry-furred",
  "bristly-furred",
  "spiky-furred",
  "gray-furred",
  "smoky-grey-furred",
  "ash-grey-furred",
  //"black-furred",
  //"ebony-furred",
  "sooty-grey-furred",
  "charcoal-grey-furred",
  //"coal-black-furred",
  //"inky-furred",
  //"ebon-furred",
  //"brown-furred",
  //"muddy-brown-furred",
  //"dark-brown-furred",
  "\n"
};

const char *warg_adj2[] = {
  "scarred",
  "wickedly-scarred",
  "grotesquely-scarred",
  "narrow-muzzled",
  "sharp-muzzled",
  "short-muzzled",
  "long-muzzled",
  "crook-tailed",
  "hook-tailed",
  "stumpy-tailed",
  "tailless",
  "long-necked",
  "sharp-eared",
  "wide-eared",
  "one-eared",
  "red-eyed",
  "narrow-eyed",
  "glassy-eyed",
  //"black-eyed",
  //"grey-eyed",
  //"blue-eyed",
  //"white-eyed",
  "dull-eyed",
  "sallow-eyed",
  //"crimson-eyed",
  "orange-eyed",
  "dark-eyed",
  "one-eyed",
  "glint-eyed",
  "razor-fanged",
  "yellow-fanged",
  "frothy-mawed",
  "sharp-toothed",
  "jagged-toothed",
  "sharp-clawed",
  "yellow-toothed",
  //"blunt-toothed",
  "razor-clawed",
  "long-legged",
  "large-pawed",
  "fiery-eyed",
  "\n"
};

const char *elf_adj1[] = {
  "fair",
  "ethereal",
  "graceful",
  "slender",
  "supple",
  "pale",
  "comely",
  "elegant",
  "aristocratic",
  "limber",
  "trim",
  "airy",
  "alluring",
  "exquisite",
  "radiant",
  "enthralling",
  "entrancing",
  "flawless",
  "bewitching",
  "almond-eyed",
  "aquiline-nosed",
  "hawk-nosed",
  "athletic",
  "delicate",
  "diminutive",
  "lithe",
  "lean",
  "lithe",
  "lissome",
  "lithe",
  "slender",
  "petite",
  "willowy",
  "sinewy",
  "sleek",
  "slight",
  "slender",
  "slim",
  "statuesque",
  "svelte",
  "tall",
  "sylphlike",
  "wiry",
  "dignified",
  "regal",
  "stately",
  "resplendent",
  "luminous",
  "sublime",
  "thin",
  "willowy",
  "wiry",
  "blue-eyed",
  "jade-eyed",
  "sky-blue-eyed",
  "sapphirine-eyed",
  "azure-eyed",
  "glacial-eyed",
  "green-eyed",
  "emerald-eyed",
  "jade-eyed",
  "verdant-eyed",
  "forest-green-eyed",
  "brown-eyed",
  "chocolate-eyed",
  "dark-eyed",
  "amber-eyed",
  "golden-eyed",
  "violet-eyed",
  "amethyst-eyed",
  "grey-eyed",
  "stormy-eyed",
  "silver-eyed",
  "hazel-eyed",
  "cinnamon-eyed",
  "\n"
};

const char *elf_adj2[] = {
  "black-haired",
  "ebony-haired",
  "jet-haired",
  "midnight-haired",
  "onyx-haired",
  "raven-haired",
  "auburn-haired",
  "copper-haired",
  "red-haired",
  "golden-haired",
  "honey-haired",
  "flaxen-haired",
  "tawny-haired",
  "bronze-haired",
  "chestnut-haired",
  "russet-haired",
  "sable-haired",
  "dusky-haired",
  "aristocratic",
  "midnight-blue-haired",
  "fiery-haired",
  "mahogany-haired",
  "silver-haired",
  "platinum-blonde-haired",
  "\n"
};

const char *noldo_adj2[] = {
  "black-haired",
  "ebony-haired",
  "jet-haired",
  "midnight-haired",
  "onyx-haired",
  "raven-haired",
  "mahogany-haired",
  "russet-haired",
  "sable-haired",
  "dusky-haired",
  "\n"
};

const char *avar_adj2[] = {
  "golden-haired",
  "honey-haired",
  "flaxen-haired",
  "tawny-haired",
  "aristocratic",
  "platinum-blonde-haired",
  "\n"
};

const char *human_adj1[] = {
  "acned",
  "cadaverous",
  "dirty",
  "dust-covered",
  "doughy",
  "fair",
  "greasy",
  "jaundiced",
  "pale",
  "livid",
  "pallid",
  "hearty",
  "scarred",
  "sun-browned",
  "swarthy",
  "wan",
  "waxy",
  "weatherbeaten",
  "almond-eyed",
  "beady-eyed",
  "cock-eyed",
  "owlish",
  "rheumy-eyed",
  "squinty-eyed",
  "aquiline-nosed",
  "beak-nosed",
  "bent-nosed",
  "knob-nosed",
  "flat-nosed",
  "hawk-nosed",
  "pig-nosed",
  "pug-nosed",
  "athletic",
  "brawny",
  "bent",
  "bow-spined",
  "burly",
  "chubby",
  "colossal",
  "brawny",
  "delicate",
  "diminutive",
  "lithe",
  "large",
  "thin",
  "fat",
  "fleshy",
  "fragile",
  "gangly",
  "gaunt",
  "haggard",
  "hunched",
  "husky",
  "lanky",
  "lean",
  "lithe",
  "lissome",
  "lissome",
  "lithe",
  "muscled",
  "obese",
  "lanky",
  "paunchy",
  "slender",
  "petite",
  "portly",
  "pot-bellied",
  "pudgy",
  "reedy",
  "rickety",
  "willowy",
  "robust",
  "rotund",
  "rugged",
  "scrawny",
  "runty",
  "sinewy",
  "runty",
  "skeletal",
  "sleek",
  "slight",
  "slender",
  "slim",
  "spindly",
  "squat",
  "stalwart",
  "statuesque",
  "svelte",
  "tall",
  "thickset",
  "thin",
  "waspish",
  "well-muscled",
  "whip-thin",
  "willowy",
  "wiry",
  "blue-eyed",
  "azure-eyed",
  "green-eyed",
  "emerald-eyed",
  "jade-eyed",
  "brown-eyed",
  "chocolate-eyed",
  "dark-eyed",
  "grey-eyed",
  "stormy-eyed",
  "hazel-eyed",
  "\n"
};

const char *human_adj2[] = {
  "black-haired",
  "coal-haired",
  "ebony-haired",
  "jet-haired",
  "midnight-haired",
  "onyx-haired",
  "raven-haired",
  "auburn-haired",
  "copper-haired",
  "red-haired",
  "scarlet-haired",
  "sepia-haired",
  "blonde-haired",
  "golden-haired",
  "ginger-haired",
  "honey-haired",
  "flaxen-haired",
  "sandy-haired",
  "sorrel-haired",
  "tawny-haired",
  "bronze-haired",
  "brown-haired",
  "chestnut-haired",
  "dun-haired",
  "russet-haired",
  "sable-haired",
  "taupe-haired",
  "wheat-haired",
  "henna-haired",
  "dusky-haired",
  "ecru-haired",
  "angular-faced",
  "aristocratic",
  "comely-faced",
  "careworn",
  "cherubic",
  "comely-faced",
  "drawn-faced",
  "feline-faced",
  "narrow-faced",
  "square-faced",
  "stoop-shouldered",
  "broad-shouldered",
  "drooping-shouldered",
  "delicate-shouldered",
  "\n"
};

const char *haladin_adj1[] = {
  "acned",
  "cadaverous",
  "dirty",
  "dust-covered",
  "doughy",
  "greasy",
  "livid",
  "pallid",
  "hearty",
  "scarred",
  "sun-browned",
  "swarthy",
  "dark",
  "dark-complexed",
  "brown-skinned",
  "dark-skinned",
  "wan",
  "waxy",
  "weatherbeaten",
  "beady-eyed",
  "cock-eyed",
  "owlish",
  "rheumy-eyed",
  "squinty-eyed",
  "aquiline-nosed",
  "beak-nosed",
  "bent-nosed",
  "knob-nosed",
  "flat-nosed",
  "hawk-nosed",
  "pig-nosed",
  "pug-nosed",
  "athletic",
  "bent",
  "bow-spined",
  "delicate",
  "diminutive",
  "lithe",
  "thin",
  "fat",
  "fleshy",
  "fragile",
  "gangly",
  "gaunt",
  "haggard",
  "hunched",
  "lanky",
  "lean",
  "lithe",
  "lissome",
  "lanky",
  "paunchy",
  "slender",
  "petite",
  "portly",
  "pot-bellied",
  "pudgy",
  "reedy",
  "rickety",
  "willowy",
  "rugged",
  "scrawny",
  "runty",
  "skeletal",
  "sleek",
  "slight",
  "slender",
  "slim",
  "spindly",
  "squat",
  "svelte",
  "thin",
  "waspish",
  "whip-thin",
  "willowy",
  "wiry",
  "blue-eyed",
  "azure-eyed",
  "green-eyed",
  "emerald-eyed",
  "jade-eyed",
  "brown-eyed",
  "chocolate-eyed",
  "dark-eyed",
  "grey-eyed",
  "stormy-eyed",
  "hazel-eyed",
  "\n"
};

const char *haladin_adj2[] = {
  "black-haired",
  "coal-haired",
  "ebony-haired",
  "jet-haired",
  "midnight-haired",
  "onyx-haired",
  "raven-haired",
  "auburn-haired",
  "copper-haired",
  "red-haired",
  "scarlet-haired",
  "sepia-haired",
  "ginger-haired",
  "bronze-haired",
  "brown-haired",
  "chestnut-haired",
  "dun-haired",
  "russet-haired",
  "sable-haired",
  "henna-haired",
  "dusky-haired",
  "ecru-haired",
  "angular-faced",
  "aristocratic",
  "comely-faced",
  "careworn",
  "cherubic",
  "comely-faced",
  "drawn-faced",
  "narrow-faced",
  "stoop-shouldered",
  "drooping-shouldered",
  "delicate-shouldered",
  "\n"
};

const char *dwarf_adj1[] = {
  "florid-faced",
  "short",
  "compact",
  "hefty",
  "stout",
  "heavily-muscled",
  "heavyset",
  "stocky",
  "burly",
  "hale",
  "hardy-looking",
  "lusty",
  "staunch-looking",
  "strapping",
  "well-built",
  "powerful",
  "sturdy",
  "dour-looking",
  "dirty",
  "dust-covered",
  "doughy",
  "hearty",
  "scarred",
  "sun-browned",
  "swarthy",
  "weatherbeaten",
  "beady-eyed",
  "rheumy-eyed",
  "squinty-eyed",
  "bent-nosed",
  "knob-nosed",
  "flat-nosed",
  "pig-nosed",
  "pug-nosed",
  "athletic",
  "brawny",
  "burly",
  "brawny",
  "large",
  "fleshy",
  "husky",
  "muscled",
  "paunchy",
  "portly",
  "pot-bellied",
  "pudgy",
  "robust",
  "rotund",
  "rugged",
  "squat",
  "stalwart",
  "statuesque",
  "thickset",
  "well-muscled",
  "blue-eyed",
  "green-eyed",
  "jade-eyed",
  "brown-eyed",
  "dark-eyed",
  "grey-eyed",
  "flint-eyed",
  "hazel-eyed",
  "\n"
};

const char *dwarf_adj2[] = {
  "black-haired",
  "coal-haired",
  "ebony-haired",
  "jet-haired",
  "midnight-haired",
  "onyx-haired",
  "raven-haired",
  "auburn-haired",
  "copper-haired",
  "red-haired",
  "scarlet-haired",
  "sepia-haired",
  "blonde-haired",
  "golden-haired",
  "ginger-haired",
  "honey-haired",
  "flaxen-haired",
  "sandy-haired",
  "sorrel-haired",
  "tawny-haired",
  "bronze-haired",
  "brown-haired",
  "chestnut-haired",
  "dun-haired",
  "russet-haired",
  "sable-haired",
  "taupe-haired",
  "wheat-haired",
  "henna-haired",
  "dusky-haired",
  "ecru-haired",
  "comely-faced",
  "careworn",
  "full-bearded",
  "tangle-bearded",
  "braid-bearded",
  "healthy-bearded",
  "glossy-bearded",
  "bristle-bearded",
  "broad-shouldered",
  "\n"
};

const char *orc_adj1[] = {
  "acned",
  "froglike",
  "stumpy",
  "scabbed",
  "balding",
  "bloated",
  "grumpy",
  "meaty",
  "chunky",
  "cadaverous",
  "dirty",
  "filthy",
  "dust-covered",
  "greasy",
  "wan-looking",
  "weatherbeaten",
  "beady-eyed",
  "cock-eyed",
  "rheumy-eyed",
  "squinty-eyed",
  "frog-like",
  "stumpy",
  "scabbed",
  "balding",
  "beak-nosed",
  "pointy-nosed",
  "flat-nosed",
  "pig-nosed",
  "pug-nosed",
  "brawny",
  "bent",
  "bow-spined",
  "burly",
  "chubby",
  "brawny",
  "large",
  "thin",
  "fat",
  "fleshy",
  "gangly",
  "haggard",
  "hunched",
  "lanky",
  "muscled",
  "obese",
  "grossly obese",
  "paunchy",
  "pot-bellied",
  "portly",
  "pudgy",
  "reedy",
  "rickety-looking",
  "scrawny",
  "runty",
  "sinewy",
  "skeletal",
  "sleek",
  "spindly",
  "squat",
  "stalwart",
  "thickset",
  "waspish",
  "cat-eyed",
  "feline-eyed",
  "reptilian-eyed",
  "green-eyed",
  "bright-green-eyed",
  "yellow-eyed",
  "bright-yellow-eyed",
  "milky-eyed",
  "black-eyed",
  "dark-eyed",
  "red-eyed",
  "bright-red-eyed",
  "crimson-eyed",
  "yellow-green-eyed",
  "green-skinned",
  "dark-green-skinned",
  "forest-green-skinned",
  "oily-skinned",
  "gruesomely-scarred",
  "gangly-limbed",
  "black-skinned",
  "dark-skinned",
  "brown-skinned",
  "dark-brown-skinned",
  "leathery-skinned",
  "wrinkled",
  "flabby",
  "potbellied",
  "flat-headed",
  "hollow-eyed",
  "\n"
};

const char *orc_adj2[] = {
  "black-haired",
  "coal-haired",
  "ebony-haired",
  "pumice-haired",
  "jet-haired",
  "onyx-haired",
  "dun-haired",
  "midnight-haired",
  "dark-brown-haired",
  "sorrel-haired",
  "brown-haired",
  "mud-haired",
  "dusky-haired",
  "crimson-haired",
  "dark-red-haired",
  "white-haired",
  "scar-faced",
  "snaggletoothed",
  "leer-faced",
  "sharp-toothed",
  "wrinkle-faced",
  "scraggly-haired",
  "cracked-lipped",
  "large-eared",
  "lopsided-looking",
  "stooped",
  "broad-shouldered",
  "bent-shouldered",
  "narrow-faced",
  "square-faced",
  "flat-faced",
  "nefarious-looking",
  "black-lipped",
  "wickedly scarred",
  "wart-covered",
  "saw-toothed",
  "small-toothed",
  "hunchbacked",
  "triangular-faced",
  "sneer-faced",
  "gap-toothed",
  "brown-toothed",
  "yellow-toothed",
  "wickedly-fanged",
  "small-tusked",
  "\n"
};

char *
return_adj2 (CHAR_DATA * mob)
{
  int roll, limit;
  static char adj[MAX_STRING_LENGTH];

  if (!str_cmp
      (lookup_race_variable (mob->race, RACE_NAME), "Gondorian Human") || !str_cmp(lookup_race_variable (mob->race, RACE_NAME), "Renegade Human"))
    {
      for (limit = 0; *human_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", human_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Orc"))
    {
      for (limit = 0; *orc_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", orc_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Ent"))
    {
      for (limit = 0; *ent_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", ent_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Hill Troll") ||
	   !str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Cave Troll"))
    {
      for (limit = 0; *troll_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", troll_adj2[roll]);
      return adj;
    }
  else if (!str_cmp
	(lookup_race_variable (mob->race, RACE_NAME), "Beorian Human"))
    {
      for (limit = 0; *beor_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", beor_adj2[roll]);
      return adj;
    }
  else if (!str_cmp
	(lookup_race_variable (mob->race, RACE_NAME), "Maracian Human"))
    {
      for (limit = 0; *marach_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", marach_adj2[roll]);
      return adj;
    }
  else if (!str_cmp
	(lookup_race_variable (mob->race, RACE_NAME), "Haladin Human"))
    {
      for (limit = 0; *haladin_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", haladin_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME),
			"Sinda Elf")
	   || !str_cmp (lookup_race_variable (mob->race, RACE_NAME),
			"Vanya Elf"))
    {
      for (limit = 0; *elf_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", elf_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Wolf"))
    {
      for (limit = 0; *wolf_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", wolf_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Wolfspawn"))
    {
      for (limit = 0; *warg_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", warg_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Noldo Elf"))
    {
      for (limit = 0; *noldo_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", noldo_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Avar Elf"))
    {
      for (limit = 0; *avar_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", avar_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Dwarf"))
    {
      for (limit = 0; *dwarf_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", dwarf_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Horse") ||
	   !str_cmp (lookup_race_variable (mob->race, RACE_NAME), "WarHorse"))
    {
      for (limit = 0; *horse_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", horse_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Bird"))
    {
      for (limit = 0; *bird_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", bird_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Rat"))
    {
      for (limit = 0; *rat_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", rat_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Spider"))
    {
      for (limit = 0; *spider_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", spider_adj2[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Giant Spider"))
    {
      for (limit = 0; *spider_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", spider_adj2[roll]);
      return adj;
    }
  else
    if (!str_cmp
	(lookup_race_variable (mob->race, RACE_NAME), "Wraith"))
    {
      for (limit = 0; *wight_adj2[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", wight_adj2[roll]);
      return adj;
    }

  return "fur-covered";
}

char *
return_adj1 (CHAR_DATA * mob)
{
  int roll, limit;
  static char adj[MAX_STRING_LENGTH];

  if (!str_cmp
      (lookup_race_variable (mob->race, RACE_NAME), "Gondorian Human") || !str_cmp(lookup_race_variable (mob->race, RACE_NAME), "Renegade Human"))
    {
      for (limit = 0; *human_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", human_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Orc"))
    {
      for (limit = 0; *orc_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", orc_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Ent"))
    {
      if (mob->delay_info1 == 1)
	{
	  for (limit = 0; *ent_coniferous_adj1[limit] != '\n'; limit++)
	    ;
	  limit--;
	  roll = number (0, limit);
	  sprintf (adj, "%s", ent_coniferous_adj1[roll]);
	}
      else
	{
	  for (limit = 0; *ent_deciduous_adj1[limit] != '\n'; limit++)
	    ;
	  limit--;
	  roll = number (0, limit);
	  sprintf (adj, "%s", ent_deciduous_adj1[roll]);
	}
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Cave Troll") ||
	   !str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Hill Troll"))
    {
      for (limit = 0; *troll_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", troll_adj1[roll]);
      return adj;
    }
  else if (!str_cmp
	(lookup_race_variable (mob->race, RACE_NAME), "Beorian Human"))
    {
      for (limit = 0; *beor_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", beor_adj1[roll]);
      return adj;
    }
  else if (!str_cmp
	(lookup_race_variable (mob->race, RACE_NAME), "Marachian Human"))
    {
      for (limit = 0; *marach_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", marach_adj1[roll]);
      return adj;
    }
  else if (!str_cmp
	(lookup_race_variable (mob->race, RACE_NAME), "Haladin Human"))
    {
      for (limit = 0; *haladin_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", haladin_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Wolf"))
    {
      for (limit = 0; *wolf_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", wolf_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Wolfspawn"))
    {
      for (limit = 0; *warg_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", warg_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Noldo Elf")
	   || !str_cmp (lookup_race_variable (mob->race, RACE_NAME),
			"Sinda Elf")
	   || !str_cmp (lookup_race_variable (mob->race, RACE_NAME),
			"Vanya Elf")
	   || !str_cmp (lookup_race_variable (mob->race, RACE_NAME),
			"Avar Elf"))
    {
      for (limit = 0; *elf_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", elf_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Dwarf"))
    {
      for (limit = 0; *dwarf_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", dwarf_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Bird"))
    {
      for (limit = 0; *bird_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", bird_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Rat"))
    {
      for (limit = 0; *rat_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", rat_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Spider"))
    {
      for (limit = 0; *spider_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", spider_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Giant Spider"))
    {
      for (limit = 0; *spider_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", spider_adj1[roll]);
      return adj;
    }
  else
    if (!str_cmp
	(lookup_race_variable (mob->race, RACE_NAME), "Wraith"))
    {
      for (limit = 0; *wight_adj1[limit] != '\n'; limit++)
	;
      limit--;
      roll = number (0, limit);
      sprintf (adj, "%s", wight_adj1[roll]);
      return adj;
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Horse") ||
	   !str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Warhorse"))
    {
      if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Warhorse"))
	{
	  for (limit = 0; *war_horse_adj1[limit] != '\n'; limit++)
	    ;
	  limit--;
	  roll = number (0, limit);
	  sprintf (adj, "%s", war_horse_adj1[roll]);
	  return adj;
	}
      else
	{
	  for (limit = 0; *horse_adj1[limit] != '\n'; limit++)
	    ;
	  limit--;
	  roll = number (0, limit);
	  sprintf (adj, "%s", horse_adj1[roll]);
	  return adj;
	}
    }

  return "short";
}

char *
return_name (CHAR_DATA * mob)
{
  static char buf[MAX_STRING_LENGTH];
  int roll, roll2;

  *buf = '\0';

  if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Orc"))
    {
    /*if (GET_SEX (mob) == SEX_MALE)
	return "male orc";
      else if (GET_SEX (mob) == SEX_FEMALE)
	return "female orc";
      else*/
	return "orc";
    }
  else
    if (!str_cmp
	(lookup_race_variable (mob->race, RACE_NAME), "Wraith"))
    {
      mob->sex = SEX_NEUTRAL;
      if (!number (0, 4))
        return "shadow";
      else if (!number (0, 3))
        return "spectre";
      else if (!number (0, 2))
        return "fiend";
      else
        return "wraith";
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Noldo Elf")
	   || !str_cmp (lookup_race_variable (mob->race, RACE_NAME),
			"Sinda Elf")
	   || !str_cmp (lookup_race_variable (mob->race, RACE_NAME),
			"Avar Elf")
	   || !str_cmp (lookup_race_variable (mob->race, RACE_NAME),
			"Vanya Elf"))
    {
    /*  if (GET_SEX (mob) == SEX_MALE)
	return "male elf";
      else if (GET_SEX (mob) == SEX_FEMALE)
	return "female elf";
      else*/
	return "elf";
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Dwarf"))
    {
    /*  if (GET_SEX (mob) == SEX_MALE)
	return "male dwarf";
      else if (GET_SEX (mob) == SEX_FEMALE)
	return "female dwarf";
      else*/
	return "dwarf";
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Bird"))
    return "bird";
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Spider"))
    return "spider";
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Giant Spider"))
  {
    roll = number (1, 7);
    if (roll == 1)
      return "giant spider";
    else if (roll == 2)
      return "humongous spider";
    else if (roll == 3)
      return "gigantic spider";
    else if (roll == 4)
      return "immense spider";
    else if (roll == 5)
      return "titantic sider";
    else if (roll == 6)
      return "colossal spider";
    else
      return "tremendous spider";
  }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Rat"))
    return "rat";
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Horse") ||
	   !str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Warhorse"))
    {
      if (GET_SEX (mob) == SEX_MALE)
	{
	  if (!number (0, 4))
	    return "young stallion";
	  return "stallion";
	}
      else if (GET_SEX (mob) == SEX_FEMALE)
	{
	  if (!number (0, 4))
	    return "young mare";
	  else
	    return "mare";
	}
      else
	return "horse";
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Ent"))
    {
      if (!number (0, 1))
	{
	  roll = number (1, 9);
	  mob->delay_info1 = 1;
	  if (roll == 1)
	    return "pine tree";
	  else if (roll == 2)
	    return "red pine tree";
	  else if (roll == 3)
	    return "white spruce tree";
	  else if (roll == 4)
	    return "black spruce tree";
	  else if (roll == 5)
	    return "hemlock tree";
	  else if (roll == 6)
	    return "mountain pine tree";
	  else if (roll == 7)
	    return "yew tree";
	  else if (roll == 8)
	    return "fir tree";
	  else
	    return "larch tree";
	}
      else
	{
	  roll = number (1, 13);
	  if (roll == 1)
	    return "ash tree";
	  else if (roll == 2)
	    return "elm tree";
	  else if (roll == 3)
	    return "oak tree";
	  else if (roll == 4)
	    return "birch tree";
	  else if (roll == 5)
	    return "maple tree";
	  else if (roll == 6)
	    return "alder tree";
	  else if (roll == 7)
	    return "chestnut tree";
	  else if (roll == 8)
	    return "beech tree";
	  else if (roll == 9)
	    return "hickory tree";
	  else if (roll == 10)
	    return "sycamore tree";
	  else if (roll == 11)
	    return "poplar tree";
	  else if (roll == 12)
	    return "aspen tree";
	  else
	    return "willow tree";
	}
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Cave Troll") ||
	   !str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Hill Troll"))
    {
      return "troll";
    }

  roll = number (0, 5);

  if (roll == 0 && !IS_SET (mob->act, ACT_ENFORCER))
    sprintf (buf, "old ");
  else if (roll == 4 && !IS_SET (mob->act, ACT_ENFORCER))
    sprintf (buf, "young ");

  if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Wolf"))
    {
      strcat (buf, "wolf");
      return buf;
    }

  if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Wolfspawn"))
    {
      strcat (buf, "wolf");
      return buf;
    }

  if (IS_SET (mob->act, ACT_ENFORCER))
    {
      if (GET_SEX (mob) == SEX_MALE)
	sprintf (buf, "man");
      else
	sprintf (buf, "woman");
      return buf;
    }

  if (roll == 0 && mob->sex == SEX_MALE)
    {
      roll2 = number (1, 3);
      if (roll2 == 1)
	strcat (buf, "gaffer");
      else if (roll2 == 2)
	strcat (buf, "geezer");
      else if (roll2 == 3)
	strcat (buf, "man");
      else
	strcat (buf, "man");
    }
  else if (roll == 0 && mob->sex == SEX_FEMALE)
    {
      roll2 = number (1, 7);
      if (roll2 == 1)
	strcat (buf, "crone");
      else if (roll2 == 2)
	strcat (buf, "harridan");
      else if (roll2 == 3)
	strcat (buf, "matron");
      else if (roll2 == 4)
	strcat (buf, "spinster");
      else
	strcat (buf, "woman");
    }
  else if (roll == 4 && mob->sex == SEX_MALE)
    {
      roll2 = number (1, 4);
      if (roll2 == 1)
	strcat (buf, "lad");
      else if (roll2 == 2)
	strcat (buf, "waif");
      else
	strcat (buf, "man");
    }
  else if (roll == 4 && mob->sex == SEX_FEMALE)
    {
      roll2 = number (1, 4);
      if (roll2 == 1)
	strcat (buf, "lass");
      else if (roll2 == 2)
	strcat (buf, "maid");
      else
	strcat (buf, "woman");
    }
  else if (mob->sex == SEX_MALE)
    strcat (buf, "man");
  else if (mob->sex == SEX_FEMALE)
    strcat (buf, "woman");
  else
    sprintf (buf + strlen (buf), "person %d", mob->race);

  return buf;
}

/*                                                                          *
 * function: create_description                                             *
 *                                                                          *
 * 09/28/2004 [JWW] - Added travel strings some arbitrary mobs              *
 *                                                                          */
void
create_description (CHAR_DATA * mob)
{
  char sdesc_frame[MAX_STRING_LENGTH];
  char sdesc[MAX_STRING_LENGTH];
  char adj1[MAX_STRING_LENGTH];
  char adj2[MAX_STRING_LENGTH];
  char name[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  bool found = false;
  int roll, i, j;

  for (i = 0; *variable_races[i] != '\n'; i++)
    if (!str_cmp
	(variable_races[i], lookup_race_variable (mob->race, RACE_NAME)))
      found = true;

  if (!found)
    {
      return;
    }
  if (!number (0, 1))
    {
      if (!number (0, 1))
	sprintf (sdesc_frame, "$adj1, $adj2 $name");
      else
	sprintf (sdesc_frame, "$adj2, $adj1 $name");
    }
  else
    {
      if (!number (0, 1))
	sprintf (sdesc_frame, "$adj1 $name");
      else
	sprintf (sdesc_frame, "$adj2 $name");
    }

  *sdesc = '\0';
  *adj1 = '\0';
  *adj2 = '\0';
  *name = '\0';
  *buf2 = '\0';

  sprintf (name, "%s", return_name (mob));

  for (size_t i = 0; i <= strlen (sdesc_frame); i++)
    {
      if (sdesc_frame[i] == '$')
	{
	  j = i;
	  *buf = '\0';
	  while (sdesc_frame[i] && sdesc_frame[i] != ' '
		 && sdesc_frame[i] != ',')
	    {
	      sprintf (buf + strlen (buf), "%c", sdesc_frame[i]);
	      i++;
	    }
	  i = j;
	  if (!str_cmp (buf, "$adj1"))
	    {
	      sprintf (adj1, "%s", return_adj1 (mob));
	      if (!*sdesc
		  && (adj1[0] == 'a' || adj1[0] == 'e' || adj1[0] == 'i'
		      || adj1[0] == 'o' || adj1[0] == 'u'))
		sprintf (sdesc + strlen (sdesc), "an ");
	      else if (!*sdesc)
		sprintf (sdesc + strlen (sdesc), "a ");
	      sprintf (sdesc + strlen (sdesc), "%s", adj1);
	      sprintf (buf2 + strlen (buf2), "%s ", adj1);
	    }
	  else if (!str_cmp (buf, "$adj2"))
	    {
	      sprintf (adj2, "%s", return_adj2 (mob));
	      if (!*sdesc
		  && (adj2[0] == 'a' || adj2[0] == 'e' || adj2[0] == 'i'
		      || adj2[0] == 'o' || adj2[0] == 'u'))
		sprintf (sdesc + strlen (sdesc), "an ");
	      else if (!*sdesc)
		sprintf (sdesc + strlen (sdesc), "a ");
	      while (!str_cmp (adj1, adj2))
		sprintf (adj2, "%s", return_adj2 (mob));
	      sprintf (sdesc + strlen (sdesc), "%s", adj2);
	      sprintf (buf2 + strlen (buf2), "%s ", adj2);
	    }
	  else if (!str_cmp (buf, "$name"))
	    {
	      sprintf (sdesc + strlen (sdesc), "%s", name);
	      sprintf (buf2 + strlen (buf2), "%s", name);
	    }
	  i += strlen (buf) - 1;
	  continue;
	}
      else
	sprintf (sdesc + strlen (sdesc), "%c", sdesc_frame[i]);
    }

  mob->delay_info1 = 0;

  if (mob->short_descr)
    mem_free (mob->short_descr);
  mob->short_descr = add_hash (sdesc);
  *buf = '\0';
  if (IS_SET (mob->act, ACT_ENFORCER) && IS_SET (mob->act, ACT_SENTINEL))
    {
      roll = number (1, 3);
      if (roll == 1)
	sprintf (buf, "%s stands at attention here.", sdesc);
      else if (roll == 2)
	sprintf (buf, "%s stands here, watching for signs of trouble.",
		 sdesc);
      else if (roll == 3)
	sprintf (buf, "%s patrols here, looking hawkishly about.", sdesc);
      mob->travel_str = add_hash ("looking hawkishly about");
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Wolf") ||
           !str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Wolfspawn"))
    {
      roll = number (1, 7);
      if (roll == 1)
	sprintf (buf, "%s prowls through the area.", sdesc);
      else if (roll == 2)
	sprintf (buf, "%s paces here.", sdesc);
      else if (roll == 3)
	sprintf (buf, "%s paces back and forth.", sdesc);
      else if (roll == 4)
	sprintf (buf, "%s pads about here.", sdesc);
      else if (roll == 5)
	{
	  sprintf (buf, "%s pauses here, sniffing the air.", sdesc);
	  mob->travel_str = add_hash ("sniffing the ground");
	}
      else if (roll == 6)
	{
	  sprintf (buf, "%s pads around, sniffing the ground.", sdesc);
	  mob->travel_str = add_hash ("sniffing the ground");
	}
      else if (roll == 7)
	sprintf (buf, "%s watches its surroundings alertly.", sdesc);
    }
  else if (IS_SET (mob->act, ACT_ENFORCER)
	   && !IS_SET (mob->act, ACT_SENTINEL))
    {
      roll = number (1, 3);
      if (roll == 1)
	{
	  sprintf (buf, "%s patrols here, looking for signs of trouble.",
		   sdesc);
	  mob->travel_str = add_hash ("looking about purposefully");
	}
      else if (roll == 2)
	{
	  sprintf (buf, "%s moves by, watching the area attentively.", sdesc);
	  mob->travel_str = add_hash ("looking about the area attentively");
	}
      else if (roll == 3)
	{
	  sprintf (buf, "%s strides through, watching intently.", sdesc);
	  mob->travel_str = add_hash ("looking hawkishly about");
	}
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Horse") ||
	   !str_cmp (lookup_race_variable (mob->race, RACE_NAME), "WarHorse"))
    {
      roll = number (1, 5);
      if (roll == 1)
	sprintf (buf, "%s stands here, stamping the ground.", sdesc);
      else if (roll == 2)
	sprintf (buf, "%s is here, whickering softly.", sdesc);
      else if (roll == 3)
	sprintf (buf, "%s stands here quietly.", sdesc);
      else if (roll == 4)
	sprintf (buf, "%s is here, flicking its tail.", sdesc);
      else if (roll == 5)
	sprintf (buf, "%s stands here calmly.", sdesc);
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Ent"))
    {
      roll = number (1, 4);
      if (roll == 1)
	sprintf (buf, "%s has taken root here.", sdesc);
      else if (roll == 2)
	sprintf (buf, "%s towers here.", sdesc);
      else if (roll == 3)
	sprintf (buf, "%s grows here.", sdesc);
      else if (roll == 4)
	sprintf (buf, "%s stands here.", sdesc);
    }
  else
    if (!str_cmp
	(lookup_race_variable (mob->race, RACE_NAME), "Wraith"))
    {
      roll = number (1, 4);
      if (roll == 1)
	sprintf (buf, "%s is here, looming malevolently.", sdesc);
      else if (roll == 2)
	sprintf (buf, "%s stands here in eerie silence.", sdesc);
      else if (roll == 3)
	sprintf (buf, "%s lurks from the shadows.", sdesc);
      else if (roll == 4)
	sprintf (buf, "%s bathes the area in an unnatural chill.", sdesc);
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Bird"))
    {
      roll = number (1, 4);
      if (roll == 1)
	sprintf (buf, "%s perches here.", sdesc);
      else if (roll == 2)
	sprintf (buf, "%s perches here, observing quietly.", sdesc);
      else if (roll == 3)
	sprintf (buf, "%s flies through the area.", sdesc);
      else if (roll == 4)
	sprintf (buf, "%s is here, watching in silence.", sdesc);
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Rat"))
    {
      roll = number (1, 7);
      if (roll == 1)
	{
	  sprintf (buf, "%s skitters around the area.", sdesc);
	  mob->travel_str = add_hash ("skittering about nervously");
	}
      else if (roll == 2)
	sprintf (buf, "%s is here, skulking about.", sdesc);
      else if (roll == 3)
	sprintf (buf, "%s moves quietly by.", sdesc);
      else if (roll == 4)
	sprintf (buf, "%s waits here, unmoving.", sdesc);
      else if (roll == 5)
	sprintf (buf, "%s is here.", sdesc);
      else if (roll == 6)
	sprintf (buf, "%s lies low to the ground here.", sdesc);
      else if (roll == 7)
	{
	  sprintf (buf, "%s sneaks about quietly.", sdesc);
	  mob->travel_str = add_hash ("padding along quietly in the shadows");
	}
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Giant Spider"))
    {
      roll = number (1, 7);
      if (roll == 1)
	sprintf (buf, "%s crawls about silently.", sdesc);
      else if (roll == 2)
	sprintf (buf, "%s moves slowly.", sdesc);
      else if (roll == 3)
	sprintf (buf, "%s crouches low to the ground.", sdesc);
      else if (roll == 4)
	sprintf (buf, "%s stands here, clacking its mandibles.", sdesc);
      else if (roll == 5)
	sprintf (buf, "%s is here.", sdesc);
      else if (roll == 6)
	sprintf (buf, "%s moves across the rough ground.", sdesc);
      else if (roll == 7)
	sprintf (buf, "%s sits here, unmoving.", sdesc);
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Cave Troll") ||
	   !str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Hill Troll"))
    {
      roll = number (1, 5);
      if (roll == 1)
	sprintf (buf, "%s looms here menacingly.", sdesc);
      else if (roll == 2)
	sprintf (buf, "%s towers here, snarling.", sdesc);
      else if (roll == 3)
	sprintf (buf, "%s is here, growling.", sdesc);
      else if (roll == 4)
	sprintf (buf, "%s stands here, glaring angrily.", sdesc);
      else if (roll == 5)
	sprintf (buf, "%s is here, glaring at passersby.", sdesc);
    }
  else if (!str_cmp (lookup_race_variable (mob->race, RACE_NAME), "Orc"))
    {
      roll = number (1, 14);
      if (roll == 1)
	sprintf (buf, "%s stands here menacingly.", sdesc);
      else if (roll == 2)
	sprintf (buf, "%s towers here, snarling.", sdesc);
      else if (roll == 3)
	sprintf (buf, "%s is here, growling.", sdesc);
      else if (roll == 4)
	sprintf (buf, "%s stands here, glaring angrily.", sdesc);
      else if (roll == 5)
	sprintf (buf, "%s is here, glaring at passersby.", sdesc);
      else if (roll == 6)
	sprintf (buf, "%s loiters here sullenly.", sdesc);
      else if (roll == 7)
	sprintf (buf, "%s sulks here, glancing about.", sdesc);
      else if (roll == 8)
	{
	  sprintf (buf, "%s is here, glancing skittishly about.", sdesc);
	  mob->travel_str = add_hash ("padding along apprehensively");
	}
      else if (roll == 9)
	sprintf (buf, "%s stands here.", sdesc);
      else if (roll == 10)
	sprintf (buf, "%s is here.", sdesc);
      else if (roll == 11)
	sprintf (buf, "%s stalks through the area.", sdesc);
      else if (roll == 12)
	sprintf (buf, "%s scampers through the area.", sdesc);
      else if (roll == 13)
	{
	  sprintf (buf, "%s storms angrily by.", sdesc);
	  mob->travel_str = add_hash ("angrily glaring about");
	}
      else if (roll == 14)
	sprintf (buf, "%s skitters by.", sdesc);

    }
  else if (!str_cmp(lookup_race_variable (mob->race, RACE_NAME), "Beorian Human") 
        || !str_cmp(lookup_race_variable (mob->race, RACE_NAME), "Marachian Human")
        || !str_cmp(lookup_race_variable (mob->race, RACE_NAME), "Haladin Human"))
    {
      roll = number (1, 9);
      if (roll == 1)
	{
	  sprintf (buf, "%s is here, gazing about.", sdesc);
	  mob->travel_str = add_hash ("gazing about absently");
	}
      else if (roll == 2)
	sprintf (buf, "%s wanders through the area.", sdesc);
      else if (roll == 3)
	{
	  sprintf (buf, "%s passes through, looking about.", sdesc);
	  mob->travel_str = add_hash ("looking about absently");
	}
      else if (roll == 4)
	{
	  sprintf (buf, "%s moves by, lost in thought.", sdesc);
	  mob->travel_str = add_hash ("lost in thought");
	}
      else if (roll == 5)
	{
	  sprintf (buf, "%s strides purposefully through the area.", sdesc);
	  mob->travel_str = add_hash ("looking ahead purposefully");
	}
      else if (roll == 6)
	sprintf (buf, "%s is here.", sdesc);
      else if (roll == 7)
	sprintf (buf, "%s stands here.", sdesc);
      else if (roll == 8)
	sprintf (buf, "%s lingers here.", sdesc);
      else if (roll == 9)
	sprintf (buf, "%s loiters here.", sdesc);
    }

  if (!*buf)
    sprintf (buf, "%s is here.", sdesc);

  *buf = toupper (*buf);

  if (mob->long_descr)
    mem_free (mob->long_descr);
  mob->long_descr = add_hash (buf);

  if (mob->name)
    mem_free (mob->name);
  mob->name = add_hash (buf2);
}

/**
*  type 0 is normal racial defaults
*  type 1 will be slightly better stats +10%
*  type 2 will be elite with even better stats +50%
**/
void
randomize_mobile (CHAR_DATA * mob)
{
  CHAR_DATA *proto;
  int attr_starters[] = { 16, 15, 12, 12, 11, 10, 8 };
  int attr_priorities[] = { -1, -1, -1, -1, -1, -1, 1 };
  int slots_taken[] = { 0, 0, 0, 0, 0, 0, 0 };
  int i, roll, bonus;
	int type_bonus = 0;

	if (is_name_in_list("elite", mob->short_descr))
		type_bonus = 30;

	else if (is_name_in_list("regular", mob->short_descr))
		type_bonus = 10;

	else if (is_name_in_list("veteran", mob->short_descr))
		type_bonus = 20;

  if (mob->race >= 0 && mob->race <= 11)
    {
      for (i = 0; i <= 6; i++)
	{
	  roll = number (0, 6);
	  if (slots_taken[roll])
	    {
	      i--;
	      continue;
	    }

	  slots_taken[roll] = 1;
	  attr_priorities[i] = roll;
	}

      for (bonus = 8; bonus;)
	{
	  roll = number (0, 6);
	  if (attr_starters[attr_priorities[roll]] < 18)
	    {
	      attr_starters[attr_priorities[roll]]++;
	      bonus--;
	    }
	}

      mob->str = attr_starters[attr_priorities[0]];
      mob->dex = attr_starters[attr_priorities[1]];
      mob->con = attr_starters[attr_priorities[2]];
      mob->wil = attr_starters[attr_priorities[3]];
      mob->intel = attr_starters[attr_priorities[4]];
      mob->aur = attr_starters[attr_priorities[5]];
      mob->agi = attr_starters[attr_priorities[6]];

      mob->tmp_str = mob->str;
      mob->tmp_dex = mob->dex;
      mob->tmp_intel = mob->intel;
      mob->tmp_aur = mob->aur;
      mob->tmp_agi = mob->agi;
      mob->tmp_con = mob->con;
      mob->tmp_wil = mob->wil;

      for (i = 1; i <= LAST_SKILL; i++)
	mob->skills[i] = 0;

      if (IS_SET (mob->act, ACT_ENFORCER))
        mob->skills[SKILL_RIDE] = 10 + number (5, 15) + type_bonus;

      make_height (mob);
      make_frame (mob);

      for (i = 1; i <= 16; i++) //weapon skills
        mob->skills[i] = number (20, 30) + type_bonus;

      mob->skills[SKILL_AVERT] = number (20, 30) + type_bonus;
      mob->skills[SKILL_DODGE] = number (20, 30) + type_bonus;

      if (is_name_in_list("sneaky", mob->short_descr))
      {
  	mob->skills[SKILL_SNEAK] = number (20, 30) + type_bonus;
	mob->skills[SKILL_HIDE] = number (20, 30) + type_bonus;
      }

      for (i = 1; i <= LAST_SKILL; i++)
	{
	  if (mob->skills[i] > calc_lookup (mob, REG_CAP, i))
	    mob->skills[i] = calc_lookup (mob, REG_CAP, i);
	  if (mob->skills[i] < 0)
	    mob->skills[i] = number (1, 10);
	}

      if (mob->mob)
	proto = vtom (mob->mob->nVirtual);
      else
	proto = vtom (998);

      if (lookup_race_variable (mob->race, RACE_NATIVE_TONGUE))
	{
	  mob->speaks =
	    atoi (lookup_race_variable (mob->race, RACE_NATIVE_TONGUE));
			
// Sindar elves might know some Haladin, Quenya elves some Taliska, men some Sindarin.

	  switch (mob->speaks)
	    {
	    case SKILL_SPEAK_QUENYA:
	      mob->skills[SKILL_SPEAK_TALISKA] =
		number (20,
			MIN (50,
			     calc_lookup (mob, REG_CAP,
					  SKILL_SPEAK_TALISKA)));
	      mob->skills[SKILL_SPEAK_SINDARIN] =
		number (40,
			MIN (65,
			     calc_lookup (mob, REG_CAP,
					  SKILL_SPEAK_SINDARIN)));
					break;
	    case SKILL_SPEAK_SINDARIN:
	      mob->skills[SKILL_SPEAK_HALADIN] =
		number (20,
			MIN (50,
			     calc_lookup (mob, REG_CAP,
					  SKILL_SPEAK_HALADIN)));
	      mob->skills[SKILL_SPEAK_QUENYA] =
		number (20,
			MIN (40,
			     calc_lookup (mob, REG_CAP,
					  SKILL_SPEAK_QUENYA)));
					break;
	    case SKILL_SPEAK_HALADIN:
	    case SKILL_SPEAK_TALISKA:
	      mob->skills[SKILL_SPEAK_SINDARIN] =
		number (30,
			MIN (50,
			     calc_lookup (mob, REG_CAP,
					  SKILL_SPEAK_SINDARIN)));
					break;
					
				default:
					break;
				}

	  mob->skills[mob->speaks] = calc_lookup (mob, REG_CAP, mob->speaks);
	}

      for (i = 1; i <= LAST_SKILL; i++)
			{
	proto->skills[i] = mob->skills[i];
			}

      proto->speaks = mob->speaks;

      if (mob->pc)
	{
	  for (i = 1; i <= LAST_SKILL; i++)
				{
	    mob->pc->skills[i] = mob->skills[i];
				}
	}

      fix_offense (mob);
      fix_offense (proto);
		}  //if (mob->race >= 0 && mob->race <= 11)
  else
    {
      make_height (mob);
      make_frame (mob);
    }

  mob->sex = number (1, 2);
  if (IS_SET (mob->act, ACT_ENFORCER))
    {
      roll = number (1, 10);
      if (roll == 10)
	mob->sex = SEX_FEMALE;
      else
	mob->sex = SEX_MALE;
    }

  mob->max_move = calc_lookup (mob, REG_MISC, MISC_MAX_MOVE);
  mob->move_points = mob->max_move;

  if (IS_SET (mob->flags, FLAG_VARIABLE) || mob->pc)
    {
      create_description (mob);
      switch (number (1, 5))
	{
	case 1:
	  mob->speed = SPEED_CRAWL;
	  break;
	case 2:
	  mob->speed = SPEED_PACED;
	  break;
	default:
	  mob->speed = SPEED_WALK;
	  break;
	}
    }

  switch (number(1,3))
  {
	  case 1:
	    mob->fight_mode = 1;
	    break;
	  case 2:
	    mob->fight_mode = 2;
	    break;
	  case 3:
	    mob->fight_mode = 3;
	    break;
  }
}
