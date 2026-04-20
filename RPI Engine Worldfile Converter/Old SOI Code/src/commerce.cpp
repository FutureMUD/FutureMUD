/*------------------------------------------------------------------------\
|  commerce.c : shopkeeper commerce routines          www.middle-earth.us |
|  Copyright (C) 2005, Shadows of Isildur: Traithe                        |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/

#include <string.h>
#include <time.h>
#include <stdlib.h>
#include <ctype.h>
#include <errno.h>
#include <math.h>
#include <mysql/mysql.h>
#include <unistd.h>
#include <sys/stat.h>

#include "server.h"
#include "structs.h"
#include "protos.h"
#include "utils.h"
#include "group.h"
#include "utility.h"

#define VNPC_COPPER_PURSE		200
#define MAX_INV_COUNT			60

extern rpie::server engine;

const char *standard_object_colors[] = {
  "black",
  "white",
  "grey",
  "red",
  "orange",
  "yellow",
  "green",
  "blue",
  "indigo",
  "purple",
  "brown",
  "\n"
};

const char *drab_object_colors[] = {
  "faded black",
  "bruise black",
  "dim black",
  "dingy black",
  "dull black",
  "greyish black",
  "grimy black",
  "stained black",
  "blotchy black",
  "dingy white",
  "off white",
  "dingy bone white",
  "blotchy bone white",
  "stained bone white",
  "grimy bone white",
  "stained white",
  "grimy white",
  "blotchy white",
  "blotchy ivory colored",
  "dingy ivory colored",
  "stained ivory colored",
  "grimy ivory colored",
  "bland beige",
  "dingy beige",
  "dreary beige",
  "grimy beige",
  "oatmeal colored",
  "blotchy beige",
  "stained beige",
  "drab beige",
  "faded grey",
  "dull grey",
  "dim grey",
  "grimy grey",
  "dusky grey",
  "ashen grey",
  "chalky grey",
  "dingy grey",
  "blotchy grey",
  "stained grey",
  "dreary grey",
  "pallid grey",
  "faded slate grey",
  "grimy slate grey",
  "blotchy slate grey",
  "dingy slate grey",
  "stained slate grey",
  "faded pink",
  "dingy pink",
  "grimy pink",
  "blotchy pink",
  "stained pink",
  "gaudy pink",
  "faded red",
  "dingy red",
  "stained red",
  "grimy red",
  "blotchy red",
  "dim red",
  "faded rust red",
  "grimy rust red",
  "dingy rust red",
  "dim rust red",
  "stained rust red",
  "blotchy rust red",
  "dull rust red",
  "reddish",
  "faded reddish",
  "stained reddish",
  "grimy reddish",
  "blotchy reddish",
  "dull red",
  "dull reddish",
  "faded reddish orange",
  "dingy reddish orange",
  "stained reddish orange",
  "blotchy reddish orange",
  "grimy reddish orange",
  "gaudy reddish orange",
  "faded orange",
  "dull orange",
  "grimy orange",
  "blotchy orange",
  "stained orange",
  "gaudy orange",
  "salmon colored",
  "dingy salmon colored",
  "grimy salmon colored",
  "faded yellow",
  "grimy yellow",
  "dingy yellow",
  "sickly yellow",
  "mustard yellow",
  "lurid yellow",
  "bland yellow",
  "blotchy yellow",
  "stained yellow",
  "gaudy yellow",
  "dingy mustard yellow",
  "grimy mustard yellow",
  "blotchy mustard yellow",
  "gaudy mustard yellow",
  "stained mustard yellow",
  "sallow colored",
  "faded brown",
  "drab brown",
  "dull brown",
  "dingy brown",
  "stained brown",
  "blotchy brown",
  "rust brown",
  "dust brown",
  "dreary brown",
  "muddy brown",
  "mucky brown",
  "dirty brown",
  "faded green",
  "blotchy green",
  "stained green",
  "dull green",
  "sickly green",
  "dingy green",
  "greyish green",
  "dingy greyish green",
  "dim greyish green",
  "stained greyish green",
  "blotchy greyish green",
  "dreary greyish green",
  "faded greyish green",
  "dull greyish green",
  "murky greyish green",
  "mucky green",
  "scummy green",
  "brownish green",
  "faded olive green",
  "stained olive green",
  "dingy olive green",
  "blotchy olive green",
  "dim olive green",
  "murky olive green",
  "dull olive green",
  "grimy olive green",
  "stained olive green",
  "dreary olive green",
  "mucky olive green",
  "faded blue",
  "dull blue",
  "pallid blue",
  "dingy blue",
  "grimy blue",
  "blotchy blue",
  "dim blue",
  "stained blue",
  "dreary blue",
  "blue black",
  "faded blue black",
  "dreary blue black",
  "dim blue black",
  "stained blue black",
  "dingy blue black",
  "stained blue black",
  "dull blue black",
  "blotchy blue black",
  "faded purple",
  "dull purple",
  "blotchy purple",
  "stained purple",
  "dingy purple",
  "dim purple",
  "faded lavender",
  "dingy lavender",
  "stained lavender",
  "blotchy lavender",
  "dull lavender",
  "dim lavender",
  "grimy lavender",
  "\n"
};

const char *fine_object_colors[] = {
  "ebon black",
  "onyx black",
  "obsidian colored",
  "night black",
  "midnight black",
  "ink black",
  "jet black",
  "soot black",
  "coal black",
  "sable colored",
  "pitch black",
  "ivory colored",
  "ecru colored",
  "snow white",
  "gleaming white",
  "pure white",
  "milk white",
  "frost white",
  "chalk white",
  "pearl white",
  "lily white",
  "bright white",
  "bone white",
  "ghost white",
  "mist grey",
  "charcoal grey",
  "thistle grey",
  "smoky grey",
  "slate grey",
  "sooty grey",
  "storm grey",
  "shadowy grey",
  "iron grey",
  "steel grey",
  "pale grey",
  "lead grey",
  "platinum grey",
  "dark grey",
  "ash grey",
  "silver grey",
  "soft grey",
  "argent grey",
  "apple red",
  "flame red",
  "wine red",
  "crimson",
  "scarlet",
  "ruby red",
  "blood red",
  "rose red",
  "garnet red",
  "bright red",
  "burgundy colored",
  "magenta hued",
  "brick red",
  "rust red",
  "cherry red",
  "rosy pink",
  "coral pink",
  "deep pink",
  "vermillion red",
  "pale pink",
  "reddish-orange",
  "gold colored",
  "bright orange",
  "yellow-orange",
  "vivid orange",
  "copper colored",
  "fiery orange",
  "ocher colored",
  "sunset orange",
  "amber colored",
  "straw yellow",
  "tawny hued",
  "light yellow",
  "brilliant yellow",
  "pale yellow",
  "golden yellow",
  "sand yellow",
  "lemon yellow",
  "sunny yellow",
  "bright yellow",
  "topaz-hued",
  "spring green",
  "silvery green",
  "sea green",
  "hunter green",
  "olive green",
  "sage green",
  "pine green",
  "bright green",
  "rich green",
  "pale green",
  "emerald green",
  "dark green",
  "verdant green",
  "forest green",
  "pea green",
  "leaf green",
  "seaweed green",
  "moss green",
  "grass green",
  "lime green",
  "apple green",
  "fir green",
  "jade green",
  "chartreuse",
  "aquamarine",
  "slate blue",
  "bright blue",
  "powder blue",
  "turquoise",
  "pale blue",
  "light blue",
  "sapphire blue",
  "ice blue",
  "sky blue",
  "silvery blue",
  "watery blue",
  "dark blue",
  "deep blue",
  "royal blue",
  "ocean blue",
  "teal",
  "azure",
  "beryl colored",
  "cerulean blue",
  "viridian blue",
  "cobalt blue",
  "midnight blue",
  "rich indigo",
  "deep indigo",
  "vivid indigo",
  "earthen brown",
  "deep brown",
  "rich brown",
  "sienna hued",
  "chocolate brown",
  "cinnamon colored",
  "mahogany colored",
  "light brown",
  "umber hued",
  "sandy brown",
  "amethyst colored",
  "deep purple",
  "rich purple",
  "mauve colored",
  "mulberry colored",
  "orchid purple",
  "plum colored",
  "lavender colored",
  "royal purple",
  "violet",
  "\n"
};

const char *gem_colors[] = {
  "aquamarine",
  "topaz",
  "citrine",
  "amethyst",
  "rose quartz",
  "quartz",
  "blue topaz",
  "agate",
  "peridot",
  "amber",
  "moonstone",
  "garnet",
  "sardonyx",
  "smoky quartz",
  "carnelian",
  "moss agate",
  "blue aventurine",
  "\n"
};


const char *blade_type_colors[]= {
  "jagged",
  "serrated",
  "thin",
  "narrow",
  "thick",
  "broad",
  "curved",
  "straight",
  "\n"
};

const char *orc_colors[] = {
  "severed ears",
  "severed fingers",
  "a severed hand",
  "skeletal hands",
  "fangs and teeth",
  "a shrunken, skin-bound head",
  "a jawless skull",
  "a yellowed skull",
  "a fractured skull",
  "bloodied hunks of fur and skin",
  "rat tails",
  "rodent skeletons",
  "ragged wolf paws", 
  "dessicated serpent remains",
  "charms of bronze and iron",
  "bloodied arrowheads",
  "dried blood and chunks of gore",
  "a rotting humanoid arm",
  "a decaying wolf head",
  "shriveled eyeballs",
  "strips of flayed skin",
  "animal jawbones",
  "rotting fishheads",
  "\n"
};

const char *animal_colors[] = {
  "falcon",
  "hawk",
  "eagle",
  "dragon",
  "wolf",
  "bear",
  "buck",
  "hare",
  "fox",
  "hound",
  "stoat",
  "marmot",
  "weasel",
  "otter",
  "badger",
  "boar",
  "bird",
  "spider",
  "doe",
  "deer",
  "dog",
  "cat",
  "mouse",
  "\n"
};


const char *design_type_colors[] = {
// bodyparts

  "clenched fist",
  "open palm",
  "eye",
  "closed eye",
  "staked head",
  "skull",
  "grinning skull",
  "splintered skull",
  "staked head",
  "brawny arm",

// geography/nature

  "tree",
  "skeletal tree",
  "mountain",
  "river",
  "field",
  "lightning bolt",
  "mountain peak",
  "fire",
  "raging inferno",

// arsenal
  "pair of crossed swords",
  "sword",
  "broken sword",
  "shield",
  "helmet",
  "plumed helm",
  "spear",
  "dagger",
  "chipped dagger",
  "axe",
  "notched axe",
  "whip",
  "arrow",
  "bow",

// animals

  "falcon",
  "hawk",
  "swooping eagle",
  "soaring eagle",
  "eagle",
  "roaring dragon",
  "sleeping dragon",
  "dragon",
  "wolf",
  "howling wolf",
  "bear",
  "buck",
  "hare",
  "fox",
  "hound",
  "stoat",
  "marmot",
  "weasel",
  "otter",
  "badger",
  "boar",
  "bird",
  "spider",
  "doe",
  "deer",
  "dog",
  "cat",
  "mouse",

// structures

  "town",
  "city",
  "tower",
  "fort",
  "keep",
  "castle",
  "wall",
  "\n"
};

const char *light_object_colors[] = {
  "light grey",
  "light red",
  "light orange",
  "light yellow",
  "light green",
  "light blue",
  "light purple",
  "light brown",
  "\n"
};

const char *stone_colors[] = {
  "granite",
  "shale",
  "flint",
  "basalt",
  "slate",
  "\n"
};

const char *war_art[] = {
  "a hunter firing an arrow at a buck",
  "a hunter hurling a spear at a buck",
  "a man armed with a spear lurking through tree branches",
  "two dogs chasing a doe through a field",
  "a hound chasing after a hare",
  "a herd of deer prancing across a glade",
  "a man on horseback pursuing a boar",
  "several men with spears fighting an enraged bear",
  "men chasing various animals through the woods",
  "a pack of wolves circling an armed man",
  "a man and orc battling one another",
  "a number of men with spears marching down a trail",
  "a spear-brandishing man standing atop a dead orc",
  "men armed with bows firing arrows at a fort",
  "a sword-wielding man fighting a pair of orcs",
  "a semi-circle of armed men sitting about a campfire",
  "two armed men on galloping horses",
  "a man and an elf fighting a number of orcs",
  "a woman mourning a dead soldier",
  "a field filled with slain orcs",
  "men fighting a gigantic, fire-breathing beast",
  "men engaged in a pitched battle",
  "an archer firing his bow",
  "a group of spearmen ready for battle",
  "\n"
};



const char *beor_art[] = {
  //hunting
  "a hunter firing an arrow at a buck",
  "a hunter hurling a spear at a buck",
  "a man armed with a spear lurking through tree branches",
  "two dogs chasing a doe through a field",
  "a hound chasing after a hare",
  "a herd of deer prancing across a glade",
  "a man on horseback pursuing a boar",
  "several men with spears fighting an enraged bear",
  "men chasing various animals through the woods",
  "a pack of wolves circling an armed man",

  // warfare
  "a man and orc battling one another",
  "a number of men with spears marching down a trail",
  "a spear-brandishing man standing atop a dead orc",
  "men armed with bows firing arrows at a fort",
  "a sword-wielding man fighting a pair of orcs",
  "a semi-circle of armed men sitting about a campfire",
  "two armed men on galloping horses",
  "a man and an elf fighting a number of orcs",
  "a woman mourning a dead soldier",
  "a field filled with slain orcs",

  // peasant life
  "men tilling barley fields",
  "a shepard leading a flock",
  "women threshing grain",
  "farmers working alongside a river",
  "a group of women brewing",
  "children running through an orchird",
  "a flock of goats spread across a pasture",
  "fields of barley swaying in the wind",
  "men and women at work in the fields",
  "winter snows falling upon a cottage",

  // history
  "a man and an elf shaking hands",
  "a mass of people walking through a mountain pass",
  "a group of men descending in to a shallow valley",
  "men fighting a gigantic, fire-breathing beast",
  "frightened men fleeing fiendish ghouls",
  "three groups of men departing seperate ways",
  "an elf tutoring a group of men",
  "men and women at work making a log fort",
  "a man turning his back upon a sinister fiend",
  "\n"
};

const char *dark_object_colors[] = {
  "dark grey",
  "dark red",
  "dark orange",
  "dark yellow",
  "dark green",
  "dark blue",
  "dark purple",
  "dark brown",
  "\n"
};

const char *hardwood_colors[] = {
  "pine",
  "oak",
  "alder",
  "yew",
  "larch",
  "rowan",
  "willow",
  "ash",
  "beech",
  "sycamore",
  "hazel",
  "maple",
  "elm",
  "fir",
  "\n"
};

const char *metal_damage_colors[] = {
  "notched",
  "rusted",
  "chipped",
  "scratched",
  "nicked",
  "cracked",
  "corroded",
  "splintered",
  "pocked",
  "discoloured",
  "marred",
  "\n"
};


const char *fine_gem_colors[] = {
  "jet",
  "ruby",
  "diamond",
  "sapphire",
  "opal",
  "emerald",
  "lapis lazuli",
  "onyx",
  "pearl",
  "sunstone",
  "turquoise",
  "jade",
  "malachite",
  "\n"
};
const char *sizes[] = {
  "Sizeless",
  "XXS",
  "XS",
  "S",
  "M",
  "L",
  "XL",
  "XXL",
  "\n"
};

const char *sizes_named[] = {
  "\01",			/* Binary 1 (^A) should be hard to enter for players */
  "XX-Small",
  "X-Small",
  "Small",
  "Medium",
  "Large",
  "X-Large",
  "XX-Large",
  "\n"
};


const char *econ_flags[] = {
  "sinda",			// 1 << 0
  "noldo",			// 1 << 1
  "orkish",			// 1 << 2
  "dwarvish",			// 1 << 3
  "beorian",			// 1 << 4
  "marachain",			// 1 << 5
  "haladin",			// 1 << 6
  "generic",			// 1 << 7
  "junk",			// 1 << 8
  "fine",			// 1 << 9
  "poor",			// 1 << 10
  "raw",			// 1 << 11
  "cooked",			// 1 << 12
  "admin",			// 1 << 13
  "BUG",			// 1 << 14
  "practice",			// 1 << 15
  "used",			// 1 << 16
  "admin",			// 1 << 17
  "nobarter",                   // 1 << 18
  "\n"
};

// Text of resource types.
// IMPORTANT - must have the same amount of entries in it as does econ flags,
// because the resource system piggy-backs off of it. Got to love these shortcuts! 

const char *resources[] = {
  "coal",
  "iron",
  "copper",
  "gold",
  "silver",
  "salt",
  "wheat",
  "barley",
  "wool",
  "flax",
  "linen",
  "silk",
  "FILLER",
  "FILLER",
  "FILLER",
  "FILLER",
  "FILLER",
  "FILLER",
  "nobarter",
  "\n",
};

/*
Adding a new econ zone?  Remember to update zone_to_econ_zone ()
*/

const struct econ_data default_econ_info[] = {
/* sold from     sold to -->
      V            minas tirith   osgiliath     gondor     minas morgul   foreign */
  {"minas tirith",
   {{1.00, 0.50}, {1.25, 0.75}, {1.40, 0.90}, {2.25, 1.75}, {1.60, 1.25}}},
  {"osgiliath",
   {{1.00, 0.75}, {1.00, 0.50}, {1.20, 0.80}, {2.00, 1.50}, {1.40, 0.95}}},
  {"gondor",
   {{1.10, 0.85}, {1.20, 0.95}, {1.00, 0.50}, {2.10, 1.65}, {1.50, 1.05}}},
  {"minas morgul",
   {{0.50, 0.25}, {0.75, 0.50}, {0.60, 0.40}, {1.00, 0.50}, {0.70, 0.45}}},
  {"foreign",
   {{1.10, 0.85}, {1.20, 0.95}, {1.25, 1.00}, {1.60, 1.25}, {1.00, 0.50}}},
  {"\n", {{1.00, 0.50}, {1.00, 0.50}, {1.00, 0.50}, {1.00, 0.50}, {1.00, 0.50}}}
};


// Returns true if namestring contains new material name-tag.

int
is_tagged (char *name_str)
{
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  sprintf (buf, "%s", name_str);

  for (int i = 0; *materials[i] != '\n'; i++)
    {
      sprintf (buf2, "%s", materials[i]);
      for (size_t j = 0; j <= strlen (buf2); j++)
	buf2[j] = toupper (buf2[j]);
      if (isnamec (buf2, buf))
	return 1;
    }

  return 0;
}

// Function to help ease the pain of revamping the obj database with materials flags.

void
do_classify (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char buf3[MAX_STRING_LENGTH];
  OBJ_DATA *obj;
  int updated = 0;
  bool update = false;

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Usage: classify <object ##> <material flag>\n", ch);
      return;
    }

  if (!str_cmp (buf, "all"))
    {
      if (str_cmp (ch->tname, "Traithe"))
	{
	  send_to_char ("Get Traithe to do this.\n", ch);
	  return;
	}

      for (int i = 0; *materials[i] != '\n'; i++)
	{

	  sprintf (buf2, "%s", materials[i]);
	  for (size_t j = 0; j <= strlen (buf2); j++)
	    buf2[j] = toupper (buf2[j]);

	  for (obj = full_object_list; obj; obj = obj->lnext)
	    {

	      if (is_tagged (obj->name))
		continue;

	      if (!str_cmp (buf2, "TEXTILE") && !isnamec (obj->name, buf2))
		{
		  if (isname ("silk", obj->name) || isname ("wool", obj->name)
		      || isname ("woolen", obj->name)
		      || isname ("linen", obj->name)
		      || isname ("cloth", obj->name))
		    update = true;
		}
	      else if (!str_cmp (buf2, "LEATHER")
		       && !isnamec (obj->name, buf2))
		{
		  if (isname ("leather", obj->name)
		      || isname ("hide", obj->name)
		      || isname ("deerskin", obj->name)
		      || isname ("deerhide", obj->name)
		      || isname ("pelt", obj->name)
		      || isname ("rawhide", obj->name))
		    update = true;
		}
	      else if (!str_cmp (buf2, "METAL") && !isnamec (obj->name, buf2))
		{
		  if (isname ("sword", obj->name)
		      || isname ("scimitar", obj->name)
		      || isname ("pike", obj->name)
		      || isname ("dagger", obj->name)
		      || isname ("knife", obj->name)
		      || isname ("mace", obj->name)
		      || isname ("halberd", obj->name)
		      || isname ("iron", obj->name)
		      || isname ("steel", obj->name)
		      || isname ("mithril", obj->name)
		      || isname ("ringmail", obj->name)
		      || isname ("metal", obj->name)
		      || isname ("pewter", obj->name)
		      || isname ("scalemail", obj->name)
		      || isname ("bronze", obj->name)
		      || isname ("copper", obj->name)
		      || isname ("brass", obj->name)
		      || isname ("chainmail", obj->name)
		      || isname ("gold", obj->name)
		      || isname ("estoc", obj->name)
		      || isname ("platemail", obj->name)
		      || isname ("polearm", obj->name)
		      || isname ("silver", obj->name)
		      || isname ("lead", obj->name)
		      || isname ("brigandine", obj->name))
		    update = true;
		}
	      else if (!str_cmp (buf2, "WOOD") && !isnamec (obj->name, buf2))
		{
		  if (isname ("shortbow", obj->name)
		      || isname ("longbow", obj->name)
		      || isname ("crossbow", obj->name)
		      || isname ("oak", obj->name)
		      || isname ("lebethron", obj->name)
		      || isname ("wooden", obj->name)
		      || isname ("wood", obj->name)
		      || isname ("cedar", obj->name)
		      || isname ("ash", obj->name))
		    update = true;
		}
	      else if (!str_cmp (buf2, "STONE") && !isnamec (obj->name, buf2))
		{
		  if (isname ("rock", obj->name)
		      || isname ("stone", obj->name)
		      || isname ("granite", obj->name)
		      || isname ("onyx", obj->name)
		      || isname ("obsidian", obj->name))
		    update = true;
		}
	      else if (!str_cmp (buf2, "GLASS") && !isnamec (obj->name, buf2))
		{
		  if (isname ("glass", obj->name)
		      || isname ("bottle", obj->name))
		    update = true;
		}
	      else if (!str_cmp (buf2, "PARCHMENT")
		       && !isnamec (obj->name, buf2))
		{
		  if (isname ("parchment", obj->name)
		      || isname ("paper", obj->name)
		      || isname ("vellum", obj->name)
		      || isname ("scroll", obj->name)
		      || isname ("book", obj->name)
		      || isname ("scroll", obj->name))
		    update = true;
		}
	      else if (!str_cmp (buf2, "LIQUID")
		       && !isnamec (obj->name, buf2))
		{
		  if (isname ("fluid", obj->name)
		      || isname ("liquid", obj->name)
		      || isname ("water", obj->name)
		      || isname ("milk", obj->name)
		      || isname ("ale", obj->name)
		      || isname ("beer", obj->name)
		      || isname ("juice", obj->name)
		      || isname ("oil", obj->name)
		      || isname ("tea", obj->name)
		      || isname ("river", obj->name)
		      || isname ("pond", obj->name))
		    update = true;
		}
	      else if (!str_cmp (buf2, "VEGETATION")
		       && !isnamec (obj->name, buf2))
		{
		  if (isname ("herb", obj->name)
		      || isname ("flower", obj->name)
		      || isname ("plant", obj->name)
		      || isname ("root", obj->name))
		    update = true;
		}
	      else if (!str_cmp (buf2, "CERAMIC")
		       && !isnamec (obj->name, buf2))
		{
		  if (isname ("ceramic", obj->name)
		      || isname ("clay", obj->name))
		    update = true;
		}


	      if (update)
		{
		  updated++;
		  sprintf (buf3, "%s %s", obj->name, buf2);
		  if (obj->name && strlen (obj->name) > 1)
		    mem_free (obj->name);
		  obj->name = str_dup (buf3);
		  update = false;
		}
	    }
	}

      sprintf (buf, "%d items updated.\n", updated);
      send_to_char (buf, ch);
      return;
    }

  if (!str_cmp (buf, "count"))
    {
      for (obj = full_object_list; obj; obj = obj->lnext)
	if (!is_tagged (obj->name))
	  updated++;

      sprintf (buf, "%d items remain untagged.\n", updated);
      send_to_char (buf, ch);
      return;
    }

  if (!(obj = vtoo (atoi (buf))))
    {
      send_to_char ("That object VNUM couldn't be found in the database.\n",
		    ch);
      return;
    }

  if (is_tagged (obj->name))
    {
      send_to_char ("That object already has a material tag!\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  for (size_t j = 0; j <= strlen (buf); j++)
    buf[j] = toupper (buf[j]);

  *buf2 = '\0';

  if (!is_tagged (buf))
    {
      send_to_char
	("Unrecognized material type. See HELP MATERIALS for details.\n", ch);
      return;
    }
  else
    sprintf (buf2, "%s %s", obj->name, buf);

  if (obj->name && strlen (obj->name) > 1)
    mem_free (obj->name);

  obj->name = str_dup (buf2);

  send_to_char ("Done.\n", ch);
}

// The function called by give() when a player tries to redeem an order ticket.

void
redeem_order (CHAR_DATA * ch, OBJ_DATA * ticket, CHAR_DATA * keeper)
{
  MYSQL_RES *result;
  MYSQL_ROW row;
  OBJ_DATA *obj;
  char buf[MAX_STRING_LENGTH];
  char *date_str;
  time_t current_time;

  if (!ch || !ticket || !keeper)
    return;

  mysql_safe_query ("SELECT * FROM special_orders WHERE id = %d",
		    ticket->o.od.value[0]);

  result = mysql_store_result (database);
  if (!result)
    {
      send_to_char ("That special order couldn't be found in the database.\n",
		    ch);
      return;
    }

  row = mysql_fetch_row (result);

  if (!row || !mysql_num_rows (result))
    {
      if (result)
	mysql_free_result (result);
      send_to_char ("That special order couldn't be found in the database.\n",
		    ch);
      return;
    }

  if (keeper->mob->nVirtual != atoi (row[2]))
    {
      act ("$N doesn't recognize $p - you seem to be in the wrong shop!",
	   false, ch, ticket, keeper, TO_CHAR | _ACT_FORMAT);
      mysql_free_result (result);
      return;
    }

  if (time (0) < atoi (row[8]))
    {
      act
	("$N informs you that your order hasn't yet arrived. Please try back at a later time.",
	 false, ch, 0, keeper, TO_CHAR | _ACT_FORMAT);
      mysql_free_result (result);
      return;
    }

  if (*row[4] && strlen (row[4]) > 2 && str_cmp (row[4], "(null)"))
    obj = load_colored_object (atoi (row[3]), row[4], 0, 0);
  else
    obj = load_object (atoi (row[3]));

  obj->count = atoi (row[5]);

  if (!obj)
    {
      send_to_char ("That object could not be loaded.\n", ch);
      if (result)
	mysql_free_result (result);
      return;
    }

  mysql_safe_query ("UPDATE special_orders SET redeemed = 1 WHERE id = %d",
		    ticket->o.od.value[0]);

  int port = engine.get_port ();
  mysql_safe_query ("INSERT INTO %s.receipts "
		    "(time, shopkeep, transaction, who, customer, vnum, "
		    "item, qty, cost, room, gametime, port) "
		    "VALUES (NOW(),%d,'sold','%s','%s',%d,'%s',%d,%d,%d,'%d-%d-%d %d:00',%d)",
		    (engine.get_config ("player_log_db")).c_str (),
		    keeper->mob->nVirtual, GET_NAME (ch), char_short (ch),
		    obj->nVirtual, obj->short_description, obj->count,
		    atoi (row[6]), keeper->in_room, time_info.year,
		    time_info.month + 1, time_info.day + 1, time_info.hour,
		    port);

  mysql_free_result (result);

  mysql_safe_query
    ("SELECT * FROM virtual_boards WHERE board_name = 'Orders' AND post_number = %d",
     ticket->o.od.value[0]);
  result = mysql_store_result (database);

  if (result && mysql_num_rows (result) > 0)
    {
      row = mysql_fetch_row (result);
      current_time = time (0);
      date_str = asctime (localtime (&current_time));
      if (strlen (date_str) > 1)
	date_str[strlen (date_str) - 1] = '\0';

      sprintf (buf, "%s\n#2Note:#0 Order redeemed by %s on %s.\n", row[5],
	       ch->tname, date_str);

      mysql_safe_query
	("UPDATE virtual_boards SET message = '%s' WHERE board_name = 'Orders' AND post_number = %d",
	 buf, ticket->o.od.value[0]);
    }

  if (result)
    mysql_free_result (result);

  act ("Nodding, $N accepts your ticket, producing $p a few moments later.",
       false, ch, obj, keeper, TO_CHAR | _ACT_FORMAT);
  act
    ("Nodding, $N accepts $n's ticket before producing $p from the back room.",
     false, ch, obj, keeper, TO_NOTVICT | _ACT_FORMAT);

  obj_from_char (&ticket, 0);
  extract_obj (ticket);

  obj_to_char (obj, ch);
}

// Unified function to calculate the sale price of a given object based on
// the shopkeeper and any negotiations by a CHAR_DATA, if specified.

// Sell argument is set to T when PC is selling (NPC is buying), and F when
// NPC is selling (PC is buying).

// Results are not rounded for individual list price, but they are rounded
// when an item is purchased or sold.

float
calculate_sale_price (OBJ_DATA * obj, CHAR_DATA * keeper, CHAR_DATA * ch,
		      int quantity, bool round_result, bool sell)
{
  OBJ_DATA *tobj;
  OBJ_DATA *robj;
  ROOM_DATA *store;
  NEGOTIATION_DATA *neg;
  float val_in_farthings = 0;
  float markup = 1.0;
  float markup2 = 1.0;

  if (!keeper || !obj)
    return -1;

  // Calculate costs of item; use econ_markup() for sale to PC, econ_discount()
  // for purchase from NPC or for sale to PC of previously used item.

  // Calculate added cost of any drink in container.

  if (GET_ITEM_TYPE (obj) == ITEM_DRINKCON &&
      obj->o.drinkcon.volume > 0 &&
      (tobj = vtoo (obj->o.drinkcon.liquid)) &&
      GET_ITEM_TYPE (tobj) == ITEM_FLUID)
    {
      if (!IS_SET (obj->obj_flags.extra_flags, ITEM_PC_SOLD) && !sell)
	markup2 = econ_markup (keeper, tobj);
      else
	markup2 = econ_discount (keeper, tobj);
      val_in_farthings += tobj->silver * obj->o.drinkcon.volume * 4 * markup2;
      val_in_farthings += tobj->farthings * obj->o.drinkcon.volume * markup2;
    }

  // Calculate cost of main item.

  if (!IS_SET (obj->obj_flags.extra_flags, ITEM_PC_SOLD) && !sell)
    markup = econ_markup (keeper, obj);
  else //value for selling an item
    markup = econ_discount (keeper, obj);

  if (obj->obj_flags.set_cost > 0)
    {
      val_in_farthings = ((float)(obj->obj_flags.set_cost)/100.0) * quantity;
    }
  else if (obj->obj_flags.set_cost < 0)
    {
      val_in_farthings = 0.0f;
    }
  else
    {
      val_in_farthings +=
	(obj->silver * 4.0 + obj->farthings) * markup * quantity;
    }

  if (ch != NULL)
    {
      if (!sell)
	{			// PC buying item
	  for (neg = keeper->shop->negotiations; neg; neg = neg->next)
	    {
	      if (neg->ch_coldload_id == ch->coldload_id &&
		  neg->obj_vnum == obj->nVirtual)
		break;
	    }
	}
      else
	{			// PC selling item
	  for (neg = keeper->shop->negotiations; neg; neg = neg->next)
	    {
	      if (neg->ch_coldload_id == ch->coldload_id &&
		  neg->obj_vnum == obj->nVirtual && !neg->true_if_buying)
		break;
	    }
	}

      if (neg && neg->price_delta)
	val_in_farthings =
	  val_in_farthings * (100.0 + neg->price_delta) / 100.0 ; //fixed formula
	  
   	}
//ch != NULL

  // Resource modification: price is based on current amount/100 of that resource,
  // so the more you buy the more it costs, and the more you sell the less it is worth.
  // Calculated off the shopkeep's stack. Easy if the PC is buying, but a little tricky
  // if the PC is selling.

  if (GET_ITEM_TYPE(obj) == ITEM_RESOURCE)
  {
    if(!sell)
      val_in_farthings = val_in_farthings * (200 - obj->count) / 100;
    else
    {
      store = vtor (keeper->shop->store_vnum);

      if(!store->contents)
        return 0;

      for (robj = store->contents; robj; robj = robj->next_content) 
      {
        if(robj->nVirtual == obj->nVirtual && robj->count >= 1)
        {
          val_in_farthings = val_in_farthings * (200 - robj->count) / 100;          
          break;
        }
      }

      if(!robj)
        return 0;

    }

    if (val_in_farthings < 100)
      val_in_farthings = 100;
  }

  if (round_result)
    {
      if (!sell)
	val_in_farthings = (int) (ceilf (val_in_farthings));
      else
	val_in_farthings = (int) (floorf (val_in_farthings));
      return val_in_farthings;
    }
  else
    return val_in_farthings;

}

// Control function to check whether or not keeper can order a given object.

bool
can_order (OBJ_DATA * obj, CHAR_DATA * keeper)
{
  if (!obj || !keeper)
    return false;

  // Items in z0 cannot be ordered, since a lot of OOC/staff items are there.
  if (obj->nVirtual < 1000)
    return false;

  // Rare items (those with < 5 instances in-game) cannot be ordered.
  if (vtoo (obj->nVirtual) && vtoo (obj->nVirtual)->instances < 5)
    return false;

  if (!obj->silver && !obj->farthings)
    return false;

  if (GET_ITEM_TYPE (obj) == ITEM_NPC_OBJECT)
    return false;

  if (GET_ITEM_TYPE (obj) == ITEM_KEY)
    return false;

  if (IS_SET (obj->obj_flags.extra_flags, ITEM_MAGIC))
    return false;

  if (!IS_SET (obj->obj_flags.wear_flags, ITEM_TAKE))
    return false;

  if (strstr (obj->name, "unique"))
    return false;

  if (strstr (obj->name, "IMM") || strstr (obj->name, "PC_"))
    return false;

  if (keeper->shop && vtor (keeper->shop->shop_vnum) &&
      IS_SET (vtor (keeper->shop->shop_vnum)->room_flags, LAWFUL) &&
      IS_SET (obj->obj_flags.extra_flags, ITEM_ILLEGAL))
    return false;

  if (!trades_in (keeper, obj))
    return false;

  // Shopkeeps won't order items flagged with their NOBUY flags.
  if (obj->econ_flags & keeper->shop->nobuy_flags)
    return false;

  if (keeper_has_item (keeper, obj->nVirtual))
    return false;

  return true;
}

// Merchants won't bother with special orders below this in value.

#define MINIMUM_ORDER_COST	15

// Base markup on special-ordered items.

#define ORDERING_MARKUP		.85

// Markups for items ordered in specific colors.

#define DRABCOLOR_MARKUP	.25
#define COLOR_MARKUP		.45
#define FINECOLOR_MARKUP	.65

// Waiting period defines for specially-ordered items, in IC days.

#define MIN_WAIT_PERIOD		4
#define MAX_WAIT_PERIOD		28

// Special order routine for NPC shopkeepers.

void
do_order (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *keeper = NULL;
  CHAR_DATA *tch = NULL;
  OBJ_DATA *obj = NULL;
  OBJ_DATA *tobj = NULL;
  ROOM_DATA *room = NULL;
  time_t current_time;
  char *date_str;
  char date[MAX_STRING_LENGTH];
  char color[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char buf3[MAX_STRING_LENGTH];
  char output[MAX_STRING_LENGTH];
  char *p;
  float val_in_farthings = 0, color_markup = 0;
  int quantity = 0, vnum = 0, order_id = 0, i = 0;
  bool header_said = false;

  if (cmd != 2)
    {
      argument = one_argument (argument, buf);

      if (!*buf)
	{
	  send_to_char ("What did you wish to order?\n", ch);
	  return;
	}

      if ((keeper = get_char_room_vis (ch, buf))
	  && IS_SET (keeper->flags, FLAG_KEEPER))
	argument = one_argument (argument, buf);
      else
	{
	  for (tch = ch->room->people; tch; tch = tch->next_in_room)
	    if (tch != ch && IS_SET (tch->flags, FLAG_KEEPER))
	      break;

	  keeper = tch;
	}

      if (!keeper || !keeper->shop)
	{
	  send_to_char ("I see no shopkeeper here to order from.\n", ch);
	  return;
	}

      if (!*buf)
	{
	  act ("Order what from $N?", true, ch, 0, keeper,
	       TO_CHAR | _ACT_FORMAT);
	  return;
	}
    }

  // Confirming an order from previous use of the command.

  else if (cmd == 2)
    {
      if (ch->delay_type != DEL_ORDER_ITEM || !ch->delay_obj)
	{
	  send_to_char
	    ("There is no order awaiting your acceptance, I'm afraid.\n", ch);
	  return;
	}

      obj = ch->delay_obj;
      quantity = ch->delay_info1;
      keeper = ch->delay_ch;
      val_in_farthings = ch->delay_info2;

      sprintf (color, "(null)");
      if (ch->delay_who && strlen (ch->delay_who) > 1)
	sprintf (color, "%s", ch->delay_who);

      ch->delay_obj = NULL;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay_info3 = 0;
      ch->delay_who = NULL;
      ch->delay_ch = NULL;
      ch->delay_type = 0;

      if (!obj || !keeper || val_in_farthings <= 0)
	{
	  send_to_char ("There has been a problem. Order aborted.\n", ch);
	  return;
	}

      if (!can_subtract_money
	  (ch, (int) val_in_farthings, keeper->mob->currency_type))
	{
	  name_to_ident (ch, buf2);
	  sprintf (buf,
		   "%s Come back later when you have the coin for it, won't you?",
		   buf2);
	  do_whisper (keeper, buf, 83);
	  return;
	}

      tobj = load_object (VNUM_ORDER_TICKET);

      if (!tobj)
	{
	  send_to_char
	    ("The ticket object seems to be missing. Aborting...\n", ch);
	  return;
	}

      mysql_safe_query
	("INSERT INTO special_orders (char_name, shopkeep_vnum, item_vnum, var_color, quantity, "
	 "total_cost, order_placed, order_ready, redeemed) VALUES ('%s', %d, %d, '%s', %d, %d, UNIX_TIMESTAMP(), "
	 "(UNIX_TIMESTAMP() + (60*60*6*%d)), 0)", ch->tname,
	 keeper->mob->nVirtual, obj->nVirtual, color, quantity,
	 (int) val_in_farthings, obj->order_time);

      order_id = mysql_insert_id (database);

      mysql_safe_query
	("DELETE FROM virtual_boards WHERE board_name = 'Orders' AND post_number = %d",
	 order_id);

      sprintf (buf, "#2%s#0 [%d]", obj_short_desc (obj), obj->nVirtual);

      if (*color && str_cmp (color, "(null)"))
	sprintf (buf3, "Ordered In:     %s\n", color);

      sprintf (buf2, "Purchased From: #5%s#0 [mob %d in room %d]\n"
	       "Quantity:       %d\n%s"
	       "Total Price:    %d cp\n"
	       "Ready In:       %d in-game day%s\n", char_short (keeper),
	       keeper->mob->nVirtual, keeper->shop->shop_vnum, quantity, buf3,
	       (int) val_in_farthings, obj->order_time,
	       obj->order_time != 1 ? "s" : "");

      current_time = time (0);
      date_str = asctime (localtime (&current_time));
      if (strlen (date_str) > 1)
	date_str[strlen (date_str) - 1] = '\0';

      mysql_safe_query
	("INSERT INTO virtual_boards VALUES ('%s', %d, '%s', '%s', '%s', '%s', UNIX_TIMESTAMP())",
	 "Orders", order_id, buf, ch->tname, date_str, buf2);

      tobj->o.od.value[0] = order_id;
      tobj->o.od.value[1] = keeper->mob->nVirtual;

      *buf2 = '\0';
      if (quantity > 1)
	sprintf (buf2, "%d of", quantity);

      room = vtor (keeper->shop->shop_vnum);

    sprintf (date, "%s %d", short_time_string(time_info.day + 1, time_info.month), time_info.year);

      *buf3 = '\0';
      if (*color && str_cmp (color, "(null)"))
	sprintf (buf3, "in %s,", color);

      sprintf (buf,
	       "This is a small parchment merchandise ticket, redeemable from #5%s#0 "
	       "at #6%s#0. The ticket has been made out for %s #2%s#0, %s for which %d copper was paid, "
	       "and which %s due to be available at the shop approximately %d day%s after %s.",
	       char_short (keeper), room ? room->name : "their shop",
	       *buf2 ? buf2 : "", obj_short_desc (obj), *buf3 ? buf3 : "",
	       (int) val_in_farthings, quantity != 1 ? "are" : "is",
	       obj->order_time, obj->order_time != 1 ? "s" : "", date);

      reformat_desc (buf, &tobj->full_description);

      act ("$n hands $N $p.", false, keeper, tobj, ch,
	   TO_NOTVICT | _ACT_FORMAT);
      act ("$n hands you $p.", false, keeper, tobj, ch,
	   TO_VICT | _ACT_FORMAT);

      obj_to_char (tobj, ch);

      subtract_money (ch, (int) val_in_farthings, keeper->mob->currency_type);

      return;
    }

  if (IS_SET (keeper->act, ACT_NOORDER))
    {
      send_to_char
	("This shopkeeper does not order items for customers, unfortunately.\n",
	 ch);
      return;
    }

  if ((room = vtor (keeper->shop->store_vnum)))
    load_save_room (room);

  // New entry of the command by a player

  if (!str_cmp (buf, "colors"))
    {

      sprintf (buf2, "#5%s#0 can order in these colors:\n\n   ",
	       char_short (keeper));
      buf2[2] = toupper (buf2[2]);

      for (i = 0; *(char *) standard_object_colors[i] != '\n'; i++)
	{
	  sprintf (buf2 + strlen (buf2), "%-23s ",
		   (char *) standard_object_colors[i]);
	  if (!((i + 1) % 3))
	    strcat (buf2, "\n   ");
	}

      if (!((i + 1) % 3) || ((i + 1) % 3) == 2)
	strcat (buf2, "\n");

      page_string (ch->desc, buf2);

      return;
    }

  if (!str_cmp (buf, "finecolors"))
    {

      sprintf (buf2, "#5%s#0 can order in these refined hues:\n\n   ",
	       char_short (keeper));
      buf2[2] = toupper (buf2[2]);

      for (i = 0; *(char *) fine_object_colors[i] != '\n'; i++)
	{
	  sprintf (buf2 + strlen (buf2), "%-23s ",
		   (char *) fine_object_colors[i]);
	  if (!((i + 1) % 3))
	    strcat (buf2, "\n   ");
	}

      if (!((i + 1) % 3) || ((i + 1) % 3) == 2)
	strcat (buf2, "\n");

      page_string (ch->desc, buf2);

      return;
    }

  if (!str_cmp (buf, "drabcolors"))
    {

      sprintf (buf2, "#5%s#0 can order in these drab colorations:\n\n   ",
	       char_short (keeper));
      buf2[2] = toupper (buf2[2]);

      for (i = 0; *(char *) drab_object_colors[i] != '\n'; i++)
	{
	  sprintf (buf2 + strlen (buf2), "%-23s ",
		   (char *) drab_object_colors[i]);
	  if (!((i + 1) % 3))
	    strcat (buf2, "\n   ");
	}

      if (!((i + 1) % 3) || ((i + 1) % 3) == 2)
	strcat (buf2, "\n");

      page_string (ch->desc, buf2);

      return;
    }

  if (!str_cmp (buf, "gemcolors"))
    {

      sprintf (buf2, "#5%s#0 can order in these gem stone colors:\n\n   ",
	       char_short (keeper));
      buf2[2] = toupper (buf2[2]);

      for (i = 0; *(char *) gem_colors[i] != '\n'; i++)
	{
	  sprintf (buf2 + strlen (buf2), "%-23s ", (char *) gem_colors[i]);
	  if (!((i + 1) % 3))
	    strcat (buf2, "\n   ");
	}

      if (!((i + 1) % 3) || ((i + 1) % 3) == 2)
	strcat (buf2, "\n");

      page_string (ch->desc, buf2);

      return;
    }

  if (!str_cmp (buf, "finegemcolors"))
    {

      sprintf (buf2,
	       "#5%s#0 can order in these refined gem stone colors:\n\n   ",
	       char_short (keeper));
      buf2[2] = toupper (buf2[2]);

      for (i = 0; *(char *) fine_gem_colors[i] != '\n'; i++)
	{
	  sprintf (buf2 + strlen (buf2), "%-23s ",
		   (char *) fine_gem_colors[i]);
	  if (!((i + 1) % 3))
	    strcat (buf2, "\n   ");
	}

      if (!((i + 1) % 3) || ((i + 1) % 3) == 2)
	strcat (buf2, "\n");

      page_string (ch->desc, buf2);

      return;
    }

  if (!str_cmp (buf, "preview"))
    {

      argument = one_argument (argument, buf);
      if (!*buf || !isdigit (*buf))
	{
	  send_to_char ("Which item number did you wish to preview?\n", ch);
	  return;
	}

      vnum = atoi (buf);

      if (!(obj = vtoo (vnum)) || !can_order (obj, keeper))
	{
	  name_to_ident (ch, buf);
	  sprintf (buf + strlen (buf),
		   " I'm sorry, but I don't think I can get that particular item for you at this time. Perhaps you should try back again later?");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      act ("$N checks $S records a moment before describing $p to you:",
	   false, ch, obj, keeper, TO_CHAR | _ACT_FORMAT);
      send_to_char ("\n", ch);
      show_obj_to_char (obj, ch, 15);

      return;
    }

  if (!str_cmp (buf, "buy"))
    {

      argument = one_argument (argument, buf);
      if (!*buf || !isdigit (*buf))
	{
	  send_to_char ("Which item number did you wish to order?\n", ch);
	  return;
	}

      vnum = atoi (buf);

      *color = '\0';

      argument = one_argument (argument, buf);
      if (*buf)
	{
	  if (isdigit (*buf))
	    {			// Second number, so previous is qty, this is item #.
	      quantity = vnum;
	      vnum = atoi (buf);
	      argument = one_argument (argument, buf);
	      if (*buf)
		sprintf (color, "%s", buf);
	    }
	  else			// Word, so this is presumably a color for the item.
	    sprintf (color, "%s", buf);
	}

      if (!(obj = vtoo (vnum)) || !can_order (obj, keeper))
	{
	  name_to_ident (ch, buf);
	  sprintf (buf + strlen (buf),
		   " I'm sorry, but I don't think I can get that particular item for you at this time. Perhaps you should try back again later?");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      if (*color)
	{
	  if (!strstr (obj->name, "$color")
	      && !strstr (obj->name, "$drabcolor")
	      && !strstr (obj->name, "$finecolor"))
	    {
	      send_to_char
		("That item cannot be ordered in a specific color.\n", ch);
	      return;
	    }
	  else if ((i = index_lookup (standard_object_colors, color)) != -1)
	    {
	      if (!strstr (obj->name, "$color"))
		{
		  send_to_char
		    ("That item cannot be ordered in this color. See ORDER COLORS for a list.\n",
		     ch);
		  return;
		}
	    }
	  else if ((i = index_lookup (fine_object_colors, color)) != -1)
	    {
	      if (!strstr (obj->name, "$finecolor"))
		{
		  send_to_char
		    ("That item cannot be ordered in this color. See ORDER FINECOLORS for a list.\n",
		     ch);
		  return;
		}
	    }
	  else if ((i = index_lookup (drab_object_colors, color)) != -1)
	    {
	      if (!strstr (obj->name, "$drabcolor"))
		{
		  send_to_char
		    ("That item cannot be ordered in this color. See ORDER DRABCOLORS for a list.\n",
		     ch);
		  return;
		}
	    }
	  else
	    {
	      send_to_char
		("That is not a recognized color. See HELP ORDER for more information.\n",
		 ch);
	      return;
	    }
	}

      if (quantity <= 0)
	quantity = 1;

      if (quantity > 50)
	{
	  send_to_char ("You may only order up to 50 items at one time.\n",
			ch);
	  return;
	}

      // Add in markup percentage for special order color, if any.

      if (*color && strstr (obj->name, "$drabcolor"))
	color_markup = DRABCOLOR_MARKUP;
      else if (*color && strstr (obj->name, "$color"))
	color_markup = COLOR_MARKUP;
      else if (*color && strstr (obj->name, "$finecolor"))
	color_markup = FINECOLOR_MARKUP;
      else
	color_markup = 0.0;

      val_in_farthings =
	(int) (ceilf
	       (calculate_sale_price (obj, keeper, ch, 1, true, false) *
		(1.0 + ORDERING_MARKUP) * (1.0 + color_markup)) * quantity);

      if (val_in_farthings < MINIMUM_ORDER_COST)
	{
	  name_to_ident (ch, buf2);
	  sprintf (buf,
		   "%s Sorry, but special orders worth less than %d copper just aren't worth the effort.",
		   buf2, MINIMUM_ORDER_COST);
	  do_whisper (keeper, buf, 83);
	  return;
	}

      // No previously calculated order time this reboot, so let's figure out a delay (in IC days).
      if (!obj->order_time)
	{
	  obj->order_time = number (MIN_WAIT_PERIOD, MAX_WAIT_PERIOD);
	}

      sprintf (buf2, "%d of", quantity);

      *buf3 = '\0';

      if (*color)
	{
	  sprintf (buf3, "in %s,", color);
	  sprintf (output, ", including shipment and dye");
	}
      else
	sprintf (output, ", including shipment");

      sprintf (buf,
	       "You have opted to order %s #2%s#0, %s for a total of %d copper%s%s. Your"
	       " order will be available for retrieval from #5%s#0 after %d in-game day%s. To confirm"
	       " and render payment, please use the ACCEPT command.",
	       quantity > 1 ? buf2 : "", obj_short_desc (obj),
	       *buf3 ? buf3 : "", (int) val_in_farthings,
	       val_in_farthings != 1 ? "s" : "", output, char_short (keeper),
	       obj->order_time, obj->order_time != 1 ? "s" : "");

      reformat_string (buf, &p);
      send_to_char (p, ch);
      mem_free (p);

      ch->delay_type = DEL_ORDER_ITEM;
      ch->delay_obj = obj;
      ch->delay_info1 = quantity;
      ch->delay_info2 = (int) val_in_farthings;
      ch->delay_ch = keeper;
      if (*color)
	ch->delay_who = str_dup (color);

      return;
    }
  else if (!str_cmp (buf, "list"))
    {
      argument = one_argument (argument, buf);

      *output = '\0';

      for (obj = full_object_list; obj; obj = obj->lnext)
	{

	  if (*buf && !isname (buf, obj->name))
	    continue;

	  if (!can_order (obj, keeper))
	    continue;

	  if (!header_said)
	    {
	      act ("$N can order the following merchandise:", false, ch, 0,
		   keeper, TO_CHAR | _ACT_FORMAT);
	      sprintf (output + strlen (output),
		       "    #       price        item\n");
	      sprintf (output + strlen (output),
		       "  =====     =====        ====\n");
	      header_said = 1;
	    }

	  val_in_farthings =
	    calculate_sale_price (obj, keeper, ch, 1, false,
				  false) * (1.0 + ORDERING_MARKUP);

	  sprintf (buf2, "  #1%5d   %7.2f cp%s  #2%-55.55s#0",
		   obj->nVirtual,
		   val_in_farthings, "   ", obj_short_desc (obj));

	  if (strlen (obj_short_desc (obj)) > 52)
	    {
	      buf2[74] = '.';
	      buf2[75] = '.';
	      buf2[76] = '.';
	      buf2[77] = '\0';
	    }

	  if (strlen (output) + strlen (buf2) >= MAX_STRING_LENGTH)
	    break;

	  sprintf (output + strlen (output), "%s#0\n", buf2);
	}

      if (!header_said)
	act ("$N cannot order any such merchandise, unfortunately.", false,
	     ch, 0, keeper, TO_CHAR | _ACT_FORMAT);
      else
	page_string (ch->desc, output);
    }
}

void
refresh_colors (CHAR_DATA * keeper)
{
  ROOM_DATA *room;
  OBJ_DATA *tobj, *next_obj;
  int i = 0, j = 0, reload_objs[500];

  if (!IS_NPC (keeper) || !IS_SET (keeper->flags, FLAG_KEEPER)
      || !keeper->shop)
    return;

  if (!(room = vtor (keeper->shop->store_vnum)))
    return;

  for (tobj = room->contents; tobj; tobj = next_obj)
    {
      next_obj = tobj->next_content;
      if (keeper_makes (keeper, tobj->nVirtual)
	  && IS_SET (tobj->obj_flags.extra_flags, ITEM_VARIABLE)
	  && !number (0, 1))
	{
	  reload_objs[i] = tobj->nVirtual;
	  i++;
	  extract_obj (tobj);
	}
    }

  if (i)
    {
      for (j = 0; j < i; j++)
	{
	  tobj = load_object (reload_objs[j]);
	  if (tobj)
	    obj_to_room (tobj, room->nVirtual);
	}
    }
}

void
manage_resources (CHAR_DATA * keeper)
{
  ROOM_DATA *room;
  OBJ_DATA *tobj;

  if (!IS_NPC (keeper) || !IS_SET (keeper->flags, FLAG_KEEPER)
      || !keeper->shop || !IS_SET(keeper->act, ACT_BULKTRADER))
    return;

  if (!(room = vtor (keeper->shop->store_vnum)))
    return;

  if (IS_SET (keeper->act, ACT_NOVNPC))
    return;

  if (!room->psave_loaded)
    load_save_room (room);

  for (tobj = room->contents; tobj; tobj = tobj->next_content)
    {
      if (GET_ITEM_TYPE (tobj) != ITEM_RESOURCE)
	continue;

      if(keeper_makes(keeper, tobj->nVirtual) && tobj->count < 100)
        tobj->count += 1;
      else if(tobj->count > 100)
        tobj->count -= 1;
    }

  return;
}


int
vnpc_customer (CHAR_DATA * keeper, int purse)
{
  ROOM_DATA *room;
  OBJ_DATA *tobj;
  int items_in_list = 0, target_item = 0, i = 0;
  int required_check = 0, item_cost = 0;
  float delivery_cost = 0;

  if (!IS_NPC (keeper) || !IS_SET (keeper->flags, FLAG_KEEPER)
      || !keeper->shop)
    return purse;

  if (IS_SET (keeper->act, ACT_NOVNPC))
    return purse;

  if (!(room = vtor (keeper->shop->store_vnum)))
    return purse;

  if (!room->psave_loaded)
    load_save_room (room);

  for (tobj = room->contents; tobj; tobj = tobj->next_content)
    {
      if (GET_ITEM_TYPE (tobj) == ITEM_MONEY)
	continue;
      items_in_list++;
    }

  if (!items_in_list)
    return purse;

  if (items_in_list == 1)
    target_item = 1;
  else
    target_item = number (1, items_in_list);

  for (tobj = room->contents; tobj; tobj = tobj->next_content)
    {
      if (GET_ITEM_TYPE (tobj) == ITEM_MONEY)
	continue;
      i++;
      if (i == target_item)
	break;
    }

  if (!tobj)
    return purse;

  // vNPC sales on things we produce just screw things up.

  if(keeper_makes (keeper, tobj->nVirtual))
    return purse;

  // Cost of item being sold to vNPC
  item_cost = (int) calculate_sale_price (tobj, keeper, NULL, 1, true, false);

  // Cost of ordering replacement item for merchant
  delivery_cost = calculate_sale_price (tobj, keeper, NULL, 1, true, true);

  if (item_cost > VNPC_COPPER_PURSE)
    return item_cost;

  required_check = 55 - (item_cost / 4);
  required_check = MAX (6, required_check);
  int port = engine.get_port ();

  if (number (1, 100) <= required_check)
    {
      target_item = tobj->nVirtual;
      obj_from_room (&tobj, 1);

      money_to_storeroom (keeper, item_cost);

      mysql_safe_query
	("INSERT INTO %s.receipts "
	 "(time, shopkeep, transaction, who, customer, vnum, "
	 "item, qty, cost, room, gametime, port) "
	 "VALUES (NOW(),%d,'sold','%s','%s',%d,'%s',%d,%d,%d,'%d-%d-%d %d:00',%d)",
	 (engine.get_config ("player_log_db")).c_str (),
	 keeper->mob->nVirtual, "vNPC Customer",
	 "an honest-looking person", tobj->nVirtual,
	 tobj->short_description, 1, (int) item_cost, keeper->in_room,
	 time_info.year, time_info.month + 1, time_info.day + 1,
	 time_info.hour, port);

      if (keeper_makes (keeper, target_item)
	  && !get_obj_in_list_num (target_item, room->contents))
	{
	  if (keeper_has_money (keeper, (int) delivery_cost))
	    {
	      subtract_keeper_money (keeper, (int) delivery_cost);
	      obj_to_room (load_object (target_item),
			   keeper->shop->store_vnum);
	      mysql_safe_query 
		("INSERT INTO %s.receipts "
		 "(time, shopkeep, transaction, who, customer, vnum, "
		 "item, qty, cost, room, gametime, port) "
		 "VALUES (NOW()+1,%d,'bought','%s','%s',%d,'%s',%d,%f,%d,'%d-%d-%d %d:00',%d)",
		 (engine.get_config ("player_log_db")).c_str (),
		 keeper->mob->nVirtual, "vNPC Merchant",
		 "an honest-looking merchant", tobj->nVirtual,
		 tobj->short_description, 1, delivery_cost,
		 keeper->in_room, time_info.year, time_info.month + 1,
		 time_info.day + 1, time_info.hour, port);
	    }
	}
      extract_obj (tobj);
    }

  return item_cost;
}


#define NO_SUCH_ITEM1 "Don't seem to have that in my inventory. Would you like to buy something else?"
#define NO_SUCH_ITEM2 "I don't see that particular item. Perhaps you misplaced it?"
#define MISSING_CASH1 "A little too expensive for me now -- why don't you try back later?"
#define MISSING_CASH2 "You're a little short on coin, I see; come back when you can afford it."
#define DO_NOT_BUY    "I don't buy those sorts of things, I'm afraid."

void
do_list (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  char stock_buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char output[MAX_STRING_LENGTH];
  int i;
  float val_in_farthings = 0;
  int header_said = 0;
  CHAR_DATA *keeper = NULL;
  CHAR_DATA *tch;
  ROOM_DATA *room;
  ROOM_DATA *store;
  OBJ_DATA *obj;
  NEGOTIATION_DATA *neg;

  room = ch->room;

  argument = one_argument (argument, buf);

  if (*buf && (keeper = get_char_room_vis (ch, buf)))
    argument = one_argument (argument, buf);

  else
    {
      for (tch = room->people; tch; tch = tch->next_in_room)
	if (tch != ch && IS_SET (tch->flags, FLAG_KEEPER))
	  break;

      keeper = tch;
    }

  if (!keeper)
    {
      send_to_char ("There is no merchant here.\n", ch);
      return;
    }

  if (!GET_TRUST (ch) && !CAN_SEE (keeper, ch))
    {
      do_say (keeper, "Who's there?", 0);
      return;
    }

  if (GET_POS (keeper) <= POSITION_SLEEPING)
    {
      act ("$N is not conscious.", true, ch, 0, keeper, TO_CHAR);
      return;
    }

  if (GET_POS (keeper) == POSITION_FIGHTING)
    {
      do_say (keeper, "Have you no eyes!  I'm fighting for my life!", 0);
      return;
    }

  if (!keeper->shop || !IS_SET (keeper->flags, FLAG_KEEPER))
    {

      if (keeper == ch)
	send_to_char ("You don't have a shop.\n", ch);
      else
	act ("$N isn't a shopkeeper.", false, ch, 0, keeper, TO_CHAR);

      return;
    }

  if (keeper->shop->shop_vnum && keeper->shop->shop_vnum != ch->in_room)
    {
      do_say (keeper, "I'm sorry.  Please catch me when I'm in my shop.", 0);
      return;
    }

  if (!(store = vtor (keeper->shop->store_vnum)))
    {
      do_say (keeper, "I've lost my business.  You'll have to go elsewhere.",
	      0);
      return;
    }

  if (!store->psave_loaded)
    load_save_room (store);

  if (!store->contents)
    {
      do_say (keeper, "I have nothing for sale at the moment.", 0);
      return;
    }

  i = 0;

  *output = '\0';

  for (obj = store->contents; obj; obj = obj->next_content)
    {

      i++;

      if (GET_ITEM_TYPE (obj) == ITEM_MONEY
	  && keeper_uses_currency_type (keeper->mob->currency_type, obj))
	{
	  i--;
	  if (obj->next_content)
	    {
	      continue;
	    }
	  else
	    break;
	}

      OBJ_DATA* drink;
      if (*buf
	  && (!isname (buf, obj->name)
	      && !(GET_ITEM_TYPE (obj) == ITEM_BOOK && obj->book_title
		   && isname (buf, obj->book_title))
	      && !(GET_ITEM_TYPE (obj) == ITEM_DRINKCON
		   && obj->o.drinkcon.volume
		   && (drink = vtoo (obj->o.drinkcon.liquid))
		   && isname (buf, drink->name))))
	continue;

      if (!CAN_WEAR (obj, ITEM_TAKE))
	continue;

      /* Prevent players from buying back items they've sold, and prevent
         all others from buying a sold item for 15 minutes to prevent abuse */

      // I'm curious as to how this is abuse, especially as you can just manually buy the thing.
      // Removing this until I see players abusing it like an Austrian girl locked in
      // an underground cellar complex.
      // - special K
     
      /*
      if ((obj->sold_by != ch->coldload_id
	   && (time (0) - obj->sold_at <= 60 * 15))
	  || (obj->sold_by == ch->coldload_id
	      && (time (0) - obj->sold_at <= 60 * 60)))
	continue;
      */

      if (!header_said)
	{
	  act ("$N describes $S inventory:", false, ch, 0, keeper, TO_CHAR);
	  sprintf (output + strlen (output),
		   "\n   #      price        item\n");
	  sprintf (output + strlen (output), "  ===     =====        ====\n");
	  header_said = 1;
	}

      val_in_farthings =
	calculate_sale_price (obj, keeper, ch, 1, false, false);

      if (val_in_farthings == 0 && obj->obj_flags.set_cost == 0)
	val_in_farthings = 1;

      *stock_buf = '\0';

      if (!keeper_makes (keeper, obj->nVirtual))
	sprintf (stock_buf, "(%d in stock)", obj->count);

      // We don't want the number of resources to be displayed, so show the
      // actual short description, not the short desc modified via count et al.
      // - special K

      // Tossed it in so it displays both the buy/sell price, for ease of use.

      char amt[30] = "";
      char ramt[30] = "";

      if (GET_ITEM_TYPE(obj) == ITEM_RESOURCE)
      {

        sprintf (amt,"%7.2f #9#1buy#0#1",val_in_farthings);
        sprintf (ramt,"%7.2f #9#2sell#0#1", calculate_sale_price (obj, keeper, ch, 1, false, true));
        sprintf (buf2, "  #1%3d   %s%s%s%s  #2%-55.55s#0",
	         i, ((val_in_farthings) ? amt : "   free   "), 
	         "#9#6 /#0#1", ((val_in_farthings) ? ramt : "   free   "), 
	         "   ", obj->short_description);
      }
      else
      {
        sprintf (amt,"%7.2f cp",val_in_farthings);
        sprintf (buf2, "  #1%3d   %s%s  #2%-55.55s#0",
	         i, ((val_in_farthings) ? amt : "   free   "),
	         "   ", obj_short_desc (obj));
      }

      for (neg = keeper->shop->negotiations; neg; neg = neg->next)
	{
	  if (neg->ch_coldload_id == ch->coldload_id &&
	      neg->obj_vnum == obj->nVirtual)
	    break;
	}
      if (IS_WEARABLE (obj) || neg)
	{
	  if (strlen (obj_short_desc (obj)) > 40)
	    {
	      buf2[62] = '.';
	      buf2[63] = '.';
	      buf2[64] = '.';
	    }
	  buf2[65] = '\0';
	  strcat (buf2, "#0");
	}
      else
	{
	  if (strlen (obj_short_desc (obj)) > 52)
	    {
	      buf2[74] = '.';
	      buf2[75] = '.';
	      buf2[76] = '.';
	    }
	  buf2[77] = '\0';
	  strcat (buf2, "#0");
	}

      if (IS_WEARABLE (obj))
	{
	  if (obj->size)
	    sprintf (buf2 + strlen (buf2), " (%s)", sizes_named[obj->size]);
	  else if (keeper_makes (keeper, obj->nVirtual))
	    sprintf (buf2 + strlen (buf2), " (all sizes)");
	  else
	    strcat (buf2, " (all sizes)");
	}


      if(IS_SET(obj->obj_flags.extra_flags, ITEM_VARIABLE) && keeper_makes (keeper, obj->nVirtual))
        sprintf (buf2 + strlen (buf2), " #2(variable)#0");

      if (IS_SET (obj->obj_flags.extra_flags, ITEM_PC_SOLD))
	strcat (buf2, " #6(used)#0");

      sprintf (buf2 + strlen (buf2), "%s", neg ? " (neg)" : "");

      strcat (buf2, "\n");

      if (strlen (output) + strlen (buf2) > MAX_STRING_LENGTH)
	break;

      sprintf (output + strlen (output), "%s", buf2);
    }

  if (!header_said)
    {
      if (*buf)
	act ("$N doesn't have any of those.", false, ch, 0, keeper, TO_CHAR);
      else
	act ("Sadly, $N has nothing to sell.", false, ch, 0, keeper, TO_CHAR);
    }
  else
    page_string (ch->desc, output);
}

void
do_preview (CHAR_DATA * ch, char *argument, int cmd)
{
  int i;
  OBJ_DATA *obj;
  CHAR_DATA *keeper = NULL;
  CHAR_DATA *tch;
  ROOM_DATA *room;
  ROOM_DATA *store;
  bool found = false;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  /* buy [keeper] [count] item [size | !] */

  /* cmd is 1 when this is a barter. */

  room = ch->room;

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Preview what?\n", ch);
      return;
    }

  if ((keeper = get_char_room_vis (ch, buf)))
    argument = one_argument (argument, buf);

  else
    {
      for (tch = room->people; tch; tch = tch->next_in_room)
	if (tch != ch && IS_SET (tch->flags, FLAG_KEEPER))
	  break;

      keeper = tch;

      if (!keeper)
	{
	  send_to_char ("There is no shopkeeper here.\n", ch);
	  return;
	}

      if (!*buf)
	{
	  act ("PREVIEW what from $N?", true, ch, 0, keeper, TO_CHAR);
	  return;
	}
    }

  if (!keeper)
    {
      send_to_char ("There is no merchant here.\n", ch);
      return;
    }

  if (GET_POS (keeper) <= POSITION_SLEEPING)
    {
      act ("$N is not conscious.", true, ch, 0, keeper, TO_CHAR);
      return;
    }

  if (GET_POS (keeper) == POSITION_FIGHTING)
    {
      do_say (keeper, "Have you no eyes!  I'm fighting for my life!", 0);
      return;
    }

  if (!keeper->shop)
    {

      if (keeper == ch)
	send_to_char ("You are not a shopkeeper.", ch);
      else
	act ("$N is not a keeper.", false, ch, 0, keeper, TO_CHAR);

      return;
    }

  if (keeper->shop->shop_vnum && keeper->shop->shop_vnum != ch->in_room)
    {
      do_say (keeper, "I'm sorry.  Please catch me when I'm in my shop.", 0);
      return;
    }

  if (!(store = vtor (keeper->shop->store_vnum)))
    {
      do_say (keeper, "I've lost my business.  You'll have to go elsewhere.",
	      0);
      return;
    }

  if (!store->psave_loaded)
    load_save_room (store);

  if (!store->contents)
    {
      do_say (keeper, "I have nothing for sale at the moment.", 0);
      return;
    }

  if (store->contents && is_number (buf))
    {

      obj = store->contents;

      while (obj && GET_ITEM_TYPE (obj) == ITEM_MONEY
	     && keeper_uses_currency_type (keeper->mob->currency_type, obj))
	obj = obj->next_content;

      for (i = 1;; i++)
	{

	  if (!obj)
	    break;

	  if (GET_ITEM_TYPE (obj) == ITEM_MONEY
	      && keeper_uses_currency_type (keeper->mob->currency_type, obj))
	    {
	      i--;
	      if (obj->next_content)
		{
		  obj = obj->next_content;
		  continue;
		}
	      else
		break;
	    }

	  if (i == atoi (buf))
	    {
	      found = true;
	      break;
	    }

	  obj = obj->next_content;
	}

      if (!obj || !found)
	{
	  name_to_ident (ch, buf2);
	  sprintf (buf, "%s %s", buf2, NO_SUCH_ITEM1);
	  do_whisper (keeper, buf, 83);
	  return;
	}
    }

  else if (!(obj = get_obj_in_list_vis_not_money (ch, buf,
						  vtor (keeper->shop->
							store_vnum)->
						  contents)))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s %s", buf2, NO_SUCH_ITEM1);
      do_whisper (keeper, buf, 83);
      return;
    }

  act ("$N shows you $p.", false, ch, obj, keeper, TO_CHAR);

  send_to_char ("\n", ch);

  show_obj_to_char (obj, ch, 15);

  //append the output of the evaluate command
  show_evaluate_information(ch, obj);
}

int
keeper_has_item (CHAR_DATA * keeper, int ovnum)
{
  OBJ_DATA *tobj;
  ROOM_DATA *room;

  if (!keeper || !(room = vtor (keeper->shop->store_vnum)) || !ovnum)
    return 0;

  for (tobj = room->contents; tobj; tobj = tobj->next_content)
    {
      if (tobj->nVirtual == ovnum
	  && !IS_SET (vtoo (ovnum)->obj_flags.extra_flags, ITEM_VARIABLE))
	return 1;
    }

  return 0;
}

int
keeper_makes (CHAR_DATA * keeper, int ovnum)
{
  int i;

  if (!keeper || !keeper->shop || !ovnum)
    return 0;

  for (i = 0; i < MAX_TRADES_IN; i++)
    if (keeper->shop->delivery[i] == ovnum)
      return 1;

  return 0;
}

void
money_to_storeroom (CHAR_DATA * keeper, int amount)
{
  OBJ_DATA *obj, *next_obj;
  ROOM_DATA *store;
  int money = 0;

  if (!keeper->shop)
    return;
  if (!keeper->shop->store_vnum)
    return;
  if (!(store = vtor (keeper->shop->store_vnum)))
    return;

  while (keeper_has_money (keeper, 1))
    {
      for (obj = store->contents; obj; obj = next_obj)
	{
	  next_obj = obj->next_content;
	  if (GET_ITEM_TYPE (obj) == ITEM_MONEY)
	    {
	      money += ((int) obj->farthings) * obj->count;
	      obj_from_room (&obj, 0);
	      extract_obj (obj);
	    }
	}
    }
  money += amount;
  if (money / 5000)
    {				// Mithril/gold hundredpiece.
      //if (keeper->mob->currency_type == CURRENCY_ORKISH) -- other currencies
      obj = load_object (995);	
      obj->count = money / 5000;
      obj_to_room (obj, store->nVirtual);
      money %= 5000;
    }
  if (money / 1000)
    {				// Gold crown.
      obj = load_object (994);
      obj->count = money / 1000;
      obj_to_room (obj, store->nVirtual);
      money %= 1000;
    }

  if (money / 500)
    {				// Silver tree.
      obj = load_object (993);
      obj->count = money / 500;
      obj_to_room (obj, store->nVirtual);
      money %= 500;
    }

  if (money / 100)
    {				// Silver royal.
      obj = load_object (992);
      obj->count = money / 100;
      obj_to_room (obj, store->nVirtual);
      money %= 100;
    }

  if (money / 10)
    {				// Bronze copper.
      obj = load_object (991);
      obj->count = money / 10;
      obj_to_room (obj, store->nVirtual);
      money %= 10;
    }
  if (money)
    {				// Copper bit.
      obj = load_object (990);
      obj->count = money;
      obj_to_room (obj, store->nVirtual);
    }
}

void
subtract_keeper_money (CHAR_DATA * keeper, int cost)
{
  OBJ_DATA *obj, *next_obj;
  ROOM_DATA *store;
  int money = 0;

  if (!keeper->shop)
    return;
  if (!keeper->shop->store_vnum)
    return;
  if (!(store = vtor (keeper->shop->store_vnum)))
    return;

  while (keeper_has_money (keeper, 1))
    {
      for (obj = store->contents; obj; obj = next_obj)
	{
	  next_obj = obj->next_content;
	  if (GET_ITEM_TYPE (obj) == ITEM_MONEY
	      && keeper_uses_currency_type (keeper->mob->currency_type, obj))
	    {
	      money += ((int) obj->farthings) * obj->count;
	      obj_from_room (&obj, 0);
	      extract_obj (obj);
	    }
	}
    }

  money -= cost;

  if (money / 5000)
    {				// Mithril/gold hundredpiece.
      //if (keeper->mob->currency_type == CURRENCY_ORKISH)
      obj = load_object (995);
      obj->count = money / 5000;
      obj_to_room (obj, store->nVirtual);
      money %= 5000;
    }

  if (money / 1000)
    {				// Gold crown.
      obj = load_object (994);
      obj->count = money / 1000;
      obj_to_room (obj, store->nVirtual);
      money %= 1000;
    }

  if (money / 500)
    {				// Silver tree.
      obj = load_object (993);
      obj->count = money / 500;
      obj_to_room (obj, store->nVirtual);
      money %= 500;
    }

  if (money / 100)
    {				// Silver royal.
      obj = load_object (992);
      obj->count = money / 100;
      obj_to_room (obj, store->nVirtual);
      money %= 100;
    }

  if (money / 10)
    {				// Bronze copper.
      obj = load_object (991);
      obj->count = money / 10;
      obj_to_room (obj, store->nVirtual);
      money %= 10;
    }

  if (money)
    {				// Copper bit.
      obj = load_object (990);
      obj->count = money;
      obj_to_room (obj, store->nVirtual);
    }
}

void
do_buy (CHAR_DATA * ch, char *argument, int cmd)
{
  int buy_count = 1;
  float keepers_cost = 0;
  float delivery_cost = 0;
  int i;
  int regardless = 0;
  int wants_off_size = 0;
  int size = -1;
  int nobarter_flag;
  int discount;
  int keeper_success;
  int language_barrier = 0;
  int ch_success, flags = 0;
  int orig_count = 0;
  OBJ_DATA *obj;
  OBJ_DATA *tobj;
  CHAR_DATA *horse;
  CHAR_DATA *keeper = NULL;
  CHAR_DATA *tch;
  ROOM_DATA *room;
  ROOM_DATA *store;
  NEGOTIATION_DATA *neg = NULL;
  char name[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char buf3[MAX_STRING_LENGTH] = {'\0'};
  char buf4[MAX_STRING_LENGTH] = {'\0'};
  bool variable = false;

  /* buy [keeper] [count] item [size | ! | variable] */

  /* cmd is 1 when this is a barter. */

  /* cmd is 2 when confirming a purchase */

  room = ch->room;

  argument = one_argument (argument, buf);

  if (cmd != 2)
    {
      if (!*buf)
	{
	  send_to_char ("Buy what?\n", ch);
	  return;
	}

      if ((keeper = get_char_room_vis (ch, buf))
	  && IS_SET (keeper->flags, FLAG_KEEPER))
	argument = one_argument (argument, buf);

      else
	{
	  for (tch = room->people; tch; tch = tch->next_in_room)
	    if (tch != ch && IS_SET (tch->flags, FLAG_KEEPER))
	      break;

	  keeper = tch;

	  if (!*buf)
	    {
	      act ("Buy what from $N?", true, ch, 0, keeper, TO_CHAR);
	      return;
	    }
	}

      argument = one_argument (argument, buf2);

      /* buf is a count if buf2 is an object */

      if (isdigit (*buf) && *buf2 &&
	  (isdigit (*buf2)
	   || (keeper && keeper->shop
	       && get_obj_in_list_vis_not_money (ch, buf2,
						 vtor (keeper->shop->
						       store_vnum)->
						 contents))))
	{
	  buy_count = atoi (buf);

	  if (buy_count > 50)
	    {
	      send_to_char ("You can only buy up to 50 items at a time.\n",
			    ch);
	      return;
	    }
	  strcpy (buf, buf2);
	  argument = one_argument (argument, buf2);
	}

      if (*buf2 == '!')
	regardless = 1;
      else if (!str_cmp(buf2, "variable"))
      {
        argument = one_argument(argument, buf3);
        argument = one_argument(argument, buf4);      
        variable = true;
      }
      else if (*buf2)
	{

	  size = index_lookup (sizes_named, buf2);

	  if (size == -1)
	    size = index_lookup (sizes, buf2);

	  wants_off_size = 1;
	}
    }
  else
    keeper = ch->delay_ch;

  if (!keeper || keeper->room != ch->room)
    {
      send_to_char ("There is no merchant here.\n", ch);
      return;
    }

  if (keeper == ch)
    {
      send_to_char ("You can't buy from yourself!\n", ch);
      return;
    }

  if (GET_POS (keeper) <= POSITION_SLEEPING)
    {
      act ("$N is not conscious.", true, ch, 0, keeper, TO_CHAR);
      return;
    }

  if (GET_POS (keeper) == POSITION_FIGHTING)
    {
      do_say (keeper, "Have you no eyes!  I'm fighting for my life!", 0);
      return;
    }

  if (!GET_TRUST (ch) && !CAN_SEE (keeper, ch))
    {
      do_say (keeper, "Who's there?", 0);
      return;
    }

  if (!keeper->shop)
    {

      if (keeper == ch)
	send_to_char ("You are not a shopkeeper.", ch);
      else
	act ("$N is not a keeper.", false, ch, 0, keeper, TO_CHAR);

      return;
    }

  if (!keeper->shop || !IS_NPC (keeper))
    {
      send_to_char ("Are you sure they're a shopkeeper?\n", ch);
      return;
    }

  if (keeper->shop->shop_vnum && keeper->shop->shop_vnum != ch->in_room)
    {
      do_say (keeper, "I'm sorry.  Please catch me when I'm in my shop.", 0);
      return;
    }

  if (!(store = vtor (keeper->shop->store_vnum)))
    {
      do_say (keeper, "I've lost my business.  You'll have to go elsewhere.",
	      0);
      return;
    }

  if (!store->psave_loaded)
    load_save_room (store);

  if (!store->contents)
    {
      do_say (keeper, "I have nothing for sale at the moment.", 0);
      return;
    }

  if (cmd != 2)
    {
      if (is_number (buf))
	{

	  obj = store->contents;

	  while (obj && GET_ITEM_TYPE (obj) == ITEM_MONEY
		 && keeper_uses_currency_type (keeper->mob->currency_type,
					       obj))
	    obj = obj->next_content;

	  for (i = 1;; i++)
	    {

	      if (!obj)
		break;

	      if (GET_ITEM_TYPE (obj) == ITEM_MONEY
		  && keeper_uses_currency_type (keeper->mob->currency_type,
						obj))
		{
		  i--;
		  if (obj->next_content)
		    {
		      obj = obj->next_content;
		      continue;
		    }
		  else
		    {
		      obj = NULL;
		      break;
		    }
		}

	      /* Prevent players from buying back items they've sold, and prevent
	         all others from buying a sold item for 15 minutes to prevent abuse

	      if ((obj->sold_by != ch->coldload_id
		   && (time (0) - obj->sold_at <= 60 * 15))
		  || (obj->sold_by == ch->coldload_id
		      && (time (0) - obj->sold_at <= 60 * 60)))
		{
		  i--;
		  if (obj->next_content)
		    {
		      obj = obj->next_content;
		      continue;
		    }
		  else
		    {
		      obj = NULL;
		      break;
		    }
		}*/

	      if (i == atoi (buf))
		break;

	      obj = obj->next_content;
	    }

	  if (!obj)
	    {
	      send_to_char ("There are not that many items in the keeper's "
			    "inventory.\n", ch);
	      return;
	    }
	}

      else if (!(obj = get_obj_in_list_vis_not_money (ch, buf,
						      vtor (keeper->shop->
							    store_vnum)->
						      contents)))
	{
	  name_to_ident (ch, buf2);
	  sprintf (buf, "%s %s", buf2, NO_SUCH_ITEM1);
	  do_whisper (keeper, buf, 83);
	  return;
	}

      if (IS_WEARABLE (obj) && wants_off_size)
	{
	  if (obj->size && obj->size != size)
	    {
	      act ("$p isn't that size.", false, ch, obj, 0, TO_CHAR);
	      return;
	    }
	}

      else if (IS_WEARABLE (obj) && obj->size &&
	       obj->size != get_size (ch) && !regardless && obj->size != size)
	{
	  act ("$p wouldn't fit you.", false, ch, obj, 0, TO_CHAR);
	  act ("(End the buy command with ! if you really want it.)",
	       false, ch, obj, 0, TO_CHAR);
	  return;
	}
/*
      if(GET_ITEM_TYPE(obj) == ITEM_RESOURCE && buy_count > 1)
      {
	  send_to_char("You can only buy one load of resources at a time.\n", ch);
	  do_whisper (keeper, buf, 83);
	  return;        
      }*/
      
      if (!keeper_makes (keeper, obj->nVirtual) && buy_count > obj->count)
	{
	  name_to_ident (ch, buf2);
	  sprintf (buf, "%s I only have %d of that in stock at the moment.",
		   buf2, obj->count);
	  do_whisper (keeper, buf, 83);
	  return;
	}
      else if (keeper_makes (keeper, obj->nVirtual) && buy_count > obj->count)
	obj->count = buy_count;

      if (buy_count < 1)
	buy_count = 1;
    }
  else
    {
      if (ch->delay_type != DEL_PURCHASE_ITEM || !ch->delay_obj)
	{
	  send_to_char ("There is no purchase in progress, I'm afraid.\n",
			ch);
	  ch->delay_type = 0;
	  ch->delay_info1 = 0;
	  ch->delay_obj = NULL;
	  ch->delay_ch = NULL;
	  return;
	}

      if ((obj = ch->delay_obj) && obj->in_room != keeper->shop->store_vnum)
	{
	  send_to_char ("That item is no longer available for purchase.\n",
			ch);
	  ch->delay_type = 0;
	  ch->delay_info1 = 0;
	  ch->delay_obj = NULL;
	  ch->delay_ch = NULL;
	  return;
	}
      else if (!obj)
	{
	  send_to_char ("That item is no longer available for purchase.\n",
			ch);
	  ch->delay_type = 0;
	  ch->delay_info1 = 0;
	  ch->delay_obj = NULL;
	  ch->delay_ch = NULL;
	  return;
	}

      buy_count = ch->delay_info1;

      if (buy_count < 1)
	buy_count = 1;

      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_obj = NULL;
      ch->delay_ch = NULL;
    }

  keepers_cost =
    calculate_sale_price (obj, keeper, ch, buy_count, true, false);

  if (IS_SET (ch->room->room_flags, LAWFUL) &&
      IS_SET (obj->obj_flags.extra_flags, ITEM_ILLEGAL))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s Those are illegal.  I can't sell them.", buf2);
      do_whisper (keeper, buf, 83);
      return;
    }

  if (GET_ITEM_TYPE (obj) == ITEM_NPC_OBJECT)
    {
      if (cmd != 2)
	{
	  if (!*buf2)
	    {
	      send_to_char
		("You'll need to specify a name for your new NPC, e.g. \"buy horse Shadowfax\".\n",
		 ch);
	      return;
	    }
	}
      else
	{
	  sprintf (buf2, "%s", ch->delay_who);
	  mem_free (ch->delay_who);
	}
      if (strlen (buf2) > 26)
	{
	  send_to_char
	    ("The NPC's name must be 26 letters or fewer in length.\n", ch);
	  return;
	}
      for (size_t i = 0; i < strlen (buf2); i++)
	{
	  if (!isalpha (buf2[i]))
	    {
	      send_to_char ("Invalid characters in the proposed NPC name.\n",
			    ch);
	      return;
	    }
	}
      sprintf (name, "%s", buf2);
    }

  if (cmd == 1)
    {				/* passed by barter command to do_buy */

      name_to_ident (ch, buf);

      if (keepers_cost < 20)
	{
	  strcat (buf, " This isn't worth haggling over.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      if (IS_SET (obj->obj_flags.extra_flags, ITEM_PC_SOLD))
	{
	  strcat (buf, " I won't haggle over a used piece of merchandse.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      nobarter_flag = index_lookup (econ_flags, "nobarter");

      if (nobarter_flag != -1 &&
	  IS_SET (keeper->shop->nobuy_flags, 1 << nobarter_flag))
	{
	  strcat (buf, " I'm sorry, but I will not haggle.  My prices "
		  "are fixed, take it or leave it.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      if (nobarter_flag != -1 && IS_SET (obj->econ_flags, 1 << nobarter_flag))
	{
	  strcat (buf, " I'm sorry, but I will not haggle over the price "
		  "of this item.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      /* Search for existing entry in keepers negotiations list */

      for (neg = keeper->shop->negotiations; neg; neg = neg->next)
	{
	  if (neg->ch_coldload_id == ch->coldload_id &&
	      neg->obj_vnum == obj->nVirtual && neg->true_if_buying)
	    break;
	}

      if (neg)
	{
	  if (neg->price_delta > 0)
	    {
	      strcat (buf, " No, no, I cannot afford to lower the price on "
		      "that again.");
	      do_whisper (keeper, buf, 83);
	      return;
	    }

	  strcat (buf, " You're persistent, aren't you?  I said no, and I "
		  "meant no.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      /* keeper will be reluctant to sell to foreigners */
      language_barrier =
	(keeper->skills[ch->speaks] <
	 15) ? (15 - keeper->skills[ch->speaks]) : (0);
      keeper_success =
	combat_roll (MIN
		     (95, keeper->skills[SKILL_BARTER] + language_barrier), 0, 0);
      ch_success =
	combat_roll (MAX (5, ch->skills[SKILL_BARTER] - language_barrier), 0, 0);

      if (ch_success == SUC_CS && keeper_success == SUC_MS)
	discount = 5;
      else if (ch_success == SUC_CS && keeper_success == SUC_MF)
	discount = 10;
      else if (ch_success == SUC_CS && keeper_success == SUC_CF)
	discount = 15;

      else if (ch_success == SUC_MS && keeper_success == SUC_MS)
	discount = 0;
      else if (ch_success == SUC_MS && keeper_success == SUC_MF)
	discount = 5;
      else if (ch_success == SUC_MS && keeper_success == SUC_CF)
	discount = 10;

      else if (ch_success == SUC_MF && keeper_success == SUC_MS)
	discount = 0;
      else if (ch_success == SUC_MF && keeper_success == SUC_MF)
	discount = 0;
      else if (ch_success == SUC_MF && keeper_success == SUC_CF)
	discount = 5;

      else
	discount = 0;		/* A CF by ch */

      discount = -1 * discount; //changing to a lower price
      neg = (NEGOTIATION_DATA *) alloc (sizeof (NEGOTIATION_DATA), 40);
      neg->ch_coldload_id = ch->coldload_id;
      neg->obj_vnum = obj->nVirtual;
      neg->time_when_forgotten = time (NULL) + 6 * 60 * 60;	/* 6 hours */
      neg->price_delta = discount;
      neg->transactions = 0;
      neg->true_if_buying = 1;
      neg->next = keeper->shop->negotiations;
      keeper->shop->negotiations = neg;

      if (discount == 0)
	{
	  strcat (buf, " The price is as stated.  Take it or leave it.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      else if (discount == 5)
	strcat (buf, " I like your face, you seem an honest and "
		"trustworthy sort.  You can have it for ");
      else if (discount == 10)
	strcat (buf, " It's just not my day, is it?  All right, you win, "
		"I'll sell at your price.  It's yours for ");
      else
	strcat (buf, " My word!  I need to learn to count!  At this rate, "
		"I'll be out of business in a week.  Here, here, take "
		"your ill-gotten gain and begone.  Take it away for ");

      keepers_cost = keepers_cost * (100 + discount) / 100;

      keepers_cost = (int) keepers_cost;

      sprintf (buf + strlen (buf), "%d copper%s", (int) keepers_cost,
	       (int) keepers_cost > 1 ? "s" : "");

      strcat (buf, ".");

      do_whisper (keeper, buf, 83);

      return;
    }

  if ((keepers_cost > 0 && keepers_cost < 1)
      || (keepers_cost == 0 && obj->obj_flags.set_cost == 0))
    keepers_cost = 1;


  keepers_cost = (int) keepers_cost;

  if (cmd != 2)
    {
      orig_count = obj->count;
      obj->count = buy_count;
      if (obj->in_room != keeper->shop->store_vnum)
	obj->in_room = keeper->shop->store_vnum;

      if(variable && keeper_makes (keeper, obj->nVirtual))
      {
        tobj = load_colored_object(obj->nVirtual, buf3, buf4, NULL);
        sprintf (buf, "You have opted to purchase #2%s#0, for a total of %d copper%s. To confirm, please use the ACCEPT command.\n", obj_short_desc (tobj), (int) keepers_cost, (int) keepers_cost > 1 ? "s" : "");
        sprintf(buf + strlen(buf), "\nThe long description of this object will be: #2%s#0", tobj->description);

        if(!*buf3)
          sprintf(buf3, tobj->var_color);
        if(!*buf4)
          sprintf(buf4, tobj->var_color2);
      }
      else
        sprintf (buf, "You have opted to purchase #2%s#0, for a total of %d copper%s. To confirm, please use the ACCEPT command.", obj_short_desc (obj), (int) keepers_cost, (int) keepers_cost > 1 ? "s" : "");


      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      ch->delay_type = DEL_PURCHASE_ITEM;
      ch->delay_obj = obj;
      ch->delay_info1 = buy_count;
      if (ch->delay_info1 < 1)
	ch->delay_info1 = 1;
      ch->delay_ch = keeper;
      obj->count = orig_count;
      if (GET_ITEM_TYPE (obj) == ITEM_NPC_OBJECT)
	ch->delay_who = str_dup (name);
      else if(variable)
      {
        ch->delay_who = str_dup(buf3);
        ch->delay_who2 = str_dup(buf4);
      }

      

      return;
    }

  if (!can_subtract_money (ch, (int) keepers_cost, keeper->mob->currency_type))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s %s", buf2, MISSING_CASH2);
      do_whisper (keeper, buf, 83);
      return;
    }

  if (obj->morphTime)
    {
      obj->clock = vtoo (obj->nVirtual)->clock;
      obj->morphTime = time (0) + obj->clock * 15 * 60;
    }

  tobj = obj;
  obj_from_room (&tobj, buy_count);

  int port = engine.get_port ();

  mysql_safe_query 
    ("INSERT INTO %s.receipts "
     "(time, shopkeep, transaction, who, customer, vnum, "
     "item, qty, cost, room, gametime, port) "
     "VALUES (NOW(),%d,'sold','%s','%s',%d,'%s',%d,%f,%d,'%d-%d-%d %d:00',%d)",
     (engine.get_config ("player_log_db")).c_str (),
     keeper->mob->nVirtual, GET_NAME (ch), char_short (ch),
     tobj->nVirtual, tobj->short_description, tobj->count, keepers_cost,
     keeper->in_room, time_info.year, time_info.month + 1,
     time_info.day + 1, time_info.hour, port);
  
  if (keeper_makes (keeper, obj->nVirtual)
      && !get_obj_in_list_num (obj->nVirtual,
			       vtor (keeper->shop->store_vnum)->contents))
    {
      tobj->count = buy_count;
      delivery_cost = calculate_sale_price (obj, keeper, NULL, 1, true, true);
      if (keeper_has_money (keeper, (int) delivery_cost))
	{
	  obj_to_room (load_object (obj->nVirtual), keeper->shop->store_vnum);
	  subtract_keeper_money (keeper, (int) delivery_cost);
	  mysql_safe_query
	    ("INSERT INTO %s.receipts "
	     "(time, shopkeep, transaction, who, customer, vnum, "
	     "item, qty, cost, room, gametime, port) "
	     "VALUES (NOW()+1,%d,'bought','%s','%s',%d,'%s',%d,%f,%d,'%d-%d-%d %d:00',%d)",
	     (engine.get_config ("player_log_db")).c_str (),
	     keeper->mob->nVirtual, "vNPC Merchant",
	     "an honest-looking merchant", obj->nVirtual,
	     obj->short_description, 1, delivery_cost, keeper->in_room,
	     time_info.year, time_info.month + 1, time_info.day + 1,
	     time_info.hour, port);
	}
    }

  if(keeper_makes (keeper, tobj->nVirtual)
      && IS_SET(tobj->obj_flags.extra_flags, ITEM_VARIABLE) && (ch->delay_who || ch->delay_who2))
  {
    tobj = load_colored_object(obj->nVirtual, ch->delay_who, ch->delay_who2, NULL);
    ch->delay_who = NULL;
    ch->delay_who2 = NULL;
  }

  act ("$n buys $p.", false, ch, tobj, 0, TO_ROOM);
  act ("$N sells you $p.", false, ch, tobj, keeper, TO_CHAR);
  act ("You sell $N $p.", false, keeper, tobj, ch, TO_CHAR);

  subtract_money (ch, (int) keepers_cost, keeper->mob->currency_type);

  name_to_ident (ch, buf2);

  if (ch->room->zone == 5 || ch->room->zone == 6)
    {
      sprintf (buf,
	       "%s You're lucky I gave it to you for %d copper%s, maggot.",
	       buf2, (int) keepers_cost, (int) keepers_cost > 1 ? "s" : "");
    }
  else
    {
      sprintf (buf, "%s A veritable steal at ", buf2);
      sprintf (buf + strlen (buf),
	       "%d copper%s! Enjoy it, my friend.", (int) keepers_cost,
	       (int) keepers_cost > 1 ? "s" : "");
    }

  do_whisper (keeper, buf, 83);

  money_to_storeroom (keeper, (int) keepers_cost);

  if (GET_ITEM_TYPE (tobj) == ITEM_NPC_OBJECT)
    {
      *name = toupper (*name);
      if (tobj->o.od.value[0] == 0
	  || !(horse = load_mobile (tobj->o.od.value[0])))
	{
	  send_to_char
	    ("There seems to be a problem. Please inform the staff.\n", ch);
	  return;
	}
      send_to_room ("\n", keeper->in_room);
      sprintf (buf, "%s#0 is released into your custody.",
	       char_short (horse));
      *buf = toupper (*buf);
      sprintf (buf2, "#5%s", buf);
      act (buf2, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      sprintf (buf, "%s#0 is released into #5%s#0's custody.",
	       char_short (horse), char_short (ch));
      *buf = toupper (*buf);
      sprintf (buf2, "#5%s", buf);
      act (buf2, false, ch, 0, 0, TO_NOTVICT | _ACT_FORMAT);
      char_to_room (horse, ch->in_room);
      horse->act |= ACT_STAYPUT;
      sprintf (buf, "%s %s", horse->name, name);
      mem_free (horse->name);
      horse->name = str_dup (buf);
      if (get_clan (ch, "hules", &flags))
	{
	  add_clan (horse, "hules", CLAN_MEMBER);
	}
      if (IS_SET (horse->act, ACT_MOUNT))
	hitch_char (ch, horse);
      if (!IS_NPC (ch))
	{
	  horse->mob->owner = str_dup (ch->tname);
	  save_char (ch, true);
	}
      return;
    }

  if (GET_ITEM_TYPE (obj) == ITEM_LIGHT)
    {
      tobj->o.od.value[1] = obj->o.od.value[1];
    }

  if (IS_WEARABLE (tobj))
    {
      if (size != -1)
	tobj->size = size;
      else if (regardless && tobj->size)
	;
      else
	tobj->size = get_size (ch);
    }

  if (GET_ITEM_TYPE (tobj) == ITEM_CONTAINER && tobj->o.od.value[2] > 0
      && vtoo (tobj->o.od.value[2])
      && GET_ITEM_TYPE (vtoo (tobj->o.od.value[2])) == ITEM_KEY)
    {
      obj = load_object (tobj->o.od.value[2]);
      obj->o.od.value[1] = tobj->coldload_id;
      obj_to_obj (obj, tobj);
      obj = load_object (tobj->o.od.value[2]);
      obj->o.od.value[1] = tobj->coldload_id;
      obj_to_obj (obj, tobj);
    }

  if(GET_ITEM_TYPE(tobj) == ITEM_RESOURCE)
  {
    one_argument (obj->name, buf2);
    sprintf (buf, "emote sets *%s down nearby, nodding to ~%s.", buf2, ch->tname);
    command_interpreter (keeper, buf);
    obj_to_room (tobj, keeper->in_room);
  }
  else if (ch->right_hand && ch->left_hand)
    {
      sprintf (buf,
	       "%s Your hands seem to be full, so I'll just set this down for you to pick up when you've a chance.",
	       ch->tname);
      send_to_char ("\n", ch);
      do_whisper (keeper, buf, 83);
      one_argument (obj->name, buf2);
      send_to_char ("\n", ch);
      sprintf (buf, "emote sets *%s down nearby, nodding to ~%s.", buf2,
	       ch->tname);
      command_interpreter (keeper, buf);
      obj_to_room (tobj, keeper->in_room);
    }
  else
    obj_to_char (tobj, ch);

  if (neg)
    if (!neg->transactions++)
      skill_use (ch, SKILL_BARTER, 0);
}

int
keeper_uses_currency_type (int currency_type, OBJ_DATA * obj)
{
  if (currency_type == CURRENCY_NOLDO)
    {
      if (obj->nVirtual == 995 || obj->nVirtual == 994
	  || obj->nVirtual == 993 || obj->nVirtual == 992
	  || obj->nVirtual == 991 || obj->nVirtual == 990)
	return 1;
    }
  //else if (currency_type == CURRENCY_ORKISH)
  return 0;
}

int
trades_in (CHAR_DATA * keeper, OBJ_DATA * obj)
{
  OBJ_DATA *tobj;
  int i;
  bool block = true;

  if (!(obj->silver + obj->farthings))
	  return 0;
  
   if (obj->obj_flags.type_flag == 0)
  	  return 0;
  	  
  if (GET_ITEM_TYPE (obj) == ITEM_MONEY
      && keeper_uses_currency_type (keeper->mob->currency_type, obj))
    return 0;

  if (IS_SET (keeper->room->room_flags, LAWFUL)
      && IS_SET (obj->obj_flags.extra_flags, ITEM_ILLEGAL))
    return 0;

//since fluids are no-take, this check has to be here
//casuing some side effects so I am setting it back to no-buy
  if (GET_ITEM_TYPE (obj) == ITEM_FLUID)
  	  return 0;
  	  
  if (!IS_SET (obj->obj_flags.wear_flags, ITEM_TAKE))
	  return 0;
	  
  for (i = 0; i < MAX_TRADES_IN; i++)
    {
      if (obj->obj_flags.type_flag == keeper->shop->trades_in[i])
	block = false;
    }

  if (block)
    return 0;

  if (keeper->shop->materials
      && !IS_SET (keeper->shop->materials, GET_MATERIAL_TYPE (obj)))
    return 0;

  if (keeper->shop->buy_flags && !(obj->econ_flags & keeper->shop->buy_flags))
    return 0;

  if (keeper->shop->nobuy_flags
      && (obj->econ_flags & keeper->shop->nobuy_flags))
    return 0;

  // Check any liquid inside the object for trades_in eligibility.

  if (GET_ITEM_TYPE (obj) == ITEM_DRINKCON && obj->o.drinkcon.volume > 0)
    {
      if ((tobj = vtoo (obj->o.drinkcon.liquid)))
	{
	  if (!trades_in (keeper, tobj))
	    return 0;
	}
      else
	return 0;
    }

  return 1;
}

int
keeper_has_money (CHAR_DATA * keeper, int cost)
{
  ROOM_DATA *store;
  OBJ_DATA *obj;
  int money = 0;

  if (!keeper->shop)
    return 0;
  if (!keeper->shop->store_vnum)
    return 0;
  if (!(store = vtor (keeper->shop->store_vnum)))
    return 0;

  if (!store->psave_loaded)
    load_save_room (store);

  if (store->contents && GET_ITEM_TYPE (store->contents) == ITEM_MONEY
      && keeper_uses_currency_type (keeper->mob->currency_type,
				    store->contents))
    {
      money = ((int) store->contents->farthings) * store->contents->count;
    }
  for (obj = store->contents; obj; obj = obj->next_content)
    {
      if (obj->next_content && GET_ITEM_TYPE (obj->next_content) == ITEM_MONEY
	  && keeper_uses_currency_type (keeper->mob->currency_type,
					obj->next_content))
	{
	  money += ((int) obj->next_content->farthings) * obj->next_content->count;
	}
    }

  if (money < cost)
    return 0;
  else
    return money;
}

void
keeper_money_to_char (CHAR_DATA * keeper, CHAR_DATA * ch, int money)
{
  OBJ_DATA *obj, *tobj;
  char buf[MAX_STRING_LENGTH];
  int mithril = 0, crown = 0, tree = 0, hundredpiece = 0;
  int royal = 0, copper = 0, bit = 0, location;
  bool money_found = false;

  for (location = 0; location < MAX_WEAR; location++)
    {
      if (!(tobj = get_equip (ch, location)))
	continue;
      if (GET_ITEM_TYPE (tobj) == ITEM_CONTAINER)
	{
	  for (obj = tobj->contains; obj; obj = obj->next_content)
	    if (GET_ITEM_TYPE (obj) == ITEM_MONEY)
	      money_found = true;
	  if (money_found)
	    break;
	}
    }

  if (!tobj)
    {
      for (location = 0; location < MAX_WEAR; location++)
	{
	  if (!(tobj = get_equip (ch, location)))
	    continue;
	  if (GET_ITEM_TYPE (tobj) == ITEM_CONTAINER)
	    break;
	}
    }

  if (money / 5000)
    {				// Mithril/gold hundredpiece.
      //if (keeper->mob->currency_type == CURRENCY_ORKISH)
      obj = load_object (995);
      obj->count = money / 5000;
      mithril = money / 5000;
      if (tobj)
	obj_to_obj (obj, tobj);
      else
	obj_to_char (obj, ch);
      money %= 5000;
    }

  if (money / 1000)
    {				// Gold crown.
      obj = load_object (994);
      obj->count = money / 1000;
      if (tobj)
	obj_to_obj (obj, tobj);
      else
	obj_to_char (obj, ch);
      crown = money / 1000;
      money %= 1000;
    }

  if (money / 500)
    {				// Silver tree.
      obj = load_object (993);
      obj->count = money / 500;
      if (tobj)
	obj_to_obj (obj, tobj);
      else
	obj_to_char (obj, ch);
      tree = money / 500;
      money %= 500;
    }

  if (money / 100)
    {				// Silver royal.
      obj = load_object (992);
      obj->count = money / 100;
      if (tobj)
	obj_to_obj (obj, tobj);
      else
	obj_to_char (obj, ch);
      royal = money / 100;
      money %= 100;
    }

  if (money / 10)
    {				// Bronze copper.
      obj = load_object (991);
      obj->count = money / 10;
      if (tobj)
	obj_to_obj (obj, tobj);
      else
	obj_to_char (obj, ch);
      copper = money / 10;
      money %= 10;
    }

  if (money)
    {				// Copper bit.
      obj = load_object (990);
      obj->count = money;
      if (tobj)
	obj_to_obj (obj, tobj);
      else
	obj_to_char (obj, ch);
      bit = money;
    }

  send_to_char ("\n", ch);
  if (tobj)
    sprintf (buf,
	     "$N gives you the following coin, which you tuck away in #2%s#0:",
	     obj_short_desc (tobj));
  else
    sprintf (buf, "$N gives you the following coin:");

  act (buf, true, ch, 0, keeper, TO_CHAR | _ACT_FORMAT);

  *buf = '\0';

      if (mithril)
	sprintf (buf + strlen (buf), "   #2%d faceted, ridged gold coin%s#0\n",
		 mithril, mithril > 1 ? "s" : "");
      if (crown)
	sprintf (buf + strlen (buf),
		 "   #2%d eight-sided gold coin%s#0\n", crown,
		 crown > 1 ? "s" : "");
      if (tree)
	sprintf (buf + strlen (buf),
		 "   #2%d small, squared silver coin%s#0\n", tree,
		 tree > 1 ? "s" : "");
      if (royal)
	sprintf (buf + strlen (buf), "   #2%d small, squared silver coin%s#0\n",
		 royal, royal > 1 ? "s" : "");
      if (copper)
	sprintf (buf + strlen (buf),
		 "   #2%d weighty copper coin%s#0\n", copper,
		 copper > 1 ? "s" : "");
      if (bit)
	sprintf (buf + strlen (buf), "   #2%d tiny copper coin%s#0\n",
		 bit, bit > 1 ? "s" : "");

  send_to_char (buf, ch);
}

void
do_sell (CHAR_DATA * ch, char *argument, int cmd)
{
  int objs_in_storage;
  int sell_count = 1;
  int nobarter_flag;
  int language_barrier = 0;
  int keeper_success;
  int ch_success;
  int discount, same_obj = 0;
  float keepers_cost = 0;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  OBJ_DATA *obj;
  OBJ_DATA *tobj;
  CHAR_DATA *keeper = NULL;
  CHAR_DATA *tch;
  ROOM_DATA *room;
  NEGOTIATION_DATA *neg;
  bool resource = false;
  int i;

  argument = one_argument (argument, buf);

  room = ch->room;

  if (isdigit (*buf))
    {
      sell_count = atoi (buf);
      argument = one_argument (argument, buf);
      if (sell_count > MAX_INV_COUNT)
	{
	  sprintf (buf2,
		   "Sorry, but you can only sell up to %d items at a time.\n",
		   MAX_INV_COUNT);
	  send_to_char (buf2, ch);
	  return;
	}
    }

  if (!*buf)
    {
      send_to_char ("Sell what?\n", ch);
      return;
    }

  if ((keeper = get_char_room_vis (ch, buf)))
    argument = one_argument (argument, buf);
  else
    {
      for (tch = room->people; tch; tch = tch->next_in_room)
	if (tch != ch && IS_SET (tch->flags, FLAG_KEEPER))
	  break;

      keeper = tch;

      if (!*buf)
	{
	  act ("Sell what to $N?", true, ch, 0, keeper, TO_CHAR);
	  return;
	}
    }

  if (!keeper)
    {
      send_to_char ("There is no merchant here.\n", ch);
      return;
    }

  if (keeper == ch)
    {
      send_to_char ("You can't sell to yourself!\n", ch);
      return;
    }

  if (!keeper->shop || !IS_NPC (keeper))
    {
      act ("$N does not seem to be a shopkeeper.", true, ch, 0, keeper,
	   TO_CHAR | _ACT_FORMAT);
      return;
    }

  if (GET_POS (keeper) <= POSITION_SLEEPING)
    {
      act ("$N is not conscious.", true, ch, 0, keeper, TO_CHAR);
      return;
    }

  if (GET_POS (keeper) == POSITION_FIGHTING)
    {
      do_say (keeper, "Have you no eyes!  I'm fighting for my life!", 0);
      return;
    }

  if (!keeper->shop ||
      (keeper->shop->shop_vnum && keeper->shop->shop_vnum != ch->in_room))
    {
      do_say (keeper, "I'm sorry.  Please catch me when I'm in my shop.", 0);
      return;
    }

  if (!(room = vtor (keeper->shop->store_vnum)))
    {
      do_say (keeper, "I've lost my business.  You'll have to go elsewhere.",
	      0);
      return;
    }

  if (IS_SET (keeper->act, ACT_NOBUY))
    {
      do_say (keeper, "Sorry, but I don't deal in second-hand merchandise.",
	      0);
      return;
    }

  if (!room->psave_loaded)
    load_save_room (room);

  if (!GET_TRUST (ch) && !CAN_SEE (keeper, ch))
    {
      do_say (keeper, "Who's there?", 0);
      return;
    }

  argument = one_argument (argument, buf2);

  if(!str_cmp (buf2, "from") || !str_cmp (buf2, "on"))
  {
    argument = one_argument (argument, buf2);  
    if (!(tch = get_char_room_vis (ch, buf2)))
    {
      act ("Sell what from who?", true, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }
    
    if(!IS_SET(tch->affected_by, AFF_TRANSPORTING))
    {
      act ("$N isn't carrying anything that can be sold like that.", true, ch, 0, tch, TO_CHAR | _ACT_FORMAT);
      return;
    }
 
    if (!(obj = get_obj_in_list_vis (ch, buf, tch->equip)))
    {
      act ("You don't see that on $N.", true, ch, 0, tch, TO_CHAR); 
      return;
    }

    if(!is_leader (ch, tch) || GET_ITEM_TYPE(obj) != ITEM_RESOURCE)
    {
      act ("You can't take $p from $N.", true, ch, obj, tch, TO_CHAR); 
      return;
    }

    resource = true;
  }

  if(!resource)
  {
    if (!(obj = get_obj_in_list_vis (ch, buf, ch->right_hand)) &&
        !(obj = get_obj_in_list_vis (ch, buf, ch->left_hand)))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s %s", buf2, NO_SUCH_ITEM2);
      do_whisper (keeper, buf, 83);
      return;
    }
  }

  if (obj->count < sell_count)
    sell_count = obj->count;

  if (sell_count < 1)
    sell_count = 1;

  keepers_cost =
    calculate_sale_price (obj, keeper, ch, sell_count, true, true);

  if (IS_SET (keeper->act, ACT_BULKTRADER))
  {
    if(GET_ITEM_TYPE(obj) != ITEM_RESOURCE)
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s I only deal in bulk goods.", buf2);
      do_whisper (keeper, buf, 83);
      return;
    }
  }

  if (!trades_in (keeper, obj))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s %s", buf2, DO_NOT_BUY);
      do_whisper (keeper, buf, 83);
      return;
    }

  if ((GET_ITEM_TYPE (obj) == ITEM_LIGHT && !obj->o.od.value[1]))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s No, I wouldn't even think of buying that.", buf2);
      do_whisper (keeper, buf, 83);
      return;
    }

  if (IS_SET (ch->room->room_flags, LAWFUL) &&
      IS_SET (obj->obj_flags.extra_flags, ITEM_ILLEGAL))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s I can't buy that.  It's illegal to possess.", buf2);
      do_whisper (keeper, buf, 83);
      return;
    }

  if (vtor (keeper->shop->store_vnum))
    {

      objs_in_storage = 0;

      for (tobj = vtor (keeper->shop->store_vnum)->contents;
	   tobj; tobj = tobj->next_content)
	{
	  if (GET_ITEM_TYPE (tobj) != ITEM_MONEY
	      && IS_SET (tobj->obj_flags.extra_flags, ITEM_PC_SOLD))
	    objs_in_storage++;
	  if (tobj->nVirtual == obj->nVirtual)
	    same_obj += tobj->count;
	}

      if (((same_obj + sell_count) > MAX_INV_COUNT) 
          && (GET_ITEM_TYPE (obj) != ITEM_TOSSABLE  && GET_ITEM_TYPE (obj) != ITEM_RESOURCE))
	{
	  name_to_ident (ch, buf2);
	  sprintf (buf,
		   "%s I have quite enough of that for now, thank you; try back again later.",
		   buf2);
	  do_whisper (keeper, buf, 83);
	  return;
	}

      if (objs_in_storage > 125)
	{
	  name_to_ident (ch, buf2);
	  sprintf (buf,
		   "%s I have too much stuff as it is.  Perhaps you'd like to purchase something instead?",
		   buf2);
	  do_whisper (keeper, buf, 83);
	  return;
	}
    }

  if (cmd == 1)
    {				/* passed by barter command to do_sell */

      if (sell_count > 1)
	{
	  send_to_char ("You can only barter for one item at a time.\n", ch);
	  return;
	}

      name_to_ident (ch, buf);

      if (keepers_cost < 20)
	{
	  strcat (buf, " This isn't worth haggling over.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      nobarter_flag = index_lookup (econ_flags, "nobarter");

      if (nobarter_flag != -1 &&
	  IS_SET (keeper->shop->nobuy_flags, 1 << nobarter_flag))
	{
	  strcat (buf, " I'm sorry, but I will not haggle.  My prices "
		  "are fixed, take it or leave it.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      if (IS_SET (obj->obj_flags.extra_flags, ITEM_PC_SOLD))
	{
	  strcat (buf, " I won't haggle over a used piece of merchandse.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      if (nobarter_flag != -1 && IS_SET (obj->econ_flags, 1 << nobarter_flag))
	{
	  strcat (buf, " I'm sorry, but I will not haggle over the price "
		  "of this item.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      /* Search for existing entry in keepers negotiations list */

      for (neg = keeper->shop->negotiations; neg; neg = neg->next)
	{
	  if (neg->ch_coldload_id == ch->coldload_id &&
	      neg->obj_vnum == obj->nVirtual && !neg->true_if_buying)
	    break;
	}

      if (neg)
	{
	  if (neg->price_delta > 0)
	    {
	      strcat (buf, " No, no, I will not pay any higher a price.");
	      do_whisper (keeper, buf, 83);
	      return;
	    }

	  strcat (buf, " Listen, as much as I like you, I simply cannot "
		  "offer you what you're asking.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      /* keeper will be reluctant to sell to foreigners */
      language_barrier =
	(keeper->skills[ch->speaks] <
	 15) ? (15 - keeper->skills[ch->speaks]) : (0);
      keeper_success =
	combat_roll (MIN
		     (95, keeper->skills[SKILL_BARTER] + language_barrier), 0, 0);
      ch_success =
	combat_roll (MAX (5, ch->skills[SKILL_BARTER] - language_barrier), 0, 0);

      if (ch_success == SUC_CS && keeper_success == SUC_MS)
	discount = 5;
      else if (ch_success == SUC_CS && keeper_success == SUC_MF)
	discount = 10;
      else if (ch_success == SUC_CS && keeper_success == SUC_CF)
	discount = 15;

      else if (ch_success == SUC_MS && keeper_success == SUC_MS)
	discount = 0;
      else if (ch_success == SUC_MS && keeper_success == SUC_MF)
	discount = 5;
      else if (ch_success == SUC_MS && keeper_success == SUC_CF)
	discount = 10;

      else if (ch_success == SUC_MF && keeper_success == SUC_MS)
	discount = 0;
      else if (ch_success == SUC_MF && keeper_success == SUC_MF)
	discount = 0;
      else if (ch_success == SUC_MF && keeper_success == SUC_CF)
	discount = 5;

      else
	discount = 0;		/* A CF by ch */

      neg = (NEGOTIATION_DATA *) alloc (sizeof (NEGOTIATION_DATA), 40);
      neg->ch_coldload_id = ch->coldload_id;
      neg->obj_vnum = obj->nVirtual;
      neg->time_when_forgotten = time (NULL) + 6 * 60 * 60;	/* 6 hours */
      neg->price_delta = discount;
      neg->transactions = 0;
      neg->true_if_buying = 0;
      neg->next = keeper->shop->negotiations;
      keeper->shop->negotiations = neg;

      if (discount == 0)
	{
	  strcat (buf, " Sorry, but it's just not worth more than my "
		  "initial offer.");
	  do_whisper (keeper, buf, 83);
	  return;
	}

      else if (discount == 5)
	strcat (buf, " I've been looking for these.  It's a pleasure doing "
		"business with you.  I'll pay you ");
      else if (discount == 10)
	strcat (buf, " Perhaps if I go back to bed now, I can salvage some "
		"small part of my self respect.  I'll pay you ");
      else
	strcat (buf, " It is a dark day.  I'll have to sell my home and "
		"business just to recoup what I've lost this day."
		"  I'll give you ");

      keepers_cost = keepers_cost * (100 + discount) / 100;

      sprintf (buf + strlen (buf), "%d copper%s", (int) keepers_cost,
	       (int) keepers_cost > 1 ? "s" : "");

      strcat (buf, ".");

      do_whisper (keeper, buf, 83);

      return;
    } //end  if (cmd == 1) bartering

  keepers_cost = (int) keepers_cost;

  /* Look up negotiations for this ch/obj on keeper */

  for (neg = keeper->shop->negotiations; neg; neg = neg->next)
    {
      if (neg->ch_coldload_id == ch->coldload_id &&
	  neg->obj_vnum == obj->nVirtual && !neg->true_if_buying)
	break;
    }

  keepers_cost = (int) keepers_cost;

  if (keepers_cost < 1)
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s Bah, that isn't even worth my time!", buf2);
      do_whisper (keeper, buf, 83);
      return;
    }

  if (!keeper_has_money (keeper, (int) keepers_cost))
    {

      name_to_ident (ch, buf2);
      sprintf (buf, "%s %s", buf2, MISSING_CASH1);

      do_whisper (keeper, buf, 83);

      if (!IS_SET (keeper->act, ACT_PCOWNED) ||
	  !vtor (keeper->shop->store_vnum))
	return; /// \todo Flagged as unreachable

      return;
    }

  act ("$n sells $p.", false, ch, obj, 0, TO_ROOM);
  act ("You sell $p.", false, ch, obj, 0, TO_CHAR);
  send_to_char ("\n", ch);

  name_to_ident (ch, buf2);
  sprintf (buf, "%s Here's the amount we've agreed upon.", buf2);

  /* Pay customer */

  do_whisper (keeper, buf, 83);

  subtract_keeper_money (keeper, (int) keepers_cost);
  keeper_money_to_char (keeper, ch, (int) keepers_cost);

  obj_from_char (&obj, sell_count);
  int port = engine.get_port ();

  mysql_safe_query
    ("INSERT INTO %s.receipts "
     "(time, shopkeep, transaction, who, customer, vnum, "
     "item, qty, cost, room, gametime, port) "
     "VALUES (NOW(),%d,'bought','%s','%s',%d,'%s',%d,%f,%d,'%d-%d-%d %d:00',%d)",
     (engine.get_config ("player_log_db")).c_str (),
     keeper->mob->nVirtual, GET_NAME (ch), char_short (ch),
     obj->nVirtual, obj->short_description, obj->count, keepers_cost,
     keeper->in_room, time_info.year, time_info.month + 1,
     time_info.day + 1, time_info.hour, port);

  money_from_char_to_room (keeper, keeper->shop->store_vnum);

  if (keeper_makes (keeper, obj->nVirtual) && GET_ITEM_TYPE(obj) != ITEM_RESOURCE)
    extract_obj (obj);
  else
    {
      if(GET_ITEM_TYPE (obj) != ITEM_RESOURCE)
        obj->obj_flags.extra_flags |= ITEM_PC_SOLD;
      obj->sold_at = (int) time (0);
      obj->sold_by = ch->coldload_id;
      obj_to_room (obj, keeper->shop->store_vnum);
    }

  if (neg)
    if (!neg->transactions++)
      skill_use (ch, SKILL_BARTER, 0);
}

void
do_value (CHAR_DATA * ch, char *argument, int cmd)
{
  float keepers_cost = 0;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char buf3[MAX_STRING_LENGTH];
  OBJ_DATA *obj;
  ROOM_DATA *room;
  CHAR_DATA *keeper = NULL;
  CHAR_DATA *tch;

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Appraise what?\n", ch);
      return;
    }

  room = ch->room;

  if ((keeper = get_char_room_vis (ch, buf)))
    argument = one_argument (argument, buf);

  else
    {
      for (tch = room->people; tch; tch = tch->next_in_room)
	if (tch != ch && IS_SET (tch->flags, FLAG_KEEPER))
	  break;

      keeper = tch;

      if (!*buf)
	{
	  act ("Have $N appraise what?", true, ch, 0, keeper, TO_CHAR);
	  return;
	}
    }

  if (!keeper)
    {
      send_to_char ("There is no merchant here.\n", ch);
      return;
    }

  if (!keeper->shop)
    {
      act ("$N does not seem to be a shopkeeper.", true, ch, 0, keeper,
	   TO_CHAR | _ACT_FORMAT);
      return;
    }

  if (GET_POS (keeper) <= POSITION_SLEEPING)
    {
      act ("$N is not conscious.", true, ch, 0, keeper, TO_CHAR);
      return;
    }

  if (GET_POS (keeper) == POSITION_FIGHTING)
    {
      do_say (keeper, "Have you no eyes!  I'm fighting for my life!", 0);
      return;
    }

  if (!(obj = get_obj_in_list_vis (ch, buf, ch->right_hand)) &&
      !(obj = get_obj_in_list_vis (ch, buf, ch->left_hand)))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s %s", buf2, NO_SUCH_ITEM2);
      do_whisper (keeper, buf, 83);
      return;
    }

  if (!trades_in (keeper, obj))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s %s", buf2, DO_NOT_BUY);
      do_whisper (keeper, buf, 83);
      return;
    }

  if ((keepers_cost == 0 && GET_ITEM_TYPE (obj) == ITEM_MONEY))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s No, I wouldn't even think of buying that.", buf2);
      do_whisper (keeper, buf, 83);
      return;
    }

  if (!GET_TRUST (ch) && !CAN_SEE (keeper, ch))
    {
      do_say (keeper, "Who's there?", 0);
      return;
    }

  keepers_cost =
    calculate_sale_price (obj, keeper, ch, obj->count, true, true);

  name_to_ident (ch, buf2);

  *buf3 = '\0';

  sprintf (buf3, "%d copper%s", (int) keepers_cost,
	   (int) keepers_cost > 1 ? "s" : "");

  keepers_cost = (int) keepers_cost;

  if (!keepers_cost)
  {
    if(GET_ITEM_TYPE(obj) == ITEM_RESOURCE)
    {
      sprintf (buf, "%s I do not deal in those sort of resources.", buf2);
    }
    else
    {
      sprintf (buf, "%s I'm afraid that isn't even worth my time...", buf2);
    }
  }
  else
    sprintf (buf, "%s I'd buy %s for... %s.", buf2,
	     obj->count > 1 ? "those" : "that", buf3);

  do_whisper (keeper, buf, 83);
}

void
do_exchange (CHAR_DATA * ch, char *argument, int cmd)
{
  int count;
  CHAR_DATA *keeper;
  OBJ_DATA *coins;
  OBJ_DATA *new_coins;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf || *buf == '?' || !isdigit (*buf))
    {
      send_to_char ("> exchange 10 silver      Gives you 40 farthings\n", ch);
      send_to_char ("> exchange 24 farthings   Gives you 6 pennies\n", ch);
      return;
    }

  count = atoi (buf);

  if (!count)
    return;

  argument = one_argument (argument, buf);

  if (!(coins = get_obj_in_list_vis (ch, buf, ch->right_hand)) &&
      !(coins = get_obj_in_list_vis (ch, buf, ch->left_hand)))
    {
      send_to_char ("You don't have those.\n", ch);
      return;
    }

  if (coins->nVirtual != VNUM_PENNY && coins->nVirtual != VNUM_FARTHING)
    {
      send_to_char ("You can only exchange pennies or farthings.\n", ch);
      return;
    }

  if (coins->count < count)
    {
      send_to_char ("You don't have that many coins to exchange.\n", ch);
      return;
    }

  for (keeper = ch->room->people; keeper; keeper = keeper->next_in_room)
    if (keeper != ch && IS_SET (keeper->flags, FLAG_KEEPER))
      break;

  if (!keeper)
    {
      send_to_char
	("Sorry, there is no shopkeeper here to give you change.\n", ch);
      return;
    }

  if (coins->nVirtual == VNUM_PENNY)
    {
      new_coins = load_object (VNUM_FARTHING);
      new_coins->count = count * 4;
      name_money (new_coins);
      obj_to_char (new_coins, ch);

      if (count == coins->count)
	extract_obj (coins);
      else
	coins->count -= count;

      sprintf (buf, "$N takes your %d penn%s and gives you %d farthings.",
	       count, count > 1 ? "ies" : "y", count * 4);
      act (buf, false, ch, 0, keeper, TO_CHAR);
      act ("$N makes change for $n.", false, ch, 0, keeper, TO_NOTVICT);
      return;
    }

  /* Making change for farthings */

  count = count - (count % 4);	/* Round the farthings to pennies */

  if (!count)
    {
      act ("$N can't make change for less than 4 farthings.",
	   false, ch, 0, keeper, TO_CHAR);
      return;
    }

  new_coins = load_object (VNUM_PENNY);
  new_coins->count = count / 4;
  name_money (new_coins);
  obj_to_char (new_coins, ch);

  if (count == coins->count)
    extract_obj (coins);
  else
    coins->count -= count;

  sprintf (buf, "$N takes your %d farthings and gives you %d penn%s.",
	   count, count / 4, (count / 4) > 1 ? "ies" : "y");
  act (buf, false, ch, 0, keeper, TO_CHAR);
  act ("$N makes change for $n.", false, ch, 0, keeper, TO_NOTVICT);
}

void
do_barter (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];

  if (!real_skill (ch, SKILL_BARTER))
    {
      send_to_char
	("You're not convincing enough to barter, unfortunately.\n", ch);
      return;
    }
  argument = one_argument (argument, buf);

  if (is_abbrev (buf, "buy"))
    do_buy (ch, argument, 1);
  else if (is_abbrev (buf, "sell"))
    do_sell (ch, argument, 1);
  else
    send_to_char ("barter sell ITEM    or\n barter buy ITEM\n", ch);
}

/*
TODO: this command needs some work. some old code was never finished and the new code is different
> receipts for [<day>] [<month>] [<year>]
> receipts to <person>
> receipts from <object>
-------
> receipts  [summary] [r <rvnum>]

*/
void
do_receipts (CHAR_DATA * ch, char *argument, int cmd)
{
  short last_day = -1, last_month = -1, last_year = -1, nArgs = 0, nSeekDay =
    0, nSeekMonth = 0, nSeekYear = 0;
  int nAmtSold = 0, nAmtBought = 0, nTAmtSold = 0, nTAmtBought = 0;
  long day = -1, month = -1, year = -1;
  char *ptrFor = NULL, *ptrFrom = NULL, *ptrTo = NULL;
  CHAR_DATA *keeper = NULL;
  MYSQL_RES *result;
  MYSQL_ROW row;
  char args[3][AVG_STRING_LENGTH / 3] = { "", "", "" };
  char buf[MAX_STRING_LENGTH] = "";
  bool sumchk = false;
  //int shopnum = 0;
	
	/****** begin future options test statments ***/
  if (argument && argument[0])
    {
    if (!strncmp (argument, "for ", 4))
			{
	  	ptrFor = argument + 4;
			}
		else if ((ptrFor = strstr (argument, " for ")))
			{
	  	ptrFor = ptrFor + 5;
			}
    
    if (ptrFor)
			{
	  	nArgs = sscanf (ptrFor, "%s %s %s", args[0], args[1], args[2]);
	  	if (nArgs == 3	/* DD MONTH YYYY */
	      && isdigit (args[0][0]) && isdigit (args[2][0])
	      && (nSeekMonth = index_lookup (month_lkup, args[1])))
	    	{
	      nSeekYear = strtol (args[2], NULL, 10);
		    if (nSeekYear < time_info.year - 1
				  || nSeekYear > time_info.year)
					{
		  		nSeekYear = 0;
					}
	  		if (nSeekMonth < 1 || nSeekMonth > 12)
					{
				  nSeekMonth = 0;
					}
	    	else if (!nSeekYear)
					{
				  nSeekYear =
					    (nSeekMonth <=
		  		   time_info.month) ? time_info.year : time_info.year - 1;
					}
				nSeekDay = strtol (args[0], NULL, 10);
      	if (nSeekDay < 1 || nSeekDay > 31)
					{
				  nSeekDay = 0;
					}
	  		else if (!nSeekMonth)
					{
				  nSeekMonth = (nSeekDay <= time_info.day)
		    			? time_info.month
		    			: (((time_info.month - 1) >= 0)
		    			? (time_info.month - 1) : 11);
					}
				}//if (nArgs == 3
				
	  	else if (nArgs == 2	/* MONTH YYYY */
		   && (nSeekMonth = index_lookup (month_lkup, args[0]))
		   && isdigit (args[1][0]))
	    	{
	  		}
	  
	  	else if (nArgs == 2	/* DD MONTH */
		   && isdigit (args[0][0])
		   && (nSeekMonth = index_lookup (month_lkup, args[1])))
	    	{
	    	}
		}// if (ptrFor)

	if (!strncmp (argument, "from ", 5))
		{
	  ptrFrom = argument;
		}
  
  else if ((ptrFrom = strstr (argument, " from ")))
		{
	  ptrFrom = ptrFrom + 6;
		}

  if (!strncmp (argument, "to ", 3))
		{
	  ptrTo = argument;
		}
  else if ((ptrTo = strstr (argument, " to ")))
		{
	  ptrTo = ptrTo + 4;
		}

	if (!strncmp (argument, "summary", 7))
		{
		sumchk = true;
		}
	}// if (argument && argument[0])

	

  if (IS_NPC (ch) && ch->shop)
    {
      keeper = ch;
    }
  else
    {
      for (keeper = character_list; keeper; keeper = keeper->next)
	{
	  if (IS_NPC (keeper) && keeper->shop
	      && keeper->shop->store_vnum == ch->in_room)
	    break;
	}
    }
  if (keeper == NULL)
    {
      send_to_char ("You do not see a book of receipts here.\n", ch);
      return;
    }

  /* Detail */
  int port = engine.get_port ();

  mysql_safe_query
    ("SELECT time, shopkeep, transaction, who, customer, vnum, "
     "item, qty, cost, room, gametime, port, "
     "EXTRACT(YEAR FROM gametime) as year, "
     "EXTRACT(MONTH FROM gametime) as month, "
     "EXTRACT(DAY FROM gametime) as day "
     "FROM %s.receipts "
     "WHERE shopkeep = '%d' AND port = '%d' "
     "ORDER BY time DESC;", 
     (engine.get_config ("player_log_db")).c_str (),
     keeper->mob->nVirtual, port);

  if ((result = mysql_store_result (database)) == NULL)
    {
      send_to_gods ((char *) mysql_error (database));
      send_to_char ("The book of receipts is unavailable at the moment.\n",
		    ch);
      return;
    }

  send_to_char ("Examining a book of receipts:\n", ch);
  while ((row = mysql_fetch_row (result)) != NULL)
    {

      day = strtol (row[14], NULL, 10);
      month = strtol (row[13], NULL, 10) - 1;
      year = strtol (row[12], NULL, 10);
      if (day != last_day || month != last_month || year != last_year)
	{

	  if (last_day > 0 && month != last_month)
	    {
	      sprintf (buf + strlen (buf),
		       "\n    Total for #6%s %d#0: Sales #2%d cp#0, Purchases #2%d cp#0.\n\n",
		       month_short_name[(int) last_month], (int) last_year,
		       nAmtSold, nAmtBought);
	      /* send_to_char ( buf, ch ); */
	      nTAmtBought += nAmtBought;
	      nTAmtSold += nAmtSold;
	      nAmtBought = 0;
	      nAmtSold = 0;
	    }
	 
	 if (!sumchk)
	  	{
	  sprintf (buf + strlen (buf),
		   "\nOn #6%s %d#0:\n\n",
		   short_time_string(day, month), (int) year);
	  /* send_to_char ( buf, ch ); */
	  	}

	  last_day = day;
	  last_month = month;
	  last_year = year;

	}

      if (strcmp (row[2], "sold") == 0)
	{
	  nAmtSold += strtol (row[8], NULL, 10);
	}
      else if (strcmp (row[2], "bought") == 0)
	{
	  nAmtBought += strtol (row[8], NULL, 10);
	}
	  
      row[2][0] = toupper (row[2][0]);
      if (!sumchk)
      	{
	      sprintf (buf + strlen (buf),
		       "%s #2%s#0 of #2%s#0 %s #5%s#0 for #2%s cp#0.\n",
	  	     row[2], row[7], row[6], (row[2][0] == 's') ? "to" : "from",
	    	   (IS_NPC (ch) || IS_MORTAL (ch)) ? row[4] : row[3], row[8]);
      	/* send_to_char ( buf, ch ); */
      	}
      	
      if (strlen (buf) > MAX_STRING_LENGTH - 512)
				{
	  		strcat (buf,
		  		"\n#1There were more sales than could be displayed.#0\n\n");
	  		break;
				}
			}

  mysql_free_result (result);

  if (last_day > 0)
    {
      if ((nTAmtBought + nTAmtSold == 0) && (nAmtSold + nAmtBought > 0))
	{
	  sprintf (buf + strlen (buf),
		   "\n    Total for #6%s %d#0: Sales #2%d cp#0, Purchases #2%d cp#0.\n\n"
		   "    Current coin on hand: #2%d cp#0.\n",
		   month_short_name[(int) last_month], (int) last_year,
		   nAmtSold, nAmtBought, keeper_has_money (keeper, 0));
	}
      else
	{
	  sprintf (buf + strlen (buf),
		   "\n    Total for #6%s %d#0: Sales #2%d cp#0, Purchases #2%d cp#0.\n\n"
		   "    Total for period:    Sales #2%d cp#0, Purchases #2%d cp#0.\n"
		   "    Current coin on hand:  #2%d cp#0.\n",
		   month_short_name[(int) last_month], (int) last_year,
		   nAmtSold, nAmtBought, nTAmtSold, nTAmtBought,
		   keeper_has_money (keeper, 0));
	}
      page_string (ch->desc, buf);
    }
}

int
get_uniq_ticket (void)
{
  int tn = 1;
  int i;
  FILE *fp_ls;
  char buf[MAX_STRING_LENGTH];

  if (!(fp_ls = popen ("ls tickets", "r")))
    {
      system_log ("The ticket system is broken, get_uniq_ticket()", true);
      return -1;
    }

  /* The TICKET_DIR should be filled with files that have seven
     digit names (zero padded on the left).
   */

  while (!feof (fp_ls))
    {

      if (!fgets (buf, 80, fp_ls))
	break;

      for (i = 0; i < 7; i++)
	if (!isdigit (buf[i]))
	  continue;

      if (tn != atoi (buf))
	break;

      tn = atoi (buf) + 1;
    }

  pclose (fp_ls);

  return tn;
}

void unhitch_char (CHAR_DATA * ch, CHAR_DATA * hitch);
void
do_stable (CHAR_DATA * ch, char *argument, int cmd)
{
  int ticket_num, i = 0;
  CHAR_DATA *animal = NULL;
  CHAR_DATA *new_hitch;
  CHAR_DATA *keeper;
  AFFECTED_TYPE *af;
  OBJ_DATA *ticket;
  FILE *fp;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  bool paid_for = false;

  for (keeper = ch->room->people; keeper; keeper = keeper->next_in_room)
    if (keeper != ch &&
	keeper->mob &&
	IS_SET (keeper->flags, FLAG_KEEPER) &&
	IS_SET (keeper->flags, FLAG_OSTLER))
      break;

  if (!keeper)
    {
      send_to_char ("There is no ostler here.\n", ch);
      return;
    }

  if (!ch->hitchee || !is_he_here (ch, ch->hitchee, 0))
    {
      send_to_char ("You have no hitch.\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (*buf)
    {
      if (!(animal = get_char_room_vis (ch, buf)))
	{
	  send_to_char ("There isn't such an animal here.\n", ch);
	  return;
	}

      if (animal != ch->hitchee)
	{
	  act ("$N isn't hitched to you.", false, ch, 0, animal, TO_CHAR);
	  return;
	}
    }

  /* Make sure mount isn't already mounted */

  if (ch->hitchee->mount && is_he_here (ch, ch->hitchee->mount, 0))
    {
      name_to_ident (ch->hitchee->mount, buf2);
      sprintf (buf, "tell %s Have #5%s#0 dismount your hitch first.",
	       buf2, char_short (ch->hitchee->mount));
      command_interpreter (keeper, buf);
    }

  animal = ch->hitchee;

  i = MAGIC_STABLING_PAID;
  while ((af = get_affect (ch, i)))
    {
      if (af->a.spell.sn == animal->coldload_id)
	{
	  paid_for = true;
	  break;
	}
      i++;
      if (i > MAGIC_STABLING_LAST)
	{
	  i = MAGIC_STABLING_PAID;
	  break;
	}
    }

  if (!paid_for && !is_brother (ch, keeper)
      && !can_subtract_money (ch, 20, keeper->mob->currency_type))
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "%s You seem to be a bit short on coin right now.", buf2);
      do_whisper (keeper, buf, 83);
      return;
    }

  if (!paid_for)
    {
      CREATE (af, AFFECTED_TYPE, 1);
      af->type = i;
      af->a.spell.sn = animal->coldload_id;
      af->a.spell.duration = 168;
      affect_to_char (ch, af);
      if (!is_brother (ch, keeper))
	subtract_money (ch, 20, keeper->mob->currency_type);
    }

  if ((ticket_num = get_uniq_ticket ()) == -1)
    {
      send_to_char ("OOC:  The ticket system is broken.  Sorry.\n", ch);
      return;
    }

  unhitch_char (ch, animal);

  /* Take reigns of hitches hitch, if chained */

  if (animal->hitchee && is_he_here (animal, animal->hitchee, 0))
    {

      new_hitch = animal->hitchee;
      unhitch_char (animal, new_hitch);

      if (hitch_char (ch, new_hitch))
	act ("You take the reigns of $N.", false, ch, 0, new_hitch, TO_CHAR);
    }

  ticket = load_object (VNUM_TICKET);

  if (!ticket)
    {
      send_to_char ("OOC:  Your hitch could not be saved.  Please report "
		    "this as a problem.\n", ch);
      return;
    }

  ticket->o.ticket.ticket_num = ticket_num;
  ticket->o.ticket.keeper_vnum = keeper->mob->nVirtual;
  ticket->o.ticket.stable_date = (int) time (0);

  obj_to_char (ticket, ch);

  sprintf (buf, "The number %d is scrawled on this ostler's ticket.",
	   ticket->o.ticket.ticket_num);
  sprintf (buf + strlen (buf),
	   "\n\n#6OOC: To retrieve your mount, GIVE this ticket to the ostler\n"
	   "     with whom you stabled it; be sure you don't lose this!#0");
  ticket->full_description = str_dup (buf);

  act ("$N gives you $p.", false, ch, ticket, keeper, TO_CHAR | _ACT_FORMAT);
  act ("You give $N $p.", false, keeper, ticket, ch, TO_CHAR | _ACT_FORMAT);
  act ("$N gives $n $p.", false, ch, ticket, keeper,
       TO_NOTVICT | _ACT_FORMAT);

  act ("$N leads $n to the stables.",
       false, animal, 0, keeper, TO_ROOM | _ACT_FORMAT);

  sprintf (buf, TICKET_DIR "/%07d", ticket_num);

  if (!(fp = fopen (buf, "w")))
    {
      perror (buf);
      system_log ("Unable to save ticketed mobile to file.", true);
      return;
    }

  save_mobile (animal, fp, "HITCH", 1);	/* Extracts the mobile */

  fclose (fp);
}

void
unstable (CHAR_DATA * ch, OBJ_DATA * ticket, CHAR_DATA * keeper)
{
  CHAR_DATA *back_hitch = NULL;
  CHAR_DATA *animal;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char ticket_file[MAX_STRING_LENGTH];

  if (!keeper)
    {
      for (keeper = ch->room->people; keeper; keeper = keeper->next_in_room)
	if (keeper != ch &&
	    keeper->mob &&
	    IS_SET (keeper->flags, FLAG_KEEPER) &&
	    IS_SET (keeper->flags, FLAG_OSTLER))
	  break;

      if (!keeper)
	{
	  send_to_char ("There is no ostler here.\n", ch);
	  return;
	}
    }

  if (!keeper->mob)
    {				/* Can happen if given specifically to a PC */
      send_to_char ("Only NPCs can be ostlers.\n", ch);
      return;
    }

  if (!CAN_SEE (keeper, ch))
    {
      act ("It appears that $N can't see you.",
	   false, ch, 0, keeper, TO_CHAR);
      return;
    }

  if (ticket->o.ticket.keeper_vnum != keeper->mob->nVirtual)
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "tell %s Sorry, that's not one of my tickets.", buf2);
      command_interpreter (keeper, buf);
      return;
    }

  sprintf (ticket_file, TICKET_DIR "/%07d", ticket->o.ticket.ticket_num);

  if (stat (ticket_file, (struct stat *) buf2) == -1)
    {
      name_to_ident (ch, buf2);
      sprintf (buf, "tell %s Yeah, that's my ticket, but I don't have "
	       "anyting in that stall.", buf2);
      command_interpreter (keeper, buf);
      return;
    }

  if (is_he_here (ch, ch->hitchee, 0))
    {
      back_hitch = ch->hitchee;
      unhitch_char (ch, back_hitch);
    }

  animal = load_saved_mobiles (ch, ticket_file);
  if (ticket->o.od.value[0])
    offline_healing (animal, ticket->o.ticket.stable_date);

  if (!animal)
    {
      send_to_char ("OOC:  See an admin.  There is something wrong with that "
		    "ticket.\n", ch);
      if (back_hitch)
	hitch_char (ch, back_hitch);

      return;
    }

  if (back_hitch && !ch->hitchee)
    hitch_char (ch, back_hitch);
  else if (back_hitch)
    hitch_char (ch->hitchee, back_hitch);

  extract_obj (ticket);

  save_char (ch, true);
  unlink (ticket_file);

  act ("$N trots $3 from the stables and hands you the reins.",
       false, ch, (OBJ_DATA *) animal, keeper, TO_CHAR | _ACT_FORMAT);
  act ("$N trots $3 from the stables and hands $n the reins.",
       false, ch, (OBJ_DATA *) animal, keeper, TO_NOTVICT | _ACT_FORMAT);
  act ("You trot $3 from the stables and hand $N the reins.",
       false, keeper, (OBJ_DATA *) animal, ch, TO_CHAR | _ACT_FORMAT);
}

void
do_pay2 (CHAR_DATA * ch, char *argument, int cmd)
{
  int seen_tollkeeper = 0;
  int dir = 0;
  int max_toll;
  int num_crossers = 0;
  int tolls;
  int i;
  CHAR_DATA *crosser = NULL;
  CHAR_DATA *cross_list[50];
  CHAR_DATA *tch = NULL;
  AFFECTED_TYPE *af = NULL;
  AFFECTED_TYPE *taf;
  char buf[MAX_STRING_LENGTH];

  /* pay
     pay [<char | direction>] # [all | char]
   */

/* Help:

PAY

Pay a toll to a tollkeeper.  If a tollkeeper prevents travel along a path,
he can be paid a toll to allow passage.  You can pay a toll for yourself,
for others, or for all those following you.

   > pay                 Shows the amount of toll being collected
   > pay #               Pay the toll as long as it is '#' or less.
   > pay <direction> #   Pay the toll to the tollkeeper who guards a
                         specific direction, but not more than '#'.
   > pay <char> #        Pay the toll to the tollkeeper named <char>,
                         but not more than '#'.

   > pay <char>        "all"    Pay <char> or character who guards <direction>
         <direction> # <char>   a max toll of '#' for <char> or everyone (all)
                                following you.

Examples:

   > pay
   > pay north 5 all
   > pay troll 10 guard

For convenience, the SET command can be used to set AUTOTOLL.  If you follow
another character, you will automatically pay your toll (if it hasn't been
paid for you) up to a limit of your AUTOTOLL value.  Example of AUTOTOLL:

   > set autotoll 5     When following, automatically pay tolls up to 5.

The TOLL command can be used to setup a toll crossing, but only in areas setup
to support tolls.

See also:  SET, TOLL

*/

  if (!is_human (ch))
    {
      send_to_char ("Non-humans don't have to pay toll.\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (!*buf)
    {

      for (tch = ch->room->people; tch; tch = tch->next_in_room)
	{

	  if (tch->deleted)
	    continue;

	  if (tch == ch)
	    continue;

	  if (!(af = get_affect (tch, MAGIC_TOLL)))
	    continue;

	  if (af->a.toll.room_num != ch->in_room)
	    continue;

	  seen_tollkeeper = 1;

	  sprintf (buf, "$n wants %d copper%s to cross %s.",
		   af->a.toll.charge,
		   af->a.toll.charge == 1 ? "" : "s", dirs[af->a.toll.dir]);
	  act (buf, false, tch, 0, ch, TO_VICT);
	}

      if (!seen_tollkeeper)
	send_to_char ("There are no toll keepers here.\n", ch);

      return;
    }

  if (!just_a_number (buf))
    {				/* pay ch | dir */

      if (!str_cmp (buf, "all"))
	{
	}

      else if ((tch = get_char_room_vis (ch, buf)))
	{			/* pay <ch> */

	  if (!(af = get_affect (tch, MAGIC_TOLL)))
	    {
	      act ("$N isn't collecting tolls.", false, ch, 0, tch, TO_CHAR);
	      return;
	    }

	  dir = af->a.toll.dir;
	}

      else
	{

	  if ((dir = index_lookup (dirs, buf)) == -1)
	    {			/* pay dir */
	      send_to_char ("No such tollkeeper or direction.\n", ch);
	      return;
	    }

	  /* Who is the char covering this direction? */

	  for (tch = ch->room->people; tch; tch = tch->next_in_room)
	    {

	      if (tch->deleted ||
		  tch == ch ||
		  !(af = get_affect (tch, MAGIC_TOLL)) ||
		  af->a.toll.room_num != ch->in_room)
		continue;

	      if (dir == af->a.toll.dir)
		break;
	    }

	  if (!tch)
	    {
	      send_to_char ("There is nobody there taking tolls.\n", ch);
	      return;
	    }
	}

      argument = one_argument (argument, buf);
    }

  if (!just_a_number (buf))
    {
      act ("How much are you willing to pay $N?", false, ch, 0, tch, TO_CHAR);
      return;
    }

  /* if pay #, then we still have to find someone to pay off */

  if (!tch)
    {
      for (tch = ch->room->people; tch; tch = tch->next_in_room)
	{

	  if (tch->deleted ||
	      tch == ch ||
	      !(af = get_affect (tch, MAGIC_TOLL)) ||
	      af->a.toll.room_num != ch->in_room)
	    continue; /// \todo Flagges as unreachable

	  break;
	}

      if (!tch)
	{
	  send_to_char ("Nobody is collecting tolls here.\n", ch);
	  return;
	}

      dir = af->a.toll.dir;
    }

  /* Collect the money */

  af = get_affect (tch, MAGIC_TOLL);

  if (!af)
    {
      send_to_char ("Something is wrong.  Please contact an admin.\n", ch);
      system_log ("No affect on tch. Expected a toll affect.", true);
      return;
    }

  if (!can_subtract_money (ch, af->a.toll.charge, tch->mob->currency_type))
    {
      send_to_char ("You don't have enough coin to pay the toll.\n", ch);
      return;
    }

  max_toll = atoi (buf);

  argument = one_argument (argument, buf);

  if (!*buf)
    cross_list[num_crossers++] = ch;

  else if (!str_cmp (buf, "all"))
    {				/* Pay for followers */

      cross_list[num_crossers++] = ch;

      for (crosser = ch->room->people; tch; tch = tch->next_in_room)
	{

	  if (crosser == ch)
	    continue;

	  if (crosser->following != ch || !CAN_SEE (ch, crosser))
	    continue;

	  cross_list[num_crossers] = crosser;
	  num_crossers++;
	}
    }

  else
    {
      while (*buf)
	{
	  if (!(crosser = get_char_room_vis (ch, buf)))
	    {
	      send_to_char ("Pay toll for whom?\n", ch);
	      return;
	    }

	  cross_list[num_crossers++] = crosser;

	  argument = one_argument (argument, buf);
	}
    }

  /* How many tolls will ch pay? */

  tolls = 0;

  for (i = 0; i < num_crossers; i++)
    {

      if ((taf = get_affect (cross_list[i], MAGIC_TOLL_PAID)) &&
	  taf->a.toll.dir == dir && taf->a.toll.room_num == ch->in_room)
	continue;

      tolls++;
    }

  if (tolls <= 0)
    {
      send_to_char ("Nobody needs a toll paid.\n", ch);
      return;
    }

  if (max_toll < af->a.toll.charge)
    {
      sprintf (buf, "$N wants a %d copper toll.", af->a.toll.charge);
      act (buf, false, ch, 0, tch, TO_CHAR);
      return;
    }

  if (!can_subtract_money
      (ch, af->a.toll.charge * tolls, tch->mob->currency_type))
    {
      send_to_char ("You don't have enough coin to pay the tolls.\n", ch);
      return;
    }

  for (i = 0; i < num_crossers; i++)
    {

      if ((taf = get_affect (cross_list[i], MAGIC_TOLL_PAID)) &&
	  taf->a.toll.dir == dir && taf->a.toll.room_num == ch->in_room)
	{

	  if (num_crossers == 1)
	    {
	      if (cross_list[i] == ch)
		send_to_char ("You've already paid for your toll.\n", ch);
	      else
		act ("$3 has already accepted a toll for $N.",
		     false, ch, (OBJ_DATA *) tch, cross_list[i], TO_CHAR);
	    }

	  continue;

	}

      magic_add_affect (ch, MAGIC_TOLL_PAID, -1, 0, 0, 0, 0);

      taf = get_affect (ch, MAGIC_TOLL_PAID);

      taf->a.toll.dir = dir;
      taf->a.toll.room_num = ch->in_room;

      if (cross_list[i] != ch)
	{
	  act ("$n pays $N a toll for $3.",
	       true, ch, (OBJ_DATA *) cross_list[i], tch, TO_NOTVICT);
	  act ("You pay a toll to $N for $3.",
	       false, ch, (OBJ_DATA *) tch, cross_list[i], TO_CHAR);
	}

      else
	{
	  sprintf (buf, "You pay $N a %d penny toll.", af->a.toll.charge);
	  act (buf, false, ch, 0, tch, TO_CHAR);
	  act ("$n pays $N a toll.", true, ch, 0, tch, TO_ROOM);
	}
    }

  subtract_money (ch, af->a.toll.charge * num_crossers,
		  tch->mob->currency_type);

  act ("$n pays you a toll.", false, ch, 0, tch, TO_VICT);
}

int
is_toll_paid (CHAR_DATA * ch, int dir)
{
  CHAR_DATA *collector;
  AFFECTED_TYPE *af;

  if (!is_human (ch))
    return 1;

  if (!(collector = toll_collector (ch->room, dir)))
    return 1;

  if (is_brother (collector, ch))
    return 1;

  if (!CAN_SEE (collector, ch))
    return 1;

  if (!(af = get_affect (ch, MAGIC_TOLL_PAID)))
    return 0;

  if (af->a.toll.dir == dir && af->a.toll.room_num == ch->in_room)
    return 1;

  return 0;
}

void
do_pay (CHAR_DATA * ch, char *argument, int cmd)
{
  /* pay
     pay [<char | direction>] # [all | char]
   */

  int max_pay = 0;
  int num_payees = 0;
  int dir = -1;
  int seen_tollkeeper = 0;
  int currency_type = 0;
  int i;
  CHAR_DATA *collector;
  CHAR_DATA *tch;
  CHAR_DATA *payees[50];
  AFFECTED_TYPE *af;
  AFFECTED_TYPE *af_collector;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      for (dir = 0; dir <= LAST_DIR; dir++)
	{
	  if (!(tch = toll_collector (ch->room, dir)) || tch == ch)
	    continue;

	  af = get_affect (tch, MAGIC_TOLL);

	  seen_tollkeeper = 1;

	  sprintf (buf, "$n wants %d penn%s to cross %s.",
		   af->a.toll.charge,
		   af->a.toll.charge == 1 ? "y" : "ies",
		   dirs[af->a.toll.dir]);
	  act (buf, false, tch, 0, ch, TO_VICT);
	}

      if (!seen_tollkeeper)
	send_to_char ("There are no toll keepers here.\n", ch);

      return;
    }

  if ((dir = index_lookup (dirs, buf)) != -1)
    {
      collector = toll_collector (ch->room, dir);
      if (!collector)
	{
	  send_to_char ("There is nobody here collecting tolls that way.\n",
			ch);
	  return;
	}
    }

  else if ((collector = get_char_room_vis (ch, buf)))
    {
      if (!(af = get_affect (collector, MAGIC_TOLL)))
	{
	  act ("$N isn't collecting a toll.\n",
	       false, ch, 0, collector, TO_CHAR);
	  return;
	}

      dir = af->a.toll.dir;
    }

  else if (!just_a_number (buf))
    {
      send_to_char ("You don't see that person collecting tolls.\n", ch);
      return;
    }

  else
    {				/* A number WITHOUT a char/dir */
      max_pay = atoi (buf);

      for (dir = 0; dir <= LAST_DIR; dir++)
	if ((collector = toll_collector (ch->room, dir)))
	  break;

      if (!collector)
	{
	  send_to_char ("Nobody is collecting tolls here.\n", ch);
	  return;
	}
    }

  if (GET_POS (collector) <= SLEEP)
    {
      act ("$N isn't conscious.", false, ch, 0, collector, TO_CHAR);
      return;
    }

  if (GET_POS (collector) == FIGHT)
    {
      act ("$N is fighting!", false, ch, 0, collector, TO_CHAR);
      return;
    }

  /* We know dir and who to pay now (collector).  If we don't know
     how much to pay, figure that out. */

  if (!max_pay)
    {
      argument = one_argument (argument, buf);

      if (!just_a_number (buf))
	{
	  send_to_char ("How much are you willing to pay?\n", ch);
	  return;
	}

      max_pay = atoi (buf);
    }

  af_collector = get_affect (collector, MAGIC_TOLL);

  argument = one_argument (argument, buf);

  if (!str_cmp (buf, "for"))
    argument = one_argument (argument, buf);

  if (!*buf)
    {

      if ((af = get_affect (ch, MAGIC_TOLL_PAID)) && af->a.toll.dir == dir)
	{
	  send_to_char ("You already have permission to cross.\n", ch);
	  return;
	}

      payees[num_payees++] = ch;
    }

  else if (!str_cmp (buf, "all"))
    {

      for (tch = ch->room->people; tch; tch = tch->next_in_room)
	{
	  if (!is_human (tch))
	    continue;

	  if (tch != ch)
	    if (tch->deleted || tch->following != ch || !CAN_SEE (ch, tch))
	      continue;

	  if ((af = get_affect (tch, MAGIC_TOLL_PAID)) &&
	      af->a.toll.dir == dir)
	    continue;

	  payees[num_payees++] = tch;
	}

      if (!num_payees)
	{
	  send_to_char ("There is nobody to pay for.\n", ch);
	  return;
	}
    }

  else if ((tch = get_char_room_vis (ch, buf)))
    {

      if (!is_human (tch))
	{
	  send_to_char ("Only humans need pay tolls.\n", ch);
	  return;
	}

      if ((af = get_affect (tch, MAGIC_TOLL_PAID)) && af->a.toll.dir == dir)
	{
	  act ("$N already has permission to cross.",
	       false, ch, 0, tch, TO_CHAR);
	  return;
	}

      payees[num_payees++] = tch;
    }

  else
    {
      send_to_char ("Pay for whom?\n", ch);
      return;
    }

  if (af_collector->a.toll.charge > max_pay)
    {
      sprintf (buf, "$N wants %d copper%s per toll.",
	       af_collector->a.toll.charge,
	       af_collector->a.toll.charge == 1 ? "" : "s");
      act (buf, false, ch, 0, collector, TO_CHAR);
      return;
    }

  if (!collector->mob)
    currency_type = 0;
  else if (collector->mob->currency_type > 1)
    currency_type = 0;
  else
    currency_type = 1;

  if (!can_subtract_money
      (ch, af_collector->a.toll.charge * num_payees, currency_type))
    {
      send_to_char ("You don't have enough coin to pay the toll.\n", ch);
      return;
    }

  act ("$n pays $N a toll for:", false, ch, 0, collector, TO_NOTVICT);

  sprintf (buf, "$n pays you a toll of %d copper%s for:",
	   af_collector->a.toll.charge * num_payees,
	   af_collector->a.toll.charge * num_payees == 1 ? "" : "s");
  act (buf, false, ch, 0, collector, TO_VICT);

  sprintf (buf, "You pay $N a toll of %d copper%s for:",
	   af_collector->a.toll.charge * num_payees,
	   af_collector->a.toll.charge * num_payees == 1 ? "" : "s");
  act (buf, false, ch, 0, collector, TO_CHAR);

  for (i = 0; i < num_payees; i++)
    {
      if (ch == payees[i])
	act ("   yourself", false, ch, 0, 0, TO_CHAR);
      else
	act ("   you", false, payees[i], 0, 0, TO_CHAR);

      act ("   $n", false, payees[i], 0, ch, TO_ROOM);

      magic_add_affect (payees[i], MAGIC_TOLL_PAID, -1, 0, 0, 0, 0);

      af = get_affect (payees[i], MAGIC_TOLL_PAID);
      af->a.toll.dir = dir;
      af->a.toll.room_num = ch->in_room;
    }

  subtract_money (ch, af_collector->a.toll.charge * num_payees,
		  currency_type);
}

void
stop_tolls (CHAR_DATA * ch)
{
  AFFECTED_TYPE *af;

  if (!(af = get_affect (ch, MAGIC_TOLL)))
    return;

  if (af->a.toll.room_num == ch->in_room)
    {
      send_to_char ("You stop collecting tolls.\n", ch);
      act ("$n stops collecting tolls.", true, ch, 0, 0, TO_ROOM);
    }

  remove_affect_type (ch, MAGIC_TOLL);
}

CHAR_DATA *
toll_collector (ROOM_DATA * room, int dir)
{
  CHAR_DATA *tch;
  AFFECTED_TYPE *af;

  for (tch = room->people; tch; tch = tch->next_in_room)
    {

      if (tch->deleted)
	continue;

      if (!(af = get_affect (tch, MAGIC_TOLL)))
	continue;

      if (af->a.toll.dir != dir || af->a.toll.room_num != room->nVirtual)
	continue;

      return tch;
    }

  return NULL;
}

void
do_toll (CHAR_DATA * ch, char *argument, int cmd)
{
  int dir;
  CHAR_DATA *tch;
  AFFECTED_TYPE *af;
  ROOM_DIRECTION_DATA *exit;
  char buf[MAX_STRING_LENGTH];

/*

Help:

TOLL

Setup a toll crossing.  Any character may setup a toll crossing in an area
that supports tolls.  All humans are stopped and expected to pay a toll of
your choosing.

   > toll <direction> <amount to collect>

Example:

   > toll north 5         Setup a toll crossing to the North.  The toll is
                          5 coins.

Non-humans are not expected to pay tolls, since they usually don't have money.
People who you cannot see can pass without paying toll, if they choose.

See also:  PAY, SET

*/

  argument = one_argument (argument, buf);

  if (!*buf || *buf == '?')
    {
      send_to_char ("toll <direction> <amount to collect>\n", ch);
      return;
    }

  if ((dir = index_lookup (dirs, buf)) == -1)
    {
      send_to_char ("Expected a direction:  North, South, East, or West.\n"
		    "toll <direction> <amount to collect>\n", ch);
      return;
    }

  if ((tch = toll_collector (ch->room, dir)) && tch != ch)
    {
      act ("$N is already collecting a toll here.\n",
	   false, ch, 0, tch, TO_CHAR);
      return;
    }

  if (!(exit = EXIT (ch, dir)))
    {
      send_to_char ("There is no exit there.\n", ch);
      return;
    }

  if (!IS_SET (exit->exit_info, EX_TOLL))
    {
      send_to_char ("It isn't possible to collect tolls in that direction.\n",
		    ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (!just_a_number (buf) || atoi (buf) < 1)
    {
      send_to_char ("How much will your charge for others for the toll?\n"
		    "toll <direction> <amount to collect>\n", ch);
      return;
    }

  if (tch)
    {				/* We just want to modify direction or toll charge */
      af = get_affect (ch, MAGIC_TOLL);

      af->a.toll.dir = dir;
      af->a.toll.charge = atoi (buf);
      af->a.toll.room_num = ch->in_room;

      sprintf (buf, "You will collect %d penn%s when people pass %s.\n",
	       af->a.toll.charge, af->a.toll.charge > 1 ? "ies" : "y",
	       dirs[af->a.toll.dir]);
      send_to_char (buf, ch);

      return;
    }

  magic_add_affect (ch, MAGIC_TOLL, -1, 0, 0, 0, 0);

  af = get_affect (ch, MAGIC_TOLL);

  af->a.toll.dir = dir;
  af->a.toll.charge = atoi (buf);
  af->a.toll.room_num = ch->in_room;

  sprintf (buf, "You will collect %d penn%s when people pass %s.\n",
	   af->a.toll.charge, af->a.toll.charge > 1 ? "ies" : "y",
	   dirs[af->a.toll.dir]);
  send_to_char (buf, ch);

  sprintf (buf, "$n stands %s, ready to collect tolls.", dirs[dir]);
  act (buf, true, ch, 0, 0, TO_ROOM);
}

int
can_subtract_money (CHAR_DATA * ch, int farthings_to_subtract,
		    int currency_type)
{
  OBJ_DATA *obj, *tobj;
  int money = 0, location;

  if ((obj = ch->right_hand))
    {
      if (GET_ITEM_TYPE (obj) == ITEM_MONEY)
	{
	  //if (currency_type == CURRENCY_ORKISH)
          if (obj->nVirtual >= 990 && obj->nVirtual <= 995)
		money += ((int) ch->right_hand->farthings) * ch->right_hand->count;
	}
      else if (GET_ITEM_TYPE (obj) == ITEM_CONTAINER)
	{
	  for (tobj = obj->contains; tobj; tobj = tobj->next_content)
	    {
	      if (GET_ITEM_TYPE (tobj) == ITEM_MONEY)
		{
  	          if (tobj->nVirtual >= 990 && tobj->nVirtual <= 995)
		    money += ((int) tobj->farthings) * tobj->count;
		}
	    }
	}
    }

  if ((obj = ch->left_hand))
    {
      if (GET_ITEM_TYPE (obj) == ITEM_MONEY)
	{
          if (obj->nVirtual >= 990 && obj->nVirtual <= 995)
	    money += ((int) ch->left_hand->farthings) * ch->left_hand->count;
	}
      else if (GET_ITEM_TYPE (obj) == ITEM_CONTAINER)
	{
	  for (tobj = obj->contains; tobj; tobj = tobj->next_content)
	    {
	      if (GET_ITEM_TYPE (tobj) == ITEM_MONEY)
		{
  	          if (tobj->nVirtual >= 990 && tobj->nVirtual <= 995)
		    money += ((int) tobj->farthings) * tobj->count;
		}
	    }
	}
    }

  for (location = 0; location < MAX_WEAR; location++)
    {
      if (!(obj = get_equip (ch, location)))
	continue;
      if (GET_ITEM_TYPE (obj) != ITEM_CONTAINER)
	continue;
      if (IS_SET (obj->o.container.flags, CONT_CLOSED))
	continue;
      for (tobj = obj->contains; tobj; tobj = tobj->next_content)
	{
	  if (GET_ITEM_TYPE (tobj) == ITEM_MONEY)
	    {
	      //if (currency_type == CURRENCY_ORKISH)
	      if (tobj->nVirtual >= 990 && tobj->nVirtual <= 995)
	        money += ((int) tobj->farthings) * tobj->count;
	    }
	}
    }

  if (money < farthings_to_subtract)
    return 0;

  return 1;
}
// If you give the subtract_money function a negative farthings_to_subtract, it will supress all output to the player
void
subtract_money (CHAR_DATA * ch, int farthings_to_subtract, int currency_type)
{
  char buf[MAX_STRING_LENGTH];
  OBJ_DATA *tobj, *obj, *next_obj;
  bool change = false;
  int money = 0, location;
  bool container = false;
  bool SupressOutput = false; // Japheth's addition

  if (farthings_to_subtract < 0)
  {
	  farthings_to_subtract *= -1;
	  SupressOutput = true;
  }

  for (location = 0; location < MAX_WEAR; location++)
    {
      if (!(obj = get_equip (ch, location)))
	continue;
      if (GET_ITEM_TYPE (obj) != ITEM_CONTAINER)
	continue;
      if (IS_SET (obj->o.container.flags, CONT_CLOSED))
	continue;
      for (tobj = obj->contains; tobj; tobj = next_obj)
	{
	  next_obj = tobj->next_content;
	  if (GET_ITEM_TYPE (tobj) == ITEM_MONEY)
	    {
	      if ((currency_type == CURRENCY_NOLDO
  	       && (tobj->nVirtual >= 990 && tobj->nVirtual <= 995)))
		{
		  money += ((int) tobj->farthings) * tobj->count;
		  obj_from_obj (&tobj, 0);
		  extract_obj (tobj);
		  ch->delay_obj = obj;
		  container = true;
		}
	    }
	}
    }

  if ((obj = ch->right_hand))
    {
      if (GET_ITEM_TYPE (obj) == ITEM_MONEY)
	{
	  if ((currency_type == CURRENCY_NOLDO
		  && (obj->nVirtual >= 990 && obj->nVirtual <= 995)))
	    {
	      money += ((int) ch->right_hand->farthings) * ch->right_hand->count;
	      extract_obj (ch->right_hand);
	      ch->right_hand = NULL;
	    }
	}
      else if (GET_ITEM_TYPE (obj) == ITEM_CONTAINER)
	{
	  for (tobj = obj->contains; tobj; tobj = next_obj)
	    {
	      next_obj = tobj->next_content;
	      if (GET_ITEM_TYPE (tobj) == ITEM_MONEY)
		{
		  if ((currency_type == CURRENCY_NOLDO
			  && (tobj->nVirtual >= 990
			      && tobj->nVirtual <= 995)))
		    {
		      money += ((int)tobj->farthings * tobj->count);
		      obj_from_obj (&tobj, 0);
		      extract_obj (tobj);
		      ch->delay_obj = obj;
		      container = true;
		    }
		}
	    }
	}
    }

  if ((obj = ch->left_hand))
    {
      if (GET_ITEM_TYPE (obj) == ITEM_MONEY)
	{
	  if ((currency_type == CURRENCY_NOLDO
		  && (obj->nVirtual >= 990 && obj->nVirtual <= 995)))
	    {
	      money += ((int)ch->left_hand->farthings * ch->left_hand->count);
	      extract_obj (ch->left_hand);
	      ch->left_hand = NULL;
	    }
	}
      else if (GET_ITEM_TYPE (obj) == ITEM_CONTAINER)
	{
	  for (tobj = obj->contains; tobj; tobj = next_obj)
	    {
	      next_obj = tobj->next_content;
	      if (GET_ITEM_TYPE (tobj) == ITEM_MONEY)
		{
		  if ((currency_type == CURRENCY_NOLDO
			  && (tobj->nVirtual >= 990
			      && tobj->nVirtual <= 995)))
		    {
		      money += ((int) tobj->farthings * tobj->count);
		      obj_from_obj (&tobj, 0);
		      extract_obj (tobj);
		      ch->delay_obj = obj;
		      container = true;
		    }
		}
	    }
	}
    }

  money -= farthings_to_subtract;

  if (money <= 0) // Serious bugfix - Japheth 10th May 2007
  {
	  return;
  }

  obj = ch->delay_obj;
  ch->delay_obj = NULL;

  if (money / 5000)
    {				// Mithril/gold hundredpiece.
      //if (currency_type == CURRENCY_ORKISH)
      tobj = load_object (995);
      tobj->count = money / 5000;
      if (obj)
	obj_to_obj (tobj, obj);
      else
	obj_to_char (tobj, ch);
      money %= 5000;
      change = true;
    }

  if (money / 1000)
    {				// Gold crown.
      tobj = load_object (994);
      if (obj)
	obj_to_obj (tobj, obj);
      else
	obj_to_char (tobj, ch);
      tobj->count = money / 1000;
      money %= 1000;
      change = true;
    }

  if (money / 500)
    {				// Silver tree.
      tobj = load_object (993);
      if (obj)
	obj_to_obj (tobj, obj);
      else
	obj_to_char (tobj, ch);
      tobj->count = money / 500;
      money %= 500;
      change = true;
    }

  if (money / 100)
    {				// Silver royal.
      tobj = load_object (992);
      if (obj)
	obj_to_obj (tobj, obj);
      else
	obj_to_char (tobj, ch);
      tobj->count = money / 100;
      money %= 100;
      change = true;
    }

  if (money / 10)
    {				// Bronze copper.
      tobj = load_object (991);
      if (obj)
	obj_to_obj (tobj, obj);
      else
	obj_to_char (tobj, ch);
      tobj->count = money / 10;
      money %= 10;
      change = true;
    }

  if (money)
    {				// Copper bit.
      tobj = load_object (990);
      if (obj)
	obj_to_obj (tobj, obj);
      else
	obj_to_char (tobj, ch);
      tobj->count = money;
      change = true;
    }
  if (!SupressOutput)
  {
  if (container)
    send_to_char
      ("\nRifling through your belongings, you retrieve your coin.\n", ch);
  else
    send_to_char ("\nYou offer up the specified amount.\n", ch);

  if (change)
    {
      if (obj)
	sprintf (buf, "Change is made, which you then deposit in #2%s#0.",
		 obj_short_desc (obj));
      else
	sprintf (buf, "Change is made for the amount offered.");
      send_to_char ("\n", ch);
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
    }
  }

  ch->delay_obj = NULL;
}

void
money_from_char_to_room (CHAR_DATA * ch, int vnum)
{
  OBJ_DATA *obj;

  if (!vnum || !vtor (vnum))
    return;

  if ((obj = get_obj_in_list_num (VNUM_FARTHING, ch->right_hand)) ||
      (obj = get_obj_in_list_num (VNUM_FARTHING, ch->left_hand)))
    {
      obj_from_char (&obj, 0);
      obj_to_room (obj, vnum);
    }

  if ((obj = get_obj_in_list_num (VNUM_PENNY, ch->right_hand)) ||
      (obj = get_obj_in_list_num (VNUM_PENNY, ch->left_hand)))
    {
      obj_from_char (&obj, 0);
      obj_to_room (obj, vnum);
    }
}

int
name_to_econ_zone (char *econ_zone_name)
{
  int i;

  for (i = 0; *default_econ_info[i].flag_name != '\n'; i++)
    if (!strcmp (econ_zone_name, default_econ_info[i].flag_name))
      return i;

  return -1;
}

int
zone_to_econ_zone (int zone)
{
  switch (zone)
    {
    case 15:
      return name_to_econ_zone ("azadmere");
    case 20:
      return name_to_econ_zone ("cherafir");
    case 5:
      return name_to_econ_zone ("coranan");
    case 50:
      return name_to_econ_zone ("evael");
    case 10:
    case 11:
    case 32:
    case 76:
      return name_to_econ_zone ("kaldor");
    case 51:
      return name_to_econ_zone ("kanday");
    case 23:
    case 25:
    case 27:
    case 31:
      return name_to_econ_zone ("orbaal");
    case 40:
      return name_to_econ_zone ("rethem");
    case 58:
      return name_to_econ_zone ("shiran");
    case 59:
    case 60:
    case 62:
      return name_to_econ_zone ("telen");
    case 14:
      return name_to_econ_zone ("thay");
    case 12:
      return name_to_econ_zone ("trobridge");

    default:
      return -1;
    }
}

int
obj_to_econ_zone (OBJ_DATA * obj)
{
  int i;
  int j;

  for (i = 0; *econ_flags[i] != '\n'; i++)
    if (IS_SET (obj->econ_flags, (1 << i)))
      for (j = 0; *default_econ_info[j].flag_name != '\n'; j++)
	if (!strcmp (econ_flags[i], default_econ_info[j].flag_name))
	  return j;

  return -1;
}

void
econ_markup_discount (CHAR_DATA * keeper, OBJ_DATA * obj, float *markup,
		      float *discount)
{
  /* Ok, this is a bit nutty.  How it works is...

     If the flag ACT_ECONZONE is set, then we have default markups
     and discounts (defined by default_econ_info in constants.c) that
     apply and override settings the keeper might have.

     Certain zones are econ zones, such as Kaldor which is zones
     10, 11, 31, and 76.  So, if an object has an econ_flag listed
     in default_econ_info, we use the markup/discount determined in
     the matrix using the object's econ flag against the zone's econ
     zone.

     If the keeper isn't flagged as ACT_ECONZONE, then match the
     different econ flags against the object to pick a discount/markup.
   */

  int keeper_econ_zone = -1;
  int object_econ_zone = -1;
  char buf[MAX_STRING_LENGTH];

  if (IS_SET (keeper->act, ACT_ECONZONE))
    {
      keeper_econ_zone = zone_to_econ_zone (keeper->room->zone);
      object_econ_zone = obj_to_econ_zone (obj);

      if (object_econ_zone == -1)
	object_econ_zone = keeper_econ_zone;

      if (keeper_econ_zone == -1)
	{
	  sprintf (buf, "Keeper %d in room %d can't be associated with "
		   "an econ zone.",
		   IS_NPC (keeper) ? keeper->mob->nVirtual : 0,
		   keeper->in_room);
	  system_log (buf, true);
	}

      else
	{
	  *markup = default_econ_info[object_econ_zone].obj_econ_info
	    [keeper_econ_zone].markup;
	  *discount = default_econ_info[object_econ_zone].obj_econ_info
	    [keeper_econ_zone].discount;
	  return;
	}
    }

  if (obj->econ_flags & keeper->shop->econ_flags1 &&
      keeper->shop->econ_markup1 > 0)
    {
      *markup = keeper->shop->econ_markup1;
      *discount = keeper->shop->econ_discount1;
    }

  else if (obj->econ_flags & keeper->shop->econ_flags2 &&
	   keeper->shop->econ_markup2 > 0)
    {
      *markup = keeper->shop->econ_markup2;
      *discount = keeper->shop->econ_discount2;
    }

  else if (obj->econ_flags & keeper->shop->econ_flags3 &&
	   keeper->shop->econ_markup3 > 0)
    {
      *markup = keeper->shop->econ_markup3;
      *discount = keeper->shop->econ_discount3;
    }

  else if (obj->econ_flags & keeper->shop->econ_flags4 &&
	   keeper->shop->econ_markup4 > 0)
    {
      *markup = keeper->shop->econ_markup4;
      *discount = keeper->shop->econ_discount4;
    }

  else if (obj->econ_flags & keeper->shop->econ_flags5 &&
	   keeper->shop->econ_markup5 > 0)
    {
      *markup = keeper->shop->econ_markup5;
      *discount = keeper->shop->econ_discount5;
    }

  else if (obj->econ_flags & keeper->shop->econ_flags6 &&
	   keeper->shop->econ_markup6 > 0)
    {
      *markup = keeper->shop->econ_markup6;
      *discount = keeper->shop->econ_discount6;
    }

  else if (obj->econ_flags & keeper->shop->econ_flags7 &&
	   keeper->shop->econ_markup7 > 0)
    {
      *markup = keeper->shop->econ_markup7;
      *discount = keeper->shop->econ_discount7;
    }

  else
    {
      *markup = keeper->shop->markup;
      *discount = keeper->shop->discount;
    }
}

float
econ_discount (CHAR_DATA * keeper, OBJ_DATA * obj)
{
  float markup;
  float discount;

  econ_markup_discount (keeper, obj, &markup, &discount);

  return discount;
}

float
econ_markup (CHAR_DATA * keeper, OBJ_DATA * obj)
{
  float markup;
  float discount;

  econ_markup_discount (keeper, obj, &markup, &discount);

  return markup;
}



float
tally (OBJ_DATA * obj, char *buffer, int depth)
{
  int count = 0;
  float cost = 0.0, subtotal = 0.0;
  char format[AVG_STRING_LENGTH] = "";
  OBJ_DATA *tobj = NULL;

  if (!obj || (strlen (buffer) > MAX_STRING_LENGTH - 256))
    return 0.00;

  count = (obj->count) ? obj->count : 1;
  cost = (obj->farthings + obj->silver * 4) * count;
  sprintf (format, "%% 10.02f cp - %%%dc#2%%s#0", 2 * depth);
  sprintf (buffer + strlen (buffer), format, cost, ' ',
	   obj->short_description);
  if (count > 1)
    {
      sprintf (buffer + strlen (buffer), " (x%d)\n", count);
    }
  else
    {
      strcat (buffer, "\n");
    }
  subtotal += cost;

  for (tobj = obj->contains; tobj; tobj = tobj->next_content)
    {
      subtotal += tally (tobj, buffer, depth + 1);
    }
  return subtotal;
}

void
do_tally (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch = NULL, *tch2 = NULL;
  OBJ_DATA *obj = NULL;
  int location = 0;
  float subtotal = 0.0, total = 0.0;
  bool bTallyAll = true;
  char arg1[AVG_STRING_LENGTH] = "";
  char buffer[MAX_STRING_LENGTH] = "";

  if (!GET_TRUST (ch))
    {
      send_to_char ("Huh?\n", ch);
      return;
    }

  if (argument && *argument)
    {
      argument = one_argument (argument, arg1);
      bTallyAll = false;
      if (!(tch2 = get_char_room_vis (ch, arg1)))
	{
	  send_to_char ("You do not see them here.\n", ch);
	  return;
	}
    }

  if (bTallyAll)
    {
      strcpy (buffer, "\n#6Tally in Room:#0\n");
      for (obj = ch->room->contents; obj; obj = obj->next_content)
	{
	  subtotal += tally (obj, buffer, 1);
	}
      sprintf (buffer + strlen (buffer),
	       "----------------------------------------------\n"
	       "% 10.02f cp - Subtotal\n", subtotal);
      total += subtotal;
    }

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {

      if ((!bTallyAll && tch != tch2) || (bTallyAll && tch == ch))
	{
	  continue;
	}


      subtotal = 0.0;
      sprintf (buffer + strlen (buffer),
	       "\n#6Tally of #5%s#0 (%s):#0\n", tch->short_descr, tch->tname);

      if ((obj = tch->right_hand))
	{
	  subtotal += tally (obj, buffer, 1);
	}

      if ((obj = tch->left_hand))
	{
	  subtotal += tally (obj, buffer, 1);
	}

      for (location = 0; location < MAX_WEAR; location++)
	{
	  if (!(obj = get_equip (tch, location)))
	    continue;
	  subtotal += tally (obj, buffer, 1);
	}

      sprintf (buffer + strlen (buffer),
	       "----------------------------------------------\n"
	       "% 10.02f cp - %sotal\n",
	       subtotal, (bTallyAll) ? "Subt" : "T");

      total += subtotal;
    }

  if (bTallyAll)
    {

      sprintf (buffer + strlen (buffer),
	       "==============================================\n"
	       "% 10.02f cp - Total\n", total);

    }

  page_string (ch->desc, buffer);
}

#include <memory>

void
do_mark (CHAR_DATA* ch, char *argument, int cmd)
{

  // First - Assert we have a valid usage case
  if (!IS_NPC(ch) || !ch->shop)
    {
      const char* message =
	"#6OOC - Only the shopkeeper mob may use this command.#0\n"
	"#6      e.g: command <clanmember> mark <item> <value>#0\n";
      send_to_char (message, ch);
      return;
    }

  // Make certain we have a storage room
  ROOM_DATA* store = 0;
  if (!(store = vtor (ch->shop->store_vnum)))
    {
      do_ooc (ch, "I seem to have lost my store room.", 0);
      return;
    }

  // Load the storage if we need to
  if (!store->psave_loaded)
    {
      load_save_room (store);
    }

  // Check if we have an inventory to work with
  if (!store->contents)
    {
      do_say (ch, "I have nothing for sale at the moment.", 0);
      return;
    }


  ////////////////////////////////////////////////////////////////////////////
  // Usages: mark <what to change> <new value>
  // what to change: number, keyword, nth.keyword, all.keyword, all
  // new value: number, free, reset
  ////////////////////////////////////////////////////////////////////////////
  char* ptr = 0;

  enum {
    mark_none = 0,
    mark_by_list_number,
    mark_by_keyword,
    mark_nth_by_keyword,
    mark_all_by_keyword,
    mark_all
  }
  mark_mode = mark_none;

  // first argument either a number or keyword

  if (!argument || !*argument)
    {
      do_say (ch, "What did you want to change the price of?", 0);
      return;
    }

  int list_number = 0;
  std::string keyword;
  char *item_string = new char[strlen (argument)];
  argument = one_argument (argument, item_string);
  if (strncasecmp ("all", item_string, 3) == 0)
    {
      if (strlen (item_string) == 3)
	{
	  mark_mode = mark_all;
	}
      else if (item_string[3] == '.')
	{
	  mark_mode = mark_all_by_keyword;
	  keyword = item_string + 4;
	}
      else
	{
	  mark_mode = mark_by_keyword;
	  keyword = item_string;
	}
    }
  else if ((list_number = strtol (item_string, &ptr, 10)))
    {
      if (ptr && *ptr)
	{
	  if (*ptr == '.')
	    {
	      mark_mode = mark_nth_by_keyword;
	      keyword = ptr + 1;
	    }
	  else
	    {
	      mark_mode = mark_by_keyword;
	      keyword = item_string;
	    }
	}
      else
	{
	  mark_mode = mark_by_list_number;
	}
    }
  else
    {
      mark_mode = mark_by_keyword;
      keyword = item_string;
    }
  delete [] item_string;

  // second argument must be a decimal, "free", or "reset"
  if (!argument || !*argument)
    {
      do_say (ch, "What price did you want to set?", 0);
      return;
    }

  float new_value;
  char* value_string = new char[strlen (argument)];
  argument = one_argument (argument, value_string);
  if (strcasecmp ("free", value_string) == 0)
    {
      new_value = -1.0f;
    }
  else if (strcasecmp ("reset", value_string) == 0)
    {
      new_value = 0.0f;
    }
  else if (!(new_value = strtof (value_string, &ptr))
	   && (ptr >= value_string))
    {
      char* errmsg =
	"The 'new value' parameter must be either a decimal number, "
	"'free', or 'reset'.";
      delete [] value_string;
      do_ooc (ch, errmsg, 0);
      return;
    }
  else if (new_value <= 0.0f)
    {
      new_value = -1.0f;
    }
  delete [] value_string;
  // else: new_value is a valid decimal.


  if (mark_mode == mark_by_keyword)
    {
      mark_mode = mark_nth_by_keyword;
      list_number = 1;
    }

  bool found = false;
  int i = 1;
  int count = 0;
  int currency = ch->mob->currency_type;
  OBJ_DATA* drink = 0;
  OBJ_DATA* obj = 0;
  for (obj = store->contents; (obj && !found); obj = obj->next_content)
    {
      // skip money
      if (GET_ITEM_TYPE (obj) == ITEM_MONEY
	  && keeper_uses_currency_type (currency, obj))
	{
	  continue;
	}

      if(
	 // mark all
	 (mark_mode == mark_all)

	 // mark_by_list_number
	 || (mark_mode == mark_by_list_number
	     && (list_number == i++)
	     && (found = true))

	 // mark_(nth/all)_by_keyword
	 || ((isname (keyword.c_str (), obj->name)
	      || (GET_ITEM_TYPE (obj) == ITEM_BOOK
		  && obj->book_title
		  && isname (keyword.c_str (), obj->book_title))
	      || (GET_ITEM_TYPE (obj) == ITEM_DRINKCON
		  && obj->o.drinkcon.volume
		  && (drink = vtoo (obj->o.drinkcon.liquid))
		  && isname (keyword.c_str (), drink->name)))
	     && (mark_mode == mark_all_by_keyword
		 || (mark_mode == mark_nth_by_keyword
		     && (list_number == i++)
		     && (found = true)))))

	{
	  obj->obj_flags.set_cost = (int)(100.0 * new_value);
	  ++count;
	}

    }
  if (count)
    {
      do_say (ch, "Our inventory is updated.", 0);
    }
  else
    {
      do_say (ch, "There doesn't seem to be anything like that "
	      "in our inventory.", 0);
    }
}

void
do_payroll (CHAR_DATA * ch, char *argument, int cmd)
{
  short last_day = -1, last_month = -1, last_year = -1;
  int payrollAmt = 0, payrollTAmt = 0;
  long day = -1, month = -1, year = -1;
  CHAR_DATA *keeper = NULL;
  MYSQL_RES *result;
  MYSQL_ROW row;
  char buf[MAX_STRING_LENGTH];
  //int shopnum = 0;
	
  if (IS_NPC (ch) && ch->shop)
		{
		keeper = ch;
		}
	else
		{
		for (keeper = character_list; keeper; keeper = keeper->next)
			{
			if (IS_NPC (keeper) &&
				keeper->shop &&
				keeper->shop->store_vnum == ch->in_room)
				break;
			}
		}

	if (keeper == NULL)
		{
		send_to_char ("You do not see a payroll ledger here.\n", ch);
		return;
		}

	/* Detail */
	int port = engine.get_port ();
	mysql_safe_query 
	  ("SELECT time, shopkeep, customer, "
	   "amount, room, gametime, port, "
	   "EXTRACT(YEAR FROM gametime) as year, "
	   "EXTRACT(MONTH FROM gametime) as month, "
	   "EXTRACT(DAY FROM gametime) as day "
	   "FROM %s.payroll "
	   "WHERE shopkeep = '%d' AND port = '%d' "
	   "ORDER BY time DESC;", 
	   (engine.get_config ("player_log_db")).c_str (),
	   keeper->mob->nVirtual, port);

	if ((result = mysql_store_result (database)) == NULL)
		{
		send_to_gods ((char *) mysql_error (database));
		send_to_char ("The payroll ledgers are unavailable at the moment.\n", ch);
		return;
		}

	send_to_char ("Examining a payroll ledger:\n", ch);
	
	sprintf(buf, "---------------------------\n");
	while ((row = mysql_fetch_row (result)) != NULL)
		{

		day = strtol (row[9], NULL, 10);
		month = strtol (row[8], NULL, 10) - 1;
		year = strtol (row[7], NULL, 10);
		if (day != last_day || month != last_month || year != last_year)
			{
			if (last_day > 0 && month != last_month)
				{
				sprintf (buf + strlen (buf),
					"\n    Total for #6%s %d#0: #2%d cp#0.\n\n",
					month_short_name[(int) last_month],
					(int) last_year,
					payrollAmt);
				
				payrollTAmt += payrollAmt;
				payrollAmt = 0;
				}
			
			sprintf (buf + strlen (buf),
				"\n On #6%s %d#0:\n\n",
				short_time_string(day, month),
				(int) year);
				
			last_day = day;
			last_month = month;
			last_year = year;
			}

		payrollAmt += strtol (row[3], NULL, 10);

		sprintf (buf + strlen (buf), " #5%s#0 was paid %d coppers.\n", row[2], strtol (row[3], NULL, 10));

		if (strlen (buf) > MAX_STRING_LENGTH - 512)
			{
			strcat (buf, "\n #1There were more paychecks than could be displayed.#0\n\n");
			break;
			}
		} //end while

		mysql_free_result (result);

		if (last_day > 0)
			{
			if (payrollTAmt == 0 &&
			    payrollAmt > 0)
				{
				sprintf (buf + strlen (buf),
					"\n    Total for #6%s %d#0: Payroll #2%d cp#0.\n\n"
					"    Current coin on hand: #2%d cp#0.\n",
					month_short_name[(int) last_month],
					(int) last_year,
					payrollAmt,
					keeper_has_money (keeper, 0));
				}
			else
				{
				sprintf (buf + strlen (buf),
					"\n    Total for #6%s %d#0: Payroll #2%d cp#0.\n\n"
					"    Total for period:      Payroll #2%d cp#0.\n"
					"    Current coin on hand:  #2%d cp#0.\n",
					month_short_name[(int) last_month],
					(int) last_year,
					payrollAmt, 
					payrollTAmt,
					keeper_has_money (keeper, 0));
				}
			page_string (ch->desc, buf);
			}
		}
