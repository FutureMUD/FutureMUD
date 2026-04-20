/*------------------------------------------------------------------------\
|  staff.c : Staff Command Module                     www.middle-earth.us | 
|  Copyright (C) 2004, Shadows of Isildur: Traithe                        |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/

#include <stdio.h>
#include <dirent.h>
#include <string.h>
#include <stdlib.h>
#include <unistd.h>
#include <ctype.h>
#include <time.h>
#include <signal.h>
#include <sys/time.h>
#include <sys/resource.h>
#include <sys/stat.h>
#include <mysql/mysql.h>
#include <limits.h>

#include "server.h"
#include "structs.h"
#include "net_link.h"
#include "protos.h"
#include "account.h"
#include "utils.h"
#include "decl.h"
#include "sys/stat.h"
#include "clan.h"		/* clan__assert_objs() */
#include "utility.h"

char s_buf[4096];
char b_buf[B_BUF_SIZE];

const char *player_bits[] = {
  "Brief",
  "NoShout",
  "Compact",
  "DONTSET",
  "Quiet",
  "Reboot",
  "Shutdown",
  "Build",
  "Approval",
  "Outlaw",
  "\n"
};

extern rpie::server engine;
extern const char *skills[];
extern const char *exit_bits[];

#define IMOTE_OPCHAR '^'
#define s(a) send_to_char (a "\n", ch);

CHAR_DATA *
is_switched (CHAR_DATA * ch)
{
  CHAR_DATA *tch;

  for (tch = character_list; tch; tch = tch->next)
    {
      if (tch->deleted)
	continue;
      if (!tch->desc)
	continue;
      if (tch->desc->original && tch->desc->original == ch)
	return tch;
    }

  return NULL;
}


void
do_roster (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch = NULL;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char admin[MAX_STRING_LENGTH];
  char title[MAX_STRING_LENGTH];
  char depart[MAX_STRING_LENGTH];
	int weight = 0;
	MYSQL_RES *result;
  MYSQL_ROW row;
	
  if (!*argument)
    {
      send_to_char ("Usage: roster (add | remove) <admin name> (<title> <department>)\n Usage: roster list\n",
        ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (str_cmp (buf, "add") && str_cmp (buf, "remove") && str_cmp (buf, "list"))
    {
      send_to_char ("Usage: roster (add | remove) <admin name> (<title> <department>)\n Usage: roster list\n", ch);
      return;
    }
    
	if (!str_cmp (buf, "list"))
		{
			sprintf(buf2, "Name: Title (Department)\n--------------------------\n");
			send_to_char (buf2, ch);
			mysql_safe_query
      ("SELECT name, title, dept FROM staff_roster");
      
      if ((result = mysql_store_result (database)) != NULL)
    		{
      		//row = mysql_fetch_row (result);
      		while (row = mysql_fetch_row (result))
      			{
        	 		sprintf (buf2, "%s:  %s (%s)\n", row[0], row[1], row[2]);
        	 		send_to_char (buf2, ch);
        	 	}
         }
    	
    	mysql_free_result (result);
      return;
    }

  argument = one_argument (argument, admin);
  argument = one_argument (argument, title);
  argument = one_argument (argument, depart);


  if (!*admin) 
    {
      send_to_char ("Usage: roster (add | remove) <admin name> (<title> <department>)\n Usage: roster list\n",
        ch);
      return;
    }

	if (!*admin && !*title && !*depart && !str_cmp (buf, "add"))
		{
      send_to_char ("Usage: roster (add | remove) <admin name> (<title> <department>)\n Usage: roster list\n",
		    ch);
      return;
    }

  if (islower (*admin))
    *admin = toupper (*admin);

  if (islower (*title))
    *title = toupper (*title);

  if (islower (*depart))
    *depart = toupper (*depart);

  if (!str_cmp (buf, "add") && !(tch = load_pc (admin)))
    {
      send_to_char ("That PC could not be found in our database.\n", ch);
      return;
    }

  if (!str_cmp (buf, "add") && tch->pc->level < 3)
    {
      send_to_char
	("All staff listed on the roster must be at least level 3.\n", ch);
      return;
    }

  if (tch)
    unload_pc (tch);

  if (!str_cmp (buf, "add"))
    {
      mysql_safe_query ("INSERT INTO staff_roster VALUES ('%s', '%s',  %d, '%s', %d)",
      admin, title, (int) time (0), depart, weight);
      send_to_char
	("The specified individual has been added to the staff roster.\n",
	 ch);
      return;
    }
  else if (!str_cmp (buf, "remove"))
    {
      mysql_safe_query ("DELETE FROM staff_roster WHERE name = '%s'", admin);
      mysql_safe_query ("DELETE FROM clan_assignments WHERE imm_name = '%s'",
			admin);
      send_to_char
	("The specified individual has been removed from the roster.\n", ch);
      return;
    }

  send_to_char ("Usage: roster (add | remove) <admin name> (<title> <department>)\n Usage: roster list\n", ch);
  return;
}

void
do_nuke (CHAR_DATA * ch, char *argument, int cmd)
{
  account *acct;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  if (!IS_IMPLEMENTOR (ch))
    {
      send_to_char
	("This command is temporarily disabled. It causes a whole slew of errors in the\nforums. Not to mention there doesn't seem to be any accounting system to\ntrack who has been nuked. If you need someone's account removed let me know\nand I will lock it down.--Sighentist\n",
	 ch);
      return;
    }

  if (!*argument)
    {
      send_to_char ("Nuke which account?\n", ch);
      return;
    }

  *argument = toupper (*argument);
  argument = one_argument (argument, buf);
  acct = new account (buf);
  if (!acct->is_registered ())
    {
      delete acct;
      send_to_char ("There is no account by that name in our database.\n",
		    ch);
      return;
    }
  delete acct;

  if (GET_TRUST (ch) < 5 && is_admin (acct->name.c_str ()))
    {
      send_to_char ("Only level 5 admins may delete other admin accounts.\n",
		    ch);
      return;
    }

  //if (IS_IMPLEMENTOR (ch))
  //  {
  //    send_to_char ("Naughty, naughty...\n", ch);
  //    return;
  //  }

  if (!*argument || *argument != '!')
    {
      sprintf (buf2, "Please type \'nuke %s !\', sans quotes, to confirm.\n",
	       buf);
      send_to_char (buf2, ch);
      return;
    }

  mysql_safe_query ("DELETE FROM forum_users WHERE username = '%s'", buf);

  sprintf (buf2, "Account %s has been excised from the database.\n", buf);
  send_to_char (buf2, ch);
}

void
do_register (CHAR_DATA * ch, char *argument, int cmd)
{
  account *acct;
  unsigned int i = 0;
  char name[MAX_STRING_LENGTH];
  char email[MAX_STRING_LENGTH];

  argument = one_argument (argument, name);
  argument = one_argument (argument, email);

  if (!*name || !*email)
    {
      send_to_char ("Usage: register <new account name> <email address>\n",
		    ch);
      return;
    }

  if (strlen (name) > 36)
    {
      send_to_char ("The account name must be 36 characters or less.\n", ch);
      return;
    }

  if (!isalpha (*name))
    {
      send_to_char
	("The first character of the account name MUST be a letter.\n", ch);
      return;
    }

  for (i = 0; i < strlen (name); i++)
    {
      if (!isalpha (name[i]) && !isdigit (name[i]))
	{
	  send_to_char
	    ("Account names may only contain letters and numbers.\n", ch);
	  return;
	}
    }

  *name = toupper (*name);

  if (!str_cmp (name, "Anonymous") || !str_cmp (name, IMPLEMENTOR_ACCOUNT))
    {
      send_to_char ("That account name cannot be registered.\n", ch);
      return;
    }
  acct = new account (name);
  if (acct->is_registered ())
    {
      delete acct;
      send_to_char ("That account name has already been taken.\n", ch);
      return;
    }
  delete acct;

  if (!strstr (email, "@") || !strstr (email, "."))
    {
      send_to_char
	("Email addresses must be given in the form of \'address@domain.com\'.\n",
	 ch);
      return;
    }

  new_accounts++;
  mysql_safe_query
    ("UPDATE newsletter_stats SET new_accounts=new_accounts+1");

  acct = new account;

  acct->set_email (email);
  acct->set_name (name);
  acct->created_on = time (0);

  setup_new_account (acct);

  send_to_char
    ("The new account has been registered, and notification has been emailed.\n",
     ch);
}

void
post_log (DESCRIPTOR_DATA * d)
{
  CHAR_DATA *ch;
  char report[MAX_STRING_LENGTH];

  ch = d->character;
  if (!*d->pending_message->message)
    {
      send_to_char ("No newsletter report posted.\n", ch);
      return;
    }

  sprintf (report, "%s", d->pending_message->message);

  if (!str_cmp (ch->delay_who, "blog"))
    {
      mysql_safe_query ("INSERT INTO building_log VALUES ('%s', '%s', %d)",
			ch->tname, report, (int) time (0));
      send_to_char ("Your building report has been filed.\n", ch);
      return;
    }
  else if (!str_cmp (ch->delay_who, "clog"))
    {
      mysql_safe_query ("INSERT INTO coding_log VALUES ('%s', '%s', %d)",
			ch->tname, report, (int) time (0));
      send_to_char ("Your coding report has been filed.\n", ch);
      return;
    }
  else if (!str_cmp (ch->delay_who, "plog"))
    {
      mysql_safe_query ("INSERT INTO plot_log VALUES ('%s', '%s', %d)",
			ch->tname, report, (int) time (0));
      send_to_char ("Your plot report has been filed.\n", ch);
      return;
    }
  else if (!str_cmp (ch->delay_who, "alog"))
    {
      mysql_safe_query ("INSERT INTO announcements VALUES ('%s', '%s', %d)",
			ch->tname, report, (int) time (0));
      send_to_char ("Your announcement has been filed.\n", ch);
      return;
    }
  else if (!str_cmp (ch->delay_who, "wlog"))
    {
      mysql_safe_query ("INSERT INTO website_log VALUES ('%s', '%s', %d)",
			ch->tname, report, (int) time (0));
      send_to_char ("Your website update has been filed.\n", ch);
      return;
    }
  else
    send_to_char ("There seems to be a problem with this command.\n", ch);

  ch->delay_who = NULL;
  unload_message (d->pending_message);
}

void
do_clog (CHAR_DATA * ch, char *argument, int cmd)
{
  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);
  send_to_char
    ("Enter a coding report to be included in the next newsletter:\n", ch);

  make_quiet (ch);

  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);

  ch->desc->str = &ch->desc->pending_message->message;
  ch->desc->max_str = MAX_STRING_LENGTH;

  ch->delay_who = str_dup ("clog");

  ch->desc->proc = post_log;
}

void
do_alog (CHAR_DATA * ch, char *argument, int cmd)
{
  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);
  send_to_char
    ("Enter a general staff announcement to be included in the next newsletter:\n",
     ch);

  make_quiet (ch);

  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);

  ch->desc->str = &ch->desc->pending_message->message;
  ch->desc->max_str = MAX_STRING_LENGTH;

  ch->delay_who = str_dup ("alog");

  ch->desc->proc = post_log;
}

void
do_blog (CHAR_DATA * ch, char *argument, int cmd)
{

  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);
  send_to_char
    ("Enter a building report to be included in the next newsletter:\n", ch);

  make_quiet (ch);

  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);

  ch->desc->str = &ch->desc->pending_message->message;
  ch->desc->max_str = MAX_STRING_LENGTH;

  ch->delay_who = str_dup ("blog");

  ch->desc->proc = post_log;
}

void
do_wlog (CHAR_DATA * ch, char *argument, int cmd)
{

  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);
  send_to_char
    ("Enter a website report to be included in the next newsletter:\n", ch);

  make_quiet (ch);

  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);

  ch->desc->str = &ch->desc->pending_message->message;
  ch->desc->max_str = MAX_STRING_LENGTH;

  ch->delay_who = str_dup ("wlog");

  ch->desc->proc = post_log;
}

void
do_plog (CHAR_DATA * ch, char *argument, int cmd)
{

  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);
  send_to_char
    ("Enter a plot report to be included in the next newsletter:\n", ch);

  make_quiet (ch);

  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);

  ch->desc->str = &ch->desc->pending_message->message;
  ch->desc->max_str = MAX_STRING_LENGTH;

  ch->delay_who = str_dup ("plog");

  ch->desc->proc = post_log;
}

void
do_wclone (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  OBJ_DATA *fbook, *tbook;
  WRITING_DATA *fwriting, *twriting;

  argument = one_argument (argument, buf);
  fbook = get_obj_in_list_vis (ch, buf, ch->right_hand);
  if (!fbook)
    fbook = get_obj_in_list_vis (ch, buf, ch->left_hand);
  if (!fbook)
    fbook = get_obj_in_list_vis (ch, buf, ch->room->contents);

  if (!fbook)
    {
      send_to_char ("Which written object did you wish to clone from?\n", ch);
      return;
    }
  if (GET_ITEM_TYPE (fbook) != ITEM_PARCHMENT
      && GET_ITEM_TYPE (fbook) != ITEM_BOOK)
    {
      send_to_char
	("The object you want to clone from isn't a parchment or book.\n",
	 ch);
      return;
    }

  if (!fbook->writing_loaded)
    load_writing (fbook);

  argument = one_argument (argument, buf);
  tbook = get_obj_in_list_vis (ch, buf, ch->right_hand);
  if (!tbook)
    tbook = get_obj_in_list_vis (ch, buf, ch->left_hand);
  if (!tbook)
    tbook = get_obj_in_list_vis (ch, buf, ch->room->contents);

  if (!tbook)
    {
      send_to_char ("Which written object did you wish to clone to?\n", ch);
      return;
    }
  if (GET_ITEM_TYPE (fbook) != ITEM_PARCHMENT
      && GET_ITEM_TYPE (fbook) != ITEM_BOOK)
    {
      send_to_char
	("The object you want to clone to isn't a parchment or book.\n", ch);
      return;
    }

  if (GET_ITEM_TYPE (fbook) != GET_ITEM_TYPE (tbook))
    {
      send_to_char
	("This command only works with two written objects of the same type.\n",
	 ch);
      return;
    }

  if (!fbook->writing)
    {
      send_to_char ("The specified source has no writing in it.\n", ch);
      return;
    }

  if (fbook == tbook)
    {
      send_to_char ("Source and destination can't be the same.\n", ch);
      return;
    }

  for (fwriting = fbook->writing; fwriting; fwriting = fwriting->next_page)
    {
      if (fwriting == fbook->writing)
	{
	  if (!tbook->writing)
	    CREATE (tbook->writing, WRITING_DATA, 1);
	  twriting = tbook->writing;
	  twriting->message = add_hash (fwriting->message);
	  twriting->author = add_hash (fwriting->author);
	  twriting->date = add_hash (fwriting->date);
	  twriting->ink = add_hash (fwriting->ink);
	  twriting->script = fwriting->script;
	  twriting->skill = fwriting->skill;
	  twriting->torn = false;
	  twriting->language = fwriting->language;
	  twriting->next_page = NULL;
	}
      else
	for (twriting = tbook->writing; twriting;
	     twriting = twriting->next_page)
	  {
	    if (!twriting->next_page)
	      {
		CREATE (twriting->next_page, WRITING_DATA, 1);
		twriting->next_page->message = add_hash (fwriting->message);
		twriting->next_page->author = add_hash (fwriting->author);
		twriting->next_page->date = add_hash (fwriting->date);
		twriting->next_page->ink = add_hash (fwriting->ink);
		twriting->next_page->script = fwriting->script;
		twriting->next_page->skill = fwriting->skill;
		twriting->next_page->torn = false;
		twriting->language = fwriting->language;
		twriting->next_page->next_page = NULL;
		break;
	      }
	  }
    }

  tbook->o.od.value[0] = fbook->o.od.value[0];

  if (fbook->book_title && *fbook->book_title)
    {
      tbook->book_title = add_hash (fbook->book_title);
      tbook->title_skill = fbook->title_skill;
      tbook->title_script = fbook->title_script;
      tbook->title_language = fbook->title_language;
    }

  sprintf (buf, "Writing cloned from #2%s#0 to #2%s#0.",
	   obj_short_desc (fbook), obj_short_desc (tbook));
  act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);

  save_writing (tbook);
}

void
do_wizlock (CHAR_DATA * ch, char *argument, int cmd)
{
  if (engine.in_play_mode ())
    {
      if (maintenance_lock)
	{
	  unlink ("maintenance_lock");
	}
      else
	{
	  system ("touch maintenance_lock");
	}
      
      send_to_char ("Wizlock toggled.\n", ch);
    }
  else
    {
      send_to_char ("Not on the player port. Wizlock not toggled.\n", ch);
    }
}

void
do_stayput (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch;
  char buf[MAX_STRING_LENGTH];

  if (!*argument)
    {
      send_to_char ("Toggle who's stayput flag?\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (!(tch = get_char_room_vis (ch, buf)))
    {
      send_to_char ("They aren't here.\n", ch);
      return;
    }

  if (!IS_NPC (tch))
    {
      send_to_char ("This command is for use on NPCs only.\n", ch);
      return;
    }

  if (!engine.in_play_mode ())
    {
      send_to_char ("This command is for use on the player port only.\n", ch);
      return;
    }

  if (IS_SET (tch->act, ACT_STAYPUT))
    {
      send_to_char ("This mobile will no longer save over reboots.\n", ch);
      tch->act &= ~ACT_STAYPUT;
      return;
    }
  else
    {
      send_to_char ("This mobile will now save over reboots.\n", ch);
      tch->act |= ACT_STAYPUT;
      save_stayput_mobiles ();
      return;
    }
}

void
do_clockout (CHAR_DATA * ch, char *argument, int cmd)
{
  if (!IS_NPC (ch))
    {
      send_to_char ("Eh?\n", ch);
      return;
    }

  if (engine.in_build_mode ())
    return;

  if (!ch->shop)
    {
      send_to_char ("This command is only for NPC shopkeepers.\n", ch);
      return;
    }

  ch->flags &= ~FLAG_KEEPER;
  send_to_char ("You will no longer sell things until you 'clockin'.\n", ch);
}

void
do_clockin (CHAR_DATA * ch, char *argument, int cmd)
{
  if (!IS_NPC (ch))
    {
      send_to_char ("Eh?\n", ch);
      return;
    }

  if (engine.in_build_mode ())
    return;

  if (!ch->shop)
    {
      send_to_char ("This command is only for NPC shopkeepers.\n", ch);
      return;
    }

  ch->flags |= FLAG_KEEPER;
  send_to_char ("You are now open for business again.\n", ch);
}

void
do_roll (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];

  *buf = '\0';

  if (strncasecmp (argument, "vs ", 3) == 0)
    {
      argument = one_argument (argument, buf);
      argument = one_argument (argument, buf);

      if (buf[3] == 0)
	{
	//roll 1-30 vs ATTR: attribute of 15 will have a 50% chance of sucess
	  int roll = dice (1, 30); 
	  int attr = 0;
	  if (strcasecmp (buf, "str") == 0)
	    {
	      attr = ch->str;
	    }
	  else if (strcasecmp (buf, "dex") == 0)
	    {
	      attr = ch->dex;
	    }
	  else if (strcasecmp (buf, "con") == 0)
	    {
	      attr = ch->con;
	    }
	  else if (strcasecmp (buf, "int") == 0)
	    {
	      attr = ch->intel;
	    }
	  else if (strcasecmp (buf, "wil") == 0)
	    {
	      attr = ch->wil;
	    }
	  else if (strcasecmp (buf, "aur") == 0)
	    {
	      attr = ch->aur;
	    }
	  else if (strcasecmp (buf, "agi") == 0)
	    {
	      attr = ch->agi;
	    }
	  else if (strcasecmp (buf, "luk") == 0)
	    {
	      attr = dice (3,6);
	    }
	  else // else attr = 0 ! \;)
	    {
	      strcpy (buf, "???");
	    }
	  
	  int diff = attr - roll;
	  char output_ch [AVG_STRING_LENGTH] = "";
	  char output_room [AVG_STRING_LENGTH] = "";

	  sprintf (output_ch, "#6OOC: Rolling vs. %s... you ", buf);
	  sprintf (output_room, "#6OOC: $n #6rolls vs. %s and ", buf);

	  if (diff >= 0)
	      {
		if (diff < 4)
		  {
		    strcat (output_ch, "just barely pass.#0\n");
		    strcat (output_room, "just barely passes.#0");
		  }
		else if (diff > 10)
		  {
		    strcat (output_ch, "pass gracefully.#0\n");
		    strcat (output_room, "passes gracefully.#0");
		  }
		else
		  {		    
		    strcat (output_ch, "pass.#0\n");
		    strcat (output_room, "passes.#0");
		  }
	      }
	    else
	      {
		if (diff > -5)
		  {
		    strcat (output_ch, "fall short of passing.#0\n");
		    strcat (output_room, "falls short of passing.#0");
		  }
		else if (diff < -10)
		  {
		    strcat (output_ch, "fail miserably.#0\n");
		    strcat (output_room, "fails miserably.#0");
		  }
		else
		  {
		    strcat (output_ch, "fail.#0\n");
		    strcat (output_room, "fails.#0");
		  }
	      }
	  send_to_char (output_ch, ch);
	  argument = one_argument (argument, buf);
	  if (strcasecmp (buf, "ooc") == 0) {
	    act (output_room, false, ch, 0, 0, TO_ROOM);
	  }
	  return;
	  
	}
      else 
	{
	  int ind = index_lookup (skills, buf);
	  if (ind >= 0)
	    {  
	      int roll = number (1, SKILL_CEILING);
	      int diff = skill_level (ch, ind, 0) - roll;

	      char output_ch [AVG_STRING_LENGTH] = "";
	      char output_room [AVG_STRING_LENGTH] = "";

	      sprintf (output_ch, "#6OOC: Rolling vs. %s... you ", skills[ind]);
	      sprintf (output_room, "#6OOC: $n #6rolls vs. %s and ", skills[ind]);

	      if (diff >= 0)
		{
		  if (diff < 10)
		    {
		      strcat (output_ch, "just barely pass.#0\n");
		      strcat (output_room, "just barely passes.#0");
		    }
		  else if (diff > 50)
		    {
		      strcat (output_ch, "pass gracefully.#0\n");
		      strcat (output_room, "passes gracefully.#0");
		    }
		  else
		    {		    
		      strcat (output_ch, "pass.#0\n");
		      strcat (output_room, "passes.#0");
		    }
		}
	      else
		{
		  if (diff > -10)
		    {
		      strcat (output_ch, "fall short of passing.#0\n");
		      strcat (output_room, "falls short of passing.#0");
		    }
		  else if (diff < -50)
		    {
		      strcat (output_ch, "fail miserably.#0\n");
		      strcat (output_room, "fails miserably.#0");
		    }
		  else
		    {
		      strcat (output_ch, "fail.#0\n");
		      strcat (output_room, "fails.#0");
		    }
		}
	      send_to_char (output_ch, ch);
	      argument = one_argument (argument, buf);
	      if (strcasecmp (buf, "ooc") == 0) {
		act (output_room, false, ch, 0, 0, TO_ROOM);
	      }
	      return;
	    }
	}

    }
  else
    {      
      char * p, * ooc;
      int rolls = strtol (argument, &p, 0);
      int die = strtol ((*p)?(p+1):(p), &ooc, 0);

      if ((rolls > 0 && rolls <= 100)
	  && (die > 0 && die <= 1000)
	  && (*p == ' ' || *p == 'd'))
	{

	  unsigned int total = dice (rolls, die);

	  bool ooc_output = false;
	  if (strcasecmp (ooc, " ooc") == 0)
	    {
	      ooc_output = true;
	    }
	  
	  sprintf (buf, "#6OOC: Rolling %dd%d... #2%d#0\n", rolls, die, total);
	  send_to_char (buf, ch);
	  
	  if (ooc_output) 
	    {
	      sprintf (buf, "#6OOC: $n #6rolls %dd%d, the result is #E%d#6.#0", rolls, die, total);
	      act (buf, false, ch, 0, 0, TO_ROOM);
	    }
	  return;
	}
    }
  
  send_to_char ("Usage: 'roll XdY', equivalent to rolling XdY.\n"
		"X and Y may be a numbers from 1 to 100.\n"
		"Add 'ooc' at the end to inform the rest of the room.\n", ch);
  return;

}

void
do_deduct (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch;
  account *acct;
  char buf[MAX_STRING_LENGTH];
  bool loaded = false;

  if (!*argument)
    {
      send_to_char ("Who would you like to deduct?\n", ch);
      return;
    }

  if (IS_NPC (ch))
    {
      send_to_char ("This is a PC-only command.\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  /// \todo Only lookup PCs, as there may be an NPC with the same keyword
  /// Or better yet make this command lookup by account name
  if (!(tch = get_char_room_vis (ch, buf)) && !(tch = get_char_vis (ch, buf)))
    {
      if (!(tch = load_pc (buf)))
	{
	  send_to_char ("No PC with that name was found. "
	     "Is it spelled correctly?\n", ch);
	  return;
	}
      else
	loaded = true;
    }
  
  if (!tch->pc || !tch->pc->account_name || !tch->pc->account_name[0])
    {
      send_to_char ("No PC with that name was found. "
		    "Is it spelled correctly?\n", ch);
      return;
    }

  acct = new account (tch->pc->account_name);

  if (!acct->is_registered ())
    {
      delete acct;
      act
	("Hmm... there seems to be a problem with that character's account. Please notify the staff.",
	 false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  argument = one_argument (argument, buf);

  if (!isdigit (*buf))
    {
      delete acct;
      send_to_char
	("You must specify a number of reward points to deduct.\n", ch);
      return;
    }

  int deduction = strtol(buf, 0, 10);
  if (deduction < 1 || deduction > acct->get_rpp ())
    {
      delete acct;
      act
	("The specified number must be greater than 0 and less than or equal to the named character's RP point total.",
	 false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  acct->deduct_rpp (deduction);
  delete acct;

  if (loaded)
    unload_pc (tch);

  send_to_char
    ("#1The specified number of points have been removed from their account.\n",
     ch);
  send_to_char
    ("Please be sure to leave a note explaining why on their pfile.#0\n", ch);

  sprintf (buf, "write %s Reward Point Deduction.", tch->tname);
  command_interpreter (ch, buf);

}

void
do_award (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch;
  char buf[MAX_STRING_LENGTH];
  account *acct;
  bool loaded = false;

  if (!*argument)
    {
      send_to_char ("Who would you like to award?\n", ch);
      return;
    }

  if (IS_NPC (ch))
    {
      send_to_char ("This is a PC-only command.\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (!(tch = get_char_room_vis (ch, buf)) && !(tch = get_char_vis (ch, buf)))
    {
      if (!(tch = load_pc (buf)))
	{
	  send_to_char
	    ("No PC with that name was found. Is it spelled correctly?\n",
	     ch);
	  return;
	}
      else
	loaded = true;
    }

  if (IS_NPC (tch))
    {
      send_to_char ("This command only works for player characters.\n", ch);
      return;
    }

  acct = new account (tch->pc->account_name);

  if (!acct->is_registered ())
    {
      delete acct;
      act
	("Hmm... there seems to be a problem with that character's account. Please notify the staff.",
	 false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      return;
    }

  if (!str_cmp (tch->pc->account_name, ch->pc->account_name) 
      && !engine.in_test_mode ())
    {
      delete acct;
      send_to_char
	("Adding reward points to your own account is against policy.\n",
	 ch);
      return;
    }

  if (tch->desc && tch->desc->acct
      && IS_SET (tch->desc->acct->flags, ACCOUNT_RPPDISPLAY))
    send_to_char
      ("#6Congratulations, you've just been awarded a reward point!#0\n",
       tch);
  acct->award_rpp ();
  delete acct;

  if (loaded)
    unload_pc (tch);

  send_to_char
    ("#2A reward point has been added to that character's account.\n", ch);
  send_to_char
    ("Please be sure to leave a note explaining why on their pfile.#0\n", ch);

  sprintf (buf, "write %s Reward Point.", tch->tname);
  command_interpreter (ch, buf);

}

void
post_email (DESCRIPTOR_DATA * d)
{
  account *acct;
  char from[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];

  if (!*d->pending_message->message)
    {
      send_to_char ("Email aborted.\n", d->character);
      d->pending_message = NULL;
      return;
    }

  sprintf (buf, "%s", d->pending_message->message);
  d->pending_message = NULL;

  sprintf (from, "%s <%s>", d->character->tname, d->acct->email.c_str ());

  acct = new account (d->character->delay_who);

  send_email (acct, d->acct->email.c_str (), from, d->character->delay_who2,
	      buf);

  delete acct;
  d->character->act &= ~PLR_QUIET;

  send_to_char
    ("Your email has been sent out. It has been copied to your address for reference.\n",
     d->character);

  d->character->delay_who = NULL;
  d->character->delay_who2 = NULL;
}

void
do_email (CHAR_DATA * ch, char *argument, int cmd)
{
  account *acct;
  CHAR_DATA *tch;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("To whom did you wish to send an email?\n", ch);
      return;
    }

  if (!(tch = load_pc (buf)))
    {
      send_to_char ("That PC's file could not be loaded.\n", ch);
      return;
    }
  acct = new account (tch->pc->account_name);
  if (!acct->is_registered ())
    {
      send_to_char ("That character's account could not be loaded.\n", ch);
      unload_pc (tch);
      return;
    }

  ch->delay_who = str_dup (acct->name.c_str ());
  unload_pc (tch);

  if (!*argument)
    {
      send_to_char
	("What would you like the subject-line of the email to be?\n", ch);
      return;
    }

  if (IS_NPC (ch))
    {
      send_to_char ("Only PCs may use this command.\n", ch);
      return;
    }

  send_to_char ("\n#2Enter the message you'd like to email this player:#0\n",
		ch);

  CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);
  ch->desc->str = &ch->desc->pending_message->message;
  ch->desc->max_str = MAX_STRING_LENGTH;
  ch->desc->proc = post_email;
  ch->delay_who2 = str_dup (argument);
  ch->act |= PLR_QUIET;

}

void
do_unban (CHAR_DATA * ch, char *argument, int cmd)
{
  SITE_INFO *site, *next_site;
  char buf[MAX_STRING_LENGTH];
  int i;

  if (!*argument || !isdigit (*argument))
    {
      send_to_char
	("You must specify the number of the site from the banned sitelist.\n",
	 ch);
      return;
    }

  if (!banned_site)
    {
      send_to_char ("There are currently no sites to unban!\n", ch);
      return;
    }

  for (i = 1, site = banned_site; site; i++, site = next_site)
    {
      next_site = site->next;
      if (atoi (argument) == 1 && i == 1)
	{
	  if (GET_TRUST (ch) < 5
	      && str_cmp (site->banned_by, "Password Security System")
	      && str_cmp (ch->tname, site->banned_by))
	    {
	      sprintf (buf,
		       "Sorry, but you'll need to get %s or a level 5 admin to do that.\n",
		       site->banned_by);
	      send_to_char (buf, ch);
	      return;
	    }
	  unban_site (site);
	  return;
	}
      else if (i + 1 == atoi (argument) && site->next)
	{
	  if (GET_TRUST (ch) < 5
	      && str_cmp (ch->tname, site->next->banned_by))
	    {
	      sprintf (buf,
		       "Sorry, but you'll need to get %s or a level 5 admin to do that.\n",
		       site->banned_by);
	      send_to_char (buf, ch);
	      return;
	    }
	  unban_site (site->next);
	  return;
	}
    }

  send_to_char ("That number is not on the banned site list.\n", ch);

}

bool is_banned (DESCRIPTOR_DATA * d);
void
disconnect_banned_hosts ()
{
  SITE_INFO *site;
  DESCRIPTOR_DATA *d, *next_desc;

  for (site = banned_site; site; site = site->next)
    {
      for (d = descriptor_list; d; d = next_desc)
	{
	  next_desc = d->next;
	  if (!d->acct)
	    continue;
	  if (!IS_SET (d->acct->flags, ACCOUNT_NOBAN))
	    {
	      if (strstr (d->strClientHostname, site->name))
		{
		  close_socket (d);
		}
	    }
	}
    }
}

void
ban_host (char *host, char *banned_by, int length)
{
  SITE_INFO *site, *tmp_site;
  char buf[MAX_STRING_LENGTH];

  CREATE (site, SITE_INFO, 1);
  site->name = add_hash (host);
  site->banned_by = add_hash (banned_by);
  site->banned_on = time (0);
  site->next = NULL;
  
  if (length != -1 && length != -2)
    site->banned_until = time (0) + (60 * 60 * 24 * length);
  else if (length == -2)
    site->banned_until = time (0) + (60 * 60);
  else if (length == -1)
    site->banned_until = -1;

  if (!banned_site)
    banned_site = site;
  else
    for (tmp_site = banned_site; tmp_site; tmp_site = tmp_site->next)
      {
	if (!tmp_site->next)
	  {
	    tmp_site->next = site;
	    break;
	  }
      }

  sprintf (buf, "%s has sitebanned %s.\n", banned_by, host);
  send_to_gods (buf);
  system_log (buf, false);

  save_banned_sites ();
}

void
do_disconnect (CHAR_DATA * ch, char *argument, int cmd)
{
  DESCRIPTOR_DATA *d;

  if (!argument || !*argument)
    {
      send_to_char ("Usage: disconnect <account>\n", ch);
    }

  for (d = descriptor_list; d; d = d->next)
    {

      if (d->acct &&
	  d->acct->name.length () && !strcmp (d->acct->name.c_str (), argument))
	{

	  close_socket (d);

	}
    }
}


/*                                                                          *
 * funtion: do_ban                      < e.g.> ban [ <host> <duration> ]   *
 *                                                                          *
 * 09/17/2004 [JWW] - was freeing the static char[] that asctime() returns! *
 *                    now switched to using alloc, asctime_r, and mem_free  *
 *                                                                          */
void
do_ban (CHAR_DATA * ch, char *argument, int cmd)
{
  SITE_INFO *site;
  MYSQL_RES *result;
  MYSQL_ROW row;
  char *start_date, *end_date;
  char buf[MAX_STRING_LENGTH];
  char host[MAX_STRING_LENGTH];
  char length[MAX_STRING_LENGTH];
  char accounts[MAX_STRING_LENGTH];
  unsigned int i, j;
  time_t ban_time, end_ban_time;

  if (!*argument)
    {
      sprintf (buf, "\n#6Currently Banned Sites:#0\n\n");
      if (!banned_site)
	{
	  sprintf (buf + strlen (buf), "   None.\n");
	  send_to_char (buf, ch);
	  return;
	}
      else
	for (i = 1, site = banned_site; site; i++, site = site->next)
	  {
	    *accounts = '\0';
	    if (site->name[0] == '^')
	      {
		mysql_safe_query
		  ("SELECT username,account_flags FROM forum_users WHERE user_last_ip LIKE '%s%%'",
		   site->name + 1);
	      }
	    else
	      {
		mysql_safe_query
		  ("SELECT username,account_flags FROM forum_users WHERE user_last_ip LIKE '%%%s%%'",
		   site->name);
	      }
	    result = mysql_store_result (database);
	    j = 1;
	    if (result)
	      {
		if (mysql_num_rows (result) >= 10)
		  sprintf (accounts + strlen (accounts),
			   " #110+ accounts affected#0");
		else
		  while ((row = mysql_fetch_row (result)))
		    {
		      if (IS_SET (atoi (row[1]), ACCOUNT_NOBAN))
			continue;
		      if (j != 1)
			sprintf (accounts + strlen (accounts), ",");
		      sprintf (accounts + strlen (accounts), " %s", row[0]);
		      j++;
		    }
		mysql_free_result (result);
	      }
	    if (i != 1)
	      sprintf (buf + strlen (buf), "\n");
	    sprintf (buf + strlen (buf), "   %d. #6%s#0 [%s]\n", i,
		     site->name, site->banned_by);
	    if (i > 9)
	      sprintf (buf + strlen (buf), " ");
	    if (*accounts)
	      sprintf (buf + strlen (buf), "      #2Accounts:#0%s\n",
		       accounts);
	    else
	      sprintf (buf + strlen (buf),
		       "      #2Accounts:#0 None Found\n");

	    ban_time = site->banned_on;
	    start_date = (char *) alloc (256, 31);
	    if (asctime_r (localtime (&ban_time), start_date) != NULL)
	      {
		start_date[strlen (start_date) - 1] = '\0';
	      }
	    else
	      {
		start_date[0] = '\0';
	      }

	    if (site->banned_until != -1)
	      {
		if (i > 9)
		  sprintf (buf + strlen (buf), " ");
		sprintf (buf + strlen (buf), "      #2Ban Period:#0 %s to ",
			 start_date);

		end_ban_time = site->banned_until;
		end_date = (char *) alloc (256, 31);
		if (asctime_r (localtime (&end_ban_time), end_date) != NULL)
		  {
		    end_date[strlen (end_date) - 1] = '\0';
		  }
		else
		  {
		    end_date[0] = '\0';
		  }

		sprintf (buf + strlen (buf), "%s\n", end_date);
		mem_free (end_date);
		end_date = NULL;
	      }
	    else
	      {
		if (i > 9)
		  sprintf (buf + strlen (buf), " ");
		sprintf (buf + strlen (buf),
			 "      #2Permanently Banned On:#0 %s\n", start_date);
	      }
	    mem_free (start_date);
	    start_date = NULL;
	  }
      page_string (ch->desc, buf);
      return;
    }

  argument = one_argument (argument, host);
  argument = one_argument (argument, length);

  for (i = 0; i < strlen (host); i++)
    {
      if (!isalpha (host[i]) && !isdigit (host[i])
	  && host[i] != '.' && host[i] != '_'
	  && host[i] != '-' && host[i] != '*' && host[i] != '^')
	{
	  sprintf (buf, "Illegal character '%c' in siteban entry.\n",
		   host[i]);
	  send_to_char (buf, ch);
	  return;
	}
    }

  if (!*length || (!isdigit (*length) && *length != '!'))
    {
      send_to_char
	("You need to specify a length of days; otherwise, '!' for a permanent ban.\n",
	 ch);
      return;
    }

  if (*length != '!' && atoi (length) < 1)
    {
      send_to_char ("The minimum ban period is 1 day.\n", ch);
      return;
    }

  if (*length == '!')
    ban_host (host, ch->tname, -1);
  else
    ban_host (host, ch->tname, atoi (length));

  disconnect_banned_hosts ();
}

void
do_echo (CHAR_DATA * ch, char *argument, int cmd)
{
  int i;
  char buf[MAX_STRING_LENGTH];

  for (i = 0; *(argument + i) == ' '; i++);

  if (!*(argument + i))
    {
      send_to_char ("That must be a mistake...\n", ch);
      return;
    }

  sprintf (buf, "%s\n", argument + i);
  send_to_room (buf, ch->in_room);
}

void
do_gecho (CHAR_DATA * ch, char *argument, int cmd)
{
  int i;
  ROOM_DATA *troom;

  for (i = 0; *(argument + i) == ' '; i++);

  if (!*(argument + i))
    send_to_char ("That must be a mistake...\n", ch);
  else
    {
      sprintf (s_buf, "%s\n", argument + i);

      for (troom = full_room_list; troom; troom = troom->lnext)
	if (troom->people)
	  send_to_room (s_buf, troom->nVirtual);
    }
}

void
do_broadcast (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  char *p;
  int i;
  DESCRIPTOR_DATA *d;

  for (i = 0; *(argument + i) == ' '; i++);

  if (!*(argument + i))
    send_to_char ("That must be a mistake...\n", ch);
  else
    {
      sprintf (s_buf, "%s\n", argument + i);
      sprintf (buf, "\a\n#2Staff Announcement:#0\n\n");
      reformat_string (s_buf, &p);
      sprintf (buf + strlen (buf), "%s", p);

      for (d = descriptor_list; d; d = d->next)
	SEND_TO_Q (buf, d);

    }
}

void
do_zecho (CHAR_DATA * ch, char *argument, int cmd)
{
  int zone = -1;
  int first_room = -1;
  int last_room = -1;
  ROOM_DATA *room;
  char buf[MAX_STRING_LENGTH];
  char mess[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf || !strcmp (buf, "?"))
    {
      send_to_char ("  zecho <zone> Message        send message to specific "
		    "zone.\n", ch);
      send_to_char ("  zecho Message               send message to zone you "
		    "are in.\n", ch);
	  send_to_char ("  zecho r <first room num> <last room num> Message   send message to rooms in a range.\n", ch);	    
      return;
    }

  while (*argument == ' ')
    argument++;

  if (isdigit (*buf) && atoi (buf) < 100)
    {
      zone = atoi (buf);
      sprintf(mess, argument);
    }
  else if (!strcmp(buf, "r"))
  	{
  	argument = one_argument (argument, buf);
  	if (isdigit (*buf))
	  	first_room = atoi (buf);
  	else
  		{
  		send_to_char ("  zecho r <first room num> <last room num> Message   send message to rooms in a range.\n", ch);	    
        return;
  		}
  		
  	argument = one_argument (argument, buf);
  	if (isdigit (*buf))
	  	last_room = atoi (buf);
  	else
  		{
  		send_to_char ("  zecho r <first room num> <last room num> Message   send message to rooms in a range.\n", ch);	    
        return;
  		}
  		
  	sprintf(mess, argument);  
  	}
  else
  	{
    zone = ch->in_room / 1000;
    sprintf(mess, buf);
	}
	
  strcat (mess, "\n");
  
  if (zone >= 0)
  	{
  	sprintf (buf, "(to zone %d): %s", zone, mess);
  	send_to_char (buf, ch);
  	}
  else
  	{
  	sprintf (buf, "(to rooms %d  to %d): %s", first_room, last_room, mess);
  	send_to_char (buf, ch);
  	}
  	
  for (room = full_room_list; room; room = room->lnext)
  	{
  	if (zone >= 0)
  		{
    if (room->people && room->zone == zone)
	    	{
			send_to_room (mess, room->nVirtual);
			}
  	  	}
  	else if ((first_room > 0) && (last_room > 0))
  		{
  		if ((room->people) &&
  			(room->nVirtual >= first_room) &&
  			(room->nVirtual <= last_room))
  			{
			send_to_room (mess, room->nVirtual);
			}
  		}
	}//for (room = full_room_list
  	  	
}

void
do_pecho (CHAR_DATA * ch, char *argument, int cmd)
{

  CHAR_DATA *vict;
  DESCRIPTOR_DATA *d;
  char name[MAX_STRING_LENGTH];
  char message[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];

  half_chop (argument, name, message);

  if (!*name || !*message)
    send_to_char ("Who do you wish to pecho to and what??\n", ch);

  else if (!(vict = get_char_nomask (name)) || IS_NPC (vict) ||
	   (!GET_TRUST (vict) && IS_SET (vict->flags, FLAG_WIZINVIS)))
    send_to_char ("There is nobody playing the mud by that name.\n", ch);

  else if (ch == vict)
    send_to_char ("You try to tell yourself something.\n", ch);

  else if (IS_SET (vict->act, PLR_QUIET))
    {
      send_to_char ("That player is editing, try again later.\n", ch);
      return;
    }

  else
    {
      if (IS_MORTAL (ch) && IS_SET (vict->flags, FLAG_ANON))
	{
	  send_to_char ("There is nobody playing the mud by that name.\n",
			ch);
	  return;
	}

      if (!vict->desc && !IS_NPC (vict))
	{
	  for (d = descriptor_list; d; d = d->next)
	    {
	      if (d == vict->pc->owner)
		break;
	    }

	  if (!d)
	    {
	      send_to_char ("That player has disconnected.\n", ch);
	      return;
	    }
	}
      sprintf (buf, "You pecho to %s: '%s#0'\n", GET_NAME (vict), message);
      strcat (message, "#0\n");
      send_to_char (message, vict);
      send_to_char (buf, ch);
    }
}


void
do_transfer (CHAR_DATA * ch, char *argument, int cmd)
{
  DESCRIPTOR_DATA *i;
  CHAR_DATA *victim;
  char buf[MAX_STRING_LENGTH];
  int target;

  argument = one_argument (argument, buf);
  if (!*buf)
    send_to_char ("Who do you wish to transfer?\n", ch);
  else if (str_cmp ("all", buf))
    {
      if (!(victim = get_char_vis (ch, buf)))
	send_to_char ("No-one by that name around.\n", ch);
      else
	{
	  act ("$n disappears in a mushroom cloud.", false, victim, 0, 0,
	       TO_ROOM);
	  target = ch->in_room;
	  char_from_room (victim);
	  char_to_room (victim, target);
	  act ("$n arrives from a puff of smoke.", false, victim, 0, 0,
	       TO_ROOM);
	  act ("$n has transferred you!", false, ch, 0, victim, TO_VICT);
	  do_look (victim, "", 15);
	}
    }
  else
    {				/* Trans All */
      if (GET_TRUST (ch) < 5)
	{
	  send_to_char
	    ("Sorry, but TRANSFER ALL is a level 5-only command.\n", ch);
	  return;
	}

      if (!*argument || *argument != '!')
	{
	  send_to_char ("Please type 'transfer all !' to confirm.\n", ch);
	  return;
	}

      for (i = descriptor_list; i; i = i->next)
	{
	  if (i->character != ch && !i->connected)
	    {
	      victim = i->character;
	      act ("$n disappears in a mushroom cloud.", false, victim, 0, 0,
		   TO_ROOM);
	      target = ch->in_room;
	      char_from_room (victim);
	      char_to_room (victim, target);
	      act ("$n arrives from a puff of smoke.", false, victim, 0, 0,
		   TO_ROOM);
	      act ("$n has transferred you!", false, ch, 0, victim, TO_VICT);
	      do_look (victim, "", 15);
	    }
	}

      send_to_char
	("All online PCs have been transferred to your location.\n", ch);
      return;
    }
}

void
do_at (CHAR_DATA * ch, char *argument, int cmd)
{
  char command[MAX_INPUT_LENGTH];
  char loc_str[MAX_INPUT_LENGTH];
  int location;
  int original_loc;
  CHAR_DATA *target_mob;
  OBJ_DATA *target_obj;
  ROOM_DATA *target_room;

  half_chop (argument, loc_str, command);

  if (!*loc_str)
    {
      send_to_char ("You must supply a room number or a name.\n", ch);
      return;
    }


  if (isdigit (*loc_str))
    {
      if (!(target_room = vtor (atoi (loc_str))))
	{
	  send_to_char ("No room exists with that number.\n", ch);
	  return;
	}

      location = target_room->nVirtual;
    }

  else if ((target_mob = get_char_vis (ch, loc_str)))
    location = target_mob->in_room;

  else if ((target_obj = get_obj_vis (ch, loc_str)))
    if (target_obj->in_room != NOWHERE)
      location = target_obj->in_room;
    else
      {
	send_to_char ("The object is not available.\n", ch);
	return;
      }
  else
    {
      send_to_char ("No such creature or object around.\n", ch);
      return;
    }

  /* a location has been found. */

  original_loc = ch->in_room;
  char_from_room (ch);
  char_to_room (ch, location);
  command_interpreter (ch, command);

  /* check if the guy's still there */

  for (target_mob = vtor (location)->people;
       target_mob; target_mob = target_mob->next_in_room)
    if (ch == target_mob)
      {
	char_from_room (ch);
	char_to_room (ch, original_loc);
      }
}

void
do_goto (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_INPUT_LENGTH];
  int location;
  CHAR_DATA *target_mob, *tch;
  OBJ_DATA *target_obj;
  ROOM_DATA *troom;

  clear_pmote (ch);

  argument = one_argument (argument, buf);
  if (!*buf)
    {
      send_to_char ("You must supply a room number or a name.\n", ch);
      return;
    }

  if (isdigit (*buf))
    {

      if (!(troom = vtor (atoi (buf))))
	{
	  send_to_char ("No room exists with that number.\n", ch);
	  return;
	}

      location = troom->nVirtual;
    }

  else if ((target_mob = get_char_vis (ch, buf)))
    location = target_mob->in_room;

  else if ((target_obj = get_obj_vis (ch, buf)))
    {
      if (target_obj->in_room != NOWHERE)
	location = target_obj->in_room;
      else
	{
	  send_to_char ("The object is not available.\n", ch);
	  return;
	}
    }
  else
    {
      send_to_char ("No such creature or object around.\n", ch);
      return;
    }

  /* a location has been found. */

  if (ch->in_room == location)
    {
      send_to_char ("You're already there.\n", ch);
      return;
    }

  if (ch->pc)
    {
      if (!ch->pc->imm_leave || !strncmp (ch->pc->imm_leave, "(null)", 6))
	ch->pc->imm_leave = str_dup ("");

      if (!ch->pc->imm_enter || !strncmp (ch->pc->imm_enter, "(null)", 6))
	{
	  ch->pc->imm_enter = str_dup ("");
	}

      if (*ch->pc->imm_leave)
	act (ch->pc->imm_leave, true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      else
	act ("$n leaves the area.", true, ch, 0, 0, TO_ROOM);
    }
  else
    act ("$n leaves the area.", true, ch, 0, 0, TO_ROOM);

  if (!IS_SET (ch->flags, FLAG_WIZINVIS) && *argument != '!')
    {

      troom = vtor (location);

      for (tch = troom->people; tch; tch = tch->next_in_room)
	{
	  if (!IS_NPC (tch) && IS_MORTAL (tch))
	    {
	      send_to_char
		("You're currently visible to players. Use '!' to confirm the goto.\n",
		 ch);
	      return;
	    }
	}
    }

  char_from_room (ch);
  char_to_room (ch, location);

  if (ch->pc && ch->pc->imm_enter && ch->pc->imm_enter[0])
    {
      act (ch->pc->imm_enter, true, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
    }
  else
    {
      act ("$n enters the area.", true, ch, 0, 0, TO_ROOM);
    }
  do_look (ch, "", 15);
}

void
pad_buffer (char *buf, int pad_stop)
{
  int to_pad;
  char *p;

  p = buf + strlen (buf);

  for (to_pad = pad_stop - strlen (buf); to_pad > 0; to_pad--, p++)
    *p = ' ';

  *p = 0;
}

#define ADD buf + strlen (buf)

void
charstat (CHAR_DATA * ch, char *name, bool bPCsOnly)
{
  int i, j = 0;
  int i2 = 0;
  int loaded_char = 0;
  int loads;
  int econ_zone;
  int instance = 1;
  float value_pay;
  int time_diff = 0, hours_remaining = 0, days_remaining = 0, mins_remaining = 0;
  char *p;
  CHAR_DATA *k = NULL;
  CHAR_DATA *tch = NULL;
  OBJ_DATA *vobj = NULL;
  AFFECTED_TYPE *af;
  AFFECTED_TYPE *next_affect;
  ENCHANTMENT_DATA *ench;
  POISON_DATA *poison;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  bool bulktrader = false;

  if (name == NULL || !*name)
    {
      send_to_char ("Stat who?\n", ch);
      return;
    }

  if (bPCsOnly)
    {

      if ((k = get_char_room_vis (ch, name)) && !IS_NPC (k))
	{
	}
      else if ((k = get_char_vis (ch, name)) && !IS_NPC (k))
	{
	}
      else if ((k = load_pc (name)))
	{
	  loaded_char = 1;
	}
    }
  if (k == NULL)
    {
      if (atoi (name) && !strstr (name, "."))
	{

	  if (!(k = vtom (atoi (name))))
	    {
	      send_to_char ("No such mobile with that vnum.\n", ch);
	      return;
	    }
	  instance = 0;

	}
      else if ((k = get_char_room_vis (ch, name)) && IS_NPC (k))
	{
	}
      else if ((k = get_char_vis (ch, name)) && IS_NPC (k))
	{
	}
      else
	{

	  for (tch = full_mobile_list; tch; tch = tch->mob->lnext)
	    {

	      if (!*tch->name)
		continue;

	      if (strstr (tch->name, name))
		{
		  k = tch;
		}
	    }

	}
      if (k == NULL && !bPCsOnly)
	{
	  charstat (ch, name, true);
	}
    }

  if (k == NULL)
    {
      send_to_char ("No player or mobile with that name found.\n", ch);
      return;
    }


  if (!IS_NPC (k))
    sprintf (buf, "\n#2Name:#0   %s, a %s PC", k->tname,
	     sex_types[(int) k->sex]);
  else
    sprintf (buf, "\n#2VNUM:#0   %d, a %s %s", k->mob->nVirtual,
	     sex_types[(int) k->sex],
	     instance ? "[INSTANCE]" : "[PROTOTYPE]");

  if (IS_NPC (k))
    {
      loads = 0;

      for (tch = character_list; tch; tch = tch->next)
	{
	  if (tch->deleted || !IS_NPC (tch))
	    continue;

	  if (tch->mob->nVirtual == k->mob->nVirtual)
	    loads++;
	}
      sprintf (ADD, " [Loaded Instances: %d]", loads);
    }
  else if (k->pc && k->pc->account_name)
    {
      sprintf (ADD, ", registered to account %s", k->pc->account_name);
    }

  sprintf (ADD, " [id %d]\n", k->coldload_id);

  send_to_char (buf, ch);

  sprintf (buf, "#2Keys:#0   %s\n", GET_NAMES (k));

  sprintf (buf + strlen (buf), "#2SDesc:#0  %s\n",
	   k->short_descr ? k->short_descr : "None");

  sprintf (buf + strlen (buf), "#2LDesc:#0  %s\n",
	   k->long_descr ? k->long_descr : "None");

  send_to_char (buf, ch);
  send_to_char ("\n", ch);

  sprintf (buf, "#2Str:#0 %d/%d", GET_STR (k), k->str);
  pad_buffer (buf, 25);
  sprintf (ADD, "#2MaxHP:#0 %d", GET_MAX_HIT (k));
  pad_buffer (buf, 53);
  if (instance)
    sprintf (ADD, "#2Moves:#0  %d/%d", GET_MOVE (k), GET_MAX_MOVE (k));
  else
    sprintf (ADD, "#2Moves:#0  N/A");
  send_to_char (buf, ch);
  send_to_char ("\n", ch);

  sprintf (buf, "#2Dex:#0 %d/%d", GET_DEX (k), k->dex);
  pad_buffer (buf, 25);
  sprintf (ADD, "#2Race:#0  %s", lookup_race_variable (k->race, RACE_NAME));
  pad_buffer (buf, 53);
  sprintf (ADD, "#2Speak:#0  %s", skills[k->speaks]);
  send_to_char (buf, ch);
  send_to_char ("\n", ch);

  sprintf (buf, "#2Con:#0 %d/%d", GET_CON (k), k->con);
  pad_buffer (buf, 25);
  sprintf (buf + strlen (buf), "#2Age:#0   %dY %dM %dD %dH",
	   age (k).year, age (k).month, age (k).day, age (k).hour);
  pad_buffer (buf, 53);
  if (instance)
    sprintf (ADD, "#2Ht/Wt:#0  %d/%d", k->height, get_weight (k) / 100);
  else
    sprintf (ADD, "#2Ht/Wt:#0  N/A");
  send_to_char (buf, ch);
  send_to_char ("\n", ch);

  sprintf (buf, "#2Int:#0 %d/%d", GET_INT (k), k->intel);
  pad_buffer (buf, 25);
  sprintf (ADD, "#2Fight:#0 %s", fight_tab[k->fight_mode].name);
  pad_buffer (buf, 53);
  sprintf (ADD, "#2RmPos:#0  %d", k->room_pos);
  sprintf (ADD, "\n");
  send_to_char (buf, ch);

  sprintf (buf, "#2Wil:#0 %d/%d", GET_WIL (k), k->wil);
  pad_buffer (buf, 25);
  sprintf (ADD, "#2Hrnss:#0 %d", k->harness);
  pad_buffer (buf, 53);
  sprintf (ADD, "#2MHrnss:#0 %d", k->max_harness);
  send_to_char (buf, ch);
  send_to_char ("\n", ch);

  sprintf (buf, "#2Aur:#0 %d/%d", GET_AUR (k), k->aur);
  pad_buffer (buf, 25);
  sprintf (ADD, "#2Armor:#0 %d", k->armor);
  pad_buffer (buf, 53);
  sprintf (ADD, "#2Speed:#0  %s", speeds[k->speed]);
  send_to_char (buf, ch);
  send_to_char ("\n", ch);

  sprintf (buf, "#2Agi:#0 %d/%d", GET_AGI (k), k->agi);

  pad_buffer (buf, 25);
  sprintf (ADD, "#2Room:#0  %d", k->in_room);
  pad_buffer (buf, 53);
  if (IS_NPC (k))
    {
      sprintf (ADD, "#2Spwn:#0   %d", k->mob->spawnpoint);
    }
  else
    {
      sprintf (ADD, "#2Cond:#0   %d,%d,%d",
	       k->intoxication, k->hunger, k->thirst);
    }

  sprintf (ADD, "\n");
  send_to_char (buf, ch);

  if (IS_NPC (k))
    {
      *buf = '\0';
      sprintf (buf, "#2Sum:#0 %d",
	       k->str + k->dex + k->con + k->intel + k->wil +
	       k->aur + k->agi);
      pad_buffer (buf, 25);
      sprintf (buf + strlen (buf), "#2Carc:#0  %d", k->mob->carcass_vnum);
      pad_buffer (buf, 53);
      sprintf (ADD, "#2Skin:#0   %d", k->mob->skinned_vnum);
      sprintf (ADD, "\n");
      send_to_char (buf, ch);
      *buf = '\0';
      pad_buffer (buf, 21);
      sprintf (buf + strlen (buf), "#2Att:#0   %s",
	       attack_names[k->nat_attack_type]);
      pad_buffer (buf, 49);
      sprintf (ADD, "#2NDam:#0   %dd%d", k->mob->damnodice,
	       k->mob->damsizedice);
      sprintf (ADD, "\n");
      send_to_char (buf, ch);
      *buf = '\0';
      pad_buffer (buf, 21);
      sprintf (buf + strlen (buf), "#2Fallback:#0   %d", k->mob->fallback);
      sprintf (ADD, "\n");
      send_to_char (buf, ch);
    }
  else if (k->pc)
    {
      *buf = '\0';
      sprintf (buf, "#2Sum:#0 %d",
	       k->str + k->dex + k->con + k->intel + k->wil +
	       k->aur + k->agi);
      pad_buffer (buf, 25);
      sprintf (buf + strlen (buf), "#2Skillcap:#0 %d/%d", skill_max(k, 0, 1), (600 + ( 70 * (k->intel-10))));
      pad_buffer (buf, 53);
      if (k->pc->level > 5)
	{
	  strcat (buf, "#2Level:#0 Implementor");
	}
      else
	{
	  sprintf (buf + strlen (buf), "#2Level:#0 %d", k->pc->level);
	}
      sprintf (ADD, "\n");
      send_to_char (buf, ch);
    }

  sprintf (buf, "\n#2Clans:#0  [%s]\n", k->clans
	   && *k->clans ? k->clans : "");
  send_to_char (buf, ch);

  sprintf (buf, "#2States:#0 [%s", position_types[GET_POS (k)]);

  if (k->intoxication > 0)
    sprintf (ADD, ", drunk");

  if (!k->hunger)
    sprintf (ADD, ", hungry");

  if (!k->thirst)
    sprintf (ADD, ", thirsty");

  if (!IS_NPC (k))
    {
      switch (k->pc->create_state)
	{
	case STATE_REJECTED:
	  sprintf (ADD, ", REJECTED");
	  break;
	case STATE_APPLYING:
	  sprintf (ADD, ", APPLYING");
	  break;
	case STATE_SUBMITTED:
	  sprintf (ADD, ", SUBMITTED");
	  break;
	case STATE_APPROVED:
	  sprintf (ADD, ", APPROVED");
	  break;
	case STATE_SUSPENDED:
	  sprintf (ADD, ", SUSPENDED");
	  break;
	case STATE_DIED:
	  sprintf (ADD, ", DIED");
	  break;
	}
    }

  if (GET_FLAG (k, FLAG_ENTERING))
    sprintf (ADD, ", ENTERING");

  if (GET_FLAG (k, FLAG_LEAVING))
    sprintf (ADD, ", LEAVING");

  if (GET_FLAG (k, FLAG_OSTLER))
    sprintf (ADD, ", OSTLER");

  sprintf (ADD, "]\n");
  send_to_char (buf, ch);


  if (k->mob && k->mob->access_flags)
    {

      sprintf (buf, "Access: [");

      for (i = 0; *room_bits[i] != '\n'; i++)
	if (IS_SET (k->mob->access_flags, 1 << i))
	  sprintf (buf + strlen (buf), "%s ", room_bits[i]);

      strcat (buf, "]\n");
      send_to_char (buf, ch);
    }

  if (k->mob && k->mob->noaccess_flags)
    {

      sprintf (buf, "Noaccess: [");

      for (i = 0; *room_bits[i] != '\n'; i++)
	if (IS_SET (k->mob->noaccess_flags, 1 << i))
	  sprintf (buf + strlen (buf), "%s ", room_bits[i]);

      strcat (buf, "]\n");
      send_to_char (buf, ch);
    }

  sprintf (buf, "#2Flags:#0  [");

  if (!IS_NPC (k))
    {
      for (i = 0; *player_bits[i] != '\n'; i++)
	if (IS_SET (k->act, 1 << i))
	  sprintf (buf + strlen (buf), " %s", player_bits[i]);

    }
  else
    {
      for (i = 0; *action_bits[i] != '\n'; i++)
	if (IS_SET (k->act, 1 << i))
	  sprintf (buf + strlen (buf), " %s", action_bits[i]);

      if (k->mob->vehicle_type == VEHICLE_BOAT)
	strcat (buf, " Boat");

      else if (k->mob->vehicle_type == VEHICLE_HITCH)
	strcat (buf, " Hitch");
    }

  if (GET_FLAG (k, FLAG_GUEST))
    strcat (buf, " Guest");

  if (IS_SET (k->plr_flags, NOPETITION))
    strcat (buf, " NoPetition");

  if (GET_FLAG (k, FLAG_BRIEF))
    strcat (buf, " Brief");

  if (GET_FLAG (k, FLAG_WIZINVIS))
    strcat (buf, " Wizinvis");

  if (GET_FLAG (k, FLAG_KILL) && k->fighting)
    strcat (buf, " Killing");
  else if (!GET_FLAG (k, FLAG_KILL) && k->fighting)
    strcat (buf, " Hitting");

  if (GET_FLAG (k, FLAG_FLEE))
    strcat (buf, " Fleeing");

  if (GET_FLAG (k, FLAG_BINDING))
    strcat (buf, " Binding");

  if (GET_FLAG (k, FLAG_VARIABLE))
    strcat (buf, " Variable");

  if (GET_FLAG (k, FLAG_PACIFIST))
    strcat (buf, " Pacifist");

  if (IS_SET (k->plr_flags, NEWBIE_HINTS))
    strcat (buf, " Hints");

  if (IS_SET (k->plr_flags, MENTOR))
    strcat (buf, " Mentor");

  if (IS_SET (k->plr_flags, NEWBIE))
    strcat (buf, " Newbie");

  if (IS_SET (k->act, ACT_NOBIND))
    strcat (buf, " NoBind");

  if (IS_SET (k->act, ACT_NOBLEED))
    strcat (buf, " NoBleed");

  if (IS_SET (k->act, ACT_FLYING))
    strcat (buf, " Flying");

  if (IS_SET (k->act, ACT_PHYSICIAN))
    strcat (buf, " Physician");

  if (IS_SET (k->act, ACT_PHYSICIAN))
    strcat (buf, " Auctioneer");
	
  if (GET_FLAG (k, FLAG_SEE_NAME))
    strcat (buf, " See_name");

  if (GET_FLAG (k, FLAG_AUTOFLEE))
    strcat (buf, " Autoflee");

  if (IS_SET (k->act, ACT_PREY))
    strcat (buf, " Prey");

  if (GET_FLAG (k, FLAG_NOPROMPT))
    strcat (buf, " Noprompt");

  if (GET_FLAG (k, FLAG_OSTLER))
    strcat (buf, " Ostler");

  if (GET_FLAG (k, FLAG_ISADMIN))
    strcat (buf, " IsAdminPC");

  if (!IS_NPC (k) && k->pc->is_guide)
    strcat (buf, " IsGuide");

  if (IS_SET (k->plr_flags, NO_PLAYERPORT))
    strcat (buf, " NoPlayerPort");

  if (k->color)
    strcat (buf, " ANSI");

  sprintf (buf + strlen (buf), " ]\n");
  send_to_char (buf, ch);

  if (IS_NPC (k) && k->mob->owner)
    {
      sprintf (buf, "#2Owner:#0  [%s]\n", k->mob->owner);
      send_to_char (buf, ch);
    }

  if (IS_SET (k->act, ACT_ENFORCER))
    {
      sprintf (buf, "#2Jail Location:#0 [ %d ]\n", k->mob->jail);
      send_to_char (buf, ch);
    }

  if (IS_SET (k->act, ACT_JAILER))
    {
      sprintf (buf, "#2Jail Info:#0 [ Cell1: %d Cell2: %d Cell3: %d ].\n",
	       k->cell_1, k->cell_2, k->cell_3);
      send_to_char (buf, ch);
    }

  if (k->poison_type)
    {
      // p = lookup_string (k->poison_type, REG_MAGIC_SPELLS);
      // sprintf (buf, "#2Bite:#0 %s\n", p ? p : "Unknown Poison Type");
      sprintf (buf, "#2Bite:#0 %d\n", k->poison_type);
      send_to_char (buf, ch);
    }

  if (k->desc && k->desc->snoop.snooping &&
      is_he_somewhere (k->desc->snoop.snooping))
    {
      sprintf (buf, "#2Snooping:#0  %s\n",
	       GET_NAME (k->desc->snoop.snooping));
      send_to_char (buf, ch);
    }

  if (k->desc && k->desc->snoop.snoop_by &&
      is_he_somewhere (k->desc->snoop.snoop_by))
    {
      sprintf (buf, "#2Snooped by:#0  %s\n",
	       GET_NAME (k->desc->snoop.snoop_by));
      send_to_char (buf, ch);
    }

  if (k->following && is_he_somewhere (k->following))
    {
      sprintf (buf, "#2Following:#0 %s\n", GET_NAME (k->following));
      send_to_char (buf, ch);
    }

  if (k->affected_by)
    {
      sprintf (buf, "#2Affect:#0 [ ");
      for (i = 0; *affected_bits[i] != '\n'; i++)
	if (IS_SET (k->affected_by, 1 << i))
	  sprintf (buf + strlen (buf), "%s ", affected_bits[i]);
      sprintf (buf + strlen (buf), "]\n");
      send_to_char (buf, ch);
    }

  if (k->venom)
    {
      sprintf (buf, "#2Venoms:#0 ");
      for (poison = k->venom; poison; poison = poison->next)
	{
	  if (poison != k->venom)
	    sprintf (buf + strlen (buf), "        ");
	  sprintf (buf + strlen (buf), "[%s, Duration: %d, Power: %d",
		   somatics[poison->poison_type - 20000],
		   poison->duration, poison->lvl_power);
	  sprintf (buf + strlen (buf), "]\n");
	}
      send_to_char (buf, ch);
    }

  if (k->enchantments)
    {
      sprintf (buf, "------------ Enchantments ------------\n");
      for (ench = k->enchantments; ench; ench = ench->next)
	sprintf (buf + strlen (buf), "%s: %d hours remaining\n", ench->name,
		 ench->current_hours);
      sprintf (buf + strlen (buf),
	       "--------------------------------------\n\n");
      send_to_char (buf, ch);
    }

  for (af = k->hour_affects; af; af = next_affect)
    {

      if (af == k->hour_affects)
	send_to_char ("\n", ch);

      next_affect = af->next;	/* WatchX affect can delete af */

      if (af->type == AFFECT_SHADOW)
	{

	  if (!af->a.shadow.shadow && af->a.shadow.edge != -1)
	    sprintf (buf, "#2%5d#0   Standing", af->type);

	  else if (!is_he_somewhere (af->a.shadow.shadow))
	    continue;

	  else if (IS_NPC (af->a.shadow.shadow))
	    sprintf (buf, "#2%5d#0   Shadowing %s (%d)", af->type,
		     af->a.shadow.shadow->short_descr,
		     af->a.shadow.shadow->mob->nVirtual);
	  else
	    sprintf (buf, "#2%5d#0   Shadowing PC %s", af->type,
		     GET_NAME (af->a.shadow.shadow));

	  if (af->a.shadow.edge != -1)
	    sprintf (buf + strlen (buf), " on %s edge.",
		     dirs[af->a.shadow.edge]);

	  sprintf (buf + strlen (buf), "\n");

	  send_to_char (buf, ch);

	  continue;
	}

      if (af->type == AFFECT_GUARD_DIR)
	{

	  if (!af->a.shadow.shadow && af->a.shadow.edge != -1)
	    sprintf (buf, "#2%5d#0   Guarding", af->type);

	  sprintf (buf + strlen (buf), " the %s exit.",
		   dirs[af->a.shadow.edge]);

	  sprintf (buf + strlen (buf), "\n");

	  send_to_char (buf, ch);

	  continue;
	}

      if (af->type == AFFECT_WATCH_DIR)
	{

	  if (!af->a.shadow.shadow && af->a.shadow.edge != -1)
	    sprintf (buf, "#2%5d#0   Watching", af->type);

	  sprintf (buf + strlen (buf), " the %s exit.",
		   dirs[af->a.shadow.edge]);

	  sprintf (buf + strlen (buf), "\n");

	  send_to_char (buf, ch);

	  continue;
	}

      else if (af->type == MAGIC_SIT_TABLE)
	{
	  sprintf (buf, "#2%5d#0   Sitting at table.\n", af->type);
	  send_to_char (buf, ch);
	  continue;
	}

      else if (af->type >= MAGIC_STABLING_PAID
	       && af->type <= MAGIC_STABLING_LAST)
	{
	  sprintf (buf,
		   "#2%5d#0   Stabling paid for ID %d, %d IC hours remaining.\n",
		   af->type, af->a.spell.sn, af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MUTE_EAVESDROP)
	{
	  sprintf (buf, "#2%5d#0    Muting Eavesdropping\n", af->type);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type >= CRAFT_FIRST && af->type <= CRAFT_LAST)
	{
/*
			sprintf (buf, "#2%5d#0   Craft %s subcraft %s\n",
						  af->type,
						  af->a.craft->subcraft->craft_name,
						  af->a.craft->subcraft->subcraft_name);
			send_to_char (buf, ch);
*/
	  continue;
	}

      if (af->type >= JOB_1 && af->type <= JOB_3)
	{

	  i = time_info.year * 12 * 30 + time_info.month * 30 + time_info.day;
	  i = af->a.job.pay_date - i;

	  vobj = vtoo (af->a.job.object_vnum);

	  if (!vobj)
		value_pay = 0;
	  else
		{
		value_pay = vobj->farthings * af->a.job.count;
		}

	  sprintf (buf,
	  		"#2%5d#0   Job %d:  Pays %.0f coppers with %d of %d days until payday\n", 
	  		af->type,
	  		af->type - JOB_1 + 1,
	  		value_pay,
	  		i,
	  		af->a.job.days);
	  		
	   sprintf (buf + strlen(buf),
	 		"             %d x %s (%d)",
	  		af->a.job.count,
	  		!vobj ? "UNDEFINED" : vobj->short_description,
	  		af->a.job.object_vnum
	  		);

	  if (af->a.job.employer)
		{
		sprintf (buf + strlen(buf),
			"\n             Employer: %s (%d)\n",
			vtom(af->a.job.employer)->short_descr,
			af->a.job.employer);
		}
	  else
		{
		sprintf (buf+ strlen(buf),"\n");
		}

	  send_to_char (buf, ch);
	  continue;
	} //if (af->type >= JOB_1

      if (af->type == MAGIC_CRAFT_BRANCH_STOP)
	{
	  sprintf (buf,
		   "#2%5d#0   May branch a craft again in %d RL minutes.\n",
		   af->type, af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_PETITION_MESSAGE)
	{
	  sprintf (buf,
		   "#2%5d#0   Will not receive petition message again for %d RL minutes.\n",
		   af->type, af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == AFFECT_LOST_CON)
	{
	  sprintf (buf,
		   "#2%5d#0   Will regain %d CON points in %d in-game hours.\n",
		   af->type, af->a.spell.sn, af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == AFFECT_HOLDING_BREATH)
	{
	  sprintf (buf, "#2%5d#0   Can hold breath for %d more RL seconds.\n",
		   af->type, af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == AFFECT_GROUP_RETREAT)
	{
	  sprintf (buf, "#2%5d#0   Retreating in %d more RL seconds.\n",
		   af->type, af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_CRAFT_DELAY)
	{
	  if (time (0) >= af->a.spell.modifier)
	    continue;
	  time_diff = af->a.spell.modifier - time (0);
	  days_remaining = time_diff / (60 * 60 * 24);
	  time_diff %= 60 * 60 * 24;
	  hours_remaining = time_diff / (60 * 60);
	  time_diff %= 60 * 60;
	  mins_remaining = time_diff / 60;
	  time_diff %= 60;
	  sprintf (buf,
		   "#2%5d#0   OOC craft delay timer engaged. [%dd %dh %dm %ds remaining]\n",
		   af->type, days_remaining, hours_remaining, mins_remaining,
		   time_diff);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type >= MAGIC_SKILL_GAIN_STOP
	  && af->type < MAGIC_SPELL_GAIN_STOP)
	{
	  sprintf (buf, "#2%5d#0   May improve %s again in %d RL minutes.\n",
		   af->type, skills[af->type - MAGIC_SKILL_GAIN_STOP],
		   af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type >= MAGIC_FLAG_NOGAIN
	  && af->type <= MAGIC_FLAG_NOGAIN + LAST_SKILL)
	{
	  sprintf (buf,
		   "#2%5d#0   Skill gain in %s has been halted by an administrator.\n",
		   af->type, skills[af->type - 6000]);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type >= MAGIC_FLAG_FOCUS && af->type <= MAGIC_FLAG_FOCUS + LAST_SKILL)
	{
	  sprintf (buf,"#2%5d#0   Focusing upon %s skill.\n",af->type, skills[af->type - 6200]);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_RAISED_HOOD)
	{
	  sprintf (buf,
		   "#2%5d#0   Mob has raised hood due to inclement weather.\n",
		   af->type);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type >= MAGIC_CRIM_BASE && af->type <= MAGIC_CRIM_BASE + 100)
	{
	  sprintf (buf, "#2%5d#0   Wanted in zone %d for %d hours.\n",
		   af->type, af->type - MAGIC_CRIM_BASE,
		   af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_NOTIFY)
	{
	  if (is_he_somewhere ((CHAR_DATA *) af->a.spell.t))
	    {
	      sprintf (buf, "#2%5d#0   Notify pending on %s for %d hours.\n",
		       af->type,
		       GET_NAME ((CHAR_DATA *) af->a.spell.t),
		       af->a.spell.duration);
	      send_to_char (buf, ch);
	    }

	  continue;
	}

      if (af->type == MAGIC_CLAN_NOTIFY)
	{
	  if (is_he_somewhere ((CHAR_DATA *) af->a.spell.t))
	    {
	      sprintf (buf,
		       "#2%5d#0   CLAN Notify pending on %s for %d hours.\n",
		       af->type, GET_NAME ((CHAR_DATA *) af->a.spell.t),
		       af->a.spell.duration);
	      send_to_char (buf, ch);
	    }

	  continue;
	}

      if (af->type == MAGIC_GUARD)
	{
	  if (is_he_somewhere ((CHAR_DATA *) af->a.spell.t))
	    {
	      sprintf (buf, "#2%5d#0   Guarding %s.\n", af->type,
		       GET_NAME ((CHAR_DATA *) af->a.spell.t));
	      send_to_char (buf, ch);
	    }
	  else
	    send_to_char ("   Guarding someone who's left the game.\n", ch);

	  continue;
	}

      if (af->type == MAGIC_WATCH)
	{
	  if (is_he_somewhere ((CHAR_DATA *) af->a.spell.t))
	    {
	      sprintf (buf, "#2%5d#0   Watching %s.\n", af->type,
		       GET_NAME ((CHAR_DATA *) af->a.spell.t));
	      send_to_char (buf, ch);
	    }
	  else
	    send_to_char ("   Watching someone who's left the game.\n", ch);

	  continue;
	}

      if (af->type >= MAGIC_CRIM_HOODED && af->type < MAGIC_CRIM_HOODED + 100)
	{
	  sprintf (buf,
		   "#2%5d#0   Hooded criminal charge in zone %d for %d RL seconds.\n",
		   af->type, af->type - MAGIC_CRIM_HOODED,
		   af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_STARED)
	{
	  sprintf (buf,
		   "#2%5d#0   Studied by an enforcer.  Won't be studied for "
		   "%d RL seconds.\n", af->type, af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_ALERTED)
	{
	  sprintf (buf, "%d   Gave out an alert.  Won't be "
		   "alerting again for %d RL seconds.\n",
		   af->type, af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_WARNED)
	{
	  sprintf (buf,
		   "#2%5d#0   Warned by an enforcer.  Won't be warned for "
		   "%d RL seconds.\n", af->type, af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_HIDDEN)
	{
	  sprintf (buf, "#2%5d#0   Hidden.\n", af->type);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_SNEAK)
	{
	  sprintf (buf, "#2%5d#0   Currently trying to sneak.\n", af->type);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_TOLL)
	{
	  sprintf (buf, "#2%5d#0   TOLL:  Collecting %d for going %s.\n",
		   af->type, af->a.toll.charge, dirs[af->a.toll.dir]);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type >= MAGIC_AFFECT_FIRST && af->type <= MAGIC_AFFECT_LAST)
	{
	  sprintf (buf, "#2%5d#0   Spell effect, %d RL minutes remaining.\n",
		   af->type, af->a.spell.duration);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_TOLL_PAID)
	{
	  sprintf (buf, "#2%5d#0   TOLL PAID to travel %s.\n",
		   af->type, dirs[af->a.toll.dir]);
	  send_to_char (buf, ch);
	  continue;
	}

      if (af->type == MAGIC_WATCH1 ||
	  af->type == MAGIC_WATCH2 || af->type == MAGIC_WATCH3)
	continue;

      if (af->type >= MAGIC_FIRST_SOMA && af->type <= MAGIC_LAST_SOMA)
	{
	  soma_stat (ch, af);
	  continue;
	}

      p = lookup_string (af->type, REG_SPELLS);

      if (!p)
	p = lookup_string (af->type, REG_MAGIC_SPELLS);

      if (!p)
	p = lookup_string (af->type, REG_CRAFT_MAGIC);

      sprintf (buf, "#2%5d#0   %s affects you by %d for %d hours%s.\n",
	       af->type,
	       p == NULL ? "Unknown" : p,
	       af->a.spell.modifier,
	       af->a.spell.duration,
	       af->a.spell.duration < 0 ? " (permanently)" : "");
      send_to_char (buf, ch);
    }

  send_to_char ("\n", ch);

/**** morphing mobs ******/
if ((k->clock > 0) || (k->morphto > 0))
	{
	int month, day, hour;

	month = k->clock / (24 * 30);
	hour = k->clock - month * 24 * 30;

	day = hour / 24;
	hour -= day * 24;

	sprintf (buf,
		"\n#2Morphing Information:#0\n#2Clock:#0   %d %d %d    #2MorphTo:#0 %d    #2Morph Type:#0 %d\n\n",
		month, day, hour, k->morphto, k->morph_type);
	send_to_char (buf, ch);

	if (k->morph_time > 0)
		{
		int delta, days, hours, minutes;

		delta = k->morph_time - time (0);

		days = delta / 86400;
		delta -= days * 86400;

		hours = delta / 3600;
		delta -= hours * 3600;

		minutes = delta / 60;

		sprintf (buf,
			"This mob will morph in %d days, %d hours, and %d minutes\n",
			days, hours, minutes);
		send_to_char (buf, ch);
		}
	}
/**************************/
  if (k->shop && IS_SET (k->flags, FLAG_KEEPER))
    {
      sprintf (buf, "  #2Shop Rm:#0  %5d\n", k->shop->shop_vnum);
      send_to_char (buf, ch);

      sprintf (buf, "  #2Store Rm:#0 %5d\n", k->shop->store_vnum);
      send_to_char (buf, ch);

      sprintf (buf, "  #2Markup:#0    %2.2f                \n",
	       k->shop->markup);
      send_to_char (buf, ch);

      if (IS_SET (k->act, ACT_ECONZONE))
	{
	  send_to_char ("   #3NOTE:#0  #5ECONZONE flag overrides standard "
			"markups and discounts.#0\n", ch);

	  econ_zone = zone_to_econ_zone (ch->room->zone);

	  if (econ_zone != -1)
	    {

	      sprintf (buf, "          Markup/Discounts are those paid in "
		       "your zone:  %d (%s)\n",
		       ch->room->zone,
		       default_econ_info[econ_zone].flag_name);
	      send_to_char (buf, ch);

	      *buf = '\0';

	      for (i = 0; *default_econ_info[i].flag_name != '\n'; i++)
		{

		  if ((i % 3) == 0)
		    {
		      if (*buf)
			{
			  send_to_char (buf, ch);
			  send_to_char ("\n", ch);
			}
		      strcpy (buf, "     ");
		    }

		  else if ((i % 3) == 1)
		    pad_buffer (buf, 28);

		  else
		    pad_buffer (buf, 53);

		  sprintf (buf + strlen (buf), "  %-10.10s: %1.2f/%1.2f",
			   default_econ_info[i].flag_name,
			   default_econ_info[i].obj_econ_info[econ_zone].
			   markup,
			   default_econ_info[i].obj_econ_info[econ_zone].
			   discount);
		}

	      if (*buf)
		{
		  send_to_char (buf, ch);
		  send_to_char ("\n", ch);
		}
	    }
	}

      else
	{
          if(IS_SET(k->act, ACT_BULKTRADER))
            bulktrader = true;

	  sprintf (buf, "  #2Discount:#0  %2.2f\n", k->shop->discount);
	  send_to_char (buf, ch);

	  sprintf (buf, "  #2Econ Markup1:#0 %2.2f", k->shop->econ_markup1);
	  pad_buffer (buf, 23);
	  sprintf (ADD, "   #2Discount1:#0 %2.2f", k->shop->econ_discount1);
	  pad_buffer (buf, 40);

          bulktrader ? sprintf (ADD, "  #2Res1:#0 [") : sprintf (ADD, "  #2Econ1:#0 [");

	  for (i = 0; str_cmp (econ_flags[i], "\n"); i++)
	    {
	      if (IS_SET (k->shop->econ_flags1, 1 << i))
		{
                  sprintf (buf + strlen (buf), 
                  (bulktrader ? resources[i] : econ_flags[i]));
		  sprintf (buf + strlen (buf), " ");
		}
	    }

	  if (buf[strlen (buf) - 1] != '[')
	    buf[strlen (buf) - 1] = ']';
	  else
	    sprintf (buf + strlen (buf), "]");

	  sprintf (buf + strlen (buf), "\n");
	  send_to_char (buf, ch);

	  sprintf (buf, "  #2Econ Markup2:#0 %2.2f", k->shop->econ_markup2);
	  pad_buffer (buf, 23);
	  sprintf (ADD, "   #2Discount2:#0 %2.2f", k->shop->econ_discount2);
	  pad_buffer (buf, 40);
          bulktrader ? sprintf (ADD, "  #2Res2:#0 [") : sprintf (ADD, "  #2Econ2:#0 [");

	  for (i = 0; str_cmp (econ_flags[i], "\n"); i++)
	    {
	      if (IS_SET (k->shop->econ_flags2, 1 << i))
		{
                  sprintf (buf + strlen (buf), 
                  (bulktrader ? resources[i] : econ_flags[i]));
		  sprintf (buf + strlen (buf), " ");
		}
	    }

	  if (buf[strlen (buf) - 1] != '[')
	    buf[strlen (buf) - 1] = ']';
	  else
	    sprintf (buf + strlen (buf), "]");

	  sprintf (buf + strlen (buf), "\n");
	  send_to_char (buf, ch);

	  sprintf (buf, "  #2Econ Markup3:#0 %2.2f", k->shop->econ_markup3);
	  pad_buffer (buf, 23);
	  sprintf (ADD, "   #2Discount3:#0 %2.2f", k->shop->econ_discount3);
	  pad_buffer (buf, 40);
          bulktrader ? sprintf (ADD, "  #2Res3:#0 [") : sprintf (ADD, "  #2Econ3:#0 [");

	  for (i = 0; str_cmp (econ_flags[i], "\n"); i++)
	    {
	      if (IS_SET (k->shop->econ_flags3, 1 << i))
		{
                  sprintf (buf + strlen (buf), 
                  (bulktrader ? resources[i] : econ_flags[i]));
		  sprintf (buf + strlen (buf), " ");
		}
	    }

	  if (buf[strlen (buf) - 1] != '[')
	    buf[strlen (buf) - 1] = ']';
	  else
	    strcat (buf, "]");

	  strcat (buf, "\n");
	  send_to_char (buf, ch);

	  sprintf (buf, "  #2Econ Markup4:#0 %2.2f", k->shop->econ_markup4);
	  pad_buffer (buf, 23);
	  sprintf (ADD, "   #2Discount4:#0 %2.2f", k->shop->econ_discount4);
	  pad_buffer (buf, 40);
          bulktrader ? sprintf (ADD, "  #2Res4:#0 [") : sprintf (ADD, "  #2Econ4:#0 [");

	  for (i = 0; str_cmp (econ_flags[i], "\n"); i++)
	    {
	      if (IS_SET (k->shop->econ_flags4, 1 << i))
		{
                  sprintf (buf + strlen (buf), 
                  (bulktrader ? resources[i] : econ_flags[i]));
		  strcat (buf, " ");
		}
	    }

	  if (buf[strlen (buf) - 1] != '[')
	    buf[strlen (buf) - 1] = ']';
	  else
	    strcat (buf, "]");

	  strcat (buf, "\n");
	  send_to_char (buf, ch);


	  sprintf (buf, "  #2Econ Markup5:#0 %2.2f", k->shop->econ_markup5);
	  pad_buffer (buf, 23);
	  sprintf (ADD, "   #2Discount5:#0 %2.2f", k->shop->econ_discount5);
	  pad_buffer (buf, 40);
          bulktrader ? sprintf (ADD, "  #2Res5:#0 [") : sprintf (ADD, "  #2Econ5:#0 [");

	  for (i = 0; str_cmp (econ_flags[i], "\n"); i++)
	    {
	      if (IS_SET (k->shop->econ_flags5, 1 << i))
		{
                  sprintf (buf + strlen (buf), 
                  (bulktrader ? resources[i] : econ_flags[i]));
		  strcat (buf, " ");
		}
	    }

	  if (buf[strlen (buf) - 1] != '[')
	    buf[strlen (buf) - 1] = ']';
	  else
	    strcat (buf, "]");

	  strcat (buf, "\n");
	  send_to_char (buf, ch);


	  sprintf (buf, "  #2Econ Markup6:#0 %2.2f", k->shop->econ_markup6);
	  pad_buffer (buf, 23);
	  sprintf (ADD, "   #2Discount6:#0 %2.2f", k->shop->econ_discount6);
	  pad_buffer (buf, 40);
          bulktrader ? sprintf (ADD, "  #2Res6:#0 [") : sprintf (ADD, "  #2Econ6:#0 [");

	  for (i = 0; str_cmp (econ_flags[i], "\n"); i++)
	    {
	      if (IS_SET (k->shop->econ_flags6, 1 << i))
		{
                  sprintf (buf + strlen (buf), 
                  (bulktrader ? resources[i] : econ_flags[i]));
		  strcat (buf, " ");
		}
	    }

	  if (buf[strlen (buf) - 1] != '[')
	    buf[strlen (buf) - 1] = ']';
	  else
	    strcat (buf, "]");

	  strcat (buf, "\n");
	  send_to_char (buf, ch);


	  sprintf (buf, "  #2Econ Markup7:#0 %2.2f", k->shop->econ_markup7);
	  pad_buffer (buf, 23);
	  sprintf (ADD, "   #2Discount7:#0 %2.2f", k->shop->econ_discount7);
	  pad_buffer (buf, 40);
          bulktrader ? sprintf (ADD, "  #2Res7:#0 [") : sprintf (ADD, "  #2Econ7:#0 [");

	  for (i = 0; str_cmp (econ_flags[i], "\n"); i++)
	    {
	      if (IS_SET (k->shop->econ_flags7, 1 << i))
		{
                  sprintf (buf + strlen (buf), 
                  (bulktrader ? resources[i] : econ_flags[i]));
		  strcat (buf, " ");
		}
	    }

	  if (buf[strlen (buf) - 1] != '[')
	    buf[strlen (buf) - 1] = ']';
	  else
	    strcat (buf, "]");

	  strcat (buf, "\n");
	  send_to_char (buf, ch);

	}

      sprintf (buf, "  #2Buy Flags:#0    [");

      for (i = 0; str_cmp (econ_flags[i], "\n"); i++)
	{
	  if (IS_SET (k->shop->buy_flags, 1 << i))
	    {
	      strcat (buf, (IS_SET(k->act, ACT_BULKTRADER) ? resources[i] : econ_flags[i]));
	      strcat (buf, " ");
	    }
	}

      if (buf[strlen (buf) - 1] != '[')
	buf[strlen (buf) - 1] = ']';
      else
	strcat (buf, "]");

      strcat (buf, "\n");
      send_to_char (buf, ch);

      sprintf (buf, "  #2NoBuy Flags:#0  [");

      for (i = 0; str_cmp (econ_flags[i], "\n"); i++)
	{
	  if (IS_SET (k->shop->nobuy_flags, 1 << i))
	    {
	      strcat (buf, (IS_SET(k->act, ACT_BULKTRADER) ? resources[i] : econ_flags[i]));
	      strcat (buf, " ");
	    }
	}

      if (buf[strlen (buf) - 1] != '[')
	buf[strlen (buf) - 1] = ']';
      else
	strcat (buf, "]");

      strcat (buf, "\n");
      send_to_char (buf, ch);

      sprintf (buf, "  #2Trades in:#0    [");

      for (i = 0; i < MAX_TRADES_IN; i++)
	if (k->shop->trades_in[i])
	  sprintf (ADD, "%s ", item_types[k->shop->trades_in[i]]);

      sprintf (ADD, "]\n");
      send_to_char (buf, ch);

      *buf2 = '\0';

      for (i = 0; *materials[i] != '\n'; i++)
	{
	  if (IS_SET (k->shop->materials, (1 << i)))
	    sprintf (buf2 + strlen (buf2), "%s ", materials[i]);
	}

      if (*buf2)
	buf2[strlen (buf2) - 1] = ']';
      else
	sprintf (buf2, "]");

      sprintf (buf, "  #2Materials:#0    [%s\n\n", buf2);
      send_to_char (buf, ch);

      if (k->shop->delivery[0])
	{
	  sprintf (buf, "#2Deliveries:#0\n   ");
	  j = 1;
	  for (i = 0; i <= MAX_DELIVERIES; i++)
	    {
	      if (!k->shop->delivery[i])
		break;
	      sprintf (buf + strlen (buf), "#2%3d:#0 %-8d ", i,
		       k->shop->delivery[i]);
	      if (!(j % 5))
		strcat (buf, "\n   ");
	      j++;
	    }
	  strcat (buf, "\n\n");
	  send_to_char (buf, ch);
	}
    }

  i2 = 1;
  sprintf (buf, "#2%8.8s:#0 %03d/%03d  %s", "Offense", k->offense,
	   k->offense, i2 % 4 ? "" : "\n");
  send_to_char (buf, ch);

  for (i = 0; i < MAX_SKILLS; i++)
    {
      if (k->skills[i] && real_skill (k, i))
	{
	  i2++;
	  sprintf (buf, "#2%8.8s:#0 %03d/%03d  %s", skills[i], real_skill (k, i),
		   calc_lookup (k, REG_CAP, i), i2 % 4 ? "" : "\n");
	  send_to_char (buf, ch);
	}
    }

  if (buf[strlen (buf) - 1] != '\n')
    send_to_char ("\n", ch);

  if ((k->plan && !k->plan->empty()) || (k->goal && !k->goal->empty()))
    {
      buf[0] = 0;
      if (k->goal)
	sprintf (buf, "\n#2Long-term Goal:#0\n   %s\n", k->goal->c_str());

      if (k->plan)
	sprintf (buf + strlen (buf), "\n#2Current Plan:#0\n%s\n", k->plan->c_str());
      
      send_to_char (buf, ch);

    }
  if (IS_NPC(k) && k->mob->cues && !k->mob->cues->empty())
    {

      const char * cues [] = 
	{
	  "none", "notes", "flags", "memory",
	  "on_hour", "on_time",
	  "on_health", "on_moves",
	  "on_command", "on_receive", "on_hear", "on_nod",
	  "on_theft", "on_witness",
	  "on_engage", "on_flee", "on_scan", "on_hit",
          "on_reboot", "mob_present", "obj_present"
	};

      strcpy (buf, "\n#2Mobile Cues:#0\n");

      typedef std::multimap<mob_cue,std::string>::const_iterator N;
      for (N n = k->mob->cues->begin(); n != k->mob->cues->end(); n++)
	{
	  std::string cue_string = n->second;
	  if (!cue_string.empty ())
	    {
	      cue_string = std::string (cues[n->first]) + std::string (" ") 
		+ cue_string + "\n";
	      strcat (buf, cue_string.c_str ());
	    }
	}
      send_to_char (buf, ch);
    }


  if (loaded_char)
    unload_pc (k);
}

void
roomstat (CHAR_DATA * ch, char *name)
{
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  ROOM_DATA *rm;
  CHAR_DATA *k;
  OBJ_DATA *j;
  EXTRA_DESCR_DATA *desc;
  int i;
  AFFECTED_TYPE *herbed;


  rm = vtor (ch->in_room);
  sprintf (buf, "[%4d]: %s\n", rm->nVirtual, rm->name);
  send_to_char (buf, ch);

  sprinttype (rm->sector_type, sector_types, buf2);
  sprintf (buf, "Sector type[%s] ", buf2);
  send_to_char (buf, ch);

  sprintbit ((long) rm->room_flags, room_bits, buf2);
  sprintf (buf, "Rflags[%s]\n", buf2);
  send_to_char (buf, ch);

  if (rm->xerox > 0)
  	{
	  sprintf (buf, "Prototype: %d\n", rm->xerox);
    	  send_to_char (buf, ch);
	}

  if (rm->capacity > 0)
  	{
	  sprintf (buf, "Capacity: %d\n", rm->capacity);
    	  send_to_char (buf, ch);
	}
		
  //send_to_char ("Description:\n", ch);
  //send_to_char (rm->description, ch);
  //send_to_char ("\n", ch);
  if (rm->ex_description)
    {
    send_to_char ("Ex-desc keyword(s):\n", ch);
      for (desc = rm->ex_description; desc; desc = desc->next)
	{
	  sprintf (buf, "[%s] %s", desc->keyword, desc->description);
	  send_to_char (buf, ch);
	  }
    send_to_char ("\n", ch);
    }

  herbed = is_room_affected (rm->affects, HERBED_COUNT);

  if (!herbed)
    send_to_char ("\nThis room has not been herbed.\n\n", ch);
  else
    {
      sprintf (buf2, "This room has been herbed %d times (Duration = %d)\n\n",
	       herbed->a.herbed.timesHerbed, herbed->a.herbed.duration);
      send_to_char (buf2, ch);
    }

  send_to_char ("------- Chars present -------\n", ch);
  for (k = rm->people; k; k = k->next_in_room)
    {
      if (IS_NPC (k))
	act ("NPC: $N", false, ch, 0, k, TO_CHAR);
      else
	act ("PC:  $N", false, ch, 0, k, TO_CHAR);
    }
  send_to_char ("\n", ch);

  send_to_char ("--------- Contents ---------\n", ch);
  for (j = rm->contents; j; j = j->next_content)
    {
      sprintf (buf, "%s\n", j->name);
      send_to_char (buf, ch);
    }
  send_to_char ("\n", ch);

  send_to_char ("------- Exits defined -------\n", ch);
  for (i = 0; i <= LAST_DIR; i++)
    {
      if (rm->dir_option[i])
	{
	  sprintf (buf, "#3Direction %s . Keyword : %s#0\n",
		   dirs[i], rm->dir_option[i]->keyword);
	  send_to_char (buf, ch);
	  strcpy (buf, "Description:\n  ");
	  
	  if (rm->dir_option[i]->general_description)
	    strcat (buf, rm->dir_option[i]->general_description);
	  else
	    strcat (buf, "UNDEFINED\n");
	  send_to_char (buf, ch);
	  
	  sprintbit (rm->dir_option[i]->exit_info, exit_bits, buf2);
	  sprintf (buf,
		   "Exit flag: %s \nKey no: %d  (pick penalty %d)\nTo room (Virtual: %d)\n\n",
		   buf2, rm->dir_option[i]->key,
		   rm->dir_option[i]->pick_penalty,
		   rm->dir_option[i]->to_room);
	  send_to_char (buf, ch);
		}//if (rm->dir_option[i])
	  
	  if (rm->secrets[i])
		{
		sprintf(buf,
			"Secret: %s  %d percent difficulty\n",
			rm->secrets[i]->stext,
			rm->secrets[i]->diff);
		send_to_char (buf, ch);
		}
    }//for (i = 0; i <= LAST_DIR; i++)
}

/*                                                                          *
 * funtion: acctstat                    < e.g.> stat acct traithe           *
 *                                                                          *
 * 09/17/2004 [JWW] - Ensured that (result != NULL) in a few spots          *
 *                  - was freeing the static char[] that asctime() returns! *
 *                    now switched to using alloc, asctime_r, and mem_free  *
 *                  - ordered the statement execution to match output flow  *
 *                                                                          */
void
acctstat (CHAR_DATA * ch, char *name)
{
  account *acct;
  char *date;
  time_t account_time;
  char strTime[AVG_STRING_LENGTH] = "";
  char buf[MAX_STRING_LENGTH];
  char state[MAX_STRING_LENGTH];
  char line[MAX_STRING_LENGTH];
  MYSQL_RES *result;
  MYSQL_ROW row = NULL;
  int messages = 0, nDays = 0, nHours = 0, nColor = 0;

  if (!*name || atoi (name))
    {
      send_to_char ("Please specify an account name.\n", ch);
      return;
    }
  acct = new account (name);
  if (!acct->is_registered ())
    {
      delete acct;
      send_to_char ("That account does not exist.\n", ch);
      return;
    }


  *buf = '\0';
  *line = '\0';

  sprintf (buf, "\n   #2Account Name:#0             %s\n", acct->name.c_str ());
  sprintf (buf + strlen (buf), "   #2Registered Email:#0         %s\n",
	   acct->email.c_str ());
  sprintf (buf + strlen (buf), "   #2Last Logged In From:#0      %s\n",
	   acct->last_ip.c_str ());

  /* Retreive a list of other users from this IP ( default = no-op ) */
  mysql_safe_query
    ("SELECT username FROM forum_users WHERE user_last_ip = '%s' AND username != '%s'",
     acct->last_ip.c_str (), acct->name.c_str ());
  if ((result = mysql_store_result (database)) != NULL)
    {
      if (!IS_SET (acct->flags, ACCOUNT_IPSHARER)
	  && mysql_num_rows (result) > 0 && str_cmp (acct->name.c_str (), "Guest")
	  && acct->last_ip.compare ("(null)") != 0
	  && acct->last_ip.find ("middle-earth.us") == std::string::npos)
	{
	  sprintf (buf + strlen (buf), "   #1Accounts Sharing IP:#0     ");
	  while ((row = mysql_fetch_row (result)))
	    {
	      sprintf (buf + strlen (buf), " %s", row[0]);
	    }
	  sprintf (buf + strlen (buf), "#0\n");
	}
      mysql_free_result (result);
      result = NULL;
    }
  else
    {
      system_log
	("Warning: MySql returned a NULL result in staff.c::acctstat() while fetching account sharing info.",
	 true);
    }

  /* Retreive the timestamp for Last Activity ( default = "" ) */
  date = (char *) alloc (256, 31);
  date[0] = '\0';
  std::string player_db = engine.get_config ("player_db");
  mysql_safe_query
    ("SELECT lastlogon"
     " FROM %s.pfiles"
     " WHERE account = '%s'"
     " ORDER BY lastlogon DESC LIMIT 1",
     player_db.c_str (), acct->name.c_str ());
  if ((result = mysql_store_result (database)) != NULL)
    {
      row = mysql_fetch_row (result);
      if (row && *row[0])
	{
	  account_time = (time_t) atoi (row[0]);
	  if (asctime_r (localtime (&account_time), date) != NULL)
	    {
	      date[strlen (date) - 1] = '\0';	/* chop the newline asctime_r tacks on */
	    }
	}
      mysql_free_result (result);
      result = NULL;
    }
  else
    {
      system_log
	("Warning: MySql returned a NULL result in staff.c::acctstat() while fetching lastlogin info.",
	 true);
    }
  sprintf (buf + strlen (buf), "   #2Last PC Activity On:#0      %s\n",
	   (date[0]) ? date : "None Recorded");
  mem_free (date);

  /* Account Registration Date ( default = "None Recorded" ) */
  date = (char *) alloc (256, 31);
  date[0] = '\0';
  account_time = acct->created_on;
  asctime_r (localtime (&account_time), date);
  sprintf (buf + strlen (buf), "   #2Account Registered On:#0    %s",
	   (date[0]) ? date : "None Recorded");
  mem_free (date);

  sprintf (buf + strlen (buf), "   #2Accrued Reward Points:#0  %d\n",
	   acct->get_rpp ());

  if (acct->get_last_rpp_date () > 0)
    {
      std::string date = acct->get_last_rpp_date_string ();
      sprintf (buf + strlen (buf), "   #2Last RP Award On:#0        %s\n",
	       date.c_str ());
    }

  if (acct->get_last_nominate () > 0)
    {
      std::string date = acct->get_nominate_rpp_date_string ();
      sprintf (buf + strlen (buf), "   #2Last Nomination Made On:#0        %s\n",
	       date.c_str ());
    }

  /* Retreive the number of stored Hobbitmails ( default = 0 ) */
  messages = 0;
  mysql_safe_query ("SELECT COUNT(*) FROM hobbitmail WHERE account = '%s'",
		    acct->name.c_str ());
  if ((result = mysql_store_result (database)) != NULL)
    {
      messages = ((row = mysql_fetch_row (result)) != NULL)
	? atoi (row[0]) : 0;
      mysql_free_result (result);
      result = NULL;
    }
  else
    {
      system_log
	("Warning: MySql returned a NULL result in staff.c::acctstat() while fetching hmail count",
	 true);
    }
  sprintf (buf + strlen (buf), "   #2Stored Hobbitmails:#0       %d\n",
	   messages);

  sprintf (buf + strlen (buf), "   #2Web Forum Posts:#0          %d\n",
	   acct->forum_posts);

  if (acct->flags)
    {
      sprintf (buf + strlen (buf), "   #2Account Flags Set:#0       ");
      if (IS_SET (acct->flags, ACCOUNT_NOPETITION))
	sprintf (buf + strlen (buf), " NoPetition");
      if (IS_SET (acct->flags, ACCOUNT_NOGUEST))
	sprintf (buf + strlen (buf), " NoGuest");
      if (IS_SET (acct->flags, ACCOUNT_NOPSI))
	sprintf (buf + strlen (buf), " NoPsi");
      if (IS_SET (acct->flags, ACCOUNT_NOBAN))
	sprintf (buf + strlen (buf), " NoBan");
      if (IS_SET (acct->flags, ACCOUNT_NORETIRE))
	sprintf (buf + strlen (buf), " NoRetire");
      if (IS_SET (acct->flags, ACCOUNT_NOVOTE))
	sprintf (buf + strlen (buf), " NoVoteReminder");
      if (IS_SET (acct->flags, ACCOUNT_IPSHARER))
	sprintf (buf + strlen (buf), " IPSharingOK");
      if (IS_SET (acct->flags, ACCOUNT_RPPDISPLAY))
	sprintf (buf + strlen (buf), " RPPDisplay");
      sprintf (buf + strlen (buf), "\n");
    }
  sprintf (buf + strlen (buf), "   #2ANSI Color Setting:#0       %s\n",
	   acct->color ? "On" : "Off");
  sprintf (buf + strlen (buf), "   #2Downloaded Sourcecode:#0    %s\n",
	   acct->code ? "Yes" : "No");
  sprintf (buf + strlen (buf), "   #2Newsletter Status:#0        %s\n",
	   acct->newsletter ? "Subscribed" : "Opted Out");

  mysql_safe_query
    ("SELECT name,create_state,sdesc,played"
     " FROM %s.pfiles"
     " WHERE account = '%s'"
     " ORDER BY birth ASC",
     player_db.c_str (),
     acct->name.c_str ());
  result = mysql_store_result (database);

  if (result && mysql_num_rows (result) > 0)
    {
      sprintf (buf + strlen (buf), "\n");
      sprintf (buf + strlen (buf), "   #2Registered Characters:#0\n");
      while ((row = mysql_fetch_row (result)))
	{
	  if (strlen (buf) + strlen (line) >= MAX_STRING_LENGTH)
	    break;
	  if (atoi (row[1]) < 1)
	    {
	      sprintf (state, "#3(Pending)#0");
	      nColor = 3;
	    }
	  else if (atoi (row[1]) == 1)
	    {
	      sprintf (state, "#6(Submitted)#0");
	      nColor = 6;
	    }
	  else if (atoi (row[1]) == 2)
	    {
	      sprintf (state, "#2(Active)#0");
	      nColor = 2;
	    }
	  else if (atoi (row[1]) == 3)
	    {
	      sprintf (state, "#5(Suspended)#0");
	      nColor = 5;
	    }
	  else
	    {
	      sprintf (state, "#1(Deceased)#0");
	      nColor = 1;
	    }

	  nDays = (strtol (row[3], NULL, 10) / (60 * 60 * 24));
	  nHours = (strtol (row[3], NULL, 10) % (60 * 60 * 24));
	  nHours = (nHours / (60 * 60));

	  if (nDays > 0)
	    sprintf (strTime, "%3dd", nDays);
	  else
	    strTime[0] = '\0';

	  sprintf (strTime + strlen (strTime), " %2dh", nHours);
	  if (!nDays && !nHours)
	    strcpy (strTime, "<  1h");

	  sprintf (line, "   #%d%-15s %8s#0  #5%-42s\n", nColor, row[0],
		   strTime, row[2]);


	  if (strlen (row[2]) > 32)
	    {
	      sprintf (line + 68, "...#0 %s\n", state);
	    }
	  else
	    {
	      sprintf (line + 71, "#0 %s\n", state);
	    }
	  sprintf (buf + strlen (buf), "%s", line);
	}
    }

  if (result)
    mysql_free_result (result);

  delete acct;

  page_string (ch->desc, buf);
}

void
objstat (CHAR_DATA * ch, char *name)
{
  OBJ_DATA *j;
  OBJ_DATA *j2;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char farthing_buf[MAX_STRING_LENGTH];
  char *sp = '\0';
  char *bow_type[] = { "War Bow", "Hunting Bow", "Sling" };
  int i;
  int instance = 0;
  float cost = 0;
  AFFECTED_TYPE *af;
  POISON_DATA *poison;

  if (!*name && !atoi (name))
    {
      send_to_char ("Please specifiy an object name or vnum.\n", ch);
      return;
    }

  if (just_a_number (name) && vtoo (atoi (name)))
    j = vtoo (atoi (name));

  else if ((j = get_obj_in_list_vis (ch, name, ch->right_hand)) ||
	   (j = get_obj_in_list_vis (ch, name, ch->left_hand)) ||
	   (j = get_obj_in_list_vis (ch, name, ch->room->contents)))
    instance = 1;

  else if (!IS_NPC (ch) && !*name)
    j = vtoo (ch->pc->edit_obj);

  if (!j)
    {
      send_to_char ("No such object.\n", ch);
      return;
    }

  if (!instance)
    sprintf (buf2, " [%d instances]", j->instances);
  else
    sprintf (buf2, " [id %d]", j->coldload_id);

  sprintf (buf, "\n#2VNUM:#0  %d [%s] [#2%s#0]%s\n", j->nVirtual,
	   instance ? "#2Instance#0" : "#2Prototype#0",
	   item_types[(int) GET_ITEM_TYPE (j)], buf2);

  sprintf (buf + strlen (buf), "#2Keys: #0 %s\n", j->name);
  send_to_char (buf, ch);

  sprintf (buf, "#2SDesc:#0 %s\n#2LDesc:#0 %s\n", j->short_description,
	   j->description);
  send_to_char (buf, ch);

  /*
  if(!instance)
  {
    sprintf (buf, "#2Created by:#0 %s", j->creater);
    pad_buffer (buf, 45);
    sprintf(buf + strlen(buf), "#2Lasted Edited by:#0 %s\n", j->editer);
    reformat_string (buf, &sp);
    sprintf (buf, "%s", sp);
    mem_free (sp);
    sp = NULL;
    send_to_char (buf, ch);
 
  }
  */

  if(instance)
  {
    sprintf (buf, "#2Omote:#0 %s\n\n", (j->omote_str) ? j->omote_str : "None");
    reformat_string (buf, &sp);
    sprintf (buf, "%s", sp);
    mem_free (sp);
    sp = NULL;
    send_to_char (buf, ch);
  }

  


  if (j->clan_data) // owned by only 1 clan
  	{
  	sprintf (buf, "#2Clans:#0    ");
  	sprintf (buf + strlen (buf), "'%s' %s  ",
  		j->clan_data->name,
  		j->clan_data->rank);
 
  		strcat (buf, "\n");
      	send_to_char (buf, ch);
  	}

  if (j->obj_flags.type_flag == ITEM_INK)
    {
      sprintf (buf, "#2Ink String:#0  %s\n", j->ink_color);
      send_to_char (buf, ch);
    }

  if (j->obj_flags.wear_flags)
    {
      sprintf (buf, "#2Wearbits:#0    ");
      for (i = 0; (*wear_bits[i] != '\n'); i++)
	if (IS_SET (j->obj_flags.wear_flags, (1 << i)))
	  sprintf (buf + strlen (buf), "%s ", wear_bits[i]);
      strcat (buf, "\n");
      send_to_char (buf, ch);
    }

  if (j->obj_flags.bitvector)
    {
      sprintf (buf, "#2Bitvector:#0   ");
      for (i = 0; (*affected_bits[i] != '\n'); i++)
	if (IS_SET (j->obj_flags.bitvector, (1 << i)))
	  sprintf (buf + strlen (buf), "%s ", affected_bits[i]);
      strcat (buf, "\n");
      send_to_char (buf, ch);
    }

  if (j->obj_flags.extra_flags)
    {
      sprintf (buf, "#2Extra Flags:#0 ");
      for (i = 0; (*extra_bits[i] != '\n'); i++)
	if (IS_SET (j->obj_flags.extra_flags, (1 << i)))
	  sprintf (buf + strlen (buf), "%s ", extra_bits[i]);
      if (j->activation)
	sprintf (buf + strlen (buf), "; #2Activation:#0 %s\n",
		 lookup_string (j->activation, REG_MAGIC_SPELLS));
      else
	strcat (buf, "\n");
      send_to_char (buf, ch);
    }

  if (j->econ_flags)
  {
    if(GET_ITEM_TYPE(j) == ITEM_RESOURCE)
    {
      sprintf (buf, "#2Resource Flags:#0  ");
      for (i = 0; str_cmp (resources[i], "\n"); i++)
	{
	  if (IS_SET (j->econ_flags, 1 << i))
	    {
	      strcat (buf, resources[i]);
	      strcat (buf, " ");
	    }
	}
      strcat (buf, "\n");
      send_to_char (buf, ch);
    }
    else
    {
      sprintf (buf, "#2Econ Flags:#0  ");
      for (i = 0; str_cmp (econ_flags[i], "\n"); i++)
	{
	  if (IS_SET (j->econ_flags, 1 << i))
	    {
	      strcat (buf, econ_flags[i]);
	      strcat (buf, " ");
	    }
	}
      strcat (buf, "\n");
      send_to_char (buf, ch);
    }
  }

  *buf = 0;

  if (j->size)
    {
      if (IS_WEARABLE (j))
	sprintf (buf, "#2Size:#0 %s (%s)\n",
		 sizes_named[j->size], sizes[j->size]);
      else
	sprintf (buf, "#2Size:#0 %s (%s) #1[N/A]#0\n",
		 sizes_named[j->size], sizes[j->size]);
    }

  else if (IS_WEARABLE (j))
    sprintf (buf, "#2Size:#0 Any\n");

  if (*buf)
    send_to_char (buf, ch);

  if (GET_ITEM_TYPE (j) == ITEM_FLUID)
    sprintf (farthing_buf,
	     "%d.%02df (%d)", (int) j->farthings / 100,
	     (int) j->farthings % 100, (int) j->farthings);
  else
    sprintf (farthing_buf, "%df", (int) j->farthings);

  cost = j->farthings + j->silver * 4;

  sprintf (buf, "#2Weight:#0      %d.%02d", j->obj_flags.weight / 100,
	   j->obj_flags.weight % 100);
  pad_buffer (buf, 32);
  sprintf (buf + strlen (buf), "#2Full Weight:#0  %d.%02d",
	   OBJ_MASS (j) / 100, OBJ_MASS (j) % 100);
  pad_buffer (buf, 61);
  sprintf (buf + strlen (buf), "#2Timer:#0  %d\n", j->obj_timer);
  send_to_char (buf, ch);

  sprintf (buf, "#2Cost:#0        %.02f cp", cost);
  pad_buffer (buf, 32);
  sprintf (buf + strlen (buf), "#2Quality:#0 %d", j->quality);
  pad_buffer (buf, 61);
  sprintf (buf + strlen (buf), "#2Cond.:#0  %d%%\n", j->item_wear);

  send_to_char (buf, ch);

  sprintf (buf, "#2Room-Pos:#0        %d\n", j->room_pos);
  send_to_char (buf, ch);

  switch (j->obj_flags.type_flag)
    {
    case ITEM_FLUID:
      sprintf (buf, "#2Oval0 - Alcohol:#0 %d per sip\n"
	       "#2Oval1 - Water:#0   %d per sip\n"
	       "#2Oval2 - Food:#0    %d per sip\n",
	       j->o.fluid.alcohol, j->o.fluid.water, j->o.fluid.food);
      break;

    case ITEM_LIGHT:

      if ((j2 = vtoo (j->o.light.liquid)))
	{
	  sprintf (buf2, "%d (%s)", j2->nVirtual, j2->short_description);
	  if (GET_ITEM_TYPE (j2) != ITEM_LIQUID_FUEL)
	    sprintf (buf2 + strlen (buf2), " [Need %s; this is %s]\n",
		     item_types[ITEM_LIQUID_FUEL],
		     item_types[(int) GET_ITEM_TYPE (j2)]);
	}

      else if (j->o.light.v4 < 33)
	{
	  sprinttype (j->o.light.v4, drinks, buf2);
	  strcat (buf2, " [NEEDS OBJECT ASSIGNED]");
	}

      else
	sprintf (buf2, "%d (NO SUCH OBJECT EXISTS)", j->o.light.liquid);

      sprintf (buf, "#2Oval0 - Capacity:#0 %d\n"
	       "#2Oval1 - Hours:#0    %d\n"
	       "#2Oval2 - Liquid:#0   %s\n"
	       "#2Oval3 - On:#0       %d (0 is no, 1 is yes)\n",
	       j->o.light.capacity, j->o.light.hours, buf2, j->o.light.on);
      break;

    case ITEM_FOOD:
      sprintf (buf, "#2Oval0 - Food Value:#0 %d\n"
	       "#2Oval1 - Spell:#0 %3d %s\n"
	       "#2Oval2 - Spell:#0 %3d %s\n"
	       "#2Oval3 - Spell:#0 %3d %s\n"
	       "#2Oval4 - Spell:#0 %3d %s\n"
	       "#2Oval5 - Bites:#0 %3d\n",
	       j->o.od.value[0],
	       j->o.od.value[1], type_to_spell_name (j->o.od.value[1]),
	       j->o.od.value[2], type_to_spell_name (j->o.od.value[2]),
	       j->o.od.value[3], type_to_spell_name (j->o.od.value[3]),
	       j->o.od.value[4], type_to_spell_name (j->o.od.value[4]),
	       j->o.od.value[5]);
      break;
    case ITEM_TOSSABLE:
      sprintf (buf,
	       "#2Oval0 - Facets:#0 %d\n"
	       "#2Oval1 -  Bonus:#0 %d\n",
	       j->o.od.value[0], j->o.od.value[1]);
      sprintf (buf + strlen (buf), "\n#2MKeys:#0 %s\n", j->desc_keys);
      break;
    case ITEM_WORN:
      sprintf (buf, "#2MKeys:#0 %s\n", j->desc_keys);
      break;

    case ITEM_POTION:
    case ITEM_SCROLL:
    case ITEM_SALVE:
      sprintf (buf, "#2Oval0 - Counter 1:#0 %d\n",
	       j->o.od.value[0]);
      sprintf (buf + strlen (buf), "#2Oval1 - Power 1:#0 %d\n",
	       j->o.od.value[1]);
      sprintf (buf + strlen (buf), "#2Oval2 - Counter 2:#0 %d\n",
	       j->o.od.value[2]);
      sprintf (buf + strlen (buf), "#2Oval3 - Power 2:#0 %d\n",
	       j->o.od.value[3]);
      sprintf (buf + strlen (buf), "#2Oval4 - Counter 3:#0 %d\n",
	       j->o.od.value[4]);
      sprintf (buf + strlen (buf), "#2Oval5 - Power 3:#0 %d\n",
	       j->o.od.value[5]);
      send_to_char (buf, ch);
      *buf = '\0';
      break;

    case ITEM_WEAPON:
      if (j->o.weapon.use_skill != SKILL_SHORTBOW
	  && j->o.weapon.use_skill != SKILL_LONGBOW
	  && j->o.weapon.use_skill != SKILL_SLING)
	{
	  sprintf (buf,
		   "#2Hands (Oval0):#0 %d \n", j->o.od.value[0]);
	  sprintf (buf + strlen (buf), "#2Damage (Oval1/2):#0 %dd%d\n",
		   j->o.od.value[1], j->o.od.value[2]);
	  sprintf (buf + strlen (buf), "#2Weapon Type (Type):#0 %s\n",
		   skills[j->o.od.value[3]]);
	  if (j->o.od.value[4] >= 0 && j->o.od.value[4] <= 4)
	    sprintf (buf + strlen (buf), "#2Hit Theme (Oval4):#0 %d (%s)\n",
		     j->o.od.value[4], weapon_theme[j->o.od.value[4]]);
	  else
	    sprintf (buf + strlen (buf), "#2Hit Theme (Oval4):#0 %d (None!)",
		     j->o.od.value[4]);
	  sprintf (buf + strlen (buf), "#2DamBonus (Oval5):#0 %d\n",
		   j->o.od.value[5]);
	}
      else
	{
	  sprintf (buf, "#2Hands (Oval0):#0 %d\n", j->o.od.value[0]);
	  sprintf (buf + strlen (buf), "#2Bow Type (Oval1):#0 %s\n",
		   bow_type[j->o.od.value[1]]);
	  sprintf (buf + strlen (buf), "#2Accuracy Modifier (Oval2):#0 %d\n",
		   j->o.od.value[2]);
	  sprintf (buf + strlen (buf),
		   "#2Skill Used (Oval3):#0 %s [skill number %d]\n",
		   skills[j->o.od.value[3]], j->o.od.value[3]);
	  sprintf (buf + strlen (buf), "#2Damage Modifier (Oval4):#0 %d\n",
		   j->o.od.value[4]);
          sprintf (buf + strlen (buf), "#2Status (Oval5):#0 %s\n", (j->o.od.value[5] == 2) ? "broken" : j->o.od.value[5] ? "strung" : "unstrung");
	}
      break;

    case ITEM_ARMOR:
      sprintf (buf, "#2AC-Apply:#0 [%d]\n#2Type:#0 [%d: %s]\n#2MKeys:#0 %s\n",
	       j->o.od.value[0], j->o.od.value[1],
	       armor_types[j->o.od.value[1]], j->desc_keys);
      break;

    case ITEM_NPC_OBJECT:
      {
	int npc_object_mob_id = j->o.od.value[0];
	CHAR_DATA* npc_object_mob = npc_object_mob_id ? 
	  vtom (npc_object_mob_id) : 0;
	sprintf (buf, "#2NPC VNUM:#0 %d [#5%s#0]\n", npc_object_mob_id,
		 (npc_object_mob) ? npc_object_mob->short_descr : "none");
      }
      break;

    case ITEM_CONTAINER:
      if (!IS_SET (j->obj_flags.extra_flags, ITEM_TABLE))
	sprintf (buf, "#2Oval0 - Capacity:#0  %d  (%d.%.2d lbs)\n"
		 "#2Oval1 - Lockflags:#0 %d\n"
		 "#2Oval2 - Key#0:       %d\n"
		 "#2Oval3 - Pick Penl:#0 %d\n"
		 "#2Oval4 - N/A:#0       %d\n"
		 "#2Oval5 - Corpse:#0    %s\n",
		 j->o.container.capacity,
		 j->o.container.capacity / 100,
		 j->o.container.capacity % 100,
		 j->o.container.flags,
		 j->o.container.key,
		 j->o.container.pick_penalty,
		 j->o.container.v4,
		 j->nVirtual == VNUM_CORPSE ? "Yes" : "No");
      else
	sprintf (buf, "#2Oval0 - Capacity:#0    %d  (%d.%.2d lbs)\n"
		 "#2Oval1 - Lockflags:#0   %d\n"
		 "#2Oval2 - Key:#0         %d\n"
		 "#2Oval3 - Pick Penl:#0   %d\n"
		 "#2Oval4 - N/A:#0         %d\n"
		 "#2Oval5 - Seating Cap:#0 %d\n",
		 j->o.container.capacity,
		 j->o.container.capacity / 100,
		 j->o.container.capacity % 100,
		 j->o.container.flags,
		 j->o.container.key,
		 j->o.container.pick_penalty,
		 j->o.container.v4, j->o.container.table_max_sitting);

        sprintf (buf + strlen(buf), "#2MKeys:#0 %s\n", j->desc_keys);
      break;

    case ITEM_DRINKCON:
    case ITEM_FOUNTAIN:

      if ((j2 = vtoo (j->o.drinkcon.liquid)))
	{
	  sprintf (buf2, "%d (%s)", j2->nVirtual, j2->short_description);
	  if (GET_ITEM_TYPE (j2) != ITEM_FLUID)
	    sprintf (buf2 + strlen (buf2), " [Need %s; this is %s]\n",
		     item_types[ITEM_FLUID],
		     item_types[(int) GET_ITEM_TYPE (j2)]);
	}

      else if (j->o.drinkcon.liquid)
	strcat (buf2, " [None set]");

      else if (j->o.drinkcon.liquid < 33)
	{
	  sprinttype (j->o.drinkcon.liquid, drinks, buf2);
	  strcat (buf2, " [NEEDS OBJECT ASSIGNED]");
	}

      else
	sprintf (buf2, "%d (NO SUCH OBJECT EXISTS)", j->o.drinkcon.liquid);

      sprintf (buf, "#2Oval0 - Capacity:#0 %d\n"
	       "#2Oval1 - Contains:#0 %d\n"
	       "#2Oval2 - Liquid:#0   %s\n",
	       j->o.drinkcon.capacity, j->o.drinkcon.volume, buf2);
      break;

    case ITEM_KEY:
      sprintf (buf, "#2Oval0 - Keytype:#0  %d\n", j->o.od.value[0]);
      sprintf (buf + strlen (buf), "#2Oval1 - Keyed To:#0 %d\n",
	       j->o.od.value[1]);
      break;

    case ITEM_TICKET:
      sprintf (buf, "#2Oval0 - Ticket Number:#0 %d\n"
	       "#2Oval1 - Keeper VNUM:#0   %d\n",
	       j->o.ticket.ticket_num, j->o.ticket.keeper_vnum);
      break;

    case ITEM_MERCH_TICKET:
      sprintf (buf,
	       "#2Oval0 - Order ID##:#0    %d [see matching post on 'Orders' vboard]\n"
	       "#2Oval1 - Keeper VNUM:#0  %d\n", j->o.od.value[0],
	       j->o.od.value[1]);
      break;

    case ITEM_SMALLGOOD:
      sprintf (buf,
	       "#2Oval0 - Amount Remaining:#0 %d\n"
	       "#2Oval1 - Good Type:#0 %s [%d]\n", j->o.od.value[0],
	       smallgood_types[j->o.od.value[1]], j->o.od.value[1]);
      break;


    case ITEM_PERFUME:
      sprintf (buf, "#2Oval0 - Perfume Type:#0 %d\n"
	       "#2Oval1 - Duration:#0     %d\n"
	       "#2Oval2 - Aroma Str:#0    %d\n"
	       "#2Oval3 - Doses:#0        %d\n",
	       j->o.perfume.type,
	       j->o.perfume.duration,
	       j->o.perfume.aroma_strength, j->o.perfume.doses);
      break;

    case ITEM_QUIVER:
      sprintf (buf, "#2Oval0 - Missile Capacity:#0 %d\n", j->o.od.value[0]);
      break;

    case ITEM_KEYRING:
      sprintf (buf, "#2Oval0 - Key Capacity:#0 %d\n", j->o.od.value[0]);
      break;

    case ITEM_SHEATH:
      sprintf (buf, "#2Oval0 - Capacity:#0 %d (%d lbs.)\n", j->o.od.value[0],
	       j->o.od.value[0] / 100);
      break;

    case ITEM_MISSILE:
    case ITEM_BULLET:
      sprintf (buf, "#2Base Damage:#0 %dd%d\n", j->o.od.value[0],
	       j->o.od.value[1]);
      break;

    case ITEM_PLANT:
      sprintf (buf,                "#2Produce Object1:#0 %d\n", j->o.od.value[0]);
      sprintf (buf + strlen (buf), "#2Produce Amount1:#0 %d\n", j->o.od.value[1]);
      sprintf (buf + strlen (buf), "#2Produce Object2:#0 %d\n", j->o.od.value[2]);
      sprintf (buf + strlen (buf), "#2Produce Amount2:#0 %d\n", j->o.od.value[3]);
      sprintf (buf + strlen (buf), "#2Difficulty:#0      %d\n", j->o.od.value[4]);
      sprintf (buf + strlen (buf), "#2Identifiable:#0    %d\n", j->o.od.value[5]);
      break;

    case ITEM_RESOURCE:
      sprintf (buf, "#2Resource Size:#0 %s\n", (j->o.od.value[0] == 0 ? "small" : j->o.od.value[0] >= 2 ? "large" : "medium"));
      sprintf (buf + strlen (buf), "#2Salvage Object1:#0 %d\n", j->o.od.value[1]);
      sprintf (buf + strlen (buf), "#2Salvage Amount1:#0 %d\n", j->o.od.value[2]);
      sprintf (buf + strlen (buf), "#2Salvage Object2:#0 %d\n", j->o.od.value[3]);
      sprintf (buf + strlen (buf), "#2Salvage Amount2:#0 %d\n", j->o.od.value[4]);
      break;
 
    case ITEM_POISON_PASTE:
    case ITEM_POISON_LIQUID:

      *buf = '\0';

      sprintf (buf + strlen (buf), "#2Oval 0 - Doses:#0        %d\n",
	       j->o.poison.uses);
      sprintf (buf + strlen (buf), "#2Oval 1 - Poison/Spell:#0 %s\n",
                (j->o.od.value[1] > 20000 ? somatics[j->o.od.value[1] - 20000] : somatics[0]));
      sprintf (buf + strlen (buf), "#2Oval 2 - Attack:#0 %d\n",
	        j->o.od.value[2]);
      sprintf (buf + strlen (buf), "#2Oval 3 - Delay:#0 %d\n",
	        j->o.od.value[3]);
      sprintf (buf + strlen (buf), "#2Oval 4 - Sustain:#0 %d\n",
	        j->o.od.value[4]);
      sprintf (buf + strlen (buf), "#2Oval 5 - Release:#0 %d\n",
	        j->o.od.value[5]);

      break;

    case ITEM_CARCASS:
      *buf = '\0';

      sprintf (buf + strlen (buf), "#2Oval 0 - Meat1 Vnum:#0 %d\n",
	       j->o.od.value[0]);
      sprintf (buf + strlen (buf), "#2Oval 1 - Meat1 Amount:#0 %d\n",
                j->o.od.value[1]);
      sprintf (buf + strlen (buf), "#2Oval 2 - Meat2 Vnum:#0 %d\n",
	        j->o.od.value[2]);
      sprintf (buf + strlen (buf), "#2Oval 3 - Meat2 Amount:#0 %d\n",
	        j->o.od.value[3]);
      sprintf (buf + strlen (buf), "#2Oval 4 - Meat3 Vnum:#0 %d\n",
	        j->o.od.value[4]);
      sprintf (buf + strlen (buf), "#2Oval 5 - Meat3 Amount:#0 %d\n",
	        j->o.od.value[5]);
      break;

    case ITEM_INK:
      sprintf (buf, "#2Oval 0 -  Dips Left:#0  %d\n", j->o.od.value[0]);
      sprintf (buf + strlen (buf), "#2Oval 1 -  Capacity:#0   %d\n",
	       j->o.od.value[1]);
      break;

    case ITEM_HEALER_KIT:
      sprintf (buf, "#2Oval 0 -  Uses Remaining:#0  %d\n", j->o.od.value[0]);
      sprintf (buf + strlen (buf), "#2Oval 1 -  Healing Bonus:#0   %d\n",
	       j->o.od.value[1]);
      sprintf (buf + strlen (buf), "#2Oval 2 -  Required Skill:#0  %d\n",
	       j->o.od.value[2]);
      sprintf (buf + strlen (buf), "#2Oval 3 -  Immediate Regen:#0 %d\n",
	       j->o.od.value[3]);
      sprintf (buf + strlen (buf), "#2Treats These Wound Types:#0 ");
      if (!j->o.od.value[5] || IS_SET (j->o.od.value[5], TREAT_ALL))
	{
	  sprintf (buf + strlen (buf), " All\n");
	  break;
	}
      if (IS_SET (j->o.od.value[5], TREAT_BLUNT))
	sprintf (buf + strlen (buf), " Contusion");
      if (IS_SET (j->o.od.value[5], TREAT_SLASH))
	sprintf (buf + strlen (buf), " Slash");
      if (IS_SET (j->o.od.value[5], TREAT_PUNCTURE))
	sprintf (buf + strlen (buf), " Puncture");
      if (IS_SET (j->o.od.value[5], TREAT_BURN))
	sprintf (buf + strlen (buf), " Burn");
      if (IS_SET (j->o.od.value[5], TREAT_FROST))
	sprintf (buf + strlen (buf), " Frost");
      if (IS_SET (j->o.od.value[5], TREAT_BLEED))
        sprintf (buf + strlen (buf), " Bleeding");
      sprintf (buf + strlen (buf), "\n");
      break;

    case ITEM_REPAIR_KIT:
      sprintf (buf, "#2Oval 0 -  Uses Remaining:#0  %d\n", j->o.od.value[0]);
      sprintf (buf + strlen (buf), "#2Oval 1 -  Mending Bonus:#0   %d\n",
	       j->o.od.value[1]);
      sprintf (buf + strlen (buf), "#2Oval 2 -  Required Skill:#0  %d\n",
	       j->o.od.value[2]);
      sprintf (buf + strlen (buf), "#2Skill needed to use kit:#0   %s\n",
	       (j->o.od.value[3] >
		0) ? skills[(j->o.od.value[3])] : (j->o.od.value[3] <
						   0) ? "#1None#0" :
	       "#1Any#0");
      sprintf (buf + strlen (buf), "#2Repairs This Item Types:#0   ");
      if (j->o.od.value[5] == 0)
	{
	  sprintf (buf + strlen (buf), "#1All#0\n");
	  break;
	}
      else if (j->o.od.value[5] < 0)
	{
	  sprintf (buf + strlen (buf), "#1None#0\n");
	  break;
	}
      else
	{
	  sprintf (buf + strlen (buf), "%s\n",
		   item_types[(j->o.od.value[5])]);
	  break;
	}

      sprintf (buf + strlen (buf), "\n");
      break;

    case ITEM_DWELLING:
      sprintf (buf, "#2Oval 0 -  Destination Room:#0 %d\n", j->o.od.value[0]);
      sprintf (buf + strlen (buf), "#2Oval 1 -  Max Occupancy:#0    %d\n",
	       j->o.od.value[1]);
      sprintf (buf + strlen (buf), "#2Oval 2 -  Lockflags:#0        %d\n",
	       j->o.od.value[2]);
      sprintf (buf + strlen (buf), "#2Oval 3 -  Entry Key:#0        %d\n",
	       j->o.od.value[3]);
      sprintf (buf + strlen (buf), "#2Oval 4 -  Pick Penalty:#0     %d\n",
	       j->o.od.value[4]);
      sprintf (buf + strlen (buf), "#2Oval 5 -  Master Floorplan:#0 %d\n",
	       j->o.od.value[5]);
      break;

    default:
      if (j->nVirtual == 666)
	sprintf (buf, "#2Original Object VNUM:#0 %d\n", j->o.od.value[0]);
      else
	sprintf (buf, "#2Values 0-5:#0 [%d] [%d] [%d] [%d] [%d] [%d]\n",
		 j->o.od.value[0],
		 j->o.od.value[1],
		 j->o.od.value[2],
		 j->o.od.value[3], j->o.od.value[4], j->o.od.value[5]);
      break;
    }

  send_to_char (buf, ch);

  if (j->contains)
    {
      strcpy (buf, "\n#2Contains#0[ ");
      for (j2 = j->contains; j2; j2 = j2->next_content)
	sprintf (buf + strlen (buf), "%s ", fname (j2->name));
      strcat (buf, "]\n");
      send_to_char (buf, ch);
    }

  if (j->poison)
    {
      sprintf (buf, "#2Poisons:#0 ");
      for (poison = j->poison; poison; poison = poison->next)
	{
	  if (poison != j->poison)
	    sprintf (buf + strlen (buf), "        ");
	  sprintf (buf + strlen (buf), "[%s, Duration: %d mins, Power: %d, Doses: %d",
		   (poison->poison_type >= 20000 ? somatics[poison->poison_type - 20000] : somatics[0]),  poison->release, poison->lvl_power, poison->uses);
	  sprintf (buf + strlen (buf), "]\n");
	}
      send_to_char (buf, ch);
    }


  for (af = j->xaffected; af; af = af->next)
    {

      if (af->type == MAGIC_HIDDEN)
	continue;
      if (af->a.spell.location >= 10000)
	{
	  sprintf (buf, "    #2Affects:#0 %s by %d\n",
		   skills[af->a.spell.location - 10000],
		   af->a.spell.modifier);
	  send_to_char (buf, ch);
	}
    }

  if (j->clock || j->morphto)
    {
      int month, day, hour;

      month = j->clock / (24 * 30);
      hour = j->clock - month * 24 * 30;

      day = hour / 24;
      hour -= day * 24;

      sprintf (buf,
	       "\n#2Morphing Information:#0\n#2Clock:#0   %d %d %d\n#2MorphTo:#0 %d\n",
	       month, day, hour, j->morphto);
      send_to_char (buf, ch);

      if (j->morphTime)
	{
	  int delta, days, hours, minutes;

	  delta = j->morphTime - time (0);

	  days = delta / 86400;
	  delta -= days * 86400;

	  hours = delta / 3600;
	  delta -= hours * 3600;

	  minutes = delta / 60;

	  sprintf (buf,
		   "This object will morph in %d days, %d hours, and %d minutes\n",
		   days, hours, minutes);
	  send_to_char (buf, ch);
	}
    }

  if (j->count > 1)
    {
      sprintf (buf, "\n#2Count:#0 %d\n", j->count);
      send_to_char (buf, ch);
    }
  if (j->obj_flags.set_cost)
    {
      sprintf (buf, "\n#2Sale Price:#0 %7.2f\n", 
	       ((float)j->obj_flags.set_cost) / 100.0);
      send_to_char (buf, ch);
    }
}

/*                                                                          *
 * funtion: emailstat                    < e.g.> stat email null@bar.fu     *
 *                                                                          *
 * 09/17/2004 [JWW] - Fixed two instances where mysql result was not freed  *
 *                    by consolodating if / else / return logic             *
 *                                                                          */
void
emailstat (CHAR_DATA * ch, char *argument)
{
  MYSQL_RES *result;
  MYSQL_ROW row;
  char buf[MAX_STRING_LENGTH];
  int i = 0;

  if (!*argument)
    {
      send_to_char
	("You'll need to either specify a partial or full email address to search for.\n",
	 ch);
      return;
    }

  if (strchr (argument, ';'))
    {
      send_to_char ("You have included an illegal character in your query.\n",
		    ch);
      return;
    }

  mysql_safe_query
    ("SELECT username, user_email FROM forum_users WHERE user_email LIKE '%%%s%%' ORDER BY username ASC",
     argument);
  result = mysql_store_result (database);
  if (result == NULL || mysql_num_rows (result) == 0)
    {
      send_to_char
	("No login accounts were found with email addresses matching your query.\n",
	 ch);
    }
  else if (mysql_num_rows (result) > 255)
    {
      send_to_char
	("More than 255 records were found matching this query. Aborting.\n",
	 ch);
    }
  else
    {

      *buf = '\0';
      sprintf (buf,
	       "\n#6The following user accounts were found to match your query:#0\n\n");
      while ((row = mysql_fetch_row (result)) != NULL)
	{
	  i++;
	  sprintf (buf + strlen (buf), "   %-25s: %s\n", row[0], row[1]);
	}
      sprintf (buf + strlen (buf), "\n#6Matching Records Found: %d#0\n", i);
      page_string (ch->desc, buf);

    }

  if (result)
    {
      mysql_free_result (result);
    }
  else
    {
      system_log
	("Warning: MySql returned a NULL result in staff.c::emailstat() while fetching matching accounts.",
	 true);
    }
  return;
}

void
do_stat (CHAR_DATA * ch, char *argument, int cmd)
{
  char arg1[MAX_STRING_LENGTH];
  char arg2[MAX_STRING_LENGTH];

  half_chop (argument, arg1, arg2);


	if  (GET_TRUST(ch) < 2)
		{
			if (*arg1 != 'o' && *arg1 != 'r')
				{
					s ("Usage: stat <r|o> name\n");
    			s ("");
					s ("    r         room");
					s ("    o         object");
					s ("");
      
      return;
				}
		}
		
	if  (GET_TRUST (ch) < 3)
		{
			if (*arg1 != 'o' && 
					*arg1 != 'r' &&
					*arg1 != 'm' &&
					*arg1 != 'f' )
				{
					s ("Usage: stat <r|o> name\n");
    			s ("");
					s ("    r         room");
					s ("    m         mobile");
					s ("    o         object");
					s ("    f         craft");
					s ("");
   		   return;
				}	
		}
		
		
  if (!*arg1)
    {
      s ("Usage: stat <r|c|m|o|a|e|f> name\n");
	  s ("");
      s ("    r         room");
      s ("    c         character");
      s ("    m         mobile");
      s ("    o         object");
      s ("    a         account");
      s ("    e         email");
      s ("    f         craft");
      s ("");
      
      return;
    }
  else
    {
      switch (*arg1)
	{
	case 'r':
	  roomstat (ch, arg2);
	  return;
	case 'c':
	  charstat (ch, arg2, true);
	  return;
	case 'm':
	  charstat (ch, arg2, false);
	  return;
	case 'o':
	  objstat (ch, arg2);
	  return;
	case 'e':
	  emailstat (ch, arg2);
	  return;
	case 'a':
	  acctstat (ch, arg2);
	  return;
	case 'f':
	  craftstat (ch, arg2);
	  return;
	default:
	  s ("Usage: stat <room|char|mob|obj|acct|email|craft> name\n");
	  s ("");
      s ("    r         room");
      s ("    c         character");
      s ("    m         mobile");
      s ("    o         object");
      s ("    a         account");
      s ("    e         email");
      s ("    f         craft");
      s ("");
	  return;
	}

    }
}

void
save_character_state (CHAR_DATA * tch)
{
  AFFECTED_TYPE *af;
  PHASE_DATA *phase;
  SIGHTED_DATA *sighted;
  char craft_name[MAX_STRING_LENGTH];
  int phase_num;

  if (tch->fighting && tch->fighting->in_room == tch->in_room)
    {
      mysql_safe_query ("INSERT INTO copyover_fighting VALUES (%d, %d)",
			tch->coldload_id, tch->fighting->coldload_id);
    }

  if (tch->aiming_at)
    {
      mysql_safe_query ("INSERT INTO copyover_aiming VALUES (%d, %d)",
			tch->coldload_id, tch->aiming_at->coldload_id);
    }

  if (tch->sighted)
    {
      for (sighted = tch->sighted; sighted; sighted = sighted->next)
	{
	  mysql_safe_query
	    ("INSERT INTO copyover_sighted_targets VALUES (%d, %d)",
	     tch->coldload_id, sighted->target->coldload_id);
	}
    }

  if (tch->subdue && GET_FLAG (tch, FLAG_SUBDUER))
    {
      mysql_safe_query ("INSERT INTO copyover_subduers VALUES (%d, %d)",
			tch->coldload_id, tch->subdue->coldload_id);
    }
/*
	if ( tch->delay_type ) {
		if ( !tch->delay_who )
			sprintf (who, " ");
		else sprintf (who, "%s", tch->delay_who);
		mysql_safe_query ("INSERT INTO copyover_delays VALUES (%d, %d, %d, %d, %d, '%s')",
			tch->coldload_id, tch->delay, tch->delay_type, tch->delay_info1, tch->delay_info2, who);
	}
*/
  if (tch->following)
    {
      mysql_safe_query ("INSERT INTO copyover_followers VALUES (%d, %d)",
			tch->coldload_id, tch->following->coldload_id);
    }

  if ((af = is_crafting (tch)))
    {
      phase_num = 0;
      for (phase = af->a.craft->subcraft->phases; phase; phase = phase->next)
	{
	  phase_num++;
	  if (phase == af->a.craft->phase)
	    break;
	}
      sprintf (craft_name, "%s %s", af->a.craft->subcraft->command,
	       af->a.craft->subcraft->subcraft_name);
      mysql_safe_query
	("INSERT INTO copyover_crafts VALUES (%d, '%s', %d, %d)",
	 tch->coldload_id, craft_name, phase_num, af->a.craft->timer);
    }
}


void
do_shutdown (CHAR_DATA * ch, char *argument, int cmd)
{
  DESCRIPTOR_DATA *d;
  char buf[MAX_STRING_LENGTH];
  char arg[MAX_STRING_LENGTH];
  FILE *fs;
  bool block = false;
  extern bool morgul_arena_fight;	// Defined in arena.c
  extern bool te_pit_fight;	// Defined in arena.c


  argument = one_argument (argument, arg);

  if (!*arg)
    {
      send_to_char ("Specify a shutdown mode please.", ch);
      return;
    }

  if (!str_cmp (arg, "die"))
    {
      system ("touch booting");
      sprintf (buf, "%s is shutting down the server...\n", GET_NAME (ch));
      send_to_gods (buf);
      system_log (buf, false);
      sprintf (buf,
	       "\n#1--- ANNOUNCEMENT ---#0\n#1We are shutting the server down for maintenance. Please check\nback later; our apologies for the inconvenience.#0\n#1--------------------\n#0");
      send_to_all_unf (buf);
      shutd = 1;
      if ((fs = fopen (".reboot", "w")) == NULL)
	{
	  system_log ("Error creating shutdown file.", true);
	  return;
	}
      if (ch->desc->original)
	sprintf (buf, "%s\n", GET_NAME (ch->desc->original));
      else
	sprintf (buf, "%s\n", GET_NAME (ch));
      fputs (buf, fs);
      fclose (fs);
    }
  else if (!str_cmp (arg, "stop"))
    {
      if (!pending_reboot)
	{
	  send_to_char ("No reboot is currently pending.\n", ch);
	  return;
	}
      pending_reboot = false;
      sprintf (buf, "%s has cancelled the pending reboot.", ch->tname);
      send_to_gods (buf);
    }
  else if (!str_cmp (arg, "reboot"))
    {
      if (engine.in_play_mode () && GET_TRUST (ch) < 5)
	{
	  send_to_char ("You'll need to wait for the 4 AM PST auto-reboot.\n",
			ch);
	  return;
	}
      if (morgul_arena_fight)
	block = true;

	if (te_pit_fight)
		block = true;

      for (d = descriptor_list; d; d = d->next)
	{
	  if (!d->character)
	    continue;
	  if (d->character && IS_NPC (d->character))
	    {
	      continue;
	    }
	  if (d->connected != CON_PLYNG && d->character->pc->nanny_state)
	    block = true;
	  if (d->character->pc->create_state == STATE_APPLYING)
	    block = true;
	  if (IS_SET (d->character->act, PLR_QUIET))
	    block = true;
	  if (d->character->fighting && IS_NPC (d->character->fighting))
	    block = true;
	  if (d->character->subdue && IS_NPC (d->character->subdue))
	    block = true;
	  if (d->character->aiming_at && IS_NPC (d->character->aiming_at))
	    block = true;
	  if (d->character->following && IS_NPC (d->character->following))
	    block = true;
	}

      argument = one_argument (argument, arg);
      if (block && *arg != '!')
	{
	  if (pending_reboot)
	    {
	      send_to_char ("There is already a reboot pending.\n", ch);
	      return;
	    }
	  send_to_char ("\n", ch);
	  sprintf (buf, "%s has queued a server reboot.", ch->tname);
	  send_to_gods (buf);
	  pending_reboot = true;
	  return;
	}
      if (!(fs = fopen (".copyover_data", "w")))
	{
	  send_to_char
	    ("#1Copyover file not writeable; reboot aborted. Please notify the staff.#0\n",
	     ch);
	  system_log ("Error creating copyover file.", true);
	}
      fclose (fs);
      if (!(fs = fopen (".reboot", "w")))
	{
	  system_log ("Error creating reboot file.", true);
	  return;
	}
      if (ch->desc->original)
	sprintf (buf, "%s\n", GET_NAME (ch->desc->original));
      else
	sprintf (buf, "%s\n", GET_NAME (ch));
      fputs (buf, fs);
      fclose (fs);
      unlink ("booting");
      prepare_copyover (0);
    }
  else
    send_to_char ("Specify shutdown parameter:  Die or Reboot ?\n", ch);
}

void
clear_watch (CHAR_DATA * ch)
{
  CHAR_DATA *tch;
  AFFECTED_TYPE *af;

  for (tch = character_list; tch; tch = tch->next)
    {

      if (tch->deleted || IS_NPC (tch))
	continue;

      if ((af = get_affect (tch, MAGIC_WATCH1)) && af->a.spell.t == (long int) ch)
	affect_remove (tch, af);

      if ((af = get_affect (tch, MAGIC_WATCH2)) && af->a.spell.t == (long int) ch)
	affect_remove (tch, af);

      if ((af = get_affect (tch, MAGIC_WATCH3)) && af->a.spell.t == (long int) ch)
	affect_remove (tch, af);
    }
}

void
do_watch (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  AFFECTED_TYPE *af;
  CHAR_DATA *tch;

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      clear_watch (ch);
      send_to_char ("Ok.\n", ch);
      return;
    }

  if (!(tch = get_pc (buf)))
    {
      send_to_char ("PC not found.  Did you enter the full name?\n", ch);
      return;
    }

  if ((af = get_affect (tch, MAGIC_WATCH1)) && af->a.spell.t == (long int) ch)
    {
      send_to_char ("Watch removed.\n", ch);
      affect_remove (tch, af);
      return;
    }

  if ((af = get_affect (tch, MAGIC_WATCH2)) && af->a.spell.t == (long int) ch)
    {
      send_to_char ("Watch removed.\n", ch);
      affect_remove (tch, af);
      return;
    }

  if ((af = get_affect (tch, MAGIC_WATCH3)) && af->a.spell.t == (long int) ch)
    {
      send_to_char ("Watch removed.\n", ch);
      affect_remove (tch, af);
      return;
    }

  if (!get_affect (tch, MAGIC_WATCH1))
    {
      magic_add_affect (tch, MAGIC_WATCH1, -1, 0, 0, 0, 0);
      get_affect (tch, MAGIC_WATCH1)->a.spell.t = (long int) ch;
      send_to_char ("Watch added.\n", ch);
    }

  else if (!get_affect (tch, MAGIC_WATCH2))
    {
      magic_add_affect (tch, MAGIC_WATCH2, -1, 0, 0, 0, 0);
      get_affect (tch, MAGIC_WATCH2)->a.spell.t = (long int) ch;
      send_to_char ("Watch added.\n", ch);
    }

  else if (!get_affect (tch, MAGIC_WATCH3))
    {
      magic_add_affect (tch, MAGIC_WATCH3, -1, 0, 0, 0, 0);
      get_affect (tch, MAGIC_WATCH3)->a.spell.t = (long int) ch;
      send_to_char ("Watch added.\n", ch);
    }

  else
    {
      send_to_char ("Three other people watching...try snooping.\n", ch);
      return;
    }
}

void
do_snoop (CHAR_DATA * ch, char *argument, int cmd)
{
  char arg[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  CHAR_DATA *victim;

  if (!ch->desc)
    return;

  one_argument (argument, arg);

  if (!*arg)
    strcpy (arg, GET_NAME (ch));

  if (IS_NPC (ch))
    {
      send_to_char ("Mixing snoop and switch is bad for your health.\n", ch);
      return;
    }

  if (!(victim = get_char_vis (ch, arg)))
    {
      send_to_char ("No such person around.\n", ch);
      return;
    }

  if (!victim->desc)
    {
      send_to_char ("There's no link.. nothing to snoop.\n", ch);
      return;
    }

  if (!victim->pc)
    {
      send_to_char ("Sorry... can only snoop PCs!\n", ch);
      return;
    }

  if (victim == ch)
    {

      send_to_char ("You stop snooping.\n", ch);

      if (ch->desc->snoop.snooping)
	{
	  if (ch->desc->snoop.snooping->pc->level)
	    {
	      sprintf (buf, "%s stops snooping you.\n", GET_NAME (ch));
	      send_to_char (buf, ch->desc->snoop.snooping);
	    }
	  ch->desc->snoop.snooping->desc->snoop.snoop_by = 0;
	  ch->desc->snoop.snooping = 0;
	}

      return;
    }

  if (victim->desc->snoop.snoop_by)
    {
      sprintf (buf, "%s is snooping already.\n",
	       GET_NAME (victim->desc->snoop.snoop_by));
      send_to_char (buf, ch);
      return;
    }

  if (victim->pc->level)
    {
      sprintf (buf, "%s is snooping you.\n", GET_NAME (ch));
      send_to_char (buf, victim);
    }

  send_to_char ("Ok.\n", ch);

  if (ch->desc->snoop.snooping)
    ch->desc->snoop.snooping->desc->snoop.snoop_by = 0;

  ch->desc->snoop.snooping = victim;
  victim->desc->snoop.snoop_by = ch;
  return;
}

void
do_summon (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *victim;
  AFFECTED_TYPE *af;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Summon whom?\n", ch);
      return;
    }

  victim = get_char (buf);

  if (!victim)
    {
      send_to_char ("Couldn't find that character.\n", ch);
      return;
    }

  if (victim == ch)
    {
      send_to_char ("You cannot summon yourself.\n", ch);
      return;
    }

  if (victim->in_room == ch->in_room)
    {
      act ("$N is already here.", false, ch, 0, victim, TO_CHAR);
      return;
    }

  if ((af = get_affect (victim, MAGIC_STAFF_SUMMON)))
    af->a.spell.t = ch->in_room;
  else
    {
      magic_add_affect (victim, MAGIC_STAFF_SUMMON, 24, 0, 0, 0, 0);
      af = get_affect (victim, MAGIC_STAFF_SUMMON);
      af->a.spell.t = victim->in_room;
    }

  char_from_room (victim);
  char_to_room (victim, ch->in_room);

  if (GET_HIT (victim) <= 0)
    GET_HIT (victim) = 1;

  if (!AWAKE (victim))
  	{
    GET_POS (victim) = STAND;
    remove_cover(victim, 0);
    }

  act ("$N has been summoned.", false, ch, 0, victim, TO_CHAR);
  act ("$n summons $N.", false, ch, 0, victim, TO_NOTVICT);
  act ("You have been summoned by $N.", true, victim, 0, ch, TO_CHAR);

  send_to_char ("Use the RETURN command to return to your former location "
		"when you are ready.\n", victim);
}

void
do_switch (CHAR_DATA * ch, char *argument, int cmd)
{
  char arg[MAX_STRING_LENGTH];
  CHAR_DATA *victim;

  one_argument (argument, arg);

  if (!*arg)
    {
      do_switch_item (ch, argument, cmd);
      return;
    }

  if (IS_NPC (ch))
    return;

  if (IS_MORTAL (ch))
    {
      send_to_char ("Eh?\n", ch);
      return;
    }

  if (ch->pc && ch->pc->create_state == STATE_DIED)
    {
      send_to_char ("Huh?\n", ch);
      return;
    }

  if (!(victim = get_char_room_vis (ch, arg)))
    if (!(victim = get_char (arg)))
      {
	send_to_char ("They aren't here.\n", ch);
	return;
      }

  if (ch == victim)
    {
      send_to_char
	("Seek some help bud, no multi-personality disorders allowed here.\n",
	 ch);
      return;
    }

  if (!ch->desc || ch->desc->snoop.snoop_by || ch->desc->snoop.snooping)
    {
      send_to_char ("Mixing snoop & switch is bad for your health.\n", ch);
      return;
    }

  if (victim->desc || (!IS_NPC (victim) && !victim->pc->admin_loaded))
    send_to_char ("You can't do that, the body is already in use!\n", ch);
  else
    {
      send_to_char ("Ok.\n", ch);

      ch->desc->character = victim;
      ch->desc->original = ch;
      if (ch->desc->original->color)
	victim->color = 1;

      victim->desc = ch->desc;
      ch->desc = 0;
    }
}

void
do_return (CHAR_DATA * ch, char *argument, int cmd)
{
  AFFECTED_TYPE *af;

  if (ch->desc)
    {
      sprintf (s_buf, "return ch->desc->original: %d",
	       (long int) ch->desc->original);
    }

  if (ch->desc && ch->desc->original && is_he_somewhere (ch->desc->original))
    {
      send_to_char ("You return to your original character.\n", ch);
      ch->desc->character = ch->desc->original;
      ch->desc->original = NULL;
      ch->desc->character->desc = ch->desc;
      ch->desc = NULL;
      return;
    }

  if ((af = get_affect (ch, MAGIC_STAFF_SUMMON)))
    {
      act ("$n unsummons $mself.", false, ch, 0, 0, TO_ROOM);
      char_from_room (ch);
      char_to_room (ch, af->a.spell.t);
      act ("$n appears.", false, ch, 0, 0, TO_ROOM);
      do_look (ch, "", 0);
      affect_remove (ch, af);
      return;
    }

  if (IS_MORTAL (ch))
    send_to_char ("You are where you should be.\n", ch);
  else
    send_to_char ("You are what you should be.\n", ch);
}

void
do_force (CHAR_DATA * ch, char *argument, int cmd)
{
  struct descriptor_data *i;
  CHAR_DATA *vict;
  char name[100], to_force[100], buf[100];

  half_chop (argument, name, to_force);

  if (!*name || !*to_force)
    send_to_char ("Who do you wish to force to do what?\n", ch);
  else if ((str_cmp ("all", name) && str_cmp ("room", name))
	   || GET_TRUST (ch) < 5)
    {
      if (!(vict = get_char_room_vis (ch, name)))
	send_to_char ("No-one by that name here.\n", ch);
      else
	{
	  if (vict == ch)
	    {
	      send_to_char ("Force yourself?\n", ch);
	      return;
	    }
	  if (GET_TRUST (ch) < GET_TRUST (vict))
	    vict = ch;
	  sprintf (buf, "$n has forced you to '%s'.", to_force);
	  act (buf, false, ch, 0, vict, TO_VICT);
	  send_to_char ("Ok.\n", ch);
	  command_interpreter (vict, to_force);
	}
    }
  else if (!str_cmp ("room", name))
    {
      for (vict = ch->room->people; vict; vict = vict->next_in_room)
	{
	  if (GET_TRUST (vict) < GET_TRUST (ch))
	    command_interpreter (vict, to_force);
	}
    }
  else
    {				/* force all */
      for (i = descriptor_list; i; i = i->next)
	{
	  if (i->character != ch && !i->connected)
	    {
	      vict = i->character;
	      if (GET_TRUST (ch) < GET_TRUST (vict))
		send_to_char ("Oh no you don't!!\n", ch);
	      else
		{
		  sprintf (buf, "$n has forced you to '%s'.", to_force);
		  act (buf, false, ch, 0, vict, TO_VICT);
		  command_interpreter (vict, to_force);
		}
	    }
	}
      send_to_char ("Ok.\n", ch);
    }
}

void
do_as (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch;
  char buf[MAX_STRING_LENGTH];
  DESCRIPTOR_DATA *save_d;

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Syntax:  as <character> <command>\n", ch);
      return;
    }

  if (!(tch = get_char_room_vis (ch, buf)))
    {
      send_to_char ("They aren't here.\n", ch);
      return;
    }

  if (!IS_NPC (tch) && IS_IMPLEMENTOR (tch))
    {
      tch = ch;
    }

  if (!IS_NPC (tch) && !IS_NPC (ch))
    {
      if (tch->pc->level > ch->pc->level)
	{
	  send_to_char ("I'm sure they wouldn't appreciate that.\n", ch);
	  return;
	}
    }

  one_argument (argument, buf);

  if (!str_cmp (buf, "switch"))
    {
      send_to_char ("Naughty genie...\n", ch);
      return;
    }

  if (!str_cmp (buf, "quit"))
    {
      send_to_char ("Oh, not quit...that would crash the mud...use force "
		    "instead.\n", ch);
      return;
    }

  save_d = tch->desc;
  tch->desc = ch->desc;

  command_interpreter (tch, argument);

  tch->desc = save_d;
}


OBJ_DATA *
get_eq_by_type_and_name (CHAR_DATA * ch, int type, char *name)
{
  int i = 0;
  OBJ_DATA *eq = NULL;

  if (!type && !*name)
    {
      return NULL;
    }

  for (i = 0; i < MAX_WEAR; i++)
    {

      if (!(eq = get_equip (ch, i)))
	{
	  continue;
	}

      /* If we are looking for a particular type */
      if (type && GET_ITEM_TYPE (eq) != type)
	{
	  continue;
	}

      /* If we are looking for a specific name */
      if (name && *name)
	{
	  if (isname (name, eq->name))
	    {

	      return eq;
	    }
	  else
	    {
	      continue;
	    }
	}
      else
	{

	  return eq;
	}

    }

  return NULL;
}


OBJ_DATA *
draw_missile_from_quiver (CHAR_DATA * ch, char *missile, OBJ_DATA * quiver,
			  bool bBoltOnly)
{
  OBJ_DATA *pMissile = NULL;

  if (quiver && GET_ITEM_TYPE (quiver) == ITEM_QUIVER)
    {

      for (pMissile = quiver->contains; pMissile;
	   pMissile = pMissile->next_content)
	{

	  if (GET_ITEM_TYPE (pMissile) != ITEM_MISSILE)
	    {
	      continue;
	    }

	  if (missile && *missile && !isname (missile, pMissile->name))
	    {
	      continue;
	    }

	  if (bBoltOnly && !isname ("bolt", pMissile->name))
	    {
	      continue;
	    }

	  if (!bBoltOnly && !isname ("arrow", pMissile->name))
	    {
	      continue;
	    }

	  ch->delay_info2 = pMissile->nVirtual;
	  ch->delay_info1 = quiver->nVirtual;

	  return pMissile;
	}
    }

  return NULL;
}


/* load [ <missile> ] [ [ from ] [ <quiver> ] ] */
void
do_load_missile (CHAR_DATA * ch, char *argument, int cmd)
{
  // 1. are we wielding an unloaded bow?
  // 2. do we have appropriate ammo

  bool bBolts = false;		/* Looking for bolts or arrows? */
  short int nArgC = 0;
  OBJ_DATA *ptrBow = NULL;
  OBJ_DATA *ptrOffHand = NULL;
  OBJ_DATA *ptrMissile = NULL;
  OBJ_DATA *ptrQuiver = NULL;
  char strMissile[AVG_STRING_LENGTH] = "";
  char strFrom[AVG_STRING_LENGTH] = "";
  char strQuiver[AVG_STRING_LENGTH] = "";
  char strBuf[AVG_STRING_LENGTH * 4] = "";


  /* Check for a bow first */
  if (!(ptrBow = get_equip (ch, WEAR_PRIM))
      || (ptrBow->o.weapon.use_skill != SKILL_SHORTBOW
	  && ptrBow->o.weapon.use_skill != SKILL_LONGBOW)
      || (ptrBow->loaded)
      || get_equip (ch, WEAR_SEC) || get_equip (ch, WEAR_BOTH))
    {
      send_to_char
  ("You must first be wielding an unloaded bow, and only that weapon.\n", ch);
      return;
    }

  if(ptrBow->o.od.value[5] != 1)
  {
    send_to_char("You must string the bow before you can load it.\n", ch);
    return;
  }
  

	if (is_mounted(ch) &&
		(ptrBow->o.weapon.use_skill == SKILL_LONGBOW))
		{
			send_to_char ("You can't load that while mounted!\n", ch);
      return;
    }

  // bBolts = (ptrBow->o.weapon.use_skill == SKILL_CROSSBOW);
  ptrOffHand = (ptrBow == ch->right_hand) ? ch->left_hand : ch->right_hand;

  nArgC = (argument
	   && *argument) ? sscanf (argument, "%s %s %s", strMissile, strFrom,
				   strQuiver) : 0;

  if (nArgC == 0)
    {

      /* no arguments specified check off-hand first */

      if (ptrOffHand)
	{

	  if (GET_ITEM_TYPE (ptrOffHand) == ITEM_MISSILE
	      && ptrOffHand->count == 1
	      && isname ((bBolts) ? "bolt" : "arrow", ptrOffHand->name))
	    {

	      ptrMissile = ptrOffHand;
	    }
	  else
	    {
	      send_to_char
		("Your off-hand may contain one missile, otherwise it needs to be empty.\n",
		 ch);
	      return;
	    }
	}

      /* nothing in our off-hand, load from the first quiver we find */

      else if ((ptrQuiver = get_eq_by_type_and_name (ch, ITEM_QUIVER, NULL)))
	{

	  ptrMissile = draw_missile_from_quiver (ch, NULL, ptrQuiver, bBolts);
	}

      /* no quiver either */

      else
	{
	  send_to_char
	    ("You don't seem to be holding a missile or wearing a quiver.\n",
	     ch);
	  return;
	}

    }
  else if (nArgC == 1)
    {

      /* may be "load <missile>" or "load <quiver>" */

      /* check first for arrow in hand */

      if (ptrOffHand)
	{

	  if (GET_ITEM_TYPE (ptrOffHand) == ITEM_MISSILE
	      && ptrOffHand->count == 1
	      && isname (strMissile, ptrOffHand->name)
	      && isname ((bBolts) ? "bolt" : "arrow", ptrOffHand->name))
	    {

	      ptrMissile = ptrOffHand;
	    }
	  else
	    {
	      send_to_char ("Load What?\n", ch);
	      return;
	    }

	}

      /* nothing there, check for a quiver first, then an arrow */

      else
	{
	  if ((ptrQuiver =
	       get_eq_by_type_and_name (ch, ITEM_QUIVER, strMissile)))
	    {
	      ptrMissile =
		draw_missile_from_quiver (ch, NULL, ptrQuiver, bBolts);
	    }
	  else
	    if ((ptrQuiver = get_eq_by_type_and_name (ch, ITEM_QUIVER, NULL)))
	    {
	      ptrMissile =
		draw_missile_from_quiver (ch, strMissile, ptrQuiver, bBolts);
	    }
	  else
	    {
	      send_to_char ("Load what? Or from what?\n", ch);
	      return;
	    }
	}
    }
  else if (nArgC == 2 && !strcmp (strMissile, "from"))
    {

      /* must be "load from <quiver>" */

      if ((ptrQuiver = get_eq_by_type_and_name (ch, ITEM_QUIVER, strFrom)))
	{

	  ptrMissile = draw_missile_from_quiver (ch, NULL, ptrQuiver, bBolts);
	}
      else
	{
	  send_to_char ("Load from what?\n", ch);
	  return;
	}

    }
  else if (nArgC == 3 && !strcmp (strFrom, "from"))
    {

      /* must be "load <missile> from <quiv>" */

      if ((ptrQuiver = get_eq_by_type_and_name (ch, ITEM_QUIVER, strQuiver)))
	{

	  ptrMissile =
	    draw_missile_from_quiver (ch, strMissile, ptrQuiver, bBolts);
	}
      else
	{
	  send_to_char ("Load from what?\n", ch);
	  return;
	}
    }
  else
    {
      do_help (ch, "combat load", 0);
      return;
    }


  /* Was there a missle? */
  if (!ptrMissile)
    {
      send_to_char ("Load what?\n", ch);
      return;
    }

  ch->delay_info1 = (ptrQuiver) ? ptrQuiver->nVirtual : -1;
  ch->delay_info2 = ptrMissile->nVirtual;

  if (bBolts)
    {
      sprintf (strBuf,
	       "You begin the somewhat laborious task of loading #2%s#0 with #2%s#0.",
	       obj_short_desc (ptrBow), ptrMissile->short_description);
      act (strBuf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      sprintf (strBuf,
	       "$n begins the somewhat laborious task of loading #2%s#0 with #2%s#0.",
	       obj_short_desc (ptrBow), ptrMissile->short_description);
      act (strBuf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      // ch->delay = 15 - (skill_level (ch, SKILL_CROSSBOW, 0) / 20);
      ch->delay_type = DEL_LOAD_WEAPON;
      ch->delay_who = ptrMissile->name;
      return;
    }
  else
    {
      sprintf (strBuf, "You nock #2%s#0, drawing back upon #2%s#0.",
	       ptrMissile->short_description, obj_short_desc (ptrBow));
      act (strBuf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
      sprintf (strBuf, "$n nocks #2%s#0 and draws back upon #2%s#0.",
	       ptrMissile->short_description, obj_short_desc (ptrBow));
      act (strBuf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      ch->delay = 4 - (skill_level (ch, ptrBow->o.weapon.use_skill, 0) / 30);
      ch->delay = MAX (ch->delay, 1);
      ch->delay_type = DEL_LOAD_WEAPON;
      ch->delay_who = ptrMissile->name;
      return;
    }

}

void
do_load_mobile (CHAR_DATA * ch, char *argument, int cmd)
{
  int nMobileVNUM = 0;
  CHAR_DATA *mob = NULL;

  sscanf (argument, "%d", &nMobileVNUM);

  if (!(mob = load_mobile (nMobileVNUM)))
    {
      send_to_char ("There is no mobile with that number.\n", ch);
      return;
    }

  char_to_room (mob, ch->in_room);
  mob->mob->spawnpoint = ch->in_room;

  act ("$n hastily completes a blue requisition form.", true, ch, 0, 0,
       TO_ROOM);
  act ("$N enters the area.", false, ch, 0, mob, TO_ROOM);
  act ("$N appears.", false, ch, 0, mob, TO_CHAR);
}

void
do_load_object (CHAR_DATA * ch, char *argument, int cmd)
{
  /* if load o <count> <vnum>, we need to check for <vnum> */
  /*

     argument = one_argument ( argument, num ) ;
     number = atoi ( num ) ;

     if ( number < 0 || number > 99999 ) {
     send_to_char( "Vnum must be from 0 to 99999\n", ch ) ;
     return;
     }

     argument = one_argument (argument, buf);

     if ( isdigit (*buf) ) {
     count = number;
     number = atoi (buf);
     argument = one_argument (argument, buf);
     } 
     else {
     count = 1;
     }
     obj = NULL;

     if ( *buf ) {
     if ( (ind = index_lookup (standard_object_colors, buf)) != -1 )
     ch->delay_who = add_hash(standard_object_colors[ind]);
     else if ( (ind = index_lookup (fine_object_colors, buf)) != -1 )
     ch->delay_who = add_hash(fine_object_colors[ind]);
     else if ( (ind = index_lookup (drab_object_colors, buf)) != -1 )
     ch->delay_who = add_hash(drab_object_colors[ind]);
     if ( ind != -1 ) {
     if ( !(obj = load_colored_object (number, ch->delay_who)) ) {
     send_to_char ("There is no object with that number.\n", ch);
     return;
     }
     argument = one_argument (argument, buf);
     }
     else {
     if ( !(obj = load_object (number)) ) {
     send_to_char ("There is no object with that number.\n", ch);
     return;
     }
     }
     }
     else {
     if ( !(obj = load_object (number)) ) {
     send_to_char ("There is no object with that number.\n", ch);
     return;
     }
     }

     ch->delay_who = NULL;

     obj->count = count;

     mob = NULL;

     if ( obj->nVirtual == VNUM_PENNY || obj->nVirtual == VNUM_FARTHING )
     name_money (obj);

     if ( *buf ) {
     if ( !(mob = get_char_room_vis (ch, buf)) ) {
     send_to_char ("Mob was not located.\n", ch);
     extract_obj (obj);
     return;
     }
     }

     if ( mob ) {
     act ("$N hastily completes a red requisition form;\n"
     "   $p appears in your hands.",
     true, mob, obj, ch, TO_CHAR);
     if ( ch != mob )
     act ("$p was created in $N's hands.", true, ch, obj, mob, TO_CHAR);
     obj_to_char (obj, mob);
     }
     else {
     obj_to_room ( obj, ch->in_room );
     act ( "$n hastily completes a red requisition form.", true, ch, 0, 0, TO_ROOM ) ;
     act ( "$p immediately appears!", false, ch, obj, 0, TO_ROOM ) ;
     act ( "$p appears.", false, ch, obj, 0, TO_CHAR ) ;
     }
   */
}

void _do_load (CHAR_DATA * ch, char *argument, int cmd);
void
do_load (CHAR_DATA * ch, char *argument, int cmd)
{
  if (IS_MORTAL (ch))
    {
      do_load_missile (ch, argument, cmd);
      return;
    }
  else if (argument
	   && (*argument == 'c' || *argument == 'o' || *argument == 'm'))
    {
      _do_load (ch, argument, cmd);
    }
  else
    {
      do_load_missile (ch, argument, cmd);
      return;
    }
}

void
_do_load (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *mob = NULL;
  OBJ_DATA *obj;
  OBJ_DATA *bow;
  OBJ_DATA *ammo = NULL;
  OBJ_DATA *quiver;
  ROOM_DATA *room;
  char buf[MAX_STRING_LENGTH];
  char buffer[MAX_STRING_LENGTH];
  char arg1[MAX_STRING_LENGTH];
  char arg2[MAX_STRING_LENGTH];
  char type[MAX_STRING_LENGTH];
  char num[MAX_STRING_LENGTH];
  char color[MAX_STRING_LENGTH];
  char color2[MAX_STRING_LENGTH];
  char color3[MAX_STRING_LENGTH];
  int number, i, ind = 0;
  int count, quiver_used = 0;
  int ammo_found = 0;

  argument = one_argument (argument, type);
  argument = one_argument (argument, num);

  sprintf (arg1, "%s", type);
  sprintf (arg2, "%s", num);

  if (!ammo_found || *argument)
    {
      if ((obj = get_obj_in_dark (ch, arg1, ch->right_hand)) ||
	  (obj = get_obj_in_dark (ch, arg1, ch->left_hand)))
	{
	  if (GET_ITEM_TYPE (obj) == ITEM_WEAPON)
	    *arg1 = '\0';
	  obj = NULL;
	}
      if (*arg1)
	{
	  if (!str_cmp (arg2, "from"))
	    argument = one_argument (argument, arg2);
	  if (!*arg2)
	    {
	      for (i = 0; i < MAX_WEAR; i++)
		{
		  if (!(quiver = get_equip (ch, i)))
		    continue;
		  if (GET_ITEM_TYPE (quiver) == ITEM_QUIVER
		      && get_obj_in_list_vis (ch, arg1, quiver->contains))
		    break;
		}
	    }
	  else
	    {
	      for (i = 0; i < MAX_WEAR; i++)
		{
		  if (!(quiver = get_equip (ch, i)))
		    continue;
		  if (GET_ITEM_TYPE (quiver) == ITEM_QUIVER
		      && isname (arg2, quiver->name))
		    break;
		}
	    }
	  if (quiver)
	    {
	      for (ammo = quiver->contains; ammo; ammo = ammo->next_content)
		{
		  if (GET_ITEM_TYPE (ammo) == ITEM_MISSILE
		      && isname (arg1, ammo->name))
		    {
		      ammo_found++;
		      quiver_used++;
		      break;
		    }
		}
	    }
	}
      else
	{
	  for (i = 0; i < MAX_WEAR; i++)
	    {
	      if (!(quiver = get_equip (ch, i)))
		continue;
	      if (GET_ITEM_TYPE (quiver) == ITEM_QUIVER)
		{
		  for (ammo = quiver->contains; ammo;
		       ammo = ammo->next_content)
		    {
		      if (GET_ITEM_TYPE (ammo) == ITEM_MISSILE)
			{
			  ammo_found++;
			  quiver_used++;
			  ch->delay_info2 = ammo->nVirtual;
			  ch->delay_info1 = quiver->nVirtual;
			  break;
			}
		      if (quiver_used)
			break;
		    }
		}
	    }
	}
    }

  if ((!ammo || !ammo_found) && !*type && !*num)
    {
      send_to_char
	("You must have the ammunition you wish to load it with in your inventory.\n",
	 ch);
      return;
    }

  if (ammo_found && ammo)
    {
      if (!(bow = get_equip (ch, WEAR_BOTH)))
	{
	  send_to_char
	    ("You must first be wielding the weapon you wish to load.\n", ch);
	  return;
	}

      if (bow && bow->loaded)
	{
	  send_to_char ("That's already loaded.\n", ch);
	  return;
	}

      else if (bow)
	{
	  if (bow->o.weapon.use_skill != SKILL_SHORTBOW &&
	      bow->o.weapon.use_skill != SKILL_LONGBOW)
	    {
	      send_to_char ("You can't load that!\n", ch);
	      return;
	    }
	}

      if (!strn_cmp (ammo->name, "arrow", 5) &&
	  !(bow->o.weapon.use_skill == SKILL_LONGBOW
	    || bow->o.weapon.use_skill == SKILL_SHORTBOW))
	{
	  send_to_char
	    ("This isn't the right sort of ammunition. A bow uses arrows.\n",
	     ch);
	  return;
	}

/*      if (!strn_cmp (ammo->name, "bolt", 4) &&
	  bow->o.weapon.use_skill != SKILL_CROSSBOW)
	{
	  send_to_char
	    ("This isn't the right sort of ammunition. A crossbow uses bolts.\n",
	     ch);
	  return;
	} */

      ch->delay_info2 = ammo->nVirtual;
      if (quiver)
	ch->delay_info1 = quiver->nVirtual;

/*      if (bow && bow->o.weapon.use_skill == SKILL_CROSSBOW)
	{
	  sprintf (buf,
		   "You begin the somewhat laborious task of loading #2%s#0 with #2%s#0.",
		   obj_short_desc (bow), ammo->short_description);
	  act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
	  sprintf (buf,
		   "%s#0 begins the somewhat laborious task of loading #2%s#0 with #2%s#0.",
		   char_short (ch), obj_short_desc (bow),
		   ammo->short_description);
	  *buf = toupper (*buf);
	  sprintf (buffer, "#5%s", buf);
	  sprintf (buf, "%s", buffer);
	  act (buf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
	  ch->delay = 15 - ch->skills[SKILL_CROSSBOW] / 20;
	  ch->delay_type = DEL_LOAD_WEAPON;
	  ch->delay_who = ammo->name;
	  return;
	}*/
      if (bow
	       && (bow->o.weapon.use_skill == SKILL_SHORTBOW
		   || bow->o.weapon.use_skill == SKILL_LONGBOW))
	{
	  sprintf (buf, "You nock #2%s#0, drawing back upon #2%s#0.",
		   ammo->short_description, obj_short_desc (bow));
	  act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
	  sprintf (buf, "%s#0 nocks #2%s#0 and draws back upon #2%s#0.",
		   char_short (ch), ammo->short_description,
		   obj_short_desc (bow));
	  *buf = toupper (*buf);
	  sprintf (buffer, "#5%s", buf);
	  sprintf (buf, "%s", buffer);
	  act (buf, false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
	  ch->delay = 4 - ch->skills[bow->o.weapon.use_skill] / 30;
	  ch->delay = MAX (ch->delay, 2);
	  ch->delay_type = DEL_LOAD_WEAPON;
	  ch->delay_who = ammo->name;
	  return;
	}
    }

  if (IS_MORTAL (ch))
    {
      send_to_char
	("You must have a weapon equipped and a missile available.\n", ch);
      return;
    }

  if (!*type || !*num || *type == '?')
    {
      send_to_char ("Syntax:\n"
		    "   load <'char' | 'obj'> <number>\n"
		    "   load <'player'> <name>\n"
		    "   load <'money'> <amount>\n", ch);
      return;
    }

  number = atoi (num);

  if (number < 0 || number > 99999)
    {
      send_to_char ("Vnum must be from 0 to 99999\n", ch);
      return;
    }

  else if (is_abbrev (type, "character") || is_abbrev (type, "mobile"))
    {
      if (!(mob = load_mobile (number)))
	{
	  send_to_char ("There is no mobile with that number.\n", ch);
	  return;
	}

      char_to_room (mob, ch->in_room);
      mob->mob->spawnpoint = ch->in_room;

      act ("$N immediately appears!", false, ch, 0, mob, TO_ROOM);
      act ("$N appears.", false, ch, 0, mob, TO_CHAR);

    }

  else if (is_abbrev (type, "player"))
    {
      if (!engine.in_test_mode ())
	{
	  send_to_char
	    ("Temporarily disabled, to see if this alleviates the crashing...\n",
	     ch);
	  return;
	}

      sprintf (buf, "%s", num);

      if (!*buf)
	{
	  send_to_char ("Which PC did you wish to load?\n", ch);
	  return;
	}

      CREATE (mob->pc, PC_DATA, 1);
      mob = load_char_mysql (buf);
      if (mob)
	{
	  load_char_objs (mob, mob->tname);
	  mob->pc->admin_loaded = true;
	  mob->act |= ACT_ISNPC;
	  char_to_room (mob, ch->in_room);
	  act ("$N immediately appears!", false, ch, 0, mob, TO_ROOM);
	  act ("$N appears.", false, ch, 0, mob, TO_CHAR);
	  return;
	}
      else
	{
	  send_to_char ("That PC could not be loaded from the database.\n",
			ch);
	  return;
	}
    }

  else if (is_abbrev (type, "obj"))
    {

      /* if load o <count> <vnum>, we need to check for <vnum> */

      argument = one_argument (argument, buf);

      if (isdigit (*buf))
	{
	  count = number;
	  number = atoi (buf);
	  argument = one_argument (argument, buf);
	}
      else
	count = 1;

      obj = NULL;

      if (*buf)
	{
	  if ((ind = index_lookup (standard_object_colors, buf)) != -1)
	    ch->delay_who = add_hash (standard_object_colors[ind]);
	  else if ((ind = index_lookup (fine_gem_colors, buf)) != -1)
	    ch->delay_who = add_hash (fine_gem_colors[ind]);
	  else if ((ind = index_lookup (gem_colors, buf)) != -1)
	    ch->delay_who = add_hash (gem_colors[ind]);
	  else if ((ind = index_lookup (blade_type_colors, buf)) != -1)
	    ch->delay_who = add_hash (blade_type_colors[ind]);
	  else if ((ind = index_lookup (design_type_colors, buf)) != -1)
	    ch->delay_who = add_hash (design_type_colors[ind]);
	  else if ((ind = index_lookup (animal_colors, buf)) != -1)
	    ch->delay_who = add_hash (animal_colors[ind]);
	  else if ((ind = index_lookup (light_object_colors, buf)) != -1)
	    ch->delay_who = add_hash (light_object_colors[ind]);
	  else if ((ind = index_lookup (stone_colors, buf)) != -1)
	    ch->delay_who = add_hash (stone_colors[ind]);
	  else if ((ind = index_lookup (beor_art, buf)) != -1)
	    ch->delay_who = add_hash (beor_art[ind]);
	  else if ((ind = index_lookup (war_art, buf)) != -1)
	    ch->delay_who = add_hash (war_art[ind]);
	  else if ((ind = index_lookup (dark_object_colors, buf)) != -1)
	    ch->delay_who = add_hash (dark_object_colors[ind]);
	  else if ((ind = index_lookup (hardwood_colors, buf)) != -1)
	    ch->delay_who = add_hash (hardwood_colors[ind]);
	  else if ((ind = index_lookup (metal_damage_colors, buf)) != -1)
	    ch->delay_who = add_hash (metal_damage_colors[ind]);
	  else if ((ind = index_lookup (orc_colors, buf)) != -1)
	    ch->delay_who = add_hash (orc_colors[ind]);
	  else if ((ind = index_lookup (fine_object_colors, buf)) != -1)
	    ch->delay_who = add_hash (fine_object_colors[ind]);
	  else if ((ind = index_lookup (drab_object_colors, buf)) != -1)
	    ch->delay_who = add_hash (drab_object_colors[ind]);
	  if (ind != -1)
	    {
              sprintf(color, "%s", ch->delay_who);
	      argument = one_argument (argument, buf);
              ind = 0;
  
  	    if ((ind = index_lookup (standard_object_colors, buf)) != -1)
	      ch->delay_who = add_hash (standard_object_colors[ind]);
	    else if ((ind = index_lookup (fine_gem_colors, buf)) != -1)
	      ch->delay_who = add_hash (fine_gem_colors[ind]);
	    else if ((ind = index_lookup (gem_colors, buf)) != -1)
	      ch->delay_who = add_hash (gem_colors[ind]);
	    else if ((ind = index_lookup (blade_type_colors, buf)) != -1)
	      ch->delay_who = add_hash (blade_type_colors[ind]);
	    else if ((ind = index_lookup (design_type_colors, buf)) != -1)
	      ch->delay_who = add_hash (design_type_colors[ind]);
	    else if ((ind = index_lookup (animal_colors, buf)) != -1)
	      ch->delay_who = add_hash (animal_colors[ind]);
	    else if ((ind = index_lookup (light_object_colors, buf)) != -1)
	      ch->delay_who = add_hash (light_object_colors[ind]);
	    else if ((ind = index_lookup (stone_colors, buf)) != -1)
	      ch->delay_who = add_hash (stone_colors[ind]);
	    else if ((ind = index_lookup (beor_art, buf)) != -1)
	      ch->delay_who = add_hash (beor_art[ind]);
	    else if ((ind = index_lookup (war_art, buf)) != -1)
	      ch->delay_who = add_hash (war_art[ind]);
	    else if ((ind = index_lookup (dark_object_colors, buf)) != -1)
	      ch->delay_who = add_hash (dark_object_colors[ind]);
	    else if ((ind = index_lookup (hardwood_colors, buf)) != -1)
	      ch->delay_who = add_hash (hardwood_colors[ind]);
	    else if ((ind = index_lookup (metal_damage_colors, buf)) != -1)
	      ch->delay_who = add_hash (metal_damage_colors[ind]);
	    else if ((ind = index_lookup (orc_colors, buf)) != -1)
	      ch->delay_who = add_hash (orc_colors[ind]);
	    else if ((ind = index_lookup (fine_object_colors, buf)) != -1)
	      ch->delay_who = add_hash (fine_object_colors[ind]);
	    else if ((ind = index_lookup (drab_object_colors, buf)) != -1)
	      ch->delay_who = add_hash (drab_object_colors[ind]);

            if (ind != -1)
	      {
                sprintf(color2, "%s", ch->delay_who);
	        argument = one_argument (argument, buf);
                ind = 0;

              if ((ind = index_lookup (standard_object_colors, buf)) != -1)
                ch->delay_who = add_hash (standard_object_colors[ind]);
	      else if ((ind = index_lookup (fine_gem_colors, buf)) != -1)
	        ch->delay_who = add_hash (fine_gem_colors[ind]);
	      else if ((ind = index_lookup (gem_colors, buf)) != -1)
	        ch->delay_who = add_hash (gem_colors[ind]);
	      else if ((ind = index_lookup (blade_type_colors, buf)) != -1)
	        ch->delay_who = add_hash (blade_type_colors[ind]);
	      else if ((ind = index_lookup (design_type_colors, buf)) != -1)
	        ch->delay_who = add_hash (design_type_colors[ind]);
	      else if ((ind = index_lookup (animal_colors, buf)) != -1)
	        ch->delay_who = add_hash (animal_colors[ind]);
	      else if ((ind = index_lookup (light_object_colors, buf)) != -1)
	        ch->delay_who = add_hash (light_object_colors[ind]);
	      else if ((ind = index_lookup (stone_colors, buf)) != -1)
	        ch->delay_who = add_hash (stone_colors[ind]);
  	      else if ((ind = index_lookup (beor_art, buf)) != -1)
	        ch->delay_who = add_hash (beor_art[ind]);
  	      else if ((ind = index_lookup (war_art, buf)) != -1)
	        ch->delay_who = add_hash (war_art[ind]);
	      else if ((ind = index_lookup (dark_object_colors, buf)) != -1)
	        ch->delay_who = add_hash (dark_object_colors[ind]);
	      else if ((ind = index_lookup (hardwood_colors, buf)) != -1)
	        ch->delay_who = add_hash (hardwood_colors[ind]);
	      else if ((ind = index_lookup (metal_damage_colors, buf)) != -1)
	        ch->delay_who = add_hash (metal_damage_colors[ind]);
	      else if ((ind = index_lookup (orc_colors, buf)) != -1)
	        ch->delay_who = add_hash (orc_colors[ind]);
	      else if ((ind = index_lookup (fine_object_colors, buf)) != -1)
	        ch->delay_who = add_hash (fine_object_colors[ind]);
              else if ((ind = index_lookup (drab_object_colors, buf)) != -1)
	        ch->delay_who = add_hash (drab_object_colors[ind]);

              if (ind != -1)
	      {
                sprintf(color3, "%s", ch->delay_who);
	        argument = one_argument (argument, buf);
                ind = 0;
              }
            }

	    if (!(obj = load_colored_object (number, color, color2, color3)))
	    {
	      send_to_char ("There is no object with that number.\n", ch);
	      return;
	    }

	  }
	  else
	    {
	      if (!(obj = load_object (number)))
		{
		  send_to_char ("There is no object with that number.\n", ch);
		  return;
		}
	    }
	}
      else
	{
	  if (!(obj = load_object (number)))
	    {
	      send_to_char ("There is no object with that number.\n", ch);
	      return;
	    }
	}

      ch->delay_who = NULL;

      obj->count = count;

      mob = NULL;

      if (obj->nVirtual == VNUM_PENNY || obj->nVirtual == VNUM_FARTHING)
	name_money (obj);

      if (*buf)
	{
	  if (!(mob = get_char_room_vis (ch, buf)))
	    {
	      send_to_char ("Mob was not located.\n", ch);
	      extract_obj (obj);
	      return;
	    }
	}

      if (mob)
	{
	  act ("$p appears in your hands.", true, mob, obj, ch, TO_CHAR);
	  if (ch != mob)
	    act ("$p was created in $N's hands.",
		 true, ch, obj, mob, TO_CHAR);
	  obj_to_char (obj, mob);
	}
      else
	{
	  obj_to_room (obj, ch->in_room);

	  act ("$p immediately appears!", false, ch, obj, 0, TO_ROOM);
	  act ("$p appears.", false, ch, obj, 0, TO_CHAR);
	}
    }
  else
    send_to_char ("Use keyword char, obj or money.\n", ch);

  room = vtor (ch->in_room);

}


/* clean a room of all mobiles and objects */
void
do_purge (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *vict, *next_v;
  OBJ_DATA *obj, *next_o;
  char name[100];

  if (IS_NPC (ch))
    return;

  one_argument (argument, name);

  if (*name)			/* argument supplied. destroy single object or char */
    {
      if ((vict = get_char_room_vis (ch, name)))
	{
	  if (!IS_NPC (vict) && !vict->pc->admin_loaded)
	    {
	      send_to_char ("\nNow, now. Let's not be hostile, mmmkay?\n",
			    ch);
	      return;
	    }
	  act ("$n disintegrates $N.", false, ch, 0, vict, TO_ROOM);
	  act ("You disintegrate $N.", false, ch, 0, vict, TO_CHAR);
	  if (vict->mount)
	    vict->mount->mount = NULL;
	  vict->mount = NULL;
	  extract_char (vict);
	}

      else if ((obj = get_obj_in_list_vis (ch, name,
					   vtor (ch->in_room)->contents)))
	{
	  act ("$n destroys $p.", false, ch, obj, 0, TO_ROOM);
	  act ("You destroy $p.", false, ch, obj, 0, TO_CHAR);
	  extract_obj (obj);
	}
      else
	{
	  send_to_char ("I don't know anyone or anything by that name.\n",
			ch);
	  return;
	}

    }
  else				/* no argument. clean out the room */
    {
      if (IS_NPC (ch))
	{
	  send_to_char ("Don't... You would only kill yourself..\n", ch);
	  return;
	}

      if (!ch->room->contents)
	{
	  send_to_char
	    ("There aren't any objects that need purging in this room.\n",
	     ch);
	  return;
	}

      act
	("$n briefly narrows $s eyes. A moment later, the clutter in the area dissipates away in a glimmering haze of binary code.",
	 false, ch, 0, 0, TO_ROOM | _ACT_FORMAT);
      act
	("You narrow your eyes, concentrating. A moment later, the clutter in the area dissipates away in a glimmering haze of binary code.",
	 false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);

      for (vict = vtor (ch->in_room)->people; vict; vict = next_v)
	{
	  next_v = vict->next_in_room;
	  if (IS_NPC (vict))
	    {
	      if (IS_SET (vict->hmflags, HM_CLONE))
		extract_char (vict);
	      else
		{
		  act ("$N is a master mobile.  Kill it if you really want "
		       "it destroyed.",
		       false, ch, 0, vict, TO_CHAR | _ACT_FORMAT);
		}
	    }
	}

      for (obj = vtor (ch->in_room)->contents; obj; obj = next_o)
	{
	  next_o = obj->next_content;
	  if (GET_ITEM_TYPE (obj) == ITEM_DWELLING
	      && obj->o.od.value[0] >= 100000)
	    {
	      act
		("$p is a functional player dwelling. Purge it manually to remove it.",
		 false, ch, obj, NULL, TO_CHAR | _ACT_FORMAT);
	      continue;
	    }
           else if (IS_SET (obj->obj_flags.extra_flags, ITEM_NOPURGE))
	    {
	      act
		("$p is a no-purge object, probably scenery. Purge it manually to remove it.",
		 false, ch, obj, NULL, TO_CHAR | _ACT_FORMAT);
	      continue;
	    }             
	  extract_obj (obj);
	}
    }
}

void
do_restore (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *victim;
  char buf[100];

  one_argument (argument, buf);
  if (!*buf)
    send_to_char ("Who do you wish to restore?\n", ch);
  else
    {
      if (!(victim = get_char_room_vis (ch, buf)))
	send_to_char ("I don't see that person here.\n", ch);
      else
	{
	  heal_all_wounds (victim);
	  victim->move = victim->max_move;
	  send_to_char ("Done.\n", ch);
	  act ("You have been fully healed by $N!",
	       false, victim, 0, ch, TO_CHAR);
	  GET_POS (victim) = STAND;
	  remove_cover(victim, 0);
	}
    }
}

void
do_zlock (CHAR_DATA * ch, char *argument, int cmd)
{
  char arg1[80], arg2[80];
  int zone;

  half_chop (argument, arg1, arg2);

  if (!*arg1)
    {
      send_to_char ("Which zone would you like to lock?\n", ch);
      return;
    }
  if (!*arg2)
    {
      send_to_char ("Turn lock on or off?\n", ch);
      return;
    }
  zone = atoi (arg1);
  if ((zone < 0) || (zone > 99))
    {
      send_to_char ("Zone must be between 0 and 99.\n", ch);
      return;
    }
  if (!strncmp (arg2, "on", 2))
    {
      if (IS_SET (zone_table[zone].flags, Z_LOCKED))
	{
	  send_to_char ("Zone is already locked.\n", ch);
	  return;
	}
      else
	{
	  zone_table[zone].flags |= Z_LOCKED;
	  send_to_char ("Zone Locked.\n", ch);
	  return;
	}
    }
  else if (!strncmp (arg2, "of", 2))
    {
      if (!IS_SET (zone_table[zone].flags, Z_LOCKED))
	{
	  send_to_char ("Zone was not locked.\n", ch);
	  return;
	}
      else
	{
	  zone_table[zone].flags &= ~Z_LOCKED;
	  send_to_char ("Zone Unlocked.\n", ch);
	  return;
	}
    }
  else
    send_to_char ("Lock or Unlock which zone?\n", ch);
}

void
do_invis (CHAR_DATA * ch, char *argument, int cmd)
{
  if (!IS_SET (ch->flags, FLAG_WIZINVIS))
    {
      ch->flags |= FLAG_WIZINVIS;
      send_to_char ("You dematerialize.\n", ch);
    }
  else
    {
      send_to_char ("You are already invisible.\n", ch);
    }
}

void
do_vis (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  AFFECTED_TYPE *af;

  if ((af = get_affect (ch, MAGIC_AFFECT_INVISIBILITY)))
    affect_remove (ch, af);

  if (IS_SET (ch->flags, FLAG_WIZINVIS))
    {
      ch->flags &= ~FLAG_WIZINVIS;
      remove_affect_type (ch, MAGIC_HIDDEN);

      strcpy (buf, "$n materializes before your eyes.");
      act (buf, false, ch, 0, 0, TO_ROOM);

      send_to_char ("You materialize.\n", ch);
    }

  else if (get_affect (ch, MAGIC_HIDDEN))
    {
      remove_affect_type (ch, MAGIC_HIDDEN);
      send_to_char ("You reveal yourself.\n", ch);
      act ("$n reveals $mself.", false, ch, 0, 0, TO_ROOM);
    }

  else
    send_to_char ("You are already visible.\n", ch);
}

void
fix_shops (CHAR_DATA * ch)
{
  CHAR_DATA *sch;
  OBJ_DATA *obj;
  char buf[MAX_STRING_LENGTH];

  for (sch = character_list; sch; sch = sch->next)
    {

      if (sch->deleted)
	continue;

      if (!sch->shop)
	continue;

      if (!sch->shop->store_vnum)
	continue;

      sprintf (buf, "%05d %-12.12s %05d %s",
	       sch->mob->nVirtual, GET_NAME (sch), sch->shop->store_vnum,
	       !sch->shop->store_vnum ? "" :
	       (vtor (sch->shop->store_vnum) ? "" : "None"));
      send_to_char (buf, ch);

      obj = get_obj_in_list_num (42, sch->right_hand);
      if (!obj)
	obj = get_obj_in_list_num (42, sch->left_hand);

      if (!obj)
	{
	  obj = create_money (number (300, 600));
	  obj_to_char (obj, sch);
	  sprintf (buf, "; made amount %f\n", obj->farthings);
	  send_to_char (buf, ch);
	}
      else
	{
	  sprintf (buf, "; CURRENT: %f\n", obj->farthings);
	  send_to_char (buf, ch);
	}
    }
}

void
read_race_data (CHAR_DATA * ch)
{
  FILE *fp;
  char buf[MAX_STRING_LENGTH];
  char *p;
  int vnum;
  char race[MAX_STRING_LENGTH];
  int count = 0;
  CHAR_DATA *mob;
  int ind;

  if (!(fp = fopen ("race.done", "r")))
    {
      perror ("lib/race.done");
      return;
    }

  while (fgets (buf, 256, fp))
    {

      p = one_argument (buf, race);

      vnum = atoi (race);

      p = one_argument (p, race);

      ind = lookup_race_id (race);

      if (ind == -1)
	printf ("%05d  %15s; INVALID RACE NAME.", vnum, race);
      else
	printf ("%05d  %15s = %s", vnum, race,
		lookup_race_variable (ind, RACE_NAME));

      if (!(mob = vtom (vnum)))
	printf ("INVALID VNUM!\n");
      else
	{
	  if (ind != -1)
	    mob->race = ind;
	  printf ("\n");
	}

      count++;
    }

  fclose (fp);

  printf ("%d mobs processed.\n", count);
}

void
debug_info (CHAR_DATA * ch, char *argument)
{
  CHAR_DATA *target;
  CHAR_DATA *other;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("debug info <target n/pc> [<other n/pc>]\n", ch);
      send_to_char ("   Perception is of target on other.\n", ch);
      return;
    }

  if (!(target = get_char_room (buf, ch->in_room)))
    {
      send_to_char ("No such target.\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (!*buf)
    other = ch;
  else if (!(other = get_char_room (buf, ch->in_room)))
    {
      send_to_char ("No such other.\n", ch);
      return;
    }

  sprintf (buf, "is_brother (target, other):   %s\n",
	   is_brother (target, other) ? "true" : "false");
  send_to_char (buf, ch);

  sprintf (buf, "is_leader (target, other:     %s\n",
	   is_leader (target, other) ? "true" : "false");
  send_to_char (buf, ch);

  sprintf (buf, "is_area_enforcer (target):    %s\n",
	   is_area_enforcer (target) ? "true" : "false");
  send_to_char (buf, ch);

  sprintf (buf, "is_area_leader (target):      %s\n",
	   is_area_leader (target) ? "true" : "false");
  send_to_char (buf, ch);
}

void
write_obj_values (CHAR_DATA * ch)
{
  int i;
  int obj_load_count;
  OBJ_DATA *obj;
  OBJ_DATA *obj2;
  FILE *fp;

  if (!(fp = fopen ("obj_values", "w")))
    {
      send_to_char ("Unable to open file lib/obj_values.\n", ch);
      return;
    }

  /* There are about 45 object types when this was written */

  for (i = 0; i < 50; i++)
    {
      for (obj = full_object_list; obj; obj = obj->lnext)
	{
	  if (obj->obj_flags.type_flag != i)
	    continue;

	  obj_load_count = 0;

	  for (obj2 = object_list; obj2; obj2 = obj2->next)
	    if (obj2->nVirtual == obj->nVirtual)
	      obj_load_count++;

	  fprintf (fp, "%05d  %4d %d [%s:%d] %s\n",
		   obj->nVirtual,
		   (int) obj->silver,
		   (int) obj->farthings,
		   item_types[(int) obj->obj_flags.type_flag],
		   obj_load_count, obj->short_description);
	}
    }

  fclose (fp);
}

void
write_obj_silver_farthing_values (CHAR_DATA * ch)
{
  OBJ_DATA *obj;
  FILE *fp;

  if (!(fp = fopen ("obj_values_sf", "w")))
    {
      send_to_char ("Unable to open file lib/obj_values_sf.\n", ch);
      return;
    }

  /* There are about 45 object types when this was written */

  for (obj = full_object_list; obj; obj = obj->lnext)
    {

      if (!obj->farthings)
	continue;

      fprintf (fp, "%05d  %4d %d\n",
	       obj->nVirtual, (int) obj->silver, (int) obj->farthings);
    }

  fclose (fp);
}

void
read_obj_silver_farthing_values (CHAR_DATA * ch)
{
  int ns;
  int nVirtual;
  int silver;
  int farthing;
  char buf[MAX_STRING_LENGTH];
  FILE *fp;
  OBJ_DATA *obj;

  if (!(fp = fopen ("obj_values_sf", "r")))
    {
      send_to_char ("Unable to open file lib/obj_values_sf.\n", ch);
      return;
    }

  while (!feof (fp))
    {

      ns = fscanf (fp, "%d %d %d\n", &nVirtual, &silver, &farthing);

      if (ns != 3)
	{
	  send_to_char ("Scan of less than 3.\n", ch);
	  return;
	}

      if (!(obj = vtoo (nVirtual)))
	{
	  sprintf (buf, "Object %d missing.\n", nVirtual);
	  send_to_char (buf, ch);
	  continue;
	}

      if (obj->silver != silver)
	{
	  sprintf (buf, "Object %d had silver %d->%d\n", nVirtual,
		   (int) obj->silver, silver);
	  send_to_char (buf, ch);
	}

      if (obj->farthings)
	{
	  sprintf (buf, "Object %d farthings  %d->%d\n", nVirtual,
		   (int) obj->farthings, farthing);
	  send_to_char (buf, ch);
	}

      if (farthing)
	{
	  sprintf (buf, "Object %d %d:%d ->  %d:%d\n", nVirtual,
		   (int) obj->silver, (int) obj->farthings, silver, farthing);
	  send_to_char (buf, ch);
	  obj->farthings = farthing;
	  redefine_objects (obj);
	}
    }
}


void
update_obj_values (CHAR_DATA * ch)
{
  int vnum;
  int farthings;
  int pennies;
  int line = 0;
  int unchanged = 0;
  int changed = 0;
  int zeroed = 0;
  int instances = 0;
  OBJ_DATA *obj;
  char *argument;
  FILE *fp;
  char buf[MAX_STRING_LENGTH];
  char token[MAX_STRING_LENGTH];

  if (!(fp = fopen ("obj_values_updated", "r")))
    {
      send_to_char ("Unable to open file lib/obj_values_updated.\n", ch);
      return;
    }

  while (fgets (buf, 256, fp))
    {

      line++;

      argument = one_argument (buf, token);

      if (!just_a_number (token))
	{
	  sprintf (token, "Error on line %d:  %s\n", line, buf);
	  send_to_char (token, ch);
	  return;
	}

      vnum = atoi (token);

      argument = one_argument (argument, token);

      if (!just_a_number (token))
	{
	  sprintf (token, "Error on line %d:  %s\n", line, buf);
	  send_to_char (token, ch);
	  return;
	}

      pennies = atoi (token);

      argument = one_argument (argument, token);

      if (!just_a_number (token))
	{
	  sprintf (token, "Error on line %d:  %s\n", line, buf);
	  send_to_char (token, ch);
	  return;
	}

      farthings = atoi (token);

      if (!(obj = vtoo (vnum)))
	{
	  sprintf (token, "Vnum %d no longer exists.\n", vnum);
	  send_to_char (token, ch);
	  continue;
	}

      if (obj->farthings == farthings && obj->silver == pennies)
	{
	  unchanged++;
	  continue;
	}

      if (farthings == 0 && pennies == 0)
	{
	  zeroed++;
	  continue;
	}

      if (vnum > 30 && vnum < 50)
	{
	  sprintf (token, "Vnum %d cannot be changed.\n", vnum);
	  send_to_char (token, ch);
	  continue;
	}

      changed++;

      obj->farthings = farthings;
      obj->silver = pennies;

      instances += redefine_objects (obj);
    }

  fclose (fp);

  sprintf (buf, "%d unchanged objects.\n", unchanged);
  send_to_char (buf, ch);

  sprintf (buf, "%d had zero values and not updated.\n", zeroed);
  send_to_char (buf, ch);

  sprintf (buf, "%d prototypes changed, affecting %d instances.\n",
	   changed, instances);
  send_to_char (buf, ch);
}

void
do_debug (CHAR_DATA * ch, char *argument, int cmd)
{
  int i;
  OBJ_DATA *obj;
  CHAR_DATA *mob;
  char buf[MAX_INPUT_LENGTH];

  argument = one_argument (argument, buf);

  if (!str_cmp (buf, "writefarthings"))
    {
      write_obj_silver_farthing_values (ch);
      return;
    }

  if (!str_cmp (buf, "readfarthings"))
    {
      read_obj_silver_farthing_values (ch);
      return;
    }

  if (!str_cmp (buf, "updatevalues"))
    {
      update_obj_values (ch);
      return;
    }

  if (!str_cmp (buf, "cdr"))
    {
      mob = vtom (14071);
      if (!mob)
	{
	  send_to_char ("Mob not defined:  14071\n", ch);
	  return;
	}
      sprintf (buf, "%d: %s\n", mob->mob->nVirtual, mob->clans);
      send_to_char (buf, ch);
      return;
    }

  if (!str_cmp (buf, "value"))
    {
      write_obj_values (ch);
      return;
    }

  if (!str_cmp (buf, "quiet"))
    {
      ch->act |= PLR_QUIET;
      return;
    }

  if (!str_cmp (buf, "loud"))
    {
      ch->act &= ~PLR_QUIET;
      return;
    }

  if (!str_cmp (buf, "info"))
    {
      debug_info (ch, argument);
      return;
    }

  if (!str_cmp (buf, "help"))
    {
      write_help ("testhelp", help_list);
      return;
    }

  if (!str_cmp (buf, "races"))
    {
      read_race_data (ch);
      return;
    }

  if (!str_cmp (buf, "shops"))
    {
      fix_shops (ch);
      return;
    }

  if (!str_cmp (buf, "createobj"))
    {
      for (i = 0; i < 100; i++)
	{
	  obj = load_object (5);
	  obj_to_room (obj, 1);
	  extract_obj (obj);
	  cleanup_the_dead (2);
	}
    }

  if (!str_cmp (buf, "createmob"))
    {
      for (i = 0; i < 10000; i++)
	{
	  free_char (new_char (0));
	}
    }

  if (!str_cmp (buf, "fighting") || !str_cmp (buf, "fight"))
    {
      if (ch->debug_mode & DEBUG_FIGHT)
	ch->debug_mode &= ~DEBUG_FIGHT;
      else
	ch->debug_mode |= DEBUG_FIGHT;
    }

  else if (!str_cmp (buf, "misc"))
    {
      if (ch->debug_mode & DEBUG_MISC)
	ch->debug_mode &= ~DEBUG_MISC;
      else
	ch->debug_mode |= DEBUG_MISC;
    }

  else if (!str_cmp (buf, "skills"))
    {
      if (ch->debug_mode & DEBUG_SKILLS)
	ch->debug_mode &= ~DEBUG_SKILLS;
      else
	ch->debug_mode |= DEBUG_SKILLS;
    }

  else if (!str_cmp (buf, "summary"))
    {
      if (ch->debug_mode & DEBUG_SUMMARY)
	ch->debug_mode &= ~DEBUG_SUMMARY;
      else
	ch->debug_mode |= DEBUG_SUMMARY;
    }

  else if (!str_cmp (buf, "subdue"))
    {
      if (ch->debug_mode & DEBUG_SUBDUE)
	ch->debug_mode &= ~DEBUG_SUBDUE;
      else
	ch->debug_mode |= DEBUG_SUBDUE;
    }

  else if (!str_cmp (buf, "x1"))
    {
      if (x1)
	x1 = 0;
      else
	x1 = 1;
    }

  else if (!strcmp (buf, "crash"))
    {
      printf ("Debug crash core dump:\n");
      fflush (stdout);
      ((int *) 0)[-1] = 0;
    }

  else
    send_to_char ("The only modes available:\n"
		  "    fighting, skills, misc.\n", ch);

  send_to_char ("Flags set:  ", ch);

  if (IS_SET (ch->debug_mode, DEBUG_FIGHT))
    send_to_char ("FIGHT ", ch);

  if (IS_SET (ch->debug_mode, DEBUG_MISC))
    send_to_char ("MISC ", ch);

  if (IS_SET (ch->debug_mode, DEBUG_SKILLS))
    send_to_char ("SKILLS ", ch);

  if (IS_SET (ch->debug_mode, DEBUG_SUMMARY))
    send_to_char ("SUMMARY ", ch);

  if (IS_SET (ch->debug_mode, DEBUG_SUBDUE))
    send_to_char ("SUBDUE ", ch);

  if (x1)
    send_to_char ("X1 ", ch);

  send_to_char ("\n", ch);
}

void
do_hour (CHAR_DATA * ch, char *argument, int cmd)
{
  next_hour_update = time (0);
  next_minute_update = time (0);

  times_do_hour_called++;
}

void
do_day (CHAR_DATA * ch, char *argument, int cmd)
{
  int i = 0, times = 0;

  if (*argument && isdigit (*argument))
    times = atoi (argument);

  if (!times)
    times = 1;

  for (i = 1; i <= times; i++)
    {

      time_info.hour = 24;

      next_hour_update = time (0);
      next_minute_update = time (0);

      weather_and_time (1);
    }
}

void
char__unbalance (CHAR_DATA * ch, float nMultiplier)
{

  float nPenalty = 0.0;

  if (ch->agi <= 9)
    nPenalty = 15;
  else if (ch->agi > 9 && ch->agi <= 13)
    nPenalty = 13;
  else if (ch->agi > 13 && ch->agi <= 15)
    nPenalty = 11;
  else if (ch->agi > 15 && ch->agi <= 18)
    nPenalty = 9;
  else
    nPenalty = 7;

  ch->balance =
    MAX (ch->balance - (int) (nPenalty * nMultiplier), -50);
}


unsigned char
char__set_fight_mode (CHAR_DATA * ch, unsigned char nMode)
{
  unsigned char nUnBalance;

  if (nMode == ch->fight_mode)
    {
      send_to_char ("You are already in that combat mode.\n", ch);
      if (GET_FLAG (ch, FLAG_PACIFIST))
	{
	  send_to_char ("Use 'set pacifist' to return attacks again.\n", ch);
	}
      return 0;
    }

  if (ch->fighting)
    {
      if (ch->balance < -15)
	{
	  send_to_char
	    ("You fail to get the footing you need to change your fighting stance!\n",
	     ch);
	  return 0;
	}
      nUnBalance =
	(nMode >
	 ch->fight_mode) ? (nMode - ch->fight_mode) : (ch->fight_mode -
						       nMode);
      char__unbalance (ch, nUnBalance);
    }

  if (nMode < 5)
    {
      ch->fight_mode = nMode;
    }

  return 1;
}

void
do_keywords (CHAR_DATA *ch, char *argument)
{
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  sprintf(buf, "%s", keyword_gen(ch, ch->short_descr, 0));

  if(!*argument)
  {
    sprintf(buf2, "\nKeywords reverted to default: #6%s#0.\n", buf);
  }
  else
  {
    sprintf(buf + strlen(buf), "%s", keyword_gen(ch, argument, 1));
    sprintf(buf2, "\nKeywords update to: #6%s#0.\n", buf);
  }

  send_to_char(buf2, ch);
  ch->name = add_hash(buf);
}

void
do_set (CHAR_DATA * ch, char *argument, int cmd)
{
  int ind;
  char buf[MAX_INPUT_LENGTH];
  char subcmd[MAX_INPUT_LENGTH];
  AFFECTED_TYPE *af;
  bool crippled = false, fury = false;

  argument = one_argument (argument, subcmd);

  if((af = get_soma_affect(ch, SOMA_PLANT_FURY)))
    fury = true;
  else if((af = get_soma_affect(ch, SOMA_BLUNT_SEVLEG)))
    crippled = true;

  if (!str_cmp (subcmd, "scan"))
    {
	  if (!IS_SET (ch->plr_flags, QUIET_SCAN))
		{  
    	  ch->plr_flags |= QUIET_SCAN;
    	  send_to_char
	    	("You will now scan surrounding rooms.\n", ch);
	  	  return;
		}
	
      ch->plr_flags &= ~QUIET_SCAN;
      send_to_char
		("You will no longer scan surrounding rooms.\n", ch);
      return;
    }

  if (!str_cmp (subcmd, "autofire"))
  {
    if (!IS_SET (ch->plr_flags, AUTO_FIRE))
    {  
      ch->plr_flags |= AUTO_FIRE;
      send_to_char("You will now automatically fire at targets moving out of range.\n", ch);
      return;
    }
    ch->plr_flags &= ~AUTO_FIRE;
    send_to_char("You will no longer automatically fire at targets moving out of range.\n", ch);
    return;
  }

  if (!str_cmp (subcmd, "highlight"))
  {
    if (!IS_SET (ch->plr_flags, HIGHLIGHT))
    {  
      ch->plr_flags |= HIGHLIGHT;
      send_to_char("Melee attacks by you and aimed at you are now highlighted.\n", ch);
      return;
    }
    ch->plr_flags &= ~HIGHLIGHT;
    send_to_char("Melee attacks by you and aimed at you are no longer highlighted..\n", ch);
    return;
  }


  if (!str_cmp (subcmd, "keywords"))
  {
    do_keywords(ch, argument);
    return;
  }
    
  if (!str_cmp (subcmd, "hints"))
    {
      if (!IS_SET (ch->plr_flags, NEWBIE_HINTS))
	{
	  ch->plr_flags |= NEWBIE_HINTS;
	  send_to_char
	    ("You will now periodically receive game- and syntax-related hints.\n",
	     ch);
	  return;
	}
      ch->plr_flags &= ~NEWBIE_HINTS;
      send_to_char
	("You will no longer receive game- and syntax-related hints.\n", ch);
      return;
    }

  else if (!str_cmp (subcmd, "combat"))
    {
      if (!IS_SET (ch->plr_flags, COMBAT_FILTER))
	{
	  ch->plr_flags |= COMBAT_FILTER;
	  send_to_char
	    ("You will now only see local combat messages on your screen.\n",
	     ch);
	  return;
	}
      ch->plr_flags &= ~COMBAT_FILTER;
      send_to_char ("You will now see all combat messages in your area.\n",
		    ch);
      return;
    }

  else if (!str_cmp (subcmd, "voting") && ch->desc && ch->desc->acct)
    {
      if (ch->desc->acct->toggle_ignore_vote_notes ()) 
	{
	  send_to_char ("You will no longer receive voting reminders "
			"upon entering the game.\n", ch);
	}
      else
	{
	  send_to_char ("You will now be shown voting reminders "
			"upon entering the game.\n", ch);
	}

      return;
    }

  else if (!str_cmp (subcmd, "rp") && ch->desc && ch->desc->acct)
    {
      if (IS_SET (ch->flags, FLAG_GUEST))
	{
	  send_to_char ("This option is unavailable to guest logins.\n", ch);
	  return;
	}

      if (ch->desc->acct->toggle_rpp_visibility ())
	{
	  send_to_char ("The output to SCORE will now show "
			"your number of reward points.\n", ch);
	}
      else
	{
	  send_to_char ("Your reward points will no longer "
			"be visible in SCORE.\n", ch);
	}
      return;
    }

  else if (!str_cmp (subcmd, "ansi"))
    {
      if (!IS_SET (ch->flags, FLAG_GUEST))
	{
	  send_to_char ("This switch is only for guest users.\n", ch);
	  return;
	}
      if (ch->desc->color)
	{
	  ch->desc->color = 0;
	  send_to_char ("ANSI color has been disabled.\n", ch);
	  return;
	}
      ch->desc->color = 1;
      send_to_char ("#2ANSI color has been enabled.#0\n", ch);
      return;
    }

  else if (!str_cmp (subcmd, "mute"))
    {
      if (!IS_SET (ch->plr_flags, MUTE_BEEPS))
	{
	  ch->plr_flags |= MUTE_BEEPS;
	  send_to_char
	    ("You will now no longer hear system beeps upon receiving notifies.\n",
	     ch);
	  return;
	}
      ch->plr_flags &= ~MUTE_BEEPS;
      send_to_char
	("You will now hear system beeps upon receiving notifies.\n", ch);
      return;
    }

  else if (!str_cmp (subcmd, "newbie"))
    {
      if (IS_SET (ch->plr_flags, NEW_PLAYER_TAG))
	{
	  ch->plr_flags &= ~NEW_PLAYER_TAG;
	  send_to_char
	    ("The #2(new player)#0 tag has been removed from your long description.\n",
	     ch);
	  return;
	}
      if (!IS_MORTAL (ch))
	{
	  ch->plr_flags |= NEW_PLAYER_TAG;
	  send_to_char ("New player tag has been enabled.\n", ch);
	  return;
	}
      send_to_char
	("Shame on you - trying to pass yourself off as a clueless newbie?\n",
	 ch);
      return;
    }

  else if (!str_cmp (subcmd, "mentor"))
    {
      if (IS_SET (ch->flags, FLAG_GUEST))
	{
	  send_to_char ("Guests cannot toggle this flag.\n", ch);
	  return;
	}
      if (!IS_SET (ch->plr_flags, MENTOR))
	{
	  act
	    ("Your mentor flag has been enabled. Be SURE to read the policies listed in #6HELP MENTOR#0 - you WILL be held accountable to them. If you don't agree to them, please type SET MENTOR again now to remove the flag.",
	     false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
	  ch->plr_flags |= MENTOR;
	  return;
	}
      send_to_char ("Your mentor flag has been disabled.\n", ch);
      ch->plr_flags &= ~MENTOR;
      return;
    }

  else if (!str_cmp (subcmd, "prompt"))
    {
      if (!GET_FLAG (ch, FLAG_NOPROMPT))
	{
	  ch->flags |= FLAG_NOPROMPT;
	  send_to_char
	    ("Prompt disabled. Use the HEALTH command to check your character's welfare.\n",
	     ch);
	  return;
	}
      else
	ch->flags &= ~FLAG_NOPROMPT;
      send_to_char ("Informative prompt enabled.\n", ch);
      return;
    }

  else if (!str_cmp (subcmd, "mana"))
    {
      if (!ch->max_mana)
	{
	  send_to_char ("You are not a spellcaster.\n", ch);
	  return;
	}
      if (GET_FLAG (ch, FLAG_HARNESS))
	{
	  ch->flags &= ~FLAG_HARNESS;
	  send_to_char ("Done.\n", ch);
	  return;
	}
      else
	ch->flags |= FLAG_HARNESS;
      send_to_char ("Done.\n", ch);
      return;
    }

  else if (!str_cmp (subcmd, "pacifist"))
    {
      if(fury && !get_trust(ch))
      {
        send_to_char("The rage inside of you will not tolerate such a fighting style.\n", ch);
        ch->fight_mode = 1;
        return;
      }
      if (char__set_fight_mode (ch, 5))
	{

	  if (GET_FLAG (ch, FLAG_PACIFIST))
	    {
	      ch->flags &= ~FLAG_PACIFIST;
	      send_to_char ("You will now return attacks in combat.\n", ch);
	    }
	  else
	    {
	      ch->flags |= FLAG_PACIFIST;
	      send_to_char
		("You will now forego returning attacks in combat.\n", ch);
	    }

	}
      return;
    }

  else if (!str_cmp (subcmd, "wiznet")
	   && ((ch->pc && ch->pc->level) 
	       || IS_SET (ch->flags, FLAG_ISADMIN)
	       || (IS_NPC (ch) && ch->desc->original)))
    {
      CHAR_DATA* real_ch = 
	(IS_NPC(ch) && ch->desc->original)
	? ch->desc->original
	: ch;

      if (GET_FLAG (real_ch, FLAG_WIZNET))
	{
	  real_ch->flags &= ~FLAG_WIZNET;
	  send_to_char ("You are no longer tuned into the wiznet.\n", ch);
	  sprintf (buf, "%s has left the wiznet channel.\n", GET_NAME (real_ch));
	  send_to_gods (buf);
	}

      else
	{
	  real_ch->flags |= FLAG_WIZNET;
	  send_to_char ("You are now tuned into the wiznet.\n", ch);
	  sprintf (buf, "%s has rejoined the wiznet channel.\n",
		   GET_NAME (real_ch));
	  send_to_gods (buf);
	}
      return;
    }

  else if (ch->pc && ch->pc->level && !str_cmp (subcmd, "names"))
    {
      if (GET_FLAG (ch, FLAG_SEE_NAME))
	{
	  ch->flags &= ~FLAG_SEE_NAME;
	  send_to_char ("'SAY' will no longer show player names.\n", ch);
	}

      else
	{
	  ch->flags |= FLAG_SEE_NAME;
	  send_to_char ("'SAY' will show player names.\n", ch);
	}

      return;
    }

  else if (!IS_MORTAL (ch) && !str_cmp (subcmd, "telepath"))
    {
      if (GET_FLAG (ch, FLAG_TELEPATH))
	{
	  ch->flags &= ~FLAG_TELEPATH;
	  send_to_char ("You will no longer receive PC thoughts.\n", ch);
	}
      else
	{
	  send_to_char
	    ("You are now attuned to PC thoughts -- may the gods help you.\n",
	     ch);
	  ch->flags |= FLAG_TELEPATH;
	}
    }

  else if (!IS_MORTAL (ch) && !str_cmp (subcmd, "guardian"))
    {

      argument = one_argument (argument, buf);

      if (!*buf)
	{
	  /* Toggle ON | OFF */
	  if (ch->guardian_mode)
	    {
	      ch->guardian_mode = 0;
	    }
	  else
	    {
	      ch->guardian_mode = 0xFFFF;
	    }
	}

      else if (!str_cmp (buf, "on") || !str_cmp (buf, "all"))
	{
	  ch->guardian_mode = 0xFFFF;
	}

      else if (!str_cmp (buf, "off") || !str_cmp (buf, "none"))
	{
	  ch->guardian_mode = 0;
	}

      else if (!str_cmp (buf, "pc"))
	{
	  TOGGLE_BIT (ch->guardian_mode, GUARDIAN_PC);
	}
      else if (!str_cmp (buf, "npc"))
	{
	  if ((ch->guardian_mode & ~GUARDIAN_PC))
	    ch->guardian_mode &= GUARDIAN_PC;
	  else
	    {
	      ch->guardian_mode |= ~GUARDIAN_PC;
	    }
	}
      else if (!str_cmp (buf, "humanoids"))
	{
	  TOGGLE_BIT (ch->guardian_mode, GUARDIAN_NPC_HUMANOIDS);
	}
      else if (!str_cmp (buf, "wildlife"))
	{
	  TOGGLE_BIT (ch->guardian_mode, GUARDIAN_NPC_WILDLIFE);
	}
      else if (!str_cmp (buf, "shopkeeps"))
	{
	  TOGGLE_BIT (ch->guardian_mode, GUARDIAN_NPC_SHOPKEEPS);
	}
      else if (!str_cmp (buf, "sentinel"))
	{
	  TOGGLE_BIT (ch->guardian_mode, GUARDIAN_NPC_SENTINELS);
	}
      else if (!str_cmp (buf, "keyholders"))
	{
	  TOGGLE_BIT (ch->guardian_mode, GUARDIAN_NPC_KEYHOLDER);
	}
      else if (!str_cmp (buf, "enforcers"))
	{
	  TOGGLE_BIT (ch->guardian_mode, GUARDIAN_NPC_ENFORCERS);
	}

      if (ch->guardian_mode == 0xFFFF)
	{
	  send_to_char
	    ("You will be informed of any significant PC initiated attacks.\n",
	     ch);
	}
      else if (ch->guardian_mode)
	{
	  strcpy (buf, "You will be informed of PC attacks on:");

	  if ((ch->guardian_mode & GUARDIAN_PC))
	    strcat (buf, " PCs,");

	  if ((ch->guardian_mode & ~GUARDIAN_PC) == 0xFFFE)
	    strcat (buf, " NPCs.");
	  else if ((ch->guardian_mode & ~GUARDIAN_PC))
	    {
	      strcat (buf, " NPC");
	      if ((ch->guardian_mode & GUARDIAN_NPC_HUMANOIDS))
		strcat (buf, " Humanoids,");
	      if ((ch->guardian_mode & GUARDIAN_NPC_WILDLIFE))
		strcat (buf, " Wildlife,");
	      if ((ch->guardian_mode & GUARDIAN_NPC_SHOPKEEPS))
		strcat (buf, " Shopkeeps,");
	      if ((ch->guardian_mode & GUARDIAN_NPC_SENTINELS))
		strcat (buf, " Sentinels,");
	      if ((ch->guardian_mode & GUARDIAN_NPC_KEYHOLDER))
		strcat (buf, " Keyholders,");
	      if ((ch->guardian_mode & GUARDIAN_NPC_ENFORCERS))
		strcat (buf, " Enforcers,");
	    }
	  buf[(strlen (buf) - 1)] = '\n';
	  send_to_char (buf, ch);
	}
      else
	{
	  send_to_char
	    ("You will not be informed of any PC initiated attacks.\n", ch);
	}

    }

  else if (!IS_MORTAL (ch) && !str_cmp (subcmd, "prompt"))
    {
      if (GET_FLAG (ch, FLAG_NOPROMPT))
	ch->flags &= ~FLAG_NOPROMPT;
      else
	{
	  send_to_char
	    ("Prompt removed.  Enter 'set prompt' to get it back.\n", ch);
	  ch->flags |= FLAG_NOPROMPT;
	}
    }

  else if (!IS_MORTAL (ch) && !str_cmp (subcmd, "immwalk"))
    ch->speed = SPEED_IMMORTAL;

  else if (!IS_MORTAL (ch) && !str_cmp (subcmd, "available"))
    {
      if (IS_SET (ch->flags, FLAG_AVAILABLE))
	{
	  ch->flags &= ~FLAG_AVAILABLE;
	  send_to_char
	    ("You will no longer register as available for petitions.\n", ch);
	}

      else
	{
	  ch->flags |= FLAG_AVAILABLE;
	  send_to_char
	    ("You will now be listed as available for petitions.\n", ch);
	}

      return;
    }

  else if (!str_cmp (subcmd, "autoflee"))
    {
      if(fury && !get_trust(ch))
      {
        if (GET_FLAG (ch, FLAG_AUTOFLEE))
          TOGGLE (ch->flags, FLAG_AUTOFLEE);
        send_to_char("The rage inside of you will not tolerate such a fighting style.\n", ch);
        ch->fight_mode = 1;
        return;
      }
      TOGGLE (ch->flags, FLAG_AUTOFLEE);

      if (GET_FLAG (ch, FLAG_AUTOFLEE))
	send_to_char ("You will flee at the first sign of trouble.\n", ch);
      else
	send_to_char ("You will stand and fight from now on.\n", ch);
    }

  else if (!str_cmp (subcmd, "frantic"))
    {

      if(crippled && !get_trust(ch))
      {
        send_to_char("The agonising fracture in your leg prevents such a fighting style.\n", ch);
        ch->fight_mode = 3;
        return;
      }
      else if (char__set_fight_mode (ch, 0))
	{
	  send_to_char ("You start fighting frantically.\n", ch);
	}
    }

  else if (!str_cmp (subcmd, "aggressive"))
    {
      if(crippled && !get_trust(ch))
      {
        send_to_char("The agonising fracture in your leg prevents such a fighting style.\n", ch);
        ch->fight_mode = 3;
        return;
      }
      else if (char__set_fight_mode (ch, 1))
	{
	  send_to_char ("You start fighting more aggressively.\n", ch);
	}
    }

  else if (!str_cmp (subcmd, "normal"))
    {
      if(crippled && !get_trust(ch))
      {
        send_to_char("The agonising fracture in your leg prevents such a fighting style.\n", ch);
        ch->fight_mode = 3;
        return;
      }
      else if(fury && !get_trust(ch))
      {
        send_to_char("The rage inside of you will not tolerate such a fighting style.\n", ch);
        ch->fight_mode = 1;
        return;
      }
      else if (char__set_fight_mode (ch, 2))
	{
	  send_to_char ("You start fighting normally.\n", ch);
	}
    }

  else if (!str_cmp (subcmd, "careful"))
    {
      if(fury && !get_trust(ch))
      {
        send_to_char("The rage inside of you will not tolerate such a fighting style.\n", ch);
        ch->fight_mode = 1;
        return;
      }
        if (char__set_fight_mode (ch, 3))
	{
	  send_to_char ("You start fighting more carefully.\n", ch);
	}
    }
  else if (!str_cmp (subcmd, "defensive"))
    {
      if(fury && !get_trust(ch))
      {
        send_to_char("The rage inside of you will not tolerate such a fighting style.\n", ch);
        ch->fight_mode = 1;
        return;
      }
      if (char__set_fight_mode (ch, 4))
	{
	  send_to_char ("You start fighting more defensively.\n", ch);
	}
    }

  else if (ch->pc && ch->pc->level && !str_cmp (subcmd, "mortal"))
    {
      ch->pc->mortal_mode = 1;
      send_to_char ("Now playing in mortal mode.\n", ch);
    }

  else if (ch->pc && ch->pc->level
	   && !strn_cmp (subcmd, "immortal", strlen (subcmd)))
    {
      ch->pc->mortal_mode = 0;
      send_to_char ("Returning to immortal mode.\n", ch);
    }

  else if (ch->desc && !str_cmp (subcmd, "lines"))
    {

      argument = one_argument (argument, buf);

      if (!isdigit (*buf) || atoi (buf) > 79)
	send_to_char ("Expected a number 1..79", ch);
      else
	ch->desc->max_lines = atoi (buf);
    }

  else if (ch->desc && !str_cmp (subcmd, "columns"))
    {

      argument = one_argument (argument, buf);

      if (!isdigit (*buf) || atoi (buf) > 199)
	send_to_char ("Expected a number 1..199", ch);
      else
	ch->desc->max_columns = atoi (buf);
    }

  else if (ch->pc && ch->pc->level && !str_cmp (subcmd, "immenter"))
    {

      while (*argument && *argument == ' ')
	argument++;

      if (ch->pc->imm_enter)
	mem_free (ch->pc->imm_enter);

      if (!*argument)
	{
	  send_to_char ("You will use the standard immenter message.\n", ch);
	  ch->pc->imm_enter = str_dup ("");
	  return;
	}

      if (*argument == '"' || *argument == '\'')
	send_to_char ("Note:  You probably didn't mean to use quotes.\n", ch);

      ch->pc->imm_enter = str_dup (argument);
    }

  else if (ch->pc && ch->pc->level && !str_cmp (subcmd, "immleave"))
    {

      while (*argument && *argument == ' ')
	argument++;

      if (ch->pc->imm_leave)
	mem_free (ch->pc->imm_leave);

      if (!*argument)
	{
	  send_to_char ("You will use the standard immleave message.\n", ch);
	  ch->pc->imm_leave = str_dup ("");
	  return;
	}

      if (*argument == '"' || *argument == '\'')
	send_to_char ("Note:  You probably didn't mean to use quotes.\n", ch);

      ch->pc->imm_leave = str_dup (argument);
    }

  else if (ch->pc &&
	   IS_RIDER (ch) && (ind = index_lookup (mount_speeds, subcmd)) != -1)
    {

      if (ind == SPEED_SWIM)
	{
	  send_to_char ("Your mount will swim when in water.\n", ch);
	  return;
	}

      if (ind == SPEED_IMMORTAL && !get_trust (ch))
	{
	  send_to_char ("Your mount would disintegrate if you tried that.",
			ch);
	  return;
	}

      ch->pc->mount_speed = ind;
    }

  else if ((ind = index_lookup (speeds, subcmd)) != -1 && *subcmd)
    {

      if (ind == SPEED_SWIM)
	{
	  send_to_char ("You'll swim when you're in water.\n", ch);
	  return;
	}

      if (ind == SPEED_IMMORTAL && !get_trust (ch))
	{
	  send_to_char
	    ("Your muscles would rip away from your bones if you tried that.\n",
	     ch);
	  return;
	}

      if(crippled && !get_trust(ch))
      {
        send_to_char("You're not moving any faster than a trudge until your leg is healed.\n", ch);
        ch->speed = 1;
        return;
      }

      ch->speed = ind;
      sprintf (buf, "From now on you will %s.\n", speeds[ind]);
      send_to_char (buf, ch);
      clear_travel (ch);

    }

  else
    {

#define s(a) send_to_char (a "\n", ch);

      s ("\n   #6Movement:#0");
      s ("   Walk speeds  - trudge, pace, walk, jog, run, sprint");
      s ("   Mount speeds - walk, trot, canter, gallop");
      s ("");
      s ("   #6Character Information:#0");
      s ("   Keywords   - Add extra keywords to your character");      
      s ("");
      s ("   #6Combat Modes:#0");
      s ("   Frantic    - Offense, but no defense (no combat learning)");
      s ("   Aggressive - More offense than defense");
      s ("   Normal     - Balanced offense/defense");
      s ("   Careful    - More defense then offense");
      s ("   Defensive  - Mostly defense, some offense");
      s ("");
      s ("   #6Combat Flags:#0");
      s ("   Combat     - Toggle filtering of non-local combat messages");
      s ("   Autoflee   - Run away immediately if someone attacks");
      s ("   Pacifist   - Forego all attacks, for a defensive bonus");
      s ("   Autofire   - Shoot automatically at fastmoving targets");
      s ("   Highlight  - Personal melee attacks are highlighted");
      s ("");
      s ("   #6Informative:#0");
      s ("   Hints      - Toggle receipt of game-related hints");
      s ("   Mentor     - Please see #6HELP MENTOR#0 for details");
      s ("   Mute       - Toggle system beeps upon being notified");
      s ("   Newbie     - Turn off your #2(new player)#0 ldesc tag");
      s ("   Prompt     - Toggle informative prompt on and off");
      s ("   Rp         - Toggle output of Reward total in SCORE");
//    s ("   Voting     - Toggle receipt of voting reminders at login");
      if (!IS_MORTAL (ch))
	{
	  s ("\n   #6Staff-Only Commands:#0");
	  s ("   Available - Toggle your availability for petitions");
	  s ("   Immenter <message>  - Set \"goto\" enter message");
	  s ("   Immleave <message>  - Set \"goto\" leave message");
	  s ("   Immortal  - Return to immortal mode");
	  s ("   Immwalk   - Removes the walking delay when you move");
	  s ("   Mortal    - Become a player (for testing)");
	  s ("   Names     - Causes 'SAY' to show player names");
	  s ("   Telepath  - Allows you to 'listen in' on PC thoughts");
	  s ("   Wiznet    - Toggles your presence on the wiznet");
	}
    }

#undef s
}

void
do_passwd (CHAR_DATA * ch, char *argument, int cmd)
{
  char account_name[MAX_INPUT_LENGTH];
  char new_pass[MAX_INPUT_LENGTH];

  argument = one_argument (argument, account_name);

  if (!*account_name)
    {
      send_to_char ("Syntax:  passwd <account-name> <password>\n", ch);
      return;
    }

  argument = one_argument (argument, new_pass);

  if (!*new_pass)
    {
      send_to_char ("Syntax:  passwd <account-name> <password>\n", ch);
      return;
    }

  if (strlen (new_pass) > 10 || strlen (new_pass) < 4)
    {
      send_to_char ("Make the password between 4 and 10 characters.\n", ch);
      return;
    }

  account acct (account_name);
  if (!acct.is_registered ())
    {
      send_to_char ("No such login account.\n", ch);
      return;
    }
  
  char* epass = encrypt_buf(new_pass);
  acct.update_password (epass);
  mem_free (epass);
  send_to_char ("Password set.\n", ch);
}

void
do_whap (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *victim;
  OBJ_DATA *obj;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Whap who?\n", ch);
      return;
    }

  if (!(victim = get_char_room_vis (ch, buf)))
    {
      send_to_char ("They aren't here.\n", ch);
      return;
    }

  if (victim == ch)
    {
      send_to_char ("You probably don't want to whap yourself.\n", ch);
      return;
    }

  if (!(obj = get_equip (ch, WEAR_PRIM)) &&
      !(obj = get_equip (ch, WEAR_SEC)) && !(obj = get_equip (ch, WEAR_BOTH)))
    {
      send_to_char ("You must be wielding a weapon to whap someone.\n", ch);
      return;
    }

  if (!IS_NPC (victim) && IS_IMPLEMENTOR (victim))
    {
      act ("The moment you slip behind $m, $e spins around, and in a "
	   "flash $e grabs $p from your hands and whaps you with it!",
	   false, ch, obj, 0, TO_CHAR | _ACT_FORMAT);
      act ("You're out like a light!", false, ch, 0, 0,
	   TO_CHAR | _ACT_FORMAT);
      act ("HA! $N tries to whap you with $p.", false, victim, obj, ch,
	   TO_CHAR | _ACT_FORMAT);

      GET_POS (ch) = POSITION_STUNNED;

      act ("$N whaps $n on the head.  $n loses consciousness.",
	   true, victim, 0, ch, TO_NOTVICT | _ACT_FORMAT);

      ch->delay_type = DEL_WHAP;
      ch->delay = 10;

      return;
    }

  if (!IS_NPC (victim) && !str_cmp ("Nix", victim->tname))
    {
      act ("The moment you slip behind Nix, she spins around, and in a "
	   "flash she grabs\n$p from your hands and whaps you with it!",
	   false, ch, obj, 0, TO_CHAR);
      act ("You're out like a light!", false, ch, 0, 0,
	   TO_CHAR | _ACT_FORMAT);
      act ("HA!  $N tries to whap you with $p.", false, victim, obj, ch,
	   TO_CHAR | _ACT_FORMAT);

      GET_POS (ch) = POSITION_STUNNED;

      act ("$N whaps $n on the head.  $n looses consciousness.",
	   true, victim, 0, ch, TO_NOTVICT | _ACT_FORMAT);

      ch->delay_type = DEL_WHAP;
      ch->delay = 10;

      return;
    }

  if (GET_TRUST (victim) > GET_TRUST (ch))
    {
      act ("$N glares as you raise $p.  You think better of it.",
	   false, ch, obj, victim, TO_CHAR | _ACT_FORMAT);
      act ("You notice $N trying to whap you and glare at $m.",
	   false, victim, 0, ch, TO_CHAR | _ACT_FORMAT);
      return;
    }

  act ("You slip behind $N and whap $M with $p.",
       false, ch, obj, victim, TO_CHAR | _ACT_FORMAT);

  act ("In a flash, $N slips behind you and forcefully taps\nyou on the "
       "head with the blunt side of $p.",
       false, victim, obj, ch, TO_CHAR | _ACT_FORMAT);

  act ("Just before your eyes close, you see $N standing over\n"
       "you laughing.", false, victim, 0, ch, TO_CHAR | _ACT_FORMAT);

  GET_POS (victim) = POSITION_STUNNED;

  act ("$N whaps $n on the head; $e promptly loses consciousness.",
       true, victim, 0, ch, TO_NOTVICT | _ACT_FORMAT);

  victim->delay_type = DEL_WHAP;
  victim->delay = 25;
}

void
delayed_whap (CHAR_DATA * ch)
{
  if (GET_POS (ch) == POSITION_STUNNED)
    {
      GET_POS (ch) = POSITION_RESTING;
      send_to_char ("You regain consciousness.  Your head aches, "
		    "but otherwise you're fine.\n", ch);
      act ("$n regains consciousness.", true, ch, 0, 0, TO_ROOM);
    }

  ch->delay = 0;
}

void
do_maylay (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *victim;
  OBJ_DATA *obj;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Whap who?\n", ch);
      return;
    }

  if (!(victim = get_char_room_vis (ch, buf)))
    {
      send_to_char ("They aren't here.\n", ch);
      return;
    }

  if (victim == ch)
    {
      send_to_char ("You probably don't want to whap yourself.\n", ch);
      return;
    }

  if (!(obj = get_equip (ch, WEAR_PRIM)) &&
      !(obj = get_equip (ch, WEAR_SEC)) && !(obj = get_equip (ch, WEAR_BOTH)))
    {
      send_to_char ("You must be wielding a weapon to whap someone.\n", ch);
      return;
    }

  if (!IS_NPC (victim) && !str_cmp ("Nix", victim->tname))
    {
      act ("The moment you slip behind Nix, she spins around, and in a "
	   "flash she grabs\n$p from your hands and whaps you with it!",
	   false, ch, obj, 0, TO_CHAR);
      act ("You're out like a light!", false, ch, 0, 0,
	   TO_CHAR | _ACT_FORMAT);
      act ("HA!  $N tries to whap you with $p.", false, victim, obj, ch,
	   TO_CHAR | _ACT_FORMAT);

      GET_POS (ch) = POSITION_STUNNED;

      act ("$N whaps $n on the head.  $n looses consciousness.",
	   true, victim, 0, ch, TO_NOTVICT | _ACT_FORMAT);

      ch->delay_type = DEL_WHAP;
      ch->delay = 10;

      return;
    }

  if (GET_TRUST (victim) > GET_TRUST (ch))
    {
      act ("$N glares as you raise $p.  You think better of it.",
	   false, ch, obj, victim, TO_CHAR);
      act ("You notice $N trying to whap you and glare at $m.",
	   false, victim, 0, ch, TO_CHAR);
      return;
    }

  act ("You slip behind $N and whap $m with your $p.",
       false, ch, obj, victim, TO_CHAR);

  act ("In a flash, $N slips behind you and forcefully taps\nyou on the "
       "head with the blunt side of $p.", false, victim, obj, ch, TO_CHAR);

  act ("Just before your eyes close, you see $N standing over\n"
       "you laughing.", false, victim, 0, ch, TO_CHAR);

  GET_POS (victim) = POSITION_STUNNED;

  act ("$N whaps $n on the head.  $n looses consciousness.",
       true, victim, 0, ch, TO_NOTVICT);

  victim->delay_type = DEL_WHAP;
  victim->delay = 10;
}

void
delayed_maylay (CHAR_DATA * ch)
{
  if (GET_POS (ch) == POSITION_STUNNED)
    {
      GET_POS (ch) = POSITION_RESTING;
      send_to_char ("You regain consciousness.  Your head aches, "
		    "but otherwise you're fine.\n", ch);
      act ("$n regains consciousness.", true, ch, 0, 0, TO_ROOM);
    }

  ch->delay = 0;
}

void
do_wanted (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
  char buf1[MAX_STRING_LENGTH];
  CHAR_DATA *tch;
  AFFECTED_TYPE *af;
  int criminals = 0;
  int zone = -1;

  argument = one_argument (argument, buf);

  if (*buf && isdigit (*buf))
    zone = atoi (buf);

  *s_buf = '\0';

  if (IS_MORTAL (ch) && !is_area_enforcer (ch))
    {
      send_to_char ("You don't know who is wanted around here.\n", ch);
      return;
    }

  if (IS_MORTAL (ch))
    zone = ch->room->zone;

  for (tch = character_list; tch; tch = tch->next)
    {

      if (tch->deleted)
	continue;

      for (af = tch->hour_affects; af; af = af->next)
	{

	  if (af->type < MAGIC_CRIM_BASE || af->type > MAGIC_CRIM_BASE + 100)
	    continue;

	  /*
	     If ch is not an imm, and the criminal is in the
	     same zone, then if the criminal is not wanted in
	     the zone, skip him.
	   */
	  if (zone != -1)
	    {
	      if (!is_same_zone (tch->room->zone, zone))
		{
		  continue;
		}
	      else
		{

		  /* Ugliness to deal with tashal's 2 zones */

		  if (zone == 10 || zone == 76)
		    {
		      if (!(get_affect (tch, MAGIC_CRIM_BASE
					+ 10) ||
			    get_affect (tch, MAGIC_CRIM_BASE + 76)))
			continue;
		    }
		  else if (!get_affect (tch, MAGIC_CRIM_BASE + zone))
		    continue;
		}
	    }

	  criminals++;

	  if (IS_MORTAL (ch))
	    {
	      if (af->a.spell.duration == -1)
		sprintf (buf, "Forever %s\n", tch->short_descr);
	      else
		sprintf (buf, "  %d    %s\n", af->a.spell.duration,
			 tch->short_descr);
	    }

	  else
	    {

	      if (IS_NPC (tch))
		sprintf (buf1, "[%d] ", tch->mob->nVirtual);
	      else
		sprintf (buf1, "[%s] ", GET_NAME (tch));

	      sprintf (buf, "%5d  %2d      %2d  %s%s\n",
		       tch->in_room, af->a.spell.duration,
		       af->type - MAGIC_CRIM_BASE, buf1, tch->short_descr);
	    }

	  strcat (s_buf, buf);
	}
    }

  if (!criminals)
    send_to_char ("No criminals.\n", ch);
  else
    {
      if (!IS_MORTAL (ch))
	ch->desc->header = str_dup ("Room  Hours  Zone  Name\n");
      else if (criminals)
	ch->desc->header = str_dup ("Hours   Description\n");

      page_string (ch->desc, s_buf);
    }
}

void
report_exits (CHAR_DATA * ch, int zone)
{
  int dir;
  ROOM_DATA *room;
  ROOM_DATA *troom;

  *b_buf = '\0';

  for (room = full_room_list; room; room = room->lnext)
    {
    
    	if (zone != -1 && room->zone != zone)
      continue;

      for (dir = 0; dir <= LAST_DIR; dir++)
  {

    if (!room->dir_option[dir])
      continue;

    if (!(troom = vtor (room->dir_option[dir]->to_room)))
      {
        sprintf (b_buf + strlen (b_buf),
           "Room %5d %5s doesn't go to room %d\n",
           room->nVirtual, dirs[dir],
           room->dir_option[dir]->to_room);
      }

    else if (!troom->dir_option[rev_dir[dir]])
      {
        sprintf (b_buf + strlen (b_buf),
           "Room %5d %5s is one-way to room %d\n",
           room->nVirtual, dirs[dir],
           room->dir_option[dir]->to_room);
      }

    else if (room->nVirtual != troom->dir_option[rev_dir[dir]]->to_room)
      {
        sprintf (b_buf + strlen (b_buf),
           "Room %5d %5s->%5d  BUT  %5d <- %5s %5d\n",
           room->nVirtual, dirs[dir], troom->nVirtual,
           troom->dir_option[rev_dir[dir]]->to_room,
           dirs[rev_dir[dir]], troom->nVirtual);
      }
  }
    }

  page_string (ch->desc, b_buf);
}

void
report_mobiles (CHAR_DATA * ch)
{
  int i;
  int zone_counts[100];
  int total_mobs = 0;
  int recount = 0;
  CHAR_DATA *tch;
  char buf[MAX_STRING_LENGTH];
  extern int hash_dup_strings, hash_dup_length;

  for (i = 0; i < 100; i++)
    zone_counts[i] = 0;

  for (tch = character_list; tch; tch = tch->next)
    {

      if (tch->deleted || !IS_NPC (tch))
	continue;

      zone_counts[tch->mob->reset_zone]++;
      total_mobs++;
    }

  for (i = 0; i < 25; i++)
    {
      sprintf (buf,
	       "[%2d]: %3d     [%2d]: %3d      [%2d]: %3d      [%2d]: %3d\n",
	       i, zone_counts[i], i + 25, zone_counts[i + 25], i + 50,
	       zone_counts[i + 50], i + 75, zone_counts[i + 75]);
      send_to_char (buf, ch);

      recount += zone_counts[i] + zone_counts[i + 25] +
	zone_counts[i + 50] + zone_counts[i + 75];
    }

  sprintf (buf, "  Total: %d; recount: %d\n", total_mobs, recount);
  send_to_char (buf, ch);
  sprintf (buf, "  Hash dups: %d; Dup'ed len: %d\n",
	   hash_dup_strings, hash_dup_length);
  send_to_char (buf, ch);
}

void
report_objects (CHAR_DATA * ch)
{
  int i = 0;
  int j;
  FILE *fp;
  OBJ_DATA *obj;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

	if (GET_TRUST (ch) < 6)
		send_to_char("Please ask an Implementor for this information\n", ch);
		
  if (!(fp = fopen ("objects", "w+")))
    perror ("Couldn't open objects");

  fprintf (fp,
	   "Virtual\tQuality\tSize\tCondition\tWeight\tCost\tItemType\tOV1\tOV2\tOV3\tOV4\tOV5\tFlags\tKeywords\tShort\tOneLine\n");

  for (obj = full_object_list; obj; obj = obj->lnext)
    {

      if (obj->nVirtual == VNUM_FARTHING || obj->nVirtual == VNUM_PENNY)
	continue;

      *buf2 = '\0';

      for (j = 0; str_cmp (econ_flags[j], "\n"); j++)
	{
	  if (IS_SET (obj->econ_flags, 1 << j))
	    {
	      strcat (buf2, econ_flags[j]);
	      strcat (buf2, " ");
	    }
	}

      /*          virt qual sz   cnd   wt  cost  it  v0  v1  v2  v3  v4  v5  fl  ky  sd  ld */

      fprintf (fp,
	       "%05d\t%2d\t%5d\t%2d\t%5d\t%5.2f\t%s\t%d\t%d\t%d\t%d\t%d\t%d\t%s\t%s\t%s\t%s\n",
	       obj->nVirtual, obj->quality, obj->size, obj->item_wear,
	       obj->obj_flags.weight, obj->farthings,
	       item_types[(int) obj->obj_flags.type_flag], obj->o.od.value[0],
	       obj->o.od.value[1], obj->o.od.value[2], obj->o.od.value[3],
	       obj->o.od.value[4], obj->o.od.value[5], buf2, obj->name,
	       obj->short_description, obj->description);

      i++;
    }

  sprintf (buf, "Write %d records.\n", i);

  send_to_char (buf, ch);

  fclose (fp);
}

void
revise_objects (CHAR_DATA * ch)
{
  int ind;
  int vnum;
  FILE *fp;
  OBJ_DATA *obj;
  char *p;
  char *argument;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  if (!(fp = fopen ("objects.revised", "r")))
    perror ("Couldn't open objects");

  while (fgets (buf, 256, fp))
    {

      p = buf;
      while (*p)
	{
	  if (*p == ',')
	    *p = ' ';
	  p++;
	}

      argument = one_argument (buf, buf2);

      vnum = atoi (buf2);

      if (!vnum || !(obj = vtoo (vnum)))
	{
	  printf ("Vnum %d doesn't work.\n", vnum);
	  continue;
	}

      argument = one_argument (argument, buf2);
      argument = buf2;

      obj->econ_flags = 0;

      while (1)
	{
	  argument = one_argument (argument, buf);

	  if (!*buf)
	    break;

	  if ((ind = index_lookup (econ_flags, buf)) == -1)
	    {
	      printf ("Unknown flag name: %s\n", buf);
	      continue;
	    }

	  obj->econ_flags |= (1 << ind);
	}
    }

  fclose (fp);
  send_to_char ("Done.\n", ch);
}

void
report_races (CHAR_DATA * ch)
{
  int i = 0;
  FILE *fp;
  CHAR_DATA *mob;
  char buf[MAX_STRING_LENGTH];

	if (GET_TRUST (ch) < 6)
		send_to_char("Please ask an Implementor for this information\n", ch);
		
  if (!(fp = fopen ("races", "w+")))
    perror ("Couldn't create races");

  for (mob = full_mobile_list; mob; mob = mob->mob->lnext)
    {
      fprintf (fp, "%05d  %-7.7s %-60.60s\n", mob->mob->nVirtual,
	       lookup_race_variable (mob->race, RACE_NAME), GET_NAMES (mob));
      i++;
    }

  sprintf (buf, "Wrote %d records.\n", i);
  send_to_char (buf, ch);

  fclose (fp);
}

void
report_shops (CHAR_DATA * ch)
{
  int write_count = 0;
  int i;
  FILE *fp;
  CHAR_DATA *mob;
  OBJ_DATA *tobj;
  char buf[MAX_STRING_LENGTH];

/*
I have a request for you...  merchants listing, tab delimited, with:
vnum, short desc, store room, markup, discount, trades in, deliveries 
0-9, and if possible, a listing of what is in their storerooms.

I'd like to give you back vnum, all the markups, discounts, and nobuy
*/

	if (GET_TRUST (ch) < 6)
		send_to_char("Please ask an Implementor for this information\n", ch);

  if (!(fp = fopen ("shops", "w+")))
    perror ("Couldn't create shops");

  fprintf (fp,
	   "Vnum \tShop \tStore\tMrkup\tDscnt\tDel-0\tName0\tDel-1\tName1\tDel-2\tName2\tDel-3\tName3\tDel-4\tName4\tDel-5\tName5\tDel-6\tName6\tDel-7\tName7\tDel-8\tName8\tDel-9\tName9\tKeeper Name\ttrade-flags\n");

  for (mob = full_mobile_list; mob; mob = mob->mob->lnext)
    {

      if (!mob->shop)
	continue;

      sprintf (buf, "%05d\t%5d\t%5d\t%3.2f\t%3.2f\t",
	       mob->mob->nVirtual, mob->shop->shop_vnum,
	       mob->shop->store_vnum, mob->shop->markup, mob->shop->discount);

      for (i = 0; i < MAX_DELIVERIES; i++)
	{

	  sprintf (buf + strlen (buf), "%5d\t", mob->shop->delivery[i]);

	  if (mob->shop->delivery[i] > 0 &&
	      (tobj = vtoo (mob->shop->delivery[i])))
	    sprintf (buf + strlen (buf), "%s\t", tobj->short_description);
	  else
	    sprintf (buf + strlen (buf), "\t");
	}

      sprintf (buf + strlen (buf), "%s\t", mob->short_descr);

      for (i = 0; i < MAX_TRADES_IN; i++)
	if (mob->shop->trades_in[i])
	  sprintf (buf + strlen (buf), "%s ",
		   item_types[mob->shop->trades_in[i]]);

      fprintf (fp, "\t%s\n", buf);

      write_count++;
    }

  sprintf (buf, "Wrote %d records.\n", write_count);
  send_to_char (buf, ch);

  fclose (fp);
}

void
get_float (float *f, char **argument)
{
  char buf[MAX_STRING_LENGTH];

  *argument = one_argument (*argument, buf);

  sscanf (buf, "%f", f);
}

void
get_flags (int *flags, char **argument)
{
  int ind;
  char *p;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  *argument = one_argument (*argument, buf);

  *flags = 0;

  p = buf;

  while (1)
    {
      p = one_argument (p, buf2);

      if (!*buf2)
	break;

      if ((ind = index_lookup (econ_flags, buf2)) == -1)
	{
	  printf ("Unknown flag name: %s\n", buf2);
	  continue;
	}

      *flags |= (1 << ind);
    }
}

void
revise_shops (CHAR_DATA * ch)
{
  int vnum;
  FILE *fp;
  CHAR_DATA *mob;
  char *p;
  char *argument;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  if (!(fp = fopen ("shops.revised", "r")))
    perror ("Couldn't open shops.revised");

  while (fgets (buf, 256, fp))
    {

      p = buf;
      while (*p)
	{
	  if (*p == ',')
	    *p = ' ';
	  p++;
	}

/*
"Vnum ","Shop ","Store","Mrkup","Dscnt","Mrkup1","Dscnt1","Flags1","Mrkup2","Dscnt2","Flags2","Mrkup3","Dscnt3","Flags3","Nobuy"
1173,14173,2951,1.15,0.15,2.30,0.30,"fine khuzan rare valuable magical sindarin noble foreign",1.44,0.19,"ntharda stharda orbaal",0.58,0.07,"poor","junk gargun"
*/
      argument = one_argument (buf, buf2);	/* Keeper vnum */
      vnum = atoi (buf2);

      if (!vnum || !(mob = vtom (vnum)))
	{
	  printf ("Vnum %d doesn't work.\n", vnum);
	  continue;
	}

      argument = one_argument (argument, buf2);	/* Shop */
      argument = one_argument (argument, buf2);	/* Store */

      get_float (&mob->shop->markup, &argument);
      get_float (&mob->shop->discount, &argument);
      get_float (&mob->shop->econ_markup1, &argument);
      get_float (&mob->shop->econ_discount1, &argument);
      get_flags (&mob->shop->econ_flags1, &argument);
      get_float (&mob->shop->econ_markup2, &argument);
      get_float (&mob->shop->econ_discount2, &argument);
      get_flags (&mob->shop->econ_flags2, &argument);
      get_float (&mob->shop->econ_markup3, &argument);
      get_float (&mob->shop->econ_discount3, &argument);
      get_flags (&mob->shop->econ_flags3, &argument);
      get_float (&mob->shop->econ_markup4, &argument);
      get_float (&mob->shop->econ_discount4, &argument);
      get_flags (&mob->shop->econ_flags4, &argument);
      get_float (&mob->shop->econ_markup5, &argument);
      get_float (&mob->shop->econ_discount5, &argument);
      get_flags (&mob->shop->econ_flags5, &argument);
      get_float (&mob->shop->econ_markup6, &argument);
      get_float (&mob->shop->econ_discount6, &argument);
      get_flags (&mob->shop->econ_flags6, &argument);
      get_float (&mob->shop->econ_markup7, &argument);
      get_float (&mob->shop->econ_discount7, &argument);
      get_flags (&mob->shop->econ_flags7, &argument);
      get_flags (&mob->shop->nobuy_flags, &argument);
    }

  fclose (fp);
  send_to_char ("Done.\n", ch);
}

#define ZCMD zone_table[zone].cmd[cmd_no]

void
unused_objects (CHAR_DATA * ch)
{
/*
    int             cmd_no;
    OBJ_DATA        *obj = NULL;
    ROOM_DATA       *room;

	for ( obj = full_object_list; obj; obj = obj->lnext ) {
		for ( cmd_no = 0; ; cmd_no++ ) {

			if ( (ZCMD.command == 'O' ||
				  ZCMD.command == 'P' ||
				  ZCMD.command == 'G' ||
				  ZCMD.command == 'E') &&
				 ZCMD.arg1 == obj->nVirtual )
				

		for ( 
	}
*/
}

void
do_report (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];
	char temp[MAX_STRING_LENGTH];
	int zone = -1;

  argument = one_argument (argument, buf);

  if (!*buf || *buf == '?')
    {
      send_to_char ("   objects   - object list to file 'objects'.\n", ch);
      send_to_char ("   shops     - keeper list to file 'shops'.\n", ch);
      send_to_char ("   race      - race report.\n", ch);
      send_to_char ("   mobiles   - Mob population by zone.\n", ch);
      send_to_char ("   exits [zone]    - Faulty links in exits\n", ch);
    }
    
	else if (!str_cmp (buf, "exits"))
		{
			argument = one_argument (argument, temp);
			
			if (isdigit (*temp))
				{
					if ((zone = atoi (temp)) >= MAX_ZONE)
						{
							send_to_char ("Zone not in range 0..99\n", ch);
							return;
						}
					report_exits(ch, zone);
				}
			else
				report_exits (ch, -1);
    }

  else if (!str_cmp (buf, "uobjs"))
    unused_objects (ch);
  else if (!str_cmp (buf, "crash"))
    ((int *) 0)[-1] = 0;
  else if (!str_cmp (buf, "revshops"))
    revise_shops (ch);
  else if (!str_cmp (buf, "revobjs"))
    revise_objects (ch);
  else if (!str_cmp (buf, "shops") || !str_cmp (buf, "keepers"))
    report_shops (ch);
  else if (!str_cmp (buf, "objs") || !str_cmp (buf, "objects"))
    report_objects (ch);
  else if (!str_cmp (buf, "mobiles") || !str_cmp (buf, "mobs"))
    report_mobiles (ch);
  else if (!str_cmp (buf, "race") || !str_cmp (buf, "races"))
    report_races (ch);
  else
    send_to_char ("report ?   - for help\n", ch);
}

/*
SELECT name,
  CONCAT( SUBSTRING(skills,LOCATE('Aura',skills),20),', ',
  SUBSTRING(skills,LOCATE('Bolt',skills),20),', ',
  SUBSTRING(skills,LOCATE('Clai',skills),20),', ',
  SUBSTRING(skills,LOCATE('Dang',skills),20),', ',
  SUBSTRING(skills,LOCATE('Empa',skills),20),', ',
  SUBSTRING(skills,LOCATE('Hex',skills),20),', ',
  SUBSTRING(skills,LOCATE('Tele',skills),20),', ',
  SUBSTRING(skills,LOCATE('Pres',skills),20)) Psi
FROM player_db.pfiles 
WHERE create_state = 2 AND level < 1
AND ( 
  skills LIKE '%Aura%' 
  OR skills like '%Bolt%' 
  OR skills like '%Clai%' 
  OR skills like '%Dang%' 
  OR skills like '%Empa%' 
  OR skills like '%Hex%' 
  OR skills like '%Tele%' 
  OR skills like '%Pres%' 
);


*/
void
do_openskill (CHAR_DATA * ch, char *argument, int cmd)
{
  int sn;
  CHAR_DATA *victim;
  char buf1[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf1);
  argument = one_argument (argument, buf2);

  if ((sn = index_lookup (skills, buf1)) == -1)
    {
      if ((sn = index_lookup (skills, buf2)) == -1)
	{
	  send_to_char ("No such skill.  Use the 'show v skills' command.\n",
			ch);
	  return;
	}

      victim = get_char_room_vis (ch, buf1);
    }
  else
    victim = get_char_room_vis (ch, buf2);

  if (!victim)
    {
      send_to_char
	("Either use 'at <person> openskill' or go to his/her room.\n", ch);
      return;
    }

  if (victim->skills[sn])
    {
      sprintf (buf1, "Skill %s is already at %d on that character.\n",
	       skills[sn], victim->skills[sn]);
      return;
    }
  if (sn == SKILL_CLAIRVOYANCE || sn == SKILL_DANGER_SENSE
      || sn == SKILL_EMPATHIC_HEAL || sn == SKILL_HEX
      || sn == SKILL_MENTAL_BOLT || sn == SKILL_PRESCIENCE
      || sn == SKILL_SENSITIVITY || sn == SKILL_TELEPATHY)
    {
      if (!IS_IMPLEMENTOR (ch))
	{
	  send_to_char
	    ("Psionics may only be rolled randomly at character creation.\n",
	     ch);
	  return;
	}
    }

  open_skill (victim, sn);

  sprintf (buf1, "$N's %s has opened at %d.", skills[sn], victim->skills[sn]);

  act (buf1, false, ch, 0, victim, TO_CHAR);
}

void
do_swap (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (str_cmp (buf, "worldfile") && str_cmp (buf, "binary"))
    {
      send_to_char ("You must type 'swap worldfile' or 'swap binary'.\n", ch);
      return;
    }

  if (pending_reboot)
    {
      send_to_char ("You should cancel the pending reboot before swapping.\n",
		    ch);
      return;
    }

  if (!str_cmp (buf, "worldfile") && !*argument)
    {
      if (engine.in_build_mode ())
	{
	  send_to_char ("On the builder port?\n", ch);
	  return;
	}
      if (GET_TRUST (ch) < 4)
	{
	  send_to_char ("You must specify the zone number to swap.\n", ch);
	  return;
	}
      send_to_char ("Transferring ALL builders' changes...\n", ch);

      std::string build_source (engine.get_base_path ("build"));
      std::string swap_destination (engine.get_base_path ());
      std::string swap_command ("/bin/cp "
				+ build_source + "/regions/* "
				+ swap_destination + "/regions/");

      system (swap_command.c_str ());
      sprintf (buf, "%s has just transferred ALL building changes.\n",
	       ch->tname);
      send_to_gods (buf);
    }
  else if (!str_cmp (buf, "worldfile") && isdigit (*argument))
    {
      int z = strtol (argument,0,0);
      if (z == 99)
	{
	  send_to_char ("That's the temproom zone; no swapping allowed.\n",
			ch);
	  return;
	}
	if (z > 99)
		{
  			send_to_char ("Please use a zone number between 1 and 99\n",
      ch);
		return;
		}
      if (str_cmp (ch->tname, zone_table[z].lead)
	  && GET_TRUST (ch) < 4)
	{
	  send_to_char ("You are not the lead for this building project.\n",
			ch);
	  return;
	}
      if (z >= 0 && z <= 98)
	{
	  sprintf (buf, "Transferring builders' changes to %s...\n",
		   zone_table[z].name);
	  send_to_char (buf, ch);
	  sprintf (buf,"*.%d ", z);

	  std::string build_source (engine.get_base_path ("build"));
	  std::string swap_destination (engine.get_base_path ());
	  std::string swap_command ("/bin/cp "
				    + build_source + "/regions/" + std::string(buf)
				    + swap_destination + "/regions/");

	  system (swap_command.c_str ());
	  sprintf (buf, "%s has just swapped over the changes to %s.\n",
		   ch->tname, zone_table[z].name);
	  send_to_gods (buf);
	}
      else
	{
	  send_to_char ("Please specify a number between 0 and 99.\n", ch);
	  return;
	}
    }
  else if (!str_cmp (buf, "binary"))
    {
      if (!IS_IMPLEMENTOR (ch))
	{
	  send_to_char ("Please ask an implementor to do it.\n", ch);
	  return;
	}
      if (engine.in_test_mode ())
	{
	  send_to_char ("You are running the test server.\n", ch);
	  return;
	}

      std::string swap_destination (engine.get_base_path ());
      std::string test_source (engine.get_base_path ("test"));
      std::string swap_command ("/bin/cp "
				+ test_source + "/bin/server "
				+ swap_destination + "/bin/tmp_server"
				+ " && "
				+ "/bin/mv " 
				+ swap_destination + "/bin/tmp_server "
				+ swap_destination + "/bin/server");

      system (swap_command.c_str ());
      send_to_char ("The server binary on this port has been updated.\n", ch);
    }
}

void
do_affect (CHAR_DATA * ch, char *argument, int cmd)
{
  int power_specified = 0;
  int duration;
  int power = 0;
  int affect_no;
  AFFECTED_TYPE *af = NULL;
  CHAR_DATA *tch = NULL;
  OBJ_DATA *obj = NULL;
  ROOM_DATA *room;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!*buf || *buf == '?')
    {

      send_to_char ("           char <mob> <affect no/name>   delete\n"
		    "   affect  obj  <obj> <affect no/name>   <duration> [power]\n"
		    "           room       <affect no/name>\n"
		    "\n"
		    "Example:\n\n"
		    "   affect room 'aklash odor' delete\n"
		    "   affect char 2311 5000 1000   (2311 = Incense Smoke)\n\n"
		    "If an affect doesn't already exist, one is created by this "
		    "command.\n\n"
		    "   affect char 2311 4000        (Only modify duration)\n\n"
		    "Most affect durations are in mud hours, some are in RL seconds.\n",
		    ch);

      return;
    }

  room = ch->room;

  if (is_abbrev (buf, "character"))
    {

      argument = one_argument (argument, buf);

      if (!(tch = get_char_room_vis (ch, buf)))
	{
	  send_to_char ("Couldn't find that character in the room.\n", ch);
	  return;
	}
    }

  else if (is_abbrev (buf, "object"))
    {

      argument = one_argument (argument, buf);

      if (!(obj = get_obj_in_dark (ch, buf, ch->room->contents)))
	{
	  send_to_char ("Couldn't find that object in the room.\n", ch);
	  return;
	}
    }

  else if (is_abbrev (buf, "room"))
    {
    }

  else
    {
      send_to_char ("Expected 'character', 'object', or 'room' after affect.",
		    ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (just_a_number (buf))
    affect_no = atoi (buf);
  else
    {
      if ((affect_no = lookup_value (buf, REG_MAGIC_SPELLS)) == -1 &&
	  (affect_no = lookup_value (buf, REG_CRAFT_MAGIC)) == -1 &&
	  (affect_no = lookup_value (buf, REG_SPELLS)) == -1)
	{
	  send_to_char ("No such affect or spell.\n", ch);
	  return;
	}
    }

  argument = one_argument (argument, buf);

  if (is_abbrev (buf, "delete"))
    {

      if (obj)
	{
	  af = get_obj_affect (obj, affect_no);
	  if (!af)
	    {
	      send_to_char ("No such affect on that object.\n", ch);
	      return;
	    }

	  remove_obj_affect (obj, affect_no);
	  send_to_char ("Affected deleted from object.\n", ch);
	  return;
	}

      if (tch)
	{
	  af = get_affect (ch, affect_no);
	  if (!af)
	    {
	      send_to_char ("No such affect on that character.\n", ch);
	      return;
	    }

	  affect_remove (ch, af);
	  send_to_char ("Affect deleted from character.\n", ch);
	  return;
	}

      /* It must be a room affect */

      if (!(af = is_room_affected (room->affects, affect_no)))
	{
	  send_to_char ("No such affect on this room.\n", ch);
	  return;
	}

      remove_room_affect (room, affect_no);
      send_to_char ("Affect deleted from room.\n", ch);
      return;
    }

  /* We're not deleting, get the duration and optional power so we
     can either update an affect or create a new one.
   */

  if (!just_a_number (buf))
    {
      send_to_char ("Duration expected.\n", ch);
      return;
    }

  duration = atoi (buf);

  argument = one_argument (argument, buf);

  if (*buf)
    {
      if (!just_a_number (buf))
	{
	  send_to_char ("Power should be a number.\n", ch);
	  return;
	}

      power_specified = 1;
      power = atoi (buf);
    }

  /* Affect already exist? */

  if (ch)
    af = get_affect (tch, affect_no);
  else if (obj)
    af = get_obj_affect (obj, affect_no);
  else
    af = is_room_affected (room->affects, affect_no);

  if (af)
    {

      af->a.spell.duration = duration;

      if (power_specified)
	af->a.spell.modifier = power;

      send_to_char ("Affect modified.\n", ch);
      return;
    }

  /* Add a new affect */

  af = (AFFECTED_TYPE *) alloc (sizeof (AFFECTED_TYPE), 13);

  af->type = affect_no;

  if (affect_no >= MAGIC_FIRST_SOMA && affect_no <= MAGIC_LAST_SOMA)
    {
      af->a.spell.duration = 4 * duration;
      af->a.soma.latency = 0;
      af->a.soma.minute = 0;
      af->a.soma.max_power = 1000;
      af->a.soma.lvl_power = 700;
      af->a.soma.atm_power = 0;
      af->a.soma.attack = duration * 15;
      af->a.soma.decay = duration * 30;
      af->a.soma.sustain = duration * 45;
      af->a.soma.release = duration * 60;
      if (tch)
	{
	  af->next = tch->hour_affects;
	  tch->hour_affects = af;
	}
      else
	{
	  send_to_char
	    ("Somatic effects may only be applied to characters.\n", ch);
	}
    }
  else
    {
      af->a.spell.duration = duration;

      if (power_specified)
	{
	  af->a.spell.modifier = power;
	}
      if (tch)
	affect_to_char (tch, af);
      else if (obj)
	affect_to_obj (obj, af);
      else
	{
	  af->next = room->affects;
	  room->affects = af;
	}
    }
  send_to_char ("Affect created.\n", ch);
}

void
list_all_crafts (CHAR_DATA * ch)
{
  SUBCRAFT_HEAD_DATA *craft;
  char *p;

  *b_buf = '\0';
  p = b_buf;

  sprintf (p, "We currently have the following crafts available:\n\n");
  p += strlen (p);

  for (craft = crafts; craft; craft = craft->next)
    {

      sprintf (p, "#6Craft:#0 %-20s #6Sub:#0 %-24s #6Cmd:#0 %-10s\n",
	       craft->craft_name, craft->subcraft_name, craft->command);
      p += strlen (p);
    }

  send_to_char ("\n", ch);
  page_string (ch->desc, b_buf);
}

void
display_craft (CHAR_DATA * ch, SUBCRAFT_HEAD_DATA * craft)
{
  PHASE_DATA *phase;
  DEFAULT_ITEM_DATA *items;
  DEFAULT_ITEM_DATA *fitems;
  int i, j, phasenum = 1;
  char flag[MAX_STRING_LENGTH];

  float low_consumed_expense = 0;
  float high_consumed_expense = 0;
  float low_reusable_expense = 0;
  float high_reusable_expense = 0;
  float low_produced_value = 0;
  float high_produced_value = 0;

  for (phase = craft->phases; phase; phase = phase->next, phasenum++)
    {

      sprintf (b_buf + strlen (b_buf), "  #6Phase %d:#0\n", phasenum);

      if (phase->failure)
			sprintf (b_buf + strlen (b_buf), "1stFail:  %s\n",
			phase->failure);

      if (phase->second_failure)
	sprintf (b_buf + strlen (b_buf), "2ndFail:  %s\n",
		 phase->second_failure);

      if (phase->third_failure)
	sprintf (b_buf + strlen (b_buf), "3rdFail:  %s\n",
		 phase->third_failure);

	  if (phase->fail_group_mess)
		 sprintf (b_buf + strlen (b_buf), "FailGroup:  %s\n",
	 	 phase->fail_group_mess);
		 	
      if (phase->first)
			sprintf (b_buf + strlen (b_buf), "    1st:  %s\n",
			phase->first);

      if (phase->second)
			sprintf (b_buf + strlen (b_buf), "    2nd:  %s\n",
			phase->second);

      if (phase->third)
			sprintf (b_buf + strlen (b_buf), "    3rd:  %s\n",
			phase->third);

	  if (phase->group_mess)
			sprintf (b_buf + strlen (b_buf), "  Group:  %s\n",
		 	phase->group_mess);

      if (phase->phase_seconds)
	sprintf (b_buf + strlen (b_buf), "      T:  %d\n",
		 phase->phase_seconds);

		if (phase->skill > 0)
	sprintf (b_buf + strlen (b_buf), "  Skill:  %s vs %dd%d\n",
		 skills[phase->skill], phase->dice, phase->sides);

		if ( phase->attribute > -1)
			{
			sprintf (b_buf + strlen(b_buf), "  Attri:  %s vs %dd%d\n",
		       attrs[phase->attribute], phase->dice, phase->sides);
			}
      
      if (phase->flags)
	{
	  sprintf (b_buf + strlen (b_buf), "      F: ");
	  if (IS_SET (phase->flags, 1 << 0))
	    sprintf (b_buf + strlen (b_buf), " cannot-leave-room");
	  if (IS_SET (phase->flags, 1 << 1))
	    sprintf (b_buf + strlen (b_buf), " open_on_self");
	  if (IS_SET (phase->flags, 1 << 2))
	    sprintf (b_buf + strlen (b_buf), " require_on_self");
	  if (IS_SET (phase->flags, 1 << 3))
	    sprintf (b_buf + strlen (b_buf), " require_greater");
	  sprintf (b_buf + strlen (b_buf), "\n");
	}

      if (phase->move_cost)
	sprintf (b_buf + strlen (b_buf), "   Cost:  moves %d\n",
		 phase->move_cost);

	if (phase->hit_cost)
	sprintf (b_buf + strlen (b_buf), "   Cost:  hits %d\n",
		 phase->hit_cost);

    if (craft->obj_items)
			{
	  	for (i = 1; craft->obj_items[i]; i++)
	    	{
	      items = craft->obj_items[i];
	      if (items->phase != phase)
		continue;
	      if (items->items && items->items[0])
		{
		  if (IS_SET (items->flags, SUBCRAFT_GIVE))
		    sprintf (flag, "give");
		  else if (IS_SET (items->flags, SUBCRAFT_HELD))
		    sprintf (flag, "held");
		  else if (IS_SET (items->flags, SUBCRAFT_WIELDED))
		    sprintf (flag, "wielded");
		  else if (IS_SET (items->flags, SUBCRAFT_USED))
		    sprintf (flag, "used");
		  else if (IS_SET (items->flags, SUBCRAFT_PRODUCED))
		    sprintf (flag, "produced");
		  else if (IS_SET (items->flags, SUBCRAFT_WORN))
		    sprintf (flag, "worn");
		  else
		    sprintf (flag, "in-room");
		  sprintf (b_buf + strlen (b_buf), "      %d:  (%s", i, flag);
		  if (items->item_counts > 1)
		    sprintf (b_buf + strlen (b_buf), " x%d",
			     items->item_counts);

		  float low_cost = 0;
		  float high_cost = 0;
		  for (j = 0; j <= MAX_DEFAULT_ITEMS; j++)
		    {
		      if (items->items[j]
			  && items->items[j] != items->item_counts)
			{
			  sprintf (b_buf + strlen (b_buf), " %d",
				   items->items[j]);
			  OBJ_DATA *obj = vtoo (items->items[j]);
			  if (obj)
			    {
			      float obj_value = obj->farthings + (4*obj->silver);
			      if (!low_cost || (low_cost > obj_value))
				{
				  low_cost = obj_value;
				}
			      if (high_cost < obj_value)
				{
				  high_cost = obj_value;
				}
			    }
			}
		    }
		  low_cost *= items->item_counts;
		  high_cost *= items->item_counts;

		  if (IS_SET (items->flags, SUBCRAFT_USED))
		    {
		      low_consumed_expense += low_cost;
		      high_consumed_expense += high_cost;
		    }
		  else if (IS_SET (items->flags, SUBCRAFT_PRODUCED)
			   || IS_SET (items->flags, SUBCRAFT_GIVE))
		    {
		      low_produced_value += low_cost;
		      high_produced_value += high_cost;
		    }
		  else
		    {
		      low_reusable_expense += low_cost;
		      high_reusable_expense += high_cost;
		    }

		  sprintf (b_buf + strlen (b_buf), ")\n");
		}
	    }
	}

	if (craft->fails)
	{
	  for (i = 1; craft->fails[i]; i++)
	    {
	      fitems = craft->fails[i];
	      if (fitems->phase != phase)
				continue;
	      if (fitems->items && fitems->items[0])
		{
		  sprintf (b_buf + strlen (b_buf), " Fail %d:", i );
		  if (fitems->item_counts > 1)
		    sprintf (b_buf + strlen (b_buf), " x%d", fitems->item_counts);
		for (j = 0; j <= MAX_DEFAULT_ITEMS; j++)
		    {
		      if (fitems->items[j]
			  && fitems->items[j] != fitems->item_counts)
			{
			  sprintf (b_buf + strlen (b_buf), "  (%d)",
				   fitems->items[j]);
			}
	    }
		  sprintf (b_buf + strlen (b_buf), "\n");
		  
		}
	  }
	}
      if (phase->load_mob && vtom (phase->load_mob))
	{
	  sprintf (b_buf + strlen (b_buf), "    Mob:  #5%s#0",
		   char_short (vtom (phase->load_mob)));
	  if (IS_SET (phase->nMobFlags, CRAFT_MOB_SETOWNER))
	    {
	      strcat (b_buf, " set-owner");
	    }
	  strcat (b_buf, "\n");
	}
    }
  sprintf (b_buf + strlen (b_buf), 
	   "\n"
	   "#6Reusable Material Costs:#0 % 7.2f - % 7.2f bits\n"
	   "#6Expended Material Costs:#0 % 7.2f - % 7.2f bits\n"
	   "#6Produced Material Value:#0 % 7.2f - % 7.2f bits\n",
	   low_reusable_expense, high_reusable_expense,
	   low_consumed_expense, high_consumed_expense,
	   low_produced_value, high_produced_value);
  
  page_string (ch->desc, b_buf);

}

void
alias_free (ALIAS_DATA * alias)
{
  ALIAS_DATA *tmp_alias;

  while (alias)
    {
      tmp_alias = alias->next_line;

      if (alias->command)
	mem_free (alias->command);

      if (alias->line)
	mem_free (alias->line);

      mem_free (alias);

      alias = tmp_alias;
    }
}

void
alias_delete (CHAR_DATA * ch, char *alias_cmd)
{
  ALIAS_DATA *alias;
  ALIAS_DATA *tmp_alias;

  if (!ch->pc->aliases)
    return;

  if (!str_cmp (alias_cmd, ch->pc->aliases->command))
    {
      alias = ch->pc->aliases;
      ch->pc->aliases = alias->next_alias;
      alias_free (alias);
      return;
    }

  for (alias = ch->pc->aliases; alias->next_alias; alias = alias->next_alias)
    {
      if (!str_cmp (alias->next_alias->command, alias_cmd))
	{
	  tmp_alias = alias->next_alias;
	  alias->next_alias = tmp_alias->next_alias;
	  alias_free (tmp_alias);
	  return;
	}
    }
}

void
alias_create (CHAR_DATA * ch, char *alias_cmd, char *value)
{
  ALIAS_DATA *alias;
  char line[MAX_STRING_LENGTH];

  if (!get_line (&value, line))
    return;

  alias = (ALIAS_DATA *) alloc (sizeof (ALIAS_DATA), 28);

  alias->command = str_dup (alias_cmd);
  alias->line = str_dup (line);

  alias->next_alias = ch->pc->aliases;
  ch->pc->aliases = alias;

  while (get_line (&value, line))
    {
      alias->next_line = (ALIAS_DATA *) alloc (sizeof (ALIAS_DATA), 28);
      alias = alias->next_line;

      alias->line = str_dup (line);
    }
}

void
post_alias (DESCRIPTOR_DATA * d)
{
  CHAR_DATA *ch;

  ch = d->character;

  alias_create (ch, (char *) ch->delay_info1, ch->delay_who);

  mem_free ((char *) ch->delay_info1);
  mem_free ((char *) ch->delay_who);
}

ALIAS_DATA *
is_alias (CHAR_DATA * ch, char *buf)
{
  ALIAS_DATA *alias;

  if (!ch->pc)
    return NULL;

  if (!*buf)
    return NULL;

  for (alias = ch->pc->aliases; alias; alias = alias->next_alias)
    if (!str_cmp (alias->command, buf))
      return alias;

  return NULL;
}

void
do_alias (CHAR_DATA * ch, char *argument, int cmd)
{
  int seen_alias = 0;
  ALIAS_DATA *alias;
  char buf[MAX_STRING_LENGTH];
  char alias_cmd[MAX_STRING_LENGTH];
  char value[MAX_STRING_LENGTH];

  if (!ch->pc)
    {
      send_to_char ("This is a PC only command.\n", ch);
      return;
    }

  if (GET_FLAG (ch, FLAG_ALIASING))
    {
      send_to_char ("Alias command can't be used by an alias, sorry.\n", ch);
      return;
    }

  argument = one_argument (argument, alias_cmd);

  if (*alias_cmd == '?')
    {
      send_to_char ("  alias               - list aliases\n", ch);
      send_to_char ("  alias <cmd>         - list value of alias\n", ch);
      send_to_char ("  alias <cmd> =       - set value of alias\n", ch);
      send_to_char ("  alias <cmd> \"value\" - set value of alias\n", ch);
      return;
    }

  if (!*alias_cmd)
    {

      for (alias = ch->pc->aliases; alias; alias = alias->next_alias)
	{
	  sprintf (buf, "   %s\n", alias->command);
	  send_to_char (buf, ch);
	  seen_alias = 1;
	}

      if (!seen_alias)
	send_to_char ("No aliases defined.\n", ch);

      return;
    }

  argument = one_argument (argument, value);

  if (*value == '=')
    {
      alias_delete (ch, alias_cmd);

      ch->delay_who = NULL;

      ch->delay_ch = ch;
      ch->delay_info1 = (long int) str_dup (alias_cmd);

      ch->desc->str = &ch->delay_who;
      ch->desc->max_str = STR_MULTI_LINE;
      ch->desc->proc = post_alias;

      return;
    }

  else if (*value)
    {
      alias_delete (ch, alias_cmd);
      alias_create (ch, alias_cmd, value);
      return;
    }

  for (alias = ch->pc->aliases; alias; alias = alias->next_alias)
    {

      if (str_cmp (alias_cmd, alias->command))
	continue;

      sprintf (buf, "Alias:  %s\n", alias->command);
      send_to_char (buf, ch);

      for (; alias; alias = alias->next_line)
	{
	  sprintf (buf, "  %s\n", alias->line);
	  send_to_char (buf, ch);
	  seen_alias = 1;
	}

      break;
    }

  if (!seen_alias)
    send_to_char ("No such alias.\n", ch);
}

int
is_name_legal (char *buf)
{
  unsigned int i;

  if (strlen (buf) > 15)
    return 0;

  for (i = 0; i < strlen (buf); i++)
    {
      if (!isalpha (buf[i]))
	{
	  return 0;
	}

      if (i)
	buf[i] = tolower (buf[i]);
      else
	buf[i] = toupper (buf[i]);
    }

  return 1;
}

void
do_pfile (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch;
  char pfile[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  char new_name[MAX_STRING_LENGTH];
  struct stat stat_buf;

  if (GET_TRUST (ch) != 5)
    {
      send_to_char ("No way.\n", ch);
      return;
    }

  argument = one_argument (argument, pfile);

  if (!*pfile || *pfile == '?')
    {
      send_to_char
	("pfile <pc-name>   activate [<new-name>] - return from cold storage\n"
	 "                  archive               - place in cold storage\n"
	 "                  copy <new-name>       - make a copy of a pfile, including\n"
	 "                                          objects.\n"
	 "                  delete <pfile>        - delete an active pfile\n"
	 "                  rename <new-name>     - rename a pfile\n\n", ch);
      return;
    }

  if (!is_name_legal (pfile))
    {
      send_to_char ("Illegal pfile name.\n", ch);
      return;
    }

  argument = one_argument (argument, buf);

  if (str_cmp (buf, "activate") &&
      str_cmp (buf, "archive") &&
      str_cmp (buf, "copy") &&
      str_cmp (buf, "delete") && str_cmp (buf, "rename"))
    {
      send_to_char ("Unrecognized keyword, type 'pfile ?' for help.\n", ch);
      return;
    }

  if (!str_cmp (buf, "activate"))
    {

      argument = one_argument (argument, new_name);

      if (!*new_name)
	strcpy (new_name, pfile);

      if (!is_name_legal (new_name))
	{
	  send_to_char ("Illegal new-name.\n", ch);
	  return;
	}

      sprintf (buf, "save/player/%c/%s", tolower (*new_name), new_name);

      /* Stat returns 0 if successful, -1 if it fails */

      if (stat (buf, &stat_buf) == 0)
	{
	  send_to_char ("Target pfile (new-name) already exists.\n", ch);
	  return;
	}

      sprintf (buf, "save/archive/%s", new_name);

      if (stat (buf, &stat_buf) == -1)
	{
	  send_to_char ("Archive pfile (pc-name) does not exist in the "
			"archive directory.\n", ch);
	  return;
	}

      /* Pfile */

      sprintf (buf, "mv save/archive/%s save/player/%c/%s",
	       new_name, tolower (*new_name), new_name);
      system (buf);

      /* Mobs saved with pfile */

      sprintf (buf, "mv save/archive/%s.a save/player/%c/%s.a",
	       new_name, tolower (*new_name), new_name);
      system (buf);

      /* Player's objects */

      sprintf (buf, "mv save/archive/%s.objs save/objs/%c/%s",
	       new_name, tolower (*new_name), new_name);
      system (buf);

      send_to_char ("Player character re-activated.\n", ch);
      return;
    }

  if (!str_cmp (buf, "delete"))
    {

      send_to_char ("Delete operation.\n", ch);
      send_to_char (pfile, ch);
      send_to_char (" being deleted.\n", ch);
      tch = load_pc (pfile);

      if (!tch)
	{
	  send_to_char ("That pfile doesn't exist or isn't active.\n", ch);
	  return;
	}

      sprintf (buf, "Load count is %d\n", tch->pc->load_count);
      send_to_char (buf, ch);

      if (tch->pc->load_count != 1)
	{
	  act ("The player can't be online and the pfile can't be loaded by "
	       "another admin when it is deleted.  You or someone else may "
	       "have the pfile 'mob'ed.", false,
	       ch, 0, 0, TO_CHAR | _ACT_FORMAT);
	  unload_pc (tch);
	  return;
	}

      unload_pc (tch);

      sprintf (buf, "rm save/player/%c/%s", tolower (*pfile), pfile);
      send_to_char (buf, ch);
      send_to_char ("\n", ch);

      strcat (buf, ".a");
      send_to_char (buf, ch);
      send_to_char ("\n", ch);

      sprintf (buf, "rm save/objs/%c/%s", tolower (*pfile), pfile);
      send_to_char (buf, ch);
      send_to_char ("\n", ch);

      strcat (buf, ".died");
      send_to_char (buf, ch);
      send_to_char ("\n", ch);

      sprintf (buf, "rm save/player/submit/%s", pfile);
      send_to_char (buf, ch);
      send_to_char ("\n", ch);

      send_to_char ("Ok.\n", ch);

      return;
    }

  if (!str_cmp (buf, "archive"))
    {

      for (tch = character_list; tch; tch = tch->next)
	{

	  if (tch->deleted || !tch->pc)
	    continue;

	  if (!str_cmp (new_name, tch->tname))
	    {
	      send_to_char ("That character is currently loaded.  Figure out "
			    "a way to unload it.\n", ch);
	      return;
	    }
	}
    }

  send_to_char ("End of procedure\n", ch);
}

void
do_last (CHAR_DATA * ch, char *argument, int cmd)
{
  time_t localtime;
  CHAR_DATA *tch;
  char buf[MAX_STRING_LENGTH];

  argument = one_argument (argument, buf);

  if (!(tch = load_pc (buf)))
    {
      send_to_char ("No such PC.\n", ch);
      return;
    }

  if (tch->pc->last_connect)
    {
      sprintf (buf, "Last connect:     %s", ctime (&tch->pc->last_connect));
      send_to_char (buf, ch);
    }

  if (tch->pc->last_logon)
    {
      sprintf (buf, "Last logon:       %s", ctime (&tch->pc->last_logon));
      send_to_char (buf, ch);
    }

  if (tch->pc->last_logoff)
    {
      sprintf (buf, "Last logoff:      %s", ctime (&tch->pc->last_logoff));
      send_to_char (buf, ch);
    }

  if (tch->pc->last_disconnect)
    {
      sprintf (buf, "Last disconnect:  %s",
	       ctime (&tch->pc->last_disconnect));
      send_to_char (buf, ch);
    }

  if (tch->pc->last_died)
    {
      sprintf (buf, "Last died:        %s", ctime (&tch->pc->last_died));
      send_to_char (buf, ch);
    }

  localtime = time (0);

  send_to_char ("\nCurrent Server Time: ", ch);

  send_to_char (ctime (&localtime), ch);

  unload_pc (tch);
}

void
do_log (CHAR_DATA * ch, char *argument, int cmd)
{
  int immortal = 0, guest = 0, system = 0, descend = 1, port_num =
    0, room_num = -1;
  int days = 0, hours = 0, minutes = 0, results = 0, prev_days =
    0, curr_days = 0;
  char *args = NULL, *date = NULL;
  char npc[MAX_STRING_LENGTH] = {'\0'};
  char name[MAX_STRING_LENGTH] = {'\0'};
  char account_name[MAX_STRING_LENGTH] = {'\0'};
  char command[MAX_STRING_LENGTH] = {'\0'};
  char buf[MAX_STRING_LENGTH] = {'\0'};
  char buf2[MAX_STRING_LENGTH] = {'\0'};
  char query[MAX_STRING_LENGTH] = {'\0'};
  char output[MAX_STRING_LENGTH] = {'\0'};
  char search_args[MAX_STRING_LENGTH] = {'\0'};
  MYSQL_ROW row;
  MYSQL_RES *result;
  bool prev_arg = false, alloc = false, error = false, truncated = false;
  time_t current_time;
  //send_to_char ("Disabled.\n", ch);

  /*if (engine.in_play_mode ())
    {
      send_to_char
	("To avoid game lag, please do this on the builder port or website.\n",
	 ch);
      return;
    }
  */

  std::string log_db = engine.get_config ("player_log_db");

  while (*argument == ' ')
    argument++;

  if (!*argument)
    {
      mysql_safe_query ("SELECT count(*) FROM %s.mud", log_db.c_str ());
      result = mysql_store_result (database);
      if (!result)
	{
	  send_to_char ("What did you wish to search the logs for?\n", ch);
	  return;
	}
      if ((row = mysql_fetch_row (result)))
	{
	  results = strtol (row[0], NULL, 0);
	}
      sprintf (buf,
	       "The server's log database currently contains #6%d#0 entries.\n",
	       results);
      send_to_char (buf, ch);
      mysql_free_result (result);
      return;
    }

  /*
  *query = '\0';
  *search_args = '\0';
  *name = '\0';
  *account_name = '\0';
  *npc = '\0';
  *command = '\0';
  */
  port_num = 1455;

  if (*argument == '(')
    {
      while (*++argument != ')')
	{
	  if (!*argument)
	    {
	      send_to_char
		("You didn't include a closed parenthetical to end your search parameters.\n",
		 ch);
	      return;
	    }
	  sprintf (search_args + strlen (search_args), "%c", *argument);
	}
      if (*argument == ')')
	argument++;
    }

  if (*search_args)
    {
      args = str_dup (search_args);
      alloc = true;
    }

  while (args && *args && (args = one_argument (args, buf)))
    {
      if (!strcasecmp (buf, "port"))
	{
	  args = one_argument (args, buf);
	  port_num = strtol (buf, 0, 10);
	  if (port_num <= 0 || port_num >= INT_MAX)
	    {
	      send_to_char ("Expected a port number to search on. "
			    "E.g. 1455, or 1456.\n", ch);
	      mem_free (search_args);
	      return;
	    }
	}
      else if (!strcasecmp (buf, "player"))
	{
	  args = one_argument (args, buf);
	  if (!*buf)
	    {
	      send_to_char ("Expected a player name to search for.\n", ch);
	      mem_free (search_args);
	      return;
	    }
	  else
	    sprintf (name, "%s", buf);
	}
      else if (!strcasecmp (buf, "account"))
	{
	  args = one_argument (args, buf);
	  if (!*buf)
	    {
	      send_to_char ("Expected an account name to search for.\n", ch);
	      mem_free (search_args);
	      return;
	    }
	  else
	    sprintf (account_name, "%s", buf);
	}
      else if (!strcasecmp (buf, "command"))
	{
	  args = one_argument (args, buf);
	  if (!*buf)
	    {
	      send_to_char ("Expected a command to search for.\n", ch);
	      mem_free (search_args);
	      return;
	    }
	  else
	    sprintf (command, "%s", buf);
	}
      else if (!strcasecmp (buf, "npc"))
	{
	  args = one_argument (args, buf);
	  if (!*buf)
	    {
	      send_to_char ("Expected an NPC name to search for.\n", ch);
	      mem_free (search_args);
	      return;
	    }
	  else
	    sprintf (npc, "%s", buf);
	}
      else if (!strcasecmp (buf, "room"))
	{
	  args = one_argument (args, buf);
	  if (!isdigit (*buf))
	    {
	      send_to_char ("Expected a room number to search for.\n", ch);
	      mem_free (search_args);
	      return;
	    }
	  else
	    room_num = atoi (buf);
	}
      else if (!strcasecmp (buf, "minutes"))
	{
	  args = one_argument (args, buf);
	  if (!isdigit (*buf))
	    {
	      send_to_char
		("Expected a number of minutes to search within.\n", ch);
	      mem_free (search_args);
	      return;
	    }
	  else
	    minutes = atoi (buf);
	}
      else if (!strcasecmp (buf, "hours"))
	{
	  args = one_argument (args, buf);
	  if (!isdigit (*buf))
	    {
	      send_to_char ("Expected a number of hours to search within.\n",
			    ch);
	      mem_free (search_args);
	      return;
	    }
	  else
	    hours = atoi (buf);
	}
      else if (!strcasecmp (buf, "days"))
	{
	  args = one_argument (args, buf);
	  if (!isdigit (*buf))
	    {
	      send_to_char ("Expected a number of days to search within.\n",
			    ch);
	      mem_free (search_args);
	      return;
	    }
	  else
	    days = atoi (buf);
	}
      else if (!strcasecmp (buf, "guest"))
	guest = true;
      else if (!strcasecmp (buf, "immortal"))
	{
	  if (GET_TRUST (ch) < 5)
	    {
	      send_to_char
		("Only level 5 staff members may search the immortal logs.\n",
		 ch);
	      mem_free (search_args);
	      return;
	    }
	  else
	    immortal = true;
	}
      else if (!strcasecmp (buf, "system"))
	system = true;
      else if (!strcasecmp (buf, "error"))
	error = true;
      else if (!strcasecmp (buf, "ascend"))
	descend = false;
      else
	{
	  sprintf (buf2,
		   "You've specified an unknown log search parameter: '%s'.\n",
		   buf);
	  send_to_char (buf2, ch);
	  mem_free (search_args);
	  return;
	}
      if (!*args)
	break;
    }

  if (alloc && search_args)
    mem_free (search_args);

  if (!*search_args && !*argument)
    {
      send_to_char ("What did you wish to search the logs for?\n", ch);
      return;
    }

  sprintf (query,
	   "SELECT name, account, switched_into, timestamp, port, room, guest, immortal, error, command, entry FROM %s.mud WHERE ", log_db.c_str ());

  if (*argument)
    sprintf (query + strlen (query), "(");
  else
    {
      sprintf (query + strlen (query), "entry LIKE '%%')");
    }

  while (*argument && (argument = one_argument (argument, buf)))
    {
      if (!strcasecmp (buf, "and") || !str_cmp (buf, "&"))
	{
	  sprintf (query + strlen (query), " AND ");
	  prev_arg = false;
	  continue;
	}
      else if (!strcasecmp (buf, "or"))
	{
	  sprintf (query + strlen (query), " OR ");
	  prev_arg = false;
	  continue;
	}
      else if (prev_arg)
	{
	  send_to_char
	    ("You'll need to include either \"or\" or \"and\" for multiple terms.\n",
	     ch);
	  return;
	}
      sprintf (query + strlen (query), "entry LIKE '%%%s%%'", buf);
      prev_arg = true;
      if (!*argument)
	{
	  sprintf (query + strlen (query), ")");
	  break;
	}
    }

  if (!system)
    sprintf (query + strlen (query), " AND name != 'System'");
  else
    sprintf (query + strlen (query), " AND name = 'System'");

  if (!immortal)
    sprintf (query + strlen (query), " AND immortal = false");
  else
    sprintf (query + strlen (query), " AND immortal = true");

  if (!guest)
    sprintf (query + strlen (query), " AND guest = false");
  else
    sprintf (query + strlen (query), " AND guest = true");

  if (error)
    sprintf (query + strlen (query), " AND error = true");
  else
    sprintf (query + strlen (query), " AND error = false");

  /*
  if (minutes)
    sprintf (query + strlen (query),
	     " AND (UNIX_TIMESTAMP() - timestamp) <= 60 * %d", minutes);
  else if (hours)
    sprintf (query + strlen (query),
	     " AND (UNIX_TIMESTAMP() - timestamp) <= 60 * 60 * %d", hours);
  else if (days)
    sprintf (query + strlen (query),
	     " AND (UNIX_TIMESTAMP() - timestamp) <= 60 * 60 * 24 * %d",
	     days);

  if (!days && !hours && !minutes)
    sprintf (query + strlen (query),
	     " AND (UNIX_TIMESTAMP() - timestamp) <= 60 * 60 * 24 * 14");*/

  sprintf (query + strlen (query), " AND port = %d", port_num);

  if (room_num > -1)
    sprintf (query + strlen (query), " AND room = %d", room_num);

  if (*name)
    sprintf (query + strlen (query), " AND name LIKE '%s'", name);
  if (*account_name)
    sprintf (query + strlen (query), " AND account LIKE '%s'", account_name);
  if (*npc)
    sprintf (query + strlen (query), " AND switched_into LIKE '%s'", npc);
  if (*command)
    sprintf (query + strlen (query), " AND command LIKE '%s'", command);

  if (!descend)
    sprintf (query + strlen (query), " ORDER BY timestamp ASC");
  else
    sprintf (query + strlen (query), " ORDER BY timestamp DESC");

  sprintf (query + strlen (query), " LIMIT 1000");

  // Safe to assume that people with access to the log command won't be injecting SQL,
  // and mysql_safe_query() would be a royal pain in the arse to use here.

  mysql_real_query (database, query, strlen (query));
  result = mysql_store_result (database);

  if (!result || mysql_num_rows (result) <= 0)
    {
      send_to_char ("Your log search did not return any results.\n", ch);
      return;
    }

  *output = '\0';

  while ((row = mysql_fetch_row (result)))
    {
      curr_days = atoi (row[3]) / (60 * 60);

      if (curr_days != prev_days)
	{
	  current_time = atoi (row[3]);
	  date = asctime (localtime (&current_time));
	  date[strlen (date) - 1] = '\0';
	  if (descend)
	    sprintf (output + strlen (output), "\n#2Prior to %s:#0\n\n",
		     date);
	  else
	    sprintf (output + strlen (output), "\n#2After %s:#0\n\n", date);
	  prev_days = curr_days;
	}

      if (*npc)
	sprintf (buf, "%s (%s) [%s]: %s %s\n", row[2], row[0], row[5], row[9],
		 row[10]);
      else if (*account_name)
	sprintf (buf, "%s (%s) [%s]: %s %s\n", row[1], row[0], row[5], row[9],
		 row[10]);
      else if (system)
	sprintf (buf, "System Message: %s\n", row[10]);
      else
	sprintf (buf, "%s [%s]: %s %s\n", row[0], row[5], row[9], row[10]);

      if (strlen (output) + strlen (buf) >= MAX_STRING_LENGTH)
	{
	  truncated = true;
	  sprintf (buf, "...output truncated. Too many results.\n");
	  if (strlen (output) + strlen (buf) < MAX_STRING_LENGTH)
	    sprintf (output + strlen (output), "%s", buf);
	  else
	    break;
	}
      sprintf (output + strlen (output), "%s", buf);
    }

  if (!*output)
    {
      send_to_char ("Your log search did not return any results.\n", ch);
      mysql_free_result (result);
      return;
    }
  else
    {
      results = mysql_num_rows (result);
      sprintf (buf, "#6Entries Matching Your Query:#0 %d%s\r\n%s", results,
	       truncated ? " (truncated)" : "", output);
      page_string (ch->desc, buf);
    }

  mysql_free_result (result);
}

void
list_help (CHAR_DATA * ch, HELP_DATA * help)
{
  int i = 0;
  char buf[MAX_STRING_LENGTH];

  *buf = '\0';

  while (help)
    {

      if (help->master_element)
	{

	  sprintf (buf + strlen (buf), "%-18s ", help->keyword);

	  if (++i == 4)
	    {
	      i = 0;
	      send_to_char (buf, ch);
	      send_to_char ("\n", ch);
	      *buf = '\0';
	    }
	}

      help = help->next;
    }

  if (*buf)
    {
      send_to_char (buf, ch);
      send_to_char ("\n", ch);
    }
}

void
list_texts (CHAR_DATA * ch, TEXT_DATA * list)
{
  int i = 0;
  char buf[MAX_STRING_LENGTH];

  *buf = '\0';

  while (list)
    {

      sprintf (buf + strlen (buf), "%-18s ", list->name);

      if (++i == 4)
	{
	  i = 0;
	  send_to_char (buf, ch);
	  send_to_char ("\n", ch);
	  *buf = '\0';
	}

      list = list->next;
    }

  if (*buf)
    {
      send_to_char (buf, ch);
      send_to_char ("\n", ch);
    }
}

TEXT_DATA *
find_text (CHAR_DATA * ch, TEXT_DATA * list, char *buf)
{
  while (list)
    {

      if (!str_cmp (list->name, buf))
	return list;

      list = list->next;
    }

  return NULL;
}

char *
get_text_buffer (CHAR_DATA * ch, TEXT_DATA * list, char *text_name)
{
  TEXT_DATA *entry;

  if ((entry = find_text (ch, list, text_name)))
    return entry->text;

  return "(no info)\n";
}

int
get_edit_line_length (char **line)
{
  int length = 0;

  if (!**line)
    return length;

  while (**line && **line != '\n')
    {
      (*line)++;
      length++;
    }

  if (**line)
    {
      length++;
      (*line)++;
    }

  return length;
}

char *
get_document (int type, int index)
{
  if (type == EDIT_TYPE_DOCUMENT || type == EDIT_TYPE_TEXT)
    return ((TEXT_DATA *) index)->text;

  if (type == EDIT_TYPE_HELP || type == EDIT_TYPE_BHELP)
    return ((HELP_DATA *) index)->help_info;

  printf ("Type: %d, index: %d\n", type, index);
  fflush (stdout);

  return NULL;
}

void
set_document (int type, int index, char *new_string)
{
  TEXT_DATA *text;
  HELP_DATA *element;

  if (type == EDIT_TYPE_DOCUMENT || type == EDIT_TYPE_TEXT)
    {
      text = (TEXT_DATA *) index;
      mem_free (text->text);
      text->text = new_string;
    }

  else if (type == EDIT_TYPE_HELP || type == EDIT_TYPE_BHELP)
    {
      element = (HELP_DATA *) index;
      mem_free (element->help_info);
      element->help_info = new_string;
    }

  else
    {
      printf ("type %d, index %d\n", type, index);
    }
}

void
edit_string (DESCRIPTOR_DATA * d, std::string argument)
{
  int new_doc_length;
  int length = 0;
  int i;
  char *document;
  char *new_doc;
  char *p;

  if (argument[0] != '@')
    {
      p =
	(char *) alloc (strlen (d->edit_string) + argument.length () + 2, 35);

      strcpy (p, d->edit_string);
      strcat (p, argument.c_str ());
      strcat (p, "\n");

      mem_free (d->edit_string);

      d->edit_string = p;

      if (d->edit_line_first >= 100)
	SEND_TO_Q ("   >", d);
      else
	SEND_TO_Q ("  >", d);

      return;
    }

  document = get_document (d->edit_type, d->edit_index);

  new_doc_length = strlen (document) -
    d->edit_length + strlen (d->edit_string) + 1;

  new_doc = (char *) alloc (new_doc_length, 35);

  *new_doc = '\0';

  p = document;

  /* How many bytes from the beginning of original string? */

/* printf ("line_first - 1 = %d\n", d->edit_line_first); */
  for (i = 1; i < d->edit_line_first; i++)
    length += get_edit_line_length (&p);

  memcpy (new_doc, document, length);
/* printf ("Doc %d:\n%s<-", length, new_doc); */
  strcpy (new_doc + length, d->edit_string);
/* printf ("Plus:\n%s<-", new_doc);
   fflush (stdout);
*/

  /* Add lines at end of the document */

  strcpy (new_doc + strlen (new_doc), document + length + d->edit_length);

  mem_free (d->edit_string);

  set_document (d->edit_type, d->edit_index, new_doc);

  d->edit_index = -1;

  SEND_TO_Q ("Document needs to be saved.\n", d);
}

int
doc_parse (CHAR_DATA * ch, char *argument, char **start, int *length,
	   int *start_line, int *doc_type)
{
  int doc_num;
  int line_start;
  int line_end;
  int line;
  int i;
  HELP_DATA *element;
  TEXT_DATA *text;
  char *document;
  char *p;
  char buf[MAX_STRING_LENGTH];

  *start_line = 1;

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      send_to_char ("Expected TEXT, DOCUMENT, HELP, or BHELP.\n", ch);
      return -1;
    }

  if (*buf == '.')
    {
      if (!ch->pc)
	{
	  send_to_char ("Only use '.' while being a PC.\n", ch);
	  return -1;
	}

      if (ch->pc->doc_index == -1)
	{
	  send_to_char ("No document set.  Use DOCUMENT <doc> first.\n", ch);
	  return -1;
	}

      *doc_type = ch->pc->doc_type;
      doc_num = ch->pc->doc_index;

      document = get_document (*doc_type, doc_num);
    }

  else if (!str_cmp (buf, "help"))
    {

      argument = one_argument (argument, buf);

      if (!*buf)
	{
	  list_help (ch, help_list);
	  return -1;
	}

      if (!(element = is_help (ch, help_list, buf)))
	{
	  send_to_char ("No such help topic.\n", ch);
	  return -1;
	}

      *doc_type = EDIT_TYPE_HELP;
      doc_num = (long int) element;
      document = element->help_info;
    }

  else if (!str_cmp (buf, "bhelp"))
    {

      argument = one_argument (argument, buf);

      if (!*buf)
	{
	  list_help (ch, bhelp_list);
	  return -1;
	}

      if (!(element = is_help (ch, bhelp_list, buf)))
	{
	  send_to_char ("No such bhelp topic.\n", ch);
	  return -1;
	}

      *doc_type = EDIT_TYPE_BHELP;
      doc_num = (long int) element;
      document = element->help_info;
    }

  else if (!str_cmp (buf, "text"))
    {

      argument = one_argument (argument, buf);

      if (!*buf)
	{
	  list_texts (ch, text_list);
	  return -1;
	}

      if (!(text = find_text (ch, text_list, buf)))
	{
	  send_to_char ("No such text.\n", ch);
	  return -1;
	}

      *doc_type = EDIT_TYPE_TEXT;
      doc_num = (long int) text;
      document = text->text;
    }

  else if (!str_cmp (buf, "document") ||
	   !str_cmp (buf, "doc") || !str_cmp (buf, "documents"))
    {

      argument = one_argument (argument, buf);

      if (!*buf)
	{
	  list_texts (ch, document_list);
	  return -1;
	}

      if (!(text = find_text (ch, document_list, buf)))
	{
	  send_to_char ("No such document.\n", ch);
	  return -1;
	}

      *doc_type = EDIT_TYPE_DOCUMENT;
      doc_num = (long int) text;
      document = text->text;
    }

  else
    {
      send_to_char ("No such document type, expected on of TEXT, DOCUMENT, "
		    "HELP or BHELP.\n", ch);
      return -1;
    }

  ch->pc->doc_type = *doc_type;
  ch->pc->doc_index = doc_num;

  argument = one_argument (argument, buf);

  if (!*buf)
    {
      *length = strlen (document);
      *start = document;
      *start_line = 1;
      return doc_num;
    }

  if (*buf == '[')
    {
      *length = 0;
      *start = document;
      *start_line = 1;
      return doc_num;
    }

  if (*buf == ']')
    {
      *length = 0;
      *start = &document[strlen (document)];

      *start_line = 0;

      for (p = document; *p; (*start_line)++)
	get_edit_line_length (&p);

      return doc_num;
    }

  if (!isdigit (*buf))
    {
      send_to_char ("Expected line or range:  ##  or  ##/##\n", ch);
      return -1;
    }

  if (just_a_number (buf))
    {

      line = atoi (buf);
      *start_line = line;
      *length = 0;
      *start = document;

      while (**start && line-- > 0)
	get_edit_line_length (start);

      return doc_num;
    }

  line_start = atoi (buf);
  p = buf;

  while (isdigit (*p))
    p++;

  if (!*p || *p != '/')
    {
      send_to_char ("Expected '/' between line numbers.\n", ch);
      return -1;
    }

  p++;

  line_end = atoi (p);

  if (line_start > line_end)
    {
      send_to_char ("You cannot edit a negative range like that.\n", ch);
      return -1;
    }

  if (line_start < 1)
    line_start = 1;

  *start_line = line_start;

  *start = document;

  for (i = 1; i < line_start; i++)
    {
      get_edit_line_length (start);
      if (!**start)
	{
	  send_to_char ("You can't edit beyond the last line like that.\n",
			ch);
	  return -1;
	}
    }

  *length = 0;
  p = *start;

  while (i++ <= line_end)
    {
      *length += get_edit_line_length (&p);
      if (!*p)
	{
	  (*length)--;
	  return doc_num;
	}
    }

  return doc_num;
}

void
do_edit (CHAR_DATA * ch, char *argument, int cmd)
{
  int length;
  int line_start;
  int doc_type;
  int doc_num;
  char *start;

  if (!ch->desc)
    {
      send_to_char ("This is a PC only command.\n", ch);
      return;
    }

  do_print (ch, argument, cmd);

  while (isspace (*argument))
    argument++;

  if (!*argument)
    return;

  if (-1 == (doc_num = doc_parse (ch, argument, &start, &length,
				  &line_start, &doc_type)))
    return;

  ch->desc->edit_string = str_dup ("");

  ch->desc->edit_type = doc_type;
  ch->desc->edit_index = doc_num;
  ch->desc->edit_line_first = line_start;
  ch->desc->edit_length = length;

  if (ch->desc->edit_line_first >= 100)
    SEND_TO_Q ("   >", ch->desc);
  else
    SEND_TO_Q ("  >", ch->desc);
}

void
do_print (CHAR_DATA * ch, char *argument, int cmd)
{
  int length;
  int line_start;
  int line_length;
  int doc_type;
  char *start;
  char *cur_line;
  char *p;
  char buf[MAX_STRING_LENGTH];

  if (-1 == doc_parse (ch, argument, &start, &length, &line_start, &doc_type))
    return;

  cur_line = start;

  for (; *cur_line && start + length > cur_line; line_start++)
    {

      p = cur_line;

      line_length = get_edit_line_length (&cur_line);

      if (!*cur_line)
	return;

      sprintf (buf, "%2d>", line_start);

      buf[strlen (buf) + line_length] = '\0';

      memcpy (buf + strlen (buf), p, line_length);

      send_to_char (buf, ch);
    }
}

void
save_document (CHAR_DATA * ch, char *argument)
{
  char *p;
  TEXT_DATA *text;
  char buf[MAX_STRING_LENGTH];

  p = argument;
  argument = one_argument (argument, buf);

  if (!str_cmp (buf, "help"))
    {
      write_help (HELP_FILE, help_list);
      send_to_char ("Help file written.\n", ch);
      return;
    }

  if (!str_cmp (buf, "bhelp"))
    {
      write_help (BHELP_FILE, bhelp_list);
      send_to_char ("Bhelp file written.\n", ch);
      return;
    }

  if (!str_cmp (buf, "text"))
    {

      text = find_text (ch, text_list, argument);

      if (!text)
	{
	  send_to_char ("No such text.\n", ch);
	  return;
	}

      write_text (ch, text);
      return;
    }

  if (!str_cmp (buf, "document"))
    {

      text = find_text (ch, document_list, argument);

      if (!text)
	{
	  send_to_char ("No such document.\n", ch);
	  return;
	}

      write_text (ch, text);
      return;
    }

  if (*buf == '.')
    {

      if (ch->pc->doc_index == -1)
	{
	  send_to_char ("You don't have a current document set.\n", ch);
	  return;
	}

      if (ch->pc->doc_type == EDIT_TYPE_DOCUMENT ||
	  ch->pc->doc_type == EDIT_TYPE_TEXT)
	write_text (ch, (TEXT_DATA *) ch->pc->doc_index);

      else if (ch->pc->doc_type == EDIT_TYPE_HELP)
	write_help (HELP_FILE, help_list);

      else if (ch->pc->doc_type == EDIT_TYPE_BHELP)
	write_help (BHELP_FILE, bhelp_list);

      else
	{
	  send_to_char ("Unknown document type for save.\n", ch);
	  return;
	}

      send_to_char ("Written.\n", ch);
      return;
    }

  send_to_char ("Expected '.', DOCUMENT, TEXT, HELP, or BHELP.\n", ch);
}

void
write_text (CHAR_DATA * ch, TEXT_DATA * text)
{
  int bytes;
  int bytes_written;
  char *p;
  FILE *fp_doc;

  unlink (text->filename);

  if (!(fp_doc = fopen (text->filename, "w")))
    {
      perror ("save_document");
      send_to_char ("Tragically, there was a problem writing the text.  "
		    "It is probably lost now.\n", ch);
      return;
    }

  p = text->text;
  bytes = strlen (p);

  while (bytes)
    {

      bytes_written = fwrite (p, 1, bytes, fp_doc);

      if (!bytes_written)
	{
	  perror ("fwrite of text");
	  break;
	}

      bytes -= bytes_written;
      p += bytes_written;
    }

  if (bytes == (int) strlen (text->text))
    send_to_char ("No part of the text could be written!\n", ch);
  else if (bytes)
    send_to_char ("Part of the text could not be written!\n", ch);
  else
    send_to_char ("Document written.\n", ch);

  fclose (fp_doc);
}

HELP_DATA *
add_help_topics (CHAR_DATA * ch, HELP_DATA ** list, char *argument)
{
  HELP_DATA *master_element;
  HELP_DATA *element;
  HELP_DATA *last_element = NULL;
  char *p;
  char buf[MAX_STRING_LENGTH];

  for (p = one_argument (argument, buf); *buf; p = one_argument (p, buf))
    {

      for (element = *list; ch && element; element = element->next)
	{
	  if (!element->master_element)
	    continue;

	  if (!str_cmp (element->keyword, buf))
	    {
	      sprintf (buf, "Keyword %s is already in use.\n", buf);
	      send_to_char (buf, ch);
	      return NULL;
	    }
	}
    }

  if (*list)
    for (last_element = *list;
	 last_element->next; last_element = last_element->next)
      ;

  master_element = (HELP_DATA *) alloc (sizeof (HELP_DATA), 36);
  master_element->keywords = str_dup (argument);
  master_element->help_info = str_dup ("");

  if (last_element)
    last_element->next = master_element;
  else
    *list = master_element;

  last_element = master_element;

  for (p = one_argument (argument, buf); *buf; p = one_argument (p, buf))
    {

      element = (HELP_DATA *) alloc (sizeof (HELP_DATA), 36);

      element->master_element = master_element;
      element->help_info = NULL;
      element->keyword = str_dup (buf);

      last_element->next = element;
      last_element = element;
    }

  return master_element;
}

void
delete_help_topics (CHAR_DATA * ch, HELP_DATA ** list, char *argument)
{
}

TEXT_DATA *
add_document (CHAR_DATA * ch, TEXT_DATA ** list, char *argument)
{
  TEXT_DATA *element;
  TEXT_DATA *last_element;
  char *p;
  char doc_name[MAX_STRING_LENGTH];
  char filename[MAX_STRING_LENGTH];

  argument = one_argument (argument, doc_name);

  for (element = *list; element; element = element->next)
    {
      if (!str_cmp (element->name, doc_name))
	{
	  send_to_char ("That document already exists.\n", ch);
	  return NULL;
	}

      last_element = element;
    }

  for (p = doc_name; *p; p++)
    {
      if (!isalnum (*p) && *p != '_' && *p != '-' && *p != '.')
	{
	  send_to_char ("Document name restricted to characters a-z, A-Z, "
			"_, -, and .\n", ch);
	  return NULL;
	}
    }

  strcpy (filename, "documents/");
  strcat (filename, doc_name);

  return add_text (list, filename, doc_name);
}

void
delete_document (CHAR_DATA * ch, TEXT_DATA ** list, char *argument)
{
  TEXT_DATA *element;
  char doc_name[MAX_STRING_LENGTH];

  one_argument (argument, doc_name);

  if (!*doc_name)
    {
      send_to_char ("Delete which document?\n", ch);
      return;
    }

  if (!*list)
    {
      send_to_char ("There are no documents.\n", ch);
      return;
    }

  if (!str_cmp ((*list)->name, doc_name))
    {
      element = *list;
      *list = element->next;
    }

  else
    {
      for (element = *list; element->next; element = element->next)
	{
	  if (!str_cmp (element->next->name, doc_name))
	    {
	      element = element->next;
	      break;
	    }
	}
    }

  if (str_cmp (element->name, doc_name))
    {
      send_to_char ("Document not found.\n", ch);
      return;
    }

  unlink (element->filename);

  mem_free (element->text);
  mem_free (element->filename);
  mem_free (element->name);
  mem_free (element);
}

void
write_dynamic_registry (CHAR_DATA * ch)
{
  FILE *fp_dr;
  NAME_SWITCH_DATA *nsp;

  if (!(fp_dr = fopen (DYNAMIC_REGISTRY ".new", "w")))
    {
      perror (DYNAMIC_REGISTRY ".new");
      system_log ("Unable to write dynamic registry!", true);

      if (ch)
	send_to_char ("Unable to write dynamic registry!", ch);
    }

  for (nsp = clan_name_switch_list; nsp; nsp = nsp->next)
    fprintf (fp_dr, "newclanname %s %s\n", nsp->old_name, nsp->new_name);
  clan__filedump (fp_dr);

  fclose (fp_dr);

  system ("mv " DYNAMIC_REGISTRY " " DYNAMIC_REGISTRY ".old");
  system ("mv " DYNAMIC_REGISTRY ".new " DYNAMIC_REGISTRY);

  if (ch)
    send_to_char ("Dynamic registry updated.\n", ch);
}



#define MAP_MAX_RADIUS 3
#define MAP_GRID_WIDTH ((MAP_MAX_RADIUS * 2) + 1)
#define MAP_GRID_DEPTH 5

void
fill_map (ROOM_DATA * ptrRoom, int x, int y,
	  int map[MAP_GRID_DEPTH][MAP_GRID_WIDTH][MAP_GRID_WIDTH])
{
  int n = 0, e = 0, s = 0, w = 0;
  static unsigned char radius = 0;
  ROOM_DIRECTION_DATA *ptrNExit = NULL;
  ROOM_DIRECTION_DATA *ptrEExit = NULL;
  ROOM_DIRECTION_DATA *ptrSExit = NULL;
  ROOM_DIRECTION_DATA *ptrWExit = NULL;

  if (!ptrRoom)
    return;

  if (!map[0][x][y])
    {
      map[0][x][y] = ptrRoom->nVirtual;
    }
  map[1][x][y] = (ptrRoom->sector_type >= 0
		  && ptrRoom->sector_type < 21) ? ptrRoom->sector_type : 21;
  if ((ptrEExit = ptrRoom->dir_option[1]) != NULL)
    {
      map[2][x][y] = (IS_SET (ptrEExit->exit_info, (EX_ISDOOR || EX_ISGATE))) ? 2 : 1;
    }
  if ((ptrSExit = ptrRoom->dir_option[2]) != NULL)
    {
      map[3][x][y] = (IS_SET (ptrSExit->exit_info, (EX_ISDOOR || EX_ISGATE))) ? 2 : 1;
    }
  if ((ptrRoom->dir_option[4]) != NULL)
    {
      map[4][x][y] = 1;
    }
  if ((ptrRoom->dir_option[5]) != NULL)
    {
      map[4][x][y] = (map[4][x][y]) ? 3 : 2;
    }


  if ((y > 0) && !map[0][x][y - 1] && (ptrNExit = ptrRoom->dir_option[0]))
    {
      n = ptrNExit->to_room;
    }
  if (!map[0][x + 1][y] && (x + 1 < MAP_GRID_WIDTH) && (ptrEExit != NULL))
    {
      e = ptrEExit->to_room;
    }
  if (!map[0][x][y + 1] && (y + 1 < MAP_GRID_WIDTH) && (ptrSExit != NULL))
    {
      s = ptrSExit->to_room;
    }
  if (!map[0][x - 1][y] && (x - 1 >= 0)
      && (ptrWExit = ptrRoom->dir_option[3]))
    {
      w = ptrWExit->to_room;
    }

  if (radius > MAP_MAX_RADIUS + 1)
    return;

  radius++;

  if (n)
    {
      fill_map (vtor (n), x, y - 1, map);
    }
  if (e)
    {
      fill_map (vtor (e), x + 1, y, map);
    }
  if (s)
    {
      fill_map (vtor (s), x, y + 1, map);
    }
  if (w)
    {
      fill_map (vtor (w), x - 1, y, map);
    }
  radius--;
}



void
do_map (CHAR_DATA * ch, char *argument, int cmd)
{
  int map[MAP_GRID_DEPTH][MAP_GRID_WIDTH][MAP_GRID_WIDTH];
  char buf[AVG_STRING_LENGTH * 2] = "";
  char buf2[AVG_STRING_LENGTH] = "";
  char arg1[AVG_STRING_LENGTH] = "";
  char bogy[4][2][6] = {
    {" ", " "},
    {"#b(#0", "#b)#0"},
    {"#d(#0", "#d)#0"},
    {"#f(#0", "#f)#0"}
  };
  const char strEWall[3][7] = {
    "#1X#0",
    " ",
    "#1|#0"
  };
  const char strSWall[3][18] = {
    "#1XXXXXXXXXXX#0",
    "          #1X#0",
    "#1----------X#0"
  };
  const char strVExit[4][7] = {
    " ",
    "#6'#0",
    "#6,#0",
    "#6%#0"
  };
  const char *strSect[] = {
    "", "#7", "#7", "#7", "#a",
    "#2", "#2", "#3", "#b", "#e",
    "#6", "#e", "#a", "#3", "#d",
    "", "#4", "#4", "#4", "#6",
    "#c", "#9"
  };
  unsigned char i = 0, j = 0, x = 0, y = 0, nInRoom = 0, bSearch = 0;
  int r = 0;
  CHAR_DATA *rch;
  ROOM_DATA *room;

  if (*argument)
    {
      bSearch = 1;
      argument = one_argument (argument, arg1);
    }

  for (i = 0; i < MAP_GRID_WIDTH; i++)
    {
      for (j = 0; j < MAP_GRID_WIDTH; j++)
	{
	  map[0][i][j] = 0;	/* room */
	  map[1][i][j] = 0;	/* sect */
	  map[2][i][j] = 0;	/* e links */
	  map[3][i][j] = 0;	/* s links */
	  map[4][i][j] = 0;	/* u/d links */
	}
    }
  x = MAP_MAX_RADIUS;
  y = MAP_MAX_RADIUS;

  fill_map (ch->room, x, y, map);

  strcpy (buf, "#0\n");
  for (j = 0; j < MAP_GRID_WIDTH; j++)
    {
      for (i = 0; i < MAP_GRID_WIDTH; i++)
	{
	  if ((r = map[0][i][j]) > 0)
	    {
	      if ((room = vtor (r)))
		{

		  if (i == MAP_MAX_RADIUS && j == MAP_MAX_RADIUS)
		    {
		      sprintf (buf + strlen (buf), " #d<#0%s%6d#d>#0%s%s",
			       strSect[map[1][i][j]],
			       r,
			       strVExit[map[4][i][j]],
			       strEWall[map[2][i][j]]);
		    }
		  else
		    {
		      nInRoom = 0;
		      if ((rch = room->people))
			{
			  for (; rch; rch = rch->next_in_room)
			    {
			      if (bSearch && strstr (rch->name, arg1))
				{
				  nInRoom = 3;
				  break;
				}
			      if (GET_TRUST (rch))
				{
				  nInRoom = 2;
				  break;
				}
			      if (!IS_NPC (rch))
				{
				  nInRoom = 1;
				  break;
				}
			    }
			}
		      sprintf (buf + strlen (buf), " %s%s%6d#0%s%s%s",
			       bogy[nInRoom][0],
			       strSect[map[1][i][j]],
			       r,
			       bogy[nInRoom][1],
			       strVExit[map[4][i][j]],
			       strEWall[map[2][i][j]]);
		    }
		}
	      strcat (buf2, strSWall[map[3][i][j]]);

	    }
	  else
	    {
	      sprintf (buf + strlen (buf), "          %s",
		       ((i < MAP_GRID_WIDTH - 1)
			&& (map[0][i + 1][j])) ? strEWall[0] : " ");
	      if (j < MAP_GRID_WIDTH - 1 && map[0][i][j + 1])
		{
		  strcat (buf2, strSWall[0]);
		}
	      else
		{
		  sprintf (buf2 + strlen (buf2), "          %s",
			   ((i < MAP_GRID_WIDTH - 1)
			    && (map[0][i + 1][j])) ? strEWall[0] : "#1+#0");
		}
	    }
	}
      strcat (buf, "#0\n");
      strcat (buf2, "#0\n");
      strcat (buf, buf2);
      send_to_char (buf, ch);
      buf[0] = '\0';
      buf[1] = '\0';
      buf2[0] = '\0';
      buf[2] = '\0';
    }

  strcpy (buf, "\n  #6'#0 Up       #6,#0 Down     #6%#0 Up/Down\n  ");

  for (i = 0; sector_types[i][0] != '\n'; i++)
    {
      sprintf (buf + strlen (buf), "%s%-10s#0 ", strSect[i], sector_types[i]);
      if (!((i + 1) % 7))
	strcat (buf, "\n  ");
    }

  send_to_char (buf, ch);
}

void
do_aecho (CHAR_DATA * ch, char *argument, int cmd)
{


}

#include <fstream>
bool
csv_obj (const CHAR_DATA* ch)
{
  bool success = false;

  std::ofstream fp ("/tmp/objects.csv");
  
  if (fp)
    {
      for (OBJ_DATA* tobj = full_object_list; tobj; tobj = tobj->lnext)
	{
	  fp 
	    << tobj->zone << ", "
	    << tobj->nVirtual << ", "
	    << '"' << item_types[(size_t)tobj->obj_flags.type_flag] << '"' << ", "
	    << '"' << tobj->name << '"' << ", "
	    << '"' << tobj->short_description << '"' << ", "
	    << '"' << tobj->description << '"' << ", "
	    << (((float)tobj->obj_flags.weight) * 0.01) << ", "
	    << tobj->farthings << ';' << std::endl;
	}
      fp.close ();
      success = true;
    }
  else
    {
      send_to_char ("Error: Cannot open objects.csv for writing\n", ch);
    }
  return success;
}

bool
csv_craft (const CHAR_DATA* ch)
{
  bool success = false;

  std::ofstream fp ("/tmp/crafts.csv");
  
  if (fp)
    {
      for (OBJ_DATA* tobj = full_object_list; tobj; tobj = tobj->lnext)
	{
	  fp 
	    << tobj->zone << ", "
	    << tobj->nVirtual << ", "
	    << '"' << item_types[(size_t)tobj->obj_flags.type_flag] << '"' << ", "
	    << '"' << tobj->name << '"' << ", "
	    << '"' << tobj->short_description << '"' << ", "
	    << '"' << tobj->description << '"' << ", "
	    << (((float)tobj->obj_flags.weight) * 0.01) << ", "
	    << tobj->farthings << ';' << std::endl;
	}
      fp.close ();
      success = true;
    }
  else
    {
      send_to_char ("Error: Cannot open objects.csv for writing\n", ch);
    }
  return success;
}

void
do_becho (CHAR_DATA * ch, char *argument, int cmd)
{
	int i;
  std::string output;
	char buf[MAX_STRING_LENGTH];
	
  for (i = 0; *(argument + i) == ' '; i++);

  if (!*(argument + i))
    {
      send_to_char ("What did you want to echo?\n", ch);
      return;
    }

	output.assign ("#9<<<****************************************>>>#0\n");
	output.append ("#B<<<---------------------------------------->>>#0\n\n");
  
  sprintf (buf, "%s\n\n", argument + i);
  output.append (buf);
  
  output.append ("#B<<<---------------------------------------->>>#0\n");
  output.append ("#9<<<****************************************>>>#0\n");

  
  send_to_room ((char*)output.c_str(), ch->in_room);

}

void
do_csv (CHAR_DATA* ch, char *argument, int cmd)
{
  if (argument[0] == 'o')
    {
      send_to_char ("Begining Object Save...", ch);
      if (csv_obj (ch))
	send_to_char ("Done\n", ch);
    }
  else if  (argument[0] == 'c')
    {
      send_to_char ("Begining Craft Save...", ch);
      if (csv_craft (ch))
	send_to_char ("Done\n", ch);      
    }
  else
    {
      send_to_char ("Usage:\no: objects\nc: crafts\n", ch);
    }
}

void
do_fallback(CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH];

  ROOM_DATA *room;

  argument = one_argument (argument, buf);

  if (!IS_NPC (ch))
    {
      send_to_char ("This command is for use by NPCs only.\n", ch);
      return;
    }
 
  if(!*buf)
  {
    if (IS_SET (ch->affected_by, AFF_FALLBACK))
    {
      send_to_char ("This mobile will no longer fall back.\n", ch);
      ch->affected_by &= ~AFF_FALLBACK;
      return;
    }
    else
    {
      send_to_char ("This mobile will now fall back.\n", ch);
      ch->affected_by |= AFF_FALLBACK;
      return;
    }
  }

  if(isdigit(buf[0]))
  {
    if (!(room = vtor (atoi (buf))))
    {
      send_to_char ("No such room exists\n", ch);
      return;
    }
    else
    {
      ch->mob->fallback = atoi (buf);
      sprintf(buf, "Fallback room set as #6[%d] %s#0.\n", room->nVirtual, room->name);
      send_to_char (buf, ch);
      return;
    }
  }
  else
  {
    if(!str_cmp(buf, "here"))
    {
      ch->mob->fallback = ch->in_room;
      sprintf(buf, "Fallback room set as here: #6[%d] %s#0.\n", ch->in_room, ch->room->name);
      send_to_char (buf, ch);
      return;
    }
    else if(!str_cmp(buf, "on"))
    {
      if (IS_SET (ch->affected_by, AFF_FALLBACK))
        send_to_char ("This mobile is already falling back.\n", ch);
      else
      {
        send_to_char ("This mobile will now fall back.\n", ch);
        ch->affected_by |= AFF_FALLBACK;
      }
      return;
    }
    else if(!str_cmp(buf, "off"))
    {
      if (IS_SET (ch->affected_by, AFF_FALLBACK))
      {
        send_to_char ("This mobile will no longer fall back.\n", ch);
        ch->affected_by &= ~AFF_FALLBACK;
      }
      else
      {
        send_to_char ("This mobile wasn't falling back.\n", ch);
      }
      return;
    }
  }
}

void
do_aggro (CHAR_DATA * ch, char *argument, int cmd)
{
  if (!IS_NPC (ch))
    {
      send_to_char ("This command is for use by NPCs only.\n", ch);
      return;
    }

  if (!engine.in_play_mode ())
    {
      send_to_char ("This command is for use on the player port only.\n", ch);
      return;
    }

  if (IS_SET (ch->act, ACT_AGGRESSIVE))
    {
      send_to_char ("This mobile is no longer aggressive.\n", ch);
      ch->act &= ~ACT_AGGRESSIVE;
      return;
    }
  else
    {
      send_to_char ("This mobile is now AGGRESSIVE!\n", ch);
      ch->act |= ACT_AGGRESSIVE;
      return;
    }
}

void
do_sentinel (CHAR_DATA * ch, char *argument, int cmd)
{
  if (!IS_NPC (ch))
    {
      send_to_char ("This command is for use by NPCs only.\n", ch);
      return;
    }

  if (!engine.in_play_mode ())
    {
      send_to_char ("This command is for use on the player port only.\n", ch);
      return;
    }

  if (IS_SET (ch->act, ACT_SENTINEL))
    {
      send_to_char ("This mobile is no longer a roamer.\n", ch);
      ch->act &= ~ACT_SENTINEL;
      return;
    }
  else
    {
      send_to_char ("This mobile is now a roamer.\n", ch);
      ch->act |= ACT_SENTINEL;
      return;
    }
}

void do_wmotd(CHAR_DATA * ch, char *argument, int cmd)
{
  
  std::string msg_line;
	std::string output;
 
	std::ifstream fin( "MOTD" );
	
	if( !fin )
		{
			system_log ("The MOTD could not be found", true);
			send_to_char("The MOTD could not be found", ch);
			return;
 	}

 while( getline(fin, msg_line) )
		{
		 output.append(msg_line);
		}
	
 	fin.close();
 
 	if (!output.empty())
    {
      send_to_char ("The old MOTD was: \n", ch);
      send_to_char (output.c_str(), ch);
      send_to_char ("\n", ch);
    }

  	act ("$n begins editing the MOTD.", false, ch, 0, 0, TO_ROOM);

		CREATE (ch->desc->pending_message, MESSAGE_DATA, 1);
		ch->desc->str = &ch->desc->pending_message->message;
		ch->desc->max_str = MAX_STRING_LENGTH;
		ch->delay_info1 = ch->room->nVirtual;
	
		send_to_char
				("Please enter the new MOTD; terminate with an '@'\n\n", ch);
		send_to_char
  	("1-------10--------20--------30--------40--------50--------60---65\n",
   ch);
	make_quiet (ch);

	ch->desc->proc = post_motd;

	return;
}


void
post_motd (DESCRIPTOR_DATA * d)
{
  CHAR_DATA *ch;
  char buf[MAX_STRING_LENGTH];
  char endl = {'\n'};
  
  std::ofstream fout ("MOTD");
  
  ch = d->character;
  ch->delay_info1 = 0;

  if (!*d->pending_message->message)
  {
    send_to_char ("No MOTD posted.\n", ch);
    return;
  }

  if( !fout )
  {
    system_log ("MOTD could not be found", true);
    send_to_char("MOTD could not be found", ch);
    return;
   }
	
  sprintf (buf, "%s", d->pending_message->message);
	
  fout << buf << endl;
  //fout.write(buf);
  fout.close();
 
  d->pending_message = NULL;
 	

}
