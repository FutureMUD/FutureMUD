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

public class WieldingBodypartProto : ExternalBodypartProto, IWield
{
	public WieldingBodypartProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
		MaxSingleSize = Enum.IsDefined(typeof(SizeCategory), proto.MaxSingleSize ?? 0)
			? (SizeCategory)(proto.MaxSingleSize ?? 0)
			: SizeCategory.Normal;
	}

	public WieldingBodypartProto(WieldingBodypartProto rhs, string newName) : base(rhs, newName)
	{
	}

	public override IBodypart Clone(string newName)
	{
		return new WieldingBodypartProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Wielding;

	protected override void InternalSave(BodypartProto dbitem)
	{
		dbitem.MaxSingleSize = (int)MaxSingleSize;
	}

	public override string FrameworkItemType => "WieldingBodypartProto";

	public SizeCategory MaxSingleSize { get; protected set; }

	public override string ShowToBuilder(ICharacter builder)
	{
		var sb = new StringBuilder(base.ShowToBuilder(builder));
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Display Order: {DisplayOrder.ToString("N0", builder).ColourValue()}",
			$"Max Single Wield: {MaxSingleSize.Describe().ColourValue()}",
			""
		);
		return sb.ToString();
	}

	protected override string HelpInfo =>
		$"{base.HelpInfo}\n\tdisplay <number> - changes the display order\n\twieldsize <size> - sets the maximum size that can be wielded one handed";

	public override bool BuildingCommand(ICharacter builder, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "wieldsize":
				return BuildingCommandWieldSize(builder, command);
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
		return false;
	}

	public IWieldItemWieldResult CanWield(IGameItem item, IInventory body)
	{
		var wieldable = item.GetItemType<IWieldable>();
		if (wieldable == null)
		{
			return IWieldItemWieldResult.NotWieldable;
		}

		if (!body.WieldedItemsFor(this).Any())
		{
			return IWieldItemWieldResult.AlreadyWielding;
		}

		return IWieldItemWieldResult.Success;
	}

	public IWieldItemUnwieldResult CanUnwield(IGameItem item, IInventory body)
	{
		if (body.WieldedItemsFor(this).SingleOrDefault() == item)
		{
			return IWieldItemUnwieldResult.Success;
		}

		return IWieldItemUnwieldResult.NotWielding;
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

		var wieldedItems = owner.WieldedItemsFor(this).ToList();
		if (!wieldedItems.Any())
		{
			return result;
		}

		var limb = owner.GetLimbFor(this);

		foreach (var item in wieldedItems)
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
				case CanUseBodypartResult.CantUseSpinalDamage:
					owner.Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because #0 can no longer move &0's {FullDescription().ToLowerInvariant()}.",
								owner.Actor, owner.Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseBodypartResult.CantUseSevered:
					break;
			}

			owner.Drop(item, silent: true);
		}

		return true;
	}

	#endregion
}