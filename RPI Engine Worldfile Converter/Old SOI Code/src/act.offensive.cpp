/*------------------------------------------------------------------------\
|  act.offensive.c : Violence Module                  www.middle-earth.us | 
|  Copyright (C) 2004, Shadows of Isildur: Traithe                        |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/

#include <stdlib.h>
#include <signal.h>
#include <stdio.h>
#include <string.h>
#include <ctype.h>
#include <stdlib.h>

#include <string>
#include "structs.h"
#include "net_link.h"
#include "protos.h"
#include "utils.h"
#include "decl.h"
#include "group.h"
#include "utility.h"

// Wasn't any function to quickly determine how many people are attacking somone,
// so here it is. Run through a list of people in the room, and if they're attacking
// ch, increase the counter.

int
num_attackers (CHAR_DATA * ch)
{

  CHAR_DATA * tch;
  int i = 0;

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (tch->fighting == ch)
	{
          i += 1;
	}
    }

  return i;
}


/* ch here is the victim mob or PC */
void
notify_guardians (CHAR_DATA * ch, CHAR_DATA * tch, int cmd)
{
  unsigned short int flag = 0x00;
  char buf[AVG_STRING_LENGTH];
  char *strViolation[] = {
    "Hits to injure", "Hits to kill", "Hits to injure", "Took aim at",
    "Slings at", "Throws at",
    "Attempts to steal from", "Attempts to pick the lock of"
  };


  if (!ch || !tch || IS_NPC (ch)	/* ignore attacks by npcs */
          || IS_SET (ch->flags, FLAG_GUEST) 
          || (IS_SET (tch->act, ACT_PREY) && cmd >= 3) 
          || GET_TRUST (ch)	/* ignore attacks by imms */
          || GET_TRUST (tch)	/* ignore attacks on imms */
    )
    {

      return;

    }

  flag |= (!IS_NPC (tch)) ? (GUARDIAN_PC) : 0;
  flag |= (tch->race >= 0
	   && tch->race <=
	   16) ? (GUARDIAN_NPC_HUMANOIDS) : (GUARDIAN_NPC_WILDLIFE);
  flag |= (tch->shop) ? (GUARDIAN_NPC_SHOPKEEPS) : 0;
  flag |= (IS_SET (tch->act, ACT_SENTINEL)) ? (GUARDIAN_NPC_SENTINELS) : 0;
  flag |= (IS_SET (tch->act, ACT_ENFORCER)) ? (GUARDIAN_NPC_ENFORCERS) : 0;
  flag |= ((tch->right_hand && GET_ITEM_TYPE (tch->right_hand) == ITEM_KEY)
	   || (tch->left_hand
	       && GET_ITEM_TYPE (tch->left_hand) ==
	       ITEM_KEY)) ? (GUARDIAN_NPC_KEYHOLDER) : 0;

  if (ch->in_room == tch->in_room)
    {

      sprintf (buf, "#3[Guardian: %s%s]#0 %s %s%s in %d.",
	       GET_NAME (ch),
	       IS_SET (ch->plr_flags, NEW_PLAYER_TAG) ? " (new)" : "",
	       strViolation[cmd],
	       (!IS_NPC (tch)) ? GET_NAME (tch) : (tch->short_descr),
	       (!IS_NPC (tch)
		&& IS_SET (tch->plr_flags,
			   NEW_PLAYER_TAG)) ? " (new)" : (IS_SET (flag,
								  GUARDIAN_NPC_KEYHOLDER)
							  ? " (keyholder)"
							  : (IS_SET
							     (flag,
							      GUARDIAN_NPC_SHOPKEEPS)
							     ? " (shopkeeper)"
							     : (IS_SET
								(flag,
								 GUARDIAN_NPC_ENFORCERS)
								?
								" (enforcer)"
								: (IS_SET
								   (flag,
								    GUARDIAN_NPC_SENTINELS)
								   ?
								   " (sentinel)"
								   : "")))),
	       tch->in_room);

    }
  else
    {

      sprintf (buf, "#3[Guardian: %s%s]#0 %s %s%s in %d, from %d.#0",
	       GET_NAME (ch),
	       IS_SET (ch->plr_flags, NEW_PLAYER_TAG) ? " (new)" : "",
	       strViolation[cmd],
	       (!IS_NPC (tch)) ? GET_NAME (tch) : (tch->short_descr),
	       (!IS_NPC (tch)
		&& IS_SET (tch->plr_flags,
			   NEW_PLAYER_TAG)) ? " (new)" : (IS_SET (flag,
								  GUARDIAN_NPC_KEYHOLDER)
							  ? " (keyholder)"
							  : (IS_SET
							     (flag,
							      GUARDIAN_NPC_SHOPKEEPS)
							     ? " (shopkeeper)"
							     : (IS_SET
								(flag,
								 GUARDIAN_NPC_ENFORCERS)
								?
								" (enforcer)"
								: (IS_SET
								   (flag,
								    GUARDIAN_NPC_SENTINELS)
								   ?
								   " (sentinel)"
								   : "")))),
	       tch->in_room, ch->in_room);
    }
  buf[11] = toupper (buf[11]);
  send_to_guardians (buf, flag);

}

void
do_throw (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *tobj, *armor1 = NULL, *armor2 = NULL;
  ROOM_DATA *troom = NULL;
  ROOM_DIRECTION_DATA *exit = NULL;
  CHAR_DATA *tch = NULL;
  AFFECTED_TYPE *af;
  bool can_lodge = false, ranged = false;
  int dir = 0, result = 0, location = 0;
  int wear_loc1 = 0, wear_loc2 = 0, wound_type = 0;
  float damage = 0;
  int range_mod = 0;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char buf3[MAX_STRING_LENGTH];
  char buf4[MAX_STRING_LENGTH];
  char buffer[MAX_STRING_LENGTH];
  char strike_location[MAX_STRING_LENGTH];
  int poison = 0;

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

  if (IS_SWIMMING (ch))
    {
      send_to_char ("You can't do that while swimming!\n", ch);
      return;
    }

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch))
    {
      send_to_char ("You cannot do this in an OOC area.\n", ch);
      return;
    }

  if(IS_SET(ch->room->room_flags, PEACE))
    {
      act ("Something prohibits you from taken such an action.", false, ch, 0, 0, TO_CHAR);
      return;
    }

  if(get_soma_affect(ch, SOMA_PLANT_VISIONS) && number(0,30) > GET_WIL(ch))
  {
    act ("You're paralyzed with fear, and unable to undertake such an action!", false, ch, 0, 0, TO_CHAR);
    return;
  }

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("What did you wish to throw?\n", ch);
      return;
    }

  if (!(tobj = get_obj_in_list (buf, ch->right_hand)) &&
      !(tobj = get_obj_in_list (buf, ch->left_hand)))
    {
      send_to_char ("You aren't holding that in either hand.\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("At what did you wish to throw?\n", ch);
      return;
    }

  if ((dir = is_direction (buf)) == -1)
    {
    if (!(tch = get_char_room_vis (ch, buf)))
      {
        send_to_char ("At what did you wish to throw?\n", ch);
	return;
      }
    }

  if (ch->fighting)
    {
      send_to_char
	("You are currently engaged in melee combat and cannot throw.\n", ch);
      return;
    }

  if (ch->balance <= -15)
    {
      send_to_char ("You're far too off-balance to attempt a throw.\n", ch);
      return;
    }

  if (tobj->obj_flags.weight / 100 > ch->str * 2)
    {
      send_to_char ("That object is too heavy to throw effectively.\n", ch);
      return;
    }

  if (tobj->count > 1)
    {
      send_to_char ("You may only throw one object at a time.\n", ch);
      return;
    }

  if (!tch && dir != -1)
    {
      if ((exit = EXIT (ch, dir)))
	troom = vtor (EXIT (ch, dir)->to_room);

				/* for throwing 'out' of a dwelling */
			//if ((dir == 6) && (ch->in_room > 100000))
			//	troom = vtor(ch->was_in_room);

	/*** harsh way to deal with throwing things out without crashing the game	**/	
	  if ((dir == 6) && (ch->in_room > 100000))
		{
		obj_from_char (&tobj, 0);
		extract_obj (tobj);
		sprintf (buf, "You dispose of  #2%s#0.", tobj->short_description);
		act (buf, false, ch, tobj, 0, TO_CHAR | _ACT_FORMAT);
		return;
		}
/****/
      if (!troom)
	{
	  send_to_char ("There is no exit in that direction.\n", ch);
	  return;
	}

      if (exit 
      && IS_SET (exit->exit_info, EX_ISDOOR)
      && IS_SET (exit->exit_info, EX_CLOSED)
      && !IS_SET (exit->exit_info, EX_ISGATE))
	{
	  send_to_char ("Your view is blocked.\n", ch);
	  return;
	}

      argument = one_argument (argument, buf);

      if (*buf)
	{
	  tch = get_char_room_vis2 (ch, troom->nVirtual, buf);
	  if (!has_been_sighted (ch, tch))
	    tch = NULL;
	  if (!tch)
	    {
	      send_to_char
		("You do not see anyone like that in this direction.\n", ch);
	      return;
	    }
	}
	
      if (!tch)
	{
	  sprintf (buf, "You hurl #2%s#0 %sward.", tobj->short_description,
		   dirs[dir]);
	  		
	  act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);

	  sprintf (buf, "#5%s#0 hurls #2%s#0 %sward.", char_short (ch),
		   tobj->short_description, dirs[dir]);
	  buf[2] = toupper (buf[2]);
          watched_action(ch, buf, 0, 1);
	  act (buf, true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);

	  obj_from_char (&tobj, 0);
	  obj_to_room (tobj, troom->nVirtual);

	  sprintf (buf, "#2%s#0 flies in, hurled from %s.",
		   tobj->short_description, verbose_dirs[rev_dir[dir]]);
	  buf[2] = toupper (buf[2]);
	  send_to_room (buf, troom->nVirtual);

	  return;
	}
  		}//if (!tch && dir != -1)

  if (tch)
    {
      if (tch == ch)
	{
	  send_to_char ("That wouldn't require much of a throw...\n", ch);
	  return;
	}

      skill_learn(ch, SKILL_THROWN);

      if (are_grouped (ch, tch) && *argument != '!')
      {
        sprintf (buf,  
	       "#1You decide not to throw at $N #1who is a fellow group member!#0");
        act (buf, false, ch, 0, tch, TO_CHAR | _ACT_FORMAT);
        return;
      }
 
      if (IS_SET (tch->act, ACT_VEHICLE))
      {
         sprintf (buf, "And what good will that do to hurt $N?");
        act (buf, false, ch, 0, tch, TO_CHAR | _ACT_FORMAT);
        return;
      }

      if (ch->room != tch->room && dir != -1)
	{
	  if ((exit = EXIT (ch, dir)))
	    troom = vtor (EXIT (ch, dir)->to_room);

	  if (!troom)
	    {
	      send_to_char ("There is no exit in that direction.\n", ch);
	      return;
	    }

	  if (exit 
	  		&& IS_SET (exit->exit_info, EX_CLOSED)
	  		&& !IS_SET (exit->exit_info, EX_ISGATE))
	    {
	      send_to_char ("Your view is blocked.\n", ch);
	      return;
	    }
	}
      notify_guardians (ch, tch, 5);

      if (dir != -1)
        range_mod = 5;

      result =
	calculate_missile_result (ch, SKILL_THROWN, ((ch->balance * -10) + range_mod), tch, 0,
				  0, tobj, NULL, &location, &damage, &poison);

    
  if (get_affect (ch, MAGIC_HIDDEN))
    {
      remove_affect_type (ch, MAGIC_HIDDEN);
      send_to_char ("You emerge from concealment and prepare to throw.\n\n",
		    ch);
    }

  if ((result == CRITICAL_MISS || result == MISS) && tch->fighting
      && tch->fighting != ch && number (1, 25) > ch->dex)
    {
 	send_to_char("You realize there are no safe targets.\n", ch);
	return;
    }

      damage = (int) damage;

      wear_loc1 = body_tab[0][location].wear_loc1;
      wear_loc2 = body_tab[0][location].wear_loc2;

      if (wear_loc1)
	{
	  armor1 = get_equip (tch, wear_loc1);
	  if (armor1 && GET_ITEM_TYPE (armor1) != ITEM_ARMOR)
	    armor1 = NULL;
	}
      if (wear_loc2)
	{
	  armor2 = get_equip (tch, wear_loc2);
	  if (armor2 && GET_ITEM_TYPE (armor2) != ITEM_ARMOR)
	    armor2 = NULL;
	}

      if (!ch->fighting && damage > 3)
	criminalize (ch, tch, tch->room->zone, CRIME_KILL);

      if (ch->room != tch->room)
	{
	  sprintf (buf, "You hurl #2%s#0 %s, toward #5%s#0.",
		   tobj->short_description, dirs[dir],
		   char_short (tch));
	  sprintf (buf2, "#5%s#0 hurls #2%s#0 %s, toward #5%s#0.",
		   char_short (ch), tobj->short_description,
		   dirs[dir], char_short (tch));
	  buf2[2] = toupper (buf2[2]);
          watched_action(ch, buf2, 0, 1);
	  act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
	  act (buf2, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
	}
      else
	{
	  sprintf (buf, "You hurl #2%s#0 forcefully at #5%s#0.",
		   tobj->short_description, char_short (tch));
	  sprintf (buf2, "#5%s#0 hurls #2%s#0 forcefully at #5%s#0.",
		   char_short (ch), tobj->short_description,
		   char_short (tch));
	  buf2[2] = toupper (buf2[2]);
          watched_action(ch, buf2, 0, 1);
	  sprintf (buf3, "#5%s#0 hurls #2%s#0 forcefully at you.",
		   char_short (ch), tobj->short_description);
	  buf3[2] = toupper (buf3[2]);
	  act (buf, false, ch, 0, tch, TO_CHAR | _ACT_FORMAT);
	  act (buf2, false, ch, 0, tch, TO_NOTVICT | _ACT_FORMAT);
	  act (buf3, false, ch, 0, tch, TO_VICT | _ACT_FORMAT);
	}

      if (GET_ITEM_TYPE (tobj) == ITEM_WEAPON
	  && (tobj->o.weapon.hit_type == 0 || tobj->o.weapon.hit_type == 1
	      || tobj->o.weapon.hit_type == 2
	      || tobj->o.weapon.hit_type == 4))
	{
	  if (result != CRITICAL_HIT && armor1
	      && armor1->o.armor.armor_type >= 2)
	    can_lodge = false;
	  else if (result != CRITICAL_HIT && armor2
		   && armor2->o.armor.armor_type >= 2)
	    can_lodge = false;
	  else if (result != CRITICAL_HIT && tch->armor && tch->armor >= 4)
	    can_lodge = false;
	  else
	    can_lodge = true;
	}

      sprintf (strike_location, "%s", figure_location (tch, location));

      if ((af = get_affect (tch, MAGIC_AFFECT_PROJECTILE_IMMUNITY)))
	{
	  sprintf (buf, "%s", spell_activity_echo (af->a.spell.sn, af->type));
	  if (!*buf)
	    sprintf (buf,
		     "\nThe projectile is deflected harmlessly aside by some invisible force.");
	  sprintf (buf2, "\n%s", buf);
	  result = MISS;
	  damage = 0;
	}
	
      else if (result == MISS)
	{
	  sprintf (buf, "%s#0 comes flying through the air from the %s, headed straight toward #5%s#0\n\nIt misses completely.", tobj->short_description, dirs[rev_dir[dir]], char_short(tch));
          *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "%s", buffer);

	  sprintf (buf2, "#2%s#0 comes flying through the air from the %s, headed straight toward you!\n\nIt misses completely.", tobj->short_description, dirs[rev_dir[dir]]);
          *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "%s", buffer);

	  sprintf (buf4, "It misses completely.");
	}
      else if (result == CRITICAL_MISS)
	{
	  sprintf (buf, "#2%s#0 comes flying through the air from the %s, headed straight toward #5%s#0\n\nIt flies far wide of any target.", tobj->short_description, dirs[rev_dir[dir]], char_short(tch));
          *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "%s", buffer);

	  sprintf (buf2, "#2%s#0 comes flying through the air from the %s, headed straight toward you!\n\nIt flies far wide of any target.", tobj->short_description, dirs[rev_dir[dir]]);
          *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "%s", buffer);

	  sprintf (buf4, "It flies far wide of any target.");
	}
      else if (result == SHIELD_BLOCK)
	{
	  if (obj_short_desc (get_equip (tch, WEAR_SHIELD)))
	  {
	  sprintf (buf, "#2%s#0 comes flying through the air from the %s, headed straight toward #5%s#0\n\nIt glances harmlessly off #2%s#0.", tobj->short_description, dirs[rev_dir[dir]], char_short(tch), obj_short_desc (get_equip (tch, WEAR_SHIELD)));
          *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "%s", buffer);

	  sprintf (buf2, "#2%s#0 comes flying through the air from the %s, headed straight toward you!\n\nIt glances harmlessly off #2%s#0.", tobj->short_description, dirs[rev_dir[dir]], obj_short_desc (get_equip (tch, WEAR_SHIELD)));
          *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "%s", buffer);

	  sprintf (buf4, "It glances harmlessly off #2%s#0.", obj_short_desc (get_equip (tch, WEAR_SHIELD)));
		  }
		  else
		  	{
	  sprintf (buf, "#2%s#0 comes flying through the air from the %s, headed straight toward #5%s#0\n\nIt glances harmlessly off something.", tobj->short_description, dirs[rev_dir[dir]], char_short(tch));
          *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "%s", buffer);

	  sprintf (buf2, "#2%s#0 comes flying through the air from the %s, headed straight toward you!\n\nIt glances harmlessly off something.", tobj->short_description, dirs[rev_dir[dir]]);
          *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "%s", buffer);

	  sprintf (buf4, "It glances harmlessly off something.");
		  	}
	}
      else if (result == GLANCING_HIT)
	{
	  sprintf (buf, "#2%s#0 comes flying through the air from the %s, headed straight toward #5%s#0\n\nIt grazes %s on the %s.", tobj->short_description, dirs[rev_dir[dir]], char_short(tch), HMHR (tch), expand_wound_loc (strike_location));
          *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "%s", buffer);

	  sprintf (buf2, "#2%s#0 comes flying through the air from the %s, headed straight toward you!\n\nIt grazes you on the %s.", tobj->short_description, dirs[rev_dir[dir]], expand_wound_loc (strike_location));
          *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "%s", buffer);

	  sprintf (buf4, "It grazes %s on the %s.",HMHR (tch), expand_wound_loc (strike_location));

	}
      else if (result == HIT)
	{
	  if (can_lodge)
	    {
	  sprintf (buf, "#2%s#0 comes flying through the air from the %s, headed straight toward #5%s#0\n\nIt lodges in %s %s.", tobj->short_description, dirs[rev_dir[dir]], char_short(tch), HSHR (tch), expand_wound_loc (strike_location));
          *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "%s", buffer);
		       
	  sprintf (buf2, "#2%s#0 comes flying through the air from the %s, headed straight toward you!\n\nIt lodges in your %s.", tobj->short_description, dirs[rev_dir[dir]], expand_wound_loc (strike_location));
          *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "%s", buffer);

	  sprintf (buf4, "It lodges in %s %s.", HSHR (tch), expand_wound_loc (strike_location));
	    }
	  else
	    {
	  sprintf (buf, "#2%s#0 comes flying through the air from the %s, headed straight toward #5%s#0\n\nIt strikes %s on the %s.", tobj->short_description, dirs[rev_dir[dir]], char_short(tch),  HMHR (tch), expand_wound_loc (strike_location));
          *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "%s", buffer);
		      
	  sprintf (buf2, "#2%s#0 comes flying through the air from the %s, headed straight toward you!\n\nIt strikes you on the %s.", tobj->short_description, dirs[rev_dir[dir]], expand_wound_loc (strike_location));
          *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "%s", buffer);

	  sprintf (buf4, "It strikes %s on the %s.", HMHR (tch), expand_wound_loc (strike_location));
	    }
	}
      else if (result == CRITICAL_HIT)
	{
	  if (can_lodge)
	    {
	  sprintf (buf, "#2%s#0 comes flying through the air from the %s, headed straight toward #5%s#0\n\nIt lodges deeply in %s %s!", tobj->short_description, dirs[rev_dir[dir]], char_short(tch),  HSHR (tch), expand_wound_loc (strike_location));
          *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "%s", buffer);

	  sprintf (buf2, "#2%s#0 comes flying through the air from the %s, headed straight toward you!\n\nIt lodges deeply in your %s!", tobj->short_description, dirs[rev_dir[dir]], expand_wound_loc (strike_location));
          *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "%s", buffer);

	  sprintf (buf4, "It lodges deeply in %s %s.", HSHR (tch), expand_wound_loc (strike_location));
	    }
	  else
	    {
	  sprintf (buf, "#2%s#0 comes flying through the air from the %s, headed straight toward #5%s#0\n\nIt strikes %s solidly on the %s.", HMHR (tch), tobj->short_description, dirs[rev_dir[dir]], char_short(tch), expand_wound_loc (strike_location));
          *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "%s", buffer);
		       
	  sprintf (buf2, "#2%s#0 comes flying through the air from the %s, headed straight toward you!\n\nIt strikes you solidly on the %s.", tobj->short_description, dirs[rev_dir[dir]], expand_wound_loc (strike_location));
          *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "%s", buffer);

	  sprintf (buf4, "It strikes %s solidly on the %s.", HMHR (tch), expand_wound_loc (strike_location));
	    }
	}

      char *out1, *out2, *out3;
      CHAR_DATA* rch = 0;
      reformat_string (buf, &out1);
      reformat_string (buf2, &out2);
      reformat_string (buf4, &out3);

      if (ch->room != tch->room)
	{
	  // aggressor's room
	  for (rch = ch->room->people; rch; rch = rch->next_in_room)
	    {
	      send_to_char ("\n", rch);
	      send_to_char (out3, rch);
	    }
	}

      // victim's room
      for (rch = tch->room->people; rch; rch = rch->next_in_room)
	{
	  send_to_char ("\n", rch);
          if(ch->room == tch->room)
            send_to_char (out3, rch);
	  else if (rch == tch)
	    send_to_char (out2, rch);
	  else
	    send_to_char (out1, rch);
	}

      mem_free (out1);
      mem_free (out2);

      obj_from_char (&tobj, 0);

      if ((result == HIT || result == CRITICAL_HIT) && can_lodge)
	{
	  lodge_missile (tch, tobj, strike_location);
	}
      else
	obj_to_room (tobj, tch->in_room);

      if (damage > 0)
	{
	  if (!IS_NPC (tch))
	    {
	      tch->delay_ch = ch;
	      tch->delay_info1 = tobj->nVirtual;
	    }
	  if (ch->room != tch->room)
	    ranged = true;
	  if (GET_ITEM_TYPE (tobj) == ITEM_WEAPON)
	    wound_type = tobj->o.weapon.hit_type;
	  else
	    wound_type = 3;
	  if (wound_to_char
	      (tch, strike_location, (int) damage, wound_type, 0, poison, 0))
	    {
	      if (ranged)
		send_to_char ("\nYour target collapses, dead.\n", ch);
	      ch->ranged_enemy = NULL;
	      return;
	    }
	  if (!IS_NPC (tch))
	    {
	      tch->delay_ch = NULL;
	      tch->delay_info1 = 0;
	    }
	}

      if (ch->agi <= 9)
	ch->balance += -10;
      else if (ch->agi > 9 && ch->agi <= 13)
	ch->balance += -8;
      else if (ch->agi > 13 && ch->agi <= 15)
	ch->balance += -7;
      else if (ch->agi > 15 && ch->agi <= 18)
	ch->balance += -6;
      else
	ch->balance += -5;

      ch->balance = MAX (ch->balance, -25);
      
      npc_ranged_response (tch, ch);	// do_throw

      return;
    }

  send_to_char
    ("There has been an error; please report your command syntax to the staff.\n",
     ch);
}

void
do_whirl (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *sling;

  if (IS_SWIMMING (ch))
    {
      send_to_char ("You can't do that while swimming!\n", ch);
      return;
    }

  if (ch->fighting)
    {
      send_to_char ("You're fighting for your life!\n", ch);
      return;
    }

  if (!get_equip (ch, WEAR_PRIM))
    {
      send_to_char
	("You need to be wielding a loaded sling to use this command.\n", ch);
      return;
    }

  sling = get_equip (ch, WEAR_PRIM);

  if (sling->o.weapon.use_skill != SKILL_SLING)
    {
      send_to_char ("This command is for use with slings only.\n", ch);
      return;
    }

  if (!sling->contains)
    {
      send_to_char ("The sling needs to be loaded, first.\n", ch);
      return;
    }

  act ("You begin whirling $p, gathering momentum for a shot.", false, ch,
       sling, 0, TO_CHAR | _ACT_FORMAT);
  act ("$n begins whirling $p, gathering momentum for a shot.", true, ch,
       sling, 0, TO_ROOM | _ACT_FORMAT);

  ch->whirling = 1;
}

/** There is no block from a critical hit. 
** If you are under cover, all other shots are misses. 
** If you are not under cover, then you have a chance to block missiles with your shield.
**/
int
projectile_shield_block (CHAR_DATA * ch, int result)
{
  OBJ_DATA *shield_obj;
  int roll, dir = 0;
	
  if (result == CRITICAL_HIT)
    return 0;

	dir = ch->cover_from_dir;
    
	if ((dir == 0 && get_affect (ch, AFFECT_COVER_NORTH))||
				(dir == 1 && get_affect (ch, AFFECT_COVER_EAST))||
				(dir == 2 && get_affect (ch, AFFECT_COVER_SOUTH))||
				(dir == 3 && get_affect (ch, AFFECT_COVER_WEST))||
				(dir == 4 && get_affect (ch, AFFECT_COVER_UP))||
				(dir == 5 && get_affect (ch, AFFECT_COVER_DOWN)))
			return 1;

  if ((shield_obj = get_equip (ch, WEAR_SHIELD)))
    {
      skill_use (ch, SKILL_AVERT, 0);
      roll = number (1, SKILL_CEILING);
      if (roll <= ch->skills[SKILL_AVERT])
	{
	  return 1;
	}
    }

  return 0;
}

int
calculate_missile_result (CHAR_DATA * ch, int ch_skill, int att_modifier,
			  CHAR_DATA * target, int def_modifier,
			  OBJ_DATA * weapon, OBJ_DATA * missile,
			  AFFECTED_TYPE * spell, int *location, float *damage, int *poison)
{
  OBJ_DATA *armor1 = NULL, *armor2 = NULL;
  int roll = 0, defense = 0, assault = 0, result = 0;
  int wear_loc1 = 0, wear_loc2 = 0, body_type = 0;
  int hit_type = 0;
  POISON_DATA *xpoison;
  int pdam = 0;

  /* Determine result of hit attempt. */

  if (!CAN_SEE (target, ch))
    def_modifier += number (15, 30);

  skill_use (target, SKILL_DODGE, 0);

  roll = number (1, SKILL_CEILING);
  roll += def_modifier;
  roll = MIN (roll, SKILL_CEILING);

  if (roll > target->skills[SKILL_DODGE])
    {
      if (roll % 5 == 0 || roll == 1)
	defense = RESULT_CF;
      else
	defense = RESULT_MF;
    }
  else
    {
      if (roll % 5 == 0)
	defense = RESULT_CS;
      else
	defense = RESULT_MS;
    }

  skill_use (ch, ch_skill, 0);

// Weather effects, only affect bows, but it does affect it A LOT

  if(weapon)
  {
    if(weapon->o.weapon.use_skill == SKILL_LONGBOW || weapon->o.weapon.use_skill == SKILL_SHORTBOW)
    {
    if (weather_info[ch->room->zone].fog == THIN_FOG)
      att_modifier += 5;
    if (weather_info[ch->room->zone].fog == THICK_FOG)
      att_modifier += 10;
    if (weather_info[ch->room->zone].state == LIGHT_RAIN)
      att_modifier += 10;    
    if (weather_info[ch->room->zone].state == STEADY_RAIN)
      att_modifier += 20;
    if (weather_info[ch->room->zone].state == HEAVY_RAIN)
      att_modifier += 30;
    if (weather_info[ch->room->zone].state == STEADY_SNOW)
      att_modifier += 25;
    }
  }

// If you're shooting at someone in a forest or a wood, and you're not in the same room
// as they are, take a big hit to your chances.

  if (target->room->sector_type ==SECT_WOODS && target->room->nVirtual != ch->room->nVirtual)
 		att_modifier += 15; 
  if (target->room->sector_type ==SECT_FOREST && target->room->nVirtual != ch->room->nVirtual) 
 		att_modifier += 25;

// Random modifier
  roll = number (1, SKILL_CEILING);
  roll += att_modifier;
  roll = MIN (roll, SKILL_CEILING);

  ch->aim = 0;

  if (ch->skills[ch_skill])
    {
      if (roll > ch->skills[ch_skill])
	{
	  if (roll % 5 == 0 || roll == 1)
	    assault = RESULT_CF;
	  else
	    assault = RESULT_MF;
	}
      else if (roll <= ch->skills[ch_skill])
	{
	  if (roll % 5 == 0)
	    assault = RESULT_CS;
	  else
	    assault = RESULT_MS;
	}
    }
  else
    {
      if (roll > ch->skills[SKILL_OFFENSE])
	{
	  if (roll % 5 == 0 || roll == 1)
	    assault = RESULT_CF;
	  else
	    assault = RESULT_MF;
	}
      else if (roll <= ch->skills[SKILL_OFFENSE])
	{
	  if (roll % 5 == 0)
	    assault = RESULT_CS;
	  else
	    assault = RESULT_MS;
	}
    }

  if (assault == defense)
    {
      if (number (1, 10) > 5)
	result = GLANCING_HIT;
      else
	result = MISS;
    }
  if (assault == RESULT_CS)
    {
      if (defense == RESULT_MS)
	result = HIT;
      else if (defense == RESULT_MF)
	result = HIT;
      else if (defense == RESULT_CF)
	result = CRITICAL_HIT;
    }
  else if (assault == RESULT_MS)
    {
      if (defense == RESULT_CS)
	result = MISS;
      else if (defense == RESULT_MF)
	result = HIT;
      else if (defense == RESULT_CF)
	result = CRITICAL_HIT;
    }
  else if (assault == RESULT_MF)
    {
      if (defense == RESULT_CS)
	result = CRITICAL_MISS;
      else if (defense == RESULT_MS)
	result = MISS;
      else if (defense == RESULT_CF)
	result = GLANCING_HIT;
    }
  else if (assault == RESULT_CF)
    {
      if (defense == RESULT_CS)
	result = CRITICAL_MISS;
      else if (defense == RESULT_MS)
	result = CRITICAL_MISS;
      else if (defense == RESULT_MF)
	result = MISS;
    }

  if (!AWAKE (target) && assault != RESULT_CF)
    result = CRITICAL_HIT;

  if ((result == HIT || result == GLANCING_HIT)
      && projectile_shield_block (target, result))
    result = SHIELD_BLOCK;

  if (result == MISS || result == CRITICAL_MISS || result == SHIELD_BLOCK)
    return result;

  /* Determine damage of hit, if applicable. */

  *damage = 0;

  body_type = 0;

  roll = number (1, 100);
  *location = -1;

  while (roll > 0)
    roll = roll - body_tab[body_type][++(*location)].percent;

  wear_loc1 = body_tab[body_type][*location].wear_loc1;
  wear_loc2 = body_tab[body_type][*location].wear_loc2;

  if (wear_loc1)
    {
      armor1 = get_equip (target, wear_loc1);
      if (armor1 && GET_ITEM_TYPE (armor1) != ITEM_ARMOR)
	armor1 = NULL;
    }
  if (wear_loc2)
    {
      armor2 = get_equip (target, wear_loc2);
      if (armor2 && GET_ITEM_TYPE (armor2) != ITEM_ARMOR)
	armor2 = NULL;
    }

  if (missile)
    {
      if (GET_ITEM_TYPE (missile) == ITEM_MISSILE)
      {
        hit_type = 1;
	*damage = dice (missile->o.od.value[0], missile->o.od.value[1]);
      }
      else if(GET_ITEM_TYPE(missile) == ITEM_BULLET)
      {
        hit_type = 3;
	*damage = dice (missile->o.od.value[0], missile->o.od.value[1]);
      }
      else if (GET_ITEM_TYPE (missile) == ITEM_WEAPON)
      {
        hit_type = missile->o.weapon.hit_type;
	*damage = dice (missile->o.od.value[1], missile->o.od.value[2]);
      }
      else
      {
        hit_type = 3;
	*damage = dice (1, 3);
      }

      if (weapon) 		/*  Launched from a bow/sling */
      {
	*damage += weapon->o.od.value[4];

         // Longbows are a lot more damaging, but far less accurate.
         if(weapon->o.weapon.use_skill == SKILL_LONGBOW)
           *damage += 2;
      }

      // As all archery, hits can be shrugged off, but critical hits are often lethal.

      if (weapon || IS_SET (missile->obj_flags.extra_flags, ITEM_THROWING))
	{
	  if (result == HIT)
	    {			
	      *damage += number (1, 2);
	      *damage *= 1.25;
	    }
	  else if (result == CRITICAL_HIT)
	    {
	      *damage += number (3, 5);
	      *damage *= 2.50;
	    }
	}

      if (armor1 != NULL)
	{
	  *damage -= armor1->o.armor.armor_value;
	  *damage += weapon_armor_table[hit_type][armor1->o.armor.armor_type];
	}
      if (armor2 != NULL)
	{
	  *damage -= armor2->o.armor.armor_value;
	  *damage += weapon_armor_table[hit_type][armor2->o.armor.armor_type];
	}
      if (target->armor)
	{
	  *damage -= target->armor;
	}

      if (weapon)
	{
          // Missiles do more damage to the torso, neck, head and eyes, but significantly
          // less to the limbs and arms. Can run across the field with a dozen arrows sticking
          // from your shoulders and legs, but one lucky one to the throat and you're dead.
          if (*location == 0)
              *damage *= 1.60;
          if (*location == 1)
              *damage *= 0.4;
          if (*location == 2)
              *damage *= 0.4;
          if (*location == 3)
              *damage *= 2.80;
          if (*location == 4)
              *damage *= 3.45;
          if (*location == 5)
              *damage *= 0.10;
          if (*location == 6)
              *damage *= 0.10;
          if (*location == 7)
              *damage *= 4.40;
	}
      else if(IS_SET (missile->obj_flags.extra_flags, ITEM_THROWING)) 
      {     
        *damage *= body_tab[body_type][*location].damage_mult;
        *damage /= body_tab[body_type][*location].damage_div;
      }
      else if (missile)
	{
	  if (GET_ITEM_TYPE (missile) != ITEM_WEAPON
	      && GET_ITEM_TYPE (missile) != ITEM_MISSILE
	      && GET_ITEM_TYPE (missile) != ITEM_BULLET)
	    *damage = 0;
	}
    }

  if (weapon && weapon->o.weapon.use_skill == SKILL_SLING)
    {
      if (att_modifier < 0)
	att_modifier *= -1;
      if (att_modifier <= 1)
	*damage *= 0.250;
      else if (att_modifier > 1 && att_modifier <= 2)
	*damage *= 0.500;
      else if (att_modifier > 2 && att_modifier <= 3)
	*damage *= 0.750;
      else if (att_modifier > 3)
	;
    }

  // Like in fight.cpp, we roll to see if we can poison them or not - but it's a lot
  // easier to do it with distance objects.

  pdam = (int) *damage;
 
  if(pdam && missile && missile->poison)
  {
    for (xpoison = missile->poison; xpoison; xpoison = xpoison->next)
    {
      if((hit_type != 3 && hit_type != 9) && 
        ((number(1,20) >= (GET_CON(target) + 5 - pdam)) || 
        ((number(1,20) >= (GET_CON(target) - pdam)) && 
        (hit_type == 0 || hit_type == 7 || hit_type == 1))))
      {

        soma_add_affect(target, xpoison->poison_type, xpoison->duration, xpoison->latency, 
                       xpoison->minute, xpoison->max_power, xpoison->lvl_power, xpoison->atm_power,
                       xpoison->attack, xpoison->decay, xpoison->sustain, xpoison->release);
 
        *poison = xpoison->poison_type;
      }
    }
  }


  if(*poison > 0)
    send_to_char(lookup_poison_variable(*poison, 2), target);

  return result;
}

void
lodge_missile (CHAR_DATA * target, OBJ_DATA * ammo, char *strike_location)
{
  LODGED_OBJECT_INFO *lodged = NULL;

  if (!target->lodged)
    {
      CREATE (target->lodged, LODGED_OBJECT_INFO, 1);
      target->lodged->vnum = ammo->nVirtual;
      target->lodged->location = add_hash (strike_location);
      target->lodged->colored = 0;

      if(ammo->var_color)
      {
        target->lodged->colored = 1;
        target->lodged->var_color = ammo->var_color;
        target->lodged->var_color2 = ammo->var_color2;
        target->lodged->var_color3 = ammo->var_color3;
        target->lodged->short_description = ammo->short_description;
      }
    }
  else
    for (lodged = target->lodged; lodged; lodged = lodged->next)
      {
	if (!lodged->next)
	  {
	    CREATE (lodged->next, LODGED_OBJECT_INFO, 1);
	    lodged->next->vnum = ammo->nVirtual;
	    lodged->next->location = add_hash (strike_location);
            lodged->next->colored = 0;

            if(ammo->var_color)
            {
              lodged->next->colored = 1;
              lodged->next->var_color = ammo->var_color;
              lodged->next->var_color2 = ammo->var_color2;
              lodged->next->var_color3 = ammo->var_color3;
              lodged->next->short_description = ammo->short_description;
            }

	    break;
	  }
	else
	  continue;
      }
}

void
fire_sling (CHAR_DATA * ch, OBJ_DATA * sling, char *argument)
{
  CHAR_DATA *tch = NULL;
  ROOM_DATA *troom = NULL;
  OBJ_DATA *ammo;
  AFFECTED_TYPE *af;
  char buf[MAX_STRING_LENGTH], strike_location[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH], buf3[MAX_STRING_LENGTH];
  float damage = 0;
  int dir = 0, location = 0, result = 0, attack_mod = 0, wound_type = 0;
  bool ranged = false;
  int poison = 0;

  const char *fancy_dirs[] = {
    "northward",
    "eastward",
    "southward",
    "westward",
    "upward",
    "downward",
    "\n"
  };

  if (!ch || !sling)
    return;

  if (!*argument)
    {
      send_to_char ("At what did you wish to fire your sling?\n", ch);
      return;
    }

  if (!ch->whirling)
    {
      send_to_char
	("You'll need to begin whirling your sling before you can fire.\n",
	 ch);
      return;
    }

  if (!sling->contains)
    {
      send_to_char ("You'll need to load your sling first.\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if ((dir = is_direction (buf)) == -1)
    {
      if (!(tch = get_char_room_vis (ch, buf)))
      	{
	  send_to_char ("At what did you wish to fire?\n", ch);
	  return;
	}
    }

  if (!tch && dir != -1)
    {
      if (EXIT (ch, dir))
	troom = vtor (EXIT (ch, dir)->to_room);

      if (!troom)
	{
	  send_to_char ("There is no exit in that direction.\n", ch);
	  return;
	}

      argument = one_argument (argument, buf);

      if (*buf)
	{
	  tch = get_char_room_vis2 (ch, troom->nVirtual, buf);
	  if (!has_been_sighted (ch, tch))
	    tch = NULL;
	  if (!tch)
	    {
	      send_to_char
		("You do not see anyone like that in this direction.\n", ch);
	      return;
	    }
	}
    }

  if (!tch)
    {
      send_to_char ("At whom did you wish to fire your sling?\n", ch);
      return;
    }

  if (tch == ch)
    {
      send_to_char ("Don't be silly.\n", ch);
      return;
    }

  if (IS_SET (tch->act, ACT_VEHICLE))
  {
      sprintf (buf, "And what good will that do to hurt $N?");
      act (buf, false, ch, 0, tch, TO_CHAR | _ACT_FORMAT);
      return;
  }


  attack_mod -= ch->whirling;
  ch->whirling = 0;
  notify_guardians (ch, tch, 4);

  result =
    calculate_missile_result (ch, sling->o.weapon.use_skill, attack_mod, tch,
			      0, sling, sling->contains, NULL, &location,
			      &damage, &poison);
  damage = (int) damage;

  sprintf (strike_location, "%s", figure_location (tch, location));

  if (ch->room != tch->room)
    {
      sprintf (buf, "You sling #2%s#0 %s, toward #5%s#0.",
	       sling->contains->short_description, fancy_dirs[dir],
	       char_short (tch));
      sprintf (buf2, "#5%s#0 slings #2%s#0 %s, toward #5%s#0.",
	       char_short (ch), sling->contains->short_description,
	       fancy_dirs[dir], char_short (tch));
      buf2[2] = toupper (buf2[2]);
      watched_action(ch, buf2, 0, 1);
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      act (buf2, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
    }
  else
    {
      sprintf (buf, "You sling #2%s#0 forcefully at #5%s#0.",
	       sling->contains->short_description, char_short (tch));
      sprintf (buf2, "#5%s#0 slings #2%s#0 forcefully at #5%s#0.",
	       char_short (ch), sling->contains->short_description,
	       char_short (tch));
      sprintf (buf3, "#5%s#0 slings #2%s#0 forcefully at you.",
	       char_short (ch), sling->contains->short_description);
      watched_action(ch, buf2, 0, 1);
      act (buf, false, ch, 0, tch, TO_CHAR | _ACT_FORMAT);
      act (buf, false, ch, 0, tch, TO_VICT | _ACT_FORMAT);
      act (buf2, false, ch, 0, tch, TO_NOTVICT | _ACT_FORMAT);
    }

  if ((af = get_affect (tch, MAGIC_AFFECT_PROJECTILE_IMMUNITY)))
    {
      sprintf (buf, "%s", spell_activity_echo (af->a.spell.sn, af->type));
      if (!*buf)
	sprintf (buf,
		 "\nThe is deflected harmlessly aside by some invisible force.");
      sprintf (buf2, "\n%s", buf);
      result = MISS;
      damage = 0;
    }
  else if (result == MISS)
    {
      sprintf (buf, "It misses completely.");
      sprintf (buf2, "It misses completely.");
    }
  else if (result == CRITICAL_MISS)
    {
      sprintf (buf, "It flies far wide of any target.");
      sprintf (buf2, "It flies far wide of any target.");
    }
  else if (result == SHIELD_BLOCK)
    {
      if (obj_short_desc (get_equip (tch, WEAR_SHIELD)))
			{
		  sprintf (buf, "It glances harmlessly off #2%s#0.",
			   obj_short_desc (get_equip (tch, WEAR_SHIELD)));
	  	sprintf (buf2, "It glances harmlessly off #2%s#0.",
		  	 obj_short_desc (get_equip (tch, WEAR_SHIELD)));
		  }
		  else
		  	{
		  	sprintf (buf, "It glances harmlessly off something.");
		  	sprintf (buf2, "It glances harmlessly off something.");
		  	}
    }
  else if (result == GLANCING_HIT)
    {
      sprintf (buf, "It grazes %s on the %s.", HMHR (tch),
	       expand_wound_loc (strike_location));
      sprintf (buf2, "It grazes you on the %s.",
	       expand_wound_loc (strike_location));
    }
  else if (result == HIT)
    {
      sprintf (buf, "It strikes %s on the %s.", HMHR (tch),
	       expand_wound_loc (strike_location));
      sprintf (buf2, "It strikes you on the %s.",
	       expand_wound_loc (strike_location));
    }
  else if (result == CRITICAL_HIT)
    {
      sprintf (buf, "It strikes %s solidly on the %s.", HMHR (tch),
	       expand_wound_loc (strike_location));
      sprintf (buf2, "It strikes you solidly on the %s.",
	       expand_wound_loc (strike_location));
    }

  ammo = sling->contains;

  obj_from_obj (&ammo, 0);
  obj_to_room (ammo, tch->in_room);

  if (result == CRITICAL_MISS && !number (0, 1))
    {
      sprintf (buf + strlen (buf),
	       "\n\nThe missile recedes hopelessly from sight.");
      sprintf (buf2 + strlen (buf2),
	       "\n\nThe missile recedes hopelessly from sight.");
      sprintf (buf3 + strlen (buf3),
	       "\n\nThe missile recedes hopelessly from sight.");
      extract_obj (ammo);
    }

  char *out1, *out2;
  CHAR_DATA* rch = 0;
  reformat_string (buf, &out1);
  reformat_string (buf2, &out2);
  
  if (ch->room != tch->room)
    {
      // aggressor's room
      for (rch = ch->room->people; rch; rch = rch->next_in_room)
	{
	  send_to_char ("\n", rch);
	  send_to_char (out1, rch);
	}
    }
  
  // victim's room
  for (rch = tch->room->people; rch; rch = rch->next_in_room)
    {
      send_to_char ("\n", rch);
      if (rch == tch)
	send_to_char (out2, rch);
      else
	send_to_char (out1, rch);
    }
  
  mem_free (out1);
  mem_free (out2);

  if (damage > 0)
    {
      if (!IS_NPC (tch))
	{
	  tch->delay_ch = ch;
	  tch->delay_info1 = ammo->nVirtual;
	}
      if (ch->room != tch->room)
	ranged = true;
      wound_type = 3;
      if (wound_to_char
	  (tch, strike_location, (int) damage, wound_type, 0, poison, 0))
	{
	  if (ranged)
	    send_to_char ("\nYour target collapses, dead.\n", ch);
	  ch->ranged_enemy = NULL;
	  return;
	}
      if (!IS_NPC (tch))
	{
	  tch->delay_ch = NULL;
	  tch->delay_info1 = 0;
	}
      if (!ch->fighting && damage > 3)
	criminalize (ch, tch, tch->room->zone, CRIME_KILL);
    }

  npc_ranged_response (tch, ch);

}

void
do_fire (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *target = NULL;
  CHAR_DATA *tch = NULL;
  CHAR_DATA *fired_ch[25], *retal_ch = NULL;
  OBJ_DATA *obj;
  OBJ_DATA *ammo;
  OBJ_DATA *bow = NULL;
  OBJ_DATA *armor1 = NULL, *armor2 = NULL;
  OBJ_DATA *shield_obj = NULL;
  ROOM_DATA *room;
  AFFECTED_TYPE *af;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char buf3[MAX_STRING_LENGTH];
  char buffer[MAX_STRING_LENGTH];
  char strike_location[MAX_STRING_LENGTH];
  char *from_direction = NULL;
  char *p;
  int roll = 0, ranged = 0, num_fired_ch = 0, i = 0;
  int result = 0, location = 0, attack_mod = 0;
  float damage = 0;
  int dir = 0;
  bool switched_target = false, block = true;
  int poison = 0;
  int was_hidden = 0;

  if (!str_cmp (argument, "volley"))
    {
      if (!is_group_leader (ch))
	{
	  send_to_char ("You are not currently leading a group.\n", ch);
	  return;
	}
      for (tch = ch->room->people; tch; tch = tch->next_in_room)
	{
	  if ((tch == ch || are_grouped (tch, ch)) && tch->aiming_at
	      && is_archer (tch))
	    block = false;
	}
      if (block)
	{
	  send_to_char
	    ("No one in your group is currently preparing to fire.\n", ch);
	  return;
	}

      act ("You give your group the signal to fire.", false, ch, 0, 0,
	   TO_CHAR | _ACT_FORMAT);
      act ("$n gives the signal to fire!", false, ch, 0, 0,
	   TO_ROOM | _ACT_FORMAT);

      i = 0;

      for (tch = ch->room->people; tch; tch = tch->next_in_room)
	{
	  if (tch == ch)
	    continue;
	  if (!are_grouped (tch, ch))
	    continue;
	  if (!tch->aiming_at || !is_archer (tch))
	    continue;
	  send_to_room ("\n", ch->in_room);
	  if (num_fired_ch < 25)
	    fired_ch[num_fired_ch] = tch;
	  num_fired_ch++;
	  do_fire (tch, "", 1);
	}

      num_fired_ch = MIN (num_fired_ch, 24);

      fired_ch[num_fired_ch] = ch;

      // Leader doesn't need to fire if victim dies.

      if (!ch->aiming_at)
	return;

      send_to_room ("\n", ch->in_room);
    }
  else
    fired_ch[0] = ch;

  if (IS_SWIMMING (ch))
    {
      send_to_char ("You can't do that while swimming!\n", ch);
      return;
    }

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch))
    {
      send_to_char ("You cannot do this in an OOC area.\n", ch);
      return;
    }

  if(IS_SET(ch->room->room_flags, PEACE))
    {
      act ("Something prohibits you from taken such an action.", false, ch, 0, 0, TO_CHAR);
      return;
    }

  if(get_soma_affect(ch, SOMA_PLANT_VISIONS) && number(0,30) > GET_WIL(ch))
  {
    act ("You're paralyzed with fear, and unable to undertake such an action!", false, ch, 0, 0, TO_CHAR);
    return;
  }

  if (((bow = get_equip (ch, WEAR_PRIM)) && GET_ITEM_TYPE (bow) == ITEM_WEAPON
       && bow->o.weapon.use_skill == SKILL_SLING)
      || ((bow = get_equip (ch, WEAR_SEC))
	  && GET_ITEM_TYPE (bow) == ITEM_WEAPON
	  && bow->o.weapon.use_skill == SKILL_SLING))
    {
      skill_learn(ch, SKILL_SLING);
      fire_sling (ch, bow, argument);
      return;
    }
      
  if (!ch->aiming_at)
    {
      send_to_char ("You aren't aiming at anything.\n", ch);
      return;
    }

  if (!IS_NPC (ch->aiming_at)
      && ch->aiming_at->pc->create_state == STATE_DIED)
    {
      send_to_char ("You aren't aiming at anything.\n", ch);
      return;
    }

  if (!(bow = get_equip (ch, WEAR_BOTH)))
    {
      send_to_char ("You aren't wielding a ranged weapon!\n", ch);
      ch->aiming_at->targeted_by = NULL;
      ch->aiming_at = NULL;
      ch->aim = 0;
      return;
    }

	if (is_mounted(ch) && (bow->o.weapon.use_skill == SKILL_LONGBOW))
		{
			send_to_char ("You can't fire that while mounted!\n", ch);
			return;
		}
		
  if (!bow->loaded)
    {
      send_to_char ("Your weapon isn't loaded...\n", ch);
      ch->aiming_at->targeted_by = NULL;
      ch->aiming_at = NULL;
      ch->aim = 0;
      return;
    }

  if (!bow->contains)
    {
      send_to_char ("Your weapon isn't strung...\n", ch);
      ch->aiming_at->targeted_by = NULL;
      ch->aiming_at = NULL;
      ch->aim = 0;
      return;
    }  

  if (IS_SET (ch->plr_flags, NEW_PLAYER_TAG)
      && IS_SET (ch->room->room_flags, LAWFUL) && *argument != '!')
    {
      sprintf (buf,
	       "You are in a lawful area; you would likely be flagged wanted for assault. "
	       "To confirm, type \'#6fire !#0\', without the quotes.");
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  attack_mod -= ch->aim;
  target = ch->aiming_at;

  if(!cmd)
    target->targeted_by = NULL;

  ch->aiming_at = NULL;
  ch->aim = 0;

  if (IS_IMPLEMENTOR (target) && !IS_NPC (target) && !IS_NPC (ch))
    target = ch;

  if (ch->in_room != target->in_room) 
    {
      ranged = 1;
    }

  if (ranged)
    {
      if (!ch->delay_who)
	{
	  send_to_char ("You seem to have lost sight of your quarry.\n", ch);
	  return;
	}
      if (!strn_cmp ("north", ch->delay_who, strlen (ch->delay_who)))
	dir = 0;
      else if (!strn_cmp ("east", ch->delay_who, strlen (ch->delay_who)))
	dir = 1;
      else if (!strn_cmp ("south", ch->delay_who, strlen (ch->delay_who)))
	dir = 2;
      else if (!strn_cmp ("west", ch->delay_who, strlen (ch->delay_who)))
	dir = 3;
      else if (!strn_cmp ("up", ch->delay_who, strlen (ch->delay_who)))
	dir = 4;
      else if (!strn_cmp ("down", ch->delay_who, strlen (ch->delay_who)))
	dir = 5;
    }

  if (get_affect (ch, MAGIC_HIDDEN))
    {
      remove_affect_type (ch, MAGIC_HIDDEN);
      send_to_char ("You emerge from concealment and prepare to fire.\n\n",
		    ch);
      was_hidden = 1;
    }

  if (bow->o.weapon.use_skill == SKILL_SHORTBOW)
    skill_learn(ch, SKILL_SHORTBOW);
  else
    skill_learn(ch, SKILL_LONGBOW);

  /* ranged_projectile_echoes (ch, target, bow, ammo); */

  if (bow->o.weapon.use_skill == SKILL_SHORTBOW
      || bow->o.weapon.use_skill == SKILL_LONGBOW)
    {
      if (ranged)
	{
	  sprintf (buf,
		   "You release the bowstring, launching #2%s#0 %sward through the air toward #5%s#0.",
		   bow->loaded->short_description, ch->delay_who,
		   char_short (target));
	  sprintf (buf2,
		   "%s#0 releases the bowstring, launching #2%s#0 %sward through the air toward #5%s#0.",
		   char_short (ch), bow->loaded->short_description,
		   ch->delay_who, char_short (target));
	  *buf2 = toupper (*buf2);
          watched_action(ch, buf2, 0, 1);
	  sprintf (buffer, "#5%s", buf2);
	  sprintf (buf2, "%s", buffer);
	}
      else
	{
	  sprintf (buf2,
		   "%s#0 releases the bowstring, launching #2%s#0 through the air toward #5%s#0.",
		   char_short (ch), bow->loaded->short_description,
		   char_short (target));
	  *buf2 = toupper (*buf2);
          watched_action(ch, buf2, 0, 1);
	  sprintf (buffer, "#5%s", buf2);
	  sprintf (buf2, "%s", buffer);
	  sprintf (buf3,
		   "%s#0 releases the bowstring, launching #2%s#0 through the air toward you!",
		   char_short (ch), bow->loaded->short_description);
	  *buf3 = toupper (*buf3);
	  sprintf (buffer, "#5%s", buf3);
	  sprintf (buf3, "%s", buffer);
	  sprintf (buf,
		   "You release the bowstring, launching #2%s#0 through the air toward #5%s#0!",
		   bow->loaded->short_description, char_short (target));
	}
    }
  else
    {
      sprintf (buffer, "ARCHERY BUG? %s fires (VNUM: %d) from (VNUM: %d)",
	       ch->tname, bow->loaded->nVirtual, bow->nVirtual);
      system_log (buffer, true);
    }

  if (ranged)
    {
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      act (buf2, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      sprintf (buf3,
	       "You spy #2%s#0 as it arcs high overhead, streaking %sward.",
	       bow->loaded->short_description, ch->delay_who);
      reformat_string (buf3, &p);
      if (EXIT (ch, dir))
	room = vtor (EXIT (ch, dir)->to_room);
      else
	room = NULL;
      if (room && target->room != room)
	{
	  for (tch = room->people; tch; tch = tch->next_in_room)
	    //if (skill_use (tch, SKILL_SCAN, 0))
	      send_to_char (p, tch);
	  if (room->dir_option[dir])
	    room = vtor (room->dir_option[dir]->to_room);
	  else
	    room = NULL;
	  if (room && target->room != room)
	    {
	      for (tch = room->people; tch; tch = tch->next_in_room)
		//if (skill_use (tch, SKILL_SCAN, 0))
		  send_to_char (p, tch);
	      if (room->dir_option[dir])
		room = vtor (room->dir_option[dir]->to_room);
	      else
		room = NULL;
	      if (room && target->room != room)
		{
		  for (tch = room->people; tch; tch = tch->next_in_room)
		    //if (skill_use (tch, SKILL_SCAN, 0))
		      send_to_char (p, tch);
		}
	    }
	}
      mem_free (p);
    }
  else if (!ranged)
    {
      act (buf, false, ch, 0, target, TO_CHAR | _ACT_FORMAT);
      act (buf3, false, ch, 0, target, TO_VICT | _ACT_FORMAT);
      act (buf2, false, ch, 0, target, TO_NOTVICT | _ACT_FORMAT);
    }

  ammo = load_object (bow->loaded->nVirtual);
  ammo->count = 1;
  ammo->in_obj = NULL;
  ammo->carried_by = NULL;
  ammo->equiped_by = NULL;

  if (ranged && ch->delay_who)
    {
      if (!str_cmp (ch->delay_who, "north"))
	from_direction = str_dup ("the south");
      else if (!str_cmp (ch->delay_who, "east"))
	from_direction = str_dup ("the west");
      else if (!str_cmp (ch->delay_who, "south"))
	from_direction = str_dup ("the north");
      else if (!str_cmp (ch->delay_who, "west"))
	from_direction = str_dup ("the east");
      else if (!str_cmp (ch->delay_who, "up"))
	from_direction = str_dup ("below");
      else if (!str_cmp (ch->delay_who, "down"))
	from_direction = str_dup ("above");
      if (ch->delay_who && strlen (ch->delay_who) > 1)
	mem_free (ch->delay_who);
      ch->delay_who = NULL;
    }
  else
    from_direction = add_hash ("an indeterminate direction");

  attack_mod -= bow->o.od.value[2];

//if you are in combat, it is harder to fire a bow
  if (ch->fighting)
    {
      if (bow->o.weapon.use_skill == SKILL_LONGBOW)
	attack_mod += 30;
      else if (bow->o.weapon.use_skill == SKILL_SHORTBOW)
	attack_mod += 20;
    }

  // Firing at a target in the same room.
  if (!ch->delay_info1)
    {
      if (bow->o.weapon.use_skill == SKILL_LONGBOW)
	attack_mod += 20;
    }

   // Firing at a target one room away.
/*  else if (ch->delay_info1 == 1)
    {

    }
*/

  // Firing at a target two rooms away.
  else if (ch->delay_info1 == 2)
  {
    if (bow->o.weapon.use_skill == SKILL_SHORTBOW)
      attack_mod += 10;
  }

  if (is_mounted (ch))
    attack_mod +=15;

  // Warbows take a -30 to hit: not designed for shooting single targets.

  if(bow->o.weapon.use_skill == SKILL_LONGBOW)
  {
    attack_mod += 30;
  }
	
  // adjust target if it is grouped: much easier to hit big groups of people,
  // as archery is designed for.

  int group_size = 1;
  for (CHAR_DATA *grp = target->room->people; grp; grp = grp->next_in_room)
    {
      if (grp == ch || grp == target)
	{
	  continue;
	}

      if ((grp->following && grp->following == target->following)
	  || grp->following == target
	  || target->following == grp)
	{
          // each person in the group reduces the difficulty by 2%.
          if(group_size > 1)
            attack_mod -= 2;

	  group_size++;          
	}
    }

  // If they're in a group, there's a good chance that you'll be hitting
  // someone at random in the group, rather than anyone in particular.

  CHAR_DATA *original_target = target;
  if (group_size > 1 && number (1, 10) < (group_size + ch->delay_info1))
    {
      int new_target = number (1, group_size);
      group_size = 1;
      for (CHAR_DATA *grp = target->room->people; grp; grp = grp->next_in_room)
	{
	  if (grp == ch)
	    {
	      continue;
	    }

	  if ((grp->following && grp->following == target->following)
	      || grp->following == target
	      || target->following == grp)
	    {
	      if (group_size == new_target)
		{
		  target = grp;
		  if (target != original_target)
		    {
		      switched_target = true;
		    }
		  break;
		}
	      group_size++;
	    }
	}    
    }

  // Shooting at mounted folk is easier, and there's
  // a chance you'll hit the other person in the team.

  if(is_mounted(target))
  {
    attack_mod -= 10;
  
    if(number(0,1))
      target = target->mount;
      switched_target = true;
  }

    //Potentially changed target if orginal is being guarded, or in formation.

    for (CHAR_DATA *tch = target->room->people; tch; tch = tch->next_in_room)
    {
      if (tch->fighting)
        continue;
		
      af = get_affect (tch, MAGIC_GUARD);
      if (!af || (target->formation != 2 && target->formation != 3 && tch->formation != 1))
        continue;

      if ((CHAR_DATA *) af->a.spell.t == target 
          || (tch->formation == 1 && (target->formation == 2 || target->formation == 3)))
      {
      	//bonus given based on agility of the shieldman
	if (skill_use (tch, SKILL_AVERT, (39 - (tch->agi*3))))
	  target = tch;      	
      }
    }

  // If you're hidden, it's a lot harder for people to dodge your shot in calculate_missile_result... however, we removed our hidden flag at the start of this procedure so we could echo properly. Hence, the was_hidden flag.

  if(was_hidden)
  {
    was_hidden += number(15, 30);
  }
	
  result =
    calculate_missile_result (ch, bow->o.weapon.use_skill, attack_mod, target,
			      was_hidden, bow, ammo, NULL, &location, &damage, &poison);
  damage = (int) damage;

//you hit the opponent of your target if you miss and you miss a second  dex roll...ie, you hit your own people.
  if ((result == CRITICAL_MISS || result == MISS) && target->fighting
      && target->fighting != ch && number (1, 25) > ch->dex)
    {
      target = target->fighting;
      result =
	calculate_missile_result (ch, bow->o.weapon.use_skill, roll, target,
				  0, bow, ammo, NULL, &location, &damage, &poison);
      switched_target = true;
    }

  *buf = '\0';
  *buf2 = '\0';
  *buf3 = '\0';
  *buffer = '\0';

  if ((af = get_affect (target, MAGIC_AFFECT_PROJECTILE_IMMUNITY)))
    {
      sprintf (buf, "%s", spell_activity_echo (af->a.spell.sn, af->type));
      if (!*buf)
	sprintf (buf,
		 "\nThe is deflected harmlessly aside by some invisible force.");
      sprintf (buf2, "\n%s", buf);
      sprintf (buf3, "\n%s", buf);
      result = MISS;
      damage = 0;
    }
  else if (result == MISS)
    {
      if (ranged)
	{
	  sprintf (buf,
		   "%s#0 comes whirring through the air from %s, headed straight toward you!\n\nIt misses you completely.",
		   ammo->short_description, from_direction);
	  *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "%s", buffer);
	  sprintf (buf2,
		   "%s#0 comes whirring through the air from %s, headed straight toward #5%s#0.\n\nIt misses %s completely.",
		   ammo->short_description, from_direction,
		   char_short (target), HMHR (target));
	  *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "%s", buffer);
	  sprintf (buf3, "It misses %s completely.", HMHR (target));
	}
      else
	{
	  sprintf (buf, "\nIt misses %s completely.", HMHR (target));
	  sprintf (buf2, "\nIt misses you completely.");
	}
    }				/* Miss. */

  else if (result == CRITICAL_MISS)
    {
      if (ranged)
	{
	  sprintf (buf,
		   "%s#0 comes whirring through the air from %s, but it flies far wide of striking any target.",
		   ammo->short_description, from_direction);
	  *buf = toupper (*buf);
	  sprintf (buffer, "#2%s", buf);
	  sprintf (buf, "\n%s", buffer);
	  sprintf (buf2,
		   "%s#0 comes whirring through the air from %s, but it flies far wide of striking any target.",
		   ammo->short_description, from_direction);
	  *buf2 = toupper (*buf2);
	  sprintf (buffer, "#2%s", buf2);
	  sprintf (buf2, "\n%s", buffer);
	  sprintf (buf3, "The shot flies far wide of striking any target.");
	}
      else if (!ranged)
	{
	  sprintf (buf,
		   "It streaks off to one side, far wide of any desired target.");
	  sprintf (buf2,
		   "It streaks off to one side, far wide of any desired target.");
	}
    }				/* Critical miss. */

  else if (result == SHIELD_BLOCK)
    {
      shield_obj = get_equip (target, WEAR_SHIELD);
      if (ranged && shield_obj)
	{
	  sprintf (buf,
		   "%s#0 comes whirring through the air from %s, headed straight toward you!\n\nIt glances harmlessly off #2%s#0.",
		   ammo->short_description, from_direction,
		   obj_short_desc (shield_obj));
	  		*buf = toupper (*buf);
	  		sprintf (buffer, "#2%s", buf);
	  		sprintf (buf, "\n%s", buffer);
	  
	  sprintf (buf2,
		   "%s#0 comes whirring through the air from %s, headed straight toward #5%s#0.\n\nIt glances harmlessly off #2%s#0.",
		   ammo->short_description, from_direction,
		   char_short (target), obj_short_desc (shield_obj));
	  sprintf (buf3, "It glances harmlessly off #2%s#0.",
		   obj_short_desc (shield_obj));
		   	   		
		   		*buf2 = toupper (*buf2);
		   		sprintf (buffer, "#2%s", buf2);
	  			sprintf (buf2, "\n%s", buffer);
	}
			else if (ranged && !shield_obj)
	{
	  		sprintf (buf,
		   		"%s#0 comes whirring through the air from %s, headed straight toward you!\n\nIt glances harmlessly off something nearby.",
		   		ammo->short_description, from_direction);
	  		*buf = toupper (*buf);
	  		sprintf (buffer, "#2%s", buf);
	  		sprintf (buf, "\n%s", buffer);
	  			
	  		sprintf (buf2,
		   		"%s#0 comes whirring through the air from %s, headed straight toward #5%s#0.\n\nIt glances harmlessly off something nearby.",
		   		ammo->short_description, from_direction,
		   		char_short (target));
		   	*buf2 = toupper (*buf2);
		   	sprintf (buffer, "#2%s", buf2);
	  		sprintf (buf2, "\n%s", buffer);
	  			
	  		sprintf (buf3, "It glances harmlessly off something nearby.");
		   		
		   		
		   		
	}
	
      else if (!ranged && shield_obj)
	{
	  sprintf (buf, "It glances harmlessly off #2%s#0.",
		   obj_short_desc (shield_obj));
	  sprintf (buf2, "It glances harmlessly off #2%s#0.",
		   obj_short_desc (shield_obj));
	}
	
	else if (!ranged && !shield_obj)
	{
	  sprintf (buf, "It glances harmlessly off off something nearby.");
	  sprintf (buf2, "It glances harmlessly off something nearby.");
	}
    }

  else if (result == HIT || result == GLANCING_HIT || result == CRITICAL_HIT)
    {
      sprintf (strike_location, "%s", figure_location (target, location));

      if (ranged)
	{
	  if (result != CRITICAL_HIT && result != HIT)
	    {
	      sprintf (buf,
		       "%s#0 comes whirring through the air from %s, headed straight toward you!\n\nIt grazes you on the %s.",
		       ammo->short_description, from_direction,
		       expand_wound_loc (strike_location));
	      *buf = toupper (*buf);
	      sprintf (buffer, "#2%s", buf);
	      sprintf (buf, "\n%s", buffer);
	      sprintf (buf2,
		       "%s#0 comes whirring through the air from %s, headed straight toward #5%s#0.\n\nIt grazes %s on the %s.",
		       ammo->short_description, from_direction,
		       char_short (target), HMHR (target),
		       expand_wound_loc (strike_location));
	      *buf2 = toupper (*buf2);
	      sprintf (buffer, "#2%s", buf2);
	      sprintf (buf2, "\n%s", buffer);
	      if (switched_target)
		sprintf (buf3,
			 "It strays off-course, instead striking #5%s#0 on the %s!",
			 char_short (target),
			 expand_wound_loc (strike_location));
	      else
		sprintf (buf3, "It grazes %s on the %s.", HMHR (target),
			 expand_wound_loc (strike_location));
	    }
	  else
	    {
	      if (result == CRITICAL_HIT)
		sprintf (buf,
			 "%s#0 comes whirring through the air from %s, headed straight toward you!\n\nThe missile lodges deeply in your %s!",
			 ammo->short_description, from_direction,
			 expand_wound_loc (strike_location));
	      else
		sprintf (buf,
			 "%s#0 comes whirring through the air from %s, headed straight toward you!\n\nThe missile lodges in your %s!",
			 ammo->short_description, from_direction,
			 expand_wound_loc (strike_location));
	      *buf = toupper (*buf);
	      sprintf (buffer, "#2%s", buf);
	      sprintf (buf, "\n%s", buffer);
	      if (result == CRITICAL_HIT)
		sprintf (buf2,
			 "%s#0 comes whirring through the air from %s, headed straight toward #5%s#0.\n\nThe missile lodges deeply in %s %s!",
			 ammo->short_description, from_direction,
			 char_short (target), HSHR (target),
			 expand_wound_loc (strike_location));
	      else
		sprintf (buf2,
			 "%s#0 comes whirring through the air from %s, headed straight toward #5%s#0.\n\nThe missile lodges in %s %s!",
			 ammo->short_description, from_direction,
			 char_short (target), HSHR (target),
			 expand_wound_loc (strike_location));
	      *buf2 = toupper (*buf2);
	      sprintf (buffer, "#2%s", buf2);
	      sprintf (buf2, "\n%s", buffer);
	      if (switched_target)
		{
		  if (result == CRITICAL_HIT)
		    sprintf (buf3,
			     "It strays off-course, instead lodging deeply in #5%s#0's %s!",
			     char_short (target),
			     expand_wound_loc (strike_location));
		  else
		    sprintf (buf3,
			     "It strays off-course, instead lodging in #5%s#0's %s!",
			     char_short (target),
			     expand_wound_loc (strike_location));
		}
	      else
		{
		  if (result == CRITICAL_HIT)
		    sprintf (buf3, "The missile lodges deeply in %s %s!",
			     HSHR (target),
			     expand_wound_loc (strike_location));
		  else
		    sprintf (buf3, "The missile lodges in %s %s!",
			     HSHR (target),
			     expand_wound_loc (strike_location));
		}
	    }
	}
      else if (!ranged && result != CRITICAL_HIT && result != HIT)
	{
	  if (switched_target)
	    {
	      sprintf (buf, "It strays off-course, grazing #5%s#0 on the %s.",
		       char_short (target),
		       expand_wound_loc (strike_location));
	      sprintf (buf2, "It strays off-course, grazing you on the %s.",
		       expand_wound_loc (strike_location));
	    }
	  else
	    {
	      sprintf (buf, "It grazes %s on the %s.", HMHR (target),
		       expand_wound_loc (strike_location));
	      sprintf (buf2, "It grazes you on the %s.",
		       expand_wound_loc (strike_location));
	    }
	}
      else if (!ranged && (result == CRITICAL_HIT || result == HIT))
	{
	  if (switched_target)
	    {
	      sprintf (buf,
		       "The missile strays off-course, striking #5%s#0 instead, and lodging deeply in %s %s!",
		       char_short (target), HSHR (target),
		       expand_wound_loc (strike_location));
	      sprintf (buf2,
		       "The missile strays off-course, striking you instead, and lodging deeply in your %s!",
		       expand_wound_loc (strike_location));
	    }
	  else
	    {
	      if (result == CRITICAL_HIT)
		{
		  sprintf (buf, "The missile lodges deeply in %s %s!",
			   HSHR (target), expand_wound_loc (strike_location));
		  sprintf (buf2, "The missile lodges deeply in your %s!",
			   expand_wound_loc (strike_location));
		}
	      else
		{
		  sprintf (buf, "The missile lodges in %s %s!", HSHR (target),
			   expand_wound_loc (strike_location));
		  sprintf (buf2, "The missile lodges in your %s!",
			   expand_wound_loc (strike_location));
		}
	    }
	}
    }

  if (result == GLANCING_HIT || result == MISS || result == SHIELD_BLOCK)
    {
      if (!number (0, 9)
	  && ((armor1 || armor2 || target->armor) || result == MISS
	      || result == SHIELD_BLOCK))
	{
	  sprintf (buf + strlen (buf),
		   "\n\nThe missile shatters upon impact at its destination.");
	  sprintf (buf2 + strlen (buf2),
		   "\n\nThe missile shatters upon impact at its destination.");
	  sprintf (buf3 + strlen (buf3),
		   "\n\nThe missile shatters upon impact at its destination.");
	}
      else
	{
	  obj = load_object (ammo->nVirtual);
	  obj->deleted = 0;
	  obj->count = 1;
	  obj->obj_timer = 8;	/* 2 RL hours. */
	  obj->next_content = NULL;
	  obj_to_room (obj, target->in_room);
	}
      extract_obj (bow->loaded);
      bow->loaded = NULL;
    }
  else if (result == CRITICAL_HIT || result == HIT)
    {
      lodge_missile (target, ammo, strike_location);
      bow->loaded = NULL;
    }


  char *out1, *out2, *out3;
  CHAR_DATA* rch = 0;
  reformat_string (buf, &out1);
  reformat_string (buf2, &out2);
  reformat_string (buf3, &out3);
  
  if (ranged)
    {
      for (rch = ch->room->people; rch; rch = rch->next_in_room)
	{
	  send_to_char ("\n", rch);
	  send_to_char (out3, rch);
	}

      for (rch = target->room->people; rch; rch = rch->next_in_room)
	{
	  send_to_char ("\n", rch);
	  
	  if (rch == target)
	    send_to_char (out1, rch);
	  else
	    send_to_char (out2, rch);
	}
    }
  else
    {
      for (rch = target->room->people; rch; rch = rch->next_in_room)
	{
	  send_to_char ("\n", rch);
	  if (rch == target)
	    send_to_char (out2, rch);
	  else
	    send_to_char (out1, rch);
	}
    }
  mem_free (out1);
  mem_free (out2);
  mem_free (out3);

  if (bow->o.weapon.use_skill == SKILL_SHORTBOW ||
      bow->o.weapon.use_skill == SKILL_LONGBOW)
    {
      bow->location = WEAR_PRIM;
    }
  bow->loaded = NULL;

  if (!ch->fighting)
    criminalize (ch, target, target->room->zone, CRIME_KILL);

  ch->enemy_direction = NULL;

  if (from_direction)
    mem_free (from_direction);
  if (ch->delay_who)
    {
      mem_free (ch->delay_who);
      ch->delay_who = NULL;
    }

  if (damage)
    {
      if (!IS_NPC (target))
	{
	  target->delay_ch = ch;
	  target->delay_info1 = ammo->nVirtual;
	}
      if (wound_to_char (target, strike_location, (int) damage, 1, 0, poison, 0))
	{
	  if (ranged)
	    send_to_char ("\nYour target collapses, dead.\n", ch);
	  ch->ranged_enemy = NULL;
	  return;
	}
      if (!IS_NPC (target))
	{
	  target->delay_ch = NULL;
	  target->delay_info1 = 0;
	}
    }

  if(IS_NPC(target) && IS_RIDEE(target))
  {
    dump_rider(target->mount, 2);
  }

  if (!cmd)
    {				// Cmd > 0; volley-fire, delay NPC reaction to it.
      *buf = '\0';
      if (num_fired_ch > 1)
	retal_ch = fired_ch[number (0, num_fired_ch)];
      else
	retal_ch = ch;
      sprintf (buf, "Random retal_ch: %s\n", retal_ch->name);
      if (!retal_ch)
	retal_ch = ch;
      sprintf (buf + strlen (buf), "Failsafe retal_ch: %s\n", retal_ch->name);
      npc_ranged_response (target, retal_ch);
    }

  return;

}

void
npc_ranged_retaliation (CHAR_DATA * target, CHAR_DATA * ch)
{
  /* If the target's a NPC:
        if they're a mount, or on a mount, add treat to both the rider and the mount.
        otherwise, if the target is not a mount, not fleeing, not leaving, not entering, not fighting, 
        and not following, add_threat and charge the shooter.
  */

  if (IS_NPC (target) && !target->desc)
    {
      // add_threat (target, ch, 7); -- If this is here, then evaluate_threat means the NPC charges irregardless.
      if (IS_SET (target->act, ACT_MOUNT) && target->mount)
      {
	add_threat (target->mount, ch, 7);
        add_threat (target, ch, 7);
      } 
      else if (!IS_SET (target->act, ACT_MOUNT) && !target->fighting &&
	       !IS_SET (target->flags, FLAG_ENTERING) &&
	       !IS_SET (target->flags, FLAG_LEAVING) &&
	       !IS_SET (target->flags, FLAG_FLEE) && !(target->following))
	{
	  do_stand (target, "", 0);
	  target->speed = 4;
          add_threat (target, ch, 7);
	  npc_charge (target, ch);
	}
      else if (IS_SET (target->act, ACT_MOUNT)
	       && target->mount
	       && !target->fighting
	       && !IS_SET (target->flags, FLAG_ENTERING)
	       && !IS_SET (target->flags, FLAG_LEAVING)
	       && !IS_SET (target->flags, FLAG_FLEE)
	       // todo: add wildness factor to mob based on race and mod penalty
	       && !skill_use (target->mount, SKILL_RIDE, 15))
	{
	  do_stand (target, "", 0 ) ;
	  target->speed = 4 ;
          add_threat (target, ch, 7);
	  npc_charge (target, ch);
	}
    }
}

// Function to determine whether or not mob will retaliate against ranged
// attacks if it isn't flagged AGGRO or ENFORCER.

int
is_combat_ready (CHAR_DATA * ch)
{
  if (!ch || !IS_NPC (ch))
    return 0;

  if (IS_SET (ch->act, ACT_WILDLIFE))
    {
      if (IS_SET (ch->act, ACT_PREY))
	return 0;
      if (ch->max_hit < 30)
	return 0;
      if (ch->mob->damsizedice < 4)
	return 0;
      return 1;
    }

  return 0;
}

// To do: add in function to check if NPC is combat-ready (e.g. has skills/gear
// or is big wildlife) even if it isn't flagged aggro or enforcer, so it will
// retaliate instead of fleeing.
//      Function: npc_ranged_response
// 
//      Called By
//              do_throw - IF a missile was released UNLESS the victim is killed.
//              fire_sling - IF a missile was released UNLESS the victim is killed.
//              do_fire - IF a missile was released UNLESS the victim is killed.

void
npc_ranged_response (CHAR_DATA * npc, CHAR_DATA * retal_ch)
{
  CHAR_DATA *tch;

  if (!npc || !retal_ch || !IS_NPC (npc) || npc->desc
      || IS_SET (npc->act, ACT_VEHICLE) || IS_SET (npc->act, ACT_PASSIVE))
    return;

  // Deal with the specified NPC's reaction first.
  // Evade -IF- morale is broken 
  //       -OR- the attacker is not visible 
  //       -OR- the victim is not aggro, an enforcer, combat ready, or following 
  // Else Retaliate
// Unless they are under cover. IF undercover, do nothing (ie. stay undercover)

  if (morale_broken (npc)
      || (!IS_SET (npc->act, ACT_ENFORCER)
	  && !IS_SET (npc->act, ACT_AGGRESSIVE)
	  && !is_combat_ready (npc)
	  && !npc->following) 
      || !CAN_SEE (npc, retal_ch))

		{
			npc_evasion (npc, track (npc, retal_ch->in_room));
		}
	else
		{
		if (!under_cover(npc))
      npc_ranged_retaliation (npc, retal_ch);
    }

  // Deal with any of the NPC's clanmates in the same room second.

  for (tch = npc->room->people; tch; tch = tch->next_in_room)
    {

      if (!IS_NPC (tch) || !is_brother (npc, tch))
	continue;
      
      if(npc == tch)
        continue;


      if (morale_broken (tch)
	  || (!IS_SET (tch->act, ACT_ENFORCER)
	      && !IS_SET (tch->act, ACT_AGGRESSIVE)
	      && !is_combat_ready (tch)
	      && !tch->following) 
	  || !CAN_SEE (tch, retal_ch))

			{
					npc_evasion (npc, track (npc, retal_ch->in_room));
			}
		
		else
			{
			if (!under_cover(npc))
	  npc_ranged_retaliation (tch, retal_ch);
	}
    }

}

int
has_been_sighted (CHAR_DATA * ch, CHAR_DATA * target)
{
  SIGHTED_DATA *sighted;

  if (!ch || !target)
    return 0;

  if (!IS_MORTAL (ch))
    return 1;

  if (IS_NPC (ch) && !ch->desc)
    return 1;			/* We know non-animated NPCs only acquire targets via SCANning; */
  /* don't need anti-twink code for them. */

  for (sighted = ch->sighted; sighted; sighted = sighted->next)
    {
      if (sighted->target == target)
	return 1;
    }

  return 0;
}

void
do_destring (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *ptrBow = NULL;
  OBJ_DATA *ptrOffHand = NULL;
  char strBuf[AVG_STRING_LENGTH * 4] = "";

  if (!(ptrBow = get_equip (ch, WEAR_PRIM))
      || (ptrBow->o.weapon.use_skill != SKILL_SHORTBOW
	  && ptrBow->o.weapon.use_skill != SKILL_LONGBOW)
      || ptrBow->loaded
      || get_equip (ch, WEAR_SEC) || get_equip (ch, WEAR_BOTH)
      || ptrBow->o.od.value[5] == 0
      || !ptrBow->contains)
    {
      send_to_char
  ("You must first be wielding a strung, unloaded bow, and only that weapon.\n", ch);
      return;
    }

  if (is_mounted(ch) &&
		(ptrBow->o.weapon.use_skill == SKILL_LONGBOW))
  {
    send_to_char ("You can't destring that while mounted!\n", ch);
    return;
  }

  ptrOffHand = (ptrBow == ch->right_hand) ? ch->left_hand : ch->right_hand;

  if (ptrOffHand)
  {
    send_to_char
    ("Your off-hand needs to be empty.\n", ch);
    return;
  }

  sprintf (strBuf, "You strain as you bend #2%s#0 in order to release the string.",
	       ptrBow->short_description);
  act (strBuf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
  sprintf (strBuf, "$n bends #2%s#0 as $e attempts to release the string.",
	       ptrBow->short_description);
  act (strBuf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);

  ch->delay = 6;
  ch->delay_type = DEL_DESTRING_BOW;
  ch->delay_info1 = ptrBow->nVirtual;
}

void
delayed_destring (CHAR_DATA *ch)
{
  OBJ_DATA * bow;
  OBJ_DATA * string;
  char buf1[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  if (!
      ((bow = get_equip (ch, WEAR_PRIM))
       || (bow = get_equip (ch, WEAR_BOTH))) || bow->nVirtual != ch->delay_info1)
    {
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      send_to_char ("Having misplaced your weapon, you cease destringing it.\n",
		    ch);
      return;
    }

  if(!(string = bow->contains))
  {
    send_to_char ("You find that the bow has already been destringed!\n", ch);
  }

  sprintf (buf1, "You destring #2%s#0, ", bow->short_description);
  sprintf (buf2, "$n destrings #2%s#0.", bow->short_description);

  if(bow->o.od.value[5] == 2)
  {
    sprintf(buf1 + strlen(buf1), "discarding the snapped bowstring.");
  }
  else if(bow->o.od.value[5] == 1)
  {
    sprintf(buf1 + strlen(buf1), "successfully recovering #2%s#0.", string->short_description);    
    string = load_object(bow->contains->nVirtual);
    string->in_obj = NULL;
    string->carried_by = NULL;
    obj_to_char (string, ch);
  }

  act (buf1, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
  act (buf2, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);

  bow->contains = NULL;
  ch->delay_type = 0;
  ch->delay_info1 = 0;
  bow->o.od.value[5] = 0;
}

void
do_string (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *ptrBow = NULL;
  OBJ_DATA *ptrOffHand = NULL;
  char strBuf[AVG_STRING_LENGTH * 4] = "";

  if (!(ptrBow = get_equip (ch, WEAR_PRIM))
      || (ptrBow->o.weapon.use_skill != SKILL_SHORTBOW
	  && ptrBow->o.weapon.use_skill != SKILL_LONGBOW)
      || (ptrBow->loaded)
      || get_equip (ch, WEAR_SEC) || get_equip (ch, WEAR_BOTH)
      || ptrBow->o.od.value[5] == 1
      || ptrBow->contains)
    {
      send_to_char
  ("You must first be wielding an unstrung, unloaded bow, and only that weapon.\n", ch);
      return;
    }

  if (is_mounted(ch) &&
		(ptrBow->o.weapon.use_skill == SKILL_LONGBOW))
  {
    send_to_char ("You can't string that while mounted!\n", ch);
    return;
  }

  ptrOffHand = (ptrBow == ch->right_hand) ? ch->left_hand : ch->right_hand;

  if (!ptrOffHand || ptrOffHand && GET_ITEM_TYPE(ptrOffHand) != ITEM_BOW_STRING)
  {
    send_to_char
    ("Your off-hand needs to be holding a bowstring.\n", ch);
    return;
  }

  sprintf (strBuf, "You strain as you bend #2%s#0 in order to string it.",
	       ptrBow->short_description);
  act (strBuf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
  sprintf (strBuf, "$n bends #2%s#0 as $e attempts to string the weapon.",
	       ptrBow->short_description);
  act (strBuf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);

  ch->delay = 16;
  ch->delay_type = DEL_STRING_BOW;
  ch->delay_info1 = ptrBow->nVirtual;
  ch->delay_info2 = ptrOffHand->nVirtual;
}

void delayed_string(CHAR_DATA *ch)
{
  OBJ_DATA *bow;
  OBJ_DATA *string;
  char strBuf[AVG_STRING_LENGTH * 4] = "";

  if (!
      ((bow = get_equip (ch, WEAR_PRIM))
       || (bow = get_equip (ch, WEAR_BOTH))) || bow->nVirtual != ch->delay_info1)
    {
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      send_to_char ("Having removed your weapon, you cease stringing it.\n",
		    ch);
      return;
    }

  string = NULL;

  if (ch->right_hand && ch->right_hand->nVirtual == ch->delay_info2)
  {
    string = ch->right_hand;
  }
  else if (ch->left_hand && ch->left_hand->nVirtual == ch->delay_info2)
  {
    string = ch->left_hand;
  }

  if(!string)
  {
    ch->delay = 0;
    ch->delay_type = 0;
    ch->delay_info1 = 0;
    ch->delay_info2 = 0;
    send_to_char("You need to be holding a string to properly string a bow!\n", ch);
  }

  sprintf (strBuf, "You successfully string #2%s#0 with #2%s#0.",
	       bow->short_description, string->short_description);
  act (strBuf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
  sprintf (strBuf, "$n successfully strings #2%s#0 with #2%s#0.",
	       bow->short_description, string->short_description);
  act (strBuf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);

  bow->o.od.value[5] = 1;
  bow->contains = load_object(string->nVirtual);
  bow->contains->in_obj = bow;

  ch->delay_type = 0;
  ch->delay_info1 = 0;

  obj_from_char (&string, 0);
  extract_obj (string);

  ch->delay = 0;
  ch->delay_info1 = 0;
  ch->delay_info2 = 0;
  ch->delay_type = 0;
}

void
do_restring (CHAR_DATA * ch, char *argument, int cmd)
{

}

void
delayed_restring (CHAR_DATA * ch)
{

}

void
do_aim (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *bow;
  CHAR_DATA *target = NULL;
  CHAR_DATA *tch = NULL;
  ROOM_DATA *room = NULL;
  ROOM_DIRECTION_DATA *exit = NULL;
  char arg1[MAX_STRING_LENGTH];
  char arg2[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  char buffer[MAX_STRING_LENGTH];
  int ranged = 0, dir = 0;

  if (IS_SWIMMING (ch))
    {
      send_to_char ("You can't do that while swimming!\n", ch);
      return;
    }

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch))
    {
      send_to_char ("You cannot do this in an OOC area.\n", ch);
      return;
    }

  if(IS_SET(ch->room->room_flags, PEACE))
    {
      act ("Something prohibits you from taken such an action.", false, ch, 0, 0, TO_CHAR);
      return;
    }

  if(get_soma_affect(ch, SOMA_PLANT_VISIONS) && number(0,30) > GET_WIL(ch))
  {
    act ("You're paralyzed with fear, and unable to undertake such an action!", false, ch, 0, 0, TO_CHAR);
    return;
  }

  if (ch->delay_type == DEL_LOAD_WEAPON)
    {
      return;
    }

  argument = one_argument (argument, arg1);
  argument = one_argument (argument, arg2);

  if (!*arg1)
    {
      send_to_char ("Usage: aim (<direction>) <target>\n", ch);
      return;
    }

  if (!((bow = get_equip (ch, WEAR_BOTH))
	|| (bow = get_equip (ch, WEAR_PRIM)))
      || (bow->o.weapon.use_skill != SKILL_SHORTBOW
	  && bow->o.weapon.use_skill != SKILL_LONGBOW))
    {
      send_to_char ("You aren't wielding a ranged weapon.\n", ch);
      return;
    }

  if (!bow->loaded)
    {
      send_to_char ("Your weapon isn't loaded.\n", ch);
      return;
    }
/*
  if (bow->o.weapon.use_skill == SKILL_CROSSBOW
      && ch->right_hand && ch->left_hand)
    {
      send_to_char ("You need two hands to aim a crossbow.\n", ch);
      return;
    }
*/

  if ((!strn_cmp ("east", arg1, strlen (arg1)) ||
       !strn_cmp ("west", arg1, strlen (arg1)) ||
       !strn_cmp ("north", arg1, strlen (arg1)) ||
       !strn_cmp ("south", arg1, strlen (arg1)) ||
       !strn_cmp ("up", arg1, strlen (arg1)) ||
       !strn_cmp ("down", arg1, strlen (arg1))) && *arg2)
    ranged = 1;

  if (!ranged)
    {
      if (is_sunlight_restricted (ch, room))
	return;
	
      if (!(target = get_char_room_vis (ch, arg1)))
	{
	  send_to_char ("Who did you want to target?\n", ch);
	  return;
	}
      if (target == ch)
	{
	  send_to_char
	    ("Now, now, now... things can't be THAT bad, can they?\n", ch);
	  return;
	}
      if (IS_SET (target->act, ACT_VEHICLE))
      {
        sprintf (buf, "And what good will that do to hurt $N?");
        act (buf, false, ch, 0, target, TO_CHAR | _ACT_FORMAT);
        return;
      }
      if (get_affect (target, MAGIC_HIDDEN) && IS_MORTAL (ch))
	{
	  send_to_char
	    ("Due to your target's cover, you cannot get a clear shot.\n",
	     ch);
	  return;
	}
      sprintf (buf, "You begin to carefully take aim at #5%s#0.",
	       char_short (target));
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      sprintf (buf,
	       "%s#0 begins to carefully take aim at #5%s#0 with #2%s#0.",
	       char_short (ch), char_short (target), bow->short_description);
      *buf = toupper (*buf);
      sprintf (buffer, "#5%s", buf);
      for (tch = ch->room->people; tch; tch = tch->next_in_room)
	{
	  if (tch == ch)
	    continue;
	  if (tch == target)
	    continue;
	  if (CAN_SEE (tch, ch))
	    act (buffer, false, ch, 0, tch, TO_VICT | _ACT_FORMAT);
	}

      sprintf (buf, "%s#0 begins to carefully take aim at you with #2%s#0!",
	       char_short (ch), bow->short_description);
      *buf = toupper (*buf);
      sprintf (buffer, "#5%s", buf);
      if (CAN_SEE (target, ch))
	act (buffer, false, ch, 0, target, TO_VICT | _ACT_FORMAT);
      else if (real_skill (target, SKILL_DANGER_SENSE))
	{
	  if (skill_use (target, SKILL_DANGER_SENSE, 0))
	    send_to_char
	      ("The back of your neck prickles, as if you are being watched closely.\n",
	       target);
	}
/*
      if (bow->o.weapon.use_skill == SKILL_CROSSBOW)
	{
	  bow->location = WEAR_BOTH;
	}
*/
      ch->aiming_at = target;
      target->targeted_by = ch;
      notify_guardians (ch, target, 3);
      return;
    }

  else if (ranged)
    {
   
      if (!strn_cmp ("north", arg1, strlen (arg1)))
				dir = 0;
      else if (!strn_cmp ("east", arg1, strlen (arg1)))
				dir = 1;
      else if (!strn_cmp ("south", arg1, strlen (arg1)))
				dir = 2;
      else if (!strn_cmp ("west", arg1, strlen (arg1)))
				dir = 3;
      else if (!strn_cmp ("up", arg1, strlen (arg1)))
				dir = 4;
      else if (!strn_cmp ("down", arg1, strlen (arg1)))
				dir = 5;
				 	

      if (!EXIT (ch, dir))
	{
	  send_to_char ("There isn't an exit in that direction.\n", ch);
	  return;
	}

      ch->delay_who = str_dup (dirs[dir]);

      room = vtor (EXIT (ch, dir)->to_room);

      exit = EXIT (ch, dir);
      if (exit 
      	&& IS_SET (exit->exit_info, EX_CLOSED)
      	&& !IS_SET (exit->exit_info, EX_ISGATE))
	{
	  send_to_char ("Your view is blocked.\n", ch);
	  return;
	}

      if (is_sunlight_restricted (ch, room))
	return;

      if (IS_SET (room->room_flags, STIFLING_FOG))
	{
	  send_to_char
	    ("The stiflingly heavy fog in that area thwarts any such attempt.\n",
	     ch);
	  return;
	}

      if (!(target = get_char_room_vis2 (ch, room->nVirtual, arg2))
	  || !has_been_sighted (ch, target))
	{
	  exit = room->dir_option[dir];
	  if (!exit)
	    {
	      send_to_char ("You don't see them within range.\n", ch);
	      return;
	    }
		if (exit 
      	&& IS_SET (exit->exit_info, EX_CLOSED)
      	&& !IS_SET (exit->exit_info, EX_ISGATE))	    {
	      send_to_char ("Your view is blocked.\n", ch);
	      return;
	    }
	  if (room->dir_option[dir])
	    room = vtor (room->dir_option[dir]->to_room);
	  else
	    room = NULL;
	  if (bow->o.od.value[1] == 3 || !room)
	    {			/* Sling, eventually. */
	      send_to_char ("You don't see them within range.\n", ch);
	      return;
	    }
	  if (is_sunlight_restricted (ch, room))
	    return;
	  if (IS_SET (room->room_flags, STIFLING_FOG))
	    {
	      send_to_char
		("The stiflingly heavy fog in that area thwarts any such attempt.\n",
		 ch);
	      return;
	    }
	  if (!(target = get_char_room_vis2 (ch, room->nVirtual, arg2))
	      || !has_been_sighted (ch, target))
	    {
	      exit = room->dir_option[dir];
	      if (!exit)
		{
		  send_to_char ("You don't see them within range.\n", ch);
		  return;
		}
		if (exit 
      	&& IS_SET (exit->exit_info, EX_CLOSED)
      	&& !IS_SET (exit->exit_info, EX_ISGATE))		{
		  send_to_char ("Your view is blocked.\n", ch);
		  return;
		}
	      if (room->dir_option[dir])
		room = vtor (room->dir_option[dir]->to_room);
	      else
		room = NULL;
	      if (bow->o.od.value[1] == 1 || !room)
		{		/* Crossbows and shortbows; two-room range. */
		  send_to_char ("You don't see them within range.\n", ch);
		  return;
		}
	      if (is_sunlight_restricted (ch, room))
		return;
	      if (IS_SET (room->room_flags, STIFLING_FOG))
		{
		  send_to_char
		    ("The stiflingly heavy fog in that area thwarts any such attempt.\n",
		     ch);
		  return;
		}
	      if (!(target = get_char_room_vis2 (ch, room->nVirtual, arg2))
		  || !has_been_sighted (ch, target))
		{
		  exit = room->dir_option[dir];
		  if (!exit)
		    {
		      send_to_char ("You don't see them within range.\n", ch);
		      return;
		    }
			if (exit 
      	&& IS_SET (exit->exit_info, EX_CLOSED)
      	&& !IS_SET (exit->exit_info, EX_ISGATE))		    {
		      send_to_char ("Your view is blocked.\n", ch);
		      return;
		    }
		  send_to_char ("You don't see them within range.\n", ch);
		  return;
		}
	      else
		ch->delay_info1 = 3;
	    }
	  else
	    ch->delay_info1 = 2;
	}
      if (!target || !CAN_SEE (ch, target) || !has_been_sighted (ch, target))
	{
	  send_to_char ("You don't see them within range.\n", ch);
	  return;
	}
      if (target == ch)
	{
	  send_to_char
	    ("Now, now, now... things can't be THAT bad, can they?\n", ch);
	  return;
	}
      if (get_affect (target, MAGIC_HIDDEN) && IS_MORTAL (ch))
	{
	  send_to_char
	    ("Due to your target's cover, you cannot get a clear shot.\n",
	     ch);
	  return;
	}
	
	/*** add in check to see if the target has taken cover **/
		if ((dir == 0 && get_affect (target, AFFECT_COVER_SOUTH))||
				(dir == 1 && get_affect (target, AFFECT_COVER_WEST))||
				(dir == 2 && get_affect (target, AFFECT_COVER_NORTH))||
				(dir == 3 && get_affect (target, AFFECT_COVER_EAST))||
				(dir == 4 && get_affect (target, AFFECT_COVER_DOWN))||
				(dir == 5 && get_affect (target, AFFECT_COVER_UP)))
			{
			send_to_char
			("Your target is under cover and you do not have a clear shot.\n",
			ch);
			}
			
      sprintf (buf, "You begin to carefully take aim at #5%s#0.",
	       char_short (target));
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      sprintf (buf,
	       "%s#0 turns %sward, taking aim at a distant target with #2%s#0.",
	       char_short (ch), dirs[dir], bow->short_description);
      *buf = toupper (*buf);
      ch->ranged_enemy = target;
      sprintf (buffer, "#5%s", buf);
      for (tch = ch->room->people; tch; tch = tch->next_in_room)
	{
	  if (tch == ch)
	    continue;
	  if (tch == target)
	    continue;
	  if (CAN_SEE (tch, ch))
	    act (buffer, false, ch, 0, tch, TO_VICT | _ACT_FORMAT);
	}
      if (real_skill (target, SKILL_DANGER_SENSE))
	{
	  if (skill_use (target, SKILL_DANGER_SENSE, 0))
	    send_to_char
	      ("The back of your neck prickles, as if you are being watched closely.\n",
	       target);
	}
/*
      if (bow->o.weapon.use_skill == SKILL_CROSSBOW)
	{
	  bow->location = WEAR_BOTH;
	}
*/
      ch->aiming_at = target;
      target->targeted_by = ch;
      
      if (dir == 0)
				target->cover_from_dir = 2;
			else if (dir == 1)
				target->cover_from_dir = 3;
      else if (dir == 2)
				target->cover_from_dir = 0;
      else if (dir == 3)
				target->cover_from_dir = 1;
      else if (dir == 4)
				target->cover_from_dir = 5;
      else if (dir == 5)
				target->cover_from_dir = 4;		
      
      notify_guardians (ch, target, 3);
      return;
    }
}

void
do_unload (CHAR_DATA * ch, char *argument, int cmd)
{
  OBJ_DATA *bow, *arrow, *quiver;
  int i;
  char *error;
  char buf[MAX_STRING_LENGTH];
  char buffer[MAX_STRING_LENGTH];

  bow = get_bow (ch);

  if (!bow)
    {
      send_to_char ("What did you wish to unload?\n", ch);
      return;
    }

  if (!bow->loaded)
    {
      send_to_char ("That isn't loaded.\n", ch);
      return;
    }

  if (!(arrow = bow->loaded))
    {
      send_to_char ("That isn't loaded.\n", ch);
      return;
    }

  sprintf (buf, "%s#0 unloads #2%s#0.", char_short (ch),
	   bow->short_description);
  *buf = toupper (*buf);
  sprintf (buffer, "#5%s", buf);
  act (buffer, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);

  arrow = load_object (bow->loaded->nVirtual);
  arrow->in_obj = NULL;
  arrow->carried_by = NULL;

  for (i = 0; i < MAX_WEAR; i++)
    {
      if (!(quiver = get_equip (ch, i)))
	continue;
      if (GET_ITEM_TYPE (quiver) == ITEM_QUIVER
	  && get_obj_in_list_num (arrow->nVirtual, quiver->contains)
	  && can_obj_to_container (arrow, quiver, &error, 1))
	break;
    }

  if (!quiver)
    {
      for (i = 0; i < MAX_WEAR; i++)
	{
	  if (!(quiver = get_equip (ch, i)))
	    continue;
	  if (GET_ITEM_TYPE (quiver) == ITEM_QUIVER
	      && can_obj_to_container (arrow, quiver, &error, 1))
	    break;
	  else
	    quiver = NULL;
	}
    }

  if (bow->o.weapon.use_skill == SKILL_SHORTBOW ||
      bow->o.weapon.use_skill == SKILL_LONGBOW)
    {
      bow->location = WEAR_PRIM;
    }

  if (quiver)
    {
      send_to_char ("\n", ch);
      sprintf (buf, "You unload #2%s#0, and slide #2%s#0 into #2%s#0.",
	       obj_short_desc (bow), obj_short_desc (arrow),
	       obj_short_desc (quiver));
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      obj_to_obj (arrow, quiver);
    }
  else if (!quiver)
    {
      sprintf (buf, "\nYou unload #2%s#0.\n", obj_short_desc (bow));
      send_to_char (buf, ch);
      obj_to_char (arrow, ch);
    }
  bow->loaded = NULL;
}

void
delayed_load (CHAR_DATA * ch)
{
  OBJ_DATA *bow;
  OBJ_DATA *ammo;
  OBJ_DATA *quiver;
  char buf[MAX_STRING_LENGTH];
  int i;

  if (!
      ((bow = get_equip (ch, WEAR_PRIM))
       || (bow = get_equip (ch, WEAR_BOTH))))
    {
      ch->delay_who = NULL;
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      send_to_char ("Having removed your weapon, you cease loading it.\n",
		    ch);
      return;
    }

  if (bow->o.weapon.use_skill != SKILL_SHORTBOW &&
      bow->o.weapon.use_skill != SKILL_LONGBOW)
    {
      ch->delay_who = NULL;
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      send_to_char
	("Having switched your weapon, you cease loading your bow.\n", ch);
      return;
    }

  ammo = NULL;

  if (ch->delay_info1 < 0)
    {

      if (ch->right_hand && ch->right_hand->nVirtual == ch->delay_info2)
	{
	  ammo = ch->right_hand;
	}
      else if (ch->left_hand && ch->left_hand->nVirtual == ch->delay_info2)
	{
	  ammo = ch->left_hand;
	}
    }
  else
    {
      if (ch->left_hand && ch->right_hand)
	{
	  send_to_char
	    ("Having taken another object in hand, you cease loading your weapon.\n",
	     ch);
	  return;
	}

      for (i = 0; i < MAX_WEAR; i++)
	{

	  if (!(quiver = get_equip (ch, i)))
	    {

	      continue;
	    }

	  if (quiver->nVirtual != ch->delay_info1
	      || !get_obj_in_list_num (ch->delay_info2, quiver->contains))
	    {

	      continue;
	    }

	  if (GET_ITEM_TYPE (quiver) == ITEM_QUIVER)
	    {

	      for (ammo = quiver->contains; ammo; ammo = ammo->next_content)
		{

		  if (GET_ITEM_TYPE (ammo) == ITEM_MISSILE
		      && ammo->nVirtual == ch->delay_info2)
		    {

		      break;
		    }
		}
	      if (ammo)
		{

		  break;
		}
	    }
	}

    }
  if (!ammo)
    {
      send_to_char
	("Having lost your ammunition, you cease loading your weapon.\n", ch);
      return;
    }

  sprintf (buf, "You finish loading #2%s#0.\n", bow->short_description);
  send_to_char (buf, ch);

  sprintf (buf, "$n finishes loading #2%s#0.", bow->short_description);
  act (buf, true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);

  bow->loaded = load_object (ammo->nVirtual);

  bow->location = WEAR_BOTH;

  if (ch->delay_info1 >= 0)
    {
      obj_from_obj (&ammo, 1);
    }
  else
    {
      obj_from_char (&ammo, 0);
      extract_obj (ammo);
    }
  ch->delay_who = NULL;
  ch->delay = 0;
  ch->delay_info1 = 0;
  ch->delay_info2 = 0;
  ch->delay_type = 0;

}


void
do_hit (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *victim, *tch;
  OBJ_DATA *obj;
  int i, agi_diff = 0;
  char buf[MAX_STRING_LENGTH];
  char original[MAX_STRING_LENGTH];
  SECOND_AFFECT *sa = NULL;
  bool found = false;

  sprintf (original, "%s", argument);

  /* cmd = 0 if hit,
     cmd = 1 if kill */

  if (IS_SWIMMING (ch))
    {
      send_to_char ("You can't do that while swimming!\n", ch);
      return;
    }

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch))
    {
      send_to_char ("You cannot do this in an OOC area.\n", ch);
      return;
    }

  if (is_room_affected (ch->room->affects, MAGIC_ROOM_CALM))
    {
      act ("Try as you might, you simply cannot muster the will to break "
	   "the peace that pervades the area.", false, ch, 0, 0, TO_CHAR);
      return;
    }

  if(IS_SET(ch->room->room_flags, PEACE))
    {
      act ("Something prohibits you from taken such an action.", false, ch, 0, 0, TO_CHAR);
      return;
    }

  if(get_soma_affect(ch, SOMA_PLANT_VISIONS) && number(0,30) > GET_WIL(ch))
  {
    act ("You're paralyzed with fear, and unable to undertake such an action!", false, ch, 0, 0, TO_CHAR);
    return;
  }

  if (IS_SET (ch->flags, FLAG_PACIFIST))
    {
      send_to_char ("Remove your pacifist flag, first...\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if ((obj = get_equip (ch, WEAR_BOTH)))
    {
      if (obj->o.weapon.use_skill == SKILL_LONGBOW ||
	  obj->o.weapon.use_skill == SKILL_SHORTBOW ||
	  obj->o.weapon.use_skill == SKILL_SLING ||
	  obj->o.weapon.use_skill == SKILL_THROWN)
	{
	  send_to_char ("You can't use that in melee combat!\n", ch);
	  return;
	}
    }

  if (get_affect (ch, MAGIC_AFFECT_PARALYSIS))
    {
      send_to_char ("You are paralyzed and unable to fight!\n\r", ch);
      return;
    }

  if (get_affect (ch, MAGIC_AFFECT_FEAR))
    {
      send_to_char ("You are too afraid to fight!\n\r", ch);
      return;
    }

  if (get_affect (ch, AFFECT_GROUP_RETREAT))
    {
      send_to_char ("You stop trying to retreat.\n", ch);
      remove_affect_type (ch, AFFECT_GROUP_RETREAT);
    }

  if (IS_SUBDUER (ch))
    {
      act ("You can't attack while you have $N subdued.",
	   false, ch, 0, ch->subdue, TO_CHAR);
      return;
    }

  if (IS_SET (ch->plr_flags, NEW_PLAYER_TAG)
      && IS_SET (ch->room->room_flags, LAWFUL) && *argument != '!')
    {
      sprintf (buf,
	       "You are in a lawful area; you would likely be flagged wanted for assault. "
	       "To confirm, type \'#6hit %s !#0\', without the quotes.",
	       original);
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  if (IS_SET (ch->room->room_flags, BRAWL) && IS_SET (ch->room->room_flags, LAWFUL) 
      && *argument != '!' && has_weapon(ch))
    {
      sprintf (buf, "You are in a lawful, but brawable area area; if you hit this person with that weapon you're holding, you'll be wanted for assault. You can either put away the weapon, or type \'#6hit %s !#0\', without the quotes, to continue.", original);
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  if (!*buf)
    {

      if (IS_SET (ch->flags, FLAG_FLEE))
	{
	  send_to_char ("You stop trying to flee.\n\r", ch);
	  ch->flags &= ~FLAG_FLEE;
	  return;
	}
      send_to_char ("Hit whom?\n\r", ch);
      return;
    }

  if (!(victim = get_char_room_vis (ch, buf)))
    {
      send_to_char ("They aren't here.\n\r", ch);
      return;
    }

  if (victim == ch)
    {
      send_to_char ("You affectionately pat yourself.\n\r", ch);
      act ("$n affectionately pats $mself.", false, ch, 0, victim, TO_ROOM);
      return;
    }

  if ((sa = get_second_affect(ch, SA_WARDED, NULL)))
  {
    if((CHAR_DATA *) sa->obj == victim)
    {
      send_to_char ("You're still warded away by their weapon.\n\r", ch);
      return;
    }   
  }

  // Make sure people don't hit the wrong person.
  if (are_grouped (ch, victim) && is_brother (ch, victim) && *argument != '!')
    {
      sprintf (buf,
	       "#1You are about to attack $N #1who is a fellow group member!#0 To confirm, type \'#6hit %s !#0\', without the quotes.",
	       original);
      act (buf, false, ch, 0, victim, TO_CHAR | _ACT_FORMAT);
      return;
    }

  //  sprintf (buf, "victim->fighting->short_descr = #2%s#0\n", victim->fighting->short_descr);
  //  send_to_gods(buf);
  // send_to_gods(ch->name);

  if (IS_SET (victim->act, ACT_FLYING) && !IS_SET (ch->act, ACT_FLYING)
      && AWAKE (victim) && !(victim->fighting)) // and victim is not fighting you
    {
      send_to_char ("They are flying out of reach!\n", ch);
      return;
    }

  if (IS_SET (victim->act, ACT_VEHICLE) || IS_SET (victim->act, ACT_PASSIVE))
    {
      send_to_char ("How do you propose to kill an inanimate object, hmm?\n",
		    ch);
      return;
    }

  i = 0;
  for (tch = vtor (ch->in_room)->people; tch; tch = tch->next_in_room)
    {
      if (tch->fighting == victim)
	{
	  if (++i >= 4)
	    {
	      act ("You can't find an opening to attack $N.",
		   false, ch, 0, victim, TO_CHAR);
	      return;
	    }

	}
    }

  if (IS_SET (victim->act, ACT_PREY) && AWAKE (victim))
    {
      if (!get_affect (ch, MAGIC_HIDDEN) || !skill_use (ch, SKILL_SNEAK, -30))
	{
	  act
	    ("As you approach, $N spots you and darts away! Try using a ranged weapon or an ambush from hiding instead.",
	     false, ch, 0, victim, TO_CHAR | _ACT_FORMAT);
	  act ("As $n approaches, $N spots $m and darts away.", false, ch, 0,
	       victim, TO_ROOM | _ACT_FORMAT);
	  npc_evasion (victim, -1);
	  add_threat (victim, ch, 7);
          remove_affect_type (ch, MAGIC_HIDDEN);
	  return;
	}
    }

  ch->flags &= ~FLAG_FLEE;
  ch->act &= ~PLR_STOP;

  if (GET_POS (ch) == POSITION_STANDING &&
      !ch->fighting && victim != ch->fighting)
    {

      if (ch->delay && ch->delay_type != DEL_CAST)
	break_delay (ch);

/*
      Removed to prevent easy cheesing of combat.

      ch->primary_delay = 0;
      ch->secondary_delay = 0;
*/

      if(victim->formation > 1 && do_group_size(victim) > 9)
      {
        for (tch = ch->room->people; tch; tch = tch->next_in_room)
        {
          if (tch->following != victim->following && tch->following != victim)
	    continue;

          if (num_attackers(tch) <= 2 
              && tch->formation == 1
              && GET_POS (tch) >= POSITION_SITTING)
          {
            found = true;
            break;
          }
        }

        if(found && CAN_SEE(tch, ch))
        {
          sprintf(buf, "Your attempted attack at #5%s#0 is obstructed, instead you find yourself engaged by  #5%s#0.", char_short(victim), char_short(tch));
          act(buf, false, ch, 0, tch, TO_CHAR | _ACT_FORMAT);
          sprintf(buf, "You intercept $n's attack at #5%s#0.", char_short(victim));
          act(buf, false, ch, 0, tch, TO_VICT | _ACT_FORMAT);
          victim = tch;
        }
      }

      if (cmd == 0)
	ch->flags &= ~FLAG_KILL;
      else
	ch->flags |= FLAG_KILL;

      if (GET_POS (ch) != POSITION_DEAD && GET_POS (victim) != POSITION_DEAD)
      {
	  criminalize (ch, victim, vtor (victim->in_room)->zone, CRIME_KILL);
      }
  
      set_fighting (ch, victim);
      if (!victim->fighting)
	{
	  set_fighting (victim, ch);
	  notify_guardians (ch, victim, cmd);
	  act ("$N engages $n in combat.", false, 
	       victim, 0, ch, TO_NOTVICT | _ACT_FORMAT);

          sprintf(buf, "engage #5%s#0 in combat.", char_short(victim));
          watched_action(ch, buf, 1, 0);
	}

      if(get_second_affect(ch, SA_AIMSTRIKE, NULL))
        remove_second_affect(get_second_affect(ch, SA_AIMSTRIKE, NULL));

      hit_char (ch, victim, 0);

      WAIT_STATE (ch, 9);

      if (victim->deleted)
	return;

      if (IS_SET (victim->act, ACT_MEMORY) && IS_NPC (victim))
	add_memory (ch, victim);

      /* Looks like a problem.  If you hit/kill in one hit, then
         trigger isn't called. */

      trigger (ch, argument, TRIG_HIT);

      if (ch->fighting == victim && IS_SUBDUEE (victim))
	stop_fighting (ch);
    }

  else if (ch->fighting == victim &&
	   !IS_SET (ch->flags, FLAG_KILL) && AWAKE (ch) && cmd == 1)
    {
      act ("You will try to kill $N.", false, ch, 0, victim, TO_CHAR);
      ch->flags |= FLAG_KILL;
    }

  else if (ch->fighting == victim &&
	   IS_SET (ch->flags, FLAG_KILL) && AWAKE (ch) && cmd == 0)
    {
      act ("You will try NOT to kill $N.", false, ch, 0, victim, TO_CHAR);
      ch->flags &= ~FLAG_KILL;
    }

  else if (ch->fighting &&
	   (GET_POS (ch) == FIGHT ||
	    GET_POS (ch) == STAND) && victim != ch->fighting)
    {

      if (ch->agi <= 9)
	ch->balance += -8;
      else if (ch->agi > 9 && ch->agi <= 13)
	ch->balance += -7;
      else if (ch->agi > 13 && ch->agi <= 15)
	ch->balance += -6;
      else if (ch->agi > 15 && ch->agi <= 18)
	ch->balance += -5;
      else
	ch->balance += -3;
      ch->balance = MAX (ch->balance, -25);

      if (ch->balance < -15)
	{
	  act ("You need more balance before you can try to attack $N!",
	       false, ch, 0, victim, TO_CHAR | _ACT_FORMAT);
	  return;
	}

      if(victim->formation > 1 && do_group_size(victim) > 9)
      {
        for (tch = ch->room->people; tch; tch = tch->next_in_room)
        {
          if (tch->following != victim->following && tch->following != victim)
	    continue;

          if (num_attackers(tch) <= 2 
              && tch->formation == 1
              && GET_POS (tch) >= POSITION_SITTING)
          {
            found = true;
            break;
          }
        }

        if(found && CAN_SEE(tch, ch))
        {
          sprintf(buf, "Your attempted attack at #5%s#0 is obstructed, instead you find yourself engaged by  #5%s#0.", char_short(victim), char_short(tch));
          act(buf, false, ch, 0, tch, TO_CHAR | _ACT_FORMAT);
          sprintf(buf, "You intercept $n's attack at #5%s#0.", char_short(victim));
          act(buf, false, ch, 0, tch, TO_VICT | _ACT_FORMAT);
          victim = tch;
        }
      }

      if (ch->fighting && !found)
	{

	  if (ch->fighting->fighting == ch)
	    {

	      agi_diff = GET_AGI (ch) - GET_AGI (ch->fighting);

	      if (agi_diff > number (-10, 10) && (number (0, 19) != 0))
		{

		  act ("You fail to shift your attention away from $N.",
		       false, ch, 0, ch->fighting,
		       TO_CHAR | _ACT_FORMAT | _ACT_COMBAT);
		  act ("$N fails to shift their attention away from you.",
		       false, ch->fighting, 0, ch,
		       TO_CHAR | _ACT_FORMAT | _ACT_COMBAT);
		  act ("$N fails to shift their attention away from $n.",
		       false, ch->fighting, 0, ch,
		       TO_NOTVICT | _ACT_FORMAT | _ACT_COMBAT);

		  return;
		}
	    }

	  act ("You stop fighting $N.", false, ch, 0, ch->fighting, TO_CHAR);
	  act ("You ready yourself for battle with $N.",
	       false, ch, 0, victim, TO_CHAR);
	  stop_fighting (ch);

	}

      if(!found)
        act ("You notice $N's attention shift toward you!",
	     false, victim, 0, ch, TO_CHAR);

      if ((is_area_enforcer (victim)) && IS_NPC (victim) && !get_affect(victim, MAGIC_ALERTED))
      {
	do_alert (victim, "", 0);
        magic_add_affect (ch, MAGIC_ALERTED, 90, 0, 0, 0, 0);
      }

      if (GET_POS (ch) != POSITION_DEAD && GET_POS (victim) != POSITION_DEAD)
	criminalize (ch, victim, vtor (victim->in_room)->zone, CRIME_KILL);

      set_fighting (ch, victim);

      if (cmd == 0)
	ch->flags &= ~FLAG_KILL;
      else
	ch->flags |= FLAG_KILL;

      if (IS_SET (victim->act, ACT_MEMORY) && IS_NPC (victim))
	add_memory (ch, victim);

      trigger (ch, argument, TRIG_HIT);

    }

  else
    send_to_char ("You're doing the best you can!\n\r", ch);
}

void
do_strike (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *victim, *tch;
  OBJ_DATA *obj;
  int i;
  char buf[MAX_STRING_LENGTH];
  char original[MAX_STRING_LENGTH];

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch))
    {
      send_to_char ("You cannot do this in an OOC area.\n", ch);
      return;
    }

  if (is_room_affected (ch->room->affects, MAGIC_ROOM_CALM))
    {
      act ("Try as you might, you simply cannot muster the will to break "
	   "the peace that pervades the area.", false, ch, 0, 0, TO_CHAR);
      return;
    }

  if(IS_SET(ch->room->room_flags, PEACE))
    {
      act ("Something prohibits you from taken such an action.", false, ch, 0, 0, TO_CHAR);
      return;
    }

  if(get_soma_affect(ch, SOMA_PLANT_VISIONS) && number(0,30) > GET_WIL(ch))
  {
    act ("You're paralyzed with fear, and unable to undertake such an action!", false, ch, 0, 0, TO_CHAR);
    return;
  }

  if (IS_SET (ch->flags, FLAG_PACIFIST))
    {
      send_to_char ("Remove your pacifist flag, first...\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if ((obj = get_equip (ch, WEAR_BOTH)))
    {
      if (obj->o.weapon.use_skill == SKILL_LONGBOW ||
	  obj->o.weapon.use_skill == SKILL_SHORTBOW ||
	  obj->o.weapon.use_skill == SKILL_SLING ||
	  obj->o.weapon.use_skill == SKILL_THROWN)
	{
	  send_to_char ("You can't use that in melee combat!\n", ch);
	  return;
	}
    }

  if (get_affect (ch, MAGIC_AFFECT_PARALYSIS))
    {
      send_to_char ("You are paralyzed and unable to fight!\n\r", ch);
      return;
    }

  if (get_affect (ch, MAGIC_AFFECT_FEAR))
    {
      send_to_char ("You are too afraid to fight!\n\r", ch);
      return;
    }

  if (IS_SUBDUER (ch))
    {
      act ("You can't attack while you have $N subdued.",
	   false, ch, 0, ch->subdue, TO_CHAR);
      return;
    }

  if (IS_SET (ch->plr_flags, NEW_PLAYER_TAG)
      && IS_SET (ch->room->room_flags, LAWFUL) && *argument != '!')
    {
      sprintf (buf,
	       "You are in a lawful area; you would likely be flagged wanted for assault. "
	       "To confirm, type \'#6strike %s !#0\', without the quotes.",
	       original);
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  if (!*buf)
    {

      if (IS_SET (ch->flags, FLAG_FLEE))
	{
	  send_to_char ("You stop trying to flee.\n\r", ch);
	  ch->flags &= ~FLAG_FLEE;
	  return;
	}

      send_to_char ("Hit whom?\n\r", ch);
      return;
    }

  if (!(victim = get_char_room_vis (ch, buf)))
    {
      send_to_char ("They aren't here.\n\r", ch);
      return;
    }

  if (victim == ch)
    {
      send_to_char ("You affectionately pat yourself.\n\r", ch);
      act ("$n affectionately pats $mself.", false, ch, 0, victim, TO_ROOM);
      return;
    }

  if (IS_SET (victim->act, ACT_FLYING) && !IS_SET (ch->act, ACT_FLYING)
      && AWAKE (victim) && !(victim->fighting))
    {
      send_to_char ("They are flying out of reach!\n", ch);
      return;
    }


  i = 0;
  for (tch = vtor (ch->in_room)->people; tch; tch = tch->next_in_room)
    {
      if (tch->fighting == victim)
	{
	  if (++i >= 4)
	    {
	      act ("You can't find an opening to strike $N.",
		   false, ch, 0, victim, TO_CHAR);
	      return;
	    }

	}
    }

  ch->flags &= ~FLAG_FLEE;
  ch->act &= ~PLR_STOP;

  if (GET_POS (ch) == POSITION_STANDING && !ch->fighting)
    {

      if (ch->delay && ch->delay_type != DEL_CAST)
	break_delay (ch);

      ch->primary_delay = 0;
      ch->secondary_delay = 0;

      hit_char (ch, victim, 1);

      WAIT_STATE (ch, 9);

      if (victim->deleted)
	return;

      if (IS_SET (victim->act, ACT_MEMORY) && IS_NPC (victim))
	add_memory (ch, victim);

      /* Looks like a problem.  If you hit/kill in one hit, then
         trigger isn't called. */

      if (IS_NPC (victim))
	add_threat (victim, ch, 3);

      trigger (ch, argument, TRIG_HIT);
    }
  else
    {
      send_to_char
	("You need to be standing and clear of combat to intiate a strike.\n",
	 ch);
    }
}

void
do_nokill (CHAR_DATA * ch, char *argument, int cmd)
{
  send_to_char ("Please spell out all of 'kill' to avoid any mistakes.\n",
		ch);
}

void
do_kill (CHAR_DATA * ch, char *argument, int cmd)
{
  char verify[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  char original[MAX_STRING_LENGTH];
  CHAR_DATA *victim;

  sprintf (original, "%s", argument);
  argument = one_argument(argument, buf);
 
  if (IS_NPC (ch) || !GET_TRUST (ch))
  {
    do_hit (ch, original, 1);
    return;
  }
  else
  {
    if (!*buf)
    {
      send_to_char ("Smite whom?\n\r", ch);
      return;
    }
  }

  if (!(victim = get_char_room_vis (ch, buf)))
  {
    send_to_char ("They aren't here.\n\r", ch);
    return;
  }

  if (victim == ch)
  {
    send_to_char ("Smiting yourself is not a good move..\n\r", ch);
    return;
  }

  argument = one_argument(argument, verify);

  if (!IS_NPC (victim) && *verify != '!')
  {
    send_to_char ("Target is a player character.  Please use "
		"'KILL <name> !' syntax if \n\ryou really mean it.'\n\r",
    ch);
    return;
  }

  if (GET_TRUST (victim) > GET_TRUST (ch))
  {
    victim = ch;
  }
  act ("$n stares at you, narrowing $s eyes. Shortly thereafter, your heart obediently ceases to beat, and you feel death upon you...", false, ch, 0, victim, TO_VICT | _ACT_FORMAT);
  act ("You narrow your eyes in concentration, and $N collapses, dead.",
	   false, ch, 0, victim, TO_CHAR | _ACT_FORMAT);
  act ("$n stares at $N, narrowing $s eyes. Suddenly, $N collapses, dead.",
	 false, ch, 0, victim, TO_NOTVICT | _ACT_FORMAT);
  die (victim);
}

void
do_command (CHAR_DATA * ch, char *argument, int cmd)
{
  int everyone = 0;
  CHAR_DATA *victim = NULL;
  CHAR_DATA *next_in_room;
  CHAR_DATA *tch;
  char buf[MAX_STRING_LENGTH];
  char command[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Command whom?\n\r", ch);
      return;
    }

  /* We can command invis chars as well, why not :) */

  if (!str_cmp (buf, "all"))
    everyone = 1;

  else if (!str_cmp (buf, "group") || !str_cmp (buf, "follower") ||
      !str_cmp (buf, "followers"))
    everyone = 2;

  else if (!(victim = get_char_room_vis (ch, buf)) &&
	   !(victim = get_char_room (buf, ch->in_room)))
    {
      send_to_char ("They aren't here.\n\r", ch);
      return;
    }

  while (isspace (*argument))
    argument++;

  if (!*argument)
    {
      send_to_char ("What is your command?\n\r", ch);
      return;
    }

  strcpy (command, argument);

  if (victim == ch)
    {
      send_to_char ("You don't have to give yourself commands.\n\r", ch);
      return;
    }

  if (victim)
    {

      if (!is_leader (ch, victim) && victim->following != ch)
	{
	  act ("You do not have the authority to command $N.", false, ch, 0,
	       victim, TO_CHAR);
	  return;
	}

      sprintf (buf, "#5%s#0 commands you to '%s'.\n", char_short (ch),
	       command);
      buf[2] = toupper (buf[2]);
      send_to_char (buf, victim);

      /* Bag this for now per Regi */
      /*
         act ("$n gives $N an command.", false, ch, 0, victim, TO_NOTVICT);
       */

      if (victim->mob)
	{
	  send_to_char ("Ok.\n", ch);
	  command_interpreter (victim, command);
	}
      else
	{
	  sprintf (buf, "You command #5%s#0 to '%s'.\n",
		   char_short (victim), command);
	  send_to_char (buf, ch);
	}

      return;
    }
  else if(everyone == 1)
  {
    for (tch = ch->room->people; tch; tch = next_in_room)
    {

      next_in_room = tch->next_in_room;

      if (!is_leader (ch, tch) && (tch->following != ch))
	continue;

      sprintf (buf, "#5%s#0 commands you to '%s'.\n", char_short (ch),
	       command);
      buf[2] = toupper (buf[2]);
      send_to_char (buf, tch);

      if (tch->mob)
	command_interpreter (tch, command);
    }
  }
  else if(everyone == 2)
  {
    for (tch = ch->room->people; tch; tch = next_in_room)
    {

      next_in_room = tch->next_in_room;

      if (tch->following != ch)
	continue;

      sprintf (buf, "#5%s#0 commands you to '%s'.\n", char_short (ch),
	       command);
      buf[2] = toupper (buf[2]);
      send_to_char (buf, tch);

      if (tch->mob)
	command_interpreter (tch, command);
    }
  }

  send_to_char ("Ok.\n", ch);
}


void
retreat (CHAR_DATA* ch, int direction, CHAR_DATA* leader)
{
  // base number of seconds
  int duration = 0;
  AFFECTED_TYPE *af;

  if ((af = get_affect (ch, AFFECT_GROUP_RETREAT)))
    {
      if (af->a.spell.sn == direction)
	{
	  send_to_char ("You are already retreating in that direction!\n", ch);
	  return;
	}
      else
	{
	  remove_affect_type (ch, AFFECT_GROUP_RETREAT);  
	}
    }

  char message[AVG_STRING_LENGTH];
  if (leader)
    {
      if (ch == leader)
	{
	  sprintf(message,
		  "You command your group to retreat %sward.\n",
		  dirs[direction]);
          send_to_char (message, ch);
          sprintf (message, "$n's group begins to fall back to the %s.",
	       dirs[direction]);
          act (message, false, ch, 0, 0, TO_ROOM);
	}
      else
	{
	  sprintf(message,
		  "Your group begins to retreat %sward "
		  "at the command of #5%s#0.\n",
		  dirs[direction], char_short(leader));
          send_to_char (message, ch);
	}
    }
  else
    {
      sprintf(message,
	      "You attempt to retreat %sward.\n",
	      dirs[direction]);
      send_to_char (message, ch);	  
      sprintf (message, "$n begins to fall back to the %s.",
	       dirs[direction]);
      act (message, false, ch, 0, 0, TO_ROOM);
    }

  if (!ch->following || ch->fighting || leader) 
    {
      duration = 40;

      // Commenting out a lot of this stuff, as retreating piece-meal is counter to the point
      // of a retreat, as opposed to a whole-sale route. You retreat as a group, to help prevent
      // slaughter. If you want to flee in bits and drabs, then use the flee command.

      /*
      // terrain penalty
      duration += movement_loss[ch->room->sector_type];
      
      // wound penalty
      int damage = 0;
      int health_percent;
      for ( WOUND_DATA *wound = ch->wounds; wound; wound = wound->next)
	{
	  damage += wound->damage;
	}
      damage += ch->damage;
      health_percent = (damage * 100) / ch->max_hit;
      if (health_percent < 50)
	{
	  // penalty or bonus depending on your luck and ability.
	  duration += number (12,20);
	  duration -= MAX(ch->wil,ch->con);
	}
      
      // defensive stance bonus
      duration -= (IS_SET (ch->flags, FLAG_PACIFIST)) ? (5) : (ch->fight_mode);
      
      // agility roll bonus
      duration -= number (1, ch->agi);*/
      
      magic_add_affect (ch, AFFECT_GROUP_RETREAT, duration, 0, 0, 0, direction);
    }    
  else
    {
      do_move (ch, "", direction);
    }
      

}
  

void
do_flee (CHAR_DATA * ch, char *argument, int cmd)
{
  int dir;
  float damage = 0;

  if (!can_move(ch))
  	return;
  	
  if (!ch->fighting)
    {
      send_to_char ("You're not fighting.\n\r", ch);
      return;
    }

  if (get_affect (ch, MAGIC_AFFECT_PARALYSIS))
    {
      send_to_char ("You are paralyzed and unable to flee!\n\r", ch);
      return;
    }

  if (get_soma_affect (ch, SOMA_PLANT_FURY))
  {
      damage = get_damage_total(ch);
      if(damage / ch->max_hit > 0.20)
        send_to_char ("Flee? You frenzied state causes you to laugh at the mere suggestion!\n\r", ch);
      return;
  }

  if (IS_SET (ch->flags, FLAG_FLEE))
    {
      send_to_char ("You are already trying to escape!\n\r", ch);
      return;
    }

  for (dir = 0; dir <= LAST_DIR; dir++)
    if (CAN_GO (ch, dir) && !isguarded (ch->room, dir))
      break;

  if (dir == 6)
    {
      send_to_char ("THERE IS NOWHERE TO FLEE!!\n\r", ch);
      return;
    }

  ch->flags |= FLAG_FLEE;

  send_to_char ("You resolve to escape combat. . .\n\r", ch);

  act ("$n's eyes dart about looking for an escape path!",
       false, ch, 0, 0, TO_ROOM);
}

int
flee_attempt (CHAR_DATA * ch)
{
  int dir;
  int enemies = 0;
  int mobless_count = 0;
  int mobbed_count = 0;
  int mobless_dirs[6];
  int mobbed_dirs[6];
  CHAR_DATA *tch;
  char buf[MAX_STRING_LENGTH];
  ROOM_DATA *troom;
/*
	if ( IS_SET (ch->flags, FLAG_SUBDUING) ) {
		ch->flags &= ~FLAG_FLEE;
		return 0;
	}
*/
  if (GET_POS (ch) < FIGHT)
    return 0;

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {

      if (tch->fighting != ch)
	continue;

      if (GET_POS (tch) != FIGHT && GET_POS (tch) != STAND)
	continue;

      if (!CAN_SEE (tch, ch))
	continue;

      enemies++;
    }

  for (dir = 0; dir <= LAST_DIR; dir++)
    {

      if (!CAN_GO (ch, dir))
	continue;

      if (vtor (EXIT (ch, dir)->to_room)->people)
	mobbed_dirs[mobbed_count++] = dir;
      else
	mobless_dirs[mobless_count++] = dir;
    }

  if (!mobbed_count && !mobless_count)
    {
      send_to_char ("There is nowhere to go!  You continue fighting!\n\r",
		    ch);
      ch->flags &= ~FLAG_FLEE;
      return 0;
    }

  if (enemies && number (0, enemies))
    {
      switch (number (1, 3))
	{
	case 1:
	  send_to_char ("You attempt escape, but fail . . .\n\r", ch);
	  break;
	case 2:
	  send_to_char ("You nearly escape, but are blocked . . .\n\r", ch);
	  break;
	case 3:
	  send_to_char ("You continue seeking escape . . .\n\r", ch);
	  break;
	}

      act ("$n nearly flees!", true, ch, 0, 0, TO_ROOM);

      return 0;
    }

  if (mobless_count)
    dir = mobless_dirs[number (0, mobless_count - 1)];
  else
    dir = mobbed_dirs[number (0, mobbed_count - 1)];

  troom = ch->room;

  /* stop_fighting_sounds (ch, troom); */
  stop_followers (ch);
  if (ch->fighting)
    {
      stop_fighting (ch);
    }

  do_move (ch, "", dir);

  act ("$n #3flees!#0", false, ch, 0, 0, TO_ROOM);

  sprintf (buf, "#3YOU FLEE %s!#0", dirs[dir]);
  act (buf, false, ch, 0, 0, TO_CHAR);

  if (!enemies)
    sprintf (buf, "\nYou easily escaped to the %s.\n\r", dirs[dir]);
  else
    sprintf (buf, "\nYou barely escaped to the %s.\n\r", dirs[dir]);

  ch->formation = 0;
  ch->following = 0;

  if (ch->room != troom)
    send_to_char (buf, ch);

  ch->flags &= ~FLAG_FLEE;

  return 1;
}

	/* In case victim is being guarded, make sure rescue affects are active. */

void
guard_check (CHAR_DATA * victim)
{
  CHAR_DATA *tch;
  AFFECTED_TYPE *af;

  for (tch = victim->room->people; tch; tch = tch->next_in_room)
    {

      if (!(af = get_affect (tch, MAGIC_GUARD)))
	continue;

      if ((CHAR_DATA *) af->a.spell.t == victim &&
	  !get_second_affect (tch, SA_RESCUE, NULL))
      {
	add_second_affect (SA_RESCUE, 1, tch, (OBJ_DATA *) victim, NULL, 0);
 

        // If you're guarding and you've got three or more attackers, you lose the
        // guard -- can't keep it up forever my friend.
 
        if(num_attackers(tch) >= 3)
        {
	  remove_affect_type (tch, MAGIC_GUARD);
        }
      }

    }
}

void
do_guard (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *target = NULL, *tch = NULL;
  AFFECTED_TYPE *af;
  char buf[MAX_STRING_LENGTH];
  int dir;

  if (IS_SET (ch->room->room_flags, OOC))
    {
      send_to_char ("That command cannot be used in an OOC area.\n", ch);
      return;
    }

  if ((af = get_affect (ch, AFFECT_GUARD_DIR)))
    affect_remove (ch, af);

  argument = one_argument (argument, buf);

  if (*buf && !cmd && !(target = get_char_room_vis (ch, buf)))
    {
      if ((dir = index_lookup (dirs, buf)) == -1)
	{
	  send_to_char ("Who or what did you want to guard?\n", ch);
	  return;
	}

      if (!(af = get_affect (ch, AFFECT_GUARD_DIR)))
	magic_add_affect (ch, AFFECT_GUARD_DIR, -1, 0, 0, 0, 0);

      af = get_affect (ch, AFFECT_GUARD_DIR);

      af->a.shadow.shadow = NULL;
      af->a.shadow.edge = dir;

      sprintf (buf, "You will now guard the %s exit.\n", dirs[dir]);
      send_to_char (buf, ch);
      sprintf (buf, "$n moves to block the %s exit.", dirs[dir]);
      act (buf, true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      return;

    }
  else if(cmd && !(target = (CHAR_DATA *)cmd))
    return;



  if ((!*buf || target == ch))
    {
      if (!(af = get_affect (ch, MAGIC_GUARD))
	  && !(af = get_affect (ch, AFFECT_GUARD_DIR)))
	{
	  send_to_char ("You are not currently guarding anything.\n", ch);
	  return;
	}
      else if ((af = get_affect (ch, MAGIC_GUARD))
	       && (tch = (CHAR_DATA *) af->a.spell.t) != NULL)
	{
	  act ("You cease guarding $N.", true, ch, 0, tch,
	       TO_CHAR | _ACT_FORMAT);
	  act ("$n ceases guarding you.", false, ch, 0, tch,
	       TO_VICT | _ACT_FORMAT);
	  act ("$n ceases guarding $N.", false, ch, 0, tch,
	       TO_NOTVICT | _ACT_FORMAT);
	  remove_affect_type (ch, MAGIC_GUARD);
	  return;
	}
      else if ((af = get_affect (ch, AFFECT_GUARD_DIR)))
	{
	  act ("You cease guarding the exit.", true, ch, 0, 0, TO_CHAR);
	  remove_affect_type (ch, AFFECT_GUARD_DIR);
	  return;
	}
      else
	{
	  send_to_char ("You cease guarding.\n", ch);
	  remove_affect_type (ch, MAGIC_GUARD);
	  remove_affect_type (ch, AFFECT_GUARD_DIR);
	  return;
	}
    }

  if (get_affect (target, MAGIC_GUARD)
      || get_affect (target, AFFECT_GUARD_DIR))
    {
      send_to_char ("They're already trying to guard something themselves!\n",
		    ch);
      return;
    }

  if(num_attackers(ch) >= 3)
  {
    act ("You are under attack from too many foes to help $N.", false, ch, 0, target, TO_CHAR);    
    return;
  }


  if ((af = get_affect (ch, MAGIC_GUARD)))
    affect_remove (ch, af);

  magic_add_affect (ch, MAGIC_GUARD, -1, 0, 0, 0, 0);

  if (!(af = get_affect (ch, MAGIC_GUARD)))
    {
      send_to_char ("There is a bug in guard.  Please let an admin "
		    "know.\n", ch);
      return;
    }

  af->a.spell.t = (long int) target;

  act ("You will now guard $N.", false, ch, 0, target, TO_CHAR | _ACT_FORMAT);
  act ("$n moves into position to guard you.", false, ch, 0, target,
       TO_VICT | _ACT_FORMAT);
  act ("$n moves into position to guard $N.", false, ch, 0, target,
       TO_NOTVICT | _ACT_FORMAT);
}
