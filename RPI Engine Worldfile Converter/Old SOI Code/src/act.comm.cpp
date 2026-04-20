/*------------------------------------------------------------------------\
|  act.comm.c : Communication Module		      www.middle-earth.us |
|  Copyright (C) 2005, Shadows of Isildur: Traithe		          |
|  Derived under license from DIKU GAMMA (0.0).				  |
\------------------------------------------------------------------------*/

#include <string>
#include <time.h>
#include <stdio.h>
#include <string.h>
#include <ctype.h>
#include <stdlib.h>
#include <time.h>


#include "structs.h"
#include "net_link.h"
#include "account.h"
#include "protos.h"
#include "utils.h"
#include "utility.h"
#include "group.h"

/* extern variables */

extern ROOM_DATA *world;
extern DESCRIPTOR_DATA *descriptor_list;
extern const char *skills[];

#define TOK_WORD		0
#define	TOK_NEWLINE		1
#define TOK_PARAGRAPH	2
#define TOK_END			3
#define TOK_SENTENCE	4

int AudienceRoom[5] = {11281, 11283, 11285, 11290, 11288};
int StageRoom = 11286;

void
reformat_say_string (char *source, char **target, CHAR_DATA * to)
{
  int token_value = 0;
  int first_line = 1;
  int line_len = 0;
  char *s = '\0';
  char *r = '\0';
  char token[MAX_STRING_LENGTH] = { '\0' };
  char result[MAX_STRING_LENGTH] = { '\0' };

  s = source;
  r = result;
  *result = '\0';

  line_len = 0;

  while (token_value != TOK_END)
    {

      token_value = get_token (&s, token);

      if (token_value == TOK_PARAGRAPH)
	{

	  if (first_line)
	    first_line = 0;
	  else
	    strcat (result, "\n");

	  strcat (result, "   ");
	  line_len = 3;
	  continue;
	}

      if (token_value == TOK_NEWLINE)
	{
	  if (line_len != 0)
	    strcat (result, "\n");	/* Catch up */
	  strcat (result, "\n");
	  line_len = 0;
	  continue;
	}

      if (token_value == TOK_WORD)
	{
	  if (line_len + strlen (token) > 72)
	    {
	      strcat (result, "\n    ");
	      line_len = 0;
	    }

	  strcat (result, token);
	  strcat (result, " ");
	  line_len += strlen (token) + 1;
	}
    }

  result[strlen (result) - 1] = '\0';

  if (result[strlen (result) - 1] != '.' &&
      result[strlen (result) - 1] != '!' &&
      result[strlen (result) - 1] != '?')
    result[strlen (result)] = '.';

  *target = str_dup (result);
}

#include <memory>
void
do_ooc (CHAR_DATA * ch, char *argument, int cmd)
{
  int i = 0;
  char buf[MAX_STRING_LENGTH] = { '\0' };

  if (IS_SET (ch->flags, FLAG_GUEST) && IS_SET (ch->room->room_flags, OOC))
    {
      do_say (ch, argument, 0);
      return;
    }

  if (ch->room->nVirtual == AMPITHEATRE && IS_MORTAL (ch))
    {
      if (!get_obj_in_list_num (VNUM_SPEAKER_TOKEN, ch->right_hand) &&
	  !get_obj_in_list_num (VNUM_SPEAKER_TOKEN, ch->left_hand))
	{
	  send_to_char
	    ("You decide against making a commotion. PETITION to request to speak.\n",
	     ch);
	  return;
	}
    }

  for (i = 0; *(argument + i) == ' '; i++);

  if (!*(argument + i))
    send_to_char ("Surely you can think of more to say than that!\n", ch);

  else
    {
      std::string formatted = argument;
      formatted[0] = toupper (formatted[0]);
      char *p = 0;
      char *s = str_dup (formatted.c_str ());
      reformat_say_string (s, &p, 0);
      sprintf (buf, "$n says, out of character,\n   \"%s\"", p + i);
      act (buf, false, ch, 0, 0, TO_ROOM);
      sprintf (buf, "You say, out of character,\n   \"%s\"\n", p + i);
      send_to_char (buf, ch);
      mem_free (s);
      mem_free (p);
    }
}

void
do_pmote (CHAR_DATA * ch, char *argument, int cmd)
{
	char * result = NULL;
  char buf[MAX_STRING_LENGTH] = { '\0' };

  while (isspace (*argument))
    argument++;

  if (!*argument)
    {
      sprintf (buf, "Your current pmote: (#2%s#0)\n",
	       (ch->pmote_str) ? ch->pmote_str : "(#2none#0)");
      send_to_char (buf, ch);
    }

  else if (!strcmp (argument, "normal"))
    {
      sprintf (buf, "Your current pmote has been cleared.");
      act (buf, false, ch, 0, 0, TO_CHAR);
      clear_pmote (ch);
    }

	else if (IS_NPC(ch) && argument)
		{	
			ch->pmote_str = add_hash (argument);
		}
		
  else
    {
      if (ch && argument)
				{
				
					result = swap_xmote_target (ch, argument, 2);
					if(!result)
						return;
				}
     
         sprintf (buf, "You pmote: %s", result);

      ch->pmote_str = add_hash (result);

      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
    }
}

void
do_dmote (CHAR_DATA * ch, char *argument, int cmd)
{

  char buf[MAX_STRING_LENGTH] = { '\0' };

  while (isspace (*argument))
    argument++;

  if (!*argument)
    {
      sprintf (buf, "Your current dmote: (%s)\n",
	       (ch->dmote_str) ? ch->dmote_str : "(none)");
      send_to_char (buf, ch);
    }

  else if (!strcmp (argument, "normal"))
    {
      sprintf (buf, "Your current dmote has been cleared.");
      act (buf, false, ch, 0, 0, TO_CHAR);
      clear_dmote (ch);
    }

  else
    {

      
	sprintf (buf, "You dmote: %s", argument);

      ch->dmote_str = add_hash (argument);

      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
    }
}

void
clear_dmote (CHAR_DATA * ch)
{
  if (ch->dmote_str)
    {
      mem_free (ch->dmote_str); // char*
      ch->dmote_str = NULL;
    }
}

void
do_omote (CHAR_DATA * ch, char *argument, int cmd)
{

  char buf[AVG_STRING_LENGTH * 4] = { '\0' };
  char arg1[MAX_STRING_LENGTH] = { '\0' };
  char * result = NULL;
  OBJ_DATA *obj = NULL;

  argument = one_argument (argument, arg1);

  if (!*arg1)
    {
      send_to_char ("What would you like to omote on?\n", ch);
      return;
    }

  if (!(obj = get_obj_in_list_vis (ch, arg1, ch->room->contents)))
    {
      send_to_char ("You don't see that here.\n", ch);
      return;
    }

  if (!CAN_WEAR (obj, ITEM_TAKE) && IS_MORTAL (ch))
    {
      send_to_char ("You can't omote on that.\n", ch);
      return;
    }

  if (IS_SET (ch->room->room_flags, OOC))
    {
      send_to_char ("You can't do this in an OOC area.\n", ch);
      return;
    }

  if (!*argument)
    {
      send_to_char ("What will you omote?\n", ch);
      return;
    }

	result = swap_xmote_target (ch, argument, 3);
  if (!result)
    return;

  if (strlen (result) >= AVG_STRING_LENGTH * 4)
    {
      send_to_char ("Your omote needs to be more succinct.\n", ch);
      return;
    }

  sprintf (buf, "%s%s%s",
	   result,
	   (result[strlen (result) - 1] != '.') ? "." : "",
	   (obj->short_description[0] == '#') ? "#0" : "");

  obj->omote_str = add_hash (buf);
  sprintf (buf, "You omote: %s %s", obj->short_description, obj->omote_str);

  if (obj->short_description[0] == '#')
    {
      buf[13] = toupper (buf[13]);
    }

  act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);

}


void
do_think (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *tch = NULL;
  char *p = '\0';
  char buf[MAX_STRING_LENGTH] = { '\0' };
  char buf1[MAX_STRING_LENGTH] = { '\0' };
  char buf2[MAX_STRING_LENGTH] = { '\0' };

  if (IS_SET (ch->room->room_flags, OOC) && IS_MORTAL (ch))
    {
      send_to_char ("This command has been disabled in OOC zones.\n", ch);
      return;
    }

  while (isspace (*argument))
    argument++;

  if (!*argument)
    {
      send_to_char ("What would you like to think?\n", ch);
      return;
    }

  if (IS_SET (ch->plr_flags, NOPETITION))
    {
      act ("Your ability to petition/think has been revoked by an admin.",
	   false, ch, 0, 0, TO_CHAR);
      return;
    }

  sprintf (buf, "You thought: %s", argument);
  sprintf (buf1, "%s thinks, \"%s\"", GET_NAME (ch), argument);
  reformat_say_string (argument, &p, 0);
  sprintf (buf2, "#6You hear #5%s#6 think,\n   \"%s\"#0\n", char_short (ch),
	   p);
  mem_free (p); // char*

  act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);

  /* Send thoughts to global Imm telepaths */
  for (tch = character_list; tch; tch = tch->next)
    {
      if (tch->deleted)
	continue;
      if (IS_MORTAL (tch))
	continue;
      if (tch == ch)
	continue;
      if (!IS_SET (tch->flags, FLAG_TELEPATH))
	continue;
      act (buf1, false, tch, 0, 0, TO_CHAR | _ACT_FORMAT);
    }

  /* Send thoughts to in-room PC telepaths */
  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (tch == ch)		/* You don't get an echo of your own throught */
	continue;
      if (!IS_MORTAL (ch) && !IS_NPC (ch))	/* Imm thinks are not overheard */
	continue;
      if (!IS_MORTAL (tch))	/* Imms get a different echo */
	continue;
      if (skill_use (tch, SKILL_TELEPATHY, ch->skills[SKILL_TELEPATHY] / 3)
	  || (IS_NPC (ch) && tch->skills[SKILL_TELEPATHY]))
	send_to_char (buf2, tch);
    }
}

void
personalize_string (CHAR_DATA * src, CHAR_DATA * tar, char *emote)
{
  char desc[MAX_STRING_LENGTH] = { '\0' };
  char output[MAX_STRING_LENGTH] = { '\0' };
  char buf[MAX_STRING_LENGTH] = { '\0' };
  CHAR_DATA *tch = NULL, *target = NULL;

  *output = '\0';

  while (*emote)
    {
      *desc = '\0';
      if (*emote == '#')
	{
	  emote++;
	  if (*emote == '5')
	    {
	      emote++;
	      while (*emote != '#')
		{
		  sprintf (desc + strlen (desc), "%c", *emote);
		  emote++;
		}
	      for (tch = tar->room->people; tch; tch = tch->next_in_room)
		if (strcasecmp (char_short (tch), desc) == STR_MATCH)
		  {
		    break;
		  }
	      emote++;
	      emote++;
	      if (*emote == '\'')
		{
		  strcat (desc, "\'");
		  emote++;
		  if (*emote == 's')
		    {
		      strcat (desc, "s");
		    }
		  else
		    {
		      emote--;
		    }
		}
	      else
		{
		  emote--;
		  emote--;
		}
	      if (!tch)
		continue;
	      if (tch == tar)
		{
		  sprintf (buf, "%c", desc[strlen (desc) - 1]);
		  if (desc[strlen (desc) - 1] == '\''
		      || desc[strlen (desc) - 2] == '\'')
		    {
		      strcat (output, "#5your#0");
		      emote--;
		    }
		  else
		    strcat (output, "#5you#0");
		  target = tch;
		  emote++;
		}
	      else
		{
		  sprintf (buf, "#5%s#0", char_short (tch));
		  strcat (output, buf);
		  emote--;
		  if (*emote == '\'')
		    emote--;
		}
	    }
	  else
	    sprintf (output + strlen (output), "#%c", *emote);
	}
      else
	sprintf (output + strlen (output), "%c", *emote);
      emote++;
    }
  if (target)
    {
      if (*output == '#')
	output[2] = toupper (output[2]);
      act (output, false, src, 0, target, TO_VICT | _ACT_FORMAT);
      magic_add_affect (target, MAGIC_SENT, -1, 0, 0, 0, 0);
    }
}

void
personalize_emote (CHAR_DATA * src, char *emote)
{
  char desc[MAX_STRING_LENGTH] = { '\0' };
  char copy[MAX_STRING_LENGTH] = { '\0' };
  CHAR_DATA *tch = NULL;

  sprintf (copy, "%s", emote);

  while (*emote)
    {
      *desc = '\0';
      if (*emote == '#')
	{
	  emote++;
	  if (*emote == '5')
	    {
	      emote++;
	      while (*emote != '#')
		{
		  sprintf (desc + strlen (desc), "%c", *emote);
		  emote++;
		}
	      tch = get_char_room_vis (src, desc);
	      for (tch = src->room->people; tch; tch = tch->next_in_room)
		if (strcasecmp (char_short (tch), desc) == STR_MATCH)
		  break;
	      if (!tch)
		continue;
	      if (!get_affect (tch, MAGIC_SENT))
		personalize_string (src, tch, copy);
	    }
	}
      emote++;
    }

  for (tch = src->room->people; tch; tch = tch->next_in_room)
    {
      if (tch == src)
	continue;
      if (get_affect (tch, MAGIC_SENT))
	continue;
      act (copy, true, tch, 0, 0, TO_CHAR | _ACT_FORMAT);
    }

  for (tch = src->room->people; tch; tch = tch->next_in_room)
    {
      if (get_affect (tch, MAGIC_SENT))
	affect_remove (tch, get_affect (tch, MAGIC_SENT));
    }
}

void
do_emote (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH] = { '\0' };
  char copy[MAX_STRING_LENGTH] = { '\0' };
  char *result = NULL;
  CHAR_DATA *tch = NULL;
  int index = 0;
  char *p = '\0';

  while (isspace (*argument))
    argument++;

  if (ch->room->nVirtual == AMPITHEATRE && IS_MORTAL (ch))
    {
      if (!get_obj_in_list_num (VNUM_SPEAKER_TOKEN, ch->right_hand) &&
	  !get_obj_in_list_num (VNUM_SPEAKER_TOKEN, ch->left_hand))
	{
	  send_to_char
	    ("You decide against making a commotion. PETITION to request to speak.\n",
	     ch);
	  return;
	}
    }

  if (strstr (argument, "\""))
    {
      send_to_char
	("Conveying speech via emote is considered code abuse here. Please use SAY instead.\n",
	 ch);
      return;
    }

  if (!*argument)
    send_to_char ("What would you like to emote?\n", ch);
  else
    {
      //p = copy;

      sprintf(copy, argument);
     
      /** Removed code and created swap_xmote_target function **/
      result = swap_xmote_target (ch, copy, 1);

      if (!result)
        return;

      sprintf (buf, "%s", result);  

      personalize_emote (ch, buf); //adjusts for "you" if needed
			
      if (!strcmp(result, buf))
        act (buf, false, ch, 0, 0, TO_ROOM | TO_CHAR | _ACT_FORMAT);
      else
	act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
 				
 
 /** Theatre Begin
       if (ch->in_room == StageRoom)
  		{
  		for (index = 0; index < 5; index = index +1)
  			{
			for (tch = vtor (AudienceRoom[index])->people; tch; tch = tch->next_in_room)
    			{
				act (buf, false, tch, 0, ch, TO_CHAR | _ACT_FORMAT);
      			}
      		}
      	}//if (ch->in_room == StageRoom)
  Theatre End **/

    }
}

void
reply_reset (CHAR_DATA * ch, CHAR_DATA * target, char *buf, int cmd)
{
  static int avoid_loop = 0;
  RESET_DATA *reset = NULL;
  char *argument = '\0';
  char keywords[MAX_STRING_LENGTH] = { '\0' };
  char reply[MAX_STRING_LENGTH] = { '\0' };
  char question[MAX_STRING_LENGTH] = { '\0' };
  char buf2[MAX_STRING_LENGTH] = { '\0' };

  if (avoid_loop)		/* Don't get into infinite loops between mobs */
    return;

  for (reset = target->mob->resets; reset; reset = reset->next)
    {

      if (reset->type != RESET_REPLY)
	continue;

      argument = one_argument (reset->command, keywords);

      while (isspace (*argument))
	argument++;

      strcpy (reply, argument);

      one_argument (buf, question);

      for (argument = one_argument (keywords, buf2);
	   *buf2; argument = one_argument (argument, buf2))
	{

	  if (strcasecmp (buf2, question) == STR_MATCH)
	    {
	      name_to_ident (ch, buf2);
	      sprintf (buf2 + strlen (buf2), " %s", reply);
	      avoid_loop = 1;

	      send_to_char ("\n", ch);

	      if (cmd == 4)
		do_whisper (target, buf2, cmd);
	      else
		do_say (target, buf2, cmd);

	      avoid_loop = 0;

	      return;
	    }
	}
    }
}

void
do_speak (CHAR_DATA * ch, char *argument, int cmd)
{
  int i = 0;
  char buf[MAX_INPUT_LENGTH] = { '\0' };

  struct lang_info
  {
    char lang[15];
    int skill;
  } lang_tab[] =
  {
    {
    "Taliska", SKILL_SPEAK_TALISKA},
    {
    "Haladin", SKILL_SPEAK_HALADIN},
    {
    "Avarin", SKILL_SPEAK_AVARIN},
    {
    "Nandorin", SKILL_SPEAK_NANDORIN},
    {
    "Druag", SKILL_SPEAK_DRUAG},
    {
    "Beast-Tongue", SKILL_SPEAK_BEAST_TONGUE},
    {
    "Valarin", SKILL_SPEAK_VALARIN},
    {
    "Trollish", SKILL_SPEAK_TROLLISH},
    {
    "Thrunon", SKILL_SPEAK_THRUNON},
    {
    "Sindarin", SKILL_SPEAK_SINDARIN},
    {
    "Quenya", SKILL_SPEAK_QUENYA},
    {
    "Khuzdul", SKILL_SPEAK_KHUZDUL},
    {
    "Orkish", SKILL_SPEAK_ORKISH},
    {
    "Black-Wise", SKILL_BLACK_WISE},
    {
    "Grey-Wise", SKILL_GREY_WISE},
    {
    "White-Wise", SKILL_WHITE_WISE},
    {
    "\0", 0}
  };

  argument = one_argument (argument, buf);

  for (i = 0; lang_tab[i].skill; i++)
    if (strcasecmp (buf, lang_tab[i].lang) == STR_MATCH)
      break;

  if (!lang_tab[i].skill)
    {
      send_to_char ("Possible choices are:\n"
		    "\n"
		    "       Sindarin, Taliska, Orkish, Quenya, Haladin,\n"
		    "       Khuzdul, Druag, Trollish, Beast-Tongue,\n"
		    "       Nandorin, Thrunon, Avarin or Valarin.\n\n"
		    "For spellcasters:\n\n"
		    "       Black-Wise, Grey-Wise, and White-Wise.\n", ch);
      return;
    }

  if (!real_skill (ch, lang_tab[i].skill)
      && !get_affect (ch, MAGIC_AFFECT_TONGUES))
    {
      sprintf (buf, "You are unfamiliar with %s.\n", CAP (lang_tab[i].lang));
      send_to_char (buf, ch);
      return;
    }

  if (ch->speaks == lang_tab[i].skill)
    {
      sprintf (buf, "You are already speaking %s.\n", CAP (lang_tab[i].lang));
      send_to_char (buf, ch);
      return;
    }

  ch->speaks = lang_tab[i].skill;

  sprintf (buf, "You begin speaking %s.\n", CAP (lang_tab[i].lang));
  send_to_char (buf, ch);
}

void
do_select_script (CHAR_DATA * ch, char *argument, int cmd)
{
  int i = 0;
  char buf[MAX_INPUT_LENGTH] = { '\0' };

  struct lang_info
  {
    char lang[30];
    int skill;
  } lang_tab[] =
  {
    {
    "sarati", SKILL_SCRIPT_SARATI},
    {
    "tengwar", SKILL_SCRIPT_TENGWAR},
    {
    "cirth", SKILL_SCRIPT_CIRTH},
    {
    "valarin", SKILL_SCRIPT_VALARIN_SCRIPT},
    {
    "\0", 0}
  };

  argument = one_argument (argument, buf);

  for (i = 0; lang_tab[i].skill; i++)
    if (strcasecmp (buf, lang_tab[i].lang) == STR_MATCH)
      break;

  if (!lang_tab[i].skill)
    {
      send_to_char ("Possible choices are:\n"
		    "\n"
		    "       Sarati, Tengwar, Cirth, or Valarin.\n", ch);
      return;
    }

  if (!real_skill (ch, lang_tab[i].skill)
      && !get_affect (ch, MAGIC_AFFECT_TONGUES))
    {
      sprintf (buf, "You are unfamiliar with %s.\n", CAP (lang_tab[i].lang));
      send_to_char (buf, ch);
      return;
    }

  if (ch->speaks == lang_tab[i].skill)
    {
      sprintf (buf, "You are already writing in %s.\n",
	       CAP (lang_tab[i].lang));
      send_to_char (buf, ch);
      return;
    }

  ch->writes = lang_tab[i].skill;

  sprintf (buf, "You will now write in %s.\n", CAP (lang_tab[i].lang));
  send_to_char (buf, ch);
}

void
do_mute (CHAR_DATA * ch, char *argument, int cmd)
{

  char buf[MAX_STRING_LENGTH] = { '\0' };
  AFFECTED_TYPE *af = NULL;

  /*
  if (!real_skill (ch, SKILL_EAVESDROP))
    {
      sprintf (buf,
	       "You don't have any skill to try and overhear conversations, so you are already muted by default.\n");
      send_to_char (buf, ch);
      return;
    }*/

  while (isspace (*argument))
    argument++;

  if (!*argument)
    {
      sprintf (buf, "You %s listening to others' conversations.\n",
	       get_affect (ch, MUTE_EAVESDROP) ? "aren't" : "are");
      send_to_char (buf, ch);
      return;
    }

  if (strcasecmp (argument, "on") == STR_MATCH)
    {

      if (!get_affect (ch, MUTE_EAVESDROP))
	{
	  af = (AFFECTED_TYPE *) alloc (sizeof (AFFECTED_TYPE), 13);

	  af->type = MUTE_EAVESDROP;
	  af->a.listening.duration = -1;
	  af->a.listening.on = 1;

	  affect_to_char (ch, af);
	}
      sprintf (buf, "You will now not listen to others' conversations.\n");
      send_to_char (buf, ch);
    }
  else if (strcasecmp (argument, "off") == STR_MATCH)
    {

      if (get_affect (ch, MUTE_EAVESDROP))
	{
	  remove_affect_type (ch, MUTE_EAVESDROP);
	}
      sprintf (buf, "You will now listen to others' conversations.\n");
      send_to_char (buf, ch);
    }
  else
    {
      sprintf (buf,
	       "You can change your mute status by 'mute on' or 'mute off'.  To see what your mute status is use 'mute'\n");
      send_to_char (buf, ch);
    }
}

#define VOICE_RESET "normal"	/* Users use this to unset their voices */

void
do_voice (CHAR_DATA * ch, char *argument, int cmd)
{

  char buf[MAX_STRING_LENGTH] = { '\0' };

  while (isspace (*argument))
    argument++;

  if (strchr (argument, '~'))
    {
      send_to_char
	("Sorry, but you can't use tildae when setting a voice string.\n",
	 ch);
      return;
    }

  if (!*argument)
    {
      if (ch->voice_str)
	{
	  sprintf (buf, "Your current voice string: (#2%s#0)\n",
		   ch->voice_str);
	}
      else
	sprintf (buf, "You do not currently have a voice string set.\n");
      send_to_char (buf, ch);
    }
  else
    {
      if (strcasecmp (argument, VOICE_RESET) == STR_MATCH)
	{
	  clear_voice (ch);
	  sprintf (buf, "Your voice string has been cleared.");
	}
      else
	{
	  sprintf (buf, "Your voice string has been set to: (#2%s#0)",
		   argument);
	  ch->voice_str = add_hash (argument);
	}

      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
    }
}

int
decipher_speaking (CHAR_DATA * ch, int skillnum, int skill)
{
  int check = 0;

  if (skill > 0 && skill <= 10)
    check = 70;
  else if (skill > 10 && skill < 20)
    check = 50;
  else if (skill >= 20 && skill < 35)
    check = 30;
  else if (skill >= 35 && skill < 50)
    check = 20;
  else if (skill >= 50)
    check = 5;

  skill_use (ch, skillnum, 0);

  if (ch->skills[skillnum] >= check)
    return 1;
  else
    return 0;
}

char *
accent_desc (CHAR_DATA * ch, int skill)
{
  if (skill < 10)
    return "with very crude enunciation";
  else if (skill >= 10 && skill < 20)
    return "with crude enunciation";
  else if (skill >= 20 && skill < 30)
    return "with awkward enunciation";
  else if (skill >= 30 && skill < 40)
    return "with slightly awkward enunciation";
  else if (skill >= 40 && skill < 50)
    return "with a very faintly awkward enunciation";
  else
    return "with a faint accent";
}

void
do_say (CHAR_DATA * ch, char *argument, int cmd)
{
  int talked_to_another = 0, i = 0, key_e = 0;
  int index = 0;
  int heard_something = 0;
  CHAR_DATA *tch = NULL;
  CHAR_DATA *target = NULL;
  OBJ_DATA *obj = NULL;
  AFFECTED_TYPE *tongues = NULL;
  AFFECTED_TYPE *af_table = NULL;
  bool deciphered = false, allocd = false;
  char key[MAX_STRING_LENGTH] = { '\0' };

  char buf[MAX_STRING_LENGTH] = { '\0' };
  char buf4[MAX_STRING_LENGTH] = { '\0' };
  char buf2[MAX_STRING_LENGTH] = { '\0' };
  char buf3[MAX_STRING_LENGTH] = { '\0' };
  char buf5[MAX_STRING_LENGTH] = { '\0' };
  char target_key[MAX_STRING_LENGTH] = { '\0' };
  char voice[MAX_STRING_LENGTH] = { '\0' };
  char argbuf[MAX_STRING_LENGTH] = { '\0' };
  char *utters[] = { "say", "sing", "tell", "murmur", "wouldbewhisper" };
  bool bIsWithGroup = false;


  if (ch->room->sector_type == SECT_UNDERWATER)
    {
      send_to_char ("You can't do that underwater!\n", ch);
      return;
    }
 
  if (ch->room->nVirtual == AMPITHEATRE && IS_MORTAL (ch))
    {
      if (!get_obj_in_list_num (VNUM_SPEAKER_TOKEN, ch->right_hand) &&
	  !get_obj_in_list_num (VNUM_SPEAKER_TOKEN, ch->left_hand))
		{
	  	send_to_char
	    ("You decide against speaking out of turn. PETITION to request to speak.\n",
	     ch);
	  	return;
		}
    }

/* We modify *argument, make sure we don't */
/*  have a problem with const arguments   */
  strcpy (argbuf, argument);	
  argument = argbuf;		

  while (isspace (*argument))
    argument++;

  if (!*argument)
    {
      send_to_char ("What would you like to say?\n", ch);
      return;
    }

  if (ch->speaks == SKILL_WHITE_WISE || ch->speaks == SKILL_GREY_WISE
      || ch->speaks == SKILL_BLACK_WISE)
    {
      if (!is_incantation (argument))
	{
	  send_to_char
	    ("The tongue of the Wise should not be used flippantly or without cause!\n",
	     ch);
	  return;
	}
    }

  if (cmd == 5) //group
    {
      cmd = 3;
      bIsWithGroup = true;
    }
  
  if (cmd == 2) //tell
    {
      *target_key = '\0';
      argument = one_argument (argument, target_key);
    }

  if (ch->voice_str && ch->voice_str[0])
    {
      strcpy (voice, ch->voice_str);
    }

  //Get the intro phrase and the message
  if (*argument == '(')
    {
      *voice = '\0';
      *buf = '\0';
      sprintf (buf, "%s", argument);
      i = 1;
      *buf2 = '\0';
      while (buf[i] != ')')
	{
	  if (buf[i] == '\0')
	    {
	      send_to_char ("What did you wish to say?\n", ch);
	      return;
	    }
	  if (buf[i] == '*')
	    {
	      i++;
	      while (isalpha (buf[i]))
		key[key_e++] = buf[i++];
	      key[key_e] = '\0';
	      key_e = 0;

	      if (!get_obj_in_list_vis (ch, key, ch->room->contents) &&
		  !get_obj_in_list_vis (ch, key, ch->right_hand) &&
		  !get_obj_in_list_vis (ch, key, ch->left_hand) &&
		  !get_obj_in_list_vis (ch, key, ch->equip))
		{
		  sprintf (buf, "I don't see %s here.\n", key);
		  send_to_char (buf, ch);
		  return;
		}

	      obj = get_obj_in_list_vis (ch, key, ch->right_hand);
	      if (!obj)
		obj = get_obj_in_list_vis (ch, key, ch->left_hand);
	      if (!obj)
		obj = get_obj_in_list_vis (ch, key, ch->room->contents);
	      if (!obj)
		obj = get_obj_in_list_vis (ch, key, ch->equip);
	      sprintf (buf2 + strlen (buf2), "#2%s#0", obj_short_desc (obj));
	      *key = '\0';
	      continue;
	    }
	  if (buf[i] == '~')
	    {
	      i++;
	      while (isalpha (buf[i]))
		key[key_e++] = buf[i++];
	      key[key_e] = '\0';
	      key_e = 0;

	      if (!get_char_room_vis (ch, key))
		{
		  sprintf (buf, "I don't see %s here.\n", key);
		  send_to_char (buf, ch);
		  return;
		}

	      sprintf (buf2 + strlen (buf2), "#5%s#0",
		       char_short (get_char_room_vis (ch, key)));
	      *key = '\0';
	      continue;
	    }
	  sprintf (buf2 + strlen (buf2), "%c", buf[i]);
	  i++;
	}
      strcpy (voice, buf2);
      while (*argument != ')')
	argument++;
      argument += 2;

      i = 0;
      *buf = '\0';
      if (cmd == 2 && *target_key)
	sprintf (buf, "%s %s", target_key, argument);
      else
	sprintf (buf, "%s", argument);
      *argument = '\0';
      argument = buf;
      if (!*argument)
	{
	  send_to_char ("What did you wish to say?\n", ch);
	  return;
	}
    }
  else if (cmd == 2 && *target_key)
    {
      sprintf (buf, "%s %s", target_key, argument);
      sprintf (argument, "%s", buf);
    }

  if (!*argument)
    {
      send_to_char ("What did you wish to say?\n", ch);
      return;
    }

  if (cmd == 2)
    {				/* Tell */

      argument = one_argument (argument, buf);

      reformat_say_string (argument, &argument, 0);

      if (!*argument)
	{
	  send_to_char ("What did you wish to tell?\n", ch);
	  return;
	}

      if (!(target = get_char_room_vis (ch, buf)))
	{
	  send_to_char ("Tell who?\n", ch);
	  return;
	}

      if (target == ch)
	{
	  send_to_char ("You want to tell yourself?\n", ch);
	  return;
	}

      while (isspace (*argument))
	argument++;
    }
  else
    reformat_say_string (argument, &argument, 0);

  if (!*argument)
    {
      if (cmd == 1)
	send_to_char ("What are the words to the song?\n", ch);
      else if (cmd == 2)
	send_to_char ("What would you like to tell?\n", ch);
      else
	send_to_char ("What would you like to say?\n", ch);
      return;
    }

  tongues = get_affect (ch, MAGIC_AFFECT_TONGUES);

  if (cmd == 3)
    {
      if ((af_table = get_affect (ch, MAGIC_SIT_TABLE)) != NULL)
	{
	  bIsWithGroup = false;
	}
    }
  if (!tongues && !real_skill (ch, ch->speaks))
    {
      send_to_char
	("You can't even make a guess at the language you want to speak.\n",
	 ch);
      return;
    }

  sprintf (buf4, argument);	/* The intended message, sent to the player. */
  sprintf (buf5, argument);
  sprintf (buf2, argument);

  if (cmd == 0)
    {
      if (buf4[strlen (buf4) - 1] == '?')
	{
	  utters[cmd] = str_dup ("ask");
	  allocd = true;
	}
      else if (buf4[strlen (buf4) - 1] == '!')
	{
	  utters[cmd] = str_dup ("exclaim");
	  allocd = true;
	}
    }
  else if (cmd == 2)
    {
      if (buf4[strlen (buf4) - 1] == '?')
	{
	  utters[cmd] = str_dup ("ask");
	  allocd = true;
	}
      else if (buf4[strlen (buf4) - 1] == '!')
	{
	  utters[cmd] = str_dup ("emphatically tell");
	  allocd = true;
	}
    }

  skill_use (ch, ch->speaks, 0);

// Now decided what the target person hears
  for (tch = vtor (ch->in_room)->people; tch; tch = tch->next_in_room)
    {

      if (tch == ch)		/* Don't say it to ourselves */
	continue;

      if (!tch->desc)	/* NPC don't hear anything */
	continue;

      if ((af_table && !is_at_table (tch, af_table->a.table.obj))
	  || (bIsWithGroup
	      && (!are_grouped (ch, tch)
		  || get_affect (tch, MAGIC_SIT_TABLE))))
	{

	  /* If the guy is muting, punt */
	  if (get_affect (tch, MUTE_EAVESDROP))
	    continue;

	  sprintf (buf2, argument);

	  heard_something = whisper_it (tch, buf2, buf2);
         
          //if(!heard_something)
	  //  continue;

	  if ((tch->skills[ch->speaks]
	       && decipher_speaking (tch, ch->speaks, ch->skills[ch->speaks]) && heard_something)
	      || IS_SET (ch->room->room_flags, OOC))
	    {
	      if (!IS_SET (ch->room->room_flags, OOC))
		{
		  sprintf (buf, "You overhear $N say in %s,",
			   skills[ch->speaks]);
		  if (tch->skills[ch->speaks] >= 50
		      && ch->skills[ch->speaks] < 50)
		    sprintf (buf + strlen (buf), " %s,",
			     accent_desc (ch, ch->skills[ch->speaks]));
		}
	      else
		{
		  sprintf (buf, "You overhear $N say,");
		}
	      if (voice && *voice)
		sprintf (buf + strlen (buf), " %s,", voice);
	      deciphered = true;
	    }
	  else
	    {
	      if (tch->skills[ch->speaks])
		sprintf (buf, "You overhear $N say something in %s,",
			 skills[ch->speaks]);
	      else
		sprintf (buf, "You overhear $N say something,");
	      if (tch->skills[ch->speaks] >= 50
		  && ch->skills[ch->speaks] < 50)
		sprintf (buf + strlen (buf), " %s,",
			 accent_desc (ch, ch->skills[ch->speaks]));
	      if (voice && *voice)
		sprintf (buf + strlen (buf), " %s,", voice);


              if(heard_something)
	        sprintf (buf + strlen (buf),
		       " but you are unable to decipher %s words.", HSHR (ch));
              else
 	        sprintf (buf + strlen (buf), 
                       " but you are unable to make out %s words.", HSHR (ch));

	      deciphered = false;
	    }
	  act (buf, false, tch, 0, ch, TO_CHAR | _ACT_FORMAT);

	  if (tch->desc && deciphered)
	    {
	      *buf2 = toupper (*buf2);
	      sprintf (buf, "   \"%s\"\n", buf2);
	      send_to_char (buf, tch);
	    }

	  continue;
	}

      if (tch->desc)
	talked_to_another = 1;

      if (GET_TRUST (tch) && !IS_NPC (tch) && GET_FLAG (tch, FLAG_SEE_NAME))
	sprintf (buf3, " (%s)", GET_NAME (ch));
      else
	*buf3 = '\0';

      if (cmd == 0 || cmd == 1)
	{
	  if (!IS_SET (ch->room->room_flags, OOC)
	      && decipher_speaking (tch, ch->speaks, ch->skills[ch->speaks]))
	    {
	      sprintf (buf, "$N%s %ss in %s,", buf3, utters[cmd],
		       (tch->skills[ch->speaks]
			|| tongues) ? skills[ch->
					     speaks] : "an unknown tongue");
	      if (tch->skills[ch->speaks] >= 50
		  && ch->skills[ch->speaks] < 50)
		sprintf (buf + strlen (buf), " %s,",
			 accent_desc (ch, ch->skills[ch->speaks]));
	      deciphered = true;
	      if (voice && *voice)
		sprintf (buf + strlen (buf), " %s,", voice);
	    }
	  else if (!IS_SET (ch->room->room_flags, OOC))
	    {
	      sprintf (buf, "$N%s %ss something in %s,", buf3, utters[cmd],
		       (tch->skills[ch->speaks]
			|| tongues) ? skills[ch->
					     speaks] : "an unknown tongue");
	      if (voice && *voice)
		sprintf (buf + strlen (buf), " %s,", voice);
	      sprintf (buf + strlen (buf),
		       " but you are unable to decipher %s words.",
		       HSHR (ch));
	      deciphered = false;
	    }
	  else if (IS_SET (ch->room->room_flags, OOC))
	    {
	      sprintf (buf, "$N%s %ss,", buf3, utters[cmd]);
	      if (voice && *voice)
		sprintf (buf + strlen (buf), " %s,", voice);
	      deciphered = true;
	    }
	}

      else if (cmd == 2)
	{
	  if (tch == target)
	    {
	      if (!IS_SET (ch->room->room_flags, OOC)
		  && decipher_speaking (tch, ch->speaks,
					ch->skills[ch->speaks]))
		{
		  sprintf (buf, "$N%s %ss you in %s,", buf3,
			   utters[cmd], (tch->skills[ch->speaks] || tongues) ?
			   skills[ch->speaks] : "an unknown tongue");
		  deciphered = true;
		  if (tch->skills[ch->speaks] >= 50
		      && ch->skills[ch->speaks] < 50)
		    sprintf (buf + strlen (buf), " %s,",
			     accent_desc (ch, ch->skills[ch->speaks]));
		  if (voice && *voice)
		    sprintf (buf + strlen (buf), " %s,", voice);
		}
	      else if (!IS_SET (ch->room->room_flags, OOC))
		{
		  sprintf (buf, "$N%s %ss you something in %s,", buf3,
			   utters[cmd], (tch->skills[ch->speaks] || tongues) ?
			   skills[ch->speaks] : "an unknown tongue");
		  if (voice && *voice)
		    sprintf (buf + strlen (buf), " %s,", voice);
		  sprintf (buf + strlen (buf),
			   " but you are unable to decipher %s words.",
			   HSHR (ch));
		  deciphered = false;
		}
	      else
		{
		  sprintf (buf, "$N%s %ss you,", buf3, utters[cmd]);
		  deciphered = true;
		}
	    }
	  else
	    {
	      if (!IS_SET (ch->room->room_flags, OOC)
		  && decipher_speaking (tch, ch->speaks,
					ch->skills[ch->speaks]))
		{
		  sprintf (buf, "$N%s %ss %s in %s,", buf3,
			   utters[cmd], char_short (target),
			   (tch->skills[ch->speaks]
			    || tongues) ? skills[ch->
						 speaks] :
			   "an unknown tongue");
		  deciphered = true;
		  if (tch->skills[ch->speaks] >= 50
		      && ch->skills[ch->speaks] < 50)
		    sprintf (buf + strlen (buf), " %s,",
			     accent_desc (ch, ch->skills[ch->speaks]));
		  if (voice && *voice)
		    sprintf (buf + strlen (buf), " %s,", voice);
		}
	      else if (!IS_SET (ch->room->room_flags, OOC))
		{
		  sprintf (buf, "$N%s %ss %s something in %s,", buf3,
			   utters[cmd], char_short (target),
			   (tch->skills[ch->speaks] || tongues) ?
			   skills[ch->speaks] : "an unknown tongue");
		  if (voice && *voice)
		    sprintf (buf + strlen (buf), " %s,", voice);
		  sprintf (buf + strlen (buf),
			   " but you are unable to decipher %s words.",
			   HSHR (ch));
		  deciphered = false;
		}
	      else
		{
		  sprintf (buf, "$N%s %ss %s,", buf3, utters[cmd],
			   char_short (target));
		  if (voice && *voice)
		    sprintf (buf + strlen (buf), " %s,", voice);
		  deciphered = true;
		}
	    }
	}

      else if (cmd == 3)
	{
	  if (!IS_SET (ch->room->room_flags, OOC)
	      && decipher_speaking (tch, ch->speaks, ch->skills[ch->speaks]))
	    {
	      sprintf (buf, "$N%s %ss in %s,", buf3,
		       utters[cmd],
		       (tch->skills[ch->speaks] || tongues) ?
		       skills[ch->speaks] : "an unknown tongue");
	      deciphered = true;
	      if (tch->skills[ch->speaks] >= 50
		  && ch->skills[ch->speaks] < 50)
		sprintf (buf + strlen (buf), " %s,",
			 accent_desc (ch, ch->skills[ch->speaks]));
	      if (voice && *voice)
		sprintf (buf + strlen (buf), " %s,", voice);
	    }
	  else if (!IS_SET (ch->room->room_flags, OOC))
	    {
	      sprintf (buf, "$N%s %ss something in %s,", buf3,
		       utters[cmd],
		       (tch->skills[ch->speaks] || tongues) ?
		       skills[ch->speaks] : "an unknown tongue");
	      if (voice && *voice)
		sprintf (buf + strlen (buf), " %s,", voice);
	      sprintf (buf + strlen (buf),
		       " but you are unable to decipher %s words.",
		       HSHR (ch));
	      deciphered = false;
	    }
	  else
	    {
	      sprintf (buf, "$N%s %ss,", buf3, utters[cmd]);
	      deciphered = true;
	    }
	}

      act (buf, false, tch, 0, ch, TO_CHAR | _ACT_FORMAT);

      if (tch->desc && deciphered)
	{
	  *buf4 = toupper (*buf4);
	  sprintf (buf, "   \"%s\"\n", buf4);
	  send_to_char (buf, tch);
	}

      sprintf (argument, buf5);
      deciphered = false;
    }

  *buf4 = toupper (*buf4);

  if (cmd == 2)
    {
      if (voice && *voice)
	{
	  if (!IS_SET (ch->room->room_flags, OOC))
	    sprintf (buf, "You %s #5%s#0 in %s, %s,\n   \"%s\"\n",
		     utters[cmd], char_short (target), skills[ch->speaks],
		     voice, buf4);
	  else
	    sprintf (buf, "You %s #5%s#0, %s,\n   \"%s\"\n", utters[cmd],
		     char_short (target), voice, buf4);
	}
      else
	{
	  if (!IS_SET (ch->room->room_flags, OOC))
	    sprintf (buf, "You %s #5%s#0 in %s,\n   \"%s\"\n",
		     utters[cmd], char_short (target), skills[ch->speaks],
		     buf4);
	  else
	    sprintf (buf, "You %s #5%s#0,\n   \"%s\"\n", utters[cmd],
		     char_short (target), buf4);
	}
    }
  else
    {
      if (voice && *voice)
	{
	  if (!IS_SET (ch->room->room_flags, OOC))
	    sprintf (buf, "You %s in %s, %s,\n   \"%s\"\n",
		     utters[cmd], skills[ch->speaks], voice, buf4);
	  else
	    sprintf (buf, "You %s, %s,\n   \"%s\"\n", utters[cmd], voice,
		     buf4);
	}
      else
	{
	  if (!IS_SET (ch->room->room_flags, OOC))
	    sprintf (buf, "You %s in %s,\n   \"%s\"\n", utters[cmd],
		     skills[ch->speaks], buf4);
	  else
	    sprintf (buf, "You %s,\n   \"%s\"\n", utters[cmd], buf4);
	}
    }

  send_to_char (buf, ch);

/** For Theatre and Audience room only **/
  if (ch->in_room == StageRoom)
  {
  for (index = 0; index < 5; index = index +1)
  	{
	for (tch = vtor (AudienceRoom[index])->people; tch; tch = tch->next_in_room)
    {
		if (!tch->desc)	/* NPC don't hear anything */
			continue;

		if (tch->desc)
			talked_to_another = 1;

		if (GET_TRUST (tch) &&
			!IS_NPC (tch) &&
			GET_FLAG (tch, FLAG_SEE_NAME))
			sprintf (buf3, " (%s)", GET_NAME (ch));
		else
			*buf3 = '\0';

		if (cmd == 0 || cmd == 1)
			{
			if (!IS_SET (ch->room->room_flags, OOC) &&
				decipher_speaking (tch, ch->speaks, ch->skills[ch->speaks]))
				{
				sprintf (buf,
								"$N%s %ss in %s,",
								buf3,
								utters[cmd],
								(tch->skills[ch->speaks] || tongues) ? skills[ch->speaks] : "an unknown tongue");
			
				if (tch->skills[ch->speaks] >= 50 &&
						ch->skills[ch->speaks] < 50)
					{
					sprintf (buf + strlen (buf),
									" %s,",
									accent_desc (ch, ch->skills[ch->speaks]));
					}
				deciphered = true;
				if (voice && *voice)
					{
					sprintf (buf + strlen (buf),
									" %s,",
									voice);
					}
				}

			else if (!IS_SET (ch->room->room_flags, OOC))
				{
				sprintf (buf,
								"$N%s %ss something in %s,",
								buf3,
								utters[cmd],
								(tch->skills[ch->speaks] || tongues) ? skills[ch->
speaks] : "an unknown tongue");

				if (voice && *voice)
					{
					sprintf (buf + strlen (buf),
									" %s,",
									voice);
					}
			
				sprintf (buf + strlen (buf),
								" but you are unable to decipher %s words.",
								HSHR (ch));
				deciphered = false;
				}
			}
		
		else
			continue;
				
		act (buf, false, tch, 0, ch, TO_CHAR | _ACT_FORMAT);
		sprintf (buf3, "   \"%s\"\n", buf4);
		send_to_char(buf3, tch);
		} //for (tch = vtor (AudienceRoom[index])->people
	  }//for (index = 1; index; index = index +1)
	}//if (ch->in_room = StageRoom)
/** end theatre/audience room **/

  trigger (ch, argument, TRIG_SAY);

  if (cmd == 2 && IS_NPC (target))
    reply_reset (ch, target, argument, cmd);

  if (cmd == 0)
    {
      if (allocd)
	mem_free (utters[cmd]); // char[]
    }

  if (ch->speaks == SKILL_BLACK_WISE || ch->speaks == SKILL_GREY_WISE
      || ch->speaks == SKILL_WHITE_WISE)
    magic_incantation (ch, argument);

  mem_free (argument); // char * ??? <- why freeing this here???
}

void
do_sing (CHAR_DATA * ch, char *argument, int cmd)
{
  do_say (ch, argument, 1);	/* 1 = sing */
}

void
do_ichat (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf1[MAX_STRING_LENGTH] = { '\0' };
  char *p = '\0';
  DESCRIPTOR_DATA *i = NULL;

  if (!GET_TRUST (ch) && !IS_SET (ch->flags, FLAG_ISADMIN))
    {
      send_to_char ("Eh?\n", ch);
      return;
    }

  for (; *argument == ' '; argument++);

  if (!(*argument))
    {
      send_to_char ("What message would you like to send?\n", ch);
      return;
    }
  else
    {
      /// Use the admin's wiznet flag (ignore the NPC's)
      bool ch_wiznet_set = 
	(IS_NPC(ch) && ch->desc->original)
	? GET_FLAG (ch->desc->original, FLAG_WIZNET)
	: GET_FLAG (ch, FLAG_WIZNET);
      
      if (!ch_wiznet_set)
	{
	  send_to_char
	    ("You are not currently tuned into the wiznet. "
	     "Type SET WIZNET to change this.\n", ch);
	  return;
	}
      
      if (IS_NPC (ch) && ch->desc->original)
	{
	  sprintf (buf1, "#1[Wiznet: %s (%s)]#0 %s\n",
		   GET_NAME (ch->desc->original), 
		   GET_NAME (ch), CAP (argument));
	}
      else
	{
	  sprintf (buf1, "#1[Wiznet: %s]#0 %s\n", 
		   GET_NAME (ch), CAP (argument));
	}
      
      reformat_string (buf1, &p);
      p[0] = toupper (p[0]);
      
      for (i = descriptor_list; i; i = i->next)
	{
	  if (i->character
	      && !i->connected
	      && (GET_TRUST (i->character)
		  || IS_SET (i->character->flags, FLAG_ISADMIN)))
	    {
	      *s_buf = '\0';
	      
	      bool tch_wiznet_set = 
		(i->original)
		? GET_FLAG (i->original, FLAG_WIZNET)
		: GET_FLAG (i->character, FLAG_WIZNET);
	      
	      if (IS_SET (i->character->act, PLR_QUIET))
		{
		  sprintf (s_buf, "#2[%s is editing.]#0\n",
			   GET_NAME (i->character));
		}
	      else if (!tch_wiznet_set)
		{
		  if (!IS_MORTAL (i->character))
		    {
		      CHAR_DATA *tch = (i->original)
			? (i->original)
			: (i->character);
		      sprintf 
			(s_buf, "#2[%s is not listening to the wiznet.]#0\n",
			 GET_NAME (tch));
		    }
		}
	      else
		{
		  send_to_char (p, i->character);
		}
	      
	      if (*s_buf)
		{
		  send_to_char (s_buf, ch);
		}
	    }
	}
      mem_free (p); // char*
    }
}

void
do_fivenet (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf1[MAX_STRING_LENGTH] = { '\0' };
  char *p = '\0';
  DESCRIPTOR_DATA *i = NULL;

  for (; *argument == ' '; argument++);

  if (!(*argument))
    {
      send_to_char ("What message would you like to send?\n", ch);
      return;
    }
  else
    {
      if (!GET_FLAG (ch, FLAG_WIZNET) && !IS_NPC (ch))
	{
	  send_to_char
	    ("You are not currently tuned into the wiznet. Type SET WIZNET to change this.\n",
	     ch);
	  return;
	}

      if (IS_NPC (ch) && ch->desc->original)
	sprintf (buf1, "#C[Fivenet: %s (%s)]#0 %s\n",
		 GET_NAME (ch->desc->original), GET_NAME (ch), argument);
      else
	sprintf (buf1, "#C[Fivenet: %s]#0 %s\n", GET_NAME (ch),
		 CAP (argument));

      reformat_string (buf1, &p);

      for (i = descriptor_list; i; i = i->next)
	if (i->character && GET_TRUST (i->character) >= 5)
	  {
	    if (!i->connected)
	      {
		*s_buf = '\0';
		if (!IS_SET (i->character->act, PLR_QUIET)
		    && (GET_FLAG (i->character, FLAG_WIZNET) || i->original))
		  {
		    send_to_char (p, i->character);
		  }
		else
		  {
		    if (IS_SET (i->character->act, PLR_QUIET))
		      sprintf (s_buf, "#2[%s is editing.]#0\n",
			       GET_NAME (i->character));
		    else if (!GET_FLAG (i->character, FLAG_WIZNET))
		      sprintf (s_buf,
			       "#2[%s is not listening to the wiznet.]#0\n",
			       GET_NAME (i->character));
		    send_to_char (s_buf, ch);
		  }
	      }
	  }
      mem_free (p); // char*
    }

}

void
do_tell (CHAR_DATA * ch, char *argument, int cmd)
{
  do_say (ch, argument, 2);
}

void
do_immtell (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *vict = NULL;
  DESCRIPTOR_DATA *d = NULL;
  char *p = '\0';
  char name[MAX_STRING_LENGTH] = { '\0' };
  char message[MAX_STRING_LENGTH] = { '\0' };
  char buf[MAX_STRING_LENGTH] = { '\0' };

  half_chop (argument, name, message);

  if (!*name || !*message)
    send_to_char ("Who do you wish to tell what??\n", ch);

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

      sprintf (buf, "#2[From %s]#0 %s\n",
	       (IS_NPC (ch) ? ch->short_descr : GET_NAME (ch)),
	       CAP (message));
      reformat_string (buf, &p);
      send_to_char (p, vict);
      mem_free (p); // char*

      sprintf (buf, "#5[To %s]#0 %s\n", GET_NAME (vict), CAP (message));
      reformat_string (buf, &p);
      send_to_char (p, ch);
      mem_free (p); // char*
    }
}

int
whisper_it (CHAR_DATA * ch, char *source, char *target)
{
  int missed = 0;
  int got_one = 0;
  int bonus = 0;
  int base = 0;
  char *in = '\0';
  char *out = '\0';
  char buf[MAX_STRING_LENGTH] = { '\0' };
  OBJ_DATA *helm = NULL;

  if (ch->skills[SKILL_TELEPATHY])
    bonus = 20;

  base = ch->skills[SKILL_EAVESDROP];
  // if you're wearing a helm or a mask, you're not going to be able to hear very well.

  if (((helm = get_equip (ch, WEAR_HEAD)) && IS_SET (helm->obj_flags.extra_flags, ITEM_MASK))
       || ((helm = get_equip (ch, WEAR_FACE)) && IS_SET (helm->obj_flags.extra_flags, ITEM_MASK)))
    base = base / 2;


  if(base <= 0)
    base = 10;

  in = source;
  out = buf;
  *out = '\0';

  while (*in)
    {

      while (*in == ' ')
	{
	  in++;
	  *out = ' ';
	  out++;
	}

      *out = '\0';

      if (base + bonus < number (1, SKILL_CEILING))
	{
	  if (!missed)
	    {
	      strcat (out, " . . .");
	      out += strlen (out);
	    }

	  missed = 1;

	  while (*in && *in != ' ')
	    in++;
	}

      else
	{
	  while (*in && *in != ' ')
	    {
	      *out = *in;
	      out++;
	      in++;
	    }

	  got_one = 1;
	  missed = 0;
	}

      *out = '\0';
    }

  strcpy (target, buf);

  return got_one;
}

void
do_whisper (CHAR_DATA * ch, char *argument, int cmd)
{
  int i = 0, key_e = 0;
  int heard_something = 0;
  CHAR_DATA *vict = NULL;
  CHAR_DATA *tch = NULL;
  OBJ_DATA *obj = NULL;
  AFFECTED_TYPE *tongues = NULL;
  char name[MAX_STRING_LENGTH] = { '\0' };
  char message[MAX_STRING_LENGTH] = { '\0' };
  char buf[MAX_STRING_LENGTH] = { '\0' };
  char buf2[MAX_STRING_LENGTH] = { '\0' };
  char buf3[MAX_STRING_LENGTH] = { '\0' };
  char buf4[MAX_STRING_LENGTH] = { '\0' };
  char buf5[MAX_STRING_LENGTH] = { '\0' };
  char key[MAX_STRING_LENGTH] = { '\0' };
  char target_key[MAX_STRING_LENGTH] = { '\0' };
  char voice[MAX_STRING_LENGTH] = { '\0' };


  if (ch->room->sector_type == SECT_UNDERWATER)
    {
      send_to_char ("You can't do that underwater!\n", ch);
      return;
    }

  argument = one_argument (argument, name);

  if (ch->room->nVirtual == AMPITHEATRE && IS_MORTAL (ch))
    {
      if (!get_obj_in_list_num (VNUM_SPEAKER_TOKEN, ch->right_hand) &&
	  !get_obj_in_list_num (VNUM_SPEAKER_TOKEN, ch->left_hand))
	{
	  send_to_char
	    ("You decide against speaking out of turn. PETITION to request to speak.\n",
	     ch);
	  return;
	}
    }

  //half_chop (argument, name, message);

  if (*argument == '(')
    {
      *voice = '\0';
      *buf = '\0';
      sprintf (buf, "%s", argument);
      i = 1;
      *buf5 = '\0';
      while (buf[i] != ')')
	{
	  if (buf[i] == '\0')
	    {
	      send_to_char ("What did you wish to whisper?\n", ch);
	      return;
	    }
	  if (buf[i] == '*')
	    {
	      i++;
	      while (isalpha (buf[i]))
		key[key_e++] = buf[i++];
	      key[key_e] = '\0';
	      key_e = 0;

	      if (!get_obj_in_list_vis (ch, key, ch->room->contents) &&
		  !get_obj_in_list_vis (ch, key, ch->right_hand) &&
		  !get_obj_in_list_vis (ch, key, ch->left_hand) &&
		  !get_obj_in_list_vis (ch, key, ch->equip))
		{
		  sprintf (buf, "I don't see %s here.\n", key);
		  send_to_char (buf, ch);
		  return;
		}

	      obj = get_obj_in_list_vis (ch, key, ch->right_hand);
	      if (!obj)
		obj = get_obj_in_list_vis (ch, key, ch->left_hand);
	      if (!obj)
		obj = get_obj_in_list_vis (ch, key, ch->room->contents);
	      if (!obj)
		obj = get_obj_in_list_vis (ch, key, ch->equip);
	      sprintf (buf5 + strlen (buf5), "#2%s#0", obj_short_desc (obj));
	      *key = '\0';
	      continue;
	    }
	  if (buf[i] == '~')
	    {
	      i++;
	      while (isalpha (buf[i]))
		key[key_e++] = buf[i++];
	      key[key_e] = '\0';
	      key_e = 0;

	      if (!get_char_room_vis (ch, key))
		{
		  sprintf (buf, "I don't see %s here.\n", key);
		  send_to_char (buf, ch);
		  return;
		}

	      sprintf (buf5 + strlen (buf5), "#5%s#0",
		       char_short (get_char_room_vis (ch, key)));
	      *key = '\0';
	      continue;
	    }
	  sprintf (buf5 + strlen (buf5), "%c", buf[i]);
	  i++;
	}
      strcpy (voice, buf5);
      while (*argument != ')')
	argument++;
      argument += 2;

      sprintf(buf5, "%s,", buf5);

      i = 0;
      *buf = '\0';
      if (cmd == 2 && *target_key)
	sprintf (buf, "%s %s", target_key, argument);
      else
	sprintf (buf, "%s", argument);
      *argument = '\0';
      argument = buf;
      if (!*argument)
	{
	  send_to_char ("What did you wish to say?\n", ch);
	  return;
	}
    }

  *argument = toupper (*argument);

  if (!*name || !*argument)
    {
      send_to_char ("Who do you want to whisper to.. and what?\n", ch);
      return;
    }

  if (cmd != 83 && !(vict = get_char_room_vis (ch, name)) )
    {
      send_to_char ("No-one by that name here.\n", ch);
      return;
    }
  else if ( cmd == 83 && !(vict = get_char_room (name, ch->in_room)) ) 	// Whisper used by NPC's only for the AUCTION command.
  {
    return;
  }
  else if (vict == ch)
    {
      act ("$n whispers quietly to $mself.", false, ch, 0, 0, TO_ROOM);
      send_to_char ("You whisper to yourself.\n", ch);
      return;
    }

  tongues = get_affect (ch, MAGIC_AFFECT_TONGUES);

  if (!tongues && !real_skill (ch, ch->speaks)
      && !IS_SET (ch->room->room_flags, OOC))
    {
      send_to_char ("You don't know the language you want to "
		    "whisper\n", ch);
      return;
    }

  char *p = '\0';
  reformat_say_string (argument, &p, 0);

  if (!IS_SET (ch->room->room_flags, OOC))
    sprintf (buf, "You whisper to $N in %s, %s\n   \"%s\"",
	     skills[ch->speaks], buf5, p);
  else
    sprintf (buf, "You whisper to $N, %s \n   \"%s\"", buf5, p);

  act (buf, true, ch, 0, vict, TO_CHAR);

  sprintf (buf4, "%s", p);
  *buf4 = toupper (*buf4);
  sprintf (buf3, "%s", p);
  *buf3 = toupper (*buf3);

  skill_use (ch, ch->speaks, 0);

  for (tch = vtor (ch->in_room)->people; tch; tch = tch->next_in_room)
    {

    if (tch == ch)		/* Don't say it to ourselves */
		continue;

	if ( tch != vict && cmd == 83 ) /* Coded shopkeep whisper - skip to reduce spam */
		continue;
		
      sprintf (buf2, p);

      heard_something = 1;

    if (tch != vict)
	{
	  if (get_affect (tch, MUTE_EAVESDROP))
	    continue;
	  heard_something = whisper_it (tch, buf2, buf2);
	}
    if (!heard_something)
	{
          sprintf (buf, "$n whispers something to $3, %s but you can't quite make out the words.",
          buf5);

	  act(buf, true, ch, (OBJ_DATA *) vict, tch, TO_VICT | _ACT_FORMAT);
          continue;


	}

	if (tch == vict)
	{
	  if (!IS_SET (ch->room->room_flags, OOC)
	      && decipher_speaking (tch, ch->speaks, ch->skills[ch->speaks]))
	    {
	      sprintf (buf, "$3 whispers to you in %s, %s",
		       (tch->skills[ch->speaks] || tongues) ?
		       skills[ch->speaks] : "an unknown tongue", buf5);
	      if (tch->skills[ch->speaks] >= 50 && ch->skills[ch->speaks] < 50)
		sprintf (buf + strlen (buf), " %s,", accent_desc (ch, ch->skills[ch->speaks]));
	      act (buf, false, tch, (OBJ_DATA *) ch, 0,
		   TO_CHAR | _ACT_FORMAT);
	      sprintf (buf, "   \"%s\"", buf3);
	      act (buf, false, tch, 0, 0, TO_CHAR);
	    }
	  else if (!IS_SET (ch->room->room_flags, OOC))
	    {
	      sprintf (buf,
		       "$3 whispers something to you in %s, %s but you cannot decipher %s words.",
		       (tch->skills[ch->speaks]
			|| tongues) ? skills[ch->
					     speaks] : "an unknown tongue", buf5,
		       HSHR (ch));
	      act (buf, false, tch, (OBJ_DATA *) ch, 0,
		   TO_CHAR | _ACT_FORMAT);
	    }
	  else
	    {
	      sprintf (buf, "$3 whispers to you, %s \n   \"%s\"", buf5, buf4);
	      act (buf, false, tch, (OBJ_DATA *) ch, 0, TO_CHAR);
	    }
	  if (IS_NPC (vict))
	    reply_reset (ch, vict, buf2, 4);	/* 4 = whisper */
	}

      else
	{
	  if (!IS_SET (ch->room->room_flags, OOC)
	      && decipher_speaking (tch, ch->speaks, ch->skills[ch->speaks]))
	    {
	      sprintf (buf, "You overhear $3 whispering in %s to $N, %s ",
		       (tch->skills[ch->speaks] || tongues) ?
		       skills[ch->speaks] : "an unknown tongue", buf5);
	      if (tch->skills[ch->speaks] >= 50
		  && ch->skills[ch->speaks] < 50)
		sprintf (buf + strlen (buf), " %s,",
			 accent_desc (ch, ch->skills[ch->speaks]));
	      act (buf, false, tch, (OBJ_DATA *) ch, vict,
		   TO_CHAR | _ACT_FORMAT);
	      sprintf (buf, "   \"%s\"", buf2);
	      act (buf, false, tch, 0, 0, TO_CHAR);
	    }
	  else if (!IS_SET (ch->room->room_flags, OOC))
	    {
	      sprintf (buf,
		       "You overhear $3 whispering something in %s to $N, %s but you cannot decipher %s words.",
		       (tch->skills[ch->speaks]
			|| tongues) ? skills[ch->
					     speaks] : "an unknown tongue", buf5,
		       HSHR (ch));
	      act (buf, false, tch, (OBJ_DATA *) ch, vict,
		   TO_CHAR | _ACT_FORMAT);
	    }
	  else
	    {
	      sprintf (buf, "You overhear $3 whisper to $N, %s \n   \"%s\"",
		       buf5, buf2);
	      act (buf, false, tch, (OBJ_DATA *) ch, vict, TO_CHAR);
	    }
	}

      sprintf (p, buf3);
    }

  mem_free (p); // char*

  trigger (ch, argument, TRIG_WHISPER);
}


void
do_ask (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *vict = NULL;
  char name[MAX_STRING_LENGTH] = { '\0' };
  char message[MAX_STRING_LENGTH] = { '\0' };
  char buf[MAX_STRING_LENGTH] = { '\0' };

  half_chop (argument, name, message);

  if (!*name || !*message)
    send_to_char ("Who do you want to ask something.. and what??\n", ch);
  else if (!(vict = get_char_room_vis (ch, name)))
    send_to_char ("No-one by that name here.\n", ch);
  else if (vict == ch)
    {
      act ("$n quietly asks $mself a question.", false, ch, 0, 0, TO_ROOM);
      send_to_char ("You think about it for a while...\n", ch);
    }
  else
    {
      sprintf (buf, "$n asks you '%s'", message);
      act (buf, false, ch, 0, vict, TO_VICT);
      send_to_char ("Ok.\n", ch);
      act ("$n asks $N a question.", false, ch, 0, vict, TO_NOTVICT);
    }
}

void
do_talk (CHAR_DATA * ch, char *argument, int cmd)
{
  if (is_at_table (ch, NULL))
    {
      do_say (ch, argument, 3);
    }
  else if (is_with_group (ch))
    {
      do_say (ch, argument, 5);
    }
  else
    {
      do_say (ch, argument, 0);
    }
}

void
do_petition (CHAR_DATA * ch, char *argument, int cmd)
{
  CHAR_DATA *admin = NULL;
  char buf[MAX_STRING_LENGTH] = { '\0' };
  char *p = '\0';
  bool sent = false;
  char *date;
  time_t current_time;

  argument = one_argument (argument, buf);

  while (isspace (*argument))
    argument++;

  if ( !ch->desc )
      return;

  if (!*argument)
    {
      send_to_char ("Petition what message?\n", ch);
      return;
    }

  if (IS_SET (ch->plr_flags, NOPETITION))
    {
      act ("Your ability to petition/think has been revoked by an admin.",
	   false, ch, 0, 0, TO_CHAR);
      return;
    }


  if (ch->desc->acct
      && IS_SET (ch->desc->acct->flags, ACCOUNT_NOPETITION))
    {
      act ("Your ability to petition has been revoked by an admin.", false,
	   ch, 0, 0, TO_CHAR);
      return;
    }

  if (strcasecmp (buf, "all") == STR_MATCH)
    {

      sprintf (buf, "#6[Petition: %s]#0 %s\n",
	       IS_NPC (ch) ? ch->short_descr : GET_NAME (ch), CAP (argument));
      reformat_string (buf, &p);

      for (admin = character_list; admin; admin = admin->next)
	{

	  if (admin->deleted)
	    continue;

	  if (!GET_TRUST (admin))
	    continue;

	  if (!admin->desc)
	    continue;

	  send_to_char (p, admin);

          if (!admin->desc->idle)
            sent = true;
	}

      mem_free (p); // char*

      sprintf (buf, "You petitioned: %s\n", CAP (argument));
      reformat_string (buf, &p);
      send_to_char (p, ch);
      mem_free (p); // char*

      if (!get_affect (ch, MAGIC_PETITION_MESSAGE))
	{
	  sprintf (buf,
		   "\n#6Please understand that, as we are a staff of volunteers, we cannot be online\n"
		   "to respond to petitions 24 hours a day. If you find your petitions going largely\n"
		   "unanswered, email may be a more efficient recourse: "
		   STAFF_EMAIL ".#0\n");
	  send_to_char (buf, ch);
	  sprintf (buf,
		   "\n#6If there are no staff currently online, your petition will be logged for review\n"
		   "and responded to within a few days by an administrator via email if necessary.#0\n");
	  send_to_char (buf, ch);
	  sprintf (buf,
		   "\n#6If you have not already, please read #0HELP PETITION#6 for petitioning guidelines.\n#0");
	  send_to_char (buf, ch);
	  magic_add_affect (ch, MAGIC_PETITION_MESSAGE, 480, 0, 0, 0, 0);
	}

      if (!sent)
	{
	  current_time = time (0);
	  date = (char *) asctime (localtime (&current_time));
	  date[strlen (date) - 1] = '\0';
	  sprintf (buf, "From: %s [%d]\n\n", ch->tname, ch->in_room);
	  sprintf (buf + strlen (buf), "%s\n", argument);
	  add_message (1, "Petitions", -5, ch->tname, date, "Logged Petition",
		       "", buf, 0);
	  mem_free (date); // char*
	}

      return;
    }

  admin = load_pc (buf);

  if (!admin)
    {
      send_to_char ("Are you sure you didn't mistype the name?\n", ch);
      return;
    }

  if (admin == ch)
    {
      send_to_char ("Petition yourself? I see...\n", ch);
      unload_pc (admin);
      return;
    }

  if (!is_he_somewhere (admin) || !IS_SET (admin->flags, FLAG_AVAILABLE)
      || !admin->pc->level)
    {
      send_to_char ("Sorry, but that person is currently unavailable.\n", ch);
      unload_pc (admin);
      return;
    }

  if (IS_SET (admin->act, PLR_QUIET))
    {
      send_to_char ("That admin is editing.  Please try again in a minute.\n",
		    ch);
      unload_pc (admin);
      return;
    }

  sprintf (buf, "#5[Private Petition: %s]#0 %s\n",
	   IS_NPC (ch) ? ch->short_descr : GET_NAME (ch), CAP (argument));
  reformat_string (buf, &p);
  send_to_char (p, admin);
  mem_free (p); // char*
  sprintf (buf, "You petitioned %s: %s\n", GET_NAME (admin), CAP (argument));
  reformat_string (buf, &p);
  send_to_char (p, ch);
  mem_free (p); // char*

  unload_pc (admin);
}


void
do_shout (CHAR_DATA * ch, char *argument, int cmd)
{
  char key[MAX_STRING_LENGTH] = { '\0' };
  char target_key[MAX_STRING_LENGTH] = { '\0' };
  char voice[MAX_STRING_LENGTH] = { '\0' };
  int i = 0, key_e = 0;
  ROOM_DATA *room;
  CHAR_DATA *tch = NULL;
  OBJ_DATA *obj = NULL;
  AFFECTED_TYPE *tongues = NULL;
  int door = 0;
  char buf[MAX_STRING_LENGTH] = { '\0' };
  char buf2[MAX_STRING_LENGTH] = { '\0' };
  char buf3[MAX_STRING_LENGTH] = { '\0' };
  char buf4[MAX_STRING_LENGTH] = { '\0' };  
  char buf5[MAX_STRING_LENGTH] = { '\0' };
  char *rev_d[] = {
    "the south,",
    "the west,",
    "the north,",
    "the east,",
    "below,",
    "above,"
  };

  if (ch->room->sector_type == SECT_UNDERWATER)
    {
      send_to_char ("You can't do that underwater!\n", ch);
      return;
    }

  tongues = get_affect (ch, MAGIC_AFFECT_TONGUES);
  for (; isspace (*argument); argument++);

  if (ch->room->nVirtual == AMPITHEATRE && IS_MORTAL (ch))
    {
      if (!get_obj_in_list_num (VNUM_SPEAKER_TOKEN, ch->right_hand) &&
	  !get_obj_in_list_num (VNUM_SPEAKER_TOKEN, ch->left_hand))
	{
	  send_to_char
	    ("You decide against speaking out of turn. PETITION to request to speak.\n",
	     ch);
	  return;
	}
    }

  if (!tongues && !real_skill (ch, ch->speaks))
    {
      send_to_char ("You don't know that language!\n", ch);
      return;
    }

  if (!*argument)
    {
      send_to_char ("What would you like to shout?\n", ch);
      return;
    }

  if (*argument == '(')
    {
      *voice = '\0';
      *buf = '\0';
      sprintf (buf, "%s", argument);
      i = 1;
      *buf2 = '\0';
      while (buf[i] != ')')
	{
	  if (buf[i] == '\0')
	    {
	      send_to_char ("What did you wish to whisper?\n", ch);
	      return;
	    }
	  if (buf[i] == '*')
	    {
	      i++;
	      while (isalpha (buf[i]))
		key[key_e++] = buf[i++];
	      key[key_e] = '\0';
	      key_e = 0;

	      if (!get_obj_in_list_vis (ch, key, ch->room->contents) &&
		  !get_obj_in_list_vis (ch, key, ch->right_hand) &&
		  !get_obj_in_list_vis (ch, key, ch->left_hand) &&
		  !get_obj_in_list_vis (ch, key, ch->equip))
		{
		  sprintf (buf, "I don't see %s here.\n", key);
		  send_to_char (buf, ch);
		  return;
		}

	      obj = get_obj_in_list_vis (ch, key, ch->right_hand);
	      if (!obj)
		obj = get_obj_in_list_vis (ch, key, ch->left_hand);
	      if (!obj)
		obj = get_obj_in_list_vis (ch, key, ch->room->contents);
	      if (!obj)
		obj = get_obj_in_list_vis (ch, key, ch->equip);
	      sprintf (buf5 + strlen (buf5), "#2%s#0", obj_short_desc (obj));
	      *key = '\0';
	      continue;
	    }
	  if (buf[i] == '~')
	    {
	      i++;
	      while (isalpha (buf[i]))
		key[key_e++] = buf[i++];
	      key[key_e] = '\0';
	      key_e = 0;

	      if (!get_char_room_vis (ch, key))
		{
		  sprintf (buf, "I don't see %s here.\n", key);
		  send_to_char (buf, ch);
		  return;
		}

	      sprintf (buf5 + strlen (buf5), "#5%s#0",
		       char_short (get_char_room_vis (ch, key)));
	      *key = '\0';
	      continue;
	    }
	  sprintf (buf5 + strlen (buf5), "%c", buf[i]);
	  i++;
	}
      sprintf (voice, buf5);
      sprintf(buf5, ", %s", voice);
      while (*argument != ')')
	argument++;
      argument += 2;

      i = 0;
      *buf = '\0';
      if (cmd == 2 && *target_key)
	sprintf (buf, "%s %s", target_key, argument);
      else
	sprintf (buf, "%s", argument);
      *argument = '\0';
      argument = buf;
      if (!*argument)
	{
	  send_to_char ("What did you wish to say?\n", ch);
	  return;
	}
    }


  argument[0] = toupper (argument[0]);
  reformat_say_string (argument, &argument, 0);

  sprintf (buf4, argument);	/* The intended message, sent to the player. */

  for (tch = vtor (ch->in_room)->people; tch; tch = tch->next_in_room)
    {

      if (tch == ch)
	{
	  if (!IS_SET (ch->room->room_flags, OOC))
	    {
	      sprintf (buf, "You shout in %s%s", skills[ch->speaks], buf5);
	    }
	  else
	    {
	      sprintf (buf, "You shout%s,", buf5);
	    }
	  sprintf (buf2, "   \"%s\"", buf4);
	  act (buf, false, ch, 0, 0, TO_CHAR);
	  act (buf2, false, ch, 0, 0, TO_CHAR);
	  continue;
	}

      if (!IS_SET (ch->room->room_flags, OOC))
	sprintf (buf, "$N shouts in %s%s,", skills[ch->speaks], buf5);
      else
	sprintf (buf, "$N shouts%s,", buf5);

      if (!tch->skills[ch->speaks] && !tongues
	  && !IS_SET (ch->room->room_flags, OOC))
	{
	  sprintf (buf, "$N shouts%s, in an unknown tongue,", buf5);
	}

      if (tch->skills[ch->speaks] >= 50 && ch->skills[ch->speaks] < 50)
	sprintf (buf + strlen (buf), " %s,",
		 accent_desc (ch, ch->skills[ch->speaks]));

      if (!decipher_speaking (tch, ch->speaks, ch->skills[ch->speaks])
	  && !IS_SET (ch->room->room_flags, OOC))
	{
	  sprintf (buf + strlen (buf),
		   " something that you fail to decipher.");
	  act (buf, false, tch, 0, ch, TO_CHAR | _ACT_FORMAT);
	}
      else if (tch->desc)
	{
	  act (buf, false, tch, 0, ch, TO_CHAR | _ACT_FORMAT);
	  if (decipher_speaking (tch, ch->speaks, ch->skills[ch->speaks]))
	    {
	      sprintf (buf, "   \"%s\"\n", buf4);
	      send_to_char (buf, tch);
	    }
	}

      if (GET_TRUST (tch) && !IS_NPC (tch) && GET_FLAG (tch, FLAG_SEE_NAME))
	sprintf (buf3, " (%s)", GET_NAME (ch));
      else
	*buf3 = '\0';
      sprintf (buf2, argument);	/* Reset, for next listener. */
      continue;

    }
  /*
  if (ch->in_room == 5144 || ch->in_room == 5201 || ch->in_room == 5200)
    {
      arena__do_shout (ch, argument, cmd);
    }
  */
  for (door = 0; door <= 5; door++)
    {
      if (EXIT (ch, door) && (EXIT (ch, door)->to_room != -1))
	{
	  for (tch = vtor (EXIT (ch, door)->to_room)->people; tch;
	       tch = tch->next_in_room)
	    {

	      if (tch == ch)
		continue;

	      if (ch->race >= 11 && ch->race <= 13)
		{
		  if (!IS_SET (ch->room->room_flags, OOC))
		    {
		      if (!tch->skills[ch->speaks])
			sprintf (buf,
				 "You hear a bestial voice shout in an unknown tongue from %s",
				 rev_d[door]);
		      else
			sprintf (buf,
				 "You hear a bestial voice shout in %s from %s",
				 skills[ch->speaks], rev_d[door]);
		    }
		  else
		    sprintf (buf, "You hear a bestial voice shout from %s",
			     rev_d[door]);
		}
	      else if (ch->race >= 6 && ch->race <= 10)
		{
		  if (!IS_SET (ch->room->room_flags, OOC))
		    {
		      if (!tch->skills[ch->speaks])
			sprintf (buf,
				 "You hear a lyrical voice shout in an unknown tongue from %s",
				 rev_d[door]);
		      else
			sprintf (buf,
				 "You hear a lyrical voice shout in %s from %s",
				 skills[ch->speaks], rev_d[door]);
		    }
		  else
		    sprintf (buf, "You hear a lyrical voice shout from %s",
			     rev_d[door]);
		}
	      else if (GET_SEX (ch) == SEX_MALE)
		{
		  if (!IS_SET (ch->room->room_flags, OOC))
		    {
		      if (!tch->skills[ch->speaks])
			sprintf (buf,
				 "You hear a male voice shout in an unknown tongue from %s",
				 rev_d[door]);
		      else
			sprintf (buf,
				 "You hear a male voice shout in %s from %s",
				 skills[ch->speaks], rev_d[door]);
		    }
		  else
		    sprintf (buf, "You hear a male voice shout from %s",
			     rev_d[door]);
		}
	      else if (GET_SEX (ch) == SEX_FEMALE)
		{
		  if (!IS_SET (ch->room->room_flags, OOC))
		    {
		      if (!tch->skills[ch->speaks])
			sprintf (buf,
				 "You hear a female voice shout in an unknown tongue from %s",
				 rev_d[door]);
		      else
			sprintf (buf,
				 "You hear a female voice shout in %s from %s",
				 skills[ch->speaks], rev_d[door]);
		    }
		  else
		    sprintf (buf, "You hear a female voice shout from %s",
			     rev_d[door]);
		}
	      else if (GET_SEX (ch) == SEX_NEUTRAL)
		{
		  if (!IS_SET (ch->room->room_flags, OOC))
		    {
		      if (!tch->skills[ch->speaks])
			sprintf (buf,
				 "You hear a voice shout in an unknown tongue from %s",
				 rev_d[door]);
		      else
			sprintf (buf, "You hear a voice shout in %s from %s",
				 skills[ch->speaks], rev_d[door]);
		    }
		  else
		    sprintf (buf, "You hear a voice shout from %s",
			     rev_d[door]);
		}
	      if (tch->skills[ch->speaks] >= 50
		  && ch->skills[ch->speaks] < 50)
		sprintf (buf + strlen (buf), " %s,",
			 accent_desc (ch, ch->skills[ch->speaks]));
	      if (!decipher_speaking
		  (tch, ch->speaks, ch->skills[ch->speaks]))
		{
		  sprintf (buf + strlen (buf),
			   " though you cannot decipher the words.");
		  act (buf, false, tch, 0, ch, TO_CHAR | _ACT_FORMAT);
		}
	      else if (tch->desc)
		{
		  act (buf, false, tch, 0, ch, TO_CHAR | _ACT_FORMAT);
		  if (decipher_speaking
		      (tch, ch->speaks, ch->skills[ch->speaks]))
		    {
		      if (!IS_SET (ch->room->room_flags, OOC))
			sprintf (buf, "   \"%s\"\n", buf4);
		      else
			sprintf (buf, "   \"%s\"\n", buf4);
		      send_to_char (buf, tch);
		    }
		}

	      if (GET_TRUST (tch) && !IS_NPC (tch)
		  && GET_FLAG (tch, FLAG_SEE_NAME))
		sprintf (buf3, " (%s)", GET_NAME (ch));
	      else
		*buf3 = '\0';

	      sprintf (buf2, argument);	/* Reset. */

	    }
	}
    }

  for (room = full_room_list; room; room = room->lnext)
    {
      if (room->entrance == ch->in_room)
	{
	  for (tch = room->people; tch; tch = tch->next_in_room)
	    {

	      if (tch == ch)
		continue;

	      if (ch->race >= 11 && ch->race <= 13)
		{
     	          if (!IS_SET (ch->room->room_flags, OOC))
		  {
		    if (!tch->skills[ch->speaks])
		     sprintf (buf,
		    "You hear a bestial voice shout in an unknown tongue from the outside,");
		      else
			sprintf (buf,
				 "You hear a bestial voice shout in %s from the outside,",
				 skills[ch->speaks]);
		    }
		  else
		    sprintf (buf,
			     "You hear a bestial voice shout from the outside,");
		}
	      else if (ch->race >= 6 && ch->race <= 10)
		{
		  if (!IS_SET (ch->room->room_flags, OOC))
		    {
		      if (!tch->skills[ch->speaks])
			sprintf (buf,
				 "You hear a lyrical voice shout in an unknown tongue from the outside,");
		      else
			sprintf (buf,
				 "You hear a lyrica voice shout in %s from the outside,",
				 skills[ch->speaks]);
		    }
		  else
		    sprintf (buf,
			     "You hear a lyrical voice shout from the outside,");
		}
	      else if (GET_SEX (ch) == SEX_MALE)
		{
		  if (!IS_SET (ch->room->room_flags, OOC))
		    {
		      if (!tch->skills[ch->speaks])
			sprintf (buf,
				 "You hear a male voice shout in an unknown tongue from the outside,");
		      else
			sprintf (buf,
				 "You hear a male voice shout in %s from the outside,",
				 skills[ch->speaks]);
		    }
		  else
		    sprintf (buf,
			     "You hear a male voice shout from the outside,");
		}
	      else if (GET_SEX (ch) == SEX_FEMALE)
		{
		  if (!IS_SET (ch->room->room_flags, OOC))
		    {
		      if (!tch->skills[ch->speaks])
			sprintf (buf,
				 "You hear a female voice shout in an unknown tongue from the outside,");
		      else
			sprintf (buf,
				 "You hear a female voice shout in %s from the outside,",
				 skills[ch->speaks]);
		    }
		  else
		    sprintf (buf,
			     "You hear a female voice shout from the outside,");
		}
	      else if (GET_SEX (ch) == SEX_NEUTRAL)
		{
		  if (!IS_SET (ch->room->room_flags, OOC))
		    {
		      if (!tch->skills[ch->speaks])
			sprintf (buf,
				 "You hear a voice shout in an unknown tongue from the outside,");
		      else
			sprintf (buf,
				 "You hear a voice shout in %s from the outside,",
				 skills[ch->speaks]);
		    }
		  else
		    sprintf (buf, "You hear a voice shout from the outside,");
		}
	      if (tch->skills[ch->speaks] >= 50
		  && ch->skills[ch->speaks] < 50)
		sprintf (buf + strlen (buf), " %s,",
			 accent_desc (ch, ch->skills[ch->speaks]));
	      if (!decipher_speaking
		  (tch, ch->speaks, ch->skills[ch->speaks]))
		{
		  sprintf (buf + strlen (buf),
			   " though you cannot decipher the words.");
		  act (buf, false, tch, 0, ch, TO_CHAR | _ACT_FORMAT);
		}
	      else if (tch->desc)
		{
		  act (buf, false, tch, 0, ch, TO_CHAR | _ACT_FORMAT);
		  if (decipher_speaking
		      (tch, ch->speaks, ch->skills[ch->speaks]))
		    {
		      if (!IS_SET (ch->room->room_flags, OOC))
			sprintf (buf, "   \"%s\"\n", buf4);
		      else
			sprintf (buf, "   \"%s\"\n", buf4);
		      send_to_char (buf, tch);
		    }
		}

	      if (GET_TRUST (tch) && !IS_NPC (tch)
		  && GET_FLAG (tch, FLAG_SEE_NAME))
		sprintf (buf3, " (%s)", GET_NAME (ch));
	      else
		*buf3 = '\0';

	      sprintf (buf2, argument);	/* Reset. */

	    }
	}
    }

  mem_free (argument); // char* <- should we be freeing this???
}

int
add_to_list (ROOM_DATA ** list, ROOM_DATA * room, int *elements)
{
  int i = 0;

  for (i = 0; i < *elements; i++)
    if (list[i] == room)
      return 0;

  list[*elements] = room;

  (*elements)++;

  return 1;
}

void
get_room_list (int radius, ROOM_DATA * room, ROOM_DATA ** rooms,
	       int dists[], int *num_rooms)
{
  int room_set_top = 0;
  int room_set_bot = 0;
  int dir = 0;

  *num_rooms = 0;

  add_to_list (rooms, room, num_rooms);

  room_set_top = 0;
  room_set_bot = 0;

  while (radius--)
    {
      while (room_set_top <= room_set_bot)
	{

	  for (dir = 0; dir <= 5; dir++)
	    if (rooms[room_set_top]->dir_option[dir])
	      add_to_list (rooms, vtor (rooms[room_set_top]->
					dir_option[dir]->to_room), num_rooms);

	  room_set_top++;
	}

      room_set_bot = *num_rooms - 1;

      if (room_set_top >= *num_rooms)
	break;			/* Ran out of rooms */
    }
}

void
wolf_howl (CHAR_DATA * ch)
{
  int num_rooms = 0;
  int dists[220];
  int i = 0;
  int dir = 0;
  ROOM_DATA *rooms[220];
  CHAR_DATA *tch = NULL;
  char buf[MAX_STRING_LENGTH] = { '\0' };

  for (i = 0; i <= 219; i++)
    dists[i] = 0;
  for (i = 0; i <= 219; i++)
    rooms[i] = NULL;

  get_room_list (3, ch->room, rooms, dists, &num_rooms);

  act ("$n howls mournfully.", false, ch, 0, 0, TO_ROOM);
  act ("You howl mournfully.", false, ch, 0, 0, TO_CHAR);

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (!IS_NPC (tch))
	continue;
      if (tch->race != 17 && tch->race != 18 && tch->race != 19)
	continue;
      if (!ch->fighting)
	continue;
      if (tch->fighting || tch->delay)
	continue;
      set_fighting (ch, tch);
    }

  for (i = 1; i < num_rooms; i++)
    {

      if (rooms[i]->people)
	{
	  dir = track (rooms[i]->people, ch->room->nVirtual);

	  if (dir == -1)
	    continue;

	  sprintf (buf,
		   "There is a loud, mournful howl coming from the %s.\n",
		   dirs[dir]);

	  send_to_room (buf, rooms[i]->nVirtual);
	}

      for (tch = rooms[i]->people; tch; tch = tch->next_in_room)
	{

	  if (!IS_NPC (tch))
	    continue;

	  if (tch->race != 17 && tch->race != 18 && tch->race != 19)
	    continue;

	  if (tch->fighting || tch->delay || IS_SET (tch->act, ACT_SENTINEL))
	    continue;

	  act ("$n stands upright suddenly, cocking $s head.",
	       true, tch, 0, 0, TO_ROOM);

	  tch->delay = 2;
	  tch->delay_type = DEL_ALERT;
	  tch->delay_info1 = ch->in_room;
	  tch->delay_info2 = 8;
	  if (ch->fighting)
	    add_threat (tch, ch->fighting, 3);
	}
    }
}

void
spider_screech (CHAR_DATA * ch)
{
  int num_rooms = 0;
  int dists[220];
  int i = 0;
  int dir = 0;
  ROOM_DATA *rooms[220];
  CHAR_DATA *tch = NULL;
  char buf[MAX_STRING_LENGTH] = { '\0' };

  for (i = 0; i <= 219; i++)
    dists[i] = 0;
  for (i = 0; i <= 219; i++)
    rooms[i] = NULL;

  get_room_list (3, ch->room, rooms, dists, &num_rooms);

  act ("$n screeches loudly.", false, ch, 0, 0, TO_ROOM);
  act ("You screech loudly.", false, ch, 0, 0, TO_CHAR);

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (!IS_NPC (tch))
	continue;
      if (tch->race != 26)
	continue;
      if (!ch->fighting)
	continue;
      if (tch->fighting || tch->delay)
	continue;
      set_fighting (ch, tch);
    }

  for (i = 1; i < num_rooms; i++)
    {

      if (rooms[i]->people)
	{
	  dir = track (rooms[i]->people, ch->room->nVirtual);

	  if (dir == -1)
	    continue;

	  sprintf (buf,
		   "There is a loud, horrid screech coming from the %s.\n",
		   dirs[dir]);

	  send_to_room (buf, rooms[i]->nVirtual);
	}

      for (tch = rooms[i]->people; tch; tch = tch->next_in_room)
	{

	  if (!IS_NPC (tch))
	    continue;

	  if (tch->race != 26)
	    continue;

	  if (tch->fighting || tch->delay || IS_SET (tch->act, ACT_SENTINEL))
	    continue;

	  act ("$n turns suddenly, cocking $s head.",
	       true, tch, 0, 0, TO_ROOM);

	  tch->delay = 2;
	  tch->delay_type = DEL_ALERT;
	  tch->delay_info1 = ch->in_room;
	  tch->delay_info2 = 8;
	  if (ch->fighting)
	    add_threat (tch, ch->fighting, 3);
	}
    }
}

void
warg_howl (CHAR_DATA * ch)
{
  int num_rooms = 0;
  int dists[220];
  int i = 0;
  int dir = 0;
  ROOM_DATA *rooms[220];
  CHAR_DATA *tch = NULL;
  char buf[MAX_STRING_LENGTH] = { '\0' };

  for (i = 0; i <= 219; i++)
    dists[i] = 0;
  for (i = 0; i <= 219; i++)
    rooms[i] = NULL;

  get_room_list (3, ch->room, rooms, dists, &num_rooms);

  act ("$n gives a blood-curdling howl.", false, ch, 0, 0, TO_ROOM);
  act ("You give a blood-curdling howl.", false, ch, 0, 0, TO_CHAR);

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (!IS_NPC (tch))
	continue;
      if (tch->race != 17 && tch->race != 18 && tch->race != 19)
	continue;
      if (!ch->fighting)
	continue;
      if (tch->fighting || tch->delay)
	continue;
      set_fighting (ch, tch);
    }

  for (i = 1; i < num_rooms; i++)
    {

      if (rooms[i]->people)
	{
	  dir = track (rooms[i]->people, ch->room->nVirtual);

	  if (dir == -1)
	    continue;

	  sprintf (buf,
		   "There is a blood-curdling howl coming from the %s.\n",
		   dirs[dir]);

	  send_to_room (buf, rooms[i]->nVirtual);
	}

      for (tch = rooms[i]->people; tch; tch = tch->next_in_room)
	{

	  if (!IS_NPC (tch))
	    continue;

	  if (tch->race != 62)
	    continue;

	  if (tch->fighting || tch->delay || IS_SET (tch->act, ACT_SENTINEL))
	    continue;

	  act ("$n stands upright suddenly, and bares $s teeth.",
	       true, tch, 0, 0, TO_ROOM);

	  tch->delay = 2;
	  tch->delay_type = DEL_ALERT;
	  tch->delay_info1 = ch->in_room;
	  tch->delay_info2 = 8;
	  if (ch->fighting)
	    add_threat (tch, ch->fighting, 3);
	}
    }
}
void
insect_smell (CHAR_DATA * ch)
{
  int num_rooms = 0;
  int dists[220];
  int i = 0;
  int dir = 0;
  ROOM_DATA *rooms[220];
  CHAR_DATA *tch = NULL;
  char buf[MAX_STRING_LENGTH] = { '\0' };

  for (i = 0; i <= 219; i++)
    dists[i] = 0;
  for (i = 0; i <= 219; i++)
    rooms[i] = NULL;

  get_room_list (3, ch->room, rooms, dists, &num_rooms);

  act ("$n exudes a strong scent.", false, ch, 0, 0, TO_ROOM);
  act ("You exude a strong scent.", false, ch, 0, 0, TO_CHAR);

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (!IS_NPC (tch))
	continue;
      if (tch->race != 108)
	continue;
      if (!ch->fighting)
	continue;
      if (tch->fighting || tch->delay)
	continue;
      set_fighting (ch, tch);
    }

  for (i = 1; i < num_rooms; i++)
    {

      if (rooms[i]->people)
	{
	  dir = track (rooms[i]->people, ch->room->nVirtual);

	  if (dir == -1)
	    continue;

	  sprintf (buf,
		   "You catch a whiff of a strong smell coming from the %s.\n",
		   dirs[dir]);

	  send_to_room (buf, rooms[i]->nVirtual);
	}

      for (tch = rooms[i]->people; tch; tch = tch->next_in_room)
	{

	  if (!IS_NPC (tch))
	    continue;

	  if (tch->race != 108)
	    continue;

	  if (tch->fighting || tch->delay || IS_SET (tch->act, ACT_SENTINEL))
	    continue;

	  act ("$n shudders and turns towards the source of a drifting scent.",
	       true, tch, 0, 0, TO_ROOM);

	  tch->delay = 2;
	  tch->delay_type = DEL_ALERT;
	  tch->delay_info1 = ch->in_room;
	  tch->delay_info2 = 8;
	  if (ch->fighting)
	    add_threat (tch, ch->fighting, 3);
	}
    }
}

void
do_alert (CHAR_DATA * ch, char *argument, int cmd)
{
  int num_rooms = 0;
  int dists[220];
  int i = 0;
  int dir = 0;
  ROOM_DATA *rooms[220];
  CHAR_DATA *tch = NULL;
  char buf[MAX_STRING_LENGTH] = { '\0' };

  for (i = 0; i <= 219; i++)
    dists[i] = 0;
  for (i = 0; i <= 219; i++)
    rooms[i] = NULL;

  if (GET_POS (ch) < REST)
    return;

  if (ch->race == 17 || ch->race == 18 || ch->race == 19) // ? (ch->act & ACT_WILDLIFE)
    {
      wolf_howl (ch);
      return;
    }

  if (ch->race == 26) // ? (ch->act & ACT_WILDLIFE)
    {
      spider_screech (ch);
      return;
    }
    
    if (ch->race == 108) // ? (ch->act & ACT_WILDLIFE)
    {
      insect_smell (ch);
      return;
    }
  get_room_list (3, ch->room, rooms, dists, &num_rooms);

  act ("$n whistles very loudly.", false, ch, 0, 0, TO_ROOM);
  act ("You whistle very loudly.", false, ch, 0, 0, TO_CHAR);

  for (tch = ch->room->people; tch; tch = tch->next_in_room)
    {
      if (!IS_NPC (tch))
	continue;
      if (!ch->fighting)
	continue;
      if (tch->fighting || tch->delay)
	continue;
      if (!is_brother (ch, tch))
	continue;
      set_fighting (ch, tch);
    }

  for (i = 1; i < num_rooms; i++)
    {

      if (rooms[i]->people)
	{
	  dir = track (rooms[i]->people, ch->room->nVirtual);

	  if (dir == -1)
	    continue;

	  sprintf (buf, "There is a loud whistle coming from the %s.\n",
		   dirs[dir]);

	  send_to_room (buf, rooms[i]->nVirtual);
	}

      for (tch = rooms[i]->people; tch; tch = tch->next_in_room)
	{

	  if (!IS_NPC (tch))
	    continue;

	  if (tch->fighting || tch->delay || IS_SET (tch->act, ACT_SENTINEL))
	    continue;

	  if (!is_brother (ch, tch))
	    continue;

	  act ("$n glances up suddenly with a concerned look on $s face.",
	       true, tch, 0, 0, TO_ROOM);

	  tch->delay = 2;
	  tch->delay_type = DEL_ALERT;
	  tch->delay_info1 = ch->in_room;
	  tch->delay_info2 = 8;
	  if (ch->fighting)
	    add_threat (tch, ch->fighting, 3);
	}
    }
}

void
delayed_alert (CHAR_DATA * ch)
{
  int dir = 0;
  int save_speed = 0;
  int current_room = 0;
  ROOM_DATA *to_room = NULL;
  char buf[MAX_STRING_LENGTH] = { '\0' };

  dir = track (ch, ch->delay_info1);

  if (dir == -1)
    {
      send_to_char ("You can't figure out where the whistle came from.\n",
		    ch);
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      return;
    }

  current_room = ch->in_room;

  if (IS_HITCHEE (ch))
    return;

  if (IS_SUBDUEE (ch))
    return;

  if (!EXIT (ch, dir))
    return;

  if (!(to_room = vtor (EXIT (ch, dir)->to_room)))
    return;

  if (IS_SET (to_room->room_flags, NO_MOB))
    return;

  if (IS_MERCHANT (ch) && IS_SET (to_room->room_flags, NO_MERCHANT))
    return;

  if (ch->mob && ch->mob->access_flags &&
      !(ch->mob->access_flags & to_room->room_flags))
    return;

  if (IS_SET (ch->act, ACT_STAY_ZONE) && ch->room->zone != to_room->zone)
    return;

  save_speed = ch->speed;
  ch->speed = SPEED_RUN;

  sprintf (buf, "%s", dirs[dir]);

  command_interpreter (ch, buf);

  ch->speed = save_speed;

  ch->delay_info2--;

  if (current_room == ch->in_room || ch->delay_info2 <= 0)
    {
      send_to_char ("You can't locate the whistle.\n", ch);
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
      ch->delay_ch = 0;
      return;
    }

  if (ch->in_room != ch->delay_info1)
    ch->delay = 8;
  else
    {
      ch->delay = 0;
      ch->delay_type = 0;
      ch->delay_info1 = 0;
      ch->delay_info2 = 0;
    }
}

void
clear_voice (CHAR_DATA * ch)
{
  if (ch->voice_str)
    {
      mem_free (ch->voice_str); // char*
      ch->voice_str = NULL;
    }
}


/****************************************************************************
 *                                                            TRAVEL STRING *
 ****************************************************************************/

#define TRAVEL_RESET "normal"	/* keyword to reset user travel string */

/*                                                                          *
 * function: clear_travel               < e.g.> travel normal               *
 *                                                                          */
void
clear_travel (CHAR_DATA * ch)
{
  if (ch->travel_str)
    {
      mem_free (ch->travel_str); // char*
      ch->travel_str = NULL;
      send_to_char ("Your travel string has been cleared.\n", ch);
    }
}

/*                                                                          *
 * function: clear_travel               < e.g.> travel [ normal|<string> ]  *
 *                                                                          */
void
do_travel (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[MAX_STRING_LENGTH] = { '\0' };

  while (isspace (*argument))
    argument++;

  if (strchr (argument, '~'))
    {
      send_to_char
	("Sorry, but you can't use tildae when setting a travel string.\n",
	 ch);
      return;
    }

  if (!*argument)
    {
      if (ch->travel_str)
	{
	  sprintf (buf, "Your current travel string: (#2%s#0)\n",
		   ch->travel_str);
	}
      else
	{
	  sprintf (buf, "You do not currently have a travel string set.\n");
	}
      send_to_char (buf, ch);
    }
  else
    {
      if (strcasecmp (argument, TRAVEL_RESET) == STR_MATCH)
	{
	  clear_travel (ch);
	}
      else
	{
	  sprintf (buf, "Your travel string has been set to: (#2%s#0)",
		   argument);
	  ch->travel_str = add_hash (argument);
	}
      act (buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);
    }
}

bool evaluate_emote_string (CHAR_DATA * ch, std::string * first_person, std::string third_person, std::string argument)
{
	OBJ_DATA * object;
	CHAR_DATA * tch = NULL;
	int i = 1;
	std::string output_string = "", key_string = "", error_string = "";
	
	if (argument[0] == '(')
	{
		for (tch = ch->room->people; tch; tch = tch->next_in_room)
		{
			i = 1;
			output_string.clear();
			while (argument[i] != ')')
			{
				if (argument[i] == '\0')
				{
					send_to_char ("Incorrect usage of emote string - please see HELP EMOTE\n", ch);
					return false;
				}

				if (argument[i] == '*')
				{
					i++;
					key_string.clear();
					object = NULL;
					while (isalpha(argument[i]) || argument[i] == '-')
						key_string.push_back(argument[i++]);
					if (!(object = get_obj_in_list_vis(ch, key_string.c_str(), ch->right_hand)) &&
					    !(object = get_obj_in_list_vis(ch, key_string.c_str(), ch->left_hand)) &&
					    !(object = get_obj_in_list_vis(ch, key_string.c_str(), ch->room->contents)) &&
					    !(object = get_obj_in_list_vis(ch, key_string.c_str(), ch->equip)) )
					{
						error_string = "You cannot find an object with the keyword [#2";
						error_string.append(key_string);
						error_string.append("]#0\n");
						send_to_char (error_string.c_str(), ch);
						return false;
					}

					output_string.append("#2");
					output_string.append(obj_short_desc(object));
					output_string.append("#0");
					continue;
				}

				if (argument[i] == '~')
				{
					i++;
					key_string.clear();
					while (isalpha(argument[i]) || argument[i] == '-')
						key_string.push_back(argument[i++]);
					if (!get_char_room_vis(ch, key_string.c_str()))
					{
						error_string = "You cannot find a person with the keyword [#5";
						error_string.append(key_string);
						error_string.append("#0]\n");
						send_to_char (error_string.c_str(), ch);
						return false;
					}
					
					if (get_char_room_vis(ch, key_string.c_str()) == tch)
					{
						output_string.append("#5you#0");
					}
					else
					{
						output_string.append("#5");
						output_string.append(char_short(get_char_room_vis(ch, key_string.c_str())));
						output_string.append("#0");
					}
					continue;
				}

				output_string.push_back(argument[i++]);
			}
			
			if (tch == ch)
			{
				send_to_char (first_person->c_str(), ch);
			}
			else
			{
				send_to_char (third_person.c_str(), tch);
			}
			output_string.push_back('.');
			output_string.push_back('\n');
			send_to_char (", ", tch);
			send_to_char (output_string.c_str(), tch);
			continue;
		}
	}

	else
	{

		char non_const_first [MAX_STRING_LENGTH] = "";
		const_to_non_const_cstr(first_person->append(".").c_str(), non_const_first);
		char non_const_third [MAX_STRING_LENGTH] = "";
		const_to_non_const_cstr(third_person.append(".").c_str(), non_const_third);
		act (non_const_first, false, ch, 0, 0, TO_CHAR);
		act (non_const_third, false, ch, 0, 0, TO_ROOM);
	}

	error_string.clear();
	first_person->clear();
	if (argument[0] == '(')
	{
		for ( i++; argument[i] != '\0'; i++)
		{
			error_string.push_back(argument[i]);
		}
		first_person->assign(error_string);
	}
	else 
		first_person->assign(argument);

	return true;

}

void
do_plan (CHAR_DATA * ch, char *argument, int cmd)
{
  char buf[AVG_STRING_LENGTH * 2] = "";

  // change of plans
  if (argument && *argument)
    {
      while (isspace (*argument))
	argument++;
      
      // clear both strings
      if (strcasecmp (argument, "clear") == STR_MATCH )
	{
	  if (ch->plan)
	    {
	      delete ch->plan;
	      ch->plan = 0;
	    }
	  if (ch->goal)
	    {
	      delete ch->goal;
	      ch->goal = 0;
	    }
	  send_to_char ("All of your plans have been cleared.\n", ch);
	}

      // change the short-term plan
      else if (strncmp (argument, "short ", 6) == STR_MATCH)
	{
	  argument += 6;

	  // clear the short term plan
	  if (strcasecmp (argument, "clear") == STR_MATCH)
	    {
	      if (ch->plan)
		{
		  delete ch->plan;
		  ch->plan = 0;
		}
	      send_to_char ("Your short-term plan has been cleared.\n", ch);
	    }

	  // (re)set the short-term plan
	  else
	    {
	      int plan_length = strlen(argument);
	      if (plan_length && plan_length < 80)
		{
		  if (ch->plan)
		    {
		      delete ch->plan;
		      ch->plan = 0;
		    }
		  
		  ch->plan = new std::string (argument);
		  sprintf (buf, "Your short-term plan has been set to:\n"
			   "#6%s#0\n", argument);
		  send_to_char (buf, ch);
		}

	      // bad plan message size
	      else
		{
		  send_to_char ("Your short-term plan must be less than eighty characters in length.\nTo clear your plan, type #6plan short clear#0.\n", ch);
		}
	    }
	}

      // change the long-term plan
      else if (strncmp (argument, "long ", 5) == STR_MATCH)
	{
	  argument += 5;

	  // clear the long-term plan
	  if (strcasecmp (argument, "clear") == STR_MATCH)
	    {
	      if (ch->goal)
		{
		  delete ch->goal;
		  ch->goal = 0;
		}
	      send_to_char ("Your long-term plan has been cleared.\n", ch);
	    }
	  
	  // (re)set the long-term plan
	  else
	    {
	      int goal_length = strlen(argument);
	      if (goal_length && goal_length < 240)
		{
		  if (ch->goal)
		    {
		      delete ch->goal;
		      ch->goal = 0;
		    }
		  
		  ch->goal = new std::string (argument);
		  sprintf (buf, "Your long-term plan has been set to:\n\n"
			   "   #6%s#0\n", argument);
		  char *p;
		  reformat_string (buf, &p);
		  send_to_char (p, ch);
		  mem_free (p);
		}
	      
	      // bad message size
	      else
		{
		  send_to_char ("Your long-term plan must be at most three lines.\nTo clear your plan, type #6plan long clear#0.\n", ch);
		}
	    }
	}
      else
	{
	  const char * usage =
	    "To set your short-term plan, type:     #6plan short <message>#0\n"
	    "To clear your short-term plan, type:   #6plan short clear#0\n"
	    "To set your long-term plan, type:      #6plan long <message>#0\n"
	    "To clear your long-term plan, type:    #6plan long clear#0\n"
	    "To clear your all of your plans, type: #6plan clear#0\n";
	  send_to_char (usage,ch);
	}
    }
  else
    {
      if ((ch->plan && !ch->plan->empty()) || (ch->goal && !ch->goal->empty()))
	{
	  strcat (buf,"Your plans:\n");
	  if (ch->goal)
	    sprintf (buf + strlen(buf), "\nLong-term:\n#6   %s#0\n", ch->goal->c_str());
	  
	  if (ch->plan)
	    sprintf (buf + strlen(buf), "\nCurrently:\n#6%s#0\n", ch->plan->c_str());

	  send_to_char (buf, ch);
	}
      else
	{
	  send_to_char ("You do not have any plans.\n", ch);
	}
    }
  
}
