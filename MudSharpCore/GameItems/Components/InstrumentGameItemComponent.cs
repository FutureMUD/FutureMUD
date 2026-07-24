#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Audio;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class InstrumentGameItemComponent : GameItemComponent, IInstrument
{
	protected InstrumentGameItemComponentProto _prototype;
	private ICharacter? _currentPerformer;
	private string? _currentStyle;
	private Outcome _currentOutcome = Outcome.NotTested;
	private bool _stopping;

	public InstrumentGameItemComponent(InstrumentGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public InstrumentGameItemComponent(MudSharp.Models.GameItemComponent component,
		InstrumentGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	protected InstrumentGameItemComponent(InstrumentGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public string InstrumentFamily => _prototype.InstrumentFamily;
	public ITraitDefinition? PerformanceTrait => _prototype.PerformanceTrait;
	public Difficulty PerformanceDifficulty => _prototype.PerformanceDifficulty;
	public AudioVolume Volume => _prototype.Volume;
	public int RequiredHands => _prototype.RequiredHands;
	public InstrumentUseMode UseModes => _prototype.UseModes;
	public double InitialStaminaCost => _prototype.InitialStaminaCost;
	public double StaminaPerTick => _prototype.StaminaPerTick;
	public TimeSpan TickInterval => _prototype.TickInterval;
	public IReadOnlyCollection<string> Styles => _prototype.Styles;
	public ICharacter? CurrentPerformer => _currentPerformer;
	public string? CurrentStyle => _currentStyle;
	public Outcome CurrentOutcome => _currentOutcome;
	public bool IsBeingPlayed => _currentPerformer is not null;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new InstrumentGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (InstrumentGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	public virtual bool CanPlay(ICharacter actor, string style)
	{
		return string.IsNullOrEmpty(WhyCannotPlay(actor, style));
	}

	public virtual string WhyCannotPlay(ICharacter actor, string style)
	{
		if (IsBeingPlayed)
		{
			return $"{Parent.HowSeen(actor, true)} is already being played.";
		}

		if (!CharacterState.Able.HasFlag(actor.State))
		{
			return "You are not in a fit state to play an instrument.";
		}

		if (actor.Combat is not null)
		{
			return "You cannot begin playing an instrument while in combat.";
		}

		if (!_prototype.Styles.Any(x => x.EqualTo(style)))
		{
			return $"{Parent.HowSeen(actor, true)} does not support the {style.ColourName()} style.";
		}

		if (_prototype.AllowedPositions.Count > 0 &&
		    !_prototype.AllowedPositions.Contains(actor.PositionState.Name))
		{
			return $"You cannot play {Parent.HowSeen(actor)} while {actor.PositionState.DefaultDescription()}.";
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
		if (availableHands < RequiredHands)
		{
			return $"You need {RequiredHands.ToString("N0", actor)} functioning hand{(RequiredHands == 1 ? "" : "s")} to play {Parent.HowSeen(actor)}.";
		}

		if (!actor.CanSpendStamina(InitialStaminaCost))
		{
			return "You are too exhausted to begin playing.";
		}

		if (_prototype.CanPlayProg?.ExecuteBool(false, actor, Parent, style) == false)
		{
			return _prototype.WhyCannotPlayProg?.ExecuteString(actor, Parent, style) ??
			       "You cannot play that instrument right now.";
		}

		return string.Empty;
	}

	public virtual bool Play(ICharacter actor, string style, IEmote? playerEmote = null)
	{
		var why = WhyCannotPlay(actor, style);
		if (!string.IsNullOrEmpty(why))
		{
			actor.OutputHandler.Send(why);
			return false;
		}

		actor.SpendStamina(InitialStaminaCost);
		var outcome = ResolveOutcome(actor);
		if (outcome.IsFail())
		{
			EmitLocal(actor, _prototype.FailureEmote, playerEmote);
			return false;
		}

		_currentPerformer = actor;
		_currentStyle = style;
		_currentOutcome = outcome;
		EmitPerformance(actor, _prototype.LocalPlayEmote, playerEmote);
		_prototype.OnPlayProg?.Execute(actor, Parent, style, (int)outcome);
		actor.AddEffect(new PlayingInstrument(actor, this), TickInterval);
		return true;
	}

	protected Outcome ResolveOutcome(ICharacter actor)
	{
		if (PerformanceTrait is null || PerformanceDifficulty == Difficulty.Automatic)
		{
			return Outcome.Pass;
		}

		return Gameworld.GetCheck(CheckType.GenericSkillCheck)
		                .Check(actor, PerformanceDifficulty, PerformanceTrait)
		                .Outcome;
	}

	protected void EmitLocal(ICharacter actor, string emote, IEmote? playerEmote = null)
	{
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(emote, actor, actor, Parent)).Append(playerEmote));
	}

	protected void EmitPerformance(ICharacter actor, string emote, IEmote? playerEmote = null)
	{
		EmitLocal(actor, emote, playerEmote);
		actor.Location?.HandleAudioEcho(
			string.Format(_prototype.DistantPlayEmote, "{0}"),
			Volume,
			Parent,
			actor.RoomLayer);
	}

	public void PerformTick()
	{
		var actor = _currentPerformer;
		if (actor is null)
		{
			return;
		}

		if (!CanContinue(actor))
		{
			StopPlaying(actor);
			return;
		}

		if (!actor.CanSpendStamina(StaminaPerTick))
		{
			actor.OutputHandler.Send("You are too exhausted to continue playing.");
			StopPlaying(actor);
			return;
		}

		actor.SpendStamina(StaminaPerTick);
		EmitPerformance(actor, _prototype.LocalTickEmote);
	}

	internal bool CanContinue(ICharacter actor)
	{
		if (!CharacterState.Able.HasFlag(actor.State) || actor.Combat is not null)
		{
			return false;
		}

		if (_prototype.AllowedPositions.Count > 0 &&
		    !_prototype.AllowedPositions.Contains(actor.PositionState.Name))
		{
			return false;
		}

		var handheld = actor.Body.HeldOrWieldedItems.Contains(Parent);
		var worn = actor.Body.WornItems.Contains(Parent);
		var roomPositioned = Parent.Location == actor.Location &&
		                     Parent.InInventoryOf is null &&
		                     Parent.ContainedIn is null;
		return handheld && UseModes.HasFlag(InstrumentUseMode.Handheld) ||
		       worn && UseModes.HasFlag(InstrumentUseMode.Worn) ||
		       roomPositioned && UseModes.HasFlag(InstrumentUseMode.Room);
	}

	public void StopPlaying(ICharacter actor, bool echo = true)
	{
		if (_currentPerformer != actor || _stopping)
		{
			return;
		}

		_stopping = true;
		var style = _currentStyle ?? "general";
		if (echo)
		{
			EmitLocal(actor, _prototype.StopEmote);
		}

		_currentPerformer = null;
		_currentStyle = null;
		_currentOutcome = Outcome.NotTested;
		actor.RemoveAllEffects<PlayingInstrument>(x => ReferenceEquals(x.Instrument, this), true);
		_prototype.OnStopProg?.Execute(actor, Parent, style);
		_stopping = false;
	}

	internal void EffectEnded(ICharacter actor)
	{
		if (_currentPerformer == actor && !_stopping)
		{
			StopPlaying(actor);
		}
	}

	public override void Delete()
	{
		if (_currentPerformer is not null)
		{
			StopPlaying(_currentPerformer, false);
		}

		base.Delete();
	}

	public override void Quit()
	{
		if (_currentPerformer is not null)
		{
			StopPlaying(_currentPerformer, false);
		}

		base.Quit();
	}
}
