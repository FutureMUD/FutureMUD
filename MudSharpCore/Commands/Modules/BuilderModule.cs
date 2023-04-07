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

internal class BuilderModule : Module<ICharacter>
{
	private BuilderModule()
		: base("Builder")
	{
		IsNecessary = true;
	}

	public static BuilderModule Instance { get; } = new();

	#region Foragables

	private const string ForagableHelpText =
		@"This command is used to view and edit foragables. Foragables are records of items that can be loaded by the foraging system. The items need to be first built using the item system, and you also need to add the foragable record to a foragable profile before it will show up in the world. A single foragable record can be shared between multiple foragable profiles.

You can use the following options with this command:

    foragable list [+key, -key] - shows all the foragables
    foragable show <which> - shows a foragable
    foragable set ... - edits the properties of a foragable
    foragable edit <which> - opens a revision of a foragable
    foragable edit new - creates a new foragable
    foragable edit - an alias for foragable show on an opened foragable
    foragable edit close - closes the currently edited foragable
    foragable edit submit - submits the foragable for review
    foragable edit delete - deletes a non-approved revision
    foragable edit obsolete - marks a foragable as obsolete
    foragable review all|mine|<which> - opens a foragable for review
    foragable review list - shows all the foragables due to review
    foragable review history <which> - shows the history of a foragable";

	[PlayerCommand("Foragable", "foragable")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("foragable", ForagableHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Foragable(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "review":
				GenericReview(actor, ss, EditableRevisableItemHelper.ForagableHelper);
				break;
			case "edit":
				GenericRevisableEdit(actor, ss, EditableRevisableItemHelper.ForagableHelper);
				break;
			case "set":
				GenericRevisableSet(actor, ss, EditableRevisableItemHelper.ForagableHelper);
				break;
			case "list":
				GenericRevisableList(actor, ss, EditableRevisableItemHelper.ForagableHelper);
				break;
			case "show":
				GenericRevisableShow(actor, ss, EditableRevisableItemHelper.ForagableHelper);
				break;
			default:
				actor.OutputHandler.Send(ForagableHelpText);
				break;
		}
	}

	#endregion

	#region Foragable Profiles

	private const string ForagableProfileHelpText =
		@"This command is used to view and edit foragables profiles. Foragable profiles are attached typically to zones or terrain types, and control both what yield types appear in that location and what items can be foraged by players. See also the closely related FORAGABLE command, which you will also need to use.

You can use the following options with this command:

    fp list [+key, -key] - shows all the foragable profiles
    fp show <which> - shows a foragable profile
    fp set ... - edits the properties of a foragable profile
    fp edit <which> - opens a revision of a foragable profile
    fp edit new - creates a new foragable profile
    fp edit - an alias for fp show on an opened foragable profile
    fp edit close - closes the currently edited foragable profile
    fp edit submit - submits the foragable profile for review
    fp edit delete - deletes a non-approved revision
    fp edit obsolete - marks a foragable profile as obsolete
    fp review all|mine|<which> - opens a foragable profile for review
    fp review list - shows all the foragable profile due to review
    fp review history <which> - shows the history of a foragable profile";

	[PlayerCommand("ForagableProfile", "foragableprofile", "fp")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("foragableprofile", ForagableProfileHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void ForagableProfile(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "review":
				GenericReview(actor, ss, EditableRevisableItemHelper.ForagableProfileHelper);
				break;
			case "edit":
				GenericRevisableEdit(actor, ss, EditableRevisableItemHelper.ForagableProfileHelper);
				break;
			case "set":
				GenericRevisableSet(actor, ss, EditableRevisableItemHelper.ForagableProfileHelper);
				break;
			case "list":
				GenericRevisableList(actor, ss, EditableRevisableItemHelper.ForagableProfileHelper);
				break;
			case "show":
				GenericRevisableShow(actor, ss, EditableRevisableItemHelper.ForagableProfileHelper);
				break;
			case "load":
				Item_Load(actor, ss);
				break;
			default:
				actor.OutputHandler.Send(ForagableProfileHelpText);
				break;
		}
	}

	#endregion

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

	private static void Item_Load(ICharacter actor, StringStack input)
	{
		IGameItemProto proto;
		int quantity;
		if (!long.TryParse(input.Pop(), out var value))
		{
			actor.OutputHandler.Send("What is the ID of the item you wish to load?");
			return;
		}

		if (!input.IsFinished && long.TryParse(input.Peek(), out var value2))
		{
			if (value <= 0)
			{
				actor.OutputHandler.Send("The quantity of items to load must be greater than 0.");
				return;
			}

			proto = actor.Gameworld.ItemProtos.Get(value2);
			quantity = (int)value;
			input.Pop();
		}
		else
		{
			proto = actor.Gameworld.ItemProtos.Get(value);
			quantity = 1;
		}

		if (proto == null)
		{
			actor.OutputHandler.Send("There is no such prototype to load.");
			return;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send(proto.EditHeader().ColourName() + " is not approved for use.");
			return;
		}

		if (proto.Components.Any(x => x.PreventManualLoad))
		{
			actor.OutputHandler.Send(
				$"{proto.EditHeader().ColourName()} contains a component that should not be manually loaded, such as currency, corpses or bodyparts.");
			return;
		}

		var varProto = proto.GetItemType<VariableGameItemComponentProto>();
		var prePopulatedVariables =
			new Dictionary<ICharacteristicDefinition, ICharacteristicValue>();
		IGameItemSkin skin = null;
		if (!input.IsFinished && input.PeekSpeech().StartsWith("*"))
		{
			var skinText = input.PopSpeech()[1..];

			if (long.TryParse(skinText, out var skinid))
			{
				skin = actor.Gameworld.ItemSkins.Get(skinid);
			}
			else
			{
				skin = actor.Gameworld.ItemSkins.Get(skinText).FirstOrDefault(x => x.ItemProto.Id == proto.Id);
			}

			if (skin is null)
			{
				actor.OutputHandler.Send($"There is no item skin like that for {proto.EditHeader().ColourName()}.");
				return;
			}

			if (skin.ItemProto.Id != proto.Id)
			{
				actor.OutputHandler.Send(
					$"{skin.EditHeader().ColourName()} is not designed for {proto.EditHeader().ColourName()}.");
				return;
			}

			if (skin.Status != RevisionStatus.Current)
			{
				actor.OutputHandler.Send($"{skin.EditHeader().ColourName()} is not approved for use.");
				return;
			}
		}

		if (!input.IsFinished)
		{
			if (varProto == null)
			{
				actor.OutputHandler.Send(
					"That is not a variable item, and so you cannot populate it with variables.");
				return;
			}

			prePopulatedVariables = varProto.GetValuesFromString(input.SafeRemainingArgument);
		}

		if (quantity > 1)
		{
			if (proto.IsItemType<StackableGameItemComponentProto>())
			{
				var item = proto.CreateNew(actor);
				if (skin is not null)
				{
					item.Skin = skin;
				}

				actor.Gameworld.Add(item);
				item.GetItemType<IStackable>().Quantity = quantity;
				var vitem = item.GetItemType<IVariable>();
				if (vitem != null)
				{
					Item_Load_PopulateCharacteristics(vitem, prePopulatedVariables);
				}

				if (actor.Body.CanGet(item, 0))
				{
					actor.Body.Get(item, silent: true);
				}
				else
				{
					item.RoomLayer = actor.RoomLayer;
					actor.Location.Insert(item);
				}

				item.HandleEvent(EventType.ItemFinishedLoading, item);
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ load|loads $0.", actor, item),
						flags: OutputFlags.SuppressObscured));
			}
			else
			{
				IGameItem item = null;
				for (var i = 0; i < quantity; i++)
				{
					item = proto.CreateNew(actor);
					if (skin is not null)
					{
						item.Skin = skin;
					}

					actor.Gameworld.Add(item);
					var vitem = item.GetItemType<IVariable>();
					if (vitem != null)
					{
						Item_Load_PopulateCharacteristics(vitem, prePopulatedVariables);
					}

					if (actor.Body.CanGet(item, 0))
					{
						actor.Body.Get(item, silent: true);
					}
					else
					{
						item.RoomLayer = actor.RoomLayer;
						actor.Location.Insert(item);
					}

					item.HandleEvent(EventType.ItemFinishedLoading, item);
				}

				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ load|loads $0 " + quantity + " times.", actor, item),
						flags: OutputFlags.SuppressObscured));
			}
		}
		else
		{
			var item = proto.CreateNew(actor);
			if (skin is not null)
			{
				item.Skin = skin;
			}

			actor.Gameworld.Add(item);
			var vitem = item.GetItemType<IVariable>();
			if (vitem != null)
			{
				Item_Load_PopulateCharacteristics(vitem, prePopulatedVariables);
			}

			if (actor.Body.CanGet(item, 0))
			{
				actor.Body.Get(item, silent: true);
			}
			else
			{
				item.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(item);
			}

			item.HandleEvent(EventType.ItemFinishedLoading, item);
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ load|loads $0.", actor, item),
					flags: OutputFlags.SuppressObscured));
		}
	}

	private static void Item_Load_PopulateCharacteristics(IVariable variable,
		Dictionary<ICharacteristicDefinition, ICharacteristicValue
		> prePopulatedVariables)
	{
		foreach (var characteristic in variable.CharacteristicDefinitions)
		{
			if (prePopulatedVariables.ContainsKey(characteristic))
			{
				variable.SetCharacteristic(characteristic, prePopulatedVariables[characteristic]);
			}
			else
			{
				variable.SetRandom(characteristic);
			}
		}
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

	#region Generic Revisable

	public static void GenericRevisableEdit(ICharacter character, StringStack input, EditableRevisableItemHelper helper)
	{
		var cmd = input.PopSpeech().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			if (helper.GetEditableItemFunc(character) != null)
			{
				var sb = new StringBuilder();
				sb.AppendLine("You are currently editing " + helper.GetEditableItemFunc(character).EditHeader());
				sb.AppendLine();
				sb.Append(helper.GetEditableItemFunc(character).Show(character));
				character.OutputHandler.Send(sb.ToString());
				return;
			}

			character.OutputHandler.Send("What do you wish to edit?");
			return;
		}

		switch (cmd)
		{
			case "close":
				GenericRevisableEditClose(character, input, helper);
				break;
			case "delete":
				GenericRevisableEditDelete(character, input, helper);
				break;
			case "submit":
				GenericRevisableEditSubmit(character, input, helper);
				break;
			case "obsolete":
				GenericRevisableEditObsolete(character, input, helper);
				break;
			case "new":
				GenericRevisableEditNew(character, input, helper);
				break;
			default:
				GenericRevisableEditDefault(character, input, helper);
				break;
		}
	}

	public static void GenericRevisableEditClose(ICharacter character, StringStack input,
		EditableRevisableItemHelper helper)
	{
		if (helper.GetEditableItemFunc(character) == null)
		{
			character.Send("You are not currently editing any {0}.", helper.ItemNamePlural);
			return;
		}

		helper.SetEditableItemAction(character, null);
		character.Send("You close your current edited {0}.", helper.ItemName);
	}

	public static void GenericRevisableEditDelete(ICharacter character, StringStack input,
		EditableRevisableItemHelper helper)
	{
		if (helper.GetEditableItemFunc(character) == null)
		{
			character.Send("You are not currently editing any {0}.", helper.ItemNamePlural);
			return;
		}

		if (helper.GetEditableItemFunc(character).Status != RevisionStatus.UnderDesign)
		{
			character.Send("That {0} is not currently under design.", helper.ItemName);
			return;
		}

		character.OutputHandler.Send("You delete " + helper.GetEditableItemFunc(character).EditHeader() + ".");
		character.Gameworld.SaveManager.Flush();
		helper.DeleteEditableItemAction(helper.GetEditableItemFunc(character));
		helper.SetEditableItemAction(character, null);
	}

	public static void GenericRevisableEditSubmit(ICharacter character, StringStack input,
		EditableRevisableItemHelper helper)
	{
		if (helper.GetEditableItemFunc(character) == null)
		{
			character.Send("You are not currently editing any {0}.", helper.ItemNamePlural);
			return;
		}

		if (helper.GetEditableItemFunc(character).Status != RevisionStatus.UnderDesign)
		{
			character.Send("That {0} is not currently under design.", helper.ItemName);
			return;
		}

		if (!helper.GetEditableItemFunc(character).CanSubmit())
		{
			character.Send(helper.GetEditableItemFunc(character).WhyCannotSubmit());
			return;
		}

		var comment = input.IsFinished ? "" : input.SafeRemainingArgument;

		helper.GetEditableItemFunc(character)
		      .ChangeStatus(RevisionStatus.PendingRevision, comment, character.Account);
		character.OutputHandler.Send("You submit " + helper.GetEditableItemFunc(character).EditHeader() +
		                             " for review" + (comment.Length > 0 ? ", with the comment: " + comment : "."));
		helper.SetEditableItemAction(character, null);
	}

	public static void GenericRevisableEditObsolete(ICharacter character, StringStack input,
		EditableRevisableItemHelper helper)
	{
		if (helper.GetEditableItemFunc(character) == null)
		{
			character.Send("You are not currently editing any {0}.", helper.ItemNamePlural);
			return;
		}

		if (helper.GetEditableItemFunc(character).Status != RevisionStatus.Current)
		{
			character.Send("You are not editing the most current revision of this {0}.", helper.ItemName);
			return;
		}

		helper.GetEditableItemFunc(character)
		      .ChangeStatus(RevisionStatus.Obsolete, input.SafeRemainingArgument, character.Account);
		character.Send("You mark {0} as an obsolete {1}.", helper.GetEditableItemFunc(character).EditHeader(),
			helper.ItemName);
		helper.SetEditableItemAction(character, null);
	}

	public static void GenericRevisableEditNew(ICharacter character, StringStack input,
		EditableRevisableItemHelper helper)
	{
		helper.EditableNewAction(character, input);
	}

	public static void GenericRevisableEditDefault(ICharacter character, StringStack input,
		EditableRevisableItemHelper helper)
	{
		var cmd = input.Last;
		if (!long.TryParse(cmd, out var id))
		{
			character.Send("You must either enter an ID of {0} to edit, or use the {1} keyword.",
				helper.ItemName.A_An(), "new".Colour(Telnet.Cyan));
			return;
		}

		IEditableRevisableItem proto = null;
		cmd = input.PopSpeech();
		if (cmd.Length == 0)
		{
			var protos = helper.GetAllEditableItemsByIdFunc(character, id).ToList();
			//proto = protos.FirstOrDefault(x => x.Status == RevisionStatus.Current) ?? protos.FirstOrDefault(x => (x.Status == RevisionStatus.UnderDesign) || (x.Status == RevisionStatus.PendingRevision)) ?? protos.LastOrDefault(x => x.Status == RevisionStatus.Rejected);
			proto =
				protos.FirstOrDefault(x => x.Status == RevisionStatus.UnderDesign ||
				                           x.Status == RevisionStatus.PendingRevision) ??
				protos.LastOrDefault(x => x.Status == RevisionStatus.Rejected) ??
				protos.LastOrDefault(x => x.Status == RevisionStatus.Current);
		}
		else
		{
			if (!int.TryParse(cmd, out var revision))
			{
				character.OutputHandler.Send(
					"You must either enter just an ID to open the most recent revision, or specify a numerical revision number.");
				return;
			}

			proto = helper.GetEditableItemByIdRevNumFunc(character, id, revision);

			if (proto == null)
			{
				character.Send("There is no such {0} for you to edit.", helper.ItemName);
				return;
			}

			if (!helper.CanEditItemFunc(character, proto))
			{
				character.OutputHandler.Send($"You are not permitted to edit that {helper.ItemName}.");
				return;
			}

			if (proto.Status != RevisionStatus.UnderDesign && proto.Status != RevisionStatus.PendingRevision &&
			    proto.Status != RevisionStatus.Rejected)
			{
				character.Send("You cannot open that {0} for editing, you must open a fresh one.", helper.ItemName);
				return;
			}
		}

		if (proto == null)
		{
			character.Send("There is no such {0} for you to edit.", helper.ItemName);
			return;
		}

		if (proto.ReadOnly)
		{
			character.Send(
				$"You cannot create a new revision or edit the properties of that {helper.ItemName} because it is read-only.");
			return;
		}

		if (proto.Status == RevisionStatus.Rejected)
			//Re-open the most recent rejected revision for editing. 
		{
			proto.ChangeStatus(RevisionStatus.UnderDesign, "Returning to design status.", character.Account);
		}

		if (proto.Status == RevisionStatus.UnderDesign || proto.Status == RevisionStatus.PendingRevision)
		{
			helper.SetEditableItemAction(character, proto);
		}
		else
		{
			helper.SetEditableItemAction(character, proto.CreateNewRevision(character));
			helper.AddItemToGameWorldAction(helper.GetEditableItemFunc(character));
		}

		character.OutputHandler.Send("You open " + helper.GetEditableItemFunc(character).EditHeader() +
		                             " for editing.");
	}

	public static void GenericReview(ICharacter character, StringStack input, EditableRevisableItemHelper helper)
	{
		if (!helper.CanReviewFunc(character))
		{
			character.OutputHandler.Send($"You are not permitted to review {helper.ItemNamePlural}.");
			return;
		}

		var cmd = input.Pop().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			GenericReviewList(character, input, helper);
			return;
		}

		switch (cmd)
		{
			case "list":
				GenericReviewList(character, input, helper);
				break;
			case "history":
				GenericReviewHistory(character, input, helper);
				break;
			case "all":
				GenericReviewAll(character, input, helper);
				break;
			default:
				GenericReviewDefault(character, input, helper);
				break;
		}
	}

	public static void GenericReviewList(ICharacter character, StringStack input, EditableRevisableItemHelper helper)
	{
		var protos = helper.GetAllEditableItems(character).Where(x => x.Status == RevisionStatus.PendingRevision);

		while (!input.IsFinished)
		{
			var cmd = input.Pop().ToLowerInvariant();
			switch (cmd)
			{
				case "by":
					cmd = input.Pop().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						character.Send("List {0} for Review by whom?", helper.ItemNamePlural);
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name.ToLowerInvariant() == cmd);
						if (dbaccount == null)
						{
							character.OutputHandler.Send("There is no such account.");
							return;
						}

						protos = protos.Where(x => x.BuilderAccountID == dbaccount.Id);
						break;
					}

				case "mine":
					protos = protos.Where(x => x.BuilderAccountID == character.Account.Id);
					break;
				default:
					character.Send("That is not a valid option for Listing {0} for Review.", helper.ItemNamePlural);
					return;
			}
		}

		// Display Output for List
		using (new FMDB())
		{
			character.OutputHandler.Send(
				StringUtilities.GetTextTable(
					helper.GetReviewTableContentsFunc(character, protos),
					helper.GetReviewTableHeaderFunc(character),
					character.Account.LineFormatLength, colour: Telnet.Green,
					unicodeTable: character.Account.UseUnicode
				)
			);
		}
	}

	public static void GenericReviewAll(ICharacter character, StringStack input, EditableRevisableItemHelper helper)
	{
		var protos = helper.GetAllEditableItems(character).Where(x => x.Status == RevisionStatus.PendingRevision);
		while (!input.IsFinished)
		{
			var cmd = input.Pop().ToLowerInvariant();
			switch (cmd)
			{
				case "by":
					cmd = input.Pop().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						character.Send("Review {0} by whom?", helper.ItemNamePlural);
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name.ToLowerInvariant() == cmd);
						if (dbaccount == null)
						{
							character.OutputHandler.Send("There is no such account.");
							return;
						}

						protos = protos.Where(x => x.BuilderAccountID == dbaccount.Id);
						break;
					}

				case "mine":
					protos = protos.Where(x => x.BuilderAccountID == character.Account.Id);
					break;
				default:
					character.Send("That is not a valid option for Reviewing {0}.", helper.ItemNamePlural);
					return;
			}
		}

		var editableItems = protos as IEditableRevisableItem[] ?? protos.ToArray();
		if (!editableItems.Any())
		{
			character.Send("There are no {0} to review.", helper.ItemNamePlural);
			return;
		}

		var count = editableItems.Length;
		using (new FMDB())
		{
			var accounts = editableItems.Select(x => x.BuilderAccountID).Distinct()
			                            .Select(x => (Id: x, Account: FMDB.Context.Accounts.Find(x)))
			                            .ToDictionary(x => x.Id, x => x.Account);
			character.OutputHandler.Send(
				$@"You are reviewing {count} {(count == 1 ? helper.ItemName : helper.ItemNamePlural)}.

{editableItems.Select(x => $"\t{x.EditHeader().ColourName()} - Edited by {accounts[x.BuilderAccountID].Name.ColourName()} {((PermissionLevel)accounts[x.BuilderAccountID].AuthorityGroup.AuthorityLevel).DescribeEnum(true).Parentheses().ColourValue()}").ListToCommaSeparatedValues("\n")}

To approve {(count == 1 ? "this" : "these")} {(count == 1 ? helper.ItemName : helper.ItemNamePlural)}, type {"accept edit".Colour(Telnet.Yellow)} or {"decline edit".Colour(Telnet.Yellow)} to reject.
If you do not wish to approve or decline, you may type {"abort edit".Colour(Telnet.Yellow)}.");
		}

		character.AddEffect(helper.GetReviewProposalEffectFunc(editableItems.ToList(), character),
			new TimeSpan(0, 0, 120));
	}

	public static void GenericReviewHistory(ICharacter character, StringStack input, EditableRevisableItemHelper helper)
	{
		var cmd = input.PopSpeech();
		if (!long.TryParse(cmd, out var value))
		{
			character.Send("Which {0} do you want to view the revision history of?", helper.ItemName);
			return;
		}

		var protos = helper.GetAllEditableItemsByIdFunc(character, value);
		if (!protos.Any())
		{
			character.Send("There is no such {0}.", helper.ItemName);
			return;
		}

		// Display Output for List
		using (new FMDB())
		{
			character.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from proto in protos.OrderBy(x => x.RevisionNumber)
					select new[]
					{
						proto.Id.ToString(), proto.RevisionNumber.ToString(),
						FMDB.Context.Accounts.Find(proto.BuilderAccountID).Name, proto.BuilderComment,
						proto.BuilderDate.GetLocalDateString(character),
						FMDB.Context.Accounts.Any(x => x.Id == proto.ReviewerAccountID)
							? FMDB.Context.Accounts.Find(proto.ReviewerAccountID).Name
							: "",
						proto.ReviewerComment,
						proto.ReviewerDate.HasValue
							? proto.ReviewerDate.Value.GetLocalDateString(character)
							: "",
						proto.Status.Describe()
					},
					new[]
					{
						"ID#", "Rev#", "Builder", "Comment", "Build Date", "Reviewer", "Comment",
						"Review Date",
						"Status"
					}, character.Account.LineFormatLength, colour: Telnet.Green,
					unicodeTable: character.Account.UseUnicode
				)
			);
		}
	}

	public static void GenericReviewDefault(ICharacter character, StringStack input, EditableRevisableItemHelper helper)
	{
		var proto = long.TryParse(input.Last, out var value)
			? helper.GetAllEditableItemsByIdFunc(character, value)
			        .FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision)
			: helper.GetAllEditableItems(character).FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision);

		if (proto == null)
		{
			character.Send("There is no such {0} that requires review.", helper.ItemName);
			return;
		}

		character.OutputHandler.Send(
			$"You are reviewing {proto.EditHeader().ColourName()}.\n\n{proto.Show(character)}\n\nTo approve this {helper.ItemName}, type {"accept edit".Colour(Telnet.Yellow)} or {"decline edit".Colour(Telnet.Yellow)} to reject.\nIf you do not wish to approve or decline, you may type {"abort edit".Colour(Telnet.Yellow)}.");
		character.AddEffect(helper.GetReviewProposalEffectFunc(new List<IEditableRevisableItem> { proto }, character),
			TimeSpan.FromSeconds(120));
	}

	public static void GenericRevisableSet(ICharacter character, StringStack input, EditableRevisableItemHelper helper)
	{
		if (helper.GetEditableItemFunc(character) == null)
		{
			character.Send("You are not currently editing any {0}.", helper.ItemNamePlural);
			return;
		}

		helper.GetEditableItemFunc(character).BuildingCommand(character, input);
	}

	public static void GenericRevisableList(ICharacter character, StringStack input, EditableRevisableItemHelper helper)
	{
		var protos = helper.GetAllEditableItems(character).ToList();

		var useCurrent = true;
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
					useCurrent = false;
					break;
				case "by":
					cmd = input.PopSpeech().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						character.Send("List {0} by whom?", helper.ItemNamePlural);
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
						if (dbaccount == null)
						{
							character.OutputHandler.Send("There is no such account.");
							return;
						}

						protos = protos.Where(x => x.BuilderAccountID == dbaccount.Id).ToList();
						break;
					}

				case "mine":
					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.Find(character.Account.Id);
						if (dbaccount == null)
						{
							character.OutputHandler.Send("There is no such account.");
							return;
						}

						protos = protos.Where(x => x.BuilderAccountID == dbaccount.Id).ToList();
						break;
					}

				case "reviewed":
					cmd = input.Pop().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						character.Send("List {0} reviewed by whom?", helper.ItemNamePlural);
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
						if (dbaccount == null)
						{
							character.OutputHandler.Send("There is no such account.");
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
							character.OutputHandler.Send("Include which keyword?");
							return;
						}

						protos = protos.Where(x =>
							x.HasKeyword(subcmd, character.Body, true) ||
							x.Name.Contains(subcmd, StringComparison.InvariantCultureIgnoreCase)).ToList();
						break;
					}

					if (cmd[0] == '-')
					{
						var subcmd = cmd.RemoveFirstCharacter();
						if (subcmd.Length == 0)
						{
							character.OutputHandler.Send("Exclude which keyword?");
							return;
						}

						protos = protos.Where(x =>
							!x.HasKeyword(subcmd, character.Body, true) &&
							!x.Name.Contains(subcmd, StringComparison.InvariantCultureIgnoreCase)).ToList();
						break;
					}

					protos = helper.CustomSearch(protos, cmd, character.Gameworld).ToList();
					break;
			}
		}

		if (useCurrent)
		{
			protos = protos.Where(x =>
				x.Status == RevisionStatus.Current || x.Status == RevisionStatus.UnderDesign ||
				x.Status == RevisionStatus.PendingRevision).ToList();
		}

		// Sort List
		protos = protos.OrderBy(x => x.Id).ThenBy(x => x.RevisionNumber).ToList();

		// Display Output for List
		character.OutputHandler.Send(
			StringUtilities.GetTextTable(
				helper.GetListTableContentsFunc(character, protos),
				helper.GetListTableHeaderFunc(character), character.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: character.Account.UseUnicode
			)
		);
	}

	public static void GenericRevisableShow(ICharacter character, StringStack input, EditableRevisableItemHelper helper)
	{
		var cmd = input.PopSpeech();
		if (!long.TryParse(cmd, out var vnum))
		{
			character.OutputHandler.Send("That is not a valid id number.");
			return;
		}

		IEditableRevisableItem proto;
		cmd = input.Pop();
		if (cmd.Length > 0)
		{
			if (!int.TryParse(cmd, out var revision))
			{
				character.OutputHandler.Send("That is not a valid revision.");
				return;
			}

			proto = helper.GetEditableItemByIdRevNumFunc(character, vnum, revision);
		}
		else
		{
			proto = helper.GetEditableItemByIdFunc(character, vnum);
		}

		if (proto == null)
		{
			character.Send("That is not a valid {0}.", helper.ItemName);
			return;
		}

		if (!helper.CanViewItemFunc(character, proto))
		{
			character.OutputHandler.Send($"You are not permitted to view that {helper.ItemName}.");
			return;
		}

		character.OutputHandler.Send(proto.Show(character));
	}

	private static void GenericRevisableClone(ICharacter actor, StringStack input, EditableRevisableItemHelper helper)
	{
		throw new NotImplementedException();
	}

	public static void GenericRevisableBuildingCommand(ICharacter actor, StringStack input,
		EditableRevisableItemHelper helper)
	{
		switch (input.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				GenericRevisableEdit(actor, input, helper);
				return;
			case "close":
				GenericRevisableEditClose(actor, input, helper);
				return;
			case "set":
				GenericRevisableSet(actor, input, helper);
				return;
			case "show":
			case "view":
				GenericRevisableShow(actor, input, helper);
				return;
			case "review":
				GenericReview(actor, input, helper);
				return;
			case "clone":
				GenericRevisableClone(actor, input, helper);
				return;
			case "list":
				GenericRevisableList(actor, input, helper);
				return;
			default:
				actor.OutputHandler.Send(helper.DefaultCommandHelp.SubstituteANSIColour());
				return;
		}
	}

	#endregion

	#region Generic Non-Revisable

	public static void GenericEdit(ICharacter character, StringStack input, EditableItemHelper helper)
	{
		var cmd = input.PopSpeech().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			if (helper.GetEditableItemFunc(character) != null)
			{
				var sb = new StringBuilder();
				sb.AppendLine(
					"You are currently editing " + helper.GetEditHeader(helper.GetEditableItemFunc(character)));
				sb.AppendLine();
				sb.Append(helper.GetEditableItemFunc(character).Show(character));
				character.OutputHandler.Send(sb.ToString());
				return;
			}

			character.OutputHandler.Send($"Which {helper.ItemName} do you wish to edit?");
			return;
		}

		switch (cmd)
		{
			case "close":
				GenericEditClose(character, input, helper);
				break;
			case "new":
				GenericEditNew(character, input, helper);
				break;
			default:
				GenericEditDefault(character, input, helper);
				break;
		}
	}

	public static void GenericEditClose(ICharacter character, StringStack input, EditableItemHelper helper)
	{
		if (helper.GetEditableItemFunc(character) == null)
		{
			character.OutputHandler.Send($"You are not currently editing any {helper.ItemNamePlural}.");
			return;
		}

		helper.SetEditableItemAction(character, null);
		character.Send($"You are no longer editing any {helper.ItemNamePlural}.");
	}

	public static void GenericEditNew(ICharacter character, StringStack input, EditableItemHelper helper)
	{
		helper.EditableNewAction(character, input);
	}

	public static void GenericEditDefault(ICharacter character, StringStack input, EditableItemHelper helper)
	{
		var cmd = input.Last;
		if (!long.TryParse(cmd, out var id))
		{
			character.OutputHandler.Send(
				$"You must either enter an ID of {helper.ItemName.A_An()} to edit, or use the {"new".Colour(Telnet.Cyan)} keyword.");
			return;
		}

		var proto = helper.GetEditableItemByIdFunc(character, id);

		if (proto == null)
		{
			character.Send("There is no such {0} for you to edit.", helper.ItemName);
			return;
		}

		helper.SetEditableItemAction(character, proto);
		character.OutputHandler.Send(
			$"You open {helper.GetEditHeader(helper.GetEditableItemFunc(character))} for editing.");
	}

	public static void GenericSet(ICharacter character, StringStack input, EditableItemHelper helper)
	{
		if (helper.GetEditableItemFunc(character) == null)
		{
			character.OutputHandler.Send($"You are not currently editing any {helper.ItemNamePlural}.");
			return;
		}

		helper.GetEditableItemFunc(character).BuildingCommand(character, input);
	}

	public static void GenericList(ICharacter character, StringStack input, EditableItemHelper helper)
	{
		var protos = helper.GetAllEditableItems(character).ToList();

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
				default:
					if (cmd[0] == '+')
					{
						var subcmd = cmd.RemoveFirstCharacter();
						if (subcmd.Length == 0)
						{
							character.OutputHandler.Send("Include which keyword?");
							return;
						}

						protos = protos.Where(x => x.Name.Contains(subcmd, StringComparison.InvariantCultureIgnoreCase))
						               .ToList();
						break;
					}

					if (cmd[0] == '-')
					{
						var subcmd = cmd.RemoveFirstCharacter();
						if (subcmd.Length == 0)
						{
							character.OutputHandler.Send("Exclude which keyword?");
							return;
						}

						protos = protos
						         .Where(x => !x.Name.Contains(subcmd, StringComparison.InvariantCultureIgnoreCase))
						         .ToList();
						break;
					}

					protos = helper.CustomSearch(protos, cmd, character.Gameworld).ToList();
					break;
			}
		}

		// Sort List
		protos = protos.OrderBy(x => x.Id).ToList();

		// Display Output for List
		character.OutputHandler.Send(
			StringUtilities.GetTextTable(
				helper.GetListTableContentsFunc(character, protos),
				helper.GetListTableHeaderFunc(character), character.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: character.Account.UseUnicode
			)
		);
	}

	public static void GenericShow(ICharacter character, StringStack input, EditableItemHelper helper)
	{
		var cmd = input.Pop();
		if (!long.TryParse(cmd, out var vnum))
		{
			character.OutputHandler.Send("That is not a valid id number.");
			return;
		}

		var proto = helper.GetEditableItemByIdFunc(character, vnum);
		;

		if (proto == null)
		{
			character.OutputHandler.Send($"That is not a valid {helper.ItemName}.");
			return;
		}

		character.OutputHandler.Send(proto.Show(character));
	}

	public static void GenericClone(ICharacter character, StringStack input, EditableItemHelper helper)
	{
		helper.EditableCloneAction(character, input);
	}

	public static void GenericBuildingCommand(ICharacter actor, StringStack input, EditableItemHelper helper)
	{
		switch (input.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				GenericEdit(actor, input, helper);
				return;
			case "close":
				GenericEditClose(actor, input, helper);
				return;
			case "set":
				GenericSet(actor, input, helper);
				return;
			case "show":
			case "view":
				GenericShow(actor, input, helper);
				return;
			case "clone":
				GenericClone(actor, input, helper);
				return;
			case "list":
				GenericList(actor, input, helper);
				return;
			default:
				actor.OutputHandler.Send(helper.DefaultCommandHelp.SubstituteANSIColour());
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

	#region Weapon Attacks

	public const string WeaponAttackHelp =
		@"This command is used to create and edit weapon attacks, as well as natural weapon attacks (e.g. unarmed attacks). Conceptually, a weapon attack either belongs to a weapon type (which makes it a melee weapon attack) or it does not (which makes it a natural/unarmed attack). 

Weapon attacks have types that determine how they work mechanically. You cannot change the type of a weapon attack once it has been made. In this case you may need to make a separate attack.

The syntax for this command is as follows:

	#3weaponattack list#0 - lists all weapon attacks
	#3weaponattack edit <id|name>#0 - opens a weapon attack for editing
	#3weaponattack edit#0 - an alias for WEAPONATTACK SHOW when you have something open
	#3weaponattack show <id|name>#0 - shows details about a particular weapon attack
	#3weaponattack close#0 - closes the open weapon attack
	#3weaponattack natural <race> <attack> <bodypart>#0 - toggles an unarmed attack being enabled for a race
	#3weaponattack new <type>#0 - creates a new weapon attack of the specified type
	#3weaponattack clone <which>#0 - creates a carbon copy of the specified weapon type
	#3weaponattack set ...#0 - edits the properties of a weapon attack. See that command for more help.";

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
		}

		actor.OutputHandler.Send(
			"The valid choices are list, show, new, edit, close, set, clone, natural and delete.");
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
				attacks = attacks.Where(x => x.Name.Contains(arg));
				continue;
			}

			if (arg[0] == '-' && arg.Length > 1)
			{
				arg = arg.Substring(1);
				attacks = attacks.Where(x => !x.Name.Contains(arg));
				continue;
			}

			if (arg[0] == '*' && arg.Length > 1)
			{
				if (!arg.Substring(1).TryParseEnum<BuiltInCombatMoveType>(out var value))
				{
					actor.OutputHandler.Send(
						$"There is no such built-in combat move type as {arg.Substring(1).ColourCommand()}.");
					return;
				}

				attacks = attacks.Where(x => x.MoveType == value);
				continue;
			}

			if (arg.EqualTo("unarmed"))
			{
				attacks = attacks.Where(x => !actor.Gameworld.WeaponTypes.All(y => !y.Attacks.Contains(x)));
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
				attack.MoveType.Describe()
			},
			new[] { "ID", "Name", "Weapon Type", "Move Type" },
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

		if (!Enum.TryParse<ItemQuality>(ss.PopSpeech(), out var quality))
		{
			actor.OutputHandler.Send("That is not a valid item quality.");
			return;
		}

		foreach (var similar in race.NaturalWeaponAttacks
		                            .Where(x => x.Attack == attack && x.Bodypart == targetbodypart).ToList())
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

	#region Combat Messages

	[PlayerCommand("CombatMessage", "combatmessage", "cm")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("combatmessage",
		"This command can be used to edit a combat message. The valid subcommands are list, show, new, edit, close, set, clone and delete.",
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
				"You can use the following options to help refine your search:\n\t<verb> - show all combat messages for the specified attack verb\n\t+<key> - include messages with the specified text\n\t-<key> - exclude messages with the specified text\n\t*<attack> - include combat messages only that apply to specified weapon attack\n\t&<type> - filters messages for a particular type");
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
				message.Prog != null
					? $"{message.Prog.FunctionName} (#{message.Prog.Id})".FluentTagMXP(
						"send", $"href='show futureprog {message.Prog.Id}'")
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

		actor.OutputHandler.Send("TODO");
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
			switch (command.PopSpeech().ToLowerInvariant())
			{
				default:
					if (!command.PopSpeech().TryParseEnum<MaterialBehaviourType>(out var materialType))
					{
						actor.Send("There is no such material general type to filter by.");
						return;
					}

					materials = materials.Where(x => x.BehaviourType == materialType);
					continue;
			}
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
		@"The tattoo command is used to view, create and inscribe tattoos. Players are able to create and submit their own tattoos but only admins can approve them.

The following options are used to view, edit and create tattoo designs:

	#3tattoo list [all|mine|+key|-key]#0 - lists all tattoos
	#3tattoo edit <id|name>#0 - opens the specified tattoo for editing
	#3tattoo edit new <name>#0 - creates a new tattoo for editing
	#3tattoo edit#0 - equivalent of doing SHOW on your currently editing tattoo
	#3tattoo clone <id|name> <new name>#0 - creates a carbon copy of a tattoo for editing
	#3tattoo show <id|name>#0 - shows a particular tattoo.
	#3tattoo set <subcommand>#0 - changes something about the tattoo. See its help for more info.
	#3tattoo edit submit#0 - submits a tattoo for review

The following commands are used to put tattoos on people:

	tattoo inscribe <target> <tattoo id|name> <bodypart> - begins inscribing a tattoo on someone
	tattoo continue <target> <tattoo keyword> - continues inscribing an unfinished tattoo on someone";

	protected const string TattooAdminHelp =
		@"The tattoo command is used to view, create and inscribe tattoos. Players are able to create and submit their own tattoos but only admins can approve them.

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

	list - view a list of all terrain types
	new <name> - creates a new terrain
	clone <terrain> <name> - creates a new terrain from an existing one
	edit <terrain> - begins editing a terrain
	edit - alias for show with no argument
	show - shows the terrain you are currently editing
	show <terrain> - shows a particular terrain
	close - closes the terrain you're editing
	set <...> - sets a property of the terrain
	planner - gets the terrain output for the terrain planner tool";

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
				actor.OutputHandler.Send(TerrainHelpText);
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

	#region Languages

	private const string LanguageCommandHelp =
		"This command allows you to view detailed information about a language that you speak. The syntax is LANGUAGE <name>, and can be used on any language that you know.";

	private const string AdminLanguageCommandHelp =
		@"This command allows you to create and edit languages. You can use the following options with this command:

    language show <which> - shows you a language
    language edit new <name> <trait> - creates a new language with the specified name and linked trait
    language edit <which> - begins editing a language
    language close - stops editing a language
    language set ... - sets the properties of a language you're editing";

	[PlayerCommand("Language", "language")]
	[HelpInfo("language", LanguageCommandHelp, AutoHelp.HelpArgOrNoArg, AdminLanguageCommandHelp)]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Language(ICharacter actor, string command)
	{
		if (!actor.IsAdministrator())
		{
			PlayerLanguage(actor, command);
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				LanguageEdit(actor, ss);
				return;
			case "set":
				LanguageSet(actor, ss);
				return;
			case "close":
				LanguageClose(actor, ss);
				return;
			case "view":
			case "show":
				LanguageView(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(AdminLanguageCommandHelp);
				return;
		}
	}

	protected static void PlayerLanguage(ICharacter actor, string input)
	{
		var which = input.RemoveFirstWord();
		var languages = actor.IsAdministrator() ? actor.Gameworld.Languages.ToList() : actor.Languages.ToList();
		var language = languages.FirstOrDefault(x => x.Name.EqualTo(which)) ??
		               languages.FirstOrDefault(x =>
			               x.Name.StartsWith(which, StringComparison.InvariantCultureIgnoreCase)) ??
		               languages.FirstOrDefault(
			               x => x.Name.Contains(which, StringComparison.InvariantCultureIgnoreCase));

		if (language == null)
		{
			actor.OutputHandler.Send("You do not know any such language.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Information for the {language.Name.Proper().Colour(Telnet.Green)} language.");
		sb.AppendLine($"Trait: {language.LinkedTrait.Name.Colour(Telnet.Green)}");
		sb.AppendLine($"Unknown Description: {language.UnknownLanguageSpokenDescription.Colour(Telnet.Yellow)}");
		sb.AppendLine();
		sb.AppendLine("Normally written with one of the following scripts:");
		var scripts = actor.Gameworld.Scripts.Where(x => x.DesignedLanguages.Contains(language)).ToList();
		if (!scripts.Any())
		{
			sb.AppendLine("\tNone that you are aware of.");
		}
		else
		{
			foreach (var script in scripts)
			{
				sb.AppendLine($"\t{script.KnownScriptDescription}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Offers mutual intelligibility with the following languages:");
		var mutuals = actor.Gameworld.Languages.Where(x => language.MutualIntelligability(x) != Difficulty.Impossible)
		                   .ToList();
		if (!mutuals.Any())
		{
			sb.AppendLine("\tNone that you are aware of.");
		}
		else
		{
			foreach (var mutual in mutuals)
			{
				sb.AppendLine(
					$"\t{mutual.Name.Proper()} @ {language.MutualIntelligability(mutual).Describe().Colour(Telnet.Green)}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}


	private static void LanguageClose(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILanguage>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any languages.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send(
			$"You are no longer editing the {editing.EditingItem.Name.ColourName()} language.");
	}

	private static void LanguageView(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILanguage>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("You are not editing any languages.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var language = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Languages.Get(value)
			: actor.Gameworld.Languages.GetByName(ss.SafeRemainingArgument);
		if (language == null)
		{
			actor.OutputHandler.Send("There is no such language.");
			return;
		}

		actor.OutputHandler.Send(language.Show(actor));
	}

	private static void LanguageSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILanguage>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send(
				"You are not editing any languages. Please specify a language that you want to edit.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void LanguageEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILanguage>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send(
					"You are not editing any languages. Please specify a language that you want to edit.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new language?");
				return;
			}

			var name = ss.PopSpeech();
			if (actor.Gameworld.Languages.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a language with that name. Names must be unique.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which trait do you want to use as the linked trait for this language?");
				return;
			}

			var trait = long.TryParse(ss.SafeRemainingArgument, out var tvalue)
				? actor.Gameworld.Traits.Get(tvalue)
				: actor.Gameworld.Traits.GetByName(ss.SafeRemainingArgument);
			if (trait == null)
			{
				actor.OutputHandler.Send("There is no such trait.");
				return;
			}

			var newLanguage = new Language(actor.Gameworld, trait, name);
			actor.Gameworld.Add(newLanguage);
			actor.RemoveAllEffects<BuilderEditingEffect<ILanguage>>();
			actor.AddEffect(new BuilderEditingEffect<ILanguage>(actor) { EditingItem = newLanguage });
			actor.OutputHandler.Send(
				$"You create a new language called {newLanguage.Name.ColourName()} with Id #{newLanguage.Id.ToString("N0", actor)}, which you are now editing.");
			return;
		}

		var language = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Languages.Get(value)
			: actor.Gameworld.Languages.GetByName(ss.SafeRemainingArgument);
		if (language == null)
		{
			actor.OutputHandler.Send("There is no such language.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ILanguage>>();
		actor.AddEffect(new BuilderEditingEffect<ILanguage>(actor) { EditingItem = language });
		actor.OutputHandler.Send($"You are now editing the {language.Name.ColourName()} language.");
	}

	#endregion

	#region Accents

	private const string AccentPlayerHelp =
		"The accent command allows you to view detailed information about a particular accent that you speak. The syntax is ACCENT <which accent>, or ACCENT <language>.<which accent> if you have accents that have the same name in multiple languages.";

	private const string AccentAdminHelp =
		@"This command allows you to create and edit accents. You can use the following options with this command:

    accent show <which> - shows you an accent
    accent edit new <name> <language> - creates a new accent with the specified name for a language
    accent edit <which> - begins editing an accent
    accent close - stops editing an accent
    accent set ... - sets the properties of an accent you're editing";

	[PlayerCommand("Accent", "accent")]
	[HelpInfo("accent", AccentPlayerHelp, AutoHelp.HelpArgOrNoArg, AccentAdminHelp)]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Accent(ICharacter actor, string input)
	{
		if (!actor.IsAdministrator())
		{
			PlayerAccent(actor, input);
			return;
		}

		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				AccentEdit(actor, ss);
				return;
			case "close":
				AccentClose(actor);
				return;
			case "set":
				AccentSet(actor, ss);
				return;
			case "view":
			case "show":
				AccentShow(actor, ss);
				return;
		}
	}

	private static void AccentEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IAccent>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send(
					"You are not editing any accents. Please specify an accent that you want to edit.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new accent?");
				return;
			}

			var name = ss.PopSpeech();

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What language do you want to create an accent for?");
				return;
			}

			var language = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.Languages.Get(value)
				: actor.Gameworld.Languages.GetByName(ss.Last);
			if (language == null)
			{
				actor.OutputHandler.Send("There is no such language.");
				return;
			}

			if (actor.Gameworld.Accents.Any(x => x.Name.EqualTo(name) && x.Language == language))
			{
				actor.OutputHandler.Send(
					"There is already an accent with that name for that language. Names must be unique.");
				return;
			}

			var newAccent = new Accent(actor.Gameworld, language, name);
			actor.Gameworld.Add(newAccent);
			actor.RemoveAllEffects<BuilderEditingEffect<IAccent>>();
			actor.AddEffect(new BuilderEditingEffect<IAccent>(actor) { EditingItem = newAccent });
			actor.OutputHandler.Send(
				$"You create a new accent called {newAccent.Name.ColourName()} with Id #{newAccent.Id.ToString("N0", actor)} for the {language.Name.ColourName()} language, which you are now editing.");
			return;
		}

		var accent = long.TryParse(ss.SafeRemainingArgument, out var evalue)
			? actor.Gameworld.Accents.Get(evalue)
			: actor.Gameworld.Accents.GetByName(ss.SafeRemainingArgument);
		if (accent == null)
		{
			actor.OutputHandler.Send("There is no such accent.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IAccent>>();
		actor.AddEffect(new BuilderEditingEffect<IAccent>(actor) { EditingItem = accent });
		actor.OutputHandler.Send($"You are now editing the {accent.Name.ColourName()} accent.");
	}

	private static void AccentClose(ICharacter actor)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IAccent>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any accents.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send(
			$"You are no longer editing the {editing.EditingItem.Name.ColourName()} accent.");
	}

	private static void AccentSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IAccent>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send(
				"You are not editing any accents. Please specify an accent that you want to edit.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void AccentShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IAccent>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("You are not editing any accents.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var accent = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Accents.Get(value)
			: actor.Gameworld.Accents.GetByName(ss.SafeRemainingArgument);
		if (accent == null)
		{
			actor.OutputHandler.Send("There is no such accent.");
			return;
		}

		actor.OutputHandler.Send(accent.Show(actor));
	}


	protected static void PlayerAccent(ICharacter actor, string input)
	{
		var which = input.RemoveFirstWord();
		var language = default(ILanguage);
		if (which.Contains('.'))
		{
			var split = which.Split(new[] { '.' }, 2);
			var lang = split[0];
			var languages = actor.IsAdministrator() ? actor.Gameworld.Languages.ToList() : actor.Languages.ToList();
			language = languages.FirstOrDefault(x => x.Name.EqualTo(lang)) ??
			           languages.FirstOrDefault(x =>
				           x.Name.StartsWith(lang, StringComparison.InvariantCultureIgnoreCase)) ??
			           languages.FirstOrDefault(x =>
				           x.Name.Contains(lang, StringComparison.InvariantCultureIgnoreCase));

			if (language == null)
			{
				actor.OutputHandler.Send("You do not know any such language.");
				return;
			}

			which = split[1];
		}

		var accents = actor.IsAdministrator() ? actor.Gameworld.Accents.ToList() : actor.Accents.ToList();
		if (language != null)
		{
			accents = accents.Where(x => x.Language == language).ToList();
		}

		var accent = accents.FirstOrDefault(x => x.Name.EqualTo(which)) ??
		             accents.FirstOrDefault(x =>
			             x.Name.StartsWith(which, StringComparison.InvariantCultureIgnoreCase)) ??
		             accents.FirstOrDefault(x => x.Name.Contains(which, StringComparison.InvariantCultureIgnoreCase));
		if (accent == null)
		{
			actor.OutputHandler.Send(
				$"You do not know any such accent{(language != null ? $" in the {language.Name.Proper().Colour(Telnet.Green)} language" : "")}.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"Information about the {accent.Name.TitleCase().Colour(Telnet.Green)} accent for the {accent.Language.Name.Proper().Colour(Telnet.Green)} language.");
		sb.AppendLine($"Group: {accent.Group.TitleCase().Colour(Telnet.Green)}");
		sb.AppendLine($"Difficulty for listener if not known: {accent.Difficulty.Describe().Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Difficulty for those who know an accent from the same group: {accent.Difficulty.StageDown(1).Describe().Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Difficulty for those who know 3+ accents from the same group: {accent.Difficulty.StageDown(2).Describe().Colour(Telnet.Green)}");
		sb.AppendLine($"Known Suffix: {accent.AccentSuffix.Colour(Telnet.Yellow)}");
		sb.AppendLine($"Unknown Suffix: {accent.VagueSuffix.Colour(Telnet.Yellow)}");
		sb.AppendLine();
		sb.AppendLine($"Accent description:");
		sb.AppendLine();
		sb.AppendLine(accent.Description.Wrap(actor.InnerLineFormatLength));
		actor.OutputHandler.Send(sb.ToString());
	}

	#endregion

	#region Scripts

	private const string ScriptCommandPlayerHelp =
		"This command allows you to set which script and language you use when you write, as well as view information about scripts. For the former usage, the syntax is SCRIPT <script> <language>. For the latter, it is SCRIPT SHOW <script>.";

	private const string ScriptCommandAdminHelp =
		@"This command allows you to create and edit scripts. You can use the following options with this command:

    script show <which> - shows you a script
    script edit new <name> <knowledge> - creates a new script with the specified name and linked knowledge
    script edit <which> - begins editing a script
    script close - stops editing a script
    script set ... - sets the properties of a script you're editing";

	[PlayerCommand("Script", "script")]
	[HelpInfo("script", ScriptCommandPlayerHelp, AutoHelp.HelpArgOrNoArg, ScriptCommandAdminHelp)]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Script(ICharacter actor, string command)
	{
		if (!actor.IsAdministrator())
		{
			PlayerScript(actor, command);
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				ScriptEdit(actor, ss);
				return;
			case "set":
				ScriptSet(actor, ss);
				return;
			case "show":
			case "view":
				ScriptShow(actor, ss);
				return;
			case "close":
				ScriptClose(actor, ss);
				return;
		}
	}

	private static void ScriptClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IScript>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any scripts.");
			return;
		}

		actor.RemoveEffect(effect);
		actor.OutputHandler.Send($"You are no longer editing the {effect.EditingItem.Name.ColourName()} script.");
	}

	private static void ScriptShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IScript>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which script do you want to show?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var script = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Scripts.Get(value)
			: actor.Gameworld.Scripts.GetByName(ss.SafeRemainingArgument);
		if (script == null)
		{
			actor.OutputHandler.Send("There is no such script.");
			return;
		}

		actor.OutputHandler.Send(script.Show(actor));
	}

	private static void ScriptSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IScript>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any scripts.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void ScriptEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IScript>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which script do you want to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var cmd = ss.PopSpeech();
		if (cmd.EqualTo("new"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which name do you want to give to your new script?");
				return;
			}

			var name = ss.PopSpeech().ToLowerInvariant().TitleCase();
			if (actor.Gameworld.Scripts.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a script with that name. Names must be unique.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					"Which knowledge do you want to use to control access to this script? You must build this knowledge first and supply it at the time of script creation.");
				return;
			}

			var knowledge = long.TryParse(ss.SafeRemainingArgument, out var value)
				? actor.Gameworld.Knowledges.Get(value)
				: actor.Gameworld.Knowledges.GetByName(ss.SafeRemainingArgument);
			if (knowledge == null)
			{
				actor.OutputHandler.Send("There is no such knowledge.");
				return;
			}

			var newScript = new Script(actor.Gameworld, name, knowledge);
			actor.Gameworld.Add(newScript);
			actor.RemoveAllEffects<BuilderEditingEffect<IScript>>();
			actor.AddEffect(new BuilderEditingEffect<IScript>(actor) { EditingItem = newScript });
			actor.OutputHandler.Send(
				$"You create a new script with Id #{newScript.Id.ToString("N0", actor)} called {name.ColourName()} linked with knowledge {knowledge.Name.ColourName()}.");
			return;
		}

		var script = long.TryParse(cmd, out var lvalue)
			? actor.Gameworld.Scripts.Get(lvalue)
			: actor.Gameworld.Scripts.GetByName(cmd);
		if (script == null)
		{
			actor.OutputHandler.Send("There is no such script.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IScript>>();
		actor.AddEffect(new BuilderEditingEffect<IScript>(actor) { EditingItem = script });
		actor.OutputHandler.Send($"You are now editing the script called {script.Name.ColourName()}.");
	}


	protected static void PlayerScript(ICharacter actor, string command)
	{
		if (!actor.IsLiterate)
		{
			actor.Send("You're illiterate. You don't know anything about fancy squiggly lines.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send(actor.CurrentScript != null
				? $"When you write, you use the {actor.CurrentScript.Name.Colour(Telnet.Green)} script and the {actor.CurrentWritingLanguage.Name.Colour(Telnet.Green)} language."
				: $"You don't have a current script set.");
			return;
		}

		var text = ss.PopSpeech();
		IScript script = null;
		var showMode = false;
		if (text.EqualToAny("view", "show", "info"))
		{
			if (ss.IsFinished)
			{
				actor.Send("Which script do you want to show information about?");
				return;
			}

			text = ss.PopSpeech();
			showMode = true;
		}

		if (!actor.IsAdministrator())
		{
			script = actor.Scripts.FirstOrDefault(
				x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		}
		else
		{
			script = actor.Gameworld.Scripts.FirstOrDefault(
				x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		}

		if (script == null)
		{
			actor.Send("You don't know any script like that.");
			return;
		}

		if (showMode)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Information about the script {script.Name.TitleCase().Colour(Telnet.Green)}.");
			sb.AppendLine($"Linked Knowledge: {script.ScriptKnowledge.Name.Colour(Telnet.Green)}");
			sb.AppendLine($"Known Description: {script.KnownScriptDescription.Colour(Telnet.Yellow)}");
			sb.AppendLine($"Unknown Description: {script.UnknownScriptDescription.Colour(Telnet.Yellow)}");
			sb.AppendLine(
				$"Document Length Modifier: {script.DocumentLengthModifier.ToString("N2", actor).Colour(Telnet.Green)}");
			sb.AppendLine($"Ink Use Modifier: {script.InkUseModifier.ToString("N2", actor).Colour(Telnet.Green)}");
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		if (ss.IsFinished && actor.CurrentWritingLanguage == null)
		{
			actor.Send("You must also specify a language to use when writing.");
			return;
		}

		var language = actor.CurrentWritingLanguage;
		if (!ss.IsFinished)
		{
			text = ss.PopSpeech();
			language = actor.IsAdministrator()
				? actor.Gameworld.Languages.FirstOrDefault(x =>
					x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
				: actor.Languages.FirstOrDefault(x =>
					x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
			if (language == null)
			{
				actor.Send("You don't know any language like that.");
				return;
			}
		}

		actor.CurrentScript = script;
		actor.CurrentWritingLanguage = language;
		actor.Send(
			$"You will now use the {script.Name.Colour(Telnet.Green)} script and the {language.Name.Colour(Telnet.Green)} language when you write.");
	}

	#endregion

	#region Knowledges

	private const string KnowledgeCommandHelp =
		"This command allows you to view detailed information about a knowledge that you have. The syntax is KNOWLEDGE <name>, and can be used on any knowledge that you know.";

	private const string AdminKnowledgeCommandHelp =
		@"This command allows you to create and edit knowledges. You can use the following options with this command:

    knowledge show <which> - shows you a knowledge
    knowledge edit new <name> - creates a new knowledge with the specified name
    knowledge edit <which> - begins editing a language
    knowledge close - stops editing a knowledge
    knowledge set ... - sets the properties of a knowledge you're editing";

	[PlayerCommand("Knowledge", "knowledge")]
	[HelpInfo("knowledge", KnowledgeCommandHelp, AutoHelp.HelpArgOrNoArg, AdminKnowledgeCommandHelp)]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Knowledge(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (!actor.IsAdministrator())
		{
			PlayerKnowledge(actor, ss);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				KnowledgeEdit(actor, ss);
				return;
			case "show":
				KnowledgeShow(actor, ss);
				return;
			case "close":
				KnowledgeClose(actor, ss);
				return;
			case "set":
				KnowledgeSet(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(AdminKnowledgeCommandHelp);
				return;
		}
	}

	private static void KnowledgeSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IKnowledge>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any knowledges.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void KnowledgeEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IKnowledge>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("Which knowledge do you want to edit?");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var cmd = ss.PopSpeech();
		if (cmd.EqualTo("new"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the new knowledge?");
				return;
			}

			var name = ss.SafeRemainingArgument;
			if (actor.Gameworld.Knowledges.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a knowledge with that name. Names must be unique.");
				return;
			}

			var newKnowledge = new Knowledge(name);
			actor.Gameworld.Add(newKnowledge);
			actor.RemoveAllEffects<BuilderEditingEffect<IKnowledge>>();
			actor.AddEffect(new BuilderEditingEffect<IKnowledge>(actor) { EditingItem = newKnowledge });
			actor.OutputHandler.Send(
				$"You create a new knowledge called {name.ColourName()}, which you are now editing.");
			return;
		}

		var knowledge = long.TryParse(cmd, out var value)
			? actor.Gameworld.Knowledges.Get(value)
			: actor.Gameworld.Knowledges.GetByName(cmd);
		if (knowledge == null)
		{
			actor.OutputHandler.Send("There is no such knowledge.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IKnowledge>>();
		actor.AddEffect(new BuilderEditingEffect<IKnowledge>(actor) { EditingItem = knowledge });
		actor.OutputHandler.Send($"You are now editing {knowledge.Name.ColourName()}.");
	}

	private static void KnowledgeShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IKnowledge>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("You are not editing any knowledges.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var knowledge = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Knowledges.Get(value)
			: actor.Gameworld.Knowledges.GetByName(ss.SafeRemainingArgument);
		if (knowledge == null)
		{
			actor.OutputHandler.Send("There is no such knowledge.");
			return;
		}

		actor.OutputHandler.Send(knowledge.Show(actor));
	}

	private static void KnowledgeClose(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IKnowledge>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any knowledges.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send($"You are no longer editing the {editing.EditingItem.Name.ColourName()} knowledge.");
	}

	private static void PlayerKnowledge(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which of your knowledges would you like to view more information on?");
			return;
		}

		var target = ss.SafeRemainingArgument;
		var knowledge = actor.CharacterKnowledges.FirstOrDefault(x => x.Knowledge.Name.EqualTo(target)) ??
		                actor.CharacterKnowledges.FirstOrDefault(x => x.Knowledge.Description.EqualTo(target)) ??
		                actor.CharacterKnowledges.FirstOrDefault(x =>
			                x.Name.StartsWith(target, StringComparison.InvariantCultureIgnoreCase));
		if (knowledge == null)
		{
			actor.OutputHandler.Send("You don't know of any knowledge like that.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Information about the {knowledge.Knowledge.Name.ColourName()} Knowledge:");
		sb.AppendLine($"Description: {knowledge.Knowledge.Description.ColourCommand()}");
		sb.AppendLine($"Long Description:");
		sb.AppendLine();
		sb.AppendLine(knowledge.Knowledge.LongDescription.Wrap(actor.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine($"Teach Difficulty: {knowledge.Knowledge.TeachDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Learn Difficulty: {knowledge.Knowledge.LearnDifficulty.Describe().ColourValue()}");
		sb.AppendLine(
			$"Learner Sessions Required: {knowledge.Knowledge.LearnerSessionsRequired.ToString("N0", actor).ColourValue()}");
		actor.OutputHandler.Send(sb.ToString());
	}

	#endregion

	#region Butchering

	private const string ButcheringHelp = "";

	[PlayerCommand("Butchering", "butchering", "butchery")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("butchering", ButcheringHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Butchering(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				ButcheringEdit(actor, ss);
				return;
			case "show":
				ButcheringShow(actor, ss);
				return;
			case "close":
				ButcheringClose(actor);
				return;
			case "set":
				ButcheringSet(actor, ss);
				return;
			case "clone":
				ButcheringClone(actor, ss, true);
				return;
			case "shallowclone":
				ButcheringClone(actor, ss, false);
				return;
		}
	}

	private static void ButcheringClone(ICharacter actor, StringStack ss, bool copyproducts)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which butchery profile do you want to clone?");
			return;
		}

		var profile = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.RaceButcheryProfiles.Get(value)
			: actor.Gameworld.RaceButcheryProfiles.GetByName(ss.Last);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such butchery profile.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the newly cloned butchery profile?");
			return;
		}

		var name = ss.SafeRemainingArgument.ToLowerInvariant().TitleCase();
		if (actor.Gameworld.RaceButcheryProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already butchery profile with that name. Names must be unique.");
			return;
		}

		var newprofile = profile.Clone(name, copyproducts);
		actor.Gameworld.Add(newprofile);
		actor.RemoveAllEffects<BuilderEditingEffect<IRaceButcheryProfile>>();
		actor.AddEffect(new BuilderEditingEffect<IRaceButcheryProfile>(actor) { EditingItem = newprofile });
		actor.OutputHandler.Send(
			$"You clone butchery profile {profile.Name.ColourName()} as {name.ColourName()} (Id #{newprofile.Id.ToString("N0", actor)}), which you are now editing.");
	}

	private static void ButcheringEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IRaceButcheryProfile>>()
			                   .FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("Which butchery profile do you want to edit?");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var cmd = ss.PopSpeech();
		if (cmd.EqualTo("new"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new butchery profile?");
				return;
			}

			var name = ss.SafeRemainingArgument.ToLowerInvariant().TitleCase();
			if (actor.Gameworld.RaceButcheryProfiles.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a butchery profile with that name. Names must be unique.");
				return;
			}

			var newprofile = new RaceButcheryProfile(actor.Gameworld, name);
			actor.Gameworld.Add(newprofile);
			actor.RemoveAllEffects<BuilderEditingEffect<IRaceButcheryProfile>>();
			actor.AddEffect(new BuilderEditingEffect<IRaceButcheryProfile>(actor) { EditingItem = newprofile });
			actor.OutputHandler.Send(
				$"You create a new butchery profile called {name.ColourName()} (Id #{newprofile.Id.ToString("N0", actor)}), which you are now editing.");
			return;
		}

		var profile = long.TryParse(cmd, out var value)
			? actor.Gameworld.RaceButcheryProfiles.Get(value)
			: actor.Gameworld.RaceButcheryProfiles.GetByName(cmd);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such butchery profile.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IRaceButcheryProfile>>();
		actor.AddEffect(new BuilderEditingEffect<IRaceButcheryProfile>(actor) { EditingItem = profile });
		actor.OutputHandler.Send(
			$"You are now editing butchery profile #{profile.Id.ToString("N0", actor)} ({profile.Name.ColourName()}).");
	}

	private static void ButcheringShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IRaceButcheryProfile>>()
			                   .FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("Which butchery profile do you want to show?");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var profile = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.RaceButcheryProfiles.Get(value)
			: actor.Gameworld.RaceButcheryProfiles.GetByName(ss.SafeRemainingArgument);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such butchery profile.");
			return;
		}

		actor.OutputHandler.Send(profile.Show(actor));
	}

	private static void ButcheringSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IRaceButcheryProfile>>()
		                   .FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any butchery profiles.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void ButcheringClose(ICharacter actor)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IRaceButcheryProfile>>()
		                   .FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any butchery profiles.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send(
			$"You are no longer editing the {editing.EditingItem.Name.ColourName()} butchery profile.");
	}

	#endregion

	#region Economic Zones

	public const string EconomicZoneHelp =
		@"This command allows you to view, create and edit economic zones, which are the backbones of all economic activity on your MUD.

The syntax for this command is as follows:

	#3ez list#0 - lists all of the economic zones
	#3ez edit <which>#0 - begins editing an economic zone
	#3ez edit new <zone> <name>#0 - generates a new economic zone using a real zone as a reference location
	#3ez clone <old> <new name>#0 - clones an existing economic zone to a new one
	#3ez close#0 - stops editing an economic zone
	#3ez show <which>#0 - views information about an economic zone
	#3ez show#0 - views information about your currently editing economic zone
	#3ez set name <name>#0 - renames this economic zone
	#3ez set currency <currency>#0 - changes the currency used in this zone
	#3ez set clock <clock>#0 - changes the clock used in this zone
	#3ez set calendar <calendar>#0 - changes the calendar used in this zone
	#3ez set interval <type> <amount> <offset>#0 - sets the interval for financial periods
	#3ez set time <time>#0 - sets the reference time for financial periods
	#3ez set timezone <tz>#0 - sets the reference timezone for this zone
	#3ez set zone <zone>#0 - sets the physical zone used as a reference for current time
	#3ez set previous <amount>#0 - sets the number of previous financial periods to keep records for
	#3ez set permitloss#0 - toggles permitting taxable losses
	#3ez set clan <clan>#0 - assigns a new clan to custody of this economic zone
	#3ez set clan none#0 - clears clan control of this economic zone
	#3ez set salestax add <type> <name>#0 - adds a new sales tax
	#3ez set salestax remove <name>#0 - removes a sales tax
	#3ez set salestax <which> <...>#0 - edit properties of a particular tax
	#3ez set profittax add <type> <name>#0 - adds a new profit tax
	#3ez set profittax remove <name>#0 - removes a profit tax
	#3ez set profittax <which> <...>#0 - edit properties of a particular tax
	#3ez set realty#0 - toggles your current location as a conveyancing/realty location
	#3ez set jobs#0 - toggles your current location as a job listing and finding location";

	[PlayerCommand("EconomicZone", "economiczone", "ez")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void EconomicZone(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				EconomicZoneList(actor, ss);
				return;
			case "edit":
				EconomicZoneEdit(actor, ss);
				return;
			case "close":
				EconomicZoneClose(actor, ss);
				return;
			case "show":
			case "view":
				EconomicZoneShow(actor, ss);
				return;
			case "set":
				EconomicZoneSet(actor, ss);
				return;
			case "clone":
				EconomicZoneClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(EconomicZoneHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void EconomicZoneClone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone do you want to clone?");
			return;
		}

		var zone = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.EconomicZones.Get(value)
			: actor.Gameworld.EconomicZones.GetByName(command.Last);
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your newly cloned zone?");
			return;
		}

		var name = command.SafeRemainingArgument;
		if (actor.Gameworld.EconomicZones.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already an economic zone with that name. Names must be unique.");
			return;
		}

		var clone = zone.Clone(name);
		actor.Gameworld.Add(clone);
		actor.OutputHandler.Send(
			$"You clone the {zone.Name.ColourName()} economic zone into a new zone called {clone.Name.ColourName()}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IEconomicZone>>();
		actor.AddEffect(new BuilderEditingEffect<IEconomicZone>(actor) { EditingItem = clone });
	}

	private static void EconomicZoneClose(ICharacter actor, StringStack command)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IEconomicZone>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any economic zones.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send(
			$"You are no longer editing the {editing.EditingItem.Name.ColourName()} economic zone.");
	}

	private static void EconomicZoneShow(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IEconomicZone>>().FirstOrDefault();
			if (editing != null)
			{
				actor.OutputHandler.Send(editing.EditingItem.Show(actor));
				return;
			}

			actor.OutputHandler.Send("Which economic zone do you want to show?");
			return;
		}

		var target = long.TryParse(command.SafeRemainingArgument, out var id)
			? actor.Gameworld.EconomicZones.Get(id)
			: actor.Gameworld.EconomicZones.GetByName(command.SafeRemainingArgument);
		if (target == null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return;
		}

		actor.OutputHandler.Send(target.Show(actor));
	}

	private static void EconomicZoneSet(ICharacter actor, StringStack command)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IEconomicZone>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing an economic zone.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, command);
	}

	private static void EconomicZoneEdit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IEconomicZone>>().FirstOrDefault();
			if (editing != null)
			{
				actor.OutputHandler.Send(editing.EditingItem.Show(actor));
				return;
			}

			actor.OutputHandler.Send("Which economic zone do you want to edit?");
			return;
		}

		if (command.PeekSpeech().EqualTo("new"))
		{
			command.PopSpeech();
			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					"Which zone do you want to use as a reference zone for time for this economic zone?");
				return;
			}

			var zone = long.TryParse(command.PopSpeech(), out var value)
				? actor.Gameworld.Zones.Get(value)
				: actor.Gameworld.Zones.GetByName(command.Last);
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such zone.");
				return;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the new economic zone?");
				return;
			}

			var name = command.SafeRemainingArgument.ToLowerInvariant().TitleCase();
			if (actor.Gameworld.EconomicZones.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already an economic zone with that name. Names must be unique.");
				return;
			}

			var economicZone = new EconomicZone(actor.Gameworld, zone, name);
			actor.Gameworld.Add(economicZone);
			actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IEconomicZone>>());
			actor.AddEffect(new BuilderEditingEffect<IEconomicZone>(actor) { EditingItem = economicZone });
			actor.OutputHandler.Send(
				$"You create a new economic zone called {name.ColourName()} based around the {zone.Name.ColourName()} zone.");
			return;
		}

		var target = long.TryParse(command.SafeRemainingArgument, out var id)
			? actor.Gameworld.EconomicZones.Get(id)
			: actor.Gameworld.EconomicZones.GetByName(command.SafeRemainingArgument);
		if (target == null)
		{
			actor.OutputHandler.Send("There is no such economic zone to edit.");
			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IEconomicZone>>());
		actor.AddEffect(new BuilderEditingEffect<IEconomicZone>(actor) { EditingItem = target });
		actor.OutputHandler.Send($"You are now editing the {target.Name.ColourName()} economic zone.");
	}

	private static void EconomicZoneList(ICharacter actor, StringStack command)
	{
		var zones = actor.Gameworld.EconomicZones.ToList();
		actor.OutputHandler.Send("Economic Zones:\n" + StringUtilities.GetTextTable(
			from zone in zones
			select new List<string>
			{
				zone.Id.ToString("N0", actor),
				zone.Name,
				zone.Currency.Name,
				actor.Gameworld.Shops.Count(x => x.EconomicZone == zone).ToString("N0", actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Currency",
				"# Shops"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	#endregion

	#region Random Names

	private const string RandomNameHelp =
		@"This command allows you to view, create and edit random name profiles, which are used to generate random names for variable NPCs or storyteller use.

For the closely related command to generate your own names on the fly, see RANDOMNAMES.

The syntax for this command is as follows:

    randomname list - lists all of the random name profiles
    randomname list <culture> - lists all of the random name profiles for a specific culture
    randomname edit <which> - begins editing a random name profile
    randomname edit new <culture> <name> <gender> - generates a new random name profile
    randomname clone <old> <new> - clones an existing random name profile to a new one
    randomname close - stops editing a random name profile
    randomname show <which> - views information about a random name profile
    randomname show - views information about your currently editing random name profile
    randomname set ... - edits the properties of a random name culture";

	[PlayerCommand("RandomName", "randomname", "rn")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("randomname", RandomNameHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void RandomName(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech())
		{
			case "list":
				RandomNameList(actor, ss);
				return;
			case "edit":
				RandomNameEdit(actor, ss);
				return;
			case "close":
				RandomNameClose(actor, ss);
				return;
			case "set":
				RandomNameSet(actor, ss);
				return;
			case "show":
			case "view":
				RandomNameShow(actor, ss);
				return;
			case "clone":
				RandomNameClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(RandomNameHelp);
				return;
		}
	}

	private static void RandomNameList(ICharacter actor, StringStack ss)
	{
		var profiles = actor.Gameworld.RandomNameProfiles.ToList();
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in profiles
			select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.Gender.DescribeEnum(),
				item.Culture.Name,
				item.RandomNames
				    .Select(x =>
					    $"{x.Value.Count.ToString("N0", actor)} {x.Key.DescribeEnum().Pluralise()}".ColourValue())
				    .ListToCommaSeparatedValues(", ")
			},
			new List<string>
			{
				"Id",
				"Name",
				"Gender",
				"Name Culture",
				"No. Elements"
			},
			actor.LineFormatLength,
			colour: Telnet.Cyan,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void RandomNameEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IRandomNameProfile>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which random name profile would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which name culture would you like to create a random name profile for?");
				return;
			}

			var culture = long.TryParse(ss.PopSpeech(), out var profileid)
				? actor.Gameworld.NameCultures.Get(profileid)
				: actor.Gameworld.NameCultures.GetByName(ss.Last);
			if (culture == null)
			{
				actor.OutputHandler.Send("There is no such name culture.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new random name profile?");
				return;
			}

			var name = ss.PopSpeech().TitleCase();
			if (actor.Gameworld.RandomNameProfiles.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a random name profile for the {culture.Name.ColourName()} name culture with that name. Names must be unique.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What gender should this random name profile generate names for?");
				return;
			}

			if (!ss.SafeRemainingArgument.TryParseEnum<Gender>(out var gender))
			{
				actor.OutputHandler.Send(
					$"That is not a valid gender. The valid genders are {Enum.GetNames<Gender>().Select(x => x.ColourValue()).ListToString()}.");
				return;
			}

			var newProfile = new RandomNameProfile(culture, gender, name);
			actor.Gameworld.Add(newProfile);
			actor.OutputHandler.Send(
				$"You create a new random name profile called {name.ColourName()}, which you are now editing.");
			actor.RemoveAllEffects<BuilderEditingEffect<IRandomNameProfile>>();
			actor.AddEffect(new BuilderEditingEffect<IRandomNameProfile>(actor) { EditingItem = newProfile });
			return;
		}

		var randomProfile = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.RandomNameProfiles.Get(value)
			: actor.Gameworld.RandomNameProfiles.GetByName(ss.SafeRemainingArgument);
		if (randomProfile == null)
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IRandomNameProfile>>();
		actor.AddEffect(new BuilderEditingEffect<IRandomNameProfile>(actor) { EditingItem = randomProfile });
		actor.OutputHandler.Send($"You are now editing the name culture {randomProfile.Name.ColourName()}.");
	}

	private static void RandomNameClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IRandomNameProfile>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any random name profiles.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IRandomNameProfile>>();
		actor.OutputHandler.Send("You are no longer editing any random name profiles.");
	}

	private static void RandomNameShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IRandomNameProfile>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which random name profile would you like to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var randomProfile = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.RandomNameProfiles.Get(value)
			: actor.Gameworld.RandomNameProfiles.GetByName(ss.SafeRemainingArgument);
		if (randomProfile == null)
		{
			actor.OutputHandler.Send("There is no such random name profile.");
			return;
		}

		actor.OutputHandler.Send(randomProfile.Show(actor));
	}

	private static void RandomNameSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IRandomNameProfile>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any random name profiles.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void RandomNameClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which random name profile do you want to clone?");
			return;
		}

		var profile = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.RandomNameProfiles.Get(value)
			: actor.Gameworld.RandomNameProfiles.GetByName(ss.Last);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such random name profile.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new random name profile?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.RandomNameProfiles.Any(x => x.Culture == profile.Culture && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a random name profile for that name culture with that name. Names must be unique per name culture.");
			return;
		}

		var clone = new RandomNameProfile((RandomNameProfile)profile, name);
		actor.Gameworld.Add(clone);
		actor.OutputHandler.Send(
			$"You clone the {profile.Name.ColourName()} random name profile, creating a new one called {name.ColourName()}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IRandomNameProfile>>();
		actor.AddEffect(new BuilderEditingEffect<IRandomNameProfile>(actor) { EditingItem = clone });
	}

	#endregion

	#region Name Cultures

	private const string NameCultureHelp =
		@"Naming cultures are used by cultures to determine what valid names an individual can have. They contain rules about the number and type of names, as well as rules for how they are combined in different circumstances.

The correct syntax for this command is as follows:

    nameculture list - lists all of the name cultures
    nameculture edit <which> - begins editing a name culture
    nameculture edit new <name> - begins editing a new name culture
    nameculture clone <old> <new> - clones an existing name culture to a new one
    nameculture close - stops editing a name culture
    nameculture show <which> - views information about a name culture
    nameculture show - views information about your currently editing name culture
    nameculture set ... - edits properties of a name culture";

	[PlayerCommand("NameCulture", "nameculture", "nc")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("nameculture", NameCultureHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void NameCulture(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech())
		{
			case "list":
				NameCultureList(actor, ss);
				return;
			case "edit":
				NameCultureEdit(actor, ss);
				return;
			case "close":
				NameCultureClose(actor, ss);
				return;
			case "set":
				NameCultureSet(actor, ss);
				return;
			case "show":
			case "view":
				NameCultureShow(actor, ss);
				return;
			case "clone":
				NameCultureClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(NameCultureHelp);
				return;
		}
	}

	private static void NameCultureClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which name culture do you want to clone?");
			return;
		}

		var nameCulture = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.NameCultures.Get(value)
			: actor.Gameworld.NameCultures.GetByName(ss.Last);
		if (nameCulture == null)
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new name culture?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.NameCultures.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a name culture with that name. Names must be unique.");
			return;
		}

		var clone = new NameCulture(nameCulture, name);
		actor.Gameworld.Add(clone);
		actor.OutputHandler.Send(
			$"You clone the {nameCulture.Name.ColourName()} name culture, creating a new one called {name.ColourName()}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<INameCulture>>();
		actor.AddEffect(new BuilderEditingEffect<INameCulture>(actor) { EditingItem = clone });
	}

	private static void NameCultureShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<INameCulture>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which name culture would you like to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var nameCulture = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.NameCultures.Get(value)
			: actor.Gameworld.NameCultures.GetByName(ss.SafeRemainingArgument);
		if (nameCulture == null)
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return;
		}

		actor.OutputHandler.Send(nameCulture.Show(actor));
	}

	private static void NameCultureSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<INameCulture>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any name cultures.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void NameCultureClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<INameCulture>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any name cultures.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<NameCulture>>();
		actor.OutputHandler.Send("You are no longer editing any name cultures.");
	}

	private static void NameCultureEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<INameCulture>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which name culture would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new name culture?");
				return;
			}

			var name = ss.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.NameCultures.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a name culture with that name. Names must be unique.");
				return;
			}

			var newName = new NameCulture(actor.Gameworld, name);
			actor.Gameworld.Add(newName);
			actor.OutputHandler.Send(
				$"You create a new name culture called {name.ColourName()}, which you are now editing.");
			actor.RemoveAllEffects<BuilderEditingEffect<INameCulture>>();
			actor.AddEffect(new BuilderEditingEffect<INameCulture>(actor) { EditingItem = newName });
		}

		var nameCulture = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.NameCultures.Get(value)
			: actor.Gameworld.NameCultures.GetByName(ss.SafeRemainingArgument);
		if (nameCulture == null)
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<INameCulture>>();
		actor.AddEffect(new BuilderEditingEffect<INameCulture>(actor) { EditingItem = nameCulture });
		actor.OutputHandler.Send($"You are now editing the name culture {nameCulture.Name.ColourName()}.");
	}

	private static void NameCultureList(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from culture in actor.Gameworld.NameCultures
			select new List<string>
			{
				culture.Id.ToString("N0", actor),
				culture.Name,
				actor.Gameworld.Cultures
				     .Where(x =>
					     x.NameCultureForGender(Gender.Male) == culture ||
					     x.NameCultureForGender(Gender.Female) == culture ||
					     x.NameCultureForGender(Gender.NonBinary) == culture ||
					     x.NameCultureForGender(Gender.Neuter) == culture ||
					     x.NameCultureForGender(Gender.Indeterminate) == culture
				     )
				     .Select(x => x.Name)
				     .ListToString()
			},
			new List<string>
			{
				"Id",
				"Name",
				"Cultures"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode,
			truncatableColumnIndex: 2
		));
	}

	#endregion

	#region Cultures

	private const string CultureHelp =
		@"Cultures reflect the way in which a character was raised - the society (or lack therefor) in which they grew up.

The correct syntax for this command is as follows:

    culture list - lists all of the cultures
    culture edit <which> - begins editing a culture
    culture edit new <name> - begins editing a new culture
    culture clone <old> <new> - clones an existing culture to a new one
    culture close - stops editing a culture
    culture show <which> - views information about a culture
    culture show - views information about your currently editing culture
    culture set ... - edits properties of a culture";

	[PlayerCommand("Culture", "culture")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Culture(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech())
		{
			case "list":
				CultureList(actor, ss);
				return;
			case "edit":
				CultureEdit(actor, ss);
				return;
			case "close":
				CultureClose(actor, ss);
				return;
			case "set":
				CultureSet(actor, ss);
				return;
			case "show":
			case "view":
				CultureShow(actor, ss);
				return;
			case "clone":
				CultureClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(CultureHelp);
				return;
		}
	}

	private static void CultureClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which culture do you want to clone?");
			return;
		}

		var culture = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Cultures.Get(value)
			: actor.Gameworld.Cultures.GetByName(ss.Last);
		if (culture == null)
		{
			actor.OutputHandler.Send("There is no such culture.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new culture?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Cultures.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a culture with that name. Names must be unique.");
			return;
		}

		var clone = new Culture(culture, name);
		actor.Gameworld.Add(clone);
		actor.OutputHandler.Send(
			$"You clone the {culture.Name.ColourName()} culture, creating a new one called {name.ColourName()}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ICulture>>();
		actor.AddEffect(new BuilderEditingEffect<ICulture>(actor) { EditingItem = clone });
	}

	private static void CultureShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ICulture>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which culture would you like to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var culture = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Cultures.Get(value)
			: actor.Gameworld.Cultures.GetByName(ss.SafeRemainingArgument);
		if (culture == null)
		{
			actor.OutputHandler.Send("There is no such culture.");
			return;
		}

		actor.OutputHandler.Send(culture.Show(actor));
	}

	private static void CultureSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ICulture>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any cultures.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void CultureClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ICulture>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any cultures.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ICulture>>();
		actor.OutputHandler.Send("You are no longer editing any cultures.");
	}

	private static void CultureEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ICulture>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which culture would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new culture?");
				return;
			}

			var name = ss.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.Cultures.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a culture with that name. Names must be unique.");
				return;
			}

			var newCulture = new Culture(actor.Gameworld, name);
			actor.Gameworld.Add(newCulture);
			actor.OutputHandler.Send(
				$"You create a new culture called {name.ColourName()}, which you are now editing.");
			actor.RemoveAllEffects<BuilderEditingEffect<ICulture>>();
			actor.AddEffect(new BuilderEditingEffect<ICulture>(actor) { EditingItem = newCulture });
			return;
		}

		var culture = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Cultures.Get(value)
			: actor.Gameworld.Cultures.GetByName(ss.SafeRemainingArgument);
		if (culture == null)
		{
			actor.OutputHandler.Send("There is no such culture.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ICulture>>();
		actor.AddEffect(new BuilderEditingEffect<ICulture>(actor) { EditingItem = culture });
		actor.OutputHandler.Send($"You are now editing the culture {culture.Name.ColourName()}.");
	}

	private static void CultureList(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from culture in actor.Gameworld.Cultures
			select new List<string>
			{
				culture.Id.ToString("N0", actor),
				culture.Name,
				new[]
				{
					culture.NameCultureForGender(Gender.Male),
					culture.NameCultureForGender(Gender.Female),
					culture.NameCultureForGender(Gender.Neuter),
					culture.NameCultureForGender(Gender.NonBinary),
					culture.NameCultureForGender(Gender.Indeterminate)
				}.Distinct().Select(x => x.Name.TitleCase()).ListToCommaSeparatedValues(", "),
				culture.PrimaryCalendar.ShortName,
				culture.SkillStartingValueProg.MXPClickableFunctionName(),
				culture.AvailabilityProg?.MXPClickableFunctionName() ?? "None"
			},
			new List<string>
			{
				"Id",
				"Name",
				"Name Culture",
				"Calendar",
				"Starting Skills",
				"Availability"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode,
			truncatableColumnIndex: 2
		));
	}

	#endregion

	#region Ethnicities

	private const string EthnicityHelp =
		@"Ethnicities represent some variation of a race, and control things like the normal range of character variables (skin colour, hair colour, eye colour etc) and a few other properties. Ethnicities can also be used to represent breeds with animal races.

See the closely related command for building races.

The correct syntax for this command is as follows:

	#3ethnicity list#0 - lists all of the ethnicities
	#3ethnicity list <race>#0 - lists all the ethnicities for a particular race
	#3ethnicity edit <which>#0 - begins editing a ethnicity
	#3ethnicity edit new <name> <race>#0 - begins editing a new ethnicity
	#3ethnicity clone <old> <new>#0 - clones an existing ethnicity to a new one
	#3ethnicity close#0 - stops editing a ethnicity
	#3ethnicity show <which>#0 - views information about a ethnicity
	#3ethnicity show#0 - views information about your currently editing ethnicity
	#3ethnicity set#0 ... - edits properties of a ethnicity";

	[PlayerCommand("Ethnicity", "ethnicity")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Ethnicity(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech())
		{
			case "list":
				EthnicityList(actor, ss);
				return;
			case "edit":
				EthnicityEdit(actor, ss);
				return;
			case "close":
				EthnicityClose(actor, ss);
				return;
			case "set":
				EthnicitySet(actor, ss);
				return;
			case "show":
			case "view":
				EthnicityShow(actor, ss);
				return;
			case "clone":
				EthnicityClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(EthnicityHelp);
				return;
		}
	}

	private static void EthnicityClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which ethnicity do you want to clone?");
			return;
		}

		var ethnicity = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Ethnicities.Get(value)
			: actor.Gameworld.Ethnicities.GetByName(ss.Last);
		if (ethnicity == null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new ethnicity?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Ethnicities.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already an ethnicity with that name. Names must be unique.");
			return;
		}

		var clone = new Ethnicity(ethnicity, name);
		actor.Gameworld.Add(clone);
		actor.OutputHandler.Send(
			$"You clone the {ethnicity.Name.ColourName()} ethnicity, creating a new one called {name.ColourName()}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IEthnicity>>();
		actor.AddEffect(new BuilderEditingEffect<IEthnicity>(actor) { EditingItem = clone });
	}

	private static void EthnicityShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IEthnicity>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which ethnicity would you like to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var ethnicity = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Ethnicities.Get(value)
			: actor.Gameworld.Ethnicities.GetByName(ss.SafeRemainingArgument);
		if (ethnicity == null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return;
		}

		actor.OutputHandler.Send(ethnicity.Show(actor));
	}

	private static void EthnicitySet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IEthnicity>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any ethnicities.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void EthnicityClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IEthnicity>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any ethnicities.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IEthnicity>>();
		actor.OutputHandler.Send("You are no longer editing any ethnicities.");
	}

	private static void EthnicityEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IEthnicity>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which ethnicity would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new ethnicity?");
				return;
			}

			var name = ss.PopSpeech().TitleCase();

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send($"You must specify a race for this ethnicity to apply to.");
				return;
			}

			var race = long.TryParse(ss.SafeRemainingArgument, out var raceid)
				? actor.Gameworld.Races.Get(raceid)
				: actor.Gameworld.Races.GetByName(ss.SafeRemainingArgument);
			if (race == null)
			{
				actor.OutputHandler.Send("There is no such race.");
				return;
			}


			if (actor.Gameworld.Ethnicities.Any(x => x.ParentRace == race && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an ethnicity for the {race.Name.ColourName()} race with that name. Names must be unique.");
				return;
			}

			var newEthnicity = new Ethnicity(actor.Gameworld, race, name);
			actor.Gameworld.Add(newEthnicity);
			actor.OutputHandler.Send(
				$"You create a new ethnicity called {name.ColourName()}, which you are now editing.");
			actor.RemoveAllEffects<BuilderEditingEffect<IEthnicity>>();
			actor.AddEffect(new BuilderEditingEffect<IEthnicity>(actor) { EditingItem = newEthnicity });
			return;
		}

		var ethnicity = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Ethnicities.Get(value)
			: actor.Gameworld.Ethnicities.GetByName(ss.SafeRemainingArgument);
		if (ethnicity == null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IEthnicity>>();
		actor.AddEffect(new BuilderEditingEffect<IEthnicity>(actor) { EditingItem = ethnicity });
		actor.OutputHandler.Send($"You are now editing the ethnicity {ethnicity.Name.ColourName()}.");
	}

	private static void EthnicityList(ICharacter actor, StringStack ss)
	{
		var ethnicities = actor.Gameworld.Ethnicities.ToList();
		if (!ss.IsFinished)
		{
			var race = long.TryParse(ss.SafeRemainingArgument, out var raceid)
				? actor.Gameworld.Races.Get(raceid)
				: actor.Gameworld.Races.GetByName(ss.SafeRemainingArgument);
			if (race == null)
			{
				actor.OutputHandler.Send("There is no such race.");
				return;
			}

			ethnicities = ethnicities.Where(x => x.ParentRace.SameRace(race)).ToList();
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from ethnicity in ethnicities
			select new List<string>
			{
				ethnicity.Id.ToString("N0", actor),
				ethnicity.Name,
				ethnicity.ParentRace?.Name ?? "",
				ethnicity.EthnicGroup ?? "",
				ethnicity.EthnicSubgroup ?? "",
				ethnicity.PopulationBloodModel?.Name ?? "",
				ethnicity.AvailabilityProg?.MXPClickableFunctionName() ?? "None"
			},
			new List<string>
			{
				"Id",
				"Name",
				"Race",
				"Group",
				"Subgroup",
				"Blood Model",
				"Availability"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode,
			truncatableColumnIndex: 2
		));
	}

	#endregion

	#region Races

	public const string RaceHelp =
		@"The race command is used to view, edit and create races. All characters have a race, and the race represents something approximating a species (although you can make a more broad paradigm and use ethnicity to control species).

Races are typically very tightly coupled with ethnicities, so you should also see the helpfile for ethnicities.

The syntax to use this command is as follows:

	#3race list#0 - lists all of the races
	#3race edit <which>#0 - begins editing a race
	#3race edit new <name> <bodyproto> [<parent race>]#0 - begins editing a new race
	#3race clone <old> <new>#0 - clones an existing race to a new one
	#3race close#0 - stops editing a race
	#3race show <which>#0 - views information about a race
	#3race show#0 - views information about your currently editing race

Additionally, there are numerous properties that can be set:

	#6Core Properties#0

	#3race set name <name>#0 - renames the race
	#3race set desc#0 - drops you into an editor to describe the race
	#3race set parent <race>#0 - sets a parent race for this race
	#3race set parent none#0 - clears a parent race from this race
	#3race set body <template>#0 - changes the body template of the race

	#6Chargen Properties#0

	#3race set chargen <prog>#0 - sets a prog that controls chargen availability
	#3race set advice <which>#0 - toggles a chargen advice applying to this race
	#3race set cost <resource> <amount>#0 - sets a cost for character creation
	#3race set require <resource> <amount>#0 - sets a non-cost requirement for character creation
	#3race set cost <resource> clear#0 - clears a resource cost for character creation	

	#6Combat Properties#0

	#3race set armour <type>#0 - sets the natural armour type for this race
	#3race set armour none#0 - clears the natural armour type
	#3race set armourquality <quality>#0 - sets the quality of the natural armour
	#3race set armourmaterial <material>#0 - sets the default material for the race's natural armour
	#3race set canattack#0 - toggles the race being able to use attacks
	#3race set candefend#0 - toggles the race being able to dodge/parry/block
	#3race set canuseweapons#0 - toggles the race being able to use weapons (if it has wielding parts)

	#6Physical Properties#0

	#3race set age <category> <minimum>#0 - sets the minimum age for a specified age category
	#3race set variable all <characteristic>#0 - adds or sets a specified characteristic for all genders
	#3race set variable male <characteristic>#0 - adds or sets a specified characteristic for males only
	#3race set variable female <characteristic>#0 - adds or sets a specified characteristic for females only
	#3race set variable remove <characteristic>#0 - removes a characteristic from this race
	#3race set variable promote <characteristic>#0 - pushes a characteristic up to the parent race
	#3race set variable demote <characteristic>#0 - pushes a characteristic down to all child races (and remove from this)
	#3race set attribute <which>#0 - toggles this race having the specified attribute
	#3race set attribute promote <which>#0 - pushes this attribute up to the parent race
	#3race set attribute demote <which>#0 - pushes this attribute down to all child races (and remove from this)
	#3race set roll <dice>#0 - the dice roll expression (#6xdy+z#0) for attributes for this race
	#3race set cap <number>#0 - the total cap on the sum of attributes for this race
	#3race set bonusprog <which>#0 - sets the prog that controls attribute bonuses
	#3race set corpse <model>#0 - changes the corpse model of the race
	#3race set health <model>#0 - changes the health mode of the race
	#3race set perception <%>#0 - sets the light-percetion multiplier of the race (higher is better)
	#3race set genders <list of genders>#0 - sets the allowable genders for this race
	#3race set butcher <profile>#0 - sets a butchery profile for this race
	#3race set butcher none#0 - clears a butchery profile from this race
	#3race set breathing nonbreather|simple|lung|gill|blowhole#0 - sets the breathing model
	#3race set breathingrate <volume per minute>#0 - sets the volume of breathing per minute
	#3race set holdbreath <seconds expression>#0 - sets the formula for breathe-holding length
	#3race set sweat <liquid>#0 - sets the race's sweat liquid
	#3race set sweat none#0 - disables sweating for this race
	#3race set sweatrate <volume per minute>#0 - sets the volume of sweating per minute
	#3race set blood <liquid>#0 - sets the race's blood liquid
	#3race set blood none#0 - disables bleeding for this race
	#3race set bloodmodel <model>#0 - sets the blood antigen typing model for this race
	#3race set bloodmodel none#0 - clears a blood antigen typing model from this race
	#3race set tempfloor <temperature>#0 - sets the base minimum tolerable temperature for this race
	#3race set tempceiling <temperature>#0 - sets the base maximum tolerable temperature for this race

	#6Eating Properties#0

	#3race set caneatcorpses#0 - toggles the race being able to eat corpses directly (without butchering)
	#3race set biteweight <weight>#0 - sets the amount of corpse weight eaten per bite
	#3race set material add <material>#0 - adds a material definition for corpse-eating
	#3race set material remove <material>#0 - removes a material as eligible for corpse-eating
	#3race set material alcohol|thirst|hunger|water|calories <amount>#0 - sets the per-kg nutrition for this material
	#3race set optinediblematerial#0 - toggles whether the race can only eat materials from the pre-defined list
	#3race set emotecorpse <emote>#0 - sets the emote for eating corpses. $0 is eater, $1 is corpse.
	#3race set yield#0 - tba

";

	[PlayerCommand("Race", "race")]
	[HelpInfo("Race", RaceHelp, AutoHelp.HelpArgOrNoArg)]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Race(ICharacter actor, string input)
	{
		GenericBuildingCommand(actor, new StringStack(input.RemoveFirstWord()), EditableItemHelper.RaceHelper);
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
		@"This command is used to make item skins, which can be applied to items to change their appearance. They can be manually added to items by admins, set to load that way through shops and created via crafts. Players with the correct permissions are allowed to create item skins of their own.

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
	#3item review all|mine|<builder name>|<id>#0 - opens the specified item skins for review and approval";

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
		@"The NPCSpawner command is used to view, create and edit NPC Spawners. NPC Spawners monitor zones for populations of NPCs and when they dip below target levels they load more in.

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
	#3npcspawner review list#0 - shows all NPC Spawner submitted for review";

	[PlayerCommand("NPCSpawner", "npcspawner", "spawner")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void NPCSpawner(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.NPCSpawnerHelper);
	}

	#endregion

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
	#6degree#0 - 0-5 for check success, 0 = marginal success, 5 = total success";

	[PlayerCommand("AmmunitionType", "ammunition", "ammunitiontype", "ammo", "ammotype")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("AmmunitionType", AmmunitionHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Ammunition(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()),
			EditableItemHelper.AmmunitionTypeHelper);
	}

	#endregion

	public const string WeaponTypeHelp =
		@"The WeaponType command is used to edit weapon types, which control the properties of a weapon. To use a weapon type on an item you need an item component, but the new/clone subcommands will automatically make one for you.

See also the closely related #6weaponattack#0 and #6traitexpression#0 commands.

You can use the following syntax with this command:

	#3weapontype list#0 - lists all weapon types
	#3weapontype edit <id|name>#0 - opens the specified weapon type for editing
	#3weapontype edit new <name>#0 - creates a new weapon type for editing
	#3weapontype edit#0 - equivalent of doing SHOW on your currently editing weapon type
	#3weapontype close#0 - closes the currently edited weapon type
	#3weapontype clone <id|name> <new name>#0 - creates a carbon copy of a weapon type for editing (including attacks)
	#3weapontype show <id|name>#0 - shows a particular weapon type.
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
}