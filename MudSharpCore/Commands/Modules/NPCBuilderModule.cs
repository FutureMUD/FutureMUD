using MudSharp.Accounts;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character.Name;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework.Revision;
using MudSharp.Framework;
using MudSharp.NPC.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.NPC.AI.Groups;
using MudSharp.NPC;
using MudSharp.PerceptionEngine;

namespace MudSharp.Commands.Modules
{
	internal class NPCBuilderModule : BaseBuilderModule
	{
		private NPCBuilderModule()
		: base("NPCBuilder")
		{
			IsNecessary = true;
		}

		public static NPCBuilderModule Instance { get; } = new();

		#region NPCs

		private static void NPCLoad(ICharacter character, StringStack command)
		{
			var template = long.TryParse(command.PopSpeech(), out var value)
				? character.Gameworld.NpcTemplates.Get(value)
				: character.Gameworld.NpcTemplates.GetByName(command.Last, true);
			if (template == null)
			{
				character.OutputHandler.Send("There is no such NPC Template to load.");
				return;
			}

			if (template.Status != RevisionStatus.Current)
			{
				character.OutputHandler.Send(
					$"NPC Template #{template.Id.ToString("N0", character)}r{template.RevisionNumber.ToString("N0", character)} is in status {template.Status.DescribeColour()}, and so can't be loaded.");
				return;
			}

			var newCharacter = template.CreateNewCharacter(character.Location);
			newCharacter.RoomLayer = character.RoomLayer;
			template.OnLoadProg?.Execute(newCharacter);

			if (newCharacter.Location.IsSwimmingLayer(newCharacter.RoomLayer) && newCharacter.Race.CanSwim)
			{
				newCharacter.PositionState = PositionSwimming.Instance;
			}
			else if (newCharacter.RoomLayer.IsHigherThan(RoomLayer.GroundLevel) && newCharacter.CanFly().Truth)
			{
				newCharacter.PositionState = PositionFlying.Instance;
			}

			character.Location.Login(newCharacter);
			newCharacter.HandleEvent(EventType.NPCOnGameLoadFinished, newCharacter);
		}

		private static void NPCClone(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which NPC Template would you like to clone?");
				return;
			}

			var template = actor.Gameworld.NpcTemplates.GetByIdOrName(command.SafeRemainingArgument);
			if (template is null)
			{
				actor.OutputHandler.Send("There is no such NPC Template.");
				return;
			}

			var newTemplate = template.Clone(actor);
			actor.Gameworld.Add(newTemplate);
			actor.RemoveAllEffects<BuilderEditingEffect<INPCTemplate>>();
			actor.AddEffect(new BuilderEditingEffect<INPCTemplate>(actor) { EditingItem = newTemplate });
			actor.OutputHandler.Send($"You clone {template.EditHeader().ColourName()} into {newTemplate.EditHeader().ColourName()}, which you are now editing.");
		}

		private static void NPCMake(ICharacter character, StringStack command)
		{
			if (command.IsFinished)
			{
				character.Send(
					"You must either specify an ID number of a character to make into a template, or supply a target for someone who is there now.");
				return;
			}

			if (long.TryParse(command.PopSpeech(), out var value))
			{
				var target = character.Gameworld.TryGetCharacter(value, true);
				if (target == null)
				{
					character.OutputHandler.Send("There is no character with that ID number.");
					return;
				}

				if (command.IsFinished)
				{
					character.OutputHandler.Send("What name do you want to give the template that you create?");
					return;
				}

				var template = new SimpleNPCTemplate(character.Gameworld, character.Account,
					target.GetCharacterTemplate(), command.PopSpeech());
				character.Gameworld.Add(template);
				character.RemoveAllEffects<BuilderEditingEffect<INPCTemplate>>();
				character.AddEffect(new BuilderEditingEffect<INPCTemplate>(character) { EditingItem = template });
				character.OutputHandler.Send(
					$"You create a new NPC Template called {template.Name.Colour(Telnet.Cyan)} (#{template.Id}) from character {target.PersonalName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green)}.");
				return;
			}

			var tch = character.TargetActorOrCorpse(command.Last);
			if (tch == null)
			{
				character.OutputHandler.Send("You don't see any such character.");
				return;
			}

			if (command.IsFinished)
			{
				character.OutputHandler.Send("What name do you want to give the template that you create?");
				return;
			}

			var ctemplate = new SimpleNPCTemplate(character.Gameworld, character.Account, tch.GetCharacterTemplate(),
				command.PopSpeech());
			character.Gameworld.Add(ctemplate);
			character.RemoveAllEffects<BuilderEditingEffect<INPCTemplate>>();
			character.AddEffect(new BuilderEditingEffect<INPCTemplate>(character) { EditingItem = ctemplate });
			character.OutputHandler.Send(
				$"You create a new NPC Template called {ctemplate.Name.Colour(Telnet.Cyan)} (#{ctemplate.Id}) from character {tch.PersonalName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green)}.");
		}

		public const string NPCHelp =
			@"The NPC command is used to create, edit and load NPC Templates. NPC stands for #6Non-Player Characters#0. The templates are used to generate characters in the world, which once created are as fully-featured as player characters.

The core syntax to use this command is as follows:

	#3npc edit new simple|variable#0 - creates a new NPC prototype
	#3npc edit <id>#0 - opens prototype with ID for editing
	#3npc edit#0 - shows the currently open NPC. Equivalent to doing NPC SHOW <ID> on it.
	#3npc edit submit#0 - submits the open NPC for review
	#3npc edit close#0 - closes the open NPC
	#3npc edit delete#0 - deletes the open NPC prototype (only if not yet approved)
	#3npc edit obsolete#0 - marks the NPC as obsolete, and no longer loadable
	#3npc show <ID>#0 - shows info about prototype with ID
	#3npc review all|mine|<admin name>|<id>#0 - opens the specified NPC prototypes for review and approval
	#3npc clone <id>#0 - clones an existing prototype to a new one (also opens for editing)
	#3npc set <parameters>#0 - makes a specific edit to an NPC. See NPC SET HELP for more info
	#3npc make <id>|<target>#0 - clones a PC into a simple NPC Template (also opens for editing)
	#3npc load <which>#0 - creates a new NPC character from the specified template
	#3npc instances <template>#0 - lists all NPCs that have the specified template
	#3npc list [<filters>]#0 - lists all NPC prototypes. See below for filters:

		#6all#0 - includes obsolete and non-current revisions
		#6mine#0 - only shows NPCs you personally created
		#6by <account>#0 - only shows NPCs the nominated account created
		#6reviewed <account>#0 - only shows NPCs the nominated account has approved
		#6+<keyword>#0 - only shows NPCs with that keyword in their name or description
		#6-<keyword>#0 - only shows NPCs without that keyword in their name or description
		#6&<ai>#0 - only shows NPCs with that AI attached";

		[PlayerCommand("NPC", "npc")]
		[CommandPermission(PermissionLevel.JuniorAdmin)]
		protected static void NPC(ICharacter character, string command)
		{
			var ss = new StringStack(command.RemoveFirstWord());
			switch (ss.PopSpeech().ToLowerInvariant())
			{
				case "list":
					GenericRevisableList(character, ss, EditableRevisableItemHelper.NpcTemplateHelper);
					break;
				case "view":
				case "show":
					GenericRevisableShow(character, ss, EditableRevisableItemHelper.NpcTemplateHelper);
					break;
				case "edit":
					GenericRevisableEdit(character, ss, EditableRevisableItemHelper.NpcTemplateHelper);
					break;
				case "set":
					GenericRevisableSet(character, ss, EditableRevisableItemHelper.NpcTemplateHelper);
					break;
				case "review":
					GenericReview(character, ss, EditableRevisableItemHelper.NpcTemplateHelper);
					break;
				case "make":
					NPCMake(character, ss);
					break;
				case "load":
					NPCLoad(character, ss);
					break;
				case "clone":
					NPCClone(character, ss);
					break;
				case "instances":
					NPCInstances(character, ss);
					break;
				default:
					character.OutputHandler.Send(NPCHelp.SubstituteANSIColour());
					break;
			}
		}

		private static void NPCInstances(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which template do you want to show NPCs for?");
				return;
			}

			var template = actor.Gameworld.NpcTemplates.GetByIdOrName(ss.SafeRemainingArgument);
			if (template is null)
			{
				actor.OutputHandler.Send("There is no such NPC Template.");
				return;
			}

			var npcs = actor.Gameworld.NPCs.OfType<INPC>().Where(x => x.Template.Id == template.Id).ToList();
			var sb = new StringBuilder();
			sb.AppendLine($"List of NPCs from NPC Template #{template.Id.ToString("N0", actor)}:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				from npc in npcs
				select new List<string>
				{
					npc.Id.ToString("N0", actor),
					npc.PersonalName.GetName(NameStyle.FullName),
					npc.HowSeen(npc, flags: PerceiveIgnoreFlags.IgnoreObscured | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee),
					npc.Location.GetFriendlyReference(actor),
					$"{npc.Template.Id.ToString("N0", actor)}r{npc.Template.RevisionNumber.ToString("N0", actor)} ({npc.Name.ColourName()})"
				},
				new List<string>
				{
					"Id",
					"Name",
					"SDesc",
					"Location",
					"Template"
				},
				actor,
				Telnet.Orange
			));
			actor.OutputHandler.Send(sb.ToString());
		}

		#endregion

		#region Group AI

		protected static string GroupAITemplateHelpText =
			$"The GroupAITemplate command (which can be abbreviated as GAIT) allows you to edit templates for group AIs, which are AIs that control an entire group of NPCs at once. Unlike some other kinds of building commands, these do not use revisions so any changes are immediate. You can use the following options:\n\t{"gait edit new - creates a new group AI Template\n\tgait edit <id> - opens a specified group AI for editing\n\tgait close - closes the currently open template\n\tgait list - lists all group AI templates\n\tgait show - shows the currently open group AI Template\n\tgait show <id> - shows the specified group AI Template\n\tgait clone <which> <newname> - clones an existing group AI Template\n\tgait set <subcommands> - sets the properties of the open group AI Template".ColourCommand()}";

		[PlayerCommand("GroupAITemplate", "groupaitemplate", "gait")]
		[CommandPermission(PermissionLevel.SeniorAdmin)]
		protected static void GroupAITemplate(ICharacter actor, string command)
		{
			var ss = new StringStack(command.RemoveFirstWord());
			switch (ss.PopSpeech().ToLowerInvariant())
			{
				case "list":
					GroupAITemplateList(actor, ss);
					return;
				case "edit":
					GroupAITemplateEdit(actor, ss);
					return;
				case "clone":
					GroupAITemplateClone(actor, ss);
					return;
				case "close":
					GroupAITemplateClose(actor, ss);
					return;
				case "set":
					GroupAITemplateSet(actor, ss);
					return;
				case "show":
				case "view":
					GroupAITemplateView(actor, ss);
					return;
				default:
					actor.OutputHandler.Send(GroupAITemplateHelpText);
					return;
			}
		}

		private static void GroupAITemplateView(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IGroupAITemplate>>().FirstOrDefault()
								   ?.EditingItem;
				if (editing == null)
				{
					actor.OutputHandler.Send("Which Group AI Template do you want to view?");
					return;
				}

				actor.OutputHandler.Send(editing.Show(actor), nopage: true);
				return;
			}

			var item = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.GroupAITemplates.Get(value)
				: actor.Gameworld.GroupAITemplates.GetByName(ss.Last);
			if (item == null)
			{
				actor.OutputHandler.Send("There is no such Group AI Template.");
				return;
			}

			actor.OutputHandler.Send(item.Show(actor), nopage: true);
		}

		private static void GroupAITemplateSet(ICharacter actor, StringStack ss)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IGroupAITemplate>>().FirstOrDefault()
							   ?.EditingItem;
			if (editing == null)
			{
				actor.OutputHandler.Send(
					"You must first open a Group AI Template with the EDIT command before you can do that.");
				return;
			}

			editing.BuildingCommand(actor, ss);
		}

		private static void GroupAITemplateClose(ICharacter actor, StringStack ss)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IGroupAITemplate>>().FirstOrDefault()
							   ?.EditingItem;
			if (editing == null)
			{
				actor.OutputHandler.Send("You are not editing a Group AI Template.");
				return;
			}

			actor.RemoveAllEffects<BuilderEditingEffect<IGroupAITemplate>>();
			actor.OutputHandler.Send("You are no longer editing any Group AI Templates.");
		}

		private static void GroupAITemplateClone(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which Group AI Template do you want to clone?");
				return;
			}

			var original = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.GroupAITemplates.Get(value)
				: actor.Gameworld.GroupAITemplates.GetByName(ss.Last);
			if (original == null)
			{
				actor.OutputHandler.Send("There is no such Group AI Template to clone.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your cloned template?");
				return;
			}

			var nameText = ss.PopSpeech().TitleCase();
			if (actor.Gameworld.GroupAITemplates.Any(x => x.Name.EqualTo(nameText)))
			{
				actor.OutputHandler.Send("There is already a Group AI Template with that name. Names must be unique.");
				return;
			}

			var newItem = new GroupAITemplate((GroupAITemplate)original, nameText);
			actor.RemoveAllEffects<BuilderEditingEffect<IGroupAITemplate>>();
			actor.AddEffect(new BuilderEditingEffect<IGroupAITemplate>(actor) { EditingItem = newItem });
			actor.OutputHandler.Send(
				$"You clone Group AI Template {original.Name.Colour(Telnet.Cyan)} into a new template called {nameText.Colour(Telnet.Cyan)}, which you are now editing.");
		}

		private static void GroupAITemplateEdit(ICharacter actor, StringStack ss)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IGroupAITemplate>>().FirstOrDefault()
							   ?.EditingItem;
			if (ss.IsFinished)
			{
				if (editing != null)
				{
					GroupAITemplateView(actor, ss);
					return;
				}

				actor.OutputHandler.Send("Which Group AI Template do you want to open for editing?");
				return;
			}

			if (ss.PeekSpeech().EqualTo("new"))
			{
				ss.PopSpeech();
				GroupAITemplateNew(actor, ss);
				return;
			}

			var template = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.GroupAITemplates.Get(value)
				: actor.Gameworld.GroupAITemplates.GetByName(ss.Last);
			if (template == null)
			{
				actor.OutputHandler.Send("There is no such Group AI Template.");
				return;
			}

			actor.RemoveAllEffects<BuilderEditingEffect<IGroupAITemplate>>();
			actor.AddEffect(new BuilderEditingEffect<IGroupAITemplate>(actor) { EditingItem = template });
			actor.OutputHandler.Send($"You open Group AI Template {template.Name.Colour(Telnet.Cyan)} for editing.");
		}

		private static void GroupAITemplateNew(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new Group AI Template?");
				return;
			}

			var nameText = ss.PopSpeech().TitleCase();
			if (actor.Gameworld.GroupAITemplates.Any(x => x.Name.EqualTo(nameText)))
			{
				actor.OutputHandler.Send("There is already a Group AI Template with that name. Names must be unique.");
				return;
			}

			var template = new GroupAITemplate(actor.Gameworld, nameText);
			actor.RemoveAllEffects<BuilderEditingEffect<IGroupAITemplate>>();
			actor.AddEffect(new BuilderEditingEffect<IGroupAITemplate>(actor) { EditingItem = template });
			actor.OutputHandler.Send(
				$"You create a new Group AI Template called {nameText.Colour(Telnet.Cyan)}, which you are now editing.");
		}

		private static void GroupAITemplateList(ICharacter actor, StringStack ss)
		{
			actor.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from template in actor.Gameworld.GroupAITemplates
					select new[]
					{
					template.Id.ToString("N0", actor),
					template.Name,
					template.GroupAIType.Name,
					actor.Gameworld.GroupAIs.Count(x => x.Template == template).ToString("N0", actor),
					template.GroupEmotes.Count().ToString("N0", actor)
					},
					new[] { "Id", "Name", "Type", "Active Groups", "Emote Count" },
					actor.LineFormatLength,
					colour: Telnet.Green,
					unicodeTable: actor.Account.UseUnicode
				)
			);
		}

		#endregion

		#region Groups

		[PlayerCommand("Groups", "groups")]
		[CommandPermission(PermissionLevel.JuniorAdmin)]
		protected static void Groups(ICharacter actor, string input)
		{
			var ss = new StringStack(input.RemoveFirstWord());
			List<IGroupAI> groups;
			if (ss.IsFinished)
			{
				groups = actor.Gameworld.GroupAIs.ToList();
				actor.OutputHandler.Send($"All Group AIs:");
			}
			else
			{
				var template = long.TryParse(ss.PopSpeech(), out var value)
					? actor.Gameworld.GroupAITemplates.Get(value)
					: actor.Gameworld.GroupAITemplates.GetByName(ss.Last);
				if (template == null)
				{
					actor.OutputHandler.Send("There is no such Group AI Template for you to filter groups by.");
					return;
				}

				groups = actor.Gameworld.GroupAIs.Where(x => x.Template == template).ToList();
				actor.OutputHandler.Send($"Groups for the {template.Name.Colour(Telnet.Cyan)} Group AI Template:");
			}

			actor.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from item in groups
					select new[]
					{
					item.Id.ToString("N0", actor),
					item.Name.TitleCase(),
					item.GroupMembers.Count().ToString("N0", actor),
					item.CurrentAction.DescribeEnum(),
					item.Alertness.DescribeEnum(),
					item.GroupRoles.Values.Distinct().Select(x =>
							$"{item.GroupRoles.Count(y => y.Value == x).ToString("N0", actor)}{x.DescribeEnum()[0]}")
						.ListToString(),
					item.GroupRoles.FirstOrDefault(x => x.Value == GroupRole.Leader).Key?.Location?.Id
						.ToString("N0", actor) ?? "N/A",
					item.Template.Name
					},
					new[] { "Id", "Name", "Members", "Priority", "Alertness", "Breakdown", "Leader Location", "Template" },
					actor.LineFormatLength,
					colour: Telnet.Green,
					truncatableColumnIndex: 5,
					unicodeTable: actor.Account.UseUnicode
				)
			);
		}

		private static string GroupCommandHelp =
			$"This command is used to create and manage AI groups, which are special types of AI that control entire groups of NPCs at once. You can use the following sub-commands:\n\t{"group new <template> <name> - creates a new group AI from the specified template\n\tgroup delete <which> - deletes a group AI\n\tgroup show <which> - shows a group AI's current status\n\tgroup addmember <which> <who> - adds the specified NPC to the specified group AI\n\tgroup removemember <which> <who> - removes the specified NPC from the specified group AI\n\tgroup setaction <which> <action> - overrides the current action priority of a group\n\tgroup setalertness <which> <alertness> - overrides the alertness level of a group".ColourCommand()}";

		[PlayerCommand("Group", "group")]
		[CommandPermission(PermissionLevel.JuniorAdmin)]
		protected static void Group(ICharacter actor, string input)
		{
			var ss = new StringStack(input.RemoveFirstWord());
			switch (ss.PopSpeech().ToLowerInvariant())
			{
				case "new":
					GroupNew(actor, ss);
					return;
				case "delete":
					GroupDelete(actor, ss);
					return;
				case "show":
					GroupShow(actor, ss);
					return;
				case "addmember":
					GroupAddMember(actor, ss);
					return;
				case "removemember":
					GroupRemoveMember(actor, ss);
					return;
				case "setaction":
					GroupSetAction(actor, ss);
					return;
				case "setalertness":
					GroupSetAlertness(actor, ss);
					return;
				default:
					actor.OutputHandler.Send(GroupCommandHelp);
					return;
			}
		}

		private static void GroupSetAlertness(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("You must specify the ID or name of a Group AI.");
				return;
			}

			var group = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.GroupAIs.Get(value)
				: actor.Gameworld.GroupAIs.GetByName(ss.Last);

			if (group == null)
			{
				actor.OutputHandler.Send("There is no such Group AI.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					$"What alertness level do you want to set for this group? The valid options are: {Enum.GetValues(typeof(GroupAlertness)).OfType<GroupAlertness>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
				return;
			}

			if (!ss.PopSpeech().TryParseEnum<GroupAlertness>(out var alertness))
			{
				actor.OutputHandler.Send(
					$"That is not a valid alertness level. The valid options are: {Enum.GetValues(typeof(GroupAlertness)).OfType<GroupAlertness>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
				return;
			}

			group.Alertness = alertness;
			actor.OutputHandler.Send(
				$"You set the alertness level for group #{group.Id.ToString("N0", actor)} ({group.Name}) to {alertness.DescribeEnum().ColourValue()}.");
		}

		private static void GroupSetAction(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("You must specify the ID or name of a Group AI.");
				return;
			}

			var group = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.GroupAIs.Get(value)
				: actor.Gameworld.GroupAIs.GetByName(ss.Last);

			if (group == null)
			{
				actor.OutputHandler.Send("There is no such Group AI.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					$"What current action do you want to set for this group? The valid options are: {Enum.GetValues(typeof(GroupAction)).OfType<GroupAction>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
				return;
			}

			if (!ss.PopSpeech().TryParseEnum<GroupAction>(out var action))
			{
				actor.OutputHandler.Send(
					$"That is not a valid action. The valid options are: {Enum.GetValues(typeof(GroupAction)).OfType<GroupAction>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
				return;
			}

			group.CurrentAction = action;
			actor.OutputHandler.Send(
				$"You set the current action for group #{group.Id.ToString("N0", actor)} ({group.Name}) to {action.DescribeEnum().ColourValue()}.");
		}

		private static void GroupRemoveMember(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("You must specify the ID or name of a Group AI.");
				return;
			}

			var group = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.GroupAIs.Get(value)
				: actor.Gameworld.GroupAIs.GetByName(ss.Last);

			if (group == null)
			{
				actor.OutputHandler.Send("There is no such Group AI.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which NPC did you want to remove from the group?");
				return;
			}

			ICharacter target = null;
			if (long.TryParse(ss.PopSpeech(), out value))
			{
				target = group.GroupMembers.FirstOrDefault(x => x.Id == value);
			}
			else
			{
				target = actor.TargetActor(ss.Last) as INPC;
			}

			if (target == null)
			{
				actor.OutputHandler.Send("There isn't any NPC with that ID or an NPC with that keyword locally.");
				return;
			}

			group.RemoveFromGroup(target);
			actor.OutputHandler.Send(
				$"You remove {target.HowSeen(actor)} from the Group AI {group.Name.Colour(Telnet.Cyan)}.");
		}

		private static void GroupAddMember(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("You must specify the ID or name of a Group AI.");
				return;
			}

			var group = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.GroupAIs.Get(value)
				: actor.Gameworld.GroupAIs.GetByName(ss.Last);

			if (group == null)
			{
				actor.OutputHandler.Send("There is no such Group AI.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which NPC did you want to add to the group?");
				return;
			}

			ICharacter target = null;
			if (long.TryParse(ss.PopSpeech(), out value))
			{
				target = actor.Gameworld.NPCs.Get(value);
			}
			else
			{
				target = actor.TargetActor(ss.Last) as INPC;
			}

			if (target == null)
			{
				actor.OutputHandler.Send("There isn't any NPC with that ID or an NPC with that keyword locally.");
				return;
			}

			if (group.GroupMembers.Contains(target))
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true)} is already a member of the Group AI {group.Name.Colour(Telnet.Cyan)}.");
				return;
			}

			group.AddToGroup(target);
			actor.OutputHandler.Send($"You add {target.HowSeen(actor)} to the Group AI {group.Name.Colour(Telnet.Cyan)}.");
		}

		private static void GroupShow(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("You must specify the ID or name of a Group AI.");
				return;
			}

			var group = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.GroupAIs.Get(value)
				: actor.Gameworld.GroupAIs.GetByName(ss.Last);

			if (group == null)
			{
				actor.OutputHandler.Send("There is no such Group AI.");
				return;
			}

			actor.OutputHandler.Send(group.Show(actor));
		}

		private static void GroupDelete(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("You must specify the ID or name of a Group AI.");
				return;
			}

			var group = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.GroupAIs.Get(value)
				: actor.Gameworld.GroupAIs.GetByName(ss.Last);

			if (group == null)
			{
				actor.OutputHandler.Send("There is no such Group AI.");
				return;
			}

			actor.AddEffect(new Accept(actor, new GenericProposal
			{
				AcceptAction = text =>
				{
					actor.OutputHandler.Send($"You delete Group AI {group.Name.Colour(Telnet.Cyan)}.");
					group.Delete();
				},
				RejectAction = text =>
				{
					actor.OutputHandler.Send($"You decide not to delete Group AI {group.Name.Colour(Telnet.Cyan)}.");
				},
				ExpireAction = () =>
				{
					actor.OutputHandler.Send($"You decide not to delete Group AI {group.Name.Colour(Telnet.Cyan)}.");
				},
				Keywords = new List<string> { "delete", "group", "ai" },
				DescriptionString = $"Deleting Group AI {group.Name}"
			}), TimeSpan.FromSeconds(120));
			actor.OutputHandler.Send(
				$"Are you sure you want to delete the Group AI {group.Name.Colour(Telnet.Cyan)}? This is an irreversible action.\n{Accept.StandardAcceptPhrasing}");
		}

		private static void GroupNew(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which Group AI Template did you want to base your new Group AI on?");
				return;
			}

			var template = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.GroupAITemplates.Get(value)
				: actor.Gameworld.GroupAITemplates.GetByName(ss.Last);
			if (template == null)
			{
				actor.OutputHandler.Send("There is no such Group AI Template.");
				return;
			}

			var (success, error) = template.IsValidForCreatingGroups;
			if (!success)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name did you want to give to your new Group AI?");
				return;
			}

			var name = ss.PopSpeech().TitleCase();
			if (actor.Gameworld.GroupAIs.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a Group AI with that name. Names must be unique.");
				return;
			}

			var group = new GroupAI(template, name);
			actor.OutputHandler.Send(
				$"You create a new Group AI called {name.Colour(Telnet.Cyan)}, with ID #{group.Id.ToString("N0", actor)}.");
		}

		#endregion
		
		#region NPC Spawners

	public const string NPCSpawnerHelp =
		$@"The NPCSpawner command is used to view, create and edit NPC Spawners. NPC Spawners monitor zones for populations of NPCs and when they dip below target levels they load more in.

The following options are available:

	#3npcspawner list [all|mine|+key|-key]#0 - lists all NPC Spawners
	#3npcspawner edit <id|name>#0 - opens the specified NPC Spawner for editing
	#3npcspawner edit new <name>#0 - creates a new NPC Spawner for editing
	#3npcspawner edit#0 - equivalent of doing SHOW on your currently editing NPC Spawner
	#3npcspawner clone <id|name> <new name>#0 - creates a carbon copy of a NPC Spawner for editing
	#3npcspawner show <id|name>#0 - shows a particular NPC Spawner.
	#3npcspawner set <subcommand>#0 - changes something about the NPC Spawner. See its help for more info.
	#3npcspawner edit submit#0 - submits a NPC Spawner for review
	#3npcspawner review all|mine|<id|name>#0 - reviews a submitted NPC Spawner
	#3npcspawner review list#0 - shows all NPC Spawner submitted for review

{GenericReviewableSearchList}";

		[PlayerCommand("NPCSpawner", "npcspawner", "spawner")]
		[CommandPermission(PermissionLevel.Admin)]
		protected static void NPCSpawner(ICharacter actor, string command)
		{
			GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.NPCSpawnerHelper);
		}

		#endregion

		#region AI

		public const string AIHelp = @"This command is used to work with and edit artificial intelligences.

The core syntax is as follows:

    #3ai list#0 - shows all AIs
    #3ai edit new <type> <name>#0 - creates a new AI
    #3ai clone <old> <new>#0 - clones an existing AI
    #3ai edit <which>#0 - begins editing a AI
    #3ai close#0 - closes an editing AI
    #3ai show <which>#0 - shows builder information about a resource
    #3ai show#0 - shows builder information about the currently edited resource
    #3ai edit#0 - an alias for AI show (with no args)
    #3ai set ...#0 - edits the properties of a AI. See #3AI set ?#0 for more info.
	#3ai add <ai> <npc>#0 - adds an AI routine to an NPC in the gameworld
	#3ai remove <ai> <npc>#0 - removes an AI routine from an NPC in the gameworld
	#3ai npclist <which>#0 - shows all NPCs who have the AI in question running

The following options are available as filters with the #3list#0 subcommand:

	#6+<keyword>#0 - only show AIs with the keyword in the name
	#6-<keyword>#0 - only show AIs without the keyword in the name";

		[PlayerCommand("AI", "ai")]
		[CommandPermission(PermissionLevel.Admin)]
		[HelpInfo("ai", AIHelp, AutoHelp.HelpArgOrNoArg)]
		protected static void AI(ICharacter actor, string input)
		{
			var ss = new StringStack(input.RemoveFirstWord());
			switch (ss.PopForSwitch())
			{
				case "add":
				case "attach":
					AIAdd(actor, ss);
					return;
				case "remove":
				case "rem":
				case "detatch":
				case "detach":
					AIRemove(actor, ss);
					return;
				case "npclist":
				case "instances":
					AINPCList(actor, ss);
					return;
			}

			GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.AIHelper);
		}

		private static void AINPCList(ICharacter actor, StringStack ss)
		{
            if (ss.IsFinished)
            {
				actor.OutputHandler.Send("Which AI do you want to show NPCs for?");
				return;
            }

			var ai = actor.Gameworld.AIs.GetByIdOrName(ss.SafeRemainingArgument);
			if (ai is null)
			{
				actor.OutputHandler.Send("There is no such AI.");
				return;
			}

			var npcs = actor.Gameworld.NPCs.OfType<INPC>().Where(x => x.AIs.Contains(ai)).ToList();
			var sb = new StringBuilder();
			sb.AppendLine($"List of NPCs with the {ai.Name.ColourName()} AI attached:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				from npc in npcs
				select new List<string>
				{
					npc.Id.ToString("N0", actor),
					npc.PersonalName.GetName(NameStyle.FullName),
					npc.HowSeen(npc, flags: PerceiveIgnoreFlags.IgnoreObscured | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee),
					npc.Location.GetFriendlyReference(actor),
					$"{npc.Template.Id.ToString("N0", actor)}r{npc.Template.RevisionNumber.ToString("N0", actor)} ({npc.Name.ColourName()})"
				},
				new List<string>
				{
					"Id",
					"Name",
					"SDesc",
					"Location",
					"Template"
				},
				actor,
				Telnet.Orange
			));
			actor.OutputHandler.Send(sb.ToString());
		}

		private static void AIRemove(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which AI did you want to remove from an NPC?");
				return;
			}

			var ai = actor.Gameworld.AIs.GetByIdOrName(ss.PopSpeech());
			if (ai is null)
			{
				actor.OutputHandler.Send("There is no such AI.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send($"Which local NPC do you want to remove the {ai.Name.ColourName()} from?");
				return;
			}

			var target = actor.TargetActor(ss.SafeRemainingArgument);
			if (target is null)
			{
				actor.OutputHandler.Send("There is nobody like that here.");
				return;
			}

			if (target is not INPC npc)
			{
				actor.OutputHandler.Send($"Unfortunately {target.HowSeen(actor)} is not an NPC, and only NPCs may have AI routines.");
				return;
			}

			if (!npc.AIs.Contains(ai))
			{
				actor.OutputHandler.Send($"The NPC {target.HowSeen(actor)} does not have the {ai.Name.ColourName()} AI routine.");
				return;
			}

			npc.RemoveAI(ai);
			actor.OutputHandler.Send($"You remove the {ai.Name.ColourName()} AI from NPC {target.HowSeen(actor)}.");
		}

		private static void AIAdd(ICharacter actor, StringStack ss)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which AI did you want to add to an NPC?");
				return;
			}

			var ai = actor.Gameworld.AIs.GetByIdOrName(ss.PopSpeech());
			if (ai is null)
			{
				actor.OutputHandler.Send("There is no such AI.");
				return;
			}

			if (!ai.IsReadyToBeUsed)
			{
				actor.OutputHandler.Send("That AI has building issues and is not ready to be used.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send($"Which local NPC do you want to add the {ai.Name.ColourName()} to?");
				return;
			}

			var target = actor.TargetActor(ss.SafeRemainingArgument);
			if (target is null)
			{
				actor.OutputHandler.Send("There is nobody like that here.");
				return;
			}

			if (target is not INPC npc)
			{
				actor.OutputHandler.Send($"Unfortunately {target.HowSeen(actor)} is not an NPC, and only NPCs may have AI routines.");
				return;
			}

			if (npc.AIs.Contains(ai))
			{
				actor.OutputHandler.Send($"The NPC {target.HowSeen(actor)} already has the {ai.Name.ColourName()} AI routine.");
				return;
			}

			npc.AddAI(ai);
			actor.OutputHandler.Send($"You add the {ai.Name.ColourName()} AI to NPC {target.HowSeen(actor)}.");
		}
		#endregion
	}
}
