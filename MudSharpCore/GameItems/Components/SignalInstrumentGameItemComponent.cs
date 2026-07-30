#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class SignalInstrumentGameItemComponent : InstrumentGameItemComponent, ISignalInstrument
{
	private SignalInstrumentGameItemComponentProto SignalPrototype =>
		(SignalInstrumentGameItemComponentProto)_prototype;

	public SignalInstrumentGameItemComponent(SignalInstrumentGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
	}

	public SignalInstrumentGameItemComponent(MudSharp.Models.GameItemComponent component,
		SignalInstrumentGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
	}

	private SignalInstrumentGameItemComponent(SignalInstrumentGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
	}

	public IReadOnlyCollection<InstrumentSignalPattern> SignalPatterns => SignalPrototype.SignalPatterns;
	public double SignalStaminaCost => SignalPrototype.SignalStaminaCost;
	public TimeSpan SignalCooldown => SignalPrototype.SignalCooldown;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SignalInstrumentGameItemComponent(this, newParent, temporary);
	}

	public bool CanSignal(ICharacter actor, string pattern)
	{
		return string.IsNullOrEmpty(WhyCannotSignal(actor, pattern));
	}

	public string WhyCannotSignal(ICharacter actor, string pattern)
	{
		if (IsBeingPlayed)
		{
			return $"You must stop playing {Parent.HowSeen(actor)} before sounding a signal.";
		}

		if (SignalPatterns.All(x => !x.Name.EqualTo(pattern)))
		{
			return $"{Parent.HowSeen(actor, true)} has no signal named {pattern.ColourName()}.";
		}

		if (actor.EffectsOfType<SignalInstrumentCooldown>()
		         .Any(x => ReferenceEquals(x.Instrument, this)))
		{
			return $"{Parent.HowSeen(actor, true)} cannot be used to signal again quite yet.";
		}

		var useWhy = WhyCannotUse(actor);
		if (!string.IsNullOrEmpty(useWhy))
		{
			return useWhy;
		}

		if (!actor.CanSpendStamina(SignalStaminaCost))
		{
			return "You are too exhausted to sound that signal.";
		}

		if (SignalPrototype.CanSignalProg?.ExecuteBool(false, actor, Parent, pattern) == false)
		{
			return SignalPrototype.WhyCannotSignalProg?.ExecuteString(actor, Parent, pattern) ??
			       "You cannot sound that signal right now.";
		}

		return string.Empty;
	}

	private string WhyCannotUse(ICharacter actor)
	{
		if (!CharacterState.Able.HasFlag(actor.State))
		{
			return "You are not in a fit state to sound a signal.";
		}

		if (actor.Combat is not null)
		{
			return "You cannot sound a deliberate signal while in combat.";
		}

		if (SignalPrototype.AllowedPositions.Count > 0 &&
		    !SignalPrototype.AllowedPositions.Contains(actor.PositionState.Name))
		{
			return $"You cannot use {Parent.HowSeen(actor)} while {actor.PositionState.DefaultDescription()}.";
		}

		var handheld = actor.Body.HeldOrWieldedItems.Contains(Parent);
		var worn = actor.Body.WornItems.Contains(Parent);
		var roomPositioned = Parent.Location == actor.Location &&
		                     Parent.InInventoryOf is null &&
		                     Parent.ContainedIn is null;
		if ((!handheld || !UseModes.HasFlag(InstrumentUseMode.Handheld)) &&
		    (!worn || !UseModes.HasFlag(InstrumentUseMode.Worn)) &&
		    (!roomPositioned || !UseModes.HasFlag(InstrumentUseMode.Room)))
		{
			return $"You are not using {Parent.HowSeen(actor)} in one of its permitted ways.";
		}

		var availableHands = actor.Body.FunctioningFreeHands.Count() + (handheld ? 1 : 0);
		return availableHands < RequiredHands
			? $"You need {RequiredHands.ToString("N0", actor)} functioning hand{(RequiredHands == 1 ? "" : "s")} to sound {Parent.HowSeen(actor)}."
			: string.Empty;
	}

	public bool Signal(ICharacter actor, string pattern, IEmote? playerEmote = null)
	{
		var why = WhyCannotSignal(actor, pattern);
		if (!string.IsNullOrEmpty(why))
		{
			actor.OutputHandler.Send(why);
			return false;
		}

		var signal = SignalPatterns.First(x => x.Name.EqualTo(pattern));
		actor.SpendStamina(SignalStaminaCost);
		var outcome = ResolveOutcome(actor);
		actor.AddEffect(new SignalInstrumentCooldown(actor, this), SignalCooldown);
		if (outcome.IsFail())
		{
			EmitLocal(actor, signal.FailureEmote, playerEmote);
			actor.Location?.HandleAudioEcho("You hear a garbled and unrecognisable signal sounded {0}.",
				Volume, Parent, actor.RoomLayer);
			return false;
		}

		EmitLocal(actor, signal.LocalEmote, playerEmote);
		actor.Location?.HandleAudioEcho(string.Format(signal.DistantEmote, "{0}"),
			Volume, Parent, actor.RoomLayer);
		SignalPrototype.OnSignalProg?.Execute(actor, Parent, signal.Name, (int)outcome);
		return true;
	}
}
