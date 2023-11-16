using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Work.Crafts;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems;
#nullable enable
public class GameItemSkin : EditableItem, IGameItemSkin
{
	public GameItemSkin(Models.GameItemSkin item, IFuturemud gameworld) : base(item.EditableItem)
	{
		Gameworld = gameworld;
		_id = item.Id;
		RevisionNumber = item.RevisionNumber;
		_name = item.Name;
		_itemProtoId = item.ItemProtoId;
		CanUseSkinProg = Gameworld.FutureProgs.Get(item.CanUseSkinProgId ?? 0) ??
		                 throw new ApplicationException($"Could not load CanUseSkinProg for skin {_id}");
		ShortDescription = item.ShortDescription;
		ItemName = item.ItemName;
		FullDescription = item.FullDescription;
		LongDescription = item.LongDescription;
		Quality = (ItemQuality?)item.Quality;
		IsPublic = item.IsPublic;
	}

	public GameItemSkin(IAccount originator, IFuturemud gameworld, IGameItemProto proto, string name) : base(originator)
	{
		Gameworld = gameworld;
		_id = Gameworld.ItemSkins.NextID();
		_itemProtoId = proto.Id;
		_name = name;
		CanUseSkinProg = Gameworld.FutureProgs.Get(Gameworld.GetStaticLong("AlwaysTrueProg")) ??
		                 throw new ApplicationException($"Could not load CanUseSkinProg for skin {_id}");
		using (new FMDB())
		{
			var dbitem = new Models.GameItemSkin
			{
				Id = _id,
				RevisionNumber = RevisionNumber,
				Name = name,
				CanUseSkinProgId = CanUseSkinProg.Id,
				IsPublic = false,
				ItemProtoId = _itemProtoId
			};
			FMDB.Context.GameItemSkins.Add(dbitem);
			dbitem.EditableItem = new Models.EditableItem
			{
				BuilderAccountId = BuilderAccountID,
				BuilderDate = BuilderDate,
				RevisionStatus = (int)Status
			};
			FMDB.Context.EditableItems.Add(dbitem.EditableItem);
			FMDB.Context.SaveChanges();
		}
	}

	protected GameItemSkin(GameItemSkin rhs, IAccount originator) : base(originator)
	{
		Gameworld = rhs.Gameworld;
		_name = rhs.Name;
		_id = rhs.Id;
		RevisionNumber = Gameworld.ItemSkins.GetAll(_id).Max(x => x.RevisionNumber) + 1;
		_itemProtoId = rhs._itemProtoId;
		ItemName = rhs.ItemName;
		ShortDescription = rhs.ShortDescription;
		LongDescription = rhs.LongDescription;
		FullDescription = rhs.FullDescription;
		Quality = rhs.Quality;
		CanUseSkinProg = rhs.CanUseSkinProg;
		using (new FMDB())
		{
			var dbitem = new Models.GameItemSkin
			{
				Id = Id,
				RevisionNumber = RevisionNumber,
				Name = Name,
				CanUseSkinProgId = CanUseSkinProg.Id,
				IsPublic = IsPublic,
				ItemProtoId = _itemProtoId,
				ItemName = ItemName,
				ShortDescription = ShortDescription,
				LongDescription = LongDescription,
				FullDescription = FullDescription,
				Quality = (int?)Quality
			};
			FMDB.Context.GameItemSkins.Add(dbitem);
			dbitem.EditableItem = new Models.EditableItem
			{
				BuilderAccountId = BuilderAccountID,
				BuilderDate = BuilderDate,
				RevisionStatus = (int)Status
			};
			FMDB.Context.EditableItems.Add(dbitem.EditableItem);
			FMDB.Context.SaveChanges();
		}
	}

	protected GameItemSkin(GameItemSkin rhs, IAccount originator, string name) : base(originator)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		_itemProtoId = rhs._itemProtoId;
		ItemName = rhs.ItemName;
		ShortDescription = rhs.ShortDescription;
		LongDescription = rhs.LongDescription;
		FullDescription = rhs.FullDescription;
		Quality = rhs.Quality;
		CanUseSkinProg = rhs.CanUseSkinProg;
		_id = Gameworld.ItemSkins.NextID();
		RevisionNumber = 0;
		using (new FMDB())
		{
			var dbitem = new Models.GameItemSkin
			{
				Id = Id,
				RevisionNumber = RevisionNumber,
				Name = Name,
				CanUseSkinProgId = CanUseSkinProg?.Id,
				IsPublic = IsPublic,
				ItemProtoId = _itemProtoId,
				ItemName = ItemName,
				ShortDescription = ShortDescription,
				LongDescription = LongDescription,
				FullDescription = FullDescription,
				Quality = (int?)Quality
			};
			FMDB.Context.GameItemSkins.Add(dbitem);
			dbitem.EditableItem = new Models.EditableItem
			{
				BuilderAccountId = BuilderAccountID,
				BuilderDate = BuilderDate,
				RevisionStatus = (int)Status
			};
			FMDB.Context.EditableItems.Add(dbitem.EditableItem);
			FMDB.Context.SaveChanges();
		}
	}

	#region Overrides of FrameworkItem

	public override string FrameworkItemType => "GameItemSkin";

	#endregion

	#region Overrides of SavableKeywordedItem

	public override void Save()
	{
		var dbitem = FMDB.Context.GameItemSkins.Find(Id, RevisionNumber);
		dbitem.Name = Name;
		dbitem.ItemName = ItemName;
		dbitem.ShortDescription = ShortDescription;
		dbitem.LongDescription = LongDescription;
		dbitem.FullDescription = FullDescription;
		dbitem.Quality = (int?)Quality;
		dbitem.CanUseSkinProgId = CanUseSkinProg.Id;
		dbitem.IsPublic = IsPublic;
		base.Save(dbitem.EditableItem);
		Changed = false;
	}

	#endregion

	#region Overrides of EditableItem

	public override string EditHeader()
	{
		return $"Item Skin {Id:F0}r{RevisionNumber:F0} ({Name})";
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.ItemSkins.GetAll(Id);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return new GameItemSkin(this, initiator.Account);
	}

	public const string BuildingHelp = @"You can use the following options with this command:

	#3name <name>#0 - renames this skin
	#3public#0 - toggles this skin being public (admin only)
	#3prog <prog>#0 - sets the prog that controls use of this skin
	#3itemname <name>#0 - sets an override for the item's name
	#3itemname#0 - clears an override for the item's name
	#3sdesc <sdesc>#0 - sets an override for the item's short description
	#3sdesc#0 - clears an override for the item's short description
	#3ldesc <ldesc>#0 - sets an override for the item's long description
	#3ldesc#0 - clears an override for the item's long description
	#3desc#0 - drops you into an editor to enter an override description
	#3desc clear#0 - clears an override for the item's full description
	#3quality <quality>#0 - sets an overriding quality for the base item
	#3quality#0 - clears an override for the item's quality";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "itemname":
			case "iname":
				return BuildingCommandItemName(actor, command);
			case "itemsdesc":
			case "sdesc":
				return BuildingCommandSDesc(actor, command);
			case "itemldesc":
			case "ldesc":
				return BuildingCommandLDesc(actor, command);
			case "itemdesc":
			case "desc":
				return BuildingCommandFDesc(actor, command);
			case "quality":
				return BuildingCommandQuality(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "public":
				return BuildingCommandPublic(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this skin?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"You rename this skin to {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandItemName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			ItemName = null;
			Changed = true;
			actor.OutputHandler.Send("This skin will no longer override the item name.");
			return true;
		}

		ItemName = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send($"This skin will now override the item name to {ItemName.ColourName()}.");
		return true;
	}

	private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			ShortDescription = null;
			Changed = true;
			actor.OutputHandler.Send("This skin will no longer override the item short description.");
			return true;
		}

		ShortDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skin will now override the item short description to {ShortDescription.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandLDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			LongDescription = null;
			Changed = true;
			actor.OutputHandler.Send("This skin will no longer override the item long description.");
			return true;
		}

		LongDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skin will now override the item long description to {LongDescription.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandFDesc(ICharacter actor, StringStack command)
	{
		if (command.Peek().EqualToAny("clear", "reset", "none", "remove", "delete"))
		{
			FullDescription = null;
			Changed = true;
			actor.OutputHandler.Send("This skin will no longer override the item's full description.");
			return true;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"Base item description:\n\n{ItemProto.FullDescription.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t")}");
		sb.AppendLine();
		if (!string.IsNullOrEmpty(FullDescription))
		{
			sb.AppendLine("\nReplacing:\n");
			sb.AppendLine(FullDescription.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
		}

		sb.AppendLine("Enter the description in the editor below.");
		sb.AppendLine();
		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(PostAction, CancelAction, 1.0);
		return true;
	}

	private void CancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to enter a full description override for this skin.");
	}

	private void PostAction(string text, IOutputHandler handler, object[] args)
	{
		FullDescription = text.Fullstop().ProperSentences();
		Changed = true;
		handler.Send($"You update the full description override for this skin to:\n\n{FullDescription.Wrap(80, "\t")}");
	}

	private bool BuildingCommandQuality(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			Quality = null;
			Changed = true;
			actor.OutputHandler.Send("This skin will no longer override the item quality.");
			return true;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<ItemQuality>(out var quality))
		{
			actor.OutputHandler.Send(
				$"That is not a valid quality. See {"show qualities".MXPSend("show qualities")} to see a list.");
			return false;
		}

		Quality = quality;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skin will now override the item quality to {Quality.DescribeEnum().ColourName()}.");
		return true;
	}

	private bool BuildingCommandPublic(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators can set a skin as public or private.");
			return false;
		}

		IsPublic = !IsPublic;
		Changed = true;
		actor.OutputHandler.Send($"This item skin is {(IsPublic ? "now" : "no longer")} public.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new[]
			{
				new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character },
				new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character, FutureProgVariableTypes.Text }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CanUseSkinProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skin will now use the prog {CanUseSkinProg.MXPClickableFunctionName()} to determine who can use it.");
		return true;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Item Skin {Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} ({Name.ColourName()})");
		sb.AppendLine($"For Item: {ItemProto.EditHeader().ColourName()}");
		sb.AppendLine($"Public: {IsPublic.ToColouredString()}");
		sb.AppendLine($"Can Use Prog: {CanUseSkinProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Override Name: {ItemName?.ColourValue() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Override SDesc: {ShortDescription?.ColourValue() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Override LDesc: {LongDescription?.ColourValue() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Override Quality: {Quality?.Describe().ColourValue() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Override Desc: {FullDescription?.ColourCommand().LeadingConcatIfNotEmpty("\n\n").Wrap(actor.InnerLineFormatLength, "\t") ?? "None".Colour(Telnet.Red)}");
		return sb.ToString();
	}

	#endregion

	#region Implementation of IGameItemSkin

	public IGameItemSkin Clone(ICharacter author, string newName)
	{
		return new GameItemSkin(this, author.Account, newName);
	}

	private long _itemProtoId;
	public IGameItemProto ItemProto => Gameworld.ItemProtos.Get(_itemProtoId);
	public string? ItemName { get; private set; }
	public string? ShortDescription { get; private set; }
	public string? FullDescription { get; private set; }
	public string? LongDescription { get; private set; }
	public ItemQuality? Quality { get; private set; }
	public bool IsPublic { get; private set; }
	public IFutureProg CanUseSkinProg { get; private set; }

	public (bool Truth, string Error) CanUseSkin(ICharacter crafter, IGameItemProto? prototype)
	{
		if (prototype is not null && prototype.Id != _itemProtoId)
		{
			return (false,
				$"This skin can only be used with the {ItemProto.EditHeader().ColourIncludingReset(Telnet.Cyan)} item prototype.");
		}

		if (crafter.IsAdministrator())
		{
			return (true, string.Empty);
		}

		if (CanUseSkinProg?.Execute<bool?>(crafter) == false)
		{
			return (false, $"You are not permitted to use that skin.");
		}

		return (true, string.Empty);
	}

	public (bool Truth, string Error) CanUseSkin(ICharacter crafter, IGameItemProto? prototype, ICraft craft)
	{
		if (prototype is not null && prototype.Id != _itemProtoId)
		{
			return (false,
				$"This skin can only be used with the {ItemProto.EditHeader().ColourIncludingReset(Telnet.Cyan)} item prototype.");
		}

		if (crafter.IsAdministrator())
		{
			return (true, string.Empty);
		}

		if (CanUseSkinProg?.Execute<bool?>(crafter, craft.Name) == false)
		{
			if (CanUseSkinProg.Execute<bool?>(crafter) == false)
			{
				return (false, $"You are not permitted to use that skin.");
			}

			return (false, $"That skin cannot be used with that craft.");
		}

		return (true, string.Empty);
	}

	#endregion
}