#nullable enable

using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class PinPullDetonatorGameItemComponent : DeadlineExplosiveTriggerGameItemComponent,
	IPinPullExplosiveTrigger
{
	private PinPullDetonatorGameItemComponentProto _prototype;

	public PinPullDetonatorGameItemComponent(PinPullDetonatorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public PinPullDetonatorGameItemComponent(MudSharp.Models.GameItemComponent component,
		PinPullDetonatorGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadDeadline(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public PinPullDetonatorGameItemComponent(PinPullDetonatorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public bool PinPulled => HasActiveDeadline;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PinPullDetonatorGameItemComponentProto)newProto;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PinPullDetonatorGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		var root = new XElement("Definition");
		SaveDeadline(root);
		return root.ToString();
	}

	public bool CanPullPin(ICharacter actor)
	{
		return !PinPulled && Parent.IsItemType<IDetonatable>() &&
		       ExplosiveDeadlineScheduler.TryGetDeadline(RuntimeClock.UtcNow, _prototype.Delay, out _);
	}

	public string WhyCannotPullPin(ICharacter actor)
	{
		if (PinPulled)
		{
			return $"The pin has already been pulled from {Parent.HowSeen(actor)}.";
		}

		if (!ExplosiveDeadlineScheduler.TryGetDeadline(RuntimeClock.UtcNow, _prototype.Delay, out _))
		{
			return $"The configured delay on {Parent.HowSeen(actor)} is too long to schedule safely.";
		}

		return Parent.IsItemType<IDetonatable>()
			? $"You cannot pull the pin from {Parent.HowSeen(actor)} at this time."
			: $"{Parent.HowSeen(actor, true)} has no explosive payload to detonate.";
	}

	public bool PullPin(ICharacter actor, IEmote? playerEmote = null)
	{
		if (!CanPullPin(actor))
		{
			actor.Send(WhyCannotPullPin(actor));
			return false;
		}

		if (!ExplosiveDeadlineScheduler.TryGetDeadline(RuntimeClock.UtcNow, _prototype.Delay, out var deadline))
		{
			actor.Send($"The configured delay on {Parent.HowSeen(actor)} is too long to schedule safely.");
			return false;
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.PullPinEmote, actor, actor, Parent)).Append(playerEmote));
		StartDeadline(deadline);
		return true;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Short or DescriptionType.Full or DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Short && PinPulled)
		{
			return $"{description} {"(pin pulled)".Colour(Telnet.BoldRed)}";
		}

		if (type == DescriptionType.Full)
		{
			if (!PinPulled)
			{
				return $"{description}\n\nIts safety pin is still in place.";
			}

			var remaining = RemainingDuration();
			var remainingText = remaining > TimeSpan.Zero
				? remaining.Describe(voyeur).Colour(Telnet.BoldRed)
				: "less than a second".Colour(Telnet.BoldRed);
			return $"{description}\n\nIts safety pin has been pulled and detonation is irreversible, with {remainingText} remaining.";
		}

		if (type == DescriptionType.Evaluate)
		{
			return
				$"It has a pin-pull detonator with an irreversible delay of {_prototype.Delay.Describe(voyeur).ColourValue()}.";
		}

		return description;
	}

	public override int DecorationPriority => int.MaxValue - 1;
}
