CLAN_DATA *clan_list = NULL;

/*
|     1487 | Minas Morgul               | mm_denizens
|     1736 | Osgiliath City Watch       | osgi_watch
|     2008 | Malred Family              | housemalred
|     3899 | Gothakra Warband           | gothakra
|     4174 | Rogues' Fellowship         | rogues
|     8026 | The Twisted Eye of Khagdu  | khagdu
|    11723 | Ithilien Battalion         | ithilien_battalion
|    14181 | Hawk and Dove              | hawk_dove_2
|    12865 | Astirian Villeins          | astirian_villeins
*/

char *
get_clan_rank_name (CHAR_DATA *ch, char * clan, int flags)
{
  if (flags == CLAN_LEADER)
  {
    return "Leadership";
  }
  else if (flags == CLAN_RECRUIT)
  {
    if (!str_cmp (clan, "gothakra"))
    {
      if (ch->race == lookup_race_id("Orc"))
      {
        return "Snaga Uruk";
      }
      else
      {
        return "Snaga";
      }
    }
    if (!str_cmp (clan, "seekers"))
    {
      return "Squire";
    }
    if (!str_cmp (clan, "shadow-cult"))
    {
      return "Initiate";
    }
    if (!str_cmp (clan, "khagdu"))
    {
      return "Push-Khur";
    }
    if (!str_cmp (clan, "eradan_battalion") || !str_cmp (clan, "ithilien_battalion"))
    {
      return "Liegeman";
    }
    return "Recruit";
  }
  else if (flags == CLAN_PRIVATE)
  {
    if (!str_cmp (clan, "gothakra"))
    {
      return "Uruk";
    }
    if (!str_cmp (clan, "seekers"))
    {
      return "Apprentice Seeker-Knight";
    }
    if (!str_cmp (clan, "shadow-cult"))
    {
      return "Acolyte";
    }
    if (!str_cmp (clan, "khagdu"))
    {
      return "Khur";
    }
    if (!str_cmp (clan, "eradan_battalion") || !str_cmp (clan, "ithilien_battalion"))
    {
      return "Footman";
    }
    if (!str_cmp (clan, "tirithguard"))
    {
      return "Ohtar";
    }
    return "Private";
  }
  else if (flags == CLAN_CORPORAL)
  {
    if (!str_cmp (clan, "gothakra"))
    {
      return "Zuruk";
    }
    if (!str_cmp (clan, "seekers"))
    {
      return "Seeker-Knight";
    }
    if (!str_cmp (clan, "khagdu"))
    {
      return "Gur";
    }
    if (!str_cmp (clan, "eradan_battalion") || !str_cmp (clan, "ithilien_battalion"))
    {
      return "Armsman";
    }
    if (!str_cmp (clan, "tirithguard"))
    {
      return "Roquen";
    }
    return "Corporal";
  }
  else if (flags == CLAN_SERGEANT)
  {
    if (!str_cmp (clan, "khagdu"))
    {
      return "Gurash";
    }
    return "Sergeant";
  }
  else if (flags == CLAN_LIEUTENANT)
  {
    if (!str_cmp (clan, "gothakra"))
    {
      return "Ba'Zaak";
    }
    if (!str_cmp (clan, "seekers"))
    {
      return "Knight-Captain";
    }
    if (!str_cmp (clan, "khagdu"))
    {
      return "Bughrak";
    }
    if (!str_cmp (clan, "eradan_battalion") || !str_cmp (clan, "ithilien_battalion") || !str_cmp (clan, "tirithguard"))
    {
      return "Constable";
    }
    return "Lieutenant";
  }
  else if (flags == CLAN_CAPTAIN)
  {
    if (!str_cmp (clan, "seekers"))
    {
      return "Knight-General";
    }
    if (!str_cmp (clan, "shadow-cult"))
    {
      return "Barun An-Nalo";
    }
    if (!str_cmp (clan, "khagdu"))
    {
      return "Duumul-Bughrak";
    }
    if (!str_cmp (clan, "eradan_battalion") || !str_cmp (clan, "ithilien_battalion"))
    {
      return "Lord";
    }
    return "Captain";
  }
  else if (flags == CLAN_GENERAL)
  {
    if (!str_cmp (clan, "seekers"))
    {
      return "Knight-Commander";
    }
    if (!str_cmp (clan, "khagdu"))
    {
      return "Gurashul";
    }
    if (!str_cmp (clan, "tirithguard"))
    {
      return "Marshall";
    }
    return "General";
  }
  else if (flags == CLAN_COMMANDER)
  {
    if (!str_cmp (clan, "seekers"))
    {
      return "Knight-Grand-Cross";
    }
    if (!str_cmp (clan, "khagdu"))
    {
      return "Duumul-Bughrak";
    }
    return "Commander";
  }
  else if (flags == CLAN_APPRENTICE)
  {
    if (!str_cmp (clan, "mordor_char"))
    {
      return "Kadar-Lai";
    }
    return "Apprentice";
  }
  else if (flags == CLAN_JOURNEYMAN)
  {
    if (!str_cmp (clan, "khagdu"))
    {
      return "Yameg";
    }
    if (!str_cmp (clan, "mordor_char"))
    {
      return "Khor";
    }
    return "Journeyman";
  }
  else if (flags == CLAN_MASTER)
  {
    if (!str_cmp (clan, "khagdu"))
    {
      return "Yameg-Khur";
    }
    if (!str_cmp (clan, "mordor_char"))
    {
      return "Black Watchman";
    }
    return "Master";
  }
  else if (flags > 0)
  {
    if (!str_cmp (clan, "mordor_char"))
    {
      return "Freeman";
    }
    return "Membership";
  }

  return NULL;
}

int
clan_flags_to_value (char *flag_names, char *clan_name)
{
  int flags = 0;
  char buf[MAX_STRING_LENGTH];

  while (1)
    {
      flag_names = one_argument (flag_names, buf);
      if (!*buf)
        break;
      else if (!str_cmp (buf, "recruit")
      || ((!str_cmp (buf, "snaga") || !str_cmp (buf, "snaga-uruk"))
          && !str_cmp (clan_name, "gothakra"))
      || ((!str_cmp (buf, "initiate"))
          && !str_cmp (clan_name, "shadow-cult"))
      || (!str_cmp (buf, "squire")
          && !str_cmp (clan_name, "seekers"))
      || (!str_cmp (buf, "push-khur")
          && !str_cmp (clan_name, "khagdu"))
      || (!str_cmp (buf, "liegeman")
          && (!str_cmp (clan_name, "eradan_battalion") || !str_cmp (clan_name, "ithilien_battalion"))))
        flags |= CLAN_RECRUIT;
      else if (!str_cmp (buf, "private")
      || (!str_cmp (buf, "ohtar")
          && !str_cmp (clan_name, "tirithguard"))
      || (!str_cmp (buf, "apprentice-seeker-knight")
          && !str_cmp (clan_name, "seekers"))
      || (!str_cmp (buf, "khur")
          && !str_cmp (clan_name, "khagdu")))
        flags |= CLAN_PRIVATE;
      else if (!str_cmp (buf, "corporal")
      || (!str_cmp (buf, "roquen")
          && !str_cmp (clan_name, "tirithguard"))
      || (!str_cmp (buf, "seeker-knight")
          && !str_cmp (clan_name, "seekers"))
      || (!str_cmp (buf, "gur")
          && !str_cmp (clan_name, "khagdu")))
        flags |= CLAN_CORPORAL;
      else if (!str_cmp (buf, "captain")
      || (!str_cmp (buf, "barun-an-nalo")
          && !str_cmp (clan_name, "shadow-cult"))
      || (!str_cmp (buf, "lord")
          && (!str_cmp (clan_name, "eradan_battalion") || !str_cmp (clan_name, "ithilien_battalion"))))
        flags |= CLAN_CAPTAIN;
      else if (!str_cmp (buf, "journeyman"))
        flags |= CLAN_JOURNEYMAN;
      else if (!str_cmp (buf, "master")
      || (!str_cmp (buf, "yameg-khur")
          && !str_cmp (clan_name, "khagdu")))
        flags |= CLAN_MASTER;
      else if (!str_cmp (buf, "member"))
        flags |= CLAN_MEMBER;
    }

  return flags;
}
