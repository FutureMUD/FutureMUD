﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
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
using NewPlayer = MudSharp.Effects.Concrete.NewPlayer;
using OpenAI_API.Moderation;

namespace MudSharp.Commands.Modules;

internal class StaffModule : Module<ICharacter>
{
	private StaffModule()
		: base("Staff")
	{
		IsNecessary = true;
	}

	public static StaffModule Instance { get; } = new();

	[PlayerCommand("TestAnsi", "testansi")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("testansi", @"This command is used to give you a list of all of the codes that can be used to add colour to text in echoes, descriptions etc. It will also help you see which of these are supported by your current MUD client.

The syntax is simply #3testansi#0.", AutoHelp.HelpArg)]
	protected static void TestANSI(ICharacter actor, string input)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Normal (#0):         text");
		sb.AppendLine($"Red (#1):            {"text".Colour(Telnet.Red)}");
		sb.AppendLine($"Green (#2):          {"text".Colour(Telnet.Green)}");
		sb.AppendLine($"Yellow (#3):         {"text".Colour(Telnet.Yellow)}");
		sb.AppendLine($"Blue (#4):           {"text".Colour(Telnet.Blue)}");
		sb.AppendLine($"Magenta (#5):        {"text".Colour(Telnet.Magenta)}");
		sb.AppendLine($"Cyan (#6):           {"text".Colour(Telnet.Cyan)}");
		sb.AppendLine($"Black (#7):          {"text".Colour(Telnet.BoldBlack)}");
		sb.AppendLine($"Orange (#8):         {"text".Colour(Telnet.Orange)}");
		sb.AppendLine($"Pink (#I):           {"text".Colour(Telnet.Pink)}");
		sb.AppendLine($"Bold Red (#9):       {"text".Colour(Telnet.BoldRed)}");
		sb.AppendLine($"Bold Green (#A):     {"text".Colour(Telnet.BoldGreen)}");
		sb.AppendLine($"Bold Yellow (#B):    {"text".Colour(Telnet.BoldYellow)}");
		sb.AppendLine($"Bold Blue (#C):      {"text".Colour(Telnet.BoldBlue)}");
		sb.AppendLine($"Bold Magenta (#D):   {"text".Colour(Telnet.BoldMagenta)}");
		sb.AppendLine($"Bold Cyan (#E):      {"text".Colour(Telnet.BoldCyan)}");
		sb.AppendLine($"Bold White (#F):     {"text".Colour(Telnet.BoldWhite)}");
		sb.AppendLine($"Bold Orange (#G):    {"text".Colour(Telnet.BoldOrange)}");
		sb.AppendLine($"Bold Pink (#H):      {"text".Colour(Telnet.BoldPink)}");
		sb.AppendLine($"Function Yellow (#J):      {"text".Colour(Telnet.FunctionYellow)}");
		sb.AppendLine($"Variable Green (#K):      {"text".Colour(Telnet.VariableGreen)}");
		sb.AppendLine($"Keyword Blue (#L):      {"text".Colour(Telnet.KeywordBlue)}");
		sb.AppendLine($"Variable Cyan (#M):      {"text".Colour(Telnet.VariableCyan)}");
		sb.AppendLine($"Text Red (#N):      {"text".Colour(Telnet.TextRed)}");
		sb.AppendLine($"Keyword Pink (#O):      {"text".Colour(Telnet.KeywordPink)}");
		sb.AppendLine($"This text has {"some underlined text".Underline()} in it.");
		sb.AppendLine($"This text has {"some blinking text".Blink()} in it.");
		sb.AppendLine($"This text is {"coloured and underlined".Underline().Colour(Telnet.BoldRed)}.");
		actor.OutputHandler.Send(sb.ToString());
	}

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
	[HelpInfo("givetattoo", "Gives someone a tattoo. #3Syntax: givetattoo <target> <tattoo> <bodypart>#0",
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
	[HelpInfo("setgender", "Sets gender of a target character. Syntax: #3setgender <target> <gender>#0",
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

	[PlayerCommand("SetAge", "setage")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("setage", "Sets age of a target character. Maintains existing birthday. Syntax: #3setage <target> <age>#0",
		AutoHelp.HelpArgOrNoArg)]
	protected static void SetAge(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anyone like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("How old do you want to make them?");
			return;
		}

		if (!int.TryParse(ss.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid non-zero number.");
			return;
		}

		var existing = new MudDate(actor.Birthday);
		var existingAge = existing.Calendar.CurrentDate.YearsDifference(existing);
		if (existingAge == value)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is already {value.ToStringN0Colour(actor)} years old.");
			return;
		}

		existing.AdvanceYears(existingAge - value, true);
		actor.Birthday = existing;
		actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is now {value.ToStringN0Colour(actor)} years old, with a birthday of {existing.Display(CalendarDisplayMode.Short).ColourValue()}.");
	}

	[PlayerCommand("SetBirthday", "setbirthday")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("setbirthday", "Sets the birthday of a target character. Syntax: #3setbirthday <target> <date>#0",
		AutoHelp.HelpArgOrNoArg)]
	protected static void SetBirthday(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anyone like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What date should their birthday be?");
			return;
		}

		if (!actor.Birthday.Calendar.TryGetDate(ss.SafeRemainingArgument, actor, out var newDate, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (newDate > actor.Birthday.Calendar.CurrentDate)
		{
			actor.OutputHandler.Send("Characters cannot have birthdays in the future.");
			return;
		}

		actor.Birthday = newDate;
		actor.OutputHandler.Send($"{target.HowSeen(actor, true)} now has a birthday of {newDate.Display(CalendarDisplayMode.Short).ColourValue()}, making them {newDate.Calendar.CurrentDate.YearsDifference(newDate).ToStringN0Colour(actor)} years old.");
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
	[HelpInfo("setcharacters", @"The #3setcharacters#0 command allows you to set the maximum number of active characters an account can have at one time. 

The default amount is obviously 1 - an account only having one character at a time. A common example of when you might want to increase this is to permit a character to make an admin avatar while still having an active player character.

The syntax is #3setcharacters <account> <##characters>#0.", AutoHelp.HelpArgOrNoArg)]
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

			if (!uint.TryParse(ss.SafeRemainingArgument, out var value))
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
		@"This command is used to shut down the game. There are two versions: #3shutdown reboot#0 and #3shutdown stop#0. Using the command #3shutdown#0 with no arguments is equivalent to the reboot version.

#3Shutdown reboot#0 means that the engine will immediately reboot. This is often used when an update needs to happen for example.

#3Shutdown stop#0 means that the engine will not attempt to reboot. You might use this version if you need to restart your server or you have some other reason to take the game down and not have it immediately come back up.",
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
	[HelpInfo("where", @"The #3where#0 command shows all characters who are logged in to the game and their whereabouts. It is one of the ways that you as an admin can keep an eye on where people are and what they're up to.

The syntax is simply #3where#0.", AutoHelp.HelpArg)]
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
	[HelpInfo("users", @"The #3users#0 command shows all telnet connections that are connected to the MUD.

The syntax is simply #3users#0.", AutoHelp.HelpArg)]
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
	[HelpInfo("broadcast", @"The #3broadcast#0 command is used to send a system message to all players logged in to the MUD, as well as the discord server.

The syntax is simply #3broadcast <message>#0.", AutoHelp.HelpArgOrNoArg)]
	protected static void Broadcast(ICharacter actor, string command)
	{
		var output = command.RemoveFirstWord();
		if (string.IsNullOrWhiteSpace(output))
		{
			actor.OutputHandler.Send("What is it that you want to broadcast to the entire MUD?");
			return;
		}

		output = output.ProperSentences().Fullstop();
		actor.Gameworld.SystemMessage(output);
		actor.Gameworld.DiscordConnection.HandleBroadcast(output);
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
	[HelpInfo("purge", @"The #3purge#0 command is used to delete all of the items in the current room that you're in. This is irreversible and has no confirmation in many circumstances, so be sure you don't have anything you want to keep in the room before you use this command.

Some types of items will require you to #3accept#0 before the purge will go through; including for example corpses, non-empty containers and certain other critical items.

There are three forms for this command:

	#3purge#0 - purges all items in the room
	#3purge <item>#0 - purges a single specific item from the room
	#3purge all <keyword>#0 - purges all items in the room that match the keyword", AutoHelp.HelpArg)]
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
	[HelpInfo("debug", @"The debug command is used for a few random commands that are sometimes useful when trying to fix a problem or test something. This command is restricted to Senior Admins and up because they're somewhat niche cases and could have side effects.

#1You should be very careful using commands in this module.#0

The following options are available:

	#3debug dead#0 - reports on characters with weird death states (obscure bugs)
	#3debug save#0 - sends you debug info on the save queue. Can be useful to ensure things are saving.
	#3debug character <id>#0 - shows save related debug info about a character
	#3debug dream <character>#0 - gives a random dream to a sleeping character
	#3debug mode#0 - toggles opting in to receiving debug messages from the engine
	#3debug update#0 - downloads and applies the latest patch for FutureMUD (only use if you're using a Binaries folder swap, like the autoseeder sets up)
	#3debug coordinates#0 - recalculates all the x,y,z coordinates for all zones
	#3debug hitchance <target> [<weapon attack>] [front|back|rflank|lflank]#0 - shows hit chances (optionally for attack)
	#3debug healing#0 - attaches the healing logger (writes to file)
	#3debug skills#0 - attaches the skill check logger (writes to file)
	#3debug scheduler#0 - shows all things in the scheduler
	#3debug listeners#0 - shows all listeners
	#3debug fixmorph#0 - resets all morph timers of shop stocked items
	#3debug fixbites#0 - resets all bite counts of shop stocked items", AutoHelp.HelpArgOrNoArg)]
	protected static void Debug(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopForSwitch())
		{
			case "fixbites":
				DebugFixBites(actor);
				return;
			case "fixmorph":
				DebugFixMorph(actor);
				return;
			case "dead":
				DebugDead(actor);
				return;
			case "save":
				DebugSaveQueue(actor);
				return;
			case "scheduler":
			case "schedules":
				DebugScheduler(actor);
				return;

			case "listeners":
				DebugListeners(actor);
				return;
			case "char":
			case "character":
				DebugCharacter(actor, ss);
				return;
			case "dream":
				DebugDream(actor, ss);
				return;
			case "healing":
				DebugHealing(actor, ss);
				return;
			case "skills":
				DebugSkills(actor);
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
			case "update":
				Debug_Update(actor);
				return;
			case "coordinates":
				DebugCoordinates(actor);
				return;
			case "hitchance":
				DebugHitChance(actor, ss);
				return;
			default:
				actor.Send("That's not a known debug routine.");
				return;
		}
	}

	#region Debug Sub-Routines
	
	private static void DebugFixBites(ICharacter actor)
	{
		var count = 0;
		foreach (var item in actor.Gameworld.Items)
		{
			if (!item.AffectedBy<ItemOnDisplayInShop>())
			{
				continue;
			}

			var food = item.GetItemType<IEdible>();
			if (food is null)
			{
				continue;
			}

			count++;
			food.BitesRemaining = food.TotalBites;
		}

		actor.OutputHandler.Send($"Reset the bite count on {count.ToStringN0Colour(actor)} stocked items.");
	}

	private static void DebugFixMorph(ICharacter actor)
	{
		var count = 0;
		foreach (var item in actor.Gameworld.Items)
		{
			if (item.AffectedBy<ItemOnDisplayInShop>())
			{
				item.ResetMorphTimer();
				count++;
			}
		}

		actor.OutputHandler.Send($"Reset the morph timer on {count.ToStringN0Colour(actor)} stocked items.");
	}
	private static void DebugScheduler(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Main Scheduler:");
		actor.Gameworld.Scheduler.DebugOutputForScheduler(sb);
		sb.AppendLine();
		sb.AppendLine("Effect Scheduler:");
		actor.Gameworld.EffectScheduler.DebugOutputForScheduler(sb);
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void DebugListeners(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine("The following listeners are registered to the gameworld:");
		foreach (var listener in actor.Gameworld.Listeners)
		{
			sb.AppendLine($"\t{listener}");
		}

		actor.Send(sb.ToString());
	}

	private static void Debug_Update(ICharacter actor)
	{
		actor.OutputHandler.Send("Downloading and applying the update...");
		actor.Gameworld.ForceOutgoingMessages();
		Thread.Sleep(100);
		using var hc = new HttpClient();
		using var responseTask = hc.GetAsync(
			Environment.OSVersion.Platform.In(PlatformID.Unix, PlatformID.MacOSX, PlatformID.Other) ?
				"https://www.labmud.com/downloads/FutureMUD-Linux.zip" :
				"https://www.labmud.com/downloads/FutureMUD-Windows.zip");
		using var stream = Task.Run(() => responseTask).Result.Content.ReadAsStream();
		var root = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		using var zip = File.Open(string.IsNullOrEmpty(root) ? "FutureMUD Update.zip" : System.IO.Path.Combine(root, "FutureMUD Update.zip"), FileMode.Create);
		stream.CopyTo(zip);
		var toPath = System.IO.Path.GetFullPath(string.IsNullOrEmpty(root) ? "Binaries" : System.IO.Path.Combine(root, "Binaries"));
		if (!Directory.Exists(toPath))
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Directory.CreateDirectory(toPath, UnixFileMode.UserWrite | UnixFileMode.UserRead | UnixFileMode.UserExecute | UnixFileMode.SetUser | UnixFileMode.SetGroup | UnixFileMode.GroupExecute | UnixFileMode.GroupRead | UnixFileMode.GroupWrite);
			}
			else
			{
				Directory.CreateDirectory(toPath);
			}
		}

		zip.Close();

		using var archive = ZipFile.OpenRead(string.IsNullOrEmpty(root) ? "FutureMUD Update.zip" : System.IO.Path.Combine(root, "FutureMUD Update.zip"));
		foreach (var entry in archive.Entries)
		{
			var destination = System.IO.Path.GetFullPath(entry.FullName, toPath);
			entry.ExtractToFile(destination, true);
		}

		actor.OutputHandler.Send("Done.");
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

	private static void DebugHealing(ICharacter actor, StringStack ss)
	{
		var filename = $"Healing Audit {DateTime.UtcNow:yyyy MMMM dd hh mm ss}.csv";
		actor.Gameworld.LogManager.InstallLogger(new HealingLogger
			{ FileName = filename });
		actor.Send($"Healing Audit commenced - saved as {filename.ColourName()}.");
	}

	private static void DebugSkills(ICharacter actor)
	{
		var filename = $"Skill Audit {DateTime.UtcNow:yyyy MMMM dd hh mm ss}.txt";
		actor.Gameworld.LogManager.InstallLogger(new CustomSkillLogger
		{
			FileName = filename
		});
		actor.Send($"Attached the skill logger - saved as {filename.ColourName()}.");
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

	private static void DebugCoordinates(ICharacter actor)
	{
		actor.Send("Recalculating all of the zone coordinates...");
		foreach (var zone in actor.Gameworld.Zones)
		{
			zone.CalculateCoordinates();
		}

		actor.Send("Done recalculating all of the zone coordinates.");
	}
	private static void DebugHitChance(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to debug the hit chance for?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		var totalHitChance = target.Body.Bodyparts.Sum(x => x.RelativeHitChance);
		var sb = new StringBuilder();
		var chances = new DoubleCounter<IBodypart>();
		var limbChances = new DoubleCounter<ILimb>();
		var facing = Facing.Front;

		if (!ss.IsFinished)
		{
			var attack = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.WeaponAttacks.Get(value)
				: actor.Gameworld.WeaponAttacks.GetByName(ss.Last);
			if (attack == null)
			{
				actor.OutputHandler.Send("There is no such weapon attack.");
				return;
			}

			if (!ss.IsFinished)
			{
				switch (ss.PopSpeech().ToLowerInvariant())
				{
					case "front":
						facing = Facing.Front;
						break;
					case "back":
					case "rear":
						facing = Facing.Rear;
						break;
					case "right":
					case "rightflank":
					case "right flank":
						facing = Facing.RightFlank;
						break;
					case "left":
					case "leftflank":
					case "left flank":
						facing = Facing.LeftFlank;
						break;
				}
			}

			for (var i = -2; i <= 2; i++)
			{
				var alignmentMultiplier = 0.0;
				switch (i)
				{
					case -2:
					case 2:
						alignmentMultiplier = 0.01;
						break;
					case 1:
					case -1:
						alignmentMultiplier = 0.24;
						break;
					case 0:
						alignmentMultiplier = 0.5;
						break;
				}

				var alignment = BodyExtensions.FromAlignmentPolar((attack.Alignment.ToAlignmentPolar(facing) + i) % 8);

				for (var j = -2; j <= 2; j++)
				{
					var multiplier = alignmentMultiplier;
					switch (j)
					{
						case -2:
						case 2:
							multiplier *= 0.01;
							break;
						case 1:
						case -1:
							multiplier *= 0.24;
							break;
						case 0:
							multiplier *= 0.5;
							break;
					}

					var orientation = attack.Orientation.RaiseUp(j);
					var parts = target.Body.Bodyparts.Where(x =>
						x.Alignment.LeftRightOnly() == alignment.LeftRightOnly() &&
						x.Alignment.FrontRearOnly() == alignment.FrontRearOnly() && x.Orientation == orientation &&
						x.RelativeHitChance > 0).ToList();
					var totalChance = parts.Sum(x => x.RelativeHitChance);
					foreach (var part in parts)
					{
						chances[part] += multiplier * 0.66666666666666667 * part.RelativeHitChance / totalChance;
						limbChances[target.Body.GetLimbFor(part)] +=
							multiplier * 0.66666666666666667 * part.RelativeHitChance / totalChance;
					}

					// If the code can't find a bodypart at that location, it just picks a totally random one instead
					if (!parts.Any())
					{
						foreach (var part in target.Body.Bodyparts.Where(x => x.RelativeHitChance > 0))
						{
							chances[part] += multiplier * 0.66666666666666667 * part.RelativeHitChance / totalHitChance;
							limbChances[target.Body.GetLimbFor(part)] += multiplier * 0.66666666666666667 *
								part.RelativeHitChance / totalHitChance;
						}
					}
				}

				var appendageParts = target.Body.Bodyparts.Where(x =>
					x.Alignment.LeftRightOnly() == alignment.LeftRightOnly() &&
					x.Alignment.FrontRearOnly() == alignment.FrontRearOnly() &&
					x.Orientation == Orientation.Appendage && x.RelativeHitChance > 0).ToList();
				var appendageTotalChance = appendageParts.Sum(x => x.RelativeHitChance);
				foreach (var part in appendageParts)
				{
					chances[part] += alignmentMultiplier * 0.3333333333333333333333333 * part.RelativeHitChance /
									 appendageTotalChance;
					limbChances[target.Body.GetLimbFor(part)] += alignmentMultiplier * 0.3333333333333333333333333 *
						part.RelativeHitChance / appendageTotalChance;
				}

				// If the code can't find a bodypart at that location, it just picks a totally random one instead
				if (!appendageParts.Any())
				{
					foreach (var part in target.Body.Bodyparts.Where(x => x.RelativeHitChance > 0))
					{
						chances[part] += alignmentMultiplier * 0.3333333333333333333333333 * part.RelativeHitChance /
										 totalHitChance;
						limbChances[target.Body.GetLimbFor(part)] += alignmentMultiplier * 0.3333333333333333333333333 *
							part.RelativeHitChance / totalHitChance;
					}
				}
			}

			sb.AppendLine(
				$"Hit Percentages for attack {attack.Name.Colour(Telnet.BoldRed)} against {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} when at {facing.Describe().Colour(Telnet.Cyan)}:");
		}
		else
		{
			sb.AppendLine($"Hit Percentages against {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)}:");
			var total = target.Body.Bodyparts.Sum(x => x.RelativeHitChance);
			foreach (var part in target.Body.Bodyparts)
			{
				chances[part] += part.RelativeHitChance / total;
				limbChances[target.Body.GetLimbFor(part)] += part.RelativeHitChance / total;
			}
		}

		sb.AppendLine();
		sb.AppendLine("Limbs:");
		sb.AppendLine();
		foreach (var limb in limbChances.OrderByDescending(x => x.Value))
		{
			sb.AppendLine($"\t{limb.Key.Name,-25} {limb.Value.ToString("P2", actor).ColourValue(),-6}");
		}

		sb.AppendLine();
		sb.AppendLine("Parts:");
		sb.AppendLine("");
		foreach (var pc in chances.OrderByDescending(x => x.Value))
		{
			sb.AppendLine($"\t{pc.Key.FullDescription(),-25} {pc.Value.ToString("P2", actor).ColourValue(),-6}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	#endregion

	[PlayerCommand("RegisterAccount", "registeraccount")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("registeraccount", @"This command is used to manually register an account when for whatever reason the email-based system is not functioning or you have some other reason to manually register an account.

The syntax for this command is as follows:

	#3registeraccount <account>#0 - sets the account to a registered account", AutoHelp.HelpArgOrNoArg)]
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
		@"This command allows you to set the layer of a target (characters and items) in the room to whatever you specify. 

The syntax is as follows: 

	#3setlayer <target> <layer>#0 - sets the layer of an item or character
	#3setlayer <target>#0 - shows the possible layers you could set the target to

The options for layers in general are as followed (but not all rooms will have all layers):

	#6GroundLevel
	Underwater
	DeepUnderwater
	VeryDeepUnderwater
	InTrees
	HighInTrees
	InAir
	HighInAir
	OnRooftops#0",
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
	[HelpInfo("plans", @"This command is used to view short and long term plans that players have set using the #3plan#0 command.

There are three ways to use this command:

	#3plans#0 - shows the plans of all online characters who have updated it within the last 14 days
	#3plans <name>#0 - shows the plans of a specific online character
	#3plans <id>#0 - shows the plans of a specific character, including offline characters", AutoHelp.HelpArg)]
	protected static void Plans(ICharacter actor, string command)
	{
		var sb = new StringBuilder();
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			var plans = actor.Gameworld.Characters
			                 .Where(x => x.AffectedBy<RecentlyUpdatedPlan>())
			                 .Select(x => (Character: x,
				                 Length: TimeSpan.FromDays(90) -
				                         x.ScheduledDuration(x.EffectsOfType<RecentlyUpdatedPlan>().First())))
			                 .OrderBy(x => x.Length)
			                 .ToList();
			if (!plans.Any())
			{
				actor.OutputHandler.Send("Nobody currently in the game has updated their plans within the last 90 days.");
				return;
			}

			sb.AppendLine("The following recent updates to character plans have been made:");
			foreach (var plan in plans)
			{
				sb.AppendLine(
					$"\n{plan.Character.PersonalName.GetName(NameStyle.FullName).ColourName()} ({plan.Character.HowSeen(actor, flags: PerceiveIgnoreFlags.TrueDescription)})");
				sb.AppendLine($"Short: {plan.Character.ShortTermPlan?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}");
				sb.AppendLine($"Long: {plan.Character.LongTermPlan?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}");
			}

			actor.OutputHandler.Send(sb.ToString());
		}

		var target = actor.Gameworld.Characters.GetByIdOrName(ss.SafeRemainingArgument);
		if (target is null ||
		    !long.TryParse(ss.SafeRemainingArgument, out var id) ||
			(target = actor.Gameworld.TryGetCharacter(id, true)) is null
		)
		{
			actor.OutputHandler.Send($"There is no character identified by the text {ss.SafeRemainingArgument.ColourCommand()}.");
			return;
		}

		sb.AppendLine($"Plans for {target.PersonalName.GetName(NameStyle.FullName).ColourName()} ({target.HowSeen(actor, flags: PerceiveIgnoreFlags.TrueDescription)})");
		sb.AppendLine($"Short: {target.ShortTermPlan?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Long: {target.LongTermPlan?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}");
		actor.OutputHandler.Send(sb.ToString());
	}

	private static string GridHelpText =>
		@"The #3grid#0 command is used to view, edit and create grids, which are ways in which certain resources (like electricity or water) are shared across multiple rooms with different inputs and outputs pulling dynamically from it.

A room can only have one of each kind of grid at a time, and the grids must be added to rooms to apply there. Other items (like electrical sockets) can be connected to the grid once it's there.

You can use the following subcommands with the grid command:

	#3grid audit#0 - shows all grids
	#3grid status <grid#>#0 - shows a particular grid
	#3grid expand <grid#> <direction>#0 - expands a grid in a direction
	#3grid withdraw <grid#>#0 removes the current location from the specified grid
	#3grid connect <thing> <grid#>#0 - connects a grid-interfacing item to a grid";

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

The syntax for this command is #3path <target>#0 to path to a character or #3path *<id>#0 to target a room. 

Note, you can use names or keywords to do this search, so the following three syntaxes would all be valid:

#3path amos#0
#3path tall.strapping.lad#0
#3path ""Amos Newbie""#0", AutoHelp.HelpArgOrNoArg)]
	protected static void Path(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var targetText = ss.SafeRemainingArgument;
		// Room Target
		IPerceivable target;
		if (targetText.Length > 1 && targetText[0] == '*')
		{
			if (!long.TryParse(targetText.Substring(1), out var id))
			{
				actor.OutputHandler.Send("If you specify a room, you must specify the room's ID.");
				return;
			}

			target = actor.Gameworld.Cells.Get(id);
			if (target is null)
			{
				actor.OutputHandler.Send("There is no room with that ID.");
				return;
			}
		}
		// Character Target
		else
		{
			target = actor.Gameworld.Actors.GetFromItemListByKeywordIncludingNames(targetText, actor);
			if (target == null)
			{
				actor.OutputHandler.Send("There is no such character for you to path to.");
				return;
			}
		}

		var exits1 = actor.ExitsBetween(target, 50).ToList();
		if (!exits1.Any())
		{
			actor.OutputHandler.Send("Could not find a path to that target within 50 rooms.");
			return;
		}

		var exits2 = actor.PathBetween(target, 50, PathSearch.PathIncludeUnlockableDoors(actor));

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
			var sb = new StringBuilder();
			sb.AppendLine("Which static configuration would you like to edit? The options are as follows:\n");
			sb.AppendLine(
				StringUtilities.GetTextTable(
					from item in actor.Gameworld.StaticConfigurationNames
					orderby item
					select new List<string>
					{
						item,
						actor.Gameworld.GetStaticConfiguration(item)
					},
					[
						"Setting",
						"Value"
					],
					actor,
					Telnet.Yellow,
					1
				)
			);
			actor.OutputHandler.Send(sb.ToString(), nopage: true);
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

	[PlayerCommand("Logs", "logs")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("logs", @"You can use this command to search the command logs. This will show you commands that characters have entered in your game. Depending on your MUD's settings, this may only go back so far and it also may exclude NPC commands.

The syntax for this command is #3logs [filters]#0. You can use multiple filters. See below for filter descriptions.

Filters:

	#6here#0 - show only logs entries in your current room
	#6*<id>#0 - show only log entries from the specified account (by ID)
	#6*<name>#0 - show only log entries from the specified account (by name)
	#6~<id>#0 - show only log entries from the specified character (by ID)
	#6~<keyword>#0 - show only log entries from the specified character that you can see
	#6>datetime#0 - show only log entries after the specified time
	#6<datetime#0 - show only log entries before the specified time
	#6$<minutes>#0 - show only log entries within the specified minutes from the present
	#6#<id>#0 - show a specific log entry by ID
	#6^<keyword>#0 - show only log entries starting with the specified text
	#6+<keyword>#0 - show only log entries containing the specified text
	#6-<keyword>#0 - exclude log entries containing the specified text

For example:

	#3logs here ~344 ^emote ""+Mary Jane""#0 would search for all logs at your current location, by character 344, starting with the text emote and containing the text ""Mary Jane"".

#BHint: This can lag the game if your logs are particularly large, and filters on the text are particularly slow. Best practice would be to order your filters so that you filter by any non-text parameters first - e.g. room, character, time, etc before you filter for keywords.#0", AutoHelp.HelpArgOrNoArg)]
	protected static void Logs(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var filterTexts = new List<string>();
		using (new FMDB())
		{
			var logs = FMDB.Context.CharacterLogs
			               .Include(x => x.Account)
			               .AsQueryable();

			while (!ss.IsFinished)
			{
				var cmd = ss.PopSpeech().ToLowerInvariant();
				if (cmd.EqualTo("here"))
				{
					logs = logs.Where(log => log.CellId == actor.Location.Id);
					filterTexts.Add($"in room {actor.Location.GetFriendlyReference(actor)}");
					continue;
				}

				if (cmd.Length > 1)
				{
					var cmd1 = cmd[1..];
					switch (cmd[0])
					{
						case '+':
							logs = logs.Where(log => log.Command.Contains(cmd1));
							filterTexts.Add($"with keyword {cmd1.ColourValue()}");
							continue;
						case '-':
							logs = logs.Where(log => !log.Command.Contains(cmd1));
							filterTexts.Add($"without keyword {cmd1.ColourValue()}");
							continue;
						case '*':
							if (long.TryParse(cmd1, out var id))
							{
								var result = id;
								logs = logs.Where(log => log.AccountId == result);
								filterTexts.Add($"from account {$"#{result.ToString("N0", actor)}".ColourValue()}");
								continue;
							}

							logs = logs.Where(log => log.Account.Name == cmd1);
							filterTexts.Add($"from account {cmd1.ColourValue()}");
							continue;
						case '~':
							if (long.TryParse(cmd1, out id))
							{
								var result = id;
								logs = logs.Where(log => log.CharacterId == result);
								filterTexts.Add($"from character {$"#{result.ToString("N0", actor)}".ColourValue()}");
								continue;
							}

							var target = actor.TargetActor(cmd1);
							if (target is not null)
							{
								var targetId = target.Id;
								logs = logs.Where(log => log.CharacterId == targetId);
								filterTexts.Add($"from character {target.HowSeen(actor)}");
								continue;
							}
							actor.OutputHandler.Send(
								$"You don't see anyone you can target by {cmd1.ColourCommand()}.");
							return;
							
						case '>':
							if (DateTime.TryParse(cmd1, actor, out var dt))
							{
								var udt = dt.ToUniversalTime();
								filterTexts.Add($"after {udt.GetLocalDateString(actor, true).ColourValue()}");
								logs = logs.Where(log => log.Time > udt);
								continue;
							}
							actor.OutputHandler.Send($"The text {cmd1.ColourCommand()} is not a valid datetime.");
							continue;
						case '<':
							if (DateTime.TryParse(cmd1, actor, out dt))
							{
								var udt = dt.ToUniversalTime();
								filterTexts.Add($"before {udt.GetLocalDateString(actor, true).ColourValue()}");
								logs = logs.Where(log => log.Time < udt);
								continue;
							}
							actor.OutputHandler.Send($"The text {cmd1.ColourCommand()} is not a valid datetime.");
							continue;

						case '^':
							logs = logs.Where(log => log.Command.StartsWith(cmd1));
							filterTexts.Add($"starting with {cmd1.ColourValue()}");
							continue;
						case '#':
							if (long.TryParse(cmd1, out id))
							{
								var result = id;
								logs = logs.Where(log => log.Id == result);
								filterTexts.Add($"with id {result.ToString("N0", actor).ColourValue()}");
								continue;
							}
							actor.OutputHandler.Send($"The text {cmd1.ColourCommand()} is not a valid ID.");
							continue;
						case '$':
							if (int.TryParse(cmd1, out var minutes))
							{
								var result = minutes;
								var now = DateTime.UtcNow;
								logs = logs.Where(log => (now - log.Time).Minutes <= result);
								filterTexts.Add($"within the last {result.ToString("N0", actor).ColourValue()} {"minute".Pluralise(result != 1)}");
								continue;
							}
							actor.OutputHandler.Send($"The text {cmd1.ColourCommand()} is not a valid amount of minutes.");
							continue;

					}
				}

				actor.OutputHandler.Send($"The text {cmd.ColourCommand()} is not a valid log filter.");
				return;
			}

			var filteredLogs = logs.ToList();
			var sb = new StringBuilder();
			sb.AppendLine($"Showing Logs...");
			foreach (var filter in filterTexts)
			{
				sb.AppendLine($"..{filter}");
			}

			sb.AppendLine();
			if (filteredLogs.Count == 1)
			{
				var log = filteredLogs[0];
				sb.AppendLine($"Log Entry #{log.Id.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.White));
				var ch = actor.Gameworld.TryGetCharacter(log.CharacterId, true);
				sb.AppendLine($"Character: {ch.PersonalName.GetName(NameStyle.FullName).ColourName()} (#{ch.Id.ToString("N0", actor)})");
				sb.AppendLine($"Account: {actor.Gameworld.Accounts.Get(log.AccountId ?? 0)?.Name.ColourName() ?? "None".ColourError()}");
				sb.AppendLine($"Location: {actor.Gameworld.Cells.Get(log.CellId)!.GetFriendlyReference(actor)}");
				sb.AppendLine($"Time: {log.Time.GetLocalDateString(actor, true).ColourValue()}");
				sb.AppendLine();
				sb.AppendLine(log.Command);
			}
			else
			{
				sb.AppendLine(StringUtilities.GetTextTable(
					from log in filteredLogs
					let cell = actor.Gameworld.Cells.Get(log.CellId)
					let account = actor.Gameworld.Accounts.Get(log.AccountId ?? 0)
					let ch = actor.Gameworld.TryGetCharacter(log.CharacterId, true)
					select new List<string>
					{
						log.Id.ToString("N0", actor),
						log.Command,
						log.Time.GetLocalDateString(actor, true),
						ch.Id.ToString("N0", actor),
						ch.PersonalName.GetName(NameStyle.FullName),
						account?.Name ?? "",
						cell.GetFriendlyReference(actor)
					},
					new List<string>
					{
						"Log #",
						"Command",
						"Time",
						"Who ID",
						"Who",
						"Account",
						"Room"
					},
					actor,
					Telnet.Yellow,
					1
				));
			}
			
			actor.OutputHandler.Send(sb.ToString());
		}
	}

	[PlayerCommand("ItemAudit", "itemaudit")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("itemaudit", @"The ItemAudit command is used to see how many items are loaded into the game world. This can be useful for keeping an eye on which items are being used and consumed, and give you further targets of investigation for related commands like #3locateitem#0 and #3summonitem#0.

The syntax is #3itemaudit [<filters>]#0.

The filters that can be used are as follows:

	#6*<tag>#0 - show only items with a particular tag
	#6%<zone>#0 - show only items in a particular zone
	#6+<keyword>#0 - show only items with a particular keyword
	#6-<keyword>#0 - show only items without a particular keyword
	#6<component>#0 - show only items with component types as described", AutoHelp.HelpArg)]
	protected static void ItemAudit(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var protos = actor.Gameworld.ItemProtos.ToList();
		var filters = new List<string>();
		IZone zone = null;

		while (!ss.IsFinished)
		{
			var keyword = ss.PopSpeech();
			if (keyword.Length < 2)
			{
				actor.OutputHandler.Send($"The text {keyword.ColourCommand()} is not a valid filter.");
				return;
			}

			if (keyword[0] == '*')
			{
				keyword = keyword.Substring(1);
				var tags = actor.Gameworld.Tags.FindMatchingTags(keyword);
				protos = protos
				             .Where(x => x.Tags.Any(y => tags.Any(z => y.IsA(z))))
				             .ToList();
				filters.Add($"with the {"tag".Pluralise(tags.Count != 1)} {tags.Select(x => x.FullName.ColourName()).ListToString(conjunction: "or ")}");
				continue;
			}

			if (keyword[0] == '+')
			{
				keyword = keyword.Substring(1);
				protos = protos
				         .Where(x => 
					         x.ShortDescription.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
							 x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
							 x.LongDescription?.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) == true ||
							 x.FullDescription.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
					        )
				         .ToList();
				filters.Add($"with the keyword {keyword.ColourCommand()}");
				continue;
			}

			if (keyword[0] == '-')
			{
				keyword = keyword.Substring(1);
				protos = protos
				         .Where(x =>
					         !x.ShortDescription.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) &&
					         !x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) &&
							 !x.LongDescription.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) &&
							 !x.FullDescription.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
				         )
				         .ToList();
				filters.Add($"without the keyword {keyword.ColourCommand()}");
				continue;
			}

			if (keyword[0] == '%')
			{
				keyword = keyword.Substring(1);
				zone = actor.Gameworld.Zones.GetByIdOrName(keyword);
				if (zone is null)
				{
					actor.OutputHandler.Send($"There is no zone identified by the text {keyword.ColourCommand()}.");
					return;
				}

				filters.Add($"in the zone {zone.Name.ColourValue()}");
				continue;
			}

			protos = protos
			               .Where(x => x.Components.Any(
				               y => y.TypeDescription.StartsWith(
					               keyword, StringComparison.InvariantCultureIgnoreCase)))
			               .ToList();
			filters.Add($"with component types starting with the text {keyword.ColourCommand()}");
		}

		var protosHash = protos.Select(x => x.Id).ToHashSet();
		var items = actor.Gameworld.Items
		                 .Where(x => protosHash.Contains(x.Prototype.Id))
		                 .ToList();
		var counterTotal = new Counter<IGameItemProto>();
		var counterWorld = new Counter<IGameItemProto>();
		var counterInventories = new Counter<IGameItemProto>();
		var counterContainers = new Counter<IGameItemProto>();

		foreach (var item in items)
		{
			var location = item.TrueLocations.FirstOrDefault();
			if (zone is not null && location is not null && location.Zone != zone)
			{
				continue;
			}

			counterTotal.Add(item.Prototype, 1);
			if (item.ContainedIn != null)
			{
				counterContainers.Add(item.Prototype, 1);
			}
			else if (item.InInventoryOf != null)
			{
				counterInventories.Add(item.Prototype, 1);
			}
			else
			{
				counterWorld.Add(item.Prototype, 1);
			}
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Audit of all world items".GetLineWithTitle(actor, Telnet.Green, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Filters:");
		sb.AppendLine();
		if (filters.Any())
		{
			foreach (var item in filters)
			{
				sb.AppendLine($"\t{item}");
			}
		}
		else
		{
			sb.AppendLine("\tNo filters");
		}

		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in counterTotal
			orderby item.Value descending
			select new List<string>
			{
				$"{item.Key.Id.ToString("N0", actor)}r{item.Key.RevisionNumber.ToString("N0", actor)}",
				item.Key.ShortDescription.Colour(item.Key.CustomColour ?? Telnet.Green),
				item.Value.ToString("N0", actor),
				counterWorld[item.Key].ToString("N0", actor),
				counterInventories[item.Key].ToString("N0", actor),
				counterContainers[item.Key].ToString("N0", actor),
				(!(item.Key.Status.In(RevisionStatus.Current, RevisionStatus.UnderDesign))).ToColouredString()
			},
			new List<string>
			{
				"Proto",
				"Description",
				"# Total",
				"# World",
				"# Inventories",
				"# Containers",
				"Obsolete"
			},
			actor,
			Telnet.Yellow
		));
		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("LocateItem", "locateitem", "li")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("locateitem", @"The #3locateitem#0 command is used to find where in the world an item or type of item is. You can use this to find particular items, find things allegedly missing or stolen, or just to get a sense of where certain items are.

The syntax is as follows:

	#3locateitem <keyword>#0 - finds all items with the specified keyword(s)
	#3locateitem *<id>#0 - finds all items with the specified item prototype ID
	#3locateitem !<id>#0 - finds a particular item by its ID", AutoHelp.HelpArgOrNoArg)]
	protected static void LocateItem(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"What keyword, item ID (prefixed by !) or item prototype ID (prefixed by *) do you want to search for?");
			return;
		}

		var text = ss.PopSpeech();
		List<IGameItem> items;
		if (text[0] == '*')
		{
			if (!long.TryParse(text.Substring(1), out var value))
			{
				actor.OutputHandler.Send("That is not a valid ID.");
				return;
			}

			items = actor.Gameworld.Items.Where(x => x.Prototype.Id == value).ToList();
		}
		else if (text[0] == '!')
		{
			if (!long.TryParse(text.Substring(1), out var value))
			{
				actor.OutputHandler.Send("That is not a valid ID.");
				return;
			}

			items = actor.Gameworld.Items.Where(x => x.Id == value).ToList();
		}
		else
		{
			var split = text.Split('.');
			// The early ToList() in the next query is necessary because this is one of the few cases in the code where the entire Gameworld.Items is queried as an 
			// IEnumerable and HasKeyword can cause some items to initialise themselves (such as corpses) that modify the list of items loaded into the world
			items = actor.Gameworld.Items.ToList().Where(x => split.All(y => x.HasKeyword(y, actor, true))).ToList();
		}

		if (!items.Any())
		{
			actor.OutputHandler.Send("You don't find any items like that.");
			return;
		}

		items = items.OrderBy(x => x.TrueLocations.FirstOrDefault()?.Id ?? 0).ToList();
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in items
			let location = item.TrueLocations.FirstOrDefault()
			select new List<string>
			{
				item.Id.ToStringN0(actor),
				item.Prototype.IdAndRevisionFor(actor),
				item.HowSeen(actor, flags: PerceiveIgnoreFlags.TrueDescription),
				location?.GetFriendlyReference(actor),
				item.ContainedIn is not null ?
					$"{item.ContainedIn.HowSeen(actor, flags: PerceiveIgnoreFlags.TrueDescription)} (#{item.ContainedIn.Id.ToStringN0(actor)})" :
					item.InInventoryOf is not null ?
						$"{item.InInventoryOf.HowSeen(actor, flags: PerceiveIgnoreFlags.TrueDescription)} (#{item.InInventoryOf.Actor.Id.ToStringN0(actor)})" :
						""
			},
			new List<string>
			{
				"Id",
				"Prototype",
				"Description",
				"Location",
				"Contained In"
			},
			1000,
			true,
			Telnet.Green,
			-1,
			actor.Account.UseUnicode));
	}
}