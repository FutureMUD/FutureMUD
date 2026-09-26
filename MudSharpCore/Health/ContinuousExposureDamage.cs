using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MudSharp.Body;

#nullable enable
namespace MudSharp.Health;

/// <summary>Evaluate existing flat/nonlinear armour formulae against a one-second rate,
/// then return to interval quantities. Ordinary combat keeps its original strike semantics.</summary>
public static class ContinuousExposureDamage
{
	[ThreadStatic] private static HealthBatch? _healthBatch;

	/// <summary>One health/status pass per actual body for simultaneous patches in a simulation substep.</summary>
	public static IDisposable BeginHealthBatch() => new HealthBatch();

	public static void ProcessWounds(IBody? body, IReadOnlyList<IWound> wounds)
	{
		if (body is null || _healthBatch is null) { wounds.ProcessPassiveWounds(body); return; }
		_healthBatch.Add(body, wounds);
	}

	private sealed class HealthBatch : IDisposable
	{
		private readonly HealthBatch? _previous = _healthBatch;
		private readonly Dictionary<IBody, HashSet<IWound>> _wounds = new(ReferenceEqualityComparer.Instance);
		public HealthBatch() { _healthBatch = this; }
		public void Add(IBody body, IEnumerable<IWound> wounds)
		{
			if (!_wounds.TryGetValue(body, out var owned)) _wounds[body] = owned = new(ReferenceEqualityComparer.Instance);
			owned.UnionWith(wounds);
		}
		public void Dispose()
		{
			_healthBatch = _previous;
			foreach (var (body, wounds) in _wounds)
				if (_previous is not null) _previous.Add(body, wounds);
				else wounds.ProcessPassiveWounds(body);
		}
	}

	public static bool TryNormalise(IDamage damage, out IDamage normalised, out double seconds)
	{
		seconds = damage.ExposureContext?.ReferenceSeconds ?? 1.0;
		if (!double.IsFinite(seconds) || seconds <= 0 || Math.Abs(seconds - 1.0) < 1e-12)
		{ normalised = damage; return false; }
		normalised = new Damage(damage, 1.0 / seconds) { ExposureContext = damage.ExposureContext! with { ReferenceSeconds = 1.0 } };
		return true;
	}
	public static IDamage? Restore(IDamage? damage, IDamage original, double seconds) => damage is null ? null :
		new Damage(damage, seconds) { ExposureContext = original.ExposureContext };

	/// <summary>Called after armour and strategy eligibility. Combat wounds are never eligible for reuse.</summary>
	public static IEnumerable<IWound> Accumulate<T>(IHaveWounds owner, IDamage damage, Func<IDamage, T> create,
		bool bodyModifiers = true) where T : class, IContinuousExposureWound
	{
		if (damage.ExposureContext is not { } context) return new IWound[] { create(damage) };
		var body = damage.TargetBody ?? (owner as ICharacter)?.Body;
		var key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
		{
			context.Route, context.SourceKind, context.SourceIdentity, context.ReactionId, context.Channel,
			Body = body?.Id ?? 0, Part = damage.Bodypart?.Id ?? 0
		}))));
		var existing = (body?.Wounds ?? owner.Wounds).OfType<T>()
			.Where(x => x.ExposureKey == key && x.DamageType == damage.DamageType && ReferenceEquals(x.Bodypart, damage.Bodypart))
			.ToList();
		var cap = bodyModifiers && body is not null && damage.Bodypart is not null ? body.HitpointsForBodypart(damage.Bodypart) : double.PositiveInfinity;
		var modifiedDamage = damage.DamageAmount * (bodyModifiers ? Math.Max(0, damage.Bodypart?.DamageModifier ?? 1) : 1);
		if (cap <= 0 || double.IsNaN(cap)) return new IWound[] { create(damage) };
		var result = new List<IWound>();
		var remaining = 1.0;
		while (remaining > 1e-12)
		{
			var wound = existing.FirstOrDefault(x => modifiedDamage <= 0 || cap - x.CurrentDamage > 1e-9);
			var space = wound is null ? cap : cap - wound.CurrentDamage;
			var fraction = modifiedDamage > 0 ? Math.Min(remaining, space / modifiedDamage) : remaining;
			var increment = new Damage(damage, fraction);
			if (wound is null)
			{
				wound = create(increment);
				wound.ExposureKey = key;
			}
			else wound.SufferAdditionalExposureDamage(increment);
			result.Add(wound);
			existing.Remove(wound);
			remaining -= fraction;
		}
		return result;
	}

	internal static BleedStatus BleedingFor(DamageType type, WoundSeverity severity) => type switch
	{
		DamageType.Slashing or DamageType.Claw or DamageType.Chopping or DamageType.Ballistic or
			DamageType.BallisticArmourPiercing or DamageType.Shearing or DamageType.Arcane when severity >= WoundSeverity.Moderate => BleedStatus.Bleeding,
		DamageType.Piercing or DamageType.ArmourPiercing or DamageType.Bite or DamageType.Shrapnel when severity >= WoundSeverity.Severe => BleedStatus.Bleeding,
		DamageType.Wrenching when severity >= WoundSeverity.Horrifying => BleedStatus.Bleeding,
		DamageType.Falling when severity >= WoundSeverity.Grievous => BleedStatus.Bleeding,
		_ => BleedStatus.NeverBled
	};
}
