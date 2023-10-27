using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using Org.BouncyCastle.Asn1.Sec;

namespace MudSharp.Commands.Modules;

public class MagicModule : Module<ICharacter>
{
	private MagicModule()
		: base("Magic")
	{
		IsNecessary = true;
	}

	public static MagicModule Instance { get; } = new();

	public static bool MagicFilterFunction(object actorObject, string commandWord)
	{
		var actor = (ICharacter)actorObject;
		if (actor.Capabilities.Any(x => x.School.SchoolVerb.EqualTo(commandWord)))
		{
			return true;
		}

		return false;
	}

	public static void MagicGeneric(ICharacter actor, string command)
	{
		var ss = new StringStack(command);
		var invoked = ss.PopSpeech().ToLowerInvariant();
		var cmdText = ss.PopSpeech();
		var school = actor.Capabilities.Select(x => x.School).Distinct().FirstOrDefault(x =>
			x.SchoolVerb.StartsWith(invoked, StringComparison.InvariantCultureIgnoreCase));
		if (school == null)
		{
			actor.OutputHandler.Send("Something went wrong with this command.");
			return;
		}

		if (string.IsNullOrEmpty(cmdText))
		{
			MagicStatus(actor, school);
			return;
		}

		if (cmdText.EqualToAny("?", "help") && ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You can use the {$"{invoked} powers".ColourCommand()} command to list all of your powers, {$"{invoked} help <powername>".ColourCommand()} to get help on the usage of a power, or {$"{invoked} <power command>".ColourCommand()} to invoke that power (see individual power help for the commands).");
			return;
		}

		var schools = actor.Capabilities.Select(x => x.School).Distinct().Where(x =>
			                   x.SchoolVerb.StartsWith(invoked, StringComparison.InvariantCultureIgnoreCase))
		                   .ToList();

		var powers = actor.Powers.Where(x => schools.Contains(x.School)).OrderBy(x => x.Name).ToList();
		if (cmdText.EqualTo("powers"))
		{
			var sb = new StringBuilder();
			sb.AppendLine(
				$"You have the following {schools.Select(x => x.SchoolAdjective).Distinct().ListToString()} powers:");
			sb.Append(StringUtilities.GetTextTable(
				from item in powers
				select new[] { item.Name, item.Blurb },
				new[] { "Name", "Blurb" },
				actor.LineFormatLength,
				colour: schools.First().PowerListColour, truncatableColumnIndex: 1,
				unicodeTable: actor.Account.UseUnicode
			));
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		if (cmdText.EqualTo("spells"))
		{
			var sb = new StringBuilder();
			sb.AppendLine(
				$"You have the following {schools.Select(x => x.SchoolAdjective).Distinct().ListToString()} spells:");
			sb.Append(StringUtilities.GetTextTable(
				from item in actor.Gameworld.MagicSpells.Where(x => schools.Contains(x.School) && x.ReadyForGame)
				select new[] { item.Name, item.Blurb },
				new[] { "Name", "Blurb" },
				actor.LineFormatLength,
				colour: schools.First().PowerListColour, truncatableColumnIndex: 1,
				unicodeTable: actor.Account.UseUnicode
			));
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		if (cmdText.EqualTo("cast"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which spell do you want to cast?");
				return;
			}

			var spell = actor.Gameworld.MagicSpells.Where(x =>
				                 schools.Contains(x.School) && x.ReadyForGame &&
				                 x.SpellKnownProg.Execute<bool?>(actor, x) == true)
			                 .GetByNameOrAbbreviation(ss.PopSpeech());
			if (spell == null)
			{
				actor.OutputHandler.Send("You do not know any such spell.");
				return;
			}

			if (spell.Trigger is not ICastMagicTrigger ct)
			{
				actor.OutputHandler.Send(
					$"The spell {spell.Name.Colour(spell.School.PowerListColour)} is not of the sort that can be cast with the cast command.");
				return;
			}

			ct.DoTriggerCast(actor, ss);
			return;
		}

		if (cmdText.EqualToAny("spellhelp", "spell"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which spell do you want to get help on?");
				return;
			}

			var spell = actor.Gameworld.MagicSpells.Where(x =>
				                 schools.Contains(x.School) && x.ReadyForGame &&
				                 x.SpellKnownProg.Execute<bool?>(actor, x) == true)
			                 .GetByNameOrAbbreviation(ss.PopSpeech());
			if (spell == null)
			{
				actor.OutputHandler.Send("You do not know any such spell.");
				return;
			}

			actor.OutputHandler.Send(spell.ShowPlayerHelp(actor));
			return;
		}

		IMagicPower power;
		if (cmdText.EqualTo("help"))
		{
			var powerText = ss.PopSpeech();
			power = powers.FirstOrDefault(x => x.Name.EqualTo(powerText)) ??
			        powers.FirstOrDefault(
				        x => x.Name.StartsWith(powerText, StringComparison.InvariantCultureIgnoreCase));
			if (power == null)
			{
				actor.OutputHandler.Send(
					$"You have no such power. See {$"{invoked} powers".ColourCommand()} for a list of your powers.");
				return;
			}

			actor.OutputHandler.Send(power.ShowHelp(actor));
			return;
		}

		power = powers.FirstOrDefault(x => x.Verbs.Any(y => y.EqualTo(cmdText))) ??
		        powers.FirstOrDefault(
			        x => x.Verbs.Any(y => y.StartsWith(cmdText, StringComparison.InvariantCultureIgnoreCase)));
		if (power == null)
		{
			actor.OutputHandler.Send(
				$"You have no such power. See {$"{invoked} powers".ColourCommand()} for a list of your powers.");
			return;
		}

		if (actor.AffectedBy<CommandDelayMagicPower>(power))
		{
			actor.OutputHandler.Send("You can't do that again yet.");
			return;
		}

		var verb = power.Verbs.FirstOrDefault(x => x.EqualTo(cmdText)) ??
		           power.Verbs.First(x => x.StartsWith(cmdText, StringComparison.InvariantCultureIgnoreCase));
		power.UseCommand(actor, verb, ss);
	}

	private static void MagicStatus(ICharacter actor, IMagicSchool school)
	{
		var sb = new StringBuilder();
		var capabilities = actor.Capabilities.Where(x => x.School == school).ToList();
		sb.AppendLine($"You are {capabilities.Select(x => x.Name.A_An(false, Telnet.Magenta)).ListToString()}");


		var concentration = capabilities.Max(x => x.ConcentrationAbility(actor));
		sb.AppendLine(
			$"You are sustaining {actor.CombinedEffectsOfType<IConcentrationConsumingEffect>().Sum(x => x.ConcentrationPointsConsumed).ToString("N2", actor).ColourValue()} / {concentration.ToString("N2", actor).ColourValue()} concentration points worth of powers.");
		foreach (var resource in actor.MagicResources)
		{
			sb.AppendLine(
				$"You currently have {actor.MagicResourceAmounts[resource].ToString("N2", actor).ColourValue()}/{resource.ResourceCap(actor).ToString("N2", actor).ColourValue()} {resource.Name}.");
		}

		foreach (var effect in actor.CombinedEffectsOfType<IMagicEffect>().Where(x => x.School == school))
		{
			sb.AppendLine($"You are currently sustaining the {effect.PowerOrigin.Name.Colour(Telnet.Magenta)} power.");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Magic", "magic")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Magic(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "school":
				MagicSchool(actor, ss);
				return;
			case "capability":
				MagicCapability(actor, ss);
				return;
			case "regenerator":
				MagicRegenerator(actor, ss);
				return;
			case "power":
				MagicPower(actor, ss);
				return;
			case "resource":
				MagicResource(actor, ss);
				return;
			case "spell":
				MagicSpell(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(@"You can use the following sub commands to edit different components of the magic system. See individual commands for help on them:

	#3magic school#0 - magic schools are types of magic
	#3magic capability#0 - magic capabilities control who can use magic
	#3magic resource#0 - magic resources are power for spells and abilities
	#3magic regenerator#0 - magic regenerators produce magic resources based on rules
	#3magic power#0 - magic powers are customisable hard-coded powers for a magic school
	#3magic spell#0 - magic spells are completely flexible and editable templates for magical effects

#ENote - It's relatively easy to add new spell effect types. Reach out to Japheth on the FutureMUD discord if you want something added.#0".SubstituteANSIColour());
				return;
		}
	}

	public static void MagicSchool(ICharacter actor, StringStack command)
	{
		BuilderModule.GenericBuildingCommand(actor, command, EditableItemHelper.MagicSchoolHelper);
	}

	public static void MagicCapability(ICharacter actor, StringStack command)
	{
		BuilderModule.GenericBuildingCommand(actor, command, EditableItemHelper.MagicCapabilityHelper);
	}

	public static void MagicRegenerator(ICharacter actor, StringStack command)
	{
		BuilderModule.GenericBuildingCommand(actor, command, EditableItemHelper.MagicRegeneratorHelper);
	}

	public static void MagicPower(ICharacter actor, StringStack command)
	{
		BuilderModule.GenericBuildingCommand(actor, command, EditableItemHelper.MagicPowerHelper);
	}

	public static void MagicResource(ICharacter actor, StringStack command)
	{
		BuilderModule.GenericBuildingCommand(actor, command, EditableItemHelper.MagicResourceHelper);
	}

	#region Magic Spells

	public static void MagicSpell(ICharacter actor, StringStack command)
	{
		BuilderModule.GenericBuildingCommand(actor, command, EditableItemHelper.MagicSpellHelper);
	}

	#endregion
}