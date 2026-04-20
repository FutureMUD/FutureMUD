/*------------------------------------------------------------------------\
|  hash.c : Central Hash Module                       www.middle-earth.us | 
|  Copyright (C) 2004, Shadows of Isildur: Traithe                        |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/

#include <ctype.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <ctype.h>
#include <unistd.h>
#include <string.h>

#include "server.h"
#include "structs.h"
#include "protos.h"
#include "utils.h"
#include "utility.h"
#include "decl.h"

#include "room.h"

extern rpie::server engine;
extern struct char_data *character_list;
extern struct obj_data *object_list;
extern int top_of_zone_table;
extern struct zone_data *zone_table;

void
mobile_load_cues (MOB_DATA * mob)
{
  std::multimap<mob_cue,std::string> * cues = new std::multimap<mob_cue,std::string>;
  std::string world_db = engine.get_config ("world_db");
  mysql_safe_query 
    ( "SELECT cue+0, reflex"
      " FROM %s.cues"
      " WHERE mid = %d "
      " ORDER BY cue, id ASC", 
      world_db.c_str (), mob->nVirtual);

  MYSQL_RES *result = mysql_store_result (database);
  if (result)
    {
      MYSQL_ROW row;
      while ((row = mysql_fetch_row (result)))
	{
	  mob_cue c = mob_cue (strtol (row[0], 0, 10));
	  cues->insert (std::make_pair (c, std::string (row[1])));
	}
      
      mysql_free_result (result);
    }

  mob->cues = cues;
}

CHAR_DATA *
fread_mobile (int vnum, const int *nZone, FILE * fp)
{
  int i;
  long tmp;
  long tmp2;
  long tmp3;
  long clan1 = 0;
  long clan2 = 0, num = 0;
  char *p;
  char *p2;
  ROOM_DATA *room;
  CHAR_DATA *mob;
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char chk;
  long materials = 0, buy_flags = 0;
  int skills[MAX_SKILLS];

  mob = new_char (0);		/* NPC */

  clear_char (mob);
  mob->mob->currency_type = 0;

  mob->mob->nVirtual = vnum;
  mob->mob->zone = *nZone;

  mob->mob->carcass_vnum = 0;
  mobile_load_cues (mob->mob);

#define CHECK_DOUBLE_DEFS 1
#ifdef CHECK_DOUBLE_DEFS
  if (vtom (vnum))
    {
      sprintf (buf, "Mob %d multiply defined!!", vnum);
      system_log (buf, true);
    }
  else
#endif

    add_mob_to_hash (mob);

  mob->name = fread_string (fp);

  one_argument (mob->name, buf);

  mob->tname = add_hash (CAP (buf));

  while ((i = strlen (mob->name)) > 0 &&
	 (mob->name[i - 1] == '\r' || mob->name[i - 1] == '\n'))
    mob->name[i - 1] = '\0';

  mob->short_descr = fread_string (fp);

  while ((i = strlen (mob->short_descr)) > 0 &&
	 (mob->short_descr[i - 1] == '\r' || mob->short_descr[i - 1] == '\n'))
    mob->short_descr[i - 1] = '\0';

  mob->long_descr = fread_string (fp);

  while ((i = strlen (mob->long_descr)) > 0 &&
	 (mob->long_descr[i - 1] == '\r' || mob->long_descr[i - 1] == '\n'))
    mob->long_descr[i - 1] = '\0';

  mob->description = fread_string (fp);

  fscanf (fp, "%lu ", &tmp);
  mob->act = tmp;
  mob->act |= ACT_ISNPC;

  fscanf (fp, " %ld ", &tmp);
  mob->affected_by = tmp;

  fscanf (fp, " %ld ", &tmp);
  mob->offense = (int) tmp;
  fscanf (fp, " %ld ", &tmp);	/* Was defense */
  mob->race = (int) tmp;
  fscanf (fp, " %ld ", &tmp);
  mob->armor = (int) tmp;

  /* Need to reformat the following -- only need one var in the mob file for hp */

  fscanf (fp, " %ldd%ld+%ld ", &tmp, &tmp2, &tmp3);
  mob->max_hit = (int) tmp3;
  mob->hit = mob->max_hit;

  fscanf (fp, " %ldd%ld+%ld ", &tmp, &tmp2, &tmp3);
  mob->mob->damroll = (int) tmp3;
  mob->mob->damnodice = (int) tmp;
  mob->mob->damsizedice = (int) tmp2;

  mob->move = 50;
  mob->max_move = 50;

  fscanf (fp, " %ld ", &mob->time.birth);

  fscanf (fp, " %ld ", &tmp);
  mob->position = (int) tmp;

  fscanf (fp, " %ld ", &tmp);
  mob->default_pos = (int) tmp;

  fscanf (fp, " %ld ", &tmp);
  mob->sex = (int) tmp;

  fscanf (fp, " %ld ", &tmp);	/* Used for Regi's 7 econs for now */
  mob->mob->merch_seven = (int) tmp;

  // Previously deity; cannibalised for 'materials' for shopkeeps below.
  fscanf (fp, " %ld ", &materials);

  fscanf (fp, " %ld ", &tmp);	/* phys?  what's that? */
  mob->mob->vehicle_type = tmp;

  fscanf (fp, " %ld \n", &buy_flags);

  fscanf (fp, "%d %d %d %d %d %d %d %d %d %d %d %d %d %d %d %d %d %d\n", &mob->mob->skinned_vnum, &mob->circle, &mob->cell_1, &mob->mob->carcass_vnum, &mob->cell_2,	/*Formerly defense_bonus - free */
	  &mob->ppoints,
	  &mob->natural_delay,
	  &mob->mob->helm_room,
	  &mob->body_type,
	  &mob->poison_type,
	  &mob->nat_attack_type,
	  &mob->mob->access_flags,
	  &mob->height, &mob->frame, &mob->mob->noaccess_flags, &mob->cell_3, &mob->room_pos, &mob->mob->fallback);

  if (world_version_in == 5)
    {
      fscanf (fp, "%d %d %d %d %d %d %d\n",
	      &mob->str,
	      &mob->intel,
	      &mob->wil, &mob->aur, &mob->dex, &mob->con, &mob->speaks);
      mob->agi = 16;
    }
  else
    fscanf (fp, "%d %d %d %d %d %d %d %d\n",
	    &mob->str,
	    &mob->intel,
	    &mob->wil,
	    &mob->aur, &mob->dex, &mob->con, &mob->speaks, &mob->agi);

  if ((mob->race >= 0 && mob->race <= 11))
    {				/* Humanoid NPCs. */
      mob->max_hit = 40 + mob->con * CONSTITUTION_MULTIPLIER;
      mob->hit = mob->max_hit;
    }

  fscanf (fp, "%d %d\n", &mob->flags, &mob->mob->currency_type);

  if (IS_SET (mob->flags, FLAG_KEEPER))
    {

      mob->shop = (SHOP_DATA *) get_perm (sizeof (SHOP_DATA));

      mob->shop->materials = materials;
      mob->shop->buy_flags = buy_flags;

      fgets (buf, 256, fp);

      sscanf (buf, "%d %d %f %f %f %f %d\n",
	      &mob->shop->shop_vnum,
	      &mob->shop->store_vnum,
	      &mob->shop->markup,
	      &mob->shop->discount,
	      &mob->shop->econ_markup1,
	      &mob->shop->econ_discount1, &mob->shop->econ_flags1);

      if (mob->mob->merch_seven > 0)
	{
	  fscanf (fp,
		  "%f %f %d %f %f %d %f %f %d %f %f %d %f %f %d %f %f %d %d\n",
		  &mob->shop->econ_markup2, &mob->shop->econ_discount2,
		  &mob->shop->econ_flags2, &mob->shop->econ_markup3,
		  &mob->shop->econ_discount3, &mob->shop->econ_flags3,
		  &mob->shop->econ_markup4, &mob->shop->econ_discount4,
		  &mob->shop->econ_flags4, &mob->shop->econ_markup5,
		  &mob->shop->econ_discount5, &mob->shop->econ_flags5,
		  &mob->shop->econ_markup6, &mob->shop->econ_discount6,
		  &mob->shop->econ_flags6, &mob->shop->econ_markup7,
		  &mob->shop->econ_discount7, &mob->shop->econ_flags7,
		  &mob->shop->nobuy_flags);
	}
      else if (world_version_in >= 8)
	{
	  fscanf (fp, "%f %f %d %f %f %d %d\n",
		  &mob->shop->econ_markup2,
		  &mob->shop->econ_discount2,
		  &mob->shop->econ_flags2,
		  &mob->shop->econ_markup3,
		  &mob->shop->econ_discount3,
		  &mob->shop->econ_flags3, &mob->shop->nobuy_flags);
	}

      for (i = 0; i <= MAX_DELIVERIES; i++)
	{
	  num = fread_number (fp);
	  if (num == -1)
	    break;
	  mob->shop->delivery[i] = num;
	}

      fscanf (fp, "%d %d %d %d %d %d %d %d %d %d\n",
	      &mob->shop->trades_in[0],
	      &mob->shop->trades_in[1],
	      &mob->shop->trades_in[2],
	      &mob->shop->trades_in[3],
	      &mob->shop->trades_in[4],
	      &mob->shop->trades_in[5],
	      &mob->shop->trades_in[6],
	      &mob->shop->trades_in[7],
	      &mob->shop->trades_in[8], &mob->shop->trades_in[9]);

      if (mob->shop->store_vnum && (room = vtor (mob->shop->store_vnum)))
	{
	  room->room_flags |= STORAGE;
	}
    }

  for (i = 0; i < MAX_SKILLS; i++)
    skills[i] = 0;

  for (i = 0; i < (world_version_in >= 12 ? MAX_SKILLS : 60) / 10; i++)
    fscanf (fp, "%d %d %d %d %d %d %d %d %d %d\n",
	    &skills[i * 10],
	    &skills[i * 10 + 1],
	    &skills[i * 10 + 2],
	    &skills[i * 10 + 3],
	    &skills[i * 10 + 4],
	    &skills[i * 10 + 5],
	    &skills[i * 10 + 6],
	    &skills[i * 10 + 7], &skills[i * 10 + 8], &skills[i * 10 + 9]);

  for (i = 0; i < MAX_SKILLS; i++)
    mob->skills[i] = skills[i];

  if (world_version_in >= 10)
    mob->clans = fread_string (fp);


// For mobiles with venom


  if(IS_SET (mob->act, ACT_VENOM))
  {
    CREATE (mob->venom, POISON_DATA, 1);
    fscanf(fp, "%d %d %d %d %d %d %d %d %d %d %d %d\n", 
           &mob->venom->poison_type, &mob->venom->duration, &mob->venom->latency,
           &mob->venom->minute, &mob->venom->max_power, &mob->venom->lvl_power,
           &mob->venom->atm_power, &mob->venom->attack, &mob->venom->decay,
           &mob->venom->sustain, &mob->venom->release, &mob->venom->uses);
  }

/*** for morphing mobs ***/
		chk = getc (fp);
		if (chk != '#')
			{
			fscanf (fp, "%d %d %d\n", &mob->clock, &mob->morphto, &mob->morph_type);
			}
		else
			ungetc (chk, fp);
	
/*************************/



  mob->time.played = 0;
  mob->time.logon = time (0);

  mob->intoxication = 0;
  mob->hunger = -1;
  mob->thirst = -1;

  mob->tmp_str = mob->str;
  mob->tmp_dex = mob->dex;
  mob->tmp_intel = mob->intel;
  mob->tmp_aur = mob->aur;
  mob->tmp_wil = mob->wil;
  mob->tmp_con = mob->con;
  mob->tmp_agi = mob->agi;

  mob->equip = NULL;

  mob->mob->nVirtual = vnum;

  mob->desc = 0;

  if (mob->speaks == 0)
    {
      mob->skills[SKILL_SPEAK_SINDARIN] = 100;
      mob->speaks = SKILL_SPEAK_SINDARIN;
    }

  if (!mob->skills[mob->speaks])
    mob->skills[mob->speaks] = 100;

  p = mob->clans;
  p2 = p;
  mob->clans = str_dup ("");

  while (*p2)
    {

      p = one_argument (p, buf);	/* flags     */
      p = one_argument (p, buf2);	/* clan name */

      if (!*buf2)
	break;

      add_clan_id (mob, buf2, buf);
    }

  if (p2 && *p2)
    mem_free (p2);

  if (clan1 == 1)
    {
      clan1 = 0;
      mob->act |= ACT_WILDLIFE;
    }

  if (clan2 == 1)
    {
      clan2 = 0;
      mob->act |= ACT_WILDLIFE;
    }

  if (clan1)
    {
      sprintf (buf, "%ld", clan1);
      add_clan_id (mob, buf,
		   GET_FLAG (mob, FLAG_LEADER_1) ? "leader" : "member");
      mob->flags &= ~FLAG_LEADER_1;
    }

  if (clan2)
    {
      sprintf (buf, "%ld", clan2);
      add_clan_id (mob, buf,
		   GET_FLAG (mob, FLAG_LEADER_2) ? "leader" : "member");
      mob->flags &= ~FLAG_LEADER_2;
    }

  fix_offense (mob);

  mob->max_mana = mob->aur * 5;
  mob->mana = mob->max_mana;

  if (lookup_race_variable (mob->race, RACE_BODY_PROTO) != NULL)
    mob->body_proto =
      atoi (lookup_race_variable (mob->race, RACE_BODY_PROTO));

  if (lookup_race_variable (mob->race, RACE_MIN_HEIGHT) != NULL)
    mob->mob->min_height =
      atoi (lookup_race_variable (mob->race, RACE_MIN_HEIGHT));

  if (lookup_race_variable (mob->race, RACE_MAX_HEIGHT) != NULL)
    mob->mob->max_height =
      atoi (lookup_race_variable (mob->race, RACE_MAX_HEIGHT));

  apply_race_affects (mob);

  return (mob);
}

CHAR_DATA *
load_mobile (int vnum)
{
  CHAR_DATA *proto;
  CHAR_DATA *new_mobile;
  MOB_DATA *mob_info;

  if (!(proto = vtom (vnum)))
    return NULL;

  new_mobile = new_char (0);	/* NPC */

  mob_info = new_mobile->mob;

  memcpy (new_mobile, proto, sizeof (CHAR_DATA));

  new_mobile->mob = mob_info;

  memcpy (new_mobile->mob, proto->mob, sizeof (MOB_DATA));

  /* A mostly unique number.  Can be used to ensure
     the same mobile is being used between game plays.  */

  new_mobile->coldload_id = get_next_coldload_id (0);
  new_mobile->deleted = 0;

  new_mobile->next = character_list;
  character_list = new_mobile;

  new_mobile->time.birth = time (0);

  new_mobile->max_move = calc_lookup (new_mobile, REG_MISC, MISC_MAX_MOVE);

  if (!new_mobile->height)
    make_height (new_mobile);

  if (!new_mobile->frame)
    make_frame (new_mobile);

  if (IS_SET (new_mobile->affected_by, AFF_HIDE))
    magic_add_affect (new_mobile, MAGIC_HIDDEN, -1, 0, 0, 0, 0);

  new_mobile->fight_mode = 2;

  if (IS_SET (new_mobile->flags, FLAG_VARIABLE))
    {
      randomize_mobile (new_mobile);
      new_mobile->flags &= ~FLAG_VARIABLE;
    }

  new_mobile->clans = str_dup (proto->clans);

  new_mobile->move = new_mobile->max_move;

  new_mobile->mount = NULL;
  new_mobile->wounds = NULL;
  new_mobile->lodged = NULL;
  new_mobile->subdue = NULL;

  new_mobile->mob->owner = NULL;

	if ((new_mobile->clock > 0) && (new_mobile->morphto > 0))
		{
    new_mobile->morph_time = time (0) + new_mobile->clock * 15 * 60;
    new_mobile->act |= ACT_STAYPUT;
    }
    
  if (new_mobile->speaks == SKILL_HEALING)
    new_mobile->speaks = SKILL_SPEAK_SINDARIN;

  return new_mobile;
}

// Determines whether the variable we have is a valid color.

int
is_variable_color(char *string)
{

  int ind = -1;
  ind = index_lookup (standard_object_colors, string);
  if(ind == -1)
    ind = index_lookup (fine_object_colors, string);
  if(ind == -1)
      ind = index_lookup (drab_object_colors, string);
  if(ind == -1)
      ind = index_lookup (fine_gem_colors, string);
  if(ind == -1)
      ind = index_lookup (gem_colors, string);
  if(ind == -1)
      ind = index_lookup (blade_type_colors, string);
  if(ind == -1)
      ind = index_lookup (design_type_colors, string);
  if(ind == -1)
      ind = index_lookup (animal_colors, string);
  if(ind == -1)
      ind = index_lookup (light_object_colors, string);
  if(ind == -1)
      ind = index_lookup (stone_colors, string);
  if(ind == -1)
      ind = index_lookup (beor_art, string);
  if(ind == -1)
      ind = index_lookup (war_art, string);
  if(ind == -1)
      ind = index_lookup (dark_object_colors, string);
  if(ind == -1)
      ind = index_lookup (hardwood_colors, string);
  if(ind == -1)
      ind = index_lookup (metal_damage_colors, string);
  if(ind == -1)
      ind = index_lookup (orc_colors, string);

  if(ind > -1)
    return 1;
  else
    return 0;
}

void
insert_string_variables (OBJ_DATA * new_obj, OBJ_DATA * proto, char *string, char *string2, char *string3)
{
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];
  char temp[MAX_STRING_LENGTH];
  char original[MAX_STRING_LENGTH];
  char color[MAX_STRING_LENGTH];
  char color2[MAX_STRING_LENGTH];
  char color3[MAX_STRING_LENGTH];
  char tempcolor[MAX_STRING_LENGTH];
  char *point;
  int i, j, limit;
  bool modified = false;

  *buf = '\0';
  *buf2 = '\0';
  *temp = '\0';

  if (string == 0)
    *color = '\0';
  else
    sprintf (color, "%s", string);

  if (string2 == 0)
    *color2 = '\0';
  else
  {
    sprintf (color2, "%s", string2);
  }

  if (string3 == 0)
    *color3 = '\0';
  else
  {
    sprintf (color3, "%s", string3);
  }

  // Prevents you loading up silly colours.

  if(!is_variable_color(color))
    *color = '\0';

  if(!is_variable_color(color2))
    *color2 = '\0';

  if(!is_variable_color(color3))
    *color3 = '\0';

  new_obj->obj_flags.extra_flags |= ITEM_VARIABLE;

  if (!*proto->full_description)
    return;

  sprintf (original, "%s", proto->description); 
  point = strpbrk (original, "$");
  int round = 0;

// This function will go to "point", 
// cut out that word and paste the appropriate color in there,
// and then continue on, until the new string is formed.
// it repeats for every other point there is.

  if (point)
    {
      for (size_t y = 0; y <= strlen (original); y++)
	{
	  if (original[y] == *point)
	    {
	      modified = true;
              sprintf (temp, "$");
              j = y + 1;
	      while (isalpha (original[j]))
              {              
                sprintf (temp + strlen (temp), "%c", original[j]);
		j++;
              }

              if ((!*color && round == 0) || (!*color2 && round == 1) || (!*color3 && round == 2))
              {
                  if (strstr (temp, "$color"))
                  {
                    for (i = 0, limit = 0; *standard_object_colors[i] != '\n';i++, limit++)
                      ;
              	    limit--;
              	    sprintf (tempcolor, "%s", standard_object_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$drabcolor"))
                  {
                    for (i = 0, limit = 0; *drab_object_colors[i] != '\n'; i++, limit++)
                      ;
              	    limit--;
              	    sprintf (tempcolor, "%s", drab_object_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$finecolor"))
                  {
                    for (i = 0, limit = 0; *fine_object_colors[i] != '\n'; i++, limit++)
                      ;
	              limit--;
              	    sprintf (tempcolor, "%s", fine_object_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$lightcolor"))
                  {
                    for (i = 0, limit = 0; *light_object_colors[i] != '\n'; i++, limit++)
                      ;
              	    limit--;
              	    sprintf (tempcolor, "%s", light_object_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$commonstone"))
                  {
                    for (i = 0, limit = 0; *stone_colors[i] != '\n'; i++, limit++)
                      ;
              	    limit--;
              	    sprintf (tempcolor, "%s", stone_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$beorart"))
                  {
                    for (i = 0, limit = 0; *beor_art[i] != '\n'; i++, limit++)
                      ;
                    limit--;
              	    sprintf (tempcolor, "%s", beor_art[number (0, limit)]);
                  }
                  else if (strstr (temp, "$warart"))
                  {
                    for (i = 0, limit = 0; *war_art[i] != '\n'; i++, limit++)
                      ;
                    limit--;
              	    sprintf (tempcolor, "%s", war_art[number (0, limit)]);
                  }
                  else if (strstr (temp, "$darkcolor"))
                  {
                    for (i = 0, limit = 0; *dark_object_colors[i] != '\n'; i++, limit++)
                      ;
              	    limit--;
              	    sprintf (tempcolor, "%s", dark_object_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$gemcolor"))
                  {
                    for (i = 0, limit = 0; *gem_colors[i] != '\n'; i++, limit++)
                      ;
              	    limit--;
              	    sprintf (tempcolor, "%s", gem_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$blade"))
                  {
                    for (i = 0, limit = 0; *blade_type_colors[i] != '\n'; i++, limit++)
                      ;
              	    limit--;
              	    sprintf (tempcolor, "%s", blade_type_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$design"))
                  {
                    for (i = 0, limit = 0; *design_type_colors[i] != '\n'; i++, limit++)
                      ;
              	    limit--;
              	    sprintf (tempcolor, "%s", design_type_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$animal"))
                  {
                    for (i = 0, limit = 0; *animal_colors[i] != '\n'; i++, limit++)
                      ;
              	    limit--;
              	    sprintf (tempcolor, "%s", animal_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$hardwood"))
                  {
                    for (i = 0, limit = 0; *hardwood_colors[i] != '\n'; i++, limit++)
                      ;
                  	limit--;
                  	sprintf (tempcolor, "%s", hardwood_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$metdam"))
                  {
                    for (i = 0, limit = 0; *metal_damage_colors[i] != '\n'; i++, limit++)
                      ;
                  	limit--;
                  	sprintf (tempcolor, "%s", metal_damage_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$orcolor"))
                  {
                    for (i = 0, limit = 0; *orc_colors[i] != '\n'; i++, limit++)
                      ;
                  	limit--;
                  	sprintf (tempcolor, "%s", orc_colors[number (0, limit)]);
                  }
                  else if (strstr (temp, "$finegemcolor"))
                  {
                    for (i = 0, limit = 0; *fine_gem_colors[i] != '\n'; i++, limit++)
                      ;
                  	limit--;
                  	sprintf (tempcolor, "%s", fine_gem_colors[number (0, limit)]);
                  }
  
                if(round == 0)
                  sprintf (color, "%s", tempcolor);
                else if(round == 1)
                  sprintf (color2, "%s", tempcolor);
                else if(round == 2)
                  sprintf (color3, "%s", tempcolor);
              }

              if(round == 0)
              {
                sprintf (buf2 + strlen (buf2), "%s", color);
                round = 1;
              }
              else if(round == 1)
              {
                sprintf (buf2 + strlen (buf2), "%s", color2);
                round = 2;
              }
              else if(round == 2)
	      sprintf (buf2 + strlen (buf2), "%s", color3);

	      j = y + 1;
	      while (isalpha (original[j]))
		j++;
	      y = j;
	    }
	  sprintf (buf2 + strlen (buf2), "%c", original[y]);
	}

      mem_free (new_obj->description);
      new_obj->description = add_hash (buf2);
    }

  *buf2 = '\0';
  sprintf (original, "%s", proto->short_description);
  point = strpbrk (original, "$");
  round = 0;

  if (point)
    {
      for (size_t y = 0; y <= strlen (original); y++)
	{
	  if (original[y] == *point)
	    {
	      modified = true;
              sprintf (temp, "$");
              j = y + 1;

              if(round == 0)
              {
                sprintf (buf2 + strlen (buf2), "%s", color);
                round = 1;
              }
              else if(round == 1)
              {
                sprintf (buf2 + strlen (buf2), "%s", color2);
                round = 2;
              }
              else if(round == 2)
	      sprintf (buf2 + strlen (buf2), "%s", color3);

	      j = y + 1;
	      while (isalpha (original[j]))
		j++;
	      y = j;
	    }
	  sprintf (buf2 + strlen (buf2), "%c", original[y]);
	}

      mem_free (new_obj->short_description);
      new_obj->short_description = add_hash (buf2);
    }

  *buf2 = '\0';
  sprintf (original, "%s", proto->full_description);
  point = strpbrk (original, "$");
  round = 0;

  if (point)
    {
      for (size_t y = 0; y <= strlen (original); y++)
	{
	  if (original[y] == *point)
	    {
	      modified = true;
              sprintf (temp, "$");
              j = y + 1;

              if(round == 0)
              {
                sprintf (buf2 + strlen (buf2), "%s", color);
                round = 1;
              }
              else if(round == 1)
              {
                sprintf (buf2 + strlen (buf2), "%s", color2);
                round = 2;
              }
              else if(round == 2)
	      sprintf (buf2 + strlen (buf2), "%s", color3);

	      j = y + 1;
	      while (isalpha (original[j]))
		j++;
	      y = j;
	    }
	  sprintf (buf2 + strlen (buf2), "%c", original[y]);
	}

      mem_free (new_obj->full_description);
      new_obj->full_description = add_hash (buf2);
    }

  *buf2 = '\0';
  sprintf (original, "%s", proto->name);
  point = strpbrk (original, "$");
  round = 0;

  if (point)
    {
      for (size_t y = 0; y <= strlen (original); y++)
	{
	  if (original[y] == *point)
	    {
	      modified = true;
              sprintf (temp, "$");
              j = y + 1;

              if(round == 0)
              {
                sprintf (buf2 + strlen (buf2), "%s", color);
                round = 1;
              }
              else if(round == 1)
              {
                sprintf (buf2 + strlen (buf2), "%s", color2);
                round = 2;
              }
              else if(round == 2)
	      sprintf (buf2 + strlen (buf2), "%s", color3);

	      j = y + 1;
	      while (isalpha (original[j]))
		j++;
	      y = j;
	    }
	  sprintf (buf2 + strlen (buf2), "%c", original[y]);
	}

      mem_free (new_obj->name);
      new_obj->name = add_hash (buf2);
    }

  if ((IS_SET (new_obj->obj_flags.extra_flags, ITEM_MASK)
       && new_obj->obj_flags.type_flag == ITEM_ARMOR)
      || (IS_SET (new_obj->obj_flags.extra_flags, ITEM_MASK)
	  && new_obj->obj_flags.type_flag == ITEM_WORN)
      || (IS_SET (new_obj->obj_flags.extra_flags, ITEM_MASK)
	  && new_obj->obj_flags.type_flag == ITEM_CONTAINER))
    {

  *buf2 = '\0';
  sprintf (original, "%s", proto->desc_keys);
  point = strpbrk (original, "$");
  round = 0;

  if (point)
    {
      for (size_t y = 0; y <= strlen (original); y++)
	{
	  if (original[y] == *point)
	    {
	      modified = true;
              sprintf (temp, "$");
              j = y + 1;

              if(round == 0)
              {
                sprintf (buf2 + strlen (buf2), "%s", color);
                round = 1;
              }
              else if(round == 1)
              {
                sprintf (buf2 + strlen (buf2), "%s", color2);
                round = 2;
              }
              else if(round == 2)
	      sprintf (buf2 + strlen (buf2), "%s", color3);

	      j = y + 1;
	      while (isalpha (original[j]))
		j++;
	      y = j;
	    }
	  sprintf (buf2 + strlen (buf2), "%c", original[y]);
	}

      mem_free (new_obj->desc_keys);
      new_obj->desc_keys = add_hash (buf2);
    }

    }
// End point.

  if (new_obj->var_color)
    mem_free (new_obj->var_color);

  if (new_obj->var_color2)
    mem_free (new_obj->var_color2);

  if (new_obj->var_color3)
    mem_free (new_obj->var_color3);

  if (!modified)
  {
    new_obj->var_color = add_hash ("none");
    new_obj->var_color2 = add_hash ("none");
    new_obj->var_color3 = add_hash ("none");
  }
  else
  {
    new_obj->var_color = add_hash (color);
    new_obj->var_color2 = add_hash (color2);
    new_obj->var_color3 = add_hash (color3);
  }
}

OBJ_DATA *
load_object (int vnum)
{
  OBJ_DATA *proto;
  OBJ_DATA *new_obj;
  WRITING_DATA *writing;
  int i;
  AFFECTED_TYPE *af;
  AFFECTED_TYPE *new_af;
  AFFECTED_TYPE *last_af = NULL;

  if (!(proto = vtoo (vnum)))
    return NULL;

  new_obj = new_object ();

  memcpy (new_obj, proto, sizeof (OBJ_DATA));

  new_obj->deleted = 0;

  new_obj->xaffected = NULL;

  new_obj->next_content = 0;

  new_obj->var_color = (char *) NULL;
  new_obj->var_color2 = (char *) NULL;
  new_obj->var_color3 = (char *) NULL;

  for (af = proto->xaffected; af; af = af->next)
    {

      new_af = (AFFECTED_TYPE *) alloc (sizeof (AFFECTED_TYPE), 13);

      memcpy (new_af, af, sizeof (AFFECTED_TYPE));

      new_af->next = NULL;

      if (!new_obj->xaffected)
	new_obj->xaffected = new_af;
      else
	last_af->next = new_af;

      last_af = new_af;
    }

  new_obj->next = object_list;
  object_list = new_obj;

  new_obj->count = 1;

  if (new_obj->clock && new_obj->morphto)
    new_obj->morphTime = time (0) + new_obj->clock * 15 * 60;

  if (GET_ITEM_TYPE (new_obj) == ITEM_BOOK)
    {
      if (!new_obj->writing && new_obj->o.od.value[0] > 0)
	{
	  CREATE (new_obj->writing, WRITING_DATA, 1);
	  for (i = 1, writing = new_obj->writing; i <= new_obj->o.od.value[0];
	       i++)
	    {
	      writing->message = add_hash ("blank");
	      writing->author = add_hash ("blank");
	      writing->date = add_hash ("blank");
	      writing->ink = add_hash ("blank");
	      writing->language = 0;
	      writing->script = 0;
	      writing->skill = 0;
	      writing->torn = false;
	      if (i != new_obj->o.od.value[0])
		{
		  CREATE (writing->next_page, WRITING_DATA, 1);
		  writing = writing->next_page;
		}
	    }
	}
    }

  if (GET_ITEM_TYPE (new_obj) == ITEM_BOOK)
    new_obj->o.od.value[1] = unused_writing_id ();

  if (GET_ITEM_TYPE (new_obj) == ITEM_PARCHMENT)
    new_obj->o.od.value[0] = unused_writing_id ();

  if (IS_SET (new_obj->obj_flags.extra_flags, ITEM_VARIABLE))
    insert_string_variables (new_obj, proto, 0, 0, 0);
  else
  {
    new_obj->var_color = 0;
    new_obj->var_color2 = 0;
    new_obj->var_color3 = 0;
  }

  if (GET_ITEM_TYPE (new_obj) == ITEM_WEAPON)
    new_obj->obj_flags.extra_flags |= ITEM_NEWSKILLS;

  new_obj->contains = NULL;
  new_obj->lodged = NULL;
  new_obj->wounds = NULL;
  new_obj->equiped_by = NULL;
  new_obj->carried_by = NULL;
  new_obj->in_obj = NULL;

  if (!new_obj->item_wear)
    new_obj->item_wear = 100;

  new_obj->coldload_id = get_next_coldload_id (2);

  vtoo (new_obj->nVirtual)->instances++;

  return new_obj;
}

OBJ_DATA *
load_colored_object (int vnum, char *color, char *color2, char *color3)
{
  OBJ_DATA *proto;
  OBJ_DATA *new_obj;
  WRITING_DATA *writing;
  int i;
  AFFECTED_TYPE *af;
  AFFECTED_TYPE *new_af;
  AFFECTED_TYPE *last_af = NULL;

  if (!(proto = vtoo (vnum)))
    return NULL;

  new_obj = new_object ();

  memcpy (new_obj, proto, sizeof (OBJ_DATA));

  new_obj->deleted = 0;

  new_obj->xaffected = NULL;

  new_obj->var_color = (char *) NULL;
  new_obj->var_color2 = (char *) NULL;
  new_obj->var_color3 = (char *) NULL;

  for (af = proto->xaffected; af; af = af->next)
    {

      new_af = (AFFECTED_TYPE *) alloc (sizeof (AFFECTED_TYPE), 13);

      memcpy (new_af, af, sizeof (AFFECTED_TYPE));

      new_af->next = NULL;

      if (!new_obj->xaffected)
	new_obj->xaffected = new_af;
      else
	last_af->next = new_af;

      last_af = new_af;
    }

  new_obj->next = object_list;
  object_list = new_obj;

  new_obj->count = 1;

  if (new_obj->clock && new_obj->morphto)
    new_obj->morphTime = time (0) + new_obj->clock * 15 * 60;

  if (GET_ITEM_TYPE (new_obj) == ITEM_BOOK)
    {
      if (!new_obj->writing && new_obj->o.od.value[0] > 0)
	{
	  CREATE (new_obj->writing, WRITING_DATA, 1);
	  for (i = 1, writing = new_obj->writing; i <= new_obj->o.od.value[0];
	       i++)
	    {
	      writing->message = add_hash ("blank");
	      writing->author = add_hash ("blank");
	      writing->date = add_hash ("blank");
	      writing->ink = add_hash ("blank");
	      writing->language = 0;
	      writing->script = 0;
	      writing->skill = 0;
	      writing->torn = false;
	      if (i != new_obj->o.od.value[0])
		{
		  CREATE (writing->next_page, WRITING_DATA, 1);
		  writing = writing->next_page;
		}
	    }
	}
    }

  if (IS_SET (new_obj->obj_flags.extra_flags, ITEM_VARIABLE))
    insert_string_variables (new_obj, proto, color, color2, color3);
  else
  {
    new_obj->var_color = 0;
    new_obj->var_color2 = 0;
    new_obj->var_color3 = 0;
  }

  if (GET_ITEM_TYPE (new_obj) == ITEM_WEAPON)
    new_obj->obj_flags.extra_flags |= ITEM_NEWSKILLS;

  vtoo (new_obj->nVirtual)->instances++;

  return new_obj;
}

OBJ_DATA *
fread_object (int vnum, int nZone, FILE * fp)
{
  OBJ_DATA *obj;
  float tmpf = 0;
  int tmp;
  char chk[50];
  char buf[MAX_STRING_LENGTH];
  EXTRA_DESCR_DATA *new_descr;
  EXTRA_DESCR_DATA *tmp_descr;
  AFFECTED_TYPE *af;
  AFFECTED_TYPE *taf;
  OBJ_CLAN_DATA *newclan = NULL;
  POISON_DATA *poison;
  POISON_DATA *xpoison;
  char peak_char;
  extern char *null_string;

  obj = new_object ();

  clear_object (obj);

  obj->nVirtual = vnum;
  obj->zone = nZone;

#if CHECK_DOUBLE_DEFS
  if (vtoo (vnum))
    {
      sprintf (buf, "OBJ %d multiply defined!!", vnum);
      system_log (buf, true);
    }
  else
#endif
    add_obj_to_hash (obj);

  obj->name = fread_string (fp);
  obj->short_description = fread_string (fp);
  obj->description = fread_string (fp);
  obj->full_description = fread_string (fp);

  if (!strcmp (obj->full_description, "(null)"))
    {
      sprintf (buf, "NOTE:  Object %d with '(null)' full description fixed.",
	       obj->nVirtual);
      system_log (buf, true);
      obj->full_description = null_string;
    }

  /* *** numeric data *** */

  fscanf (fp, " %d ", &tmp);
  obj->obj_flags.type_flag = tmp;
  fscanf (fp, " %d ", &tmp);
  obj->obj_flags.extra_flags = tmp;
  fscanf (fp, " %d ", &tmp);
  obj->obj_flags.wear_flags = tmp;
  fscanf (fp, " %d ", &tmp);
  obj->o.od.value[0] = tmp;
  fscanf (fp, " %d ", &tmp);
  obj->o.od.value[1] = tmp;
  fscanf (fp, " %d ", &tmp);
  obj->o.od.value[2] = tmp;
  fscanf (fp, " %d ", &tmp);
  obj->o.od.value[3] = tmp;

  fscanf (fp, " %d ", &tmp);	/* Weight */

  if (world_version_in == 4)
    obj->obj_flags.weight = tmp * 100;
  else
    obj->obj_flags.weight = tmp;

  fscanf (fp, " %f\n", &tmpf);
  obj->silver = tmpf;		/* Changed to silver from cost */

  fscanf (fp, " %d ", &tmp);
  obj->o.od.value[4] = tmp;

  fscanf (fp, " %d ", &tmp);
  obj->room_pos = tmp;

  if (world_version_in == 6)
    {
      fscanf (fp, " %d", &obj->o.od.value[5]);
      if (obj->obj_flags.type_flag == ITEM_WORN ||
	  obj->obj_flags.type_flag == ITEM_ARMOR)
	obj->o.od.value[5] = 0;
    }

  else
    {

      if (obj->obj_flags.type_flag == ITEM_INK)
	obj->ink_color = fread_string (fp);

      else if (IS_SET (obj->obj_flags.extra_flags, ITEM_MASK) &&
	       obj->obj_flags.type_flag == ITEM_WORN)
	obj->desc_keys = fread_string (fp);

      else if (IS_SET (obj->obj_flags.extra_flags, ITEM_MASK) &&
	       obj->obj_flags.type_flag == ITEM_ARMOR)
	obj->desc_keys = fread_string (fp);

      else if (IS_SET (obj->obj_flags.extra_flags, ITEM_MASK) &&
	       obj->obj_flags.type_flag == ITEM_CONTAINER)
	obj->desc_keys = fread_string (fp);

      else if (obj->obj_flags.type_flag == ITEM_TOSSABLE)
	obj->desc_keys = fread_string (fp);

      else
	{
	  fscanf (fp, " %d", &obj->o.od.value[5]);
	}
    }

  fscanf (fp, " %d %d %d %d %d\n",
	  &obj->activation,
	  &obj->quality, &obj->econ_flags, &obj->size, &obj->count);

  if (world_version_out >= 11)
    {
      fscanf (fp, "%f %d %d %d %d %d %d\n",
	      &obj->farthings,
	      &obj->clock,
	      &obj->morphto, &obj->item_wear, &obj->material, &tmp, &tmp);
    }

  if (GET_ITEM_TYPE (obj) == ITEM_INK)
    {
      obj->clock = 0;
      obj->morphto = 0;
    }

  /* *** extra descriptions *** */

  obj->ex_description = 0;
  obj->wdesc = 0;

  do
    {
      while ((peak_char = getc (fp)) == ' ' || peak_char == '\t' ||
	     peak_char == '\n')
	;

      ungetc (peak_char, fp);

      if (peak_char != 'E')
	break;

      fscanf (fp, " %s \n", chk);

      new_descr =
	(struct extra_descr_data *)
	get_perm (sizeof (struct extra_descr_data));

      new_descr->keyword = fread_string (fp);
      new_descr->description = fread_string (fp);

      /* Add descr's in same order as read so that they
         can get written back in same order */

      new_descr->next = NULL;

      if (!obj->ex_description)
	obj->ex_description = new_descr;
      else
	{
	  tmp_descr = obj->ex_description;
	  while (tmp_descr->next)
	    tmp_descr = tmp_descr->next;
	  tmp_descr->next = new_descr;
	}
    }
  while (1);

  tmp = 0;

  do
    {
      while ((peak_char = getc (fp)) == ' ' || peak_char == '\t' ||
	     peak_char == '\n')
	;

      ungetc (peak_char, fp);

      if (peak_char != 'A')
	break;

      fscanf (fp, " %s \n", chk);

      af = (AFFECTED_TYPE *) get_perm (sizeof (AFFECTED_TYPE));

      af->type = 0;
      af->a.spell.duration = -1;
      af->a.spell.bitvector = 0;
      af->a.spell.sn = 0;
      af->next = NULL;

      fscanf (fp, " %d %d\n", &af->a.spell.location, &af->a.spell.modifier);

      if (af->a.spell.location || af->a.spell.modifier)
	{

	  tmp++;

	  if (!obj->xaffected)
	    obj->xaffected = af;
	  else
	    {
	      for (taf = obj->xaffected; taf->next; taf = taf->next)
		;
	      taf->next = af;
	    }
	}

    }
  while (1);

  // One clan per object
   do
    {
      while ((peak_char = getc (fp)) == ' ' || peak_char == '\t' ||
	     peak_char == '\n')
	;

      ungetc (peak_char, fp);

      if (peak_char != 'C')
	break;

			fscanf (fp, "%s \n", chk);
	
			CREATE (newclan, OBJ_CLAN_DATA, 1);
	 		newclan->name = fread_string (fp);
	 		newclan->rank = fread_string (fp);
	 		newclan->next = NULL;
	 
	 		tmp++;
	 		
	 		if (!obj->clan_data)
	 			obj->clan_data = newclan;
   	}
  while (1);

  do
    {
      while ((peak_char = getc (fp)) == ' ' || peak_char == '\t' ||
	     peak_char == '\n')
	;

      ungetc (peak_char, fp);

      if (peak_char != 'P')
	break;

      fscanf (fp, " %s \n", chk);

      //CREATE (poison, POISON_DATA, 1);

      poison = (POISON_DATA *) get_perm (sizeof (POISON_DATA));

      poison->poison_type = 0;
      poison->duration = 0;
      poison->latency = 0;
      poison->minute = 0;
      poison->max_power = 0;
      poison->lvl_power = 0;
      poison->atm_power = 0;
      poison->attack = 0;
      poison->decay = 0;
      poison->sustain = 0;
      poison->release = 0;
      poison->uses = -1;
      poison->next = NULL;

      fscanf(fp, "%d %d %d %d %d %d %d %d %d %d %d %d\n", 
           &poison->poison_type, &poison->duration, &poison->latency,
           &poison->minute, &poison->max_power, &poison->lvl_power,
           &poison->atm_power, &poison->attack, &poison->decay,
           &poison->sustain, &poison->release, &poison->uses);

      if (poison->poison_type)
	{

	  tmp++;

	  if (!obj->poison)
	    obj->poison = poison;
	  else
	    {
	      for (xpoison = obj->poison; xpoison->next; xpoison = xpoison->next)
		;
	      xpoison->next = poison;
	    }
	}

    }
  while (1);

  if (tmp > 20)
    printf ("Object %d has %d affects\n", obj->nVirtual, tmp);

  obj->in_room = NOWHERE;
  obj->next_content = 0;
  obj->carried_by = 0;
  obj->equiped_by = 0;
  obj->in_obj = 0;
  obj->contains = 0;

  if (obj->count == 0)
    obj->count = 1;

  if (IS_SET (obj->obj_flags.wear_flags, ITEM_WEAR_SHIELD))
    obj->obj_flags.wear_flags &= ~ITEM_WEAR_SHIELD;

  obj->creater = fread_string (fp);
  obj->editer = fread_string (fp);

  return obj;
}

int
calculate_race_height (CHAR_DATA * tch)
{
  int min = 0, max = 0;
  float percentile = 0.0;

  if (tch->mob && tch->mob->min_height && tch->mob->max_height)
    {
      min = tch->mob->min_height;
      max = tch->mob->max_height;
      if (max < min)
	max += number (1, 10);
    }
  else if (!lookup_race_variable (tch->race, RACE_MIN_HEIGHT)
	   || !lookup_race_variable (tch->race, RACE_MAX_HEIGHT))
    return -1;
  else
    {
      min = atoi (lookup_race_variable (tch->race, RACE_MIN_HEIGHT));
      max = atoi (lookup_race_variable (tch->race, RACE_MAX_HEIGHT));
    }

  if (!IS_NPC (tch))
    {
      if (tch->height == 1)	// short
	percentile = (float) (number (1, 30) / 100.0);
      else if (tch->height == 2)	// average
	percentile = (float) (number (31, 69) / 100.0);
      else
	percentile = (float) (number (70, 100) / 100.0);	// tall
    }
  else
    percentile = (float) (number (25, 85) / 100.0);

  return ((int) (min + ((max - min) * percentile)));
}

int
calculate_size_height (CHAR_DATA * tch)
{
  if (tch->size == -3)		// XXS  (insect)
    return (number (1, 3));
  else if (tch->size == -2)	// XS   (rodent, rabbit, etc.)
    return (number (3, 7));
  else if (tch->size == -1)
    {				// S    (small humanoid, dog, cat, etc.)
      if (tch->body_proto == PROTO_HUMANOID)
	return (number (36, 60));
      else
	return (number (36, 54));
    }
  else if (tch->size == 0)
    {				// M    (average humanoid, livestock, etc.)
      if (tch->body_proto == PROTO_HUMANOID)
	return (number (58, 70));
      else
	return (number (36, 54));
    }
  else if (tch->size == 1)	// L    (large humanoid, troll)
    return (number (100, 120));
  else if (tch->size == 2)	// XL   (ent, giant)
    return (number (180, 240));
  else				// XXL  (dragon)
    return (number (300, 480));
}

void
make_height (CHAR_DATA * mob)
{
  if ((mob->height = calculate_race_height (mob)) == -1)
    mob->height = calculate_size_height (mob);

  return;
}

void
make_frame (CHAR_DATA * mob)
{
  if (!lookup_race_variable (mob->race, RACE_ID))
    {
      mob->frame = 3;
      return;
    }

  if (mob->sex == SEX_MALE)
    mob->frame = 3 + number (-1, 3);
  else
    mob->frame = 3 + number (-3, 1);
}

#define ZCMD zone_table[zone].cmd[cmd_no]

void
reset_zone (int zone)
{
  int cmd_no;
  int count_vnum_in_room;
  int i;
  int current_room = -1;
  CHAR_DATA *mob = NULL;
  CHAR_DATA *tmob = NULL;
  OBJ_DATA *obj = NULL;
  OBJ_DATA *tobj;
  SUBCRAFT_HEAD_DATA *craft;
  AFFECTED_TYPE *af;
  RESET_AFFECT *ra;
  RESET_DATA *reset;
  char buf[MAX_STRING_LENGTH];

  extern bool bIsCopyOver;	// Defined in comm.c

  if (!zone_table[zone].cmd)
    return;

  for (cmd_no = 0;; cmd_no++)
    {

      if (ZCMD.command == 'S')
	break;

      if (ZCMD.command == 'M')
	{			/* Mob to room */

	  mob = NULL;

	  if (!ZCMD.enabled)
	    continue;

	  ZCMD.enabled = 0;

	  if ( /*(!engine.in_play_mode ()) && */ (mob = load_mobile (ZCMD.arg1)))
	    {
	      mob->mob->reset_zone = zone;
	      mob->mob->reset_cmd = cmd_no;

	      if (!mob->height && !IS_SET (mob->flags, FLAG_VARIABLE))
		make_height (mob);

	      if (!mob->frame)
		make_frame (mob);

	      mob->mob->spawnpoint = ZCMD.arg3;
	      char_to_room (mob, ZCMD.arg3);

	      if (ZCMD.arg4 && (tmob = load_mobile (ZCMD.arg4)))
		{
		  tmob->mount = mob;
		  mob->mount = tmob;
		  tmob->mob->spawnpoint = ZCMD.arg3;
		  char_to_room (tmob, ZCMD.arg3);
		}
	    }
	  else if (!bIsCopyOver)
	    {
	      sprintf (buf, "Unable to load mob virtual %d!", ZCMD.arg1);
	      system_log (buf, true);
	    }
	}

      else if (ZCMD.command == 'R')
	{			/* Defining room */
	  current_room = ZCMD.arg1;
	  continue;
	}

      else if (ZCMD.command == 'A' ||	/* Affect on char */
	       ZCMD.command == 'r')
	{			/* Affect on room */

	  if (!ZCMD.arg1)
	    {
	      system_log ("ZCMD is zero.", true);
	      continue;
	    }

	  if (!mob)
	    continue;

	  ra = (RESET_AFFECT *) ZCMD.arg1;

	  if (get_affect (mob, ra->type))
	    continue;

	  af = (AFFECTED_TYPE *) alloc (sizeof (AFFECTED_TYPE), 13);

	  af->type = ra->type;
	  af->a.spell.duration = ra->duration;
	  af->a.spell.modifier = ra->modifier;
	  af->a.spell.location = ra->location;
	  af->a.spell.bitvector = ra->bitvector;
	  af->a.spell.sn = ra->sn;
	  af->a.spell.t = ra->t;
	  af->next = NULL;

	  if (ZCMD.command == 'r')
	    {
	      af->next = vtor (current_room)->affects;
	      vtor (current_room)->affects = af;
	    }
	  else
	    affect_to_char (mob, af);

	  continue;
	}

      else if (ZCMD.command == 'm')
	{

	  if (!mob)
	    continue;

	  if (ZCMD.arg1 == RESET_REPLY)
	    {
	      reset = (RESET_DATA *) alloc (sizeof (RESET_DATA), 33);

	      reset->type = RESET_REPLY;

	      reset->command = str_dup ((char *) ZCMD.arg2);

	      reset->when.month = -1;
	      reset->when.day = -1;
	      reset->when.hour = -1;
	      reset->when.minute = -1;
	      reset->when.second = -1;

	      reset_insert (mob, reset);
	    }
	}

      else if (ZCMD.command == 'C')
	{

	  if (!mob)
	    continue;

	  if (!ZCMD.arg1)
	    continue;

	  for (craft = crafts;
	       craft && str_cmp (craft->subcraft_name, (char *) ZCMD.arg1);
	       craft = craft->next)
	    ;

	  if (!craft)
	    {

	      sprintf (buf, "RESET: No such craft %s on mob %d, room %d",
		       craft->subcraft_name, mob->mob->nVirtual,
		       mob->in_room);

	      system_log (buf, true);
	    }

	  for (i = CRAFT_FIRST; i <= CRAFT_LAST; i++)
	    if (!get_affect (mob, i))
	      break;

	  magic_add_affect (mob, i, -1, 0, 0, 0, 0);

	  af = get_affect (mob, i);

	  af->a.craft =
	    (struct affect_craft_type *)
	    alloc (sizeof (struct affect_craft_type), 23);

	  af->a.craft->subcraft = craft;
	}

      else if (ZCMD.command == 'O')
	{

	  obj = NULL;

	  count_vnum_in_room = 0;

	  for (tobj = vtor (ZCMD.arg3)->contents;
	       tobj; tobj = tobj->next_content)
	    if (tobj->nVirtual == ZCMD.arg1)
	      count_vnum_in_room++;

	  if (count_vnum_in_room < ZCMD.arg2 &&
	      (obj = load_object (ZCMD.arg1)))
	    obj_to_room (obj, ZCMD.arg3);
	}

      else if (ZCMD.command == 'P')
	{
	  if (!obj)
	    continue;

	  if ((tobj = load_object (ZCMD.arg1)))
	    obj_to_obj (tobj, obj);
	}

      else if (ZCMD.command == 'G')
	{

	  obj = NULL;

	  if (!mob)
	    continue;

	  if ((obj = load_object (ZCMD.arg1)))
	    {

	      if (obj->nVirtual == VNUM_PENNY)
		{
		  obj->count = ZCMD.arg2;
		  name_money (obj);
		}

	      if (obj->nVirtual == VNUM_PENNY &&
		  IS_SET (mob->flags, FLAG_KEEPER))
		{
		  extract_obj (obj);
		  obj = NULL;
		}
	      else
		obj_to_char (obj, mob);
	    }
	}

      else if (ZCMD.command == 'E')
	{

	  obj = NULL;

	  if (!mob)
	    continue;

	  if ((obj = load_object (ZCMD.arg1)))
	    {

	      if (IS_WEARABLE (obj))
		obj->size = get_size (mob);

	      equip_char (mob, obj, ZCMD.arg3);

	      cmd_no++;
	      if (ZCMD.command == 's')
		{
		  obj_to_obj (load_object (ZCMD.arg1), obj);
		  tobj = load_object (ZCMD.arg2);
		  if (ZCMD.arg2 > 1)
		    for (i = 1; i < ZCMD.arg2; i++)
		      obj_to_obj (load_object (ZCMD.arg1), obj);
		}
	      else
		cmd_no--;

	    }
	}

      else if (ZCMD.command == 'a')
	{

	  obj = NULL;

	  if (!tmob)
	    continue;

	  if ((obj = load_object (ZCMD.arg1)))
	    {

	      if (IS_WEARABLE (obj))
		obj->size = get_size (tmob);

	      equip_char (tmob, obj, ZCMD.arg3);

	      cmd_no++;
	      if (ZCMD.command == 's')
		{
		  obj_to_obj (load_object (ZCMD.arg1), obj);
		  tobj = load_object (ZCMD.arg1);
		  if (ZCMD.arg2 > 1)
		    for (i = 1; i < ZCMD.arg2; i++)
		      obj_to_obj (load_object (ZCMD.arg1), obj);
		}
	      else
		cmd_no--;
	    }
	}

      else if (ZCMD.command == 'D')
	{
	  set_door_state (ZCMD.arg1, ZCMD.arg2, (exit_state) ZCMD.arg3);
	}
    }				/* for */

  zone_table[zone].age = 0;
}

#undef ZCMD

void
list_validate (char *name)
{
  CHAR_DATA *ch;
  OBJ_DATA *obj;
  int cycle_count;
  char buf[MAX_STRING_LENGTH];

  sprintf (buf, "List validate:  %s entered.\n", name);
  system_log (buf, false);

  for (ch = character_list, cycle_count = 0; ch; ch = ch->next)
    {
      if (cycle_count++ > 10000)
	{
	  system_log ("Character list cycle failed.", true);
	  ((int *) 0)[-1] = 0;
	}
    }

  for (obj = object_list, cycle_count = 0; obj; obj = obj->next)
    {
      if (cycle_count++ > 10000)
	{
	  system_log ("Object list cycle failed.", true);
	  ((int *) 0)[-1] = 0;
	}
    }

  sprintf (buf, "List validate:  %s completed.\n", name);
  system_log (buf, false);
}

void
cleanup_the_dead (int mode)
{
  OBJ_DATA *obj;
  OBJ_DATA *next_obj = NULL;
  OBJ_DATA *prev_obj = NULL;
  CHAR_DATA *ch;
  CHAR_DATA *next_ch;
  CHAR_DATA *prev_ch = NULL;

  if (mode == 1 || mode == 0)
    {
      for (ch = character_list; ch; ch = next_ch)
	{

	  next_ch = ch->next;

	  if (!ch->deleted)
	    {
	      prev_ch = ch;
	      continue;
	    }

	  if (ch == character_list)
	    character_list = next_ch;
	  else
	    prev_ch->next = next_ch;

	  if (!IS_NPC (ch))
	    {
	      unload_pc (ch);
	      continue;
	    }

	  free_char (ch);

	  ch = NULL;
	}
    }

  if (mode == 2 || mode == 0)
    {
      for (obj = object_list; obj; obj = next_obj)
	{

	  next_obj = obj->next;

	  if (!obj->deleted)
	    {
	      prev_obj = obj;
	      continue;
	    }

	  if (obj == object_list)
	    object_list = next_obj;
	  else
	    prev_obj->next = next_obj;

	  if (obj)
	    free_obj (obj);

	  obj = NULL;
	}
    }
}

#define ZCMD zone_table[zone].cmd[cmd_no]

void
refresh_zone (void)
{
  int cmd_no, i;
  int count_vnum_in_room;
  static int zone = 0;
  CHAR_DATA *mob = NULL, *tmob = NULL;
  ROOM_DATA *room;
  OBJ_DATA *obj = NULL;
  OBJ_DATA *tobj;
  char buf[MAX_STRING_LENGTH];

  if (!zone_table[zone].cmd)
    return;

  if (IS_FROZEN (zone))
    return;

  sprintf (buf, "Refreshing zone %d and loading any unloaded psaves...",
	   zone);
  system_log (buf, false);

  for (room = full_room_list; room; room = room->lnext)
    {
      if (room->zone != zone)
	continue;
      if (!room->psave_loaded)
	load_save_room (room);
    }

  for (cmd_no = 0;; cmd_no++)
    {

      if (ZCMD.command == 'S')
	break;

      if (ZCMD.command == 'D')
	continue;

      if (ZCMD.command == 'M')
	{

	  mob = NULL;

	  if (!ZCMD.enabled)
	    continue;

	  ZCMD.enabled = 0;

	  if ((mob = load_mobile (ZCMD.arg1)))
	    {
	      mob->mob->reset_zone = zone;
	      mob->mob->reset_cmd = cmd_no;
	      char_to_room (mob, ZCMD.arg3);
	    }

/*			act ("$n has arrived.", true, mob, 0, 0, TO_ROOM | _ACT_FORMAT);*/

	  if (ZCMD.arg4 && (tmob = load_mobile (ZCMD.arg4)))
	    {
	      tmob->mount = mob;
	      mob->mount = tmob;
	      tmob->mob->spawnpoint = ZCMD.arg3;
	      char_to_room (tmob, ZCMD.arg3);
	    }
	}

      else if (ZCMD.command == 'O')
	{

	  obj = NULL;

	  count_vnum_in_room = 0;

	  for (tobj = vtor (ZCMD.arg3)->contents;
	       tobj; tobj = tobj->next_content)
	    if (tobj->nVirtual == ZCMD.arg1)
	      count_vnum_in_room++;

	  if (count_vnum_in_room < ZCMD.arg2 &&
	      (obj = load_object (ZCMD.arg1)))
	    obj_to_room (obj, ZCMD.arg3);
	}

      else if (ZCMD.command == 'P')
	{
	  if (!obj)
	    continue;

	  if ((tobj = load_object (ZCMD.arg1)))
	    obj_to_obj (tobj, obj);
	}

      else if (ZCMD.command == 'G')
	{

	  obj = NULL;

	  if (!mob)
	    continue;

	  if ((obj = load_object (ZCMD.arg1)))
	    {
	      if (obj->nVirtual == VNUM_MONEY)
		{
		  obj->o.od.value[0] = ZCMD.arg2;
		  obj->obj_flags.set_cost = ZCMD.arg2; // ??
		  obj->obj_flags.weight = ZCMD.arg2;
		  name_money (obj);
		}
	      obj_to_char (obj, mob);
	    }
	}

      else if (ZCMD.command == 'E')
	{

	  obj = NULL;

	  if (!mob)
	    continue;

	  if ((obj = load_object (ZCMD.arg1)))
	    equip_char (mob, obj, ZCMD.arg3);
	}

      else if (ZCMD.command == 'a')
	{

	  obj = NULL;

	  if (!tmob)
	    continue;

	  if ((obj = load_object (ZCMD.arg1)))
	    {

	      if (IS_WEARABLE (obj))
		obj->size = get_size (tmob);

	      equip_char (tmob, obj, ZCMD.arg3);

	      cmd_no++;
	      if (ZCMD.command == 's')
		{
		  obj_to_obj (load_object (ZCMD.arg1), obj);
		  tobj = load_object (ZCMD.arg1);
		  if (ZCMD.arg2 > 1)
		    for (i = 1; i < ZCMD.arg2; i++)
		      obj_to_obj (load_object (ZCMD.arg1), obj);
		}
	      else
		cmd_no--;
	    }
	}

    }				/* for */

  if (zone + 1 >= MAX_ZONE)
    zone = 0;
  else
    zone++;
}

#undef ZCMD
