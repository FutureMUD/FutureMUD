﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Commands.Helpers;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using static System.Collections.Specialized.BitVector32;

namespace MudSharp.Commands.Modules;

internal class CombatBuilderModule : BaseBuilderModule
{
	private CombatBuilderModule()
		: base("CombatBuilder")
	{
		IsNecessary = true;
	}

	public static CombatBuilderModule Instance { get; } = new();

	#region Ammunition

	public const string AmmunitionHelp =
		@"The ammunition command is used to view, create and edit ammunition types for ranged weapons. Each ammunition type should have a matching item component, but this will be automatically generated for you when you make a new ammunition type.

The syntax is as follows:

	#3ammunition list#0 - lists all ammunition types
	#3ammunition edit <id|name>#0 - opens the specified ammunition type for editing
	#3ammunition edit new <name> <type>#0 - creates a new ammunition type for editing
	#3ammunition edit#0 - equivalent of doing SHOW on your currently editing ammunition type
	#3ammunition close#0 - closes the currently edited ammunition type
	#3ammunition clone <id|name> <new name>#0 - creates a carbon copy of an ammunition type for editing
	#3ammunition show <id|name>#0 - shows a particular ammunition type.
	#3ammunition set name <name>#0 - sets the name
	#3ammunition set grade <grade>#0 - sets the grade (mostly used for guns)
	#3ammunition set volume <volume>#0 - sets the volume of the shot
	#3ammunition set block <difficulty>#0 - sets how difficult it is to block a shot
	#3ammunition set dodge <difficulty>#0 - sets how difficult it is to dodge a shot
	#3ammunition set damagetype <type>#0 - sets the damage type dealt
	#3ammunition set accuracy <bonus>#0 - sets the bonus accuracy from this ammo
	#3ammunition set breakhit <%>#0 - sets the ammo break chance on hit
	#3ammunition set breakmiss <%>#0 - sets the ammo break chance on miss
	#3ammunition set damage <expression>#0 - sets the damage expression
	#3ammunition set stun <expression>#0 - sets the stun expression
	#3ammunition set pain <expression>#0 - sets the pain expression
	#3ammunition set alldamage <expression>#0 - sets the damage, pain and stun expression at once

Note, with the damage/pain/stun expressions, you can use the following variables:

	#6pointblank#0 - 1 if fired at own bodypart or during coup de grace, 0 otherwise
	#6quality#0 - 0-11 for item quality, 5 = standard quality
	#6degree#0 - 0-5 for check success, 0 = marginal success, 5 = total success
	#6inmelee#0 - 1 if being fired in melee, or 0 otherwise
	#6range#0 - the range in rooms";

	[PlayerCommand("AmmunitionType", "ammunition", "ammunitiontype", "ammo", "ammotype")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("AmmunitionType", AmmunitionHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Ammunition(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()),
			EditableItemHelper.AmmunitionTypeHelper);
	}

	#endregion

	#region Weapon Types
	public const string WeaponTypeHelp =
		@"The #3WeaponType#0 command is used to edit weapon types, which control the properties of a weapon. To use a weapon type on an item you need an item component, but the new/clone subcommands will automatically make one for you.

See also the closely related #6weaponattack#0 and #6traitexpression#0 commands.

You can use the following syntax with this command:

	#3weapontype list#0 - lists all weapon types
	#3weapontype edit <id|name>#0 - opens the specified weapon type for editing
	#3weapontype edit new <name>#0 - creates a new weapon type for editing
	#3weapontype edit#0 - equivalent of doing SHOW on your currently editing weapon type
	#3weapontype close#0 - closes the currently edited weapon type
	#3weapontype clone <id|name> <new name>#0 - creates a carbon copy of a weapon type for editing (including attacks)
	#3weapontype show <id|name>#0 - shows a particular weapon type
	#3weapontype set name <name>#0 - the name of this weapon type
	#3weapontype set classification <which>#0 - changes the classification of this weapon for law enforcement
	#3weapontype set skill <which>#0 - sets the skill which this weapon uses
	#3weapontype set parry <which>#0 - sets the skill which this weapon parries with
	#3weapontype set bonus <number>#0 - the bonus/penalty to parrying with this weapon
	#3weapontype set reach <number>#0 - sets the reach of the weapon
	#3weapontype set stamina <cost>#0 - how much stamina it takes to parry with this weapon";

	[PlayerCommand("WeaponType", "weapontype", "wt")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("WeaponType", WeaponTypeHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void WeaponType(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.WeaponTypeHelper);
	}
	#endregion

	#region Ranged Weapon Types

	public const string RangedWeaponTypeHelp = @"The #3RangedWeaponType#0 command is used to create and edit Ranged Weapon Types, which are used in the matching game item components to turn an item into a ranged weapon.

You will separately need to make the matching item component with the #3Component#0 command.

You can use the following syntax with this command:

	#3rwt list#0 - lists all ranged weapon types
	#3rwt edit <id|name>#0 - opens the specified ranged weapon type for editing
	#3rwt edit new <name>#0 - creates a new ranged weapon type for editing
	#3rwt edit#0 - equivalent of doing SHOW on your currently editing ranged weapon type
	#3rwt close#0 - closes the currently edited ranged weapon type
	#3rwt clone <id|name> <new name>#0 - creates a carbon copy of a ranged weapon type for editing (including attacks)
	#3rwt show <id|name>#0 - shows a particular ranged weapon type
	#3rwt set name <name>#0 - changes the name
	#3rwt set type <type>#0 - changes the ranged weapon type that it codedly applies to
	#3rwt set range <##>#0 - sets the range in rooms that this weapon can fire
	#3rwt set class <class>#0 - sets the weapon classification of this weapon
	#3rwt set firestamina <##>#0 - the amount of stamina needed to fire this weapon	
	#3rwt set loadstamina <##>#0 - the amount of stamina needed to load this weapon
	#3rwt set cover <##>#0 - the bonus (-ve for penalty) against targets in cover
	#3rwt set aimdifficulty <difficulty>#0 - the difficulty of the aim check
	#3rwt set aimloss <%>#0 - how much percentage of the aim to lose after firing
	#3rwt set freehand#0 - toggles needing a free hand to ready the weapon
	#3rwt set fireskill <skill>#0 - sets the skill used when firing the weapon
	#3rwt set operateskill <skill>#0 - sets the skill used when loading/readying the weapon
	#3rwt set melee#0 - toggles being able to use the weapon to shoot in melee
	#3rwt set ammotype <grade>#0 - sets the ammo type that ammunition must match
	#3rwt set ammocapacity <##>#0 - sets the internal capacity for ammunition
	#3rwt set loadtype <loadtype>#0 - sets the ammunition load type
	#3rwt set loaddelay <seconds>#0 - sets the delay after loading
	#3rwt set readydelay <seconds>#0 - sets the delay after readying
	#3rwt set firedelay <seconds>#0 - sets the delay after firing
	#3rwt set accuracy <formula>#0 - sets the formula for accuracy bonus with the weapon
	#3rwt set damage <formula>#0 - sets the formula for damage bonus with the weapon

For the accuracy formula you can use the following parameters:

	#6quality#0 - the quality of the weapon item
	#6range#0 - the range in rooms
	#6inmelee#0 - 1 if being fired in melee, 0 otherwise
	#6aim#0 - the aim percentage between 0 and 1
	#6variable#0 - the character's current value of the fire skill

For the damage formula you can use the following parameters:

	#6quality#0 - the quality of the weapon item
	#6range#0 - the range in rooms
	#6inmelee#0 - 1 if being fired in melee, or 0 otherwise
	#6pointblank#0 - 1 if fired point blank, or 0 otherwise
	#6degrees#0 - the opposed outcome degree between 0 (marginal) and 5 (total)
	#6variable#0 - the character's current value of the fire skill";
	[PlayerCommand("RangedWeaponType", "rangedweapontype", "rwt")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("RangedWeaponType", RangedWeaponTypeHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Ranged(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.RangedWeaponTypeHelper);
	}
	#endregion

	#region Shield Types
	public const string ShieldTypeHelp = @"The #3Shield#0 command is used to create and edit Shield Types, which are used in the matching game item components to turn an item into a shield.

You will separately need to make the matching item component with the #3Component#0 command.

You can use the following syntax with this command:

	#3shield list#0 - lists all shield types
	#3shield edit <id|name>#0 - opens the specified shield type for editing
	#3shield edit new <name> <skill> <armour>#0 - creates a new shield type for editing
	#3shield edit#0 - equivalent of doing SHOW on your currently editing shield type
	#3shield close#0 - closes the currently edited shield type
	#3shield clone <id|name> <new name>#0 - creates a carbon copy of a shield type for editing
	#3shield show <id|name>#0 - shows a particular shield type
	#3shield set name <name>#0 - renames the armour type
	#3shield set trait <which>#0 - sets the trait used for blocking
	#3shield set bonus <##>#0 - sets the bonus for blocking
	#3shield set stamina <##>#0 - sets the stamina usage per block
	#3shield set armour <which>#0 - sets the armour type for reducing damage";
	[PlayerCommand("ShieldType", "shieldtype")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("ShieldType", ShieldTypeHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Shield(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.ShieldTypeHelper);
	}
	#endregion

	#region Weapon Attacks

	public const string WeaponAttackHelp =
		@"This command is used to create and edit weapon attacks, as well as natural weapon attacks (e.g. unarmed attacks). Conceptually, a weapon attack either belongs to a weapon type (which makes it a melee weapon attack) or it does not (which makes it a natural/unarmed attack). 

Weapon attacks have types that determine how they work mechanically. You cannot change the type of a weapon attack once it has been made. In this case you may need to make a separate attack.

The syntax for this command is as follows:

	#3weaponattack list [<filters>]#0 - lists all weapon attacks
	#3weaponattack edit <id|name>#0 - opens a weapon attack for editing
	#3weaponattack edit#0 - an alias for WEAPONATTACK SHOW when you have something open
	#3weaponattack show <id|name>#0 - shows details about a particular weapon attack
	#3weaponattack close#0 - closes the open weapon attack
	#3weaponattack natural <race> <attack> <bodypart>#0 - toggles an unarmed attack being enabled for a race
	#3weaponattack natural <race> <attack> remove#0 - removes all attacks of that type from the race
	#3weaponattack new <type>#0 - creates a new weapon attack of the specified type
	#3weaponattack clone <which>#0 - creates a carbon copy of the specified weapon type
	#3weaponattack set ...#0 - edits the properties of a weapon attack. See that command for more help.

You can use the following arguments to refine the list command:

	#6<weapontype>#0 - shows only attacks with a specified weapon type
	#6unarmed#0 - shows only unarmed attacks
	#6+key#0 - shows only attacks whose names have the keyword
	#6-key#0 - excludes weapon types whose names have the keyword
	#6*<movetype>#0 - shows only attacks of the specified move type";

	[PlayerCommand("WeaponAttack", "weaponattack", "wa")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("weaponattack", WeaponAttackHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void WeaponAttack(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "new":
				WeaponAttackNew(actor, ss);
				return;
			case "edit":
				WeaponAttackEdit(actor, ss);
				return;
			case "close":
				WeaponAttackClose(actor, ss);
				return;
			case "set":
				WeaponAttackSet(actor, ss);
				return;
			case "delete":
				WeaponAttackDelete(actor, ss);
				return;
			case "clone":
				WeaponAttackClone(actor, ss);
				return;
			case "list":
				WeaponAttackList(actor, ss);
				return;
			case "natural":
			case "nat":
				WeaponAttackNatural(actor, ss);
				return;
			case "show":
			case "view":
				WeaponAttackView(actor, ss);
				return;
			case "audit":
				WeaponAttackAudit(actor, ss);
				return;
		}

		actor.OutputHandler.Send(WeaponAttackHelp.SubstituteANSIColour());
	}

	private static void WeaponAttackAudit(ICharacter actor, StringStack ss)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Running an audit of issues with weapon attacks...");
		var errors = false;

		var gameworld = actor.Gameworld;
		var missingMessages = new List<(BuiltInCombatMoveType, Outcome)>();
		var outcomeValues =
			Enum.GetValues(typeof(Outcome))
				.Cast<Outcome>()
				.Where(x => x != Outcome.None && x != Outcome.NotTested)
				.ToList();
		var typeValues = Enum.GetValues(typeof(BuiltInCombatMoveType)).Cast<BuiltInCombatMoveType>().ToList();
		// Make sure there is a universal fallback message for every attack type
		foreach (var typeValue in typeValues)
		{
			if (gameworld.CombatMessageManager.CombatMessages.Any(x =>
					x.Type == typeValue && (!x.Outcome.HasValue || x.Outcome == Outcome.None) &&
					x.WeaponAttackProg?.ExecuteBool(null, null, null, 0, "") != false))
			{
				continue;
			}

			if (gameworld.WeaponAttacks.Any(x => x.MoveType == typeValue) && gameworld.WeaponAttacks.Where(x => x.MoveType == typeValue).All(x =>
					outcomeValues.All(y => gameworld.CombatMessageManager.GetCombatMessageFor(null, null, null, x, typeValue, y, null) != null)))
			{
				continue;
			}

			missingMessages.AddRange(from outcomeValue in outcomeValues
									 where
										 !gameworld.CombatMessageManager.CombatMessages.Any(
											 x =>
												 x.Type == typeValue &&
												 (!x.Outcome.HasValue || x.Outcome == outcomeValue ||
												  x.Outcome == Outcome.None) &&
												 x.WeaponAttackProg?.ExecuteBool(null, null, null, 0, "") != false)
									 select (typeValue, outcomeValue));
			errors = true;
		}

		if (missingMessages.Count > 0)
		{
			sb.AppendLine($"...found missing fallback messages for the following types:".Colour(Telnet.Red));
			foreach (var (type,outcome) in missingMessages)
			{
				sb.AppendLine($"......{type.DescribeEnum(false, Telnet.Cyan)}@{outcome.DescribeColour()}");
			}
		}

		if (!errors)
		{
			sb.AppendLine("...Good news, no errors found!".Colour(Telnet.BoldGreen));
		}
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void WeaponAttackView(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished && !actor.EffectsOfType<BuilderEditingEffect<IWeaponAttack>>().Any())
		{
			actor.OutputHandler.Send("You must specify a weapon attack to show if you are not editing one.");
			return;
		}

		IWeaponAttack attack;
		if (ss.IsFinished)
		{
			attack = actor.EffectsOfType<BuilderEditingEffect<IWeaponAttack>>().First().EditingItem;
		}
		else
		{
			attack = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.WeaponAttacks.Get(value)
				: actor.Gameworld.WeaponAttacks.GetByName(ss.Last);
			if (attack == null)
			{
				actor.OutputHandler.Send("There is no such weapon attack to show you.");
				return;
			}
		}

		actor.OutputHandler.Send(attack.ShowBuilder(actor));
	}

	private static void WeaponAttackList(ICharacter actor, StringStack ss)
	{
		if (ss.Peek().EqualToAny("help", "?"))
		{
			actor.OutputHandler.Send(@"You can use the following arguments to help refine your search:

	#3<weapontype>#0 - shows only attacks with a specified weapon type
	#3unarmed#0 - shows only unarmed attacks
	#3+key#0 - shows only attacks whose names have the keyword
	#3-key#0 - excludes weapon types whose names have the keyword
	#3*<movetype>#0 - shows only attacks of the specified move type".SubstituteANSIColour());
			return;
		}

		var attacks = actor.Gameworld.WeaponAttacks.AsEnumerable();
		while (!ss.IsFinished)
		{
			var arg = ss.PopSpeech();
			if (arg[0] == '+' && arg.Length > 1)
			{
				arg = arg.Substring(1);
				attacks = attacks.Where(x => x.Name.Contains(arg, StringComparison.InvariantCultureIgnoreCase));
				continue;
			}

			if (arg[0] == '-' && arg.Length > 1)
			{
				arg = arg.Substring(1);
				attacks = attacks.Where(x => !x.Name.Contains(arg, StringComparison.InvariantCultureIgnoreCase));
				continue;
			}

			if (arg[0] == '*' && arg.Length > 1)
			{
				if (!arg.Substring(1).TryParseEnum<BuiltInCombatMoveType>(out var value))
				{
					actor.OutputHandler.Send(
						$"There is no such built-in combat move type as {arg.Substring(1).ColourCommand()}.\nThe valid types are as follows:\n\n{Enum.GetValues<BuiltInCombatMoveType>().Select(x => x.DescribeEnum(false, Telnet.Cyan)).ListToLines(true)}");
					return;
				}

				attacks = attacks.Where(x => x.MoveType == value);
				continue;
			}

			if (arg.EqualTo("unarmed"))
			{
				attacks = attacks.Where(x => actor.Gameworld.WeaponTypes.All(y => !y.Attacks.Contains(x)));
				continue;
			}

			var weaponType = actor.Gameworld.WeaponTypes.FirstOrDefault(x => x.Name.EqualTo(arg)) ??
							 actor.Gameworld.WeaponTypes.FirstOrDefault(x =>
								 x.Name.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase));
			if (weaponType == null)
			{
				actor.OutputHandler.Send("There is no such weapon type.");
				return;
			}

			attacks = attacks.Where(x => weaponType.Attacks.Contains(x));
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from attack in attacks
			select new[]
			{
				attack.Id.ToString("N0", actor),
				attack.Name,
				actor.Gameworld.WeaponTypes.FirstOrDefault(x => x.Attacks.Contains(attack))
					 ?.Name ?? "None",
				attack.MoveType.Describe(),
				attack.SpecialListText
			},
			new[] { "ID", "Name", "Weapon Type", "Move Type", "Special" },
			actor.LineFormatLength, colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void WeaponAttackClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which weapon attack would you like to clone?");
			return;
		}

		var attack = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.WeaponAttacks.Get(value)
			: actor.Gameworld.WeaponAttacks.GetByName(ss.Last);
		if (attack == null)
		{
			actor.OutputHandler.Send("There is no such weapon attack.");
			return;
		}

		var newattack = attack.CloneWeaponAttack();
		actor.Gameworld.Add(newattack);
		actor.OutputHandler.Send(
			$"You clone weapon attack #{attack.Id.ToString("N0", actor)} ({attack.Name}) into a new attack with id #{newattack.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IWeaponAttack>>());
		actor.AddEffect(new BuilderEditingEffect<IWeaponAttack>(actor) { EditingItem = newattack });
	}

	private static void WeaponAttackDelete(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<IWeaponAttack>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any weapon attacks.");
			return;
		}

		actor.OutputHandler.Send("TODO");
	}

	private static void WeaponAttackSet(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<IWeaponAttack>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any weapon attacks.");
			return;
		}

		actor.EffectsOfType<BuilderEditingEffect<IWeaponAttack>>().First().EditingItem.BuildingCommand(actor, ss);
	}

	private static void WeaponAttackClose(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<IWeaponAttack>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any weapon attacks.");
			return;
		}

		actor.OutputHandler.Send($"You are no longer editing any weapon attacks.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IWeaponAttack>>());
	}

	private static void WeaponAttackEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			if (actor.EffectsOfType<BuilderEditingEffect<IWeaponAttack>>().Any())
			{
				WeaponAttackView(actor, ss);
				return;
			}

			actor.OutputHandler.Send("Which weapon attack would you like to edit?");
			return;
		}

		var attack = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.WeaponAttacks.Get(value)
			: actor.Gameworld.WeaponAttacks.GetByName(ss.Last);
		if (attack == null)
		{
			actor.OutputHandler.Send("There is no such weapon attack.");
			return;
		}

		actor.OutputHandler.Send(
			$"You are now editing weapon attack #{attack.Id.ToString("N0", actor)} ({attack.Name}).");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IWeaponAttack>>());
		actor.AddEffect(new BuilderEditingEffect<IWeaponAttack>(actor) { EditingItem = attack });
	}

	private static void WeaponAttackNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type of weapon attack would you like to create? Valid types are {Enum.GetValues(typeof(BuiltInCombatMoveType)).OfType<BuiltInCombatMoveType>().Where(x => x.IsWeaponAttackType()).Select(x => x.Describe().Colour(Telnet.Green)).ListToString()}.");
			return;
		}

		if (!CombatExtensions.TryParseBuiltInMoveType(ss.PopSpeech(), out var value) ||
			!value.IsWeaponAttackType())
		{
			actor.OutputHandler.Send(
				$"That is not a valid attack type. Valid types are {Enum.GetValues(typeof(BuiltInCombatMoveType)).OfType<BuiltInCombatMoveType>().Where(x => x.IsWeaponAttackType()).Select(x => x.Describe().Colour(Telnet.Green)).ListToString()}.");
			return;
		}

		var newAttack = Combat.WeaponAttack.NewWeaponAttack(value, actor.Gameworld);
		actor.Gameworld.Add(newAttack);
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IWeaponAttack>>());
		actor.AddEffect(new BuilderEditingEffect<IWeaponAttack>(actor) { EditingItem = newAttack });
		actor.OutputHandler.Send(
			$"You create a new weapon attack of type {value.Describe()}, which you are now editing.");
	}

	private static void WeaponAttackNatural(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which race's natural attacks do you want to edit?");
			return;
		}

		var race = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Races.Get(value)
			: actor.Gameworld.Races.GetByName(ss.Last);
		if (race == null)
		{
			actor.OutputHandler.Send("There is no such race.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which weapon attack do you want to change for this race?");
			return;
		}

		var attack = long.TryParse(ss.PopSpeech(), out value)
			? actor.Gameworld.WeaponAttacks.Get(value)
			: actor.Gameworld.WeaponAttacks.GetByName(ss.Last);
		if (attack == null)
		{
			actor.OutputHandler.Send("There is no such weapon attack.");
			return;
		}

		if (actor.Gameworld.WeaponTypes.Any(x => x.Attacks.Contains(attack)))
		{
			actor.OutputHandler.Send(
				$"The {attack.Name.Colour(Telnet.Green)} weapon attack (#{attack.Id.ToString("N0", actor)}) is associated with weapons, so cannot be used for natural attacks.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"You can either specify a bodypart and a quality to add an attack, or use the keyword 'remove' to remove one.");
			return;
		}

		if (ss.Peek().EqualToAny("remove", "rem", "delete", "del"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				// Remove all attacks
				race.RemoveNaturalAttacksAssociatedWith(attack);
				actor.OutputHandler.Send(
					$"You remove all natural attacks associated with the {attack.Name.Colour(Telnet.Cyan)} weapon attack for the {race.Name.Colour(Telnet.Green)} race.");
				return;
			}

			var bodypart = race.BaseBody.AllExternalBodyparts.GetFromItemListByKeyword(ss.PopSpeech(), actor);
			if (bodypart == null)
			{
				actor.OutputHandler.Send("There is no such bodypart.");
				return;
			}

			var natural =
				race.NaturalWeaponAttacks.FirstOrDefault(x => x.Attack == attack && x.Bodypart == bodypart);
			if (natural == null)
			{
				actor.OutputHandler.Send(
					"There is no such natural attack for that race that matches the specified weapon attack and bodypart.");
				return;
			}

			race.RemoveNaturalAttack(natural);
			actor.OutputHandler.Send(
				$"You remove the natural attack to use {attack.Name.Colour(Telnet.Cyan)} with the {bodypart.FullDescription().Colour(Telnet.Yellow)} for the {race.Name.Colour(Telnet.Green)} race.");
			return;
		}

		var targetbodypart = race.BaseBody.AllExternalBodyparts.GetFromItemListByKeyword(ss.PopSpeech(), actor);
		if (targetbodypart == null)
		{
			actor.OutputHandler.Send("There is no such bodypart.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What quality should the attack with that bodypart be?");
			return;
		}

		if (!ss.PopSpeech().TryParseEnum(out ItemQuality quality))
		{
			actor.OutputHandler.Send("That is not a valid item quality.");
			return;
		}

		foreach (var similar in race.NaturalWeaponAttacks
									.Where(x => x.IsSimilarTo(attack, targetbodypart)).ToList())
		{
			race.RemoveNaturalAttack(similar);
		}

		race.AddNaturalAttack(new NaturalAttack
		{
			Attack = attack,
			Bodypart = targetbodypart,
			Quality = quality
		});

		actor.OutputHandler.Send(
			$"The {race.Name.Colour(Telnet.Green)} race now has the {attack.Name.Colour(Telnet.Green)} natural attack with the {targetbodypart.FullDescription().Colour(Telnet.Yellow)} bodypart at quality {quality.Describe().Colour(Telnet.Green)}.");
	}

	#endregion

	public const string AuxiliaryHelpText = @"This command is used to create and edit auxiliary moves, which are customisable, special moves that supplement weapon attacks. 

Unlike weapon attacks, auxiliaries have a number of addable effects that can be changed after creation. You can create quite complex auxiliary moves.

The syntax for this command is as follows:

	#3auxiliary list#0 - lists all auxiliary moves
	#3auxiliary edit <id|name>#0 - opens an auxiliary move for editing
	#3auxiliary edit#0 - an alias for AUXILIARY SHOW when you have something open
	#3auxiliary show <id|name>#0 - shows details about a particular auxiliary move
	#3auxiliary close#0 - closes the open auxiliary move
	#3auxiliary new <name>#0 - creates a new auxiliary move with the specified name
	#3auxiliary clone <which> <name>#0 - creates a carbon copy of the specified auxiliary move
	#3auxiliary natural <race> <attack>#0 - adds an auxiliary move to a race
	#3auxiliary natural <race> <attack> remove#0 - removes an auxiliary move from a race
	#3auxiliary set ...#0 - edits the properties of a auxiliary move. See that command for more help.";

	#region Auxiliary Moves
	[PlayerCommand("Auxiliary", "auxiliary")]
	[HelpInfo("Auxiliary", AuxiliaryHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Auxiliary(ICharacter actor, string text)
	{
		var ss = new StringStack(text.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "add":
			case "create":
			case "new":
				AuxiliaryNew(actor, ss);
				return;
			case "clone":
				AuxiliaryClone(actor, ss);
				return;
			case "list":
				AuxiliaryList(actor, ss);
				return;
			case "edit":
				AuxiliaryEdit(actor, ss);
				return;
			case "close":
				AuxiliaryClose(actor, ss);
				return;
			case "show":
			case "view":
				AuxiliaryShow(actor, ss);
				return;
			case "set":
				AuxiliarySet(actor, ss);
				return;
			case "natural":
				AuxiliaryNatural(actor, ss);
				return;
		}
	}

	private static void AuxiliaryNatural(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which race's natural auxiliary moves do you want to edit?");
			return;
		}

		var race = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Races.Get(value)
			: actor.Gameworld.Races.GetByName(ss.Last);
		if (race == null)
		{
			actor.OutputHandler.Send("There is no such race.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which auxiliary move do you want to change for this race?");
			return;
		}

		var attack = long.TryParse(ss.PopSpeech(), out value)
			? actor.Gameworld.AuxiliaryCombatActions.Get(value)
			: actor.Gameworld.AuxiliaryCombatActions.GetByName(ss.Last);
		if (attack == null)
		{
			actor.OutputHandler.Send("There is no such auxiliary move.");
			return;
		}

		if (ss.Peek().EqualToAny("remove", "rem", "delete", "del"))
		{
			if (race.RemoveAuxiliaryAction(attack))
			{
				actor.OutputHandler.Send($"You remove the auxiliary action {attack.Name.Colour(Telnet.Cyan)} from the {race.Name.Colour(Telnet.Green)} race.");
				return;
			}

			actor.OutputHandler.Send($"The {attack.Name.ColourName()} auxiliary move is provided by a parent race of {race.Name.ColourValue()}, and so must be removed from that race instead.");
			return;
		}

		if (race.AddAuxiliaryAction(attack))
		{
			actor.OutputHandler.Send($"The {race.Name.Colour(Telnet.Green)} race now has the {attack.Name.Colour(Telnet.Green)} auxiliary move.");
			return;
		}

		actor.OutputHandler.Send($"The {race.Name.ColourName()} race already has that auxiliary move.");
	}

	private static void AuxiliaryClose(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<IAuxiliaryCombatAction>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any auxiliary moves.");
			return;
		}

		actor.OutputHandler.Send($"You are no longer editing any auxiliary moves.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IAuxiliaryCombatAction>>());
	}

	private static void AuxiliaryClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which auxiliary move would you like to clone?");
			return;
		}

		var action = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.AuxiliaryCombatActions.Get(value)
			: actor.Gameworld.AuxiliaryCombatActions.GetByName(ss.Last);
		if (action == null)
		{
			actor.OutputHandler.Send("There is no such auxiliary move.");
			return;
		}
		var newAction = action.Clone();
		actor.Gameworld.Add(newAction);
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IAuxiliaryCombatAction>>());
		actor.AddEffect(new BuilderEditingEffect<IAuxiliaryCombatAction>(actor) { EditingItem = newAction });
		actor.OutputHandler.Send($"You clone auxiliary move {action.Name.ColourName()} into a new action with Id #{newAction.Id.ToString("N0", actor)}, which you are now editing.");
	}

	private static void AuxiliarySet(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<IAuxiliaryCombatAction>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any auxiliary moves.");
			return;
		}

		actor.EffectsOfType<BuilderEditingEffect<IAuxiliaryCombatAction>>().First().EditingItem.BuildingCommand(actor, ss);
	}

	private static void AuxiliaryShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished && !actor.EffectsOfType<BuilderEditingEffect<IAuxiliaryCombatAction>>().Any())
		{
			actor.OutputHandler.Send("You must specify an auxiliary move to show if you are not editing one.");
			return;
		}

		IAuxiliaryCombatAction action;
		if (ss.IsFinished)
		{
			action = actor.EffectsOfType<BuilderEditingEffect<IAuxiliaryCombatAction>>().First().EditingItem;
		}
		else
		{
			action = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.AuxiliaryCombatActions.Get(value)
				: actor.Gameworld.AuxiliaryCombatActions.GetByName(ss.Last);
			if (action == null)
			{
				actor.OutputHandler.Send("There is no such auxiliary move to show you.");
				return;
			}
		}

		actor.OutputHandler.Send(action.ShowBuilder(actor));
	}

	private static void AuxiliaryEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			if (actor.EffectsOfType<BuilderEditingEffect<IAuxiliaryCombatAction>>().Any())
			{
				AuxiliaryShow(actor, ss);
				return;
			}

			actor.OutputHandler.Send("Which auxiliary move would you like to edit?");
			return;
		}

		var action = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.AuxiliaryCombatActions.Get(value)
			: actor.Gameworld.AuxiliaryCombatActions.GetByName(ss.Last);
		if (action == null)
		{
			actor.OutputHandler.Send("There is no such auxiliary move.");
			return;
		}

		actor.OutputHandler.Send(
			$"You are now editing auxiliary move #{action.Id.ToString("N0", actor)} ({action.Name}).");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IAuxiliaryCombatAction>>());
		actor.AddEffect(new BuilderEditingEffect<IAuxiliaryCombatAction>(actor) { EditingItem = action });
	}

	private static void AuxiliaryList(ICharacter actor, StringStack ss)
	{
		if (ss.Peek().EqualToAny("help", "?"))
		{
			actor.OutputHandler.Send(@"You can use the following arguments to help refine your search:

	#3+key#0 - shows only attacks whose names have the keyword
	#3-key#0 - excludes weapon types whose names have the keyword".SubstituteANSIColour());
			return;
		}

		var actions = actor.Gameworld.AuxiliaryCombatActions.AsEnumerable();
		while (!ss.IsFinished)
		{
			var arg = ss.PopSpeech();
			if (arg[0] == '+' && arg.Length > 1)
			{
				arg = arg.Substring(1);
				actions = actions.Where(x => x.Name.Contains(arg));
				continue;
			}

			if (arg[0] == '-' && arg.Length > 1)
			{
				arg = arg.Substring(1);
				actions = actions.Where(x => !x.Name.Contains(arg));
				continue;
			}
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from action in actions
			select new[]
			{
				action.Id.ToString("N0", actor),
				action.Name,
				action.UsabilityProg?.MXPClickableFunctionName() ?? "",
				$"{action.BaseDelay.ToString("N2", actor)}s",
				action.StaminaCost.ToString("N2", actor),
				action.Weighting.ToString("N2", actor)

			},
			new[]
			{
				"ID",
				"Name",
				"Prog",
				"Delay",
				"Stamina",
				"Weighting"
			},
			actor.LineFormatLength, colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void AuxiliaryNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new auxiliary move?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		var newAction = new AuxiliaryCombatAction(name, actor.Gameworld);
		actor.Gameworld.Add(newAction);
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IAuxiliaryCombatAction>>());
		actor.AddEffect(new BuilderEditingEffect<IAuxiliaryCombatAction>(actor) { EditingItem = newAction });
		actor.OutputHandler.Send($"You create a new auxiliary move called {name.ColourName()} with Id #{newAction.Id.ToString("N0", actor)}, which you are now editing.");
	}

	#endregion

	#region Combat Messages

	private const string CombatMessageHelp =
		@"This command can be used to create, view and edit combat messages. Combat messages are used by weapon attacks, defenses and other combat moves.

The syntax for this command is as follows:

	#3cm list [<filters>]#0 - shows all combat messages. See below for filters.
	#3cm show <which>#0 - shows information about a combat message
	#3cm edit <which>#0 - begins editing a combat message
	#3cm edit#0 - an alias for #3cm show#0 on your currently edited message
	#3cm close#0 - closes the currently edited combat message
	#3cm new#0 - creates a new combat message
	#3cm clone <which>#0 - clones a combat message
	#3cm delete#0 - deletes the combat message you're currently editing
	#3cm set chance <%>#0 - the chance between 0 and 1 of this message being chosen
	#3cm set priority <number>#0 - the higher the number, the earlier the message will be evaluated to see if it applies
	#3cm set verb <verb>#0 - the verb this attack applies to, or NONE for all
	#3cm set outcome <outcome>#0 - the outcome this attack applies to, or NONE for all
	#3cm set type <type>#0 - the BuiltInCombatType this combat message is for
	#3cm set prog <prog id|name>#0 - the prog that controls whether this attack applies or NONE to clear.
	#3cm set message <message>#0 - the message for this attack
	#3cm set fail <message>#0 - the fail message for this attack
	#3cm set attack <id>#0 - toggles a specific weapon attack on or off for this message

You can also use the following options to filter searches:

	#6<verb>#0 - show all combat messages for the specified attack verb
	#6+<key>#0 - include messages with the specified text
	#6-<key>#0 - exclude messages with the specified text
	#6*<attack>#0 - include combat messages only that apply to specified weapon attack
	#6&<type>#0 - filters messages for a particular type";

	[PlayerCommand("CombatMessage", "combatmessage", "cm")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("combatmessage",
		CombatMessageHelp,
		AutoHelp.HelpArgOrNoArg)]
	protected static void CombatMessage(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "new":
				CombatMessageNew(actor, ss);
				return;
			case "edit":
				CombatMessageEdit(actor, ss);
				return;
			case "close":
				CombatMessageClose(actor, ss);
				return;
			case "set":
				CombatMessageSet(actor, ss);
				return;
			case "delete":
				CombatMessageDelete(actor, ss);
				return;
			case "clone":
				CombatMessageClone(actor, ss);
				return;
			case "list":
				CombatMessageList(actor, ss);
				return;
			case "show":
			case "view":
				CombatMessageView(actor, ss);
				return;
		}

		actor.OutputHandler.Send("The valid choices are list, show, new, edit, close, set, clone and delete.");
	}

	private static void CombatMessageView(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished && !actor.EffectsOfType<BuilderEditingEffect<ICombatMessage>>().Any())
		{
			actor.OutputHandler.Send("You must specify a combat message to show if you are not editing one.");
			return;
		}

		ICombatMessage message;
		if (ss.IsFinished)
		{
			message = actor.EffectsOfType<BuilderEditingEffect<ICombatMessage>>().First().EditingItem;
		}
		else
		{
			message = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.CombatMessageManager.CombatMessages.FirstOrDefault(x => x.Id == value)
				: default;
			if (message == null)
			{
				actor.OutputHandler.Send("There is no such combat message to show you.");
				return;
			}
		}

		actor.OutputHandler.Send(message.ShowBuilder(actor));
	}

	private static void CombatMessageList(ICharacter actor, StringStack ss)
	{
		if (ss.Peek().EqualToAny("help", "?"))
		{
			actor.OutputHandler.Send(
				@"You can use the following options to help refine your search:

	#6<verb>#0 - show all combat messages for the specified attack verb
	#6+<key>#0 - include messages with the specified text
	#6-<key>#0 - exclude messages with the specified text
	#6*<attack>#0 - include combat messages only that apply to specified weapon attack
	#6&<type>#0 - filters messages for a particular type");
			return;
		}

		var messages = actor.Gameworld.CombatMessageManager.CombatMessages.AsEnumerable();
		while (!ss.IsFinished)
		{
			var text = ss.PopSpeech();
			if (text[0] == '+' && text.Length > 1)
			{
				text = text.Substring(1);
				messages = messages.Where(x =>
					x.Message.Contains(text, StringComparison.InvariantCultureIgnoreCase));
				continue;
			}

			if (text[0] == '-' && text.Length > 1)
			{
				text = text.Substring(1);
				messages = messages.Where(x =>
					!x.Message.Contains(text, StringComparison.InvariantCultureIgnoreCase));
				continue;
			}

			if (text[0] == '*' && text.Length > 1)
			{
				text = text.Substring(1);
				var attack = long.TryParse(text, out var value)
					? actor.Gameworld.WeaponAttacks.Get(value)
					: actor.Gameworld.WeaponAttacks.GetByName(text);
				if (attack == null)
				{
					actor.OutputHandler.Send(
						$"There is no weapon attack referenced by {text.Colour(Telnet.Yellow)} that you can filter combat messages by.");
					return;
				}

				messages = messages.Where(x => x.CouldApply(attack));
				continue;
			}

			if (text[0] == '&' && text.Length > 1)
			{
				text = text.Substring(1);
				if (!text.TryParseEnum<BuiltInCombatMoveType>(out var type))
				{
					actor.OutputHandler.Send(
						$"That is not a valid built-in combat move type. Valid types are {Enum.GetValues(typeof(BuiltInCombatMoveType)).OfType<BuiltInCombatMoveType>().Select(x => x.DescribeEnum().Colour(Telnet.Green)).ListToString()}.");
					return;
				}

				messages = messages.Where(x => x.Type == type);
				continue;
			}

			if (!text.TryParseEnum<MeleeWeaponVerb>(out var verb))
			{
				actor.OutputHandler.Send(
					$"That is not a valid melee attack verb. Valid choices are {Enum.GetValues(typeof(MeleeWeaponVerb)).OfType<MeleeWeaponVerb>().Select(x => x.Describe().ColourValue()).ListToString()}.");
				return;
			}

			messages = messages.Where(x => x.Verb.HasValue && x.Verb == verb);
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from message in messages
			select new[]
			{
				message.Id.ToString("N0", actor),
				message.Priority.ToString("N0", actor),
				message.Type.DescribeEnum(),
				message.Chance.ToString("P3", actor),
				message.Verb?.Describe() ?? "Any",
				message.Outcome?.DescribeColour() ?? "Any",
				message.WeaponAttackProg != null
					? $"{message.WeaponAttackProg.FunctionName} (#{message.WeaponAttackProg.Id})".FluentTagMXP(
						"send", $"href='show futureprog {message.WeaponAttackProg.Id}'")
					: "None",
				message.Message
			},
			new[] { "ID", "Priority", "Type", "Chance", "Verb", "Outcome", "Prog", "Message" },
			actor.LineFormatLength, truncatableColumnIndex: 6, colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void CombatMessageClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which combat message would you like to clone?");
			return;
		}

		var message = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.CombatMessageManager.CombatMessages.FirstOrDefault(x => x.Id == value)
			: default;
		if (message == null)
		{
			actor.OutputHandler.Send("There is no such combat message to clone.");
			return;
		}

		var newmessage = new CombatMessage((CombatMessage)message);
		actor.Gameworld.CombatMessageManager.AddCombatMessage(newmessage);
		actor.OutputHandler.Send(
			$"You clone combat message #{message.Id.ToString("N0", actor)} into a new message with id #{newmessage.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ICombatMessage>>());
		actor.AddEffect(new BuilderEditingEffect<ICombatMessage>(actor) { EditingItem = newmessage });
	}

	private static void CombatMessageDelete(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<ICombatMessage>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any combat messages.");
			return;
		}

		var cm = actor.EffectsOfType<BuilderEditingEffect<ICombatMessage>>().First().EditingItem;

		actor.OutputHandler.Send($"Are you sure that you want to permanently delete the following combat message (this cannot be undone).\n{cm.Message.ColourCommand()}\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = $"Deleting combat message {cm.Message.ColourCommand()}",
			AcceptAction = text =>
			{
				actor.OutputHandler.Send($"You delete the combat message {cm.Message.ColourCommand()} (#{cm.Id.ToString("N0", actor)}).");
				cm.Delete();
				actor.Gameworld.Destroy(cm);
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to delete the combat message.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide not to delete the combat message.");
			}
		}), TimeSpan.FromSeconds(120));
	}

	private static void CombatMessageSet(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<ICombatMessage>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any combat messages.");
			return;
		}

		actor.EffectsOfType<BuilderEditingEffect<ICombatMessage>>().First().EditingItem.BuildingCommand(actor, ss);
	}

	private static void CombatMessageClose(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<ICombatMessage>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any combat messages.");
			return;
		}

		actor.OutputHandler.Send($"You are no longer editing any combat messages.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ICombatMessage>>());
	}

	private static void CombatMessageEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			if (actor.EffectsOfType<BuilderEditingEffect<ICombatMessage>>().Any())
			{
				CombatMessageView(actor, ss);
				return;
			}

			actor.OutputHandler.Send("Which combat message would you like to edit?");
			return;
		}

		var message = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.CombatMessageManager.CombatMessages.FirstOrDefault(x => x.Id == value)
			: default;
		if (message == null)
		{
			actor.OutputHandler.Send("There is no such combat message to edit.");
			return;
		}

		actor.OutputHandler.Send($"You are now editing combat message #{message.Id.ToString("N0", actor)}.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ICombatMessage>>());
		actor.AddEffect(new BuilderEditingEffect<ICombatMessage>(actor) { EditingItem = message });
	}

	private static void CombatMessageNew(ICharacter actor, StringStack ss)
	{
		var newmessage = new CombatMessage(actor.Gameworld);
		actor.Gameworld.CombatMessageManager.AddCombatMessage(newmessage);
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ICombatMessage>>());
		actor.AddEffect(new BuilderEditingEffect<ICombatMessage>(actor) { EditingItem = newmessage });
		actor.OutputHandler.Send(
			$"You create a new combat message with ID #{newmessage.Id}, which you are now editing.");
	}

	#endregion

	public const string ArmourTypeHelp = @"The #3Armour#0 command allows you to create and edit armour types. Armour types can be added to items, bodyparts and spells to reduce incoming damage.

You can use the following syntax with this command:

	#3armour list#0 - lists all armour types
	#3armour edit <id|name>#0 - opens the specified armour type for editing
	#3armour edit new <name>#0 - creates a new armour type for editing
	#3armour edit#0 - equivalent of doing SHOW on your currently editing armour type
	#3armour close#0 - closes the currently edited armour type
	#3armour clone <id|name> <new name>#0 - creates a carbon copy of an armour type for editing
	#3armour show <id|name>#0 - shows a particular armour type
	#3armour set name <name>#0 - renames the armour type
	#3armour set penetration <outcome>#0 - sets the minimum outcome required for penetration
	#3armour set difficulty <bonus>#0 - sets the base penalty for wearing this armour
	#3armour set stacked <bonus>#0 - sets the penalty for wearing this armour when stacked
	#3armour set transform <from> <to> <severity>#0 - sets a damage type transformation
	#3armour set transform <type> none#0 - clears a damage type transform
	#3armour set dissipate damage|stun|pain <damagetype> <formula>#0 - sets the dissipate damage/stun/pain formula for a damage type
	#3armour set absorb damage|stun|pain <damagetype> <formula>#0 - sets the absorb damage/stun/pain formula for a damage type

Note, the formulas use the following parameters:

	#6damage#0 - the raw damage amount
	#6angle#0 - the angle in radians that the attack struck at
	#6density#0 - the density of the armour material in kg/m3
	#6strength#0 - the yield strength (shear or impact depending on damage type) of the armour material in Pascals
	#6electrical#0 - the electrical conductivity of the armour material in 1/ohms
	#6thermal#0 - the thermal conductivity of the armour material in W/m/DegK
	#6organic#0 - 1 if armour material is organic, 0 if not

Additionally, absorb formulas can use the following parameter:

	#6originaldamage#0 - the original damage, before dissipation step";

	#region Armour Types
	[PlayerCommand("Armour", "armour", "armor")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("Armour", ArmourTypeHelp, AutoHelp.HelpArgOrNoArg)]
		protected static void Armour(ICharacter actor, string command)
		{
				GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.ArmourTypeHelper);
		}
		#endregion

		#region Ranged Cover

		public const string RangedCoverHelp = @"The #3rcover#0 command is used to create and edit ranged cover types.

You can use the following syntax with this command:

		#3rcover list#0 - lists all covers
		#3rcover edit <id|name>#0 - begins editing a cover
		#3rcover edit new <name>#0 - creates a new cover
		#3rcover close#0 - stops editing a cover
		#3rcover clone <id|name> <name>#0 - clones an existing cover
		#3rcover show <id|name>#0 - shows a cover
		#3rcover set name <name>#0 - renames the cover
		#3rcover set type <type>#0 - sets the cover type (hard or soft)
		#3rcover set extent <extent>#0 - sets the cover extent
		#3rcover set position <position>#0 - sets the highest position state
		#3rcover set desc <emote>#0 - sets the description emote ($0 is the cover item)
		#3rcover set action <emote>#0 - sets the action emote ($0 is the character, $1 is the cover item)
		#3rcover set max <##>#0 - sets the maximum simultaneous covers
		#3rcover set moving#0 - toggles the moving flag

For information on emote syntax see #3help emote#0.";

		[PlayerCommand("RangedCover", "rangedcover", "rcover")]
		[CommandPermission(PermissionLevel.Admin)]
		[HelpInfo("RangedCover", RangedCoverHelp, AutoHelp.HelpArgOrNoArg)]
		protected static void RangedCover(ICharacter actor, string command)
		{
				GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.RangedCoverHelper);
		}

		#endregion
}