/*------------------------------------------------------------------------\
|  handler.c : Handler Module                         www.middle-earth.us | 
|  Copyright (C) 2004, Shadows of Isildur: Traithe                        |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/

#include <unistd.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <string.h>
#include <unistd.h>

#include "server.h"
#include "structs.h"
#include "net_link.h"
#include "protos.h"
#include "utils.h"
#include "decl.h"
#include "group.h"
#include "utility.h"


extern int knockout;
extern rpie::server engine;

COMBAT_MSG_DATA *cm_list = NULL;

const char *targets[] = {
  "Self",
  "\n"
};

	/* use_table_data is filled in at boot time from registry */

struct use_table_data use_table[] = {
  {8},				/* BRAWLING */
  {10},
  {10},
  {10},
  {16},
  {16},
  {16},
  {22},
  {22},
  {22},
  {25},
  {25},
  {0},
  {0},
  {0},
  {0}
};

int
is_hooded (CHAR_DATA * ch)
{
  OBJ_DATA *obj;

  if ((obj = get_equip (ch, WEAR_NECK_1))
      && IS_SET (obj->obj_flags.extra_flags, ITEM_MASK)
      && IS_SET (ch->affected_by, AFF_HOODED))
    return 1;

  if ((obj = get_equip (ch, WEAR_NECK_2))
      && IS_SET (obj->obj_flags.extra_flags, ITEM_MASK)
      && IS_SET (ch->affected_by, AFF_HOODED))
    return 1;

  if ((obj = get_equip (ch, WEAR_ABOUT))
      && IS_SET (obj->obj_flags.extra_flags, ITEM_MASK)
      && IS_SET (ch->affected_by, AFF_HOODED))
    return 1;

  if (((obj = get_equip (ch, WEAR_HEAD))
       && IS_SET (obj->obj_flags.extra_flags, ITEM_MASK))
      || ((obj = get_equip (ch, WEAR_FACE))
	  && IS_SET (obj->obj_flags.extra_flags, ITEM_MASK)))
    {

      if (obj->obj_flags.type_flag == ITEM_WORN
	  || obj->obj_flags.type_flag == ITEM_ARMOR)
	return 1;
    }

  return 0;
}

char *
char_names (CHAR_DATA * ch)
{
  OBJ_DATA *obj;

  if ((obj = get_equip (ch, WEAR_NECK_1)) &&
      IS_SET (obj->obj_flags.extra_flags, ITEM_MASK) &&
      IS_SET (ch->affected_by, AFF_HOODED))
    return obj->desc_keys;

  if ((obj = get_equip (ch, WEAR_NECK_2)) &&
      IS_SET (obj->obj_flags.extra_flags, ITEM_MASK) &&
      IS_SET (ch->affected_by, AFF_HOODED))
    return obj->desc_keys;

  if ((obj = get_equip (ch, WEAR_ABOUT)) &&
      IS_SET (obj->obj_flags.extra_flags, ITEM_MASK) &&
      IS_SET (ch->affected_by, AFF_HOODED))
    return obj->desc_keys;

  if (((obj = get_equip (ch, WEAR_HEAD))
       && IS_SET (obj->obj_flags.extra_flags, ITEM_MASK))
      || ((obj = get_equip (ch, WEAR_FACE))
	  && IS_SET (obj->obj_flags.extra_flags, ITEM_MASK)))
    {

      if (obj->obj_flags.type_flag == ITEM_WORN)
	{
	  if (!obj->desc_keys)
	    return "obscured";

	  return obj->desc_keys;
	}

      if (obj->obj_flags.type_flag == ITEM_ARMOR)
	{
	  if (!obj->desc_keys)
	    return "obscured";

	  return obj->desc_keys;
	}
    }

  return ch->name;
}


char *
fname (char *namelist)
{
  static char holder[30];
  char *point;

  if (!namelist)
    return "";

  for (point = holder; isalpha (*namelist); namelist++, point++)
    *point = *namelist;

  *point = '\0';

  return (holder);
}

char *
fname_hyphen (char *namelist)
{
  static char holder[30];
  char *point;

  if (!namelist)
    return "obscured";

  for (point = holder;
       isalpha (*namelist) || *namelist == '-' || *namelist == ' ';
       namelist++, point++)
    *point = *namelist;

  *point = '\0';

  return holder;
}

int
isname (const char *str, char *namelist)
{
  char *curname = '\0';
  const char *curstr = '\0';

  if (!str)
    return 0;

  if (!namelist)
    return 0;

  curname = namelist;
  for (;;)
    {
      for (curstr = str;; curstr++, curname++)
	{
	  if ((!*curstr && !isalpha (*curname))
	      || is_abbrev (curstr, curname))
	    return (1);

	  if (!*curname)
	    return (0);

	  if (!*curstr || *curname == ' ')
	    break;

	  if (tolower (*curstr) != tolower (*curname))
	    break;
	}

      /* skip to next name */

      for (; isalpha (*curname); curname++);
      if (!*curname)
	return (0);
      curname++;		/* first char of new name */
    }
}

// Case-sensitive version of isname().

int
isnamec (char *str, char *namelist)
{
  char *curname = '\0';
  char *curstr = '\0';

  if (!str)
    return 0;

  if (!namelist)
    return 0;

  curname = namelist;
  for (;;)
    {
      for (curstr = str;; curstr++, curname++)
	{
	  if ((!*curstr && !isalpha (*curname))
	      || is_abbrevc (curstr, curname))
	    return (1);

	  if (!*curname)
	    return (0);

	  if (!*curstr || *curname == ' ')
	    break;

	  if (*curstr != *curname)
	    break;
	}

      /* skip to next name */

      for (; isalpha (*curname); curname++);
      if (!*curname)
	return (0);
      curname++;		/* first char of new name */
    }
}

int
real_skill (CHAR_DATA * ch, int skill)
{
  if (!IS_NPC (ch)) // || IS_SET (ch->act, ACT_STAYPUT))
    return ch->skills[skill];
  else
    return vtom (ch->mob->nVirtual)->skills[skill];
}

void
affect_modify (CHAR_DATA * ch, int type, int loc, int mod, int bitv,
	       int add, int sn)
{
  if (type >= JOB_1 && type <= JOB_3)
    return;

  if (type >= CRAFT_FIRST && type <= CRAFT_LAST)
    return;

  if (type >= MAGIC_SMELL_FIRST && type <= MAGIC_SMELL_LAST)
    return;

  if (type >= MAGIC_FIRST_SOMA && type <= MAGIC_LAST_SOMA)
    return;

  if (type == MAGIC_GUARD)
    return;

  if (type == MAGIC_WATCH)
    return;

  if (type == AFFECT_SHADOW)
    return;

  if (type == MUTE_EAVESDROP)
    return;

  if (add && bitv)
    ch->affected_by |= bitv;

  else if (bitv)
    {
      ch->affected_by &= ~bitv;
      mod = -mod;
    }

/*
	switch (type) {
		case SPELL_STRENGTH:		GET_STR (ch) += mod;				return;
		case SPELL_DEXTERITY:		GET_DEX (ch) += mod;				return;
		case SPELL_INTELLIGENCE:	GET_INT (ch) += mod;				return;
		case SPELL_AURA:			GET_AUR (ch) += mod;				return;
		case SPELL_WILL:			GET_WIL (ch) += mod;				return;
		case SPELL_CONSTITUTION:	GET_CON (ch) += mod;				return;
		case SPELL_AGILITY:			GET_AGI (ch) += mod;				return;
		default:														break;
	}
*/

  if (loc >= (SKILL_BRAWLING + 10000) && loc <= (LAST_SKILL + 10000))
    {
      if (add)
	ch->skills[loc - 10000] += mod;
      else
	{
	  if (ch->pc && ch->pc->skills[loc - 10000] < ch->skills[loc - 10000])
	    ch->skills[loc - 10000] -= mod;
	  else if (!ch->pc)
	    ch->skills[loc - 10000] -= mod;
	}
      return;
    }

  else
    switch (loc)
      {

      case APPLY_NONE:
      case APPLY_CASH:
      case APPLY_SAVING_BREATH:
      case APPLY_SAVING_SPELL:
      case APPLY_AC:
	break;

      case APPLY_STR:
	GET_STR (ch) += mod;
	break;
      case APPLY_DEX:
	GET_DEX (ch) += mod;
	break;
      case APPLY_INT:
	GET_INT (ch) += mod;
	break;
      case APPLY_AUR:
	GET_AUR (ch) += mod;
	break;
      case APPLY_WIL:
	GET_WIL (ch) += mod;
	ch->max_move = calc_lookup (ch, REG_MISC, MISC_MAX_MOVE);
	break;
      case APPLY_CON:
	GET_CON (ch) += mod;
	if (!IS_NPC (ch))
	  ch->max_hit = 10 + 6 * GET_CON (ch);
	else
	  {
	    ch->max_hit += mod * 6;
	    if (GET_HIT (ch) > ch->max_hit)
	      GET_HIT (ch) = ch->max_hit;
	  }
	ch->max_move = calc_lookup (ch, REG_MISC, MISC_MAX_MOVE);
	break;
      case APPLY_AGI:
	GET_AGI (ch) += mod;
	break;
      case APPLY_AGE:
	ch->time.birth += mod;
	break;
      case APPLY_HIT:
	ch->max_hit += mod;
	break;
      case APPLY_MOVE:
	ch->max_move += mod;
	break;
      case APPLY_DAMROLL:
	if (IS_NPC (ch))
	  GET_DAMROLL (ch) += mod;
	break;
      default:
	break;

      }				/* switch */
}

void
nullify_affects (CHAR_DATA * ch)
{
  AFFECTED_TYPE *af;
  OBJ_DATA *eq;
  int i;

  /* Remove affects caused by equipment */

  for (i = 0; i < MAX_WEAR; i++)
    {

      if (!(eq = get_equip (ch, i)) ||
	  i == WEAR_BELT_1 || i == WEAR_BELT_2 || i == WEAR_BACK)
	continue;

      for (af = eq->xaffected; af; af = af->next)
	{
	  if ((af->a.spell.location != APPLY_OFFENSE &&
	       af->a.spell.location != APPLY_DEFENSE))
	    affect_modify (ch, af->type, af->a.spell.location,
			   af->a.spell.modifier,
			   eq->obj_flags.bitvector, false, 0);
	}
    }


  /* Remove affects caused by spells */

  for (af = ch->hour_affects; af; af = af->next)
    affect_modify (ch, af->type, af->a.spell.location, af->a.spell.modifier,
		   af->a.spell.bitvector, false, af->a.spell.sn);


  /* Restore base capabilities */

  ch->tmp_str = ch->str;
  ch->tmp_dex = ch->dex;
  ch->tmp_intel = ch->intel;
  ch->tmp_aur = ch->aur;
  ch->tmp_con = ch->con;
  ch->tmp_wil = ch->wil;
}

void
reapply_affects (CHAR_DATA * ch)
{
  AFFECTED_TYPE *af;
  OBJ_DATA *eq;
  int i;

  /* Add equipment affects back */

  for (i = 0; i < MAX_WEAR; i++)
    {

      if (!(eq = get_equip (ch, i)) ||
	  i == WEAR_BELT_1 || i == WEAR_BELT_2 || i == WEAR_BACK)
	continue;

      for (af = eq->xaffected; af; af = af->next)
	if (af->a.spell.location != APPLY_OFFENSE &&
	    af->a.spell.location != APPLY_DEFENSE)
	  affect_modify (ch, af->type, af->a.spell.location,
			 af->a.spell.modifier,
			 eq->obj_flags.bitvector, true, 0);
    }

  /* Add spell affects back */

  for (af = ch->hour_affects; af; af = af->next)
    affect_modify (ch, af->type, af->a.spell.location, af->a.spell.modifier,
		   af->a.spell.bitvector, true, af->a.spell.sn);

  /* Make certain values are between 0..25, not < 0 and not > 25! */

  GET_DEX (ch) = MAX (0, MIN (GET_DEX (ch), 25));
  GET_INT (ch) = MAX (0, MIN (GET_INT (ch), 25));
  GET_WIL (ch) = MAX (0, MIN (GET_WIL (ch), 25));
  GET_AUR (ch) = MAX (0, MIN (GET_AUR (ch), 25));
  GET_CON (ch) = MAX (0, MIN (GET_CON (ch), 25));
  GET_STR (ch) = MAX (0, MIN (GET_STR (ch), 25));
}

AFFECTED_TYPE *
get_obj_affect_location (OBJ_DATA * obj, int location)
{
  AFFECTED_TYPE *af;

  if (!obj->xaffected)
    return NULL;

  for (af = obj->xaffected; af; af = af->next)
    if (af->a.spell.location == location)
      return af;

  return NULL;
}

void
remove_obj_affect_location (OBJ_DATA * obj, int location)
{
  AFFECTED_TYPE *af;
  AFFECTED_TYPE *taf;

  if (!obj->xaffected)
    return;

  if (obj->xaffected->a.spell.location == location)
    {
      af = obj->xaffected;
      obj->xaffected = obj->xaffected->next;
      mem_free (af);
      return;
    }

  for (af = obj->xaffected; af->next; af = af->next)
    if (af->next->a.spell.location == location)
      {
	taf = af->next;
	af->next = taf->next;
	mem_free (taf);
	return;
      }
}

void
affect_to_obj (OBJ_DATA * obj, AFFECTED_TYPE * af)
{
  AFFECTED_TYPE *taf;

  if (!obj->xaffected)
    {
      obj->xaffected = af;
      return;
    }

  for (taf = obj->xaffected; taf->next; taf = taf->next)
    ;

  taf->next = af;
}

void
poison_to_obj (OBJ_DATA * obj, POISON_DATA * poison)
{
  POISON_DATA *taf;

  if (!obj->poison)
    {
      obj->poison = poison;
      return;
    }

  for (taf = obj->poison; taf->next; taf = taf->next)
    ;

  taf->next = poison;
}

void remove_object_poison (OBJ_DATA * obj, POISON_DATA *af)
{
  POISON_DATA *taf;

  if (af == obj->poison)
    {
      obj->poison = af->next;
      mem_free (af);
      return;
    }

  for (taf = obj->poison; taf->next; taf = taf->next)
    {
      if (af == taf->next)
	{
	  taf->next = af->next;
	  mem_free (af);
	  return;
	}
    }
}

void
remove_obj_affect (OBJ_DATA * obj, int type)
{
  AFFECTED_TYPE *af;
  AFFECTED_TYPE *free_af;

  if (!obj->xaffected)
    return;

  if (obj->xaffected->type == type)
    {
      af = obj->xaffected;
      obj->xaffected = af->next;
      mem_free (af);
      return;
    }

  for (af = obj->xaffected; af->next; af = af->next)
    {
      if (af->next->type == type)
	{
	  free_af = af->next;
	  af->next = free_af->next;
	  mem_free (free_af);
	  return;
	}
    }
}

AFFECTED_TYPE *
get_obj_affect (OBJ_DATA * obj, int type)
{
  AFFECTED_TYPE *af;

  if (!obj->xaffected)
    return NULL;

  for (af = obj->xaffected; af; af = af->next)
    if (af->type == type)
      return af;

  return NULL;
}

AFFECTED_TYPE *
get_affect (const CHAR_DATA * ch, int affect_type)
{
  AFFECTED_TYPE *af;

  for (af = ch->hour_affects; af && af->type != affect_type; af = af->next);
    return af;
}

// Don't return true for soma affects that have a latency, e.g. have yet to occur.

AFFECTED_TYPE *
get_soma_affect (const CHAR_DATA * ch, int affect_type)
{
  AFFECTED_TYPE *af;

  for (af = ch->hour_affects; af && af->type != affect_type; af = af->next);
  {
    if (af && af->type >= MAGIC_FIRST_SOMA && af->type <= MAGIC_LAST_SOMA)
    {
      if(af->a.soma.latency <= 0)
        return af;
    }
  }
 
  return NULL;
}


/* Insert an affect_type in a char_data structure
   Automatically sets apropriate bits and apply's.
*/

void
affect_to_char (CHAR_DATA * ch, AFFECTED_TYPE * af)
{
  af->next = ch->hour_affects;
  ch->hour_affects = af;

  affect_modify (ch, af->type, af->a.spell.location, af->a.spell.modifier,
		 af->a.spell.bitvector, true, af->a.spell.sn);
}

void
remove_affect_type (CHAR_DATA * ch, int type)
{
  AFFECTED_TYPE *af;

  if ((af = get_affect (ch, type)))
    affect_remove (ch, af);
}

/* Remove an affected_type structure from a char (called when duration
   reaches zero).  Pointer *af must never be NIL!  Frees mem and calls 
   affect_location_apply.
*/

void
affect_remove (CHAR_DATA * ch, AFFECTED_TYPE * af)
{
  AFFECTED_TYPE *taf;

  affect_modify (ch, af->type, af->a.spell.location, af->a.spell.modifier,
		 af->a.spell.bitvector, false, af->a.spell.sn);

  /* remove structure *af from linked list */

  if (ch->hour_affects == af)
    ch->hour_affects = af->next;

  else
    {
      for (taf = ch->hour_affects; taf && taf->next != af; taf = taf->next)
	;

      if (!taf)
	{
	  return;
	}

      taf->next = af->next;
    }

  if (af->type >= CRAFT_FIRST && af->type <= CRAFT_LAST)
    mem_free (af->a.craft);

  mem_free (af);
}

void
char_from_room (CHAR_DATA * ch)
{
  CHAR_DATA *i;
  ROOM_DATA *room;
  AFFECTED_TYPE *af;

  if (ch->in_room == NOWHERE)
    {
      system_log
	("NOWHERE extracting char from room (handler.c, char_from_room)",
	 true);
      abort ();
    }

  if (ch->room == NULL)
    return;

  room = ch->room;

  if (room && room->sector_type == SECT_LEANTO && IS_MORTAL (ch))
    room->deity--;

  if (ch == room->people)	/* head of list */
    room->people = ch->next_in_room;

  else
    {				/* locate the previous element */

      for (i = room->people; i; i = i->next_in_room)
	{
	  if (i->next_in_room == ch)
	    {
	      i->next_in_room = ch->next_in_room;
	      break;
	    }
	}

    }

  ch->in_room = NOWHERE;
  ch->room = NULL;
  ch->next_in_room = NULL;

  if ((af = get_affect (ch, MAGIC_SIT_TABLE)))
    remove_affect_type (ch, MAGIC_SIT_TABLE);

  if (room)
    {
      room_light (room);
      if (IS_MORTAL (ch) && room->nVirtual >= 100000
	  && IS_SET (room->room_flags, TEMPORARY))
	{
	  if (!room->people)
	    rdelete (room);
	}
    }
}


/* place a character in a room */
void
char_to_room (CHAR_DATA * ch, int room_num)
{
  OBJ_DATA *obj;
  ROOM_DATA *room = NULL, *troom;
  char buf[MAX_STRING_LENGTH];
  int curr = 0;

  // Handle echoes/vNPC decloaking for portable dwellings, i.e. tents.

  if (ch->in_room >= 100000)
    {
      room = vtor (ch->in_room);
      if (room && room->dir_option[OUTSIDE]
	  && (troom = vtor (room->dir_option[OUTSIDE]->to_room)))
	{
	  load_save_room (troom);
	  for (obj = troom->contents; obj; obj = obj->next_content)
	    {
	      if (obj->deleted)
		continue;
	      if (GET_ITEM_TYPE (obj) == ITEM_DWELLING &&
		  IS_SET (obj->obj_flags.extra_flags, ITEM_VNPC_DWELLING) &&
		  obj->o.od.value[0] == ch->in_room)
		{
		  sprintf (buf, "#2%s#0 rustles as its occupants stir.",
			   obj_short_desc (obj));
		  buf[2] = toupper (buf[2]);
		  send_to_room (buf, obj->in_room);
		  obj->obj_flags.extra_flags &= ~ITEM_VNPC;
		  room->occupants++;
		  break;
		}
	    } //for (obj = troom->contents
	} //if (room &&
	
      else if (!room)
	room = vtor (0);
    } //if (ch->in_room >= 100000)
    
  else if (!(room = vtor (room_num)))
    {
      sprintf (buf, "Room %d doesn't exist in char_to_room()! (%s)", room_num,
	       ch->tname);
      system_log (buf, true);
      room = vtor (0);
    }

  if (room->people && room->people != ch)
    ch->next_in_room = room->people;
  else
    ch->next_in_room = NULL;

  room->people = ch;

  ch->in_room = room->nVirtual;
  ch->room = room;

  if (!IS_NPC (ch) && !GET_TRUST (ch))
    room->room_flags |= PC_ENTERED;

  if (room->sector_type == SECT_UNDERWATER
      && !get_affect (ch, AFFECT_HOLDING_BREATH))
    {
      if (IS_MORTAL (ch))
	{
	  act
	    ("You take a deep breath just before plunging into the water. . .",
	     false, ch, 0, 0, TO_CHAR);
	  send_to_char ("\n", ch);
	  curr = MAX ((ch->con*2 + ch->str + ch->agi) * number (1, 3), 25);
	  magic_add_affect (ch, AFFECT_HOLDING_BREATH, curr, 0, 0, 0, curr);
	}
    }
  else if (room->sector_type != SECT_UNDERWATER
	   && get_affect (ch, AFFECT_HOLDING_BREATH))
    {
      if (IS_MORTAL (ch))
	{
	  send_to_char ("\n", ch);
	  act ("You inhale deeply, filling your lungs with much-needed air.",
	       false, ch, 0, 0, TO_CHAR);
	  send_to_char ("\n", ch);
	}
      remove_affect_type (ch, AFFECT_HOLDING_BREATH);
    }

  room_light (ch->room);

  if (!room->psave_loaded)
    load_save_room (room);
}

void
obj_to_char (OBJ_DATA * obj, CHAR_DATA * ch)	/* STACKing */
{
  int dir = 0;
  OBJ_DATA *tobj;
  bool stacked_obj = false;

  // Make sure we have a count of at least 1
  obj->count = (obj->count > 0) ? obj->count : 1;

  // reset any value override
  obj->obj_flags.set_cost = 0;

  /* Do object stacking */

  if (obj->nVirtual == VNUM_PENNY || obj->nVirtual == VNUM_FARTHING)
    return;

  if (GET_ITEM_TYPE (obj) == ITEM_CARD)
    {

      /* if this is a card, we check if we are holding a card of the same kind */
      /* if so add it to a linked list of cards in that hand */

      if (ch->right_hand
	  && GET_ITEM_TYPE (ch->right_hand) == ITEM_CARD
	  && ch->right_hand->o.od.value[0] != 0
	  && ch->right_hand->o.od.value[0] == obj->o.od.value[0])
	{

	  obj->next_content = ch->right_hand;
	  ch->right_hand = obj;
	  obj->o.od.value[5] = obj->next_content->o.od.value[5] + 1;
	  obj->carried_by = ch;
	  obj->location = -1;
	  obj->in_room = NOWHERE;
	  obj->in_obj = NULL;

	  return;
	}
      else if (ch->left_hand
	       && GET_ITEM_TYPE (ch->left_hand) == ITEM_CARD
	       && ch->left_hand->o.od.value[0] != 0
	       && ch->left_hand->o.od.value[0] == obj->o.od.value[0])
	{

	  obj->next_content = ch->left_hand;
	  ch->left_hand = obj;
	  obj->o.od.value[5] = obj->next_content->o.od.value[5] + 1;
	  obj->carried_by = ch;
	  obj->location = -1;
	  obj->in_room = NOWHERE;
	  obj->in_obj = NULL;
	  return;
	}
      else
	{
	  obj->o.od.value[5] = 1;
	}


    }
  else if (IS_SET (obj->obj_flags.extra_flags, ITEM_STACK) &&
	   !(GET_ITEM_TYPE (obj) == ITEM_FOOD &&
	     obj->o.food.food_value != obj->o.food.bites))
    {

      if (ch->right_hand && ch->right_hand->nVirtual == obj->nVirtual
	  && ch->right_hand->morphTime == obj->morphTime)
	{
	  tobj = ch->right_hand;
	  obj->count += tobj->count;
	  if (GET_ITEM_TYPE (obj) == ITEM_MONEY)
	    name_money (obj);
	  extract_obj (tobj);
	  stacked_obj = true;
	  ch->right_hand = obj;
	  obj->carried_by = ch;
	  obj->location = -1;
	  obj->in_room = NOWHERE;
	  obj->in_obj = NULL;
	  return;
	}

      else if (ch->left_hand && ch->left_hand->nVirtual == obj->nVirtual
	       && ch->left_hand->morphTime == obj->morphTime)
	{
	  tobj = ch->left_hand;
	  obj->count += tobj->count;
	  if (obj->nVirtual == VNUM_PENNY || obj->nVirtual == VNUM_FARTHING)
	    name_money (obj);
	  extract_obj (tobj);
	  stacked_obj = true;
	  ch->left_hand = obj;
	  obj->carried_by = ch;
	  obj->location = -1;
	  obj->in_room = NOWHERE;
	  obj->in_obj = NULL;
	  return;
	}
    }

  if (ch->left_hand && get_soma_affect(ch, SOMA_BLUNT_R_SEVARM))
    {
      send_to_char
	("Since your hands are full, you set the object on the ground.\n",
	 ch);
      obj_to_room (obj, ch->in_room);
      obj->in_obj = NULL;
      return;
    }

  if (((ch->right_hand && ch->left_hand) && !stacked_obj)
      || get_equip (ch, WEAR_BOTH))
    {
      send_to_char
	("Since your hands are full, you set the object on the ground.\n",
	 ch);
      obj_to_room (obj, ch->in_room);
      obj->in_obj = NULL;
      return;
    }

  if (get_soma_affect(ch, SOMA_BLUNT_R_SEVARM))
    ch->left_hand = obj;
  else if (!ch->right_hand)
    ch->right_hand = obj;
  else
    ch->left_hand = obj;

  if (GET_ITEM_TYPE (obj) == ITEM_KEY && IS_NPC (ch))
    {
      for (dir = 0; dir <= LAST_DIR; dir++)
	{
	  if (EXIT (ch, dir)
	      && IS_SET (EXIT (ch, dir)->exit_info, EX_LOCKED)
	      && has_key (ch, NULL, EXIT (ch, dir)->key) && !ch->desc)
	    {
	      do_pmote (ch, "keeps a watchful eye on the entryways here.", 0);
	      break;
	    }
	}
    }

  obj->carried_by = ch;

  obj->next_content = NULL;

  obj->location = -1;

  obj->in_room = NOWHERE;

  obj->in_obj = NULL;

  room_light (ch->room);
}

OBJ_DATA *
split_obj (OBJ_DATA * obj, int count)
{
  OBJ_DATA *new_obj;

  if (obj->count < 1 || count >= obj->count)
    return obj;

  obj->count -= count;

  new_obj = load_object (obj->nVirtual);
  new_obj->count = count;

  if (obj->carried_by)
    {

      new_obj->location = -1;

      new_obj->carried_by = obj->carried_by;

      new_obj->in_room = NOWHERE;

      IS_CARRYING_N (obj->carried_by)++;
    }

  else if (obj->in_obj)
    {
      new_obj->next_content = obj->in_obj->contains;
      obj->in_obj->contains = new_obj;
    }

  return new_obj;
}

void
obj_from_char (OBJ_DATA ** obj, int count)	/* STACKing */
{
  int make_money = 0, fluid = 0, volume = 0;
  int nMorphTime = 0;
  CHAR_DATA *ch;

  ch = (*obj)->carried_by;

  if (ch == NULL)
    {
      return;
    }

  if (GET_ITEM_TYPE (*obj) == ITEM_DRINKCON)
    {
      fluid = (*obj)->o.drinkcon.liquid;
      volume = (*obj)->o.drinkcon.volume;
    }

  /* Take a partial number of objs? */

  if (count != 0 && count < (*obj)->count)
    {

      (*obj)->count -= count;

      nMorphTime = (*obj)->morphTime;

      if ((*obj)->nVirtual == VNUM_PENNY || (*obj)->nVirtual == VNUM_FARTHING)
	name_money (*obj);

      if (IS_SET ((*obj)->obj_flags.extra_flags, ITEM_VARIABLE))
	*obj = load_colored_object ((*obj)->nVirtual, (*obj)->var_color, (*obj)->var_color2, (*obj)->var_color3);
      else
	*obj = load_object ((*obj)->nVirtual);

      (*obj)->count = count;
      (*obj)->morphTime = nMorphTime;

      if ((*obj)->nVirtual == VNUM_PENNY || (*obj)->nVirtual == VNUM_FARTHING)
	name_money (*obj);

      (*obj)->carried_by = NULL;


      if (GET_ITEM_TYPE (*obj) == ITEM_DRINKCON)
	{
	  (*obj)->o.drinkcon.liquid = fluid;
	  (*obj)->o.drinkcon.volume = volume;
	}

      return;
    }

  /* Remove object from inventory */

  if (ch->right_hand == *obj)
    ch->right_hand = NULL;
  else if (ch->left_hand == *obj)
    ch->left_hand = NULL;

  if (make_money)
    *obj = create_money (count);
  else
    IS_CARRYING_N (ch)--;

  (*obj)->carried_by = NULL;
  (*obj)->next_content = NULL;
  (*obj)->equiped_by = NULL;
  (*obj)->in_room = NOWHERE;
  (*obj)->in_obj = NULL;

  room_light (ch->room);
}

void
equip_char (CHAR_DATA * ch, OBJ_DATA * obj, int pos)
{
  char buf[MAX_STRING_LENGTH];

  if (ch == 0)
    {
      sprintf (buf,
	       "#1OBJECT MORPHING BUG! NULL ch pointer. Crash averted. Object vnum %d in room %d.#0\n",
	       obj->nVirtual,
	       obj->in_room);
      send_to_gods (buf);
      system_log (buf, true);
      return;
    }

  if (pos < 0)
    {
      sprintf (buf,
	       "#1OBJECT MORPHING BUG! Crash averted. Position %d, vnum %d, character %s in room %d.#0\n",
	       pos, obj->nVirtual, ch->tname, obj->in_room);
      send_to_gods (buf);
      system_log (buf, true);
      return;
    }

  if (get_equip (ch, pos))
    return;


  if (obj->in_room != NOWHERE)
    {
      system_log ("EQUIP: Obj is in_room when equip.", true);
      return;
    }

  obj->location = pos;

  if (pos != WEAR_PRIM && pos != WEAR_SEC && pos != WEAR_BOTH
      && pos != WEAR_SHIELD)
    {
      obj->next_content = ch->equip;
      ch->equip = obj;
    }

  if (pos == WEAR_PRIM || pos == WEAR_SEC || pos == WEAR_BOTH
      || pos == WEAR_SHIELD)
    obj->carried_by = ch;

  obj->equiped_by = ch;
}

OBJ_DATA *
unequip_char (CHAR_DATA * ch, int pos)
{
  OBJ_DATA *obj;
  OBJ_DATA *tobj;

  obj = get_equip (ch, pos);

  if (!obj)
    return NULL;

  if (IS_SET (obj->obj_flags.extra_flags, ITEM_MASK) && is_hooded (ch))
    ch->affected_by &= ~AFF_HOODED;

  if (ch->equip == obj)
    ch->equip = ch->equip->next_content;
  else
    {
      for (tobj = ch->equip; tobj; tobj = tobj->next_content)
	if (tobj->next_content && obj == tobj->next_content)
	  {
	    tobj->next_content = obj->next_content;
	    break;
	  }
    }

  obj->location = -1;
  obj->equiped_by = NULL;
  obj->next_content = NULL;

  if (ch->aiming_at)
    {
      ch->aiming_at->targeted_by = NULL;
      ch->aiming_at = NULL;
      ch->aim = 0;
      ch->delay_info1 = 0;
      ch->delay_who = NULL;
      send_to_char ("You stop aiming your weapon.\n", ch);
    }

  return (obj);
}


int
get_number (char **name)
{

  int i;
  char *ppos = '\0';
  char number[MAX_INPUT_LENGTH] = { '\0' };

  if ((ppos = (char *) strchr (*name, '.')))
    {
      *ppos++ = '\0';
      strcpy (number, *name);
      strcpy (*name, ppos);

      for (i = 0; *(number + i); i++)
	if (!isdigit (*(number + i)))
	  return (0);

      return (atoi (number));
    }

  return (1);
}

OBJ_DATA *
get_carried_item (CHAR_DATA * ch, int item_type)
{
  if (!ch)
    return NULL;

  if (ch->right_hand && GET_ITEM_TYPE (ch->right_hand) == item_type)
    return ch->right_hand;

  if (ch->left_hand && GET_ITEM_TYPE (ch->left_hand) == item_type)
    return ch->left_hand;

  return NULL;
}

/* Search a given list for an object, and return a pointer to that object */
OBJ_DATA *
get_obj_in_list (char *name, OBJ_DATA * list)
{
  OBJ_DATA *i, *drink;
  int j, number;
  char tmpname[MAX_INPUT_LENGTH];
  char *tmp;

  strcpy (tmpname, name);
  tmp = tmpname;
  if (!(number = get_number (&tmp)))
    return (0);
  
  for (i = list, j = 1; i && (j <= number); i = i->next_content)
    {
      if (isname (tmp, i->name)
	  || (GET_ITEM_TYPE (i) == ITEM_BOOK && i->book_title
	      && isname (tmp, i->book_title))
	  || (GET_ITEM_TYPE (i) == ITEM_DRINKCON
	      && i->o.drinkcon.volume
	      && (drink = vtoo (i->o.drinkcon.liquid))
	      && isname (tmp, drink->name)))
	{
	  if (j == number)
	    return (i);
	  j++;
	}
    }

  return (0);
}

OBJ_DATA *
get_obj_in_list_num (int num, OBJ_DATA * list)
{
  OBJ_DATA *obj;

  for (obj = list; obj; obj = obj->next_content)
    if (obj->nVirtual == num)
      return obj;

  return NULL;
}

/*search the entire world for an object, and return a pointer  */
OBJ_DATA *
get_obj (char *name)
{
  OBJ_DATA *i,*drink;
  int j, number;
  char tmpname[MAX_INPUT_LENGTH];
  char *tmp;

  strcpy (tmpname, name);
  tmp = tmpname;
  if (!(number = get_number (&tmp)))
    return (0);

  for (i = object_list, j = 1; i && (j <= number); i = i->next)
    {

      if (i->deleted)
	continue;

      if (isname (tmp, i->name)
	  || (GET_ITEM_TYPE (i) == ITEM_BOOK && i->book_title
	      && isname (tmp, i->book_title))
	  || (GET_ITEM_TYPE (i) == ITEM_DRINKCON
	      && i->o.drinkcon.volume
	      && (drink = vtoo (i->o.drinkcon.liquid))
	      && isname (tmp, drink->name)))
	{
	  if (j == number)
	    return (i);
	  j++;
	}
    }

  return (0);
}

CHAR_DATA *
get_char_id (int coldload_id)
{
  CHAR_DATA *ch;

  for (ch = character_list; ch; ch = ch->next)
    {
      if (ch->deleted)
	continue;
      if (ch->coldload_id == coldload_id)
	return ch;
    }

  return NULL;
}

OBJ_DATA *
get_obj_in_list_id (int coldload_id, OBJ_DATA * list)
{
  OBJ_DATA *obj;

  for (obj = list; obj; obj = obj->next_content)
    {
      if (obj->deleted)
	continue;
      if (obj->coldload_id == coldload_id)
	return obj;
    }

  return NULL;
}

OBJ_DATA *
get_obj_id (int coldload_id)
{
  OBJ_DATA *obj;

  for (obj = object_list; obj; obj = obj->next)
    {
      if (obj->deleted)
	continue;
      if (obj->coldload_id == coldload_id)
	return obj;
    }

  return NULL;
}

/* search a room for a char, and return a pointer if found..  */
CHAR_DATA *
get_char_room (char *name, int room)
{
  CHAR_DATA *i;
  int j, number;
  char tmpname[MAX_INPUT_LENGTH];
  char *tmp;

  strcpy (tmpname, name);
  tmp = tmpname;
  if (!(number = get_number (&tmp)))
    return (0);

  for (i = vtor (room)->people, j = 1; i && (j <= number);
       i = i->next_in_room)
    if (isname (tmp, char_names (i)))
      {
	if (j == number)
	  return (i);
	j++;
      }

  return (0);
}

/* search all over the world for a char, and return a pointer if found */
CHAR_DATA *
get_char (char *name)
{
  CHAR_DATA *i;
  int j;
  int number;
  char tmpname[MAX_INPUT_LENGTH];
  char *tmp;

  strcpy (tmpname, name);
  tmp = tmpname;

  if (!(number = get_number (&tmp)))
    return 0;

  for (i = character_list, j = 1; i && (j <= number); i = i->next)
    {

      if (i->deleted)
	continue;

      if (isname (tmp, char_names (i)))
	{
	  if (j == number)
	    return (i);
	  j++;
	}
    }

  return 0;
}

/* search all over the world for a char, and return a pointer if found */
CHAR_DATA *
get_mob_vnum (int nVirtual)
{
  CHAR_DATA *i;

  for (i = character_list; i; i = i->next)
    {

      if (i->deleted)
	continue;

      if (!IS_NPC (i))
	continue;

      if (i->mob->nVirtual == nVirtual)
	return i;
    }

  return 0;
}

/* search all over the world for a char, and return a pointer if found */
CHAR_DATA *
get_char_nomask (char *name)
{
  CHAR_DATA *i;
  int j;
  int number;
  char tmpname[MAX_INPUT_LENGTH];
  char *tmp;

  strcpy (tmpname, name);
  tmp = tmpname;

  if (!(number = get_number (&tmp)))
    return 0;

  for (i = character_list, j = 1; i && (j <= number); i = i->next)
    {

      if (i->deleted)
	continue;

      if (isname (tmp, GET_NAMES (i)))
	{
	  if (j == number)
	    return (i);
	  j++;
	}
    }

  return 0;
}

void
obj_to_room (OBJ_DATA * object, int room)	/* STACKing */
{
  int add_at_top = 0;
  ROOM_DIRECTION_DATA *exit;
  ROOM_DATA *r, *troom = NULL;
  OBJ_DATA *tobj;
  char buf[MAX_STRING_LENGTH];

  if (!object)
    return;

  if (object->nVirtual == VNUM_PENNY || object->nVirtual == VNUM_FARTHING)
    return;

  r = vtor (room);

  if (!r)
    return;

  if (SWIM_ONLY (r))
    {
      tobj = object->next_content;
      object->next_content = NULL;
      object__drench (NULL, object, false);
      object->next_content = tobj;
    }

  if (r->contents
      && IS_SET (object->obj_flags.extra_flags, ITEM_STACK)
      && !get_obj_affect (object, MAGIC_HIDDEN))
    {

      for (tobj = r->contents; tobj; tobj = tobj->next_content)
	{

	  if (tobj->nVirtual != object->nVirtual
	      || get_obj_affect (tobj, MAGIC_HIDDEN)
	      || tobj->morphTime != object->morphTime)
	    continue;

	  tobj->count += object->count;
	  extract_obj (object);
	  room_light (r);
	  return;
	}
    }

  if (add_at_top)
    {
      object->next_content = r->contents;
      r->contents = object;
    }
  else
    {
      if (!r->contents)
	{
	  r->contents = object;
	  object->next_content = NULL;
	}
      else
	for (tobj = r->contents; tobj; tobj = tobj->next_content)
	  {
	    if (!tobj->next_content)
	      {
		tobj->next_content = object;
		object->next_content = NULL;
		break;
	      }
	  }
    }

  object->in_room = room;
  object->carried_by = 0;
  object->equiped_by = NULL;
  object->in_obj = NULL;
  object->location = -1;

  room_light (r);

  exit = r->dir_option[DOWN];

  if (exit 
  		&& !IS_SET (exit->exit_info, EX_ISDOOR)
  		&& !IS_SET (exit->exit_info, EX_ISGATE))
    {
      troom = vtor (exit->to_room);

      if (troom && troom->sector_type == SECT_UNDERWATER && SWIM_ONLY(r))
	{

	  if (object->obj_flags.weight / 100 >= 1
	      || GET_ITEM_TYPE (object) == ITEM_MONEY)
	    {
	      sprintf (buf,
		       "#2%s#0 slowly sinks from sight down into the water.",
		       obj_short_desc (object));
	      buf[2] = toupper (buf[2]);
	      send_to_room (buf, r->nVirtual);
	      obj_from_room (&object, 0);
	      obj_to_room (object, troom->nVirtual);
	      sprintf (buf, "#2%s#0 slowly sinks into view from above.",
		       obj_short_desc (object));
	      buf[2] = toupper (buf[2]);
	      send_to_room (buf, object->in_room);
	    }

	}
      else if (troom
	       && (IS_SET (r->room_flags, CLIMB)
		   || IS_SET (r->room_flags, FALL)))
	{
	  sprintf (buf, "#2%s#0 plummets down!", obj_short_desc (object));
	  buf[2] = toupper (buf[2]);
	  send_to_room (buf, r->nVirtual);
	  obj_from_room (&object, 0);
	  obj_to_room (object, troom->nVirtual);
	  sprintf (buf, "#2%s#0 falls from above.", obj_short_desc (object));
	  buf[2] = toupper (buf[2]);
	  send_to_room (buf, object->in_room);
	}
    }

}

/* Take an object from a room */
void
obj_from_room (OBJ_DATA ** obj, int count)	/* STACKing */
{
  int make_money = 0, fluid = 0, volume = 0;
  int nMorphTime = 0;
  OBJ_DATA *tobj;
  ROOM_DATA *room;

  room = vtor ((*obj)->in_room);

  if (!room)
    return;

  // Ensure that any dwelling rooms are destroyed now that dwelling has been moved/removed.

  if (GET_ITEM_TYPE (*obj) == ITEM_DWELLING && (*obj)->o.od.value[0]
      && vtor ((*obj)->o.od.value[0]))
    {
      delete_contiguous_rblock (vtor ((*obj)->o.od.value[0]), -1,
				room->nVirtual);
      (*obj)->o.od.value[0] = 0;
    }

  if (GET_ITEM_TYPE (*obj) == ITEM_DRINKCON)
    {
      fluid = (*obj)->o.drinkcon.liquid;
      volume = (*obj)->o.drinkcon.volume;
    }

  /* NPC jailbags disappear, unless they are taken */

  if ((*obj)->nVirtual == VNUM_JAILBAG)
    (*obj)->obj_timer = 0;

  /* Take a partial number of objs? */

  if (count != 0 && count < (*obj)->count)
    {

      (*obj)->count -= count;
      nMorphTime = (*obj)->morphTime;

      if ((*obj)->nVirtual == VNUM_PENNY || (*obj)->nVirtual == VNUM_FARTHING)
	name_money (*obj);

      if (IS_SET ((*obj)->obj_flags.extra_flags, ITEM_VARIABLE))
	*obj = load_colored_object ((*obj)->nVirtual, (*obj)->var_color, (*obj)->var_color2, (*obj)->var_color3);
      else
	*obj = load_object ((*obj)->nVirtual);

      (*obj)->count = count;
      (*obj)->morphTime = nMorphTime;

      if (GET_ITEM_TYPE (*obj) == ITEM_DRINKCON)
	{
	  (*obj)->o.drinkcon.liquid = fluid;
	  (*obj)->o.drinkcon.volume = volume;
	}

      if ((*obj)->nVirtual == VNUM_PENNY || (*obj)->nVirtual == VNUM_FARTHING)
	name_money (*obj);

      room_light (room);

      (*obj)->in_room = NOWHERE;
      (*obj)->next_content = NULL;

      return;
    }

  /* Remove object from the room */

  if (room->contents == *obj)
    room->contents = (*obj)->next_content;
  else if (room->contents)
    {
      for (tobj = room->contents;
	   tobj->next_content; tobj = tobj->next_content)
	if (tobj->next_content == *obj)
	  {
	    tobj->next_content = (*obj)->next_content;
	    break;
	  }
    }

  if (make_money)
    *obj = create_money (count);

  (*obj)->in_room = NOWHERE;
  (*obj)->next_content = NULL;

  room_light (room);

  remove_obj_affect (*obj, MAGIC_HIDDEN);
}

void
obj_to_obj (OBJ_DATA * obj, OBJ_DATA * container)
{
  OBJ_DATA *tobj;

  if (!obj || !container)
    return;

  if (container->contains && IS_SET (obj->obj_flags.extra_flags, ITEM_STACK))
    {

      for (tobj = container->contains; tobj; tobj = tobj->next_content)
	{

	  if (tobj->nVirtual != obj->nVirtual
	      || tobj->morphTime != obj->morphTime)
	    continue;

	  tobj->count += obj->count;
          container->contained_wt += OBJ_MASS (tobj);
	  extract_obj (obj);
	  return;
	}
    }

  obj->in_obj = container;
  obj->equiped_by = NULL;
  obj->carried_by = NULL;
  if (container->contains && !is_obj_in_list (obj, container->contains))
    obj->next_content = container->contains;
  else if (!container->contains)
    container->contains = obj;
  else
    {
      extract_obj (obj);
      return;
    }

  container->contains = obj;
  obj->location = -1;

  for (tobj = container; tobj; tobj = tobj->in_obj)
    tobj->contained_wt += OBJ_MASS (obj);
}

void
obj_from_obj (OBJ_DATA ** obj, int count)
{
  int adjust_wt;
  int fluid = 0, volume = 0;
  OBJ_DATA *tobj;

  if (!(*obj))
    return;

  if (count < 0)
    count = 0;

  if (!count)
    count = (*obj)->count;

  if (GET_ITEM_TYPE (*obj) == ITEM_DRINKCON)
    {
      fluid = (*obj)->o.drinkcon.liquid;
      volume = (*obj)->o.drinkcon.volume;
    }

  /* Removing only part of obj from container */

  if (count < (*obj)->count)
    {

      tobj = *obj;

      if (IS_SET (tobj->obj_flags.extra_flags, ITEM_VARIABLE))
	*obj = load_colored_object (tobj->nVirtual, tobj->var_color, tobj->var_color2, tobj->var_color3);
      else
	*obj = load_object (tobj->nVirtual);

      tobj->count -= count;

      (*obj)->count = count;
      (*obj)->morphTime = tobj->morphTime;
      (*obj)->in_obj = tobj->in_obj;

      if (GET_ITEM_TYPE (*obj) == ITEM_DRINKCON)
	{
	  (*obj)->o.drinkcon.liquid = fluid;
	  (*obj)->o.drinkcon.volume = volume;
	}

      adjust_wt = (*obj)->count * (*obj)->obj_flags.weight;
    }

  else
    {
      /* Removing obj completely from container */

      adjust_wt = OBJ_MASS (*obj);

      if ((*obj)->in_obj->contains == *obj)
	(*obj)->in_obj->contains = (*obj)->next_content;
      else
	{
	  for (tobj = (*obj)->in_obj->contains;
	       tobj->next_content; tobj = tobj->next_content)
	    if (tobj->next_content == *obj)
	      {
		tobj->next_content = (*obj)->next_content;
		break;
	      }
	}
    }

  for (tobj = (*obj)->in_obj; tobj; tobj = tobj->in_obj)
    tobj->contained_wt -= adjust_wt;

  (*obj)->next_content = NULL;
  (*obj)->in_obj = NULL;
}

void
object_list_new_owner (OBJ_DATA * list, CHAR_DATA * ch)
{
  if (list)
    {
      object_list_new_owner (list->contains, ch);
      object_list_new_owner (list->next_content, ch);
      list->carried_by = ch;
    }
}

void
remove_object_affect (OBJ_DATA * obj, AFFECTED_TYPE * af)
{
  AFFECTED_TYPE *taf;

  if (af == obj->xaffected)
    {
      obj->xaffected = af->next;
      mem_free (af);
      return;
    }

  for (taf = obj->xaffected; taf->next; taf = taf->next)
    {
      if (af == taf->next)
	{
	  taf->next = af->next;
	  mem_free (af);
	  return;
	}
    }
}

void
destroy_dwelling (OBJ_DATA * obj)
{
  OBJ_DATA *tobj, *tobj_next;
  CHAR_DATA *tch, *tch_next;
  ROOM_DATA *troom;
  char buf[MAX_STRING_LENGTH];

  if (!(troom = vtor (obj->o.od.value[0])))
    return;

  sprintf (buf, "#2%s#0 is suddenly torn down around your ears!",
	   obj_short_desc (obj));
  buf[2] = toupper (buf[2]);
  send_to_room (buf, troom->nVirtual);

  for (tobj = troom->contents; tobj; tobj = tobj_next)
    {
      tobj_next = tobj->next_content;
      obj_from_room (&tobj, 0);
      obj_to_room (tobj, obj->in_room);
    }

  for (tch = troom->people; tch; tch = tch_next)
    {
      tch_next = tch->next_in_room;
      char_from_room (tch);
      char_to_room (tch, obj->in_room);
    }
}

void
extract_obj (OBJ_DATA * obj)
{
  OBJ_DATA *tobj;

  if (GET_ITEM_TYPE (obj) == ITEM_DWELLING)
    destroy_dwelling (obj);

  while (obj->contains)
    extract_obj (obj->contains);

  if (obj->equiped_by != NULL)
    {
      tobj = unequip_char (obj->equiped_by, obj->location);
      extract_obj (tobj);
    }

  if (obj->carried_by != NULL)
    obj_from_char (&obj, 0);
  else if (obj->in_obj != NULL)
    obj_from_obj (&obj, 0);
  else if (obj->in_room != NOWHERE)
    obj_from_room (&obj, 0);

  while (obj->xaffected)
    remove_object_affect (obj, obj->xaffected);

  obj->carried_by = NULL;
  obj->equiped_by = NULL;
  obj->in_obj = NULL;

  obj->deleted = 1;

  knockout = 1;			/* Get obj out of object_list ASAP */

  vtoo (obj->nVirtual)->instances--;
}

void
morph_obj (OBJ_DATA * obj)
{

  char buf[MAX_STRING_LENGTH];
  OBJ_DATA *newObj;

  if (obj->deleted)
    return;

  if (obj->in_room && vtor (obj->in_room)
      && IS_SET (vtor (obj->in_room)->room_flags, STORAGE))
    return;

  if (obj->morphto)
    {
      if (obj->morphto == 86)
	{
	  extract_obj (obj);
	  return;
	}
      else
	{
	  newObj = load_object (obj->morphto);
	}
    }
  else
    {
      sprintf (buf, "Object %d has a morph clock, but no morph Objnum\n",
	       obj->nVirtual);
      system_log (buf, true);
      return;
    }

  if (!newObj)
    {
      sprintf (buf, "Attempt to load morph obj %d for object %d failed\n",
	       obj->morphto, obj->nVirtual);
      system_log (buf, true);
      return;
    }

  newObj->count = obj->count;
  if (obj->equiped_by)
    {
      unequip_char (obj->equiped_by, obj->location);
      equip_char (obj->equiped_by, newObj, obj->location);
    }

  if (obj->carried_by)
    {
      CHAR_DATA *ch;

      ch = obj->carried_by;

      obj_from_char (&obj, 0);
      obj_to_char (newObj, ch);
    }


  if (obj->in_obj)
    {
      OBJ_DATA *container;

      container = obj->in_obj;
      obj_from_obj (&obj, 0);

      obj_to_obj (newObj, container);
    }

  if (obj->in_room != NOWHERE)
    {
      int room;

      room = obj->in_room;
      obj_from_room (&obj, 0);

      obj_to_room (newObj, room);
    }

  while (obj->xaffected)
    remove_object_affect (obj, obj->xaffected);

  obj->deleted = 1;
}

void
remove_sighting (CHAR_DATA * ch, CHAR_DATA * target)
{
  SIGHTED_DATA *sighted;

  if (!ch || !target)
    return;

  if (ch->sighted && ch->sighted->target == target)
    ch->sighted = ch->sighted->next;

  for (sighted = ch->sighted; sighted; sighted = sighted->next)
    {
      if (sighted->next && sighted->next->target == target)
	sighted->next = sighted->next->next;
    }
}

void
extract_char (CHAR_DATA * ch)
{
  CHAR_DATA *k;
  CHAR_DATA *next_char;
  CHAR_DATA *tch;
  ROOM_DATA *troom;
  OBJ_DATA *tobj;
  DESCRIPTOR_DATA *td;
  AFFECTED_TYPE *af;
  SECOND_AFFECT *sa;
  int was_in;
  char buf[MAX_STRING_LENGTH];

  extern struct timeval time_now;

  if (engine.in_play_mode () && IS_NPC (ch))
    {
      if (IS_SET (ch->act, ACT_STAYPUT))
	{
	  mysql_safe_query 
	    ("DELETE FROM stayput_mobiles"
	     " WHERE coldload_id = %d",
	     ch->coldload_id);
	  sprintf (buf, "save/mobiles/%d", ch->coldload_id);
	  unlink (buf);
	}

      mysql_safe_query ("DELETE FROM reboot_mobiles WHERE coldload_id = %d",
			ch->coldload_id);
      sprintf (buf, "save/reboot/%d", ch->coldload_id);
      unlink (buf);
    }

  if (ch->mob &&
      IS_SET (ch->act, ACT_VEHICLE) && (troom = vtor (ch->mob->nVirtual)))
    {

      /* By mistake, the vehicle might be in the same room as its
         entrance room.  In that case, n/pcs and mobs don't need to
         be moved.
       */

      if (troom == ch->room)
	{
	  act ("$n is destroyed and some people fall out.",
	       false, ch, 0, 0, TO_ROOM);
	  for (tch = troom->people; tch; tch = tch->next)
	    tch->vehicle = NULL;
	}

      else
	{

	  while (troom->people)
	    {
	      tch = troom->people;
	      char_from_room (tch);
	      char_to_room (tch, ch->in_room);
	      tch->vehicle = NULL;
	      act ("$N falls out of $n.", false, ch, 0, tch, TO_NOTVICT);
	      act ("You fall out of $N as it is destroyed!",
		   false, tch, 0, ch, TO_CHAR);
	    }

	  while (troom->contents)
	    {
	      tobj = troom->contents;
	      obj_from_room (&tobj, 0);
	      obj_to_room (tobj, ch->in_room);
	    }
	}
    }

  if (ch->desc && ch->desc->original)
    do_return (ch, "", 0);

  if (!IS_NPC (ch) && ch->room)
    save_attached_mobiles (ch, 1);

  if (!IS_NPC (ch))
    clear_watch (ch);

  /* Clear out guarders and watchers*/

  for (k = character_list; k; k = k->next)
    {
      if (k->deleted)
	continue;
      if ((af = get_affect (k, MAGIC_GUARD)) &&
	  (CHAR_DATA *) af->a.spell.t == ch)
	affect_remove (k, af);
    }

  for (k = character_list; k; k = k->next)
    {
      if (k->deleted)
	continue;
      if ((af = get_affect (k, MAGIC_WATCH)) &&
	  (CHAR_DATA *) af->a.spell.t == ch)
	affect_remove (k, af);
    }

  for (k = character_list; k; k = k->next)
    {
      if (k->deleted)
	continue;
      if (k->ranged_enemy == ch)
	{
	  k->ranged_enemy = NULL;
	  k->enemy_direction = NULL;
	}
      if (k->aiming_at == ch)
	{
	  k->aiming_at->targeted_by = NULL;
	  k->aiming_at = NULL;
	  k->aim = 0;
	}
      else if (k->targeted_by == ch)
	{
	  k->targeted_by->aiming_at = NULL;
	  k->targeted_by->aim = 0;
	  k->targeted_by = NULL;
	}
      if (k->subdue == ch)
	k->subdue = NULL;
      remove_threat (k, ch);
      remove_attacker (k, ch);
      remove_sighting (k, ch);
    }

  // Clear out second affects.

  for (sa = second_affect_list; sa; sa = sa->next)
  {
    if(sa->ch == ch)
      remove_second_affect(sa);
  }

  if (!IS_NPC (ch) && ch->pc->edit_player)
    unload_pc (ch->pc->edit_player);

  if (!IS_NPC (ch))
    save_char (ch, true);

  if (!IS_NPC (ch) && !ch->desc)
    {
      for (td = descriptor_list; td; td = td->next)
	if (td->original == ch)
	  do_return (td->character, "", 0);
    }

  if (ch->in_room == NOWHERE)
    {
      system_log ("NOWHERE extracting char. (handler.c, extract_char)", true);
      //abort ();
    }

  stop_followers (ch);

  /* Enable reset for killed mob */

  if (IS_NPC (ch))
    {
      if (ch->mob->reset_zone != 0 || ch->mob->reset_cmd != 0)
	zone_table[ch->mob->reset_zone].cmd[ch->mob->reset_cmd].enabled = 1;
    }

  if (ch->desc)
    {
      if (ch->desc->snoop.snooping && ch->desc->snoop.snooping->desc)
	ch->desc->snoop.snooping->desc->snoop.snoop_by = 0;

      if (ch->desc->snoop.snoop_by && ch->desc->snoop.snoop_by->desc)
	{
	  send_to_char ("Your victim is no longer among us.\n\r",
			ch->desc->snoop.snoop_by);
	  ch->desc->snoop.snoop_by->desc->snoop.snooping = 0;
	}

      ch->desc->snoop.snooping = ch->desc->snoop.snoop_by = 0;
    }

  if (ch->fighting)
    stop_fighting (ch);

  for (k = combat_list; k; k = next_char)
    {				/* FUNDAMENTALLY FLAWED LOGIC */
      next_char = k->next_fighting;
      if (k->fighting == ch)
	stop_fighting (k);
    }

  if (get_queue_position (ch) != -1)
    update_assist_queue (ch, true);

  /* Must remove from room before removing the equipment! */

  was_in = ch->in_room;
  char_from_room (ch);
  ch->in_room = was_in;

  if (ch->right_hand)
    extract_obj (ch->right_hand);

  if (ch->left_hand)
    extract_obj (ch->left_hand);

  while (ch->equip)
    extract_obj (ch->equip);

  ch->deleted = 1;

  if (IS_NPC (ch))
    {
      while (ch->hour_affects)
	affect_remove (ch, ch->hour_affects);
    }

  if (ch->desc != NULL)
    ch->desc->character = NULL;

  knockout = 1;			/* Get N/PC out of character_list ASAP */

  if (ch->desc != NULL && !ch->desc->acct
      && !IS_SET (ch->flags, FLAG_GUEST))
    {
      td = ch->desc;
      if (!maintenance_lock)
	SEND_TO_Q (get_text_buffer (NULL, text_list, "greetings"), td);
      else
	SEND_TO_Q (get_text_buffer (NULL, text_list, "greetings.maintenance"),
		   td);
      SEND_TO_Q ("Your Choice: ", td);
      td->connected = CON_LOGIN;
      ch->desc = NULL;
      td->character = NULL;
      td->original = NULL;
      td->prompt_mode = 0;
      td->login_time = time_now;
      td->time_last_activity = mud_time;
    }
}

CHAR_DATA *
get_char_room_vis (CHAR_DATA * ch, const char *name)
{
  CHAR_DATA *tch;
  int j = 1;
  int number;
  char tmpname[MAX_STRING_LENGTH];
  char immkeys[AVG_STRING_LENGTH];
  char *tmp;
  char *tchtemp;

  /* The player may use '.' to indicate last targeted n/pc. */

  if (!strcmp (name, "."))
    {
      if (!ch->pc)
	return NULL;

      if (is_he_here (ch, ch->pc->dot_shorthand, true))
	return ch->pc->dot_shorthand;
    }

  if (!strcmp (name, "self") || !strcmp (name, "me"))
    return ch;

  strcpy (tmpname, name);

  tmp = tmpname;

  if (!(number = get_number (&tmp)))
    return NULL;

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {

      sprintf (immkeys, "%s %s person", char_names (tch), tch->name);

			if (are_grouped (ch, tch) && IS_MORTAL (ch))
				tchtemp = tch->name;
			else if (IS_MORTAL (ch))
				tchtemp = char_names (tch);
			else 
				tchtemp = tch->name;
				
				
     // if (isname (tmp, IS_MORTAL (ch) ? char_names (tch) : immkeys))
     if (isname (tmp, tchtemp) || isname (tmp, char_names (tch)))
				{
			
					if (CAN_SEE (ch, tch) || ch == tch)
						{
	      if (j == number)
		{
		  if (ch->pc)
		    ch->pc->dot_shorthand = tch;
		  return tch;
		}

	      j++;
	    }
	}
    }

  return NULL;
}

CHAR_DATA *
get_char_room_vis2 (CHAR_DATA * ch, int vnum, char *name)
{
  ROOM_DATA *room;
  CHAR_DATA *tch;
  int j = 1;
  int number;
  char tmpname[MAX_STRING_LENGTH];
  char immkeys[AVG_STRING_LENGTH];
  char *tmp;

  if (!(room = vtor (vnum)))
    return NULL;

  /* The player may use '.' to indicate last targeted n/pc. */

  if (!strcmp (name, "."))
    {
      if (!ch->pc)
	return NULL;

      if (is_he_here (ch, ch->pc->dot_shorthand, true))
	return ch->pc->dot_shorthand;
    }

  if (!strcmp (name, "self") || !strcmp (name, "me"))
    return ch;

  strcpy (tmpname, name);

  tmp = tmpname;

  if (!(number = get_number (&tmp)))
    return NULL;

  for (tch = room->people; tch; tch = tch->next_in_room)
    {

      sprintf (immkeys, "%s %s person", char_names (tch), tch->name);

      if (isname (tmp, IS_MORTAL (ch) ? char_names (tch) : immkeys))
	{
	  if (CAN_SEE (ch, tch) || ch == tch)
	    {

	      if (j == number)
		{
		  if (ch->pc)
		    ch->pc->dot_shorthand = tch;
		  return tch;
		}

	      j++;
	    }
	}
    }

  return NULL;
}

CHAR_DATA *
get_char_vis (CHAR_DATA * ch, char *name)
{
  CHAR_DATA *i;
  int j, number;
  char tmpname[MAX_INPUT_LENGTH];
  char *tmp;

  if (!strcmp (name, "self") || !strcmp (name, "me"))
    return ch;

  /* check location */
  if ((i = get_char_room_vis (ch, name)))
    return (i);

  strcpy (tmpname, name);
  tmp = tmpname;
  if (!(number = get_number (&tmp)))
    return (0);

  for (i = character_list, j = 1; i && (j <= number); i = i->next)
    {

      if (i->deleted)
	continue;

      if (isname (tmp, GET_NAMES (i)))
	if (CAN_SEE (ch, i))
	  {
	    if (j == number)
	      return i;
	    j++;
	  }
    }

  return 0;
}

OBJ_DATA *
get_obj_in_list_vis_not_money (CHAR_DATA * ch, char *name, OBJ_DATA * list)
{
  OBJ_DATA *obj,*drink;
  int j = 1;
  int number;
  char tmpname[MAX_INPUT_LENGTH];
  char *tmp;

  strcpy (tmpname, name);
  tmp = tmpname;

  if (!(number = get_number (&tmp)))
    return NULL;

  if (isdigit (*name))
    {

      if (!(number = atoi (name)))
	return NULL;

      for (obj = list; number && obj; obj = obj->next_content)
	{

	  if (obj->nVirtual == VNUM_FARTHING || obj->nVirtual == VNUM_PENNY)
	    continue;

	  if (GET_ITEM_TYPE (obj) == ITEM_MONEY)
	    continue;

	  if (CAN_SEE_OBJ (ch, obj) && !(--number))
	    return obj;
	}

      return NULL;
    }

  for (obj = list; obj && (j <= number); obj = obj->next_content)
    {

      if (obj->nVirtual == VNUM_FARTHING || obj->nVirtual == VNUM_PENNY)
	continue;

      if (GET_ITEM_TYPE (obj) == ITEM_MONEY)
	continue;

      if (isname (tmp, obj->name)
	  || (GET_ITEM_TYPE (obj) == ITEM_BOOK && obj->book_title
	      && isname (tmp, obj->book_title))
	  || (GET_ITEM_TYPE (obj) == ITEM_DRINKCON
	      && obj->o.drinkcon.volume
	      && (drink = vtoo (obj->o.drinkcon.liquid))
	      && isname (tmp, drink->name)))
	if (CAN_SEE_OBJ (ch, obj))
	  {
	    if (j == number)
	      return (obj);
	    j++;
	  }
    }

  return 0;
}

OBJ_DATA *
get_obj_in_list_vis (CHAR_DATA * ch, const char *name, OBJ_DATA * list)
{
  OBJ_DATA *obj;
  int j = 1;
  int number;
  char tmpname[MAX_INPUT_LENGTH];
  char *tmp;

  if (!name || !*name)
    return NULL;

  if (!list)
    return NULL;

  strcpy (tmpname, name);
  tmp = tmpname;

  if (!(number = get_number (&tmp)))
    return NULL;

  if (*name == '#')
    {

      if (!(number = atoi (&name[1])))
	return NULL;

      for (obj = list; number && obj; obj = obj->next_content)
	if (CAN_SEE_OBJ (ch, obj) && !(--number))
	  return obj;

      return NULL;
    }

  for (obj = list; obj && (j <= number); obj = obj->next_content)
    {
      if (isname (tmp, obj->name)
	  || (GET_ITEM_TYPE (obj) == ITEM_BOOK && obj->book_title
	      && isname (tmp, obj->book_title)))
	if (CAN_SEE_OBJ (ch, obj))
	  {
	    if (j == number)
	      return (obj);
	    j++;
	  }
    }

  return 0;
}

OBJ_DATA *
get_obj_in_dark (CHAR_DATA * ch, char *name, OBJ_DATA * list)
{
  OBJ_DATA *obj;
  int j = 1;
  int number;
  char tmpname[MAX_INPUT_LENGTH];
  char *tmp = '\0';

  *tmpname = '\0';

  strcpy (tmpname, name);
  tmp = tmpname;

  if (!(number = get_number (&tmp)))
    return 0;

  if (*name == '#')
    {

      if (!(number = atoi (&name[1])))
	return NULL;

      for (obj = list; number && obj; obj = obj->next_content)
	if (IS_OBJ_VIS (ch, obj) && !(--number))
	  return obj;

      return NULL;
    }

  for (obj = list; obj && (j <= number); obj = obj->next_content)
    {
      if (isname (tmp, obj->name)
	  || (GET_ITEM_TYPE (obj) == ITEM_BOOK && obj->book_title
	      && isname (tmp, obj->book_title)))
	if (IS_OBJ_VIS (ch, obj))
	  {
	    if (j == number)
	      return obj;
	    j++;
	  }
    }

  return 0;
}

/* search the entire world for an object, and return a pointer  */
OBJ_DATA *
get_obj_vis (CHAR_DATA * ch, char *name)
{
  OBJ_DATA *i;
  int j, number;
  char tmpname[MAX_INPUT_LENGTH];
  char *tmp;

  /* scan items carried */
  if ((i = get_obj_in_list_vis (ch, name, ch->right_hand)))
    return (i);

  if ((i = get_obj_in_list_vis (ch, name, ch->left_hand)))
    return (i);

  /* scan room */
  if ((i = get_obj_in_list_vis (ch, name, vtor (ch->in_room)->contents)))
    return (i);

  strcpy (tmpname, name);
  tmp = tmpname;
  if (!(number = get_number (&tmp)))
    return (0);

  /* ok.. no luck yet. scan the entire obj list   */
  for (i = object_list, j = 1; i && (j <= number); i = i->next)
    {

      if (i->deleted)
	continue;

      if (isname (tmp, i->name)
	  || (GET_ITEM_TYPE (i) == ITEM_BOOK && i->book_title
	      && isname (tmp, i->book_title)))
	if (CAN_SEE_OBJ (ch, i))
	  {
	    if (j == number)
	      return (i);
	    j++;
	  }
    }
  return (0);
}

void
name_money (OBJ_DATA * obj)
{
  static char *farthing_one = "coin copper money cash brass farthing";
  static char *farthing_some = "coins copper money cash brass farthings";

  static char *farthing_short_1 = "one farthing";
  static char *farthing_short_2 = "two farthings";
  static char *farthing_short_3 = "three farthings";
  static char *farthing_short_4 = "four farthings";
  static char *farthing_short_handful = "a handful of farthings";
  static char *farthing_short_small = "a small pile of farthings";
  static char *farthing_short_pile = "a pile of farthings";
  static char *farthing_short_big = "a big pile of farthings";
  static char *farthing_short_huge = "a huge pile of farthings";
  static char *farthing_short_enormous = "an enormous pile of farthings";

  static char *name_one_penny = "coin silver penny money cash";
  static char *name_some_pennies = "coins silver pennies money cash";

  static char *short_mountain = "a mountain of silver coins";
  static char *short_huge = "a huge pile of silver coins";
  static char *short_big = "a big pile of silver coins";
  static char *short_pile = "a pile of silver coins";
  static char *short_several = "some silver coins";
  static char *short_one_penny = "a silver penny";

  static char *long_mountain = "A mountain of silver coins!";
  static char *long_huge = "A huge pile of silver coins!";
  static char *long_big = "A big pile of silver coins.";
  static char *long_pile = "A pile of silver coins.";
  static char *long_several = "Several silver coins.";
  static char *long_one_penny = "One silver penny.";

  obj->full_description = '\0';

  if (obj->nVirtual == VNUM_FARTHING)
    {

      obj->description = str_dup ("");

      if (obj->count == 1)
	obj->name = str_dup (farthing_one);
      else
	obj->name = str_dup (farthing_some);

      if (obj->count == 1)
	obj->short_description = str_dup (farthing_short_1);

      else if (obj->count == 2)
	obj->short_description = str_dup (farthing_short_2);

      else if (obj->count == 3)
	obj->short_description = str_dup (farthing_short_3);

      else if (obj->count == 4)
	obj->short_description = str_dup (farthing_short_4);

      else if (obj->count > 2501)	/* more than 2500 coins */
	obj->short_description = str_dup (farthing_short_enormous);

      else if (obj->count > 1001)	/* 1001 - 2500 coins */
	obj->short_description = str_dup (farthing_short_huge);

      else if (obj->count > 101)	/* 101 - 1000 coins */
	obj->short_description = str_dup (farthing_short_big);

      else if (obj->count > 51)	/* 51 - 100 coins */
	obj->short_description = str_dup (farthing_short_pile);

      else if (obj->count > 21)	/* 21 - 50 coins */
	obj->short_description = str_dup (farthing_short_small);

      else			/* 5 - 20 coins */
	obj->short_description = str_dup (farthing_short_handful);

      return;
    }

  if (obj->o.od.value[0] == 1)
    {
      obj->name = str_dup (name_one_penny);
      obj->short_description = str_dup (short_one_penny);
      obj->description = str_dup (long_one_penny);
    }

  else if (obj->o.od.value[0] >= 100000)
    {
      obj->name = str_dup (name_some_pennies);
      obj->short_description = str_dup (short_mountain);
      obj->description = str_dup (long_mountain);
    }

  else if (obj->o.od.value[0] >= 10000)
    {
      obj->name = str_dup (name_some_pennies);
      obj->short_description = str_dup (short_huge);
      obj->description = str_dup (long_huge);
    }

  else if (obj->o.od.value[0] >= 1000)
    {
      obj->name = str_dup (name_some_pennies);
      obj->short_description = str_dup (short_big);
      obj->description = str_dup (long_big);
    }

  else if (obj->o.od.value[0] >= 100)
    {
      obj->name = str_dup (name_some_pennies);
      obj->short_description = str_dup (short_pile);
      obj->description = str_dup (long_pile);
    }

  else
    {
      obj->name = str_dup (name_some_pennies);
      obj->short_description = str_dup (short_several);
      obj->description = str_dup (long_several);
    }
}


OBJ_DATA *
create_money (int amount)
{
  OBJ_DATA *obj;

  if (amount <= 0)
    {
      system_log ("ERROR: Try to create 0 or negative money.", true);
      abort ();
    }

  obj = load_object (VNUM_PENNY);

  obj->count = amount;

  name_money (obj);

  return (obj);
}



/* Generic Find, designed to find any object/character                    */
/* Calling :                                                              */
/*  *arg     is the sting containing the string to be searched for.       */
/*           This string doesn't have to be a single word, the routine    */
/*           extracts the next word itself.                               */
/*  bitv..   All those bits that you want to "search through".            */
/*           Bit found will be result of the function                     */
/*  *ch      This is the person that is trying to "find"                  */
/*  **tar_ch Will be NULL if no character was found, otherwise points     */
/* **tar_obj Will be NULL if no object was found, otherwise points        */
/*                                                                        */
/* The routine returns a pointer to the next word in *arg (just like the  */
/* one_argument routine).                                                 */

int
generic_find (char *arg, int bitvector, CHAR_DATA * ch,
	      CHAR_DATA ** tar_ch, OBJ_DATA ** tar_obj)
{
  static char *ignore[] = {
    "the",
    "in",
    "on",
    "at",
    "\n"
  };

  int i;
  char name[256];
  bool found;

  found = false;


  /* Eliminate spaces and "ignore" words */
  while (*arg && !found)
    {

      for (; *arg == ' '; arg++);

      for (i = 0; (name[i] = *(arg + i)) && (name[i] != ' '); i++);
      name[i] = 0;
      arg += i;
      if (search_block (name, ignore, true) > -1)
	found = true;

    }

  if (!name[0])
    return (0);

  *tar_ch = 0;
  *tar_obj = 0;

  if (IS_SET (bitvector, FIND_CHAR_ROOM))
    {				/* Find person in room */
      if ((*tar_ch = get_char_room_vis (ch, name)))
	{
	  return (FIND_CHAR_ROOM);
	}
    }

  if (IS_SET (bitvector, FIND_CHAR_WORLD))
    {
      if ((*tar_ch = get_char_vis (ch, name)))
	{
	  return (FIND_CHAR_WORLD);
	}
    }

  if (IS_SET (bitvector, FIND_OBJ_EQUIP))
    {
      for (found = false, i = 0; i < MAX_WEAR && !found; i++)
	if (get_equip (ch, i) && !str_cmp (name, get_equip (ch, i)->name))
	  {
	    *tar_obj = get_equip (ch, i);
	    found = true;
	  }
      if (found)
	{
	  return (FIND_OBJ_EQUIP);
	}
    }

  if (IS_SET (bitvector, FIND_OBJ_INV))
    {
      if ((*tar_obj = get_obj_in_list (name, ch->right_hand)))
	{
	  return (FIND_OBJ_INV);
	}

      if ((*tar_obj = get_obj_in_list (name, ch->left_hand)))
	{
	  return (FIND_OBJ_INV);
	}

      if ((*tar_obj = get_obj_in_list (name, ch->equip)))
	{
	  return (FIND_OBJ_INV);
	}
    }

  if (IS_SET (bitvector, FIND_OBJ_ROOM))
    {
      if ((*tar_obj =
	   get_obj_in_list_vis (ch, name, vtor (ch->in_room)->contents)))
	{
	  return (FIND_OBJ_ROOM);
	}
    }

  if (IS_SET (bitvector, FIND_OBJ_WORLD))
    {
      if ((*tar_obj = get_obj_vis (ch, name)))
	{
	  return (FIND_OBJ_WORLD);
	}
    }

  return (0);
}

/* Return true if obj with vnum is equipt */

bool
get_obj_in_equip_num (CHAR_DATA * ch, long vnum)
{
  OBJ_DATA *eq;

  for (eq = ch->equip; eq; eq = eq->next_content)
    if (eq->nVirtual == vnum)
      return 1;

  return 0;
}

void
update_delays (void)
{
  AFFECTED_TYPE *af;
  CHAR_DATA *ch, *ch_next;

  arena__update_delays ();
  te_pit_update_delays ();

  for (ch = character_list; ch; ch = ch_next)
    {

      ch_next = ch->next;

      if (!ch || ch->deleted || !ch->room || ch->in_room == NOWHERE)
	continue;

      if (IS_MORTAL (ch) && (af = get_affect (ch, AFFECT_HOLDING_BREATH)))
	{
	  if (--af->a.spell.duration <= 0)
	    {
	      if (drowned (ch))
		continue;
	    }
	}

      if ((af = get_affect (ch, AFFECT_GROUP_RETREAT)))
	{
	  if (--af->a.spell.duration <= 0)
	    {

	      if (ch->fighting)
		{
		  stop_fighting (ch);
		}

              if(GET_POS(ch) > SIT)
	      {
	        char buf[AVG_STRING_LENGTH] = "";
	        sprintf (buf, "#3You retreat %sward!#0\n", dirs[af->a.spell.sn]);
	        send_to_char (buf, ch);

	        sprintf (buf, "$n retreats %sward.", dirs[af->a.spell.sn]);
	        act (buf, false, ch, 0, 0, TO_ROOM);
	        do_move (ch, "", af->a.spell.sn);
              }

	      remove_affect_type (ch, AFFECT_GROUP_RETREAT);
	    }
	}

      if (ch->stun > 0)
	{
	  ch->stun--;
	  if (ch->stun <= 0)
	    {
	      act ("You shake your head, recovering from the stun.", false,
		   ch, 0, 0, TO_CHAR | _ACT_FORMAT);
	      act ("$n shakes $s head, seeming to recover.", false, ch, 0, 0,
		   TO_ROOM | _ACT_FORMAT);
	    }
	}
      if (ch->roundtime)
	{
	  ch->roundtime--;
	  if (ch->roundtime <= 0)
	    {
	      act ("You can perform another action, now.", false, ch, 0, 0,
		   TO_CHAR | _ACT_FORMAT);
	    }
	}
      if (ch->balance < 0)
	{
	  ch->balance++;
	  if (ch->balance == 0)
	    {
	      send_to_char
		("You feel as if you have fully regained your balance.\n",
		 ch);
	    }
	}
      if (ch->aiming_at && ch->aim < 15)
	{
	  ch->aim++;
	  if ((ch->in_room == ch->aiming_at->in_room && ch->aim >= 5
	       && IS_NPC (ch)) || ch->aim == 15)
	    {
	      send_to_char
		("You feel as if you have the best aim you're able to take.\n",
		 ch);
	      if (IS_NPC (ch) && !ch->desc && !IS_FROZEN (ch->room->zone))
		{
		  if (CAN_SEE (ch, ch->aiming_at) &&
		      (IS_SET (ch->act, ACT_AGGRESSIVE)
		       || !IS_SUBDUEE (ch->aiming_at)))
		    {
		      do_fire (ch, "", 0);
		    }
		  else
		    {
		      ch->aiming_at = NULL;
		      ch->aim = 0;
		    }
		}
	    }
	}
      if (ch->whirling && ch->whirling < 4)
	{
	  ch->whirling++;
	  if (ch->whirling == 4)
	    {
	      send_to_char
		("You've got the sling whirling about as fast as you can manage.\n",
		 ch);
	    }
	}

      if (ch->deleted || !ch->delay || --ch->delay)
	continue;

      switch (ch->delay_type)
	{
	case DEL_PITCH:
	  delayed_pitch (ch);
	  break;
	case DEL_OOC:
	  delayed_ooc (ch);
	  break;
	case DEL_FORAGE:
	  delayed_forage (ch);
	  break;
	case DEL_STRING_BOW:
	  delayed_string (ch);
	  break;
	case DEL_DESTRING_BOW:
	  delayed_destring (ch);
	  break;
        case DEL_RESTRING_BOW:
          delayed_restring (ch);
          break;
	case DEL_SCOUT:
	  delayed_scout (ch);
	  break;
        case DEL_FORM:
          delayed_form (ch);
          break;
	case DEL_LOG1:
	  delayed_log1 (ch);
	  break;
	case DEL_LOG2:
	  delayed_log2 (ch);
	  break;
	case DEL_LOG3:
	  delayed_log3 (ch);
	  break;
	case DEL_RUMMAGE:
	  delayed_rummage (ch);
	  break;
	case DEL_QUAFF:
	  delayed_quaff (ch);
	  break;
	case DEL_APP_APPROVE:
	  ch->delay += 10;
	  break;
	case DEL_CAST:
	  delayed_cast (ch);
	  break;
/*Fyren start */
/*??			case DEL_SKIN:			delayed_skin (ch);				break; */
	case DEL_SKIN_1:
	  delayed_skin_new1 (ch);
	  break;
	case DEL_SKIN_2:
	  delayed_skin_new2 (ch);
	  break;
	case DEL_SKIN_3:
	  delayed_skin_new3 (ch);
	  break;
/*Fyren end */
	case DEL_BUTC_1:
	  delayed_butcher1 (ch);
	  break;
	case DEL_BUTC_2:
	  delayed_butcher2 (ch);
	  break;
	case DEL_BUTC_3:
	  delayed_butcher3 (ch);
	  break;
	case DEL_COUNT_COIN:
	  delayed_count_coin (ch);
	  break;
	case DEL_IDENTIFY:
	  delayed_identify (ch);
	  break;
	case DEL_GATHER:
	  delayed_gather (ch);
	  break;
	case DEL_COMBINE:
	  break;
	case DEL_WHAP:
	  delayed_whap (ch);
	  break;
	case DEL_WATER_REMOVE:
	  delayed_remove (ch);
	  break;
	case DEL_GET_ALL:
	  delayed_get (ch);
	  break;
	case DEL_EMPATHIC_HEAL:
	  delayed_heal (ch);
	  break;
	case DEL_MENTAL_BOLT:
	  delayed_bolt (ch);
	  break;
	case DEL_SEARCH:
	  delayed_search (ch);
	  break;
	case DEL_PICK:
	  delayed_pick (ch);
	  break;
	case DEL_ALERT:
	  delayed_alert (ch);
	  break;
	case DEL_INVITE:
	  break_delay (ch);
	  break;
	case DEL_CAMP1:
	  delayed_camp1 (ch);
	  break;
	case DEL_CAMP2:
	  delayed_camp2 (ch);
	  break;
	case DEL_CAMP3:
	  delayed_camp3 (ch);
	  break;
	case DEL_CAMP4:
	  delayed_camp4 (ch);
	  break;
	case DEL_COVER:
	  delayed_cover (ch);
	  break;
	case DEL_TAKE:
	  delayed_take (ch);
	  break;
	case DEL_PUTCHAR:
	  delayed_putchar (ch);
	  break;
	case DEL_STARE:
	  delayed_study (ch);
	  break;
	case DEL_HIDE:
	  delayed_hide (ch);
	  break;
	case DEL_HIDE_OBJ:
	  delayed_hide_obj (ch);
	  break;
	case DEL_PICK_OBJ:
	  delayed_pick_obj (ch);
	  break;
	case DEL_BIND_WOUNDS:
	  delayed_bind (ch);
	  break;
	case DEL_TREAT_WOUND:
	  delayed_treatment (ch);
	  break;
	case DEL_LOAD_WEAPON:
	  delayed_load (ch);
	  break;
	case DEL_TRACK:
	  delayed_track (ch);
	  break;
	case DEL_POISON_ITEM:
	  delayed_poison (ch);
	  break;
	}
    }
}

void
break_delay (CHAR_DATA * ch)
{

  OBJ_DATA *obj;

  if (ch->aim)
    {
      send_to_char ("You cease aiming your weapon.\n", ch);
      ch->aim = 0;
      if (ch->aiming_at)
	{
	  ch->aiming_at->targeted_by = NULL;
	  ch->aiming_at = NULL;
	}
      return;
    }

  if (ch->whirling)
    {
      send_to_char ("You cease whirling your sling.\n", ch);
      ch->whirling = 0;
      return;
    }

  switch (ch->delay_type)
    {

    case DEL_LOAD_WEAPON:
      send_to_char ("You cease loading your weapon.\n", ch);
      ch->delay = 0;
      break;

    case DEL_WATER_REMOVE:
      send_to_char ("You cease attempting to remove that item.\n", ch);
      ch->delay = 0;
      break;

    case DEL_BIND_WOUNDS:
      ch->flags &= ~FLAG_BINDING;
      ch->delay = 0;
      send_to_char ("You cease your ministrations.\n", ch);
      break;

    case DEL_TRACK:
      ch->delay = 0;
      send_to_char ("You discontinue your search for tracks.\n", ch);
      break;

    case DEL_TREAT_WOUND:
      ch->delay = 0;
      send_to_char ("You cease your ministrations.\n", ch);
      break;

    case DEL_BACKSTAB:
      ch->delay = 0;
      send_to_char ("You abort your backstab attempt.\n\r", ch);
      break;

    case DEL_PURCHASE_ITEM:
    case DEL_ORDER_ITEM:
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay_ch = NULL;
      send_to_char ("You decide against making the purchase.\n\r", ch);
      break;

    case DEL_CANCEL_AUCTION:
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay_ch = NULL;
      send_to_char ("You decide against cancelling the auction.\n\r", ch);
      break;
	  
    case DEL_PLACE_AUCTION:
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay_ch = NULL;
      send_to_char ("You decide against placing the auction.\n\r", ch);
      break;
	  
	case DEL_PLACE_BID:
	case DEL_PLACE_BUYOUT:
	  ch->delay_type = 0;
	  ch->delay_ch = NULL;
	  ch->delay_info1 = 0;
	  ch->delay_info2 = 0;
	  send_to_char ("You decide against placing the bid.\n\r", ch);
	  break;
	  
    case DEL_PITCH:
      ch->delay = 0;
      ch->delay_obj = NULL;
      send_to_char ("You cease pitching the tent.\n\r", ch);
      break;

    case DEL_OOC:
      ch->delay = 0;
      send_to_char ("You decide against going to the OOC lounge.\n\r", ch);
      break;

    case DEL_STRING_BOW:
      ch->delay = 0;
      ch->delay_info1 = 0;
      send_to_char("You stop stringing your bow.\n\r", ch);
      break;

    case DEL_DESTRING_BOW:
      ch->delay = 0;
      ch->delay_info1 = 0;
      send_to_char("You stop destringing your bow.\n\r", ch);
      break;

    case DEL_FORAGE:
      ch->delay = 0;
      send_to_char ("You stop foraging.\n\r", ch);
      break;

    case DEL_RESTRING_BOW:
      ch->delay = 0;
      ch->delay_info1 = 0;
      send_to_char("You stop restringing your bow.\n\r", ch);
      break;

    case DEL_FORM:
      ch->delay = 0;
      send_to_char ("You stop ordering your followers in to formation.\n", ch);
      act ("$n halts ordering $s followers in to formation.\n", false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      break;

    case DEL_SCOUT:
      ch->delay = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      send_to_char ("You stop scouting a tree.\n\r", ch);
      break;

    case DEL_LOG1:
    case DEL_LOG2:
    case DEL_LOG3:
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      send_to_char ("You stop chopping down the tree.\n\r", ch);
      act ("$n abruptly stops chopping down the tree.",
	   false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      break;

    case DEL_WEAVESIGHT:
      ch->delay = 0;
      send_to_char
	("You blink and shake your heard, ceasing your concentration.\n\r",
	 ch);
      break;

    case DEL_RUMMAGE:
      ch->delay = 0;
      send_to_char ("You stop rummaging.\n\r", ch);
      break;

    case DEL_APP_APPROVE:
      ch->delay = 0;
      send_to_char ("You pass on this application.\n\r", ch);
      if (ch->pc->msg)
	{
	  send_to_char ("Your prepared message was deleted.\n\r", ch);
	  mem_free (ch->pc->msg);
	  ch->pc->msg = NULL;
	}
      break;

    case DEL_SKIN_1:
    case DEL_SKIN_2:
    case DEL_SKIN_3:
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      act ("You abruptly end your skinning.", true, ch, 0, 0, TO_CHAR);
      act ("$n abruptly stops skinning.",
	   false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      break;

    case DEL_BUTC_1:
    case DEL_BUTC_2:
    case DEL_BUTC_3:
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      act ("You abruptly stop butchering.", true, ch, 0, 0, TO_CHAR);
      act ("$n abruptly stops butchering.",
	   false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      break;

    case DEL_COUNT_COIN:
      stop_counting (ch);
      break;

    case DEL_IDENTIFY:
      ch->delay = 0;
      if (ch->delay_who)
	{
	  mem_free (ch->delay_who);
	  ch->delay_who = NULL;
	}
      act ("You stop trying to identify the flora.", true, ch, 0, 0, TO_CHAR);
      break;

    case DEL_GATHER:
      ch->delay = 0;
      if (ch->delay_who)
	{
	  mem_free (ch->delay_who);
	  ch->delay_who = NULL;
	}
      act ("You stop gathering.", true, ch, 0, 0, TO_CHAR);
      break;

    case DEL_COMBINE:
      ch->delay_who = 0;
      ch->delay = 0;
      break;

    case DEL_GET_ALL:
      get_break_delay (ch);
      break;

    case DEL_EMPATHIC_HEAL:
      ch->delay = 0;
      ch->delay_ch = NULL;
      act ("You stop your healing concentration.", false, ch, 0, 0, TO_CHAR);
      break;

    case DEL_MENTAL_BOLT:
      ch->delay = 0;
      ch->delay_ch = NULL;
      act ("You stop your mental attack.", false, ch, 0, 0, TO_CHAR);
      break;

    case DEL_SEARCH:
      ch->delay = 0;
      act ("You stop searching.", false, ch, 0, 0, TO_CHAR);
      act ("$n stops searching.", true, ch, 0, 0, TO_ROOM);
      break;

    case DEL_PICK:
      ch->delay = 0;
      act ("You stop trying to pick the lock.", false, ch, 0, 0, TO_CHAR);
      break;

    case DEL_ALERT:
      ch->delay = 0;
      act ("You forget about responding to the alert.", false, ch, 0, 0,
	   TO_CHAR);
      break;

    case DEL_INVITE:
      ch->delay = 0;
      send_to_char ("You decline to join.\n", ch);
      if (is_he_here (ch, ch->delay_ch, 1))
	act ("$N declines your offer.", false, ch->delay_ch, 0, ch, TO_CHAR);
      break;

    case DEL_CAMP1:
    case DEL_CAMP2:
    case DEL_CAMP3:
    case DEL_CAMP4:
      ch->delay = 0;
      send_to_char ("You stop building your camp.\n", ch);
      act ("$n stops building $s camp.", true, ch, 0, 0, TO_ROOM);
      break;

    case DEL_TAKE:
      ch->delay = 0;
      send_to_char ("You stop trying to remove the object.\n", ch);
      act ("$n stops trying to get the object from the body.",
	   false, ch, 0, 0, TO_ROOM);
      break;

    case DEL_PUTCHAR:
      ch->delay = 0;
      send_to_char ("You stop doing what you're doing.\n", ch);
      act ("$n 'stops doing what $e's doing.", false, ch, 0, 0, TO_ROOM);
      break;

    case DEL_STARE:
      ch->delay = 0;
      break;

    case DEL_HIDE:
      ch->delay = 0;
      send_to_char ("You stop trying to hide.\n", ch);
      break;

    case DEL_SCAN:
      ch->delay = 0;
      break;

    case DEL_QUICK_SCAN:
      ch->delay = 0;
      break;

    case DEL_HIDE_OBJ:
      ch->delay = 0;
      break;

    case DEL_PICK_OBJ:
      ch->delay = 0;
      break;

    case DEL_POISON_ITEM:
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay_info3 = 0;
      send_to_char("You stop your poisoning.\n", ch);
      act ("$n stops $s nefarious poisoning.", false, ch, 0, 0, TO_ROOM);
      break;

    case DEL_QUAFF:
      ch->delay = 0;
      ch->delay_type = 0;
      obj = (OBJ_DATA *) ch->delay_info1;
      act ("You stop ingesting $p.", true, ch, obj, 0, TO_CHAR);
      act ("$n stops ingesting $p.", false, ch, obj, 0, TO_ROOM);
      ch->delay_info1 = 0;
    break;
      
     case DEL_COVER:
      ch->delay = 0;
      send_to_char ("You stop trying to take cover.\n", ch);
      act ("$n stops taking cover.", false, ch, 0, 0, TO_ROOM);
      break;  
    }

  ch->delay_type = 0;
}

AFFECTED_TYPE *
is_room_affected (AFFECTED_TYPE * af, int type)
{
  while (af)
    {
      if (af->type == type)
	return af;

      af = af->next;
    }

  return NULL;
}

void
add_room_affect (AFFECTED_TYPE ** af, int type, int duration)
{
  AFFECTED_TYPE *raffect;

  if ((raffect = is_room_affected (*af, type)))
    {
      raffect->a.room.duration += duration;
      return;
    }

  raffect = (AFFECTED_TYPE *) alloc (sizeof (AFFECTED_TYPE), 13);

  raffect->type = type;
  raffect->a.room.duration = duration;
  raffect->next = *af;

  *af = raffect;
}

int
is_in_room (CHAR_DATA * ch, CHAR_DATA * target)
{
  CHAR_DATA *tch;

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (tch == target)
	return 1;
    }

  return 0;
}

int
add_registry (int reg_index, int value, const char *string)
{
  REGISTRY_DATA *new_reg;
  static int init = 0;

  for (; init < MAX_REGISTRY; init++)	/* Only executes once in */
    registry[init] = NULL;	/* the life of the game  */

  if (value == -1)
    {
      abort ();
    }

  CREATE (new_reg, REGISTRY_DATA, 1);

  new_reg->string = str_dup (string);
  new_reg->value = value;
  new_reg->next = registry[reg_index];
  registry[reg_index] = new_reg;

  return 0;
}

void
free_registry (void)
{
  int i = 0;

  while (registry[i])
    i++;
  i--;
  while (i)
    {
      mem_free (registry[i]->string);
      mem_free (registry[i]);
      i--;
    }

  mem_free (registry[0]->string);
  mem_free (registry[0]);
}

int
lookup_value (char *string, int reg_index)
{
  REGISTRY_DATA *reg;

  if (!string)
    return -1;

  for (reg = registry[reg_index]; reg; reg = reg->next)
    if (!str_cmp (string, reg->string))
      return reg->value;

  return -1;
}

char *
lookup_string (int value, int reg_index)
{
  REGISTRY_DATA *reg;

  for (reg = registry[reg_index]; reg; reg = reg->next)
    if (value == reg->value)
      return reg->string;

  return NULL;
}

void
reg_additional_spell_info (int sn, char *buf)
{
  char buf2[MAX_STRING_LENGTH];
  char *argument;

  argument = one_argument (buf, buf2);

  while (*buf2)
    {

      if (!str_cmp (buf2, "duration"))
	{
	  argument = one_argument (argument, buf2);
	  add_registry (REG_DURATION, atoi (buf2), spell_table[sn].name);
	}

      argument = one_argument (argument, buf2);
    }
}

void
reg_read_magic (FILE * fp_reg, char *buf)
{
  char buf2[MAX_STRING_LENGTH];
  char spell_name[MAX_STRING_LENGTH];
  char *argument;
  int sn = 0;

  const char *target_types[] = {
    "area", "offense", "defense", "self", "obj", "objinv",
    "secret", "ignore", "\n"
  };

  while (!feof (fp_reg))
    {

      fgets (buf, MAX_STRING_LENGTH, fp_reg);

      if (*buf && buf[strlen (buf) - 1] == '\n')
	buf[strlen (buf) - 1] = '\0';

      if (!*buf || *buf == '#')
	continue;

      if (*buf == '+')
	{
	  reg_additional_spell_info (sn, buf);
	  continue;
	}

      argument = one_argument (buf, spell_name);

      if (!str_cmp (spell_name, "[end]"))
	return;

      if ((sn = spell_lookup (spell_name)) == -1)
	{
	  printf ("Spell '%s' does not exist.\n", spell_name);
	  abort ();
	}

      argument = one_argument (argument, buf2);

      if ((spell_table[sn].target = index_lookup (target_types, buf2)) == -1)
	{
	  printf ("Spell '%s' has illegal target: %s\n", spell_name, buf2);
	  abort ();
	}

      argument = one_argument (argument, buf2);

      if (!*buf2)
	continue;

      spell_table[sn].piety_cost = atoi (buf2);

      argument = one_argument (argument, buf2);

      if (!*buf2)
	continue;

      spell_table[sn].delay = atoi (buf2);

      argument = one_argument (argument, buf2);

      if (!*buf2)
	continue;

      spell_table[sn].msg_off = str_dup (buf2);
    }
}

void
reg_read_ov_lv_cap (FILE * fp_reg, char *buf)
{
  int sn;
  char *argument;
  char skill_name[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  while (!feof (fp_reg))
    {

      fgets (buf, MAX_STRING_LENGTH, fp_reg);

      if (*buf && buf[strlen (buf) - 1] == '\n')
	buf[strlen (buf) - 1] = '\0';

      if (!*buf || *buf == '#')
	continue;

      argument = one_argument (buf, skill_name);

      if (!str_cmp (skill_name, "[end]"))
	return;

      if ((sn = index_lookup (skills, skill_name)) == -1)
	{
	  printf ("Unknown skill name in registry file: %s\n", skill_name);
	  abort ();
	}

      argument = one_argument (argument, buf2);

      add_registry (REG_OV, sn, buf2);

      argument = one_argument (argument, buf2);

      add_registry (REG_LV, sn, buf2);

      argument = one_argument (argument, buf2);

      add_registry (REG_CAP, sn, buf2);
    }
}

void
reg_read_skill_max_rates (FILE * fp_reg, char *buf)
{
  int sn;
  char *argument;
  char buf2[MAX_STRING_LENGTH];
  char skill_name[MAX_STRING_LENGTH];

  while (!feof (fp_reg))
    {

      fgets (buf, MAX_STRING_LENGTH, fp_reg);

      if (*buf && buf[strlen (buf) - 1] == '\n')
	buf[strlen (buf) - 1] = '\0';

      if (!*buf || *buf == '#')
	continue;

      argument = one_argument (buf, skill_name);

      if (!str_cmp (buf, "[end]"))
	return;

      if ((sn = index_lookup (skills, skill_name)) == -1)
	{
	  printf ("Unknown skill name in registry file: %s\n", skill_name);
	  abort ();
	}

      argument = one_argument (argument, buf2);

      if (!isdigit (*buf2))
	{
	  printf ("Invalid hourly rate for skill %s\n", skill_name);
	  abort ();
	}

      add_registry (REG_MAX_RATES, sn, buf2);
    }
}

void
reg_read_variables (FILE * fp_reg, char *buf)
{
  char buf2[MAX_STRING_LENGTH];
  char buf3[MAX_STRING_LENGTH];
  char *argument;
  int value;

  while (!feof (fp_reg))
    {

      fgets (buf, MAX_STRING_LENGTH, fp_reg);

      if (*buf && buf[strlen (buf) - 1] == '\n')
	buf[strlen (buf) - 1] = '\0';

      if (!*buf || *buf == '#')
	continue;

      argument = one_argument (buf, buf2);

      if (!str_cmp (buf2, "[end]"))
	return;

      argument = one_argument (argument, buf3);

      value = atoi (buf3);

      if (!value)
	{
	  system_log ("ILLEGAL VARIABLE VALUE FOR VARIABLE:", true);
	  system_log (buf2, true);
	  system_log (buf3, true);
	  abort ();
	}

      if (!str_cmp (buf2, "pulse_violence"))
	pulse_violence = value;
      else
	{
	  system_log ("UNKNOWN VARIABLE:", true);
	  system_log (buf2, true);
	  abort ();
	}
    }
}

void
insert_combat_msg (COMBAT_MSG_DATA * cm)
{
  COMBAT_MSG_DATA *tcm;
  COMBAT_MSG_DATA *prev = NULL;

  if (!cm_list)
    {
      cm_list = cm;
      return;
    }

  if (cm->off_result <= cm_list->off_result)
    {
      cm->next = cm_list;
      cm_list = cm;
      return;
    }

  for (tcm = cm_list; tcm; tcm = tcm->next)
    {

      if (cm->off_result <= tcm->off_result)
	break;

      prev = tcm;
    }

  if (!tcm)
    prev->next = cm;
  else
    {
      cm->next = prev->next;
      prev->next = cm;
    }
}

void
add_combat_message (char *line)
{
  int r1;
  int r2;
  char *argument;
  char buf[MAX_STRING_LENGTH];

  static COMBAT_MSG_DATA *cm;
  static int party = 0;

  if (!party)
    {
      argument = one_argument (line, buf);

      if ((r1 = index_lookup (rs_name, buf)) == -1)
	{
	  system_log ("Illegal 1st result in registry, line:", true);
	  system_log (line, true);
	  abort ();
	}

      argument = one_argument (argument, buf);

      if ((r2 = index_lookup (rs_name, buf)) == -1)
	{
	  system_log ("Illegal 2nd result in registry, line:", true);
	  system_log (line, true);
	  abort ();
	}

      argument = one_argument (argument, buf);

      CREATE (cm, COMBAT_MSG_DATA, 1);

      *buf = toupper (*buf);

      if (*buf == 'I' || *buf == 'P' || *buf == 'B' ||
	  *buf == 'D' || *buf == 'F' || *buf == '*')
	cm->table = *buf;
      else
	{
	  system_log ("Illegal table name:  table, off-result:", true);
	  system_log (buf, true);
	  system_log (rs_name[r1], true);
	  abort ();
	}

      cm->off_result = r1;
      cm->def_result = r2;
      cm->next = NULL;

    }
  else
    argument = line;

  while (isspace (*argument))
    argument++;

  if (party == 0)
    cm->def_msg = str_dup (argument);
  else if (party == 1)
    cm->off_msg = str_dup (argument);
  else
    cm->other_msg = str_dup (argument);

  if (party == 0)
    insert_combat_msg (cm);

  party++;

  if (party > 2)
    party = 0;
}


#define TABLE_USE_TABLE 		1
#define TABLE_COMBAT_MESSAGE	2

void
reg_read_tables (FILE * fp_reg, char *buf)
{
  int table_id = 0;
  int sn;
  char skill_name[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char *argument;

  while (!feof (fp_reg))
    {

      fgets (buf, MAX_STRING_LENGTH, fp_reg);

      if (*buf && buf[strlen (buf) - 1] == '\n')
	buf[strlen (buf) - 1] = '\0';

      if (!*buf || *buf == '#')
	continue;

      argument = one_argument (buf, buf2);

      if (!str_cmp (buf2, "[end]"))
	return;

      if (!strn_cmp (buf2, "end-", 4) && table_id)
	{
	  table_id = 0;
	  continue;
	}

      if (!table_id)
	{
	  if (!str_cmp (buf2, "begin-use-table"))
	    table_id = TABLE_USE_TABLE;
	  else if (!str_cmp (buf2, "begin-combat-message-table"))
	    table_id = TABLE_COMBAT_MESSAGE;
	  else
	    {
	      system_log ("UNKNOWN TABLE NAME:", true);
	      system_log (buf2, true);
	      abort ();
	    }

	  continue;
	}

      if (table_id == TABLE_USE_TABLE)
	{
	  if ((sn = index_lookup (skills, buf2)) == -1)
	    {
	      printf ("Unknown skill in USE-TABLE: %s\n", skill_name);
	      abort ();
	    }

	  argument = one_argument (argument, buf2);

	  if (!atoi (buf2))
	    {
	      system_log ("ILLEGAL/NO VALUE IN REGISTRY, USE-TABLE, SKILL:",
			  true);
	      system_log (skills[sn], true);
	      abort ();
	    }

	  use_table[sn].delay = atoi (buf2);
	}

      else if (table_id == TABLE_COMBAT_MESSAGE)
	{
	  add_combat_message (buf);
	}
    }
}

void
read_spell (PHASE_DATA * phase, char *argument)
{
  int spell_type;
  int ind;
  int duration = 0;
  int power = 0;
  int target_flags = 0;
  char buf[MAX_STRING_LENGTH];

  if (phase->spell_type)
    {
      system_log ("TWO SPELL DEFINED IN ONE PHASE (Second one ignored):",
		  true);
      system_log (argument, true);
      return;
    }

  argument = one_argument (argument, buf);

  if ((spell_type = lookup_value (buf, REG_CRAFT_MAGIC)) == -1)
    {
      system_log ("ERROR ON SPELL LINE:", true);
      system_log (buf, true);
      system_log (argument, true);
      return;
    }

  while (*(argument = one_argument (argument, buf)))
    {

      if (!str_cmp (buf, "duration"))
	{
	  argument = one_argument (argument, buf);
	  duration = atoi (buf);
	  if (!duration)
	    {
	      system_log ("DURATION PROBLEM IN CRAFT USING SPELL:", true);
	      system_log (lookup_string (spell_type, REG_CRAFT_MAGIC), true);
	      return;
	    }
	}

      else if (!str_cmp (buf, "power"))
	{
	  argument = one_argument (argument, buf);
	  if (!isdigit (*buf))
	    {
	      system_log
		("POWER HAS A PROBLEM IN CRAFT DEFINITION FOR SPELL:", true);
	      system_log (lookup_string (spell_type, REG_CRAFT_MAGIC), true);
	      return;
	    }

	  power = atoi (buf);
	}

      else if ((ind = index_lookup (targets, buf)) != -1)
	target_flags |= 1 << ind;

      else
	{
	  system_log ("UNKNOWN KEYWORD IN SPELL DEFINITION FOR SPELL:", true);
	  system_log (lookup_string (spell_type, REG_CRAFT_MAGIC), true);
	  system_log (buf, true);
	}
    }

  phase->spell_type = spell_type;
  phase->duration = duration;
  phase->targets = target_flags;
  phase->power = power;
}

void
read_registry ()
{
  FILE *fp_reg;
  char buf[MAX_STRING_LENGTH];

  if (!(fp_reg = fopen (REGISTRY_FILE, "r")))
    {
      perror ("Unable to open registry!");
      abort ();
    }

  while (!feof (fp_reg))
    {

      fgets (buf, MAX_STRING_LENGTH, fp_reg);

      if (*buf && buf[strlen (buf) - 1] == '\n')
	buf[strlen (buf) - 1] = '\0';

      if (!*buf || *buf == '#')
	continue;

      if (!str_cmp (buf, "[END]"))
	break;

      else if (!str_cmp (buf, "[MAGIC]"))
	reg_read_magic (fp_reg, buf);

      else if (!str_cmp (buf, "[OV-LV-CAP]"))
	reg_read_ov_lv_cap (fp_reg, buf);

      else if (!str_cmp (buf, "[VARIABLES]"))
	reg_read_variables (fp_reg, buf);

      else if (!str_cmp (buf, "[TABLES]"))
	reg_read_tables (fp_reg, buf);

      else if (!str_cmp (buf, "[SKILL-MAX-RATES]"))
	reg_read_skill_max_rates (fp_reg, buf);

      else if (!str_cmp (buf, "[CRAFTS]"))
	reg_read_crafts (fp_reg, buf);
    }

  fclose (fp_reg);
}

void
setup_registry (void)
{
  int i;

  add_registry (REG_REGISTRY, REG_REGISTRY, "Registry");
  add_registry (REG_REGISTRY, REG_SPELLS, "Spells");
  add_registry (REG_REGISTRY, REG_DURATION, "Duration");
  add_registry (REG_REGISTRY, REG_OV, "OV");
  add_registry (REG_REGISTRY, REG_LV, "LV");
  add_registry (REG_REGISTRY, REG_CAP, "Cap");
  add_registry (REG_REGISTRY, REG_MISC, "Misc");
  add_registry (REG_REGISTRY, REG_MAGIC_SPELLS, "Magic");
  add_registry (REG_REGISTRY, REG_MAX_RATES, "Rates");
  add_registry (REG_REGISTRY, REG_CRAFT_MAGIC, "Craftmagic");

  add_registry (REG_CRAFT_MAGIC, MAGIC_AKLASH_ODOR, "Aklash Odor");
  add_registry (REG_CRAFT_MAGIC, MAGIC_ROSE_SCENT, "Rose Scent");
  add_registry (REG_CRAFT_MAGIC, MAGIC_JASMINE_SCENT, "Jasmine Scent");
  add_registry (REG_CRAFT_MAGIC, MAGIC_SEWER_STENCH, "Sewer Stench");
  add_registry (REG_CRAFT_MAGIC, MAGIC_SOAP_AROMA, "Soap Aroma");

  add_registry (REG_CRAFT_MAGIC, MAGIC_CINNAMON_SCENT, "Cinnamon Scent");
  add_registry (REG_CRAFT_MAGIC, MAGIC_LEORTEVALD_STENCH,
		"Leortevald Stench");
  add_registry (REG_CRAFT_MAGIC, MAGIC_YULPRIS_ODOR, "Yulpris Odor");
  add_registry (REG_CRAFT_MAGIC, MAGIC_FRESH_BREAD, "Fresh Bread");
  add_registry (REG_CRAFT_MAGIC, MAGIC_MOWN_HAY, "Mown Hay");

  add_registry (REG_CRAFT_MAGIC, MAGIC_FRESH_LINEN, "Fresh Linen");
  add_registry (REG_CRAFT_MAGIC, MAGIC_INCENSE_SMOKE, "Incense Smoke");
  add_registry (REG_CRAFT_MAGIC, MAGIC_WOOD_SMOKE, "Wood Smoke");

  /* Add in all skills to craft magic */

  for (i = 1; i < LAST_SKILL; i++)
    add_registry (REG_CRAFT_MAGIC, MAGIC_SKILL_MOD_FIRST + i, skills[i]);

  add_registry (REG_SKILLS, SKILL_SPEAK_AVARIN, "Avarin");
  add_registry (REG_SKILLS, SKILL_APOTHECARY, "Apothecary");
  add_registry (REG_SKILLS, SKILL_ASTRONOMY, "Astronomy");
  add_registry (REG_SKILLS, SKILL_AVERT, "Avert");
  add_registry (REG_SKILLS, SKILL_AXE, "Axe");
  add_registry (REG_SKILLS, SKILL_SENSITIVITY, "Aura-Sight");
  add_registry (REG_SKILLS, SKILL_BARTER, "Barter");
  add_registry (REG_SKILLS, SKILL_SPEAK_BEAST_TONGUE, "Beast-Tongue");
  add_registry (REG_SKILLS, SKILL_BLACK_WISE, "Black-Wise");
  add_registry (REG_SKILLS, SKILL_BRAWLING, "Brawling");
  add_registry (REG_SKILLS, SKILL_BREWING, "Brewing");
  add_registry (REG_SKILLS, SKILL_SCRIPT_CIRTH, "Cirth");
  add_registry (REG_SKILLS, SKILL_CLAIRVOYANCE, "Clairvoyance");
  add_registry (REG_SKILLS, SKILL_CLUB, "Club");
  add_registry (REG_SKILLS, SKILL_COOKERY, "Cookery");
  add_registry (REG_SKILLS, SKILL_DUAL, "Dual-Wield");
  add_registry (REG_SKILLS, SKILL_DANGER_SENSE, "Danger-Sense");

/*	add_registry (REG_SKILLS, SKILL_DEFENSE,		"Defense"); */

  add_registry (REG_SKILLS, SKILL_DODGE, "Dodge");
  add_registry (REG_SKILLS, SKILL_SPEAK_DRUAG, "Druag");
  add_registry (REG_SKILLS, SKILL_EAVESDROP, "Eavesdrop");
  add_registry (REG_SKILLS, SKILL_EMPATHIC_HEAL, "Empathy");
  add_registry (REG_SKILLS, SKILL_FLAIL, "Flail");
  add_registry (REG_SKILLS, SKILL_FORAGE, "Forage");
  add_registry (REG_SKILLS, SKILL_GREY_WISE, "Grey-Wise");
  add_registry (REG_SKILLS, SKILL_SPEAK_HALADIN, "Haladin");
  add_registry (REG_SKILLS, SKILL_HEALING, "Healing");
  add_registry (REG_SKILLS, SKILL_HERBALISM, "Herbalism");
  add_registry (REG_SKILLS, SKILL_HEX, "Hex");
  add_registry (REG_SKILLS, SKILL_HIDE, "Hide");
  add_registry (REG_SKILLS, SKILL_HIDEWORKING, "Hideworking");
  add_registry (REG_SKILLS, SKILL_SPEAK_KHUZDUL, "Khuzdul");
  add_registry (REG_SKILLS, SKILL_LARGE_WEAPON, "Double-Handed");
  add_registry (REG_SKILLS, SKILL_LITERACY, "Literacy");
  add_registry (REG_SKILLS, SKILL_LONGBOW, "Warbow");
  add_registry (REG_SKILLS, SKILL_LUMBERJACK, "Lumberjack");
  add_registry (REG_SKILLS, SKILL_PICK, "Picklock");
  add_registry (REG_SKILLS, SKILL_MENTAL_BOLT, "Psychic-Bolt");
  add_registry (REG_SKILLS, SKILL_METALCRAFT, "Metalcraft");
  add_registry (REG_SKILLS, SKILL_MINING, "Mining");
  add_registry (REG_SKILLS, SKILL_SPEAK_NANDORIN, "Nandorin");

  add_registry (REG_SKILLS, SKILL_SPEAK_ORKISH, "Orkish");

/*	add_registry (REG_SKILLS, SKILL_OFFENSE,		"Offense"); */

  add_registry (REG_SKILLS, SKILL_DEFUNCT, "DEFUNCT");
  add_registry (REG_SKILLS, SKILL_POISONING, "Poisoning");
  add_registry (REG_SKILLS, SKILL_POLEARM, "Polearm");
  add_registry (REG_SKILLS, SKILL_PRESCIENCE, "Prescience");
  add_registry (REG_SKILLS, SKILL_SPEAK_QUENYA, "Quenya");
  add_registry (REG_SKILLS, SKILL_RIDE, "Ride");
  add_registry (REG_SKILLS, SKILL_SCRIPT_SARATI, "Sarati");
  add_registry (REG_SKILLS, SKILL_SHORTBOW, "Hunting-bow");
  add_registry (REG_SKILLS, SKILL_SPEAK_SINDARIN, "Sindarin");
  add_registry (REG_SKILLS, SKILL_SINGLE_WEAPON, "Sole-Wield");
  add_registry (REG_SKILLS, SKILL_SHIELD_STYLE, "Shield-Use");
  add_registry (REG_SKILLS, SKILL_BUTCHERY, "Butchery");
  add_registry (REG_SKILLS, SKILL_SLING, "Sling");
  add_registry (REG_SKILLS, SKILL_SMALL_BLADE, "Small-Blade");
  add_registry (REG_SKILLS, SKILL_SNEAK, "Sneak");
  add_registry (REG_SKILLS, SKILL_STEAL, "Steal");
  add_registry (REG_SKILLS, SKILL_SWORD, "Sword");
  add_registry (REG_SKILLS, SKILL_SPEAK_TALISKA, "Taliska");
  add_registry (REG_SKILLS, SKILL_TELEPATHY, "Telepathy");
  add_registry (REG_SKILLS, SKILL_SCRIPT_TENGWAR, "Tengwar");
  add_registry (REG_SKILLS, SKILL_THROWN, "Throwing");
  add_registry (REG_SKILLS, SKILL_SPEAK_THRUNON, "Thrunon");
  add_registry (REG_SKILLS, SKILL_SPEAK_TROLLISH, "Trollish");
  add_registry (REG_SKILLS, SKILL_TRACKING, "Tracking");
  add_registry (REG_SKILLS, SKILL_SPEAK_VALARIN, "Valarin");
  add_registry (REG_SKILLS, SKILL_SCRIPT_VALARIN_SCRIPT, "Valarin-Script");
  add_registry (REG_SKILLS, SKILL_WHITE_WISE, "White-Wise");
  add_registry (REG_SKILLS, SKILL_WOODCRAFT, "Woodcraft");

  add_registry (REG_MISC_NAMES, MISC_DELAY_OFFSET, "Delayoffset");
  add_registry (REG_MISC_NAMES, MISC_MAX_CARRY_N, "Maxcarry_n");
  add_registry (REG_MISC_NAMES, MISC_MAX_CARRY_W, "Maxcarry_w");
  add_registry (REG_MISC_NAMES, MISC_MAX_MOVE, "Maxmove");
  add_registry (REG_MISC_NAMES, MISC_STEAL_DEFENSE, "Stealdefense");

  add_registry (REG_MISC, MISC_DELAY_OFFSET, "(str + dex) / 6");
  add_registry (REG_MISC, MISC_MAX_CARRY_N, "dex / 3 + 4");
  add_registry (REG_MISC, MISC_MAX_CARRY_W, "str * 2500");
  add_registry (REG_MISC, MISC_MAX_MOVE, "(con + wil) / 2 * 5 + 25");
  add_registry (REG_MISC, MISC_STEAL_DEFENSE, "(int + dex) / 2");

  add_registry (REG_MAGIC_SPELLS, POISON_LETHARGY, "Lethargy");

  add_registry (REG_MAGIC_SPELLS, MAGIC_AKLASH_ODOR, "Aklash Odor");
  add_registry (REG_MAGIC_SPELLS, MAGIC_ROSE_SCENT, "Rose Scent");
  add_registry (REG_MAGIC_SPELLS, MAGIC_JASMINE_SCENT, "Jasmine Scent");
  add_registry (REG_MAGIC_SPELLS, MAGIC_SEWER_STENCH, "Sewer Stench");
  add_registry (REG_MAGIC_SPELLS, MAGIC_SOAP_AROMA, "Soap Aroma");
  add_registry (REG_MAGIC_SPELLS, MAGIC_CINNAMON_SCENT, "Cinnamon Scent");
  add_registry (REG_MAGIC_SPELLS, MAGIC_LEORTEVALD_STENCH,
		"Leortevald Stench");
  add_registry (REG_MAGIC_SPELLS, MAGIC_YULPRIS_ODOR, "Yulpris Odor");
  add_registry (REG_MAGIC_SPELLS, MAGIC_FRESH_BREAD, "Fresh Bread");
  add_registry (REG_MAGIC_SPELLS, MAGIC_MOWN_HAY, "Mown Hay");
  add_registry (REG_MAGIC_SPELLS, MAGIC_FRESH_LINEN, "Fresh Linen");
  add_registry (REG_MAGIC_SPELLS, MAGIC_INCENSE_SMOKE, "Incense Smoke");
  add_registry (REG_MAGIC_SPELLS, MAGIC_WOOD_SMOKE, "Wood Smoke");

  read_registry ();
}

void
load_dynamic_registry (void)
{
  FILE *fp_dr;
  NAME_SWITCH_DATA *last_name = NULL;
  NAME_SWITCH_DATA *name_switch;
  char *argument;
  char token[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];

  if (!(fp_dr = fopen (DYNAMIC_REGISTRY, "r")))
    {
      perror (DYNAMIC_REGISTRY);
      system_log ("Couldn't open DYNAMIC REGISTRY.", true);
      return;
    }

  while (fgets (buf, MAX_STRING_LENGTH - 1, fp_dr))
    {

      if (buf[strlen (buf) - 1] == '\n')
	buf[strlen (buf) - 1] = '\0';

      argument = one_argument (buf, token);

      if (!str_cmp (token, "newclanname"))
	{

	  name_switch =
	    (struct name_switch_data *)
	    alloc (sizeof (struct name_switch_data), 38);

	  argument = one_argument (argument, token);
	  name_switch->old_name = str_dup (token);

	  argument = one_argument (argument, token);
	  name_switch->new_name = str_dup (token);

	  if (!last_name)
	    clan_name_switch_list = name_switch;
	  else
	    last_name->next = name_switch;

	  last_name = name_switch;

	  continue;
	}
      /*
         if ( !str_cmp (token, "clandef") ) {
         add_clandef (argument);
         continue;
         }
       */
    }


  fclose (fp_dr);
  clan__do_load ();
}

void
morph_mob (CHAR_DATA * ch)
{
	char nbuf[MAX_STRING_LENGTH] = {'\0'};
	CHAR_DATA *newMob = NULL;
	OBJ_DATA *nobj;
	int temp_vnum = 0;
	int troom = 0;
	int index = 0;
	int jdex = 0;
	int flag = 0;
	
	if (ch->deleted)
		return;

	if (GET_POS (ch) == POSITION_FIGHTING 
  		|| IS_SUBDUER (ch)
  		|| IS_SUBDUEE (ch)
  		|| IS_HITCHER (ch)
  		|| IS_HITCHEE (ch)
  		|| IS_RIDER (ch)
  		|| IS_RIDEE (ch)
  		|| IS_DROWNING(ch)
  		|| ch->following > 0
  		|| !IS_NPC (ch))
  		return;

	temp_vnum = ch->morphto;
	troom = ch->in_room;
	flag = ch->morph_type;
	
	if (temp_vnum <= 0) 
		{
		sprintf (nbuf, "Mob %d has a morph clock, but no morph Mobvnum\n",
ch->mob->nVirtual);
		system_log (nbuf, true);
		return;
		}
		
		if (troom <= 0)
		{
		sprintf (nbuf, "Mob %d has a morph clock, and will morph to %d but there is an error with the room number\n",
ch->mob->nVirtual, ch->morphto);
		system_log (nbuf, true);
		return;
		}
		
	if (temp_vnum == 86)
			{
			extract_char (ch);
			return;
			}
		else
			{
	newMob = load_mobile (temp_vnum);
			}
	
	if (!newMob)
		{
		sprintf (nbuf, "Attempt to load target morph mob %d from mob %d failed\n",
ch->morphto, ch->mob->nVirtual);
		system_log (nbuf, true);
		send_to_gods(nbuf);
		return;
		}
		
/********************
morphtype = 1 will simply transfer gear to the new mob.
morphtype = 2 will keep the same description, just change the skills

*********************/
	if (flag == 1) 
// physical morph includes new skills from new prototype
		{
		newMob->was_in_room = ch->was_in_room;
		newMob->last_room = ch->last_room;
		newMob->hour_affects = ch->hour_affects;
		}

	else if (flag == 2) // skill morph - Physical remains the same
		{
			
		newMob->flags = ch->flags;
		
		newMob->attack_type = ch->attack_type;
		newMob->hit = ch->hit;
		newMob->curse = ch->curse;
		newMob->defensive = ch->defensive;
		newMob->nat_attack_type = ch->nat_attack_type;
		newMob->attack_type = ch->attack_type;
		newMob->damage = ch->damage;
		newMob->natural_delay = ch->natural_delay;
		newMob->fight_mode = ch->fight_mode;
		newMob->offense = ch->offense;
		
		newMob->agi = ch->agi;
		newMob->aur = ch->aur;
		newMob->con = ch->con;
		newMob->dex = ch->dex;
		newMob->intel = ch->intel;
		newMob->str = ch->str;
		newMob->wil = ch->wil;
		
		newMob->tmp_agi = ch->tmp_agi;
		newMob->tmp_agi  = ch->tmp_agi; 
		newMob->tmp_con = ch->tmp_con;
		newMob->tmp_dex = ch->tmp_dex;
		newMob->tmp_intel = ch->tmp_intel;
		newMob->tmp_str = ch->tmp_str;
		newMob->tmp_wil = ch->tmp_wil;
		
		newMob->frame = ch->frame;
		newMob->height = ch->height;
		newMob->bmi = ch->bmi;
		newMob->body_proto = ch->body_proto;
		newMob->body_type = ch->body_type	;
		newMob->sex = ch->sex;
		newMob->size = ch->size;
		
		newMob->hire_storeobj = ch->hire_storeobj;
		newMob->hire_storeroom = ch->hire_storeroom;
		newMob->cell_1 = ch->cell_1;
		newMob->cell_2 = ch->cell_2;
		newMob->cell_3 = ch->cell_3;
  	newMob->in_room = ch->in_room;
		newMob->was_in_room = ch->was_in_room;
		newMob->last_room = ch->last_room;
		newMob->harness = ch->harness;
		newMob->max_harness = ch->max_harness;
		newMob->max_hit = ch->max_hit;
		newMob->max_move = ch->max_move;
		newMob->move = ch->move;
		newMob->speaks = ch->speaks;
		newMob->speed = ch->speed;
		newMob->nightmare = ch->nightmare;
		newMob->ppoints = ch->ppoints;

		newMob->poison_type = ch->poison_type;
		newMob->race = ch->race;
		newMob->affected_by = ch->affected_by;
		newMob->act = ch->act;
					
		if (ch->clans)
			{
			sprintf (nbuf, "%s", ch->clans);
  		newMob->name = str_dup (nbuf);
  		}		
		
		if (ch->name)
			{
			sprintf (nbuf, "%s", ch->name);
  		newMob->name = str_dup (nbuf);
  		}
  	
  	if (ch->tname)
			{
	  	sprintf (nbuf, "%s", ch->tname);
  		newMob->tname = str_dup (nbuf);
  		}
  		
  	if (ch->short_descr)
			{	
  		sprintf (nbuf, "%s", ch->short_descr);
  		newMob->short_descr = str_dup (nbuf);
  		}
  	
  	if (ch->long_descr)
			{	
  		sprintf (nbuf, "%s", ch->long_descr);
  		newMob->long_descr = str_dup (nbuf);
  		}
  	
  	if (ch->pmote_str)
			{	
  		sprintf (nbuf, "%s", ch->pmote_str);
  		newMob->pmote_str = str_dup (nbuf);
			}
	
		if (ch->voice_str)
			{	
  		sprintf (nbuf, "%s", ch->voice_str);
  		newMob->voice_str = str_dup (nbuf);
			} 
		
		if (ch->description)
			{	
		  sprintf (nbuf, "%s", ch->description);
  		newMob->description = str_dup (nbuf);
			}
			
		if (ch->travel_str)
			{	
  		sprintf (nbuf, "%s", ch->travel_str);
  		newMob->travel_str = str_dup (nbuf);
			}
			
		if (ch->dmote_str)
			{	
  		sprintf (nbuf, "%s", ch->dmote_str);
  		newMob->dmote_str = str_dup (nbuf);
			}  	
	
		newMob->hour_affects = ch->hour_affects;

/*
The new mob will take the best skill level between his old skill and the new skill level.
*/	 

		for (index = 0; index <= MAX_SKILLS; index ++)
			{
			if (ch->skills[index] > newMob->skills[index])
				{
				newMob->skills[index] = ch->skills[index];
				}
			}
		} //flag ==2		

/******** objects and equip for all ***/
	for (jdex = 1; jdex < MAX_WEAR; jdex++)
	  {
	    if (get_equip (ch, jdex))
		{
		nobj = unequip_char (ch, jdex);
		obj_to_char (nobj, newMob);
		equip_char (newMob, nobj, jdex);
		}
	  }

	if (ch->right_hand)
		{
		nobj = ch->right_hand;
		ch->right_hand = NULL;
		nobj->equiped_by = newMob;
		nobj->carried_by = newMob; 
		newMob->right_hand = nobj;
		}

	if (ch->left_hand)
		{
		nobj = ch->left_hand;
		ch->left_hand = NULL;
		nobj->equiped_by = newMob;
		nobj->carried_by = newMob;
		newMob->left_hand = nobj;
		}
	
	
	newMob->act |= ACT_STAYPUT;
	extract_char (ch);
	char_to_room (newMob, troom);
	

}

int
can_learn (CHAR_DATA * ch)
{

	CHAR_DATA * src = NULL;
	int flag = 0;
	
	for (src = character_list; src; src = src->next)
    {
		if (src->morphto == ch->mob->nVirtual)
			{
			flag = 1;
			return (flag);
			}
		}
	return 0;
}
