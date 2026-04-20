/*------------------------------------------------------------------------\
|  limits.c : Gain Control Module                     www.middle-earth.us | 
|  Copyright (C) 2004, Shadows of Isildur: Traithe                        |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/


#include <cstdlib>
#include <stdio.h>
#include <stdlib.h>
#include <cstring>
#include <stdio.h>
#include <time.h>
#include <ctype.h>
#include <string.h>

#include "structs.h"
#include "account.h"
#include "net_link.h"
#include "protos.h"
#include "utils.h"
#include "decl.h"
#include "math.h"
#include "utility.h"


int
armor_penalty (CHAR_DATA * ch)
{
  /* Amour provides inherent restrictions to movement, not just added weight.
     Measures armour on legs, body, arms and about. Quilted and furs is 1,
     leathers 2, chain 3, and scale 3.5.
     If the total is 7 or greater, then they are lightly restricted,
     9 or greater, restricted,
     10 or greater, heavily restricted.

     Called in determining movement regen rates, movement speed, stam losses
     from movement, climbing tests, swim tests, and so on.
  */

  OBJ_DATA *eq;
  float i = 0;

  eq = get_equip (ch, WEAR_ARMS);

  if (eq && eq->obj_flags.type_flag == ITEM_ARMOR)
  {
    if (eq->o.armor.armor_type == 4)
      i += 3;
    else if(eq->o.armor.armor_type == 3 || eq->o.armor.armor_type == 5)
      i += 3.5;
    else if(eq->o.armor.armor_type == 2)
      i += 2;
    else
      i += 1;    
  }

  eq = get_equip (ch, WEAR_BODY);

  if (eq && eq->obj_flags.type_flag == ITEM_ARMOR)
  {
    if (eq->o.armor.armor_type == 4)
      i += 3;
    else if(eq->o.armor.armor_type == 3 || eq->o.armor.armor_type == 5)
      i += 3.5;
    else if(eq->o.armor.armor_type == 2)
      i += 2;
    else
      i += 1;
  }

  eq = get_equip (ch, WEAR_LEGS);

  if (eq && eq->obj_flags.type_flag == ITEM_ARMOR)
  {
    if (eq->o.armor.armor_type == 4)
      i += 3;
    else if(eq->o.armor.armor_type == 3 || eq->o.armor.armor_type == 5)
      i += 3.5;
    else if(eq->o.armor.armor_type == 2)
      i += 2;
    else
      i += 1;
  }

  eq = get_equip (ch, WEAR_ABOUT);

  if (eq && eq->obj_flags.type_flag == ITEM_ARMOR)
  {
    if (eq->o.armor.armor_type == 4)
      i += 3;
    else if(eq->o.armor.armor_type == 3 || eq->o.armor.armor_type == 5)
      i += 3.5;
    else if(eq->o.armor.armor_type == 2)
      i += 2;
    else
      i += 1;
  }

  if(i >= 10) // heavily armoured - full suit of scale or chain plus fur
    return 4; 
  else if(i >= 9) // full suit of chain or scale, with or without fur
    return 3; 
  else if(i >= 7) // full suit of leather plus fur, or leather and chain + scale
    return 2;
  else if(i >= 4) // full suit of leather sans fur, only chain/scale hauberk, or quilted + fur
    return 1;
  else // incomplete leather, or full quilted.
    return 0;
}

int
move_gain (CHAR_DATA * ch)
{
  int gain;
  int move_rate;
  int moves_gained;
  AFFECTED_TYPE *af;
  AFFECTED_TYPE *taf;

  /* Move_rate is 100 moves per 5 mins  ( * 10000 for granularity ) */

  move_rate = 10000 * GET_CON (ch) * UPDATE_PULSE / (12 * 16);

  gain = move_rate;

  switch (GET_POS (ch))
    {
    case POSITION_SLEEPING:
      gain += gain >> 1;
      break;
    case POSITION_RESTING:
      gain += gain >> 2;
      break;
    case POSITION_SITTING:
      gain += gain >> 3;
      break;
    }

  // Bright weed makes you regen movement faster, the stronger the more so.

  if((af = get_soma_affect(ch, SOMA_PLANT_ALERTNESS)))
  {
    if(af->a.soma.atm_power > 750)
      gain += gain;
    else if(af->a.soma.atm_power > 500)
      gain += gain >> 1;
    else if(af->a.soma.atm_power > 250)
      gain += gain >> 2;
    else
      gain += gain >> 3;
  }

  // If you're relaxing, you don't regen much.

  if(get_soma_affect(ch, SOMA_PLANT_RELAX))
    gain >>= 2;
 
  // If you're hungry, thirsy, or got a busted rib, very slow movement regen.

  if (!ch->hunger || !ch->thirst || get_soma_affect(ch, SOMA_BLUNT_SEVBODY))
    gain >>= 2;

  if (ch->move_points < 0)
    ch->move_points = 0;

  ch->move_points += gain;

  moves_gained = ch->move_points / 10000;

  ch->move_points -= moves_gained * 10000;
  
  if (IS_SET (ch->room->room_flags, OOC) && !IS_SET (ch->flags, FLAG_GUEST))
    moves_gained = 0;

  return moves_gained;
}

int
sleep_needed_in_seconds (CHAR_DATA * ch)
{
  if (!ch->pc)
    return 0;

  return ch->pc->sleep_needed / 100000;
}

void
sleep_credit (CHAR_DATA * ch)
{
  /* We're aiming for straight credit for sleeping, which will
     be 5 seconds per update if UPDATE_PULSE is 5 * 4 (5 seconds) */

  if (!ch->pc || !ch->desc)
    return;

  ch->pc->sleep_needed -= (UPDATE_PULSE / 4) * 100000;

  if (ch->pc->sleep_needed < 0)
    ch->pc->sleep_needed = 0;
}

void
sleep_need (CHAR_DATA * ch)
{
  int need;

  if (!ch->pc)
    return;
  else				/* To enable sleep, get rid of this next return */
    return;

  /* 5 * 60 is RL seconds (5 RL minutes sleep need per mud day) */
  /* sleep needed is called (60 * 60 * 24) / (UPDATE_PULSE/4)
     times a mud day. */
  /* 10000 is stuck in for granularity */

  /* If sleep needed is too long, assume that staff has set it
     this way. */

  if (ch->pc->sleep_needed > 10 * 60 * 100000)
    return;

  need = 5 * 60 * 100000 * (UPDATE_PULSE / 4) / (60 * 60 * 24);

  ch->pc->sleep_needed += need;

  if (ch->pc->sleep_needed > 10 * 60 * 100000)
    ch->pc->sleep_needed = 10 * 60 * 100000;
}

void
check_linkdead (void)
{
  CHAR_DATA *tch;

  for (tch = character_list; tch; tch = tch->next)
    {
      if (tch->deleted)
	continue;
      if (IS_NPC (tch) || tch->desc || !IS_MORTAL (tch) || !tch->room)
	continue;
      if (tch->pc->time_last_activity + PLAYER_DISCONNECT_SECS < mud_time)
	{
	  do_quit (tch, "", 3);
	}
    }
}

void
check_idling (DESCRIPTOR_DATA * d)
{
  /* Unmark people who aren't really idle */

  if (d->idle && d->time_last_activity + PLAYER_IDLE_SECS > mud_time)
    {
      d->idle = 0;
      return;
    }

  if (d->original)
    return;

  if (!d->character
      && d->time_last_activity + DESCRIPTOR_DISCONNECT_SECS < mud_time
      && (d->connected <= CON_ACCOUNT_MENU
	  || d->connected == CON_PENDING_DISC))
    {
      close_socket (d);
      return;
    }

  /* Disconnect those people idle for too long */

  if (d->idle && d->time_last_activity + PLAYER_DISCONNECT_SECS < mud_time)
    {

      if (d->character
	  && (IS_NPC (d->character) || !IS_MORTAL (d->character)))
	return;

      if (d->original)
	return;

      /* Idle PCs in the chargen process */

      if (d->character && !d->character->room)
	{
	  close_socket (d);
	}

      if (d->character && d->connected && d->character->room)
	do_quit (d->character, "", 3);

      return;
    }

  /* Warn people who are just getting to be idle */

  if (!d->idle && d->time_last_activity + PLAYER_IDLE_SECS < mud_time)
    {

      if (d->character)
	{
	  if (d->connected == CON_PLYNG)
	    SEND_TO_Q ("Your thoughts begin to drift. #2(Idle)#0\n\r", d);
	  else
	    SEND_TO_Q
	      ("\n\rYour attention is required to prevent disconnection from the server. #2(Idle)#0\n\r",
	       d);
	}

      d->idle = 1;
    }
}

void
check_idlers ()
{
  DESCRIPTOR_DATA *d, *d_next;

  for (d = descriptor_list; d; d = d_next)
    {
      d_next = d->next;
      check_idling (d);
    }
}

/* Update both PC's & NPC's and objects*/
void
point_update (void)
{
  int cycle_count;
  int roll, i;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  CHAR_DATA *ch;
  CHAR_DATA *tch;
  CHAR_DATA *next_ch = NULL;
  ROOM_DATA *room;
  AFFECTED_TYPE *af;
  WOUND_DATA *wound, *next_wound;
  struct time_info_data healing_time;
  struct time_info_data bled_time;
  struct time_info_data playing_time;

  static int reduceIntox = 0;

  cycle_count = 0;

  mud_time = time (NULL);
  mud_time_str = (char *) asctime (localtime (&mud_time));
  *(mud_time_str + strlen (mud_time_str) - 1) = '\0';

  for (i = 0; i <= 99; i++)
    zone_table[i].player_in_zone = 0;

  for (ch = character_list; ch; ch = next_ch)
    {

      if (!ch)
	continue;

      next_ch = ch->next;

      if (ch->deleted)
	continue;

      if (!ch->room)
	continue;

      if (!IS_NPC (ch) && ch->room)
	zone_table[ch->room->zone].player_in_zone++;

      room = ch->room;

      *ch->short_descr = tolower (*ch->short_descr);

//boats take damage
      if (IS_SET (ch->act, ACT_VEHICLE))
	{
	  if (room->sector_type == SECT_REEF)
	    {

	      for (tch = character_list; tch; tch = tch->next)
		{

		  if (tch->deleted)
		    continue;

		  if (tch->vehicle == ch)
		    send_to_char
		      ("The boat shudders and you hear the sides of the "
		       "ship scrape against\n\r" "something.\n\r", tch);
		}

	      send_to_char ("Ouch!  You're scraping against the reef!\n\r",
			    ch);

	      if (weaken (ch, 10, 0, "boat in REEF"))	/* 10 hits for scraping the reef */
		continue;
	    }
	}

//Sleep
      if (GET_POS (ch) >= SLEEP)
	{

	  if (ch->pc)
	    {

	      if ((af = get_affect (ch, MAGIC_AFFECT_PARALYSIS)))
		{

		  if (GET_POS (ch) == STAND)
		    {
		      send_to_char ("You fall down.\n", ch);
		      act ("$n falls down.", false, ch, 0, 0, TO_ROOM);
		    }

		  else if (GET_POS (ch) == SIT)
		    {
		      send_to_char ("You collapse.\n", ch);
		      act ("$n collapses.", false, ch, 0, 0, TO_ROOM);
		    }

		  GET_POS (ch) = REST;
		}

	      if ((GET_POS (ch) == REST || GET_POS (ch) == SIT) &&
		  sleep_needed_in_seconds (ch) > 7 * 60 &&
		  IS_MORTAL (ch) && !number (0, 8) &&
		  !get_second_affect (ch, SA_STAND, NULL))
		{
		  act ("$n falls alseep.", false, ch, 0, 0, TO_ROOM);
		  GET_POS (ch) = SLEEP;
		}

	      if (sleep_needed_in_seconds (ch) == 300 && IS_MORTAL (ch))
		{
		  send_to_char ("You are feeling drowsy.\n", ch);
		  /* Adding this is 1 sec.  If we don't do this, the
		     drowsy message appears every 5 seconds. */
		  ch->pc->sleep_needed += 100000;
		}

	      else if (sleep_needed_in_seconds (ch) == 350 && IS_MORTAL (ch))
		{
		  send_to_char ("You are feeling sleepy.\n", ch);
		  ch->pc->sleep_needed += 100000;
		}

	      else if (sleep_needed_in_seconds (ch) == 400 && IS_MORTAL (ch))
		{
		  send_to_char ("You are about to fall asleep.\n", ch);
		  ch->pc->sleep_needed += 100000;
		}

	      else if (GET_POS (ch) >= REST && GET_POS (ch) != FIGHT &&
		       sleep_needed_in_seconds (ch) > 6 * 60 &&
		       number (0, 25) == 25 &&
		       IS_MORTAL (ch) && ch->desc && ch->desc->original == ch)
		{
		  if (number (0, 1))
		    act ("You stifle a yawn.", false, ch, 0, 0, TO_CHAR);
		  else
		    {

		      if (get_affect (ch, MAGIC_HIDDEN) && would_reveal (ch))
			{
			  remove_affect_type (ch, MAGIC_HIDDEN);
			  act ("$n reveals $mself.", true, ch, 0, 0, TO_ROOM);
			}

		      act ("You yawn.", false, ch, 0, 0, TO_CHAR);
		      act ("$n yawns.", false, ch, 0, 0, TO_ROOM);
		    }
		}

	      if (GET_POS (ch) == SLEEP)
		sleep_credit (ch);
	      else if (IS_MORTAL (ch))
		sleep_need (ch);
	    }

	  if (GET_POS (ch) == POSITION_SLEEPING && ch->desc &&
	      ch->pc && ch->pc->dreams && !number (0, 5))
	    dream (ch);

          if((af = get_soma_affect(ch, SOMA_PLANT_SLEEP)) && GET_POS(ch) > SLEEP)
          {

            if(number(0,1) && number(0,25) >= GET_CON(ch))
            {
              if(af->a.soma.atm_power > 750)
                GET_MOVE(ch) -= 4;
              else if(af->a.soma.atm_power > 500)
                GET_MOVE(ch) -= 3;
              else if(af->a.soma.atm_power > 250)
                GET_MOVE(ch) -= 2;
              else
                GET_MOVE(ch) -= 1;
            }

            if(GET_MOVE(ch) < 0)
              GET_MOVE(ch) = 0;

          }
	  else if (GET_MOVE (ch) < GET_MAX_MOVE (ch))
          {
	    GET_MOVE (ch) =
	      MIN (GET_MOVE (ch) + move_gain (ch), GET_MAX_MOVE (ch));
          }
	}

// Healing
      if (!ch)
	continue;

      int old_damage = ch->damage;
      int new_damage = 0;
      if (ch->damage)
	{
	  int base = (get_soma_affect (ch, SOMA_PLANT_RELAX)
		      ||ch->skills[SKILL_EMPATHIC_HEAL]) 
	    ? BASE_SPECIAL_HEALING
	    : BASE_PC_HEALING;
	  
	  healing_time = real_time_passed (time (0) - ch->lastregen, 0);
	  if (healing_time.minute >= (base - ch->con / 5))
	    {
	      roll = dice (1, 100);
	      if (GET_POS (ch) == POSITION_SLEEPING)
		roll -= 20;
	      if (GET_POS (ch) == POSITION_RESTING)
		roll -= 10;
	      if (GET_POS (ch) == POSITION_SITTING)
		roll -= 5;
	      if (roll <= (ch->con * 4))
		{
		  if (roll % 5 == 0)
		    ch->damage -= 2;
		  else
		    ch->damage -= 1;
		}
	      ch->lastregen = time (0);
	    }
	  
	}

// Stun recovery
      if (!ch)
	continue;

      if (GET_POS (ch) == POSITION_STUNNED)
	{
	  if ((time (0) - ch->laststuncheck) >= number (15, 20))
	    {
	      ch->laststuncheck = time (0);
	      GET_POS (ch) = REST;
	      send_to_char
		("You shake your head vigorously, recovering from your stun.\n",
		 ch);
	      sprintf (buf,
		       "$n shakes $s head vigorously, seeming to recover.");
	      act (buf, false, ch, 0, 0, TO_ROOM);
	      if (IS_NPC (ch))
		do_stand (ch, "", 0);
	    }
	}

//regain consciousness
      if (!ch)
	continue;

      if (GET_POS (ch) == POSITION_UNCONSCIOUS)
	{
	  healing_time = real_time_passed (time (0) - ch->knockedout, 0);
	  if (healing_time.minute >= 5)
	    {
	      GET_POS (ch) = REST;
	      send_to_char ("Groaning groggily, you regain consciousness.\n",
			    ch);
	      sprintf (buf, "Groaning groggily, $n regains consciousness.");
	      act (buf, false, ch, 0, 0, TO_ROOM);
	      if (IS_NPC (ch))
		do_stand (ch, "", 0);
	    }
	}

//intoxication
      if (!ch)
	continue;

      if (reduceIntox == 3)
	{
	  if (ch->intoxication > 0)
	    if (!--ch->intoxication)
	      ;
	  /*send_to_char ("You are sober.\n", ch); */
	  reduceIntox = 0;

	}
      else
	reduceIntox++;

//Application RPP cost
      if (!ch)
	continue;

      if (!IS_NPC (ch) && ch->pc->app_cost && ch->desc)
	{
	  playing_time =
	    real_time_passed (time (0) - ch->time.logon + ch->time.played, 0);
	  if (playing_time.hour >= 10 && ch->desc->acct)
	    {

	      ch->desc->acct->pay_application_cost (ch->pc->app_cost);
	      ch->pc->app_cost = 0;
	      save_char (ch, true);
	    }
	}

//Remove New Player Flag
      if (!IS_NPC (ch) && IS_SET (ch->plr_flags, NEW_PLAYER_TAG))
	{
	  playing_time =
	    real_time_passed (time (0) - ch->time.logon + ch->time.played, 0);
	  if (playing_time.hour > 12)
	    {
	      ch->plr_flags &= ~NEW_PLAYER_TAG;
	      act
		("You've been playing for over 12 hours, now; the #2(new player)#0 tag on your long description has been removed. Once again, welcome - have fun, and best of luck in your travels!\n",
		 false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
	    }
	}

// Bleeding
      if (!ch)
	continue;

      for (wound = ch->wounds; wound; wound = next_wound)
      {
        if (!ch->wounds)
          break;   // they aren't really wounded
					
	if (ch->delay_type == DEL_BIND_WOUNDS)
	  break;  //they are binding, so no bloodloss
						
	next_wound = wound->next;
	int wound_damage = wound->damage;
	old_damage += wound_damage;
	new_damage += wound_damage;
	healing_time = real_time_passed (time (0) - wound->lasthealed, 0);
	bled_time = real_time_passed (time (0) - wound->lastbled, 0);
    
	if (IS_MORTAL (ch) && bled_time.minute >= BLEEDING_INTERVAL
	    && wound->bleeding)
	{
	  //ch->damage += wound->bleeding;
	  wound->lastbled = time (0);
	  if (wound->bleeding > 0 && wound->bleeding <= 3)
	  {
	    sprintf (buf, "#1Blood continues to seep from a %s %s on your %s.#0",
	      wound->severity, wound->name, expand_wound_loc (wound->location));
									
            sprintf (buf2, "Blood continues to seep from a %s %s on #5%s#0's %s.",
	      wound->severity, wound->name, char_short (ch), expand_wound_loc (wound->location));
	  }
	  else if (wound->bleeding > 3 && wound->bleeding <= 6)
	  {
	    sprintf (buf, "#1Blood flows from a %s %s on your %s.#0",
 	      wound->severity, wound->name, expand_wound_loc (wound->location));
									
	    sprintf (buf2, "Blood flows from a %s %s on #5%s#0's %s.",
	      wound->severity, wound->name, char_short (ch), expand_wound_loc (wound->location));
	  }
	  else if (wound->bleeding > 6 && wound->bleeding <= 9)
	  {
	    sprintf (buf, "#1Blood flows heavily from a %s %s on your %s!#0",
	      wound->severity, wound->name, expand_wound_loc (wound->location));

            sprintf (buf2, "Blood flows heavily from a %s %s on #5%s#0's %s!",
	      wound->severity, wound->name, char_short (ch), expand_wound_loc (wound->location));
	  }
	  else if (wound->bleeding > 9)
	  {
	    sprintf (buf, "#1Blood gushes from a %s %s on your %s!#0",
	      wound->severity, wound->name, expand_wound_loc (wound->location));
							
	    sprintf (buf2, "Blood gushes from a %s %s on #5%s#0's %s!",
	      wound->severity, wound->name, char_short (ch), expand_wound_loc (wound->location));
	  }
								
	  act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT); // You want to see your bleed.
	  act (buf2, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT | _ACT_BLEED);
        
	  if (IS_SET (ch->plr_flags, NEW_PLAYER_TAG))
    	    act ("#6To stop the bleeding before it's too late, type BIND.#0", 
            false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
    						
	  if (general_damage (ch, wound->bleeding))
	    continue;

	}
      			
	int base = (get_soma_affect (ch, SOMA_PLANT_RELAX)
		     || ch->skills[SKILL_EMPATHIC_HEAL]) ? BASE_SPECIAL_HEALING : BASE_PC_HEALING;

	  if (healing_time.minute >= (base + (wound->damage / 3) 
	      - (GET_CON (ch) / 7) - (wound->healerskill / 20)))
	  {
	    wound->lasthealed = time (0);
	    if (GET_POS (ch) != POSITION_FIGHTING 
		&& !IS_SET (ch->room->room_flags, OOC))
	    {
	      new_damage -= wound_damage;

	      // returns false if wound still exists
	      if (!natural_healing_check (ch, wound))
	      {
	        new_damage += wound->damage;
	      }
	    }
	  }
	}

      new_damage += ch->damage; // we add this later, since bloodloss can add
      // NPCs that healed trigger on_health (>NN)
      if (IS_NPC(ch) && new_damage < old_damage && ch->mob->cues)
	{
	  // hook: cue_on_health reflex
	  // We only want ones that have the flag (>NN)
	  typedef std::multimap<mob_cue,std::string>::const_iterator N;
	  std::pair<N,N> range = ch->mob->cues->equal_range (cue_on_health);
	  if (range.first != range.second) 
	    {
	      // Want to only execute the health cue the first time health 
	      // drops below NN
	      int old_health = 100 - (int((old_damage * 100.0) / (ch->max_hit)));
	      int new_health = 100 - (int((new_damage * 100.0) / (ch->max_hit)));
	      
	      for (N n = range.first; n != range.second; n++)
		{
		  std::string cue = n->second;
		  const char *r = cue.c_str();
		  
		  if (!cue.empty () && strncmp(r,"(>",2) == 0)
		    {
		      char *p;
		      int threshold = strtol (r+2, &p, 0);
		      if (new_health > threshold && old_health <= threshold)
			{
			  char reflex[AVG_STRING_LENGTH] = "";
			  strcpy (reflex, p+2);
			  command_interpreter (ch, reflex);
			}
		    }
		}
	    }



	}

      if ((af = get_affect (ch, MAGIC_CRAFT_DELAY)))
	{
	  if (time (0) >= af->a.spell.modifier)
	    {
	      send_to_char
		("#6OOC: Your craft delay timer has expired. You may resume crafting delayed items.#0\n",
		 ch);
	      remove_affect_type (ch, MAGIC_CRAFT_DELAY);
	    }
	}

      if (ch->damage < 0)
	ch->damage = 0;

      if (ch->room && sun_light && IS_SET (ch->affected_by, AFF_SUNLIGHT_PET)
	  && !is_overcast (ch->room))
	{
	  ch->plr_flags |= FLAG_PETRIFIED;
	  die (ch);
	}

    }				/* for */
}

void
hourly_update (void)
{
  int current_time;
  int hours;
  int nomsg = 0;
  CHAR_DATA *ch, *next_ch;
  OBJ_DATA *obj;
  OBJ_DATA *objj;
  OBJ_DATA *next_thing2;
  ROOM_DATA *room;
  ENCHANTMENT_DATA *ench;
  NEGOTIATION_DATA *neg;
  NEGOTIATION_DATA *new_list = NULL;
  NEGOTIATION_DATA *tmp_neg;
  POISON_DATA *poison;
  char your_buf[MAX_STRING_LENGTH];
  char room_buf[MAX_STRING_LENGTH];
  char room_msg_buf[MAX_STRING_LENGTH];

  current_time = time (NULL);

  for (ch = character_list; ch; ch = next_ch)
    {

      next_ch = ch->next;

      if (ch->deleted || !ch->room)
	continue;

      for (ench = ch->enchantments; ench; ench = ench->next)
	{
	  ench->current_hours--;
	  if (ench->current_hours <= 0)
	    remove_enchantment (ch, ench);
	}
      
      if (IS_NPC (ch))
	{
	  // Mighty Morphing
	  if ((ch->morph_time) && ch->morph_time < current_time)
	    {
	      morph_mob (ch);
	    }

	  // Delete Old Barters
	  if (IS_SET (ch->flags, FLAG_KEEPER) && ch->shop)
	    {
	      neg = ch->shop->negotiations;
	      new_list = NULL;
	      
	      while (neg)
		{
		  tmp_neg = neg->next;

		  if (neg->time_when_forgotten <= current_time)
		    {
		      mem_free (neg);
		    }
		  else
		    {
		      neg->next = new_list;
		      new_list = neg;
		    }
		  neg = tmp_neg;
		}
	      ch->shop->negotiations = new_list;
	    }

	  // hook: cue_on_hour reflex
	  if (ch->mob->cues) 
	    {
	      typedef std::multimap<mob_cue,std::string>::const_iterator N;
	      std::pair<N,N> range = ch->mob->cues->equal_range (cue_on_hour);
	      for (N n = range.first; n != range.second; n++)
		{
		  std::string cue = n->second;
		  if (!cue.empty ())
		    {
		      strcpy (your_buf, cue.c_str ());
		      command_interpreter (ch, your_buf);
		    }
		}
	    }
	}
      else 
	{
	  if (!IS_SET (ch->room->room_flags, OOC))
	    hunger_thirst_process (ch);
	}

      trigger (ch, "", TRIG_HOUR);
    }

  for (obj = object_list; obj; obj = obj->next)
    {

      if (obj->deleted)
	continue;

      if(obj->poison)
      {
        for (poison = obj->poison; poison; poison = poison->next)
	{
          if(poison->uses < 1)
            remove_object_poison(obj, obj->poison);
          else
            poison->uses--;
        }
      }

      /* Mob jailbags need to disappear after a time */

      if (obj->nVirtual == VNUM_JAILBAG &&
	  obj->obj_timer && --obj->obj_timer == 0)
	extract_obj (obj);

      else if (IS_SET (obj->obj_flags.extra_flags, ITEM_TIMER)
	       && --obj->obj_timer <= 0)
	{

	  room = (obj->in_room == NOWHERE ? NULL : vtor (obj->in_room));

	  if (obj->carried_by)
	    act ("$p decays in your hands.",
		 false, obj->carried_by, obj, 0, TO_CHAR);

	  else if (room && room->people)
	    {
	      act ("$p gradually decays away.",
		   true, room->people, obj, 0, TO_ROOM);
	      act ("$p gradually decays away.",
		   true, room->people, obj, 0, TO_CHAR);
	    }

	  for (objj = obj->contains; objj; objj = next_thing2)
	    {

	      next_thing2 = objj->next_content;	/* Next in inventory */

	      if (obj->nVirtual == VNUM_CORPSE)
		{
		  objj->obj_flags.extra_flags |= ITEM_TIMER;
		  objj->obj_timer = 12;	/* Stuff from corpses lasts 1 */
		  /* 3 RL hours, unless handled by a PC. */
		}

	      obj_from_obj (&objj, 0);

	      if (obj->in_obj)
		obj_to_obj (objj, obj->in_obj);
	      else if (obj->carried_by)
		obj_to_room (objj, obj->carried_by->in_room);
	      else if (obj->in_room != NOWHERE)
		obj_to_room (objj, obj->in_room);
	      else
		extract_obj (obj);
	    }

	  extract_obj (obj);
	}

      else if (GET_ITEM_TYPE (obj) == ITEM_LIGHT &&
	       obj->o.light.on && obj->o.light.hours)
	{

	  obj->o.light.hours--;

	  hours = obj->o.light.hours;

	  if (!(ch = obj->carried_by))
	    ch = obj->equiped_by;

	  switch (hours)
	    {
	    case 0:
	      strcpy (your_buf, "Your $o burns out.");
	      strcpy (room_buf, "$n's $o burns out.");
	      strcpy (room_msg_buf, "$p burns out.");
	      break;

	    case 1:
	      strcpy (your_buf, "Your $o is just a dim flicker now.");
	      strcpy (room_buf, "$n's $o is just a dim flicker now.");
	      strcpy (room_msg_buf, "$p is just a dim flicker now.");
	      break;

	    case 2:
	      strcpy (your_buf, "Your $o begins to burn low.");
	      strcpy (room_buf, "$n's $o begins to burn low.");
	      strcpy (room_msg_buf, "$p begins to burn low.");
	      break;

	    case 10:
	      strcpy (your_buf, "Your $o sputters.");
	      strcpy (room_buf, "$n's $o sputters.");
	      strcpy (room_msg_buf, "$p sputters.");
	      break;

	    default:
	      nomsg = 1;
	      break;
	    }

	  if (hours == 0 ||
	      (!is_name_in_list ("candle", obj->name) && !nomsg))
	    {

	      if (ch)
		{
		  act (your_buf, false, ch, obj, 0, TO_CHAR);
		  act (room_buf, false, ch, obj, 0, TO_ROOM);
		}

	      if (obj->in_room &&
		  (room = vtor (obj->in_room)) && room->people)
		{
		  act (room_msg_buf, false, room->people, obj, 0, TO_ROOM);
		  act (room_msg_buf, false, room->people, obj, 0, TO_CHAR);
		}
	    }

	  if (obj->o.light.hours > 0)
	    continue;

	  if (is_name_in_list ("candle", obj->name))
	    extract_obj (obj);
	}

      if ((obj->morphTime) && obj->morphTime < current_time)
	morph_obj (obj);
    }
}

int
remove_room_affect (ROOM_DATA * room, int type)
{
  AFFECTED_TYPE *af;
  AFFECTED_TYPE *free_af;

  if (!room->affects)
    return 0;

  if (room->affects->type == type)
    {
      free_af = room->affects;
      room->affects = free_af->next;
      mem_free (free_af);
      return 1;
    }

  for (af = room->affects; af->next; af = af->next)
    if (af->next->type == type)
      {
	free_af = af->next;
	af->next = free_af->next;
	mem_free (free_af);
	return 1;
      }

  return 0;
}

void
room_affect_wearoff (ROOM_DATA * room, int type)
{
  if (!remove_room_affect (room, type))
    return;

  switch (type)
    {
    case MAGIC_ROOM_CALM:
      if (room)
	send_to_room
	  ("Slowly, the sense of peace dissipates, and things return to normal.\n",
	   room->nVirtual);
      else
	send_to_all ("The sense of peace everwhere fades.\n\r");
      break;

    case MAGIC_ROOM_LIGHT:
      if (room)
	send_to_room ("The unnatural light emanations fade.\n\r",
		      room->nVirtual);
      else
	send_to_all
	  ("The unnatural light emanations fade from the land.\n\r");
      break;

    case MAGIC_ROOM_DARK:
      if (room)
	send_to_room ("The unnatural darkness fades away.\n\r",
		      room->nVirtual);
      else
	send_to_all ("The unnatural darkness fades from the land.\n\r");
      break;

    case MAGIC_WORLD_SOLAR_FLARE:
      if (room)
	send_to_room ("The localized solar flare has ended.\n\r",
		      room->nVirtual);
      else
	send_outside ("The ball of flame in the sky slowly dies out.\n\r");
      break;
    }
}

void
room_update (void)
{
  /* Expire affects on rooms */
/*
	for ( room = full_room_list; room; room = room->lnext ) {
		for ( room_affect = room->affects;
			  room_affect;
			  room_affect = next_room_affect ) {

			next_room_affect = room_affect->next;

			if ( room_affect->type >= MAGIC_SMELL_FIRST &&
				 room_affect->type <= MAGIC_SMELL_LAST )
				continue;

			if ( room_affect->type == MAGIC_ROOM_FIGHT_NOISE )
				continue;

			if ( room_affect->a.room.duration > 0 )
				room_affect->a.room.duration--;

			if ( !room_affect->a.room.duration )
				room_affect_wearoff (room, room_affect->type);
		}
	}
*/
}

/* returns the situation-affected skill level */
int
skill_level (CHAR_DATA * ch, int skill, int diff_mod)
{
  int skill_lev;
  OBJ_DATA *obj;
  AFFECTED_TYPE *af;

  skill_lev = ch->skills[skill];
  skill_lev -= diff_mod;

  if (ch->stun > 0)
    skill_lev -= ch->stun * 2;

  if ((af = get_affect (ch, MAGIC_AFFECT_CURSE)))
    skill_lev -= af->a.spell.modifier;

  if ((ch->race == 24 || ch->race == 25) && sun_light
      && ch->room->sector_type != SECT_INSIDE)
    {
      if (weather_info[ch->room->zone].clouds == CLEAR_SKY)
	skill_lev -= 40;
      else if (weather_info[ch->room->zone].clouds == LIGHT_CLOUDS)
	skill_lev -= 30;
      else if (weather_info[ch->room->zone].clouds == HEAVY_CLOUDS)
	skill_lev -= 15;
      else if (weather_info[ch->room->zone].clouds == OVERCAST)
	skill_lev -= 5;
    }

  for (obj = ch->equip; obj; obj = obj->next_content)
    {
      /* Why skip weapons?
         if ( GET_ITEM_TYPE (obj) == ITEM_WEAPON || GET_ITEM_TYPE (obj) == ITEM_SHIELD )
         continue;
       */
      for (af = obj->xaffected; af; af = af->next)
	{
	  if (af->a.spell.location - 10000 == skill)
	    skill_lev += af->a.spell.modifier;
	}
    }

  if ((obj = ch->right_hand))
    {
      for (af = obj->xaffected; af; af = af->next)
	{
	  if (af->a.spell.location - 10000 == skill)
	    skill_lev += af->a.spell.modifier;
	}
    }

  if ((obj = ch->left_hand))
    {
      for (af = obj->xaffected; af; af = af->next)
	{
	  if (af->a.spell.location - 10000 == skill)
	    skill_lev += af->a.spell.modifier;
	}
    }

  skill_lev = MAX (2, skill_lev);
  skill_lev = MIN (99, skill_lev);

  return skill_lev;
}

// This opens a character with 1 point in that skill, and a big delay.

void
skill_learn (CHAR_DATA *ch, int skill)
{
  if (IS_NPC(ch))
    return;
  if(real_skill(ch, skill))
    return;
  else
    ch->skills[skill] = 1;

  magic_add_affect (ch, MAGIC_SKILL_GAIN_STOP + skill, 200, 0, 0, 0, 0);
}


// Tht.is function is called to determine whether a player has gone over their skill cap or not.

int
skill_max (CHAR_DATA *ch, int skill, int mode)
{
  int i;
  int j= 0;
  int skillmax;

  if(ch->intel > 20)
     skillmax = 1370;
  else if(ch-> intel < 10)
     skillmax = 530;
  else
    skillmax = 600 + (70 * (ch->intel - 10));

  for (i = 0; i <= LAST_SKILL; i++)
  {
    // Skip any skill 10 or under, or their native tongue, or Sindarin, or Literacy, or Brawling

    if (ch->skills[i] < 11
       || (i == atoi (lookup_race_variable (ch->race, RACE_NATIVE_TONGUE))) 
       || (skills[i] == skills[SKILL_SPEAK_SINDARIN]) 
       || (skills[i] == skills[SKILL_LITERACY]) 
       || (skills[i] == skills[SKILL_BRAWLING]))
        ;
    else
    j += ch->skills[i];
  }
  
  if(mode == 0)
  {
    if((j + skill) >= skillmax)
      return 1;
    else
      return 0;
  }
  else
  {
    return j;
  }
}

int
skill_use (CHAR_DATA * ch, int skill, int diff_mod)
{
  double roll;
  int lv;
  int cap;
  int min, max;
  int i, j = 0;
  bool gain = false;
  char buf[MAX_STRING_LENGTH] = { '\0' };

  if (!real_skill (ch, skill))
    return 0;

  if(get_soma_affect(ch, SOMA_SPIDER_PARALYZE))
    diff_mod += 5;

  if(get_soma_affect(ch, SOMA_PLANT_BLINDNESS) 
     && get_soma_affect(ch, SOMA_PLANT_BLINDNESS)->a.soma.atm_power >= 500)
    diff_mod += 5;

  lv = calc_lookup (ch, REG_LV, skill);
  cap = calc_lookup (ch, REG_CAP, skill);

  cap = MIN (cap, SKILL_CEILING);

  roll = number (1, SKILL_CEILING);

  if (!AWAKE (ch))
    return 0;

  if (roll <= skill_level (ch, skill, diff_mod))
    return 1;

  if (IS_NPC (ch))
    {
      if (ch->skills[skill] < cap && lv >= number (1, SKILL_CEILING))
	ch->skills[skill]++;
    }
  else if (ch->pc->skills[skill] < cap && lv >= number (1, SKILL_CEILING))
    {

      if (!ch->desc || ch->desc->idle)	/* No skill gain idle/discon */
	return 0;

      if (IS_SET (ch->room->room_flags, OOC))	/* No skill gain in OOC areas. */
	return 0;

      if (!get_affect (ch, MAGIC_SKILL_GAIN_STOP + skill)
	  && !get_affect (ch, MAGIC_FLAG_NOGAIN + skill))
	{

          // If players haven't reached their skill cap, then give them the point.
          // If they have, select a skill at random to deduct a point from, and
          // then give them their point but not from their native tongue, 
          // skills under 11, skills being focused, the skill in question, and Sindarin.
          // If there isn't any skills to select (e.g. they've focused everything),
          // then they don't get a skill-raise at all.

          if (!skill_max(ch, 0, 0))
          {
            ch->skills[skill]++;
	    ch->pc->skills[skill]++;
            min = 40;

	    max = 40 + ch->skills[skill] + number (1, 60);
	    max = MIN (180, max);

	    magic_add_affect (ch, MAGIC_SKILL_GAIN_STOP + skill,
		    	      number (min, max), 0, 0, 0, 0);

	    return 2;
          }
          else
          {
            for (i = 1; i <= LAST_SKILL; i++)
            {
              if ((ch->skills[i] < 11) || get_affect (ch, MAGIC_FLAG_FOCUS + i) || (skills[i] == skills[skill]) || (i == atoi (lookup_race_variable (ch->race, RACE_NATIVE_TONGUE))) || (skills[i] == skills[SKILL_SPEAK_SINDARIN]))
              ;
              else                
              gain = true;
            }
 
            if(gain)
            {
              while(j == 0)
              {
              i = number(0, LAST_SKILL);

              if (real_skill (ch, i) && (ch->skills[i] >= 11) 
                  && !get_affect (ch, MAGIC_FLAG_FOCUS + i) && (skills[i] != skills[skill]) && (i != atoi (lookup_race_variable (ch->race, RACE_NATIVE_TONGUE))) && (skills[i] != skills[SKILL_SPEAK_SINDARIN]))
                j = i;
              }

              ch->skills[j]--;
    	      ch->pc->skills[j]--;

              ch->skills[skill]++;
    	      ch->pc->skills[skill]++;
              min = 60;

              max = 60 + ch->skills[skill] + number (1, 40);
	      max = MIN (200, max);

              if (ch->skills[skill] < 10)
              {
                min = 200;
                max = 300;
              }

	      magic_add_affect (ch, MAGIC_SKILL_GAIN_STOP + skill,
		      	        number (min, max), 0, 0, 0, 0);

              sprintf(buf, "You feel your %s skill is improved, at the expense of your %s skill.\n", skills[skill], skills[j]);
              send_to_char(buf, ch);

	      return 2;
            }
            else
            {
              sprintf(buf, "You feel that you were close to increasing your %s skill, but something was holding you back.\n", skills[skill]);
              send_to_char(buf, ch);
              return 0;
            }
          }
	}
    }

  return 0;
}
