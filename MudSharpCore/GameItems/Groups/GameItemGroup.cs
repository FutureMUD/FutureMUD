using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Groups;

public class GameItemGroup : SavableKeywordedItem, IGameItemGroup
{
	private readonly List<IGameItemGroupForm> _forms = new();

	public GameItemGroup(ItemGroup group, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = group.Id;
		_name = group.Name;
		_keywords = new Lazy<List<string>>(() => new List<string>(group.Keywords.Split(' ')));
		foreach (var form in group.ItemGroupForms)
		{
			_forms.Add(GameItemGroupFactory.CreateGameItemGroupForm(form, this, gameworld));
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ItemGroups.Find(Id);
			dbitem.Name = Name;
			dbitem.Keywords = Keywords.ListToString(separator: " ", conjunction: "");
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public void Delete()
	{
		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			Gameworld.Destroy(this);
			OnDelete?.Invoke(this, EventArgs.Empty);
			var dbitem = FMDB.Context.ItemGroups.Find(Id);
			if (dbitem != null)
			{
				FMDB.Context.ItemGroups.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	public override string FrameworkItemType => "GameItemGroup";

	#region IGameItemGroup Members

	public IEnumerable<IGameItemGroupForm> Forms => _forms;

	public bool CannotBeDeleted => Gameworld.GetStaticLong("TooManyItemsGameItemGroup") == Id;

	public string Describe(IPerceiver voyeur, IEnumerable<IGameItem> items, ICell cell)
	{
		var form = Forms.FirstOrDefault(x => x.Applies(cell));
		if (form == null)
		{
			var sb = new StringBuilder();
			foreach (var item in items)
			{
				sb.AppendLine(item.HowSeen(voyeur, true, DescriptionType.Long));
			}

			return sb.ToString();
		}

		return form.Describe(voyeur, items);
	}

	public string Show(IPerceiver voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLineFormat(voyeur, "{0} Name {2} (Id #{1:N0})", "Item Group".Colour(Telnet.Cyan), Id,
			Name.Colour(Telnet.Green));
		sb.AppendLineFormat(voyeur, "Keywords: {0}", Keywords.ListToString(separator: " ", conjunction: ""));
		if (!_forms.Any())
		{
			sb.AppendLine();
			sb.AppendLineFormat(voyeur, "This group does not yet have any forms. Use {0} to add one.",
				string.Format(voyeur, "{0}{1:N0}{2}".Colour(Telnet.Yellow), "itemgroup ", Id, " form add <type>"));
		}
		else
		{
			foreach (var form in _forms)
			{
				sb.AppendLine();
				sb.Append(form.Show(voyeur));
			}
		}

		sb.AppendLine();
		sb.AppendLine($"Items using this group:");
		sb.AppendLine();
		var items = Gameworld.ItemProtos
		                     .Where(x => x.ItemGroup == this)
		                     .OrderByDescending(x => x.Status == RevisionStatus.Current)
		                     .ThenByDescending(x => x.RevisionNumber)
		                     .DistinctBy(x => x.Id)
		                     .OrderBy(x => x.Id)
		                     .ToList();
		if (items.Any())
		{
			foreach (var item in items)
			{
				sb.AppendLine($"\t{item.EditHeader().ColourObject()}");
			}
		}
		else
		{
			sb.AppendLine("\tNone");
		}

		return sb.ToString();
	}

	public void BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"Do you want to set the name of this group, add a form, delete a form, or set a property of a form?");
			return;
		}

		switch (command.Pop().ToLowerInvariant())
		{
			case "name":
				BuildingCommandName(actor, command);
				return;
			case "keywords":
				BuildingCommandKeywords(actor, command);
				return;
			case "form":
				BuildingCommandForm(actor, command);
				return;
			default:
				actor.OutputHandler.Send(@"The valid options for this command are as follows:

	#3name <name>#0 - renames this item group
	#3keywords <space separated list>#0 - sets the item group keywords
	#3form add <type>#0 - adds a new description form for this group
	#3form delete <id>#0 - deletes the specified form from this group
	#3form <id> room <id>#0 - toggles a room as using or not the form
	#3form <id> ldesc <ldesc>#0 - sets the long description (i.e. in room) of the form
	#3form <id> name <item name>#0 - how items in this item group are referred to
	#3form <id> desc#0 - drops into an editor to write a look description for the form".SubstituteANSIColour());
				return;
		}
	}

	private void BuildingCommandKeywords(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What keywords do you want this item group to have?");
			return;
		}

		_keywords = new Lazy<List<string>>(() => new List<string>
		(
			command.SafeRemainingArgument.Split(' ').Select(x => x.ToLowerInvariant()).Distinct()
		));

		actor.Send("That item group now has the following keywords: {0}",
			command.SafeRemainingArgument.Split(' ')
			       .Select(x => x.ToLowerInvariant())
			       .Distinct()
			       .ListToString(separator: " ", conjunction: "")
			       .Colour(Telnet.Green));
		Changed = true;
	}

	private void BuildingCommandForm(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to add or delete a form, or edit a specific form?");
			return;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				BuildingCommandFormAdd(actor, command);
				return;
			case "delete":
			case "del":
			case "remove":
			case "rem":
				BuildingCommandFormDelete(actor, command);
				return;
		}


		if (!long.TryParse(command.Last, out var value))
		{
			actor.Send("What is the ID of the form you wish to edit?");
			return;
		}

		var form = _forms.FirstOrDefault(x => x.Id == value);
		if (form == null)
		{
			actor.Send("There is no such form on this item group.");
			return;
		}

		form.BuildingCommand(actor, command);
	}

	private void BuildingCommandFormAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What type of form do you want to add to this item group?");
			return;
		}

		var newForm = GameItemGroupFactory.CreateGameItemGroupForm(this, command.Pop(), Gameworld);
		if (newForm == null)
		{
			actor.Send("There is no such item group form type.");
			return;
		}

		_forms.Add(newForm);
		actor.Send("You create a new form for this item group with ID {0:N0} of type {1}", newForm.Id,
			command.Last.Proper().Colour(Telnet.Green));
	}

	private void BuildingCommandFormDelete(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which form do you want to delete from this item group?");
			return;
		}

		if (!long.TryParse(command.Pop(), out var value))
		{
			actor.Send("Which form do you want to delete from this item group?");
			return;
		}

		var form = _forms.FirstOrDefault(x => x.Id == value);
		if (form == null)
		{
			actor.Send("This item group does not have such a form to delete.");
			return;
		}

		actor.AddEffect(new Accept(actor, new GameItemGroupFormDeletionProposal(actor, this, form)),
			TimeSpan.FromSeconds(120));
		actor.Send(
			"You are proposing to permanently delete form {0:N0} from item group {1:N0}. This cannot be undone. Use {2} to proceed with the deletion.",
			form.Id, Id, "accept delete".Colour(Telnet.Yellow));
	}

	private void BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What name do you want to give to this item group?");
			return;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (
			actor.Gameworld.ItemGroups.Except(this)
			     .Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already an item group with that name. Item group names must be unique.");
			return;
		}

		_name = name;
		actor.OutputHandler.Send(
			$"The item group with ID {Id.ToString("N0", actor)} is now called {_name.ColourName()}.");
		Changed = true;
	}

	public string LookDescription(IPerceiver voyeur, IEnumerable<IGameItem> items, ICell cell)
	{
		var form = Forms.FirstOrDefault(x => x.Applies(cell));
		if (form == null)
		{
			var sb = new StringBuilder();
			foreach (var item in items)
			{
				sb.AppendLine(item.HowSeen(voyeur, true, DescriptionType.Long));
			}

			return sb.ToString();
		}

		return form.LookDescription(voyeur, items);
	}

	public void RemoveForm(IGameItemGroupForm form)
	{
		// No need to set changed, the deletion is handled in the database
		_forms.Remove(form);
	}

	public event EventHandler OnDelete;

	#endregion
}