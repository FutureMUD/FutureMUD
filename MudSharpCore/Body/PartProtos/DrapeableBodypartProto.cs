using System;
using System.Linq;
using System.Text;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Body.PartProtos;

public class DrapeableBodypartProto : ExternalBodypartProto, IWear
{
	public DrapeableBodypartProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
		DisplayOrder = proto.DisplayOrder ?? 0;
	}

	public DrapeableBodypartProto(DrapeableBodypartProto rhs, string newName) : base(rhs, newName)
	{
		DisplayOrder = rhs.DisplayOrder;
	}

	public override IBodypart Clone(string newName)
	{
		return new DrapeableBodypartProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Drapeable;

	protected override void InternalSave(BodypartProto dbitem)
	{
		dbitem.DisplayOrder = DisplayOrder;
	}

	protected override string HelpInfo => $"{base.HelpInfo}\n\tdisplay <number> - changes the display order";

	public override bool BuildingCommand(ICharacter builder, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "display":
				return BuildingCommandDisplay(builder, command);
		}

		return base.BuildingCommand(builder, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
	}

	private bool BuildingCommandDisplay(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which order should this bodypart display in inventory commands?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value))
		{
			builder.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		DisplayOrder = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart will now display at position {DisplayOrder.ToString("N0", builder).ColourValue()} in inventory commands.");
		return true;
		throw new NotImplementedException();
	}

	public override string ShowToBuilder(ICharacter builder)
	{
		var sb = new StringBuilder(base.ShowToBuilder(builder));
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Display Order: {DisplayOrder.ToString("N0", builder).ColourValue()}",
			"",
			""
		);
		return sb.ToString();
	}

	public override string FrameworkItemType => "DrapeableBodypartProto";


	public WearableItemCoverStatus HowCovered(IGameItem item, IBody body)
	{
		return CoverInformation(item, body).Item1;
	}

	public Tuple<WearableItemCoverStatus, IGameItem> CoverInformation(IGameItem item, IBody body)
	{
		var wornItems = body.WornItemsProfilesFor(this).ToList();

		// Check the item even exists on this location
		if (wornItems.All(x => x.Item1 != item))
		{
			return new Tuple<WearableItemCoverStatus, IGameItem>(WearableItemCoverStatus.NoCoverInformation, null);
		}

		// The DrapedItems list is ordered from oldest to newest - items further in the list are "over the top" of other items. To take everything over the top
		// of a given item, we'll reverse the list, then takewhile.
		var coverItems = wornItems.AsEnumerable().Reverse().TakeWhile(x => x.Item1 != item).ToList();
		if (!coverItems.Any()) // No items, not covered
		{
			return new Tuple<WearableItemCoverStatus, IGameItem>(WearableItemCoverStatus.Uncovered, null);
		}

		if (coverItems.All(x => x.Item2.Transparent))
		{
			return new Tuple<WearableItemCoverStatus, IGameItem>(WearableItemCoverStatus.TransparentlyCovered,
				coverItems.LastOrDefault().Item1);
		}

		return new Tuple<WearableItemCoverStatus, IGameItem>(WearableItemCoverStatus.Covered,
			coverItems.First().Item1);
	}

	public bool CanWear(IGameItem item, IInventory body)
	{
		return true;
	}

	public bool CanRemove(IGameItem item, IInventory body)
	{
		return
			!body.WornItemsProfilesFor(this)
			     .Reverse()
			     .TakeWhile(x => x.Item1 != item)
			     .Any(x => x.Item2.PreventsRemoval);
	}
}