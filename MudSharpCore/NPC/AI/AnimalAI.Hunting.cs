#nullable enable

using MudSharp.Body.Needs;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Health;
using MudSharp.Traps;

namespace MudSharp.NPC.AI;

public partial class AnimalAI
{
	public AnimalHuntingSettings Hunting { get; private set; } = new();

	internal static bool IsStarving(ICharacter actor) => actor.NeedsModel.Status.HasFlag(NeedsResult.Starving);

	internal static bool RaceInLineage(IRace race, IEnumerable<long> ids)
	{
		var visited = new HashSet<long>();
		for (var current = race; current is not null && visited.Add(current.Id); current = current.ParentRace)
		{
			if (ids.Contains(current.Id)) return true;
		}
		return false;
	}

	internal double HuntThreshold(ICharacter actor, bool continuing) =>
		Math.Max(0, (continuing ? Hunting.AbandonThreshold : Hunting.EngageThreshold) -
		            (IsStarving(actor) ? Hunting.StarvationAdjustment : 0));

	/// <summary>Absolute policy gates apply before any advantage, starvation adjustment or trap opportunity.</summary>
	internal string? PreyRejection(ICharacter actor, ICharacter target, bool continuing = false)
	{
		if (!Hunting.Enabled) return "advanced hunting is disabled";
		if (actor == target) return "the hunter itself";
		if (IsSociallyTrusted(actor, target)) return "socially protected";
		if (!CanObserveTarget(actor, target)) return "not currently observed";
		if (target.State.IsDead()) return "already dead";
		if (RaceInLineage(target.Race, Hunting.ExcludedRaces)) return "excluded lineage";
		if (Hunting.IncludedRaces.Count > 0 && !RaceInLineage(target.Race, Hunting.IncludedRaces)) return "outside included lineages";
		var people = Hunting.ClassificationProgId == 0
			? !AnimalLineageHelper.IsAnimal(target)
			: Gameworld.FutureProgs.Get(Hunting.ClassificationProgId)?.ExecuteBool(true, actor, target) != false;
		if (people && (Hunting.People == AnimalPeoplePreyPolicy.Never ||
		              Hunting.People == AnimalPeoplePreyPolicy.Desperate && !IsStarving(actor))) return "people policy";
		var size = target.CurrentContextualSize(SizeContext.Scan) - actor.CurrentContextualSize(SizeContext.Scan);
		if (Hunting.MinimumSizeDifference is { } min && size < min ||
		    Hunting.MaximumSizeDifference is { } max && size > max) return "outside apparent size bounds";
		if (Hunting.EligibilityProgId != 0 &&
		    Gameworld.FutureProgs.Get(Hunting.EligibilityProgId)?.ExecuteBool(false, actor, target) != true) return "eligibility prog";
		if (!PredatorAIHelpers.CouldEatAfterKilling(actor, target)) return "inedible prey";
		if (!PredatorAIHelpers.IsHungry(actor) || NpcSurvivalAIHelpers.IsThirsty(actor)) return "survival needs";
		if (!CharacterState.Able.HasFlag(actor.State)) return "hunter cannot act";
		if (!continuing && actor.Combat is not null) return "already in combat";
		if (!continuing && !actor.CanEngage(target)) return actor.WhyCannotEngage(target);
		if (HuntingAttackPermissionRejection(actor, target) is { } permission) return permission;
		if (!MovementStrategyHandler.CanReachTargetLayer(this, actor, target.RoomLayer)) return "unreachable layer";
		if (Hunting.Opening == AnimalHuntOpening.TrapWait && !continuing && !IsCapturedPrey(actor, target) &&
		    !(Hunting.Opportunistic && target.State.IsDisabled())) return "waiting for captured or helpless prey";
		return null;
	}

	private static string? HuntingAttackPermissionRejection(ICharacter actor, ICharacter target)
	{
		// Apply the ordinary attack permissions independently of the observable risk assessment.
		if (!actor.Race.CombatSettings.CanAttack) return "hunter's race cannot attack";
		if (!actor.Race.CombatSettings.CanUseWeapons && actor.CombatSettings.WeaponUsePercentage >= 1.0 &&
		    !actor.CombatSettings.FallbackToUnarmedIfNoWeapon) return "combat settings prohibit unarmed attacks";
		if (actor.Body.EffectsOfType<IPacifismEffect>().Any(x => x.IsSuperPeaceful)) return "hunter is too peaceful to attack";
		var strategy = CombatStrategyFactory.GetStrategy(actor.CombatStrategyMode);
		return strategy.WillAttack(actor, target) ? null : strategy.WhyWontAttack(actor, target);
	}

	private (bool Ready, string Reason) HuntingReadiness()
	{
		if (!Hunting.Enabled) return (true, string.Empty);
		if (Hunting.MinimumSizeDifference > Hunting.MaximumSizeDifference)
			return (false, "hunting size bounds are inverted");
		if (Hunting.Followup == AnimalHuntFollowup.Extract && !Hunting.PreferredLayer.HasValue)
			return (false, "extraction requires a preferred hunting layer");
		foreach (var (id, name, type) in new[]
		         {
			         (Hunting.ClassificationProgId, "classification", ProgVariableTypes.Boolean),
			         (Hunting.EligibilityProgId, "eligibility", ProgVariableTypes.Boolean),
			         (Hunting.PreferenceProgId, "preference", ProgVariableTypes.Number)
		         })
		{
			if (id == 0) continue;
			var prog = Gameworld.FutureProgs.Get(id);
			if (prog is null || !prog.ReturnType.CompatibleWith(type) ||
			    !prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character]))
				return (false, $"hunting {name} prog must return {type.DescribeEnum()} and accept hunter and prey characters");
		}
		return (true, string.Empty);
	}

	private static bool IsCapturedPrey(ICharacter actor, ICharacter target)
	{
		var restraints = target.EffectsOfType<TrapRestraintEffect>().ToList();
		// A one-use trap can be spent and removed before its (possibly delayed) payload fires.
		if (target.Location == actor.Location && restraints.Any(x => x.CreatorId == actor.Id &&
		    x.OriginCellId == actor.Location.Id)) return true;
		// Old saved restraints have no creator receipt; require the original local trap for those.
		var legacyIds = restraints.Where(x => x.CreatorId == 0).Select(x => x.TrapInstanceId).ToHashSet();
		return LocalOwnedTraps(actor).Any(x => legacyIds.Contains(x.InstanceId));
	}

	internal static IEnumerable<ITrap> LocalOwnedTraps(ICharacter actor) => actor.Location.EffectsOfType<ITrap>()
		.Concat(actor.Location.GameItems.SelectMany(x => x.EffectsOfType<ITrap>()))
		.Where(x => x.CreatorId == actor.Id);

	internal AnimalAssessmentResult AssessPrey(ICharacter actor, ICharacter target)
	{
		// Callers must establish perception first. Never inspect target health totals, skills, stamina or drugs.
		var visible = actor.Location.Characters.Where(x => !ReferenceEquals(x, actor) && actor.CanSee(x)).ToList();
		var allies = visible.Count(x => CharacterState.Able.HasFlag(x.State) && x.CombatTarget == target &&
		                               IsSociallyTrusted(actor, x));
		var threats = visible.Count(x => !ReferenceEquals(x, target) && x.CombatTarget == actor);
		var injury = target.VisibleWounds(actor, WoundExaminationType.Glance)
			.Select(x => (int)x.Severity / 8.0).DefaultIfEmpty(0).Max();
		var vulnerability = target.State.IsDisabled() ? 1.0 :
			!target.PositionState.Upright ? 0.35 : IsCapturedPrey(actor, target) ? 0.25 : 0.0;
		var tactic = IsCapturedPrey(actor, target) ||
		             Hunting.Opening == AnimalHuntOpening.Ambush && actor.AffectedBy<IHideEffect>() &&
		             (!Hunting.PreferredLayer.HasValue || actor.RoomLayer == Hunting.PreferredLayer) ? 1.0 : 0.0;
		return AnimalThreatAssessment.Instance.Assess(new AnimalAssessmentInput(
			actor.CurrentContextualSize(SizeContext.Scan) - target.CurrentContextualSize(SizeContext.Scan),
			injury, vulnerability, tactic, allies - threats,
			target.Body.WieldedItems.Any(x => actor.CanSee(x) && x.GetItemType<IMeleeWeapon>() is not null) ? 1 : 0,
			1 - actor.HealthStrategy.CurrentHealthPercentage(actor),
			actor.MaximumStamina > 0 ? 1 - actor.CurrentStamina / actor.MaximumStamina : 0), Hunting.Weights, Hunting.ConfidenceBias);
	}

	private double PreyPreference(ICharacter actor, ICharacter target)
	{
		var preference = Hunting.PreferredRaces.Where(x => RaceInLineage(target.Race, [x.Key]))
			.Select(x => x.Value).DefaultIfEmpty(0).Max();
		if (Hunting.PreferenceProgId != 0)
		{
			preference += Gameworld.FutureProgs.Get(Hunting.PreferenceProgId)?.ExecuteDouble(actor, target) ?? 0;
		}
		return double.IsFinite(preference) ? preference : 0;
	}

	internal IEnumerable<ICharacter> RankPrey(ICharacter actor, IEnumerable<ICharacter> candidates)
	{
		var eligible = candidates.DistinctPhysicalInstances().Where(x => PreyRejection(actor, x) is null)
			.Select(x => new { Target = x, Score = AssessPrey(actor, x).Score, Preference = PreyPreference(actor, x),
				Distance = actor.DistanceBetween(x, (uint)Hunting.PursuitRange), Size = x.CurrentContextualSize(SizeContext.Scan) })
			.Where(x => x.Score >= HuntThreshold(actor, false) && x.Distance >= 0 && x.Distance <= Hunting.PursuitRange).ToList();
		return (Hunting.Selection switch
		{
			AnimalPreySelection.Nearest => eligible.OrderBy(x => x.Distance).ThenByDescending(x => x.Score).ThenByDescending(x => x.Preference),
			AnimalPreySelection.LargestManageable => eligible.OrderByDescending(x => x.Size).ThenByDescending(x => x.Score).ThenByDescending(x => x.Preference),
			_ => eligible.OrderByDescending(x => x.Score).ThenByDescending(x => x.Preference).ThenBy(x => x.Distance)
		}).ThenBy(x => x.Target.Id).Select(x => x.Target);
	}

	public string HuntingDiagnostic(ICharacter actor, ICharacter? target, IPerceiver voyeur)
	{
		var hunt = actor.EffectsOfType<AnimalHuntEffect>().FirstOrDefault(x => x.AiId == Id);
		var sb = new StringBuilder(ShowHunting(voyeur));
		sb.AppendLine();
		sb.AppendLine($"Hunt phase: {hunt?.Phase.ToString() ?? "idle"}; target: {hunt?.TargetId.ToString("N0", voyeur) ?? "none"}; deadline: {hunt?.Deadline.ToString("O") ?? "none"}");
		sb.AppendLine($"Combat: {(actor.Combat is null ? "none" : "active")}; combat target: {actor.CombatTarget?.Id.ToString("N0", voyeur) ?? "none"}; mode: {actor.CombatStrategyMode.DescribeEnum()}; melee: {actor.MeleeRange.ToColouredString()}");
		foreach (var grapple in actor.EffectsOfType<IGrappling>())
			sb.AppendLine($"Grapple target: {grapple.Target.Id.ToString("N0", voyeur)}; controlled: {grapple.TargetEffect.UnderControl.ToColouredString()}");
		if (target is null) return sb.AppendLine("No observed assessment target.").ToString();
		sb.AppendLine($"Candidate: #{target.Id.ToString("N0", voyeur)}");
		sb.AppendLine($"Eligibility: {PreyRejection(actor, target, hunt is not null) ?? "eligible"}");
		if (!CanObserveTarget(actor, target)) return sb.AppendLine("No assessment without a current observation.").ToString();
		var assessment = AssessPrey(actor, target);
		sb.AppendLine($"Assessment: {assessment.Score.ToString("N1", voyeur)}; start {HuntThreshold(actor, false).ToString("N1", voyeur)}, abandon {HuntThreshold(actor, true).ToString("N1", voyeur)}");
		sb.AppendLine(assessment.Contributions.Select(x => $"{x.Key}: {x.Value.ToString("+0.0;-0.0;0", voyeur)}").ListToCommaSeparatedValues());
		return sb.ToString();
	}

	private string ShowHunting(IPerceiver voyeur) =>
		$"Hunting: {Hunting.Enabled.ToColouredString()}; people: {Hunting.People.DescribeEnum().ColourName()}; selection: {Hunting.Selection.DescribeEnum().ColourName()}\n" +
		$"Opening: {Hunting.Opening.DescribeEnum()}; followup: {Hunting.Followup.DescribeEnum()}; layer: {Hunting.PreferredLayer?.DescribeEnum() ?? "any"}; opportunity: {Hunting.Opportunistic.ToColouredString()}\n" +
		$"Assessment start/abandon: {Hunting.EngageThreshold.ToString("N1", voyeur)}/{Hunting.AbandonThreshold.ToString("N1", voyeur)}; starving adjustment: {Hunting.StarvationAdjustment.ToString("N1", voyeur)}; confidence: {Hunting.ConfidenceBias.ToString("N1", voyeur)}\n" +
		$"Pursuit: {Hunting.PursuitRange.ToString("N0", voyeur)} cells, {Hunting.PursuitTimeout.Describe(voyeur)}; lost: {Hunting.LostTimeout.Describe(voyeur)}\n" +
		$"Size differences: {Hunting.MinimumSizeDifference?.ToString(voyeur) ?? "any"} to {Hunting.MaximumSizeDifference?.ToString(voyeur) ?? "any"}; include: {Hunting.IncludedRaces.Select(x => x.ToString(voyeur)).ListToCommaSeparatedValues()}; exclude: {Hunting.ExcludedRaces.Select(x => x.ToString(voyeur)).ListToCommaSeparatedValues()}\n" +
		$"Preference lineages: {Hunting.PreferredRaces.Select(x => $"{x.Key}: {x.Value}").ListToCommaSeparatedValues()}; progs (classification/eligibility/preference): {Hunting.ClassificationProgId}/{Hunting.EligibilityProgId}/{Hunting.PreferenceProgId}\n" +
		$"Weights: {Hunting.Weights.Select(x => $"{x.Key}: {x.Value.ToString("N1", voyeur)}").ListToCommaSeparatedValues()}";
}
