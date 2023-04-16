using System;
using System.Linq;
using System.Text;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.PartProtos;

public class GrabbingWieldingBodypartProto : GrabbingBodypartProto, IWield, IWear
{
	public GrabbingWieldingBodypartProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
		MaxSingleSize = Enum.IsDefined(typeof(SizeCategory), proto.MaxSingleSize ?? 0)
			? (SizeCategory)(proto.MaxSingleSize ?? 0)
			: SizeCategory.Normal;
	}

	public GrabbingWieldingBodypartProto(GrabbingWieldingBodypartProto rhs, string newName) : base(rhs, newName)
	{
		MaxSingleSize = rhs.MaxSingleSize;
	}

	public override IBodypart Clone(string newName)
	{
		return new GrabbingWieldingBodypartProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.GrabbingWielding;

	protected override void InternalSave(BodypartProto dbitem)
	{
		base.InternalSave(dbitem);
		dbitem.MaxSingleSize = (int)MaxSingleSize;
	}

	public override string ShowToBuilder(ICharacter builder)
	{
		var sb = new StringBuilder(ShowToBuilderSkipThis(builder));
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Display Order: {DisplayOrder.ToString("N0", builder).ColourValue()}",
			$"Unary {Unary.ToColouredString()}",
			$"Max Single Wield: {MaxSingleSize.Describe().ColourValue()}"
		);
		return sb.ToString();
	}

	public SizeCategory MaxSingleSize { get; protected set; }

	protected override string HelpInfo =>
		$"{base.HelpInfo}\n\twieldsize <size> - sets the maximum size that can be wielded one handed";

	public override bool BuildingCommand(ICharacter builder, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "wieldsize":
				return BuildingCommandWieldSize(builder, command);
		}

		return base.BuildingCommand(builder, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
	}

	private bool BuildingCommandWieldSize(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("What is the maximum size that this bodypart can wield one handed?");
			return false;
		}

		if (!GameItemEnumExtensions.TryParseSize(command.PopSpeech(), out var size))
		{
			builder.OutputHandler.Send("That is not a valid size.");
			return false;
		}

		MaxSingleSize = size;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart can now wield anything {MaxSingleSize.Describe().ColourValue()} or smaller in one hand.");
		return true;
	}

	public bool SelfUnwielder()
	{
		return true;
	}

	public IWieldItemWieldResult CanWield(IGameItem item, IInventory body)
	{
		var wieldable = item.GetItemType<IWieldable>();
		if (wieldable == null)
		{
			return IWieldItemWieldResult.NotWieldable;
		}

		if (body.WieldedItemsFor(this).Any())
		{
			return IWieldItemWieldResult.AlreadyWielding;
		}

		switch (body.CanUseBodypart(this))
		{
			case CanUseBodypartResult.CantUseLimbDamage:
			case CanUseBodypartResult.CantUseLimbPain:
			case CanUseBodypartResult.CantUsePartDamage:
			case CanUseBodypartResult.CantUsePartPain:
			case CanUseBodypartResult.CantUseSevered:
			case CanUseBodypartResult.CantUseNonFunctionalProsthetic:
			case CanUseBodypartResult.CantUseLimbGrappled:
			case CanUseBodypartResult.CantUseMissingBone:
			case CanUseBodypartResult.CantUseSpinalDamage:
				return IWieldItemWieldResult.TooDamaged;
		}

		if (body.HeldItemsFor(this).Any(x => x != item))
		{
			return IWieldItemWieldResult.GrabbingWielderHoldOtherItem;
		}

		return IWieldItemWieldResult.Success;
	}

	public IWieldItemUnwieldResult CanUnwield(IGameItem item, IInventory body)
	{
		return body.WieldedItemsFor(this).Contains(item)
			? IWieldItemUnwieldResult.Success
			: IWieldItemUnwieldResult.NotWielding;
	}

	public int Hands(IGameItem item)
	{
		return item.GetItemType<IWieldable>()?.AlwaysRequiresTwoHandsToWield == true || item.Size > MaxSingleSize
			? 2
			: 1;
	}

	#region Overrides of BodypartPrototype

	public override bool PartDamageEffects(IBody owner, CanUseBodypartResult why)
	{
		if (why == CanUseBodypartResult.CanUse)
		{
			return false;
		}

		var result = base.PartDamageEffects(owner, why);

		var items = owner.WieldedItemsFor(this).Concat(owner.HeldItemsFor(this)).ToList();
		if (!items.Any())
		{
			return result;
		}

		var limb = owner.GetLimbFor(this);

		foreach (var item in items)
		{
			switch (why)
			{
				case CanUseBodypartResult.CantUseLimbDamage:
					owner.Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {limb.Name.ToLowerInvariant()} is too badly damaged.",
								owner.Actor, owner.Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseBodypartResult.CantUseLimbPain:
					owner.Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {limb.Name.ToLowerInvariant()} is in too much pain.",
								owner.Actor, owner.Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseBodypartResult.CantUseLimbGrappled:
					owner.Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {limb.Name.ToLowerInvariant()} is being grappled.",
								owner.Actor, owner.Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseBodypartResult.CantUsePartDamage:
					owner.Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {FullDescription().ToLowerInvariant()} is too badly damaged.",
								owner.Actor, owner.Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseBodypartResult.CantUsePartPain:
					owner.Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {FullDescription().ToLowerInvariant()} is in too much pain.",
								owner.Actor, owner.Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseBodypartResult.CantUseSevered:
					break;
			}

			if (owner.Actor.Combat != null)
			{
				owner.Actor.AddEffect(new CombatGetItemEffect(owner.Actor, item));
				item.AddEffect(new CombatNoGetEffect(item, owner.Actor.Combat), TimeSpan.FromSeconds(20));
			}

			owner.Drop(item, silent: true);
		}

		return true;
	}

	#endregion

	#region Implementation of IWear

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

	#endregion
}