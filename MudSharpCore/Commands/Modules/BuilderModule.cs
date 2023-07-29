using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Commands.Helpers;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Groups;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Body.Disfigurements;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.NPC.AI.Groups;
using MudSharp.NPC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Accounts;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.Commands.Trees;
using MudSharp.Communication.Language;
using MudSharp.Economy;
using MudSharp.FutureProg;
using MudSharp.RPG.Knowledge;
using MudSharp.Work.Butchering;

namespace MudSharp.Commands.Modules;

internal class BuilderModule : BaseBuilderModule
{
	private BuilderModule()
		: base("Builder")
	{
		IsNecessary = true;
	}

	public static BuilderModule Instance { get; } = new();
	
	#region Item Prototypes

	[PlayerCommand("Item", "item")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("item",
		@"The ITEM command is used to edit, view and load item prototypes. The editing sub-commands are all done on whichever item prototype the builder currently has open for editing. 

The valid sub-commands and their syntaxes are as follows:

	#3item edit new#0 - creates a new item prototype
	#3item edit <id>#0 - opens prototype with ID for editing
	#3item edit#0 - shows the currently open item. Equivalent to doing ITEM SHOW <ID> on it.
	#3item edit submit#0 - submits the open item for review
	#3item edit close#0 - closes the open item
	#3item edit delete#0 - deletes the open item prototype (only if not yet approved)
	#3item edit obsolete#0 - marks the item as obsolete, and no longer loadable
	#3item show <ID>#0 - shows info about prototype with ID
	#3item review all|mine|<admin name>|<id>#0 - opens the specified item prototypes for review and approval
	#3item clone <id>#0 - clones an existing prototype to a new one (also opens for editing)
	#3item set <parameters>#0 - makes a specific edit to an item. See ITEM SET HELP for more info

	#3item list [<filters>]#0 - lists all item prototypes. See below for filters.
		#6all#0 - includes obsolete and non-current revisions
		#6mine#0 - only shows items you personally created
		#6by <account>#0 - only shows items the nominated account created
		#6reviewed <account>#0 - only shows items the nominated account has approved
		#6+<keyword>#0 - only shows items with the nominated keyword
		#6-<keyword>#0 - excludes items with the nominated keyword
		#6<type>#0 - shows only items that have a component of the specified type
		#6*<tag>#0 - shows only items that 'are' the specified tag

	#3item load [<quantity>] <id> [<extra args>]#0 - loads an item into the game. See below for extra args:
        #6variable=value|id#0 - sets a specific variable
        #6variable=""value""#0 - same as above, for variable values with spaces
        #6*<skin name|id>#0 - sets a skin for the item. Must be the first extra argument",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Item(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "review":
				GenericReview(actor, ss, EditableRevisableItemHelper.GameItemHelper);
				break;
			case "edit":
				GenericRevisableEdit(actor, ss, EditableRevisableItemHelper.GameItemHelper);
				break;
			case "set":
				GenericRevisableSet(actor, ss, EditableRevisableItemHelper.GameItemHelper);
				break;
			case "list":
				GenericRevisableList(actor, ss, EditableRevisableItemHelper.GameItemHelper);
				break;
			case "show":
				GenericRevisableShow(actor, ss, EditableRevisableItemHelper.GameItemHelper);
				break;
			case "load":
				Item_Load(actor, ss);
				break;
			case "clone":
				Item_Clone(actor, ss);
				break;
			default:
				actor.OutputHandler.Send("That is not a valid usage of the item command.");
				break;
		}
	}

	private static void Item_Clone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which item prototype do you want to clone?");
			return;
		}

		var proto = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.ItemProtos.Get(value)
			: actor.Gameworld.ItemProtos.GetByName(ss.SafeRemainingArgument, true);
		if (proto == null)
		{
			actor.Send("There is no such item prototype for you to clone.");
			return;
		}

		var newProto = proto.Clone(actor);
		actor.Gameworld.Add(newProto);
		actor.Send(
			$"You create a clone of item {proto.Id}r{proto.RevisionNumber} ({proto.FullDescription}).\nThe new item has an ID of {newProto.Id}, and you are now editing it.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IGameItemProto>>());
		actor.AddEffect(new BuilderEditingEffect<IGameItemProto>(actor) { EditingItem = newProto });
	}
	#endregion

	#region Item Component Prototypes

	[PlayerCommand("Component", "component", "comp")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("component",
		@"The COMPONENT command is used to edit and view item components. The editing subcommands are all done on whichever item component the builder currently has open for editing. 

The valid subcommands and their syntaxes are as follows:

	comp edit new <type> - creates a new component of the specified type
	comp types - lists the available component types
    comp types +<keyword>|-<keyword> - lists the available component types with keyword filters
    comp typehelp <type> - shows the COMP SET HELP for the requested type
	comp edit <id> - opens component with ID for editing
	comp edit - shows the currently open component. Equivalent to doing COMP SHOW <ID> on it.
	comp edit submit - submits the open component for review
	comp edit close - closes the currently open component
	comp edit delete - deletes the current component (only if not approved)
	comp edit obsolete - marks the current component as obsolete and no longer usable
	comp show <ID> - shows info about component with ID
	comp review all|mine|<admin name>|<id> - opens the specified components for review and approval
	comp set <parameters> - makes a specific edit to a component. See COMP SET HELP for more info
	comp list [<filters>] - lists all item components. See below for filters.

		all - includes obsolete and non-current revisions
		mine - only shows components you personally created
		by <account> - only shows components the nominated account created
		reviewed <account> - only shows components the nominated account has approved
		+<keyword> - only shows components with the nominated keyword
		-<keyword> - excludes components with the nominated keyword
		<type> - shows only components of the specified type

Note: The following two subcommands take a long time and can sometimes cause issues with performance afterwards because of the aggressive loading of things that they do. You should generally plan to reboot the MUD immediately after running these.

    comp update - updates all items with obsolete components or revisions to their current versions
    comp update all - same as comp update, but also loads all characters so that their inventories are included",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Component(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "review":
				Component_Review(actor, ss);
				break;
			case "edit":
				Component_Edit(actor, ss);
				break;
			case "set":
				Component_Set(actor, ss);
				break;
			case "list":
				Component_List(actor, ss);
				break;
			case "show":
				Component_Show(actor, ss);
				break;
			case "types":
				Component_Types(actor, ss);
				break;
			case "typehelp":
				Component_TypeHelp(actor, ss);
				break;
			case "update":
				Component_Update(actor, ss);
				break;
			default:
				actor.OutputHandler.Send("That is not a valid usage of the component command.");
				break;
		}
	}

	private static void Component_Update(ICharacter actor, StringStack input)
	{
		var includeOffline = input.Pop().Equals("all", StringComparison.InvariantCultureIgnoreCase);
		actor.Send(
			"Warning: This command can take a very long time to run, particularly if you use the \"all\" option. Are you sure you want to do this right now? Type ACCEPT to begin.");
		actor.AddEffect(new Accept(actor, new GenericProposal(
			text =>
			{
				actor.Gameworld.SystemMessage(
					"A maintenance task is due to begin in 30 seconds that may run for a long time, and cause the game to appear to hang.");
				actor.Gameworld.Scheduler.AddSchedule(new Schedule(() =>
				{
					actor.Gameworld.SystemMessage(
						"The long-running maintenance task has begun.");
					actor.Gameworld.ForceOutgoingMessages();
					actor.Gameworld.SaveManager.Flush();
					var count = 0;
					using (new FMDB())
					{
						foreach (var item in actor.Gameworld.ItemProtos)
						{
							if (item.CheckForComponentPrototypeUpdates())
							{
								count++;
							}
						}

						FMDB.Context.SaveChanges();
					}

					actor.Gameworld.SystemMessage($"Updated {count} item prototypes.",
						true);
					actor.Gameworld.ForceOutgoingMessages();

					var items = new List<IGameItem>();
					using (new FMDB())
					{
						if (includeOffline)
						{
							var dbitems = FMDB
							              .Context.GameItems.AsNoTracking()
							              .Select(x => x.Id).ToList();
							actor.Gameworld.PrimeGameItems();
							foreach (var id in dbitems)
							{
								items.Add(actor.Gameworld.TryGetItem(id, true));
							}

							actor.Gameworld.ReleasePrimedGameItems();
						}
						else
						{
							items = actor.Gameworld.Items.ToList();
						}
					}

					count = 0;
					using (new FMDB())
					{
						foreach (var item in items)
						{
							if (item.CheckPrototypeForUpdate())
							{
								count++;
							}
						}

						FMDB.Context.SaveChanges();
					}

					actor.Gameworld.SaveManager.Flush();
					actor.Gameworld.SystemMessage($"Updated {count} of {items.Count} total items.", true);
					actor.Gameworld.SystemMessage(
						"The long-running maintenance task has now completed.");
				}, ScheduleType.System, TimeSpan.FromSeconds(30), "Component Update"));
			},
			text => { actor.Send("You decide against updating the components."); },
			() => { actor.Send("You decide against updating the components."); },
			"Updating components",
			"components",
			"updates"
		)), TimeSpan.FromSeconds(120));
	}

	private static void Component_Types(ICharacter actor, StringStack input)
	{
		var types = actor.Gameworld.GameItemComponentManager.TypeHelpInfo.ToList();
		while (!input.IsFinished)
		{
			var txt = input.PopSpeech();
			switch (txt[0])
			{
				case '+':
					types = types.Where(x =>
						x.Blurb.Contains(txt.Substring(1), StringComparison.InvariantCultureIgnoreCase)).ToList();
					break;
				case '-':
					types = types.Where(x =>
						!x.Blurb.Contains(txt.Substring(1), StringComparison.InvariantCultureIgnoreCase)).ToList();
					break;
				default:
					actor.OutputHandler.Send(
						"If you provide any filters, they must begin with + or - to include or exclude respectively.");
					return;
			}
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in types
			select new List<string>
			{
				item.Name,
				item.Blurb
			},
			new List<string>
			{
				"Component",
				"Summary"
			},
			actor.LineFormatLength,
			unicodeTable: actor.Account.UseUnicode,
			truncatableColumnIndex: 1
		));
	}

	private static void Component_TypeHelp(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which type do you want to show the type help for?");
			return;
		}

		var which = input.SafeRemainingArgument;
		var type = actor.Gameworld.GameItemComponentManager.TypeHelpInfo.FirstOrDefault(x => x.Name.EqualTo(which));
		if (string.IsNullOrEmpty(type.Help))
		{
			actor.OutputHandler.Send(
				"That is not a valid component type, or that component type doesn't yet have a help entry.");
			return;
		}

		actor.OutputHandler.Send($@"Help for {type.Name.ColourName()} components:
Summary: {type.Blurb}

Help:

{type.Help}");
	}

	private static void Component_Review(ICharacter actor, StringStack input)
	{
		var cmd = input.Pop().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			Component_Review_List(actor, input);
			return;
		}

		switch (cmd)
		{
			case "list":
				Component_Review_List(actor, input);
				break;
			case "history":
				Component_Review_History(actor, input);
				break;
			case "all":
				Component_Review_All(actor, input);
				break;
			default:
				Component_Review_Default(actor, input);
				break;
		}
	}

	private static void Component_Review_List(ICharacter actor, StringStack input)
	{
		var protos = actor.Gameworld.ItemComponentProtos.Where(x => x.Status == RevisionStatus.PendingRevision);

		while (!input.IsFinished)
		{
			var cmd = input.Pop().ToLowerInvariant();
			switch (cmd)
			{
				case "by":
					cmd = input.Pop().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						actor.OutputHandler.Send("List Item Components for Review by whom?");
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name.ToLowerInvariant() == cmd);
						if (dbaccount == null)
						{
							actor.OutputHandler.Send("There is no such account.");
							return;
						}

						protos = protos.Where(x => x.BuilderAccountID == dbaccount.Id);
						break;
					}

				case "mine":
					protos = protos.Where(x => x.BuilderAccountID == actor.Account.Id);
					break;
				default:
					actor.OutputHandler.Send("That is not a valid option for Listing Item Components for Review.");
					return;
			}
		}

		// Display Output for List
		using (new FMDB())
		{
			actor.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from proto in protos
					select
						new[]
						{
							proto.Id.ToString(), proto.RevisionNumber.ToString(), proto.Name.Proper(),
							proto.Description, proto.TypeDescription,
							FMDB.Context.Accounts.Find(proto.BuilderAccountID).Name,
							proto.BuilderComment ?? ""
						},
					new[] { "ID#", "Rev#", "Name", "Description", "Type", "Builder", "Comment" },
					actor.Account.LineFormatLength, colour: Telnet.Green,
					unicodeTable: actor.Account.UseUnicode
				)
			);
		}
	}

	private static void Component_Review_History(ICharacter actor, StringStack input)
	{
		var cmd = input.Pop();
		if (!long.TryParse(cmd, out var value))
		{
			actor.OutputHandler.Send("Which item component do you want to view the revision history of?");
			return;
		}

		var protos = actor.Gameworld.ItemComponentProtos.GetAll(value);
		if (!protos.Any())
		{
			actor.OutputHandler.Send("There is no such item component.");
			return;
		}

		// Display Output for List
		using (new FMDB())
		{
			actor.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from proto in protos.OrderBy(x => x.RevisionNumber)
					select new[]
					{
						proto.Id.ToString(), proto.RevisionNumber.ToString(),
						FMDB.Context.Accounts.Find(proto.BuilderAccountID).Name, proto.BuilderComment,
						proto.BuilderDate.ToString("dd/mmm/yyyy"),
						FMDB.Context.Accounts.Any(x => x.Id == proto.ReviewerAccountID)
							? FMDB.Context.Accounts.Find(proto.ReviewerAccountID).Name
							: "",
						proto.ReviewerComment ?? "",
						proto.ReviewerDate?.ToString("dd/mmm/yyyy") ?? "",
						proto.Status.Describe()
					},
					new[]
					{
						"ID#", "Rev#", "Builder", "Comment", "Build Date", "Reviewer", "Comment",
						"Review Date",
						"Status"
					}, actor.Account.LineFormatLength, colour: Telnet.Green,
					unicodeTable: actor.Account.UseUnicode
				)
			);
		}
	}

	private static void Component_Review_All(ICharacter actor, StringStack input)
	{
		var protos = actor.Gameworld.ItemComponentProtos.Where(x => x.Status == RevisionStatus.PendingRevision);
		while (!input.IsFinished)
		{
			var cmd = input.Pop().ToLowerInvariant();
			switch (cmd)
			{
				case "by":
					cmd = input.Pop().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						actor.OutputHandler.Send("Review Item Components by whom?");
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name.ToLowerInvariant() == cmd);
						if (dbaccount == null)
						{
							actor.OutputHandler.Send("There is no such account.");
							return;
						}

						protos = protos.Where(x => x.BuilderAccountID == dbaccount.Id);
						break;
					}

				case "mine":
					protos = protos.Where(x => x.BuilderAccountID == actor.Account.Id);
					break;
				default:
					actor.OutputHandler.Send("That is not a valid option for Reviewing Item Components.");
					return;
			}
		}

		if (!protos.Any())
		{
			actor.OutputHandler.Send("There are no Item Components to review.");
			return;
		}

		var editableItems = protos.ToList();
		var count = editableItems.Count;
		using (new FMDB())
		{
			var accounts = editableItems.Select(x => x.BuilderAccountID).Distinct()
			                            .Select(x => (Id: x, Account: FMDB.Context.Accounts.Find(x)))
			                            .ToDictionary(x => x.Id, x => x.Account);
			actor.OutputHandler.Send(
				string.Format(
					"You are reviewing {0} {1}.\n\n{6}\nTo approve {2} {1}, type {3} or {4} to reject.\nIf you do not wish to approve or decline, you may type {5}.",
					count,
					count == 1 ? "Component" : "Components",
					count == 1 ? "this" : "these",
					"accept edit <your comments>".Colour(Telnet.Yellow),
					"decline edit <your comments>".Colour(Telnet.Yellow),
					"abort edit".Colour(Telnet.Yellow),
					editableItems.Select(x =>
						             $"\t{x.EditHeader()} - Edited by {accounts[x.BuilderAccountID].Name.ColourName()} {((PermissionLevel)accounts[x.BuilderAccountID].AuthorityGroup.AuthorityLevel).DescribeEnum(true).Parentheses().ColourValue()}")
					             .ListToCommaSeparatedValues("\n")
				));
		}

		actor.AddEffect(
			new Accept(actor, new EditableItemReviewProposal<IGameItemComponentProto>(actor, protos.ToList())),
			new TimeSpan(0, 0, 60));
	}

	private static void Component_Review_Default(ICharacter actor, StringStack input)
	{
		if (!long.TryParse(input.Last, out var value))
		{
			actor.OutputHandler.Send("Which item component do you wish to review?");
			return;
		}

		IGameItemComponentProto proto = null;

		var cmd = input.Pop();
		if (cmd.Length == 0)
		{
			proto =
				actor.Gameworld.ItemComponentProtos.FirstOrDefault(
					x => x.Id == value && x.Status == RevisionStatus.PendingRevision);
		}
		else
		{
			if (!int.TryParse(cmd, out var revnum))
			{
				actor.OutputHandler.Send("Which specific revision do you wish to review?");
				return;
			}

			proto = actor.Gameworld.ItemComponentProtos.Get(value, revnum);
			if (proto != null && proto.Status != RevisionStatus.PendingRevision)
			{
				proto = null;
			}
		}

		if (proto == null)
		{
			actor.OutputHandler.Send("There is no such item component that requires review.");
			return;
		}

		actor.OutputHandler.Send(("You are reviewing " + proto.EditHeader()).Colour(Telnet.Red) + "\n\n" +
		                         proto.Show(actor) + "\n\nTo approve this item component, type " +
		                         "accept edit <your comments>".Colour(Telnet.Yellow) + " or " +
		                         "decline edit <your comments>".Colour(Telnet.Yellow) +
		                         " to reject.\nIf you do not wish to approve or decline, your request will time out in 60 seconds.");
		actor.AddEffect(
			new Accept(actor,
				new EditableItemReviewProposal<IGameItemComponentProto>(actor,
					new List<IGameItemComponentProto>
						{ proto })),
			new TimeSpan(0, 0, 60));
	}

	private static void Component_Edit(ICharacter actor, StringStack input)
	{
		var cmd = input.Pop().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			if (actor.EditingItemComponent != null)
			{
				var sb = new StringBuilder();
				sb.AppendLine("You are currently editing " + actor.EditingItemComponent.EditHeader());
				sb.AppendLine();
				sb.Append(actor.EditingItemComponent.Show(actor));
				actor.OutputHandler.Send(sb.ToString());
				return;
			}

			actor.OutputHandler.Send("What do you wish to edit?");
			return;
		}

		switch (cmd)
		{
			case "close":
				Component_Edit_Close(actor, input);
				break;
			case "delete":
				Component_Edit_Delete(actor, input);
				break;
			case "submit":
				Component_Edit_Submit(actor, input);
				break;
			case "obsolete":
				Component_Edit_Obsolete(actor, input);
				break;
			case "new":
				Component_Edit_New(actor, input);
				break;
			default:
				Component_Edit_Default(actor, input);
				break;
		}
	}

	private static void Component_Edit_Close(ICharacter actor, StringStack input)
	{
		if (actor.EditingItemComponent == null)
		{
			actor.OutputHandler.Send("You are not currently editing any item component.");
			return;
		}

		actor.EditingItemComponent = null;
		actor.OutputHandler.Send("You close your current edited item component.");
	}

	private static void Component_Edit_Delete(ICharacter actor, StringStack input)
	{
		if (actor.EditingItemComponent == null)
		{
			actor.OutputHandler.Send("You are not currently editing any item component.");
			return;
		}

		if (actor.EditingItemComponent.Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send("That item component is not currently under design.");
			return;
		}

		using (new FMDB())
		{
			var dbproto = FMDB.Context.GameItemComponentProtos.Find(actor.EditingItemComponent.Id,
				actor.EditingItemComponent.RevisionNumber);
			if (dbproto != null)
			{
				actor.Gameworld.SaveManager.Flush();
				FMDB.Context.GameItemComponentProtos.Remove(dbproto);
				FMDB.Context.SaveChanges();
			}
		}

		actor.OutputHandler.Send("You delete " + actor.EditingItemComponent.EditHeader() + ".");
		actor.Gameworld.Destroy((IGameItemComponentProto)actor.EditingItemComponent);
		actor.EditingItemComponent = null;
	}

	private static void Component_Edit_Submit(ICharacter actor, StringStack input)
	{
		if (actor.EditingItemComponent == null)
		{
			actor.OutputHandler.Send("You are not currently editing any item component.");
			return;
		}

		if (actor.EditingItemComponent.Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send("That item component is not currently under design.");
			return;
		}

		if (!actor.EditingItemComponent.CanSubmit())
		{
			actor.Send(actor.EditingItemComponent.WhyCannotSubmit());
			return;
		}

		var comment = input.IsFinished ? "" : input.SafeRemainingArgument;

		actor.EditingItemComponent.ChangeStatus(RevisionStatus.PendingRevision, comment, actor.Account);
		actor.OutputHandler.Send("You submit " + actor.EditingItemComponent.EditHeader() + " for review" +
		                         (comment.Length > 0 ? ", with the comment: " + comment : "."));
		actor.EditingItemComponent = null;
	}

	private static void Component_Edit_Obsolete(ICharacter actor, StringStack input)
	{
		if (actor.EditingItemComponent == null)
		{
			actor.OutputHandler.Send("You are not currently editing any item component.");
			return;
		}

		if (actor.EditingItemComponent.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send("You are not editing the most current revision of this item component.");
			return;
		}

		actor.EditingItemComponent.ChangeStatus(RevisionStatus.Obsolete, input.SafeRemainingArgument, actor.Account);
		actor.OutputHandler.Send("You mark " + actor.EditingItemComponent.EditHeader() +
		                         " as an obsolete component prototype.");
		actor.EditingItemComponent = null;
	}

	private static void Component_Edit_New(ICharacter actor, StringStack input)
	{
		var cmd = input.PopSpeech().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			actor.OutputHandler.Send("What sort of item component do you wish to create?");
			return;
		}

		var result = actor.Gameworld.GameItemComponentManager.GetProto(cmd, actor.Gameworld, actor.Account);
		if (result == null)
		{
			actor.OutputHandler.Send("That is not a valid type of item component.");
			return;
		}

		actor.EditingItemComponent = result;
		actor.Gameworld.Add((IGameItemComponentProto)actor.EditingItemComponent);
		actor.OutputHandler.Send("You create a new item component prototype with ID " +
		                         actor.EditingItemComponent.Id + ".");
	}

	private static void Component_Edit_Default(ICharacter actor, StringStack input)
	{
		var cmd = input.Last;
		if (!long.TryParse(cmd, out var id))
		{
			actor.OutputHandler.Send("You must either enter an ID of an item component to edit, or use the " +
			                         "new".Colour(Telnet.Cyan) + " keyword.");
			return;
		}

		IGameItemComponentProto proto = null;
		cmd = input.Pop();
		if (cmd.Length == 0)
		{
			proto = actor.Gameworld.ItemComponentProtos.Get(id);
		}
		else
		{
			if (!int.TryParse(cmd, out var revision))
			{
				actor.OutputHandler.Send(
					"You must either enter just an ID to open the most recent revision, or specify a numerical revision number.");
				return;
			}

			proto = actor.Gameworld.ItemComponentProtos.Get(id, revision);
			if (proto.Status != RevisionStatus.UnderDesign && proto.Status != RevisionStatus.PendingRevision)
			{
				actor.OutputHandler.Send("You cannot open that component for editing, you must open a fresh one.");
				return;
			}
		}

		if (proto == null)
		{
			actor.OutputHandler.Send("There is no such item component for you to edit.");
			return;
		}

		if (proto.ReadOnly)
		{
			actor.OutputHandler.Send("You cannot edit that component as it is read-only.");
			return;
		}

		if (proto.Status == RevisionStatus.UnderDesign || proto.Status == RevisionStatus.PendingRevision)
		{
			actor.EditingItemComponent = proto;
		}
		else
		{
			actor.EditingItemComponent = proto.CreateNewRevision(actor);
			actor.Gameworld.Add((IGameItemComponentProto)actor.EditingItemComponent);
		}

		actor.OutputHandler.Send("You open " + actor.EditingItemComponent.EditHeader() + " for editing.");
	}

	private static void Component_Set(ICharacter actor, StringStack input)
	{
		if (actor.EditingItemComponent == null)
		{
			actor.OutputHandler.Send("You are not editing an Item Component.");
			return;
		}

		actor.EditingItemComponent.BuildingCommand(actor, input);
	}

	private static void Component_List(ICharacter actor, StringStack input)
	{
		var protos = actor.Gameworld.ItemComponentProtos.Where(x => x.Status == RevisionStatus.Current).ToList();

		// Apply user-supplied filter criteria
		while (!input.IsFinished)
		{
			var cmd = input.PopSpeech().ToLowerInvariant();
			if (long.TryParse(cmd, out var value))
			{
				protos = protos.Where(x => x.Id == value).ToList();
				continue;
			}

			switch (cmd)
			{
				case "all":
					protos = actor.Gameworld.ItemComponentProtos.ToList();
					break;
				case "mine":
					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.Find(actor.Account.Id);
						if (dbaccount == null)
						{
							actor.OutputHandler.Send("There is no such account.");
							return;
						}

						protos = protos.Where(x => x.BuilderAccountID == dbaccount.Id).ToList();
						break;
					}

				case "by":
					cmd = input.PopSpeech().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						actor.OutputHandler.Send("List Item Components by whom?");
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
						if (dbaccount == null)
						{
							actor.OutputHandler.Send("There is no such account.");
							return;
						}

						protos = protos.Where(x => x.BuilderAccountID == dbaccount.Id).ToList();
						break;
					}

				case "reviewed":
					cmd = input.PopSpeech().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						actor.OutputHandler.Send("List Item Components reviewed by whom?");
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name.ToLowerInvariant() == cmd);
						if (dbaccount == null)
						{
							actor.OutputHandler.Send("There is no such account.");
							return;
						}

						protos = protos.Where(x => x.ReviewerAccountID == dbaccount.Id).ToList();
						break;
					}

				default:
					if (cmd[0] == '+')
					{
						var subcmd = cmd.RemoveFirstCharacter();
						if (subcmd.Length == 0)
						{
							actor.OutputHandler.Send("Include which keyword?");
							return;
						}

						protos = protos
						         .Where(x => x.Name.Contains(subcmd, StringComparison.InvariantCultureIgnoreCase) ||
						                     x.Description.Contains(
							                     subcmd, StringComparison.InvariantCultureIgnoreCase)).ToList();
						break;
					}

					if (cmd[0] == '-')
					{
						var subcmd = cmd.RemoveFirstCharacter();
						if (subcmd.Length == 0)
						{
							actor.OutputHandler.Send("Exclude which keyword?");
							return;
						}

						protos = protos
						         .Where(
							         x => !x.Name.Contains(subcmd, StringComparison.InvariantCultureIgnoreCase) &&
							              !x.Description.Contains(
								              subcmd, StringComparison.InvariantCultureIgnoreCase)).ToList();
						break;
					}

					protos =
						protos.Where(x => x.TypeDescription.StartsWith(
							      cmd, StringComparison.InvariantCultureIgnoreCase))
						      .ToList();
					break;
			}
		}

		// Sort List
		protos = protos.OrderBy(x => x.Id).ThenBy(x => x.RevisionNumber).ToList();

		// Display Output for List
		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from proto in protos
				select
					new[]
					{
						proto.Id.ToString(), proto.RevisionNumber.ToString(), proto.Name.Proper(),
						proto.Description, proto.TypeDescription, proto.Status.Describe()
					},
				new[] { "ID#", "Rev#", "Name", "Description", "Type", "Status" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Component_Show(ICharacter actor, StringStack input)
	{
		var cmd = input.Pop();
		if (!long.TryParse(cmd, out var vnum))
		{
			actor.OutputHandler.Send("That is not a valid id number.");
			return;
		}

		IGameItemComponentProto proto = null;
		cmd = input.Pop();
		if (cmd.Length > 0)
		{
			if (!int.TryParse(cmd, out var revision))
			{
				actor.OutputHandler.Send("That is not a valid revision.");
				return;
			}

			proto = actor.Gameworld.ItemComponentProtos.Get(vnum, revision);
		}
		else
		{
			proto = actor.Gameworld.ItemComponentProtos.Get(vnum);
		}

		if (proto == null)
		{
			actor.OutputHandler.Send("That is not a valid item component.");
			return;
		}

		actor.OutputHandler.Send(proto.Show(actor));
	}

	#endregion

	#region Characteristics

	private const string CharacteristicHelp =
		@"The characteristic command is used to work with characteristic definitions, values and profiles. Characteristics are also sometimes known as 'variables', and are used for both items (where they may represent things like variable colours, shapes, designs or the like) and also characters (where they can represent things like eye colour, hair style, etc).

Characteristic Definitions are the 'types' of characteristics.
Characteristic Values are the possible values that may match a characteristic type.
Characteristic Profiles are curated lists of characteristic values that control permitted values.

You must use one of the following subcommands of this command:

characteristic definition ... - work with characteristic definitions
characteristic value ... - work with characteristic values
characteristic profile ... - work with characteristic profiles";

	[PlayerCommand("Characteristic", "characteristic", "variable")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Characteristic(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "definition":
			case "def":
				Characteristic_Definition(actor, ss);
				break;
			case "value":
			case "val":
				Characteristic_Value(actor, ss);
				break;
			case "profile":
			case "prof":
				Characteristic_Profile(actor, ss);
				break;
			default:
				actor.OutputHandler.Send(CharacteristicHelp);
				return;
		}
	}

	private static void Characteristic_Definition(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "add":
			case "new":
			case "create":
				Characteristic_Definition_Add(actor, command, false, false);
				break;
			case "addchild":
			case "newchild":
			case "createchild":
				Characteristic_Definition_Add(actor, command, true, false);
				break;
			case "addbodypart":
			case "addpart":
			case "newbodypart":
			case "newpart":
			case "createbodypart":
			case "createpart":
				Characteristic_Definition_Add(actor, command, false, true);
				return;
			case "addbodypartchild":
			case "addpartchild":
			case "newbodypartchild":
			case "newpartchild":
			case "createbodypartchild":
			case "createpartchild":
				Characteristic_Definition_Add(actor, command, true, true);
				return;
			case "delete":
			case "remove":
			case "del":
			case "rem":
				Characteristic_Definition_Remove(actor, command);
				break;
			case "set":
				Characteristic_Definition_Set(actor, command);
				break;
			case "list":
				CharacteristicDefinitionList(actor, command);
				return;
			case "show":
			case "view":
				CharacteristicDefinitionShow(actor, command);
				return;
			default:
				actor.OutputHandler.Send(@"You can use the following options with this command:

	list [<names...>] - lists all of the characteristic definitions
	show <which> - views detailed information about a characteristic definition
	add <name> <type> - creates a new characteristic of the specified type
	addchild <name> <parent> - creates a new child characteristic of another
	addpart <name> <shape> <type> - creates a new bodypart-linked characteristic of the specified type
	addpartchild <name> <shape> <parent> - creates a new bodypart-linked characteristic that is a child of another
	delete <name> - permanently deletes a characteristic type
	set <which> name <name> - changes the name of a characteristic definition
	set <which> desc <desc> - changes the description of a characteristic definition
	set <which> pattern <regex> - changes the regex pattern used to identify in descriptions
	set <which> parent <parent> - if already a child type, changes the parent
	set <which> ... - some types may have additional options");
				return;
		}
	}

	private static void CharacteristicDefinitionList(ICharacter actor, StringStack input)
	{
		var characteristics = actor.Gameworld.Characteristics.ToList();
		while (!input.IsFinished)
		{
			var cmd = input.PopSpeech();
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
						definition.Id.ToString("N0", actor),
						definition.Name,
						definition.Pattern.ToString(),
						definition.Description,
						definition.Type.ToString(),
						definition.Parent?.Name ?? ""
					},
				new[] { "ID#", "Name", "Pattern", "Description", "Type", "Parent" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void CharacteristicDefinitionShow(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic definition do you want to show?");
			return;
		}

		var definition = actor.Gameworld.Characteristics.GetByIdOrName(command.SafeRemainingArgument);
		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition.");
			return;
		}

		actor.OutputHandler.Send(definition.Show(actor));
	}

	private static void Characteristic_Definition_Add(ICharacter actor, StringStack command, bool client, bool bodypart)
	{
		var name = command.PopSpeech().TitleCase();
		if (string.IsNullOrEmpty(name))
		{
			actor.OutputHandler.Send("What name do you want to give your new characteristic definition?");
			return;
		}

		IBodypartShape shape = null;
		if (bodypart)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which bodypart shape should this characteristic be tied to?");
				return;
			}

			shape = actor.Gameworld.BodypartShapes.GetByIdOrName(command.PopSpeech());
			if (shape == null)
			{
				actor.OutputHandler.Send("There is no such bodypart shape.");
				return;
			}
		}

		if (client)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					"Which other characteristic definition do you want to make this one a child definition of?");
				return;
			}

			var parent = actor.Gameworld.Characteristics.GetByIdOrName(command.SafeRemainingArgument);
			if (parent == null)
			{
				actor.OutputHandler.Send("There is no such characteristic definition.");
				return;
			}

			if (bodypart)
			{
				var child = new BodypartSpecificClientCharacteristicDefinition(name, name.ToLowerInvariant(),
					$"The {name} characteristic", shape, parent, actor.Gameworld);
				actor.OutputHandler.Send(
					$"You create a new child bodypart-specific characteristic definition called {name.ColourName()} with ID #{child.Id.ToString("N0", actor)}, which is linked to the characteristic {parent.Name.ColourName()} and the shape {shape.Name.ColourValue()}.");
			}
			else
			{
				var child = new ClientCharacteristicDefinition(name, name.ToLowerInvariant(),
					$"The {name} characteristic", parent, actor.Gameworld);
				actor.OutputHandler.Send(
					$"You create a new child characteristic definition called {name.ColourName()} with ID #{child.Id.ToString("N0", actor)}, which is linked to the characteristic {parent.Name.ColourName()}.");
			}

			return;
		}

		CharacteristicType ctype;
		var type = command.PopSpeech().ToLowerInvariant().CollapseString();
		switch (type)
		{
			case "relativeheight":
			case "relheight":
				ctype = CharacteristicType.RelativeHeight;
				break;
			case "standard":
			case "basic":
				ctype = CharacteristicType.Standard;
				break;
			case "coloured":
			case "colored":
			case "colour":
			case "color":
				ctype = CharacteristicType.Coloured;
				break;
			case "otherbasic":
			case "multiform":
				ctype = CharacteristicType.Multiform;
				break;
			case "growth":
			case "growing":
			case "growable":
				ctype = CharacteristicType.Growable;
				break;
			default:
				actor.OutputHandler.Send("Valid types for Characteristic Definitions are " +
				                         new[] { "Standard", "Coloured", "Multiform", "Relative Height", "Growable" }
					                         .Select(
						                         x => x
							                         .Colour(
								                         Telnet
									                         .Cyan))
					                         .ListToString() +
				                         ".");
				return;
		}

		if (bodypart)
		{
			var definition = new BodypartSpecificCharacteristicDefinition(name, name.ToLowerInvariant(),
				$"The {name} characteristic", ctype, shape,
				actor.Gameworld);
			actor.OutputHandler.Send(
				$"You create a bodypart-specific characteristic definition called {name.ColourName()} with ID #{definition.Id.ToString("N0", actor)} of type {ctype.Describe().ColourValue()} linked to bodypart shape {shape.Name.ColourValue()}.");
		}
		else
		{
			var definition = new CharacteristicDefinition(name, name.ToLowerInvariant(), $"The {name} characteristic",
				ctype,
				actor.Gameworld);
			actor.OutputHandler.Send(
				$"You create a characteristic definition called {name.ColourName()} with ID #{definition.Id.ToString("N0", actor)} of type {ctype.Describe().ColourValue()}.");
		}
	}

	private static void Characteristic_Definition_Remove(ICharacter actor, StringStack command)
	{
		var name = command.SafeRemainingArgument;
		ICharacteristicDefinition definition = null;
		definition = actor.Gameworld.Characteristics.GetByIdOrName(name);

		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition for you to remove.");
			return;
		}

		actor.AddEffect(
			new Accept(actor, new CharacteristicDefinitionRemovalProposal(actor, definition)),
			TimeSpan.FromSeconds(60));
		actor.OutputHandler.Send(
			$"You are proposing to delete the characteristic definition {name.ColourName()} with ID #{definition.Id.ToString("N0", actor)}.\nThis will delete all associated values and profiles, and remove the variable from all item components and items.\n{Accept.StandardAcceptPhrasing}");
	}

	private static void Characteristic_Definition_Set(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		ICharacteristicDefinition definition = null;
		definition = actor.Gameworld.Characteristics.GetByIdOrName(name);

		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition for you to edit.");
			return;
		}

		definition.BuildingCommand(actor, command);
	}

	private static void Characteristic_Value(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "add":
				Characteristic_Value_Add(actor, command);
				break;
			case "clone":
				CharacteristicValueClone(actor, command);
				return;
			case "delete":
			case "remove":
			case "del":
			case "rem":
				Characteristic_Value_Remove(actor, command);
				break;
			case "set":
				Characteristic_Value_Set(actor, command);
				break;
			case "list":
				CharacteristicValueList(actor, command);
				return;
			case "show":
			case "view":
				CharacteristicValueShow(actor, command);
				return;
			default:
				actor.OutputHandler.Send(@"You can use the following options with this command:

    list [<definition>|*<profile>|+<keyword>|-<keyword>] - shows all values meeting the optional filter criteria
    show <which> - shows detailed information about a characteristic value
    add <definition> <name> [<type specific extra args>] - creates a new value
	clone <which> <name> - creates a new value from an existing value
    remove <name> - permanently deletes a characteristic value
    set <which> ... - changes properties of a value. See values for more info.");
				return;
		}
	}

	private static void CharacteristicValueClone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic value would you like to clone?");
			return;
		}

		var clone = actor.Gameworld.CharacteristicValues.GetByIdOrName(command.PopSpeech());
		if (clone == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new cloned value?");
			return;
		}

		var newValue = clone.Clone(command.SafeRemainingArgument);
		actor.Gameworld.Add(newValue);
		actor.OutputHandler.Send(
			$"You clone the characteristic value {clone.Name.ColourValue()} into a new value called {newValue.Name.ColourValue()}.");
	}

	private static void CharacteristicValueShow(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic value do you want to show?");
			return;
		}

		var value = actor.Gameworld.CharacteristicValues.GetByIdOrName(command.SafeRemainingArgument);
		if (value == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value.");
			return;
		}

		actor.OutputHandler.Send(value.Show(actor));
	}

	private static void CharacteristicValueList(ICharacter actor, StringStack command)
	{
		var values = actor.Gameworld.CharacteristicValues.ToList();
		while (!command.IsFinished)
		{
			var cmd = command.PopSpeech();
			if (cmd[0] == '*' && cmd.Length > 1)
			{
				cmd = cmd.Substring(1);
				var profile = actor.Gameworld.CharacteristicProfiles.GetByIdOrName(cmd);
				if (profile == null)
				{
					actor.OutputHandler.Send(
						$"There is no such characteristic profile to filter by as {cmd.ColourCommand()}.");
					return;
				}

				values = values.Where(x => profile.Values.Contains(x)).ToList();
			}
			else if (cmd[0] == '+' && cmd.Length > 1)
			{
				cmd = cmd.Substring(1);
				values = values
				         .Where(x =>
					         x.GetValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) ||
					         x.GetBasicValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) ||
					         x.GetFancyValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase)
				         )
				         .ToList();
			}
			else if (cmd[0] == '-' && cmd.Length > 1)
			{
				cmd = cmd.Substring(1);
				cmd = cmd.Substring(1);
				values = values
				         .Where(x =>
					         !x.GetValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) &&
					         !x.GetBasicValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) &&
					         !x.GetFancyValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase)
				         )
				         .ToList();
			}
			else
			{
				var definition = actor.Gameworld.Characteristics.FirstOrDefault(x => x.Pattern.IsMatch(cmd));
				if (definition == null)
				{
					actor.OutputHandler.Send(
						$"The argument {cmd.ColourCommand()} was not a valid characteristic definition.");
					return;
				}

				values = values.Where(x => x.Definition == definition).ToList();
			}
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from value in values
				select
					new[]
					{
						value.Id.ToString("N0", actor),
						value.Name,
						value.GetValue,
						value.GetBasicValue,
						value.ChargenApplicabilityProg?.MXPClickableFunctionName() ?? "",
						value.OngoingValidityProg?.MXPClickableFunctionName() ?? "",
						value.Definition.IsDefaultValue(value) ? "Y" : "N"
					},
				new[] { "ID#", "Name", "Value", "Basic Value", "Chargen", "Ongoing", "Default?" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Characteristic_Value_Add(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		ICharacteristicDefinition definition = null;
		definition = long.TryParse(name, out var value)
			? actor.Gameworld.Characteristics.Get(value)
			: actor.Gameworld.Characteristics.Get(name).FirstOrDefault();

		if (definition == null)
		{
			actor.OutputHandler.Send(
				"There is no such Characteristic Definition with which you may create a new value.");
			return;
		}

		name = command.PopSpeech();
		if (string.IsNullOrEmpty(name))
		{
			actor.OutputHandler.Send("What name do you want to give your new Characteristic Value?");
			return;
		}

		ICharacteristicValue newvalue = null;
		var basic = command.PopSpeech();
		switch (definition.Type)
		{
			case CharacteristicType.Coloured:
			{
				IColour colour = null;
				colour = actor.Gameworld.Colours.GetByIdOrName(basic);

				if (colour == null)
				{
					actor.OutputHandler.Send("There is no such colour for you to use.");
					return;
				}

				newvalue = new ColourCharacteristicValue(name, definition, colour);
			}
				break;
			case CharacteristicType.RelativeHeight:
				actor.OutputHandler.Send("This kind of Characteristic cannot have explicitly created values.");
				return;
			case CharacteristicType.Multiform:

				var fancy = command.PopSpeech();
				if (string.IsNullOrEmpty(basic) || string.IsNullOrEmpty(fancy))
				{
					actor.OutputHandler.Send(
						"You must specify both a basic and fancy value for this kind of characteristic.");
					return;
				}

				newvalue = new MultiformCharacteristicValue(name, definition, basic, fancy);

				break;
			case CharacteristicType.Growable:
				fancy = command.PopSpeech();
				if (string.IsNullOrEmpty(basic) || string.IsNullOrEmpty(fancy))
				{
					actor.OutputHandler.Send(
						"You must specify both a basic and fancy value for this kind of characteristic.");
					return;
				}

				if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var length))
				{
					actor.OutputHandler.Send("You must specify a valid numerical growth stage for the value.");
					return;
				}

				newvalue = new GrowableCharacteristicValue(name, definition, basic, fancy, length);
				break;
			default:
				newvalue = new CharacteristicValue(name, definition, name, string.Empty);
				break;
		}

		actor.Gameworld.Add(newvalue);
		actor.Gameworld.SaveManager.Flush();
		actor.OutputHandler.Send(
			$"You create a new value for the {definition.Name.ColourName()} definition with a value of {newvalue.GetValue.ColourValue()} and ID of {newvalue.Id.ToString("N0", actor)}.");
	}

	private static void Characteristic_Value_Remove(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		var cvalue = actor.Gameworld.CharacteristicValues.GetByIdOrName(name);

		if (cvalue == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value for you to remove.");
			return;
		}

		actor.AddEffect(
			new Accept(actor, new CharacteristicValueRemovalProposal(actor, cvalue)),
			TimeSpan.FromSeconds(60));
		actor.OutputHandler.Send(
			$"You are proposing to permanently delete the characteristic value {name.ColourName()} with ID {cvalue.Id.ToString("N0", actor)}.\nThis will remove it from all associated profiles, and remove the variable from all item components and items (randomising them to a new value).\n{Accept.StandardAcceptPhrasing}");
	}

	private static void Characteristic_Value_Set(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		var cvalue = actor.Gameworld.CharacteristicValues.GetByIdOrName(name);

		if (cvalue == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value for you to edit.");
			return;
		}

		cvalue.BuildingCommand(actor, command);
	}

	private static void Characteristic_Profile(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "add":
			case "new":
			case "create":
				CharacteristicProfileNew(actor, command);
				return;
			case "list":
				CharacteristicProfileList(actor, command);
				return;
			case "show":
			case "view":
				CharacteristicProfileView(actor, command);
				return;
			case "set":
				CharacteristicProfileSet(actor, command);
				return;
			case "clone":
				CharacteristicProfileClone(actor, command);
				return;
			default:
				actor.OutputHandler.Send(@"You can use the following options to edit characteristic profiles:

	characteristic profile list - lists all of the characteristic profiles
	characteristic profile show <which> - views detailed information about a profile
	characteristic profile new <definition> <type> <name> - creates a new profile
	characteristic profile clone <which> <name> - clones an existing profile to a new one
	characteristic profile set <which> ... - edits properties of a profile. See individual types for more help");
				return;
		}
	}

	private static void CharacteristicProfileClone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic profile do you want to clone?");
			return;
		}

		var clone = actor.Gameworld.CharacteristicProfiles.GetByIdOrName(command.PopSpeech());
		if (clone == null)
		{
			actor.OutputHandler.Send("There is no such characteristic profile.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this cloned profile?");
			return;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.CharacteristicProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a characteristic profile with that name. Names must be unique.");
			return;
		}

		var newProfile = clone.Clone(name);
		actor.Gameworld.Add(newProfile);
		actor.OutputHandler.Send(
			$"You clone the characteristic profile {clone.Name.ColourName()} into a new profile called {name.ColourName()}.");
	}

	private static void CharacteristicProfileSet(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic profile do you want to edit?");
			return;
		}

		var which = actor.Gameworld.CharacteristicProfiles.GetByIdOrName(command.PopSpeech());
		if (which == null)
		{
			actor.OutputHandler.Send("There is no such characteristic profile.");
			return;
		}

		which.BuildingCommand(actor, command);
	}

	private static void CharacteristicProfileView(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic profile do you want to view?");
			return;
		}

		var which = actor.Gameworld.CharacteristicProfiles.GetByIdOrName(command.PopSpeech());
		if (which == null)
		{
			actor.OutputHandler.Send("There is no such characteristic profile.");
			return;
		}

		actor.OutputHandler.Send(which.Show(actor));
	}

	private static void CharacteristicProfileList(ICharacter actor, StringStack command)
	{
		var characteristics = actor.Gameworld.CharacteristicProfiles.ToList();
		while (!command.IsFinished)
		{
			var cmd = command.PopSpeech();
			if (cmd[0] == '+' && cmd.Length > 1)
			{
				cmd = cmd.Substring(1);
				characteristics = characteristics
				                  .Where(x => x.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) ||
				                              x.Description.Contains(cmd, StringComparison.InvariantCultureIgnoreCase))
				                  .ToList();
			}
			else if (cmd[0] == '-' && cmd.Length > 1)
			{
				cmd = cmd.Substring(1);
				characteristics = characteristics
				                  .Where(x => !x.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) &&
				                              !x.Description.Contains(cmd, StringComparison.InvariantCultureIgnoreCase))
				                  .ToList();
			}
			else
			{
				var definition = actor.Gameworld.Characteristics.FirstOrDefault(x => x.Pattern.IsMatch(cmd));
				if (definition == null)
				{
					actor.OutputHandler.Send(
						$"The argument {cmd.ColourCommand()} was not a valid characteristic definition.");
					return;
				}

				characteristics = characteristics.Where(x => x.TargetDefinition == definition).ToList();
			}
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from definition in characteristics
				select
					new[]
					{
						definition.Id.ToString(),
						definition.Name,
						definition.TargetDefinition.Name,
						definition.Description,
						definition.Values.Count().ToString("N0", actor),
						definition.Type
					},
				new[] { "ID#", "Name", "Definition", "Description", "Values", "Type" }, actor.Account.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 3,
				unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void CharacteristicProfileNew(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic definition do you want to make a profile for?");
			return;
		}

		var definition = actor.Gameworld.Characteristics.GetByIdOrName(command.PopSpeech());
		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition.");
			return;
		}

		var typeText = command.PopSpeech().ToLowerInvariant().CollapseString();

		switch (typeText)
		{
			case "standard":
			case "all":
			case "compound":
			case "weighted":
				break;
			default:
				actor.OutputHandler.Send(
					$"You must specify a valid type from the following list: {new[] { "standard", "compound", "all", "weighted" }.Select(x => x.ColourValue()).ListToString()}");
				return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your profile?");
			return;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.CharacteristicProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a characteristic profile with that name. Names must be unique.");
			return;
		}

		switch (typeText)
		{
			case "standard":
				var profile = new CharacteristicProfile(name, definition, "Standard");
				actor.Gameworld.Add(profile);
				actor.OutputHandler.Send(
					$"You create a new standard characteristic profile called {name.ColourName()} for the {definition.Name.ColourName()} definition.");
				return;
			case "all":
				profile = new CharacterProfileAll(name, definition);
				actor.Gameworld.Add(profile);
				actor.OutputHandler.Send(
					$"You create a new all-values characteristic profile called {name.ColourName()} for the {definition.Name.ColourName()} definition.");
				return;
			case "compound":
				profile = new CharacteristicProfileCompound(name, definition);
				actor.Gameworld.Add(profile);
				actor.OutputHandler.Send(
					$"You create a new compound characteristic profile called {name.ColourName()} for the {definition.Name.ColourName()} definition.");
				return;
			case "weighted":
				profile = new WeightedCharacteristicProfile(name, definition);
				actor.Gameworld.Add(profile);
				actor.OutputHandler.Send(
					$"You create a new weighted characteristic profile called {name.ColourName()} for the {definition.Name.ColourName()} definition.");
				return;
		}
	}

	#endregion
	
	#region Dreams

	public const string DreamHelpText =
		@"You can use this command to create and edit dreams, and also to assign them to people.

The syntax for this command is as follows:

	dream list - lists all dreams
	dream show <which> - shows a particular dream
	dream edit <which> - begins editing a particular dream
	dream edit - an alias for dream show <editing item>
	dream close - stops editing a dream
	dream edit new <name> - creates a new dream
	dream clone <old> <new name> - clones an existing dream
	dream give <which> <character> - gives a dream to a character
	dream remove <which> <character> - removes a dream from a character
	dream set ... - edits the properties of the dream you have open";

	[PlayerCommand("Dream", "dream")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("dream", DreamHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Dream(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		GenericBuildingCommand(actor, ss, EditableItemHelper.DreamHelper);
	}

	#endregion

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
	#3npc list [<filters>]#0 - lists all NPC prototypes. See below for filters:

		#6all#0 - includes obsolete and non-current revisions
		#6mine#0 - only shows NPCs you personally created
		#6by <account>#0 - only shows NPCs the nominated account created
		#6reviewed <account>#0 - only shows NPCs the nominated account has approved";

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
			default:
				character.OutputHandler.Send(NPCHelp.SubstituteANSIColour());
				break;
		}
	}

	#endregion

	#region Item Groups

	[PlayerCommand("ItemGroup", "itemgroup", "ig")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("itemgroup",
		@"Item Groups are used to group similar items when shown in a room, for example, when a venue has multiple tables that you don't want to show individually, or if you wanted to have lots of insignificant or small items grouped together to not drown out more important items.

This command is used to manage those item groups and set their parameters. To add these groups to item prototypes, see the ITEM command.

Item groups also have something called #6forms#0. Forms cause all the same item groups to appear differently in different contexts (usually different rooms), so you could have a grouping for tables and chairs which could be presented differently in different locations, such as different taverns or cafes. 

The syntax for this command is as follows:

	#3itemgroup add <name>#0 - adds a new item group
	#3itemgroup delete <which>#0 - permanently deletes an item group
	#3itemgroup show <which>#0 - shows info about an item group
	#3itemgroup list#0 - lists all item groups **
	#3itemgroup <which> name <name>#0 - renames a group
	#3itemgroup <which> keywords <space separated list>#0 - sets the item group keywords
	#3itemgroup <which> form add <type>#0 - adds a new description form for a group
	#3itemgroup <which> form delete <id>#0 - deletes the specified form from a group
	#3itemgroup <which> form <id> room <id>#0 - toggles a room as using the form
	#3itemgroup <which> form <id> ldesc <ldesc>#0 - sets the long description of the form
	#3itemgroup <which> form <id> name <item name>#0 - sets the name of items in the form
	#3itemgroup <which> form <id> desc#0 - sets the look description for the form

** - The following options are available to filter the list:

	#6+<keyword>#0 - searches for a keyword in the item group or one of its forms
	#6-<keyword>#0 - excludes a keyword in the item group or one of its forms
	#6*<room id>#0 - searches for groups with forms that explicitly contain the specified room",
		AutoHelp.HelpArgOrNoArg)]
	protected static void ItemGroup(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		var cmd = ss.PopSpeech();
		switch (cmd.ToLowerInvariant())
		{
			case "new":
			case "create":
			case "add":
				ItemGroupNew(actor, ss);
				return;
			case "delete":
			case "del":
			case "remove":
			case "rem":
				ItemGroupDelete(actor, ss);
				return;
			case "show":
			case "view":
				ItemGroupShow(actor, ss);
				return;
			case "list":
				ItemGroupList(actor, ss);
				return;
		}

		var itemgroup = long.TryParse(cmd, out var value)
			? actor.Gameworld.ItemGroups.Get(value)
			: actor.Gameworld.ItemGroups.FirstOrDefault(
				x => x.Name.Equals(cmd, StringComparison.InvariantCultureIgnoreCase));

		if (itemgroup == null)
		{
			actor.Send("There is no such item group for you to edit.");
			return;
		}

		itemgroup.BuildingCommand(actor, ss);
	}

	private static void ItemGroupList(ICharacter actor, StringStack command)
	{
		var itemgroups = actor.Gameworld.ItemGroups.AsEnumerable();
		while (!command.IsFinished)
		{
			var text = command.PopSpeech();
			if (text.Length <= 1)
			{
				actor.OutputHandler.Send($"The text {text.ColourCommand()} is not a valid filter.");
				return;
			}

			long id;
			string keyword;
			switch (text[0])
			{
				case '*':
					if (!long.TryParse(text.Substring(1), out id))
					{
						actor.OutputHandler.Send($"There is no room with ID {text.Substring(1).ColourValue()}.");
						return;
					}

					itemgroups = itemgroups.Where(x => x.Forms.Any(x => x.SpecialFormFor(id)));
					continue;
				case '+':
					keyword = text.Substring(1);
					itemgroups = itemgroups.Where(x => x.Forms.Any(y =>
						y.Describe(actor, Enumerable.Empty<IGameItem>())
						 .Contains(keyword, StringComparison.InvariantCultureIgnoreCase)));
					continue;
				case '-':
					keyword = text.Substring(1);
					itemgroups = itemgroups.Where(x => x.Forms.Any(y =>
						!y.Describe(actor, Enumerable.Empty<IGameItem>())
						  .Contains(keyword, StringComparison.InvariantCultureIgnoreCase)));
					continue;
				default:
					actor.OutputHandler.Send($"The text {text.ColourCommand()} is not a valid filter.");
					return;
			}
		}

		var protos = actor.Gameworld.ItemProtos.GetAllApprovedOrMostRecent(true).ToList();
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in itemgroups
			select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.Forms.Count().ToString("N0", actor),
				protos.Count(x => x.ItemGroup == item).ToString("N0", actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Forms",
				"On Items"
			},
			actor,
			Telnet.Green
		));
	}


	private static void ItemGroupShow(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which item group do you want to show?");
			return;
		}

		var itemgroup = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.ItemGroups.Get(value)
			: actor.Gameworld.ItemGroups.FirstOrDefault(
				x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));

		if (itemgroup == null)
		{
			actor.Send("There is no such item group for you to show.");
			return;
		}

		actor.OutputHandler.Send(itemgroup.Show(actor));
	}

	private static void ItemGroupDelete(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which item group do you want to delete?");
			return;
		}

		var itemgroup = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.ItemGroups.Get(value)
			: actor.Gameworld.ItemGroups.FirstOrDefault(
				x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));

		if (itemgroup == null)
		{
			actor.Send("There is no such item group for you to delete.");
			return;
		}

		if (itemgroup.CannotBeDeleted)
		{
			actor.OutputHandler.Send(
				$"The {itemgroup.Name.ColourName()} item group is read only and cannot be deleted.");
			return;
		}

		actor.AddEffect(new Accept(actor, new GameItemGroupDeletionProposal(actor, itemgroup)),
			TimeSpan.FromSeconds(120));
		actor.Send(
			"You are proposing to permanently delete item group {0:N0}. This cannot be undone. Use {1} to proceed with the deletion.",
			itemgroup.Id, "accept delete".Colour(Telnet.Yellow));
	}

	private static void ItemGroupNew(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What name would you like to give to the new item group?");
			return;
		}

		if (actor.Gameworld.ItemGroups.Any(x => x.Name.EqualTo(command.SafeRemainingArgument)))
		{
			actor.OutputHandler.Send(
				$"There is already an item group called {command.SafeRemainingArgument.TitleCase().ColourName()}. Names must be unique.");
			return;
		}

		using (new FMDB())
		{
			var dbitem = new Models.ItemGroup
			{
				Name = command.SafeRemainingArgument.TitleCase(),
				Keywords = command.SafeRemainingArgument
			};
			FMDB.Context.ItemGroups.Add(dbitem);
			FMDB.Context.SaveChanges();
			var newItem = GameItemGroupFactory.CreateGameItemGroup(dbitem, actor.Gameworld);
			actor.Gameworld.Add(newItem);
			actor.OutputHandler.Send(
				$"You create a new item group called {dbitem.Name.ColourName()} with ID {newItem.Id.ToString("N0", actor)}.");
		}
	}

	#endregion Item Groups

	#region Tags

	private const string TagHelp = @"The tag command is used to create, view and manage tags. 

Tags are used in numerous parts of the code to classify things and apply basic hierarchies. These hierarchies are best explained through an example.

Say you had a tag called #6Knife#0, and you wanted knives to be also #6Cutting Tools#0. The knife tag would be a child of the cutting tool tag, and would be represented as #6Cutting Tools / Knife#0.

In some places you might want to check directly for knives, but you could also check more generally for cutting tools, which might also pick up #6Cutting Tools / Saw#0.

Tags are used on items and crafts, but also other places in the code as well.

The syntax to use this command is as follows:

	#3tag list#0 - shows all the top level tags
	#3tag show <which>#0 - shows info about a particular tag
	#3tag new <name> [<parent>]#0 - creates a new tag (with optional parent)
	#3tag insert <name> <parent>#0 - creates a new tag as a child of a tag and takes its children
	#3tag parent <tag> <newparent>#0 - changes the parent of a tag
	#3tag rename <tag> <name>#0 - changes the name of a tag
	#3tag prog <tag> <which>#0 - sets a prog that controls visibility of the tag
	#3tag prog none#0 - clears a visiblity prog";

	[PlayerCommand("Tag", "tag")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("Tag", TagHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Tag(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				TagAdd(actor, ss);
				return;
			case "remove":
			case "rem":
				TagRemove(actor, ss);
				return;
			case "insert":
			case "ins":
				TagInsert(actor, ss);
				return;
			case "view":
			case "show":
				TagView(actor, ss);
				return;
			case "list":
				TagList(actor, ss);
				return;
			case "rename":
				TagRename(actor, ss);
				return;
			case "parent":
				TagParent(actor, ss);
				return;
			case "prog":
				TagProg(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(TagHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void TagProg(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to change the parent of?");
			return;
		}

		var tag = actor.Gameworld.Tags.GetByIdOrName(ss.PopSpeech());
		if (tag is null)
		{
			actor.OutputHandler.Send($"There is no such tag identified by {ss.Last.ColourCommand()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either use {"none".ColourCommand()}, or specify a prog to be the new parent of the {tag.FullName.ColourName()} tag?");
			return;
		}

		if (ss.SafeRemainingArgument.EqualTo("none"))
		{
			tag.ShouldSeeProg = null;
			actor.OutputHandler.Send(
				$"The {tag.FullName.ColourName()} tag will not use any prog to control visibility (always visible instead).");
			return;
		}

		var prog = new FutureProgLookupFromBuilderInput(actor.Gameworld, actor, ss.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return;
		}

		tag.ShouldSeeProg = prog;
		actor.OutputHandler.Send(
			$"The {tag.FullName.ColourName()} prog now uses the {prog.MXPClickableFunctionName()} prog to control visibility.");
	}

	private static void TagParent(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to change the parent of?");
			return;
		}

		var tag = actor.Gameworld.Tags.GetByIdOrName(ss.PopSpeech());
		if (tag is null)
		{
			actor.OutputHandler.Send($"There is no such tag identified by {ss.Last.ColourCommand()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the new parent of the {tag.FullName.ColourName()} tag?");
			return;
		}

		var parent = actor.Gameworld.Tags.GetByIdOrName(ss.SafeRemainingArgument);
		if (parent is null)
		{
			actor.OutputHandler.Send($"There is no such tag identified by {ss.SafeRemainingArgument.ColourCommand()}.");
			return;
		}

		if (parent.IsA(tag))
		{
			actor.OutputHandler.Send(
				$"You cannot make this change as {parent.FullName.ColourName()} is already a {tag.FullName.ColourName()}, and this would create a loop.");
			return;
		}

		var old = tag.FullName.ColourName();
		tag.Parent = parent;
		actor.OutputHandler.Send($"You change the tag from {old} to {tag.FullName.ColourName()}.");
	}

	private static void TagRename(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to change the name of?");
			return;
		}

		var tag = actor.Gameworld.Tags.GetByIdOrName(ss.PopSpeech());
		if (tag is null)
		{
			actor.OutputHandler.Send($"There is no such tag identified by {ss.Last.ColourCommand()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What new name do you want to give to the {tag.FullName.ColourName()} tag?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase().Trim();
		if (actor.Gameworld.Tags.Except(tag).Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"The {actor.Gameworld.Tags.First(x => x.Name.EqualTo(name)).FullName.ColourName()} tag already has the name {name}. Names must be unique.");
			return;
		}

		var old = tag.FullName.ColourName();
		tag.SetName(name);
		actor.OutputHandler.Send($"You rename the tag {old} to {tag.FullName.ColourName()}.");
	}

	private static void TagList(ICharacter actor, StringStack command)
	{
		var topLevel = actor.Gameworld.Tags.Where(x => x.Parent == null).ToList();
		var sb = new StringBuilder();
		sb.AppendLine(
			"There are the following top level tags. To see any of their descendents please use TAG VIEW <tag>:");
		foreach (var tag in topLevel)
		{
			sb.AppendLine($"[{tag.Id.ToString("N0", actor)}] {tag.Name.Colour(Telnet.Cyan)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void TagView(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send($"What tag do you want to view? Syntax is {"tag view \"<tag>\"".Colour(Telnet.Yellow)}.");
			return;
		}

		var tags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (tags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return;
		}

		if (tags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{tags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return;
		}

		var targetTag = tags.Single();
		var children = actor.Gameworld.Tags.Except(targetTag).Where(x => x.IsA(targetTag)).ToList();
		var sb = new StringBuilder();

		void DescribeLevel(ITag tag, int level)
		{
			sb.AppendLine(new string('\t', level) + $"[{tag.Id.ToString("N0", actor)}] {tag.Name}");
			foreach (var branch in children.Where(x => x.Parent == tag).ToList())
			{
				DescribeLevel(branch, level + 1);
			}
		}

		DescribeLevel(targetTag, 0);
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void TagInsert(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What tag do you want to insert? Syntax is {"tag insert \"<tag>\" [\"<old parent>\"]".Colour(Telnet.Yellow)}.");
			return;
		}

		var tagName = command.PopSpeech();
		if (actor.Gameworld.Tags.Any(x => x.Name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already a tag with that name. You must choose a new, unique name for your tag.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to insert this new tag between it and its children?");
			return;
		}

		var parentTag = actor.Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument);
		if (parentTag == null)
		{
			actor.Send("There is no such tag for you to insert between it and its children.");
			return;
		}

		var children = actor.Gameworld.Tags.Where(x => x.Parent == parentTag).ToList();
		var newTag = new Tag(tagName, parentTag, actor.Gameworld);
		actor.Gameworld.Add(newTag);
		foreach (var child in children)
		{
			child.Parent = newTag;
		}

		actor.OutputHandler.Send(
			$"You add the tag {newTag.FullName.Colour(Telnet.Cyan)}, taking all the children of its parent tag.");
	}

	private static void TagRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What tag do you want to remove? Syntax is {"tag delete \"<tag>\"".Colour(Telnet.Yellow)}.");
			return;
		}

		var tags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (tags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return;
		}

		if (tags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{tags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return;
		}

		var targetTag = tags.Single();

		var childrenCount = actor.Gameworld.Tags.Except(targetTag).Count(x => x.IsA(targetTag));
		actor.Send(
			$"Are you sure that you want to permanently delete the {targetTag.FullName.Colour(Telnet.Cyan)} tag? This action cannot be undone.{(childrenCount > 0 ? $"This will also delete all {childrenCount} children tags of this tag." : "")}");
		actor.Send(
			$"Use the command {"accept".Colour(Telnet.Yellow)} to accept or {"decline".Colour(Telnet.Yellow)} to cancel.");
		actor.AddEffect(new Accept(actor, new GenericProposal(
			message =>
			{
				actor.Send($"You delete the {targetTag.FullName.Colour(Telnet.Cyan)} tag.");
				var tagsToDelete =
					actor.Gameworld.Tags.Where(x => x.IsA(targetTag)).ToList();
				foreach (var tag in tagsToDelete)
				{
					tag.GetEditable.Delete();
				}
			},
			message =>
			{
				actor.Send(
					$"You decide not to delete the {targetTag.FullName.Colour(Telnet.Cyan)} tag.");
			},
			() =>
			{
				actor.Send(
					$"You decide not to delete the {targetTag.FullName.Colour(Telnet.Cyan)} tag.");
			},
			$"Proposing to remove the tag {targetTag.FullName.Colour(Telnet.Cyan)}.",
			"tag", "remove", targetTag.Name
		)), TimeSpan.FromSeconds(120));
	}

	private static void TagAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What tag do you want to add? Syntax is {"tag add \"<tag>\" [\"<parent>\"]".Colour(Telnet.Yellow)}.");
			return;
		}

		var tagName = command.PopSpeech();
		if (actor.Gameworld.Tags.Any(x => x.Name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already a tag with that name. You must choose a new, unique name for your tag.");
			return;
		}

		ITag parentTag = null;
		if (!command.IsFinished)
		{
			parentTag = actor.Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument);
			if (parentTag == null)
			{
				actor.Send("There is no such tag for you to make as the parent of your new tag.");
				return;
			}
		}

		var newTag = new Tag(tagName, parentTag, actor.Gameworld);
		actor.Gameworld.Add(newTag);
		actor.OutputHandler.Send(
			$"You add the {(parentTag is null ? "top level " : "")}tag {newTag.FullName.Colour(Telnet.Cyan)}");
	}

	#endregion

	#region Wear Profiles

	#region Wear Profile Subcommands

	private static void WearProfileSet(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which wear profile do you wish to edit?");
			return;
		}

		var profile = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.WearProfiles.Get(value)
			: actor.Gameworld.WearProfiles.GetByName(ss.Last);
		if (profile == null)
		{
			actor.Send("There is no such wear profile for you to edit.");
			return;
		}

		profile.BuildingCommand(actor, ss);
	}

	private static void WearProfileAdd(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send(
				$"Which type of Wear Profile do you want to add? You can add {"direct".Colour(Telnet.Yellow)} or {"shape".Colour(Telnet.Yellow)}.");
			return;
		}

		var type = ss.Pop().ToLowerInvariant();
		switch (type)
		{
			case "direct":
			case "shape":
				break;
			default:
				actor.Send(
					$"That is not a valid type of wear profile. You can add {"direct".Colour(Telnet.Yellow)} or {"shape".Colour(Telnet.Yellow)}.");
				return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What name do you want to give your new wear profile?");
			return;
		}

		var name = ss.SafeRemainingArgument;
		if (actor.Gameworld.WearProfiles.GetByName(name) != null)
		{
			actor.Send("There is already a wear profile with that name. Wear profile names must be unique.");
			return;
		}

		using (new FMDB())
		{
			var dbitem = new Models.WearProfile();
			FMDB.Context.WearProfiles.Add(dbitem);
			dbitem.Name = name;
			dbitem.Description = "An undescribed wear profile";
			dbitem.WearAction1st = "put";
			dbitem.WearAction3rd = "puts";
			dbitem.WearAffix = "on";
			dbitem.Type = type.Proper();
			dbitem.WearStringInventory = "worn on";
			dbitem.WearlocProfiles = "<Profiles/>";
			FMDB.Context.SaveChanges();
			var newWearProfile = GameItems.Inventory.WearProfile.LoadWearProfile(dbitem, actor.Gameworld);
			actor.Gameworld.Add(newWearProfile);
			actor.Send(
				$"You create a new Wear Profile called {name.Colour(Telnet.Green)} with ID {newWearProfile.Id:N0}.");
		}
	}

	#endregion

	[PlayerCommand("WearProfile", "wearprofile")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	protected static void WearProfile(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "add":
				WearProfileAdd(actor, ss);
				break;
			case "set":
				WearProfileSet(actor, ss);
				break;
			default:
				actor.Send($"Valid options are {"add".Colour(Telnet.Yellow)}, and {"set".Colour(Telnet.Yellow)}");
				break;
		}
	}

	#endregion
	
	#region Trait Expressions

	[PlayerCommand("TraitExpression", "traitexpression", "te")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void TraitExpression(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "new":
				TraitExpressionNew(actor, ss);
				return;
			case "edit":
				TraitExpressionEdit(actor, ss);
				return;
			case "close":
				TraitExpressionClose(actor, ss);
				return;
			case "set":
				TraitExpressionSet(actor, ss);
				return;
			case "delete":
				TraitExpressionDelete(actor, ss);
				return;
			case "clone":
				TraitExpressionClone(actor, ss);
				return;
			case "list":
				TraitExpressionList(actor, ss);
				return;
			case "show":
			case "view":
				TraitExpressionView(actor, ss);
				return;
		}

		actor.OutputHandler.Send("The valid choices are list, show, new, edit, close, set, clone and delete.");
	}

	private static void TraitExpressionView(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished && !actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().Any())
		{
			actor.OutputHandler.Send("You must specify a trait expression to show if you are not editing one.");
			return;
		}

		ITraitExpression expression;
		if (ss.IsFinished)
		{
			expression = actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().First().EditingItem;
		}
		else
		{
			expression = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.TraitExpressions.Get(value)
				: actor.Gameworld.TraitExpressions.GetByName(ss.Last);
			if (expression == null)
			{
				actor.OutputHandler.Send("There is no such trait expression to show you.");
				return;
			}
		}

		actor.OutputHandler.Send(expression.ShowBuilder(actor));
	}

	private static void TraitExpressionList(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in actor.Gameworld.TraitExpressions
			select new[]
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.Formula.OriginalExpression
			},
			new[] { "ID", "Name", "Formula" },
			actor.LineFormatLength, truncatableColumnIndex: 2, colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void TraitExpressionClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which trait expression would you like to clone?");
			return;
		}

		var expression = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.TraitExpressions.Get(value)
			: actor.Gameworld.TraitExpressions.GetByName(ss.Last);
		if (expression == null)
		{
			actor.OutputHandler.Send("There is no such trait expression to clone.");
			return;
		}

		var newexpr = new TraitExpression((TraitExpression)expression);
		actor.Gameworld.Add(newexpr);
		actor.OutputHandler.Send(
			$"You clone trait expression #{expression.Id.ToString("N0", actor)} into a new expression with id #{newexpr.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ITraitExpression>>());
		actor.AddEffect(new BuilderEditingEffect<ITraitExpression>(actor) { EditingItem = newexpr });
	}

	private static void TraitExpressionDelete(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any trait expressions.");
			return;
		}

		actor.OutputHandler.Send("TODO");
	}

	private static void TraitExpressionSet(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any trait expressions.");
			return;
		}

		actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().First().EditingItem.BuildingCommand(actor, ss);
	}

	private static void TraitExpressionClose(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any trait expressions.");
			return;
		}

		actor.OutputHandler.Send($"You are no longer editing any trait expressions.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ITraitExpression>>());
	}

	private static void TraitExpressionEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			if (actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().Any())
			{
				TraitExpressionView(actor, ss);
				return;
			}

			actor.OutputHandler.Send("Which trait expression would you like to edit?");
			return;
		}

		var expr = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.TraitExpressions.Get(value)
			: actor.Gameworld.TraitExpressions.GetByName(ss.Last);
		if (expr == null)
		{
			actor.OutputHandler.Send("There is no such trait expression to edit.");
			return;
		}

		actor.OutputHandler.Send($"You are now editing trait expression #{expr.Id.ToString("N0", actor)}.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ITraitExpression>>());
		actor.AddEffect(new BuilderEditingEffect<ITraitExpression>(actor) { EditingItem = expr });
	}

	private static void TraitExpressionNew(ICharacter actor, StringStack ss)
	{
		var name = "Unnamed Trait Expression";
		if (!ss.IsFinished)
		{
			name = ss.SafeRemainingArgument;
			if (actor.Gameworld.TraitExpressions.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a Trait Expression with that name. Names must be unique.");
				return;
			}
		}

		var newexpr = new TraitExpression(actor.Gameworld, name);

		actor.Gameworld.Add(newexpr);
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ITraitExpression>>());
		actor.AddEffect(new BuilderEditingEffect<ITraitExpression>(actor) { EditingItem = newexpr });
		actor.OutputHandler.Send(
			$"You create a new trait expression with ID #{newexpr.Id}, which you are now editing.");
	}

	#endregion

	#region Materials

	private const string GasHelpText =
		@"The gas command allows you to create and edit gases and their properties. These gases in turn can be used by items, characters and other effects.

The syntax for this command is as follows:

	#3gas list#0 - Lists all gases
	#3gas show <which>#0 - shows information about a gases
	#3gas edit <which>#0 - begins editing a gas
	#3gas edit#0 - same as #3material show#0 for your currently edited gas
	#3gas clone <which> <new name> <new description>#0 - clones a gas
	#3gas new <name>#0 - creates a new gas
	#3gas set organic#0 - toggles counting as organic
	#3gas set description <description>#0 - sets the description
	#3gas set density <value>#0 - sets density in kg/m3
	#3gas set electrical <value>#0 - sets electrical conductivity in siemens
	#3gas set thermal <value>#0 - sets the thermal conductivity in watts/kelvin
	#3gas set specificheat <value>#0 - sets the specific heat capacity in J/Kg.Kelvin
	#3gas set colour <ansi>#0 - sets the display colour
	#3gas set drug <which>#0 - sets the contained drug
	#3gas set drug none#0 - clears the drug
	#3gas set drugvolume <amount>#0 - sets the drug volume
	#3gas set viscosity <viscosity>#0 - sets the viscosity in cSt
	#3gas set smell <intensity> <smell> [<vague smell>]#0 - sets the smell
	#3gas set countsas <gas>#0 - sets a gas that this counts as
	#3gas set countsas none#0 - clears a counts-as gas
	#3gas set quality <quality>#0 - sets the maximum quality of the gas when counting-as
	#3gas set condensation <temp>|none#0 - sets or clears the temperature at which this gas becomes a liquid
	#3gas set liquid <liquid>|none#0 - sets or clears the liquid form of this gas";

	[PlayerCommand("Gas", "gas")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Gas(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "clone":
				GasClone(actor, ss);
				return;
			case "new":
				GasNew(actor, ss);
				return;
			case "edit":
				GasEdit(actor, ss);
				return;
			case "set":
				GasSet(actor, ss);
				return;
			case "close":
				GasClose(actor, ss);
				return;
			case "show":
			case "view":
				GasShow(actor, ss);
				return;
			case "list":
				GasList(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(GasHelpText.SubstituteANSIColour());
				return;
		}
	}


	private static void GasShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<IGas>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send(
					"You are not editing any gases. You must either edit one or specify which one you'd like to show.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var gas = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Gases.Get(value)
			: actor.Gameworld.Gases.GetByName(ss.Last);
		if (gas == null)
		{
			actor.OutputHandler.Send("There is no such gas.");
			return;
		}

		actor.OutputHandler.Send(gas.Show(actor));
	}


	private static void GasClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IGas>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any gases.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IGas>>();
		actor.OutputHandler.Send("You are no longer editing any gases.");
	}

	private static void GasSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IGas>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any gases.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void GasEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<IGas>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("You must specify a gas that you wish to edit.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var gas = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Gases.Get(value)
			: actor.Gameworld.Gases.GetByName(ss.Last);
		if (gas == null)
		{
			actor.OutputHandler.Send("There is no such gas.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IGas>>();
		actor.AddEffect(new BuilderEditingEffect<IGas>(actor) { EditingItem = gas });
		actor.OutputHandler.Send($"You are now editing gas {gas.Name.Colour(gas.DisplayColour)}.");
	}

	private static void GasNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give your new gas?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Gases.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a gas with that name. Names must be unique.");
			return;
		}

		var newGas = new Gas(name, actor.Gameworld);
		actor.OutputHandler.Send(
			$"You create a new gas called {name.Colour(newGas.DisplayColour)} with ID #{newGas.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IGas>>();
		actor.AddEffect(new BuilderEditingEffect<IGas>(actor) { EditingItem = newGas });
	}

	private static void GasClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which gas do you want to clone?");
			return;
		}

		var gas = actor.Gameworld.Gases.GetByIdOrName(ss.PopSpeech());
		if (gas == null)
		{
			actor.OutputHandler.Send("There is no such gas.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to your cloned gas?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Gases.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a gas with that name. Gas names must be unique.");
			return;
		}

		var newMaterial = gas.Clone(name);
		actor.OutputHandler.Send(
			$"You clone the gas {gas.Name.Colour(gas.DisplayColour)} as {name.Colour(gas.DisplayColour)} with ID #{newMaterial.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IGas>>();
		actor.AddEffect(new BuilderEditingEffect<IGas>(actor) { EditingItem = newMaterial });
	}

	private static void GasList(ICharacter actor, StringStack command)
	{
		var gases = actor.Gameworld.Gases.AsEnumerable();
		while (!command.IsFinished)
		{
			// Filter
			var cmd = command.PopSpeech();
			switch (cmd.ToLowerInvariant().CollapseString())
			{
				default:
					actor.OutputHandler.Send($"The text {cmd.ColourCommand()} is not a valid filter for gases.");
					return;
			}
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from gas in gases
			select new List<string>
			{
				gas.Id.ToString("N0", actor),
				gas.Name,
				gas.MaterialDescription.Colour(gas.DisplayColour),
				gas.CountsAs is not null
					? $"{gas.CountsAs.Name.Colour(gas.CountsAs.DisplayColour)} @ {gas.CountsAsQuality.Describe().ColourValue()}"
					: "",
				gas.Drug is not null
					? $"{gas.DrugGramsPerUnitVolume.ToString("N3", actor).ColourValue()}g/L {gas.Drug.Name.ColourName()}"
					: ""
			},
			new List<string>
			{
				"Id",
				"Name",
				"Description",
				"Count As",
				"Drug"
			},
			actor,
			Telnet.Orange
		));
	}

	private const string LiquidHelpText =
		@"The material command allows you to create and edit solid materials and their properties. These materials in turn can be used by items, characters and other effects.

The syntax for this command is as follows:

	#3liquid list#0 - Lists all liquids
	#3liquid show <which>#0 - shows information about a liquid
	#3liquid edit <which>#0 - begins editing a liquid
	#3liquid edit#0 - same as #3liquid show#0 for your currently edited liquid
	#3liquid clone <which> <new name> <new description>#0 - clones a liquid
	#3liquid new <name>#0 - creates a new liquid
	#3liquid set organic#0 - toggles counting as organic
	#3liquid set description <description>#0 - sets the description
	#3liquid set density <value>#0 - sets density in kg/m3
	#3liquid set electrical <value>#0 - sets electrical conductivity in siemens
	#3liquid set thermal <value>#0 - sets the thermal conductivity in watts/kelvin
	#3liquid set specificheat <value>#0 - sets the specific heat capacity in J/Kg.Kelvin
	#3liquid set colour <ansi>#0 - sets the display colour
	#3liquid set drug <which>#0 - sets the contained drug
	#3liquid set drug none#0 - clears the drug
	#3liquid set drugvolume <amount>#0 - sets the drug volume
	#3liquid set viscosity <viscosity>#0 - sets the viscosity in cSt
	#3liquid set smell <intensity> <smell> [<vague smell>]#0 - sets the smell
	#3liquid set taste <intensity> <taste> [<vague taste>]#0 - sets the taste
	#3liquid set ldesc <desc>#0 - sets the more detailed description when looked at
	#3liquid set alcohol <litres per litre>#0 - how many litres of pure alcohol per litre of liquid
	#3liquid set thirst <hours>#0 - how many hours of thirst quenched per litre drunk
	#3liquid set hunger <hours>#0 - how many hours of hunger quenched per litre drunk
	#3liquid set water <litres per litre>#0 - how many litres of hydrating water per litre of liquid#B*#0
	#3liquid set calories <calories per litre>#0 - how many calories per litre of liquid#B*#0
	#3liquid set prog <which>#0 - sets a prog to be executed when the liquid is drunk
	#3liquid set prog none#0 - clears the draught prog
	#3liquid set solvent <liquid>#0 - sets a solvent required for cleaning this liquid off things
	#3liquid set solvent none#0 - no solvent required for cleaning
	#3liquid set solventratio <percentage>#0 - sets the volume of solvent to contaminant required
	#3liquid set residue <solid>#0 - sets a material to leave behind as a residue when dry
	#3liquid set residue none#0 - dry clean, leave no residue
	#3liquid set residueamount <percentage>#0 - percentage of weight of liquid that is residue
	#3liquid set countsas <liquid>#0 - sets another liquid that this one counts as
	#3liquid set countsas none#0 - this liquid counts as no other liquid
	#3liquid set countquality <quality>#0 - sets the maximum quality for this when counting as
	#3liquid set freeze <temp>#0 - sets the freeze temperature of this liquid#B*#0
	#3liquid set boil <temp>#0 - sets the boil temperature of this liquid#B*#0
	#3liquid set ignite <temp>#0 - sets the ignite temperature of this liquid#B*#0
	#3liquid set ignite none#0 - clears the ignite temperature of this liquid#B*#0

#9Note#0: Liquid properties marked with a #B*#0 above are currently not used by the engine but will see inclusion in the future.";

	[PlayerCommand("Liquid", "liquid")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Liquid(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "clone":
				LiquidClone(actor, ss);
				return;
			case "new":
				LiquidNew(actor, ss);
				return;
			case "edit":
				LiquidEdit(actor, ss);
				return;
			case "set":
				LiquidSet(actor, ss);
				return;
			case "close":
				LiquidClose(actor, ss);
				return;
			case "show":
			case "view":
				LiquidShow(actor, ss);
				return;
			case "list":
				LiquidList(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(LiquidHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void LiquidList(ICharacter actor, StringStack command)
	{
		var liquids = actor.Gameworld.Liquids.AsEnumerable();
		while (!command.IsFinished)
		{
			// Filter
			var cmd = command.PopSpeech();
			switch (cmd.ToLowerInvariant().CollapseString())
			{
				default:
					actor.OutputHandler.Send($"The text {cmd.ColourCommand()} is not a valid filter for liquids.");
					return;
			}
		}

		actor.Send(
			StringUtilities.GetTextTable(
				from liquid in liquids
				select new[]
				{
					liquid.Id.ToString("N0", actor),
					liquid.Name.TitleCase(),
					liquid.MaterialDescription.Proper().Colour(liquid.DisplayColour),
					liquid.Organic.ToColouredString(),
					liquid.WaterLitresPerLitre.ToString("P0", actor),
					liquid.DrinkSatiatedHoursPerLitre.ToString("N1", actor),
					liquid.CaloriesPerLitre.ToString("N0", actor),
					liquid.FoodSatiatedHoursPerLitre.ToString("N1", actor),
					liquid.AlcoholLitresPerLitre.ToString("P0", actor),
					liquid.DraughtProg?.MXPClickableFunctionNameWithId() ?? "None",
					liquid.Drug != null
						? $"{liquid.DrugGramsPerUnitVolume.ToString("N4", actor).ColourValue()}g/L {liquid.Drug.Name.ColourName()}"
						: ""
				},
				new[]
				{
					"ID",
					"Name",
					"Description",
					"Organic",
					"Water",
					"Thirst",
					"Calories",
					"Food",
					"Alcohol",
					"Draught",
					"Drug"
				},
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void LiquidShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<ILiquid>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send(
					"You are not editing any liquids. You must either edit one or specify which one you'd like to show.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var liquid = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Liquids.Get(value)
			: actor.Gameworld.Liquids.GetByName(ss.Last);
		if (liquid == null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return;
		}

		actor.OutputHandler.Send(liquid.Show(actor));
	}

	private static void LiquidClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ILiquid>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any liquids.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ILiquid>>();
		actor.OutputHandler.Send("You are no longer editing any liquids.");
	}

	private static void LiquidSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ILiquid>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any liquids.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void LiquidEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<ILiquid>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("You must specify a liquid that you wish to edit.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var liquid = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Liquids.Get(value)
			: actor.Gameworld.Liquids.GetByName(ss.Last);
		if (liquid == null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ILiquid>>();
		actor.AddEffect(new BuilderEditingEffect<ILiquid>(actor) { EditingItem = liquid });
		actor.OutputHandler.Send($"You are now editing liquid {liquid.Name.Colour(liquid.DisplayColour)}.");
	}

	private static void LiquidNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give your new liquid?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Liquids.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a liquid with that name. Names must be unique.");
			return;
		}

		var newLiquid = new Liquid(name, actor.Gameworld);
		actor.OutputHandler.Send(
			$"You create a new liquid called {name.Colour(newLiquid.DisplayColour)} with ID #{newLiquid.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ILiquid>>();
		actor.AddEffect(new BuilderEditingEffect<ILiquid>(actor) { EditingItem = newLiquid });
	}

	private static void LiquidClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which liquid do you want to clone?");
			return;
		}

		var liquid = actor.Gameworld.Liquids.GetByIdOrName(ss.PopSpeech());
		if (liquid == null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to your cloned liquid?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Liquids.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a liquid with that name. Liquid names must be unique.");
			return;
		}

		var newMaterial = liquid.Clone(name);
		actor.OutputHandler.Send(
			$"You clone the liquid {liquid.Name.Colour(liquid.DisplayColour)} as {name.Colour(liquid.DisplayColour)} with ID #{newMaterial.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ILiquid>>();
		actor.AddEffect(new BuilderEditingEffect<ILiquid>(actor) { EditingItem = newMaterial });
	}

	private const string MaterialHelpText =
		@"The material command allows you to create and edit solid materials and their properties. These materials in turn can be used by items, characters and other effects.

The syntax for this command is as follows:

	#3material list#0 - Lists all materials
	#3material show <which>#0 - shows information about a material
	#3material edit <which>#0 - begins editing a material
	#3material edit#0 - same as #3material show#0 for your currently edited material
	#3material clone <which> <new name> <new description>#0 - clones a material
	#3material new <type> <name>#0 - creates a new material of the specified type
	#3material set organic#0 - toggles counting as organic
	#3material set description <description>#0 - sets the description
	#3material set density <value>#0 - sets density in kg/m3
	#3material set electrical <value>#0 - sets electrical conductivity in siemens
	#3material set thermal <value>#0 - sets the thermal conductivity in watts/kelvin
	#3material set specificheat <value>#0 - sets the specific heat capacity in J/Kg.Kelvin
	#3material set impactyield <value>#0 - sets the impact yield strength in kPa
	#3material set impactfracture <value>#0 - sets the impact fracture strength in kPa
	#3material set impactstrain <value>#0 - sets the strain at yield for impact
	#3material set shearyield <value>#0 - sets the shear yield strength in kPa
	#3material set shearfracture <value>#0 - sets the shear fracture strength in kPa
	#3material set shearstrain <value>#0 - sets the strain at yield for shear
	#3material set heatdamage <temp>|none#0 - sets or clears the temperature for heat damage
	#3material set ignition <temp>|none#0 - sets or clears the temperature for ignition
	#3material set melting <temp>|none#0 - sets or clears the temperature for melting
	#3material set absorbency <%>#0 - sets the absorbency for liquids
	#3material set solvent <liquid>|none#0 - sets or clears the required solvent for residues
	#3material set solventratio <%>#0 - sets the ratio of solvent to removed contaminant by mass
	#3material set liquidform <liquid>|none#0 - sets or clears a liquid as the liquid form of this
	#3material set residuecolour <colour>#0 - sets the colour of this material and its residues
	#3material set residuesdesc <tag>|none#0 - sets or clears the sdesc tag for residues of this
	#3material set residuedesc <desc>|none#0 - sets or clears the added description for residues of this";

	[PlayerCommand("Material", "material")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("material", MaterialHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Material(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "clone":
				MaterialClone(actor, ss);
				return;
			case "new":
				MaterialNew(actor, ss);
				return;
			case "edit":
				MaterialEdit(actor, ss);
				return;
			case "set":
				MaterialSet(actor, ss);
				return;
			case "close":
				MaterialClose(actor, ss);
				return;
			case "show":
			case "view":
				MaterialShow(actor, ss);
				return;
			case "list":
				MaterialList(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(MaterialHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void MaterialShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<ISolid>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send(
					"You are not editing any material. You must either edit one or specify which one you'd like to show.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var material = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Materials.Get(value)
			: actor.Gameworld.Materials.GetByName(ss.Last);
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return;
		}

		actor.OutputHandler.Send(material.Show(actor));
	}

	private static void MaterialClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ISolid>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any materials.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ISolid>>();
		actor.OutputHandler.Send("You are no longer editing any materials.");
	}

	private static void MaterialSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ISolid>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any materials.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void MaterialEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<ISolid>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("You must specify a material that you wish to edit.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var material = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Materials.Get(value)
			: actor.Gameworld.Materials.GetByName(ss.Last);
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ISolid>>();
		actor.AddEffect(new BuilderEditingEffect<ISolid>(actor) { EditingItem = material });
		actor.OutputHandler.Send($"You are now editing material {material.Name.Colour(Telnet.Yellow)}.");
	}

	private static void MaterialNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What behaviour type did you want to set for your new material? Valid options are: {Enum.GetValues(typeof(MaterialBehaviourType)).OfType<MaterialBehaviourType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return;
		}

		if (!ss.PopSpeech().TryParseEnum<MaterialBehaviourType>(out var behaviour))
		{
			actor.OutputHandler.Send(
				$"That is not a valid behaviour type. Valid options are: {Enum.GetValues(typeof(MaterialBehaviourType)).OfType<MaterialBehaviourType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give your new material?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Materials.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a material with that name. Names must be unique.");
			return;
		}

		var newMaterial = new Solid(name, behaviour, actor.Gameworld);
		actor.OutputHandler.Send(
			$"You create a new {behaviour.DescribeEnum().ColourValue()} material called {name.Colour(newMaterial.ResidueColour)} with ID #{newMaterial.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ISolid>>();
		actor.AddEffect(new BuilderEditingEffect<ISolid>(actor) { EditingItem = newMaterial });
	}

	private static void MaterialClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which material do you want to clone?");
			return;
		}

		var material = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Materials.Get(value)
			: actor.Gameworld.Materials.GetByName(ss.Last);
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to your cloned material?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Materials.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a material with that name. Material names must be unique.");
			return;
		}

		var description = name;
		if (!ss.IsFinished)
		{
			description = ss.PopSpeech();
		}

		var newMaterial = material.Clone(name, description);
		actor.OutputHandler.Send(
			$"You clone the material {material.Name.Colour(material.ResidueColour)} as {name.Colour(material.ResidueColour)} with ID #{newMaterial.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ISolid>>();
		actor.AddEffect(new BuilderEditingEffect<ISolid>(actor) { EditingItem = newMaterial });
	}

	private static void MaterialList(ICharacter actor, StringStack command)
	{
		var materials = actor.Gameworld.Materials.AsEnumerable();
		while (!command.IsFinished)
		{
			if (!command.PopSpeech().TryParseEnum<MaterialBehaviourType>(out var materialType))
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
					material.MaterialDescription.Colour(material.ResidueColour),
					material.BehaviourType.DescribeEnum(),
					$"{material.Density.ToString("N0").ColourValue()}kg/m3",
					material.Solvent is not null
						? $"{material.SolventRatio.ToString("P2", actor).ColourValue()} x {material.Solvent.Name.Colour(material.Solvent.DisplayColour)}"
						: ""
				},
				new[]
				{
					"ID",
					"Name",
					"Description",
					"Type",
					"Density",
					"Solvent"
				},
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	#endregion

	#region Tattoos

	protected const string TattooHelp =
		@$"The tattoo command is used to view, create and inscribe tattoos. Players are able to create and submit their own tattoos but only admins can approve them.

The following options are used to view, edit and create tattoo designs:

	#3tattoo list [all|mine|+key|-key]#0 - lists all tattoos
	#3tattoo edit <id|name>#0 - opens the specified tattoo for editing
	#3tattoo edit new <name>#0 - creates a new tattoo for editing
	#3tattoo edit#0 - equivalent of doing SHOW on your currently editing tattoo
	#3tattoo clone <id|name> <new name>#0 - creates a carbon copy of a tattoo for editing
	#3tattoo show <id|name>#0 - shows a particular tattoo.
	#3tattoo set <subcommand>#0 - changes something about the tattoo. See its help for more info.
	#3tattoo edit submit#0 - submits a tattoo for review

{GenericReviewableSearchList}

The following commands are used to put tattoos on people:

	tattoo inscribe <target> <tattoo id|name> <bodypart> - begins inscribing a tattoo on someone
	tattoo continue <target> <tattoo keyword> - continues inscribing an unfinished tattoo on someone";

	protected const string TattooAdminHelp =
		@$"The tattoo command is used to view, create and inscribe tattoos. Players are able to create and submit their own tattoos but only admins can approve them.

The following options are used to view, edit and create tattoo designs:

	#3tattoo list [all|mine|+key|-key]#0 - lists all tattoos
	#3tattoo edit <id|name>#0 - opens the specified tattoo for editing
	#3tattoo edit new <name>#0 - creates a new tattoo for editing
	#3tattoo edit#0 - equivalent of doing SHOW on your currently editing tattoo
	#3tattoo clone <id|name> <new name>#0 - creates a carbon copy of a tattoo for editing
	#3tattoo show <id|name>#0 - shows a particular tattoo.
	#3tattoo set <subcommand>#0 - changes something about the tattoo. See its help for more info.
	#3tattoo edit submit#0 - submits a tattoo for review
	#3tattoo review all|mine|<id|name>#0 - reviews a submitted tattoo
	#3tattoo review list#0 - shows all tattoos submitted for review

{GenericReviewableSearchList}

The following commands are used to put tattoos on people:

	#3tattoo inscribe <target> <tattoo id|name> <bodypart>#0 - begins inscribing a tattoo on someone
	#3tattoo continue <target> <tattoo keyword>#0 - continues inscribing an unfinished tattoo on someone

Also, as an admin you should see the two related commands #3GIVETATTOO#0 and #3FINISHTATTOO#0.";

	[PlayerCommand("Tattoo", "tattoo")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoCombatCommand]
	[NoHideCommand]
	[NoMovementCommand]
	[HelpInfo("tattoo", TattooHelp, AutoHelp.HelpArgOrNoArg, TattooAdminHelp)]
	protected static void Tattoo(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(TattooHelp.SubstituteANSIColour());
			return;
		}

		switch (ss.PopSpeech())
		{
			case "list":
				TattooList(actor, ss);
				break;
			case "edit":
				TattooEdit(actor, ss);
				break;
			case "set":
				TattooSet(actor, ss);
				break;
			case "clone":
				TattooClone(actor, ss);
				break;
			case "show":
			case "view":
				TattooView(actor, ss);
				break;
			case "review":
				TattooReview(actor, ss);
				break;
			case "inscribe":
				TattooInscribe(actor, ss);
				break;
			case "continue":
				TattooContinue(actor, ss);
				break;
			case "help":
			case "?":
			default:
				actor.OutputHandler.Send(TattooHelp.SubstituteANSIColour());
				return;
		}
	}

	private static IInventoryPlanTemplate _tattooNeedlePlan;

	private static IInventoryPlanTemplate TattooNeedlePlan
	{
		get
		{
			if (_tattooNeedlePlan == null)
			{
				_tattooNeedlePlan = new InventoryPlanTemplate(Futuremud.Games.First(), new[]
				{
					new InventoryPlanPhaseTemplate(1, new[]
					{
						new InventoryPlanActionHold(Futuremud.Games.First(),
							Futuremud.Games.First().GetStaticLong("TattooNeedleTag"), 0, null, null)
						{
							OriginalReference = "tattoo tool",
							ItemsAlreadyInPlaceOverrideFitnessScore = true
						}
					})
				});
			}

			return _tattooNeedlePlan;
		}
	}

	private static void TattooContinue(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Whose unfinished tattoo do you want to continue work on?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You do not see anyone like that.");
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You cannot work on your own tattoos.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which tattoo of {target.ApparentGender(actor).GeneralPossessive()} do you want to finish?");
			return;
		}

		var tattoo = target.Body.Tattoos.Where(x => target.Body.ExposedBodyparts.Contains(x.Bodypart))
		                   .GetFromItemListByKeyword(ss.PopSpeech(), actor);
		if (tattoo == null)
		{
			actor.OutputHandler.Send(
				$"You cannot see any such tattoo on {target.HowSeen(actor, false, DescriptionType.Possessive)} body.");
			return;
		}

		if (tattoo.CompletionPercentage >= 1.0)
		{
			actor.OutputHandler.Send("That tattoo is already complete and needs no further work.");
			return;
		}

		if (!tattoo.TattooTemplate.CanProduceTattoo(actor))
		{
			actor.OutputHandler.Send("You don't know how to finish that tattoo off.");
			return;
		}

		var plan = TattooNeedlePlan.CreatePlan(actor);
		switch (plan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				actor.OutputHandler.Send(
					$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				actor.OutputHandler.Send(
					$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				actor.OutputHandler.Send($"You are lacking a tool with which to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
		}

		void BeginTattooAction()
		{
			if (!CharacterState.Able.HasFlag(actor.State))
			{
				actor.OutputHandler.Send(
					$"You can no longer continue to work on that tattoo because you are {actor.State.Describe()}.");
				return;
			}

			plan = TattooNeedlePlan.CreatePlan(actor);
			switch (plan.PlanIsFeasible())
			{
				case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
					actor.OutputHandler.Send(
						$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
				case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
					actor.OutputHandler.Send(
						$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
				case InventoryPlanFeasibility.NotFeasibleMissingItems:
					actor.OutputHandler.Send($"You are lacking a tool with which to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
			}

			if (!actor.ColocatedWith(target))
			{
				actor.OutputHandler.Send("Your target is no longer in the same location as you.");
				return;
			}

			if (!target.Body.Tattoos.Contains(tattoo))
			{
				actor.OutputHandler.Send("The target of your tattoo inscription no longer has the tattoo...");
				return;
			}

			if (!target.Body.ExposedBodyparts.Contains(tattoo.Bodypart))
			{
				actor.OutputHandler.Send(
					"The target bodypart for your tattoo inscription is covered up. You must have bare skin to work with.");
				return;
			}

			if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
			{
				actor.OutputHandler.Send(
					$"You must first stop {actor.Effects.First(x => x.IsBlockingEffect("general")).BlockingDescription("general", actor)} before you can begin inscribing a tattoo.");
				return;
			}

			if (actor.Movement != null || target.Movement != null)
			{
				actor.OutputHandler.Send("Neither you nor your target can be moving if you want to inscribe a tattoo.");
				return;
			}

			if (actor.Combat != null || target.Combat != null)
			{
				actor.OutputHandler.Send("You cannot inscribe tattoos while you or your target are in combat.");
				return;
			}

			if (target.EffectsOfType<HavingTattooInked>().Any())
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true)} is already having a tattoo inked. Only one tattoo can be worked on at a time.");
				return;
			}

			var inkplan = tattoo.TattooTemplate.GetInkPlan(actor);
			if (inkplan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
			{
				actor.OutputHandler.Send("You do not have the inks you require to work on that tattoo.");
				return;
			}

			var results = plan.ExecuteWholePlan();
			inkplan.ExecuteWholePlan();
			plan.FinalisePlanNoRestore();
			inkplan.FinalisePlanNoRestore();
			actor.AddEffect(
				new InkingTattoo(actor, target, tattoo,
					results.First(x => x.OriginalReference?.ToString().EqualTo("tattoo tool") == true).PrimaryTarget),
				InkingTattoo.EffectTimespan);
		}

		if (!target.UnableToResistInterventions(actor))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ are|is proposing to begin continue working on $1's !2.", actor, actor, target,
				new DummyPerceivable(
					perc => tattoo.ShortDescription.SubstituteWrittenLanguage(perc, actor.Gameworld).Strip_A_An(),
					perc => ""))));
			target.OutputHandler.Send(Accept.StandardAcceptPhrasing);
			target.AddEffect(new Accept(target, new GenericProposal
			{
				AcceptAction = perc => BeginTattooAction(),
				RejectAction = perc =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decide|decides against getting tattoo work from $1.", target, target, actor)));
				},
				ExpireAction = () =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decide|decides against getting tattoo work from $1.", target, target, actor)));
				},
				DescriptionString = "Proposing to continue a tattoo",
				Keywords = new List<string> { "tattoo", "inscribe" }
			}), TimeSpan.FromSeconds(120));
		}
		else
		{
			BeginTattooAction();
		}
	}

	private static void TattooInscribe(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which tattoo do you want to inscribe?");
			return;
		}

		if ((long.TryParse(ss.PopSpeech(), out var value)
			    ? actor.Gameworld.DisfigurementTemplates.Get(value)
			    : actor.Gameworld.DisfigurementTemplates.GetByName(ss.Last)) is not ITattooTemplate template ||
		    !template.CanSeeTattooInList(actor))
		{
			actor.OutputHandler.Send("There is no such tattoo.");
			return;
		}

		if (template.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send("That tattoo template is not approved for use.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to inscribe that tatoo on?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that here.");
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You are not able to inscribe tattoos on yourself.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which bodypart do you want to put the tattoo on?");
			return;
		}

		if (target.Body.GetTargetBodypart(ss.PopSpeech()) is not IExternalBodypart bodypart)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} does not have any such bodypart.");
			return;
		}

		if (!target.Body.ExposedBodyparts.Contains(bodypart))
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true, DescriptionType.Possessive)} {bodypart.FullDescription()} is covered up. You must have bare skin to tattoo.");
			return;
		}

		if (!template.CanBeAppliedToBodypart(target.Body, bodypart))
		{
			actor.OutputHandler.Send(
				$"The {template.Name.Colour(Telnet.Cyan)} tattoo cannot be applied to the {bodypart.FullDescription().ColourValue()} bodypart.");
			return;
		}

		if (!template.CanProduceTattoo(actor))
		{
			actor.OutputHandler.Send("You are not capable of inscribing that tattoo.");
			return;
		}

		// TODO - skill requirements
		var plan = TattooNeedlePlan.CreatePlan(actor);
		switch (plan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				actor.OutputHandler.Send(
					$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				actor.OutputHandler.Send(
					$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				actor.OutputHandler.Send($"You are lacking a tool with which to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
		}

		void BeginTattooAction()
		{
			if (!CharacterState.Able.HasFlag(actor.State))
			{
				actor.OutputHandler.Send(
					$"You can no longer begin a tattoo inscription because you are {actor.State.Describe()}.");
				return;
			}

			plan = TattooNeedlePlan.CreatePlan(actor);
			switch (plan.PlanIsFeasible())
			{
				case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
					actor.OutputHandler.Send(
						$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
				case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
					actor.OutputHandler.Send(
						$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
				case InventoryPlanFeasibility.NotFeasibleMissingItems:
					actor.OutputHandler.Send($"You are lacking a tool with which to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
			}

			if (!actor.ColocatedWith(target))
			{
				actor.OutputHandler.Send("Your target is no longer in the same location as you.");
				return;
			}

			if (!target.Body.Bodyparts.Contains(bodypart))
			{
				actor.OutputHandler.Send(
					"The target of your tattoo inscription no longer has the bodypart you were planning on tattooing.");
				return;
			}

			if (!target.Body.ExposedBodyparts.Contains(bodypart))
			{
				actor.OutputHandler.Send(
					"The target bodypart for your tattoo inscription is covered up. You must have bare skin to work with.");
				return;
			}

			if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
			{
				actor.OutputHandler.Send(
					$"You must first stop {actor.Effects.First(x => x.IsBlockingEffect("general")).BlockingDescription("general", actor)} before you can begin inscribing a tattoo.");
				return;
			}

			if (actor.Movement != null || target.Movement != null)
			{
				actor.OutputHandler.Send("Neither you nor your target can be moving if you want to inscribe a tattoo.");
				return;
			}

			if (actor.Combat != null || target.Combat != null)
			{
				actor.OutputHandler.Send("You cannot inscribe tattoos while you or your target are in combat.");
				return;
			}

			if (target.EffectsOfType<HavingTattooInked>().Any())
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true)} is already having a tattoo inked. Only one tattoo can be worked on at a time.");
				return;
			}

			var inkplan = template.GetInkPlan(actor);
			if (inkplan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
			{
				actor.OutputHandler.Send("You do not have the inks you require to work on that tattoo.");
				return;
			}

			var results = plan.ExecuteWholePlan();
			inkplan.ExecuteWholePlan();
			plan.FinalisePlanNoRestore();
			inkplan.FinalisePlanNoRestore();
			var tattoo = template.ProduceTattoo(actor, target, bodypart);
			target.Body.AddTattoo(tattoo);
			actor.AddEffect(
				new InkingTattoo(actor, target, tattoo,
					results.First(x => x.OriginalReference?.ToString().EqualTo("tattoo tool") == true).PrimaryTarget),
				InkingTattoo.EffectTimespan);
		}

		if (!target.UnableToResistInterventions(actor))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ are|is proposing to begin inscribing a tattoo on $1's {bodypart.FullDescription()}.", actor, actor,
				target)));
			target.OutputHandler.Send(Accept.StandardAcceptPhrasing);
			target.AddEffect(new Accept(target, new GenericProposal
			{
				AcceptAction = perc => BeginTattooAction(),
				RejectAction = perc =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decide|decides against getting a tattoo from $1.", target, target, actor)));
				},
				ExpireAction = () =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decide|decides against getting a tattoo from $1.", target, target, actor)));
				},
				DescriptionString = "Proposing to inscribe a tattoo",
				Keywords = new List<string> { "tattoo", "inscribe" }
			}), TimeSpan.FromSeconds(120));
		}
		else
		{
			BeginTattooAction();
		}
	}

	private static void TattooReview(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators can review tattoos at this time.");
			return;
		}

		GenericReview(actor, command, EditableRevisableItemHelper.TattooHelper);
	}

	private static void TattooView(ICharacter actor, StringStack command)
	{
		GenericRevisableShow(actor, command, EditableRevisableItemHelper.TattooHelper);
	}

	private static void TattooClone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tattoo do you want to clone?");
			return;
		}

		var tattoo =
			(long.TryParse(command.PopSpeech(), out var value)
				? actor.Gameworld.DisfigurementTemplates.Get(value)
				: actor.Gameworld.DisfigurementTemplates.GetByName(command.Last)) as ITattooTemplate;
		if (tattoo == null)
		{
			actor.OutputHandler.Send("There is no such tattoo.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new tattoo?");
			return;
		}

		var name = command.PopSpeech();
		if (actor.Gameworld.DisfigurementTemplates.OfType<ITattooTemplate>().Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a tattoo with that name. Tattoo names must be unique.");
			return;
		}

		var newTattoo = (ITattooTemplate)tattoo.Clone(actor.Account, name);
		actor.Gameworld.Add(newTattoo);
		actor.RemoveAllEffects<BuilderEditingEffect<ITattooTemplate>>();
		actor.AddEffect(new BuilderEditingEffect<ITattooTemplate>(actor) { EditingItem = newTattoo });
		actor.OutputHandler.Send(
			$"You clone the {tattoo.Name.ColourName()} into a new tattoo, called {name.ColourName()}, which you are now editing.");
	}

	private static void TattooSet(ICharacter actor, StringStack command)
	{
		GenericRevisableSet(actor, command, EditableRevisableItemHelper.TattooHelper);
	}

	private static void TattooEdit(ICharacter actor, StringStack command)
	{
		GenericRevisableEdit(actor, command, EditableRevisableItemHelper.TattooHelper);
	}

	private static void TattooList(ICharacter actor, StringStack command)
	{
		GenericRevisableList(actor, command, EditableRevisableItemHelper.TattooHelper);
	}

	#endregion

	#region Bodyparts

	private const string BodypartCommandHelpText = @"You can use the following options with this command:

	#3bodypart edit <body> <part>#0 - edits a bodypart
	#3bodypart edit#0 - equivalent to bodypart show on your edited bodypart
	#3bodypart clone <newname>#0 - clones your currently edited bodypart
	#3bodypart close#0 - closes your editing bodypart
	#3bodypart show <body> <part>#0 - shows a bodypart
	#3bodypart set <...>#0 - changes something about a bodypart. See command for more info.";

	[PlayerCommand("Bodypart", "bodypart")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("Bodypart", BodypartCommandHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Bodypart(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopSpeech())
		{
			case "edit":
				BodypartEdit(actor, ss);
				return;
			case "close":
				BodypartClose(actor, ss);
				return;
			case "set":
				BodypartSet(actor, ss);
				return;
			case "view":
			case "show":
				BodypartShow(actor, ss);
				return;
			case "clone":
				BodypartClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(BodypartCommandHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void BodypartClone(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IBodypart>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not currently editing any bodyparts.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"What name do you want to give to the new bodypart that you clone from the currently edited one?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (effect.EditingItem.Body.AllBodypartsBonesAndOrgans.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a bodypart with that name. Bodypart names must be unique.");
			return;
		}

		var newPart = effect.EditingItem.Clone(name);
		effect.EditingItem.Body.UpdateBodypartRole(newPart,
			effect.EditingItem.Body.CoreBodyparts.Contains(effect.EditingItem)
				? BodypartRole.Core
				: BodypartRole.Extra);
		actor.OutputHandler.Send(
			$"You clone the bodypart {effect.EditingItem.FullDescription().Colour(Telnet.Yellow)} as {name.Colour(Telnet.Yellow)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IBodypart>>();
		actor.AddEffect(new BuilderEditingEffect<IBodypart>(actor) { EditingItem = newPart });
	}

	private static void BodypartShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which body do you want to view a bodypart for?");
			return;
		}

		var body = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.BodyPrototypes.Get(value)
			: actor.Gameworld.BodyPrototypes.GetByName(ss.Last);
		if (body == null)
		{
			actor.OutputHandler.Send("There is no such body prototype.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which bodypart from the {body.Name.Colour(Telnet.Cyan)} body do you want to view?");
			return;
		}

		var text = ss.PopSpeech();
		var bodypart = body.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Name.EqualTo(text));
		if (bodypart == null)
		{
			actor.OutputHandler.Send($"The {body.Name.Colour(Telnet.Cyan)} body has no such bodypart.");
			return;
		}

		actor.OutputHandler.Send(bodypart.ShowToBuilder(actor));
	}

	private static void BodypartSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IBodypart>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not currently editing any bodyparts.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void BodypartClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IBodypart>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not currently editing any bodyparts.");
			return;
		}

		actor.RemoveEffect(effect);
		actor.OutputHandler.Send("You are no longer editing any bodyparts.");
	}

	private static void BodypartEdit(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IBodypart>>().FirstOrDefault();
		if (ss.IsFinished)
		{
			if (effect != null)
			{
				actor.OutputHandler.Send(effect.EditingItem.ShowToBuilder(actor));
				return;
			}

			actor.OutputHandler.Send("Which body do you want to edit a bodypart from?");
			return;
		}

		var body = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.BodyPrototypes.Get(value)
			: actor.Gameworld.BodyPrototypes.GetByName(ss.Last);
		if (body == null)
		{
			actor.OutputHandler.Send("There is no such body prototype.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which bodypart from the {body.Name.Colour(Telnet.Cyan)} body do you want to edit?");
			return;
		}

		var text = ss.PopSpeech();
		var bodypart = body.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Name.EqualTo(text));
		if (bodypart == null)
		{
			actor.OutputHandler.Send($"The {body.Name.Colour(Telnet.Cyan)} body has no such bodypart.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IBodypart>>();
		actor.AddEffect(new BuilderEditingEffect<IBodypart>(actor) { EditingItem = bodypart });
		actor.OutputHandler.Send(
			$"You open the {bodypart.FullDescription().Colour(Telnet.Yellow)} bodypart from the {body.Name.Colour(Telnet.Cyan)} body for editing.");
	}

	#endregion

	#region BodypartShapes

	public const string BodypartShapesHelp =
		@"The Bodypart Shapes command is used to view, create and manage bodypart shapes. Bodypart shapes are used on bodyparts (and things that work with them, like worn items) to represent the generic concept of things like 'hands' or 'toe'. So the shape represents the general case of the bodypart.

You can use the following syntax with this command:

	#3bodypartshape list#0 - lists all the bodypart shapes
	#3bodypartshape show <which>#0 - shows a bodypart shape
	#3bodypartshape edit <which>#0 - starts editing a bodypart shape
	#3bodypartshape edit#0 - an alias for show when editing a shape
	#3bodypartshape close#0 - stops editing a bodypart shape
	#3bodypartshape new <name>#0 - creates a new bodypart shape
	#3bodypartshape set name <name>#0 - renames a bodypart shape";

	[PlayerCommand("BodypartShape", "bodypartshapes", "bps", "shape")]
	protected static void BodypartShape(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		GenericBuildingCommand(actor, ss, EditableItemHelper.BodypartShapeHelper);
	}

	#endregion

	#region Terrain

	private const string TerrainHelpText = @"This command is used to create, edit and view terrains.

You can use the following options with this command:

	#3terrain list#0 - view a list of all terrain types
	#3terrain new <name>#0 - creates a new terrain
	#3terrain clone <terrain> <name>#0 - creates a new terrain from an existing one
	#3terrain edit <terrain>#0 - begins editing a terrain
	#3terrain edit#0 - alias for show with no argument
	#3terrain show#0 - shows the terrain you are currently editing
	#3terrain show <terrain>#0 - shows a particular terrain
	#3terrain close#0 - closes the terrain you're editing
	#3terrain planner#0 - gets the terrain output for the terrain planner tool

You can also edit the following specific properties:

	#3terrain set name <name>#0 - renames this terrain type
	#3terrain set atmosphere none#0 - sets the terrain to have no atmosphere
	#3terrain set atmosphere gas <gas>#0 - sets the atmosphere to a specified gas
	#3terrain set atmosphere liquid <liquid>#0 - sets the atmosphere to specified liquid
	#3terrain set movement <multiplier>#0 - sets the movement speed multiplier
	#3terrain set stamina <cost>#0 - sets the stamina cost for movement
	#3terrain set hide <difficulty>#0 - sets the hide difficulty
	#3terrain set spot <difficulty>#0 - sets the minimum spot difficulty
	#3terrain set forage none#0 - removes the forage profile from this terrain
	#3terrain set forage <profile>#0 - sets the foragable profile
	#3terrain set weather none#0 - removes a weather controller
	#3terrain set weather <controller>#0 - sets the weather controller
	#3terrain set cover <cover>#0 - toggles a ranged cover
	#3terrain set default#0 - sets this terrain as the default for new rooms
	#3terrain set infection <type> <difficulty> <virulence>#0 - sets the infection for this terrain
	#3terrain set outdoors|indoors|exposed|cave|windows#0 - sets the default behaviour type
	#3terrain set model <model>#0 - sets the layer model. See TERRAIN SET MODEL for a list of valid values.
    #3terrain set mapcolour <0-255>#0 - sets the ANSI colour for the MAP command
    #3terrain set editorcolour <#00000000>#0 - sets the hexadecimal colour for the terrain planner
    #3terrain set editortext <1 or 2 letter code>#0 - sets a code to appear on the terrain planner tile";

	[PlayerCommand("Terrain", "terrain")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("terrain", TerrainHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Terrain(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "new":
				TerrainNew(actor, ss);
				return;
			case "clone":
				TerrainClone(actor, ss);
				return;
			case "edit":
				TerrainEdit(actor, ss);
				return;
			case "close":
				TerrainClose(actor, ss);
				return;
			case "set":
				TerrainSet(actor, ss);
				return;
			case "view":
			case "show":
				TerrainView(actor, ss);
				return;
			case "list":
				ShowModule.Show_Terrain(actor, ss);
				return;
			case "planner":
				TerrainPlanner(actor);
				return;
			default:
				actor.OutputHandler.Send(TerrainHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void TerrainPlanner(ICharacter actor)
	{
		var terrains = JsonSerializer.Serialize(actor.Gameworld.Terrains.Select(x =>
			new
			{
				Id = x.Id, Name = x.Name, TerrainEditorColour = x.TerrainEditorColour,
				TerrainEditorText = x.TerrainEditorText
			}).ToList(), new JsonSerializerOptions
		{
			WriteIndented = true
		});
		actor.OutputHandler.Send($"Terrain file for terrain builder:\n\n{terrains}\n\n", false, true);
	}

	private static void TerrainView(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ITerrain>>().FirstOrDefault();
		if (ss.IsFinished)
		{
			if (effect == null)
			{
				actor.OutputHandler.Send("which terrain type would you like to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var terrain = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Terrains.Get(value)
			: actor.Gameworld.Terrains.GetByName(ss.Last);
		if (terrain == null)
		{
			actor.OutputHandler.Send("There is no such terrain.");
			return;
		}

		actor.OutputHandler.Send(terrain.Show(actor));
	}

	private static void TerrainSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ITerrain>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any terrain types.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void TerrainClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ITerrain>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any terrain types.");
			return;
		}

		actor.RemoveEffect(effect);
		actor.OutputHandler.Send(
			$"You are no longer editing the {effect.EditingItem.Name.Colour(Telnet.Cyan)} terrain type.");
	}

	private static void TerrainEdit(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ITerrain>>().FirstOrDefault();
		if (ss.IsFinished)
		{
			if (effect == null)
			{
				actor.OutputHandler.Send("which terrain type would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var terrain = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Terrains.Get(value)
			: actor.Gameworld.Terrains.GetByName(ss.Last);
		if (terrain == null)
		{
			actor.OutputHandler.Send("There is no such terrain.");
			return;
		}

		effect = new BuilderEditingEffect<ITerrain>(actor) { EditingItem = terrain };
		actor.AddEffect(effect);
		actor.OutputHandler.Send($"You are now editing the {terrain.Name.Colour(Telnet.Cyan)} terrain.");
	}

	private static void TerrainClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which terrain would you like to clone?");
			return;
		}

		var clone = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Terrains.Get(value)
			: actor.Gameworld.Terrains.GetByName(ss.Last);
		if (clone == null)
		{
			actor.OutputHandler.Send("There is no such terrain for you to clone.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new terrain?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (actor.Gameworld.Terrains.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("That name is not unique. The name of the terrain must be unique.");
			return;
		}

		var terrain = new Terrain(clone, name);
		actor.Gameworld.Add(terrain);
		actor.RemoveAllEffects<BuilderEditingEffect<ITerrain>>();
		actor.AddEffect(new BuilderEditingEffect<ITerrain>(actor) { EditingItem = terrain });
		actor.OutputHandler.Send(
			$"You create the new terrain type {name.Colour(Telnet.Cyan)} as a clone of {clone.Name.Colour(Telnet.Cyan)}, which you are now editing.");
	}

	private static void TerrainNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new terrain?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (actor.Gameworld.Terrains.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("That name is not unique. The name of the terrain must be unique.");
			return;
		}

		var terrain = new Terrain(actor.Gameworld, name);
		actor.Gameworld.Add(terrain);
		actor.RemoveAllEffects<BuilderEditingEffect<ITerrain>>();
		actor.AddEffect(new BuilderEditingEffect<ITerrain>(actor) { EditingItem = terrain });
		actor.OutputHandler.Send(
			$"You create the new terrain type {name.Colour(Telnet.Cyan)}, which you are now editing.");
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
	
	#region Chargen Advices

	public const string ChargenAdviceHelp =
		@"Character Creation Advices (or Chargen Advices) are little pieces of information that can be presented to a player who is creating a character offering them advice or instructions on the choices that they are making.

These advices appear on a particular screen (such as race selection, skill selection, background comment, etc.) and can be set to appear if someone has selected a particular race, culture, ethnicity or role.

The syntax for this command is as follows:

	#3chargenadvice list#0 - lists all of the chargen advices
	#3chargenadvice edit <which>#0 - begins editing a chargen advice
	#3chargenadvice edit new <stage> <title>#0 - creates a new chargen advice
	#3chargenadvice clone <old>#0 - clones an existing chargen advice to a new one
	#3chargenadvice close#0 - stops editing a chargen advice
	#3chargenadvice show <which>#0 - views information about a chargen advice
	#3chargenadvice show#0 - views information about your currently editing chargen advice
	#3chargenadvice set ...#0 - edits properties of a chargen advice";

	[PlayerCommand("ChargenAdvice", "chargenadvice")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("chargenadvice", ChargenAdviceHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void ChargenAdvice(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		GenericBuildingCommand(actor, ss, EditableItemHelper.ChargenAdviceHelper);
	}

	#endregion

	#region Skins

	public const string ItemSkinHelp =
		@$"This command is used to make item skins, which can be applied to items to change their appearance. They can be manually added to items by admins, set to load that way through shops and created via crafts. Players with the correct permissions are allowed to create item skins of their own.

When players are editing item skins they will only be able to edit skins that they originally created.

The syntax to use this command is as follows:

	#3itemskin list#0 - lists all of the item skins
	#3itemskin protos#0 - lists all the items that players can create skins for
	#3itemskin edit <which>#0 - begins editing an item skin
	#3itemskin edit new <proto> <name>#0 - creates a new item skin
	#3itemskin clone <old> <new name>#0 - clones an existing item skin to a new one
	#3itemskin close#0 - stops editing an item skin
	#3itemskin show <which>#0 - views information about an item skin
	#3itemskin show#0 - views information about your currently editing item skin
	#3itemskin set ...#0 - edits properties of an item skin
	#3item review all|mine|<builder name>|<id>#0 - opens the specified item skins for review and approval

{GenericReviewableSearchList}";

	[PlayerCommand("ItemSkin", "itemskin", "is")]
	[HelpInfo("itemskin", ItemSkinHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void ItemSkin(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.PeekSpeech().EqualTo("protos"))
		{
			var protos = actor.Gameworld.ItemProtos
			                  .Where(x => x.PermitPlayerSkins && x.Status == RevisionStatus.Current).ToList();
			var sb = new StringBuilder();
			sb.AppendLine($"Skinnable Item Prototypes:\n");
			sb.AppendLine(StringUtilities.GetTextTable(from proto in protos
			                                           select new List<string>
			                                           {
				                                           proto.Id.ToString("N0", actor),
				                                           proto.Name,
				                                           proto.ShortDescription,
				                                           proto.BaseItemQuality.Describe()
			                                           },
				new List<string>
				{
					"Id",
					"Name",
					"Short Description",
					"Quality"
				},
				actor.LineFormatLength,
				colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode));
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		GenericRevisableBuildingCommand(actor, ss, EditableRevisableItemHelper.ItemSkinHelper);
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

}