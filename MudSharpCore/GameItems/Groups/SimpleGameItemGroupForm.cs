using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Groups;

public class SimpleGameItemGroupForm : GameItemGroupForm
{
	public SimpleGameItemGroupForm(IGameItemGroup parent) : base(parent)
	{
		ItemName = "item";
		RoomDescription = "a number of items are here.";
		Description = "This is a group of items. This is what you see when you look at it.";
		using (new FMDB())
		{
			var dbitem = new ItemGroupForm();
			FMDB.Context.ItemGroupForms.Add(dbitem);
			dbitem.ItemGroupId = parent.Id;
			dbitem.Definition = SaveToXml();
			dbitem.Type = "Simple";
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public SimpleGameItemGroupForm(ItemGroupForm form, IGameItemGroup parent) : base(form, parent)
	{
		LoadFromXML(XElement.Parse(form.Definition));
	}

	protected override string GameItemGroupFormType => "Simple";
	public string Description { get; set; }
	public string RoomDescription { get; set; }
	public string ItemName { get; set; }

	private void LoadFromXML(XElement definition)
	{
		var element = definition.Element("Description");
		if (element != null)
		{
			Description = element.Value;
		}

		element = definition.Element("RoomDescription");
		if (element != null)
		{
			RoomDescription = element.Value;
		}

		element = definition.Element("ItemName");
		if (element != null)
		{
			ItemName = element.Value;
		}

		element = definition.Element("Cells");
		if (element != null)
		{
			foreach (var item in element.Elements("Cell"))
			{
				Cells.Add(Gameworld.Cells.Get(long.Parse(item.Value)));
			}
		}
	}

	public override void BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What is it that you want to edit about this item group form?");
			return;
		}

		switch (command.Pop().ToLowerInvariant())
		{
			case "desc":
				BuildingCommandDesc(actor, command);
				return;
			case "rdesc":
			case "ldesc":
				BuildingCommandRDesc(actor, command);
				return;
			case "name":
			case "itemname":
			case "iname":
				BuildingCommandItemName(actor, command);
				return;
			default:
				base.BuildingCommand(actor, command);
				return;
		}
	}

	private void BuildingCommandDescPost(string text, IOutputHandler handler, params object[] args)
	{
		Description = text;
		Changed = true;
		handler.Send("You change the look description for that item group form.");
	}

	private static void BuildingCommandDescCancel(IOutputHandler handler, params object[] args)
	{
		handler.Send("You decide not to change the look description for that item group form after all.");
	}

	private void BuildingCommandDesc(ICharacter actor, StringStack command)
	{
		actor.Send("Enter the description that people will see when they look at this group below.");
		actor.EditorMode(BuildingCommandDescPost, BuildingCommandDescCancel, 1.0);
	}

	private void BuildingCommandRDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What description should people see in place of all the ldescs of the items?");
			return;
		}

		RoomDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.Send(
			"When there are items of this item group form present in a room, they will show up as follows:\n\t{0}",
			RoomDescription.ProperSentences().Fullstop().Colour(Telnet.Green));
	}

	private void BuildingCommandItemName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What item name should be the representative name for this item group form?");
			return;
		}

		ItemName = command.SafeRemainingArgument;
		actor.Send("That item group form now identifies its items as {0}.",
			ItemName.Pluralise().Colour(Telnet.Green));
		Changed = true;
	}

	public override string Show(IPerceiver voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLineFormat(voyeur, "{0} (Id {1:N0})", "Simple Form".Colour(Telnet.Cyan), Id);
		sb.AppendLine();
		sb.AppendLineFormat(voyeur, "This simple stack names its contents {0}",
			ItemName.Pluralise().Colour(Telnet.Green));
		sb.AppendLineFormat(voyeur, "It appears to the room as {0}",
			RoomDescription != null
				? RoomDescription.ProperSentences().Fullstop().Colour(Telnet.Green)
				: "not set".Colour(Telnet.Red));
		if (Cells.Any())
		{
			sb.AppendLine("This form only activates in the following cells:");
			foreach (var cell in Cells)
			{
				sb.AppendLineFormat("\tId {0}\t{1}", cell.Id, cell.HowSeen(voyeur));
			}
		}
		else
		{
			sb.AppendLine("This form will activate in any cell.");
		}

		sb.AppendLine();
		sb.AppendLine("When looked at, it shows the following description:");
		sb.AppendLine();
		sb.AppendLine((Description ?? "").Wrap(voyeur.InnerLineFormatLength, "\t"));
		return sb.ToString();
	}

	public override string Describe(IPerceiver voyeur, IEnumerable<IGameItem> items)
	{
		return items.Count() == 1 ? items.First().HowSeen(voyeur, true, DescriptionType.Long) : RoomDescription.SubstituteANSIColour();
	}

	public override string LookDescription(IPerceiver voyeur, IEnumerable<IGameItem> items)
	{
		var sb = new StringBuilder();
		sb.AppendLine(Description.SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLineFormat("Included {0}:", ItemName.Pluralise());
		foreach (var item in items)
		{
			sb.AppendLineFormat("\t{0}", item.HowSeen(voyeur, type: DescriptionType.Long));
		}

		return sb.ToString().Wrap(voyeur.InnerLineFormatLength);
	}

	#region Overrides of SaveableItem

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ItemGroupForms.Find(Id);
			dbitem.Definition =
				SaveToXml();
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	private string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Description", new XText(Description ?? "")),
			new XElement("RoomDescription", new XText(RoomDescription ?? "")),
			new XElement("ItemName", new XText(ItemName ?? "")),
			new XElement("Cells", from cell in Cells select new XElement("Cell", cell.Id))
		).ToString();
	}

	#endregion
}