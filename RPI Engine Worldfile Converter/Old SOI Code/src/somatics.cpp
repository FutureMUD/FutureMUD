/*------------------------------------------------------------------------\
|  somatics.c : Short and Long Term Somatic Effects   www.middle-earth.us |
|  Copyright (C) 2004, Shadows of Isildur: Sighentist                     |
|  Derived under license from DIKU GAMMA (0.0).                           |
\------------------------------------------------------------------------*/


#include <stdio.h>
#include <time.h>

#ifndef MACOSX
#include <malloc.h>
#endif

#include <string.h>
#include <stdlib.h>
#include <ctype.h>
#include <unistd.h>

#include "structs.h"
#include "net_link.h"
#include "account.h"
#include "utils.h"
#include "protos.h"
#include "decl.h"
#include "utility.h"

void
soma_stat (CHAR_DATA * ch, AFFECTED_TYPE * af)
{
  char buf[MAX_STRING_LENGTH];
  char buf2[MAX_STRING_LENGTH];

  switch (af->type)
    {
    case SOMA_MUSCULAR_CRAMP:
      sprintf (buf2, "a muscle cramp");
      break;
    case SOMA_MUSCULAR_TWITCHING:
      sprintf (buf2, "twitching");
      break;
    case SOMA_MUSCULAR_TREMOR:
      sprintf (buf2, "tremors");
      break;
    case SOMA_MUSCULAR_PARALYSIS:
      sprintf (buf2, "paralysis");
      break;
    case SOMA_DIGESTIVE_ULCER:
      sprintf (buf2, "stomach ulcer");
      break;
    case SOMA_DIGESTIVE_VOMITING:
      sprintf (buf2, "vomiting");
      break;
    case SOMA_DIGESTIVE_BLEEDING:
      sprintf (buf2, "vomiting blood");
      break;
    case SOMA_EYE_BLINDNESS:
      sprintf (buf2, "blindness");
      break;
    case SOMA_EYE_BLURRED:
      sprintf (buf2, "blurred vision");
      break;
    case SOMA_EYE_DOUBLE:
      sprintf (buf2, "double vision");
      break;
    case SOMA_EYE_DILATION:
      sprintf (buf2, "dilated pupils");
      break;
    case SOMA_EYE_CONTRACTION:
      sprintf (buf2, "contracted pupils");
      break;
    case SOMA_EYE_LACRIMATION:
      sprintf (buf2, "lacrimation");
      break;
    case SOMA_EYE_PTOSIS:
      sprintf (buf2, "ptosis");
      break;
    case SOMA_EAR_TINNITUS:
      sprintf (buf2, "tinnitus");
      break;
    case SOMA_EAR_DEAFNESS:
      sprintf (buf2, "deafness");
      break;
    case SOMA_EAR_EQUILLIBRIUM:
      sprintf (buf2, "ear imbalance");
      break;
    case SOMA_NOSE_ANOSMIA:
      sprintf (buf2, "anosmia");
      break;
    case SOMA_NOSE_RHINITIS:
      sprintf (buf2, "rhinitis");
      break;
    case SOMA_MOUTH_SALIVATION:
      sprintf (buf2, "salivation");
      break;
    case SOMA_MOUTH_TOOTHACHE:
      sprintf (buf2, "toothache");
      break;
    case SOMA_MOUTH_DRYNESS:
      sprintf (buf2, "dry mouth");
      break;
    case SOMA_MOUTH_HALITOSIS:
      sprintf (buf2, "halitosis");
      break;
    case SOMA_CHEST_DIFFICULTY:
      sprintf (buf2, "difficulty breathing");
      break;
    case SOMA_CHEST_WHEEZING:
      sprintf (buf2, "wheezing");
      break;
    case SOMA_CHEST_RAPIDBREATH:
      sprintf (buf2, "rapid breathing");
      break;
    case SOMA_CHEST_SLOWBREATH:
      sprintf (buf2, "shallow breathing");
      break;
    case SOMA_CHEST_FLUID:
      sprintf (buf2, "fluidous lungs");
      break;
    case SOMA_CHEST_PALPITATIONS:
      sprintf (buf2, "heart palpitations");
      break;
    case SOMA_CHEST_COUGHING:
      sprintf (buf2, "coughing fits");
      break;
    case SOMA_CHEST_PNEUMONIA:
      sprintf (buf2, "pneumonia");
      break;
    case SOMA_NERVES_PSYCHOSIS:
      sprintf (buf2, "psychosis");
      break;
    case SOMA_NERVES_DELIRIUM:
      sprintf (buf2, "delerium ");
      break;
    case SOMA_NERVES_COMA:
      sprintf (buf2, "a comatose state");
      break;
    case SOMA_NERVES_CONVULSIONS:
      sprintf (buf2, "convulsions");
      break;
    case SOMA_NERVES_HEADACHE:
      sprintf (buf2, "a headache");
      break;
    case SOMA_NERVES_CONFUSION:
      sprintf (buf2, "confusion");
      break;
    case SOMA_NERVES_PARETHESIAS:
      sprintf (buf2, "parethesias");
      break;
    case SOMA_NERVES_ATAXIA:
      sprintf (buf2, "ataxia");
      break;
    case SOMA_NERVES_EQUILLIBRIUM:
      sprintf (buf2, "nervous imbalance");
      break;
    case SOMA_SKIN_CYANOSIS:
      sprintf (buf2, "cyanosis of the skin");
      break;
    case SOMA_SKIN_DRYNESS:
      sprintf (buf2, "dryness of the skin");
      break;
    case SOMA_SKIN_CORROSION:
      sprintf (buf2, "corrosion of the skin");
      break;
    case SOMA_SKIN_JAUNDICE:
      sprintf (buf2, "jaundice of the skin");
      break;
    case SOMA_SKIN_REDNESS:
      sprintf (buf2, "redness of the skin");
      break;
    case SOMA_SKIN_RASH:
      sprintf (buf2, "a rash on the skin");
      break;
    case SOMA_SKIN_HAIRLOSS:
      sprintf (buf2, "hairloss");
      break;
    case SOMA_SKIN_EDEMA:
      sprintf (buf2, "edema of the skin");
      break;
    case SOMA_SKIN_BURNS:
      sprintf (buf2, "burns on the skin");
      break;
    case SOMA_SKIN_PALLOR:
      sprintf (buf2, "pallor of the skin");
      break;
    case SOMA_SKIN_SWEATING:
      sprintf (buf2, "the sweats");
      break;
    case SOMA_GENERAL_WEIGHTLOSS:
      sprintf (buf2, "weight loss");
      break;
    case SOMA_GENERAL_LETHARGY:
      sprintf (buf2, "lethargy");
      break;
    case SOMA_GENERAL_APPETITELOSS:
      sprintf (buf2, "appetite loss");
      break;
    case SOMA_GENERAL_PRESSUREDROP:
      sprintf (buf2, "low blood pressure");
      break;
    case SOMA_GENERAL_PRESSURERISE:
      sprintf (buf2, "high blood pressure");
      break;
    case SOMA_GENERAL_FASTPULSE:
      sprintf (buf2, "a fast pulse");
      break;
    case SOMA_GENERAL_SLOWPULSE:
      sprintf (buf2, "a slow pulse");
      break;
    case SOMA_GENERAL_HYPERTHERMIA:
      sprintf (buf2, "hyperthermia");
      break;
    case SOMA_GENERAL_HYPOTHERMIA:
      sprintf (buf2, "hypothermia");
      break;

// effects caused by blunt weapons

    case SOMA_BLUNT_MEDHEAD:
      sprintf (buf2, "minorly stunned");
      break;
    case SOMA_BLUNT_SEVHEAD:
      sprintf (buf2, "a concussion");
      break;
    case SOMA_BLUNT_R_SEVARM:
      sprintf (buf2, "a broken right arm");
      break;
    case SOMA_BLUNT_L_SEVARM:
      sprintf (buf2, "a broken left arm");
      break;
    case SOMA_BLUNT_SEVLEG:
      sprintf (buf2, "a broken leg");
      break;
    case SOMA_BLUNT_SEVBODY:
      sprintf (buf2, "a broken rib");
      break;
    case SOMA_SPIDER_PARALYZE:
      sprintf (buf2, "paralysing spider venom");
      break;
    case SOMA_SPIDER_NECROTIC:
      sprintf(buf2, "XXXX");
      break;
    case SOMA_SPIDER_HIBERNATE:
      sprintf(buf2, "XXXX");
      break;
    case SOMA_SNAKE_NEUROTOXIN:
      sprintf(buf2, "nerve-wracking snake venom");
      break;
    case SOMA_SNAKE_CYTOTOXIN:
      sprintf(buf2, "necrotic snake venom");
      break;
    case SOMA_SNAKE_HEMOTOXIN:
      sprintf(buf2, "afflicting snake venom");
      break;
    case SOMA_TOAD_POISON:
      sprintf(buf2, "XXXX");
      break;
    case SOMA_TOAD_VISIONS:
      sprintf(buf2, "XXXX");
      break;
    case SOMA_PLANT_DROWSINESS:
      sprintf(buf2, "XXXX");
      break;
    case SOMA_PLANT_SLEEP:
      sprintf(buf2, "concentrated dream moss");
      break;
    case SOMA_PLANT_BLEEDING:
      sprintf(buf2, "vicious snowfruit");
      break;
    case SOMA_PLANT_NASEUA:
      sprintf(buf2, "XXXX");
      break;
    case SOMA_PLANT_BLINDNESS:
      sprintf(buf2, "boiled nightcap mushroom");
      break;
    case SOMA_PLANT_VISIONS:
      sprintf(buf2, "mashed screamcaps");
      break;
    case SOMA_PLANT_RELAX:
      sprintf(buf2, "mashed scarbane root");
      break;
    case SOMA_PLANT_ALERTNESS:
      sprintf(buf2, "crushed bright weed");
      break;
    case SOMA_PLANT_FURY:
      sprintf(buf2, "fermented bile fruit");
      break;
    case SOMA_PLANT_SPIDERPROOF:
      sprintf(buf2, "XXXX");
      break;
    case SOMA_PLANT_WRAITHCURE:
      sprintf(buf2, "wraithcure");
      break;
    case SOMA_MAGIC_WRAITHCURSE:
      sprintf(buf2, "wraithcurse");
      break;
    case SOMA_MAGIC_SOULSPITE:
      sprintf(buf2, "soulspite");
      break;
    case SOMA_PLANT_FOCUS:
      sprintf(buf2, "benign snowfruit");
      break;
    case SOMA_MAGIC_BLANK2:
      sprintf(buf2, "XXXX");
      break;
    case SOMA_MAGIC_BLANK3:
      sprintf(buf2, "XXXX");
      break;
    case SOMA_MAGIC_BLANK4:
      sprintf(buf2, "XXXX");
      break;
    case SOMA_SNOWCOLD:
      sprintf(buf2, "frostbite");
      break;
    default:
      sprintf (buf2, "an unknown somatic effect");
      break;
    }

	send_to_char("\n", ch);
	if (!IS_MORTAL (ch))
		{
  sprintf (buf,
	   "#2%5d#0   Suffers from %s for %d more in-game hours.\n        Latency: %d hrs Power: %d to %d (%d @ %d min)\n        A: %d min, D: %d min, S: %d min, R: %d min\n",
	   af->type, buf2, af->a.soma.duration, af->a.soma.latency,
	   af->a.soma.max_power, af->a.soma.lvl_power, af->a.soma.atm_power,
	   af->a.soma.minute, af->a.soma.attack, af->a.soma.decay,
	   af->a.soma.sustain, af->a.soma.release);
	   }
	 else
	 {
	 sprintf (buf, "You suffer from %s.", buf2);
	 }
  send_to_char (buf, ch);
}



void
soma_ten_second_affect (CHAR_DATA * ch, AFFECTED_TYPE * af)
{
  int save = 0, stat = 0, save2 = 0;
  char *locat = NULL;
  char buf2[MAX_STRING_LENGTH];
  char buf[MAX_STRING_LENGTH];
  
  stat = GET_CON (ch);
  if ((number (1, 1000) > af->a.soma.atm_power)
      || (number (1, (stat > 25) ? stat : 25) <= stat))
    return;

  switch (af->type)
    {
    case SOMA_CHEST_COUGHING:

      stat = GET_WIL (ch);
      save = number (1, (stat > 20) ? stat : 20);

      if (get_affect (ch, MAGIC_HIDDEN) && would_reveal (ch))
	{
	  if (save > stat)
	    {
	      remove_affect_type (ch, MAGIC_HIDDEN);
	      act ("$n reveals $mself with an audible cough.", true, ch, 0, 0,
		   TO_ROOM);
	    }
	  else if (save > (stat / 2))
	    {
	      act ("You hear a muffled sound from somewhere nearby.", true,
		   ch, 0, 0, TO_ROOM);
	    }
	}
      else if ((save <= stat) && (save > (stat / 2)))
	{
	  act ("$n tries to stifle a cough.", true, ch, 0, 0, TO_ROOM);
	}

      if (save > stat)
	{
	  act ("You cough audibly.", true, ch, 0, 0, TO_CHAR);
	}
      else
	{
	  act ("You try to stifle a cough silently.", true, ch, 0, 0,
	       TO_CHAR);
	}
      break;

      case SOMA_CHEST_WHEEZING:

      stat = GET_WIL (ch);
      save = number (1, (stat > 20) ? stat : 20);

      if (get_affect (ch, MAGIC_HIDDEN) && would_reveal (ch))
	{
	  if (save > stat)
	    {
	      remove_affect_type (ch, MAGIC_HIDDEN);
	      act ("$n reveals $mself with an audible wheeze.", true, ch, 0, 0,
		   TO_ROOM);
	    }
	  else if (save > (stat / 2))
	    {
	      act ("You hear a muffled sound from somewhere nearby.", true,
		   ch, 0, 0, TO_ROOM);
	    }
	}
      else if ((save <= stat) && (save > (stat / 2)))
	{
	  act ("$n tries to stifle their wheezing.", true, ch, 0, 0, TO_ROOM);
	}

      if (save > stat)
	{
	  act ("You wheeze audibly.", true, ch, 0, 0, TO_CHAR);
	}
      else
	{
	  act ("You try to stifle your wheezing.", true, ch, 0, 0,
	       TO_CHAR);
	}
      break;


      case SOMA_NERVES_HEADACHE:

      stat = GET_WIL (ch);
      save = number (1, (stat > 20) ? stat : 20);

      if (save > stat)
	{
	  act ("Your head pounds with a headache.", true, ch, 0, 0, TO_CHAR);
	}
      break;

      case SOMA_DIGESTIVE_ULCER:

      stat = GET_CON (ch);
      save = number (1, (stat > 24) ? stat : 24);

       if(save > stat)
       {
         switch (number(0,7))
         {
           case 0:
             send_to_char("Your stomach churns and rolls, a wave of nausea washing over your body.\n", ch);
             break;
           case 1:
             send_to_char("Sharp cramps stab your abdomen, your stomach twitching and writhing.\n",  ch);
             break;
           case 2:
             send_to_char("You feel bile rise in your throat, and you struggle to keep from vomiting.\n",  ch);
             break;
           case 3:
             send_to_char("A sickly chill tingles through your body, your stomach roiling in protest.\n",  ch);
             break;
           case 4:
             send_to_char("Gurgles and growls emit from your upset stomach.\n",  ch);
             break;
           case 5:
             send_to_char("You can feel your innards writhing and churning.\n",  ch);
             break;
           case 6:
             send_to_char("A hollow sickness pervades your body.\n",  ch);
             break;
           default:
             send_to_char("A nauseating feeling of hollowness fills your gut.\n", ch);
           break;
         }        
       }

      break;

	case SOMA_MUSCULAR_CRAMP:
	
			stat = GET_WIL (ch);
      save = number (1, (stat > 20) ? stat : 20);
			locat = expand_wound_loc(figure_location(ch, number(0,2)));

      if (save > stat)
				{
				sprintf(buf, "You get an intense cramp in your %s which persist for several minutes beofre finally relaxing\n", locat);
	  		act (buf, true, ch, 0, 0, TO_CHAR);
	  		 act ("$n suddenly cringes in pain.", true, ch, 0, 0, TO_ROOM);
			act ("$n tries to stifle a cough.", true, ch, 0, 0, TO_ROOM);	}
      else
				{
				sprintf(buf, "You get an intense cramp in your %s, but you shake it off quickly\n", locat);
	  		act (buf, true, ch, 0, 0, TO_CHAR);
	  		act ("$n suddenly cringes in pain, but quickly recovers.", true, ch, 0, 0, TO_ROOM);
				}
      break;

     
    case SOMA_MUSCULAR_TWITCHING:
    	stat = GET_WIL (ch);
      save = number (1, (stat > 20) ? stat : 20);
			locat = expand_wound_loc(figure_location(ch, number(0,2)));
			
      if (save > stat)
				{
				sprintf(buf, "You feel a strong twitching in your %s which persist for several minutes before finally relaxing\n", locat);
	  		act (buf, true, ch, 0, 0, TO_CHAR);
	  		sprintf(buf2, "$n suddenly cringes in pain, as $s %s twitches.", locat);
	  		act (buf2, true, ch, 0, 0, TO_ROOM);
				}
      else
				{
				sprintf(buf, "You feel a strong twitching in your %s, but you control it quickly\n", locat);
	  		act (buf, true, ch, 0, 0, TO_CHAR);
	  		sprintf(buf2, "$n cringes in pain, as $s %s twitches momentarily.", locat);
	  		act (buf2, true, ch, 0, 0, TO_ROOM);
	  		
				}
      break;      

  // If you take a moderate or above blunt to the forehad, and fail a con test.
  // Makes you dizzy, lowers your combat rolls by 10%.
 

     case SOMA_BLUNT_MEDHEAD:
       
       switch (number(0,2))
       {
         case 0:
           send_to_char("Your vision swims, and you struggle to remain upright.\n", ch);
           break;
         case 1:
           send_to_char("Everything seems suddenly closer in your vision, and you stumble as you try to orientate yourself.\n",  ch);
           break;
         default:
           send_to_char("Your ears ring and vision blackens for a moment.\n", ch);
           break;
       }

       if(!number(0,2) && GET_POS(ch) > SIT)
       {
         act("$n stumbles back and forth on $s feet.\n", true, ch, 0, 0, TO_ROOM);
       }
       break;

     case SOMA_BLUNT_SEVLEG:
       stat = GET_WIL (ch);
       save = number (1, (stat > 20) ? stat : 20);
       if(save > stat && GET_POS(ch) > SIT)
       {
         switch (number(0,2))
         {
           case 0:
             send_to_char("You gasp in agony as white hot pain shoots up your leg.\n", ch);
             break;
           case 1:
             send_to_char("Your leg buckles beneath you, pain radiating from your wound.\n",  ch);
             break;
           default:
             send_to_char("Your vision narrows momentarily as pain flares from your leg.\n", ch);
           break;
         }        
       }
      break;

     case SOMA_BLUNT_L_SEVARM:
       stat = GET_WIL (ch);
       save = number (1, (stat > 20) ? stat : 20);
       if(save > stat)
       {
         switch (number(0,2))
         {
           case 0:
             send_to_char("You gasp in agony as white hot pain shoots up your arm.\n", ch);
             break;
           case 1:
             send_to_char("Your arm cramps and jerks, pain radiating from your wound.\n",  ch);
             break;
           default:
             send_to_char("Your vision narrows momentarily as pain flares from your arm.\n", ch);
           break;
         }        
       }
      break;

     case SOMA_BLUNT_R_SEVARM:
       stat = GET_WIL (ch);
       save = number (1, (stat > 20) ? stat : 20);
       if(save > stat)
       {
         switch (number(0,2))
         {
           case 0:
             send_to_char("You gasp in agony as white hot pain shoots up your arm.\n", ch);
             break;
           case 1:
             send_to_char("Your arm cramps and jerks, pain radiating from your wound.\n",  ch);
             break;
           default:
             send_to_char("Your vision narrows momentarily as pain flares from your arm.\n", ch);
           break;
         }        
       }
      break;

    case SOMA_BLUNT_SEVHEAD:
      stat = GET_WIL (ch);
      save = number (1, (stat > 20) ? stat : 20);     
      if(GET_POS (ch) > SIT && !IS_SUBDUEE(ch))
      {
        if (save > stat)
        {

        switch (number(0,2))
        {
          case 0:
            send_to_char("The world twisting and spinning, you fall to your knees.\n", ch);
            break;
          case 1:
            send_to_char("Everything goes dark, and you wake up face down on the ground.\n",  ch);
            break;
          default:
            send_to_char("You begin to pitch forward, unable to control yourself as you approach the ground.\n", ch);
            break;
        }

	  sprintf(buf, "$n stumbles, falling to $s knees.");
	  act (buf, true, ch, 0, 0, TO_ROOM);
          GET_POS (ch) = SIT;
          add_second_affect (SA_STAND, ((25-GET_WIL(ch))+number(1,3)), ch, NULL, NULL, 0);
        }
      }
      else
      {
        switch (number(0,2))
        {
          case 0:
            send_to_char("Your vision swims, and you struggle to remain upright.\n", ch);
            break;
          case 1:
            send_to_char("Suddenly, everything seems closer, and you stumble to keep standing.\n",  ch);
            break;
          default:
            send_to_char("Your ears ring and vision blackens for a moment.\n", ch);
            break;
        }
        if(!number(0,2) && GET_POS(ch) > SIT)
        {
          act("$n stumbles back and forth on $s feet.\n", true, ch, 0, 0, TO_ROOM);
        }
      }
      break; 

    // Bright weed - makes you more active, but you hunger at a faster rate too.

    case SOMA_PLANT_ALERTNESS:
      stat = GET_CON (ch);
      save = number (1, (stat > 20) ? stat : 20);     
      if (save > stat)
      {
      switch (number(0,4))
      {
        case 0:
          send_to_char("You feel alive and active, your head clear and mind sharp.\n", ch);
          break;
        case 1:
          send_to_char("Your ears fill with the sound of your own panting and heartbeat, your.\nbreath and pulse quickening.\n",  ch);
          break;
        case 2:
          send_to_char("Your belly churns hollow, the sensation matched by the clarity you feel in your bones.\n", ch);
          break;
        case 3:
          send_to_char("The palms of your hands continue to sweat, and saliva continues to gather in your mouth.\n", ch);
          break;
        default:
          send_to_char("You feel as if you can barely contain the energy within you, hands constantly shaking.\n", ch);
          break;
      }

      if(!number(0,3) && GET_POS(ch) > SIT)
      {
        if(number(0,1))
          act("$n visibly sweats.\n", true, ch, 0, 0, TO_ROOM);
        else
          act("$n's hands twitch and jerk.\n", true, ch, 0, 0, TO_ROOM);
      }
      }

      break; 


    // Poison Snow Fruit - you bleed out, and fast.

    case SOMA_PLANT_BLEEDING:
      stat = GET_CON (ch);
      save = number (1, (stat > 20) ? stat : 20);     
      if (save > stat)
      {
      switch (number(0,4))
      {
        case 0:
          send_to_char("You feel the life slowly drain from your body.\n", ch);
          break;
        case 1:
          send_to_char("Your extremeties and limbs slowly turn numb.\n",  ch);
          break;
        case 2:
          send_to_char("Your senses grow dull, an irrational fear beginning to spread inside your mind.\n", ch);
          break;
        case 3:
          send_to_char("Your bones ache and muscles spasm.\n", ch);
          break;
        default:
          send_to_char("Every breath you take seems shallower than the last.\n", ch);
          break;
      }
      }

      break; 

    // Dream Moss: you eventually fall asleep..

    case SOMA_PLANT_SLEEP:
      stat = GET_CON (ch);
      save = number (1, (stat > 20) ? stat : 20);
      if (GET_POS(ch) > SLEEP)
      {
        switch (number(0,4))
        {
          case 0:
            send_to_char("You struggle to keep your eyes open.\n", ch);
            break;
          case 1:
            send_to_char("Your limbs feel as heavy as lead.\n",  ch);
            break;
          case 2:
            send_to_char("Your body aches, crying out for rest.\n", ch);
            break;
          case 3:
            send_to_char("Your mind dulls, all activities becoming more difficult.\n", ch);
            break;
          default:
            send_to_char("Every time your eyes close shut, even for a moment, it takes all your will to reopen them.\n", ch);
            break;
        }

        if(save > stat)
        {
          if(GET_MOVE(ch) <= 2 && !get_affect(ch, MAGIC_AFFECT_SLEEP))
          {
            send_to_char("\n. . . and finally, sleep overcomes you . . . \n", ch);
            GET_POS (ch) = SLEEP;            
            magic_add_affect (ch, MAGIC_AFFECT_SLEEP, af->a.soma.release, 0, 0, 0, 0);
          }
        }
      }
      break;

    // Terror Mushrooms - you know they're coming to get you.

    case SOMA_PLANT_VISIONS:
      stat = GET_CON (ch);
      save = number (1, (stat > 24) ? stat : 24);     
      if (save > stat && GET_POS(ch) > SLEEP)
      {
      switch (number(0,10))
      {
        case 0:
          send_to_char("You feel frightened, and instinctively know something, or someone, is after you.\n", ch);
          break;
        case 1:
          send_to_char("You suddenly spot a face marked with an evil grin directed towards you.\n",  ch);
          break;
        case 2:
          send_to_char("You hear a blade being drawn behind you.\n", ch);
          break;
        case 3:
          send_to_char("You hear an arrow whistle past your ear.\n", ch);
          break;
        case 4:
          send_to_char("A rotting stench wafts past your nose, an aroma that could only belong to an orc.\n", ch);
          break;
        case 5:
          send_to_char("From the corner of your eye you see something rapidly approach you.\n", ch);
          break;
        case 6:
          send_to_char("You smell smoke, and can faintly hear the crackle of a fire.\n", ch);
          break;
        case 7:
          send_to_char("You feel something around your ankle.\n", ch);
          break;
        case 8:
          send_to_char("You hear your name being called by a cold, eerie voice.\n", ch);
          break;
        case 9:
          send_to_char("A loud crash sounds out just behind you!\n", ch);
          break;
        default:
          send_to_char("The distant howl of wolves sounds out.\n", ch);
          break;
      }
      }

      break; 


    case SOMA_PLANT_FURY:
      if (GET_FLAG (ch, FLAG_AUTOFLEE))
        ch->flags &= ~FLAG_AUTOFLEE;

      if (GET_FLAG (ch, FLAG_PACIFIST))
        ch->flags &= ~FLAG_PACIFIST;

      if(ch->fight_mode > 1)
        ch->fight_mode = 1;

      stat = GET_CON (ch);
      save = number (1, (stat > 20) ? stat : 20);     
      if (save > stat)
      {
      switch (number(0,4))
      {
        case 0:
          send_to_char("Anger rises and surges, an almost palpalbe force you can feel in your gut.\n", ch);
          break;
        case 1:
          send_to_char("Your breathing becomes low and raspy, like a rabid wolf eying its prey.\n",  ch);
          break;
        case 2:
          send_to_char("Your vision mists at the edges, and you feel your hands clench tightly of their own volition.\n", ch);
          break;
        case 3:
          send_to_char("Images of pain, anger, rage and fury flash before your eyes, and you can taste blood.\n", ch);
          break;
        default:
          send_to_char("The urge to lash out and hit something is becoming almost more than you can handle. . .\n", ch);
          break;
      }

      if(!number(0,3) && GET_POS(ch) > SIT)
      {
        if(number(0,1))
          act("$n's hands clench and unclench, and a savage look holds in their eye.\n", true, ch, 0, 0, TO_ROOM);
        else
          act("$n's gaze narrows, their breathing becoming slow and raspy.\n", true, ch, 0, 0, TO_ROOM);
      }
      }

      break; 

    case SOMA_PLANT_RELAX:
      stat = GET_CON (ch);
      save = number (1, (stat > 20) ? stat : 20);     
      if (save > stat)
      {
      switch (number(0,3))
      {
        case 0:
          send_to_char("A warm glow suffuses your body, drawing away your pain and aches.\n", ch);
          break;
        case 1:
          send_to_char("All senses seem dulled, replaced by a content blandess.\n",  ch);
          break;
        case 2:
          send_to_char("You feel your body at work healing itself, and imagine yourself growing healthier by the moment.\n", ch);
          break;
        default:
          send_to_char("Your limbs feel weak, and all your movements are sluggish.\n", ch);
          break;
      }

      if(!number(0,3) && GET_POS(ch) == STAND)
      {
        if(number(0,1))
          act("$n lets out a contented sigh.\n", true, ch, 0, 0, TO_ROOM);
        else
          act("$n moves in a sluggish manner, swaying gently as $e stands.\n", true, ch, 0, 0, TO_ROOM);
      }
      }

      break; 

    case SOMA_PLANT_BLINDNESS:
      stat = GET_CON (ch);
      save = number (1, (stat > 20) ? stat : 20);     
      if (save > stat)
      {
      switch (number(0,3))
      {
        case 0:
          send_to_char("The pressure behind your eyes builds, and a headache pounds within your skull.\n", ch);
          break;
        case 1:
          send_to_char("Colours and light flicker before your eyes.\n",  ch);
          break;
        case 2:
          send_to_char("Your surroundings become evermore dim, and you struggle to see anything at all.\n", ch);
          break;
        default:
          send_to_char("You feel pain and pressure build behind your eyeballs.\n", ch);
          break;
      }
      }
      break; 


      case SOMA_SNOWCOLD:
        stat = GET_CON (ch);
        save = number (1, (stat > 24) ? stat : 24);
        if(af->a.soma.atm_power <= 500)
        {
          switch(number(0, 8))
          {
            case 0:
              sprintf(buf, "You feel the cold seeping in to your bones.");
              break;
            case 1:
              sprintf(buf, "A sudden chill wracks your body.");
              break;
            case 2:
              sprintf(buf, "You feel the cold seeping in to your bones.");
              break;
            case 3:
              sprintf(buf, "You breath fogs over, coming out in frosty clouds.");
              break;
            case 4:
              sprintf(buf, "The nip in the air is particularly noticeable to your nose, the tip very cold to the touch.");
              break;
            case 5:
              sprintf(buf, "Your hands begin to feel chilled, and it becomes more difficult to move them as your muscles protest the cold.");
              break;
            case 6:
              sprintf(buf, "An unexpected chill courses down your spine, causing your entire body to shiver briefly.");
              break;
            case 7:
              sprintf(buf, "Steam emerges from your mouth with each breath that you take, your lungs burning faintly from inhaling and exhaling the cold air.");
              break;
            default:
              sprintf(buf, "Your toes, even through the fabric of your boots or shoes, begin to tingle slightly, the cold touching even them.");
              break;
          }

          act(buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);

        }
        else if(af->a.soma.atm_power <= 750 && number(0,1) && IS_OUTSIDE(ch))
        {
          switch(number(0, 4))
          {
            case 0:
              sprintf(buf, "The shiver that passes over your body begins at your neck, and passes all the way to your toes, lingering longer than previous chills.");
              break;
            case 1:
              sprintf(buf, "You find that you can no longer feel the tips of your fingers or your toes as the cold sinks through your skin, all the way to the bone.");
              break;
            case 2:
              sprintf(buf, "The air seems to chill you all the way to the skin, as if you were standing outside completely bare.");
              break;
            case 3:
              sprintf(buf, "Your knees stiffen as the cold reaches fully through your clothing or armor, making it harder for you to move.");
              break;
            default:
              sprintf(buf, "A dull ache lingers on your joints - knees, elbows, wrists, shoulders - as your time in the cold is prolonged. They seem unable to get warm.");
              break;
          }

          act(buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);

          if(!number(0,2))
            wound_to_char (ch, figure_location (ch, number(5,6)), 1, 5, 0, 0, 0);

        }
        else if(af->a.soma.atm_power >= 750 && number(0,1) && IS_OUTSIDE(ch))
        {
          switch(number(0, 4))
          {
            case 0:
              sprintf(buf, "A sharp pain seers through your extremities, though the numbness from the cold allows it to fade almost as quickly as it comes.");
              break;
            case 1:
              sprintf(buf, "Your lungs seem to be hardening, and you have a hard time breathing the cold air. Even the back of your throat is aching in pain from the very process.");
              break;
            case 2:
              sprintf(buf, "Your joints stiffen to the point where every movement is a very painful struggle. Even your feet seem to drag along the ground.");
              break;
            case 3:
              sprintf(buf, "You begin losing all feeling in your body, except the occasional stab of pain as your clothing rubs against your frozen skin.");
              break;
            default:
              sprintf(buf, "You blink, your eyelids nearly fusing shut with the movement as you struggle to open them. You seem suddenly exhausted, every movement difficult and painful.");
              break;
          }
          
          act(buf, false, ch, 0, 0, TO_CHAR | _ACT_FORMAT);

          wound_to_char (ch, figure_location (ch, number(5,6)), 1, 5, 0, 0, 0);
          if(number(0,1))
            wound_to_char (ch, figure_location (ch, number(5,6)), 1, 5, 0, 0, 0);
        }
        break;
   

  // Paralyzing spider poison. Adds combat delays here, if you really fail it
  // knocks you out, screws up your movement and adds a -10 drop to all skills.

    case SOMA_SPIDER_PARALYZE:

      stat = GET_WIL (ch);
      save = number (1, (stat > 20) ? stat : 20);
      save2 = number (1, (stat > 20) ? stat : 20);
      locat = expand_wound_loc(figure_location(ch, number(0,2)));
			
      if (save > stat)
      {

        if((save2 == 19 || save2 == 20) && (GET_POS (ch) > SIT))
        {
          if (get_affect (ch, MAGIC_HIDDEN) && would_reveal (ch))
            remove_affect_type (ch, MAGIC_HIDDEN);

          sprintf(buf, "Your %s violently twitches, and you fall to the ground in agony.\n", locat);
	  act (buf, true, ch, 0, 0, TO_CHAR);
	  sprintf(buf2, "$n twitches violently, limbs spasming as $e collapses to the ground.");
	  act (buf2, true, ch, 0, 0, TO_ROOM);
          GET_POS (ch) = SIT;
          add_second_affect (SA_STAND, ((25-GET_AGI(ch))+number(1,3)), ch, NULL, NULL, 0);
        }
        else
        {
          if (get_affect (ch, MAGIC_HIDDEN) && would_reveal (ch))
            remove_affect_type (ch, MAGIC_HIDDEN);

          sprintf(buf, "You feel a strong twitching in your %s, and you struggle to move your limbs at all.\n", locat);
	  act (buf, true, ch, 0, 0, TO_CHAR);
	  sprintf(buf2, "$n suddenly cringes in pain, as $s %s twitches.", locat);
	  act (buf2, true, ch, 0, 0, TO_ROOM);
        }
      
        if(ch->primary_delay > 0)     
        ch->primary_delay += number(1, 8);
        if(ch->secondary_delay > 0)
        ch->secondary_delay += number(1,8);
      }
      else
      {
        sprintf(buf, "You feel a strong twitching in your %s, but you control it quickly\n", locat);
	act (buf, true, ch, 0, 0, TO_CHAR);
        if(number(0,1))
        {
	  sprintf(buf2, "$n cringes in pain, as $s %s twitches momentarily.", locat);
	  act (buf2, true, ch, 0, 0, TO_ROOM);
        }
      }
      break;   


    default:
      break;
    }
}


void
soma_rl_minute_affect (CHAR_DATA * ch, AFFECTED_TYPE * af)
{
  unsigned short int minute = ++af->a.soma.minute;
  unsigned short int max_power = af->a.soma.max_power;
  unsigned short int lvl_power = af->a.soma.lvl_power;

  unsigned short int attack = af->a.soma.attack;
  unsigned short int decay = af->a.soma.decay;
  unsigned short int sustain = af->a.soma.sustain;
  unsigned short int release = af->a.soma.release;
  WOUND_DATA *wound;
  bool broken = false;

  switch (af->type)
    {

  // Broken bone effects: they last until the wound has become a "minor" wound.
  // If there is no wound with a corresponding value, then remove the affect.     
  case SOMA_BLUNT_SEVHEAD:
      for (wound = ch->wounds; wound; wound = wound->next)
      {
        if(wound->fracture == SOMA_BLUNT_SEVHEAD)
        {
          broken = true;
          if(!str_cmp (wound->severity, "small")
	      || !str_cmp (wound->severity, "minor"))
          {
            affect_remove (ch, af);  
            wound->fracture = 0;
          }
        }   
      }

      if(!broken)
        affect_remove (ch, af);    
    break;
  case SOMA_BLUNT_SEVBODY:
      for (wound = ch->wounds; wound; wound = wound->next)
      {
        if(wound->fracture == SOMA_BLUNT_SEVBODY)
        {
          broken = true;
          if(!str_cmp (wound->severity, "small")
	      || !str_cmp (wound->severity, "minor"))
          {
            affect_remove (ch, af);  
            wound->fracture = 0;
          }
        }   
      }

      if(!broken)
        affect_remove (ch, af);    
    break;

  case SOMA_BLUNT_SEVLEG:
      for (wound = ch->wounds; wound; wound = wound->next)
      {
        if(wound->fracture == SOMA_BLUNT_SEVLEG)
        {
          broken = true;
          if(!str_cmp (wound->severity, "small")
	      || !str_cmp (wound->severity, "minor"))
          {
            affect_remove (ch, af);  
            wound->fracture = 0;
          }
        }   
      }

      if(!broken)
        affect_remove (ch, af);    
    break;

  case SOMA_BLUNT_R_SEVARM:
      for (wound = ch->wounds; wound; wound = wound->next)
      {
        if(wound->fracture == SOMA_BLUNT_R_SEVARM)
        {
          broken = true;
          if(!str_cmp (wound->severity, "small")
	      || !str_cmp (wound->severity, "minor"))
          {
            affect_remove (ch, af);  
            wound->fracture = 0;
          }
        }   
      }

      if(!broken)
        affect_remove (ch, af);    
    break;

  case SOMA_BLUNT_L_SEVARM:
      for (wound = ch->wounds; wound; wound = wound->next)
      {
        if(wound->fracture == SOMA_BLUNT_L_SEVARM)
        {
          broken = true;
          if(!str_cmp (wound->severity, "small")
	      || !str_cmp (wound->severity, "minor"))
          {
            affect_remove (ch, af);  
            wound->fracture = 0;
          }
        }   
      }

      if(!broken)
        affect_remove (ch, af);    
    break;

  case SOMA_PLANT_BLEEDING:
    general_damage (ch, 1);
  case SOMA_MUSCULAR_CRAMP:
  case SOMA_DIGESTIVE_ULCER:
  case SOMA_MUSCULAR_TWITCHING:
  case SOMA_CHEST_COUGHING:
  case SOMA_CHEST_WHEEZING:
  case SOMA_NERVES_HEADACHE:
  case SOMA_SPIDER_PARALYZE:
  case SOMA_PLANT_ALERTNESS:
  case SOMA_PLANT_FURY:
  case SOMA_PLANT_SLEEP:
  case SOMA_PLANT_FOCUS:
  case SOMA_PLANT_VISIONS:
  case SOMA_PLANT_RELAX:
  case SOMA_PLANT_BLINDNESS:
  case SOMA_BLUNT_MEDHEAD:
  case SOMA_SNOWCOLD:

      // This is the standard "rise and fall". Four lengths - attack, decay, sustain and release.
      // Two powers - max, and level.
      // From 0 to attack, power rises to max.
      // From attack to decay, power falls from max to decay.
      // From decay to sustain, power holds.
      // From sustain to release, power falls to 0.
      // At release, remove the affect.
   
      if (minute <= attack)
	{
	  af->a.soma.atm_power = (max_power * minute) / attack;
	}
      else if (minute <= decay)
	{
	  af->a.soma.atm_power = max_power - (((max_power - lvl_power) * 
          (minute - attack)) / (decay - attack));
	}
      else if (minute <= sustain)
	{
	  af->a.soma.atm_power = lvl_power;
	}
      else if (minute <= release)
	{
	  af->a.soma.atm_power = lvl_power - (((lvl_power) * 
          (minute - sustain)) / (release - sustain));
	}
      else
	{
          // Send them the all-clear message, if the poison has one.
          send_to_char(lookup_poison_variable(af->type, 3), ch);
          affect_remove (ch, af);
	}
      break;

    default:
      break;
    }
}

int
lookup_soma (char *argument)
{
	if (!argument)
		return (-1);
		
	if (!strcmp(argument, "cramps"))
		return (SOMA_MUSCULAR_CRAMP);
	
	else if (!strcmp(argument,"twitching")) 
		return (SOMA_MUSCULAR_TWITCHING);
		
	else if (!strcmp(argument, "cough"))
		return (SOMA_CHEST_COUGHING);
		
	else if (!strcmp(argument, "wheeze"))
		return (SOMA_CHEST_WHEEZING);
		
	else if (!strcmp(argument, "headache"))
		return (SOMA_NERVES_HEADACHE);
	
	else
		return (-1);
	
}

int
soma_add_affect (CHAR_DATA * ch, int type, int duration, int latency,
		  int minute, int max_power, int lvl_power, int atm_power,
                  int attack, int decay, int sustain, int release)
{
  AFFECTED_TYPE *soma;

  // If they already have the somatic effect, add half the timers of the new
  // one to the old, and then select the maximum powers.

  if ((soma = get_affect (ch, type)))
    {

      if(soma->type == SOMA_SNOWCOLD)
      {
	soma->a.soma.duration += 1;
        soma->a.soma.attack += 1;
        soma->a.soma.decay += 1;
        soma->a.soma.sustain += 1;
        soma->a.soma.release += 1;
        soma->a.soma.max_power += max_power;
        soma->a.soma.lvl_power += lvl_power;
        soma->a.soma.atm_power = soma->a.soma.max_power;

        if(soma->a.soma.max_power > 1000)
          soma->a.soma.max_power = 1000;

        if(soma->a.soma.atm_power > 1000)
          soma->a.soma.atm_power = 1000;

        if(soma->a.soma.lvl_power > 1000)
          soma->a.soma.lvl_power = 1000;

        return 0;
      }

      if (soma->a.soma.duration == -1)	/* Perm already */
	return 0;

      if (duration == -1)
	soma->a.soma.duration = duration;
      else
      {
	soma->a.soma.duration += duration/2;
        soma->a.soma.attack += attack/2;
        soma->a.soma.decay += decay/2;
        soma->a.soma.sustain += sustain/2;
        soma->a.soma.release += release/2;
      }

      soma->a.soma.max_power = MAX ((int) soma->a.soma.max_power, max_power);
      soma->a.soma.lvl_power = MAX ((int) soma->a.soma.lvl_power, lvl_power);
      soma->a.soma.atm_power = MAX ((int) soma->a.soma.atm_power, atm_power);
      return 0;
    }

  soma = NULL;

  CREATE (soma, AFFECTED_TYPE, 1);

  soma->type = type;

  soma->a.soma.duration = duration;
  soma->a.soma.latency = latency;
  soma->a.soma.minute = minute;
  soma->a.soma.max_power = max_power;
  soma->a.soma.lvl_power = lvl_power;
  soma->a.soma.atm_power = atm_power;
  soma->a.soma.attack = attack;
  soma->a.soma.decay = decay;
  soma->a.soma.sustain = sustain;
  soma->a.soma.release = release;

  affect_to_char (ch, soma);

  return 1;
}
