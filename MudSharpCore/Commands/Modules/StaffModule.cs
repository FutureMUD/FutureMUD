using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Dapper;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Construction.Grids;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Editor;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Email;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Logging;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using Account = MudSharp.Accounts.Account;
using Exit = MudSharp.Construction.Boundary.Exit;
using TimeZoneInfo = System.TimeZoneInfo;
using MudSharp.NPC;

namespace MudSharp.Commands.Modules;

internal class StaffModule : Module<ICharacter>
{
	private StaffModule()
		: base("Staff")
	{
		IsNecessary = true;
	}

	public static StaffModule Instance { get; } = new();

	[PlayerCommand("PartInfo", "partinfo")]
	[HelpInfo("partinfo", "Full info report on a body's specific bodypart. Syntax: partinfo <target> <partName>",
		AutoHelp.HelpArgOrNoArg)]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void PartInfo(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which bodypart do you want to view?");
			return;
		}

		var partText = ss.Pop();
		var part = target.Body.Bodyparts.Concat(target.Body.Bodyparts.SelectMany(x => x.Organs)).Distinct()
		                 .FirstOrDefault(x => x.Name.StartsWith(partText, StringComparison.InvariantCultureIgnoreCase));
		if (part == null)
		{
			actor.Send(StringUtilities.HMark + $"{target.HowSeen(actor, true)} does not have any such bodypart.");
			actor.Send(StringUtilities.HMark + $"Valid parts include: " +
			           $"{target.Body.Bodyparts.Select(x => x.Name).ListToString()}" +
			           $"{target.Body.Bodyparts.SelectMany(x => x.Organs).Distinct().Select(y => y.Name).ListToString()}"
			);

			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"Showing info for {target.HowSeen(actor, false, DescriptionType.Possessive)} bodypart {part.FullDescription().Colour(Telnet.Yellow)}:");
		sb.AppendLine($"\tMaterial: {target.Body.GetMaterial(part).Name.ColourValue()}");
		sb.AppendLine($"\tAlignment: {part.Alignment.Describe().ColourValue()}");
		sb.AppendLine($"\tOrientation: {part.Orientation.Describe().ColourValue()}");
		sb.AppendLine(
			$"\tHP/Pain to Disable: {target.Body.HitpointsForBodypart(part).ToString("N2", actor).ColourValue()}");
		if (part.CanSever)
		{
			sb.AppendLine(
				$"\tHP to Sever: {target.Race.ModifiedSeverthreshold(part).ToString("N2", actor).ColourValue()}");
		}

		sb.AppendLine($"\tDamage Modifer: {part.DamageModifier.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"\tPain Modifer: {part.PainModifier.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"\tStun Modifer: {part.StunModifier.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"\tIs Vital: {part.IsVital.ToColouredString()}");
		sb.AppendLine($"\tIs Significant: {part.Significant.ToColouredString()}");
		sb.AppendLine(
			$"\tOrgans: {part.Organs.Select(x => x.FullDescription().Colour(Telnet.Green)).DefaultIfEmpty("None").ListToString()}");
		sb.AppendLine($"\tConnects to: {part.UpstreamConnection?.FullDescription() ?? "Nothing"}");
		sb.AppendLine($"\tHit Chance: {part.RelativeHitChance.ToString("N2", actor).ColourValue()}");

		actor.Send(sb.ToString());
	}

	[PlayerCommand("SetHeight", "setheight")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void SetHeight(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Whose height do you want to edit?");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone here like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("How tall should I make them?");
			return;
		}

		var result =
			actor.Gameworld.UnitManager.GetBaseUnits(ss.SafeRemainingArgument, UnitType.Length, out var success);
		if (!success)
		{
			actor.Send("That is not a valid height.");
			return;
		}

		target.Body.Height = result;
		target.Body.TotalBloodVolumeLitres = Character.Character.TotalBloodVolume(target);
		target.Body.CurrentBloodVolumeLitres = target.Body.TotalBloodVolumeLitres;
		actor.Send(
			$"You change the height of {target.HowSeen(actor)} to {actor.Gameworld.UnitManager.DescribeExact(result, UnitType.Length, actor)}.");
	}

	[PlayerCommand("SetWeight", "setweight")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void SetWeight(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Whose weight do you want to edit?");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone here like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("How heavy should I make them?");
			return;
		}

		var result =
			actor.Gameworld.UnitManager.GetBaseUnits(ss.SafeRemainingArgument, UnitType.Mass, out var success);
		if (!success)
		{
			actor.Send("That is not a valid weight.");
			return;
		}

		target.Body.Weight = result;
		target.Body.TotalBloodVolumeLitres = Character.Character.TotalBloodVolume(target);
		target.Body.CurrentBloodVolumeLitres = target.Body.TotalBloodVolumeLitres;
		actor.Send(
			$"You change the weight of {target.HowSeen(actor)} to {actor.Gameworld.UnitManager.DescribeExact(result, UnitType.Mass, actor)}.");
	}

	[PlayerCommand("SetCharacteristic", "setcharacteristic")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("setcharacteristic",
		"This command is used to set a characteristic for a character or item. The syntax is setcharacteristic <target> <definition> <value>.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void SetCharacteristic(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (actor.Target(ss.Pop()) is not IPerceivableHaveCharacteristics target)
		{
			actor.Send("There is nothing or noone like that here for you to set the characteristics of.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send($"Which characteristic do you want to set for {target.HowSeen(actor)}?");
			return;
		}

		var definition = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Characteristics.Get(value)
			: actor.Gameworld.Characteristics.FirstOrDefault(x => x.Pattern.IsMatch(ss.Last));
		if (definition == null)
		{
			actor.Send($"There is no such characteristic for you to set for {target.HowSeen(actor)}.");
			return;
		}

		if (!target.CharacteristicDefinitions.Contains(definition))
		{
			actor.Send(
				$"{target.HowSeen(actor, true)} does not have the {definition.Name.Colour(Telnet.Green)} characteristic.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send(
				$"Which characteristic value do you want to set for the {definition.Name.Colour(Telnet.Green)} characteristic for {target.HowSeen(actor)}?");
			return;
		}

		var cvalue = long.TryParse(ss.PopSpeech(), out value)
			? actor.Gameworld.CharacteristicValues.FirstOrDefault(x => definition.IsValue(x) && x.Id == value)
			: actor.Gameworld.CharacteristicValues.FirstOrDefault(x => x.Name.EqualTo(ss.Last));
		if (cvalue == null)
		{
			actor.Send(
				$"There is no such characteristic value for {definition.Name.Colour(Telnet.Green)} for you to set for {target.HowSeen(actor)}.");
			return;
		}

		target.SetCharacteristic(definition, cvalue);
		actor.Send(
			$"You set the {definition.Name.Colour(Telnet.Green)} characteristic for {target.HowSeen(actor)} to have a value of {cvalue.Name.Colour(Telnet.Green)}.");
	}

	[PlayerCommand("GiveAccent", "giveaccent")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void GiveAccent(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("To whom do you want to give a new accent?");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone like that to give an accent to.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which accent do you want to give them?");
			return;
		}

		IAccent accent = null;
		if (long.TryParse(ss.PopSpeech(), out var value))
		{
			accent = actor.Gameworld.Accents.Get(value);
		}
		else
		{
			var accents = actor.Gameworld.Accents.Get(ss.Last);
			if (accents.Count == 1)
			{
				accent = accents.First();
			}
			else if (accents.Count > 1)
			{
				actor.Send(
					$"That matched multiple accents. Try using the id. Accents matched include: {accents.Select(x => $"{x.Name} ({x.Id})").ListToString()}");
				return;
			}
		}

		if (accent == null)
		{
			actor.Send("There is no such accent.");
			return;
		}

		var diff = Difficulty.Automatic;
		if (!ss.IsFinished)
		{
			if (!CheckExtensions.GetDifficulty(ss.SafeRemainingArgument, out diff))
			{
				actor.Send("No such difficulty. Either leave it blank, or see SHOW DIFFICULTIES.");
				return;
			}
		}

		if (target.Accents.Contains(accent) && target.AccentDifficulty(accent, false) <= diff)
		{
			actor.Send("The target already has that accent at that difficulty or better.");
			return;
		}

		target.LearnAccent(accent, diff);
		actor.Send(
			$"You give the accent {accent.Name.Colour(Telnet.Green)} to {target.HowSeen(actor)} at a familiarity of {diff.Describe()}.");
	}

	[PlayerCommand("GiveKnowledge", "giveknowledge")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("giveknowledge", "Grant Knowledge to character via: giveknowledge <character> <knowledge>",
		AutoHelp.HelpArgOrNoArg)]
	protected static void GiveKnowledge(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who do you want to give a knowledge to?");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which knowledge do you want to give them?");
			return;
		}

		var name = ss.SafeRemainingArgument;
		var knowledge = long.TryParse(name, out var value)
			? actor.Gameworld.Knowledges.Get(value)
			: actor.Gameworld.Knowledges.GetByName(name);
		if (knowledge == null)
		{
			actor.Send("There is no such knowledge.");
			return;
		}

		if (target.Knowledges.Contains(knowledge))
		{
			actor.Send($"{target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} already knows that knowledge.");
			return;
		}

		target.AddKnowledge(new RPG.Knowledge.CharacterKnowledge(target, knowledge, "teach"));
		actor.Send($"You add the {knowledge.Description} knowledge to {target.HowSeen(actor)}.");
	}

	[PlayerCommand("RemoveKnowledge", "removeknowledge")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("removeknowledge", "Remove Knowledge from character via: removeknowledge <character> <knowledge>",
		AutoHelp.HelpArgOrNoArg)]
	protected static void RemoveKnowledge(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		if (ss.CountRemainingArguments() < 2)
		{
			actor.Send(StringUtilities.HMark + "Syntax: removeknowledge <character> <knowledge>");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone named that.");
			return;
		}

		var name = ss.SafeRemainingArgument;
		var knowledge = long.TryParse(name, out var value)
			? actor.Gameworld.Knowledges.Get(value)
			: actor.Gameworld.Knowledges.GetByName(name);
		if (knowledge == null)
		{
			actor.Send("There is no such knowledge.");
			return;
		}

		if (!target.Knowledges.Contains(knowledge))
		{
			actor.Send($"{target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} does not have that knowledge.");
			return;
		}

		target.RemoveKnowledge(knowledge);
		actor.Send($"You remove the {knowledge.Description} knowledge from {target.HowSeen(actor)}.");
	}

	[PlayerCommand("GiveMerit", "givemerit")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("givemerit", "Grant a merit. Syntax: givemerit <target> <meritname>", AutoHelp.HelpArgOrNoArg)]
	protected static void GiveMerit(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		if (ss.Peek().Equals("list", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.Send(StringUtilities.HMark + "Known merits include: " +
			           actor.Gameworld.Merits.Select(x => x.Name).ToList().ListToString());
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send(StringUtilities.HMark + "You don't see anyone like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send(StringUtilities.HMark + "Which merit do you want to give them?");
			return;
		}

		var merit = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Merits.Get(value)
			: actor.Gameworld.Merits.GetByName(ss.SafeRemainingArgument);

		if (merit == null)
		{
			actor.Send(StringUtilities.HMark +
			           "There is no such merit. Use 'givemerit list' to see a list of valid merits.");
			return;
		}

		if (merit.MeritScope == RPG.Merits.MeritScope.Character)
		{
			if (target.Merits.Contains(merit))
			{
				actor.Send(StringUtilities.HMark +
				           $"{target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} already has that merit.");
				return;
			}

			if (!target.AddMerit(merit))
			{
				actor.Send(StringUtilities.HMark + "Failed to add merit due to unknown error.");
				return;
			}
		}
		else if (merit.MeritScope == RPG.Merits.MeritScope.Body)
		{
			if (target.Body.Merits.Contains(merit))
			{
				actor.Send(StringUtilities.HMark +
				           $"{target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} already has that merit.");
				return;
			}

			if (!target.Body.AddMerit(merit))
			{
				actor.Send(StringUtilities.HMark + "Failed to add merit due to unknown error.");
				return;
			}

			target.Body.RecalculatePartsAndOrgans(); // Merits can cause bodyparts to change
		}

		actor.Send(StringUtilities.HMark +
		           $"{merit.Name.Colour(Telnet.Cyan)} granted to {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)}.");
	}

	[PlayerCommand("RemoveMerit", "removemerit")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("removemerit", "Remove a merit. Syntax: removemerit <target> <meritname>", AutoHelp.HelpArgOrNoArg)]
	protected static void RemoveMerit(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send(StringUtilities.HMark + "You don't see anyone that looks like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send(StringUtilities.HMark + "Which merit do you want to remove?");
			return;
		}

		var merit = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Merits.Get(value)
			: actor.Gameworld.Merits.GetByName(ss.SafeRemainingArgument);

		if (merit == null)
		{
			actor.Send(StringUtilities.HMark +
			           "There is no such merit. Use 'givemerit list' to see a list of valid merits.");
			return;
		}

		if (merit.MeritScope == RPG.Merits.MeritScope.Character)
		{
			if (!target.Merits.Contains(merit))
			{
				actor.Send(StringUtilities.HMark +
				           $"{target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} does not have that merit.");
				return;
			}

			if (!target.RemoveMerit(merit))
			{
				actor.Send(StringUtilities.HMark + "Failed to remove merit for unknown reason.");
				return;
			}
		}
		else if (merit.MeritScope == RPG.Merits.MeritScope.Body)
		{
			if (!target.Body.Merits.Contains(merit))
			{
				actor.Send(StringUtilities.HMark +
				           $"{target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} does not have that merit.");
				return;
			}

			if (!target.Body.RemoveMerit(merit))
			{
				actor.Send(StringUtilities.HMark + "Failed to remove merit for unknown reason.");
				return;
			}
		}

		actor.Send(StringUtilities.HMark +
		           $"Removed {merit.Name.Colour(Telnet.Cyan)} from {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)}.");
	}

	[PlayerCommand("FinishTattoo", "finishtattoo")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("finishtattoo", "Finishes someone's unfinished tatoo. Syntax: finishtattoo <target> <tattoo>",
		AutoHelp.HelpArgOrNoArg)]
	protected static void FinishTattoo(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetActorOrCorpse(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which of their unfinished tattoos do you want to finish off?");
			return;
		}

		var tattoo = target.Body.Tattoos.Where(x => x.CompletionPercentage < 1.0)
		                   .GetFromItemListByKeyword(ss.PopSpeech(), actor);
		if (tattoo == null)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote("$1 $1|have|has no such unfinished tattoo.", actor,
				actor, target)));
			return;
		}

		tattoo.CompletionPercentage = 1.0;
		target.Body.TattoosChanged = true;
		actor.OutputHandler.Send(new EmoteOutput(new Emote(
			$"You finish off the {tattoo.ShortDescription.Colour(Telnet.BoldOrange)} tattoo on $1's {tattoo.Bodypart.FullDescription().ColourValue()}.",
			actor, actor, target)));
	}

	[PlayerCommand("GiveTattoo", "givetattoo")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("givetattoo", "Gives someone a tattoo. Syntax: givetattoo <target> <tattoo> <bodypart>",
		AutoHelp.HelpArgOrNoArg)]
	protected static void GiveTattoo(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetActorOrCorpse(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What tattoo do you want to give them?");
			return;
		}

		var template = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.DisfigurementTemplates.Get(value)
			: actor.Gameworld.DisfigurementTemplates.GetByName(ss.Last);
		if (template == null || !(template is ITattooTemplate tattooTemplate))
		{
			actor.OutputHandler.Send("There is no such tattoo template.");
			return;
		}

		if (template.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send(
				$"That tattoo is in the {template.Status.DescribeEnum().ColourValue()} status and cannot be used.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which bodypart do you want to put the tattoo on?");
			return;
		}

		var bodypart = target.Body.GetTargetPart(ss.PopSpeech());
		if (bodypart == null || bodypart is IOrganProto || bodypart is IBone)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} does not have any such bodypart.");
			return;
		}

		if (!tattooTemplate.CanBeAppliedToBodypart(target.Body, bodypart))
		{
			actor.OutputHandler.Send("That tattoo cannot be applied to that bodypart.");
			return;
		}

		var tattoo = tattooTemplate.ProduceTattoo(actor, target, bodypart);
		tattoo.CompletionPercentage = 1.0;
		target.Body.AddTattoo(tattoo);
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote(
				$"@ tattoo|tattoos {tattoo.ShortDescription.Colour(Telnet.BoldOrange)} on $1's {bodypart.FullDescription()}.",
				actor, actor, target), flags: OutputFlags.SuppressObscured));
	}

	[PlayerCommand("SetGender", "setgender")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("setgender", "Sets gender of a target character. Syntax: setgender <target> <gender>",
		AutoHelp.HelpArgOrNoArg)]
	protected static void SetGender(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anyone like that to set the gender of.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which gender do you want to set them to?");
			return;
		}

		var gender = Gendering.Get(ss.SafeRemainingArgument);
		if (gender == null)
		{
			actor.Send("There is no such gender.");
			return;
		}

		if (!target.Race.AllowedGenders.Contains(gender.Enum))
		{
			actor.Send("That is not a valid gender for their race.");
			return;
		}

		target.SetGender(gender.Enum);
		var output = new EmoteOutput(new Emote($"@ set|sets $0's gender to {gender.GenderClass()}.", actor, target));
		actor.OutputHandler.Send(output);
		target.OutputHandler.Send(output);
	}

	[PlayerCommand("ALock", "alock")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void ALock(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("help") || ss.Peek().EqualTo("?"))
		{
			actor.Send($"Syntax is {"alock <direction>|<item> [<lock>]".Colour(Telnet.Yellow)}.");
			return;
		}

		var target = ss.Pop();
		var targetExit = actor.Location.ExitsFor(actor).GetFromItemListByKeyword(target, actor);
		var targetItem = targetExit?.Exit.Door?.Parent;
		if (targetItem == null && targetExit != null)
		{
			actor.Send("There is no door in that direction for you to admin lock.");
			return;
		}

		if (targetItem == null)
		{
			targetItem = actor.TargetLocalItem(target);
			if (targetItem == null)
			{
				actor.Send("You don't see anything like that to admin lock.");
				return;
			}
		}

		var lockable = targetItem.GetItemType<ILockable>();
		var lockitem = targetItem.GetItemType<ILock>();
		if (lockable != null && !ss.IsFinished)
		{
			var container = targetItem;
			target = ss.Pop();
			targetItem = lockable.Locks.Select(x => x.Parent).GetFromItemListByKeyword(target, actor);
			if (targetItem == null)
			{
				actor.Send($"{container.HowSeen(actor, true)} does not contain any locks like that.");
				return;
			}

			lockitem = targetItem.GetItemType<ILock>();
		}
		else if (lockable == null && lockitem == null)
		{
			actor.Send($"{targetItem.HowSeen(actor, true)} is not something that can be locked or that has locks.");
			return;
		}

		if (lockitem != null)
		{
			if (lockitem.Parent.EffectsOfType<AdminLock>().Any())
			{
				lockitem.Parent.RemoveAllEffects(x => x.IsEffectType<AdminLock>());
				actor.OutputHandler.Handle(new EmoteOutput(
					new Emote("@ remove|removes the admin lock from $0.", actor, lockitem.Parent),
					flags: OutputFlags.WizOnly));
				return;
			}

			lockitem.Parent.AddEffect(new AdminLock(lockitem.Parent));
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ add|adds an admin lock from $0.", actor, lockitem.Parent), flags: OutputFlags.WizOnly));
			return;
		}

		if (lockable.Parent.EffectsOfType<AdminLock>().Any())
		{
			lockable.Parent.RemoveAllEffects(x => x.IsEffectType<AdminLock>());
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ remove|removes the admin lock from $0.", actor, lockable.Parent),
				flags: OutputFlags.WizOnly));
			return;
		}

		lockable.Parent.AddEffect(new AdminLock(lockable.Parent));
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ add|adds an admin lock from $0.", actor, lockable.Parent), flags: OutputFlags.WizOnly));
	}

	[PlayerCommand("Drugs", "drugs")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Drugs(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("See drugs for whom?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("There is noone like that to view drugs for.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Drugs for {target.HowSeen(actor)}:");
		foreach (var drug in target.Body.ActiveDrugDosages)
		{
			sb.AppendLine(
				$"\t{drug.Drug.Name.Colour(Telnet.Cyan)} (#{drug.Drug.Id}) @ {actor.Gameworld.UnitManager.DescribeExact(drug.Grams * 0.001 / actor.Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).Colour(Telnet.Green)}");
		}

		foreach (var drug in target.Body.LatentDrugDosages)
		{
			sb.AppendLine(
				$"\t{drug.Drug.Name.Colour(Telnet.Cyan)} (#{drug.Drug.Id}) @ {actor.Gameworld.UnitManager.DescribeExact(drug.Grams * 0.001 / actor.Gameworld.UnitManager.BaseWeightToKilograms, UnitType.Mass, actor).Colour(Telnet.Green)} via {drug.OriginalVector.Describe()} (latent)");
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("Sober", "sober")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Sober(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Sober up who?");
			return;
		}

		var target = actor.Target(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("There is nobody like that to sober up.");
			return;
		}

		if (target is not ICharacter targetCharacter)
		{
			if (target is not IGameItem targetItem)
			{
				actor.Send("You can only sober up living characters or corpses.");
				return;
			}

			var targetCorpse = targetItem.GetItemType<ICorpse>();
			if (targetCorpse == null)
			{
				actor.Send("You can only sober up living characters or corpses.");
				return;
			}

			targetCharacter = targetCorpse.OriginalCharacter;
		}

		targetCharacter.Body.Sober();
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ sober|sobers up $0.", actor, targetCharacter)));
	}

	[PlayerCommand("Effect", "effect")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("effect",
		"This command allows you to view effects on individuals or items, as well as remove those effects. You should be VERY careful when manually removing effects; some could have unintended consequences. The syntax is EFFECT LIST <target> [<subtarget>] or EFFECT REMOVE <target> [<subtarget>] <position>",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Effect(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				EffectList(actor, ss);
				break;
			case "remove":
			case "rem":
			case "delete":
			case "del":
				EffectRemove(actor, ss);
				break;
			default:
				actor.Send("What do you want to do with the effect command?");
				return;
		}
	}

	private static void EffectRemove(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Who or what do you want to remove effects from?");
			return;
		}

		var target = actor.Target(ss.Pop());
		if (target == null)
		{
			actor.Send("You do not see that target.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which effect do you want to remove? Use the position number as reported by EFFECT LIST.");
			return;
		}

		if (!int.TryParse(ss.PopSpeech(), out var value))
		{
			if (!(target is ICharacter))
			{
				actor.Send(
					"You can only supply an additional 'next level down' target when your target is a character.");
				return;
			}

			var secondTarget = (target as ICharacter).Inventory.GetFromItemListByKeyword(ss.Last, actor);
			if (secondTarget == null)
			{
				actor.Send($"{target.HowSeen(actor, true)} does not have anything like that in their inventory.");
				return;
			}

			target = secondTarget;

			if (!int.TryParse(ss.PopSpeech(), out value))
			{
				actor.Send("Which effect do you want to remove? Use the position number as reported by EFFECT LIST.");
				return;
			}
		}

		var effects = target.Effects.Concat((target as ICharacter)?.Body.Effects ?? Enumerable.Empty<IEffect>())
		                    .ToList();
		if (value <= 0 || value > effects.Count)
		{
			actor.Send(
				"That is not a valid position number of an effect to remove. Use the position number reported by EFFECT LIST.");
			return;
		}

		var effect = effects.ElementAt(value - 1);
		if (target.Effects.Contains(effect))
		{
			target.RemoveEffect(effect, true);
		}
		else
		{
			((ICharacter)target).Body.RemoveEffect(effect, true);
		}

		actor.Send($"You remove the effect {effect.Describe(actor)} from {target.HowSeen(actor)}.");
	}

	private static void EffectList(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Who or what do you want to list the effects for?");
			return;
		}

		var target = actor.Target(ss.Pop());
		if (target == null)
		{
			actor.Send("You do not see that target to list the effects of.");
			return;
		}

		if (!ss.IsFinished)
		{
			if (!(target is ICharacter))
			{
				actor.Send("You can only list the effects on the inventory of characters.");
				return;
			}

			var secondTarget = (target as ICharacter).Inventory.GetFromItemListByKeyword(ss.Pop(), actor);
			if (secondTarget == null)
			{
				actor.Send($"{target.HowSeen(actor, true)} does not have anything like that in their inventory.");
				return;
			}

			target = secondTarget;
		}

		var sb = new StringBuilder();
		sb.AppendLineFormat("Effects for {0}:", target.HowSeen(actor));
		var i = 1;
		foreach (var effect in target.Effects)
		{
			sb.AppendLine($"\t{i++}) {actor.Gameworld.EffectScheduler.Describe(effect, actor)}");
		}

		foreach (var effect in (target as ICharacter)?.Body.Effects ?? Enumerable.Empty<IEffect>())
		{
			sb.AppendLine($"\t{i++}) [B] {actor.Gameworld.EffectScheduler.Describe(effect, actor)}");
		}

		actor.SendNoNewLine(sb.ToString());
	}

	private static void PostBan(string text, IOutputHandler handler, params object[] objects)
	{
		var accountId = (long)objects.ElementAt(0);
		var authorId = (long)objects.ElementAt(1);
		var gameworld = (Futuremud)objects.ElementAt(2);
		using (new FMDB())
		{
			var dbaccount = FMDB.Context.Accounts.Find(accountId);
			if (dbaccount == null)
			{
				handler.Send("The account no longer exists.");
				return;
			}

			dbaccount.AccessStatus = (int)AccountStatus.Suspended;
			var newNote = new AccountNote
			{
				Text = text,
				AuthorId = authorId,
				Subject = "Account Banned",
				TimeStamp = DateTime.UtcNow,
				AccountId = accountId
			};
			FMDB.Context.AccountNotes.Add(newNote);
			FMDB.Context.SaveChanges();
			var account = gameworld.TryAccount(dbaccount);
			((Account)account).AccountStatus = AccountStatus.Suspended;
			lock (gameworld.Connections)
			{
				var connection =
					gameworld.Connections.FirstOrDefault(
						x => x.ControlPuppet != null && x.ControlPuppet.Account == account);
				connection?.AddOutgoing($"Your account has been banned by {newNote.Author.Name.TitleCase()}.\n");
				connection?.PrepareOutgoing();
				connection?.SendOutgoing();
				connection?.Dispose();
			}

			handler.Send($"You ban account {dbaccount.Name.TitleCase()}.");
			gameworld.SystemMessage(
				$"{newNote.Author.Name.TitleCase()} has banned account {dbaccount.Name.TitleCase()}.", true);
		}
	}

	private static void CancelBan(IOutputHandler handler, params object[] objects)
	{
		handler.Send("You decide not to enact the account ban.");
	}

	[PlayerCommand("Ban", "ban")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	protected static void Ban(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which account do you want to ban?");
			return;
		}

		using (new FMDB())
		{
			var name = ss.Pop();
			var dbaccount =
				FMDB.Context.Accounts.FirstOrDefault(
					x => x.Name == name);
			if (dbaccount == null)
			{
				actor.OutputHandler.Send("There is no such account.");
				return;
			}

			if (dbaccount.Id == actor.Account.Id)
			{
				actor.OutputHandler.Send("You cannot ban yourself.");
				return;
			}

			if (dbaccount.AuthorityGroup.AuthorityLevel >= (int)actor.Account.Authority.Level)
			{
				actor.OutputHandler.Send("You cannot ban accounts with equal or higher permissions than your own.");
				return;
			}

			if ((AccountStatus)dbaccount.AccessStatus == AccountStatus.Suspended)
			{
				actor.OutputHandler.Send("That account has already been banned.");
				return;
			}

			actor.Send("Please enter a reason for your ban on account {0}:", dbaccount.Name.Proper());
			actor.EditorMode(PostBan, CancelBan, 1.0, null, EditorOptions.None,
				new object[] { dbaccount.Id, actor.Account.Id, actor.Gameworld });
		}
	}

	[PlayerCommand("Unban", "unban")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	protected static void Unban(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which account do you want to unban?");
			return;
		}

		using (new FMDB())
		{
			var name = ss.Pop();
			var dbaccount =
				FMDB.Context.Accounts.FirstOrDefault(
					x => x.Name == name);
			if (dbaccount == null)
			{
				actor.OutputHandler.Send("There is no such account.");
				return;
			}

			if ((AccountStatus)dbaccount.AccessStatus != AccountStatus.Suspended)
			{
				actor.OutputHandler.Send("That account has not been banned.");
				return;
			}

			dbaccount.AccessStatus = (int)AccountStatus.Normal;
			FMDB.Context.SaveChanges();
			actor.Gameworld.SystemMessage(
				$"{actor.Account.Name.Proper()} has lifted the ban on account {dbaccount.Name.Proper()}.", true);
			var account = actor.Gameworld.TryAccount(dbaccount);
			((Account)account).AccountStatus = AccountStatus.Normal;
		}
	}

	[PlayerCommand("Siteban", "siteban")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	protected static void Siteban(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.OutputHandler.Send("Which IP Address do you want to ban?");
			return;
		}

		var ipAddress = ss.Pop().Replace('*', '%').Replace('?', '_');
		if (ss.IsFinished)
		{
			character.OutputHandler.Send("When do you want to site ban them until?");
			character.OutputHandler.Send("Note: Use " + "siteban <address> !".Colour(Telnet.Yellow) +
			                             " for a permanent ban.");
			return;
		}

		var banUntil = ss.PopSpeech();
		DateTime? banUntilDate = null;
		if (banUntil != "!")
		{
			if (!DateTime.TryParse(banUntil, character, DateTimeStyles.None, out var result))
			{
				character.OutputHandler.Send("That is not a valid datetime.");
				return;
			}

			banUntilDate = result.Kind == DateTimeKind.Unspecified
				? TimeZoneInfo.ConvertTimeToUtc(result, character.Account.TimeZone)
				: result.ToUniversalTime();
		}

		using (new FMDB())
		{
			var newBan = new Ban
			{
				IpMask = ipAddress,
				Expiry = banUntilDate,
				BannerAccountId = character.Account.Id,
				Reason = ss.SafeRemainingArgument
			};
			FMDB.Context.Bans.Add(newBan);
			FMDB.Context.SaveChanges();
			var results = FMDB.Connection.Query<string>(
				$"select distinct Accounts.Name from LoginIPs inner join Accounts on LoginIPs.AccountId = Accounts.Id where LoginIPs.IpAddress like '{ipAddress}'");
			var message =
				$"{character.Account.Name.Proper()} has banned site {ipAddress} {(banUntilDate.HasValue ? $"for {(banUntilDate.Value - DateTime.UtcNow).Describe()}" : "permanently")}{(results.Any() ? string.Format(", potentially affecting account{1} {0}. ", results.ListToString(), results.Count() == 1 ? "" : "s") : ", affecting no known accounts. ")}{(string.IsNullOrWhiteSpace(newBan.Reason) ? "No reason was given." : $"The reason supplied was: {newBan.Reason.ProperSentences().Fullstop()}")}.";
			character.Gameworld.SystemMessage(message, true);
		}
	}

	[PlayerCommand("Unsiteban", "unsiteban")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	protected static void Unsiteban(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.OutputHandler.Send("Which site ban do you wish to remove?");
			return;
		}

		using (new FMDB())
		{
			var bans = long.TryParse(ss.Pop(), out var banID)
				? FMDB.Context.Bans.Where(x => x.Id == banID)
				: FMDB.Context.Bans.Where(x => x.IpMask == ss.Last);
			bans = bans.Where(x => x.Expiry == null || x.Expiry >= DateTime.UtcNow);
			if (!bans.Any())
			{
				character.OutputHandler.Send("There are no current site bans that meet the specified criteria.");
				return;
			}

			character.Gameworld.SystemMessage(
				$"{character.Account.Name.Proper()} has lifted {bans.Count()} site ban{(bans.Count() == 1 ? "" : "s")} on IP Address {bans.Select(x => x.IpMask).First()}",
				true);
			character.Gameworld.SaveManager.Flush();
			foreach (var ban in bans.ToList())
			{
				FMDB.Context.Bans.Remove(ban);
			}

			FMDB.Context.SaveChanges();
		}
	}

	[PlayerCommand("Sitebans", "sitebans")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Sitebans(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var time = DateTime.UtcNow;
		using (new FMDB())
		{
			var bans = FMDB.Context.Bans.Where(x => !x.Expiry.HasValue || x.Expiry.Value > time).ToList();
			character.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from ban in bans
					orderby ban.Expiry ?? DateTime.MinValue
					select
						new[]
						{
							ban.Id.ToString(), ban.IpMask, ban.BannerAccount.Name.TitleCase(), ban.Reason,
							ban.Expiry.HasValue ? ban.Expiry.Value.GetLocalDateString(character) : "Permanent"
						},
					new[] { "ID", "IP Address", "Banner", "Reason", "Expiry" },
					character.Account.LineFormatLength,
					colour: Telnet.Green, unicodeTable: character.Account.UseUnicode
				)
			);
			// TODO filters
		}
	}

	[PlayerCommand("RandomNames", "randomnames")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void RandomNames(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must supply a name culture and random name profile to use. Valid name cultures are {actor.Gameworld.NameCultures.Select(x => x.Name.ColourName()).ListToString()}.");
			return;
		}

		var culture = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.NameCultures.Get(value)
			: actor.Gameworld.NameCultures.FirstOrDefault(
				x => x.Name.StartsWith(ss.Last, StringComparison.InvariantCultureIgnoreCase));

		if (culture == null)
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which random name profile do you want to use? Valid choices for the supplied name culture are {culture.RandomNameProfiles.Select(x => x.Name.ColourName()).ListToString()}.");
			return;
		}

		var profile = long.TryParse(ss.PopSpeech(), out value)
			? culture.RandomNameProfiles.FirstOrDefault(x => x.Id == value)
			: culture.RandomNameProfiles.FirstOrDefault(
				x => x.Name.StartsWith(ss.Last, StringComparison.InvariantCultureIgnoreCase));

		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such random name profile for you to use.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"You generate 5 random names from the {culture.Name.Proper()} culture with the {profile.Name.Proper()} profile.");

		for (var i = 0; i < 5; i++)
		{
			var random = profile.GetRandomPersonalName(true);
			if (random.GetName(NameStyle.FullName).EqualTo(random.GetName(NameStyle.FullWithNickname)))
			{
				sb.AppendLine($"\t{i + 1}: {random.GetName(NameStyle.FullName).Colour(Telnet.Cyan)}");
			}
			else
			{
				sb.AppendLine(
					$"\t{i + 1}: {random.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} AKA {random.GetName(NameStyle.FullWithNickname).Colour(Telnet.Cyan)}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("ResetPassword", "resetpassword")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	protected static void ResetPassword(ICharacter actor, string input)
	{
		var account = input.RemoveFirstWord();
		if (string.IsNullOrEmpty(input))
		{
			actor.OutputHandler.Send("For which account do you want to reset the password?");
			return;
		}

		string newPassword;
		using (new FMDB())
		{
			var dbaccount =
				FMDB.Context.Accounts.FirstOrDefault(
					x => x.Name == account);
			if (dbaccount == null)
			{
				actor.OutputHandler.Send("There is no such account for which you can reset the password.");
				return;
			}

			newPassword = SecurityUtilities.GetRandomString(12, Constants.RandomPasswordCharacters.ToCharArray());

#if DEBUG
			Console.WriteLine("Generated new password {0}", newPassword);
#endif
			dbaccount.Salt = SecurityUtilities.GetSalt64();
			dbaccount.Password = SecurityUtilities.GetPasswordHash(newPassword, dbaccount.Salt);
			EmailHelper.Instance.SendEmail(EmailTemplateTypes.AccountPasswordReset, dbaccount.Email,
				dbaccount.Name.Proper(), actor.Account.Name.Proper(), newPassword);
			FMDB.Context.SaveChanges();
		}

		actor.OutputHandler.Send(
			$"You have reset the password of account {account.Proper()}, and an email has been sent to them with their new, randomly generated password.");
		if (actor.IsAdministrator(PermissionLevel.Founder))
		{
			actor.OutputHandler.Send($"The new password is {newPassword.ColourValue()}.");
		}
	}

	[PlayerCommand("ToggleAdminAvatar", "toggleadminavatar")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("toggleadminavatar",
		@"This command is used to toggle the Admin Avatar flag on a character in the same location as you. You need to first have set their account authority to an admin level with the SETAUTHORITY command.

The syntax for this command is: 

	#3toggleadminavatar <character>#0",
		AutoHelp.HelpArgOrNoArg)]
	protected static void ToggleAdminAvatar(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send(StringUtilities.HMark + "You do not see any such person to turn into an admin avatar.");
			return;
		}

		if (target.Account == DummyAccount.Instance)
		{
			actor.Send(StringUtilities.HMark + "You cannot make NPCs into admin avatars.");
			return;
		}

		if (target.PermissionLevel >= PermissionLevel.JuniorAdmin)
		{
			//Remove admin avatar status. Don't bother checking if they're already an
			//admin avatar as a failsafe in case something ends up out of synch between
			//their account authority and character permission level.
			using (new FMDB())
			{
				var dbcharacter = FMDB.Context.Characters.Find(target.Id);
				dbcharacter.IsAdminAvatar = false;
				FMDB.Context.SaveChanges();
			}

			if (target.Authority.Level >= PermissionLevel.Guide)
			{
				target.ChangePermissionLevel(PermissionLevel.Guide);
				actor.Gameworld.SystemMessage(new EmoteOutput(new Emote(
					$"You|{actor.Account.Name.Proper()} have|has removed Admin Avatar status from " +
					$"{target.PersonalName.GetName(NameStyle.SimpleFull).Colour(Telnet.Green)} " +
					$"({target.Account.Name.Colour(Telnet.Green)}) but they still have {"Guide".ColourBold(Telnet.Green)} access.",
					actor)), true);
			}
			else
			{
				target.ChangePermissionLevel(PermissionLevel.Player);
				actor.Gameworld.SystemMessage(new EmoteOutput(new Emote(
					$"You|{actor.Account.Name.Proper()} have|has removed Admin Avatar status from " +
					$"{target.PersonalName.GetName(NameStyle.SimpleFull).Colour(Telnet.Green)} " +
					$"({target.Account.Name.Colour(Telnet.Green)}).",
					actor)), true);
			}

			target.RemoveAllEffects(x => x.IsEffectType<IAdminEffect>(), true);
		}
		else
		{
			//Grant admin avatar status
			if (target.Account.Authority.Level < PermissionLevel.JuniorAdmin)
			{
				actor.Send(StringUtilities.HMark + $"{target.HowSeen(actor, true)}({target.Account.Name}) " +
				           $"does not have a high enough Account Authority Group to have an Admin Avatar.\n" +
				           StringUtilities.Indent + "Use 'setauthority' to promote the account.");
				return;
			}

			using (new FMDB())
			{
				var dbcharacter = FMDB.Context.Characters.Find(target.Id);
				dbcharacter.IsAdminAvatar = true;
				FMDB.Context.SaveChanges();
			}

			target.ChangePermissionLevel(target.Account.Authority.Level);

			actor.Gameworld.SystemMessage(new EmoteOutput(new Emote(
				$"You|{actor.Account.Name.Proper()} have|has granted Admin Avatar status to " +
				$"{target.PersonalName.GetName(NameStyle.SimpleFull).Colour(Telnet.Green)} " +
				$"({target.Account.Name.Colour(Telnet.Green)}) " +
				$"with a permission level of: {target.Authority.Level.Describe().ColourBold(Telnet.Green)}.",
				actor)), true);
		}
	}

	[PlayerCommand("SetAuthority", "setauthority")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	[HelpInfo("setauthority",
		@"This command is used to promote or demote an account to a specific level of authority. This can turn a player account into a guide or an admin account, demote an admin or guide back down to a player, or change the level of authority that an admin holds.

#BNote: see the TOGGLEADMINAVATAR command for how to make a specific character into an admin avatar.#0

The syntax for this command is: 
	
	#3setauthority <account> <level>#0",
		AutoHelp.HelpArgOrNoArg)]
	protected static void SetAuthority(ICharacter actor, string input)
	{
		using (new FMDB())
		{
			var ss = new StringStack(input.RemoveFirstWord());
			var accountName = ss.PopSpeech();
			var target =
				FMDB.Context.Accounts.FirstOrDefault(x => x.Name == accountName);
			if (target == null)
			{
				actor.OutputHandler.Send(
					$"There is no account with the name {accountName.ColourName()} to give a new authority level.");
				return;
			}

			if (target.Id == actor.Account.Id)
			{
				actor.Send("You cannot change your own authority level.");
				return;
			}

			var authorityName = ss.SafeRemainingArgument;
			var authority = long.TryParse(authorityName, out var value)
				? actor.Gameworld.Authorities.Get(value)
				: actor.Gameworld.Authorities.FirstOrDefault(
					x => x.Name.Equals(authorityName, StringComparison.InvariantCultureIgnoreCase));

			if (authority is null || authority.Level == PermissionLevel.NPC || authority.Level == PermissionLevel.Guest)
			{
				var levels = actor.Gameworld.Authorities.OrderBy(x => x.Level)
				                  .Where(y => y.Level != PermissionLevel.NPC && y.Level != PermissionLevel.Guest)
				                  .Select(z => z.Name.ColourValue())
				                  .ToList();
				actor.OutputHandler.Send(
					$"That is not a valid authority level to give them.\nThe valid options are:\n\n{levels.ListToLines(true)}");
				return;
			}

			if ((int)authority.Level == target.AuthorityGroup.AuthorityLevel)
			{
				actor.OutputHandler.Send(
					$"The account {target.Name.ColourName()} already has that level of authority.");
				return;
			}

			if (authority.Level > actor.Authority.Level)
			{
				actor.OutputHandler.Send("You cannot promote somebody to a higher level of authority than you hold.");
				return;
			}

			var promotion = (int)authority.Level > target.AuthorityGroup.AuthorityLevel;
			target.AuthorityGroupId = authority.Id;
			FMDB.Context.SaveChanges();
			var fmAccount = actor.Gameworld.Accounts.Get(target.Id);
			fmAccount?.SetAccountAuthority(authority);

			//Demote the permission level of any characters with too high a level.
			//Remove admin effects if the new Authority Group is below Junior Admin
			//Grant all connected characters the Guide permissions if they are not already
			//higher and the new rank is at least Guide
			foreach (var character in actor.Gameworld.Characters.Where(x => x.Account == fmAccount))
			{
				if (character.PermissionLevel > authority.Level)
				{
					character.ChangePermissionLevel(authority.Level);
				}

				if (authority.Level < PermissionLevel.JuniorAdmin)
				{
					character.RemoveAllEffects(x => x.IsEffectType<IAdminEffect>(), true);
				}

				if (authority.Level >= PermissionLevel.Guide && character.PermissionLevel < PermissionLevel.Guide)
				{
					character.ChangePermissionLevel(PermissionLevel.Guide);
				}
			}

			actor.Gameworld.SystemMessage(
				new EmoteOutput(
					new Emote(
						string.Format("You|{0} have|has {3} {1} to the {2} authority level.",
							actor.Account.Name.Proper(), target.Name.Proper(),
							authority.Name.Proper().ColourBold(Telnet.Green), promotion ? "promoted" : "demoted"),
						actor),
					flags: OutputFlags.WizOnly));
			if (promotion && authority.Level > PermissionLevel.Guide)
			{
				actor.OutputHandler.Send(
					$"You may need to flag one of the characters belonging to {target.Name.Proper().Colour(Telnet.Green)} as an Admin Avatar via 'ToggleAdminAvatar'.");
			}
		}
	}

	[PlayerCommand("SetCharacters", "setcharacters")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	protected static void SetCharacters(ICharacter actor, string input)
	{
		using (new FMDB())
		{
			var ss = new StringStack(input.RemoveFirstWord());
			if (string.IsNullOrEmpty(ss.Pop()))
			{
				actor.OutputHandler.Send(
					"You must enter an account for which to set the number of allowed characters.");
				return;
			}

			var target =
				FMDB.Context.Accounts.FirstOrDefault(x => x.Name == ss.Last);
			if (target == null)
			{
				actor.OutputHandler.Send("There is no such account to give a number of allowed characters.");
				return;
			}

			if (!uint.TryParse(ss.Pop(), out var value))
			{
				actor.OutputHandler.Send("You must enter a number of allowed active characters for this account.");
				return;
			}

			target.ActiveCharactersAllowed = (int)value;
			FMDB.Context.SaveChanges();
			var fmAccount = actor.Gameworld.Accounts.Get(target.Id);
			if (fmAccount != null)
			{
				fmAccount.ActiveCharactersAllowed = (int)value;
			}

			actor.OutputHandler.Send($"You allow {target.Name.Proper()} to have up to {value} characters at once.");
		}
	}

	[PlayerCommand("Shutdown", "shutdown")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("shutdown",
		"This command is used to shut down the game. There are two versions: 'shutdown reboot' and 'shutdown stop'. 'shutdown' with no arguments is equivalent to the reboot version.\n\nShutdown reboot means that the engine will immediately reboot. This is often used when an update needs to happen for example.\n\nShutdown stop means that the engine will not attempt to reboot. You might use this version if you need to restart your server or you have some other reason to take the game down and not have it immediately come back up.",
		AutoHelp.HelpArg)]
	protected static void Shutdown(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var arg = ss.Pop();
		if (arg.Length == 0 || arg.EqualTo("reboot"))
		{
			Console.WriteLine($"{actor.Name.Proper()} excecuted a shutdown [reboot] command.");
			actor.OutputHandler.Send(
				$"You shutdown {actor.Gameworld.Name.Proper().Colour(Telnet.Cyan)} with the reboot argument.");
			actor.OutputHandler.Handle(
				new RawOutput($"{actor.Account.Name.Proper()} has executed a shutdown [reboot] command.",
					flags: OutputFlags.WizOnly | OutputFlags.SuppressSource), OutputRange.All);
			actor.Gameworld.SystemMessage(string.Format(actor.Gameworld.GetStaticString("GameShutdownMessageReboot"),
				actor.Gameworld.Name.Proper().ColourName()));
			actor.Gameworld.Characters.ForEach(x => x.EffectsChanged = true);
			actor.Gameworld.SaveManager.Flush();
			actor.Gameworld.DiscordConnection.NotifyShutdown(actor.Account.Name);
			using (new FMDB())
			{
				var time = DateTime.UtcNow;
				foreach (var dbchar in actor.Gameworld.Characters.SelectNotNull(ch =>
					         FMDB.Context.Characters.Find(ch.Id)))
				{
					dbchar.LastLogoutTime = time;
				}

				FMDB.Context.SaveChanges();
			}

			actor.Gameworld.HaltGameLoop();
		}
		else if (arg.EqualTo("stop"))
		{
			Console.WriteLine($"{actor.Name.Proper()} excecuted a shutdown command.");
			actor.OutputHandler.Send(
				$"You shutdown {actor.Gameworld.Name.Proper().Colour(Telnet.Cyan)} with the stop argument.");
			actor.OutputHandler.Handle(
				new RawOutput($"{actor.Account.Name.Proper()} has executed a shutdown [stop] command.",
					flags: OutputFlags.WizOnly | OutputFlags.SuppressSource), OutputRange.All);
			actor.Gameworld.SystemMessage(string.Format(actor.Gameworld.GetStaticString("GameShutdownMessage"),
				actor.Gameworld.Name.Proper().ColourName()));
			actor.Gameworld.Characters.ForEach(x => x.EffectsChanged = true);
			actor.Gameworld.SaveManager.Flush();
			actor.Gameworld.DiscordConnection.NotifyShutdown(actor.Account.Name);
			using (var fs = File.Create("STOP-REBOOTING"))
			{
			}

			using (new FMDB())
			{
				var time = DateTime.UtcNow;
				foreach (var dbchar in actor.Gameworld.Characters.SelectNotNull(ch =>
					         FMDB.Context.Characters.Find(ch.Id)))
				{
					dbchar.LastLogoutTime = time;
				}

				FMDB.Context.SaveChanges();
			}

			actor.Gameworld.HaltGameLoop();
		}
		else
		{
			actor.OutputHandler.Send(
				$"You must either use {"shutdown stop".Colour(Telnet.Cyan)} or {"shutdown reboot".Colour(Telnet.Cyan)}.");
		}
	}

	[PlayerCommand("Where", "where")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Where(ICharacter actor, string command)
	{
		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from character in actor.Gameworld.Characters.Where(x => !x.State.HasFlag(CharacterState.Dead))
				                       .OrderBy(x => x.Location.Id).ToList()
				select new[]
				{
					character.Id.ToString("N0", actor),
					character.PersonalName.GetName(NameStyle.FullWithNickname).TitleCase(),
					character.Location.Id.ToString("N0", actor),
					character.Location.HowSeen(character, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee),
					character.Account.Name.TitleCase()
				},
				new[] { "ID", "Name", "Room", "Room Desc", "Account" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 3,
				unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	[PlayerCommand("Users", "users")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	protected static void Users(ICharacter actor, string command)
	{
		try
		{
			lock (actor.Gameworld.Connections)
			{
				actor.OutputHandler.Send(
					StringUtilities.GetTextTable(
						from connection in actor.Gameworld.Connections
						select new[]
						{
							connection.IP,
							connection.State.ToString(),
							connection.ControlPuppet?.Account?.Name.Proper() ?? "None",
							connection.ControlPuppet?.Actor?.HowSeen(actor, colour: false,
								flags: PerceiveIgnoreFlags.IgnoreSelf) ??
							"None",
							$"{(connection.InactivityMilliseconds / 1000.0).ToString("N1", actor)}s"
						},
						new[] { "IP", "State", "Account", "Character", "Inactivity" },
						actor.Account.LineFormatLength,
						colour: Telnet.Green,
						truncatableColumnIndex: 3,
						unicodeTable: actor.Account.UseUnicode
					)
				);
			}
		}
		catch (SocketException)
		{
			actor.Send("One of the connections is causing a socket exception.");
		}
	}

	[PlayerCommand("Broadcast", "broadcast")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Broadcast(ICharacter actor, string command)
	{
		var output = command.RemoveFirstWord();
		if (string.IsNullOrWhiteSpace(output))
		{
			actor.OutputHandler.Send("What is it that you want to broadcast to the entire MUD?");
			return;
		}

		actor.Gameworld.SystemMessage(output.ProperSentences().Fullstop());
		actor.OutputHandler.Send("You broadcast the following message to all players:\n\t" +
		                         output.ProperSentences().Fullstop());
		actor.Gameworld.DiscordConnection.HandleBroadcast(output.ProperSentences().Fullstop());
	}

	[PlayerCommand("Wizlock", "wizlock")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	protected static void Wizlock(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (!ss.IsFinished && ss.Peek().Equals("status", StringComparison.InvariantCultureIgnoreCase))
		{
			var flags = new List<string>();
			if (actor.Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoLogin))
			{
				flags.Add("Non-admins may not log in to characters");
			}

			if (actor.Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoAccountLogin))
			{
				flags.Add("Non-admins may not log in to accounts at all");
			}

			if (actor.Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoChargen))
			{
				flags.Add("Non-admins may not create characters");
			}

			actor.Send("The MUD {0} in maintenance mode{1}.",
				actor.Gameworld.MaintenanceMode != MaintenanceModeSetting.None ? "is" : "is not",
				flags.Any()
					? ":\n" +
					  flags.ListToString(separator: "\n", article: "\t", twoItemJoiner: "\n", conjunction: "")
					: ""
			);
			return;
		}

		if (ss.IsFinished)
		{
			if (actor.Gameworld.MaintenanceMode != MaintenanceModeSetting.None)
			{
				actor.Gameworld.MaintenanceMode = MaintenanceModeSetting.None;
				actor.Gameworld.SystemMessage(
					"All maintenance mode restrictions have been lifted - anyone may login to the game and create characters.");
			}
			else
			{
				actor.Gameworld.MaintenanceMode = MaintenanceModeSetting.NoLogin |
				                                  MaintenanceModeSetting.NoChargen;
				actor.Gameworld.SystemMessage(
					"The MUD has been put into maintenance mode. This means that only administrators may login to the game or create characters.");
			}

			return;
		}

		switch (ss.Pop().ToLowerInvariant())
		{
			case "chargen":
				if (actor.Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoChargen))
				{
					actor.Gameworld.MaintenanceMode ^= MaintenanceModeSetting.NoChargen;
					actor.Gameworld.SystemMessage(
						"Chargen maintenance mode restrictions have been lifted - anyone may create characters.");
				}
				else
				{
					actor.Gameworld.MaintenanceMode |= MaintenanceModeSetting.NoChargen;
					actor.Gameworld.SystemMessage(
						"Chargen has been put into maintenance mode. This means that only administrators may create characters.");
				}

				return;
			case "login":
				if (actor.Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoLogin))
				{
					actor.Gameworld.MaintenanceMode ^= MaintenanceModeSetting.NoLogin;
					actor.Gameworld.SystemMessage(
						"Login maintenance mode restrictions have been lifted - anyone may login to the game.");
				}
				else
				{
					actor.Gameworld.MaintenanceMode |= MaintenanceModeSetting.NoLogin;
					actor.Gameworld.SystemMessage(
						"Login has been put into maintenance mode. This means that only administrators may login to the game.");
				}

				return;
			default:
				actor.Send(
					"That is not a valid option to choose with wizlock. You can either toggle chargen or login.");
				return;
		}
	}

	[PlayerCommand("Purge", "purge")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Purge(ICharacter actor, string command)
	{
		var ss = new StringStack(command);
		if (!ss.Pop().EqualTo("purge"))
		{
			actor.OutputHandler.Send("You must type out the entire PURGE command for it to work.");
			return;
		}

		var items = new List<IGameItem>();
		Emote emote;

		if (ss.IsFinished)
		{
			items = actor.Location.LayerGameItems(actor.RoomLayer).ToList();
			emote = new Emote("@ purge|purges the location of all items.", actor);
		}
		else
		{
			if (ss.Peek().Equals("all", StringComparison.InvariantCultureIgnoreCase))
			{
				ss.Pop();
				if (ss.IsFinished)
				{
					actor.Send("Purge the location of all items with which keyword?");
					return;
				}

				var keyword = ss.PopSpeech();
				items = actor.Location.LayerGameItems(actor.RoomLayer).Where(x => x.HasKeyword(keyword, actor, true))
				             .ToList();
				emote = new Emote(
					$"@ purge|purges the location of all items with the {keyword.ColourValue()} keyword.",
					actor);
			}
			else
			{
				var target = actor.TargetLocalItem(ss.Pop());
				if (target == null)
				{
					actor.Send("There is no such item for you to purge.");
					return;
				}

				items = new List<IGameItem> { target };
				emote = new Emote("@ purge|purges $0 from the location.", actor, target);
			}
		}

		if (!items.Any())
		{
			actor.OutputHandler.Send("There are no items here to purge.");
			return;
		}

		if (!items.Any(x => x.WarnBeforePurge))
		{
			actor.OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.SuppressObscured));
			foreach (var item in items)
			{
				item.Delete();
			}

			return;
		}

		actor.OutputHandler.Send(
			"There are items that would be purged which are throwing a warning (e.g. containers, writing, etc). If you still want to purge, you can ACCEPT. Otherwise DECLINE.");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = perc =>
			{
				actor.OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.SuppressObscured));
				foreach (var item in items)
				{
					item.Delete();
				}
			},
			RejectAction = perc => { actor.OutputHandler.Send("You decide not to purge items."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to purge items."); },
			Keywords = new List<string> { "purge", "items" },
			DescriptionString = "Purge items from the location"
		}), TimeSpan.FromSeconds(120));
	}

	[PlayerCommand("Debug", "debug")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	protected static void Debug(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send(
				$"Debug what? Options are {new[] { "dead", "save", "char", "dream", "mode" }.Select(x => x.Colour(Telnet.Yellow)).ListToString()}.");
			return;
		}

		switch (ss.Pop())
		{
			case "dead":
				DebugDead(actor);
				return;
			case "save":
				DebugSaveQueue(actor);
				return;
			case "char":
			case "character":
				DebugCharacter(actor, ss);
				return;
			case "dream":
				DebugDream(actor, ss);
				return;
			case "mode":
				if (actor.AffectedBy<DebugMode>())
				{
					actor.RemoveAllEffects<DebugMode>();
					actor.OutputHandler.Send("You are no longer receiving debug messages.");
					return;
				}

				actor.AddEffect(new DebugMode(actor));
				actor.OutputHandler.Send("You are now receiving debug messages.");
				return;
			default:
				actor.Send("That's not a known debug routine.");
				return;
		}
	}

	private static void DebugDead(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine("The following characters had weird death states:");
		foreach (var cell in actor.Gameworld.Cells)
		foreach (var ch in cell.Characters.Where(x => x.State == CharacterState.Dead))
		{
			sb.AppendLine(
				$"Cell {cell.Id:N0} ({cell.CurrentOverlay.CellName}) had dead character {ch.Id} ({ch.HowSeen(actor)})");
		}

		foreach (var ch in actor.Gameworld.Characters)
		{
			if (ch.State != CharacterState.Dead && ch.Status == CharacterStatus.Deceased)
			{
				sb.AppendLine($"Character {ch.Id} ({ch.HowSeen(actor)}) was deceased but {ch.State.Describe()}");
			}

			if (ch.State != CharacterState.Dead && ch.State.HasFlag(CharacterState.Dead))
			{
				sb.AppendLine($"Character {ch.Id} ({ch.HowSeen(actor)}) had combo state {ch.State.Describe()}");
			}
		}

		actor.Send(sb.ToString());
	}

	private static void DebugCharacter(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Debug which character?");
			return;
		}

		var ch = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Actors.Get(value)
			: actor.TargetActor(ss.Last);
		if (ch == null)
		{
			actor.Send("There is noone like that to debug.");
			return;
		}

		actor.Send(ch.DebugInfo());
	}

	private static void DebugDream(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Who do you want to give a dream to?");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You do not see anyone like that to give a dream to.");
			return;
		}

		if (!target.State.HasFlag(CharacterState.Sleeping))
		{
			actor.Send("{0} is not asleep.", target.HowSeen(actor, true));
			return;
		}

		if (target.EffectsOfType<IDreamingEffect>().Any())
		{
			actor.Send("{0} is already dreaming.", target.HowSeen(actor, true));
			return;
		}

		target.RemoveAllEffects(x => x.IsEffectType<INoDreamEffect>());
		var dream =
			actor.Gameworld.Dreams.Where(x => x.CanDream(target))
			     .GetWeightedRandom(x => x.Priority);
		if (dream == null)
		{
			actor.Send("No valid dreams for {0}", target.HowSeen(actor));
			return;
		}

		target.AddEffect(new Dreaming(target, dream));
	}

	private static void DebugSaveQueue(ICharacter actor)
	{
		actor.Send(actor.Gameworld.SaveManager.DebugInfo(actor.Gameworld));
	}

	[PlayerCommand("RegisterAccount", "registeraccount")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	protected static void RegisterAccount(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Which account do you want to register?");
			return;
		}

		var name = ss.Pop();

		using (new FMDB())
		{
			var account =
				FMDB.Context.Accounts.FirstOrDefault(x => x.Name == name);
			if (account == null)
			{
				actor.Send("There is no such account.");
				return;
			}

			if (account.IsRegistered)
			{
				actor.Send("That account is already registered.");
				return;
			}

			var gaccount = actor.Gameworld.TryAccount(account);
			actor.OutputHandler.Send(gaccount.TryAccountRegistration(account.RegistrationCode)
				? $"You register the account {account.Name}."
				: "There was some sort of problem registering that account...");
		}
	}

	[PlayerCommand("SetLayer", "setlayer", "sl")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("setlayer",
		"This command allows you to set the layer of a target to whatever you specify. Syntax is SETLAYER <target> <layer>",
		AutoHelp.HelpArgOrNoArg)]
	protected static void SetLayer(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (actor.TargetLocal(ss.PopSpeech()) is not IPerceiver target)
		{
			actor.OutputHandler.Send("There is no such target.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What layer do you want to set for {target.HowSeen(actor)}? Valid choices are {target.Location.Terrain(target).TerrainLayers.Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return;
		}

		if (!Enum.TryParse<RoomLayer>(ss.PopSpeech(), true, out var layer))
		{
			actor.OutputHandler.Send(
				$"That is not a valid layer. Valid choices are {target.Location.Terrain(target).TerrainLayers.Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ set|sets the layer for $0 to {layer.DescribeEnum().ColourValue()}.", actor, target)));
		target.RoomLayer = layer;
	}

	[PlayerCommand("Plans", "plans")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Plans(ICharacter actor, string command)
	{
		var plans = actor.Gameworld.Characters
		                 .Where(x => x.AffectedBy<RecentlyUpdatedPlan>())
		                 .Select(x => (Character: x,
			                 Length: TimeSpan.FromDays(14) -
			                         x.ScheduledDuration(x.EffectsOfType<RecentlyUpdatedPlan>().First())))
		                 .OrderBy(x => x.Length)
		                 .ToList();
		if (!plans.Any())
		{
			actor.OutputHandler.Send("Nobody currently in the game has updated their plans within the last 14 days.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("The following recent updates to character plans have been made:");
		foreach (var plan in plans)
		{
			sb.AppendLine(
				$"\n{plan.Character.PersonalName.GetName(NameStyle.FullName)} ({plan.Character.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)})");
			sb.AppendLine($"Short: {plan.Character.ShortTermPlan?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}");
			sb.AppendLine($"Long: {plan.Character.LongTermPlan?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static string GridHelpText =>
		"You can use the following subcommands with the grid command:\n\taudit - shows all grids\n\tstatus <grid#> - shows a particular grid\n\texpand <grid#> <direction> - expands a grid in a direction\n\twithdraw <grid#> removes the current location from the specified grid\n\tconnect <thing> <grid#> - connects a grid-interfacing item to a grid";

	[PlayerCommand("Grid", "grid")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Grid(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "connect":
				GridConnect(actor, ss);
				return;
			case "audit":
				GridAudit(actor, ss);
				return;
			case "status":
				GridStatus(actor, ss);
				return;
			case "expand":
				GridExpand(actor, ss);
				return;
			case "withdraw":
				GridWithdraw(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(GridHelpText);
				return;
		}
	}

	private static void GridConnect(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to connect to the grid?");
			return;
		}

		var target = actor.TargetLocalItem(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anything like that here.");
			return;
		}

		var connect = target.GetItemType<ICanConnectToGrid>();
		if (connect == null)
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true)} is not the kind of thing that can be connected to a grid.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which grid do you want to connect to?");
			return;
		}

		if (!long.TryParse(ss.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid grid ID.");
			return;
		}

		var grid = actor.Gameworld.Grids.Get(value);
		if (grid == null)
		{
			actor.OutputHandler.Send("There is no such grid.");
			return;
		}

		if (!grid.GridType.EqualTo(connect.GridType))
		{
			actor.OutputHandler.Send(
				$"Grid #{grid.Id.ToString("N0", actor)} is not the right sort of grid for {target.HowSeen(actor)}.");
			return;
		}

		if (!grid.Locations.Contains(actor.Location))
		{
			actor.OutputHandler.Send("That grid is not present in your current location.");
			return;
		}

		connect.Grid = grid;
		actor.OutputHandler.Send($"You connect {target.HowSeen(actor)} to grid #{grid.Id.ToString("N0", actor)}.");
	}

	private static void GridWithdraw(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which grid did you want to withdraw from this location?");
			return;
		}

		if (!long.TryParse(ss.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid grid ID.");
			return;
		}

		var grid = actor.Gameworld.Grids.Get(value);
		if (grid == null)
		{
			actor.OutputHandler.Send("There is no such grid.");
			return;
		}

		if (!grid.Locations.Contains(actor.Location))
		{
			actor.OutputHandler.Send("That grid is not present in your current location.");
			return;
		}

		grid.WithdrawFrom(actor.Location);
		actor.OutputHandler.Send($"You withdraw your current location from grid #{grid.Id.ToString("N0", actor)}.");
	}

	private static void GridExpand(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which grid do you want to expand?");
			return;
		}

		if (!long.TryParse(ss.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid grid ID.");
			return;
		}

		var grid = actor.Gameworld.Grids.Get(value);
		if (grid == null)
		{
			actor.OutputHandler.Send("There is no such grid.");
			return;
		}

		if (!grid.Locations.Contains(actor.Location))
		{
			actor.OutputHandler.Send("That grid is not present in your current location.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which direction do you want to extend that grid in?");
			return;
		}

		var exit = actor.Location.GetExitKeyword(ss.PopSpeech(), actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit.");
			return;
		}

		if (grid.Locations.Contains(exit.Destination))
		{
			actor.OutputHandler.Send($"{exit.Destination.HowSeen(actor)} is already a part of that grid.");
			return;
		}

		grid.ExtendTo(exit.Destination);
		actor.OutputHandler.Send(
			$"You extend grid #{grid.Id.ToString("N0", actor)} to {exit.OutboundDirectionDescription}.");
	}

	private static void GridStatus(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which grid do you want to show the status of?");
			return;
		}

		if (!long.TryParse(ss.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid grid ID.");
			return;
		}

		var grid = actor.Gameworld.Grids.Get(value);
		if (grid == null)
		{
			actor.OutputHandler.Send("There is no such grid.");
			return;
		}

		actor.OutputHandler.Send(grid.Show(actor));
	}

	private static void GridAudit(ICharacter actor, StringStack ss)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"There are {actor.Gameworld.Grids.Count().ToString("N0", actor).ColourValue()} {(actor.Gameworld.Grids.Count() == 1 ? "grid" : "grids")} in the game world.");
		var here = actor.Gameworld.Grids.Where(x => x.Locations.Contains(actor.Location)).ToList();
		if (!here.Any())
		{
			sb.AppendLine("There are no grids where you are.");
		}
		else
		{
			foreach (var grid in here)
			{
				sb.AppendLine($"Grid #{grid.Id.ToString("N0", actor)} ({grid.GridType}) is here.");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Path", "path")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("path",
		@"This command allows you to calculate a path between yourself and a character (PC or NPC). The command will look for paths up to 50 rooms away, and will return a list of exits that you could follow to get there.

The syntax for this command is #3path <target>#0. Note, you can use names or keywords to do this search, so the following three syntaxes would all be valid:

#3path amos#0
#3path tall.strapping.lad#0
#3path ""Amos Newbie""#0", AutoHelp.HelpArgOrNoArg)]
	protected static void Path(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var targetText = ss.SafeRemainingArgument;
		var target = actor.Gameworld.Actors.GetFromItemListByKeywordIncludingNames(targetText, actor);
		if (target == null)
		{
			actor.OutputHandler.Send("There is no such character for you to path to.");
			return;
		}

		var exits1 = actor.ExitsBetween(target, 50).ToList();
		if (!exits1.Any())
		{
			actor.OutputHandler.Send("Could not find a path to that target within 50 rooms.");
			return;
		}

		var exits2 = actor.PathBetween(target, 50, true);

		var directionStrings1 = exits1.Select(x =>
		{
			if (x.OutboundDirection != CardinalDirection.Unknown)
			{
				return x.OutboundDirection.DescribeBrief();
			}

			return x is NonCardinalCellExit nc ? $"{nc.Verb} {nc.PrimaryKeyword}".ToLowerInvariant() : "??";
		}).ToList();

		var directionStrings2 = exits2.Select(x =>
		{
			if (x.OutboundDirection != CardinalDirection.Unknown)
			{
				return x.OutboundDirection.DescribeBrief();
			}

			return x is NonCardinalCellExit nc ? $"({nc.Verb} {nc.PrimaryKeyword})".ToLowerInvariant() : "??";
		}).ToList();

		var sb = new StringBuilder();
		sb.AppendLine($"Path to {target.HowSeen(actor)}:");
		sb.AppendLine($"Ignore Doors: {directionStrings1.ListToString(separator: " ", conjunction: "")}");
		sb.AppendLine($"Include Openable Doors: {directionStrings2.ListToString(separator: " ", conjunction: "")}");
		actor.Send(sb.ToString());
	}

	[PlayerCommand("CurrencyConvert", "currencyconvert", "cc")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("currencyconvert",
		@"This command is used to convert text representing an amount of currency (a #6currency text#0) into a #5base value#0, which is a different way of representing currency amounts often used in progs. It can also do the conversion in the other direction. 

As an example, if your currency was dollars you might have a #5base value#0 representing cents. Therefore a base value of #272#0 might represent #272c#0 whereas #2650#0 might represent #2$6.50#0. While the example of dollars is fairly trivial, obviously in more complicated non-decimal currencies it can be difficult to guess the conversion between the two, which is what this command is designed to help with.

The syntax is as follows:

	#3currencyconvert <#>#0 - shows the conversion of a #5base value#0 number to #6currency text#0 in all currencies
	#3currencyconvert reverse <text>#0 - converts a #6currency text#0 to a #5base value#0 in the user's current currency",
		AutoHelp.HelpArgOrNoArg)]
	protected static void CurrencyTest(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		decimal amount;
		var sb = new StringBuilder();
		if (ss.PeekSpeech().ToLowerInvariant().EqualTo("reverse"))
		{
			if (actor.Currency == null)
			{
				actor.OutputHandler.Send(
					"You have not set a currency to use in economic transactions, which is necessary to use this command.");
				return;
			}

			ss.PopSpeech();
			amount = actor.Currency.GetBaseCurrency(ss.SafeRemainingArgument, out var success);
			if (success)
			{
				sb.AppendLine(
					$"The currency string {ss.SafeRemainingArgument.ColourCommand()} equates to {amount.ToString("N2", actor).ColourValue()} base units, and described as follows:");
				foreach (var value in Enum.GetValues(typeof(CurrencyDescriptionPatternType)))
				{
					sb.AppendLine(
						$"\tFor type {Enum.GetName(typeof(CurrencyDescriptionPatternType), value).Colour(Telnet.Cyan)} the value is {actor.Currency.Describe(amount, (CurrencyDescriptionPatternType)value).Colour(Telnet.Green)}"
							.NoWrap());
				}

				actor.OutputHandler.Send(sb.ToString());
				return;
			}

			actor.OutputHandler.Send("The supplied text did not successfully convert to a currency string.");
			return;
		}

		if (!decimal.TryParse(ss.Pop(), out amount))
		{
			actor.OutputHandler.Send("That is not a valid decimal number.");
			return;
		}

		foreach (var currency in actor.Gameworld.Currencies)
		{
			if (sb.Length != 0)
			{
				sb.AppendLine();
			}

			sb.AppendLine(
				$"Showing {amount.ToString("N2", actor).ColourValue()} in the {currency.Name.ColourName()} currency:");
			foreach (var value in Enum.GetValues(typeof(CurrencyDescriptionPatternType)))
			{
				sb.AppendLine(
					$"\tFor type {Enum.GetName(typeof(CurrencyDescriptionPatternType), value).Colour(Telnet.Cyan)} the value is {currency.Describe(amount, (CurrencyDescriptionPatternType)value).Colour(Telnet.Green)}"
						.NoWrap());
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("EditStaticString", "editstaticstring")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	[HelpInfo("editstaticstring",
		@"This command is used to edit static strings, which are typically areas where the echoes of the MUD can be customised in some way. For example, the main menu login screen and the emote when someone begins to fly are both examples of static strings.

This command should be used with extreme caution. You can break the game with bad strings and need to fix it in the database.

The syntax is #3editstaticstring <whichstring>#0, which will drop you into an editor. You can use #3editstaticstring#0 on its own to see a list of strings that you can edit.",
		AutoHelp.HelpArg)]
	protected static void EditStaticString(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which static string would you like to edit? The options are as follows:\n\n{actor.Gameworld.StaticStringNames.OrderBy(x => x).Select(x => x.ColourCommand()).SplitTextIntoColumns((uint)actor.LineFormatLength / 60U, (uint)actor.LineFormatLength)}",
				nopage: true);
			return;
		}

		var text = ss.SafeRemainingArgument;
		var matchingName = actor.Gameworld.StaticStringNames.FirstOrDefault(x => x.EqualTo(text));
		if (string.IsNullOrEmpty(matchingName))
		{
			actor.OutputHandler.Send(
				$"There is no static string with that name. Type {"editstaticstring".FluentTagMXP("send", "href='editstaticstring'")} to see a list.",
				nopage: true);
			return;
		}

		var oldValue = actor.Gameworld.GetStaticString(matchingName);
		if (!string.IsNullOrEmpty(oldValue))
		{
			actor.OutputHandler.Send("Replacing:\n\n" + oldValue);
		}

		actor.OutputHandler.Send("Enter the new value of the static string in the editor below.");

		actor.EditorMode(PostStringAction, CancelStringAction, 1.0, oldValue,
			EditorOptions.None, new object[] { matchingName, actor.Gameworld });
	}

	private static void CancelStringAction(IOutputHandler handler, object[] args)
	{
		var which = args[0].ToString();
		handler.Send($"\nYou decide not to alter the value of static string {which.ColourCommand()}.");
	}

	private static void PostStringAction(string text, IOutputHandler handler, object[] args)
	{
		var which = args[0].ToString();
		text = text.SanitiseExceptNumbered(10);

		try
		{
			var test = string.Format(text, new string[10]);
		}
		catch (Exception)
		{
			handler.Send(
				"\nThere was an error with your submitted string - most likely unbalanced curly braces. Please check your string. If you must use curly braces that are not intended to be part of a text-replacement (e.g. not being used as like {1} as supported by your particular string), you should double the curly brace to escape it, e.g. { => {{ or } => }}");
			return;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.StaticStrings.Find(which);
			if (dbitem == null)
			{
				FMDB.Context.StaticStrings.Add(new StaticString { Id = which, Text = text });
			}
			else
			{
				dbitem.Text = text;
			}

			FMDB.Context.SaveChanges();
		}

		handler.Send($"\nYou change the value of the static string {which.ColourCommand()}.");
		((IFuturemud)args[1]).UpdateStaticString(which, text);
	}

	[PlayerCommand("EditStaticConfiguration", "editstaticconfiguration")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	[HelpInfo("editstaticconfiguration",
		@"This command is used to edit static configurations, which are used to configure certain miscellaneous settings for the game that don't belong elsewhere.

This command should be used with extreme caution. You can break the game with bad strings and need to fix it in the database.

Due to the nature of how some of these settings are used the MUD may need to be rebooted for them to apply, so you should generally reboot the MUD after you are done with this command and associated editing.

The syntax is #3editstaticconfig <whichsetting>#0, which will drop you into an editor. You can use #3editstaticconfig#0 on its own to see a list of configurations that you can edit.",
		AutoHelp.HelpArg)]
	protected static void EditStaticConfiguration(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which static configuration would you like to edit? The options are as follows:\n\n{actor.Gameworld.StaticConfigurationNames.OrderBy(x => x).Select(x => x.ColourCommand()).SplitTextIntoColumns((uint)actor.LineFormatLength / 60U, (uint)actor.LineFormatLength)}",
				nopage: true);
			return;
		}

		var text = ss.SafeRemainingArgument;
		var matchingName = actor.Gameworld.StaticConfigurationNames.FirstOrDefault(x => x.EqualTo(text));
		if (string.IsNullOrEmpty(matchingName))
		{
			actor.OutputHandler.Send(
				$"There is no static configuration with that name. Type {"editstaticconfig".FluentTagMXP("send", "href='editstaticconfig'")} to see a list.",
				nopage: true);
			return;
		}

		var oldValue = actor.Gameworld.GetStaticConfiguration(matchingName);
		if (!string.IsNullOrEmpty(oldValue))
		{
			actor.OutputHandler.Send("Replacing:\n\n" + oldValue);
		}

		actor.OutputHandler.Send("Enter the new value of the static configuration in the editor below.");

		actor.EditorMode(PostConfigAction, CancelConfigAction, 1.0, oldValue,
			EditorOptions.None, new object[] { matchingName, actor.Gameworld });
	}

	private static void CancelConfigAction(IOutputHandler handler, object[] args)
	{
		var which = args[0].ToString();
		handler.Send($"\nYou decide not to alter the value of static configuration {which.ColourCommand()}.");
	}

	private static void PostConfigAction(string text, IOutputHandler handler, object[] args)
	{
		var which = args[0].ToString();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.StaticConfigurations.Find(which);
			if (dbitem == null)
			{
				FMDB.Context.StaticConfigurations.Add(
					new StaticConfiguration { SettingName = which, Definition = text });
			}
			else
			{
				dbitem.Definition = text;
			}

			FMDB.Context.SaveChanges();
		}

		handler.Send($"\nYou change the value of the static configuration {which.ColourCommand()}.");
		((IFuturemud)args[1]).UpdateStaticConfiguration(which, text);
	}

	[PlayerCommand("Map", "map")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("map",
		@"This command will show you a textual representation of your surroundings, with some attempt to show you key features. The syntax is simply #3map#0.

Warning: This command doesn't play especially nice with diagonal exits (NW, NE, SE, SW) in that it doesn't display them. Also if your building is particularly non-cartesian you might not get a useful map.",
		AutoHelp.HelpArg)]
	protected static void Map(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		// TODO - different display modes based on input

		var width = (actor.LineFormatLength - 11) / 10;
		if (width % 2 == 0)
		{
			width -= 1;
		}

		var centre = width / 2;

		var cells = new ICell[width, width];
		var hasNonCompass = new bool[width, width];
		var hasCartesianClashes = new bool[width, width];
		var hasBank = new bool[width, width];
		var hasShop = new bool[width, width];
		var hasAuctionHouse = new bool[width, width];
		var hasPlayers = new bool[width, width];
		var hasHostiles = new bool[width, width];

		cells[centre, centre] = actor.Location;
		var exits = actor.Location.ExitsFor(actor, true).ToList();
		var queue = new Queue<(ICellExit Exit, int OriginX, int OriginY)>();

		foreach (var exit in exits)
		{
			queue.Enqueue((exit, centre, centre));
		}

		void AddExitCell(ICellExit exitToAdd, int originX, int originY)
		{
			switch (exitToAdd.OutboundDirection)
			{
				case CardinalDirection.North:
					originY -= 1;
					break;
				case CardinalDirection.NorthEast:
					originY -= 1;
					originX += 1;
					break;
				case CardinalDirection.East:
					originX += 1;
					break;
				case CardinalDirection.SouthEast:
					originX += 1;
					originY += 1;
					break;
				case CardinalDirection.South:
					originY += 1;
					break;
				case CardinalDirection.SouthWest:
					originX -= 1;
					originY += 1;
					break;
				case CardinalDirection.West:
					originX -= 1;
					break;
				case CardinalDirection.NorthWest:
					originX -= 1;
					originY -= 1;
					break;
				case CardinalDirection.Up:
				case CardinalDirection.Down:
				case CardinalDirection.Unknown:
					hasNonCompass[originX, originY] = true;
					break;
			}

			if (originX < 0 || originX >= width || originY < 0 || originY >= width)
			{
				return;
			}

			if (cells[originX, originY] is not null)
			{
				if (cells[originX, originY] != exitToAdd.Destination)
				{
					hasCartesianClashes[originX, originY] = true;
				}

				return;
			}

			var destinationCell = exitToAdd.Destination;
			cells[originX, originY] = destinationCell;
			foreach (var newExit in destinationCell.ExitsFor(actor, true).Except(exitToAdd))
			{
				switch (newExit.OutboundDirection)
				{
					case CardinalDirection.Up:
					case CardinalDirection.Down:
					case CardinalDirection.Unknown:
						hasNonCompass[originX, originY] = true;
						break;
				}

				queue.Enqueue((newExit, originX, originY));
			}

			if (destinationCell.Shop is not null)
			{
				hasShop[originX, originY] = true;
			}

			if (actor.Gameworld.Banks.Any(x => x.BranchLocations.Contains(destinationCell)))
			{
				hasBank[originX, originY] = true;
			}

			if (actor.Gameworld.AuctionHouses.Any(x => x.AuctionHouseCell == destinationCell))
			{
				hasAuctionHouse[originX, originY] = true;
			}

			if (destinationCell.Characters.Any(x => x.IsPlayerCharacter))
			{
				hasPlayers[originX, originY] = true;
			}

			if (destinationCell.Characters.Any(x =>
				    x is INPC npc && !npc.AffectedBy<IPauseAIEffect>() && npc.AIs.Any(y => y.CountsAsAggressive)))
			{
				hasHostiles[originX, originY] = true;
			}
		}

		while (queue.Count > 0)
		{
			var (exit, x, y) = queue.Dequeue();
			AddExitCell(exit, x, y);
		}

		hasPlayers[centre, centre] = true;

		actor.OutputHandler.Send(
			StringUtilities.DrawMap(actor, width, width, cells, hasNonCompass, hasCartesianClashes, hasBank, hasShop,
				hasAuctionHouse, hasPlayers, hasHostiles), nopage: true);
	}

	[PlayerCommand("Reskin", "reskin")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("reskin",
		@"This command is used to allow an admin to apply a skin to an item already in game. See ITEMSKIN LIST for a list of item skins.

The syntax is as follows:

	#3reskin <item> <skin>#0 - applies a skin to an item
	#3reskin <item> none#0 - removes a skin from an item", AutoHelp.HelpArgOrNoArg)]
	protected static void Reskin(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = actor.TargetItem(ss.PopSpeech());
		if (target is null)
		{
			actor.OutputHandler.Send("You don't see anything like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which skin would you like to apply?");
			return;
		}

		var old = target.HowSeen(actor);
		if (ss.SafeRemainingArgument.EqualTo("none"))
		{
			if (target.Skin is null)
			{
				actor.OutputHandler.Send($"{target.HowSeen(actor, true)} does not have a skin applied to it.");
				return;
			}

			target.Skin = null;
			actor.OutputHandler.Send($"You remove the skin from {old}.\nIt now displays as {target.HowSeen(actor)}.");
			return;
		}

		var skin = actor.Gameworld.ItemSkins.GetByIdOrName(ss.SafeRemainingArgument);
		if (skin is null)
		{
			actor.OutputHandler.Send("There is no such skin.");
			return;
		}

		if (skin.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"{skin.EditHeader().ColourName()} is not approved for use.");
			return;
		}

		var (truth, error) = skin.CanUseSkin(actor, target.Prototype);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}


		target.Skin = skin;
		actor.OutputHandler.Send(
			$"You apply the {skin.EditHeader().ColourName()} skin to the item {old}.\nIt now displays as {target.HowSeen(actor)}.");
	}

	[PlayerCommand("FindStock", "findstock")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("findstock",
		@"This command is used to search shops, vending machines and future equivalents for a specified item and tell you what it usually costs.

The syntax is as follows:

	#3findstock <idnum>#0 - finds all stock of a particular proto
	#3findstock <item>#0 - finds all stock of a specified item's proto", AutoHelp.HelpArgOrNoArg)]
	protected static void FindStock(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		long protoId;
		IGameItem targetItem = null;
		if (long.TryParse(ss.SafeRemainingArgument, out protoId))
		{
			if (actor.Gameworld.ItemProtos.Get(protoId) is null)
			{
				actor.OutputHandler.Send("There is no item prototype with that ID number.");
				return;
			}
		}
		else
		{
			targetItem = actor.TargetItem(ss.SafeRemainingArgument);
			if (targetItem is null)
			{
				actor.OutputHandler.Send("You can't see any item like that.");
				return;
			}

			protoId = targetItem.Id;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Finding Stock for Item {actor.Gameworld.ItemProtos.Get(protoId).EditHeader().ColourObject()}");
		sb.AppendLine();
		var found = false;

		foreach (var vendingMachine in actor.Gameworld.Items.SelectNotNull(x => x.GetItemType<IVendingMachine>()))
		{
			var selection = vendingMachine.Selections.FirstOrDefault(x => x.Prototype.Id == protoId);
			if (selection is null)
			{
				continue;
			}

			sb.AppendLine(
				$"{vendingMachine.Currency.Describe(selection.Cost, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} @ {vendingMachine.Parent.HowSeen(actor)} in {vendingMachine.Parent.TrueLocations.First().GetFriendlyReference(actor).ColourName()}");
			found = true;
		}

		foreach (var shop in actor.Gameworld.Shops)
		foreach (var merchandise in shop.Merchandises)
		{
			if (merchandise.Item.Id != protoId)
			{
				continue;
			}

			sb.AppendLine(
				$"{shop.Currency.Describe(shop.PriceForMerchandise(actor, merchandise, 1), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} @ {shop.Name.ColourName()} in {shop.CurrentLocations.Select(x => x.GetFriendlyReference(actor).ColourName()).DefaultIfEmpty("Nowhere".ColourError()).ListToString()}");
			found = true;
		}

		if (!found)
		{
			sb.AppendLine("None found.");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("SummonItem", "summonitem")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("summonitem",
		@"This command is used to take an item from anywhere in the world (even players inventories or orphaned items that don't exist anywhere) and gives it to you, removing it from wherever it was.

#1Warning: This command could potentially be very dangerous if you summon things like property keys or other things that are meant to be off-grid. It's not possible for this command to detect all of these kinds of collisions. Use with caution.#0

The syntax for this command is simply #3summonitem <id>#0.", AutoHelp.HelpArgOrNoArg)]
	protected static void SummonItem(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (!long.TryParse(ss.SafeRemainingArgument, out var id))
		{
			actor.OutputHandler.Send($"That is not a valid ID number.");
			return;
		}

		var item = actor.Gameworld.TryGetItem(id, true);
		if (item is null)
		{
			actor.OutputHandler.Send("There was no item like that.");
			return;
		}

		item.ContainedIn?.Take(item);
		item.InInventoryOf?.Take(item);
		item.Location?.Extract(item);
		item.Quit();
		item.Login();
		var sb = new StringBuilder();
		sb.AppendLine(
			$"You summon item #{id.ToString("N0", actor)} ({item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}).");
		if (actor.Body.CanGet(item, 0))
		{
			actor.Body.Get(item);
		}
		else
		{
			item.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(item);
			sb.AppendLine($"You couldn't hold it, so it is now on the ground.");
		}

		actor.OutputHandler.Send(sb.ToString());
	}
}