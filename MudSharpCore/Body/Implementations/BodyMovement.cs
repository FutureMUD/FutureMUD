using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	public IMoveSpeed CurrentSpeed => CurrentSpeeds.ContainsKey(PositionState) ? CurrentSpeeds[PositionState] : null;

	public Dictionary<IPositionState, IMoveSpeed> CurrentSpeeds { get; } = new();

	public IEnumerable<IMoveSpeed> Speeds => Prototype.Speeds;

	#region IHaveStamina Members

	private bool _staminaChanged;

	public bool StaminaChanged
	{
		get => _staminaChanged;
		set
		{
			if (!_noSave)
			{
				if (value && !_staminaChanged)
				{
					Changed = true;
				}

				_staminaChanged = value;
			}
		}
	}

	public double MaximumStamina { get; set; }

	private double _staminaSpentInCurrentPeriod;

	private double _currentStamina;

	public double CurrentStamina
	{
		get => _currentStamina;
		set
		{
			var old = _currentStamina;
			if (value < _currentStamina)
			{
				_staminaSpentInCurrentPeriod += _currentStamina - value;
			}

			_currentStamina = Math.Max(0.0, value);
			if (_currentStamina != old)
			{
				StaminaChanged = true;
			}
		}
	}

	public bool CanSpendStamina(double amount)
	{
		return amount <= CurrentStamina + SecondWindsAvailable().Count() * MaximumStamina;
	}

	public void GainStamina(double amount)
	{
		_currentStamina += amount;
		_currentStamina = Math.Min(_currentStamina, MaximumStamina);
		StaminaChanged = true;
	}

	public void SpendStamina(double amount)
	{
		_staminaSpentInCurrentPeriod += amount;
		if (amount > _currentStamina)
		{
			foreach (var availableSecondWind in SecondWindsAvailable())
			{
				if (availableSecondWind == null)
				{
					Actor.OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("DefaultSecondWindEmote"), Actor, Actor),
						style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
					Actor.AddEffect(new SecondWindExhausted(Actor, availableSecondWind),
						TimeSpan.FromSeconds(Gameworld.GetStaticDouble("DefaultSecondWindRecoveryTime")));
				}
				else
				{
					Actor.OutputHandler.Handle(new EmoteOutput(new Emote(availableSecondWind.Emote, Actor, Actor),
						style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
					Actor.AddEffect(new SecondWindExhausted(Actor, availableSecondWind),
						availableSecondWind.RecoveryDuration);
				}

				_currentStamina += MaximumStamina;

				if (amount <= _currentStamina)
				{
					break;
				}
			}
		}

		_currentStamina -= amount;
		_currentStamina = Math.Max(0.0, _currentStamina);
		StaminaChanged = true;
	}

	public IEnumerable<ISecondWindMerit> SecondWindsAvailable()
	{
		var availableSecondWind = Actor.Merits.OfType<ISecondWindMerit>().Where(x =>
			x.Applies(Actor) && !CombinedEffectsOfType<ISecondWindExhaustedEffect>().Any(y => y.Applies(x))).ToList();

		if (CombinedEffectsOfType<ISecondWindExhaustedEffect>().All(x => x.Merit != null))
		{
			availableSecondWind.Add(null);
		}

		return availableSecondWind;
	}

	private static TraitExpression _encumbranceLimitExpression;

	public static TraitExpression EncumbranceLimitExpression
	{
		get
		{
			if (_encumbranceLimitExpression == null)
			{
				_encumbranceLimitExpression = new TraitExpression(
					Futuremud.Games.First().GetStaticConfiguration("EncumbranceLimitExpression"),
					Futuremud.Games.First());
			}

			return _encumbranceLimitExpression;
		}
	}

	public double EncumbrancePercentage
	{
		get
		{
			var weight = ExternalItems.Sum(x => x.Weight);
			var limit = EncumbranceLimitExpression.Evaluate(Actor, null, TraitBonusContext.Encumbrance);
			if (limit <= 0.0)
			{
				return 0.0;
			}

			return weight / limit;
		}
	}

	public EncumbranceLevel Encumbrance
	{
		get
		{
			var weight = ExternalItems.Sum(x => x.Weight);
			var limit = EncumbranceLimitExpression.Evaluate(Actor, null, TraitBonusContext.Encumbrance);
			if (weight >= limit)
			{
				return EncumbranceLevel.CriticallyEncumbered;
			}

			var ratio = weight / limit;
			if (ratio >= Gameworld.GetStaticDouble("EncumbranceLimitRatioHeavy"))
			{
				return EncumbranceLevel.HeavilyEncumbered;
			}

			if (ratio >= Gameworld.GetStaticDouble("EncumbranceLimitRatioModerate"))
			{
				return EncumbranceLevel.ModeratelyEncumbered;
			}

			if (ratio >= Gameworld.GetStaticDouble("EncumbranceLimitRatioLight"))
			{
				return EncumbranceLevel.LightlyEncumbered;
			}

			return EncumbranceLevel.Unencumbered;
		}
	}

	public event ExertionEvent OnExertionChanged;

	private ExertionLevel _longtermExertion;

	public ExertionLevel LongtermExertion
	{
		get => _longtermExertion;
		protected set
		{
			if (_longtermExertion != value)
			{
				OnExertionChanged?.Invoke(_longtermExertion, value);
			}

			_longtermExertion = value;
		}
	}

	private ExertionLevel _currentExertion;

	public ExertionLevel CurrentExertion
	{
		get => _currentExertion;
		protected set
		{
			if (_currentExertion != value)
			{
				OnExertionChanged?.Invoke(_currentExertion, value);
			}

			_currentExertion = value;
		}
	}

	private bool _tenSecondStaminaActive;
	private bool _minuteStaminaActive;

	public void InitialiseStamina()
	{
		MaximumStamina = Character.Character.MaximumStaminaFor(Actor);
		_currentStamina = Math.Min(CurrentStamina, MaximumStamina);
		_currentExertion = ExertionLevel.Rest;
		_longtermExertion = ExertionLevel.Rest;
	}

	public void StartStaminaTick()
	{
		if (Actor.State.HasFlag(CharacterState.Stasis))
		{
			return;
		}

		if (!_tenSecondStaminaActive)
		{
			Gameworld.HeartbeatManager.TenSecondHeartbeat += StaminaTenSecondHeartbeat;
			_tenSecondStaminaActive = true;
			StaminaTenSecondHeartbeat();
		}

		if (!_minuteStaminaActive)
		{
			Gameworld.HeartbeatManager.MinuteHeartbeat += StaminaMinuteHeartbeat;
			_minuteStaminaActive = true;
			StaminaMinuteHeartbeat();
		}
	}

	public void EndStaminaTick(bool includeMinute)
	{
		Gameworld.HeartbeatManager.TenSecondHeartbeat -= StaminaTenSecondHeartbeat;
		_tenSecondStaminaActive = false;
		if (includeMinute)
		{
			Gameworld.HeartbeatManager.MinuteHeartbeat -= StaminaMinuteHeartbeat;
			_minuteStaminaActive = false;
		}
	}

	public void StaminaTenSecondHeartbeat()
	{
		if (CurrentStamina < MaximumStamina)
		{
			GainStamina(Convert.ToDouble(Prototype.StaminaRecoveryProg.Execute(Actor)));
		}

		var staminaSpentRatio = _staminaSpentInCurrentPeriod / MaximumStamina;
		ExertionLevel staminaExertion;
		if (staminaSpentRatio > 2)
		{
			staminaExertion = ExertionLevel.ExtremelyHeavy;
		}
		else if (staminaSpentRatio > 1)
		{
			staminaExertion = ExertionLevel.VeryHeavy;
		}
		else if (staminaSpentRatio > 0.5)
		{
			staminaExertion = ExertionLevel.Heavy;
		}
		else if (staminaSpentRatio > 0.25)
		{
			staminaExertion = ExertionLevel.Normal;
		}
		else if (_staminaSpentInCurrentPeriod > 0 || Combat != null)
		{
			staminaExertion = ExertionLevel.Low;
		}
		else
		{
			if (Actor.State.HasFlag(CharacterState.Sleeping))
			{
				staminaExertion = ExertionLevel.Sleep;
			}
			else if (PositionState.CompareTo(PositionSitting.Instance) != PositionHeightComparison.Higher)
			{
				staminaExertion = ExertionLevel.Rest;
			}
			else
			{
				staminaExertion = ExertionLevel.Low;
			}
		}

		CurrentExertion = staminaExertion < CurrentExertion ? CurrentExertion.StageDown() : staminaExertion;

		if (CurrentExertion > LongtermExertion)
		{
			LongtermExertion = CurrentExertion;
		}

		if (Location.IsSwimmingLayer(RoomLayer) && PositionState != PositionFlying.Instance)
		{
			Actor.DoSwimHeartbeat();
		}

		if (PositionState == PositionFlying.Instance)
		{
			Actor.DoFlyHeartbeat();
		}

		DoBreathing();
		CurrentStamina = Math.Min(CurrentStamina, MaximumStamina);
	}

	public void StaminaMinuteHeartbeat()
	{
		var isDead = Actor.State.HasFlag(CharacterState.Dead);
		if (LongtermExertion > ExertionLevel.Normal && Race.SweatLiquid != null && !isDead)
		{
			var multiplier = 0.4;
			switch (LongtermExertion)
			{
				case ExertionLevel.VeryHeavy:
					multiplier = 0.7;
					break;
				case ExertionLevel.ExtremelyHeavy:
					multiplier = 0.9;
					break;
			}

			var sweatTotal = Race.SweatRateInLitresPerMinute * multiplier;
			FulfilNeeds(new NeedFulfiller
			{
				WaterLitres = -1 * sweatTotal,
				ThirstPoints = -5.0 / 12.0 * multiplier
			});

			if (Race.SweatLiquid != null && sweatTotal > 0.0)
			{
				// Use relative hit chance as a proxy for bodypart size
				// TODO: use actual bodypart size
				// TODO: sweat modifiers for bodyparts
				// TODO: sweat modifiers for traits

				sweatTotal *= 0.5; // A substantial amount of the sweat is probably lost to the atmosphere

				var bodypartHitChanceSum = Math.Max(1.0, Bodyparts.Sum(x => x.RelativeHitChance));

				foreach (var part in _wornItems.GroupBy(x => x.Wearloc))
				{
					part.FirstOrDefault().Item
					    ?.ExposeToLiquid(
						    new LiquidMixture(Race.SweatLiquid,
							    sweatTotal * part.Key.RelativeHitChance / bodypartHitChanceSum, Gameworld), part.Key,
						    LiquidExposureDirection.FromUnderneath);
				}
			}
		}

		_staminaSpentInCurrentPeriod = 0;
		if (isDead)
		{
			CurrentExertion = CurrentExertion.StageDown();
		}

		if (CurrentExertion < LongtermExertion)
		{
			if (LongtermExertion > ExertionLevel.Sleep || isDead)
			{
				LongtermExertion = LongtermExertion.StageDown();
			}
		}

		if (LongtermExertion > ExertionLevel.Low)
		{
			var check = Gameworld.GetCheck(RPG.Checks.CheckType.ArmourUseCheck);
			check.Check(Actor, Actor.ArmourUseCheckDifficulty(ArmourUseDifficultyContext.General));
		}

		if (LongtermExertion == ExertionLevel.Stasis)
		{
			EndStaminaTick(true);
		}
	}

	public void SetExertion(ExertionLevel level)
	{
		LongtermExertion = level;
	}

	#endregion

	public void CheckPositionStillValid()
	{
		switch (PositionState)
		{
			case PositionStanding _:
			case PositionStandingAttention _:
			case PositionStandingEasy _:
			case PositionSquatting _:
				CheckStandingStillValid();
				break;
			case PositionKneeling _:
			case PositionProstrate _:
				CheckKneelingStillValid();
				break;
			case PositionClimbing _:
				CheckClimbingStillValid();
				break;
		}
	}

	private void CheckClimbingStillValid()
	{
		// TODO
	}

	private void CheckKneelingStillValid()
	{
		if (!CanKneel(false))
		{
			OutputHandler.Handle(new EmoteOutput(
				new Emote("@ can no longer continue to kneel and fall|falls to the ground.", Actor, Actor),
				flags: OutputFlags.SuppressObscured));
			Actor.PositionState = PositionSprawled.Instance;
		}
	}

	private void CheckStandingStillValid()
	{
		if (!CanStand(false))
		{
			OutputHandler.Handle(new EmoteOutput(
				new Emote("@ can no longer continue to stand and tumble|tumbles to the ground.", Actor, Actor),
				flags: OutputFlags.SuppressObscured));
			Actor.PositionState = PositionSprawled.Instance;
		}
	}
}