using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Magic.SpellTriggers;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.Planes;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private const int AreaInputBound = 512;
	private sealed record AreaCandidate(ICharacter Target, IBody Body, SpellAreaTarget Receipt);
	private sealed record AreaPlan(ControlledSpellArea Policy, ICell Location, IBody CasterBody,
		long CasterInstance, RoomLayer CasterLayer, IReadOnlyList<AreaCandidate> Candidates)
	{
		public ResolvedMagicCastingArea Receipt => new(Policy.Scope, Policy.Selection, Policy.Identity,
			Policy.MaximumApplications, Array.AsReadOnly(Candidates.Select(x => x.Receipt).ToArray()));
	}

	private AreaPlan PrepareArea(MagicCastingIntent intent, MagicSpell spell)
	{
		if (spell.GradeProfile?.Area is not { } policy || spell.Trigger is not CastingTriggerCharacter trigger)
			throw new InvalidOperationException("Area casting is unavailable until this spell has a valid explicit area policy and native character trigger.");
		if (!intent.Targets.Trim().EqualTo("here"))
			throw new InvalidOperationException("This area policy requires exactly on here; selected-character, room and exit targeting retain their own native contracts.");
		if (intent.Actor.Location.RouteDefinition is not null && policy.Scope == SpellAreaScope.RoomCharacters)
			throw new InvalidOperationException("Whole-room area scope is unavailable in a spatial RouteCell; explicitly author ImmediateCharacters for its native local neighbourhood.");
		var actor = intent.Actor;
		var snapshot = AreaLocalCharacters(actor, policy).Take(AreaInputBound + 1).ToArray();
		if (snapshot.Length > AreaInputBound)
			throw new InvalidOperationException("The area exceeds the 512 physical input bound; no payment was made.");
		var candidates = snapshot.Where(x => AreaEligible(actor, x, policy, trigger))
			.OrderBy(CharacterInstanceIdentityComparer.IdentityId)
			.ThenBy(x => ReferenceEquals(x, actor) ? 0 : 1)
			.ThenBy(x => CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(actor, x) ? 0 : 1)
			.ThenBy(x => x.Body.Id).ThenBy(x => x.InstanceId)
			.DistinctBy(x => policy.Identity == SpellAreaIdentity.PhysicalBody ? x.Body.Id : CharacterInstanceIdentityComparer.IdentityId(x))
			.Select(x => new AreaCandidate(x, x.Body, new(CharacterInstanceIdentityComparer.IdentityId(x), x.InstanceId, x.Body.Id,
				CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(actor, x) ? policy.CasterDamageMultiplier : policy.OtherDamageMultiplier)))
			.ToArray();
		if (candidates.Length == 0) throw new InvalidOperationException("No eligible area targets remain; no payment was made.");
		// Refuse overflow instead of silently protecting arbitrary late-sorted actors.
		if (candidates.Length > policy.MaximumTargets)
			throw new InvalidOperationException("The eligible area exceeds this spell's authored maximum targets; no payment was made.");
		return new(policy, actor.Location, actor.Body, actor.InstanceId, actor.RoomLayer, Array.AsReadOnly(candidates));
	}

	private static IEnumerable<ICharacter> AreaLocalCharacters(ICharacter actor, ControlledSpellArea policy) =>
		policy.Scope == SpellAreaScope.ImmediateCharacters
			? actor.Location.CharactersInImmediateVicinity(actor, sameLayerOnly: false)
			: actor.Location.Characters;

	private bool AreaEligible(ICharacter actor, ICharacter target, ControlledSpellArea policy, CastingTriggerCharacter trigger)
	{
		var body = target.Body; var casterBody = actor.Body; var location = actor.Location;
		var instance = target.InstanceId; var identity = CharacterInstanceIdentityComparer.IdentityId(target); var layer = actor.RoomLayer;
		if (!AreaPhysicalEligible(actor, target, policy, trigger)) return false;
		if (!trigger.AllowsTarget(target, actor) ||
			policy.FilterProgId != 0 && _world.FutureProgs.Get(policy.FilterProgId)?.Execute<bool?>(target, actor) != true) return false;
		// Progs are executable callbacks. A true return cannot authorise a replacement physical target.
		return ReferenceEquals(target.Body, body) && ReferenceEquals(actor.Body, casterBody) &&
			ReferenceEquals(actor.Location, location) && actor.RoomLayer == layer && target.InstanceId == instance &&
			CharacterInstanceIdentityComparer.IdentityId(target) == identity && AreaPhysicalEligible(actor, target, policy, trigger);
	}

	private static bool AreaPhysicalEligible(ICharacter actor, ICharacter target, ControlledSpellArea policy, CastingTriggerCharacter trigger)
	{
		if (target.Body is null || target.Body.Id <= 0 || target.InstanceId <= 0 || CharacterInstanceIdentityComparer.IdentityId(target) <= 0 ||
			!ReferenceEquals(actor.Location, target.Location) || policy.SameLayer && target.RoomLayer != actor.RoomLayer ||
			policy.ExcludeStaff && target.IsAdministrator() ||
			!actor.CanInteractPlanar(target, PlanarInteractionKind.Magic) ||
			policy.Plane == SpellAreaPlane.PhysicalAndMagicReach && !actor.CanInteractPlanar(target, PlanarInteractionKind.Physical) ||
			policy.GroundedOnly && !AreaGrounded(target)) return false;
		var self = CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(actor, target);
		if (self ? !policy.IncludeCaster || !trigger.CanTargetSelf : actor.IsAlly(target) ? !policy.IncludeAllies : !policy.IncludeOthers)
			return false;
		return true;
	}

	// Deliberately labelled as a provisional native mapping in authored source examples.
	// Specific races, protections, mounts and falls belong to their separately authored predicates/effects.
	private static bool AreaGrounded(ICharacter actor) => actor.RoomLayer == RoomLayer.GroundLevel &&
		actor.PositionState is not null and not (PositionFlying or PositionFloatingInZeroGravity or PositionFloatingInWater or
			PositionSwimming or PositionClimbing or PositionHanging);

	private bool AreaStillEligible(ICharacter actor, AreaCandidate candidate, AreaPlan plan, MagicSpell spell)
	{
		if (!CapturedStructureMatches() || spell.Trigger is not CastingTriggerCharacter trigger ||
			!AreaEligible(actor, candidate.Target, plan.Policy, trigger) || !CapturedStructureMatches()) return false;
		var current = AreaLocalCharacters(actor, plan.Policy).Take(AreaInputBound + 1).ToArray();
		return current.Length <= AreaInputBound && current.Any(x => ReferenceEquals(x, candidate.Target));

		bool CapturedStructureMatches() => ReferenceEquals(actor.Body, plan.CasterBody) && actor.InstanceId == plan.CasterInstance &&
			ReferenceEquals(actor.Location, plan.Location) && actor.RoomLayer == plan.CasterLayer &&
			ReferenceEquals(candidate.Target.Body, candidate.Body) && candidate.Target.Body.Id == candidate.Receipt.BodyId &&
			candidate.Target.InstanceId == candidate.Receipt.InstanceId &&
			CharacterInstanceIdentityComparer.IdentityId(candidate.Target) == candidate.Receipt.CharacterId;
	}

	private IReadOnlyList<AreaCandidate> SelectAreaApplications(AreaPlan plan)
	{
		var pool = plan.Candidates.ToList();
		if (plan.Policy.Selection == SpellAreaSelection.Ordered)
			return Array.AsReadOnly(pool.Take(plan.Policy.MaximumApplications).ToArray());
		List<AreaCandidate> result = [];
		for (var i = 0; i < plan.Policy.MaximumApplications && pool.Count > 0; i++)
		{
			var index = _areaRandom(pool.Count);
			if (index < 0 || index >= pool.Count) throw new InvalidOperationException("Invalid area random selection.");
			result.Add(pool[index]);
			if (plan.Policy.Selection == SpellAreaSelection.RandomDistinct) pool.RemoveAt(index);
		}
		return result.AsReadOnly();
	}

	private static XElement AreaReceipt(AreaPlan plan, IReadOnlyList<AreaCandidate>? applications = null) =>
		new("Area", new XAttribute("scope", plan.Policy.Scope), new XAttribute("selection", plan.Policy.Selection),
			new XAttribute("identity", plan.Policy.Identity), new XAttribute("plane", plan.Policy.Plane),
			new XAttribute("cell", plan.Location.Id), new XAttribute("layer", (int)plan.CasterLayer),
			new XAttribute("maximumTargets", plan.Policy.MaximumTargets), new XAttribute("maximumApplications", plan.Policy.MaximumApplications),
			new XAttribute("caster", plan.Policy.IncludeCaster), new XAttribute("allies", plan.Policy.IncludeAllies),
			new XAttribute("others", plan.Policy.IncludeOthers), new XAttribute("sameLayer", plan.Policy.SameLayer),
			new XAttribute("grounded", plan.Policy.GroundedOnly), new XAttribute("excludeStaff", plan.Policy.ExcludeStaff),
			new XAttribute("filter", plan.Policy.FilterProgId), new XAttribute("provenance", plan.Policy.Provenance),
			plan.Candidates.Select(x => TargetElement("Candidate", x)), applications?.Select(x => TargetElement("Application", x)));

	private static XElement TargetElement(string name, AreaCandidate candidate) => new(name,
		new XAttribute("character", candidate.Receipt.CharacterId), new XAttribute("instance", candidate.Receipt.InstanceId),
		new XAttribute("body", candidate.Receipt.BodyId), new XAttribute("damage", candidate.Receipt.DamageMultiplier));
}
