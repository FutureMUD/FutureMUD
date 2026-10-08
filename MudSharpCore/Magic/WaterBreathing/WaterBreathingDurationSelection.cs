#nullable enable

using MudSharp.Magic.SpellEffects;

namespace MudSharp.Magic.WaterBreathing;

/// <summary>
/// One source duration draw. The existing prepared-selection protocol will own
/// recipient/cohort admission and carry this immutable value between casting copies.
/// </summary>
internal sealed class WaterBreathingDurationSelection
{
	private WaterBreathingDurationSelection(int grade, SpellPower power, MagicSpellLifetimePolicy policy, int draw)
	{
		Grade = grade;
		Power = power;
		Policy = policy;
		Draw = draw;
		DurationUnits = Math.Max(1, draw);
		Increment = TimeSpan.FromSeconds((long)DurationUnits * policy.UnitSeconds);
	}

	public int Grade { get; }
	public SpellPower Power { get; }
	public MagicSpellLifetimePolicy Policy { get; }
	public int Draw { get; }
	public int DurationUnits { get; }
	public TimeSpan Increment { get; }

	public static WaterBreathingDurationSelection Select(int grade, SpellPower power,
		MagicSpellLifetimePolicy policy, Func<int, int, int> nextInclusive)
	{
		ArgumentNullException.ThrowIfNull(policy);
		ArgumentNullException.ThrowIfNull(nextInclusive);
		if (grade is < 1 or > 7) throw new ArgumentOutOfRangeException(nameof(grade));
		if (!Enum.IsDefined(power)) throw new ArgumentOutOfRangeException(nameof(power));
		if (DetectInvisibleEffect.PolicyError(policy) is { } error) throw new ArgumentException(error, nameof(policy));
		var lower = grade / 2;
		var upper = grade * 3;
		var draw = nextInclusive(lower, upper);
		if (draw < lower || draw > upper)
			throw new InvalidOperationException("The Water Breathing duration sampler returned a value outside its inclusive source bounds.");
		return new(grade, power, policy, draw);
	}

	public bool MatchesBinding(int grade, SpellPower power, MagicSpellLifetimePolicy policy) =>
		Grade == grade && Power == power && Policy == policy;
}
