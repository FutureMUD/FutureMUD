using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MailKit.Search;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Communication.Language;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using Org.BouncyCastle.Utilities;

namespace MudSharp.Commands.Modules;

public class ShowModule : Module<ICharacter>
{
	private ShowModule()
		: base("Show")
	{
		IsNecessary = true;
	}

	internal static ShowModule Instance { get; } = new();

	#region Information

	private const string PlayerShowDefaultText =
		@"That is not a valid option for the show command. Valid options are:

	#3bodypartshapes#0 - shows all bodypart shapes
	#3calendars#0 - shows all calendars
	#3calendar <which> [<year>]#0 - shows details about a calendar
	#3combatflags#0 - shows all combat flags
	#3difficulties#0 - shows a list of the possible difficulties
	#3events#0 - shows all AI events
	#3event <which>#0 - shows a specific AI event
	#3itemquality#0 - shows all item qualities
	#3outcomes#0 - shows a list of the possible outcomes
	#3progs#0 - shows all progs
	#3prog <id|name>#0 - shows a specific prog
	#3sizes#0 - shows all sizes 
	#3surgeries#0 - shows a list of possible surgery types
	#3wears#0 - shows all wear profiles
	#3wear <id|name>#0 - shows a specific wear profile
	#3writingstyles#0 - shows writing styles";

	private const string GuideShowDefaultText =
		@"That is not a valid option for the show command. Valid options are:

	#3bodypartshapes#0 - shows all bodypart shapes
	#3calendars#0 - shows all calendars
	#3calendar <which> [<year>]#0 - shows details about a calendar
	#3characteristics#0 - shows all characteristic definitions
	#3colours#0 - shows all available colours
	#3combatflags#0 - shows all combat flags
	#3difficulties#0 - shows a list of the possible difficulties	
	#3events#0 - shows all AI events
	#3event <which>#0 - shows a specific AI event
	#3itemquality#0 - shows all item qualities
	#3materials#0 - shows all materials
	#3outcomes#0 - shows a list of the possible outcomes
	#3profiles#0 - shows all characteristic profiles
	#3progs#0 - shows all progs
	#3prog <id|name>#0 - shows a specific prog
	#3sizes#0 - shows all sizes 
	#3surgeries#0 - shows a list of possible surgery types
	#3terrain#0 - shows all terrain types
	#3values#0 - shows all characteristic values
	#3wears#0 - shows all wear profiles
	#3wear <id|name>#0 - shows a specific wear profile
	#3writingstyles#0 - shows writing styles";

	private const string AdminShowDefaultText =
		@"That is not a valid option for the show command. Valid options are:

	#3accents#0 - shows all accents
	#3account <account>#0 - shows a specific account
	#3accounts [+/- keywords, <date, >date]#0 - shows all accounts [optionally matching criteria]
	#3ais#0 - shows a list of AIs
	#3ai <id|name>#0 - shows a specific AI
	#3ammos#0 - shows all ammunition types
	#3ammo <id|name>#0 - shows a specific ammunition type
	#3armours#0 - shows all armour types
	#3armour <id|name>#0 - shows a specific armour type
	#3attributes#0 - shows all attributes
	#3autoareas#0 - shows all autobuilder area templates
	#3autoarea <id|name>#0 - shows a specific autobuilder area template
	#3autorooms#0 - shows all autobuilder room templates
	#3autoroom <id|name>#0 - shows a specific autobuilder room template
	#3bodies#0 - lists all of the body prototypes
	#3body <which>#0 - shows a particular body prototype
	#3bodypartshapes#0 - shows all bodypart shapes
	#3calendars#0 - shows all calendars
	#3calendar <which> [<year>]#0 - shows details about a calendar
	#3characteristics#0 - shows all characteristic definitions
	#3character <id>#0 - shows a specific character
	#3characters [+/- keywords, <date, >date, $days, *account, guest, alive|dead|retired|suspended]#0 - shows a list of all characters
	#3climates#0 - shows a list of regional climates
	#3climate <id|name>#0 - shows a specific regional climate
	#3climatemodels#0 - shows a list of climate models
	#3clocks#0 - shows all clocks
	#3covers#0 - shows all ranged cover
	#3culture <id|name>#0 - shows a specific culture
	#3cultures#0 - shows all cultures
	#3colours#0 - shows all available colours
	#3combatflags#0 - shows all combat flags
	#3damages#0 - shows a list of damage types
	#3decorators#0 - shows all skill/attribute decorators
	#3decorator <id|name>#0 - shows a particular decorator
	#3difficulties#0 - shows a list of the possible difficulties
	#3drugs#0 - shows all drugs
	#3ethnicity <id|name>#0 - shows an ethnicity
	#3ethnicities#0 - shows all ethnicities
	#3events#0 - shows all AI events
	#3event <which>#0 - shows a specific AI event
	#3exittemplates#0 - shows all non-cardinal exit templates
	#3hwmodels#0 - shows height/weight models for NPC building
	#3itemquality#0 - shows all item qualities 
	#3knowledges#0 - shows knowledges
	#3languages#0 - shows all languages
	#3materials#0 - shows all materials
	#3merits#0 - shows all merits
	#3outcomes#0 - shows a list of the possible outcomes
	#3permissions#0 - shows all permission levels for accounts
	#3profiles#0 - shows all characteristic profiles
	#3overlays#0 - shows all cell overlay packages
	#3pattern <id>#0 - shows a description pattern
	#3patterns#0 - shows all description patterns
	#3progs#0 - shows all progs
	#3prog <id|name>#0 - shows a specific prog
	#3race <id|name>#0 - shows a particular race
	#3races#0 - shows all races
	#3ranges#0 - shows all ranged weapon types
	#3ranged <id|name>#0 - shows a specific ranged weapon type
	#3scripts#0 - shows all scripts
	#3skills#0 - shows all skills
	#3sky <template>#0 - shows a sky template
	#3skies#0 - shows all sky templates
	#3sizes#0 - shows all sizes
	#3stacks#0 - shows all stack descriptors 
	#3staticconfig <which>#0 - shows a static config setting
	#3staticstring <which>#0 - shows a static string setting
	#3surgeries#0 - shows a list of possible surgery types
	#3tags#0 - shows a list of all top level tags
	#3terrain#0 - shows all terrain types
	#3timezones#0 - shows all timezones
	#3timezones <clock>#0 - shows all timezones for a particular clock
	#3values#0 - shows all characteristic values
	#3weapons#0 - shows all weapon types
	#3weapon <id|name>#0 - shows a specific weapon type
	#3wears#0 - shows all wear profiles
	#3wear <id|name>#0 - shows a specific wear profile
	#3weathers#0 - shows a list of weather controllers
	#3weather <id|name>#0 - shows a specific weather controller
	#3weatherevents#0 - shows a list of weather events
	#3weatherevent <id|name>#0 - shows a specific weather event
	#3writingstyles#0 - shows writing styles";

	[PlayerCommand("Show", "show")]
	[CustomModuleName("Game")]
	protected static void Show(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "bodies":
				Show_Bodies(actor);
				return;
			case "body":
				Show_Body(actor, ss);
				return;
			case "accounts":
				Show_Accounts(actor, ss);
				return;
			case "sky":
				Show_Sky(actor, ss);
				return;
			case "skies":
				Show_Skies(actor, ss);
				return;
			case "merits":
				Show_Merits(actor, ss);
				return;
			case "clocks":
				Show_Clocks(actor);
				return;
			case "calendars":
				Show_Calendars(actor);
				return;
			case "calendar":
				Show_Calendar(actor, ss);
				return;
			case "timezones":
				Show_Timezones(actor, ss);
				return;
			case "ammo":
			case "ammunition":
			case "ammotype":
				Show_Ammo(actor, ss);
				return;
			case "ammos":
			case "ammunitions":
			case "ammunitiontypes":
			case "ammotypes":
				Show_Ammos(actor, ss);
				return;
			case "decorators":
				Show_Decorators(actor);
				break;
			case "decorator":
				Show_Decorator(actor, ss);
				break;
			case "improvers":
				Show_Improvers(actor);
				break;
			case "hwmodel":
			case "hwmodels":
			case "height":
			case "weight":
				Show_HeightWeightModels(actor);
				return;
			case "bodypartshapes":
				Show_BodypartShapes(actor);
				break;
			case "climatemodels":
			case "climate models":
				Show_ClimateModels(actor);
				break;
			case "climates":
				Show_Climates(actor);
				break;
			case "climate":
				Show_Climate(actor, ss);
				break;
			case "weathers":
				Show_WeatherControllers(actor);
				break;
			case "weather":
				Show_WeatherController(actor, ss);
				break;
			case "ai":
				Show_AI(actor, ss);
				break;
			case "ais":
				Show_AIs(actor, ss);
				break;
			case "events":
				Show_Events(actor);
				break;
			case "event":
				Show_Event(actor, ss);
				break;
			case "writingstyles":
				Show_WritingStyles(actor, ss);
				break;
			case "drugs":
				Show_Drugs(actor, ss);
				break;
			case "drug":
				Show_Drug(actor, ss);
				break;
			case "combatflags":
				Show_CombatFlags(actor, ss);
				break;
			case "armour":
			case "armor":
				Show_Armour(actor, ss);
				break;
			case "armours":
				Show_Armours(actor, ss);
				break;
			case "weapon":
				Show_Weapon(actor, ss);
				break;
			case "weapons":
				Show_Weapons(actor, ss);
				break;
			case "ranges":
				Show_RangedWeapons(actor, ss);
				break;
			case "ranged":
				Show_RangedWeapon(actor, ss);
				break;
			case "colours":
			case "colors":
				Show_Colours(actor, ss);
				break;
			case "wear":
			case "wear profile":
				Show_WearProfile(actor, ss);
				break;
			case "wears":
			case "wear profiles":
				Show_WearProfiles(actor, ss);
				break;
			case "stacks":
				Show_StackDecorators(actor, ss);
				break;
			case "itemsizes":
			case "item sizes":
			case "sizes":
				Show_ItemSizes(actor, ss);
				break;
			case "itemqualities":
			case "itemquality":
			case "item qualities":
			case "item quality":
			case "qualities":
			case "quality":
				Show_ItemQuality(actor, ss);
				break;
			case "skills":
				Show_Skills(actor, ss);
				break;
			case "attributes":
				Show_Attributes(actor, ss);
				break;
			case "languages":
				Show_Languages(actor, ss);
				break;
			case "accents":
				Show_Accents(actor, ss);
				break;
			case "scripts":
				Show_Scripts(actor, ss);
				break;
			case "values":
			case "characteristic values":
				Show_CharacteristicValues(actor, ss);
				break;
			case "profiles":
			case "characteristic profiles":
				Show_CharacteristicProfiles(actor, ss);
				break;
			case "characteristic definitions":
			case "characteristics":
				Show_CharacteristicDefinitions(actor, ss);
				break;
			case "prog":
			case "program":
			case "futureprog":
				Show_FutureProg(actor, ss);
				break;
			case "progs":
			case "futureprogs":
			case "programs":
				Show_FutureProgs(actor, ss);
				break;
			case "cell overlay packages":
			case "overlay packages":
			case "overlays":
				Show_CellOverlayPackages(actor, ss);
				break;
			case "terrain":
			case "terrains":
				Show_Terrain(actor, ss);
				break;
			case "exits":
			case "exittemplates":
			case "exit templates":
				Show_NonCardinalExitTemplates(actor, ss);
				break;
			case "materials":
				Show_Materials(actor, ss);
				break;
			case "account":
				Show_Account(actor, ss);
				break;
			case "character":
				Show_Character(actor, ss);
				break;
			case "characters":
				Show_Characters(actor, ss);
				break;
			case "permissions":
				Show_Permissions(actor, ss);
				break;
			case "knowledges":
				Show_Knowledges(actor, ss);
				break;
			case "cultures":
				Show_Cultures(actor, ss);
				break;
			case "culture":
				Show_Culture(actor, ss);
				break;
			case "ethnicities":
				Show_Ethnicities(actor, ss);
				break;
			case "ethnicity":
				Show_Ethnicity(actor, ss);
				break;
			case "races":
				Show_Races(actor, ss);
				break;
			case "race":
				Show_Race(actor, ss);
				break;
			case "patterns":
				Show_Patterns(actor, ss);
				break;
			case "pattern":
				Show_Pattern(actor, ss);
				break;
			case "covers":
				Show_Covers(actor, ss);
				break;
			case "difficulties":
			case "difficulty":
				Show_Difficulties(actor);
				break;
			case "outcomes":
				Show_Outcomes(actor);
				break;
			case "autoareas":
				Show_AutoAreas(actor, ss);
				break;
			case "autoarea":
				Show_AutoArea(actor, ss);
				break;
			case "autorooms":
				Show_AutoRooms(actor, ss);
				break;
			case "autoroom":
				Show_AutoRoom(actor, ss);
				break;
			case "tags":
				Show_Tags(actor, ss);
				break;
			case "damages":
			case "damagetypes":
				Show_DamageTypes(actor);
				break;
			case "surgeries":
				Show_Surgeries(actor);
				break;
			case "config":
			case "configuration":
			case "staticconfiguration":
			case "staticconfig":
				Show_StaticConfiguration(actor, ss);
				break;
			case "string":
			case "staticstring":
				Show_StaticString(actor, ss);
				break;
			default:
				if (actor.IsAdministrator())
				{
					actor.Send(AdminShowDefaultText.SubstituteANSIColour());
				}
				else if (actor.PermissionLevel == PermissionLevel.Guide)
				{
					actor.Send(GuideShowDefaultText.SubstituteANSIColour());
				}
				else
				{
					actor.Send(PlayerShowDefaultText.SubstituteANSIColour());
				}

				return;
		}
	}

	private static void Show_Characters(ICharacter actor, StringStack ss)
	{
		actor.Gameworld.LoadAllPlayerCharacters();
		var characters = actor.Gameworld.Characters.Where(x => !x.IsGuest).ToList();
		var filterTexts = new List<string>();
		while (!ss.IsFinished)
		{
			var cmd = ss.PopSpeech().ToLowerInvariant();
			if (cmd.EqualToAny("guests", "guest"))
			{
				characters = actor.Gameworld.Characters.Where(x => x.IsGuest).ToList();
				filterTexts.Add("...who are guest Avatars");
				continue;
			}

			if (cmd.EqualToAny("alive", "live", "living"))
			{
				characters = characters.Where(x => x.Status == CharacterStatus.Active).ToList();
				filterTexts.Add("...who are alive");
				continue;
			}

			if (cmd.EqualToAny("dead", "deceased", "unalive"))
			{
				characters = characters.Where(x => x.Status == CharacterStatus.Deceased).ToList();
				filterTexts.Add("...who are dead");
				continue;
			}

			if (cmd.EqualToAny("suspended", "banned"))
			{
				characters = characters.Where(x => x.Status == CharacterStatus.Suspended).ToList();
				filterTexts.Add("...who are suspended");
				continue;
			}

			if (cmd.EqualToAny("retired", "stored"))
			{
				characters = characters.Where(x => x.Status == CharacterStatus.Retired).ToList();
				filterTexts.Add("...who are retired");
				continue;
			}

			if (cmd.Length > 1)
			{
				var cmdSub = cmd.Substring(1);
				switch (cmd[0])
				{
					case '+':
						characters = characters.Where(x => 
						x.PersonalName.GetName(NameStyle.FullName).Contains(cmdSub, StringComparison.InvariantCultureIgnoreCase) ||
						x.HowSeen(actor, false, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreDisguises | PerceiveIgnoreFlags.IgnoreObscured | PerceiveIgnoreFlags.IgnoreLoadThings | PerceiveIgnoreFlags.IgnoreNamesSetting).Contains(cmdSub, StringComparison.InvariantCultureIgnoreCase)
						).ToList();
						filterTexts.Add($"...with name or description containing {cmdSub.ColourCommand()}");
						continue;
					case '-':
						characters = characters.Where(x =>
						!x.PersonalName.GetName(NameStyle.FullName).Contains(cmdSub, StringComparison.InvariantCultureIgnoreCase) &&
						!x.HowSeen(actor, false, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreDisguises | PerceiveIgnoreFlags.IgnoreObscured | PerceiveIgnoreFlags.IgnoreLoadThings | PerceiveIgnoreFlags.IgnoreNamesSetting).Contains(cmdSub, StringComparison.InvariantCultureIgnoreCase)
						).ToList();
						filterTexts.Add($"...with name or description not containing {cmdSub.ColourCommand()}");
						continue;
					case '*':
						characters = characters.Where(x => x.Account.Name.EqualTo(cmdSub)).ToList();
						filterTexts.Add($"...with account name {cmdSub.ColourValue()}");
						continue;
					case '>':
						if (DateTime.TryParse(cmdSub, actor, out var dt))
						{
							var udt = dt.ToUniversalTime();
							filterTexts.Add($"...last logged in after {udt.GetLocalDateString(actor, true).ColourValue()}");
							characters = characters.Where(x => x.LoginDateTime > udt).ToList();
							continue;
						}
						actor.OutputHandler.Send($"The text {cmdSub.ColourCommand()} is not a valid datetime.");
						continue;
					case '<':
						if (DateTime.TryParse(cmdSub, actor, out dt))
						{
							var udt = dt.ToUniversalTime();
							filterTexts.Add($"...last logged in before {udt.GetLocalDateString(actor, true).ColourValue()}");
							characters = characters.Where(x => x.LoginDateTime < udt).ToList();
							continue;
						}
						actor.OutputHandler.Send($"The text {cmdSub.ColourCommand()} is not a valid datetime.");
						continue;
					case '$':
						if (int.TryParse(cmdSub, out var minutes))
						{
							var result = minutes;
							var now = DateTime.UtcNow;
							characters = characters.Where(x => (now - x.LoginDateTime).Days <= result).ToList();
							filterTexts.Add($"...played in the last {result.ToString("N0", actor).ColourValue()} {"day".Pluralise(result != 1)}");
							continue;
						}
						actor.OutputHandler.Send($"The text {cmdSub.ColourCommand()} is not a valid amount of days.");
						continue;
				}
			}
		}
		var sb = new StringBuilder();
		sb.AppendLine($"Showing all Player Characters...");
		foreach (var filter in filterTexts)
		{
			sb.AppendLine(filter);
		}
		sb.AppendLine(StringUtilities.GetTextTable(
			from ch in characters
			select new List<string>
			{
				ch.Id.ToString("N0", actor),
				ch.PersonalName.GetName(NameStyle.FullName),
				ch.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreDisguises | PerceiveIgnoreFlags.IgnoreObscured | PerceiveIgnoreFlags.IgnoreLoadThings | PerceiveIgnoreFlags.IgnoreNamesSetting),
				ch.Status.Describe(),
				ch.Account.Name,
				TimeSpan.FromMinutes(ch.TotalMinutesPlayed).Describe(actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Description",
				"Status",
				"Account",
				"Time Played"
			},
			actor,
			Telnet.Magenta
		));
	}

	private static void Show_Bodies(ICharacter actor)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from body in actor.Gameworld.BodyPrototypes
			select new List<string>
			{
				body.Id.ToString("N0", actor),
				body.Name,
				body.MinimumLegsToStand.ToString("N0", actor),
				body.MinimumWingsToFly.ToString("N0", actor),
				body.StaminaRecoveryProg.MXPClickableFunctionName(),
				body.Communications?.Name ?? "",
				$"{body.WielderDescriptionSingular} / {body.WielderDescriptionPlural}",
				$"{body.LegDescriptionSingular} / {body.LegDescriptionPlural}",
				body.Parent?.Name ?? ""
			},
			new List<string>
			{
				"Id",
				"Name",
				"Stand Legs #",
				"Fly Wings #",
				"Stamina Prog",
				"Communicate",
				"Wielder",
				"Legs",
				"Parent"
			},
			actor,
			Telnet.Red
		));
	}

	private static void Show_Body(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which body prototype would you like to show?");
			return;
		}

		var body = actor.Gameworld.BodyPrototypes.GetByIdOrName(ss.SafeRemainingArgument);
		if (body is null)
		{
			actor.OutputHandler.Send("There is no such body prototype.");
			return;
		}

		actor.OutputHandler.Send(body.Show(actor));
	}

	private static void Show_Skies(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var templates = actor.Gameworld.SkyDescriptionTemplates.ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"Sky Templates");
		sb.AppendLine();
		sb.Append(StringUtilities.GetTextTable(
			from template in templates
			select new List<string>
			{
				template.Id.ToString("N0", actor),
				template.Name,
				template.SkyDescriptions.Count.ToString("N0", actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"# Descriptions"
			},
			actor,
			Telnet.Magenta
		));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void Show_Sky(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which Sky Description Template would you like to view? See {"show skies".MXPSend("show skies")} for options.");
			return;
		}

		var sky = actor.Gameworld.SkyDescriptionTemplates.GetByIdOrName(ss.SafeRemainingArgument);
		if (sky is null)
		{
			actor.OutputHandler.Send(
				$"There is no Sky Description Template like that. See {"show skies".MXPSend("show skies")} for options.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Sky Description Template #{sky.Id.ToString("N0", actor)} - {sky.Name}".ColourName());
		sb.AppendLine();
		sb.AppendLine("Values below are in mag/arcsec2 and default values are Bortle Class ranges. ");
		sb.AppendLine();
		foreach (var description in sky.SkyDescriptions.Ranges)
		{
			sb.AppendLine();
			sb.AppendLine(
				$"[{description.LowerBound.ToString("N", actor)} to {description.UpperBound.ToString("N", actor)} mag/arcsec2]"
					.Colour(Telnet.Magenta));
			sb.AppendLine();
			sb.AppendLine(description.Value.Wrap(actor.InnerLineFormatLength, "\t"));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void Show_Accounts(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		//actor.Gameworld.PreloadAccounts();
		var accounts = actor.Gameworld.Accounts.ToList();
		while (!ss.IsFinished)
		{
			var arg = ss.PopSpeech();
			string dateText;
			DateTime dt;

			switch (arg[0])
			{
				case '+':
					if (arg.Length == 1)
					{
						goto default;
					}

					arg = arg[1..];
					accounts = accounts.Where(x =>
						x.Name.Contains(arg, StringComparison.InvariantCultureIgnoreCase) ||
						x.EmailAddress.Contains(arg, StringComparison.InvariantCultureIgnoreCase)
					).ToList();
					continue;
				case '-':
					if (arg.Length == 1)
					{
						goto default;
					}


					arg = arg[1..];
					accounts = accounts.Where(x =>
						!x.Name.Contains(arg, StringComparison.InvariantCultureIgnoreCase) &&
						!x.EmailAddress.Contains(arg, StringComparison.InvariantCultureIgnoreCase)
					).ToList();
					continue;
				case '<':
					if (arg.Length == 1)
					{
						goto default;
					}

					dateText = arg[1..];
					if (string.IsNullOrWhiteSpace(dateText))
					{
						goto default;
					}

					if (!DateTime.TryParse(dateText, actor, DateTimeStyles.None, out dt))
					{
						actor.OutputHandler.Send($"The text {dateText.ColourCommand()} is not a valid date.");
						return;
					}

					accounts = accounts.Where(x => x.CreationDate <= dt).ToList();
					continue;
				case '>':
					if (arg.Length == 1)
					{
						goto default;
					}

					dateText = arg[1..];
					if (string.IsNullOrWhiteSpace(dateText))
					{
						goto default;
					}

					if (!DateTime.TryParse(dateText, actor, DateTimeStyles.None, out dt))
					{
						actor.OutputHandler.Send($"The text {dateText.ColourCommand()} is not a valid date.");
						return;
					}

					accounts = accounts.Where(x => x.CreationDate >= dt).ToList();
					continue;
				case '*':

					if (arg.Length == 1)
					{
						goto default;
					}

					dateText = arg[2..];
					if (string.IsNullOrWhiteSpace(dateText))
					{
						goto default;
					}

					if (!DateTime.TryParse(dateText, actor, DateTimeStyles.None, out dt))
					{
						actor.OutputHandler.Send($"The text {dateText.ColourCommand()} is not a valid date.");
						return;
					}

					switch (arg[1])
					{
						case '>':

							accounts = accounts.Where(x => x.LastLoginTime >= dt).ToList();
							continue;
						case '<':
							accounts = accounts.Where(x => x.LastLoginTime <= dt).ToList();
							continue;
						default:
							goto default;
					}
				default:
					actor.OutputHandler.Send(
						$@"The value {arg.ColourCommand()} is not a valid account filter. The valid filters are:

	#3+keyword#0 - account name or emails containing the keyword
	#3-keyword#0 - account name or email NOT containing the keyword
	#3>date#0 - account created after date
	#3<date#0 - account created before date
	#3*>date#0 - last login after date
	#3*<date#0 - last login before date".SubstituteANSIColour());
					return;
			}
		}

		accounts = accounts.OrderBy(x => x.Name).ToList();

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from account in accounts
				select new List<string>
				{
					account.Id.ToString("N0", actor),
					account.Name,
					account.CreationDate.GetLocalDateString(actor, true),
					account.LastLoginTime.GetLocalDateString(actor, true),
					account.TimeZone.Id,
					account.Culture.Name,
					account.EmailAddress
				},
				new List<string>
				{
					"Id",
					"Name",
					"Created",
					"Last Login",
					"Timezone",
					"Culture",
					"Email"
				},
				actor,
				Telnet.Green,
				truncatableColumnIndex: 6
			)
		);
	}

	private static void Show_Timezones(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		IEnumerable<IMudTimeZone> timezones;
		if (command.IsFinished)
		{
			timezones = actor.Gameworld.Clocks.SelectMany(x => x.Timezones);
		}
		else
		{
			var clock = actor.Gameworld.Clocks.GetByIdOrName(command.SafeRemainingArgument);
			if (clock is null)
			{
				actor.OutputHandler.Send("There is no such clock.");
				return;
			}

			timezones = clock.Timezones;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from timezone in timezones
			select new List<string>
			{
				timezone.Id.ToString("N0", actor),
				timezone.Alias,
				timezone.Name,
				timezone.OffsetHours.ToString("N0", actor),
				timezone.OffsetMinutes.ToString("N0", actor),
				timezone.Clock.Alias
			},
			new List<string>
			{
				"Id",
				"Alias",
				"Name",
				"Hours",
				"Minutes",
				"Clock"
			},
			actor,
			Telnet.Green
		));
	}

	private static void Show_Calendars(ICharacter actor)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from calendar in actor.Gameworld.Calendars
			select new List<string>
			{
				calendar.Id.ToString("N0", actor),
				calendar.Alias,
				calendar.FullName,
				calendar.FeedClock.Alias,
				calendar.CurrentDate.Display(CalendarDisplayMode.Short)
			},
			new List<string>
			{
				"Id",
				"Alias",
				"Name",
				"Clock",
				"Current Date"
			},
			actor,
			Telnet.Green
		));
	}

	private static void Show_Calendar(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which calendar would you like to view?");
			return;
		}

		var calendar = actor.Gameworld.Calendars.GetByIdOrName(command.PopSpeech());
		if (calendar is null)
		{
			actor.OutputHandler.Send("There is no such calendar.");
			return;
		}

		if (command.IsFinished)
		{
			TimeModule.DisplayCalendarToCharacter(actor, calendar, null);
			return;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var year))
		{
			actor.OutputHandler.Send("That is not a valid year to view.");
			return;
		}

		TimeModule.DisplayCalendarToCharacter(actor, calendar, calendar.CreateYear(year));
	}

	private static void Show_Clocks(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from clock in actor.Gameworld.Clocks
			select new List<string>
			{
				clock.Id.ToString("N0", actor),
				clock.Alias,
				clock.Name,
				clock.PrimaryTimezone.Alias,
				clock.CurrentTime.Display(TimeDisplayTypes.Immortal)
			},
			new List<string>
			{
				"Id",
				"Alias",
				"Name",
				"Primary Timezone",
				"Current Time"
			},
			actor,
			Telnet.Green
		));
	}

	private static void Show_StaticString(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which static string would you like to view? The options are as follows:\n\n{actor.Gameworld.StaticStringNames.OrderBy(x => x).Select(x => x.ColourCommand()).SplitTextIntoColumns((uint)actor.LineFormatLength / 60U, (uint)actor.LineFormatLength)}",
				nopage: true);
			return;
		}

		var text = ss.SafeRemainingArgument;
		var matchingName = actor.Gameworld.StaticStringNames.FirstOrDefault(x => x.EqualTo(text));
		if (string.IsNullOrEmpty(matchingName))
		{
			actor.OutputHandler.Send(
				$"There is no static string with that name. The options are as follows:\n\n{actor.Gameworld.StaticStringNames.OrderBy(x => x).Select(x => x.ColourCommand()).SplitTextIntoColumns((uint)actor.LineFormatLength / 60U, (uint)actor.LineFormatLength)}",
				nopage: true);
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Static String: {matchingName.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine(actor.Gameworld.GetStaticString(matchingName));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void Show_StaticConfiguration(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which static configuration would you like to view? The options are as follows:\n\n{actor.Gameworld.StaticConfigurationNames.OrderBy(x => x).Select(x => x.ColourCommand()).SplitTextIntoColumns((uint)actor.LineFormatLength / 60U, (uint)actor.LineFormatLength)}",
				nopage: true);
			return;
		}

		var text = ss.SafeRemainingArgument;
		var matchingName = actor.Gameworld.StaticConfigurationNames.FirstOrDefault(x => x.EqualTo(text));
		if (string.IsNullOrEmpty(matchingName))
		{
			actor.OutputHandler.Send(
				$"There is no static configuration with that name. The options are as follows:\n\n{actor.Gameworld.StaticConfigurationNames.OrderBy(x => x).Select(x => x.ColourCommand()).SplitTextIntoColumns((uint)actor.LineFormatLength / 60U, (uint)actor.LineFormatLength)}",
				nopage: true);
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Static Configuration: {matchingName.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine(actor.Gameworld.GetStaticConfiguration(matchingName));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void Show_Improvers(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.OutputHandler.Send("Skill Improvement Models:\n" + StringUtilities.GetTextTable(
			from improver in actor.Gameworld.ImprovementModels
			select new List<string>
			{
				improver.Id.ToString("N0", actor),
				improver.Name.TitleCase()
			},
			new List<string>
			{
				"Id",
				"Name"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void Show_Decorators(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.OutputHandler.Send("Trait Decorators:\n" + StringUtilities.GetTextTable(
			from decorator in actor.Gameworld.TraitDecorators
			select new List<string>
			{
				decorator.Id.ToString("N0", actor),
				decorator.Name.TitleCase()
			},
			new List<string>
			{
				"Id",
				"Name"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void Show_Decorator(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var decorator = actor.Gameworld.TraitDecorators.GetByIdOrName(command.SafeRemainingArgument);
		if (decorator is null)
		{
			actor.OutputHandler.Send("There is no such trait decorator.");
			return;
		}

		actor.OutputHandler.Send(decorator.Show(actor));
	}

	private static void Show_HeightWeightModels(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.OutputHandler.Send("Height/Weight Models:\n" + StringUtilities.GetTextTable(
			from hwmodel in actor.Gameworld.HeightWeightModels
			select new[]
			{
				hwmodel.Id.ToString("N0", actor),
				hwmodel.Name,
				actor.Gameworld.UnitManager.DescribeBrief(hwmodel.MeanHeight, UnitType.Length, actor),
				actor.Gameworld.UnitManager.DescribeBrief(hwmodel.StandardDeviationHeight, UnitType.Length, actor),
				actor.Gameworld.UnitManager.DescribeBrief(hwmodel.MeanBMI, UnitType.Length, actor),
				actor.Gameworld.UnitManager.DescribeBrief(hwmodel.StandardDeviationBMI, UnitType.Length, actor)
			},
			new[] { "Id", "Name", "Mean Height", "Height StdDev", "Mean BMI", "BMI StdDev" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Surgeries(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.OutputHandler.Send(
			$"The valid surgery types are as follows: {Enum.GetValues(typeof(SurgicalProcedureType)).OfType<SurgicalProcedureType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}");
	}

	private static void Show_BodypartShapes(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.OutputHandler.Send("Bodypart Shapes:\n".Colour(Telnet.Cyan) + StringUtilities.GetTextTable(
			from shape in actor.Gameworld.BodypartShapes
			select new[] { shape.Id.ToString("N0", actor), shape.Name },
			new[] { "Id", "Name" },
			actor.Account.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_ClimateModels(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(from climate in actor.Gameworld.ClimateModels
		                                                      select new[]
		                                                      {
			                                                      climate.Id.ToString("N0", actor),
			                                                      climate.Name,
			                                                      climate.MinuteProcessingInterval
			                                                             .ToString("N0", actor),
			                                                      climate.MinimumMinutesBetweenFlavourEchoes.ToString(
				                                                      "N0", actor),
			                                                      climate.MinuteFlavourEchoChance.ToString("N4", actor)
		                                                      },
			new[] { "ID", "Name", "Interval", "Flavour Minimum Minutes", "Flavour Echo Chance" },
			actor.LineFormatLength,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Climates(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(from climate in actor.Gameworld.RegionalClimates
		                                                      select new[]
		                                                      {
			                                                      climate.Id.ToString("N0", actor),
			                                                      climate.Name,
			                                                      climate.ClimateModel.Name
		                                                      },
			new[] { "ID", "Name", "Model" },
			actor.LineFormatLength,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Climate(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which regional climate do you want to view?");
			return;
		}

		var climate = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.RegionalClimates.Get(value)
			: actor.Gameworld.RegionalClimates.GetByName(command.Last);
		if (climate == null)
		{
			actor.OutputHandler.Send("There is no such regional climate to show you.");
			return;
		}

		actor.OutputHandler.Send(climate.Show(actor));
	}

	private static void Show_WeatherControllers(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(from controller in actor.Gameworld.WeatherControllers
		                                                      select new[]
		                                                      {
			                                                      controller.Id.ToString("N0", actor),
			                                                      controller.Name,
			                                                      controller.RegionalClimate.Name,
			                                                      controller.CurrentSeason.Name,
			                                                      controller.CurrentWeatherEvent.Name,
			                                                      actor.Gameworld.UnitManager.DescribeExact(
				                                                      controller.CurrentTemperature,
				                                                      UnitType.Temperature, actor)
		                                                      },
			new[] { "ID", "Name", "Climate", "Season", "Weather", "Temperature" },
			actor.LineFormatLength,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_WeatherController(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which weather controller do you want to view?");
			return;
		}

		var controller = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.WeatherControllers.Get(value)
			: actor.Gameworld.WeatherControllers.GetByName(command.Last);
		if (controller == null)
		{
			actor.OutputHandler.Send("There is no such weather controller to show you.");
			return;
		}

		actor.OutputHandler.Send(controller.Show(actor));
	}

	private static void Show_Outcomes(ICharacter actor)
	{
		actor.OutputHandler.Send("The possible outcomes are " + Enum.GetValues(typeof(Outcome)).OfType<Outcome>()
		                                                            .Select(x => x.DescribeColour()).ListToString());
	}

	private static void Show_DamageTypes(ICharacter actor)
	{
		actor.OutputHandler.Send("The possible damage types are " + Enum.GetValues(typeof(DamageType))
		                                                                .OfType<DamageType>().Select(x =>
			                                                                x.Describe().Colour(Telnet.Cyan))
		                                                                .ListToString());
	}

	private static void Show_Race(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("You must enter a race to show.");
			return;
		}

		var race = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Races.Get(value)
			: actor.Gameworld.Races.GetByName(ss.Last);
		if (race == null)
		{
			actor.Send("There is no such race to show you.");
			return;
		}

		var sb = new StringBuilder();

		sb.AppendLine($"Race #{race.Id:N0} - {race.Name}");
		sb.AppendLine();
		sb.AppendLine(race.Description.Wrap(actor.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine(new[]
			{
				$"Body: {race.BaseBody.Name.Colour(Telnet.Green)}",
				$"Health Model: {race.DefaultHealthStrategy.Name.Colour(Telnet.Green)}",
				$"Breathes?: {race.NeedsToBreathe.ToString().Colour(Telnet.Green)}"
			}
			.ArrangeStringsOntoLines(3U, (uint)actor.LineFormatLength));
		sb.AppendLine(new[]
			{
				$"Corpse: {race.CorpseModel.Name.Colour(Telnet.Green)}",
				$"Butchery: {race.ButcheryProfile?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}",
				$"Genders: {race.AllowedGenders.Select(x => Gendering.Get(x).GenderClass(true).Colour(Telnet.Cyan)).ListToString()}"
			}
			.ArrangeStringsOntoLines(3U, (uint)actor.LineFormatLength));
		sb.AppendLine(new[]
			{
				$"Blood: {race.BloodLiquid?.Name.Colour(Telnet.BoldRed) ?? "None".Colour(Telnet.BoldWhite)}",
				$"Sweat: {race.SweatLiquid?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}",
				$"Sweat Rate: {$"{actor.Gameworld.UnitManager.Describe(race.SweatRateInLitresPerMinute / actor.Gameworld.UnitManager.BaseFluidToLitres, UnitType.FluidVolume, actor)} per minute".Colour(Telnet.Green)}"
			}
			.ArrangeStringsOntoLines(3U, (uint)actor.LineFormatLength));
		sb.AppendLine(new[]
			{
				$"Communication: {race.CommunicationStrategy.Name.Colour(Telnet.Green)}",
				$"Handedness: {race.HandednessOptions.Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}",
				$"Default Hand: {race.DefaultHandedness.Describe().Colour(Telnet.Green)}"
			}
			.ArrangeStringsOntoLines(3U, (uint)actor.LineFormatLength));
		sb.AppendLine(new[]
			{
				$"Armour: {race.NaturalArmourType?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}",
				$"Armour Material: {race.NaturalArmourMaterial?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}",
				$"Armour Quality: {race.NaturalArmourQuality.Describe().Colour(Telnet.Green)}"
			}
			.ArrangeStringsOntoLines(3U, (uint)actor.LineFormatLength));
		sb.AppendLine($"Attributes: {race.Attributes.Select(x => x.Name.Colour(Telnet.Green)).ListToString()}");
		sb.AppendLine(new[]
			{
				$"Attribute Roll: {race.DiceExpression.Colour(Telnet.Green)}",
				$"Attribute Cap: {race.IndividualAttributeCap.ToString("N0", actor).Colour(Telnet.Green)}",
				$"Total Cap: {race.AttributeTotalCap.ToString("N0", actor).Colour(Telnet.Green)}"
			}
			.ArrangeStringsOntoLines(3U, (uint)actor.LineFormatLength));
		sb.AppendLine(new[]
			{
				$"Bonus Prog: {(race.AttributeBonusProg == null ? "None".Colour(Telnet.Red) : string.Format("{0} (#{1:N0})".FluentTagMXP("send", $"href='show futureprog {race.AttributeBonusProg.Id}'"), race.AttributeBonusProg.FunctionName, race.AttributeBonusProg.Id))}",
				"",
				""
			}
			.ArrangeStringsOntoLines(3U, (uint)actor.LineFormatLength));
		sb.AppendLine(new[]
			{
				$"Can Attack: {race.CombatSettings.CanAttack.ToString().Colour(Telnet.Green)}",
				$"Can Defend: {race.CombatSettings.CanDefend.ToString().Colour(Telnet.Green)}",
				$"Use Weapons: {race.CombatSettings.CanUseWeapons.ToString().Colour(Telnet.Green)}"
			}
			.ArrangeStringsOntoLines(3U, (uint)actor.LineFormatLength));

		sb.AppendLine($"Extra Bodyparts: {race.BodypartAdditions.Select(x => x.Name).ListToString()}");
		sb.AppendLine($"Extra Male Bodyparts: {race.MaleOnlyAdditions.Select(x => x.Name).ListToString()}");
		sb.AppendLine($"Extra Female Bodyparts: {race.FemaleOnlyAdditions.Select(x => x.Name).ListToString()}");
		sb.AppendLine();
		sb.AppendLine("Natural Attacks:");
		sb.AppendLine();
		foreach (var item in race.NaturalWeaponAttacks)
		{
			sb.AppendLine(
				$"\t{item.Attack.Name} (#{item.Attack.Id}) - {item.Bodypart.Name} - {item.Quality.Describe()}");
		}

		actor.Send(sb.ToString());
	}

	private static void Show_Races(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.Send(StringUtilities.GetTextTable(
			actor.Gameworld.Races.Select(x => new[]
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.BaseBody.Name,
				x.ButcheryProfile?.Name ?? "None",
				x.CorpseModel.Name,
				x.IlluminationPerceptionMultiplier.ToString("P0", actor)
			}),
			new[]
			{
				"ID",
				"Name",
				"Body",
				"Butchery Profile",
				"Corpse Model",
				"Illumination Mod"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_AutoAreas(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.Send(StringUtilities.GetTextTable(
			actor.Gameworld.AutobuilderAreas.Select(x => new[]
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.ShowCommandByLine
			}),
			new[]
			{
				"ID",
				"Name",
				"About"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_AutoArea(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which autobuilder area template do you want to show?");
			return;
		}

		var template = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.AutobuilderAreas.Get(value)
			: actor.Gameworld.AutobuilderAreas.GetByName(ss.Last);
		if (template == null)
		{
			actor.Send("There is no such autobuilder area template for you to view.");
			return;
		}

		actor.Send(template.Show(actor));
	}

	private static void Show_AutoRooms(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.Send(StringUtilities.GetTextTable(
			actor.Gameworld.AutobuilderRooms.Select(x => new[]
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.ShowCommandByline
			}),
			new[]
			{
				"ID",
				"Name",
				"About"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_AutoRoom(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which autobuilder room template do you want to show?");
			return;
		}

		var template = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.AutobuilderRooms.Get(value)
			: actor.Gameworld.AutobuilderRooms.GetByName(ss.Last);
		if (template == null)
		{
			actor.Send("There is no such autobuilder room template for you to view.");
			return;
		}

		actor.Send(template.Show(actor));
	}

	private static void Show_AI(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which AI would you like to view?");
			return;
		}

		var ai = actor.Gameworld.AIs.GetByIdOrName(ss.SafeRemainingArgument);
		if (ai is null)
		{
			actor.OutputHandler.Send("There is no such AI.");
			return;
		}

		actor.OutputHandler.Send(ai.Show(actor));
	}

	private static void Show_AIs(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.Send(StringUtilities.GetTextTable(
			actor.Gameworld.AIs.Select(x => new[]
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.AIType
			}),
			new[] { "ID", "Name", "Type" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Difficulties(ICharacter actor)
	{
		actor.Send(
			$"There are the following difficulties, from easiest to hardest:\n\t{Enum.GetValues(typeof(Difficulty)).OfType<Difficulty>().Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}");
	}

	private static void Show_Events(ICharacter actor)
	{
		actor.Send(
			StringUtilities.GetTextTable(
				Enum.GetValues(typeof(EventType))
				    .OfType<EventType>()
				    .Select(x => (Event: x, Attribute: x.GetAttribute<EventInfoAttribute>()))
				    .Select(x => new[]
				    {
					    ((int)x.Event).ToString("N0"), 
					    Enum.GetName(typeof(EventType), x.Event),
					    x.Attribute?.Description ?? "Unknown",
				    }),
				new[] { "Number", "Name", "Description" },
				actor.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 2,
				unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_Event(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which event would you like to view information about? See {"show events".MXPSend("show events")} for more information.");
			return;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out EventType @event))
		{
			actor.OutputHandler.Send(
				$"That is not a valid event. See {"show events".MXPSend("show events")} for more information.");
			return;
		}

		var info = @event.GetAttribute<EventInfoAttribute>();
		var sb = new StringBuilder();
		sb.AppendLine($"Event [{@event.DescribeEnum()}]".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine("Parameters:");
		sb.AppendLine();
		foreach (var parameter in info?.Parameters ?? Enumerable.Empty<(string,string)>())
		{
			sb.AppendLine($"\t{parameter.type.Colour(Telnet.VariableGreen)} {parameter.name.Colour(Telnet.VariableCyan)}");
		}
		sb.AppendLine();
		sb.AppendLine(info?.Description?.Wrap(actor.InnerLineFormatLength) ?? "No event-specific description set up yet.");
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void Show_WritingStyles(ICharacter actor, StringStack ss)
	{
		var handwritingDecorator =
			actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("HandwritingSkillId"))?.Decorator;
		actor.Send(StringUtilities.GetTextTable(
			from style in Enum.GetValues(typeof(WritingStyleDescriptors)).OfType<WritingStyleDescriptors>()
			                  .Except(WritingStyleDescriptors.None).OrderBy(x => x.IsModifierDescriptor())
			                  .ThenBy(x => x.IsMachineDescriptor()).ThenBy(x => x.Describe())
			select new[]
			{
				style.Describe(), style.IsModifierDescriptor().ToString(), style.IsMachineDescriptor().ToString(),
				handwritingDecorator?.Decorate(style.MinimumHandwritingSkill()) ??
				style.MinimumHandwritingSkill().ToString("N2", actor)
			},
			new[] { "Style", "Modifier", "Machine Only?", "Minimum Skill" },
			actor.LineFormatLength,
			colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Character(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not permitted to use the show command in that way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which character ID would you like to view?");
			return;
		}

		if (!long.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid ID number of the character you wish to view.");
			return;
		}

		var character = actor.Gameworld.TryGetCharacter(value, true);
		if (character == null)
		{
			actor.Send("There is no such character for you to view.");
			return;
		}

		actor.Send(character.ShowStat(actor));
	}

	private static void Show_Covers(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not permitted to use the show command in that way.");
			return;
		}

		actor.Send(StringUtilities.GetTextTable(
			from cover in actor.Gameworld.RangedCovers
			select
				new[]
				{
					cover.Id.ToString("N0", actor), cover.Name, cover.CoverType.ToString(),
					cover.CoverExtent.ToString(), cover.MaximumSimultaneousCovers.ToString("N0", actor),
					cover.DescriptionString
				},
			new[] { "Id", "Name", "Type", "Extent", "Max#", "Message" },
			actor.LineFormatLength,
			truncatableColumnIndex: 5,
			colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Drug(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not permitted to use the show command in that way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which drug do you want to see?");
			return;
		}

		var drug = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Drugs.Get(value)
			: actor.Gameworld.Drugs.GetByName(ss.Last);
		if (drug == null)
		{
			actor.Send("There is no such drug to show you.");
			return;
		}

		actor.Send(drug.Show(actor));
	}

	private static void Show_Drugs(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not permitted to use the show command in that way.");
			return;
		}

		var drugs = actor.Gameworld.Drugs.ToList();
		// TODO - filtering
		actor.Send(StringUtilities.GetTextTable(
			drugs.Select(x => new[]
			{
				x.Id.ToString(),
				x.Name,
				x.IntensityPerGram.ToString("N4"),
				x.RelativeMetabolisationRate.ToString("N4"),
				x.DrugVectors.Describe(),
				x.DrugTypes
				 .Select(y => x.DescribeEffect(y, actor))
				 .ListToString()
			}),
			new[] { "Id", "Name", "Power per/g", "Metabolise Rate", "Vectors", "Effects" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode,
			truncatableColumnIndex: 5
		));
	}

	private static void Show_CombatFlags(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send("There are the following combat flags to select from: " +
		                         Enum.GetValues(typeof(CombatMoveIntentions))
		                             .OfType<CombatMoveIntentions>()
		                             .OrderBy(x => (int)x)
		                             .Select(x => x.Describe().Colour(Telnet.Green))
		                             .ListToString() + ".");
	}

	private static void Show_ItemQuality(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send("You can use the following item qualities, ranked from worst to best: " +
		                         Enum.GetValues(typeof(ItemQuality))
		                             .OfType<ItemQuality>()
		                             .OrderBy(x => (int)x)
		                             .Select(x => x.Describe().Colour(Telnet.Green))
		                             .ListToString() + ".");
	}

	private static void Show_Weapon(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which weapon type did you want to view?");
			return;
		}

		var weapon = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.WeaponTypes.Get(value)
			: actor.Gameworld.WeaponTypes.GetByName(ss.Last);
		if (weapon == null)
		{
			actor.Send("There is no such weapon type.");
			return;
		}

		actor.OutputHandler.Send(weapon.Show(actor), nopage: true);
	}

	private static void Show_Weapons(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var weapons = actor.Gameworld.WeaponTypes.ToList();
		// TODO - filtering criteria
		actor.Send(StringUtilities.GetTextTable(
			from weapon in weapons
			select new[]
			{
				weapon.Id.ToString("N0", actor),
				weapon.Name.TitleCase(),
				weapon.Classification.Describe(),
				weapon.AttackTrait?.Name.TitleCase() ?? "None",
				weapon.ParryTrait?.Name.TitleCase() ?? "None",
				weapon.ParryBonus.ToString("N0", actor),
				weapon.Reach.ToString("N0", actor),
				weapon.Attacks.Count().ToString("N0", actor)
			},
			new[]
			{
				"ID",
				"Name",
				"Classification",
				"Trait",
				"Parry",
				"Parry Bonus",
				"Reach",
				"Attacks"
			},
			actor.LineFormatLength,
			colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_RangedWeapon(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which ranged weapon type did you want to view?");
			return;
		}

		var weapon = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.RangedWeaponTypes.Get(value)
			: actor.Gameworld.RangedWeaponTypes.GetByName(ss.Last);
		if (weapon == null)
		{
			actor.Send("There is no such ranged weapon type.");
			return;
		}

		actor.OutputHandler.Send(weapon.Show(actor), nopage: true);
	}

	private static void Show_RangedWeapons(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var weapons = actor.Gameworld.RangedWeaponTypes.ToList();
		// TODO - filtering criteria
		actor.Send(StringUtilities.GetTextTable(
			from weapon in weapons
			select new[]
			{
				weapon.Id.ToString("N0", actor),
				weapon.Name.TitleCase(),
				weapon.Classification.Describe(),
				weapon.FireTrait?.Name.TitleCase() ?? "None",
				weapon.OperateTrait?.Name.TitleCase() ?? "None",
				weapon.FireableInMelee.ToString(actor),
				weapon.DefaultRangeInRooms.ToString("N0", actor),
				weapon.AccuracyBonusExpression.OriginalFormulaText
			},
			new[]
			{
				"ID",
				"Name",
				"Classification",
				"Trait",
				"Operate",
				"Melee?",
				"Range",
				"Accuracy"
			},
			actor.LineFormatLength,
			colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Armours(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var armours = actor.Gameworld.ArmourTypes.ToList();
		// TODO - filtering criteria
		actor.Send(StringUtilities.GetTextTable(
			from armour in armours
			select new[]
			{
				armour.Id.ToString("N0", actor),
				armour.Name.TitleCase()
			},
			new[]
			{
				"ID",
				"Name"
			},
			actor.LineFormatLength,
			colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Armour(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which armour type did you want to view?");
			return;
		}

		var armour = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.ArmourTypes.Get(value)
			: actor.Gameworld.ArmourTypes.GetByName(ss.Last);
		if (armour == null)
		{
			actor.Send("There is no such armour type.");
			return;
		}

		actor.OutputHandler.Send(armour.Show(actor), nopage: true);
	}

	private static void Show_Pattern(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which description pattern would you like to see?");
			return;
		}

		if (!long.TryParse(ss.Pop(), out var value))
		{
			actor.Send("Which description pattern do you want to see?");
			return;
		}

		var pattern = actor.Gameworld.EntityDescriptionPatterns.Get(value);
		if (pattern == null)
		{
			actor.Send("There is no such pattern for you to view.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.SendNoNewLine(pattern.Show(actor));
			return;
		}

		IRace race = null;
		ICulture culture = null;
		IEthnicity ethnicity = null;
		IEnumerable<Tuple<ICharacteristicDefinition, ICharacteristicValue>> characteristics;
		Gendering gender;
		double height;
		string whoFor;
		int age;

		switch (ss.Pop().ToLowerInvariant())
		{
			case "me":
				race = actor.Race;
				culture = actor.Culture;
				ethnicity = actor.Ethnicity;
				characteristics =
					actor.CharacteristicDefinitions.Select(x => Tuple.Create(x, actor.GetCharacteristic(x, actor)))
					     .ToList();
				gender = actor.Gender;
				height = actor.Height;
				age = actor.AgeInYears;
				whoFor = "yourself";
				break;
			case "npc":
				if (ss.IsFinished)
				{
					actor.Send("Which NPC Template do you want to view?");
					return;
				}

				if (!long.TryParse(ss.Pop(), out value))
				{
					actor.Send("That is not a valid ID number for an NPC template.");
					return;
				}

				var template = actor.Gameworld.NpcTemplates.Get(value);
				if (template == null)
				{
					actor.Send("There is no such NPC Template.");
					return;
				}

				var charTemplate = template.GetCharacterTemplate();
				race = charTemplate.SelectedRace;
				culture = charTemplate.SelectedCulture;
				ethnicity = charTemplate.SelectedEthnicity;
				whoFor = template.EditHeader();
				gender = Gendering.Get(charTemplate.SelectedGender);
				height = charTemplate.SelectedHeight;
				characteristics = charTemplate.SelectedCharacteristics;
				age = charTemplate.SelectedCulture?.PrimaryCalendar.CurrentDate.YearsDifference(
					      charTemplate.SelectedBirthday ??
					      charTemplate.SelectedCulture?.PrimaryCalendar.CurrentDate) ??
				      0;
				break;
			case "char":
			case "character":
				if (ss.IsFinished)
				{
					actor.Send("Which Character do you want to view?");
					return;
				}

				if (!long.TryParse(ss.Pop(), out value))
				{
					actor.Send("That is not a valid ID number for a Character.");
					return;
				}

				var character = actor.Gameworld.TryGetCharacter(value, true);
				if (character == null)
				{
					actor.Send("There is no such character.");
					return;
				}

				race = character.Race;
				culture = character.Culture;
				ethnicity = character.Ethnicity;
				gender = character.Gender;
				whoFor = string.Format(actor, "Character {0:N0} - {1}", character.Id,
					character.CurrentName.GetName(NameStyle.FullWithNickname));
				characteristics =
					character.CharacteristicDefinitions.Select(
						         x => Tuple.Create(x, character.GetCharacteristic(x, actor)))
					         .ToList();
				height = character.Height;
				age = character.AgeInYears;
				break;
			default:
				actor.Send("You can see how it looks for an NPC, a PC or yourself.");
				return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(pattern.Show(actor));
		sb.AppendLineFormat("How it looks for {0}", whoFor);
		sb.AppendLine();
		sb.Append(IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(pattern.Pattern, characteristics,
			gender, actor.Gameworld,
			race, culture, ethnicity, age, height));
		actor.Send(sb.ToString());
	}

	private static void Show_Patterns(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		long value;
		IRace race = null;
		ICulture culture = null;
		IEthnicity ethnicity = null;
		IEnumerable<Tuple<ICharacteristicDefinition, ICharacteristicValue>> characteristics = null;
		Gendering gender = null;
		var height = 0.0;
		var whoFor = string.Empty;
		var age = 0;

		var patterns = actor.Gameworld.EntityDescriptionPatterns.ToList();
		if (ss.Peek().Equals("short", StringComparison.InvariantCultureIgnoreCase) ||
		    ss.Peek().Equals("sdesc", StringComparison.InvariantCultureIgnoreCase))
		{
			patterns = patterns.Where(x => x.Type == EntityDescriptionType.ShortDescription).ToList();
			ss.Pop();
		}
		else if (ss.Peek().Equals("full", StringComparison.InvariantCultureIgnoreCase) ||
		         ss.Peek().Equals("desc", StringComparison.InvariantCultureIgnoreCase))
		{
			patterns = patterns.Where(x => x.Type == EntityDescriptionType.FullDescription).ToList();
			ss.Pop();
		}

		switch (ss.Pop().ToLowerInvariant())
		{
			case "me":
				race = actor.Race;
				culture = actor.Culture;
				ethnicity = actor.Ethnicity;
				characteristics =
					actor.CharacteristicDefinitions.Select(x => Tuple.Create(x, actor.GetCharacteristic(x, actor)))
					     .ToList();
				gender = actor.Gender;
				whoFor = "yourself";
				height = actor.Height;
				age = actor.AgeInYears;
				break;
			case "npc":
				if (ss.IsFinished)
				{
					actor.Send("Which NPC Template do you want to view?");
					return;
				}

				if (!long.TryParse(ss.Pop(), out value))
				{
					actor.Send("That is not a valid ID number for an NPC template.");
					return;
				}

				var template = actor.Gameworld.NpcTemplates.Get(value);
				if (template == null)
				{
					actor.Send("There is no such NPC Template.");
					return;
				}

				var charTemplate = template.GetCharacterTemplate();
				race = charTemplate.SelectedRace;
				culture = charTemplate.SelectedCulture;
				ethnicity = charTemplate.SelectedEthnicity;
				whoFor = template.EditHeader();
				gender = Gendering.Get(charTemplate.SelectedGender);
				characteristics = charTemplate.SelectedCharacteristics;
				height = charTemplate.SelectedHeight;
				age = charTemplate.SelectedCulture?.PrimaryCalendar.CurrentDate.YearsDifference(
					      charTemplate.SelectedBirthday ??
					      charTemplate.SelectedCulture?.PrimaryCalendar.CurrentDate) ??
				      0;
				break;
			case "char":
			case "character":
				if (ss.IsFinished)
				{
					actor.Send("Which Character do you want to view?");
					return;
				}

				if (!long.TryParse(ss.Pop(), out value))
				{
					actor.Send("That is not a valid ID number for a Character.");
					return;
				}

				var character = actor.Gameworld.TryGetCharacter(value, true);
				if (character == null)
				{
					actor.Send("There is no such character.");
					return;
				}

				race = character.Race;
				culture = character.Culture;
				ethnicity = character.Ethnicity;
				gender = character.Gender;
				whoFor = string.Format(actor, "Character {0:N0} - {1}", character.Id,
					character.CurrentName.GetName(NameStyle.FullWithNickname));
				height = character.Height;
				age = character.AgeInYears;
				characteristics =
					character.CharacteristicDefinitions.Select(
						         x => Tuple.Create(x, character.GetCharacteristic(x, actor)))
					         .ToList();
				break;
			case "":
				break;
			default:
				actor.Send("You can see how it looks for an NPC, a PC or yourself.");
				return;
		}

		if (race == null)
		{
			actor.Send(StringUtilities.GetTextTable(
				from pattern in patterns
				select new[]
				{
					pattern.Id.ToString("N0", actor),
					pattern.Type.Describe(),
					pattern.Pattern
				},
				new[]
				{
					"ID",
					"Type",
					"Pattern"
				},
				actor.LineFormatLength,
				colour: Telnet.Green
			));
		}
		else
		{
			actor.Send(StringUtilities.GetTextTable(
				from pattern in patterns
				select new[]
				{
					pattern.Id.ToString("N0", actor),
					pattern.Type.Describe(),
					IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(pattern.Pattern, characteristics,
						gender, actor.Gameworld, race, culture, ethnicity, age, height)
				},
				new[]
				{
					"ID",
					"Type",
					"Example"
				},
				actor.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 2, unicodeTable: actor.Account.UseUnicode
			));
		}
	}

	private static void Show_Ethnicities(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var ethnicities = actor.Gameworld.Ethnicities;
		// todo - filters

		actor.Send(StringUtilities.GetTextTable(
			from ethnicity in ethnicities
			select new[]
			{
				ethnicity.Id.ToString(actor),
				ethnicity.Name.TitleCase(),
				ethnicity.EthnicGroup,
				ethnicity.EthnicSubgroup,
				ethnicity.ParentRace != null ? ethnicity.ParentRace.Name.TitleCase() : "None",
				ethnicity.AvailabilityProg != null
					? string.Format(actor, "{0} ({1:N0})".FluentTagMXP("send",
							$"href='show futureprog {ethnicity.AvailabilityProg.Id}'"),
						ethnicity.AvailabilityProg.FunctionName, ethnicity.AvailabilityProg.Id)
					: "None"
			},
			new[]
			{
				"ID",
				"Name",
				"Group",
				"Subgroup",
				"Parent Race",
				"Availability"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			truncatableColumnIndex: 5,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Ethnicity(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which ethnicity did you want to view?");
			return;
		}

		var ethnicity = actor.Gameworld.Ethnicities.GetByIdOrName(input.SafeRemainingArgument);
		if (ethnicity is null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return;
		}

		actor.OutputHandler.Send(ethnicity.Show(actor));
	}

	private static void Show_Cultures(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var cultures = actor.Gameworld.Cultures;
		// todo - filters
		actor.Send(StringUtilities.GetTextTable(
			from culture in cultures
			select new[]
			{
				culture.Id.ToString(actor),
				culture.Name.TitleCase(),
				new[]
				{
					culture.NameCultureForGender(Gender.Male),
					culture.NameCultureForGender(Gender.Female),
					culture.NameCultureForGender(Gender.Neuter),
					culture.NameCultureForGender(Gender.NonBinary),
					culture.NameCultureForGender(Gender.Indeterminate)
				}.Distinct().Select(x => x.Name.TitleCase()).ListToCommaSeparatedValues(", "),
				culture.PrimaryCalendar.FullName,
				culture.SkillStartingValueProg?.MXPClickableFunctionNameWithId() ?? "None",
				culture.AvailabilityProg?.MXPClickableFunctionNameWithId() ?? "None"
			},
			new[]
			{
				"ID",
				"Name",
				"Name Culture",
				"Calendar",
				"Skill Prog",
				"Availability"
			},
			actor.LineFormatLength,
			colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
		));
	}


	private static void Show_Culture(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var culture = long.TryParse(input.SafeRemainingArgument, out var value)
			? actor.Gameworld.Cultures.Get(value)
			: actor.Gameworld.Cultures.GetByName(input.SafeRemainingArgument);
		if (culture == null)
		{
			actor.OutputHandler.Send("There is no such culture.");
			return;
		}

		actor.OutputHandler.Send(culture.Show(actor));
	}

	private static void Show_Account(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		using (new FMDB())
		{
			var dbaccount = long.TryParse(input.Pop(), out var value)
				? FMDB.Context.Accounts.FirstOrDefault(x => x.Id == value)
				: FMDB.Context.Accounts.FirstOrDefault(x => x.Name == input.Last);
			if (dbaccount == null)
			{
				actor.Send("There is no account with that name or id number.");
				return;
			}

			var account = actor.Gameworld.TryAccount(dbaccount);
			var sb = new StringBuilder();
			sb.AppendLineFormat(actor, "Account {0} (#{1:N0})".Colour(Telnet.Cyan), account.Name.Proper(),
				account.Id);
			sb.AppendLine();
			sb.AppendLineFormat("Email: {0}", account.EmailAddress.Colour(Telnet.Green));
			sb.AppendLineFormat("Last IP Address: {0}", account.LastIP.Colour(Telnet.Green));
			sb.Append(new[]
			{
				string.Format(actor, "Created: {0}",
					account.CreationDate.GetLocalDateString(actor).Colour(Telnet.Green)),
				string.Format(actor, "Last Login: {0}",
					account.LastLoginTime.GetLocalDateString(actor).Colour(Telnet.Green))
			}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
			sb.Append(new[]
			{
				string.Format(actor, "Registered: {0}", (account.IsRegistered ? "Yes" : "No").Colour(Telnet.Green)),
				string.Format(actor, "Authority: {0}", account.Authority.Name.TitleCase().Colour(Telnet.Green))
			}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
			sb.Append(new[]
			{
				string.Format(actor, "Characters Allowed: {0}",
					account.ActiveCharactersAllowed.ToString("N0", actor).Colour(Telnet.Green)),
				string.Format(actor, "Unicode: {0}",
					(account.UseUnicode ? "Enabled" : "Disabled").Colour(Telnet.Green))
			}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
			sb.AppendLineFormat(actor, "Culture: {0}", account.Culture.NativeName.Colour(Telnet.Green));
			sb.AppendLineFormat(actor, "Timezone: {0}", account.TimeZone.DisplayName.Colour(Telnet.Green));
			sb.AppendLineFormat(actor, "Unit Preference: {0}", account.UnitPreference.Colour(Telnet.Green));
			var totalminutes = dbaccount.Characters.Sum(x => x.TotalMinutesPlayed);
			sb.AppendLineFormat(actor, "Total Time Played: {0}",
				TimeSpan.FromMinutes(totalminutes).Describe().Colour(Telnet.Green));
			if (account.AccountResources.Any(x => x.Value > 0))
			{
				sb.AppendLine();
				sb.AppendLine("Resources:");
				sb.AppendLine();
				foreach (var item in account.AccountResources.Where(x => x.Value > 0))
				{
					sb.AppendLineFormat(actor, "\t{0} {1} - Last Awarded {2}",
						item.Value.ToString("N0", actor).Colour(Telnet.Green),
						(item.Value == 1 ? item.Key.Name.TitleCase() : item.Key.PluralName.TitleCase()).Colour(
							Telnet.Green),
						account.AccountResourcesLastAwarded[item.Key].Value.GetLocalDateString(actor)
						       .Colour(Telnet.Green)
					);
				}
			}

			var characters = FMDB.Context.Characters.Where(x => x.AccountId == dbaccount.Id).ToList();
			var chargens = FMDB.Context.Chargens.Where(x => x.AccountId == dbaccount.Id).ToList();
			if (characters.Any() || chargens.Any())
			{
				sb.AppendLine();
				sb.AppendLine("Characters:");
				sb.AppendLine();
				foreach (var item in characters.Where(x => x.Status == 2))
				{
					sb.AppendLineFormat("\t#{3:N0} - {0} - {1} played - {2}",
						new PersonalName(XElement.Parse(item.NameInfo).Element("PersonalName").Element("Name"),
							actor.Gameworld).GetName(NameStyle.FullWithNickname),
						TimeSpan.FromMinutes(item.TotalMinutesPlayed).Describe().Colour(Telnet.Green),
						"Active".Colour(Telnet.Green),
						item.Id
					);
				}

				foreach (var item in chargens.Where(x => x.Status < 2))
				{
					var definition = XElement.Parse(item.Definition);
					var gender = (Gender)short.Parse(definition.Element("SelectedGender").Value);
					var nameElement = definition.Element("SelectedName");
					if (nameElement == null || nameElement.Value == "-1" ||
					    nameElement.Element("NotSet") != null)
					{
						sb.AppendLine(
							$"\tUnnamed - {(item.Status == 0 ? "In Creation" : "Awaiting Approval").Colour(Telnet.Magenta)}");
					}
					else
					{
						var nameCulture =
							actor.Gameworld.Cultures.Get(long.Parse(definition.Element("SelectedCulture").Value))
							     .NameCultureForGender(gender);
						var name = new PersonalName(nameCulture, nameElement.Element("Name"));
						sb.AppendLine(
							$"\t{name.GetName(NameStyle.FullWithNickname)} - {(item.Status == 0 ? "In Creation" : "Awaiting Approval").Colour(Telnet.Magenta)}");
					}
				}

				foreach (var item in characters.Where(x => x.Status != 2))
				{
					sb.AppendLineFormat("\t#{3:N0} - {0} - {1} played - {2}",
						new PersonalName(XElement.Parse(item.NameInfo).Element("PersonalName").Element("Name"),
							actor.Gameworld).GetName(NameStyle.FullWithNickname),
						TimeSpan.FromMinutes(item.TotalMinutesPlayed).Describe().Colour(Telnet.Green),
						MUDConstants.CharacterStatusStrings[item.Status].Colour(Telnet.Red),
						item.Id
					);
				}
			}

			actor.Send(sb.ToString());
		}
	}

	private static void Show_Materials(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		IEnumerable<IMaterial> materials = actor.Gameworld.Materials;
		if (!input.IsFinished)
		{
			if (!input.PopSpeech().TryParseEnum<MaterialBehaviourType>(out var materialType))
			{
				actor.Send("There is no such material general type to filter by.");
				return;
			}

			materials = materials.Where(x => x.BehaviourType == materialType);
		}

		actor.Send(
			StringUtilities.GetTextTable(
				from material in materials
				select new[]
				{
					material.Id.ToString("N0", actor),
					material.Name,
					material.MaterialDescription,
					material.MaterialType.ToString(),
					material.BehaviourType.ToString()
				},
				new[]
				{
					"ID",
					"Name",
					"Description",
					"Type",
					"General"
				},
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_NonCardinalExitTemplates(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.Send(
			StringUtilities.GetTextTable(
				from template in actor.Gameworld.NonCardinalExitTemplates
				orderby template.Name
				select new[]
				{
					template.Id.ToString("N0", actor),
					template.Name,
					template.OutboundVerb,
					template.OriginOutboundPreface,
					template.OriginInboundPreface,
					template.InboundVerb,
					template.DestinationOutboundPreface,
					template.DestinationInboundPreface
				},
				new[] { "ID", "Name", "O-Verb", "O-OPref", "O-IPref", "I-Verb", "D-OPref", "D-IPref" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	public static void Show_Terrain(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		actor.Send(
			StringUtilities.GetTextTable(
				from terrain in actor.Gameworld.Terrains
				orderby terrain.Name
				select new[]
				{
					terrain.Id.ToString("N0", actor),
					terrain.Name.TitleCase(),
					terrain.FrameworkItemType,
					terrain.MovementRate.ToString("N2", actor),
					terrain.HideDifficulty.Describe(),
					terrain.SpotDifficulty.Describe(),
					terrain.TerrainBehaviourString,
					terrain.DefaultTerrain ? "Yes" : "No"
				},
				new[] { "ID", "Name", "Mode", "Move Rate", "Hide Diff", "Spot Diff", "Model", "Default" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	public static void Show_FutureProg(ICharacter actor, StringStack input)
	{
		IFutureProg prog;
		if (long.TryParse(input.PopSpeech(), out var id))
		{
			prog = actor.Gameworld.FutureProgs.Get(id);
		}
		else
		{
			var progs = actor.Gameworld.FutureProgs.Get(input.Last);
			if (progs.Count > 1)
			{
				actor.OutputHandler.Send(
					"Your query matched multiple programs. Please use the ID number to specify the one that you want:\n\n"
					+ StringUtilities.GetTextTable(
						from qprog in progs
						select
							new[]
							{
								qprog.Id.ToString(), qprog.FunctionName,
								qprog.ReturnType.Describe(), qprog.DescribeParameters()
							},
						new[] { "ID#", "Name", "Returns", "Parameters" },
						actor.Account.LineFormatLength, unicodeTable: actor.Account.UseUnicode)
				);
				return;
			}

			prog = progs.FirstOrDefault();
		}

		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return;
		}

		if (!prog.Public && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("You don't have permission to view that prog.");
			return;
		}

		var raw = input.Peek().EqualTo("raw");

		var sb = new StringBuilder();
		sb.Append(new[]
		{
			$"FutureProg ID #{prog.Id.ToString().Colour(Telnet.Green)}",
			$"Name: {prog.FunctionName.Colour(Telnet.Green)}",
			$"Compiled: {(string.IsNullOrEmpty(prog.CompileError) ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}"
		}.ArrangeStringsOntoLines(3,
			(uint)actor.LineFormatLength
		));
		sb.Append(new[]
		{
			$"Category: {prog.Category.Colour(Telnet.Green)}",
			$"Subcategory: {prog.Subcategory.Colour(Telnet.Green)}",
			$"Public: {(prog.Public ? "Yes" : "No").Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(3,
			(uint)actor.LineFormatLength
		));
		sb.Append(new[]
		{
			$"Returns: {prog.ReturnType.Describe().Colour(Telnet.VariableGreen)}",
			$"Static: {prog.StaticType.DescribeEnum().ColourValue()}",
			$"Compile Time: {(!string.IsNullOrEmpty(prog.CompileError) ? "N/A" : TimeSpan.FromMilliseconds(prog.CompileTimeMilliseconds).Describe(actor).ColourValue())}"
		}.ArrangeStringsOntoLines(3,
			(uint)actor.LineFormatLength
		));
		sb.AppendLine(
			$"Parameters: \n\n{(prog.AcceptsAnyParameters ? "Accepts any Parameters".Colour(Telnet.VariableGreen) : (from parameter in prog.NamedParameters select $"\t{parameter.Item2.Colour(Telnet.VariableCyan)} as {parameter.Item1.Describe().Colour(Telnet.VariableGreen)}").ListToLines(true))}");
		if (!string.IsNullOrEmpty(prog.CompileError))
		{
			sb.AppendLineFormat("Compile Error:\n\n{0}", prog.CompileError.Wrap(80, "\t"));
		}

		if (!string.IsNullOrEmpty(prog.FunctionComment))
		{
			sb.AppendLine("\nComment: \n\n" + prog.FunctionComment.Wrap(80, "\t").ColourCommand());
		}

		sb.AppendLine("\nProgram Text:");
		sb.AppendLine();
		sb.AppendLine(raw ? prog.FunctionText : prog.ColourisedFunctionText);
		actor.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	public static void Show_FutureProgs(ICharacter actor, StringStack input)
	{
		var progs = actor.IsAdministrator()
			? actor.Gameworld.FutureProgs.ToList()
			: actor.Gameworld.FutureProgs.Where(x => x.Public).ToList();

		while (!input.IsFinished)
		{
			var keyword = input.PopSpeech();
			if (string.IsNullOrWhiteSpace(keyword))
			{
				continue;
			}

			if (keyword[0] == '+')
			{
				keyword = keyword.Substring(1);
				progs = progs
				        .Where(x => x.FunctionName.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
				                    x.FunctionComment.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
				        .ToList();
				continue;
			}

			if (keyword[0] == '-')
			{
				keyword = keyword.Substring(1);
				progs = progs
				        .Where(x => !x.FunctionName.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) &&
				                    !x.FunctionComment.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
				        .ToList();
				continue;
			}

			if (keyword[0] == '*')
			{
				keyword = keyword.Substring(1);
				progs = progs
				        .Where(x => x.Subcategory.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase))
				        .ToList();
				continue;
			}

			if (keyword.EqualTo("uncompiled"))
			{
				progs = progs
				        .Where(x => !string.IsNullOrEmpty(x.CompileError))
				        .ToList();
			}

			progs = progs.Where(x => x.Category.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase))
			             .ToList();
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from prog in progs
				select
					new[]
					{
						prog.Id.ToString(), prog.FunctionName, prog.ReturnType.Describe(),
						prog.DescribeParameters(),
						prog.Public.ToColouredString(),
						prog.Category,
						prog.Subcategory,
						string.IsNullOrEmpty(prog.CompileError).ToColouredString()
					},
				new[] { "ID#", "Name", "Returns", "Parameters", "Public", "Category", "Subcategory", "Compiled" },
				actor.Account.LineFormatLength, truncatableColumnIndex: 3, unicodeTable: actor.Account.UseUnicode)
		);
	}

	private static void Show_CharacteristicProfiles(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var characteristics = actor.Gameworld.CharacteristicProfiles.ToList();
		while (!input.IsFinished)
		{
			var cmd = input.Pop();
			characteristics =
				characteristics.Where(x => x.Name.StartsWith(cmd, StringComparison.CurrentCultureIgnoreCase))
				               .ToList();
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from definition in characteristics
				select
					new[]
					{
						definition.Id.ToString(), definition.Name, definition.TargetDefinition.Name,
						definition.Description, definition.FrameworkItemType
					},
				new[] { "ID#", "Name", "Target Definition", "Description", "Type" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_CharacteristicValues(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var cmd = input.Pop();
		var definition = actor.Gameworld.Characteristics.FirstOrDefault(x => x.Pattern.IsMatch(cmd));
		if (definition == null)
		{
			actor.OutputHandler.Send("Show characteristic values for which definition?");
			return;
		}

		var values = actor.Gameworld.CharacteristicValues.Where(definition.IsValue);
		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from value in values
				select
					new[]
					{
						value.Id.ToString(), value.Name, value.GetValue, value.GetBasicValue,
						definition.IsDefaultValue(value) ? "Y" : "N"
					},
				new[] { "ID#", "Name", "Value", "Basic Value", "Default?" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_CharacteristicDefinitions(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var characteristics = actor.Gameworld.Characteristics.ToList();
		while (!input.IsFinished)
		{
			var cmd = input.Pop();
			characteristics =
				characteristics.Where(x => x.Name.StartsWith(cmd, StringComparison.CurrentCultureIgnoreCase))
				               .ToList();
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from definition in characteristics
				select
					new[]
					{
						definition.Id.ToString(), definition.Name, definition.Pattern.ToString(),
						definition.Description, definition.Type.ToString(),
						definition.Parent?.Id.ToString() ?? ""
					},
				new[] { "ID#", "Name", "Pattern", "Description", "Type", "Parent" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	public static void Show_Skills(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var cmd = input.PopSpeech().ToLowerInvariant();
		var skills = actor.Gameworld.Traits.Where(x => x.TraitType == TraitType.Skill);
		if (!string.IsNullOrEmpty(cmd))
		{
			skills = skills.Where(x => x.Group.ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from skill in skills
				select
					new[]
					{
						skill.Id.ToString(), skill.Name, skill.Group, skill.Decorator.Name, skill.MaxValueString,
						skill.Hidden.ToString()
					},
				new[] { "ID#", "Name", "Group", "Decorator", "Max Value", "Hidden?" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_Attributes(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var cmd = input.PopSpeech().ToLowerInvariant();
		var attributes = actor.Gameworld.Traits.Where(x => x.TraitType == TraitType.Attribute);
		if (!string.IsNullOrEmpty(cmd))
		{
			attributes = attributes.Where(x => x.Group.ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from attribute in attributes
				select
					new[]
					{
						attribute.Id.ToString(), attribute.Name, attribute.Group, attribute.Decorator.Name,
						attribute.MaxValueString, attribute.Hidden.ToString()
					},
				new[] { "ID#", "Name", "Group", "Decorator", "Max Value", "Hidden?" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_Languages(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var languages = actor.Gameworld.Languages.ToList();
		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from language in languages
				select
					new[]
					{
						language.Id.ToString(), language.Name, language.LinkedTrait.Name,
						language.UnknownLanguageSpokenDescription, language.Model.Name,
						language.LanguageObfuscationFactor.ToString("N3")
					},
				new[] { "ID#", "Name", "Linked Trait", "Unknown Desc", "Model", "Obfuscation" },
				actor.Account.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_Accents(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var cmd = input.PopSpeech().ToLowerInvariant();
		var accents = actor.Gameworld.Accents.ToList();
		while (!string.IsNullOrEmpty(cmd))
		{
			switch (cmd[0])
			{
				case '+':
					accents = accents.Where(x =>
						x.Group.ToLowerInvariant()
						 .StartsWith(cmd.Substring(1), StringComparison.InvariantCultureIgnoreCase)).ToList();
					break;
				case '-':
					accents = accents.Where(x =>
						!x.Group.ToLowerInvariant()
						  .StartsWith(cmd.Substring(1), StringComparison.InvariantCultureIgnoreCase)).ToList();
					break;
				default:
					accents = accents.Where(x =>
						                 x.Language.Name.ToLowerInvariant()
						                  .StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase))
					                 .ToList();
					break;
			}

			cmd = input.PopSpeech().ToLowerInvariant();
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from accent in accents
				select
					new[]
					{
						accent.Id.ToString(), accent.Name.ProperSentences(), accent.Language.Name,
						accent.Group.Proper(), accent.AccentSuffix, accent.Difficulty.Describe()
					},
				new[] { "ID#", "Name", "Language", "Group", "Suffix", "Difficulty" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_Scripts(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var cmd = input.PopSpeech().ToLowerInvariant();
		var scripts = actor.Gameworld.Scripts.ToList();
		while (!string.IsNullOrEmpty(cmd))
		{
			scripts = scripts.Where(x =>
				x.Name.ToLowerInvariant().StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase)).ToList();
			cmd = input.PopSpeech().ToLowerInvariant();
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from script in scripts
				select
					new[]
					{
						script.Id.ToString(), script.Name.ProperSentences(), script.KnownScriptDescription
					},
				new[] { "ID#", "Name", "Known Description" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_ItemSizes(ICharacter actor, StringStack input)
	{
		actor.OutputHandler.Send("You can use the following sizes, ranked from smallest to largest: " +
		                         Enum.GetValues(typeof(SizeCategory))
		                             .OfType<SizeCategory>()
		                             .OrderBy(x => (int)x)
		                             .Select(x => x.Describe().Colour(Telnet.Green))
		                             .ListToString() + ".");
	}

	private static void Show_Colours(ICharacter actor, StringStack input)
	{
		var colours = actor.Gameworld.Colours.ToList();
		//while (!input.IsFinished) {
		// TODO - filter conditions
		//}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from colour in colours.OrderBy(x => x.Id)
				select
					new[]
					{
						colour.Id.ToString(), colour.Name.Proper(), colour.Basic.ToString(), colour.Fancy,
						colour.Red.ToString(),
						colour.Green.ToString(), colour.Blue.ToString()
					},
				new[] { "ID#", "Colour", "Basic Colour", "Fancy", "Red", "Green", "Blue" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_WearProfile(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which wear profile do you wish to view?");
			return;
		}

		var profile = long.TryParse(input.PopSpeech(), out var value)
			? actor.Gameworld.WearProfiles.Get(value)
			: actor.Gameworld.WearProfiles.FirstOrDefault(
				x => x.Name.Equals(input.Last, StringComparison.InvariantCultureIgnoreCase));
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such wear profile to view.");
			return;
		}

		actor.OutputHandler.Send(profile.ShowTo(actor));
	}

	private static void Show_WearProfiles(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var profiles = actor.Gameworld.WearProfiles.ToList();

		//while (!input.IsFinished) {
		// TODO - filter conditions
		//}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from profile in profiles.OrderBy(x => x.Id)
				select
					new[]
					{
						profile.Id.ToString(), profile.Name?.Proper() ?? "Unnamed",
						profile.DesignedBody?.Name.Proper() ?? "None",
						profile.Description
					},
				new[] { "ID#", "Name", "Designed Body", "Description" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_StackDecorators(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var decorators = actor.Gameworld.StackDecorators.ToList();

		//while (!input.IsFinished) {
		// TODO - filter conditions
		//}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from decorator in decorators.OrderBy(x => x.Id)
				select new[] { decorator.Id.ToString(), decorator.Name.Proper(), decorator.Description },
				new[] { "ID#", "Name", "Description" }, actor.Account.LineFormatLength, colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Show_CellOverlayPackages(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var overlays = actor.Gameworld.CellOverlayPackages.ToList();

		// TODO - filters

		using (new FMDB())
		{
			actor.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from overlay in overlays.OrderBy(x => x.Id)
					select
						new[]
						{
							overlay.Id.ToString(), overlay.RevisionNumber.ToString(), overlay.Name,
							overlay.Status.Describe(), FMDB.Context.Accounts.Find(overlay.BuilderAccountID).Name,
							overlay.BuilderDate.GetLocalDateString(actor, true)
						},
					new[] { "ID#", "Rev#", "Name", "Status", "Builder", "Date" }, actor.Account.LineFormatLength,
					colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
				)
			);
		}
	}

	private static void Show_Permissions(ICharacter actor, StringStack input)
	{
		actor.Send("There are the following permissions, in order from lowest to highest:\n\n{0}",
			Enum.GetNames(typeof(PermissionLevel))
			    .Select(x => "\t" + x)
			    .ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n"));
	}

	private static void Show_Knowledges(ICharacter actor, StringStack input)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not authorised to use show in this way.");
			return;
		}

		var knowledges = actor.Gameworld.Knowledges.ToList();
		if (!input.IsFinished)
		{
			var category = input.PopSpeech();
			knowledges =
				knowledges.Where(x => x.KnowledgeType.Equals(category, StringComparison.InvariantCultureIgnoreCase))
				          .ToList();

			if (!input.IsFinished)
			{
				var subcategory = input.PopSpeech();
				knowledges =
					knowledges.Where(
						          x =>
							          x.KnowledgeSubtype.Equals(subcategory,
								          StringComparison.InvariantCultureIgnoreCase))
					          .ToList();
			}
		}

		actor.Send(StringUtilities.GetTextTable(
			from knowledge in knowledges
			select new[]
			{
				knowledge.Id.ToString(actor),
				knowledge.Name,
				knowledge.Description,
				knowledge.KnowledgeType,
				knowledge.KnowledgeSubtype,
				knowledge.CanPickChargenProg?.MXPClickableFunctionName() ?? "N/A",
				knowledge.CanLearnProg?.MXPClickableFunctionName() ?? "N/A",
				knowledge.Learnable.ToString()
			},
			new[] { "Id", "Name", "Description", "Type", "Subtype", "Chargen Prog", "Lrn Prog", "Learnable" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			truncatableColumnIndex: 2, unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Tags(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not permitted to use the show command in that way.");
			return;
		}

		var roots = actor.Gameworld.Tags.Where(x => x.Parent == null).ToList();
		actor.Send(StringUtilities.GetTextTable(
			from root in roots
			select new[] { root.Id.ToString("N0", actor), root.Name },
			new[] { "ID", "Name" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void Show_Ammo(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not permitted to use the show command in that way.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which ammunition type do you want to show? See {"show ammos".MXPSend("show ammos", "View a list of ammunition types")}.");
			return;
		}

		var ammo = actor.Gameworld.AmmunitionTypes.GetByIdOrName(command.SafeRemainingArgument);
		if (ammo is null)
		{
			actor.OutputHandler.Send(
				$"There is no such ammunition type. See {"show ammos".MXPSend("show ammos", "View a list of ammunition types")} for a list.");
			return;
		}

		actor.OutputHandler.Send(ammo.Show(actor));
	}

	private static void Show_Ammos(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not permitted to use the show command in that way.");
			return;
		}

		var ammos = actor.Gameworld.AmmunitionTypes.AsEnumerable();
		while (!command.IsFinished)
		{
			var filter = command.PopSpeech();
			if (filter.TryParseEnum<RangedWeaponType>(out var rwt))
			{
				ammos = ammos.Where(x => x.RangedWeaponTypes.Contains(rwt));
				continue;
			}

			ammos = ammos.Where(x =>
				x.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
				x.SpecificType.Contains(filter, StringComparison.InvariantCultureIgnoreCase));
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in ammos
			select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.SpecificType,
				item.RangedWeaponTypes.Select(x => x.DescribeEnum().ColourValue()).ListToCommaSeparatedValues(", "),
				item.DamageProfile.DamageExpression.OriginalFormulaText
			},
			new List<string>
			{
				"Id",
				"Name",
				"Grade",
				"Types",
				"Damage"
			},
			actor.LineFormatLength,
			colour: Telnet.Blue,
			unicodeTable: actor.Account.UseUnicode,
			truncatableColumnIndex: 4
		));
	}

	private static void Show_Merits(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.Send("You are not permitted to use the show command in that way.");
			return;
		}

		var merits = actor.Gameworld.Merits.OfType<ICharacterMerit>().ToList();
		// TODO - filters
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from merit in merits
			select new List<string>
			{
				merit.Id.ToString("N0", actor),
				merit.Name,
				merit.MeritType.DescribeEnum(),
				merit.ApplicabilityProg?.MXPClickableFunctionName() ?? "None",
				merit.ChargenAvailableProg?.MXPClickableFunctionName() ?? "None",
				merit.DatabaseType
			},
			new List<string>
			{
				"Id",
				"Name",
				"Quirk",
				"Applies Prog",
				"Chargen Prog",
				"Type"
			},
			actor,
			Telnet.Green
		));
	}

	#endregion
}