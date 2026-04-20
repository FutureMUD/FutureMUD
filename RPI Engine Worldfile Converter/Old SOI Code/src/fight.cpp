/*------------------------------------------------------------------------\
|  fight.c : Central Combat Processor                 www.middle-earth.us |
|  Copyright (C) 2004, Shadows of Isildur: Traithe                        |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/

#include <sys/types.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <signal.h>
#include <ctype.h>
#include <time.h>
#include <unistd.h>
#include "server.h"

#include "structs.h"
#include "net_link.h"
#include "account.h"
#include "protos.h"
#include "utils.h"
#include "decl.h"
#include "group.h"
#include "utility.h"


CHAR_DATA *combat_list = 0;
CHAR_DATA *combat_next_dude = 0;	/* Next dude global trick           */

const int shield_parry_offense[4][4] = {
  {RESULT_FUMBLE, RESULT_NONE, RESULT_NONE, RESULT_NONE},
  {RESULT_NONE, RESULT_BLOCK, RESULT_NONE, RESULT_NONE},
  {RESULT_HIT1, RESULT_HIT, RESULT_BLOCK, RESULT_NONE},
  {RESULT_HIT3, RESULT_HIT1, RESULT_HIT, RESULT_BLOCK}
};

/*     CF             MF            MS          CS    */
const int shield_parry_defense[4][4] = {
  {RESULT_FUMBLE, RESULT_NONE, RESULT_ADV, RESULT_ADV},	/* CF */
  {RESULT_NONE, RESULT_BLOCK, RESULT_NONE, RESULT_ADV},	/* MF */
  {RESULT_NONE, RESULT_NONE, RESULT_BLOCK, RESULT_ADV},	/* MS */
  {RESULT_NONE, RESULT_NONE, RESULT_NONE, RESULT_BLOCK}	/* CS */
};

const int dodge_offense[4][4] = {
  {RESULT_FUMBLE, RESULT_NONE, RESULT_NONE, RESULT_NONE},
  {RESULT_NONE, RESULT_NONE, RESULT_NONE, RESULT_NONE},
  {RESULT_HIT2, RESULT_HIT, RESULT_NONE, RESULT_NONE},
  {RESULT_HIT4, RESULT_HIT2, RESULT_HIT, RESULT_NONE}
};

const int dodge_defense[4][4] = {
  {RESULT_FUMBLE, RESULT_NONE, RESULT_ADV, RESULT_ADV},
  {RESULT_NONE, RESULT_NONE, RESULT_NONE, RESULT_ADV},
  {RESULT_NONE, RESULT_NONE, RESULT_NONE, RESULT_NONE},
  {RESULT_NONE, RESULT_NONE, RESULT_NONE, RESULT_NONE}
};

const int ignore_offense[4] =
  { RESULT_FUMBLE, RESULT_HIT, RESULT_HIT2, RESULT_HIT4 };

const struct fight_data fight_tab[] = {
  {"Frantic", 1.20, 0.75, -5},	/* Frantic */
  {"Aggressive", 1.05, 0.95, -1},	/* Aggressive */
  {"Normal", 1.00, 1.00, 0},	/* Normal */
  {"Careful", 0.90, 1.10, 1},	/* Careful */
  {"Defensive", 0.80, 1.20, 4},	/* Defensive */
};

const char *crithit[] = {
  "deft", "nimble",		/* stab */
  "forceful", "deft",		/* pierce */
  "powerful", "staggering",	/* chop */
  "crushing", "staggering",	/* bludgeon */
  "savage", "brutal",		/* slash */
  "hurtful", "hurtful",		/* lash */
  "savage", "brutal"
};

const char *crithits[] = {
  "deftly", "nimbly",		/* stab */
  "forcefully", "deftly",	/* pierce */
  "savagely", "brutally",	/* chop */
  "powerfully", "brutally",	/* bludgeon */
  "savagely", "brutally",	/* slash */
  "hurtfully", "hurtfully",	/* lash */
  "savagely", "brutally"
};				/* natural */


const char *wtype[] = {
  "stab", "stab", "pierce",		/* stab */
  "thrust", "lunge", "stab",		/* pierce */
  "chop", "hack", "cut",	/* chop */
  "strike", "bash", "crush",		/* bludgeon */
  "slash", "slice", "cut",		/* slash */
  "lash", "lash", "slice"
};				/* lash */

const char *wtype2[] = {
  "stabs", "stabs", "pierces",		/* stab */
  "thrusts", "lunges", "stabs",		/* pierce */
  "chops", "hacks", "cuts",		/* chop */
  "strikes", "bashes", "crushes",		/* bludgeon */
  "slashes", "slices", "cuts",		/* slash */
  "lashes", "lashes", "slices"
};				/* lash */

const char *deathhit[] = {
  "run through", "perforated", "mortally lacerated",
  "skewered", "run through", "greviously impaled",
  "torn open", "rent asunder", "hacked apart", 
  "smashed down", "utterly ruined", "brutally bludgeoned",
  "cut down", "torn open", "sliced open",
  "torn open", "rent asunder", "cut down"
};

const char *expiremsg[] = {
   ""
};

const char *cs_name[] = {
  "CritFail", "ModrFail", "ModrSucc", "CritSucc", "\n"
};

const char *rs_name[] = {
  "None", "Advant", "Block", "Parry", "Fumble",
  "Hit0", "Hit1", "Hit2", "Hit3", "Hit4", "Stumble",
  "NearFumble", "NearStumble", "Dead", "Any", "WeaponBreak",
  "ShieldBreak", "KO", "Just_KO", "\n"
};


const char *armor_types[] = {
  "Fur",
  "Quilted",
  "Leather",
  "Scale",
  "Mail",
  "Plate"
};

const struct body_info body_tab[NUM_BODIES][MAX_HITLOC] = {
  {{"body", 2, 1, 55, WEAR_BODY, WEAR_ABOUT},
   {"leg", 4, 5, 14, WEAR_LEGS},
   {"arms", 4, 5, 17, WEAR_ARMS, WEAR_ABOUT},
   {"head", 5, 2, 7, WEAR_HEAD},
   {"neck", 3, 1, 2, WEAR_NECK_1, WEAR_ABOUT},
   {"feet", 1, 5, 0, WEAR_FEET},
   {"hands", 1, 5, 4, WEAR_HANDS},
   {"eyes", 4, 1, 1, WEAR_HEAD, WEAR_FACE},
   }
};
const char *attack_names[] = {
  "punch",
  "bite",
  "claw",
  "peck",
  "trample",
  "gore",
  "chill",
  "stab",
  "burn",
  "\n"
};

const char *attack_names_plural[] = {
  "punches",
  "bites",
  "claws",
  "pecks",
  "tramples",
  "gores",
  "chills",
  "stabs",
  "burns"
};

const char *attack_part[] = {
  "fist",
  "sharp teeth",
  "paw",
  "sharp beak",
  "feet",
  "tusks",
  "freezing touch",
  "stinger",
  "burning grasp"
};

const char *miss_descriptor[] = {
  "utterly ", "completely ",
  "broadly ", "easily ",
  "", "",
  "narrowly ", "barely ",
  "fractionally ", "only just "
};

const char *generic_descriptor[] = {
  "clumsy ", "inept ", "wild ", "lazy ", 
  "amateur ", "unskilled ", "novice ", "rudimentary ",
  "", "", "", "",
  "neat ", "measured ", "skilled ", "adept ",
  "expert ", "excellent ", "masterful ", "near perfect "
};

const char *block_descriptor[] = {
  "only just ", "clumsily ",
  "narrowly ", "hastily ",
  "", "", 
  "deftly ", "easily ", 
  "gracefully ", "effortlessly ",
  "eerily ", "flawlessly "
};

const char *parry_descriptor[] = {
  "only just ", "clumsily ",
  "narrowly ", "hastily ",
  "", "", 
  "deftly ", "easily ", 
  "gracefully ", "effortlessly ",
  "eerily ", "flawlessly "
};

const char *break_def[] = {
  "slip past", "slips past",	/* stab */
  "thrust through", "thrusts through",		/* pierce */
  "break down", "breaks down",	/* chop */
  "force through", "forces through",	/* bludgeon */
  "knock aside", "knocks aside",	/* slash */
  "maneuver around", "maneuvers around",	/* lash */
  "force through", "forces through"  /* natural */
};				

const int weapon_armor_table[6][6] = {
  //F   Q   L   S   C   P
  { 1,  0,  0,  2,  0,  0},		// stab -- small-blades, knives, daggers
  { 1,  0, -1,  1,  1,  0},		// pierce -- polearms, arrows
  { 0,  1,  1, -1, -1, -1},		// chop -- axes 
  {-1, -1, -1,  2,  2,  1},		// bludgeon -- clubs, flails
  { 0,  1,  1, -2, -3, -2},		// slash -- swords
  { 0,  0,  0,  0,  0,  0}	        // lash -- whips
};

/* old system
const int weapon_nat_attack_table[4][6] = {
  // Q   L   R   S   M   P
  {-1, -2, -1, -1, 2, 2},	// punch
  {1, 1, 1, 1, -2, -2},		// bite
  {0, -1, 0, 0, 1, 2},		// claw
  {1, 1, 1, 1, 0, -1}		// peck
};
*/
const int weapon_nat_attack_table[8][6]={
  //F  Q  L  S  C  P
  {-1, -1, 0, 0, 0, 0}, // punch
  {-1, -1, 0, 0, 0, 0}, // bite
  {-1, -1, 0, 0, 0, 0}, // claw
  {-1, -1, 0, 0, 0, 0}, // peck
  {-1, -1, 0, 0, 0, 0}, // trample/feet
  {-1, -1, 0, 0, 0, 0}, // gore/tusks
  {0, 0, 0, 0, 0, 0}, // chill
  {-1, -1, 0, 0, 0, 0}  // sting
};



void compete (CHAR_DATA * ch, CHAR_DATA * src, CHAR_DATA * tar,
	      int iterations);


int
get_damage_total (CHAR_DATA* ch)
{
  int damage = 0;
  WOUND_DATA *wound = ch->wounds;
  for (; wound; wound = wound->next)
    {
      damage += wound->damage;
    }
  damage += ch->damage;

  return damage;
}

/// Used only in strike
int
figure_wound_skillcheck_penalties (CHAR_DATA * ch, int skill)
{

  // Fury-suffering people don't care about their wounds.
  if(get_soma_affect (ch, SOMA_PLANT_FURY))
    return skill;

  // Save vs WILLPOWER
  if (number (1, 25) <= ch->wil)
    {
      return skill;
    }

  int damage = get_damage_total (ch);
  if (damage)
    {
      /// 2^14 = 16384 gives us the best precision for our range of hp and
      /// skill. The integer math matches the correctly rounded value about
      /// 49% of the time as opposed to casting the value from floats (which
      /// are the same 51% of the time). Accurate within 1% of the skill
      /// value and about 1/20th the speed of "nearestint" and about 5/6th
      /// the speed of casting floats.

      const int shift_precision = 14;

      int max_hp = ch->max_hit;
      int percent = ((max_hp - damage) << shift_precision) / max_hp;
      skill *= percent;
      skill >>= shift_precision;
    }

  return skill;
}


void
add_criminal_time (CHAR_DATA * ch, int zone, int penalty_time)
{
  AFFECTED_TYPE *af;

  magic_add_affect (ch, MAGIC_CRIM_BASE + zone, penalty_time, 0, 0, 0, 0);

  af = get_affect (ch, MAGIC_CRIM_BASE + zone);

  if (af->a.spell.duration > 36)
    af->a.spell.duration = 36;
}


// criminalize
//
// Refrences:
// * act.movement.c - do_pick() on failed door pick
// * act.offensive.c - do_throw() on ch!fighting and damage > 3
// * act.offensive.c - fire_sling() on ch!fighting and damage > 3
// * act.offensive.c - do_fire() on ch!fighting
// * act.offensive.c - do_hit() on ch!dead && vict!dead
// * act.offensive.c - do_kill() on ch!dead and vict!dead
// * act.other.c - do_steal() stealing from victim
// * act.other.c - do_steal() stealing from victim container
// * fight.c - do_rescue()
// * fight.c - delayed_study() ch=yrchish
// * transformation.c - transformation_animal_spell()
void
criminalize (CHAR_DATA * ch, CHAR_DATA * vict, int zone, int crime)
{
  int criminalize_him = 0;
  int penalty_time = 0;
  AFFECTED_TYPE *af;
  ROOM_DATA *room;
  CHAR_DATA *tch;
  char *date;
  time_t time_now;
  char msg[MAX_STRING_LENGTH];

  // Make sure we have a criminal
  if (!ch)
    {
      send_to_gods("Crime commited without a criminal. Contact Kithrater.");
      return;
    }

  // Make sure we have a victim for KILL or STEAL
  if (!vict && (crime == CRIME_KILL || crime == CRIME_STEAL || crime == CRIME_ASSAULT))
    {
      send_to_gods("Crime commited without a target. Contact Kithrater.");
      return;
    }

  if (crime == CRIME_KILL)
    penalty_time = 24;
  else if (crime == CRIME_ASSAULT)
    penalty_time = 24;
  else if (crime == CRIME_STEAL)
    penalty_time = 8;
  else if (crime == CRIME_PICKLOCK)
    penalty_time = 4;
  else
    penalty_time = 4;

  // For fighting, if this is a brawling room, then don't worry about a thing.

  if(crime == CRIME_KILL && IS_SET(ch->room->room_flags, BRAWL) && GET_POS (vict) > 2 
     && AWAKE(vict) && (!IS_NPC(vict) || (IS_NPC(vict) && GET_TRUST(vict))) && !has_weapon(ch))
    return;

  /* This allows someone to avoid criminalization by assisting a guard and
     then turning on the guard, or other similar things. This is bad. */

  if (ch->fighting && crime != CRIME_ASSAULT)
    return;

  room = ch->room;

  if (is_area_enforcer (ch))
    return;

  if (!IS_SET (room->room_flags, LAWFUL))
    return;

  if (vict)
    {
      if (IS_SET (vict->act, ACT_WILDLIFE))
	return;

      if (IS_SET (vict->act, ACT_AGGRESSIVE))
	return;

      if (IS_SET (vict->act, ACT_PARIAH))
	return;

      if ((get_affect (vict, MAGIC_CRIM_BASE + zone) ||
	   get_affect (vict, MAGIC_CRIM_HOODED + zone)) &&
	  !get_affect (ch, MAGIC_CRIM_BASE + zone))
	return;
    }

  if (is_hooded (ch))
    {

      if ((!IS_OUTSIDE (ch) && !IS_LIGHT (room)) ||
	  (IS_OUTSIDE (ch) && IS_NIGHT))
	{

	  if (!(af = get_affect (ch, MAGIC_CRIM_HOODED + zone)))
	    magic_add_affect (ch, MAGIC_CRIM_HOODED + zone, 1, penalty_time,
			      0, 0, 0);
	  else
	    af->a.spell.modifier += penalty_time;

	  for (tch = ch->room->people; tch; tch = tch->next_in_room)
	    enforcer (tch, ch, 1, 1);

	  if (!ch->deleted && !af)
	    {
	      af = get_affect (ch, MAGIC_CRIM_HOODED + zone);
	      affect_remove (ch, af);
	    }

	  return;
	}

      else if (!number (0, 4))
	criminalize_him = 1;
      else
	magic_add_affect (ch, MAGIC_CRIM_HOODED + zone, 40, penalty_time, 0,
			  0, 0);

    }
  else
    criminalize_him = 1;

  if (criminalize_him)
    {
      send_to_char ("An onlooker gasps at your actions and runs off to find "
		    "help!\n", ch);

      /* Add hooded crim affect with penalty of 0 as a marker */

      magic_add_affect (ch, MAGIC_CRIM_HOODED + zone, 40, 0, 0, 0, 0);

      add_criminal_time (ch, zone, penalty_time);

      /* Interconnected city zones */
      /*
      if (zone == 1)
	{
	  magic_add_affect (ch, MAGIC_CRIM_HOODED + 3, 40, 0, 0, 0, 0);
	  add_criminal_time (ch, 3, penalty_time);
	  magic_add_affect (ch, MAGIC_CRIM_HOODED + 11, 40, 0, 0, 0, 0);
	  add_criminal_time (ch, 11, penalty_time);
	}*/

      if (IS_MORTAL (ch))
	{
	  time_now = time (0);
	  date = (char *) asctime (localtime (&time_now));
	  date[strlen (date) - 1] = '\0';

	  if (crime == CRIME_KILL || crime == CRIME_ASSAULT)
	    sprintf (msg,
		     "Flagged wanted for Assault in %s for %d hours. [%d]\n",
		     zone_table[ch->room->zone].name, penalty_time,
		     ch->in_room);
	  else if (crime == CRIME_STEAL)
	    sprintf (msg,
		     "Flagged wanted for Attempted Theft in %s for %d hours. [%d]\n",
		     zone_table[ch->room->zone].name, penalty_time,
		     ch->in_room);
	  else if (crime == CRIME_PICKLOCK)
	    sprintf (msg,
		     "Flagged wanted for Breaking and Entering in %s for %d hours. [%d]\n",
		     zone_table[ch->room->zone].name, penalty_time,
		     ch->in_room);
	  else
	    sprintf (msg, "Flagged wanted in %s for %d hours. [%d]\n",
		     zone_table[ch->room->zone].name, penalty_time,
		     ch->in_room);

	  if (!IS_NPC (ch))
	    {
	      add_message (1, ch->tname, -2, "Server", date, "Wanted.", "",
			   msg, 0);
	      add_message (1, "Crimes", -5, "Server", date, ch->tname, "",
			   msg, 0);
	    }
	}
    }

  /* Immediate guard response */

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    enforcer (tch, ch, 1, 1);
}


void
stop_fighting_sounds (CHAR_DATA * ch, ROOM_DATA * room)
{
  int dir, from_dir;
  ROOM_DIRECTION_DATA *exit;
  ROOM_DATA *next_room;
  char buf[MAX_STRING_LENGTH];
  char *e_dirs[] =
    { "to the north", "to the east", "to the south", "to the west", "above",
    "below"
  };
  AFFECTED_TYPE *af;

  if (!ch->room)
    return;

  if (!room)
    return;

  for (dir = 0; dir <= LAST_DIR; dir++)
    {
      if (!(exit = EXIT (ch, dir)))
	continue;
      if (!(next_room = vtor (exit->to_room)))
	continue;
      if (dir == 0)
	from_dir = 2;
      else if (dir == 1)
	from_dir = 3;
      else if (dir == 2)
	from_dir = 0;
      else if (dir == 3)
	from_dir = 1;
      else if (dir == 4)
	from_dir = 5;
      else
	from_dir = 4;
      if (next_room->affects &&
	  next_room->affects->type == MAGIC_ROOM_FIGHT_NOISE &&
	  next_room->affects->a.room.duration == from_dir)
	{
	  next_room->affects = next_room->affects->next;
	}
      else
	for (af = next_room->affects; af; af = af->next)
	  {
	    if (!af->next)
	      break;
	    if (af->next->type != MAGIC_ROOM_FIGHT_NOISE)
	      continue;
	    if (af->next->a.room.duration != from_dir)
	      continue;
	    af->next = af->next->next;
	  }
      sprintf (buf, "The sounds of an armed conflict %s have died away.",
	       e_dirs[from_dir]);
      send_to_room (buf, next_room->nVirtual);
    }
}

void
fighting_sounds (CHAR_DATA * ch, ROOM_DATA * room)
{
  int dir, from_dir;
  ROOM_DIRECTION_DATA *exit;
  ROOM_DATA *next_room;
  AFFECTED_TYPE *af;
  char buf[MAX_STRING_LENGTH];
  char *e_dirs[] =
    { "the north", "the east", "the south", "the west", "above", "below" };
  bool found = false;

  for (dir = 0; dir <= LAST_DIR; dir++)
    {
      if (!(exit = EXIT (ch, dir)))
	continue;
      if (!(next_room = vtor (exit->to_room)))
	continue;
      if (dir == 0)
	from_dir = 2;
      else if (dir == 1)
	from_dir = 3;
      else if (dir == 2)
	from_dir = 0;
      else if (dir == 3)
	from_dir = 1;
      else if (dir == 4)
	from_dir = 5;
      else
	from_dir = 4;
      for (af = next_room->affects; af; af = af->next)
	{
	  if (af->type != MAGIC_ROOM_FIGHT_NOISE)
	    continue;
	  if (af->a.room.duration != from_dir)
	    continue;
	  found = true;
	}
      if (!found)
	{
	  if (!next_room->affects)
	    {
	      next_room->affects =
		(AFFECTED_TYPE *) alloc (sizeof (AFFECTED_TYPE), 13);
	      next_room->affects->type = MAGIC_ROOM_FIGHT_NOISE;
	      next_room->affects->a.room.duration = from_dir;
	      next_room->affects->next = NULL;
	    }
	  else
	    for (af = next_room->affects; af; af = af->next)
	      {
		if (!af->next)
		  {
		    af->next =
		      (AFFECTED_TYPE *) alloc (sizeof (AFFECTED_TYPE), 13);
		    af->next->type = MAGIC_ROOM_FIGHT_NOISE;
		    af->next->a.room.duration = from_dir;
		    af->next->next = NULL;
		    break;
		  }
	      }
	  sprintf (buf, "You hear the sounds of armed battle erupt from %s!",
		   e_dirs[from_dir]);
	  send_to_room (buf, next_room->nVirtual);
	}
    }
}

void
set_fighting (CHAR_DATA * ch, CHAR_DATA * vict)
{
  AFFECTED_TYPE *af;

  clear_pmote (ch);

  if (IS_SET (ch->flags, FLAG_COMPETE) || IS_SET (vict->flags, FLAG_COMPETE))
    return;

  if (ch->fighting)
    {
      return;
    }

  if (ch == vict)		/* No way, not in this game :( */
    return;

  ch->next_fighting = combat_list;
  combat_list = ch;

  //if ((af = get_affect (ch, MAGIC_AFFECT_SLEEP)))
  //  affect_remove (ch, af);

  if ((GET_FLAG (ch, FLAG_AUTOFLEE) 
   || (get_soma_affect(ch, SOMA_PLANT_VISIONS) && number(0,20) > GET_WIL(ch))) && AWAKE (ch))
    {
      send_to_char ("You try to escape!\n\r", ch);
      act ("$n tries to escape!", false, ch, 0, 0, TO_ROOM);
      ch->flags |= FLAG_FLEE;
    }

  if ((!morale_broken (vict) && !IS_SET (vict->act, ACT_SENTINEL) &&
	(IS_SET (vict->act, ACT_AGGRESSIVE)
	 || IS_SET (vict->act, ACT_ENFORCER))) && !vict->following)
    {
      /* vict->following = ch; */
      vict->speed = ch->speed;
    }

  if ((!morale_broken (ch) && !IS_SET (ch->act, ACT_SENTINEL) &&
	(IS_SET (ch->act, ACT_AGGRESSIVE) || IS_SET (ch->act, ACT_ENFORCER)))
      && !ch->following)
    {
      /* vict->following = ch; */
      ch->speed = vict->speed;
    }

  if ((af = is_crafting (ch)))
    {
      act ("$n stops doing $s craft.", false, ch, 0, 0, TO_ROOM);
      send_to_char ("You stop doing your craft.\n", ch);
      af->a.craft->timer = 0;
    }

  if ((af = is_crafting (vict)))
    {
      act ("$n stops doing $s craft.", false, vict, 0, 0, TO_ROOM);
      send_to_char ("You stop doing your craft.\n", vict);
      af->a.craft->timer = 0;
    }

  ch->fighting = vict;

  /* fighting_sounds(ch, ch->room); */

  add_threat (vict, ch, 2);
  add_threat (ch, vict, 2);

  clear_moves (ch);
  clear_moves (vict);

  if (IS_NPC (ch) && !ch->desc && (is_area_enforcer (ch))
      && IS_SET (vict->flags, FLAG_KILL) && !get_affect(ch, MAGIC_ALERTED))
  {
    do_alert (ch, "", 0);
    magic_add_affect (ch, MAGIC_ALERTED, 90, 0, 0, 0, 0);
  }

  if (IS_NPC (vict) && !vict->desc && (is_area_enforcer (vict))
      && IS_SET (ch->flags, FLAG_KILL) && !get_affect(vict, MAGIC_ALERTED))
  {
    do_alert (vict, "", 0);
    magic_add_affect (vict, MAGIC_ALERTED, 90, 0, 0, 0, 0);
  }
}

void
stop_fighting (CHAR_DATA * ch)
{
  CHAR_DATA *tch;
  bool fighting = false;

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (tch == ch)
	continue;
      if (tch->fighting)
	fighting = true;
    }

  if (ch == combat_next_dude)
    combat_next_dude = ch->next_fighting;

  if (combat_list == ch)
    combat_list = ch->next_fighting;
  else
    {
      for (tch = combat_list; tch && (tch->next_fighting != ch);
	   tch = tch->next_fighting);
      if (!tch)
	{
	  system_log
	    ("Char fighting not found Error (fight.c, stop_fighting)", true);
	  sigsegv (SIGSEGV);
	}
      tch->next_fighting = ch->next_fighting;
    }

  ch->next_fighting = 0;
  ch->fighting = 0;
  if (GET_POS (ch) == FIGHT)
  	{
    GET_POS (ch) = STAND;
		remove_cover(ch,0);
		}
  ch->flags &= ~FLAG_KILL;

  if (ch->mount && !IS_SET (ch->act, ACT_MOUNT) && ch->mount->fighting)
    stop_fighting (ch->mount);

  if (IS_NPC (ch))
    {
      ch->speed = 0;
      ch->threats = NULL;
      ch->attackers = NULL;
    }
}

void
make_statue (CHAR_DATA * ch)
{
  OBJ_DATA *statue;
  OBJ_DATA *o;
  WOUND_DATA *wound;
  WOUND_DATA *cwound;
  LODGED_OBJECT_INFO *lodged;
  LODGED_OBJECT_INFO *clodged;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  int i;

  statue = load_object (VNUM_STATUE);

  if (!IS_NPC (ch))
    sprintf (buf, "statue pc_%s", GET_NAME (ch));
  else
    {
      one_argument (GET_NAME (ch), buf2);
      sprintf (buf, "statue npc_%s", buf2);
    }

  statue->name = str_dup (buf);

  sprintf (buf, "An eerily lifelike statue of %s looms here.",
	   ch->short_descr);
  statue->description = str_dup (buf);

  sprintf (buf, "an eerily lifelike statue of %s", ch->short_descr);
  statue->short_description = str_dup (buf);

  for (wound = ch->wounds; wound; wound = wound->next)
    {
      if (!statue->wounds)
	{
	  CREATE (statue->wounds, WOUND_DATA, 1);
	  statue->wounds->location = add_hash (wound->location);
	  statue->wounds->type = add_hash (wound->type);
	  statue->wounds->name = add_hash (wound->name);
	  statue->wounds->severity = add_hash (wound->severity);
	  statue->wounds->bleeding = 0;
	  statue->wounds->poison = wound->poison;
	  statue->wounds->infection = wound->infection;
	  statue->wounds->healerskill = wound->healerskill;
	  statue->wounds->lasthealed = wound->lasthealed;
	  statue->wounds->lastbled = wound->lastbled;
	  statue->wounds->next = NULL;
	}
      else
	for (cwound = statue->wounds; cwound; cwound = cwound->next)
	  {
	    if (cwound->next)
	      continue;
	    CREATE (cwound->next, WOUND_DATA, 1);
	    cwound->next->location = add_hash (wound->location);
	    cwound->next->type = add_hash (wound->type);
	    cwound->next->name = add_hash (wound->name);
	    cwound->next->severity = add_hash (wound->severity);
	    cwound->next->bleeding = 0;
	    cwound->next->poison = wound->poison;
	    cwound->next->infection = wound->infection;
	    cwound->next->healerskill = wound->healerskill;
	    cwound->next->lasthealed = wound->lasthealed;
	    cwound->next->lastbled = wound->lastbled;
	    cwound->next->next = NULL;
	    break;
	  }
    }

  for (lodged = ch->lodged; lodged; lodged = lodged->next)
    {
      if (!statue->lodged)
	{
	  CREATE (statue->lodged, LODGED_OBJECT_INFO, 1);
	  statue->lodged->vnum = lodged->vnum;
	  statue->lodged->location = add_hash (lodged->location);
	  statue->lodged->next = NULL;
	}
      else
	for (clodged = statue->lodged; clodged; clodged = clodged->next)
	  {
	    if (!clodged->next)
	      {
		CREATE (clodged->next, LODGED_OBJECT_INFO, 1);
		clodged->next->vnum = lodged->vnum;
		clodged->next->location = add_hash (lodged->location);
		clodged->next->next = NULL;
		break;
	      }
	  }
    }

  if (ch->right_hand)
    {
      o = ch->right_hand;
      ch->right_hand = NULL;
      o->equiped_by = NULL;
      o->carried_by = NULL;
      o->next_content = NULL;
      obj_to_obj (o, statue);
    }

  if (ch->left_hand)
    {
      o = ch->left_hand;
      ch->left_hand = NULL;
      o->equiped_by = NULL;
      o->carried_by = NULL;
      o->next_content = NULL;
      obj_to_obj (o, statue);
    }

  statue->obj_flags.weight = get_weight (ch) * 5;

  for (i = 0; i < MAX_WEAR; i++)
    if (get_equip (ch, i))
      obj_to_obj (unequip_char (ch, i), statue);

  IS_CARRYING_N (ch) = 0;

  obj_to_room (statue, ch->in_room);
}

#define MAX_NPC_CORPSE_TIME	192	/* 2 RL day -- corpses are saved. */
#define MAX_PC_CORPSE_TIME	384	/* 4 RL day -- corpses are saved. */
#define ASHES_VNUM		1651
void
make_corpse (CHAR_DATA * ch)
{
  OBJ_DATA *corpse;
  OBJ_DATA *o;
  WOUND_DATA *wound;
  WOUND_DATA *cwound;
  LODGED_OBJECT_INFO *lodged;
  LODGED_OBJECT_INFO *clodged;
  char buf[MAX_STRING_LENGTH];
  int i;

  corpse = load_object (VNUM_CORPSE);

  if (!IS_NPC (ch))
    sprintf (buf, "corpse pc %s", GET_NAMES (ch));
  else
    {
      sprintf (buf, "corpse npc %s", GET_NAMES (ch));
    }

  corpse->name = str_dup (buf);

  sprintf (buf, "The corpse of %s is lying here.", ch->short_descr);
  corpse->description = str_dup (buf);

  sprintf (buf, "the corpse of %s", ch->short_descr);
  corpse->short_description = str_dup (buf);

  for (wound = ch->wounds; wound; wound = wound->next)
    {
      if (!corpse->wounds)
	{
	  CREATE (corpse->wounds, WOUND_DATA, 1);
	  corpse->wounds->location = add_hash (wound->location);
	  corpse->wounds->type = add_hash (wound->type);
	  corpse->wounds->name = add_hash (wound->name);
	  corpse->wounds->severity = add_hash (wound->severity);
	  corpse->wounds->bleeding = 0;
	  corpse->wounds->poison = wound->poison;
	  corpse->wounds->infection = wound->infection;
	  corpse->wounds->healerskill = wound->healerskill;
	  corpse->wounds->lasthealed = wound->lasthealed;
	  corpse->wounds->lastbled = wound->lastbled;
	  corpse->wounds->next = NULL;
	}
      else
	for (cwound = corpse->wounds; cwound; cwound = cwound->next)
	  {
	    if (cwound->next)
	      continue;
	    CREATE (cwound->next, WOUND_DATA, 1);
	    cwound->next->location = add_hash (wound->location);
	    cwound->next->type = add_hash (wound->type);
	    cwound->next->name = add_hash (wound->name);
	    cwound->next->severity = add_hash (wound->severity);
	    cwound->next->bleeding = 0;
	    cwound->next->poison = wound->poison;
	    cwound->next->infection = wound->infection;
	    cwound->next->healerskill = wound->healerskill;
	    cwound->next->lasthealed = wound->lasthealed;
	    cwound->next->lastbled = wound->lastbled;
	    cwound->next->next = NULL;
	    break;
	  }
    }

  for (lodged = ch->lodged; lodged; lodged = lodged->next)
    {
      if (!corpse->lodged)
	{
	  CREATE (corpse->lodged, LODGED_OBJECT_INFO, 1);
	  corpse->lodged->vnum = lodged->vnum;
	  corpse->lodged->location = add_hash (lodged->location);
	  corpse->lodged->next = NULL;
          corpse->lodged->colored = 0;

          if(lodged->colored)
          {
            corpse->lodged->colored = 1;
            corpse->lodged->var_color = lodged->var_color;
            corpse->lodged->var_color2 = lodged->var_color2;
            corpse->lodged->var_color3 = lodged->var_color3;
            corpse->lodged->short_description = lodged->short_description;
          }
	}
      else
	for (clodged = corpse->lodged; clodged; clodged = clodged->next)
	  {
	    if (!clodged->next)
	      {
		CREATE (clodged->next, LODGED_OBJECT_INFO, 1);
		clodged->next->vnum = lodged->vnum;
		clodged->next->location = add_hash (lodged->location);

                clodged->next->colored = 0;

                if(lodged->colored)
                {
                  clodged->next->colored = 1;
                  clodged->next->var_color = lodged->var_color;
                  clodged->next->var_color2 = lodged->var_color2;
                  clodged->next->var_color3 = lodged->var_color3;
                  clodged->next->short_description = lodged->short_description;
                }

		clodged->next->next = NULL;
		break;
	      }
	  }
    }

  if (ch->right_hand)
    {
      o = ch->right_hand;
      ch->right_hand = NULL;
      o->equiped_by = NULL;
      o->carried_by = NULL;
      o->next_content = NULL;
      obj_to_obj (o, corpse);
    }

  if (ch->left_hand)
    {
      o = ch->left_hand;
      ch->left_hand = NULL;
      o->equiped_by = NULL;
      o->carried_by = NULL;
      o->next_content = NULL;
      obj_to_obj (o, corpse);
    }

  if (ch->mob)
    {
      if (GET_FLAG (ch, FLAG_WILLSKIN))
	{
	  corpse->o.od.value[2] = -ch->mob->skinned_vnum;
	  corpse->o.od.value[3] = -ch->mob->carcass_vnum;
	}
      else
	{
	  corpse->o.od.value[2] = ch->mob->skinned_vnum;
	  corpse->o.od.value[3] = ch->mob->carcass_vnum;
	}
    }
  else
    {
      corpse->o.od.value[2] = 0;
      corpse->o.od.value[3] = 0;
    }

  corpse->o.od.value[4] = ch->race;

  corpse->obj_flags.weight = get_weight (ch);

  if (IS_NPC (ch))
    corpse->obj_timer = MAX_NPC_CORPSE_TIME;
  else
    corpse->obj_timer = MAX_PC_CORPSE_TIME;

  corpse->obj_flags.extra_flags |= ITEM_TIMER;

  for (i = 0; i < MAX_WEAR; i++)
    {
      if (get_equip (ch, i))
	{
	  if (GET_ITEM_TYPE (get_equip (ch, i)) == ITEM_CONTAINER
	      && IS_SET (ch->plr_flags, NEW_PLAYER_TAG))
	    continue;

          if (GET_ITEM_TYPE (get_equip (ch, i)) == ITEM_RESOURCE)
            obj_to_room(unequip_char (ch, i), ch->in_room);
          else
  	    obj_to_obj (unequip_char (ch, i), corpse);
	}
    }

  IS_CARRYING_N (ch) = 0;


  // Wights shouldn't leave corpses (temp fix)
  if (ch->race == 30)
    {

      send_to_room ("\n", ch->in_room);
      if (corpse->contains)
	{
	  act
	    ("$n disintegrates into ashes as it collapses to the ground, dropping its belongings:",
	     false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
	}
      else
	{
	  act
	    ("$n disintegrates into ashes as it collapses to the ground.",
	     false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
	}

      if ((o = load_object (ASHES_VNUM)) != NULL)
	{
	  obj_to_room (o, ch->in_room);
	}

      while (corpse->contains)
	{
	  o = corpse->contains;
	  obj_from_obj (&o, 0);
	  obj_to_room (o, ch->in_room);
	  act ("    $p", false, ch, o, 0, TO_CHAR);
	  act ("    $p", false, ch, o, 0, TO_ROOM);
	}
      extract_obj (corpse);
      return;
    }

  obj_to_room (corpse, ch->in_room);
}

void
remove_guest_skills (CHAR_DATA * ch)
{
  int i;

  if (!IS_SET (ch->flags, FLAG_GUEST))
    return;

  for (i = 0; i <= MAX_SKILLS; i++)
    {
      ch->skills[i] = 0;
      ch->pc->skills[i] = 0;
    }
/*
        ch->speaks = race_table[ch->race].race_speaks;
        ch->skills [ch->speaks] = calc_lookup(ch, REG_CAP, race_table[ch->race].race_speaks);
        ch->pc->skills [ch->speaks] = ch->skills [ch->speaks];
*/
}

void
death_email (CHAR_DATA * ch)
{
  char buf[MAX_STRING_LENGTH];
  MYSQL_RES *result;
  MYSQL_ROW row;
  account *acct;
  FILE *fp;
  bool found = false;

  if (!ch || !ch->pc || !ch->pc->account_name)
    return;
  acct = new account (ch->pc->account_name);
  if (!acct->is_registered ())
    {
      delete acct;
      return;
    }

  if (!(fp = fopen (ch->tname, "w+")))
    {
      return;
    }


  fprintf (fp, "From: %s <%s>\n"
	   "To: %s\n"
	   "X-Sender: %s"
	   "Mime-Version: 1.0\n"
	   "Content-type: text/plain;charset=\"us-ascii\"\n"
	   "Organization: %s\n"
	   "Subject: %s's Recent Demise\n"
	   "\n", MUD_NAME, MUD_EMAIL, acct->email.c_str (), MUD_EMAIL, MUD_NAME, ch->tname);

  result = NULL;

  fprintf (fp, "Hello,\n\n"
	   "   Our records indicate that your PC, %s, has recently passed on. For\n"
	   "your convenience and future reference, we have taken the liberty of\n"
	   "compiling all of %s journal entries, in-game board posts and in-\n"
	   "character writings; they are attached below.\n\n"
	   "   Thanks for playing, and we hope to see you back again soon.\n"
	   "\n"
	   "\n"
	   "                                           Best Regards,\n"
	   "                                           The Admin Team\n\n",
	   ch->tname, HSHR (ch));

  mysql_safe_query
    ("SELECT * FROM player_journals WHERE name = \'%s\' ORDER BY post_number ASC",
     ch->tname);
  result = mysql_store_result (database);
  if (mysql_num_rows (result))
    {
      fprintf (fp, "Journal Entries:\n\n");
      while ((row = mysql_fetch_row (result)))
	{
	  found = true;
	  fprintf (fp, "--\nDate: %s\nSubject: %s\n\n%s\n", row[4], row[2],
		   row[5]);
	}
      fprintf (fp, "\n");
      mysql_free_result (result);
      result = NULL;
    }

  mysql_safe_query
    ("SELECT * FROM boards WHERE author = \'%s\' ORDER BY board_name,post_number ASC",
     ch->tname);
  result = mysql_store_result (database);
  if (mysql_num_rows (result))
    {
      fprintf (fp, "In-Game Board Posts:\n\n");
      while ((row = mysql_fetch_row (result)))
	{
	  found = true;
	  fprintf (fp, "--\nDate: %s [%s]\nSubject: %s\n\n%s\n", row[5],
		   row[4], row[2], row[6]);
	}
      fprintf (fp, "\n");
      mysql_free_result (result);
      result = NULL;
    }

  mysql_safe_query
    ("SELECT * FROM player_writing WHERE author = \'%s\' ORDER BY db_key,page ASC",
     ch->tname);
  result = mysql_store_result (database);
  if (mysql_num_rows (result))
    {
      fprintf (fp, "In-Character Writings:\n\n");
      while ((row = mysql_fetch_row (result)))
	{
	  found = true;
	  fprintf (fp, "--\nDate: %s\n\n%s\n", row[3], row[8]);
	}
      fprintf (fp, "\n");
      mysql_free_result (result);
      result = NULL;
    }

  if (!found)
    fprintf (fp, "--\nNo writing was found in our database.\n");

  fclose (fp);

  sprintf (buf, "/usr/sbin/sendmail %s < %s", acct->email.c_str (), ch->tname);
  system (buf);

  unlink (ch->tname);

  delete acct;
}

// A program that replaces the boring "$n collapses to the ground dead"
// with a wider variety of death messages. Also useful in compressed
// combat messages to work out who kills who.

void
violent_death (CHAR_DATA *ch)
{
  char buf[MAX_STRING_LENGTH];
  OBJ_DATA * obj;
  int aw_type;
 
  obj = vtoo (ch->delay_info1);

  aw_type = obj->o.weapon.hit_type;

  sprintf(buf, "$N is %s by $n's %s %s, %s!", 
    deathhit[aw_type * 3 + number (0,2)],
    crithit[aw_type * 2 + number (0, 1)],
    wtype[aw_type * 3 + number (0, 2)],
    (GET_POS(ch) >= 7 ? "collapsing to the ground, dead" : "expiring with a ragged exhalation"));
    
  act (buf, true, ch->delay_ch, 0, ch, TO_NOTVICT | _ACT_FORMAT);    
  act (buf, true, ch->delay_ch, 0, ch, TO_VICT | _ACT_FORMAT);

  sprintf(buf, "$N is %s by #5your#0 %s %s, %s!", 
    deathhit[aw_type * 3 + number (0,2)],
    crithit[aw_type * 2 + number (0, 1)],
    wtype[aw_type * 3 + number (0, 2)],
    (GET_POS(ch) >= 7 ? "collapsing to the ground, dead" : "expiring with a ragged exhalation"));

  act (buf, true, ch->delay_ch, 0, ch, TO_CHAR | _ACT_FORMAT);

}

void
raw_kill (CHAR_DATA * ch)
{
  CHAR_DATA *tch;
  DESCRIPTOR_DATA *d;

  if (ch->fighting)
    {
      if (ch == combat_next_dude)
	combat_next_dude = ch->next_fighting;

      if (combat_list == ch)
	combat_list = ch->next_fighting;
      else
	{
	  for (tch = combat_list; tch && (tch->next_fighting != ch);
	       tch = tch->next_fighting);
	  if (!tch)
	    {
	      system_log
		("Char fighting not found Error (fight.c, stop_fighting)",
		 true);
	      sigsegv (SIGSEGV);
	    }
	  tch->next_fighting = ch->next_fighting;
	}

      ch->next_fighting = 0;
      ch->fighting = 0;
    }

  for (tch = character_list; tch; tch = tch->next)
    {
      if (tch->deleted)
	continue;
      if (tch->aiming_at == ch && ch->room != tch->room)
	act ("$n collapses, slain. You lower your weapon.", false, ch, 0, tch,
	     TO_VICT | _ACT_FORMAT);
    }

  if (!IS_SET (ch->plr_flags, FLAG_PETRIFIED))
    make_corpse (ch);
  else
    make_statue (ch);

  if (IS_SET (ch->plr_flags, FLAG_PETRIFIED))
    {
      act
	("$n suddenly seems to grow stiff, turning to stone before your very eyes!",
	 true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      act
	("As the hated rays of the sun strike your skin, you suddenly grow stiff, and your mind is awash in helpless rage before -- nothing...",
	 true, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
    }
  else if (!ch->mount && ch->race != 30)
    {
      send_to_room ("\n", ch->in_room);
      if (ch->delay_ch && ch->delay_info1)
        violent_death(ch);
      else if (GET_POS (ch) >= 7)
	act ("$n collapses to the ground, dead.", true, ch, 0, 0,
	     TO_ROOM | _ACT_FORMAT);
      else
	act ("$n expires with a ragged exhalation.", true, ch, 0, 0,
	     TO_ROOM | _ACT_FORMAT);
      ch->mount = NULL;
    }
  else if (ch->mount && IS_SET (ch->act, ACT_MOUNT))
    {
      send_to_room ("\n", ch->in_room);
      act ("$n collapses, dead, dumping $N to the ground in the process.",
	   true, ch, 0, ch->mount, TO_NOTVICT | _ACT_FORMAT);
      act ("$n collapses, dead, dumping you to the ground in the process.",
	   true, ch, 0, ch->mount, TO_VICT | _ACT_FORMAT);
      ch->mount->mount = NULL;
      ch->mount = NULL;
    }
  else if (ch->mount && !IS_SET (ch->act, ACT_MOUNT))
    {
      send_to_room ("\n", ch->in_room);
      act ("$n falls to the ground from atop $N, dead.", true, ch, 0,
	   ch->mount, TO_NOTVICT | _ACT_FORMAT);
      act ("$n falls to the ground from atop you, dead.", true, ch, 0,
	   ch->mount, TO_VICT | _ACT_FORMAT);
      ch->mount->mount = NULL;
      ch->mount = NULL;
    }

  if ((!IS_NPC (ch) && !IS_SET (ch->flags, FLAG_GUEST))
      || (IS_SET (ch->flags, FLAG_GUEST) && ch->desc))
    {
      GET_POS (ch) = POSITION_STANDING;
      while (ch->wounds)
	wound_from_char (ch, ch->wounds);
      while (ch->lodged)
	lodge_from_char (ch, ch->lodged);
      ch->damage = 0;
      if (IS_SET (ch->flags, FLAG_GUEST))
	{
	  create_guest_avatar (ch->desc, "recreate");
	}
      else
	{
	  d = ch->desc;
	  clan_forum_remove_all (ch);
	  extract_char (ch);
	  SEND_TO_Q
	    ("#0Your character has, regrettably, passed away. Our condolences. . .#0\n\n"
	     "#0Thank you for playing - we hope to see you back again soon!#0\n",
	     d);
	  if (d)
	    {
	      d->connected = CON_ACCOUNT_MENU;
	      nanny (d, "");
	    }
	  if (IS_MORTAL (ch) && !ch->pc->mortal_mode)
	    {
	      death_email (ch);
	      mysql_safe_query
		("UPDATE newsletter_stats SET pc_deaths=pc_deaths+1");
	    }
	}
    }
  else
    {
      extract_char (ch);
    }
}

void
die (CHAR_DATA * ch)
{
  int duration = 0;
  time_t time_now;
  char *date;
  char buf[MAX_STRING_LENGTH];
  char msg[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  // Spare the lives of Immortals (Crash Prevention Program #823)
  if (ch->pc && ch->pc->level)
    {
      heal_all_wounds (ch);
      ch->move = ch->max_move;
      return;
    }

  if (ch->room->nVirtual == 60003 || ch->room->nVirtual == 60004
      || ch->room->nVirtual == 60002 || ch->room->nVirtual == 60001)
    {
      arena__death (ch);
    }

if (ch->in_room == 66953 || ch->in_room == 66954 || ch->in_room == 66955 || ch->in_room == 66956)
    {
      te_pit_death (ch);
    }
    
  if (ch->combat_log)
    {
      system_log (ch->combat_log, false);
    }

  if (!IS_NPC (ch))
    {

      if (!IS_SET (ch->flags, FLAG_GUEST))
	{

	  time_now = time (0);
	  date = (char *) asctime (localtime (&time_now));
	  date[strlen (date) - 1] = '\0';

	  if (IS_SET (ch->plr_flags, FLAG_PETRIFIED))
	    sprintf (msg, "Location: %s [%d]\n\nDied of petrification.\n",
		     ch->room->name, ch->in_room);
	  else if (ch->delay_info1 && ch->delay_ch
		   && !IS_SET (ch->flags, FLAG_BINDING))
	    {
	      sprintf (buf2, " [%s]", ch->delay_ch->tname);
	      sprintf (msg,
		       "Location: %s [%d]\n\nAssailant: #5%s#0%s\n\nWeapon: #2%s#0\n",
		       ch->room->name, ch->in_room, ch->delay_ch->short_descr,
		       !IS_NPC (ch->delay_ch) ? buf2 : "",
		       vtoo (ch->delay_info1)->short_description);
	    }
	  else if (ch->delay_ch && !ch->delay_info1
		   && !IS_SET (ch->flags, FLAG_BINDING))
	    {
	      sprintf (buf2, " [%s]", ch->delay_ch->tname);
	      sprintf (msg, "Location: %s [%d]\n\nAssailant: #5%s#0%s\n",
		       ch->room->name, ch->in_room, ch->delay_ch->short_descr,
		       !IS_NPC (ch->delay_ch) ? buf2 : "");
	    }
	  else
	    sprintf (msg,
		     "Location: %s [%d]\n\nDied of unspecified causes, e.g. bloodloss or falling.\n",
		     ch->room->name, ch->in_room);

	  add_message (1, ch->tname, -2, "Server", date, "Died.", "", msg, 0);
	  add_message (1, "Deaths", -5, "Server", date, ch->tname, "", msg,
		       0);

	  mem_free (date);
	}

      if (!IS_SET (ch->flags, FLAG_GUEST))
	{
	  if (!IS_SET (ch->plr_flags, FLAG_PETRIFIED))
	    sprintf (buf, "%s has been slain! [%d]\n", ch->tname,
		     ch->in_room);
	  else
	    sprintf (buf, "%s has been petrified! [%d]\n", ch->tname,
		     ch->in_room);
	  send_to_gods (buf);
	}


/*

      Moved this below, to implement some interesting death messages.

      ch->delay_ch = 0;
      ch->delay_info1 = 0;
*/

      /* NOTE:  af doesn't point to anything after affects are removed */

      if (duration > 100)	/* Fix a bug where some players have a LARGE */
	duration = 1;		/* number of hours */
/*
      while (ch->hour_affects)   Moved this so it happens after the call to save_char, should take care of not deleting crafts from a character upon death
      affect_remove (ch, ch->hour_affects);
*/

      ch->pc->create_state = STATE_DIED;
      ch->pc->last_died = time (0);

      save_char (ch, true);

      while (ch->hour_affects)
	affect_remove (ch, ch->hour_affects);
    }

  raw_kill (ch);

  ch->delay_ch = 0;
  ch->delay_info1 = 0;

  if (IS_SET (ch->plr_flags, FLAG_PETRIFIED))
    ch->plr_flags &= ~FLAG_PETRIFIED;
}

#define		SINGLE		1
#define		PRIMARY		2
#define		SECONDARY	3

#define ADDBUF debug_buf + strlen (debug_buf)


// Procedure for striking individual bodyparts

void
do_aimstrike (CHAR_DATA * ch, char *argument, int cmd)
{
  char arg[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  CHAR_DATA * victim;
  int bodypart;
  int delay;
  bool target = false;

// This is only temporary.
// Ultimately, you can strike from nothing, to allow one-punch fights and executions.

  if (!ch->fighting)
  {
    send_to_char ("You need to be fighting to strike at a specific area.\n", ch);
    return;
  }

  victim = ch->fighting;

  if(get_second_affect (ch, SA_AIMSTRIKE, NULL))
  {
    send_to_char ("You are already making an aimed strike.\n", ch);
    return;
  }

  if (ch->balance <= -15)
  {
    send_to_char ("You're far too off-balance to attempt an aimed strike.\n", ch);
    return;
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




  argument = one_argument (argument, arg);

  if (!*arg)
    {
      send_to_char ("Which bodypart are you trying to hit?\n", ch);
      return;
    }
  if (!strcmp(arg, "body"))
  {
    bodypart = 0;
    delay = 130;
  }
  else if (!strcmp(arg, "leg") || !strcmp(arg, "legs"))
  {
    bodypart = 1;
    delay = 100;
  }
  else if (!strcmp(arg, "arm") || !strcmp(arg, "arms"))
  {
    bodypart = 2;
    delay = 100;
  }
  else if (!strcmp(arg, "head"))
  {
    if(GET_POS (victim) == SIT || GET_POS(victim) == REST)
    {
      target = true;
    }
    bodypart = 3;
    delay = 160;
  }
  else if (!strcmp(arg, "neck"))
  {
    if(GET_POS (victim) == SIT || GET_POS(victim) == REST)
    {
      target = true;
    }
    bodypart = 4;
    delay = 190;
  }
  else if (!strcmp(arg, "foot") || !strcmp(arg, "feet"))
  {
    bodypart = 5;
    delay = 80;
  }
  else if (!strcmp(arg, "hand") || !strcmp(arg, "hands"))
  {
    bodypart = 6;
    delay = 80;
  }
  else if (!strcmp(arg, "eye") || !strcmp(arg, "eyes"))
  {
    if(GET_POS (victim) == SIT || GET_POS(victim) == REST)
    {
      target = true;
    }
    bodypart = 7;
    delay = 300;
  }
  else
  {
    send_to_char ("That's not a bodypart! Choose head, neck, arms, body, legs or eyes.\n", ch);
    return;
  }

  if(target)
  {
    sprintf(buf, "You're unable to easily strike $N's %s in $S current position. Aim elsewhere.", arg);
    act (buf, true, ch, 0, victim, TO_CHAR | _ACT_FORMAT);
    return;
  }
  else
  {
    sprintf (buf, "You take aim at $N's %s.", arg);
    act (buf, true, ch, 0, victim, TO_CHAR | _ACT_FORMAT);
    sprintf (buf, "$n takes aim at your %s.", arg);
    act (buf, true, ch, 0, victim, TO_VICT | _ACT_FORMAT);
    add_second_affect (SA_AIMSTRIKE, delay, ch, NULL, NULL, bodypart);
    ch->primary_delay += 8;
    ch->secondary_delay += 8;
  }
}

void
do_combat_feint (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA * victim = NULL;
  OBJ_DATA * prim = NULL;
  OBJ_DATA * sec = NULL;

  if (!ch->fighting)
  {
    send_to_char ("You need to be fighting to make a feinted strike.\n", ch);
    return;
  }

  if (!real_skill (ch, SKILL_DUAL) || ch->skills[SKILL_DUAL] < 50)
  {
    send_to_char ("You're not proficient enough to attempt such a manevour.\n", ch);
    return;
  }

  if (ch->balance <= -1)
  {
    send_to_char ("You're far too off-balance to attempt a feinted strike.\n", ch);
    return;
  }

  if(get_second_affect (ch, SA_FEINT, NULL))
  {
    send_to_char ("You are already making a feinted strike.\n", ch);
    return;
  }

  prim = get_equip (ch, WEAR_PRIM);
  sec = get_equip (ch, WEAR_SEC);

  if (prim && sec)
  {
    if(!(prim->obj_flags.type_flag == ITEM_WEAPON && sec->obj_flags.type_flag == ITEM_WEAPON))
    {
      send_to_char ("You need to be wielding two weapons in order to do that.\n", ch);
      return;
    } 
  }
  else
  {
    send_to_char ("You need to be wielding two weapons in order to do that.\n", ch);
    return;
  } 
  
  victim = ch->fighting;

  ch->balance += -30;
  ch->balance = MAX (ch->balance, -30);

  if (!number(0,5))
    skill_use(ch, SKILL_DUAL, 0);

  act ("You prepare to launch a feinted strike against $N.", true, ch, 0, victim, TO_CHAR | _ACT_FORMAT);
  add_second_affect (SA_FEINT, 60, ch, NULL, NULL, 0);
  
}

void
do_combat_ward (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA * victim = NULL;
  CHAR_DATA * rch = NULL;
  OBJ_DATA * weapon = NULL;
  int attackers = 0;
  bool pass = false;

  if (!ch->fighting)
  {
    send_to_char ("You need to be fighting to ward off foes strike.\n", ch);
    return;
  }

  if (!real_skill (ch, SKILL_LARGE_WEAPON) || ch->skills[SKILL_LARGE_WEAPON] < 50)
  {
    send_to_char ("You're not proficient enough to attempt such a manevour.\n", ch);
    return;
  }

  if (ch->balance <= -1)
  {
    send_to_char ("You're far too off-balance to ward off your attackers.\n", ch);
    return;
  }

  weapon = get_equip (ch, WEAR_BOTH);

  if (!weapon || weapon->obj_flags.type_flag != ITEM_WEAPON)
  {
    send_to_char ("You need to be wielding a weapon with both hands to do that.\n", ch);
    return;
  }
  
  victim = ch->fighting;

  ch->balance += -30;
  ch->balance = MAX (ch->balance, -30);

  for (rch = ch->room->people; rch; rch = rch->next_in_room)
  {
    if (rch->fighting == ch && rch != ch->fighting)
      attackers++;
  }

  if (skill_use(ch, SKILL_LARGE_WEAPON, -10))
    pass = true;

  if (!attackers)
  {
    if (pass)
    {
      act ("Using $p, you knock $N off balance.", true, ch, weapon, victim, TO_CHAR | _ACT_FORMAT | _ACT_COMBAT);
      act ("$n knocks you off balance with $p.", true, ch, weapon, victim, TO_VICT | _ACT_FORMAT);
      act ("$n knocks $N off balance with $p.", true, ch, weapon, victim, TO_NOTVICT | _ACT_FORMAT);
      victim->balance += -10;
      victim->primary_delay -= 8;
      victim->secondary_delay -= 8;
    }
    else
    {
      act ("You attempt to knock $N off balance with $p, but fail, unbalancing yourself instead.", true, ch, weapon, victim, TO_CHAR | _ACT_FORMAT | _ACT_COMBAT);
      ch->primary_delay -= 8;
    }
  }
  else
  {
    if (!pass)
    {
      act ("You attempt to ward off your attackers with $p, but fail, unbalancing yourself instead.", true, ch, weapon, victim, TO_CHAR | _ACT_FORMAT | _ACT_COMBAT);
      ch->primary_delay -= 8;
    }
    else
    {

      act ("Using $p, you force those attacking you to keep their distance.", true, ch, weapon, NULL, TO_CHAR | _ACT_FORMAT | _ACT_COMBAT);
      act ("$n uses $p to keep $s attackers at distance", true, ch, weapon, NULL, TO_ROOM | _ACT_FORMAT | _ACT_COMBAT);

      while(attackers>0)
      {
        for (rch = ch->room->people; rch; rch = rch->next_in_room)
        {
          if (skill_use(ch, SKILL_LARGE_WEAPON, 0) && rch->fighting == ch && ch->fighting != rch)
          {
            act ("$n forces you back with skillful use of $p.", 
              true, ch, weapon, rch, TO_VICT | _ACT_FORMAT);        
            stop_fighting (rch);
            add_second_affect (SA_WARDED, 60, rch, (OBJ_DATA *) ch, NULL, 0);
          }
        }
        attackers--;
      }
    }
  }

}

void
do_combat_bash (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char buf3[MAX_STRING_LENGTH];
  char strike_location[AVG_STRING_LENGTH];
  CHAR_DATA * victim = NULL;
  OBJ_DATA * shield = NULL;
  int defense = 0;
  int damage = 0;
  int roll = 0;
  int result = 0;
  OBJ_DATA *armor = NULL;
  int wear_loc = 0;

  if (!ch->fighting)
  {
    send_to_char ("You need to be fighting to bash someone.\n", ch);
    return;
  }

  if (ch->balance <= -1)
  {
    send_to_char ("You're far too off-balance to attempt to bash someone.\n", ch);
    return;
  }

  if (!real_skill (ch, SKILL_SHIELD_STYLE) || ch->skills[SKILL_SHIELD_STYLE] < 50)
  {
    send_to_char ("You're not proficient enough to attempt such a manevour.\n", ch);
    return;
  }

  shield = get_equip (ch, WEAR_SHIELD);

  if ((shield && shield->obj_flags.type_flag != ITEM_SHIELD) || !shield)
  {
    send_to_char ("You need to be holding a shield to do that.\n", ch);  
    return;
  }

  victim = ch->fighting;
  defense = victim->skills[SKILL_SHIELD_STYLE] / 4; 

  roll = number(1,100);
  result = ch->skills[SKILL_SHIELD_STYLE] - roll;
  result -= defense;

  ch->balance += -30;
  ch->balance = MAX (ch->balance, -30);

  if (get_equip (ch, WEAR_PRIM) == shield || get_equip (ch, WEAR_BOTH) == shield)
    ch->primary_delay += 30;
  else if (get_equip (ch, WEAR_SEC) == shield)
    ch->secondary_delay += 30;

  wear_loc = body_tab[0][3].wear_loc1;
  if (wear_loc)
  {
    armor = get_equip (victim, wear_loc);
    if (armor && GET_ITEM_TYPE (armor) != ITEM_ARMOR)
	armor = NULL;
  }
  sprintf (strike_location, "%s", figure_location (victim, 3));

  if (result < -40)
  {
    sprintf(buf, "$n's attempts to thrust %s into your face, but instead $e stumbles and falls to the ground.", obj_short_desc(shield));
    sprintf(buf2, "You attempt to thrust %s at $N's face, but instead stumble and fall to the ground.", obj_short_desc(shield));
    sprintf(buf3, "$n attempts to thrust %s at $N's face, but instead $e stumble and fall to the ground.", obj_short_desc(shield));
    GET_POS (ch) = SIT;
    add_second_affect (SA_STAND, ((25-GET_AGI(ch))+number(1,3)), ch, NULL, NULL, 0);
  }
  else if (result < 0)
  {
    sprintf(buf, "$n thrusts #2%s#0 at your face, but misses.", obj_short_desc(shield));
    sprintf(buf2, "You thrust #2%s#0 at $N's face, but miss.", obj_short_desc(shield));
    sprintf(buf3, "$n misses $N with a thrust from #2%s#0.", obj_short_desc(shield));
  }
  else if (result < 25)
  {
    damage = dice(2,4);
    if (armor)
    {
      damage -= armor->o.armor.armor_value;
      damage += weapon_armor_table[3][armor->o.armor.armor_type];
    }
    wound_to_char (victim, strike_location, damage, 3, 0, 0, 0);
    sprintf(buf, "$n catches you across the %s with #2%s#0.", 
      strike_location, obj_short_desc(shield));
    sprintf(buf2, "You catch $N across the %s with #2%s#0.", 
      strike_location, obj_short_desc(shield));
    sprintf(buf3, "$n catches $N across the %s with #2%s#0.", 
      strike_location, obj_short_desc(shield));
  }
  else if (result < 45)
  {
    damage = dice(2,5);
    if (armor)
    {
      damage -= armor->o.armor.armor_value;
      damage += weapon_armor_table[3][armor->o.armor.armor_type];
    }
    wound_to_char (victim, strike_location, damage, 3, 0, 0, 0);
    sprintf(buf, "$n thrusts #2%s#0 into your %s.", obj_short_desc(shield), strike_location);
    sprintf(buf2, "You thrust #2%s#0 into $N's %s.", obj_short_desc(shield), strike_location);
    sprintf(buf3, "$n thrusts #2%s#0 into $N's %s.", obj_short_desc(shield), strike_location);
  }
  else if (result < 65)
  {
    damage = dice(2,6);
    damage += 1;
    if (armor)
    {
      damage -= armor->o.armor.armor_value;
      damage += weapon_armor_table[3][armor->o.armor.armor_type];
    }
    victim->balance += -10;
    victim->balance = MAX (ch->balance, -25);
    wound_to_char (victim, strike_location, damage, 3, 0, 0, 0);
    sprintf(buf, "$n %s bashes #2%s#0 into your %s, knocking you off balance.", 
      crithits[number(0,7)], obj_short_desc(shield), strike_location);
    sprintf(buf2, "You %s bash #2%s#0 into $N's %s.", 
      crithits[number(0,7)], obj_short_desc(shield), strike_location);
    sprintf(buf3, "$n %s bashes #2%s#0 into $N's %s.", 
      crithits[number(0,7)], obj_short_desc(shield), strike_location); 
    soma_add_affect(victim, SOMA_NERVES_HEADACHE, 2, 0, 0, 800, 400, 800, 2, 4, 6, 8);
    soma_add_affect(victim, SOMA_BLUNT_MEDHEAD, 2, 0, 0, 800, 400, 800, 1, 1, 1, 2);
  }
  else
  {
    damage = dice(2,7);
    damage += 1;
    if (armor)
    {
      damage -= armor->o.armor.armor_value;
      damage += weapon_armor_table[3][armor->o.armor.armor_type];
    }
    victim->balance += -10;
    victim->balance = MAX (ch->balance, -25);
    wound_to_char (victim, strike_location, damage, 3, 0, 0, 0);
    sprintf(buf, "$n %s slams #2%s#0 into your %s, knocking you to the ground.", 
      crithits[number(0,7)], obj_short_desc(shield), strike_location);
    sprintf(buf2, "You %s slam #2%s#0 into $N's %s, knocking $M to the ground.", 
      crithits[number(0,7)], obj_short_desc(shield), strike_location);
    sprintf(buf3, "$n %s slams #2%s#0 into $N's %s, knocking $M to the ground", 
      crithits[number(0,7)], obj_short_desc(shield), strike_location); 
    soma_add_affect(victim, SOMA_NERVES_HEADACHE, 2, 0, 0, 800, 400, 800, 2, 4, 6, 8);
    soma_add_affect(victim, SOMA_BLUNT_MEDHEAD, 2, 0, 0, 800, 400, 800, 1, 1, 1, 2);
    GET_POS (victim) = SIT;
    add_second_affect (SA_STAND, ((25-GET_AGI(victim))+number(2,6)), victim, NULL, NULL, 0);
  }

  act(buf, false, ch, 0, victim, TO_VICT | _ACT_FORMAT | _ACT_COMBAT);
  act(buf2, false, ch, 0, victim, TO_CHAR | _ACT_FORMAT | _ACT_COMBAT);
  act(buf3, false, ch, 0, victim, TO_NOTVICT | _ACT_FORMAT | _ACT_COMBAT);

}


void
hit_char (CHAR_DATA * ch, CHAR_DATA * victim, int strike_parm)
{
  int hide = 0;
  int sneak = 0;
  int killed = 0;
  bool feint = false;
  OBJ_DATA *weapon;

  if (!ch || !victim)
    return;

  if (GET_FLAG (ch, FLAG_FLEE))
    {
      if (!ch->primary_delay && !flee_attempt(ch))
	ch->primary_delay=16;

      return;
    }


  if (IS_SET (victim->act, ACT_FLYING)
      && !IS_SET (ch->act, ACT_FLYING)
      && AWAKE (victim) && victim->race != 1
      && victim->fighting != ch)
    {
      send_to_char ("They are flying out of reach!\n", ch);
      stop_fighting(ch);
      return;
    }

  // If you attack someone from hiding, you get ambush bonuses.
  // If you pass your sneak check, you get 3 strikes that double your weapon skill.
  // If you fail the sneak check, you get 1 strike that gives a +50% boost.

  if (AWAKE (victim) && (get_affect (ch, MAGIC_HIDDEN)))
  {

    if(skill_use (ch, SKILL_SNEAK, 0))
      sneak = 1;
    if(skill_use (ch, SKILL_HIDE, 0))
      hide = 1;

    remove_affect_type (ch, MAGIC_HIDDEN);
    act ("$n ambushes $N.", false, ch, 0, victim, TO_NOTVICT);

    if (sneak && hide)
    {
      act ("You ambush takes $N by utter surprise!", false, ch, 0, victim, TO_CHAR);
      act ("You are startled by $n's sudden attack!", false, ch, 0, victim, TO_VICT);       
      if(!(get_second_affect (ch, SA_AMBUSH, NULL)))
        add_second_affect (SA_AMBUSH, 20, ch, NULL, NULL, 250);
    }
    else if (sneak || hide)
    {
      act ("You ambush takes $N by surprise!", false, ch, 0, victim, TO_CHAR);
      act ("$n ambushes you!", false, ch, 0, victim, TO_VICT);       
      if(!(get_second_affect (ch, SA_AMBUSH, NULL)))
        add_second_affect (SA_AMBUSH, 20, ch, NULL, NULL, 150);
    }
    else
    {
      act ("$N spots your ambush!", false, ch, 0, victim, TO_CHAR);
      act ("You spot $n's ambush a moment before the blow falls!", false, ch, 0, victim, TO_VICT); 
      if(!(get_second_affect (ch, SA_AMBUSH, NULL)))
        add_second_affect (SA_AMBUSH, 1, ch, NULL, NULL, 50);
    }
  }

  if (IS_NPC (ch) && !ch->desc)
    {
      ready_melee_weapons (ch);
    }

  if (IS_NPC (victim) && !victim->desc)
    {
      ready_melee_weapons (victim);
    }

  if (victim->delay && victim->delay_type != DEL_LOAD_WEAPON)
    break_delay (victim);

  if (ch->delay && ch->delay_type != DEL_LOAD_WEAPON)
    break_delay (ch);

  if (!ch->fighting && !strike_parm)
    set_fighting (ch, victim);

  if (IS_SUBDUEE (victim))
    {
      act ("$n takes a swing at $N.", false, ch, 0, victim, TO_NOTVICT);
      act ("$N takes a swing at you.", false, victim, 0, ch, TO_CHAR);
    }

  if (IS_SUBDUEE (ch))
    {
      if (ch->fighting)
	stop_fighting (ch);
      return;
    }

  /*
    if (GET_FLAG (ch, FLAG_FLEE))
    {

      if (!ch->primary_delay && !flee_attempt (ch))
  	ch->primary_delay = 16;

   return;
    }
  */

  guard_check (victim);

  if (IS_SUBDUER (ch))
    return;

  if (get_affect (victim, AFFECT_GUARD_DIR))
    {
      act ("The attack prevents you from continuing to guard the exit.",
	   false, victim, 0, 0, TO_CHAR);
      act ("The attack prevents $n from continuing to guard the exit.", false,
	   victim, 0, 0, TO_ROOM | _ACT_FORMAT);
      affect_remove (victim, get_affect (victim, AFFECT_GUARD_DIR));
    }

  if (get_affect (victim, AFFECT_WATCH_DIR))
    {
      act ("The attack prevents you from continuing to watch the exit.",
	   false, victim, 0, 0, TO_CHAR);
      affect_remove (victim, get_affect (victim, AFFECT_WATCH_DIR));
    }

  /* Empty handed attack / Primary / Dual */

  if (!ch->primary_delay &&
      (get_equip (ch, WEAR_PRIM) ||
       get_equip (ch, WEAR_BOTH) ||
       (!get_equip (ch, WEAR_PRIM) &&
	!get_equip (ch, WEAR_BOTH) && !get_equip (ch, WEAR_SEC))))
    {

      if (GET_FLAG (ch, FLAG_SUBDUING))
	{

	  if (!(weapon = get_equip (ch, WEAR_PRIM)))
	    return;

	  if (weapon->o.weapon.use_skill != SKILL_SMALL_BLADE)
	    return;
	}

      if (get_affect (ch, MAGIC_AFFECT_DIZZINESS) && !number (0, 3))
	{
	  send_to_char ("You battle your dizziness as you ready your attack.",
			ch);
	  act ("$N staggers, appearing dizzy.", true, ch, 0, 0, TO_ROOM);
	  ch->primary_delay += 5 * 4;
	}
      else if (get_second_affect (ch, SA_FEINT, NULL))
      { 
        feint = true;
      }
      else
	strike (ch, victim, 1, 0);
    }

  if (ch->deleted || victim->deleted || !victim->room || !ch->room)
    killed = 1;

  if (!killed && !can_move (victim) && !GET_FLAG (ch, FLAG_KILL) &&
      ch->fighting)
    {
      //act ("$N can't move.", false, ch, 0, victim, TO_CHAR);

      /* Aggressives don't stop trying to kill */

      if (!ch->mob || !IS_SET (ch->act, ACT_AGGRESSIVE))
	stop_fighting (ch);

      return;
    }

  /* Secondary weapon attack */

  if (!killed && !ch->secondary_delay && get_equip (ch, WEAR_SEC))
    {

      if (GET_FLAG (ch, FLAG_SUBDUING) &&
	  !((weapon = get_equip (ch, WEAR_SEC)) &&
	    weapon->o.weapon.use_skill != SKILL_SMALL_BLADE))
	return;

      if (get_second_affect (ch, SA_FEINT, NULL) && !feint)
        return;
      else if (get_second_affect (ch, SA_FEINT, NULL) && feint)
      {
        // Make the second attack first.
        strike (ch, victim, 2, 0);
        strike (ch, victim, 1, 0);  
      }
      else  
        strike (ch, victim, 2, 0);
    }

  if (!ch->deleted && !victim->deleted && ch->fighting &&
      ch->room && victim->room && IS_SUBDUEE (victim))
    stop_fighting (ch);

  if (!ch->deleted && !victim->deleted && ch->fighting && !killed &&
      !can_move (victim) && !GET_FLAG (ch, FLAG_KILL))
    {

      //act ("$N can't move.", false, ch, 0, victim, TO_CHAR);

      /* Aggressives don't stop trying to kill */

      if (!ch->mob || !IS_SET (ch->act, ACT_AGGRESSIVE))
	stop_fighting (ch);
    }

  if (!ch->deleted && !ch->fighting && GET_POS (ch) == FIGHT)
  	{
    GET_POS (ch) = STAND;
    remove_cover(ch,0);
    }
}

void
poison_bite (CHAR_DATA * src, CHAR_DATA * tar)
{
/*  POISON_DATA *poison;
  int duration;

  for (poison = src->venom; poison; poison = poison->next)
    {
      if (!poison->poison_type)
	continue;
      if (GET_CON (tar) > number (0, 25))
	continue;
      duration = number (poison->duration_die_1, poison->duration_die_2);
      if (poison->poison_type == POISON_LETHARGY)
	act
	  ("Your head begins swimming as you suddenly feel overwhelmingly lethargic.",
	   false, tar, 0, 0, TO_CHAR | _ACT_FORMAT);
      else
	act ("You grimace as you feel a foreign substance enter your veins.",
	     false, tar, 0, 0, TO_CHAR | _ACT_FORMAT);
      magic_add_affect (tar, poison->poison_type, duration, 0, 0, 0, 0);
    }*/
}

char *
get_dam_word (int damage, CHAR_DATA * tar)
{
  
  float limit = 0;

  limit = tar->max_hit;


  if (damage <= (limit * .02))
    return " glancingly ";
  else if (damage > (limit * .02) && damage <= (limit * .10))
    return " ";
  else if (damage > (limit * .10) && damage <= (limit * .20))
    return " hard ";
  else if (damage > (limit * .20) && damage <= (limit * .30))
    return " very hard ";
  else if (damage > (limit * .30) && damage <= (limit * .40))
    return " extremely hard ";
  else if (damage > (limit * .40) && damage <= (limit * .50))
    return " incredibly hard ";
  else
    return " astonishingly hard ";
}

int
combat_roll (int ability, int *diff, int feint)
{
  int r;
  int roll_result;

  r = number (1, SKILL_CEILING);

  if (feint == 1)
  {
    r = MIN(r, (number (1, SKILL_CEILING)));
    r = MIN(r, (number (1, SKILL_CEILING)));
  }

  if (ability > 100)
    ability = 100;

  if (ability < 5)
    ability = 5;

  if (r > ability)
    roll_result = SUC_MF - ((r % 5) ? 0 : 1);
  else
    roll_result = SUC_MS + ((r % 5) ? 0 : 1);

  if (feint == 2)
    roll_result = SUC_MF;

  if(diff)
    *diff = ability - r; // How much you pass by

  return roll_result;
}

#define AD (fd + strlen (fd))

void
advance (CHAR_DATA * src, CHAR_DATA * tar)
{
  if (src->distance_to_target >= 2)
    {
      act ("You begin advancing on $N, preparing for battle.", false, src, 0,
	   tar, TO_CHAR);
      act ("$n begins advancing on $N, preparing for battle.", false, src, 0,
	   tar, TO_NOTVICT);
      act ("$n begins advancing on you, preparing for battle.", false, src, 0,
	   tar, TO_VICT);
    }
  else if (src->distance_to_target == 1)
    {
      act ("You close to polearm range on $N!", false, src, 0, tar, TO_CHAR);
      act ("$n closes to polearm range on $N!", false, src, 0, tar,
	   TO_NOTVICT);
      act ("$n closes to polearm range on you!", false, src, 0, tar, TO_VICT);
    }
  else if (src->distance_to_target == 0)
    {
      act ("You close to melee range on $N!", false, src, 0, tar, TO_CHAR);
      act ("$n closes to melee range on $N!", false, src, 0, tar, TO_NOTVICT);
      act ("$n closes to melee range on you!", false, src, 0, tar, TO_VICT);
    }
  src->primary_delay = 20;
  src->distance_to_target -= 1;
  tar->distance_to_target -= 1;

}


int
strike (CHAR_DATA * src, CHAR_DATA * tar, int attack_num, int mode)
{
  float defense = 0;
  float attack = 0;
  int off_success;
  int def_success;
  int off_result = 0;
  int def_result = 0;
  int off_style = 0;
  int def_style = 0;
  int off_wep_style_mod = 0;
  int def_wep_style_mod = 0;
  int defense_hand;
  int location;
  int damage = 0, hit_type = 0;
  int i;
  int j, wear_loc1 = 0, wear_loc2 = 0;
  int strchk;
  int movecost;
  char loc[MAX_STRING_LENGTH];
  OBJ_DATA *tar_prim = get_equip (tar, WEAR_PRIM);
  OBJ_DATA *tar_sec = get_equip (tar, WEAR_SEC);
  OBJ_DATA *tar_both = get_equip (tar, WEAR_BOTH);
  OBJ_DATA *src_prim = get_equip (src, WEAR_PRIM);
  OBJ_DATA *src_sec = get_equip (src, WEAR_SEC);
  OBJ_DATA *src_both = get_equip (src, WEAR_BOTH);
  OBJ_DATA *attack_weapon = NULL;
  OBJ_DATA *defense_weapon = NULL;
  OBJ_DATA *shield = NULL;
  OBJ_DATA *attack_shield = NULL;
  OBJ_DATA *eq1 = NULL, *eq2 = NULL, *broken_eq = NULL;
  CHAR_DATA *mount;
  AFFECTED_TYPE *af = NULL;
  SECOND_AFFECT *saf = NULL;
  int bonus;
  float attack_modifier;
  float defense_modifier;
  float r1;
  float fatchk;
  int att_diff = 0, def_diff = 0; // how much you passed/failed by.
  
  char fd[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  CHAR_DATA *dch;

  *fd = 0;

  if (IS_SET (src->act, ACT_VEHICLE))
    return 0;

  sprintf (AD, "%s [%d hp %d mvs] strike %d %s [%d hp %d mvs]  ",
	   GET_NAME (src), GET_HIT (src), GET_MOVE (src), attack_num,
	   GET_NAME (tar), GET_HIT (tar), GET_MOVE (tar));

  attack_modifier = 100.0;
  defense_modifier = 100.0;

  if (src->in_room != tar->in_room)
    return 0;

  if (attack_num == 1)
    attack_weapon = src_prim ? src_prim : src_both;
  else
    attack_weapon = src_sec;

  if (attack_weapon &&
      (attack_weapon->o.weapon.use_skill == SKILL_SHORTBOW ||
       attack_weapon->o.weapon.use_skill == SKILL_LONGBOW ||
       attack_weapon->o.weapon.use_skill == SKILL_SLING ||
       attack_weapon->o.weapon.use_skill == SKILL_THROWN))
    return 0;

  if (attack_weapon && GET_ITEM_TYPE (attack_weapon) != ITEM_WEAPON &&
      GET_ITEM_TYPE (attack_weapon) != ITEM_SHIELD)
    attack_weapon = NULL;

  if (IS_SET (src->flags, FLAG_PACIFIST))
    return 0;

  if (attack_weapon)
    sprintf (AD, "%s\n\r", attack_weapon->short_description);
  else
    {
      attack = src->skills[SKILL_BRAWLING];	/* default attack */
      sprintf (AD, "BRAWLING\n\r");
    }

  if (attack_weapon)
  {
    attack = src->skills[attack_weapon->o.weapon.use_skill];
  }

  if (attack < src->offense)
    {
      attack = src->offense;
      sprintf (AD, "Using Offense %d ", (int) attack);
    }

  sprintf (AD, "ABase %d ", (int) attack);

  /* Weapon bonus/penalty */

  bonus = 100;

  if (attack_weapon)
    {
      for (af = attack_weapon->xaffected; af; af = af->next)
	switch (af->a.spell.location - 10000)
	  {
	  case SKILL_OFFENSE:
	  case SKILL_CLUB:
	  case SKILL_SWORD:
	  case SKILL_SMALL_BLADE:
	  case SKILL_AXE:
	  case SKILL_FLAIL:
	  case SKILL_POLEARM:
            if(attack_weapon->o.weapon.use_skill == af->a.spell.location - 10000)
            {
	      sprintf (AD, "+%d WEAP-AFF ", af->a.spell.modifier);
	      attack += af->a.spell.modifier;
            }
	    break;
	  default:
	    break;
	  }
    }

  if ((attack_shield = get_equip (src, WEAR_SHIELD)))
    {

      for (af = attack_shield->xaffected; af; af = af->next)
	{
	  if (af->a.spell.location - 10000 == SKILL_OFFENSE)
	    {
	      sprintf (AD, "%d OFF/SHIELD PEN ", af->a.spell.modifier);
	      bonus += af->a.spell.modifier;
	    }
	}
    }

  if (!attack_weapon)
    ;
  else if (attack_shield)
    off_style = SKILL_SHIELD_STYLE;
  else if (attack_weapon->obj_flags.type_flag == ITEM_WEAPON)
    {
      if (attack_weapon->o.weapon.use_skill == SKILL_LONGBOW ||
	  attack_weapon->o.weapon.use_skill == SKILL_SHORTBOW ||
	  attack_weapon->o.weapon.use_skill == SKILL_SLING ||
	  attack_weapon->o.weapon.use_skill == SKILL_THROWN)
        ;
      else
      {
        if (!(src_prim && src_sec))
          off_style = SKILL_SINGLE_WEAPON;
        else if ((src_prim->obj_flags.type_flag == ITEM_WEAPON) && (src_sec->obj_flags.type_flag == ITEM_WEAPON))
          off_style = SKILL_DUAL;

        if (src_both)
          off_style = SKILL_LARGE_WEAPON;
      }
    }

  if (!src->skills[off_style])
    skill_learn (src, off_style);

  // If they've got the ambush flag, reduce it by one for every strike that's
  // made, and if they're attacking the person they ambushed, add the bonus.

  saf = get_second_affect(src, SA_AMBUSH, NULL);
 
  if(saf) 
  {
    if(saf->seconds > 0)
    {
      bonus += saf->info2;
      saf->seconds -= 1;
    }
    else
    {
      remove_second_affect(saf);
    }
  }

  if (bonus < 0)
    attack_modifier = 0.0;
  else
    attack_modifier = attack_modifier * bonus / 100.0;

  sprintf (AD, "weapmod %d ", bonus);

/* Encumberance penalty */

  for (i = 0; i < ENCUMBERANCE_ENTRIES; i++)
    {
    	if (GET_STR (src) >= 20)
    		strchk = 20;
    	else
    		strchk = GET_STR (src);
    	
      if (strchk * enc_tab[i].str_mult_wt >= IS_CARRYING_W (src))
				break;
    }

  attack_modifier = attack_modifier * enc_tab[i].penalty;
  sprintf (AD, "Enc %3.2f ", enc_tab[i].penalty);
	/* Move costs */    
    /** 
    move_cost = enc_tab[i].move + 2.0 for Frantic
    move_cost = enc_tab[i].move + 1.5 for Aggressive
    move_cost = enc_tab[i].move + 1.0 for Normal
    move_cost = enc_tab[i].move + 0.5 for Careful
    move_cost = enc_tab[i].move + 0.0 for Defensive
    **/
    
  movecost = int (enc_tab[i].move + (0.5) * (4 - src->fight_mode));
		

		 
/* 50% chance to lose a move if they would have lost no points */  
  if ((number (1, 100) > 50) && (movecost <= 1))
  	movecost = 1;
  	
	src->move = src->move - movecost;
  if (src->move < 0)
  	src->move = 0;
			
  /* Fatigue penalty */

  if (GET_MAX_MOVE (src) > 0)
    j = GET_MOVE (src) * 100 / GET_MAX_MOVE (src);
  else
    j = 0;

  if (j > 100)
    j = 100;

  for (i = 0; j > fatigue[i].percent; i++)
    ;

	// Save vs WILLPOWER
  if (number (1, 25) > src->wil)
  	fatchk = fatigue[i].penalty;
  else
  	fatchk = 1.00;

	
	attack_modifier = attack_modifier * fatchk;
	sprintf (AD, "\nAtk Fatigue %3.2f real Fatigue  %3.2f", fatchk, fatigue[i].penalty);

  /* Dual wield penalty */

  if (attack_num == 2)
    {
      r1 = .60 + .40 * src->skills[SKILL_DUAL] / 100.0;
      sprintf (AD, "Dual Pen %3.2f ", r1);
      attack_modifier = attack_modifier * r1;
    }

  /* Fightmode modifier */

  attack_modifier = attack_modifier *
    fight_tab[src->fight_mode].offense_modifier;

  if (get_affect (src, MAGIC_AFFECT_FURY))
    {
      attack_modifier = attack_modifier * 1.25;
      sprintf (AD, "* 1.25 [FURY] ");
    }

  if(IS_SET(src->affected_by, AFF_TRANSPORTING))
  {
    attack_modifier = attack_modifier * 0.25;
    sprintf (AD, "* 0.25 [TRANSPORTING] ");
  }

  if(IS_SET(tar->affected_by, AFF_TRANSPORTING))
  {
    defense_modifier = attack_modifier * 0.25;
    sprintf (AD, "* 0.25 [TRANSPORTING] ");
  }
 

  if (get_affect (src, MAGIC_AFFECT_DIZZINESS))
    {
      attack_modifier = attack_modifier * 0.75;
      sprintf (AD, "* 0.75 [DIZZINESS] ");
    }

  sprintf (AD, "FM %3.2f ", fight_tab[src->fight_mode].offense_modifier);
  sprintf (AD, " = OFFENSE %3.0f\n", attack * attack_modifier / 100);

  /* DEFENSE */

  /* We need to know which weapon to defend with */

  shield = get_equip (tar, WEAR_SHIELD);

  if (shield && shield->obj_flags.type_flag != ITEM_SHIELD)
    {
      printf ("Non-shield object %d, on %d(%s), at %d\n",
	      shield->nVirtual, IS_NPC (tar) ? tar->mob->nVirtual : 0,
	      tar->name, tar->room->nVirtual);
      fflush (stdout);
    }

  defense_weapon = NULL;

  if (tar_both)
    {
      defense_weapon = tar_both;
      defense_hand = 1;
      def_style = SKILL_LARGE_WEAPON;
    }

  else if (tar_prim && !tar_sec && !shield)
    {
      defense_weapon = tar_prim;
      defense_hand = 1;
    }

  else if (tar_sec && !tar_prim && !shield)
    {
      defense_weapon = tar_sec;
      defense_hand = 2;
    }

  else if (shield && !tar_prim && !tar_sec)
    {
      defense_weapon = shield;
      defense_hand = 1;
    }

  else if (shield && tar_prim)
    {
      if (tar->primary_delay > tar->secondary_delay)
	{
	  defense_weapon = shield;
	  defense_hand = 2;
	}
      else
	{
	  defense_weapon = tar_prim;
	  defense_hand = 1;
	}
    }

  else if (shield && tar_sec)
    {
      if (tar->primary_delay > tar->secondary_delay)
	{
	  defense_weapon = tar_sec;
	  defense_hand = 2;
	}
      else
	{
	  defense_weapon = shield;
	  defense_hand = 1;
	}
    }

  else if (tar_prim && tar_sec)
    {
      if (tar->primary_delay > tar->secondary_delay)
	{
	  defense_weapon = tar_sec;
	  defense_hand = 2;
	}
      else
	{
	  defense_weapon = tar_prim;
	  defense_hand = 1;
	}
    }

  else
    {
      defense_weapon = NULL;
      defense_hand = 1;
    }

  if (shield != defense_weapon)
    shield = NULL;

  if (defense_weapon && GET_ITEM_TYPE (defense_weapon) != ITEM_WEAPON
      && GET_ITEM_TYPE (defense_weapon) != ITEM_SHIELD)
    defense_weapon = NULL;

  if (!defense_weapon)
    defense = tar->skills[SKILL_DODGE];
  else if (defense_weapon->obj_flags.type_flag == ITEM_SHIELD)
  {
    defense = tar->skills[SKILL_AVERT];
    def_style = SKILL_SHIELD_STYLE;
  }
  else if (defense_weapon->obj_flags.type_flag == ITEM_WEAPON)
    {
      if (defense_weapon->o.weapon.use_skill == SKILL_LONGBOW ||
	  defense_weapon->o.weapon.use_skill == SKILL_SHORTBOW ||
	  defense_weapon->o.weapon.use_skill == SKILL_SLING ||
	  defense_weapon->o.weapon.use_skill == SKILL_THROWN)
	{
	  defense = tar->skills[SKILL_DODGE];
	}
      else
      {
	defense = tar->skills[SKILL_AVERT];
        if (!(tar_prim && tar_sec))
          def_style = SKILL_SINGLE_WEAPON;
        else if ((tar_prim->obj_flags.type_flag == ITEM_WEAPON) && (tar_sec->obj_flags.type_flag == ITEM_WEAPON))
          def_style = SKILL_DUAL;

        if (tar_both)
          def_style = SKILL_LARGE_WEAPON;
      }
    }
  else
    {
      defense = 0;
    }

  if (real_skill (tar, SKILL_DANGER_SENSE))
    if (skill_use (tar, SKILL_DANGER_SENSE, 0))
      defense += tar->skills[SKILL_DANGER_SENSE] / 5;
  /* On a successful use of the Danger Sense skill, if */
  /* the skill being checked is a defensive combat skill, */
  /* it grants them a bonus; they are able to sense the */
  /* impending blow before it lands. (Nexus) */

  if (IS_SET (tar->flags, FLAG_PACIFIST))
    defense += 10;

  if (defense > 99)
    defense = 99;

  /*  Unless you're Legolas, using bows in melee isn't so bright... */
  if (tar->aiming_at)
    defense -= 20;

  sprintf (AD, "DBase %d ", (int) defense);


 /* Encumberance penalty */

  for (i = 0; i < ENCUMBERANCE_ENTRIES; i++)
    {
      if (GET_STR (tar) >= 18)
    		strchk = 18;
    	else
    		strchk = GET_STR (tar);
    	
      if (strchk * enc_tab[i].str_mult_wt >= IS_CARRYING_W (tar))
				break;
    }

  sprintf (AD, "Enc %3.2f ", enc_tab[i].penalty);
  defense_modifier = defense_modifier * enc_tab[i].penalty;
  
  /* Move costs */    
    /** 
    move_cost = enc_tab[i].move + 2.0 for Frantic
    move_cost = enc_tab[i].move + 1.5 for Aggressive
    move_cost = enc_tab[i].move + 1.0 for Normal
    move_cost = enc_tab[i].move + 0.5 for Careful
    move_cost = enc_tab[i].move + 0.0 for Defensive
    **/

  movecost = int (enc_tab[i].move + (0.5) * (4 - tar->fight_mode));
   /* 50% chance to lose a move if they would have lost no points */  
  if ((number (1, 100) > 50) && (movecost <= 1))
  	movecost = 1;
  	
	tar->move = tar->move - movecost;
  if (tar->move < 0)
  	tar->move = 0;
  /* Fatigue penalty */

  if (GET_MAX_MOVE (tar) > 0)
    j = GET_MOVE (tar) * 100 / GET_MAX_MOVE (tar);
  
  if (j > 100)
    j = 100;

  for (i = 0; j > fatigue[i].percent; i++)
    ;

	
  // Save vs WILLPOWER
  if (number (1, 25) > tar->wil)
  	fatchk = fatigue[i].penalty;
  else
  	fatchk = 1.00;
	
	defense_modifier = defense_modifier * fatchk;
	sprintf (AD, "\nDef Fatigue %3.2f FatiguePen %3.2f", fatchk, fatigue[i].penalty);

  /* Fightmode modifier */

  defense_modifier = defense_modifier *
    fight_tab[tar->fight_mode].defense_modifier;

  sprintf (AD, "FM %3.2f ", fight_tab[tar->fight_mode].defense_modifier);

  /* Hand delay modifier */

  if (defense_hand == 1)
    r1 = (100 - tar->primary_delay) / 100.0;
  else
    r1 = (100 - tar->secondary_delay) / 100.0;

  if (r1 < .50)			/* Maximum 50% penalty for being delayed. */
    r1 = .50;

  defense_modifier = defense_modifier * r1;

  sprintf (AD, "DelayPen %3.2f ", r1);

  /* Weapon/shield defense */

  if (defense_weapon)
    {

      bonus = 100;

      for (af = defense_weapon->xaffected; af; af = af->next)
	{

	  if (defense_weapon->obj_flags.type_flag == ITEM_SHIELD)
	    {
	      if (af->a.spell.location - 10000 == SKILL_AVERT)
		defense += af->a.spell.modifier;

              // Work around for old shields.

	      if (af->a.spell.location - 10000 == SKILL_DEFUNCT)
		defense += af->a.spell.modifier;

	    }

	  else if (af->a.spell.location - 10000 == SKILL_AVERT)
	    defense += af->a.spell.modifier;
	}
    }


/* If the attacker is using a weapon, compare the defender's skill in
   that weapon with the attacker's skill. The difference is multipled to
   the defense modifier, to a maximum of 10%.
   It's a range of 20%, but will give people incentive to train up other
   skills.
   Defaults to offense. */

  if(attack_weapon)
  {

    if(src->skills[attack_weapon->o.weapon.use_skill] > src->offense)
      off_wep_style_mod = src->skills[attack_weapon->o.weapon.use_skill];
    else
      off_wep_style_mod = src->offense;

    if(tar->skills[attack_weapon->o.weapon.use_skill] > tar->offense)
      def_wep_style_mod = tar->skills[attack_weapon->o.weapon.use_skill];
    else
      def_wep_style_mod = tar->offense;

    // Positive if defender's mod is higher than attacker's mod.

    r1 = (def_wep_style_mod - off_wep_style_mod) / 4;

    r1 = (100 + r1) / 100;

    if (r1 < .90)
      r1 = .90;
    else if (r1 > 1.10)
      r1 = 1.10;
  
    defense_modifier = defense_modifier * r1;
  }


/* Like the differing weapons, compare the styles of the attackers,
   and then modifier the offensive modifier by the difference of the
   two divided by 2. If there is neither an off style and a def style,
   e.g. because someone is brawling, then this isn't applied. */

  if(off_style || def_style)
  {
    if(off_style)
      off_wep_style_mod = src->skills[off_style];
    else
      off_wep_style_mod = src->offense;

    if(def_style)
      def_wep_style_mod = tar->skills[def_style];
    else
      def_wep_style_mod = tar->offense;

    // Positive if attacker's mod is higher than defender's mod.

    r1 = (off_wep_style_mod - def_wep_style_mod) / 2;


    r1 = (100 + r1) / 100;

    if (r1 < .80)
      r1 = .80;
    else if (r1 > 1.20)
      r1 = 1.20;
  
    attack_modifier = attack_modifier * r1;
  }

  // If you are suffering from a stun, and fail your willpower test, lose 5 points.

  if(get_soma_affect(src, SOMA_BLUNT_MEDHEAD) && number(0,25) > GET_WIL(src))
    attack -= 5;
  if(get_soma_affect(tar, SOMA_BLUNT_MEDHEAD) && number(0,25) > GET_WIL(tar))
    defense -= 5;

  if(get_soma_affect(src, SOMA_PLANT_FURY))
    attack += 5;
  if(get_soma_affect(tar, SOMA_PLANT_FURY))
    defense += 5;

  if(get_soma_affect(src, SOMA_PLANT_RELAX))
    attack -= 5;
  if(get_soma_affect(tar, SOMA_PLANT_RELAX))
    defense -= 5;

  if(get_soma_affect(src, SOMA_PLANT_BLINDNESS))
    attack -= 5;
  if(get_soma_affect(tar, SOMA_PLANT_BLINDNESS))
    defense -= 5;

  defense = defense * defense_modifier / 100;
  attack = attack * attack_modifier / 100;

  if (attack_weapon
      && (attack_weapon->o.od.value[0] == 1))
    {
      if (attack_weapon->location == WEAR_SEC)
	attack -= 10;
    }

  if (attack_weapon
      && (attack_weapon->o.od.value[0] == 3))
    {
      if (attack_weapon->location == WEAR_PRIM)
	attack -= 5;
      if (attack_weapon->location == WEAR_SEC)
	attack -= 10;
    }

  if (defense_weapon
      && (defense_weapon->o.od.value[0] == 3))
    {
      if (defense_weapon->location == WEAR_PRIM)
	defense -= 5;
      if (defense_weapon->location == WEAR_SEC)
	defense -= 10;
    }

  if ((src->race == 11 || src->race == 13) && sun_light
      && src->room->sector_type != SECT_INSIDE)
    {
      if (weather_info[src->room->zone].clouds == CLEAR_SKY)
	attack -= 20;
      else if (weather_info[src->room->zone].clouds == LIGHT_CLOUDS)
	attack -= 15;
      else if (weather_info[src->room->zone].clouds == HEAVY_CLOUDS)
	attack -= 10;
      else if (weather_info[src->room->zone].clouds == OVERCAST)
	attack -= 5;
    }

  if ((tar->race == 11 || tar->race == 13) && sun_light
      && tar->room->sector_type != SECT_INSIDE)
    {
      if (weather_info[tar->room->zone].clouds == CLEAR_SKY)
	defense -= 20;
      else if (weather_info[tar->room->zone].clouds == LIGHT_CLOUDS)
	defense -= 15;
      else if (weather_info[tar->room->zone].clouds == HEAVY_CLOUDS)
	defense -= 10;
      else if (weather_info[tar->room->zone].clouds == OVERCAST)
	defense -= 5;
    }

  // If they're in a big enough group and in formation, give'em their 5 bonus.

  if(do_group_size(tar) > 9 && tar->formation > 0)
  {
    defense += 5;
  }

  sprintf (AD, " = DEFENSE %d\n", (int) defense);

  attack = figure_wound_skillcheck_penalties (src, (int) attack);
  defense = figure_wound_skillcheck_penalties (tar, (int) defense);

  defense = MAX (5.0f, defense);
  attack = MAX (5.0f, attack);

  if (get_second_affect(src, SA_FEINT, NULL))
    off_success = combat_roll ((int) attack, &att_diff, attack_num);
  else
    off_success = combat_roll ((int) attack, &att_diff, 0);

  def_success = combat_roll ((int) defense, &def_diff, 0);

  sprintf (buf, "End Result: %f Attack %f Defense\n", attack, defense);
/*	send_to_char (buf, src); */

  if (attack_weapon)
    hit_type = attack_weapon->o.weapon.hit_type;
  else
    {
      if (src->nat_attack_type == 0 || src->nat_attack_type == 4)
	hit_type = 9;
      else if (src->nat_attack_type == 1 || src->nat_attack_type == 3 || src->nat_attack_type == 5)
	hit_type = 7;
      else if (src->nat_attack_type == 2)
	hit_type = 8;
      else if (src->nat_attack_type == 6)
        hit_type = 5;
      else if (src->nat_attack_type == 7)
        hit_type = 1;
      else if (src->nat_attack_type == 8)
        hit_type = 6;
    }

  /* Must be standing or fighting AND not in frantic mode */

  if ((GET_POS (tar) != STAND && GET_POS (tar) != FIGHT) ||
      tar->fight_mode == 0)
    {
      def_result = RESULT_NONE;
      off_result = ignore_offense[off_success];
      sprintf (AD, "IGNORE:  %s = %s\n",
	       cs_name[off_success], rs_name[off_result]);
    }

  else if (defense_hand == 1 && !defense_weapon)
    {
      off_result = dodge_offense[off_success][def_success];
      def_result = dodge_defense[off_success][def_success];
      sprintf (AD, "DODGE:  %s(%d) = %s(%d);    %s(%d) = %s(%d)\n",
	       cs_name[off_success], off_success, rs_name[off_result],
	       off_result, cs_name[def_success], def_success,
	       rs_name[def_result], def_result);
    }

  else if (defense_weapon)
    {
      off_result = shield_parry_offense[off_success][def_success];
      def_result = shield_parry_defense[off_success][def_success];

      if (off_result == RESULT_BLOCK && defense_weapon != shield)
	{
	  off_result = RESULT_PARRY;
	  def_result = RESULT_PARRY;
	}

      sprintf (AD, "BLOCK/PARRY:  %s = %s;    %s = %s\n",
	       cs_name[off_success], rs_name[off_result],
	       cs_name[def_success], rs_name[def_result]);
    }

  figure_damage (src, tar, attack_weapon, off_result, &damage, &location);

  sprintf (loc, "%s", figure_location (tar, location));

  wear_loc1 = body_tab[0][location].wear_loc1;
  wear_loc2 = body_tab[0][location].wear_loc2;
  eq1 = get_equip (tar, wear_loc1);
  eq2 = get_equip (tar, wear_loc2);

/*
  if (eq2 && IS_SET (eq2->obj_flags.wear_flags, ITEM_WEAR_ABOUT)
      && (isname ("cloak", eq2->name) || isname ("cape", eq2->name))
      && number (0, 2))
    eq2 = NULL;
*/

  if (off_result == RESULT_FUMBLE)
    {

      if (GET_DEX (src) <= number (1, 21))
	{
	  if (attack_weapon && number (0, 1))
	    off_result = RESULT_NEAR_FUMBLE;
	  else
	    off_result = RESULT_NEAR_STUMBLE;
	}

      else if (!attack_weapon || number (0, 1))
	off_result = RESULT_STUMBLE;

      if (off_result != RESULT_FUMBLE)
	sprintf (AD, "offensive result -> %s\n", rs_name[off_result]);
    }

  if (def_result == RESULT_FUMBLE)
    {

      if (GET_DEX (tar) <= number (1, 21))
	{
	  if (defense_weapon && number (0, 1))
	    def_result = RESULT_NEAR_FUMBLE;
	  else
	    def_result = RESULT_NEAR_STUMBLE;
	}

      else if (!defense_weapon || number (0, 1))
	def_result = RESULT_STUMBLE;

      if (def_result != RESULT_FUMBLE)
	sprintf (AD, "defensive result -> %s\n", rs_name[def_result]);
    }

  /* DA can occur only if defending a primary attacker */

  if (def_result == RESULT_ADV && tar->fighting != src)
    def_result = RESULT_NONE;
/*
  if (attack_weapon
      && (off_result == RESULT_BLOCK || off_result == RESULT_PARRY))
  {
    if (attack_weapon && ((defense_weapon
	&& attack_weapon->quality <= defense_weapon->quality) 
        || (shield && attack_weapon->quality <= shield->quality))
	  && number (1, 100) > attack_weapon->quality)
    {
      if (shield)
        object__add_damage (attack_weapon, DAMAGE_BLUNT, number (1, MAX (damage, 5)));
      else
	object__add_damage (attack_weapon, (DAMAGE_TYPE) defense_weapon->o.weapon.hit_type, 
        number (1, MAX (damage, 5)));

      if ((number (1, 100) > attack_weapon->quality && number (1, 100) > attack_weapon->item_wear)
	  || attack_weapon->item_wear <= 0)
        off_result = RESULT_WEAPON_BREAK;
    }
    else if (attack_weapon && defense_weapon && number (1, 100) > defense_weapon->quality)
    {
      object__add_damage (defense_weapon, (DAMAGE_TYPE) hit_type, number (1, MAX (damage, 5)));

      if ((number (1, 100) > defense_weapon->quality 
          && number (1, 100) > defense_weapon->item_wear) || defense_weapon->item_wear <= 0)
        def_result = RESULT_WEAPON_BREAK;
    }
    else if (attack_weapon && shield && number (1, 100) > shield->quality)
    {
      object__add_damage (shield, (DAMAGE_TYPE) hit_type, number (1, MAX (damage, 5)));
      if ((number (1, 100) > shield->quality && number (1, 100) > shield->item_wear)
	  || shield->item_wear <= 0)
        def_result = RESULT_SHIELD_BREAK;
    }
      else if (attack_weapon
	       &&
	       ((defense_weapon
		 && attack_weapon->quality > defense_weapon->quality)
		|| (shield && attack_weapon->quality > shield->quality))
	       && attack_weapon->quality > defense_weapon->quality
	       && number (1, 100) > attack_weapon->quality)
	{
	  if (shield)
	    object__add_damage (attack_weapon, DAMAGE_BLUNT,
				number (1, MAX (damage, 5)));
	  else
	    object__add_damage (attack_weapon,
				(DAMAGE_TYPE) defense_weapon->o.weapon.
				hit_type, number (1, MAX (damage, 5)));
	  if ((number (1, 100) > attack_weapon->quality
	       && number (1, 100) > attack_weapon->item_wear)
	      || attack_weapon->item_wear <= 0)
	    off_result = RESULT_WEAPON_BREAK;
	}
    }

  if ((off_result == RESULT_HIT1 || off_result == RESULT_HIT2
       || off_result == RESULT_HIT3 || off_result == RESULT_HIT4)
      && attack_weapon && (eq1 || eq2))
    {
      if (attack_weapon && eq1 && eq1->o.od.value[0] > 2
	  && attack_weapon->quality <= eq1->quality
	  && number (1, 100) > attack_weapon->quality)
	{
	  object__add_damage (attack_weapon, DAMAGE_BLUNT,
			      number (1, MAX (damage, 5)));
	  if ((number (1, 100) > attack_weapon->quality
	       && number (1, 100) > attack_weapon->item_wear)
	      || attack_weapon->item_wear <= 0)
	    {
	      off_result = RESULT_WEAPON_BREAK;
	      def_result = RESULT_ANY;
	    }
	}
      else if (attack_weapon && eq2 && eq2->o.od.value[0] > 2
	       && attack_weapon->quality <= eq2->quality
	       && number (1, 100) > attack_weapon->quality)
	{
	  object__add_damage (attack_weapon, DAMAGE_BLUNT,
			      number (1, MAX (damage, 5)));
	  if ((number (1, 100) > attack_weapon->quality
	       && number (1, 100) > attack_weapon->item_wear)
	      || attack_weapon->item_wear <= 0)
	    {
	      off_result = RESULT_WEAPON_BREAK;
	      def_result = RESULT_ANY;
	    }
	}
      else if (attack_weapon && eq1 && number (1, 100) > eq1->quality)
	{
	  if (GET_ITEM_TYPE (eq1) == ITEM_ARMOR)
	    object__add_damage (eq1, (DAMAGE_TYPE) hit_type,
				number (1, MAX (damage, 5)));
	  else
	    object__add_damage (eq1, (DAMAGE_TYPE) hit_type,
				number (1, MAX (damage, 5)));
	  if ((number (1, 100) > eq1->quality
	       && number (1, 100) > eq1->item_wear) || eq1->item_wear <= 0)
	    broken_eq = eq1;
	}
      else if (attack_weapon && eq2 && number (1, 100) > eq2->quality)
	{
	  if (GET_ITEM_TYPE (eq2) == ITEM_ARMOR)
	    object__add_damage (eq2, (DAMAGE_TYPE) hit_type,
				number (1, MAX (damage, 5)));
	  else
	    object__add_damage (eq2, (DAMAGE_TYPE) hit_type,
				number (1, MAX (damage, 5)));
	  if (number (1, 100) > eq2->quality
	      && number (1, 100) > eq2->item_wear)
	    broken_eq = eq2;
	}
      else if (attack_weapon && eq1 && eq1->o.od.value[0] > 2
	       && attack_weapon->quality > eq1->quality
	       && number (1, 100) > attack_weapon->quality)
	{
	  object__add_damage (attack_weapon, DAMAGE_BLUNT,
			      number (1, MAX (damage, 5)));
	  if ((number (1, 100) > attack_weapon->quality
	       && number (1, 100) > attack_weapon->item_wear)
	      || attack_weapon->item_wear <= 0)
	    {
	      off_result = RESULT_WEAPON_BREAK;
	      def_result = RESULT_ANY;
	    }
	}
      else if (attack_weapon && eq2 && eq2->o.od.value[0] > 2
	       && attack_weapon->quality > eq2->quality
	       && number (1, 100) > attack_weapon->quality)
	{
	  object__add_damage (attack_weapon, DAMAGE_BLUNT,
			      number (1, MAX (damage, 5)));
	  if ((number (1, 100) > attack_weapon->quality
	       && number (1, 100) > attack_weapon->item_wear)
	      || attack_weapon->item_wear <= 0)
	    {
	      off_result = RESULT_WEAPON_BREAK;
	      def_result = RESULT_ANY;
	    }
	}
    }
*/
  if (IS_SET (tar->flags, FLAG_COMPETE))
    {
      if (attack_weapon)
	return wound_to_char (tar, loc, damage,
			      attack_weapon->o.weapon.hit_type, 0, 0, 0);
      else
	return wound_to_char (tar, loc, damage, src->nat_attack_type, 0, 0,
			      0);
    }

  if (tar->room->nVirtual == 60001 || tar->room->nVirtual == 60002
      || tar->room->nVirtual == 60003 || tar->room->nVirtual == 60004)
    arena_combat_message (src, tar, loc, damage, tar->room->nVirtual);
	
  combat_results (src,
		  tar,
		  attack_weapon,
		  defense_weapon,
		  broken_eq,
		  damage,
		  loc,
		  off_result,
		  def_result, attack_num, fd, off_success, def_success, off_style, def_style, location, att_diff, def_diff);

  sprintf (AD, "---------------------------------------\n");


  if (get_second_affect(src, SA_FEINT, NULL) && attack_num == 1)
    remove_second_affect(get_second_affect(src, SA_FEINT, NULL));

  for (dch = src->room->people; dch; dch = dch->next_in_room)
    if (IS_SET (dch->debug_mode, DEBUG_FIGHT))
      send_to_char (fd, dch);

  if (tar->deleted)
    return 1;
  else
    {
      if (IS_RIDER (tar))
	{

	  mount = tar->mount;
/*
			if ( mount && (mount->skills [SKILL_RIDE] < 33 ||
				 (mount->skills [SKILL_RIDE] < 66 &&
				  !skill_use (mount, SKILL_RIDE, 0))) ) {
				dump_rider (tar, false);
				flee_attempt (mount);
			}
*/
	}

      return 0;
    }
}

void
combat_msg_substitute (char *msg, char *template_string, CHAR_DATA * src,
		       CHAR_DATA * tar, OBJ_DATA * attack_weapon,
		       OBJ_DATA * defense_weapon, char *location, int damage, int att_diff, int def_diff)
{
  int aw_type = 6;
  char loc[MAX_STRING_LENGTH];


  if (attack_weapon)
    aw_type = attack_weapon->o.weapon.hit_type;

  *msg = '\0';

  while (*template_string)
    {

      if (*template_string != '#')
	{
	  *msg = *template_string;
	  template_string++;
	  msg++;
	  *msg = '\0';
	}

      else
	{
	  switch (template_string[1])
	    {
	    case 'S':
	      if (aw_type == 6)
		strcat (msg, attack_names_plural[src->nat_attack_type]);
	      else
		strcat (msg, wtype2[aw_type * 3 + number (0, 2)]);
	      break;
	    case 's':
	      if (aw_type == 6)
		strcat (msg, attack_names[src->nat_attack_type]);
	      else
		strcat (msg, wtype[aw_type * 3 + number (0, 2)]);
	      break;

            // This next section adds a bit of extra flavour, describing
            // how badly you miss or how well you block any given hit.
            // No coded effect but cosmetic, but should be a good demonstration
            // of your actual combat skill.

            case 'Z':
               if(att_diff > 0)
                 strcat (msg, miss_descriptor[8 + number(0,1)]);
               else if (att_diff > -10)
                 strcat (msg, miss_descriptor[6 + number(0,1)]);
               else if (att_diff > -20)
                 strcat (msg, miss_descriptor[4 + number(0,1)]);
               else if (att_diff > -40)
                 strcat (msg, miss_descriptor[2 + number(0,1)]);
               else
                 strcat (msg, miss_descriptor[0 + number(0,1)]);               
	       break;
           
            case 'X':
               if(def_diff > 10)
                 strcat (msg, block_descriptor[10 + number(0,1)]);
               else if(def_diff > -5)
                 strcat (msg, block_descriptor[8 + number(0,1)]);
               else if (def_diff > -15)
                 strcat (msg, block_descriptor[6 + number(0,1)]);
               else if (def_diff > - 25)
                 strcat (msg, block_descriptor[4 + number(0,1)]);
               else if (def_diff > - 40)
                 strcat (msg, block_descriptor[2 + number(0,1)]);
               else
                 strcat (msg, block_descriptor[0 + number(0,1)]);
	       break;

            case 'G':
               if(att_diff > 20)
                 strcat (msg, generic_descriptor[16 + number(0,3)]);
               else if(att_diff > 0)
                 strcat (msg, generic_descriptor[12 + number(0,3)]);
               else if (att_diff > -20)
                 strcat (msg, generic_descriptor[8 + number(0,3)]);
               else if (att_diff > - 40)
                 strcat (msg, generic_descriptor[4 + number(0,3)]);
               else
                 strcat (msg, generic_descriptor[0 + number(0,3)]);
	       break;

            case 'Y':
               if(def_diff > 10)
                 strcat (msg, parry_descriptor[10 + number(0,1)]);
               else if(def_diff > -5)
                 strcat (msg, parry_descriptor[8 + number(0,1)]);
               else if (def_diff > -15)
                 strcat (msg, parry_descriptor[6 + number(0,1)]);
               else if (def_diff > -25)
                 strcat (msg, parry_descriptor[4 + number(0,1)]);
               else if (def_diff > -40)
                 strcat (msg, parry_descriptor[2 + number(0,1)]);
               else
                 strcat (msg, parry_descriptor[0 + number(0,1)]);
	       break;

	    case 'O':
	      if (!attack_weapon)
		strcat (msg, attack_part[src->nat_attack_type]);
	      else
		strcat (msg, OBJN (attack_weapon, tar));
	      break;
	    case 'o':
	      if (!defense_weapon)
		strcat (msg, "fist");
	      else
		strcat (msg, OBJN (defense_weapon, src));
	      break;
	    case 'L':
	      sprintf (loc, "%s", expand_wound_loc (location));
	      strcat (msg, loc);
	      break;
	    case 'C':
	      strcat (msg, crithits[aw_type * 2 + number (0, 1)]);
	      break;
	    case 'c':
	      strcat (msg, crithit[aw_type * 2 + number (0, 1)]);
	      break;
	    case 'B':
	      strcat (msg, break_def[aw_type * 2 + 1]);
	      break;
	    case 'b':
	      strcat (msg, break_def[aw_type * 2]);
	      break;
	    case 'D':
	      strcat (msg, get_dam_word (damage, tar));
	      break;
	    case 'R':
	      strcat (msg, "\n\r");
	      break;
	    default:
	      *msg = *template_string;	/* Not a variable */
	      msg[1] = '\0';
	      template_string--;	/* Work around for advance 2 chars */
	      break;
	    }

	  msg = &msg[strlen (msg)];

	  template_string = &template_string[2];	/* Advance two characters */
	}
    }
}

void
combat_results (CHAR_DATA * src, CHAR_DATA * tar, OBJ_DATA * attack_weapon,
		OBJ_DATA * defense_weapon, OBJ_DATA * broken_eq, int damage,
		char *location, int off_result, int def_result,
		int attack_num, char *fd, int off_success, int def_success, 
                int off_style, int def_style, int org_loc, int att_diff, int def_diff)
{
  int attack_delay;
  int i;
  int j;
  int skill;
  int delay_modifier;
  int hit_type = 0;
  int table;
  int current_sum;
  int best_sum;
  int poisoned = 0;
  int body_type = 0;
  POISON_DATA *poison;
  AFFECTED_TYPE *invulnerability = NULL;
  COMBAT_MSG_DATA *best_cm = NULL;
  COMBAT_MSG_DATA *tcm = NULL;
  CHAR_DATA *tch;
  OBJ_DATA *tobj;
  OBJ_DATA *eq;
  char msg1[MAX_STRING_LENGTH];
  char msg2[MAX_STRING_LENGTH];
  char msg3[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  char log_message[MAX_STRING_LENGTH];
  extern COMBAT_MSG_DATA *cm_list;

  *log_message = '\0';

  if (tar)
    {
      sprintf (log_message, "by %s", src->short_descr);
      if (IS_NPC (src))
	sprintf (log_message + strlen (log_message), " (%d)",
		 src->mob->nVirtual);
      else
	sprintf (log_message + strlen (log_message), " (PC %s)", src->tname);
    }

  if (attack_weapon)
    sprintf (log_message + strlen (log_message), " w/ %s (%d)",
	     attack_weapon->short_description, attack_weapon->nVirtual);

  if ((off_result < RESULT_HIT || off_result > RESULT_HIT4) && !number (0, 1))
    {

      skill = SKILL_BRAWLING;

      if (attack_weapon)
	skill = attack_weapon->o.weapon.use_skill;

      skill_learn(src, skill);
      skill_use (src, skill, 0);


      if(off_style)
        skill_use (src, off_style, 0);

      fix_offense (src);
    }

  if (off_result >= RESULT_HIT && off_result <= RESULT_HIT4 && !number (0, 1))
    {

      if (GET_POS (tar) == STAND || GET_POS (tar) == FIGHT)
	{

	  if (!defense_weapon)
	    skill = SKILL_DODGE;
	  else if (defense_weapon->obj_flags.type_flag == ITEM_WEAPON)
	    skill = SKILL_AVERT;
	  else if (defense_weapon->obj_flags.type_flag == ITEM_SHIELD)
	    skill = SKILL_AVERT;
	  else
	    {
	      skill = SKILL_AVERT;
	    }

	  skill_use (tar, skill, 0);

          if(def_style)
            skill_use (tar, def_style, 0);

	  fix_offense (tar);
	}
    }


  if (tar->fight_mode == 0 &&	/* frantic mode */
      (GET_POS (tar) == STAND || GET_POS (tar) == FIGHT))
    table = 'F';

  else if (GET_POS (tar) != STAND && GET_POS (tar) != FIGHT &&
	   tar->fight_mode != 0)
    table = 'I';		/* Ignore */

  else if (!defense_weapon)
    table = 'D';		/* Dodge */

  else if (defense_weapon->obj_flags.type_flag == ITEM_SHIELD)
    table = 'B';		/* Block */

  else
    table = 'P';		/* Parry */

  for (tcm = cm_list; tcm; tcm = tcm->next)
    {

      if (off_result != tcm->off_result)
	continue;

      if (tcm->table != '*' && tcm->table != table)
	continue;

      if (tcm->def_result != RESULT_ANY && tcm->def_result != def_result)
	continue;

      if (!best_cm)
	{
	  best_cm = tcm;
	  continue;
	}

      /* Hierarchy                */
      /*              x any * msg                     */
      /*              x any t msg                     */
      /*              x y   * msg                     */
      /*              x y   t msg                     */

      current_sum = 0;
      if (best_cm->def_result != RESULT_ANY)
	current_sum += 2;

      if (best_cm->table != '*')
	current_sum += 1;

      if (current_sum == 3)
	break;

      best_sum = 0;

      if (tcm->def_result != RESULT_ANY)
	best_sum += 2;

      if (tcm->table != '*')
	best_sum += 1;

      if (current_sum < best_sum)
	best_cm = tcm;
    }

  combat_msg_substitute (msg1, best_cm->def_msg, src, tar, attack_weapon,
			 defense_weapon, location, damage, att_diff, def_diff);
  combat_msg_substitute (msg2, best_cm->off_msg, src, tar, attack_weapon,
			 defense_weapon, location, damage, att_diff, def_diff);
  combat_msg_substitute (msg3, best_cm->other_msg, src, tar, attack_weapon,
			 defense_weapon, location, damage, att_diff, def_diff);

  /* Oh wait, vehicles get a generic message */

  if (IS_SET (tar->act, ACT_VEHICLE))
    {
      if (attack_weapon)
	{
	  act ("You attack $N with your $o.",
	       false, src, attack_weapon, tar, TO_CHAR);
	  act ("$n attacks you with his $o.",
	       false, src, attack_weapon, tar, TO_VICT);
	  act ("$n attacks $N with $s $o.",
	       false, src, attack_weapon, tar, TO_NOTVICT);
	}
      else
	{
	  act ("You kick at $N, trying to destroy it.",
	       false, src, 0, tar, TO_CHAR);
	  act ("$n kicks at you, trying to break you.",
	       false, src, 0, tar, TO_VICT);
	  act ("$n kicks at $N, trying to break it.",
	       false, src, 0, tar, TO_NOTVICT);
	}
    }
  else
    {
      act (msg1, false, tar, attack_weapon, src,
	   TO_CHAR | _ACT_FORMAT | _ACT_COMBAT | _HIGHLIGHT);
      act (msg2, false, tar, attack_weapon, src,
	   TO_VICT | _ACT_FORMAT | _ACT_COMBAT | _HIGHLIGHT);
      act (msg3, false, tar, attack_weapon, src,
	   TO_NOTVICT | _ACT_FORMAT | _ACT_COMBAT);
    }

  /* Make sure invulnerability is set last in the IF clause below. */

  if (damage &&
      attack_weapon &&
      !IS_SET (attack_weapon->obj_flags.extra_flags, ITEM_MAGIC) &&
      (invulnerability = get_affect (tar, MAGIC_AFFECT_INVULNERABILITY)))
    {
      act ("$n appears undamaged!", false, tar, 0, 0, TO_ROOM);
      act ("$n's $o didn't wound you.",
	   false, src, attack_weapon, tar, TO_VICT);
    }

  if (damage)
    {

      if (attack_weapon)
	hit_type = attack_weapon->o.weapon.hit_type;
      else
	hit_type = 6;		/* natural attack */

      if (off_result != RESULT_HIT && off_result != RESULT_HIT1 &&
	  off_result != RESULT_HIT2 && off_result != RESULT_HIT3 &&
	  off_result != RESULT_HIT4)
	system_log ("Damage result without a HIT.", true);
    }

  else if (off_result == RESULT_STUMBLE)
    {
      GET_POS (src) = SIT;
      add_second_affect (SA_STAND, ((25-GET_AGI(src))+number(1,3)), src, NULL, NULL, 0);

      if (IS_SET (tar->flags, FLAG_FLEE))
	flee_attempt (tar);
    }

  else if (off_result == RESULT_FUMBLE)
    {
      if ((tobj = get_equip (src, WEAR_PRIM)) == attack_weapon
	  && !IS_SET (src->flags, FLAG_COMPETE))
	obj_to_room (unequip_char (src, WEAR_PRIM), src->in_room);
      else if ((tobj = get_equip (src, WEAR_SEC)) == attack_weapon
	       && !IS_SET (src->flags, FLAG_COMPETE))
	obj_to_room (unequip_char (src, WEAR_SEC), src->in_room);
      else if ((tobj = get_equip (src, WEAR_BOTH)) == attack_weapon
	       && !IS_SET (src->flags, FLAG_COMPETE))
	obj_to_room (unequip_char (src, WEAR_BOTH), src->in_room);
      else if (!IS_SET (src->flags, FLAG_COMPETE))
	system_log ("Disarm, but couldn't find weapons's hand. (attacker)",
		    true);

      if (!IS_SET (src->flags, FLAG_COMPETE))
	{  
	  if (attack_weapon == src->right_hand)
	    src->right_hand = NULL;
	  else if (attack_weapon == src->left_hand)
	    src->left_hand = NULL;
	  add_second_affect (SA_GET_OBJ, (10-(GET_DEX(src)/5)+number(2,4)), src, attack_weapon, "", 0);
	  add_second_affect (SA_WEAR_OBJ, (13-(GET_DEX(src)/5)+number(2,5)), src, attack_weapon, "", 0);
	  attack_weapon->tmp_flags |= SA_DROPPED;
	}
    }

/*  else if (off_result == RESULT_WEAPON_BREAK)
    {
      if (get_equip (src, WEAR_PRIM) == attack_weapon)
	extract_obj (unequip_char (src, WEAR_PRIM));
      else if (get_equip (src, WEAR_SEC) == attack_weapon)
	extract_obj (unequip_char (src, WEAR_SEC));
      else if (get_equip (src, WEAR_BOTH) == attack_weapon)
	extract_obj (unequip_char (src, WEAR_BOTH));
      else
	system_log
	  ("Weapon Break, but couldn't find weapons's hand. (attacker)",
	   true);

      if (attack_weapon == src->right_hand)
	src->right_hand = NULL;
      else if (attack_weapon == src->left_hand)
	src->left_hand = NULL;
    }
*/

  else if (off_result == RESULT_ADV)
    system_log ("How could offence get RESULT_ADV?", true);

  if (def_result == RESULT_FUMBLE)
    {
      if (get_equip (tar, WEAR_PRIM) == defense_weapon
	  && !IS_SET (tar->flags, FLAG_COMPETE))
	obj_to_room (unequip_char (tar, WEAR_PRIM), tar->in_room);
      else if (get_equip (tar, WEAR_SEC) == defense_weapon
	       && !IS_SET (tar->flags, FLAG_COMPETE))
	obj_to_room (unequip_char (tar, WEAR_SEC), tar->in_room);
      else if (get_equip (tar, WEAR_BOTH) == defense_weapon
	       && !IS_SET (tar->flags, FLAG_COMPETE))
	obj_to_room (unequip_char (tar, WEAR_BOTH), tar->in_room);
      else if (get_equip (tar, WEAR_SHIELD) == defense_weapon
	       && !IS_SET (tar->flags, FLAG_COMPETE))
	obj_to_room (unequip_char (tar, WEAR_SHIELD), tar->in_room);
      else if (!IS_SET (tar->flags, FLAG_COMPETE))
	system_log ("Disarm, but couldn't find weapons's hand. (defender)",
		    true);

      if (!IS_SET (tar->flags, FLAG_COMPETE))
	{
	  if (defense_weapon == tar->right_hand)
	    tar->right_hand = NULL;
	  else if (defense_weapon == tar->left_hand)
	    tar->left_hand = NULL;
	  add_second_affect (SA_GET_OBJ, (10-(GET_DEX(tar)/5)+number(2,4)), tar, defense_weapon, "", 0);
	  add_second_affect (SA_WEAR_OBJ, (13-(GET_DEX(tar)/5)+number(2,5)), tar, defense_weapon, "", 0);
	  defense_weapon->tmp_flags |= SA_DROPPED;
	}
    }

/*  else if (def_result == RESULT_WEAPON_BREAK ||
	   def_result == RESULT_SHIELD_BREAK)
    {
      if (get_equip (tar, WEAR_PRIM) == defense_weapon)
	extract_obj (unequip_char (tar, WEAR_PRIM));
      else if (get_equip (tar, WEAR_SEC) == defense_weapon)
	extract_obj (unequip_char (tar, WEAR_SEC));
      else if (get_equip (tar, WEAR_BOTH) == defense_weapon)
	extract_obj (unequip_char (tar, WEAR_BOTH));
      else if (get_equip (tar, WEAR_SHIELD) == defense_weapon)
	extract_obj (unequip_char (tar, WEAR_SHIELD));
      else
	system_log ("BREAK, but couldn't find weapons's hand. (defender)",
		    true);

      if (defense_weapon == tar->right_hand)
	tar->right_hand = NULL;
      else if (defense_weapon == tar->left_hand)
	tar->left_hand = NULL;
    }
*/

  else if (def_result == RESULT_STUMBLE)
    {
      if (GET_POS (tar) == FIGHT || GET_POS (tar) == STAND)
	{
	  GET_POS (tar) = SIT;
	  add_second_affect (SA_STAND, ((25-GET_AGI(tar))+number(1,3)), tar, NULL, NULL, 0);
	}
      else
	def_result = RESULT_NONE;
    }

/*  if (damage && src->venom
      && (off_result == RESULT_HIT3 || off_result == RESULT_HIT4))
    poison_bite (src, tar);*/

  if (attack_weapon)
    hit_type = attack_weapon->o.weapon.hit_type;
  else
    {
      if (src->nat_attack_type == 0 || src->nat_attack_type == 4)
	hit_type = 9;
      else if (src->nat_attack_type == 1 || src->nat_attack_type == 3 || src->nat_attack_type == 5)
	hit_type = 7;
      else if (src->nat_attack_type == 2)
	hit_type = 8;
      else if (src->nat_attack_type == 6)
        hit_type = 5;
      else if (src->nat_attack_type == 7)
        hit_type = 1;
      else if (src->nat_attack_type == 8)
        hit_type = 6;
    }

  if (broken_eq)
    {
      sprintf (buf, "#2%s#0 is destroyed by the blow.",
	       obj_short_desc (broken_eq));
      buf[2] = toupper (buf[2]);
      act (buf, false, tar, 0, 0, TO_CHAR | _ACT_FORMAT | _ACT_COMBAT);
      act (buf, false, tar, 0, 0, TO_ROOM | _ACT_FORMAT | _ACT_COMBAT);
      extract_obj (broken_eq);
    }

// Per Evan's suggestion, most professional combat is more
// a bludgeoning match until someone falls over unconscious.
// Therefore, if the damage you do is less than the armor
// value of the piece of armour, chances are the damage
// type is converted to blunt damage.

  int wear_loc1 = body_tab[body_type][org_loc].wear_loc1;
  eq = get_equip (tar, wear_loc1);

  if (eq && eq->obj_flags.type_flag == ITEM_ARMOR && (hit_type !=  5 || hit_type != 6 ))
    if(damage < eq->o.armor.armor_value && number(0, eq->o.armor.armor_value - damage))
      hit_type = 3;

// If they strike has some venom, did some damage, and the target isn't invincible,
// add all the poisons they have, IF
// they weren't hit with fists or a blunt weapon,
// If the attacker is using fangs or a piercing weapon, roll a 1d20, and if that is
// higher than the target's con - damage, they're poisoned,
// If the attacker is using something else, roll a 1d20, and they're poisoned if the
// roll is higher than their con + 10 - damage.
// e.g., For an average 14 con, you'll need to do around 10~15 damage with a slashing
// weapon to have a 50% chance to poison, and 5~10 damage with a piercing weapon

  if(attack_weapon && attack_weapon->poison && damage && !invulnerability)
  {
    for (poison = attack_weapon->poison; poison; poison = poison->next)
    {
      if((hit_type != 3 && hit_type != 9) && 
        ((number(1,20) >= (GET_CON(tar) + 10 - damage)) || 
        ((number(1,20) >= (GET_CON(tar) + 5- damage)) && 
        (hit_type == 0 || hit_type == 7 || hit_type == 1))))
      {

        soma_add_affect(tar, poison->poison_type, poison->duration, poison->latency, 
                        poison->minute, poison->max_power, poison->lvl_power, poison->atm_power,
                        poison->attack, poison->decay, poison->sustain, poison->release);
 
        poisoned = poison->poison_type;
      }

      // For each strike you make with a weapon, if it hits the target, some of
      // the poison then rubs off.
  
      if((off_result >= RESULT_HIT && off_result <= RESULT_HIT4) 
          && (def_result != RESULT_PARRY || def_result != RESULT_BLOCK 
              || def_result != RESULT_ADV))
      {
        if(poison->uses == 0 || poison->uses == 1)
          remove_object_poison(attack_weapon, poison);
        else if(poison->uses > -1)
          poison->uses --;
      }
    }
  }

  if(!attack_weapon && src->venom && damage && !invulnerability)
  {
    for (poison = src->venom; poison; poison = poison->next)
    {
      if((hit_type != 3 && hit_type != 9) && 
        ((number(1,20) >= (GET_CON(tar) + 10 - damage)) || 
        ((number(1,20) >= (GET_CON(tar) + 5 - damage)) && 
        (hit_type == 0 || hit_type == 7 || hit_type == 1))))
      {

        soma_add_affect(tar, poison->poison_type, poison->duration, poison->latency, 
                        poison->minute, poison->max_power, poison->lvl_power, poison->atm_power,
                        poison->attack, poison->decay, poison->sustain, poison->release);
 
        poisoned = poison->poison_type;
      }
    }
  }

  if(poisoned > 0)
    send_to_char(lookup_poison_variable(poisoned, 2), tar);

  tar->delay_ch = src;
  if (attack_weapon)
    tar->delay_info1 = attack_weapon->nVirtual;

  if (!invulnerability && damage
      && wound_to_char (tar, location, damage, hit_type, 0, poisoned, org_loc + 50))
    {
      def_result = RESULT_DEAD;
    }

  else
    {

      if (tar->fighting && IS_SET (tar->act, ACT_VEHICLE))
	stop_fighting (tar);

      if (!tar->fighting && !IS_SET (tar->act, ACT_VEHICLE))
	{
	  set_fighting (tar, src);
	  if (!AWAKE (tar) && GET_POS (tar) != POSITION_UNCONSCIOUS)
	    do_wake (tar, "", 0);
	  add_second_affect (SA_STAND, 8, tar, NULL, NULL, 0);
	}

      if (!src->fighting)
	set_fighting (src, tar);

      if ((GET_POS (tar) == POSITION_STUNNED
	   || GET_POS (tar) == POSITION_UNCONSCIOUS)
	  && !IS_SET (src->flags, FLAG_KILL))
	{

	  if (src->fighting)
	    stop_fighting (src);

	  for (tch = src->room->people; tch; tch = tch->next_in_room)
	    if (tch->fighting == src && tch != src && GET_HIT (tch) > 0)
	      {
		set_fighting (src, tch);
		break;
	      }
	}
    }

  tar->delay_ch = NULL;
  tar->delay_info1 = 0;

  if (!src->deleted && !tar->deleted)
    spell_defenses (tar, src);	/* Bee attack and such */

  if (def_result == RESULT_PARRY || def_result == RESULT_BLOCK)
    {
      if (get_equip (tar, WEAR_PRIM) == defense_weapon ||
	  get_equip (tar, WEAR_BOTH) == defense_weapon)
	tar->primary_delay+= 2;
      else if (get_equip (tar, WEAR_SEC) == defense_weapon)
	tar->secondary_delay+= 2;
    }

  else if (def_result == RESULT_ADV)
    {
      tar->primary_delay = 0;
      tar->secondary_delay = 0;
    }

  attack_delay = 0;

  /* Weapon delay / natural delay / calculated delay */

  if (attack_weapon)
    {
      attack_delay += use_table[attack_weapon->o.weapon.use_skill].delay;
      // attack_delay += attack_weapon->o.od.value[5]; - ov5 is now damage bonus.
      if ((attack_weapon->o.od.value[0] == 1)
	  && attack_weapon->location == WEAR_BOTH)
      {
	attack_delay += 20;
      }

      if ((attack_weapon->o.od.value[0] == 3)
	  && ((attack_weapon->location == WEAR_PRIM) || (attack_weapon->location == WEAR_SEC)))
	attack_delay += 16;

      if ((attack_weapon->o.od.value[0] == 3)
	  && (attack_weapon->location == WEAR_BOTH))
	attack_delay += 32;
    }

  else if (IS_NPC (src))
    attack_delay += 30 - src->natural_delay + use_table[SKILL_BRAWLING].delay;
  else
    attack_delay += 20 - GET_AGI (src) + use_table[SKILL_BRAWLING].delay;

  sprintf (AD, "AttDel %d ", attack_delay);
  /* agi delay adjustment */

  delay_modifier = 126;

  delay_modifier = delay_modifier - (GET_AGI (src) * 3);

  sprintf (AD, "AGIadj %d%% ", delay_modifier);

  attack_delay = attack_delay * delay_modifier / 100;

  /* Fatigue adjustment */

  if (GET_MAX_MOVE (src) > 0)
    j = GET_MOVE (src) * 100 / GET_MAX_MOVE (src);
  else
    j = 0;

  if (j > 100)
    j = 100;

  for (i = 0; j > fatigue[i].percent; i++)
    ;

  if (i == 0)
    attack_delay += 8;		/* Completely Exhausted */
  else if (i == 1)
    attack_delay += 4;		/* Exhausted */

  sprintf (AD, "Fatigue %d ", attack_delay);

  if (get_second_affect(src, SA_AIMSTRIKE, NULL))
  {
    attack_delay = attack_delay * ((get_second_affect(src, SA_AIMSTRIKE, NULL)->seconds)/100) ;
    attack_delay += number(5,8);
    remove_second_affect(get_second_affect (src, SA_AIMSTRIKE, NULL));
  }

  /* Fightmode delay adjustment */

  attack_delay += fight_tab[src->fight_mode].delay;
  attack_delay += number(0,3);

  sprintf (AD, "FM %d = %d [%d] (2)=%d\n", attack_delay, attack_delay,
	   attack_num, src->secondary_delay);

  if (attack_num == 1)
    src->primary_delay = attack_delay;
  else
    src->secondary_delay = attack_delay;
}

void
figure_damage (CHAR_DATA * src, CHAR_DATA * tar, OBJ_DATA * attack_weapon,
	       int off_result, int *damage, int *location)
{
  OBJ_DATA *eq;
  char buf[MAX_STRING_LENGTH];
  int body_type;
  int wear_loc1;
  int wear_loc2;
  int i;
  float dam = 0;
  bool chill = false;
  AFFECTED_TYPE *shock = NULL;
  AFFECTED_TYPE *af;

  *damage = 0;

  /* Determine hit location
     If we have a second_affect of AIMSTRIKE, then we already have our
     selected location, at the expense of more delay timer. */

  body_type = 0;
  *location = -1;

  if(get_second_affect(src, SA_AIMSTRIKE, NULL))
  {   
    *location = get_second_affect(src, SA_AIMSTRIKE, NULL)->info2;
  }
  else
  {
    i = number (1, 100);
    while (i > 0)
      i = i - body_tab[body_type][++(*location)].percent;
  }

    wear_loc1 = body_tab[body_type][*location].wear_loc1;
    wear_loc2 = body_tab[body_type][*location].wear_loc2;


  /* Assess automatic damage by NPCs */

  if (IS_NPC (src))
    dam += src->mob->damroll;

  /* For weapons, add weapon damage roll and affects */

  if (attack_weapon)
    {
      if (attack_weapon->o.weapon.dice && attack_weapon->o.weapon.sides)
	{

        // A double-gripped medium weapon adds 1 to the roll.
        // A single-gripped heavy weapon loses 1 to the roll.

	  if ((attack_weapon->o.od.value[0] == 1)
	      && attack_weapon->location == WEAR_BOTH)
	      {
                dam += dice (attack_weapon->o.weapon.dice, attack_weapon->o.weapon.sides) + 1;
              }
	  else if ((attack_weapon->o.od.value[0] == 3)
	      && (attack_weapon->location == WEAR_PRIM || WEAR_SEC))
              {
 	      	dam += dice (attack_weapon->o.weapon.dice, attack_weapon->o.weapon.sides) - 1;
              }
           else
	    dam +=
	      dice (attack_weapon->o.weapon.dice, attack_weapon->o.weapon.sides);
	}
      else
	{
	  sprintf (buf, "Ineffective weapon, vnum %d on mob %d room %d\n",
		   attack_weapon->nVirtual,
		   src->mob ? src->mob->nVirtual : -1, src->in_room);
	  system_log (buf, true);
	}

      for (af = attack_weapon->xaffected; af; af = af->next)
	{
	  if (af->a.spell.location == APPLY_DAMROLL)
	    dam += af->a.spell.modifier;
	}

      dam += attack_weapon->o.od.value[5];  

    }

  /* For NPCs with no weapons, add natural attack */

  else if (IS_NPC (src))
    {

      if (src->mob->damnodice * src->mob->damsizedice < 8 && shock)
	dam += dice (2, 4);
      else
	dam += dice (src->mob->damnodice, src->mob->damsizedice);
    }

  /* For bare handed PCs */

  else if (shock)
    dam += dice (2, 4);
  else
    dam += number (0, 1);

  // Chill ignores armour.

  if(!attack_weapon && src->nat_attack_type == 6)
    chill = true;

  /* Mobs will have marmor, which is natural armor */

  if (attack_weapon || !chill)
    dam -= (tar->armor);

  /* Subtract the armor protection at the hit location */

  eq = get_equip (tar, wear_loc1);

  if (eq && eq->obj_flags.type_flag == ITEM_ARMOR)
    if (attack_weapon || !chill)
      dam -= eq->o.armor.armor_value;

  if (attack_weapon && eq)
  {

    // If the weapons effectiveness against the armour type is greater than the protective
    // value of the armour, then cap the damage boost to that of the armour's value, e.g.
    // Your sword does +3 damage against leather, but the leather's AC is 2. It doesn't make
    // sense that your sword is more effective against the leather than if the person was naked,
    // so cap the effectiveness bonus to 2.

    if(weapon_armor_table[attack_weapon->o.weapon.hit_type][eq->o.armor.armor_type] > 
         eq->o.armor.armor_value)
      dam += eq->o.armor.armor_value;   
    else
      dam += weapon_armor_table[attack_weapon->o.weapon.hit_type][eq->o.armor.armor_type];
  }
  else if (!attack_weapon && eq)
    dam += weapon_nat_attack_table[src->nat_attack_type][eq->o.armor.armor_type];

  /* Second layer of armour */

  eq = get_equip (tar, wear_loc2);

  if (eq && eq->obj_flags.type_flag == ITEM_ARMOR)
    if (attack_weapon || !chill)
      dam -= eq->o.armor.armor_value;

  if (attack_weapon && eq)
  {    
    if(weapon_armor_table[attack_weapon->o.weapon.hit_type][eq->o.armor.armor_type] > 
         eq->o.armor.armor_value)
      dam += eq->o.armor.armor_value;   
    else
      dam += weapon_armor_table[attack_weapon->o.weapon.hit_type][eq->o.armor.armor_type];
  }
  else if (!attack_weapon && eq)
    dam += weapon_nat_attack_table[src->nat_attack_type][eq->o.armor.armor_type];

  if(get_second_affect(src, SA_AMBUSH, NULL))
  {
    dam += 1;
  }

  if(GET_STR (src) >= 20)
  {
    if((GET_STR (src) == 20 && number(0,1)) || (GET_STR (src) > 20))
    dam += 1;
  } 

  if(GET_STR (src) >= 22)
  {
    if((GET_STR (src) == 24 && number(0,1)) || (GET_STR (src) > 22))
    dam += 1;
  }  

  /* Multiply by hit location multiplier */

  dam *= (body_tab[body_type][*location].damage_mult * 1.0) /
    (body_tab[body_type][*location].damage_div * 1.0);

  if(GET_STR (src) >= 18)
  {
    if((GET_STR (src) == 17 && number(0,1)) || (GET_STR (src) > 17))
    dam += 1;
  } 

  if(GET_STR (src) >= 15)
  {
    if((GET_STR (src) == 16 && number(0,1)) || (GET_STR (src) == 15 && number(0,2)) || (GET_STR (src) > 16))
    dam += 1;
  }

  if (get_second_affect (src, SA_FEINT, NULL))
    dam += 1;

  /* Multiply in critical strike bonus */

  if (off_result == RESULT_HIT
      || (off_result == RESULT_HIT
	  && (!attack_weapon && src->nat_attack_type == 0)))
    dam *= 1;
  else if (off_result == RESULT_HIT1)
    dam *= 1.3;
  else if (off_result == RESULT_HIT2)
    dam *= 1.5;
  else if (off_result == RESULT_HIT3
	   || (off_result == RESULT_HIT && !attack_weapon))
    {
      dam += 2;
      dam *= 1.7;
    }
  else if (off_result == RESULT_HIT4)
    {
      dam += 3;
      dam *= 2;
    }
  else
    {
      *damage = 0;
      return;
    }

  if(GET_STR (src) >= 13)
  {
    if((GET_STR (src) == 13 && number(0,1)) || (GET_STR (src) > 13))
    dam += 1;
  }

  if(GET_STR (src) >= 24)
  {
    if((GET_STR (src) == 24 && number(0,1)) || (GET_STR (src) > 24))
    dam += 1;
  }   

  /* Subtract/add spell offsets */

  if ((af = get_affect (tar, MAGIC_AFFECT_ARMOR)))
    if (attack_weapon || !shock)
      dam -= af->a.spell.modifier;

  /* Reduce damage by SANCTUARY or BLESS.  Note:  Not cumulative */

  if (get_affect (tar, MAGIC_AFFECT_CURSE)
      && !get_affect (tar, MAGIC_AFFECT_BLESS))
    dam += dam / 4 + 1;

  else if (get_affect (tar, MAGIC_AFFECT_BLESS) &&
	   !get_affect (tar, MAGIC_AFFECT_CURSE))
    dam = dam * 3 / 4;

  if (attack_weapon || src->nat_attack_type > 0)
    dam *= COMBAT_BRUTALITY;

  dam = (int) dam;

  if (dam <= 0)
    dam = number (0, 1);

  *damage = (int)dam;
}

int
weaken (CHAR_DATA * victim, uint16 hp_penalty, uint16 mp_penalty,
	char *log_msg)
{
  char buf[MAX_STRING_LENGTH];

  if (hp_penalty == 0 && mp_penalty == 0)
    return 0;

  if (log_msg)
    {
      sprintf (buf, "%s (%dh/%dm)", log_msg, hp_penalty, mp_penalty);
      add_combat_log (victim, buf);
    }

  /* if ( immortal && not an npc) */
  if (!IS_MORTAL (victim) && !IS_NPC (victim))
    return 0;

  if (GET_MOVE (victim) > mp_penalty)
    GET_MOVE (victim) -= mp_penalty;
  else
    GET_MOVE (victim) = 0;

  if (GET_POS (victim) == POSITION_DEAD)
    {

      if (mp_penalty)
	{
	  act ("$n dies of exhaustion!", false, victim, 0, 0, TO_ROOM);
	  add_combat_log (victim, "Death by exhaustion");
	}

      die (victim);

      return 1;
    }

  return 0;
}

void
spell_defenses (CHAR_DATA * defender, CHAR_DATA * target)
{
}

void
subdue_resync (CHAR_DATA * ch)
{
  CHAR_DATA *tch;

  /* ch needs someone else to subdue */

  if (ch->fighting)
    stop_fighting (ch);

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {

      if (tch->fighting == ch)
	{
	  set_fighting (ch, tch);

	  act ("You turn to confront $N.", false, ch, 0, tch, TO_CHAR);
	  act ("$n turns to confront $N.", false, ch, 0, tch, TO_ROOM);
	  act ("$N turns to confront you.", false, tch, 0, ch, TO_CHAR);

	  if (!IS_SET (tch->flags, FLAG_SUBDUING))
	    ch->flags &= ~FLAG_SUBDUING;
	  break;
	}
    }

  if (!ch->fighting)
    send_to_char ("You stop fighting.\n\r", ch);
}

void
remove_subduer (CHAR_DATA * ch)
{
  CHAR_DATA *tch;
  int was_in_room;

  for (tch = ch->room->people; tch == ch;)
    tch = tch->next_in_room;

  was_in_room = ch->in_room;

  char_from_room (ch);

  for (; tch; tch = tch->next_in_room)
    if (tch->fighting == ch)
      subdue_resync (tch);

  char_to_room (ch, was_in_room);
}

void
subdue_char (CHAR_DATA * ch, CHAR_DATA * victim)
{
  int victim_was_room;
  OBJ_DATA *obj;
  AFFECTED_TYPE *af;

  if (ch->fighting)
    stop_fighting (ch);

  if (victim->fighting)
    stop_fighting (victim);

  ch->flags &= ~(FLAG_SUBDUING | FLAG_SUBDUEE);
  victim->flags &= ~(FLAG_SUBDUING | FLAG_SUBDUER);

  ch->flags |= FLAG_SUBDUER;
  victim->flags |= FLAG_SUBDUEE;

  clear_moves (victim);
  clear_current_move (victim);

  ch->subdue = victim;
  victim->subdue = ch;

  if (GET_POS (victim) < POSITION_STANDING)
    {
      act ("You fall upon your foe and get $M into a firm headlock.", false,
	   ch, 0, victim, TO_CHAR);
      act ("$N falls upon you and gets you into a firm headlock.", false,
	   victim, 0, ch, TO_CHAR);
      act ("$n falls upon $N.", false, ch, 0, victim, TO_NOTVICT);
      act ("You haul $N to $S feet.", false, ch, 0, victim, TO_CHAR);
      act ("$N hauls you to your feet.", false, victim, 0, ch, TO_CHAR);
      act ("$e hauls $M to $S feet in a firm headlock.", false, ch, 0, victim,
	   TO_NOTVICT);
      GET_POS (victim) = POSITION_STANDING;
    }
  else
    {
      act ("You grapple $N and lock $M firmly in your grasp.", false, ch, 0,
	   victim, TO_CHAR);
      act ("$N grapples you and locks you firmly in $S grasp!", false, victim,
	   0, ch, TO_CHAR);
      act ("$n grapples $N and locks $M firmly in $s grasp.", false, ch, 0,
	   victim, TO_NOTVICT);
    }
  /* Nobody is trying to subdue ch.  Lets stop or redirect subdue
     attempts against victim */

  victim_was_room = victim->in_room;

  char_from_room (victim);

  remove_subduer (ch);

  char_to_room (victim, victim_was_room);

  remove_subduer (victim);

  if (ch->mob && IS_SET (ch->act, ACT_ENFORCER) && is_hooded (victim))
    {

      if ((obj = get_equip (victim, WEAR_ABOUT)))
	{
	  obj_to_char (unequip_char (victim, WEAR_ABOUT), victim);
	  act ("$n removes your $p.", false, ch, obj, victim, TO_VICT);
	  act ("$n removes $N's $p.", false, ch, obj, victim, TO_NOTVICT);
	  act ("You remove $N's $p.", false, ch, obj, victim, TO_CHAR);
	}

      if ((obj = get_equip (victim, WEAR_HEAD)))
	{
	  obj_to_char (unequip_char (victim, WEAR_HEAD), victim);
	  act ("$n removes your $p.", false, ch, obj, victim, TO_VICT);
	  act ("$n removes $N's $p.", false, ch, obj, victim, TO_NOTVICT);
	  act ("You remove $N's $p.", false, ch, obj, victim, TO_CHAR);
	}

      if ((af = get_affect (victim, MAGIC_CRIM_HOODED + ch->room->zone)))
	add_criminal_time (victim, ch->room->zone, af->a.spell.modifier);
    }
}

/* Unused function */
int
subdue_attempt (CHAR_DATA * ch, CHAR_DATA * target)
{
/*  

SKILL_SUBDUE is undefined

  int help_skills = 0;
  int foe_skills = 0;
  CHAR_DATA *foe = NULL;
  CHAR_DATA *tch;

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {

      if (!tch->fighting)
	continue;

      if (tch->fighting == ch->fighting)
	help_skills += tch->skills[SKILL_SUBDUE];

      if (tch->fighting == ch && ch->fighting != tch)
	{
	  foe = tch;
	  foe_skills += tch->skills[SKILL_SUBDUE];
	}
    }

  if (number (1, 100) < foe_skills)
    {
      act ("$N nearly pins you!", false, ch, 0, foe, TO_CHAR);
      return 0;
    }

  if (number (1, 100) > help_skills)
    return 0;*/

  return 1;
}

void
perform_violence (void)
{
  CHAR_DATA *ch;
  CHAR_DATA *new_combat_list = NULL;

  for (ch = combat_list; ch; ch = combat_next_dude)
    {

      if (ch->next_fighting
	  && (!ch->next_fighting->fighting || !ch->next_fighting->room))
	ch->next_fighting = ch->next_fighting->next_fighting;

      combat_next_dude = ch->next_fighting;

      if (!ch->fighting || !ch->fighting->room)
	continue;

      if (get_affect (ch, MAGIC_HIDDEN))
	{
	  remove_affect_type (ch, MAGIC_HIDDEN);
	  act ("$n reveals $mself.", true, ch, 0, 0, TO_ROOM);
	}

      if (IS_NPC (ch) && !IS_SET (ch->flags, FLAG_FLEE)
	  && (IS_SET (ch->flags, FLAG_AUTOFLEE) || morale_broken (ch)))
	{
	  ch->speed = 4;
	  do_flee (ch, "", 0);
	  add_threat (ch, ch->fighting, 5);
	  continue;
	}

      if (!ch->fighting)
	{
	  sigsegv (SIGSEGV);
	}

      /* Remove delays from both hands */

      ch->primary_delay -= pulse_violence;
      ch->secondary_delay -= pulse_violence;

      if (ch->primary_delay < 0)
	ch->primary_delay = 0;

      if (ch->secondary_delay < 0)
	ch->secondary_delay = 0;

      /* Stop fighting if player is physically incapable */

      if (!AWAKE (ch) ||
	  ch->in_room != ch->fighting->in_room ||
	  get_affect (ch, MAGIC_AFFECT_PARALYSIS))
	{
	  stop_fighting (ch);
	  continue;
	}

      /* No combat if there are delays */

      if (ch->primary_delay && ch->secondary_delay)
	continue;

      if (GET_POS (ch) == STAND)
	GET_POS (ch) = FIGHT;

      if (GET_POS (ch) != FIGHT)
	continue;

      if (ch->mount && !ch->mount->fighting)
	set_fighting (ch->mount, ch->fighting);

      hit_char (ch, ch->fighting, 0);

      if (combat_next_dude && combat_next_dude->deleted)
	system_log ("Loss of combat_next_dude in fight.c: perform_violence.",
		    true);
    }

  /* Reverse the combat list */

  new_combat_list = combat_list;

  if (combat_list)
    {
      combat_list = combat_list->next_fighting;
      new_combat_list->next_fighting = NULL;
    }

  while (combat_list)
    {

      ch = combat_list;
      combat_list = combat_list->next_fighting;

      ch->next_fighting = new_combat_list;
      new_combat_list = ch;
    }

  combat_list = new_combat_list;
}

void
do_stop (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch;
  AFFECTED_TYPE *af;

  if (is_mounted (ch) && ch->mount->moves)
    ch = ch->mount;

  if (ch->moves)
    {

      clear_moves (ch);
      clear_current_move (ch);

      send_to_char ("Movement commands cancelled.\n", ch);

      if (is_mounted (ch))
	send_to_char ("Movement commands cancelled.\n", ch->mount);

      return;
    }

  if ((af = is_crafting (ch)))
    {
      act ("$n stops doing $s craft.", false, ch, 0, 0, TO_ROOM);
      send_to_char ("You stop doing your craft.\n", ch);
      af->a.craft->timer = 0;
      return;
    }

  if ((af = get_affect (ch, MAGIC_TOLL)))
    {

      if (af->a.toll.room_num == ch->in_room)
	{
	  stop_tolls (ch);
	  return;
	}

      /* Toll affect should have been there...continue with stop */

      stop_tolls (ch);
    }

  if (clear_current_move (ch))
    return;

  if (ch->delay || ch->aim)
    {
      break_delay (ch);
      return;
    }

  if (GET_TRUST (ch))
    {
      for (tch = ch->room->people; tch; tch = tch->next_in_room)
	{
	  if (tch->fighting)
	    {
	      if (tch != ch)
		{
		  act ("You immediately obey $N's command to stop fighting!",
		       true, tch, 0, ch, TO_CHAR);
		  act ("$N obeys.", false, ch, 0, tch, TO_CHAR);
		}
	      forget (tch, tch->fighting);
	      stop_fighting (tch);
	    }
	  if (IS_NPC (tch))
	    {
	      tch->attackers = NULL;
	      tch->threats = NULL;
	      tch->act &= ~ACT_AGGRESSIVE;
	    }
	}
      send_to_char ("All combat in the room has been stopped.\n", ch);
      return;
    }

  if (!ch->fighting)
    {
      send_to_char ("You're not fighting anyone.\n\r", ch);
      return;
    }

  if (ch->fighting->fighting != ch || GET_FLAG (ch->fighting, FLAG_FLEE))
    {
      send_to_char ("You stop fighting.\n\r", ch);
      stop_fighting (ch);
      return;
    }

  ch->act |= PLR_STOP;	/* Same as ACT_STOP */

  if (IS_SET (ch->fighting->act, PLR_STOP)
      || GET_FLAG (ch->fighting, FLAG_PACIFIST))
    {

      send_to_char ("Your opponent agrees.\n\r", ch);

      act ("Both you and $N stop fighting.\n\r",
	   false, ch->fighting, 0, ch, TO_CHAR);

      if (IS_NPC (ch->fighting))
	{
	  remove_threat (ch->fighting, ch);
	  remove_attacker (ch->fighting, ch);
	}
      if (IS_NPC (ch))
	{
	  remove_threat (ch, ch->fighting);
	  remove_attacker (ch, ch->fighting);
	}

      ch->act &= ~PLR_STOP;
      ch->fighting->act &= ~PLR_STOP;

      if (ch->fighting->fighting)
	stop_fighting (ch->fighting);

      if (ch->fighting)
	stop_fighting (ch);

      return;
    }

  send_to_char ("You motion for a truce to your opponent.\n\r", ch);
  act ("$N motions for a truce.", false, ch->fighting, 0, ch, TO_CHAR);
  act ("$N motions for a truce with $n.",
       false, ch->fighting, 0, ch, TO_ROOM);
}

void
do_release (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  CHAR_DATA *target;

  if (!IS_SUBDUER (ch))
    {

      if (!IS_SET (ch->flags, FLAG_SUBDUER))
	{
	  send_to_char ("You have no prisoner in tow.", ch);
	  return;
	}

      ch->flags &= ~FLAG_SUBDUER;

      if (ch->subdue &&
	  is_he_somewhere (ch->subdue) && ch->subdue->subdue == ch)
	{
	  ch->subdue->flags &= ~(FLAG_SUBDUER | FLAG_SUBDUEE);
	  send_to_char ("You are no longer a prisoner.\n\r", ch->subdue);
	  send_to_char ("Ok.\n\r", ch);
	  ch->subdue->subdue = NULL;
	  return;
	}

      ch->subdue = NULL;
      send_to_char ("Alright.\n\r", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (*buf && isname (buf, GET_NAMES (ch->subdue)))
    argument = one_argument (argument, buf);

  if (!*buf)
    {
      release_prisoner (ch, NULL);
      return;
    }

  if (!str_cmp (buf, "to"))
    {

      argument = one_argument (argument, buf);

      if (!*buf)
	{
	  send_to_char ("Who do you want to release your prisoner to?\n\r",
			ch);
	  return;
	}
    }

  if (!(target = get_char_room_vis (ch, buf)))
    {
      send_to_char
	("There is no such person here to receive your prisoner.\n\r", ch);
      return;
    }

  release_prisoner (ch, target);
}

int
release_prisoner (CHAR_DATA * ch, CHAR_DATA * target)
{
  int quiet = 0;

  if (!target)
    {

      if (!IS_SUBDUER (ch))
	{
	  ch->flags &= ~FLAG_SUBDUER;
	  act ("You have no prisoner.", false, ch, 0, 0, TO_CHAR);
	  return 0;
	}

      act ("$N releases you.", false, ch->subdue, 0, ch, TO_CHAR);
      act ("You release $N.", false, ch, 0, ch->subdue, TO_CHAR);
      act ("$n releases $N.", false, ch, 0, ch->subdue, TO_NOTVICT);

      ch->flags &= ~(FLAG_SUBDUER | FLAG_SUBDUEE);

      ch->subdue->subdue = NULL;
      ch->subdue = NULL;

      return 1;
    }

  if (IS_SUBDUER (target))
    {
      act ("$N has $S arms full with another prisoner.",
	   false, ch, 0, target, TO_CHAR);
      return 0;
    }

  if (IS_SUBDUEE (target))
    {
      act ("$N is a prisoner himself!", false, ch, 0, target, TO_CHAR);
      return 0;
    }

  if (GET_POS (target) != STAND)
    {
      act ("$N cannot accept your prisoner at the moment.",
	   false, ch, 0, target, TO_CHAR);
      return 0;
    }

  target->subdue = ch->subdue;
  target->subdue->subdue = target;
  ch->subdue = NULL;

  target->flags |= FLAG_SUBDUER;
  ch->flags &= ~FLAG_SUBDUER;

  act ("You release your prisoner to $N.", false, ch, 0, target, TO_CHAR);
  act ("$N releases his prisoner to you.", false, target, 0, ch, TO_CHAR);

  if (IS_SET (target->subdue->act, PLR_QUIET))
    quiet = 1;
  else
    target->subdue->act |= PLR_QUIET;

  act ("$n releases his prisoner to $N.", false, ch, 0, target, TO_NOTVICT);

  if (!quiet)
    target->subdue->act &= ~PLR_QUIET;

  act ("You are tossed into the arms of $N.", false, target->subdue, 0,
       target, TO_CHAR);

  trigger (ch, "", TRIG_PRISONER);

  return 1;
}

/*int retaliate (CHAR_DATA *ch, CHAR_DATA *subject)
{
	if (!IS_NPC(ch))
        return;

    if (IS_SET(ch->act, ACT_WIMPY)) {
        s
	    return 0;

    ;
}*/

int
flee_room (CHAR_DATA * ch)
{
  ROOM_DATA *room_exit;
  ROOM_DATA *room;
  int room_exit_virt;
  int exit_tab[6];
  int zone;
  int num_exits = 0;
  int to_exit;
  int i;
  char log_msg[MAX_STRING_LENGTH];

  room = vtor (ch->in_room);
  zone = room->zone;

  if (GET_POS (ch) == POSITION_FIGHTING)
    {
      do_flee (ch, "", 0);
      return 0;
    }

  if (GET_POS (ch) < POSITION_RESTING)
    return 0;

  if (GET_POS (ch) < POSITION_STANDING)
    do_stand (ch, "", 0);

  if (ch->desc && ch->desc->original)
    return 0;

  for (i = 0; i <= LAST_DIR; i++)
    {

      if (!CAN_GO (ch, i))
	continue;

      room_exit = vtor (EXIT (ch, i)->to_room);

      if (!room_exit)
	{
	  sprintf (log_msg, "ERROR:  Room %d, dir %d doesn't go to %d",
		   ch->in_room, i, EXIT (ch, i)->to_room);
	  system_log (log_msg, true);
	  continue;
	}
      exit_tab[num_exits++] = i;
    }

  if (num_exits == 0)
    return 0;

  to_exit = number (1, num_exits) - 1;

  if (vtor (EXIT (ch, exit_tab[to_exit])->to_room)->nVirtual == ch->last_room)
    to_exit = (to_exit + 1) % num_exits;

  room_exit_virt = vtor (EXIT (ch, exit_tab[to_exit])->to_room)->nVirtual;

  ch->last_room = room_exit_virt;

  do_move (ch, "", exit_tab[to_exit]);

  return 1;
}

void
do_subdue (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  CHAR_DATA *target;

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch))
    {
      send_to_char ("You can't do that in an OOC area.\n", ch);
      return;
    }
/*
	if ( atoi(lookup_race_variable (ch->race, RACE_HUMANOID)) == false ) {
		send_to_char ("I'm afraid you can't do that.", ch);
		return;
	}
*/
  argument = one_argument (argument, buf);

  if (!*buf && !ch->fighting)
    {
      send_to_char ("Subdue whom?\n\r", ch);
      return;
    }

  if (ch->subdue && !is_he_here (ch, ch->subdue, 0))
    ch->subdue = NULL;

  if (ch->fighting && !is_he_here (ch, ch->fighting, 0))
    stop_fighting (ch);

  if (ch->fighting && (!*buf || !isname (buf, GET_NAMES (ch->fighting))))
    target = ch->fighting;

  else if (!(target = get_char_room_vis (ch, buf)))
    {
      send_to_char ("You don't see them here.\n\r", ch);
      return;
    }

  else if (ch == target)
    {
      send_to_char ("Be serious.\n\r", ch);
      return;
    }

  else if (ch->subdue && isname (buf, GET_NAMES (ch->subdue)))
    {
      send_to_char ("You already have them subdued!\n\r", ch);
      return;
    }

  else if (IS_SUBDUEE (target))
    {
      sprintf (buf, "$N has already been subdued by %s.\n",
	       char_short (target->subdue));
      act (buf, false, ch, 0, target, TO_CHAR);
      return;
    }

  else if (ch->subdue)
    {
      send_to_char ("Release your current prisoner, first.\n", ch);
      return;
    }

  if (GET_POS (target) != UNCON && GET_POS (target) != SLEEP)
    {
      send_to_char ("They must be unconscious or asleep, first.\n", ch);
      return;
    }

  if (IS_NPC (ch) && !ch->desc)
    {
      do_sheathe (ch, "", 0);
      do_wear (ch, "shield", 0);
      do_sheathe (ch, "", 0);
      if (ch->right_hand)
	{
	  one_argument (ch->right_hand->name, buf);
	  do_wear (ch, buf, 0);
	}
      if (ch->left_hand)
	{
	  one_argument (ch->left_hand->name, buf);
	  do_wear (ch, buf, 0);
	}
    }

  if (ch->right_hand || ch->left_hand)
    {
      send_to_char ("You'll need both hands free to subdue.\n", ch);
      return;
    }

  if (GET_POS (target) == SLEEP)
    {
      GET_POS (target) = STAND;
      remove_cover(target,0);
      if ( skill_use(ch, SKILL_SNEAK, -20) || (number (1, SKILL_CEILING) <= target->intel*2.5))
	{
	  do_wake (target, "", 0);
	  act
	    ("You are startled awake as $N unsuccessfully attempts to grab you!",
	     false, target, 0, ch, TO_CHAR | _ACT_FORMAT);
	  act
	    ("$n is startled awake as you unsuccessfully attempt to grab $m!",
	     true, target, 0, ch, TO_VICT | _ACT_FORMAT);
	  return;
	}
      else
	{
	  act ("You are awakened abruptly!", false, target, 0, ch,
	       TO_CHAR | _ACT_FORMAT);
	}
    }

  else if (ch->fighting)
    {
      send_to_char ("You abandon your opponent.\n\r", ch);
      act ("$N turns away from you suddenly.", false, ch->fighting, 0, ch,
	   TO_CHAR);
      act ("$n turns away from $N.", false, ch, 0, target, TO_NOTVICT);
    }

  if (IS_SUBDUER (target))
    release_prisoner (target, NULL);

  subdue_char (ch, target);
}

void
do_escape (CHAR_DATA * ch, char *argument, int cmd)
{
  int chance;
  int pressure;
  char log_message[MAX_STRING_LENGTH];

  if (!IS_SUBDUEE (ch))
    {

      if (ch->fighting)
	do_flee (ch, "", 0);
      else
	send_to_char ("You are not currently subdued.\n", ch);

      return;
    }

  if (get_second_affect (ch, SA_ESCAPE, NULL))
    {
      act ("$N is still holding you very tightly.",
	   false, ch, 0, ch->subdue, TO_CHAR);
      return;
    }

  chance = 20 + 2 * (GET_STR (ch) + GET_AGI (ch) - GET_STR (ch->subdue)
		     - GET_DEX (ch->subdue));

  if (chance > number (0, 100))
    {
      act ("You manage to wriggle free of $N's grasp!",
	   false, ch, 0, ch->subdue, TO_CHAR);
      act ("$N wriggles free of your grasp!",
	   false, ch->subdue, 0, ch, TO_CHAR);
      act ("$n escapes from $N's grasp.",
	   true, ch, 0, ch->subdue, TO_NOTVICT);

      ch->subdue->flags &= ~FLAG_SUBDUER;
      ch->flags &= ~FLAG_SUBDUEE;

      ch->subdue->subdue = NULL;
      ch->subdue = NULL;

      return;
    }

  act ("$N chokes you when you attempt to escape.",
       false, ch, 0, ch->subdue, TO_CHAR);
  act ("You choke $N to prevent $M from escaping.",
       false, ch->subdue, 0, ch, TO_CHAR);
  act ("$n tries and fails to escape $N.",
       true, ch, 0, ch->subdue, TO_NOTVICT);

  pressure = GET_STR (ch->subdue) - 10;

  if (pressure > 0)
    {

      sprintf (log_message, "Choked by %s while escaping", ch->tname);

      if (wound_to_char (ch, "neck", number (1, pressure), 9, 0, 0, 0))
	{
	  act ("You have choked $N death!",
	       false, ch->subdue, 0, ch, TO_CHAR);
	  act ("$n has choked $N to death!",
	       false, ch, 0, ch->subdue, TO_NOTVICT);
	  return;
	}
    }

  add_second_affect (SA_ESCAPE, 10, ch, NULL, NULL, 0);
}

void
do_choke (CHAR_DATA * ch, char *argument, int cmd)
{
  int pressure;

  send_to_char ("This command is disabled.\n", ch);
  return;

  if (!IS_SUBDUER (ch))
    {
      send_to_char ("You can only choke someone you have subdued.\n", ch);
      return;
    }

  pressure = GET_STR (ch) - 10;

  if (pressure <= 0)
    {
      act ("You are not strong enough to choke $N.",
	   false, ch, 0, ch->subdue, TO_CHAR);
      act ("$N applies pressure to your neck, but doesn't hurt you.",
	   false, ch->subdue, 0, ch, TO_CHAR);
      return;
    }

  act ("You choke $N.", false, ch, 0, ch->subdue, TO_CHAR);
  act ("$N chokes you!", true, ch->subdue, 0, ch, TO_CHAR);
  act ("$n chokes $N.", false, ch, 0, ch->subdue, TO_NOTVICT);

  if (wound_to_char (ch, "neck", 2 * pressure, 9, 0, 0, 0))
    {
      act ("$n crumples to the ground dead!",
	   false, ch->subdue, 0, ch, TO_CHAR);
      act ("$n chokes $N to death!", false, ch, 0, ch->subdue, TO_NOTVICT);
    }
}

void
fix_offense (CHAR_DATA * ch)
{
  int i = 0;
  int best_weapon_skill = SKILL_BRAWLING;
  int best_combat_style = SKILL_LARGE_WEAPON;
  int best_defense_skill = SKILL_DODGE;

  for (i = SKILL_BRAWLING; i <= SKILL_FLAIL; i++)
    if (ch->skills[i] > ch->skills[best_weapon_skill])
      best_weapon_skill = i;

  for (i = SKILL_LARGE_WEAPON; i <= SKILL_DUAL; i++)
    if (ch->skills[i] > ch->skills[best_combat_style])
      best_combat_style = i;

  if(ch->skills[SKILL_AVERT] > ch->skills[best_defense_skill])
    best_defense_skill = SKILL_AVERT;

  ch->offense = (ch->skills[best_weapon_skill] + ch->skills[best_defense_skill] + ch->skills[best_combat_style]) / 5;
}

void
do_compete (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  CHAR_DATA *src;
  CHAR_DATA *tar;
  int iterations = 100;

  extern rpie::server engine;

  argument = one_argument (argument, buf);

  if (!IS_IMPLEMENTOR (ch))
    {
      send_to_char ("This command is for the implementor only.\n", ch);
      return;
    }

  if (!(src = get_char_room_vis (ch, buf)))
    {
      send_to_char ("First combatant isn't here.\n\r", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (!(tar = get_char_room_vis (ch, buf)))
    {
      send_to_char ("Second combatant isn't here.\n\r", ch);
      return;
    }

  // in-server lag will cause a reboot here
  engine.disable_timer_abort ();

  argument = one_argument (argument, buf);

  if (atoi (buf))
    iterations = atoi (buf);

  compete (ch, src, tar, iterations);

  engine.enable_timer_abort ();
}

struct stats_data
{
  int kills;
  int hits_given;
  int hits_given_2;
  int swipes;
  int swipes_2;
  int strikes;
  int strikes_2;
  int breaks;
} src_stats, tar_stats;

int
compete_strike (CHAR_DATA * src, CHAR_DATA * tar, struct stats_data *stats)
{
  int old_hits;
  int killed = 0;

  old_hits = get_damage_total (tar);

  if (!src->primary_delay &&
      (get_equip (src, WEAR_PRIM) ||
       get_equip (src, WEAR_BOTH) ||
       (!get_equip (src, WEAR_PRIM) &&
	!get_equip (src, WEAR_BOTH) && !get_equip (src, WEAR_SEC))))
    {
      stats->swipes++;
      killed = strike (src, tar, 1, 0);
    }

  if (old_hits != get_damage_total (tar))
    {
      stats->strikes++;
      stats->hits_given += get_damage_total (tar) - old_hits;
    }

  if (killed)
    {
      stats->kills++;
      heal_all_wounds (src);
      heal_all_wounds (tar);
      return 1;
    }

  old_hits = get_damage_total (tar);

  if (!src->secondary_delay && get_equip (src, WEAR_SEC))
    {
      stats->swipes_2++;
      killed = strike (src, tar, 2, 0);
    }

  if (old_hits != get_damage_total (tar))
    {
      stats->strikes_2++;
      stats->hits_given_2 += get_damage_total (tar) - old_hits;
    }

  if (killed)
    {
      stats->kills++;
      heal_all_wounds (src);
      heal_all_wounds (tar);
      return 1;
    }

  return 0;
}

void
compete (CHAR_DATA * ch, CHAR_DATA * src, CHAR_DATA * tar, int iterations)
{
  int tick = 0;
  int i;
  OBJ_DATA *src_prim = NULL;
  OBJ_DATA *tar_prim = NULL;
  OBJ_DATA *src_sec = NULL;
  OBJ_DATA *tar_sec = NULL;
  OBJ_DATA *src_both = NULL;
  OBJ_DATA *tar_both = NULL;
  OBJ_DATA *src_shield = NULL;
  OBJ_DATA *tar_shield = NULL;
  OBJ_DATA *obj;
  char buf[MAX_STRING_LENGTH];
  char name[MAX_STRING_LENGTH];

  extern int tics;

  if (get_equip (src, WEAR_PRIM))
    src_prim = vtoo (get_equip (src, WEAR_PRIM)->nVirtual);

  if (get_equip (tar, WEAR_PRIM))
    tar_prim = vtoo (get_equip (tar, WEAR_PRIM)->nVirtual);

  if (get_equip (src, WEAR_SEC))
    src_sec = vtoo (get_equip (src, WEAR_SEC)->nVirtual);

  if (get_equip (tar, WEAR_SEC))
    tar_sec = vtoo (get_equip (tar, WEAR_SEC)->nVirtual);

  if (get_equip (src, WEAR_BOTH))
    src_both = vtoo (get_equip (src, WEAR_BOTH)->nVirtual);

  if (get_equip (tar, WEAR_BOTH))
    tar_both = vtoo (get_equip (tar, WEAR_BOTH)->nVirtual);

  if (get_equip (src, WEAR_SHIELD))
    src_shield = vtoo (get_equip (src, WEAR_SHIELD)->nVirtual);

  if (get_equip (tar, WEAR_SHIELD))
    tar_shield = vtoo (get_equip (tar, WEAR_SHIELD)->nVirtual);

  src->flags |= FLAG_COMPETE;
  tar->flags |= FLAG_COMPETE;

  src->primary_delay = 0;
  src->secondary_delay = 0;

  src_stats.kills = 0;
  src_stats.hits_given = 0;
  src_stats.hits_given_2 = 0;
  src_stats.swipes = 0;
  src_stats.swipes_2 = 0;
  src_stats.strikes = 0;
  src_stats.strikes_2 = 0;
  src_stats.breaks = 0;

  tar->primary_delay = 0;
  tar->secondary_delay = 0;

  tar_stats.kills = 0;
  tar_stats.hits_given = 0;
  tar_stats.hits_given_2 = 0;
  tar_stats.swipes = 0;
  tar_stats.swipes_2 = 0;
  tar_stats.strikes = 0;
  tar_stats.strikes_2 = 0;
  tar_stats.breaks = 0;

  GET_MOVE (src) = GET_MAX_MOVE (src);
  GET_MOVE (tar) = GET_MAX_MOVE (tar);

  GET_HIT (src) = GET_MAX_HIT (src);
  GET_HIT (tar) = GET_MAX_HIT (tar);

  src->flags &= ~FLAG_KILL;
  tar->flags &= ~FLAG_KILL;

  for (i = 0; i < iterations; i++)
    {

      while (1)
	{

	  if (!(tick % 4))
	    second_affect_update ();

	  if (tick & 1)
	    {
	      if (compete_strike (src, tar, &src_stats))
		break;

	      if (compete_strike (tar, src, &tar_stats))
		break;
	    }
	  else
	    {
	      if (compete_strike (tar, src, &tar_stats))
		break;

	      if (compete_strike (src, tar, &src_stats))
		break;
	    }

	  if (src->primary_delay)
	    src->primary_delay--;

	  if (src->secondary_delay)
	    src->secondary_delay--;

	  if (tar->primary_delay)
	    tar->primary_delay--;

	  if (tar->secondary_delay)
	    tar->secondary_delay--;

	  tick++;
	}

      GET_MOVE (src) = GET_MAX_MOVE (src);
      GET_MOVE (tar) = GET_MAX_MOVE (tar);

      second_affect_update ();
      second_affect_update ();
      second_affect_update ();
      second_affect_update ();
      second_affect_update ();
      second_affect_update ();
      second_affect_update ();
      second_affect_update ();
      second_affect_update ();
      second_affect_update ();

      if (src_prim && !get_equip (src, WEAR_PRIM))
	{
	  src_stats.breaks++;
	  obj = load_object (src_prim->nVirtual);
	  obj_to_char (obj, src);
	  equip_char (src, obj, WEAR_PRIM);
	}

      if (tar_prim && !get_equip (tar, WEAR_PRIM))
	{
	  tar_stats.breaks++;
	  obj = load_object (tar_prim->nVirtual);
	  obj_to_char (obj, tar);
	  equip_char (tar, obj, WEAR_PRIM);
	}

      if (src_sec && !get_equip (src, WEAR_SEC))
	{
	  src_stats.breaks++;
	  obj = load_object (src_sec->nVirtual);
	  obj_to_char (obj, src);
	  equip_char (src, obj, WEAR_SEC);
	}

      if (tar_sec && !get_equip (tar, WEAR_SEC))
	{
	  tar_stats.breaks++;
	  obj = load_object (tar_sec->nVirtual);
	  obj_to_char (obj, tar);
	  equip_char (tar, obj, WEAR_SEC);
	}

      if (src_both && !get_equip (src, WEAR_BOTH))
	{
	  src_stats.breaks++;
	  obj = load_object (src_both->nVirtual);
	  obj_to_char (obj, src);
	  equip_char (src, obj, WEAR_BOTH);
	}

      if (tar_both && !get_equip (tar, WEAR_BOTH))
	{
	  tar_stats.breaks++;
	  obj = load_object (tar_both->nVirtual);
	  obj_to_char (obj, tar);
	  equip_char (tar, obj, WEAR_BOTH);
	}

      if (src_shield && !get_equip (src, WEAR_SHIELD))
	{
	  src_stats.breaks++;
	  obj = load_object (src_shield->nVirtual);
	  obj_to_char (obj, src);
	}

      if (tar_shield && !get_equip (tar, WEAR_SHIELD))
	{
	  tar_stats.breaks++;
	  obj = load_object (tar_shield->nVirtual);
	  obj_to_char (obj, src);
	}

      tics++;
    }

  sprintf (buf,
	   "Name              Kills    DamP    DamS   StrkP   StrkS   MissP   MissS   Breaks\n\r");
  send_to_room_unf (buf, src->in_room);
  sprintf (buf,
	   "===============   ======   =====   =====  ======  ======  ======  ======  ======\n\r");
  send_to_room_unf (buf, src->in_room);

  strncpy (name, src->name, 15);

  name[15] = '\0';

  while (strlen (name) < 15)
    strcat (name, " ");

  sprintf (buf, "%15s   %-6d   %-5d   %-5d  %-6d  %-6d  %-6d  %-6d  %-6d\n\r",
	   name,
	   src_stats.kills,
	   tar_stats.hits_given,
	   tar_stats.hits_given_2,
	   src_stats.strikes,
	   src_stats.strikes_2,
	   src_stats.swipes, src_stats.swipes_2, src_stats.breaks);
  send_to_room_unf (buf, src->in_room);

  strncpy (name, tar->name, 15);

  name[15] = '\0';

  while (strlen (name) < 15)
    strcat (name, " ");

  sprintf (buf, "%15s   %-6d   %-5d   %-5d  %-6d  %-6d  %-6d  %-6d  %-6d\n\r",
	   name,
	   tar_stats.kills,
	   src_stats.hits_given,
	   src_stats.hits_given_2,
	   tar_stats.strikes,
	   tar_stats.strikes_2,
	   tar_stats.swipes, tar_stats.swipes_2, tar_stats.breaks);
  send_to_room_unf (buf, src->in_room);

  src->flags &= ~FLAG_COMPETE;
  tar->flags &= ~FLAG_COMPETE;

/*  for (obj = src->room->contents; obj; obj = next_obj)
    {
      next_obj = obj->next_content;
      extract_obj (obj);
    }*/
}

void
sa_rescue (SECOND_AFFECT * sa)
{
  int result;
  CHAR_DATA *tch, *rescuee;

  if (!is_he_somewhere (sa->ch))
    return;

  rescuee = (CHAR_DATA *) sa->obj;
  result = rescue_attempt (sa->ch, rescuee);

  if (result == 2)		/* can't rescue...stop trying */
    return;

  else if (result == 0)
    {				/* Failed, try again */

      act ("$n makes another failed attempt at rescuing $N.",
	   false, sa->ch, 0, rescuee, TO_NOTVICT | _ACT_BLEED);
      act ("$N tries again, but fails to rescue you.",
	   false, rescuee, 0, sa->ch, TO_CHAR);
      act ("You try again, but fail to rescue $n.",
	   false, rescuee, 0, sa->ch, TO_VICT);

      add_second_affect (SA_RESCUE, 3, sa->ch, sa->obj, NULL, 0);

      return;
    }

  else if (result == 1)		/* Couldn't try...try asap */
    add_second_affect (SA_RESCUE, 1, sa->ch, sa->obj, NULL, 0);

  else if (result == 3)
    {

      for (tch = sa->ch->room->people; tch; tch = tch->next_in_room)
	if (tch->fighting == rescuee)
	  break;

      if (!tch)
	return;

      if (!sa->ch->fighting)
	set_fighting (sa->ch, tch);
      else
	sa->ch->fighting = tch;

      if (tch->fighting)
	{
	  stop_fighting (tch);
	  if (rescuee->fighting)
	    stop_fighting (rescuee);
	}
      set_fighting (tch, sa->ch);

      act ("You draw $N's attention.", false, sa->ch, 0, tch, TO_CHAR);
      act ("$N draws your attention.", false, tch, 0, sa->ch, TO_CHAR);
      act ("$N draws $n's attention.", false, tch, 0, sa->ch, TO_NOTVICT | _ACT_BLEED);
      act ("You stop fighting $N.", false, rescuee, 0, tch, TO_CHAR);

    }
}

	/* Rescue results:
	   0 = failed to rescue (3 sec pause)
	   Normal failure...try again
	   1 = no rescue attempt, not possible yet (1 sec pause)
	   Not in FIGHT or STAND mode
	   Being subdued
	   2 = can't rescue (delete sa)
	   Fighting friend, or friend fighting us
	   Friend not here
	   Friend begin fought
	   3 = success
	 */

int
rescue_attempt (CHAR_DATA * ch, CHAR_DATA * friendPtr)
{
  CHAR_DATA *tch;
  int agi_diff;

  if (GET_POS (ch) < FIGHT)
    return 1;

  if (IS_SET (ch->flags, FLAG_SUBDUING))
    return 1;

  if (!is_he_here (ch, friendPtr, true))
    return 2;

  if (IS_SET (friendPtr->flags, FLAG_SUBDUING))
    return 1;

  if (ch->fighting == friendPtr || friendPtr->fighting == ch)
    return 2;

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    if (tch->fighting == friendPtr)
      break;

  if (!tch)
    return 2;

  if (IS_SET(tch->act, ACT_FLYING))
    return 2;

  agi_diff = (GET_AGI (ch) - GET_AGI (tch)) +
    /* easier to rescue if you are fighting your friend's enemy */
    /* harder to rescue if you are fighting someone else */
    /* otherwise rescue as normal */
    (ch->fighting == tch) ? 8 : (ch->fighting) ? 2 : 5;

  // Much easier to rescue someone if you're in formation.
  if(ch->formation == 1 && friendPtr->formation == 2 && do_group_size(ch) > 9)
    agi_diff += 7;

  if (agi_diff >= number (1, 10))
    return 3;
  else if (number (0, 19) == 0)
    return 3;
  else
    return 0;
}

void
do_rescue (CHAR_DATA * ch, char *argument, int cmd)
{
  int result;
  CHAR_DATA *friendPtr;
  CHAR_DATA *tch;
  SECOND_AFFECT *sa;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  sa = get_second_affect (ch, SA_RESCUE, NULL);

  if (!*buf)
    {

      if (sa)
	{
	  send_to_char ("You stop trying to rescue.\n", ch);
	  remove_second_affect (sa);
	  return;
	}

      send_to_char ("Rescue whom?\n", ch);
      return;
    }

  if (!(friendPtr = get_char_room_vis (ch, buf)))
    {
      send_to_char ("You don't see them here.\n", ch);
      return;
    }

  if (friendPtr == ch)
    {
      send_to_char ("Rescue yourself?\n", ch);
      return;
    }

  if (friendPtr == ch->fighting)
    {
      send_to_char ("You can't rescue your opponent?\n", ch);
      return;
    }

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    if (tch->fighting == friendPtr)
      break;

  if (!tch)
    {
      act ("$N doesn't need rescuing.", false, ch, 0, friendPtr, TO_CHAR);
      return;
    }

  if (IS_SET(tch->act, ACT_FLYING))
  {
    act ("You cannot help $N.", false, ch, 0, friendPtr, TO_CHAR);
    return;
  }
 
  if (ch->balance <= -1)
  {
    act ("You too off-balance to assist $N.", false, ch, 0, friendPtr, TO_CHAR);    
    return;

  }

  if(num_attackers(ch) >= 4)
  {
    act ("You are under attack from too many foes to help $N.", false, ch, 0, friendPtr, TO_CHAR);    
    return;
  }

  if (sa)
    {
      if ((CHAR_DATA *) sa->obj == friendPtr)
	{
	  act ("You're still trying your best to rescue $N.",
	       false, ch, 0, friendPtr, TO_CHAR);
	  return;
	}

      sa->obj = (OBJ_DATA *) friendPtr;
      act ("You will try to rescue $N now.", false, ch, 0, friendPtr,
	   TO_CHAR);
      return;
    }

  result = rescue_attempt (ch, friendPtr);

  if (result == 0)
    {
      act ("You try to draw $N's attention.", false, ch, 0, tch, TO_CHAR);
      act ("$N tries to draw your attention.", false, tch, 0, ch, TO_CHAR);
      act ("$n tries to draw $N's attention.", false, ch, 0, tch, TO_NOTVICT);

      add_second_affect (SA_RESCUE, 3, ch, (OBJ_DATA *) friendPtr, NULL, 0);
    }

  else if (result == 3)
    {
      act ("You draw $N's attention.", false, ch, 0, tch, TO_CHAR);
      act ("$N draws your attention.", false, tch, 0, ch, TO_CHAR);
      act ("$N draws $n's attention.", false, tch, 0, ch, TO_NOTVICT);

      if (GET_POS (ch) != POSITION_DEAD && GET_POS (tch) != POSITION_DEAD)
	criminalize (ch, tch, ch->room->zone, CRIME_KILL);

      if (!tch->fighting)
	set_fighting (tch, ch);
      else
	tch->fighting = ch;
    }

  else if (result == 1)
    {
      act ("You will try to rescue $N when you can.",
	   false, ch, 0, friendPtr, TO_CHAR);

      add_second_affect (SA_RESCUE, 1, ch, (OBJ_DATA *) friendPtr, NULL, 0);
    }

  else
    {				/* better be result == 2, shouldn't be possible */
      printf ("Rescue attempt, result = 2\n");
    }
}

void
do_surrender (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (IS_SWIMMING (ch))
    {
      send_to_char ("You can't do that while swimming!\n", ch);
      return;
    }

  if (IS_SET (ch->flags, FLAG_SUBDUER))
    {
      send_to_char ("Release your prisoner, first.\n", ch);
      return;
    }

  if (IS_SET (ch->room->room_flags, OOC))
    {
      send_to_char ("Sorry, but this command is disabled in OOC areas.\n",
		    ch);
      return;
    }

  if (!*buf && ch->fighting)
    tch = ch->fighting;
  else if (!(tch = get_char_room_vis (ch, buf)))
    {
      send_to_char ("Surrender to whom?\n", ch);
      return;
    }

  if (ch == tch)
    {
      send_to_char ("Surrender to yourself? Hmm...\n", ch);
      return;
    }

  if (tch->fighting && tch->fighting != ch)
    {
      send_to_char
	("They're a little too busy to take prisoners, right now...\n", ch);
      return;
    }

  if (tch->subdue)
    {
      send_to_char ("They are unable to take another prisoner.\n", ch);
      return;
    }

  if (IS_SET (tch->flags, FLAG_KILL))
    {
      send_to_char
	("They don't look particularly interested in taking prisoners...\n",
	 ch);
      return;
    }

  act ("You surrender yourself into $N's custody.", false, ch, 0, tch,
       TO_CHAR | _ACT_FORMAT);
  act ("$n surrenders $mself into your custody.", true, ch, 0, tch,
       TO_VICT | _ACT_FORMAT);
  act ("$n surrenders $mself into $N's custody.", true, ch, 0, tch,
       TO_NOTVICT | _ACT_FORMAT);

  subdue_char (tch, ch);

}

void
do_study (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *crim;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (IS_SET (ch->room->room_flags, OOC))
    {
      send_to_char ("That command is disabled in OOC areas.\n", ch);
      return;
    }

  if (!*buf)
    {
      send_to_char ("Study is used to identify those who are masked.\n", ch);
      return;
    }

  if (!(crim = get_char_room_vis (ch, buf)))
    {
      send_to_char ("Study whom?\n", ch);
      return;
    }

  if (crim == ch)
    {
      send_to_char ("You like the way you look.\n", ch);
      return;
    }

  do_look (ch, buf, 0);

  act ("You stare at $N.", false, ch, 0, crim, TO_CHAR);
  if (IS_NPC (ch) && number (0, 10) < 2)
    {
      act ("$n is staring at $N.", false, ch, 0, crim, TO_NOTVICT);
    }
  act ("$n stares at you.", false, ch, 0, crim, TO_VICT);

  if (!is_hooded (crim))
    return;

  ch->delay_ch = crim;
  ch->delay_type = DEL_STARE;
  ch->delay = 5;
}

void
delayed_study (CHAR_DATA * ch)
{
  OBJ_DATA *obj = NULL;
  AFFECTED_TYPE *af;
  char buf[MAX_STRING_LENGTH];

  if (!is_he_here (ch, ch->delay_ch, true))
    return;

  if (!is_hooded (ch->delay_ch))
    return;

  if (((obj = get_equip (ch->delay_ch, WEAR_FACE))
       && IS_SET (obj->obj_flags.extra_flags, ITEM_MASK))
      || ((ch->intel*2.5 + ch->wil + ch->aur / 2) < number(0,100)))
    {
      act ("The identity of $N remains a mystery.", false, ch, 0,
	   ch->delay_ch, TO_CHAR);
      return;
    }

  sprintf (buf, "You discover that $N is %s.", ch->delay_ch->short_descr);
  act (buf, false, ch, 0, ch->delay_ch, TO_CHAR);

  if (ch->mob && IS_SET (ch->act, ACT_ENFORCER))
    {
      magic_add_affect (ch->delay_ch, MAGIC_STARED, 90, 0, 0, 0, 0);
      if ((is_area_enforcer (ch) && (ch->race == 0 || ch->race == 1))
	  && (ch->in_room / 1000 == 1 || ch->in_room / 1000 == 2
	      || ch->in_room / 1000 == 3))
	{
	  if (ch->delay_ch->race >= 11 && ch->delay_ch->race <= 13
	      && CAN_SEE (ch, ch->delay_ch))
	    {			/* Orc-kin in Gondor? Uh oh... */
	      criminalize (ch->delay_ch, ch, ch->room->zone, 6);
	    }
	}
    }

  if (!get_affect (ch->delay_ch, MAGIC_CRIM_BASE + ch->room->zone))
    return;

  if (is_area_enforcer (ch))
    act ("$E is a criminal!", false, ch, 0, ch->delay_ch, TO_CHAR);

  /* Make criminal hot */

  if (!(af = get_affect (ch->delay_ch, MAGIC_CRIM_HOODED)))
    magic_add_affect (ch->delay_ch, MAGIC_CRIM_HOODED + ch->room->zone,
		      400, 0, 0, 0, 0);
  else
    af->a.spell.duration = 400;

  /* Probably should check to see if this is an enforcer of the zone */

  if (ch->mob && is_area_enforcer (ch))
    enforcer (ch, ch->delay_ch, 1, 1);
}
