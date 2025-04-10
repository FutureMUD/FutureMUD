﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Editor;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Email;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.OpenAI;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Logging;
using MudSharp.NPC;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.OpenAI;
using System.Reflection;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Dapper;
using System.Numerics;

namespace MudSharp.Commands.Modules;

/// <summary>
///     Implementor Module is for commands designed to be executed either by the implementor only or used primarily in
///     testing
/// </summary>
public class ImplementorModule : Module<ICharacter>
{
	private ImplementorModule()
		: base("Implementor")
	{
		IsNecessary = true;
	}

	public static ImplementorModule Instance { get; } = new();

	[PlayerCommand("CommandLevels", "commandlevels")]
	[CommandPermission(PermissionLevel.Founder)]
	protected static void CommandLevels(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var minimum = PermissionLevel.Any;
		if (!ss.IsFinished)
		{
			if (!ss.SafeRemainingArgument.TryParseEnum(out minimum))
			{
				actor.OutputHandler.Send(
					$"The valid permission levels are {Enum.GetValues<PermissionLevel>().OrderBy(x => x).Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
				return;
			}
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Commands for permission level {minimum.DescribeEnum().ColourName()} and up:");
		foreach (var group in actor.CommandTree.Commands.TCommands.Values
								   .Where(x => x.PermissionRequired >= minimum).GroupBy(x => x.PermissionRequired)
								   .OrderBy(x => x.Key))
		{
			sb.AppendLine($"\n{group.Key.DescribeEnum().ColourName()}\n");
			sb.AppendLineColumns((uint)actor.LineFormatLength, (uint)actor.LineFormatLength / 30,
				group.Select(x => x.Name).Distinct().OrderBy(x => x).ToArray());
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("CurrencyTest", "currencytest")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	protected static void CurrencyTest(ICharacter actor, string input)
	{
		if (string.IsNullOrEmpty(input.RemoveFirstWord()))
		{
			actor.OutputHandler.Send("Enter a number to convert.");
			return;
		}

		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.Peek().ToLowerInvariant() == "convert")
		{
			ss.Pop();
			var amount = actor.Currency.GetBaseCurrency(ss.SafeRemainingArgument, out var success);
			if (success)
			{
				actor.Send("Equates to {0} base units, and described as follows:", amount);
				foreach (var value in Enum.GetValues(typeof(CurrencyDescriptionPatternType)))
				{
					actor.OutputHandler.Send(
						$"\tFor Type {Enum.GetName(typeof(CurrencyDescriptionPatternType), value).Colour(Telnet.Cyan)} the value is {actor.Currency.Describe(amount, (CurrencyDescriptionPatternType)value).Colour(Telnet.Green)}"
							.NoWrap());
				}
			}
			else
			{
				actor.OutputHandler.Send("Not successful.");
			}
		}
		else
		{
			if (!decimal.TryParse(ss.Pop(), out var amount))
			{
				actor.OutputHandler.Send("You must enter a number.");
				return;
			}

			foreach (var currency in actor.Gameworld.Currencies)
			{
				actor.Send("Showing {0} in the {1} currency.", amount, currency.Name);
				foreach (var value in Enum.GetValues(typeof(CurrencyDescriptionPatternType)))
				{
					actor.OutputHandler.Send(
						$"\tFor Type {Enum.GetName(typeof(CurrencyDescriptionPatternType), value).Colour(Telnet.Cyan)} the value is {currency.Describe(amount, (CurrencyDescriptionPatternType)value).Colour(Telnet.Green)}"
							.NoWrap());
				}

				actor.OutputHandler.Send("");
			}
		}
	}

	[PlayerCommand("TestMXP", "testmxp")]
	[CommandPermission(PermissionLevel.Founder)]
	protected static void Showoff(ICharacter actor, string input)
	{
		var sb = new StringBuilder();
		sb.AppendLine("This line has a little bit of " + "bold text".FluentTagMXP("B") + " as well as something " +
					  "struck-out".FluentTagMXP("S") + ".");
		sb.AppendLine("This line is in a different font family.".FluentTagMXP("FONT", "FACE=\"Times New Roman\""));
		sb.AppendLine("This line has " + "blinking text".FluentTagMXP("COLOR", "FORE=Blink") + " and text with " +
					  "red foreground".FluentTagMXP("COLOR", "FORE=Red") + " and " +
					  "white text on red".FluentTagMXP("COLOR", "FORE=White BACK=Red") + ".");
		sb.AppendLine("This line contains a " +
					  "clickable link to google".FluentTagMXP("A",
						  "HREF=\"http://www.google.com\" hint=\"Hey look, it has a hint!\"") + ".");
		sb.AppendLine();
		sb.AppendLine("Image!  " +
					  MXP.TagMXP("image batman-thumbs-up-o.gif URL=\"http://stream1.gifsoup.com/view3/3414762/\""));
		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("TestGPT", "testgpt")]
	[CommandPermission(PermissionLevel.Founder)]
	protected static void TestGPT(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which GPT Thread shall we test?");
			return;
		}

		using (new FMDB())
		{
			Models.GPTThread thread;
			var cmd = ss.PopSpeech();
			if (long.TryParse(cmd, out var id))
			{
				thread = FMDB.Context.GPTThreads.Find(id);
			}
			else
			{
				thread = FMDB.Context.GPTThreads.Include(x => x.Messages).FirstOrDefault(x => x.Name == cmd);
			}

			if (thread is null)
			{
				actor.OutputHandler.Send("There is no such GPT Thread.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What is your prompt for GPT?");
				return;
			}

			var threadName = thread.Name;
			actor.OutputHandler.Send(
				$"You send the following request to the {thread.Name.ColourName()} GPT thread:\n\n{ss.RemainingArgument}");
			OpenAI.OpenAIHandler.MakeGPTRequest(thread, ss.RemainingArgument, actor, text =>
			{
				actor.OutputHandler.Send($"#B[GPT Response for {threadName}]#0\n\n{text.Wrap(actor.InnerLineFormatLength)}".SubstituteANSIColour());
			});
		}
	}

	[PlayerCommand("TestAnthropic", "testanthropic")]
	[CommandPermission(PermissionLevel.Founder)]
	protected static void TestAnthropic(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which GPT Thread shall we test?");
			return;
		}

		using (new FMDB())
		{
			Models.GPTThread thread;
			var cmd = ss.PopSpeech();
			if (long.TryParse(cmd, out var id))
			{
				thread = FMDB.Context.GPTThreads.Find(id);
			}
			else
			{
				thread = FMDB.Context.GPTThreads.Include(x => x.Messages).FirstOrDefault(x => x.Name == cmd);
			}

			if (thread is null)
			{
				actor.OutputHandler.Send("There is no such GPT Thread.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What is your prompt for GPT?");
				return;
			}

			var threadName = thread.Name;
			actor.OutputHandler.Send(
				$"You send the following request to the {thread.Name.ColourName()} Anthropic thread:\n\n{ss.RemainingArgument}");
			OpenAI.OpenAIHandler.MakeAnthropicRequest(thread.Prompt, ss.SafeRemainingArgument, text =>
			{
				actor.OutputHandler.Send($"#B[Anthropic Response for {threadName}]#0\n\n{text.Wrap(actor.InnerLineFormatLength)}".SubstituteANSIColour());
			}, actor.Gameworld.GetStaticConfiguration("AnthropicDefaultModel"));
		}
	}

	[PlayerCommand("TestGemini", "testgemini")]
	[CommandPermission(PermissionLevel.Founder)]
	protected static void TestGemini(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which GPT Thread shall we test?");
			return;
		}

		using (new FMDB())
		{
			Models.GPTThread thread;
			var cmd = ss.PopSpeech();
			if (long.TryParse(cmd, out var id))
			{
				thread = FMDB.Context.GPTThreads.Find(id);
			}
			else
			{
				thread = FMDB.Context.GPTThreads.Include(x => x.Messages).FirstOrDefault(x => x.Name == cmd);
			}

			if (thread is null)
			{
				actor.OutputHandler.Send("There is no such GPT Thread.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What is your prompt for GPT?");
				return;
			}

			var threadName = thread.Name;
			actor.OutputHandler.Send(
				$"You send the following request to the {thread.Name.ColourName()} Gemini thread:\n\n{ss.RemainingArgument}");
			OpenAI.OpenAIHandler.MakeGeminiRequest(thread.Prompt, ss.SafeRemainingArgument, text =>
			{
				actor.OutputHandler.Send($"#B[Gemini Response for {threadName}]#0\n\n{text.Wrap(actor.InnerLineFormatLength)}".SubstituteANSIColour());
			}, actor.Gameworld.GetStaticConfiguration("GeminiDefaultModel"));
		}
	}

	[PlayerCommand("TestUnicode", "testunicode")]
	[CommandPermission(PermissionLevel.Founder)]
	protected static void Unicode(ICharacter actor, string input)
	{
		actor.OutputHandler.Send("Здравствуйте, комраде! Я скажу в юникоде.");
		actor.OutputHandler.Send("ส็็็็็็็็็็็็็็็็็็็็็็็็็༼ ຈل͜ຈ༽ส้้้้้้้้้้้้้้้้้้้้้้้");
		actor.OutputHandler.Send("(╯°□°）╯︵ ┻━┻");
	}
	
	protected static void Debug_Flare(ICharacter actor)
	{
		actor.Location.Zone.AddEffect(new FlareEffect(actor.Location.Zone, 1000,
			"A bright white flare burns in the sky.", Telnet.BoldWhite,
			"The flare in the sky burns out and fades to nothing."));
		actor.OutputHandler.Send("Added a flare.");
	}

	[PlayerCommand("ImpDebug", "impdebug")]
	[HelpInfo("impdebug",
		@"This command runs various routines that should generally only be done by an implementor in the course of debugging and/or maintenance. There are the following routines to choose from:

	#3cleanupitems#0 - deletes all 'orphaned' items that have become disconnected from the game world
	#3sun <sun> <minutes>#0 - adds the specified number of minutes to a sun
	#3save#0 - shows debug info about the save queue
	#3character <id>#0 - shows debug info about a character
	#3duplication#0 - checks for duplicated items and characters
	#3reloademail#0 - reloads the email client
	#3reloadstatics#0 - reloads static strings and configs. Some things may not work until a reboot
	#3reloadchargen#0 - reloads the chargen definition
	#3celestials#0 - runs all the celestial objects through 1 year and tracks their position, writing out to file. Do not do this on the live server.
	#3guests <number> <template>#0 - generates the specified number of guest avatars from the template
	#3string <which>#0 - shows the value of a static string
	#3config <which>#0 - shows the value of a static config
	#3dream <who>#0 - gives a dream to the specified PC
	#3combatspeed <multiplier>#0 - sets a global multiplier to combat speed
	#3progfunctions#0 - writes out all the prog function help to a file (saved to disk)
	#3cleanupcorpses#0 - deletes all NPC corpses of skeletal decay level. Loads up all PCs to check their inventories
	#3descriptions short|full [<person>]#0 - writes all valid descriptors for that person to a file on disk.
	#3flare#0 - sends a flare up into the sky.
	#3time#0 - shows the current time where you are
	#3addtime <timespan>#0 - adds the specified amount of time to all in-game clocks
	#3seedrooms#0 - loads the 10 rooms with the highest IDs into the new room queue
	#3failemail#0 - tests the fail email routine
	#3crash#0 - causes the MUD to crash
	#3heartbeat hour|minute|second|5second|10second|30second#0 - manually triggers a heartbeat
	#3freezetime#0 - freezes all in game clocks
	#3unfreezetime#0 - resumes all in game clocks", AutoHelp.HelpArgOrNoArg)]
	[CommandPermission(PermissionLevel.Founder)]
	protected static void ImpDebug(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopSpeech().CollapseString().ToLowerInvariant())
		{
			case "freezetime":
				DebugFreezeTime(actor);
				return;
			case "unfreezetime":
				DebugUnfreezeTime(actor);
				return;
			case "exportcrafts":
				DebugExportCrafts(actor, ss);
				return;
			case "testcover":
				DebugTestCover(actor, ss);
				return;
			case "heartbeat":
				Debug_Heartbeat(actor, ss);
				return;
			case "sql":
				DebugSql(actor, ss);
				return;
			case "crash":
				Debug_Crash(actor);
				return;
			case "failemail":
				Debug_FailEmail(actor);
				return;
			case "seedrooms":
				Debug_SeedRooms(actor);
				return;
			case "addtime":
				Debug_AddTime(actor, ss);
				return;
			case "flare":
				Debug_Flare(actor);
				return;
			case "cleanupthedead":
			case "cleanuporphans":
			case "cleanupitems":
				// DebugCleanupOrphanedItems(actor);
				return;
			case "sun":
				DebugSun(actor, ss);
				return;
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
			case "duplication":
				DebugDuplication(actor, ss);
				return;
			case "reloademail":
				EmailHelper.SetupEmailClient();
				actor.Send("Mail client reset.");
				return;
			case "reloadstatics":
				using (new FMDB())
				{
					(actor.Gameworld as IFuturemudLoader).LoadStaticValues();
				}

				actor.Send("Static strings and settings reloaded.");
				return;
			case "reloadchargen":
				(actor.Gameworld as IFuturemudLoader).LoadChargen();
				actor.Send("Chargen reloaded.");
				return;
			case "celestials":
				DebugCelestials(actor);
				return;
			case "guests":
				DebugGuests(actor, ss);
				return;
			
			case "characteristics":
				DebugCharacteristics(actor, ss);
				return;
			case "string":
				DebugString(actor, ss);
				return;
			case "config":
				DebugStatic(actor, ss);
				return;
			case "dream":
				DebugDream(actor, ss);
				return;
			case "combatspeed":
				DebugCombatSpeed(actor, ss);
				return;
			case "time":
				var datetime = new MudDateTime(actor.Location.Date(actor.Location.Calendars.First()),
					actor.Location.Time(actor.Location.Clocks.First()),
					actor.Location.Room.Zone.GetEditableZone.TimeZones[actor.Location.Clocks.First()]);
				actor.Send("Current Datetime: {0}", datetime.GetDateTimeString());
				break;
			case "descriptions":
				DebugDescriptions(actor, ss);
				return;
			case "progfunctions":
				DebugProgFunctions(actor);
				return;
			case "cleanupcorpses":
				DebugCleanupCorpses(actor);
				return;
			case "backup":
				DebugBackup(actor);
				return;
			case "orphans":
				DebugOrphans(actor);
				return;
			default:
				actor.Send("That's not a known debug routine.");
				return;
		}
	}

	private static void DebugUnfreezeTime(ICharacter actor)
	{
		actor.Gameworld.ClockManager.UnfreezeTime();
		actor.OutputHandler.Send("You unfreeze all in-game time progression.");
	}

	private static void DebugFreezeTime(ICharacter actor)
	{
		actor.Gameworld.ClockManager.FreezeTime();
		actor.OutputHandler.Send("You freeze all in-game time progression.");
	}

	private static void DebugOrphans(ICharacter actor)
	{
		var sb = new StringBuilder();
		actor.OutputHandler.Send("Show items that are potentially orphans...");
		actor.Gameworld.ForceOutgoingMessages();
		var loadedPCs = EnsureAllPCsAreLoaded(actor.Gameworld);
		actor.OutputHandler.Send("Ensuring that all items are loaded...");
		actor.Gameworld.ForceOutgoingMessages();
		var ids = new List<long>();
		using (new FMDB())
		{
			ids.AddRange(FMDB.Context.GameItems.Select(x => x.Id));
		}

		var items = new List<IGameItem>(ids.Count);
		foreach (var id in ids)
		{
			var item = actor.Gameworld.TryGetItem(id, true);
			if (item is null)
			{
				continue;
			}

			items.Add(item);
		}

		actor.OutputHandler.Send("Done loading items...\nPre-calculating critical item types...");
		actor.Gameworld.ForceOutgoingMessages();

		var crafts = items.SelectNotNull(x => x.GetItemType<IActiveCraftGameItemComponent>()).ToList();
		var ranged = items.SelectNotNull(x => x.GetItemType<IRangedWeapon>()).ToList();
		var wounds = items.SelectMany(x => x.Wounds).Concat(actor.Gameworld.Characters.SelectMany(x => x.Wounds)).Where(x => x.Lodged is not null).ToList();

		actor.OutputHandler.Send("Done pre-calculating...\nLooking for ophans...");
		actor.Gameworld.ForceOutgoingMessages();

		foreach (var item in items)
		{
			if (item.Location is not null)
			{
				continue;
			}

			if (item.ContainedIn is not null)
			{
				continue;
			}

			if (item.InInventoryOf is not null)
			{
				continue;
			}

			if (crafts.Any(x => x.GameItemIsPartOfCraft(item)))
			{
				continue;
			}

			if (actor.Gameworld.Properties.Any(x => x.PropertyKeys.Any(y => y.GameItem == item)))
			{
				continue;
			}

			if (item.GetItemType<IBeltable>() is { } beltable)
			{
				if (beltable.ConnectedTo is not null)
				{
					continue;
				}
			}

			if (item.GetItemType<IDoor>() is { } door)
			{
				if (door.InstalledExit is not null)
				{
					continue;
				}
			}

			if (item.GetItemType<IImplant>() is { } implant)
			{
				if (implant.InstalledBody is not null)
				{
					continue;
				}
			}

			if (ranged.Any(x => x.AllContainedItems.Any(y => y == item)))
			{
				continue;
			}

			if (wounds.Any(x => x.Lodged == item))
			{
				continue;
			}

			sb.AppendLine($"Item #{item.Id} ({item.HowSeen(actor, flags: PerceiveIgnoreFlags.TrueDescription)} [proto {item.Prototype.IdAndRevisionFor(actor)}] was potentially orphaned.");
		}

		actor.OutputHandler.Send(sb.ToString());
		CleanupAllPCsLoaded(actor.Gameworld, loadedPCs);
	}

	private static void DebugExportCrafts(ICharacter actor, StringStack ss)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Id,Revision,Name,Category,Blurb,Status,Action,Item SDesc,Appear Prog,CanUse Prog,WhyCantUse Prog,OnStart Prog,OnCancel Prog,OnFinishProg,Trait,Difficulty,Threshold,FreeChecks,FailPhase,Interruptable,PhaseLength1,PhaseLength2,PhaseLength3,PhaseLength4,PhaseLength5,PhaseLength6,PhaseLength7,PhaseLength8,PhaseLength9,PhaseLength10,PhaseEcho1,PhaseEcho2,PhaseEcho3,PhaseEcho4,PhaseEcho5,PhaseEcho6,PhaseEcho7,PhaseEcho8,PhaseEcho9,PhaseEcho10,PhaseFailEcho1,PhaseFailEcho2,PhaseFailEcho3,PhaseFailEcho4,PhaseFailEcho5,PhaseFailEcho6,PhaseFailEcho7,PhaseFailEcho8,PhaseFailEcho9,PhaseFailEcho10,Input1,Input2,Input3,Input4,Input5,Input6,Input7,Input8,Input9,Input10,Tool1,Tool2,Tool3,Tool4,Tool5,Tool6,Tool7,Tool8,Tool9,Tool10,Product1,Product2,Product3,Product4,Product5,Product6,Product7,Product8,Product9,Product10,FailProduct1,FailProduct2,FailProduct3,FailProduct4,FailProduct5,FailProduct6,FailProduct7,FailProduct8,FailProduct9,FailProduct10");
		foreach (var craft in actor.Gameworld.Crafts)
		{
			sb.Append($"{craft.Id},{craft.RevisionNumber},\"{craft.Name}\",\"{craft.Category}\",\"{craft.Blurb}\",{craft.Status.DescribeEnum()},\"{craft.ActionDescription}\",\"{craft.ActiveCraftItemSDesc}\",");
			sb.Append($"{craft.AppearInCraftsListProg?.Name ?? ""},{craft.CanUseProg?.Name ?? ""},{craft.WhyCannotUseProg?.Name ?? ""},{craft.OnUseProgStart?.Name ?? ""},{craft.OnUseProgCancel?.Name ?? ""},{craft.OnUseProgComplete?.Name ?? ""},");
			sb.Append($"{craft.CheckTrait?.Name ?? ""},{craft.CheckDifficulty.DescribeEnum()},{craft.FailThreshold.DescribeEnum()},{craft.FreeSkillChecks},{craft.FailPhase},{craft.Interruptable}");
			var lengths = craft.PhaseLengths.ToList();
			for (var i = 0; i < 10; i++)
			{
				if (i < lengths.Count)
				{
					sb.Append($",{lengths[i].TotalSeconds}");
					continue;
				}

				sb.Append(",");
			}

			var echoes = craft.PhaseEchoes.ToList();
			for (var i = 0; i < 10; i++)
			{
				if (i < echoes.Count)
				{
					sb.Append($",\"{echoes[i]}\"");
					continue;
				}

				sb.Append(",");
			}

			var failEchoes = craft.FailPhaseEchoes.ToList();
			for (var i = 0; i < 10; i++)
			{
				if (i < failEchoes.Count)
				{
					sb.Append($",\"{failEchoes[i]}\"");
					continue;
				}

				sb.Append(",");
			}

			var inputs = craft.Inputs.ToList();
			for (var i = 0; i < 10; i++)
			{
				if (i < inputs.Count)
				{
					var input = inputs[i];
					sb.Append($",\"{input.InputType} - {input.HowSeen(actor).StripANSIColour()}\"");
					continue;
				}

				sb.Append(",");
			}

			var tools = craft.Tools.ToList();
			for (var i = 0; i < 10; i++)
			{
				if (i < tools.Count)
				{
					var tool = tools[i];
					sb.Append($",\"{tool.ToolType} - {tool.DesiredState.DescribeEnum()} - {tool.HowSeen(actor).StripANSIColour()}\"");
					continue;
				}

				sb.Append(",");
			}

			var products = craft.Products.ToList();
			for (var i = 0; i < 10; i++)
			{
				if (i < products.Count)
				{
					var product = products[i];
					sb.Append($",\"{product.ProductType} - {product.HowSeen(actor).StripANSIColour()}\"");
					continue;
				}

				sb.Append(",");
			}

			var failProducts = craft.FailProducts.ToList();
			for (var i = 0; i < 10; i++)
			{
				if (i < failProducts.Count)
				{
					var product = failProducts[i];
					sb.Append($",\"{product.ProductType} - {product.HowSeen(actor).StripANSIColour()}\"");
					continue;
				}

				sb.Append(",");
			}

			sb.AppendLine();
		}

		using var fs = new FileStream($"CraftsExport{DateTime.UtcNow.ToFileTimeUtc()}.csv", FileMode.Create);
		using var writer = new StreamWriter(fs);
		writer.Write(sb);
		writer.Flush();
		writer.Close();
		actor.OutputHandler.Send($"Successfully exported crafts list to {fs.Name.ColourCommand()}");
		fs.Close();
	}

	private static void DebugTestCover(ICharacter actor, StringStack ss)
	{
		var covers = actor.Body.GetAllItemsCoverStatus(true);
		foreach (var item in actor.Body.WornItems)
		{
			var cover = actor.Body.CoverInformation(item).ToList();
			if (cover.All(x => x.Item1 == WearableItemCoverStatus.Covered))
			{
				if (covers[item] != WearableItemCoverStatus.Covered)
				{
					actor.OutputHandler.Send($"The item {item.HowSeen(actor)} was {covers[item].DescribeEnum()} instead of covered.");
				}
				continue;
			}

			if (cover.All(x => x.Item1 == WearableItemCoverStatus.Uncovered))
			{
				if (covers[item] != WearableItemCoverStatus.Uncovered)
				{
					actor.OutputHandler.Send($"The item {item.HowSeen(actor)} was {covers[item].DescribeEnum()} instead of uncovered.");
				}

				continue;
			}

			if (cover.All(x => x.Item1 != WearableItemCoverStatus.Uncovered))
			{
				if (covers[item] != WearableItemCoverStatus.TransparentlyCovered)
				{
					actor.OutputHandler.Send($"The item {item.HowSeen(actor)} was {covers[item].DescribeEnum()} instead of partially covered.");
				}

				continue;
			}
		}

		actor.OutputHandler.Send("Done");
	}

	private static void DebugBackup(ICharacter actor)
	{
		try
		{
			var root = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var toPath = System.IO.Path.GetFullPath(string.IsNullOrEmpty(root) ? "Backups" : System.IO.Path.Combine(root, "Backups"));
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

			var fileName = $"DB-Backup-{DateTime.UtcNow:yyyMMddhhmmss}.sql";
			var destination = System.IO.Path.GetFullPath(fileName, toPath);

			using (var conn = new MySqlConnection(FMDB.ConnectionString))
			{
				using (var cmd = new MySqlCommand())
				{
					using (var mb = new MySqlBackup(cmd))
					{
						cmd.Connection = conn;
						conn.Open();
						mb.ExportInfo.AddCreateDatabase = true;
						mb.ExportInfo.AddDropDatabase = true;
						mb.ExportToFile(destination);
						conn.Close();
					}
				}
			}

			actor.OutputHandler.Send($"Successfully created file {destination.Colour(Telnet.BoldGreen)}");
		}
		catch (System.Exception e)
		{
			actor.OutputHandler.Send($"Backup failed. Reason below.\n\n{e}");
		}
	}

	private static void DebugSql(ICharacter actor, StringStack ss)
	{
		using (new FMDB())
		{
			FMDB.Connection.Execute(ss.RemainingArgument);
		}

		actor.OutputHandler.Send($@"You execute the following SQL command:

{ss.RemainingArgument.ColourCommand()}");
	}

	private static void Debug_Heartbeat(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "hour":
				actor.OutputHandler.Send("Manually triggering hour heartbeat...");
				actor.Gameworld.HeartbeatManager.ManuallyFireHeartbeatHour();
				return;
			case "minute":
				actor.OutputHandler.Send("Manually triggering 1 minute heartbeat...");
				actor.Gameworld.HeartbeatManager.ManuallyFireHeartbeatMinute();
				return;
			case "second":
				actor.OutputHandler.Send("Manually triggering 1 second heartbeat...");
				actor.Gameworld.HeartbeatManager.ManuallyFireHeartbeatSecond();
				return;
			case "5second":
				actor.OutputHandler.Send("Manually triggering 5 second heartbeat...");
				actor.Gameworld.HeartbeatManager.ManuallyFireHeartbeat5Second();
				return;
			case "10second":
				actor.OutputHandler.Send("Manually triggering 10 second heartbeat...");
				actor.Gameworld.HeartbeatManager.ManuallyFireHeartbeat10Second();
				return;
			case "30second":
				actor.OutputHandler.Send("Manually triggering 30 second heartbeat...");
				actor.Gameworld.HeartbeatManager.ManuallyFireHeartbeat30Second();
				return;
		}

		actor.OutputHandler.Send("The valid values are #3hour#0, #3minute#0, #3second#0, #35second#0, #310second#0, and #330second#0.".SubstituteANSIColour());
	}


	private static void Debug_Crash(ICharacter actor)
	{
		throw new ApplicationException($"{actor.PersonalName.GetName(NameStyle.FullName)} intentionally crashed the game using IMPDEBUG CRASH.");
	}

	private static void Debug_FailEmail(ICharacter actor)
	{
		EmailHelper.Instance.TestFailSendEmail();
		actor.OutputHandler.Send("Fail email tried.");
	}

	private static void Debug_SeedRooms(ICharacter actor)
	{
		RoomBuilderModule.BuiltCells.AddRange(actor.Gameworld.Cells.OrderByDescending(x => x.Id).Take(10).Reverse());
		actor.OutputHandler.Send(
			$"You add the 10 rooms with the highest ID to the 'new room' queue for GOTO/Room building.");
	}

	private static void Debug_AddTime(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"How much time do you want to add to all in-game clocks? Generally speaking the format is days:hours:minutes:seconds and it matches the way your account's cultureinfo handles timespans.");
			return;
		}

		if (!TimeSpan.TryParse(ss.SafeRemainingArgument, out var timespan))
		{
			actor.OutputHandler.Send(
				"That is not a valid timespan. Generally speaking the format is days:hours:minutes:seconds and it matches the way your account's cultureinfo handles timespans.");
			return;
		}

		var oldTime = actor.Location.DateTime().ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short);
		foreach (var clock in actor.Gameworld.Clocks)
		{
			clock.CurrentTime.AddSeconds(timespan.Seconds);
			clock.CurrentTime.AddMinutes(timespan.Minutes);
			clock.CurrentTime.AddHours(timespan.Hours);
			clock.AdvanceDays(timespan.Days);
		}

		actor.OutputHandler.Send(
			$"Advanced all clocks by {timespan.Describe().ColourValue()}.\nOld time was {oldTime.ColourValue()} and new time is {actor.Location.DateTime().ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
	}

	
	private static void DebugCleanupOrphanedItems(ICharacter actor)
	{
		// WARNING - this command is making some serious false positives, such as items stored in crafts
		// Do not reenable without some serious work

		void DoCleanupOrphanedItems()
		{
			// Firstly, flush the save manager in case there is anything pending
			actor.Gameworld.SystemMessage("Flushing the save manager...", true);
			actor.Gameworld.SaveManager.Flush();
			// Make sure that the messaging thread gets a chance to resume and send out echoes
			actor.Gameworld.ForceOutgoingMessages();
			Thread.Sleep(100);

			// Next, let's load up all the offline but alive PCs so that their inventory is counted.
			var loadedPCs = new List<ICharacter>();
			var onlinePCIDs = actor.Gameworld.Characters.Select(x => x.Id).ToList();
			actor.Gameworld.SystemMessage("Loading offline PCs so their inventory is accounted for...", true);
			using (new FMDB())
			{
				var PCsToLoad =
					FMDB.Context.Characters.Where(
							x => !x.NpcsCharacter.Any() && x.Guest == null && !onlinePCIDs.Contains(x.Id) &&
								 (x.Status == (int)CharacterStatus.Active ||
								  x.Status == (int)CharacterStatus.Suspended))
						.OrderBy(x => x.Id);
				var i = 0;
				while (true)
				{
					var any = false;
					foreach (var pc in PCsToLoad.Skip(i++ * 10).Take(10).ToList())
					{
						any = true;
						var character = actor.Gameworld.TryGetCharacter(pc.Id, true);
						character.Register(new NonPlayerOutputHandler());
						loadedPCs.Add(character);
						actor.Gameworld.Add(character, false);
					}

					if (!any)
					{
						break;
					}
				}
			}

			actor.Gameworld.SystemMessage($"Loaded {loadedPCs.Count} offline PCs", true);
			// Make sure that the messaging thread gets a chance to resume and send out echoes
			actor.Gameworld.ForceOutgoingMessages();
			Thread.Sleep(100);

			// Next, we force all corpses to load their characters, so their inventories are accounted for
			actor.Gameworld.SystemMessage("Forcing all corpses to load their characters...", true);
			var corpsePCs = new HashSet<ICharacter>();
			foreach (var item in actor.Gameworld.Items.SelectNotNull(x => x.GetItemType<ICorpse>()).ToList())
			{
				corpsePCs.Add(item.OriginalCharacter);
			}

			actor.Gameworld.SystemMessage($"Loaded {corpsePCs.Count} corpses", true);
			// Make sure that the messaging thread gets a chance to resume and send out echoes
			actor.Gameworld.ForceOutgoingMessages();
			Thread.Sleep(100);

			// Next, we make sure all exits have been loaded so we can consider their doors
			actor.Gameworld.ExitManager.PreloadCriticalExits();

			// Next, we work out what items aren't registered in the game world. As all living PCs and NPCs should be loaded, their inventories should be in game. Also, dead but resurrectable characters on corpses would also have registered their items in game.
			actor.Gameworld.SystemMessage("Identifying orphaned items...", true);
			var loadedItemIDs = actor.Gameworld.Items.Select(x => x.Id).ToList();
			using (new FMDB())
			{
				FMDB.Context.GameItems.RemoveRange(FMDB.Context.GameItems.Where(x => !loadedItemIDs.Contains(x.Id)));
				actor.Gameworld.SystemMessage($"Found {loadedItemIDs.Count} items, deleting...", true);
				// Make sure that the messaging thread gets a chance to resume and send out echoes
				actor.Gameworld.ForceOutgoingMessages();
				Thread.Sleep(100);
				FMDB.Context.SaveChanges();
				actor.Gameworld.SystemMessage("Successfully removed all orphaned items...", true);
			}

			foreach (var character in loadedPCs)
			{
				character.Quit(true);
			}
		}

		actor.Send(
			"This can take a long time to complete. Are you sure you wish to do this now? Type ACCEPT to begin.");
		actor.AddEffect(new Accept(actor, new GenericProposal(
			text =>
			{
				actor.Gameworld.SystemMessage(
					"A maintenance task is due to begin in 30 seconds that may run for a long time, and cause the game to appear to hang.");
				actor.Gameworld.Scheduler.AddSchedule(new Schedule(() =>
				{
					actor.Gameworld.SystemMessage("The long-running maintenance task has begun.");
					actor.Gameworld.SaveManager.Flush();
					DoCleanupOrphanedItems();
					actor.Gameworld.SystemMessage("The long-running maintenance task has now completed.");
				}, ScheduleType.System,
#if DEBUG
					TimeSpan.FromSeconds(1),
#else
					TimeSpan.FromSeconds(30), 
#endif
					 "Cleaning Orphaned Items"));
			},
			text => { actor.Send("You decide against cleaning up orphaned items."); },
			() => { actor.Send("You decide against cleaning up orphaned items."); },
			"Cleaning up orphaned items",
			"cleanup",
			"items"
		)), TimeSpan.FromSeconds(120));
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

	private static void DebugDuplication(ICharacter actor, StringStack ss)
	{
		new Framework.Monitoring.DuplicationMonitor(actor.Gameworld).AuditCharacters();
		actor.Send("Done");
	}

	private static void DebugProgFunctions(ICharacter actor)
	{
		var infos = FutureProg.FutureProg.GetFunctionCompilerInformations().ToList();
		WriteProgParametersByCategory(actor.Gameworld, infos);
		WriteProgParametersAlphabetically(actor.Gameworld, infos);
		WriteTypeHelps(actor.Gameworld);
		WriteCollectionHelps(actor.Gameworld);
		actor.Send("Done.");
	}

	public static void WriteCollectionHelps(IFuturemud gameworld)
	{
		using var html = new StreamWriter("ProgCollectionHelps.html");
		html.WriteLine("<html>");
		html.WriteLine(@"<head><style>
table, th, td {
  border: 1px solid black;
  text-align: left;
  padding: 2px;
}
p{
	display: inline-block;
	white-space: pre;
}
span.var-span {
  color: blue;
}
span.func-span{
  color: teal
}
div.function-box{
  border-bottom: 1px solid black;
  padding: 10px;
}
h3{
  color: sienna;
}
div.function-generalhelp {
  color: darkred;
}
</style></head>");
		html.WriteLine(@"<body>
<h1>FutureMUD Collection Extension Reference</h1>
<p>These functions are accessed by doing something in the following form after a collection variable:

	CollectionVariable.FunctionName(ItemVariableName, InnerFunction)

Where:
	
	CollectionVariable is any variable or function returning a collection
	FunctionName is the specific collection extension function you want to run (e.g. Any, Sum, etc)
	ItemVariableName is a variable name that will be used inside the inner function to refer to each item in the collection
	InnerFunction is a function (usually returning a Boolean or Number) that is run on each element in the collection

For example, if you had a Number Collection called fibonacci that contained the following items:

	1, 1, 2, 3, 5, 8, 13

You could run:

	@fibonacci.Sum(number, @number)

And the result would be a number with the value of 33.</p>");

		foreach (var compiler in CollectionExtensionFunction.FunctionCompilerInformations
															.OrderBy(x => x.FunctionName))
		{
			html.WriteLine($"<details><summary>{compiler.FunctionName.ToUpperInvariant()}</summary><p>{compiler.FunctionHelp}</p></details>");
		}

		html.WriteLine("</body></html>");
		html.Flush();
		html.Close();
	}

	public static void WriteTypeHelps(IFuturemud gameworld)
	{
		using var html = new StreamWriter("ProgTypeHelps.html");
		html.WriteLine("<html>");
		html.WriteLine(@"<head><style>
table, th, td {
  border: 1px solid black;
  text-align: left;
  padding: 2px;
}
span.var-span {
  color: blue;
}
span.func-span{
  color: teal
}
div.function-box{
  border-bottom: 1px solid black;
  padding: 10px;
}
h3{
  color: sienna;
}
div.function-generalhelp {
  color: darkred;
}
</style></head>");
		html.WriteLine("<body>\n<h1>FutureMUD Type Help Reference</h1>");

		foreach (var type in ProgVariableTypes.Anything
													.GetAllFlags()
													.OrderBy(x => x is ProgVariableTypes.Collection or ProgVariableTypes.CollectionDictionary or ProgVariableTypes.Dictionary)
													.ThenBy(x => x.Describe())
													.ToList())
		{
			var info = ProgVariable.DotReferenceCompileInfos.GetValueOrDefault(type, null);
			if (info is null)
			{
				continue;
			}

			html.WriteLine($"<details><summary>{type.DescribeEnum()}</summary>");
			html.WriteLine("      <div class=\"function-box\"><table><tr><th>Property</th><th>Return Type</th><th>Property Help</th></tr>");
			foreach (var item in info.PropertyTypeMap)
			{
				html.WriteLine($"		<tr><td>{item.Key}</td><td>{item.Value.Describe()}</td><td>{info.PropertyHelpInfo.GetValueOrDefault(item.Key, "")}</td></tr>");
			}
			html.WriteLine("</table></div>");
			html.WriteLine("</details>");
		}

		html.WriteLine("</body></html>");
		html.Flush();
		html.Close();
	}

	public static void WriteProgParametersAlphabetically(IFuturemud gameworld,
		IEnumerable<FunctionCompilerInformation> infos)
	{
		using var html = new StreamWriter("ProgFunctionsAlphabetically.html");
		html.WriteLine("<html>");
		html.WriteLine(@"<head><style>
table, th, td {
  border: 1px solid black;
  text-align: left;
  padding: 2px;
}
span.var-span {
  color: blue;
}
span.func-span{
  color: teal
}
div.function-box{
  border-bottom: 1px solid black;
  padding: 10px;
}
h3{
  color: sienna;
}
div.function-generalhelp {
  color: darkred;
}
</style></head>");
		html.WriteLine("<body>\n<h1>FutureMUD Function Reference</h1>");
		foreach (var function in infos.OrderBy(x => x.FunctionName))
		{
			html.WriteLine("  <div class=\"function-box\">");
			html.WriteLine($"    <h3>{function.FunctionName.ToUpperInvariant()}</h3>");
			List<string> parameterNames;
			if (function.ParameterNames != null)
			{
				parameterNames = function.ParameterNames.ToList();
			}
			else
			{
				var list = new List<string>();
				foreach (var item in function.Parameters)
				{
					list.Add(list.NameOrAppendNumberToName(item.Describe().ToLowerInvariant()
															   .IncrementNumberOrAddNumber()));
				}

				parameterNames = list;
			}

			ProgVariableTypes returnType;
			if (function.ReturnType != ProgVariableTypes.Error)
			{
				returnType = function.ReturnType;
			}
			else
			{
				var compiled = function.CompilerFunction(new List<IFunction>(), gameworld);
				returnType = compiled.ReturnType;
			}

			html.Write("    <div class=\"function-generalform\">");
			html.Write(
				$"<span class=\"var-span\">{returnType.Describe().ToLowerInvariant()}</span> <span class=\"func-span\">{function.FunctionName.ToLowerInvariant()}</span>(");
			var parameterCount = parameterNames.Count();
			for (var i = 0; i < parameterCount; i++)
			{
				if (i >= 1)
				{
					html.Write(", ");
				}

				html.Write(
					$"<span class=\"var-span\">{function.Parameters.ElementAt(i).Describe().ToLowerInvariant()}</span> {parameterNames.ElementAt(i).ToLowerInvariant()}");
			}

			html.WriteLine(")</div>");

			IEnumerable<string> parameterHelp;
			if (function.ParameterHelp != null)
			{
				parameterHelp = function.ParameterHelp;
			}
			else
			{
				var list = new List<string>();
				foreach (var item in function.Parameters)
				{
					list.Add("This parameter has no help information.");
				}

				parameterHelp = list;
			}

			html.WriteLine("    <div class=\"function-parameters\">");
			html.WriteLine(
				"      <table><tr><th>Parameter</th><th>Variable Type</th><th>Parameter Help</th></tr>");
			for (var i = 0; i < parameterCount; i++)
			{
				html.WriteLine(
					$"      <tr><td>{parameterNames.ElementAt(i).ToLowerInvariant()}</td><td>{function.Parameters.ElementAt(i).Describe().ToLowerInvariant()}</td><td>{parameterHelp.ElementAt(i).ToLowerInvariant()}</td></tr>");
			}

			html.WriteLine("    </table></div>");

			html.WriteLine("      <div class=\"function-generalhelp\">");
			html.WriteLine(
				$"        <p>{function.FunctionHelp ?? "This function has no general help information."}</p>");
			html.WriteLine("      </div>");

			html.WriteLine("    </div>");
		}

		html.WriteLine("</body></html>");
		html.Flush();
		html.Close();
	}

	public static void WriteProgParametersByCategory(IFuturemud gameworld, IEnumerable<FunctionCompilerInformation> infos)
	{
		using var html = new StreamWriter("ProgFunctionsByCategory.html");
		html.WriteLine("<html>");
		html.WriteLine(@"<head><style>
table, th, td {
  border: 1px solid black;
  text-align: left;
  padding: 2px;
}
span.var-span {
  color: blue;
}
span.func-span{
  color: teal
}
div.function-box{
  border-bottom: 1px solid black;
  padding: 10px;
}
h3{
  color: sienna;
}
div.function-generalhelp {
  color: darkred;
}
</style></head>");
		html.WriteLine("<body>\n<h1>FutureMUD Function Reference</h1>");
		foreach (var group in infos.GroupBy(x => x.Category).OrderBy(x => x.Key))
		{
			html.WriteLine(
				$"<details><summary>{group.Key.TitleCase()}</summary>");
			foreach (var function in group.OrderBy(x => x.FunctionName))
			{
				html.WriteLine("  <div class=\"function-box\">");
				html.WriteLine($"    <h3>{function.FunctionName.ToUpperInvariant()}</h3>");
				List<string> parameterNames;
				if (function.ParameterNames != null)
				{
					parameterNames = function.ParameterNames.ToList();
				}
				else
				{
					var list = new List<string>();
					foreach (var item in function.Parameters)
					{
						list.Add(list.NameOrAppendNumberToName(item.Describe().ToLowerInvariant()
																   .IncrementNumberOrAddNumber()));
					}

					parameterNames = list;
				}

				ProgVariableTypes returnType;
				if (function.ReturnType != ProgVariableTypes.Error)
				{
					returnType = function.ReturnType;
				}
				else
				{
					var compiled = function.CompilerFunction(new List<IFunction>(), gameworld);
					returnType = compiled.ReturnType;
				}

				html.Write("    <div class=\"function-generalform\">");
				html.Write(
					$"<span class=\"var-span\">{returnType.Describe().ToLowerInvariant()}</span> <span class=\"func-span\">{function.FunctionName.ToLowerInvariant()}</span>(");
				var parameterCount = parameterNames.Count();
				for (var i = 0; i < parameterCount; i++)
				{
					if (i >= 1)
					{
						html.Write(", ");
					}

					html.Write(
						$"<span class=\"var-span\">{function.Parameters.ElementAt(i).Describe().ToLowerInvariant()}</span> {parameterNames.ElementAt(i).ToLowerInvariant()}");
				}

				html.WriteLine(")</div>");

				IEnumerable<string> parameterHelp;
				if (function.ParameterHelp != null)
				{
					parameterHelp = function.ParameterHelp;
				}
				else
				{
					var list = new List<string>();
					foreach (var item in function.Parameters)
					{
						list.Add("This parameter has no help information.");
					}

					parameterHelp = list;
				}

				html.WriteLine("    <div class=\"function-parameters\">");
				html.WriteLine(
					"      <table><tr><th>Parameter</th><th>Variable Type</th><th>Parameter Help</th></tr>");
				for (var i = 0; i < parameterCount; i++)
				{
					html.WriteLine(
						$"      <tr><td>{parameterNames.ElementAt(i).ToLowerInvariant()}</td><td>{function.Parameters.ElementAt(i).Describe().ToLowerInvariant()}</td><td>{parameterHelp.ElementAt(i).ToLowerInvariant()}</td></tr>");
				}

				html.WriteLine("    </table></div>");

				html.WriteLine("      <div class=\"function-generalhelp\">");
				html.WriteLine(
					$"        <p>{function.FunctionHelp ?? "This function has no general help information."}</p>");
				html.WriteLine("      </div>");

				html.WriteLine("    </div>");
			}

			html.WriteLine("</details>");
		}

		html.WriteLine("</body></html>");
		html.Flush();
		html.Close();
	}

	private static void DebugDescriptions(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Debug short or long descriptions?");
			return;
		}

		IEnumerable<IEntityDescriptionPattern> patterns;
		switch (ss.Pop().ToLowerInvariant())
		{
			case "short":
			case "sdesc":
				patterns =
					actor.Gameworld.EntityDescriptionPatterns.Where(
						x => x.Type == EntityDescriptionType.ShortDescription).ToList();
				break;
			case "long":
			case "desc":
			case "full":
				patterns =
					actor.Gameworld.EntityDescriptionPatterns.Where(
						x => x.Type == EntityDescriptionType.FullDescription).ToList();
				break;
			default:
				actor.Send("Debug short or long descriptions?");
				return;
		}

		var target = ss.IsFinished ? actor : actor.TargetActor(ss.Pop());

		if (target == null)
		{
			actor.Send("No such person.");
			return;
		}

		using var file = new FileStream($"Debug Descriptions {DateTime.Now.ToFileTime()}.csv", FileMode.Create,
			FileAccess.Write);
		var writer = new StreamWriter(file);
		writer.WriteLine("id,text,pattern");
		foreach (
			var pattern in patterns.Where(x => x.IsValidSelection(target)).ToList())
		{
			writer.WriteLine(
				$"{pattern.Id},\"{target.ParseCharacteristics(pattern.Pattern, actor)}\",\"{pattern.Pattern}\"");
		}

		actor.Send($"Wrote to file #3{file.Name}#0".SubstituteANSIColour());
		writer.Close();
	}

	private static void DebugString(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which static string do you want to view?");
			return;
		}

		try
		{
			actor.OutputHandler.Send(actor.Gameworld.GetStaticString(ss.Pop()), nopage: true);
		}
		catch
		{
			actor.Send("There is no such static string.");
		}
	}

	private static void DebugStatic(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which static config do you want to view?");
			return;
		}

		try
		{
			actor.OutputHandler.Send(actor.Gameworld.GetStaticConfiguration(ss.Pop()), nopage: true);
		}
		catch
		{
			actor.Send("There is no such static configuration.");
		}
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

	private static void DebugCharacteristics(ICharacter actor, StringStack ss)
	{
		actor.Send("Pruning invalid characteristics...");
		actor.Send("Coming soon...");
	}

	private static void DebugGuests(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("There are {0} guests generated.", actor.Gameworld.Guests.Count());
			return;
		}

		if (!int.TryParse(ss.Pop(), out var number))
		{
			actor.Send("How many new guests do you want to generate?");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which NPC Template do you want to use to generate guests?");
			return;
		}

		var template = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.NpcTemplates.Get(value)
			: actor.Gameworld.NpcTemplates.GetByName(ss.Last, true);

		if (template == null)
		{
			actor.Send("There is no such template.");
			return;
		}

		if (GuestCharacter.GuestLoungeCell == null)
		{
			actor.Send("There is no guest lounge cell set. You cannot initialise guests.");
			return;
		}

		for (var i = 0; i < number; i++)
		{
			var newTemplate = template.GetCharacterTemplate(GuestCharacter.GuestLoungeCell);
			var character = new GuestCharacter(newTemplate, actor.Gameworld);
			template.OnLoadProg?.Execute(character);
			actor.Gameworld.AddGuest(character);
		}

		actor.Send("Generated {0} new guests from template {1} (#{2})", number, template.Name, template.Id);
	}

	private static void DebugSun(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which sun do you wish to change the time of?");
			return;
		}

		var sun = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.CelestialObjects.Get(value)
			: actor.Gameworld.CelestialObjects.GetByName(ss.Last);
		if (sun == null)
		{
			actor.Send("No such sun.");
			return;
		}

		if (ss.IsFinished || !int.TryParse(ss.SafeRemainingArgument, out var time))
		{
			actor.Send("How many minutes do you want to change the celestial time by?");
			return;
		}

		actor.Send("Done.");
		sun.AddMinutes(time);
	}

	private static void DebugCelestials(ICharacter actor)
	{
		actor.Send("Debugging Celestials...");
		var zone = actor.Location.Room.Zone;
		var now = DateTime.UtcNow;
		var zoneWriter = new StreamWriter(
			$"Zone {zone.Id} - {zone.Name} - Lat {zone.Geography.Latitude.RadiansToDegrees()} Long {zone.Geography.Longitude.RadiansToDegrees()} - {now:yyyyMMMMddhhmmss}.csv");
		zoneWriter.WriteLine("Date\tTime\tAscension\tAzimuth\tDirection\tIllumination\tLight Level");
		var calendar = actor.Location.Calendars.First();
		var clock = calendar.FeedClock;
		var timezone = actor.Location.Room.Zone.GetEditableZone.TimeZones[clock];
		var celestial = zone.Celestials.First();

		// Approximately 2 years
		for (var i = 0; i < 1000000; i++)
		{
			if (i % 10000 == 0)
			{
				Console.WriteLine(((double)i / 1000000.0).ToString("P0"));
			}

			clock.CurrentTime.AddMinutes(1);
			var celestialInfo = celestial.CurrentPosition(zone.Geography);
			var illumination = celestial.CurrentIllumination(zone.Geography);
			var datetime = new MudDateTime(zone.Date(calendar), zone.Time(clock), timezone);
			zoneWriter.WriteLine("\"{0}\"\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"",
				calendar.DisplayDate(datetime.Date, CalendarDisplayMode.Short),
				clock.DisplayTime(datetime.Time, TimeDisplayTypes.Short),
				celestialInfo.LastAscensionAngle.RadiansToDegrees().ToString("N3", actor),
				celestialInfo.LastAzimuthAngle.RadiansToDegrees().ToString("N3", actor),
				celestialInfo.Direction.Describe(), illumination,
				actor.Gameworld.LightModel.GetIlluminationDescription(illumination));
		}

		zoneWriter.Close();
		actor.Send("Done debugging Celestials.");
	}

	private static void DebugCombatSpeed(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send($"The current combat speed is {CombatBase.CombatSpeedMultiplier:N5}");
			return;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.Send("You must enter a valid positive number for the combat speed multiplier.");
			return;
		}

		using (new FMDB())
		{
			var setting = FMDB.Context.StaticConfigurations.Find("CombatSpeedMultiplier");
			setting.Definition = ss.SafeRemainingArgument;
			FMDB.Context.SaveChanges();
			(actor.Gameworld as IFuturemudLoader).LoadStaticValues();
		}

		CombatBase.CombatSpeedMultiplier = value;
		actor.Send($"The combat speed multiplier is now {value:N5}");
	}

	private static void DebugSaveQueue(ICharacter actor)
	{
		actor.Send(actor.Gameworld.SaveManager.DebugInfo(actor.Gameworld));
	}

	private static void CleanupAllPCsLoaded(IFuturemud gameworld, IEnumerable<ICharacter> loadedPCs)
	{
		foreach (var character in loadedPCs)
		{
			character.Quit(true);
		}

		gameworld.GameStatistics.RecordPlayersPaused = false;
	}

	private static IEnumerable<ICharacter> EnsureAllPCsAreLoaded(IFuturemud gameworld)
	{
		gameworld.GameStatistics.RecordPlayersPaused = true;

		// Firstly, flush the save manager in case there is anything pending
		gameworld.SystemMessage("Flushing the save manager...", true);
		gameworld.SaveManager.Flush();
		// Make sure that the messaging thread gets a chance to resume and send out echoes
		gameworld.ForceOutgoingMessages();
		Thread.Sleep(100);

		// Next, let's load up all the offline but alive PCs so that their inventory is counted.
		var loadedPCs = new List<ICharacter>();
		var onlinePCIDs = gameworld.Characters.Select(x => x.Id).ToList();
		gameworld.SystemMessage("Loading offline PCs so their inventory is accounted for...", true);
		using (new FMDB())
		{
			var PCsToLoad =
				FMDB.Context.Characters
					.Include(x => x.NpcsCharacter)
					.Where(
						x =>
							x.NpcsCharacter.Count == 0 &&
							x.Guest == null &&
							!onlinePCIDs.Contains(x.Id) &&
							(x.Status == (int)CharacterStatus.Active || x.Status == (int)CharacterStatus.Suspended)
							)
					.OrderBy(x => x.Id);
			var i = 0;
			while (true)
			{
				var any = false;
				foreach (var pc in PCsToLoad.Skip(i++ * 10).Take(10).ToList())
				{
					any = true;
					var character = gameworld.TryGetCharacter(pc.Id, true);
					character.Register(new NonPlayerOutputHandler());
					loadedPCs.Add(character);
					gameworld.Add(character, false);
				}

				if (!any)
				{
					break;
				}
			}
		}

		gameworld.SystemMessage($"Loaded {loadedPCs.Count} offline PCs", true);
		// Make sure that the messaging thread gets a chance to resume and send out echoes
		gameworld.ForceOutgoingMessages();
		Thread.Sleep(100);

		// Next, we force all corpses to load their characters, so their inventories are accounted for
		gameworld.SystemMessage("Forcing all corpses to load their characters...", true);
		gameworld.ForceOutgoingMessages();
		var corpsePCs = new HashSet<ICharacter>();
		foreach (var item in gameworld.Items.SelectNotNull(x => x.GetItemType<ICorpse>()).ToList())
		{
			corpsePCs.Add(item.OriginalCharacter);
		}

		gameworld.SystemMessage($"Loaded {corpsePCs.Count} corpses", true);
		// Make sure that the messaging thread gets a chance to resume and send out echoes
		gameworld.ForceOutgoingMessages();
		Thread.Sleep(100);

		// Next, we make sure all exits have been loaded so we can consider their doors
		gameworld.ExitManager.PreloadCriticalExits();

		return loadedPCs;
	}

	private static void DebugCleanupCorpses(ICharacter actor)
	{
		void DoCleanupCorpses()
		{
			var loadedPCs = EnsureAllPCsAreLoaded(actor.Gameworld);

			// Next we delete all corpses whose players are alive again, as well as skeletal remains of NPCs
			int pcs = 0, npcs = 0;
			actor.Gameworld.SystemMessage("Identifying superfluous corpses...", true);
			var corpses = actor.Gameworld.Items.SelectNotNull(x => x.GetItemType<ICorpse>()).ToList();
			foreach (var corpse in corpses)
			{
				if (corpse.OriginalCharacter.Status != CharacterStatus.Deceased)
				{
					corpse.Parent.Delete();
					pcs += 1;
					continue;
				}

				if (corpse.Decay == DecayState.Skeletal && corpse.OriginalCharacter is INPC)
				{
					corpse.Parent.Delete();
					npcs += 1;
					continue;
				}
			}

			actor.Gameworld.SystemMessage($"Removed {pcs} superfluous PC Corpses and {npcs} superfluous NPC corpses.",
				true);

			// Make sure that the messaging thread gets a chance to resume and send out echoes
			actor.Gameworld.ForceOutgoingMessages();
			Thread.Sleep(100);

			var severed = actor.Gameworld.Items.SelectNotNull(x => x.GetItemType<ISeveredBodypart>()).ToList();
			var npcsToRemove = new List<Models.Npc>();
			using (new FMDB())
			{
				foreach (var npc in FMDB.Context.Npcs.Include(x => x.Character.Body)
										.Where(x => x.Character.State == (int)CharacterState.Dead).ToList())
				{
					if (corpses.Any(x => x.OriginalCharacter.Id == npc.CharacterId))
					{
						continue;
					}

					if (severed.Any(x => x.OriginalCharacterId == npc.CharacterId))
					{
						continue;
					}

					npcsToRemove.Add(npc);
				}

				FMDB.Context.Bodies.RemoveRange(npcsToRemove.Select(x => x.Character.Body));
				FMDB.Context.SaveChanges();
			}

			actor.Gameworld.SystemMessage($"Removed {npcsToRemove.Count} dead NPCs.", true);
			// Make sure that the messaging thread gets a chance to resume and send out echoes
			actor.Gameworld.ForceOutgoingMessages();
			Thread.Sleep(100);

			CleanupAllPCsLoaded(actor.Gameworld, loadedPCs);
		}

		actor.Send(
			"This can take a long time to complete. Are you sure you wish to do this now? Type ACCEPT to begin.");
		actor.AddEffect(new Accept(actor, new GenericProposal(
			text =>
			{
				actor.Gameworld.SystemMessage(
					"A maintenance task is due to begin in 30 seconds that may run for a long time, and cause the game to appear to hang.");
				actor.Gameworld.Scheduler.AddSchedule(new Schedule(() =>
				{
					actor.Gameworld.SystemMessage("The long-running maintenance task has begun.");
					actor.Gameworld.SaveManager.Flush();
					DoCleanupCorpses();
					actor.Gameworld.SystemMessage("The long-running maintenance task has now completed.");
				}, ScheduleType.System,
#if DEBUG
				TimeSpan.FromSeconds(1)
#else
				TimeSpan.FromSeconds(30)
#endif

				, "Cleaning up corpses"));
			},
			text => { actor.Send("You decide against cleaning up corpses."); },
			() => { actor.Send("You decide against cleaning up corpses."); },
			"Cleaning up corpses",
			"cleanup",
			"items"
		)), TimeSpan.FromSeconds(120));
	}

	private const string GPTHelp =
		@"This command is used to create GPTThreads which can be used in progs, AI and autobuilding.

The syntax is as follows:

#3GPT list#0 - lists all GPT Threads
#3GPT show <which>#0 - shows a GPT Thread
#3GPT create <name> <temp> <model>#0 - drops you into a model to create a new GPT Thread
#3GPT delete <which>#0 - deletes a GPT Thread
#3GPT set <which> name <name>#0 - renames a GPT Thread
#3GPT set <which> temperature <temp>#0 - sets the thread temperature
#3GPT set <which> model <name>#0 - changes the GPT Model
#3GPT set <which> prompt#0 - drops into an editor to write the prompt";

	[PlayerCommand("GPT", "gpt")]
	[CommandPermission(PermissionLevel.Founder)]
	[HelpInfo("GPT", GPTHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void GPT(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "list":
				GPTList(actor, ss);
				return;
			case "add":
			case "create":
			case "new":
				GPTCreate(actor, ss);
				return;
			case "delete":
				GPTDelete(actor, ss);
				return;
			case "show":
			case "view":
				GPTShow(actor, ss);
				return;
			case "set":
				GPTSet(actor, ss);
				return;
			case "query":
				GPTQuery(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(GPTHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void GPTQuery(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which GPT Thread would you like to set the properties of?");
			return;
		}

		using (new FMDB())
		{
			Models.GPTThread thread;
			if (long.TryParse(ss.PopSpeech(), out var id))
			{
				thread = FMDB.Context.GPTThreads.Find(id);
			}
			else
			{
				thread = FMDB.Context.GPTThreads.Include(x => x.Messages).FirstOrDefault(x => x.Name == ss.Last);
			}

			if (thread is null)
			{
				actor.OutputHandler.Send("There is no such GPT Thread.");
				return;
			}

			actor.OutputHandler.Send($"You make the following request to the GPT Thread:\n\n{ss.SafeRemainingArgument}");
			MudSharp.OpenAI.OpenAIHandler.MakeGPTRequest(thread, ss.SafeRemainingArgument, actor, text =>
			{
				actor.OutputHandler.Send($"GPT Response:\n\n{text}");
			}, -1, true);
		}
	}

	private static void GPTSet(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which GPT Thread would you like to set the properties of?");
			return;
		}

		using (new FMDB())
		{
			Models.GPTThread thread;
			if (long.TryParse(ss.PopSpeech(), out var id))
			{
				thread = FMDB.Context.GPTThreads.Find(id);
			}
			else
			{
				thread = FMDB.Context.GPTThreads.Include(x => x.Messages).FirstOrDefault(x => x.Name == ss.Last);
			}

			if (thread is null)
			{
				actor.OutputHandler.Send("There is no such GPT Thread.");
				return;
			}

			switch (ss.PopForSwitch())
			{
				case "model":
					GPTSetModel(actor, thread, ss);
					return;
				case "temp":
				case "temperature":
					GPTSetTemperature(actor, thread, ss);
					return;
				case "prompt":
					GPTSetPrompt(actor, thread, ss);
					return;
				case "name":
					GPTSetName(actor, thread, ss);
					return;
				default:
					actor.OutputHandler.Send(
						"You must either use #3model#0, #3name#0, #3prompt#0 or #3temperature#0 as your argument."
							.SubstituteANSIColour());
					return;
			}
		}
	}

	private static void GPTSetName(ICharacter actor, GPTThread thread, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this GPTThread?");
			return;
		}

		var name = ss.SafeRemainingArgument.Trim();
		if (FMDB.Context.GPTThreads.Any(x => x.Name == name))
		{
			actor.OutputHandler.Send(
				$"There is already a GPTThread called {name.ColourName()}. Names must be unique.");
			return;
		}

		var oldName = thread.Name;
		thread.Name = name;
		FMDB.Context.SaveChanges();
		actor.OutputHandler.Send($"You rename the GPTThread {oldName.ColourName()} to {name.ColourName()}.");
	}

	private static void GPTSetPrompt(ICharacter actor, GPTThread thread, StringStack ss)
	{
		actor.OutputHandler.Send($"Replacing the following prompt:\n\n{thread.Prompt.Wrap(actor.InnerLineFormatLength, "\t")}\n\nEnter your new prompt below.");
		actor.EditorMode(PostAction, CancelAction, 1.0, thread.Prompt, EditorOptions.None, new object[] { thread.Id, thread.Name});
	}

	private static void CancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send($"You decide not to alter the prompt for GPTThread {args[1].ToString().ColourName()}.");
	}

	private static void PostAction(string text, IOutputHandler handler, object[] args)
	{
		using (new FMDB())
		{
			var thread = FMDB.Context.GPTThreads.Find((long)args[0]);
			if (thread is null)
			{
				handler.Send("The thread had been deleted by the time the post action was made.");
				return;
			}

			thread.Prompt = text;
			FMDB.Context.SaveChanges();
			handler.Send($"You update the prompt for the {args[1].ToString().ColourName()} GPTThread.");
		}
	}

	private static void GPTSetTemperature(ICharacter actor, GPTThread thread, StringStack ss)
	{
		if (ss.IsFinished || !double.TryParse(ss.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send("You must enter a valid number to be the mode temperature.");
			return;
		}

		thread.Temperature = value;
		FMDB.Context.SaveChanges();
		actor.OutputHandler.Send($"The {thread.Name.ColourName()} GPTThread will now have a temperature of {value.ToString("N3", actor).ColourValue()}.");
	}

	private static void GPTSetModel(ICharacter actor, GPTThread thread, StringStack ss)
	{
		var models = OpenAIHandler.GPTModels().Result;
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which model should this GPTThread use?\nThe valid models are {models.Select(x => x.ColourName()).ListToString()}.");
			return;
		}

		var modelText = ss.SafeRemainingArgument;
		var model = models.FirstOrDefault(x => x.EqualTo(modelText)) ??
					models.FirstOrDefault(x => x.StartsWith(modelText, StringComparison.InvariantCultureIgnoreCase));
		if (model is null)
		{
			actor.OutputHandler.Send($"There is no such model.\nThe valid models are {models.Select(x => x.ColourName()).ListToString()}.");
			return;
		}

		thread.Model = model;
		FMDB.Context.SaveChanges();
		actor.OutputHandler.Send($"The {thread.Name.ColourName()} GPTThread will now use the {model.ColourName()} model.");
	}

	private static void GPTShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which GPT Thread would you like to view?");
			return;
		}

		using (new FMDB())
		{
			Models.GPTThread thread;
			if (long.TryParse(ss.PopSpeech(), out var id))
			{
				thread = FMDB.Context.GPTThreads.Find(id);
			}
			else
			{
				thread = FMDB.Context.GPTThreads.Include(x => x.Messages).FirstOrDefault(x => x.Name == ss.Last);
			}

			if (thread is null)
			{
				actor.OutputHandler.Send("There is no such GPT Thread.");
				return;
			}

			var sb = new StringBuilder();
			sb.AppendLine($"GPT Thread #{thread.Id.ToString("N0", actor)} - {thread.Name.ColourName()}");
			sb.AppendLine($"Model: {thread.Model.ColourValue()}");
			sb.AppendLine($"Temperature: {thread.Temperature.ToString("N2", actor).ColourValue()}");
			sb.AppendLine($"Messages: {thread.Messages.Count.ToString("N0", actor).ColourValue()}");
			sb.AppendLine($"Prompt:");
			sb.AppendLine();
			sb.AppendLine(thread.Prompt.Wrap(actor.InnerLineFormatLength, "\t").ColourCommand());
			actor.OutputHandler.Send(sb.ToString());
		}
	}

	private static void GPTDelete(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which GPTThread are you trying to delete?");
			return;
		}

		using (new FMDB())
		{
			Models.GPTThread thread;
			if (long.TryParse(ss.PopSpeech(), out var id))
			{
				thread = FMDB.Context.GPTThreads.Find(id);
			}
			else
			{
				thread = FMDB.Context.GPTThreads.Include(x => x.Messages).FirstOrDefault(x => x.Name == ss.Last);
			}

			if (thread is null)
			{
				actor.OutputHandler.Send("There is no such GPT Thread.");
				return;
			}

			actor.OutputHandler.Send(
				$"Are you sure you want to delete the {thread.Name.ColourName()} GPTThread? This cannot be undone.\n{Accept.StandardAcceptPhrasing}");
			actor.AddEffect(new Accept(actor, new GenericProposal
			{
				DescriptionString = "Deleting a GPTThread",
				AcceptAction = text =>
				{
					actor.OutputHandler.Send($"You delete the {thread.Name.ColourName()} GPTThread.");
					using (new FMDB())
					{
						var dbitem = FMDB.Context.GPTThreads.Find(thread.Id);
						if (dbitem == null)
						{
							return;
						}

						FMDB.Context.GPTThreads.Remove(dbitem);
						FMDB.Context.SaveChanges();
					}
				},
				RejectAction = text =>
				{
					actor.OutputHandler.Send($"You decide not to delete the {thread.Name.ColourName()} GPTThread.");
				},
				ExpireAction = () =>
				{
					actor.OutputHandler.Send($"You decide not to delete the {thread.Name.ColourName()} GPTThread.");
				}
			}), TimeSpan.FromSeconds(120));
		}
	}

	private static void GPTCreate(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new model?");
			return;
		}

		var name = ss.PopSpeech();
		using (new FMDB())
		{
			if (FMDB.Context.GPTThreads.Any(x => x.Name == name))
			{
				actor.OutputHandler.Send(
					$"There is already a GPTThread called {name.ColourName()}. Names must be unique.");
				return;
			}
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the temperature of this model? Numbers between 0.3 and 2.0 are common, with higher numbers meaning more randomness.");
			return;
		}

		if (!double.TryParse(ss.PopSpeech(), out var temperature) || temperature < 0)
		{
			actor.OutputHandler.Send("That is not a valid temperature.");
			return;
		}

		var models = OpenAIHandler.GPTModels().Result;
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which GPT Model should your thread use? Valid choices are {models.Select(x => x.ColourName()).ListToString()}.");
			return;
		}

		var modelText = ss.SafeRemainingArgument;
		if (modelText is null)
		{
			actor.OutputHandler.Send($"There is no such model.\nThe valid models are {models.Select(x => x.ColourName()).ListToString()}.");
			return;
		}

		actor.OutputHandler.Send("Enter a prompt for GPT below:");
		actor.EditorMode(PostActionCreate, CancelActionCreate, 1.0, null, EditorOptions.None, new object[]
		{
			name,
			temperature,
			modelText,
			actor
		});
	}

	private static void CancelActionCreate(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to create a new GPTThread.");
	}

	private static void PostActionCreate(string text, IOutputHandler handler, object[] args)
	{
		using (new FMDB())
		{
			var dbitem = new GPTThread
			{
				Name = (string)args[0],
				Temperature = (double)args[1],
				Model = (string)args[2],
				Prompt = text
			};
			FMDB.Context.GPTThreads.Add(dbitem);
			FMDB.Context.SaveChanges();
			handler.Send($"You create GPTThread #{dbitem.Id.ToString("N0", (ICharacter)args[3])} {dbitem.Name.ColourName()}.");
		}
	}

	private static void GPTList(ICharacter actor, StringStack ss)
	{
		using (new FMDB())
		{
			actor.OutputHandler.Send(StringUtilities.GetTextTable(
				FMDB.Context.GPTThreads.Include(x => x.Messages).Select(x => new []
				{
					x.Id.ToString("N0", actor),
					x.Name,
					x.Model,
					x.Temperature.ToString("N2", actor),
					x.Messages.Count.ToString("N0", actor)
				}),
				new List<string>
				{
					"Id",
					"Name",
					"Model",
					"Temperature",
					"Messages"
				},
				actor,
				colour: Telnet.BoldOrange
			));
		}
	}
}