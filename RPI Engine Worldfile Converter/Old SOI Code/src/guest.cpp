/*------------------------------------------------------------------------\
|  guest.c : guest login generation routines          www.middle-earth.us | 
|  Copyright (C) 2005, Shadows of Isildur: Traithe                        |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/

#include <stdlib.h>
#include <ctype.h>

#include "structs.h"
#include "net_link.h"
#include "account.h"
#include "protos.h"
#include "utils.h"


void
create_guest_avatar (DESCRIPTOR_DATA * d, char *argument)
{
  CHAR_DATA *ch = NULL;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char account_name[MAX_STRING_LENGTH];
  char tname_buf[MAX_STRING_LENGTH];
  int roll, i = 1;
  extern int finished_booting;
  extern int guest_conns;

  if (!d)
    return;

  *buf = '\0';
  *buf2 = '\0';
  *account_name = '\0';
  *tname_buf = '\0';

  if (d->character)
    extract_char (d->character);

  sprintf (tname_buf, "%s-Guest", d->acct->name.c_str ());

  d->character = new_char (1);
  clear_char (d->character);

  d->character->deleted = 0;

  d->original = NULL;

  ch = d->character;

  ch->color = d->acct->color;

  d->character->pc->create_state = STATE_APPROVED;
  d->prompt_mode = 1;
  d->character->desc = d;
  d->character->pc->owner = d;
  d->character->pc->load_count = 1;

  roll = number (1, 11);

  ch->race = 2
;

  d->character->race = ch->race;

  /* Bestow the random traits to the new guest avatar */

  randomize_mobile (ch);

  ch->pc->account_name = str_dup (d->acct->name.c_str ());

  if (is_guide (d->acct->name.c_str ()))
    ch->pc->is_guide = true;

  ch->fight_mode = 2;
  ch->clans = add_hash ("");

  /* Address naming issues with our user's new account handle */

  ch->tname = add_hash (tname_buf);

  if (ch->pc->is_guide)
    sprintf (buf2, "%s Guide %s", ch->name, ch->tname);
  else
    sprintf (buf2, "%s %s", ch->name, ch->tname);

  if (ch->name)
    mem_free (ch->name);
  ch->name = add_hash (buf2);

  ch->hit = 100;
  ch->max_hit = 100;
  ch->move = 100;
  ch->max_move = 100;

  d->character->flags |= FLAG_GUEST;

  if (d->character->race >= 0 && d->character->race <= 16)
    {
      for (i = 0; i <= MAX_SKILLS; i++)
	{
	  d->character->skills[i] = 0;
	  d->character->pc->skills[i] = 0;
	}

      if (lookup_race_variable (d->character->race, RACE_NATIVE_TONGUE))
	{
	  d->character->speaks =
	    atoi (lookup_race_variable
		  (d->character->race, RACE_NATIVE_TONGUE));
	  d->character->skills[d->character->speaks] =
	    calc_lookup (d->character, REG_CAP, d->character->speaks);
	  d->character->pc->skills[d->character->speaks] =
	    calc_lookup (d->character, REG_CAP, d->character->speaks);
	}
    }
  else
    {
	d->character->nat_attack_type = 1;
	d->character->skills[SKILL_SPEAK_SINDARIN] = 55;
	d->character->pc->skills[SKILL_SPEAK_SINDARIN] = 55;
	d->character->speaks = SKILL_SPEAK_SINDARIN;
    }

  guest_conns++;

  if (ch->description)
    mem_free (ch->description);

  if (ch->pc->is_guide)
    ch->description =
      str_dup
      ("One of our friendly player #BGuides#0 is here, awaiting questions.\n");
  else
    ch->description =
      str_dup
      ("Another Guest is here, passing through. Be sure to welcome them!\n");

  ch->plr_flags |= NEWBIE_HINTS;

  if (is_admin (ch->pc->account_name))
  {
    ch->flags |= FLAG_ISADMIN;
    ch->flags |= FLAG_WIZNET;
  }
  else
    ch->flags &= ~FLAG_ISADMIN;

  //if (ch->race != 89 && ch->race != 69 && ch->race != 64)
    equip_newbie (ch);

  ch->hunger = -1;
  ch->thirst = -1;

  // If we're recreating, we're either recovering from a reboot or returning a dead
  // guest to the lounge, in which case we can skip a lot of this.

  pc_to_game (ch);

  char_to_room (ch, OOC_LOUNGE);

  if (str_cmp (argument, "recreate"))
    {
      act ("$n is incarnated in a soft glimmer of light.", true, d->character,
	   0, 0, TO_ROOM | _ACT_FORMAT);
      sprintf (buf, "%s [%s] has entered the lounge.", ch->tname,
	       ch->desc->strClientHostname);
      send_to_gods (buf);
      d->connected = CON_PLYNG;
      guest_conns++;
      sprintf (buf, "%s has logged in from %s as %s.",
	       char_short (d->character), d->strClientHostname,
	       d->character->tname);
      *buf = toupper (*buf);
      system_log (buf, false);
      mysql_safe_query
	("UPDATE newsletter_stats SET guest_logins=guest_logins+1");
      do_look (ch, "", 0);
    }
  else
    {
      if (finished_booting)	// Dead Guest being returned to the lounge.
	act
	  ("$n appears in a sudden glimmer of light, looking slightly dazed.",
	   true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      act
	("You feel your form briefly waver before it solidifies into yet another new guise, returned safely to the pleasant confines of Club Angband.",
	 false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
    }
}
