#nullable enable

using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class CountdownDetonatorGameItemComponent : DeadlineExplosiveTriggerGameItemComponent,
	IArmableExplosiveTrigger
{
	private CountdownDetonatorGameItemComponentProto _prototype;

	public CountdownDetonatorGameItemComponent(CountdownDetonatorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public CountdownDetonatorGameItemComponent(MudSharp.Models.GameItemComponent component,
		CountdownDetonatorGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadDeadline(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public CountdownDetonatorGameItemComponent(CountdownDetonatorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public bool Armed => HasActiveDeadline;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (CountdownDetonatorGameItemComponentProto)newProto;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new CountdownDetonatorGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		var root = new XElement("Definition");
		SaveDeadline(root);
		return root.ToString();
	}

	public bool CanArm(ICharacter actor, string argument)
	{
		return !Armed &&
		       Parent.IsItemType<IDetonatable>() &&
		       TryResolveDelay(actor, argument, out _, out _);
	}

	public string WhyCannotArm(ICharacter actor, string argument)
	{
		if (Armed)
		{
			return $"{Parent.HowSeen(actor, true)} is already armed.";
		}

		if (!Parent.IsItemType<IDetonatable>())
		{
			return $"{Parent.HowSeen(actor, true)} has no explosive payload to detonate.";
		}

		return TryResolveDelay(actor, argument, out _, out var error)
			? $"{Parent.HowSeen(actor, true)} cannot be armed at this time."
			: error;
	}

	public bool Arm(ICharacter actor, string argument, IEmote? playerEmote = null)
	{
		if (!CanArm(actor, argument))
		{
			actor.Send(WhyCannotArm(actor, argument));
			return false;
		}

		TryResolveDelay(actor, argument, out var delay, out _);
		if (!ExplosiveDeadlineScheduler.TryGetDeadline(RuntimeClock.UtcNow, delay, out var deadline))
		{
			actor.Send("That countdown is too long to schedule safely.");
			return false;
		}
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.ArmEmote, actor, actor, Parent)).Append(playerEmote));
		StartDeadline(deadline);
		return true;
	}

	public bool CanDisarm(ICharacter actor)
	{
		return Armed && _prototype.CanBeDisarmed;
	}

	public string WhyCannotDisarm(ICharacter actor)
	{
		if (!Armed)
		{
			return $"{Parent.HowSeen(actor, true)} is not armed.";
		}

		return _prototype.CanBeDisarmed
			? $"{Parent.HowSeen(actor, true)} cannot be disarmed at this time."
			: $"Once armed, {Parent.HowSeen(actor)} has an irreversible countdown.";
	}

	public bool Disarm(ICharacter actor, IEmote? playerEmote = null)
	{
		if (!CanDisarm(actor))
		{
			actor.Send(WhyCannotDisarm(actor));
			return false;
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.DisarmEmote, actor, actor, Parent)).Append(playerEmote));
		CancelDeadline();
		return true;
	}

	private bool TryResolveDelay(ICharacter actor, string argument, out TimeSpan delay, out string error)
	{
		delay = _prototype.DefaultDelay;
		error = string.Empty;
		if (!string.IsNullOrWhiteSpace(argument))
		{
			if (!_prototype.PlayersCanSetDelay)
			{
				error = $"{Parent.HowSeen(actor, true)} has a fixed countdown of {_prototype.DefaultDelay.Describe(actor).ColourValue()}.";
				return false;
			}

			if (!TimeSpan.TryParse(argument, actor, out delay))
			{
				error = "That is not a valid countdown duration.";
				return false;
			}
		}

		if (delay < _prototype.MinimumDelay || delay > _prototype.MaximumDelay)
		{
			error =
				$"The countdown must be between {_prototype.MinimumDelay.Describe(actor).ColourValue()} and {_prototype.MaximumDelay.Describe(actor).ColourValue()}.";
			return false;
		}

		if (!ExplosiveDeadlineScheduler.TryGetDeadline(RuntimeClock.UtcNow, delay, out _))
		{
			error = "That countdown is too long to schedule safely.";
			return false;
		}

		return true;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Short or DescriptionType.Full or DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Short && Armed)
		{
			return $"{description} {"(armed)".Colour(Telnet.BoldRed)}";
		}

		if (type == DescriptionType.Full)
		{
			return Armed
				? $"{description}\n\nIts countdown detonator is armed with {DescribeRemaining(voyeur)} remaining."
				: $"{description}\n\nIts countdown detonator is currently disarmed.";
		}

		if (type == DescriptionType.Evaluate)
		{
			return
				$"It has a countdown detonator with a default delay of {_prototype.DefaultDelay.Describe(voyeur).ColourValue()} and a permitted range of {_prototype.MinimumDelay.Describe(voyeur).ColourValue()} to {_prototype.MaximumDelay.Describe(voyeur).ColourValue()}. Players {(_prototype.PlayersCanSetDelay ? "can" : "cannot")} choose the delay. It {(_prototype.CanBeDisarmed ? "can" : "cannot")} be disarmed after arming.";
		}

		return description;
	}

	private string DescribeRemaining(IPerceiver voyeur)
	{
		var remaining = RemainingDuration();
		return remaining > TimeSpan.Zero
			? remaining.Describe(voyeur).Colour(Telnet.BoldRed)
			: "less than a second".Colour(Telnet.BoldRed);
	}

	public override int DecorationPriority => int.MaxValue - 1;
}
