﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using ExpressionEngine;
using TimeZoneConverter;

namespace MudSharp.Commands.Modules;

internal class CharacterInformationModule : Module<ICharacter>
{
	private CharacterInformationModule()
		: base("Character Information")
	{
		IsNecessary = true;
	}

	public override int CommandsDisplayOrder => 7;

	public static CharacterInformationModule Instance { get; } = new();

	[PlayerCommand("Set", "set")]
	protected static void Set(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var sb = new StringBuilder();
		if (ss.IsFinished)
		{
			sb.AppendLine("Account Settings:".Colour(Telnet.Cyan));
			sb.AppendLine();
			sb.AppendLine(
				$"Line Wrap Width: {actor.Account.LineFormatLength.ToString("N0", actor).Colour(Telnet.Green)} (Ordinary) - {actor.Account.InnerLineFormatLength.ToString("N0", actor).Colour(Telnet.Green)} (Text Blocks)");
			sb.AppendLine($"Page Length: {actor.Account.PageLength.ToString().Colour(Telnet.Green)}");
			sb.AppendLine($"Unicode: {actor.Account.UseUnicode.ToString().Colour(Telnet.Green)}");
			sb.AppendLine($"Mud Sound Protocol (MSP): {actor.Account.UseUnicode.ToString().Colour(Telnet.Green)}");
			sb.AppendLine(
				$"Mud Client Compression Protocol (MCCP): {actor.Account.UseUnicode.ToString().Colour(Telnet.Green)}");
			sb.AppendLine($"Culture: {actor.Account.Culture.ToString().Colour(Telnet.Green)}");
			sb.AppendLine($"Timezone: {actor.Account.TimeZone.ToString().Colour(Telnet.Green)}");
			sb.AppendLine($"Unit Preference: {actor.Account.UnitPreference.Colour(Telnet.Green)}");
			sb.AppendLine($"Prompt Type: {actor.Account.PromptType.Describe().Colour(Telnet.Green)}");
			sb.AppendLine($"Tabs in Room Descs: {actor.Account.TabRoomDescriptions.ToString().Colour(Telnet.Green)}");
			sb.AppendLine(
				$"Room Desc Additions on New Line: {actor.Account.CodedRoomDescriptionAdditionsOnNewLine.ToColouredString()}");
			sb.AppendLine(
				$"Newlines between multiple echoes: {actor.Account.AppendNewlinesBetweenMultipleEchoesPerPrompt.ToColouredString()}");
			sb.AppendLine(
				$"Character Name Overlays: {actor.Account.CharacterNameOverlaySetting.DescribeEnum().ColourValue()}");
			sb.AppendLine($"Acting Lawfully: {actor.Account.ActLawfully.ToColouredString()}");
			sb.AppendLine();
			sb.AppendLine("Character Settings:".Colour(Telnet.Cyan));
			sb.AppendLine();
			sb.AppendLine($"Writing Style: {actor.WritingStyle.Describe().Colour(Telnet.Green)}");
			sb.AppendLine($"Currency: {actor.Currency?.Name.Colour(Telnet.Green)}");
			sb.AppendLine($"Showing Mercy: {(!actor.NoMercy).ToColouredString()}");
			if (actor.IsAdministrator())
			{
				sb.AppendLine(
					$"Admin Telepathy: {actor.Effects.Any(x => x is AdminTelepathy).ToString().Colour(Telnet.Green)}");
			}

			sb.AppendLine(
				$"Brief Combat Mode: {(actor.BriefCombatMode ? "True".Colour(Telnet.Green) : "False".Colour(Telnet.Red))}");
			sb.AppendLine(
				$"Brief Room Descriptions: {(actor.BriefRoomDescs ? "True".Colour(Telnet.Green) : "False".Colour(Telnet.Red))}");
			sb.AppendLine();
			sb.AppendLine("Note - for combat settings, see COMBAT HELP.".Colour(Telnet.Yellow));

			actor.Send(sb.ToString());
			return;
		}

		switch (ss.Pop().ToLowerInvariant())
		{
			case "?":
			case "help":
				actor.Send("Options for the {0} command:\n{1}{2}", "set".Colour(Telnet.Yellow), new[]
					{
						"set wrap <normal> <text> - sets the wrap width of ordinary text and text blocks. Usually the second argument should be smaller than the first.",
						"set page <length> - sets the number of lines before one requires the use of the more command.",
						"set unicode - toggles unicode mode on or off.",
						"set msp - toggles msp support on or off.",
						"set mccp - toggles mccp support on or off.",
						"set culture <culture> - changes your out of character culture - e.g. en-us or fr-fr.",
						"set timezone <timezone> - changes your out of character timezone. Use set timezone ? to show a list.",
						"set units <unit system> - changes your unit preferences - e.g. imperial or metric.",
						"set currency <currency> - changes the currency you use in economic transactions.",
						"set combatbrief - toggles the hiding of combat messages not applying to you.",
						"set writing <styles> - sets what style of writing you use. See SHOW WRITINGSTYLES",
						"set brief - toggles whether to show room descriptions or not.",
						"set prompt full|classic|brief|fullbrief|speaking|stealth|position|magic - sets which prompt you prefer to see.",
						"set roomtabs - toggles tabs at the beginning of room descriptions",
						"set roomadditions - toggles coded room description additions (weather, light, temp etc) being on new lines.",
						"set newlines - toggles newlines between multiple echoes between prompts",
						"set names none|brackets|replace - sets name replacement overlay options",
						"set mercy - toggles showing no mercy (accepting or not accepting surrenders)",
						"set lawful - toggles acting lawfully and preventing actions that would criminalise you"
					}.ListToString(separator: "\n", twoItemJoiner: "\n", conjunction: "", article: "\t"),
					actor.IsAdministrator()
						? "\n\tset telepathy - toggles admin telepathy (seeing player thoughts and feelings)"
						: ""
				);
				return;
			case "roomtabs":
				actor.Account.TabRoomDescriptions = !actor.Account.TabRoomDescriptions;
				actor.Send(
					$"You will {(actor.Account.TabRoomDescriptions ? "now" : "no longer")} see tabs at the beginning of room descriptions");
				return;
			case "mercy":
				actor.NoMercy = !actor.NoMercy;
				actor.OutputHandler.Send(
					$"You will {(actor.NoMercy ? "no longer" : "now")} show mercy to your foes in combat.");
				return;
			case "roomaddition":
			case "addition":
			case "additions":
			case "roomadditions":
				actor.Account.CodedRoomDescriptionAdditionsOnNewLine =
					!actor.Account.CodedRoomDescriptionAdditionsOnNewLine;
				actor.Send(
					$"You will {(actor.Account.CodedRoomDescriptionAdditionsOnNewLine ? "now" : "no longer")} see coded room descriptions like light levels on new lines.");
				return;
			case "newlines":
			case "newline":
				actor.Account.AppendNewlinesBetweenMultipleEchoesPerPrompt =
					!actor.Account.AppendNewlinesBetweenMultipleEchoesPerPrompt;
				actor.OutputHandler.Send(
					$"You will {(actor.Account.AppendNewlinesBetweenMultipleEchoesPerPrompt ? "now" : "no longer")} see additional new lines between echoes when you get multipler per prompt.");
				return;
			case "lawful":
				actor.Account.ActLawfully = !actor.Account.ActLawfully;
				actor.OutputHandler.Send(
					$"You will {(actor.Account.ActLawfully ? "now" : "no longer")} act lawfully and refuse to do criminal actions.");
				return;
			case "names":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send(
						"Which setting do you want for displaying names with short descriptions? The valid options are:\n\tnone - do not display names with short descriptions\n\tbrackets - display the name in brackets after the short description\n\treplace - replace the short description with the name");
					return;
				}

				switch (ss.SafeRemainingArgument.ToLowerInvariant())
				{
					case "none":
					case "off":
						actor.Account.CharacterNameOverlaySetting = Accounts.CharacterNameOverlaySetting.None;
						actor.OutputHandler.Send(
							"You will no longer see any names added to or replacing short descriptions.");
						return;
					case "brackets":
					case "bracket":
					case "after":
						actor.Account.CharacterNameOverlaySetting =
							Accounts.CharacterNameOverlaySetting.AppendWithBrackets;
						actor.OutputHandler.Send(
							"You will now see names appended after short descriptions in brackets.");
						return;
					case "replace":
					case "substitute":
						actor.Account.CharacterNameOverlaySetting = Accounts.CharacterNameOverlaySetting.Replace;
						actor.OutputHandler.Send("You will now see names instead of short descriptions.");
						return;
				}

				actor.OutputHandler.Send("The valid options are none, brackets and replace.");
				return;
			case "prompt":
				switch (ss.Pop().ToLowerInvariant())
				{
					case "full":
						actor.Account.PromptType &= ~PromptType.Classic;
						actor.Account.PromptType &= ~PromptType.FullBrief;
						actor.Account.PromptType &= ~PromptType.Brief;
						actor.Account.PromptType |= PromptType.Full | PromptType.PositionInfo;
						actor.Send("You now use the full, verbose prompt.");
						return;
					case "classic":
						actor.Account.PromptType &= ~PromptType.Full;
						actor.Account.PromptType &= ~PromptType.FullBrief;
						actor.Account.PromptType &= ~PromptType.Brief;
						actor.Account.PromptType |= PromptType.Classic;
						actor.Send("You now use a classic RPI-Engine style prompt.");
						return;
					case "fullbrief":
						actor.Account.PromptType &= ~PromptType.Classic;
						actor.Account.PromptType &= ~PromptType.Full;
						actor.Account.PromptType &= ~PromptType.Brief;
						actor.Account.PromptType |= PromptType.FullBrief;
						actor.Send("You now use an abbreviated version of the verbose prompt.");
						return;
					case "brief":
						actor.Account.PromptType &= ~PromptType.Classic;
						actor.Account.PromptType &= ~PromptType.Full;
						actor.Account.PromptType &= ~PromptType.FullBrief;
						actor.Account.PromptType |= PromptType.Brief;
						actor.Send("You will now use a totally minimal prompt.");
						return;
					case "speaking":
					case "speak":
					case "language":
						if (actor.Account.PromptType.HasFlag(PromptType.SpeakInfo))
						{
							actor.Account.PromptType &= ~PromptType.SpeakInfo;
							actor.Send("You will no longer see what language you're speaking in your prompt.");
						}
						else
						{
							actor.Account.PromptType |= PromptType.SpeakInfo;
							actor.Send("You will now see what language you're speaking in your prompt.");
						}

						return;
					case "position":
						if (actor.Account.PromptType.HasFlag(PromptType.PositionInfo))
						{
							actor.Account.PromptType &= ~PromptType.PositionInfo;
							actor.Send("You will no longer see Position info in Full, FullBrief or Classic mode.");
						}
						else
						{
							actor.Account.PromptType |= PromptType.PositionInfo;
							actor.Send("You will now see Position info in Full, FullBrief or Classic mode.");
						}

						return;
					case "sneak":
					case "sneaking":
					case "stealth":
					case "hiding":
					case "hide":
						if (actor.Account.PromptType.HasFlag(PromptType.StealthInfo))
						{
							actor.Account.PromptType &= ~PromptType.StealthInfo;
							actor.Send("You will no longer see the sneaking field when hiding.");
						}
						else
						{
							actor.Account.PromptType |= PromptType.StealthInfo;
							actor.Send(
								"You will now have a sneaking status field when hiding in Full, FullBrief or Classic mode.");
						}

						return;
					case "magic":
						if (!actor.Capabilities.Any())
						{
							goto default;
						}

						if (actor.Account.PromptType.HasFlag(PromptType.IncludeMagic))
						{
							actor.Account.PromptType &= ~PromptType.IncludeMagic;
							actor.OutputHandler.Send("You will no longer see magic resources in the prompt.");
						}
						else
						{
							actor.Account.PromptType |= PromptType.IncludeMagic;
							actor.OutputHandler.Send("You will now see magic resources in the prompt.");
						}

						return;
					default:
						if (ss.IsFinished)
						{
							var promptb = new StringBuilder();
							if (actor.Account.PromptType == 0 || actor.Account.PromptType.HasFlag(PromptType.Full))
							{
								promptb.Append(" - Your prompt mode is: Full\n");
							}
							else if (actor.Account.PromptType.HasFlag(PromptType.FullBrief))
							{
								promptb.Append(" - Your prompt mode is: Brief\n");
							}
							else if (actor.Account.PromptType.HasFlag(PromptType.Classic))
							{
								promptb.Append(" - Your prompt mode is: Classic\n");
							}

							promptb.AppendLine(actor.Account.PromptType.HasFlag(PromptType.SpeakInfo)
								? " - Speaking Field: Enabled"
								: " - Speaking Field: Disabled");
							if (actor.Account.PromptType.HasFlag(PromptType.PositionInfo) &&
							    (actor.Account.PromptType.HasFlag(PromptType.Full) ||
							     actor.Account.PromptType.HasFlag(PromptType.FullBrief)))
							{
								promptb.AppendLine(" - Position Field: Enabled");
							}
							else
							{
								promptb.AppendLine(" - Position Field: Disabled");
							}

							promptb.AppendLine(actor.Account.PromptType.HasFlag(PromptType.StealthInfo)
								? " - Stealth Field: Enabled"
								: " - Stealth Field: Disabled");
							actor.Send(promptb.ToString());
						}
						else
						{
							actor.Send(
								"That is not a valid type of prompt preference to set. Select 'full', 'brief', 'fullbrief' or 'classic', or toggle 'language', 'stealth', or 'position'.");
						}

						return;
				}
			case "telepathy":
				if (!actor.IsAdministrator())
				{
					goto default;
				}

				if (actor.Effects.Any(x => x is AdminTelepathy))
				{
					actor.RemoveAllEffects(x => x is AdminTelepathy);
					actor.Send("You will no longer tune into player thoughts.");
					return;
				}

				actor.AddEffect(new AdminTelepathy(actor));
				actor.Send("You are now tuned into player thoughts.");
				return;
			case "wrap":
				if (ss.IsFinished)
				{
					actor.Send("What width do you want text to wrap at?");
					return;
				}

				if (!int.TryParse(ss.Pop(), out var outerWrap))
				{
					actor.Send("You must enter a number for your wrap width.");
					return;
				}

				if (ss.IsFinished)
				{
					actor.Send("What width do you want large blocks of text to?");
					return;
				}

				if (!int.TryParse(ss.Pop(), out var innerWrap))
				{
					actor.Send("You must enter a number for your wrap width.");
					return;
				}

				if (outerWrap < 50 || innerWrap < 50)
				{
					actor.Send("The minimum wrap width is 50.");
					return;
				}

				actor.Account.LineFormatLength = outerWrap;
				actor.Account.InnerLineFormatLength = innerWrap;
				actor.Send("You set your wrap width to {0} for ordinary text and {1} for text blocks.", outerWrap,
					innerWrap);
				return;
			case "page":
				if (ss.IsFinished)
				{
					actor.Send("What number of lines do you want to break pages at?");
					return;
				}

				if (!int.TryParse(ss.Pop(), out var pageLength))
				{
					actor.Send("You must enter a number for your page length.");
					return;
				}

				if (pageLength < 10)
				{
					actor.Send("The minimum page length is 10.");
					return;
				}

				actor.Account.PageLength = pageLength;
				actor.Send("You set your page length to {0}.", pageLength);
				return;
			case "unicode":
				actor.Send(actor.Account.UseUnicode
					? "You will no longer use unicode."
					: "You will now use unicode.");
				actor.Account.UseUnicode = !actor.Account.UseUnicode;
				return;
			case "culture":
				if (ss.IsFinished)
				{
					actor.Send("Which culture do you want to use?");
					return;
				}

				var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
				var culturePick = ss.Pop();
				if (!cultures.Any(x => x.Name.Equals(culturePick, StringComparison.InvariantCultureIgnoreCase)))
				{
					actor.Send("That is not a valid culture for you to select.");
					return;
				}

				actor.Account.Culture =
					cultures.First(x => x.Name.Equals(culturePick, StringComparison.InvariantCultureIgnoreCase));
				actor.Send("You will now use the {0} culture.",
					actor.Account.Culture.EnglishName.Colour(Telnet.Green));
				return;
			case "timezone":
				if (ss.IsFinished)
				{
					actor.Send("Which timezone do you want to use?");
					return;
				}

				TimeZoneInfo timezone;
				try
				{
					timezone = TZConvert.GetTimeZoneInfo(ss.SafeRemainingArgument);
				}
				catch (TimeZoneNotFoundException)
				{
					actor.Send("There is no such timezone.");
					return;
				}

				actor.Account.TimeZone = timezone;
				actor.Send("You will now use the {0} timezone.",
					actor.Account.TimeZone.DisplayName.Colour(Telnet.Green));
				return;
			case "units":
				var unitSystems = actor.Gameworld.UnitManager.Units.Select(x => x.System).Distinct().ToList();
				if (ss.IsFinished)
				{
					actor.Send("You must select one of the following options: {0}",
						unitSystems.Select(x => x.Colour(Telnet.Green)).ListToString(conjunction: "or "));
					return;
				}

				var unitChoice = ss.Pop();
				if (!unitSystems.Any(x => x.Equals(unitChoice, StringComparison.InvariantCultureIgnoreCase)))
				{
					actor.Send("You must select one of the following options: {0}",
						unitSystems.Select(x => x.Colour(Telnet.Green)).ListToString(conjunction: "or "));
					return;
				}

				actor.Account.UnitPreference = unitChoice.ToLowerInvariant().Proper();
				actor.Send("You will now use the {0} system for display of units.",
					unitChoice.ProperSentences().Colour(Telnet.Green));
				return;
			case "writing":
				if (ss.IsFinished)
				{
					actor.Send("What writing style do you wish to adopt?");
					return;
				}

				var newStyle = WritingStyleDescriptors.None;
				var text = ss.PopSpeech();
				var value = WritingStyleDescriptors.None.Parse(text);
				if (value == WritingStyleDescriptors.None)
				{
					actor.Send("There is no such writing style. See SHOW WRITINGSTYLES.");
					return;
				}

				if (value.IsMachineDescriptor() && !actor.IsAdministrator())
				{
					actor.Send("Only machines can write with the necessary precision to adopt that writing style.");
					return;
				}

				if (value.MinimumHandwritingSkill() >
				    actor.TraitValue(actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("HandwritingSkillId"))))
				{
					actor.Send(
						$"Your {actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("HandwritingSkillId")).Name} skill is not high enough to adopt that handwriting style.");
					return;
				}

				if (ss.IsFinished)
				{
					actor.WritingStyle = value;
					actor.Send($"Your handwriting style is now {actor.WritingStyle.Describe().Colour(Telnet.Green)}.");
					return;
				}

				newStyle = value;
				value = WritingStyleDescriptors.None.Parse(ss.PopSpeech());
				if (value == WritingStyleDescriptors.None)
				{
					actor.Send("There is no such writing style. See SHOW WRITINGSTYLES.");
					return;
				}

				if (value.IsMachineDescriptor() && !actor.IsAdministrator())
				{
					actor.Send("Only machines can write with the necessary precision to adopt that writing style.");
					return;
				}

				if (value.IsModifierDescriptor())
				{
					actor.Send(
						"If you specify a second writing style, it can only be a non-modifier style. See SHOW WRITINGSTYLES.");
					return;
				}

				if (value.MinimumHandwritingSkill() >
				    actor.TraitValue(actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("HandwritingSkillId"))))
				{
					actor.Send(
						$"Your {actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("HandwritingSkillId")).Name} skill is not high enough to adopt that handwriting style.");
					return;
				}

				newStyle |= value;
				actor.WritingStyle = newStyle;
				actor.Send($"Your handwriting style is now {actor.WritingStyle.Describe().Colour(Telnet.Green)}.");
				return;
			case "combatbrief":
				actor.BriefCombatMode = !actor.BriefCombatMode;
				actor.Send(
					$"You will {(!actor.BriefCombatMode ? "no longer" : "now")} hide messages that do not pertain to you or people you are targeting in combat.");
				actor.Changed = true;
				return;
			case "brief":
				actor.BriefRoomDescs = !actor.BriefRoomDescs;
				actor.Send(
					$"You will {(actor.BriefRoomDescs ? "now" : "no longer")} see full room descriptions as you move about.");
				actor.Changed = true;
				return;
			default:
				actor.Send("That is not a valid option for the set command.");
				return;
		}
	}

	[PlayerCommand("Consider", "consider", "con")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Consider(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var arg = ss.Pop();
		if (string.IsNullOrEmpty(arg))
		{
			actor.OutputHandler.Send("Who do you wish to consider?");
			return;
		}

		var target = actor.TargetActor(arg);
		if (target == null)
		{
			actor.OutputHandler.Send("You do not see them here to consider.");
			return;
		}

		var gender = target.ApparentGender(actor);
		var sb = new StringBuilder();
		sb.AppendLine("You consider " + target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf) + ":");
		sb.AppendLine();
		if (target.Race == actor.Race || actor.IsAdministrator())
		{
			var age = target.Birthday.Calendar.CurrentDate.YearsDifference(target.Birthday) + 0.001;
			sb.AppendLine(
				$"{gender.Subjective(true)}{(gender == Indeterminate.Instance ? " are" : " is ")}between {(Math.Floor(age / 5.0) * 5).ToString("N0", actor).ColourValue()} and {(Math.Ceiling(age / 5.0) * 5).ToString("N0", actor).ColourValue()} years old, making {gender.Objective()} {target.Race.AgeCategory(target).DescribeEnum(true).A_An(false, Telnet.Green)}.");
		}

		sb.AppendLine(
			$"{gender.Subjective(true)} {gender.Is()} about {actor.Gameworld.UnitManager.Describe(target.Height, UnitType.Length, actor).ColourValue()} tall and {target.DescribeCharacteristic("Height", actor.Body).IfEmpty("of similar height").ColourValue()} relative to you.");
		sb.AppendLine(
			$"{gender.Subjective(true)} {(gender.UseThirdPersonVerbForms ? "weighs" : "weigh")} about {actor.Gameworld.UnitManager.DescribeMostSignificant(target.Weight, UnitType.Mass, actor).ColourValue()}.");
		if (actor.Race == target.Race || actor.IsAdministrator())
		{
			sb.AppendLine($"You'd guess their ethnicity to be {target.Ethnicity.Name.ColourValue()}.");
		}

		var consider = target.GetConsiderString(actor);
		if (!string.IsNullOrWhiteSpace(consider))
		{
			sb.AppendLine(consider);
		}

		actor.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	[PlayerCommand("SkillCategories", "skillcategories")]
	[HelpInfo("skillcategories",
		"The SKILLCATEGORIES command allows you to view all of your skills and their current levels, but splits the output by categories.",
		AutoHelp.HelpArg)]
	protected static void SkillCategories(ICharacter actor, string input)
	{
		var sb = new StringBuilder();
		var adminMode = actor.IsAdministrator();
		foreach (var group in actor.TraitsOfType(TraitType.Skill).Where(x => !x.Hidden).GroupBy(x => x.Definition.Group)
		                           .OrderBy(x => x.Key))
		{
			if (sb.Length > 0)
			{
				sb.AppendLine();
			}

			sb.AppendLine($"Category: {group.Key.TitleCase()}".GetLineWithTitle(actor.LineFormatLength,
				actor.Account.UseUnicode, Telnet.Green, Telnet.Cyan));
			sb.AppendLine();
			var categorySkills = new List<string>();
			foreach (var skill in group.OrderBy(x => x.Definition.Name))
			{
				categorySkills.Add(
					$"{skill.Definition.Name.TitleCase().RawTextPadRight(15)} {(adminMode ? CurrentMaxDecorator.Instance.Decorate(skill) : actor.GetTraitDecorated(skill.Definition))}");
			}

			sb.Append(categorySkills.SplitTextIntoColumns((uint)actor.LineFormatLength / 40,
				(uint)actor.LineFormatLength));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Skills", "skills", "sk", "ski", "skil")]
	[HelpInfo("skills",
		"This command is used to view the current level of your skills. The most basic syntax is to simply type SKILLS with no arguments, which shows you all of your skills and their current level. Additionally, you can filter by categories by using SKILL <CATEGORY>. See the SKILLCATEGORIES command to see what categories of skills you have. Finally, administrators can specify a target for the SKILLS command to see someone else's skills.",
		AutoHelp.HelpArg)]
	protected static void Skills(ICharacter actor, string input)
	{
		var adminMode = actor.IsAdministrator();
		var ss = new StringStack(input.RemoveFirstWord());
		var arg = ss.Pop();
		if (arg.Length == 0)
		{
			actor.OutputHandler.Send("Skills:\n\n" +
			                         actor.Body.Traits
			                              .Where(x => x.Definition.TraitType == TraitType.Skill && !x.Hidden)
			                              .OrderBy(x => x.Definition.Name)
			                              .Select(
				                              x =>
					                              x.Definition.Name.TitleCase().RawTextPadRight(15) +
					                              (adminMode
						                              ? CurrentMaxDecorator.Instance.Decorate(x)
						                              : actor.Body.GetTraitDecorated(x.Definition)))
			                              //.ArrangeStringsOntoLines(3, (uint)actor.LineFormatLength)
			                              .SplitTextIntoColumns((uint)actor.LineFormatLength / 40,
				                              (uint)actor.LineFormatLength, 2)
				, nopage: true
			);
		}
		else
		{
			if (actor.IsAdministrator())
			{
				var target = actor.TargetActor(arg);
				if (target == null)
				{
					actor.Send("You do not see anyone like that to check the skills of.");
					return;
				}

				actor.SendNoPage("{0} Skills:\n\n{1}", target.HowSeen(actor, true, DescriptionType.Possessive),
					target.Body.Traits
					      .Where(x => x.Definition.TraitType == TraitType.Skill && !x.Hidden)
					      .OrderBy(x => x.Definition.Name)
					      .Select(
						      x =>
							      x.Definition.Name.TitleCase().ColourName().RawTextPadRight(12) +
							      (adminMode
								      ? CurrentMaxDecorator.Instance.Decorate(x)
								      : actor.Body.GetTraitDecorated(x.Definition)))
					      .ArrangeStringsOntoLines(Math.Max(1, (uint)actor.LineFormatLength / 30),
						      (uint)actor.LineFormatLength));
				return;
			}

			if (
				actor.Body.Traits.Any(
					x =>
						x.Definition.TraitType == TraitType.Skill &&
						x.Definition.Group.ToLowerInvariant() == arg.ToLowerInvariant()))
			{
				actor.OutputHandler.Send($"Skills for Category ({arg.Proper().ColourName()}):\n\n" +
				                         actor.TraitsOfType(TraitType.Skill)
				                              .Where(x => !x.Hidden && x.Definition.Group.EqualTo(arg))
				                              .OrderBy(x => x.Definition.Name)
				                              .Select(
					                              x =>
						                              x.Definition.Name.TitleCase().ColourName().RawTextPadRight(12) +
						                              actor.Body.GetTraitDecorated(x.Definition)
						                                   .RawTextPadLeft(10)
						                                   .RawTextPadRight(11))
				                              .ArrangeStringsOntoLines(Math.Max(1, (uint)actor.LineFormatLength / 30),
					                              (uint)actor.LineFormatLength)
				);
			}
			else
			{
				actor.OutputHandler.Send("You do not have any skills in the " + arg.Colour(Telnet.Cyan) +
				                         " category.");
			}
		}
	}

	[PlayerCommand("SkillAudit", "skillaudit", "saudit")]
	[HelpInfo("skillaudit",
		"This command is used to view detailed information about your skill levels and their prospects for improvement. The syntax is simply SKILLAUDIT.",
		AutoHelp.HelpArg)]
	protected static void SkillAudit(ICharacter actor, string input)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Full skill audit for {actor.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)}:");
		var difficulties = Enum.GetValues<Difficulty>().ToList();
		foreach (var group in actor.TraitsOfType(TraitType.Skill).Where(x => !x.Hidden).OfType<ISkill>()
		                           .GroupBy(x => x.Definition.Group).OrderBy(x => x.Key))
		{
			sb.AppendLine();
			sb.AppendLine($"Category: {group.Key.TitleCase()}".GetLineWithTitle(actor.LineFormatLength,
				actor.Account.UseUnicode, Telnet.Green, Telnet.Cyan));
			sb.AppendLine();
			foreach (var skill in group.OrderBy(x => x.Definition.Name))
			{
				Difficulty? minDifficultyPractical = null;
				Difficulty? minDifficultyTheory = null;
				foreach (var difficulty in difficulties)
				{
					if (difficulty == Difficulty.Impossible)
					{
						break;
					}

					if (skill.SkillDefinition.Improver.CanImprove(actor, skill, difficulty, TraitUseType.Practical,
						    true))
					{
						minDifficultyPractical = difficulty;
						break;
					}
				}

				foreach (var difficulty in difficulties)
				{
					if (difficulty == Difficulty.Impossible)
					{
						break;
					}

					if (skill.SkillDefinition.Improver.CanImprove(actor, skill, difficulty, TraitUseType.Theoretical,
						    true))
					{
						minDifficultyTheory = difficulty;
						break;
					}
				}

				if (minDifficultyPractical is null)
				{
					if (minDifficultyTheory is null)
					{
						sb.AppendLine($"\t{skill.SkillDefinition.Name.ColourName()} cannot be improved any further.");
						continue;
					}

					sb.AppendLine(
						$"\t{skill.SkillDefinition.Name.ColourName()} can only be improved in theoretical ability at minimum difficulty {minDifficultyTheory.DescribeEnum(true).ColourValue()}.");
					continue;
				}

				if (minDifficultyTheory is null)
				{
					sb.AppendLine(
						$"\t{skill.SkillDefinition.Name.ColourName()} can only be improved in practical ability at minimum difficulty {minDifficultyPractical.DescribeEnum(true).ColourValue()}.");
					continue;
				}

				if (minDifficultyTheory == minDifficultyPractical)
				{
					sb.AppendLine(
						$"\t{skill.SkillDefinition.Name.ColourName()} can only be improved at minimum difficulty {minDifficultyTheory.DescribeEnum(true).ColourValue()}.");
					continue;
				}

				sb.AppendLine(
					$"\t{skill.SkillDefinition.Name.ColourName()} can only be improved at a difficulty of {minDifficultyPractical.DescribeEnum(true).ColourValue()} for practical and {minDifficultyTheory.DescribeEnum(true).ColourValue()} for theory.");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Knowledges", "knowledges")]
	protected static void Knowledges(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = actor;
		if (!ss.IsFinished && actor.IsAdministrator() && ss.Peek()[0] == '*')
		{
			var targetText = ss.Pop().RemoveFirstCharacter();
			target = long.TryParse(targetText, out var value)
				? actor.Gameworld.TryGetCharacter(value, true)
				: actor.TargetActor(targetText);

			if (target == null)
			{
				actor.Send("There is no such character to view knowledges of.");
				return;
			}
		}

		var knowledges = target.CharacterKnowledges;

		if (!ss.IsFinished)
		{
			var type = ss.PopSpeech();
			knowledges =
				knowledges.Where(
					x =>
						x.Knowledge.KnowledgeType.StartsWith(type,
							StringComparison.InvariantCultureIgnoreCase));
		}

		if (!ss.IsFinished)
		{
			var subtype = ss.PopSpeech();
			knowledges =
				knowledges.Where(
					x =>
						x.Knowledge.KnowledgeSubtype.StartsWith(subtype,
							StringComparison.InvariantCultureIgnoreCase));
		}

		actor.OutputHandler.Send($"{target.HowSeen(actor, true)} know the following knowledges:\n\n" +
		                         StringUtilities.GetTextTable(
			                         from knowledge in knowledges
			                         select new[]
			                         {
				                         knowledge.Knowledge.Name.Proper(),
				                         knowledge.Knowledge.Description.Proper(),
				                         knowledge.TimesTaught.ToString("N0", actor)
			                         },
			                         new[]
			                         {
				                         "Name",
				                         "Description",
				                         "Times Taught"
			                         },
			                         actor.LineFormatLength,
			                         colour: Telnet.Green,
			                         truncatableColumnIndex: 1,
			                         unicodeTable: actor.Account.UseUnicode
		                         ) + "\n\nSee " +
		                         "help knowledges".Colour(Telnet.Yellow) +
		                         " for more information.");
	}

	[PlayerCommand("Quirks", "merits", "flaws", "quirks")]
	protected static void Merits(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		var target = actor;
		if (!ss.IsFinished && actor.IsAdministrator())
		{
			target = actor.TargetActor(ss.Pop());
			if (target == null)
			{
				actor.Send("You do not see anyone like that whose quirks you can view.");
				return;
			}
		}

		actor.Send("Quirks for {0}:\n\n{1}", target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf),
			target.Merits
			      .OfType<ICharacterMerit>()
			      .Where(x => x.DisplayInCharacterMeritsCommand(actor))
			      .Select(x =>
				      $"{x.Name.TitleCase()} ({(x.Applies(target) ? "In Effect".Colour(Telnet.Green) : "Inactive".Colour(Telnet.Red))})")
			      .ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n"));
	}

	[PlayerCommand("Score", "score", "sc", "sco", "scor")]
	protected static void Score(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (actor.IsAdministrator() && !ss.IsFinished)
		{
			var targetActor = actor.TargetActor(ss.Pop());
			if (targetActor == null)
			{
				actor.Send("You don't see anybody like that to view the score of.");
				return;
			}

			actor.OutputHandler.Send(targetActor.ShowScore(actor), nopage: true);
			return;
		}

		actor.OutputHandler.Send(actor.ShowScore(actor), nopage: true);
	}

	[PlayerCommand("Attributes", "attributes", "attr")]
	protected static void Attributes(ICharacter actor, string input)
	{
		var who = actor;
		var ss = new StringStack(input.RemoveFirstWord());
		if (!ss.IsFinished && actor.IsAdministrator())
		{
			who = actor.TargetActorOrCorpse(ss.SafeRemainingArgument);
			if (who == null)
			{
				actor.OutputHandler.Send("You don't see anyone like that here.");
				return;
			}
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{(who == actor ? "You have" : $"{who.HowSeen(actor, true)} has")} the following attributes:");
		sb.AppendLine();
		foreach (var attribute in who.Body.Traits.OfType<IAttribute>().OrderBy(x => x.AttributeDefinition.DisplayOrder))
		{
			if (attribute.Hidden && !actor.IsAdministrator())
			{
				continue;
			}

			if (!attribute.AttributeDefinition.ShowInAttributeCommand)
			{
				continue;
			}

			if (attribute.AttributeDefinition.DisplayAsSubAttribute)
			{
				sb.AppendLine(
					$"\t{(actor.Account.UseUnicode ? "→ " : "-> ")}{attribute.Definition.Name.TitleCase().ColourName()} ({attribute.AttributeDefinition.Alias.Proper()}): {attribute.Definition.Decorator.Decorate(attribute)}");
			}
			else
			{
				sb.AppendLine(
					$"\t{attribute.Definition.Name.TitleCase().ColourValue()} ({attribute.AttributeDefinition.Alias.Proper()}): {attribute.Definition.Decorator.Decorate(attribute)}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	internal abstract class TestType
	{
		protected static Difficulty[] AllDifficulties =
		{
			Difficulty.Automatic,
			Difficulty.Trivial,
			Difficulty.ExtremelyEasy,
			Difficulty.VeryEasy,
			Difficulty.Easy,
			Difficulty.Normal,
			Difficulty.Hard,
			Difficulty.VeryHard,
			Difficulty.ExtremelyHard,
			Difficulty.Insane,
			Difficulty.Impossible
		};

		public IFuturemud Gameworld { get; set; }
		public abstract CheckOutcome TestAgainst(ICharacter actor, Difficulty difficulty);

		public abstract IReadOnlyDictionary<Difficulty, CheckOutcome> TestAgainstAllDifficulties(
			ICharacter actor, Difficulty difficulty);

		public abstract string DescribeTest();

		protected Outcome GetOutcome(int successes)
		{
			switch (successes)
			{
				case -3:
					return Outcome.MajorFail;
				case -2:
					return Outcome.Fail;
				case -1:
					return Outcome.MinorFail;
				case 1:
					return Outcome.MinorPass;
				case 2:
					return Outcome.Pass;
				case 3:
					return Outcome.MajorPass;
				default:
					return Outcome.NotTested;
			}
		}
	}

	internal class SkillTest : TestType
	{
		public ITraitDefinition Trait { get; set; }

		#region Overrides of TestType

		public override CheckOutcome TestAgainst(ICharacter actor, Difficulty difficulty)
		{
			return Gameworld.GetCheck(CheckType.GenericSkillCheck).Check(actor, difficulty, Trait);
		}

		public override IReadOnlyDictionary<Difficulty, CheckOutcome> TestAgainstAllDifficulties(ICharacter actor,
			Difficulty difficulty)
		{
			return Gameworld.GetCheck(CheckType.GenericSkillCheck)
			                .CheckAgainstAllDifficulties(actor, difficulty, Trait);
		}

		public override string DescribeTest()
		{
			return $"{Trait.Name.ToLowerInvariant().Colour(Telnet.Cyan)} skill";
		}

		#endregion
	}

	internal class AttributeTest : TestType
	{
		public ITraitDefinition Trait { get; set; }

		#region Overrides of TestType

		public override CheckOutcome TestAgainst(ICharacter actor, Difficulty difficulty)
		{
			return Gameworld.GetCheck(CheckType.GenericAttributeCheck).Check(actor, difficulty, Trait);
		}

		public override IReadOnlyDictionary<Difficulty, CheckOutcome> TestAgainstAllDifficulties(ICharacter actor,
			Difficulty difficulty)
		{
			return Gameworld.GetCheck(CheckType.GenericAttributeCheck)
			                .CheckAgainstAllDifficulties(actor, difficulty, Trait);
		}

		public override string DescribeTest()
		{
			return $"{Trait.Name.ToLowerInvariant().Colour(Telnet.Cyan)} attribute";
		}

		#endregion
	}

	internal class HeightTest : TestType
	{
		private static Expression _testChance;

		public Expression TestChance =>
			_testChance ??= new Expression(Gameworld.GetStaticConfiguration("HeightTestChanceExpression"));

		#region Overrides of TestType

		public override CheckOutcome TestAgainst(ICharacter actor, Difficulty difficulty)
		{
			TestChance.Parameters["difficulty"] = (int)Difficulty.Normal;
			TestChance.Parameters["height"] = actor.Height;
			var origTN = (double)TestChance.Evaluate();
			TestChance.Parameters["difficulty"] = (int)difficulty;
			var targetNumber = (double)TestChance.Evaluate();
			var outcome = GetOutcome(RandomUtilities.ConsecutiveRoll(100, targetNumber, 3, out var rolls));

			var result = new CheckOutcome
			{
				Outcome = outcome,
				CheckType = CheckType.None,
				Rolls = rolls,
				FinalDifficulty = difficulty,
				OriginalDifficulty = difficulty,
				FinalDifficultyModifier = targetNumber - origTN,
				OriginalDifficultyModifier = targetNumber - origTN,
				TargetNumber = targetNumber
			};

			return result;
		}

		public override IReadOnlyDictionary<Difficulty, CheckOutcome> TestAgainstAllDifficulties(ICharacter actor,
			Difficulty difficulty)
		{
			var results = new Dictionary<Difficulty, CheckOutcome>();
			foreach (var thisDifficulty in AllDifficulties)
			{
				results[thisDifficulty] = TestAgainst(actor, thisDifficulty);
			}

			return results;
		}

		public override string DescribeTest()
		{
			return "height";
		}

		#endregion
	}

	internal class WeightTest : TestType
	{
		private static Expression _testChance;

		public Expression TestChance =>
			_testChance ??= new Expression(Gameworld.GetStaticConfiguration("WeightTestChanceExpression"));

		#region Overrides of TestType

		public override CheckOutcome TestAgainst(ICharacter actor, Difficulty difficulty)
		{
			TestChance.Parameters["weight"] = actor.Weight;
			TestChance.Parameters["difficulty"] = (int)Difficulty.Normal;
			var origTN = (double)TestChance.Evaluate();
			TestChance.Parameters["difficulty"] = (int)difficulty;
			var targetNumber = (double)TestChance.Evaluate();
			var outcome = GetOutcome(RandomUtilities.ConsecutiveRoll(100, targetNumber, 3, out var rolls));

			var result = new CheckOutcome
			{
				Outcome = outcome,
				CheckType = CheckType.None,
				Rolls = rolls,
				FinalDifficulty = difficulty,
				OriginalDifficulty = difficulty,
				FinalDifficultyModifier = targetNumber - origTN,
				OriginalDifficultyModifier = targetNumber - origTN,
				TargetNumber = targetNumber
			};

			return result;
		}

		public override IReadOnlyDictionary<Difficulty, CheckOutcome> TestAgainstAllDifficulties(ICharacter actor,
			Difficulty difficulty)
		{
			var results = new Dictionary<Difficulty, CheckOutcome>();
			foreach (var thisDifficulty in AllDifficulties)
			{
				results[thisDifficulty] = TestAgainst(actor, thisDifficulty);
			}

			return results;
		}

		public override string DescribeTest()
		{
			return "weight";
		}

		#endregion
	}

	[PlayerCommand("Test", "test")]
	[HelpInfo("test", "Test skill/attribute against a difficulty, optionally against another character.\nSyntax:\n\t" +
	                  "test <attribute/skill/height/weight> [<difficulty>] [echo]\n\ttest <attribute/skill/height/weight> vs <target> <attribute/skill> [<difficulty>]",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Test(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var experimental = false;
		if (ss.Peek().EqualTo("experimental"))
		{
			experimental = true;
			ss.Pop();
		}

		var cmd = ss.PopSpeech();

		TestType playerTest;
		if (cmd.EqualTo("height"))
		{
			playerTest = new HeightTest { Gameworld = actor.Gameworld };
		}
		else if (cmd.EqualTo("weight"))
		{
			playerTest = new WeightTest { Gameworld = actor.Gameworld };
		}
		else
		{
			var trait =
				actor.Gameworld.Traits.FirstOrDefault(x =>
					x.Name.ToLowerInvariant().StartsWith(cmd.ToLowerInvariant(), StringComparison.Ordinal));
			if (trait == null)
			{
				actor.OutputHandler.Send(StringUtilities.HMark +
				                         "There is no such attribute or skill to test against.");
				return;
			}

			if (trait.TraitType == TraitType.Attribute)
			{
				playerTest = new AttributeTest { Gameworld = actor.Gameworld, Trait = trait };
			}
			else
			{
				playerTest = new SkillTest { Gameworld = actor.Gameworld, Trait = trait };
			}
		}

		Difficulty diff;
		var echo = false;

		cmd = ss.PopSpeech();
		if (cmd.ToLowerInvariant() == "vs")
		{
			echo = true;
			cmd = ss.PopSpeech();
			if (cmd.Length == 0)
			{
				actor.OutputHandler.Send(StringUtilities.HMark + "Who do you wish to test your traits against?");
				return;
			}

			var target = actor.TargetActor(cmd);
			if (target == null)
			{
				actor.OutputHandler.Send(StringUtilities.HMark +
				                         "You do not see anyone such as that to test your traits against.");
				return;
			}

			cmd = ss.Pop();
			if (cmd.Length == 0)
			{
				actor.OutputHandler.Send(StringUtilities.HMark + "What trait of " +
				                         target.HowSeen(actor, type: DescriptionType.Possessive) +
				                         " do you want to test against?");
				return;
			}

			TestType targetTest;
			if (cmd.EqualTo("height"))
			{
				targetTest = new HeightTest { Gameworld = actor.Gameworld };
			}
			else if (cmd.EqualTo("weight"))
			{
				targetTest = new WeightTest { Gameworld = actor.Gameworld };
			}
			else
			{
				var trait =
					actor.Gameworld.Traits.FirstOrDefault(x =>
						x.Name.ToLowerInvariant().StartsWith(cmd.ToLowerInvariant(), StringComparison.Ordinal));
				if (trait == null)
				{
					actor.OutputHandler.Send(StringUtilities.HMark +
					                         "There is no such attribute or skill to test against.");
					return;
				}

				if (trait.TraitType == TraitType.Attribute)
				{
					targetTest = new AttributeTest { Gameworld = actor.Gameworld, Trait = trait };
				}
				else
				{
					targetTest = new SkillTest { Gameworld = actor.Gameworld, Trait = trait };
				}
			}

			cmd = ss.Pop();
			if (cmd.Length > 0)
			{
				if (!CheckExtensions.GetDifficulty(cmd, out diff))
				{
					actor.OutputHandler.Send(StringUtilities.HMark +
					                         "That is not a valid difficulty. Valid difficulties are " +
					                         (from differ in Enum.GetNames(typeof(Difficulty))
					                          select differ.Colour(Telnet.Cyan)).ListToString() + ".");
					return;
				}
			}
			else
			{
				diff = Difficulty.Normal;
			}

			cmd = ss.Pop();
			if (cmd.ToLowerInvariant() == "echo")
			{
				actor.Send(StringUtilities.HMark + "Versus tests are always echoed, to prevent abuse.");
				return;
			}

			var vsOutput =
				$"$0 test|tests &0's {playerTest.DescribeTest()}  against $1's {targetTest.DescribeTest()} at {diff.Describe().ToLowerInvariant().Colour(Telnet.Yellow)} difficulty.";
			ICharacter vsWinner = null;
			CheckOutcome playerOutcome, targetOutcome;
			IReadOnlyDictionary<Difficulty, CheckOutcome> playerOutcomes = new Dictionary<Difficulty, CheckOutcome>();
			IReadOnlyDictionary<Difficulty, CheckOutcome> targetOutcomes = new Dictionary<Difficulty, CheckOutcome>();
			OpposedOutcome outcome;
			if (!experimental)
			{
				playerOutcome = playerTest.TestAgainst(actor, diff);
				targetOutcome = targetTest.TestAgainst(target, diff);
				outcome = new OpposedOutcome(playerOutcome, targetOutcome);
			}
			else
			{
				playerOutcomes = playerTest.TestAgainstAllDifficulties(actor, diff);
				targetOutcomes = targetTest.TestAgainstAllDifficulties(target, diff);
				outcome = new OpposedOutcome(playerOutcomes, targetOutcomes, diff, diff);
				playerOutcome = playerOutcomes[outcome.ProponentDifficulty];
				targetOutcome = targetOutcomes[outcome.OpponentDifficulty];
			}

			if (experimental)
			{
				vsOutput = vsOutput.Append("\n\tThe ties-forbidden result is a " +
				                           outcome.Degree.DescribeColour().ToLowerInvariant() + " success to $2.");
				switch (outcome.Outcome)
				{
					case OpposedOutcomeDirection.Proponent:
						vsWinner = actor;
						break;
					case OpposedOutcomeDirection.Opponent:
						vsWinner = target;
						break;
				}

				var nonExperimentalOutcome = new OpposedOutcome(playerOutcomes[diff], targetOutcomes[diff]);
				switch (nonExperimentalOutcome.Outcome)
				{
					case OpposedOutcomeDirection.Stalemate:
						vsOutput = vsOutput.Append($"\n\tThe regular result is a {"stalemate".Colour(Telnet.Yellow)}");
						break;
					case OpposedOutcomeDirection.Proponent:
						vsOutput = vsOutput.Append(
							$"\n\tThe regular result is a {nonExperimentalOutcome.Degree.DescribeColour().ToLowerInvariant()} success to $0.");
						break;
					case OpposedOutcomeDirection.Opponent:
						vsOutput = vsOutput.Append(
							$"\n\tThe regular result is a {nonExperimentalOutcome.Degree.DescribeColour().ToLowerInvariant()} success to $1.");
						break;
				}
			}
			else
			{
				switch (outcome.Outcome)
				{
					case OpposedOutcomeDirection.Stalemate:
						vsOutput = vsOutput.Append("\n\tThe result is a " + "stalemate".Colour(Telnet.Yellow) +
						                           ".");
						break;
					case OpposedOutcomeDirection.Proponent:
						vsOutput = vsOutput.Append("\n\tThe result is a " +
						                           outcome.Degree.DescribeColour().ToLowerInvariant() +
						                           " success to $2.");
						vsWinner = actor;
						break;
					case OpposedOutcomeDirection.Opponent:
						vsOutput = vsOutput.Append("\n\tThe result is a " +
						                           outcome.Degree.DescribeColour().ToLowerInvariant() +
						                           " success to $2.");
						vsWinner = target;
						break;
				}
			}

			//Now handle output, 'vs' always echo to everyone in the room
			foreach (var witness in actor.Location?.LayerCharacters(actor.RoomLayer).Where(x => !x.IsAdministrator()) ??
			                        Enumerable.Empty<ICharacter>())
			{
				witness.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(
							vsOutput, actor, actor, target, vsWinner)), OutputRange.Personal);
			}

			//Generate admin output
			var vsAdminOutput = new StringBuilder();
			vsAdminOutput.Append(vsOutput);
			vsAdminOutput.Append("\n\n Data for $3:");
			vsAdminOutput.Append("\n\tFinal Outcome: " + playerOutcome.Outcome.DescribeColour() + " at " +
			                     playerOutcome.FinalDifficulty.Describe().Colour(Telnet.Green) + "[" +
			                     Math.Round(playerOutcome.FinalDifficultyModifier, 2).ToString()
			                         .Colour(Telnet.BoldYellow) + "]");
			vsAdminOutput.Append("\n\tRolls vs " +
			                     Math.Round(playerOutcome.TargetNumber, 2).ToString().Colour(Telnet.BoldYellow) +
			                     ": " +
			                     playerOutcome.Rolls.Select(x => Math.Round(x, 2).ToString()
			                                                         .Colour(x <= playerOutcome.TargetNumber
				                                                         ? Telnet.BoldGreen
				                                                         : Telnet.BoldRed))
			                                  .ListToString());
			if (playerOutcome.ActiveBonuses?.Count() > 0)
			{
				vsAdminOutput.Append("\n\tActive Bonuses: " +
				                     playerOutcome.ActiveBonuses.Select(x => x.Item1 + "(" +
				                                                             Math.Round(x.Item2, 2).ToString()
					                                                             .Colour(x.Item2 > 0
						                                                             ? Telnet.BoldGreen
						                                                             : Telnet.BoldRed) + ")")
				                                  .ListToString() +
				                     "\n\tTotal Bonuses: " + Math.Round(playerOutcome.FinalBonus, 2).ToString()
				                                                 .Colour(playerOutcome.FinalBonus > 0
					                                                 ?
					                                                 Telnet.BoldGreen
					                                                 :
					                                                 playerOutcome.FinalBonus < 0
						                                                 ? Telnet.BoldRed
						                                                 : Telnet.BoldYellow));
			}

			vsAdminOutput.Append("\n\n Data for $4:");
			vsAdminOutput.Append("\n\tFinal Outcome: " + targetOutcome.Outcome.DescribeColour() + " at " +
			                     targetOutcome.FinalDifficulty.Describe().Colour(Telnet.Green) + "[" +
			                     Math.Round(playerOutcome.FinalDifficultyModifier, 2).ToString()
			                         .Colour(Telnet.BoldYellow) + "]");
			vsAdminOutput.Append("\n\tRolls vs " +
			                     Math.Round(targetOutcome.TargetNumber, 2).ToString().Colour(Telnet.BoldYellow) +
			                     ": " +
			                     targetOutcome.Rolls.Select(x => Math.Round(x, 2).ToString()
			                                                         .Colour(x <= targetOutcome.TargetNumber
				                                                         ? Telnet.BoldGreen
				                                                         : Telnet.BoldRed))
			                                  .ListToString());
			if (targetOutcome.ActiveBonuses?.Count() > 0)
			{
				vsAdminOutput.Append("\n\tActive Bonuses: " +
				                     targetOutcome.ActiveBonuses.Select(x => x.Item1 + "(" +
				                                                             Math.Round(x.Item2, 2).ToString()
					                                                             .Colour(x.Item2 > 0
						                                                             ? Telnet.BoldGreen
						                                                             : Telnet.BoldRed) + ")")
				                                  .ListToString() +
				                     "\n\tTotal Bonuses: " + Math.Round(targetOutcome.FinalBonus, 2).ToString()
				                                                 .Colour(targetOutcome.FinalBonus > 0
					                                                 ?
					                                                 Telnet.BoldGreen
					                                                 :
					                                                 targetOutcome.FinalBonus < 0
						                                                 ? Telnet.BoldRed
						                                                 : Telnet.BoldYellow));
			}

			//Now send it to any observing admin
			foreach (var admin in actor.Location?.LayerCharacters(actor.RoomLayer).Where(x => x.IsAdministrator()) ??
			                      Enumerable.Empty<ICharacter>())
			{
				admin.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(
							vsAdminOutput.ToString(), actor, actor, target, vsWinner, actor, target)),
					OutputRange.Personal);
			}
		}
		else
		{
			if (cmd.Length > 0)
			{
				if (!CheckExtensions.GetDifficulty(cmd, out diff))
				{
					actor.OutputHandler.Send(StringUtilities.HMark +
					                         "That is not a valid difficulty. Valid difficulties are " +
					                         (from differ in Enum.GetNames(typeof(Difficulty))
					                          select differ.Colour(Telnet.Cyan)).ListToString() + ".");
					return;
				}
			}
			else
			{
				diff = Difficulty.Normal;
			}

			cmd = ss.PopSpeech();
			if (cmd.ToLowerInvariant() == "echo")
			{
				echo = true;
				cmd = ss.Pop();
			}

			CheckOutcome outcome;
			IReadOnlyDictionary<Difficulty, CheckOutcome> allOutcomes = new Dictionary<Difficulty, CheckOutcome>();
			if (experimental)
			{
				allOutcomes = playerTest.TestAgainstAllDifficulties(actor, diff);
				outcome = allOutcomes[diff];
			}
			else
			{
				outcome = playerTest.TestAgainst(actor, diff);
			}

			var outputStr =
				$"@ test|tests against &0's {playerTest.DescribeTest()} at {diff.Describe().ToLowerInvariant().Colour(Telnet.Yellow)} difficulty.";
			outputStr = outputStr.Append("\n\tThe result is a " + outcome.Outcome.DescribeColour().ToLowerInvariant() +
			                             ".");

			//Private echo or echo everyone in the room, admin see either way
			if (!echo && !actor.IsAdministrator())
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(outputStr, actor, actor)), OutputRange.Personal);
			}
			else
			{
				foreach (var witness in actor.Location?.LayerCharacters(actor.RoomLayer)
				                             .Where(x => !x.IsAdministrator()) ?? Enumerable.Empty<ICharacter>())
				{
					witness.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								outputStr, actor, actor)), OutputRange.Personal);
				}
			}

			if (experimental)
			{
				var sb = new StringBuilder();
				foreach (var result in allOutcomes)
				{
					sb.AppendLine(
						$"\t\tAt {result.Key.Describe().Colour(Telnet.Yellow)} the result was {result.Value.Outcome.DescribeColour()}.");
				}

				actor.OutputHandler.Send(sb.ToString());
			}

			//Generate admin output
			var bonusPerDifficulty = Futuremud.Games.First().GetStaticInt("CheckBonusPerDifficultyLevel");
			var bonusAdjustment = (int)(outcome.FinalBonus / bonusPerDifficulty);
			var adminOutput = new StringBuilder();
			adminOutput.Append(
				$"@ test|tests against &0's {playerTest.DescribeTest()} at {diff.Describe().ToLowerInvariant().Colour(Telnet.Yellow)}[" +
				outcome.OriginalDifficultyModifier.ToString()
				       .Colour(outcome.OriginalDifficultyModifier > 0 ? Telnet.BoldGreen :
					       outcome.OriginalDifficultyModifier < 0 ? Telnet.BoldRed : Telnet.BoldYellow) +
				"] difficulty");
			if (outcome.FinalDifficulty != outcome.OriginalDifficulty)
			{
				adminOutput.Append(" which got adjusted to " +
				                   outcome.FinalDifficulty.Describe().Colour(Telnet.Green) + "[" +
				                   outcome.FinalDifficultyModifier.ToString()
				                          .Colour(outcome.FinalDifficultyModifier > 0 ? Telnet.BoldGreen :
					                          outcome.FinalDifficultyModifier < 0 ? Telnet.BoldRed :
					                          Telnet.BoldYellow) +
				                   "]");
			}

			if (echo)
			{
				adminOutput.Append(" (ECHOED)");
			}

			adminOutput.AppendLine("\n\tThe result is a " + outcome.Outcome.DescribeColour().ToLowerInvariant() + ".");

			adminOutput.Append("\tRolls vs " +
			                   Math.Round(outcome.TargetNumber, 2).ToString().Colour(Telnet.BoldYellow) +
			                   ": " +
			                   outcome.Rolls.Select(x => Math.Round(x, 2).ToString()
			                                                 .Colour(x <= outcome.TargetNumber
				                                                 ? Telnet.BoldGreen
				                                                 : Telnet.BoldRed))
			                          .ListToString());

			if (outcome.ActiveBonuses?.Count() > 0)
			{
				adminOutput.Append("\n\tActive Bonuses: " +
				                   outcome.ActiveBonuses.Select(x => x.Item1 +
				                                                     "(" +
				                                                     Math.Round(x.Item2, 2).ToString()
				                                                         .Colour(x.Item2 > 0
					                                                         ? Telnet.BoldGreen
					                                                         : Telnet.BoldRed) + ")").ListToString() +
				                   "\n\tTotal Bonuses: " + Math.Round(outcome.FinalBonus, 2).ToString()
				                                               .Colour(outcome.FinalBonus > 0 ? Telnet.BoldGreen :
					                                               outcome.FinalBonus < 0 ? Telnet.BoldRed :
					                                               Telnet.BoldYellow));
			}

			foreach (var admin in actor.Location?.LayerCharacters(actor.RoomLayer).Where(x => x.IsAdministrator()) ??
			                      Enumerable.Empty<ICharacter>())
			{
				admin.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(
							adminOutput.ToString(), actor, actor)), OutputRange.Personal);
			}
		}
	}

	[PlayerCommand("Plan", "plan")]
	protected static void Plan(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			var planText =
				$"Your short term plan: {actor.ShortTermPlan?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}\nYour long term plan: {actor.LongTermPlan?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}"
					.Wrap(actor.InnerLineFormatLength);
			actor.Send(planText);
			return;
		}

		switch (ss.Pop().ToLowerInvariant())
		{
			case "short":
			case "shortterm":
				if (ss.IsFinished)
				{
					actor.Send("You clear your short term plan.");
					actor.ShortTermPlan = null;
					actor.RemoveAllEffects<RecentlyUpdatedPlan>();
					return;
				}

				if (ss.RemainingArgument.Length > 300)
				{
					actor.Send("Your short term plan may be a maximum of 300 characters in length.");
					return;
				}

				actor.ShortTermPlan = ss.RemainingArgument.Trim().ProperSentences();
				actor.Send("You set your short term plan to: {0}", actor.ShortTermPlan.Colour(Telnet.Green));
				actor.RemoveAllEffects<RecentlyUpdatedPlan>();
				actor.AddEffect(new RecentlyUpdatedPlan(actor), TimeSpan.FromDays(14));
				return;
			case "long":
			case "longterm":
				if (ss.IsFinished)
				{
					actor.Send("You clear your long term plan.");
					actor.LongTermPlan = null;
					actor.RemoveAllEffects<RecentlyUpdatedPlan>();
					return;
				}

				if (ss.RemainingArgument.Length > 300)
				{
					actor.Send("Your long term plan may be a maximum of 300 characters in length.");
					return;
				}

				actor.LongTermPlan = ss.RemainingArgument.Trim().ProperSentences();
				actor.Send("You set your long term plan to: {0}", actor.LongTermPlan.Colour(Telnet.Green));
				actor.RemoveAllEffects<RecentlyUpdatedPlan>();
				actor.AddEffect(new RecentlyUpdatedPlan(actor), TimeSpan.FromDays(14));
				return;
			default:
				if (!actor.IsAdministrator())
				{
					actor.Send("You may either set your short term plan (short), or your long term plan (long).");
					return;
				}

				var target = actor.Gameworld.Characters.GetByPersonalName(ss.Last);
				if (target == null)
				{
					actor.Send("There is nobody like that online for you to view the plans of.");
					return;
				}

				var planText =
					$"{target.HowSeen(actor, true, DescriptionType.Possessive)} plans:\nShort term plan: {target.ShortTermPlan?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}\nLong term plan: {target.LongTermPlan?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}"
						.Wrap(actor.InnerLineFormatLength);
				actor.Send(planText);
				return;
		}
	}

	[PlayerCommand("Dmote", "dmote")]
	protected static void Dmote(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What do you want to set your dmote to? Use {0} to clear your current dmote.",
				"dmote clear".Colour(Telnet.Yellow));
			return;
		}

		if (ss.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.RemoveAllEffects(x => x.GetSubtype<IDescriptionAdditionEffect>()?.PlayerSet ?? false);
			actor.Send("You clear your current dmote.");
			return;
		}

		if (ss.RemainingArgument.Length > 300)
		{
			actor.Send("Your dmote may be a maximum of 300 characters in length.");
			return;
		}

		actor.RemoveAllEffects(x => x.GetSubtype<IDescriptionAdditionEffect>()?.PlayerSet ?? false);
		actor.AddEffect(new DescriptionAddition(actor, ss.SafeRemainingArgument.ProperSentences().Trim(), true, null));
		actor.Send("You set your dmote to: {0}", ss.SafeRemainingArgument.Colour(Telnet.Cyan));
	}

	[PlayerCommand("Undub", "undub")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void UnDub(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who or what do you want to Undub?");
			return;
		}

		var targetDub = actor.Dubs.GetFromItemListByKeyword(ss.Pop(), actor);
		if (targetDub == null)
		{
			actor.Send("You don't have any dubs like that to remove.");
			return;
		}

		actor.Dubs.Remove(targetDub);
		if (targetDub.TargetType == "Character")
		{
			actor.RemoveAlly(targetDub.TargetId);
		}

		using (new FMDB())
		{
			actor.Gameworld.SaveManager.Abort(targetDub);
			actor.Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.Dubs.Find(targetDub.Id);
			if (dbitem != null)
			{
				FMDB.Context.Dubs.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		actor.Send("You will no longer have any dub for {0}.", targetDub.LastDescription);
	}

	[PlayerCommand("Dub", "dub")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Dub(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to dub?");
			return;
		}

		var targetText = ss.Pop();

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What keyword do you wish to dub them?");
			return;
		}

		var keywordText = ss.Pop();
		if (keywordText.Equals("me", StringComparison.InvariantCultureIgnoreCase) ||
		    keywordText.Equals("self", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.Send("That is not a valid dub to give to anybody.");
			return;
		}

		var target = actor.Target(targetText);
		if (target == null)
		{
			actor.OutputHandler.Send("You do not see that person or thing here to dub.");
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You cannot dub yourself.");
			return;
		}

		var targetDub =
			actor.Dubs.FirstOrDefault(x => x.TargetId == target.Id && x.TargetType == target.FrameworkItemType);
		if (targetDub != null)
		{
			if (targetDub.Keywords.Any(x => x.Equals(keywordText, StringComparison.InvariantCultureIgnoreCase)))
			{
				actor.Send("You have already dubbed {0} with that keyword.", target.HowSeen(actor));
				return;
			}

			targetDub.Keywords.Add(keywordText.ToLowerInvariant());
			targetDub.Changed = true;
		}
		else
		{
			using (new FMDB())
			{
				var dbdub = new Models.Dub();
				FMDB.Context.Dubs.Add(dbdub);
				dbdub.CharacterId = actor.Id;
				dbdub.LastDescription = target.HowSeen(actor, colour: false);
				dbdub.LastUsage = DateTime.UtcNow;
				dbdub.Keywords = keywordText;
				dbdub.TargetId = target.Id;
				dbdub.TargetType = target.FrameworkItemType;
				FMDB.Context.SaveChanges();
				actor.Dubs.Add(new Dub(dbdub, actor, actor.Gameworld));
			}
		}

		actor.Send("You dub {0} \"{1}\".", target.HowSeen(actor), keywordText);
	}

	[PlayerCommand("DubName", "dubname")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("dubname",
		"This command allows you to manually set an introduced name for a character. You must first have given them a dub. The syntax is DUBNAME <person> <name>",
		AutoHelp.HelpArgOrNoArg)]
	protected static void DubName(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone like that to give a dub name to.");
			return;
		}

		var dub = actor.Dubs.FirstOrDefault(x => x.TargetId == target.Id && x.TargetType == target.FrameworkItemType);
		if (dub == null)
		{
			actor.Send("You must first dub someone before you can set a dub name for them.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What name do you want to set for {target.HowSeen(actor)}?");
			return;
		}

		dub.IntroducedName = ss.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You update your dub name for {target.HowSeen(actor)} to {dub.IntroducedName.ColourName()}.");
	}

	[PlayerCommand("Dubs", "dubs")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Dubs(ICharacter actor, string command)
	{
		if (!actor.Dubs.Any())
		{
			actor.OutputHandler.Send("You do not have any dubs.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		IEnumerable<IDub> dubs = actor.Dubs;
		while (!ss.IsFinished)
		{
			var filterText = ss.Pop();
			dubs =
				dubs.Where(
					x => x.Keywords.Any(y => y.Equals(filterText, StringComparison.InvariantCultureIgnoreCase)) ||
					     x.LastDescription.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0);
		}

		if (!dubs.Any())
		{
			actor.OutputHandler.Send("You do not have any dubs that match the specified criteria.");
			return;
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from dub in dubs
				select
					new[]
					{
						dub.Keywords.ListToString(conjunction: "", twoItemJoiner: ", "),
						dub.LastDescription,
						//dub.LastUsage.GetLocalDateString(actor),
						dub.IntroducedName ?? "None",
						dub.TargetType == "Character"
							? actor.AllyIDs.Contains(dub.TargetId)
								? actor.TrustedAllyIDs.Contains(dub.TargetId) ? "Trusted" : "Yes"
								: "No"
							: "N/A"
					},
				new[] { "Keywords", "Last Description", "Introduced Name", "Ally?" },
				actor.Account.LineFormatLength,
				truncatableColumnIndex: 1,
				colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	[PlayerCommand("Allies", "allies")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Allies(ICharacter actor, string input)
	{
		if (!actor.AllyIDs.Any())
		{
			actor.Send("You have no allies.");
			return;
		}

		actor.Send(
			StringUtilities.GetTextTable(
				from ally in actor.AllyIDs
				let dub = actor.Dubs.FirstOrDefault(x => x.TargetId == ally && x.TargetType == "Character")
				where dub != null
				select new[]
				{
					dub.Keywords.ListToString(conjunction: "", separator: " "),
					dub.LastDescription,
					actor.TrustedAllyIDs.Contains(ally) ? "Yes" : "No"
				},
				new[] { "Dub", "Last Description", "Trusted?" },
				actor.LineFormatLength, unicodeTable: actor.Account.UseUnicode, colour: Telnet.Green)
		);
	}

	[PlayerCommand("Ally", "ally")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Ally(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("help") || ss.Peek().EqualTo("?"))
		{
			actor.Send(
				$"Use this command to mark someone as an ally. Syntax is {"ally <target> [TRUSTED]".Colour(Telnet.Yellow)}. See also the UNALLY command.");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone like that to declare your ally.");
			return;
		}

		if (actor.Dubs.All(x => x.TargetId != target.Id || x.TargetType != "Character"))
		{
			actor.Send("You must know someone well enough to have given them a dub in order to make them your ally.");
			return;
		}

		var trusted = ss.Peek().EqualTo("trusted");
		if (actor.AllyIDs.Contains(target.Id))
		{
			if (trusted && actor.TrustedAllyIDs.Contains(target.Id))
			{
				actor.Send("{0} is already a trusted ally.", target.HowSeen(actor));
				return;
			}

			actor.SetTrusted(target.Id, true);
			actor.Send("{0} is now a trusted ally.", target.HowSeen(actor));
			actor.Send(
				"Warning: this means you will let them do nearly anything to you without resisting. Be very, very careful with this setting."
					.Colour(Telnet.BoldRed));
			return;
		}

		actor.SetAlly(target.Id);
		actor.SetTrusted(target.Id, trusted);
		actor.Send($"{{0}} is now {(trusted ? "a trusted ally" : "an ally")}.", target.HowSeen(actor));
		if (trusted)
		{
			actor.Send(
				"Warning: this means you will let them do nearly anything to you without resisting. Be very, very careful with this setting."
					.Colour(Telnet.BoldRed));
		}
	}

	[PlayerCommand("Unally", "unally")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Unally(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("help") || ss.Peek().EqualTo("?"))
		{
			actor.Send(
				$"Use this command to remove the ally status of someone who was already an ally. Syntax is {"unally <target>".Colour(Telnet.Yellow)} or {"unally *<dub>".Colour(Telnet.Yellow)}");
			return;
		}

		IDub targetDub;
		if (ss.Peek()[0] == '*')
		{
			targetDub = actor.Dubs.GetFromItemListByKeyword(ss.Pop().RemoveFirstCharacter(), actor);
			if (targetDub == null)
			{
				actor.Send("You don't have anyone dubbed like that to unally.");
				return;
			}
		}
		else
		{
			var target = actor.Target(ss.Pop());
			if (target == null)
			{
				actor.Send("You don't see anyone here like that who you can unally.");
				return;
			}

			targetDub = actor.Dubs.FirstOrDefault(x => x.TargetId == target.Id && x.TargetType == "Character");
			if (targetDub == null || !actor.AllyIDs.Contains(targetDub.TargetId))
			{
				actor.Send("{0} is not someone that you have allied.", target.HowSeen(actor));
				return;
			}
		}

		actor.RemoveAlly(targetDub.TargetId);
		actor.Send("You are no longer allied with {0}", targetDub.LastDescription.ColourCharacter());
	}

	[PlayerCommand("Body", "body")]
	[HelpInfo("body",
		@"This command allows you to see the bodyparts that your body has. Admins can also optionally target someone else to see their bodyparts.

You can use the following options with this command:

	#3body#0 - show all your bodyparts
	#3body limbs#0 - show which limbs you have
	#3body <limb>#0 - shows only bodyparts on the specified limb
	#3body <target>#0 - for admins only, shows another person's bodyparts", AutoHelp.HelpArg)]
	protected static void Body(ICharacter actor, string command)
	{
		var target = actor;
		var ss = new StringStack(command.RemoveFirstWord());
		ILimb limb = null;
		var sb = new StringBuilder();
		if (!ss.IsFinished)
		{
			if (ss.SafeRemainingArgument.EqualTo("limbs"))
			{
				sb.AppendLine($"Your body has the following limbs:\n");
				foreach (var thelimb in actor.Body.Limbs.OrderBy(x => x.LimbType.LimbOrder()))
				{
					sb.AppendLine($"\t{thelimb.Name.ColourName()}");
				}

				actor.OutputHandler.Send(sb.ToString());
				return;
			}

			limb = actor.Body.Limbs.GetByNameOrAbbreviation(ss.SafeRemainingArgument);
			if (limb is null)
			{
				if (actor.IsAdministrator())
				{
					target = actor.TargetActorOrCorpse(ss.PopSpeech());
					if (target == null)
					{
						actor.OutputHandler.Send("You don't see anyone like that.");
						return;
					}
				}
				else
				{
					actor.OutputHandler.Send("You don't have any limbs by that name.");
					return;
				}
			}
		}


		sb.AppendLine(
			$"{(target == actor ? "Your" : target.HowSeen(actor, true, DescriptionType.Possessive))} {(limb is null ? "body" : limb.Name.ToLowerInvariant())} has the following bodyparts:");
		var parts =
			limb is null
				? target.Body.Bodyparts.ToList()
				: target.Body.Bodyparts.Where(x => limb.Parts.Contains(x)).ToList();

		var root =
			limb is null
				? parts.FirstOrDefault(x => x.UpstreamConnection == null)
				: parts.FirstOrDefault(x => limb.RootBodypart == x);

		if (root == null)
		{
			actor.Send(
				"It seems there is something wrong with the way your body is configured...the admins should probably hear about this.");
			return;
		}

		sb.AppendLine();

		void DescribeLevel(IBodypart proto, int level)
		{
			sb.AppendLine(
				$"{new string('\t', level)}{proto.FullDescription().ColourValue()} [{proto.Name.ColourName()}]");
			foreach (var branch in parts.Where(x => x.UpstreamConnection?.CountsAs(proto) == true).ToList())
			{
				DescribeLevel(branch, level + 1);
			}
		}

		DescribeLevel(root, 0);
		var bones = (limb is null
			? target.Body.Bones
			: target.Body.Bones.Where(x =>
				limb.Parts.Any(y => y.BoneInfo.Any(z => z.Key == x && z.Value.IsPrimaryInternalLocation)))).ToArray();
		if ((actor.IsAdministrator() ||
		    actor.Knowledges.Any(x => actor.Gameworld.SurgicalProcedures.Any(y => y.KnowledgeRequired == x))))
		{
			if (bones.Any())
			{
				sb.AppendLine();
				if (target == actor)
				{
					sb.AppendLine(
						$"{(limb is null ? "You" : $"Your {limb.Name.ToLowerInvariant()}")} also {(limb is null ? "have" : "has")} these bones:\n");
				}
				else
				{
					sb.AppendLine($"{target.ApparentGender(actor).Subjective(true)} also has these bones:\n");
				}



				foreach (var bone in bones)
				{
					sb.AppendLine($"\t{bone.FullDescription().ColourValue()} [{bone.Name.ColourName()}]");
				}
			}

			var organs = (limb is null
				? target.Body.Organs
				: target.Body.Organs.Where(x =>
					limb.Parts.Any(y => y.OrganInfo.Any(z => z.Key == x && z.Value.IsPrimaryInternalLocation)))).ToArray();
			if (organs.Any())
			{
				sb.AppendLine();
				if (target == actor)
				{
					sb.AppendLine(
						$"{(limb is null ? "You" : $"Your {limb.Name.ToLowerInvariant()}")} also {(limb is null ? "have" : "has")} these organs:\n");
				}
				else
				{
					sb.AppendLine($"{target.ApparentGender(actor).Subjective(true)} also has these organs:\n");
				}

				foreach (var organ in organs)
				{
					sb.AppendLine($"\t{organ.FullDescription().ColourValue()} [{organ.Name.ColourName()}]");
				}
			}
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("Uncovered", "uncovered")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("uncovered",
		"The uncovered command allows you to view the bodyparts that are uncovered for a particular target, including yourself. The syntax is uncovered <target>.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Uncovered(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anyone like that from whom to view the uncovered bodyparts.");
			return;
		}

		var bodyparts = target.Body.Bodyparts.ToList();
		var coverInfo = target.Body.WornItemsFullInfo.ToList();
		var uncoveredParts = new List<IBodypart>();
		var transparentDictionary = new Dictionary<IBodypart, bool>();

		foreach (var part in bodyparts)
		{
			if (coverInfo.Any(x => x.Wearloc == part && !x.Profile.Transparent))
			{
				continue;
			}

			uncoveredParts.Add(part);
			transparentDictionary[part] = coverInfo.Where(x => x.Wearloc == part).Any();
		}

		if (!uncoveredParts.Any())
		{
			actor.OutputHandler.Send(new EmoteOutput(
				new Emote("@ are|is completely covered up, not even a single bodypart is visible.", target)));
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"The following bodyparts are visible for {target.HowSeen(actor)}:");
		foreach (var part in uncoveredParts.OfType<IBodypartWithInventoryDisplayOrder>().OrderBy(x => x.DisplayOrder))
		{
			sb.AppendLine(
				$"\t{part.FullDescription()}{(transparentDictionary[part] ? " (covered by transparent items)" : "")}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Evaluate", "evaluate")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Evaluate(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualToAny("help", "?"))
		{
			actor.Send(
				$"This command is used to get some information about items in your vicinity. The syntax is evaluate <item>.");
			return;
		}

		var item = actor.TargetItem(ss.Pop());
		if (item == null)
		{
			actor.Send("You don't see anything like that.");
			return;
		}

		actor.Send(item.Evaluate(actor));
	}
}