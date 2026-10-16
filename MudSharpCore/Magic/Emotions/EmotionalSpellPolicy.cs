#nullable enable

using MudSharp.Magic;

namespace MudSharp.Magic.Emotions;

public enum EmotionalSpellKind { Fury, Calm }

/// <summary>Explicit caster-terrain mapping; rational units are truncated as in the source.</summary>
public sealed record EmotionalTerrainRule(int DurationNumerator, int DurationDenominator, int EnduranceBonusPerGrade)
{
	public int DurationUnits(int grade)
	{
		EmotionalSpellPolicy.ValidateGrade(grade);
		if (DurationNumerator <= 0 || DurationDenominator <= 0 || EnduranceBonusPerGrade < 0)
			throw new ArgumentException("Terrain duration and endurance mappings must be finite positive units with a nonnegative bonus.");
		return Math.Max(1, checked(grade * DurationNumerator) / DurationDenominator);
	}
}

public sealed record EmotionalCounter(int RemainingIncomingGrade, int RemainingOpposingGrade, bool RemoveOpposing)
{
	public bool Continue => RemainingIncomingGrade > 0;
	public bool ChangesOpposing => RemoveOpposing || RemainingOpposingGrade > 0;
}

/// <summary>Source strength and delivered native fields are deliberately independent.</summary>
public sealed record EmotionalRetainedState(int SourceGrade, SpellPower Power, double Intensity, double EndurancePoints);
public sealed record EmotionalLifetime(TimeSpan Duration, EmotionalRetainedState State);

public static class EmotionalSpellPolicy
{
	public static void ValidateGrade(int grade)
	{
		if (grade is < 1 or > 7) throw new ArgumentOutOfRangeException(nameof(grade), "An explicit source grade from one to seven is required.");
	}

	public static EmotionalCounter Counter(int incomingGrade, int? opposingGrade)
	{
		ValidateGrade(incomingGrade);
		if (opposingGrade is null) return new(incomingGrade, 0, false);
		ValidateGrade(opposingGrade.Value);
		return opposingGrade > incomingGrade
			? new(0, opposingGrade.Value - incomingGrade, false)
			: new(incomingGrade - opposingGrade.Value, 0, true);
	}

	/// <summary>codedump.c number(): inverted bounds return from; endpoints are inclusive.</summary>
	public static int RollEndurance(int grade, EmotionalTerrainRule terrain, Func<int, int, int> inclusiveRandom)
	{
		terrain.DurationUnits(grade);
		var upper = grade * grade / 2;
		var roll = upper < grade ? grade : inclusiveRandom(grade, upper);
		if (roll < grade || roll > Math.Max(grade, upper)) throw new InvalidOperationException("The endurance draw lies outside the source interval.");
		return checked(roll + grade * terrain.EnduranceBonusPerGrade);
	}

	public static EmotionalLifetime ResolveLifetime(EmotionalSpellKind kind, int grade, EmotionalTerrainRule terrain,
		int unitSeconds, int capUnits, EmotionalRetainedState incoming, TimeSpan? remaining,
		EmotionalRetainedState? previous)
	{
		ValidateGrade(grade);
		if (!Enum.IsDefined(kind) || unitSeconds <= 0 || capUnits <= 0 ||
			(double)unitSeconds * capUnits > TimeSpan.MaxValue.TotalSeconds)
			throw new ArgumentException("Invalid emotional lifetime mapping.");
		ValidateState(incoming);
		if (incoming.SourceGrade != grade) throw new ArgumentException("Incoming state must record the residual source grade.");
		if (remaining.HasValue != (previous is not null)) throw new ArgumentException("Previous strength and remaining duration must be paired.");
		var units = kind == EmotionalSpellKind.Calm ? checked(2 * grade) : terrain.DurationUnits(grade);
		var state = incoming;
		if (previous is not null)
		{
			ValidateState(previous);
			if (remaining <= TimeSpan.Zero || remaining > TimeSpan.FromSeconds((double)unitSeconds * capUnits))
				throw new ArgumentException("Retained effects require a positive bounded native remaining duration.");
			units = checked(units + (int)Math.Ceiling(remaining!.Value.TotalSeconds / unitSeconds));
			// Fury copies the old affect, including power and modifier. Calm uses stack_spell_affect's MAX power.
			if (kind == EmotionalSpellKind.Fury || previous.SourceGrade > incoming.SourceGrade) state = previous;
			if (kind == EmotionalSpellKind.Calm && previous.SourceGrade == incoming.SourceGrade &&
				(previous.Power != incoming.Power || previous.Intensity != incoming.Intensity))
				throw new ArgumentException("Equal source grades disagree on their editable native mapping.");
		}
		return new(TimeSpan.FromSeconds((double)Math.Min(capUnits, units) * unitSeconds), state);
	}

	public static void ValidateState(EmotionalRetainedState state)
	{
		ValidateGrade(state.SourceGrade);
		if (!Enum.IsDefined(state.Power) || !double.IsFinite(state.Intensity) || state.Intensity < 0 ||
			!double.IsFinite(state.EndurancePoints) || state.EndurancePoints < 0)
			throw new ArgumentException("Invalid retained emotional source/native state.");
	}
}
