using System;
using System.Linq;
using System.Text;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.PartProtos;

public class GrabbingBodypartProto : ExternalBodypartProto, IGrab
{
	public GrabbingBodypartProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
		Unary = proto.Unary ?? true;
		DisplayOrder = proto.DisplayOrder ?? 0;
	}

	public GrabbingBodypartProto(GrabbingBodypartProto rhs, string newName) : base(rhs, newName)
	{
		Unary = rhs.Unary;
		DisplayOrder = rhs.DisplayOrder;
	}

	public override IBodypart Clone(string newName)
	{
		return new GrabbingBodypartProto(this, newName);
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Grabbing;

	protected override void InternalSave(BodypartProto dbitem)
	{
		dbitem.Unary = Unary;
		dbitem.DisplayOrder = DisplayOrder;
	}

	public bool Unary { get; protected set; }

	public override string ShowToBuilder(ICharacter builder)
	{
		var sb = new StringBuilder(base.ShowToBuilder(builder));
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Display Order: {DisplayOrder.ToString("N0", builder).ColourValue()}",
			$"Unary: {Unary.ToColouredString()}",
			""
		);
		return sb.ToString();
	}

	protected string ShowToBuilderSkipThis(ICharacter builder)
	{
		return base.ShowToBuilder(builder);
	}

	protected override string HelpInfo =>
		$"{base.HelpInfo}\n\tdisplay <number> - changes the display order\n\tunary - toggles unitary mode (e.g. hands vs inventory)";

	public override bool BuildingCommand(ICharacter builder, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "display":
				return BuildingCommandDisplay(builder, command);
			case "unary":
				return BuildingCommandUnary(builder, command);
		}

		return base.BuildingCommand(builder, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
	}

	private bool BuildingCommandUnary(ICharacter builder, StringStack command)
	{
		Unary = !Unary;
		Changed = true;
		if (Unary)
		{
			builder.OutputHandler.Send(
				"This bodypart is now unary - i.e. it can only hold a single item at a time, like 'hands' mode.");
		}
		else
		{
			builder.OutputHandler.Send(
				"This bodypart is now non-unary, i.e. it behaves like an inventory that can have multipler items.");
		}

		return true;
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

	public bool ItemsVisible()
	{
		return Unary;
	}

	public WearlocGrabResult CanGrab(IGameItem item, IInventory body)
	{
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
				return WearlocGrabResult.FailDamaged;
		}

		if (Unary && (body.HeldItemsFor(this).Any() || body.WieldedItemsFor(this).Any()))
		{
			return WearlocGrabResult.FailFull;
		}

		return WearlocGrabResult.Success;
	}

	public WearlocDropResult CanDrop(IGameItem item, IInventory body)
	{
		return WearlocDropResult.Success;
	}

	#region Overrides of BodypartPrototype

	public override bool PartDamageEffects(IBody owner, CanUseBodypartResult why)
	{
		if (why == CanUseBodypartResult.CanUse)
		{
			return false;
		}

		var result = base.PartDamageEffects(owner, why);

		var items = owner.HeldItemsFor(this).ToList();
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
								owner.Actor, owner.Actor, item)));
					break;
				case CanUseBodypartResult.CantUseLimbPain:
					owner.Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {limb.Name.ToLowerInvariant()} is in too much pain.",
								owner.Actor, owner.Actor, item)));
					break;
				case CanUseBodypartResult.CantUseLimbGrappled:
					owner.Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {limb.Name.ToLowerInvariant()} is being grappled.",
								owner.Actor, owner.Actor, item)));
					break;
				case CanUseBodypartResult.CantUsePartDamage:
					owner.Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {FullDescription().ToLowerInvariant()} is too badly damaged.",
								owner.Actor, owner.Actor, item)));
					break;
				case CanUseBodypartResult.CantUsePartPain:
					owner.Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {FullDescription().ToLowerInvariant()} is in too much pain.",
								owner.Actor, owner.Actor, item)));
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
}