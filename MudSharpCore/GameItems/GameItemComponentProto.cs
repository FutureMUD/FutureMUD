using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.GameItems;

public abstract class GameItemComponentProto : EditableItem, IGameItemComponentProto
{
	protected GameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto.EditableItem)
	{
		_noSave = true;
		Gameworld = gameworld;
		_id = proto.Id;
		_name = proto.Name;
		Description = proto.Description;
		if (proto.Definition.Length > 0)
		{
			LoadFromXml(XElement.Parse(proto.Definition));
		}

		_noSave = false;
	}

	protected GameItemComponentProto(IFuturemud gameworld, IAccount originator, string type)
		: base(originator)
	{
		Gameworld = gameworld;
		_id = Gameworld.ItemComponentProtos.NextID();
		_name = "Unnamed";
		Description = "Undefined " + type + " Component";

		using (new FMDB())
		{
			var dbproto = new Models.GameItemComponentProto
			{
				Id = Id,
				RevisionNumber = RevisionNumber,
				EditableItem = new Models.EditableItem
				{
					BuilderAccountId = BuilderAccountID,
					BuilderDate = BuilderDate,
					RevisionStatus = (int)Status
				},
				Description = Description,
				Name = Name,
				Type = type,
				Definition = "<Definition></Definition>"
			};
			FMDB.Context.GameItemComponentProtos.Add(dbproto);
			FMDB.Context.EditableItems.Add(dbproto.EditableItem);
			FMDB.Context.SaveChanges();
		}

		Changed = true;
	}

	public override string FrameworkItemType => "GameItemComponentProto";

	#region IGameItemComponentProto Members

	/// <summary>
	/// Whether or not we should warn the purger before purging this item
	/// </summary>
	public virtual bool WarnBeforePurge => false;

	public string Description { get; protected set; }

	#endregion

	protected sealed override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.ItemComponentProtos.GetAll(Id);
	}

	public sealed override string EditHeader()
	{
		return
			$"{TypeDescription.SplitCamelCase().ColourName()}: [{Name.Proper().ColourValue()}] (#{Id}r{RevisionNumber})";
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"{string.Format($"{TypeDescription} Prototype #{Id} - Revision {RevisionNumber}", TypeDescription, Id, RevisionNumber),-60}{string.Format($"Status: {Status.Describe()}", Status.Describe()),50}");
		sb.AppendLine($"Name: {Name.Proper()}");
		sb.AppendLine($"Description: {Description.Wrap(actor.InnerLineFormatLength)}");
		sb.AppendLine();
		sb.AppendLine(ComponentDescriptionOLC(actor));
		return sb.ToString();
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Last)
		{
			case "desc":
			case "description":
				return BuildingCommand_Description(actor, command);
			case "name":
				return BuildingCommand_Name(actor, command);
			case "keywords":
				return BuildingCommand_Keywords(actor, command);
			case "help":
			case "?":
				return BuildingCommand_Help(actor);
			default:
				actor.OutputHandler.Send("That is not a valid option for editing this component.");
				return false;
		}
	}

	public virtual string ShowBuildingHelp => "This component does not yet have specific help.";

	public abstract string TypeDescription { get; }
	public override bool ReadOnly => false;
	public virtual bool PreventManualLoad => false;

	public abstract string ComponentDescriptionOLC(ICharacter actor);
	public abstract IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false);
	public abstract IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent);

	private bool BuildingCommand_Description(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the description of this item component to?");
			return false;
		}

		Description = command.SafeRemainingArgument.Proper();
		actor.OutputHandler.Send("You change the description of this component to: " + Description);
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Name(IPerceivable actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the name of this item component to?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase();

		var existing = Gameworld.ItemComponentProtos.GetByName(_name);
		if (existing is not null && existing.Id != Id)
		{
			actor.OutputHandler.Send(
				$"There is already a component with the name {_name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send("You change the name of this component to: " + Name);
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Keywords(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What keywords do you want to give to this item component?");
			return false;
		}

		_keywords = new Lazy<List<string>>(() => new List<string>(command.SafeRemainingArgument.Split(' ')));
		actor.OutputHandler.Send("You set the following keywords: " + Keywords.ListToString());
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Help(ICharacter actor)
	{
		actor.OutputHandler.Send(ShowBuildingHelp);
		return true;
	}

	protected abstract void LoadFromXml(XElement root);
	protected abstract string SaveToXml();

	protected IEditableRevisableItem CreateNewRevision(ICharacter initiator,
		Func<MudSharp.Models.GameItemComponentProto, IFuturemud, IEditableRevisableItem> function)
	{
		using (new FMDB())
		{
			var dbnew = new Models.GameItemComponentProto
			{
				Id = Id,
				RevisionNumber = FMDB.Context.GameItemComponentProtos.Where(x => x.Id == Id)
				                     .Select(x => x.RevisionNumber)
				                     .AsEnumerable()
				                     .DefaultIfEmpty(0)
				                     .Max() + 1,
				EditableItem = new Models.EditableItem()
			};
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
			dbnew.EditableItem.BuilderAccountId = initiator.Account.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;
			dbnew.Description = Description;
			dbnew.Name = Name.Proper();
			dbnew.Definition = SaveToXml();
			dbnew.Type = TypeDescription;
			FMDB.Context.GameItemComponentProtos.Add(dbnew);
			FMDB.Context.SaveChanges();

			return function(dbnew, Gameworld);
		}
	}

	#region ISaveable Members

	public override void Save()
	{
		using (new FMDB())
		{
			var dbproto = FMDB.Context.GameItemComponentProtos.Find(Id, RevisionNumber);
			dbproto.Name = Name.Proper();
			dbproto.Description = Description;
			if (_statusChanged)
			{
				base.Save(dbproto.EditableItem);
			}

			dbproto.Definition = SaveToXml();
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion
}