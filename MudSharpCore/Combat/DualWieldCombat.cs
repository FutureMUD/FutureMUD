#nullable enable

using System.Globalization;
using System.Runtime.CompilerServices;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

internal static class DualWieldCombat
{
	private static readonly ConditionalWeakTable<IFuturemud, Dictionary<string, string>> Diagnostics = new();

	internal static void Diagnose(IFuturemud world, string key, string value)
	{
		var seen = Diagnostics.GetOrCreateValue(world);
		if (seen.TryGetValue(key, out var previous) && previous == value) return;
		seen[key] = value;
		world.DebugMessage($"Dual Wield: invalid {key} ({value}); inspect show config / show checks before enabling double attacks.");
	}

	internal static double Number(IFuturemud world, string key, double fallback, bool allowZero = false)
	{
		var text = world.GetStaticConfiguration(key);
		if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) &&
		    double.IsFinite(value) && (allowZero ? value >= 0 : value > 0)) return value;
		Diagnose(world, key, text ?? "missing");
		return fallback;
	}

	internal static int Integer(IFuturemud world, string key, int fallback, int min, int max)
	{
		var text = world.GetStaticConfiguration(key);
		if (int.TryParse(text, out var value) && value >= min && value <= max) return value;
		Diagnose(world, key, text ?? "missing");
		return fallback;
	}

	internal static bool Enabled(IFuturemud world)
	{
		var text = world.GetStaticConfiguration("DualWieldDoubleAttacksEnabled");
		if (!bool.TryParse(text, out var enabled))
		{
			Diagnose(world, "DualWieldDoubleAttacksEnabled", text ?? "missing");
			return false;
		}
		if (!enabled) return false;
		var check = world.GetCheck(CheckType.DualWieldCoordinationCheck);
		if (ValidCheck(check)) return true;
		Diagnose(world, "DualWieldCoordinationCheck", check?.TargetNumberExpression?.OriginalFormulaText ?? "missing");
		return false;
	}

	internal static bool ValidCheck(ICheck? check)
	{
		if (check is null || check.Type != CheckType.DualWieldCoordinationCheck ||
		    check.CanTraitBranchIfMissing || check.FailIfTraitMissing != FailIfTraitMissingType.FailIfAnyMissing ||
		    check.TargetNumberExpression?.Parameters is not { Count: > 0 } parameters) return false;
		if (check.TargetNumberExpression.HasErrors()) return false;
		var learning = parameters.Values.Where(x => x.CanImprove).ToArray();
		return learning.Length == 1 && learning[0].Trait is { TraitType: TraitType.Skill, OwnerScope: TraitOwnerScope.Character } &&
		       parameters.Values.All(x => x.Trait is not null && !x.CanBranch) &&
		       parameters.Values.GroupBy(x => x.Trait.Id).All(x => x.Count(y => y.CanImprove) <= 1);
	}

	internal static int Stages(IFuturemud world, Outcome outcome)
	{
		var fallback = outcome switch
		{
			Outcome.MajorPass => -1,
			Outcome.Pass or Outcome.MinorPass => 0,
			Outcome.MinorFail or Outcome.Fail => 1,
			Outcome.MajorFail => 2,
			_ => throw new ArgumentOutOfRangeException(nameof(outcome))
		};
		return Integer(world, $"DualWieldCoordination{outcome}Stages", fallback, -10, 10);
	}

	internal static bool EligibleProfile(IWeaponAttack attack) =>
		attack.MoveType == BuiltInCombatMoveType.UseWeaponAttack && attack.MaximumTargets == 1 &&
		double.IsFinite(attack.BaseDelay) && attack.BaseDelay > 0 &&
		double.IsFinite(attack.StaminaCost) && attack.StaminaCost >= 0 &&
		attack.HandednessOptions is AttackHandednessOptions.Any or AttackHandednessOptions.OneHandedOnly;

	internal static bool WeaponReady(ICharacter actor, IMeleeWeapon weapon)
	{
		var hand = actor.Body.WieldedHand(weapon.Parent);
		return !weapon.Parent.Destroyed && !weapon.Parent.IsItemType<IShield>() &&
		       weapon is not IImplantMeleeWeapon and not IProstheticMeleeWeapon &&
		       actor.Body.WieldedItems.Any(x => ReferenceEquals(x, weapon.Parent)) &&
		       actor.Body.WieldedHandCount(weapon.Parent) == 1 && hand is IBodypart part &&
		       actor.Body.CanUseBodypart(part) == CanUseBodypartResult.CanUse;
	}

	/// <summary>Samples one affordable unordered profile pair; candidate mass does not grow with combinations.</summary>
	internal static (DualWieldDoubleAttackMove Move, double Weight)? Candidate(ICharacter actor,
		IReadOnlyList<(IMeleeWeapon Weapon, IWeaponAttack Attack, double Weight)> candidates,
		Func<double>? random = null)
	{
		if (actor.CombatTarget is not ICharacter target || ReferenceEquals(actor, target) ||
		    !actor.State.IsAble() || !target.State.IsAble() || !actor.MeleeRange ||
		    !actor.ColocatedWith(target) || !Enabled(actor.Gameworld)) return null;
		var factor = Number(actor.Gameworld, "DualWieldDoubleAttackWeightMultiplier", 0, true);
		if (factor == 0) return null;
		random ??= () => Constants.Random.NextDouble();
		var eligible = candidates.Where(x => double.IsFinite(x.Weight) && x.Weight > 0 &&
			EligibleProfile(x.Attack) && actor.CombatSettings.ClassificationsAllowed.Contains(x.Weapon.Classification) && WeaponReady(actor, x.Weapon))
			.OrderBy(x => x.Weapon.Parent.Id).ThenBy(x => x.Attack.Id).ToArray();
		var participating = new HashSet<int>();
		(int A, int B)? chosen = null;
		var total = 0.0;
		// Scale weights first, avoiding overflow without changing the relative distribution.
		var scale = eligible.Select(x => x.Weight).DefaultIfEmpty(1).Max();
		for (var a = 0; a < eligible.Length; a++)
		for (var b = a + 1; b < eligible.Length; b++)
		{
			if (ReferenceEquals(eligible[a].Weapon.Parent, eligible[b].Weapon.Parent) ||
			    !actor.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(actor, eligible[a].Attack) +
			                           MeleeWeaponAttack.MoveStaminaCost(actor, eligible[b].Attack))) continue;
			var weight = eligible[a].Weight / scale * (eligible[b].Weight / scale);
			if (weight <= 0) continue;
			total += weight;
			participating.Add(a); participating.Add(b);
			if (chosen is null || random() * total < weight) chosen = (a, b);
		}
		if (chosen is not { } pair) return null;
		var first = eligible[pair.A]; var second = eligible[pair.B];
		if (random() < 0.5) (first, second) = (second, first);
		var mass = factor * participating.Sum(x => eligible[x].Weight);
		if (!double.IsFinite(mass) || mass <= 0) return null;
		return (new DualWieldDoubleAttackMove(actor, target, first.Weapon, first.Attack, second.Weapon, second.Attack), mass);
	}
}
