/*------------------------------------------------------------------------\
|  objects.c : Object Module                          www.middle-earth.us | 
|  Copyright (C) 2004, Shadows of Isildur: Traithe                        |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <math.h>
#include <ctype.h>

#include "server.h"
#include "structs.h"
#include "protos.h"
#include "utils.h"
#include "utility.h"
#include "decl.h"

extern rpie::server engine;

MATERIAL_TYPE object__get_material (OBJ_DATA * thisPtr);

const char *weapon_theme[] =
  { "stab", "pierce", "chop", "bludgeon", "slash", "lash" };




// Determines the material type of a given object; called by GET_MATERIAL_TYPE().

int
determine_material (OBJ_DATA * obj)
{
  char buf[MAX_STRING_LENGTH];
  unsigned int i = 0, j = 0;

  for (i = 0; *materials[i] != '\n'; i++)
    {
      sprintf (buf, "%s", materials[i]);
      for (j = 0; j <= strlen (buf); j++)
	{
	  buf[j] = toupper (buf[j]);
	}
      if (isnamec (buf, obj->name))
	return (1 << i);
    }

  return (1 << 0);
}

/*------------------------------------------------------------------------\
|  do_mend()                                                              |
|                                                                         |
|  User command to repair a damaged object.                               |
\------------------------------------------------------------------------*/
void
do_mend (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *obj;
  OBJ_DATA *kit;
  OBJECT_DAMAGE *damage;
  bool blnCanMend = true;
  int nItemType = 0;
  char buf[AVG_STRING_LENGTH];

  /* TODO: Remove this when we're ready to go live with damage */
  if (!engine.in_test_mode ())
    return;

  if (!*argument || strlen (argument) > AVG_STRING_LENGTH)
    {
      send_to_char ("Mend what?\n", ch);
      return;
    }
  argument = one_argument (argument, buf);

  /* Are we holding what we wanted? */
  if (!(obj = get_obj_in_dark (ch, buf, ch->right_hand))
      && !(obj = get_obj_in_dark (ch, buf, ch->left_hand)))
    {
      send_to_char ("You're not holding anything like that.\n", ch);
      return;
    }

  /* Get the kit from our other hand */
  if (!(kit = (obj == ch->right_hand) ? ch->left_hand : ch->right_hand)
      || GET_ITEM_TYPE (kit) != ITEM_REPAIR_KIT)
    {
      act ("You're not holding anything that you can mend $p with.",
	   false, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }


  if ((nItemType = GET_ITEM_TYPE (obj)))
    {

      /* Check if the kit works with the item */

      if (kit->o.repair.arrItemType[0] == 255	/* all items */
	  || kit->o.repair.arrItemType[0] == nItemType
	  || kit->o.repair.arrItemType[1] == nItemType
	  || kit->o.repair.arrItemType[2] == nItemType
	  || kit->o.repair.arrItemType[3] == nItemType)
	{

	  /* Check if the kit works with the material */

	  for (damage = obj->damage; damage; damage = damage->next)
	    {

	    }
	}

    }

  /* If we can't then exit */

  if (!blnCanMend)
    {
      act ("You're not holding anything that you can mend $p with.",
	   false, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }
  /*
     1. Can the kit mend the object?

     This item may be used to mend _DAMAGE_TYPE_ damage to the _MATERIAL_TYPE parts of _ITEM_TYPE_ objects.


     uchar filteritem1;
     uchar filteritem2;
     uchar filteritem3;
     uchar filteritem4;



     if the kit can repair 'Any' Item-Type



   */

  act ("You begin to mend $p with $P.", false, ch, obj, kit,
       TO_CHAR | _ACT_FORMAT);

}

/*------------------------------------------------------------------------\
|  do_rend()                                                              |
|                                                                         |
|  User command to tear into an object or player                          |
\------------------------------------------------------------------------*/
void
do_rend (CHAR_DATA * ch, char *argument, int cmd)
{
  int i = 0;
  int num_args = 0;
  unsigned int impact = 0;
  unsigned int infection = 0;
  int type = -1;
  int wound_loc = -1;
  float dam = 0;
  char buf[13][MAX_STRING_LENGTH / 13];
  char *str_damage_sdesc = NULL;
  OBJ_DATA *insert_obj = NULL;
  OBJ_DATA *target_obj = NULL;
  OBJECT_DAMAGE *damage = NULL;
  CHAR_DATA *target_obj_ch = ch;	/* in possession of target_obj */
  CHAR_DATA *victim = NULL;
  char *error[] = {
    "\n"
      "Usage: rend [OPTIONS] object  or  rend [OPTIONS] victim\n"
      "\n"
      "  -d IMPACT         - 0-100 a percent of the target's total hits.\n"
      "  -t TYPE           - See 'tag damage-types' for values.\n"
      "  -b BODYLOC        - See 'tag woundlocs' for values.\n"
      "  -o OBJECT         - Will insert OBJECT into the victim's wound.\n"
      "  -c CHARACTER      - Will damage the specified object on CHARACTER.\n"
      "  -i RATE           - Determines whether the wound is infected or not.\n"
      "\n"
      "Examples:\n"
      "  > rend tunic                             - Apply random damage to an obj\n"
      "  > rend -d 12 -t stab vest                - Specific damage to an obj.\n"
      "  > rend -t bloodstain -c traithe gloves   - Damage someone elses obj.\n"
      "\n"
      "  > rend dravage                           - Apply random wound to char.\n"
      "  > rend -d 12 -t stab -b leye mirudus     - Specific damage to an char.\n"
      "  > rend -b head -o arrow tusken           - Wound char with inserted item.\n",
    "#1Please specify an object or character to rend.#0\n",
    "#1Damage must be 1-100; a percentage of the target's total hits.#0\n",
    "#1Unknown attack type, refer to 'tags object-damage' for a list of values.#0\n",
    "#1Unknown wound location, refer to 'tags woundlocs' for a list of values.#0\n",
    "#1You don't see the subject of the -o option.#0\n",
    "#1You don't see the subject of the -c option.#0\n",
    "#1You don't see that target object or victim.#0\n",
    "#1You can't use the -c option with a victim.#0\n",
    "#1You can't use the -b option with a target object.#0\n",
    "#1You can't use the -i option with a target object.#0\n"
  };

  if (!GET_TRUST (ch))
    {
      return;
    }

  if (!(num_args = sscanf (argument, "%s %s %s %s %s %s %s %s %s %s %s %s %s",
			   buf[0], buf[1], buf[2], buf[3], buf[4], buf[5],
			   buf[6], buf[7], buf[8], buf[9], buf[10], buf[11], buf[12])))
    {
      send_to_char (error[0], ch);
      return;
    }

  for (i = 0; i < num_args; i++)
    {
      if (strcmp (buf[i], "-d") == 0)
	{
	  if ((i == num_args - 1) || !sscanf (buf[i + 1], "%d", &impact)
	      || impact < 0 || impact > 100)
	    {
	      send_to_char (error[2], ch);
	      send_to_char (error[0], ch);
	      return;
	    }
	  else
	    {
	      buf[++i][0] = '\0';
	    }
	}
      else if (strcmp (buf[i], "-i") == 0)
	{
	  if ((i == num_args - 1) || !sscanf (buf[i + 1], "%d", &infection)
	      || impact < 0 || impact > 100)
	    {
	      send_to_char (error[0], ch);
	      return;
	    }
	  else
	    {
	      buf[++i][0] = '\0';
	    }
	}
      else if (strcmp (buf[i], "-t") == 0)
	{
	  if ((i == num_args - 1)
	      || (type = index_lookup (damage_type, buf[i + 1])) < 0)
	    {
	      send_to_char (error[3], ch);
	      send_to_char (error[0], ch);
	      return;
	    }
	  else
	    {
	      buf[++i][0] = '\0';
	    }
	}
      else if (strcmp (buf[i], "-b") == 0)
	{
	  if ((i == num_args - 1)
	      || (wound_loc = index_lookup (wound_locations, buf[i + 1])) < 0)
	    {
	      send_to_char (error[4], ch);
	      send_to_char (error[0], ch);
	      return;
	    }
	  else
	    {
	      buf[++i][0] = '\0';
	    }
	}
      else if (strcmp (buf[i], "-o") == 0)
	{
	  if ((i == num_args - 1) || !*buf[i + 1]
	      ||
	      !((insert_obj =
		 get_obj_in_dark (ch, buf[i + 1], ch->right_hand))
		|| (insert_obj =
		    get_obj_in_dark (ch, buf[i + 1], ch->left_hand))
		|| (insert_obj = get_obj_in_dark (ch, buf[i + 1], ch->equip))
		|| (insert_obj =
		    get_obj_in_list_vis (ch, buf[i + 1],
					 ch->room->contents))))
	    {

	      send_to_char (error[5], ch);
	      return;
	    }
	  else
	    {
	      buf[++i][0] = '\0';
	    }
	}
      else if (strcmp (buf[i], "-c") == 0)
	{
	  if ((i == num_args - 1) || !*buf[i + 1]
	      || !(target_obj_ch = get_char_room_vis (ch, buf[i + 1])))
	    {
	      send_to_char (error[6], ch);
	      return;
	    }
	  else
	    {
	      buf[++i][0] = '\0';
	    }
	}
      else
	{
	  if (!*buf[i]
	      || !((victim = get_char_room_vis (ch, buf[i]))
		   || (target_obj =
		       get_obj_in_dark (target_obj_ch, buf[i],
					target_obj_ch->right_hand))
		   || (target_obj =
		       get_obj_in_dark (target_obj_ch, buf[i],
					target_obj_ch->left_hand))
		   || (target_obj =
		       get_obj_in_dark (target_obj_ch, buf[i],
					target_obj_ch->equip))
		   || (target_obj =
		       get_obj_in_list_vis (target_obj_ch, buf[i],
					    target_obj_ch->room->contents))))
	    {
	      send_to_char (error[7], ch);
	      return;
	    }
	}
    }
  if (!target_obj && !victim)
    {
      send_to_char (error[7], ch);
      send_to_char (error[0], ch);
      return;
    }
  else if (victim && target_obj_ch != ch)
    {
      send_to_char (error[8], ch);
      send_to_char (error[0], ch);
      return;
    }
  else if (target_obj && wound_loc >= 0)
    {
      send_to_char (error[9], ch);
      send_to_char (error[0], ch);
      return;
    }
  else if (target_obj && infection >= 0)
    {
      send_to_char (error[10], ch);
      send_to_char (error[0], ch);
      return;
    }


  if (victim)
    {
      impact = (impact <= 0) ? number (0, victim->hit/2) : impact;
      wound_loc = wound_loc >= 0 ? wound_loc : number(0,49);
      type = (type < 0) ? number (0, 10) : type;
      sprintf (buf[0],
	       "Dam: %d %% of %d hits\nType: %s\nBodyLoc: %s\nInsert: %s\nInto Victim: %s\n",
	       impact, victim->max_hit, damage_type[type],
	       (wound_loc >= 0) ? wound_locations[wound_loc] : "Random",
	       (insert_obj) ? insert_obj->short_description : "None",
	       victim->short_descr);
      act (buf[0], false, ch, target_obj, 0, TO_CHAR | _ACT_FORMAT);

      dam = impact;
      dam = dam / 100;
      dam = dam * victim->max_hit;
      impact = (int) dam;

      wound_to_char(victim, add_hash(wound_locations[wound_loc]), impact, type, 0, 0, infection);

      if(insert_obj && insert_obj->short_description)
      {
        lodge_missile(victim, insert_obj, add_hash(wound_locations[wound_loc]));
      }


    }
  else
    {
      impact = (impact < 0) ? number (0, target_obj->item_wear) : impact;
      type = (type < 0) ? number (0, 10) : type;

      if ((damage =
	   object__add_damage (target_obj, (DAMAGE_TYPE) type,
			       (unsigned int) impact))
	  && (str_damage_sdesc = object_damage__get_sdesc (damage)))
	{

	  sprintf (buf[0],
		   "You concentrate on %s until #1%s#0 appears on %s.",
		   (target_obj_ch != ch) ? "$N" : "$p", str_damage_sdesc,
		   (target_obj_ch != ch) ? "$p" : "#3it#0");
	  sprintf (buf[1], "You notice #1%s#0 on $p.", str_damage_sdesc);
	  mem_free (str_damage_sdesc);

	}
      else
	{
	  sprintf (buf[0],
		   "You concentrate on %s but nothing seems to happen to %s.",
		   (target_obj_ch != ch) ? "$N" : "$p",
		   (target_obj_ch != ch) ? "$p" : "#2it#0");
	  buf[1][0] = '\0';
	}
      act (buf[0], false, ch, target_obj, target_obj_ch,
	   TO_CHAR | _ACT_FORMAT);
      if ((target_obj_ch != ch) && buf[1][0])
	act (buf[1], false, target_obj_ch, target_obj, 0,
	     TO_CHAR | _ACT_FORMAT);
    }

}


/*------------------------------------------------------------------------\
|  object__drench()                                                       |
|                                                                         |
|  Iterate through the object list and apply water damage where necessary |
\------------------------------------------------------------------------*/
void
object__drench (CHAR_DATA * ch, OBJ_DATA * _obj, bool isChEquip)
{
  OBJ_DATA *obj;

  if (_obj != NULL)
    {

      for (obj = _obj; obj != NULL; obj = obj->next_content)
	{

	  /* Lights get extinguished in water */

	  if (GET_ITEM_TYPE (obj) == ITEM_LIGHT
	      && !IS_SET (obj->obj_flags.extra_flags, ITEM_MAGIC))
	    {

	      /* Lights out */
	      if (obj->o.light.on)
		{
		  act ("$p is extinguished.", false, 0, obj, ch,
		       TO_ROOM | TO_CHAR | _ACT_FORMAT);
		  obj->o.light.on = false;
		}

	      /* Spoil lanterns */
	      obj->o.light.hours =
		(obj->o.light.hours <= obj->o.light.capacity
		 && obj->o.light.hours > 0) ? 0 : obj->o.light.hours;
	    }

	  /* Precious metals rust in water */

	  if ((GET_ITEM_TYPE (obj) == ITEM_ARMOR
	       || GET_ITEM_TYPE (obj) == ITEM_WEAPON)
	      && !IS_SET (obj->obj_flags.extra_flags, ITEM_MAGIC)
	      && (strstr (obj->name, "steel") || strstr (obj->name, "iron")
		  || strstr (obj->full_description, "steel")
		  || strstr (obj->full_description, "iron")))
	    {
	      object__add_damage (obj, DAMAGE_WATER, 1);
	    }


	}

    }
  if (isChEquip)
    {
      object__drench (ch, ch->left_hand, false);
      object__drench (ch, ch->right_hand, false);
    }
}


/*------------------------------------------------------------------------\
|  examine_damage()                                                       |
|                                                                         |
|  Iterate through the damage list and show the sdesc of each instance.   |
\------------------------------------------------------------------------*/
char *
object__examine_damage (OBJ_DATA * thisPtr)
{
  OBJECT_DAMAGE *damage;
  char *p, *str_damage_sdesc;
  static char buf[MAX_STRING_LENGTH];

  *buf = '\0';

  /* TODO: Remove this when we're ready to go live with damage */
  if (!engine.in_test_mode ())
    return buf;

  /* Iterate through the damage instances attached to this object */
  for (damage = thisPtr->damage; damage; damage = damage->next)
    {

      if ((str_damage_sdesc = object_damage__get_sdesc (damage)) != NULL)
	{

	  sprintf (buf, "%s%s %s%s",
		   (*buf) ? buf : "It bears ",
		   ((!damage->next
		     && damage != thisPtr->damage) ? "and" : ""),
		   str_damage_sdesc, ((!damage->next) ? ".#0\n" : ", "));

	  mem_free (str_damage_sdesc);
	}

    }

  if (*buf)
    {
      reformat_string (buf, &p);
      sprintf (buf, "\n   %s", p);
      mem_free (p);
    }

  return buf;
}


/*------------------------------------------------------------------------\
|  get_material()                                                         |
|                                                                         |
|  Try to get a material used to compose this object instance.            |
\------------------------------------------------------------------------*/
MATERIAL_TYPE
object__get_material (OBJ_DATA * thisPtr)
{
  /* TODO: flesh this out */

  if ((GET_ITEM_TYPE (thisPtr) == ITEM_ARMOR && thisPtr->o.od.value[0] <= 2)
      || GET_ITEM_TYPE (thisPtr) == ITEM_WORN)
    {
      return MATERIAL_WOOL;
    }
  else
    {
      return MATERIAL_STEEL;
    }
}


/*------------------------------------------------------------------------\
|  add_damage()                                                           |
|                                                                         |
|  Try to get a material used to compose this object instance.            |
\------------------------------------------------------------------------*/
OBJECT_DAMAGE *
object__add_damage (OBJ_DATA * thisPtr, DAMAGE_TYPE source,
		    unsigned int impact)
{
  OBJECT_DAMAGE *damage = NULL;

  /* TODO: Remove this when we're ready to go live with damage */
  if (!engine.in_test_mode ())
    return NULL;

  if ((damage =
       object_damage__new_init (source, impact,
				object__get_material (thisPtr), 0)))
    {
      damage->next = thisPtr->damage;
      thisPtr->damage = damage;
      thisPtr->item_wear -= damage->impact;
    }
  return damage;
}


void
do_grip (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  OBJ_DATA *obj = NULL;

  if (!*argument && !(obj = get_equip (ch, WEAR_PRIM))
      && !(obj = get_equip (ch, WEAR_BOTH)))
    {
      send_to_char ("What item did you wish to shift your grip on?\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (*buf && !(obj = get_obj_in_dark (ch, buf, ch->right_hand)) &&
      !(obj = get_obj_in_dark (ch, buf, ch->left_hand)))
    {
      send_to_char ("You don't have that in your inventory.\n", ch);
      return;
    }

  if (GET_ITEM_TYPE (obj) != ITEM_WEAPON)
    {
      send_to_char ("The grip command is only for use with weapons.\n", ch);
      return;
    }

  if(obj->location != WEAR_PRIM && obj->location != WEAR_BOTH && obj->location != WEAR_SEC)
  {
    send_to_char("You must first 'wield' a weapon before you can change your grip on it.\n", ch);
    return;
  }

  if ((obj->o.od.value[3] == SKILL_BRAWLING) || (obj->o.od.value[0] == 2))
    {
      send_to_char
	("The grip command cannot be used with this weapon type.\n", ch);
      return;
    }

  if (obj->o.od.value[0] != 1)
    {
      if (ch->str > 18 && (obj->o.od.value[0] == 3))
	;
      else
	{
	  send_to_char ("You cannot shift your grip upon that weapon.\n", ch);
	  return;
	}
    }

  argument = one_argument (argument, buf);

  if ((!*buf && obj->location == WEAR_PRIM) || !str_cmp (buf, "both"))
    {
      if (ch->right_hand && ch->left_hand)
	{
	  send_to_char
	    ("You'll need to free your other hand to switch to a two-handed grip.\n",
	     ch);
	  return;
	}

      if (ch->right_hand && get_soma_affect(ch, SOMA_BLUNT_L_SEVARM))
      {
        send_to_char
        ("Your left arm is too damaged to grip a weapon.\n", ch);
	  return;
      }
  
      if (ch->left_hand && get_soma_affect(ch, SOMA_BLUNT_R_SEVARM))
      {
        send_to_char
        ("Your right arm is too damaged to grip a weapon.\n", ch);
	  return;
      }

      if (obj->location == WEAR_BOTH)
	{
	  send_to_char
	    ("You are already gripping your weapon in both hands.\n", ch);
	  return;
	}
      sprintf (buf, "You shift to a two-handed grip on #2%s#0.",
	       obj->short_description);
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      sprintf (buf, "$n shifts to a two-handed grip on #2%s#0.",
	       obj->short_description);
      act (buf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      obj->location = WEAR_BOTH;
    }
  else if (!(*buf && obj->location == WEAR_BOTH) || !str_cmp (buf, "single"))
    {
      if (obj->location == WEAR_PRIM || obj->location == WEAR_SEC)
	{
	  send_to_char
	    ("You are already gripping your weapon in your primary hand.\n",
	     ch);
	  return;
	}
      sprintf (buf, "You shift to a single-handed grip on #2%s#0.",
	       obj->short_description);
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      sprintf (buf, "$n shifts to a single-handed grip on #2%s#0.",
	       obj->short_description);
      act (buf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      obj->location = WEAR_PRIM;
    }
}

void
do_switch_item (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *right, *left;

  right = ch->right_hand;
  left = ch->left_hand;

  if (!right && !left)
    {
      act ("You have nothing to switch!", false, ch, 0, 0,
	   TO_CHAR | _ACT_FORMAT);
      return;
    }

  if ((right && right->location == WEAR_BOTH) ||
      (left && left->location == WEAR_BOTH))
    {
      act ("You must grip that in both hands!", false, ch, 0, 0,
	   TO_CHAR | _ACT_FORMAT);
      return;
    }

  ch->right_hand = NULL;
  ch->left_hand = NULL;

  if (right && right->location != WEAR_BOTH)
    {

      if (get_soma_affect(ch, SOMA_BLUNT_L_SEVARM))
      {
        send_to_char("Your left arm is too damaged to hold that.\n", ch);
        ch->right_hand = right;
	return;
      }

      act ("You shift $p to your left hand.", false, ch, right, 0,
	   TO_CHAR | _ACT_FORMAT);
      ch->left_hand = right;
    }

  if (left && left->location != WEAR_BOTH)
    {

  
      if (get_soma_affect(ch, SOMA_BLUNT_R_SEVARM))
      {
        send_to_char("Your left arm is too damaged to hold that.\n", ch);
        ch->left_hand = left;
        return;
      }

      act ("You shift $p to your right hand.", false, ch, left, 0,
	   TO_CHAR | _ACT_FORMAT);
      ch->right_hand = left;
    }

}


void
clear_omote (OBJ_DATA * obj)
{

  if (obj->omote_str)
    {
      mem_free (obj->omote_str);
      obj->omote_str = (char *) NULL;
    }
}

int
can_obj_to_container (OBJ_DATA * obj, OBJ_DATA * container, char **msg,
		      int count)
{
  OBJ_DATA *tobj;
  int i = 0;
  static char message[160];

  if (count > obj->count || count <= 0)
    count = obj->count;

  *msg = message;

  if (GET_ITEM_TYPE (container) == ITEM_SHEATH)
    {
      if (GET_ITEM_TYPE (obj) != ITEM_WEAPON)
	{
	  sprintf (message, "Sheaths are only for storing weaponry.\n");
	  return 0;
	}
      if (container->contains)
	{
	  sprintf (message, "This sheath already contains a weapon.\n");
	  return 0;
	}
      if (container->contained_wt + OBJ_MASS (obj) > container->o.od.value[0])
	{
	  sprintf (message, "That weapon is too large for the sheath.\n");
	  return 0;
	}
      return 1;
    }

  if (GET_ITEM_TYPE (container) == ITEM_KEYRING)
    {
      if (GET_ITEM_TYPE (obj) != ITEM_KEY)
	{
	  sprintf (message, "Keyrings are only able to hold keys!\n");
	  return 0;
	}
      for (tobj = container->contains, i = 1; tobj; tobj = tobj->next_content)
	i++;
      if (i + 1 > container->o.od.value[0])
	{
	  sprintf (message,
		   "There are too many keys on this keyring to add another.\n");
	  return 0;
	}
      return 1;
    }

  if (GET_ITEM_TYPE (container) == ITEM_WEAPON
      && container->o.weapon.use_skill == SKILL_SLING)
    {
      if (GET_ITEM_TYPE (obj) != ITEM_BULLET)
	{
	  sprintf (message,
		   "You'll need to find proper ammunition use with the sling.\n");
	  return 0;
	}
      if (container->contains)
	{
	  sprintf (message, "The sling is already loaded!\n");
	  return 0;
	}
      if (count > 1)
	{
	  sprintf (message,
		   "The sling is only capable of throwing one projectile at a time.\n");
	  return 0;
	}
      return 1;
    }

  if (GET_ITEM_TYPE (container) == ITEM_QUIVER)
    {
      if (GET_ITEM_TYPE (obj) != ITEM_MISSILE)
	{
	  sprintf (message,
		   "Quivers are only for storing arrows and bolts.\n");
	  return 0;
	}
      for (tobj = container->contains, i = 0; tobj; tobj = tobj->next_content)
	i += tobj->count;
      if (i + count > container->o.od.value[0])
	{
	  sprintf (message, "The quiver can't hold that many missiles.\n");
	  return 0;
	}
      return 1;
    }

  if (GET_ITEM_TYPE (container) != ITEM_CONTAINER)
    {
      sprintf (message, "%s is not a container.\n",
	       container->short_description);
      *message = toupper (*message);
      return 0;
    }

  if (container == obj)
    {
      sprintf (message, "You can't do that.\n");
      return 0;
    }

  if (IS_SET (container->o.od.value[1], CONT_CLOSED))
    {
      sprintf (message, "%s is closed.\n", container->short_description);
      *message = toupper (*message);
      return 0;
    }

  if (count > 1)
    {
      if (container->contained_wt + obj->obj_flags.weight * count >
	  container->o.od.value[0])
	{
	  sprintf (message, "That much won't fit.\n");
	  return 0;
	}
    }
  else if (container->contained_wt + obj->obj_flags.weight >
	   container->o.od.value[0])
    {

      sprintf (message, "%s won't fit.\n", obj->short_description);
      *message = toupper (*message);
      return 0;
    }

  return 1;
}

#define NO_TOO_MANY		1
#define NO_TOO_HEAVY	2
#define NO_CANT_TAKE	3
#define NO_CANT_SEE		4
#define NO_HANDS_FULL	5
#define NO_RESOURCE 6
#define LEFT_BROKEN 7
#define RIGHT_BROKEN 8

int
can_obj_to_inv (OBJ_DATA * obj, CHAR_DATA * ch, int *error, int count)
{

  if (count > obj->count || count <= 0)
    count = obj->count;

  *error = 0;

  if (!obj->in_obj && !CAN_SEE_OBJ (ch, obj))
    {
      *error = NO_CANT_SEE;
      return 0;
    }

  if (!CAN_WEAR (obj, ITEM_TAKE)
      || IS_SET (obj->obj_flags.extra_flags, ITEM_PITCHED))
    {
      *error = NO_CANT_TAKE;
      return 0;
    }

  if (GET_ITEM_TYPE(obj) == ITEM_RESOURCE)
  {
    *error = NO_RESOURCE;
    return 0;
  }

  if (ch->right_hand && (ch->right_hand->nVirtual == obj->nVirtual)
      && (GET_ITEM_TYPE (ch->right_hand) == ITEM_MONEY))
    return 1;

  if (ch->left_hand && (ch->left_hand->nVirtual == obj->nVirtual)
      && (GET_ITEM_TYPE (ch->left_hand) == ITEM_MONEY))
    return 1;

  if ((ch->right_hand && ch->left_hand) || get_equip (ch, WEAR_BOTH))
    {
      *error = NO_HANDS_FULL;
      return 0;
    }

  /* Check out the weight */

  if (!(obj->in_obj && obj->in_obj->carried_by == ch))
    {

      if ((IS_CARRYING_W (ch) + (count * obj->obj_flags.weight)) >
	  CAN_CARRY_W (ch) && IS_MORTAL (ch))
	{
	  *error = NO_TOO_HEAVY;
	  return 0;
	}

    }

  /*
  if (!ch->right_hand && !ch->left_hand && get_soma_affect(ch, SOMA_BLUNT_R_SEVARM))
  {
    * error = RIGHT_BROKEN;
    return 0;
  }
  */

  if (ch->right_hand && get_soma_affect(ch, SOMA_BLUNT_L_SEVARM))
  {
    * error = LEFT_BROKEN;
    return 0;
  }

  if (ch->left_hand && get_soma_affect(ch, SOMA_BLUNT_R_SEVARM))
  {
    * error = RIGHT_BROKEN;
    return 0;
  }

  return 1;
}

int
obj_activate (CHAR_DATA * ch, OBJ_DATA * obj)
{
  if (!obj->activation)
    return 0;

  magic_affect (ch, obj->activation);

  /* Deal with one time activation on object */

  if (!IS_SET (obj->obj_flags.extra_flags, ITEM_MULTI_AFFECT))
    obj->activation = 0;

  /* Oops, I guess that killed him. */

  if (GET_POS (ch) == POSITION_DEAD)
    return 1;

  return 0;
}

void
get (CHAR_DATA * ch, OBJ_DATA * obj, int count)
{
  OBJ_DATA *container;
  CHAR_DATA *i;

  if (IS_SET (obj->obj_flags.extra_flags, ITEM_TIMER)
      && obj->nVirtual != VNUM_CORPSE)
    {
      obj->obj_flags.extra_flags &= ~ITEM_TIMER;
      obj->obj_timer = 0;
    }

  if (!obj->in_obj)
    {

      if (IS_SET (obj->tmp_flags, SA_DROPPED))
	{
	  send_to_char ("That can't be picked up at the moment.\n", ch);
	  return;
	}

      // Mod for taking a corpse - Methuselah
      // If someone is skinning the corpse, don't allow it to be taken

      if (obj->nVirtual == VNUM_CORPSE)
	{
  	  for (i = ch->room->people; i; i = i->next_in_room)
	    {
	      if (i->delay_info1 == (long int) obj)
	        {
		  send_to_char ("You can't take that while it's being skinned.\n", ch);
                  return;
	        }
	    }
	}
      // End mod for not taking an object when it's being skinned.  -Methuselah

      obj_from_room (&obj, count);

      clear_omote (obj);

      act ("You get $p.", false, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
      act ("$n gets $p.", false, ch, obj, 0, TO_ROOM | _ACT_FORMAT);

      obj_to_char (obj, ch);

      if (obj->activation &&
	  IS_SET (obj->obj_flags.extra_flags, ITEM_GET_AFFECT))
	obj_activate (ch, obj);

      return;
    }

  /* Don't activate object if it is in an object we're carrying */

  container = obj->in_obj;

  obj_from_obj (&obj, count);

  act ("You get $p from $P.", false, ch, obj, container,
       TO_CHAR | _ACT_FORMAT);
  act ("$n gets $p from $P.", true, ch, obj, container,
       TO_ROOM | _ACT_FORMAT);

  obj_to_char (obj, ch);

  if (obj->activation && IS_SET (obj->obj_flags.extra_flags, ITEM_GET_AFFECT))
    obj_activate (ch, obj);
}

int
just_a_number (char *buf)
{
  unsigned int i;

  if (!*buf)
    return 0;

  for (i = 0; i < strlen (buf); i++)
    if (!isdigit (buf[i]))
      return 0;

  return 1;
}

void
do_junk (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *obj;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("What did you wish to junk?\n", ch);
      return;
    }

  if (!(obj = get_obj_in_list_vis (ch, buf, ch->right_hand)) &&
      !(obj = get_obj_in_list_vis (ch, buf, ch->left_hand)))
    {
      send_to_char ("You don't seem to be holding that.\n", ch);
      return;
    }

  obj_from_char (&obj, 0);
  if ( engine.in_play_mode ()) 
  {
    obj_to_room (obj, JUNKYARD);
    obj->obj_timer = 960;		// Junked items saved for 10 RL days.
    obj->obj_flags.extra_flags |= ITEM_TIMER;
  }
  else 
  {
    extract_obj (obj);
  }

  sprintf (buf, "You junk #2%s#0.", obj->short_description);
  act (buf, false, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
}

#define CONTAINER_LOC_NOT_FOUND	0
#define CONTAINER_LOC_ROOM		1
#define CONTAINER_LOC_INVENTORY	2
#define CONTAINER_LOC_WORN		3
#define CONTAINER_LOC_UNKNOWN	4

void
do_get (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *hitch;
  ROOM_DATA *hitch_room;
  OBJ_DATA *obj = NULL;
  OBJ_DATA *container = NULL;
  int container_loc = CONTAINER_LOC_UNKNOWN;
  int count = 0;
  int error;
  SECOND_AFFECT *sa;
  char arg1[MAX_STRING_LENGTH];
  char arg2[MAX_STRING_LENGTH];
  bool coldload_id = false;

  *arg1 = '\0';
  *arg2 = '\0';

  if (IS_MORTAL (ch) && IS_SET (ch->room->room_flags, OOC) 
        && str_cmp (ch->room->name, PREGAME_ROOM_NAME))
    {
      send_to_char ("This command has been disabled in OOC zones.\n", ch);
      return;
    }

  argument = one_argument (argument, arg1);

  if (just_a_number (arg1))
    {
      count = atoi (arg1);
      argument = one_argument (argument, arg1);
    }
  else if (!str_cmp (arg1, ".c"))
    {
      coldload_id = true;
      argument = one_argument (argument, arg1);
    }

  argument = one_argument (argument, arg2);

  if (!str_cmp (arg2, "from") || !str_cmp (arg2, "in"))
    argument = one_argument (argument, arg2);

  if (!str_cmp (arg2, "ground") || !str_cmp (arg2, "room"))
    {
      argument = one_argument (argument, arg2);
      container_loc = CONTAINER_LOC_ROOM;
    }

  else if (!str_cmp (arg2, "worn") || !str_cmp (arg2, "my"))
    {
      argument = one_argument (argument, arg2);
      container_loc = CONTAINER_LOC_WORN;
    }

  else if (!strn_cmp (arg2, "inventory", 3))
    {
      argument = one_argument (argument, arg2);
      container_loc = CONTAINER_LOC_INVENTORY;
    }

  if (*arg2 &&
      container_loc == CONTAINER_LOC_UNKNOWN &&
      (hitch = get_char_room_vis (ch, arg2)) &&
      hitch->mob &&
      hitch->mob->vehicle_type == VEHICLE_HITCH &&
      (hitch_room = vtor (hitch->mob->nVirtual)))
    {

      if (!(obj = get_obj_in_list_vis (ch, arg1, hitch_room->contents)))
	{
	  act ("You don't see that in $N.", false, ch, 0, hitch,
	       TO_CHAR | _ACT_FORMAT);
	  return;
	}

      if (!can_obj_to_inv (obj, ch, &error, count))
	{

	  if (error == NO_CANT_TAKE)
	    act ("You can't take $o.", true, ch, obj, 0, TO_CHAR);
	  else if (error == NO_TOO_MANY)
	    act ("You can't handle so much.", true, ch, 0, 0, TO_CHAR);
	  else if (error == NO_TOO_HEAVY)
	    act ("You can't carry so much weight.", true, ch, 0, 0, TO_CHAR);
	  else if (error == NO_CANT_SEE)
	    act ("You don't see it.", true, ch, 0, 0, TO_CHAR);
	  else if (error == NO_HANDS_FULL)
	    act ("Your hands are full!", true, ch, 0, 0, TO_CHAR);
          else if (error == NO_RESOURCE)
            act ("You'll need a pack animal to move $p.", true, ch, obj, 0, TO_CHAR);
          else if (error == LEFT_BROKEN)            
            act ("Your left arm is too damaged to hold $p.",  true, ch, obj, 0, TO_CHAR);
          else if (error == RIGHT_BROKEN)            
            act ("Your right arm is too damaged to hold $p.",  true, ch, obj, 0, TO_CHAR);
	  return;
	}

      obj_from_room (&obj, count);

      act ("You take $p from $N.", false, ch, obj, hitch,
	   TO_CHAR | _ACT_FORMAT);
      act ("$n takes $p from $N.", false, ch, obj, hitch,
	   TO_NOTVICT | _ACT_FORMAT);

      char_from_room (ch);
      char_to_room (ch, hitch->mob->nVirtual);

      act ("$n reaches in and takes $p.", false, ch, obj, hitch,
	   TO_NOTVICT | _ACT_FORMAT);

      char_from_room (ch);
      char_to_room (ch, hitch->in_room);

      obj_to_char (obj, ch);

      return;
    }

  else if (*arg2)
    {

      if (container_loc == CONTAINER_LOC_UNKNOWN &&
	  !(container = get_obj_in_dark (ch, arg2, ch->right_hand)) &&
	  !(container = get_obj_in_dark (ch, arg2, ch->left_hand)) &&
	  !(container = get_obj_in_dark (ch, arg2, ch->equip)) &&
	  !(container = get_obj_in_list_vis (ch, arg2, ch->room->contents)))
	container_loc = CONTAINER_LOC_NOT_FOUND;

      else if (container_loc == CONTAINER_LOC_ROOM &&
	       !(container =
		 get_obj_in_list_vis (ch, arg2, ch->room->contents)))
	{
	  container_loc = CONTAINER_LOC_NOT_FOUND;
	}

      else if (container_loc == CONTAINER_LOC_INVENTORY &&
	       !(container = get_obj_in_dark (ch, arg2, ch->right_hand)) &&
	       !(container = get_obj_in_dark (ch, arg2, ch->left_hand)))
	container_loc = CONTAINER_LOC_NOT_FOUND;

      else if (container_loc == CONTAINER_LOC_WORN &&
	       !(container = get_obj_in_dark (ch, arg2, ch->equip)))
	container_loc = CONTAINER_LOC_NOT_FOUND;

      if (container_loc == CONTAINER_LOC_NOT_FOUND)
	{
	  send_to_char ("You neither have nor see such a container.\n", ch);
	  return;
	}

      if (GET_ITEM_TYPE (container) != ITEM_CONTAINER &&
	  GET_ITEM_TYPE (container) != ITEM_QUIVER &&
	  (GET_ITEM_TYPE (container) != ITEM_WEAPON &&
	   container->o.weapon.use_skill != SKILL_SLING) &&
	  GET_ITEM_TYPE (container) != ITEM_SHEATH &&
	  GET_ITEM_TYPE (container) != ITEM_KEYRING)
	{
	  act ("$o isn't a container.", true, ch, container, 0, TO_CHAR);
	  return;
	}
        /*
         if ( GET_ITEM_TYPE (container) != ITEM_QUIVER
         && ( bow = get_equip (ch, WEAR_PRIM) ) 
         && ( bow->o.weapon.use_skill == SKILL_SHORTBOW
         || bow->o.weapon.use_skill == SKILL_LONGBOW) ) {
         send_to_char ("You'll have to stop using the bow first.\n", ch);
         return;                
         }
        */
      if (IS_SET (container->o.container.flags, CONT_CLOSED))
	{
	  send_to_char ("That's closed!\n", ch);
	  return;
	}
      
      if (container->o.weapon.use_skill == SKILL_SHORTBOW 
          || container->o.weapon.use_skill == SKILL_LONGBOW)
      {
        send_to_char ("You'll need to destring the bow to remove the string.\n", ch);
        return;
      }

      if (container_loc == CONTAINER_LOC_UNKNOWN)
	{
	  if (container->carried_by)
	    container_loc = CONTAINER_LOC_INVENTORY;
	  else if (container->equiped_by)
	    container_loc = CONTAINER_LOC_WORN;
	  else
	    container_loc = CONTAINER_LOC_ROOM;
	}
    }

  if (!*arg1)
    {
      send_to_char ("Get what?\n", ch);
      return;
    }

  if (!str_cmp (arg1, "all"))
    {

      for (obj = (container ? container->contains : ch->room->contents);
	   obj && !can_obj_to_inv (obj, ch, &error, 1);
	   obj = obj->next_content)
	;

      if (!obj)
	{
	  send_to_char ("There is nothing left you can take.\n", ch);
	  return;
	}

      ch->delay_type = DEL_GET_ALL;
      ch->delay_who = (char *) container;
      ch->delay_info1 = container_loc;
      ch->delay = 4;
    }

  else if (*arg2 && !str_cmp (arg2, "all"))
    {
      send_to_char ("You'll have to get things one at a time.\n", ch);
      return;
    }

  if (!container)
    {

      if (!obj && isdigit (*arg1) && coldload_id)
	{
	  if (!(obj = get_obj_in_list_id (atoi (arg1), ch->room->contents))
	      || obj->in_room != ch->in_room)
	    {
	      send_to_char ("You don't see that here.\n", ch);
	      return;
	    }
	}

      if (!obj && !(obj = get_obj_in_list_vis (ch, arg1, ch->room->contents)))
	{
	  send_to_char ("You don't see that here.\n", ch);
	  return;
	}

      if (!can_obj_to_inv (obj, ch, &error, count))
	{

	  if (error == NO_CANT_TAKE)
	    act ("You can't take $o.", true, ch, obj, 0, TO_CHAR);
	  else if (error == NO_TOO_MANY)
	    act ("You can't handle so much.", true, ch, 0, 0, TO_CHAR);
	  else if (error == NO_TOO_HEAVY)
	    act ("You can't carry so much weight.", true, ch, 0, 0, TO_CHAR);
	  else if (error == NO_CANT_SEE)
	    act ("You don't see it.", true, ch, 0, 0, TO_CHAR);
	  else if (error == NO_HANDS_FULL)
	    act ("Your hands are full!", true, ch, 0, 0, TO_CHAR);
          else if (error == NO_RESOURCE)
            act ("You'll need a pack animal to move $p.", true, ch, obj, 0, TO_CHAR);
          else if (error == LEFT_BROKEN)            
            act ("Your left arm is too damaged to hold $p.",  true, ch, obj, 0, TO_CHAR);
          else if (error == RIGHT_BROKEN)            
            act ("Your right arm is too damaged to hold $p.",  true, ch, obj, 0, TO_CHAR);
	  return;
	}

      if ((sa = get_second_affect (ch, SA_GET_OBJ, obj)))
	return;

      get (ch, obj, count);
      return;
    }

  /* get obj from container */

  if (!obj && !(obj = get_obj_in_dark (ch, arg1, container->contains)))
    {
      act ("You don't see that in $o.", true, ch, container, 0, TO_CHAR);
      return;
    }

  if (!can_obj_to_inv (obj, ch, &error, count))
    {

      if (error == NO_CANT_TAKE)
	act ("You cannot take $o.", true, ch, obj, 0, TO_CHAR);
      else if (error == NO_TOO_HEAVY)
	send_to_char ("You can't carry so much weight.\n", ch);
      else if (error == NO_TOO_MANY)
	send_to_char ("Your can't handle so much.\n", ch);
      else if (error == NO_HANDS_FULL)
	act ("Your hands are full!", true, ch, 0, 0, TO_CHAR);
      else if (error == NO_RESOURCE)
        act ("You'll need a pack animal to move $p.", true, ch, obj, 0, TO_CHAR);
      else if (error == LEFT_BROKEN)            
        act ("Your left arm is too damaged to hold $p.",  true, ch, obj, 0, TO_CHAR);
      else if (error == RIGHT_BROKEN)            
        act ("Your right arm is too damaged to hold $p.",  true, ch, obj, 0, TO_CHAR);
      return;
    }

  if (container && container != obj)
    obj->in_obj = container;
  if (!container->contains)
    container->contains = obj;

  get (ch, obj, count);
}

void
do_take (CHAR_DATA * ch, char *argument, int cmd)
{
  int worn_object = 0;
  CHAR_DATA *victim;
  OBJ_DATA *obj;
  char obj_name[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch)
        && str_cmp (ch->room->name, PREGAME_ROOM_NAME))
    {
      send_to_char ("This command has been disabled in OOC zones.\n", ch);
      return;
    }

  argument = one_argument (argument, obj_name);

  if (!*obj_name)
    {
      send_to_char ("Take what?\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (!str_cmp (buf, "from"))
    argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Take from whom?\n", ch);
      return;
    }

  if (!(victim = get_char_room_vis (ch, buf)))
    {
      send_to_char ("You don't see them.\n", ch);
      return;
    }

  if (victim == ch)
    {
      send_to_char ("Why take from yourself?\n", ch);
      return;
    }

  if (!(obj = get_obj_in_list_vis (ch, obj_name, victim->right_hand)) &&
      !(obj = get_obj_in_list_vis (ch, obj_name, victim->left_hand)))
    {

      if (!(obj = get_obj_in_list_vis (ch, obj_name, victim->equip)))
	{
	  act ("You don't see that on $N.", true, ch, 0, victim, TO_CHAR);
	  return;
	}

      if (GET_TRUST (ch))
	{
	  unequip_char (victim, obj->location);
	  obj_to_char (obj, victim);
	}
      else
	worn_object = 1;
    }

  if (GET_POS (victim) == SLEEP && !GET_TRUST (ch))
    {

      wakeup (victim);

      if (GET_POS (victim) != SLEEP)
	{

	  act ("$N awakens as you touch $M.", true, ch, 0, victim, TO_CHAR);
	  act ("$n awakens $N as $e touches $M.",
	       false, ch, 0, victim, TO_NOTVICT);

	  if (!get_affect (victim, MAGIC_AFFECT_PARALYSIS))
	    return;
	}
    }

  if (!(GET_TRUST (ch) ||
	GET_POS (victim) <= SLEEP ||
	get_affect (victim, MAGIC_AFFECT_PARALYSIS) ||
	IS_SUBDUEE (victim) || IS_MOUNT (victim)))
    {
      act ("$N prevents you from taking $p.", true, ch, obj, victim, TO_CHAR);
      act ("$N unsuccessfully tries to take $p from you.",
	   true, victim, obj, ch, TO_CHAR);
      act ("$n prevents $N from taking $p.",
	   false, victim, obj, ch, TO_NOTVICT);
      return;
    }

  if (worn_object)
    {

      strcpy (buf2, locations[obj->location]);
      *buf2 = tolower (*buf2);

      sprintf (buf, "You begin to remove $p from $N's %s.", buf2);
      act (buf, true, ch, obj, victim, TO_CHAR | _ACT_FORMAT);

      sprintf (buf, "$n begins removing $p from $N's %s.", buf2);
      act (buf, false, ch, obj, victim, TO_NOTVICT | _ACT_FORMAT);

      sprintf (buf, "$N begins removing $p from your %s.", buf2);
      act (buf, true, victim, obj, ch, TO_CHAR | _ACT_FORMAT);

      ch->delay_info1 = (long int) obj;
      ch->delay_info2 = obj->location;
      ch->delay_ch = victim;
      ch->delay_type = DEL_TAKE;
      ch->delay = 15;

      return;
    }

  obj_from_char (&obj, 0);
  obj_to_char (obj, ch);

  if (!GET_TRUST (ch))
    {
      act ("$n takes $p from you.", true, victim, obj, ch,
	   TO_CHAR | _ACT_FORMAT);
      act ("$n takes $p from $N.", false, ch, obj, victim,
	   TO_NOTVICT | _ACT_FORMAT);
    }

  clear_omote (obj);

  act ("You take $p from $N.", true, ch, obj, victim, TO_CHAR);

  if (obj->activation && IS_SET (obj->obj_flags.extra_flags, ITEM_GET_AFFECT))
    obj_activate (ch, obj);
}

void
delayed_take (CHAR_DATA * ch)
{
  OBJ_DATA *obj;
  CHAR_DATA *victim;
  char buf[MAX_STRING_LENGTH];

  ch->delay = 0;
  victim = ch->delay_ch;
  obj = (OBJ_DATA *) ch->delay_info1;

  if (!is_he_here (ch, victim, true))
    {
      send_to_char ("Your victim left before you could finish taking the "
		    "object.\n", ch);
      return;
    }

  if (get_equip (victim, ch->delay_info2) != obj)
    {
      send_to_char ("The thing you were after is gone now.", ch);
      return;
    }

  if (GET_POS (victim) == SLEEP && !GET_TRUST (ch))
    {

      wakeup (victim);

      if (GET_POS (victim) != SLEEP)
	{

	  act ("$N awakens as struggle with $M.",
	       true, ch, 0, victim, TO_CHAR);
	  act ("$n awakens $N as $e struggles with $M.",
	       false, ch, 0, victim, TO_NOTVICT);

	  if (!get_affect (victim, MAGIC_AFFECT_PARALYSIS))
	    return;
	}
    }

  if (!(GET_TRUST (ch) ||
	GET_POS (victim) <= SLEEP ||
	get_affect (victim, MAGIC_AFFECT_PARALYSIS) ||
	IS_SUBDUEE (victim) || IS_MOUNT (victim)))
    {
      act ("$N prevents you from taking $p.", true, ch, obj, victim, TO_CHAR);
      act ("$N unsuccessfully tries to take $p from you.",
	   true, victim, obj, ch, TO_CHAR);
      act ("$n prevents $N from taking $p.",
	   false, victim, obj, ch, TO_NOTVICT);
      return;
    }

  sprintf (buf, "$n removes and takes $p from $N's %s.",
	   locations[obj->location]);
  act (buf, false, ch, obj, victim, TO_NOTVICT | _ACT_FORMAT);

  sprintf (buf, "$N removes and takes $p from your %s.",
	   locations[obj->location]);
  act (buf, true, victim, obj, ch, TO_CHAR | _ACT_FORMAT);

  sprintf (buf, "You remove and take $p from $N's %s.",
	   locations[obj->location]);
  act (buf, true, ch, obj, victim, TO_CHAR | _ACT_FORMAT);

  unequip_char (victim, obj->location);

  if(GET_ITEM_TYPE(obj) == ITEM_RESOURCE)
  {
    if(IS_SET (victim->affected_by, AFF_TRANSPORTING))
      victim->affected_by &= AFF_TRANSPORTING;
    obj_to_room(obj, ch->in_room);
  }
  else
  {
    obj_to_char (obj, ch);
  }

  if (obj->activation && IS_SET (obj->obj_flags.extra_flags, ITEM_GET_AFFECT))
    obj_activate (ch, obj);
}

void
get_break_delay (CHAR_DATA * ch)
{
  if (ch->delay_info1 == CONTAINER_LOC_ROOM)
    send_to_char ("You stop picking things up.\n", ch);
  else
    send_to_char ("You stop removing things.\n", ch);

  ch->delay = 0;
}

void
delayed_get (CHAR_DATA * ch)
{
  OBJ_DATA *container;
  OBJ_DATA *obj;
  OBJ_DATA *first_obj;
  char buf[MAX_STRING_LENGTH];
  int item_num = 0;
  int container_num = 0;
  int error;
  char *locs[] = { "", "room", "inventory", "worn", "" };

  if (ch->delay_who)
    {
      /* Makes sure that this container is in the room */

      container = (OBJ_DATA *) ch->delay_who;

      if (ch->delay_info1 == CONTAINER_LOC_ROOM)
	obj = ch->room->contents;
      else if (ch->delay_info1 == CONTAINER_LOC_INVENTORY)
	obj = ch->right_hand;
      else if (ch->delay_info1 == CONTAINER_LOC_WORN)
	obj = ch->equip;
      else
	obj = NULL;

      for (; obj; obj = obj->next_content)
	{

	  container_num++;

	  if (obj == container)
	    break;
	}

      if (!obj)
	{
	  send_to_char ("You can't get anything else.\n", ch);
	  return;
	}

      first_obj = container->contains;
    }
  else
    first_obj = ch->room->contents;

  for (obj = first_obj; obj; obj = obj->next_content)
    {

      if (!IS_OBJ_VIS (ch, obj))
	continue;

      item_num++;

      if (can_obj_to_inv (obj, ch, &error, 0))
	{

	  if (container_num)
	    sprintf (buf, "#%d %s #%d",
		     item_num, locs[ch->delay_info1], container_num);
	  else
	    sprintf (buf, "#%d", item_num);

	  do_get (ch, buf, 0);

	  if (obj->carried_by != ch)
	    printf ("Oh boy...couldn't pick up %d\n", obj->nVirtual);
	  else
	    ch->delay = 4;

	  return;
	}
    }

  send_to_char ("...and that's about all you can get.\n", ch);

  ch->delay = 0;
}

void
delayed_remove (CHAR_DATA * ch)
{
  OBJ_DATA *obj, *eq;

  obj = (OBJ_DATA *) ch->delay_who;

  if (!obj)
    {
      ch->delay_type = 0;
      ch->delay_who = 0;
      ch->delay = 0;
      return;
    }

  if (obj->location == WEAR_WAIST)
    {
      if ((eq = get_equip (ch, WEAR_BELT_1)))
	{
	  act ("$p falls free.", true, ch, eq, 0, TO_CHAR);
	  act ("$n drops $p.", true, ch, eq, 0, TO_ROOM);
	  obj_to_room (unequip_char (ch, WEAR_BELT_1), ch->in_room);
	}

      if ((eq = get_equip (ch, WEAR_BELT_2)))
	{
	  act ("$p falls free.", true, ch, eq, 0, TO_CHAR);
	  act ("$n drops $p.", true, ch, eq, 0, TO_ROOM);
	  obj_to_room (unequip_char (ch, WEAR_BELT_2), ch->in_room);
	}
    }

  if (obj->loaded /*&& obj->o.weapon.use_skill != SKILL_CROSSBOW*/)
    do_unload (ch, "", 0); 

  if (IS_SET (obj->obj_flags.extra_flags, ITEM_MASK) &&
      IS_SET (ch->affected_by, AFF_HOODED))
    do_hood (ch, "", 0);

  if (obj->location == WEAR_LIGHT && GET_ITEM_TYPE (obj) == ITEM_LIGHT)
    light (ch, obj, false, true);

  if (obj->location == WEAR_PRIM || obj->location == WEAR_SEC ||
      obj->location == WEAR_BOTH || obj->location == WEAR_SHIELD)
    unequip_char (ch, obj->location);
  else
    obj_to_char (unequip_char (ch, obj->location), ch);

  act ("You stop using $p.", false, ch, obj, 0, TO_CHAR);
  act ("$n stops using $p.", true, ch, obj, 0, TO_ROOM);

  ch->delay = 0;
  ch->delay_type = 0;
  ch->delay_who = 0;
}

void
drop_all (CHAR_DATA * ch)
{
  char buf[MAX_STRING_LENGTH];
  char buffer[MAX_STRING_LENGTH];
  OBJ_DATA *tobj, *obj;

  if (ch->right_hand && GET_POS (ch) != POSITION_UNCONSCIOUS)
    {
      act ("You drop:", false, ch, 0, 0, TO_CHAR);
      act ("$n drops:", false, ch, 0, 0, TO_ROOM);
    }

  while (ch->right_hand || ch->left_hand)
    {

      if (ch->right_hand)
	obj = ch->right_hand;
      else
	obj = ch->left_hand;

      if(GET_POS (ch) != POSITION_UNCONSCIOUS)
      {
        act ("   $p", false, ch, obj, 0, TO_CHAR);
        act ("   $p", false, ch, obj, 0, TO_ROOM);
      }

      obj_from_char (&obj, 0);
      obj_to_room (obj, ch->in_room);

      if(ch->fighting && GET_ITEM_TYPE (obj) == ITEM_WEAPON && (obj->o.od.value[3] == SKILL_LONGBOW || obj->o.od.value[3] == SKILL_SHORTBOW) && obj->o.od.value[5] == 1)
      {
        if(number(0,1))
        {
          send_to_char("The bowstring curls and snaps as the melee rages on!\n", ch);
          obj->o.od.value[5] = 2;
        }
      }

      if (obj->loaded/* && obj->o.weapon.use_skill != SKILL_CROSSBOW*/)
	{
	  sprintf (buffer, "%s#0 clatters to the ground!",
		   obj_short_desc (obj->loaded));
	  *buffer = toupper (*buffer);
	  sprintf (buf, "#2%s", buffer);
	  act (buf, true, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
	  act (buf, true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
	  tobj = load_object (obj->loaded->nVirtual);
	  extract_obj (obj->loaded);
	  obj->loaded = NULL;
	  obj_to_room (tobj, ch->in_room);
	  obj->loaded = NULL;
	}

      if (GET_ITEM_TYPE (obj) == ITEM_WEAPON
	  && obj->o.weapon.use_skill == SKILL_SLING && ch->whirling)
	{
	  send_to_char ("You cease whirling your sling.\n", ch);
	  ch->whirling = 0;
	}
    }
}

void
do_drop (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH] = "";
  char buffer[MAX_STRING_LENGTH] = "";
  OBJ_DATA *obj, *tobj;
  ROOM_DATA *room;
  int count = 0, old_count = 1;
  std::string first_person, third_person;

  argument = one_argument (argument, buf);

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch)
        && str_cmp (ch->room->name, PREGAME_ROOM_NAME))
    {
      send_to_char ("This command has been disabled in OOC zones.\n", ch);
      return;
    }

  if (just_a_number (buf))
    {
      count = atoi (buf);
      argument = one_argument (argument, buf);
    }

  if (!*buf)
    {
      send_to_char ("Drop what?\n", ch);
      return;
    }

  if (!str_cmp (buf, "all"))
    {

      argument = one_argument (argument, buf);

      if (*buf)
	{
	  send_to_char ("You can only 'drop all'.\n", ch);
	  return;
	}

      drop_all (ch);

      return;
    }

  if (!(obj = get_obj_in_dark (ch, buf, ch->right_hand)) &&
      !(obj = get_obj_in_dark (ch, buf, ch->left_hand)))
    {
      send_to_char ("You do not have that item.\n", ch);
      return;
    }

  if (count > obj->count)
    count = obj->count;

  sprintf (buffer, "%s", char_short(ch));
  *buffer = toupper(*buffer);
  old_count = obj->count;
  if (count)
     obj->count = count;

  first_person.assign("You drop #2");
  first_person.append(obj_short_desc(obj));
  first_person.append("#0");
  third_person.assign("#5");
  third_person.append(buffer);
  third_person.append("#0 drops #2");
  third_person.append(obj_short_desc(obj));
  third_person.append("#0");

  obj->count = old_count;

  if (evaluate_emote_string (ch, &first_person, third_person, argument))
  {
	obj_from_char (&obj, count);
	obj_to_room (obj, ch->in_room);
	if (obj->activation &&
	      IS_SET (obj->obj_flags.extra_flags, ITEM_DROP_AFFECT))
	    obj_activate (ch, obj);

      if(ch->fighting && GET_ITEM_TYPE (obj) == ITEM_WEAPON && (obj->o.od.value[3] == SKILL_LONGBOW || obj->o.od.value[3] == SKILL_SHORTBOW) && obj->o.od.value[5] == 1)
      {
        if(number(0,1))
        {
          send_to_char("The bowstring curls and snaps as the melee rages on!\n", ch);
          obj->o.od.value[5] = 2;
        }
      }

 	if (obj->loaded/*  && obj->o.weapon.use_skill != SKILL_CROSSBOW*/)
    	{
   	   	sprintf (buffer, "%s#0 clatters to the ground!",
	  	     obj_short_desc (obj->loaded));
     	 	*buffer = toupper (*buffer);
 	     	sprintf (buf, "#2%s", buffer);
	      	act (buf, true, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
 	     	act (buf, true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
 	     	tobj = load_object (obj->loaded->nVirtual);
 	     	extract_obj (obj->loaded);
 	     	obj->loaded = NULL;
 	     	obj_to_room (tobj, ch->in_room);
 	     	obj->loaded = NULL;
  	}

 	if (GET_ITEM_TYPE (obj) == ITEM_WEAPON
 	     && obj->o.weapon.use_skill == SKILL_SLING && ch->whirling)
 	{
 	     send_to_char ("You cease whirling your sling.\n", ch);
 	     ch->whirling = 0;
	}

	if ( !(first_person.empty()) )
	{
		sprintf (buffer, "%s %s", buf, first_person.c_str());
		do_omote (ch, buffer, 0);
	}
  }

  room = ch->room;

}

void
put_on_char (CHAR_DATA * ch, CHAR_DATA * victim, OBJ_DATA * obj,
	     int count, char *argument)
{
  int location;
  ROOM_DATA *hitch_room;

  if (victim->mob && victim->mob->vehicle_type == VEHICLE_HITCH)
    {

      if ((hitch_room = vtor (victim->mob->nVirtual)))
	{

	  obj_from_char (&obj, count);

	  act ("You put $p in $N.", false, ch, obj, victim,
	       TO_CHAR | _ACT_FORMAT);
	  act ("$n puts $p in $N.", false, ch, obj, victim,
	       TO_NOTVICT | _ACT_FORMAT);

	  char_from_room (ch);
	  char_to_room (ch, victim->mob->nVirtual);

	  act ("$n reaches in and drops $p.",
	       false, ch, obj, victim, TO_NOTVICT | _ACT_FORMAT);

	  char_from_room (ch);
	  char_to_room (ch, victim->in_room);

	  obj_to_room (obj, victim->mob->nVirtual);

	  return;
	}

      act ("You can't put $p in $N.", false, ch, obj, victim,
	   TO_CHAR | _ACT_FORMAT);

      return;
    }

  if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_NECK))
    location = WEAR_NECK_1;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_BODY))
    location = WEAR_BODY;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_HEAD))
    location = WEAR_HEAD;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_BACK))
    location = WEAR_BACK;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_LEGS))
    location = WEAR_LEGS;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_FEET))
    location = WEAR_FEET;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_HANDS))
    location = WEAR_HANDS;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_ARMS))
    location = WEAR_ARMS;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_ABOUT))
    location = WEAR_ABOUT;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_WAIST))
    location = WEAR_WAIST;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_WRIST))
    {
      if (get_equip (victim, WEAR_WRIST_L))
	location = WEAR_WRIST_R;
      else
	location = WEAR_WRIST_L;
    }
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_HAIR))
    location = WEAR_HAIR;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_FACE))
    location = WEAR_FACE;
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_ANKLE))
    {
      if (get_equip (victim, WEAR_ANKLE_L))
	location = WEAR_ANKLE_R;
      else
	location = WEAR_ANKLE_L;
    }
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_SHOULDER))
    {
      if (get_equip (victim, WEAR_SHOULDER_L))
	location = WEAR_SHOULDER_R;
      else
	location = WEAR_SHOULDER_L;
    }
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_ARMBAND))
    {
      if (get_equip (victim, WEAR_ARMBAND_R))
	location = WEAR_ARMBAND_L;
      else
	location = WEAR_ARMBAND_R;
    }
  else if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_BLINDFOLD))
    location = WEAR_BLINDFOLD;
  else
    {
      act ("You can't put $p on $M.", true, ch, obj, victim, TO_CHAR);
      return;
    }

  if (IS_MOUNT (victim) && !IS_SET (obj->obj_flags.extra_flags, ITEM_MOUNT) && GET_ITEM_TYPE(obj) != ITEM_RESOURCE)
    {
      act ("$N is a mount.  You can't put $p on it.",
	   true, ch, obj, victim, TO_CHAR);
      return;
    }

  if (get_equip (victim, location))
    {
      act ("$N is already wearing $p.",
	   false, ch, get_equip (victim, location), victim, TO_CHAR);
      return;
    }

  if (GET_POS (victim) == SLEEP && !GET_TRUST (ch))
    {

      wakeup (victim);

      if (GET_POS (victim) != SLEEP)
	{

	  act ("$N awakens as you touch $M.", true, ch, 0, victim, TO_CHAR);
	  act ("$n awakens $N as $e touches $M.",
	       false, ch, 0, victim, TO_NOTVICT);

	  if (!get_affect (victim, MAGIC_AFFECT_PARALYSIS))
	    return;
	}
    }

  if (!(GET_TRUST (ch) ||
	GET_POS (victim) <= SLEEP ||
	get_affect (victim, MAGIC_AFFECT_PARALYSIS) || IS_MOUNT (victim)))
    {
      act ("$N stops you from putting $p on $M.",
	   true, ch, obj, victim, TO_CHAR);
      act ("$N unsuccessfully tries to put $p from you.",
	   true, victim, obj, ch, TO_CHAR);
      act ("$n stops $N from putting $p on $M.",
	   false, victim, obj, ch, TO_NOTVICT);
      return;
    }

  ch->delay_type = DEL_PUTCHAR;
  ch->delay = 7;
  ch->delay_ch = victim;
  ch->delay_info1 = (long int) obj;
  ch->delay_info2 = location;

  act ("$n begins putting $p on $N.", false, ch, obj, victim,
       TO_NOTVICT | _ACT_FORMAT);
  act ("$n begins putting $p on you.", true, ch, obj, victim,
       TO_VICT | _ACT_FORMAT);
  act ("You begin putting $p on $N.", true, ch, obj, victim,
       TO_CHAR | _ACT_FORMAT);
}

void
delayed_putchar (CHAR_DATA * ch)
{
  int location;
  OBJ_DATA *obj;
  OBJ_DATA *tobj;
  CHAR_DATA *victim;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  ch->delay = 0;
  victim = ch->delay_ch;
  obj = (OBJ_DATA *) ch->delay_info1;
  location = ch->delay_info2;

  if (!is_he_here (ch, victim, true))
    {
      send_to_char ("Your victim left before you could finish dressing "
		    "them.\n", ch);
      return;
    }

  if(GET_ITEM_TYPE(obj) == ITEM_RESOURCE)
  {
    if (!is_obj_in_list (obj, ch->room->contents))
    {
      send_to_char ("You don't see that here.\n", ch);
      return;
    }    
  }
  else
  {
    if (!is_obj_in_list (obj, ch->right_hand) &&
        !is_obj_in_list (obj, ch->left_hand))
    {
      act ("You no longer have the thing you were putting on $N.",
	   false, ch, 0, victim, TO_CHAR);
      act ("$n stops putting something on $N.", true, ch, 0, victim, TO_ROOM);
      act ("$n stops putting something on you.",
	   true, ch, 0, victim, TO_VICT);
      return;
    }
  }


  tobj = get_equip (victim, ch->delay_info2);

  if (tobj && tobj != obj)
    {
      act ("You discover that $N is already wearing $p.",
	   true, ch, tobj, victim, TO_CHAR);
      act ("$n stops dressing $N.", false, ch, 0, victim, TO_NOTVICT);
      return;
    }

  if (GET_POS (victim) == SLEEP && !GET_TRUST (ch))
    {

      wakeup (victim);

      if (GET_POS (victim) != SLEEP)
	{

	  act ("$N awakens as struggle with $M.",
	       true, ch, 0, victim, TO_CHAR);
	  act ("$n awakens $N as $e struggles with $M.",
	       false, ch, 0, victim, TO_NOTVICT);

	  if (!get_affect (victim, MAGIC_AFFECT_PARALYSIS))
	    return;
	}
    }

  if (!(GET_TRUST (ch) ||
	GET_POS (victim) <= SLEEP ||
	get_affect (victim, MAGIC_AFFECT_PARALYSIS) || IS_MOUNT (victim)))
    {
      act ("$N prevents you from taking $p.", true, ch, obj, victim, TO_CHAR);
      act ("$N unsuccessfully tries to take $p from you.",
	   true, victim, obj, ch, TO_CHAR);
      act ("$n prevents $N from taking $p.",
	   false, victim, obj, ch, TO_NOTVICT);
      return;
    }

  if(GET_ITEM_TYPE(obj) == ITEM_RESOURCE)
  {
    if(obj->count > 1)
    {
      obj_from_room (&obj, 1);
      equip_char (victim, obj, location);
    }
    else
    {
      obj_from_room (&obj, 0);
      equip_char (victim, obj, location);
    }
 
    if(!IS_SET (victim->affected_by, AFF_TRANSPORTING))
      victim->affected_by |= AFF_TRANSPORTING;
  }
  else
  {
    obj_from_char (&obj, 0);
    equip_char (victim, obj, location);
  }

  strcpy (buf2, locations[location]);
  *buf2 = tolower (*buf2);

  sprintf (buf, "$n puts $p on $N's %s.", buf2);
  act (buf, false, ch, obj, victim, TO_NOTVICT | _ACT_FORMAT);

  sprintf (buf, "$N puts $p on your %s.", buf2);
  act (buf, true, victim, obj, ch, TO_CHAR | _ACT_FORMAT);

  sprintf (buf, "You put $p on $N's %s.", buf2);
  act (buf, true, ch, obj, victim, TO_CHAR | _ACT_FORMAT);

  if (obj->activation &&
      IS_SET (obj->obj_flags.extra_flags, ITEM_WEAR_AFFECT))
    obj_activate (ch, obj);
}

void
do_put (CHAR_DATA * ch, char *argument, int cmd)
{
  char buffer[MAX_STRING_LENGTH] = "";
  char arg[MAX_STRING_LENGTH] = "";
  char *error;
  OBJ_DATA *obj;
  OBJ_DATA *tar;
  int count = 0, old_count = 1, put_light_on_table = 0;
  CHAR_DATA *victim;
  std::string first_person, third_person;

  argument = one_argument (argument, arg);

  if (just_a_number (arg))
    {
      count = atoi (arg);
      argument = one_argument (argument, arg);
    }

  if (!arg)
    {
      send_to_char ("Put what?\n", ch);
      return;
    }

  if (!(obj = get_obj_in_dark (ch, arg, ch->right_hand)) &&
      !(obj = get_obj_in_dark (ch, arg, ch->left_hand)))
    {
      sprintf (buffer, "You don't have a %s.\n", arg);
      send_to_char (buffer, ch);
      return;
    }

  argument = one_argument (argument, arg);

  if (!str_cmp (arg, "in") || !str_cmp (arg, "into"))
    argument = one_argument (argument, arg);

  else if (!str_cmp (arg, "on"))
    {

      argument = one_argument (argument, arg);

      if (!(victim = get_char_room_vis (ch, arg)))
	{
	  act ("Put $p on whom?", true, ch, obj, 0, TO_CHAR);
	  return;
	}

      put_on_char (ch, victim, obj, count, argument);

      return;
    }

  if (!*arg)
    {
      act ("Put $o into what?", false, ch, obj, 0, TO_CHAR);
      return;
    }

  if (!(tar = get_obj_in_dark (ch, arg, ch->right_hand)) &&
      !(tar = get_obj_in_dark (ch, arg, ch->left_hand)) &&
      !(tar = get_obj_in_dark (ch, arg, ch->equip)) &&
      !(tar = get_obj_in_list_vis (ch, arg, ch->room->contents)))
    {

      if ((victim = get_char_room_vis (ch, arg)))
	{
	  put_on_char (ch, victim, obj, count, argument);
	  return;
	}

      sprintf (buffer, "You don't see a %s.\n", arg);
      send_to_char (buffer, ch);
      return;
    }

  if (GET_ITEM_TYPE (obj) == ITEM_WEAPON
      /*&& obj->o.weapon.use_skill != SKILL_CROSSBOW*/ && obj->loaded)
    {
      send_to_char ("You'll need to unload that, first.\n", ch);
      return;
    }

  // Can't wear stringed bows, or put them in to containers.

  if (GET_ITEM_TYPE (obj) == ITEM_WEAPON && 
      (obj->o.weapon.use_skill == SKILL_SHORTBOW || obj->o.weapon.use_skill == SKILL_LONGBOW)
      && obj->o.od.value[5] == 1)
  {
    send_to_char("You'll need to destring that first.\n", ch);
    return;
  }


  // mod by Methuselah for table_lamp

	if (GET_ITEM_TYPE (obj) == ITEM_LIGHT
	   && obj->o.light.on == true
	   && !IS_SET (obj->obj_flags.extra_flags, ITEM_MAGIC))
    {
      put_light_on_table++;
    }
    
  if (!can_obj_to_container (obj, tar, &error, count))
    {
      send_to_char (error, ch);
      return;
    }

  sprintf (buffer, "%s", char_short(ch));
  *buffer = toupper(*buffer);
  old_count = obj->count;
  if (count)
     obj->count = count;
  
  first_person.assign("You put #2");
  first_person.append(obj_short_desc(obj));
  third_person.assign("#5");
  third_person.append(buffer);
  third_person.append("#0 puts #2");
  third_person.append(obj_short_desc(obj));
  
    if (IS_SET (tar->obj_flags.extra_flags, ITEM_TABLE))
  {
	first_person.append("#0 on #2");
	third_person.append("#0 on #2");
  }
  else
  {
	first_person.append("#0 into #2");
	third_person.append("#0 into #2");
  }
  
  first_person.append(obj_short_desc(tar));
  first_person.append("#0");
  third_person.append(obj_short_desc(tar));
  third_person.append("#0");

  obj->count = old_count;
 

  if (evaluate_emote_string (ch, &first_person, third_person, argument))
  {
	obj_from_char (&obj, count);
	obj_to_obj (obj, tar);
  }
  if (put_light_on_table)
    room_light(ch->room);

  return;
}


void
do_give (CHAR_DATA * ch, char *argument, int cmd)
{
  char obj_name[MAX_INPUT_LENGTH];
  char vict_name[MAX_INPUT_LENGTH];
  CHAR_DATA *vict;
  OBJ_DATA *obj;
  int count = 0, error;

  argument = one_argument (argument, obj_name);

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch)
    && str_cmp (ch->room->name, PREGAME_ROOM_NAME))
    {
      send_to_char ("This command has been disabled in OOC zones.\n", ch);
      return;
    }

  if (just_a_number (obj_name))
    {
      count = atoi (obj_name);
      argument = one_argument (argument, obj_name);
    }

  if (IS_SET (ch->act, ACT_MOUNT))
    {
      send_to_char ("Mounts can't use this command.n", ch);
      return;
    }

  if (!*obj_name)
    {
      send_to_char ("Give what?\n", ch);
      return;
    }

  if (!(obj = get_obj_in_dark (ch, obj_name, ch->right_hand)) &&
      !(obj = get_obj_in_dark (ch, obj_name, ch->left_hand)))
    {
      send_to_char ("You do not seem to have anything like that.\n", ch);
      return;
    }

  argument = one_argument (argument, vict_name);

  if (!str_cmp (vict_name, "to"))
    argument = one_argument (argument, vict_name);

  if (!*vict_name)
    {

      if (obj->obj_flags.type_flag == ITEM_TICKET)
	{
	  unstable (ch, obj, NULL);
	  return;
	}

      send_to_char ("Give to whom?\n", ch);
      return;
    }

  if (!(vict = get_char_room_vis (ch, vict_name)))
    {
      send_to_char ("No one by that name around here.\n", ch);
      return;
    }

  if (vict == ch)
    {
      send_to_char ("Give it to yourself? How generous...\n", ch);
      return;
    }

  if (obj->obj_flags.type_flag == ITEM_TICKET)
    {
      unstable (ch, obj, vict);
      return;
    }

  if (IS_NPC (vict) && IS_SET (vict->flags, FLAG_KEEPER) &&
      obj->obj_flags.type_flag == ITEM_MERCH_TICKET)
    {
      redeem_order (ch, obj, vict);
      return;
    }

  if (!can_obj_to_inv (obj, vict, &error, count))
    {
      if (error == NO_HANDS_FULL)
	{
	  act ("$N's hands are currently occupied, I'm afraid.", true, ch,
	       obj, vict, TO_CHAR | _ACT_FORMAT);
	  act ("$n just tried to give you $o, but your hands are full.", true,
	       ch, obj, vict, TO_VICT | _ACT_FORMAT);
	  return;
	}
      if (error == LEFT_BROKEN)
	{
	  act ("$N's left arm is too damaged to hold $o.", true, ch,
	       obj, vict, TO_CHAR | _ACT_FORMAT);
	  act ("$n just tried to give you $o, but your left arm is too damaged to hold it.", true,
	       ch, obj, vict, TO_VICT | _ACT_FORMAT);
	  return;
	}
      if (error == RIGHT_BROKEN)
	{
	  act ("$N's right arm is too damaged to hold $o.", true, ch,
	       obj, vict, TO_CHAR | _ACT_FORMAT);
	  act ("$n just tried to give you $o, but your right arm is too damaged to hold it.", true,
	       ch, obj, vict, TO_VICT | _ACT_FORMAT);
	  return;
	}
      else if (error == NO_TOO_HEAVY || error == NO_RESOURCE)
	{
	  act
	    ("$N struggles beneath the weight of the object, and so you take it back.",
	     true, ch, obj, vict, TO_CHAR | _ACT_FORMAT);
	  act
	    ("$n just tried to give you $o, but it is too heavy for you to carry.",
	     true, ch, obj, vict, TO_VICT | _ACT_FORMAT);
	  return;
	}
      else if (error == NO_CANT_TAKE)
	{
	  act ("This item cannot be given.", false, ch, 0, 0, TO_CHAR);
	  return;
	}
    }

  if (IS_CARRYING_N (vict) + 1 > CAN_CARRY_N (vict))
    {
      if (CAN_CARRY_N (vict) == 0)
	act ("$N isn't capable of carrying anyting.",
	     false, ch, 0, vict, TO_CHAR);
      else
	act ("$N hands are full.", 0, ch, 0, vict, TO_CHAR);
      return;
    }

  if (OBJ_MASS (obj) + IS_CARRYING_W (vict) > CAN_CARRY_W (vict))
    {
      act ("$E can't carry that much weight.", 0, ch, 0, vict, TO_CHAR);
      return;
    }

  obj_from_char (&obj, count);

  act ("$n gives $p to $N.", 1, ch, obj, vict, TO_NOTVICT | _ACT_FORMAT);
  act ("$n gives you $p.", 0, ch, obj, vict, TO_VICT | _ACT_FORMAT);
  act ("You give $p to $N.", 1, ch, obj, vict, TO_CHAR | _ACT_FORMAT);

  obj_to_char (obj, vict);

}

OBJ_DATA *
get_equip (CHAR_DATA * ch, int location)
{
  OBJ_DATA *obj;

  if (location == WEAR_SHIELD)
    {
      if (ch->right_hand && GET_ITEM_TYPE (ch->right_hand) == ITEM_SHIELD)
	return ch->right_hand;
      if (ch->left_hand && GET_ITEM_TYPE (ch->left_hand) == ITEM_SHIELD)
	return ch->left_hand;
      return NULL;
    }

  if (ch->right_hand && ch->right_hand->location == location)
    return ch->right_hand;

  if (ch->left_hand && ch->left_hand->location == location)
    return ch->left_hand;

  if (location != WEAR_SHIELD)
    {
      for (obj = ch->equip; obj; obj = obj->next_content)
	if (obj->location == location)
	  return obj;
    }

  return NULL;
}

void
do_drink (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *container;
  OBJ_DATA *drink;
  char buf[MAX_STRING_LENGTH] = "";
  char buf2[MAX_STRING_LENGTH];
  std::string first_person, third_person;
  int sips = 1, range = 0;
  const char *verbose_liquid_amount [] = {"", "some of the ", "a lot of the ", "most of the ", "all of the "};
  POISON_DATA *poison;
  int poisoned = 0;

  argument = one_argument (argument, buf);

  if (!(container = get_obj_in_dark (ch, buf, ch->right_hand)) &&
      !(container = get_obj_in_list_vis (ch, buf, ch->left_hand)) &&
      !(container = get_obj_in_list_vis (ch, buf, ch->room->contents)))
    {
      act ("You can't find it.", false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  if (GET_ITEM_TYPE (container) != ITEM_DRINKCON &&
      GET_ITEM_TYPE (container) != ITEM_FOUNTAIN)
    {
      act ("You cannot drink from $p.", false, ch, container, 0, TO_CHAR);
      return;
    }

  if (ch->hunger + ch->thirst > 90)
    {
      send_to_char ("You are too full to consume anything else.\n", ch);
      return;
    }

  if (get_soma_affect(ch, SOMA_DIGESTIVE_ULCER))
  {
      act ("The nausea emanating from your stomach drives any thought of drinking far from your mind.", false, ch, 0, 0, TO_CHAR);
      return;
  }

  if (container->o.drinkcon.volume == 0)
    {
      act ("$p is empty.", false, ch, container, 0, TO_CHAR);
      return;
    }

  if (!(drink = vtoo (container->o.drinkcon.liquid)))
    {
      act ("$p appears empty.", false, ch, container, 0, TO_CHAR);
      return;
    }

  if (*argument != '(' && *argument)
  {
	argument = one_argument (argument, buf);
	if (just_a_number(buf))
	{
		if (container->o.drinkcon.volume < atoi(buf) && container->o.drinkcon.volume != -1)
		{
			send_to_char ("There simply isn't that much to drink!\n", ch);
			return;
		}

		if (atoi(buf) < 1)
		{
			send_to_char ("As amusing as regurgitation can be, you may not drink negative amounts.\n", ch);
		}

		sips = atoi(buf);
	}
	else 
	{
		send_to_char ("The correct syntax is #3drink <container> [<amount>] [(emote)]#0.\n", ch);
		return;
	}
  }
  if (sips > 1)
	range = 1;
  if (sips > (container->o.od.value[0] / 3))
	range = 2;
  if (sips > (container->o.od.value[0] * 2 / 3))
	range = 3;
  if (sips == container->o.od.value[0])
	range = 4;
  if (container->o.od.value[1] == -1 && sips > 1)
	range = 1;
  if (container->o.od.value[1] == -1 && sips == 1)
    range = 0;

  sprintf (buf, "%s", char_short(ch));
  *buf = toupper(*buf);
  first_person.assign("You drink ");
  first_person.append(verbose_liquid_amount[range]);
  first_person.append("#2");
  first_person.append(fname(drink->name));
  first_person.append("#0 from #2");
  first_person.append(obj_short_desc(container));
  first_person.append("#0");
  third_person.assign("#5");
  third_person.append(buf);
  third_person.append("#0 drinks ");
  third_person.append(verbose_liquid_amount[range]);
  third_person.append("#2");
  third_person.append(fname(drink->name));
  third_person.append("#0 from #2");
  third_person.append(obj_short_desc(container));
  third_person.append("#0");


  if (evaluate_emote_string(ch, &first_person, third_person, argument))
  {
  	if (ch->intoxication != -1)
  	{
	  if (number (6, 20) > ((ch->con + ch->wil) / 2))
		ch->intoxication += (drink->o.fluid.alcohol * sips);
	  else
		ch->intoxication += ((drink->o.fluid.alcohol * sips) / 2);
	}

          if(container->poison && IS_MORTAL (ch) && (container->obj_flags.type_flag == ITEM_DRINKCON || container->obj_flags.type_flag == ITEM_FOUNTAIN))
          {
            for (poison = container->poison; poison; poison = poison->next)
            {
              if(number(1,25) > GET_CON(ch))
              {
                soma_add_affect(ch, poison->poison_type, poison->duration, poison->latency, 
                        poison->minute, poison->max_power, poison->lvl_power, poison->atm_power,
                        poison->attack, poison->decay, poison->sustain, poison->release);

                  poisoned = poison->poison_type;
              }

              if(container->poison->uses == 1 || container->poison->uses == 0 
               || (container->o.drinkcon.volume - sips <= 0 && container->o.drinkcon.volume != -1))
                remove_object_poison(container, container->poison);
              else if (container->poison->uses != -1)
                container->poison->uses --;
            }
          }

	  if (ch->thirst != -1)
	    ch->thirst += (drink->o.fluid.water * sips);

	  if (ch->hunger != -1)
	    ch->hunger += (drink->o.fluid.food * sips);
	
	  if (ch->thirst < 0 && IS_MORTAL (ch))
	    ch->thirst = 0;
	  if (ch->hunger < 0 && IS_MORTAL (ch))
	    ch->hunger = 0;

/*
		if( ch->intoxication > 0) {
	            sprintf(buf, "You feel %s.\n",
        	        verbal_intox  [get_comestible_range(ch->intoxication)]);
	            send_to_char (buf, ch);
		}
		if ( ch->intoxication > 10 )
			send_to_char ("You feel intoxicated.\n", ch);
*/

	  if (container->o.drinkcon.volume != -1)
	    container->o.drinkcon.volume -= sips;

          // You can "cloak" poison messages, but that makes sense.

          if(poisoned > 0)
            send_to_char(lookup_poison_variable(poisoned, 1), ch);

  }
}


void
do_eat (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_INPUT_LENGTH];
  char buf2[MAX_INPUT_LENGTH];
  OBJ_DATA *obj;
  std::string first_person, third_person;
  int bites = 1, range = 0, old_count = 1;
  const char *verbose_bites_amount [] = {"", "a bite of ", "a few bites of ", "a lot of ", "most of ", "all of "};
  POISON_DATA *poison;
  int poisoned = 0;

  argument = one_argument (argument, buf);

  if (!(obj = get_obj_in_dark (ch, buf, ch->right_hand)) &&
      !(obj = get_obj_in_dark (ch, buf, ch->left_hand)) &&
      !(obj = get_obj_in_dark (ch, buf, ch->equip)))
    {
      send_to_char ("You can't find it.\n", ch);
      return;
    }

  if (obj->obj_flags.type_flag != ITEM_FOOD)
    {
      send_to_char ("That isn't food.  You can't eat it.\n", ch);
      return;
    }

  if (obj->equiped_by && obj->obj_flags.type_flag != ITEM_FOOD)
    {
      send_to_char ("You must remove that item before destroying it.\n", ch);
      return;
    }

  if (ch->hunger + ch->thirst > 90)
    {
      act ("You are too full to eat!", false, ch, 0, 0, TO_CHAR);
      return;
    }

  if (get_soma_affect(ch, SOMA_DIGESTIVE_ULCER))
  {
      act ("The nausea emanating from your stomach drives any thought of eating far from your mind.", false, ch, 0, 0, TO_CHAR);
      return;
  }

  if (*argument != '(' && *argument)
  {
	argument = one_argument (argument, buf);
	if (just_a_number(buf))
	{
		if (obj->o.food.bites < atoi(buf) && obj->o.food.bites != -1)
		{
			send_to_char ("There simply isn't that much to eat!\n", ch);
			return;
		}

		if (atoi(buf) < 1)
		{
			send_to_char ("As amusing as regurgitation can be, you may not eat negative amounts.\n", ch);
			return;
		}

		bites = atoi(buf);
	}
	else if (!strcmp (buf, "all"))
	{
		bites = MAX (1, obj->o.food.bites);
		if ((obj->o.food.food_value + ch->hunger + ch->thirst) > 96)
		{
			send_to_char ("You are much too full to eat that much!\n", ch);
			return;
		}
			
	}
	else 
	{
		send_to_char ("The correct syntax is #3eat <food> [<amount>] [(emote)]#0.\n", ch);
		return;
	}

  }

  if (obj->o.food.bites > 1)
	range = 1;
  if (bites > 1)
	range = 2;
  if (bites > (obj->o.food.bites / 3) && bites != 1 && obj->o.food.bites > 4)
	range = 3;
  if (bites > (obj->o.food.bites * 2 / 3) && bites != 1)
	range = 4;                 
  if (bites == obj->o.food.bites && bites != 1)
	range = 5;
  if (obj->o.food.bites == -1)
	range = 1;

  sprintf(buf, "%s", char_short(ch));
  *buf = toupper(*buf);

  old_count = obj->count;
  obj->count = 1;

  first_person.assign("You eat ");
  first_person.append(verbose_bites_amount[range]);
  first_person.append("#2");
  first_person.append(obj_short_desc(obj));
  first_person.append("#0");
  third_person.assign("#5");
  third_person.append(buf);
  third_person.append("#0 eats ");
  third_person.append(verbose_bites_amount[range]);
  third_person.append("#2");
  third_person.append(obj_short_desc(obj));
  third_person.append("#0");

  obj->count = old_count;

  if (evaluate_emote_string (ch, &first_person, third_person, argument))
  {
	  if (obj->equiped_by)
	    unequip_char (ch, obj->location);	

	for (int i = 0; i < bites; i++)
	{

          if(obj->poison && IS_MORTAL (ch) && obj->obj_flags.type_flag == ITEM_FOOD)
          {
            for (poison = obj->poison; poison; poison = poison->next)
            {

              if(number(1,25) > GET_CON(ch))
              {
                soma_add_affect(ch, poison->poison_type, poison->duration, poison->latency, 
                        poison->minute, poison->max_power, poison->lvl_power, poison->atm_power,
                        poison->attack, poison->decay, poison->sustain, poison->release);

                  poisoned = poison->poison_type;
              }

              if(obj->poison->uses == 0 || obj->poison->uses == 1)
                remove_object_poison(obj, obj->poison);
              else if (obj->poison->uses != -1)
                obj->poison->uses --;
            }
          }

	  ch->hunger += get_bite_value (obj);
	  obj->o.food.bites--;

	  if (obj->count > 1 && obj->o.food.bites < 1)
	    {
	      obj->count--;
	      obj->o.food.bites = vtoo (obj->nVirtual)->o.food.bites;
	    }
	  else if (obj->o.food.bites < 1 && obj->count <= 1)
	    extract_obj (obj);
	  else if (!IS_MORTAL (ch))
	    {
	      extract_obj (obj);
	      return;
	    }
	}

	  if (ch->hunger > 48)
	    ch->hunger = 48;
	  if (ch->hunger > 36)
	    act ("You are full.", false, ch, 0, 0, TO_CHAR);

          if(poisoned > 0)
            send_to_char(lookup_poison_variable(poisoned, 1), ch);

  }
}

void
do_fill (CHAR_DATA * ch, char *argument, int cmd)
{
  int volume_to_transfer;
  OBJ_DATA *from;
  OBJ_DATA *to;
  OBJ_DATA *fuel;
  char buf[MAX_STRING_LENGTH];
  POISON_DATA *poison;

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("FILL <object> from/with <object>\n", ch);
      send_to_char ("Example:  fill bucket from well\n", ch);
      send_to_char ("          fill bucket well      (same thing)\n", ch);
      return;
    }

  if (!(to = get_obj_in_dark (ch, buf, ch->right_hand)) &&
      !(to = get_obj_in_list_vis (ch, buf, ch->left_hand)) &&
      !(to = get_obj_in_list_vis (ch, buf, ch->room->contents)))
    {

      if (get_obj_in_dark (ch, buf, ch->room->contents))
	{
	  send_to_char ("It is too dark for that.\n", ch);
	  return;
	}

      send_to_char ("Fill what?\n", ch);

      return;
    }

  if (GET_ITEM_TYPE (to) != ITEM_DRINKCON && GET_ITEM_TYPE (to) != ITEM_LIGHT)
    {
      act ("You can't fill $p.", false, ch, to, 0, TO_CHAR);
      return;
    }

  if (GET_ITEM_TYPE (to) == ITEM_LIGHT &&
      is_name_in_list ("candle", to->name))
    {
      act ("You can't do that with $p.", false, ch, to, 0, TO_CHAR);
      return;
    }

  if (to->o.drinkcon.volume == -1 ||
      (to->o.drinkcon.volume >= to->o.drinkcon.capacity))
    {
      act ("$p is full already.", false, ch, to, 0, TO_CHAR);
      return;
    }

  argument = one_argument (argument, buf);

  if (!str_cmp (buf, "with") || !str_cmp (buf, "from"))
    argument = one_argument (argument, buf);

  if (!*buf)
    {

      for (from = ch->room->contents; from; from = from->next_content)
	if (GET_ITEM_TYPE (from) == ITEM_FOUNTAIN && CAN_SEE_OBJ (ch, from))
	  break;

      if (!from)
	{
	  act ("Fill $p from what?", false, ch, to, 0, TO_CHAR);
	  return;
	}
    }

  else if (!(from = get_obj_in_dark (ch, buf, ch->room->contents)))
    {
      act ("Fill $p from what?", false, ch, to, 0, TO_CHAR);
      return;
    }

  if (GET_ITEM_TYPE (from) != ITEM_FOUNTAIN &&
      GET_ITEM_TYPE (from) != ITEM_DRINKCON)
    {
      act ("There is no way to fill $p from $P.",
	   false, ch, to, from, TO_CHAR);
      return;
    }

  if (!from->o.drinkcon.volume)
    {
      act ("$p is empty.", false, ch, from, 0, TO_CHAR);
      return;
    }

  if (GET_ITEM_TYPE (to) == ITEM_LIGHT &&
      to->o.light.liquid != from->o.drinkcon.liquid)
    {

      if (!(fuel = vtoo (to->o.light.liquid)))
	{
	  act ("$p is broken.", false, ch, to, 0, TO_CHAR);
	  return;
	}

      act ("$p only burns $O.", false, ch, to, fuel, TO_CHAR);

      if ((fuel = vtoo (from->o.drinkcon.liquid)))
	act ("$p contains $O.", false, ch, from, fuel, TO_CHAR);

      return;
    }

  if (to->o.drinkcon.liquid && to->o.drinkcon.volume
      && to->o.drinkcon.liquid != from->o.drinkcon.liquid)
    {
      send_to_char ("You shouldn't mix fluids.\n", ch);
      return;
    }

  sprintf (buf, "You fill $p from $P with %s.",
	   vnum_to_liquid_name (from->o.drinkcon.liquid));
  act (buf, false, ch, to, from, TO_CHAR | _ACT_FORMAT);

  sprintf (buf, "$n fills $p from $P with %s.",
	   vnum_to_liquid_name (from->o.drinkcon.liquid));
  act (buf, true, ch, to, from, TO_ROOM | _ACT_FORMAT);

  if (GET_ITEM_TYPE (to) == ITEM_DRINKCON && !to->o.drinkcon.volume)
    to->o.drinkcon.liquid = from->o.drinkcon.liquid;

  volume_to_transfer = from->o.drinkcon.volume;

  if (from->o.drinkcon.volume == -1)
    volume_to_transfer = to->o.drinkcon.capacity;

  if (volume_to_transfer > (to->o.drinkcon.capacity - to->o.drinkcon.volume))
    volume_to_transfer = to->o.drinkcon.capacity - to->o.drinkcon.volume;

  if (from->o.drinkcon.volume != -1)
    from->o.drinkcon.volume -= volume_to_transfer;

  if (to->o.drinkcon.volume != -1)
    to->o.drinkcon.volume += volume_to_transfer;

  /* Transfer a poison */

  if (GET_ITEM_TYPE (from) == ITEM_DRINKCON &&
      GET_ITEM_TYPE (to) == ITEM_DRINKCON && from->poison)
  {
    CREATE (poison, POISON_DATA, 1);
    poison->poison_type = from->poison->poison_type;
    poison->duration = from->poison->duration;
    poison->latency = from->poison->latency;
    poison->minute = from->poison->minute;
    poison->max_power = from->poison->max_power;
    poison->lvl_power = from->poison->lvl_power;
    poison->atm_power = from->poison->atm_power;
    poison->attack = from->poison->attack;
    poison->decay = from->poison->decay;
    poison->sustain = from->poison->sustain;
    poison->release = from->poison->release;
    poison->uses = volume_to_transfer;
    poison->next = NULL;
 
    poison_to_obj(to, poison);
  
    if(from->poison->uses > volume_to_transfer)
      from->poison->uses -= volume_to_transfer;
    else
      remove_object_poison(from, from->poison);

  }

  if (GET_ITEM_TYPE (from) == ITEM_LIGHT &&
      !from->o.light.hours && from->o.light.on)
    {
      act ("$p is extinguished.", false, ch, from, 0, TO_CHAR);
      act ("$p is extinguished.", false, ch, from, 0, TO_ROOM);
      from->o.light.on = 0;
    }

}

void
do_fillx (CHAR_DATA * ch, char *argument, int cmd)
{
  char arg1[80], arg2[80];
  OBJ_DATA *from, *to;
  bool found;

  argument_interpreter (argument, arg1, arg2);
  found = false;

  if (!arg1[0])
    {
      send_to_char ("Fill what?\n", ch);
      return;
    }
  if (!(to = get_obj_in_list_vis (ch, arg1, ch->right_hand)) &&
      !(to = get_obj_in_list_vis (ch, arg1, ch->left_hand)))
    {
      if (to != get_obj_in_dark (ch, arg1, ch->right_hand))
	send_to_char ("It's too dark for that.\n", ch);
      else
	send_to_char ("You don't have it!\n", ch);
      return;
    }
  if (!(to->obj_flags.type_flag == ITEM_DRINKCON))
    {
      send_to_char ("You can't fill that!\n", ch);
      return;
    }
  if (to->o.od.value[1] == to->o.od.value[0])
    {
      send_to_char ("It is already full.\n", ch);
      return;
    }
  if (!arg2[0])
    {
      for (from = ch->room->contents; from; from = from->next_content)
	{
	  if (from->obj_flags.type_flag == ITEM_FOUNTAIN)
	    {
	      found = true;
	      act ("$n fills $p from $P.", true, ch, to, from, TO_ROOM);
	      act ("You fill $p from $P.", false, ch, to, from, TO_CHAR);

	      to->o.od.value[1] = to->o.od.value[0];
	      to->o.od.value[2] = from->o.od.value[2];

	      return;
	    }
	}
      if (!found)
	{
	  send_to_char ("Could not find anything to fill from.\n", ch);
	  return;
	}


    }
  if (!(from = get_obj_in_list_vis (ch, arg2, ch->room->contents)))
    {
      send_to_char ("Doesn't seem to be anything like that here.\n", ch);
      return;
    }
  if (!(from->obj_flags.type_flag == ITEM_FOUNTAIN))
    {
      send_to_char ("You can't fill anything from that!\n", ch);
      return;
    }
  act ("$n fills $p from $P.", true, ch, to, from, TO_ROOM);
  act ("You fill $p from $P.", false, ch, to, from, TO_CHAR);

  to->o.od.value[1] = to->o.od.value[0];
  to->o.od.value[2] = from->o.od.value[2];
  return;

}

void
do_pour (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *from;
  OBJ_DATA *to;
  int volume_to_transfer;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  POISON_DATA *poison;

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("What do you want to pour?\n", ch);
      return;
    }

  if (!(from = get_obj_in_dark (ch, buf, ch->right_hand)) &&
      !(from = get_obj_in_dark (ch, buf, ch->left_hand)))
    {
      send_to_char ("You can't find it.\n", ch);
      return;
    }

  if (GET_ITEM_TYPE (from) != ITEM_DRINKCON &&
      GET_ITEM_TYPE (from) != ITEM_LIGHT)
    {
      act ("You can't pour from $p.\n", false, ch, from, 0, TO_CHAR);
      return;
    }

  if (GET_ITEM_TYPE (from) == ITEM_LIGHT &&
      is_name_in_list ("candle", from->name))
    {
      send_to_char ("You can't pour wax from a candle (yet).\n", ch);
      return;
    }

  if (GET_ITEM_TYPE (from) == ITEM_LIGHT && !from->o.light.hours)
    {
      act ("$p contains no fuel.", false, ch, from, 0, TO_CHAR);
      return;
    }

  if (GET_ITEM_TYPE (from) == ITEM_DRINKCON && !from->o.drinkcon.volume)
    {
      act ("$p is empty.", false, ch, from, 0, TO_CHAR);
      return;
    }

  argument = one_argument (argument, buf2);

  if (!str_cmp (buf2, "out"))
    {
      do_empty (ch, buf, 0);
      return;
    }

  if (!*buf2)
    {
      send_to_char ("What do you want to pour it into?", ch);
      return;
    }

  if (!(to = get_obj_in_dark (ch, buf2, ch->right_hand)) &&
      !(to = get_obj_in_list_vis (ch, buf2, ch->left_hand)) &&
      !(to = get_obj_in_list_vis (ch, buf2, ch->room->contents)))
    {
      act ("You can't find it to pour $p into.", false, ch, from, 0,
	   TO_CHAR | _ACT_FORMAT);
      return;
    }

  if (GET_ITEM_TYPE (to) != ITEM_DRINKCON && GET_ITEM_TYPE (to) != ITEM_LIGHT)
    {
      act ("You can't pour into $p.", false, ch, to, 0, TO_CHAR);
      return;
    }

  if (GET_ITEM_TYPE (to) == ITEM_LIGHT &&
      !to->o.light.hours && to->o.light.on)
    to->o.light.on = 0;

  if (GET_ITEM_TYPE (to) == ITEM_LIGHT)
    {

      if (is_name_in_list ("candle", to->name))
	{
	  send_to_char ("You can't pour it into a candle.", ch);
	  return;
	}

      if (from->o.light.liquid != to->o.light.liquid)
	{
	  sprintf (buf, "$p only burns %s.",
		   vnum_to_liquid_name (to->o.light.liquid));
	  act (buf, false, ch, to, 0, TO_CHAR);
	  return;
	}
    }

  if (to->o.drinkcon.volume &&
      from->o.drinkcon.liquid != to->o.drinkcon.liquid)
    {
      act ("If you want to fill $p with another liquid, then empty it first.",
	   false, ch, to, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  if (to->o.drinkcon.capacity == to->o.drinkcon.volume)
    {
      act ("$p is already full.", false, ch, to, 0, TO_CHAR);
      return;
    }

  sprintf (buf, "You pour %s from $p into $P.",
	   vnum_to_liquid_name (from->o.drinkcon.liquid));
  act (buf, false, ch, from, to, TO_CHAR | _ACT_FORMAT);

  sprintf (buf, "$n pours %s from $p into $P.",
	   vnum_to_liquid_name (from->o.drinkcon.liquid));
  act (buf, true, ch, from, to, TO_ROOM | _ACT_FORMAT);

  if (GET_ITEM_TYPE (to) == ITEM_DRINKCON && !to->o.drinkcon.volume)
    to->o.drinkcon.liquid = from->o.drinkcon.liquid;

  volume_to_transfer = from->o.drinkcon.volume;
  if (volume_to_transfer > (to->o.drinkcon.capacity - to->o.drinkcon.volume))
    volume_to_transfer = to->o.drinkcon.capacity - to->o.drinkcon.volume;

  from->o.drinkcon.volume -= volume_to_transfer;

  to->o.drinkcon.volume += volume_to_transfer;

  /* Transfer a poison */

  if (GET_ITEM_TYPE (from) == ITEM_DRINKCON &&
      GET_ITEM_TYPE (to) == ITEM_DRINKCON && from->poison)
  {
    CREATE (poison, POISON_DATA, 1);
    poison->poison_type = from->poison->poison_type;
    poison->duration = from->poison->duration;
    poison->latency = from->poison->latency;
    poison->minute = from->poison->minute;
    poison->max_power = from->poison->max_power;
    poison->lvl_power = from->poison->lvl_power;
    poison->atm_power = from->poison->atm_power;
    poison->attack = from->poison->attack;
    poison->decay = from->poison->decay;
    poison->sustain = from->poison->sustain;
    poison->release = from->poison->release;
    poison->uses = volume_to_transfer >= from->poison->uses ? from->poison->uses : volume_to_transfer;
    poison->next = NULL;
 
    poison_to_obj(to, poison);
  
    if(from->poison->uses > volume_to_transfer)
      from->poison->uses -= volume_to_transfer;
    else
      remove_object_poison(from, from->poison);

  }

  if (GET_ITEM_TYPE (from) == ITEM_LIGHT &&
      !from->o.light.hours && from->o.light.on)
    {
      act ("$p is extinguished.", false, ch, from, 0, TO_CHAR);
      act ("$p is extinguished.", false, ch, from, 0, TO_ROOM);
      from->o.light.on = 0;
    }
}

/* functions related to wear */

void
perform_wear (CHAR_DATA * ch, OBJ_DATA * obj, int keyword)
{
  switch (keyword)
    {

    case 0:

      if (obj->o.light.hours < 1)
	{
	  act ("You hold $p and realize it is spent.",
	       true, ch, obj, 0, TO_CHAR);
	}

      else
	{
	  if (!obj->o.light.on)
	    {
	      act ("You light $p and hold it.", false, ch, obj, 0, TO_CHAR);
	      act ("$n lights $p and holds it.", false, ch, obj, 0, TO_ROOM);
	    }
	  else
	    {
	      act ("You hold $p.", false, ch, obj, 0, TO_CHAR);
	      act ("$n holds $p.", false, ch, obj, 0, TO_ROOM);
	    }
	}
      break;

    case 1:
      act ("$n wears $p on $s finger.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      break;
    case 2:
      act ("$n wears $p around $s neck.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p around your neck.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 3:
      act ("$n wears $p on $s body.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p on your body.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      if(!obj->size)
        obj->size = get_size (ch);
      break;
    case 4:
      act ("$n wears $p on $s head.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p on your head.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 5:
      act ("$n wears $p on $s legs.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p on your legs.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      if(!obj->size)
        obj->size = get_size (ch);
      break;
    case 6:
      act ("$n wears $p on $s feet.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p on your feet.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 7:
      act ("$n wears $p on $s hands.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p on your hands.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 8:
      act ("$n wears $p on $s arms.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p on your arms.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      if(!obj->size)
        obj->size = get_size (ch);
      break;
    case 9:
      act ("$n wears $p about $s body.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p about your body.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 10:
      act ("$n wears $p about $s waist.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p about your waist.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 11:
      act ("$n wears $p around $s wrist.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      break;
    case 12:
      act ("$n wields $p.", true, ch, obj, 0, TO_ROOM | _ACT_FORMAT);
      act ("You wield $p.", true, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
      break;
    case 13:
      act ("$n grabs $p.", true, ch, obj, 0, TO_ROOM | _ACT_FORMAT);
      act ("You grab $p.", true, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
      break;
    case 14:
      act ("$n starts using $p.", true, ch, obj, 0, TO_ROOM | _ACT_FORMAT);
      break;
    case 15:
      act ("$n affixes $p to $s belt.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You affix $p to your belt.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 16:
      act ("$n stores $p across $s back.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You store $p across your back.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 17:
      break;
    case 18:
      act ("$n wears $p around $s neck.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p around your neck.", false, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 19:
      act ("$n wears $p on $s ears.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p on your ears.", false, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 20:
      act ("$n slings $p over $s shoulder.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      break;
    case 21:
      act ("$n wears $p around $s ankle.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      break;
    case 22:
      act ("$n wears $p in $s hair.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p in your hair.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 23:
      act ("$n wears $p on $s face.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      act ("You wear $p on your face.", true, ch, obj, 0,
	   TO_CHAR | _ACT_FORMAT);
      break;
    case 24:
    case 25:
    case 26:
      act ("$n wears $p around $s upper arm.", true, ch, obj, 0,
	   TO_ROOM | _ACT_FORMAT);
      /* TO_CHAR act occurs in wear() */
      break;
    }
}

void
wear (CHAR_DATA * ch, OBJ_DATA * obj_object, int keyword)
{
  char buffer[MAX_STRING_LENGTH];
  char *hour_names[] = {
    "",
    "the remainder of the",
    "two", "three", "four", "five", "six", "seven", "eight", "nine",
    "ten", "eleven", "twelve", "quite a few"
  };
  int hours;

  if (IS_SET (obj_object->obj_flags.extra_flags, ITEM_MOUNT) &&
      !IS_SET (ch->act, ACT_MOUNT))
    {
      send_to_char ("That item is for animals.\n", ch);
      return;
    }

  if (!IS_SET (obj_object->obj_flags.extra_flags, ITEM_MOUNT) &&
      IS_SET (ch->act, ACT_MOUNT))
    {
      send_to_char ("You are an animal.  That item isn't for you.\n", ch);
      return;
    }

  if (obj_object->loaded && GET_ITEM_TYPE (obj_object) == ITEM_WEAPON /*&&
      obj_object->o.weapon.use_skill != SKILL_CROSSBOW*/)
    {
      send_to_char ("You'll need to unload that, first.\n", ch);
      return;
    }

  // Can't wear a strung bow, to prevent the instant sniper-to-melee problem.

  if (keyword != 12 && GET_ITEM_TYPE (obj_object) == ITEM_WEAPON && 
      (obj_object->o.weapon.use_skill == SKILL_SHORTBOW || obj_object->o.weapon.use_skill == SKILL_LONGBOW)
      && obj_object->o.od.value[5] == 1)
  {
    send_to_char("You'll need to destring that first.\n", ch);
    return;
  }


  if (GET_ITEM_TYPE (obj_object) == ITEM_LIGHT
      && obj_object->o.light.on == true
      && !IS_SET (obj_object->obj_flags.extra_flags, ITEM_MAGIC))
    {
      send_to_char ("You'll need to snuff that, first.\n", ch);
      return;
    }


  if (keyword == 13 && obj_object->obj_flags.type_flag == ITEM_LIGHT)
    keyword = 0;

  switch (keyword)
    {

    case 0:			/* LIGHT SOURCE */
      if (get_equip (ch, WEAR_LIGHT))
	send_to_char ("You are already holding a light source.\n", ch);

      else if (!can_handle (obj_object, ch))
	send_to_char ("Your hands are occupied.\n", ch);

      else
	{
	  perform_wear (ch, obj_object, keyword);
	  if (obj_object == ch->right_hand)
	    ch->right_hand = NULL;
	  else if (obj_object == ch->left_hand)
	    ch->left_hand = NULL;
	  equip_char (ch, obj_object, WEAR_LIGHT);

	  light (ch, obj_object, true, false);

	  hours = obj_object->o.light.hours;

	  if (!hours)
	    {
	      send_to_char ("It doesn't produce light.\n", ch);
	      break;
	    }

	  sprintf (buffer, "It will provide light for %s hour%s.\n",
		   hours < 13 ? hour_names[hours] : hour_names[13],
		   hours > 1 ? "s" : "");

	  send_to_char (buffer, ch);
	}

      break;

    case 1:
      if (CAN_WEAR (obj_object, ITEM_WEAR_FINGER))
	{

	  if (get_equip (ch, WEAR_FINGER_L) && get_equip (ch, WEAR_FINGER_R))
	    send_to_char ("You are already wearing something on your "
			  "ring fingers.\n", ch);

	  else
	    {

	      perform_wear (ch, obj_object, keyword);

	      if (get_equip (ch, WEAR_FINGER_L))
		{
		  sprintf (buffer,
			   "You slip the %s on your right ring finger.\n",
			   fname (obj_object->name));
		  send_to_char (buffer, ch);
		  if (obj_object == ch->right_hand)
		    ch->right_hand = NULL;
		  else if (obj_object == ch->left_hand)
		    ch->left_hand = NULL;
		  equip_char (ch, obj_object, WEAR_FINGER_R);
		}

	      else
		{
		  sprintf (buffer,
			   "You slip the %s on your left ring finger.\n",
			   fname (obj_object->name));
		  send_to_char (buffer, ch);
		  if (obj_object == ch->right_hand)
		    ch->right_hand = NULL;
		  else if (obj_object == ch->left_hand)
		    ch->left_hand = NULL;
		  equip_char (ch, obj_object, WEAR_FINGER_L);
		}
	    }
	}
      else
	send_to_char ("You can't wear that on your ring finger.\n", ch);
      break;

    case 2:
      if (CAN_WEAR (obj_object, ITEM_WEAR_NECK))
	{
	  if (get_equip (ch, WEAR_NECK_1) || get_equip (ch, WEAR_NECK_2))
	    send_to_char ("You can only wear one thing around your "
			  "neck.\n", ch);
	  else
	    {
	      perform_wear (ch, obj_object, keyword);
	      if (get_equip (ch, WEAR_NECK_1))
		{
		  if (obj_object == ch->right_hand)
		    ch->right_hand = NULL;
		  else if (obj_object == ch->left_hand)
		    ch->left_hand = NULL;
		  equip_char (ch, obj_object, WEAR_NECK_2);
		}
	      else
		{
		  if (obj_object == ch->right_hand)
		    ch->right_hand = NULL;
		  else if (obj_object == ch->left_hand)
		    ch->left_hand = NULL;
		  equip_char (ch, obj_object, WEAR_NECK_1);
		}
	    }
	}
      else
	send_to_char ("You can't wear that around your neck.\n", ch);
      break;

    case 3:
      if (CAN_WEAR (obj_object, ITEM_WEAR_BODY))
	{
	  if (get_equip (ch, WEAR_BODY))
	    send_to_char ("You already wear something on your body."
			  "\n", ch);
	  else if (obj_object->size && obj_object->size != get_size (ch))
	    {
	      if (obj_object->size > get_size (ch))
		act ("$p won't fit, it is too large.",
		     true, ch, obj_object, 0, TO_CHAR);
	      else
		act ("$p won't fit, it is too small.",
		     true, ch, obj_object, 0, TO_CHAR);
	    }

	  else
	    {
	      perform_wear (ch, obj_object, keyword);
	      if (obj_object == ch->right_hand)
		ch->right_hand = NULL;
	      else if (obj_object == ch->left_hand)
		ch->left_hand = NULL;
	      equip_char (ch, obj_object, WEAR_BODY);
	    }
	}
      else
	send_to_char ("You can't wear that on your body.\n", ch);
      break;

    case 4:
      if (CAN_WEAR (obj_object, ITEM_WEAR_HEAD))
	{
	  if (get_equip (ch, WEAR_HEAD))
	    send_to_char ("You already wear something on your head."
			  "\n", ch);
	  else
	    {
	      perform_wear (ch, obj_object, keyword);
	      if (obj_object == ch->right_hand)
		ch->right_hand = NULL;
	      else if (obj_object == ch->left_hand)
		ch->left_hand = NULL;
	      equip_char (ch, obj_object, WEAR_HEAD);
	    }
	}
      else
	send_to_char ("You can't wear that on your head.\n", ch);
      break;

    case 5:
      if (CAN_WEAR (obj_object, ITEM_WEAR_LEGS))
	{
	  if (get_equip (ch, WEAR_LEGS))
	    send_to_char ("You already wear something on your legs."
			  "\n", ch);
	  else if (obj_object->size && obj_object->size != get_size (ch))
	    {
	      if (obj_object->size > get_size (ch))
		act ("$p won't fit, it is too large.",
		     true, ch, obj_object, 0, TO_CHAR);
	      else
		act ("$p won't fit, it is too small.",
		     true, ch, obj_object, 0, TO_CHAR);
	    }

	  else
	    {
	      perform_wear (ch, obj_object, keyword);
	      if (obj_object == ch->right_hand)
		ch->right_hand = NULL;
	      else if (obj_object == ch->left_hand)
		ch->left_hand = NULL;
	      equip_char (ch, obj_object, WEAR_LEGS);
	    }
	}
      else
	send_to_char ("You can't wear that on your legs.\n", ch);
      break;

    case 6:
      if (CAN_WEAR (obj_object, ITEM_WEAR_FEET))
	{
	  if (get_equip (ch, WEAR_FEET))
	    send_to_char ("You already wear something on your feet."
			  "\n", ch);
	  else
	    {
	      perform_wear (ch, obj_object, keyword);
	      obj_from_char (&obj_object, 0);
	      equip_char (ch, obj_object, WEAR_FEET);
	    }
	}
      else
	send_to_char ("You can't wear that on your feet.\n", ch);
      break;

    case 7:
      if (CAN_WEAR (obj_object, ITEM_WEAR_HANDS))
	{
	  if (get_equip (ch, WEAR_HANDS))
	    send_to_char ("You already wear something on your hands."
			  "\n", ch);
	  else
	    {
	      perform_wear (ch, obj_object, keyword);
	      if (obj_object == ch->right_hand)
		ch->right_hand = NULL;
	      else if (obj_object == ch->left_hand)
		ch->left_hand = NULL;
	      equip_char (ch, obj_object, WEAR_HANDS);
	    }
	}
      else
	send_to_char ("You can't wear that on your hands.\n", ch);
      break;

    case 8:
      if (CAN_WEAR (obj_object, ITEM_WEAR_ARMS))
	{
	  if (get_equip (ch, WEAR_ARMS))
	    send_to_char ("You already wear something on your arms."
			  "\n", ch);
	  else if (obj_object->size && obj_object->size != get_size (ch))
	    {
	      if (obj_object->size > get_size (ch))
		act ("$p won't fit, it is too large.",
		     true, ch, obj_object, 0, TO_CHAR);
	      else
		act ("$p won't fit, it is too small.",
		     true, ch, obj_object, 0, TO_CHAR);
	    }

	  else
	    {
	      perform_wear (ch, obj_object, keyword);
	      if (obj_object == ch->right_hand)
		ch->right_hand = NULL;
	      else if (obj_object == ch->left_hand)
		ch->left_hand = NULL;
	      equip_char (ch, obj_object, WEAR_ARMS);
	    }
	}
      else
	send_to_char ("You can't wear that on your arms.\n", ch);
      break;

    case 9:
      if (CAN_WEAR (obj_object, ITEM_WEAR_ABOUT))
	{
	  if (get_equip (ch, WEAR_ABOUT))
	    {
	      send_to_char ("You already wear something about your body.\n",
			    ch);
	    }
	  else
	    {
	      perform_wear (ch, obj_object, keyword);
	      if (obj_object == ch->right_hand)
		ch->right_hand = NULL;
	      else if (obj_object == ch->left_hand)
		ch->left_hand = NULL;
	      equip_char (ch, obj_object, WEAR_ABOUT);
	    }
	}
      else
	{
	  send_to_char ("You can't wear that about your body.\n", ch);
	}
      break;
    case 10:
      {
	if (CAN_WEAR (obj_object, ITEM_WEAR_WAIST))
	  {
	    if (get_equip (ch, WEAR_WAIST))
	      {
		send_to_char
		  ("You already wear something about your waist.\n", ch);
	      }
	    else
	      {
		perform_wear (ch, obj_object, keyword);
		if (obj_object == ch->right_hand)
		  ch->right_hand = NULL;
		else if (obj_object == ch->left_hand)
		  ch->left_hand = NULL;
		equip_char (ch, obj_object, WEAR_WAIST);
	      }
	  }
	else
	  {
	    send_to_char ("You can't wear that about your waist.\n", ch);
	  }
      }
      break;
    case 11:
      {
	if (CAN_WEAR (obj_object, ITEM_WEAR_WRIST))
	  {
	    if (get_equip (ch, WEAR_WRIST_L) && get_equip (ch, WEAR_WRIST_R))
	      {
		send_to_char
		  ("You already wear something around both your wrists.\n",
		   ch);
	      }
	    else
	      {
		perform_wear (ch, obj_object, keyword);
		if (obj_object == ch->right_hand)
		  ch->right_hand = NULL;
		else if (obj_object == ch->left_hand)
		  ch->left_hand = NULL;
		if (get_equip (ch, WEAR_WRIST_L))
		  {
		    sprintf (buffer,
			     "You wear the %s around your right wrist.\n",
			     fname (obj_object->name));
		    send_to_char (buffer, ch);
		    equip_char (ch, obj_object, WEAR_WRIST_R);
		  }
		else
		  {
		    sprintf (buffer,
			     "You wear the %s around your left wrist.\n",
			     fname (obj_object->name));
		    send_to_char (buffer, ch);
		    equip_char (ch, obj_object, WEAR_WRIST_L);
		  }
	      }
	  }
	else
	  {
	    send_to_char ("You can't wear that around your wrist.\n", ch);
	  }
      }
      break;

    case 12:
      if (CAN_WEAR (obj_object, ITEM_WIELD) && !(obj_object->o.od.value[0] == 0))
	{
/*
				if ( !can_handle (obj_object, ch) ) {
					send_to_char ("Your hands are occupied!\n", ch);
					return;
				}
*/
	  switch (obj_object->o.od.value[0])
	    {
	    case 1:	// primary weapons

	      if (ch->str < 18)
		{
		  if (get_equip (ch, WEAR_PRIM))
		    {
		      send_to_char
			("You are already wielding a primary weapon.\n", ch);
		      return;
		    }
		  else if (get_equip (ch, WEAR_BOTH))
		    {
		      send_to_char
			("You are already wielding a two-handed weapon.\n",
			 ch);
		      return;
		    }
		  else
		    {
		      send_to_char ("OK.\n", ch);
		      perform_wear (ch, obj_object, keyword);
		      equip_char (ch, obj_object, WEAR_PRIM);
		    }
		  break;
		}		// > 17 str or troll wields ME in either hand.

	    case 2:	// Light weapons, and should include brawling weapons.
	    case SKILL_SLING:
	      if (get_equip (ch, WEAR_PRIM) && get_equip (ch, WEAR_SEC))
		{
		  send_to_char
		    ("You are already wielding both a primary and a secondary weapon.\n",
		     ch);
		  return;
		}
	      if (get_equip (ch, WEAR_BOTH))
		{
		  send_to_char
		    ("You are already wielding a two-handed weapon.\n", ch);
		  return;
		}
	      send_to_char ("OK.\n", ch);
	      perform_wear (ch, obj_object, keyword);
	      if (!get_equip (ch, WEAR_PRIM))
		equip_char (ch, obj_object, WEAR_PRIM);
	      else
		equip_char (ch, obj_object, WEAR_SEC);
	      break;

	    case 4:
	      if (get_equip (ch, WEAR_BOTH) ||
		  get_equip (ch, WEAR_PRIM) || get_equip (ch, WEAR_SEC))
		{
		  send_to_char
		    ("You cannot wield this weapon while wielding another.\n",
		     ch);
		  return;
		}
	      send_to_char ("OK.\n", ch);
	      perform_wear (ch, obj_object, keyword);
	      equip_char (ch, obj_object, WEAR_PRIM);
	      break;

	    case 3:	// Heavy weapons.
	      if ((get_equip (ch, WEAR_PRIM) || get_equip (ch, WEAR_BOTH))
		  && (ch->race == 12 || ch->race == 13))
		{		// Trolls can wield heavy weapons in either hand.
		  if (get_equip (ch, WEAR_PRIM) && get_equip (ch, WEAR_SEC))
		    {
		      send_to_char
			("You are already wielding both a primary and a secondary weapon.\n",
			 ch);
		      return;
		    }
		  send_to_char ("OK.\n", ch);
		  perform_wear (ch, obj_object, keyword);
		  if (!get_equip (ch, WEAR_PRIM) && !get_equip (ch, WEAR_SEC)
		      && !get_equip (ch, WEAR_BOTH))
		    equip_char (ch, obj_object, WEAR_BOTH);
		  else if (!get_equip (ch, WEAR_PRIM))
		    equip_char (ch, obj_object, WEAR_PRIM);
		  else
		    equip_char (ch, obj_object, WEAR_SEC);
		  break;
		}
	      if (ch->str >= 20)
		{		// Extremely strong chars can wield two-handed weapons with one hand.
		  if (get_equip (ch, WEAR_PRIM))
		    {
		      send_to_char
			("You are already wielding a primary weapon.\n", ch);
		      return;
		    }
		  else if (get_equip (ch, WEAR_BOTH))
		    {
		      send_to_char
			("You are already wielding a two-handed weapon.\n",
			 ch);
		      return;
		    }
		  else
		    {
		      send_to_char ("OK.\n", ch);
		      perform_wear (ch, obj_object, keyword);
		      if (get_equip (ch, WEAR_SEC)
			  || (ch->right_hand && ch->left_hand))
			equip_char (ch, obj_object, WEAR_PRIM);
		      else
			equip_char (ch, obj_object, WEAR_BOTH);
		    }
		  break;
		}
	      if (get_equip (ch, WEAR_BOTH) ||
		  get_equip (ch, WEAR_PRIM) ||
		  get_equip (ch, WEAR_SEC) ||
		  (ch->right_hand && ch->left_hand))
		{
		  send_to_char ("You need both hands to wield this weapon.\n",
				ch);
		  return;
		}
	      send_to_char ("OK.\n", ch);
	      perform_wear (ch, obj_object, keyword);
	      equip_char (ch, obj_object, WEAR_BOTH);
	      break;
	    }

	}
      else
	{
	  send_to_char ("You can't wield that.\n", ch);
	}
      break;

    case 14:
      {
	if (CAN_WEAR (obj_object, ITEM_WEAR_SHIELD))
	  {
	    if (get_equip (ch, WEAR_SHIELD))
	      send_to_char ("You are already using a shield.\n", ch);

	    else if (!can_handle (obj_object, ch))
	      send_to_char ("Your hands are occupied.\n", ch);

	    else
	      {
		perform_wear (ch, obj_object, keyword);
		sprintf (buffer, "You start using the %s.\n",
			 fname (obj_object->name));
		send_to_char (buffer, ch);
		equip_char (ch, obj_object, WEAR_SHIELD);
	      }
	  }
	else
	  {
	    send_to_char ("You can't use that as a shield.\n", ch);
	  }
      }
      break;
    case 15:
      if (!CAN_WEAR (obj_object, ITEM_WEAR_BELT))
	send_to_char ("You cannot wear that on your belt.\n", ch);

      else if (!get_equip (ch, WEAR_WAIST))
	send_to_char ("You need a belt to wear that.\n", ch);

      else if (get_equip (ch, WEAR_BELT_1) && get_equip (ch, WEAR_BELT_2))
	send_to_char ("Your belt is full.\n", ch);

      else
	{
	  int belt_loc;

	  /* Mostly I expect pouches to be equiped here.
	     put them in the second belt loc first */

	  if (!get_equip (ch, WEAR_BELT_2))
	    belt_loc = WEAR_BELT_2;
	  else
	    belt_loc = WEAR_BELT_1;

	  perform_wear (ch, obj_object, keyword);
	  if (obj_object == ch->right_hand)
	    ch->right_hand = NULL;
	  else if (obj_object == ch->left_hand)
	    ch->left_hand = NULL;
	  equip_char (ch, obj_object, belt_loc);
	}
      break;

    case 16:
      if (!CAN_WEAR (obj_object, ITEM_WEAR_BACK))
	send_to_char ("You cannot wear that across your back.\n", ch);

      else if (get_equip (ch, WEAR_BACK))
	send_to_char ("You are already wearing something there.\n", ch);

      else
	{
	  perform_wear (ch, obj_object, keyword);
	  if (obj_object == ch->right_hand)
	    ch->right_hand = NULL;
	  else if (obj_object == ch->left_hand)
	    ch->left_hand = NULL;
	  equip_char (ch, obj_object, WEAR_BACK);
	}
      break;

    case 17:
      if (!CAN_WEAR (obj_object, ITEM_WEAR_BLINDFOLD))
	send_to_char ("You cannot wear that over your eyes.\n", ch);

      else if (get_equip (ch, WEAR_BLINDFOLD))
	send_to_char ("Something already covers your eyes.\n", ch);

      else
	{
	  perform_wear (ch, obj_object, keyword);
	  if (obj_object == ch->right_hand)
	    ch->right_hand = NULL;
	  else if (obj_object == ch->left_hand)
	    ch->left_hand = NULL;
	  equip_char (ch, obj_object, WEAR_BLINDFOLD);
	}

      break;

    case 18:
      if (!CAN_WEAR (obj_object, ITEM_WEAR_THROAT))
	send_to_char ("You cannot wear that around your throat.\n", ch);

      else if (get_equip (ch, WEAR_THROAT))
	act ("You are already wearing $p around your throat.",
	     false, ch, get_equip (ch, WEAR_THROAT), 0, TO_CHAR);

      else
	{
	  perform_wear (ch, obj_object, keyword);
	  if (obj_object == ch->right_hand)
	    ch->right_hand = NULL;
	  else if (obj_object == ch->left_hand)
	    ch->left_hand = NULL;
	  equip_char (ch, obj_object, WEAR_THROAT);
	}

      break;

    case 19:
      if (!CAN_WEAR (obj_object, ITEM_WEAR_EAR))
	send_to_char ("You cannot wear that on your ears.\n", ch);

      else if (get_equip (ch, WEAR_EAR))
	act ("You are already wearing $p on your ears.",
	     false, ch, get_equip (ch, WEAR_EAR), 0, TO_CHAR);

      else
	{
	  perform_wear (ch, obj_object, keyword);
	  if (obj_object == ch->right_hand)
	    ch->right_hand = NULL;
	  else if (obj_object == ch->left_hand)
	    ch->left_hand = NULL;
	  equip_char (ch, obj_object, WEAR_EAR);
	}

      break;

    case 20:
      {
	if (CAN_WEAR (obj_object, ITEM_WEAR_SHOULDER))
	  {
	    if (get_equip (ch, WEAR_SHOULDER_L) &&
		get_equip (ch, WEAR_SHOULDER_R))
	      {
		send_to_char
		  ("You already wear something on both shoulders.\n", ch);
	      }
	    else
	      {
		perform_wear (ch, obj_object, keyword);
		if (obj_object == ch->right_hand)
		  ch->right_hand = NULL;
		else if (obj_object == ch->left_hand)
		  ch->left_hand = NULL;
		if (get_equip (ch, WEAR_SHOULDER_L))
		  {
		    sprintf (buffer,
			     "You sling the %s over your right shoulder.\n",
			     fname (obj_object->name));
		    send_to_char (buffer, ch);
		    equip_char (ch, obj_object, WEAR_SHOULDER_R);
		  }
		else
		  {
		    sprintf (buffer,
			     "You sling the %s over your left shoulder.\n",
			     fname (obj_object->name));
		    send_to_char (buffer, ch);
		    equip_char (ch, obj_object, WEAR_SHOULDER_L);
		  }
	      }
	  }
	else
	  {
	    send_to_char ("You can't wear that on your shoulder.\n", ch);
	  }
      }
      break;

    case 21:
      {
	if (CAN_WEAR (obj_object, ITEM_WEAR_ANKLE))
	  {
	    if (get_equip (ch, WEAR_ANKLE_L) && get_equip (ch, WEAR_ANKLE_R))
	      {
		send_to_char
		  ("You already wear something around both your ankles.\n",
		   ch);
	      }
	    else
	      {
		perform_wear (ch, obj_object, keyword);
		if (obj_object == ch->right_hand)
		  ch->right_hand = NULL;
		else if (obj_object == ch->left_hand)
		  ch->left_hand = NULL;
		if (get_equip (ch, WEAR_ANKLE_L))
		  {
		    sprintf (buffer,
			     "You wear the %s around your right ankle.\n",
			     fname (obj_object->name));
		    send_to_char (buffer, ch);
		    equip_char (ch, obj_object, WEAR_ANKLE_R);
		  }
		else
		  {
		    sprintf (buffer,
			     "You wear the %s around your left ankle.\n",
			     fname (obj_object->name));
		    send_to_char (buffer, ch);
		    equip_char (ch, obj_object, WEAR_ANKLE_L);
		  }
	      }
	  }
	else
	  {
	    send_to_char ("You can't wear that around your ankle.\n", ch);
	  }
      }
      break;

    case 22:
      if (!CAN_WEAR (obj_object, ITEM_WEAR_HAIR))
	send_to_char ("You cannot wear that in your hair.\n", ch);

      else if (get_equip (ch, WEAR_HAIR))
	act ("You are already wearing $p in your hair.",
	     false, ch, get_equip (ch, WEAR_HAIR), 0, TO_CHAR);

      else
	{
	  perform_wear (ch, obj_object, keyword);
	  if (obj_object == ch->right_hand)
	    ch->right_hand = NULL;
	  else if (obj_object == ch->left_hand)
	    ch->left_hand = NULL;
	  equip_char (ch, obj_object, WEAR_HAIR);
	}

      break;

    case 23:
      {
	if (!CAN_WEAR (obj_object, ITEM_WEAR_FACE))
	  send_to_char ("You cannot wear that on your face.\n", ch);

	else if (get_equip (ch, WEAR_FACE))
	  act ("You are already wearing $p on your face.",
	       false, ch, get_equip (ch, WEAR_FACE), 0, TO_CHAR);

	else
	  {
	    perform_wear (ch, obj_object, keyword);
	    if (obj_object == ch->right_hand)
	      ch->right_hand = NULL;
	    else if (obj_object == ch->left_hand)
	      ch->left_hand = NULL;
	    equip_char (ch, obj_object, WEAR_FACE);
	  }

	break;
      }

      /* ARMBANDS, PATCHES, ARMLETS, ETC */
    case 24:
    case 25:
    case 26:
      {
	if (!CAN_WEAR (obj_object, ITEM_WEAR_ARMBAND))
	  {
	    send_to_char ("You can't wear that around your upper arm.\n", ch);
	    return;;
	  }

	if ((keyword == 24 && get_equip (ch, WEAR_ARMBAND_R)
	     && get_equip (ch, WEAR_ARMBAND_L))
	    || (keyword == 25 && get_equip (ch, WEAR_ARMBAND_R))
	    || (keyword == 26 && get_equip (ch, WEAR_ARMBAND_L)))
	  {
	    send_to_char
	      ("You already wearing something around your upper arm.\n", ch);
	    return;
	  }

	perform_wear (ch, obj_object, keyword);

	if (obj_object == ch->right_hand)
	  {
	    ch->right_hand = NULL;
	  }
	else if (obj_object == ch->left_hand)
	  {
	    ch->left_hand = NULL;
	  }

	if (keyword == 25
	    || (keyword == 24 && !get_equip (ch, WEAR_ARMBAND_R)))
	  {
	    sprintf (buffer, "You wear the %s around your upper right arm.\n",
		     fname (obj_object->name));
	    send_to_char (buffer, ch);
	    equip_char (ch, obj_object, WEAR_ARMBAND_R);
	  }
	else
	  {
	    sprintf (buffer, "You wear the %s around your upper left arm.\n",
		     fname (obj_object->name));
	    send_to_char (buffer, ch);
	    equip_char (ch, obj_object, WEAR_ARMBAND_L);
	  }
	break;
      }
    case -1:
      {
	sprintf (buffer, "Wear %s where?.\n", fname (obj_object->name));
	send_to_char (buffer, ch);
      }
      break;
    case -2:
      {
	sprintf (buffer, "You can't wear the %s.\n",
		 fname (obj_object->name));
	send_to_char (buffer, ch);
      }
      break;
    default:
      {
	sprintf (buffer, "Unknown type called in wear, obj VNUM %d.",
		 obj_object->nVirtual);
	system_log (buffer, true);
      }
      break;
    }
}

void
do_wear (CHAR_DATA * ch, char *argument, int cmd)
{
  char arg1[MAX_STRING_LENGTH];
  char arg2[MAX_STRING_LENGTH];
  char buf[256];
  char buffer[MAX_STRING_LENGTH];
  OBJ_DATA *obj_object;
  int keyword;
  /* These match a switch/case statement in wear() above */
  static char *keywords[] = {
    "finger",			/* 1 */
    "neck",
    "body",
    "head",
    "legs",			/* 5 */
    "feet",
    "hands",
    "arms",
    "about",
    "waist",			/* 10 */
    "wrist",
    "HOLDER",			/* Someone miss something here?  - Rassilon */
    "HOLDER",
    "shield",
    "belt",			/* 15 */
    "back",
    "blindfold",		/* 17 */
    "throat",			/* 18 */
    "ears",			/* 19 */
    "shoulder",			/* 20 */
    "ankle",
    "hair",
    "face",			/* 23 */
    "armband",			/* 24 */
    "armbandright",		/* 25 */
    "armbandleft",		/* 26 */
    "\n"
  };

  *arg1 = '\0';
  *arg2 = '\0';

  argument = one_argument (argument, arg1);
  argument = one_argument (argument, arg2);

  if (*arg1)
    {
      if (!strn_cmp (arg1, ".c", 2))
	{
	  obj_object = get_obj_in_list_id (atoi (arg2), ch->right_hand);
	  if (!obj_object)
	    obj_object = get_obj_in_list_id (atoi (arg2), ch->left_hand);
	}
      else
	{
	  obj_object = get_obj_in_dark (ch, arg1, ch->right_hand);
	  if (!obj_object)
	    obj_object = get_obj_in_dark (ch, arg1, ch->left_hand);
	}
      if (obj_object)
	{
	  if (*arg2 && !isdigit (*arg2))
	    {
	      keyword = search_block (arg2, keywords, false);	/* Partial Match */
	      if (keyword == -1)
		{
		  sprintf (buf, "%s is an unknown body location.\n", arg2);
		  send_to_char (buf, ch);
		}
	      else
		{
		  wear (ch, obj_object, keyword + 1);
		}
	    }
	  else
	    {
	      keyword = -2;

	      if (CAN_WEAR (obj_object, ITEM_WEAR_SHIELD))
		keyword = 14;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_FINGER))
		keyword = 1;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_NECK))
		keyword = 2;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_WRIST))
		keyword = 11;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_WAIST))
		keyword = 10;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_ARMS))
		keyword = 8;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_HANDS))
		keyword = 7;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_FEET))
		keyword = 6;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_LEGS))
		keyword = 5;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_ABOUT))
		keyword = 9;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_HEAD))
		keyword = 4;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_BODY))
		keyword = 3;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_BELT))
		keyword = 15;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_BACK))
		keyword = 16;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_BLINDFOLD))
		keyword = 17;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_THROAT))
		keyword = 18;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_EAR))
		keyword = 19;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_SHOULDER))
		keyword = 20;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_ANKLE))
		keyword = 21;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_HAIR))
		keyword = 22;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_FACE))
		keyword = 23;
	      if (CAN_WEAR (obj_object, ITEM_WEAR_ARMBAND))
		keyword = 24;

	      if (obj_object->activation &&
		  IS_SET (obj_object->obj_flags.extra_flags,
			  ITEM_WEAR_AFFECT))
		obj_activate (ch, obj_object);

	      wear (ch, obj_object, keyword);
	    }
	}
      else
	{
	  sprintf (buffer, "You do not seem to have the '%s'.\n", arg1);
	  send_to_char (buffer, ch);
	}
    }
  else
    {
      send_to_char ("Wear what?\n", ch);
    }
}

void
do_wield (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *obj_object;
  int keyword = 12;
  SECOND_AFFECT *sa;
  char buffer[MAX_STRING_LENGTH];
  char arg1[MAX_STRING_LENGTH];
  char arg2[MAX_STRING_LENGTH];

  argument = one_argument (argument, arg1);
  argument = one_argument (argument, arg2);

  if (!*arg1)
    {
      send_to_char ("Wield what?\n", ch);
      return;
    }

  if (!str_cmp (arg1, ".c"))
    {
      obj_object = get_obj_in_list_id (atoi (arg2), ch->right_hand);
      if (!obj_object)
	obj_object = get_obj_in_list_id (atoi (arg2), ch->left_hand);
    }
  else
    {
      obj_object = get_obj_in_dark (ch, arg1, ch->right_hand);
      if (!obj_object)
	obj_object = get_obj_in_dark (ch, arg1, ch->left_hand);
    }

  if (obj_object)
    {
      if (obj_object->location == WEAR_PRIM
	  || obj_object->location == WEAR_SEC
	  || obj_object->location == WEAR_BOTH)
	{
	  send_to_char ("You're already wielding that!\n", ch);
	  return;
	}

      if ((sa = get_second_affect (ch, SA_WEAR_OBJ, obj_object)))
	return;

      if (obj_object->activation &&
	  IS_SET (obj_object->obj_flags.extra_flags, ITEM_WIELD_AFFECT))
	obj_activate (ch, obj_object);

      // If you're in a brawl room, fighting, didn't currently have a weapon,
      // and your opponent doesn't have a weapon, you crim-flag yourself.

      if(IS_SET(ch->room->room_flags, BRAWL) && IS_SET(ch->room->room_flags, LAWFUL)
         && ch->fighting && !has_weapon(ch) && !has_weapon(ch->fighting) 
         && (*argument == '!' || *arg2 == '!'))
      {
        criminalize (ch, ch->fighting, vtor (ch->fighting->in_room)->zone, CRIME_ASSAULT); 
      }
      else if(IS_SET(ch->room->room_flags, BRAWL) && IS_SET(ch->room->room_flags, LAWFUL)
         && ch->fighting && !has_weapon(ch) && !has_weapon(ch->fighting))
      {
        sprintf (buffer, "You are in a lawful, but brawable area area; if you wield that weapon, you'll be wanted for assault. Type \'#6wield %s !#0\', without the quotes, to continue.", arg1);
        act (buffer, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
        return;
      }

      wear (ch, obj_object, keyword);

    }

  else
    {
      sprintf (buffer, "You do not seem to have the '%s'.\n", arg1);
      send_to_char (buffer, ch);
    }
}

void
do_remove (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *arrow = NULL;
  char arg1[MAX_STRING_LENGTH];
  char arg2[MAX_STRING_LENGTH];
  char arg3[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char buffer[MAX_STRING_LENGTH];
  char location[MAX_STRING_LENGTH];
  CHAR_DATA *tch = NULL;
  OBJ_DATA *obj = NULL;
  OBJ_DATA *eq;
  LODGED_OBJECT_INFO *lodged;
  int removed = 0, target_found = 0;
  int modif = 0;
  int target_obj = 0, target_char = 0;

  if (IS_MORTAL (ch) && IS_SET (ch->room->room_flags, OOC)
        && str_cmp (ch->room->name, PREGAME_ROOM_NAME))
    {
      send_to_char ("This command has been disabled in OOC zones.\n", ch);
      return;
    }

  argument = one_argument (argument, arg1);

  if (!*arg1)
    {
      send_to_char ("Remove what?\n", ch);
      return;
    }

  if (IS_SET (ch->act, ACT_MOUNT))
    {
      send_to_char ("Mounts can't use this command.\n", ch);
      return;
    }

  for (obj = ch->equip; obj; obj = obj->next_content)
    if (IS_OBJ_VIS (ch, obj) && isname (arg1, obj->name))
      break;

  if (!obj)
    {
      if (ch->right_hand)
	{
	  if (isname (arg1, ch->right_hand->name))
	    obj = ch->right_hand;
	}
      if (ch->left_hand)
	{
	  if (isname (arg1, ch->left_hand->name))
	    obj = ch->left_hand;
	}
    }

  if ((get_equip (ch, WEAR_BOTH) && obj != get_equip (ch, WEAR_BOTH))
      || ((ch->right_hand && obj != ch->right_hand)
	  && (ch->left_hand && obj != ch->left_hand)))
    {
      send_to_char ("Your hands are otherwise occupied, at the moment.\n",
		    ch);
      return;
    }


  if (!obj && *arg1)
    {
      if ((tch = get_char_room_vis (ch, arg1)))
	{
	  target_found++;
	  target_char++;
	}
				
      //for (obj = ch->room->contents; obj; obj = obj->next_content)
	//{
	 // if (IS_OBJ_VIS (ch, obj) && isname (arg1, obj->name))
	   // {
	     // target_found++;
	     // target_obj++;
	     // break;
	   // }
	//}
	
	if ((obj = get_obj_in_list_vis (ch, arg1, ch->room->contents)))
	  {
		target_found++;
		target_obj++;
	  }			
      if (!tch && !obj)
	{
	  send_to_char ("Remove what?\n", ch);
	  return;
	}

/**
remove target object
**/
      if (!target_found)
	{
	  tch = ch;
	  sprintf (arg2, "%s", arg1);
	  target_found++;
	}
      else
	{
					argument = one_argument (argument, arg2);
				}
				
      if (!*arg2)
	{
	  send_to_char ("Remove what?\n", ch);
	  return;
	}
				
      if (target_char)
	{
	  if (GET_POS (tch) > POSITION_RESTING && IS_MORTAL (ch) && !IS_SET(tch->act, ACT_PASSIVE))
	    {
	      send_to_char
		("The target must be resting before you can remove a lodged item.\n",
		 ch);
	      return;
	    }
/***
remove target object area
***/
       argument = one_argument (argument, arg3);
					
       for (lodged = tch->lodged; lodged; lodged = lodged->next)
       {
         if (isname (arg2, vtoo (lodged->vnum)->name))
   	 {
//we have a specified location, and it doesn't match this wound 
//so we break out and go to the next wound. If there is no 
//specified location, then we continue with the regular code.
	   if ((*arg3) && strcmp(arg3, lodged->location))
             continue;
	   else
	   {
	     sprintf (location, "%s", lodged->location);
             if(lodged->colored == 1)
	       obj = load_colored_object (lodged->vnum, lodged->var_color, lodged->var_color2, lodged->var_color3);
             else
	       obj = load_object (lodged->vnum);
	     obj->count = 1;
 	     int error = 0;

	     if (can_obj_to_inv (obj, ch, &error, 1))
	       obj_to_char (obj, ch);
	     else
	       obj_to_room (obj, ch->in_room);

	     lodge_from_char (tch, lodged);
	     removed++;
	     break;
           }
	 }
       }
						
       if (removed && tch == ch)
       {
         *buf = '\0';
         *buf2 = '\0';
         sprintf (buf, "You carefully work #2%s#0 loose, wincing in pain as removing it opens the wound on your %s anew.",
         obj->short_description, expand_wound_loc (location));

         sprintf (buf2, "%s#0 carefully works #2%s#0 loose, wincing in pain as removing it opens the wound on %s %s anew.",
         char_short (ch), obj->short_description, HSHR (ch), expand_wound_loc (location));
	 *buf2 = toupper (*buf2);
	 sprintf (buffer, "#5%s", buf2);
	 sprintf (buf2, "%s", buffer);
	 act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
	 act (buf2, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
							
	if ((strcmp(location, "skull")) && (strcmp(location, "reye")) 
            && (strcmp(location, "leye")) && (strcmp(location, "abdomen")) 
            && (strcmp(location, "groin")) && (strcmp(location, "muzzle")))
  	  modif = 0; //not in those loctions, so normal chance
        else
	  modif = 20; //in a bad spot, so a penalty to healing check
	
        if (skill_use (ch, SKILL_HEALING, modif))
	  return; // no extra damage if you make your heal skill check
	else
	{
	  wound_to_char (tch, location, dice (obj->o.od.value[0], obj->o.od.value[1]), 
          0, number (1, 4), 0, 0);
	  return;
	}
      }
      else if (removed && tch != ch)
      {
	      sprintf (buf,
		       "You carefully work #2%s#0 loose.",
		       obj->short_description);
	      sprintf (buf2,
		       "%s#0 carefully works #2%s#0 loose.",
		       char_short (ch), obj->short_description);
	      *buf2 = toupper (*buf2);
	      sprintf (buffer, "#5%s", buf2);
	      sprintf (buf2, "%s", buffer);
	      act (buf, false, ch, 0, tch, TO_CHAR | _ACT_FORMAT);
	      act (buf2, false, ch, 0, tch, TO_NOTVICT | _ACT_FORMAT);
	      sprintf (buf,
		       "%s#0 carefully works #2%s#0 loose, wincing as removing it opens the wound on your %s anew.",
		       char_short (ch), obj->short_description,
		       expand_wound_loc (location));
	      *buf = toupper (*buf);
	      sprintf (buffer, "#5%s", buf);
	      sprintf (buf, "%s", buffer);
	      act (buf, false, ch, 0, tch, TO_VICT | _ACT_FORMAT);
							
   	      if ((strcmp(location, "skull")) &&
		  (strcmp(location, "reye")) &&
		  (strcmp(location, "leye")) &&
		  (strcmp(location, "abdomen")) &&
		  (strcmp(location, "groin")) &&
		  (strcmp(location, "muzzle")))
		modif = 0; //not in those loctions, so normal chance
	     else
		modif = 20; //in a bad spot, so a penalty to healing check
									
	      if (skill_use (ch, SKILL_HEALING, modif))
		return; // no extra damage if you make your heal skill check
	      else
	      {
		wound_to_char (tch, location, dice (obj->o.od.value[0], obj->o.od.value[1]),
                0, number (1, 4), 0, 0);
		return;
	      }
	    }
	    else if (!removed)
	    {
	      send_to_char("You don't see that -- how could you remove it?\n", ch);
              return;
	    }
	  } // if (target_char)
				
      else if (target_obj)
	{

	  for (lodged = obj->lodged; lodged; lodged = lodged->next)
	    {

	      if (isname (arg2, vtoo (lodged->vnum)->name))
		{
		  sprintf (location, "%s", lodged->location);

                  if(lodged->colored == 1)
	            arrow = load_colored_object (lodged->vnum, lodged->var_color, lodged->var_color2, lodged->var_color3);
                  else
	            arrow = load_object (lodged->vnum);
		  arrow->count = 1;
		  obj_to_char (arrow, ch);
		  lodge_from_obj (obj, lodged);
		  removed++;
		  break;
		}
	    }
						
	  if (removed)
	    {
	      sprintf (buf, "You retrieve #2%s#0 from #2%s#0's %s.",
		       arrow->short_description, obj->short_description,
		       expand_wound_loc (location));
	      sprintf (buf2, "%s#0 retrieves #2%s#0 from #2%s#0's %s.",
		       char_short (ch), arrow->short_description,
		       obj->short_description, expand_wound_loc (location));
	      *buf2 = toupper (*buf2);
	      sprintf (buffer, "#5%s", buf2);
	      sprintf (buf2, "%s", buffer);
	      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
	      act (buf2, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
	      return;
	    }
	  else if (!removed)
	    {
	      send_to_char
		("You don't see that -- how could you remove it?\n", ch);
	      return;
	    }
				} //if (target_obj)
    } //if (!obj && *arg1)

  if (!obj)
    {
      send_to_char ("Remove what?\n", ch);
      return;
    }

  if (obj->location == -1)
    {
      send_to_char ("You don't need to remove that!\n", ch);
      return;
    }

  if (SWIM_ONLY (ch->room))
    {
      act ("You begin attempting to remove $p, hindered by the water. . .",
	   false, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
      ch->delay_type = DEL_WATER_REMOVE;
      ch->delay_who = (char *) obj;
      ch->delay = 45 - number (1, 15);
      return;
    }

  if (GET_ITEM_TYPE(obj) == ITEM_RESOURCE)
  {
    act ("You can't remove $p on your own - someone will need to unpack it from you.", true, ch, obj, 0, TO_CHAR);
    return;
  }

  if (obj->location == WEAR_WAIST)
    {
      if ((eq = get_equip (ch, WEAR_BELT_1)))
	{
	  act ("$p falls to the floor.", true, ch, eq, 0, TO_CHAR);
	  act ("$n drops $p.", true, ch, eq, 0, TO_ROOM);
	  obj_to_room (unequip_char (ch, WEAR_BELT_1), ch->in_room);
	}

      if ((eq = get_equip (ch, WEAR_BELT_2)))
	{
	  act ("$p falls to the floor.", true, ch, eq, 0, TO_CHAR);
	  act ("$n drops $p.", true, ch, eq, 0, TO_ROOM);
	  obj_to_room (unequip_char (ch, WEAR_BELT_2), ch->in_room);
	}
    }

  if (obj->loaded/*&& obj->o.weapon.use_skill != SKILL_CROSSBOW*/)
    do_unload (ch, "", 0);

  if (IS_SET (obj->obj_flags.extra_flags, ITEM_MASK) &&
      IS_SET (ch->affected_by, AFF_HOODED))
    do_hood (ch, "", 0);

  if (obj->location == WEAR_LIGHT && GET_ITEM_TYPE (obj) == ITEM_LIGHT)
    light (ch, obj, false, true);

  if (obj->location == WEAR_PRIM || obj->location == WEAR_SEC ||
      obj->location == WEAR_BOTH || obj->location == WEAR_SHIELD)
    unequip_char (ch, obj->location);
  else
    obj_to_char (unequip_char (ch, obj->location), ch);

  act ("You stop using $p.", false, ch, obj, 0, TO_CHAR);
  act ("$n stops using $p.", true, ch, obj, 0, TO_ROOM | _ACT_BLEED);

}

int
can_handle (OBJ_DATA * obj, CHAR_DATA * ch)
{
  int wear_count = 0;

  if (get_equip (ch, WEAR_BOTH))
    wear_count = wear_count + 2;
  if (get_equip (ch, WEAR_SHIELD))
    wear_count++;
  if (get_equip (ch, WEAR_LIGHT))
    wear_count++;
  if (get_equip (ch, WEAR_PRIM))
    wear_count++;
  if (get_equip (ch, WEAR_SEC))
    wear_count++;

  if (wear_count > 2)
    {
      return 0;
    }

  if (wear_count == 2)
    return 0;

  if (obj->o.od.value[0] == 0 && get_equip (ch, WEAR_PRIM))
    return 0;

  if ((obj->obj_flags.wear_flags & ITEM_WIELD) && obj->o.od.value[0] == 3)
    {				/* Two-handed */
      if (wear_count == 0)
	return 1;
      else
	return 0;
    }

  return 1;
}



#define IS_SHEATHABLE(obj) ( (obj != NULL)  && ( ( GET_ITEM_TYPE(obj) == ITEM_MISSILE ) || ( GET_ITEM_TYPE(obj) == ITEM_WEAPON && obj->o.weapon.use_skill != SKILL_SHORTBOW && obj->o.weapon.use_skill != SKILL_LONGBOW) ) )

void
do_sheathe (CHAR_DATA * ch, char *argument, int cmd)
{
  char arg1[MAX_STRING_LENGTH] = "";
  char arg2[MAX_STRING_LENGTH] = "";
  char buf[MAX_STRING_LENGTH] = "";
  OBJ_DATA *obj = NULL;
  OBJ_DATA *obj_prim;
  OBJ_DATA *obj_sec;
  OBJ_DATA *obj_both;
  OBJ_DATA *sheath = NULL;
  char *msg;
  int i = 0, sheathed = 0;
  std::string first_person, third_person;

  if (*argument != '(') 
  {
     argument = one_argument (argument, arg1);
     if (*argument != '(')
	argument = one_argument (argument, arg2);
  }

  obj_prim = get_equip (ch, WEAR_PRIM);
  obj_sec = get_equip (ch, WEAR_SEC);
  obj_both = get_equip (ch, WEAR_BOTH);

  if (!*arg1)
    {
      if (obj_prim && IS_OBJ_VIS (ch, obj_prim))
	{
	  obj = obj_prim;
	}
      else if (obj_sec && IS_OBJ_VIS (ch, obj_sec))
	{
	  obj = obj_sec;
	}
      else if (obj_both && IS_OBJ_VIS (ch, obj_both))
	{
	  obj = obj_both;
	}
      else if (IS_SHEATHABLE (ch->right_hand))
	{
	  obj = ch->right_hand;
	}
      else if (IS_SHEATHABLE (ch->left_hand))
	{
	  obj = ch->left_hand;
	}
    }
  else if (obj_prim &&
	   IS_OBJ_VIS (ch, obj_prim) && isname (arg1, obj_prim->name))
    {
      obj = obj_prim;
    }
  else if (obj_sec &&
	   IS_OBJ_VIS (ch, obj_sec) && isname (arg1, obj_sec->name))
    {
      obj = obj_sec;
    }
  else if (obj_both &&
	   IS_OBJ_VIS (ch, obj_both) && isname (arg1, obj_both->name))
    {
      obj = obj_both;
    }
  else if (IS_SHEATHABLE (ch->right_hand)
	   && IS_OBJ_VIS (ch, ch->right_hand)
	   && isname (arg1, ch->right_hand->name))
    {
      obj = ch->right_hand;
    }
  else if (IS_SHEATHABLE (ch->left_hand)
	   && IS_OBJ_VIS (ch, ch->left_hand)
	   && isname (arg1, ch->left_hand->name))
    {
      obj = ch->left_hand;
    }

  if (!obj)
    {
      if (!*arg1)
	send_to_char ("You aren't wielding anything.\n", ch);
      else
	send_to_char ("You aren't wielding that.\n", ch);
      return;
    }

  if (!IS_SHEATHABLE (obj))
    {
      send_to_char ("You can only sheath a melee weapon or a missile.\n", ch);
      return;
    }

  if (IS_NPC (ch) && IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_BELT))
    {
      one_argument (obj->name, buf);
      do_wear (ch, buf, 0);
      return;
    }

  if (*arg2)
    {
      if (!(sheath = get_obj_in_list_vis (ch, arg2, ch->equip)))
	{
	  send_to_char ("What did you want to sheathe it in?\n", ch);
	  return;
	}
      if (can_obj_to_container (obj, sheath, &msg, 1))
	sheathed++;
    }
  else if (!*arg2)
    {
      for (i = 0; i < MAX_WEAR; i++)
	{
	  if (!(sheath = get_equip (ch, i)))
	    continue;

	  if (GET_ITEM_TYPE (sheath) != ITEM_SHEATH
	      && GET_ITEM_TYPE (sheath) != ITEM_QUIVER)
	    {
	      continue;
	    }
	  else if (GET_ITEM_TYPE (obj) == ITEM_WEAPON
		   && GET_ITEM_TYPE (sheath) != ITEM_SHEATH)
	    {
	      continue;
	    }
	  else if (GET_ITEM_TYPE (obj) == ITEM_MISSILE
		   && GET_ITEM_TYPE (sheath) != ITEM_QUIVER)
	    {
	      continue;
	    }
	  if (!can_obj_to_container (obj, sheath, &msg, 1))
	    continue;
	  sheathed++;
	  break;
	}
    }

  if (!sheathed)
    {
      send_to_char
	("You aren't wearing anything capable of bearing that item.\n", ch);
      return;
    }

	sprintf (buf, "%s", char_short(ch));
	*buf = toupper(*buf);
	first_person.assign("You sheath #2");
	first_person.append(obj_short_desc(obj));
	first_person.append("#0 in #2");
	first_person.append(obj_short_desc(sheath));
	first_person.append("#0");
	third_person.assign("#5");
    third_person.append(buf);
    third_person.append("#0 sheathes #2");
    third_person.append(obj_short_desc(obj));
    third_person.append("#0 in #2");
    third_person.append(obj_short_desc(sheath));
    third_person.append("#0");

	if (evaluate_emote_string (ch, &first_person, third_person, argument) )
	{
		if (ch->right_hand == obj)
		    ch->right_hand = NULL;
		else if (ch->left_hand == obj)
		    ch->left_hand = NULL;
		obj_to_obj (obj, sheath);
	}

	return;



}

void
do_draw (CHAR_DATA * ch, char *argument, int cmd)
{
  char arg1[MAX_STRING_LENGTH] = "";
  int obj_destination = 0, i;
  OBJ_DATA *obj = NULL;
  OBJ_DATA *sheath;
  std::string first_person, third_person;
  bool crim = false;
  char buf[MAX_STRING_LENGTH];

  if (*argument != '(')
	argument = one_argument (argument, arg1);

  for (i = 0; i < MAX_WEAR; i++)
    {
      if (!(sheath = get_equip (ch, i)))
	continue;
      if (GET_ITEM_TYPE (sheath) != ITEM_SHEATH)
	continue;
      if (*arg1)
	{
	  if (!(obj = get_obj_in_list_vis (ch, arg1, sheath->contains)))
	    continue;
	  break;
	}
      else if ((!*arg1) && sheath->contains)
	{
	  obj = sheath->contains;
	  break;
	}
    }

  if (!obj && get_equip (ch, WEAR_BELT_1) &&
      GET_ITEM_TYPE (get_equip (ch, WEAR_BELT_1)) == ITEM_WEAPON)
    obj = get_equip (ch, WEAR_BELT_1);
  else if (!obj && get_equip (ch, WEAR_BELT_2) &&
	   GET_ITEM_TYPE (get_equip (ch, WEAR_BELT_2)) == ITEM_WEAPON)
    obj = get_equip (ch, WEAR_BELT_2);

  if (!obj)
    {
      if (!*arg1)
	send_to_char ("You have nothing to draw!\n", ch);
      else
	send_to_char ("You don't have that in a sheath.\n", ch);
      return;
    }

  if (!IS_SET (obj->obj_flags.wear_flags, ITEM_WIELD))
    {
      act ("You can't wield $o.", false, ch, obj, 0, TO_CHAR);
      return;
    }

  switch (obj->o.od.value[0])
    {
    case 1:	// Medium weapons.
      if (ch->str < 18)
	{
	  if (get_equip (ch, WEAR_PRIM))
	    {
	      send_to_char ("You are already wielding a primary weapon.\n",
			    ch);
	      return;
	    }
	  else if (get_equip (ch, WEAR_BOTH))
	    {
	      send_to_char ("You are already wielding a two-handed weapon.\n",
			    ch);
	      return;
	    }
	  else
	    {
	      obj_destination = WEAR_PRIM;
	    }
	  break;
	}			// > 17 str wields ME in either hand.
    case 2:	// Light weapons.

      if (get_equip (ch, WEAR_PRIM) && get_equip (ch, WEAR_SEC))
	{
	  send_to_char
	    ("You are already wielding both a primary and a secondary weapon.\n",
	     ch);
	  return;
	}
      if (get_equip (ch, WEAR_BOTH))
	{
	  send_to_char ("You are already wielding a two-handed weapon.\n",
			ch);
	  return;
	}
      if (!get_equip (ch, WEAR_PRIM))
	obj_destination = WEAR_PRIM;
      else
	obj_destination = WEAR_SEC;
      break;

    case 4:
      if (get_equip (ch, WEAR_BOTH) ||
	  get_equip (ch, WEAR_PRIM) ||
	  get_equip (ch, WEAR_SEC) || (ch->right_hand || ch->left_hand))
	{
	  send_to_char ("You need both hands to wield this weapon.\n", ch);
	  return;
	}
      obj_destination = WEAR_BOTH;
      break;
    case 3: // HEAVY WEAPONS
      if ((get_equip (ch, WEAR_PRIM) || get_equip (ch, WEAR_BOTH))
	  && (ch->race == 12 || ch->race == 13))
	{			// Trolls can
	  if (get_equip (ch, WEAR_PRIM) && get_equip (ch, WEAR_SEC))
	    {
	      send_to_char
		("You are already wielding both a primary and a secondary weapon.\n",
		 ch);
	      return;
	    }
	  if (!get_equip (ch, WEAR_PRIM) && !get_equip (ch, WEAR_SEC)
	      && !get_equip (ch, WEAR_BOTH))
	    obj_destination = WEAR_BOTH;
	  else if (!get_equip (ch, WEAR_PRIM))
	    obj_destination = WEAR_PRIM;
	  else
	    obj_destination = WEAR_SEC;
	  break;
	}
      if (ch->str >= 20)
	{			// Extremely strong chars can wield two-handed weapons with one hand.
	  if (get_equip (ch, WEAR_PRIM))
	    {
	      send_to_char ("You are already wielding a primary weapon.\n",
			    ch);
	      return;
	    }
	  else if (get_equip (ch, WEAR_BOTH))
	    {
	      send_to_char ("You are already wielding a two-handed weapon.\n",
			    ch);
	      return;
	    }
	  else
	    {
	      if (get_equip (ch, WEAR_SEC)
		  || (ch->right_hand || ch->left_hand))
		obj_destination = WEAR_PRIM;
	      else
		obj_destination = WEAR_BOTH;
	    }
	  break;
	}
      if (get_equip (ch, WEAR_BOTH) ||
	  get_equip (ch, WEAR_PRIM) ||
	  get_equip (ch, WEAR_SEC) || (ch->right_hand || ch->left_hand))
	{
	  send_to_char ("You need both hands to wield this weapon.\n", ch);
	  return;
	}
      obj_destination = WEAR_BOTH;
      break;
    }

  if (obj_destination == WEAR_BOTH && (ch->right_hand || ch->left_hand))
    {
      send_to_char ("You'll need both hands free to draw this weapon.\n", ch);
      return;
    }
  else if ((obj_destination == WEAR_PRIM || obj_destination == WEAR_SEC)
	   && (ch->right_hand && ch->left_hand))
    {
      send_to_char ("You'll need a free hand to draw that weapon.\n", ch);
      return;
    }

    if(IS_SET(ch->room->room_flags, BRAWL) && IS_SET(ch->room->room_flags, LAWFUL) 
       && ch->fighting && !has_weapon(ch) 
       && !has_weapon(ch->fighting) && (*argument == '!' || *arg1 == '!'))
    {
      crim = true;
    }
    else if(IS_SET(ch->room->room_flags, BRAWL) && IS_SET(ch->room->room_flags, LAWFUL) 
       && ch->fighting && !has_weapon(ch) && !has_weapon(ch->fighting))
    {
      sprintf (buf, "You are in a lawful, but brawable area area; if you wield that weapon, you'll be wanted for assault. Type \'#6draw %s !#0\', without the quotes, to continue.", arg1);
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

    
  unequip_char (ch, obj->location);

  if (obj->in_obj)
    obj_from_obj (&obj, 0);

  equip_char (ch, obj, obj_destination);

  if (obj_destination == WEAR_BOTH)
    ch->right_hand = obj;
  else if (obj_destination == WEAR_PRIM)
    {
      if (ch->right_hand)
	ch->left_hand = obj;
      else
	ch->right_hand = obj;
    }
  else if (obj_destination == WEAR_SEC)
    {
      if (ch->right_hand)
	ch->left_hand = obj;
      else
	ch->right_hand = obj;
    }

   sprintf (arg1, "%s", char_short(ch));
   arg1[0] = toupper(arg1[0]);

   first_person.assign("You draw #2");
   first_person.append(obj_short_desc(obj));
   third_person.assign("#5");
   third_person.append(arg1);
   third_person.append("#0 draws #2");
   third_person.append(obj_short_desc(obj));
   if (sheath)
   {
	first_person.append("#0 from #2");
	first_person.append(obj_short_desc(sheath));
   	third_person.append("#0 from #2");
	third_person.append(obj_short_desc(sheath));
   }
   first_person.append("#0");
   third_person.append("#0"); 

  if(!(evaluate_emote_string (ch, &first_person, third_person, argument)))
  {
      if (ch->right_hand == obj)
	  ch->right_hand = NULL;
      else if (ch->left_hand == obj)
	ch->left_hand = NULL;

      obj_to_obj(obj, sheath);
  }
  else
  {
    if(crim)
      criminalize (ch, ch->fighting, vtor (ch->fighting->in_room)->zone, CRIME_ASSAULT);
  }
}

void
do_butcher (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  OBJ_DATA *obj;
  char arg[MAX_INPUT_LENGTH];
  CHAR_DATA *tch;

  if (!((ch->right_hand && GET_ITEM_TYPE (ch->right_hand) 
      == ITEM_WEAPON && (ch->right_hand->o.weapon.use_skill == SKILL_SMALL_BLADE)) 
      || (ch->left_hand && GET_ITEM_TYPE (ch->left_hand) 
      == ITEM_WEAPON && (ch->left_hand->o.weapon.use_skill == SKILL_SMALL_BLADE))))
    {

      send_to_char
	("You need to be holding some sort of knife to properly butcher.\n",
	 ch);
      return;
    }

  argument = one_argument (argument, arg);

  /* Allow the user to enter a carcass name in case he wants to
     say 2.carcass instead of just carcass */

  if (!*arg)
    strcpy (arg, "carcass");

  obj = get_obj_in_list_vis (ch, arg, ch->room->contents);

  if (!obj)
    {
      send_to_char ("You don't see a carcass here.\n", ch);
      return;
    }

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (tch->delay_info1 == (long int) obj)
	{
	  send_to_char
	    ("Someone's already butchering that carcass at the moment.\n", ch);
	  return;
	}
    }

  if (GET_ITEM_TYPE(obj) != ITEM_CARCASS)
    {
      send_to_char ("You can only butcher carcasses.\n", ch);
      return;
    }

  if (!obj->o.od.value[0])
    {
      send_to_char
	("After a moment of poking, you realize that it isn't worth butchering.\n",
	 ch);
      return;
    }

  ch->delay_info1 = (long int) obj;

  skill_learn(ch, SKILL_BUTCHERY);

  if (!real_skill (ch, SKILL_BUTCHERY))
    send_to_char ("You begin hacking away at the carcass.\n", ch);
  else if (ch->skills[SKILL_BUTCHERY] < 30)
    send_to_char ("You begin flailing at the carcass.\n", ch);
  else if (ch->skills[SKILL_BUTCHERY] < 45)
    send_to_char ("You begin slashing into the carcass.\n", ch);
  else if (ch->skills[SKILL_BUTCHERY] < 60)
    send_to_char ("You begin carving the carcass.\n", ch);
  else if (ch->skills[SKILL_BUTCHERY] < 75)
    send_to_char ("You begin operating on the carcass.\n", ch);
  else
    send_to_char ("You begin delicately stripping meat from the carcass.\n", ch);

  sprintf (buf, "$n begins to butcher #2%s#0.", obj->short_description);

  act (buf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);

  watched_action(ch, "butcher a carcass", 0, 1);

  ch->delay_type = DEL_BUTC_1;
  ch->delay = 12;

  // Add time to the object timer so it doesn't decay while we're skinning it.  -Methuselah
  if ((obj->obj_timer - time(0)) < 1000)
    obj->obj_timer = time(0) + 1000;

  // Add time to the object->morphTime timer in case it's close to morphing.  -Methuselah
  if ((obj->morphTime - time(0)) < 1000)
    obj->morphTime = time(0) + 1000;
}



void
delayed_butcher1 (CHAR_DATA * ch)
{
  OBJ_DATA *obj;

  // make sure the carcass is still here, if not throw an error and abort.
  obj = (OBJ_DATA *) ch->delay_info1;
  
  // is it really a carcass?
  if (GET_ITEM_TYPE(obj) != ITEM_CARCASS)
      {
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("The carcass you were butchering is no longer here.\n", ch);
	return; 
      }
      
  if (!obj)
  	{
      // The carcass being skinned is gone, abort.
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("The carcass you were butchering is no longer here.\n", ch);
	return;   
 }

  if (CAN_SEE_OBJ (ch, obj))
    {
  send_to_char ("You cut through the bones and gristle of the carcass as you strip the meat from it.\n", ch);
  act ("$n cuts through the bones and gristle of $p.", false, ch, obj, ch,
       TO_ROOM | _ACT_FORMAT);

  ch->delay_type = DEL_BUTC_2;
  ch->delay = 12;
}
  else
    {
      // Can't see the carcass anymore
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("You can't see any carcass to butcher.\n", ch);
	return;
    }
}

void
delayed_butcher2 (CHAR_DATA * ch)
{
  OBJ_DATA *obj;

  // make sure the carcass is still here, if not throw an error and abort.
  obj = (OBJ_DATA *) ch->delay_info1;

  // is it really a carcass?
  if (GET_ITEM_TYPE(obj) != ITEM_CARCASS)
      {
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("The carcass you were butchering is no longer here.\n", ch);
	return; 
      }
      
	if (!obj)
  	{
      // The carcass being skinned is gone, abort.
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("The carcass you were butchering is no longer here.\n", ch);
	return;    
        }
    
  if (CAN_SEE_OBJ (ch, obj))
    {
       send_to_char ("You strip viscera from the carcass, and continue flaying meat.\n", ch);
  act ("$n flays meat and strips viscera from $p.", false,
       ch, obj, 0, TO_ROOM | _ACT_FORMAT);

  ch->delay_type = DEL_BUTC_3;
  ch->delay = 12;
    }
  else
    { 
      // The carcass being skinned is gone, abort.
      ch->delay_info1 = 0;
      ch->delay_info2 = 0; 
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("You can't see any carcass to butcher.\n", ch);
	return;    
    }
}
void
delayed_butcher3 (CHAR_DATA * ch)
{
  OBJ_DATA *obj1;
  OBJ_DATA *obj2;
  OBJ_DATA *obj3;
  OBJ_DATA *carcass;
  OBJ_DATA *remains;
  int i = 1;
  bool failed = false;
  char buf[MAX_STRING_LENGTH];
  

  carcass = (OBJ_DATA *) ch->delay_info1;

	// is it really a CARCASS
  if (GET_ITEM_TYPE(carcass) != ITEM_CARCASS)
      {
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("The carcass you were butchering is no longer here.\n", ch);
	return; 
      }
      
  if (!carcass)
      {
	// The carcass being skinned is gone, abort.
	ch->delay_info1 = 0;
	ch->delay_info2 = 0;
	ch->delay = 0;
	ch->delay_type = 0;
	send_to_char ("The carcass you were butchering is no longer here.\n", ch);
        return;			
      }
    
  if (!CAN_SEE_OBJ (ch, carcass))
    {
      // The carcass being skinned is gone, abort.
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("You can't see the carcass you were butchering.\n", ch);
      return;
    }

  if (!carcass || sizeof (*carcass) < sizeof (OBJ_DATA))
    {
      send_to_char
	("There has been an error with the carcass. Aborting to avoid crash.\n", ch);
      return;
    }

  // First two ranks, easy to butcher.
 
  if (skill_level (ch, SKILL_BUTCHERY, -40))
  {
    if (carcass->o.od.value[0])
    {
      if(!(obj1 = load_object(carcass->o.od.value[0])))
      {
        send_to_char ("Problem...please contact an immortal.\n", ch);
        return;
      }
      else
      {

        // Give them a piece of meat for free, otherwise,
        // they roll their skill for each piece of meat.

        obj_to_room (obj1, ch->in_room);

        for(i = 1; i < carcass->o.od.value[1]; i++)
        {
          if(skill_use(ch, SKILL_BUTCHERY, -40))
            obj_to_room (load_object(carcass->o.od.value[0]), ch->in_room);
        }

        sprintf(buf, "You succeed in cutting #2%s#0 from the carcass.\n", obj_short_desc (obj1));
        send_to_char(buf, ch);
      }
    }
    else
      failed = true;
  }
  else
  {
    failed = true;
    send_to_char ("You demolished the carcass, failing to recover any meat from the bones.\n", ch);
    act ("$n demolishes a carcass.", false, ch, 0, 0, TO_ROOM);
  }

  if(!failed && skill_use(ch, SKILL_BUTCHERY, -20))
  {
    if (carcass->o.od.value[2])
    {
      if(!(obj2 = load_object(carcass->o.od.value[2])))
      {
        send_to_char ("Problem...please contact an immortal.\n", ch);
        return;
      }
      else
      {

        // Give them a piece of meat for free, otherwise,
        // they roll their skill for each piece of meat.

        obj_to_room (obj2, ch->in_room);

        for(i = 1; i < carcass->o.od.value[3]; i++)
        {
          if(skill_use(ch, SKILL_BUTCHERY, -20))
            obj_to_room (load_object(carcass->o.od.value[2]), ch->in_room);
        }

        sprintf(buf, "You prevail in recovering #2%s#0 from the bones.\n", obj_short_desc (obj2));
        send_to_char(buf, ch);
      }
    }
    else
      failed = true;
  }
  else
  {
    failed = true;
    send_to_char ("You fail to render anything else of use from the carcass.\n", ch);
    act ("$n cuts some meat from a carcass.", false, ch, 0, 0, TO_ROOM);
  }

  if(!failed && skill_use(ch, SKILL_BUTCHERY, 0))
  {
    if (carcass->o.od.value[4])
    {
      if(!(obj3 = load_object(carcass->o.od.value[4])))
      {
        send_to_char ("Problem...please contact an immortal.\n", ch);
        return;
      }
      else
      {

        // Give them a piece of meat for free, otherwise,
        // they roll their skill for each piece of meat.

        obj_to_room (obj3, ch->in_room);

        for(i = 1; i < carcass->o.od.value[5]; i++)
        {
          if(skill_use(ch, SKILL_BUTCHERY, 0))
            obj_to_room (load_object(carcass->o.od.value[4]), ch->in_room);
        }

        sprintf(buf, "With #2%s#0, you have stripped the carcass for all it has.\n", obj_short_desc (obj3));
        send_to_char(buf, ch);

      }
    }
    act ("$n strips $p for all the meat is has.", false, ch, carcass, 0, TO_ROOM);

  }
  else if(!failed)
  {
    send_to_char ("You fail to render anything else of use from the carcass.\n", ch);
    act ("$n cuts a good deal of meat from a carcass.", false, ch, 0, 0, TO_ROOM);
  }

  if(carcass->quality)
  {
    if(!(remains = load_object(carcass->quality)))
      ;
    else
      obj_to_room(remains, ch->in_room);
  }

  
  ch->delay_type = 0;
  ch->delay = 0;
  ch->delay_info1 = 0;
  ch->delay_info2 = 0;

  extract_obj (carcass);
}


void
do_skin (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  OBJ_DATA *obj_corpse;
  char obj_name[MAX_INPUT_LENGTH];
  CHAR_DATA *tch;

  if (!((ch->right_hand && GET_ITEM_TYPE (ch->right_hand) 
      == ITEM_WEAPON && (ch->right_hand->o.weapon.use_skill == SKILL_SMALL_BLADE)) 
      || (ch->left_hand && GET_ITEM_TYPE (ch->left_hand) 
      == ITEM_WEAPON && (ch->left_hand->o.weapon.use_skill == SKILL_SMALL_BLADE))))
    {

      send_to_char
	("You need to be holding some sort of knife to properly skin.\n",
	 ch);
      return;
    }

  argument = one_argument (argument, obj_name);

  /* Allow the user to enter a corpse name in case he wants to
     say 2.corpse instead of just corpse */

  if (!*obj_name)
    strcpy (obj_name, "corpse");

  obj_corpse = get_obj_in_list_vis (ch, obj_name, ch->room->contents);

  if (!obj_corpse)
    {
      send_to_char ("You don't see a corpse here.\n", ch);
      return;
    }

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (tch->delay_info1 == (long int) obj_corpse)
	{
	  send_to_char
	    ("Someone's already skinning that corpse at the moment.\n", ch);
	  return;
	}
    }

  if (obj_corpse->nVirtual != VNUM_CORPSE
      && !strstr (obj_corpse->name, "corpse"))
    {
      send_to_char ("You can only skin corpses.\n", ch);
      return;
    }

  if (!obj_corpse->o.od.value[2])
    {
      send_to_char
	("After a moment of poking, you realize that it isn't worth skinning.\n",
	 ch);
      return;
    }

  ch->delay_info1 = (long int) obj_corpse;

  skill_learn(ch, SKILL_BUTCHERY);

  if (!real_skill (ch, SKILL_BUTCHERY))
    send_to_char ("You begin hacking away at the corpse.\n", ch);
  else if (ch->skills[SKILL_BUTCHERY] < 30)
    send_to_char ("You begin flailing at the corpse.\n", ch);
  else if (ch->skills[SKILL_BUTCHERY] < 45)
    send_to_char ("You begin slashing into the corpse.\n", ch);
  else if (ch->skills[SKILL_BUTCHERY] < 60)
    send_to_char ("You begin carving the corpse.\n", ch);
  else if (ch->skills[SKILL_BUTCHERY] < 75)
    send_to_char ("You begin operating on the corpse.\n", ch);
  else
    send_to_char ("You begin delicately skinning the corpse.\n", ch);

  sprintf (buf, "$n begins skinning #2%s#0.", obj_corpse->short_description);

  act (buf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);

  ch->delay_type = DEL_SKIN_1;
  ch->delay = 3;

  // Add time to the object timer so it doesn't decay while we're skinning it.  -Methuselah
  if ((obj_corpse->obj_timer - time(0)) < 1000)
    obj_corpse->obj_timer = time(0) + 1000;

  // Add time to the object->morphTime timer in case it's close to morphing.  -Methuselah
  if ((obj_corpse->morphTime - time(0)) < 1000)
    obj_corpse->morphTime = time(0) + 1000;
}

void
delayed_skin_new1 (CHAR_DATA * ch)
{
  OBJ_DATA *obj_corpse;

  // make sure the corpse is still here, if not throw an error and abort.
  obj_corpse = (OBJ_DATA *) ch->delay_info1;
  
  // is it really a corpse?
   if (obj_corpse->nVirtual != VNUM_CORPSE
      && !strstr (obj_corpse->name, "corpse"))
      {
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("The corpse you were skinning is no longer here.\n", ch);
	return; 
      }
      
  if (!obj_corpse)
  	{
      // The corpse being skinned is gone, abort.
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("The corpse you were skinning is no longer here.\n", ch);
	return;   
 }

  if (CAN_SEE_OBJ (ch, obj_corpse))
    {
  send_to_char ("You start to cut into the corpse.\n", ch);
  act ("$n starts to cut into the corpse.", false, ch, 0, 0,
       TO_ROOM | _ACT_FORMAT);

  watched_action(ch, "skin a corpse.", 0, 0);

  ch->delay_type = DEL_SKIN_2;
  ch->delay = 7;
}
  else
    {
      // Can't see the corpse anymore
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("You can't see any corpse to skin.\n", ch);
	return;
    }
}

void
delayed_skin_new2 (CHAR_DATA * ch)
{
  OBJ_DATA *obj_corpse;

  // make sure the corpse is still here, if not throw an error and abort.
  obj_corpse = (OBJ_DATA *) ch->delay_info1;

// is it really a corpse?
	if (obj_corpse->nVirtual != VNUM_CORPSE
      && !strstr (obj_corpse->name, "corpse"))
      {
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("The corpse you were skinning is no longer here.\n", ch);
	return; 
      }
      
	if (!obj_corpse)
  	{
      // The corpse being skinned is gone, abort.
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("The corpse you were skinning is no longer here.\n", ch);
	return;    
        }
    
  if (CAN_SEE_OBJ (ch, obj_corpse))
    {
       send_to_char ("You seem to be making progress as you dig into the corpse.\n", ch);
  act ("$n seems to be making progress as $e digs into the corpse.", false,
       ch, 0, 0, TO_ROOM | _ACT_FORMAT);

  ch->delay_type = DEL_SKIN_3;
  ch->delay = 10;
    }
  else
    { 
      // The corpse being skinned is gone, abort.
      ch->delay_info1 = 0;
      ch->delay_info2 = 0; 
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("You can't see any corpse to skin.\n", ch);
	return;    
    }
}

void
delayed_skin_new3 (CHAR_DATA * ch)
{
  OBJ_DATA *skin;
  OBJ_DATA *corpse;
  OBJ_DATA *carcass;
  char buf[MAX_INPUT_LENGTH];
  char *p;

  corpse = (OBJ_DATA *) ch->delay_info1;

	// is it really a corpse?
	if (corpse->nVirtual != VNUM_CORPSE
      && !strstr (corpse->name, "corpse"))
      {
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("The corpse you were skinning is no longer here.\n", ch);
	return; 
      }
      
	if (!corpse)
			{
				// The corpse being skinned is gone, abort.
				ch->delay_info1 = 0;
				ch->delay_info2 = 0;
				ch->delay = 0;
				ch->delay_type = 0;
				send_to_char ("The corpse you were skinning is no longer here.\n", ch);
return;			
}
    
  if (!CAN_SEE_OBJ (ch, corpse))
    {
      // The corpse being skinned is gone, abort.
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay = 0;
      ch->delay_type = 0;
      send_to_char ("You can't see the corpse you were skinning.\n", ch);
      return;
    }

  if (!corpse || sizeof (*corpse) < sizeof (OBJ_DATA))
    {
      send_to_char
	("There has been an error with the corpse. Aborting to avoid crash.\n",
	 ch);
      return;
    }

  // It's not difficult to skin an animal, hence the +50 bonus.
  // Real skill is in the butchery.
 
  if (skill_use (ch, SKILL_BUTCHERY, -50))
    {
//A corpse that is WILL_SKIN has a negative o.od.value[2], See make-corpse() for details . We must adjust to get a vnum we can load?
      if (!(skin = load_object (corpse->o.od.value[2])))
      {
	if (!(skin = load_object (-corpse->o.od.value[2])))
	{
	  send_to_char ("Problem...please contact an immortal.\n", ch);
	  return;
	}
      }

      obj_to_room (skin, ch->in_room);

      strcpy (buf, skin->name);

      for (p = buf; *p && *p != ' ';)
	{
	  p++;
	}

      *p = '\0';

      if (CAN_SEE_OBJ (ch, skin))
	{
	  send_to_char ("You have successfully skinned the corpse.\n", ch);
	  act ("$n has successfully skinned the corpse.", false, ch, 0, 0,
	       TO_ROOM | _ACT_FORMAT);
	  do_get (ch, buf, 0);
	}
      else
	{
	  send_to_char ("You demolished the corpse.\n", ch);
	  act ("$n demolishes a corpse.", false, ch, 0, 0,
	       TO_ROOM | _ACT_FORMAT);
	}
    }
  else
    {
      send_to_char ("You demolished the corpse.\n", ch);
      act ("$n demolishes a corpse.", false, ch, 0, 0, TO_ROOM);
    }

  ch->delay_type = 0;
  ch->delay = 0;
  ch->delay_info1 = 0;
  ch->delay_info2 = 0;

  if (!(carcass = load_object (corpse->o.od.value[3])))
    {
  		if (!(carcass = load_object (-corpse->o.od.value[3])))
				{
					extract_obj (corpse);
					return;
				}
		}
  extract_obj (corpse);

  obj_to_room (carcass, ch->in_room);
}

/*
    Return an int which is the objnum of a plant suitable for the sector type
    with some probability depending on rarity, and what the character was after.
*/

  const int herbArray[HERB_NUMSECTORS * HERB_RARITIES][5] =
/* Arrays for herbs by rsector and rarity by obj number */
/* { #herbs, obj#'s } by rarity, from very rare to very common */

// 7000-7009, fungii
// 7010-7019, flowers
// 7020-7029, berries
// 7030-7039, weeds
// 7040-7049, other

// Each and every one of these has five possible things, for the five types of objects.
// If you've specificed what you've sought, then you might just find it, otherwise,
// it loads it up for you at random.

/* field herb plants */
  { 
  {3, 7010, 7016, 7004, 0}, {3, 7009, 7012, 7017, 0}, {3, 7008, 7015, 7021, 0},
  {3, 7023, 7024, 7025, 0}, {3, 7020, 7022, 7011, 0},

/* pasture herb plants */
  {3, 7010, 7016, 7004, 0}, {3, 7009, 7012, 7017, 0}, {3, 7008, 7015, 7021, 0},
  {3, 7023, 7024, 7025, 0}, {3, 7020, 7022, 7011, 0},

/* wood herb plants */
  {2, 7016, 7017, 0, 0}, {3, 7004, 7007, 7010, 0}, {3, 7003, 7009, 7015, 0},
  {3, 7001, 7006, 7008, 0}, {3, 7000, 7002, 7005, 0},

/* forest herb plants */
  {2, 7013, 7014, 0, 0}, {3, 7004, 7007, 7010, 0}, {3, 7003, 7009, 7015, 0},
  {3, 7001, 7006, 7008, 0}, {3, 7000, 7002, 7005, 0},

/* mountain herb plants */
  {3, 7010, 7016, 7004, 0}, {3, 7009, 7012, 7017, 0}, {3, 7008, 7015, 7021, 0},
  {3, 7023, 7024, 7025, 0}, {3, 7020, 7022, 7011, 0},

/* swamp herb plants */
  {3, 7010, 7016, 7004, 0}, {3, 7009, 7012, 7017, 0}, {3, 7008, 7015, 7021, 0},
  {3, 7023, 7024, 7025, 0}, {3, 7020, 7022, 7011, 0},

/* heath herb plants */
  {3, 7010, 7016, 7004, 0}, {3, 7009, 7012, 7017, 0}, {3, 7008, 7015, 7021, 0},
  {3, 7023, 7024, 7025, 0}, {3, 7020, 7022, 7011, 0},

/* hill herb plants */
  {3, 7010, 7016, 7004, 0}, {3, 7009, 7012, 7017, 0}, {3, 7008, 7015, 7021, 0},
  {3, 7023, 7024, 7025, 0}, {3, 7020, 7022, 7011, 0},

/* waternoswim herb plants */
  {3, 7010, 7016, 7004, 0}, {3, 7009, 7012, 7017, 0}, {3, 7008, 7015, 7021, 0},
  {3, 7020, 7022, 7024, 0}, {3, 7020, 7022, 7011, 0}
  };





int
GetHerbPlant (int sector_type, int pos, int rarity)
{

#define RUMM_VERY_RARE	10
#define RUMM_RARE	25
#define RUMM_UNCOMMON	40
#define RUMM_COMMON	65
#define RUMM_VCOMMON	100

#define RUMM_MAX 100

  int i, sect, objnum, rarityIndex, numHerbs;

  // So, it rolls a number, and decides what you get.

  i = number (1, RUMM_MAX);

  if (i <= RUMM_VERY_RARE)
    rarityIndex = 0;
  else if (i <= RUMM_RARE)
    rarityIndex = 1;
  else if (i <= RUMM_UNCOMMON)
    rarityIndex = 2;
  else if (i <= RUMM_COMMON)
    rarityIndex = 3;
  else				/* very common */
    rarityIndex = 4;

  /*
     I could get clever with the rarities in accessing the herb arrays,
     but I won't in case we decide we need to change the rarites and
     stuff.  The code will just look a tad clunky IMHO.
   */

  switch (sector_type)
    {
    case SECT_FIELD:
      sect = HERB_FIELD;
      break;
    case SECT_PASTURE:
      sect = HERB_PASTURE;
      break;
    case SECT_WOODS:
      sect = HERB_WOODS;
      break;
    case SECT_FOREST:
      sect = HERB_FOREST;
      break;
    case SECT_MOUNTAIN:
      sect = HERB_MOUNTAINS;
      break;
    case SECT_SWAMP:
      sect = HERB_SWAMP;
      break;
    case SECT_HEATH:
      sect = HERB_HEATH;
      break;
    case SECT_HILLS:
      sect = HERB_HILLS;
      break;
    default:
      return (0);

    }

  numHerbs = herbArray[(sect * HERB_RARITIES) + rarityIndex][0];

  if (numHerbs == 1)
    objnum = herbArray[(sect * HERB_RARITIES) + rarityIndex][1];
  else
    objnum =
      herbArray[(sect * HERB_RARITIES) + rarityIndex][number (1, numHerbs)];

  if(pos && rarity)
    objnum = herbArray[(sect * HERB_RARITIES) + (rarity - 1)][pos];

  return (objnum);

}

void
do_rummage (CHAR_DATA * ch, char *argument, int cmd)
{
  int sector_type;
  int sect = 0;
  char arg[MAX_STRING_LENGTH];
  char use[AVG_STRING_LENGTH] = {'\0'};
  char buf[MAX_STRING_LENGTH] = {'\0'};
  OBJ_DATA *obj = NULL;
  int i = 0;
  int j = 1;
  int k = 0;
  int seek = 0;

  skill_learn(ch, SKILL_FORAGE);

  if (!real_skill (ch, SKILL_FORAGE))
    {
      send_to_char ("You don't have any idea how to forage!\n\r", ch);
      return;
    }

  if (is_dark (ch->room) && !get_affect (ch, MAGIC_AFFECT_INFRAVISION)
      && !IS_SET (ch->affected_by, AFF_INFRAVIS))
    {
      send_to_char ("It's too dark to forage.\n\r", ch);
      return;
    }

  if (is_sunlight_restricted (ch))
    return;

  sector_type = vtor (ch->in_room)->sector_type;

  if (sector_type == SECT_CITY || sector_type == SECT_INSIDE)
    {
      send_to_char ("You will not find any healthy plants growing here.\n",
		    ch);
      return;
    }

  if(IS_SET(ch->room->room_flags, NO_LOG))
    {
      send_to_char ("You cannot forage here.\n", ch);
      return;
    }    

  if (sector_type != SECT_FIELD &&
      sector_type != SECT_PASTURE &&
      sector_type != SECT_WOODS &&
      sector_type != SECT_FOREST &&
      sector_type != SECT_MOUNTAIN &&
      sector_type != SECT_SWAMP &&
      sector_type != SECT_HEATH && sector_type != SECT_HILLS)
    {
      send_to_char
	("Any plant that might be growing here has long since been trampled\n\r",
	 ch);
      return;
    }

  argument = one_argument (argument, arg);

  if(*arg)
  {
    if(!str_cmp(arg, "survey"))
    {
      switch (sector_type)
      {
        case SECT_FIELD:
          sect = HERB_FIELD;
          break;
        case SECT_PASTURE:
          sect = HERB_PASTURE;
          break; 
        case SECT_WOODS:
          sect = HERB_WOODS;
          break;
        case SECT_FOREST:
          sect = HERB_FOREST;
          break;
        case SECT_MOUNTAIN:
          sect = HERB_MOUNTAINS;
          break;
        case SECT_SWAMP:
          sect = HERB_SWAMP;
          break;
        case SECT_HEATH:    
          sect = HERB_HEATH;    
          break;
        default:
          sect = HERB_HILLS;
          break;
      }

      sprintf (buf, "You believe you could find the following plants here...\n");

      for (k = HERB_RARITIES - 1; k >= 0; k--)
      {

        sprintf (use, "Very Common");

        if (k == 3)
          if(ch->skills[SKILL_FORAGE] >= 20)
            sprintf (use, "Common");
          else 
            continue;
        else if (k == 2)
          if(ch->skills[SKILL_FORAGE] >= 35)
            sprintf (use, "Uncommon");
          else 
            continue;
        else if (k == 1)
          if(ch->skills[SKILL_FORAGE] >= 50)
            sprintf (use, "Rare");
          else 
            continue;
        else if (k == 0)
          if(ch->skills[SKILL_FORAGE] >= 60)
            sprintf (use, "Very Rare");
          else 
            continue;

        sprintf (buf + strlen(buf), "#2\n%s Plants:#0\n", use);     

        j = 1;

        for (i = 1; i <= herbArray[(sect * HERB_RARITIES) + k][0]; i++)
        {
          if((obj = load_object (herbArray[(sect * HERB_RARITIES) + k][i])))
          {
            if(ch->skills[SKILL_FORAGE] >= obj->o.od.value[4])
            {  
              switch(obj->o.od.value[5])
              {
                case 1:
                  sprintf (use, "#2(edible)#0");
                  break;
                case 2:
                  sprintf (use, "#3(cookable)#0");
                  break;
                case 3:
                  sprintf (use, "#6(herb)#0");  
                  break;
                default:
                  sprintf (use, "#1(unknown)#0");
                  break;            
              }
            }
            else
            {
              sprintf (use, "#1(unknown)#0");
            }  

            sprintf (buf + strlen (buf), "   #2%d#0: %-14s %s\n", j, use, obj->short_description);
            j++;
          }
        }   
      }
      send_to_char(buf, ch);
      return;
    }
    else if(!str_cmp(arg, "vcommon") || !str_cmp(arg, "vc"))
    {
      argument = one_argument(argument, use);
      if (isdigit(*use))
      {
        seek = atoi(use);
        ch->delay_info2 = 5;
      }
    }
    else if((!str_cmp(arg, "common") || !str_cmp(arg, "c")) && ch->skills[SKILL_FORAGE] >= 20)
    {
      argument = one_argument(argument, use);
      if (isdigit(*use))
      {
        seek = atoi(use);
        ch->delay_info2 = 4;
      }
    }
    else if((!str_cmp(arg, "uncommon") || !str_cmp(arg, "uc"))  && ch->skills[SKILL_FORAGE] >= 35)
    {
      argument = one_argument(argument, use);
      if (isdigit(*use))
      {
        seek = atoi(use);
        ch->delay_info2 = 3;
      }
    }
    else if((!str_cmp(arg, "rare") || !str_cmp(arg, "r"))  && ch->skills[SKILL_FORAGE] >= 50)
    {
      argument = one_argument(argument, use);
      if (isdigit(*use))
      {
        seek = atoi(use);
        ch->delay_info2 = 2;
      }
    }
    else if((!str_cmp(arg, "vrare") || !str_cmp(arg, "vr"))  && ch->skills[SKILL_FORAGE] >= 60)
    {
      argument = one_argument(argument, use);
      if (isdigit(*use))
      {
        seek = atoi(use);
        ch->delay_info2 = 1;
      }
    }
  }

  if(get_affect(ch, MAGIC_HIDDEN))
  {
    remove_affect_type (ch, MAGIC_HIDDEN);
    act ("$n reveals $mself.", true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
    act ("Your actions have compromised your concealment.", true, ch, 0, 0, TO_CHAR);
  }

  send_to_char ("You begin searching the area for useful plants.\n\r", ch);
  act ("$n begins rummaging through the flora.", true, ch, 0, 0,
       TO_ROOM);

  watched_action(ch, "rummaging through the flora.", 0, 0);

  ch->delay_type = DEL_RUMMAGE;
  
  if(seek)
    ch->delay = 30;
  else 
    ch->delay = 20;

  ch->delay_info1 = seek;
}

void
delayed_rummage (CHAR_DATA * ch)
{
  OBJ_DATA *obj;
  int objnum;
  AFFECTED_TYPE *herbed;
  char arg[MAX_STRING_LENGTH];

  herbed = is_room_affected (ch->room->affects, HERBED_COUNT);

  if (herbed && (herbed->a.herbed.timesHerbed >= MAX_HERBS_PER_ROOM))
    {
      send_to_char ("This area has been stripped of all useful plants.\n\r",
		    ch);
      return;
    }

  if (skill_use (ch, SKILL_FORAGE, (ch->delay_info1 ? 0 : -20)))
  {
    objnum = GetHerbPlant (ch->room->sector_type, ch->delay_info1, ch->delay_info2);
    obj = load_object (objnum);

    if (obj)
    {
      obj_to_room (obj, ch->in_room);
      act ("Your rummaging has revealed $p.", false, ch, obj, 0, TO_CHAR);
      act ("$n's rummaging unconvers $p.", false, ch, obj, 0, TO_ROOM);

      if (!herbed)
      {
        herbed = (AFFECTED_TYPE *) alloc (sizeof (AFFECTED_TYPE), 13);
        herbed->type = HERBED_COUNT;
        herbed->a.herbed.timesHerbed = 1;
        herbed->a.herbed.duration = HERB_RESET_DURATION;
        herbed->next = ch->room->affects;
        ch->room->affects = herbed;
      }
      else
      {
        herbed->a.herbed.timesHerbed++;
        herbed->a.herbed.duration = HERB_RESET_DURATION;
      }
    }
    else
    {
      send_to_char("You successfully rummaged but there is naught to be found\n\r", ch);
      sprintf(arg, "Herbalism Object %d missing.", objnum);
      send_to_gods(arg);
    }
  }
  else
    {

      if (!herbed)
      {
        herbed = (AFFECTED_TYPE *) alloc (sizeof (AFFECTED_TYPE), 13);
        herbed->type = HERBED_COUNT;
        herbed->a.herbed.timesHerbed = 1;
        herbed->a.herbed.duration = HERB_RESET_DURATION;
        herbed->next = ch->room->affects;
        ch->room->affects = herbed;
      }
      else
      {
        herbed->a.herbed.timesHerbed++;
        herbed->a.herbed.duration = HERB_RESET_DURATION;
      }

      send_to_char ("Your rummaging efforts are of no avail.\n\r", ch);
      act ("$n stops searching the plant life.", true, ch, 0, 0, TO_ROOM);
    }
}

void
do_gather (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  OBJ_DATA *plant;
  ROOM_DATA *room;

  skill_learn(ch, SKILL_FORAGE);

  if (!real_skill (ch, SKILL_FORAGE))
    {
      send_to_char ("You need training in forgaging before you can gather flora.\n", ch);
      return;
    }

  room = ch->room;

  argument = one_argument (argument, buf);

  if (!(plant = get_obj_in_list_vis (ch, buf, room->contents)))
    {
      send_to_char ("You don't see that here.\n", ch);
      return;
    }

  if (plant->obj_flags.type_flag != ITEM_PLANT)
    {
      act ("$p is not flora.  You can only gather from objects produced by foraging.",
	   true, ch, plant, 0, TO_CHAR);
      return;
    }

  if(get_affect(ch, MAGIC_HIDDEN))
  {
    remove_affect_type (ch, MAGIC_HIDDEN);
    act ("$n reveals $mself.", true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
    act ("Your actions have compromised your concealment.", true, ch, 0, 0, TO_CHAR);
  }


  act ("You begin to rummage through $p for anything of use.", true, ch, plant, 0, TO_CHAR);
  act ("$n begins to rummage through $p.", true, ch, plant, 0, TO_ROOM);

  ch->delay_type = DEL_GATHER;
  ch->delay_who = str_dup (buf);
  ch->delay = 20 - (ch->skills[SKILL_FORAGE] / 10);
}

void
delayed_gather (CHAR_DATA * ch)
{
  OBJ_DATA *plant;
  OBJ_DATA *obj1;
  OBJ_DATA *obj2;
  ROOM_DATA *room;
  int i = 0;
  char buf[MAX_STRING_LENGTH];
  bool failed = true;

  room = ch->room;

  strcpy (buf, ch->delay_who);

  mem_free (ch->delay_who);
  ch->delay_who = NULL;

  if (!(plant = get_obj_in_list_vis (ch, buf, room->contents)))
    {
      send_to_char ("You failed to gather the contents of the plant before "
		    "it was destroyed.\n", ch);
      return;
    }

  // Works off plant difficulty.
  if (!plant->o.od.value[0] ||
      !vtoo (plant->o.od.value[0]) || !skill_use (ch, SKILL_FORAGE, (-80 + plant->o.od.value[4])))
    {
      act ("You destroy $p while failing to gather anything from it.",
	   true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
      act ("$n manages to gather nothing from $p, destroying it in the process.", true, ch, plant, 0, TO_ROOM | _ACT_FORMAT);
      failed = true;
    }
  else
  {
      // Give them a piece of meat for free, otherwise,
      // they roll their skill for each piece of meat.
      if(!(obj1 = load_object(plant->o.od.value[0])))
      {
        send_to_char ("Problem...please contact an immortal.\n", ch);
        return;
      }

      obj_to_room (obj1, ch->in_room);

      for(i = 1; i < plant->o.od.value[1]; i++)
      {
        if(skill_use(ch, SKILL_FORAGE, (-60 + plant->o.od.value[4])))
          obj_to_room (load_object(plant->o.od.value[0]), ch->in_room);
      }

      sprintf(buf, "You succeed in gathering #2%s#0 from the flora.\n", obj_short_desc (obj1));
      act ("$n has gathered $p from the flora.", true, ch, obj1, 0, TO_ROOM);
      send_to_char(buf, ch);
      failed = false;
  }

  if (plant->o.od.value[2] && !failed && vtoo (plant->o.od.value[2]) 
   && skill_use (ch, SKILL_FORAGE, (-40 + plant->o.od.value[4])))
  {
    if (plant->o.od.value[2])
    {
      if(!(obj2 = load_object(plant->o.od.value[2])))
      {
        send_to_char ("Problem...please contact an immortal.\n", ch);
        return;
      }
      else
      {

        // Give them a piece of meat for free, otherwise,
        // they roll their skill for each piece of meat.

      obj_to_room (obj2, ch->in_room);

      for(i = 1; i < plant->o.od.value[3]; i++)
      {
        if(skill_use(ch, SKILL_FORAGE, (-20 + plant->o.od.value[4])))
          obj_to_room (load_object(plant->o.od.value[2]), ch->in_room);
      }

      sprintf(buf, "You further managed to extract #2%s#0 from the flora.\n", obj_short_desc (obj2));
      act ("$n also manages to extract $p from the flora.", true, ch, obj2, 0, TO_ROOM);
      send_to_char(buf, ch);
      }
    }
  }

  obj_from_room (&plant, 0);
  extract_obj (plant);
}

void
do_identify (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  OBJ_DATA *plant;
  ROOM_DATA *room;

  skill_learn(ch, SKILL_FORAGE);

  if (!real_skill (ch, SKILL_HERBALISM) && !real_skill (ch, SKILL_FORAGE))
    {
      send_to_char ("You need training in herbalism or foraging before you can identify "
		    "flora.\n", ch);
      return;
    }

  room = ch->room;

  argument = one_argument (argument, buf);

  if (!(plant = get_obj_in_list_vis (ch, buf, room->contents)) &&
      !(plant = get_obj_in_list (buf, ch->right_hand)) &&
      !(plant = get_obj_in_list (buf, ch->left_hand)))
    {
      send_to_char ("You don't see that here.\n", ch);
      return;
    }

  if (!(plant->obj_flags.type_flag == ITEM_PLANT 
      || (plant->obj_flags.type_flag == ITEM_FOOD && GET_MATERIAL_TYPE(plant) == 1 << 9)))
    {
      act ("$p isn't something you can identify: try and #6forage#0 for something first, or identify something gained by #6gather#0ing.",
	   true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  act ("You begin to examine $p.", true, ch, plant, 0, TO_CHAR);
  act ("$n begins examining $p.", true, ch, plant, 0, TO_ROOM);

  ch->delay_type = DEL_IDENTIFY;
  ch->delay_who = str_dup (buf);
  ch->delay = 15 - (ch->skills[SKILL_FORAGE] / 10);
}

void
delayed_identify (CHAR_DATA * ch)
{
  OBJ_DATA *plant;
  ROOM_DATA *room;
  char buf[MAX_STRING_LENGTH];

  room = ch->room;

  strcpy (buf, ch->delay_who);

  mem_free (ch->delay_who);
  ch->delay_who = NULL;

  if (!(plant = get_obj_in_list_vis (ch, buf, room->contents)) &&
      !(plant = get_obj_in_list (buf, ch->right_hand)) &&
      !(plant = get_obj_in_list (buf, ch->left_hand)))
    {
      send_to_char ("You failed to identify the contents of the plant before "
		    "it was destroyed.\n", ch);
      return;
    }

  act ("$n finishes looking at $p.", true, ch, plant, 0, TO_ROOM);

  if (!plant->o.od.value[5] || !plant->o.od.value[4]
      || (ch->skills[SKILL_HERBALISM] < plant->o.od.value[4] / 2 
          && ch->skills[SKILL_FORAGE] < plant->o.od.value[4] / 2))
    {
      act ("You have no clue as to what $p might yield.",
	   true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  if(ch->skills[SKILL_HERBALISM] >= plant->o.od.value[4] 
     || ch->skills[SKILL_FORAGE] >= plant->o.od.value[4])
  {
    if(plant->obj_flags.type_flag == ITEM_PLANT)
    {
      switch(plant->o.od.value[5])
      {
        case 1:
          act ("You determine that $p will yield safe, edible food.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
          break;
        case 2:
          act ("You determine that $p will yield food that whilst not safe to eat raw, may be edible once cooked.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
          break;
        case 3:
          act ("You determine that $p will yield food dangerous to eat, but may have other uses.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
          break;
        case 4:
          act ("You determine that $p will not yield anything edible but may have other uses.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
          break;
        default:
          act ("You're unable to determine what $p will yield.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
          break;
      } 
    }
    else
    {
      switch(plant->o.od.value[3])
      {
        case 1:
          act ("You determine that $p is safe, edible food.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
          break;
        case 2:
          act ("You determine that $p will be safe to eat once properly cooked or prepared.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
          break;
        case 3:
          act ("You determine that $p is dangerous to eat, but can be prepared in other ways.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
          break;
        case 4:
          act ("You determine that $p is not edible but may have other uses.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
          break;
        default:
          act ("You unable to determine what use $p is.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
          break;
      } 
    }
  }
  else if(ch->skills[SKILL_HERBALISM] >= plant->o.od.value[4] / 2 
     || ch->skills[SKILL_FORAGE] >= plant->o.od.value[4] / 2)
  {
    if(plant->obj_flags.type_flag == ITEM_PLANT)
    {
    switch(plant->o.od.value[5])
    {
      case 1:
      case 2:
      case 3:
        act ("You determine that $p will yield edible food.", 
              true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
        break;
      case 4:
        act ("You determine that $p will not yield anything edible but may have other uses.", 
              true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
        break;
      default:
          act ("You're unable to determine what $p will yield.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
        break;
    }
    }
    else
    {
    switch(plant->o.od.value[3])
    {
      case 1:
      case 2:
      case 3:
        act ("You determine that $p is edible food.", 
              true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
        break;
      case 4:
        act ("You determine that $p is not edible but may have other uses.", 
              true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
        break;
      default:
        act ("You determine that $p will not yield anything of use.", 
              true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
        break;
    }
    }
  }
  else
  {
          act ("You unable to determine what use $p is.", 
                true, ch, plant, 0, TO_CHAR | _ACT_FORMAT);
  }
}

void
do_empty (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *obj;
  OBJ_DATA *container;
  char buf[MAX_STRING_LENGTH];
  POISON_DATA *poison;

  argument = one_argument (argument, buf);

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch)
        && str_cmp (ch->room->name, PREGAME_ROOM_NAME))
    {
      send_to_char ("That is not allowed in OOC areas.\n", ch);
      return;
    }

  if (!(container = get_obj_in_dark (ch, buf, ch->right_hand)) &&
      !(container = get_obj_in_dark (ch, buf, ch->left_hand)))
    {
      send_to_char ("You don't have that object.\n", ch);
      return;
    }

  if (GET_ITEM_TYPE (container) == ITEM_CONTAINER)
    {

      if (container->contains)
	{
	  act ("You turn $p upside down, spilling its contents:",
	       false, ch, container, 0, TO_CHAR);
	  act ("$n turns $p upside down, spilling its contents:",
	       true, ch, container, 0, TO_ROOM);
	}

      while (container->contains)
	{
	  obj = container->contains;
	  obj_from_obj (&obj, 0);
	  obj_to_room (obj, ch->in_room);
	  act ("    $p", false, ch, obj, 0, TO_CHAR);
	  act ("    $p", false, ch, obj, 0, TO_ROOM);
	}

      // Sanity check - sometimes objects refuse to be cleared of their weight, and this
      // fixes it.

      container->contained_wt = container->obj_flags.weight;

      return;
    }

  if (GET_ITEM_TYPE (container) == ITEM_POTION)
    {
      act ("You spill the contents of $p on the ground.",
	   false, ch, container, 0, TO_CHAR);
      act ("$n spills the contents of $p on the ground.",
	   false, ch, container, 0, TO_ROOM);
      extract_obj (container);
      return;
    }

  if (GET_ITEM_TYPE (container) == ITEM_DRINKCON)
    {

      if (!container->o.drinkcon.volume)
	{
	  act ("$o is already empty.", false, ch, container, 0, TO_CHAR);
	  return;
	}

      act ("You spill the contents of $p on the ground.",
	   false, ch, container, 0, TO_CHAR);
      act ("$n spills the contents of $p on the ground.",
	   false, ch, container, 0, TO_ROOM);

      container->o.drinkcon.liquid = 0;
      container->o.drinkcon.volume = 0;
      container->o.drinkcon.spell_1 = 0;
      container->o.drinkcon.spell_2 = 0;
      container->o.drinkcon.spell_3 = 0;

      for(poison = container->poison; poison; poison = poison->next)
        remove_object_poison(container, container->poison);

      return;
    }

  if (GET_ITEM_TYPE (container) == ITEM_LIGHT &&
      !is_name_in_list ("candle", container->name))
    {

      if (!container->o.light.hours)
	{
	  act ("$o is already empty.", false, ch, container, 0, TO_CHAR);
	  return;
	}

      if (container->o.light.on)
	light (ch, container, false, true);

      sprintf (buf, "You empty %s from $p on the ground.",
	       vnum_to_liquid_name (container->o.light.liquid));
      act (buf, false, ch, container, 0, TO_CHAR);

      sprintf (buf, "$n empties %s from $p on the ground.",
	       vnum_to_liquid_name (container->o.light.liquid));
      act (buf, false, ch, container, 0, TO_ROOM);

      container->o.light.hours = 0;

      return;
    }

  act ("You can't figure out how to empty $p.",
       false, ch, container, 0, TO_CHAR);
}

void
do_blindfold (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *victim;
  OBJ_DATA *obj;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!(victim = get_char_room_vis (ch, buf)))
    {
      send_to_char ("There's no such person here.\n", ch);
      return;
    }

  if (victim == ch)
    {
      send_to_char ("Wear a blindfold if you want to blindfold yourself.\n",
		    ch);
      return;
    }

  if (get_equip (victim, WEAR_BLINDFOLD))
    {
      act ("$N is already blindfolded.", false, ch, 0, victim, TO_CHAR);
      return;
    }

  if (ch->right_hand
      && IS_SET (ch->right_hand->obj_flags.wear_flags, ITEM_WEAR_BLINDFOLD))
    obj = ch->right_hand;

  else if (ch->left_hand
	   && IS_SET (ch->left_hand->obj_flags.wear_flags,
		      ITEM_WEAR_BLINDFOLD))
    obj = ch->left_hand;

  if (!obj)
    {
      send_to_char ("You don't have a blindfold available.\n", ch);
      return;
    }

  if (!AWAKE (victim) && number (0, 4))
    {
      if (wakeup (victim))
	{
	  act ("You've awoken $N.", false, ch, 0, victim, TO_CHAR);
	  act ("$N wakes you up while trying to blindfold you!",
	       false, ch, 0, victim, TO_VICT);
	  act ("$n wakes $N up while trying to bindfold $M.",
	       false, ch, 0, victim, TO_NOTVICT);
	}
    }

  if (!(!AWAKE (victim) ||
	get_affect (victim, MAGIC_AFFECT_PARALYSIS) || IS_SUBDUEE (victim)))
    {
      act ("$N won't let you blindfold $M.", false, ch, 0, victim, TO_CHAR);
      return;
    }

  if (obj->carried_by)
    obj_from_char (&obj, 0);

  act ("$N blindfolds you!", true, victim, 0, ch, TO_CHAR);
  act ("You place $p over $N's eyes.", false, ch, obj, victim, TO_CHAR);
  act ("$n places $p over $N's eyes.", false, ch, obj, victim, TO_NOTVICT);

  equip_char (victim, obj, WEAR_BLINDFOLD);
}

/*

   Create a head object.

   Modify corpse object to be 'headless'.

*/

void
do_behead (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  OBJ_DATA *corpse;
  OBJ_DATA *tool;
  OBJ_DATA *head;

  argument = one_argument (argument, buf);

  if (!(corpse = get_obj_in_list_vis (ch, buf, ch->room->contents)))
    {
      if (get_obj_in_list_vis (ch, buf, ch->right_hand)
	  || get_obj_in_list_vis (ch, buf, ch->left_hand))
	send_to_char ("Drop the corpse to behead it.\n", ch);
      else
	send_to_char ("You don't see that corpse here.\n", ch);
      return;
    }

  if (corpse->nVirtual != VNUM_CORPSE)
    {
      act ("$o is not a corpse.", false, ch, corpse, 0, TO_CHAR);
      return;
    }

  if (IS_SET (corpse->o.container.flags, CONT_BEHEADED))
    {
      act ("$p looks headless already.", false, ch, corpse, 0, TO_CHAR);
      return;
    }

  if (!(tool = get_equip (ch, WEAR_PRIM)) &&
      !(tool = get_equip (ch, WEAR_SEC)) &&
      !(tool = get_equip (ch, WEAR_BOTH)))
    {
      act ("You need to wield a sword, axe or knife to behead $p.",
	   false, ch, corpse, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  if (tool->o.weapon.hit_type < 0 || tool->o.weapon.hit_type > 5)
    {
      sprintf (buf, "Your weapon, $p, vnum %d, doesn't have a valid hit_type "
	       "set.  This is a bug.  Please petition and report it.  "
	       "Thanks.", tool->nVirtual);
      act (buf, false, ch, tool, 0, TO_CHAR);
      sprintf (buf, "Weapon vnum %d has illegal hit type %d",
	       tool->nVirtual, tool->o.weapon.hit_type);
      system_log (buf, true);
      return;
    }

  if (tool->o.weapon.hit_type == 1 ||	/* 1 = pierce */
      tool->o.weapon.hit_type == 3)
    {				/* 3 = bludgeon */
      sprintf (buf2, "You can't %s the head off with $p.  Try a weapon "
	       "that will slice or chop.",
	       weapon_theme[tool->o.weapon.hit_type]);
      act (buf2, false, ch, tool, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  head = load_object (VNUM_HEAD);

  head->name = str_dup ("head");
  if (!strncmp (corpse->short_description, "the corpse of ", 14))
    strcpy (buf2, corpse->short_description + 14);
  else
    strcpy (buf2, "some unfortunate creature");

  sprintf (buf, "The head of %s rests here.", buf2);
  head->description = str_dup (buf);

  sprintf (buf, "the head of %s", buf2);
  head->short_description = str_dup (buf);

  sprintf (buf, "You %s the head from $p.",
	   weapon_theme[tool->o.weapon.hit_type]);
  act (buf, false, ch, corpse, 0, TO_CHAR | _ACT_FORMAT);

  sprintf (buf, "$n cuts the head from $p.",
	   weapon_theme[tool->o.weapon.hit_type]);
  act (buf, false, ch, corpse, 0, TO_ROOM | _ACT_FORMAT);

  mem_free (corpse->description);
  mem_free (corpse->short_description);

  sprintf (buf, "The headless corpse of %s is lying here.", buf2);
  corpse->description = str_dup (buf);

  sprintf (buf, "the headless corpse of %s", buf2);
  corpse->short_description = str_dup (buf);

  strcpy (buf2, corpse->name);

  mem_free (corpse->name);

  sprintf (buf, "headless %s", buf2);
  corpse->name = str_dup (buf);

  head->obj_flags.weight = corpse->obj_flags.weight / 10;
  corpse->obj_flags.weight -= head->obj_flags.weight;

  corpse->o.container.flags |= CONT_BEHEADED;

  head->obj_timer = corpse->obj_timer;

  if(corpse->o.od.value[4] == 11)
  {
    TOGGLE (head->econ_flags, 1 << 2);
    head->silver = 20;
  }

  obj_to_room (head, ch->in_room);
}

void
light (CHAR_DATA * ch, OBJ_DATA * obj, int on, int on_off_msg)
{
  /* Automatically correct any problems with on/off status */

  if (obj->o.light.hours <= 0)
    obj->o.light.on = 0;

  if (!on && !obj->o.light.on)
    return;

  if (on && obj->o.light.hours <= 0)
    return;

  if (on && obj->o.light.on)
    return;

  obj->o.light.on = on;

  if (on && get_affect (ch, MAGIC_HIDDEN))
    {
      if (would_reveal (ch))
	act ("You reveal yourself.", false, ch, 0, 0, TO_CHAR);
      else
	act ("The light will reveal your hiding place.",
	     false, ch, 0, 0, TO_CHAR);

      remove_affect_type (ch, MAGIC_HIDDEN);
    }

  if (on)
    {
      room_light (ch->room);	/* lighten before messages */
      if (on_off_msg)
	{
	  act ("You light $p.", false, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
	  act ("$n lights $p.", false, ch, obj, 0, TO_ROOM | _ACT_FORMAT);
	}
    }
  else
    {
      if (on_off_msg)
	{
	  act ("You put out $p.", false, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
	  act ("$n puts out $p.", false, ch, obj, 0, TO_ROOM | _ACT_FORMAT);
	}
      room_light (ch->room);	/* darken after messages */
    }
}

void
do_light (CHAR_DATA * ch, char *argument, int cmd)
{
  int on = 1;
  int room_only = 0;
  OBJ_DATA *obj;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!str_cmp (buf, "room"))
    {
      room_only = 1;
      argument = one_argument (argument, buf);
    }

  if (!*buf)
    {
      send_to_char ("Light what?\n", ch);
      return;
    }

  if (room_only)
    {
      obj = get_obj_in_dark (ch, buf, ch->room->contents);
    }
  else
    {
      obj = get_obj_in_dark (ch, buf, ch->right_hand);
      if (!obj)
	obj = get_obj_in_dark (ch, buf, ch->left_hand);
      if (!obj && (obj = get_obj_in_dark (ch, buf, ch->equip))
	  && !IS_SET (obj->obj_flags.extra_flags, ITEM_MAGIC))
	{
	  act ("You can't light $p while you're still wearing it.\n", false,
	       ch, obj, 0, TO_CHAR);
	  return;
	}
    }

  if (!obj)
    {
      send_to_char ("You don't see that light source.\n", ch);
      return;
    }

  if (obj->obj_flags.type_flag != ITEM_LIGHT)
    {
      act ("You cannot light $p.", false, ch, obj, 0, TO_CHAR);
      return;
    }

  argument = one_argument (argument, buf);

  if (!str_cmp (buf, "off"))
    on = 0;

  if (on && SWIM_ONLY (ch->room)
      && !IS_SET (obj->obj_flags.extra_flags, ITEM_MAGIC))
    {
      act ("You can't light $p while you're swimming.\n", false, ch, obj, 0,
	   TO_CHAR);
      return;
    }

  if (!on && !obj->o.light.on)
    {
      act ("$p isn't lit.", false, ch, obj, 0, TO_CHAR);
      return;
    }

  if (on && obj->o.light.hours <= 0)
    {
      act ("$p will no longer light.", false, ch, obj, 0, TO_CHAR);
      return;
    }

  if (on && obj->o.light.on)
    {
      act ("$p is already lit.", false, ch, obj, 0, TO_CHAR);
      return;
    }

  light (ch, obj, on, true);
}

void
do_smell (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Smelling rooms doesn't work yet.\n", ch);
      return;
    }

  if (!(tch = get_char_room_vis (ch, buf)))
    {
      send_to_char ("You don't see that person.\n", ch);
      return;
    }

  if (!get_stink_message (tch, NULL, buf, ch))
    {
      act ("$N does not have any peculiar smells.",
	   false, ch, 0, tch, TO_CHAR);
      return;
    }

  act (buf, false, ch, 0, tch, TO_CHAR);
  act ("$N smells you.", false, ch, 0, tch, TO_VICT);
  act ("$n smells $N.", false, ch, 0, tch, TO_NOTVICT);
}

// Program allows for the packing of resource-type objects, to get around
// the fact that they're big and heavy and not the sort of thing anything
// other than a mule or a troll can conceivably move.

void
do_pack (CHAR_DATA * ch, char *argument, int cmd)
{

  char arg[MAX_STRING_LENGTH] = "";
  OBJ_DATA *obj;
  CHAR_DATA *victim;
  int count;

  argument = one_argument (argument, arg);

  if (just_a_number (arg))
    {
      count = atoi (arg);
      argument = one_argument (argument, arg);
    }

  if (!*arg)
    {
      send_to_char ("Pack what?\n", ch);
      return;
    }

  if (!(obj = get_obj_in_list_vis (ch, arg, ch->room->contents)))
  {
    send_to_char ("You don't see that here.\n", ch);
    return;
  }

  if (!CAN_WEAR (obj, ITEM_TAKE)
      || IS_SET (obj->obj_flags.extra_flags, ITEM_PITCHED))
    {
      act ("$p cannot be lifted at all.", true, ch, obj, 0, TO_CHAR);
      return;
    }

  if(GET_ITEM_TYPE(obj) != ITEM_RESOURCE)
  {
    act("There is no need to pack $p. Use the 'put' command instead.", true, ch, obj, 0, TO_CHAR);
    return;
  }

  argument = one_argument (argument, arg);

  if (!str_cmp (arg, "on") || !str_cmp (arg, "onto"))
    argument = one_argument (argument, arg);


  if (!(victim = get_char_room_vis (ch, arg)))
  {
    act ("Pack $p on to whom?", true, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
    return;
  }

  if (!IS_SET (victim->act, ACT_PACKANIMAL))
  {
    act ("$N isn't capable of carrying things such as $p.", true, ch, obj, victim, TO_CHAR | _ACT_FORMAT);
    return;
  }

  put_on_char (ch, victim, obj, 0, argument);
  return;
}
