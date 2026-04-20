/*------------------------------------------------------------------------\
|  object_damage.c : Obejct Damage Class             www.middle-earth.us  | 
|  Copyright (C) 2005, Shadows of Isildur: Sighentist                     |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/

#include <string.h>
#include <stdlib.h>
#include <time.h>

#include "server.h"
#include "structs.h"
#include "utils.h"
#include "protos.h"
#include "object_damage.h"
#include "utility.h"


extern rpie::server engine;

const char *unspecified_conditions[] = {
  "It has been completely destroyed, and is clearly unfit for use.\n",
  "It has been savaged, rendering the item nearly unusable.\n",
  "This item is in shambles, having sustained quite a bit of damage.\n",
  "It is in ill repair, and rather damaged in appearance.\n",
  "It seems to have seen a decent amount of usage.\n",
  "It appears to be nearly flawless in condition.\n"
};

const char *fabric_conditions[] = {
  "All that remains of this item is an unsalvageable mass of torn fabric.\n",
  "What few hale stitches remain between large tears are frayed and worn.\n",
  "It is torn, frayed, and altogether quite piteous in appearance.\n",
  "It bears a few noticeable tears and rips from hard usage.\n",
  "This item has seen some usage, looking frayed and somewhat worn.\n",
  "It is nearly flawless, frayed only very slightly in a few spots.\n"
};

const char *leather_conditions[] = {
  "This item has been utterly ravaged, and is entirely unsalvageable.\n",
  "What little undamaged material remains is dark with age and use.\n",
  "It has been cracked and worn in many places with time and abuse.\n",
  "It bears a number of small cracks and scars, looking quite used.\n",
  "It has seen some usage, appearing cracked and worn in a few spots.\n",
  "It is in excellent condition, bearing only a few blemishes and scars.\n"
};

const char *metal_conditions[] = {
  "That this item is even recognisable is a miracle in of itself.\n",
  "Rust, wear, tears and punctures dominate this item.\n",
  "It is beaten, misshapen and rusted with time and abuse.\n",
  "It bears an amount of rust and sections beaten out of place.\n",
  "It has seen some usage, showing a little rust and scuffing.\n",
  "It is in excellent condition, completely free of rust and with few blemishes.\n"
};

const char *wood_conditions[] = {
  "All that remains of this item is a few jagged splinters of wood.\n",
  "This object has been cracked and splintered to the point of unusability.\n",
  "It has been cracked in a number of places, bearing numerous splinters.\n",
  "A few small but noticeable cracks have been made in this object.\n",
  "Splintered in a few places, this looks to have seen some usage.\n",
  "It is nearly flawless, only a small splinter or two out of place.\n"
};

const char *bone_conditions[] = {
  "This item has been cracked, chipped and flaked into oblivion.\n",
  "Nearly unusable, it has been cracked and chipped beyond recognition.\n",
  "It is cracked and chipped in a significant number of places.\n",
  "A few small, very faint cracks can be seen on this item.\n",
  "Nearly without flaw, only a small bit of wear is visible.\n",
};

short skill_to_damage_name_index (ushort n_skill);

/*------------------------------------------------------------------------\
|  new()                                                                  |
|                                                                         |
|  Returns a pointer to the newly allocated instance of object_damage.    |
\------------------------------------------------------------------------*/
OBJECT_DAMAGE *
object_damage__new ()
{
  OBJECT_DAMAGE *thisPtr = NULL;

  /* TODO: Remove this when we're ready to go live with damage */
  if (!engine.in_test_mode ())
    return NULL;

  CREATE (thisPtr, OBJECT_DAMAGE, 1);

  thisPtr->source = (DAMAGE_TYPE) 0;
  thisPtr->material = (MATERIAL_TYPE) 0;
  thisPtr->severity = (DAMAGE_SEVERITY) 0;
  thisPtr->impact = 0;
  thisPtr->name = 0;
  thisPtr->lodged = 0;
  thisPtr->when = 0;


  return thisPtr;
}


/*------------------------------------------------------------------------\
|  new_init()                                                             |
|                                                                         |
|  Returns a pointer to the newly allocated and initialized instance of   |
|  object damage.                                                         |
\------------------------------------------------------------------------*/
OBJECT_DAMAGE *
object_damage__new_init (DAMAGE_TYPE source, ushort impact,
			 MATERIAL_TYPE material, int lodged)
{
  OBJECT_DAMAGE *thisPtr = NULL;

  /* Exemption List (sketchy at best) */
  if ((source == DAMAGE_FIST)
      || ((source == DAMAGE_FREEZE) && (material <= MATERIAL_LEATHER))
      || ((source == DAMAGE_WATER) && (material <= MATERIAL_LEATHER))
      || ((source == DAMAGE_BLUNT) && (material <= MATERIAL_LEATHER)))
    {

      return NULL;

    }

  if (!(thisPtr = object_damage__new ()))
    {
      return NULL;
    }

  thisPtr->source = source;
  thisPtr->material = material;
  thisPtr->severity =
    (DAMAGE_SEVERITY) ((impact >
			5) ? (MIN (8,
				   ((impact - 1) / 10) + 4)) : ((impact / 2) +
								1));
  thisPtr->impact = (source == DAMAGE_BLOOD) ? 0 : impact;
  thisPtr->name = number (0, 3);
  thisPtr->lodged = lodged;
  thisPtr->when = time (0);

  return thisPtr;
}


/*------------------------------------------------------------------------\
|  delete()                                                               |
|                                                                         |
|  Frees the memory of this object damage instance and returns the next.  |
\------------------------------------------------------------------------*/
OBJECT_DAMAGE *
object_damage__delete (OBJECT_DAMAGE * thisPtr)
{
  OBJECT_DAMAGE *next = NULL;

  if (thisPtr)
    {
      next = thisPtr->next;
      mem_free (thisPtr);
    }

  return next;
}


/*------------------------------------------------------------------------\
|  get_sdesc()                                                            |
|                                                                         |
|  Returns a short description of the damage instance.                    |
\------------------------------------------------------------------------*/
char *
object_damage__get_sdesc (OBJECT_DAMAGE * thisPtr)
{
  ushort n_sdesc_length = 0;
  char *str_sdesc = NULL;
  extern const char *damage_severity[DAMAGE_SEVERITY_MAX + 1];
  extern const char *damage_name[12][5][4];

  if (thisPtr->source == DAMAGE_PERMANENT)
    return NULL;

  n_sdesc_length = strlen (damage_severity[thisPtr->severity])
    +
    strlen (damage_name[thisPtr->source]
	    [(thisPtr->material < MATERIAL_IRON) ? 0 : 1][thisPtr->name]) + 5;
  str_sdesc = (char *) alloc (sizeof (char) * n_sdesc_length, 0);
  sprintf (str_sdesc, "%s %s %s",
	   (isvowel (damage_severity[thisPtr->severity][0])) ? "an" : "a",
	   damage_severity[thisPtr->severity],
	   damage_name[thisPtr->
		       source][(thisPtr->material <
				MATERIAL_IRON) ? 0 : 1][thisPtr->name]);
  return str_sdesc;
}


/*------------------------------------------------------------------------\
|  write_to_file()                                                        |
|                                                                         |
|  Export a string that describes this damage instance.                   |
\------------------------------------------------------------------------*/
int
object_damage__write_to_file (OBJECT_DAMAGE * thisPtr, FILE * fp)
{
  return fprintf (fp, "Damage     %d %d %d %d %d %d %d\n",
		  thisPtr->source, thisPtr->material,
		  thisPtr->severity, thisPtr->impact,
		  thisPtr->name, thisPtr->lodged, thisPtr->when);
}


/*------------------------------------------------------------------------\
|  read_from_file()                                                       |
|                                                                         |
|  Import a string that describes a damage instance.                      |
\------------------------------------------------------------------------*/
OBJECT_DAMAGE *
object_damage__read_from_file (FILE * fp)
{
  OBJECT_DAMAGE *thisPtr = NULL;

  if (!(thisPtr = object_damage__new ()))
    {
      return NULL;
    }

  /* TODO: Make this an fscanf statement? */
  thisPtr->source = (DAMAGE_TYPE) fread_number (fp);
  thisPtr->material = (MATERIAL_TYPE) fread_number (fp);
  thisPtr->severity = (DAMAGE_SEVERITY) fread_number (fp);
  thisPtr->impact = fread_number (fp);
  thisPtr->name = fread_number (fp);
  thisPtr->lodged = fread_number (fp);
  thisPtr->when = fread_number (fp);

  return thisPtr;
}


/*------------------------------------------------------------------------\
|                                                                         |
|  STRING STORAGE                                                         |
|                                                                         |
\------------------------------------------------------------------------*/

const char *damage_type[DAMAGE_TYPE_MAX + 1] = {
  "stab",
  "pierce",
  "chop",
  "blunt",
  "slash",
  "freeze",			/*  5 */
  "burn",
  "tooth",
  "claw",
  "fist",
  "blood",			/* 10 */
  "water",
  "lightning",
  "permanent",			/* 13 */
  "repair",
  "\n"
};


const char *material_type[MATERIAL_TYPE_MAX + 1] = {
  "undefined",
  "burlap",
  "wool",
  "linen",
  "cotton",
  "silk",
  "leather",
  "steel",
  "iron",
  "wood",
  "ivory",
  "\n"
};


const char *damage_severity[DAMAGE_SEVERITY_MAX + 1] = {
  "unnoticeable",
  "miniscule",
  "small",
  "minor",
  "moderate",
  "large",
  "deep",
  "massive",
  "terrible",
  "\n"
};



short
skill_to_damage_name_index (ushort n_skill)
{
  switch (n_skill)
    {

/*    case SKILL_TEXTILECRAFT:
      return 0;
    case SKILL_HIDEWORKING:
      return 1;
    case SKILL_METALCRAFT:
      return 2;
    case SKILL_WOODCRAFT:
      return 3;
    case SKILL_STONECRAFT:
      return 4;
    case SKILL_DYECRAFT:
      return -1;
    case SKILL_GLASSWORK:
      return -1;
    case SKILL_GEMCRAFT:
      return -1;
    case SKILL_POTTERY:
      return -1; */
    default:
      return -1;

    }
}

const char *damage_name[12][5][4] = {

  {				/*  DAMAGE_STAB  */
   {"puncture", "hole", "perforation", "piercing"},	/* vs cloth */
   {"puncture", "hole", "perforation", "piercing"},	/* vs hide */
   {"puncture", "gouge", "perforation", "rupture"},	/* vs metal */
   {"chip",     "notch", "chip", "notch"},		/* vs wood */
   {"chip",     "scratch", "scratch", "chip"}		/* vs stone */
   },
  {				/*  DAMAGE_PIERCE  */
   {"puncture", "hole", "perforation", "piercing"},	/* vs cloth */
   {"puncture", "hole", "perforation", "piercing"},	/* vs hide */
   {"puncture", "gouge", "perforation", "rupture"},	/* vs metal */
   {"chip", "chip", "notch", "notch"},		/* vs wood */
   {"scratch", "chip", "scratch", "chip"}		/* vs stone */
   },
  {				/*  DAMAGE_CHOP  */
   {"slice", "cut", "slash", "gash"},	/* vs cloth */
   {"slice", "cut", "slash", "gash"},	/* vs hide */
   {"nick", "chip", "rivet", "gash"},	/* vs metal */
   {"", "", "", ""},		/* vs wood */
   {"", "", "", ""}		/* vs stone */
   },
  {				/*  DAMAGE_BLUNT  */
   {"tear", "rip", "rent", "tatter"},	/* vs cloth */
   {"tear", "split", "rent", "tatter"},	/* vs hide */
   {"gouge", "rivet", "nick", "dent"},	/* vs metal */
   {"", "", "", ""},		/* vs wood */
   {"", "", "", ""}		/* vs stone */
   },
  {				/*  DAMAGE_SLASH  */
   {"slice", "cut", "slash", "gash"},	/* vs cloth */
   {"slice", "cut", "slash", "gash"},	/* vs hide */
   {"nick", "chip", "rivet", "gash"},	/* vs metal */
   {"", "", "", ""},		/* vs wood */
   {"", "", "", ""}		/* vs stone */
   },
  {				/*  DAMAGE_FREEZE  */
   {"", "", "", ""},		/* vs cloth */
   {"", "", "", ""},		/* vs hide */
   {"crack", "split", "dent", "tarnish"},	/* vs metal */
   {"", "", "", ""},		/* vs wood */
   {"", "", "", ""}		/* vs stone */
   },
  {				/*  DAMAGE_BURN  */
   {"scorching", "charring", "blackening", "hole"},	/* vs cloth */
   {"scorching", "charring", "blackening", "hole"},	/* vs hide */
   {"scorching", "charring", "blackening", "tarnish"},	/* vs metal */
   {"scorching", "charring", "blackening", "tarnish"},		/* vs wood */
   {"scorching", "scorching", "blackening", "blackening"}		/* vs stone */
   },
  {				/*  DAMAGE_TOOTH  */
   {"tear", "rip", "rent", "tatter"},	/* vs cloth */
   {"tear", "rip", "rent", "tatter"},	/* vs hide */
   {"gouge", "rivet", "nick", "dent"},	/* vs metal */
   {"", "", "", ""},		/* vs wood */
   {"", "", "", ""}		/* vs stone */
   },
  {				/*  DAMAGE_CLAW  */
   {"tear", "rip", "rent", "tatter"},	/* vs cloth */
   {"tear", "rip", "rent", "tatter"},	/* vs hide */
   {"gouge", "rivet", "nick", "dent"},	/* vs metal */
   {"", "", "", ""},		/* vs wood */
   {"", "", "", ""}		/* vs stone */
   },
  {				/*  DAMAGE_FIST  */
   {"dent", "deformation", "dimple", "impression"},	/* vs metal */
   {"", "", "", ""},		/* vs wood */
   {"", "", "", ""},		/* vs cloth */
   {"", "", "", ""},		/* vs hide */
   {"", "", "", ""}		/* vs stone */
   },
  {				/*  DAMAGE_BLOOD  */
   {"bloodstain", "bloodstain", "blood-splatter", "stain"},	/* vs cloth */
   {"bloodstain", "bloodstain", "blood-splatter", "stain"},	/* vs hide */
   {"bloodstain", "bloodstain", "blood-splatter", "tarnish"},	/* vs metal */
   {"bloodstain", "bloodstain", "blood-splatter", "stain"},		/* vs wood */
   {"bloodstain", "bloodstain", "blood-splatter", "blood-splatter"}		/* vs stone */
   },
  {				/*  DAMAGE_WATER  */
   {"stain", "discoloration", "blemish", "splotch"},	/* vs cloth */
   {"stain", "discoloration", "blemish", "splotch"},	/* vs hide */
   {"tarnish", "corrosion", "flaking", "deterioration"},	/* vs metal */
   {"stain", "stain", "stain", "stain"},		/* vs wood */
   {"stain", "stain", "stain", "stain"}		/* vs stone */
   }
};
