﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Commands.Modules;

internal abstract class BaseBuilderModule : Module<ICharacter>
{
	protected BaseBuilderModule(string name)
		: base(name)
	{
		IsNecessary = true;
	}
		
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
		IEditableRevisableItem proto = null;
		if (input.IsFinished)
		{
			proto = helper.GetAllEditableItems(character).GetByIdOrNameRevisableForEditing(cmd);
		}
		else if (long.TryParse(cmd, out var id))
		{
			if (!int.TryParse(input.SafeRemainingArgument, out var revision))
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
		else
		{
			character.OutputHandler.Send("You must either enter a name or ID, or an ID and a revision number.");
			return;
		}

		if (proto == null)
		{
			character.Send("There is no such {0} for you to edit.", helper.ItemName);
			return;
		}

		if (proto.ReadOnly)
		{
			character.Send($"You cannot create a new revision or edit the properties of that {helper.ItemName} because it is read-only.");
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

		character.OutputHandler.Send($"You open {helper.GetEditableItemFunc(character).EditHeader()} for editing.");
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
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
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
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
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
	
	protected const string GenericReviewableSearchList = @"The full list of search filters is as follows:

	#6all#0 - includes obsolete and non-current revisions
	#6mine#0 - only shows things you personally created
	#6by <account>#0 - only shows things the nominated account created
	#6reviewed <account>#0 - only shows things the nominated account has approved
	#6+<keyword>#0 - only shows things with the nominated keyword
	#6-<keyword>#0 - excludes things with the nominated keyword";

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
		if (input.IsFinished)
		{
			if (helper.GetEditableItemFunc(character) != null)
			{
				character.OutputHandler.Send(helper.GetEditableItemFunc(character).Show(character));
				return;
			}

			character.OutputHandler.Send($"Which {helper.ItemName} do you want to show?");
			return;
		}

		var cmd = input.PopSpeech();
		IEditableRevisableItem proto = null;
		if (input.IsFinished)
		{
			proto = helper.GetAllEditableItems(character).GetByIdOrNameRevisable(cmd);
		}
		else if (long.TryParse(cmd, out var id))
		{
			if (!int.TryParse(input.SafeRemainingArgument, out var revision))
			{
				character.OutputHandler.Send(
					"You must either enter just an ID to view the most recent revision, or specify a numerical revision number.");
				return;
			}

			proto = helper.GetEditableItemByIdRevNumFunc(character, id, revision);
		}
		else
		{
			character.OutputHandler.Send("You must either enter just an ID to view the most recent revision, or specify a numerical revision number.");
			return;
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
			case "open":
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
			case "new":
				GenericRevisableEditNew(actor, input, helper);
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
				sb.AppendLine($"You are currently editing {helper.GetEditHeader(helper.GetEditableItemFunc(character))}");
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

		var proto = helper.GetEditableItemByIdOrNameFunc(character, cmd);

		if (proto == null)
		{
			character.Send($"The text {cmd.ColourCommand()} is not a valid {helper.ItemName} for you to edit.");
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
		var cmd = input.PopSpeech();
		if (cmd.Length == 0)
		{
			if (helper.GetEditableItemFunc(character) != null)
			{
				var sb = new StringBuilder();
				sb.AppendLine($"You are currently editing {helper.GetEditHeader(helper.GetEditableItemFunc(character))}");
				sb.AppendLine();
				sb.Append(helper.GetEditableItemFunc(character).Show(character));
				character.OutputHandler.Send(sb.ToString());
				return;
			}

			character.OutputHandler.Send($"Which {helper.ItemName} do you want to show?");
			return;
		}

		var proto = helper.GetEditableItemByIdOrNameFunc(character, cmd);

		if (proto == null)
		{
			character.OutputHandler.Send($"The text {cmd.ColourCommand()} is not a valid {helper.ItemName}.");
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
		if (helper is null)
		{
			actor.OutputHandler.Send("This editable type does not have a building command module set up.");
			return;
		}

		switch (input.PopSpeech().ToLowerInvariant())
		{
			case "edit":
			case "open":
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
			case "new":
				GenericEditNew(actor, input, helper);
				return;
			default:
				actor.OutputHandler.Send(helper.DefaultCommandHelp.SubstituteANSIColour());
				return;
		}
	}

	#endregion

	#region Common Item Building
	protected static void Item_Load(ICharacter actor, StringStack input)
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

		if (quantity > 1)
		{
			if (proto.IsItemType<StackableGameItemComponentProto>())
			{
				var item = proto.CreateNew(actor, skin, quantity, input.SafeRemainingArgument).First();
				actor.Gameworld.Add(item);

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
				item.Login();
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ load|loads $0.", actor, item),
						flags: OutputFlags.SuppressObscured));
			}
			else
			{
				if (quantity > 10)
				{
					actor.OutputHandler.Send("The most non-stackable items you can load at one time is 10.");
					return;
				}
				var items = proto.CreateNew(actor, skin, quantity, input.SafeRemainingArgument).ToList();
				foreach(var item in items)
				{
					actor.Gameworld.Add(item);
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
					item.Login();
				}

				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ load|loads $0", actor, new PerceivableGroup(items)),
						flags: OutputFlags.SuppressObscured));
			}
		}
		else
		{
			var item = proto.CreateNew(actor, skin, 1, input.SafeRemainingArgument).First();
			actor.Gameworld.Add(item);

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
			item.Login();
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ load|loads $0.", actor, item),
					flags: OutputFlags.SuppressObscured));
		}
	}
	#endregion
}