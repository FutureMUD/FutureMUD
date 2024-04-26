using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Revision;
using MudSharp.Framework;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.PerceptionEngine;
using MudSharp.GameItems.Groups;
using MudSharp.Framework.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace MudSharp.Commands.Modules
{
	internal class ItemBuilderModule : BaseBuilderModule
	{
		private ItemBuilderModule()
		: base("ItemBuilder")
		{
			IsNecessary = true;
		}

		public static ItemBuilderModule Instance { get; } = new();

		#region Item Prototypes

		[PlayerCommand("Item", "item")]
		[CommandPermission(PermissionLevel.JuniorAdmin)]
		[HelpInfo("item",
			@"The item command is used to edit, view and load item prototypes. The editing sub-commands are all done on whichever item prototype the builder currently has open for editing. 

You should also see the related #3component#0 command. Components are what give items their functionality (like being containers, being weapons etc).

The valid sub-commands and their syntaxes are as follows:

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
        #6*<skin name|id>#0 - sets a skin for the item. Must be the first extra argument

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
	#3item set add <id|name>#0 - adds the specified component to this item
	#3item set remove <id|name>#0 - removes the specified component from this item
	#3item set noun <noun>#0 - sets the primary noun for this item. Single word only.
	#3item set sdesc <sdesc>#0 - sets the short description (e.g. a solid iron sword)
	#3item set ldesc <ldesc>#0 - sets an overrided long description (in room) for this item
	#3item set ldesc none#0 - clears an overrided long description
	#3item set desc#0 - drops you into an editor to enter the full description (when looked at)
	#3item set suggestdesc [<optional extra context>]#0 - asks your GPT model to suggest a description
	#3item set size <size>#0 - sets the item size
	#3item set weight <weight>#0 - sets the item weight
	#3item set material <material>#0 - sets the item material
	#3item set quality <quality>#0 - sets the base item quality
	#3item set cost <cost>#0 - sets the base item cost
	#3item set tag <tag>#0 - adds the specified tag to this item
	#3item set untag <tag>#0 - removes the specified tag from this item
	#3item set priority#0 - toggles this item being high priority, which means appearing at the top of the item list in the room
	#3item set colour <ansi colour>#0 - overrides the default green colour for this item
	#3item set colour none#0 - resets the item colour to the default
	#3item set onload <prog>#0 - toggles a particular prog to run when the item is loaded
	#3item set canskin#0 - toggles whether players can make skins for this item
	#3item set register <variable name> <default value>#0 - sets a default value for a register variable for this item
	#3item set register delete <variable name>#0 - deletes a default value for a register variable
	#3item set morph <item##|none> <seconds> [<emote>]#0 - sets item morph information. The 'none' value makes the item disappear.
	#3item set morph clear#0 - clears any morph info for this item
	#3item set group <id|name>#0 - sets this item's item group (for in-room grouping)
	#3item set group none#0 - clears this item's item group
	#3item set destroyed <id>#0 - sets an item to load up in-place of this item when it is destroyed
	#3item set destroyed none#0 - clears a destroyed item setting
	#3item set strategy <id|name>#0 - sets a custom health strategy for this item
	#3item set strategy none#0 - sets the item to use the default item health strategy
    #3item set extra add <prog>#0 - adds a new extra description slot with a specified prog
    #3item set extra remove <which##>#0 - removes the specified numbered extra description
    #3item set extra swap <first##> <second##>#0 - swaps the evaluation order of two extra descriptions
    #3item set extra <which##> sdesc <sdesc>#0 - sets the short description for the extra description
    #3item set extra <which##> clear sdesc#0 - clears the short description for the extra description
    #3item set extra <which##> desc <desc>#0 - sets the full description for the extra description
    #3item set extra <which##> clear desc#0 - clears the full description for the extra description
    #3item set extra <which##> addendum <text>#0 - sets an addendum text for the full description
    #3item set extra <which##> clear addendum#0 - clears the addendum text for the full description",
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
			@"The component command is used to edit and view item components. Item components are shared definitions that can be added to items to give them some kind of functionality, like turning them into a container, making them into a weapon or giving them the ability to provide power.

The editing subcommands are all done on whichever item component the builder currently has open for editing. 

The valid subcommands and their syntaxes are as follows:

	#3comp edit new <type>#0 - creates a new component of the specified type
	#3comp types#0 - lists the available component types
    #3comp types +<keyword>|-<keyword>#0 - lists the available component types with keyword filters
    #3comp typehelp <type>#0 - shows the COMP SET HELP for the requested type
	#3comp edit <id>#0 - opens component with ID for editing
	#3comp edit#0 - shows the currently open component. Equivalent to doing COMP SHOW <ID> on it.
	#3comp edit submit#0 - submits the open component for review
	#3comp edit close#0 - closes the currently open component
	#3comp edit delete#0 - deletes the current component (only if not approved)
	#3comp edit obsolete#0 - marks the current component as obsolete and no longer usable
	#3comp show <ID>#0 - shows info about component with ID
	#3comp review all|mine|<admin name>|<id>#0 - opens the specified components for review and approval
	#3comp set <parameters>#0 - makes a specific edit to a component. See COMP SET HELP for more info
	#3comp list [<filters>]#0 - lists all item components. See below for filters.

		#6all#0 - includes obsolete and non-current revisions
		#6mine#0 - only shows components you personally created
		#6by <account>#0 - only shows components the nominated account created
		#6reviewed <account>#0 - only shows components the nominated account has approved
		#6+<keyword>#0 - only shows components with the nominated keyword
		#6-<keyword>#0 - excludes components with the nominated keyword
		#6<type>#0 - shows only components of the specified type

Note: The following two subcommands take a long time and can sometimes cause issues with performance afterwards because of the aggressive loading of things that they do. You should generally plan to reboot the MUD immediately after running these.

    #3comp update#0 - updates all items with obsolete components or revisions to their current versions
    #3comp update all#0 - same as comp update, but also loads all characters so that their inventories are included",
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
					}, ScheduleType.System,
#if DEBUG
					TimeSpan.FromSeconds(1),
#else
					TimeSpan.FromSeconds(30), 
#endif
					"Component Update"));
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
							var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
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
							var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
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
							var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
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
#3itemskin review all|mine|<builder name>|<id>#0 - opens the specified item skins for review and approval

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
	}
}
